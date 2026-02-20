/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Localidade.js" />

// #region Objetos Globais do Arquivo

var _planejamentoRemetentes;
var _gridRemetentes;

// #endregion Objetos Globais do Arquivo

// #region Classes

var PlanejamentoRemetentes = function () {
    this.Remetente = PropertyEntity({ type: types.event, text: "Adicionar Remetente", idBtnSearch: guid(), idGrid: guid() });
}

// #endregion Classes

// #region Funções de Inicialização

function loadRemetentes() {
    _planejamentoRemetentes = new PlanejamentoRemetentes();
    KoBindings(_planejamentoRemetentes, "knockoutRemetentes");

    loadGridRemetentes();
}

function loadGridRemetentes() {
    var opcaoExcluir = { descricao: "Excluir", id: guid(), metodo: excluirRemetenteClick }
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 15, opcoes: [opcaoExcluir] };

    var header = [
        { data: "Codigo", title: "CPF/CNPJ", width: "85%" },
        { data: "Descricao", title: "Descrição", width: "85%" }
    ];

    _gridRemetentes = new BasicDataTable(_planejamentoRemetentes.Remetente.idGrid, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    BuscarClientes(_planejamentoRemetentes.Remetente, null, null, null, null, _gridRemetentes);

    _gridRemetentes.CarregarGrid([]);
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function excluirRemetenteClick(registroSelecionado) {
    var listaRemetentes = _gridRemetentes.BuscarRegistros().slice();

    for (var i = 0; i < listaRemetentes.length; i++) {
        if (registroSelecionado.Codigo == listaRemetentes[i].Codigo) {
            listaRemetentes.splice(i, 1);
            break;
        }
    }

    _gridRemetentes.CarregarGrid(listaRemetentes);
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function limparCamposRemetente() {
    _gridRemetentes.CarregarGrid([]);
}

function recarregarGridRemetentes() {
    var data = new Array();
    if (!string.IsNullOrWhiteSpace(_planejamentoVolume.Remetentes.val())) {
        $.each(_planejamentoVolume.Remetentes.val(), function (i, Remetente) {
            var remetentesGrid = new Object();

            remetentesGrid.Codigo = Remetente.CPFCNPJRemetente;
            remetentesGrid.Descricao = Remetente.NomeRemetente;

            data.push(remetentesGrid);
        });

    }
    _gridRemetentes.CarregarGrid(data);
}
// #endregion Funções Públicas
