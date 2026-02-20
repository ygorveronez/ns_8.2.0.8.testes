/// <reference path="../../../../js/Global/CRUD.js" />
/// <reference path="../../../../js/Global/Globais.js" />
/// <reference path="../../../../js/Global/Grid.js" />
/// <reference path="../../../../js/Global/Mensagem.js" />
/// <reference path="../../../../js/Global/Rest.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridCargaDadosZonaTransporte;
var _cargaDadosZonaTransporte;
var _HTMLCargaDadosZonaTransporte;

var CargaDadosZonaTransporte = function () {
    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Grid = PropertyEntity({ idGrid: guid(), visible: ko.observable(false), enable: ko.observable(false) });
};

//*******EVENTOS*******

function loadCargaDadosZonaTransporte(knoutCarga) {
    CarregarHTMLCargaDadosTransporteZonaTransporte().then(function () {

        SetarHTMLCargaDadosZonaTransporte("tabCargaDadosZonaTransporte_" + knoutCarga.EtapaInicioTMS.idGrid, knoutCarga.DivCarga.id);

        _cargaDadosZonaTransporte = new CargaDadosZonaTransporte();
        KoBindings(_cargaDadosZonaTransporte, "divContainerCargaDadosZonaTransporte_" + knoutCarga.DivCarga.id);

        _cargaDadosZonaTransporte.Carga.val(knoutCarga.Codigo.val());

        LocalizeCurrentPage();

        buscarCargaDadosZonaTransporte();

        $("#litabCargaDadosZonaTransporte_" + knoutCarga.EtapaInicioTMS.idGrid).removeClass("d-none");
    });
}

//*******MÉTODOS*******

function buscarCargaDadosZonaTransporte() {
    var callbackOrdenacao = function (retorno, reverterOrdenacao) { reordenarSequenciaZonaTransporte(retorno, reverterOrdenacao); };
    var quantidadePorPagina = 50;

    _gridCargaDadosZonaTransporte = new GridView(_cargaDadosZonaTransporte.Grid.idGrid, "CargaDadosZonaTransporte/ObterZonasTransportes", _cargaDadosZonaTransporte, null, { column: 2, dir: orderDir.asc }, quantidadePorPagina, null, null, null, null, null, null, null, callbackOrdenacao);
    _gridCargaDadosZonaTransporte.CarregarGrid();
}

function reordenarSequenciaZonaTransporte(retornoOrdenacao, reverterOrdenacao) {
    var dados = {
        Carga: _cargaDadosZonaTransporte.Carga.val(),
        Codigo: retornoOrdenacao.itemReordenado.idLinha,
        NovaSequencia: retornoOrdenacao.itemReordenado.posicaoAtual
    };

    executarReST("CargaDadosZonaTransporte/ReordenarSequenciaZonasTransporte", dados, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.AtualizadoComSucesso);

                _gridCargaDadosZonaTransporte.CarregarGrid();
            }
            else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
                reverterOrdenacao();
            }
        }
        else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
            reverterOrdenacao();
        }
    });
}

function CarregarHTMLCargaDadosTransporteZonaTransporte() {
    var p = new promise.Promise();

    if (_HTMLCargaDadosZonaTransporte == null) {
        $.get("Content/Static/Carga/CargaDadosZonaTransporte.html?dyn=" + guid(), function (data) {
            _HTMLCargaDadosZonaTransporte = data;
            p.done();
        });
    } else {
        p.done();
    }

    return p;
}

function SetarHTMLCargaDadosZonaTransporte(idContent, idReplace) {
    $("#" + idContent).html(_HTMLCargaDadosZonaTransporte.replace(/#idDivCarga/g, idReplace));
}