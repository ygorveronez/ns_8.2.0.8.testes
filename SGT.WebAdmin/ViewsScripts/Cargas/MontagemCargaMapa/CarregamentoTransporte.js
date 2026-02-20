/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="Bloco.js" />
/// <reference path="Carregamento.js" />
/// <reference path="CarregamentoPedido.js" />
/// <reference path="Carregamentos.js" />
/// <reference path="DirecoesGoogleMaps.js" />
/// <reference path="Distancia.js" />
/// <reference path="GoogleMaps.js" />
/// <reference path="MontagemCarga.js" />
/// <reference path="Pedido.js" />
/// <reference path="PedidoProduto.js" />
/// <reference path="PedidosMapa.js" />
/// <reference path="Roteirizador.js" />
/// <reference path="SimulacaoFrete.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _carregamentoTransporte;
var _gridMotoristas;
var _gridAjudantes;

var CarregamentoTransporte = function () {
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable((_CONFIGURACAO_TMS.TransportadorObrigatorioMontagemCarga ? "*" : "") + Localization.Resources.Cargas.MontagemCargaMapa.Transportador.getFieldDescription()), idBtnSearch: guid(), enable: ko.observable(true), required: _CONFIGURACAO_TMS.TransportadorObrigatorioMontagemCarga });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable(Localization.Resources.Cargas.MontagemCargaMapa.Veiculo.getFieldDescription()), idBtnSearch: guid(), issue: 143, enable: ko.observable(true), visible: ko.observable(true) });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: (_CONFIGURACAO_TMS.TipoOperacaoObrigatorioMontagemCarga ? "*" : "") + Localization.Resources.Cargas.MontagemCargaMapa.TipoDeOperacao.getFieldDescription(), idBtnSearch: guid(), enable: ko.observable(true), required: _CONFIGURACAO_TMS.TipoOperacaoObrigatorioMontagemCarga });
    this.TipoDeCarga = PropertyEntity({ type: types.entity, codEntity: _carregamento.TipoDeCarga.codEntity, val: _carregamento.TipoDeCarga.val, entityDescription: _carregamento.TipoDeCarga.entityDescription, text: (_CONFIGURACAO_TMS.TipoCargaObrigatorioMontagemCarga ? "*" : "") + Localization.Resources.Cargas.MontagemCargaMapa.TipoDeCarga.getFieldDescription(), idBtnSearch: guid(), enable: ko.observable(true), required: _CONFIGURACAO_TMS.TipoCargaObrigatorioMontagemCarga, eventChange: tipoCargaCarregamentoTransportadorBlur, visible: !_CONFIGURACAO_TMS.ExibirTipoDeCargaNaAbaCarregamentoNaMontagemCarga });
    this.Recebedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable(Localization.Resources.Cargas.MontagemCargaMapa.Recebedor.getFieldDescription()), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(false) });
    this.Expedidor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.MontagemCargaMapa.Expedidor, idBtnSearch: guid(), issue: 0, visible: ko.observable(true), required: ko.observable(false) });
    this.Fronteira = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Cargas.MontagemCargaMapa.Fronteira.getFieldDescription()), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true), required: false });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.MontagemCargaMapa.Motorista.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(_CONFIGURACAO_TMS.DesativarMultiplosMotoristasMontagemCarga) });
    this.Motoristas = PropertyEntity({ type: types.map, required: false, text: Localization.Resources.Cargas.MontagemCargaMapa.InformarMotorista, getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), enable: ko.observable(true), issue: 145, visible: ko.observable(!_CONFIGURACAO_TMS.DesativarMultiplosMotoristasMontagemCarga) });
    this.ListaMotoristas = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.Ajudantes = PropertyEntity({ type: types.map, required: false, text: Localization.Resources.Cargas.MontagemCargaMapa.InformarAjudante, getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), enable: ko.observable(true), issue: 145, visible: ko.observable(false) });
    this.ListaAjudantes = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.RaizCNPJEmpresa = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.Raiz.getFieldDescription(), val: ko.observable("") });
    this.Apolice = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCargaMapa.Apolice.getFieldDescription()), type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(false), required: ko.observable(false) });
    this.RotaFrete = PropertyEntity({ required: ko.observable(false), text: ko.observable(Localization.Resources.Cargas.MontagemCargaMapa.Rota.getFieldDescription()), type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(false) });
    this.VisualizarRotaFreteMapa = PropertyEntity({ eventClick: VisualizarRotaFreteMapaClick, type: types.event, text: ko.observable(Localization.Resources.Cargas.MontagemCargaMapa.VisualizarRota), visible: ko.observable(false) });

    this.PermitirVeiculoDiferente = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool });
    this.NaoExigeRoteirizacaoMontagemCarga = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool });

    this.Recebedor.codEntity.subscribe(function (novoValor) {
        _carregamento.Recebedor.codEntity(_carregamentoTransporte.Recebedor.codEntity());
        _carregamento.Recebedor.val(_carregamentoTransporte.Recebedor.val());
    });

    this.TipoOperacao.codEntity.subscribe(function (novoValor) {
        if (novoValor == 0)
            _carregamentoTransporte.PermitirVeiculoDiferente.val(false);
    });

    this.Expedidor.codEntity.subscribe(function (novoValor) {
        _carregamento.Expedidor.codEntity(_carregamentoTransporte.Expedidor.codEntity());
        _carregamento.Expedidor.val(_carregamentoTransporte.Expedidor.val());
    });
};

//*******EVENTOS*******

function loadCarregamentoTransporte() {
    _carregamentoTransporte = new CarregamentoTransporte();
    KoBindings(_carregamentoTransporte, "knoutTransporte");

    GridMotoristas();
    GridAjudantes();
    
    BuscarTransportadores(_carregamentoTransporte.Empresa, null, null, null, null, _configuracoesMontagemCarga.UtilizarFiliaisHabilitadasTransportarMontagemCargaMapa ?  _pesquisaMontegemCarga.Filial : null, null, null, null, null, null, null, null, null, null, null, null, true);

    if (_CONFIGURACAO_TMS.ForcarFiltroModeloNaConsultaVeiculo)
        BuscarVeiculos(_carregamentoTransporte.Veiculo, RetornoVeiculo, _carregamentoTransporte.Empresa, _carregamento.ModeloVeicularCarga, null, null, null, null, null, null, null, null, null, null, null, null, null, true);
    else
        BuscarVeiculos(_carregamentoTransporte.Veiculo, RetornoVeiculo, _carregamentoTransporte.Empresa, _carregamento.ModeloVeicularCarga);

    BuscarTiposOperacao(_carregamentoTransporte.TipoOperacao, callbackTipoOperacao);
    BuscarTiposdeCarga(_carregamentoTransporte.TipoDeCarga, retornoTipoCargaCarregamentoTransportador);
    BuscarClientes(_carregamentoTransporte.Fronteira, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, true);
    BuscarClientes(_carregamentoTransporte.Recebedor, salvarDadosRecebedor);
    new BuscarClientes(_carregamentoTransporte.Expedidor);
    BuscarMotoristas(_carregamentoTransporte.Motorista, null, _carregamentoTransporte.Empresa, null, true);
    BuscarMotoristas(_carregamentoTransporte.Motoristas, RetornoInserirMotorista, _carregamentoTransporte.Empresa, _gridMotoristas, true);
    BuscarMotoristas(_carregamentoTransporte.Ajudantes, RetornoInserirAjudante, _carregamentoTransporte.Empresa, _gridAjudantes, true, null, null, null, null, false);
    BuscarRotasFrete(_carregamentoTransporte.RotaFrete, null, null, null, null, null, null, PEDIDOS_SELECIONADOS);
    
    if (_CONFIGURACAO_TMS.InformaApoliceSeguroMontagemCarga) {
        BuscarApolicesSeguro(_carregamentoTransporte.Apolice, null, null, null, null, true, _carregamentoTransporte.Empresa, true);
        _carregamentoTransporte.Apolice.visible(true);
        _carregamentoTransporte.Apolice.required(true);
        _carregamentoTransporte.Apolice.text(Localization.Resources.Cargas.MontagemCargaMapa.Apolice.getRequiredFieldDescription());
        _carregamentoTransporte.Veiculo.visible(false);
        _carregamentoTransporte.Motoristas.visible(false);
        _carregamentoTransporte.Motorista.visible(false);

        if (!_carregamento.InformarPeriodoCarregamento.val()) {
            _carregamento.DataDescarregamento.visible(true);
            _carregamento.DataDescarregamento.required(true);
        }
    }

    if (_CONFIGURACAO_TMS.PermiteSelecionarRotaMontagemCarga) {
        _carregamentoTransporte.RotaFrete.visible(true);
    }

    if (_CONFIGURACAO_TMS.TipoMontagemCargaPadrao === EnumTipoMontagemCarga.AgruparCargas) {
        _carregamentoTransporte.Empresa.required = true;
        _carregamentoTransporte.Veiculo.required = true;
        _carregamentoTransporte.Empresa.text(Localization.Resources.Cargas.MontagemCargaMapa.Transportador.getRequiredFieldDescription());
        _carregamentoTransporte.Veiculo.text(Localization.Resources.Cargas.MontagemCargaMapa.Veiculo.getRequiredFieldDescription());
    }
}

function GridMotoristas() {
    var excluir = {
        descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), evento: "onclick", metodo: function (data) {
            RemoverMotoristaClick(_carregamentoTransporte.Motoristas, data);
        }, tamanho: "15", icone: ""
    };
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [excluir]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "CPF", title: Localization.Resources.Cargas.MontagemCargaMapa.CPF, width: "20%", className: "text-align-left" },
        { data: "Nome", title: Localization.Resources.Cargas.MontagemCargaMapa.Nome, width: "60%", className: "text-align-left" }
    ];

    _gridMotoristas = new BasicDataTable(_carregamentoTransporte.Motoristas.idGrid, header, menuOpcoes);
    _carregamentoTransporte.Motoristas.basicTable = _gridMotoristas;
    RecarregarListaMotoristas();
}

function GridAjudantes() {
    var excluir = {
        descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), evento: "onclick", metodo: function (data) {
            RemoverAjudanteClick(_carregamentoTransporte.Ajudantes, data);
        }, tamanho: "15", icone: ""
    };
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [excluir]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "CPF", title: Localization.Resources.Cargas.MontagemCargaMapa.CPF, width: "20%", className: "text-align-left" },
        { data: "Nome", title: Localization.Resources.Cargas.MontagemCargaMapa.Nome, width: "60%", className: "text-align-left" }
    ];

    _gridAjudantes = new BasicDataTable(_carregamentoTransporte.Ajudantes.idGrid, header, menuOpcoes);
    _carregamentoTransporte.Ajudantes.basicTable = _gridAjudantes;
    RecarregarListaAjudantes();
}

function RetornoVeiculo(data) {
    if (data != null) {
        if (_carregamento.ModeloVeicularCarga.codEntity() > 0 && _carregamento.ModeloVeicularCarga.codEntity() != data.CodigoModeloVeicularCarga && !_carregamentoTransporte.PermitirVeiculoDiferente.val()) {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Cargas.MontagemCargaMapa.VeiculoDiferente, Localization.Resources.Cargas.MontagemCargaMapa.AtencaoTipoDoVeiculoDiferenteDoModeloEscolhidoParaCarregamento.format(_carregamento.ModeloVeicularCarga.val()));
        }

        executarReST("Veiculo/ObterDetalhesVeiculo", { Codigo: data.Codigo }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    var dados = r.Data;
                    _carregamentoTransporte.Veiculo.codEntity(dados.Codigo);
                    if (dados.Reboque != "")
                        _carregamentoTransporte.Veiculo.val(dados.Placa + " (" + dados.Reboque + ")");
                    else if (dados.Tracao != "" && dados.CodigoTracao > 0) {
                        _carregamentoTransporte.Veiculo.val(dados.Tracao + " (" + dados.Placa + ")");
                        _carregamentoTransporte.Veiculo.codEntity(dados.CodigoTracao);
                    }
                    else
                        _carregamentoTransporte.Veiculo.val(dados.Placa);

                    // Limpa motoristas antes de inserir automatico
                    _gridMotoristas.CarregarGrid([]);
                    _carregamentoTransporte.Motorista.codEntity(0);
                    _carregamentoTransporte.Motorista.val('');
                    _carregamentoTransporte.Motorista.entityDescription('');

                    var motoristas = new Array();

                    if (dados.Motoristas.length > 0) {
                        $.each(dados.Motoristas, function (i, motorista) {
                            var obj = new Object();
                            obj.Codigo = motorista.CodigoMotorista;
                            obj.CPF = motorista.CPF;
                            obj.Nome = motorista.Nome;
                            motoristas.push(obj);
                        });
                    }
                    else if (dados.MotoristasTracao.length > 0) {
                        $.each(dados.MotoristasTracao, function (i, motorista) {
                            var obj = new Object();
                            obj.Codigo = motorista.CodigoMotorista;
                            obj.CPF = motorista.CPF;
                            obj.Nome = motorista.Nome;
                            motoristas.push(obj);
                        });
                    }

                    if (motoristas.length > 0) {
                        var dataGrid = _gridMotoristas.BuscarRegistros();

                        $.each(motoristas, function (i, motorista) {
                            dataGrid.push(motorista);

                            if (i == 0) {
                                if (_CONFIGURACAO_TMS.DesativarMultiplosMotoristasMontagemCarga) {
                                    _carregamentoTransporte.Motorista.codEntity(motorista.Codigo);
                                    _carregamentoTransporte.Motorista.val(motorista.Nome);
                                    _carregamentoTransporte.Motorista.entityDescription(motorista.Nome);
                                }
                            }
                        });

                        _gridMotoristas.CarregarGrid(dataGrid);
                    }

                    // Limpa ajudantes antes de inserir automatico
                    _gridAjudantes.CarregarGrid([]);

                    var ajudantes = new Array();

                    $.each(dados.Ajudantes, function (i, motorista) {
                        var obj = new Object();
                        obj.Codigo = motorista.CodigoMotorista;
                        obj.CPF = motorista.CPF;
                        obj.Nome = motorista.Nome;
                        ajudantes.push(obj);
                    });

                    if (ajudantes.length > 0) {
                        var dataGrid = _gridAjudantes.BuscarRegistros();

                        $.each(ajudantes, function (i, ajudante) {
                            dataGrid.push(ajudante);
                        });

                        _gridAjudantes.CarregarGrid(dataGrid);
                    }
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    }
}

function RemoverMotoristaClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.MontagemCargaMapa.RealmenteDesejaExcluirMotorista.format(sender.Nome), function () {
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

function RemoverAjudanteClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.MontagemCargaMapa.RealmenteDesejaExcluirAjudante.format(sender.Nome), function () {
        var ajudanteGrid = e.basicTable.BuscarRegistros();

        for (var i = 0; i < ajudanteGrid.length; i++) {
            if (sender.Codigo == ajudanteGrid[i].Codigo) {
                ajudanteGrid.splice(i, 1);
                break;
            }
        }
        e.basicTable.CarregarGrid(ajudanteGrid);
    });
}

function RetornoInserirMotorista(data) {
    if (data != null) {
        var dataGrid = _gridMotoristas.BuscarRegistros();

        if (_carregamentoTransporte.Veiculo.codEntity() == "" || _carregamentoTransporte.Veiculo.codEntity() == 0) {
            if (data[0].Veiculo != "" && data[0].CodigoVeiculo > 0) {
                _carregamentoTransporte.Veiculo.val(data[0].Veiculo);
                _carregamentoTransporte.Veiculo.codEntity(data[0].CodigoVeiculo);
            }
        }

        for (var i = 0; i < data.length; i++) {
            var obj = new Object();
            obj.Codigo = data[i].Codigo;
            obj.CPF = data[i].CPF;
            obj.Nome = data[i].Nome;

            dataGrid.push(obj);
        }
        _gridMotoristas.CarregarGrid(dataGrid);
    }
}

function RetornoInserirAjudante(data) {
    if (data != null) {
        var dataGrid = _gridAjudantes.BuscarRegistros();

        if (_carregamentoTransporte.Veiculo.codEntity() == "" || _carregamentoTransporte.Veiculo.codEntity() == 0) {
            if (data[0].Veiculo != "" && data[0].CodigoVeiculo > 0) {
                _carregamentoTransporte.Veiculo.val(data[0].Veiculo);
                _carregamentoTransporte.Veiculo.codEntity(data[0].CodigoVeiculo);
            }
        }

        for (var i = 0; i < data.length; i++) {
            var obj = new Object();
            obj.Codigo = data[i].Codigo;
            obj.CPF = data[i].CPF;
            obj.Nome = data[i].Nome;

            dataGrid.push(obj);
        }
        _gridAjudantes.CarregarGrid(dataGrid);
    }
}

function RecarregarListaMotoristas() {
    var cont = 0;
    var total = 0;
    var data = new Array();

    if (!string.IsNullOrWhiteSpace(_carregamentoTransporte.ListaMotoristas.val())) {
        $.each(_carregamentoTransporte.ListaMotoristas.val(), function (i, motorista) {
            var obj = new Object();

            obj.Codigo = motorista.Codigo;
            obj.CPF = motorista.CPF;
            obj.Nome = motorista.Nome;

            data.push(obj);
        });
    }

    _gridMotoristas.CarregarGrid(data);
}

function RecarregarListaAjudantes() {
    var cont = 0;
    var total = 0;
    var data = new Array();

    if (!string.IsNullOrWhiteSpace(_carregamentoTransporte.ListaAjudantes.val())) {
        $.each(_carregamentoTransporte.ListaAjudantes.val(), function (i, motorista) {
            var obj = new Object();

            obj.Codigo = motorista.Codigo;
            obj.CPF = motorista.CPF;
            obj.Nome = motorista.Nome;

            data.push(obj);
        });
    }

    _gridAjudantes.CarregarGrid(data);
}

function preencherListaMotorista() {
    _carregamentoTransporte.ListaMotoristas.list = new Array();

    var motoristas = new Array();

    $.each(_carregamentoTransporte.Motoristas.basicTable.BuscarRegistros(), function (i, motorista) {
        motoristas.push({ Motorista: motorista });
    });

    _carregamentoTransporte.ListaMotoristas.val(JSON.stringify(motoristas));
}

function preencherListaAjudante() {
    _carregamentoTransporte.ListaAjudantes.list = new Array();

    var ajudantes = new Array();
    
    $.each(_carregamentoTransporte.Ajudantes.basicTable.BuscarRegistros(), function (i, ajudante) {
        ajudantes.push({ Ajudante: ajudante });
    });
    
    _carregamentoTransporte.ListaAjudantes.val(JSON.stringify(ajudantes));
}

function preencherDadosTransporte(dadosTransporte) {
    PreencherObjetoKnout(_carregamentoTransporte, { Data: dadosTransporte });

    _carregamentoTransporte.TipoOperacao.NaoExigeRoteirizacaoMontagemCarga = dadosTransporte.TipoOperacao.NaoExigeRoteirizacaoMontagemCarga;
    _carregamentoTransporte.TipoDeCarga.Paletizado = dadosTransporte.TipoDeCarga.Paletizado;
    _carregamentoTransporte.Ajudantes.visible(dadosTransporte.TipoOperacao.PermitirInformarAjudantesNaCarga);
    //_carregamentoTransporte.Fronteira.Latitude = dadosTransporte.Fronteira.Latitude;
    //_carregamentoTransporte.Fronteira.Longitude = dadosTransporte.Fronteira.Longitude;
    RecarregarListaMotoristas();
    RecarregarListaAjudantes();
}

function limparCarregamentoTransporte() {

    LimparCampos(_carregamentoTransporte);
    _carregamentoTransporte.ListaMotoristas.list = new Array();
    _carregamentoTransporte.ListaAjudantes.list = new Array();
    _carregamentoTransporte.Empresa.enable(true);
    _carregamentoTransporte.Fronteira.enable(true);
    _carregamentoTransporte.Fronteira.visible(false);
    _carregamentoTransporte.Fronteira.required = false;
    _carregamentoTransporte.Veiculo.enable(true);
    _carregamentoTransporte.TipoOperacao.enable(true);

    RecarregarListaMotoristas();
    RecarregarListaAjudantes();
    LimparSimulacaoFrete();
}

function retornoTipoCargaCarregamentoTransportador(registroSelecionado) {
    _carregamentoTransporte.TipoDeCarga.codEntity(registroSelecionado.Codigo);
    _carregamentoTransporte.TipoDeCarga.entityDescription(registroSelecionado.Descricao);
    _carregamentoTransporte.TipoDeCarga.val(registroSelecionado.Descricao);
    _carregamentoTransporte.TipoDeCarga.Paletizado = registroSelecionado.Paletizado;

    obterPesosEAjustarCapacidade();
    buscarCapacidadeJanelaCarregamento(buscarPeriodoCarregamento);
}

function tipoCargaCarregamentoTransportadorBlur() {
    if (_carregamentoTransporte.TipoDeCarga.val() == "") {
        _carregamentoTransporte.TipoDeCarga.Paletizado = false;

        obterPesosEAjustarCapacidade();
        buscarCapacidadeJanelaCarregamento(buscarPeriodoCarregamento);
    }
}

function setarTransportadorCarregamentoTransporte(pedido) {
    if (_carregamentoTransporte.Empresa.val() == "") {
        _carregamentoTransporte.Empresa.codEntity(pedido.CodigoEmpresa);
        _carregamentoTransporte.Empresa.entityDescription(pedido.DescricaoEmpresa);
        _carregamentoTransporte.Empresa.val(pedido.DescricaoEmpresa);
    }
}

function setarTipoOperacaoColetaTransporte(pedido) {
    if ((_gerarCargasDeColeta) && (_carregamentoTransporte.TipoOperacao.val() == "")) {
        _carregamentoTransporte.TipoOperacao.codEntity(pedido.CodigoTipoOperacaoColeta);
        _carregamentoTransporte.TipoOperacao.entityDescription(pedido.DescricaoTipoOperacaoColeta);
        _carregamentoTransporte.TipoOperacao.val(pedido.DescricaoTipoOperacaoColeta);
        _carregamentoTransporte.Recebedor.visible(pedido.AtivarRecebedor);
    }
}

function ExibirModalMapaRotaFrete() {
    Global.abrirModal('divModalMapaMotagemCarga');
    $("#divModalMapaMotagemCarga").one('hidden.bs.modal', function () {
    });
}

function VisualizarRotaFreteMapaClick() {
    var data = { rota: _carregamentoTransporte.RotaFrete.codEntity() };

    executarReST("MontagemCarga/ObterDadosRotaFrete", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {

                ExibirModalMapaRotaFrete();

                if (_mapaRotaMontagemCarga == null)
                    _mapaRotaMontagemCarga = new Mapa("mapaRotaMotagemCarga", false);

                _mapaRotaMontagemCarga.limparMapa();

                setTimeout(function () {
                    _mapaRotaMontagemCarga.desenharPolilinha(arg.Data.PolilinhaRota, true);
                    _mapaRotaMontagemCarga.adicionarMarcadorComPontosDaRota(arg.Data.PontosDaRota);

                }, 500);


            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function callbackTipoOperacao(data) {
    if (_CONFIGURACAO_TMS.PermiteSelecionarRotaMontagemCarga) {
        if (data.ObrigarRotaNaMontagemDeCarga) {
            _carregamentoTransporte.RotaFrete.required(true);
            _carregamentoTransporte.RotaFrete.text(Localization.Resources.Cargas.MontagemCargaMapa.Rota.getRequiredFieldDescription());
        }
        else {
            _carregamentoTransporte.RotaFrete.required(false);
            _carregamentoTransporte.RotaFrete.text(Localization.Resources.Cargas.MontagemCargaMapa.Rota);
        }
    }
    
    _carregamentoTransporte.PermitirVeiculoDiferente.val(data.PermitirVeiculoDiferenteMontagemCarga);
    _carregamentoTransporte.NaoExigeRoteirizacaoMontagemCarga.val(data.NaoExigeRoteirizacaoMontagemCarga);
    _carregamentoTransporte.Ajudantes.visible(data.PermitirInformarAjudantesNaCarga);
    _carregamentoTransporte.TipoOperacao.codEntity(data.Codigo);
    _carregamentoTransporte.TipoOperacao.entityDescription(data.Descricao);
    _carregamentoTransporte.TipoOperacao.val(data.Descricao);

    recebedorVisivelTransporte(data.PermitirInformarRecebedorMontagemCarga);
}

function recebedorVisivelTransporte(permitirInformarRecebedorMontagemCarga) {
    _carregamentoTransporte.Recebedor.visible(permitirInformarRecebedorMontagemCarga);
    
}
function salvarDadosRecebedor(recebedor) {
    const pedidos = PEDIDOS_SELECIONADOS();

    for (let i = 0; i < pedidos.length; i++) {
        pedidos[i].Recebedor = recebedor.Descricao;
        pedidos[i].CodRecebedor = recebedor.Codigo;
    }
    _carregamentoTransporte.Recebedor.val(recebedor.Descricao)
    _carregamentoTransporte.Recebedor.codEntity(recebedor.Codigo)

    RenderizarGridMotagemPedidos();
}