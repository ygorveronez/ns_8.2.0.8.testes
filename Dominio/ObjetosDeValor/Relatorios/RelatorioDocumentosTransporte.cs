using System;

namespace Dominio.ObjetosDeValor.Relatorios
{
    public class RelatorioDocumentosTransporte
    {
        public long Numero { get; set; }

        public DateTime Data { get; set; }

        public decimal Valor { get; set; }

        public string Veiculo { get; set; }

        public string Motorista { get; set; }

        public string Status { get; set; }

        public string Notas { get; set; }


        public string TipoDocumento { get; set; }
        public int DocumentoNumero { get; set; }

        public string DocumentoSerie { get; set; }

        public DateTime? DocumentoDataEmissao { get; set; }

        public decimal DocumentoValor { get; set; }
    }
}
