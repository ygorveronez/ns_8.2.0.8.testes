/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="..\..\Consultas\Estado.js" />
/// <reference path="..\..\Consultas\Filial.js" />
/// <reference path="..\..\Consultas\Localidade.js" />
/// <reference path="..\..\Consultas\ModeloVeicularCarga.js" />
/// <reference path="..\..\Consultas\Regiao.js" />
/// <reference path="..\..\Consultas\TipoCarga.js" />
/// <reference path="..\..\Consultas\TipoOperacao.js" />
/// <reference path="..\..\Consultas\Tranportador.js" />
/// <reference path="..\..\Consultas\Veiculo.js" />
/// <reference path="Container.js" />

// #region Objetos Globais do Arquivo

var _gridMotoristaPreCargaManual;
var _gridDestinoPreCargaManual;
var _gridEstadoDestinoPreCargaManual;
var _gridRegiaoDestinoPreCargaManual;
var _preCargaManual;

// #endregion Objetos Globais do Arquivo

// #region Classes

var PreCargaManual = function () {
    var self = this;
    var dadosTransporteObrigatorios = isDadosTransporteObrigatorios();
    var campoTransportadorObrigatorio = _configuracaoPreCarga.TransportadorObrigatorioPreCarga || dadosTransporteObrigatorios;

    this.UtilizarProgramacaoCarga = PropertyEntity({ val: ko.observable(_configuracaoPreCarga.UtilizarProgramacaoCarga), def: _configuracaoPreCarga.UtilizarProgramacaoCarga, getType: typesKnockout.bool });
    this.Data = PropertyEntity({ text: "*Data de Pré Planejamento:", getType: typesKnockout.dateTime, required: _configuracaoPreCarga.UtilizarProgramacaoCarga, visible: _configuracaoPreCarga.UtilizarProgramacaoCarga });
    this.DataPrevisaoEntrega = PropertyEntity({ text: "Data Previsão de Entrega:", getType: typesKnockout.dateTime, required: ko.observable(false), visible: _configuracaoPreCarga.UtilizarProgramacaoCarga });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Filial: ", idBtnSearch: guid(), required: true });
    this.ModeloVeicularCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Modelo Veicular: ", idBtnSearch: guid(), NumeroReboques: 0 });
    this.TipoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Carga: ", idBtnSearch: guid() });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Operação: ", idBtnSearch: guid(), ExigePlacaTracao: false });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: (campoTransportadorObrigatorio ? "*" : "") + "Transportador: ", label: "Transportador: ", idBtnSearch: guid(), required: campoTransportadorObrigatorio, visible: !_configuracaoPreCarga.UtilizarProgramacaoCarga });
    this.Tracao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Veículo: "), idBtnSearch: guid(), required: dadosTransporteObrigatorios, Empresa: 0, visible: !_configuracaoPreCarga.UtilizarProgramacaoCarga });
    this.Reboque = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Veículo (Carreta): "), idBtnSearch: guid(), visible: ko.observable(false), required: false, Empresa: 0 });
    this.SegundoReboque = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Veículo (Carreta 2):"), idBtnSearch: guid(), visible: ko.observable(false), required: false, Empresa: 0 });
    this.RotaFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Rota: ", idBtnSearch: guid(), required: false });
    this.Observacao = PropertyEntity({ val: ko.observable(""), text: Localization.Resources.Cargas.Carga.Observacao.getFieldDescription(), visible: ko.observable(true) });
    this.Peso = PropertyEntity({ maxlength: 12, getType: typesKnockout.decimal, text: "Peso:", required: false, visible: ko.observable(true) });
    this.QuantidadePallets = PropertyEntity({ maxlength: 12, getType: typesKnockout.decimal, text: "Quantidade de Pallets:", required: false, visible: ko.observable(true) });



    this.ModeloVeicularCarga.codEntity.subscribe(function (novoValor) {
        if ((novoValor == 0) && (self.ModeloVeicularCarga.NumeroReboques > 0)) {
            self.ModeloVeicularCarga.NumeroReboques = 0;

            controlarExibicaoCamposVeiculoPreCargaManual();
        }
    });

    this.TipoOperacao.codEntity.subscribe(function (novoValor) {
        if ((novoValor == 0) && self.TipoOperacao.ExigePlacaTracao) {
            self.TipoOperacao.ExigePlacaTracao = false;

            controlarExibicaoCamposVeiculoPreCargaManual();
        }
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarPreCargaManualClick, type: types.event, text: Localization.Resources.Cargas.Carga.Adicionar, idGrid: guid() });
    this.AdicionarMotoristas = PropertyEntity({ idBtnSearch: guid(), type: types.event, text: Localization.Resources.Cargas.Carga.AdicionarMotoristas, idGrid: guid() });
    this.AdicionarDestinos = PropertyEntity({ type: types.event, text: Localization.Resources.Cargas.Carga.AdicionarCidades, idBtnSearch: guid(), idGrid: guid() });
    this.AdicionarEstadosDestino = PropertyEntity({ type: types.event, text: Localization.Resources.Cargas.Carga.AdicionarEstados, idBtnSearch: guid(), idGrid: guid() });
    this.AdicionarRegioesDestino = PropertyEntity({ type: types.event, text: Localization.Resources.Cargas.Carga.AdicionarRegioes, idBtnSearch: guid(), idGrid: guid() });
}

// #endregion Classes

// #region Funções de Inicialização

function loadGridDestinoPreCargaManual() {
    var opcaoExcluir = { descricao: "Excluir", id: guid(), metodo: function (registroSelecionado) { excluirDestinoPreCargaManualClick(_gridDestinoPreCargaManual, registroSelecionado); } };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 15, opcoes: [opcaoExcluir] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Cargas.Carga.Descricao, width: "85%" }
    ];

    _gridDestinoPreCargaManual = new BasicDataTable(_preCargaManual.AdicionarDestinos.idGrid, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarLocalidades(_preCargaManual.AdicionarDestinos, null, null, null, _gridDestinoPreCargaManual, controlarVisibilidadeAbasPreCargaManualDestino);

    _gridDestinoPreCargaManual.CarregarGrid([]);
}

function loadGridEstadoDestinoPreCargaManual() {
    var opcaoExcluir = { descricao: "Excluir", id: guid(), metodo: function (registroSelecionado) { excluirDestinoPreCargaManualClick(_gridEstadoDestinoPreCargaManual, registroSelecionado); } };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 15, opcoes: [opcaoExcluir] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Cargas.Carga.Descricao, width: "85%" }
    ];

    _gridEstadoDestinoPreCargaManual = new BasicDataTable(_preCargaManual.AdicionarEstadosDestino.idGrid, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarEstados(_preCargaManual.AdicionarEstadosDestino, null, _gridEstadoDestinoPreCargaManual, controlarVisibilidadeAbasPreCargaManualDestino);

    _gridEstadoDestinoPreCargaManual.CarregarGrid([]);
}

function loadGridMotoristaPreCargaManual() {
    var opcaoEditar = { descricao: "Excluir", id: guid(), metodo: excluirMotoristaPreCargaManualClick };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [opcaoEditar]};
    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoEmpresa", visible: false },
        { data: "CPF", title: Localization.Resources.Cargas.Carga.CPF, width: "15%", className: "text-align-center", orderable: false },
        { data: "Nome", title: Localization.Resources.Cargas.Carga.Nome, width: "70%", className: "text-align-left", orderable: false }
    ];

    _gridMotoristaPreCargaManual = new BasicDataTable(_preCargaManual.AdicionarMotoristas.idGrid, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarMotoristas(_preCargaManual.AdicionarMotoristas, null, _preCargaManual.Transportador, _gridMotoristaPreCargaManual);

    _gridMotoristaPreCargaManual.CarregarGrid([]);
}

function loadGridRegiaoDestinoPreCargaManual() {
    var opcaoExcluir = { descricao: "Excluir", id: guid(), metodo: function (registroSelecionado) { excluirDestinoPreCargaManualClick(_gridRegiaoDestinoPreCargaManual, registroSelecionado); } };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 15, opcoes: [opcaoExcluir] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Cargas.Carga.Descricao, width: "85%" }
    ];

    _gridRegiaoDestinoPreCargaManual = new BasicDataTable(_preCargaManual.AdicionarRegioesDestino.idGrid, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarRegioes(_preCargaManual.AdicionarRegioesDestino, null, _gridRegiaoDestinoPreCargaManual, controlarVisibilidadeAbasPreCargaManualDestino);

    _gridRegiaoDestinoPreCargaManual.CarregarGrid([]);
}

function loadPreCargaManual() {
    _preCargaManual = new PreCargaManual();
    KoBindings(_preCargaManual, "knockoutAdicionarPreCargaManual");

    new BuscarFilial(_preCargaManual.Filial);
    new BuscarRotasFrete(_preCargaManual.RotaFrete);
    new BuscarModelosVeicularesCarga(_preCargaManual.ModeloVeicularCarga, retornoConsultaModeloVeicularCargaPreCargaManual);
    new BuscarTiposdeCargaPorFilial(_preCargaManual.TipoCarga, null, _preCargaManual.Filial);
    new BuscarTiposOperacao(_preCargaManual.TipoOperacao, retornoConsultaTipoOperacaoPreCargaManual);
    new BuscarTransportadores(_preCargaManual.Transportador, retornoConsultaTransportadorPreCargaManual, null, null, null, _preCargaManual.Filial);
    new BuscarVeiculos(_preCargaManual.Tracao, retornoConsultaTracaoPreCargaManual, _preCargaManual.Transportador, null, null, null, null, null, null, null, null, _preCargaManual.TipoVeiculo);
    new BuscarVeiculos(_preCargaManual.Reboque, retornoConsultaReboquePreCargaManual, _preCargaManual.Transportador, null, null, null, null, null, null, null, null, "1");
    new BuscarVeiculos(_preCargaManual.SegundoReboque, retornoConsultaSegundoReboquePreCargaManual, _preCargaManual.Transportador, null, null, null, null, null, null, null, null, "1");

    loadGridMotoristaPreCargaManual();
    loadGridDestinoPreCargaManual();
    loadGridEstadoDestinoPreCargaManual();
    loadGridRegiaoDestinoPreCargaManual();
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function adicionarPreCargaManualClick() {
    if (!validarCamposObrigatoriosPreCargaManual())
        return;

    executarReST("PreCarga/AdicionarPreCargaManual", obterPreCargaManualSalvar(), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Cargas.Carga.Sucesso, Localization.Resources.Cargas.Carga.PrePlanejamentoAdicionadoSucesso);
                fecharModalAdicionarPreCargaManual();
                recarregarPreCargas();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Cargas.Carga.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Cargas.Carga.Falha, retorno.Msg);
    });
}

function excluirMotoristaPreCargaManualClick(registroSelecionado) {
    var motoristas = _gridMotoristaPreCargaManual.BuscarRegistros();

    for (var i = 0; i < motoristas.length; i++) {
        if (registroSelecionado.Codigo == motoristas[i].Codigo) {
            motoristas.splice(i, 1);
            break;
        }
    }

    _gridMotoristaPreCargaManual.CarregarGrid(motoristas);
}

function excluirDestinoPreCargaManualClick(gridDestino, registroSelecionado) {
    var listaDestino = gridDestino.BuscarRegistros().slice();

    for (var i = 0; i < listaDestino.length; i++) {
        if (registroSelecionado.Codigo == listaDestino[i].Codigo) {
            listaDestino.splice(i, 1);
            break;
        }
    }

    gridDestino.CarregarGrid(listaDestino);
    controlarVisibilidadeAbasPreCargaManualDestino();
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function exibirModalAdicionarPreCargaManual(filial) {
    if (filial) {
        _preCargaManual.Filial.codEntity(filial.Codigo);
        _preCargaManual.Filial.val(filial.Descricao);
    }

    _preCargaManual.Data.minDate(Global.DataAtual());

    controlarExibicaoCamposVeiculoPreCargaManual();

    Global.abrirModal('divModalAdicionarPreCargaManual');
    $("#divModalAdicionarPreCargaManual").one('hidden.bs.modal', function () {
        limparCamposPreCargaManual();
    });
}

// #endregion Funções Públicas

// #region Funções Privadas

function controlarExibicaoCamposVeiculoPreCargaManual() {
    if (_configuracaoPreCarga.UtilizarProgramacaoCarga)
        return;

    var reboqueVisivel = false;
    var segundoReboqueVisivel = false;

    if (_preCargaManual.TipoOperacao.ExigePlacaTracao) {
        reboqueVisivel = (_preCargaManual.ModeloVeicularCarga.NumeroReboques >= 1);
        segundoReboqueVisivel = (_preCargaManual.ModeloVeicularCarga.NumeroReboques > 1);
    }

    if (!reboqueVisivel)
        limparCampoVeiculo(_preCargaManual.Reboque);

    if (!segundoReboqueVisivel)
        limparCampoVeiculo(_preCargaManual.SegundoReboque);

    _preCargaManual.Tracao.text((_preCargaManual.Tracao.required ? "*" : "") + (reboqueVisivel ? Localization.Resources.Cargas.Carga.TracaoCavalo : Localization.Resources.Cargas.Carga.Veiculo));

    _preCargaManual.Reboque.required = (reboqueVisivel && _configuracaoPreCarga.DadosTransporteObrigatorioPreCarga);
    _preCargaManual.Reboque.text((_preCargaManual.Reboque.required ? "*" : "") + (segundoReboqueVisivel ? Localization.Resources.Cargas.Carga.VeiculoCarretaUm : Localization.Resources.Cargas.Carga.VeiculosCarreta));
    _preCargaManual.Reboque.visible(reboqueVisivel);

    _preCargaManual.SegundoReboque.required = (segundoReboqueVisivel && _configuracaoPreCarga.DadosTransporteObrigatorioPreCarga);
    _preCargaManual.SegundoReboque.text((_preCargaManual.SegundoReboque.required ? "*" : "") + Localization.Resources.Cargas.Carga.VeiculoCarretaDois);
    _preCargaManual.SegundoReboque.visible(segundoReboqueVisivel);
}

function controlarVisibilidadeAbasPreCargaManualDestino() {
    if (_gridDestinoPreCargaManual.BuscarRegistros().length > 0) {
        $("#liTabPreCargaManualCidadesDestino").show();
        $("#liTabPreCargaManualEstadosDestino").hide();
        $("#liTabPreCargaManualRegioesDestino").hide();

        $(".nav-tabs a[href='#tabPreCargaManualCidadesDestino']").tab('show');
    }
    else if (_gridEstadoDestinoPreCargaManual.BuscarRegistros().length > 0) {
        $("#liTabPreCargaManualCidadesDestino").hide();
        $("#liTabPreCargaManualEstadosDestino").show();
        $("#liTabPreCargaManualRegioesDestino").hide();

        $(".nav-tabs a[href='#tabPreCargaManualEstadosDestino']").tab('show');
    }
    else if (_gridRegiaoDestinoPreCargaManual.BuscarRegistros().length > 0) {
        $("#liTabPreCargaManualCidadesDestino").hide();
        $("#liTabPreCargaManualEstadosDestino").hide();
        $("#liTabPreCargaManualRegioesDestino").show();

        $(".nav-tabs a[href='#tabPreCargaManualRegioesDestino']").tab('show');
    }
    else {
        $("#liTabPreCargaManualCidadesDestino").show();
        $("#liTabPreCargaManualEstadosDestino").show();
        $("#liTabPreCargaManualRegioesDestino").show();
    }
}

function excluirMotoristasTransportadorDiferentePreCargaManual(codigoTransportadorSelecionado) {
    var motoristas = _gridMotoristaPreCargaManual.BuscarRegistros();
    var motoristasMesmoTransportador = new Array();

    for (var i = 0; i < motoristas.length; i++) {
        if (codigoTransportadorSelecionado == motoristas[i].CodigoEmpresa)
            motoristasMesmoTransportador.push(motoristas[i]);
    }

    _gridMotoristaPreCargaManual.CarregarGrid(motoristasMesmoTransportador);
}

function fecharModalAdicionarPreCargaManual() {
    Global.fecharModal("divModalAdicionarPreCargaManual");
}

function isDadosTransporteObrigatorios() {
    return (_configuracaoPreCarga.DadosTransporteObrigatorioPreCarga && _configuracaoPreCarga.UtilizarProgramacaoCarga);
}

function limparCampoVeiculo(knoutVeiculo) {
    LimparCampoEntity(knoutVeiculo);

    knoutVeiculo.Empresa = 0;
}

function limparCamposPreCargaManual() {
    LimparCampos(_preCargaManual);

    _preCargaManual.ModeloVeicularCarga.NumeroReboques = 0;
    _preCargaManual.Reboque.Empresa = 0;
    _preCargaManual.SegundoReboque.Empresa = 0;
    _preCargaManual.TipoOperacao.ExigePlacaTracao = false;
    _preCargaManual.Tracao.Empresa = 0;

    _gridMotoristaPreCargaManual.CarregarGrid([]);
    _gridDestinoPreCargaManual.CarregarGrid([]);
    _gridEstadoDestinoPreCargaManual.CarregarGrid([]);
    _gridRegiaoDestinoPreCargaManual.CarregarGrid([]);

    controlarVisibilidadeAbasPreCargaManualDestino();

    $(".nav-tabs a[href='#tabPreCargaManualCidadesDestino']").tab('show');
}

function obterPreCargaManualSalvar() {
    var codigosDestinos = [];
    var codigosEstadosDestino = [];
    var codigosMotoristas = [];
    var codigosRegioesDestino = [];

    _gridDestinoPreCargaManual.BuscarRegistros().slice().forEach(function (destino) {
        codigosDestinos.push(destino.Codigo);
    });

    _gridEstadoDestinoPreCargaManual.BuscarRegistros().slice().forEach(function (estadoDestino) {
        codigosEstadosDestino.push(estadoDestino.Codigo);
    });

    _gridRegiaoDestinoPreCargaManual.BuscarRegistros().slice().forEach(function (regiaoDestino) {
        codigosRegioesDestino.push(regiaoDestino.Codigo);
    });

    _gridMotoristaPreCargaManual.BuscarRegistros().slice().forEach(function (motorista) {
        codigosMotoristas.push(motorista.Codigo);
    });

    var preCargaManual = RetornarObjetoPesquisa(_preCargaManual);

    preCargaManual.Destinos = JSON.stringify(codigosDestinos);
    preCargaManual.EstadosDestino = JSON.stringify(codigosEstadosDestino);
    preCargaManual.Motoristas = JSON.stringify(codigosMotoristas);
    preCargaManual.RegioesDestino = JSON.stringify(codigosRegioesDestino);

    return preCargaManual;
}

function preencherTransportadorPorVeiculoSelecionadoPreCargaManual(veiculoSelecionado) {
    if (!_configuracaoPreCarga.DadosTransporteObrigatorioPreCarga && _preCargaManual.Transportador.codEntity() == 0 && veiculoSelecionado.CodigoEmpresa > 0) {
        _preCargaManual.Transportador.codEntity(veiculoSelecionado.CodigoEmpresa);
        _preCargaManual.Transportador.entityDescription(veiculoSelecionado.Empresa);
        _preCargaManual.Transportador.val(veiculoSelecionado.Empresa);
    }
}

function retornoConsultaModeloVeicularCargaPreCargaManual(modeloSelecionado) {
    _preCargaManual.ModeloVeicularCarga.codEntity(modeloSelecionado.Codigo);
    _preCargaManual.ModeloVeicularCarga.entityDescription(modeloSelecionado.Descricao);
    _preCargaManual.ModeloVeicularCarga.val(modeloSelecionado.Descricao);

    if (_preCargaManual.ModeloVeicularCarga.NumeroReboques != modeloSelecionado.NumeroReboques) {
        _preCargaManual.ModeloVeicularCarga.NumeroReboques = modeloSelecionado.NumeroReboques;

        controlarExibicaoCamposVeiculoPreCargaManual();
    }
}

function retornoConsultaReboquePreCargaManual(reboqueSelecionado) {
    if (_preCargaManual.SegundoReboque.codEntity() == reboqueSelecionado.Codigo) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Cargas.Carga.VeiculoCarretaUm, Localization.Resources.Cargas.Carga.NaoPossivelSelecionarDuasCarretasIguais);

        limparCampoVeiculo(_preCargaManual.Reboque);
    }
    else {
        _preCargaManual.Reboque.codEntity(reboqueSelecionado.Codigo);
        _preCargaManual.Reboque.entityDescription(reboqueSelecionado.Placa);
        _preCargaManual.Reboque.val(reboqueSelecionado.Placa);
        _preCargaManual.Reboque.Empresa = reboqueSelecionado.CodigoEmpresa;

        preencherTransportadorPorVeiculoSelecionadoPreCargaManual(reboqueSelecionado);
    }
}

function retornoConsultaSegundoReboquePreCargaManual(reboqueSelecionado) {
    if (_preCargaManual.Reboque.codEntity() == reboqueSelecionado.Codigo) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Cargas.Carga.VeiculoCarretaUm, Localization.Resources.Cargas.Carga.NaoPossivelSelecionarDuasCarretasIguais);

        limparCampoVeiculo(_preCargaManual.SegundoReboque);
    }
    else {
        _preCargaManual.SegundoReboque.codEntity(reboqueSelecionado.Codigo);
        _preCargaManual.SegundoReboque.entityDescription(reboqueSelecionado.Placa);
        _preCargaManual.SegundoReboque.val(reboqueSelecionado.Placa);
        _preCargaManual.SegundoReboque.Empresa = reboqueSelecionado.CodigoEmpresa;

        preencherTransportadorPorVeiculoSelecionadoPreCargaManual(reboqueSelecionado);
    }
}

function retornoConsultaTipoOperacaoPreCargaManual(tipoOperacaoSelecionado) {
    _preCargaManual.TipoOperacao.codEntity(tipoOperacaoSelecionado.Codigo);
    _preCargaManual.TipoOperacao.entityDescription(tipoOperacaoSelecionado.Descricao);
    _preCargaManual.TipoOperacao.val(tipoOperacaoSelecionado.Descricao);

    if (_preCargaManual.TipoOperacao.ExigePlacaTracao != tipoOperacaoSelecionado.ExigePlacaTracao) {
        _preCargaManual.TipoOperacao.ExigePlacaTracao = tipoOperacaoSelecionado.ExigePlacaTracao;

        controlarExibicaoCamposVeiculoPreCargaManual();
    }
}

function retornoConsultaTracaoPreCargaManual(veiculoSelecionado) {
    _preCargaManual.Tracao.codEntity(veiculoSelecionado.Codigo);
    _preCargaManual.Tracao.Empresa = veiculoSelecionado.CodigoEmpresa;

    if (_preCargaManual.TipoOperacao.ExigePlacaTracao) {
        _preCargaManual.Tracao.entityDescription(veiculoSelecionado.Placa);
        _preCargaManual.Tracao.val(veiculoSelecionado.Placa);
    }
    else {
        _preCargaManual.Tracao.entityDescription(veiculoSelecionado.ConjuntoPlacasSemModeloVeicular);
        _preCargaManual.Tracao.val(veiculoSelecionado.ConjuntoPlacasSemModeloVeicular);
    }

    Global.setarFocoProximoCampo(_preCargaManual.Tracao.id);

    preencherTransportadorPorVeiculoSelecionadoPreCargaManual(veiculoSelecionado);
}

function retornoConsultaTransportadorPreCargaManual(transportadorSelecionado) {
    _preCargaManual.Transportador.codEntity(transportadorSelecionado.Codigo);
    _preCargaManual.Transportador.entityDescription(transportadorSelecionado.Descricao);
    _preCargaManual.Transportador.val(transportadorSelecionado.Descricao);

    if (_preCargaManual.Tracao.codEntity() > 0 && _preCargaManual.Tracao.Empresa != transportadorSelecionado.Codigo)
        limparCampoVeiculo(_preCargaManual.Tracao);

    if (_preCargaManual.Reboque.codEntity() > 0 && _preCargaManual.Reboque.Empresa != transportadorSelecionado.Codigo)
        limparCampoVeiculo(_preCargaManual.Reboque);

    if (_preCargaManual.SegundoReboque.codEntity() > 0 && _preCargaManual.SegundoReboque.Empresa != transportadorSelecionado.Codigo)
        limparCampoVeiculo(_preCargaManual.SegundoReboque);

    excluirMotoristasTransportadorDiferentePreCargaManual(transportadorSelecionado.Codigo);
}

function validarCamposObrigatoriosPreCargaManual() {
    if (!ValidarCamposObrigatorios(_preCargaManual)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Cargas.Carga.CamposObrigatorios, Localization.Resources.Cargas.Carga.PorFavorInformeOsCamposObrigatoriosAntesDeContinuar);
        return false;
    }

    if (isDadosTransporteObrigatorios()) {
        if (_gridMotoristaPreCargaManual.BuscarRegistros().length == 0) {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Cargas.Carga.CamposObrigatorios, Localization.Resources.Cargas.Carga.PorFavorInformeMotorista);
            return false;
        }
    }

    if (_configuracaoPreCarga.UtilizarProgramacaoCarga) {
        var totalDestinos = _gridDestinoPreCargaManual.BuscarRegistros().length;
        var totalEstadosDestino = _gridEstadoDestinoPreCargaManual.BuscarRegistros().length;
        var totalRegioesDestino = _gridRegiaoDestinoPreCargaManual.BuscarRegistros().length;
        var possuiDestinoinformado = (totalDestinos > 0) || (totalEstadosDestino > 0) || (totalRegioesDestino > 0);

        if (!possuiDestinoinformado) {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Cargas.Carga.CamposObrigatorios, Localization.Resources.Cargas.Carga.PorFavorInformeMotorista);
            return false;
        }
    }

    return true;
}

// #endregion Funções Privadas
