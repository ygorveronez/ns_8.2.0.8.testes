var _pesagemLoteInterno;
var _pesagemLoteInternoCRUD;

var PesagemLoteInterno = function () {
    this.FluxoGestaoPatio = PropertyEntity({ val: ko.observable(0), getType: typesKnockout.int });
    
    this.LoteInterno = PropertyEntity({ text: "*Lote Interno 1:", val: ko.observable(""), getType: typesKnockout.string, maxlength: 12, required: true });
    this.LoteInternoDois = PropertyEntity({ text: "Lote Interno 2:", val: ko.observable(""), getType: typesKnockout.string, maxlength: 12, required: false });
};

var PesagemLoteInternoCRUD = function () {
    this.SalvarPesagemLoteInterno = PropertyEntity({ eventClick: salvarPesagemLoteInternoClick, type: types.event, text: "Salvar Informações de Lote", visible: ko.observable(true) });
};

function LoadPesagemLoteInterno() {
    _pesagemLoteInterno = new PesagemLoteInterno();
    KoBindings(_pesagemLoteInterno, "knockoutPesagemLoteInterno");
    
    _pesagemLoteInternoCRUD = new PesagemLoteInternoCRUD();
    KoBindings(_pesagemLoteInternoCRUD, "knockoutPesagemLoteInternoCRUD");
}

function abrirPesagemLoteInternoClick(codigoFluxoGestaoPatio) {
    _pesagemLoteInterno.FluxoGestaoPatio.val(codigoFluxoGestaoPatio);

    executarReST("Guarita/BuscarInformacoesPesagemLoteInterno", { FluxoGestaoPatio: _pesagemLoteInterno.FluxoGestaoPatio.val() }, function (r) {
        if (r.Success) {
            if (r.Data) {
                PreencherObjetoKnout(_pesagemLoteInterno, r);
                
                Global.abrirModal('divModalPesagemLoteInterno');
                
                $("#divModalPesagemLoteInterno").one('hidden.bs.modal', function () {
                    LimparCampos(_pesagemLoteInterno);
                });
            } else {
                exibirMensagem(tipoMensagem.aviso, "Atenção!", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }
    });
}

function salvarPesagemLoteInternoClick(e, sender) {
    Salvar(_pesagemLoteInterno, "Guarita/SalvarInformacoesPesagemLoteInterno", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Dados de lote salvos.");
                Global.fecharModal('divModalPesagemLoteInterno');
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}
