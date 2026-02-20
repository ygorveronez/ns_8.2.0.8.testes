/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDMotivoInconsistenciaDigitacao;
var _motivoInconsistenciaDigitacao;
var _pesquisaMotivoInconsistenciaDigitacao;
var _gridMotivoInconsistenciaDigitacao;

/*
 * Declaração das Classes
 */

var CRUDMotivoInconsistenciaDigitacao = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

var MotivoInconsistenciaDigitacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Observacao = PropertyEntity({ text: "Observação:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 2000 });
    this.Status = PropertyEntity({ text: "*Situação: ", issue: 557, val: ko.observable(true), options: _status, def: true });
    this.ExigeObservacao = PropertyEntity({ text: "Exige Observação", getType: typesKnockout.bool, val: ko.observable(false), def: false });
}

var PesquisaMotivoInconsistenciaDigitacao = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "Situação: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });

    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridMotivoInconsistenciaDigitacao, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridMotivoInconsistenciaDigitacao() {
    var opcaoEditar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var configuracoesExportacao = { url: "MotivoInconsistenciaDigitacao/ExportarPesquisa", titulo: "Motivo de Inconsistência na Digitação" };

    _gridMotivoInconsistenciaDigitacao = new GridViewExportacao(_pesquisaMotivoInconsistenciaDigitacao.Pesquisar.idGrid, "MotivoInconsistenciaDigitacao/Pesquisa", _pesquisaMotivoInconsistenciaDigitacao, menuOpcoes, configuracoesExportacao);
    _gridMotivoInconsistenciaDigitacao.CarregarGrid();
}

function loadMotivoInconsistenciaDigitacao() {
    _motivoInconsistenciaDigitacao = new MotivoInconsistenciaDigitacao();
    KoBindings(_motivoInconsistenciaDigitacao, "knockoutMotivoInconsistenciaDigitacao");

    HeaderAuditoria("MotivoInconsistenciaDigitacao", _motivoInconsistenciaDigitacao);

    _CRUDMotivoInconsistenciaDigitacao = new CRUDMotivoInconsistenciaDigitacao();
    KoBindings(_CRUDMotivoInconsistenciaDigitacao, "knockoutCRUDMotivoInconsistenciaDigitacao");

    _pesquisaMotivoInconsistenciaDigitacao = new PesquisaMotivoInconsistenciaDigitacao();
    KoBindings(_pesquisaMotivoInconsistenciaDigitacao, "knockoutPesquisaMotivoInconsistenciaDigitacao", false, _pesquisaMotivoInconsistenciaDigitacao.Pesquisar.id);

    loadGridMotivoInconsistenciaDigitacao();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarClick(e, sender) {
    Salvar(_motivoInconsistenciaDigitacao, "MotivoInconsistenciaDigitacao/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                recarregarGridMotivoInconsistenciaDigitacao();
                limparCamposMotivoInconsistenciaDigitacao();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_motivoInconsistenciaDigitacao, "MotivoInconsistenciaDigitacao/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                recarregarGridMotivoInconsistenciaDigitacao();
                limparCamposMotivoInconsistenciaDigitacao();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function cancelarClick() {
    limparCamposMotivoInconsistenciaDigitacao();
}

function editarClick(registroSelecionado) {
    limparCamposMotivoInconsistenciaDigitacao();

    _motivoInconsistenciaDigitacao.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_motivoInconsistenciaDigitacao, "MotivoInconsistenciaDigitacao/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaMotivoInconsistenciaDigitacao.ExibirFiltros.visibleFade(false);

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
        ExcluirPorCodigo(_motivoInconsistenciaDigitacao, "MotivoInconsistenciaDigitacao/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    recarregarGridMotivoInconsistenciaDigitacao();
                    limparCamposMotivoInconsistenciaDigitacao();
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
    _CRUDMotivoInconsistenciaDigitacao.Atualizar.visible(isEdicao);
    _CRUDMotivoInconsistenciaDigitacao.Excluir.visible(isEdicao);
    _CRUDMotivoInconsistenciaDigitacao.Cancelar.visible(isEdicao);
    _CRUDMotivoInconsistenciaDigitacao.Adicionar.visible(!isEdicao);
}

function limparCamposMotivoInconsistenciaDigitacao() {
    var isEdicao = false;

    controlarBotoesHabilitados(isEdicao);
    LimparCampos(_motivoInconsistenciaDigitacao);
}

function recarregarGridMotivoInconsistenciaDigitacao() {
    _gridMotivoInconsistenciaDigitacao.CarregarGrid();
}