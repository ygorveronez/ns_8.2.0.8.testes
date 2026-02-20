using System;

namespace Dominio.ObjetosDeValor.Relatorios
{
    public class DetalheFaturaAvon
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
        public string TelefoneEmpresa { get; set; }

        public int Numero { get; set; }
        public DateTime DataVencimento { get; set; }
        public decimal ValorTotal { get; set; }
        public string ValorTotalPorExtenso { get; set; }

        public string NomeTomador { get; set; }
        public string CNPJTomador { get; set; }
        public string IETomador { get; set; }
        public string EnderecoTomador { get; set; }
        public string NumeroTomador { get; set; }
        public string BairroTomador { get; set; }
        public string CidadeTomador { get; set; }
        public string CEPTomador { get; set; }
        public string TelefoneTomador { get; set; }
    }
}
