/// <reference path="../../Consultas/PerfilAcesso.js" />

var _gridPermissaoPerfilAcesso

function preencherPerfilAcesso(tipoOcorrencia) {
    var listaPerfilAcesso = obterListaPerfilAcesso();

    tipoOcorrencia["PerfilAcesso"] = JSON.stringify(listaPerfilAcesso);
}

function obterListaPerfilAcesso() {
    return _gridPermissaoPerfilAcesso.BuscarRegistros().slice();
}

function limparCamposPermissoes() {
    _gridPermissaoPerfilAcesso.CarregarGrid([]);
}

function excluirPermissaoPerfilAcesso(registroSelecionado) {
    var perfisAcesso = _gridPermissaoPerfilAcesso.BuscarRegistros();

    for (var i = 0; i < perfisAcesso.length; i++) {
        if (registroSelecionado.Codigo == perfisAcesso[i].Codigo) {
            perfisAcesso.splice(i, 1);
            break;
        }
    }

    _gridPermissaoPerfilAcesso.CarregarGrid(perfisAcesso);
}

function preencherPerfisAcesso(perfisAcesso) {
    _gridPermissaoPerfilAcesso.CarregarGrid(perfisAcesso);
}

function loadGridPermissaoPerfilAcesso() {
    var excluir = { descricao: Localization.Resources.Ocorrencias.TipoOcorrencia.Excluir, id: guid(), evento: "onclick", metodo: excluirPermissaoPerfilAcesso, tamanho: "15", icone: "", visible: true };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(excluir);

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Ocorrencias.TipoOcorrencia.Descrição, width: "80%", className: "text-align-left" }
    ];

    _gridPermissaoPerfilAcesso = new BasicDataTable(_tipoOcorrencia.GridPermissaoPerfilAcesso.idGrid, header, menuOpcoes, null, null, 5);

    new BuscarPerfilAcesso(_tipoOcorrencia.AdicionarPerfilAcesso, null, null, _gridPermissaoPerfilAcesso);

    _tipoOcorrencia.AdicionarPerfilAcesso.basicTable = _gridPermissaoPerfilAcesso;

    recarregarGridPermissaoPerfilAcesso();
}

function recarregarGridPermissaoPerfilAcesso() {
    _gridPermissaoPerfilAcesso.CarregarGrid(_tipoOcorrencia.AdicionarPerfilAcesso.val() || []);
}