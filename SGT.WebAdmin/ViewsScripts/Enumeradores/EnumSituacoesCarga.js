const EnumSituacoesCargaHelper = function () {
    this.NaLogistica = 0;
    this.Nova = 1;
    this.CalculoFrete = 2;
    this.EmLeilao = 3;
    this.AgTransportador = 4;
    this.AgNFe = 5;
    this.PendeciaDocumentos = 6;
    this.AgImpressaoDocumentos = 7;
    this.ProntoTransporte = 8;
    this.EmTransporte = 9;
    this.LiberadoPagamento = 10;
    this.Encerrada = 11;
    this.EmCancelamento = 12;
    this.Cancelada = 13;
    this.RejeicaoCancelamento = 14;
    this.AgIntegracao = 15;
    this.EmTransbordo = 17;
    this.Anulada = 18;
    this.Todas = 99;
    this.PermiteCTeManual = 101;
};

EnumSituacoesCargaHelper.prototype = {
    isPermiteAlterarFaixaTemperatura: function (situacao) {
        const situacoes = this.obterSituacoesFaixaTemperatura();

        return ($.inArray(situacao, situacoes) > -1);
    },
    obterOpcoesPesquisa: function () {
        return [
            { text: Localization.Resources.Enumeradores.SituacoesCarga.Todas, value: this.Todas },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.DadosDaCarga, value: this.Nova },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.Cancelada, value: this.Cancelada },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.ComLogistica, value: this.NaLogistica },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.EmTransporte, value: this.EmTransporte },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.Encerrada, value: this.Encerrada },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.NotaFiscal, value: this.AgNFe }
        ];
    }, obterOpcoesPesquisaMultiplas: function () {
        return [
            { text: Localization.Resources.Enumeradores.SituacoesCarga.DadosDaCarga, value: this.Nova },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.Cancelada, value: this.Cancelada },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.ComLogistica, value: this.NaLogistica },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.EmTransporte, value: this.EmTransporte },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.Encerrada, value: this.Encerrada },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.NotaFiscal, value: this.AgNFe }
        ];
    },
    obterOpcoesPesquisaSupervisor: function () {
        return [{ text: Localization.Resources.Enumeradores.SituacoesCarga.Todas, value: this.Todas }].concat(this.obterOpcoesEmbarcador());
    },
    obterOpcoesPesquisaTMS: function () {
        return [{ text: Localization.Resources.Enumeradores.SituacoesCarga.Todas, value: this.Todas }].concat(this.obterOpcoesTMS());
    },
    obterOpcoesPesquisaTMSMultiplas: function () {
        return this.obterOpcoesTMS();
    },
    obterOpcoesPesquisaSupervisorMultiplas: function () {
        return this.obterOpcoesEmbarcador();
    },
    obterOpcoesIntegracaoPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.SituacoesCarga.Todas, value: this.Todas }].concat(this.obterOpcoesIntegracao());
    },
    obterOpcoesTMS: function () {
        return [
            { text: Localization.Resources.Enumeradores.SituacoesCarga.EmAndamento, value: this.NaLogistica },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.DadosDaCarga, value: this.Nova },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.DocumentosParaEmissao, value: this.AgNFe },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.Frete, value: this.CalculoFrete },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.EmissaoDosDocumentos, value: this.PendeciaDocumentos },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.Integracao, value: this.AgIntegracao },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.EmTransporte, value: this.EmTransporte },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.Finalizada, value: this.Encerrada },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.Canceladas, value: this.Cancelada },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.Anulada, value: this.Anulada }
        ];
    },
    obterOpcoesEmbarcador: function () {
        return [
            { text: Localization.Resources.Enumeradores.SituacoesCarga.CalculoDeFrete, value: this.CalculoFrete },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.Cancelada, value: this.Cancelada },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.ComLogistica, value: this.NaLogistica },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.DadosDaCarga, value: this.Nova },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.EmissaoDosDocumentos, value: this.PendeciaDocumentos },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.EmTransporte, value: this.EmTransporte },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.Encerrada, value: this.Encerrada },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.Impressao, value: this.AgImpressaoDocumentos },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.Integracao, value: this.AgIntegracao },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.NotaFiscal, value: this.AgNFe },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.PagamentoLiberado, value: this.LiberadoPagamento },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.Transportador, value: this.AgTransportador }
        ];
    },
    obterOpcoesTMSSemCancelada: function () {
        return [
            { text: Localization.Resources.Enumeradores.SituacoesCarga.EmAndamento, value: this.NaLogistica },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.DadosDaCarga, value: this.Nova },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.DocumentosParaEmissao, value: this.AgNFe },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.Frete, value: this.CalculoFrete },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.EmissaoDosDocumentos, value: this.PendeciaDocumentos },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.Integracao, value: this.AgIntegracao },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.EmTransporte, value: this.EmTransporte },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.Finalizada, value: this.Encerrada }
        ];
    },
    obterOpcoesEmbarcadorSemCancelada: function () {
        return [
            { text: Localization.Resources.Enumeradores.SituacoesCarga.CalculoDeFrete, value: this.CalculoFrete },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.ComLogistica, value: this.NaLogistica },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.DadosDaCarga, value: this.Nova },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.EmissaoDosDocumentos, value: this.PendeciaDocumentos },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.EmTransporte, value: this.EmTransporte },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.Encerrada, value: this.Encerrada },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.Impressao, value: this.AgImpressaoDocumentos },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.Integracao, value: this.AgIntegracao },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.NotaFiscal, value: this.AgNFe },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.PagamentoLiberado, value: this.LiberadoPagamento },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.Transportador, value: this.AgTransportador }
        ];
    },
    obterSituacoesCargaNaoFaturada: function () {
        return [
            this.AgNFe,
            this.AgTransportador,
            this.CalculoFrete,
            this.Nova,
            this.PendeciaDocumentos
        ];
    },
    obterSituacoesCargaNaoFaturadaComDescricao: function () {
        return [
            { text: Localization.Resources.Enumeradores.SituacoesCarga.DadosDaCarga, value: this.Nova },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.NotaFiscal, value: this.AgNFe },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.Transportador, value: this.AgTransportador },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.CalculoDeFrete, value: this.CalculoFrete },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.EmissaoDosDocumentos, value: this.PendeciaDocumentos }
        ];
    },
    obterSituacoesFaixaTemperatura: function () {
        return [
            this.AgNFe,
            this.AgTransportador,
            this.Nova,
        ];
    },
    obterSituacoesCargaCanceladaEncerradaAnulada: function () {
        return [
            this.Encerrada,
            this.Cancelada,
            this.Anulada
        ];
    },

    obterOpcoesIntegracao: function () {
        return [
            { text: Localization.Resources.Enumeradores.SituacoesCarga.CalculoDeFrete, value: this.CalculoFrete },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.Cancelada, value: this.Cancelada },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.ComLogistica, value: this.NaLogistica },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.NovaCarga, value: this.Nova },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.PendenciasNaEmissao, value: this.PendeciaDocumentos },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.EmTransporte, value: this.EmTransporte },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.Finalizada, value: this.Encerrada },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.Cancelada, value: this.Cancelada },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.AguardandoImpressaoDosDocumentos, value: this.AgImpressaoDocumentos },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.AguardandoIntegracao, value: this.AgIntegracao },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.AguardandoNotasFiscais, value: this.AgNFe },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.PagamentoLiberado, value: this.LiberadoPagamento },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.AguardandoTransportador, value: this.AgTransportador },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.ProntoParaTransporte, value: this.ProntoTransporte }
        ];
    },

    obterOpcoesIntegracaoApisul: function () {
        return [
            { text: Localization.Resources.Enumeradores.SituacoesCarga.Todas, value: this.Todas },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.DadosDaCarga, value: this.Nova },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.Integracao, value: this.AgIntegracao },
        ];
    },
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.SituacoesCarga.NaLogistica, value: this.NaLogistica },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.DadosDaCarga, value: this.Nova },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.CalculoFrete, value: this.CalculoFrete },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.EmLeilao, value: this.EmLeilao },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.AguardandoTransportador, value: this.AgTransportador },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.AguardandoNotasFiscais, value: this.AgNFe },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.PendenciaDocumentos, value: this.PendeciaDocumentos },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.AguardandoImpressaoDocumentos, value: this.AgImpressaoDocumentos },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.ProntoParaTransporte, value: this.ProntoTransporte },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.EmTransporte, value: this.EmTransporte },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.LiberadoPagamento, value: this.LiberadoPagamento },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.Encerrada, value: this.Encerrada },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.EmCancelamento, value: this.EmCancelamento },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.Cancelada, value: this.Cancelada },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.RejeicaoCancelamento, value: this.RejeicaoCancelamento },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.Integracao, value: this.AgIntegracao },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.EmTransbordo, value: this.EmTransbordo },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.Anulada, value: this.Anulada },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.Todas, value: this.Todas },
            { text: Localization.Resources.Enumeradores.SituacoesCarga.PermiteCTeManual, value: this.PermiteCTeManual }
        ];
    },
    obterDescricao: function (codigo) {
        let opcoes = this.obterOpcoes();
        let [descricao] = opcoes.filter(item => item.value === codigo);

        if (!descricao)
            return "";

        return descricao.text;
    }
};

const EnumSituacoesCarga = Object.freeze(new EnumSituacoesCargaHelper());