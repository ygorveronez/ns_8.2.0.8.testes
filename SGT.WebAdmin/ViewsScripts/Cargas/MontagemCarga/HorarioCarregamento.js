/// <autosync enabled="true" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridHorariosInfoCarregamento;

var InfoCarregamento = function () {
    this.Filial = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.TipoOperacao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.EncaixarHorario = PropertyEntity({ val: ko.observable(false), def: false, text: Localization.Resources.Cargas.MontagemCarga.EncaixarHorario.getFieldDescription(), eventClick: encaixarHorarioClick, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.DataCarregamento = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCarga.DataCarregamento.getRequiredFieldDescription(), required: true, getType: typesKnockout.dateTime, visible: ko.observable(false) });
    this.DataCarregamentoDisponibilidade = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCarga.Data.getFieldDescription(), getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), diminuirData: diminuirDataClick, aumentarData: aumentarDataClick, idGrid: guid() });
    this.ConfirmarAlteracao = PropertyEntity({ type: types.event, eventClick: confirmarAlteracaoCarregamentoMontagemCargaClick, text: Localization.Resources.Gerais.Geral.Confirmar, visible: ko.observable(true) });
}


//*******EVENTOS*******

function alterarDataCarregamentoClick() {
    var infoCarregamento = new InfoCarregamento();
    infoCarregamento.TipoOperacao.val(_carregamentoTransporte.TipoOperacao.codEntity());

    var pedidos = PEDIDOS_SELECIONADOS();
    var filial = (pedidos.length > 0) ? pedidos[0].CodigoFilial : 0;
    infoCarregamento.Filial.val(filial);

    if (_CONFIGURACAO_TMS.PermiteSelecionarHorarioEncaixe === true)
        infoCarregamento.EncaixarHorario.visible(true);

    KoBindings(infoCarregamento, "knoutAlterarCarregamento");

    CarregarGridHorarios(infoCarregamento);
    infoCarregamento.DataCarregamentoDisponibilidade.val.subscribe(function () {
        AtualizarHorarios(infoCarregamento);
    });

    Global.abrirModal("divModalAlterarCarregamento");
}

function aumentarDataClick(e, sender) {
    SetarData(e, 1);
}

function diminuirDataClick(e, sender) {
    SetarData(e, -1);
}

function confirmarAlteracaoCarregamentoMontagemCargaClick(e) {
    definirDataCarregamentoPorFilial(e.DataCarregamento.val(), e.EncaixarHorario.val());
    Global.fecharModal("divModalAlterarCarregamento");
}

function encaixarHorarioClick(e) {
    e.EncaixarHorario.val(true);
    e.EncaixarHorario.visible(false);
    e.DataCarregamento.visible(true);
}

//*******MÉTODOS*******

function CarregarGridHorarios(e) {
    if (_gridHorariosInfoCarregamento) _gridHorariosInfoCarregamento.Destroy();

    var multiplaescolha = {
        basicGrid: null,
        callbackSelecionado: function (obj, data) {
            HorarioSelecionado(e, data);
        },
        callbackNaoSelecionado: function () {
        },
        eventos: function () {
        },
        selecionados: new Array(),
        naoSelecionados: new Array(),
        somenteLeitura: false
    };

    _gridHorariosInfoCarregamento = new GridView(e.DataCarregamentoDisponibilidade.idGrid, "JanelaCarregamento/ObterHorariosDisponiveis", e, null, null, 10, null, null, null, multiplaescolha);
    _gridHorariosInfoCarregamento.CarregarGrid();
}

function HorarioSelecionado(e, data) {
    _gridHorariosInfoCarregamento.AtualizarRegistrosSelecionados([data]);
    _gridHorariosInfoCarregamento.DrawTable();

    var dataHoraComSegundos = e.DataCarregamentoDisponibilidade.val() + ' ' + data.HoraInicio;
    var dataHoraSemSegundos = dataHoraComSegundos.substring(0, 16);
    e.DataCarregamento.val(dataHoraSemSegundos);
}

function LimparHorarioSelecionado(e) {
    _gridHorariosInfoCarregamento.AtualizarRegistrosSelecionados([]);
    _gridHorariosInfoCarregamento.DrawTable();

    e.DataCarregamento.val('');
}

function SetarData(e, dias) {
    if (!e.DataCarregamentoDisponibilidade.val()) return;

    var objData = moment(e.DataCarregamentoDisponibilidade.val(), 'DD/MM/YYYY');
    objData.add(dias, 'day');
    e.DataCarregamentoDisponibilidade.val(objData.format('DD/MM/YYYY'));
    LimparHorarioSelecionado(e);
}

function AtualizarHorarios(e) {
    if (!e.DataCarregamentoDisponibilidade.val()) return;
    _gridHorariosInfoCarregamento.CarregarGrid();
}