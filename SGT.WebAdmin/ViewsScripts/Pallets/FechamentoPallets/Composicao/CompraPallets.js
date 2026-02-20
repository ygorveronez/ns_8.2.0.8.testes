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

var _compraPallets;
var _gridCompraPallets;

var CompraPallets = function () {
    // Codigo da entidade
    this.Grid = PropertyEntity({});
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
}

//*******EVENTOS*******
function LoadCompraPallets() {
    _compraPallets = new CompraPallets();
    KoBindings(_compraPallets, "knockoutComposicaoCompraPallets");

    // Inicia busca
    GridCompraPallets();
}

function incluirFechamentoCompraPallets(dataGrid) {
    executarReST("FechamentoPalletsCompraPallets/Atualizar", { Codigo: dataGrid.Codigo, Adicionar: true }, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Alterado com sucesso.");
                _gridCompraPallets.CarregarGrid();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function removerFechamentoCompraPallets(dataGrid) {
    executarReST("FechamentoPalletsCompraPallets/Atualizar", { Codigo: dataGrid.Codigo, Adicionar: false }, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Alterado com sucesso.");
                _gridCompraPallets.CarregarGrid();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}


//*******MÉTODOS*******
function GridCompraPallets() {
    //-- Grid
    // Opcoes
    var incluir = { descricao: "Incluir", id: guid(), evento: "onclick", metodo: incluirFechamentoCompraPallets, visibilidade: visibilidadeComposicaoFechamentoIncluir };
    var remover = { descricao: "Remover", id: guid(), evento: "onclick", metodo: removerFechamentoCompraPallets, visibilidade: visibilidadeComposicaoFechamentoRemover };

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
    if (_gridCompraPallets != null)
        _gridCompraPallets.Destroy();
    _gridCompraPallets = new GridView(_compraPallets.Grid.id, "FechamentoPalletsCompraPallets/Pesquisa", _compraPallets, menuOpcoes);
    _gridCompraPallets.CarregarGrid();
}

function VisualizarCompraPallets(dados) {
    _compraPallets.Codigo.val(dados.Codigo);
    GridCompraPallets();
}

function LimparCamposCompraPallets() {
    LimparCampos(_compraPallets);
    _gridCompraPallets.CarregarGrid();
}