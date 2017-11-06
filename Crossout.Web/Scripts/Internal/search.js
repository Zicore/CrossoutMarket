$(document).ready(function () {

    adjustTimeStamp();

    var domOption =
        "<'content-space'<'row'<'col-sm-6'p><'col-sm-4'f><'col-sm-2'l>>>" +
        "tr" +
        "<'content-space'<'row'<'col-sm-4'p><'col-sm-4'B><'col-sm-2'i><'col-sm-2'l>>>";

    $.fn.DataTable.ext.pager.numbers_length = 10;

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
        order: [[4, "asc"]],
        lengthMenu: [[10, 20, 50, -1], [10, 20, 50, "All"]],
        pagingType: "full_numbers",
        dom: domOption
    });

    $('#sellmin, #sellmax, #buymin, #buymax, #marginmin, #marginmax').keyup(function () {
        table.draw();
    });
});

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