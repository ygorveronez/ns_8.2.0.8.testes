/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/AreaVeiculoPosicao.js" />
/// <reference path="../../Consultas/Carga.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Enumeradores/EnumTipoAreaVeiculo.js" />

// #region Objetos Globais do Arquivo

var _callbackAtualizacaoDadosParaTransporte;
var _configuracaoPreCargaDadosParaTransporte;
var _corPreCargaDadosParaTransporteFilaCarregamentoAnterior = "#ffcc99";
var _corPreCargaDadosParaTransporteFilaCarregamentoSelecionada = "#e6ffcc";
var _dicionarioAuditoriaPreCarga = {};
var _gridPreCargaDadosParaTransporteFilaCarregamento;
var _gridPreCargaDadosParaTransporteFilaCarregamentoMotorista;
var _permissoesPersonalizadasPreCargaDadosParaTransporte;
var _preCargaDadosParaTransporte;

// #endregion Objetos Globais do Arquivo

// #region Classes

var PreCargaDadosParaTransporte = function () {
    var campoTransportadorObrigatorio = _configuracaoPreCargaDadosParaTransporte.TransportadorObrigatorioPreCarga || _configuracaoPreCargaDadosParaTransporte.DadosTransporteObrigatorioPreCarga;

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.PossuiCarga = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.CargaPodeSerModificada = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.ExibirFilaCarregamentoMotorista = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.ExibirFilaCarregamentoVeiculo = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.UtilizarMotoristaJornadaExcedida = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.ExigirConfirmacaoTracao = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.NumeroReboques = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.TipoVeiculo = PropertyEntity({ val: ko.observable(""), visible: false });

    this.BuscaRemetentes = PropertyEntity({ val: ko.observable("") });
    this.BuscaDestinatarios = PropertyEntity({ val: ko.observable("") });

    this.NumeroPreCarga = PropertyEntity({ val: ko.observable(""), text: "Número: " });
    this.ObservacaoPreCarga = PropertyEntity({ val: ko.observable(""), text: "Observação: ", visible: ko.observable() });
    this.MotivoAlteracaoData = PropertyEntity({ val: ko.observable(""), text: "Motivo Alteração Data: " });
    this.MotivoCancelamento = PropertyEntity({ val: ko.observable(""), text: "Motivo Cancelamento: " });
    this.Filial = PropertyEntity({ text: "Filial: ", type: types.entity, codEntity: ko.observable(0) });
    this.Pedidos = PropertyEntity({ val: ko.observable(""), text: "Pedidos: ", visible: !_configuracaoPreCargaDadosParaTransporte.UtilizarProgramacaoCarga });
    this.ModeloVeicular = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), val: ko.observable(""), text: "Modelo Veicular: " });
    this.RotaFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), val: ko.observable(""), text: "Rota: " });
    this.TipoCarga = PropertyEntity({ val: ko.observable(""), text: "Tipo de Carga: " });
    this.TipoOperacao = PropertyEntity({ val: ko.observable(""), text: "Tipo de Operação: " });
    this.ConfiguracaoProgramacaoCarga = PropertyEntity({ val: ko.observable(""), text: "Configuração de Pré Planejamento: ", visible: _configuracaoPreCargaDadosParaTransporte.UtilizarProgramacaoCarga });
    this.FaixaTemperatura = PropertyEntity({ text: "Faixa de Temperatura: ", val: ko.observable(true), options: ko.observable([]), visible: !_configuracaoPreCargaDadosParaTransporte.UtilizarProgramacaoCarga });
    this.CidadesDestino = PropertyEntity({ text: "CIdades de Destino: ", visible: _configuracaoPreCargaDadosParaTransporte.UtilizarProgramacaoCarga });
    this.EstadosDestino = PropertyEntity({ text: "Estados de Destino: ", visible: _configuracaoPreCargaDadosParaTransporte.UtilizarProgramacaoCarga });
    this.RegioesDestino = PropertyEntity({ text: "Regiões de Destino: ", visible: _configuracaoPreCargaDadosParaTransporte.UtilizarProgramacaoCarga });
    this.Remetente = PropertyEntity({ val: ko.observable(""), text: "Origem: ", codEntity: ko.observable(0), visible: !_configuracaoPreCargaDadosParaTransporte.UtilizarProgramacaoCarga });
    this.Destinatario = PropertyEntity({ val: ko.observable(""), text: "Destino: ", codEntity: ko.observable(0), visible: !_configuracaoPreCargaDadosParaTransporte.UtilizarProgramacaoCarga });
    this.Origem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), val: ko.observable(""), visible: !_configuracaoPreCargaDadosParaTransporte.UtilizarProgramacaoCarga });
    this.Destino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), val: ko.observable(""), visible: !_configuracaoPreCargaDadosParaTransporte.UtilizarProgramacaoCarga });

    this.Peso = PropertyEntity({ val: ko.observable(""), text: "Peso: ", visible: !_configuracaoPreCargaDadosParaTransporte.UtilizarProgramacaoCarga });
    this.PesoPlanejamento = PropertyEntity({ val: ko.observable(""), text: "Peso do Planejamento: ", visible: ko.observable(false) });
    this.QuantidadePallets = PropertyEntity({ val: ko.observable(""), text: "Quantidade de Pallets: ", visible: ko.observable(false) });
    this.DataInclusao = PropertyEntity({ val: ko.observable(""), text: "Data de Inclusão do PP: ", visible: ko.observable(false) });
    this.Entregas = PropertyEntity({ val: ko.observable(""), text: "Entregas: ", visible: !_configuracaoPreCargaDadosParaTransporte.UtilizarProgramacaoCarga });
    this.DataPrevisaoEntrega = PropertyEntity({ val: ko.observable(""), text: "Data de Pré Planejamento: ", visible: _configuracaoPreCargaDadosParaTransporte.UtilizarProgramacaoCarga });
    this.DataPrevisaoEntregaManual = PropertyEntity({ val: ko.observable(""), text: "Data Previsão de Entrega: ", visible: _configuracaoPreCargaDadosParaTransporte.UtilizarProgramacaoCarga });
    this.DataInicioViagem = PropertyEntity({ val: ko.observable(""), text: "Data Início de Viagem: ", visible: !_configuracaoPreCargaDadosParaTransporte.UtilizarProgramacaoCarga });
    this.PrevisaoChegadaDoca = PropertyEntity({ val: ko.observable(""), text: "Previsão Chegada no Local de Carregamento: ", visible: !_configuracaoPreCargaDadosParaTransporte.UtilizarProgramacaoCarga });
    this.DataImportacao = PropertyEntity({ val: ko.observable(""), text: "Data importação: ", visible: !_configuracaoPreCargaDadosParaTransporte.UtilizarProgramacaoCarga });
    this.DataAtualizacaoImportacao = PropertyEntity({ val: ko.observable(""), text: "Data última importação: ", visible: !_configuracaoPreCargaDadosParaTransporte.UtilizarProgramacaoCarga });
    this.CargaRetorno = PropertyEntity({ val: ko.observable(""), text: "Reconhecer Retorno: ", visible: !_configuracaoPreCargaDadosParaTransporte.UtilizarProgramacaoCarga });
    this.LocalCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: (_configuracaoPreCargaDadosParaTransporte.LocalCarregamentoObrigatorioPreCarga ? "*" : "") + "Local de Carregamento: ", idBtnSearch: guid(), required: _configuracaoPreCargaDadosParaTransporte.LocalCarregamentoObrigatorioPreCarga, visible: !_configuracaoPreCargaDadosParaTransporte.UtilizarProgramacaoCarga });
    this.ProblemaVincularCarga = PropertyEntity({ });

    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Carga: ", idBtnSearch: guid() });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: (campoTransportadorObrigatorio ? "*" : "") + "Transportador: ", label: "Transportador: ", idBtnSearch: guid(), required: campoTransportadorObrigatorio });
    this.Tracao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Veículo: "), idBtnSearch: guid(), required: _configuracaoPreCargaDadosParaTransporte.DadosTransporteObrigatorioPreCarga });
    this.Reboque = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Veículo (Carreta): "), idBtnSearch: guid(), visible: ko.observable(false), required: false });
    this.SegundoReboque = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Veículo (Carreta 2):"), idBtnSearch: guid(), visible: ko.observable(false), required: false });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: (_configuracaoPreCargaDadosParaTransporte.DadosTransporteObrigatorioPreCarga ? "*" : "") + "Motorista: ", label: "Motorista: ", idBtnSearch: guid(), required: _configuracaoPreCargaDadosParaTransporte.DadosTransporteObrigatorioPreCarga });
    this.FilaCarregamento = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, visible: false });
    this.FilaCarregamentoAnterior = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, visible: false });
    this.FilaCarregamentoMotorista = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, visible: false });

    this.Auditar = PropertyEntity({ eventClick: auditarPreCargaDadosParaTransporteClick, type: types.event, visible: ko.observable(false) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarPreCargaDadosParaTransporteClick, type: types.event, text: "Atualizar", visible: ko.observable(true) });
}

// #endregion Classes

// #region Funções de Inicialização

function loadPreCargaDadosParaTransporte() {
    loadHtmlPreCargaDadosParaTransporte(function () {
        loadConfiguracaoPreCargaDadosParaTransporte(function () {
            _preCargaDadosParaTransporte = new PreCargaDadosParaTransporte();
            KoBindings(_preCargaDadosParaTransporte, "knockoutPreCargaDadosParaTransporte");

            new BuscarTransportadores(_preCargaDadosParaTransporte.Transportador, retornoConsultaPreCargaDadosParaTransporteTransportador, null, null, null, _preCargaDadosParaTransporte.Filial);
            new BuscarVeiculos(_preCargaDadosParaTransporte.Tracao, retornoConsultaPreCargaDadosParaTransporteTracao, _preCargaDadosParaTransporte.Transportador, null, null, null, null, null, null, null, null, _preCargaDadosParaTransporte.TipoVeiculo);
            new BuscarVeiculos(_preCargaDadosParaTransporte.Reboque, retornoConsultaPreCargaDadosParaTransporteReboque, _preCargaDadosParaTransporte.Transportador, null, null, null, null, null, null, null, null, "1");
            new BuscarVeiculos(_preCargaDadosParaTransporte.SegundoReboque, retornoConsultaPreCargaDadosParaTransporteSegundoReboque, _preCargaDadosParaTransporte.Transportador, null, null, null, null, null, null, null, null, "1");
            new BuscarMotoristas(_preCargaDadosParaTransporte.Motorista, retornoConsultaPreCargaDadosParaTransporteMotorista, ((_configuracaoPreCargaDadosParaTransporte.DadosTransporteObrigatorioPreCarga || _configuracaoPreCargaDadosParaTransporte.UtilizarProgramacaoCarga) ? _preCargaDadosParaTransporte.Transportador : undefined));
            new BuscarCargaParaPreCarga(_preCargaDadosParaTransporte.Carga, retornoConsultaPreCargaDadosParaTransporteCarga, _preCargaDadosParaTransporte.BuscaRemetentes, _preCargaDadosParaTransporte.BuscaDestinatarios, _preCargaDadosParaTransporte.Filial);
            new BuscarAreaVeiculoPosicao(_preCargaDadosParaTransporte.LocalCarregamento, null, null, _preCargaDadosParaTransporte.Codigo, null, EnumTipoAreaVeiculo.Doca);

            _dicionarioAuditoriaPreCarga = {
                VeiculosVinculados: "Reboque",
                JustificativaCancelamento: "Justificativa",
                SituacaoPreCarga: "Situação",
                UsuarioCancelamento: "Usuário do Cancelamento",
            };

            loadGridPreCargaDadosParaTransporteFilaCarregamento();
            loadGridPreCargaDadosParaTransporteFilaCarregamentoMotorista();

            if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.PreCarga_Supervisor, _permissoesPersonalizadasPreCargaDadosParaTransporte)) {
                _preCargaDadosParaTransporte.Auditar.visible(true);
                HeaderAuditoria("PreCarga", _preCargaDadosParaTransporte, null, _dicionarioAuditoriaPreCarga, false, true);
            }
        });
    });
}

function loadConfiguracaoPreCargaDadosParaTransporte(callback) {
    executarReST("PreCarga/ObterConfiguracaoDadosParaTransporte", undefined, function (retorno) {
        if (retorno.Success) {
            _configuracaoPreCargaDadosParaTransporte = retorno.Data.Configuracoes;
            _permissoesPersonalizadasPreCargaDadosParaTransporte = retorno.Data.PermissoesPersonalizadas;

            callback();
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function loadGridPreCargaDadosParaTransporteFilaCarregamento() {
    var limiteRegistros = 6;
    var totalRegistrosPorPagina = 6;

    _gridPreCargaDadosParaTransporteFilaCarregamento = new GridView("grid-pre-carga-dados-transporte-fila-carregamento", "FilaCarregamento/PesquisaPreCarga", _preCargaDadosParaTransporte, null, null, totalRegistrosPorPagina, null, false, false, undefined, limiteRegistros, undefined, undefined, undefined, undefined, callbackRowGridPreCargaDadosParaTransporteFilaCarregamento);
    _gridPreCargaDadosParaTransporteFilaCarregamento.CarregarGrid();
}

function loadGridPreCargaDadosParaTransporteFilaCarregamentoMotorista() {
    var limiteRegistros = 6;
    var totalRegistrosPorPagina = 6;

    _gridPreCargaDadosParaTransporteFilaCarregamentoMotorista = new GridView("grid-pre-carga-dados-transporte-fila-carregamento-motorista", "FilaCarregamento/PesquisaPreCargaMotorista", _preCargaDadosParaTransporte, null, null, totalRegistrosPorPagina, null, false, false, undefined, limiteRegistros, undefined, undefined, undefined, undefined, callbackRowGridPreCargaDadosParaTransporteFilaCarregamentoMotorista);
    _gridPreCargaDadosParaTransporteFilaCarregamentoMotorista.CarregarGrid();
}

function loadHtmlPreCargaDadosParaTransporte(callback) {
    $.get("Content/Static/PreCargas/DadosParaTransporte.html?dyn=" + guid(), function (data) {
        var $containerModalPreCargaDadosParaTransporte = $("#containerModalPreCargaDadosParaTransporte");

        if ($containerModalPreCargaDadosParaTransporte.length == 0) {
            $("#widget-grid").append("<div id='containerModalPreCargaDadosParaTransporte'></div>");
            $containerModalPreCargaDadosParaTransporte = $("#containerModalPreCargaDadosParaTransporte");
        }

        $containerModalPreCargaDadosParaTransporte.html(data);
        callback();
    });
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function auditarPreCargaDadosParaTransporteClick() {
    var exibirAuditoriaPreCarga = OpcaoAuditoria("PreCarga", null, _preCargaDadosParaTransporte, _dicionarioAuditoriaPreCarga);

    exibirAuditoriaPreCarga({ Codigo: _preCargaDadosParaTransporte.Codigo.val() });
}

function atualizarPreCargaDadosParaTransporteClick(e, sender) {
    _preCargaDadosParaTransporte.UtilizarMotoristaJornadaExcedida.val(false);

    atualizarPreCargaDadosParaTransporte(sender);
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function exibirPreCargaDadosParaTransporte(codigoPreCarga, callbackAtualizacao) {
    _preCargaDadosParaTransporte.Codigo.val(codigoPreCarga);
    _callbackAtualizacaoDadosParaTransporte = callbackAtualizacao;

    BuscarPorCodigo(_preCargaDadosParaTransporte, "PreCarga/BuscarDadosParaTransporte", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                if (retorno.Data.ExibirFilaCarregamentoMotorista)
                    _gridPreCargaDadosParaTransporteFilaCarregamentoMotorista.CarregarGrid();

                if (retorno.Data.ExibirFilaCarregamentoVeiculo)
                    _gridPreCargaDadosParaTransporteFilaCarregamento.CarregarGrid();

                _preCargaDadosParaTransporte.FaixaTemperatura.options([{ text: "", value: "0"}].concat(retorno.Data.FaixasTemperatura));
                _preCargaDadosParaTransporte.Tracao.Empresa = retorno.Data.EmpresaTracao;
                _preCargaDadosParaTransporte.Reboque.Empresa = retorno.Data.EmpresaReboque;
                _preCargaDadosParaTransporte.SegundoReboque.Empresa = retorno.Data.EmpresaSegundoReboque;
                _preCargaDadosParaTransporte.Motorista.Empresa = retorno.Data.EmpresaMotorista
                _preCargaDadosParaTransporte.FaixaTemperatura.val(retorno.Data.FaixaTemperatura.Codigo.toString());
                _preCargaDadosParaTransporte.TipoVeiculo.val(retorno.Data.ExigirConfirmacaoTracao ? "0" : "");

                controlarExibicaoCamposPreCargaDadosParaTransporteVeiculo();
                exibirModalPreCargaDadosParaTransporte();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

// #endregion Funções Públicas

// #region Funções Privadas

function atualizarPreCargaDadosParaTransporte(sender) {
    Salvar(_preCargaDadosParaTransporte, "PreCarga/AtualizarDadosParaTransporte", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                if (retorno.Data.JornadaExcedida) {
                    exibirConfirmacao("Confirmação", retorno.Data.Mensagem + " Deseja realmente utilizar este motorista?", function () {
                        _preCargaDadosParaTransporte.UtilizarMotoristaJornadaExcedida.val(true);
                        atualizarPreCargaDadosParaTransporte(sender);
                    });
                }
                else {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                    fecharModalPreCargaDadosParaTransporte();

                    if (_callbackAtualizacaoDadosParaTransporte instanceof Function)
                        _callbackAtualizacaoDadosParaTransporte();
                }
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function callbackRowGridPreCargaDadosParaTransporteFilaCarregamento(row, data) {
    if (data.Codigo == _preCargaDadosParaTransporte.FilaCarregamento.val()) {
        $(row).css({
            "background-color": _corPreCargaDadosParaTransporteFilaCarregamentoSelecionada,
            "color": "#212529"
        });
    }

    if (data.Codigo == _preCargaDadosParaTransporte.FilaCarregamentoAnterior.val()) {
        $(row).css({
            "background-color": _corPreCargaDadosParaTransporteFilaCarregamentoAnterior,
            "color": "#212529"
        });
    }

    $(row).click(function () {
        selecionarPreCargaDadosParaTransporteFilaCarregamento(data);
    });
}

function callbackRowGridPreCargaDadosParaTransporteFilaCarregamentoMotorista(row, data) {
    if (data.Codigo == _preCargaDadosParaTransporte.FilaCarregamentoMotorista.val()) {
        $(row).css({
            "background-color": _corPreCargaDadosParaTransporteFilaCarregamentoSelecionada,
            "color": "#212529"
        });
    }

    $(row).click(function () {
        selecionarPreCargaDadosParaTransporteFilaCarregamentoMotorista(data);
    });
}

function controlarExibicaoCamposPreCargaDadosParaTransporteVeiculo() {
    var cargaPodeSerModificada = _preCargaDadosParaTransporte.CargaPodeSerModificada.val();
    var reboqueVisivel = false;
    var segundoReboqueVisivel = false;

    if (_preCargaDadosParaTransporte.ExigirConfirmacaoTracao.val()) {
        reboqueVisivel = (_preCargaDadosParaTransporte.NumeroReboques.val() >= 1);
        segundoReboqueVisivel = (_preCargaDadosParaTransporte.NumeroReboques.val() > 1);
    }

    if (!reboqueVisivel)
        LimparCampoEntity(_preCargaDadosParaTransporte.Reboque);

    if (!segundoReboqueVisivel)
        LimparCampoEntity(_preCargaDadosParaTransporte.SegundoReboque);

    _preCargaDadosParaTransporte.Tracao.required = _configuracaoPreCargaDadosParaTransporte.DadosTransporteObrigatorioPreCarga;
    _preCargaDadosParaTransporte.Tracao.text((_preCargaDadosParaTransporte.Tracao.required && cargaPodeSerModificada ? "*" : "") + (reboqueVisivel ? "Tração (Cavalo):" : "Veiculo:"));

    _preCargaDadosParaTransporte.Reboque.required = (reboqueVisivel && _configuracaoPreCargaDadosParaTransporte.DadosTransporteObrigatorioPreCarga);
    _preCargaDadosParaTransporte.Reboque.text((_preCargaDadosParaTransporte.Reboque.required && cargaPodeSerModificada ? "*" : "") + (segundoReboqueVisivel ? "Veículo (Carreta 1):" : "Veículo (Carreta):"));
    _preCargaDadosParaTransporte.Reboque.visible(reboqueVisivel);

    _preCargaDadosParaTransporte.SegundoReboque.required = (segundoReboqueVisivel && _configuracaoPreCargaDadosParaTransporte.DadosTransporteObrigatorioPreCarga);
    _preCargaDadosParaTransporte.SegundoReboque.text((_preCargaDadosParaTransporte.SegundoReboque.required && cargaPodeSerModificada ? "*" : "") + "Veículo (Carreta 2):");
    _preCargaDadosParaTransporte.SegundoReboque.visible(segundoReboqueVisivel);
}

function exibirModalPreCargaDadosParaTransporte() {
    Global.abrirModal('divModalPreCargaDadosParaTransporte');
    $("#divModalPreCargaDadosParaTransporte").one('hidden.bs.modal', function () {
        limparCamposPreCargaDadosParaTransporte();
    });
}

function fecharModalPreCargaDadosParaTransporte() {
    Global.fecharModal('divModalPreCargaDadosParaTransporte');
}

function limparCamposPreCargaDadosParaTransporte() {
    LimparCampos(_preCargaDadosParaTransporte);

    _preCargaDadosParaTransporte.Tracao.Empresa = 0;
    _preCargaDadosParaTransporte.Reboque.Empresa = 0;
    _preCargaDadosParaTransporte.SegundoReboque.Empresa = 0;
    _preCargaDadosParaTransporte.Motorista.Empresa = 0;

    _gridPreCargaDadosParaTransporteFilaCarregamento.CarregarGrid();
    _gridPreCargaDadosParaTransporteFilaCarregamentoMotorista.CarregarGrid();
}

function preencherRetornoSelecaoPreCargaDadosParaTransporteCarga(data) {
    _preCargaDadosParaTransporte.Carga.codEntity(data.Codigo);
    _preCargaDadosParaTransporte.Carga.val(data.Descricao);

    if (!_CONFIGURACAO_TMS.PermiteEmissaoCargaSomenteComTracao && _preCargaDadosParaTransporte.ExigirConfirmacaoTracao.val() && (data.CodigoTracao > 0) && (data.TipoVeiculoTracao == "1")) {
        data.CodigoReboque = data.CodigoTracao;
        data.DescricaoReboque = data.DescricaoTracao;
        data.CodigoEmpresaReboque = data.CodigoEmpresaTracao;

        data.CodigoTracao = "0";
        data.DescricaoTracao = "";
        data.CodigoEmpresaTracao = "0";

        data.CodigoSegundoReboque = "0";
        data.DescricaoSegundoReboque = "";
        data.CodigoEmpresaSegundoReboque = "0";
    }

    if (_preCargaDadosParaTransporte.Transportador.codEntity() == 0 && data.CodigoTransportador > 0) {
        _preCargaDadosParaTransporte.Transportador.codEntity(data.CodigoTransportador);
        _preCargaDadosParaTransporte.Transportador.entityDescription(data.Transportador);
        _preCargaDadosParaTransporte.Transportador.val(data.Transportador);
    }

    if (_preCargaDadosParaTransporte.Tracao.codEntity() == 0 && data.CodigoTracao > 0) {
        _preCargaDadosParaTransporte.Tracao.codEntity(data.CodigoTracao);
        _preCargaDadosParaTransporte.Tracao.entityDescription(data.DescricaoTracao);
        _preCargaDadosParaTransporte.Tracao.val(data.DescricaoTracao);
        _preCargaDadosParaTransporte.Tracao.Empresa = data.CodigoEmpresaTracao;
    }
    else if (_preCargaDadosParaTransporte.Tracao.Empresa > 0 && data.CodigoTransportador > 0 && _preCargaDadosParaTransporte.Tracao.Empresa != data.CodigoTransportador) {
        LimparCampoEntity(_preCargaDadosParaTransporte.Tracao);
        _preCargaDadosParaTransporte.Tracao.Empresa = 0;
    }

    if (_preCargaDadosParaTransporte.Reboque.codEntity() == 0 && data.CodigoReboque > 0) {
        _preCargaDadosParaTransporte.Reboque.codEntity(data.CodigoReboque);
        _preCargaDadosParaTransporte.Reboque.entityDescription(data.DescricaoReboque);
        _preCargaDadosParaTransporte.Reboque.val(data.DescricaoReboque);
        _preCargaDadosParaTransporte.Reboque.Empresa = data.CodigoEmpresaReboque;
    }
    else if (_preCargaDadosParaTransporte.Reboque.Empresa > 0 && data.CodigoTransportador > 0 && _preCargaDadosParaTransporte.Reboque.Empresa != data.CodigoTransportador) {
        LimparCampoEntity(_preCargaDadosParaTransporte.Reboque);
        _preCargaDadosParaTransporte.Reboque.Empresa = 0;
    }

    if (_preCargaDadosParaTransporte.SegundoReboque.codEntity() == 0 && data.CodigoSegundoReboque > 0) {
        _preCargaDadosParaTransporte.SegundoReboque.codEntity(data.CodigoSegundoReboque);
        _preCargaDadosParaTransporte.SegundoReboque.entityDescription(data.DescricaoSegundoReboque);
        _preCargaDadosParaTransporte.SegundoReboque.val(data.DescricaoSegundoReboque);
        _preCargaDadosParaTransporte.SegundoReboque.Empresa = data.CodigoEmpresaSegundoReboque;
    }
    else if (_preCargaDadosParaTransporte.SegundoReboque.Empresa > 0 && data.CodigoTransportador > 0 && _preCargaDadosParaTransporte.SegundoReboque.Empresa != data.CodigoTransportador) {
        LimparCampoEntity(_preCargaDadosParaTransporte.Reboque);
        _preCargaDadosParaTransporte.Reboque.Empresa = 0;
    }

    if (_configuracaoPreCargaDadosParaTransporte.DadosTransporteObrigatorioPreCarga && _preCargaDadosParaTransporte.Motorista.Empresa > 0 && data.CodigoTransportador > 0 && _preCargaDadosParaTransporte.Motorista.Empresa != data.CodigoTransportador) {
        LimparCampoEntity(_preCargaDadosParaTransporte.Motorista);
        _preCargaDadosParaTransporte.Motorista.Empresa = 0;
    }

    if (_preCargaDadosParaTransporte.FaixaTemperatura.val() == 0) 
        _preCargaDadosParaTransporte.FaixaTemperatura.val(data.CodigoFaixaTemperatura);

    if (_preCargaDadosParaTransporte.Motorista.codEntity() == 0) {
        _preCargaDadosParaTransporte.Motorista.codEntity(data.CodigoMotorista);
        _preCargaDadosParaTransporte.Motorista.val(data.DescricaoMotorista);
    }
    
}

function preencherPreCargaDadosParaTransporteMotoristaPorVeiculoSelecionado(veiculoSelecionado) {
    if (!_configuracaoPreCargaDadosParaTransporte.DadosTransporteObrigatorioPreCarga && _preCargaDadosParaTransporte.Motorista.codEntity() == 0 && veiculoSelecionado.CodigoMotorista > 0) {
        _preCargaDadosParaTransporte.Motorista.codEntity(veiculoSelecionado.CodigoMotorista);
        _preCargaDadosParaTransporte.Motorista.entityDescription(veiculoSelecionado.Motorista);
        _preCargaDadosParaTransporte.Motorista.val(veiculoSelecionado.Motorista);
    }
}

function preencherPreCargaDadosParaTransporteReboquesPorVeiculoSelecionado(veiculoSelecionado) {
    if (!_preCargaDadosParaTransporte.ExigirConfirmacaoTracao.val())
        return;

    if (!veiculoSelecionado.CodigosVeiculosVinculados)
        return;

    var codigosReboques = veiculoSelecionado.CodigosVeiculosVinculados.split(", ");
    var placasReboques = veiculoSelecionado.VeiculosVinculados.split(", ");

    if (_preCargaDadosParaTransporte.Reboque.visible()) {
        _preCargaDadosParaTransporte.Reboque.codEntity(codigosReboques[0]);
        _preCargaDadosParaTransporte.Reboque.entityDescription(placasReboques[0]);
        _preCargaDadosParaTransporte.Reboque.val(placasReboques[0]);
        _preCargaDadosParaTransporte.Reboque.Empresa = veiculoSelecionado.CodigoEmpresa;
    }

    if (_preCargaDadosParaTransporte.SegundoReboque.visible() && (codigosReboques.length > 1)) {
        _preCargaDadosParaTransporte.SegundoReboque.codEntity(codigosReboques[1]);
        _preCargaDadosParaTransporte.SegundoReboque.entityDescription(placasReboques[1]);
        _preCargaDadosParaTransporte.SegundoReboque.val(placasReboques[1]);
        _preCargaDadosParaTransporte.SegundoReboque.Empresa = veiculoSelecionado.CodigoEmpresa;
    }
}

function preencherPreCargaDadosParaTransporteTransportadorPorVeiculoSelecionado(veiculoSelecionado) {
    if (!_configuracaoPreCargaDadosParaTransporte.DadosTransporteObrigatorioPreCarga && _preCargaDadosParaTransporte.Transportador.codEntity() == 0 && veiculoSelecionado.CodigoEmpresa > 0) {
        _preCargaDadosParaTransporte.Transportador.codEntity(veiculoSelecionado.CodigoEmpresa);
        _preCargaDadosParaTransporte.Transportador.entityDescription(veiculoSelecionado.Empresa);
        _preCargaDadosParaTransporte.Transportador.val(veiculoSelecionado.Empresa);
    }
}

function retornoConsultaPreCargaDadosParaTransporteCarga(data) {
    var obj = {
        Carga: data.Codigo,
        Remetentes: _preCargaDadosParaTransporte.BuscaRemetentes.val(),
        Destinatarios: _preCargaDadosParaTransporte.BuscaDestinatarios.val(),
    };

    executarReST("PreCarga/ValidaCargaSelecionada", obj, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false)
                preencherRetornoSelecaoPreCargaDadosParaTransporteCarga(data);
            else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                LimparCampoEntity(_preCargaDadosParaTransporte.Carga);
            }
        }
        else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            LimparCampoEntity(_preCargaDadosParaTransporte.Carga);
        }
    }, null);
}

function retornoConsultaPreCargaDadosParaTransporteMotorista(motoristaSelecionado) {
    _preCargaDadosParaTransporte.Motorista.codEntity(motoristaSelecionado.Codigo);
    _preCargaDadosParaTransporte.Motorista.entityDescription(motoristaSelecionado.Descricao);
    _preCargaDadosParaTransporte.Motorista.val(motoristaSelecionado.Descricao);
    _preCargaDadosParaTransporte.Motorista.Empresa = motoristaSelecionado.CodigoEmpresa;
}

function retornoConsultaPreCargaDadosParaTransporteReboque(reboqueSelecionado) {
    if (_preCargaDadosParaTransporte.SegundoReboque.codEntity() == reboqueSelecionado.Codigo) {
        exibirMensagem(tipoMensagem.atencao, "Veículo (Carreta 1)", "Não é possível selecionar duas carretas iguais.");

        LimparCampoEntity(_preCargaDadosParaTransporte.Reboque);
    }
    else {
        _preCargaDadosParaTransporte.Reboque.codEntity(reboqueSelecionado.Codigo);
        _preCargaDadosParaTransporte.Reboque.entityDescription(reboqueSelecionado.Placa);
        _preCargaDadosParaTransporte.Reboque.val(reboqueSelecionado.Placa);
        _preCargaDadosParaTransporte.Reboque.Empresa = reboqueSelecionado.CodigoEmpresa;

        preencherPreCargaDadosParaTransporteTransportadorPorVeiculoSelecionado(reboqueSelecionado);
    }
}

function retornoConsultaPreCargaDadosParaTransporteSegundoReboque(reboqueSelecionado) {
    if (_preCargaDadosParaTransporte.Reboque.codEntity() == reboqueSelecionado.Codigo) {
        exibirMensagem(tipoMensagem.atencao, "Veículo (Carreta 1)", "Não é possível selecionar duas carretas iguais.");

        LimparCampoEntity(_preCargaDadosParaTransporte.SegundoReboque);
    }
    else {
        _preCargaDadosParaTransporte.SegundoReboque.codEntity(reboqueSelecionado.Codigo);
        _preCargaDadosParaTransporte.SegundoReboque.entityDescription(reboqueSelecionado.Placa);
        _preCargaDadosParaTransporte.SegundoReboque.val(reboqueSelecionado.Placa);
        _preCargaDadosParaTransporte.SegundoReboque.Empresa = reboqueSelecionado.CodigoEmpresa;

        preencherPreCargaDadosParaTransporteTransportadorPorVeiculoSelecionado(reboqueSelecionado);
    }
}

function retornoConsultaPreCargaDadosParaTransporteTracao(veiculoSelecionado) {
    _preCargaDadosParaTransporte.Tracao.codEntity(veiculoSelecionado.Codigo);
    _preCargaDadosParaTransporte.Tracao.Empresa = veiculoSelecionado.CodigoEmpresa;

    if (_preCargaDadosParaTransporte.ExigirConfirmacaoTracao.val()) {
        _preCargaDadosParaTransporte.Tracao.entityDescription(veiculoSelecionado.Placa);
        _preCargaDadosParaTransporte.Tracao.val(veiculoSelecionado.Placa);
    }
    else {
        _preCargaDadosParaTransporte.Tracao.entityDescription(veiculoSelecionado.ConjuntoPlacasSemModeloVeicular);
        _preCargaDadosParaTransporte.Tracao.val(veiculoSelecionado.ConjuntoPlacasSemModeloVeicular);
    }

    Global.setarFocoProximoCampo(_preCargaDadosParaTransporte.Tracao.id);

    preencherPreCargaDadosParaTransporteTransportadorPorVeiculoSelecionado(veiculoSelecionado);
    preencherPreCargaDadosParaTransporteMotoristaPorVeiculoSelecionado(veiculoSelecionado);
    preencherPreCargaDadosParaTransporteReboquesPorVeiculoSelecionado(veiculoSelecionado);
}

function retornoConsultaPreCargaDadosParaTransporteTransportador(transportadorSelecionado) {
    _preCargaDadosParaTransporte.Transportador.codEntity(transportadorSelecionado.Codigo);
    _preCargaDadosParaTransporte.Transportador.entityDescription(transportadorSelecionado.Descricao);
    _preCargaDadosParaTransporte.Transportador.val(transportadorSelecionado.Descricao);

    if (_preCargaDadosParaTransporte.Tracao.codEntity() > 0 && _preCargaDadosParaTransporte.Tracao.Empresa != transportadorSelecionado.Codigo) {
        LimparCampoEntity(_preCargaDadosParaTransporte.Tracao);
        _preCargaDadosParaTransporte.Tracao.Empresa = 0;
    }

    if (_preCargaDadosParaTransporte.Reboque.codEntity() > 0 && _preCargaDadosParaTransporte.Reboque.Empresa != transportadorSelecionado.Codigo) {
        LimparCampoEntity(_preCargaDadosParaTransporte.Reboque);
        _preCargaDadosParaTransporte.Reboque.Empresa = 0;
    }

    if (_preCargaDadosParaTransporte.SegundoReboque.codEntity() > 0 && _preCargaDadosParaTransporte.SegundoReboque.Empresa != transportadorSelecionado.Codigo) {
        LimparCampoEntity(_preCargaDadosParaTransporte.SegundoReboque);
        _preCargaDadosParaTransporte.SegundoReboque.Empresa = 0;
    }

    if (_configuracaoPreCargaDadosParaTransporte.DadosTransporteObrigatorioPreCarga && _preCargaDadosParaTransporte.Motorista.codEntity() > 0 && _preCargaDadosParaTransporte.Motorista.Empresa != transportadorSelecionado.Codigo) {
        LimparCampoEntity(_preCargaDadosParaTransporte.Motorista);
        _preCargaDadosParaTransporte.Motorista.Empresa = 0;
    }
}

function selecionarPreCargaDadosParaTransporteFilaCarregamento(filaCarregamentoSelecionada) {
    $("#grid-pre-carga-dados-transporte-fila-carregamento tbody #" + _preCargaDadosParaTransporte.FilaCarregamento.val()).css({
        "background-color": "",
        "color": ""
    });

    if (_preCargaDadosParaTransporte.FilaCarregamento.val() == filaCarregamentoSelecionada.Codigo) {
        _preCargaDadosParaTransporte.FilaCarregamento.val(0);

        if (filaCarregamentoSelecionada.CodigoEmpresa > 0)
            LimparCampoEntity(_preCargaDadosParaTransporte.Transportador);

        if (filaCarregamentoSelecionada.CodigoMotorista > 0) {
            LimparCampoEntity(_preCargaDadosParaTransporte.Motorista);
            _preCargaDadosParaTransporte.Motorista.Empresa = 0;
        }

        if (filaCarregamentoSelecionada.CodigoReboque > 0) {
            LimparCampoEntity(_preCargaDadosParaTransporte.Reboque);
            _preCargaDadosParaTransporte.Reboque.Empresa = 0;
        }

        if (filaCarregamentoSelecionada.CodigoSegundoReboque > 0) {
            LimparCampoEntity(_preCargaDadosParaTransporte.SegundoReboque);
            _preCargaDadosParaTransporte.SegundoReboque.Empresa = 0;
        }

        if (filaCarregamentoSelecionada.CodigoTracao > 0) {
            LimparCampoEntity(_preCargaDadosParaTransporte.Tracao);
            _preCargaDadosParaTransporte.Tracao.Empresa = 0;
        }
    }
    else {
        _preCargaDadosParaTransporte.FilaCarregamento.val(filaCarregamentoSelecionada.Codigo);

        if (filaCarregamentoSelecionada.CodigoEmpresa > 0) {
            _preCargaDadosParaTransporte.Transportador.codEntity(filaCarregamentoSelecionada.CodigoEmpresa);
            _preCargaDadosParaTransporte.Transportador.entityDescription(filaCarregamentoSelecionada.DescricaoEmpresa);
            _preCargaDadosParaTransporte.Transportador.val(filaCarregamentoSelecionada.DescricaoEmpresa);
        }

        if (filaCarregamentoSelecionada.CodigoMotorista > 0) {
            _preCargaDadosParaTransporte.Motorista.codEntity(filaCarregamentoSelecionada.CodigoMotorista);
            _preCargaDadosParaTransporte.Motorista.entityDescription(filaCarregamentoSelecionada.Motorista);
            _preCargaDadosParaTransporte.Motorista.val(filaCarregamentoSelecionada.Motorista);
            _preCargaDadosParaTransporte.Motorista.Empresa = filaCarregamentoSelecionada.CodigoEmpresaMotorista;
        }

        if (filaCarregamentoSelecionada.CodigoReboque > 0) {
            _preCargaDadosParaTransporte.Reboque.codEntity(filaCarregamentoSelecionada.CodigoReboque);
            _preCargaDadosParaTransporte.Reboque.entityDescription(filaCarregamentoSelecionada.DescricaoReboque);
            _preCargaDadosParaTransporte.Reboque.val(filaCarregamentoSelecionada.DescricaoReboque);
            _preCargaDadosParaTransporte.Reboque.Empresa = filaCarregamentoSelecionada.CodigoEmpresaReboque;
        }

        if (filaCarregamentoSelecionada.CodigoSegundoReboque > 0) {
            _preCargaDadosParaTransporte.SegundoReboque.codEntity(filaCarregamentoSelecionada.CodigoSegundoReboque);
            _preCargaDadosParaTransporte.SegundoReboque.entityDescription(filaCarregamentoSelecionada.DescricaoSegundoReboque);
            _preCargaDadosParaTransporte.SegundoReboque.val(filaCarregamentoSelecionada.DescricaoSegundoReboque);
            _preCargaDadosParaTransporte.SegundoReboque.Empresa = filaCarregamentoSelecionada.CodigoEmpresaSegundoReboque;
        }

        if (filaCarregamentoSelecionada.CodigoTracao > 0) {
            _preCargaDadosParaTransporte.Tracao.codEntity(filaCarregamentoSelecionada.CodigoTracao);
            _preCargaDadosParaTransporte.Tracao.entityDescription(filaCarregamentoSelecionada.Tracao);
            _preCargaDadosParaTransporte.Tracao.val(filaCarregamentoSelecionada.Tracao);
            _preCargaDadosParaTransporte.Tracao.Empresa = filaCarregamentoSelecionada.CodigoEmpresaTracao;
        }

        $("#grid-pre-carga-dados-transporte-fila-carregamento tbody #" + filaCarregamentoSelecionada.Codigo).css({
            "background-color": (_preCargaDadosParaTransporte.FilaCarregamentoAnterior.val() == filaCarregamentoSelecionada.Codigo ? _corPreCargaDadosParaTransporteFilaCarregamentoAnterior : _corPreCargaDadosParaTransporteFilaCarregamentoSelecionada),
            "color": "#212529"
        });
    }
}

function selecionarPreCargaDadosParaTransporteFilaCarregamentoMotorista(filaCarregamentoSelecionadaMotorista) {
    $("#grid-pre-carga-dados-transporte-fila-carregamento-motorista tbody #" + _preCargaDadosParaTransporte.FilaCarregamentoMotorista.val()).css({
        "background-color": "",
        "color": ""
    });

    if (_preCargaDadosParaTransporte.FilaCarregamentoMotorista.val() == filaCarregamentoSelecionadaMotorista.Codigo) {
        _preCargaDadosParaTransporte.FilaCarregamentoMotorista.val(0);

        if (filaCarregamentoSelecionadaMotorista.CodigoEmpresaMotorista > 0)
            LimparCampoEntity(_preCargaDadosParaTransporte.Transportador);

        if (filaCarregamentoSelecionadaMotorista.CodigoMotorista > 0) {
            LimparCampoEntity(_preCargaDadosParaTransporte.Motorista);
            _preCargaDadosParaTransporte.Motorista.Empresa = 0;
        }

        if (filaCarregamentoSelecionadaMotorista.CodigoTracao > 0) {
            LimparCampoEntity(_preCargaDadosParaTransporte.Tracao);
            _preCargaDadosParaTransporte.Motorista.Tracao = 0;
        }
    }
    else {
        _preCargaDadosParaTransporte.FilaCarregamentoMotorista.val(filaCarregamentoSelecionadaMotorista.Codigo);

        if (filaCarregamentoSelecionadaMotorista.CodigoEmpresaMotorista > 0) {
            _preCargaDadosParaTransporte.Transportador.codEntity(filaCarregamentoSelecionadaMotorista.CodigoEmpresaMotorista);
            _preCargaDadosParaTransporte.Transportador.entityDescription(filaCarregamentoSelecionadaMotorista.DescricaoEmpresaMotorista);
            _preCargaDadosParaTransporte.Transportador.val(filaCarregamentoSelecionadaMotorista.DescricaoEmpresaMotorista);
        }

        if (filaCarregamentoSelecionadaMotorista.CodigoMotorista > 0) {
            _preCargaDadosParaTransporte.Motorista.codEntity(filaCarregamentoSelecionadaMotorista.CodigoMotorista);
            _preCargaDadosParaTransporte.Motorista.entityDescription(filaCarregamentoSelecionadaMotorista.Motorista);
            _preCargaDadosParaTransporte.Motorista.val(filaCarregamentoSelecionadaMotorista.Motorista);
            _preCargaDadosParaTransporte.Motorista.Empresa = filaCarregamentoSelecionadaMotorista.CodigoEmpresaMotorista;
        }

        if (filaCarregamentoSelecionadaMotorista.CodigoTracao > 0) {
            _preCargaDadosParaTransporte.Tracao.codEntity(filaCarregamentoSelecionadaMotorista.CodigoTracao);
            _preCargaDadosParaTransporte.Tracao.entityDescription(filaCarregamentoSelecionadaMotorista.Tracao);
            _preCargaDadosParaTransporte.Tracao.val(filaCarregamentoSelecionadaMotorista.Tracao);
            _preCargaDadosParaTransporte.Tracao.Empresa = filaCarregamentoSelecionadaMotorista.CodigoEmpresaTracao;
        }

        $("#grid-pre-carga-dados-transporte-fila-carregamento-motorista tbody #" + filaCarregamentoSelecionadaMotorista.Codigo).css({
            "background-color": _corPreCargaDadosParaTransporteFilaCarregamentoSelecionada,
            "color": "#212529"
        });
    }
}

// #endregion Funções Privadas
