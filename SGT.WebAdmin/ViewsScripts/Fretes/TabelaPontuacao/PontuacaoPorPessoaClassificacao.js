
/*
 * Declaração de Objetos Globais do Arquivo
 */

var _pontuacaoPorPessoaClassificacao;
var _gridPontuacaoPorPessoaClassificacao;

var PontuacaoPorPessoaClassificacao = function () {

    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridPontuacaoPorPessoaClassificacao, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.PessoaClassificacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Classificação de Cliente:", issue: 121, idBtnSearch: guid(), enable: ko.observable(true), destinatarioObrigatorio: false });

    this.Pontuacao = PropertyEntity({ val: ko.observable(""), text: "*Pontuação:", def: "", getType: typesKnockout.int, configInt: { precision: 0, allowZero: true }, required: true, maxlength: 10 });

    this.Adicionar = PropertyEntity({ eventClick: adicionarPontuacaoPorPessoaClassificacaoClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarPontuacaoPorPessoaClassificacaoClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarPontuacaoPorPessoaClassificacaoClick, type: types.event, text: "Cancelar / Nova" });
    this.Excluir = PropertyEntity({ eventClick: excluirPontuacaoPorPessoaClassificacaoClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}


/*
 * Declaração das Funções de Inicialização
 */

function loadGridPontuacaoPorPessoaClassificacao() {
    var opcaoEditar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarPontuacaoPorPessoaClassificacaoClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };

    _gridPontuacaoPorPessoaClassificacao = new GridViewExportacao(_pontuacaoPorPessoaClassificacao.Pesquisar.idGrid, "PontuacaoPorPessoaClassificacao/Pesquisa", _pontuacaoPorPessoaClassificacao, menuOpcoes);

}

function loadPontuacaoPorPessoaClassificacao() {
    _pontuacaoPorPessoaClassificacao = new PontuacaoPorPessoaClassificacao();
    KoBindings(_pontuacaoPorPessoaClassificacao, "knockoutPontuacaoPorPessoaClassificacao");

    new BuscarPessoaClassificacao(_pontuacaoPorPessoaClassificacao.PessoaClassificacao);

    loadGridPontuacaoPorPessoaClassificacao();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarPontuacaoPorPessoaClassificacaoClick(e, sender) {
    Salvar(_pontuacaoPorPessoaClassificacao, "PontuacaoPorPessoaClassificacao/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                recarregarGridPontuacaoPorPessoaClassificacao();
                limparCamposPontuacaoPorPessoaClassificacao();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function atualizarPontuacaoPorPessoaClassificacaoClick(e, sender) {
    Salvar(_pontuacaoPorPessoaClassificacao, "PontuacaoPorPessoaClassificacao/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                recarregarGridPontuacaoPorPessoaClassificacao();
                limparCamposPontuacaoPorPessoaClassificacao();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function cancelarPontuacaoPorPessoaClassificacaoClick() {
    limparCamposPontuacaoPorPessoaClassificacao();
}

function editarPontuacaoPorPessoaClassificacaoClick(registroSelecionado) {
    limparCamposPontuacaoPorPessoaClassificacao();

    _pontuacaoPorPessoaClassificacao.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_pontuacaoPorPessoaClassificacao, "PontuacaoPorPessoaClassificacao/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            controlarBotoesPontuacaoPorPessoaClassificacaoHabilitados();
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function excluirPontuacaoPorPessoaClassificacaoClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse cadastro?", function () {
        ExcluirPorCodigo(_pontuacaoPorPessoaClassificacao, "PontuacaoPorPessoaClassificacao/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    recarregarGridPontuacaoPorPessoaClassificacao();
                    limparCamposPontuacaoPorPessoaClassificacao();
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

function controlarBotoesPontuacaoPorPessoaClassificacaoHabilitados() {
    var isEdicao = _pontuacaoPorPessoaClassificacao.Codigo.val() > 0;

    _pontuacaoPorPessoaClassificacao.Atualizar.visible(isEdicao);
    _pontuacaoPorPessoaClassificacao.Excluir.visible(isEdicao);
    _pontuacaoPorPessoaClassificacao.Adicionar.visible(!isEdicao);
}

function limparCamposPontuacaoPorPessoaClassificacao() {
    LimparCampos(_pontuacaoPorPessoaClassificacao);
    controlarBotoesPontuacaoPorPessoaClassificacaoHabilitados();
}

function recarregarGridPontuacaoPorPessoaClassificacao() {
    _gridPontuacaoPorPessoaClassificacao.CarregarGrid();
}