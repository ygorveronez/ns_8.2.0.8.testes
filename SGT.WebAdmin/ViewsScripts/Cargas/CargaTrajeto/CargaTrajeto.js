/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../Enumeradores/EnumSituacaoTrajeto.js"/>

var _cargaTrajetoPrincipal;
var _HTMLCargaCargaTrajeto;
var _pesquisaCargaTrajeto;

var PesquisaCargaTrajeto = function () {
    this.Carga = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), text: "Número Carga:" });
    this.SituacaoTrajeto = PropertyEntity({ val: ko.observable(0), options: EnumSituacaoTrajeto.obterOpcoesPesquisa(), text: "Situação Trajeto:" });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            buscarCargaTrajeto(1, false);
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e == undefined) return;
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            }
            else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var CargaTrajetoCarga = function () {
    this.Etapa = PropertyEntity({
        text: ko.observable(""), type: types.local, enable: ko.observable(true), visible: ko.observable(true), idGrid: guid(), idTab: guid(), eventClick: function () { },
        step: ko.observable(1),
        tooltip: ko.observable(""),
        tooltipTitle: ko.observable(""),
        cssClass: ko.observable("")
    });

    this.Carga = PropertyEntity({ getType: typesKnockout.string, val: ko.observable("") });
    this.Informacoes = PropertyEntity({ getType: typesKnockout.string, val: ko.observable("") });
    this.Ordem = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0) });
    this.SituacaoTrajetoCarga = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0) });
}

var CargaTrajeto = function () {
    this.TamanhoEtapa = PropertyEntity({ getType: typesKnockout.string, val: ko.observable("") });
    this.Etapas = PropertyEntity({ type: types.map, val: ko.observableArray() });

    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0) });

    this.KmPercorrido = PropertyEntity({ getType: types.string, val: ko.observable("") });
    this.TransportadorAtual = PropertyEntity({ getType: types.string, val: ko.observable("") });
    this.CidadeInicial = PropertyEntity({ getType: types.string, val: ko.observable("") });
    this.DataInicial = PropertyEntity({ getType: types.string, val: ko.observable("") });

    this.CidadeFinal = PropertyEntity({ getType: types.string, val: ko.observable("") });
    this.DataFinal = PropertyEntity({ getType: types.string, val: ko.observable("") });
}

var CargaTrajetoPrincipal = function () {
    this.Trajetos = PropertyEntity({ type: types.map, val: ko.observableArray([]) });
}

function loadCargaTrajeto() {
    _cargaTrajetoPrincipal = new CargaTrajetoPrincipal();
    _pesquisaCargaTrajeto = new PesquisaCargaTrajeto();

    KoBindings(_pesquisaCargaTrajeto, "knockoutPesquisaCargaTrajeto");
    KoBindings(_cargaTrajetoPrincipal, "knockoutCargaTrajeto");

    loadCargasCargaTrajeto();

    buscarCargaTrajeto(0, false);
}

function loadCargasCargaTrajeto() {
    return $.get("Content/Static/Carga/CargaTrajeto/CargaTrajeto.html?dyn=" + guid(), function (data) {
        _HTMLCargaCargaTrajeto = data;
    });
}

function buscarCargaTrajeto(page, paginou) {
    var itensPorPagina = 10;

    var data = RetornarObjetoPesquisa(_pesquisaCargaTrajeto);

    data.inicio = itensPorPagina * (page - 1);
    data.limite = itensPorPagina;

    executarReST("CargaTrajeto/Pesquisa", data, function (retorno) {
        if (retorno.Success)
        {
            if (retorno.Data) {
                PreencherObjetoKnout(_cargaTrajetoPrincipal, retorno);

                for (var j = 0; j < retorno.Data.Trajetos.length; j++) {
                    var listaEtapas = new Array();
                    var registroAtual = retorno.Data.Trajetos[j];

                    for (var i = 0; i < registroAtual.Etapas.length; i++) {
                        var _cargaEtapa = new CargaTrajetoCarga();

                        _cargaEtapa.Informacoes.val(registroAtual.Etapas[i].Informacoes);
                        _cargaEtapa.Carga.val(registroAtual.Etapas[i].Carga);
                        _cargaEtapa.Ordem.val(registroAtual.Etapas[i].Ordem);

                        _cargaEtapa.SituacaoTrajetoCarga.val(registroAtual.Etapas[i].SituacaoTrajetoCarga);

                        if (registroAtual.Etapas[i].SituacaoTrajetoCarga == EnumSituacaoTrajetoCarga.AguardandoInicio)
                            _cargaEtapa.Etapa.cssClass("step");
                        else if (registroAtual.Etapas[i].SituacaoTrajetoCarga == EnumSituacaoTrajetoCarga.EmTransporte)
                            _cargaEtapa.Etapa.cssClass("step yellow");
                        else
                            _cargaEtapa.Etapa.cssClass("step green");

                        listaEtapas.push(_cargaEtapa);
                    }

                    _cargaTrajetoPrincipal.Trajetos.val()[j].Etapas = listaEtapas;
                }

                configurarTrajetos();
                configurarPaginacao(page, paginou, retorno, itensPorPagina);
            }
            else 
                exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });

}

function configurarTrajetos() {
    $("#knockoutCargaTrajeto").html("");

    for (var i = 0; i < _cargaTrajetoPrincipal.Trajetos.val().length; i++) {
        var registroAtual = _cargaTrajetoPrincipal.Trajetos.val()[i];
        var trajeto = new CargaTrajeto();
        
        trajeto.KmPercorrido.val(registroAtual.KmPercorrido);
        trajeto.TransportadorAtual.val(registroAtual.TransportadorAtual);
        trajeto.DataInicial.val(registroAtual.DataInicial);
        trajeto.DataFinal.val(registroAtual.DataFinal);
        trajeto.DataInicial.val(registroAtual.DataInicial);
        trajeto.DataFinal.val(registroAtual.DataFinal);
        trajeto.CidadeInicial.val(registroAtual.CidadeInicial);
        trajeto.CidadeFinal.val(registroAtual.CidadeFinal);
        trajeto.Etapas = registroAtual.Etapas;
        trajeto.TamanhoEtapa.val(100.0 / registroAtual.Etapas.length + "%");

        var idTrajeto = registroAtual.Codigo + "_CargaTrajetoCarga";
        $("#knockoutCargaTrajeto").append(_HTMLCargaCargaTrajeto.replace(/#idCargaTrajeto/g, idTrajeto));
        KoBindings(trajeto, idTrajeto);
    }

    $("[rel=popover-hover]").popover({ trigger: "hover", html: true });
}

function configurarPaginacao(page, paginou, retorno, itensPorPagina) {
    if (!paginou) {
        if (retorno.QuantidadeRegistros > 0) {
            $("#divPaginationCargaTrajeto").html('<ul style="float:right" id="paginacaoCargaTrajeto" class="pagination"></ul>');
            var paginas = Math.ceil((retorno.QuantidadeRegistros / itensPorPagina));
            $('#paginacaoCargaTrajeto').twbsPagination({
                first: 'Primeiro',
                prev: 'Anterior',
                next: 'Próximo',
                last: 'Último',
                totalPages: paginas,
                visiblePages: 5,
                onPageClick: function (event, page) {
                    buscarCargaTrajeto(page, true);
                }
            });
        }
        else
            $("#divPaginationCargaTrajeto").html('<span>Nenhum Registro Encontrado</span>');
    }
}