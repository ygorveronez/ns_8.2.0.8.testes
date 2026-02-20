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
/// <reference path="../../Consultas/CentroResultado.js" />

var _manutencaoCentroResultado;

var ManutencaoCentroResultado = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.CentroResultado = PropertyEntity({ text: ko.observable("Selecione o Centro de Resultado:"), type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
}

function loadManutencaoCentroResultado() {
    _manutencaoCentroResultado = new ManutencaoCentroResultado();
    KoBindings(_manutencaoCentroResultado, "knockoutPesquisaCentroResultado");

    _painelMotoristas = new PainelMotoristas();
    KoBindings(_painelMotoristas, "knockoutMotoristas");

    _painelTracao = new PainelTracao();
    KoBindings(_painelTracao, "knockoutTracao");

    _painelReboques = new PainelReboques();
    KoBindings(_painelReboques, "knockoutReboques");

    new BuscarCentroResultado(_manutencaoCentroResultado.CentroResultado, null, null, retornoCentroResultado);
    new BuscarMotoristas(_painelMotoristas.Adicionar, retornoAdicionarMotorista);
    new BuscarTracaoManobra(_painelTracao.Adicionar, retornoAdicionarTracao);
    new BuscarReboques(_painelReboques.Adicionar, retornoAdicionarReboque);

    CarregarGrids();
}

function CarregarGrids() {
    CarregarGridMotoristas();
    CarregarGridTracao();
    CarregarGridReboques();
}

function retornoCentroResultado(data) {
    _manutencaoCentroResultado.CentroResultado.text("Centro de Resultado:")

    _manutencaoCentroResultado.Codigo.val(data.Codigo);
    _manutencaoCentroResultado.CentroResultado.codEntity(data.Codigo);
    _manutencaoCentroResultado.CentroResultado.val(data.Descricao);

    BuscarRegistros();
}

function BuscarRegistros() {
    _gridMotoristas.CarregarGrid();
    _gridTracao.CarregarGrid();
    _gridReboques.CarregarGrid();

    $("#divPaineis").show();
}