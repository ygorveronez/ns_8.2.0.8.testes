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
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoTelaLicenca.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumStatusLicenca.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Licenca.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Motorista.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Veiculo.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Usuario.js" />
/// <reference path="../../../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioLicenca, _gridLicenca, _pesquisaLicenca, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var PesquisaLicenca = function () {
    this.DataInicial = PropertyEntity({ text: "Data Vencimento Inicial: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Vencimento Final: ", getType: typesKnockout.date });
    this.Descricao = PropertyEntity({ text: "Descrição Licença: ", getType: typesKnockout.string });
    this.NumeroLicenca = PropertyEntity({ text: "N° Licença: ", getType: typesKnockout.string });
    this.TipoTelaLicenca = PropertyEntity({ val: ko.observable(EnumTipoTelaLicenca.Todos), options: EnumTipoTelaLicenca.obterOpcoesPesquisa(), def: EnumTipoTelaLicenca.Todos, text: "Tipo Licença: " });
    this.Licenca = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Licença:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid(), visible: ko.observable(false) });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid(), visible: ko.observable(false) });
    this.Funcionario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Funcionário:", idBtnSearch: guid(), visible: ko.observable(false) });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Pessoa:", idBtnSearch: guid(), visible: ko.observable(false) });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(""), options: EnumStatusLicenca.obterOpcoesPesquisa(), def: "" });
    this.StatusEntidade = PropertyEntity({ text: "Status da Entidade:", options: _statusPesquisa, val: ko.observable(_statusPesquisa.Todos), def: _statusPesquisa.Todos });

    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridLicenca.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******


function loadRelatorioLicenca() {

    _pesquisaLicenca = new PesquisaLicenca();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridLicenca = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/Licenca/Pesquisa", _pesquisaLicenca, null, null, 10);
    _gridLicenca.SetPermitirEdicaoColunas(true);

    _relatorioLicenca = new RelatorioGlobal("Relatorios/Licenca/BuscarDadosRelatorio", _gridLicenca, function () {
        _relatorioLicenca.loadRelatorio(function () {
            KoBindings(_pesquisaLicenca, "knockoutPesquisaLicenca");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaLicenca");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaLicenca");

            new BuscarLicenca(_pesquisaLicenca.Licenca);
            new BuscarFuncionario(_pesquisaLicenca.Funcionario);
            new BuscarClientes(_pesquisaLicenca.Pessoa);
            new BuscarVeiculos(_pesquisaLicenca.Veiculo);
            new BuscarMotoristas(_pesquisaLicenca.Motorista);

            _pesquisaLicenca.TipoTelaLicenca.val.subscribe(exibeCamposFiltro);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaLicenca);
}
function exibeCamposFiltro(val) {
    if (val == EnumTipoTelaLicenca.Funcionario)
        _pesquisaLicenca.Funcionario.visible(true);
    else
        _pesquisaLicenca.Funcionario.visible(false);

    if (val == EnumTipoTelaLicenca.Veiculo)
        _pesquisaLicenca.Veiculo.visible(true);
    else
        _pesquisaLicenca.Veiculo.visible(false);

    if (val == EnumTipoTelaLicenca.Motorista)
        _pesquisaLicenca.Motorista.visible(true);
    else
        _pesquisaLicenca.Motorista.visible(false);

    if (val == EnumTipoTelaLicenca.Pessoa)
        _pesquisaLicenca.Pessoa.visible(true);
    else
        _pesquisaLicenca.Pessoa.visible(false);

    LimparCampo(_pesquisaLicenca.Funcionario);
    LimparCampo(_pesquisaLicenca.Veiculo);
    LimparCampo(_pesquisaLicenca.Motorista);
    LimparCampo(_pesquisaLicenca.Pessoa);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioLicenca.gerarRelatorio("Relatorios/Licenca/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioLicenca.gerarRelatorio("Relatorios/Licenca/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}