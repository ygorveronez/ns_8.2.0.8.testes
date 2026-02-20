var _checkListAuditoria;
var _gridCheckListAuditoria;
var _registrosAuditoriaChecklist = [];

var CheckListAuditoria = function () {
    this.Grid = PropertyEntity({ type: types.local, idGrid: guid() });
}

function carregarModalAuditoriaCheckList(data) {
    _registrosAuditoriaChecklist = data;

    var $divModalAuditoria = $("#divModalChecklistAuditoria");

    if ($divModalAuditoria.length == 0) {
        $.get("Content/Static/GestaoPatio/CheckListAuditoria.html" + "?dyn=" + guid(), function (data) {
            $("#widget-grid").after(data);

            _checkListAuditoria = new CheckListAuditoria();
            KoBindings(_checkListAuditoria, "divModalChecklistAuditoria");

            carregarGridAuditoriaCheckList();
        }).done(function () {
            abrirModalAuditoriaCheckList();
        });
    }
    else
        abrirModalAuditoriaCheckList();
}

function carregarGridAuditoriaCheckList() {
    var header = [
        { data: "Codigo" },
        { data: "Pergunta", title: "Pergunta", width: "30%", className: "text-align-left" },
        { data: "Data", title: "Data", width: "10%", className: "text-align-center" },
        { data: "RespostaAntiga", title: "Resposta Antiga", width: "20%", className: "text-align-left" },
        { data: "RespostaNova", title: "Resposta Nova", width: "20%", className: "text-align-left" },
        { data: "Observacao", title: "Observação", width: "20%", className: "text-align-left" },
    ];

    _gridCheckListAuditoria = new BasicDataTable(_checkListAuditoria.Grid.idGrid, header, null);
    _gridCheckListAuditoria.CarregarGrid([]);
}

function abrirModalAuditoriaCheckList() {
    _gridCheckListAuditoria.CarregarGrid(_registrosAuditoriaChecklist);
    
    $("#divModalChecklistAuditoria").modal("show")
        .one("hidden.bs.modal", function () {
            _gridCheckListAuditoria.CarregarGrid([]);
        });
}