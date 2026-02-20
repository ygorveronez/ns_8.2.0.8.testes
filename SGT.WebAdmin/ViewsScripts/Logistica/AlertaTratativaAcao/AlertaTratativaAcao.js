/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDAlertaTratativaAcao;
var _AlertaTratativaAcao;
var _pesquisaAlertaTratativaAcao;
var _gridAlertaTratativaAcao;


/*
 * Declaração das Classesm
 */

var CRUDAlertaTratativaAcao = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

var AlertaTratativaAcao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.AcaoMonitorada = PropertyEntity({ text: "Ação Monitorada", getType: typesKnockout.bool, val: ko.observable(false) });
    this.TipoAlerta = PropertyEntity({ text: "*Tipo Alerta: ", val: ko.observable(EnumTipoAlerta.SemAlerta), options: EnumTipoAlerta.obterOpcoes(), def: EnumTipoAlerta.SemAlerta, visible: true });
    this.Status = PropertyEntity({ text: "*Situação: ", issue: 557, val: ko.observable(true), options: _status, def: true });
}

var PesquisaAlertaTratativaAcao = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "Situação: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });
    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridAlertaTratativaAcao, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridAlertaTratativaAcao() {
    var opcaoEditar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var configuracoesExportacao = { url: "AlertaTratativaAcao/ExportarPesquisa", titulo: "Motivo Seleção de Motorista Fora da Ordem" };

    _gridAlertaTratativaAcao = new GridViewExportacao(_pesquisaAlertaTratativaAcao.Pesquisar.idGrid, "AlertaTratativaAcao/Pesquisa", _pesquisaAlertaTratativaAcao, menuOpcoes, configuracoesExportacao);
    _gridAlertaTratativaAcao.CarregarGrid();
}

function loadAlertaTratativaAcao() {
    _AlertaTratativaAcao = new AlertaTratativaAcao();
    KoBindings(_AlertaTratativaAcao, "knockoutAlertaTratativaAcao");

    HeaderAuditoria("AlertaTratativaAcao", _AlertaTratativaAcao);

    _CRUDAlertaTratativaAcao = new CRUDAlertaTratativaAcao();
    KoBindings(_CRUDAlertaTratativaAcao, "knockoutCRUDAlertaTratativaAcao");

    _pesquisaAlertaTratativaAcao = new PesquisaAlertaTratativaAcao();
    KoBindings(_pesquisaAlertaTratativaAcao, "knockoutPesquisaAlertaTratativaAcao", false, _pesquisaAlertaTratativaAcao.Pesquisar.id);

    loadGridAlertaTratativaAcao();
}


/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarClick(e, sender) {
    Salvar(_AlertaTratativaAcao, "AlertaTratativaAcao/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                recarregarGridAlertaTratativaAcao();
                limparCamposAlertaTratativaAcao();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_AlertaTratativaAcao, "AlertaTratativaAcao/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                recarregarGridAlertaTratativaAcao();
                limparCamposAlertaTratativaAcao();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function cancelarClick() {
    limparCamposAlertaTratativaAcao();
}

function editarClick(registroSelecionado) {
    limparCamposAlertaTratativaAcao();

    _AlertaTratativaAcao.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_AlertaTratativaAcao, "AlertaTratativaAcao/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaAlertaTratativaAcao.ExibirFiltros.visibleFade(false);

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
        ExcluirPorCodigo(_AlertaTratativaAcao, "AlertaTratativaAcao/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    recarregarGridAlertaTratativaAcao();
                    limparCamposAlertaTratativaAcao();
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
    _CRUDAlertaTratativaAcao.Atualizar.visible(isEdicao);
    _CRUDAlertaTratativaAcao.Excluir.visible(isEdicao);
    _CRUDAlertaTratativaAcao.Cancelar.visible(isEdicao);
    _CRUDAlertaTratativaAcao.Adicionar.visible(!isEdicao);
}

function limparCamposAlertaTratativaAcao() {
    var isEdicao = false;

    controlarBotoesHabilitados(isEdicao);
    LimparCampos(_AlertaTratativaAcao);
}

function recarregarGridAlertaTratativaAcao() {
    _gridAlertaTratativaAcao.CarregarGrid();
}