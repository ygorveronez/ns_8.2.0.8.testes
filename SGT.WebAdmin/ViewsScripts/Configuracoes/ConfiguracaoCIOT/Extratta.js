var _configuracaoExtratta = null;

var ConfiguracaoExtratta = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.URLAPI = PropertyEntity({ text: "URL API: ", maxlength: 200 });
    this.CNPJAplicacao = PropertyEntity({ text: "CNPJ Aplicação: ", maxlength: 14 });
    this.Token = PropertyEntity({ text: "Token:", maxlength: 50 });
    this.UtilizarCNPJAplicacaoPreenchimentoCNPJEmpresa = PropertyEntity({ text: "Utilizar CNPJ Aplicação para preenchimento do CNPJ Empresa", val: ko.observable(false), def: false, enable: ko.observable(true) });
    this.PrefixoCampoNumeroControle = PropertyEntity({ text: "Prefixo campo NumeroControle:", maxlength: 3 });
    this.ForcarCIOTNaoEquiparado = PropertyEntity({ text: "Forçar CIOT não Equiparado", val: ko.observable(false), def: false, enable: ko.observable(true) });
    this.UtilizarTipoGeracaoCIOTPreenchimentoHabilitarContratoCiotAgregado = PropertyEntity({ text: "Utilizar Tipo Geração CIOT para preenchimento do campo HabilitarContratoCiotAgregado", val: ko.observable(false), def: false, enable: ko.observable(true) });
    this.EnviarQuantidadesMaioresQueZeroExtratta = PropertyEntity({ text: "Enviar quantidades(peso, volumes e valor de mercadoria) sempre maiores que zero", val: ko.observable(false), def: false, enable: ko.observable(true) });
    this.NaoRealizarQuitacaoViagemEncerramentoCIOT = PropertyEntity({ text: "Não realizar quitação da viagem no encerramento do CIOT", val: ko.observable(false), def: false, enable: ko.observable(true) });
    this.NomeUsuarioExtratta = PropertyEntity({ text: "Usuário:", maxlength: 100, required: ko.observable(false) });
    this.DocumentoUsuarioExtratta = PropertyEntity({ text: "Documento usuário:", maxlength: 50, required: ko.observable(false) });
    this.EnviarCarretaViagemV2 = PropertyEntity({ text: "Enviar Carreta Viagem V2", val: ko.observable(false), def: false, enable: ko.observable(true) });    

};

function LoadConfiguracaoExtratta() {
    _configuracaoExtratta = new ConfiguracaoExtratta();
    KoBindings(_configuracaoExtratta, "tabExtratta");
}

function LimparCamposConfiguracaoExtratta() {
    LimparCampos(_configuracaoExtratta);
}