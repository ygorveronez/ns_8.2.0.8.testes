using System.ServiceModel;
using System.Threading.Tasks;

namespace SGT.WebServiceCarrefour
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ICargas" in both code and config file together.
    [ServiceContract]
    public interface ICargas
    {
        [OperationContract]
        Retorno<Dominio.ObjetosDeValor.WebServiceCarrefour.Carga.Protocolos> AdicionarCarga(Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao);
    }
}
