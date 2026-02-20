/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _gridHorarioAcesso;
var _horarioAcesso;

var HorarioAcessoMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: 0, def: 0, getType: typesKnockout.int });
    this.DiasDaSemana = PropertyEntity({ type: types.map, val: "" });
    this.DiasDaSemanaDescricao = PropertyEntity({ type: types.map, val: "" });
    this.HoraInicial = PropertyEntity({ type: types.map, val: "" });
    this.HoraFinal = PropertyEntity({ type: types.map, val: "" });
    this.HorarioDescricao = PropertyEntity({ type: types.map, val: "" });
};

var HorarioAcesso = function () {
    this.Grid = PropertyEntity({ type: types.local });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.DiasDaSemana = PropertyEntity({ text: Localization.Resources.Filiais.Turno.DiasSemana.getRequiredFieldDescription(), getType: typesKnockout.selectMultiple, val: ko.observable(new Array()), def: new Array(), options: EnumDiaSemana.obterOpcoes() });
    this.HoraInicial = PropertyEntity({ getType: typesKnockout.time, text: Localization.Resources.Filiais.Turno.HoraInicial.getRequiredFieldDescription(), type: types.time, required: true });
    this.HoraFinal = PropertyEntity({ getType: typesKnockout.time, text: Localization.Resources.Filiais.Turno.HoraFinal.getRequiredFieldDescription(), type: types.time, required: true });

    this.Adicionar = PropertyEntity({ eventClick: adicionarHorarioAcessoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarHorarioAcessoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirHorarioAcessoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarHorarioAcessoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });
};

function loadHorarioAcesso() {
    _horarioAcesso = new HorarioAcesso();
    KoBindings(_horarioAcesso, "knockoutHorarioAcesso");

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: editarHorarioAcessoClick }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "DiasDaSemana", visible: false },
        { data: "DiasDaSemanaDescricao", title: Localization.Resources.Filiais.Turno.DiasSemana, width: "50%" },
        { data: "HoraInicial", visible: false },
        { data: "HoraFinal", visible: false },
        { data: "HorarioDescricao", title: Localization.Resources.Filiais.Turno.Horarios, width: "30%" }
    ];

    _gridHorarioAcesso = new BasicDataTable(_horarioAcesso.Grid.id, header, menuOpcoes);
    RecarregarGridHorarioAcesso();
}

function editarHorarioAcessoClick(data) {
    LimparCamposHorarioAcesso();
        
    $.each(_turno.HorariosAcessos.list, function (i, horarioAcesso) {
        if (horarioAcesso.Codigo.val == data.Codigo) {
            _horarioAcesso.Codigo.val(horarioAcesso.Codigo.val);
            //_horarioAcesso.DiasDaSemana.val(JSON.parse(horarioAcesso.DiasDaSemana.val));
            _horarioAcesso.HoraInicial.val(horarioAcesso.HoraInicial.val);
            _horarioAcesso.HoraFinal.val(horarioAcesso.HoraFinal.val);

            $("#" + _horarioAcesso.DiasDaSemana.id).selectpicker('val', JSON.parse(horarioAcesso.DiasDaSemana.val));

            return false;
        }
    });

    _horarioAcesso.Atualizar.visible(true);
    _horarioAcesso.Cancelar.visible(true);
    _horarioAcesso.Excluir.visible(true);
    _horarioAcesso.Adicionar.visible(false);
}

function cancelarHorarioAcessoClick(e) {
    LimparCamposHorarioAcesso();
}

function excluirHorarioAcessoClick() {
    for (var i = 0; i < _turno.HorariosAcessos.list.length; i++) {
        horarioAcesso = _turno.HorariosAcessos.list[i];

        if (_horarioAcesso.Codigo.val() == horarioAcesso.Codigo.val)
            _turno.HorariosAcessos.list.splice(i, 1);
    }
    LimparCamposHorarioAcesso();
    RecarregarGridHorarioAcesso();
}

function adicionarHorarioAcessoClick(e, sender) {
    if (ValidarCamposObrigatorios(_horarioAcesso) && e.DiasDaSemana.val().length > 0) {
        if (ValidarHorario(e)) {
            _turno.HorariosAcessos.list.push(ObterEntityHorarioAcesso(_horarioAcesso));
            RecarregarGridHorarioAcesso();
            LimparCamposHorarioAcesso();
            $("#" + _horarioAcesso.DiasDaSemana.id).focus();
        } else {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Filiais.Turno.Atencao, Localization.Resources.Filiais.Turno.HorarioFinalDeveSerMaiorInicial);
        }
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
    }
}

function atualizarHorarioAcessoClick(e, sender) {
    if (ValidarCamposObrigatorios(_horarioAcesso) && e.DiasDaSemana.val().length > 0) {
        if (ValidarHorario(e)) {
            $.each(_turno.HorariosAcessos.list, function (i, horarioAcesso) {
                if (horarioAcesso.Codigo.val == _horarioAcesso.Codigo.val()) {
                    var horarioAcessoAtual = ObterEntityHorarioAcesso(e);

                    horarioAcesso.DiasDaSemana.val = horarioAcessoAtual.DiasDaSemana.val;
                    horarioAcesso.DiasDaSemanaDescricao.val = horarioAcessoAtual.DiasDaSemanaDescricao.val;
                    horarioAcesso.HoraInicial.val = horarioAcessoAtual.HoraInicial.val;
                    horarioAcesso.HoraFinal.val = horarioAcessoAtual.HoraFinal.val;
                    horarioAcesso.HorarioDescricao.val = horarioAcessoAtual.HorarioDescricao.val;
                    return false;
                }
            });
            RecarregarGridHorarioAcesso();
            LimparCamposHorarioAcesso();
        } else {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Filiais.Turno.Atencao, Localization.Resources.Filiais.Turno.HorarioFinalDeveSerMaiorInicial);
        }
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
    }
}

//*******METODOS*******
function RecarregarGridHorarioAcesso() {
    var data = new Array();

    $.each(_turno.HorariosAcessos.list, function (i, horarioAcesso) {
        var horarioAcessoGrid = new Object();

        horarioAcessoGrid.Codigo = horarioAcesso.Codigo.val;
        horarioAcessoGrid.DiasDaSemana = horarioAcesso.DiasDaSemana.val;
        horarioAcessoGrid.DiasDaSemanaDescricao = horarioAcesso.DiasDaSemanaDescricao.val;
        horarioAcessoGrid.HoraInicial = horarioAcesso.HoraInicial.val;
        horarioAcessoGrid.HoraFinal = horarioAcesso.HoraFinal.val;
        horarioAcessoGrid.HorarioDescricao = horarioAcesso.HorarioDescricao.val;

        data.push(horarioAcessoGrid);
    });
    _gridHorarioAcesso.CarregarGrid(data);
}

function LimparCamposHorarioAcesso() {
    _horarioAcesso.Atualizar.visible(false);
    _horarioAcesso.Excluir.visible(false);
    _horarioAcesso.Cancelar.visible(false);
    _horarioAcesso.Adicionar.visible(true);
    LimparCampos(_horarioAcesso);
}

function ObterEntityHorarioAcesso(knout) {
    var diasDaSemana = knout.DiasDaSemana.val();
    var descricaoDiasDaSemana = new Array();

    if (diasDaSemana.length > 0) {
        diasDaSemana.forEach(function (DiaDaSemana, i) {
            descricaoDiasDaSemana.push(EnumDiaSemana.obterDescricaoSemConfiguracao(DiaDaSemana));
        });
    }
    var horarioAcesso = new HorarioAcessoMap();
    
    horarioAcesso.Codigo.val = knout.Codigo.val() > 0 ? knout.Codigo.val() : guid();
    horarioAcesso.DiasDaSemana.val = JSON.stringify(knout.DiasDaSemana.val());
    horarioAcesso.DiasDaSemanaDescricao.val = descricaoDiasDaSemana.join(", ");
    horarioAcesso.HoraInicial.val = knout.HoraInicial.val();
    horarioAcesso.HoraFinal.val = knout.HoraFinal.val();
    horarioAcesso.HorarioDescricao.val = Localization.Resources.Filiais.Turno.XAteX.format(knout.HoraInicial.val(), knout.HoraFinal.val());

    return horarioAcesso;
}

function ValidarHorario(knout) {
    var horaInicial = knout.HoraInicial.val();
    var horaFinal = knout.HoraFinal.val();

    if (horaInicial == horaFinal) {
        return false;
    } else {
        var horarioinicial = horaInicial.split(":");
        var horariofinal = horaFinal.split(":");
        var totalInicial = ((parseInt(horarioinicial[0]) * 60) + parseInt(horarioinicial[1]));
        var totalFinal = ((parseInt(horariofinal[0]) * 60) + parseInt(horariofinal[1]));

        if (totalFinal <= totalInicial)
            return false;
    }

    return true;
}