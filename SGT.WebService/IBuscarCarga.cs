using System.Collections.Generic;
using CoreWCF;

namespace SGT.WebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IBuscarCarga" in both code and config file together.
    [ServiceContract]
    public interface IBuscarCarga
    {
        [OperationContract]
        Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao>> BuscarDadosCarga(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo);
    }
}
