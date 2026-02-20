/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/FaixaTemperatura.js" />

// #region Objetos Globais do Arquivo

var _faixasTemperatura;
var _gridFaixaTemperatura;
var _listaFaixaTemperatura = new Array();

// #endregion Objetos Globais do Arquivo

// #region Classes

var FaixasTemperatura = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.FaixaTemperatura = PropertyEntity({ type: types.event, text: Localization.Resources.Veiculos.VeiculoLicenca.AdicionarFaixaTemperatura, idBtnSearch: guid(), visible: ko.observable(true) });
};

// #endregion Classes

// #region Funções de Inicialização

function loadFaixasTemperatura() {
    _faixasTemperatura = new FaixasTemperatura();
    KoBindings(_faixasTemperatura, "knockoutLicencaVeiculoFaixasTemperatura");

    loadGridFaixasTemperatura();
}

function loadGridFaixasTemperatura() {
    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: function (data) {
                excluirFaixaTemperaturaClick(_faixasTemperatura.FaixaTemperatura, data)
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "70%" }
    ];

    _gridFaixaTemperatura = new BasicDataTable(_faixasTemperatura.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarFaixaTemperatura(_faixasTemperatura.FaixaTemperatura, function (r) {
        if (r != null) {
            for (var i = 0; i < r.length; i++)
                _listaFaixaTemperatura.push({ Codigo: parseInt(r[i].Codigo), Descricao: r[i].Descricao });

            recarregarGridFaixaTemperatura();
        }
    }, null, null, _gridFaixaTemperatura);

    _faixasTemperatura.FaixaTemperatura.basicTable = _gridFaixaTemperatura;
    recarregarGridFaixaTemperatura();
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function excluirFaixaTemperaturaClick(knoutFaixaTemperatura, data) {
    var FaixaTemperaturaGrid = knoutFaixaTemperatura.basicTable.BuscarRegistros();

    for (var i = 0; i < FaixaTemperaturaGrid.length; i++) {
        if (data.Codigo == FaixaTemperaturaGrid[i].Codigo) {
            FaixaTemperaturaGrid.splice(i, 1);
            break;
        }
    }

    knoutFaixaTemperatura.basicTable.CarregarGrid(FaixaTemperaturaGrid);
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function obterDescricaoFaixaTemperatura(listaFaixaTemperatura) {
    var descricoes = [];

    for (var i = 0; i < listaFaixaTemperatura.length; i++)
        descricoes.push(listaFaixaTemperatura[i].Descricao);

    return descricoes.join(", ");
}

function obterListaFaixaTemperaturaSalvar(listaFaixaTemperatura) {
    var listaFaixaTemperaturaSalvar = [];

    for (var i = 0; i < listaFaixaTemperatura.length; i++)
        listaFaixaTemperaturaSalvar.push(listaFaixaTemperatura[i].Codigo);

    return JSON.stringify(listaFaixaTemperaturaSalvar);
}

function recarregarGridFaixaTemperatura() {
    _gridFaixaTemperatura.CarregarGrid(_listaFaixaTemperatura);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiCTe) {
        _gridFaixaTemperatura.DesabilitarOpcoes();
    }
}

// #endregion Funções Públicas
