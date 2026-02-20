/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/TipoDeCarga.js" />

// #region Objetos Globais do Arquivo

var _configuracaoProgramacaoCargaTipoCarga;
var _gridTipoCarga;

// #endregion Objetos Globais do Arquivo

// #region Classes

var ConfiguracaoProgramacaoCargaTipoCarga = function () {
    this.TipoCarga = PropertyEntity({ type: types.event, text: "Adicionar Tipo de Carga", idBtnSearch: guid(), idGrid: guid() });
}

// #endregion Classes

// #region Funções de Inicialização

function loadTipoCarga() {
    _configuracaoProgramacaoCargaTipoCarga = new ConfiguracaoProgramacaoCargaTipoCarga();
    KoBindings(_configuracaoProgramacaoCargaTipoCarga, "knockoutCadastroConfiguracaoProgramacaoCargaTipoCarga");

    loadGridTipoCarga();
}

function loadGridTipoCarga() {
    var opcaoExcluir = { descricao: "Excluir", id: guid(), metodo: excluirTipoCargaClick }
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 15, opcoes: [opcaoExcluir] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "85%" }
    ];

    _gridTipoCarga = new BasicDataTable(_configuracaoProgramacaoCargaTipoCarga.TipoCarga.idGrid, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarTiposdeCarga(_configuracaoProgramacaoCargaTipoCarga.TipoCarga, null, null, _gridTipoCarga);

    _gridTipoCarga.CarregarGrid([]);
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function excluirTipoCargaClick(registroSelecionado) {
    var listaTipoCarga = _gridTipoCarga.BuscarRegistros().slice();

    for (var i = 0; i < listaTipoCarga.length; i++) {
        if (registroSelecionado.Codigo == listaTipoCarga[i].Codigo) {
            listaTipoCarga.splice(i, 1);
            break;
        }
    }

    _gridTipoCarga.CarregarGrid(listaTipoCarga);
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function limparCamposTipoCarga() {
    _gridTipoCarga.CarregarGrid([]);
}

function preencherTipoCarga(configuracaoProgramacaoCarga) {
    _gridTipoCarga.CarregarGrid(configuracaoProgramacaoCarga.TipoCarga);
}

function preencherTipoCargaSalvar(configuracaoProgramacaoCarga) {
    configuracaoProgramacaoCarga["TipoCarga"] = JSON.stringify(_gridTipoCarga.BuscarRegistros().slice());
}

// #endregion Funções Públicas
