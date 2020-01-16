const defaultOrder = [[9, "desc"]];

$(document).ready(function () {

    $('#table-wrapper').removeClass('d-none');
    $('#dtButtonWrapper').removeClass('d-none');
    adjustTimeStamp();

    var domOption =
        "<'row m-1'<'d-inline-flex justify-content-start'p><'d-inline-flex ml-auto'l>>" +
        "<tr>" +
        "<'row m-1'<'d-inline-flex justify-content-start'p><'d-none d-sm-inline-flex ml-auto'i>>";

    $.fn.DataTable.ext.pager.numbers_length = 5;

    var order = [[9, "desc"]];

    var table = $('#ItemTable').DataTable({
        paging: true,
        searching: true,
        search: {
            smart: false,
            regex: false
        },
        autoWidth: false,
        responsive: {
            details: {
                type: 'inline'
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
                    compareSelected();
                },
                text: 'Compare selected'
            }, {
                action: function () {
                    watchlistSelected();
                },
                text: 'Watchlist selected'
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

    $('#sellmin, #sellmax, #buymin, #buymax, #marginmin, #marginmax').keyup(function () {
        table.draw();
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

    $('#filterRemovedItems').click(function(e) {
        $(this).toggleClass('active');
        filterTable(table);
        updateLocationHash(table);
        e.preventDefault();
    });

    $('#filterMetaItems').click(function(e) {
        $(this).toggleClass('active');
        filterTable(table);
        updateLocationHash(table);
        e.preventDefault();
    });

    $('#searchBar, #searchBarMobile').keyup(function () {
        updateLocationHash(table);
        applyLocationHash(table);
    });

    getFilterStateFromCookie();
    applyColumnVis(table);

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

const columnList = ['name', 'rarity', 'faction', 'category', 'type', 'popularity', 'sellprice', 'selloffers', 'buyprice', 'buyorders', 'margin', 'lastupdate'];

function getFilterStateFromCookie() {
    columnList.forEach(function (e, i) {
        var cookieval = Cookies.get('showColumn-' + e);
        if (cookieval === 'true') {
            $('.colvis-' + e).parent().addClass('active');
        } else if (cookieval === 'false') {
            $('.colvis-' + e).parent().removeClass('active');
        }
    });
}

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
        table.column(2).search(filterFactionString, true);
    } else {
        table.column(2).search('');
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
        table.column(1).search(filterRarityString, true);
    } else {
        table.column(1).search('');
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
        table.column(3).search(filterCategoryString, true);
    } else {
        table.column(3).search('');
    }

    if ($('#filterRemovedItems').hasClass('active')) {
        table.column(5).search('no');
    } else {
        table.column(5).search('yes');
    }

    if ($('#filterMetaItems').hasClass('active')) {
        table.column(6).search('yes');
    } else {
        table.column(6).search('no');
    }

    table.draw();
}

function applyColumnVis(table) {
    $('.colvis').each(function() {
        currentCol = $(this);
        columnList.forEach(function(e, i) {
            if (currentCol.hasClass('colvis-' + e)) {
                var col = table.column(i)
                if (currentCol.parent().hasClass('active')) {
                    col.visible(true);
                    Cookies.set('showColumn-' + e, true);
                } else {
                    col.visible(false);
                    Cookies.set('showColumn-' + e, false);
                }
            }
        });
    });
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
});

function compareSelected() {
    if (selectedList.length !== 0) {
        var url = "/compare/";
        var i = 0;
        selectedList.forEach(function (e) {
            if (i !== 0) {
                url += ",";
            }
            var id = e;
            url += id;
            i++;
        });
        window.location = url;
    }
}

function watchlistSelected() {
    if (selectedList.length !== 0) {
        var url = "/watchlist/";
        var i = 0;
        selectedList.forEach(function (e) {
            if (i !== 0) {
                url += ",";
            }
            var id = e;
            url += id;
            i++;
        });
        window.location = url;
    }
}

$.fn.dataTable.ext.search.push(
    function (settings, data, dataIndex) {
        var sellmin = parseInt($('#sellmin').val(), 10);
        var sellmax = parseInt($('#sellmax').val(), 10);
        var sellprice = parseFloat(data[8]) || 0;

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
        var buyprice = parseFloat(data[10]) || 0;

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
        var marginmin = parseInt($('#marginmin').val(), 10);
        var marginmax = parseInt($('#marginmax').val(), 10);
        var margin = parseFloat(data[12]) || 0;

        if ((isNaN(marginmin) && isNaN(marginmax)) ||
            (isNaN(marginmin) && margin <= marginmax) ||
            (marginmin <= margin && isNaN(marginmax)) ||
            (marginmin <= margin && margin <= marginmax)) {
            return true;
        }
        return false;
    }
);