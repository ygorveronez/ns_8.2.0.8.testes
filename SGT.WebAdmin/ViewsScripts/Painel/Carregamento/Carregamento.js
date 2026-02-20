/// <reference path="../../Enumeradores/EnumEtapaFluxoGestaoPatio.js" />
/// <reference path="../../Enumeradores/EnumTipoFluxoGestaoPatio.js" />

var _controleAutomatico = true;
var _painelCarregamento;
var _timeOutCarregamento;
var _tempoParaTroca = 30000;
var _UltimaFilial = null;
var _UltimaDataInicial = null;
var _UltimaDataFim = null;

var _paginasPrevistas = 1;
var _paginaAtual = 1;
var _paginou = false;

var _HTMLItemCarregamento;
var _dataDia = "";

var PesquisaCarregamento = function () {
    var dataAtual = moment().format("DD/MM/YYYY");
    var tipoPadrao = _CONFIGURACAO_TMS.GerarFluxoPatioDestino ? EnumTipoFluxoGestaoPatio.Todos : EnumTipoFluxoGestaoPatio.Origem;

    this.DataInicial = PropertyEntity({ text: "*Data Inicial:", val: ko.observable(dataAtual), def: dataAtual, getType: typesKnockout.date, visible: ko.observable(true), required: true });
    this.DataFinal = PropertyEntity({ text: "*Data Final:", val: ko.observable(dataAtual), def: dataAtual, getType: typesKnockout.date, visible: ko.observable(true), required: true });
    this.Filial = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "* Filial:", idBtnSearch: guid(), required: ko.observable(true) });
    this.Tipo = PropertyEntity({ val: ko.observable(tipoPadrao), options: EnumTipoFluxoGestaoPatio.obterOpcoesPesquisa(), text: "Tipo: ", def: tipoPadrao, visible: ko.observable(_CONFIGURACAO_TMS.GerarFluxoPatioDestino) });
    this.EtapaFluxoGestaoPatio = PropertyEntity({ val: ko.observable([]), def: [], getType: typesKnockout.selectMultiple, text: "Etapa:", options: ko.observableArray([]) });

    this.SomenteFluxosAbertos = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, text: "Somente Fluxos Abertos" });

    this.DataInicial.dateRangeLimit = this.DataFinalCarregamento;
    this.DataFinal.dateRangeInit = this.DataInicialCarregamento;

    this.Tipo.val.subscribe(recarregarSituacoesFiliais);

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            pesquisarFluxoCarga(1, false);
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(false)
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
}

var Carregamento = function () {
    this.Carga = PropertyEntity();
    this.Destino = PropertyEntity();
    this.ModeloVeicular = PropertyEntity();
    this.AreaVeiculo = PropertyEntity();
    this.Hora = PropertyEntity();
    this.Motorista = PropertyEntity();
    this.Placas = PropertyEntity();
    this.Situacao = PropertyEntity();
    this.Carregamento = PropertyEntity({ cssClass: ko.observable("") });
}

function loadPainelCarregamento() {
    LimparTimeOut();
    _painelCarregamento = new PesquisaCarregamento();
    KoBindings(_painelCarregamento, "knoutPainelCarregamento", false);
    new BuscarFilial(_painelCarregamento.Filial);

    _dataDia = moment().format("DD/MM/YYYY");
    _painelCarregamento.Filial.multiplesEntities.subscribe(recarregarSituacoesFiliais);

    BuscarFilialPadrao();

    $.get("Content/Static/Painel/ItemCarregamento.html?dyn=" + guid(), function (data) {
        _HTMLItemCarregamento = data;

        carregarCarregamentos(_paginaAtual, _paginou, executarPesquisaTimeOut);

        $(window).one('hashchange', function () {
            LimparTimeOut();
        });
    });
}

function recarregarSituacoesFiliais() {
    var filiais = recursiveMultiplesEntities(_painelCarregamento.Filial);

    if (filiais.length == 0) {
        _painelCarregamento.EtapaFluxoGestaoPatio.options([]);
        SetarSelectMultiple(_painelCarregamento.EtapaFluxoGestaoPatio);
        return;
    }

    executarReST("FluxoPatio/ObterMultiplasEtapasDisponiveis", { Filiais: JSON.stringify(filiais), Tipo: _painelCarregamento.Tipo.val() }, function (retorno) {
        if (retorno.Success && retorno.Data) {
            var etapasFluxoPatio = [];

            for (var i = 0; i < retorno.Data.length; i++) {
                etapasFluxoPatio.push({
                    text: retorno.Data[i].Descricao,
                    value: retorno.Data[i].Enumerador
                });
            }

            _painelCarregamento.EtapaFluxoGestaoPatio.options(etapasFluxoPatio);
            SetarSelectMultiple(_painelCarregamento.EtapaFluxoGestaoPatio);
        }
    });
}

function BuscarFilialPadrao() {
    executarReST("DadosPadrao/ObterFilial", {}, function (r) {
        if (r.Success && r.Data) {
            var data = r.Data;

            _painelCarregamento.Filial.multiplesEntities([]);
            _painelCarregamento.Filial.multiplesEntities([{ Codigo: data.Codigo, Descricao: data.Descricao }]);
        }
    });
}

function executarPesquisaTimeOut() {
    validarPaginacao();
    _timeOutCarregamento = setTimeout(function () {
        carregarCarregamentos(_paginaAtual, _paginou, executarPesquisaTimeOut);
    }, _tempoParaTroca);
}

function validarPaginacao() {
    if (_paginaAtual >= _paginasPrevistas) {
        _paginaAtual = 1;
        _paginou = false;
    } else {
        _paginaAtual++;
        _paginou = true;
    }
}

function carregarCarregamentos(page, paginou, callback) {

    var datadia = moment().format("DD/MM/YYYY");
    if (_dataDia != datadia) {
        _painelCarregamento.DataFinal.val(datadia);
        _painelCarregamento.DataInicial.val(datadia);
    };

    var itensPorPagina = 10;

    if (_UltimaDataFim == null || _UltimaDataFim != _painelCarregamento.DataFinal.val()) {
        _UltimaDataFim = _painelCarregamento.DataFinal.val();
        page = 1;
        paginou = false;
    }

    if (_UltimaDataInicial == null || _UltimaDataInicial != _painelCarregamento.DataInicial.val()) {
        _UltimaDataInicial = _painelCarregamento.DataInicial.val();
        page = 1;
        paginou = false;
    }

    if (_UltimaFilial == null || _UltimaFilial != _painelCarregamento.Filial.codEntity()) {
        _UltimaFilial = _painelCarregamento.Filial.codEntity();
        page = 1;
        paginou = false;
    }

    var data = RetornarObjetoPesquisa(_painelCarregamento);
    data.inicio = itensPorPagina * (page - 1);
    data.limite = itensPorPagina;

    //var data = {
    //    inicio: itensPorPagina * (page - 1),
    //    limite: itensPorPagina,
    //};

    _ControlarManualmenteProgresse = true;
    executarReST("Carregamentos/ObterCarregamentos", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                $("#divItensCarregamento").html("");
                var dotted = false;
                for (var i = 0; i < arg.Data.length; i++) {
                    var carregamento = arg.Data[i];

                    var knoutCarregamento = new Carregamento();
                    var dataCarregamento = { Data: carregamento };
                    PreencherObjetoKnout(knoutCarregamento, dataCarregamento);

                    if (dotted) {
                        dotted = false;
                        knoutCarregamento.Carregamento.cssClass("row divbodyCarregamento bgBodyDotted");
                    } else {
                        dotted = true;
                        knoutCarregamento.Carregamento.cssClass("row divbodyCarregamento ");
                    }

                    var idCarregamento = carregamento.Codigo + "_Carregamento";
                    $("#divItensCarregamento").append(_HTMLItemCarregamento.replace(/#idItemCarregamento/g, idCarregamento));
                    KoBindings(knoutCarregamento, idCarregamento);
                }
                if (!paginou) {
                    if (arg.QuantidadeRegistros > 0) {
                        _paginasPrevistas = arg.QuantidadeRegistros / itensPorPagina;
                    } else {
                        $("#divItensCarregamento").html('<div class="row divbodyCarregamento p-3" style="text-align:center"><h2>Nenhum Registro Encontrado</h2></span>');
                    }
                }
                callback();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
                callback();
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            callback();
        }

        _ControlarManualmenteProgresse = false;
        finalizarRequisicao();
    });
}

function LimparTimeOut() {
    if (_timeOutCarregamento != null)
        clearTimeout(_timeOutCarregamento);
}
