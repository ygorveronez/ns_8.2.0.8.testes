var _mensagemMotoristaSituacao; 

function LoadGridMotoristas() {
    var excluir = {
        descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), evento: "onclick", metodo: function (data) {
            RemoverMotoristaClick(_pedido.Motoristas, data);
        }, tamanho: "15", icone: ""
    };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(excluir);

    var header = [
        { data: "Codigo", visible: false },
        { data: "CPF", title: Localization.Resources.Pedidos.Cpf, width: "20%", className: "text-align-left" },
        { data: "Nome", title: Localization.Resources.Gerais.Geral.Nome, width: "60%", className: "text-align-left" }
    ];

    _gridMotoristas = new BasicDataTable(_pedido.Motoristas.idGrid, header, menuOpcoes);
    _pedido.Motoristas.basicTable = _gridMotoristas;

    new BuscarMotorista(_pedido.Motoristas, RetornoInserirMotorista, _gridMotoristas, null, EnumSituacaoColaborador.Trabalhando);
    RecarregarListaMotoristas();
}

function RemoverMotoristaClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Pedidos.Pedido.RealmenteDesejaExcluirMotorista.format(sender.Nome), function () {
        var motoristaGrid = e.basicTable.BuscarRegistros();

        for (var i = 0; i < motoristaGrid.length; i++) {
            if (sender.Codigo == motoristaGrid[i].Codigo) {
                motoristaGrid.splice(i, 1);
                break;
            }
        }
        e.basicTable.CarregarGrid(motoristaGrid);
    });
}

function RetornoInserirMotorista(data) {
    if (data != null) {
        var dataGrid = _gridMotoristas.BuscarRegistros();

        if (_pedido.Veiculo.codEntity() == "" || _pedido.Veiculo.codEntity() == 0) {
            if (data[0].Veiculo != "" && data[0].CodigoVeiculo > 0) {
                _pedido.Veiculo.val(data[0].Veiculo);
                _pedido.Veiculo.codEntity(data[0].CodigoVeiculo);
            }
        }

        dataGrid = [].concat(dataGrid, data);

        _gridMotoristas.CarregarGrid(dataGrid);

        if (_CONFIGURACAO_TMS.AcoplarMotoristaAoVeiculoAoSelecionarNaCarga && dataGrid.length == 1 && _pedido.CodigoMotoristaVeiculo.val() != dataGrid[0].Codigo) {
            var motorista = dataGrid[0];

            exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Pedidos.Pedido.IdentificamosQueMotoristaVinculadoVeiculo.format(_pedido.NomeMotoristaVeiculo.val(), motorista.Nome, _pedido.Veiculo.val()) , function () {
                executarReST("Veiculo/VincularMotorista", { Veiculo: _pedido.Veiculo.codEntity(), Motorista: motorista.Codigo }, function (retorno) {
                    if (retorno.Success) {
                        if (retorno.Data) {
                            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Pedidos.Pedido.VinculadoVeiculoComSucesso.format(motorista.Nome, _pedido.Veiculo.val()));
            
                            _pedido.NomeMotoristaVeiculo.val(motorista.Nome);
                            _pedido.CodigoMotoristaVeiculo.val(motorista.Codigo);
                        }
                        else
                            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
                    }
                    else
                        exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
                });
            });
        }
        if (dataGrid != null)
        {
            _mensagemMotoristaSituacao = "";
            let motoristasituacao = dataGrid[0];
            ValidarSituacao(motoristasituacao).then(function () {
                if (_mensagemMotoristaSituacao != "") {
                    exibirAlerta("Atenção", _mensagemMotoristaSituacao, "Ok"); 
                }
            });
        }
    }
}

function RecarregarListaMotoristas() {
    var data = new Array();
    if (!string.IsNullOrWhiteSpace(_pedido.ListaMotoristas.val())) {
        $.each(_pedido.ListaMotoristas.val(), function (i, motorista) {
            var obj = new Object();

            obj.Codigo = motorista.Codigo;
            obj.CPF = motorista.CPF;
            obj.Nome = motorista.Nome;

            data.push(obj);
        });
    }
    _gridMotoristas.CarregarGrid(data);
}

function preencherListaMotorista() {
    _pedido.ListaMotoristas.list = new Array();

    var motoristas = new Array();

    $.each(_pedido.Motoristas.basicTable.BuscarRegistros(), function (i, motorista) {
        motoristas.push({ Motorista: motorista });
    });

    _pedido.ListaMotoristas.val(JSON.stringify(motoristas));
}

function contemMotoristas() {
    return _pedido.Motoristas.basicTable.BuscarRegistros().length > 0 ? true : false;
}

function ValidarSituacao(e) {
    let p = new promise.Promise();
    let data = { Codigo: e.Codigo }

    executarReST("Motorista/ValidarMotoristaSituacao", data, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                if (arg.Data.ExibirConfirmacaoMotoristaSituacao) {
                    _mensagemMotoristaSituacao = arg.Msg;
                } else
                    _mensagemMotoristaSituacao = "";
            } else {
                _mensagemMotoristaSituacao = "";
            }
        } else {
            _mensagemMotoristaSituacao = "";
        }

        p.done();
    });

    return p;
}