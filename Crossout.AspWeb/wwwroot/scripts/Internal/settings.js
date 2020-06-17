const defaultSettings = {
    'dark-mode': false,
    'timestamp-format-date': 'YYYY-MM-DD',
    'timestamp-format-time': 'HH:mm:ss',
    'full-size-table': false,
    'dropdown': true,
    'vertical-buttons': false,
    'legacy-filters': false,
    'range-filters': false,
    'export-buttons': false,
    'length': 10,
    'full-size-item-tabs': false,
    'data-grouping': true,
    'recipe-other-costs-amount': 1,
    'recipe-other-costs-sellprice': 0,
    'recipe-other-costs-buyprice': 0,
    'full-size-packs': false
};

var settings = {};

function readSetting(setting) {
    settings = $.extend({}, defaultSettings, settings);
    writeCookie();
    return settings[setting];
}

function writeSetting(setting, value) {
    settings = $.extend({}, defaultSettings, settings);
    settings[setting] = value;
    writeCookie();
}

function writeCookie() {
    var settingsJSON = JSON.stringify(settings);
    var settingsBase64 = btoa(settingsJSON);
    Cookies.set('settings', settingsBase64, { expires: 365, sameSite: 'strict' });
}

function readCookie() {
    try {
        var settingsBase64 = Cookies.get('settings');
        var settingsJSON = atob(settingsBase64);
        settings = JSON.parse(settingsJSON);
    }
    catch (err) {
        resetSettings();
    }
}

function exportCookie() {
    $('#settingsStringBase64').val(Cookies.get('settings'));
}

function parseCookie() {
    var value = $('#settingsStringBase64').val();
    try {
        var settingsBase64 = value;
        var settingsJSON = atob(settingsBase64);
        JSON.parse(settingsJSON);
        $('#settingsStringBase64').removeClass('is-invalid');
        $('#settingsStringBase64').addClass('is-valid');
        return true;
    }
    catch (err) {
        $('#settingsStringBase64').removeClass('is-valid');
        $('#settingsStringBase64').addClass('is-invalid');
        return false;
    }
}

function importCookie() {
    if (parseCookie()) {
        var value = $('#settingsStringBase64').val();
        Cookies.set('settings', value, { expires: 365, sameSite: 'strict' });
        readCookie();
    }
}

function resetSettings() {
    settings = defaultSettings;
    writeCookie();
}

function readFilterSettings() {
    if (readSetting('dropdown'))
        return 'dropdown';
    if (readSetting('vertical-buttons'))
        return 'vertical-buttons';
    if (readSetting('legacy-filters'))
        return 'legacy-filters';
    return 'dropdown';
}