//*******MAPEAMENTO KNOUCKOUT*******

var _gridFronteira;
var _fronteira;

var Fronteira = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Fronteira = PropertyEntity({ type: types.event, text: Localization.Resources.Fretes.TabelaFrete.AdicionarFronteira, idBtnSearch: guid() });
}

//*******EVENTOS*******

function LoadFronteira() {

    _fronteira = new Fronteira();
    KoBindings(_fronteira, "knockoutFronteira");

    let menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: Localization.Resources.Fretes.TabelaFrete.Excluir, id: guid(), metodo: function (data) {
                ExcluirFronteiraClick(_fronteira.Fronteira, data);
            }
        }]
    };

    let header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Fretes.TabelaFrete.Descricao, width: "80%" }
    ];

    _gridFronteira = new BasicDataTable(_fronteira.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    BuscarClientes(_fronteira.Fronteira, null, null, null, null, _gridFronteira, null, null, null, null, null, null, null, null, null, null, true, null);

    _fronteira.Fronteira.basicTable = _gridFronteira;

    RecarregarGridFronteira();
}

function RecarregarGridFronteira() {

    let data = new Array();

    if (!string.IsNullOrWhiteSpace(_tabelaFrete.Fronteiras.val())) {

        $.each(_tabelaFrete.Fronteiras.val(), function (i, fronteira) {
            let fronteiraGrid = new Object();

            fronteiraGrid.Codigo = fronteira.Codigo;
            fronteiraGrid.Descricao = fronteira.Descricao;

            data.push(fronteiraGrid);
        });
    }

    _gridFronteira.CarregarGrid(data);
}


function ExcluirFronteiraClick(knoutFronteira, data) {
    let fronteiraGrid = knoutFronteira.basicTable.BuscarRegistros();

    for (let i = 0; i < fronteiraGrid.length; i++) {
        if (data.Codigo == fronteiraGrid[i].Codigo) {
            fronteiraGrid.splice(i, 1);
            break;
        }
    }

    knoutFronteira.basicTable.CarregarGrid(fronteiraGrid);
}

function LimparCamposFronteira() {
    LimparCampos(_fronteira);
}