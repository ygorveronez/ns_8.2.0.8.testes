using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.WebService.NFS
{
    public class RequestNFSOcorrencia
    {
        public int ProtocoloOcorrencia { get; set; }
        public TipoDocumentoRetorno TipoDocumentoRetorno { get; set; }
        public int Inicio { get; set; }
        public int Limite { get; set; }

    }
}
