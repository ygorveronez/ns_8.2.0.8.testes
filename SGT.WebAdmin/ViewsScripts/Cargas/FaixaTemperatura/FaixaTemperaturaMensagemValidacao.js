var _faixaTemperaturaMensagemValidacao;

/*
 * Declaração das Classes
 */

var FaixaTemperaturaMensagemValidacao = function () {
    this.MensagemLicencaVencidaEmbarcador = PropertyEntity({ text: "Mensagem Licença Vencida Embarcador:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 300 });
    this.MensagemLicencaVencidaTransportador = PropertyEntity({ text: "Mensagem Licença Vencida Transportador:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 300 });
    this.MensagemLicencaReprovadaEmbarcador = PropertyEntity({ text: "Mensagem Licença Reprovada Embarcador:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 300 });
    this.MensagemLicencaReprovadaTransportador = PropertyEntity({ text: "Mensagem Licença Reprovada Transportador:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 300 });
}
/*
 * Declaração das Funções de Inicialização
 */

function loadFaixaTemperaturaMensagemValidacao() {
    _faixaTemperaturaMensagemValidacao = new FaixaTemperaturaMensagemValidacao();
    KoBindings(_faixaTemperaturaMensagemValidacao, "knockoutMensagensValidacao");
}