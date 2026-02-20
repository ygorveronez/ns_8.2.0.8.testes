using System;

namespace Dominio.Relatorios.Embarcador.DataSource.NFe
{
    public class DANFEParcela
    {
        public int CodigoNota { get; set; }
        public int CodigoParcela { get; set; }
        public int SequenciaParcela { get; set; }
        public DateTime DataVencimentoParcela { get; set; }
        public string NumeroParcela { get; set; }
        public decimal ValorParcela { get; set; }

    }
}
