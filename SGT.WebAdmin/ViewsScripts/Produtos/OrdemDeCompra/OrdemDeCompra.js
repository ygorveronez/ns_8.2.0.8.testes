/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="OrdemDocumento.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDOrdemDeCompra;
var _corPadrao = '#FFFFFF';
var _gridOrdemDeCompra;
var _ordemDeCompra;
var _pesquisaOrdemDeCompra;
var _pesquisaHistoricoOrdemCompra;
var _gridHistoricoOrdemCompra;

/*
 * Declaração das Classes
 */

var CRUDOrdemDeCompra = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(false) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar / Novo", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

var OrdemDeCompra = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.ControleIntegracaoEmbarcador = PropertyEntity({ text: 'Controle Integração Embarcador', required: true, val: ko.observable("") });

    this.OrdemItem = PropertyEntity({ getType: typesKnockout.dynamic, val: ko.observable([]), text: "Ordem Documento: " });
}

var PesquisaHistoricoOrdemCompra = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Historico = PropertyEntity({ idGrid: guid() });
};


var PesquisaOrdemDeCompra = function () {
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial: ", idBtnSearch: guid() });
    this.Fornecedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Fornecedor: ", idBtnSearch: guid() });
    this.NumeroOrdem = PropertyEntity({ text: 'Codigo Integração:', getType: typesKnockout.string, required: false, val: ko.observable("") });
    //this.Situacao = PropertyEntity({ val: ko.observable(""), options: EnumSituacaoColaborador.obterOpcoesPesquisa(), text: "Situação: " });
    this.Importar = PropertyEntity({
        type: types.local,
        text: "Importar",
        visible: ko.observable(false),
        accept: ".xls,.xlsx,.csv",
        cssClass: "btn-default waves-effect waves-themed ms-2",
        UrlImportacao: "",
        UrlConfiguracao: "",
        CodigoControleImportacao: EnumCodigoControleImportacao.O050_ImportacaoCarga,
        CallbackImportacao: function () {
            //_gridCarga.CarregarGrid();
        }
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridOrdemDeCompra, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}

function BuscarHistoricoIntegracaoOrdemCompra(integracao) {
    _pesquisaHistoricoOrdemCompra.Codigo.val(integracao.Codigo);
    var download = { descricao: Localization.Resources.Gerais.Geral.DownloadArquivos, id: guid(), evento: "onclick", metodo: DownloadArquivosOrdemCompraClick, tamanho: 10, icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    _gridHistoricoOrdemCompra = new GridView(_pesquisaHistoricoOrdemCompra.Historico.idGrid, "OrdemDeCompra/ConsultarHistoricoIntegracao", _pesquisaHistoricoOrdemCompra, menuOpcoes);
    _gridHistoricoOrdemCompra.CarregarGrid();
}
/*
 * Declaração das Funções de Inicialização
 */

function loadGridOrdemDeCompra() {
    var opcaoEditar = { descricao: "Editar", id: guid(), metodo: editarClick, tamanho: 10, icone: "" };
    var opcaoHistorico = { descricao: "Historico Integração", id: guid(), metodo: exibirHistoricoIntegracoesClickOrdemCompra, tamanho: 10, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 15, opcoes: [opcaoEditar, opcaoHistorico] };

    _gridOrdemDeCompra = new GridViewExportacao(_pesquisaOrdemDeCompra.Pesquisar.idGrid, "OrdemDeCompra/Pesquisa", _pesquisaOrdemDeCompra, menuOpcoes);
    _gridOrdemDeCompra.CarregarGrid();
}

function loadOrdemDeCompra() {
    _ordemDeCompra = new OrdemDeCompra();
    KoBindings(_ordemDeCompra, "knockoutOrdemDeCompra");

    HeaderAuditoria("OrdemDeCompra", _ordemDeCompra);

    _CRUDOrdemDeCompra = new CRUDOrdemDeCompra();
    KoBindings(_CRUDOrdemDeCompra, "knockoutCRUDOrdemDeCompra");

    $("#litabCondicoes").hide();

    _pesquisaOrdemDeCompra = new PesquisaOrdemDeCompra();
    KoBindings(_pesquisaOrdemDeCompra, "knockoutPesquisaOrdemDeCompra", false, _pesquisaOrdemDeCompra.Pesquisar.idGrid);

    _pesquisaHistoricoOrdemCompra = new PesquisaHistoricoOrdemCompra();
    KoBindings(_pesquisaHistoricoOrdemCompra, "knockouHistorioOrdemDeCompra");

    new BuscarFilial(_pesquisaOrdemDeCompra.Filial);
    new BuscarClientes(_pesquisaOrdemDeCompra.Fornecedor);


    loadGridOrdemDeCompra();
    loadOrdemDocumento();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarClick(e, sender) {

    if (!ValidarCamposObrigatorios())
        return exibirMensagem(tipoMensagem.aviso, "Atenção!", "Preecha campos obrigadorios");

    PreecherListaDocumentoOrdems();

    Salvar(_ordemDeCompra, "OrdemDeCompra/Adicionar", function (retorno) {
        if (!retorno.Success)
            return exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);

        if (!retorno.Data)
            return exibirMensagem(tipoMensagem.aviso, "Atenção!", retorno.Msg);

        exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
        limparCamposOrdemDeCompra();



    }, sender);
}

function atualizarClick(e, sender) {
    PreecherListaDocumentoOrdems();
    Salvar(_ordemDeCompra, "OrdemDeCompra/Atualizar", function (retorno) {
        if (!retorno.Success)
            return exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);

        if (!retorno.Data)
            return exibirMensagem(tipoMensagem.aviso, "Atenção!", retorno.Msg);

        exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso.");
        limparCamposOrdemDeCompra();


    }), sender;
}

function cancelarClick() {
    limparCamposOrdemDeCompra();
}

function editarClick(registroSelecionado) {
    _ordemDeCompra.Codigo.val(registroSelecionado.Codigo);
    BuscarPorCodigo(_ordemDeCompra, "OrdemDeCompra/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaOrdemDeCompra.ExibirFiltros.visibleFade(false);
                recarregarGridOrdemDocumento();
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
        ExcluirPorCodigo(_ordemDeCompra, "OrdemDeCompra/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    recarregarGridOrdemDeCompra();
                    limparCamposOrdemDeCompra();
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

function controlarBotoesHabilitados() {
    var isEdicao = _ordemDeCompra.Codigo.val() > 0;

    _CRUDOrdemDeCompra.Atualizar.visible(false);
    _CRUDOrdemDeCompra.Excluir.visible(false);
    _CRUDOrdemDeCompra.Cancelar.visible(isEdicao);
    _CRUDOrdemDeCompra.Adicionar.visible(false);
}

function limparCamposOrdemDeCompra() {
    LimparCampos(_ordemDeCompra);
    limparCamposOrdemDocumento()

    recarregarGridOrdemDocumento()
    controlarBotoesHabilitados();
}

function recarregarGridOrdemDeCompra() {
    _gridOrdemDeCompra.CarregarGrid();
}

function ValidarCamposObrigatoriosOrdemDeCompra() {
    if (!ValidarCamposObrigatorios(_ordemDeCompra)) {
        exibirMensagem(tipoMensagem.atencao, "Campos obrigatórios", "Por favor, insira os campos obrigatórios.");
        return false;
    }
    return true;
}

function DownloadArquivosOrdemCompraClick(historicoConsulta) {
    executarDownload("OrdemDeCompra/DownloadArquivosHistoricoIntegracao", { Codigo: historicoConsulta.Codigo });
}

function exibirHistoricoIntegracoesClickOrdemCompra(integracao) {
    BuscarHistoricoIntegracaoOrdemCompra(integracao);
    Global.abrirModal("divModalHistoricoOrdemCompra");
}
