var _informarKmAtualVeiculoTracao, _validafechamento;

var InformarKmAtualVeiculoTracao = function () {
    this.Reboque = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), visible: ko.observable(false) });
    this.Veiculo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), visible: ko.observable(false) });
    this.Placa = PropertyEntity({ text: "Veículo: ", getType: typesKnockout.string, visible: ko.observable(true), enable: ko.observable(false) });
    this.KmAtual = PropertyEntity({ text: "*KM Atual: ", getType: typesKnockout.int, val: ko.observable(""), maxlength: 30, required: true });
    this.TipoMovimento = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), visible: ko.observable(false) });
    this.Salvar = PropertyEntity({ eventClick: salvarKmAtualVeiculoTracaoClick, type: types.event, text: "Salvar", visible: ko.observable(true) });    
}

function loadInformarKmAtualVeiculoTracao() {
    _informarKmAtualVeiculoTracao = new InformarKmAtualVeiculoTracao();    
    KoBindings(_informarKmAtualVeiculoTracao, "knockoutInformarKmAtualVeiculoTracao");
    _informarKmAtualVeiculoTracao.Placa.enable(false);
}

function exibirModalInformarKmAtualVeiculoTracao(veiculo, reboque, tipo, exibir) {             
    _validafechamento = false;
    let p = new promise.Promise();
    if (exibir) {
        _informarKmAtualVeiculoTracao.Veiculo.val(veiculo);
        _informarKmAtualVeiculoTracao.Reboque.val(reboque);
        _informarKmAtualVeiculoTracao.TipoMovimento.val(tipo);
        executarReST("Veiculo/BuscarPorCodigo", { codigo: veiculo }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    var veiculo = arg.Data;
                    _informarKmAtualVeiculoTracao.Placa.val(veiculo.Placa);
                    Global.abrirModal('divModalInformarKmAtualVeiculoTracao');

                    $("#divModalInformarKmAtualVeiculoTracao").on('hidden.bs.modal', function () {
                        LimparCampos(_informarKmAtualVeiculoTracao);                                                
                        p.done(_validafechamento);                             
                    });
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                p.done(_validafechamento);
            }
        });
    } else {
        p.done(true);
    }
    return p;
}

function salvarKmAtualVeiculoTracaoClick() {
    executarReST("MovimentacaoDePlacas/SalvarKmAtualVeiculoTracao", {
        Reboque: _informarKmAtualVeiculoTracao.Reboque.val(),
        Veiculo: _informarKmAtualVeiculoTracao.Veiculo.val(),
        KmAtual: _informarKmAtualVeiculoTracao.KmAtual.val(),
        TipoMovimento: _informarKmAtualVeiculoTracao.TipoMovimento.val()
    }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _validafechamento = true;
                exibirMensagem(tipoMensagem.ok, "Sucesso", "A Km Atual Veiculo Tracao foi salva!");
                Global.fecharModal('divModalInformarKmAtualVeiculoTracao');
            }
            else
                exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}