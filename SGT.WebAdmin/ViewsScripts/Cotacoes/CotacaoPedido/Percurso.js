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
/// <reference path="../../Enumeradores/EnumTipoClienteCotacaoPedido.js" />
/// <reference path="../../Enumeradores/EnumStatusCotacaoPedido.js" />
/// <reference path="../../Enumeradores/EnumTipoModal.js" />
/// <reference path="CotacaoPedido.js" />

var _cotacaoPedidoPercurso;
var _mapaMonitoramento;

var CotacaoPedidoPercurso = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
};

//*******EVENTOS*******

function loadCotacaoPedidoPercurso() {
    _cotacaoPedidoPercurso = new CotacaoPedidoPercurso();
    KoBindings(_cotacaoPedidoPercurso, "knockoutPercurso");

    loadMapa();
}

function loadMapa() {
    if (!_mapaMonitoramento) {
        var opcoesmapa = new OpcoesMapa(false, false);
        _mapaMonitoramento = new MapaGoogle("mapPercursoCotacao", false, opcoesmapa);
    }
}

//**********METODOS*********

function carregarDadosMapaPercursoCotacao() {
    var data = {
        CNPJClienteAtivo: _cotacaoPedido.ClienteAtivo.codEntity(),
        CNPJClienteInativo: _cotacaoPedido.ClienteInativo.codEntity(),
        CodigoLocalidadeDestino: _localidadeDestino.Localidade.codEntity(),
        CodigoLocalidadeOrigem: _localidadeOrigem.Localidade.codEntity(),
        CNPJDestinatario: _cotacaoPedido.Destinatario.codEntity()
    };
    if ((data.CNPJClienteAtivo > 0 || data.CNPJClienteInativo > 0) && (data.CNPJDestinatario > 0)) {
        executarReST("CotacaoPedido/ObterDadosMapa", data, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    setTimeout(function () {
                        _mapaMonitoramento.clear();
                        criarMakerTrajeto(arg.Data);
                    }, 350);
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    } else
        limparCamposPercurso();
}

function criarMakerTrajeto(info) {
    if ((typeof info.Latitude) === "string")
        info.Latitude = Globalize.parseFloat(info.Latitude);

    if ((typeof info.Longitude) === "string")
        info.Longitude = Globalize.parseFloat(info.Longitude);

    if ((typeof info.LatitudeDestino) === "string")
        info.LatitudeDestino = Globalize.parseFloat(info.LatitudeDestino);

    if ((typeof info.LongitudeDestino) === "string")
        info.LongitudeDestino = Globalize.parseFloat(info.LongitudeDestino);

    if (!isNaN(info.Latitude) && !isNaN(info.Longitude) && !isNaN(info.LatitudeDestino) && !isNaN(info.LongitudeDestino)) {
        var listapontos = [];

        var pontoOrigem = new PontosRota("Origem", info.Latitude, info.Longitude);
        listapontos.push(pontoOrigem);

        var pontoDestino = new PontosRota("Destino", info.LatitudeDestino, info.LongitudeDestino);
        listapontos.push(pontoDestino);

        _mapaMonitoramento.direction.setZoom(17);
        _mapaMonitoramento.direction.roteirizar(listapontos, EnumTipoUltimoPontoRoteirizacao.AteOrigem, function (resposta) {
            if (resposta.status !== "OK")
                exibirMensagem(tipoMensagem.atencao, "Atenção", "Erro ao roteirizar. " + resposta.status);
            else
                _mapaMonitoramento.direction.setZoom(17);
        });

        //gerarRoteirizacaoGoogleMaps();
    }
}

function limparCamposPercurso() {
    _mapaMonitoramento.clear();
    _mapaMonitoramento.direction.limparMapa();
}