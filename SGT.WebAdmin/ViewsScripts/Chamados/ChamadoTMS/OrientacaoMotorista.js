/// <reference path="AutorizacaoCliente.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _orientacaoMotorista;

var OrientacaoMotorista = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.DataUltimoEnvioMensagemOrientacaoMotorista = PropertyEntity({ text: "Data Último Envio:", getType: typesKnockout.dateTime, required: ko.observable(false), enable: ko.observable(false), visible: ko.observable(true) });
    this.MensagemOrientacaoMotorista = PropertyEntity({ val: ko.observable(""), def: "", text: "*Mensagem:", required: ko.observable(true), enable: ko.observable(true) });

    this.EnviarMensagem = PropertyEntity({ eventClick: EnviarMensagemOrientacaoMotoristaClick, type: types.event, text: "Enviar pelo WhatsApp", visible: ko.observable(true), enable: ko.observable(true) });
};

//*******EVENTOS*******

function loadOrientacaoMotorista() {
    _orientacaoMotorista = new OrientacaoMotorista();
    KoBindings(_orientacaoMotorista, "tabOrientacaoMotorista");
}

function EnviarMensagemOrientacaoMotoristaClick(e, sender) {
    _orientacaoMotorista.DataUltimoEnvioMensagemOrientacaoMotorista.val(Global.DataHoraAtual());

    Salvar(_orientacaoMotorista, "ChamadoTMS/RetornaMensagemOrientacaoMotorista", function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Orientação de Motorista processada com sucesso");
                chamarWhatsappComMensagem(arg.Data);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function chamarWhatsappComMensagem(mensagemFormatada) {
    //Dicas da opção:
    //precisa colocar o 55 na frente para o envio, assim já preenche a mensagem
    //caso não tenha o número, pede para selecionar alguma contato
    //ao selecionar mais de um contato, já envia a mensagem, caso for só um, preenche na tela para o usuário enviar

    if (string.IsNullOrWhiteSpace(string.OnlyNumbers(_abertura.CelularMotorista.val())))
        window.open('https://web.whatsapp.com/send?phone=&text=' + mensagemFormatada);
    else
        window.open('https://web.whatsapp.com/send?phone=55' + string.OnlyNumbers(_abertura.CelularMotorista.val()) + '&text=' + mensagemFormatada);
}

function ControleCamposOrientacaoMotorista(status) {
    SetarEnableCamposKnockout(_orientacaoMotorista, status);

    _orientacaoMotorista.DataUltimoEnvioMensagemOrientacaoMotorista.enable(false);
}

function limparCamposOrientacaoMotorista() {
    LimparCampos(_orientacaoMotorista);
}