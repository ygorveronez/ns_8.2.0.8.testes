/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/ColetaEntrega.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Enumeradores/EnumSituacaoGestaoDadosColeta.js" />
/// <reference path="DadosTransporteIntegracao.js" />
/// <reference path="GestaoDadosColeta.js" />

// #region Objetos Globais do Arquivo

var _gestaoDadosColetaDadosTransporte;
var _gridGestaoDadosColetaDadosTransporteMotorista;
var _resumoDadosColetaDadosTransporte;

// #endregion Objetos Globais do Arquivo

// #region Classes

var GestaoDadosColetaDadosTransporte = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoGestaoDadosColeta.AguardandoAprovacao), def: EnumSituacaoGestaoDadosColeta.AguardandoAprovacao });
    this.ExigirConfirmacaoTracao = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.NumeroReboques = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.TipoVeiculo = PropertyEntity({ val: ko.observable(""), visible: false });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), visible: false });

    this.Coleta = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), val: ko.observable(""), text: ko.observable("*Coleta:"), idBtnSearch: guid(), required: false, modeloVeicular: ko.observable(0), visible: ko.observable(true) });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("*Veículo:"), idBtnSearch: guid(), required: true, enable: ko.observable(true) });
    this.Reboque = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(""), idBtnSearch: guid(), visible: ko.observable(false), enable: ko.observable(true) });
    this.SegundoReboque = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("*Veículo (Carreta 2):"), idBtnSearch: guid(), visible: ko.observable(false), enable: ko.observable(true) });

    this.AdicionarMotorista = PropertyEntity({ type: types.event, text: "Adicionar Motoristas", idBtnSearch: guid(), idGrid: guid() });
    this.SalvarDadosTransporte = PropertyEntity({ type: types.event, eventClick: adicionarGestaoDadosColetaDadosTransporteClick, text: "Salvar", visible: ko.observable(true) });
    this.AprovarDadosTransporte = PropertyEntity({ type: types.event, eventClick: aprovarGestaoDadosColetaDadosTransporteClick, text: "Aprovar", visible: ko.observable(false) });
    this.RejeitarDadosTransporte = PropertyEntity({ type: types.event, eventClick: rejeitarGestaoDadosColetaDadosTransporteClick, text: "Rejeitar", visible: ko.observable(false) });
}

var ResumoGestaoDadosColetaDadosTransporte = function () {
    this.Carga = PropertyEntity({ val: ko.observable(""), text: "Carga: " });
    this.Cliente = PropertyEntity({ val: ko.observable(""), text: "Cliente: " });
    this.Endereco = PropertyEntity({ val: ko.observable(""), text: "Endereço do Cliente: " });
    this.Pedidos = PropertyEntity({ val: ko.observable(""), text: "Pedidos: " });
    this.Transportador = PropertyEntity({ val: ko.observable(""), text: "Transportador: " });
    this.Origem = PropertyEntity({ val: ko.observable(""), text: "Origem: " });
    this.Destino = PropertyEntity({ val: ko.observable(""), text: "Destino: " });
}

// #endregion Classes

// #region Funções de Inicialização

function loadGestaoDadosColetaDadosTransporte() {
    _gestaoDadosColetaDadosTransporte = new GestaoDadosColetaDadosTransporte();
    KoBindings(_gestaoDadosColetaDadosTransporte, "knockoutGestaoDadosColetaDadosTransporte");

    _resumoDadosColetaDadosTransporte = new ResumoGestaoDadosColetaDadosTransporte();
    KoBindings(_resumoDadosColetaDadosTransporte, "knockoutResumoDadosTransporte");

    var transportadorFiltrarDadosTransporte = (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe) ? _gestaoDadosColetaDadosTransporte.Transportador : null;

    new BuscarColetas(_gestaoDadosColetaDadosTransporte.Coleta, retornoConsultaGestaoDadosColetaDadosTransporteColeta);
    new BuscarVeiculos(_gestaoDadosColetaDadosTransporte.Veiculo, retornoConsultaGestaoDadosColetaDadosTransporteVeiculo, transportadorFiltrarDadosTransporte, null, null, null, null, null, null, null, null, _gestaoDadosColetaDadosTransporte.TipoVeiculo);
    new BuscarVeiculos(_gestaoDadosColetaDadosTransporte.Reboque, retornoConsultaGestaoDadosColetaDadosTransporteReboque, transportadorFiltrarDadosTransporte, null, null, null, null, null, null, null, null, "1");
    new BuscarVeiculos(_gestaoDadosColetaDadosTransporte.SegundoReboque, retornoConsultaGestaoDadosColetaDadosTransporteSegundoReboque, transportadorFiltrarDadosTransporte, null, null, null, null, null, null, null, null, "1");
    
    loadGridGestaoDadosColetaDadosTransporteMotorista();
    loadGestaoDadosColetaDadosTransporteIntegracao();
}

function loadGridGestaoDadosColetaDadosTransporteMotorista() {
    var opcaoExcluir = { descricao: "Excluir", id: guid(), tamanho: "25", metodo: excluirGestaoDadosColetaDadosTransporteMotoristaClick }
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoExcluir] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "75%" }
    ];

    _gridGestaoDadosColetaDadosTransporteMotorista = new BasicDataTable(_gestaoDadosColetaDadosTransporte.AdicionarMotorista.idGrid, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarMotoristas(_gestaoDadosColetaDadosTransporte.AdicionarMotorista, null, null, _gridGestaoDadosColetaDadosTransporteMotorista);

    _gridGestaoDadosColetaDadosTransporteMotorista.CarregarGrid([]);
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function adicionarGestaoDadosColetaDadosTransporteClick() {
    if (!ValidarCamposObrigatorios(_gestaoDadosColetaDadosTransporte)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return;
    }

    var codigosMotoristas = _gridGestaoDadosColetaDadosTransporteMotorista.BuscarCodigosRegistros();

    if (codigosMotoristas.length == 0) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, "Por favor, informe um ou mais motoristas.");
        return;
    }

    var data = {
        Coleta: _gestaoDadosColetaDadosTransporte.Coleta.codEntity(),
        Veiculo: _gestaoDadosColetaDadosTransporte.Veiculo.codEntity(),
        Reboque: _gestaoDadosColetaDadosTransporte.Reboque.codEntity(),
        SegundoReboque: _gestaoDadosColetaDadosTransporte.SegundoReboque.codEntity(),
        Motoristas: JSON.stringify(codigosMotoristas)
    };

    executarReST("GestaoDadosColeta/AdicionarGestaoDadosColetaDadosTransporte", data, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, retorno.Msg);
                Global.fecharModal("divModalAdicionarGestaoDadosColetaDadosTransporte");
                recarregarGridGestaoDadosColeta();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function aprovarGestaoDadosColetaDadosTransporteClick() {
    executarReST("GestaoDadosColeta/AprovacaoGestaoDadosColetaDadosTransporte", { Codigo: _gestaoDadosColetaDadosTransporte.Codigo.val() } , function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, retorno.Msg);
                Global.fecharModal("divModalAdicionarGestaoDadosColetaDadosTransporte");
                recarregarGridGestaoDadosColeta();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function excluirGestaoDadosColetaDadosTransporteMotoristaClick(registroSelecionado) {
    var listaMotoristas = _gridGestaoDadosColetaDadosTransporteMotorista.BuscarRegistros().slice();

    for (var i = 0; i < listaMotoristas.length; i++) {
        if (registroSelecionado.Codigo == listaMotoristas[i].Codigo) {
            listaMotoristas.splice(i, 1);
            break;
        }
    }

    _gridGestaoDadosColetaDadosTransporteMotorista.CarregarGrid(listaMotoristas);
}

function rejeitarGestaoDadosColetaDadosTransporteClick() {
    executarReST("GestaoDadosColeta/RejeitarGestaoDadosColetaDadosTransporte", { Codigo: _gestaoDadosColetaDadosTransporte.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, retorno.Msg);
                Global.fecharModal("divModalAdicionarGestaoDadosColetaDadosTransporte");
                recarregarGridGestaoDadosColeta();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function exibirModalAdicionarGestaoDadosColetaDadosTransporte() {
    limparCamposGestaoDadosColetaDadosTransporte();
    controlarCamposGestaoDadosColetaDadosTransporte();
    exibirModalGestaoDadosColetaDadosTransporte();
}

function exibirModalEditarGestaoDadosColetaDadosTransporte(registroSelecionado) {
    limparCamposGestaoDadosColetaDadosTransporte();

    executarReST("GestaoDadosColeta/BuscarDadosTransportePorCodigo", { Codigo: registroSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                PreencherObjetoKnout(_resumoDadosColetaDadosTransporte, { Data: retorno.Data.GestaoDadosColeta });
                PreencherObjetoKnout(_gestaoDadosColetaDadosTransporte, { Data: retorno.Data.DadosTransporte });
                _gridGestaoDadosColetaDadosTransporteMotorista.CarregarGrid(retorno.Data.DadosTransporte.Motoristas, false);
                preencherGestaoDadosColetaDadosTransporteIntegracao(registroSelecionado.Codigo, retorno.Data.DadosTransporte.Situacao);
                controlarVisibilidadeGestaoDadosColetaDadosTransporteVeiculos();
                controlarCamposGestaoDadosColetaDadosTransporte();
                exibirModalGestaoDadosColetaDadosTransporte();
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

// #endregion Funções Públicas

// #region Funções Privadas

function controlarCamposGestaoDadosColetaDadosTransporte() {
    var registroEmEdicao = _gestaoDadosColetaDadosTransporte.Codigo.val() > 0;
    var permitirAprovacao = (_gestaoDadosColetaDadosTransporte.Situacao.val() == EnumSituacaoGestaoDadosColeta.AguardandoAprovacao) && (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCTe);

    if (registroEmEdicao) {
        $('#divDetalhesColetaTransporte').show();
        $('.botoao-adicionar-motorista').hide();

        _gestaoDadosColetaDadosTransporte.SalvarDadosTransporte.visible(false);
        _gestaoDadosColetaDadosTransporte.AprovarDadosTransporte.visible(permitirAprovacao);
        _gestaoDadosColetaDadosTransporte.RejeitarDadosTransporte.visible(permitirAprovacao);
    }
    else {
        $('#divDetalhesColetaTransporte').hide();
        $('.botoao-adicionar-motorista').show();

        _gestaoDadosColetaDadosTransporte.SalvarDadosTransporte.visible(true);
        _gestaoDadosColetaDadosTransporte.AprovarDadosTransporte.visible(false);
        _gestaoDadosColetaDadosTransporte.RejeitarDadosTransporte.visible(false);
    }

    _gestaoDadosColetaDadosTransporte.Coleta.visible(!registroEmEdicao);
    _gestaoDadosColetaDadosTransporte.Coleta.required = !registroEmEdicao;
    _gestaoDadosColetaDadosTransporte.Veiculo.enable(!registroEmEdicao);
    _gestaoDadosColetaDadosTransporte.Reboque.enable(!registroEmEdicao);
    _gestaoDadosColetaDadosTransporte.SegundoReboque.enable(!registroEmEdicao);
}

function controlarVisibilidadeGestaoDadosColetaDadosTransporteVeiculos() {
    var reboqueVisivel = false;
    var segundoReboqueVisivel = false;

    if (_gestaoDadosColetaDadosTransporte.ExigirConfirmacaoTracao.val()) {
        reboqueVisivel = (_gestaoDadosColetaDadosTransporte.NumeroReboques.val() >= 1);
        segundoReboqueVisivel = (_gestaoDadosColetaDadosTransporte.NumeroReboques.val() > 1);
    }

    if (!reboqueVisivel)
        LimparCampoEntity(_gestaoDadosColetaDadosTransporte.Reboque);

    if (!segundoReboqueVisivel)
        LimparCampoEntity(_gestaoDadosColetaDadosTransporte.SegundoReboque);

    _gestaoDadosColetaDadosTransporte.Veiculo.text(reboqueVisivel ? "*Tração (Cavalo):" : "*Veículo:");

    _gestaoDadosColetaDadosTransporte.Reboque.required = reboqueVisivel;
    _gestaoDadosColetaDadosTransporte.Reboque.text(segundoReboqueVisivel ? "*Veículo (Carreta 1):" : "*Veículo (Carreta):");
    _gestaoDadosColetaDadosTransporte.Reboque.visible(reboqueVisivel);

    _gestaoDadosColetaDadosTransporte.SegundoReboque.required = segundoReboqueVisivel;
    _gestaoDadosColetaDadosTransporte.SegundoReboque.visible(segundoReboqueVisivel);
}

function exibirModalGestaoDadosColetaDadosTransporte() {
    Global.abrirModal('divModalAdicionarGestaoDadosColetaDadosTransporte');
}

function limparCamposGestaoDadosColetaDadosTransporte() {
    _gridGestaoDadosColetaDadosTransporteMotorista.CarregarGrid([]);
    LimparCampos(_gestaoDadosColetaDadosTransporte);
    limparGestaoDadosColetaDadosTransporteIntegracao();
    Global.ResetarAbas();
    controlarVisibilidadeGestaoDadosColetaDadosTransporteVeiculos();
}

function preencherGestaoDadosColetaDadosTransporteReboquesPorVeiculoSelecionado(veiculoSelecionado) {
    if (!_gestaoDadosColetaDadosTransporte.ExigirConfirmacaoTracao.val())
        return;

    if (!veiculoSelecionado.CodigosVeiculosVinculados)
        return;

    var codigosReboques = veiculoSelecionado.CodigosVeiculosVinculados.split(", ");
    var placasReboques = veiculoSelecionado.VeiculosVinculados.split(", ");

    if (_gestaoDadosColetaDadosTransporte.Reboque.visible()) {
        _gestaoDadosColetaDadosTransporte.Reboque.codEntity(codigosReboques[0]);
        _gestaoDadosColetaDadosTransporte.Reboque.entityDescription(placasReboques[0]);
        _gestaoDadosColetaDadosTransporte.Reboque.val(placasReboques[0]);
    }

    if (_gestaoDadosColetaDadosTransporte.SegundoReboque.visible() && (codigosReboques.length > 1)) {
        _gestaoDadosColetaDadosTransporte.SegundoReboque.codEntity(codigosReboques[1]);
        _gestaoDadosColetaDadosTransporte.SegundoReboque.entityDescription(placasReboques[1]);
        _gestaoDadosColetaDadosTransporte.SegundoReboque.val(placasReboques[1]);
    }
}

function retornoConsultaGestaoDadosColetaDadosTransporteColeta(registroSelecionado) {
    _gestaoDadosColetaDadosTransporte.Coleta.codEntity(registroSelecionado.Codigo);
    _gestaoDadosColetaDadosTransporte.Coleta.entityDescription(registroSelecionado.Descricao);
    _gestaoDadosColetaDadosTransporte.Coleta.val(registroSelecionado.Descricao);

    _gestaoDadosColetaDadosTransporte.NumeroReboques.val(registroSelecionado.NumeroReboques)
    _gestaoDadosColetaDadosTransporte.ExigirConfirmacaoTracao.val(registroSelecionado.ExigePlacaTracao);
    _gestaoDadosColetaDadosTransporte.TipoVeiculo.val(registroSelecionado.ExigePlacaTracao ? "0" : "");

    controlarVisibilidadeGestaoDadosColetaDadosTransporteVeiculos();
}

function retornoConsultaGestaoDadosColetaDadosTransporteReboque(reboqueSelecionado) {
    if (_gestaoDadosColetaDadosTransporte.SegundoReboque.codEntity() == reboqueSelecionado.Codigo) {
        exibirMensagem(tipoMensagem.atencao, "Veículo (Carreta 1)", "Não é possível selecionar duas carretas iguais.");

        LimparCampoEntity(_gestaoDadosColetaDadosTransporte.Reboque);
    }
    else {
        _gestaoDadosColetaDadosTransporte.Reboque.codEntity(reboqueSelecionado.Codigo);
        _gestaoDadosColetaDadosTransporte.Reboque.entityDescription(reboqueSelecionado.Placa);
        _gestaoDadosColetaDadosTransporte.Reboque.val(reboqueSelecionado.Placa);
    }
}

function retornoConsultaGestaoDadosColetaDadosTransporteSegundoReboque(reboqueSelecionado) {
    if (_gestaoDadosColetaDadosTransporte.Reboque.codEntity() == reboqueSelecionado.Codigo) {
        exibirMensagem(tipoMensagem.atencao, "Veículo (Carreta 1)", "Não é possível selecionar duas carretas iguais.");

        LimparCampoEntity(_gestaoDadosColetaDadosTransporte.SegundoReboque);
    }
    else {
        _gestaoDadosColetaDadosTransporte.SegundoReboque.codEntity(reboqueSelecionado.Codigo);
        _gestaoDadosColetaDadosTransporte.SegundoReboque.entityDescription(reboqueSelecionado.Placa);
        _gestaoDadosColetaDadosTransporte.SegundoReboque.val(reboqueSelecionado.Placa);
    }
}

function retornoConsultaGestaoDadosColetaDadosTransporteVeiculo(veiculoSelecionado) {
    _gestaoDadosColetaDadosTransporte.Veiculo.codEntity(veiculoSelecionado.Codigo);

    if (_gestaoDadosColetaDadosTransporte.ExigirConfirmacaoTracao.val()) {
        _gestaoDadosColetaDadosTransporte.Veiculo.entityDescription(veiculoSelecionado.Placa);
        _gestaoDadosColetaDadosTransporte.Veiculo.val(veiculoSelecionado.Placa);
    }
    else {
        _gestaoDadosColetaDadosTransporte.Veiculo.entityDescription(veiculoSelecionado.ConjuntoPlacasSemModeloVeicular);
        _gestaoDadosColetaDadosTransporte.Veiculo.val(veiculoSelecionado.ConjuntoPlacasSemModeloVeicular);
    }

    Global.setarFocoProximoCampo(_gestaoDadosColetaDadosTransporte.Veiculo.id);

    preencherGestaoDadosColetaDadosTransporteReboquesPorVeiculoSelecionado(veiculoSelecionado);
}

// #endregion Funções Privadas
