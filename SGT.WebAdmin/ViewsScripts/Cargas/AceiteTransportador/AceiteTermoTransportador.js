var _aceiteTermoTransporte;

var AceiteTermoTransporte = function () {
    this.CodigoJanelaCarregamentoTransportador = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.CodigoCarga = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.TermoAceite = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), enable: ko.observable(false), id: "termo-aceite" });
    this.Aceitar = PropertyEntity({ eventClick: aceitarTermoAceiteClick, type: types.event, text: "Aceitar", idGrid: guid(), visible: ko.observable(true), enable: ko.observable(false) });

    this.CallBack = PropertyEntity({ val: null, type: types.local });
}

function loadAceiteTermoTransportador() {
    var $divModal = $("#divModalAceiteTermoTransporte");

    if ($divModal.length == 0) {
        $.get("Content/Static/Carga/AceiteDoTransportador.html" + "?dyn=" + guid(), function (data) {
            $("#widget-grid").after(data);
        }).done(function () {
            _aceiteTermoTransporte = new AceiteTermoTransporte();
            KoBindings(_aceiteTermoTransporte, "knockoutAceiteTermoTransporte");

            $('#termo-aceite').scroll(function () {
                if ($(this).scrollTop() + $(this).innerHeight() + 2 >= $(this)[0].scrollHeight) {
                    _aceiteTermoTransporte.Aceitar.enable(true);
                }
            });
        });
    }
}

function exibirModalAceiteTermoTransporte(data) {
    _aceiteTermoTransporte.CallBack.val = data.Callback;
    _aceiteTermoTransporte.TermoAceite.val(data.TermoAceite);
    _aceiteTermoTransporte.CodigoJanelaCarregamentoTransportador.val(data.CodigoJanelaCarregamentoTransportador);
    _aceiteTermoTransporte.CodigoCarga.val(data.CodigoCarga);
    
    $("#divModalAceiteTermoTransporte")
        .modal('show')
        .one('shown.bs.modal', function () {
            var elemento = $("#termo-aceite")[0];
            var temScrollVertical = elemento.scrollHeight > elemento.clientHeight;
            _aceiteTermoTransporte.Aceitar.enable(!temScrollVertical);
        })
        .one('hide.bs.modal', function () {
            $('#termo-aceite').scrollTop(0);
            LimparCampos(_aceiteTermoTransporte);
        });
}

function fecharModalAceiteTermoTransporte() {
    Global.fecharModal('divModalAceiteTermoTransporte');
}

function aceitarTermoAceiteClick(e) {
    executarReST("JanelaCarregamentoTransportador/AceitarTermoDeAceite", { Codigo: _aceiteTermoTransporte.CodigoJanelaCarregamentoTransportador.val(), CodigoCarga: _aceiteTermoTransporte.CodigoCarga.val(), TermoAceite: _aceiteTermoTransporte.TermoAceite.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                fecharModalAceiteTermoTransporte();
                exibirMensagem(tipoMensagem.ok, "Sucesso", retorno.Msg);

                if (_aceiteTermoTransporte.CallBack.val != null)
                    _aceiteTermoTransporte.CallBack.val();

                if (typeof(_pesquisaJanelaCarregamentoTransportador) != 'undefined') {
                    var cargas = _pesquisaJanelaCarregamentoTransportador.Cargas();

                    for (var i = 0; i < cargas.length; i++) {
                        if (cargas[i].Codigo.val() == retorno.Data.Codigo) {
                            AdicionarCarga(retorno.Data, i);
                            break;
                        }
                    }
                }
            }
            else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
            }
        }
        else {
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }
    });
}