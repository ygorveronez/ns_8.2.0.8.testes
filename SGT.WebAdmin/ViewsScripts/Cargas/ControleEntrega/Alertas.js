/*Alertas.js*/
var _dadosAlerta;
var _mapAlerta;


var DadosAlerta = function () {
    this.CodigoAlerta = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Alerta.getFieldDescription() });
    this.Data = PropertyEntity({ getType: typesKnockout.string, text: ko.observable(Localization.Resources.Cargas.ControleEntrega.DataInicio.getFieldDescription()) });
    this.DataFim = PropertyEntity({ getType: typesKnockout.string, text: ko.observable(Localization.Resources.Cargas.ControleEntrega.DataFim.getFieldDescription()) });
    this.ObservacaoMotorista = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Cargas.ControleEntrega.ObservacaoDoMotorista.getFieldDescription(), visible: ko.observable(false) });
    this.TipoAlerta = PropertyEntity({});
    this.Latitude = PropertyEntity({});
    this.Longitude = PropertyEntity({});
    this.Observacao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Observacao.getRequiredFieldDescription(), getType: typesKnockout.string, enable: ko.observable(true), val: ko.observable(""), maxlength: 2000, visible: ko.observable(false) });
    this.Tratativa = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Tratativa.getRequiredFieldDescription(), val: ko.observable(true), enable: ko.observable(true), options: ko.observable([]), def: 1, visible: ko.observable(false) });

    this.UtilizaTratativa = PropertyEntity({ val: ko.observable(true) });

    this.TempoParado = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Cargas.ControleEntrega.TempoParado.getFieldDescription(), visible: ko.observable(false) });
    this.ValorAlerta = PropertyEntity({ getType: typesKnockout.string, text: "", visible: ko.observable(false) });

    this.Confirmar = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: confirmarTratativaAlertaClick, text: Localization.Resources.Cargas.ControleEntrega.Finalizar, visible: ko.observable(false) });
    this.Fechar = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: fecharTratativaAlertaClick, text: Localization.Resources.Cargas.ControleEntrega.Fechar, visible: ko.observable(true) });
}

/*
 * Declaração das Funções
 */
function buscarAcoesTratativaAlerta(codigoAlerta) {
    executarReST("AlertaTratativaAcao/BuscarPorTipoAlerta", { Codigo: codigoAlerta }, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                _dadosAlerta.Tratativa.options(arg.Data.TiposAcao);


                _dadosAlerta.UtilizaTratativa.val(arg.Data.TiposAcao.length > 0);

                var visivel = _dadosAlerta.UtilizaTratativa.val();

                if (_dadosAlerta.TipoAlerta.val() == 3) {
                    setarTratamentoPorTipoAlerta(_dadosAlerta);
                }

                setarVisibilidadeControles(visivel);

                exibeModalEtapa("divModalAlertas");

                carregarMapaAlerta();

                criarMakerAlerta({ lat: _dadosAlerta.Latitude.val(), lng: _dadosAlerta.Longitude.val() })


            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });

}

function setarVisibilidadeControles(visivel) {

    if (visivel) {
        _dadosAlerta.Observacao.visible(true);
        _dadosAlerta.Tratativa.visible(true);
    }

    var finalizado = !_dadosAlerta.DataFim.val() != "";

    _dadosAlerta.Confirmar.visible(finalizado);

}

function exibirDetalhesAlerta(alerta, alertas) {
    loadTratativaAlerta(alerta, [])
}

function setarTratamentoPorTipoAlerta(alerta) {

    //alerta do tipo Parada Nao Programada (tem uma tratativa diferente) #25952
    alerta.Data.text(Localization.Resources.Cargas.ControleEntrega.DataAlerta);
    alerta.DataFim.text(Localization.Resources.Cargas.ControleEntrega.DataTratativa);
    var valor = alerta.ValorAlerta.val();
    if (valor != null && valor.length > 5) {
        alerta.Data.val(alerta.ValorAlerta.val());
        alerta.TempoParado.val(valor.substr(valor.length - 5, 5));
        alerta.TempoParado.visible(true);
    }

}

function loadDadosAlerta(alerta) {
    _dadosAlerta = new DadosAlerta();
    KoBindings(_dadosAlerta, "knockouControleEntregaAlerta");

    PreencherObjetoKnout(_dadosAlerta, { Data: alerta });
    if (_dadosAlerta.ObservacaoMotorista.val() != "")
        _dadosAlerta.ObservacaoMotorista.visible(true);
    else
        _dadosAlerta.ObservacaoMotorista.visible(false);

    buscarAcoesTratativaAlerta(alerta.CodigoAlerta);
}

function confirmarTratativaAlertaClick(e, sender) {


    Salvar(_dadosAlerta, "AlertaTratativa/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.CadastradoComSucesso);

                Global.fecharModal("divModalAlertas");

                atualizarControleEntrega();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    }, sender);


}

function fecharTratativaAlertaClick() {

    Global.fecharModal("divModalAlertas");

}

function carregarMapaAlerta() {

    if (_mapAlerta == null) {
        opcoesMapa = new OpcoesMapa(false, false);

        _mapAlerta = new MapaGoogle("mapaControleEntregaAlerta", false, opcoesMapa);
    }

    _mapAlerta.clear();
}

function criarMakerAlerta(coordenada) {

    if ((coordenada.lat == 0) && (coordenada.lng == 0))
        return;

    if ((typeof coordenada.lat) == "string")
        coordenada.lat = Globalize.parseFloat(coordenada.lat);

    if ((typeof coordenada.lng) == "string")
        coordenada.lng = Globalize.parseFloat(coordenada.lng);

    var marker = new ShapeMarker();
    marker.setPosition(coordenada.lat, coordenada.lng);
    marker.icon = _mapAlerta.draw.icons.truck();
    marker.title = '';
    _mapAlerta.draw.addShape(marker);
    _mapAlerta.direction.centralizar(coordenada.lat, coordenada.lng);
}





