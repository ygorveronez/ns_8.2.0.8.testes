
/*
 * Declaração de Objetos Globais do Arquivo
 */

var _tempoEsperaPorPontuacao;
var _gridTempoEsperaPorPontuacao;

/*
 * Declaração das Classes
 */

var TempoEsperaPorPontuacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.PontuacaoInicial = PropertyEntity({ val: ko.observable("0"), text: "*Pontuação Inicial:", def: "0", getType: typesKnockout.int, required: true, configInt: { precision: 0, allowZero: true, thousands: "" }, maxlength: 5 });
    this.PontuacaoFinal = PropertyEntity({ val: ko.observable("0"), text: "*Pontuação Final:", def: "0", getType: typesKnockout.int, required: true, configInt: { precision: 0, allowZero: true, thousands: "" }, maxlength: 5 });
    this.TempoEsperaEmMinutos = PropertyEntity({ val: ko.observable(""), text: "*Tempo de Espera (Em Minutos):", def: "", getType: typesKnockout.int, required: true, configInt: { precision: 0, allowZero: true, thousands: "" }, maxlength: 5 });

    this.Adicionar = PropertyEntity({ eventClick: adicionarTempoEsperaPorPontuacaoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarTempoEsperaPorPontuacaoClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarTempoEsperaPorPontuacaoClick, type: types.event, text: "Cancelar / Novo" });
    this.Excluir = PropertyEntity({ eventClick: excluirTempoEsperaPorPontuacaoClick, type: types.event, text: "Excluir", visible: ko.observable(false) });

    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridTempoEsperaPorPontuacao, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}


/*
 * Declaração das Funções de Inicialização
 */

function loadGridTempoEsperaPorPontuacao() {
    var opcaoEditar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarTempoEsperaPorPontuacaoClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };

    _gridTempoEsperaPorPontuacao = new GridViewExportacao(_tempoEsperaPorPontuacao.Pesquisar.idGrid, "TempoEsperaPorPontuacao/Pesquisa", _tempoEsperaPorPontuacao, menuOpcoes);
    _gridTempoEsperaPorPontuacao.CarregarGrid();
}

function loadTempoEsperaPorPontuacao() {
    _tempoEsperaPorPontuacao = new TempoEsperaPorPontuacao();
    KoBindings(_tempoEsperaPorPontuacao, "knockoutTempoEsperaPorPontuacao");

    HeaderAuditoria("TempoEsperaPorPontuacao", _tempoEsperaPorPontuacao);

    loadGridTempoEsperaPorPontuacao();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarTempoEsperaPorPontuacaoClick(e, sender) {
    Salvar(_tempoEsperaPorPontuacao, "TempoEsperaPorPontuacao/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                recarregarGridTempoEsperaPorPontuacao();
                limparCamposTempoEsperaPorPontuacao();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function atualizarTempoEsperaPorPontuacaoClick(e, sender) {
    Salvar(_tempoEsperaPorPontuacao, "TempoEsperaPorPontuacao/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                recarregarGridTempoEsperaPorPontuacao();
                limparCamposTempoEsperaPorPontuacao();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function cancelarTempoEsperaPorPontuacaoClick() {
    limparCamposTempoEsperaPorPontuacao();
}

function editarTempoEsperaPorPontuacaoClick(registroSelecionado) {
    limparCamposTempoEsperaPorPontuacao();

    _tempoEsperaPorPontuacao.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_tempoEsperaPorPontuacao, "TempoEsperaPorPontuacao/BuscarPorCodigo", function (retorno) {
        if (retorno.Success)
            controlarBotoesTempoEsperaPorPontuacaoHabilitados();
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function excluirTempoEsperaPorPontuacaoClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse cadastro?", function () {
        ExcluirPorCodigo(_tempoEsperaPorPontuacao, "TempoEsperaPorPontuacao/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    recarregarGridTempoEsperaPorPontuacao();
                    limparCamposTempoEsperaPorPontuacao();
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

function controlarBotoesTempoEsperaPorPontuacaoHabilitados() {
    var isEdicao = _tempoEsperaPorPontuacao.Codigo.val() > 0;

    _tempoEsperaPorPontuacao.Atualizar.visible(isEdicao);
    _tempoEsperaPorPontuacao.Excluir.visible(isEdicao);
    _tempoEsperaPorPontuacao.Adicionar.visible(!isEdicao);
}

function limparCamposTempoEsperaPorPontuacao() {
    LimparCampos(_tempoEsperaPorPontuacao);
    controlarBotoesTempoEsperaPorPontuacaoHabilitados();
}

function recarregarGridTempoEsperaPorPontuacao() {
    _gridTempoEsperaPorPontuacao.CarregarGrid();
}