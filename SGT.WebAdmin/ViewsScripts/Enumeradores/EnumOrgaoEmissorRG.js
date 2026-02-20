var EnumOrgaoEmissorRGHelper = function () {
    this.SSP = 1;
    this.CNH = 2;
    this.MMA = 3;
    this.DIC = 4;
    this.POF = 5;
    this.IFP = 6;
    this.POM = 7;
    this.IPF = 8;
    this.SES = 9;
    this.MAE = 10;
    this.MEX = 11;
    this.SJS = 12;
    this.SJ = 13;
    this.SPTC = 14;
    this.SECC = 15;
    this.SEJUSP = 16;
}

EnumOrgaoEmissorRGHelper.prototype = {
    descricoes: function () {
        return [
            { text: "SSP", value: this.SSP },
            { text: "CNH", value: this.CNH },
            { text: "MMA", value: this.MMA },
            { text: "DIC", value: this.DIC },
            { text: "POF", value: this.POF },
            { text: "IFP", value: this.IFP },
            { text: "POM", value: this.POM },
            { text: "IPF", value: this.IPF },
            { text: "SES", value: this.SES },
            { text: "MAE", value: this.MAE },
            { text: "MEX", value: this.MEX },
            { text: "SJS", value: this.SJS },
            { text: "SJ", value: this.SJ },
            { text: "SPTC", value: this.SPTC },
            { text: "SECC", value: this.SECC },
            { text: "SEJUSP", value: this.SEJUSP }
        ];
    },

    obterOpcoes: function (options, opcaoTodos) {
        var _options = options || [];
        var arrayOptions = [];
        var _descricoes = this.descricoes();

        if (_options.length == 0) {
            for (var i in this) {
                if (this.hasOwnProperty(i)) _options.push(this[i]);
            }
        }

        if (opcaoTodos)
            arrayOptions.push({ text: 'Todos', value: '' });

        for (var i in _descricoes) {
            if ($.inArray(_descricoes[i].value, _options) > -1)
                arrayOptions.push(_descricoes[i]);
        }

        return arrayOptions;
    }
}

var EnumOrgaoEmissorRG = Object.freeze(new EnumOrgaoEmissorRGHelper());