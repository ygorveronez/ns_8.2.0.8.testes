
/// <autosync enabled="true" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridHorariosInfoCarregamentoMontagem;

var InfoCarregamentoMontagem = function () {
    this.Filial = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.TipoOperacao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.ModeloVeicularCarga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.EncaixarHorario = PropertyEntity({ val: ko.observable(false), def: false, text: Localization.Resources.Cargas.MontagemCargaMapa.EncaixarHorario, eventClick: encaixarHorarioClick, getType: typesKnockout.bool, visible: ko.observable(false) });
    this.DataCarregamento = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.DataCarregamento.getRequiredFieldDescription(), required: true, getType: typesKnockout.dateTime, visible: ko.observable(false) });
    this.DataCarregamentoDisponibilidade = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.Data.getFieldDescription(), getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), diminuirData: diminuirDataClick, aumentarData: aumentarDataClick, idGrid: guid() });
    this.ConfirmarAlteracao = PropertyEntity({ type: types.event, eventClick: confirmarAlteracaoCarregamentoMontagemCargaMapaClick, text: Localization.Resources.Gerais.Geral.Confirmar, visible: ko.observable(true) });
}


//*******EVENTOS*******

function alterarDataCarregamentoMontagemCarga() {
    var infoCarregamentoMontagem = new InfoCarregamentoMontagem();
    infoCarregamentoMontagem.TipoOperacao.val(_carregamentoTransporte.TipoOperacao.codEntity());
    infoCarregamentoMontagem.ModeloVeicularCarga.val(_carregamento.ModeloVeicularCarga.codEntity());

    var pedidos = PEDIDOS_SELECIONADOS();
    var filial = (pedidos.length > 0) ? pedidos[0].CodigoFilial : 0;
    infoCarregamentoMontagem.Filial.val(filial);

    if (_CONFIGURACAO_TMS.PermiteSelecionarHorarioEncaixe === true)
        infoCarregamentoMontagem.EncaixarHorario.visible(true);

    KoBindings(infoCarregamentoMontagem, "knoutAlterarCarregamentoMontagem");

    CarregarGridHorarios(infoCarregamentoMontagem);
    infoCarregamentoMontagem.DataCarregamentoDisponibilidade.val.subscribe(function () {
        AtualizarHorarios(infoCarregamentoMontagem);
    });
        
    Global.abrirModal("divModalAlterarCarregamentoMontagem");
}

function aumentarDataClick(e, sender) {
    SetarData(e, 1);
}

function diminuirDataClick(e, sender) {
    SetarData(e, -1);
}

function confirmarAlteracaoCarregamentoMontagemCargaMapaClick(e) {
    var data = e.DataCarregamento.val();
    _carregamento.DataCarregamento.val(data);
    _carregamento.EncaixarHorario.val(e.EncaixarHorario.val());
    Global.fecharModal('divModalAlterarCarregamentoMontagem');
}

function encaixarHorarioClick(e) {
    e.EncaixarHorario.val(true);
    e.EncaixarHorario.visible(false);
    e.DataCarregamento.visible(true);
}

//*******MÉTODOS*******

function CarregarGridHorarios(e) {
    if (_gridHorariosInfoCarregamentoMontagem) _gridHorariosInfoCarregamentoMontagem.Destroy();

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

    _gridHorariosInfoCarregamentoMontagem = new GridView(e.DataCarregamentoDisponibilidade.idGrid, "JanelaCarregamento/ObterHorariosDisponiveis", e, null, null, 10, null, null, null, multiplaescolha);
    _gridHorariosInfoCarregamentoMontagem.CarregarGrid();
}

function HorarioSelecionado(e, data) {
    _gridHorariosInfoCarregamentoMontagem.AtualizarRegistrosSelecionados([data]);
    _gridHorariosInfoCarregamentoMontagem.DrawTable();

    var dataHoraComSegundos = e.DataCarregamentoDisponibilidade.val() + ' ' + data.HoraInicio;
    var dataHoraSemSegundos = dataHoraComSegundos.substring(0, 16);
    e.DataCarregamento.val(dataHoraSemSegundos);
}

function LimparHorarioSelecionado(e) {
    _gridHorariosInfoCarregamentoMontagem.AtualizarRegistrosSelecionados([]);
    _gridHorariosInfoCarregamentoMontagem.DrawTable();
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
    _gridHorariosInfoCarregamentoMontagem.CarregarGrid();
}