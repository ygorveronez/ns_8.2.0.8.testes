/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Localidade.js" />

// #region Objetos Globais do Arquivo

var _planejamentoDestinatarios;
var _gridDestinatarios;

// #endregion Objetos Globais do Arquivo

// #region Classes

var PlanejamentoDestinatario = function () {
    this.Destinatario = PropertyEntity({ type: types.event, text: "Adicionar Destinatário", idBtnSearch: guid(), idGrid: guid() });
}

// #endregion Classes

// #region Funções de Inicialização

function loadDestinatarios() {
    _planejamentoDestinatarios = new PlanejamentoDestinatario();
    KoBindings(_planejamentoDestinatarios, "knockoutDestinatarios");

    loadGridDestinatarios();
}

function loadGridDestinatarios() {
    var opcaoExcluir = { descricao: "Excluir", id: guid(), metodo: excluirDestinatarioClick };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 15, opcoes: [opcaoExcluir] };

    var header = [
        { data: "Codigo", title: "CPF/CNPJ", width: "85%" },
        { data: "Descricao", title: "Descrição", width: "85%" }
    ];

    _gridDestinatarios = new BasicDataTable(_planejamentoDestinatarios.Destinatario.idGrid, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    BuscarClientes(_planejamentoDestinatarios.Destinatario, null, null, null, null, _gridDestinatarios);

    _gridDestinatarios.CarregarGrid([]);
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function excluirDestinatarioClick(registroSelecionado) {
    var listaDestinatarios = _gridDestinatarios.BuscarRegistros().slice();

    for (var i = 0; i < listaDestinatarios.length; i++) {
        if (registroSelecionado.Codigo == listaDestinatarios[i].Codigo) {
            listaDestinatarios.splice(i, 1);
            break;
        }
    }

    _gridDestinatarios.CarregarGrid(listaDestinatarios);
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function limparCamposDestinatario() {
    _gridDestinatarios.CarregarGrid([]);
}

function recarregarGridDestinatarios() {
    var data = new Array();
    if (!string.IsNullOrWhiteSpace(_planejamentoVolume.Destinatarios.val())) {
        $.each(_planejamentoVolume.Destinatarios.val(), function (i, Destinatario) {
            var destinatariosGrid = new Object();

            destinatariosGrid.Codigo = Destinatario.CPFCNPJDestinatario;
            destinatariosGrid.Descricao = Destinatario.NomeDestinatario;

            data.push(destinatariosGrid);
        });

    }
    _gridDestinatarios.CarregarGrid(data);
}
// #endregion Funções Públicas
