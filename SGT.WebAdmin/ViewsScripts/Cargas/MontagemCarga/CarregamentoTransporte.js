/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/ApoliceSeguro.js" />
/// <reference path="../../Consultas/Fronteira.js" />
/// <reference path="../../Consultas/GrupoTransportador.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/RotaFrete.js" />
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="Bloco.js" />
/// <reference path="Carga.js" />
/// <reference path="Carregamento.js" />
/// <reference path="CarregamentoCarga.js" />
/// <reference path="CarregamentoPedido.js" />
/// <reference path="Carregamentos.js" />
/// <reference path="DirecoesGoogleMaps.js" />
/// <reference path="Distancia.js" />
/// <reference path="GoogleMaps.js" />
/// <reference path="MontagemCarga.js" />
/// <reference path="OrigemDestino.js" />
/// <reference path="Pedido.js" />
/// <reference path="PedidoProduto.js" />
/// <reference path="PedidosMapa.js" />
/// <reference path="Roteirizador.js" />
/// <reference path="SimulacaoFrete.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _carregamentoTransporte;
var _pesquisaTransporteVeiculo;
var _gridMotoristas;
var _mapaRotaMontagemCarga;
var _isFiltrarTipoOperacaoPorTransportador = true;

var CarregamentoTransporte = function () {
    var self = this;

    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable((_CONFIGURACAO_TMS.TransportadorObrigatorioMontagemCarga ? "*" : "") + Localization.Resources.Cargas.MontagemCarga.Transportador), idBtnSearch: guid(), enable: ko.observable(true), required: ko.observable(_CONFIGURACAO_TMS.TransportadorObrigatorioMontagemCarga), eventChange: carregamentoTransporteEmpresaBlur });
    this.GrupoTransportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Cargas.MontagemCarga.GrupoTransportador), idBtnSearch: guid(), enable: ko.observable(true), required: ko.observable(false), eventChange: carregamentoTransporteGrupoTransportadorBlur, visible: _CONFIGURACAO_TMS.PermitirInformarGrupoTransportador });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: _CONFIGURACAO_TMS.VeiculoObrigatorioMontagemCarga, text: ko.observable((_CONFIGURACAO_TMS.VeiculoObrigatorioMontagemCarga ? "*" : "") + Localization.Resources.Cargas.MontagemCarga.Veiculo), idBtnSearch: guid(), issue: 143, enable: ko.observable(true), visible: ko.observable(true) });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: (_CONFIGURACAO_TMS.TipoOperacaoObrigatorioMontagemCarga ? "*" : "") + Localization.Resources.Cargas.MontagemCarga.TipoOperacao, idBtnSearch: guid(), enable: ko.observable(true), required: _CONFIGURACAO_TMS.TipoOperacaoObrigatorioMontagemCarga });
    this.Recebedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.MontagemCarga.Recebedor, idBtnSearch: guid(), issue: 0, visible: ko.observable(false), required: ko.observable(false) });
    this.Expedidor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.MontagemCarga.Expedidor, idBtnSearch: guid(), issue: 0, visible: ko.observable(true), required: ko.observable(false) });
    this.Fronteira = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Cargas.MontagemCarga.Fronteiras), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true), required: ko.observable(false) });
    this.Motorista = PropertyEntity({ type: types.entity, required: (_CONFIGURACAO_TMS.MotoristaObrigatorioMontagemCarga && _CONFIGURACAO_TMS.DesativarMultiplosMotoristasMontagemCarga), codEntity: ko.observable(0), text: ko.observable((_CONFIGURACAO_TMS.MotoristaObrigatorioMontagemCarga ? "*" : "") + Localization.Resources.Cargas.MontagemCarga.Motorista), idBtnSearch: guid(), visible: ko.observable(_CONFIGURACAO_TMS.DesativarMultiplosMotoristasMontagemCarga) });
    this.Motoristas = PropertyEntity({ type: types.map, required: false, text: Localization.Resources.Cargas.MontagemCarga.Motoristas, getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), enable: ko.observable(true), issue: 145, visible: ko.observable(!_CONFIGURACAO_TMS.DesativarMultiplosMotoristasMontagemCarga) });
    this.TempoLimiteConfirmacaoMotorista = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCarga.TempoLimiteConfirmacaoMotorista.getRequiredFieldDescription(), getType: typesKnockout.timeSec, required: ko.observable(false), visible: ko.observable(false) });
    this.NecessarioConfirmacaoMotorista = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool });
    this.ListaMotoristas = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.RaizCNPJEmpresa = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCarga.RaizCNPJEmpresa, val: ko.observable("") });
    this.Apolice = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Cargas.MontagemCarga.Apolice), idBtnSearch: guid(), visible: ko.observable(false), required: ko.observable(false) });
    this.RotaFrete = PropertyEntity({ required: ko.observable(false), text: ko.observable(Localization.Resources.Cargas.MontagemCarga.Rota), type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(false) });
    this.TipoDeCarga = PropertyEntity({ type: types.entity, codEntity: _carregamento.TipoDeCarga.codEntity, val: _carregamento.TipoDeCarga.val, entityDescription: _carregamento.TipoDeCarga.entityDescription, text: (_CONFIGURACAO_TMS.TipoCargaObrigatorioMontagemCarga ? "*" : "") + Localization.Resources.Cargas.MontagemCarga.TipoDeCarga, idBtnSearch: guid(), enable: ko.observable(true), required: _CONFIGURACAO_TMS.TipoCargaObrigatorioMontagemCarga, eventChange: tipoCargaCarregamentoTransportadorBlur, visible: !_CONFIGURACAO_TMS.ExibirTipoDeCargaNaAbaCarregamentoNaMontagemCarga });

    this.CodigoMotoristaVeiculo = PropertyEntity();
    this.NomeMotoristaVeiculo = PropertyEntity();
    this.TransportadorLocalCarregamentoRestringido = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool });
    this.CodigoPedidoBase = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), visible: false });

    this.PermitirVeiculoDiferente = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool });

    this.VisualizarRotaFreteMapa = PropertyEntity({ eventClick: VisualizarRotaFreteMapaClick, type: types.event, text: ko.observable(Localization.Resources.Cargas.MontagemCarga.VisualizarRotaFreteMapa), visible: ko.observable(false) });

    this.TipoOperacao.codEntity.subscribe(function (novoValor) {
        if (novoValor == 0)
            _carregamentoTransporte.PermitirVeiculoDiferente.val(false);
    });

    this.Empresa.required.subscribe(function (novoValor) {
        self.Empresa.text((novoValor ? "*" : "") + Localization.Resources.Cargas.MontagemCarga.Transportador);
    });

    this.Apolice.required.subscribe(function (novoValor) {
        self.Apolice.text((novoValor ? "*" : "") + Localization.Resources.Cargas.MontagemCarga.Apolice);
    });

    this.GrupoTransportador.required.subscribe(function (novoValor) {
        self.GrupoTransportador.text((novoValor ? "*" : "") + Localization.Resources.Cargas.MontagemCarga.GrupoTransportador);
    });

    this.Recebedor.codEntity.subscribe(function (novoValor) {
        if (_carregamentoTransporte.Recebedor.visible()) {
            _carregamento.Recebedor.codEntity(_carregamentoTransporte.Recebedor.codEntity());
            _carregamento.Recebedor.val(_carregamentoTransporte.Recebedor.val());
        }
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

    new BuscarTransportadores(_carregamentoTransporte.Empresa, retornoConsultaCarregamentoTransporteEmpresa, null, null, null, null, null, null, null, null, null, _CONFIGURACAO_TMS.Transportador.ExisteTransportadorPadraoContratacao, _carregamentoTransporte.CodigoPedidoBase);
    new BuscarGruposTransportadores(_carregamentoTransporte.GrupoTransportador, retornoConsultaCarregamentoTransporteGrupoTransportador);
    new BuscarTiposOperacao(_carregamentoTransporte.TipoOperacao, callbackTipoOperacao, null, null, null, null, null, null, null, null, _isFiltrarTipoOperacaoPorTransportador, null, null, true);
    new BuscarClientes(_carregamentoTransporte.Fronteira, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, true);
    new BuscarMotoristas(_carregamentoTransporte.Motorista, null, _carregamentoTransporte.Empresa, null, true, EnumSituacaoColaborador.Trabalhando);
    new BuscarMotoristas(_carregamentoTransporte.Motoristas, RetornoInserirMotorista, _carregamentoTransporte.Empresa, _gridMotoristas, true, EnumSituacaoColaborador.Trabalhando, null, ObterPrimeiroRemetenteLista);
    new BuscarRotasFrete(_carregamentoTransporte.RotaFrete, RetornoRotaFrete, null, null, null, null, null, PEDIDOS_SELECIONADOS);
    new BuscarClientes(_carregamentoTransporte.Recebedor);
    new BuscarClientes(_carregamentoTransporte.Expedidor);

    if (_CONFIGURACAO_TMS.ForcarFiltroModeloNaConsultaVeiculo)
        _pesquisaTransporteVeiculo = new BuscarVeiculos(_carregamentoTransporte.Veiculo, RetornoVeiculo, _carregamentoTransporte.Empresa, _carregamento.ModeloVeicularCarga, null, null, null, null, null, null, null, null, null, null, null, null, null, true, null, null, null, null, null, null, null, null, _carregamentoTransporte.TipoOperacao);
    else
        _pesquisaTransporteVeiculo = new BuscarVeiculos(_carregamentoTransporte.Veiculo, RetornoVeiculo, _carregamentoTransporte.Empresa, _carregamento.ModeloVeicularCarga, null, null, null, null, null, null, null, null, null, null, null, null, null, true, null, null, null, null, null, null, null, null, _carregamentoTransporte.TipoOperacao);

    //Tarefa MARFRIG, #9391 vamos filtrar somente os tipos de carga da filial.
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador)
        new BuscarTiposdeCargaPorFilial(_carregamentoTransporte.TipoDeCarga, retornoTipoCargaCarregamentoTransportador, _pesquisaMontegemCarga.Filial, null, null, true);
    else
        new BuscarTiposdeCarga(_carregamentoTransporte.TipoDeCarga, retornoTipoCargaCarregamentoTransportador, null, null, null, null, null, true);

    if (_CONFIGURACAO_TMS.InformaApoliceSeguroMontagemCarga) {
        new BuscarApolicesSeguro(_carregamentoTransporte.Apolice, null, null, null, null, true, _carregamentoTransporte.Empresa, true);

        _carregamentoTransporte.Apolice.visible(true);
        _carregamentoTransporte.Apolice.required(true);
        _carregamentoTransporte.Veiculo.visible(false);
        _carregamentoTransporte.Motoristas.visible(false);
        _carregamentoTransporte.Motorista.visible(false);
    }

    if (_CONFIGURACAO_TMS.PermiteSelecionarRotaMontagemCarga) {
        _carregamentoTransporte.RotaFrete.visible(true);
        //_carregamentoTransporte.VisualizarRotaFreteMapa.visible(true);
    }

    if (_CONFIGURACAO_TMS.TipoMontagemCargaPadrao === EnumTipoMontagemCarga.AgruparCargas) {
        _carregamentoTransporte.Empresa.required = true;
        _carregamentoTransporte.Veiculo.required = true;
        _carregamentoTransporte.Empresa.text(Localization.Resources.Cargas.MontagemCarga.Transportador.getRequiredFieldDescription());
        _carregamentoTransporte.Veiculo.text(Localization.Resources.Cargas.MontagemCarga.Veiculo.getRequiredFieldDescription());
    }
}


function carregamentoTransporteEmpresaBlur() {
    if (_carregamentoTransporte.Empresa.val() == "") {
        _carregamentoTransporte.RaizCNPJEmpresa.val("");

        removerEmpresaDadosCarregamentoPorFilial();
    }
}

function carregamentoTransporteGrupoTransportadorBlur() {
    if (_carregamentoTransporte.GrupoTransportador.val() == "")
        controlarCamposPorGrupoTransportadorInformado();
}

function GridMotoristas() {
    var excluir = {
        descricao: "Remover", id: guid(), evento: "onclick", metodo: function (data) {
            RemoverMotoristaClick(_carregamentoTransporte.Motoristas, data);
        }, tamanho: "15", icone: ""
    };
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [excluir]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "CPF", title: Localization.Resources.Cargas.MontagemCarga.CPF, width: "20%", className: "text-align-left" },
        { data: "Nome", title: Localization.Resources.Cargas.MontagemCarga.Nome, width: "60%", className: "text-align-left" }
    ];

    _gridMotoristas = new BasicDataTable(_carregamentoTransporte.Motoristas.idGrid, header, menuOpcoes);
    _carregamentoTransporte.Motoristas.basicTable = _gridMotoristas;
    RecarregarListaMotoristas();
}

function RetornoVeiculo(data) {
    if (data != null) {
        if (_carregamento.ModeloVeicularCarga.codEntity() > 0 && _carregamento.ModeloVeicularCarga.codEntity() != data.CodigoModeloVeicularCarga && !_carregamentoTransporte.PermitirVeiculoDiferente.val()) {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Cargas.MontagemCarga.VeiculoDiferente, Localization.Resources.Cargas.MontagemCarga.AtencaoTipoDoVeiculoDiferenteDoModeloEscolhidoParaCarregamento.format(_carregamento.ModeloVeicularCarga.val()));
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
                                _carregamentoTransporte.CodigoMotoristaVeiculo.val(motorista.Codigo);
                                _carregamentoTransporte.NomeMotoristaVeiculo.val(motorista.Nome);

                                if (_CONFIGURACAO_TMS.DesativarMultiplosMotoristasMontagemCarga) {
                                    _carregamentoTransporte.Motorista.codEntity(motorista.Codigo);
                                    _carregamentoTransporte.Motorista.val(motorista.Nome);
                                    _carregamentoTransporte.Motorista.entityDescription(motorista.Nome);
                                }
                            }
                        });

                        _gridMotoristas.CarregarGrid(dataGrid);

                        if (_carregamentoTransporte.NecessarioConfirmacaoMotorista.val() && _gridMotoristas.BuscarRegistros().length > 0) {
                            _carregamentoTransporte.TempoLimiteConfirmacaoMotorista.visible(true);
                            _carregamentoTransporte.TempoLimiteConfirmacaoMotorista.required(true);
                        }

                    } else {
                        _carregamentoTransporte.CodigoMotoristaVeiculo.val(0);
                        _carregamentoTransporte.NomeMotoristaVeiculo.val("");
                    }
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Cargas.MontagemCarga.Aviso, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Cargas.MontagemCarga.Falha, arg.Msg);
            }
        });
    }
}

function RemoverMotoristaClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Cargas.MontagemCarga.Confirmacao, Localization.Resources.Cargas.MontagemCarga.RealmenteDesejaExcluirMotorista.format(sender.Nome), function () {
        let motoristaGrid = e.basicTable.BuscarRegistros();

        for (let i = 0; i < motoristaGrid.length; i++) {
            if (sender.Codigo == motoristaGrid[i].Codigo) {
                motoristaGrid.splice(i, 1);
                break;
            }
        }
        e.basicTable.CarregarGrid(motoristaGrid);

        if (e.basicTable.BuscarRegistros().length == 0) {
            _carregamentoTransporte.TempoLimiteConfirmacaoMotorista.visible(false);
            _carregamentoTransporte.TempoLimiteConfirmacaoMotorista.required(false);
        }

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

        dataGrid = [].concat(dataGrid, data);

        _gridMotoristas.CarregarGrid(dataGrid);

        if (_CONFIGURACAO_TMS.AcoplarMotoristaAoVeiculoAoSelecionarNaCarga && dataGrid.length == 1 && _carregamentoTransporte.CodigoMotoristaVeiculo.val() != dataGrid[0].Codigo) {
            var motorista = dataGrid[0];

            exibirConfirmacao(Localization.Resources.Cargas.MontagemCarga.Atencao, Localization.Resources.Cargas.MontagemCarga.IdentificamosMotoristaVinculadoAoVeiculoDiferenteDoSelecionado.format(_carregamentoTransporte.NomeMotoristaVeiculo.val(), motorista.Nome, _carregamentoTransporte.Veiculo.val()), function () {
                executarReST("Veiculo/VincularMotorista", { Veiculo: _carregamentoTransporte.Veiculo.codEntity(), Motorista: motorista.Codigo }, function (retorno) {
                    if (retorno.Success) {
                        if (retorno.Data) {
                            exibirMensagem(tipoMensagem.ok, Localization.Resources.Cargas.MontagemCarga.Sucesso, Localization.Resources.Cargas.MontagemCarga.MotoristaVinculadoAoVeiculoComSucesso.format(motorista.Nome, _carregamentoTransporte.Veiculo.val()));

                            _carregamentoTransporte.NomeMotoristaVeiculo.val(motorista.Nome);
                            _carregamentoTransporte.CodigoMotoristaVeiculo.val(motorista.Codigo);
                        }
                        else
                            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Cargas.MontagemCarga.Aviso, retorno.Msg);
                    }
                    else
                        exibirMensagem(tipoMensagem.falha, Localization.Resources.Cargas.MontagemCarga.Falha, retorno.Msg);
                });
            });
        }

        if (_carregamentoTransporte.NecessarioConfirmacaoMotorista.val() && _gridMotoristas.BuscarRegistros().length > 0) {
            _carregamentoTransporte.TempoLimiteConfirmacaoMotorista.visible(true);
            _carregamentoTransporte.TempoLimiteConfirmacaoMotorista.required(true);
        }

    }
}

function RetornoRotaFrete(rotaFrete) {
    _carregamentoTransporte.RotaFrete.codEntity(rotaFrete.Codigo);
    _carregamentoTransporte.RotaFrete.val(rotaFrete.Descricao);
    _carregamentoTransporte.RotaFrete.entityDescription(rotaFrete.Descricao);

    // Adicionando automaticamente as fronteiras
    const fronteiras = JSON.parse(rotaFrete.Fronteiras);
    if (rotaFrete && fronteiras && fronteiras.length > 0) {
        _carregamentoTransporte.Fronteira.multiplesEntities(fronteiras);
        _carregamentoTransporte.Fronteira.entityDescription(fronteiras[0].Descricao);
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

    if (_carregamentoTransporte.NecessarioConfirmacaoMotorista.val() && _gridMotoristas.BuscarRegistros().length > 0) {
        _carregamentoTransporte.TempoLimiteConfirmacaoMotorista.visible(true);
        _carregamentoTransporte.TempoLimiteConfirmacaoMotorista.required(true);
    }
}

function preencherListaMotorista() {
    _carregamentoTransporte.ListaMotoristas.list = new Array();

    var motoristas = new Array();

    $.each(_carregamentoTransporte.Motoristas.basicTable.BuscarRegistros(), function (i, motorista) {
        motoristas.push({ Motorista: motorista });
    });

    _carregamentoTransporte.ListaMotoristas.val(JSON.stringify(motoristas));
}

function preencherDadosTransporte(dadosTransporte) {
    PreencherObjetoKnout(_carregamentoTransporte, { Data: dadosTransporte });

    _carregamentoTransporte.TipoDeCarga.Paletizado = dadosTransporte.TipoDeCarga.Paletizado;

    _carregamentoTransporte.TipoOperacao.PermitirInformarRecebedorMontagemCarga = dadosTransporte.TipoOperacao.PermitirInformarRecebedorMontagemCarga;

    _carregamento.Peso.visible(!_carregamentoTransporte.TipoOperacao.ControlarCapacidadePorUnidade);
    _carregamento.Unidade.visible(_carregamentoTransporte.TipoOperacao.ControlarCapacidadePorUnidade);

    recebedorVisivelTransporte(_carregamentoTransporte.TipoOperacao.PermitirInformarRecebedorMontagemCarga);

    RecarregarListaMotoristas();
}

function limparCarregamentoTransporte() {
    LimparCampos(_carregamentoTransporte);
    _carregamentoTransporte.ListaMotoristas.list = new Array();

    _carregamentoTransporte.Fronteira.enable(true);
    _carregamentoTransporte.Fronteira.visible(false);
    _carregamentoTransporte.Fronteira.required(false);

    _carregamentoTransporte.Veiculo.enable(true);
    _carregamentoTransporte.TipoOperacao.enable(true);

    controlarCamposPorGrupoTransportadorInformado();
    RecarregarListaMotoristas();
    LimparSimulacaoFrete();
}

function retornoConsultaCarregamentoTransporteEmpresa(registroSelecionado) {
    var raizCnpjAlterada = _carregamentoTransporte.RaizCNPJEmpresa.val() != registroSelecionado.RaizCnpj;

    _carregamentoTransporte.Empresa.val(registroSelecionado.Descricao);
    _carregamentoTransporte.Empresa.entityDescription(registroSelecionado.Descricao);
    _carregamentoTransporte.Empresa.codEntity(registroSelecionado.Codigo);
    _carregamentoTransporte.RaizCNPJEmpresa.val(registroSelecionado.RaizCnpj);
    _carregamentoTransporte.TransportadorLocalCarregamentoRestringido.val(registroSelecionado.RestringirLocaisCarregamentoAutorizadosMotoristas);

    if (raizCnpjAlterada) {
        removerEmpresaDadosCarregamentoPorFilial();
        _carregamentoTransporte.ListaMotoristas.list = new Array();
        RecarregarListaMotoristas();
    }

    //43261 - Buscar o veiculo do transportador.. se for único...
    if (_CONFIGURACAO_TMS.Transportador.ExisteTransportadorPadraoContratacao && registroSelecionado.Codigo > 0) {
        _carregamentoTransporte.Veiculo.codEntity(0);
        _carregamentoTransporte.Veiculo.val('');
        _pesquisaTransporteVeiculo.registroUnico();
    }
}

function retornoConsultaCarregamentoTransporteGrupoTransportador(registroSelecionado) {
    _carregamentoTransporte.GrupoTransportador.val(registroSelecionado.Descricao);
    _carregamentoTransporte.GrupoTransportador.entityDescription(registroSelecionado.Descricao);
    _carregamentoTransporte.GrupoTransportador.codEntity(registroSelecionado.Codigo);

    controlarCamposPorGrupoTransportadorInformado();
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

function controlarCamposPorGrupoTransportadorInformado() {
    if (_carregamentoTransporte.GrupoTransportador.val() == "") {
        _carregamentoTransporte.GrupoTransportador.required(false);
        _carregamentoTransporte.Empresa.enable(true);
        _carregamentoTransporte.Empresa.required(_CONFIGURACAO_TMS.TransportadorObrigatorioMontagemCarga);
        _carregamentoTransporte.Apolice.required(_CONFIGURACAO_TMS.InformaApoliceSeguroMontagemCarga);

        _carregamento.ValorFreteManual.visible(false);
    }
    else {
        _carregamentoTransporte.GrupoTransportador.required(_CONFIGURACAO_TMS.TransportadorObrigatorioMontagemCarga);
        _carregamentoTransporte.Empresa.enable(false);
        _carregamentoTransporte.Empresa.required(false);
        _carregamentoTransporte.Apolice.required(false);
        _carregamentoTransporte.RaizCNPJEmpresa.val("");

        _carregamento.ValorFreteManual.visible(true);

        LimparCampo(_carregamentoTransporte.Empresa);
        removerEmpresaDadosCarregamentoPorFilial();
    }
}

function setarTransportadorCarregamentoTransporte(pedido) {
    if (_carregamentoTransporte.Empresa.val() == "") {
        if (!_CONFIGURACAO_TMS.Transportador.ExisteTransportadorPadraoContratacao || pedido.CodigoEmpresa != _CONFIGURACAO_TMS.Transportador.TransportadorPadraoContratacao.Codigo) {
            _carregamentoTransporte.Empresa.codEntity(pedido.CodigoEmpresa);
            _carregamentoTransporte.Empresa.entityDescription(pedido.DescricaoEmpresa);
            _carregamentoTransporte.Empresa.val(pedido.DescricaoEmpresa);
            _carregamentoTransporte.TransportadorLocalCarregamentoRestringido.val(pedido.TransportadorLocalCarregamentoRestringido);
        }
    }
}

function setarTipoOperacaoColetaTransporte(pedido) {
    if ((_gerarCargasDeColeta) && (_carregamentoTransporte.TipoOperacao.val() == "")) {
        _carregamentoTransporte.TipoOperacao.codEntity(pedido.CodigoTipoOperacaoColeta);
        _carregamentoTransporte.TipoOperacao.entityDescription(pedido.DescricaoTipoOperacaoColeta);
        _carregamentoTransporte.TipoOperacao.val(pedido.DescricaoTipoOperacaoColeta);
    }
}

function ExibirModalMapaRotaFrete() {
    Global.abrirModal("divModalMapaMotagemCarga");
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
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Cargas.MontagemCarga.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Cargas.MontagemCarga.Falha, arg.Msg);
        }
    });
}

function callbackTipoOperacao(data) {

    if (_CONFIGURACAO_TMS.PermiteSelecionarRotaMontagemCarga) {
        if (data.ObrigarRotaNaMontagemDeCarga) {
            _carregamentoTransporte.RotaFrete.required(true);
            _carregamentoTransporte.RotaFrete.text(Localization.Resources.Cargas.MontagemCarga.Rota.getRequiredFieldDescription());
        }
        else {
            _carregamentoTransporte.RotaFrete.required(false);
            _carregamentoTransporte.RotaFrete.text(Localization.Resources.Cargas.MontagemCarga.Rota);
        }
    }

    if (data.NecessarioConfirmacaoMotorista) {
        _carregamentoTransporte.NecessarioConfirmacaoMotorista.val(true);

        if (data.TempoLimiteConfirmacaoMotorista != "" || data.TempoLimiteConfirmacaoMotorista != "00:00:00")
            _carregamentoTransporte.TempoLimiteConfirmacaoMotorista.val(data.TempoLimiteConfirmacaoMotorista)

        if (_gridMotoristas.BuscarRegistros().length > 0) {
            _carregamentoTransporte.TempoLimiteConfirmacaoMotorista.visible(true);
            _carregamentoTransporte.TempoLimiteConfirmacaoMotorista.required(true);
        }
    } else {
        _carregamentoTransporte.NecessarioConfirmacaoMotorista.val(false);
        _carregamentoTransporte.TempoLimiteConfirmacaoMotorista.visible(false);
        _carregamentoTransporte.TempoLimiteConfirmacaoMotorista.required(false);
    }

    _carregamentoTransporte.PermitirVeiculoDiferente.val(data.PermitirVeiculoDiferenteMontagemCarga);
    _carregamentoTransporte.TipoOperacao.codEntity(data.Codigo);
    _carregamentoTransporte.TipoOperacao.entityDescription(data.Descricao);
    _carregamentoTransporte.TipoOperacao.val(data.Descricao);

    _carregamento.Peso.visible(!data.ControlarCapacidadePorUnidade);
    _carregamento.Unidade.visible(data.ControlarCapacidadePorUnidade);

    recebedorVisivelTransporte(data.PermitirInformarRecebedorMontagemCarga);
    dataInicioViagemPrevistaVisivel(data.ExigirInformarDataPrevisaoInicioViagem)
}

function recebedorVisivelTransporte(permitirInformarRecebedorMontagemCarga) {
    //Se é para gerar cargas de coleta.. o recebedor aparece na aba "Carregamento" vamos ocultar da transporte..
    if (_gerarCargasDeColeta) {
        _carregamentoTransporte.Recebedor.visible(false);
    } else {
        _carregamentoTransporte.Recebedor.visible(permitirInformarRecebedorMontagemCarga);
    }
}

function dataInicioViagemPrevistaVisivel(exigirInformarDataPrevisaoInicioViagem) {
    _carregamento.DataInicioViagemPrevista.visible(exigirInformarDataPrevisaoInicioViagem);
    _carregamento.DataInicioViagemPrevista.required(exigirInformarDataPrevisaoInicioViagem);
}