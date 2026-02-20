var _grid;
var _pesquisaPedidoLoteCarregamento;
var _gridPesquisa;
var _CRUDEditarNumeroCarregamento;

var PesquisaPedidoLoteCarregamento = function () {
    this.Lote = PropertyEntity({ text: "Lote:", required: true, getType: typesKnockout.string, val: ko.observable(""), maxlength: 200 });
    this.Carregamento = PropertyEntity({ text: "Carregamento:", required: false, getType: typesKnockout.string, val: ko.observable(""), maxlength: 200 });

    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridPesquisa, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}

var NumeroCarregamentoPorLote = function () {
    this.NumeroLote = PropertyEntity({ text: "*Lote:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.NumeroCarregamento = PropertyEntity({ text: "*Carregamento:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Lista = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid() });

    // Botões
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true), enable: ko.observable(true), idBtnSearch: guid() });
    this.Salvar = PropertyEntity({ eventClick: salvarClick, type: types.event, text: "Salvar", visible: ko.observable(true), enable: ko.observable(true) });
    this.Validar = PropertyEntity({ eventClick: validarClick, type: types.event, text: "Validar Carregamento", visible: ko.observable(true), enable: ko.observable(true), idBtnSearch: guid(), val: ko.observable(false) });
}

var CRUDEditarNumeroCarregamento = function () {
    this.Confirmar = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: confirmarAlterarCarregamentoClick, text: "Confirmar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: cancelarAlterarCarregamentoClick, text: "Cancelar", visible: ko.observable(true) });
    this.NumeroCarregamento = PropertyEntity({ text: "*Carregamento:", required: true, getType: typesKnockout.string, val: ko.observable("") });

    this.CodigoPedido = PropertyEntity();
};

function load() {

    _numeroCarregamentoPorLote = new NumeroCarregamentoPorLote();
    KoBindings(_numeroCarregamentoPorLote, "knockoutNumeroCarregamentoPorLote");

    _pesquisaPedidoLoteCarregamento = new PesquisaPedidoLoteCarregamento();
    KoBindings(_pesquisaPedidoLoteCarregamento, "knockoutPesquisaPedidosLoteCarregamento", false, _pesquisaPedidoLoteCarregamento.Pesquisar.id);

    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.list;
    menuOpcoes.opcoes = [
        { descricao: "Excluir", id: guid(), evento: "onclick", metodo: onExcluir, tamanho: "7", icone: "" },
    ];

    var linhasPorPaginas = 99;

    var header = [
        { data: "Codigo", visible: false },
        { data: "Lote", title: "Lote" },
        { data: "Carregamento", title: "Carregamento" },
    ];

    _grid = new BasicDataTable(_numeroCarregamentoPorLote.Lista.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _grid.CarregarGrid([]);

    loadGridPesquisa();
    loadCRUDEditarNumeroCarregamento();
    setupBip();
}

function loadGridPesquisa() {
    var opcaoEditar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var configuracoesExportacao = { url: "NumeroCarregamentoPorLote/ExportarPesquisa", titulo: "Pedidos do Lote" };

    _gridPesquisa = new GridViewExportacao(_pesquisaPedidoLoteCarregamento.Pesquisar.idGrid, "NumeroCarregamentoPorLote/Pesquisa", _pesquisaPedidoLoteCarregamento, menuOpcoes, configuracoesExportacao);
    _gridPesquisa.CarregarGrid();
}


function loadCRUDEditarNumeroCarregamento() {
    _CRUDEditarNumeroCarregamento = new CRUDEditarNumeroCarregamento();
    KoBindings(_CRUDEditarNumeroCarregamento, "knockoutCRUDNumeroCarregamento");

    LimparCampos(_CRUDEditarNumeroCarregamento);
}

// Essa função garante que os scanners vão funcionar da forma correta ao usar a tela.
function setupBip() {
    // Se o foco vai para o botão adicionar, adiciona e volta o foco pro Lote
    $("#" + _numeroCarregamentoPorLote.Adicionar.idBtnSearch).focus((e) => {
        adicionarClick();
        $("#" + _numeroCarregamentoPorLote.NumeroLote.id).focus();
    });
}

function recarregarGrid() {
    var lista = obterLista();

    const dadosGrid = lista.map(item => {
        return item;
    });

    _grid.CarregarGrid(dadosGrid);
}

function obterLista() {
    return _numeroCarregamentoPorLote.Lista.val().slice();
}

function exibirFiltrosClick(e) {
    e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
}

function adicionarClick() {
    if (!_numeroCarregamentoPorLote.Validar.val())
        exibirMensagem(tipoMensagem.aviso, "Aviso", "O Número do Carregamento não foi validado, por favor valide o mesmo!");

    if (ValidarCamposObrigatorios(_numeroCarregamentoPorLote)) {
        var lista = obterLista();
        lista.push({
            Codigo: lista.length,
            Lote: _numeroCarregamentoPorLote.NumeroLote.val(),
            Carregamento: _numeroCarregamentoPorLote.NumeroCarregamento.val()
        });

        _numeroCarregamentoPorLote.Lista.val(lista);
        recarregarGrid();
        LimparCampos(_numeroCarregamentoPorLote);
        _numeroCarregamentoPorLote.Validar.val(false);
    }

}

function onExcluir(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse item?", function () {
        var registros = _grid.BuscarRegistros();

        for (var i = 0; i < registros.length; i++) {
            if (e.Codigo == registros[i].Codigo) {
                registros.splice(i, 1);
                break;
            }
        }
        _numeroCarregamentoPorLote.Lista.val(registros);
        recarregarGrid();
    });
}

function cancelarAlterarCarregamentoClick() {
    Global.fecharModal("divModalEditarNumeroCarregamento");
    LimparCampos(_CRUDEditarNumeroCarregamento);
    _numeroCarregamentoPorLote.Validar.val(false);
}

function confirmarAlterarCarregamentoClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja alterar o número carregamento deste pedido?", function () {
        Salvar(_CRUDEditarNumeroCarregamento, "NumeroCarregamentoPorLote/AlterarCarregamentoPedido", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Número Carregamento alterado com sucesso.");

                    Global.fecharModal("divModalEditarNumeroCarregamento");
                    LimparCampos(_CRUDEditarNumeroCarregamento);
                    _numeroCarregamentoPorLote.Validar.val(false);

                    _gridPesquisa.CarregarGrid();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }, sender);
    });
}


function editarClick(registroSelecionado) {
    _CRUDEditarNumeroCarregamento.CodigoPedido.val(registroSelecionado.Codigo);
    if (registroSelecionado.Carregamento != undefined)
        _CRUDEditarNumeroCarregamento.NumeroCarregamento.val(registroSelecionado.Carregamento);

    Global.abrirModal('divModalEditarNumeroCarregamento');
}

function salvarClick() {
    // Manda pro server

    executarReST("NumeroCarregamentoPorLote/CriarConexao", { Lista: JSON.stringify(_numeroCarregamentoPorLote.Lista.val()) }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Pedidos atualizados com sucesso");

                // Limpa a lista, para adicionar mais
                _numeroCarregamentoPorLote.Lista.val([]);
                recarregarGrid();
            }
            else if (retorno.Data === false)
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });

}

function recarregarGridPesquisa() {
    _gridPesquisa.CarregarGrid();
}

function validarClick() {
    _numeroCarregamentoPorLote.Validar.val(true);

    executarReST("NumeroCarregamentoPorLote/ValidarCarregamento", { NumeroTransportePedido: _numeroCarregamentoPorLote.NumeroCarregamento.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _numeroCarregamentoPorLote.NumeroCarregamento.val(retorno.Data.Carregamento);

                exibirMensagem(tipoMensagem.ok, "Sucesso", "Carregamento Validado");
            }
            else if (retorno.Data == false)
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}