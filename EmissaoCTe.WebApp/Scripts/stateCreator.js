/**
 * State creator
 */
State = function (opt) {
    opt = $.extend({
        name: "",
        id: "id",
        render: function () { }
    }, opt);

    var STATE = [];
    var ID = opt.id;
    this.name = opt.name;
    this.render = opt.render;

    this.count = function () {
        return Count();
    }
    this.get = function (where) {
        if (arguments.length == 0)
            where = null;

        return Clone(Get(where));
    }
    this.first = function (where) {
        var data = this.get(where);

        if (data.length >= 1)
            data = data[0];
        else
            data = {};

        return data;
    }
    this.set = function (data) {
        Set(Clone(data));
        this.render();
    }
    this.remove = function (where) {
        Del(where);
        this.render();
    }
    this.insert = function (item, autoincrement) {
        if (arguments.length == 1)
            autoincrement = true;
        Add(Clone(item), autoincrement);
        this.render();
    }
    this.update = function (data) {
        Update(Clone(data));
        this.render();
    }
    this.clear = function () {
        Clear();
        this.render();
    }
    this.nextid = function () {
        return NextId();
    }
    this.toJson = function () {
        return JSON.stringify(STATE);
    }


    var Get = function (where) {
        var data = STATE;

        /**
        * Data é o array de valores salvos no state
        * Where é um objeto de comparacao
        */
        if (where == null) return data;

        /**
        * O array de dados é percorrido
        */
        var dataReturn = [];
        for (var i = 0; i < data.length; i++) {
            /**
            * O objeto de comparacao é analsiado
            * Caso o nome do parametro nao exista no dado ou o valor dos campos nao sao iguais
            * Entao nao havera exclusao
            */
            for (j in where)
                if (typeof data[i][j] != "undefined" && data[i][j] == where[j])
                    dataReturn.push(data[i]);
        }


        return dataReturn;
    }

    var Count = function () {
        return STATE.length;
    }

    var Set = function (data) {
        if (!$.isArray(data)) return false;

        STATE = data;
    }

    var Add = function (item, autoincrement) {
        if (autoincrement && typeof item[ID] != "undefined" && item[ID] == 0)
            item[ID] = NextId();

        STATE.push(item);
    }

    var Clear = function () {
        STATE = [];
    }

    var NextId = function () {
        var nextCode = 0;

        for (var i in STATE) {
            var item = STATE[i];

            if (item[ID] < 0) {
                var loopId = Math.abs(item[ID]);
                if (loopId > nextCode)
                    nextCode = loopId;
            }
        }

        return -(nextCode + 1);
    }

    var Del = function (where) {
        /**
         * Exemplo:
         * data = [{id: 1, name: "foo", type: 1}, {id: 3, name: "bar", type: 1}, {id: 4, name: "baz"}, {id: 5, name: "foobar", type: 2}]
         *
         *
         * where = {id: 1} -> [{id: 3, name: "bar", type: 1}, {id: 4, name: "baz"}, {id: 5, name: "foobar", type: 2}]
         * where = {name: "bar"} -> [{id: 1, name: "foo", type: 1}, {id: 4, name: "baz"}, {id: 5, name: "foobar", type: 2}]
         * where = {type: 1} -> [{id: 4, name: "baz"}, {id: 5, name: "foobar", type: 2}]
         */


        /**
        * Data é o array de valores salvos no state
        * Where é um objeto de comparacao
        */
        var data = STATE;
        if ($.isEmptyObject(where)) return false;

        /**
        * O array de dados é percorrido
        */
        for (var i = 0; i < data.length; i++) {
            var del = true;

            /**
            * O objeto de comparacao é analsiado
            * Caso o nome do parametro nao exista no dado ou o valor dos campos nao sao iguais
            * Entao nao havera exclusao
            */
            for (j in where)
                if (typeof data[i][j] == "undefined" || !(data[i][j] == where[j])) del = false;

            if (del) {
                if (ID in data[i]) {
                    if (data[i][ID] <= 0) {
                        data.splice(i, 1);
                        i--;
                    } else
                        data[i].Excluir = true;
                } else {
                    data.splice(i, 1);
                    i--;
                }
            }
        }

        /**
        * O array de dados é salvo subtituido os valores antigos
        */
        STATE = data;
    }

    var Update = function (newData) {
        /**
        * NewData é o novo obj
        * Where é um objeto de comparacao
        */
        var data = STATE;

        /**
        * O array de dados é percorrido
        */
        for (var i = 0; i < data.length; i++) {

            /**
            * E feito a verificacao do objeto com o ID
            * Caso seja o mesmo
            * O novo valor e inserido
            */
            if (data[i][ID] == newData[ID]) {
                data[i] = newData;
            }
        }

        /**
        * O array de dados é salvo subtituido os valores antigos
        */
        STATE = data;
    }

    var Clone = function (obj) {
        var copy;
        var cloneObj = function (cobj) {
            // Handle the 3 simple types, and null or undefined
            if (null == cobj || "object" != typeof cobj) return cobj;

            objcopy = {};
            for (var attr in cobj) {
                if (cobj.hasOwnProperty(attr)) objcopy[attr] = cloneObj(cobj[attr]);
            }
            return objcopy;
        }

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
                copy[i] = cloneObj(obj[i]);
            }
            return copy;
        }

        // Handle Object
        if (obj instanceof Object)
            return cloneObj(obj);


        return obj;
    }
}