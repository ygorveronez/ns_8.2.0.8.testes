/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="PlanejamentoPedido.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _planejamentoVeiculoPLacaCarregamento;
var _gridPlacaCarregamento;
/*
 * Declaração das Classes
 */

var PlanejamentoVeiculoDefinicaoPLacaCarregamento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Cancelar = PropertyEntity({ type: types.event, val: ko.observable(false), eventClick: fecharModalPlanejamentoVeiculoDefinicaoVeiculo, text: "Cancelar", visible: ko.observable(true) });

    this.Confirmar = PropertyEntity({ eventClick: adicionarDefinicaoPlacaCarregamentoClick, type: types.event, text: "Confirmar" });

    //this.Grid = PropertyEntity({ type: types.local });
    this.Grid = PropertyEntity({ idGrid: guid(), enable: ko.observable(true) });

};

/*
 * Declaração das Funções de Inicialização
 */

function loadPlanejamentoVeiculoDefinicaoPLacaCarregamento() {
    _planejamentoVeiculoPLacaCarregamento = new PlanejamentoVeiculoDefinicaoPLacaCarregamento();
    KoBindings(_planejamentoVeiculoPLacaCarregamento, "knockoutPlanejamentoPedidoPlacaCarregamento");
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarDefinicaoPlacaCarregamentoClick(e) {
    var dados = new Object();
    dados.CodigoPedido = e.Codigo.val();
    dados.ItensSelecionados = JSON.stringify(_gridPlacaCarregamento.ObterMultiplosSelecionados());

    executarReST("PlanejamentoPedido/SalvarPlacasCarregamento", dados, function (r) {
        if (r.Success) {
            if (r.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso!", "Placas selecionadas salvas com sucesso.");
                fecharModalPlanejamentoVeiculoDefinicaoVeiculo();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Atenção!", r.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
    });
    fecharModalPlanejamentoVeiculoDefinicaoVeiculo();
}

function exibirModalPlanejamentoVeiculoDefinicaoPlacaCarregamento(codigoPedido) {

    executarReST("PlanejamentoPedido/ObterVeiculosVinculadosSelecionados", { Codigo: codigoPedido }, function (arg) {
        if (arg.Success) {
            var selecionados = new Array();

            if (arg.Data != null) {

                $.each(arg.Data, function (i, obj) {
                    var objCTe = { DT_RowId: obj.Codigo, Codigo: obj.Codigo };
                    selecionados.push(objCTe); 
                });
            }

            Global.abrirModal('divModalPlacaCarregamento');

            var editarColuna = {
                permite: true,
                atualizarRow: true,
                callback: SalvarRetornoGridPlacaCarregamento,
            };

            var multiplaEscolha = {
                basicGrid: null,
                eventos: {},
                selecionados: selecionados,
                naoSelecionados: new Array(),
                somenteLeitura: false,
            };
            _planejamentoVeiculoPLacaCarregamento.Codigo.val(codigoPedido);

            _gridPlacaCarregamento = new GridView(_planejamentoVeiculoPLacaCarregamento.Grid.idGrid, "PlanejamentoPedido/ObterVeiculosVinculados", _planejamentoVeiculoPLacaCarregamento, null, null, null, null, null, null, multiplaEscolha, null, editarColuna);

            _gridPlacaCarregamento.CarregarGrid();


        } else {
            exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        }
    });

    


}

function fecharModalPlanejamentoVeiculoDefinicaoVeiculo() {
    Global.fecharModal('divModalPlacaCarregamento');
}
function SalvarRetornoGridPlacaCarregamento() { }


