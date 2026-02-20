using System;

namespace Dominio.ObjetosDeValor.Relatorios
{
    public class DetalheFaturaNatura
    {
        public byte[] Logo { get; set; }
        public string NomeEmpresa { get; set; }
        public string CNPJEmpresa { get; set; }
        public string IEEmpresa { get; set; }
        public string EnderecoEmpresa { get; set; }
        public string NumeroEmpresa { get; set; }
        public string BairroEmpresa { get; set; }
        public string CidadeEmpresa { get; set; }
        public string CEPEmpresa { get; set; }
        public string Telefone { get; set; }
        public int Numero { get; set; }
        public long NumeroPreFatura { get; set; }
        public DateTime DataVencimento { get; set; }
        public decimal ValorTotal { get; set; }
        public decimal ValorDesconto { get; set; }
        public string ValorTotalPorExtenso { get; set; }
        public string Sacado { get; set; }
    }
}
