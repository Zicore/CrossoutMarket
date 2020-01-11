function applyLocationHash(table) {
    var hash = location.hash;
    var pattern = '(search|faction|rarity|category|length|order)=(.*,?)';
    hash = hash.replace('#', '');
    var types = hash.split('.');
    types.forEach(function (type, i) {
        var regEx = new RegExp(pattern, 'ig');
        var matches = regEx.exec(type);
        if (matches !== null) {
            var typeName = matches[1];

            var items = matches[2].split(',');

            items.forEach(function (item, j) {
                if (typeName === "search") {
                    $('#searchBar, #searchBarMobile').val(decodeURI(item));
                }
                else if (typeName === "faction") {
                    $('.filter-faction').each(function (k, e) {
                        if (k < $('.filter-faction').toArray().length) {
                            var targetString = $(this).text().toLowerCase();
                            targetString = cleanUpString(targetString);
                            if (targetString === item) {
                                $(this).addClass('active');
                            }
                        }
                    });
                } else if (typeName === "rarity") {
                    $('.filter-rarity').each(function (k, e) {
                        if (k < $('.filter-rarity').toArray().length) {
                            var targetString = $(this).text().toLowerCase();
                            targetString = cleanUpString(targetString);
                            if (targetString === item) {
                                $(this).addClass('active');
                            }
                        }
                    });
                } else if (typeName === "category") {
                    $('.filter-category').each(function (k, e) {
                        if (k < $('.filter-category').toArray().length) {
                            var targetString = $(this).text().toLowerCase();
                            targetString = cleanUpString(targetString);
                            if (targetString === item) {
                                $(this).addClass('active');
                            }
                        }
                    });
                } else if (typeName === "length") {
                    table.page.len(item).draw();
                } 
                else if (typeName === "order") {
                    var columnNumber;
                    if (item.includes('asc')) {
                        columnNumber = item.replace('asc', '');
                        table.order([columnNumber, 'asc']);
                    } else if (item.includes('desc')) {
                        columnNumber = item.replace('desc', '');
                        table.order([columnNumber, 'desc']);
                    }
                }
            });
            filterTable(table);
        }
    }); 
}

function updateLocationHash(table) {
    var newHash = '#';

    var searchVal = $('#searchBar').val();
    if (searchVal !== '')
        newHash += 'search=' + searchVal + '.';

    $('.filter-faction').each(function (k, e) {
        if (k < $('.filter-faction').toArray().length / 2) {
            if ($(this).hasClass('active')) {
                if (!newHash.includes('faction=')) {
                    newHash += 'faction=';
                }
                var targetString = $(this).text().toLowerCase();
                targetString = cleanUpString(targetString);
                newHash += targetString + ',';
            }
        }
    });
    if (newHash.endsWith(',')) {
        newHash = newHash.substr(0, newHash.length - 1);
    }
    if (newHash.includes('faction=')) {
        newHash += '.';
    }

    $('.filter-rarity').each(function (k, e) {
        if (k < $('.filter-rarity').toArray().length / 2) {
            if ($(this).hasClass('active')) {
                if (!newHash.includes('rarity=')) {
                    newHash += 'rarity=';
                }
                var targetString = $(this).text().toLowerCase();
                targetString = cleanUpString(targetString);
                newHash += targetString + ',';
            }
        }
    });
    if (newHash.endsWith(',')) {
        newHash = newHash.substr(0, newHash.length - 1);
    }
    if (newHash.includes('rarity=')) {
        newHash += '.';
    }

    $('.filter-category').each(function (k, e) {
        if (k < $('.filter-category').toArray().length / 2) {
            if ($(this).hasClass('active')) {
                if (!newHash.includes('category=')) {
                    newHash += 'category=';
                }
                var targetString = $(this).text().toLowerCase();
                targetString = cleanUpString(targetString);
                newHash += targetString + ',';
            }
        }
    });
    if (newHash.endsWith(',')) {
        newHash = newHash.substr(0, newHash.length - 1);
    }
    if (newHash.includes('category=')) {
        newHash += '.';
    }

    if (table.page.info().length !== $.fn.DataTable.ext.pager.numbers_length) {
        if (!newHash.includes('length=')) {
            newHash += 'length=';
        }
        newHash += table.page.info().length;
    }
    if (newHash.includes('length=')) {
        newHash += '.';
    }

    var newOrder = table.order();
    if (newOrder.length > 0 && newOrder[0].toString() !== defaultOrder[0].toString()) {
        if (!newHash.includes('order=')) {
            newHash += 'order=';
        }
        newHash += newOrder[0].toString().replace(',', '');
    }
    if (newHash.includes('order=')) {
        newHash += '.';
    }

    location.hash = newHash;
}

function cleanUpString(targetString) {
    targetString = targetString.replace(' ', '');
    targetString = targetString.replace('\'', '');
    return targetString;
}

$('#ItemTable').on('length.dt', function (e, settings, len) {
    updateLocationHash($('#ItemTable').DataTable());
});

var isInitialSortingOver = false;
$('#ItemTable').on('order.dt', function () {
    if (isInitialSortingOver) {
        updateLocationHash($('#ItemTable').DataTable());
    } else {
        isInitialSortingOver = true;
    }
});