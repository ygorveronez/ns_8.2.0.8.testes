var _inclusaoMotoristaMDFe;

var InclusaoMotoristaMDFe = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true), required: true });
    this.DataEvento = PropertyEntity({ getType: typesKnockout.dateTime, text: "*Data do evento:", idBtnSearch: guid(), required: true });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarEventoInclusaoMotoristaClick, enable: ko.observable(true), type: types.event, text: "Adicionar Motorista", visible: ko.observable(true) });
}

//*******EVENTOS*******

function LoadInclusaoMotoristaMDFe() {
    _inclusaoMotoristaMDFe = new InclusaoMotoristaMDFe();
    KoBindings(_inclusaoMotoristaMDFe, "knockoutInclusaoMotoristaMDFe");

    new BuscarMotoristas(_inclusaoMotoristaMDFe.Motorista, null, null, null, true);
}

function AdicionarEventoInclusaoMotoristaClick(e, sender) {
    Salvar(e, "ConsultaMDFe/AdicionarMotoristaMDFe", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso!", "A solicitação de inclusão de motorista no MDF-e foi enviada com sucesso.");
                _gridConsultaMDFe.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }

            Global.fecharModal('knockoutInclusaoMotoristaMDFe');
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function AbrirInclusaoMotoristaMDFeClick(e) {

    var dados = { Codigo: e.Codigo };

    if (e.Status != EnumSituacaoMDFe.Autorizado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "A atual situação do MDF-e não permite a inclusão de motorista.");
        return;
    }

    LimparCampos(_inclusaoMotoristaMDFe);

    _inclusaoMotoristaMDFe.Codigo.val(e.Codigo);
    _inclusaoMotoristaMDFe.DataEvento.val(Global.DataHoraAtual());

    Global.abrirModal("knockoutInclusaoMotoristaMDFe");

}