/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Localidade.js" />

// #region Objetos Globais do Arquivo

var _planejamentoOrigens;
var _gridOrigens;

// #endregion Objetos Globais do Arquivo

// #region Classes

var PlanejamentoOrigem = function () {
    this.Origem = PropertyEntity({ type: types.event, text: "Adicionar Origem", idBtnSearch: guid(), idGrid: guid() });
}

// #endregion Classes

// #region Funções de Inicialização

function loadOrigens() {
    _planejamentoOrigens = new PlanejamentoOrigem();
    KoBindings(_planejamentoOrigens, "knockoutOrigens");

    loadGridOrigens();
}

function loadGridOrigens() {
    var opcaoExcluir = { descricao: "Excluir", id: guid(), metodo: excluirOrigemClick }
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 15, opcoes: [opcaoExcluir] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "85%" }
    ];

    _gridOrigens = new BasicDataTable(_planejamentoOrigens.Origem.idGrid, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarLocalidades(_planejamentoOrigens.Origem, null, null, null, _gridOrigens);

    _gridOrigens.CarregarGrid([]);
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function excluirOrigemClick(registroSelecionado) {
    var listaOrigens = _gridOrigens.BuscarRegistros().slice();

    for (var i = 0; i < listaOrigens.length; i++) {
        if (registroSelecionado.Codigo == listaOrigens[i].Codigo) {
            listaOrigens.splice(i, 1);
            break;
        }
    }

    _gridOrigens.CarregarGrid(listaOrigens);
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function limparCamposOrigem() {
    _gridOrigens.CarregarGrid([]);
}

function recarregarGridOrigens() {
    var data = new Array();
    if (!string.IsNullOrWhiteSpace(_planejamentoVolume.Origens.val())) {

        $.each(_planejamentoVolume.Origens.val(), function (i, origem) {
            var origensGrid = new Object();

            origensGrid.Codigo = origem.CodigoLocalidade;
            origensGrid.Descricao = origem.DescricaoLocalidade;

            data.push(origensGrid);
        });

    }
    _gridOrigens.CarregarGrid(data);
}
// #endregion Funções Públicas
