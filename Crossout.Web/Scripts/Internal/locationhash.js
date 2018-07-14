function applyLocationHash(table) {
    var hash = location.hash;
    var patttern = '#faction=(.*,?)\.rarity=(.*,?)\.category=(.*,?)\.';
    var regEx = new RegExp(patttern, 'ig');
    var matches = regEx.exec(hash);
    if (matches != null) {
        matches.forEach(function (match, i) {
            if (i !== 0) {
                var items = match.split(',');

                items.forEach(function (item, j) {
                    if (i === 1) {
                        $('.filter-faction').each(function (k, e) {
                            if (k < $('.filter-faction').toArray().length) {
                                var targetString = $(this).parent().text().toLowerCase();
                                targetString = targetString.replace(' ', '');
                                if (targetString === item) {
                                    $(this).parent().addClass('active');
                                }
                            }
                        });
                    } else if (i === 2) {
                        $('.filter-rarity').each(function (k, e) {
                            if (k < $('.filter-rarity').toArray().length) {
                                var targetString = $(this).parent().text().toLowerCase();
                                targetString = targetString.replace(' ', '');
                                if (targetString === item) {
                                    $(this).parent().addClass('active');
                                }
                            }
                        });
                    } else if (i === 3) {
                        $('.filter-category').each(function (k, e) {
                            if (k < $('.filter-category').toArray().length) {
                                var targetString = $(this).parent().text().toLowerCase();
                                targetString = targetString.replace(' ', '');
                                if (targetString === item) {
                                    $(this).parent().addClass('active');
                                }
                            }
                        });
                    }
                });
            }
        });
        filterTable(table);
    }
}

function updateLocationHash() {
    var newHash = '#';

    newHash += 'faction=';
    $('.filter-faction').each(function (k, e) {
        if (k < $('.filter-faction').toArray().length / 2) {
            if ($(this).parent().hasClass('active')) {
                var targetString = $(this).parent().text().toLowerCase();
                targetString = targetString.replace(' ', '');
                newHash += targetString + ',';
            }
        }
    });
    if (newHash.endsWith(',')) {
        newHash = newHash.substr(0, newHash.length - 1);
    }
    newHash += '.';

    newHash += 'rarity=';

    $('.filter-rarity').each(function (k, e) {
        if (k < $('.filter-rarity').toArray().length / 2) {
            if ($(this).parent().hasClass('active')) {
                var targetString = $(this).parent().text().toLowerCase();
                targetString = targetString.replace(' ', '');
                newHash += targetString + ',';
            }
        }
    });
    if (newHash.endsWith(',')) {
        newHash = newHash.substr(0, newHash.length - 1);
    }
    newHash += '.';

    newHash += 'category=';

    $('.filter-category').each(function (k, e) {
        if (k < $('.filter-category').toArray().length / 2) {
            if ($(this).parent().hasClass('active')) {
                var targetString = $(this).parent().text().toLowerCase();
                targetString = targetString.replace(' ', '');
                newHash += targetString + ',';
            }
        }
    });
    if (newHash.endsWith(',')) {
        newHash = newHash.substr(0, newHash.length - 1);
    }
    newHash += '.';
    location.hash = newHash;
}