/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Enumeradores/EnumTipoAreaVeiculo.js" />
/// <reference path="../../Consultas/CentroCarregamento.js" />
/// <reference path="AreaVeiculoDesenho.js" />
/// <reference path="AreaVeiculoPosicao.js" />
/// <reference path="AreaVeiculoTipoRetornoCarga.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDAreaVeiculo;
var _gridAreaVeiculo;
var _areaVeiculo;
var _pesquisaAreaVeiculo;

/*
 * Declaração das Classes
 */

var CRUDAreaVeiculo = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.BaixarQRCode = PropertyEntity({ eventClick: baixarQrCodeClick, type: types.event, text: "Baixar QR Code", visible: ko.observable(false) });
    this.BaixarTodosQRCode = PropertyEntity({ eventClick: baixarTodosQrCodeClick, type: types.event, text: "Baixar Todos os QR Code", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar / Novo" });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

var AreaVeiculo = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CentroCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Centro de Carregamento:", idBtnSearch: guid(), required: true });
    this.Descricao = PropertyEntity({ text: "*Descrição:", issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Observacao = PropertyEntity({ text: "Observação:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 2000 });
    this.QRCode = PropertyEntity({});
    this.Status = PropertyEntity({ text: "*Situação: ", issue: 557, val: ko.observable(true), options: _status, def: true });
    this.Tipo = PropertyEntity({ text: "*Tipo: ", val: ko.observable(EnumTipoAreaVeiculo.Todos), options: EnumTipoAreaVeiculo.obterOpcoes(), def: EnumTipoAreaVeiculo.Todos, required: true });
}

var PesquisaAreaVeiculo = function () {
    this.CentroCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Centro de Carregamento:", idBtnSearch: guid() });
    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "Situação: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });
    this.Tipo = PropertyEntity({ text: "Tipo: ", val: ko.observable(EnumTipoAreaVeiculo.Todos), options: EnumTipoAreaVeiculo.obterOpcoesPesquisa(), def: EnumTipoAreaVeiculo.Todos });

    this.ExibirFiltros = PropertyEntity({ eventClick: exibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: recarregarGridAreaVeiculo, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridAreaVeiculo() {
    var opcaoEditar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var configuracoesExportacao = { url: "AreaVeiculo/ExportarPesquisa", titulo: "Área de Veículos" };

    _gridAreaVeiculo = new GridViewExportacao(_pesquisaAreaVeiculo.Pesquisar.idGrid, "AreaVeiculo/Pesquisa", _pesquisaAreaVeiculo, menuOpcoes, configuracoesExportacao);
    _gridAreaVeiculo.CarregarGrid();
}

function loadAreaVeiculo() {
    _areaVeiculo = new AreaVeiculo();
    KoBindings(_areaVeiculo, "knockoutAreaVeiculo");

    HeaderAuditoria("AreaVeiculo", _areaVeiculo);

    _CRUDAreaVeiculo = new CRUDAreaVeiculo();
    KoBindings(_CRUDAreaVeiculo, "knockoutCRUDAreaVeiculo");

    _pesquisaAreaVeiculo = new PesquisaAreaVeiculo();
    KoBindings(_pesquisaAreaVeiculo, "knockoutPesquisaAreaVeiculo", false, _pesquisaAreaVeiculo.Pesquisar.id);

    new BuscarCentrosCarregamento(_areaVeiculo.CentroCarregamento);
    new BuscarCentrosCarregamento(_pesquisaAreaVeiculo.CentroCarregamento);

    loadAreaVeiculoPosicao();
    loadAreaVeiculoTipoRetornoCarga();
    loadGridAreaVeiculo();
    loadAreaVeiculoDesenho();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarClick() {
    if (ValidarCamposObrigatorios(_areaVeiculo)) {
        executarReST("AreaVeiculo/Adicionar", obterAreaVeiculoSalvar(), function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                    recarregarGridAreaVeiculo();
                    limparCamposAreaVeiculo();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    }
    else
        exibirMensagemCamposObrigatorio();
}

function atualizarClick() {
    if (ValidarCamposObrigatorios(_areaVeiculo)) {
        executarReST("AreaVeiculo/Atualizar", obterAreaVeiculoSalvar(), function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                    recarregarGridAreaVeiculo();
                    limparCamposAreaVeiculo();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    }
    else
        exibirMensagemCamposObrigatorio();
}

function baixarQrCodeClick() {
    executarDownload("AreaVeiculo/BaixarQrCode", { Codigo: _areaVeiculo.Codigo.val() });
}

function baixarTodosQrCodeClick() {
    executarDownloadArquivo("AreaVeiculo/BaixarTodosQrCode", { Codigo: _areaVeiculo.Codigo.val() });
}

function cancelarClick() {
    limparCamposAreaVeiculo();
}

function editarClick(registroSelecionado) {
    limparCamposAreaVeiculo();

    executarReST("AreaVeiculo/BuscarPorCodigo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaAreaVeiculo.ExibirFiltros.visibleFade(false);

                PreencherObjetoKnout(_areaVeiculo, { Data: retorno.Data.Dados });
                preencherAreaVeiculoPosicao(retorno.Data.Posicoes);
                preencherAreaVeiculoTipoRetornoCarga(retorno.Data.TiposRetornoCarga);
                preencherAreaVeiculoDesenho(retorno.Data.Posicoes);

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
        ExcluirPorCodigo(_areaVeiculo, "AreaVeiculo/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    recarregarGridAreaVeiculo();
                    limparCamposAreaVeiculo();
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
    var isEdicao = _areaVeiculo.Codigo.val() > 0;

    _CRUDAreaVeiculo.Adicionar.visible(!isEdicao);
    _CRUDAreaVeiculo.Atualizar.visible(isEdicao);
    _CRUDAreaVeiculo.BaixarQRCode.visible(_areaVeiculo.QRCode.val() != "");
    _CRUDAreaVeiculo.BaixarTodosQRCode.visible(_areaVeiculo.QRCode.val() != "");
    _CRUDAreaVeiculo.Excluir.visible(isEdicao);
}

function exibirMensagemCamposObrigatorio() {
    exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, informe os campos obrigatórios");
}

function limparCamposAreaVeiculo() {
    LimparCampos(_areaVeiculo);
    limparCamposAreaVeiculoPosicao();
    limparCamposAreaVeiculoTipoRetornoCarga();
    limparCamposAreaVeiculoDesenho();
    controlarBotoesHabilitados();

    $("#tabAreaVeiculoDados").click();
}

function obterAreaVeiculoSalvar() {
    if (!_areaVeiculo.QRCode.val())
        _areaVeiculo.QRCode.val(guid());

    var AreaVeiculo = RetornarObjetoPesquisa(_areaVeiculo);

    AreaVeiculo["Posicoes"] = obterAreaVeiculoPosicaoSalvar();
    AreaVeiculo["TiposRetornoCarga"] = obterAreaVeiculoTipoRetornoCargaSalvar();

    return AreaVeiculo;
}

function recarregarGridAreaVeiculo() {
    _gridAreaVeiculo.CarregarGrid();
}