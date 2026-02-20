/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="TipoOperacao.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDEmailDocumentacaoCarga;
var _corPadrao = '#FFFFFF';
var _gridEmailDocumentacaoCarga;
var _emailDocumentacaoCarga;
var _pesquisaEmailDocumentacaoCarga;


/*
 * Declaração das Classes
 */

var CRUDEmailDocumentacaoCarga = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar / Novo", visible: true });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

var EmailDocumentacaoCarga = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Pessoa: ", idBtnSearch: guid(), required: true });
    this.Emails = PropertyEntity({ text: '*E-mails (Separados por ";"):', getType: typesKnockout.string, required: true });
    this.EnviarCTe = PropertyEntity({ text: "Documento Fiscal/CT-e ", val: ko.observable(false), getType: typesKnockout.bool });
    this.EnviarCTeXML = PropertyEntity({ text: "XML", val: ko.observable(false), getType: typesKnockout.bool });
    this.EnviarCTePDF = PropertyEntity({ text: "PDF", val: ko.observable(false), getType: typesKnockout.bool });
    this.EnviarMDFe = PropertyEntity({ text: "MDF-e ", val: ko.observable(false), getType: typesKnockout.bool });
    this.EnviarContratoFrete = PropertyEntity({ text: "Contrato Frete ", val: ko.observable(false), getType: typesKnockout.bool });
    this.EnviarCIOT = PropertyEntity({ text: "CIOT ", val: ko.observable(false), getType: typesKnockout.bool });
    this.AgruparEnvioEmUmUnicoEmail = PropertyEntity({ text: "Agrupar Envio em um Único E-mail ", val: ko.observable(false), getType: typesKnockout.bool });
    this.TiposOperacoes = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

}

var PesquisaEmailDocumentacaoCarga = function () {
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Pessoa: ", idBtnSearch: guid() });
    this.Emails = PropertyEntity({ text: "E-mail:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Operação: ", idBtnSearch: guid() });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridEmailDocumentacaoCarga, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridEmailDocumentacaoCarga() {
    var opcaoEditar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var configuracoesExportacao = { url: "EmailDocumentacaoCarga/ExportarPesquisa", titulo: "Email Documentação Carga" };

    _gridEmailDocumentacaoCarga = new GridViewExportacao(_pesquisaEmailDocumentacaoCarga.Pesquisar.idGrid, "EmailDocumentacaoCarga/Pesquisa", _pesquisaEmailDocumentacaoCarga, menuOpcoes, configuracoesExportacao);
    _gridEmailDocumentacaoCarga.CarregarGrid();
}

function loadEmailDocumentacaoCarga() {
    _emailDocumentacaoCarga = new EmailDocumentacaoCarga();
    KoBindings(_emailDocumentacaoCarga, "knockoutEmailDocumentacaoCarga");

    HeaderAuditoria("EmailDocumentacaoCarga", _emailDocumentacaoCarga);

    _CRUDEmailDocumentacaoCarga = new CRUDEmailDocumentacaoCarga();
    KoBindings(_CRUDEmailDocumentacaoCarga, "knockoutCRUDEmailDocumentacaoCarga");

    _pesquisaEmailDocumentacaoCarga = new PesquisaEmailDocumentacaoCarga();
    KoBindings(_pesquisaEmailDocumentacaoCarga, "knockoutPesquisaEmailDocumentacaoCarga", false, _pesquisaEmailDocumentacaoCarga.Pesquisar.id);

    new BuscarClientes(_emailDocumentacaoCarga.Pessoa);
    new BuscarClientes(_pesquisaEmailDocumentacaoCarga.Pessoa);

    new BuscarTiposOperacao(_pesquisaEmailDocumentacaoCarga.TipoOperacao);

    loadGridEmailDocumentacaoCarga();
    loadTipoOperacao();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarClick(e, sender) {
    if (!ValidarCamposObrigatoriosEmailDocumentacaoCarga())
        return;

    var emailDocumentacaoCarga = obterEmailDocumentacaoCargaSalvar();
    executarReST("EmailDocumentacaoCarga/Adicionar", emailDocumentacaoCarga, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridEmailDocumentacaoCarga.CarregarGrid();
                limparCamposEmailDocumentacaoCarga();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Atenção!", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function atualizarClick(e, sender) {
    if (!ValidarCamposObrigatoriosEmailDocumentacaoCarga())
        return;

    var emailDocumentacaoCarga = obterEmailDocumentacaoCargaSalvar();
    executarReST("EmailDocumentacaoCarga/Atualizar", emailDocumentacaoCarga, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso.");
                _gridEmailDocumentacaoCarga.CarregarGrid();
                limparCamposEmailDocumentacaoCarga();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Atenção!", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }), sender;
}

function cancelarClick() {
    limparCamposEmailDocumentacaoCarga();
}

function editarClick(registroSelecionado) {
    limparCamposEmailDocumentacaoCarga();
    recarregarGridTipoOperacao()
    _emailDocumentacaoCarga.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_emailDocumentacaoCarga, "EmailDocumentacaoCarga/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaEmailDocumentacaoCarga.ExibirFiltros.visibleFade(false);
                recarregarGridTipoOperacao();
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
        ExcluirPorCodigo(_emailDocumentacaoCarga, "EmailDocumentacaoCarga/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    recarregarGridEmailDocumentacaoCarga();
                    limparCamposEmailDocumentacaoCarga();
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

function obterEmailDocumentacaoCargaSalvar() {
    preencherListasSelecao();

    var emailDocumentacaoCarga = RetornarObjetoPesquisa(_emailDocumentacaoCarga);

    return emailDocumentacaoCarga;
}

function preencherListasSelecao() {

    let tiposOperacoes = new Array();
  
    $.each(_tipoOperacao.Tipo.basicTable.BuscarRegistros(), function (i, tipoOperacao) {
        tiposOperacoes.push({ Tipo: tipoOperacao });
    });

    _emailDocumentacaoCarga.TiposOperacoes.val(JSON.stringify(tiposOperacoes));

}


/*
 * Declaração das Funções
 */

function controlarBotoesHabilitados() {
    var isEdicao = _emailDocumentacaoCarga.Codigo.val() > 0;

    _CRUDEmailDocumentacaoCarga.Atualizar.visible(isEdicao);
    _CRUDEmailDocumentacaoCarga.Excluir.visible(isEdicao);
    _CRUDEmailDocumentacaoCarga.Adicionar.visible(!isEdicao);
}

function limparCamposEmailDocumentacaoCarga() {
    LimparCampos(_emailDocumentacaoCarga);
    limparCamposTipoOperacao()

    recarregarGridTipoOperacao()
    controlarBotoesHabilitados();
}

function recarregarGridEmailDocumentacaoCarga() {
    _gridEmailDocumentacaoCarga.CarregarGrid();
}

function ValidarCamposObrigatoriosEmailDocumentacaoCarga() {
    if (!ValidarCamposObrigatorios(_emailDocumentacaoCarga)) {
        exibirMensagem(tipoMensagem.atencao, "Campos obrigatórios", "Por favor, insira os campos obrigatórios.");
        return false;
    }
    return true;
}
