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
    var averageSell = 0;
    var averageBuy = 0;
    var averageMargin = 0;


    var fromTime = chart.rangeSelector.minInput.HCTime;
    var toTime = chart.rangeSelector.maxInput.HCTime;

    var counter = 0;
    chartData[0].forEach(function (e, i) {
        if (e[0] >= fromTime && e[0] <= toTime) {
            counter++;
            var current = e[1];
            if (current > maxSell && current !== 0)
                maxSell = current;
            if (current < minSell && current !== 0 || minSell === 0)
                minSell = current;
            averageSell = averageSell + current;
        }
    });
    averageSell = averageSell / counter;

    counter = 0;
    chartData[1].forEach(function (e, i) {
        if (e[0] >= fromTime && e[0] <= toTime) {
            counter++;
            var current = e[1];
            if (current > maxBuy && current !== 0)
                maxBuy = current;
            if (current < minBuy && current !== 0 || minBuy === 0)
                minBuy = current;
            averageBuy = averageBuy + current;
        }
    });
    averageBuy = averageBuy / counter;

    maxMargin = Math.round((maxSell * 0.9 - minBuy) * 100) / 100;
    minMargin = Math.round((minSell * 0.9 - maxBuy) * 100) / 100;
    averageSell = Math.round(averageSell * 100) / 100;
    averageBuy = Math.round(averageBuy * 100) / 100;
    averageMargin = Math.round((averageSell * 0.9 - averageBuy) * 100) / 100;

    $('#maxSell').text(maxSell);
    $('#maxBuy').text(maxBuy);
    $('#minSell').text(minSell);
    $('#minBuy').text(minBuy);
    $('#maxMargin').text(maxMargin);
    $('#minMargin').text(minMargin);
    $('#averageSell').text(averageSell);
    $('#averageBuy').text(averageBuy);
    $('#averageMargin').text(averageMargin);
}