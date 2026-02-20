/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="PlanejamentoPedido.js" />
/// <reference path="PlanejamentoPedidoDisponibilidade.js" />
/// <reference path="PlanejamentoPedidoDisponibilidadeMotorista.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _planejamentoVeiculoDefinicaoVeiculo;

/*
 * Declaração das Classes
 */

var PlanejamentoVeiculoDefinicaoVeiculo = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "*Veículo:", idBtnSearch: guid() });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "*Motorista:", idBtnSearch: guid() });

    this.Adicionar = PropertyEntity({ eventClick: adicionarDefinicaoVeiculoClick, type: types.event, text: "Definir" });
};

/*
 * Declaração das Funções de Inicialização
 */

function loadPlanejamentoVeiculoDefinicaoVeiculo() {
    _planejamentoVeiculoDefinicaoVeiculo = new PlanejamentoVeiculoDefinicaoVeiculo();
    KoBindings(_planejamentoVeiculoDefinicaoVeiculo, "knockoutPlanejamentoPedidoDefinicaoVeiculo");

    new BuscarVeiculos(_planejamentoVeiculoDefinicaoVeiculo.Veiculo, RetornoSelecaoVeiculo);
    new BuscarMotoristas(_planejamentoVeiculoDefinicaoVeiculo.Motorista, RetornoSelecaoMotorista, null, null, null, EnumSituacaoColaborador.Trabalhando);
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

    adicionarDefinicaoMotoristaClick();
}

function adicionarDefinicaoMotoristaClick() {
    Salvar(_planejamentoVeiculoDefinicaoVeiculo, "PlanejamentoPedido/DefinirMotorista", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso!", "Motorista definido com sucesso.");
                _gridPlanejamentoPedido.CarregarGrid();

                forcarRecarregarGridPlanejamentoPedidoDisponibilidadeMotorista();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Atenção!", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha!", retorno.Msg);
    });
}

function adicionarDefinicaoVeiculoClick() {
    Salvar(_planejamentoVeiculoDefinicaoVeiculo, "PlanejamentoPedido/DefinirVeiculo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso!", "Veículo definido com sucesso.");
                _gridPlanejamentoPedido.CarregarGrid();

                forcarRecarregarGridPlanejamentoPedidoDisponibilidade();
                forcarRecarregarGridPlanejamentoPedidoDisponibilidadeMotorista();
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

    Global.abrirModal('divModalPlanejamentoPedidoDefinicaoVeiculo');
    $("#divModalPlanejamentoPedidoDefinicaoVeiculo").one('hidden.bs.modal', function () {
        LimparCampos(_planejamentoVeiculoDefinicaoVeiculo);
    });
}

function fecharModalPlanejamentoVeiculoDefinicaoVeiculo() {
    Global.fecharModal('divModalPlanejamentoPedidoDefinicaoVeiculo');
}