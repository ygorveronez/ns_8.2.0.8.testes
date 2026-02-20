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

var _configuracao;
var _CRUDConfiguracao;

var _listaPeriodicidade = [
    { text: "Mensal", value: 1 },
    { text: "Bimestral", value: 2 },
    { text: "Trimestral", value: 3 },
    { text: "Semestral", value: 4 },
    { text: "Anual", value: 5 },
];

var _listaSequenciaPeriodicidadeBimestral = [
    { text: "Jan/Mar/Mai/Jul/Set/Nov", value: 1 },
    { text: "Fev/Abr/Jun/Ago/Out/Dez", value: 2 },
];

var _listaSequenciaPeriodicidadeTrimestral = [
    { text: "Jan/Abr/Jul/Out", value: 3 },
    { text: "Fev/Mai/Ago/Nov", value: 4 },
    { text: "Mar/Jun/Set/Dez", value: 5 },
];

var _listaSequenciaPeriodicidadeSemestral = [
    { text: "Jan/Jul", value: 6 },
    { text: "Fev/Ago", value: 7 },
    { text: "Mar/Set", value: 8 },
    { text: "Abr/Out", value: 9 },
    { text: "Mai/Nov", value: 10 },
    { text: "Jun/Dez", value: 11 },
];

var _listaTipoCnpj = [
    { text: "A raiz do CNPJ do transportador", value: 1 },
    { text: "O CNPJ completo do transportador", value: 2 },
];
/*
 * Declaração das Classes
 */

var Configuracao = function () {
    this.HabilitarGeracaoAutomatica = PropertyEntity({ text: "Habilitar geração automática", getType: typesKnockout.bool, val: ko.observable(false) });
    this.DiasParaContestacao = PropertyEntity({ text: "Dias para contestação da Conciliação: ", getType: typesKnockout.int, val: ko.observable(0) });
    this.Periodicidade = PropertyEntity({ val: ko.observable([]), options: _listaPeriodicidade, def: [], getType: typesKnockout.select, text: "Periodicidade: ", enable: ko.observable(true) });
    this.SequenciaPeriodicidade = PropertyEntity({ val: ko.observable([]), options: ko.observable([]), def: [], getType: typesKnockout.select, text: "Sequência de meses: ", enable: ko.observable(true), visible: ko.observable(false) });
    this.TipoCnpj = PropertyEntity({ val: ko.observable([]), options: _listaTipoCnpj, def: [], getType: typesKnockout.select, text: "Gerar conciliações agrupadas por: ", enable: ko.observable(true) });

    this.Periodicidade.val.subscribe(function () {
        trocarSequenciaPeriodicidade();
    });

};

function trocarSequenciaPeriodicidade() {
    switch (_configuracao.Periodicidade.val()) {
        case 1:
        case 5:
            _configuracao.SequenciaPeriodicidade.options([]);
            _configuracao.SequenciaPeriodicidade.visible(false);
            break;
        case 2:
            _configuracao.SequenciaPeriodicidade.options(_listaSequenciaPeriodicidadeBimestral);
            _configuracao.SequenciaPeriodicidade.visible(true);
            break;
        case 3:
            _configuracao.SequenciaPeriodicidade.options(_listaSequenciaPeriodicidadeTrimestral);
            _configuracao.SequenciaPeriodicidade.visible(true);
            break;
        case 4:
            _configuracao.SequenciaPeriodicidade.options(_listaSequenciaPeriodicidadeSemestral);
            _configuracao.SequenciaPeriodicidade.visible(true);
            break;
    }
}

var CRUDConfiguracao = function () {
    this.Salvar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Salvar", icon: "fa fa-save", visible: ko.observable(true) });
};

/*
 * Declaração das Funções de Inicialização
 */

function LoadConfiguracao() {
    _configuracao = new Configuracao();
    KoBindings(_configuracao, "knockoutCadastro");

    _CRUDConfiguracao = new CRUDConfiguracao();
    KoBindings(_CRUDConfiguracao, "knockoutCRUD");

    buscarConfiguracao();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function atualizarClick() {
    if (ValidarCamposObrigatorios(_configuracao)) {
        executarReST("ConfiguracaoConciliacaoTransportador/Atualizar", obterConfiguracaoSalvar(), function (retorno) {
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
    executarReST("ConfiguracaoConciliacaoTransportador/ObterConfiguracao", {}, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                PreencherObjetoKnout(_configuracao, retorno);
            }
            else
                exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function obterConfiguracaoSalvar() {
    var configuracao = RetornarObjetoPesquisa(_configuracao);
    return configuracao;
}
