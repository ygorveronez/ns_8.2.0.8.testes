/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="Bloco.js" />
/// <reference path="Carga.js" />
/// <reference path="Carregamento.js" />
/// <reference path="CarregamentoCarga.js" />
/// <reference path="CarregamentoPedido.js" />
/// <reference path="CarregamentoTransporte.js" />
/// <reference path="DirecoesGoogleMaps.js" />
/// <reference path="Distancia.js" />
/// <reference path="GoogleMaps.js" />
/// <reference path="MontagemCarga.js" />
/// <reference path="OrigemDestino.js" />
/// <reference path="Pedido.js" />
/// <reference path="PedidoProduto.js" />
/// <reference path="PedidosMapa.js" />
/// <reference path="Roteirizador.js" />
/// <reference path="SimulacaoFrete.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _HTMLDetalheCarregamento;
var _AreaCarregamento;
var _detalheCarregamento;
var _percentualCarregamentoAutomatico
var _percentualCargaEmLote

var PercentualCarregamentoAutomatico = function () {
    this.PercentualProcessado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
};

var PercentualCargaEmLote = function () {
    this.PercentualProcessado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
};

var _knoutsCarregamentos = new Array();

var AreaCarregamento = function () {
    this.Carregamentos = PropertyEntity();
    this.CarregandoCarregamentos = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.Inicio = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Total = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, eventChange: CarregamentosPesquisaScroll });
    this.TotalCarregamentos = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Cargas.MontagemCarga.TotalCarregamentos.getFieldDescription() });
    this.GerarCarregamentoAutomatico = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCarga.GerarCarregamento.getFieldDescription(), eventClick: gerarCarregamentosClick, type: types.event, visible: ko.observable(!_CONFIGURACAO_TMS.OcultaGerarCarregamentosMontagemCarga) });
    this.GerarCargaEmLote = PropertyEntity({ text: (_CONFIGURACAO_TMS.OcultaGerarCarregamentosMontagemCarga ? Localization.Resources.Cargas.MontagemCarga.AgendarCargas : Localization.Resources.Cargas.MontagemCarga.GerarCargas), eventClick: gerarCargaEmLoteClick, type: types.event });
};

var DetalheCarregamento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0) });
    this.CarregamentoRedespacho = PropertyEntity({ val: ko.observable(false), text: Localization.Resources.Cargas.MontagemCarga.Redespacho });
    this.NumeroCarregamento = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCarga.NumeroCarregamento.getFieldDescription(), val: ko.observable(""), visible: ko.observable(false) });
    this.Filial = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCarga.Filial.getFieldDescription(), val: ko.observable("") });
    this.QuantidadeEntregras = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCarga.QuantidadeEntregras.getFieldDescription(), val: ko.observable("") });
    this.Rotas = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCarga.Rotas.getFieldDescription(), val: ko.observable("") });
    this.Transportador = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCarga.Transportador.getFieldDescription(), val: ko.observable("") });
    this.Distancia = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCarga.Distancia.getFieldDescription(), val: ko.observable("") });
    this.Peso = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCarga.Peso.getFieldDescription(), val: ko.observable("") });
    this.DataProgramada = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCarga.DataProgramada.getFieldDescription(), val: ko.observable("") });

    this.InfoCarregamento = PropertyEntity({ eventClick: detalharCarregamentoClick, type: types.event, cssClass: ko.observable("card card-carga no-padding padding-5") });
};

function loadDetalhesCarregamento(callback) {
    _AreaCarregamento = new AreaCarregamento();
    KoBindings(_AreaCarregamento, "knoutAreaCarregamento");

    _detalheCarregamento = new DetalheCarregamento();

    _percentualCarregamentoAutomatico = new PercentualCarregamentoAutomatico();
    KoBindings(_percentualCarregamentoAutomatico, "knockoutPercentualCarregamentoAutomatico");

    _percentualCargaEmLote = new PercentualCargaEmLote();
    KoBindings(_percentualCargaEmLote, "knockoutPercentualCargaEmLote");

    LoadSignalRCarregamentoAutomatico();
    LoadSignalRCargaEmLote();

    $.get("Content/Static/MontagemCarga/DetalheCarregamento.html?dyn=" + guid(), function (data) {
        _HTMLDetalheCarregamento = data;
        if (callback != null)
            callback();
    });
}

function detalharCarregamentoClick(e) {
    retornoCarregamento({ Codigo: e.Codigo.val() });
}

function buscarCarregamentos() {
    if (_carregamento.TipoMontagemCarga.val() == EnumTipoMontagemCarga.NovaCarga) {
        var data = RetornarObjetoPesquisa(_pesquisaMontegemCarga);

        data.Inicio = _AreaCarregamento.Inicio.val();
        data.Limite = 200;
        data.SituacaoCarregamento = JSON.stringify(EnumSituacaoCarregamento.obterSituacoesEmMontagem());
        data.TipoMontagemCarga = EnumTipoMontagemCarga.Todos;

        if (_CONFIGURACAO_TMS.TipoMontagemCargaPadrao === EnumTipoMontagemCarga.AgruparCargas)
            data.TipoMontagemCarga = _CONFIGURACAO_TMS.TipoMontagemCargaPadrao;

        _AreaCarregamento.CarregandoCarregamentos.val(true);

        executarReST("MontagemCarga/BuscarCarregamentos", data, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data !== false) {
                    _AreaCarregamento.Total.val(retorno.QuantidadeRegistros);
                    _AreaCarregamento.TotalCarregamentos.val(retorno.QuantidadeRegistros);
                    _AreaCarregamento.Inicio.val(_AreaCarregamento.Inicio.val() + data.Limite);

                    for (var i = 0; i < retorno.Data.length; i++) {
                        var carregamento = retorno.Data[i];
                        var knoutDetalheCarregamento = new DetalheCarregamento();
                        var html = _HTMLDetalheCarregamento.replace(/#detalheCarregamento/g, knoutDetalheCarregamento.InfoCarregamento.id);

                        $("#" + _AreaCarregamento.Carregamentos.id).append(html);

                        KoBindings(knoutDetalheCarregamento, knoutDetalheCarregamento.InfoCarregamento.id);
                        PreencherObjetoKnout(knoutDetalheCarregamento, { Data: carregamento });

                        knoutDetalheCarregamento.Peso.val(carregamento.PesoSaldoRestante);

                        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
                            knoutDetalheCarregamento.NumeroCarregamento.visible(true);

                            if (carregamento.CarregamentoPrioritario)
                                knoutDetalheCarregamento.InfoCarregamento.cssClass("card card-carga-prioritaria no-padding padding-5");
                        }

                        if (carregamento.Carregamento.Situacao == EnumSituacaoCarregamento.AguardandoAprovacaoSolicitacao)
                            knoutDetalheCarregamento.InfoCarregamento.cssClass("card card-carga-aguardando-aprovacao-solicitacao no-padding padding-5");
                        else if (carregamento.Carregamento.Situacao == EnumSituacaoCarregamento.SolicitacaoReprovada)
                            knoutDetalheCarregamento.InfoCarregamento.cssClass("card card-carga-solicitacao-reprovada no-padding padding-5");

                        _knoutsCarregamentos.push(knoutDetalheCarregamento);

                        //index = obterIndiceCarregamento(carregamento);
                        //if (index >= 0)
                        //    knoutDetalheCarregamento.InfoCarregamento.cssClass("well card-carga-selecionada no-padding padding-5");
                    }
                }
                else
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);

            _AreaCarregamento.CarregandoCarregamentos.val(false);
        });
    }
}

function CarregamentosPesquisaScroll(e, sender) {
    var elem = sender.target;
    if (_AreaCarregamento.Inicio.val() < _AreaCarregamento.Total.val() &&
        elem.scrollTop >= (elem.scrollHeight - elem.offsetHeight)) {
        buscarCarregamentos();
    }
}

function obterIndiceCarregamento(carregamento) {
    if (!NavegadorIEInferiorVersao12()) {
        return _carregamento.Carregamentos.val().findIndex(function (item) { return item.Codigo == carregamento.Codigo });
    } else {
        for (var i = 0; i < _carregamento.Carregamentos.val().length; i++) {
            if (carregamento.Codigo == _carregamento.Carregamentos.val()[i].Codigo)
                return i;
        }
        return -1;
    }
}

function obterListaPedidos() {
    var listaPedidos = [];

    for (var i = 0; i < _AreaPedido.Pedidos.val().length; i++) {
        listaPedidos.push(_AreaPedido.Pedidos.val()[i].Codigo)
    }

    return JSON.stringify(listaPedidos);
}

function gerarCarregamentosClick() {
    var data = { pedidos: obterListaPedidos() };
    executarReST("MontagemCarga/GerarCarregamentoAutomatico", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                LimparCamposPercentualCarregamentoAutomatico();
                Global.abrirModal("knockoutPercentualCarregamentoAutomatico");
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function obterListaCarregamento() {
    var listaCarregamento = [];

    for (var i = 0; i < _knoutsCarregamentos.length; i++) {
        listaCarregamento.push(_knoutsCarregamentos[i].Codigo.val())
    }

    return JSON.stringify(listaCarregamento);
}

function gerarCargaEmLoteClick() {
    var data = { codigosCarregamento: obterListaCarregamento() };
    executarReST("MontagemCarga/GerarCargaEmLote", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                LimparCamposPercentualCargaEmLote();
                Global.abrirModal("knockoutPercentualCargaEmLote");
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 15000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }

    });
}

function SetarCarregamentoAutomaticoFinalizado(dados) {
    Global.fecharModal("knockoutPercentualCarregamentoAutomatico");

    if (dados.erro !== "")
        exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, dados.erro);
    else {
        exibirMensagem(tipoMensagem.ok, Localization.Resources.Cargas.MontagemCarga.CarregamentosGeradosComSucesso);
    }
}

function SetarPercentualCarregamentoAutomatico(percentual) {
    var strPercentual = parseInt(percentual) + "%";
    _percentualCarregamentoAutomatico.PercentualProcessado.val(strPercentual);
    $("#" + _percentualCarregamentoAutomatico.PercentualProcessado.id).css("width", strPercentual);
}

function LimparCamposPercentualCarregamentoAutomatico() {
    SetarPercentualCarregamentoAutomatico(0);
    LimparCampos(_percentualCarregamentoAutomatico);
}

function SetarCargaEmLoteFinalizado(dados) {
    Global.fecharModal("knockoutPercentualCargaEmLote");

    if (dados.erro !== "")
        exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, dados.erro);
    else {
        exibirMensagem(tipoMensagem.ok, Localization.Resources.Cargas.MontagemCarga.CargasGeradasComSucesso);
    }
}

function SetarPercentualCargaEmLote(percentual) {
    var strPercentual = parseInt(percentual) + "%";
    _percentualCargaEmLote.PercentualProcessado.val(strPercentual);
    $("#" + _percentualCargaEmLote.PercentualProcessado.id).css("width", strPercentual);
}

function LimparCamposPercentualCargaEmLote() {
    SetarPercentualCargaEmLote(0);
    LimparCampos(_percentualCargaEmLote);
}
