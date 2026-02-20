var EnumIdentificadorControlePosicaoThreadHelper = function () {
    this.Todos = "";
    this.VerificarCargasLiberarSemNFe = 1;
    this.VerificarCargasEmFinalizacaoCancelamento = 2;
    this.GerarCargasAguardandoGeracaoPreCarga = 3;
    this.SolicitarEmissaoDocumentosAutorizadosNaEtapaNFe = 4;
    this.SolicitarEmissaoDocumentosAutorizadosNaEtapaDeFrete = 5;
    this.SolicitarEmissaoDocumentosAutorizadosCTesSubContratacaoFilialEmissora = 6;
    this.SolicitarEmissaoDocumentosAutorizadosNaEtapaDeFreteLeve = 7;
    this.SolicitarEmissaoDocumentosAutorizadosNaEtapaDeFreteMedia = 8;
    this.SolicitarEmissaoDocumentosAutorizadosNaEtapaDeFretePesada = 9;
    this.SolicitarEmissaoDocumentosAutorizadosNaEtapaNFeIntegracao = 10;
    this.SolicitarEmissaoDocumentosAutorizadosNaEtapaDeFreteIntegracao = 11;
    this.SolicitarEmissaoDocumentosAutorizadosCTesSubContratacaoFilialEmissoraIntegracao = 12;
    this.SolicitarEmissaoDocumentosAutorizadosNaEtapaDeFreteIntegracaoLeve = 13;
    this.SolicitarEmissaoDocumentosAutorizadosNaEtapaDeFreteIntegracaoMedia = 14;
    this.SolicitarEmissaoDocumentosAutorizadosNaEtapaDeFreteIntegracaoPesada = 15;
    this.ProcessarXMLCTeImportado = 16;
    this.VerificarLotesDeDesbloqueioPendentes = 17;
    this.GerarCargaEntregaPendentes = 18;
    this.FecharCargasEmFechamento = 19;
    this.AprovacaoMassivaChamado = 20;
    this.ConsultarValoresPedagioPendente = 21;
    this.IntegrarAverbacoesPendentesAutorizacao = 22;
    this.VerificarCargasPendentesEmissao = 23;
    this.VerificarCargasPendentesEmissaoIntegracao = 24;
    this.CargaOferta = 25;
    this.GerarCTesIntegracao = 26;
    this.GrupoMotoristas = 27;
    this.VerificarCargasOcorrenciaAutorizacaoPendentes = 28;
    this.RoteirizadorIntegracao = 29;
    this.SolicitacaoConfirmacaoDocumentosFiscais = 30;
    this.VerificarCargasGerarCTeAnteriorEmitirDocumentoSempreOrigemDestinoPedido = 31;
    this.FecharCargasEmFechamentoWorker = 32;
    this.VerificarEntregasPendentesNotificacao = 33;
    this.SolicitarEmissaoCargasEmEmissao = 34;
    this.SolicitarEmissaoCargasEmEmissaoWorker = 35;
    this.VerificarCargasFilialEmissoraAgGerarCTeAnterior = 36;
    this.GerarIntegracoesValePedagio = 37;
    this.VerificarRetornosValePedagio = 38;
    this.VerificarCargaIntegracaoPendentes = 39;
    this.VerificarIntegracoesCargaDadosTransportePendentes = 40;
    this.VerificarDocumentoComplementarPendenteEmissao = 41;
    this.VerificarMDFeManualPendentes = 42;
    this.GeracaoCargaEspelho = 43;
    this.IntegrarEventoSuperApp = 44;
    this.EfetuarDownloadXMLCTe = 46;
};

EnumIdentificadorControlePosicaoThreadHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "AprovacaoMassivaChamado", value: this.AprovacaoMassivaChamado },
            { text: "ConsultarValoresPedagioPendente", value: this.ConsultarValoresPedagioPendente },
            { text: "FecharCargasEmFechamento", value: this.FecharCargasEmFechamento },
            { text: "GerarCargaEntregaPendentes", value: this.GerarCargaEntregaPendentes },
            { text: "GerarCargasAguardandoGeracaoPreCarga", value: this.GerarCargasAguardandoGeracaoPreCarga },
            { text: "IntegrarAverbacoesPendentesAutorizacao", value: this.IntegrarAverbacoesPendentesAutorizacao },
            { text: "ProcessarXMLCTeImportado", value: this.ProcessarXMLCTeImportado },
            { text: "SolicitarEmissaoDocumentosAutorizadosCTesSubContratacaoFilialEmissora", value: this.SolicitarEmissaoDocumentosAutorizadosCTesSubContratacaoFilialEmissora },
            { text: "SolicitarEmissaoDocumentosAutorizadosCTesSubContratacaoFilialEmissoraIntegracao", value: this.SolicitarEmissaoDocumentosAutorizadosCTesSubContratacaoFilialEmissoraIntegracao },
            { text: "SolicitarEmissaoDocumentosAutorizadosNaEtapaDeFrete", value: this.SolicitarEmissaoDocumentosAutorizadosNaEtapaDeFrete },
            { text: "SolicitarEmissaoDocumentosAutorizadosNaEtapaDeFreteIntegracao", value: this.SolicitarEmissaoDocumentosAutorizadosNaEtapaDeFreteIntegracao },
            { text: "SolicitarEmissaoDocumentosAutorizadosNaEtapaDeFreteIntegracaoLeve", value: this.SolicitarEmissaoDocumentosAutorizadosNaEtapaDeFreteIntegracaoLeve },
            { text: "SolicitarEmissaoDocumentosAutorizadosNaEtapaDeFreteIntegracaoMedia", value: this.SolicitarEmissaoDocumentosAutorizadosNaEtapaDeFreteIntegracaoMedia },
            { text: "SolicitarEmissaoDocumentosAutorizadosNaEtapaDeFreteIntegracaoPesada", value: this.SolicitarEmissaoDocumentosAutorizadosNaEtapaDeFreteIntegracaoPesada },
            { text: "SolicitarEmissaoDocumentosAutorizadosNaEtapaDeFreteLeve", value: this.SolicitarEmissaoDocumentosAutorizadosNaEtapaDeFreteLeve },
            { text: "SolicitarEmissaoDocumentosAutorizadosNaEtapaDeFreteMedia", value: this.SolicitarEmissaoDocumentosAutorizadosNaEtapaDeFreteMedia },
            { text: "SolicitarEmissaoDocumentosAutorizadosNaEtapaDeFretePesada", value: this.SolicitarEmissaoDocumentosAutorizadosNaEtapaDeFretePesada },
            { text: "SolicitarEmissaoDocumentosAutorizadosNaEtapaNFe", value: this.SolicitarEmissaoDocumentosAutorizadosNaEtapaNFe },
            { text: "SolicitarEmissaoDocumentosAutorizadosNaEtapaNFeIntegracao", value: this.SolicitarEmissaoDocumentosAutorizadosNaEtapaNFeIntegracao },
            { text: "VerificarCargasEmFinalizacaoCancelamento", value: this.VerificarCargasEmFinalizacaoCancelamento },
            { text: "VerificarCargasLiberarSemNFe", value: this.VerificarCargasLiberarSemNFe },
            { text: "VerificarCargasPendentesEmissao", value: this.VerificarCargasPendentesEmissao },
            { text: "VerificarCargasPendentesEmissaoIntegracao", value: this.VerificarCargasPendentesEmissaoIntegracao },
            { text: "VerificarLotesDeDesbloqueioPendentes", value: this.VerificarLotesDeDesbloqueioPendentes },
            { text: "CargaOferta", value: this.CargaOferta },
            { text: "GerarCTesIntegracao", value: this.GerarCTesIntegracao },
            { text: "SolicitacaoConfirmacaoDocumentosFiscais", value: this.SolicitacaoConfirmacaoDocumentosFiscais },
            { text: "GrupoMotoristas", value: this.GrupoMotoristas },
            { text: "VerificarCargasOcorrenciaAutorizacaoPendentes", value: this.VerificarCargasOcorrenciaAutorizacaoPendentes },
            { text: "RoteirizadorIntegracao", value: this.RoteirizadorIntegracao },
            { text: "VerificarCargasFilialEmissoraAgGerarCTeAnterior", value: this.VerificarCargasFilialEmissoraAgGerarCTeAnterior },
            { text: "VerificarCargasGerarCTeAnteriorEmitirDocumentoSempreOrigemDestinoPedido", value: this.VerificarCargasGerarCTeAnteriorEmitirDocumentoSempreOrigemDestinoPedido },
            { text: "FecharCargasEmFechamentoWorker", value: this.FecharCargasEmFechamentoWorker },
            { text: "VerificarEntregasPendentesNotificacao", value: this.VerificarEntregasPendentesNotificacao },
            { text: "SolicitarEmissaoCargasEmEmissao", value: this.SolicitarEmissaoCargasEmEmissao },
            { text: "SolicitarEmissaoCargasEmEmissaoWorker", value: this.SolicitarEmissaoCargasEmEmissaoWorker },
            { text: "GerarIntegracoesValePedagio", value: this.GerarIntegracoesValePedagio },
            { text: "VerificarRetornosValePedagio", value: this.VerificarRetornosValePedagio },
            { text: "VerificarCargaIntegracaoPendentes", value: this.VerificarCargaIntegracaoPendentes },
            { text: "VerificarIntegracoesCargaDadosTransportePendentes", value: this.VerificarIntegracoesCargaDadosTransportePendentes },
            { text: "VerificarDocumentoComplementarPendenteEmissao", value: this.VerificarDocumentoComplementarPendenteEmissao },
            { text: "VerificarMDFeManualPendentes", value: this.VerificarMDFeManualPendentes },
            { text: "GeracaoCargaEspelho", value: this.GeracaoCargaEspelho },
            { text: "IntegrarEventoSuperApp", value: this.IntegrarEventoSuperApp },
            { text: "EfetuarDownloadXMLCTe", value: this.EfetuarDownloadXMLCTe },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumIdentificadorControlePosicaoThread = Object.freeze(new EnumIdentificadorControlePosicaoThreadHelper());
