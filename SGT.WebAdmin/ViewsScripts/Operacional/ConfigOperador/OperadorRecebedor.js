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

var _gridRecebedor;
var _recebedor;

var Recebedor = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Recebedor = PropertyEntity({ type: types.event, text: Localization.Resources.Operacional.ConfigOperador.AdicionarRecebedor, idBtnSearch: guid() });
    this.RegraRecebedorSeraSobrepostaNasDemais = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, text: Localization.Resources.Operacional.ConfigOperador.OuIndicaRegraSobreposta, visible: ko.observable(true) });
};

//*******EVENTOS*******

function LoadRecebedor() {
    _recebedor = new Recebedor();
    KoBindings(_recebedor, "knockoutOperadorRecebedor");

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Operacional.ConfigOperador.Excluir, id: guid(), metodo: function (data) { ExcluirRecebedorClick(data) } }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Operacional.ConfigOperador.Descricao, width: "80%" }
    ];

    _gridRecebedor = new BasicDataTable(_recebedor.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarClientes(_recebedor.Recebedor, null, null, null, null, _gridRecebedor);
    _recebedor.Recebedor.basicTable = _gridRecebedor;

    RecarregarGridRecebedor();
}

function RecarregarGridRecebedor() {

    var data = new Array();

    if (!string.IsNullOrWhiteSpace(_operador.Recebedores.val())) {
        $.each(_operador.Recebedores.val(), function (i, recebedor) {
            var recebedorGrid = new Object();

            recebedorGrid.Codigo = recebedor.Codigo;
            recebedorGrid.Descricao = recebedor.Descricao;

            data.push(recebedorGrid);
        });
    }

    _gridRecebedor.CarregarGrid(data);
}

function ExcluirRecebedorClick(data) {
    var recebedorGrid = _recebedor.Recebedor.basicTable.BuscarRegistros();

    for (var i = 0; i < recebedorGrid.length; i++) {
        if (data.Codigo == recebedorGrid[i].Codigo) {
            recebedorGrid.splice(i, 1);
            break;
        }
    }

    _recebedor.Recebedor.basicTable.CarregarGrid(recebedorGrid);
}

function LimparCamposRecebedor() {
    LimparCampos(_recebedor);
    _recebedor.Recebedor.basicTable.CarregarGrid(new Array());
}

function ObterRegraRecebedorSeraSobrepostaNasDemais() {
    _operador.RegraRecebedorSeraSobrepostaNasDemais.val(_recebedor.RegraRecebedorSeraSobrepostaNasDemais.val());
}