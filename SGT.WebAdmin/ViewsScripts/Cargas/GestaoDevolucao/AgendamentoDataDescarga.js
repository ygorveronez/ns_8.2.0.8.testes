// #region Objetos Globais do Arquivo

var _informacaoAgendamentoDataDescarga;
var _gridHorariosInformacaoAgendamentoDataDescarga;

// #endregion Objetos Globais do Arquivo

// #region Knout

var InformacaoAgendamentoDataDescarga = function () {
    this.DataAgendamentoDisponibilidade = PropertyEntity({ text: "Disponibilidade", getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), diminuirData: diminuirDataAgendamentoClick, aumentarData: aumentarDataAgendamentoClick, idGrid: guid() });
    this.DataEntrega = PropertyEntity({ text: ko.observable(""), required: true, getType: typesKnockout.dateTime, visible: ko.observable(false) });
    this.DescricaoDataEntrega = PropertyEntity({ text: ko.observable(""), required: true, getType: typesKnockout.dateTime, visible: ko.observable(false) });
    this.PeriodoAgendamento = PropertyEntity({ text: ko.observable(""), required: true, getType: typesKnockout.string, visible: ko.observable(false) });

    this.Destinatario = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Remetente = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.TipoCarga = PropertyEntity({ getType: typesKnockout.int, val: ko.observable("") });

    this.KnockoutCallbackData = PropertyEntity({ getType: typesKnockout.int, val: ko.observable("") });
    this.KnockoutCallbackPeriodo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable("") });

    this.ConfirmarAlteracao = PropertyEntity({ type: types.event, eventClick: confirmarHorarioAgendamentoDataDescargaClick, text: "Confirmar", visible: ko.observable(true) });

    this.DataAgendamentoDisponibilidade.val.subscribe(function () {
        if (!_informacaoAgendamentoDataDescarga.DataAgendamentoDisponibilidade.val())
            return;

        _gridHorariosInformacaoAgendamentoDataDescarga.CarregarGrid();
    })

    this.SituacaoDevolucao = PropertyEntity({ val: ko.observable(null), visible: ko.observable(false) });

}

// #endregion Knout

// #region Funções Associadas a Eventos

function aumentarDataAgendamentoClick() {
    definirDataAgendamentoDisponibilidade(1);
}

function diminuirDataAgendamentoClick() {
    definirDataAgendamentoDisponibilidade(-1);
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function loadModalAgendamentoDataDescarga(knoutData, knoutPeriodo, knoutTipoDeCarga, knoutRemetente, knoutDestinatario) {
    carregarInformacaoAgendamentoDataDescarga(knoutData, knoutPeriodo, knoutTipoDeCarga, knoutRemetente, knoutDestinatario);
    carregarGridAgendamentoDataDescarga();
}

function exibirDatasAgendamentoClick(e) {
    if (string.IsNullOrWhiteSpace(_informacaoAgendamentoDataDescarga.DataAgendamentoDisponibilidade.val()))
        _informacaoAgendamentoDataDescarga.DataAgendamentoDisponibilidade.val(Global.DataAtual());

    Global.abrirModal("divModalHorarioAgendamentoJanelaDescarregamento");
    _gridHorariosInformacaoAgendamentoDataDescarga.CarregarGrid();
}

// #endregion Funções Públicas

// #region Funções Privadas

function carregarInformacaoAgendamentoDataDescarga(knoutData, knoutPeriodo, knoutTipoDeCarga, knoutRemetente, knoutDestinatario) {
    _informacaoAgendamentoDataDescarga = new InformacaoAgendamentoDataDescarga(knoutData, knoutPeriodo);
    KoBindings(_informacaoAgendamentoDataDescarga, "knoutHorarioAgendamentoJanelaDescarregamento");

    if (string.IsNullOrWhiteSpace(_informacaoAgendamentoDataDescarga.DataAgendamentoDisponibilidade.val()))
        _informacaoAgendamentoDataDescarga.DataAgendamentoDisponibilidade.val(Global.DataAtual());

    if (knoutData)
        _informacaoAgendamentoDataDescarga.KnockoutCallbackData = knoutData;
    if (knoutPeriodo)
        _informacaoAgendamentoDataDescarga.KnockoutCallbackPeriodo = knoutPeriodo;
    if (knoutTipoDeCarga)
        _informacaoAgendamentoDataDescarga.TipoCarga = knoutTipoDeCarga;
    if (knoutRemetente)
        _informacaoAgendamentoDataDescarga.Remetente = knoutRemetente;
    if (knoutDestinatario)
        _informacaoAgendamentoDataDescarga.Destinatario = knoutDestinatario;
}

function carregarGridAgendamentoDataDescarga() {
    var multiplaescolha = {
        basicGrid: null,
        callbackSelecionado: function (obj, data) {
            horarioAgendamentoDataDescargaSelecionado(data);
        },
        callbackNaoSelecionado: function () {
            _informacaoAgendamentoDataDescarga.PeriodoAgendamento.val(0);
            _informacaoAgendamentoDataDescarga.DataEntrega.val("");
        },
        eventos: function () { },
        selecionados: new Array(),
        naoSelecionados: new Array(),
        somenteLeitura: false
    };

    _gridHorariosInformacaoAgendamentoDataDescarga = new GridView(_informacaoAgendamentoDataDescarga.DataAgendamentoDisponibilidade.idGrid, "JanelaDescarga/ObterHorariosDisponiveis", _informacaoAgendamentoDataDescarga, null, null, 10, null, null, null, multiplaescolha, 2000);
}

function confirmarHorarioAgendamentoDataDescargaClick(e) {
    if (_informacaoAgendamentoDataDescarga.PeriodoAgendamento.val() <= 0) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Informe o horário de agendamento.");
        return;
    }

    _informacaoAgendamentoDataDescarga.KnockoutCallbackPeriodo.codEntity(_informacaoAgendamentoDataDescarga.PeriodoAgendamento.val());
    _informacaoAgendamentoDataDescarga.KnockoutCallbackData.val(_informacaoAgendamentoDataDescarga.DataEntrega.val());
    _informacaoAgendamentoDataDescarga.KnockoutCallbackData.descricao(_informacaoAgendamentoDataDescarga.DescricaoDataEntrega.val());

    Global.fecharModal("divModalHorarioAgendamentoJanelaDescarregamento");
}

function definirDataAgendamentoDisponibilidade(dias) {
    if (!_informacaoAgendamentoDataDescarga.DataAgendamentoDisponibilidade.val())
        return;

    var objData = moment(_informacaoAgendamentoDataDescarga.DataAgendamentoDisponibilidade.val(), 'DD/MM/YYYY');

    objData.add(dias, 'day');

    _informacaoAgendamentoDataDescarga.DataAgendamentoDisponibilidade.val(objData.format('DD/MM/YYYY'));

    limparHorarioSelecionadoDescargaGestaoDevolucao();
}

function horarioAgendamentoDataDescargaSelecionado(data) {
    _gridHorariosInformacaoAgendamentoDataDescarga.AtualizarRegistrosSelecionados([data]);
    _gridHorariosInformacaoAgendamentoDataDescarga.DrawTable(true);

    _informacaoAgendamentoDataDescarga.PeriodoAgendamento.val(data.Codigo);
    _informacaoAgendamentoDataDescarga.DataEntrega.val(data.Data);
    _informacaoAgendamentoDataDescarga.DescricaoDataEntrega.val(data.DescricaoDataDescarga);
}

function limparHorarioSelecionadoDescargaGestaoDevolucao() {
    _gridHorariosInformacaoAgendamentoDataDescarga.AtualizarRegistrosSelecionados([]);
    _gridHorariosInformacaoAgendamentoDataDescarga.DrawTable();
}

// #endregion Funções Privadas
