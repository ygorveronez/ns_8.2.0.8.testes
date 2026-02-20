/// <reference path="..\..\..\js\Global\Mensagem.js" />
/// <reference path="..\..\..\js\Global\Rest.js" />
/// <reference path="../../Enumeradores/EnumStatusAgendaTarefa.js" />
/// <reference path="Agenda.js" />
/// <reference path="../AgendaTarefa/AgendaTarefa.js" />

var CalendarioAgenda = function () {
    this._idContainerCalendario = "#divListaAgenda";
    this._dataAtual;
}

CalendarioAgenda.prototype = {
    carregar: function () {
        var self = this;
        var dataAgenda = _dadosPesquisaAgenda.DataAgenda ? moment(_dadosPesquisaAgenda.DataAgenda, "DD/MM/YYYY") : moment();

        self._destroy();

        $(self._idContainerCalendario).fullCalendar({
            header: {
                left: 'title',
                center: '',
                right: 'prev,next'
            },
            defaultView: 'agendaDay',
            allDaySlot: false,
            allDayDefault: false,
            forceEventDuration: true,
            lang: 'pt-br',
            height: 700,
            scrollTime: self._obterScrollTime(),
            defaultDate: dataAgenda,
            slotDuration: '00:05:00',
            slotLabelFormat: 'HH:mm',
            slotEventOverlap: false,
            snapDuration: "00:01:00",
            timeFormat: 'HH:mm',
            displayEventTime: true,
            displayEventEnd: true,
            nowIndicator: true,
            droppable: false,
            events: function (start, end, timezone, callback) {
                self._dataAtual = start.format("DD/MM/YYYY");
                self._carregarTarefas(callback);
            },
            eventRender: function (event, element) {
                // Modifica a renderização
                if (event.tarefa != null) {
                    // Seta o conteúdo como HTML
                    element.find('.fc-content').html(event._html);
                }
            },
            eventClick: self._exibirDetalhes
        });
    },
    _carregarTarefas: function (callback) {
        var self = this;

        _dadosPesquisaAgenda.DataAgenda = self._dataAtual;

        executarReST("Agenda/ObterInformacoesTarefas", _dadosPesquisaAgenda, function (retorno) {
            if (retorno.Success) {
                var tarefas = new Array();

                retorno.Data.forEach(function (tarefa) {
                    tarefas.push(self._obterDadosTarefa(tarefa));
                });

                callback(tarefas);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    },
    _destroy: function () {
        $(this._idContainerCalendario).fullCalendar('destroy');
        $(this._idContainerCalendario).html("");
    },
    _exibirDetalhes: function (event) {
        editarAgendaTarefa(null, event.id);
        Global.abrirModal('divModalAgendaTarefa');
    },
    _obterDadosTarefa: function (tarefa) {
        return {
            allDay: false,
            tarefa: tarefa,
            className: "well-blue",
            constraint: "horarioDisponivel",
            duration: moment.utc().hours((tarefa.Tempo / 60 | 0)).minutes((tarefa.Tempo % 60 | 0)).format("HH:mm"),
            durationEditable: false,
            end: moment(tarefa.DataFinal, "DD/MM/YYYY HH:mm"),
            id: tarefa.Codigo,
            start: moment(tarefa.DataInicial, "DD/MM/YYYY HH:mm"),
            startEditable: false,
            title: tarefa.Observacao,
            _html: this._obterHtmlDadosTarefa(tarefa)
        };
    },
    _obterHorarioFocarCalendario: function () {
        if (_dadosPesquisaAgenda.DataAgenda == Global.DataAtual())
            return moment().format("HH:mm:00");

        /* Hora padrão */
        return "08:00:00";
    },
    _obterHtmlDadosTarefa: function (tarefa) {
        var html = '';

        html += '<div class="row tarefa-codigo-' + tarefa.Codigo + '">';
        html += this._obterHtmlFaixaInformativa(tarefa);
        html += '    <div class="col col-12 txt-ellipsis"><span class="txt-bold">Período</span>: ' + tarefa.DataInicial + ' até ' + tarefa.DataFinal + '</div>';
        html += '    <div class="col col-12 txt-ellipsis"><span class="txt-bold">Tarefa</span>: ' + tarefa.Observacao + '</div>';
        html += '    <div class="col col-12 txt-ellipsis"><span class="txt-bold">Colaborador</span>: ' + tarefa.Colaborador + '</div>';
        html += '    <div class="col col-12 txt-ellipsis"><span class="txt-bold">Cliente</span>: ' + tarefa.Cliente + '</div>';
        html += '    <div class="col col-12 txt-ellipsis"><span class="txt-bold">Prioridade</span>: ' + tarefa.DescricaoPrioridade + '</div>';
        html += '    <div class="col col-12 txt-ellipsis"><span class="txt-bold">Assinatura</span>: ' + tarefa.DescricaoTipoAssinatura + '</div>';
        html += '    <div class="col col-12 txt-ellipsis"><span class="txt-bold">Serviço</span>: ' + tarefa.Servico + '</div>';
        html += '</div>';

        return html;
    },
    _obterHtmlFaixa: function (corFaixa, informacao) {
        return '<div class="ribbon-tms ribbon-tms-' + corFaixa + '" style="margin-top: 5px; margin-right: 5px;"><span>' + informacao + '</span></div>';
    },
    _obterHtmlFaixaInformativa: function (tarefa) {
        if (tarefa.Status == EnumStatusAgendaTarefa.Cancelado)
            return this._obterHtmlFaixa("red", tarefa.DescricaoStatus);
        if (tarefa.Status == EnumStatusAgendaTarefa.Aberto)
            return this._obterHtmlFaixa("yellow", tarefa.DescricaoStatus);
        if (tarefa.Status == EnumStatusAgendaTarefa.EmAndamento)
            return this._obterHtmlFaixa("green", tarefa.DescricaoStatus);
        if (tarefa.Status == EnumStatusAgendaTarefa.Finalizado)
            return this._obterHtmlFaixa("gray", tarefa.DescricaoStatus);

        return "";
    },
    _obterScrollTime: function () {
        var horarioFocarCalendario = this._obterHorarioFocarCalendario();
        var horario = moment(horarioFocarCalendario, "HH:mm:ss");
        var isHorarioMenorDezMinutos = (horario.format('HH') == "00") && (parseInt(horario.format('mm')) < 10);

        if (isHorarioMenorDezMinutos)
            return horarioFocarCalendario;

        /* Diminui 10 minutos do horário para dar espaço visualmente */
        return horario.add(-10, 'minute').format('HH:mm:ss');
    }
}