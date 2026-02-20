/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />

// #region Objetos Globais do Arquivo

var _configuracaoProgramacaoCargaTipoOperacao;
var _gridTipoOperacao;

// #endregion Objetos Globais do Arquivo

// #region Classes

var ConfiguracaoProgramacaoCargaTipoOperacao = function () {
    this.TipoOperacao = PropertyEntity({ type: types.event, text: "Adicionar Tipo de Operação", idBtnSearch: guid(), idGrid: guid() });
}

// #endregion Classes

// #region Funções de Inicialização

function loadTipoOperacao() {
    _configuracaoProgramacaoCargaTipoOperacao = new ConfiguracaoProgramacaoCargaTipoOperacao();
    KoBindings(_configuracaoProgramacaoCargaTipoOperacao, "knockoutCadastroConfiguracaoProgramacaoCargaTipoOperacao");

    loadGridTipoOperacao();
}

function loadGridTipoOperacao() {
    var opcaoExcluir = { descricao: "Excluir", id: guid(), metodo: excluirTipoOperacaoClick }
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 15, opcoes: [opcaoExcluir] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "85%" }
    ];

    _gridTipoOperacao = new BasicDataTable(_configuracaoProgramacaoCargaTipoOperacao.TipoOperacao.idGrid, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarTiposOperacao(_configuracaoProgramacaoCargaTipoOperacao.TipoOperacao, null, null, null, _gridTipoOperacao);

    _gridTipoOperacao.CarregarGrid([]);
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function excluirTipoOperacaoClick(registroSelecionado) {
    var listaTipoOperacao = _gridTipoOperacao.BuscarRegistros().slice();

    for (var i = 0; i < listaTipoOperacao.length; i++) {
        if (registroSelecionado.Codigo == listaTipoOperacao[i].Codigo) {
            listaTipoOperacao.splice(i, 1);
            break;
        }
    }

    _gridTipoOperacao.CarregarGrid(listaTipoOperacao);
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function limparCamposTipoOperacao() {
    _gridTipoOperacao.CarregarGrid([]);
}

function preencherTipoOperacao(configuracaoProgramacaoCarga) {
    _gridTipoOperacao.CarregarGrid(configuracaoProgramacaoCarga.TipoOperacao);
}

function preencherTipoOperacaoSalvar(configuracaoProgramacaoCarga) {
    configuracaoProgramacaoCarga["TipoOperacao"] = JSON.stringify(_gridTipoOperacao.BuscarRegistros().slice());
}

// #endregion Funções Públicas
