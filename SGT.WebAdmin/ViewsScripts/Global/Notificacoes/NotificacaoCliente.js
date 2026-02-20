var NotificacaoPortalCliente = function () {
    var self = this;
    var _notificacoes;
    var $notificationIcon;
    var $notification;
    var $notificationBackdrop;

    /**
     * Definições Knockout
     */
    var ListaNotificacao = function () {
        this.Notificacoes = PropertyEntity({ val: ko.observable([]), eventClick: notificacaoClick });
        this.Total = PropertyEntity({ val: ko.observable(0), def: 0, eventChange: notificacoesScroll, visible: ko.observable(false) });
        this.Inicio = PropertyEntity({ val: ko.observable(0), def: 0, type: types.local });
        this.RequisicaoPendente = PropertyEntity({ val: ko.observable(false), def: false, type: types.local });
        this.NaoVistas = PropertyEntity({ val: ko.observable(0), def: 0, eventClick: naoVistasClick });
    };

    var Notificacao = function (dados) {
        this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
        this.CodigoObjetoNotificacao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
        this.Data = PropertyEntity();
        this.Hora = PropertyEntity();
        this.Nota = PropertyEntity();
        this.SituacaoNotificacao = PropertyEntity({ val: ko.observable(EnumSituacaoNotificacao.Nova), def: EnumSituacaoNotificacao.Nova, getType: typesKnockout.int });
        this.TipoNotificacao = PropertyEntity({ val: ko.observable(EnumTipoNotificacao.credito), def: EnumTipoNotificacao.credito, getType: typesKnockout.int });
        this.URLPagina = PropertyEntity({ eventClick: notificacaoClick });
        this.Icone = PropertyEntity();
        this.StatusVisual = PropertyEntity();

        PreencherObjetoKnout(this, { Data: dados });
    };

    /**
     * Eventos Knockout
     */
    var naoVistasClick = function () {
        _RequisicaoIniciada = true;
        if (_notificacoes.NaoVistas.val() <= 0)
            return;

        executarReST("Notificacao/SetarParaVisualizadas", null, function (arg) {
            var naoVistas = 0;

            if (arg.Success)
                naoVistas = _notificacoes.NaoVistas.def;
            else
                naoVistas = _notificacoes.NaoVistas.val();

            _notificacoes.NaoVistas.val(naoVistas);
            validarNaoVistas();
            _RequisicaoIniciada = false;
        });
    }

    var notificacoesScroll = function (e, sender) {
        var elem = sender.target;
        if (_notificacoes.Inicio.val() < _notificacoes.Total.val() && elem.scrollTop > (elem.scrollHeight - elem.offsetHeight - 200)) {
            carregarNotificacoes();
        }
    }

    function notificacaoClick(e, notificacao) {
        if (e && e.preventDefault) e.preventDefault();

        _RequisicaoIniciada = true;
        marcarNotificacaoComoLida(notificacao);

        if (notificacao.TipoNotificacao.val() == EnumTipoNotificacao.relatorio)
            baixarRelatorio(notificacao);
        else if (notificacao.TipoNotificacao.val() == EnumTipoNotificacao.arquivo)
            baixarArquivo(notificacao);
        else if (notificacao.CodigoObjetoNotificacao.val() && notificacao.URLPagina.val() != "") {
            self.SetCodigoGlobal(notificacao.CodigoObjetoNotificacao.val());

            VerificarENavegarParaURLNotificacao(notificacao.URLPagina.val());

            ocultarAbaNotificacoes();
        }
    }

    /**
     * Métodos Público
     */
    this.Load = function (id) {
        _notificacoes = new ListaNotificacao();
        KoBindings(_notificacoes, id);

        $notificationIcon = $(".open-notification");
        $notification = $("aside.notifications");
        $notificationBackdrop = $(".notifications-backdrop");

        carregarNotificacoes();

        SignalRNotificarUsuarioEvent = retornoNotificarSignalREvent;

        $notificationIcon.click(function (e) {
            if (e && e.preventDefault) e.preventDefault();
            abrirAbaNotificacoes();
        });

        $notification.on('click', '.close', function (e) {
            if (e && e.preventDefault) e.preventDefault();
            ocultarAbaNotificacoes();
        });
    }

    var _codigoGlobal = 0;
    this.SetCodigoGlobal = function (codigo) {
        _codigoGlobal = codigo;
    }

    this.GetCodigoGlobal = function () {
        return _codigoGlobal;
    }

    this.ClearCodigoGlobal = function () {
        _codigoGlobal = 0;
    }

    /**
     * Métodos Privados
     */
    function baixarArquivo(notificacao) {
        var data = { Codigo: notificacao.CodigoObjetoNotificacao.val() };
        executarDownload("ControleGeracaoArquivo/DownloadArquivo", data);
    }

    function baixarRelatorio(notificacao) {
        var data = { Codigo: notificacao.CodigoObjetoNotificacao.val() };
        executarDownload("Relatorios/Relatorio/DownloadRelatorio", data);
    }

    function marcarNotificacaoComoLida(notificacao) {
        if (notificacao.SituacaoNotificacao.val() != EnumSituacaoNotificacao.Nova)
            return;

        var data = { Codigo: notificacao.Codigo.val() };
        executarReST("Notificacao/MarcarNotificacaoComoLida", data, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    notificacao.SituacaoNotificacao.val(EnumSituacaoNotificacao.Lida);
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
                }
            }
            _RequisicaoIniciada = false;
        });
    }

    var abrirAbaNotificacoes = function () {
        $notification.show();
        $notificationBackdrop.show();
        naoVistasClick();
    }

    var ocultarAbaNotificacoes = function () {
        $notification.hide();
        $notificationBackdrop.hide();
    }

    var carregarNotificacoes = function () {
        if (_notificacoes.RequisicaoPendente.val())
            return;

        _notificacoes.RequisicaoPendente.val(true);
        var quantidadePorVez = 10;
        var data = { inicio: _notificacoes.Inicio.val(), limite: quantidadePorVez };

        _RequisicaoIniciada = true;
        executarReST("Notificacao/BuscarNovasNotificacoesPortalCliente", data, function (arg) {
            _notificacoes.RequisicaoPendente.val(false);
            _RequisicaoIniciada = false;

            if (!arg.Success)
                return;

            var retorno = arg.Data;

            _notificacoes.Total.val(retorno.Total);
            _notificacoes.Inicio.val(_notificacoes.Inicio.val() + quantidadePorVez);
            _notificacoes.NaoVistas.val(retorno.NaoVistas);

            var objetosNotificacao = retorno.Notificacoes.map(function (notificacao) {
                return new Notificacao(notificacao);
            });
            var notificacoesCarregadas = _notificacoes.Notificacoes.val().slice();

            _notificacoes.Notificacoes.val(notificacoesCarregadas.concat(objetosNotificacao));

            validarNaoVistas();
        });
    }

    var validarNaoVistas = function () {
        if (_notificacoes.NaoVistas.val() > 0) {
            $notificationIcon.addClass("has-notification");
        } else {
            $notificationIcon.removeClass("has-notification");
        }

        if (_notificacoes.Total.val() > 0) {
            _notificacoes.Total.visible(true);
        }
    }


    function retornoNotificarSignalREvent(retorno) {
        var notificacao = JSON.parse(retorno);
        _notificacoes.NaoVistas.val(_notificacoes.NaoVistas.val() + 1);

        ExibirInformacoesNotificacaoForaPagina(notificacao);
    }


    function ExibirInformacoesNotificacaoForaPagina(notificacao) {

        _notificacoes.Total.val(_notificacoes.Total.val() + 1);
        _notificacoes.Inicio.val(_notificacoes.Inicio.val() + 1);

        var notificacoesCarregadas = _notificacoes.Notificacoes.val().slice();
        var objetosNotificacao = new Notificacao(notificacao);

        _notificacoes.Notificacoes.val(notificacoesCarregadas.concat(objetosNotificacao));

        validarNaoVistas();

        exibirAlertaNotificacao(notificacao.Nota, notificacao.DataNotificacao);
    }

    function exibirAlertaNotificacao(titulo, data, timeout) {
        if (titulo.length > 50) {
            titulo = titulo.substring(0, 50) + "...";
        }
        $.sound_on = false;
        $.smallBox({
            title: titulo,
            content: "<i class='fa fa-bell-o'></i> <i>" + data + "</i>",
            color: "#3B8686",
            iconSmall: "fa fa-bell-o bounce animated",
            timeout: timeout != null ? timeout : 4000,
        });
    }

}