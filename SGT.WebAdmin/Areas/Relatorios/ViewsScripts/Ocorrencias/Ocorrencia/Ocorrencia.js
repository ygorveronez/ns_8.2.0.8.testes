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

var _gridOcorrencia;
var _pesquisaOcorrencia;
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

var PesquisaOcorrencia = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });

    //this.CodigoCargaEmbarcador = PropertyEntity({ text: "Nº da Carga:", visible: ko.observable(true) });
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.Carga.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.CargaAgrupada = PropertyEntity({ text: Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.CargaAgrupada.getFieldDescription(), getType: typesKnockout.string, visible: ko.observable(true) });
    this.NumeroOcorrenciaInicial = PropertyEntity({ text: Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.NumeroOcorrenciaInicial.getFieldDescription(), getType: typesKnockout.int, visible: ko.observable(true), configInt: _configuracaoNumeroInteiro });
    this.NumeroOcorrenciaFinal = PropertyEntity({ text: Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.NumeroOcorrenciaFinal.getFieldDescription(), getType: typesKnockout.int, visible: ko.observable(true), configInt: _configuracaoNumeroInteiro });
    this.NumeroCTeOriginal = PropertyEntity({ text: Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.NumeroCTOriginal.getFieldDescription(), getType: typesKnockout.int, visible: ko.observable(true), configInt: _configuracaoNumeroInteiro });
    this.NumeroCTeGerado = PropertyEntity({ text: Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.NumeroCTeGerado.getFieldDescription(), getType: typesKnockout.int, visible: ko.observable(true), configInt: _configuracaoNumeroInteiro });
    this.NumeroNotaFiscal = PropertyEntity({ text: Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.NumeroNotaFiscal.getFieldDescription(), getType: typesKnockout.int, visible: ko.observable(true), configInt: _configuracaoNumeroInteiro });
    this.NumeroOcorrenciaCliente = PropertyEntity({ text: Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.NumeroOcorrenciaClientee.getFieldDescription(), visible: ko.observable(false) });

    this.Operador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.Operador.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Filial = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.Filial.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Solicitante = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.Solicitante.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false) });
    this.Ocorrencia = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.DescricaoOcorrencia.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.TipoRelatorio.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });

    this.TransportadorCarga = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS ? Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.EmpresaFilialCarga.getFieldDescription(): Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.TransportadorCarga.getFieldDescription()), idBtnSearch: guid(), visible: ko.observable(true) });
    this.TransportadorChamado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS ? Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.EmpresaFilialChamado.getFieldDescription() : Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.TransportadorChamado.getFieldDescription()), idBtnSearch: guid(), visible: ko.observable(false) });
    this.ResponsavelChamado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.ResponsavelChamado, idBtnSearch: guid(), visible: ko.observable(false) });

    this.SituacaoOcorrencia = PropertyEntity({ val: ko.observable([]), def: [], getType: typesKnockout.selectMultiple, text: Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.Situacao.getFieldDescription(), options: EnumSituacaoOcorrencia.obterOpcoesPesquisaRelatorio(), visible: ko.observable(true) });
    this.SituacaoCancelamento = PropertyEntity({ val: ko.observable([]), def: [], getType: typesKnockout.selectMultiple, text: Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.SituacaoCancelamento.getFieldDescription(), options: EnumSituacaoOcorrencia.obterOpcoesPesquisaRelatorioCancelamento(), visible: ko.observable(true) });

    this.ValorInicial = PropertyEntity({ text: Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.ValorInicial.getFieldDescription(), getType: typesKnockout.decimal, visible: ko.observable(true) });
    this.ValorFinal = PropertyEntity({ text: Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.ValorFinal.getFieldDescription(), getType: typesKnockout.decimal, visible: ko.observable(true) });

    this.DataSolicitacaoInicial = PropertyEntity({ text: Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.DataSolicitacaoInicial.getFieldDescription(), val: ko.observable(Global.DataAtual()), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataSolicitacaoFim = PropertyEntity({ text: Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.DataSolicitacaoFinal.getFieldDescription(), val: ko.observable(Global.DataAtual()), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataSolicitacaoInicial.dateRangeLimit = this.DataSolicitacaoFim;
    this.DataSolicitacaoFim.dateRangeInit = this.DataSolicitacaoInicial;

    this.DataOcorrenciaInicial = PropertyEntity({ text: Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.DataAprovacaoInicial.getFieldDescription(), getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataOcorrenciaFinal = PropertyEntity({ text: Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.DataAprovacaoFinal.getFieldDescription(), getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataOcorrenciaInicial.dateRangeLimit = this.DataOcorrenciaFinal;
    this.DataOcorrenciaFinal.dateRangeInit = this.DataOcorrenciaInicial;

    this.DataCancelamentoInicial = PropertyEntity({ text: Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.DataCancelamentoAnualInicial.getFieldDescription(), getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataCancelamentoFinal = PropertyEntity({ text: Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.DataCancelamentoAnualFinal.getFieldDescription(), getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataCancelamentoInicial.dateRangeLimit = this.DataCancelamentoFinal;
    this.DataCancelamentoFinal.dateRangeInit = this.DataCancelamentoInicial;

    this.TipoPessoa = PropertyEntity({ val: ko.observable(EnumTipoPessoaGrupo.Pessoa), options: EnumTipoPessoaGrupo.obterOpcoes(), def: EnumTipoPessoaGrupo.Pessoa, text: Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.TipoPessoa.getFieldDescription() });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.Pessoa.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.GrupoPessoas = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.GrupoPessoas, idBtnSearch: guid(), visible: ko.observable(false) });

    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.Motorista.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.Veiculo.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoOperacaoCarga = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.TipoOperacaoCarga.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoDocumentoCreditoDebito = PropertyEntity({ val: ko.observable(EnumTipoDocumentoCreditoDebito.Todos), options: EnumTipoDocumentoCreditoDebito.obterOpcoesPesquisa(), def: EnumTipoDocumentoCreditoDebito.Todos, text: Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.TipoCreditoDebito.getFieldDescription(), visible: ko.observable(true) });
    this.ModeloDocumentoFiscal = PropertyEntity({ val: ko.observable(EnumTipoDocumentoEmissao.Todos), options: EnumTipoDocumentoEmissao.obterOpcoesPesquisa(), def: EnumTipoDocumentoEmissao.Todos, text: Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.TipoDocumento.getFieldDescription(), visible: ko.observable(true) });

    this.GrupoOcorrencia = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.GrupoOcorrencias.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.OcorrenciaEstadia = PropertyEntity({ val: ko.observable(""), def: "", options: Global.ObterOpcoesPesquisaBooleano("Sim", "Não"), text: Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.OcorrenciaEstadia.getFieldDescription(), visible: ko.observable(true) });
    this.EtapaEstadia = PropertyEntity({ val: ko.observable([]), def: [], getType: typesKnockout.selectMultiple, text: Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.EtapaEstadia.getFieldDescription(), options: EnumTipoCargaEntrega.obterOpcoes(), visible: ko.observable(true) });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });

    this.TipoPessoa.val.subscribe(function (novoValor) {
        if (novoValor === EnumTipoPessoaGrupo.Pessoa) {
            _pesquisaOcorrencia.Pessoa.visible(true);
            _pesquisaOcorrencia.GrupoPessoas.visible(false);
            _pesquisaOcorrencia.GrupoPessoas.codEntity(0);
            _pesquisaOcorrencia.GrupoPessoas.val('');
        } else {
            _pesquisaOcorrencia.GrupoPessoas.visible(true);
            _pesquisaOcorrencia.Pessoa.visible(false);
            _pesquisaOcorrencia.Pessoa.codEntity(0);
            _pesquisaOcorrencia.Pessoa.val('');
        }
    });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridOcorrencia.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaOcorrencia.Visible.visibleFade()) {
                _pesquisaOcorrencia.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaOcorrencia.Visible.visibleFade(true);
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

function LoadOcorrencia() {
    _pesquisaOcorrencia = new PesquisaOcorrencia();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiEmbarcador) {
        _pesquisaOcorrencia.Filial.visible(false);
    }

    if (_CONFIGURACAO_TMS.ExigirChamadoParaAbrirOcorrencia) {
        _pesquisaOcorrencia.TransportadorChamado.visible(true);
        _pesquisaOcorrencia.ResponsavelChamado.visible(true);
    }

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe) {
        _pesquisaOcorrencia.TransportadorCarga.visible(false);
    }

    _gridOcorrencia = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/Ocorrencia/Pesquisa", _pesquisaOcorrencia);

    _gridOcorrencia.SetPermitirEdicaoColunas(true);
    _gridOcorrencia.SetQuantidadeLinhasPorPagina(15);

    _relatorioOcorrencia = new RelatorioGlobal("Relatorios/Ocorrencia/BuscarDadosRelatorio", _gridOcorrencia, function () {
        _relatorioOcorrencia.loadRelatorio(function () {
            KoBindings(_pesquisaOcorrencia, "knockoutPesquisaOcorrencia", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaOcorrencia", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaOcorrencia", false);

            new BuscarFuncionario(_pesquisaOcorrencia.Solicitante, undefined, undefined, true);
            new BuscarTransportadores(_pesquisaOcorrencia.TransportadorCarga);
            new BuscarTransportadores(_pesquisaOcorrencia.TransportadorChamado);
            new BuscarFuncionario(_pesquisaOcorrencia.ResponsavelChamado);
            new BuscarCargas(_pesquisaOcorrencia.Carga);
            new BuscarFilial(_pesquisaOcorrencia.Filial);
            new BuscarOperador(_pesquisaOcorrencia.Operador);
            new BuscarTipoOcorrencia(_pesquisaOcorrencia.Ocorrencia, null, null, null, null, null, null, null, null, null, true);
            new BuscarMotoristas(_pesquisaOcorrencia.Motorista);
            new BuscarClientes(_pesquisaOcorrencia.Pessoa);
            new BuscarGruposPessoas(_pesquisaOcorrencia.GrupoPessoas);
            new BuscarVeiculos(_pesquisaOcorrencia.Veiculo);
            new BuscarTiposOperacao(_pesquisaOcorrencia.TipoOperacaoCarga);
            new BuscarGrupoOcorrencia(_pesquisaOcorrencia.GrupoOcorrencia);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaOcorrencia);

    AlterarLayoutPorTipoSistema();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function GerarRelatorioPDFClick() {
    _relatorioOcorrencia.gerarRelatorio("Relatorios/Ocorrencia/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick() {
    _relatorioOcorrencia.gerarRelatorio("Relatorios/Ocorrencia/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}

/*
 * Declaração das Funções Privadas
 */

function AlterarLayoutPorTipoSistema() {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.Terceiros) {
        _pesquisaOcorrencia.Solicitante.visible(true);
    }

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
        _pesquisaOcorrencia.NumeroOcorrenciaCliente.visible(true);
    }
}