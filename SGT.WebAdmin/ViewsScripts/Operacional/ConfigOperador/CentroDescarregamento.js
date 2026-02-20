/// <reference path="../../Consultas/CentroDescarregamento.js" />
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
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="OperadorFilial.js" />
/// <reference path="ConfigOperador.js" />
/// <reference path="OperadorTipoCarga.js" />
/// <reference path="OperadorTipoOperacao.js" />
/// <reference path="../../Consultas/CentroDescarregamento.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _gridCentroDescarregamento;
var _centroDescarregamento;

var CentroDescarregamento = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.CentroDescarregamento = PropertyEntity({ type: types.event, text: Localization.Resources.Operacional.ConfigOperador.AdicionarCentroDescarregamento, idBtnSearch: guid(), issue: 1108 });
}


//*******EVENTOS*******

function LoadCentroDescarregamento() {

    _centroDescarregamento = new CentroDescarregamento();
    KoBindings(_centroDescarregamento, "knockoutCentroDescarregamento");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: Localization.Resources.Operacional.ConfigOperador.Excluir, id: guid(), metodo: function (data) {
                ExcluirCentroDescarregamentoClick(data)
            }
        }]
    };

    var header = [{ data: "Codigo", visible: false },
    { data: "Descricao", title: Localization.Resources.Operacional.ConfigOperador.Descricao, width: "80%" }];

    _gridCentroDescarregamento = new BasicDataTable(_centroDescarregamento.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarCentrosDescarregamento(_centroDescarregamento.CentroDescarregamento, null, _gridCentroDescarregamento);
    _centroDescarregamento.CentroDescarregamento.basicTable = _gridCentroDescarregamento;

    RecarregarGridCentroDescarregamento();
}

function RecarregarGridCentroDescarregamento() {

    var data = new Array();
    if (!string.IsNullOrWhiteSpace(_operador.CentrosDescarregamento.val())) {
        $.each(_operador.CentrosDescarregamento.val(), function (i, centroDescarregamento) {
            var centroDescarregamentoGrid = new Object();

            centroDescarregamentoGrid.Codigo = centroDescarregamento.Codigo;
            centroDescarregamentoGrid.Descricao = centroDescarregamento.Descricao;

            data.push(centroDescarregamentoGrid);
        });
    }

    _gridCentroDescarregamento.CarregarGrid(data);
}


function ExcluirCentroDescarregamentoClick(data) {
    var centroDescarregamentoGrid = _centroDescarregamento.CentroDescarregamento.basicTable.BuscarRegistros();

    for (var i = 0; i < centroDescarregamentoGrid.length; i++) {
        if (data.Codigo == centroDescarregamentoGrid[i].Codigo) {
            centroDescarregamentoGrid.splice(i, 1);
            break;
        }
    }

    _centroDescarregamento.CentroDescarregamento.basicTable.CarregarGrid(centroDescarregamentoGrid);
}

function LimparCamposCentroDescarregamento() {
    LimparCampos(_centroDescarregamento);
    _centroDescarregamento.CentroDescarregamento.basicTable.CarregarGrid(new Array());
}