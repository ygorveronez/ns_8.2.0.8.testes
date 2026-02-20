var _tanques;
var _tanquesRemover;
var _tanquesAtualizar;
function Tanques() {
    this.Tanques = ko.observableArray();
    this.Tanques.subscribe(function (newValue) {
        _tanquesRemover = new Array();
        _tanquesAtualizar = new Array();
        if (newValue.length > 0)
            $("#liTabTanques").show();
        else
            $("#liTabTanques").hide();
    });
}

function LoadTanques() {
    _tanques = new Tanques();
    KoBindings(_tanques, "knockoutTanques");
}

function RemoverTanque(codigo) {
    if (!_tanquesRemover.includes(codigo)){
        _tanquesRemover.push(codigo);
        $("#TanqueMapeamento_" + codigo).hide();

    } else {
        exibirMensagem("atencao", Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Filiais.Filial.EsteCodigoJaFoiInformadoParaRemocao);
    }
}

function AtualizarTanque(codigo) {

    if (!_tanquesAtualizar.includes(codigo)) {
        _tanquesAtualizar.push(codigo);

        exibirMensagem("aviso", Localization.Resources.Gerais.Geral.Atencao, "Tanque atualizado, favor atualizar a filial.");
    } else {
        exibirMensagem("atencao", Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Filiais.Filial.EsteCodigoJaFoiInformadoParaRemocao);
    }
}

function ObterTanquesRemover() {
    return JSON.stringify(_tanquesRemover);
}

function ObterTanquesAdicionar() {
    return JSON.stringify(_tanquesAtualizar);
}