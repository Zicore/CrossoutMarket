var defaultOrder = [[4, "asc"]];

$(document).ready(function () {

    adjustTimeStamp();

    var domOption =
        "<'content-space'<'row'<'col-sm-6'p><'col-sm-4'f><'col-sm-2'l>>>" +
        "tr" +
        "<'content-space'<'row'<'col-sm-4'p><'col-sm-4'B><'col-sm-2'i><'col-sm-2'l>>>";

    $.fn.DataTable.ext.pager.numbers_length = 10;

    var order = [[4, "asc"]];

    var table = $('#ItemTable').DataTable({
        paging: true,
        searching: true,
        search: {
            smart: false,
            regex: true
        },
        buttons: {
            dom: {
                container: {
                    tag: 'ul',
                    className: 'pagination dataTables_button'
                },
                buttonContainer: {
                    tag: 'li',
                    className: 'paginate_button'
                },
                button: {
                    tag: 'a',
                    className: ''
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
        pagingType: "full_numbers",
        dom: domOption
    });

    $('#sellmin, #sellmax, #buymin, #buymax, #marginmin, #marginmax').keyup(function () {
        table.draw();
    });

    $('.filter-faction').click(function (e) {
        var text = $(this).text();
        $('.filter-faction').each(function () {
            if (text == $(this).text())
            {
                $(this).parent().toggleClass('active');
            }
        });
        filterTable(table);
        updateLocationHash(table);
        e.preventDefault();
    });

    $('.filter-rarity').click(function (e) {
        var text = $(this).text();
        $('.filter-rarity').each(function () {
            if (text == $(this).text()) {
                $(this).parent().toggleClass('active');
            }
        });
        filterTable(table);
        updateLocationHash(table);
        e.preventDefault();
    });

    $('.filter-category').click(function (e) {
        var text = $(this).text();
        $('.filter-category').each(function () {
            if (text == $(this).text()) {
                $(this).parent().toggleClass('active');
            }
        });
        filterTable(table);
        updateLocationHash(table);
        e.preventDefault();
    });

    getFilterStateFromCookie();
    applyColumnVis(table);

    applyLocationHash(table);
});

const columnList = ['name', 'rarity', 'faction', 'category', 'type', 'popularity', 'sellprice', 'selloffers', 'buyprice', 'buyorders', 'margin', 'lastupdate'];

function getFilterStateFromCookie() {
    columnList.forEach(function (e, i) {
        var cookieval = Cookies.get('showColumn-' + e);
        if (cookieval == 'true') {
            $('.colvis-' + e).parent().addClass('active');
        } else if (cookieval == 'false') {
            $('.colvis-' + e).parent().removeClass('active');
        }
    });
}

function filterTable(table) {
    var filterFactionString;
    var j = 0;
    $('.filter-faction').each(function (i, e) {
        if ($(this).parent().hasClass('active'))
        {
            var faction = $(this).parent().text();
            faction = faction.split(' ')[0];

            if (j == 0) {
                filterFactionString = faction;
            } else {
                filterFactionString += '|' + faction;
            }
            j++;
        }
    });
    if (filterFactionString !== undefined)
    {
        table.column(2).search(filterFactionString, true);
    } else {
        table.column(2).search('');
    }

    var filterRarityString;
    j = 0;
    $('.filter-rarity').each(function (i, e) {
        if ($(this).parent().hasClass('active')) {
            if (j == 0) {
                filterRarityString = $(this).parent().text();
            } else {
                filterRarityString += '|' + $(this).parent().text();
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
        if ($(this).parent().hasClass('active')) {
            var category = $(this).parent().text();
            category = category.split(' ')[0];

            if (j == 0) {
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

    table.draw();
}

function applyColumnVis(table) {
    $('.colvis').each(function () {
        currentCol = $(this);
        columnList.forEach(function (e, i) {
            if (currentCol.hasClass('colvis-' + e))
            {
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
    })
}

var selectedList = [];
var highlightSelectClass = 'info';

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
        var sellprice = parseFloat(data[5]) || 0;

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
        var buyprice = parseFloat(data[7]) || 0;

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
        var margin = parseFloat(data[9]) || 0;

        if ((isNaN(marginmin) && isNaN(marginmax)) ||
            (isNaN(marginmin) && margin <= marginmax) ||
            (marginmin <= margin && isNaN(marginmax)) ||
            (marginmin <= margin && margin <= marginmax)) {
            return true;
        }
        return false;
    }
);
