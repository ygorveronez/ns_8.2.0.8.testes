using System.Collections.Generic;
using CoreWCF;

namespace SGT.WebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IFinanceiro" in both code and config file together.
    [ServiceContract]
    public interface IFinanceiro
    {
        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Financeiro.TituloQuitado>> BuscarTituloAReceberQuitadoPendentesIntegracao(int? inicio, int? limite);

        [OperationContract]
        Retorno<bool> ConfirmarIntegracaoTituloQuitado(int protocolo);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Financeiro.DocumentoEntrada>> ConsultarDocumentoEntradaPorOrdemCompra(int protocoloOrdemCompra);

        [OperationContract]
        Retorno<bool> QuitarTituloPagar(int? protocolo, string dataPagamento, string observacao, decimal? valorAcrescimo, decimal? valorDesconto, string codigoIntegracaoFormaPagamento);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Financeiro.Titulo>> BuscarTitulosPagarPendenteIntegracao(int? inicio, int? limite, string codigoIntergracaoTipoMovimento);

        [OperationContract]
        Retorno<bool> ConfirmarIntegracaoTituloPagar(int protocolo);

        [OperationContract]
        Retorno<bool> EnviarFaturaCompleta(Dominio.ObjetosDeValor.Embarcador.Fatura.FaturaIntegracao faturaIntegracao);

        [OperationContract]
        Retorno<List<Dominio.ObjetosDeValor.Embarcador.Financeiro.DocumentoEntrada>> BuscarDocumentoEntradaPendenteIntegracao(int? inicio, int? quantidadeRegistros, string codigoTipoMovimento);

        [OperationContract]
        Retorno<bool> ConfirmarIntegracaoDocumentoEntrada(List<int> protocolos);

        [OperationContract]
        Retorno<bool> ConfirmarIntegracaoTituloFinanceiro(List<int> protocolos);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Financeiro.Titulo>> BuscarTitulosPentendesIntegracaoERP(int? quantidade);

        [OperationContract]
        Retorno<bool> IndicarAntecipacaoFreteDocumento(Dominio.ObjetosDeValor.WebService.Financeiro.DocumentoAntecipacao documentoAntecipacao);

        [OperationContract]
        Retorno<bool> ReceberDocumentoEntradaTMS(Dominio.ObjetosDeValor.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntradaTMS);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Frota.PneuHistorico>> ConsultarMovimentacoesPneusPendentesIntegracao(int? inicio, int? limite);

        [OperationContract]
        Retorno<bool> ConfirmarMovimentacaoPneuPendenteIntegracao(int protocolo);

        [OperationContract]
        Retorno<Dominio.ObjetosDeValor.Embarcador.Fatura.RetornoFaturaInserir> GerarFaturaCompleta(Dominio.ObjetosDeValor.Embarcador.Fatura.GerarFaturaCompletaIntegracao envGerarFaturaCompletaIntegracao);

        [OperationContract]
        Retorno<Dominio.ObjetosDeValor.Embarcador.Fatura.RetornoFaturaInserir> CancelarFatura(int codigo, string motivo, string codigoIntegracaoOperador);
        
        [OperationContract]
        Retorno<bool> RecebePDF(int codigo, string boleto, string numero);

        [OperationContract]
        Retorno<bool> QuitarTituloReceber(Dominio.ObjetosDeValor.WebService.Financeiro.QuitarTituloReceber quitarTituloReceber);

    }
}
