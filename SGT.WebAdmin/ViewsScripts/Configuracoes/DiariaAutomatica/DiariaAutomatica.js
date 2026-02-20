/// <reference path="Motorista.js" />
/// <reference path="Veiculo.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/ComponenteFrete.js" />
/// <reference path="../../Consultas/GrupoPessoa.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Consultas/Produto.js" />
/// <reference path="../../Consultas/PlanoConta.js" />
/// <reference path="../../Enumeradores/EnumSimNao.js" />
/// <reference path="../../Enumeradores/EnumSituacoesCarga.js" />
/// <reference path="../../Enumeradores/EnumTipoChamado.js" />
/// <reference path="../../Enumeradores/EnumTipoContratoFreteTerceiro.js" />
/// <reference path="../../Enumeradores/EnumTipoEmissaoIntramunicipal.js" />
/// <reference path="../../Enumeradores/EnumTipoFechamentoFrete.js" />
/// <reference path="../../Enumeradores/EnumTipoGeracaoTituloFatura.js" />
/// <reference path="../../Enumeradores/EnumTipoIntegracao.js" />
/// <reference path="../../Enumeradores/EnumTipoMovimentoEntidade.js" />
/// <reference path="../../Enumeradores/EnumTipoRestricaoPalletModeloVeicularCarga.js" />
/// <reference path="../../Enumeradores/EnumTipoImpressaoPedido.js" />
/// <reference path="../../Enumeradores/EnumTipoImpressaoPedidoPrestacaoServico.js" />
/// <reference path="../../Enumeradores/EnumMonitorarPosicaoAtualVeiculo.js" />
/// <reference path="../../Enumeradores/EnumTipoFiltroDataMontagemCarga.js" />
/// <reference path="../../Enumeradores/EnumDataBaseCalculoPrevisaoControleEntrega.js" />
/// <reference path="../../Enumeradores/EnumQuandoProcessarMonitoramento.js" />
/// <reference path="../../Enumeradores/EnumPaises.js" />
/// <reference path="../../Enumeradores/EnumFormaPreenchimentoCentroResultadoPedido.js" />
/// <reference path="../../Enumeradores/EnumTipoImpressaoFatura.js" />
/// <reference path="../../Enumeradores/EnumTipoCalculoPercentualViagem.js" />
/// <reference path="../../Enumeradores/EnumMonitoramentoStatusViagemTipoRegra.js" />
/// <reference path="../../Enumeradores/EnumFormatoData.js" />
/// <reference path="../../Enumeradores/EnumFormatoHora.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _diariaAutomatica;
var _CRUDDiariaAutomatica;

/*
 * Declaração das Classes
 */

var DiariaAutomatica = function () {
    this.HabilitarDiariaAutomatica = PropertyEntity({ text: "Habilitar diaria automática", getType: typesKnockout.bool, val: ko.observable(false) });
    this.FrequenciaAtualizacao = PropertyEntity({ text: "Frequência de atualização: (minutos)", getType: typesKnockout.int, val: ko.observable(0) });
};

var CRUDDiariaAutomatica = function () {
    this.Salvar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Salvar", icon: "fa fa-save", visible: ko.observable(true) });
};

/*
 * Declaração das Funções de Inicialização
 */

function LoadDiariaAutomatica() {
    _diariaAutomatica = new DiariaAutomatica();
    KoBindings(_diariaAutomatica, "knockoutCadastro");

    _CRUDDiariaAutomatica = new CRUDDiariaAutomatica();
    KoBindings(_CRUDDiariaAutomatica, "knockoutCRUD");

    buscarConfiguracao();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function atualizarClick() {
    if (ValidarCamposObrigatorios(_diariaAutomatica)) {
        executarReST("ConfiguracaoDiariaAutomatica/Atualizar", obterConfiguracaoSalvar(), function (retorno) {
            if (retorno.Success) {
                if (retorno.Data)
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    }
}


/*
 * Declaração das Funções
 */

function buscarConfiguracao() {
    executarReST("ConfiguracaoDiariaAutomatica/ObterConfiguracao", {}, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                PreencherObjetoKnout(_diariaAutomatica, retorno);
            }
            else
                exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function obterConfiguracaoSalvar() {
    var configuracao = RetornarObjetoPesquisa(_diariaAutomatica);
    return configuracao;
}

