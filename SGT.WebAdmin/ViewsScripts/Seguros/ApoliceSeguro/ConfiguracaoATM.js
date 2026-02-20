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
/// <reference path="../../Enumeradores/EnumVersaoLayoutATM.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _configuracaoATM;

var ConfiguracaoATM = function () {
    this.ATMCodigo = PropertyEntity({ text: "*Codigo:", required: true });
    this.ATMUsuario = PropertyEntity({ text: "*Usuário:", required: true });
    this.ATMSenha = PropertyEntity({ text: "*Senha:", required: true });
    this.ATMAverbaComoEmbarcador = PropertyEntity({ text: "Averba Como Embarcador. (Gera XML fictício, onde o emitente é o tomador)", val: ko.observable(false) , def: false });
    this.ATMAverbarNFeQuandoCargaPossuirNFSManual = PropertyEntity({ text: "Averbar a NF-e quando a carga possuir NFS Manual", val: ko.observable(false), def: false });
    this.VersaoLayoutATMOutrosDocumentos = PropertyEntity({ val: ko.observable(EnumVersaoLayoutATM.Versao200), def: EnumVersaoLayoutATM.Versao200, options: EnumVersaoLayoutATM.obterOpcoes(), enable: ko.observable(true), text: "Versão Layout Outros Documentos:" });
}

//*******EVENTOS*******

function loadConfiguracaoATM() {
    _configuracaoATM = new ConfiguracaoATM();
    KoBindings(_configuracaoATM, "knockoutATM");
}
