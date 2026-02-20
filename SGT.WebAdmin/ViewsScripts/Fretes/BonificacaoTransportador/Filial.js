/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/Empresa.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/ComponenteFrete.js" />
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="../../Consultas/TipoDeCarga.js" />
/// <reference path="BonificacaoTransportador.js" />

var _gridFiliais;

function loadFiliais() {

    var excluir = {
        descricao: "Remover", id: guid(), evento: "onclick", metodo: function (data) {
            RemoverFilialClick(_bonificacaoTransportador.FiliaisInfo, data)
        }, tamanho: "15", icone: ""
    };
    menuOpcoes = { tipo: TypeOptionMenu.link, opcoes : [excluir] }
    var header = [{ data: "Codigo", visible: false },
                 { data: "Descricao", title: "Descrição", width: "85%", className: "text-align-left" }
    ];

    _gridFiliais = new BasicDataTable(_bonificacaoTransportador.FiliaisInfo.idGrid, header, menuOpcoes);
    _bonificacaoTransportador.FiliaisInfo.basicTable = _gridFiliais;

    new BuscarFilial(_bonificacaoTransportador.FiliaisInfo, RetornoInserirFilial, _gridFiliais);
    RecarregarFiliais();
}

function RecarregarFiliais() {
    var cont = 0;
    var total = 0;
    var data = new Array();

    if (!string.IsNullOrWhiteSpace(_bonificacaoTransportador.Filiais.val())) {

        $.each(_bonificacaoTransportador.Filiais.val(), function (i, filial) {
            var obj = new Object();
            obj.Codigo = filial.Codigo;
            obj.Descricao = filial.Descricao;
            data.push(obj);
        });
    }
    _gridFiliais.CarregarGrid(data);
}


function preencherListaFilial() {
    _bonificacaoTransportador.Filiais.list = new Array();
    var filiais = new Array();
    $.each(_bonificacaoTransportador.FiliaisInfo.basicTable.BuscarRegistros(), function (i, filial) {
        filiais.push(filial);
    });
    _bonificacaoTransportador.Filiais.val(JSON.stringify(filiais))
}

function RetornoInserirFilial(data) {
    if (data != null) {
        var dataGrid = _gridFiliais.BuscarRegistros();

        for (var i = 0; i < data.length; i++) {
            var obj = new Object();
            obj.Codigo = data[i].Codigo;
            obj.Descricao = data[i].Descricao;
            dataGrid.push(obj);
        }
        _gridFiliais.CarregarGrid(dataGrid);
    }
}

function RemoverFilialClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a filial " + sender.Descricao + "?", function () {
        var filialGrid = e.basicTable.BuscarRegistros();

        for (var i = 0; i < filialGrid.length; i++) {
            if (sender.Codigo == filialGrid[i].Codigo) {
                filialGrid.splice(i, 1);
                break;
            }
        }
        e.basicTable.CarregarGrid(filialGrid);
    });
}