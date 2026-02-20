/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Motorista.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Usuario.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/CentroResultado.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/CentroCarregamento.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/ContratoFreteTransportador.js" />
/// <reference path="../../../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../../../js/Global/CRUD.js" />
/// <reference path="../../../../../js/Global/Rest.js" />
/// <reference path="../../../../../js/Global/Mensagem.js" />
/// <reference path="../../../../../js/Global/Grid.js" />
/// <reference path="../../../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSimNaoPesquisa.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/SegmentoVeiculo.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />
/// <reference path="../../../../../js/app.config.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridVeiculo, _pesquisaVeiculo, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _relatorioVeiculo;




var PesquisaVeiculo = function () {
    this.Visible = PropertyEntity({ visible: false, idFade: guid(), visibleFade: ko.observable(false) });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Veiculos.Veiculo.TipoDoRelatorio.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });

    this.Placa = PropertyEntity({ text: Localization.Resources.Relatorios.Veiculos.Veiculo.Placa.getFieldDescription(), maxlength: 7 });
    this.Chassi = PropertyEntity({ text: Localization.Resources.Relatorios.Veiculos.Veiculo.Chassi.getFieldDescription(), maxlength: 700 });

    this.DataCadastroInicial = PropertyEntity({ text: Localization.Resources.Relatorios.Veiculos.Veiculo.DataAtualizacaoInicial.getFieldDescription(), getType: typesKnockout.date });
    this.DataCadastroFinal = PropertyEntity({ text: Localization.Resources.Relatorios.Veiculos.Veiculo.DataAtualizacaoFinal.getFieldDescription(), getType: typesKnockout.date });
    this.DataCadastroInicial.dateRangeLimit = this.DataCadastroFinal;
    this.DataCadastroFinal.dateRangeInit = this.DataCadastroInicial;


    this.TipoVeiculo = PropertyEntity({ val: ko.observable("-1"), options: EnumTipoVeiculo.obterOpcoesPesquisa(), def: "-1", text: Localization.Resources.Relatorios.Veiculos.Veiculo.TipoVeiculo.getFieldDescription() });
    this.Tipo = PropertyEntity({ val: ko.observable("A"), def: "A", options: EnumTipoVeiculoLetra.obterOpcoesPesquisaPropriedade(), text: Localization.Resources.Relatorios.Veiculos.Veiculo.TipoDaPropriedade.getFieldDescription(), issue: 151 });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription() });

    this.Proprietario = PropertyEntity({ type: types.entity, codEntity: ko.observable(""), defCodEntity: "", text: Localization.Resources.Relatorios.Veiculos.Veiculo.Proprietario.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false) });
    this.Locador = PropertyEntity({ type: types.entity, codEntity: ko.observable(""), defCodEntity: "", text: "Locador", required: ko.observable(false), idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(""), defCodEntity: "", text: Localization.Resources.Gerais.Geral.Motorista.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Transportador = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(""), defCodEntity: "", text: Localization.Resources.Relatorios.Veiculos.Veiculo.Transportador.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false) });
    this.CentroResultado = PropertyEntity({ type: types.entity, codEntity: ko.observable(""), defCodEntity: "", text: Localization.Resources.Relatorios.Veiculos.Veiculo.CentroResultado.getFieldDescription(), idBtnSearch: guid() });
    this.Segmento = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Veiculos.Veiculo.Segmentos.getFieldDescription(), idBtnSearch: guid() });
    this.FuncionarioResponsavel = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(""), defCodEntity: "", text: Localization.Resources.Relatorios.Veiculos.Veiculo.FuncionarioResponsavel.getFieldDescription(), idBtnSearch: guid() });
    this.PossuiVinculo = PropertyEntity({ options: EnumSimNaoPesquisa.obterOpcoesPesquisa2(), text: Localization.Resources.Relatorios.Veiculos.Veiculo.PossuiVinculo.getFieldDescription(), val: ko.observable(EnumSimNaoPesquisa.Todos2), visible: true });
    this.ModeloVeicularCarga = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Veiculos.Veiculo.ModeloVeicularCarga.getFieldDescription(), idBtnSearch: guid() });

    this.CentroCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Veiculos.Veiculo.CentroCarregamento.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(false) });
    this.VeiculoPossuiTagValePedagio = PropertyEntity({ text: Localization.Resources.Relatorios.Veiculos.Veiculo.VeiculoPossuiTagValePedagio, getType: typesKnockout.bool, val: ko.observable(false) });
    this.Bloqueado = PropertyEntity({ options: EnumSimNaoPesquisa.obterOpcoesPesquisa(), text: Localization.Resources.Relatorios.Veiculos.Veiculo.Bloqueado.getFieldDescription(), val: ko.observable(EnumSimNaoPesquisa.Todos) });

    this.MarcaVeiculo = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(""), defCodEntity: "", text: Localization.Resources.Relatorios.Veiculos.Veiculo.Marca.getFieldDescription(), idBtnSearch: guid() });
    this.ModeloVeiculo = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(""), defCodEntity: "", text: Localization.Resources.Relatorios.Veiculos.Veiculo.Modelo.getFieldDescription(), idBtnSearch: guid() });

    this.DataCriacaoInicial = PropertyEntity({ text: Localization.Resources.Relatorios.Veiculos.Veiculo.DataCriacaoInicial.getFieldDescription(), getType: typesKnockout.date });
    this.DataCriacaoFinal = PropertyEntity({ text: Localization.Resources.Relatorios.Veiculos.Veiculo.DataCriacaoFinal.getFieldDescription(), getType: typesKnockout.date });
    this.DataCriacaoInicial.dateRangeLimit = this.DataCriacaoFinal;
    this.DataCriacaoFinal.dateRangeInit = this.DataCriacaoInicial;

    this.ContratoFrete = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Contrato de Frete", idBtnSearch: guid() });
    this.TagSemParar = PropertyEntity({ text: Localization.Resources.Relatorios.Veiculos.Veiculo.TagSemParar.getFieldDescription(), maxlength: 150 });
}

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridVeiculo.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Preview, idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaVeiculo.Visible.visibleFade()) {
                _pesquisaVeiculo.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaVeiculo.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Avancada, icon: ko.observable("fal fa-plus"), visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: Localization.Resources.Gerais.Geral.GerarPDF });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: Localization.Resources.Gerais.Geral.GerarPlanilhaExcel, idGrid: guid() });
};

//*******EVENTOS*******

function LoadVeiculo() {
    _pesquisaVeiculo = new PesquisaVeiculo();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridVeiculo = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/Veiculo/Pesquisa", _pesquisaVeiculo);

    _gridVeiculo.SetPermitirEdicaoColunas(true);
    _gridVeiculo.SetQuantidadeLinhasPorPagina(10);

    _relatorioVeiculo = new RelatorioGlobal("Relatorios/Veiculo/BuscarDadosRelatorio", _gridVeiculo, function () {
        _relatorioVeiculo.loadRelatorio(function () {
            KoBindings(_pesquisaVeiculo, "knockoutPesquisaVeiculo", false);
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaVeiculo", false);
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaVeiculo", false);

            new BuscarClientes(_pesquisaVeiculo.Proprietario, retornoProprietario);
            new BuscarTransportadores(_pesquisaVeiculo.Transportador, retornoSelecaoTransportador);
            new BuscarMotoristas(_pesquisaVeiculo.Motorista);
            new BuscarSegmentoVeiculo(_pesquisaVeiculo.Segmento);
            new BuscarFuncionario(_pesquisaVeiculo.FuncionarioResponsavel);
            new BuscarCentrosCarregamento(_pesquisaVeiculo.CentroCarregamento);
            new BuscarCentroResultado(_pesquisaVeiculo.CentroResultado);
            new BuscarClientes(_pesquisaVeiculo.Locador);

            new BuscarMarcasVeiculo(_pesquisaVeiculo.MarcaVeiculo);
            new BuscarModelosVeiculo(_pesquisaVeiculo.ModeloVeiculo);
            new BuscarModelosVeicularesCarga(_pesquisaVeiculo.ModeloVeicularCarga);

            new BuscarContratoFreteTransportador(_pesquisaVeiculo.ContratoFrete);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaVeiculo);

    controlarCampos();
}

function retornoProprietario(row) {
    _pesquisaVeiculo.Proprietario.codEntity(row.Codigo);
    _pesquisaVeiculo.Proprietario.val(row.Nome);
}

function retornoSelecaoTransportador(transportadorSelecionado) {
    _pesquisaVeiculo.Transportador.codEntity(transportadorSelecionado.Codigo);
    _pesquisaVeiculo.Transportador.val(transportadorSelecionado.Descricao);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioVeiculo.gerarRelatorio("Relatorios/Veiculo/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioVeiculo.gerarRelatorio("Relatorios/Veiculo/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}

//*******MÉTODOS*******

function controlarCampos() {
    _pesquisaVeiculo.Proprietario.visible(_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.Terceiros);
    _pesquisaVeiculo.Transportador.visible(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador);
    _pesquisaVeiculo.CentroCarregamento.visible(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador);
}