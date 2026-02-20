/// <reference path="RotaFrete.js" />

var _cep;

var CEP = function () {

    this.CEPs = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, idGrid: guid(), codEntity: ko.observable(0) });
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoRota = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CEPInicial = PropertyEntity({ text: Localization.Resources.Logistica.RotaFrete.CEPInicial.getRequiredFieldDescription(), required: false, maxlength: 15, val: ko.observable("") });
    this.CEPFinal = PropertyEntity({ text: Localization.Resources.Logistica.RotaFrete.CEPFinal.getRequiredFieldDescription(), required: false, maxlength: 15, val: ko.observable("") });
    this.LeadTime = PropertyEntity({ text: Localization.Resources.Logistica.RotaFrete.LeadTime.getFieldDescription(), getType: typesKnockout.int, required: false, maxlength: 12, val: ko.observable("") });
    this.PercentualADValorem = PropertyEntity({ text: Localization.Resources.Logistica.RotaFrete.ADValorem.getFieldDescription(), getType: typesKnockout.decimal, required: false, maxlength: 6, val: ko.observable("") });

    this.AdicionarCEP = PropertyEntity({ eventClick: AdicionarCEPClick, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Adicionar), visible: ko.observable(true) });
}


//*******EVENTOS*******

function loadRotaCEP() {
    _cep = new CEP();
    KoBindings(_cep, "knockoutRotaCEP");

    CarregarRotaFreteCEP();
    $("#" + _cep.CEPInicial.id).mask("00.000-000", { selectOnFocus: true, clearIfNotMatch: true });
    $("#" + _cep.CEPFinal.id).mask("00.000-000", { selectOnFocus: true, clearIfNotMatch: true });
}


function AdicionarCEPClick(e, sender) {
    var tudoCerto = true;
    if (_cep.CEPInicial.val() == "")
        tudoCerto = false;
    if (_cep.CEPFinal.val() == "")
        tudoCerto = false;

    if (tudoCerto) {
        _cep.CodigoRota.val(_rotaFrete.Codigo.val());
        Salvar(_cep, "RotaFrete/SalvarCEP", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.SalvoComSucesso);

                    _gridRotaFreteCEP.CarregarGrid();
                    limparCEP();
                    _cep.AdicionarCEP.text(Localization.Resources.Gerais.Geral.Adicionar);
                    $("#" + _cep.CEPInicial.id).focus();

                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        }, sender, function () {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        });

    } else {

        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Logistica.RotaFrete.InformeCamposObrigatoriosNoLancamento);
    }
}

function ExcluirCEPClick(e) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Logistica.RotaFrete.RealmenteDesejaRemoverFaixaDeCEP, function () {
        _cep.CEPInicial.val(e.CEPInicial);
        _cep.CEPFinal.val(e.CEPFinal);
        _cep.LeadTime.val(e.LeadTime);
        _cep.PercentualADValorem.val(e.PercentualADValorem);
        _cep.Codigo.val(e.Codigo);
        ExcluirPorCodigo(_cep, "RotaFrete/ExcluirCEP", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ExcluidoComSucesso);
                    _gridRotaFreteCEP.CarregarGrid();
                    limparCEP();
                    _cep.AdicionarCEP.text(Localization.Resources.Gerais.Geral.Adicionar);
                    $("#" + _cep.CEPInicial.id).focus();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        }, null);

    });
}

function EditarCEPClick(e) {
    _cep.CEPInicial.val(e.CEPInicial);
    _cep.CEPFinal.val(e.CEPFinal);
    _cep.LeadTime.val(e.LeadTime);
    _cep.PercentualADValorem.val(e.PercentualADValorem);
    _cep.Codigo.val(e.Codigo);

    _cep.AdicionarCEP.text("Atualizar");
}

function limparCEP() {
    $("#" + _cep.CEPInicial.id).val("");
    $("#" + _cep.CEPFinal.id).val("");

    _cep.CEPInicial.val("");
    _cep.CEPFinal.val("");
    _cep.LeadTime.val("");
    _cep.PercentualADValorem.val("");
    _cep.Codigo.val("");
}