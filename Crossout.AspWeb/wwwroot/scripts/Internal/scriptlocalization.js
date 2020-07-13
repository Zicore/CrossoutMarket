
var localizations = { data: null, loaded: false };

async function getLocalizations(category, language) {
    $.getJSON('/data/localization/frontend?category=' + category + '&l=' + language, function (data) {
        localizations.data = data;
        localizations.loaded = true;
        onLocalizationLoaded();
    });
}

function localize() {
    if (localizations.loaded) {
        $('.localize').each(function (i, e) {
            var fullname = e.dataset.locname;
            var locs = localizations.data;
            var loc = locs.find(x => x.fullname === fullname);

            var node = $(e);
            node.addClass('localization');
            if (loc !== undefined && loc !== null) {
                node.text(loc.localization);
                node.removeClass('localize');
                node.addClass('localized');
            }
            else {
                node.addClass('needs-localization');
            }
        });
    }
}

function localizeSingle(fullname, defaultLoc) {
    if (localizations.data !== null) {
        var loc = localizations.data.find(x => x.fullname === fullname);
        if (loc !== undefined && loc !== null)
            return loc.localization;
        else
            return defaultLoc;
    }
}