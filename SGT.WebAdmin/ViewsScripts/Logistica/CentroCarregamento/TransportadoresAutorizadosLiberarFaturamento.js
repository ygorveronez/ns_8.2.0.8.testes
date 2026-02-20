/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />>
/// <reference path="CentroCarregamento.js" />
/// <reference path="../../Consultas/Tranportador.js" />

var _gridTransportadoresAutorizadosLiberarFaturamento;
var _transportadoresAutorizadosLiberarFaturamento;


var TransportadoresAutorizadosLiberarFaturamento = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Transportador = PropertyEntity({ type: types.event, text: Localization.Resources.Logistica.CentroCarregamento.AdicionarTransportador, idBtnSearch: guid(), visible: ko.observable(true) });
    
    this.TempoEmMinutosLiberacao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, text: Localization.Resources.Logistica.CentroCarregamento.TempoParaLiberacaoAposInformarVeiculoMotoristaMinutos.getFieldDescription() });

    this.TempoEmMinutosLiberacao.val.subscribe(function (novoValor) {
        _centroCarregamento.TempoEmMinutosLiberacao.val(novoValor);
    });
}


//*******EVENTOS*******

function LoadTransportadoresAutorizadosLiberarFaturamento() {

    _transportadoresAutorizadosLiberarFaturamento = new TransportadoresAutorizadosLiberarFaturamento();
    KoBindings(_transportadoresAutorizadosLiberarFaturamento, "knockoutTransportadoresAutorizadosLiberarFaturamento");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: function (data) {
                ExcluirTransportadoresAutorizadosLiberarFaturamentoClick(_transportadoresAutorizadosLiberarFaturamento.Transportador, data)
            }
        }]
    };

    var header = [{ data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Logistica.CentroCarregamento.RazaoSocial, width: "80%" }];

    _gridTransportadoresAutorizadosLiberarFaturamento = new BasicDataTable(_transportadoresAutorizadosLiberarFaturamento.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc }, null, 5);

    new BuscarTransportadores(_transportadoresAutorizadosLiberarFaturamento.Transportador, null, null, true, _gridTransportadoresAutorizadosLiberarFaturamento);

    _transportadoresAutorizadosLiberarFaturamento.Transportador.basicTable = _gridTransportadoresAutorizadosLiberarFaturamento;

    RecarregarGridTransportadoresAutorizadosLiberarFaturamento();
}

function RecarregarGridTransportadoresAutorizadosLiberarFaturamento() {
    _gridTransportadoresAutorizadosLiberarFaturamento.CarregarGrid(_centroCarregamento.TransportadoresAutorizadosLiberarFaturamento.val());
}

function ExcluirTransportadoresAutorizadosLiberarFaturamentoClick(knoutTransportadoresAutorizadosLiberarFaturamento, data) {
    var transportadoresAutorizadosLiberarFaturamentoesGrid = knoutTransportadoresAutorizadosLiberarFaturamento.basicTable.BuscarRegistros();

    for (var i = 0; i < transportadoresAutorizadosLiberarFaturamentoesGrid.length; i++) {
        if (data.Codigo == transportadoresAutorizadosLiberarFaturamentoesGrid[i].Codigo) {
            transportadoresAutorizadosLiberarFaturamentoesGrid.splice(i, 1);
            break;
        }
    }

    knoutTransportadoresAutorizadosLiberarFaturamento.basicTable.CarregarGrid(transportadoresAutorizadosLiberarFaturamentoesGrid);
}

function LimparCamposTransportadoresAutorizadosLiberarFaturamento() {
    LimparCampos(_transportadoresAutorizadosLiberarFaturamento);
    _gridTransportadoresAutorizadosLiberarFaturamento.CarregarGrid(new Array());
}