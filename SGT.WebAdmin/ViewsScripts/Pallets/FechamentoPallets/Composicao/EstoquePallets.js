/// <reference path="../../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../../js/Global/CRUD.js" />
/// <reference path="../../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../../js/Global/Rest.js" />
/// <reference path="../../../../js/Global/Mensagem.js" />
/// <reference path="../../../../js/Global/Grid.js" />
/// <reference path="../../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../../js/plugin/datatables/jquery.dataTables.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _estoquePallets;
var _gridEstoquePallets;

var EstoquePallets = function () {
    // Codigo da entidade
    this.Grid = PropertyEntity({});
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
}

//*******EVENTOS*******
function LoadEstoquePallets() {
    _estoquePallets = new EstoquePallets();
    KoBindings(_estoquePallets, "knockoutComposicaoEstoquePallets");
    
    // Inicia busca
    GridEstoquePallets();
}

function incluirFechamentoEstoquePallets(dataGrid) {
    executarReST("FechamentoPalletsEstoquePallets/Atualizar", { Codigo: dataGrid.Codigo, Adicionar: true }, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Alterado com sucesso.");
                _gridEstoquePallets.CarregarGrid();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function removerFechamentoEstoquePallets(dataGrid) {
    executarReST("FechamentoPalletsEstoquePallets/Atualizar", { Codigo: dataGrid.Codigo, Adicionar: false }, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Alterado com sucesso.");
                _gridEstoquePallets.CarregarGrid();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}


//*******MÉTODOS*******
function GridEstoquePallets() {
    //-- Grid
    // Opcoes
    var incluir = { descricao: "Incluir", id: guid(), evento: "onclick", metodo: incluirFechamentoEstoquePallets, visibilidade: visibilidadeComposicaoFechamentoIncluir };
    var remover = { descricao: "Remover", id: guid(), evento: "onclick", metodo: removerFechamentoEstoquePallets, visibilidade: visibilidadeComposicaoFechamentoRemover };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.list,
        tamanho: "7",
        descricao: "Opções",
        opcoes: [incluir, remover]
    };

    if (_fechamentoPallets.Situacao.val() != EnumSituacaoFechamentoPallets.Aberto)
        menuOpcoes = null;

    // Inicia Grid de busca
    if (_gridEstoquePallets != null)
        _gridEstoquePallets.Destroy();
    _gridEstoquePallets = new GridView(_estoquePallets.Grid.id, "FechamentoPalletsEstoquePallets/Pesquisa", _estoquePallets, menuOpcoes);
    _gridEstoquePallets.CarregarGrid();
}

function VisualizarEstoquePallets(dados) {
    _estoquePallets.Codigo.val(dados.Codigo);
    GridEstoquePallets();
}

function LimparCamposEstoquePallets() {
    LimparCampos(_estoquePallets);
    _gridEstoquePallets.CarregarGrid();
}