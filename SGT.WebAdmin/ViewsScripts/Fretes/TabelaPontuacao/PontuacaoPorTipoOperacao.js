
/*
 * Declaração de Objetos Globais do Arquivo
 */

var _pontuacaoPorTipoOperacao;
var _gridPontuacaoPorTipoOperacao;


var PontuacaoPorTipoOperacao = function () {

    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridPontuacaoPorTipoOperacao, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0),  required: true, text: "*Tipo de Operação:", issue: 121, idBtnSearch: guid(), enable: ko.observable(true), destinatarioObrigatorio: false });

    this.Pontuacao = PropertyEntity({ val: ko.observable(""), text: "*Pontuação:", def: "", getType: typesKnockout.int, configInt: { precision: 0, allowZero: true }, required: true, maxlength: 10 });

    this.Adicionar = PropertyEntity({ eventClick: adicionarPontuacaoPorTipoOperacaoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarPontuacaoPorTipoOperacaoClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarPontuacaoPorTipoOperacaoClick, type: types.event, text: "Cancelar / Nova" });
    this.Excluir = PropertyEntity({ eventClick: excluirPontuacaoPorTipoOperacaoClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}


/*
 * Declaração das Funções de Inicialização
 */

function loadGridPontuacaoPorTipoOperacao() {
    var opcaoEditar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarPontuacaoPorTipoOperacaoClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };

    _gridPontuacaoPorTipoOperacao = new GridViewExportacao(_pontuacaoPorTipoOperacao.Pesquisar.idGrid, "PontuacaoPorTipoOperacao/Pesquisa", _pontuacaoPorTipoOperacao, menuOpcoes);
    
}

function loadPontuacaoPorTipoOperacao() {
    _pontuacaoPorTipoOperacao = new PontuacaoPorTipoOperacao();
    KoBindings(_pontuacaoPorTipoOperacao, "knockoutPontuacaoPorTipoOperacao");

    new BuscarTiposOperacao(_pontuacaoPorTipoOperacao.TipoOperacao);

    loadGridPontuacaoPorTipoOperacao();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarPontuacaoPorTipoOperacaoClick(e, sender) {
    Salvar(_pontuacaoPorTipoOperacao, "PontuacaoPorTipoOperacao/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                recarregarGridPontuacaoPorTipoOperacao();
                limparCamposPontuacaoPorTipoOperacao();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function atualizarPontuacaoPorTipoOperacaoClick(e, sender) {
    Salvar(_pontuacaoPorTipoOperacao, "PontuacaoPorTipoOperacao/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                recarregarGridPontuacaoPorTipoOperacao();
                limparCamposPontuacaoPorTipoOperacao();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function cancelarPontuacaoPorTipoOperacaoClick() {
    limparCamposPontuacaoPorTipoOperacao();
}

function editarPontuacaoPorTipoOperacaoClick(registroSelecionado) {
    limparCamposPontuacaoPorTipoOperacao();

    _pontuacaoPorTipoOperacao.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_pontuacaoPorTipoOperacao, "PontuacaoPorTipoOperacao/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            controlarBotoesPontuacaoPorTipoOperacaoHabilitados();
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function excluirPontuacaoPorTipoOperacaoClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse cadastro?", function () {
        ExcluirPorCodigo(_pontuacaoPorTipoOperacao, "PontuacaoPorTipoOperacao/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    recarregarGridPontuacaoPorTipoOperacao();
                    limparCamposPontuacaoPorTipoOperacao();
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

function controlarBotoesPontuacaoPorTipoOperacaoHabilitados() {
    var isEdicao = _pontuacaoPorTipoOperacao.Codigo.val() > 0;

    _pontuacaoPorTipoOperacao.Atualizar.visible(isEdicao);
    _pontuacaoPorTipoOperacao.Excluir.visible(isEdicao);
    _pontuacaoPorTipoOperacao.Adicionar.visible(!isEdicao);
}

function limparCamposPontuacaoPorTipoOperacao() {
    LimparCampos(_pontuacaoPorTipoOperacao);
    controlarBotoesPontuacaoPorTipoOperacaoHabilitados();
}

function recarregarGridPontuacaoPorTipoOperacao() {
    _gridPontuacaoPorTipoOperacao.CarregarGrid();
}