/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _manobristas;

var Manobristas = function () {
    this.QuantidadeDias = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.QuantidadeDias.getFieldDescription() });
    this.QuantidadeMotoristas = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.QuantidadeMotoristas.getFieldDescription() });
    this.ValorDiaria = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.ValorDiaria.getFieldDescription() });
    this.ValorQuinzena = PropertyEntity({ text: Localization.Resources.Ocorrencias.AutorizacaoOcorrencia.ValorQuinzena.getFieldDescription() });
}

//*******EVENTOS*******

function LoadManobristas() {
    _manobristas = new Manobristas();
    KoBindings(_manobristas, "knockoutManobrista");
}


//*******MÉTODOS*******

function PreecherManobristas(dados) {
    if (dados != null) {
        $("#liManobrista").show();
        PreencherObjetoKnout(_manobristas, { Data: dados })
    }
}

function LimparManobristas() {
    $("#liManobrista").hide();
    LimparCampos(_manobristas);
}