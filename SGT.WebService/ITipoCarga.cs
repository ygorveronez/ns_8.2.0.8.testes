using System.Collections.Generic;
using CoreWCF;

namespace SGT.WebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ITipoCarga" in both code and config file together.
    [ServiceContract]
    public interface ITipoCarga
    {
        [OperationContract]
        Retorno<List<Dominio.ObjetosDeValor.Embarcador.Carga.TipoCargaEmbarcador>> BuscarTiposCargasDisponiveis();
    }
}
