/// <reference path="../../Consultas/CentroCarregamento.js" />
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
/// <reference path="../../Consultas/CentroCarregamento.js" />
/// <reference path="../../Enumeradores/EnumTipoTransportadorCentroCarregamento.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _gridCentroCarregamento;
var _centroCarregamento;

var CentroCarregamento = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.CentroCarregamento = PropertyEntity({ type: types.event, text: Localization.Resources.Operacional.ConfigOperador.AdicionarCentroCarregamento, idBtnSearch: guid(), issue: 320 });
    this.PermiteSelecionarHorarioEncaixe = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Operacional.ConfigOperador.PermiteSelecionarHorarioEncaixe, val: ko.observable(false), def: false });
    //this.TipoTransportador = PropertyEntity({ getType: typesKnockout.selectMultiple, text: Localization.Resources.Operacional.ConfigOperador.TipoTransportador, val: ko.observable(EnumTipoTransportadorCentroCarregamento.Todos), options: EnumTipoTransportadorCentroCarregamento.obterOpcoes(), def: ko.observable([]) })
    this.TipoTransportador = PropertyEntity({ val: ko.observable([]), options: EnumTipoTransportadorCentroCarregamento.obterOpcoesPermissaoLiberarCargaTransportadorExclusivo(), def: [], getType: typesKnockout.selectMultiple, text: "Tipo Transportador", required: false, visible: ko.observable(true), enable: ko.observable(true), title: ko.observable("Não configurado") });
    this.TelaPedidosResumido = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Operacional.ConfigOperador.ExibirTelaPedidoFormaResumida, val: ko.observable(false), def: false });
}


//*******EVENTOS*******

function LoadCentroCarregamento() {

    _centroCarregamento = new CentroCarregamento();
    KoBindings(_centroCarregamento, "knockoutCentroCarregamento");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: Localization.Resources.Operacional.ConfigOperador.Excluir, id: guid(), metodo: function (data) {
                ExcluirCentroCarregamentoClick(data)
            }
        }]
    };

    var header = [{ data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Operacional.ConfigOperador.Descricao, width: "80%" }];

    _gridCentroCarregamento = new BasicDataTable(_centroCarregamento.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarCentrosCarregamento(_centroCarregamento.CentroCarregamento, null, _gridCentroCarregamento);
    _centroCarregamento.CentroCarregamento.basicTable = _gridCentroCarregamento;

    RecarregarGridCentroCarregamento();
}

function RecarregarGridCentroCarregamento() {

    var data = new Array();
    if (!string.IsNullOrWhiteSpace(_operador.CentrosCarregamento.val())) {
        $.each(_operador.CentrosCarregamento.val(), function (i, centroCarregamento) {
            var centroCarregamentoGrid = new Object();

            centroCarregamentoGrid.Codigo = centroCarregamento.Codigo;
            centroCarregamentoGrid.Descricao = centroCarregamento.Descricao;

            data.push(centroCarregamentoGrid);
        });
    }

    _gridCentroCarregamento.CarregarGrid(data);

    _centroCarregamento.PermiteSelecionarHorarioEncaixe.val(_operador.PermiteSelecionarHorarioEncaixe.val());
    _centroCarregamento.TelaPedidosResumido.val(_operador.TelaPedidosResumido.val());
}


function ExcluirCentroCarregamentoClick(data) {
    var centroCarregamentoGrid = _centroCarregamento.CentroCarregamento.basicTable.BuscarRegistros();

    for (var i = 0; i < centroCarregamentoGrid.length; i++) {
        if (data.Codigo == centroCarregamentoGrid[i].Codigo) {
            centroCarregamentoGrid.splice(i, 1);
            break;
        }
    }

    _centroCarregamento.CentroCarregamento.basicTable.CarregarGrid(centroCarregamentoGrid);
}

function LimparCamposCentroCarregamento() {
    LimparCampos(_centroCarregamento);
    _centroCarregamento.CentroCarregamento.basicTable.CarregarGrid(new Array());
}