$(document).ready(function () {

    var domOption =
        "<'content-space'<'row'<'col-sm-6'p><'col-sm-4'f><'col-sm-2'l>>>" +
            "tr" +
            "<'content-space'<'row'<'col-sm-4'p><'col-sm-4'B><'col-sm-2'i><'col-sm-2'l>>>";

    $.fn.DataTable.ext.pager.numbers_length = 10;

    $('#ItemTable').DataTable({
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