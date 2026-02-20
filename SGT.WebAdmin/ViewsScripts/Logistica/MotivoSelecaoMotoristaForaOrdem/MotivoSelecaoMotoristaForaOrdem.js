/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDMotivoSelecaoMotoristaForaOrdem;
var _motivoSelecaoMotoristaForaOrdem;
var _pesquisaMotivoSelecaoMotoristaForaOrdem;
var _gridMotivoSelecaoMotoristaForaOrdem;

/*
 * Declaração das Classes
 */

var CRUDMotivoSelecaoMotoristaForaOrdem = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

var MotivoSelecaoMotoristaForaOrdem = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Observacao = PropertyEntity({ text: "Observação:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 2000 });
    this.Status = PropertyEntity({ text: "*Situação: ", issue: 557, val: ko.observable(true), options: _status, def: true });
}

var PesquisaMotivoSelecaoMotoristaForaOrdem = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "Situação: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });

    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridMotivoSelecaoMotoristaForaOrdem, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridMotivoSelecaoMotoristaForaOrdem() {
    var opcaoEditar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var configuracoesExportacao = { url: "MotivoSelecaoMotoristaForaOrdem/ExportarPesquisa", titulo: "Motivo Seleção de Motorista Fora da Ordem" };

    _gridMotivoSelecaoMotoristaForaOrdem = new GridViewExportacao(_pesquisaMotivoSelecaoMotoristaForaOrdem.Pesquisar.idGrid, "MotivoSelecaoMotoristaForaOrdem/Pesquisa", _pesquisaMotivoSelecaoMotoristaForaOrdem, menuOpcoes, configuracoesExportacao);
    _gridMotivoSelecaoMotoristaForaOrdem.CarregarGrid();
}

function loadMotivoSelecaoMotoristaForaOrdem() {
    _motivoSelecaoMotoristaForaOrdem = new MotivoSelecaoMotoristaForaOrdem();
    KoBindings(_motivoSelecaoMotoristaForaOrdem, "knockoutMotivoSelecaoMotoristaForaOrdem");

    HeaderAuditoria("MotivoSelecaoMotoristaForaOrdem", _motivoSelecaoMotoristaForaOrdem);

    _CRUDMotivoSelecaoMotoristaForaOrdem = new CRUDMotivoSelecaoMotoristaForaOrdem();
    KoBindings(_CRUDMotivoSelecaoMotoristaForaOrdem, "knockoutCRUDMotivoSelecaoMotoristaForaOrdem");

    _pesquisaMotivoSelecaoMotoristaForaOrdem = new PesquisaMotivoSelecaoMotoristaForaOrdem();
    KoBindings(_pesquisaMotivoSelecaoMotoristaForaOrdem, "knockoutPesquisaMotivoSelecaoMotoristaForaOrdem", false, _pesquisaMotivoSelecaoMotoristaForaOrdem.Pesquisar.id);

    loadGridMotivoSelecaoMotoristaForaOrdem();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarClick(e, sender) {
    Salvar(_motivoSelecaoMotoristaForaOrdem, "MotivoSelecaoMotoristaForaOrdem/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                recarregarGridMotivoSelecaoMotoristaForaOrdem();
                limparCamposMotivoSelecaoMotoristaForaOrdem();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_motivoSelecaoMotoristaForaOrdem, "MotivoSelecaoMotoristaForaOrdem/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                recarregarGridMotivoSelecaoMotoristaForaOrdem();
                limparCamposMotivoSelecaoMotoristaForaOrdem();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function cancelarClick() {
    limparCamposMotivoSelecaoMotoristaForaOrdem();
}

function editarClick(registroSelecionado) {
    limparCamposMotivoSelecaoMotoristaForaOrdem();

    _motivoSelecaoMotoristaForaOrdem.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_motivoSelecaoMotoristaForaOrdem, "MotivoSelecaoMotoristaForaOrdem/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaMotivoSelecaoMotoristaForaOrdem.ExibirFiltros.visibleFade(false);

                var isEdicao = true;

                controlarBotoesHabilitados(isEdicao);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function excluirClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse cadastro?", function () {
        ExcluirPorCodigo(_motivoSelecaoMotoristaForaOrdem, "MotivoSelecaoMotoristaForaOrdem/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    recarregarGridMotivoSelecaoMotoristaForaOrdem();
                    limparCamposMotivoSelecaoMotoristaForaOrdem();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }, null);
    });
}

function exibirFiltrosClick(e) {
    e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
}

/*
 * Declaração das Funções
 */

function controlarBotoesHabilitados(isEdicao) {
    _CRUDMotivoSelecaoMotoristaForaOrdem.Atualizar.visible(isEdicao);
    _CRUDMotivoSelecaoMotoristaForaOrdem.Excluir.visible(isEdicao);
    _CRUDMotivoSelecaoMotoristaForaOrdem.Cancelar.visible(isEdicao);
    _CRUDMotivoSelecaoMotoristaForaOrdem.Adicionar.visible(!isEdicao);
}

function limparCamposMotivoSelecaoMotoristaForaOrdem() {
    var isEdicao = false;

    controlarBotoesHabilitados(isEdicao);
    LimparCampos(_motivoSelecaoMotoristaForaOrdem);
}

function recarregarGridMotivoSelecaoMotoristaForaOrdem() {
    _gridMotivoSelecaoMotoristaForaOrdem.CarregarGrid();
}