/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/MotivoAtrasoCarregamento.js" />


var _integracaoAVIPED;
var _gridIntegracoesAVIPED;

var IntegracaoAVIPED = function () {
    this.CodigoCarga = PropertyEntity({ getType: typesKnockout.int, def: 0, val: ko.observable(0) });
    this.ConsultarTodos = PropertyEntity({
        eventClick: consultarTodos, type: types.event, text: "Consultar Todos", idGrid: guid(), visible: ko.observable(true)
    });

    this.CodigoCarga.val.subscribe((codigo) => {
        if (codigo > 0)
            _gridIntegracoesAVIPED.CarregarGrid((retorno) => {
                if (retorno.data.length === 0)
                    consultarTodos();
            });
    });
}

function loadIntegracaoAVIPED() {
    _integracaoAVIPED = new IntegracaoAVIPED();
    KoBindings(_integracaoAVIPED, "knockoutIntegracaoAVIPED");
    loadGridIntegracoesAVIPED();
}

function loadGridIntegracoesAVIPED() {
    const consultar = { descricao: "Consultar", id: "clasConsultar", evento: "onclick", metodo: consultarAVIPED, tamanho: "15", icone: "" };
    const menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [consultar] };

    _gridIntegracoesAVIPED = new GridView(_integracaoAVIPED.ConsultarTodos.idGrid, "IntegracaoAVIPED/Pesquisa", _integracaoAVIPED, menuOpcoes);
}

function exibirModalIntegracaoAVIPED(registroSelecionado) {
    _integracaoAVIPED.CodigoCarga.val(registroSelecionado.CodigoCarga);
    Global.abrirModal('divModalIntegracaoAVIPED');
}

function consultarAVIPED(integracao, row) {
    executarReST("IntegracaoAVIPED/ConsultarIntegracao", { Codigo: integracao.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                integracao.NumeroAvisoRecebimento = retorno.Data.NumeroAvisoRecebimento
                integracao.NumeroPedidoCompra = retorno.Data.NumeroPedidoCompra;
                _gridIntegracoesAVIPED.AtualizarDataRow(row, integracao);

                if (retorno.Data.Sucesso)
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, "Integração realizada com sucesso.");
                else
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Data);
            }
            else {
                _gridIntegracoesAVIPED.CarregarGrid();
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
            }
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function consultarTodos() {
    executarReST("IntegracaoAVIPED/ConsultarIntegracoes", { CodigoCarga: _integracaoAVIPED.CodigoCarga.val() }, function (retorno) {
        if (retorno.Success) {
            _gridIntegracoesAVIPED.CarregarGrid();
            if (retorno.Data) {
                if (retorno.Data.Sucessos > 0)
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, `${retorno.Data.Sucessos} integraç${(retorno.Data.Sucessos > 1) ? 'ões' : 'ão'} realizada com sucesso.`);
                if (retorno.Data.Erros > 0)
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, `Ocorreu um erro em ${retorno.Data.Erros} integraç${(retorno.Data.Erros > 1) ? 'ões' : 'ão'}.`);
            }
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}