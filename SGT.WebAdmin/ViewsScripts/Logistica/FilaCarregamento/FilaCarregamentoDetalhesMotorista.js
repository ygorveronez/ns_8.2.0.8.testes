/*
 * Declaração de Objetos Globais do Arquivo
 */

var _filaCarregamentoDetalhesMotorista;

/*
 * Declaração das Classes
 */

var FilaCarregamentoDetalheMotorista = function () {
    this.Cpf = PropertyEntity({ text: "CPF: " });
    this.Nome = PropertyEntity({ text: "Nome: " });
    this.Telefone = PropertyEntity({ text: "Telefone: " });
    this.Transportador = PropertyEntity({ text: "Transportadora: " });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadFilaCarregamentoDetalhesMotorista() {
    _filaCarregamentoDetalhesMotorista = new FilaCarregamentoDetalheMotorista();
    KoBindings(_filaCarregamentoDetalhesMotorista, "knockoutFilaCarregamentoDetalhesMotorista");
}

/*
 * Declaração das Funções
 */

function ExibirDetalhesMotorista(filaSelecionada) {
    executarReST("FilaCarregamento/ObterDetalhesMotorista", { Codigo: filaSelecionada.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                PreencherObjetoKnout(_filaCarregamentoDetalhesMotorista, retorno);
                ExibirModalDetalhesMotorista();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function ExibirModalDetalhesMotorista() {
    Global.abrirModal('divModalDetalhesFilaCarregamentoMotorista');
    $("#divModalDetalhesFilaCarregamentoMotorista").one('hidden.bs.modal', function () {
        LimparCampos(_filaCarregamentoDetalhesMotorista);
    });
}