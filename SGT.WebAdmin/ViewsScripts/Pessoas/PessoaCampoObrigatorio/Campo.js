/// <reference path="../../Consultas/CampoPessoa.js" />
/// <reference path="PessoaCampoObrigatorio.js" />

var _gridCampos = null;

function LoadCampoPessoa() {
    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirCampoClick(_pessoaCampoObrigatorio.Campo, data);
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "80%" }
    ];

    _gridCampos = new BasicDataTable(_pessoaCampoObrigatorio.GridCampo.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarCamposPessoa(_pessoaCampoObrigatorio.Campo, null, _gridCampos);

    _pessoaCampoObrigatorio.Campo.basicTable = _gridCampos;
    _pessoaCampoObrigatorio.Campo.basicTable.CarregarGrid(new Array());
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