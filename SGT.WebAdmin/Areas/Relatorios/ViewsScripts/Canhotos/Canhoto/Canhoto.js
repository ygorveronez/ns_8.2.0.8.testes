/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoGrupoPessoas.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Usuario.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/CentroCarregamento.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Localidade.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Filial.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Tranportador.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Carga.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Estado.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Malote.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/RotaFrete.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/TipoCarga.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/TipoOperacao.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoCanhoto.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacaoPgtoCanhoto.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoPessoaGrupo.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacaoCanhoto.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumOrigemDigitalizacaoCanhoto.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumStatusViagemControleEntrega.js" />

//*******MAPEAMENTO KNOUCKOUT*******

const _enumTipoCanhoto = [
    { text: "Todos", value: "" },
    { text: "NF-e", value: EnumTipoCanhoto.NFe },
    { text: "Avulso", value: EnumTipoCanhoto.Avulso }
];


let _gridCanhoto, _pesquisaCanhoto, _CRUDRelatorio, _CRUDFiltrosRelatorio;

let _relatorioCanhoto;

const PesquisaCanhoto = function () {

    const dataAtual = moment().add(-2, 'days').format("DD/MM/YYYY");
    this.Visible = PropertyEntity({ visible: false, idFade: guid(), visibleFade: ko.observable(false) });

    this.DataInicio = PropertyEntity({ text: Localization.Resources.Relatorios.Canhotos.Canhoto.DataInicialEmissao.getFieldDescription(), val: ko.observable(dataAtual), def: dataAtual, getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataFim = PropertyEntity({ text: Localization.Resources.Relatorios.Canhotos.Canhoto.DataFinalEmissao.getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;

    this.DataEmissaoCTeInicial = PropertyEntity({ text: Localization.Resources.Relatorios.Canhotos.Canhoto.DtEmissaoCTeInicial.getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataEmissaoCTeFinal = PropertyEntity({ text: Localization.Resources.Relatorios.Canhotos.Canhoto.DtEmissaoCTeFinal.getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataEmissaoCTeInicial.dateRangeLimit = this.DataEmissaoCTeFinal;
    this.DataEmissaoCTeFinal.dateRangeInit = this.DataEmissaoCTeInicial;

    this.DataInicioDigitalizacao = PropertyEntity({ text: Localization.Resources.Relatorios.Canhotos.Canhoto.DataDigitalizacaoInicial.getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataFimDigitalizacao = PropertyEntity({ text: Localization.Resources.Relatorios.Canhotos.Canhoto.DataDigitalizacaoFinal.getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataInicioDigitalizacao.dateRangeLimit = this.DataFimDigitalizacao;
    this.DataFimDigitalizacao.dateRangeInit = this.DataInicioDigitalizacao;

    this.DataInicioEnvio = PropertyEntity({ text: Localization.Resources.Relatorios.Canhotos.Canhoto.DataEnvioInicial.getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataFimEnvio = PropertyEntity({ text: Localization.Resources.Relatorios.Canhotos.Canhoto.DataEnvioFinal.getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true) });
    this.DataInicioEnvio.dateRangeLimit = this.DataFimEnvio;
    this.DataFimEnvio.dateRangeInit = this.DataInicioEnvio;

    this.CodigoCargaEmbarcador = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Canhotos.Canhoto.NumeroDaCarga.getFieldDescription(), idBtnSearch: guid(), val: ko.observable(""), def: "", visible: ko.observable(true), cssClass: ko.observable("col col-xs-6 col-md-2") });
    this.Carga = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0, visible: ko.observable(false) });
    this.Chave = PropertyEntity({ text: Localization.Resources.Relatorios.Canhotos.Canhoto.ChaveNFe.getFieldDescription(), maxlength: 44, visible: ko.observable(false) });
    this.Terceiro = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Canhotos.Canhoto.Terceiro.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false) });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Motorista.getFieldDescription(), idBtnSearch: guid(), cssClass: ko.observable("col col-xs-12 col-sm-6 col-md-4") });
    this.Filial = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Canhotos.Canhoto.Filial.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false) });
    this.SituacaoCanhoto = PropertyEntity({ val: ko.observable(new Array()), options: EnumSituacaoCanhoto.obterOpcoes(), def: new Array(), text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription(), getType: typesKnockout.selectMultiple});
        
    this.SituacaoDigitalizacaoCanhoto = PropertyEntity({ val: ko.observable(""), options: EnumSituacaoDigitalizacaoCanhoto.ObterOpcoesPesquisa(), def: "", text: Localization.Resources.Relatorios.Canhotos.Canhoto.SituacaoDigitalizacao.getFieldDescription()});

    this.TipoCanhoto = PropertyEntity({ val: ko.observable(EnumTipoCanhoto.Todos), options: EnumTipoCanhoto.obterOpcoesPesquisaComPlaceHolder(), def: EnumTipoCanhoto.Todos, text: Localization.Resources.Gerais.Geral.Tipo.getFieldDescription(), visible: ko.observable(false) });
    this.TipoCarga = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Canhotos.Canhoto.TipoDeCarga.getFieldDescription(), idBtnSearch: guid() });
    this.TipoOperacao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Canhotos.Canhoto.TipoDeOperacao.getFieldDescription(), idBtnSearch: guid(), cssClass: ko.observable("col col-xs-12 col-sm-6 col-md-4") });
    this.Recebedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Canhotos.Canhoto.Recebedor.getFieldDescription(), idBtnSearch: guid() });
    this.Destinatario = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Destinatario.getFieldDescription(), idBtnSearch: guid() });
    this.Usuario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Usuario.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false) });

    this.Numero = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Numero.getFieldDescription(), idBtnSearch: guid(), getType: typesKnockout.int, maxlength: 9, configInt: { precision: 0, allowZero: false, thousands: "" }, visible: ko.observable(true) });
    this.Serie = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Serie.getFieldDescription(), getType: typesKnockout.int, maxlength: 3, configInt: { precision: 0, allowZero: false, thousands: "" }, visible: ko.observable(true) });
    this.LocalArmazenamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Canhotos.Canhoto.LocalDeArmazenamento.getFieldDescription(), idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.Pacote = PropertyEntity({ text: Localization.Resources.Relatorios.Canhotos.Canhoto.Pacote.getFieldDescription(), getType: typesKnockout.int, maxlength: 9, configInt: { precision: 0, allowZero: false, thousands: "" }, visible: ko.observable(true) });
    this.Posicao = PropertyEntity({ text: Localization.Resources.Relatorios.Canhotos.Canhoto.Posicao.getFieldDescription(), getType: typesKnockout.int, maxlength: 9, configInt: { precision: 0, allowZero: false, thousands: "" }, visible: ko.observable(true) });

    this.Empresa = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Canhotos.Canhoto.Transportador.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false) });

    this.TipoPessoa = PropertyEntity({ val: ko.observable(true), options: EnumTipoPessoaGrupo.obterOpcoes(), def: true, text: Localization.Resources.Relatorios.Canhotos.Canhoto.TipoDeEmitente.getFieldDescription(), issue: 306, required: true, eventChange: TipoPessoaChange, visible: ko.observable(true) });
    this.Emitente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Canhotos.Canhoto.Emitente.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true), cssClass: ko.observable("col col-xs-12 col-sm-8 col-md-4") });
    this.GrupoPessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Canhotos.Canhoto.Emitente.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false) });
    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), text: Localization.Resources.Relatorios.Canhotos.Canhoto.MarcarTodas.getFieldDescription(), visible: ko.observable(true) });

    this.SituacaoViagem = PropertyEntity({ val: ko.observable(EnumStatusViagemControleEntrega.Todos), options: EnumStatusViagemControleEntrega.obterOpcoesPesquisa(), def: EnumStatusViagemControleEntrega.Todas, text: Localization.Resources.Relatorios.Canhotos.Canhoto.SituacaoViagem.getFieldDescription(), visible: ko.observable(true) });
    this.SituacaoPgtoCanhoto = PropertyEntity({ val: ko.observable(EnumSituacaoPgtoCanhoto.Todos), options: EnumSituacaoPgtoCanhoto.obterOpcoesPesquisa(), def: EnumSituacaoPgtoCanhoto.Todas, text: Localization.Resources.Relatorios.Canhotos.Canhoto.SituacaoPgto.getFieldDescription(), visible: ko.observable(false) });
    this.OrigemDigitalizacao = PropertyEntity({ val: ko.observable(EnumOrigemDigitalizacaoCanhoto.Todas), options: EnumOrigemDigitalizacaoCanhoto.obterOpcoesPesquisa(), def: EnumOrigemDigitalizacaoCanhoto.Todas, text: Localization.Resources.Relatorios.Canhotos.Canhoto.OrigemDigitalizacao.getFieldDescription(), visible: ko.observable(true) });

    this.PlacaVeiculoResponsavelEntrega = PropertyEntity({ text: Localization.Resources.Relatorios.Canhotos.Canhoto.PlacaVeiculoResponsavelEntrega.getFieldDescription(), maxlength: 10, visible: ko.observable(true) });

    this.SituacaoHistorico = PropertyEntity({ val: ko.observable(""), def: "", options: EnumSituacaoCanhoto.obterOpcoesPesquisa("Não Selecionado", ""), text: Localization.Resources.Relatorios.Canhotos.Canhoto.SituacaoDoHistorico.getFieldDescription(), visible: ko.observable(true) });
    this.DataInicialHistorico = PropertyEntity({ text: Localization.Resources.Relatorios.Canhotos.Canhoto.DtIniHistorico.getFieldDescription(), val: ko.observable(dataAtual), def: dataAtual, getType: typesKnockout.date, visible: ko.observable(true), enable: ko.observable(false) });
    this.DataFinalHistorico = PropertyEntity({ text: Localization.Resources.Relatorios.Canhotos.Canhoto.DtFinHistorico.getFieldDescription(), val: ko.observable(dataAtual), def: dataAtual, getType: typesKnockout.date, visible: ko.observable(true), enable: ko.observable(false) });
    this.DataInicialHistorico.dateRangeLimit = this.DataFinalHistorico;
    this.DataFinalHistorico.dateRangeInit = this.DataInicialHistorico;

    this.DataCriacaoCargaInicial = PropertyEntity({ text: Localization.Resources.Relatorios.Canhotos.Canhoto.DataCriacaoCargaInicial.getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true), enable: ko.observable(true) });
    this.DataCriacaoCargaFinal = PropertyEntity({ text: Localization.Resources.Relatorios.Canhotos.Canhoto.DataCriacaoCargaFinal.getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true), enable: ko.observable(true) });


    this.Origem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Origem:", idBtnSearch: guid() });
    this.Destino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destino:", idBtnSearch: guid() });
    this.Expedidor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Expedidor:", idBtnSearch: guid() });
    this.GrupoPessoaTomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Pessoa do Tomador:", idBtnSearch: guid() });
    this.Malote = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Malote:", idBtnSearch: guid() });

    this.SituacaoHistorico.val.subscribe(function (value) {
        const enable = (!string.IsNullOrWhiteSpace(value.toString()));

        _pesquisaCanhoto.DataInicialHistorico.enable(enable);
        _pesquisaCanhoto.DataFinalHistorico.enable(enable);
    });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Canhotos.Canhoto.TipoDoRelatorio.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

const CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: Localization.Resources.Relatorios.Canhotos.Canhoto.GerarPDF});
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: Localization.Resources.Relatorios.Canhotos.Canhoto.GerarPlanilhaExcel, idGrid: guid() });
};

const CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridCanhoto.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaCanhoto.Visible.visibleFade()) {
                _pesquisaCanhoto.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaCanhoto.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Avancada, icon: ko.observable("fal fa-plus"), visible: ko.observable(false)
    });
};

//*******EVENTOS*******

function LoadCanhoto() {
    _pesquisaCanhoto = new PesquisaCanhoto();
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS)
        _pesquisaCanhoto.SituacaoCanhoto.options = EnumSituacaoCanhoto.obterOpcoesMultiTMS();

    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();
    _gridCanhoto = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/Canhoto/Pesquisa", _pesquisaCanhoto);

    _gridCanhoto.SetPermitirEdicaoColunas(true);
    _gridCanhoto.SetQuantidadeLinhasPorPagina(10);

    _relatorioCanhoto = new RelatorioGlobal("Relatorios/Canhoto/BuscarDadosRelatorio", _gridCanhoto, function () {
        _relatorioCanhoto.loadRelatorio(function () {
            KoBindings(_pesquisaCanhoto, "knockoutPesquisaCanhoto", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaCanhoto", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaCanhoto", false);

            BuscarTransportadores(_pesquisaCanhoto.Empresa);
            BuscarFilial(_pesquisaCanhoto.Filial);
            BuscarMotorista(_pesquisaCanhoto.Motorista, retornoMotorista);
            BuscarClientes(_pesquisaCanhoto.Emitente);
            BuscarClientes(_pesquisaCanhoto.Terceiro);
            BuscarClientes(_pesquisaCanhoto.Recebedor);
            BuscarClientes(_pesquisaCanhoto.Destinatario);
            BuscarGruposPessoas(_pesquisaCanhoto.GrupoPessoa, null, null, null, EnumTipoGrupoPessoas.Clientes);
            BuscarLocalArmazenamentoCanhoto(_pesquisaCanhoto.LocalArmazenamento);
            BuscarTiposdeCarga(_pesquisaCanhoto.TipoCarga);
            BuscarTiposOperacao(_pesquisaCanhoto.TipoOperacao);
            BuscarFuncionario(_pesquisaCanhoto.Usuario);
            BuscarClientes(_pesquisaCanhoto.Expedidor);
            BuscarLocalidades(_pesquisaCanhoto.Origem);
            BuscarLocalidades(_pesquisaCanhoto.Destino);
            BuscarGruposPessoas(_pesquisaCanhoto.GrupoPessoaTomador);
            BuscarMalote(_pesquisaCanhoto.Malote);
            BuscarCanhotos(_pesquisaCanhoto.Numero);
            BuscarCargas(_pesquisaCanhoto.CodigoCargaEmbarcador);

            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador) {
                _pesquisaCanhoto.Empresa.visible(true);
                _pesquisaCanhoto.Filial.visible(true);
                _pesquisaCanhoto.TipoPessoa.visible(false);
                _pesquisaCanhoto.TipoCanhoto.visible(true);
                _pesquisaCanhoto.GrupoPessoa.visible(false);
                _pesquisaCanhoto.Emitente.cssClass("col col-xs-12 col-md-6");
                _pesquisaCanhoto.Motorista.cssClass("col col-xs-12 col-md-4");
                _pesquisaCanhoto.TipoOperacao.cssClass("col col-xs-12 col-md-6");
                if (_CONFIGURACAO_TMS.UtilizaPgtoCanhoto)
                    _pesquisaCanhoto.SituacaoPgtoCanhoto.visible(true);
                _pesquisaCanhoto.DataInicioEnvio.visible(false);
                _pesquisaCanhoto.DataFimEnvio.visible(false);
            } else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
                _pesquisaCanhoto.Emitente.visible(true);
                _pesquisaCanhoto.Terceiro.visible(true);
                _pesquisaCanhoto.Usuario.visible(true);
                _pesquisaCanhoto.CodigoCargaEmbarcador.cssClass("col col-xs-12 col-sm-6 col-md-2");
                _pesquisaCanhoto.Motorista.cssClass("col col-xs-12 col-sm-6");
            } else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiCTe) {
                _pesquisaCanhoto.LocalArmazenamento.visible(false);
            }

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaCanhoto);
}

function retornoMotorista(data) {
    _pesquisaCanhoto.Motorista.codEntity(data.Codigo);
    _pesquisaCanhoto.Motorista.val(data.Nome);
}

function TipoPessoaChange(e, sender) {
    if (e.TipoPessoa.val() == EnumTipoPessoaGrupo.Pessoa) {
        e.Emitente.visible(true);
        e.GrupoPessoa.visible(false);
        LimparCampoEntity(e.GrupoPessoa);
    } else if (e.TipoPessoa.val() == EnumTipoPessoaGrupo.GrupoPessoa) {
        e.Emitente.visible(false);
        e.GrupoPessoa.visible(true);
        LimparCampoEntity(e.Emitente);
    }
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioCanhoto.gerarRelatorio("Relatorios/Canhoto/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioCanhoto.gerarRelatorio("Relatorios/Canhoto/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}