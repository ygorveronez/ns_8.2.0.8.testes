/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../Enumeradores/EnumSimNaoPesquisa.js" />
/// <reference path="../../Enumeradores/EnumSimNao.js" />
/// <reference path="../../Consultas/Irregularidade.js" />
/// <reference path="../../Enumeradores/EnumTipoMotivoIrregularidade.js" />
/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDMotivosIrregularidades;
var _motivosIrregularidades;
var _pesquisaMotivosIrregularidades;
var _gridMotivosIrregularidades;

/*
 * Declaração das Classes
 */

var CRUDMotivosIrregularidades = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

var MotivosIrregularidades = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Observacao = PropertyEntity({ text: "Observação:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 300 });
    this.Situacao = PropertyEntity({ text: "*Situação: ", issue: 557, val: ko.observable(true), options: _status, def: true });
    this.TipoMotivo = PropertyEntity({ text: "Tipo motivo: ", val: ko.observable(EnumTipoMotivoIrregularidade.NaoSelecionado), options: EnumTipoMotivoIrregularidade.obterOpcoes(), def: EnumTipoMotivoIrregularidade.NaoSelecionado });
    this.Irregularidade = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Irregularidade:", idBtnSearch: guid() });
}

var PesquisaMotivosIrregularidades = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(0), options: _statusFemPesquisa, def: 0 });

    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridMotivosIrregularidades, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridMotivosIrregularidades() {
    var opcaoEditar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var configuracoesExportacao = { url: "MotivoIrregularidade/ExportarPesquisa", titulo: "Motivo de Rejeição de Alteração de Pedido" };

    _gridMotivosIrregularidades = new GridViewExportacao(_pesquisaMotivosIrregularidades.Pesquisar.idGrid, "MotivoIrregularidade/Pesquisa", _pesquisaMotivosIrregularidades, menuOpcoes, configuracoesExportacao);
    _gridMotivosIrregularidades.CarregarGrid();
}

function loadMotivosIrregularidades() {
    _motivosIrregularidades = new MotivosIrregularidades();
    KoBindings(_motivosIrregularidades, "knockoutMotivosIrregularidades");

    HeaderAuditoria("MotivosIrregularidades", _motivosIrregularidades);

    _CRUDMotivosIrregularidades = new CRUDMotivosIrregularidades();
    KoBindings(_CRUDMotivosIrregularidades, "knockoutCRUDMotivosIrregularidades");

    _pesquisaMotivosIrregularidades = new PesquisaMotivosIrregularidades();
    KoBindings(_pesquisaMotivosIrregularidades, "knockoutPesquisaMotivosIrregularidades", false, _pesquisaMotivosIrregularidades.Pesquisar.id);

    BuscarIrregularidades(_motivosIrregularidades.Irregularidade);

    loadGridMotivosIrregularidades();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarClick(e, sender) {
    Salvar(_motivosIrregularidades, "MotivoIrregularidade/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                recarregarGridMotivosIrregularidades();
                limparCamposMotivosIrregularidades();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_motivosIrregularidades, "MotivoIrregularidade/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                recarregarGridMotivosIrregularidades();
                limparCamposMotivosIrregularidades();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function cancelarClick() {
    limparCamposMotivosIrregularidades();
}

function editarClick(registroSelecionado) {
    limparCamposMotivosIrregularidades();

    _motivosIrregularidades.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_motivosIrregularidades, "MotivoIrregularidade/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaMotivosIrregularidades.ExibirFiltros.visibleFade(false);

                controlarBotoesHabilitados();
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
        ExcluirPorCodigo(_motivosIrregularidades, "MotivoIrregularidade/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    recarregarGridMotivosIrregularidades();
                    limparCamposMotivosIrregularidades();
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
 * Declaração das Funções Privadas
 */

function controlarBotoesHabilitados() {
    var isEdicao = _motivosIrregularidades.Codigo.val() > 0;

    _CRUDMotivosIrregularidades.Atualizar.visible(isEdicao);
    _CRUDMotivosIrregularidades.Excluir.visible(isEdicao);
    _CRUDMotivosIrregularidades.Cancelar.visible(isEdicao);
    _CRUDMotivosIrregularidades.Adicionar.visible(!isEdicao);
}

function limparCamposMotivosIrregularidades() {
    LimparCampos(_motivosIrregularidades);
    controlarBotoesHabilitados();
}

function recarregarGridMotivosIrregularidades() {
    _gridMotivosIrregularidades.CarregarGrid();
}