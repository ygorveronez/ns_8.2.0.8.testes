// #region Objetos Globais do Arquivo

var _informacaoAgendamento;
var _gridHorariosInformacaoAgendamento;

// #endregion Objetos Globais do Arquivo

// #region Knout

var InformacaoAgendamento = function () {
    this.DataAgendamentoDisponibilidade = PropertyEntity({ text: "Disponibilidade", getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), diminuirData: diminuirDataAgendamentoClick, aumentarData: aumentarDataAgendamentoClick, idGrid: guid() });
    this.DataEntrega = PropertyEntity({ text: ko.observable(""), required: true, getType: typesKnockout.dateTime, visible: ko.observable(false) });
    this.PeriodoAgendamento = PropertyEntity({ text: ko.observable(""), required: true, getType: typesKnockout.string, visible: ko.observable(false) });
    this.Destinatario = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Remetente = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.ModeloVeicular = PropertyEntity({ getType: typesKnockout.int, val: ko.observable("") });
    this.TipoCarga = PropertyEntity({ getType: typesKnockout.int, val: ko.observable("") });

    this.ConfirmarAlteracao = PropertyEntity({ type: types.event, eventClick: confirmarHorarioAgendamentoClick, text: "Confirmar", visible: ko.observable(true) });

    this.DataAgendamentoDisponibilidade.val.subscribe(function () {
        if (!_informacaoAgendamento.DataAgendamentoDisponibilidade.val())
            return;

        _gridHorariosInformacaoAgendamento.CarregarGrid();
    })
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

function carregarDataAngendamentoPallet(e) {
    carregarInformacaoAgendamento();
    carregarGridAgendamento();

    _etapaAgendamentoPallet.Destinatario.val.subscribe(function () {
        _informacaoAgendamento.Destinatario.val(_etapaAgendamentoPallet.Destinatario.codEntity());
    });

    _etapaAgendamentoPallet.ModeloVeicular.val.subscribe(function () {
        _informacaoAgendamento.ModeloVeicular.val(_etapaAgendamentoPallet.ModeloVeicular.codEntity());
    });

    _etapaAgendamentoPallet.TipoCarga.val.subscribe(function () {
        _informacaoAgendamento.TipoCarga.val(_etapaAgendamentoPallet.TipoCarga.codEntity());
    });

    _etapaAgendamentoPallet.Remetente.val.subscribe(function () {
        _informacaoAgendamento.Remetente.val(_etapaAgendamentoPallet.Remetente.codEntity());
    });
}

function exibirDatasAgendamentoClick(e) {
    if (!ValidarCamposObrigatoriosExcept(_etapaAgendamentoPallet, _etapaAgendamentoPallet.DataEntrega)) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Preencha os campos obrigatórios.");
        return;
    }

    if (string.IsNullOrWhiteSpace(_informacaoAgendamento.DataAgendamentoDisponibilidade.val()))
        _informacaoAgendamento.DataAgendamentoDisponibilidade.val(Global.DataAtual());

    Global.abrirModal("divModalHorarioAgendamentoJanelaDescarregamento");
    _gridHorariosInformacaoAgendamento.CarregarGrid();
}

// #endregion Funções Públicas

// #region Funções Privadas

function carregarInformacaoAgendamento() {
    _informacaoAgendamento = new InformacaoAgendamento();
    KoBindings(_informacaoAgendamento, "knoutHorarioAgendamentoJanelaDescarregamento");

    if (string.IsNullOrWhiteSpace(_informacaoAgendamento.DataAgendamentoDisponibilidade.val()))
        _informacaoAgendamento.DataAgendamentoDisponibilidade.val(Global.DataAtual());
}

function carregarGridAgendamento() {
    var multiplaescolha = {
        basicGrid: null,
        callbackSelecionado: function (obj, data) {
            horarioAgendamentoSelecionado(data);
        },
        callbackNaoSelecionado: function () {
            _informacaoAgendamento.PeriodoAgendamento.val(0);
            _informacaoAgendamento.DataEntrega.val("");
        },
        eventos: function () { },
        selecionados: new Array(),
        naoSelecionados: new Array(),
        somenteLeitura: false
    };

    _gridHorariosInformacaoAgendamento = new GridView(_informacaoAgendamento.DataAgendamentoDisponibilidade.idGrid, "JanelaDescarga/ObterHorariosDisponiveis", _informacaoAgendamento, null, null, 10, null, null, null, multiplaescolha, 2000);
}

function confirmarHorarioAgendamentoClick(e) {
    if (_informacaoAgendamento.PeriodoAgendamento.val() <= 0) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Informe o  horário de agendamento.");
        return;
    }

    _etapaAgendamentoPallet.PeriodoAgendamento.val(_informacaoAgendamento.PeriodoAgendamento.val());
    _etapaAgendamentoPallet.DataEntrega.val(_informacaoAgendamento.DataEntrega.val());

    Global.fecharModal("divModalHorarioAgendamentoJanelaDescarregamento");
}

function definirDataAgendamentoDisponibilidade(dias) {
    if (!_informacaoAgendamento.DataAgendamentoDisponibilidade.val())
        return;

    var objData = moment(_informacaoAgendamento.DataAgendamentoDisponibilidade.val(), 'DD/MM/YYYY');

    objData.add(dias, 'day');

    _informacaoAgendamento.DataAgendamentoDisponibilidade.val(objData.format('DD/MM/YYYY'));

    limparHorarioSelecionado();
}

function horarioAgendamentoSelecionado(data) {
    _gridHorariosInformacaoAgendamento.AtualizarRegistrosSelecionados([data]);
    _gridHorariosInformacaoAgendamento.DrawTable(true);

    _informacaoAgendamento.PeriodoAgendamento.val(data.Codigo);
    _informacaoAgendamento.DataEntrega.val(data.Data);
}

function limparHorarioSelecionado() {
    _gridHorariosInformacaoAgendamento.AtualizarRegistrosSelecionados([]);
    _gridHorariosInformacaoAgendamento.DrawTable();
}

function ValidarCamposObrigatoriosExcept(kout, propExcept) {
    var tudoCerto = true;

    $.each(kout, function (i, prop) {
        if (propExcept != prop && prop.type == types.map && (prop.getType == null || prop.getType != typesKnockout.dynamic)) {
            if (!ValidarCampoObrigatorioMap(prop)) {
                tudoCerto = false;
            }
        } else if (propExcept != prop && prop.type == types.entity || prop.type == types.listEntity || prop.type == types.multiplesEntities) {
            if (!ValidarCampoObrigatorioEntity(prop)) {
                tudoCerto = false;
            }
        }
    });

    return tudoCerto;
}

// #endregion Funções Privadas
