// #region Objetos Globais do Arquivo
var _enviarNotificacaoApp;
var _gridEnviarNotificacaoApp;
// #endregion Objetos Globais do Arquivo

//#region Classes
var EnviarNotificacaoApp = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Titulo = PropertyEntity({ text: "Título", val: ko.observable(""), def: ko.observable(""), maxlength: 100, enable: ko.observable(true), required: true });
    this.Mensagem = PropertyEntity({ text: "Mensagem", enable: ko.observable(true), maxlength: 1000, val: ko.observable(""), def: ko.observable(""), required: true });
    this.Notificacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Notificação:", idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(false) });

    this.Notificar = PropertyEntity({ eventClick: enviarNotificacaoSuperAppClick, type: types.event, text: Localization.Resources.Gerais.Geral.Notificar, visible: ko.observable(true), enable: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: fecharModalEnviarNotificacaoCarga, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(true) });

    this.TagNumeroRecibo = PropertyEntity({ eventClick: function (e) { InserirTag(this.Mensagem.id, "#NumeroRecibo"); }, type: types.event, text: "Número do Recibo", enable: ko.observable(true), visible: ko.observable(true) });

    this.Titulo.val.subscribe(function (titulo) {
        _enviarNotificacaoApp.Notificar.enable(_enviarNotificacaoApp.Mensagem.val().length > 10 && titulo.length > 0);
    });
    this.Mensagem.val.subscribe(function (mensagem) {
        _enviarNotificacaoApp.Notificar.enable((_enviarNotificacaoApp.Titulo.val().length > 0 && mensagem.length > 10));
    });
};
// #endregion Classes

// #region Funções de Inicialização
function loadEnviarNotificacaoApp(tipoNotificacaoMotoristaSMS) {
    $.get("Content/TorreControle/MonitorNotificacoesApp/ModaisEnviarNotificacaoApp.html?dyn=" + guid(), function (htmlModaisEnviarNotificacaoApp) {
        $("#ModaisEnviarNotificacaoApp").html(htmlModaisEnviarNotificacaoApp);
        _enviarNotificacaoApp = new EnviarNotificacaoApp();
        KoBindings(_enviarNotificacaoApp, "knockoutModalEnviarNotificacaoApp");

        BuscarConfiguracaoNotificacaoMotoristaSMS(_enviarNotificacaoApp.Notificacao, tipoNotificacaoMotoristaSMS, callbackRetornoNotificacaoMotoristaSMS, true);

        if (tipoNotificacaoMotoristaSMS)
            _enviarNotificacaoApp.Notificacao.visible(true);
    });
}
// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos
function exibirModalEnviarNotificacaoApp(listaCargasMotoristas, monitoramento = false) {
    LimparCampos(_enviarNotificacaoApp);
    let header;
    if (monitoramento) {
        header = [
            { data: "Codigo", visible: false },
            { data: "Carga", visible: false },
            { data: "CargaEmbarcador", title: "Carga", width: "10%" },
            { data: "Tracao", title: "Placa" },
            { data: "CPFMotoristas", visible: false },
            { data: "Motoristas", title: "Motoristas" },
            { data: "RazaoSocialTransportador", title: "Transportador" },
            { data: "ClienteOrigem", title: "Origem" },
            { data: "ProximoDestino", title: "Destino" },
            { data: "DataCarregamento", title: "Data do Carregamento" },
            { data: "DataEntregaReprogramadaProximaEntrega", title: "ETA - Previsão Chegada Atualizada" },
            { data: "DescricaoLocalRaiosProximidade", title: "Local Raio Proximidade" },
            { data: "Rastreador", title: "Rastreador" },
        ];
    } else {
        header = [
            { data: "Codigo", visible: false },
            { data: "Carga", visible: false },
            { data: "CPFMotoristas", visible: false },
            { data: "Motorista", title: "Motoristas", width: "25%" },
            { data: "Tracao", title: "Placa" },
            { data: "Transportador", title: "Transportador" },
            { data: "CargaEmbarcador", title: "Carga", width: "10%" },
        ];
    }
    let configRowsSelect = { permiteSelecao: true, marcarTodos: true, permiteSelecionarTodos: true }
    _gridEnviarNotificacaoApp = new BasicDataTable(_enviarNotificacaoApp.Grid.id, header, null, null, configRowsSelect, null, null, null, null, null, null, null, null, null, gridMonitoramentoCallbackColumnDefault);

    _gridEnviarNotificacaoApp.CarregarGrid(listaCargasMotoristas);

    Global.abrirModal('divModalEnviarNotificacaoApp');
}

function fecharModalEnviarNotificacaoCarga() {
    Global.fecharModal('divModalEnviarNotificacaoApp');
}

function enviarNotificacaoSuperAppClick(dados) {
    let objetosNotificacao = [];
    _gridEnviarNotificacaoApp.ListaSelecionados().forEach(function (selecionado) { objetosNotificacao.push(GetObjetoNotificacao(selecionado)); });

    if (objetosNotificacao.length == 0) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, "Selecione pelo menos um motorista para notificar.");
        return;
    }
    else if (!ValidarCamposObrigatorios(_enviarNotificacaoApp)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Gerais.Geral.PreenchaOsCamposObrigatorios);
        return;
    }

    enviarNotificacao(objetosNotificacao, _enviarNotificacaoApp.Titulo.val(), _enviarNotificacaoApp.Mensagem.val());
}

function callbackRetornoNotificacaoMotoristaSMS(dados) {
    _enviarNotificacaoApp.Notificacao.codEntity(dados.Codigo);
    _enviarNotificacaoApp.Notificacao.val(dados.Descricao);

    _enviarNotificacaoApp.Titulo.val(dados.Descricao);
    _enviarNotificacaoApp.Mensagem.val(dados.Mensagem);
}

// #endregion Funções Associadas a Eventos


// #region Funções Publicas
// #endregion Funções Publicas

// #region Funções Privadas
function GetObjetoNotificacao(selecionado) {
    return {
        CPFMotoristas: selecionado.CPFMotoristas,
        CodigoCarga: selecionado.Carga
    };
}
function enviarNotificacao(objetosNotificacao, titulo, mensagem) {
    let dados = {
        objetosNotificacao: JSON.stringify(objetosNotificacao),
        titulo: titulo,
        mensagem: mensagem
    };
    executarReST("MonitorNotificacoesApp/EnviarNotificacaoApp", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                fecharModalEnviarNotificacaoCarga();
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Success, "Notificação registrada.");
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}
// #endregion Funções Privadas
