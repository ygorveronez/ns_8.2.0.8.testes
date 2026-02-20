
/*
 * Declaração de Objetos Globais do Arquivo
 */

var _pontuacaoPorTempoAtividade;
var _gridPontuacaoPorTempoAtividade;


var PontuacaoPorTempoAtividade = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.AnoInicio = PropertyEntity({ val: ko.observable("0"), text: "*Quantidade de Anos Inicial:", def: "0", getType: typesKnockout.int, required: true, configInt: { precision: 0, allowZero: true }, maxlength: 2 });
    this.AnoFim = PropertyEntity({ val: ko.observable("0"), text: "*Quantidade de Anos Final:", def: "0", required: true, getType: typesKnockout.int, configInt: { precision: 0, allowZero: true }, maxlength: 2 });
    this.Pontuacao = PropertyEntity({ val: ko.observable(""), text: "*Pontuação:", def: "", getType: typesKnockout.int, configInt: { precision: 0, allowZero: true }, required: true, maxlength: 10 });

    this.Adicionar = PropertyEntity({ eventClick: adicionarPontuacaoPorTempoAtividadeClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarPontuacaoPorTempoAtividadeClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarPontuacaoPorTempoAtividadeClick, type: types.event, text: "Cancelar / Nova" });
    this.Excluir = PropertyEntity({ eventClick: excluirPontuacaoPorTempoAtividadeClick, type: types.event, text: "Excluir", visible: ko.observable(false) });

    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridPontuacaoPorTempoAtividade, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}


/*
 * Declaração das Funções de Inicialização
 */

function loadGridPontuacaoPorTempoAtividade() {
    var opcaoEditar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarPontuacaoPorTempoAtividadeClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };

    _gridPontuacaoPorTempoAtividade = new GridViewExportacao(_pontuacaoPorTempoAtividade.Pesquisar.idGrid, "PontuacaoPorTempoAtividade/Pesquisa", _pontuacaoPorTempoAtividade, menuOpcoes);
    _gridPontuacaoPorTempoAtividade.CarregarGrid();
}

function loadPontuacaoPorTempoAtividade() {
    _pontuacaoPorTempoAtividade = new PontuacaoPorTempoAtividade();
    KoBindings(_pontuacaoPorTempoAtividade, "knockoutPontuacaoPorTempoAtividade");

    HeaderAuditoria("PontuacaoPorTempoAtividade", _pontuacaoPorTempoAtividade);

    loadGridPontuacaoPorTempoAtividade();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarPontuacaoPorTempoAtividadeClick(e, sender) {
    Salvar(_pontuacaoPorTempoAtividade, "PontuacaoPorTempoAtividade/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                recarregarGridPontuacaoPorTempoAtividade();
                limparCamposPontuacaoPorTempoAtividade();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function atualizarPontuacaoPorTempoAtividadeClick(e, sender) {
    Salvar(_pontuacaoPorTempoAtividade, "PontuacaoPorTempoAtividade/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                recarregarGridPontuacaoPorTempoAtividade();
                limparCamposPontuacaoPorTempoAtividade();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function cancelarPontuacaoPorTempoAtividadeClick() {
    limparCamposPontuacaoPorTempoAtividade();
}

function editarPontuacaoPorTempoAtividadeClick(registroSelecionado) {
    limparCamposPontuacaoPorTempoAtividade();

    _pontuacaoPorTempoAtividade.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_pontuacaoPorTempoAtividade, "PontuacaoPorTempoAtividade/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            controlarBotoesPontuacaoPorTempoAtividadeHabilitados();
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function excluirPontuacaoPorTempoAtividadeClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse cadastro?", function () {
        ExcluirPorCodigo(_pontuacaoPorTempoAtividade, "PontuacaoPorTempoAtividade/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    recarregarGridPontuacaoPorTempoAtividade();
                    limparCamposPontuacaoPorTempoAtividade();
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

function controlarBotoesPontuacaoPorTempoAtividadeHabilitados() {
    var isEdicao = _pontuacaoPorTempoAtividade.Codigo.val() > 0;

    _pontuacaoPorTempoAtividade.Atualizar.visible(isEdicao);
    _pontuacaoPorTempoAtividade.Excluir.visible(isEdicao);
    _pontuacaoPorTempoAtividade.Adicionar.visible(!isEdicao);
}

function limparCamposPontuacaoPorTempoAtividade() {
    LimparCampos(_pontuacaoPorTempoAtividade);
    controlarBotoesPontuacaoPorTempoAtividadeHabilitados();
}

function recarregarGridPontuacaoPorTempoAtividade() {
    _gridPontuacaoPorTempoAtividade.CarregarGrid();
}