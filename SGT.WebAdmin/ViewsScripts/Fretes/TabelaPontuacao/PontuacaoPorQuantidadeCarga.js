
/*
 * Declaração de Objetos Globais do Arquivo
 */

var _pontuacaoPorQuantidadeCarga;
var _gridPontuacaoPorQuantidadeCarga;


var PontuacaoPorQuantidadeCarga = function () {

    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridPontuacaoPorQuantidadeCarga, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.QuantidadeInicio = PropertyEntity({ val: ko.observable("0"), text: "*Quantidade de Carga Mês Inicial:", def: "0", getType: typesKnockout.int, required: true, configInt: { precision: 0, allowZero: true }, maxlength: 5 });
    this.QuantidadeFim = PropertyEntity({ val: ko.observable("0"), text: "*Quantidade de Carga Mês Final:", def: "0", required: true, getType: typesKnockout.int, configInt: { precision: 0, allowZero: true }, maxlength: 5 });
    this.Pontuacao = PropertyEntity({ val: ko.observable(""), text: "*Pontuação:", def: "", getType: typesKnockout.int, configInt: { precision: 0, allowZero: true }, required: true, maxlength: 10 });

    this.Adicionar = PropertyEntity({ eventClick: adicionarPontuacaoPorQuantidadeCargaClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarPontuacaoPorQuantidadeCargaClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarPontuacaoPorQuantidadeCargaClick, type: types.event, text: "Cancelar / Nova" });
    this.Excluir = PropertyEntity({ eventClick: excluirPontuacaoPorQuantidadeCargaClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}


/*
 * Declaração das Funções de Inicialização
 */

function loadGridPontuacaoPorQuantidadeCarga() {
    var opcaoEditar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarPontuacaoPorQuantidadeCargaClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };

    _gridPontuacaoPorQuantidadeCarga = new GridViewExportacao(_pontuacaoPorQuantidadeCarga.Pesquisar.idGrid, "PontuacaoPorQuantidadeCarga/Pesquisa", _pontuacaoPorQuantidadeCarga, menuOpcoes);
}

function loadPontuacaoPorQuantidadeCarga() {
    _pontuacaoPorQuantidadeCarga = new PontuacaoPorQuantidadeCarga();
    KoBindings(_pontuacaoPorQuantidadeCarga, "knockoutPontuacaoPorQuantidadeCarga");


    loadGridPontuacaoPorQuantidadeCarga();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarPontuacaoPorQuantidadeCargaClick(e, sender) {
    Salvar(_pontuacaoPorQuantidadeCarga, "PontuacaoPorQuantidadeCarga/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                recarregarGridPontuacaoPorQuantidadeCarga();
                limparCamposPontuacaoPorQuantidadeCarga();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function atualizarPontuacaoPorQuantidadeCargaClick(e, sender) {
    Salvar(_pontuacaoPorQuantidadeCarga, "PontuacaoPorQuantidadeCarga/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                recarregarGridPontuacaoPorQuantidadeCarga();
                limparCamposPontuacaoPorQuantidadeCarga();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function cancelarPontuacaoPorQuantidadeCargaClick() {
    limparCamposPontuacaoPorQuantidadeCarga();
}

function editarPontuacaoPorQuantidadeCargaClick(registroSelecionado) {
    limparCamposPontuacaoPorQuantidadeCarga();

    _pontuacaoPorQuantidadeCarga.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_pontuacaoPorQuantidadeCarga, "PontuacaoPorQuantidadeCarga/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            controlarBotoesPontuacaoPorQuantidadeCargaHabilitados();
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function excluirPontuacaoPorQuantidadeCargaClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse cadastro?", function () {
        ExcluirPorCodigo(_pontuacaoPorQuantidadeCarga, "PontuacaoPorQuantidadeCarga/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    recarregarGridPontuacaoPorQuantidadeCarga();
                    limparCamposPontuacaoPorQuantidadeCarga();
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

function controlarBotoesPontuacaoPorQuantidadeCargaHabilitados() {
    var isEdicao = _pontuacaoPorQuantidadeCarga.Codigo.val() > 0;

    _pontuacaoPorQuantidadeCarga.Atualizar.visible(isEdicao);
    _pontuacaoPorQuantidadeCarga.Excluir.visible(isEdicao);
    _pontuacaoPorQuantidadeCarga.Adicionar.visible(!isEdicao);
}

function limparCamposPontuacaoPorQuantidadeCarga() {
    LimparCampos(_pontuacaoPorQuantidadeCarga);
    controlarBotoesPontuacaoPorQuantidadeCargaHabilitados();
}

function recarregarGridPontuacaoPorQuantidadeCarga() {
    _gridPontuacaoPorQuantidadeCarga.CarregarGrid();
}