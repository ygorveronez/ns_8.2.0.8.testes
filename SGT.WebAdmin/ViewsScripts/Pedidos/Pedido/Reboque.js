function LoadGridReboques() {
    if (_CONFIGURACAO_TMS.PermitirSelecionarReboquePedido === true)
        $("#divReboques").show();
    else
        $("#divReboques").hide();

    var excluir = {
        descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), evento: "onclick", metodo: function (data) {
            RemoverReboqueClick(_pedido.Reboques, data);
        }, tamanho: "15", icone: ""
    };

    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [excluir] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Placa", title: Localization.Resources.Pedidos.Pedido.Placa, width: "20%", className: "text-align-left" },
        { data: "ModeloVeicular", title: Localization.Resources.Pedidos.Pedido.ModeloVeicular, width: "60%", className: "text-align-left" }
    ];

    _gridReboques = new BasicDataTable(_pedido.Reboques.idGrid, header, menuOpcoes);
    _pedido.Reboques.basicTable = _gridReboques;

    //Verifica se for TSM, se o usuário tiver permissão para incluir veiculos próprios, terceiros ou ambos
    var tipoPropriedade = "";
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
        //Caso seja TMS, vai definir valor padrão como N, desta forma, não trará nenhum veículo se o usuário não tiver nenhuma das permissões
        tipoPropriedade = "N";
        if (_CONFIGURACAO_TMS.UsuarioAdministrador || VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Pedido_PermiteInserirVeiculoProprio, _PermissoesPersonalizadas))
            tipoPropriedade = "P";
        if (_CONFIGURACAO_TMS.UsuarioAdministrador || VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Pedido_PermiteInserirVeiculoTerceiro, _PermissoesPersonalizadas))
            tipoPropriedade = tipoPropriedade == "N" ? "T" : "A";
    }
    new BuscarVeiculos(_pedido.Reboques, RetornoInserirReboque, null, null, null, null, null, null, null, null, null, null, null, _gridReboques, null, null, null, null, null, "1", null, null, null, null, tipoPropriedade);

    RecarregarListaReboques();
}

function RecarregarListaReboques() {
    var data = new Array();
    var listaReboques = _pedido.ListaReboques.val() || [];

    if (!string.IsNullOrWhiteSpace(listaReboques)) {
        $.each(listaReboques, function (i, reboque) {
            var obj = new Object();

            obj.Codigo = reboque.Codigo;
            obj.Placa = reboque.Placa + String(!string.IsNullOrWhiteSpace(reboque.NumeroFrota) ?  " (" + reboque.NumeroFrota + ") " : "");
            obj.ModeloVeicular = reboque.ModeloVeicular;

            data.push(obj);
        });
    }

    _gridReboques.CarregarGrid(data);
}

function PreencherListaReboques() {
    _pedido.ListaReboques.val(JSON.stringify(_pedido.Reboques.basicTable.BuscarRegistros().map(o => o.Codigo)));
}

function RemoverReboqueClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Pedidos.Pedido.RealmenteDesejaExlcuirOReboque.format(sender.Placa), function () {
        var reboquesGrid = e.basicTable.BuscarRegistros();

        for (var i = 0; i < reboquesGrid.length; i++) {
            if (sender.Codigo == reboquesGrid[i].Codigo) {
                reboquesGrid.splice(i, 1);
                break;
            }
        }
        e.basicTable.CarregarGrid(reboquesGrid);
    });
}

function RetornoInserirReboque(data) {
    if (data != null) {
        var dataGrid = _gridReboques.BuscarRegistros();

        dataGrid = [].concat(dataGrid, data.map(function (o) { return { Codigo: o.Codigo, Placa: o.Placa + String(!string.IsNullOrWhiteSpace(o.NumeroFrota) ? " (" + o.NumeroFrota + ") " : ""), ModeloVeicular: o.ModeloVeicularCarga } }));

        _gridReboques.CarregarGrid(dataGrid);
    }
}