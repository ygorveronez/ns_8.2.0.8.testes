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
/// <reference path="../../Enumeradores/EnumSituacaoCancelamentoPagamento.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _gridCancelamentoPagamento;
var _cancelamentoPagamento;
var _CRUDCancelamentoPagamento;
var _pesquisaCancelamentoPagamento;


var _situacaoCancelamentoPagamento = [
    { text: "Todas", value: "" },
    { text: "Em Cancelamento", value: EnumSituacaoCancelamentoPagamento.EmCancelamento },
    { text: "Pendência de Cancelamento", value: EnumSituacaoCancelamentoPagamento.PendenciaCancelamento },
    { text: "Ag. Integração", value: EnumSituacaoCancelamentoPagamento.AgIntegracao },
    { text: "Em Integração", value: EnumSituacaoCancelamentoPagamento.EmIntegracao },
    { text: "Falha na Integração", value: EnumSituacaoCancelamentoPagamento.FalhaIntegracao },
    { text: "Cancelado", value: EnumSituacaoCancelamentoPagamento.Cancelado }
];

var CancelamentoPagamento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.Situacao = PropertyEntity({ val: ko.observable(0), def: 0, text: "Situação: " });
    this.SituacaoNoCancelamento = PropertyEntity({ val: ko.observable(0), options: _situacaoCancelamentoPagamento, def: 0, text: "Situação: " });
    this.GerandoMovimentoFinanceiro = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.MotivoRejeicaoFechamentoPagamento = PropertyEntity({});
};

var CRUDCancelamentoPagamento = function () {
    this.Limpar = PropertyEntity({ eventClick: limparPagamentoClick, type: types.event, text: "Limpar (Iniciar Novo Pagamento)", idGrid: guid(), visible: ko.observable(false) });
};

var PesquisaCancelamentoPagamento = function () {

    this.DataInicio = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date, visible: true, val: ko.observable() });
    this.DataFim = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date, visible: true });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;
    this.Numero = PropertyEntity({ text: "Número Pagamento:", maxlength: 12, enable: ko.observable(true), getType: typesKnockout.int });
    this.NumeroDOC = PropertyEntity({ text: "Número Documento:", maxlength: 12, enable: ko.observable(true), getType: typesKnockout.int });

    this.Ocorrencia = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Ocorrência:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("*Transportador:"), idBtnSearch: guid(), visible: ko.observable(true), required: true });

    this.Tomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tomador:", idBtnSearch: guid(), visible: ko.observable(false) });
    this.LocalidadePrestacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Local da Prestação:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoPagamento.Todas), options: _situacaoCancelamentoPagamento, def: EnumSituacaoPagamento.Todas, text: "Situação: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridCancelamentoPagamento.CarregarGrid();
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
};



//*******EVENTOS*******

function loadCancelamentoPagamento() {
    _cancelamentoPagamento = new CancelamentoPagamento();
    HeaderAuditoria("CancelamentoPagamento", _cancelamentoPagamento);

    _CRUDCancelamentoPagamento = new CRUDCancelamentoPagamento();
    KoBindings(_CRUDCancelamentoPagamento, "knockoutCRUD");

    _pesquisaCancelamentoPagamento = new PesquisaCancelamentoPagamento();
    KoBindings(_pesquisaCancelamentoPagamento, "knockoutPesquisaCancelamentoPagamento", false, _pesquisaCancelamentoPagamento.Pesquisar.id);

    LoadFechamentoPagamento();
    loadEtapasPagamento();
    loadSelecaoDocumentos();
    loadIntegracao();
    loadIntegracaoCarga();
    loadResumo();
    LoadSignalRCancelamentoPagamento();
    
    // Inicia as buscas
    new BuscarTransportadores(_pesquisaCancelamentoPagamento.Empresa);
    new BuscarCargas(_pesquisaCancelamentoPagamento.Carga, null, null, null, null, null, null, null, null, true);
    new BuscarOcorrencias(_pesquisaCancelamentoPagamento.Ocorrencia);
    new BuscarClientes(_pesquisaCancelamentoPagamento.Tomador);
    new BuscarLocalidades(_pesquisaCancelamentoPagamento.LocalidadePrestacao);
    new BuscarFilial(_pesquisaCancelamentoPagamento.Filial);

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
        url: "CancelamentoPagamento/ExportarPesquisa",
        titulo: "Cancelamento Pagamento"
    };

    _gridCancelamentoPagamento = new GridView(_pesquisaCancelamentoPagamento.Pesquisar.idGrid, "CancelamentoPagamento/Pesquisa", _pesquisaCancelamentoPagamento, menuOpcoes, null, null, null, null, null, null, null, null, configExportacao);
    _gridCancelamentoPagamento.CarregarGrid();
}

function editarPagamento(itemGrid) {
    // Limpa os campos
    LimparCamposPagamento();

    // Esconde filtros
    _pesquisaCancelamentoPagamento.ExibirFiltros.visibleFade(false);

    // Busca dados
    BuscarCancelamentoPorCodigo(itemGrid.Codigo);
}

function BuscarCancelamentoPorCodigo(codigo, callback) {
    executarReST("CancelamentoPagamento/BuscarPorCodigo", { Codigo: codigo }, function (arg) {
        if (arg.Data != null) {
            // -- Pagamento
            EditarPagamento(arg.Data);

            // -- Selecao de Documentos
            EditarSelecaoDocumentos(arg.Data);

            PreecherResumo(arg.Data);

            CarregaIntegracao();

            CarregaIntegracaoCarga();

            SetarEtapasPagamento();

            if (_cancelamentoPagamento.Situacao.val() === EnumSituacaoCancelamentoPagamento.EmCancelamento || _cancelamentoPagamento.Situacao.val() === EnumSituacaoCancelamentoPagamento.PendenciaCancelamento)
                $("#" + _etapaCancelamentoPagamento.Etapa2.idTab).click();

        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
        if (callback != null)
            callback();
    }, null);
}

function EditarPagamento(data) {
    _cancelamentoPagamento.Codigo.val(data.Codigo);
    _cancelamentoPagamento.Situacao.val(data.Situacao);
    _cancelamentoPagamento.SituacaoNoCancelamento.val(data.SituacaoNoCancelamento);
    _cancelamentoPagamento.GerandoMovimentoFinanceiro.val(data.GerandoMovimentoFinanceiro);
    _cancelamentoPagamento.MotivoRejeicaoFechamentoPagamento.val(data.MotivoRejeicaoFechamentoPagamento);
    _CRUDCancelamentoPagamento.Limpar.visible(true);
}

function LimparCamposPagamento() {
    LimparCampos(_cancelamentoPagamento);
    _CRUDCancelamentoPagamento.Limpar.visible(false);
    _cancelamentoPagamento.Situacao.val(0);
    SetarEtapaInicio();
    LimparCamposSelecaoDocumentos();
    LimparResumo();
    LimparCamposProcessaomentoFechamentoPagamento();
    //$("#liIntegracaoEDI a").click();
}