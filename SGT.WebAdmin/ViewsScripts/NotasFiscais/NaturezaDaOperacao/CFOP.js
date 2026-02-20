//*******MAPEAMENTO KNOUCKOUT*******

var _gridCFOP;
var _cfop;

var CFOP = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.CFOP = PropertyEntity({ type: types.event, text: "Adicionar CFOP", idBtnSearch: guid() });
}

//*******EVENTOS*******

function LoadCFOP() {

    _cfop = new CFOP();
    KoBindings(_cfop, "knockoutCFOP");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirCFOPClick(_cfop.CFOP, data)
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "CFOP", title: "CFOP", width: "20%" },
        { data: "Extensao", title: "Extensão", width: "20%" },
        { data: "Descricao", title: "Descrição", width: "45%" }
    ];

    _gridCFOP = new BasicDataTable(_cfop.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarCFOPNotaFiscal(_cfop.CFOP, function (r) {
        if (r != null) {
            var cfops = _gridCFOP.BuscarRegistros();
            for (var i = 0; i < r.length; i++)
                cfops.push({
                    Codigo: r[i].Codigo,
                    CFOP: r[i].CodigoCFOP,
                    Extensao: r[i].Extensao,
                    Descricao: r[i].Descricao
                });

            _gridCFOP.CarregarGrid(cfops);
        }
    }, null, null, null, null, null, null, _gridCFOP);

    _cfop.CFOP.basicTable = _gridCFOP;

    RecarregarGridCFOP();
}

function RecarregarGridCFOP() {

    var data = new Array();
    if (!string.IsNullOrWhiteSpace(_naturezaDaOperacao.CFOPs.val())) {
        $.each(_naturezaDaOperacao.CFOPs.val(), function (i, cfop) {
            var cfopGrid = new Object();

            cfopGrid.Codigo = cfop.CFOP.Codigo;
            cfopGrid.CFOP = cfop.CFOP.CFOP;
            cfopGrid.Extensao = cfop.CFOP.Extensao;
            cfopGrid.Descricao = cfop.CFOP.Descricao;

            data.push(cfopGrid);
        });
    }

    _gridCFOP.CarregarGrid(data);
}

function ExcluirCFOPClick(knoutCFOP, data) {
    var cfopGrid = knoutCFOP.basicTable.BuscarRegistros();

    for (var i = 0; i < cfopGrid.length; i++) {
        if (data.Codigo == cfopGrid[i].Codigo) {
            cfopGrid.splice(i, 1);
            break;
        }
    }

    knoutCFOP.basicTable.CarregarGrid(cfopGrid);
}

function LimparCamposCFOP() {
    LimparCampos(_cfop);
}