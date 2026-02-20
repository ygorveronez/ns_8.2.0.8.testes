/// <reference path="EtapaCancelamentoCTeSemCarga.js"/>
/// <reference path="../../Enumeradores/EnumSituacaoCancelamentoCTeSemCarga.js" />
//*******MAPEAMENTO KNOUCKOUT*******

var _statusCTe = [
    { text: "Todos", value: EnumSituacaoCancelamentoCTeSemCarga.Todos },
    { text: "Ag. Cancelamento CTe", value: EnumSituacaoCancelamentoCTeSemCarga.AgCancelamentoCTe },
    { text: "Ag. Cancelamento Integração", value: EnumStatusCTe.AgCancelamentoIntegracao },
    { text: "Cancelado", value: EnumStatusCTe.Cancelado },
    { text: "Rejeição Cancelamento", value: EnumStatusCTe.RejeicaoCancelamento }
];

var _gridConsultaCTe;
var _pesquisaCTe;
var _cancelamentoCTe;
var _modalCancelamentoCTe;
var _gridCTeSemCarga;
var _pesquisaCTeSemCarga;
var _gridCTeEmCancelamento;
var _gridResumoCancelamentoSemCarga;
var _resumoCancelamentoSemCarga

var ResumoCancelamentoSemCarga = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.NumeroCancelamento = PropertyEntity({ text: "Número Cancelamento", visible: ko.observable(false) });
    this.DataCancelamento = PropertyEntity({ text: "Data Cancelamento" });
    this.Situacao = PropertyEntity({ text: "Situação"});
    this.MensagemRejeicaoCancelamento = PropertyEntity({ text:"Motivo Rejeição", visible: ko.observable(false) });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridResumoCancelamentoSemCarga.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid()
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var CancelamentoCTe = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.ListaCTes = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.Situacao = PropertyEntity({ text: "Situação:", val: ko.observable(""), options: _statusCTe, def: EnumSituacaoCancelamentoCTeSemCarga.Todos, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({
        eventClick: function (e) {
            CancelarClick();
        }, type: types.event, text: "Cancelar", idGrid: guid(), icon: "fa fa-chevron-down"
    });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridConsultaCTe.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid()
    });
}

var PesquisaCTeSemCarga = function () {
    this.CodigoCancelamento = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.SelecionarTodos = PropertyEntity({ val: ko.observable(false), def: false, type: types.map, getType: typesKnockout.bool, text: "Marcar/Desmarcar Todos", visible: ko.observable(false), enable: ko.observable(true) });
    this.ListaCTes = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.Pesquisar = PropertyEntity({
        eventClick: function () {
            _pesquisaCTeSemCarga.ExibirFiltros.visibleFade(false);
            limparSelecionados();
            _pesquisaCTeSemCarga.CarregarGrid();

        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
    
    this.Adicionar = PropertyEntity({ eventClick: AdicionarCancelamentoClick, type: types.event, text: ko.observable("Gerar Cancelamento"), visible: ko.observable(true) });
    this.GerarNovoCancelamentoSemCarga = PropertyEntity({ eventClick: GerarNovoCancelamentoSemCargaClick, type: types.event, text: ko.observable("Gerar Novo Cancelamento"), visible: ko.observable(false) });
};

function carregarListaCTesSemCarga() {
    var ctesSelecionados = _gridCTeSemCarga.ObterMultiplosSelecionados();

    if (ctesSelecionados.length > 0) {
        
        var dataGrid = new Array();

        $.each(ctesSelecionados, function (i, cte) {
            
            var obj = new Object();
            obj.CodigoCTe = cte.Codigo;
            dataGrid.push(obj);
        });

        _cancelamentoCTe.ListaCTes.val(JSON.stringify(dataGrid));
    }
}

function loadGridCTeSemCarga() {

    var quantidadePorPagina = 100;

    var multiplaEscolha = {
        basicGrid: null,
        callbackSelecionado: null,
        callbackNaoSelecionado: null,
        eventos: function () { },
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _pesquisaCTeSemCarga.SelecionarTodos,
        somenteLeitura: false
    };

    _pesquisaCTeSemCarga.SelecionarTodos.visible(true);
    _gridCTeSemCarga = new GridView(_pesquisaCTeSemCarga.Pesquisar.idGrid, "CancelamentoCTeSemCarga/PesquisaCTeSemCarga", _pesquisaCTeSemCarga, null, null, quantidadePorPagina, null, null, null, multiplaEscolha, quantidadePorPagina, null);
    _gridCTeSemCarga.SetPermitirRedimencionarColunas(true);

    _gridCTeSemCarga.CarregarGrid();
}

var PesquisaCTe = function () {
    this.CodigoCancelamento = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.NumeroInicial = PropertyEntity({ text: "Número Inicial:", getType: typesKnockout.int });
    this.NumeroFinal = PropertyEntity({ text: "Número Final:", getType: typesKnockout.int });
    this.DataEmissaoInicial = PropertyEntity({ text: "Data de Emissão Inicial:", getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual() });
    this.DataEmissaoFinal = PropertyEntity({ text: "Data de Emissão Final:", getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual() });
    this.Status = PropertyEntity({ val: ko.observable(EnumSituacaoCancelamentoCTeSemCarga.Todos), options: EnumSituacaoCancelamentoCTeSemCarga.ObterOpcoes(), def: EnumSituacaoCancelamentoCTeSemCarga.Todos, text: "Situação : " });
    this.DataEmissaoFinal.dateRangeInit = this.DataEmissaoInicial;
    this.DataEmissaoInicial.dateRangeLimit = this.DataEmissaoFinal;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridConsultaCTe.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid()
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: Localization.Resources.Gerais.Geral.FiltrosPesquisa, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var CRUDCancelamentoCTeSemCarga = function () {
    this.Adicionar = PropertyEntity({ eventClick: AdicionarCancelamentoClick, type: types.event, text: ko.observable("Gerar Cancelamento"), visible: ko.observable(true) });
    this.GerarNovoCancelamentoSemCarga = PropertyEntity({ eventClick: GerarNovoCancelamentoSemCargaClick, type: types.event, text: ko.observable("Gerar Novo Cancelamento"), visible: ko.observable(false) });
};

//*******EVENTOS*******

function LoadCTeCancelamento() {
    _pesquisaCTe = new PesquisaCTe();
    KoBindings(_pesquisaCTe, "knockoutCTeCancelamento", false, _pesquisaCTe.Pesquisar.id);

    _pesquisaCTeSemCarga = new PesquisaCTeSemCarga();
    KoBindings(_pesquisaCTeSemCarga, "knockoutCTeSemCargaCancelamento", false, _pesquisaCTeSemCarga.Pesquisar.id);

    _CRUDCancelamentoCTeSemCarga = new CRUDCancelamentoCTeSemCarga();
    KoBindings(_CRUDCancelamentoCTeSemCarga, "knockoutCRUDCancelamentoCTeSemCarga");

    _cancelamentoCTe = new CancelamentoCTe();
    KoBindings(_cancelamentoCTe, "divModalCancelamentoCTe");

    loadGridCTeSemCarga();
    LoadEtapaCancelamentoCTeSemCarga();
    LoadCTeSemCarga();
    LoadResumoCancelamentoSemCarga();
    BuscarCTes();
    _modalCancelamentoCTe = new bootstrap.Modal(document.getElementById("divModalCancelamentoCTe"), { backdrop: true, keyboard: true });
}

function AdicionarCancelamentoClick() {
    carregarListaCTesSemCarga();

    Salvar(_cancelamentoCTe, "CancelamentoCTeSemCarga/Cancelar", function (r) {
        if (r.Success) {
            if (r.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", r.Data.Mensagem);
                LoadCTeCancelamento();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

//*******MÉTODOS*******

function BuscarCTes() {
    //var renviar = { descricao: "Renviar Cancelamento", id: guid(), evento: "onclick", metodo: "", tamanho: "20", icone: "", visibilidade: VisualizarOpcaoCancelamento };
    var editar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: EditarCancelamentoCTeSemCarga, tamanho: "20", icone: "", visibilidade: VisualizarOpcaoCancelamento }
    var menuOpcoes = {
        tipo: TypeOptionMenu.list,
        opcoes: [editar],
        tamanho: 7
    };

    _gridConsultaCTe = new GridView(_pesquisaCTe.Pesquisar.idGrid, "CancelamentoCTeSemCarga/Pesquisa", _pesquisaCTe, menuOpcoes, { column: 4, dir: orderDir.desc }, 10);
    _gridConsultaCTe.CarregarGrid();
}

function BuscarCTesCancelamento() {
    _pesquisaCTeSemCarga.SelecionarTodos.visible(false);
    _pesquisaCTeSemCarga.CodigoCancelamento = _cancelamentoCTe.Codigo;
    _pesquisaCTeSemCarga.ExibirFiltros.visible(true);
    _resumoCancelamentoSemCarga.ExibirFiltros.visible(false);
    _gridCTeSemCarga = new GridView(_pesquisaCTeSemCarga.Pesquisar.idGrid, "CancelamentoCTeSemCarga/BuscarCTePorCodigoCancelamento", _pesquisaCTeSemCarga, null, { column: 4, dir: orderDir.desc }, 10);
    _gridCTeSemCarga.CarregarGrid();
}

function VisualizarOpcaoCancelamento(dados) {
    return true;
}

function EditarCancelamentoCTeSemCarga(cancelamentoCTeGrid) {
    LimparResumoCancelamento();

    _cancelamentoCTe.Codigo.val(cancelamentoCTeGrid.Codigo);

    BuscarCancelamentoCTeSemCargaPorCodigo();
}

function BuscarCancelamentoCTeSemCargaPorCodigo(exibirLoading) {

    if (exibirLoading == null)
        exibirLoading = true;

    if (!exibirLoading)
        _ControlarManualmenteProgresse = true;
    _pesquisaCTeSemCarga.ExibirFiltros.visible(false);
    BuscarPorCodigo(_cancelamentoCTe, "CancelamentoCTeSemCarga/BuscarPorCodigo", function (arg) {
        _ControlarManualmenteProgresse = false;

        _cancelamentoCTe.Codigo.val(arg.Data.Codigo);
        _cancelamentoCTe.Situacao.val(arg.Data.Status);

        PreecherResumoCancelamentoSemCarga(arg.Data);
        PreecherCamposEdicaoCancelamento();
    }, null, exibirLoading);

}

function limparSelecionados() {
    _gridCTeSemCarga.AtualizarRegistrosSelecionados([]);
    _gridCTeSemCarga.AtualizarRegistrosNaoSelecionados([]);
    _pesquisaCTeSemCarga.SelecionarTodos.val(false);
}

var CTe = function () {
    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.CancelamentoCarga = PropertyEntity({ val: ko.observable(0), def: 0 });

    this.BuscarCTes = PropertyEntity({ eventClick: BuscarCancelamentoCTeSemCargaPorCodigo, type: types.event, text: "Buscar / Atualizar CT-e(s)", visible: ko.observable(true), idGrid: guid() });
};

function LoadCTeSemCarga() {
    _cte = new CTe();
    KoBindings(_cte, "knockoutCTe");
}

function LoadResumoCancelamentoSemCarga() {
    _resumoCancelamentoSemCarga = new ResumoCancelamentoSemCarga();
    KoBindings(_resumoCancelamentoSemCarga, "knockoutResumoCancelamentoSemCarga");
}

//*******MÉTODOS*******

function PreecherResumoCancelamentoSemCarga(dados) {
    _resumoCancelamentoSemCarga.NumeroCancelamento.visible(true);
    _resumoCancelamentoSemCarga.NumeroCancelamento.val(dados.Codigo);
    _resumoCancelamentoSemCarga.Situacao.val(dados.Situacao);
    _resumoCancelamentoSemCarga.DataCancelamento.val(dados.DataCancelamento);
    _resumoCancelamentoSemCarga.MensagemRejeicaoCancelamento.val(dados.MensagemRejeicaoCancelamento);
    if (dados.Situacao == EnumSituacaoCancelamentoCTeSemCarga.RejeicaoCancelamento) {
        _resumoCancelamentoSemCarga.MensagemRejeicaoCancelamento.visible(true);
    } else {
        _resumoCancelamentoSemCarga.MensagemRejeicaoCancelamento.visible(false);
    }
}

function PreecherCamposEdicaoCancelamento() {
    _pesquisaCTe.ExibirFiltros.visibleFade(false);
    _pesquisaCTeSemCarga.SelecionarTodos.visible(false);
    _pesquisaCTeSemCarga.GerarNovoCancelamentoSemCarga.visible(true);
    _CRUDCancelamentoCTeSemCarga.Adicionar.visible(false);

    SetarEtapaCancelamentoCTeSemCarga();
};


function LimparResumoCancelamento() {
    LimparCampos(_resumoCancelamentoSemCarga);
}

function GerarNovoCancelamentoSemCargaClick(e) {
    LimparCamposCancelamentoSemCarga();
}

function LimparCamposCancelamentoSemCarga() {
    _pesquisaCTe.ExibirFiltros.visibleFade(true);
    _pesquisaCTeSemCarga.SelecionarTodos.visible(true);
    _CRUDCancelamento.GerarNovoCancelamento.visible(false);
    _CRUDCancelamento.Adicionar.visible(true);
    _CRUDCancelamento.Adicionar.text(Localization.Resources.Cargas.CancelamentoCarga.GerarCancelamento);

    LimparResumoCancelamento()
    LimparCampos();
    SetarEtapaInicioCancelamentoSemCarga();
}