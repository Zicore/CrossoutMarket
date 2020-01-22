var chartData = [];
var initialized = false;

function initTimeMarginCalc() {
    initialized = true;
    calculateTimeMargin();
}

function calculateTimeMargin() {
    if (!initialized)
        return;
    if (chart.rangeSelector === undefined) {
        return;
    }

    var maxSell = 0;
    var maxBuy = 0;
    var minSell = 0;
    var minBuy = 0;
    var maxMargin = 0;
    var minMargin = 0;


    var fromTime = chart.rangeSelector.minInput.HCTime;
    var toTime = chart.rangeSelector.maxInput.HCTime;

    console.log(fromTime + ' - ' + toTime);

    chartData[0].forEach(function (e, i) {
        var current = e[1];
        if (e[0] >= fromTime && e[0] <= toTime) {
            if (current > maxSell && current !== 0)
                maxSell = current;
            if (current < minSell && current !== 0 || minSell === 0)
                minSell = current;
        }
    });
    chartData[1].forEach(function (e, i) {
        if (e[0] >= fromTime && e[0] <= toTime) {
            var current = e[1];
            if (current > maxBuy && current !== 0)
                maxBuy = current;
            if (current < minBuy && current !== 0 || minBuy === 0)
                minBuy = current;
        }
    });

    maxMargin = Math.round((maxSell * 0.9 - minBuy) * 100) / 100;
    minMargin = Math.round((minSell * 0.9 - maxBuy) * 100) / 100;

    $('#maxSell').text(maxSell);
    $('#maxBuy').text(maxBuy);
    $('#minSell').text(minSell);
    $('#minBuy').text(minBuy);
    $('#maxMargin').text(maxMargin);
    $('#minMargin').text(minMargin);
}