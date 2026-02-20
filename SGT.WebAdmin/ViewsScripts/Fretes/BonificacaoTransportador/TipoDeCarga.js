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
/// <reference path="../../Consultas/TipoDeCarga.js" />
/// <reference path="../../Consultas/ComponenteFrete.js" />
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="../../Consultas/TipoDeCarga.js" />
/// <reference path="BonificacaoTransportador.js" />
/// <reference path="Filial.js" />

var _gridTiposDeCarga;

function loadTiposDeCarga() {

    var excluir = {
        descricao: "Remover", id: guid(), evento: "onclick", metodo: function (data) {
            RemoverTipoDeCargaClick(_bonificacaoTransportador.TiposDeCargaInfo, data)
        }, tamanho: "15", icone: ""
    };
    menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [excluir] }
    var header = [{ data: "Codigo", visible: false },
                 { data: "Descricao", title: "Descrição", width: "85%", className: "text-align-left" }
    ];

    _gridTiposDeCarga = new BasicDataTable(_bonificacaoTransportador.TiposDeCargaInfo.idGrid, header, menuOpcoes);
    _bonificacaoTransportador.TiposDeCargaInfo.basicTable = _gridTiposDeCarga;

    new BuscarTiposdeCarga(_bonificacaoTransportador.TiposDeCargaInfo, RetornoInserirTipoDeCarga, null, _gridTiposDeCarga);
    RecarregarTiposDeCarga();
}

function RecarregarTiposDeCarga() {
    var cont = 0;
    var total = 0;
    var data = new Array();

    if (!string.IsNullOrWhiteSpace(_bonificacaoTransportador.TiposDeCarga.val())) {

        $.each(_bonificacaoTransportador.TiposDeCarga.val(), function (i, tipoDeCarga) {
            var obj = new Object();
            obj.Codigo = tipoDeCarga.Codigo;
            obj.Descricao = tipoDeCarga.Descricao;
            data.push(obj);
        });
    }
    _gridTiposDeCarga.CarregarGrid(data);
}


function preencherListaTipoDeCarga() {
    _bonificacaoTransportador.TiposDeCarga.list = new Array();
    var tiposDeCarga = new Array();
    $.each(_bonificacaoTransportador.TiposDeCargaInfo.basicTable.BuscarRegistros(), function (i, tipoDeCarga) {
        tiposDeCarga.push(tipoDeCarga);
    });
    _bonificacaoTransportador.TiposDeCarga.val(JSON.stringify(tiposDeCarga))
}

function RetornoInserirTipoDeCarga(data) {
    if (data != null) {
        var dataGrid = _gridTiposDeCarga.BuscarRegistros();

        for (var i = 0; i < data.length; i++) {
            var obj = new Object();
            obj.Codigo = data[i].Codigo;
            obj.Descricao = data[i].Descricao;
            dataGrid.push(obj);
        }
        _gridTiposDeCarga.CarregarGrid(dataGrid);
    }
}

function RemoverTipoDeCargaClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a tipo de carga " + sender.Descricao + "?", function () {
        var tipoDeCargaGrid = e.basicTable.BuscarRegistros();

        for (var i = 0; i < tipoDeCargaGrid.length; i++) {
            if (sender.Codigo == tipoDeCargaGrid[i].Codigo) {
                tipoDeCargaGrid.splice(i, 1);
                break;
            }
        }
        e.basicTable.CarregarGrid(tipoDeCargaGrid);
    });
}