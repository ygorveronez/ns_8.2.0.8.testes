
/*
 * Declaração de Objetos Globais do Arquivo
 */

var _pontuacaoPorTipoCarga;
var _gridPontuacaoPorTipoCarga;


var PontuacaoPorTipoCarga = function () {

    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridPontuacaoPorTipoCarga, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.TipoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Tipo de Carga:", issue: 121, idBtnSearch: guid(), enable: ko.observable(true), destinatarioObrigatorio: false });

    this.Pontuacao = PropertyEntity({ val: ko.observable(""), text: "*Pontuação:", def: "", getType: typesKnockout.int, configInt: { precision: 0, allowZero: true }, required: true, maxlength: 10 });

    this.Adicionar = PropertyEntity({ eventClick: adicionarPontuacaoPorTipoCargaClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarPontuacaoPorTipoCargaClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarPontuacaoPorTipoCargaClick, type: types.event, text: "Cancelar / Nova" });
    this.Excluir = PropertyEntity({ eventClick: excluirPontuacaoPorTipoCargaClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}


/*
 * Declaração das Funções de Inicialização
 */

function loadGridPontuacaoPorTipoCarga() {
    var opcaoEditar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarPontuacaoPorTipoCargaClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };

    _gridPontuacaoPorTipoCarga = new GridViewExportacao(_pontuacaoPorTipoCarga.Pesquisar.idGrid, "PontuacaoPorTipoCarga/Pesquisa", _pontuacaoPorTipoCarga, menuOpcoes);

}

function loadPontuacaoPorTipoCarga() {
    _pontuacaoPorTipoCarga = new PontuacaoPorTipoCarga();
    KoBindings(_pontuacaoPorTipoCarga, "knockoutPontuacaoPorTipoCarga");

    new BuscarTiposdeCarga(_pontuacaoPorTipoCarga.TipoCarga);

    loadGridPontuacaoPorTipoCarga();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarPontuacaoPorTipoCargaClick(e, sender) {
    Salvar(_pontuacaoPorTipoCarga, "PontuacaoPorTipoCarga/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                recarregarGridPontuacaoPorTipoCarga();
                limparCamposPontuacaoPorTipoCarga();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function atualizarPontuacaoPorTipoCargaClick(e, sender) {
    Salvar(_pontuacaoPorTipoCarga, "PontuacaoPorTipoCarga/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                recarregarGridPontuacaoPorTipoCarga();
                limparCamposPontuacaoPorTipoCarga();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function cancelarPontuacaoPorTipoCargaClick() {
    limparCamposPontuacaoPorTipoCarga();
}

function editarPontuacaoPorTipoCargaClick(registroSelecionado) {
    limparCamposPontuacaoPorTipoCarga();

    _pontuacaoPorTipoCarga.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_pontuacaoPorTipoCarga, "PontuacaoPorTipoCarga/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            controlarBotoesPontuacaoPorTipoCargaHabilitados();
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function excluirPontuacaoPorTipoCargaClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse cadastro?", function () {
        ExcluirPorCodigo(_pontuacaoPorTipoCarga, "PontuacaoPorTipoCarga/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    recarregarGridPontuacaoPorTipoCarga();
                    limparCamposPontuacaoPorTipoCarga();
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

function controlarBotoesPontuacaoPorTipoCargaHabilitados() {
    var isEdicao = _pontuacaoPorTipoCarga.Codigo.val() > 0;

    _pontuacaoPorTipoCarga.Atualizar.visible(isEdicao);
    _pontuacaoPorTipoCarga.Excluir.visible(isEdicao);
    _pontuacaoPorTipoCarga.Adicionar.visible(!isEdicao);
}

function limparCamposPontuacaoPorTipoCarga() {
    LimparCampos(_pontuacaoPorTipoCarga);
    controlarBotoesPontuacaoPorTipoCargaHabilitados();
}

function recarregarGridPontuacaoPorTipoCarga() {
    _gridPontuacaoPorTipoCarga.CarregarGrid();
}