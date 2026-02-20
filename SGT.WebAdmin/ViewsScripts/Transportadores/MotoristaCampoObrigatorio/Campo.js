var _gridCampos = null;

function LoadCampoMotorista() {
    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirCampoClick(_motoristaCampoObrigatorio.Campo, data);
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "80%" }
    ];

    _gridCampos = new BasicDataTable(_motoristaCampoObrigatorio.GridCampo.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarCamposMotorista(_motoristaCampoObrigatorio.Campo, null, _gridCampos);

    _motoristaCampoObrigatorio.Campo.basicTable = _gridCampos;
    _motoristaCampoObrigatorio.Campo.basicTable.CarregarGrid(new Array());
}

function ExcluirCampoClick(knoutCampo, data) {
    var camposGrid = knoutCampo.basicTable.BuscarRegistros();

    for (var i = 0; i < camposGrid.length; i++) {
        if (data.Codigo == camposGrid[i].Codigo) {
            camposGrid.splice(i, 1);
            break;
        }
    }

    knoutCampo.basicTable.CarregarGrid(camposGrid);
}