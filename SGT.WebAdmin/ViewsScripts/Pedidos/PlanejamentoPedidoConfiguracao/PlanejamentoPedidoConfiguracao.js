/*
 * Declaração de Objetos Globais do Arquivo
*/
var _CRUDPlanejamentoPedidoConfiguracao;
var _planejamentoPedidoConfiguracao;

/*
 * Declaração das Classes
 */

var CRUDPlanejamentoPedidoConfiguracao = function () {
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(true) });
    
}

var PlanejamentoPedidoConfiguracao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Email = PropertyEntity({ text: "*Email:", maxlength: 1000, required: true, getType: typesKnockout.string, val: ko.observable("") });
}

/*
 * Declaração das Funções de Inicialização
 */

function BuscarConfiguracao() {

    BuscarPorCodigo(_planejamentoPedidoConfiguracao, "PlanejamentoPedidoConfiguracao/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                 
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function loadPlanejamentoPedidoConfiguracao() {
    _planejamentoPedidoConfiguracao = new PlanejamentoPedidoConfiguracao();
    KoBindings(_planejamentoPedidoConfiguracao, "knockoutPlanejamentoPedidoConfiguracao");

    _CRUDPlanejamentoPedidoConfiguracao = new CRUDPlanejamentoPedidoConfiguracao();
    KoBindings(_CRUDPlanejamentoPedidoConfiguracao, "knockoutCRUDPlanejamentoPedidoConfiguracao");

    BuscarConfiguracao();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function atualizarClick(e, sender) {
    Salvar(_planejamentoPedidoConfiguracao, "PlanejamentoPedidoConfiguracao/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

               
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

