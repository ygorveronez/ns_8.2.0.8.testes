using System.Collections.Generic;
using CoreWCF;

namespace SGT.WebService
{
    [ServiceContract]
    public interface IModeloVeicular
    {
        [OperationContract]
        Retorno<List<Dominio.ObjetosDeValor.Embarcador.Carga.ModeloVeicular>> BuscarModelosVeicularesDisponiveis();
    }
}
