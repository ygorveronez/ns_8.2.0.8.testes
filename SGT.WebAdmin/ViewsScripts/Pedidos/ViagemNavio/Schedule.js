/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />

//*******MAPEAMENTO KNOUCKOUT*******
var _schedule;
var _gridSchedules;

var Schedules = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0 });

    this.Schedules = PropertyEntity({ type: types.local });


    this.TerminalAtracacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Terminal Atracação:", idBtnSearch: guid(), required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });
    this.PortoAtracacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Porto Atracação:", idBtnSearch: guid(), required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });
    this.DataPrevisaoChegadaNavio = PropertyEntity({ text: "ETA: ", getType: typesKnockout.dateTime, required: false, issue: 2 });
    this.DataPrevisaoSaidaNavio = PropertyEntity({ text: "ETS: ", getType: typesKnockout.dateTime, required: false, issue: 2 });
    this.DataDeadLine = PropertyEntity({ text: "Dead Line ", getType: typesKnockout.dateTime, required: false, issue: 2 });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(true), options: _status, def: true, required: ko.observable(false), enable: ko.observable(true) });

    this.ETAConfirmado = PropertyEntity({ getType: typesKnockout.bool, text: "ETA Confirmado?", val: ko.observable(false), def: false, visible: true });
    this.ETSConfirmado = PropertyEntity({ getType: typesKnockout.bool, text: "ETS Confirmado?", issue: 901, val: ko.observable(false), def: false, visible: true });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarScheduleClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarScheduleClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirScheduleClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: LimparCamposSchedule, type: types.event, text: "Cancelar", visible: ko.observable(true) });
}

//*******EVENTOS*******
function LoadSchedules() {
    _schedule = new Schedules();
    KoBindings(_schedule, "knockoutSchedule");

    _viagemNavio.Codigo.val.subscribe(function (val) {
        _schedule.Codigo.val(val);
    });

    GridSchedules();

    new BuscarPorto(_schedule.PortoAtracacao);
    new BuscarTipoTerminalImportacao(_schedule.TerminalAtracacao);
}

function AdicionarScheduleClick() {
    var valido = ValidarCamposObrigatorios(_schedule);

    if (valido) {
        _schedule.Codigo.val(guid());

        var data = GetSchedules();
        var item = SalvarListEntity(_schedule);

        data.push(item);
        SetSchedules(data);

        RecarregarGridSchedules();

        LimparCamposSchedule();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }
}

function AtualizarScheduleClick() {
    var valido = ValidarCamposObrigatorios(_schedule);

    if (valido) {
        var itens = GetSchedules();
        var item = SalvarListEntity(_schedule);
        var codigo = _schedule.Codigo.val();

        for (var i = 0, s = itens.length; i < s; i++) {
            if (codigo == itens[i].Codigo.val) {
                itens[i] = item;
                break;
            }
        }
        SetSchedules(itens);

        RecarregarGridSchedules();
        LimparCamposSchedule();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }
}

function ExcluirScheduleClick() {
    var itens = GetSchedules();
    var codigo = _schedule.Codigo.val();
    for (var i = 0, s = itens.length; i < s; i++) {
        if (codigo == itens[i].Codigo.val) {
            itens.splice(i, 1);
            break;
        }
    }

    SetSchedules(itens);

    RecarregarGridSchedules();
    LimparCamposSchedule();
}

function EditarScheduleClick(data) {
    var itens = GetSchedules();
    var schedule = null;
    for (var i = 0, s = itens.length; i < s; i++) {
        if (data.Codigo == itens[i].Codigo.val) {
            schedule = itens[i];
            break;
        }
    }

    if (schedule != null) {
        _schedule.Codigo.val(schedule.Codigo.val);

        _schedule.TerminalAtracacao.val(schedule.TerminalAtracacao.val);
        _schedule.TerminalAtracacao.codEntity(schedule.TerminalAtracacao.codEntity);
        _schedule.PortoAtracacao.val(schedule.PortoAtracacao.val);
        _schedule.PortoAtracacao.codEntity(schedule.PortoAtracacao.codEntity);
        _schedule.DataPrevisaoChegadaNavio.val(schedule.DataPrevisaoChegadaNavio.val);
        _schedule.DataPrevisaoSaidaNavio.val(schedule.DataPrevisaoSaidaNavio.val);
        _schedule.DataDeadLine.val(schedule.DataDeadLine.val);
        _schedule.Status.val(schedule.Status.val);
        _schedule.ETAConfirmado.val(schedule.ETAConfirmado.val);
        _schedule.ETSConfirmado.val(schedule.ETSConfirmado.val);

        _schedule.Adicionar.visible(false);
        _schedule.Atualizar.visible(true);
        _schedule.Excluir.visible(true);
    }
}


//*******MÉTODOS*******
function GridSchedules() {
    if (_gridSchedules && _gridSchedules.Destroy)
        _gridSchedules.Destroy();
    _schedule.Schedules.get$().empty();

    var editar = { descricao: "Editar", id: guid(), metodo: EditarScheduleClick, icone: "" };
    var auditar = { descricao: "Auditoria", id: guid(), evento: "onclick", metodo: OpcaoAuditoria("PedidoViagemNavioSchedule", "Codigo"), tamanho: "20", icone: "", visibilidade: VisibilidadeOpcaoAuditoria };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 5, opcoes: [editar, auditar]  };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Status", visible: false },
        { data: "TerminalAtracacao", title: "Terminal", width: "30%" },
        { data: "PortoAtracacao", title: "Porto", width: "30%" },
        { data: "DataPrevisaoChegadaNavio", title: "ETA", width: "10%" },
        { data: "DataPrevisaoSaidaNavio", title: "ETS", width: "10%" },
        { data: "DataDeadLine", title: "Dead Line", width: "10%" },
        { data: "ETAConfirmado", visible: false },
        { data: "ETSConfirmado", visible: false }
    ];

    _gridSchedules = new BasicDataTable(_schedule.Schedules.id, header, menuOpcoes);

    RecarregarGridSchedules();
}

function GetSchedules() {
    return _viagemNavio.Schedules.list.slice();
}

function SetSchedules(data) {
    return _viagemNavio.Schedules.list = data.slice();
}

function RetornoSchedule(schedule) {
    _schedule.Schedule.val(schedule.Descricao);
    if (schedule.Codigo == 0)
        _schedule.Schedule.codEntity(schedule.CodigoSchedule);
    else
        _schedule.Schedule.codEntity(schedule.Codigo);

    _schedule.EstoqueAtual.val(schedule.Quantidade);
}

function RecarregarGridSchedules() {
    var data = [];

    $.each(GetSchedules(), function (i, schedule) {
        var itemGrid = new Object();

        itemGrid.Codigo = schedule.Codigo.val;
        itemGrid.TerminalAtracacao = schedule.TerminalAtracacao.val;
        itemGrid.PortoAtracacao = schedule.PortoAtracacao.val;
        itemGrid.DataPrevisaoChegadaNavio = schedule.DataPrevisaoChegadaNavio.val;
        itemGrid.DataPrevisaoSaidaNavio = schedule.DataPrevisaoSaidaNavio.val;
        itemGrid.DataDeadLine = schedule.DataDeadLine.val;
        itemGrid.Status = schedule.Status.val;
        itemGrid.ETAConfirmado = schedule.ETAConfirmado.val;
        itemGrid.ETSConfirmado = schedule.ETSConfirmado.val;

        data.push(itemGrid);
    });

    _gridSchedules.CarregarGrid(data);
}

function LimparCamposSchedule() {
    LimparCampos(_schedule);
    _schedule.Adicionar.visible(true);
    _schedule.Atualizar.visible(false);
    _schedule.Excluir.visible(false);
}
