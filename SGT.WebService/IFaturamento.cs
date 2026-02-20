using CoreWCF;

namespace SGT.WebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IFaturamento" in both code and config file together.
    [ServiceContract]
    public interface IFaturamento
    {

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Financeiro.DocumentoFaturamento>> BuscarDocumentosPagamentoLiberado(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno? tipoDocumentoRetorno, int? inicio, int? limite);

        [OperationContract]
        Retorno<bool> ConfirmarIntegracaoDocumentoFaturamento(int? protocolo);

    }
}
