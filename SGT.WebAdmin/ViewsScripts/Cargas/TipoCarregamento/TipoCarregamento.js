/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDTipoCarregamento;
var _corPadrao = '#FFFFFF';
var _gridTipoCarregamento;
var _tipoCarregamento;
var _pesquisaTipoCarregamento;


/*
 * Declaração das Classes
 */

var CRUDTipoCarregamento = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar / Novo", visible: true });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

var TipoCarregamento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: '*Descrição:', getType: typesKnockout.string, required: true });
    this.CodigoDeIntegracao = PropertyEntity({ text: '*Código de Integração:', getType: typesKnockout.string, required: true });
    this.Observacao = PropertyEntity({ text: 'Observação:', getType: typesKnockout.string, required: false });
    this.Situacao = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Situação: " });
    this.TipoPadraoAgrupamentoCarga = PropertyEntity({ text: "Tipo padrão para Agrupamento de Cargas", def: true, getType: typesKnockout.bool, val: ko.observable(false) });

    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridTipoCarregamento, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });

}

var PesquisaTipoCarregamento = function () {
    this.Descricao = PropertyEntity({ text: '*Descrição:', getType: typesKnockout.string, required: true });
    this.CodigoDeIntegracao = PropertyEntity({ text: "*Código de Integração:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Observacao = PropertyEntity({ text: "Observação:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Situacao = PropertyEntity({ val: ko.observable(true), options: Global.ObterOpcoesPesquisaBooleano("Ativo", "Inativo"), def: true, text: "*Situação: " });
        
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridTipoCarregamento, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });


    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridTipoCarregamento() {
    var opcaoEditar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var configuracoesExportacao = { url: "TipoCarregamento/ExportarPesquisa", titulo: "Tipo Carregamento" };

    _gridTipoCarregamento = new GridViewExportacao(_pesquisaTipoCarregamento.Pesquisar.idGrid, "TipoCarregamento/Pesquisa", _pesquisaTipoCarregamento, menuOpcoes, configuracoesExportacao);
    _gridTipoCarregamento.CarregarGrid();
}

function loadTipoCarregamento() {
    _tipoCarregamento = new TipoCarregamento();
    KoBindings(_tipoCarregamento, "knockoutTipoCarregamento");

    HeaderAuditoria("TipoCarregamento", _tipoCarregamento);

    _CRUDTipoCarregamento = new CRUDTipoCarregamento();
    KoBindings(_CRUDTipoCarregamento, "knockoutCRUDTipoCarregamento");

    _pesquisaTipoCarregamento = new PesquisaTipoCarregamento();
    KoBindings(_pesquisaTipoCarregamento, "knockoutPesquisaTipoCarregamento", false, _pesquisaTipoCarregamento.Pesquisar.id);

    loadGridTipoCarregamento();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarClick(e, sender) {
    if (!ValidarCamposObrigatoriosTipoCarregamento())
        return;

    var tipoCarregamento = obterTipoCarregamentoSalvar();
    executarReST("TipoCarregamento/Adicionar", tipoCarregamento, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridTipoCarregamento.CarregarGrid();
                limparCamposTipoCarregamento();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Atenção!", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function atualizarClick(e, sender) {
    if (!ValidarCamposObrigatoriosTipoCarregamento())
        return;

    var tipoCarregamento = obterTipoCarregamentoSalvar();
    executarReST("TipoCarregamento/Atualizar", tipoCarregamento, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso.");
                _gridTipoCarregamento.CarregarGrid();
                limparCamposTipoCarregamento();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Atenção!", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }), sender;
}

function cancelarClick() {
    limparCamposTipoCarregamento();
}

function editarClick(registroSelecionado) {
    limparCamposTipoCarregamento();
    _tipoCarregamento.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_tipoCarregamento, "TipoCarregamento/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
   
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
        ExcluirPorCodigo(_tipoCarregamento, "TipoCarregamento/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    recarregarGridTipoCarregamento();
                    limparCamposTipoCarregamento();
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

function obterTipoCarregamentoSalvar() {

    var tipoCarregamento = RetornarObjetoPesquisa(_tipoCarregamento);

    return tipoCarregamento;
}


/*
 * Declaração das Funções
 */

function controlarBotoesHabilitados() {
    var isEdicao = _tipoCarregamento.Codigo.val() > 0;

    _CRUDTipoCarregamento.Atualizar.visible(isEdicao);
    _CRUDTipoCarregamento.Excluir.visible(isEdicao);
    _CRUDTipoCarregamento.Adicionar.visible(!isEdicao);
}

function limparCamposTipoCarregamento() {
    LimparCampos(_tipoCarregamento);
    controlarBotoesHabilitados();
}

function recarregarGridTipoCarregamento() {
    _gridTipoCarregamento.CarregarGrid();
}

function ValidarCamposObrigatoriosTipoCarregamento() {
    if (!ValidarCamposObrigatorios(_tipoCarregamento)) {
        exibirMensagem(tipoMensagem.atencao, "Campos obrigatórios", "Por favor, insira os campos obrigatórios.");
        return false;
    }
    return true;
}
