/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDMotivoSucateamentoPneu;
var _motivoSucateamentoPneu;
var _pesquisaMotivoSucateamentoPneu;
var _gridMotivoSucateamentoPneu;

/*
 * Declaração das Classes
 */

var MotivoSucateamentoPneu = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 200, required: true });
    this.TipoMovimento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Tipo de Movimento:", idBtnSearch: guid(), required: true, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "*Situação: ", issue: 557, val: ko.observable(true), options: _status, def: true });
}

var CRUDMotivoSucateamentoPneu = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

var PesquisaMotivoSucateamentoPneu = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable(""), maxlength: 200 });
    this.Status = PropertyEntity({ text: "Situação: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });

    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridMotivoSucateamentoPneu, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridMotivoSucateamentoPneu() {
    var opcaoEditar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var configuracoesExportacao = { url: "MotivoSucateamentoPneu/ExportarPesquisa", titulo: "Motivo de Sucateamento de Pneu" };

    _gridMotivoSucateamentoPneu = new GridViewExportacao(_pesquisaMotivoSucateamentoPneu.Pesquisar.idGrid, "MotivoSucateamentoPneu/Pesquisa", _pesquisaMotivoSucateamentoPneu, menuOpcoes, configuracoesExportacao);
    _gridMotivoSucateamentoPneu.CarregarGrid();
}

function loadMotivoSucateamentoPneu() {
    _motivoSucateamentoPneu = new MotivoSucateamentoPneu();
    KoBindings(_motivoSucateamentoPneu, "knockoutMotivoSucateamentoPneu");

    HeaderAuditoria("MotivoSucateamentoPneu", _motivoSucateamentoPneu);

    _CRUDMotivoSucateamentoPneu = new CRUDMotivoSucateamentoPneu();
    KoBindings(_CRUDMotivoSucateamentoPneu, "knockoutCRUDMotivoSucateamentoPneu");

    _pesquisaMotivoSucateamentoPneu = new PesquisaMotivoSucateamentoPneu();
    KoBindings(_pesquisaMotivoSucateamentoPneu, "knockoutPesquisaMotivoSucateamentoPneu", false, _pesquisaMotivoSucateamentoPneu.Pesquisar.id);

    new BuscarTipoMovimento(_motivoSucateamentoPneu.TipoMovimento);;

    loadGridMotivoSucateamentoPneu();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarClick(e, sender) {
    Salvar(_motivoSucateamentoPneu, "MotivoSucateamentoPneu/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                recarregarGridMotivoSucateamentoPneu();
                limparCamposMotivoSucateamentoPneu();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_motivoSucateamentoPneu, "MotivoSucateamentoPneu/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                recarregarGridMotivoSucateamentoPneu();
                limparCamposMotivoSucateamentoPneu();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function cancelarClick() {
    limparCamposMotivoSucateamentoPneu();
}

function editarClick(registroSelecionado) {
    limparCamposMotivoSucateamentoPneu();

    _motivoSucateamentoPneu.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_motivoSucateamentoPneu, "MotivoSucateamentoPneu/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaMotivoSucateamentoPneu.ExibirFiltros.visibleFade(false);

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
        ExcluirPorCodigo(_motivoSucateamentoPneu, "MotivoSucateamentoPneu/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    recarregarGridMotivoSucateamentoPneu();
                    limparCamposMotivoSucateamentoPneu();
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
    _CRUDMotivoSucateamentoPneu.Atualizar.visible(isEdicao);
    _CRUDMotivoSucateamentoPneu.Excluir.visible(isEdicao);
    _CRUDMotivoSucateamentoPneu.Cancelar.visible(isEdicao);
    _CRUDMotivoSucateamentoPneu.Adicionar.visible(!isEdicao);
}

function limparCamposMotivoSucateamentoPneu() {
    var isEdicao = false;

    controlarBotoesHabilitados(isEdicao);
    LimparCampos(_motivoSucateamentoPneu);
}

function recarregarGridMotivoSucateamentoPneu() {
    _gridMotivoSucateamentoPneu.CarregarGrid();
}