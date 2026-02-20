var BuscarConfiguracaoNotificacaoMotoristaSMS = function (knout, tipoNotificacao, callbackRetorno, notificacaoSuperApp) {
    var idDiv = guid();
    var GridConsulta;

    var _tipoNotificaoMotorista = EnumTipoNotificacaoMotoristaSMS.Todos;
    if (tipoNotificacao != null)
        _tipoNotificaoMotorista = tipoNotificacao;

    var _notificacaoSuperApp = false;
    if (notificacaoSuperApp)
        _notificacaoSuperApp = notificacaoSuperApp;

    var OpcoesKnout = function () {
        this.Titulo = PropertyEntity({ text: "Buscar Notificação", type: types.local });
        this.TituloGrid = PropertyEntity({ text: "Notificação Motorista", type: types.local });

        this.Descricao = PropertyEntity({ text: "Descrição:", col: 6, maxlength: 250 });
        this.Ativo = PropertyEntity({ val: ko.observable(1), options: Global.ObterOpcoesPesquisaBooleano("Ativo", "Inatvo"), def: "", visible: false });
        this.NotificacaoSuperApp = PropertyEntity({ val: ko.observable(_notificacaoSuperApp), def: false, visible: false });
        this.TipoNotificacao = PropertyEntity({ col: 0, val: ko.observable(_tipoNotificaoMotorista), visible: (false) });

        this.Pesquisar = PropertyEntity({ eventClick: function (e) { GridConsulta.CarregarGrid(); }, type: types.event, text: "Pesquisar", visible: true });
    }

    var knoutOpcoes = new OpcoesKnout();
    var divBusca = new CriarBusca(idDiv, knoutOpcoes, knout, null, null, false, function () {

    });

    var callback = function (e) {
        divBusca.DefCallback(e);
    }

    if (callbackRetorno != null) {
        callback = function (e) {
            divBusca.CloseModal();
            callbackRetorno(e);
        }
    }

    GridConsulta = new GridView(idDiv + "_tabelaEntidades", "NotificacaoMotoristaSMS/Pesquisa", knoutOpcoes, divBusca.OpcaoPadrao(callback), null);
    divBusca.AddEvents(GridConsulta);

    divBusca.AddTabPressEvent(function (outraPressionada) {
        GridConsulta.CarregarGrid(function (lista) {
            if (lista.data.length == 1) {
                callback(lista.data[0]);
            } else {
                divBusca.OpenModal();
            }
        });
    });
}