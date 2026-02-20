var Curral = function () {
    this.LarguraCarroceria = PropertyEntity({ text: Localization.Resources.Veiculos.Veiculo.LarguraCarroceria.getFieldDescription(), getType: typesKnockout.decimal, maxlength: 15, val: ko.observable(""), def: "", configDecimal: { precision: 2, allowZero: false, allowNegative: false } });
    this.ComprimentoCurrais = PropertyEntity({ text: Localization.Resources.Veiculos.Veiculo.ComprimentoUtilDosCurrais.getFieldDescription(), getType: typesKnockout.decimal, maxlength: 15, val: ko.observable(""), def: "", configDecimal: { precision: 2, allowZero: false, allowNegative: false } });
};


function ajustarAbasCurrais(quantidade) {
    if (quantidade == 0)
        $("#currais").hide();
    else
        $("#currais").show();

    var lista = [];

    for (var i = 0; i < quantidade; i++) {
        var curral = new Curral();
        lista.push(curral);
    }

    _veiculo.ListaCurrais.val(lista);

    for (var i = 0; i < _veiculo.ListaCurrais.val().length; i++) {
        var koAtual = _veiculo.ListaCurrais.val()[i];
        $.each(koAtual, function (i, prop) {
            $("#" + prop.id).maskMoney(prop.configDecimal);
        });
    }
}

function obterListaCurrais() {
    var listaRetornar = [];

    for (var i = 0; i < _veiculo.ListaCurrais.val().length; i++) {
        var curralAtual = _veiculo.ListaCurrais.val()[i];
        var item = [{
            Largura: curralAtual.LarguraCarroceria.val(),
            Comprimento: curralAtual.ComprimentoCurrais.val()
        }];

        listaRetornar.push(item);
    }

    return JSON.stringify(listaRetornar);
}

function preencherCurrais(currais) {
    for (var i = 0; i < _veiculo.ListaCurrais.val().length; i++) {
        var curral = currais.filter(function (obj) { return obj.NumeroCurral == (i + 1) });
        var curralAtual = _veiculo.ListaCurrais.val()[i];
        curralAtual.ComprimentoCurrais.val(curral[0].Comprimento);
        curralAtual.LarguraCarroceria.val(curral[0].Largura);
    }

    if (currais.length > 0) {
        Global.ResetarAba("currais");
    }
}