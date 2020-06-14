
//craftingCalcWrapper

// VARS
var craftingCalcData = {
    data: {},
    loaded: false
};

var craftingCalc = {
    tree: {
        topToBottom: [],
        visible: []
    },
    calc: {
        entries: [],
        sum: 0
    }
};

var snapshots = [];
var selectedSnapshot = 1;
var snapshotDeleteConfirmation = false;

// INIT
$(document).ready(function () {

});

function onDataLoaded() {
    buyOrCraftDecider(craftingCalcData.data.recipe.recipe);
    console.log(craftingCalcData.data.recipe.recipe.itemCost);
    mapData();
    setDefaultTree();
    makeSnapshot('Current');
    readSnapshotSave();
    drawCalculator();
}

function setDefaultTree() {
    var defaultShownLayer = 1;
    craftingCalc.tree.topToBottom.forEach(function (e, i) {
        if (e.depth >= defaultShownLayer && e.hasIngredients)
            collapseRecipe(e.recipeId, true);
    });
}

// MAP
function mapData() {
    var recipe = craftingCalcData.data.recipe.recipe;
    var currentDepth = 0;
    recipe.number = 1;
    mapIngredient(recipe, null, recipe, currentDepth);
}

// ToDo: Unique IDs anstatt Rezept IDs!!!
function mapIngredient(root, rootDisplayIngredient, ingredient, currentDepth) {
    var depth = currentDepth + 1;

    var displayIngredient = {
        itemId: ingredient.item.id,
        name: ingredient.item.availableName,
        show: true,
        expanded: true,
        depth: currentDepth,
        recipeId: ingredient.id,
        hasIngredients: false,
        bundleAmount: Math.max(ingredient.item.amount, 1),
        amount: ingredient.number,
        totalAmount: calculateTotalAmount(ingredient.number, Math.max(displayIngredient ? displayIngredient.amount : 1, 1), rootDisplayIngredient),
        rootAmount: root.number,
        rootEffectiveAmount: root.number / Math.max(root.item.craftingResultAmount, 1),
        craftResultAmount: Math.max(ingredient.item.craftingResultAmount, 1),
        craftEffectiveAmount: ingredient.number / Math.max(ingredient.item.craftingResultAmount, 1),
        sellPrice: ingredient.item.sellPrice,
        buyPrice: ingredient.item.buyPrice,
        customPrice: 0,
        usedPrice: 'buy',
        totalPrice: 0,
        usedSellPrice: 'sell',
        rootDisplayIngredient: rootDisplayIngredient,
        craftVsBuy: ingredient.item.craftVsBuy
    };
    var ingredients = ingredient.ingredients;
    if (ingredients.length > 0)
        displayIngredient.hasIngredients = true;
    craftingCalc.tree.topToBottom.push(displayIngredient);
    ingredients.forEach(function (e, i) {
        mapIngredient(ingredient, displayIngredient, e, depth);
    });
}

// Optimal Route Calculator
function buyOrCraftDecider(itemObject) {
    if (itemObject.item.craftingResultAmount == 0) {
        itemObject.item.craftVsBuy = "buy";
        itemObject.itemCost = toFixed(parseFloat(itemObject.item.formatBuyPrice) / (itemObject.item.amount < 1 ? 1 : itemObject.item.amount) * itemObject.number);
    }
    else {
        if (itemObject.ingredients == null || itemObject.ingredients.length < 1) {
            itemObject.item.craftVsBuy = "buy";
            //itemObject.itemCost = -44;
        }
        else {
            $.each(itemObject.ingredients, function (key, val) {
                buyOrCraftDecider(val);
            });

            var craftCost = 0;
            $.each(itemObject.ingredients, function (key, val) {
                craftCost = toFixed(parseFloat(craftCost) + parseFloat(val.itemCost));
            });
            var craftOrg = craftCost;
            craftCost = toFixed(craftCost / itemObject.item.craftingResultAmount);

            itemObject.itemCost = parseFloat(itemObject.item.formatBuyPrice);
            if (itemObject.itemCost <= craftCost) {
                itemObject.item.craftVsBuy = "buy";
            }
            else {
                itemObject.item.craftVsBuy = "craft";
                itemObject.itemCost = craftCost;
            }
            itemObject.itemCost = toFixed(itemObject.itemCost * (itemObject.number < 1 ? 1 : itemObject.number));
        }
    }
}

// DRAW
function drawCalculator() {
    var wrapper = $('#craftingCalcWrapper').append('<div>');
    wrapper.children().remove();
    var snapshotWrapper = $('<div class="col-12"></div>').appendTo(wrapper);
    var tldrWrapper = $('<div class="col-12"></div>').appendTo(wrapper);
    var treeWrapper = $('<div class="col-12"></div>').appendTo(wrapper);
    var calcOverviewWrapper = $('<div class="col-12"></div>').appendTo(wrapper);
    var calcProfitWrapper = $('<div class="col-12"></div>').appendTo(wrapper);
    craftingCalc.tree.visible = [];
    drawSnapshotManager(snapshotWrapper);
    drawTreeHeader(treeWrapper);
    craftingCalc.tree.topToBottom.forEach(function (e, i) {
        if (e.show) {
            drawTreeEntry(e, treeWrapper);
            craftingCalc.tree.visible.push(e);
        }
    });
    drawCalculationOverview(calcOverviewWrapper);
    drawCalculationOverviewProfit(craftingCalc.calc.entries, calcProfitWrapper, tldrWrapper);
    bindEvents();
}

function drawTreeHeader(wrapper) {
    var html = '<div class="d-flex flex-row justify-content-between my-1 mx-1">' +
        '<div class="d-flex flex-row justify-content-between w-50">' +
        '<div class="font-weight-bold">' +
        'Item' +
        '</div>' +
        '</div>' +
        '<div class="d-flex flex-row justify-content-between w-50">' +
        '<div class="font-weight-bold">' +
        'Amount' +
        '</div>' +
        '<div class="font-weight-bold">' +
        'Price' +
        '</div>' +
        '</div>' +
        '</div>' +
        '<div class="d-flex flex-row justify-content-between my-1 mx-1"><button class="btn btn-outline-secondary btn-sm optimal-route-btn">Optimal Route</button></div>';
    $(wrapper).append(html);
}

function drawTreeEntry(displayIngredient, wrapper) {
    var depthSpacer = '';
    var advice = calculateAdvice(displayIngredient.recipeId);
    for (var i = 0; i < displayIngredient.depth; i++) {
        depthSpacer += '<div style="width: 24px;"></div>';
    }
    var priceSelector = '<div class="btn-group">' +
        '<button class="btn btn-sm btn-outline-secondary price-select-sell-btn ' + (displayIngredient.usedPrice === 'sell' ? 'active' : '') + '" data-recipeid="' + displayIngredient.recipeId + '">' + formatPrice(displayIngredient.sellPrice) + '<img class="ml-1" height = "14" src = "/img/Coin.png" /></button>' +
        '<button class="btn btn-sm btn-outline-secondary price-select-buy-btn ' + (displayIngredient.usedPrice === 'buy' ? 'active' : '') + '" data-recipeid="' + displayIngredient.recipeId + '">' + formatPrice(displayIngredient.buyPrice) + '<img class="ml-1" height = "14" src = "/img/Coin.png" /></button>' +
        '</div>';
    var rootItemSelector = '<div class="d-flex flex-row justify-content-between w-50">' +
        '<div class="d-flex flex-row">' +
        '<div>Results in ' + displayIngredient.craftResultAmount + '</div>' +
        '</div > ' +

        '<div class="d-flex flex-row">' +
        '<div class="btn-group">' +
        '<button class="btn btn-sm btn-outline-secondary root-price-select-sell-btn ' + (displayIngredient.usedPrice === 'sell' ? 'active' : '') + '" data-recipeid="' + displayIngredient.recipeId + '">All Sell</button>' +
        '<button class="btn btn-sm btn-outline-secondary root-price-select-buy-btn ' + (displayIngredient.usedPrice === 'buy' ? 'active' : '') + '" data-recipeid="' + displayIngredient.recipeId + '">All Buy</button>' +
        '</div>' +
        '</div > ' +
        '</div > ';

    var ingredientItemSelector = '<div class="d-flex flex-row justify-content-between w-50">' +
        '<div class="d-flex flex-row">' +
        '<div>' + (displayIngredient.expanded && displayIngredient.hasIngredients ? 'Craft ' + displayIngredient.craftEffectiveAmount : 'Buy ' + displayIngredient.amount) + '</div>' +
        '</div > ' +

        '<div class="d-flex flex-row">' +
        '<div>' + (displayIngredient.expanded && displayIngredient.hasIngredients ? 'Sum: ' + formatPrice(calculateRecipeSum(displayIngredient.recipeId)) + '<img class="ml-1" height = "14" src = "/img/Coin.png" />' : priceSelector) + '</div>' +
        '</div > ' +
        '</div > ';

    var html = '<div class="d-flex flex-row justify-content-between my-1 mx-1"">' +

        '<div class="d-flex flex-row w-50">' +
        depthSpacer +
        '<button class="btn btn-sm btn-outline-secondary recipe-expand-btn text-monospace ' + (displayIngredient.hasIngredients ? '' : 'invisible') + '" data-recipeid="' + displayIngredient.recipeId + '">' + (displayIngredient.expanded ? '-' : '+') + '</button>' +
        '<a href="/item/' + displayIngredient.itemId + '">' +
        '<div class="d-flex flex-row">' +
        '<img class="ml-1 item-image-med" src="' +
        '/img/items/' + displayIngredient.itemId + '.png' +
        '"/ >' +
        '<div class="ml-1">' +
        displayIngredient.name +
        '</div>' +
        (displayIngredient.hasIngredients ? '<div><div class="ml-1 badge badge-pill ' + (advice === 'Craft' && displayIngredient.expanded || advice === 'Buy' && !displayIngredient.expanded ? 'badge-success' : 'badge-danger') + '">' + advice + '</div></div>' : '') +
        '</div>' +
        '</a>' +
        '</div>' +

        (displayIngredient.recipeId === 0 ? rootItemSelector : ingredientItemSelector) +

        '</div>';
    $(wrapper).append(html);
}

function drawCalculationOverview(wrapper) {
    var entries = [];
    craftingCalc.tree.visible.forEach(function (e, i) {
        var entry = entries.find(x => x.itemId === e.itemId);
        if (entry === undefined && (!e.expanded || !e.hasIngredients)) {
            entry = Object.assign({}, e);
            entry.totalAmount = calculateTotalAmount(entry.amount, entry.rootDisplayIngredient ? entry.rootDisplayIngredient.amount : 1, entry.rootDisplayIngredient);
            entries.push(entry);
        } else if (entry !== undefined && (!e.expanded || !e.hasIngredients)) {
            entry.totalAmount += calculateTotalAmount(e.amount, e.rootDisplayIngredient ? e.rootDisplayIngredient.amount : 1, e.rootDisplayIngredient);
        }
        if (entry !== undefined) {
            switch (entry.usedPrice) {
                case 'buy':
                    entry.totalPrice += entry.buyPrice * (e.totalAmount / e.bundleAmount);
                    break;
                case 'sell':
                    entry.totalPrice += entry.sellPrice * (e.totalAmount / e.bundleAmount);
                    break;
                case 'custom':
                    entry.totalPrice += entry.customPrice * (e.totalAmount / e.bundleAmount);
                    break;
            }
        }
    });

    craftingCalc.calc.entries = entries;

    var calculationOverviewHeader = '<div class="mx-1"><hr></div><div class="d-flex flex-row justify-content-between mx-1"><div class="font-weight-bold">Item</div><div class="justify-content-between d-flex flex-row w-50"><div class="font-weight-bold">Amount x Price / Bundle Size</div><div class="font-weight-bold">Resulting Price</div></div></div></div>';
    $(wrapper).append(calculationOverviewHeader);

    entries.forEach(function (e, i) {
        drawCalulationOverviewEntry(e, wrapper);
    });
}

function drawCalulationOverviewEntry(displayIngredient, wrapper) {
    var html = '<div class="d-flex flex-row justify-content-between my-1 mx-1"">' +

        '<div class="d-flex flex-row w-50">' +
        '<a href="/item/' + displayIngredient.itemId + '">' +
        '<div class="d-flex flex-row">' +
        '<img class="ml-1 item-image-med" src="' +
        '/img/items/' + displayIngredient.itemId + '.png' +
        '"/ >' +
        '<div class="ml-1">' +
        displayIngredient.name +
        '</div>' +
        '</div>' +
        '</a>' +
        '</div>' +

        '<div class="d-flex flex-row justify-content-between w-50">' +
        '<div class="d-flex flex-row">' +
        '<div>' +
        displayIngredient.totalAmount + ' x ' +
        '</div>' +
        '<div class="ml-1">' +
        formatPrice(displayIngredient.usedPrice === 'buy' ? displayIngredient.buyPrice : displayIngredient.sellPrice) +
        '</div>' +
        '<div class="ml-1">' +
        (displayIngredient.bundleAmount > 1 ? ' / ' + displayIngredient.bundleAmount : '') + ' = ' +
        '</div>' +
        '</div>' +
        '<div>' +
        formatPrice(displayIngredient.totalPrice) + '<img height="14" src="/img/Coin.png" />' +
        '</input>' +
        '</div>' +
        '</div>' +

        '</div>';
    $(wrapper).append(html);
}

function drawCalculationOverviewProfit(entries, wrapper, tldrWrapper) {
    $(wrapper).children().remove();
    craftingCalc.calc.sum = calculateSum(entries, false);
    var sum = craftingCalc.calc.sum;
    var sellPrice = craftingCalc.tree.topToBottom[0].usedSellPrice === 'sell' ? craftingCalcData.data.item.sellPrice : craftingCalcData.data.item.buyPrice;
    var fee = sellPrice * 0.1;
    var sellPriceMinusFee = sellPrice - fee;
    var resultingAmount = craftingCalcData.data.item.craftingResultAmount;
    var totalSellPrice = sellPriceMinusFee * resultingAmount;
    var profit = totalSellPrice - sum;
    var htmlSpacer = '<div class="px-1"><hr></div>';
    var html = '<div class="d-flex align-items-end flex-column">' +
        '<div class="d-flex flex-row justify-content-between w-50 mr-1"><div class="font-weight-bold">Sell Price: </div>' +
        '<div class="btn-group">' +
        '<button class="btn btn-sm btn-outline-secondary btn-sm sell-price-select-sell-btn ' + (craftingCalc.tree.topToBottom[0].usedSellPrice === 'sell' ? 'active' : '') + '">' + formatPrice(craftingCalc.tree.topToBottom[0].sellPrice) + '<img class="ml-1" height = "14" src = "/img/Coin.png" /></button>' +
        '<button class="btn btn-sm btn-outline-secondary btn-sm sell-price-select-buy-btn ' + (craftingCalc.tree.topToBottom[0].usedSellPrice === 'buy' ? 'active' : '') + '">' + formatPrice(craftingCalc.tree.topToBottom[0].buyPrice) + '<img class="ml-1" height = "14" src = "/img/Coin.png" /></button>' +
        '</div>' +
        '</div>' +
        '<div class="d-flex flex-row justify-content-between w-50 mr-1"><div class="font-weight-bold">- Fee (' + formatPrice(fee) + ') : </div><div>' + formatPrice(sellPriceMinusFee) + '<img class="ml-1" height = "14" src = "/img/Coin.png" /></div></div>' +
        '<div class="d-flex flex-row justify-content-between w-50 mr-1"><div class="font-weight-bold">x Resulting Amount (' + resultingAmount + '): </div><div>' + formatPrice(totalSellPrice) + '<img class="ml-1" height = "14" src = "/img/Coin.png" /></div></div>' +
        '<div class="d-flex flex-row justify-content-between w-50 mr-1"><div class="font-weight-bold">- Ingredient Sum: </div><div>' + formatPrice(sum) + '<img class="ml-1" height = "14" src = "/img/Coin.png" /></div></div>' +
        '<div class="d-flex flex-row justify-content-between w-50 mr-1"><div class="font-weight-bold">Profit: </div><div class="' + (profit >= 0 ? 'sum-pos' : 'sum-neg') + '">' + formatPrice(profit) + '<img class="ml-1" height = "14" src = "/img/Coin.png" /></div></div>' +
        '</div>';

    var htmlTldr = '<div class="d-flex justify-content-around flex-row mx-1 mt-2 h4">' +
        '<div class="d-inline-flex flex-row mr-1"><div class="font-weight-bold mr-1">Crafting Cost: </div><div>' + formatPrice(sum) + '<img class="ml-1" height = "14" src = "/img/Coin.png" /></div></div>' +
        '<div class="d-inline-flex flex-row mr-1"><div class="font-weight-bold mr-1">Profit: </div><div class="' + (profit >= 0 ? 'sum-pos' : 'sum-neg') + '">' + formatPrice(profit) + '<img class="ml-1" height = "14" src = "/img/Coin.png" /></div></div>' +
        '</div>' +
        htmlSpacer;

    $(wrapper).append(htmlSpacer);
    $(wrapper).append(html);
    $(tldrWrapper).append(htmlTldr);
}

function drawSnapshotManager(wrapper) {
    var html = '<div class="btn-group" role="group" aria-label="Snaphsots">';

    snapshots.forEach(function (e, i) {
        html += '<button type="button" class="btn btn-sm btn-outline-secondary choose-snapshot-btn ' + (e.id === selectedSnapshot ? 'active' : '') + '" data-snapshotid="' + e.id + '">' + e.name + '</button>';
    });

    html += '<button type="button" class="btn btn-sm btn-outline-secondary create-snapshot-btn">Create Snapshot</button>' +
        '</div>';
    if (selectedSnapshot !== 1) {
        html += '<button type="button" class="btn btn-sm btn-outline-secondary ml-2 delete-snapshot-btn">' + (snapshotDeleteConfirmation ? 'Are you sure?' : 'Delete Snapshot') + '</button>';
    }
    $(wrapper).append(html);
}

// MANIPULATE
function collapseRecipe(recipeId, collapse) {
    var inTarget = false;
    var targetDepth = 0;
    craftingCalc.tree.topToBottom.forEach(function (e, i) {
        if (inTarget && e.depth > targetDepth) {
            if (e.depth > targetDepth + 1)
                e.show = false;
            else
                e.show = !collapse;
            if (e.hasIngredients)
                e.expanded = false;
        } else {
            inTarget = false;
        }

        if (e.recipeId === recipeId) {
            inTarget = true;
            targetDepth = e.depth;
            e.expanded = !collapse;
        }
    });
}

// UPDATE
function expandRecipe(recipeId, expand) {
    collapseRecipe(recipeId, !expand);
    drawCalculator();
}

// EVENT HANDLERS
function bindEvents() {
    $('.recipe-expand-btn').click(function () {
        var recipeId = parseInt($(this).attr('data-recipeid'));
        if (getRecipeExpandedStatus(recipeId))
            expandRecipe(recipeId, false);
        else {
            expandRecipe(recipeId, true);
        }
    });

    $('.price-select-sell-btn').click(function () {
        var recipeId = parseInt($(this).attr('data-recipeid'));
        setRecipeUsedPrice(recipeId, 'sell');
        drawCalculator();
    });

    $('.price-select-buy-btn').click(function () {
        var recipeId = parseInt($(this).attr('data-recipeid'));
        setRecipeUsedPrice(recipeId, 'buy');
        drawCalculator();
    });

    $('.root-price-select-sell-btn').click(function () {
        craftingCalc.tree.topToBottom.forEach(function (e, i) {
            setRecipeUsedPrice(e.recipeId, 'sell');
        });
        drawCalculator();
    });

    $('.root-price-select-buy-btn').click(function () {
        craftingCalc.tree.topToBottom.forEach(function (e, i) {
            setRecipeUsedPrice(e.recipeId, 'buy');
        });
        drawCalculator();
    });

    $('.sell-price-select-sell-btn').click(function () {
        setUsedSellPrice('sell');
        drawCalculator();
    });

    $('.sell-price-select-buy-btn').click(function () {
        setUsedSellPrice('buy');
        drawCalculator();
    });

    $('.optimal-route-btn').click(function () {
        chooseOptimalRoute();
        drawCalculator();
    });

    $('.create-snapshot-btn').click(function () {
        if (snapshots.length <= 4) {
            makeSnapshot(moment().format(readSetting('timestamp-format-date') + ' ' + readSetting('timestamp-format-time')));
            updateSnapshotSave();
            drawCalculator();
        }
    });

    $('.choose-snapshot-btn').click(function () {
        var snapshotId = parseInt($(this).attr('data-snapshotid'));
        selectedSnapshot = snapshotId;
        applySnapshot(snapshotId);
        drawCalculator();
    });

    $('.delete-snapshot-btn').click(function () {
        if (snapshotDeleteConfirmation) {
            deleteSnapshot(selectedSnapshot);
            selectedSnapshot = 1;
            snapshotDeleteConfirmation = false;
            applySnapshot(1);
            drawCalculator();
        } else {
            snapshotDeleteConfirmation = true;
            drawCalculator();
        }
    });

    $('.delete-snapshot-btn').mouseleave(function () {
        snapshotDeleteConfirmation = false;
        drawCalculator();
    });
}

// HELPERS
function getRecipeExpandedStatus(recipeId) {
    return craftingCalc.tree.topToBottom.find(x => x.recipeId === recipeId).expanded;
}

function setRecipeUsedPrice(recipeId, usedPrice) {
    var itemId = craftingCalc.tree.topToBottom.find(x => x.recipeId === recipeId).itemId;
    craftingCalc.tree.topToBottom.forEach(function (e, i) {
        if (e.itemId === itemId) {
            e.usedPrice = usedPrice;
        }
    });
}

function setUsedSellPrice(usedSellPrice) {
    craftingCalc.tree.topToBottom.find(x => x.recipeId === 0).usedSellPrice = usedSellPrice;
}

function calculateSum(entries, singleItem) {
    var sum = 0;
    entries.forEach(function (e, i) {
        var price = 0;
        switch (e.usedPrice) {
            case 'buy':
                price = e.buyPrice;
                break;
            case 'sell':
                price = e.sellPrice;
                break;
            case 'custom':
                price = e.customPrice;
                break;
        }
        if (singleItem)
            sum += price * e.amount / e.bundleAmount;
        else
            sum += price * e.totalAmount / e.bundleAmount;
    });
    return sum;
}

function calculateAdvice(recipeId) {
    var inTarget = false;
    var targetDepth = 0;
    var ingredients = [];
    var recipe = {};
    craftingCalc.tree.topToBottom.forEach(function (e, i) {
        if (inTarget && e.depth > targetDepth) {
            if (e.depth === targetDepth + 1)
                ingredients.push(e);
        } else {
            inTarget = false;
        }

        if (e.recipeId === recipeId) {
            inTarget = true;
            targetDepth = e.depth;
            recipe = e;
        }
    });

    var ingredientSum = calculateSum(ingredients, true);
    return recipe.buyPrice * recipe.craftResultAmount <= ingredientSum ? 'Buy' : 'Craft';
}

function calculateRecipeSum(recipeId) {
    var inTarget = false;
    var targetDepth = 0;
    var ingredients = [];
    craftingCalc.tree.topToBottom.forEach(function (e, i) {
        if (inTarget && e.depth > targetDepth) {
            if (e.depth === targetDepth + 1)
                ingredients.push(e);
        } else {
            inTarget = false;
        }

        if (e.recipeId === recipeId) {
            inTarget = true;
            targetDepth = e.depth;
        }
    });

    return calculateSum(ingredients, true);
}

function chooseOptimalRoute() {
    craftingCalc.tree.topToBottom.forEach(function (e, i) {
        if (e.hasIngredients) {
            //var advice = calculateAdvice(e.recipeId);
            var advice = e.craftVsBuy;
            if (advice === 'craft' || e.recipeId === 0)
                if (e.rootDisplayIngredient !== null) {
                    if (e.rootDisplayIngredient.expanded)
                        collapseRecipe(e.recipeId, false);
                    else
                        collapseRecipe(e.recipeId, true);
                } else {
                    collapseRecipe(e.recipeId, false);
                }

            else
                collapseRecipe(e.recipeId, true);
        }
    });
}

function formatPrice(price) {
    return (price / 100).toFixed(2);
}

function getRootExpandedStatus(recipeId) {
    var reversedTopToBottom = craftingCalc.tree.topToBottom.slice().reverse();
    var inTarget = false;
    var targetDepth = 0;
    var expanded = true;
    reversedTopToBottom.forEach(function (e, i) {
        if (inTarget && e.depth > targetDepth) {
            if (e.depth === targetDepth - 1)
                expanded = e.expanded;
        } else {
            inTarget = false;
        }

        if (e.recipeId === recipeId) {
            inTarget = true;
            targetDepth = e.depth;
        }
    });
    return expanded;
}

function calculateTotalAmount(baseAmount, rootAmount, rootDisplayIngredient) {
    var result = 0;
    if (rootDisplayIngredient !== null && rootDisplayIngredient.recipeId !== 0)
        result += baseAmount * rootDisplayIngredient.totalAmount / rootDisplayIngredient.craftResultAmount;
    else
        result += baseAmount * 1;
    return result;
}

function toFixed(number) {
    return number.toFixed(2);
}

function makeSnapshot(name) {
    var newId = 1;
    if (snapshots.length > 0) {
        snapshots.sort(function (a, b) { return a.id - b.id })
        newId = snapshots[snapshots.length - 1].id + 1;
    }
    var snapshot = {
        id: newId,
        name: name,
        craftingCalcData: clone(craftingCalcData),
        craftingCalc: clone(craftingCalc)
    }

    snapshots.push(snapshot);
}

function applySnapshot(snapshotId) {
    var snapshot = snapshots.find(x => x.id === snapshotId);
    craftingCalcData = clone(snapshot.craftingCalcData);
    craftingCalc = clone(snapshot.craftingCalc);
}

function deleteSnapshot(snapshotId) {
    var index = snapshots.findIndex(x => x.id === snapshotId);
    snapshots.splice(index, 1);
    updateSnapshotSave();
}

function updateSnapshotSave() {
    var snapshotsClone = [];
    snapshotsClone = clone(snapshots);
    snapshotsClone.splice(snapshotsClone.findIndex(x => x.id === 1), 1);
    if (snapshotsClone.length > 0)
        localStorage.setItem('snapshots-' + craftingCalcData.data.item.id, JSON.stringify(snapshotsClone));
    else
        localStorage.removeItem('snapshots-' + craftingCalcData.data.item.id);
}

function readSnapshotSave() {
    var readSnapshots = JSON.parse(localStorage.getItem('snapshots-' + craftingCalcData.data.item.id));
    if (readSnapshots !== null)
        readSnapshots.forEach(function (e, i) {
            snapshots.push(e);
        });
}

function clone(obj) {
    var copy;

    // Handle the 3 simple types, and null or undefined
    if (null == obj || "object" != typeof obj) return obj;

    // Handle Date
    if (obj instanceof Date) {
        copy = new Date();
        copy.setTime(obj.getTime());
        return copy;
    }

    // Handle Array
    if (obj instanceof Array) {
        copy = [];
        for (var i = 0, len = obj.length; i < len; i++) {
            copy[i] = clone(obj[i]);
        }
        return copy;
    }

    // Handle Object
    if (obj instanceof Object) {
        copy = {};
        for (var attr in obj) {
            if (obj.hasOwnProperty(attr)) copy[attr] = clone(obj[attr]);
        }
        return copy;
    }

    throw new Error("Unable to copy obj! Its type isn't supported.");
}