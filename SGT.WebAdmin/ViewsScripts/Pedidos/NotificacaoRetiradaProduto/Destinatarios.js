var _gridDestinatarios;
var _destinatarios;

var Destinatarios = function () {
    this.AdicionarDestinatario = PropertyEntity({ type: types.event, text: "Adicionar", idBtnSearch: guid() });
    this.Grid = PropertyEntity({ type: types.local });
}

function LoadDestinatario() {
    _destinatarios = new Destinatarios();
    KoBindings(_destinatarios, "knockoutDestinatarios");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirDestinatarioClick(data);
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "80%" }
    ];

    _gridDestinatarios = new BasicDataTable(_destinatarios.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarFuncionario(_destinatarios.AdicionarDestinatario, null, _gridDestinatarios);

    _destinatarios.AdicionarDestinatario.basicTable = _gridDestinatarios;

    RecarregarGridDestinatarios();
}

function RecarregarGridDestinatarios() {
    var data = new Array();
    if (!string.IsNullOrWhiteSpace(_notificacaoRetiradaProduto.Destinatarios.val())) {
        $.each(_notificacaoRetiradaProduto.Destinatarios.val(), function (i, destinatario) {
            var destinatarioGrid = new Object();

            destinatarioGrid.Codigo = destinatario.Codigo;
            destinatarioGrid.Descricao = destinatario.Descricao;

            data.push(destinatarioGrid);
        });
    }
    _gridDestinatarios.CarregarGrid(data);
}

function ExcluirDestinatarioClick(data) {
    var destinatarioGrid = _destinatarios.AdicionarDestinatario.basicTable.BuscarRegistros();

    for (var i = 0; i < destinatarioGrid.length; i++) {
        if (data.Codigo == destinatarioGrid[i].Codigo) {
            destinatarioGrid.splice(i, 1);
            break;
        }
    }

    _destinatarios.AdicionarDestinatario.basicTable.CarregarGrid(destinatarioGrid);
}

function LimparGridDestinatarios() {
    _destinatarios.AdicionarDestinatario.basicTable.CarregarGrid(new Array());
}