/// <reference path="../../../../../ViewsScripts/Consultas/CategoriaPessoa.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoPessoa.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumModalidadePessoa.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSimNaoPesquisa.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacaoPessoa.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridPessoa, _pesquisaPessoa, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioPessoa;

var PesquisaPessoa = function () {
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pessoas.Pessoa.TipoRelatorio.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.GrupoPessoas = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pessoas.Pessoa.GrupoDePessoas.getFieldDescription(), idBtnSearch: guid(), issue: 58, visible: ko.observable(true) });
    this.Localidade = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pessoas.Pessoa.Cidade.getFieldDescription(), idBtnSearch: guid(), issue: 16, visible: ko.observable(true) });
    this.Estado = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Pessoas.Pessoa.Estado.getFieldDescription(), idBtnSearch: guid(), issue: 12, visible: ko.observable(true) });
    this.Atividade = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pessoas.Pessoa.Atividade.getFieldDescription(), idBtnSearch: guid(), issue: 12, visible: ko.observable(true) });
    this.Categoria = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pessoas.Pessoa.Categoria.getFieldDescription(), idBtnSearch: guid() });

    this.TipoPessoa = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.TipoDePessoa.getFieldDescription(), options: EnumTipoPessoa.obterOpcoesPesquisa(), val: ko.observable(EnumTipoPessoa.Todas), def: EnumTipoPessoa.Todas, });
    this.ModalidadePessoa = PropertyEntity({ val: ko.observable(new Array()), issue: 0, def: new Array(), getType: typesKnockout.selectMultiple, text: Localization.Resources.Pessoas.Pessoa.Modalidade.getFieldDescription(), options: EnumModalidadePessoa.obterOpcoes(), visible: ko.observable(true) });
    this.Situacao = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.Situacao.getFieldDescription(), options: EnumSituacaoPessoa.obterOpcoes(), val: ko.observable(""), def: "", issue: 0, visible: ko.observable(true) });
    this.Bloqueado = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.Bloqueado.getFieldDescription(), options: EnumSimNaoPesquisa.obterOpcoesPesquisa(), val: ko.observable(EnumSimNaoPesquisa.Todos), def: EnumSimNaoPesquisa.Todos });
    this.AguardandoConferenciaInformacao = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.AguardandoConferenciaDeInformacao.getFieldDescription(), options: EnumSimNaoPesquisa.obterOpcoesPesquisa(), val: ko.observable(EnumSimNaoPesquisa.Todos), def: EnumSimNaoPesquisa.Todos });
    this.ComGeolocalizacao = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.ComGeolocalizacao, options: EnumSimNaoPesquisa.obterOpcoesPesquisa(), val: ko.observable(EnumSimNaoPesquisa.Todos), def: EnumSimNaoPesquisa.Todos });

    this.DataInicio = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.CadastradoDe.getFieldDescription(), getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.Ate.getFieldDescription(), getType: typesKnockout.date });
    this.DataInicio.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicio;
    this.SomenteSemCodigoIntegracao = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.ExibirSomenteSemCodigoIntegracao, val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.ExibeSomenteComCodigoIntegracao = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.ExibeSomenteComCodigoIntegracao, val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.SomenteSemContaContabil = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.ExibirSomenteSemContaContabil, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false) });

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridPessoa.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Pessoas.Pessoa.Preview, idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaPessoa.Visible.visibleFade()) {
                _pesquisaPessoa.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaPessoa.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: Localization.Resources.Pessoas.Pessoa.Avancada, icon: ko.observable("fal fa-plus"), visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: Localization.Resources.Pessoas.Pessoa.GerarPDF });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: Localization.Resources.Pessoas.Pessoa.GerarPlanilhaExcel, idGrid: guid() });
};

//*******EVENTOS*******

function LoadPessoa() {
    _pesquisaPessoa = new PesquisaPessoa();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridPessoa = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/Pessoa/Pesquisa", _pesquisaPessoa);

    _gridPessoa.SetPermitirEdicaoColunas(true);
    _gridPessoa.SetQuantidadeLinhasPorPagina(10);

    _relatorioPessoa = new RelatorioGlobal("Relatorios/Pessoa/BuscarDadosRelatorio", _gridPessoa, function () {
        _relatorioPessoa.loadRelatorio(function () {
            KoBindings(_pesquisaPessoa, "knockoutPesquisaPessoa", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaPessoa", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaPessoa", false);

            new BuscarGruposPessoas(_pesquisaPessoa.GrupoPessoas);
            new BuscarLocalidades(_pesquisaPessoa.Localidade);
            new BuscarEstados(_pesquisaPessoa.Estado);
            new BuscarAtividades(_pesquisaPessoa.Atividade);
            new BuscarCategoriaPessoa(_pesquisaPessoa.Categoria);

            _pesquisaPessoa.SomenteSemContaContabil.visible(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaPessoa);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioPessoa.gerarRelatorio("Relatorios/Pessoa/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioPessoa.gerarRelatorio("Relatorios/Pessoa/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
