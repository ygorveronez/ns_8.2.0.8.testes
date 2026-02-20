/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridClientes;

/*
 * Declaração das Classes
 */
var ClienteMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: "" });
    this.CPF_CNPJ = PropertyEntity({ type: types.map, val: "" });
    this.Descricao = PropertyEntity({ type: types.map, val: "" });
};
/*
 * Declaração das Funções de Inicialização
 */

function loadGridRepresentacoesRepresentacao() {
    var opcaoExcluir = { descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: function (data) { excluirRepresentacaoClick(data) } };

    var menuOpcoes = new Object();
    menuOpcoes.opcoes = new Array();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes.push(opcaoExcluir);
    var header = [
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "40%" },
        { data: "Codigo", title: Localization.Resources.Pessoas.Usuario.CPFBarraCNPJ, width: "40%" },
    ];

    _gridClientes = new BasicDataTable(_usuario.GridRepresentacoes.idGrid, header, menuOpcoes);
    recarregarGridRepresentacoes();
}

function loadRepresentacao() {
    loadGridRepresentacoesRepresentacao();

    new BuscarClientes(_usuario.AdicionarRepresentacoes, function (retorno) {
        if (retorno != null) {
            for (var i = 0; i < retorno.length; i++) {
                var cliente = new ClienteMap();
                cliente.Codigo.val = retorno[i].CPF_CNPJ;
                cliente.CPF_CNPJ.val = retorno[i].CPF_CNPJ;
                cliente.Descricao.val = retorno[i].Descricao;
                _usuario.GridRepresentacoes.list.push(cliente);
            }
        }
        recarregarGridRepresentacoes();
    }, false, null, null, _gridClientes);

}

/*
 * Declaração das Funções Associadas a Eventos
 */

function excluirRepresentacaoClick(data) {

    var listaAtualizada = new Array();
    $.each(_usuario.GridRepresentacoes.list, function (i, cliente) {
        if (cliente.Codigo.val != data.Codigo) {
            listaAtualizada.push(cliente);
        }
    });
    _usuario.GridRepresentacoes.list = listaAtualizada;
    recarregarGridRepresentacoes();
}
/*
 * Declaração das Funções
 */

function recarregarGridRepresentacoes() {
    var data = new Array();
    $.each(_usuario.GridRepresentacoes.list, function (i, usuario) {
        var cliente = new Object();

        cliente.Codigo = usuario.CPF_CNPJ.val;
        cliente.Descricao = usuario.Descricao.val;

        data.push(cliente);
    });
    _gridClientes.CarregarGrid(data);
}