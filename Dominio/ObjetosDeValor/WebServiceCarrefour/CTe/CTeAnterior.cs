using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebServiceCarrefour.CTe
{
    public sealed class CTeAnterior
    {
        public string CNPJCPF { get; set; }

        public string IERG { get; set; }

        public string TipoDocumento { get; set; }

        public string ChaveCTe { get; set; }

        public int Numero { get; set; }

        public string Serie { get; set; }

        public string DataHoraEmissao { get; set; }

        public decimal ValorMercadoria { get; set; }

        public decimal PesoTotal { get; set; }

        public decimal ValorTotal { get; set; }

        public List<CTeAnteriorNota> Notas { get; set; }
    }
}
