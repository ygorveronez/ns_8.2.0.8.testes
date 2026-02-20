using System.Collections.Generic;
using CoreWCF;

namespace SGT.WebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IOrdemCompra" in both code and config file together.
    [ServiceContract]
    public interface IOrdemCompra
    {
        [OperationContract]
        Retorno<bool> AdicionarOrdemCompra(Dominio.ObjetosDeValor.WebService.OrdemCompra.AdicionarOrdemCompra adicionarOrdemCompra);
    }
}
