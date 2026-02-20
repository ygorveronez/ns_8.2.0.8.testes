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

var _configuracaoPortoSeguro;

var ConfiguracaoPortoSeguro = function () {
    this.PortoSeguroUsuario = PropertyEntity({ text: "*Usuário:", required: true });
    this.PortoSeguroSenha = PropertyEntity({ text: "*Senha:", required: true });
}

//*******EVENTOS*******

function loadConfiguracaoPortoSeguro() {
    _configuracaoPortoSeguro = new ConfiguracaoPortoSeguro();
    KoBindings(_configuracaoPortoSeguro, "knockoutPortoSeguro");
}
