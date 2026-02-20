using CoreWCF;

namespace SGT.WebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IValePedagio" in both code and config file together.
    [ServiceContract]
    public interface IValePedagio
    {
        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.ValePedagio.ValePedagio>> BuscarValePedagioPorCarga(int? protocoloCarga, int? inicio, int? limite);

        [OperationContract]
        Retorno<Dominio.ObjetosDeValor.WebService.ValePedagio.ConsultaValePedagio> ObterConsultaValePedagioPorCarga(int protocoloCarga);

        [OperationContract]
        Retorno<bool> IntegrarValePedagio(Dominio.ObjetosDeValor.WebService.ValePedagio.ValePedagio valePedagio);


    }
}
