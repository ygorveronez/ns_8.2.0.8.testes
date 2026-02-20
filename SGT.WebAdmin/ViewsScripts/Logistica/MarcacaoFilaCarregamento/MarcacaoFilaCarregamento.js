/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/CentroCarregamento.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/Veiculo.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _codigoMotoristaAnterior = 0;
var _codigoVeiculoAnterior = 0;
var _CRUDMarcacaoFilaCarregamento;
var _marcacaoFilaCarregamento;
var _marcacaoFilaCarregamentoDetalhesFilaCarregamento;
var _marcacaoFilaCarregamentoMotorista;
var _marcacaoFilaCarregamentoVeiculo;

/*
 * Declaração das Classes
 */

var CRUDMarcacaoFilaCarregamento = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar / Nova", visible: ko.observable(true) });
}

var MarcacaoFilaCarregamento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CentroCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Centro de Carregamento:", idBtnSearch: guid(), required: true, enable: !_CONFIGURACAO_TMS.MarcacaoFilaCarregamentoSomentePorVeiculo });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Motorista:", idBtnSearch: guid(), required: true, enable: !_CONFIGURACAO_TMS.MarcacaoFilaCarregamentoSomentePorVeiculo });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Veículo:", idBtnSearch: guid(), required: true });

    this.Motorista.codEntity.subscribe(function () {
        atualizarDadosMotorista();
    });

    this.Veiculo.codEntity.subscribe(function () {
        atualizarDadosVeiculo();
        atualizarCentroCarregamento();
    });
}

var MarcacaoFilaCarregamentoDetalhesFilaCarregamento = function () {
    this.CentroCarregamento = PropertyEntity({ text: "Centro de Carregamento: " });
    this.DataEntrada = PropertyEntity({ text: "Registrado: " });
    this.PlacaTracao = PropertyEntity({ text: "Placa Tração: ", visible: ko.observable(false) });
    this.PlacasReboques = PropertyEntity({ text: "Placas Reboques: ", visible: ko.observable(false) });
    this.Posicao = PropertyEntity({ text: "Posição: ", visible: ko.observable(false) });
}

var MarcacaoFilaCarregamentoMotorista = function () {
    this.Codigo = PropertyEntity({ text: "Cód. Motorista: ", visible: ko.observable(false) });
    this.Nome = PropertyEntity({ text: "Motorista: " });
    this.CodigoTransportador = PropertyEntity({ text: "Cód. Transportadora: " });
    this.DescricaoTransportador = PropertyEntity({ text: "Transportadora: " });
    this.FotoMotorista = PropertyEntity({ });
    this.ValidadeCnh = PropertyEntity({ text: "Validade CNH: " });
    this.ValidadeGr = PropertyEntity({ text: "Validade GR: " });
    this.StatusValidadeCnh = PropertyEntity({ text: "Status CNH: ", cssClass: ko.observable("") });
    this.StatusValidadeGr = PropertyEntity({ text: "Status GR: ", cssClass: ko.observable("") });
}

var MarcacaoFilaCarregamentoVeiculo = function () {
    this.PlacaTracao = PropertyEntity({ text: "Placa Tração: ", visible: ko.observable(false) });
    this.PlacasReboques = PropertyEntity({ text: "Placas Reboques: " });
    this.ModeloVeicular = PropertyEntity({ text: "Tipo: " });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadMarcacaoFilaCarregamento() {
    _marcacaoFilaCarregamento = new MarcacaoFilaCarregamento();
    KoBindings(_marcacaoFilaCarregamento, "knockoutMarcacaoFilaCarregamento");

    _CRUDMarcacaoFilaCarregamento = new CRUDMarcacaoFilaCarregamento();
    KoBindings(_CRUDMarcacaoFilaCarregamento, "knockoutCRUDMarcacaoFilaCarregamento");

    _marcacaoFilaCarregamentoMotorista = new MarcacaoFilaCarregamentoMotorista();
    KoBindings(_marcacaoFilaCarregamentoMotorista, "knockoutMarcacaoFilaCarregamentoMotorista");

    _marcacaoFilaCarregamentoVeiculo = new MarcacaoFilaCarregamentoVeiculo();
    KoBindings(_marcacaoFilaCarregamentoVeiculo, "knockoutMarcacaoFilaCarregamentoVeiculo");

    _marcacaoFilaCarregamentoDetalhesFilaCarregamento = new MarcacaoFilaCarregamentoDetalhesFilaCarregamento();
    KoBindings(_marcacaoFilaCarregamentoDetalhesFilaCarregamento, "knockoutDetalhesFilaCarregamento");

    new BuscarVeiculos(_marcacaoFilaCarregamento.Veiculo, null, null, null, _marcacaoFilaCarregamento.Motorista, undefined, undefined, undefined, undefined, undefined, undefined, undefined, undefined, undefined, undefined, undefined, undefined, undefined, undefined, undefined, undefined, true);

    if (!_CONFIGURACAO_TMS.MarcacaoFilaCarregamentoSomentePorVeiculo) {
        new BuscarCentrosCarregamento(_marcacaoFilaCarregamento.CentroCarregamento);
        new BuscarMotoristas(_marcacaoFilaCarregamento.Motorista);
    }
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarClick(e, sender) {
    var nomeMetodo = _CONFIGURACAO_TMS.MarcacaoFilaCarregamentoSomentePorVeiculo ? "AdicionarNaFilaPorVeiculo" : "AdicionarNaFila";

    Salvar(_marcacaoFilaCarregamento, "MarcacaoFilaCarregamento/" + nomeMetodo, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                PreencherObjetoKnout(_marcacaoFilaCarregamentoDetalhesFilaCarregamento, retorno);
                exibirModalDetalhesFilaCarregamento(retorno);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function cancelarClick() {
    limparCamposMarcacaoFilaCarregamento();
}

/*
 * Declaração das Funções
 */

function atualizarCentroCarregamento() {
    if (_marcacaoFilaCarregamento.Veiculo.codEntity() > 0) {
        executarReST("MarcacaoFilaCarregamento/ObterCentroCarregamentoPorVeiculo", { Veiculo: _marcacaoFilaCarregamento.Veiculo.codEntity() }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    _marcacaoFilaCarregamento.CentroCarregamento.val(retorno.Data.Descricao);
                    _marcacaoFilaCarregamento.CentroCarregamento.entityDescription(retorno.Data.Descricao);
                    _marcacaoFilaCarregamento.CentroCarregamento.codEntity(retorno.Data.Codigo);
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    }
}

function atualizarDadosMotorista() {
    if (_marcacaoFilaCarregamento.Motorista.codEntity() != _codigoMotoristaAnterior) {
        _codigoMotoristaAnterior = _marcacaoFilaCarregamento.Motorista.codEntity();

        if (_marcacaoFilaCarregamento.Motorista.codEntity() > 0) {
            executarReST("MarcacaoFilaCarregamento/ObterDadosMotorista", { Motorista: _marcacaoFilaCarregamento.Motorista.codEntity() }, function (retorno) {
                if (retorno.Success) {
                    if (retorno.Data) {
                        PreencherObjetoKnout(_marcacaoFilaCarregamentoMotorista, retorno);

                        _marcacaoFilaCarregamentoMotorista.StatusValidadeCnh.cssClass(retorno.Data.ClasseCorStatusValidadeCnh);
                        _marcacaoFilaCarregamentoMotorista.StatusValidadeGr.cssClass(retorno.Data.ClasseCorStatusValidadeGr);
                        _marcacaoFilaCarregamentoMotorista.Codigo.visible(true);
                    }
                    else
                        exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
                }
                else
                    exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
            });
        }
        else {
            LimparCampos(_marcacaoFilaCarregamentoMotorista);
            _marcacaoFilaCarregamentoMotorista.Codigo.visible(false);
        }
    }
}

function atualizarDadosVeiculo() {
    if (_marcacaoFilaCarregamento.Veiculo.codEntity() != _codigoVeiculoAnterior) {
        _codigoVeiculoAnterior = _marcacaoFilaCarregamento.Veiculo.codEntity()

        if (_marcacaoFilaCarregamento.Veiculo.codEntity() > 0) {
            executarReST("MarcacaoFilaCarregamento/ObterDadosVeiculo", { Veiculo: _marcacaoFilaCarregamento.Veiculo.codEntity() }, function (retorno) {
                if (retorno.Success) {
                    if (retorno.Data) {
                        PreencherObjetoKnout(_marcacaoFilaCarregamentoVeiculo, retorno);
                        _marcacaoFilaCarregamentoVeiculo.PlacaTracao.visible(true);
                    }
                    else
                        exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
                }
                else
                    exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
            });
        }
        else {
            LimparCampos(_marcacaoFilaCarregamentoVeiculo);
            _marcacaoFilaCarregamentoVeiculo.PlacaTracao.visible(false);
        }
    }
}

function exibirModalDetalhesFilaCarregamento() {
    Global.abrirModal('divModalDetalhesFilaCarregamento');
    $("#divModalDetalhesFilaCarregamento").one('hidden.bs.modal', function () {
        LimparCampos(_marcacaoFilaCarregamentoDetalhesFilaCarregamento);
        limparCamposMarcacaoFilaCarregamento();
    });
}

function limparCamposMarcacaoFilaCarregamento() {
    _codigoMotoristaAnterior = 0;
    _codigoVeiculoAnterior = 0;

    LimparCampos(_marcacaoFilaCarregamento);
    LimparCampos(_marcacaoFilaCarregamentoMotorista);
    LimparCampos(_marcacaoFilaCarregamentoVeiculo);

    _marcacaoFilaCarregamentoMotorista.Codigo.visible(false);
    _marcacaoFilaCarregamentoVeiculo.PlacaTracao.visible(false);
}