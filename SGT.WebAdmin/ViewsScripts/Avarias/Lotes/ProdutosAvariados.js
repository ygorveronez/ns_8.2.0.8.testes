/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Consultas/MotivoAvaria.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/MotivoRemocaoLote.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _produtosAvariados;
var _gridProdutosAvariados;
var $modalProdutosAvariados;

var ProdutosAvariados = function () {
    this.Codigo = PropertyEntity({ type: types.map, getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Solicitacao = PropertyEntity({ type: types.map, getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Lote = PropertyEntity({ type: types.map, getType: typesKnockout.int, val: ko.observable(0), def: 0 });

    this.Motivo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motivo:", idBtnSearch: guid(), required: true });
    this.Observacao = PropertyEntity({ text: "Observação:", val: ko.observable("") });

    this.Atualizar = PropertyEntity({ eventClick: atualizarProdutosAvariadosClick, type: types.event, text: "Remover", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarProdutosAvariadosClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });

    this.Produtos = PropertyEntity({ type: types.local, text: ko.observable(""), val: ko.observable(""), idGrid: guid() });

    this.Descricao = PropertyEntity({ text: "Descrição:", val: ko.observable("") });
    this.CodigoEmbarcador = PropertyEntity({ text: "Código:", val: ko.observable("") });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridProdutosAvariados.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
}


//*******EVENTOS*******
function loadProdutosAvariados() {
    _produtosAvariados = new ProdutosAvariados();
    KoBindings(_produtosAvariados, "knockoutProdutosAvariados");

    new BuscarMotivoRemocaoLote(_produtosAvariados.Motivo);

    $modalProdutosAvariados = $("#divModalProdutosAvariados");
    $modalProdutosAvariados.on('hidden.bs.modal', function () {
        LimparCampos(_produtosAvariados);
    });
}

function atualizarProdutosAvariadosClick(e, sender) {
    Salvar(_produtosAvariados, "Lotes/RemoverProduto", function (arg) {
        if (arg.Success) {
            _gridProdutosAvariados.CarregarGrid();
            _gridAvarias.CarregarGrid();
            LimpaRemocaoProdutoLote();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", arg.Msg);
        }
    }, sender, exibirCamposObrigatorios);
}

function cancelarProdutosAvariadosClick(e, sender) {
    LimparCampoEntity(_produtosAvariados.Motivo);
    _produtosAvariados.Observacao.val(_produtosAvariados.Observacao.def);
    _produtosAvariados.Codigo.val(_produtosAvariados.Codigo.def);
}

function removerProduto(dataGrid) {
    _produtosAvariados.Codigo.val(parseInt(dataGrid.Codigo));
}

function adicionarProduto(dataGrid) {
    // Adiciona codigo da solicitacao
    dataGrid.Solicitacao = _produtosAvariados.Solicitacao.val();
    executarReST("Lotes/ReadionarProduto", dataGrid, function (arg) {
        if (arg.Success) {
            _gridProdutosAvariados.CarregarGrid();
            _gridAvarias.CarregarGrid();
            LimpaRemocaoProdutoLote();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", arg.Msg);
        }
    });
}

function visivelRemoverProduto(dataGrid) {
    return dataGrid.RemovidoLote == false;
}

function visivelAdicionarProduto(dataGrid) {
    return dataGrid.RemovidoLote == true;
}


//*******MÉTODOS*******
function buscarProdutosAvariados(cb) {
    var adicionar = {
        descricao: "Remover",
        id: guid(),
        metodo: removerProduto,
        visibilidade: visivelRemoverProduto,
    };
    var remover = {
        descricao: "Adicionar",
        id: guid(),
        metodo: adicionarProduto,
        visibilidade: visivelAdicionarProduto,
    };

    var menuOpcoes = {
        tipo: TypeOptionMenu.list,
        descricao: "Opções",
        tamanho: 9,
        opcoes: [
            adicionar,
            remover
        ]
    };

    // Cria a nova tabela
    _gridProdutosAvariados = new GridView(_produtosAvariados.Produtos.idGrid, "Lotes/PesquisaProdutos", _produtosAvariados, menuOpcoes);
    _gridProdutosAvariados.CarregarGrid(cb);
}

function ManutencaoAvaria(codSolicitacao, codLote) {
    _produtosAvariados.Produtos.text("Produtos da Avaria");

    _produtosAvariados.Solicitacao.val(codSolicitacao);
    _produtosAvariados.Lote.val(codLote);

    buscarProdutosAvariados(function () {
        // Abre o modal somente quando a grid terminou de carregar
        $modalProdutosAvariados.modal('show');

        $modalProdutosAvariados.one('hidden.bs.modal', function () {
            LimpaRemocaoProdutoLote();
        });
    });
}

function ManutencaoLote(codLote) {
    ManutencaoAvaria(0, codLote);

    // Sobscreve título
    _produtosAvariados.Produtos.text("Produtos do Lote");
}

function LimpaRemocaoProdutoLote(){
    _produtosAvariados.Codigo.val(0);
    _produtosAvariados.Observacao.val(_produtosAvariados.Observacao.def);
    LimparCampoEntity(_produtosAvariados.Motivo);
}