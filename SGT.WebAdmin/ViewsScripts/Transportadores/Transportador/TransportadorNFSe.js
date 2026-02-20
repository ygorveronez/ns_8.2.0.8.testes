/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../Enumeradores/EnumTipoSerie.js" />
/// <reference path="Transportador.js" />
/// <reference path="../../Configuracao/Sistema/ConfiguracaoTMS.js" />
/// <reference path="../../Enumeradores/EnumTipoImpressaoPedidoVenda.js" />
/// <reference path="../../Enumeradores/EnumTipoLancamentoFinanceiroSemOrcamento.js" />
/// <reference path="../../Enumeradores/EnumTipoEmissaoIntramunicipal.js" />
/// <reference path="../../Enumeradores/EnumPerfilEmpresa.js" />
/// <reference path="../../Consultas/TipoMovimento.js" />

var _transportadorNFSe;

var TransportadorNFSe = function () {
    this.EmissaoNFSeForaDoSistema = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.EsseTransportadorIraEmitirNFSForaDoMultiEmbarcador, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador) });
    this.NaoIncrementarNumeroLoteRPSAutomaticamente = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.NaoIncrementarNumeroLoteRPSAutomaticamente, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador) });
    this.NFSeNacional = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.UtilizaServicoNacional, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador) });
};

//*******EVENTOS*******

function loadTransportadorNFSe() {
    _transportadorNFSe = new TransportadorNFSe();
    KoBindings(_transportadorNFSe, "knockoutTransportadorNFSe");

    loadTransportadorConfiguracaoNFSe();
    loadAutomacaoNFSManual();
}

function limparCamposTransportadorNFSe() {
    LimparCampos(_transportadorNFSe);

    limparCamposTransportadorConfiguracaoNFSe();
    limparAutomacaoNFSManual();
}