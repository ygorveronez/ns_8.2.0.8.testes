var _denegarChegada;
var _modalDenegarChegada;

var DenegarChegada = function () {
    this.CodigoGuarita = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0) });
    this.Observacao = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), text: "Observação:", maxlength: 300 });

    this.DenegarChegada = PropertyEntity({ text: "Denegar Chegada", type: types.event, eventClick: denegarChegadaClick, visible: ko.observable(true) });
}

function loadDenegarChegada() {
    _denegarChegada = new DenegarChegada();
    KoBindings(_denegarChegada, "knockoutDenegarChegada");

    _modalDenegarChegada = new bootstrap.Modal(document.getElementById("divModalDenegarChegada"), { backdrop: 'static', keyboard: true });
}

function exibirModalDenegarChegada(cargaGrid) {
    _denegarChegada.CodigoGuarita.val(cargaGrid.Codigo);

    _modalDenegarChegada.show();

    $("#divModalDenegarChegada").one('hidden.bs.modal', function () { LimparCampos(_denegarChegada); });
}

function denegarChegadaClick() {
    executarReST("Guarita/DenegarChegada", RetornarObjetoPesquisa(_denegarChegada), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "A chegada foi denegada.");
                _modalDenegarChegada.hide();
                _gridGuarita.CarregarGrid();
            }
            else
                exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}