//*******MAPEAMENTO KNOUCKOUT*******

var _gridGrupoTransportadorEmpresa;
var _grupoTransportadorEmpresa;

var GrupoTransportadorEmpresa = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Empresa = PropertyEntity({ type: types.event, text: "Adicionar Transportadores", idBtnSearch: guid(), issue: 0 });
};


//*******EVENTOS*******

function LoadGrupoTransportadorEmpresa() {

    _grupoTransportadorEmpresa = new GrupoTransportadorEmpresa();
    KoBindings(_grupoTransportadorEmpresa, "knockoutGrupoTransportadorEmpresa");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirGrupoTransportadorEmpresaClick(_grupoTransportadorEmpresa.Empresa, data);
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Transportador", width: "60%" },
        { data: "CNPJ", title: "CPF/CNPJ", width: "20%", className: "text-align-center" }
    ];

    _gridGrupoTransportadorEmpresa = new BasicDataTable(_grupoTransportadorEmpresa.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarTransportadores(_grupoTransportadorEmpresa.Empresa, null, menuOpcoes, null, _gridGrupoTransportadorEmpresa);

    _grupoTransportadorEmpresa.Empresa.basicTable = _gridGrupoTransportadorEmpresa;

    RecarregarGridGrupoTransportadorEmpresa();
}

function RecarregarGridGrupoTransportadorEmpresa() {
    _gridGrupoTransportadorEmpresa.CarregarGrid(_grupoTransportador.Empresas.val());
}

function ExcluirGrupoTransportadorEmpresaClick(knoutEmpresas, data) {

    var empresasGrid = knoutEmpresas.basicTable.BuscarRegistros();

    for (var i = 0; i < empresasGrid.length; i++) {
        if (data.Codigo == empresasGrid[i].Codigo) {
            empresasGrid.splice(i, 1);
            break;
        }
    }

    knoutEmpresas.basicTable.CarregarGrid(empresasGrid);
}

function LimparCamposGrupoTransportadorEmpresa() {
    LimparCampos(_grupoTransportadorEmpresa);
    _gridGrupoTransportadorEmpresa.CarregarGrid(new Array());
}