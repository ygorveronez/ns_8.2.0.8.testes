using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.ServiceModel;

namespace SGT.WebServiceCarrefour
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ICTe" in both code and config file together.
    [ServiceContract]
    public interface ICTe
    {
        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.WebServiceCarrefour.CTe.CTe>> BuscarCTes(Dominio.ObjetosDeValor.WebServiceCarrefour.Carga.Protocolos protocolo, TipoDocumentoRetorno tipoDocumentoRetorno, int inicio, int limite);
    }
}
