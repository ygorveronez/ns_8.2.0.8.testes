using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.NFS
{
    public class NFSe
    {
        public Dominio.ObjetosDeValor.WebService.NFS.NaturezaNFSe NaturezaNFSe { get; set; }
        public int Serie { get; set; }
        public int Numero { get; set; }
        public Dominio.Enumeradores.StatusNFSe StatusNFSe { get; set; }
        public string DataEmissao { get; set; }
        public string DataCancelamento { get; set; }
        public string XML { get; set; }
        public string PDF { get; set; }
        public string MotivoCancelamento { get; set; }
        public string NumeroCarga { get; set; }
        public int ProtocoloCarga { get; set; }
        public string NumeroProtocolo { get; set; }
        public int NumeroRPS { get; set; }
        public string SerieRPS { get; set; }
        public string CodigoVerificacao { get; set; }
        public int NumeroNFSeComplementada { get; set; }
        public int SerieNFSeComplementada { get; set; }
        public List<Dominio.ObjetosDeValor.WebService.CTe.DocumentosCTe> Documentos { get; set; }
        public Dominio.ObjetosDeValor.WebService.NFS.ServicoNFSe ServicoNFSe { get; set; }
    }
}
