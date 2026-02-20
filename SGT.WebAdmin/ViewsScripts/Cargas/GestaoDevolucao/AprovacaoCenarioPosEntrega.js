/// <reference path="../../../wwwroot/js/Global/CRUD.js" />
/// <reference path="../../../wwwroot/js/Global/Globais.js" />
/// <reference path="../../../wwwroot/js/Global/Grid.js" />
/// <reference path="../../../wwwroot/js/Global/Mensagem.js" />
/// <reference path="../../../wwwroot/js/Global/Rest.js" />
/// <reference path="../../../ViewsScripts/Enumeradores/EnumTipoGestaoDevolucao.js" />
/// <reference path="../../../ViewsScripts/Enumeradores/EnumSimNao.js" />

//#region Objetos Globais do Arquivo
var _aprovacaoCenarioPosEntrega;
var _gridVisualizacaoProdutos;
var _gridVisualizacaoNotasFiscaisOrigem;
// #endregion Objetos Globais do Arquivo

//#region Classes
var AprovacaoCenarioPosEntrega = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0) });
    this.GridProdutos = PropertyEntity({ id: guid() });
    this.GridNotasFiscais = PropertyEntity({ id: guid() });
    this.NumerosNotasFiscais = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), text: "Números NF-es", visible: ko.observable(true) });
    this.DataEmissao = PropertyEntity({ text: "Data", getType: typesKnockout.dateTime, val: ko.observable(Global.DataHoraAtual()), def: Global.DataHoraAtual() });
    this.Observacao = PropertyEntity({ text: "Observação: ", required: false, maxlength: 500, enable: ko.observable(true) });
    this.TipoDevolucao = PropertyEntity({ val: ko.observable(""), text: "Tipo Devolução", visible: ko.observable(true) });
    this.PosEntrega = PropertyEntity({ visible: ko.observable(true), text: "A Devolução é uma Pós-Entrega?", val: ko.observable(EnumSimNao.Sim), options: ko.observable(EnumSimNao.obterOpcoes()), def: EnumSimNao.Sim, enable: ko.observable(true) });
    this.AlterarTipoDevolucao = PropertyEntity({ visible: ko.observable(false), text: "Alterar Tipo de Devolução", val: ko.observable(), options: ko.observable(EnumTipoGestaoDevolucao.obterOpcoes()), enable: ko.observable(true) });
    this.VisualizarProdutos = PropertyEntity({ type: types.event, eventClick: null, text: "Visualizar produtos", idGrid: guid(), visible: ko.observable(true) });
    this.VisualizarNotasFiscais = PropertyEntity({ type: types.event, eventClick: null, text: "Visualizar notas", idGrid: guid(), visible: ko.observable(true) });
    this.Recusa = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(0) });

    this.Aprovar = PropertyEntity({ type: types.event, eventClick: aprovarCenarioPosEntrega, text: ko.observable("Aprovar"), idGrid: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.SituacaoDevolucao = PropertyEntity({ val: ko.observable(null), visible: ko.observable(false) });
}
//#endregion Classes

// #region Funções de Inicialização
function loadAprovacaoCenarioPosEntrega(etapa) {
    executarReST("GestaoDevolucao/BuscarDadosDevolucaoPorEtapa", buscarInformacoesDevolucao(etapa), function (r) {
        if (r.Success) {
            $.get("Content/Static/Carga/GestaoDevolucao/AprovacaoCenarioPosEntrega.html?dyn=" + guid(), function (data) {
                $("#container-principal-content").html("");
                $("#container-principal-content").html(data);

                _aprovacaoCenarioPosEntrega = new AprovacaoCenarioPosEntrega();
                KoBindings(_aprovacaoCenarioPosEntrega, "knockoutAprovacaoCenarioPosEntrega");

                PreencherObjetoKnout(_aprovacaoCenarioPosEntrega, r);

                controlarAcoesContainerPrincipal(etapa, _aprovacaoCenarioPosEntrega);
                configurarCampos();

                $('#grid-devolucoes').hide();
                $('#container-principal').show();
            });
        }
    });
}
// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos
function loadGridVisualizacaoProdutosPosEntrega() {
    _gridVisualizacaoProdutos = new GridView(_aprovacaoCenarioPosEntrega.GridProdutos.id, "GestaoDevolucao/BuscarProdutosNotasFiscais", _aprovacaoCenarioPosEntrega, null, null, 25, null, null, null, null, null, null, null, null, null, null, null);
    _gridVisualizacaoProdutos.CarregarGrid();
}

function loadGridVisualizacaoNotasFiscaisOrigemPosEntrega() {
    _gridVisualizacaoNotasFiscaisOrigem = new GridView(_aprovacaoCenarioPosEntrega.GridNotasFiscais.id, "GestaoDevolucao/PesquisaNotasFiscaisOrigem", _aprovacaoCenarioPosEntrega, null, null, 25, null, null, null, null, null, null, null, null, null, null, null);
    _gridVisualizacaoNotasFiscaisOrigem.CarregarGrid();
}

function configurarCampos() {
    const tipoServico = _CONFIGURACAO_TMS.TipoServicoMultisoftware;

    if (tipoServico === EnumTipoServicoMultisoftware.MultiEmbarcador) {
        _aprovacaoCenarioPosEntrega.Aprovar.text("Salvar");
        _aprovacaoCenarioPosEntrega.Aprovar.eventClick = salvarPosEntrega;
        _aprovacaoCenarioPosEntrega.PosEntrega.visible(true);
        _aprovacaoCenarioPosEntrega.AlterarTipoDevolucao.visible(false);

    }
    else if (tipoServico === EnumTipoServicoMultisoftware.MultiCTe) {
        _aprovacaoCenarioPosEntrega.PosEntrega.visible(false);
        _aprovacaoCenarioPosEntrega.AlterarTipoDevolucao.enable(true);
        _aprovacaoCenarioPosEntrega.AlterarTipoDevolucao.visible(true);
        _aprovacaoCenarioPosEntrega.Aprovar.enable(true);
    }  

    if (_aprovacaoCenarioPosEntrega.Recusa.val() == 'Total') {
        _aprovacaoCenarioPosEntrega.PosEntrega.val(EnumSimNao.Nao);
        _aprovacaoCenarioPosEntrega.PosEntrega.enable(false);
    }
}

function aprovarCenarioPosEntrega() {
    executarReST("GestaoDevolucao/AprovarPosEntrega", { Codigo: _aprovacaoCenarioPosEntrega.Codigo.val(), TipoGestaoDevolucao: _aprovacaoCenarioPosEntrega.AlterarTipoDevolucao.val() }, function (retorno) {
        if (retorno.Success) {
            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, "Devolução aprovada com sucesso");
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }
    });
}

function salvarPosEntrega() {   
    executarReST("GestaoDevolucao/SalvarPosEntrega", { Codigo: _aprovacaoCenarioPosEntrega.Codigo.val(), Observacao: _aprovacaoCenarioPosEntrega.Observacao.val(), PosEntrega: _aprovacaoCenarioPosEntrega.PosEntrega.val() }, function (retorno) {
        if (retorno.Success) {
            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, retorno.Msg);
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }
    });
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas
function visualizarProdutosPosEntrega() {
    loadGridVisualizacaoProdutosPosEntrega();
    Global.abrirModal("divModalVisualizarProdutosAprovacaoCenarioPosEntrega");
}

function visualizarNotasFiscaisPosEntrega() {
    loadGridVisualizacaoNotasFiscaisOrigemPosEntrega();
    Global.abrirModal("divModalVisualizarNotasFiscaisAprovacaoCenarioPosEntrega");
}
// #endregion Funções Públicas