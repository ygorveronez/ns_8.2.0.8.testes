/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Tranportador.js" />

// #region Objetos Globais do Arquivo

var _gridEmpresaExclusiva;
var _empresaExclusiva;

// #endregion Objetos Globais do Arquivo

// #region Classes

var EmpresaExclusiva = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Empresa = PropertyEntity({ type: types.event, text: Localization.Resources.Logistica.RotaFrete.AdicionarTransportador, idBtnSearch: guid() });
}

// #endregion Classes

// #region Funções de Inicialização

function loadEmpresaExclusiva() {
    _empresaExclusiva = new EmpresaExclusiva();
    KoBindings(_empresaExclusiva, "knockoutEmpresaExclusivaRotaFrete");

    var opcaoExcluir = { descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: excluirEmpresaExclusivaClick };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 20, opcoes: [opcaoExcluir] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Logistica.RotaFrete.Transportador, width: "80%" }
    ];

    _gridEmpresaExclusiva = new BasicDataTable(_empresaExclusiva.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarTransportadores(_empresaExclusiva.Empresa, null, menuOpcoes, null, _gridEmpresaExclusiva);

    _empresaExclusiva.Empresa.basicTable = _gridEmpresaExclusiva;

    _gridEmpresaExclusiva.CarregarGrid(new Array());
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function excluirEmpresaExclusivaClick(registroSelecionado) {
    var empresasExclusivas = obterListaEmpresaExclusiva();

    for (var i = 0; i < empresasExclusivas.length; i++) {
        if (registroSelecionado.Codigo == empresasExclusivas[i].Codigo) {
            empresasExclusivas.splice(i, 1);
            break;
        }
    }

    _gridEmpresaExclusiva.CarregarGrid(empresasExclusivas);
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function limparCamposEmpresaExclusiva() {
    _gridEmpresaExclusiva.CarregarGrid(new Array());
}

function preencherEmpresaExclusiva(empresasExclusivas) {
    _gridEmpresaExclusiva.CarregarGrid(empresasExclusivas);
}

function preencherEmpresaExclusivaSalvar(rotaFrete) {
    rotaFrete["EmpresasExclusivas"] = JSON.stringify(obterListaEmpresaExclusiva());
}

// #endregion Funções Públicas

// #region Funções Privadas

function obterListaEmpresaExclusiva() {
    return _gridEmpresaExclusiva.BuscarRegistros().slice();
}

// #endregion Funções Privadas
