using System;

namespace Dominio.ObjetosDeValor.Relatorios
{
    public class VisualizacaoDuplicataParcelas
    {
        public int CodigoDuplicata { get; set; }

        public int CodigoParcela { get; set; }

        public int Parcela { get; set; }

        public decimal ValorParcela { get; set; }

        public DateTime DataVcto { get; set; }

        public decimal ValorPgto { get; set; }

        public DateTime? DataPgto { get; set; }

        public string ObservacaoBaixa { get; set; }

    }
}
