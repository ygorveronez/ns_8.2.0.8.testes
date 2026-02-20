/// <reference path="JanelaCarregamento.js" />
/// <reference path="TipoTransportadorCarga.js" />
/// <reference path="CalendarioCarregamento.js" />
/// <reference path="ControleCorCarga.js" />
/// <reference path="../../Consultas/CentroCarregamento.js" />
/// <reference path="../../Consultas/RotaFrete.js" />
/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../js/plugin/jquery-countdown/jquery.countdown.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Enumeradores/EnumSituacaoCargaJanelaCarregamento.js" />
/// <reference path="../../Enumeradores/EnumSituacoesCarga.js" />
/// <reference path="../../Enumeradores/EnumTipoCargaJanelaCarregamento.js" />

var _cargaPendente = null;
var _htmlCargaExcedente = '';
var _quantidadeCargasPorVez = 8;

var CargaPendente = function () {

    this.TotalCargasPendentes = PropertyEntity({ val: ko.observable(0), def: 0, eventChange: CargasPendentesScroll, visible: ko.observable(false) });
    this.InicioCargasPendentes = PropertyEntity({ val: ko.observable(0), def: 0, type: types.local });
    this.RequisicaoCargasPendentes = PropertyEntity({ val: ko.observable(false), def: false, type: types.local });
    this.RecarregarCargasPendentes = PropertyEntity({ eventClick: RecarregarCargasPendentes, type: types.event, text: Localization.Resources.Cargas.Carga.Recarregar, visible: ko.observable(true) });
    this.CargasPendentes = ko.observableArray();

    this.TotalCargasExcedentes = PropertyEntity({ val: ko.observable(0), def: 0, eventChange: CargasExcedentesScroll, visible: ko.observable(false) });
    this.InicioCargasExcedentes = PropertyEntity({ val: ko.observable(0), def: 0, type: types.local });
    this.RequisicaoCargasExcedentes = PropertyEntity({ val: ko.observable(false), def: false, type: types.local });
    this.RecarregarCargasExcedentes = PropertyEntity({ eventClick: RecarregarCargasExcedentes, type: types.event, text: Localization.Resources.Cargas.Carga.Recarregar, visible: ko.observable(true) });
    this.CargasExcedentes = ko.observableArray();


    this.TotalCargasEmReserva = PropertyEntity({ val: ko.observable(0), def: 0, eventChange: CargasEmReservaScroll, visible: ko.observable(false) });
    this.InicioCargasEmReserva = PropertyEntity({ val: ko.observable(0), def: 0, type: types.local });
    this.RequisicaoCargasEmReserva = PropertyEntity({ val: ko.observable(false), def: false, type: types.local });
    this.RecarregarCargasEmReserva = PropertyEntity({ eventClick: RecarregarCargasEmReserva, type: types.event, text: Localization.Resources.Cargas.Carga.Recarregar, visible: ko.observable(true) });
    this.CargasEmReserva = ko.observableArray();

    this.OrdenacaoAscDesc = PropertyEntity({
        eventClick: function (e) {
            if (e.OrdenacaoAscDesc.val()) {
                e.OrdenacaoAscDesc.val(false);
                e.OrdenacaoAscDesc.icon("fal fa-arrow-down");
            } else {
                e.OrdenacaoAscDesc.val(true);
                e.OrdenacaoAscDesc.icon("fal fa-arrow-up");
            }
            RecarregarCargasPendentes();
            RecarregarCargasExcedentes();
            RecarregarCargasEmReserva();

            BuscarCargasPendentes();
            BuscarCargasExcedentes();
            BuscarCargasEmReserva();
        }, type: types.event, text: "", idFade: guid(), icon: ko.observable("fal fa-arrow-down"), visible: ko.observable(false)
    });
}

var CargaModel = function (dados) {
    var textoDias = "";

    if (dados.DiasAtrazo > 0)
        textoDias = (dados.DiasAtrazo > 1) ? dados.DiasAtrazo + " dias" : "1 dia";

    this.Codigo = PropertyEntity({ val: ko.observable(dados.Carga.Codigo), def: dados.Carga.Codigo });
    this.CodigoJanelaCarregamento = PropertyEntity({ val: ko.observable(dados.Codigo), def: dados.Codigo });
    this.Numero = PropertyEntity({ text: Localization.Resources.Cargas.Carga.DescricaoCarga, val: ko.observable(dados.Carga.Numero), def: dados.Carga.Numero });
    this.DataCarregamento = PropertyEntity({ text: Localization.Resources.Cargas.Carga.DataDeCarregamento, val: ko.observable("(" + dados.Carga.DataCarregamento + ")"), def: "(" + dados.Carga.DataCarregamento + ")" });
    this.Situacao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Situacao, val: ko.observable(dados.Situacao), def: dados.Situacao, cssClass: ObterClasseSituacaoCarga(dados) });
    this.Destinatario = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Destinatario, val: ko.observable(dados.Destinatario), def: dados.TipoCarga.Descricao });
    this.Destino = PropertyEntity({ text: Localization.Resources.Cargas.CargaDestino, val: ko.observable(dados.Destino), def: dados.TipoCarga.Descricao });
    this.TipoCarga = PropertyEntity({ text: Localization.Resources.Cargas.Carga.TipoDeCarga, val: ko.observable(dados.TipoCarga.Descricao), def: dados.TipoCarga.Descricao });
    this.ModeloVeiculo = PropertyEntity({ text: Localization.Resources.Cargas.Carga.ModeloDeVeiculo, val: ko.observable(dados.ModeloVeiculo.Descricao), def: dados.ModeloVeiculo.Descricao });
    this.Transportador = PropertyEntity({ text: Localization.Resources.Cargas.Carga.Transportador, val: ko.observable(dados.Transportador.Descricao), def: dados.Transportador.Descricao });
    this.TempoCarregamento = PropertyEntity({ text: Localization.Resources.Cargas.Carga.CarregaEm, val: ko.observable(Localization.Resources.Cargas.Carga.Minutos.format(dados.TempoCarregamento)), minutos: dados.TempoCarregamento, def: dados.TempoCarregamento + " minutos" });
    this.ClassificacaoPessoaCor = PropertyEntity({ val: ko.observable(dados.ClassificacaoPessoaCor), def: dados.ClassificacaoPessoaCor });
    this.CargaDeComplemento = PropertyEntity({ val: ko.observable(dados.CargaDeComplemento), def: dados.CargaDeComplemento, getType: typesKnockout.bool });
    this.DataCarga = PropertyEntity({ val: ko.observable(dados.Carga.DataCarga), text: " - " });
    this.DiasAtrazo = PropertyEntity({ val: ko.observable(textoDias), def: "", visible: Boolean(textoDias) });
    this.CarregamentoReservado = PropertyEntity({ val: ko.observable("Reservada"), def: "", visible: dados.CarregamentoReservado });
    this.NaoComparecido = PropertyEntity({ val: ko.observable(EnumTipoNaoComparecimento.obterDescricao(dados.NaoComparecido)), def: "", visible: dados.NaoComparecido != EnumTipoNaoComparecimento.Compareceu });
    this.DataProximaSituacao = PropertyEntity({ text: Localization.Resources.Cargas.Carga.DataDaProximaSituaçao, val: ko.observable(dados.DataProximaSituacao), def: dados.DataProximaSituacao });
    this.DatasAgendadasDivergentes = PropertyEntity({ val: "DATAS AGENDADAS DIVERGENTES", visible: dados.DatasAgendadasDivergentes });
    this.ChegadaDenegada = PropertyEntity({ val: "CHEGADA DENEGADA", visible: dados.ChegadaDenegada });
    this.Interessados = PropertyEntity({ type: types.event, eventClick: InteressadosClick, text: Localization.Resources.Cargas.Carga.Interessado.format(dados.Interessados) + (dados.Interessados > 0 ? "s" : ""), visible: dados.Interessados > 0 && dados.Situacao == EnumSituacaoCargaJanelaCarregamento.SemTransportador ? true : false });
    this.Visualizacoes = PropertyEntity({ type: types.event, eventClick: VisualizacoesClick, text: Localization.Resources.Cargas.Carga.Visualizacoes.format(dados.Visualizacoes), visible: dados.Visualizacoes > 0 && dados.Situacao == EnumSituacaoCargaJanelaCarregamento.SemTransportador ? true : false });
    this.ValidacaoConjunto = PropertyEntity({ text: "Validação Conjunto:", val: ko.observable(dados.RecomendacaoGR), def: "", visible: ko.observable(_CONFIGURACAO_TMS.PossuiIntegracaoKlios && Boolean(dados.RecomendacaoGR)) });
    this.DataLimiteCarregamento = PropertyEntity({ text: Localization.Resources.Cargas.Carga.DataLimiteCarregamento, val: ko.observable(dados.Carga.DataLimiteCarregamento), def: dados.Carga.DataLimiteCarregamento });
    this.CargaPerigosa = PropertyEntity({ val: ko.observable(dados.CargaPerigosa), required: false, text: Localization.Resources.Cargas.Carga.CargaPerigosa, visible: ko.observable(dados.CargaPerigosa), id: guid() });
    this.Dados = dados;
}

function LoadCargaPendente() {
    _cargaPendente = new CargaPendente();
    KoBindings(_cargaPendente, "knockoutCargaPendente");
}

function AdicionarCargaExcedente(dados, indice) {
    var carga = new CargaModel(dados);

    if (indice == null)
        _cargaPendente.CargasExcedentes.push(carga);
    else
        _cargaPendente.CargasExcedentes.replace(_cargaPendente.CargasExcedentes()[indice], carga);

    var dadosEventoCarga = ObterDadosEventoCarga(dados);
    dadosEventoCarga["stick"] = false; // maintain when user navigates (see docs on the renderEvent method)

    $('#' + carga.Codigo.id).data('event', dadosEventoCarga);

    if (_FormularioSomenteLeitura == false) {
        // make the event draggable using jQuery UI
        $('#' + carga.Codigo.id).draggable({
            zIndex: 999,
            revert: true,
            revertDuration: 0,
            helper: function (event) {
                return "<div style='height: " + event.currentTarget.offsetHeight + "px; width: " + event.currentTarget.offsetWidth + "px;'>" + event.currentTarget.innerHTML + "</div>";
            }
        });
    }

    $("#" + carga.Codigo.id).click(function () {
        if ((dados.Tipo == EnumTipoCargaJanelaCarregamento.Carregamento) && (dados.Situacao == EnumSituacaoCargaJanelaCarregamento.AgLiberacaoParaTransportadores) && ((dados.Carga.SituacaoCarga != EnumSituacoesCarga.Nova) || dados.Carga.ExigeNotaFiscalParaCalcularFrete))
            if (dados.Carga.Codigo > 0)
                exibirTipoTransportadorCarga(dados, event);
            else
                ExibirDetalhesCarga(dados);
    });
}

function AdicionarCargaPendente(dados, indice) {
    var carga = new CargaModel(dados);

    if (indice == null)
        _cargaPendente.CargasPendentes.push(carga);
    else
        _cargaPendente.CargasPendentes.replace(_cargaPendente.CargasPendentes()[indice], carga);

    $("#" + carga.DataProximaSituacao.id)
        .countdown(moment(dados.DataProximaSituacao, "DD/MM/YYYY HH:mm").format("YYYY/MM/DD HH:mm:ss"), { elapse: false, precision: 1000 })
        .on('update.countdown', function (event) {
            if (event.offset.totalDays > 0)
                $(this).text(event.strftime('%-Dd %H:%M:%S'));
            else
                $(this).text(event.strftime('%H:%M:%S'));
        })
        .on('finish.countdown', function (event) {
            $(this).text(Localization.Resources.Cargas.Carga.PrazoEsgotado);
        });

    $("#" + carga.Codigo.id).click(function (event) {
        if ((dados.Tipo == EnumTipoCargaJanelaCarregamento.Carregamento) && (dados.Situacao == EnumSituacaoCargaJanelaCarregamento.AgLiberacaoParaTransportadores) && ((dados.Carga.SituacaoCarga != EnumSituacoesCarga.Nova) || dados.Carga.ExigeNotaFiscalParaCalcularFrete))
            exibirTipoTransportadorCarga(dados, event);
        else
            ExibirDetalhesCarga(dados);
    });

    if (carga.CargaPerigosa.val() == true)
        $('#' + carga.CargaPerigosa.id).css({ "color": "#000000", "font-weight": "bold", "font-size": "15px" });
    else
        $('#' + carga.CargaPerigosa.id).css({ "color": "", "font-weight": "", "font-size": "" });
}

function ObterClasseSituacaoCarga(carga) {
    var controleCorCarga = new ControleCorCarga();

    return Boolean(carga.Cores) ? Global.ObterClasseDinamica(carga.Cores) : controleCorCarga.ObterClasse(carga);
}

function ObterDadosEventoCarga(carga) {
    var dadosEventoCarga = new DadosEventoCarga();

    return dadosEventoCarga.ObterDadosEventoCarga(carga);
}

function CargasPendentesScroll(e, sender) {
    var elem = sender.target;
    if (_cargaPendente.InicioCargasPendentes.val() < _cargaPendente.TotalCargasPendentes.val() &&
        elem.scrollTop >= (elem.scrollHeight - elem.offsetHeight)) {
        BuscarCargasPendentes();
    }
}

function BuscarCargasPendentes() {
    if (!_cargaPendente.RequisicaoCargasPendentes.val()) {
        _cargaPendente.RequisicaoCargasPendentes.val(true);

        // Monta os dados de atualização herdando os dados de pesquisa
        var data = $.extend({}, _dadosPesquisaCarregamento, {
            Inicio: _cargaPendente.InicioCargasPendentes.val(),
            Limite: _quantidadeCargasPorVez,
            OrdenacaoAscDesc: _cargaPendente.OrdenacaoAscDesc.val()
        });

        executarReST("JanelaCarregamento/ObterCargasPendentes", data, function (arg) {
            if (arg.Success) {
                _cargaPendente.TotalCargasPendentes.val(arg.Data.Total);
                _cargaPendente.InicioCargasPendentes.val(_cargaPendente.InicioCargasPendentes.val() + _quantidadeCargasPorVez);

                $.each(arg.Data.Cargas, function (i, carga) {
                    AdicionarCargaPendente(carga);
                });
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }

            _cargaPendente.RequisicaoCargasPendentes.val(false);
        }, function () {
            _cargaPendente.TotalCargasPendentes.val(0);
            _cargaPendente.InicioCargasPendentes.val(_quantidadeCargasPorVez);

            _cargaPendente.RequisicaoCargasPendentes.val(false);
        }, false);
    }
}

function RecarregarCargasPendentes() {
    ExibirAbaPendete();
    _cargaPendente.TotalCargasPendentes.val(0);
    _cargaPendente.InicioCargasPendentes.val(0);
    _cargaPendente.CargasPendentes.removeAll();
    BuscarCargasPendentes();
}

function CargasExcedentesScroll(e, sender) {
    var elem = sender.target;
    if (_cargaPendente.InicioCargasExcedentes.val() < _cargaPendente.TotalCargasExcedentes.val() &&
        elem.scrollTop >= (elem.scrollHeight - elem.offsetHeight)) {
        BuscarCargasExcedentes();
    }
}

function CargasEmReservaScroll(e, sender) {
    var elem = sender.target;
    if (_cargaPendente.InicioCargasEmReserva.val() < _cargaPendente.TotalCargasEmReserva.val() &&
        elem.scrollTop >= (elem.scrollHeight - elem.offsetHeight)) {
        BuscarCargasEmReserva();
    }
}

function BuscarCargasExcedentes() {
    if (!_cargaPendente.RequisicaoCargasExcedentes.val()) {
        _cargaPendente.RequisicaoCargasExcedentes.val(true);

        // Monta os dados de atualização herdando os dados de pesquisa
        var data = $.extend({}, _dadosPesquisaCarregamento, {
            Inicio: _cargaPendente.InicioCargasExcedentes.val(),
            Limite: _quantidadeCargasPorVez,
            OrdenacaoAscDesc: _cargaPendente.OrdenacaoAscDesc.val()
        });

        executarReST("JanelaCarregamento/ObterCargasExcedentes", data, function (arg) {
            _listaCarregamento.CargaMovidaParaExcedente(0);
            if (arg.Success) {
                _cargaPendente.TotalCargasExcedentes.val(arg.Data.Total);
                _cargaPendente.InicioCargasExcedentes.val(_cargaPendente.InicioCargasExcedentes.val() + _quantidadeCargasPorVez);

                $.each(arg.Data.Cargas, function (i, carga) {
                    AdicionarCargaExcedente(carga);
                });
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }

            _cargaPendente.RequisicaoCargasExcedentes.val(false);
        }, function () {
            _cargaPendente.TotalCargasExcedentes.val(0);
            _cargaPendente.InicioCargasExcedentes.val(_quantidadeCargasPorVez);

            _cargaPendente.RequisicaoCargasExcedentes.val(false);
        }, false);
    }
}

function AdicionarCargaEmReserva(dados, indice) {
    var carga = new CargaModel(dados);

    if (indice == null)
        _cargaPendente.CargasEmReserva.push(carga);
    else
        _cargaPendente.CargasEmReserva.replace(_cargaPendente.CargasEmReserva()[indice], carga);

    var dadosEventoCarga = ObterDadosEventoCarga(dados);
    dadosEventoCarga["stick"] = false; // maintain when user navigates (see docs on the renderEvent method)
}

function BuscarCargasEmReserva() {
    if (!_cargaPendente.RequisicaoCargasEmReserva.val()) {
        _cargaPendente.RequisicaoCargasEmReserva.val(true);

        // Monta os dados de atualização herdando os dados de pesquisa
        var data = $.extend({}, _dadosPesquisaCarregamento, {
            Inicio: _cargaPendente.InicioCargasEmReserva.val(),
            Limite: _quantidadeCargasPorVez,
            OrdenacaoAscDesc: _cargaPendente.OrdenacaoAscDesc.val()
        });

        executarReST("JanelaCarregamento/ObterCargasEmReserva", data, function (arg) {
            if (arg.Success) {
                _cargaPendente.TotalCargasEmReserva.val(arg.Data.Total);
                _cargaPendente.InicioCargasEmReserva.val(_cargaPendente.InicioCargasEmReserva.val() + _quantidadeCargasPorVez);

                $.each(arg.Data.Cargas, function (i, carga) {
                    AdicionarCargaEmReserva(carga);
                });
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
            _cargaPendente.RequisicaoCargasEmReserva.val(false);
        }, function () {
            _cargaPendente.TotalCargasEmReserva.val(0);
            _cargaPendente.InicioCargasEmReserva.val(_quantidadeCargasPorVez);

            _cargaPendente.RequisicaoCargasEmReserva.val(false);
        }, false);
    }
}

function RecarregarCargasExcedentes() {
    ExibirAbaExcedentes();
    _cargaPendente.TotalCargasExcedentes.val(0);
    _cargaPendente.InicioCargasExcedentes.val(0);
    _cargaPendente.CargasExcedentes.removeAll();
    BuscarCargasExcedentes();
}

function RecarregarCargasEmReserva() {
    ExibirAbaReserva();
    _cargaPendente.TotalCargasEmReserva.val(0);
    _cargaPendente.InicioCargasEmReserva.val(0);
    _cargaPendente.CargasEmReserva.removeAll();
    BuscarCargasEmReserva();
}

function InteressadosClick(e, sender) {
    AbrirTelaInteressadosCarga(e.Codigo.val(), sender);
}
function VisualizacoesClick(e, sender) {
    AbrirTelaVisualizacoesCarga(e.Codigo.val(), sender);
}