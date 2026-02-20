/// <reference path="../../Consultas/PerfilAcesso.js" />

var _gridPerfilAcesso;

var PerfilAcesso = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.PerfilAcesso = PropertyEntity({ type: types.event, text: "Adicionar Perfil de Acesso", idBtnSearch: guid() });
}

function loadPerfilAcesso() {
    _perfilAcesso = new PerfilAcesso();
    KoBindings(_perfilAcesso, "knockoutPerfilAcesso");

    loadGridPerfilAcesso();

    new BuscarPerfilAcessoChecklist(_perfilAcesso.PerfilAcesso, function (r) {
        if (r != null) {
            var pAcessos = _gridPerfilAcesso.BuscarRegistros();
            for (var i = 0; i < r.length; i++)
                pAcessos.push({
                    Codigo: r[i].Codigo,
                    Descricao: r[i].Descricao
                });

            _gridPerfilAcesso.CarregarGrid(pAcessos);
            _checkListTipo.PerfisAcesso.multiplesEntities(_gridPerfilAcesso.BuscarRegistros());
        }
    }, null, _gridPerfilAcesso);

    _perfilAcesso.PerfilAcesso.basicTable = _gridPerfilAcesso;
}

function loadGridPerfilAcesso() {
    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirPerfilAcesso(_perfilAcesso.PerfilAcesso, data)
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "85%" }
    ];

    _gridPerfilAcesso = new BasicDataTable(_perfilAcesso.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc }, null, 5);
    _gridPerfilAcesso.CarregarGrid([]);
}

function ExcluirPerfilAcesso(grid, registro) {
    let listaPerfilAcesso = grid.basicTable.BuscarRegistros();
    for (let i = 0; i < listaPerfilAcesso.length; i++) {
        if (listaPerfilAcesso[i].Codigo == registro.Codigo) {
            listaPerfilAcesso.splice(i, 1);
            break;
        }
    }
    grid.basicTable.CarregarGrid(listaPerfilAcesso);
}
