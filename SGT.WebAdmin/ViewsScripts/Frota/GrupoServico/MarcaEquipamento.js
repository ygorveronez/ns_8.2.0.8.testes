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
/// <reference path="../../Consultas/MarcaEquipamento.js" />
/// <reference path="GrupoServico.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridMarcaEquipamento;
var _marcaEquipamento;

var MarcaEquipamento = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.MarcaEquipamento = PropertyEntity({ type: types.event, text: "Adicionar Marca de Equipamento", idBtnSearch: guid() });
};

//*******EVENTOS*******

function LoadMarcaEquipamento() {
    _marcaEquipamento = new MarcaEquipamento();
    KoBindings(_marcaEquipamento, "knockoutGrupoServicoMarcaEquipamento");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirMarcaEquipamentoClick(data)
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "80%" }
    ];

    _gridMarcaEquipamento = new BasicDataTable(_marcaEquipamento.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarMarcaEquipamentos(_marcaEquipamento.MarcaEquipamento, null, null, null, _gridMarcaEquipamento);
    _marcaEquipamento.MarcaEquipamento.basicTable = _gridMarcaEquipamento;

    RecarregarGridMarcaEquipamento();
}

function RecarregarGridMarcaEquipamento() {

    var data = new Array();

    if (!string.IsNullOrWhiteSpace(_grupoServico.MarcasEquipamento.val())) {

        $.each(_grupoServico.MarcasEquipamento.val(), function (i, marcaEquipamento) {
            var marcaEquipamentoGrid = new Object();

            marcaEquipamentoGrid.Codigo = marcaEquipamento.Codigo;
            marcaEquipamentoGrid.Descricao = marcaEquipamento.Descricao;

            data.push(marcaEquipamentoGrid);
        });
    }

    _gridMarcaEquipamento.CarregarGrid(data);
}

function ExcluirMarcaEquipamentoClick(data) {
    var marcaEquipamentoGrid = _marcaEquipamento.MarcaEquipamento.basicTable.BuscarRegistros();

    for (var i = 0; i < marcaEquipamentoGrid.length; i++) {
        if (data.Codigo == marcaEquipamentoGrid[i].Codigo) {
            marcaEquipamentoGrid.splice(i, 1);
            break;
        }
    }

    _marcaEquipamento.MarcaEquipamento.basicTable.CarregarGrid(marcaEquipamentoGrid);
}

function LimparCamposMarcaEquipamento() {
    LimparCampos(_marcaEquipamento);
    _marcaEquipamento.MarcaEquipamento.basicTable.CarregarGrid(new Array());
}