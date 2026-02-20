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
/// <reference path="../../../../../ViewsScripts/Consultas/Localidade.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Motorista.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Veiculo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/CentroResultado.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Produto.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacaoColaborador.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumAposentadoriaFuncionario.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSimNaoPesquisa.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoMotorista.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Licenca.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridMotorista, _pesquisaMotorista, _CRUDRelatorio, _relatorioMotorista, _CRUDFiltrosRelatorio;

var PesquisaMotorista = function () {
    this.Nome = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Nome.getFieldDescription() });
    this.CPF = PropertyEntity({ text: "CPF: ", maxlength: 14, getType: typesKnockout.cpf });
    this.Codigo = PropertyEntity({ text: Localization.Resources.Relatorios.Frotas.Motorista.CodigoIntegracao.getFieldDescription() });

    this.TipoMotorista = PropertyEntity({ val: ko.observable(EnumTipoMotorista.Todos), options: EnumTipoMotorista.obterOpcoesPesquisa(), def: EnumTipoMotorista.Todos, text: Localization.Resources.Relatorios.Frotas.Motorista.TipoMotorista.getFieldDescription() });
    this.Status = PropertyEntity({ val: ko.observable(0), options: _statusPesquisa, def: 0, text: Localization.Resources.Gerais.Geral.Status.getFieldDescription() });
    this.SituacaoColaborador = PropertyEntity({ text: Localization.Resources.Relatorios.Frotas.Motorista.SituacaoColaborador.getFieldDescription(), val: ko.observable(EnumSituacaoColaborador.Todos), options: EnumSituacaoColaborador.obterOpcoesPesquisa(), def: EnumSituacaoColaborador.Todos });
    this.Aposentadoria = PropertyEntity({ text: Localization.Resources.Relatorios.Frotas.Motorista.Aposentado.getFieldDescription(), val: ko.observable(EnumAposentadoriaFuncionario.Todos), options: EnumAposentadoriaFuncionario.obterOpcoesPesquisa(), def: EnumAposentadoriaFuncionario.Todos });
    this.Bloqueado = PropertyEntity({ options: EnumSimNaoPesquisa.obterOpcoesPesquisa(), text: Localization.Resources.Relatorios.Frotas.Motorista.Bloqueado.getFieldDescription(), val: ko.observable(EnumSimNaoPesquisa.Todos) });
    this.UsuarioMobile = PropertyEntity({ options: EnumSimNaoPesquisa.obterOpcoesPesquisa(), text: Localization.Resources.Relatorios.Frotas.Motorista.UsuarioMobile.getFieldDescription(), val: ko.observable(EnumSimNaoPesquisa.Todos) });
    this.NaoBloquearAcessoSimultaneo = PropertyEntity({ options: EnumSimNaoPesquisa.obterOpcoesPesquisa(), text: Localization.Resources.Relatorios.Frotas.Motorista.MudarAparelhoSemBloquear.getFieldDescription(), val: ko.observable(EnumSimNaoPesquisa.Todos) });

    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Motorista.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Frotas.Motorista.Veiculo.getFieldDescription(), idBtnSearch: guid() });
    this.Licenca = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Frotas.Motorista.Licenca.getFieldDescription(), idBtnSearch: guid() });
    this.CentroResultado = PropertyEntity({ type: types.entity, codEntity: ko.observable(""), defCodEntity: "", text: Localization.Resources.Relatorios.Frotas.Motorista.CentroResultado.getFieldDescription(), idBtnSearch: guid() });
    this.CargoMotorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Frotas.Motorista.CargoMotorista.getFieldDescription(), idBtnSearch: guid() });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Frotas.Motorista.Transportadora.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiNFe) });
    this.Gestor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable(Localization.Resources.Relatorios.Frotas.Motorista.Gestor.getFieldDescription()), idBtnSearch: guid(), enable: ko.observable(true) });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Relatorios.Frotas.Motorista.TipoRelatorio.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true) });
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridMotorista.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaMotorista.Visible.visibleFade()) {
                _pesquisaMotorista.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaMotorista.Visible.visibleFade(true);
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

function loadRelatorioMotorista() {

    _pesquisaMotorista = new PesquisaMotorista();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridMotorista = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "/Relatorios/Motorista/Pesquisa", _pesquisaMotorista, null, null, 10);
    _gridMotorista.SetPermitirEdicaoColunas(true);

    _relatorioMotorista = new RelatorioGlobal("Relatorios/Motorista/BuscarDadosRelatorio", _gridMotorista, function () {
        _relatorioMotorista.loadRelatorio(function () {
            KoBindings(_pesquisaMotorista, "knockoutPesquisaMotorista");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaMotorista");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaMotorista");

            new BuscarMotoristasPorStatus(_pesquisaMotorista.Motorista);
            new BuscarVeiculos(_pesquisaMotorista.Veiculo);
            new BuscarLicenca(_pesquisaMotorista.Licenca);
            new BuscarCentroResultado(_pesquisaMotorista.CentroResultado);
            new BuscarCargos(_pesquisaMotorista.CargoMotorista);
            new BuscarTransportadores(_pesquisaMotorista.Transportador);
            new BuscarFuncionario(_pesquisaMotorista.Gestor);
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaMotorista);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioMotorista.gerarRelatorio("Relatorios/Motorista/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioMotorista.gerarRelatorio("Relatorios/Motorista/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}
