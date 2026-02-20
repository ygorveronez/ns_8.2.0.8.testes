//*******MAPEAMENTO KNOUCKOUT*******

var _loteControleTacografo;

var _situacao = [
    { text: "Entregue", value: 1 },
    { text: "Recebido", value: 2 },
    { text: "Perdido", value: 3 },
    { text: "Extraviado", value: 4 }
];

var LoteControleTacografo = function () {
    this.DataRecebimento = PropertyEntity({ text: "*Data Repasse ao Motorista:", getType: typesKnockout.date, required: ko.observable(true) });
    this.Quantidade = PropertyEntity({ text: "*Quantidade:", getType: typesKnockout.int, val: ko.observable(1), def: 1, codEntity: ko.observable(0), required: ko.observable(true) });
    this.Situacao = PropertyEntity({ text: "*Situacao:", options: _situacao, def: 1, required: ko.observable(true) });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(true), options: _status, def: true });
    this.Motorista = PropertyEntity({ text: "*Motorista:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), required: ko.observable(true) });
    this.Veiculo = PropertyEntity({ text: "*Veículo:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), required: ko.observable(true) });

    this.Observacao = PropertyEntity({ text: "Observação:", maxlength: 5000, val: ko.observable(""), enable: ko.observable(true) });

    this.Cancelar = PropertyEntity({ eventClick: fecharLoteTacografoClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
    this.Gerar = PropertyEntity({ eventClick: gerarLoteControleTacografoClick, type: types.event, text: "Gerar", visible: ko.observable(true) });

};

//*******EVENTOS*******

function LoadLoteControleTacografo() {
    _loteControleTacografo = new LoteControleTacografo();
    KoBindings(_loteControleTacografo, "knockoutLoteControleTacografo");

    new BuscarMotoristas(_loteControleTacografo.Motorista, RetornoMotoristaLote);
    new BuscarVeiculos(_loteControleTacografo.Veiculo);
}

function gerarLoteControleTacografoClick(e, sender) {
    Salvar(_loteControleTacografo, "ControleTacografo/AdicionarEmLote", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridControleTacografo.CarregarGrid();
                limparCamposControleTacografo();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);

    fecharLoteTacografoClick();
}

//*******MÉTODOS*******

function RetornoMotoristaLote(data) {
    _loteControleTacografo.Motorista.codEntity(data.Codigo);
    _loteControleTacografo.Motorista.val(data.Descricao);

    executarReST("Veiculo/BuscarVeiculoDoMotorista", { Codigo: data.Codigo }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _loteControleTacografo.Veiculo.codEntity(arg.Data.Codigo);
                _loteControleTacografo.Veiculo.val(arg.Data.Placa);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function abrirTelaLoteTacografo() {
    Global.abrirModal('knockoutLoteControleTacografo');
    $("#knockoutLoteControleTacografo").one('hidden.bs.modal', function () {
        LimparCamposLoteTacografo();
    });
}

function fecharLoteTacografoClick() {
    Global.fecharModal('knockoutLoteControleTacografo');
}

function LimparCamposLoteTacografo() {
    LimparCampos(_loteControleTacografo);
}
