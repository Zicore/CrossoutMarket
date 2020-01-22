const defaultOrder = [[11, "desc"]];
var table;

$(document).ready(function () {

    $('#loadingIcon').removeClass('d-flex').addClass('d-none');
    $('#table-wrapper').removeClass('d-none');
    $('#dtButtonWrapper').removeClass('d-none');
    checkExtendedSetting();
    adjustTimeStamp();

    var domOption =
        "<'row m-1'<'d-inline-flex justify-content-start'p><'d-inline-flex ml-auto text-secondary'l>>" +
        "<tr>" +
        "<'row m-1'<'d-inline-flex justify-content-start'p><'d-none d-sm-inline-flex ml-auto text-secondary'i>>";

    $.fn.DataTable.ext.pager.numbers_length = 5;

    var order = [[11, "desc"]];

    table = $('#ItemTable').DataTable({
        paging: true,
        searching: true,
        search: {
            smart: false,
            regex: false
        },
        autoWidth: false,
        responsive: {
            details: {
                type: 'inline',
                renderer: function (api, rowIdx, columns) {
                    var data = $.map(columns, function (col, i) {
                        return col.hidden ?
                            '<div class="d-inline-flex flex-column card p-1 px-2 m-2" data-dt-row="' + col.rowIndex + '" data-dt-column="' + col.columnIndex + '">' +
                            '<div class="font-weight-bold">' + col.title + '' + '</div> ' +
                            '<div>' + col.data + '</div>' +
                            '</div>' :
                            '';
                    }).join('');

                    return data ?
                        $('<div/>').append(data) :
                        false;
                }
            }
        },
        columnDefs: [
            { targets: 0, width: '25%' },
            { targets: '_all', width: '15%' }
        ],
        buttons: {
            dom: {
                //container: {
                //    tag: 'ul',
                //    className: 'pagination dataTables_button'
                //},
                //buttonContainer: {
                //    tag: 'li',
                //    className: 'paginate_button'
                //},
                button: {
                    tag: 'button',
                    className: 'btn btn-sm btn-outline-secondary'
                }
            },
            buttons: [{
                extend: 'excel',
                text: 'Export as Excel',
                filename: 'CrossoutDBExcelExport',
                exportOptions: {
                    modifier: {
                        page: 'current'
                    }
                }
            }, {
                extend: 'csv',
                text: 'Export as CSV',
                filename: 'CrossoutDBCSVExport',
                exportOptions: {
                    modifier: {
                        page: 'current'
                    }
                }
            }, {
                action: function () {
                    exportToSpreadsheet();
                },
                text: 'Export to Spreadsheet'
            }
            ]
        },
        order: order,
        lengthMenu: [[10, 20, 50, -1], [10, 20, 50, "All"]],
        pagingType: "simple_numbers",
        dom: domOption,
        drawCallback: function () {
            $('.dataTables_paginate > .pagination').addClass('pagination-sm');
        }
    });

    applyLocationHash(table);

    table.page.len(readSetting('length'));

    filterTable(table);

    $('#ItemTable').on('length.dt', function (e, options, len) {
        writeSetting('length', table.page.info().length);
    });

    // Place buttons in container
    table.buttons().container().appendTo('#dt-buttons');

    // Remove ugly classes in length selector
    $('#ItemTable_length').children().children().removeClass('custom-select custom-select-sm');
});


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
    if ($(this).hasClass('active'))
        window.location = '/' + location.hash;
    else
        window.location = '/?rmdItems=true' + location.hash;
});

$('.filterMetaItems').click(function (e) {
    $('.filterMetaItems').toggleClass('active');
    filterTable(table);
    updateLocationHash(table);
    e.preventDefault();
});

var watchlist = [1, 2, 3];
function toggleWatchlist() {
    //var button = $('#watchlistFilter');
    $('#watchlistFilter').toggleClass('active');
    filterTable(table);
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
        table.column(3).search(filterFactionString, true);
    } else {
        table.column(3).search('');
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
        table.column(2).search(filterRarityString, true);
    } else {
        table.column(2).search('');
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
        table.column(4).search(filterCategoryString, true);
    } else {
        table.column(4).search('');
    }

    if ($('.filterCraftableItems').first().hasClass('active')) {
        table.column(7).search('yes');
    } else {
        table.column(7).search('');
    }

    if ($('.filterMetaItems').first().hasClass('active')) {
        table.column(8).search('yes');
    } else {
        table.column(8).search('no');
    }

    table.draw();
}

var selectedList = [];
var highlightSelectClass = 'table-info';

$('.selected-row').click(function () {
    if ($(this).hasClass(highlightSelectClass)) {
        $(this).removeClass(highlightSelectClass);
        selectedList.splice(selectedList.findIndex(x => x === $(this).data('id')), 1);
    } else {
        $(this).addClass(highlightSelectClass);
        selectedList.push($(this).data('id'));
    }

    if (selectedList.length > 0) {
        $('#watchlistSelector').removeClass('disabled');
        $('#watchlistFilter').removeClass('disabled');
    } else {
        $('#watchlistSelector').addClass('disabled');
        $('#watchlistFilter').addClass('disabled');
    }
});

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
    if (selectedList.length !== 0) {
        watchlist = [];
        selectedList.forEach(function (e) {
            watchlist.push(parseInt(e));
        });
        updateLocationHash(table);
        toggleWatchlist();
        $('.' + highlightSelectClass).removeClass(highlightSelectClass);
        selectedList = [];
    }
}

$.fn.dataTable.ext.search.push(
    function (settings, data, dataIndex) {
        var sellmin = parseInt($('#sellmin').val(), 10);
        var sellmax = parseInt($('#sellmax').val(), 10);
        var sellprice = parseFloat(data[getColumnIndexById('sellCol')]) || 0;

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
        var buymin = parseInt($('#buymin').val(), 10);
        var buymax = parseInt($('#buymax').val(), 10);
        var buyprice = parseFloat(data[getColumnIndexById('buyCol')]) || 0;

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
        var marginmin = parseInt($('#craftcostsellmin').val(), 10);
        var marginmax = parseInt($('#craftcostsellmax').val(), 10);
        var margin = parseFloat(data[getColumnIndexById('craftCostSellCol')]) || 0;

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
        var marginmin = parseInt($('#craftcostbuymin').val(), 10);
        var marginmax = parseInt($('#craftcostbuymax').val(), 10);
        var margin = parseFloat(data[getColumnIndexById('craftCostBuyCol')]) || 0;

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
        var marginmin = parseInt($('#carftingmarginmin').val(), 10);
        var marginmax = parseInt($('#carftingmarginmax').val(), 10);
        var margin = parseFloat(data[getColumnIndexById('craftMarginCol')]) || 0;

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
        var marginmin = parseInt($('#marginmin').val(), 10);
        var marginmax = parseInt($('#marginmax').val(), 10);
        var margin = parseFloat(data[getColumnIndexById('marginCol')]) || 0;

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
        var marginmin = parseInt($('#roimin').val(), 10);
        var marginmax = parseInt($('#roimax').val(), 10);
        var margin = parseFloat(data[getColumnIndexById('roiCol')]) || 0;

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
        var marginmin = parseInt($('#demandsupplymin').val(), 10);
        var marginmax = parseInt($('#demandsupplymax').val(), 10);
        var margin = parseFloat(data[getColumnIndexById('demandSupplyRatioCol')]) || 0;

        if ((isNaN(marginmin) && isNaN(marginmax)) ||
            (isNaN(marginmin) && margin <= marginmax) ||
            (marginmin <= margin && isNaN(marginmax)) ||
            (marginmin <= margin && margin <= marginmax)) {
            return true;
        }
        return false;
    }
);
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
        activateControls: [],
        deactivateControls: []
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

function switchPreset(preset) {
    $('.dt-col').addClass('none').data('priority', 10);
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