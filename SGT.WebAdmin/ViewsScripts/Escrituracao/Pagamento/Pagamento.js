/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _gridPagamento;
var _pagamento;
var _CRUDPagamento;
var _pesquisaPagamento;

var _pagamentoLiberado = [
    { text: "Sim", value: true },
    { text: "Não", value: false }
];

var Pagamento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.Situacao = PropertyEntity({ val: ko.observable(0), def: 0, text: "Situação: " });
    this.SituacaoNoCancelamento = PropertyEntity({ val: ko.observable(EnumSituacaoPagamento.Todas), options: EnumSituacaoPagamento.obterOpcoesPesquisa(), def: EnumSituacaoPagamento.Todas, text: "Situação: " });
    this.GerandoMovimentoFinanceiro = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.CargaEmCancelamento = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.MotivoRejeicaoFechamentoPagamento = PropertyEntity({});
};

var CRUDPagamento = function () {
    this.Limpar = PropertyEntity({ eventClick: limparPagamentoClick, type: types.event, text: "Limpar (Iniciar Novo Pagamento)", idGrid: guid(), visible: ko.observable(false) });
};

var PesquisaPagamento = function () {

    this.DataInicio = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date, visible: true, val: ko.observable() });
    this.DataFim = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date, visible: true });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;

    this.DataInicialEmissao = PropertyEntity({ text: "Data Inicial Emissão: ", getType: typesKnockout.date, visible: true, val: ko.observable() });
    this.DataFinalEmissao = PropertyEntity({ text: "Data Final Emissão: ", getType: typesKnockout.date, visible: true });
    this.DataInicialEmissao.dateRangeLimit = this.DataFinalEmissao;
    this.DataFinalEmissao.dateRangeInit = this.DataInicialEmissao;

    this.Numero = PropertyEntity({ text: "Número Pagamento:", maxlength: 12, enable: ko.observable(true), getType: typesKnockout.int });
    this.NumeroDOC = PropertyEntity({ text: "Número Documento:", maxlength: 12, enable: ko.observable(true), getType: typesKnockout.int });

    this.Ocorrencia = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Ocorrência:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("*Transportador:"), idBtnSearch: guid(), visible: ko.observable(true), required: true });

    this.Tomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tomador:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.LocalidadePrestacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Local da Prestação:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoPagamento.Todas), options: EnumSituacaoPagamento.obterOpcoesPesquisa(), def: EnumSituacaoPagamento.Todas, text: "Situação: " });
    this.PagamentoLiberado = PropertyEntity({ val: ko.observable(false), options: _pagamentoLiberado, def: false, text: "Pagamento Liberado: ", visible: ko.observable(_CONFIGURACAO_TMS.GerarPagamentoBloqueado && !_CONFIGURACAO_TMS.GerarSomenteDocumentosDesbloqueados) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridPagamento.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (e.BuscaAvancada.visibleFade() == true) {
                e.BuscaAvancada.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                e.BuscaAvancada.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.ReenvioMassivo = PropertyEntity({ eventClick: ReenvioMassivoClick, type: types.event, text: "Reenvio Massivo", visible: ko.observable(true), issue: 67326 });
};

//*******EVENTOS*******

function loadPagamento() {
    _pagamento = new Pagamento();
    HeaderAuditoria("Pagamento", _pagamento, null, {
        DocumentoPagamento: "Número Documento"
    });

    _CRUDPagamento = new CRUDPagamento();
    KoBindings(_CRUDPagamento, "knockoutCRUD");

    _pesquisaPagamento = new PesquisaPagamento();
    KoBindings(_pesquisaPagamento, "knockoutPesquisaPagamento", false, _pesquisaPagamento.Pesquisar.id);

    LoadFechamentoPagamento();
    loadEtapasPagamento();
    loadSelecaoDocumentos();
    loadIntegracao();
    loadIntegracaoCarga();
    loadResumo();
    LoadSignalRPagamento();
    LoadBloqueioDocumento();

    //BuscarHTMLINtegracaoPagamento();
    // Inicia as buscas
    new BuscarTransportadores(_pesquisaPagamento.Empresa);
    new BuscarCargas(_pesquisaPagamento.Carga, null, null, null, null, null, null, null, null, true);
    new BuscarOcorrencias(_pesquisaPagamento.Ocorrencia);
    new BuscarClientes(_pesquisaPagamento.Tomador);
    new BuscarLocalidades(_pesquisaPagamento.LocalidadePrestacao);
    new BuscarFilial(_pesquisaPagamento.Filial);

    BuscarPagamento();
}

function limparPagamentoClick(e, sender) {
    LimparCamposPagamento();
    GridSelecaoDocumentos();
}

//*******MÉTODOS*******
function BuscarPagamento() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarPagamento, tamanho: "15", icone: "" };
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };


    var configExportacao = {
        url: "Pagamento/ExportarPesquisa",
        titulo: "Pagamento"
    };

    _gridPagamento = new GridView(_pesquisaPagamento.Pesquisar.idGrid, "Pagamento/Pesquisa", _pesquisaPagamento, menuOpcoes, null, null, null, null, null, null, null, null, configExportacao);
    _gridPagamento.CarregarGrid();
}

function editarPagamento(itemGrid) {
    _pesquisaPagamento.ExibirFiltros.visibleFade(false);

    BuscarPagamentoPorCodigo(itemGrid.Codigo);
}

function BuscarPagamentoPorCodigo(codigo, callback) {
    LimparCamposPagamento();

    executarReST("Pagamento/BuscarPorCodigo", { Codigo: codigo }, function (arg) {
        if (arg.Data != null) {
            EditarPagamento(arg.Data);
            EditarSelecaoDocumentos(arg.Data);
            PreecherResumo(arg.Data);
            CarregaIntegracao();
            CarregaIntegracaoCarga();
            SetarEtapasPagamento();
            preencherPagamentoAutorizacao(codigo);

            if (
                _pagamento.Situacao.val() === EnumSituacaoPagamento.EmFechamento ||
                _pagamento.Situacao.val() === EnumSituacaoPagamento.PendenciaFechamento ||
                _pagamento.Situacao.val() === EnumSituacaoPagamento.AguardandoAprovacao ||
                _pagamento.Situacao.val() === EnumSituacaoPagamento.Reprovado ||
                _pagamento.Situacao.val() === EnumSituacaoPagamento.SemRegraAprovacao
            )
                Global.ExibirStep("tabFechamentoPagamento");
            //$("#tabFechamentoPagamento").click();
            //$("#tabFechamentoPagamento").tab("show");
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);

        if (callback != null)
            callback();
    }, null);
}

function EditarPagamento(data) {
    _pagamento.Codigo.val(data.Codigo);
    _pagamento.Situacao.val(data.Situacao);
    _pagamento.SituacaoNoCancelamento.val(data.SituacaoNoCancelamento);
    _pagamento.GerandoMovimentoFinanceiro.val(data.GerandoMovimentoFinanceiro);
    _pagamento.MotivoRejeicaoFechamentoPagamento.val(data.MotivoRejeicaoFechamentoPagamento);
    _pagamento.CargaEmCancelamento.val(data.CargaEmCancelamento);
    _CRUDPagamento.Limpar.visible(true);
}

function LimparCamposPagamento() {
    LimparCampos(_pagamento);
    _CRUDPagamento.Limpar.visible(false);
    _pagamento.Situacao.val(EnumSituacaoPagamento.Todas);
    SetarEtapaInicio();
    LimparCamposSelecaoDocumentos();
    LimparResumo();
    LimparCamposProcessaomentoFechamentoPagamento();
    limparPagamentoAutorizacao();
    //Global.ExibirStep("tabFechamentoPagamento");

    //$("#knockoutSelecaoDocumentos").click();
    //$("#knockoutSelecaoDocumentos").tab("show");
}

function ReenvioMassivoClick() {
    exibirConfirmacao("Atenção!", "Deseja realmente reenviar todas as integrações com falha?", function () {
        executarReST("Pagamento/ReenviarTodos", {}, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso!", "Integrações reenviadas com sucesso.");
                    _gridPagamento.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
            }
        });
    });
}