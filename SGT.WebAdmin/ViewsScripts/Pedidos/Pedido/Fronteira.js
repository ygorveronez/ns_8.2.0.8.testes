var _mensagemFronteiraSituacao; 

function LoadGridFronteiras() {
    var excluir = {
        descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), evento: "onclick", metodo: function (data) {
            RemoverFronteiraClick(_pedido.Fronteiras, data);
        }, tamanho: "15", icone: ""
    };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(excluir);

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "60%", className: "text-align-left" },
    ];

    _gridFronteiras = new BasicDataTable(_pedido.Fronteiras.idGrid, header, menuOpcoes);
    _pedido.Fronteiras.basicTable = _gridFronteiras;

    BuscarClientes(_pedido.Fronteiras, RetornoInserirFronteira, null, null, null, _gridFronteiras, null, null, null, null, null, null, null, null, null, null, true); 
    RecarregarListaFronteiras();
}

function RemoverFronteiraClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Pedidos.Pedido.RealmenteDesejaRemoverFronteira.format(sender.Descricao), function () {
        var fronteiraGrid = e.basicTable.BuscarRegistros();
        for (var i = 0; i < fronteiraGrid.length; i++) {
            if (sender.Codigo == fronteiraGrid[i].Codigo) {
                fronteiraGrid.splice(i, 1);
                break;
            }
        }
        e.basicTable.CarregarGrid(fronteiraGrid);
    });
}

function RetornoInserirFronteira(data) {
    if (data != null) {
        var dataGrid = _gridFronteiras.BuscarRegistros();

        dataGrid = [].concat(dataGrid, data);

        _gridFronteiras.CarregarGrid(dataGrid);
    }
}

function RecarregarListaFronteiras() {
    var data = new Array();
    if (!string.IsNullOrWhiteSpace(_pedido.ListaFronteiras.val())) {
        $.each(_pedido.ListaFronteiras.val(), function (i, fronteira) {
            var obj = new Object();

            obj.Codigo = fronteira.CPF_CNPJ;
            obj.Descricao = fronteira.Descricao;

            data.push(obj);
        });
    }
    _gridFronteiras.CarregarGrid(data);
}

function preencherListaFronteira() {
    _pedido.ListaFronteiras.list = new Array();

    var fronteiras = new Array();

    $.each(_pedido.Fronteiras.basicTable.BuscarRegistros(), function (i, fronteira) {
        fronteiras.push({ Fronteira: fronteira });
    });

    _pedido.ListaFronteiras.val(JSON.stringify(fronteiras));
}

function contemFronteiras() {
    return _pedido.Fronteiras.basicTable.BuscarRegistros().length > 0 ? true : false;
} 