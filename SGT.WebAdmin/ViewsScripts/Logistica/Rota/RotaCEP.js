/// <reference path="Rota.js" />

var _gridCEP;

var CEPMap = function () {
    this.CodigoCEP = PropertyEntity({ type: types.map, val: 0, def: 0, getType: typesKnockout.int });
    this.CEPInicial = PropertyEntity({ type: types.map, val: "" });
    this.CEPFinal = PropertyEntity({ type: types.map, val: "" });
}

//*******EVENTOS*******

function loadRotaCEP() {

    preencherRotaCEP();
    $("#" + _rota.CEPInicial.id).mask("00.000-000", { selectOnFocus: true, clearIfNotMatch: true });
    $("#" + _rota.CEPFinal.id).mask("00.000-000", { selectOnFocus: true, clearIfNotMatch: true });
}


function preencherRotaCEP() {

    var detalhe = {
        descricao: "Editar", id: guid(), evento: "onclick", metodo: function (data) {
            EditarCEPClick(data)
        }, tamanho: "10", icone: ""
    };
    var excluir = {
        descricao: "Remover", id: guid(), evento: "onclick", metodo: function (data) {
            excluirCEP(data)
        }, tamanho: "10", icone: ""
    };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.list;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(detalhe);
    menuOpcoes.opcoes.push(excluir);

    var header = [{ data: "CodigoCEP", visible: false },
    { data: "CEPInicial", title: "CEP Inicial", width: "40%" },
    { data: "CEPFinal", title: "CEP Final", width: "40%" }];

    _gridCEP = new BasicDataTable(_rota.CEPs.idGrid, header, menuOpcoes);
    recarregarGridRotaCEP();
}

function recarregarGridRotaCEP() {
    var data = new Array();
    $.each(_rota.CEPs.list, function (i, cep) {
        var obj = new Object();

        obj.CodigoCEP = cep.CodigoCEP.val;
        obj.CEPInicial = cep.CEPInicial.val;
        obj.CEPFinal = cep.CEPFinal.val;

        data.push(obj);
    });
    _gridCEP.CarregarGrid(data);
}

function AdicionarCEPClick(e, sender) {
    var tudoCerto = true;
    if (_rota.CEPInicial.val() == "")
        tudoCerto = false;
    if (_rota.CEPFinal.val() == "")
        tudoCerto = false;

    if (tudoCerto) {
        if (_rota.CodigoCEP.val() != "") {
            $.each(_rota.CEPs.list, function (i, cep) {
                if (cep != null && _rota.CodigoCEP.val() == cep.CodigoCEP.val)
                    _rota.CEPs.list.splice(i, 1);
            });
        }
        var map = new CEPMap();

        if (_rota.CodigoCEP.val() > 0)
            map.CodigoCEP.val = _rota.CodigoCEP.val();
        else
            map.CodigoCEP.val = guid();

        map.CEPInicial.val = _rota.CEPInicial.val();
        map.CEPFinal.val = _rota.CEPFinal.val();

        _rota.CEPs.list.push(map);


        recarregarGridRotaCEP();
        limparCEP();
        _rota.AdicionarCEP.text("Adicionar");
        $("#" + _rota.CEPInicial.id).focus();
    } else {

        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios no laçamento da faixa de CEP!");
    }
}

function excluirCEP(e) {
    exibirConfirmacao("Confirmação", "Realmente deseja remover a faixa de CEP selecionada?", function () {
        $.each(_rota.CEPs.list, function (i, cep) {
            if (cep != null && cep.CodigoCEP.val != null && e != null && e.CodigoCEP != null && e.CodigoCEP == cep.CodigoCEP.val)
                _rota.CEPs.list.splice(i, 1);
        });
        recarregarGridRotaCEP();
    });
}

function EditarCEPClick(e) {
    _rota.CEPInicial.val(e.CEPInicial);
    _rota.CEPFinal.val(e.CEPFinal);
    _rota.CodigoCEP.val(e.CodigoCEP);

    _rota.AdicionarCEP.text("Atualizar");
}

function limparCEP() {
    $("#" + _rota.CEPInicial.id).val("");
    $("#" + _rota.CEPFinal.id).val("");

    _rota.CEPInicial.val("");
    _rota.CEPFinal.val("");
    _rota.CodigoCEP.val("");    
}