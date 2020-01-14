﻿const defaultSettings = {
    'cookie-health': true,
    'dropdown': true,
    'length': 10,
    'data-grouping': true
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
    Cookies.set('settings', settingsBase64, { expires: 365 });
}

function readCookie() {
    try {
        var settingsBase64 = Cookies.get('settings');
        var settingsJSON = atob(settingsBase64);
        settings = JSON.parse(settingsJSON);
    }
    catch {
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
    catch {
        $('#settingsStringBase64').removeClass('is-valid');
        $('#settingsStringBase64').addClass('is-invalid');
        return false;
    }
}

function importCookie() {
    if (parseCookie()) {
        var value = $('#settingsStringBase64').val();
        Cookies.set('settings', value, { expires: 365 });
        readCookie();
    }
}

function resetSettings() {
    settings = defaultSettings;
    writeCookie();
}