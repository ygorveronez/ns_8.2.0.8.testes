using Dominio.ObjetosDeValor.WebService.Faturamento;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Financeiro
{
    public class DocumentoFaturamento
    {
        public int ProtocoloDocumento { get; set; }
        public Dominio.ObjetosDeValor.WebService.CTe.CTe CTe { get; set; }
        public Dominio.ObjetosDeValor.WebService.NFS.NFS NFS { get; set; }
        public List<DadoContabel> DadosContaveis { get; set; }
    }
}
