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


var _gridCancelamentoProvisao;
var _cancelamentoProvisao;
var _CRUDCancelamentoProvisao;
var _pesquisaCancelamentoProvisao;

var _situacaoCancelamentoProvisao = [
    { text: "Todas", value: "" },
    { text: "Cancelado", value: EnumSituacaoCancelamentoProvisao.Cancelado },
    { text: "Em Cancelamento", value: EnumSituacaoCancelamentoProvisao.EmCancelamento },
    { text: "Ag. Integração", value: EnumSituacaoCancelamentoProvisao.AgIntegracao },
    { text: "Falha na Integração", value: EnumSituacaoCancelamentoProvisao.FalhaIntegracao },
    { text: "Ag. Aprovação", value: EnumSituacaoCancelamentoProvisao.AgAprovacaoSolicitacao },
    { text: "Pendência no Cancelamento", value: EnumSituacaoCancelamentoProvisao.PendenciaCancelamento },
    { text: "Estornado", value: EnumSituacaoCancelamentoProvisao.Estornado }
];

var CancelamentoProvisao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.Situacao = PropertyEntity({ val: ko.observable(0), def: 0, text: "Situação: " });
    this.SituacaoNoCancelamento = PropertyEntity({ val: ko.observable(0), options: _situacaoCancelamentoProvisao, def: 0, text: "Situação: " });
    this.GerandoMovimentoFinanceiro = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.MotivoRejeicaoCancelamentoFechamentoProvisao = PropertyEntity({});
}

var CRUDCancelamentoProvisao = function () {
    this.Limpar = PropertyEntity({ eventClick: limparCancelamentoProvisaoClick, type: types.event, text: "Limpar(Iniciar Novo Cancelamento de Provisão)", idGrid: guid(), visible: ko.observable(false) });
}

var _situacaoPesquisaCancelarProvisaoContraPartida = [
    { text: "Todos", value: "" },
    { text: "Não", value: false },
    { text: "Sim", value: true }
];

var PesquisaCancelamentoProvisao = function () {

    this.DataInicio = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date, visible: true, val: ko.observable() });
    this.DataFim = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date, visible: true });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;
    this.Numero = PropertyEntity({ text: "Número Provisão:", maxlength: 12, enable: ko.observable(true), getType: typesKnockout.int });
    this.NumeroFolha = PropertyEntity({ text: "Número Folha:", enable: ko.observable(true) });
    this.NumeroDOC = PropertyEntity({ text: "Número NF-e:", maxlength: 12, enable: ko.observable(true), getType: typesKnockout.int });

    this.Ocorrencia = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Ocorrência:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("*Transportador:"), idBtnSearch: guid(), visible: ko.observable(true), required: true });

    this.Tomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tomador:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.LocalidadePrestacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Local da Prestação:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoCancelamentoProvisao.Todas), options: _situacaoCancelamentoProvisao, def: EnumSituacaoCancelamentoProvisao.Todas, text: "Situação: " });
    this.CancelamentoProvisaoContraPartida = PropertyEntity({ val: ko.observable(""), options: _situacaoPesquisaCancelarProvisaoContraPartida, def: "", text: "Provisões de contra partida canceladas: ", visible: _CONFIGURACAO_TMS.DisponbilizarProvisaoContraPartidaParaCancelamento });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridCancelamentoProvisao.CarregarGrid();
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

    this.ReenvioMassivo = PropertyEntity({
        eventClick: ReenvioMassivoCancelamentoProvisaoClick,
        type: types.event,
        text: "Reenvio Massivo",
        visible: ko.observable(true)
    });
}



//*******EVENTOS*******

function loadCancelamentoProvisao() {
    _cancelamentoProvisao = new CancelamentoProvisao();
    HeaderAuditoria("CancelamentoProvisao", _cancelamentoProvisao, null, {
        DocumentoCancelamentoProvisao: "Número Documento"
    });

    _CRUDCancelamentoProvisao = new CRUDCancelamentoProvisao();
    KoBindings(_CRUDCancelamentoProvisao, "knockoutCRUD");

    _pesquisaCancelamentoProvisao = new PesquisaCancelamentoProvisao();
    KoBindings(_pesquisaCancelamentoProvisao, "knockoutPesquisaCancelamentoProvisao", false, _pesquisaCancelamentoProvisao.Pesquisar.id);

    LoadCancelamentoFechamentoProvisao();
    loadEtapasCancelamentoProvisao();
    loadSelecaoDocumentos();
    loadIntegracao();
    loadIntegracaoCancelamentoProvisao();
    loadResumo();
    LoadSignalRCancelamentoProvisao();

    //BuscarHTMLINtegracaoCancelamentoProvisao();
    // Inicia as buscas
    new BuscarTransportadores(_pesquisaCancelamentoProvisao.Empresa);
    new BuscarCargas(_pesquisaCancelamentoProvisao.Carga, null, null, null, null, null, null, null, null, true);
    new BuscarOcorrencias(_pesquisaCancelamentoProvisao.Ocorrencia);
    new BuscarClientes(_pesquisaCancelamentoProvisao.Tomador);
    new BuscarLocalidades(_pesquisaCancelamentoProvisao.LocalidadePrestacao);
    new BuscarFilial(_pesquisaCancelamentoProvisao.Filial);

    BuscarCancelamentoProvisao();
}


function limparCancelamentoProvisaoClick(e, sender) {
    LimparCamposCancelamentoProvisao();
    GridSelecaoDocumentos();
}


//*******MÉTODOS*******
function BuscarCancelamentoProvisao() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarCancelamentoProvisao, tamanho: "15", icone: "" };
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };


    var configExportacao = {
        url: "CancelamentoProvisao/ExportarPesquisa",
        titulo: "Provisão"
    };

    _gridCancelamentoProvisao = new GridView(_pesquisaCancelamentoProvisao.Pesquisar.idGrid, "CancelamentoProvisao/Pesquisa", _pesquisaCancelamentoProvisao, menuOpcoes, null, null, null, null, null, null, null, null, configExportacao);
    _gridCancelamentoProvisao.CarregarGrid();
}

function editarCancelamentoProvisao(itemGrid) {
    // Limpa os campos
    LimparCamposCancelamentoProvisao();

    // Esconde filtros
    _pesquisaCancelamentoProvisao.ExibirFiltros.visibleFade(false);

    // Busca dados
    BuscarCancelamentoProvisaoPorCodigo(itemGrid.Codigo);
}

function BuscarCancelamentoProvisaoPorCodigo(codigo, callback) {
    executarReST("CancelamentoProvisao/BuscarPorCodigo", { Codigo: codigo }, function (arg) {
        if (arg.Data != null) {
            // -- CancelamentoProvisao
            EditarCancelamentoProvisao(arg.Data);

            // -- Selecao de Documentos
            EditarSelecaoDocumentos(arg.Data);

            PreecherResumo(arg.Data);

            CarregaIntegracao();

            CarregaIntegracaoCarga();

            SetarEtapasCancelamentoProvisao();

            if (_cancelamentoProvisao.Situacao.val() == EnumSituacaoCancelamentoProvisao.SolicitacaoReprovada) {
              
                _aprovacaoCancelamento.ReenviarAprovacao.visible(true);
                _aprovacaoCancelamento.CancelarProcessamento.visible(true);
            }
               
           
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
        if (callback != null)
            callback();
    }, null);
}

function EditarCancelamentoProvisao(data) {
    _cancelamentoProvisao.Codigo.val(data.Codigo);
    _cancelamentoProvisao.Situacao.val(data.Situacao);
    _cancelamentoProvisao.SituacaoNoCancelamento.val(data.SituacaoNoCancelamento);
    _cancelamentoProvisao.GerandoMovimentoFinanceiro.val(data.GerandoMovimentoFinanceiro);
    _cancelamentoProvisao.MotivoRejeicaoCancelamentoFechamentoProvisao.val(data.MotivoRejeicaoCancelamentoFechamentoProvisao);

    _aprovacaoCancelamento.Autorizacao.visible(data.UtilizaAutorizacao);
    _aprovacaoCancelamento.UsuariosAutorizadores.visible(data.Situacao != EnumSituacaoCancelamentoProvisao.SemRegraAprovacao);
    _aprovacaoCancelamento.SemRegraAprovacao.visible(data.Situacao == EnumSituacaoCancelamentoProvisao.SemRegraAprovacao);
    _aprovacaoCancelamento.ReprocessarRegras.visible(data.Situacao == EnumSituacaoCancelamentoProvisao.SemRegraAprovacao);

    _gridAutorizacoes.CarregarGrid();
    _CRUDCancelamentoProvisao.Limpar.visible(true);
}

function LimparCamposCancelamentoProvisao() {
    LimparCampos(_cancelamentoProvisao);
    _CRUDCancelamentoProvisao.Limpar.visible(false);
    _cancelamentoProvisao.Situacao.val(EnumSituacaoCancelamentoProvisao.Todas);
    SetarEtapaInicio();
    LimparCamposSelecaoDocumentos();
    LimparResumo();
    LimparCamposProcessaomentoCancelamentoFechamentoProvisao();
    //$("#liIntegracaoEDI a").click();
}

function ReenvioMassivoCancelamentoProvisaoClick() {
    exibirConfirmacao("Atenção!", "Deseja realmente reenviar todas as integrações com falha?", function () {
        executarReST("CancelamentoProvisao/ReenviarTodos", {}, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso!", "Integrações reenviadas com sucesso.");
                    _gridCancelamentoProvisao.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
            }
        });
    });
}