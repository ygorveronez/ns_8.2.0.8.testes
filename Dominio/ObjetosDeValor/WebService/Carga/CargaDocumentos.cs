using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Carga
{
    public class CargaDocumentos
    {
        public int ProtocoloCarga { get; set; }
        public decimal PesoBruto { get; set; }
        public decimal PesoLiquido { get; set; }
        public decimal ValorICMS { get; set; }
        public decimal ValorFrete { get; set; }
        public decimal ValorFreteAPagar { get; set; }
        public decimal ValorFreteLiquido { get; set; }
        public decimal ValorFreteEmbarcador { get; set; }

        public List<Dominio.ObjetosDeValor.WebService.CTe.CTe> Conhecimentos { get; set; }
    }
}
