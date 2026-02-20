var _configuracaoPagbem = null;

var ConfiguracaoPagbem = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.URLPagbem = PropertyEntity({ text: "URL:", maxlength: 500 });
    this.UsuarioPagbem = PropertyEntity({ text: "Usuário:", maxlength: 100 });
    this.SenhaPagbem = PropertyEntity({ text: "Senha:", maxlength: 100 });
    this.CNPJEmpresaContratante = PropertyEntity({ text: "CNPJ Contratante:", maxlength: 14, getType: typesKnockout.string });    
    this.TipoFilialContratantePagbem = PropertyEntity({ text: "Filial Contratante:", options: EnumTipoFilialContratantePagbem.obterOpcoes(), val: ko.observable(EnumTipoFilialContratantePagbem.Empresa), def: EnumTipoFilialContratantePagbem.Empresa, issue: 0, visible: ko.observable(true) });
    this.NaoIntegrarResponsavelCartaoPagbem = PropertyEntity({ text: "Não integrar responsável do cartão", val: ko.observable(false), def: false, issue: 0, visible: ko.observable(true) });
    this.IntegrarNumeroRPSNFSE = PropertyEntity({ text: "Integrar número do RPS quando NFS-e", val: ko.observable(false), def: false, issue: 0, visible: ko.observable(true) });
    this.LiberarViagemManualmente = PropertyEntity({ text: "Liberar a viagem manualmente", val: ko.observable(false), def: false, issue: 0, visible: ko.observable(true) });
    this.UtilizarCnpjContratanteIntegracao = PropertyEntity({ text: "Utilizar CNPJ Contratante para integração do CIOT", val: ko.observable(false), def: false, enable: ko.observable(true) });

    this.TipoTolerancia = PropertyEntity({ text: "Tipo Tolerância:", maxlength: 100, getType: typesKnockout.string });    
    this.FreteTipoPeso = PropertyEntity({ text: "Frete tipo peso:", maxlength: 100, getType: typesKnockout.string });    
    this.QuebraTipoCobranca = PropertyEntity({ text: "Quenta tipo cobrança:", maxlength: 100, getType: typesKnockout.string });
    this.QuebraTolerancia = PropertyEntity({ text: "Quebra tolerância:", maxlength: 100, getType: typesKnockout.decimal });    
};

function LoadConfiguracaoPagbem() {
    _configuracaoPagbem = new ConfiguracaoPagbem();
    KoBindings(_configuracaoPagbem, "tabPagbem");
}

function LimparCamposConfiguracaoPagbem() {
    LimparCampos(_configuracaoPagbem);
}