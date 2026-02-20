/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../Consultas/Carga.js" />
/// <reference path="../../Consultas/ComponenteFrete.js" />
/// <reference path="../../Consultas/CTe.js" />
/// <reference path="../../Consultas/ModeloDocumentoFiscal.js" />
/// <reference path="../../Consultas/TipoOcorrencia.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Consultas/Chamado.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Consultas/OcorrenciaIntegracaoEmbarcador.js" />
/// <reference path="../../Consultas/TiposCausadoresOcorrencia.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Estado.js" />
/// <reference path="../../Consultas/CausasTipoOcorrencia.js" />
/// <reference path="../../Creditos/ControleSaldo/ControleSaldo.js" />
/// <reference path="../../Enumeradores/EnumDezena.js" />
/// <reference path="../../Enumeradores/EnumFinalidadeTipoOcorrencia.js" />
/// <reference path="../../Enumeradores/EnumMes.js" />
/// <reference path="../../Enumeradores/EnumOrigemOcorrencia.js" />
/// <reference path="../../Enumeradores/EnumPeriodoAcordoContratoFreteTransportador.js" />
/// <reference path="../../Enumeradores/EnumQuinzena.js" />
/// <reference path="../../Enumeradores/EnumSituacaoOcorrencia.js" />
/// <reference path="../../Enumeradores/EnumTipoDocumentoCreditoDebito.js" />
/// <reference path="../../Enumeradores/EnumTipoEmissaoDocumentoOcorrencia.js" />
/// <reference path="../../Enumeradores/EnumTipoParametroOcorrencia.js" />
/// <reference path="../../Enumeradores/EnumTipoTomador.js" />
/// <reference path="../../Enumeradores/EnumTipoPessoaGrupo.js" />
/// <reference path="../../Enumeradores/EnumCodigoControleImportacao.js" />
/// <reference path="../../Enumeradores/EnumTipoObservacaoCTe.js" />
/// <reference path="../../Enumeradores/EnumICMSCTe.js" />
/// <reference path="../../Global/Notificacoes/Notificacao.js" />
/// <reference path="Anexos.js" />
/// <reference path="AnexosVeiculoContrato.js" />
/// <reference path="Autorizacao.js" />
/// <reference path="CTeComplementar.js" />
/// <reference path="Contrato.js" />
/// <reference path="ControleGridCargasComplementadas.js" />
/// <reference path="EtapasOcorrencia.js" />
/// <reference path="OcorrenciaImagem.js" />
/// <reference path="OcorrenciaPosicionamento.js" />
/// <reference path="ResumoOcorrencia.js" />
/// <reference path="TipoOcorrencia.js" />
/// <reference path="ObservacaoFiscoContribuinte.js" />
/// <reference path="NFeProduto.js"

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _cteEmitidoEmbarcador = false;
var _gridViewOcorrencia;
var _ocorrencia;
var _CRUDOcorrencia;
var _pesquisaOcorrencia;
var _gridCTeComplementado;
var _gridDocumentosParaEmissaoNFSManualComplementado;
var _gridCargasComplemento;
var _gridVeiculosImprodutivos;
var _CTesImportadosParaComplemento = new Array();
var _gridCTe;
var _gridDocumentosParaEmissaoNFSManual;
var _semTabela;
var _tipoSelecaoOcorrenciaPorPeriodo;
var _TokenAcesso;
var _selecionarProdutos = false;
var _InduzirTransportadorSelecionarApenasUmComplementoSolicitacaoComplementos = false;
let isProcessing = false;

/*
 * Declaração das Classes
 */

var PesquisaOcorrencia = function () {
    var visivelEmbarcador = _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador || _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe;
    var visivelTMS = _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS;

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DataInicio = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.DataInicio.getFieldDescription(), getType: typesKnockout.date, cssClass: ko.observable("col col-xs-6 col-lg-3") });
    this.DataFim = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.DataLimite.getFieldDescription(), getType: typesKnockout.date, cssClass: ko.observable("col col-xs-6 col-lg-3") });
    this.Carga = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.NumeroCarga.getFieldDescription(), val: ko.observable(""), def: "" });
    this.PesquisaNaOcorrencia = PropertyEntity({ val: ko.observable(true), def: true });
    this.NumeroOcorrencia = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.NumeroOcorrencia.getFieldDescription(), val: ko.observable(""), def: "" });
    this.NumeroAtendimento = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.NumeroAtendimento.getFieldDescription(), val: ko.observable(""), def: "" });
    this.NumeroNFe = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.NumeroNFe.getFieldDescription(), val: ko.observable(""), def: "" });
    this.NumeroOcorrenciaCliente = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.NumeroOcorrenciaCliente.getFieldDescription(), val: ko.observable(""), def: "", visible: visivelTMS });
    this.Pedido = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.NumeroPedido.getFieldDescription(), val: ko.observable(""), def: "" });
    this.CTeOrigem = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.CTeOrigem.getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.int });
    this.CTeComplementar = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.CTeComplementar.getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.int, cssClass: ko.observable("col col-xs-6 col-lg-3") });
    this.TipoDocumentoCreditoDebito = PropertyEntity({ val: ko.observable(EnumTipoDocumentoCreditoDebito.Todos), options: EnumTipoDocumentoCreditoDebito.obterOpcoesPesquisa(), def: EnumTipoDocumentoCreditoDebito.Todos, text: Localization.Resources.Ocorrencias.Ocorrencia.TipoCreditoDebito.getFieldDescription(), visible: ko.observable(true) });
    this.SituacaoOcorrencia = PropertyEntity({ val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.selectMultiple, options: EnumSituacaoOcorrencia.obterOpcoes(), text: Localization.Resources.Ocorrencias.Ocorrencia.Situacao.getFieldDescription(), issue: 411 });
    this.Ocorrencia = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Ocorrencias.Ocorrencia.TipoOcorrencia.getFieldDescription(), idBtnSearch: guid() });
    this.Filial = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Ocorrencias.Ocorrencia.Filial.getFieldDescription(), idBtnSearch: guid(), visible: visivelEmbarcador });
    this.TipoPessoa = PropertyEntity({ val: ko.observable(EnumTipoPessoaGrupo.Pessoa), options: EnumTipoPessoaGrupo.obterOpcoesPesquisa(), def: "", text: Localization.Resources.Ocorrencias.Ocorrencia.TipoPessoa.getFieldDescription(), visible: true });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Ocorrencias.Ocorrencia.Destinatario.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.GrupoPessoas = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Ocorrencias.Ocorrencia.GrupoPessoas.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false) });
    this.Empresa = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Ocorrencias.Ocorrencia.Transportador.getFieldDescription(), idBtnSearch: guid(), visible: visivelEmbarcador });
    this.Tomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Ocorrencias.Ocorrencia.TomadorCTeComplementar.getFieldDescription(), idBtnSearch: guid(), visible: true });
    this.ObservacaoCTe = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.ObservacaoCTe.getFieldDescription(), val: ko.observable(""), def: "" });
    this.NumeroControle = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.NumeroControle.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(false), getType: typesKnockout.string });
    this.NumeroBooking = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.Booking.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(false), getType: typesKnockout.string });
    this.NumeroOS = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.NumeroOrdemServico.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(false), getType: typesKnockout.string });
    this.DataInicialAprovacao = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.DataInicioAprovacao.getFieldDescription(), getType: typesKnockout.date });
    this.DataLimiteAprovacao = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.DataLimiteAprovacao.getFieldDescription(), dateRangeInit: this.DataInicialAprovacao, getType: typesKnockout.date });
    this.DataInicialEmissaoDocumento = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.DataInicioEmissaoDocumento.getFieldDescription(), getType: typesKnockout.date });
    this.DataLimiteEmissaoDocumento = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.DataLimiteEmissaoDcumento.getFieldDescription(), dateRangeInit: this.DataInicialEmissaoDocumento, getType: typesKnockout.date });
    this.TipoOperacao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Ocorrencias.Ocorrencia.TipoOperacao.getFieldDescription(), idBtnSearch: guid() });
    this.GrupoOcorrencia = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Ocorrencias.Ocorrencia.GrupoOcorrencia.getFieldDescription(), idBtnSearch: guid() });
    this.NumeroDocumentoOriginario = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.NumeroDocumentoOrigemCTe.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(visivelTMS), getType: typesKnockout.int });
    this.CentroResultado = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Ocorrencias.Ocorrencia.CentroResultado.getFieldDescription(), idBtnSearch: guid(), visible: visivelTMS });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Ocorrencias.Ocorrencia.Motorista.getFieldDescription(), idBtnSearch: guid() });
    this.TomadorCTeOriginal = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Ocorrencias.Ocorrencia.TomadorCTeOriginal.getFieldDescription(), idBtnSearch: guid(), visible: true });
    this.AguardandoImportacaoCTe = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.AguardandoImportacaoCTe.getFieldDescription(), options: EnumSimNaoPesquisa.obterOpcoesPesquisa(), val: ko.observable(EnumSimNaoPesquisa.Todos), def: EnumSimNaoPesquisa.Todos, visible: ko.observable(true) });
    this.TiposCausadoresOcorrencia = PropertyEntity({ type: types.entity, val: ko.observable(""), codEntity: ko.observable(0), text: Localization.Resources.Ocorrencias.Ocorrencia.CausadorOcorrencia.getFieldDescription(), required: false, idBtnSearch: guid(), enable: ko.observable(true) });
    this.CausasTipoOcorrencia = PropertyEntity({ type: types.entity, val: ko.observable(""), codEntity: ko.observable(0), text: Localization.Resources.Ocorrencias.Ocorrencia.CausasTipoOcorrencia.getFieldDescription(), required: false, idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });

    this.GrupoPessoasTomadorCteComplementar = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Ocorrencias.Ocorrencia.GrupoTomadorCTeComplementar.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.GrupoPessoasTomadorCteOriginal = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Ocorrencias.Ocorrencia.GrupoTomadorCTeOriginal.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });

    this.NumeroPedidoCliente = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.NumeroPedidoCliente.getFieldDescription(), maxlength: 150, required: false });
    this.ClienteComplementar = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Ocorrencias.Ocorrencia.ClienteComplementar.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Vendedor = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Vendedor.getFieldDescription(), idBtnSearch: guid() });
    this.Supervisor = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Supervisor.getFieldDescription(), idBtnSearch: guid() });
    this.Gerente = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Gerente.getFieldDescription(), idBtnSearch: guid() });
    this.UFDestino = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.UFDestino.getFieldDescription(), idBtnSearch: guid() });
    this.NumeroNF = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.NumeroNF.getFieldDescription(), getType: typesKnockout.int, visible: ko.observable(true) });

    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;

    this.DataInicialAprovacao.dateRangeLimit = this.DataLimiteAprovacao;
    this.DataLimiteAprovacao.dateRangeInit = this.DataInicialAprovacao;

    this.DataInicialEmissaoDocumento.dateRangeLimit = this.DataLimiteEmissaoDocumento;
    this.DataLimiteEmissaoDocumento.dateRangeInit = this.DataInicialEmissaoDocumento;

    this.OcorrenciaIntegracaoEmbarcador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Ocorrencias.Ocorrencia.OcorrenciasEmbarcador.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false) });

    this.BuscarOcorrenciaIntegracaoEmbarcador = PropertyEntity({ eventClick: AbrirConsultaOcorrenciaIntegracaoEmbarcadorClick, type: types.event, text: Localization.Resources.Ocorrencias.Ocorrencia.OcorrenciasImportadasDoEmbarcador, visible: ko.observable(_CONFIGURACAO_TMS.ImportarCargasMultiEmbarcador) });

    this.DownloadAnexosLote = PropertyEntity({ eventClick: DownloadAnexosLoteClick, type: types.event, text: Localization.Resources.Ocorrencias.Ocorrencia.DownloadAnexosEmLote, visible: ko.observable(_CONFIGURACAO_TMS.PermiteBaixarAnexoOcorrenciaEmLote) });
    this.DownloadPDFsOcorrencia = PropertyEntity({ eventClick: DownloadPDFsOcorrenciaClick, type: types.event, text: Localization.Resources.Ocorrencias.Ocorrencia.DownloadPdfsOcorrencia, visible: ko.observable(false) });

    this.TipoPessoa.val.subscribe(function (novoValor) {
        if (novoValor == EnumTipoPessoaGrupo.Pessoa) {
            _pesquisaOcorrencia.Pessoa.visible(true);
            _pesquisaOcorrencia.GrupoPessoas.visible(false);
            _pesquisaOcorrencia.GrupoPessoas.codEntity(0);
            _pesquisaOcorrencia.GrupoPessoas.val('');
        }
        else {
            _pesquisaOcorrencia.GrupoPessoas.visible(true);
            _pesquisaOcorrencia.Pessoa.visible(false);
            _pesquisaOcorrencia.Pessoa.codEntity(0);
            _pesquisaOcorrencia.Pessoa.val('');
        }
    });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _pesquisaOcorrencia.SelecionarTodos.val(false);
            _gridViewOcorrencia.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Ocorrencias.Ocorrencia.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });

    this.ReemitirCTesRejeitados = PropertyEntity({
        eventClick: reemitirTodosCTesRejeitadosClick, type: types.event, text: Localization.Resources.Ocorrencias.Ocorrencia.ReemitirTodosCtEsRejeitados, idGrid: guid(), visible: ko.observable(true)
    });

    this.ImportarNOTFIS = PropertyEntity({
        eventClick: AbrirTelaImportacaoNOTFIS, type: types.event, text: Localization.Resources.Ocorrencias.Ocorrencia.ImportarNOTFIS, idGrid: guid(), visible: ko.observable(_CONFIGURACAO_TMS.HabilitarImportacaoOcorrenciaViaNOTFIS)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
        }, type: types.event, text: Localization.Resources.Ocorrencias.Ocorrencia.FiltrosDePesquisa, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.LimparFiltros = PropertyEntity({
        eventClick: function (e) {
            limparFiltrosOcorrencia();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.LimparFiltros, idGrid: guid(), visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (e.BuscaAvancada.visibleFade()) {
                e.BuscaAvancada.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                e.BuscaAvancada.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: Localization.Resources.Ocorrencias.Ocorrencia.Avancada, idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.Upload = PropertyEntity({ type: types.event, eventChange: UploadChange, idFile: guid(), accept: ".txt", text: Localization.Resources.Ocorrencias.Ocorrencia.EnviarOcoren, icon: "fal fa-file", visible: ko.observable(true) });
    this.SelecionarTodos = PropertyEntity({ val: ko.observable(false), def: false, type: types.map, getType: typesKnockout.bool, text: Localization.Resources.Ocorrencias.Ocorrencia.MarcarDesmarcarTodos, visible: ko.observable(false) });
    this.ListaOcorrenciasPesquisa = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
};

var Ocorrencia = function () {
    var self = this;

    var ccsClassValor = "col col-xs-12 col-sm-12 col-md-6 col-lg-6";
    var anoAtual = (new Date()).getFullYear();
    var enableComponenteFrete = true;
    var enableDataOcorrencia = true;
    var mesAtual = (new Date()).getMonth() + 1;
    var statusPermitido = JSON.stringify(["A", "Z"]);
    var visivelObsCTe = true;
    var visivelOutrosDoc = false;
    var visivelTipoTomador = true;
    var visibleChamados = false;

    visivelTipoTomador = (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS || _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador)
        visivelOutrosDoc = true;
    else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.Terceiros || _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiCTe) {
        ccsClassValor = "col col-xs-12 col-sm-12 col-md-6";
        visivelOutrosDoc = false;
    }

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador || _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiCTe)
        visibleChamados = _CONFIGURACAO_TMS.PermiteInformarChamadosNoLancamentoOcorrencia;

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.Terceiros) {
        visivelObsCTe = false;
        enableComponenteFrete = false;
        enableDataOcorrencia = false;
    }
    else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiCTe) {
        enableComponenteFrete = false;
        enableDataOcorrencia = false;
    }

    this.TipoInclusaoImpostoComplemento = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.OcorrenciaReprovada = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.Ocorrencias.Ocorrencia.Carga.getFieldDescription(), idBtnSearch: guid(), eventChange: cargaBlur, enable: ko.observable(true), visible: ko.observable(true), issue: 195 });
    this.Chamado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: ko.observable(false), text: Localization.Resources.Ocorrencias.Ocorrencia.Atendimento.getFieldDescription(), idBtnSearch: guid(), eventChange: cargaBlur, enable: ko.observable(true), visible: ko.observable(false) });
    this.Chamados = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), required: false, text: ko.observable(Localization.Resources.Ocorrencias.Ocorrencia.Atendimentos.getFieldDescription()), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(visibleChamados), required: ko.observable(false) });
    this.QuantidadeAjudantes = PropertyEntity({ text: "Quantidade Ajudantes", enable: ko.observable(true), visible: ko.observable(false), required: ko.observable(false), eventChange: calcularValorTabelaFrete });
    this.Conhecimento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.Ocorrencias.Ocorrencia.Conhecimento.getFieldDescription(), idBtnSearch: guid(), eventChange: cargaBlur, enable: ko.observable(false), visible: ko.observable(false) });
    this.CTeTerceiro = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.Ocorrencias.Ocorrencia.CtEDeTerceiro.getFieldDescription(), idBtnSearch: guid(), eventChange: cargaBlur, enable: ko.observable(true), visible: ko.observable(_CONFIGURACAO_TMS.GerarOcorrenciaComplementoSubcontratacao) });
    //Preenche apenas pela adição manual de ocorrência via Lotes de Avaria
    this.LoteAvaria = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.Ocorrencias.Ocorrencia.LoteAvaria.getFieldDescription(), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(false) });
    this.Infracao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.Ocorrencias.Ocorrencia.OcorrenciaInfracao.getFieldDescription(), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(false) });

    this.Carga.codEntity.subscribe(function () {
        loadValorCentroResultado();
    });

    this.ComponenteFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), eventChange: componenteFreteBlur, required: false, enable: ko.observable(enableComponenteFrete), text: ko.observable(Localization.Resources.Ocorrencias.Ocorrencia.ComponenteDeFrete.getFieldDescription()), idBtnSearch: guid(), issue: 85 });
    this.ComponenteFrete.codEntity.subscribe(function () {
        visibilidadeValorComponenteExistente();
        visibilidadeUtilizarSelecaoPorNotasFiscaisCTe();
    });
    this.TipoOcorrencia = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), enable: ko.observable(true), required: true, text: Localization.Resources.Ocorrencias.Ocorrencia.TipoDaOcorrencia.getRequiredFieldDescription(), idBtnSearch: guid(), issue: 410, tipoEmissaoDocumentoOcorrencia: EnumTipoEmissaoDocumentoOcorrencia.Todos, cleanEntityCallback: tipoOcorrenciaApagado });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), visible: ko.observable(false), enable: ko.observable(true), required: false, text: Localization.Resources.Ocorrencias.Ocorrencia.Veiculo.getRequiredFieldDescription(), idBtnSearch: guid() });
    this.UsuarioResponsavelAprovacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), visible: ko.observable(false), enable: ko.observable(true), required: true, text: Localization.Resources.Ocorrencias.Ocorrencia.Responsavel.getRequiredFieldDescription(), idBtnSearch: guid() });
    this.CentroResultado = PropertyEntity({ val: ko.observable(""), visible: ko.observable(_CONFIGURACAO_TMS.TrazerCentroResultadoOcorrencia), text: Localization.Resources.Ocorrencias.Ocorrencia.CentroResultado.getFieldDescription(), def: "", enable: false });
    this.GrupoOcorrencia = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), visible: ko.observable(false), required: false, enable: ko.observable(true), text: ko.observable(Localization.Resources.Ocorrencias.Ocorrencia.GrupoOcorrencia.getRequiredFieldDescription()), idBtnSearch: guid() });

    /* Por padrão é um campo desativado.
     * Contrato de frete apenas é exibido quando o tipo da ocorrencia é franquia
     * O mesmo é preenchido de acordo com a empresa selecionada e o período
     */
    this.ContratoFreteTransportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), periodoAcordo: EnumPeriodoAcordoContratoFreteTransportador.NaoPossui, visible: ko.observable(false), enable: ko.observable(false), required: false, text: Localization.Resources.Ocorrencias.Ocorrencia.ContratoFreteTransportador.getFieldDescription(), idBtnSearch: guid() });
    this.DataOcorrencia = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.DataDaOcorrencia.getRequiredFieldDescription(), getType: typesKnockout.dateTime, val: ko.observable(Global.DataHoraAtual()), def: Global.DataHoraAtual(), required: true, enable: ko.observable(enableDataOcorrencia), defEnable: enableDataOcorrencia });
    this.DataEvento = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.DataDoEvento.getFieldDescription(), getType: typesKnockout.dateTime, val: ko.observable(""), def: "", enable: ko.observable(true), visible: ko.observable(false) });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Ocorrencias.Ocorrencia.Transportador.getRequiredFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false), enable: ko.observable(true) });
    this.Empresa.codEntity.subscribe(periodoCargaBlur);
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Ocorrencias.Ocorrencia.Filial.getRequiredFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false), enable: ko.observable(true) });
    this.Filial.codEntity.subscribe(periodoCargaBlur);

    this.Periodo = PropertyEntity({ val: ko.observable(EnumPeriodoAcordoContratoFreteTransportador.NaoPossui), def: EnumPeriodoAcordoContratoFreteTransportador.NaoPossui, getType: typesKnockout.int });
    this.PeriodoDezena = PropertyEntity({ val: ko.observable(EnumDezena.Primeira), options: EnumDezena.obterOpcoes(), def: EnumDezena.Primeira, text: Localization.Resources.Ocorrencias.Ocorrencia.Dezena.getFieldDescription(), visible: ko.observable(false), enable: ko.observable(true), eventChange: periodoCargaBlur });
    this.PeriodoQuinzena = PropertyEntity({ val: ko.observable(EnumQuinzena.Primeira), options: EnumQuinzena.obterOpcoes(), def: EnumQuinzena.Primeira, text: Localization.Resources.Ocorrencias.Ocorrencia.Quinzena.getFieldDescription(), visible: ko.observable(false), enable: ko.observable(true), eventChange: periodoCargaBlur });
    this.PeriodoMes = PropertyEntity({ val: ko.observable(mesAtual), options: EnumMes.obterOpcoes(), def: mesAtual, text: Localization.Resources.Ocorrencias.Ocorrencia.Mes.getFieldDescription(), enable: ko.observable(true), cssClass: ko.observable("col col-xs-12 col-md-2"), eventChange: periodoCargaBlur });
    this.PeriodoAno = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.Ano.getFieldDescription(), getType: typesKnockout.int, val: ko.observable(anoAtual), def: anoAtual, maxlength: 4, enable: ko.observable(true), cssClass: ko.observable("col col-xs-12 col-md-2"), configInt: { precision: 0, allowZero: false, thousands: '' }, eventChange: periodoCargaBlur });
    this.PeriodoInicio = PropertyEntity({ val: controlarValorPeriodoInicio });
    this.PeriodoFim = PropertyEntity({ val: obterPeriodoFim });

    this.CargasComplementadasDias = PropertyEntity({ val: GetControleGridCargasComplementadas });
    this.OcorrenciaDeEstadia = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.CTeFilialEmissora = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.PossuiNFSManual = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.NFSManualPendenteGeracao = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.Status = PropertyEntity({ val: ko.observable(statusPermitido), def: statusPermitido });
    this.EmiteComplementoFilialEmissora = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.IntegrandoFilialEmissora = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.ReenviouIntegracaoFilialEmissora = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.AgImportacaoCTe = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.EmiteNFSeFora = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.EmitirDocumentoParaFilialEmissoraComPreCTe = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.TipoEmissaoDocumentoOcorrencia = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.int });
    this.ErroIntegracaoComGPA = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.MensagemPendencia = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.NotificarDebitosAtivos = PropertyEntity({ val: ko.observable(false), visible: ko.observable(false), enable: ko.observable(true), def: false, text: Localization.Resources.Ocorrencias.Ocorrencia.InformarOTransportadorPorEMailQueODebitoFoiCriadoParaEle, getType: typesKnockout.bool });
    this.UtilizarSelecaoPorNotasFiscaisCTe = PropertyEntity({ val: ko.observable(false), visible: ko.observable(false), enable: ko.observable(true), def: false, text: "Utilizar seleção por notas fiscais do CT-e", getType: typesKnockout.bool });
    this.UtilizarSelecaoPorNotasFiscaisCTe.val.subscribe(function (novoValor) {
        if (_gridCTe != undefined && _ocorrencia.Carga.codEntity() > 0) {
            _gridCTe.CarregarGrid();
        }
    });
    this.ValorOcorrenciaOriginal = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.ValorOriginalmenteSolicitado.getRequiredFieldDescription(), getType: typesKnockout.decimal, required: false, visible: ko.observable(false) });
    this.NumeroOcorrencia = PropertyEntity({ val: ko.observable(""), visible: ko.observable(true), text: Localization.Resources.Ocorrencias.Ocorrencia.Numero.getFieldDescription(), def: "", enable: false });
    this.NumeroOcorrenciaCliente = PropertyEntity({ val: ko.observable(""), visible: ko.observable(false), text: Localization.Resources.Ocorrencias.Ocorrencia.NumeroDaOcorrenciaNoCliente.getFieldDescription(), def: "", enable: ko.observable(true) });
    this.Observacao = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.Observacao.getFieldDescription(), maxlength: 500, required: false, enable: ko.observable(true) });
    this.Motivo = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.Motivo.getRequiredFieldDescription(), maxlength: 500, required: ko.observable(false), enable: ko.observable(true), visible: ko.observable(false) });
    this.SituacaoOcorrencia = PropertyEntity({ val: ko.observable(EnumSituacaoOcorrencia.Todas), def: EnumSituacaoOcorrencia.Todas, getType: typesKnockout.int });
    this.SituacaoOcorrenciaNoCancelamento = PropertyEntity({ val: ko.observable(EnumSituacaoOcorrencia.Todas), def: EnumSituacaoOcorrencia.Todas, getType: typesKnockout.int });
    this.MotivoCancelamento = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.CargaCTes = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.XMLNotasFiscaisParaCTeGlobalizado = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.CargaDocumentosParaEmissaoNFSManual = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.ListaOcorrenciaNFeProduto = PropertyEntity({ val: ko.observable(new Array()) });
    this.CargaCTesImportados = PropertyEntity({ getType: typesKnockout.dynamic, val: ko.observable(new Array()), required: false, def: new Array(), codEntity: ko.observable(0), idGrid: guid() });
    this.SolicitacaoCredito = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.dynamic });
    this.DescricaoSituacao = PropertyEntity({});
    this.Cliente = PropertyEntity({});
    this.DefinirPeriodoEstadiaAutomaticamente = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });


    this.NumeroNF = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.NNfE.getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.int, visible: ko.observable(true) });
    this.NumeroDocumento = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.NDocumento.getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.int, visible: ko.observable(true) });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Ocorrencias.Ocorrencia.Destinatario.getFieldDescription(), enable: ko.observable(true), required: false, idBtnSearch: guid(), visible: ko.observable(true) });

    this.PesquisarCTes = PropertyEntity({
        eventClick: function (e) {
            _gridCTe.CarregarGrid();
            _gridDocumentosParaEmissaoNFSManual.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Ocorrencias.Ocorrencia.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });

    this.CreditosUtilizados = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(new Array()), def: new Array() });
    this.CodigoCreditorSolicitar = PropertyEntity({ val: ko.observable(0), def: 0, visible: false, getType: typesKnockout.int });
    this.CobrarOutroDocumento = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, enable: ko.observable(true), visible: visivelOutrosDoc });
    this.ModeloDocumentoFiscal = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Ocorrencias.Ocorrencia.CobrarValorEmOutroDocumentoFiscal, enable: ko.observable(false), required: false, idBtnSearch: guid(), visible: ko.observable(visivelOutrosDoc), issue: 370 });
    this.DadosModeloDocumentoFiscal = PropertyEntity({ getType: typesKnockout.dynamic, val: ko.observable(null), def: null });

    this.CTesComplementados = PropertyEntity({ idGrid: guid(), visible: ko.observable(false) });
    this.DocumentosParaEmissaoNFSManualComplementados = PropertyEntity({ idGrid: guid(), visible: ko.observable(false) });
    this.CTesParaComplemento = PropertyEntity({ idGrid: guid(), visible: ko.observable(true), text: ko.observable(Localization.Resources.Ocorrencias.Ocorrencia.SelecionarCTesParaComplemento), visibleFade: ko.observable(false), cssClass: ko.observable("col col-xs-12 col-sm-12") });
    this.CargaDocumentoParaEmissaoNFSManualParaComplemento = PropertyEntity({ idGrid: guid(), visible: ko.observable(true), text: ko.observable(Localization.Resources.Ocorrencias.Ocorrencia.SelecionarDocsParaNfsManualParaComplemento), visibleFade: ko.observable(false), cssClass: ko.observable("col col-xs-12 col-sm-12") });
    this.CargasParaComplemento = PropertyEntity({ idGrid: guid(), visible: ko.observable(true), text: Localization.Resources.Ocorrencias.Ocorrencia.CargasComplementadas, visibleFade: ko.observable(false), cssClass: ko.observable("col col-xs-12 col-sm-12") });
    this.VeiculosImprodutivos = PropertyEntity({ idGrid: guid(), visible: ko.observable(true), text: Localization.Resources.Ocorrencias.Ocorrencia.VeiculosImprodutivos, visibleFade: ko.observable(false), cssClass: ko.observable("col col-xs-12 col-sm-8") });
    this.DocumentosAgrupadosDoVeiculo = PropertyEntity({ idGrid: guid(), visible: ko.observable(true), text: Localization.Resources.Ocorrencias.Ocorrencia.DocumentosComplementados, visibleFade: ko.observable(false), cssClass: ko.observable("col col-xs-12 col-sm-12") });
    this.VeiculosContrato = PropertyEntity({ idGrid: guid(), visible: ko.observable(true), text: Localization.Resources.Ocorrencias.Ocorrencia.Veiculos, visibleFade: ko.observable(false), cssClass: ko.observable("col col-xs-12 col-sm-12"), val: function () { if (arguments.length == 0) return GetGridVeiculosContrato(); } });

    this.DescontarValoresOutrasCargas = PropertyEntity({ visible: ko.observable(false), enable: ko.observable(false), val: ko.observable(""), def: "" });
    this.MotoristasContrato = PropertyEntity({ visible: ko.observable(true), visibleFade: ko.observable(false) });
    this.QuantidadeMotorista = PropertyEntity({ val: ko.observable(0), text: Localization.Resources.Ocorrencias.Ocorrencia.Quantidade.getFieldDescription(), def: 0, enable: ko.observable(true), getType: typesKnockout.int, eventChange: boxMotoristaContratoChange });
    this.ValorDiarioMotorista = PropertyEntity({ val: ko.observable("0,00"), text: Localization.Resources.Ocorrencias.Ocorrencia.ValorDiario.getFieldDescription(), def: "0,00", enable: ko.observable(true) });
    this.ValorQuinzenalMotorista = PropertyEntity({ val: ko.observable("0,00"), text: Localization.Resources.Ocorrencias.Ocorrencia.ValorQuinzenal.getFieldDescription(), def: "0,00", enable: ko.observable(true) });
    this.QuantidadeDiasMotorista = PropertyEntity({ val: ko.observable(0), text: Localization.Resources.Ocorrencias.Ocorrencia.QuantidadeDias.getFieldDescription(), def: 0, enable: ko.observable(true), getType: typesKnockout.int, eventChange: boxMotoristaContratoChange });
    this.TotalMotorista = PropertyEntity({ val: ko.observable("0,00"), text: Localization.Resources.Ocorrencias.Ocorrencia.Total.getFieldDescription(), def: "0,00", enable: ko.observable(true) });

    this.IncluirICMSFrete = PropertyEntity({ val: ko.observable(true), def: true, getType: typesKnockout.bool, text: Localization.Resources.Ocorrencias.Ocorrencia.IncluirIcmsNoFrete, enable: ko.observable(true), visible: ko.observable(false) });
    this.Quantidade = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.Quantidade.getRequiredFieldDescription(), getType: typesKnockout.int, enable: ko.observable(true), visible: ko.observable(true), cssClass: ko.observable(ccsClassValor), eventChange: changeQuantidade });
    this.ValorOcorrencia = PropertyEntity({ text: ko.observable(Localization.Resources.Ocorrencias.Ocorrencia.ValorOcorrencia.getRequiredFieldDescription()), getType: typesKnockout.decimal, required: true, visibleFade: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true), cssClass: ko.observable(ccsClassValor), eventChange: changeValorOcorrencia });
    this.TipoTomador = PropertyEntity({ val: ko.observable(99), options: EnumTipoTomaodor.obterOpcoes(), visible: ko.observable(visivelOutrosDoc || visivelTipoTomador), def: 99, text: Localization.Resources.Ocorrencias.Ocorrencia.Tomador.getFieldDescription(), enable: ko.observable(true), eventChange: tipoTomadorChange, issue: 413 });
    this.Tomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Ocorrencias.Ocorrencia.Tomador.getFieldDescription(), enable: ko.observable(false), required: false, idBtnSearch: guid(), visible: ko.observable(false) });
    this.PercentualAcrescimoValor = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.PercentualDeAcrescimoDoValor.getFieldDescription(), getType: typesKnockout.decimal, required: false, enable: ko.observable(false), cssClass: ko.observable(ccsClassValor) });
    this.ObservacaoOcorrencia = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.ObservacaoOcorrencia.getFieldDescription(), maxlength: 500, enable: ko.observable(false), visible: ko.observable(false) });
    this.DividirOcorrencia = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, enable: ko.observable(true), visible: ko.observable(false) });
    this.ValorOcorrenciaDestino = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.ValorDaOcorrenciaDestino.getFieldDescription(), getType: typesKnockout.decimal, required: false, visible: ko.observable(false), enable: ko.observable(false), cssClass: ko.observable(ccsClassValor) });
    this.ObservacaoOcorrenciaDestino = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.ObservacaoOcorrenciaDestino.getFieldDescription(), maxlength: 500, enable: ko.observable(false), visible: ko.observable(false) });
    this.ObservacaoCTeDestino = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.ObservacaoNoCtEDestino.getFieldDescription(), maxlength: 500, enable: ko.observable(false), visible: ko.observable(false) });
    this.HorasOcorrencia = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.HorasDaOcorrencia.getFieldDescription(), getType: typesKnockout.decimal, required: false, visible: ko.observable(false), enable: ko.observable(false), cssClass: ko.observable(ccsClassValor) });
    this.HorasOcorrenciaDestino = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.HorasDaOcorrenciaDestino.getFieldDescription(), getType: typesKnockout.decimal, required: false, visible: ko.observable(false), enable: ko.observable(false), cssClass: ko.observable(ccsClassValor) });
    this.CargaOcorrenciaVinculada = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.ValorComponenteExistenteCTes = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.ValorDoComponenteJaExistenteNosCTesSelecionados.getFieldDescription(), val: ko.observable("0,00"), def: "0,00", visible: ko.observable(false) })

    this.BaseCalculoICMS = PropertyEntity({ text: "B.C. ICMS: ", getType: typesKnockout.decimal, required: false, visible: ko.observable(false), enable: ko.observable(true), maxlength: 10 });
    this.AliquotaICMS = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.AliquotaIcms.getFieldDescription(), getType: typesKnockout.decimal, required: false, visible: ko.observable(false), enable: ko.observable(true), maxlength: 5 });
    this.ValorICMS = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.ValorIcms.getFieldDescription(), getType: typesKnockout.decimal, required: false, visible: ko.observable(false), enable: ko.observable(true), maxlength: 10 });
    this.CSTICMS = PropertyEntity({ text: "CST ICMS:", val: ko.observable(null), options: EnumcstICMSCTe.obterOpcoesPesquisa(), def: null, enable: ko.observable(true), visible: ko.observable(false) });

    this.ObservacaoCTe = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.ObservacaoImpressa.getFieldDescription(), maxlength: 500, enable: ko.observable(true), visible: ko.observable(visivelObsCTe), enable: ko.observable(true) });
    this.SelecionarTodos = PropertyEntity({ val: ko.observable(false), def: false, type: types.map, getType: typesKnockout.bool, text: Localization.Resources.Ocorrencias.Ocorrencia.MarcarDesmarcarTodos, visible: ko.observable(true) });
    this.SelecionarTodosDocumentoParaEmissaoNFSManual = PropertyEntity({ val: ko.observable(false), def: false, type: types.map, getType: typesKnockout.bool, text: Localization.Resources.Ocorrencias.Ocorrencia.MarcarDesmarcarTodos, visible: ko.observable(true) });

    this.ParametrosOcorrencia = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.ParametrosDaOcorrencia.getFieldDescription(), getType: typesKnockout.decimal, enable: ko.observable(true), required: false, visibleFade: ko.observable(false), enable: ko.observable(true) });

    this.CodigoParametroInteiro = PropertyEntity({ val: ko.observable(0), def: 0, visible: false, getType: typesKnockout.int });
    this.ParametroInteiro = PropertyEntity({ text: ko.observable(Localization.Resources.Ocorrencias.Ocorrencia.Texto.getFieldDescription()), maxlength: 5, required: false, visible: ko.observable(false), visibleFade: ko.observable(false), enable: ko.observable(true), getType: typesKnockout.int });

    this.CodigoParametroBooleano = PropertyEntity({ val: ko.observable(0), def: 0, visible: false, getType: typesKnockout.int });
    this.ApenasReboque = PropertyEntity({ text: ko.observable(Localization.Resources.Ocorrencias.Ocorrencia.ApenasReboque.getFieldDescription()), getType: typesKnockout.bool, required: false, visible: ko.observable(false), enable: ko.observable(true), eventChange: calcularValorTabelaFrete });

    this.CodigoParametroPeriodo = PropertyEntity({ val: ko.observable(0), def: 0, visible: false, getType: typesKnockout.int });
    this.DataEntradaRaio = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.EntradaNoRaio.getFieldDescription() });
    this.DataSaidaRaio = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.SaidaDoRaio.getFieldDescription() });
    this.DataInicio = PropertyEntity({ text: ko.observable(""), defText: Localization.Resources.Ocorrencias.Ocorrencia.DataInicio.getRequiredFieldDescription(), getType: typesKnockout.dateTime, required: ko.observable(false), visible: ko.observable(false), enable: ko.observable(true), eventChange: calcularValorTabelaFrete });
    this.DataFim = PropertyEntity({ text: ko.observable(""), defText: Localization.Resources.Ocorrencias.Ocorrencia.DataFim.getRequiredFieldDescription(), getType: typesKnockout.dateTime, required: ko.observable(false), visible: ko.observable(false), enable: ko.observable(true), eventChange: calcularValorTabelaFrete });
    this.TotalHoras = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.TotalHoras.getFieldDescription(), visible: ko.observable(false), enable: ko.observable(false) });
    this.FreeTime = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.FreeTime.getFieldDescription(), visible: ko.observable(false), enable: ko.observable(false) });
    this.TotalHorasCalculo = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.TotalHorasCalculo.getFieldDescription(), visible: ko.observable(false), enable: ko.observable(false) });
    this.DistanciaCTes = PropertyEntity({ visible: ko.observable(false), enable: ko.observable(false) });

    this.CodigoParametroTexto = PropertyEntity({ val: ko.observable(0), def: 0, visible: false, getType: typesKnockout.int });
    this.ParametroTexto = PropertyEntity({ text: ko.observable(Localization.Resources.Ocorrencias.Ocorrencia.Texto), maxlength: 100, required: false, visible: ko.observable(false), enable: ko.observable(true), getType: typesKnockout.int });

    this.CodigoParametroData1 = PropertyEntity({ val: ko.observable(0), def: 0, visible: false, getType: typesKnockout.int });
    this.ParametroData1 = PropertyEntity({ text: ko.observable(Localization.Resources.Ocorrencias.Ocorrencia.DataUm.getFieldDescription()), getType: typesKnockout.dateTime, required: false, visible: ko.observable(false), enable: ko.observable(true), eventChange: calcularValorTabelaFrete });

    this.CodigoParametroData2 = PropertyEntity({ val: ko.observable(0), def: 0, visible: false, getType: typesKnockout.int });
    this.ParametroData2 = PropertyEntity({ text: ko.observable(Localization.Resources.Ocorrencias.Ocorrencia.DataDois.getFieldDescription()), getType: typesKnockout.dateTime, required: false, visible: ko.observable(false), enable: ko.observable(true) });

    this.CTeEmitidoNoEmbarcador = PropertyEntity({ getType: typesKnockout.bool, required: false, val: ko.observable(false), def: false, visible: ko.observable(false), text: Localization.Resources.Ocorrencias.Ocorrencia.OsCtEsDestaOcorrenciaForamEmitidosNoEmbarcador, enable: ko.observable(true) });

    this.Anexo = PropertyEntity({ eventClick: gerenciarAnexosClick, type: types.event, text: Localization.Resources.Ocorrencias.Ocorrencia.Anexos, visible: ko.observable(true), enable: ko.observable(true) });
    this.PossuiAnexos = PropertyEntity({ type: types.map, val: OcorrenciaPossuiAnexos });

    this.CalcularValorOcorrencia = PropertyEntity({ eventClick: calcularValorOcorrenciaClick, type: types.event, text: Localization.Resources.Ocorrencias.Ocorrencia.RecalcularValor, visible: ko.observable(false) });

    this.CalcularFrete = PropertyEntity({ eventClick: calcularValorOcorrenciaClick, type: types.event, text: Localization.Resources.Ocorrencias.Ocorrencia.RecalcularValor, visible: ko.observable(true) });
    this.CTeSemCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, enable: ko.observable(false), visible: ko.observable(false), idBtnSearch: guid() });

    this.NaoLimparCarga = PropertyEntity({ val: ko.observable(false), def: false, visible: false, getType: typesKnockout.bool });

    this.DTNatura = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: Localization.Resources.Ocorrencias.Ocorrencia.DtDaNatura.getFieldDescription(), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(false), eventChange: DTNaturaOnBlur });

    this.ListaImagens = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.Latitude = PropertyEntity({ text: ko.observable(" "), required: false, visible: ko.observable(false), maxlength: 20 });
    this.Longitude = PropertyEntity({ text: ko.observable(" "), required: false, visible: ko.observable(false), maxlength: 20 });

    this.NomeRecebedor = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.NomeDoRecebedor.getFieldDescription(), issue: 1566, visible: ko.observable(false), enable: ko.observable(true) });
    this.TipoDocumentoRecebedor = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.TipoDeDocumentoDoRecebedor.getFieldDescription(), issue: 1566, visible: ko.observable(false), enable: ko.observable(true) });
    this.NumeroDocumentoRecebedor = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.NumeroDoDocumentoDoRecebedor.getFieldDescription(), issue: 1566, visible: ko.observable(false), enable: ko.observable(true) });

    this.Moeda = PropertyEntity({ enable: ko.observable(false), text: Localization.Resources.Ocorrencias.Ocorrencia.Moeda.getFieldDescription(), visible: ko.observable(false), options: EnumMoedaCotacaoBancoCentral.obterOpcoes(), val: ko.observable(EnumMoedaCotacaoBancoCentral.Real), def: EnumMoedaCotacaoBancoCentral.Real });
    this.ValorTotalMoeda = PropertyEntity({ enable: ko.observable(true), visible: ko.observable(false), text: Localization.Resources.Ocorrencias.Ocorrencia.ValorEmMoeda.getFieldDescription(), def: "0,00", val: ko.observable("0,00"), getType: typesKnockout.decimal, maxlength: 15, configDecimal: { precision: 2, allowZero: true, allowNegative: false } });
    this.ValorCotacaoMoeda = PropertyEntity({ enable: ko.observable(false), visible: ko.observable(false), text: Localization.Resources.Ocorrencias.Ocorrencia.CotacaoDaMoeda.getFieldDescription(), def: "1,0000000000", val: ko.observable("1,0000000000"), getType: typesKnockout.decimal, maxlength: 22, configDecimal: { precision: 10, allowZero: false, allowNegative: false } });
    this.ValorTotalMoeda.val.subscribe(function () { ConverterValorMoedaOcorrencia(self); });

    this.AlterarMoeda = PropertyEntity({ eventClick: AbrirTelaAlteracaoMoedaOcorrencia, type: types.event, text: Localization.Resources.Ocorrencias.Ocorrencia.AlterarAMoeda.getFieldDescription(), visible: ko.observable(false), enable: ko.observable(false) });

    this.TiposCausadoresOcorrencia = PropertyEntity({ type: types.entity, val: ko.observable(""), codEntity: ko.observable(0), text: Localization.Resources.Ocorrencias.Ocorrencia.CausadorOcorrencia.getFieldDescription(), required: false, idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(false) });
    this.CausasTipoOcorrencia = PropertyEntity({ type: types.entity, val: ko.observable(""), codEntity: ko.observable(0), text: Localization.Resources.Ocorrencias.Ocorrencia.CausasTipoOcorrencia.getFieldDescription(), required: false, idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(false) });

    // Para a pesquisa de CargaDocumentoParaEmissaoNFSManual
    this.ApenasSemOcorrencias = PropertyEntity({ val: ko.observable(true), def: true });

    //Aba Observações Fisco/Contribuinte
    this.GridObservacoesFiscoContribuinte = PropertyEntity({ type: types.local });
    this.ObservacoesFiscoContribuinte = PropertyEntity({ type: types.listEntity, list: new Array(), codEntity: ko.observable(0), idGrid: guid() });

    this.TipoObservacaoFiscoContribuinte = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.TipoObservacao.getRequiredFieldDescription(), enable: ko.observable(true), options: EnumTipoObservacaoCTe.ObterOpcoes(), val: ko.observable(EnumTipoObservacaoCTe.Contribuinte), def: EnumTipoObservacaoCTe.Contribuinte });
    this.IdentificadorObservacaoFiscoContribuinte = PropertyEntity({ required: ko.observable(false), text: Localization.Resources.Ocorrencias.Ocorrencia.Identificador.getRequiredFieldDescription(), enable: ko.observable(true), val: ko.observable(""), maxlength: 20 });
    this.DescricaoObservacaoFiscoContribuinte = PropertyEntity({ required: ko.observable(false), text: Localization.Resources.Ocorrencias.Ocorrencia.Descricao.getRequiredFieldDescription(), enable: ko.observable(true), val: ko.observable(""), maxlength: 160 });
    this.AdicionarObservacaoFiscoContribuinte = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.AdicionarObservacao, eventClick: adicionarObservacaoFiscoContribuinteOcorrenciaClick, type: types.event, visible: ko.observable(true) });
    this.OcorrenciaSalvaPelaTelaChamadoOcorrencia = PropertyEntity({ val: ko.observable(false), def: false, visible: false, getType: typesKnockout.bool })
    this.ControleDocumento = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int })
    //

    // Aba Quilometragem
    this.Quilometragem = PropertyEntity({ val: ko.observable(0), def: 0, text: Localization.Resources.Ocorrencias.Ocorrencia.Quilometros.getRequiredFieldDescription(), getType: typesKnockout.decimal, enable: ko.observable(true), visible: ko.observable(true), cssClass: ko.observable(ccsClassValor) });
    this.SalvarQuilometragem = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.SalvarKM, eventClick: salvarQuilometragemClick, type: types.event, visible: ko.observable(true) });
    this.EditarQuilometragem = PropertyEntity({ text: Localization.Resources.Ocorrencias.Ocorrencia.EditarKM, eventClick: editarQuilometragemClick, type: types.event, visible: ko.observable(true) });
    //

    //Devolução
    this.CodigoGestaoDevolucao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    var editandoCampo = false;
    this.BaseCalculoICMS.val.subscribe(function (novoValor) {
        if (!editandoCampo) {
            editandoCampo = true;

            var aliquota = Globalize.parseFloat(_ocorrencia.AliquotaICMS.val());
            var baseCalculo = Globalize.parseFloat(novoValor);

            if (isNaN(aliquota))
                aliquota = 0;

            if (isNaN(baseCalculo))
                baseCalculo = 0;

            if (aliquota > 0 && baseCalculo > 0) {
                var resultado = (baseCalculo * (aliquota / 100));
                if (resultado >= 0.01)
                    _ocorrencia.ValorICMS.val(Globalize.format(resultado, "n2"));
            }

            editandoCampo = false;
        }
    });

    this.AliquotaICMS.val.subscribe(function (novoValor) {
        if (!editandoCampo) {
            editandoCampo = true;

            var aliquota = Globalize.parseFloat(novoValor);
            var valor = Globalize.parseFloat(_ocorrencia.ValorICMS.val());
            var baseCalculo = Globalize.parseFloat(_ocorrencia.BaseCalculoICMS.val());

            if (isNaN(aliquota))
                aliquota = 0;

            if (isNaN(valor))
                valor = 0;

            if (isNaN(baseCalculo))
                baseCalculo = 0;

            if (aliquota > 0 && baseCalculo > 0) {
                var resultado = (baseCalculo * (aliquota / 100));
                if (resultado >= 0.01)
                    _ocorrencia.ValorICMS.val(Globalize.format(resultado, "n2"));
                else
                    _ocorrencia.ValorICMS.val("");
            } else if (aliquota > 0 && valor > 0) {
                var resultado = (valor / (aliquota / 100));
                if (resultado >= 0.01)
                    _ocorrencia.BaseCalculoICMS.val(Globalize.format(resultado, "n2"));
            }

            editandoCampo = false;
        }
    });

    this.ValorICMS.val.subscribe(function (novoValor) {
        if (!editandoCampo) {
            editandoCampo = true;

            var aliquota = Globalize.parseFloat(_ocorrencia.AliquotaICMS.val());
            var valor = Globalize.parseFloat(novoValor);

            if (isNaN(aliquota))
                aliquota = 0;

            if (isNaN(valor))
                valor = 0;

            if (aliquota > 0 && valor > 0) {
                var resultado = (valor / (aliquota / 100));
                if (resultado >= 0.01)
                    _ocorrencia.BaseCalculoICMS.val(Globalize.format(resultado, "n2"));
            }

            editandoCampo = false;
        }
    });

    this.CTeEmitidoNoEmbarcador.val.subscribe(function (novoValor) {
        if (_ocorrencia.Codigo.val() <= 0 || (_ocorrencia.SituacaoOcorrencia.val() == EnumSituacaoOcorrencia.AgInformacoes && _ocorrencia.OcorrenciaDeEstadia.val() == true)) {
            if (novoValor) {

                transformarGridImportarCTe();

                _ocorrencia.ValorOcorrencia.enable(false);
                _ocorrencia.ValorTotalMoeda.enable(false);
                _ocorrencia.ObservacaoCTe.enable(false);
                _ocorrencia.CobrarOutroDocumento.enable(false);
                _ocorrencia.TipoTomador.enable(false);

                _ocorrencia.ValorOcorrencia.val("0,00");
                _ocorrencia.ObservacaoCTe.val("");
                _ocorrencia.CobrarOutroDocumento.val(false);
                _ocorrencia.TipoTomador.val(99);
            }
            else {
                transformarGridEmissaoCTe();

                _ocorrencia.CobrarOutroDocumento.enable(true);
                _ocorrencia.ValorOcorrencia.enable(_ocorrencia.Moeda.val() === EnumMoedaCotacaoBancoCentral.Real);
                _ocorrencia.ObservacaoCTe.enable(true);

                //Caso a flag PermiteSelecionarTomador não estiver marcada no Tipo de Ocorrência não deve habilitar o Tomador
                if (_ocorrencia.TipoOcorrencia.PermiteSelecionarTomador)
                    _ocorrencia.TipoTomador.enable(true);
                else
                    _ocorrencia.TipoTomador.enable(false);

                if (_ocorrencia.Codigo.val() <= 0) {
                    _ocorrencia.ValorOcorrencia.val("0,00");
                    _ocorrencia.ObservacaoCTe.val("");
                    _ocorrencia.CobrarOutroDocumento.val(false);
                    _ocorrencia.TipoTomador.val(99);
                }
            }

        }
    });

    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;

    this.DataInicio.val.subscribe(function () {
        calcularTotalHoras();
    });

    this.DataFim.val.subscribe(function () {
        calcularTotalHoras();
    });
};

var CRUDOcorrencia = function () {
    this.ConfirmarOcorrencia = PropertyEntity({ eventClick: confirmarOcorenciaClick, type: types.event, text: Localization.Resources.Ocorrencias.Ocorrencia.ConfirmarOcorrencia, visible: ko.observable(false) });
    this.Adicionar = PropertyEntity({ eventClick: adicionarOcorrenciaClick, type: types.event, text: Localization.Resources.Ocorrencias.Ocorrencia.Adicionar, visible: ko.observable(true) });
    this.GerarNovaOcorrencia = PropertyEntity({ eventClick: gerarNovaOcorrenciaClick, type: types.event, text: Localization.Resources.Ocorrencias.Ocorrencia.GerarNovaOcorrencia, visible: ko.observable(false) });
    this.ReprocessarRegras = PropertyEntity({ eventClick: buscarRegrasOcorrenciaClick, type: types.event, text: Localization.Resources.Ocorrencias.Ocorrencia.ReprocessarRegras, visible: ko.observable(false) });
    this.VoltarEtapaCadastro = PropertyEntity({ eventClick: voltarParaEtapaCadastroOcorrenciaClick, type: types.event, text: Localization.Resources.Ocorrencias.Ocorrencia.VoltarParaEtapaDeCadastro, visible: ko.observable(false) });
    this.DownloadDocumentosOcorrencia = PropertyEntity({ eventClick: DownloadDocumentosOcorrenciaClick, type: types.event, text: Localization.Resources.Ocorrencias.Ocorrencia.DownloadDocumentos, visible: ko.observable(false) });
    this.ReabrirOcorrencia = PropertyEntity({ eventClick: reabrirOcorrenciaClick, type: types.event, text: Localization.Resources.Ocorrencias.Ocorrencia.ReabrirOcorrencia, visible: ko.observable(false) });

    this.Importar = PropertyEntity({
        type: types.local,
        text: Localization.Resources.Ocorrencias.Ocorrencia.Importar,
        visible: ko.observable(false),
        accept: ".xls,.xlsx,.csv",
        cssClass: "btn-default",
        UrlImportacao: "Ocorrencia/Importar",
        UrlConfiguracao: "Ocorrencia/ConfiguracaoImportacao",
        CodigoControleImportacao: EnumCodigoControleImportacao.O029_Ocorrencia,
        CallbackImportacao: function () {
            _gridViewOcorrencia.CarregarGrid();
        }
    });
};

var CargaCTeMap = function () {
    this.Codigo = PropertyEntity({ val: 0, def: 0 });
    this.CodigoCargaCTe = PropertyEntity({ val: 0, def: 0 });
    this.CodigoNotaFiscal = PropertyEntity({ val: 0, def: 0 });
};

var CargaCTeGlobalizadoNotaFiscalMap = function () {
    this.Codigo = PropertyEntity({ val: 0, def: 0 });
    this.CodigoCargaCTe = PropertyEntity({ val: 0, def: 0 });
    this.MarcarTodos = PropertyEntity({ val: true, def: true });
};

/*
 * Declaração das Funções de Inicialização
 */

function loadCTesComplementados() {
    var detalhesCTe = { descricao: Localization.Resources.Ocorrencias.Ocorrencia.DetalhesCtE, id: guid(), metodo: detalhesCTeClick, icone: "", visibilidade: VisibilidadeOpcaoDownload }
    var baixarDACTE = { descricao: Localization.Resources.Ocorrencias.Ocorrencia.BaixarDACTE, id: guid(), metodo: baixarDacteClick, icone: "", visibilidade: VisibilidadeOpcaoDownload };
    var baixarPDF = { descricao: Localization.Resources.Ocorrencias.Ocorrencia.BaixarPDF, id: guid(), metodo: baixarDacteClick, icone: "", visibilidade: VisibilidadeDownloadOutrosDoc };
    var baixarXML = { descricao: Localization.Resources.Ocorrencias.Ocorrencia.BaixarXML, id: guid(), metodo: baixarXMLCTeClick, icone: "", visibilidade: VisibilidadeOpcaoDownload };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Ocorrencias.Ocorrencia.Opcoes, tamanho: 7, opcoes: [baixarDACTE, baixarXML, baixarPDF, detalhesCTe] };

    _gridCTeComplementado = new GridView(_ocorrencia.CTesComplementados.idGrid, "Ocorrencia/ConsultaCTesComplementados", _ocorrencia, menuOpcoes, null, null, null, null, null, null, 25);
    _gridDocumentosParaEmissaoNFSManualComplementado = new GridView(_ocorrencia.DocumentosParaEmissaoNFSManualComplementados.idGrid, "Ocorrencia/ConsultaDocumentosParaEmissaoNFSManualComplementados", _ocorrencia, null, null, null, null, null, null, null, 25);
}

function loadOcorrencia() {
    _pesquisaOcorrencia = new PesquisaOcorrencia();
    KoBindings(_pesquisaOcorrencia, "knockoutPesquisaOcorrencia", false, _pesquisaOcorrencia.Pesquisar.id);

    new BuscarTipoOcorrencia(_pesquisaOcorrencia.Ocorrencia, null, null, ["", EnumFinalidadeTipoOcorrencia.Valor], null, null, null, null, null, null, true);
    new BuscarTransportadores(_pesquisaOcorrencia.Empresa);
    new BuscarFilial(_pesquisaOcorrencia.Filial);
    new BuscarClientes(_pesquisaOcorrencia.Pessoa);
    new BuscarClientes(_pesquisaOcorrencia.Tomador);
    new BuscarGruposPessoas(_pesquisaOcorrencia.GrupoPessoas, null, null, null, EnumTipoGrupoPessoas.Clientes);
    new BuscarTiposOperacao(_pesquisaOcorrencia.TipoOperacao);
    new BuscarOcorrenciaIntegracaoEmbarcador(_pesquisaOcorrencia.OcorrenciaIntegracaoEmbarcador, null, null, true);
    new BuscarCentroResultado(_pesquisaOcorrencia.CentroResultado);
    new BuscarMotorista(_pesquisaOcorrencia.Motorista);
    new BuscarClientes(_pesquisaOcorrencia.TomadorCTeOriginal);
    new BuscarGruposPessoas(_pesquisaOcorrencia.GrupoPessoasTomadorCteComplementar, null, null, null, EnumTipoGrupoPessoas.Clientes);
    new BuscarGruposPessoas(_pesquisaOcorrencia.GrupoPessoasTomadorCteOriginal, null, null, null, EnumTipoGrupoPessoas.Clientes);
    new BuscarTiposCausadoresOcorrencia(_pesquisaOcorrencia.TiposCausadoresOcorrencia);
    new BuscarCausasTipoOcorrencia(_pesquisaOcorrencia.CausasTipoOcorrencia);
    new BuscarGrupoOcorrencia(_pesquisaOcorrencia.GrupoOcorrencia);
    new BuscarClienteComplementar(_pesquisaOcorrencia.ClienteComplementar);
    new BuscarFuncionario(_pesquisaOcorrencia.Gerente);
    new BuscarFuncionario(_pesquisaOcorrencia.Supervisor);
    new BuscarFuncionario(_pesquisaOcorrencia.Vendedor);
    new BuscarEstados(_pesquisaOcorrencia.UFDestino);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
        _pesquisaOcorrencia.TipoDocumentoCreditoDebito.visible(false);
        _pesquisaOcorrencia.CTeComplementar.cssClass("col col-xs-6 col-lg-2");
        _pesquisaOcorrencia.DataInicio.cssClass("col col-xs-6 col-lg-2");
        _pesquisaOcorrencia.DataFim.cssClass("col col-xs-6 col-lg-2");
    }

    BuscarConfiguracao();

    _pesquisaOcorrencia.Upload.file = document.getElementById(_pesquisaOcorrencia.Upload.idFile);

    if (_notificacaoGlobal != null) {
        _pesquisaOcorrencia.Codigo.val(_notificacaoGlobal.CodigoObjeto.val());
        _notificacaoGlobal.CodigoObjeto.val(0);
    }

    SetarCamposMultiModal();

    carregarLancamentoOcorrencia("conteudoOcorrencia", "modaisOcorrencia");
}

function carregarLancamentoOcorrencia(idDivConteudo, idDivModais, callback) {
    $.get("Content/Static/Ocorrencia/Ocorrencia.html?dyn=" + guid(), function (dataConteudo) {
        $("#" + idDivConteudo).html(dataConteudo);
        $.get("Content/Static/Ocorrencia/ModaisOcorrencia.html?dyn=" + guid(), function (dataModais) {
            $("#" + idDivModais).html(dataModais);

            _ocorrencia = new Ocorrencia();
            KoBindings(_ocorrencia, "knockoutCadastroDeOcorrencia");

            $("#" + _ocorrencia.Anexo.id).prop("disabled", false);

            // Evita que a auditoria sobscreva auditoria de telas externas
            var isCadastro = $("#knockoutPesquisaOcorrencia").length > 0;
            if (isCadastro)
                HeaderAuditoria("CargaOcorrencia", _ocorrencia);

            loadCTeImportacao();
            loadEtapaOcorrencia();
            loadOcorrenciaAutorizacao();
            loadOcorrenciaImagem();
            loadGeolocalizacao();
            loadAceiteDebito();
            LoadAlteracaoMoedaOcorrencia();

            _CRUDOcorrencia = new CRUDOcorrencia();
            KoBindings(_CRUDOcorrencia, "knockoutCRUDCadastroOcorrencia");

            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
                _ocorrencia.NumeroOcorrenciaCliente.visible(true);
                _ocorrencia.NomeRecebedor.visible(true);
                _ocorrencia.TipoDocumentoRecebedor.visible(true);
                _ocorrencia.NumeroDocumentoRecebedor.visible(true);
                _ocorrencia.CTeEmitidoNoEmbarcador.visible(true);
                _ocorrencia.DataEvento.visible(true);
            }

            if (_CONFIGURACAO_TMS.ExigirChamadoParaAbrirOcorrencia) {
                _ocorrencia.Chamados.required(true);
                _ocorrencia.Chamados.text(Localization.Resources.Ocorrencias.Ocorrencia.Atendimentos.getRequiredFieldDescription());
            }

            if (_CONFIGURACAO_TMS.PermitirImportarOcorrencias)
                _CRUDOcorrencia.Importar.visible(true);

            if (_CONFIGURACAO_TMS.ExigirMotivoOcorrencia) {
                _ocorrencia.Motivo.required(true);
                _ocorrencia.Motivo.visible(true);
            }

            loadResumoOcorrencia();
            loadAnexosOcorrencia();
            loadAnexosCargas();
            loadAnexosVeiculoContrato();
            loadDetalhesCargaSumarizada();
            loadDetalhesDocumentosAgrupados();
            loadNFSeManual();
            loadDocumentosAgrupados();
            loadContratoFreteOcorrencia();
            SubscribersImprodutividade();
            loadGridOcorrenciaObservacaoFiscoContribuinte();
            loadOcorrenciaNFeProduto();

            new BuscarDTsNatura(_ocorrencia.DTNatura, RetornoConsultaDTNatura);
            new BuscarCTesSemCarga(_ocorrencia.CTeSemCarga, retornoBuscarCTeSemCarga)
            new BuscarComponentesDeFrete(_ocorrencia.ComponenteFrete, retornoComponenteFrete);
            new BuscarCargas(_ocorrencia.Carga, retornoCarga, null, null, null, null, null, _ocorrencia.TipoOcorrencia, true, !_CONFIGURACAO_TMS.GerarOcorrenciaParaCargaAgrupada);
            //new BuscarCargaPermiteCTeComplementar(_ocorrencia.Carga, retornoCarga, _ocorrencia.TipoOcorrencia);
            new BuscarTipoOcorrencia(_ocorrencia.TipoOcorrencia, retornoTipoOcorrenciaGrid, _ocorrencia.Carga, ["", EnumFinalidadeTipoOcorrencia.Valor], null, true, null, null, null, true, null, true);
            new BuscarModeloDocumentoFiscal(_ocorrencia.ModeloDocumentoFiscal, retornoModeloFiscal, null, true, true, null, true);
            new BuscarClientes(_ocorrencia.Tomador, RetornoConsultaTomador);
            new BuscarClientes(_ocorrencia.Destinatario);

            new BuscarChamadosParaOcorrencia(_ocorrencia.Chamado, retornoChamado);
            new BuscarChamadosParaOcorrencia(_ocorrencia.Chamados, null, _ocorrencia.Carga, true, retornoMultiplosChamados);

            new BuscarVeiculos(_ocorrencia.Veiculo, retornoConsultaVeiculo);
            new BuscarTransportadores(_ocorrencia.Empresa, retornoTransportador);
            new BuscarFuncionarioEmbarcador(_ocorrencia.UsuarioResponsavelAprovacao, null, null, null, _ocorrencia.Carga);
            new BuscarFilial(_ocorrencia.Filial);
            new BuscarCTesTerceiro(_ocorrencia.CTeTerceiro, RetornoConsultaCTeTerceiro);
            new BuscarTiposCausadoresOcorrencia(_ocorrencia.TiposCausadoresOcorrencia);
            new BuscarGrupoOcorrencia(_ocorrencia.GrupoOcorrencia, retornoGrupoOcorrencia);
            new BuscarCausasTipoOcorrencia(_ocorrencia.CausasTipoOcorrencia, _ocorrencia.TipoOcorrencia);

            loadCTeComplementar();

            if (isCadastro)
                buscarOcorrencias();
            loadCTesComplementados();
            LoadImportacaoNOTFIS()
            loadControleSaldo();
            loadOcorrenciaIntegracao();
            LoadConexaoSignalROcorrencia();
            $("#" + _ocorrencia.CobrarOutroDocumento.id).click(cobrarOutroDocumentoClick);
            $('#FileEnviarCTeDoPreCTeOcorrencia').change(EnviarCTeDoPreCTeOcorrenciaClick);

            _semTabela = false;
            _tipoSelecaoOcorrenciaPorPeriodo = false;

            if (callback != null)
                callback();

            if (_TokenAcesso != null && _TokenAcesso != "-1") {
                editarOcorrenciaComTokenAcesso(_TokenAcesso);
                window.history.replaceState(null, null, '#/Ocorrencias/Ocorrencia');
            }
        });
    });
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function RetornoConsultaCTeTerceiro(objeto) {

    _ocorrencia.CTeTerceiro.val(objeto.Descricao);
    _ocorrencia.CTeTerceiro.codEntity(objeto.Codigo);

    executarReST("Ocorrencia/ObterDetalhesCTeTerceiroParaOcorrencia", { CTeTerceiro: objeto.Codigo }, function (r) {
        if (r.Success) {
            if (r.Data) {
                retornoCarga(r.Data.Carga, function () {
                    _gridCTe.AtualizarRegistrosSelecionados([{ DT_RowId: r.Data.CTe.CodigoCargaCTe }]);
                    _gridCTe.DrawTable();

                    _ocorrencia.ValorOcorrencia.val(Globalize.format(r.Data.ValorAReceber, "n2"));
                });
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function retornoMultiplosChamados() {
    var valorTotal = 0;
    if (_ocorrencia.Chamados.multiplesEntities != null) {
        for (var i = 0; i < _ocorrencia.Chamados.multiplesEntities().length; i++) {
            var chamado = _ocorrencia.Chamados.multiplesEntities()[i];
            valorTotal += Globalize.parseFloat(chamado.Valor);
        }
    }
    _ocorrencia.ValorOcorrencia.val(Globalize.format(valorTotal, 'n2'));
}

async function adicionarOcorrenciaClick(e, sender) {
    preencherListaNFeProdutos();
    var valido = true;

    if (!_tipoSelecaoOcorrenciaPorPeriodo) {
        _ocorrencia.CargaCTes.list = new Array();
        _ocorrencia.CargaDocumentosParaEmissaoNFSManual.list = new Array();
        _ocorrencia.XMLNotasFiscaisParaCTeGlobalizado.list = new Array();
        _ocorrencia.CargaCTesImportados.val(JSON.stringify(_CTesImportadosParaComplemento));
        var ctesSelecionados;
        var documentoParaEmissaoNFSManualSelecionados;

        if (_gridCTe) {
            if (_ocorrencia.SelecionarTodos.val()) {
                ctesSelecionados = _gridCTe.ObterMultiplosNaoSelecionados();
            } else {
                ctesSelecionados = _gridCTe.ObterMultiplosSelecionados();
            }
        }

        if (_gridDocumentosParaEmissaoNFSManual) {
            if (_ocorrencia.SelecionarTodosDocumentoParaEmissaoNFSManual.val()) {
                documentoParaEmissaoNFSManualSelecionados = _gridDocumentosParaEmissaoNFSManual.ObterMultiplosNaoSelecionados();
            } else {
                documentoParaEmissaoNFSManualSelecionados = _gridDocumentosParaEmissaoNFSManual.ObterMultiplosSelecionados();
            }
        }

        // Verifica se pelo menos um CTE ou DocumentoParaEmissaoNFSManual foi selecionado
        let temPeloMenosUmCteSelecionado = ctesSelecionados && (ctesSelecionados.length > 0 || _ocorrencia.SelecionarTodos.val());
        let temPeloMenosUmDocumentoParaEmissaoNFSManualSelecionado = documentoParaEmissaoNFSManualSelecionados && (documentoParaEmissaoNFSManualSelecionados.length > 0 || _ocorrencia.SelecionarTodosDocumentoParaEmissaoNFSManual.val());
        if (temPeloMenosUmCteSelecionado || temPeloMenosUmDocumentoParaEmissaoNFSManualSelecionado) {
            $.each(ctesSelecionados, function (i, cte) {

                var map = new CargaCTeMap();
                map.Codigo.val = cte.DT_RowId;
                map.CodigoCargaCTe.val = cte.CodigoCargaCTe;
                map.CodigoNotaFiscal.val = cte.CodigoNotaFiscal;

                _ocorrencia.CargaCTes.list.push(map);

                if (cte.NotasCTeGlobalizado != null)
                    $.each(cte.NotasCTeGlobalizado, function (i, nf) {
                        var mapNF = new CargaCTeGlobalizadoNotaFiscalMap();
                        mapNF.Codigo.val = nf.DT_RowId;
                        mapNF.CodigoCargaCTe.val = cte.DT_RowId;
                        mapNF.MarcarTodos.val = cte.NotasCTeGlobalizadoSelecionarTodos;
                        _ocorrencia.XMLNotasFiscaisParaCTeGlobalizado.list.push(mapNF);
                    });
            });

            $.each(documentoParaEmissaoNFSManualSelecionados, function (i, cte) {
                var map = new CargaCTeMap();
                map.Codigo.val = cte.DT_RowId;
                _ocorrencia.CargaDocumentosParaEmissaoNFSManual.list.push(map);
            });

        } else {
            if (_ocorrencia.DTNatura.codEntity() <= 0 && (_ocorrencia.TipoOcorrencia.OcorrenciaComplementoValorFreteCarga == null || _ocorrencia.TipoOcorrencia.OcorrenciaComplementoValorFreteCarga === false)) {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Ocorrencias.Ocorrencia.EObrigatorioInformarPeloMenosUmCtEParaGerarAOcorrencia);
                valido = false;
            }
        }
    }

    if (_semTabela && !_ocorrencia.ValorOcorrencia.enable() && !_ocorrencia.TipoInclusaoImpostoComplemento.val() && _ocorrencia.ComponenteFrete.TipoComponenteFrete != EnumTipoComponenteFrete.ICMS) {
        var valor = Globalize.parseFloat(_ocorrencia.ValorOcorrencia.val());
        if (isNaN(valor))
            valor = 0;
        if (valor == 0) {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Ocorrencias.Ocorrencia.NaoExisteTabelaConfigurada);
            valido = false;
        }
    }

    if (_CONFIGURACAO_TMS.ExigirMotivoOcorrencia && (_ocorrencia.Motivo.val() == "" || _ocorrencia.Motivo.val() == " ")) {
        valido = false;
        _ocorrencia.Motivo.requiredClass("form-control");
        _ocorrencia.Motivo.requiredClass("form-control is-invalid");
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Ocorrencias.Ocorrencia.MotivoDaOcorrenciaEObrigatorio);
    }

    if (!ValidarCamposObrigatorios(_ocorrencia)) {
        valido = false;
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CampoObrigatorio, Localization.Resources.Gerais.Geral.PorFavorInformeOsCamposObrigatorios);
    }

    if (_CONFIGURACAO_TMS.HabilitarFuncionalidadeProjetoNFTP && _ocorrencia.Carga.codEntity() > 0 && _ocorrencia.TipoOcorrencia.codEntity() > 0) {
        valido = await ValidarTipoPropostaCargaOcorrencia();
        if (!valido)
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CampoObrigatorio, Localization.Resources.Gerais.Geral.TipoDePropostaSelecionadoInvalido);
    }

    if (valido) {
        var valorOcorrencia = Globalize.parseFloat(_ocorrencia.ValorOcorrencia.val());

        if (isNaN(valorOcorrencia))
            valorOcorrencia = 0;

        if (!_CONFIGURACAO_TMS.ObrigatorioRegrasOcorrencia && valorOcorrencia > 0) {
            ValidarUtilizacaoSaldo(_ocorrencia.ValorOcorrencia.val(), function (creditosUtilizados, codigoCreditorSolicitar) {
                if (creditosUtilizados != null)
                    _ocorrencia.CreditosUtilizados.val(JSON.stringify(creditosUtilizados));

                if (codigoCreditorSolicitar != null)
                    _ocorrencia.CodigoCreditorSolicitar.val(codigoCreditorSolicitar);

                if (_gridCTe != null && ctesSelecionados != null && !(_gridCTe.NumeroRegistros() === 1 && ctesSelecionados.length === 1) &&
                    (_gridCTe.NumeroRegistros() === ctesSelecionados.length) &&
                    (_InduzirTransportadorSelecionarApenasUmComplementoSolicitacaoComplementos) &&
                    (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiCTe)
                )
                    exibirConfirmacao(tipoMensagem.atencao, Localization.Resources.Ocorrencias.Ocorrencia.RealmenteDesejaSelecionarMaisQueUmComplemento, CriarOcorrencia(sender));
                else
                    CriarOcorrencia(sender);
            }, true);
        } else {
            // verifica se todos os registros foram selecionados depois se a configuração está habilitada e por fim se o ambiente é o transportador
            if (_gridCTe != null && ctesSelecionados != null && !(_gridCTe.NumeroRegistros() === 1 && ctesSelecionados.length === 1) &&
                (_gridCTe.NumeroRegistros() === ctesSelecionados.length) &&
                (_InduzirTransportadorSelecionarApenasUmComplementoSolicitacaoComplementos) &&
                (_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiCTe)
            )
                exibirConfirmacao(tipoMensagem.atencao, Localization.Resources.Ocorrencias.Ocorrencia.RealmenteDesejaSelecionarMaisQueUmComplemento, CriarOcorrencia(sender));
            else
                CriarOcorrencia(sender);
        }
    }
}

function AbrirConsultaOcorrenciaIntegracaoEmbarcadorClick() {
    $("#" + _pesquisaOcorrencia.OcorrenciaIntegracaoEmbarcador.idBtnSearch).click();
}

function DownloadDocumentosOcorrenciaClick() {
    executarDownload("Ocorrencia/DownloadDocumentosOcorrencia", { Codigo: _ocorrencia.Codigo.val() });
}

function DownloadAnexosLoteClick() {
    exibirConfirmacao(Localization.Resources.Ocorrencias.Ocorrencia.DownloadEmLote, Localization.Resources.Ocorrencias.Ocorrencia.TemCertezaQueDesejaBaixarTodosOsArquivosDasOcorrenciasFiltradas, function () {
        executarReST("Ocorrencia/DownloadLoteAnexo", RetornarObjetoPesquisa(_pesquisaOcorrencia), function (retorno) {
            if (!retorno.Success)
                return exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);

            if (!retorno.Data)
                return exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);

            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Ocorrencias.Ocorrencia.IniciadaACompactacaoEDownloadDosArquivos);
        });
    });
}

function DownloadPDFsOcorrenciaClick() {
    exibirConfirmacao(Localization.Resources.Ocorrencias.Ocorrencia.DownloadPdfsOcorrenciasEmLote, Localization.Resources.Ocorrencias.Ocorrencia.TemCertezaQueDesejaBaixarTodosOsArquivosDasOcorrenciasSelecionadas, function () {
        var ocorrenciasSelecionadas;
        if (_pesquisaOcorrencia.SelecionarTodos.val())
            ocorrenciasSelecionadas = _gridViewOcorrencia.ObterMultiplosNaoSelecionados();
        else
            ocorrenciasSelecionadas = _gridViewOcorrencia.ObterMultiplosSelecionados();

        var codigosOcorrencias = new Array();
        for (var i = 0; i < ocorrenciasSelecionadas.length; i++)
            codigosOcorrencias.push(ocorrenciasSelecionadas[i].DT_RowId);

        if (codigosOcorrencias.length > 0 || _pesquisaOcorrencia.SelecionarTodos.val())
            _pesquisaOcorrencia.ListaOcorrenciasPesquisa.val(JSON.stringify(codigosOcorrencias));
        else
            _pesquisaOcorrencia.ListaOcorrenciasPesquisa.val("");

        executarDownload("Ocorrencia/DownloadPDFsOcorrenciaLote", RetornarObjetoPesquisa(_pesquisaOcorrencia));
    });
}

function reemitirTodosCTesRejeitadosClick() {
    executarReST("CargaCTe/ReemitirTodosCTesRejeitadosOcorrencias", {}, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Ocorrencias.Ocorrencia.OsCtEsRejeitadosForamReenviadosParaEmissao);
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg, 20000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }
    });
}

function ValidarTipoPropostaCargaOcorrencia() {
    return new Promise((resolve, reject) => {
        executarReST("Ocorrencia/ValidarTipoPropostaCargaOcorrencia", { CodigoCarga: _ocorrencia.Carga.codEntity(), CodigoOcorrencia: _ocorrencia.TipoOcorrencia.codEntity() }, function (retorno) {
            if (retorno.Success) {
                resolve(retorno.Data.Valido);
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
                resolve(false);
            }
        }
        );
    });
}


function calcularValorOcorrenciaClick(e, sender) {
    switch (_ocorrencia.TipoOcorrencia.origemOcorrencia) {
        case EnumOrigemOcorrencia.PorCarga:
            calcularValorTabelaFrete(e, sender, true, null, _ocorrencia.OcorrenciaReprovada);
            break;

        case EnumOrigemOcorrencia.PorPeriodo:
            calcularValorTipoOcorrencia(sender);
            break;

        case EnumOrigemOcorrencia.PorContrato:
            CalcularValorOcorrenciaContrato();
            break;
    }
}

function cargaBlur() {
    if (_ocorrencia.Carga.val() == "") {
        _ocorrencia.Carga.codEntity(0);
        _ocorrencia.CTeEmitidoNoEmbarcador.val(_ocorrencia.CTeEmitidoNoEmbarcador.def);
        _ocorrencia.CTesParaComplemento.visibleFade(false);
        _ocorrencia.DTNatura.codEntity(0);
        _ocorrencia.DTNatura.val("");
        _ocorrencia.DTNatura.visible(false);
    }
}

function changeQuantidade(e, sender) {
    if (_ocorrencia.Quantidade.visible()) {
        var quantidades = parseInt(_ocorrencia.Quantidade.val()) || 0;
        var total = quantidades * configuracaoTipoOcorrencia.Valor;

        _ocorrencia.ValorOcorrencia.val(Globalize.format(total, 'n2'));
    }
}

function changeValorOcorrencia(e, sender) {
    if (_ocorrencia.ValorOcorrencia.enable() && _ocorrencia.Carga.codEntity() > 0) {
        if (!_tipoSelecaoOcorrenciaPorPeriodo) {
            _ocorrencia.CargaCTes.list = new Array();
            var ctesSelecionados;
            if (_gridCTe) {
                if (_ocorrencia.SelecionarTodos.val()) {
                    ctesSelecionados = _gridCTe.ObterMultiplosNaoSelecionados();
                } else {
                    ctesSelecionados = _gridCTe.ObterMultiplosSelecionados();
                }
            }

            if (ctesSelecionados && (ctesSelecionados.length > 0 || _ocorrencia.SelecionarTodos.val())) {
                $.each(ctesSelecionados, function (i, cte) {
                    var map = new CargaCTeMap();
                    map.Codigo.val = cte.DT_RowId;
                    map.CodigoCargaCTe.val = cte.CodigoCargaCTe;
                    _ocorrencia.CargaCTes.list.push(map);
                });
            } else {
                return;
            }
        }

        calcularValorTabelaFrete(_ocorrencia);
    }
}

function cobrarOutroDocumentoClick() {
    if (_ocorrencia.CobrarOutroDocumento.val()) {

        _ocorrencia.ModeloDocumentoFiscal.required = true;
        _ocorrencia.ModeloDocumentoFiscal.enable(true);

        if (_ocorrencia.CTeEmitidoNoEmbarcador.val()) {
            transformarGridEmissaoCTe();
            _ocorrencia.ValorOcorrencia.enable(true);
            _ocorrencia.ObservacaoCTe.enable(true);
        }

    } else {

        _ocorrencia.DadosModeloDocumentoFiscal.val(_ocorrencia.DadosModeloDocumentoFiscal.def);
        _ocorrencia.ModeloDocumentoFiscal.enable(false);
        _ocorrencia.ModeloDocumentoFiscal.required = false;
        _ocorrencia.ModeloDocumentoFiscal.val("");
        _ocorrencia.ModeloDocumentoFiscal.codEntity(0);

        if (!_ocorrencia.CTeEmitidoNoEmbarcador.val()) {
            transformarGridEmissaoCTe();
        } else {
            _ocorrencia.ValorTotalMoeda.enable(false);
            _ocorrencia.ValorOcorrencia.enable(false);
            _ocorrencia.ObservacaoCTe.enable(false);
            _ocorrencia.ValorOcorrencia.val("0,00");
            _ocorrencia.ObservacaoCTe.val("");
            transformarGridImportarCTe();
        }
    }
}

function componenteFreteBlur() {
    if (_ocorrencia.ComponenteFrete.val() == "") {
        _ocorrencia.ComponenteFrete.codEntity(0);
        visiblidadeValorOcorrencia();
        if (_ocorrencia.CTeEmitidoNoEmbarcador.val()) {
            transformarGridEmissaoCTe();
        }
    }
}

function confirmarOcorenciaClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Ocorrencias.Ocorrencia.RealmenteDesejaConfirmarEstaOcorrencia, function () {
        ValidarUtilizacaoSaldo(_ocorrencia.ValorOcorrencia.val(), function (creditosUtilizados, codigoCreditorSolicitar) {
            if (creditosUtilizados != null)
                _ocorrencia.CreditosUtilizados.val(JSON.stringify(creditosUtilizados));

            if (codigoCreditorSolicitar != null)
                _ocorrencia.CodigoCreditorSolicitar.val(codigoCreditorSolicitar);

            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
                if (_ocorrencia.OcorrenciaDeEstadia.val() == true) {
                    _ocorrencia.CargaCTes.list = new Array();
                    _ocorrencia.CargaCTesImportados.val(JSON.stringify(_CTesImportadosParaComplemento));
                    var ctesSelecionados;
                    if (_gridCTe) {
                        if (_ocorrencia.SelecionarTodos.val()) {
                            ctesSelecionados = _gridCTe.ObterMultiplosNaoSelecionados();
                        } else {
                            ctesSelecionados = _gridCTe.ObterMultiplosSelecionados();
                        }
                    }
                    let temPeloMenosUmCteSelecionado = ctesSelecionados && (ctesSelecionados.length > 0 || _ocorrencia.SelecionarTodos.val());
                    if (temPeloMenosUmCteSelecionado) {
                        $.each(ctesSelecionados, function (i, cte) {
                            var map = new CargaCTeMap();
                            map.Codigo.val = cte.DT_RowId;
                            map.CodigoCargaCTe.val = cte.CodigoCargaCTe;
                            _ocorrencia.CargaCTes.list.push(map);
                        });
                    } else {
                        if (_ocorrencia.DTNatura.codEntity() <= 0 && (_ocorrencia.TipoOcorrencia.OcorrenciaComplementoValorFreteCarga == null || _ocorrencia.TipoOcorrencia.OcorrenciaComplementoValorFreteCarga === false)) {
                            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Ocorrencias.Ocorrencia.EObrigatorioInformarPeloMenosUmCtEParaGerarAOcorrencia);
                            return;
                        }
                    }
                    if (!_ocorrencia.ValorOcorrencia.enable()) {
                        var valor = Globalize.parseFloat(_ocorrencia.ValorOcorrencia.val());
                        if (isNaN(valor))
                            valor = 0;
                        if (valor == 0) {
                            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Ocorrencias.Ocorrencia.EObrigatorioInformarOValorDaOcorrencia);
                            return;
                        }
                    }
                }
            } else {
                _ocorrencia.CargaCTes.list = new Array();
                var ctesSelecionados;
                if (_gridCTe) {
                    if (_ocorrencia.SelecionarTodos.val()) {
                        ctesSelecionados = _gridCTe.ObterMultiplosNaoSelecionados();
                    } else {
                        ctesSelecionados = _gridCTe.ObterMultiplosSelecionados();
                    }
                }
                let temPeloMenosUmCteSelecionado = ctesSelecionados && (ctesSelecionados.length > 0 || _ocorrencia.SelecionarTodos.val());
                if (temPeloMenosUmCteSelecionado) {
                    $.each(ctesSelecionados, function (i, cte) {
                        var map = new CargaCTeMap();
                        map.Codigo.val = cte.DT_RowId;
                        map.CodigoCargaCTe.val = cte.CodigoCargaCTe;
                        _ocorrencia.CargaCTes.list.push(map);
                    });
                } else {
                    if (_ocorrencia.DTNatura.codEntity() <= 0 && (_ocorrencia.TipoOcorrencia.OcorrenciaComplementoValorFreteCarga == null || _ocorrencia.TipoOcorrencia.OcorrenciaComplementoValorFreteCarga === false)) {
                        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Ocorrencias.Ocorrencia.EObrigatorioInformarPeloMenosUmCtEParaGerarAOcorrencia);
                        return;
                    }

                    if (_ocorrencia.OcorrenciaDeEstadia.val() == true)
                        _ocorrencia.CargaCTesImportados.val(JSON.stringify(_CTesImportadosParaComplemento));

                    if (!_ocorrencia.ValorOcorrencia.enable()) {
                        var valor = Globalize.parseFloat(_ocorrencia.ValorOcorrencia.val());
                        if (isNaN(valor))
                            valor = 0;
                        if (valor == 0) {
                            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Ocorrencias.Ocorrencia.EObrigatorioInformarOValorDaOcorrencia);
                            return;
                        }
                    }
                }
            }

            iniciarRequisicao();
            Salvar(_ocorrencia, "Ocorrencia/ComplementarInformacoesOcorrencia", function (arg) {
                if (arg.Success) {
                    if (arg.Data != false) {
                        preencherRetornoOcorrencia(arg);
                        _CRUDOcorrencia.ConfirmarOcorrencia.visible(false);
                    } else {
                        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
                        finalizarRequisicao();
                    }
                    //AtualizarDadosControleSaldo();
                } else {
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
                    finalizarRequisicao();
                }
            }, sender, exibirCamposObrigatorioOcorrencia);
        }, true);
    });
}

function DTNaturaOnBlur() {
    if (_ocorrencia.DTNatura.val() == "" && _ocorrencia.DTNatura.codEntity() > 0) {
        _ocorrencia.DTNatura.codEntity(0);

        _ocorrencia.ValorOcorrencia.val("0,00");

        _ocorrencia.SelecionarTodos.visible(!_ocorrencia.DefinirPeriodoEstadiaAutomaticamente.val());
        _ocorrencia.SelecionarTodos.val(!_ocorrencia.DefinirPeriodoEstadiaAutomaticamente.val());
        _ocorrencia.CTesParaComplemento.text(Localization.Resources.Ocorrencias.Ocorrencia.SelecionarCtESParaComplementoArquivosEnviados);
        _gridCTe.SetarRegistrosSomenteLeitura(false);
        _gridCTe.CarregarGrid();
    }
}

function gerarNovaOcorrenciaClick(e) {
    limparCamposOcorrencia();
}

function periodoCargaBlur() {
    if (_ocorrencia.Empresa.val() == "")
        _ocorrencia.Empresa.codEntity(0);

    if (_ocorrencia.Filial.val() == "")
        _ocorrencia.Filial.codEntity(0);

    if (_ocorrencia.PeriodoAno.val() != "" && _ocorrencia.Codigo.val() <= 0 && _ocorrencia.TipoOcorrencia.codEntity() > 0 && (_ocorrencia.Empresa.codEntity() > 0 || _CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiEmbarcador)) {
        switch (_ocorrencia.TipoOcorrencia.origemOcorrencia) {
            case EnumOrigemOcorrencia.PorPeriodo:

                // Mostra a grid
                if (configuracaoTipoOcorrencia.OcorrenciaDestinadaFranquias) {
                    // Busca Contrato de Frete
                    BuscarContratoFreteTransportadorReferenteAosDados();
                    _ocorrencia.VeiculosImprodutivos.visibleFade(true);
                } else {
                    // Busca lista de cargas
                    BuscarCargasPorPeriodo();

                    _ocorrencia.CargasParaComplemento.visibleFade(true);
                }
                break;
            case EnumOrigemOcorrencia.PorContrato:
                if (configuracaoTipoOcorrencia.OcorrenciaComVeiculo) {
                    RecarregarGridDocumentosAgrupados();
                } else {
                    BuscarMotoristaContrato(function () {
                        BuscarVeiculosContrato(function () {
                            VerificarDescontoOutrasCargasContrato();
                            _ocorrencia.VeiculosContrato.visibleFade(true);
                            _ocorrencia.MotoristasContrato.visibleFade(true);
                        });
                    });
                }
                break;
        }
    }
}

function tipoTomadorChange(e) {
    verificarSeTomadorEmiteCTe(function () {
        if (!_ocorrencia.CTeEmitidoNoEmbarcador.val() || _ocorrencia.CobrarOutroDocumento.val() === true) {
            transformarGridEmissaoCTe();
        } else {
            transformarGridImportarCTe();
        }
    });
}

function UploadChange() {
    if (_pesquisaOcorrencia.Upload.file.files.length > 0) {
        var data = new FormData();

        for (var i = 0, s = _pesquisaOcorrencia.Upload.file.files.length; i < s; i++) {
            data.append("Arquivo", _pesquisaOcorrencia.Upload.file.files[i]);
        }

        enviarArquivo("Ocorrencia/EnviarArquivo", {}, data, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data !== false) {
                    if (retorno.Data.Adicionados > 0) {
                        if (retorno.Data.Adicionados > 1)
                            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Ocorrencias.Ocorrencia.ArquivosEnviados.format(retorno.Data.Adicionados));
                        else
                            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Ocorrencias.Ocorrencia.ArquivoEnviado.format(retorno.Data.Adicionados));
                    }

                    if (retorno.Data.Erros.length > 0)
                        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, Localization.Resources.Ocorrencias.Ocorrencia.OcorreuErroS.format(retorno.Data.Erros));
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
            }
            $("#" + _pesquisaOcorrencia.Upload.idFile).val("");
        });
    }
}

function veiculoBlur() {
    if (_ocorrencia.Veiculo.val() != "" && _ocorrencia.Veiculo.codEntity() > 0 && configuracaoTipoOcorrencia != null) {
        switch (_ocorrencia.TipoOcorrencia.origemOcorrencia) {
            case EnumOrigemOcorrencia.PorContrato:
                if (configuracaoTipoOcorrencia.OcorrenciaComVeiculo) {
                    RecarregarGridDocumentosAgrupados();
                }
                break;
        }
    }
}

function voltarParaEtapaCadastroOcorrenciaClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Ocorrencias.Ocorrencia.EssaAcaoRemoveraTodasAsAprovacoesRealmenteDesejaVoltarEstaOcorrenciaParaAEtapaDeCadastro, function () {
        executarReST("Ocorrencia/VoltarParaEtapaCadastro", { Codigo: _ocorrencia.Codigo.val() }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Ocorrencias.Ocorrencia.OcorrenciaRetornadaParaAEtapaDeCadastroComSucesso);
                    buscarOcorrenciaPorCodigo();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        });
    });
}

/*
 * Declaração das Funções Públicas
 */

function BuscarConfiguracao() {
    //executarReST("Ocorrencia/ObterConfiguracao", {}, function (r) {
    //    if (r.Success && r.Data) {
    //        _pesquisaOcorrencia.Upload.visible(r.Data.TemIntegracaoIntelipost);
    //    }
    //});
}

function BuscarSelecionados() {
    var selecionados = new Array();
    $.each(_CTesImportadosParaComplemento, function (i, obj) {
        selecionados.push({ DT_RowId: obj.CodigoCargaCTeParaComplementar });
    });
    return selecionados;
}

function ContarLinhasECalcularValorTipoOcorrencia() {
    if (_gridCargasComplemento.NumeroRegistros() > 0)
        calcularValorTipoOcorrencia(false);
    else
        _ocorrencia.ValorOcorrencia.val(0);
}

function controlarCamposParametrosHabilitados(habilitar) {
    _ocorrencia.ApenasReboque.enable(habilitar);
    _ocorrencia.DataFim.enable(habilitar);
    _ocorrencia.DataInicio.enable(habilitar);
    _ocorrencia.ParametroData1.enable(habilitar);
    _ocorrencia.ParametroData2.enable(habilitar);
    _ocorrencia.ParametroInteiro.enable(habilitar);
    _ocorrencia.ParametroTexto.enable(habilitar);

    _ocorrencia.CalcularFrete.visible(habilitar);
}

function controlarExibicaoCamposParametros(codigoTipoOcorrencia) {
    if (_cteEmitidoEmbarcador)
        return;

    ocultarExibicaoCamposParametros();

    if (codigoTipoOcorrencia <= 0) {
        _ocorrencia.DefinirPeriodoEstadiaAutomaticamente.val(false);
        _ocorrencia.DividirOcorrencia.val(false);
        _ocorrencia.ObservacaoCTe.val("");
        _ocorrencia.ObservacaoCTeDestino.val("");
        _ocorrencia.ObservacaoOcorrencia.val("");
        _ocorrencia.ObservacaoOcorrenciaDestino.val("");
        _ocorrencia.ValorOcorrenciaDestino.val("0,00");

        return;
    }

    executarReST("TipoOcorrencia/BuscarPorCodigo", { codigo: codigoTipoOcorrencia }, function (arg) {
        if (arg.Success) {
            if (arg.Data != null) {
                var debitaFreeTime = arg.Data.DebitaFreeTimeCalculoValorOcorrencia;
                var utilizaEntradaSaidaRaio = arg.Data.UtilizarEntradaSaidaDoRaioCargaEntrega;
                var definirPeriodoEstadiaAutomaticamente = Boolean(arg.Data.Gatilho.DefinirAutomaticamenteTempoEstadiaPorTempoParadaNoLocalEntrega) || utilizaEntradaSaidaRaio;
                _ocorrencia.DefinirPeriodoEstadiaAutomaticamente.val(definirPeriodoEstadiaAutomaticamente);
                _ocorrencia.TipoInclusaoImpostoComplemento.val(arg.Data.TipoInclusaoImpostoComplemento);

                if (arg.Data.Emitente == EnumEmitenteTipoOcorrencia.Outros && _CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.Terceiros) {
                    if (arg.Data.OutroEmitente != null) {
                        _ocorrencia.Tomador.codEntity(arg.Data.OutroEmitente.Codigo);
                        _ocorrencia.Tomador.val(arg.Data.OutroEmitente.Descricao);
                    }

                    _ocorrencia.Tomador.visible(true);
                    _ocorrencia.Tomador.enable(true);
                }
                else if (arg.Data.NaoGerarDocumento && arg.Data.PermiteSelecionarTomador) {
                    _ocorrencia.Tomador.visible(true);
                    _ocorrencia.Tomador.enable(_ocorrencia.Codigo.val() <= 0);
                }
                else {
                    _ocorrencia.Tomador.visible(false);
                    _ocorrencia.Tomador.enable(false);
                }

                if (arg.Data.PermiteInformarAprovadorResponsavel) {
                    _ocorrencia.UsuarioResponsavelAprovacao.enable(_ocorrencia.TipoOcorrencia.enable());
                    _ocorrencia.UsuarioResponsavelAprovacao.visible(true);
                    _ocorrencia.UsuarioResponsavelAprovacao.required = true;
                }

                if (arg.Data.OcorrenciaPorAjudante) {
                    _ocorrencia.QuantidadeAjudantes.visible(true);
                    _ocorrencia.QuantidadeAjudantes.required(true);
                    _ocorrencia.ValorOcorrencia.visibleFade(true);
                    //_ocorrencia.ValorOcorrencia.visible(false);
                    _ocorrencia.TipoTomador.visible(false);
                }

                if (arg.Data.EfetuarOControleQuilometragem) {
                    $("#liControleQuilometragem").show();
                } else {
                    $("#liControleQuilometragem").hide();
                }

                if (arg.Data.PermiteInformarCausadorOcorrencia) {
                    _ocorrencia.TiposCausadoresOcorrencia.visible(true);
                    _ocorrencia.CausasTipoOcorrencia.visible(true);
                }


                if (arg.Data.PermitirInformarGrupoOcorrencia) {
                    _ocorrencia.GrupoOcorrencia.visible(true);
                    _ocorrencia.GrupoOcorrencia.required = true;
                }

                var parametros = arg.Data.Gatilho.Parametro ? [arg.Data.Gatilho.Parametro] : arg.Data.Parametros;

                $.each(parametros, function (i, obj) {
                    _ocorrencia.CalcularFrete.visible(true);
                    _ocorrencia.ParametrosOcorrencia.visibleFade(true);
                    $("#liParametrosOcorrencia a").click();
                    $("#liParametrosOcorrencia a").tab("show");
                    $("liObservacoesFiscoContribuinte").show();
                    _ocorrencia.ObservacaoOcorrencia.visible(false);
                    _ocorrencia.ObservacaoOcorrenciaDestino.visible(false);
                    _ocorrencia.ValorOcorrenciaDestino.visible(false);

                    switch (obj.TipoParametro) {
                        case EnumTipoParametroOcorrencia.Booleano:
                            _ocorrencia.CodigoParametroBooleano.val(obj.Codigo);
                            _ocorrencia.ApenasReboque.required = false;
                            _ocorrencia.ApenasReboque.visible(true);
                            break;

                        case EnumTipoParametroOcorrencia.Data:
                            if (_ocorrencia.CodigoParametroData1.val() == "") {
                                _ocorrencia.CodigoParametroData1.val(obj.Codigo);
                                _ocorrencia.ParametroData1.text((obj.DescricaoParametro || obj.Descricao) + ":");
                                _ocorrencia.ParametroData1.required = true;
                                _ocorrencia.ParametroData1.visible(true);
                            }
                            else if (_ocorrencia.CodigoParametroData2.val() == "") {
                                _ocorrencia.CodigoParametroData2.val(obj.Codigo);
                                _ocorrencia.ParametroData2.text(obj.Descricao + ":");
                                _ocorrencia.ParametroData2.required = true;
                                _ocorrencia.ParametroData2.visible(true);
                            }
                            break;

                        case EnumTipoParametroOcorrencia.Inteiro:
                            _ocorrencia.CodigoParametroInteiro.val(obj.Codigo);

                            if (arg.Data.CalculaValorPorTabelaFrete) {
                                _ocorrencia.ParametroInteiro.text(obj.Descricao + ":");
                                _ocorrencia.ParametroInteiro.required = true;
                                _ocorrencia.ParametroInteiro.visible(true);
                            }
                            else
                                calcularValorTabelaFrete(_ocorrencia);
                            break;

                        case EnumTipoParametroOcorrencia.Periodo:
                            _ocorrencia.CodigoParametroPeriodo.val(obj.Codigo);
                            _ocorrencia.DataInicio.required(true);
                            _ocorrencia.DataInicio.visible(true);
                            _ocorrencia.DataInicio.text(_ocorrencia.DataInicio.defText);
                            _ocorrencia.DataFim.required(true);
                            _ocorrencia.DataFim.visible(true);
                            _ocorrencia.DataFim.text(_ocorrencia.DataFim.defText);
                            _ocorrencia.TotalHoras.visible(true);
                            if (debitaFreeTime) {
                                _ocorrencia.FreeTime.visible(true);
                                _ocorrencia.TotalHorasCalculo.visible(true);

                                _ocorrencia.FreeTime.val(arg.Data.FreeTime);
                                calcularTotalHoras();
                            }

                            if (obj.DescricaoParametro)
                                _ocorrencia.DataInicio.text(obj.DescricaoParametro + ":");

                            if (obj.DescricaoParametroFinal)
                                _ocorrencia.DataFim.text(obj.DescricaoParametroFinal + ":");

                            if (_ocorrencia.DefinirPeriodoEstadiaAutomaticamente.val() && (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe && !arg.Data.PermitirTransportadorInformarDataInicioFimRaioCarga))
                                _ocorrencia.DataInicio.enable(false);

                            break;

                        case EnumTipoParametroOcorrencia.Texto:
                            _ocorrencia.CodigoParametroTexto.val(obj.Codigo);
                            _ocorrencia.ParametroTexto.text(obj.Descricao + ":");
                            _ocorrencia.ParametroTexto.required = true;
                            _ocorrencia.ParametroTexto.visible(true);
                            break;
                    }
                });

                if (_ocorrencia.TipoOcorrencia.enable() && _ocorrencia.SituacaoOcorrencia.val() != EnumSituacaoOcorrencia.AgInformacoes) {
                    _ocorrencia.ValorOcorrencia.enable(arg.Data.PermiteInformarValor && _ocorrencia.Moeda.val() === EnumMoedaCotacaoBancoCentral.Real);

                    if (_ocorrencia.Chamado.codEntity() <= 0)
                        _ocorrencia.ValorOcorrencia.val("");

                    if (!arg.Data.PermiteInformarValor && !_ocorrencia.CalcularFrete.visible()) {
                        if (_ocorrencia.Carga.codEntity() > 0) {
                            function callbackGridCTe(response) {
                                calcularValorTabelaFrete(_ocorrencia, null, (!utilizaEntradaSaidaRaio ? true : false), response.data);
                            };

                            _gridCTe.CarregarGrid(callbackGridCTe);
                        } else {
                            calcularValorTabelaFrete(_ocorrencia, null, (!utilizaEntradaSaidaRaio ? true : false));
                        }
                    }

                }

                if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS && arg.Data.ModeloDocumentoFiscal.Codigo > 0) {
                    _ocorrencia.CobrarOutroDocumento.val(true);
                    _ocorrencia.ModeloDocumentoFiscal.enable(true);
                    _ocorrencia.ModeloDocumentoFiscal.codEntity(arg.Data.ModeloDocumentoFiscal.Codigo);
                    _ocorrencia.ModeloDocumentoFiscal.val(arg.Data.ModeloDocumentoFiscal.Descricao);
                }

                if (configuracaoTipoOcorrencia != null && _ocorrencia.Chamado.codEntity() > 0)
                    _ocorrencia.ValorOcorrencia.enable(!configuracaoTipoOcorrencia.NaoPermiteInformarValorDaOcorrenciaAoSelecionarAtendimentos);
            }
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
    });
}

function controlarExibicaoPeriodoOcorrencia() {
    if (_ocorrencia.Periodo.val() == EnumPeriodoAcordoContratoFreteTransportador.NaoPossui)
        return;

    _ocorrencia.PeriodoDezena.visible(false);
    _ocorrencia.PeriodoQuinzena.visible(false);

    if ((_ocorrencia.Periodo.val() == EnumPeriodoAcordoContratoFreteTransportador.Decendial) || (_ocorrencia.Periodo.val() == EnumPeriodoAcordoContratoFreteTransportador.Quinzenal)) {
        _ocorrencia.PeriodoMes.cssClass("col col-xs-12 col-md-2");
        _ocorrencia.PeriodoAno.cssClass("col col-xs-12 col-md-2");

        if (_ocorrencia.Periodo.val() == EnumPeriodoAcordoContratoFreteTransportador.Decendial)
            _ocorrencia.PeriodoDezena.visible(true);
        else
            _ocorrencia.PeriodoQuinzena.visible(true);
    }
    else {
        _ocorrencia.PeriodoMes.cssClass("col col-xs-12 col-md-3");
        _ocorrencia.PeriodoAno.cssClass("col col-xs-12 col-md-3");
    }
}
function salvarQuilometragemClick() {
    if (_ocorrencia.Quilometragem.val() != "") {
        exibirConfirmacao("Confirmação", "Deseja realmente alterar o valor da quilometragem para " + _ocorrencia.Quilometragem.val() + "?", function () {
            _ocorrencia.Quilometragem.enable(false);
            calcularValorOcorrenciaClick(_ocorrencia);
        });
        return;
    }
}

function editarQuilometragemClick() {
    exibirConfirmacao("Confirmação", "Deseja realmente alterar o valor da quilometragem?", function () {
        _ocorrencia.Quilometragem.enable(true);
    });
}
function controlarPeriodoOcorrencia() {
    if (!_ocorrencia.TipoOcorrencia.porPeriodo)
        _ocorrencia.Periodo.val(EnumPeriodoAcordoContratoFreteTransportador.NaoPossui);
    else {
        if (_ocorrencia.ContratoFreteTransportador.periodoAcordo !== EnumPeriodoAcordoContratoFreteTransportador.NaoPossui)
            _ocorrencia.Periodo.val(_ocorrencia.ContratoFreteTransportador.periodoAcordo);
        else
            _ocorrencia.Periodo.val(_ocorrencia.TipoOcorrencia.periodo);
    }

    controlarExibicaoPeriodoOcorrencia();
}

function controlarExigenciaChamadoParaAbrirOcorrencia() {
    if (_CONFIGURACAO_TMS.ExigirChamadoParaAbrirOcorrencia)
        return;

    var exigirChamadoParaAbrirOcorrencia = (configuracaoTipoOcorrencia != null) && configuracaoTipoOcorrencia.ExigirChamadoParaAbrirOcorrencia;

    _ocorrencia.Chamado.visible(exigirChamadoParaAbrirOcorrencia);
    _ocorrencia.Chamado.required(exigirChamadoParaAbrirOcorrencia);
    _ocorrencia.Carga.enable(!exigirChamadoParaAbrirOcorrencia && _CRUDOcorrencia.Adicionar.visible());
}

function DefineModoCalculoOcorrencia() {
    switch (_ocorrencia.TipoOcorrencia.origemOcorrencia) {
        case EnumOrigemOcorrencia.PorCarga:
            if (configuracaoTipoOcorrencia.GerarOcorrenciaComMesmoValorCTesAnteriores)
                _ocorrencia.CalcularValorOcorrencia.visible(true);
            break;
        case EnumOrigemOcorrencia.PorPeriodo:
            if (configuracaoTipoOcorrencia.PermiteInformarValor) {
                _ocorrencia.ValorOcorrencia.enable(true);
                _ocorrencia.CalcularValorOcorrencia.visible(false);
            } else {
                _ocorrencia.ValorOcorrencia.enable(false);
                _ocorrencia.CalcularValorOcorrencia.visible(true);
            }
            break;
        case EnumOrigemOcorrencia.PorContrato:
            break;
    }
}

function ExibeNotificarDebitosAtivos() {
    var modeloDocFiscalDoTipoDebito = configuracaoTipoOcorrencia != null && configuracaoTipoOcorrencia.ModeloDocumentoFiscal != null && configuracaoTipoOcorrencia.ModeloDocumentoFiscal.TipoDocumentoCreditoDebito == EnumTipoDocumentoCreditoDebito.Debito;
    var acessoMultiCTe = _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe;

    _ocorrencia.NotificarDebitosAtivos.visible(modeloDocFiscalDoTipoDebito && !acessoMultiCTe);
}

function exibirCamposObrigatorioOcorrencia() {
    exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CampoObrigatorio, Localization.Resources.Gerais.Geral.PorFavorInformeOsCamposObrigatorios);
    finalizarRequisicao();
}

function ExibirErroDataRow(_grid, row, mensagem, tipoMensagem, titulo) {
    if (_grid.DesfazerAlteracaoDataRow)
        _grid.DesfazerAlteracaoDataRow(row);
    exibirMensagem(tipoMensagem, titulo, mensagem);
}

function obterPeriodoFim() {
    var ultimoDiaPeriodo = new Date(_ocorrencia.PeriodoAno.val(), _ocorrencia.PeriodoMes.val(), 0).getDate() + '';
    var dia = ultimoDiaPeriodo;

    if (_ocorrencia.Periodo.val() == EnumPeriodoAcordoContratoFreteTransportador.Decendial) {
        if (_ocorrencia.PeriodoDezena.val() == EnumDezena.Primeira)
            dia = '10';
        else if (_ocorrencia.PeriodoDezena.val() == EnumDezena.Segunda)
            dia = '20';
    }
    else if (_ocorrencia.Periodo.val() == EnumPeriodoAcordoContratoFreteTransportador.Quinzenal) {
        if (_ocorrencia.PeriodoQuinzena.val() == EnumQuinzena.Primeira)
            dia = '15';
    }

    return obterDataPeriodoFormatada(dia);
}

function obterPeriodoInicio() {
    var dia = "01";

    if (_ocorrencia.Periodo.val() == EnumPeriodoAcordoContratoFreteTransportador.Decendial) {
        if (_ocorrencia.PeriodoDezena.val() == EnumDezena.Segunda)
            dia = '11';
        else if (_ocorrencia.PeriodoDezena.val() == EnumDezena.Terceira)
            dia = '21';
    }
    else if (_ocorrencia.Periodo.val() == EnumPeriodoAcordoContratoFreteTransportador.Quinzenal) {
        if (_ocorrencia.PeriodoQuinzena.val() == EnumQuinzena.Segunda)
            dia = '16';
    }

    return obterDataPeriodoFormatada(dia);
}

function ocultarExibicaoCamposParametros() {
    _ocorrencia.CalcularFrete.visible(false);
    _ocorrencia.ParametrosOcorrencia.visibleFade(false);
    $("#liValorOcorrencia a").click();
    $("#liValorOcorrencia a").tab("show");

    _ocorrencia.CodigoParametroBooleano.val("");
    _ocorrencia.ApenasReboque.required = false;
    _ocorrencia.ApenasReboque.visible(false);

    _ocorrencia.CodigoParametroData1.val("");
    _ocorrencia.ParametroData1.required = false;
    _ocorrencia.ParametroData1.visible(false);

    _ocorrencia.CodigoParametroData2.val("");
    _ocorrencia.ParametroData2.required = false;
    _ocorrencia.ParametroData2.visible(false);

    _ocorrencia.CodigoParametroInteiro.val("");
    _ocorrencia.ParametroInteiro.required = false;
    _ocorrencia.ParametroInteiro.visible(false);

    _ocorrencia.CodigoParametroPeriodo.val("");
    _ocorrencia.DataInicio.required(false);
    _ocorrencia.DataInicio.visible(false);
    _ocorrencia.DataFim.required(false);
    _ocorrencia.DataFim.visible(false);
    _ocorrencia.TotalHoras.visible(false);
    _ocorrencia.FreeTime.visible(false);
    _ocorrencia.TotalHorasCalculo.visible(false);

    _ocorrencia.CodigoParametroTexto.val("");
    _ocorrencia.ParametroTexto.required = false;
    _ocorrencia.ParametroTexto.visible(false);

    _ocorrencia.TiposCausadoresOcorrencia.visible(false);
    _ocorrencia.CausasTipoOcorrencia.visible(false);
}

function preecherCamposEdicaoOcorrencia(origemOcorrencia, arg) {
    AlternaEtapas();

    _ocorrencia.ComponenteFrete.TipoComponenteFrete = arg.Data.ComponenteFrete.TipoComponenteFrete;
    _ocorrencia.CTesParaComplemento.visibleFade(true);
    _ocorrencia.TipoOcorrencia.OcorrenciaComplementoValorFreteCarga = arg.Data.TipoOcorrencia.OcorrenciaComplementoValorFreteCarga;
    visibilidadeCamposOcorrencia();
    _ocorrencia.TipoOcorrencia.origemOcorrencia = origemOcorrencia;
    _ocorrencia.TipoOcorrencia.CalculaValorPorTabelaFrete = arg.Data.TipoOcorrencia.CalculaValorPorTabelaFrete;
    _ocorrencia.TipoOcorrencia.NaoCalcularValorOcorrenciaAutomaticamente = arg.Data.TipoOcorrencia.NaoCalcularValorOcorrenciaAutomaticamente;

    if (_pesquisaOcorrencia != undefined && _pesquisaOcorrencia != null)
        _pesquisaOcorrencia.ExibirFiltros.visibleFade(false);
    _CRUDOcorrencia.GerarNovaOcorrencia.visible(true);
    if (_ocorrencia.SituacaoOcorrencia.val() == EnumSituacaoOcorrencia.Rejeitada) {
        _CRUDOcorrencia.ReabrirOcorrencia.visible(true);
    } else {
        _CRUDOcorrencia.ReabrirOcorrencia.visible(false);
    }
    _CRUDOcorrencia.DownloadDocumentosOcorrencia.visible(_CONFIGURACAO_TMS.PermiteBaixarAnexoOcorrenciaEmLote);
    controlarExibicaoCamposParametros(_ocorrencia.TipoOcorrencia.codEntity());

    switch (origemOcorrencia) {
        case EnumOrigemOcorrencia.PorCarga:
            _anexosOcorrencia.Anexos.val(arg.Data.Anexos);

            if (configuracaoTipoOcorrencia != null && configuracaoTipoOcorrencia.OcorrenciaPorQuantidade) {
                _ocorrencia.Quantidade.visible(true);
                _ocorrencia.Quantidade.enable(false);
                _ocorrencia.ValorOcorrencia.enable(false);
            }

            if (_ocorrencia.UtilizarSelecaoPorNotasFiscaisCTe.val())
                _ocorrencia.UtilizarSelecaoPorNotasFiscaisCTe.visible(true);

            break;
        case EnumOrigemOcorrencia.PorPeriodo:
            if (configuracaoTipoOcorrencia != null && configuracaoTipoOcorrencia.OcorrenciaDestinadaFranquias) {
                RecarregarGridImprodutividade(function () {
                    _ocorrencia.ContratoFreteTransportador.visible(true);
                    _ocorrencia.VeiculosImprodutivos.visibleFade(true);
                });
            } else {
                BuscarCargasPorPeriodo(function () {
                    _ocorrencia.CargasParaComplemento.visibleFade(true);
                });
            }
            PreencherAnexosCargas();
            _anexosOcorrencia.Anexos.val(arg.Data.Anexos);
            break;
        case EnumOrigemOcorrencia.PorContrato:
            if (configuracaoTipoOcorrencia.OcorrenciaComVeiculo) {
                ConsultaInformacaoOcorrenciaPorContrato();
                _anexosOcorrencia.Anexos.val(arg.Data.Anexos);
            } else {
                BuscarVeiculosContrato(function () {
                    _ocorrencia.VeiculosContrato.visibleFade(true);
                    _ocorrencia.MotoristasContrato.visibleFade(true);
                });
                PreencherAnexosVeiculoContratos();
                _anexosOcorrencia.Anexos.val(arg.Data.Anexos);
            }
            break;
    }

    FluxoOcorrenciaPorPeriodo(arg.Data.OcorrenciaPorPeriodo);

    _ocorrencia.Filial.visible(_ocorrencia.Filial.codEntity() > 0);
    _ocorrencia.CTeTerceiro.enable(false);
    _ocorrencia.CTeEmitidoNoEmbarcador.enable(arg.Data.SituacaoOcorrencia === EnumSituacaoOcorrencia.AgInformacoes);
    _ocorrencia.DTNatura.enable(false);
    _ocorrencia.Carga.enable(false);
    _ocorrencia.Chamado.enable(false);
    _ocorrencia.ComponenteFrete.enable(false);
    _ocorrencia.GrupoOcorrencia.enable(false);
    _ocorrencia.DataOcorrencia.enable(false);
    _ocorrencia.TipoOcorrencia.enable(false);
    _ocorrencia.TipoOcorrencia.porPeriodo = arg.Data.OcorrenciaPorPeriodo;
    _ocorrencia.TipoOcorrencia.periodo = arg.Data.Periodo;
    _ocorrencia.TipoOcorrencia.tipoEmissaoDocumentoOcorrencia = arg.Data.TipoEmissaoDocumentoOcorrencia;
    _ocorrencia.TiposCausadoresOcorrencia.visible(_ocorrencia.TiposCausadoresOcorrencia.codEntity() > 0);
    _ocorrencia.TiposCausadoresOcorrencia.enable(false);
    _ocorrencia.CausasTipoOcorrencia.visible(_ocorrencia.CausasTipoOcorrencia.codEntity() > 0);
    _ocorrencia.CausasTipoOcorrencia.enable(false);
    _ocorrencia.Observacao.enable(false);
    _ocorrencia.NotificarDebitosAtivos.enable(false);
    _ocorrencia.ValorOcorrencia.enable(false);
    _ocorrencia.QuantidadeAjudantes.enable(false);
    _ocorrencia.Chamados.enable(false);
    _ocorrencia.ValorTotalMoeda.enable(false);
    _ocorrencia.Tomador.enable(false);
    _ocorrencia.ObservacaoCTe.enable(false);
    _ocorrencia.TipoTomador.enable(false);
    _ocorrencia.IncluirICMSFrete.enable(false);
    _ocorrencia.CobrarOutroDocumento.enable(false);
    _ocorrencia.ModeloDocumentoFiscal.enable(false);
    _ocorrencia.NumeroOcorrenciaCliente.enable(false);
    _ocorrencia.PeriodoDezena.enable(false);
    _ocorrencia.PeriodoQuinzena.enable(false);
    _ocorrencia.PeriodoMes.enable(false);
    _ocorrencia.PeriodoAno.enable(false);
    _ocorrencia.Empresa.enable(false);
    _ocorrencia.Filial.enable(false);
    _ocorrencia.ContratoFreteTransportador.enable(false);
    _ocorrencia.BaseCalculoICMS.enable(false);
    _ocorrencia.AliquotaICMS.enable(false);
    _ocorrencia.ValorICMS.enable(false);
    _ocorrencia.CSTICMS.enable(false);
    _ocorrencia.Veiculo.enable(false);
    _ocorrencia.NomeRecebedor.enable(arg.Data.SituacaoOcorrencia == EnumSituacaoOcorrencia.AgInformacoes);
    _ocorrencia.TipoDocumentoRecebedor.enable(arg.Data.SituacaoOcorrencia == EnumSituacaoOcorrencia.AgInformacoes);
    _ocorrencia.NumeroDocumentoRecebedor.enable(arg.Data.SituacaoOcorrencia == EnumSituacaoOcorrencia.AgInformacoes);
    _ocorrencia.DataEvento.enable(false);
    _ocorrencia.UsuarioResponsavelAprovacao.enable(false);
    _ocorrencia.Motivo.visible(false);
    _ocorrencia.UtilizarSelecaoPorNotasFiscaisCTe.enable(false);

    _ocorrencia.QuantidadeMotorista.enable(false);
    _ocorrencia.QuantidadeDiasMotorista.enable(false);

    if (_ocorrencia.DTNatura.codEntity() > 0)
        _ocorrencia.DTNatura.visible(true);
    else
        _ocorrencia.DTNatura.visible(false);

    controlarCamposParametrosHabilitados(false);

    if (_ocorrencia.TipoTomador.val() == EnumTipoTomador.Outros) {
        _ocorrencia.Tomador.visible(true);
    }
    else {
        _ocorrencia.Tomador.visible(false);
    }

    if (_ocorrencia.TipoOcorrencia.NaoGerarDocumento && _ocorrencia.TipoOcorrencia.PermiteSelecionarTomador)
        _ocorrencia.Tomador.visible(true);

    if (_ocorrencia.SituacaoOcorrencia.val() == EnumSituacaoOcorrencia.AgInformacoes) {
        _ocorrencia.ComponenteFrete.enable(true);
        _ocorrencia.ComponenteFrete.required = true;
        _ocorrencia.ComponenteFrete.text(Localization.Resources.Ocorrencias.Ocorrencia.ComponenteDeFrete.getRequiredFieldDescription());
        _ocorrencia.GrupoOcorrencia.enable(true);
        _ocorrencia.GrupoOcorrencia.required = true;
        _ocorrencia.NumeroOcorrenciaCliente.enable(true);
        _ocorrencia.TipoOcorrencia.enable(true);
        _CRUDOcorrencia.ConfirmarOcorrencia.visible(true);

        controlarCamposParametrosHabilitados(true);

        if (_ocorrencia.ValorOcorrencia.val() == "0,00") {
            _ocorrencia.ValorOcorrencia.enable(true);
            _ocorrencia.Observacao.enable(true);
            _ocorrencia.ObservacaoCTe.enable(true);
        }

        if (_ocorrencia.OcorrenciaDeEstadia.val()) {
            _ocorrencia.ValorOcorrencia.enable(true);
            _ocorrencia.Observacao.enable(true);
            _ocorrencia.ObservacaoCTe.enable(true);
            _ocorrencia.CTeEmitidoNoEmbarcador.enable(true);
        }
    }

    _CRUDOcorrencia.Adicionar.visible(false);
    EditarAceiteDebito();
    preecherOcorrenciaAutorizacao(_ocorrencia.SolicitacaoCredito.val());

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.Terceiros
        || _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe) {
        ResumoAutorizacaoAprovacaoOcorrencia(arg.Data.ResumoAutorizacaoAprovacaoOcorrencia);
        ResumoAutorizacaoEmissaoOcorrencia(arg.Data.ResumoAutorizacaoEmissaoOcorrencia);
    }

    preecherResumoOcorrencia(arg.Data.OcorrenciaPorPeriodo);
    PreencherEtapaEmissaoDocumentoComplementar();
    recarregarGridListaImagens();

    if (_ocorrencia.Latitude.val() != "" && _ocorrencia.Longitude.val() != "" && _ocorrencia.Latitude.val() != null && _ocorrencia.Longitude.val() != null) {
        setarCoordenadasOcorrenciaPosicionamento();
        $("#tabPosicionamento").show();
    } else
        $("#tabPosicionamento").hide();

    $("#tabImagens").show();

    ExibeNotificarDebitosAtivos();
    setarEtapasOcorrencia();

    if (_ocorrencia.SituacaoOcorrencia.val() == EnumSituacaoOcorrencia.AgAprovacao || _ocorrencia.SituacaoOcorrencia.val() == EnumSituacaoOcorrencia.SemRegraAprovacao || _ocorrencia.SituacaoOcorrencia.val() == EnumSituacaoOcorrencia.SemRegraEmissao) {
        _ocorrencia.Anexo.enable(true);
        _ocorrencia.Anexo.visible(true);
    }

    if (_ocorrencia.SituacaoOcorrencia.val() == EnumSituacaoOcorrencia.AgInformacoes) {
        if (_ocorrencia.OcorrenciaDeEstadia.val() == true) {

            _ocorrencia.SelecionarTodos.visible(true);
            _ocorrencia.CTesComplementados.visible(false);
            _ocorrencia.DocumentosParaEmissaoNFSManualComplementados.visible(false);
            _ocorrencia.CTesParaComplemento.visible(true);
            _ocorrencia.CTesParaComplemento.visibleFade(true);
            //_ocorrencia.CTesComplementados.visibleFade(false);

            buscarCTesParaComplemento();
        }
        else {
            _ocorrencia.CTesComplementados.visible(false);
            _ocorrencia.DocumentosParaEmissaoNFSManualComplementados.visible(false);
            _ocorrencia.CTesParaComplemento.visible(true);

            buscarCTesParaComplemento();
            buscarCargasDocumentoParaEmissaoNFSManualParaComplemento();
        }
    }

    if (_ocorrencia.NaoLimparCarga.val())
        _CRUDOcorrencia.GerarNovaOcorrencia.visible(false);

    _CRUDOcorrencia.VoltarEtapaCadastro.visible(isPermitirVoltarParaEtapaCadastro());

    controlarExibicaoPeriodoOcorrencia();
    controlarExigenciaChamadoParaAbrirOcorrencia();

    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira === true &&
        _ocorrencia.SituacaoOcorrencia.val() == EnumSituacaoOcorrencia.Finalizada &&
        (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_AlterarMoeda, _PermissoesPersonalizadasOcorrencia) === true || _CONFIGURACAO_TMS.UsuarioAdministrador)) {
        _ocorrencia.AlterarMoeda.enable(true);
        _ocorrencia.AlterarMoeda.visible(true);
    } else {
        _ocorrencia.AlterarMoeda.enable(false);
        _ocorrencia.AlterarMoeda.visible(false);
    }

    if (_CONFIGURACAO_TMS.ExigirMotivoOcorrencia) {
        if (_ocorrencia.SituacaoOcorrencia.val() == EnumSituacaoOcorrencia.AgInformacoes)
            _ocorrencia.Motivo.enable(true);
        else
            _ocorrencia.Motivo.enable(false);

        _ocorrencia.Motivo.visible(true);
    }

    if (_ocorrencia.OcorrenciaReprovada) {
        OcorrenciaReprovadaBlur();
    }

    controlarCamposOcorrenciaObservacaoFiscoContribuinte(false);
    recarregarGridOcorrenciaObservacaoFiscoContribuinte();
}

function preencherDadosPeriodoOcorrencia(periodo) {
    if (periodo) {
        var dia = parseInt(periodo.substr(0, 2));
        var mes = parseInt(periodo.substr(3, 2));
        var ano = parseInt(periodo.substr(6, 4));

        _ocorrencia.PeriodoDezena.val(dia <= 10 ? EnumDezena.Primeira : (dia <= 20 ? EnumDezena.Segunda : EnumDezena.Terceira));
        _ocorrencia.PeriodoQuinzena.val(dia <= 15 ? EnumQuinzena.Primeira : EnumQuinzena.Segunda);
        _ocorrencia.PeriodoMes.val(mes);
        _ocorrencia.PeriodoAno.val(ano);
    }
    else {
        _ocorrencia.PeriodoDezena.val(EnumDezena.Primeira);
        _ocorrencia.PeriodoQuinzena.val(EnumQuinzena.Primeira);
        _ocorrencia.PeriodoMes.val(_ocorrencia.PeriodoMes.def);
        _ocorrencia.PeriodoAno.val(_ocorrencia.PeriodoAno.def);
    }
}

function limparCamposOcorrencia() {
    _ocorrencia.CTesParaComplemento.visibleFade(false);
    _ocorrencia.CargasParaComplemento.visibleFade(false);
    _ocorrencia.VeiculosImprodutivos.visibleFade(false);
    _ocorrencia.DocumentosAgrupadosDoVeiculo.visibleFade(false);
    _ocorrencia.VeiculosContrato.visibleFade(false);
    _ocorrencia.MotoristasContrato.visibleFade(false);
    _CRUDOcorrencia.GerarNovaOcorrencia.visible(false);
    _CRUDOcorrencia.ReabrirOcorrencia.visible(false);
    _CRUDOcorrencia.DownloadDocumentosOcorrencia.visible(false);
    _CRUDOcorrencia.Adicionar.visible(true);
    _CRUDOcorrencia.ReprocessarRegras.visible(false);
    _CRUDOcorrencia.VoltarEtapaCadastro.visible(false);
    _ocorrenciaAutorizacao.MensagemEtapaSemRegra.visible(false);
    _ocorrenciaAutorizacaoEmissao.MensagemEtapaSemRegra.visible(false);
    _ocorrencia.Chamado.enable(true);
    _ocorrencia.ContratoFreteTransportador.visible(false);
    _ocorrencia.Quantidade.visible(false);
    _ocorrencia.Quantidade.enable(true);
    _ocorrencia.CalcularValorOcorrencia.visible(false);
    _ocorrencia.QuantidadeAjudantes.visible(false);
    _ocorrencia.QuantidadeAjudantes.enable(true);

    if (!_CONFIGURACAO_TMS.ExigirChamadoParaAbrirOcorrencia) {
        _ocorrencia.Chamados.required(false);
        _ocorrencia.Chamados.text(Localization.Resources.Ocorrencias.Ocorrencia.Atendimentos.getFieldDescription());
    }

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.Terceiros || _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe)
        _ocorrencia.ComponenteFrete.enable(false);
    else
        _ocorrencia.ComponenteFrete.enable(true);

    _ocorrencia.CTeTerceiro.enable(true);
    _ocorrencia.CTeEmitidoNoEmbarcador.enable(true);
    _ocorrencia.DTNatura.visible(false);
    _ocorrencia.DTNatura.enable(true);
    _ocorrencia.NumeroOcorrenciaCliente.enable(true);
    _ocorrencia.DataOcorrencia.enable(_ocorrencia.DataOcorrencia.defEnable);
    _ocorrencia.Observacao.enable(true);
    _ocorrencia.Motivo.enable(true);
    _ocorrencia.NotificarDebitosAtivos.enable(true);
    _ocorrencia.ObservacaoCTe.enable(true);
    _ocorrencia.TipoTomador.enable(true);
    _ocorrencia.IncluirICMSFrete.enable(true);
    _ocorrencia.TipoOcorrencia.enable(true);
    _ocorrencia.TipoOcorrencia.porPeriodo = false;
    _ocorrencia.TipoOcorrencia.periodo = EnumPeriodoAcordoContratoFreteTransportador.NaoPossui;
    _ocorrencia.TipoOcorrencia.tipoEmissaoDocumentoOcorrencia = EnumTipoEmissaoDocumentoOcorrencia.Todos;
    _cteEmitidoEmbarcador = false;
    _ocorrencia.CobrarOutroDocumento.enable(true);
    _ocorrencia.ModeloDocumentoFiscal.enable(false);
    _ocorrencia.ModeloDocumentoFiscal.required = false;
    _ocorrencia.PeriodoDezena.enable(true);
    _ocorrencia.PeriodoQuinzena.enable(true);
    _ocorrencia.PeriodoMes.enable(true);
    _ocorrencia.PeriodoAno.enable(true);
    _ocorrencia.Empresa.enable(true);
    _ocorrencia.Filial.visible(false);
    _ocorrencia.Filial.enable(true);
    _ocorrencia.ContratoFreteTransportador.enable(true);
    _ocorrencia.ContratoFreteTransportador.periodoAcordo = EnumPeriodoAcordoContratoFreteTransportador.NaoPossui;
    _ocorrencia.ValorOcorrencia.enable(true);
    _ocorrencia.Chamados.enable(true);
    _ocorrencia.ValorTotalMoeda.enable(true);
    _ocorrencia.ValorICMS.enable(true);
    _ocorrencia.BaseCalculoICMS.enable(true);
    _ocorrencia.AliquotaICMS.enable(true);
    _ocorrencia.CSTICMS.enable(true);
    _ocorrencia.Veiculo.enable(true);
    _ocorrencia.NomeRecebedor.enable(true);
    _ocorrencia.TipoDocumentoRecebedor.enable(true);
    _ocorrencia.NumeroDocumentoRecebedor.enable(true);
    _ocorrencia.DataEvento.enable(true);
    _ocorrencia.QuantidadeMotorista.enable(true);
    _ocorrencia.QuantidadeDiasMotorista.enable(true);
    _ocorrencia.DescontarValoresOutrasCargas.val("");
    _ocorrencia.DescontarValoresOutrasCargas.enable(false);
    _ocorrencia.Veiculo.visible(false);
    _ocorrencia.Veiculo.required = false;
    _ocorrencia.UsuarioResponsavelAprovacao.enable(false);
    _ocorrencia.UsuarioResponsavelAprovacao.visible(false);
    _ocorrencia.UsuarioResponsavelAprovacao.required = false;
    _ocorrencia.DocumentosAgrupadosDoVeiculo.visible(false);
    _ocorrencia.VeiculosContrato.visible(false);
    _ocorrencia.MotoristasContrato.visible(false);
    _ocorrencia.Anexo.enable(true);
    _ocorrencia.ValorOcorrencia.visibleFade(false);
    _ocorrencia.UtilizarSelecaoPorNotasFiscaisCTe.enable(true);
    _ocorrencia.UtilizarSelecaoPorNotasFiscaisCTe.visible(false);
    _ocorrencia.TiposCausadoresOcorrencia.enable(true);
    _ocorrencia.CausasTipoOcorrencia.enable(true);
    _ocorrencia.GrupoOcorrencia.enable(true);
    _ocorrencia.GrupoOcorrencia.visible(false);
    _CRUDOcorrencia.ConfirmarOcorrencia.visible(false);

    _ocorrencia.CTesComplementados.visible(false);
    _ocorrencia.DocumentosParaEmissaoNFSManualComplementados.visible(false);
    _ocorrencia.CTesParaComplemento.visible(true);
    _ocorrencia.CTesParaComplemento.cssClass("col col-xs-12 col-sm-12 col-md-12 col-lg-12");
    ko.observable(Localization.Resources.Ocorrencias.Ocorrencia.ComponenteDeFrete.getFieldDescription());

    _ocorrencia.Tomador.visible(false);
    _ocorrencia.Tomador.enable(false);
    _ocorrencia.Tomador.required = false;

    LimparCampos(_ocorrencia);
    FluxoOcorrenciaPorPeriodo(false);
    limparOcorrenciaAutorizacao();
    limparOcorrenciaAnexos();
    LimparOcorrenciaAnexosVeiculoContrato();
    LimparCamposDocumentosComplementar();
    limparResumoOcorrencia();
    LimparControleGridCargasComplementadas();
    ExibeNotificarDebitosAtivos();

    limparCamposOcorrenciaImagem();
    recarregarGridListaImagens();
    $("#tabImagens").hide();
    $("#tabPosicionamento").hide();

    setarEtapaInicioOcorrencia();
    controlarExibicaoCamposParametros(0);
    controlarCamposParametrosHabilitados(true);

    controlarCamposOcorrenciaObservacaoFiscoContribuinte(true);
    recarregarGridOcorrenciaObservacaoFiscoContribuinte();

    _CTesImportadosParaComplemento = new Array();
    _tipoSelecaoOcorrenciaPorPeriodo = false;

    resetarTabs();
    ControlarCamposMoedaEstrangeira();
}

/*
 * Declaração das Funções Privadas
 */

function buscarCTesParaComplemento(callback) {
    var multiplaescolha = {
        basicGrid: null,
        eventos: function () { },
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _ocorrencia.SelecionarTodos,
        somenteLeitura: false,
        callbackSelecionado: function (e, registroSelecionado) {
            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS)
                calcularValorTabelaFrete(_ocorrencia, null, true);
            if (_ocorrencia.DefinirPeriodoEstadiaAutomaticamente.val()) {
                _ocorrencia.DataEntradaRaio.val(registroSelecionado.DataEntradaRaio);
                _ocorrencia.DataSaidaRaio.val(registroSelecionado.DataSaidaRaio);
                _ocorrencia.DataInicio.val(registroSelecionado.DataEntrada);
                _ocorrencia.DataFim.val("");
                _ocorrencia.TotalHoras.val("00:00");
            }

            CalcularValorComponenteExistente(this.selecionados);
        },
        callbackNaoSelecionado: function () {
            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiTMS)
                calcularValorTabelaFrete(_ocorrencia, null, true);
            if (_ocorrencia.DefinirPeriodoEstadiaAutomaticamente.val()) {
                _ocorrencia.DataEntradaRaio.val("");
                _ocorrencia.DataSaidaRaio.val("");
                _ocorrencia.DataInicio.val("");
                _ocorrencia.DataFim.val("");
                _ocorrencia.TotalHoras.val("00:00");
            }

            CalcularValorComponenteExistente(this.selecionados);
        }
    }

    var detalhesCTeVinculado = { descricao: Localization.Resources.Ocorrencias.Ocorrencia.DetalhesCtEVinculado, id: guid(), metodo: detalhesCTeVinculadoClick, icone: "", visibilidade: visibilidadeCTeVinculado };
    var excluirCTeVinculado = { descricao: Localization.Resources.Ocorrencias.Ocorrencia.ExcluirCtEVinculado, id: guid(), metodo: excluirCTeVinculadoClick, icone: "", visibilidade: visibilidadeCTeVinculado };
    var vincularCTe = { descricao: Localization.Resources.Ocorrencias.Ocorrencia.VincularCtE, id: guid(), metodo: vincularCTeClick, icone: "", visibilidade: visibilidadeOpcaoImportacao };
    var importarXMLCTe = { descricao: Localization.Resources.Ocorrencias.Ocorrencia.ImportarXMLCTe, id: guid(), metodo: importarXMLCTeComplementoClick, icone: "", visibilidade: visibilidadeOpcaoImportacao };
    var baixarDACTE = { descricao: Localization.Resources.Ocorrencias.Ocorrencia.BaixarDACTE, id: guid(), metodo: baixarDacteClick, icone: "", visibilidade: VisibilidadeOpcaoDownload };
    var baixarPDF = { descricao: Localization.Resources.Ocorrencias.Ocorrencia.BaixarPDF, id: guid(), metodo: baixarDacteClick, icone: "", visibilidade: VisibilidadeDownloadOutrosDoc };
    var baixarXML = { descricao: Localization.Resources.Ocorrencias.Ocorrencia.BaixarXML, id: guid(), metodo: baixarXMLCTeClick, icone: "", visibilidade: VisibilidadeOpcaoDownload };
    var notasComplementares = { descricao: Localization.Resources.Ocorrencias.Ocorrencia.NotasFiscaisParaComplemento, id: guid(), metodo: selecionarNotasComplementares, icone: "", visibilidade: VisibilidadeOpcaoNotasComplementares };
    var devolverProdutos = { descricao: Localization.Resources.Ocorrencias.Ocorrencia.SelecionarProdutos, id: guid(), metodo: devolverProdutosClick, icone: "", visibilidade: VisibilidadeSelecionarProdutos };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Ocorrencias.Ocorrencia.Opcoes, tamanho: 7, opcoes: [baixarDACTE, baixarXML, baixarPDF, importarXMLCTe, vincularCTe, excluirCTeVinculado, detalhesCTeVinculado, notasComplementares, devolverProdutos] };

    _gridCTe = new GridView(_ocorrencia.CTesParaComplemento.idGrid, "Ocorrencia/ConsultarCargaCTeOcorrencia", _ocorrencia, menuOpcoes, null, null, null, null, null, multiplaescolha, 25);
    _gridCTe.CarregarGrid(function () {
        if (_ocorrencia.DefinirPeriodoEstadiaAutomaticamente.val()) {
            _ocorrencia.SelecionarTodos.val(false);
            _ocorrencia.SelecionarTodos.visible(false);

            _gridCTe.SetarPermissaoSelecionarSomenteUmRegistro(true);
        }
        else {
            _gridCTe.SetarPermissaoSelecionarSomenteUmRegistro(false);

            if (configuracaoTipoOcorrencia != null && configuracaoTipoOcorrencia.TodosCTesSelecionados == true) {
                setTimeout(function () {
                    if (_ocorrencia.SelecionarTodos.val() == false)
                        _ocorrencia.SelecionarTodos.get$().click();
                    _ocorrencia.SelecionarTodos.visible(false);
                    _gridCTe.SetarRegistrosSomenteLeitura(true);
                }, 100);
            }
            else if (configuracaoTipoOcorrencia != null && configuracaoTipoOcorrencia.NaoPermiteSelecionarTodosCTes == true)
                _ocorrencia.SelecionarTodos.visible(false);
        }

        if (callback != null)
            callback();
    });
}

function buscarCargasDocumentoParaEmissaoNFSManualParaComplemento(callback) {

    var multiplaescolha = {
        basicGrid: null,
        eventos: function () { },
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _ocorrencia.SelecionarTodosDocumentoParaEmissaoNFSManual,
        somenteLeitura: false,
    }

    _gridDocumentosParaEmissaoNFSManual = new GridView(_ocorrencia.CargaDocumentoParaEmissaoNFSManualParaComplemento.idGrid, "CargaNFS/ConsultarCargaDocumentoParaEmissaoNFSManual", _ocorrencia, null, null, null, null, null, null, multiplaescolha);
    _gridDocumentosParaEmissaoNFSManual.CarregarGrid((dados) => {
        if (_gridDocumentosParaEmissaoNFSManual.NumeroRegistros() > 0) {
            $("#menuTabDocsParaNFSManual").show();
        } else {
            $("#menuTabDocsParaNFSManual").hide();
        }
    });

}

function editarOcorrenciaComTokenAcesso(token) {
    limparCamposOcorrencia();
    _ocorrencia.Codigo.val(token);
    configuracaoTipoOcorrencia = null;
    buscarOcorrenciaPorCodigo();
}

function buscarOcorrenciaPorCodigo(callback) {
    callback = callback || function () { };

    BuscarPorCodigo(_ocorrencia, "Ocorrencia/BuscarPorCodigo", function (arg) {
        if (arg.Data.TipoOcorrencia.Codigo > 0) {
            var tipoOcorrenciaPromise = CarregaTipoOcorrenciaGrid(arg.Data.TipoOcorrencia);
            tipoOcorrenciaPromise.then(function () {
                preecherCamposEdicaoOcorrencia(arg.Data.OrigemOcorrencia, arg);
            });
        } else {
            preecherCamposEdicaoOcorrencia(arg.Data.OrigemOcorrencia, arg);
        }

        finalizarRequisicao();

        switch (arg.Data.OrigemOcorrencia) {
            case EnumOrigemOcorrencia.PorCarga:
                _gridCTeComplementado.CarregarGrid(callback);
                _gridDocumentosParaEmissaoNFSManualComplementado.CarregarGrid(callback);
                break;
            case EnumOrigemOcorrencia.PorPeriodo:
                callback();
                break;
            case EnumOrigemOcorrencia.PorContrato:
                callback();
                break;
        }

        _ocorrencia.OcorrenciaReprovada = arg.Data.OcorrenciaReprovada;

        CarregarIntegracoes(arg.Data);
    }, function (arg) {
        exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        limparCamposOcorrencia();
    });
}

function buscarOcorrencias() {
    if (_CONFIGURACAO_TMS.PermiteDownloadCompactadoArquivoOcorrencia) {
        _pesquisaOcorrencia.DownloadAnexosLote.visible(false);
        _pesquisaOcorrencia.DownloadPDFsOcorrencia.visible(true);
        loadGridOcorrenciasComOpcoes();
    }
    else
        loadGridOcorrencias();
}

function loadGridOcorrencias() {
    var editar = { descricao: Localization.Resources.Ocorrencias.Ocorrencia.Editar, id: "clasEditar", evento: "onclick", metodo: editarOcorrencia, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    var configExportacao = {
        url: "Ocorrencia/ExportarPesquisa",
        titulo: Localization.Resources.Ocorrencias.Ocorrencia.DescricaoOcorrencias
    };

    _gridViewOcorrencia = new GridView("grid-pesquisa-ocorrencias", "Ocorrencia/Pesquisa", _pesquisaOcorrencia, menuOpcoes, null, null, function () {
        _pesquisaOcorrencia.Codigo.val(_pesquisaOcorrencia.Codigo.def);
    }, null, null, null, null, null, configExportacao);
    _gridViewOcorrencia.SetPermitirEdicaoColunas(true);
    _gridViewOcorrencia.SetSalvarPreferenciasGrid(true);
    _gridViewOcorrencia.CarregarGrid();
}

function loadGridOcorrenciasComOpcoes() {
    _pesquisaOcorrencia.SelecionarTodos.visible(true);

    var editar = { descricao: Localization.Resources.Ocorrencias.Ocorrencia.Editar, id: guid(), metodo: editarOcorrencia, icone: "" };
    var baixarPDF = { descricao: Localization.Resources.Ocorrencias.Ocorrencia.BaixarPDF, id: guid(), metodo: baixarPDFOcorrencia, icone: "" };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Ocorrencias.Ocorrencia.Opcoes, tamanho: 10, opcoes: [editar, baixarPDF] };

    var configExportacao = {
        url: "Ocorrencia/ExportarPesquisa",
        titulo: Localization.Resources.Ocorrencias.Ocorrencia.DescricaoOcorrencias
    };

    var multiplaescolha = {
        basicGrid: null,
        eventos: function () { },
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _pesquisaOcorrencia.SelecionarTodos,
        somenteLeitura: false
    };

    _gridViewOcorrencia = new GridView("grid-pesquisa-ocorrencias", "Ocorrencia/Pesquisa", _pesquisaOcorrencia, menuOpcoes, null, null, function () {
        _pesquisaOcorrencia.Codigo.val(_pesquisaOcorrencia.Codigo.def);
    }, null, null, multiplaescolha, null, null, configExportacao);
    _gridViewOcorrencia.SetPermitirEdicaoColunas(true);
    _gridViewOcorrencia.SetSalvarPreferenciasGrid(true);
    _gridViewOcorrencia.CarregarGrid();
}

function baixarPDFOcorrencia(ocorrenciaGrid) {
    executarDownload("Ocorrencia/DownloadPDFOcorrencia", { Codigo: ocorrenciaGrid.Codigo });
}

function calcularValorTabelaFrete(e, sender, exibirAlertaErro, dadosCTe, naoVerificarCodigo) {
    if (_ocorrencia.TipoOcorrencia.NaoCalcularValorOcorrenciaAutomaticamente)
        return;

    if (!naoVerificarCodigo && _ocorrencia.Codigo.val() > 0)
        return;

    calcularTotalHoras();

    if (!naoVerificarCodigo && !_ocorrencia.TipoOcorrencia.enable() && _ocorrencia.Chamado.val() <= 0)
        return;

    if (!e.ValorOcorrencia.enable() && _tipoSelecaoOcorrenciaPorPeriodo) {
        if (e.CodigoParametroPeriodo.val() == 9 && e.DataInicio.val() == "" && e.DataFim.val() == "") {
            e.ValorOcorrencia.val("0,00");
            return;
        }

        if (e.CodigoParametroPeriodo.val() == 0 && e.ParametroData1.val() == "") {
            e.ValorOcorrencia.val("0,00");
            return;
        }

        e.ValorOcorrencia.val("0,00");
        e.ValorOcorrenciaDestino.val("0,00");
    }

    if (e.Carga.codEntity() == "0")
        return;

    if (!_tipoSelecaoOcorrenciaPorPeriodo) {
        _ocorrencia.CargaCTes.list = new Array();

        var ctesSelecionados;

        if (_gridCTe && _gridCTe.GridViewTable() != undefined) {
            if (e.SituacaoOcorrencia.val() == EnumSituacaoOcorrencia.AgInformacoes && !_ocorrencia.OcorrenciaDeEstadia.val())
                ctesSelecionados = _gridCTe.GridViewTable().data();
            else if (_ocorrencia.SelecionarTodos.val())
                ctesSelecionados = _gridCTe.ObterMultiplosNaoSelecionados();
            else
                ctesSelecionados = _gridCTe.ObterMultiplosSelecionados();

            if (ctesSelecionados.length == 0 && !_ocorrencia.SelecionarTodos.val())
                ctesSelecionados = _gridCTe.GridViewTable().data();
        } else if (dadosCTe) {
            ctesSelecionados = dadosCTe;
        }

        if (ctesSelecionados != undefined) {
            if (ctesSelecionados && (ctesSelecionados.length > 0 || _ocorrencia.SelecionarTodos.val())) {
                $.each(ctesSelecionados, function (i, cte) {
                    var map = new CargaCTeMap();
                    map.Codigo.val = cte.DT_RowId;
                    map.CodigoCargaCTe.val = cte.CodigoCargaCTe;
                    _ocorrencia.CargaCTes.list.push(map);
                });
            }
            else if (!_ocorrencia.ValorOcorrencia.enable()) {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Ocorrencias.Ocorrencia.EObrigatorioInformarPeloMenosUmCtEParaGerarAOcorrencia);
                return;
            }
            else if (_ocorrencia.ValorOcorrencia.val() == "0,00" && _ocorrencia.Carga.codEntity() > 0) {
                if (exibirAlertaErro)
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Ocorrencias.Ocorrencia.ENecessarioInformarUmValorParaAOcorrencia);

                return;
            }
        }
    }

    var listaCTe = RetornarObjetoPesquisa(_ocorrencia).CargaCTes;
    var listaCargasDocumentoParaEmissaoNFSManual = RetornarObjetoPesquisa(_ocorrencia).CargasDocumentoParaEmissaoNFSManual;

    var data = {
        Carga: e.Carga.codEntity(),
        CodigoParametroInteiro: e.CodigoParametroInteiro.val(),
        ParametroInteiro: e.ParametroInteiro.val(),
        CodigoParametroPeriodo: e.CodigoParametroPeriodo.val(),
        DataInicio: e.DataInicio.val(),
        DataFim: e.DataFim.val(),
        QuantidadeAjudantes: e.QuantidadeAjudantes.val(),
        CodigoParametroBooleano: e.CodigoParametroBooleano.val(),
        ApenasReboque: $("#" + e.ApenasReboque.id).prop("checked"),
        ValorOcorrencia: e.ValorOcorrencia.val(),
        CodigoParametroData1: e.CodigoParametroData1.val(),
        ParametroData1: e.ParametroData1.val(),
        CodigoParametroData2: e.CodigoParametroData2.val(),
        ParametroData2: e.ParametroData2.val(),
        CodigoParametroTexto: e.CodigoParametroTexto.val(),
        ParametroTexto: e.ParametroTexto.val(),
        PermiteInformarValor: e.ValorOcorrencia.enable(),
        TipoOcorrencia: e.TipoOcorrencia.codEntity(),
        SelecionarTodos: _ocorrencia.SelecionarTodos.val(),
        SelecionarTodosDocumentoParaEmissaoNFSManual: _ocorrencia.SelecionarTodosDocumentoParaEmissaoNFSManual.val(),
        CargaCTes: listaCTe,
        CargasDocumentoParaEmissaoNFSManual: listaCargasDocumentoParaEmissaoNFSManual,
        Quilometragem: _ocorrencia.Quilometragem.val()
    };

    executarReST("Ocorrencia/CalcularValorOcorrencia", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                if (arg.Data == null)
                    return;

                _semTabela = false;

                if (parseFloat(arg.Data.ValorOcorrencia) > 0) {
                    _ocorrencia.ValorOcorrencia.val(arg.Data.ValorOcorrencia);
                }
                else {
                    _ocorrencia.ValorOcorrencia.val("0,00");
                }

                _ocorrencia.PercentualAcrescimoValor.val(arg.Data.PercentualAcrescimoValor)
                _ocorrencia.ObservacaoCTe.val(arg.Data.ObservacaoCTe);
                _ocorrencia.IncluirICMSFrete.val(arg.Data.IncluirICMSFrete);
                _ocorrencia.HorasOcorrencia.val(arg.Data.HorasOcorrencia);
                _ocorrencia.TotalHoras.val(arg.Data.TotalHoras);
                _ocorrencia.ObservacaoOcorrencia.val(arg.Data.ObservacaoOcorrencia);
                calcularTotalHoras();

                if (arg.Data.DividirOcorrencia == true) {
                    _ocorrencia.DividirOcorrencia.val(arg.Data.DividirOcorrencia);
                    _ocorrencia.ValorOcorrenciaDestino.val(arg.Data.ValorOcorrenciaDestino);
                    _ocorrencia.ObservacaoOcorrenciaDestino.val(arg.Data.ObservacaoOcorrenciaDestino);
                    _ocorrencia.ObservacaoCTeDestino.val(arg.Data.ObservacaoCTeDestino);
                    _ocorrencia.HorasOcorrenciaDestino.val(arg.Data.HorasOcorrenciaDestino);
                    _ocorrencia.ObservacaoOcorrencia.visible(true);

                    if (_ocorrencia.ValorOcorrenciaDestino.val() != "0,00") {
                        _ocorrencia.ValorOcorrenciaDestino.visible(true);
                        _ocorrencia.ObservacaoOcorrenciaDestino.visible(true);
                    }
                    else {
                        _ocorrencia.ValorOcorrenciaDestino.visible(false);
                        _ocorrencia.ObservacaoOcorrenciaDestino.visible(false);
                    }
                }
                else {
                    _ocorrencia.ObservacaoOcorrencia.val("");
                    _ocorrencia.ObservacaoOcorrencia.visible(false);
                    _ocorrencia.ValorOcorrenciaDestino.val("0,00");
                    _ocorrencia.ValorOcorrenciaDestino.visible(false);
                    _ocorrencia.ObservacaoOcorrenciaDestino.val("");
                    _ocorrencia.ObservacaoOcorrenciaDestino.visible(false);
                }
            }
            else {
                _semTabela = true;
                if (exibirAlertaErro)
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        }
        else {
            _semTabela = true;

            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function calcularValorTipoOcorrencia(gerarAviso) {
    if (!_ocorrencia.TipoOcorrencia.enable())
        return;

    if (typeof gerarAviso == "undefined")
        gerarAviso = true;

    var data = {
        TipoOcorrencia: _ocorrencia.TipoOcorrencia.codEntity(),
        PeriodoInicio: obterPeriodoInicio(),
        PeriodoFim: obterPeriodoFim(),
        CargasComplementadasDias: GetControleGridCargasComplementadas()
    };

    executarReST("Ocorrencia/CalcularValorTipoOcorrencia", data, function (arg) {
        if (arg.Success) {
            if (arg.Data)
                _ocorrencia.ValorOcorrencia.val(arg.Data.ValorOcorrencia);
            else if (gerarAviso)
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
    });
}

function controlarValorPeriodoInicio(periodoInicio) {
    if (periodoInicio === undefined)
        return obterPeriodoInicio();
    else
        preencherDadosPeriodoOcorrencia(periodoInicio);
}

function CriarOcorrencia(sender) {
    // feito evitar duplicaçao da chamada, problema que certamente deve ocorrer muito poucas vezes
    if (isProcessing) {
        console.log("Requisição em andamento, ignorando nova chamada.");
        return;
    }
    isProcessing = true;

    var geradoPelaInfracao = _ocorrencia.Infracao.codEntity() > 0;
    iniciarRequisicao();
    Salvar(_ocorrencia, "Ocorrencia/Adicionar", function (arg) {
        if (arg.Success) {
            isProcessing = false;

            if (arg.Data != false) {
                preencherRetornoOcorrencia(arg);

                if (geradoPelaInfracao)
                    Global.fecharModal("divModalOcorrencia");

                if (arg.Data.RecarregarDados) {
                    limparCamposOcorrencia();
                    _ocorrencia.Codigo.val(arg.Data.Codigo)
                    buscarOcorrenciaPorCodigo();
                }

            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
                finalizarRequisicao();
            }
            //AtualizarDadosControleSaldo();
        }
        else {
            isProcessing = false;

            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            finalizarRequisicao();
        }
    }, sender, function () {
        isProcessing = false;
        finalizarRequisicao();
        exibirCamposObrigatorioOcorrencia();
    });
}

function editarOcorrencia(ocorrenciaGrid) {
    limparCamposOcorrencia();
    _ocorrencia.Codigo.val(ocorrenciaGrid.Codigo);
    configuracaoTipoOcorrencia = null;
    buscarOcorrenciaPorCodigo();
}

function isPermitirVoltarParaEtapaCadastro() {
    return false;//**REVER** Botão removido pois ao retornar ocorrencia rejeitada permitia lançar em duplicidade (Marfrig chamado 7950)
    return (
        EnumSituacaoOcorrencia.isPermiteVoltarParaEtapaCadastro(_ocorrencia.SituacaoOcorrencia.val()) &&
        VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Ocorrencia_PermitirRetornarEtapaCadastro, _PermissoesPersonalizadasOcorrencia)
    );
}

function OcorrenciaPossuiAnexos() {
    if (arguments.length > 0) return true;

    var possuiAnexosCarga = true;
    var possuiAnexosContrato = true;
    var possuiAnexosOcorrencia = true;
    var possuiAnexosOcorrenciaDestinadaFranquias = true;

    if (_anexosOcorrencia.Anexos.val().length == 0 && _ocorrencia.TipoOcorrencia.origemOcorrencia == EnumOrigemOcorrencia.PorCarga)
        possuiAnexosOcorrencia = false;

    if (_ocorrencia.TipoOcorrencia.origemOcorrencia == EnumOrigemOcorrencia.PorPeriodo) {
        if (configuracaoTipoOcorrencia != null && configuracaoTipoOcorrencia.OcorrenciaDestinadaFranquias) {
            if (_anexosOcorrencia.Anexos.val().length == 0)
                possuiAnexosOcorrenciaDestinadaFranquias = false;
        }
        else if (_anexosCarga.Anexos.val().length == 0 && _anexosOcorrencia.Anexos.val().length == 0) //Adicionado por solicitação da Marfrig, quando for por período não exigir anexo na Carga se foi informado na ocorrência
            possuiAnexosCarga = false;
    }

    if (_ocorrencia.TipoOcorrencia.origemOcorrencia == EnumOrigemOcorrencia.PorContrato && _ocorrencia.Codigo.val() == 0) {
        var codigosVeiculosComAnexo = [];
        var codigosVeiculos = [];

        GetAnexosVeiculoContratos(true).map(function (veic) {
            if ($.inArray(veic.CodigoVeiculo, codigosVeiculosComAnexo) == -1)
                codigosVeiculosComAnexo.push(veic.CodigoVeiculo);
        });

        if (_gridVeiculosContrato && _gridVeiculosContrato.BuscarRegistros) {
            codigosVeiculos = _gridVeiculosContrato.BuscarRegistros().map(function (veiculo) {
                return veiculo.CodigoVeiculo;
            });
        }

        var codigosSemAnexos = codigosVeiculos.filter(function (veiculo) {
            return $.inArray(veiculo, codigosVeiculosComAnexo) == -1;
        });

        if ((codigosSemAnexos.length > 0) || (codigosSemAnexos.length == 0 && codigosVeiculosComAnexo.length == 0 && _anexosOcorrencia.Anexos.val().length == 0))
            possuiAnexosContrato = false;

        if (_ocorrencia.QuantidadeMotorista.val() == "" || _ocorrencia.QuantidadeMotorista.val() == 0)
            possuiAnexosOcorrencia = true;
    }

    return possuiAnexosOcorrencia && possuiAnexosCarga && possuiAnexosContrato && possuiAnexosOcorrenciaDestinadaFranquias;
}

function obterDataPeriodoFormatada(dia) {
    var ano = _ocorrencia.PeriodoAno.val();
    var mes = _ocorrencia.PeriodoMes.val() < 10 ? "0" + _ocorrencia.PeriodoMes.val() : _ocorrencia.PeriodoMes.val();

    return dia + "/" + mes + "/" + ano;
}

function preencherRetornoOcorrencia(arg) {
    if (_gridViewOcorrencia != undefined)
        _gridViewOcorrencia.CarregarGrid();
    _ocorrencia.NumeroOcorrencia.val(arg.Data.NumeroOcorrencia);
    _ocorrencia.NumeroOcorrenciaCliente.val(arg.Data.NumeroOcorrenciaCliente);
    _ocorrencia.Codigo.val(arg.Data.Codigo);
    _ocorrencia.CargaOcorrenciaVinculada.val(arg.Data.CargaOcorrenciaVinculada);
    _ocorrencia.SituacaoOcorrencia.val(arg.Data.SituacaoOcorrencia);
    if (arg.Data.SituacaoOcorrencia == EnumSituacaoOcorrencia.Finalizada) {
        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Ocorrencias.Ocorrencia.CadastradaComSucesso);
    }

    var dataSituacao = arg.Data.SituacaoOcorrencia;
    if (dataSituacao == EnumSituacaoOcorrencia.AgAprovacao || dataSituacao == EnumSituacaoOcorrencia.SemRegraAprovacao) {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Ocorrencias.Ocorrencia.OcorrenciaRegistradaComSucessoPoremEsta);
    }

    if (arg.Data.SituacaoOcorrencia == EnumSituacaoOcorrencia.EmEmissaoCTeComplementar) {
        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Ocorrencias.Ocorrencia.AguardeAEmissaoDosCtEsComplementaresParaFinalizacaoDaOcorrencia);
    }

    _ocorrencia.SolicitacaoCredito.val(arg.Data.SolicitacaoCredito);

    var _GridCarregada = function () {
        arg.Data.Anexos = _anexosOcorrencia.Anexos.val();
        PreencherObjetoKnout(_ocorrencia, arg);
        preecherCamposEdicaoOcorrencia(arg.Data.OrigemOcorrencia, arg);
        finalizarRequisicao();
    };

    _ocorrencia.TipoOcorrencia.origemOcorrencia = arg.Data.OrigemOcorrencia;
    switch (_ocorrencia.TipoOcorrencia.origemOcorrencia) {
        case EnumOrigemOcorrencia.PorCarga:
            EnviarArquivosAnexadosOcorrencia(arg.Data.HashAnexos);
            _gridCTeComplementado.CarregarGrid(_GridCarregada);
            _gridDocumentosParaEmissaoNFSManualComplementado.CarregarGrid(_GridCarregada);

            break;
        case EnumOrigemOcorrencia.PorPeriodo:
            EnviarArquivosAnexadosOcorrencia(arg.Data.HashAnexos);
            EnviarArquivosAnexadosCarga(arg.Data.HashAnexos);
            FluxoOcorrenciaPorPeriodo(true);
            BuscarCargasPorPeriodo(_GridCarregada);

            break;
        case EnumOrigemOcorrencia.PorContrato:
            EnviarArquivosAnexadosOcorrencia(arg.Data.HashAnexos);
            EnviarArquivosAnexadosVeiculoContrato(arg.Data.HashAnexos);
            FluxoOcorrenciaPorPeriodo(true);
            BuscarVeiculosContrato(_GridCarregada);

            break;
    }

    _CRUDOcorrencia.VoltarEtapaCadastro.visible(isPermitirVoltarParaEtapaCadastro());
}

function resetarTabs() {
    $("#myTab a:first").tab("show");
}

function retornoCarga(data, callback) {
    executarReST("Ocorrencia/ObterDetalhesCarga", { Carga: data.Codigo }, function (r) {
        if (r.Success) {
            if (r.Data) {
                _ocorrencia.Moeda.val(r.Data.Moeda);
                _ocorrencia.ValorCotacaoMoeda.val(Globalize.format(r.Data.ValorCotacaoMoeda, "n10"));
                _ocorrencia.ValorTotalMoeda.val("0,00");

                _ocorrencia.DTNatura.val("");
                _ocorrencia.DTNatura.entityDescription("");
                _ocorrencia.DTNatura.codEntity(0);

                _ocorrencia.Carga.codEntity(data.Codigo);
                _ocorrencia.Carga.entityDescription(data.CodigoCargaEmbarcador);
                _ocorrencia.Carga.val(data.CodigoCargaEmbarcador);

                buscarCTesParaComplemento(function () {
                    verificarSeTomadorEmiteCTe(function () {
                        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiCTe) {
                            _ocorrencia.SelecionarTodos.visible(!_InduzirTransportadorSelecionarApenasUmComplementoSolicitacaoComplementos);
                        }
                        if (callback != null)
                            callback();
                    });
                });

                controlarExibicaoCamposParametros(_ocorrencia.TipoOcorrencia.codEntity());

                ControlarCamposMoedaEstrangeira();

                if ((_ocorrencia.TipoOcorrencia.OcorrenciaComplementoValorFreteCarga == null || _ocorrencia.TipoOcorrencia.OcorrenciaComplementoValorFreteCarga === false)) {

                    if (r.Data !== null && r.Data.Integracoes !== null && r.Data.Integracoes.some(function (item) { return item === EnumTipoIntegracao.Natura; }))
                        _ocorrencia.DTNatura.visible(true);
                    else
                        _ocorrencia.DTNatura.visible(false);

                    _ocorrencia.CTesParaComplemento.visibleFade(true);

                    buscarCargasDocumentoParaEmissaoNFSManualParaComplemento();
                } else {
                    _ocorrencia.CTesParaComplemento.visibleFade(false);
                }
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function ControlarCamposMoedaEstrangeira() {
    if (_ocorrencia.Moeda.val() !== EnumMoedaCotacaoBancoCentral.Real) {
        _ocorrencia.Moeda.visible(true);
        _ocorrencia.ValorCotacaoMoeda.visible(true);
        _ocorrencia.ValorTotalMoeda.visible(true);
        _ocorrencia.ValorTotalMoeda.enable(true);
    } else {
        _ocorrencia.Moeda.visible(false);
        _ocorrencia.ValorCotacaoMoeda.visible(false);
        _ocorrencia.ValorTotalMoeda.visible(false);
        _ocorrencia.ValorTotalMoeda.enable(false);
    }
}

function retornoChamado(chamado) {

    _ocorrencia.Chamado.codEntity(chamado.Codigo);
    _ocorrencia.Chamado.val(chamado.Numero);
    _ocorrencia.Chamado.entityDescription(chamado.Numero);

    executarReST("ChamadoOcorrencia/ObterTipoOcorrencia", { Chamado: _ocorrencia.Chamado.codEntity() }, function (r) {

        if (r.Data.Codigo > 0) {
            _ocorrencia.TipoOcorrencia.codEntity(r.Data.Codigo);
            _ocorrencia.TipoOcorrencia.val(r.Data.Descricao);
        }

        controlarExibicaoCamposParametros(_ocorrencia.TipoOcorrencia.codEntity());
        _ocorrencia.Carga.codEntity(chamado.CodigoCarga);
        _ocorrencia.Carga.val(chamado.Carga);
        _ocorrencia.Carga.entityDescription(chamado.Carga);
        _ocorrencia.CTesParaComplemento.visibleFade(true);

        buscarCTesParaComplemento(function () {
            verificarSeTomadorEmiteCTe(function () {
                //if (_ocorrencia.CTeEmitidoNoEmbarcador.val() && _ocorrencia.ComponenteFrete.codEntity() > 0) {
                //    transformarGridImportarCTe();
                //} else {
                //    transformarGridEmissaoCTe()
                //}
            });
        });

        buscarCargasDocumentoParaEmissaoNFSManualParaComplemento();

        if (_ocorrencia.TipoOcorrencia.codEntity() > 0 && _CONFIGURACAO_TMS.BloquearCamposOcorrenciaImportadosDoAtendimento)
            _ocorrencia.TipoOcorrencia.enable(false);
        else
            _ocorrencia.TipoOcorrencia.enable(true);

        if (r.Data.ComponenteCodigo > 0) {
            _ocorrencia.ComponenteFrete.codEntity(r.Data.ComponenteCodigo);
            _ocorrencia.ComponenteFrete.val(r.Data.ComponenteDescricao);
            if (_CONFIGURACAO_TMS.BloquearCamposOcorrenciaImportadosDoAtendimento)
                _ocorrencia.ComponenteFrete.enable(false);
            else
                _ocorrencia.ComponenteFrete.enable(true);
        }

        if (!string.IsNullOrWhiteSpace(chamado.Observacao)) {
            _ocorrencia.Observacao.val(chamado.Observacao);
            _ocorrencia.ObservacaoCTe.val(chamado.Observacao);
            if (_CONFIGURACAO_TMS.BloquearCamposOcorrenciaImportadosDoAtendimento) {
                _ocorrencia.Observacao.enable(false);
                _ocorrencia.ObservacaoCTe.enable(false);
            }
            else {
                _ocorrencia.Observacao.enable(true);
                _ocorrencia.ObservacaoCTe.enable(true);
            }
        }
        else {
            _ocorrencia.Observacao.enable(true);
            _ocorrencia.ObservacaoCTe.enable(true);
        }

        var valor = Globalize.parseFloat(chamado.Valor);
        if (valor > 0 && _ocorrencia.ValorOcorrencia.enable) {
            _ocorrencia.ValorOcorrencia.val(chamado.Valor);
            if (_CONFIGURACAO_TMS.BloquearCamposOcorrenciaImportadosDoAtendimento)
                _ocorrencia.ValorOcorrencia.enable(false);
            else
                _ocorrencia.ValorOcorrencia.enable(true);
        }
    });
}

function retornoComponenteFrete(data) {
    _ocorrencia.ComponenteFrete.codEntity(data.Codigo);
    _ocorrencia.ComponenteFrete.val(data.Descricao);
    _ocorrencia.ComponenteFrete.entityDescription(data.Descricao);
    _ocorrencia.ComponenteFrete.TipoComponenteFrete = data.TipoComponenteFrete;
    visiblidadeValorOcorrencia();
    visibilidadeUtilizarSelecaoPorNotasFiscaisCTe();
}
function retornoGrupoOcorrencia(data) {
    _ocorrencia.GrupoOcorrencia.codEntity(data.Codigo);
    _ocorrencia.GrupoOcorrencia.val(data.Descricao);
    _ocorrencia.GrupoOcorrencia.entityDescription(data.Descricao);
}

function RetornoConsultaDTNatura(data) {
    _ocorrencia.DTNatura.codEntity(data.Codigo);
    _ocorrencia.DTNatura.val(data.Numero);
    _ocorrencia.DTNatura.entityDescription(data.Numero);

    _ocorrencia.ValorOcorrencia.val(data.ValorFrete);

    _ocorrencia.SelecionarTodos.visible(false);
    _ocorrencia.SelecionarTodos.val(false);
    _ocorrencia.CTesParaComplemento.text(Localization.Resources.Ocorrencias.Ocorrencia.OsCtEsSeraoSelecionadosAutomaticamenteAPartirDoDt);
    _gridCTe.SetarRegistrosSomenteLeitura(true);
    _gridCTe.CarregarGrid();
}

function RetornoConsultaTomador(tomador) {
    _ocorrencia.Tomador.val(tomador.CPF_CNPJ + " - " + tomador.Nome);
    _ocorrencia.Tomador.entityDescription(tomador.CPF_CNPJ + " - " + tomador.Nome);
    _ocorrencia.Tomador.codEntity(tomador.Codigo);

    verificarSeTomadorEmiteCTe(function () {
        if (_ocorrencia.CTeEmitidoNoEmbarcador.val() && _ocorrencia.ComponenteFrete.codEntity() > 0) {
            transformarGridImportarCTe();
        } else {
            transformarGridEmissaoCTe();
        }
    });
}

function retornoConsultaVeiculo(data) {
    _ocorrencia.Veiculo.val(data.Placa);
    _ocorrencia.Veiculo.entityDescription(data.Placa);
    _ocorrencia.Veiculo.codEntity(data.Codigo);

    veiculoBlur();
}

function retornoModeloFiscal(data) {
    _ocorrencia.DadosModeloDocumentoFiscal.val(data);
    _ocorrencia.ModeloDocumentoFiscal.val(data.Descricao);
    _ocorrencia.ModeloDocumentoFiscal.entityDescription(data.Descricao);
    _ocorrencia.ModeloDocumentoFiscal.codEntity(data.Codigo);
}

function retornoTransportador(data) {
    _ocorrencia.Empresa.val(data.Descricao);
    _ocorrencia.Empresa.entityDescription(data.Descricao);
    _ocorrencia.Empresa.codEntity(data.Codigo);
}

function verificarSeTomadorEmiteCTe(callback) {
    if (_ocorrencia.TipoTomador.val() == EnumTipoTomador.Outros && _ocorrencia.Tomador.codEntity() <= 0) {
        _ocorrencia.Tomador.visible(true);
        _ocorrencia.Tomador.enable(true);
        _ocorrencia.Tomador.required = true;
        return;
    } else if (_ocorrencia.TipoOcorrencia.NaoGerarDocumento && _ocorrencia.TipoOcorrencia.PermiteSelecionarTomador) {
        _ocorrencia.Tomador.visible(true);
        _ocorrencia.Tomador.enable(true);
        _ocorrencia.Tomador.required = true;
        return;
    }
    else if (_ocorrencia.TipoTomador.val() != EnumTipoTomador.Outros) {
        LimparCampoEntity(_ocorrencia.Tomador);
        _ocorrencia.Tomador.visible(false);
        _ocorrencia.Tomador.enable(false);
        _ocorrencia.Tomador.required = false;
    }

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS && _ocorrencia.Carga.codEntity() > 0) {
        var data = { Carga: _ocorrencia.Carga.codEntity(), TipoTomador: _ocorrencia.TipoTomador.val(), Tomador: _ocorrencia.Tomador.codEntity(), Conhecimento: _ocorrencia.Conhecimento.codEntity() }
        executarReST("Ocorrencia/VerificarTomadorOcorrenciaEmiteCTeNoEmbarcador", data, function (arg) {
            if (arg.Success) {

                if (configuracaoTipoOcorrencia == null || configuracaoTipoOcorrencia.ModeloDocumentoFiscal == null)
                    _ocorrencia.CTeEmitidoNoEmbarcador.val(arg.Data.CTeEmitidoNoEmbarcador);

                if (arg.Data.CTeEmitidoNoEmbarcador) {
                    _cteEmitidoEmbarcador = true;
                    if (arg.Data.ValorOcorrencia > 0)
                        _ocorrencia.ValorOcorrencia.val(arg.Data.ValorOcorrencia);
                    _ocorrencia.ObservacaoCTe.val(arg.Data.Observacao);
                    _CTesImportadosParaComplemento = arg.Data.CTesImportados;
                } else {
                    _cteEmitidoEmbarcador = false;
                    if (arg.Data.ValorOcorrencia > 0)
                        _ocorrencia.ValorOcorrencia.val(arg.Data.ValorOcorrencia);
                    _ocorrencia.ObservacaoCTe.val(arg.Data.Observacao);
                    _CTesImportadosParaComplemento = new Array();
                }
                if (callback != null)
                    callback();
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
                if (callback != null)
                    callback();
            }
        });
    } else {
        _ocorrencia.CTeEmitidoNoEmbarcador.val(false);
        callback();
    }
}

function visibilidadeCamposOcorrencia() {
    _ocorrencia.CTesComplementados.visible(true);
    _ocorrencia.DocumentosParaEmissaoNFSManualComplementados.visible(true);
    _ocorrencia.CTesParaComplemento.visible(false);
    visiblidadeValorOcorrencia();
    visibilidadeUtilizarSelecaoPorNotasFiscaisCTe();
}

function visiblidadeValorOcorrencia() {
    if (_ocorrencia.ComponenteFrete.codEntity() > 0 || _ocorrencia.SituacaoOcorrencia.val() == EnumSituacaoOcorrencia.AgInformacoes) {
        _ocorrencia.ValorOcorrencia.visibleFade(true);

        if (_ocorrencia.ComponenteFrete.TipoComponenteFrete == EnumTipoComponenteFrete.ICMS) {
            _ocorrencia.ValorOcorrencia.visible(false);
            _ocorrencia.ValorOcorrencia.required = false;
            _ocorrencia.Tomador.visible(false);
            _ocorrencia.ObservacaoCTe.visible(false);
            _ocorrencia.IncluirICMSFrete.visible(false);
            _ocorrencia.ModeloDocumentoFiscal.visible(false);
            _ocorrencia.TipoTomador.visible(false);

            _ocorrencia.BaseCalculoICMS.visible(true);
            _ocorrencia.AliquotaICMS.visible(true);
            _ocorrencia.ValorICMS.visible(true);
            _ocorrencia.CSTICMS.visible(true);

            var classGridParaComplemento = "col col-xs-12 col-sm-8";
            _ocorrencia.CTesParaComplemento.cssClass(classGridParaComplemento);
            _ocorrencia.CargasParaComplemento.cssClass(classGridParaComplemento);
            _ocorrencia.DocumentosAgrupadosDoVeiculo.cssClass(classGridParaComplemento);
            _ocorrencia.VeiculosContrato.cssClass("col col-xs-12 col-sm-5");

        }
        else {
            _ocorrencia.ValorOcorrencia.visible(true);

            if (_ocorrencia.TipoOcorrencia.NaoGerarDocumento) {
                _ocorrencia.ValorOcorrencia.required = false;
                _ocorrencia.ValorOcorrencia.text(Localization.Resources.Ocorrencias.Ocorrencia.ValorDaOcorrencia.getFieldDescription());
            }
            else {
                _ocorrencia.ValorOcorrencia.required = true;
                _ocorrencia.ValorOcorrencia.text(Localization.Resources.Ocorrencias.Ocorrencia.ValorDaOcorrencia.getRequiredFieldDescription());
            }

            if ((_ocorrencia.TipoOcorrencia.OcorrenciaComplementoValorFreteCarga == null || _ocorrencia.TipoOcorrencia.OcorrenciaComplementoValorFreteCarga === false)) {

                if (_ocorrencia.TipoOcorrencia.CalculaValorPorTabelaFrete === false)
                    _ocorrencia.Tomador.visible(true);
                else
                    _ocorrencia.ApenasReboque.text(Localization.Resources.Ocorrencias.Ocorrencia.ReboqueETracao);


                if (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.Terceiros)
                    _ocorrencia.ObservacaoCTe.visible(true);

                _ocorrencia.BaseCalculoICMS.visible(false);
                _ocorrencia.AliquotaICMS.visible(false);
                _ocorrencia.ValorICMS.visible(false);
                _ocorrencia.CSTICMS.visible(false);

                if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
                    _ocorrencia.ModeloDocumentoFiscal.visible(true);
                    _ocorrencia.TipoTomador.visible(true);
                }

                var classGridParaComplemento = "col col-xs-12 col-sm-8";
                _ocorrencia.CTesParaComplemento.cssClass(classGridParaComplemento);
                _ocorrencia.CargasParaComplemento.cssClass(classGridParaComplemento);
                _ocorrencia.DocumentosAgrupadosDoVeiculo.cssClass(classGridParaComplemento);
                _ocorrencia.VeiculosContrato.cssClass("col col-xs-12 col-sm-5");

                if (_ocorrencia.CTeEmitidoNoEmbarcador.val()) {
                    transformarGridImportarCTe();
                    _ocorrencia.ValorOcorrencia.enable(false);
                    _ocorrencia.Tomador.enable(false);
                    _ocorrencia.ObservacaoCTe.enable(false);
                    _ocorrencia.IncluirICMSFrete.enable(false);
                }
            } else {
                _ocorrencia.Tomador.visible(false);
                _ocorrencia.ObservacaoCTe.visible(false);
                _ocorrencia.IncluirICMSFrete.visible(false);
                _ocorrencia.ModeloDocumentoFiscal.visible(false);
                _ocorrencia.TipoTomador.visible(false);
                _ocorrencia.CTesParaComplemento.visibleFade(false);

                _ocorrencia.BaseCalculoICMS.visible(false);
                _ocorrencia.AliquotaICMS.visible(false);
                _ocorrencia.ValorICMS.visible(false);
                _ocorrencia.CSTICMS.visible(false);
            }

            ControlarCamposMoedaEstrangeira();
        }
    }
    else {
        _ocorrencia.ValorOcorrencia.visibleFade(false);
        _ocorrencia.ValorOcorrencia.required = false;
        var classGridParaComplemento = "col col-xs-12 col-sm-12";
        _ocorrencia.CTesParaComplemento.cssClass(classGridParaComplemento);
        _ocorrencia.CargasParaComplemento.cssClass(classGridParaComplemento);
        _ocorrencia.DocumentosAgrupadosDoVeiculo.cssClass(classGridParaComplemento);
        _ocorrencia.VeiculosContrato.cssClass(classGridParaComplemento);
    }
}

function SetarCamposMultiModal() {
    if (_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal) {
        _pesquisaOcorrencia.NumeroBooking.visible(true);
        _pesquisaOcorrencia.NumeroControle.visible(true);
        _pesquisaOcorrencia.NumeroOS.visible(true);
    }
}

function calcularTotalHoras() {
    var hoursFreeTime = 0;
    var hours = 0;
    var minute = 0;
    var dataInicio = _ocorrencia.DataInicio.val();
    var dataTermino = _ocorrencia.DataFim.val();

    if (!string.IsNullOrWhiteSpace(dataInicio) && !string.IsNullOrWhiteSpace(dataTermino)) {
        dataInicio = Global.criarData(dataInicio).getTime();
        dataTermino = Global.criarData(dataTermino).getTime();

        var msec = dataTermino - dataInicio;
        if (!isNaN(msec)) {
            var mins = Math.floor(msec / 60000);
            var hrs = Math.floor(mins / 60);

            minute = mins % 60;
            hours = hrs;
            hoursFreeTime = hours - _ocorrencia.FreeTime.val();
        }
    }

    if (hours < 0)
        _ocorrencia.TotalHoras.val("-" + addZero(hours * -1) + ":" + addZero(minute));
    else
        _ocorrencia.TotalHoras.val(addZero(hours) + ":" + addZero(minute));

    _ocorrencia.TotalHorasCalculo.val(hoursFreeTime < 0 ? "-" : "" + addZero(hoursFreeTime) + ":" + addZero(minute))
}

function addZero(i) {
    if (i < 10)
        i = "0" + i;

    return i;
}


function visibilidadeValorComponenteExistente() {
    if (_ocorrencia.ComponenteFrete.codEntity() > 0 && _ocorrencia.Codigo.val() == 0 && _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS)
        _ocorrencia.ValorComponenteExistenteCTes.visible(true);
    else
        _ocorrencia.ValorComponenteExistenteCTes.visible(false);
}

function visibilidadeUtilizarSelecaoPorNotasFiscaisCTe() {
    if (_ocorrencia.ComponenteFrete.codEntity() == 0 && _ocorrencia.TipoOcorrencia.origemOcorrencia == EnumOrigemOcorrencia.PorCarga && _CONFIGURACAO_TMS.PermitirIncluirOcorrenciaPorSelecaoNotasFiscaisCTe)
        _ocorrencia.UtilizarSelecaoPorNotasFiscaisCTe.visible(true);
    else
        _ocorrencia.UtilizarSelecaoPorNotasFiscaisCTe.visible(false);
}

function CalcularValorComponenteExistente(ctes) {
    if (_ocorrencia.ComponenteFrete.codEntity() > 0 && _ocorrencia.Codigo.val() == 0 && _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
        executarReST("CargaCTe/CalcularValorComponenteCTes", { CTes: JSON.stringify(ctes), CodigoComponenteFrete: _ocorrencia.ComponenteFrete.codEntity() }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    _ocorrencia.ValorComponenteExistenteCTes.val(r.Data);
                }
            } else {
                _ocorrencia.ValorComponenteExistenteCTes.val(r.Data);
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
            }
        });
    }
}

function OcorrenciaReprovadaBlur() {
    _ocorrencia.ValorOcorrencia.enable(true);
    _ocorrencia.TipoTomador.enable(true);
    _ocorrencia.ObservacaoCTe.enable(true);
    _ocorrencia.Observacao.enable(true);
    _ocorrencia.DataOcorrencia.enable(false);
    _ocorrencia.Carga.enable(false);
    _ocorrencia.TipoOcorrencia.enable(false);
    _ocorrencia.ComponenteFrete.enable(false);
    _ocorrencia.GrupoOcorrencia.enable(false);
}

function loadValorCentroResultado() {
    if (_ocorrencia.Carga.codEntity() == 0)
        return;

    executarReST("Ocorrencia/BuscarCentroResultadoCarga", { Codigo: _ocorrencia.Carga.codEntity() }, function (arg) {
        if (arg.Success) {
            if (arg.Data != null) {
                _ocorrencia.CentroResultado.val(arg.Data.CentroResultado);
            }
        }
    });
}

function limparFiltrosOcorrencia() {
    LimparCampos(_pesquisaOcorrencia);
}

function VisibilidadeSelecionarProdutos() {
    return _selecionarProdutos;
}