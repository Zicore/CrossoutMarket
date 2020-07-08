$(document).ready(function () {
    if (readSetting('localization-helper'))
        showLocalizationHelp();
});

function showLocalizationHelp() {
    $('.localization').each(function (i, e) {
        if ($(e).hasClass('localized')) {
            $(e).addClass('bg-green');
        }
        if ($(e).hasClass('needs-localization')) {
            $(e).addClass('bg-red');
        }
        $(e).tooltip({
            title: e.dataset.locname
        });
    });
}