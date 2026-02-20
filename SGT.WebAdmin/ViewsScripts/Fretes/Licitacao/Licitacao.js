/// <reference path="../../Consultas/TabelaFrete.js" />
/// <reference path="../../Consultas/SolicitacaoLicitacao.js" />
/// <reference path="LicitacaoTransportador.js" />
/// <reference path="LicitacaoAnexo.js" />
/// <reference path="LicitacaoTabelaCliente.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDLicitacao;
var _pesquisaLicitacao;
var _gridLicitacao;
var _licitacao;

/*
 * Declaração das Classes
 */

var Licitacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.DataFim = PropertyEntity({ text: "*Data Fim:", getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual(), required: true, enable: ko.observable(true) });
    this.DataInicio = PropertyEntity({ text: "*Data Início:", getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: Global.DataAtual(), required: true, enable: ko.observable(true) });
    this.Descricao = PropertyEntity({ text: "*Descrição:", maxlength: 200, required: true, enable: ko.observable(true) });
    this.Numero = PropertyEntity({ text: "Número:", val: ko.observable(""), def: "", getType: typesKnockout.int, enable: false });
    this.Observacao = PropertyEntity({ text: "Observação:", maxlength: 2000, enable: ko.observable(true) });

    this.TabelaFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Tabela de Frete:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.SolicitacaoLicitacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Solicitação de Licitação:", idBtnSearch: guid(), enable: ko.observable(true) });

    this.LiberarTodosTransportadores = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: "Liberar para todos os transportadores", def: false });

    this.Transportadores = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, idGrid: guid() });
    this.TabelasFreteCliente = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, idGrid: guid() });

    this.TabelaFrete.codEntity.subscribe(function (novoValor) {
        LimparCamposLicitacaoTabelaFreteCliente();
        if (novoValor > 0)
            $("#liTabTabelaFreteCliente").show();
        else
            $("#liTabTabelaFreteCliente").hide();
    });
};

var CRUDLicitacao = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar / Nova", visible: true });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
};

var PesquisaLicitacao = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: ", maxlength: 200 });
    this.Numero = PropertyEntity({ text: "Número: ", getType: typesKnockout.int });
    this.TabelaFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Tabela de Frete: "), idBtnSearch: guid() });

    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridLicitacao, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
};

/*
 * Declaração das Funções de Inicialização
 */

function loadGridLicitacao() {
    var opcaoEditar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarClick, tamanho: "14", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var configuracoesExportacao = { url: "Licitacao/ExportarPesquisa", titulo: "Licitações" };

    _gridLicitacao = new GridViewExportacao(_pesquisaLicitacao.Pesquisar.idGrid, "Licitacao/Pesquisa", _pesquisaLicitacao, menuOpcoes, configuracoesExportacao);
    _gridLicitacao.CarregarGrid();
}

function loadLicitacao() {
    _licitacao = new Licitacao();
    KoBindings(_licitacao, "knockoutLicitacao");

    HeaderAuditoria("Licitacao", _licitacao);

    _CRUDLicitacao = new CRUDLicitacao();
    KoBindings(_CRUDLicitacao, "knockoutCRUDLicitacao");

    _pesquisaLicitacao = new PesquisaLicitacao();
    KoBindings(_pesquisaLicitacao, "knockoutPesquisaLicitacao", false, _pesquisaLicitacao.Pesquisar.id);

    new BuscarTabelasDeFrete(_pesquisaLicitacao.TabelaFrete);
    new BuscarTabelasDeFrete(_licitacao.TabelaFrete);
    new BuscarSolicitacaoLicitacao(_licitacao.SolicitacaoLicitacao, retornoSolicitacaoLicitacao);

    loadGridLicitacao();
    loadLicitacaoTransportador();
    loadAnexo();
    LoadLicitacaoTabelaFreteCliente();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarClick(e, sender) {
    preencherListasSelecaoLicitacao();

    Salvar(_licitacao, "Licitacao/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                enviarArquivosAnexados(retorno.Data.Codigo);
                recarregarGridLicitacao();
                limparCamposLicitacao();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function atualizarClick(e, sender) {
    preencherListasSelecaoLicitacao();

    Salvar(_licitacao, "Licitacao/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                recarregarGridLicitacao();
                limparCamposLicitacao();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function cancelarClick() {
    limparCamposLicitacao();
}

function preencherListasSelecaoLicitacao() {
    _licitacao.Transportadores.val(obterTransportadores());
    _licitacao.TabelasFreteCliente.val(JSON.stringify(_licitacaoTabelaFreteCliente.TabelaFreteCliente.basicTable.BuscarRegistros()));
}

function editarClick(registroSelecionado) {
    limparCamposLicitacao();

    executarReST("Licitacao/BuscarPorCodigo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                PreencherObjetoKnout(_licitacao, retorno);

                recarregarGridLicitacaoTransportador();
                _anexo.Anexos.val(retorno.Data.Anexos);
                RecarregarGridLicitacaoTabelaFreteCliente();

                _pesquisaLicitacao.ExibirFiltros.visibleFade(false);

                controlarComponentesHabilitados();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function excluirClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse cadastro?", function () {
        ExcluirPorCodigo(_licitacao, "Licitacao/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    recarregarGridLicitacao();
                    limparCamposLicitacao();
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

function retornoSolicitacaoLicitacao(data) {
    _licitacao.Descricao.val(data.DescricaoCotacao);
    _licitacao.DataFim.val(data.DataPrazoResposta);
    _licitacao.SolicitacaoLicitacao.codEntity(data.Codigo);
    _licitacao.SolicitacaoLicitacao.val(data.Descricao);
    _licitacao.DataInicio.val(Global.DataAtual());
}

function controlarBotoesHabilitados(isEdicao) {
    if (isEdicao) {
        _CRUDLicitacao.Adicionar.visible(false);
        _CRUDLicitacao.Atualizar.visible(isPermitirEditarRegistro());
        _CRUDLicitacao.Excluir.visible(isPermitirEditarRegistro());
    }
    else {
        _CRUDLicitacao.Adicionar.visible(true);
        _CRUDLicitacao.Atualizar.visible(false);
        _CRUDLicitacao.Excluir.visible(false);
    }
}

function controlarCamposHabilitados(isEdicao) {
    var habilitarCampos = !isEdicao || isPermitirEditarRegistro();

    _licitacao.Descricao.enable(habilitarCampos);
    _licitacao.DataFim.enable(habilitarCampos);
    _licitacao.DataInicio.enable(habilitarCampos);
    _licitacao.TabelaFrete.enable(habilitarCampos);
    _licitacao.Observacao.enable(habilitarCampos);

    _anexo.Anexos.visible(habilitarCampos);
}

function controlarComponentesHabilitados() {
    var isEdicao = isRegistroEdicao();

    controlarBotoesHabilitados(isEdicao);
    controlarCamposHabilitados(isEdicao);
}

function isPermitirEditarRegistro() {
    return true;
}

function isRegistroEdicao() {
    return _licitacao.Codigo.val() > 0;
}

function limparCamposLicitacao() {
    LimparCampos(_licitacao);
    limparCamposAnexo();
    recarregarGridLicitacaoTransportador();
    LimparCamposLicitacaoTabelaFreteCliente();

    $("#tabLicitacao").click();

    controlarComponentesHabilitados();
}

function recarregarGridLicitacao() {
    _gridLicitacao.CarregarGrid();
}