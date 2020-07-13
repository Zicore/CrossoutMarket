$(document).ready(function () {
    applyLocalizationHelper();
});

function applyLocalizationHelper() {
    if (readSetting('localization-helper'))
        showLocalizationHelp();
}

function showLocalizationHelp() {
    $('.localization').each(function (i, e) {
        if ($(e).hasClass('localized')) {
            $(e).addClass('bg-green');
        }
        if ($(e).hasClass('needs-localization')) {
            $(e).addClass('bg-red');
        }
        $(e).tooltip({
            title: e.dataset.locname,
            trigger: 'manual'
        });
    });
    $('.localization').hover(function () {
        $('.localization').tooltip('hide');
        $(this).tooltip('show');
    });
}