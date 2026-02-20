/// <reference path="../../../../../js/app.config.js" />
/// <reference path="../../../../../js/Global/CRUD.js" />
/// <reference path="../../../../../js/Global/Grid.js" />
/// <reference path="../../../../../js/Global/Mensagem.js" />
/// <reference path="../../../../../js/Global/Rest.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacaoOcorrencia.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoPessoaGrupo.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/TipoOperacao.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoDocumentoCreditoDebito.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoDocumentoEmissao.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridOcorrenciaEntrega;
var _pesquisaOcorrenciaEntrega;
var _CRUDRelatorio;
var _CRUDFiltrosRelatorio;
var _relatorioOcorrencia;

var _configuracaoNumeroInteiro = {
    precision: 0,
    allowZero: false,
    allowNegative: false,
    thousands: ""
};

/*
 * Declaração das Classes
 */

var PesquisaOcorrenciaEntrega = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });

    //this.CodigoCargaEmbarcador = PropertyEntity({ text: "Nº da Carga:", visible: ko.observable(true) });
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.Carga.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.CargaAgrupada = PropertyEntity({ text: Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.CargaAgrupada.getFieldDescription(), getType: typesKnockout.string, visible: ko.observable(true) });
    this.NumeroOcorrenciaInicial = PropertyEntity({ text: Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.NumeroOcorrenciaInicial.getFieldDescription(), getType: typesKnockout.int, visible: ko.observable(true), configInt: _configuracaoNumeroInteiro });
    this.NumeroOcorrenciaFinal = PropertyEntity({ text: Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.NumeroOcorrenciaFinal.getFieldDescription(), getType: typesKnockout.int, visible: ko.observable(true), configInt: _configuracaoNumeroInteiro });
    this.NumeroNotaFiscal = PropertyEntity({ text: Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.NumeroNotaFiscal.getFieldDescription(), getType: typesKnockout.int, visible: ko.observable(true), configInt: _configuracaoNumeroInteiro });

    this.Filial = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.Filial.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Ocorrencia = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.DescricaoOcorrencia.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.TipoRelatorio.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });

    this.TransportadorCarga = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS ? Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.EmpresaFilialCarga.getFieldDescription() : Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.TransportadorCarga.getFieldDescription()), idBtnSearch: guid(), visible: ko.observable(true) });

    this.SituacaoOcorrencia = PropertyEntity({ val: ko.observable([]), def: [], getType: typesKnockout.selectMultiple, text: Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.Situacao.getFieldDescription(), options: EnumSituacaoOcorrencia.obterOpcoesPesquisaRelatorio(), visible: ko.observable(true) });
    this.SituacaoCancelamento = PropertyEntity({ val: ko.observable([]), def: [], getType: typesKnockout.selectMultiple, text: Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.SituacaoCancelamento.getFieldDescription(), options: EnumSituacaoOcorrencia.obterOpcoesPesquisaRelatorioCancelamento(), visible: ko.observable(true) });

    this.ValorInicial = PropertyEntity({ text: Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.ValorInicial.getFieldDescription(), getType: typesKnockout.decimal, visible: ko.observable(true) });
    this.ValorFinal = PropertyEntity({ text: Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.ValorFinal.getFieldDescription(), getType: typesKnockout.decimal, visible: ko.observable(true) });

    this.DataSolicitacaoInicial = PropertyEntity({ text: Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.DataCriacaoInicial.getFieldDescription(), val: ko.observable(Global.DataAtual()), def: "", getType: typesKnockout.dateTime, visible: ko.observable(true) });
    this.DataSolicitacaoFim = PropertyEntity({ text: Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.DataCriacaoFinal.getFieldDescription(), val: ko.observable(Global.DataAtual()), def: "", getType: typesKnockout.dateTime, visible: ko.observable(true) });
    this.DataSolicitacaoInicial.dateRangeLimit = this.DataSolicitacaoFim;
    this.DataSolicitacaoFim.dateRangeInit = this.DataSolicitacaoInicial;

    this.GrupoOcorrencia = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.GrupoOcorrencias.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });

    this.TipoPessoa = PropertyEntity({ val: ko.observable(EnumTipoPessoaGrupo.Pessoa), options: EnumTipoPessoaGrupo.obterOpcoes(), def: EnumTipoPessoaGrupo.Pessoa, text: Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.TipoPessoa.getFieldDescription() });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.DestinatarioEntrega.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.GrupoPessoas = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.GrupoPessoas, idBtnSearch: guid(), visible: ko.observable(false) });

    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.Motorista.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.Veiculo.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoOperacaoCarga = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.TipoOperacaoCarga.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoDocumentoCreditoDebito = PropertyEntity({ val: ko.observable(EnumTipoDocumentoCreditoDebito.Todos), options: EnumTipoDocumentoCreditoDebito.obterOpcoesPesquisa(), def: EnumTipoDocumentoCreditoDebito.Todos, text: Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.TipoCreditoDebito.getFieldDescription(), visible: ko.observable(true) });
    this.ModeloDocumentoFiscal = PropertyEntity({ val: ko.observable(EnumTipoDocumentoEmissao.Todos), options: EnumTipoDocumentoEmissao.obterOpcoesPesquisa(), def: EnumTipoDocumentoEmissao.Todos, text: Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.TipoDocumento.getFieldDescription(), visible: ko.observable(true) });

    this.OcorrenciaEstadia = PropertyEntity({ val: ko.observable(""), def: "", options: Global.ObterOpcoesPesquisaBooleano("Sim", "Não"), text: Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.OcorrenciaEstadia.getFieldDescription(), visible: ko.observable(true) });
    this.EtapaEstadia = PropertyEntity({ val: ko.observable([]), def: [], getType: typesKnockout.selectMultiple, text: Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.EtapaEstadia.getFieldDescription(), options: EnumTipoCargaEntrega.obterOpcoes(), visible: ko.observable(true) });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });

    this.TipoPessoa.val.subscribe(function (novoValor) {
        if (novoValor === EnumTipoPessoaGrupo.Pessoa) {
            _pesquisaOcorrenciaEntrega.Pessoa.visible(true);
            _pesquisaOcorrenciaEntrega.GrupoPessoas.visible(false);
            _pesquisaOcorrenciaEntrega.GrupoPessoas.codEntity(0);
            _pesquisaOcorrenciaEntrega.GrupoPessoas.val('');
        } else {
            _pesquisaOcorrenciaEntrega.GrupoPessoas.visible(true);
            _pesquisaOcorrenciaEntrega.Pessoa.visible(false);
            _pesquisaOcorrenciaEntrega.Pessoa.codEntity(0);
            _pesquisaOcorrenciaEntrega.Pessoa.val('');
        }
    });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridOcorrenciaEntrega.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaOcorrenciaEntrega.Visible.visibleFade()) {
                _pesquisaOcorrenciaEntrega.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaOcorrenciaEntrega.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.Avancada, icon: ko.observable("fal fa-plus"), visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.GerarPDF });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.GerarPlanilhaExcel, idGrid: guid() });
};

/*
 * Declaração das Funções de Inicialização
 */

function LoadOcorrenciaEntrega() {
    _pesquisaOcorrenciaEntrega = new PesquisaOcorrenciaEntrega();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiEmbarcador) {
        _pesquisaOcorrenciaEntrega.Filial.visible(false);
    }

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe) {
        _pesquisaOcorrenciaEntrega.TransportadorCarga.visible(false);
    }

    _gridOcorrenciaEntrega = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/OcorrenciaEntrega/PesquisaEntrega", _pesquisaOcorrenciaEntrega);

    _gridOcorrenciaEntrega.SetPermitirEdicaoColunas(true);
    _gridOcorrenciaEntrega.SetQuantidadeLinhasPorPagina(15);

    _relatorioOcorrencia = new RelatorioGlobal("Relatorios/OcorrenciaEntrega/BuscarDadosRelatorioEntrega", _gridOcorrenciaEntrega, function () {
        _relatorioOcorrencia.loadRelatorio(function () {
            KoBindings(_pesquisaOcorrenciaEntrega, "knockoutPesquisaOcorrencia", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaOcorrencia", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaOcorrencia", false);

            new BuscarTransportadores(_pesquisaOcorrenciaEntrega.TransportadorCarga);
            new BuscarCargas(_pesquisaOcorrenciaEntrega.Carga);
            new BuscarFilial(_pesquisaOcorrenciaEntrega.Filial);
            new BuscarTipoOcorrencia(_pesquisaOcorrenciaEntrega.Ocorrencia, null, null, null, null, null, null, null, null, null, true);
            new BuscarMotoristas(_pesquisaOcorrenciaEntrega.Motorista);
            new BuscarClientes(_pesquisaOcorrenciaEntrega.Pessoa);
            new BuscarGrupoOcorrencia(_pesquisaOcorrenciaEntrega.GrupoOcorrencia);
            new BuscarGruposPessoas(_pesquisaOcorrenciaEntrega.GrupoPessoas);
            new BuscarVeiculos(_pesquisaOcorrenciaEntrega.Veiculo);
            new BuscarTiposOperacao(_pesquisaOcorrenciaEntrega.TipoOperacaoCarga);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaOcorrenciaEntrega);

    AlterarLayoutPorTipoSistema();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function GerarRelatorioPDFClick() {
    _relatorioOcorrencia.gerarRelatorio("Relatorios/OcorrenciaEntrega/GerarRelatorioEntrega", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick() {
    _relatorioOcorrencia.gerarRelatorio("Relatorios/OcorrenciaEntrega/GerarRelatorioEntrega", EnumTipoArquivoRelatorio.XLS);
}

/*
 * Declaração das Funções Privadas
 */

function AlterarLayoutPorTipoSistema() {

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
        _pesquisaOcorrenciaEntrega.NumeroOcorrenciaCliente.visible(true);
    }
}