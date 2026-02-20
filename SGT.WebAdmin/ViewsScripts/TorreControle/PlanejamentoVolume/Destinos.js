/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Localidade.js" />

// #region Objetos Globais do Arquivo

var _planejamentoDestinos;
var _gridDestinos;

// #endregion Objetos Globais do Arquivo

// #region Classes

var PlanejamentoDestino = function () {
    this.Destino = PropertyEntity({ type: types.event, text: "Adicionar Destino", idBtnSearch: guid(), idGrid: guid() });
}

// #endregion Classes

// #region Funções de Inicialização

function loadDestinos() {
    _planejamentoDestinos = new PlanejamentoDestino();
    KoBindings(_planejamentoDestinos, "knockoutDestinos");

    loadGridDestinos();
}

function loadGridDestinos() {
    var opcaoExcluir = { descricao: "Excluir", id: guid(), metodo: excluirDestinoClick }
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 15, opcoes: [opcaoExcluir] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "85%" }
    ];

    _gridDestinos = new BasicDataTable(_planejamentoDestinos.Destino.idGrid, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarLocalidades(_planejamentoDestinos.Destino, null, null, null, _gridDestinos);

    _gridDestinos.CarregarGrid([]);
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function excluirDestinoClick(registroSelecionado) {
    var listaDestinos = _gridDestinos.BuscarRegistros().slice();

    for (var i = 0; i < listaDestinos.length; i++) {
        if (registroSelecionado.Codigo == listaDestinos[i].Codigo) {
            listaDestinos.splice(i, 1);
            break;
        }
    }

    _gridDestinos.CarregarGrid(listaDestinos);
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function limparCamposDestino() {
    _gridDestinos.CarregarGrid([]);
}

function recarregarGridDestinos() {
    var data = new Array();
    if (!string.IsNullOrWhiteSpace(_planejamentoVolume.Destinos.val())) {

        $.each(_planejamentoVolume.Destinos.val(), function (i, destino) {
            var destinosGrid = new Object();

            destinosGrid.Codigo = destino.CodigoLocalidade;
            destinosGrid.Descricao = destino.DescricaoLocalidade;

            data.push(destinosGrid);
        });

    }
    _gridDestinos.CarregarGrid(data);
}
// #endregion Funções Públicas
