
/*
 * Declaração de Objetos Globais do Arquivo
 */

var _pontuacaoPorModeloCarroceria;
var _gridPontuacaoPorModeloCarroceria;


var PontuacaoPorModeloCarroceria = function () {

    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridPontuacaoPorModeloCarroceria, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.ModeloCarroceria = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Modelo de Carroceria:", issue: 121, idBtnSearch: guid(), enable: ko.observable(true), destinatarioObrigatorio: false });

    this.Pontuacao = PropertyEntity({ val: ko.observable(""), text: "*Pontuação:", def: "", getType: typesKnockout.int, configInt: { precision: 0, allowZero: true }, required: true, maxlength: 10 });

    this.Adicionar = PropertyEntity({ eventClick: adicionarPontuacaoPorModeloCarroceriaClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarPontuacaoPorModeloCarroceriaClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarPontuacaoPorModeloCarroceriaClick, type: types.event, text: "Cancelar / Nova" });
    this.Excluir = PropertyEntity({ eventClick: excluirPontuacaoPorModeloCarroceriaClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}


/*
 * Declaração das Funções de Inicialização
 */

function loadGridPontuacaoPorModeloCarroceria() {
    var opcaoEditar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarPontuacaoPorModeloCarroceriaClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };

    _gridPontuacaoPorModeloCarroceria = new GridViewExportacao(_pontuacaoPorModeloCarroceria.Pesquisar.idGrid, "PontuacaoPorModeloCarroceria/Pesquisa", _pontuacaoPorModeloCarroceria, menuOpcoes);

}

function loadPontuacaoPorModeloCarroceria() {
    _pontuacaoPorModeloCarroceria = new PontuacaoPorModeloCarroceria();
    KoBindings(_pontuacaoPorModeloCarroceria, "knockoutPontuacaoPorModeloCarroceria");

    new BuscarModelosCarroceria(_pontuacaoPorModeloCarroceria.ModeloCarroceria);

    loadGridPontuacaoPorModeloCarroceria();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarPontuacaoPorModeloCarroceriaClick(e, sender) {
    Salvar(_pontuacaoPorModeloCarroceria, "PontuacaoPorModeloCarroceria/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                recarregarGridPontuacaoPorModeloCarroceria();
                limparCamposPontuacaoPorModeloCarroceria();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function atualizarPontuacaoPorModeloCarroceriaClick(e, sender) {
    Salvar(_pontuacaoPorModeloCarroceria, "PontuacaoPorModeloCarroceria/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                recarregarGridPontuacaoPorModeloCarroceria();
                limparCamposPontuacaoPorModeloCarroceria();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function cancelarPontuacaoPorModeloCarroceriaClick() {
    limparCamposPontuacaoPorModeloCarroceria();
}

function editarPontuacaoPorModeloCarroceriaClick(registroSelecionado) {
    limparCamposPontuacaoPorModeloCarroceria();

    _pontuacaoPorModeloCarroceria.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_pontuacaoPorModeloCarroceria, "PontuacaoPorModeloCarroceria/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            controlarBotoesPontuacaoPorModeloCarroceriaHabilitados();
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function excluirPontuacaoPorModeloCarroceriaClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse cadastro?", function () {
        ExcluirPorCodigo(_pontuacaoPorModeloCarroceria, "PontuacaoPorModeloCarroceria/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    recarregarGridPontuacaoPorModeloCarroceria();
                    limparCamposPontuacaoPorModeloCarroceria();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }, null);
    });
}

/*
 * Declaração das Funções
 */

function controlarBotoesPontuacaoPorModeloCarroceriaHabilitados() {
    var isEdicao = _pontuacaoPorModeloCarroceria.Codigo.val() > 0;

    _pontuacaoPorModeloCarroceria.Atualizar.visible(isEdicao);
    _pontuacaoPorModeloCarroceria.Excluir.visible(isEdicao);
    _pontuacaoPorModeloCarroceria.Adicionar.visible(!isEdicao);
}

function limparCamposPontuacaoPorModeloCarroceria() {
    LimparCampos(_pontuacaoPorModeloCarroceria);
    controlarBotoesPontuacaoPorModeloCarroceriaHabilitados();
}

function recarregarGridPontuacaoPorModeloCarroceria() {
    _gridPontuacaoPorModeloCarroceria.CarregarGrid();
}