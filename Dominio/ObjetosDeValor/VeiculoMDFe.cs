namespace Dominio.ObjetosDeValor
{
    public class VeiculoMDFe
    {
        public int Codigo { get; set; }

        public string Placa { get; set; }

        public int Tara { get; set; }

        public int CapacidadeKG { get; set; }

        public int CapacidadeM3 { get; set; }

        public string RNTRC { get; set; }

        public string TipoRodado { get; set; }

        public string TipoCarroceria { get; set; }

        public string UF { get; set; }

        public string Nome { get; set; }

        public string CPFCNPJ { get; set; }

        public string IE { get; set; }

        public string UFProprietario { get; set; }

        public string TipoProprietario { get; set; }

        public string RENAVAM { get; set; }

        public string CNPJFornecedorValePedagio { get; set; }

        public string CNPJResponsavelValePedagio { get; set; }
        public string NumeroCompraValePedagio { get; set; }
        public decimal ValorValePedagio { get; set; }
        public bool Excluir { get; set; }
    }
}
