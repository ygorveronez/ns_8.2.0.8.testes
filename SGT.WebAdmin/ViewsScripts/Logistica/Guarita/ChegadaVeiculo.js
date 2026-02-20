/// <reference path="../../../js/Global/Mensagem.js" />

var _alterarChegada;
var _opcoes;

const AlterarDataChegada =  function (){
    this.Carga = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.DataChegada = PropertyEntity({ text: "Nova data chegada:", getType: typesKnockout.dateTime });

}

const Opcoes = function () {
    this.Atualizar = PropertyEntity({ eventClick: atualizarDataClick, type: types.event, text: "Atualizar", idGrid: guid(), visible: ko.observable(true) });
}

function loadAlterarChegada() {
    _alterarChegada = new AlterarDataChegada();
    KoBindings(_alterarChegada, "knockoutAlterarDataChegaVeiculo");

    _opcoes = new Opcoes();
    KoBindings(_opcoes, "knockoutAtualizarData");

}

function alterarDataChegaVeiculo(e) {
    LimparCampos(_alterarChegada);
    Global.abrirModal('divModalAlterarDataChegada');
    _alterarChegada.Carga.val(e.Codigo);
}

function atualizarDataClick() {
    const data = {
        Carga: _alterarChegada.Carga.val(),
        DataChegada: _alterarChegada.DataChegada.val()
    }

    executarReST("Guarita/AtualizarDataChegadaVeiculo", data, (args) => {
        if (!args.Success) {
          return  exibirMensagem(tipoMensagem.falha, args.Msg);
        }

        Global.fecharModal('divModalAlterarDataChegada');
        exibirMensagem(tipoMensagem.ok, "Sucesso", args.Msg);


    })
}