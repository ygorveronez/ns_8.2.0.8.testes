var _gridCampos = null;

function LoadCampoTipoOperacao() {
    var menuOpcoes = {
        tipo: TypeOptionMenu.checkbox, tamanho: 7, opcoes: [{
            descricao: "Valor Padrão", id: guid(), metodo: function (data) {
                ExcluirCampoClick(_tipoOperacaoCampoValorPadrao.Campo, data);
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "70%" },
    ];

    _gridCampos = new BasicDataTable(_tipoOperacaoCampoValorPadrao.GridCampo.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarCampoTipoOperacao(_tipoOperacaoCampoValorPadrao.Campo, null, _gridCampos);

    _tipoOperacaoCampoValorPadrao.Campo.basicTable = _gridCampos;
    _tipoOperacaoCampoValorPadrao.Campo.basicTable.CarregarGrid(new Array());
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