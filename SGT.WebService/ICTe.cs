using CoreWCF;
using Dominio.ObjetosDeValor.WebService.CTe;

namespace SGT.WebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ICTe" in both code and config file together.
    [ServiceContract]
    public interface ICTe
    {
        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTeAverbacao>> BuscarAverbacaoCTes(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno? tipoDocumentoRetorno, int? inicio, int? limite);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>> BuscarCTes(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno? tipoDocumentoRetorno, int? inicio, int? limite);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>> BuscarCTesTk(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno? tipoDocumentoRetorno, string token, int? inicio, int? limite);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>> BuscarCTesPorCarga(int? protocoloIntegracaoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno? tipoDocumentoRetorno, int? inicio, int? limite);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>> BuscarCTesPorOcorrencia(int protocoloIntegracaoOcorrencia, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno? tipoDocumentoRetorno, int? inicio, int? limite);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>> BuscarCTesPorBoleto(string nossoNumeroBoleto, string numeroBanco, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno? tipoDocumentoRetorno, int? inicio, int? limite);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>> BuscarCTesPorPeriodo(string dataInicial, string dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno? tipoDocumentoRetorno, int? inicio, int? limite, string codigoTipoOperacao, string situacao);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>> BuscarCTesPorPeriodoHora(string dataInicial, string dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno? tipoDocumentoRetorno, int? inicio, int? limite, string codigoTipoOperacao, string situacao);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>> BuscarCTesPorPeriodoTk(string dataInicial, string dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno? tipoDocumentoRetorno, int? inicio, int? limite, string codigoTipoOperacao, string situacao, string token);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTeComplementar>> BuscarCTesComplementaresAguardandoIntegracao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno? tipoDocumentoRetorno, string codificarUTF8, int? inicio, int? limite);

        [OperationContract]
        Retorno<bool> ConfirmarIntegracaoCTeComplementar(int protocoloCTe);

        [OperationContract]
        Retorno<bool> GerarCCe(Dominio.ObjetosDeValor.WebService.CTe.CamposCCe camposCCe, int protocoloCTe);

        [OperationContract]
        Retorno<bool> EnviarCCe(Dominio.ObjetosDeValor.WebService.CTe.CCe cce);

        [OperationContract]
        Retorno<bool> ConfirmarRecebimentoCancelamentoCTeComplementar(int protocoloCTe);

        [OperationContract]
        Retorno<Dominio.ObjetosDeValor.WebService.CTe.CTe> BuscarCTePorProtocolo(int protocoloCTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno? tipoDocumentoRetorno);

        [OperationContract]
        Retorno<bool> InformarPrevisaoPagamentoCTe(int protocoloCTe, string dataPrevisaoPagamento, string observacao);

        [OperationContract]
        Retorno<List<Dominio.ObjetosDeValor.WebService.CTe.InformarPrevisaoPagamentoCTe>> InformarPrevisoesPagamentosCTe(List<Dominio.ObjetosDeValor.WebService.CTe.InformarPrevisaoPagamentoCTe> InformacoesPrevisaoPagamentoCTe);

        [OperationContract]
        Retorno<bool> InformarBloqueioDocumento(int protocoloCTe, string dataBloqueio, string observacao);

        [OperationContract]
        Retorno<bool> InformarDesbloqueioDocumento(Dominio.ObjetosDeValor.WebService.CTe.InformarDesbloqueioDocumento informarDesbloqueioDocumento);

        [OperationContract]
        Retorno<bool> ConfirmarPagamentoCTe(int protocoloCTe, string dataPagamento, string observacao);

        [OperationContract]
        Retorno<Paginacao<int>> BuscarProtocoloCTesCanceladosAguardandoConfirmacaoConsulta(int? inicio, int? limite);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.Titulo>> BuscarTitulosPorPeriodo(int? inicio, int? limite, string dataInicial, string dataFinal);

        [OperationContract]
        Retorno<Dominio.ObjetosDeValor.WebService.CTe.CTe> BuscarCTeCanceladoPorProtocolo(int protocoloCTe, string statusCTe);

        [OperationContract]
        Retorno<bool> ConfirmarConsultaCTeCancelado(int protocoloCTe);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>> BuscarOutrosDocsPorPeriodo(string dataInicial, string dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno? tipoDocumentoRetorno, int? inicio, int? limite);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTeFatura>> BuscarFaturaCTes(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno? tipoDocumentoRetorno, int? inicio, int? limite);

        [OperationContract]
        Retorno<bool> QuitarTituloCTe(int? codigoTitulo, string dataPagamento, string observacao, decimal? valorAcrescimo, decimal? valorDesconto);

        [OperationContract]
        Retorno<Dominio.ObjetosDeValor.WebService.CTe.FaturasCTe> BuscarFaturasCTePorProtocolo(int protocoloCTe);

        [OperationContract]
        Retorno<Dominio.ObjetosDeValor.WebService.CTe.DocumentosCTe> BuscarNotaFiscal(string chaveNFe);

        [OperationContract]
        Retorno<Paginacao<int>> BuscarCTesFaturadosPendenteDeIntegracao(int? inicio, int? limite);

        [OperationContract]
        Retorno<bool> ConfirmarIntegracaoCTeFaturado(int protocoloCTe);

        [OperationContract]
        Retorno<List<Dominio.ObjetosDeValor.WebService.CTe.SituacaoCTe>> VerificarSituacaoCTe(List<string> chavesCTe);

        [OperationContract]
        Retorno<string> EnviarArquivoXMLCTe(Stream arquivo);

        [OperationContract]
        Retorno<bool> AnularCTe(int? protocoloCTe, string dataEventoDesacordo, decimal? valorCTeSustituicao, string cnpjTomador, string observacaoCTeAnulacao, string observacaoCTeSustituicao);

        [OperationContract]
        Retorno<bool> IntegrarDadosCTesAnteriores(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo, List<Dominio.ObjetosDeValor.Embarcador.CTe.CTe> Ctes);

        [OperationContract]
        Retorno<bool> IntegrarCTesAnteriores(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo, List<Dominio.ObjetosDeValor.Embarcador.CTe.TokenCte> TokensXMLCtes);

        [OperationContract]
        Retorno<bool> ReemitirCTesRejeitados(int protocoloCarga);

        [OperationContract]
        Retorno<bool> RetornarIntegracaoPagamento(Dominio.ObjetosDeValor.Embarcador.Escrituracao.RetornoIntegracaoPagamento retornoIntegracaoPagamento);

        [OperationContract]
        Retorno<Dominio.ObjetosDeValor.Embarcador.CTe.CTe> BuscarCTePorChave(string chaveCTe);

        [OperationContract]
        Retorno<bool> EnviarCTeAnterior(Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, string numeroCarga, string numeroBooking, string numeroOS);

        [OperationContract]
        Retorno<bool> EnviarDadosAverbacao(Dominio.ObjetosDeValor.Embarcador.CTe.DadosAverbacao dadosAverbacao, string numeroCarga, string numeroBooking, string numeroOS);

        [OperationContract]
        Retorno<List<Dominio.ObjetosDeValor.Embarcador.CTe.DadosAverbacao>> BuscarDadosAverbacao(string chaveCTe);

        [OperationContract]
        Retorno<bool> SalvarDadosDoMercante(Dominio.ObjetosDeValor.WebService.CTe.DadosDoMercante dadosDoMercante);

        [OperationContract]
        Retorno<bool> IntegrarDadosCTeAnteriores(Dominio.ObjetosDeValor.WebService.CTe.DadosCTes dadosCtes);

        [OperationContract]
        Retorno<bool> RealizarAnulacaoGerencial(RequestAnulacaoGerencial requestAnulacao);

        [OperationContract]
        Retorno<bool> EnviarCTe(Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, int? protocoloCarga);

        [OperationContract]
        Retorno<bool> InformarEscrituracaoCTe(int? protocoloCTe, string codigoEscrituracao);

        [OperationContract]
        Retorno<bool> EnviarCTesAnteriores(Dominio.ObjetosDeValor.WebService.CTe.RequestCteAnteriores requestCteAnteriores);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTe>> BuscarCTesAlteradosPorPeriodo(string dataInicial, string dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno? tipoDocumentoRetorno, int? inicio, int? limite, string codigoTipoOperacao, string situacao);

        [OperationContract]
        Retorno<bool> ConfirmarIntegracaoCTeSubstituto(int protocoloCTe);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTeComplementar>> BuscarCTesSubstitutosAguardandoIntegracao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno? tipoDocumentoRetorno, string codificarUTF8, int? inicio, int? limite);

    }

}
