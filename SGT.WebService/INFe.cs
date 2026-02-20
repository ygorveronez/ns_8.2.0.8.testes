using CoreWCF;

namespace SGT.WebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "INFe" in both code and config file together.
    [ServiceContract]
    public interface INFe
    {
        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal>> BuscarNotasFiscaisVinculadas(int? protocoloCarga, int? protocoloPedido, string chaveCTe, int? inicio, int? limite);

        [OperationContract]
        Retorno<string> EnviarStreamImagemCanhoto(Stream imagem);

        [OperationContract]
        Retorno<bool> EnviarImagemCanhotoLeituraOCR(int usuario, string tokenImagem);

        [OperationContract]
        Retorno<bool> EnviarImagemCanhoto(string latitude, string longitude, int? usuario, int? canhoto, string tokenImagem, DateTime? dataEntregaNotaCliente, string chaveNFe, int? numeroNota, int? serieNota, string cnpjCpfEmissor);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>> BuscarCargasAguardandoNotasFiscais(int? inicio, int? limite, string codigoTipoOperacao);

        [OperationContract]
        Retorno<bool> ConfirmarRecebimentoCargaAguardandoNotasFiscais(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo);

        [OperationContract]
        Retorno<string> EnviarArquivoXMLNFe(Stream arquivo);

        [OperationContract]
        Retorno<bool> IntegrarNotasFiscais(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo, List<Dominio.ObjetosDeValor.Embarcador.NFe.TokenNF> TokensXMLNotasFiscais);

        [OperationContract]
        Retorno<bool> IntegrarNotasFiscaisComAverbacaoeValePedagio(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo, List<Dominio.ObjetosDeValor.Embarcador.NFe.TokenNF> TokensXMLNotasFiscais, Dominio.ObjetosDeValor.Embarcador.Carga.Averbacao Averbacao, Dominio.ObjetosDeValor.MDFe.ValePedagio ValePedagio, Dominio.ObjetosDeValor.MDFe.CIOT Ciot, Dominio.ObjetosDeValor.MDFe.InformacoesPagamentoPedido InformacoesPagamento);

        [OperationContract]
        Retorno<int> IntegrarNotaFiscal(Dominio.ObjetosDeValor.Embarcador.NFe.TokenNF tokenXML);

        [OperationContract]
        Retorno<bool> InformarCancelamentoNotaFiscal(int protocoloNFe);

        [OperationContract]
        Retorno<bool> IntegrarNotaFiscalPorPedido(Dominio.ObjetosDeValor.WebService.Carga.Pedido Pedido, Dominio.ObjetosDeValor.Embarcador.NFe.TokenNF TokenXMLNotaFiscal);

        [OperationContract]
        Retorno<bool> SolicitarNotasFiscais(int protocoloCarga);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.NFe.CanhotoAvulso>> BuscarCanhotosAvulsos(int protocoloIntegracaoCarga, int? inicio, int? limite);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal>> BuscarNotasFiscaisComRecebimentoConfirmadoNoDestinatario(int? inicio, int? limite);

        [OperationContract]
        Retorno<bool> ConfirmarRecebimentoNotaFiscalComRecebimentoConfirmadoNoDestinatario(int protocoloNFe);

        [OperationContract]
        Retorno<bool> ExcluirNotaFiscalPorChave(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo, string chaveNFe);

        [OperationContract]
        Retorno<bool> ExcluirNotaFiscal(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo, int? protocoloNotaFiscal);

        [OperationContract]
        Retorno<bool> RemoverNotaFiscal(Dominio.ObjetosDeValor.WebService.Carga.NotaFiscal notaFiscal);

        [OperationContract]
        Retorno<bool> InformarNotasFiscaisPorNumeroControle(List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal> notasFiscais, string NumeroControle);

        [OperationContract]
        Retorno<bool> InformarDadosContabeisNotaFiscal(List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal> notasFiscais);

        [OperationContract]
        Retorno<bool> IntegrarDadosNotasFiscais(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo, List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal> notasFiscais, Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracaoCompleta cargaIntegracaoCompleta);

        [OperationContract]
        Retorno<string> EnviarStreamImagemOcorrencia(Stream imagem);

        [OperationContract]
        Retorno<bool> EnviarDigitalizacaoCanhoto(Dominio.ObjetosDeValor.Embarcador.NFe.Canhoto canhotoIntegracao);

        [OperationContract]
        Retorno<bool> EnviarImagemOcorrencia(int? usuario, int? ocorrencia, string tokenImagem);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.NFe.Canhoto>> BuscarCanhotosNotasFiscaisDigitalizados(int? inicio, int? limite);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.NFe.Canhoto>> BuscarCanhotosDigitalizadoseAgAprovacao(int? inicio, int? limite);

        [OperationContract]
        Retorno<bool> ConfirmarIntegracaoDigitalizacaoCanhotoNotasFiscais(int protocolo);

        [OperationContract]
        Retorno<Dominio.ObjetosDeValor.Embarcador.NFe.CanhotoNotaFiscal> BuscarCanhoto(int protocolo);

        [OperationContract]
        Retorno<Dominio.ObjetosDeValor.Embarcador.NFe.CanhotoNotaFiscal> BuscarCanhotoPorChaveNFe(string chaveNFe);

        [OperationContract]
        Retorno<Dominio.ObjetosDeValor.Embarcador.Enumeradores.RetornoSolicitacaoCancelamento> SolicitarCancelamentoDoPedido(int protocoloIntegracaoPedido, string motivoDoCancelamento);

        [OperationContract]
        Retorno<bool> EnvioNFeColeta(string numeroOS, string numeroContainer, decimal? taraContainer, List<string> listaLacres, List<string> listaChaves);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.NFe.DadosNotaFiscal>> BuscarNotasPorNumeroCarga(List<string> numerosCarga, string dataCriacaoCargaInicial, string dataCriacaoCargaFinal, string codigoIntegracaoFilial, string destinatario, string codigoTipoOperacao);

        [OperationContract]
        Retorno<string> EnviarXMLNotaFiscal(Stream arquivo);

        [OperationContract]
        Retorno<bool> VincularNotaFiscal(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo, List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscalChave> listaNotasFiscaisChaves);

        [OperationContract]
        Retorno<bool> ConfirmarEtapaNFe(Dominio.ObjetosDeValor.Embarcador.NFe.ConfirmarEtapaNFe confirmarEtapaNFe);

    }
}
