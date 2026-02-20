using System;

namespace Dominio.Relatorios.Embarcador.DataSource.SAC
{
    public class FaturamentoDocumento
    {
        public int Codigo { get; set; }
        public int CodigoCTe { get; set; }
        public int Numero { get; set; }
        public Int64 PreFatura { get; set; }
        public string Situacao { get; set; }
        public string DatasVencimentos { get; set; }
        public string Status { get; set; }
        public string Titulos { get; set; }
        public decimal Valor { get; set; }
    }
}
