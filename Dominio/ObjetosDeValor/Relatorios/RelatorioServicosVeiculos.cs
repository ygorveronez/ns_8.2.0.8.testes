using System;

namespace Dominio.ObjetosDeValor.Relatorios
{
    public class RelatorioServicosVeiculos
    {
        public DateTime? Data { get; set; }
        public string Placa { get; set; }
        public int CodigoVeiculo { get; set; }
        public int KMVeiculo { get; set; }
        public int KMTroca { get; set; }
        public int DiasTroca { get; set; }
        public int KMAtual { get; set; }
        public int CodigoServico { get; set; }
        public string DescricaoServico { get; set; }
        public int DiasAviso { get; set; }
        public DateTime? DataVcto { get; set; }
    }
}
