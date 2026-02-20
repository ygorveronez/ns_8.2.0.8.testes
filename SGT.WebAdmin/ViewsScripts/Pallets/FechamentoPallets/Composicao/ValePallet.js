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

var _valePallet;
var _gridValePallet;

var ValePallet = function () {
    // Codigo da entidade
    this.Grid = PropertyEntity({});
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
}

//*******EVENTOS*******
function LoadValePallet() {
    _valePallet = new ValePallet();
    KoBindings(_valePallet, "knockoutComposicaoValePallet");

    // Inicia busca
    GridValePallet();
}

function incluirFechamentoValePallet(dataGrid) {
    executarReST("FechamentoPalletsValePallet/Atualizar", { Codigo: dataGrid.Codigo, Adicionar: true }, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Alterado com sucesso.");
                _gridValePallet.CarregarGrid();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function removerFechamentoValePallet(dataGrid) {
    executarReST("FechamentoPalletsValePallet/Atualizar", { Codigo: dataGrid.Codigo, Adicionar: false }, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Alterado com sucesso.");
                _gridValePallet.CarregarGrid();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}


//*******MÉTODOS*******
function GridValePallet() {
    //-- Grid
    // Opcoes
    var incluir = { descricao: "Incluir", id: guid(), evento: "onclick", metodo: incluirFechamentoValePallet, visibilidade: visibilidadeComposicaoFechamentoIncluir };
    var remover = { descricao: "Remover", id: guid(), evento: "onclick", metodo: removerFechamentoValePallet, visibilidade: visibilidadeComposicaoFechamentoRemover };

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
    if (_gridValePallet != null)
        _gridValePallet.Destroy();
    _gridValePallet = new GridView(_valePallet.Grid.id, "FechamentoPalletsValePallet/Pesquisa", _valePallet, menuOpcoes);
    _gridValePallet.CarregarGrid();
}

function VisualizarValePallet(dados) {
    _valePallet.Codigo.val(dados.Codigo);
    GridValePallet();
}

function LimparCamposValePallet() {
    LimparCampos(_valePallet);
    _gridValePallet.CarregarGrid();
}