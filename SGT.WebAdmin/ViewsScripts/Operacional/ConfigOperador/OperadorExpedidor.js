/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="ConfigOperador.js" />
/// <reference path="../../Consultas/Cliente.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridExpedidor;
var _expedidor;

var Expedidor = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Expedidor = PropertyEntity({ type: types.event, text: Localization.Resources.Operacional.ConfigOperador.AdicionarExpedidor, idBtnSearch: guid() });
    this.RegraExpedidorSeraSobrepostaNasDemais = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, text: Localization.Resources.Operacional.ConfigOperador.OuIndicaRegraSobreposta, visible: ko.observable(true) });
};

//*******EVENTOS*******

function LoadExpedidor() {
    _expedidor = new Expedidor();
    KoBindings(_expedidor, "knockoutOperadorExpedidor");

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Operacional.ConfigOperador.Excluir, id: guid(), metodo: function (data) { ExcluirExpedidorClick(data) } }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Operacional.ConfigOperador.Descricao, width: "80%" }
    ];

    _gridExpedidor = new BasicDataTable(_expedidor.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarClientes(_expedidor.Expedidor, null, null, null, null, _gridExpedidor);
    _expedidor.Expedidor.basicTable = _gridExpedidor;

    RecarregarGridExpedidor();
}

function RecarregarGridExpedidor() {

    var data = new Array();

    if (!string.IsNullOrWhiteSpace(_operador.Expedidores.val())) {
        $.each(_operador.Expedidores.val(), function (i, expedidor) {
            var expedidorGrid = new Object();

            expedidorGrid.Codigo = expedidor.Codigo;
            expedidorGrid.Descricao = expedidor.Descricao;

            data.push(expedidorGrid);
        });
    }

    _gridExpedidor.CarregarGrid(data);
}

function ExcluirExpedidorClick(data) {
    var expedidorGrid = _expedidor.Expedidor.basicTable.BuscarRegistros();

    for (var i = 0; i < expedidorGrid.length; i++) {
        if (data.Codigo == expedidorGrid[i].Codigo) {
            expedidorGrid.splice(i, 1);
            break;
        }
    }

    _expedidor.Expedidor.basicTable.CarregarGrid(expedidorGrid);
}

function LimparCamposExpedidor() {
    LimparCampos(_expedidor);
    _expedidor.Expedidor.basicTable.CarregarGrid(new Array());
}

function ObterRegraExpedidorSeraSobrepostaNasDemais() {
    _operador.RegraExpedidorSeraSobrepostaNasDemais.val(_expedidor.RegraExpedidorSeraSobrepostaNasDemais.val());
}