

$('#reset').click(function (e) {
    $("#query").val('');
    $("#form").submit();
});

var recipeData = {
    loaded: false,
    data: {}
};


function updateTree(classname, recipe, uniqueid, show) {
    if (classname !== 'recipe-0') {
        $('.' + classname).each(function (i, obj) {
            var currentUniqueid = $(this).data('uniqueid');
            var currentParentUniqueid = $(this).data('parentuniqueid');
            var currentRecipe = $(this).data('recipe');
            var classname2 = 'recipe-' + $(this).data('recipe');
            if (currentParentUniqueid === uniqueid) {
                if (show) {
                    $(this).show();
                    //$('#shopping-list-wrapper').show();
                } else {
                    $(this).hide();
                    //$('#shopping-list-wrapper').hide();
                    $(this).find('button').removeClass('glyphicon-minus').addClass('glyphicon-plus');
                    window.updateTree(classname2, currentRecipe, currentUniqueid, show);
                }
            }
        });
    }
}

function toPrice(number) {
    return (number / 100.0).toFixed(2);
}

function toFixed(number) {
    return number.toFixed(2);
}

function getCookieOrDefault(name, defaultValue) {
    var cookieValue = Cookies.get(name);
    if (cookieValue !== undefined && !isNaN(cookieValue)) {
        return cookieValue;
    }
    return defaultValue;
}

function setCookieNumber(name, value) {
    if (!isNaN(value)) {
        Cookies.set(name, value);
    }
}

function updateSums(recipe, uniqueid) {

    $('#shopping-list > tbody').empty();

        $('.sum-row:visible').each(function (j, obj) {
            var sumuniqueid = $(this).data('uniqueid');

            var root = recipeData.data.recipe.recipe;
            var mainItem = findSumItem(root, sumuniqueid);
            var sumItem = mainItem.ingredientSum;
            var result = { items: new Array(), map: {}, shoppinglist: {} }

            if (sumItem !== null) {
                updateSum(root, mainItem, result, recipe);

                var sumBuy = 0;
                var sumSell = 0;
                var list = result.shoppinglist;
                var number;
                var item;
                var rec;
                for (var k in list) {
                    if (list.hasOwnProperty(k)) {
                        rec = list[k].item;
                        item = rec.item;
                        number = list[k].number;
                        var sell = item.sellPrice;
                        var buy = item.buyPrice;

                        sumSell += filterResourcePrice(item.id, sell) * number;
                        sumBuy += filterResourcePrice(item.id, buy) * number;
                    }
                }

                var sellPrice = mainItem.item.sellPrice * Math.max(mainItem.rootNumber, 1);
                var buyPrice = mainItem.item.buyPrice * Math.max(mainItem.rootNumber, 1);

                var sellFeePrice = sellPrice * 0.9;
                var buyFeePrice = buyPrice * 0.9;

                var sellProfit = sellFeePrice - sumSell;
                var buyProfit = buyFeePrice - sumBuy;
                var sellBuyProfit = sellFeePrice - sumBuy;

                var sellClass = sellProfit > 0 ? 'sum-pos' : 'sum-neg';
                var buyClass = buyProfit > 0 ? 'sum-pos' : 'sum-neg';
                var sellBuyClass = sellBuyProfit > 0 ? 'sum-pos' : 'sum-neg';

                // Please if someone has a way to avoid this mess without huge frameworks like angular or react message me :)

                $('#uniqueid-' + sumItem.uniqueId).find('.sum-sell-fee').text(toPrice(sellFeePrice));
                $('#uniqueid-' + sumItem.uniqueId).find('.sum-buy-fee').text(toPrice(buyFeePrice));

                $('#uniqueid-' + sumItem.uniqueId).find('.sum-sell').text(toPrice(-sumSell));
                $('#uniqueid-' + sumItem.uniqueId).find('.sum-buy').text(toPrice(-sumBuy));

                $('#uniqueid-' + sumItem.uniqueId).find('.sum-sell-diff').removeClass('sum-neg').removeClass('sum-pos').addClass(sellClass).text(toPrice(sellProfit));
                $('#uniqueid-' + sumItem.uniqueId).find('.sum-buy-diff').removeClass('sum-neg').removeClass('sum-pos').addClass(buyClass).text(toPrice(buyProfit));
                $('#uniqueid-' + sumItem.uniqueId).find('.sum-sell-buy-diff').removeClass('sum-neg').removeClass('sum-pos').addClass(sellBuyClass).text(toPrice(sellBuyProfit));

                if (mainItem.uniqueId === root.uniqueId) {
                    for (var key in result.shoppinglist) {
                        if (result.shoppinglist.hasOwnProperty(key)) {
                            rec = result.shoppinglist[key].item;
                            item = rec.item;
                            number = result.shoppinglist[key].number;
                            $('#shopping-list').append(
                                '<tr data-item-id="' +
                                item.id +
                                '"><td>' +
                                htmlName(item) +
                                '</td><td>' +
                                htmlRarity(item) +
                                '</td><td>' +
                                htmlNumberInput(number, 'input-number-' + item.id) +
                                '</td><td>' +
                                htmlPriceInput(toPrice(item.sellPrice), 'input-sell-' + item.id) +
                                '</td><td>' +
                                '' +
                                '</td><td>' +
                                htmlPriceInput(toPrice(item.buyPrice), 'input-buy-' + item.id) +
                                '</td></tr>');
                            $('#input-sell-' + item.id).on('input',
                                function (e) {
                                    calculateShoppingList(root, result.shoppinglist);
                                });
                            $('#input-buy-' + item.id).on('input',
                                function (e) {
                                    calculateShoppingList(root, result.shoppinglist);
                                });
                            $('#input-number-' + item.id).on('input',
                                function (e) {
                                    calculateShoppingList(root, result.shoppinglist);
                                });
                        }
                    }

                    $('#shopping-list').append(
                                '<tr data-item-id="' +
                                "workbench" +
                                '"><td>' +
                                htmlShoppingListTitle("Other costs") +
                                '</td><td>' +
                                "" +
                                '</td><td>' +
                                htmlNumberInput(getCookieOrDefault('workbench-number',1), 'input-number-workbench') +
                                '</td><td>' +
                                htmlPriceInput(getCookieOrDefault('workbench-sellprice',0), 'input-sell-workbench') +
                                '</td><td>' +
                                '' +
                                '</td><td>' +
                                htmlPriceInput(getCookieOrDefault('workbench-buyprice',0), 'input-buy-workbench') +
                                '</td></tr>');

                    $('#input-number-workbench').on('input',
                    function (e) {
                        calculateShoppingList(root, result.shoppinglist);
                    });
                    $('#input-sell-workbench').on('input',
                        function (e) {
                            calculateShoppingList(root, result.shoppinglist);
                        });
                    $('#input-buy-workbench').on('input',
                    function (e) {
                        calculateShoppingList(root, result.shoppinglist);
                    });
                    

                    $('#shopping-list').append(
                        '<tr data-item-id="' +
                        root.item.id +
                        '"><td>' +
                        htmlName(root.item) +
                        '</td><td>' +
                        htmlRarity(root.item) +
                        '</td><td>' +
                        '' +
                        '</td><td>' +
                        htmlPriceSum(toPrice(0), 'sell', root.item.id) +
                        '</td><td>' +
                        '' +
                        '</td><td>' +
                        htmlPriceSum(toPrice(0), 'buy', root.item.id) +
                        '</td></tr>');

                    calculateShoppingList(root, result.shoppinglist);
                }
            }
        });
    
}

function calculateShoppingList(root, list) {
    var sumSell = 0;
    var sumBuy = 0;


    for (var key in list) {
        if (list.hasOwnProperty(key)) {
            var rec = list[key].item;
            var item = rec.item;
            var number = parseInt($('#input-number-' + item.id).val());
            var sell = parseFloat($('#input-sell-' + item.id).val());
            var buy = parseFloat($('#input-buy-' + item.id).val());

            sumSell += filterResourcePrice(item.id, sell) * number;
            sumBuy += filterResourcePrice(item.id, buy) * number;
        }
    }

    var number = parseInt($('#input-number-workbench').val());
    var sell = parseFloat($('#input-sell-workbench').val());
    var buy = parseFloat($('#input-buy-workbench').val());

    setCookieNumber('workbench-number', number);
    setCookieNumber('workbench-sellprice', sell);
    setCookieNumber('workbench-buyprice', buy);

    sumSell += sell * number;
    sumBuy += buy * number;

    var sellPrice = (root.item.sellPrice * 0.9) / 100.0;
    var buyPrice = (root.item.buyPrice * 0.9) / 100.0;

    var sellProfit = sellPrice - sumSell;
    var buyProfit = buyPrice - sumBuy;
    var sellBuyProfit = sellPrice - sumBuy;

    var sellClass = sellProfit > 0 ? 'sum-pos' : 'sum-neg';
    var buyClass = buyProfit > 0 ? 'sum-pos' : 'sum-neg';
    var sellBuyClass = sellBuyProfit > 0 ? 'sum-pos' : 'sum-neg';

    $('#sum-sell-' + root.item.id).text(toFixed(-sumSell));
    $('#sum-buy-' + root.item.id).text(toFixed(-sumBuy));

    $('#sum-fee-sell-' + root.item.id).text(toFixed(sellPrice));
    $('#sum-fee-buy-' + root.item.id).text(toFixed(buyPrice));

    $('#sum-diff-sell-' + root.item.id).removeClass('sum-neg').removeClass('sum-pos').addClass(sellClass).text(toFixed(sellProfit));
    $('#sum-diff-buy-' + root.item.id).removeClass('sum-neg').removeClass('sum-pos').addClass(buyClass).text(toFixed(buyProfit));

    $('#sum-sell-buy-diff-' + root.item.id).removeClass('sum-neg').removeClass('sum-pos').addClass(sellBuyClass).text(toFixed(sellBuyProfit));
}

var ResourceNumbers =
{
    43: true, //Copper x100
    53: true, //Scrap x100
    85: true, //Wires x100
    119: true, //Coupons x100
    168: true, //Electronics x100
    330: true, //Taler x100
    337: true, //Uran x100
    522: true //Sweets x100
};

function filterResourcePrice(id, value) {
    if (id in ResourceNumbers) {
        return value / 100.0;
    }
    return value;
}

function htmlShoppingListTitle(title) {
    return '<div class="shopping-list-title">' + title + '</div>';
}

// Ugh...
function htmlName(item) {
    return '<div class="clearfix content-heading">' +
        '<div class="clearfix vertical-center pull-left">' +
        '<div>' +
        '<a href="/item/' +
        item.id +
        '">' +
        '<img style="margin-right: 8px; height: 32px;" src="/img/items/' +
        item.image +
        '" /></a>' +
        '</div>' +
        '</div>' +
        '<a href="/item/' +
        item.id +
        '" style="font-weight: bold;">' +
        item.name +
        '</a>' +
        '<div style="font-size: 11px; font-weight: bold;">' +
        item.typeName +
        '</div>' +
        '</div>';
}

function htmlRarity(item) {
    return '<span class="label label-' + item.rarityName + '">' + item.rarityName + '</span>';
}

function htmlNumber(value) {
    return '<div class="label-md rec-right">' + value + '</div>';
}

function htmlPriceSum(value, side, id) {
    var r = '<div class="label-md pull-left">' +
        'Price -10 %' +
        '</div>' +
        '<div class="recipe-price label-md rec-right">' +
        '<div class="text-right sum-value" id="sum-fee-' +
        side +
        '-' +
        id +
        '">' +
        '</div>' +
        '<img height="14" src="/img/Coin.png" />' +
        '</div>' +
        '<div class="label-md pull-left">' +
        'Cost' +
        '</div>' +
        '<div class="recipe-price label-md rec-right">' +
        '<div class="text-right sum-value" id="sum-' +
        side +
        '-' +
        id +
        '">' +
        '</div>' +
        '<img height="14" src="/img/Coin.png" />' +
        '</div>' +
        '<div class="label-md pull-left">' +
        'Profit' +
        '</div>' +
        '<div class="recipe-price label-md rec-right">' +
        '<div class="text-right sum-value" id="sum-diff-' +
        side +
        '-' +
        id +
        '">' +
        '</div>' +
        '<img height="14" src="/img/Coin.png" />' +
        '</div>';
    if (side === 'sell') {
        r += '<div class="label-md pull-left">' +
            'Sell-Buy Profit' +
            '</div>' +
            '<div class="recipe-price label-md rec-right">' +
            '<div class="text-right sum-value" id="sum-sell-buy-diff-' +
            id +
            '">' +
            '</div>' +
            '<img height="14" src="/img/Coin.png" />' +
            '</div>';
    }

    return r;
}

function htmlNumberInput(value, id) {
    return '<div class="recipe-price label-md rec-right"><input class="text-right" id="' +
        id +
        '" type="text" value="' +
        value +
        '"></div>';
}

function htmlPriceInput(value, id) {
    return '<div class="recipe-price label-md rec-right"><input class="text-right" id="' +
        id +
        '" type="text" value="' +
        value +
        '"><img height="14" src="/img/Coin.png" /></div>';
}

// Maybe someone can make this easier
function updateSum(root, item, result, recipe) {
    var valueSet = false;
    var foundItem = null;
    for (var i = 0; i < item.ingredients.length; i++) {
        updateSum(root, item.ingredients[i], result, recipe);
        var subItem = item.ingredients[i];
        if (!result.map.hasOwnProperty(subItem.uniqueId)) {
            if ($('#uniqueid-' + subItem.uniqueId).is(":visible")) {
                if (!subItem.issumrow) {
                    result.items.push({
                        //sell: subItem.item.sellPrice * Math.max(1, subItem.RootNumber),
                        //buy: subItem.item.buyPrice * Math.max(1, subItem.RootNumber),
                        item: subItem
                    });
                    if (result.shoppinglist.hasOwnProperty(subItem.item.id)) {
                        result.shoppinglist[subItem.item.id].number += subItem.rootNumber;
                    } else {
                        result.shoppinglist[subItem.item.id] = { number: subItem.rootNumber, item: subItem }
                    }
                    valueSet = true;
                    foundItem = subItem;
                }
            }
        }
    }
    
    if (valueSet && foundItem != null) {
        result.map[foundItem.parentUniqueId] = true;
    }
}

function findSumItem(item, uniqueid) {
    if (item.ingredientSum !== null && item.ingredientSum.uniqueId === uniqueid) {
        return item;
    }

    for (var i = 0; i < item.ingredients.length; i++) {
        var rs = findSumItem(item.ingredients[i], uniqueid);
        if (rs) return rs;
    }
    return null;
}