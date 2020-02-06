const defaultOrder = [[getColumnIndexById('offersCol'), "desc"]];

$('#searchBar, #searchBarMobile').keyup(function () {
    updateLocationHash(table);
    applyLocationHash(table);
});

$('.filter-faction').click(function (e) {
    var text = $(this).text();
    $('.filter-faction').each(function () {
        if (text === $(this).text()) {
            $(this).toggleClass('active');
        }
    });
    filterTable(table);
    updateLocationHash(table);
    e.preventDefault();
});

$('.filter-rarity').click(function (e) {
    var text = $(this).text();
    $('.filter-rarity').each(function () {
        if (text === $(this).text()) {
            $(this).toggleClass('active');
        }
    });
    filterTable(table);
    updateLocationHash(table);
    e.preventDefault();
});

$('.filter-category').click(function (e) {
    var text = $(this).text();
    $('.filter-category').each(function () {
        if (text === $(this).text()) {
            $(this).toggleClass('active');
        }
    });
    filterTable(table);
    updateLocationHash(table);
    e.preventDefault();
});

$('.filterCraftableItems').click(function (e) {
    $('.filterCraftableItems').toggleClass('active');
    filterTable(table);
    updateLocationHash(table);
    e.preventDefault();
});

$('.filterRemovedItems').click(function (e) {
    $('.filterRemovedItems').toggleClass('active');
    filterTable(table);
    updateLocationHash(table);
    e.preventDefault();
});

$('.filterMetaItems').click(function (e) {
    $('.filterMetaItems').toggleClass('active');
    filterTable(table);
    updateLocationHash(table);
    e.preventDefault();
});

var watchlist = [];
function toggleWatchlist() {
    //var button = $('#watchlistFilter');
    $('#watchlistFilter').toggleClass('active');
    filterTable(table);
    updateLocationHash(table);
}

const columnList = ['name', 'rarity', 'faction', 'category', 'type', 'popularity', 'sellprice', 'selloffers', 'buyprice', 'buyorders', 'margin', 'lastupdate'];

function filterTable(table) {
    var filterFactionString;
    var j = 0;

    var searchVal = $('#searchBar').val();
    table.search(searchVal);

    $('.filter-faction').each(function (i, e) {
        if ($(this).hasClass('active')) {
            var faction = $(this).text();
            faction = faction.split(' ')[0];

            if (j === 0) {
                filterFactionString = faction;
            } else {
                filterFactionString += '|' + faction;
            }
            j++;
        }
    });
    if (filterFactionString !== undefined) {
        table.column(getColumnIndexById('factionCol')).search(filterFactionString, true);
    } else {
        table.column(getColumnIndexById('factionCol')).search('');
    }

    var filterRarityString;
    j = 0;
    $('.filter-rarity').each(function (i, e) {
        if ($(this).hasClass('active')) {
            if (j === 0) {
                filterRarityString = $(this).text();
            } else {
                filterRarityString += '|' + $(this).text();
            }
            j++;
        }
    });
    if (filterRarityString !== undefined) {
        table.column(getColumnIndexById('rarityCol')).search(filterRarityString, true);
    } else {
        table.column(getColumnIndexById('rarityCol')).search('');
    }

    var filterCategoryString;
    j = 0;
    $('.filter-category').each(function (i, e) {
        if ($(this).hasClass('active')) {
            var category = $(this).text();
            category = category.split(' ')[0];

            if (j === 0) {
                filterCategoryString = category;
            } else {
                filterCategoryString += '|' + category;
            }
            j++;
        }
    });
    if (filterCategoryString !== undefined) {
        table.column(getColumnIndexById('categoryCol')).search(filterCategoryString, true);
    } else {
        table.column(getColumnIndexById('categoryCol')).search('');
    }

    if ($('.filterCraftableItems').first().hasClass('active')) {
        table.column(getColumnIndexById('craftableCol')).search('yes');
    } else {
        table.column(getColumnIndexById('craftableCol')).search('');
    }

    if ($('.filterRemovedItems').first().hasClass('active')) {
        table.column(getColumnIndexById('removedCol')).search('no');
    } else {
        table.column(getColumnIndexById('removedCol')).search('yes');
    }

    if ($('.filterMetaItems').first().hasClass('active')) {
        table.column(getColumnIndexById('metaCol')).search('yes');
    } else {
        table.column(getColumnIndexById('metaCol')).search('no');
    }

    table.draw();
}


//function compareSelected() {
//    if (selectedList.length !== 0) {
//        var url = "/compare/";
//        var i = 0;
//        selectedList.forEach(function (e) {
//            if (i !== 0) {
//                url += ",";
//            }
//            var id = e;
//            url += id;
//            i++;
//        });
//        window.location = url;
//    }
//}

function watchlistSelected() {
    var selectedDataArray = Array.from(selectedList);
    if (selectedDataArray.length !== 0) {
        watchlist = [];
        selectedDataArray.forEach(function (e) {
            watchlist.push(parseInt(e.id));
        });
        updateLocationHash(table);
        toggleWatchlist();
    }
}

colIds = {
    sellCol: getColumnIndexById('sellCol'),
    buyCol: getColumnIndexById('buyCol'),
    craftCostSellCol: getColumnIndexById('craftCostSellCol'),
    craftCostBuyCol: getColumnIndexById('craftCostBuyCol'),
    craftMarginCol: getColumnIndexById('craftMarginCol'),
    marginCol: getColumnIndexById('marginCol'),
    roiCol: getColumnIndexById('roiCol'),
    demandSupplyRatioCol: getColumnIndexById('demandSupplyRatioCol')
};

var rangeFilterValues = {
    sellmin: { value: 0, checked: false },
    sellmax: { value: NaN, checked: false },
    buymin: { value: NaN, checked: false },
    buymax: { value: NaN, checked: false },
    craftcostsellmin: { value: NaN, checked: false },
    craftcostsellmax: { value: NaN, checked: false },
    craftcostbuymin: { value: NaN, checked: false },
    craftcostbuymax: { value: NaN, checked: false },
    carftingmarginmin: { value: NaN, checked: false },
    carftingmarginmax: { value: NaN, checked: false },
    marginmin: { value: NaN, checked: false },
    marginmax: { value: NaN, checked: false },
    roimin: { value: NaN, checked: false },
    roimax: { value: NaN, checked: false },
    demandsupplymin: { value: NaN, checked: false },
    demandsupplymax: { value: NaN, checked: false }
};

function checkRangeFilterValue(filterId) {
    if (!rangeFilterValues[filterId].checked) {
        var readValue = $('#' + filterId).val();
        rangeFilterValues[filterId].value = readValue;
        rangeFilterValues[filterId].checked = true;
        return readValue;
    }
    else
        return rangeFilterValues[filterId].value;
}

function resetRangeFilterCheckedStatus() {
    Object.keys(rangeFilterValues).forEach(function (e, i) {
        rangeFilterValues[e].checked = false;
    });
}

$.fn.dataTable.ext.search.push(
    function (settings, data, dataIndex) {
        var sellmin = parseFloat(checkRangeFilterValue('sellmin'), 10);
        var sellmax = parseFloat(checkRangeFilterValue('sellmax'), 10);
        var sellprice = parseFloat(data[colIds.sellCol]) || 0;

        if ((isNaN(sellmin) && isNaN(sellmax)) ||
            (isNaN(sellmin) && sellprice <= sellmax) ||
            (sellmin <= sellprice && isNaN(sellmax)) ||
            (sellmin <= sellprice && sellprice <= sellmax)) {
            return true;
        }
        return false;
    }
);

$.fn.dataTable.ext.search.push(
    function (settings, data, dataIndex) {
        var buymin = parseFloat(checkRangeFilterValue('buymin'), 10);
        var buymax = parseFloat(checkRangeFilterValue('buymax'), 10);
        var buyprice = parseFloat(data[colIds.buyCol]) || 0;

        if ((isNaN(buymin) && isNaN(buymax)) ||
            (isNaN(buymin) && buyprice <= buymax) ||
            (buymin <= buyprice && isNaN(buymax)) ||
            (buymin <= buyprice && buyprice <= buymax)) {
            return true;
        }
        return false;
    }
);

$.fn.dataTable.ext.search.push(
    function (settings, data, dataIndex) {
        var marginmin = parseFloat(checkRangeFilterValue('craftcostsellmin'), 10);
        var marginmax = parseFloat(checkRangeFilterValue('craftcostsellmax'), 10);
        var margin = parseFloat(data[colIds.craftCostSellCol]) || 0;

        if ((isNaN(marginmin) && isNaN(marginmax)) ||
            (isNaN(marginmin) && margin <= marginmax) ||
            (marginmin <= margin && isNaN(marginmax)) ||
            (marginmin <= margin && margin <= marginmax)) {
            return true;
        }
        return false;
    }
);


$.fn.dataTable.ext.search.push(
    function (settings, data, dataIndex) {
        var marginmin = parseFloat(checkRangeFilterValue('craftcostbuymin'), 10);
        var marginmax = parseFloat(checkRangeFilterValue('craftcostbuymax'), 10);
        var margin = parseFloat(data[colIds.craftCostBuyCol]) || 0;

        if ((isNaN(marginmin) && isNaN(marginmax)) ||
            (isNaN(marginmin) && margin <= marginmax) ||
            (marginmin <= margin && isNaN(marginmax)) ||
            (marginmin <= margin && margin <= marginmax)) {
            return true;
        }
        return false;
    }
);


$.fn.dataTable.ext.search.push(
    function (settings, data, dataIndex) {
        var marginmin = parseFloat(checkRangeFilterValue('carftingmarginmin'), 10);
        var marginmax = parseFloat(checkRangeFilterValue('carftingmarginmax'), 10);
        var margin = parseFloat(data[colIds.craftMarginCol]) || 0;

        if ((isNaN(marginmin) && isNaN(marginmax)) ||
            (isNaN(marginmin) && margin <= marginmax) ||
            (marginmin <= margin && isNaN(marginmax)) ||
            (marginmin <= margin && margin <= marginmax)) {
            return true;
        }
        return false;
    }
);

$.fn.dataTable.ext.search.push(
    function (settings, data, dataIndex) {
        var marginmin = parseFloat(checkRangeFilterValue('marginmin'), 10);
        var marginmax = parseFloat(checkRangeFilterValue('marginmax'), 10);
        var margin = parseFloat(data[colIds.marginCol]) || 0;

        if ((isNaN(marginmin) && isNaN(marginmax)) ||
            (isNaN(marginmin) && margin <= marginmax) ||
            (marginmin <= margin && isNaN(marginmax)) ||
            (marginmin <= margin && margin <= marginmax)) {
            return true;
        }
        return false;
    }
);

$.fn.dataTable.ext.search.push(
    function (settings, data, dataIndex) {
        var marginmin = parseFloat(checkRangeFilterValue('roimin'), 10);
        var marginmax = parseFloat(checkRangeFilterValue('roimax'), 10);
        var margin = parseFloat(data[colIds.roiCol]) || 0;

        if ((isNaN(marginmin) && isNaN(marginmax)) ||
            (isNaN(marginmin) && margin <= marginmax) ||
            (marginmin <= margin && isNaN(marginmax)) ||
            (marginmin <= margin && margin <= marginmax)) {
            return true;
        }
        return false;
    }
);

$.fn.dataTable.ext.search.push(
    function (settings, data, dataIndex) {
        var marginmin = parseFloat(checkRangeFilterValue('demandsupplymin'), 10);
        var marginmax = parseFloat(checkRangeFilterValue('demandsupplymax'), 10);
        var margin = parseFloat(data[colIds.demandSupplyRatioCol]) || 0;

        if ((isNaN(marginmin) && isNaN(marginmax)) ||
            (isNaN(marginmin) && margin <= marginmax) ||
            (marginmin <= margin && isNaN(marginmax)) ||
            (marginmin <= margin && margin <= marginmax)) {
            return true;
        }
        return false;
    }
);
var watchlistFilterClass = { value: '', checked: false };

function checkClass(id) {
    if (!watchlistFilterClass.checked) {
        var readValue = $('#' + id).hasClass();
        watchlistFilterClass.value = readValue;
        watchlistFilterClass.checked = true;
        return readValue;
    }
    else
        return watchlistFilterClass.value;
}

$.fn.dataTable.ext.search.push(function (settings, searchData, index, rowData, counter) {
    if ($('#watchlistFilter').hasClass('active')) {
        if (watchlist.includes(parseInt(searchData[1])))
            return true;
        else
            return false;
    }
    return true;
});

function getColumnIndexById(id) {
    var index;
    $('.dt-col').each(function (i, e) {
        if ($(this).attr('id') === id)
            index = i;
    });
    return index;
}

var colPresets = {
    default: {
        buttonId: 'defaultPreset',
        cols: [{ id: 'itemCol', priority: '1' }, { id: 'sellCol', priority: '2' }, { id: 'offersCol', priority: '3' }, { id: 'buyCol', priority: '2' }, { id: 'ordersCol', priority: '3' }],
        activateFilters: [],
        deactivateFilters: ['filterCraftableItems'],
        activateControls: ['placeHolderRange'],
        deactivateControls: ['flippingRange', 'craftingRange']
    },
    crafting: {
        buttonId: 'craftingPreset',
        cols: [{ id: 'itemCol', priority: '1' }, { id: 'sellCol', priority: '4' }, { id: 'craftCostSellCol', priority: '4' }, { id: 'buyCol', priority: '4' }, { id: 'craftCostBuyCol', priority: '3' }, { id: 'craftMarginCol', priority: '2' }],
        activateFilters: ['filterCraftableItems'],
        deactivateFilters: [],
        activateControls: ['craftingRange'],
        deactivateControls: ['flippingRange', 'placeHolderRange']
    },
    flipping: {
        buttonId: 'flippingPreset',
        cols: [{ id: 'itemCol', priority: '1' }, { id: 'sellCol', priority: '4' }, { id: 'offersCol', priority: '4' }, { id: 'buyCol', priority: '4' }, { id: 'ordersCol', priority: '4' }, { id: 'marginCol', priority: '2' }, { id: 'roiCol', priority: '3' }],
        activateFilters: [],
        deactivateFilters: ['filterCraftableItems'],
        activateControls: ['flippingRange'],
        deactivateControls: ['craftingRange', 'placeHolderRange']
    }
};

function switchPreset(preset, updateLocHash) {
    var fullSizeTable = readSetting('full-size-table');
    if (fullSizeTable) {
        $('.dt-col').data('priority', 10);
    } else {
        $('.dt-col').addClass('none').data('priority', 10);
    }
    colPresets[preset].cols.forEach(function (e, i) {
        showCol(e.id);
        setColPriority(e.id, e.priority);
    });
    colPresets[preset].deactivateFilters.forEach(function (e, i) {
        deactivateFilter(e);
    });
    colPresets[preset].activateFilters.forEach(function (e, i) {
        activateFilter(e);
    });
    colPresets[preset].deactivateControls.forEach(function (e, i) {
        deactivateControl(e);
    });
    colPresets[preset].activateControls.forEach(function (e, i) {
        activateControl(e);
    });
    $('.filter-preset').removeClass('active');
    $('#' + colPresets[preset].buttonId).addClass('active');
    filterTable(table);
    if (updateLocHash)
        updateLocationHash(table);
    table.responsive.rebuild();
    table.responsive.recalc();
}

function showCol(colId) {
    $('#' + colId).removeClass('none');
}

function hideCol(colId) {
    $('#' + colId).addClass('none');
}

function setColPriority(colId, priority) {
    $('#' + colId).data('priority', priority);
}

function activateFilter(filterClass) {
    $('.' + filterClass).addClass('active');
}

function deactivateFilter(filterClass) {
    $('.' + filterClass).removeClass('active');
}

function deactivateControl(controlId) {
    $('#' + controlId).addClass('d-none').removeClass('d-flex');
}

function activateControl(controlId) {
    $('#' + controlId).removeClass('d-none').addClass('d-flex');
}

function drawTable() {
    table.draw();
}

function exportToSpreadsheet() {
    window.location = '/export';
}