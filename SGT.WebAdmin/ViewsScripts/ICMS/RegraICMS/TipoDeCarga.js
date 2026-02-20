var _gridTipoDeCarga = null;

function LoadTiposDeCarga() {
    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirTipoDeCargaClick(_regraICMS.AdicionarTipoDeCarga, data);
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "80%" }
    ];

    _gridTipoDeCarga = new BasicDataTable(_regraICMS.GridTipoDeCarga.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarTiposdeCarga(_regraICMS.AdicionarTipoDeCarga, null, _regraICMS.GrupoPessoas, _gridTipoDeCarga);

    _regraICMS.AdicionarTipoDeCarga.basicTable = _gridTipoDeCarga;
    _regraICMS.AdicionarTipoDeCarga.basicTable.CarregarGrid(new Array());
}

function ExcluirTipoDeCargaClick(knoutTipoDeCarga, data) {
    var tipoDeCargaGrid = knoutTipoDeCarga.basicTable.BuscarRegistros();

    for (var i = 0; i < tipoDeCargaGrid.length; i++) {
        if (data.Codigo == tipoDeCargaGrid[i].Codigo) {
            tipoDeCargaGrid.splice(i, 1);
            break;
        }
    }

    knoutTipoDeCarga.basicTable.CarregarGrid(tipoDeCargaGrid);
}