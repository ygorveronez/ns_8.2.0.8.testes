var _gridTipoOperacao;

function loadTipoOperacao() {
    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                excluirTipoOperacaoClick(_configuracaoDescargaCliente.Tipo, data)
            }
        }]
    };

    var header = [{ data: "Codigo", visible: false },
    { data: "Descricao", title: "Descrição", width: "80%" }];

    _gridTipoOperacao = new BasicDataTable(_configuracaoDescargaCliente.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarTiposOperacao(_configuracaoDescargaCliente.Tipo, null, null, null, _gridTipoOperacao);
    _configuracaoDescargaCliente.Tipo.basicTable = _gridTipoOperacao;

    recarregarGridTipoOperacao();
}

function recarregarGridTipoOperacao() {
    var data = new Array();

    if (!string.IsNullOrWhiteSpace(_configuracaoDescargaCliente.TiposOperacoes.val())) {

        $.each(_configuracaoDescargaCliente.TiposOperacoes.val(), function (i, tipoOperacao) {
            var tipoOperacaoGrid = new Object();

            tipoOperacaoGrid.Codigo = tipoOperacao.Codigo;
            tipoOperacaoGrid.Descricao = tipoOperacao.Descricao;

            data.push(tipoOperacaoGrid);
        });
    }

    _gridTipoOperacao.CarregarGrid(data);
}


function excluirTipoOperacaoClick(knoutTipoOperacao, data) {
    var tipoOperacaoGrid = knoutTipoOperacao.basicTable.BuscarRegistros();

    for (var i = 0; i < tipoOperacaoGrid.length; i++) {
        if (data.Codigo == tipoOperacaoGrid[i].Codigo) {
            tipoOperacaoGrid.splice(i, 1);
            break;
        }
    }

    knoutTipoOperacao.basicTable.CarregarGrid(tipoOperacaoGrid);
}

function limparCamposTipoOperacao() {
    recarregarGridTipoOperacao();
    LimparCampos(_configuracaoDescargaCliente);
}