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

var _devolucaoValePallet;
var _gridDevolucaoValePallet;

var DevolucaoValePallet = function () {
    // Codigo da entidade
    this.Grid = PropertyEntity({});
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
}

//*******EVENTOS*******
function LoadDevolucaoValePallet() {
    _devolucaoValePallet = new DevolucaoValePallet();
    KoBindings(_devolucaoValePallet, "knockoutComposicaoDevolucaoValePallet");

    // Inicia busca
    GridDevolucaoValePallet();
}

function incluirFechamentoDevolucaoValePallet(dataGrid) {
    executarReST("FechamentoPalletsDevolucaoValePallet/Atualizar", { Codigo: dataGrid.Codigo, Adicionar: true }, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Alterado com sucesso.");
                _gridDevolucaoValePallet.CarregarGrid();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function removerFechamentoDevolucaoValePallet(dataGrid) {
    executarReST("FechamentoPalletsDevolucaoValePallet/Atualizar", { Codigo: dataGrid.Codigo, Adicionar: false }, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Alterado com sucesso.");
                _gridDevolucaoValePallet.CarregarGrid();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}


//*******MÉTODOS*******
function GridDevolucaoValePallet() {
    //-- Grid
    // Opcoes
    var incluir = { descricao: "Incluir", id: guid(), evento: "onclick", metodo: incluirFechamentoDevolucaoValePallet, visibilidade: visibilidadeComposicaoFechamentoIncluir };
    var remover = { descricao: "Remover", id: guid(), evento: "onclick", metodo: removerFechamentoDevolucaoValePallet, visibilidade: visibilidadeComposicaoFechamentoRemover };

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
    if (_gridDevolucaoValePallet != null)
        _gridDevolucaoValePallet.Destroy();
    _gridDevolucaoValePallet = new GridView(_devolucaoValePallet.Grid.id, "FechamentoPalletsDevolucaoValePallet/Pesquisa", _devolucaoValePallet, menuOpcoes);
    _gridDevolucaoValePallet.CarregarGrid();
}

function VisualizarDevolucaoValePallet(dados) {
    _devolucaoValePallet.Codigo.val(dados.Codigo);
    GridDevolucaoValePallet();
}

function LimparCamposDevolucaoValePallet() {
    LimparCampos(_devolucaoValePallet);
    _gridDevolucaoValePallet.CarregarGrid();
}