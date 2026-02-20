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

var _devolucao;
var _gridDevolucao;

var Devolucao = function () {
    // Codigo da entidade
    this.Grid = PropertyEntity({});
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
}

//*******EVENTOS*******
function LoadDevolucao() {
    _devolucao = new Devolucao();
    KoBindings(_devolucao, "knockoutComposicaoDevolucao");

    // Inicia busca
    GridDevolucao();
}

function incluirFechamentoDevolucao(dataGrid) {
    executarReST("FechamentoPalletsDevolucao/Atualizar", { Codigo: dataGrid.Codigo, Adicionar: true }, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Alterado com sucesso.");
                _gridDevolucao.CarregarGrid();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function removerFechamentoDevolucao(dataGrid) {
    executarReST("FechamentoPalletsDevolucao/Atualizar", { Codigo: dataGrid.Codigo, Adicionar: false }, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Alterado com sucesso.");
                _gridDevolucao.CarregarGrid();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}


//*******MÉTODOS*******
function GridDevolucao() {
    //-- Grid
    // Opcoes
    var incluir = { descricao: "Incluir", id: guid(), evento: "onclick", metodo: incluirFechamentoDevolucao, visibilidade: visibilidadeComposicaoFechamentoIncluir };
    var remover = { descricao: "Remover", id: guid(), evento: "onclick", metodo: removerFechamentoDevolucao, visibilidade: visibilidadeComposicaoFechamentoRemover };

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
    if (_gridDevolucao != null)
        _gridDevolucao.Destroy();
    _gridDevolucao = new GridView(_devolucao.Grid.id, "FechamentoPalletsDevolucao/Pesquisa", _devolucao, menuOpcoes);
    _gridDevolucao.CarregarGrid();
}

function VisualizarDevolucao(dados) {
    _devolucao.Codigo.val(dados.Codigo);
    GridDevolucao();
}

function LimparCamposDevolucao() {
    LimparCampos(_devolucao);
    _gridDevolucao.CarregarGrid();
}