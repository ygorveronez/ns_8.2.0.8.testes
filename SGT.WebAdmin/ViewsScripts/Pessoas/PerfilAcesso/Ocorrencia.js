var Ocorrencia = function () {
    this.PermitirAbrirOcorrenciaAposPrazoSolicitacao = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, text: Localization.Resources.Pessoas.PerfilAcesso.PermitirAbrirOcorrenciaDataSuperior, visible: ko.observable(true) });
}

function loadOcorrencia() {
    _ocorrencia = new Ocorrencia();

    KoBindings(_ocorrencia, "knoutOcorrencias");

    _ocorrencia.PermitirAbrirOcorrenciaAposPrazoSolicitacao.val(_perfil.PermitirAbrirOcorrenciaAposPrazoSolicitacao.val());    

}


function preencherOcorrencia() {
    _perfil.PermitirAbrirOcorrenciaAposPrazoSolicitacao.val(_ocorrencia.PermitirAbrirOcorrenciaAposPrazoSolicitacao.val());    
}

function limparCamposOcorrencia(){
    LimparCampos(_ocorrencia);
}