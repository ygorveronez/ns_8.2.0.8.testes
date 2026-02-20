/// <reference path="Configuracao.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _configuracaoLogs;

var ConfiguracaoLogs = function () {
    this.UtilizaLogArquivo = PropertyEntity({ text: "Utilizar Log Arquivo", getType: typesKnockout.bool, val: ko.observable(false) });
    this.UtilizaLogWeb = PropertyEntity({ text: "Utilizar Log Web", getType: typesKnockout.bool, val: ko.observable(false) });
    this.Url = PropertyEntity({ text: "Url Log Web:", val: ko.observable(""), visible: ko.observable(false) });
    this.ProtocoloLogWeb = PropertyEntity({ text: "Protocolo Log Web: ", val: ko.observable(EnumProtocoloLogWeb.UDP), visible: ko.observable(false), options: EnumProtocoloLogWeb.obterOpcoes(), def: EnumProtocoloLogWeb.UDP });
    this.Porta = PropertyEntity({ text: "Porta Log Web:", getType: typesKnockout.int, val: ko.observable(0), visible: ko.observable(false) });
    this.GravarLogError = PropertyEntity({ text: "Gravar Log Error", getType: typesKnockout.bool, val: ko.observable(false) });
    this.GravarLogInfo = PropertyEntity({ text: "Gravar Log Info", getType: typesKnockout.bool, val: ko.observable(false) });
    this.GravarLogAdvertencia = PropertyEntity({ text: "Gravar Log Advertência", getType: typesKnockout.bool, val: ko.observable(false) });
    this.GravarLogDebug = PropertyEntity({ text: "Gravar Log Debug", getType: typesKnockout.bool, val: ko.observable(false) });
    this.SalvarConfiguracaoLog = PropertyEntity({ type: types.event, eventClick: SalvarConfiguracaoLogClick, text: ko.observable("Salvar"), visible: ko.observable(true) });

    this.UtilizaGraylog = PropertyEntity({ text: "Utilizar Graylog", getType: typesKnockout.bool, val: ko.observable(false) });
    this.ProtocoloLogGraylog = PropertyEntity({ text: "Protocolo Log Graylog: ", val: ko.observable(EnumProtocoloLogWeb.TCP), visible: ko.observable(false), options: EnumProtocoloLogWeb.obterOpcoes(), def: EnumProtocoloLogWeb.TCP });
    this.UrlGraylog = PropertyEntity({ text: "Url Graylog:", val: ko.observable(""), visible: ko.observable(false) });
    this.PortaGraylog = PropertyEntity({ text: "Porta Graylog:", getType: typesKnockout.int, val: ko.observable(0), visible: ko.observable(false) });


    this.UtilizaLogWeb.val.subscribe((valor) => {
        if (valor === true) {
            _configuracaoLogs.ProtocoloLogWeb.visible(true);
            _configuracaoLogs.Url.visible(true);
            _configuracaoLogs.Porta.visible(true);
        }
        else {
            _configuracaoLogs.ProtocoloLogWeb.visible(false);
            _configuracaoLogs.Url.visible(false);
            _configuracaoLogs.Porta.visible(false);
        }
    });

    this.UtilizaGraylog.val.subscribe((valor) => {
        if (valor === true) {
            _configuracaoLogs.ProtocoloLogGraylog.visible(true);
            _configuracaoLogs.UrlGraylog.visible(true);
            _configuracaoLogs.PortaGraylog.visible(true);
        }
        else {
            _configuracaoLogs.ProtocoloLogGraylog.visible(false);
            _configuracaoLogs.UrlGraylog.visible(false);
            _configuracaoLogs.PortaGraylog.visible(false);
        }
    });
};

//*******EVENTOS*******

function LoadConfiguracaoLogs() {
    _configuracaoLogs = new ConfiguracaoLogs();
    KoBindings(_configuracaoLogs, "knoutConfiguracaoLogs");

    // Inicia grid de dados
}

//*******MÉTODOS*******

function AbrirModalConfiguracaoLogs() {
    executarReST("Configuracao/BuscarConfiguracaoLogs", null, function (arg) {
        if (arg.Data != null) {
            _configuracaoLogs.UtilizaLogArquivo.val(arg.Data.UtilizaLogArquivo);
            _configuracaoLogs.UtilizaLogWeb.val(arg.Data.UtilizaLogWeb);
            _configuracaoLogs.ProtocoloLogWeb.val(arg.Data.ProtocoloLogWeb);
            _configuracaoLogs.GravarLogError.val(arg.Data.GravarLogError);
            _configuracaoLogs.GravarLogInfo.val(arg.Data.GravarLogInfo);
            _configuracaoLogs.GravarLogAdvertencia.val(arg.Data.GravarLogAdvertencia);
            _configuracaoLogs.GravarLogDebug.val(arg.Data.GravarLogDebug);
            _configuracaoLogs.Url.val(arg.Data.Url);
            _configuracaoLogs.Porta.val(arg.Data.Porta);

            _configuracaoLogs.UtilizaGraylog.val(arg.Data.UtilizaGraylog);
            _configuracaoLogs.ProtocoloLogGraylog.val(arg.Data.ProtocoloLogGraylog);
            _configuracaoLogs.UrlGraylog.val(arg.Data.UrlGraylog);
            _configuracaoLogs.PortaGraylog.val(arg.Data.PortaGraylog);

            Global.abrirModal('divConfiguracoesLog');
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function SalvarConfiguracaoLogClick(e, sender) {
    Salvar(_configuracaoLogs, "Configuracao/SalvarConfiguracaoLogs", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}