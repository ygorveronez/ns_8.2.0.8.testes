ko.observableArray.fn.where = function (checkFn) {
    return ko.utils.arrayFirst(this(), checkFn);
}

ko.observableArray.fn.update = function (checkFn, replaceFn) {
    var item = ko.utils.arrayFirst(this(), checkFn);

    if (!item) return false;

    var newItem = $.extend({}, item);
    newItem = replaceFn(newItem); 

    this.replace(item, newItem);
    return true;
}