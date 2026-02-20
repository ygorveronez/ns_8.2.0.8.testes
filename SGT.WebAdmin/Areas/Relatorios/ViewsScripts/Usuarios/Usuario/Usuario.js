/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoArquivoRelatorio.js" />
/// <reference path="../../Relatorios/Global/Relatorio.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumTipoAcesso.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumSituacaoColaborador.js" />
/// <reference path="../../../../../ViewsScripts/Enumeradores/EnumAposentadoriaFuncionario.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _relatorioUsuarios, _gridUsuarios, _pesquisaUsuarios, _CRUDRelatorio, _CRUDFiltrosRelatorio;

var _statusChar = [
    { text: "Todos", value: "" },
    { text: "Ativo", value: "A" },
    { text: "Inativo", value: "I" }
];

var _usuarioOperador = [
    { text: "Todos", value: "" },
    { text: "Não", value: false },
    { text: "Sim", value: true }
];

var _usuarioAcessoSistema = [
    { text: "Todos", value: "" },
    { text: "Liberado", value: false },
    { text: "Bloqueado", value: true }
];

var _tipoAmbiente = [
    { text: "Todos", value: "" },
    { text: "Embarcador", value: EnumTipoAcesso.Embarcador },
    { text: "Transportador", value: EnumTipoAcesso.Emissao }
];

var PesquisaUsuarios = function () {
    this.DataCadastroInicial = PropertyEntity({ text: "Data Cadastro Inicial: ", getType: typesKnockout.date });
    this.DataCadastroFinal = PropertyEntity({ text: "Data Cadastro Final: ", getType: typesKnockout.date });
    this.DataCadastroInicial.dateRangeLimit = this.DataCadastroFinal;
    this.DataCadastroFinal.dateRangeInit = this.DataCadastroInicial;

    this.UltimoAcessoInicial = PropertyEntity({ text: "Último Acesso Inicial: ", getType: typesKnockout.date });
    this.UltimoAcessoFinal = PropertyEntity({ text: "Último Acesso Final: ", getType: typesKnockout.date });
    this.UltimoAcessoInicial.dateRangeLimit = this.UltimoAcessoFinal;
    this.UltimoAcessoFinal.dateRangeInit = this.UltimoAcessoInicial;

    this.Localidade = PropertyEntity({ text: "Localidade:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.PerfilAcesso = PropertyEntity({ text: "Perfil de Acesso:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });

    this.Ambiente = PropertyEntity({ val: ko.observable(EnumTipoAcesso.Embarcador), options: _tipoAmbiente, text: "Tipo Ambiente: ", visible: true });
    this.Status = PropertyEntity({ val: ko.observable(""), options: _statusChar, text: "Situação: " });
    this.Operador = PropertyEntity({ val: ko.observable(""), options: _usuarioOperador, text: "Usuário Operador Logístico:" });
    this.AcessoSistema = PropertyEntity({ val: ko.observable(""), options: _usuarioAcessoSistema, text: "Acesso Sistema: " });
    this.SituacaoColaborador = PropertyEntity({ text: "Situação Colaborador:", val: ko.observable(EnumSituacaoColaborador.Todos), options: EnumSituacaoColaborador.obterOpcoesPesquisa(), def: EnumSituacaoColaborador.Todos });
    this.Aposentadoria = PropertyEntity({ text: "Aposentado:", val: ko.observable(EnumAposentadoriaFuncionario.Todos), options: EnumAposentadoriaFuncionario.obterOpcoesPesquisa(), def: EnumAposentadoriaFuncionario.Todos });
    this.TipoUsuario = PropertyEntity({ val: ko.observable(EnumTipoUsuario.Funcionarios), options: EnumTipoUsuario.obterOpcoesPesquisa(), def: EnumTipoUsuario.Funcionarios, text: "Tipo Usuário: " });
    this.SomenteUsuariosAtivo = PropertyEntity({ text: "Somente usuários ativos", getType: typesKnockout.bool, val: ko.observable(false), def: false, enable: ko.observable(true)});

    this.Relatorio = PropertyEntity({ getType: typesKnockout.report });
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
    this.TipoRelatorio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo do Relatório:", idBtnSearch: guid(), visible: ko.observable(true), val: ko.observable("") });
};

var CRUDFiltrosRelatorio = function () {
    this.Preview = PropertyEntity({
        eventClick: function (e) {
            _gridUsuarios.CarregarGrid();
        }, type: types.event, text: "Preview", idGrid: "gridPreviewRelatorio", visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaUsuarios.Visible.visibleFade()) {
                _pesquisaUsuarios.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaUsuarios.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", icon: ko.observable("fal fa-plus"), visible: ko.observable(true)
    });
};

var CRUDRelatorio = function () {
    this.GerarRelatorioPDF = PropertyEntity({ eventClick: GerarRelatorioPDFClick, type: types.event, text: "Gerar PDF" });
    this.GerarRelatorioExcel = PropertyEntity({ eventClick: GerarRelatorioExcelClick, type: types.event, text: "Gerar Planilha Excel", idGrid: guid() });
};

//*******EVENTOS*******


function loadRelatorioUsuarios() {
    _pesquisaUsuarios = new PesquisaUsuarios();
    _CRUDRelatorio = new CRUDRelatorio();
    _CRUDFiltrosRelatorio = new CRUDFiltrosRelatorio();

    _gridUsuarios = new GridView(_CRUDFiltrosRelatorio.Preview.idGrid, "Relatorios/Usuario/Pesquisa", _pesquisaUsuarios, null, null, 10);
    _gridUsuarios.SetPermitirEdicaoColunas(true);

    _relatorioUsuarios = new RelatorioGlobal("Relatorios/Usuario/BuscarDadosRelatorio", _gridUsuarios, function () {
        _relatorioUsuarios.loadRelatorio(function () {
            KoBindings(_pesquisaUsuarios, "knockoutPesquisaUsuario");
            KoBindings(_CRUDRelatorio, "knockoutCRUDPesquisaUsuario");
            KoBindings(_CRUDFiltrosRelatorio, "knockoutCRUDFiltrosPesquisaUsuario");

            new BuscarLocalidades(_pesquisaUsuarios.Localidade);
            new BuscarPerfilAcesso(_pesquisaUsuarios.PerfilAcesso);

            $("#divConteudoRelatorio").show();
        });
    }, null, "knockoutCRUDConfiguracaoRelatorio", _pesquisaUsuarios);
}

function GerarRelatorioPDFClick(e, sender) {
    _relatorioUsuarios.gerarRelatorio("Relatorios/Usuario/GerarRelatorio", EnumTipoArquivoRelatorio.PDF);
}

function GerarRelatorioExcelClick(e, sender) {
    _relatorioUsuarios.gerarRelatorio("Relatorios/Usuario/GerarRelatorio", EnumTipoArquivoRelatorio.XLS);
}