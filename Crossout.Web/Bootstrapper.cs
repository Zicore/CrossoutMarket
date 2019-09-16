using Crossout.Web.Auth;
using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Bootstrapper;
using Nancy.Conventions;
using Nancy.Diagnostics;
using Nancy.Gzip;
using Nancy.Security;
using Nancy.SimpleAuthentication;
using Nancy.TinyIoc;
using Newtonsoft.Json;
using SimpleAuthentication.Core;
using SimpleAuthentication.Core.Providers;
using ZicoreConnector.Zicore.Connector.Base;

namespace Crossout.Web
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        private IUserRepository userRepository;
        private UserMapper userMapper;

        public Bootstrapper()
        {
            SqlConnector sql = new SqlConnector(ConnectionType.MySql);
            sql.Open(WebSettings.Settings.CreateDescription());
            userRepository = new SqlUserRepository(sql);
            userMapper = new UserMapper(userRepository);
        }
        
        protected override void ConfigureConventions(NancyConventions nancyConventions)
        {
            base.ConfigureConventions(nancyConventions);
            nancyConventions.StaticContentsConventions.Clear();

            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("settings"));
            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("img"));
            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("content"));
            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("fonts"));
            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("scripts"));
            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddFile("/ads.txt","/Ads/ads.txt"));
        }

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);
            var googleProvider = new GoogleProvider(new ProviderParams { PublicApiKey = WebSettings.Settings.GoogleConsumerKey, SecretApiKey = WebSettings.Settings.GoogleConsumerSecret });
            googleProvider.AuthenticateRedirectionUrl = new System.Uri("https://accounts.google.com/o/oauth2/v2/auth");
            
            var authenticationProviderFactory = new AuthenticationProviderFactory();
            authenticationProviderFactory.AddProvider(googleProvider);
            container.Register<IAuthenticationCallbackProvider>(new CrossoutAuthenticationCallbackProvider(userRepository));
            container.Register<JsonSerializer, CustomJsonSerializer>();
        }

        protected override void ConfigureRequestContainer(TinyIoCContainer container, NancyContext context)
        {
            base.ConfigureRequestContainer(container, context);

            // Here we register our user mapper as a per-request singleton.
            // As this is now per-request we could inject a request scoped
            // database "context" or other request scoped services.
            container.Register<IUserMapper, UserMapper>(userMapper);
        }

        protected override void RequestStartup(TinyIoCContainer requestContainer, IPipelines pipelines, NancyContext context)
        {
            //pipelines.BeforeRequest.AddItemToStartOfPipeline(SecurityHooks.RequiresHttps(true, 443));
            SSLProxy.RewriteSchemeUsingForwardedHeaders(pipelines);
            // At request startup we modify the request pipelines to
            // include forms authentication - passing in our now request
            // scoped user name mapper.
            //
            // The pipelines passed in here are specific to this request,
            // so we can add/remove/update items in them as we please.
            var formsAuthConfiguration =
                new FormsAuthenticationConfiguration()
                {
                    RedirectUrl = "~/admin",
                    UserMapper = requestContainer.Resolve<IUserMapper>(),
                };

            FormsAuthentication.Enable(pipelines, formsAuthConfiguration);

        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            // Enable Compression with Settings
            var settings = new GzipCompressionSettings();
            settings.MinimumBytes = 1024;
            pipelines.EnableGzipCompression(settings);

            base.ApplicationStartup(container, pipelines);
        }

        protected override IRootPathProvider RootPathProvider
        {
            get { return new RootPathProvider(); }
        }
    }
}