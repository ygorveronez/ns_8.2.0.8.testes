/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="PlanejamentoPedidoTMS.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _planejamentoVeiculoDefinicaoVeiculo;
var _codigoMotoristaSelecionado;
/*
 * Declaração das Classes
 */

var PlanejamentoVeiculoDefinicaoVeiculo = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "*Veículo:", idBtnSearch: guid() });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "*Motorista:", idBtnSearch: guid() });
    this.ModeloVeicular = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Modelo Veicular:", idBtnSearch: guid() });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Tipo da Operação:", idBtnSearch: guid() });
    this.TipoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Tipo da Carga:", idBtnSearch: guid() });

    this.Adicionar = PropertyEntity({ eventClick: adicionarDefinicaoVeiculoClick, type: types.event, text: "Definir" });
};

/*
 * Declaração das Funções de Inicialização
 */

function loadPlanejamentoVeiculoDefinicaoVeiculo() {
    _planejamentoVeiculoDefinicaoVeiculo = new PlanejamentoVeiculoDefinicaoVeiculo();
    KoBindings(_planejamentoVeiculoDefinicaoVeiculo, "knockoutPlanejamentoPedidoTMSDefinicaoVeiculo");

    let tipoPropriedade = "";

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
        //Caso seja TMS, vai definir valor padrão como N, desta forma, não trará nenhum veículo se o usuário não tiver nenhuma das permissões
        tipoPropriedade = "N";
        if (_CONFIGURACAO_TMS.UsuarioAdministrador || VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteInserirVeiculoProprio, _PermissoesPersonalizadas))
            tipoPropriedade = "P";
        if (_CONFIGURACAO_TMS.UsuarioAdministrador || VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteInserirVeiculoTerceiro, _PermissoesPersonalizadas))
            tipoPropriedade = tipoPropriedade == "N" ? "T" : "A";
    }

    BuscarVeiculos(_planejamentoVeiculoDefinicaoVeiculo.Veiculo, RetornoSelecaoVeiculo, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, tipoPropriedade, null, _planejamentoVeiculoDefinicaoVeiculo.TipoOperacao);
    BuscarMotoristas(_planejamentoVeiculoDefinicaoVeiculo.Motorista, RetornoSelecaoMotorista, null, null, null, EnumSituacaoColaborador.Todos);
    BuscarModelosVeicularesCarga(_planejamentoVeiculoDefinicaoVeiculo.ModeloVeicular, RetornoSelecaoModeloVeicular);
    BuscarTiposOperacao(_planejamentoVeiculoDefinicaoVeiculo.TipoOperacao, RetornoSelecaoTiposOperacao);
    BuscarTiposdeCarga(_planejamentoVeiculoDefinicaoVeiculo.TipoCarga, RetornoSelecaoTiposDeCarga)

}

/*
 * Declaração das Funções Associadas a Eventos
 */

function RetornoSelecaoVeiculo(veiculo) {
    _planejamentoVeiculoDefinicaoVeiculo.Veiculo.codEntity(veiculo.Codigo);
    _planejamentoVeiculoDefinicaoVeiculo.Veiculo.val(veiculo.Placa);
    adicionarDefinicaoVeiculoClick();
}

function RetornoSelecaoMotorista(motorista) {
    _planejamentoVeiculoDefinicaoVeiculo.Motorista.codEntity(motorista.Codigo);
    _planejamentoVeiculoDefinicaoVeiculo.Motorista.val(motorista.Descricao);
    _codigoMotoristaSelecionado = motorista.Codigo;
    adicionarDefinicaoMotoristaClick();
}

function RetornoSelecaoTiposOperacao(tipoOperacao) {
    _planejamentoVeiculoDefinicaoVeiculo.TipoOperacao.codEntity(tipoOperacao.Codigo);
    _planejamentoVeiculoDefinicaoVeiculo.TipoOperacao.val(tipoOperacao.Descricao);

    adicionarDefinicaoTipoOperacaoClick();
}

function RetornoSelecaoModeloVeicular(modeloVeicular) {
    _planejamentoVeiculoDefinicaoVeiculo.ModeloVeicular.codEntity(modeloVeicular.Codigo);
    _planejamentoVeiculoDefinicaoVeiculo.ModeloVeicular.val(modeloVeicular.Descricao);

    adicionarDefinicaoModeloVeicularClick();
}

function RetornoSelecaoTiposDeCarga(tipoCarga) {
    _planejamentoVeiculoDefinicaoVeiculo.TipoCarga.codEntity(tipoCarga.Codigo);
    _planejamentoVeiculoDefinicaoVeiculo.TipoCarga.val(tipoCarga.Descricao);

    adicionarDefinicaoTipoCargaClick();
}

function adicionarDefinicaoMotoristaClick() {
    Salvar(_planejamentoVeiculoDefinicaoVeiculo, "PlanejamentoPedidoTMS/DefinirMotorista", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Motorista definido com sucesso.");
                _gridPlanejamentoPedidoTMS.CarregarGrid();
                ValidarSituacaoMotorista();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Atenção!", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha!", retorno.Msg);

    });
}

function validarVeiculoOutroPedidoOuCarga() {
    Salvar(_planejamentoVeiculoDefinicaoVeiculo, "PlanejamentoPedidoTMS/ValidarVeiculoOutroPedidoOuCarga", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                if (!string.IsNullOrWhiteSpace(retorno.Msg))
                    exibirConfirmacao("Atenção!", retorno.Msg, salvarDefinicaoVeiculo);
                else
                    salvarDefinicaoVeiculo();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Atenção!", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha!", retorno.Msg);
    });
}

function adicionarDefinicaoVeiculoClick() {
    if (_CONFIGURACAO_TMS.NaoPermitirInformarVeiculoDuplicadoPedidoCargaAberta)
        validarVeiculoOutroPedidoOuCarga();
    else
        salvarDefinicaoVeiculo();
}

function salvarDefinicaoVeiculo() {
    Salvar(_planejamentoVeiculoDefinicaoVeiculo, "PlanejamentoPedidoTMS/DefinirVeiculo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Veículo definido com sucesso.");               
                _gridPlanejamentoPedidoTMS.CarregarGrid();
                ValidarVeiculoSituacaoMotorista();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Atenção!", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha!", retorno.Msg);
    });
}

function ValidarVeiculoSituacaoMotorista() {
    exibirMensagem(tipoMensagem.aviso, "", "Validando motorista do veículo selecionado");
    executarReST("Motorista/ValidarVeiculoMotoristaSituacao", { Codigo: _planejamentoVeiculoDefinicaoVeiculo.Veiculo.codEntity() }, function (arg) {
        if (arg.Success && arg.Data && arg.Data.ExibirConfirmacaoMotoristaSituacao)
            exibirAlerta("Atenção", arg.Msg, "Ok");

        _codigoMotoristaSelecionado = null;
    });
}
//aqui

function adicionarDefinicaoTipoOperacaoClick() {
    Salvar(_planejamentoVeiculoDefinicaoVeiculo, "PlanejamentoPedidoTMS/DefinirTipoOperacao", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Tipo de Operação definido com sucesso.");
                _gridPlanejamentoPedidoTMS.CarregarGrid();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Atenção!", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha!", retorno.Msg);
    });
}

function adicionarDefinicaoTipoCargaClick() {
    Salvar(_planejamentoVeiculoDefinicaoVeiculo, "PlanejamentoPedidoTMS/DefinirTipoCarga", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Tipo de Carga definido com sucesso.");
                _gridPlanejamentoPedidoTMS.CarregarGrid();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Atenção!", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha!", retorno.Msg);
    });
}

function adicionarDefinicaoModeloVeicularClick() {
    Salvar(_planejamentoVeiculoDefinicaoVeiculo, "PlanejamentoPedidoTMS/DefinirModeloVeicular", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Modelo Veicular definido com sucesso.");
                _gridPlanejamentoPedidoTMS.CarregarGrid();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Atenção!", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha!", retorno.Msg);
    });
}

function exibirModalPlanejamentoVeiculoDefinicaoVeiculo(codigoPedido) {
    _planejamentoVeiculoDefinicaoVeiculo.Codigo.val(codigoPedido);

    Global.abrirModal('divModalPlanejamentoPedidoTMSDefinicaoVeiculo');
    $("#divModalPlanejamentoPedidoTMSDefinicaoVeiculo").one('hidden.bs.modal', function () {
        LimparCampos(_planejamentoVeiculoDefinicaoVeiculo);
    });
}

function fecharModalPlanejamentoVeiculoDefinicaoVeiculo() {
    Global.fecharModal('divModalPlanejamentoPedidoTMSDefinicaoVeiculo');
}

function ValidarSituacaoMotorista() {
    exibirMensagem(tipoMensagem.aviso, "", "Validando motorista selecionado");
    executarReST("Motorista/ValidarMotoristaSituacao", { Codigo: _codigoMotoristaSelecionado }, function (arg) {
        if (arg.Success && arg.Data && arg.Data.ExibirConfirmacaoMotoristaSituacao)
            exibirAlerta("Atenção", arg.Msg, "Ok");            

        _codigoMotoristaSelecionado = null;
    });
}