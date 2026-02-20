function removerAjudanteClick(knoutAjudantes, data) {

    var ajudanteGrid = knoutAjudantes.basicTable.BuscarRegistros();
    for (var i = 0; i < ajudanteGrid.length; i++) {
        if (data.Codigo == ajudanteGrid[i].Codigo) {
            ajudanteGrid.splice(i, 1);
            break;
        }
    }
    knoutAjudantes.basicTable.CarregarGrid(ajudanteGrid);
}

//*******MÉTODOS*******

function atualizarGridDadosAjudante(knoutCarga, carga) {
    var ajudantes = new Array();

    if (carga.Ajudantes != null) {
        $.each(carga.Ajudantes, function (i, ajudante) {
            ajudantes.push({ Codigo: ajudante.Codigo, CPF: ajudante.CPF, Nome: ajudante.Descricao });
        });
    }

    if (knoutCarga.AdicionarAjudantes?.basicTable != null)
        knoutCarga.AdicionarAjudantes.basicTable.CarregarGrid(ajudantes);
}

function preecherGridDadosAjudante(knoutCarga, carga) {
    var ajudantes = new Array();

    if (carga.Ajudantes != null) {
        $.each(carga.Ajudantes, function (i, ajudante) {
            ajudantes.push({ Codigo: ajudante.Codigo, CPF: ajudante.CPF, Nome: ajudante.Descricao });
        });
    }

    var remover = {
        descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), metodo: function (datagrid) {
            removerAjudanteClick(knoutCarga.AdicionarAjudantes, datagrid);
        }, icone: ""
    };
    var menuOpcoes = knoutCarga.EtapaInicioTMS.enable() ? { tipo: TypeOptionMenu.link, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 15, opcoes: [remover] } : null;

    var header = [
        { data: "Codigo", visible: false },
        { data: "CPF", title: Localization.Resources.Cargas.Carga.CPF, width: "15%", className: "text-align-center", orderable: false },
        { data: "Nome", title: Localization.Resources.Gerais.Geral.Nome, width: "70%", className: "text-align-left", orderable: false }
    ];

    var gridAjudantes = new BasicDataTable(knoutCarga.AdicionarAjudantes.idGrid, header, menuOpcoes);

    gridAjudantes.CarregarGrid(ajudantes);

    knoutCarga.AdicionarAjudantes.basicTable = gridAjudantes;
    new BuscarMotoristas(knoutCarga.AdicionarAjudantes, null, knoutCarga.Empresa, gridAjudantes, true, null, null, null, null, false);
}