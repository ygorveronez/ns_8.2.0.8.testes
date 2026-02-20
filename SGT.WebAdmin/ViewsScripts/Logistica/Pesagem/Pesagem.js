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
/// <reference path="../../../ViewsScripts/Consultas/Veiculo.js" />
/// <reference path="../../../ViewsScripts/Enumeradores/EnumStatusBalanca.js" />
/// <reference path="../../GestaoPatio/FluxoPatioPesagem/Pesagem.js" />
/// <reference path="../../GestaoPatio/FluxoPatioPesagem/PesagemFinal.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridPesagem;
var _pesquisaPesagem;

var PesquisaPesagem = function () {
    this.CodigoPesagem = PropertyEntity({ text: "Código Pesagem: " });
    this.NumeroCarga = PropertyEntity({ text: "Número Carga: " });

    this.DataPesagemInicial = PropertyEntity({ text: "Data Pesagem Inicial: ", getType: typesKnockout.date });
    this.DataPesagemFinal = PropertyEntity({ text: "Data Pesagem Final: ", getType: typesKnockout.date });
    this.DataPesagemFinal.dateRangeInit = this.DataPesagemInicial;
    this.DataPesagemInicial.dateRangeLimit = this.DataPesagemFinal;

    this.StatusBalanca = PropertyEntity({ text: "Status Balança: ", val: ko.observable(EnumStatusBalanca.Todos), options: EnumStatusBalanca.obterOpcoesPesquisa(), def: EnumStatusBalanca.Todos });

    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid() });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid() });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridPesagem.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
};

//*******EVENTOS*******

function loadPesagem() {
    _pesquisaPesagem = new PesquisaPesagem();
    KoBindings(_pesquisaPesagem, "knockoutPesquisaPesagem", false, _pesquisaPesagem.Pesquisar.id);

    HeaderAuditoria("Pesagem", {});

    new BuscarTransportadores(_pesquisaPesagem.Transportador, null, null, true);
    new BuscarVeiculos(_pesquisaPesagem.Veiculo);
    new BuscarMotoristas(_pesquisaPesagem.Motorista);

    LoadPesagemFluxoPatio();

    buscarPesagem();
}

//*******MÉTODOS*******

function buscarPesagem() {
    var pesagemInicial = { descricao: "Pesagem Inicial", id: guid(), evento: "onclick", metodo: pesagemInicialClick, tamanho: "10", icone: "" };
    var pesagemFinal = { descricao: "Pesagem Final", id: guid(), evento: "onclick", metodo: pesagemFinalClick, tamanho: "10", icone: "" };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [pesagemInicial, pesagemFinal] };

    _gridPesagem = new GridView(_pesquisaPesagem.Pesquisar.idGrid, "Pesagem/Pesquisa", _pesquisaPesagem, menuOpcoes);
    _gridPesagem.CarregarGrid();
}

function pesagemInicialClick(pesagemGrid) {
    abrirPesagemFluxoPatioClick(pesagemGrid.CodigoGuarita);
}

function pesagemFinalClick(pesagemGrid) {
    abrirPesagemFinalFluxoPatioClick(pesagemGrid.CodigoGuarita);
}