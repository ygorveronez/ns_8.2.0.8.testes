/// <reference path="../../Consultas/CanalEntrega.js" />
/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="TipoOcorrencia.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridCanaisDeEntrega;
var _canaisDeEntrega;

var CanaisDeEntrega = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Canal = PropertyEntity({ type: types.event, text: Localization.Resources.Ocorrencias.TipoOcorrencia.AdicionarCanal, idBtnSearch: guid() });
}

//*******EVENTOS*******

function loadCanaisDeEntrega() {
    _canaisDeEntrega = new CanaisDeEntrega();
    KoBindings(_canaisDeEntrega, "knockoutCanaisDeEntrega");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: Localization.Resources.Ocorrencias.TipoOcorrencia.Excluir, id: guid(), metodo: function (data) {
                excluirCanalClick(_canaisDeEntrega.Canal, data)
            }
        }]
    };

    var header = [{ data: "Codigo", visible: false }, { data: "Descricao", title: Localization.Resources.Ocorrencias.TipoOcorrencia.Descricao, width: "80%" }];

    _gridCanaisDeEntrega = new BasicDataTable(_canaisDeEntrega.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarCanaisEntrega(_canaisDeEntrega.Canal, null, _gridCanaisDeEntrega);

    _canaisDeEntrega.Canal.basicTable = _gridCanaisDeEntrega;

    _canaisDeEntrega.Canal.basicTable.BuscarRegistros()

    recarregarGridCanais();
}

function recarregarGridCanais() {
    var data = new Array();

    if (!string.IsNullOrWhiteSpace(_tipoOcorrencia.CanaisDeEntrega.val())) {

        $.each(_tipoOcorrencia.CanaisDeEntrega.val(), function (i, canal) {
            var transportadorGrid = new Object();

            transportadorGrid.Codigo = canal.Codigo;
            transportadorGrid.Descricao = canal.Descricao;

            data.push(transportadorGrid);
        });
    }

    _gridCanaisDeEntrega.CarregarGrid(data);
}


function excluirCanalClick(knoutCanal, data) {
    var registros = knoutCanal.basicTable.BuscarRegistros();

    for (var i = 0; i < registros.length; i++) {
        if (data.Codigo == registros[i].Codigo) {
            registros.splice(i, 1);
            break;
        }
    }

    knoutCanal.basicTable.CarregarGrid(registros);
}

function limparCamposTransportador() {
    LimparCampos(_canaisDeEntrega);
}

function obterGridCanaisDeEntrega() {
    return _gridCanaisDeEntrega;
}

function limparGridCanaisDeEntrega() {
    var registros = [];
    _gridCanaisDeEntrega.SetarRegistros(registros);
    _gridCanaisDeEntrega.CarregarGrid(registros);
}

function preencherGridCanaisDeEntregaSalvar(tabelaFrete) {
    var canais = _gridCanaisDeEntrega.BuscarRegistros().slice();
    var canaisSalvar = [];
    for (var i = 0; i < canais.length; i++) {
        canaisSalvar.push(canais[i].Codigo);
    }
    tabelaFrete["Canais"] = JSON.stringify(canaisSalvar);
}