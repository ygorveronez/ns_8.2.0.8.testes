/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />

// #region Objetos Globais do Arquivo

var _configuracaoProgramacaoCargaModeloVeicularCarga;
var _gridModeloVeicularCarga;

// #endregion Objetos Globais do Arquivo

// #region Classes

var ConfiguracaoProgramacaoCargaModeloVeicularCarga = function () {
    this.ModeloVeicularCarga = PropertyEntity({ type: types.event, text: "Adicionar Modelo Veicular de Carga", idBtnSearch: guid(), idGrid: guid() });
}

// #endregion Classes

// #region Funções de Inicialização

function loadModeloVeicularCarga() {
    _configuracaoProgramacaoCargaModeloVeicularCarga = new ConfiguracaoProgramacaoCargaModeloVeicularCarga();
    KoBindings(_configuracaoProgramacaoCargaModeloVeicularCarga, "knockoutCadastroConfiguracaoProgramacaoCargaModeloVeicularCarga");

    loadGridModeloVeicularCarga();
}

function loadGridModeloVeicularCarga() {
    var opcaoExcluir = { descricao: "Excluir", id: guid(), metodo: excluirModeloVeicularCargaClick }
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 15, opcoes: [opcaoExcluir] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "85%" }
    ];

    _gridModeloVeicularCarga = new BasicDataTable(_configuracaoProgramacaoCargaModeloVeicularCarga.ModeloVeicularCarga.idGrid, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarModelosVeicularesCarga(_configuracaoProgramacaoCargaModeloVeicularCarga.ModeloVeicularCarga, null, null, null, null, null, _gridModeloVeicularCarga);

    _gridModeloVeicularCarga.CarregarGrid([]);
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function excluirModeloVeicularCargaClick(registroSelecionado) {
    var listaModeloVeicularCarga = _gridModeloVeicularCarga.BuscarRegistros().slice();

    for (var i = 0; i < listaModeloVeicularCarga.length; i++) {
        if (registroSelecionado.Codigo == listaModeloVeicularCarga[i].Codigo) {
            listaModeloVeicularCarga.splice(i, 1);
            break;
        }
    }

    _gridModeloVeicularCarga.CarregarGrid(listaModeloVeicularCarga);
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function limparCamposModeloVeicularCarga() {
    _gridModeloVeicularCarga.CarregarGrid([]);
}

function preencherModeloVeicularCarga(configuracaoProgramacaoCarga) {
    _gridModeloVeicularCarga.CarregarGrid(configuracaoProgramacaoCarga.ModeloVeicularCarga);
}

function preencherModeloVeicularCargaSalvar(configuracaoProgramacaoCarga) {
    configuracaoProgramacaoCarga["ModeloVeicularCarga"] = JSON.stringify(_gridModeloVeicularCarga.BuscarRegistros().slice());
}

// #endregion Funções Públicas
