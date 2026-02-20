using System;

namespace Dominio.ObjetosDeValor.Relatorios
{
    public class RelatorioHistoricosVeiculos
    {
        public int Codigo { get; set; }
        public int CodigoVeiculo { get; set; }
        public string PlacaVeiculo { get; set; }
        public int CodigoServico { get; set; }
        public string DescricaoServico { get; set; }
        public string Observacao { get; set; }
        public DateTime? Data { get; set; }
        public int Km { get; set; }
        public decimal Quantidade { get; set; }
        public decimal Valor { get; set; }
    }
}
