using System;

namespace Dominio.ObjetosDeValor.Relatorios
{
    public class RelatorioCTesEmitidosFatura
    {
        public string Fatura { get; set; }
        public int NumeroCte { get; set; }
        public string NumeroNFe { get; set; }
        public DateTime DataEmissao { get; set; }
        public string CEPOrigem { get; set; }
        public string CEPDestino { get; set; }
        public string CidadeDestino { get; set; }
        public string CNPJDestino { get; set; }
        public decimal ValorNFe { get; set; }
        public decimal PesoKG { get; set; }
        public decimal PesoCubado { get; set; }
        public decimal FretePeso { get; set; }
        public decimal FreteValor { get; set; }
        public decimal ValorTDA { get; set; }
        public decimal ValorTDE { get; set; }
        public decimal ValorTRT { get; set; }
        public decimal ValorGRIS { get; set; }
        public decimal ValorTAS { get; set; }
        public decimal ValorOutros { get; set; }
        public decimal ValorPedagio { get; set; }
        public decimal FreteTotal { get; set; }
    }
}
