/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDMotivoRetiradaFilaCarregamento;
var _motivoRetiradaFilaCarregamento;
var _pesquisaMotivoRetiradaFilaCarregamento;
var _gridMotivoRetiradaFilaCarregamento;

/*
 * Declaração das Classes
 */

var CRUDMotivoRetiradaFilaCarregamento = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

var MotivoRetiradaFilaCarregamento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Observacao = PropertyEntity({ text: "Observação:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 2000 });
    this.Status = PropertyEntity({ text: "*Situação: ", issue: 557, val: ko.observable(true), options: _status, def: true });
    this.Mobile = PropertyEntity({ text: "Utilizar este motivo no aplicativo MultiMobile?", getType: typesKnockout.bool, val: ko.observable(false), def: false });
}

var PesquisaMotivoRetiradaFilaCarregamento = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "Situação: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });

    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridMotivoRetiradaFilaCarregamento, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridMotivoRetiradaFilaCarregamento() {
    var opcaoEditar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var configuracoesExportacao = { url: "MotivoRetiradaFilaCarregamento/ExportarPesquisa", titulo: "Motivo Punição de Veículo" };

    _gridMotivoRetiradaFilaCarregamento = new GridViewExportacao(_pesquisaMotivoRetiradaFilaCarregamento.Pesquisar.idGrid, "MotivoRetiradaFilaCarregamento/Pesquisa", _pesquisaMotivoRetiradaFilaCarregamento, menuOpcoes, configuracoesExportacao);
    _gridMotivoRetiradaFilaCarregamento.CarregarGrid();
}

function loadMotivoRetiradaFilaCarregamento() {
    _motivoRetiradaFilaCarregamento = new MotivoRetiradaFilaCarregamento();
    KoBindings(_motivoRetiradaFilaCarregamento, "knockoutMotivoRetiradaFilaCarregamento");

    HeaderAuditoria("MotivoRetiradaFilaCarregamento", _motivoRetiradaFilaCarregamento);

    _CRUDMotivoRetiradaFilaCarregamento = new CRUDMotivoRetiradaFilaCarregamento();
    KoBindings(_CRUDMotivoRetiradaFilaCarregamento, "knockoutCRUDMotivoRetiradaFilaCarregamento");

    _pesquisaMotivoRetiradaFilaCarregamento = new PesquisaMotivoRetiradaFilaCarregamento();
    KoBindings(_pesquisaMotivoRetiradaFilaCarregamento, "knockoutPesquisaMotivoRetiradaFilaCarregamento", false, _pesquisaMotivoRetiradaFilaCarregamento.Pesquisar.id);

    loadGridMotivoRetiradaFilaCarregamento();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarClick(e, sender) {
    Salvar(_motivoRetiradaFilaCarregamento, "MotivoRetiradaFilaCarregamento/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                recarregarGridMotivoRetiradaFilaCarregamento();
                limparCamposMotivoRetiradaFilaCarregamento();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_motivoRetiradaFilaCarregamento, "MotivoRetiradaFilaCarregamento/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                recarregarGridMotivoRetiradaFilaCarregamento();
                limparCamposMotivoRetiradaFilaCarregamento();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function cancelarClick() {
    limparCamposMotivoRetiradaFilaCarregamento();
}

function editarClick(registroSelecionado) {
    limparCamposMotivoRetiradaFilaCarregamento();

    _motivoRetiradaFilaCarregamento.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_motivoRetiradaFilaCarregamento, "MotivoRetiradaFilaCarregamento/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaMotivoRetiradaFilaCarregamento.ExibirFiltros.visibleFade(false);

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
        ExcluirPorCodigo(_motivoRetiradaFilaCarregamento, "MotivoRetiradaFilaCarregamento/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    recarregarGridMotivoRetiradaFilaCarregamento();
                    limparCamposMotivoRetiradaFilaCarregamento();
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
    _CRUDMotivoRetiradaFilaCarregamento.Atualizar.visible(isEdicao);
    _CRUDMotivoRetiradaFilaCarregamento.Excluir.visible(isEdicao);
    _CRUDMotivoRetiradaFilaCarregamento.Cancelar.visible(isEdicao);
    _CRUDMotivoRetiradaFilaCarregamento.Adicionar.visible(!isEdicao);
}

function limparCamposMotivoRetiradaFilaCarregamento() {
    var isEdicao = false;

    controlarBotoesHabilitados(isEdicao);
    LimparCampos(_motivoRetiradaFilaCarregamento);
}

function recarregarGridMotivoRetiradaFilaCarregamento() {
    _gridMotivoRetiradaFilaCarregamento.CarregarGrid();
}