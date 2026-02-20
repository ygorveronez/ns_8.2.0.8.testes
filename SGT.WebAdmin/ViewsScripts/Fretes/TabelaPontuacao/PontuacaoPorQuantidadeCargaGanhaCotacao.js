
/*
 * Declaração de Objetos Globais do Arquivo
 */

var _pontuacaoPorQuantidadeCargaGanhaCotacao;
var _gridPontuacaoPorQuantidadeCargaGanhaCotacao;


var PontuacaoPorQuantidadeCargaGanhaCotacao = function () {

    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridPontuacaoPorQuantidadeCargaGanhaCotacao, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.QuantidadeInicio = PropertyEntity({ val: ko.observable("0"), text: "*Quantidade de Carga Mês Inicial:", def: "0", getType: typesKnockout.int, required: true, configInt: { precision: 0, allowZero: true }, maxlength: 5 });
    this.QuantidadeFim = PropertyEntity({ val: ko.observable("0"), text: "*Quantidade de Carga Mês Final:", def: "0", required: true, getType: typesKnockout.int, configInt: { precision: 0, allowZero: true }, maxlength: 5 });
    this.Pontuacao = PropertyEntity({ val: ko.observable(""), text: "*Pontuação:", def: "", getType: typesKnockout.int, configInt: { precision: 0, allowZero: true }, required: true, maxlength: 10 });

    this.Adicionar = PropertyEntity({ eventClick: adicionarPontuacaoPorQuantidadeCargaGanhaCotacaoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarPontuacaoPorQuantidadeCargaGanhaCotacaoClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarPontuacaoPorQuantidadeCargaGanhaCotacaoClick, type: types.event, text: "Cancelar / Nova" });
    this.Excluir = PropertyEntity({ eventClick: excluirPontuacaoPorQuantidadeCargaGanhaCotacaoClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}


/*
 * Declaração das Funções de Inicialização
 */

function loadGridPontuacaoPorQuantidadeCargaGanhaCotacao() {
    var opcaoEditar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarPontuacaoPorQuantidadeCargaGanhaCotacaoClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };

    _gridPontuacaoPorQuantidadeCargaGanhaCotacao = new GridViewExportacao(_pontuacaoPorQuantidadeCargaGanhaCotacao.Pesquisar.idGrid, "PontuacaoPorQuantidadeCargaGanhaCotacao/Pesquisa", _pontuacaoPorQuantidadeCargaGanhaCotacao, menuOpcoes);
}

function loadPontuacaoPorQuantidadeCargaGanhaCotacao() {
    _pontuacaoPorQuantidadeCargaGanhaCotacao = new PontuacaoPorQuantidadeCargaGanhaCotacao();
    KoBindings(_pontuacaoPorQuantidadeCargaGanhaCotacao, "knockoutPontuacaoPorQuantidadeCargaGanhaCotacao");


    loadGridPontuacaoPorQuantidadeCargaGanhaCotacao();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarPontuacaoPorQuantidadeCargaGanhaCotacaoClick(e, sender) {
    Salvar(_pontuacaoPorQuantidadeCargaGanhaCotacao, "PontuacaoPorQuantidadeCargaGanhaCotacao/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                recarregarGridPontuacaoPorQuantidadeCargaGanhaCotacao();
                limparCamposPontuacaoPorQuantidadeCargaGanhaCotacao();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function atualizarPontuacaoPorQuantidadeCargaGanhaCotacaoClick(e, sender) {
    Salvar(_pontuacaoPorQuantidadeCargaGanhaCotacao, "PontuacaoPorQuantidadeCargaGanhaCotacao/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                recarregarGridPontuacaoPorQuantidadeCargaGanhaCotacao();
                limparCamposPontuacaoPorQuantidadeCargaGanhaCotacao();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function cancelarPontuacaoPorQuantidadeCargaGanhaCotacaoClick() {
    limparCamposPontuacaoPorQuantidadeCargaGanhaCotacao();
}

function editarPontuacaoPorQuantidadeCargaGanhaCotacaoClick(registroSelecionado) {
    limparCamposPontuacaoPorQuantidadeCargaGanhaCotacao();

    _pontuacaoPorQuantidadeCargaGanhaCotacao.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_pontuacaoPorQuantidadeCargaGanhaCotacao, "PontuacaoPorQuantidadeCargaGanhaCotacao/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            controlarBotoesPontuacaoPorQuantidadeCargaGanhaCotacaoHabilitados();
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function excluirPontuacaoPorQuantidadeCargaGanhaCotacaoClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse cadastro?", function () {
        ExcluirPorCodigo(_pontuacaoPorQuantidadeCargaGanhaCotacao, "PontuacaoPorQuantidadeCargaGanhaCotacao/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    recarregarGridPontuacaoPorQuantidadeCargaGanhaCotacao();
                    limparCamposPontuacaoPorQuantidadeCargaGanhaCotacao();
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

function controlarBotoesPontuacaoPorQuantidadeCargaGanhaCotacaoHabilitados() {
    var isEdicao = _pontuacaoPorQuantidadeCargaGanhaCotacao.Codigo.val() > 0;

    _pontuacaoPorQuantidadeCargaGanhaCotacao.Atualizar.visible(isEdicao);
    _pontuacaoPorQuantidadeCargaGanhaCotacao.Excluir.visible(isEdicao);
    _pontuacaoPorQuantidadeCargaGanhaCotacao.Adicionar.visible(!isEdicao);
}

function limparCamposPontuacaoPorQuantidadeCargaGanhaCotacao() {
    LimparCampos(_pontuacaoPorQuantidadeCargaGanhaCotacao);
    controlarBotoesPontuacaoPorQuantidadeCargaGanhaCotacaoHabilitados();
}

function recarregarGridPontuacaoPorQuantidadeCargaGanhaCotacao() {
    _gridPontuacaoPorQuantidadeCargaGanhaCotacao.CarregarGrid();
}