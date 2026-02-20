//*******MAPEAMENTO KNOUCKOUT*******

var _gridContrato;
var _contrato;

var Contrato = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Contrato = PropertyEntity({ type: types.event, text: Localization.Resources.Fretes.TabelaFrete.AdicionarContrato, idBtnSearch: guid() });
}

//*******EVENTOS*******

function LoadContrato() {
    _contrato = new Contrato();
    KoBindings(_contrato, "knockoutTabelaFreteContrato");

    let menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: Localization.Resources.Fretes.TabelaFrete.Excluir, id: guid(), metodo: function (data) {
                ExcluirContratoClick(_contrato.Contrato, data);
            }
        }]
    };

    let header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Fretes.TabelaFrete.Descricao, width: "80%" }
    ];

    _gridContrato = new BasicDataTable(_contrato.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarContratosTransporteFrete(_contrato.Contrato, null, _gridContrato);

    _contrato.Contrato.basicTable = _gridContrato;

    RecarregarGridContrato();
}

function RecarregarGridContrato() {

    let data = new Array();

    if (!string.IsNullOrWhiteSpace(_tabelaFrete.Contratos.val())) {

        $.each(_tabelaFrete.Contratos.val(), function (i, contrato) {
            let contratoGrid = new Object();

            contratoGrid.Codigo = contrato.Codigo;
            contratoGrid.Descricao = contrato.Descricao;

            data.push(contratoGrid);
        });
    }

    _gridContrato.CarregarGrid(data);
}


function ExcluirContratoClick(knoutContrato, data) {
    let contratoGrid = knoutContrato.basicTable.BuscarRegistros();

    for (let i = 0; i < contratoGrid.length; i++) {
        if (data.Codigo == contratoGrid[i].Codigo) {
            contratoGrid.splice(i, 1);
            break;
        }
    }

    knoutContrato.basicTable.CarregarGrid(contratoGrid);
}

function LimparCamposContrato() {
    LimparCampos(_contrato);
}