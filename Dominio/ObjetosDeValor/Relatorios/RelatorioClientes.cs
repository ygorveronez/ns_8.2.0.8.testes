using System;

namespace Dominio.ObjetosDeValor.Relatorios
{
    public class RelatorioClientes
    {
        public string CNPJ { get; set; }
        public string IE { get; set; }
        public string Razao { get; set; }
        public string Fantasia { get; set; }
        public string Endereco { get; set; }
        public string Numero { get; set; }
        public string Bairro { get; set; }
        public string CEP { get; set; }
        public string Cidade { get; set; }
        public string UF { get; set; }
        public string Telefone { get; set; }
        public string Email { get; set; }
        public string FreteTipoVeiculo { get; set; }
        public decimal AdValorem { get; set; }
        public decimal ValorDescarga { get; set; }
        public decimal ValorPedagio { get; set; }
        public decimal ValorFrete { get; set; }
        public decimal GRIS { get; set; }
        public DateTime DataCadastro { get; set; }
    }
}
