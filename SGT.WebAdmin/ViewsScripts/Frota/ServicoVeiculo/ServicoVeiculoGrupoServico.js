/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="ServicoVeiculo.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridServicoVeiculoGrupoServico;
var _servicoVeiculoGrupoServico;

var ServicoVeiculoGrupoServico = function () {
    this.Grid = PropertyEntity({ type: types.local });
};

//*******EVENTOS*******

function LoadServicoVeiculoGrupoServico() {
    _servicoVeiculoGrupoServico = new ServicoVeiculoGrupoServico();
    KoBindings(_servicoVeiculoGrupoServico, "knockoutServicoVeiculoGrupoServico");

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "80%" }
    ];

    _gridServicoVeiculoGrupoServico = new BasicDataTable(_servicoVeiculoGrupoServico.Grid.id, header, null, { column: 1, dir: orderDir.asc });

    RecarregarGridServicoVeiculoGrupoServico();
}

function RecarregarGridServicoVeiculoGrupoServico() {

    var data = new Array();
    if (!string.IsNullOrWhiteSpace(_servicoVeiculo.GruposServico.val())) {
        $.each(_servicoVeiculo.GruposServico.val(), function (i, grupoServico) {
            var grupoServicoGrid = new Object();

            grupoServicoGrid.Codigo = grupoServico.Codigo;
            grupoServicoGrid.Descricao = grupoServico.Descricao;

            data.push(grupoServicoGrid);
        });
    }
    _gridServicoVeiculoGrupoServico.CarregarGrid(data);
}

function LimparCamposServicoVeiculoGrupoServico() {
    LimparCampos(_servicoVeiculoGrupoServico);
    RecarregarGridServicoVeiculoGrupoServico();
    $("#liTabGrupoServico").hide();
}