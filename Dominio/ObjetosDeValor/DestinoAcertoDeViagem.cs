namespace Dominio.ObjetosDeValor
{
    public class DestinoAcertoDeViagem
    {
        public int Codigo { get; set;  }
        public int CodigoTipoCarga { get; set; }
        public string DescricaoTipoCarga { get; set; }
        public string DescricaoCTe { get; set; }
        public int CodigoCTe { get; set; }
        public string DescricaoCliente { get; set; }
        public string CodigoCliente { get; set; }
        public decimal KMInicial { get; set; }
        public decimal KMFinal { get; set; }
        public string DataInicial { get; set; }
        public string DataFinal { get; set; }
        public string UFOrigem { get; set; }
        public int MunicipioOrigem { get; set; }
        public string DescricaoOrigem { get; set; }
        public string UFDestino { get; set; }
        public int MunicipioDestino { get; set; }
        public string DescricaoDestino { get; set; }
        public string ValorFrete { get; set; }
        public string ValorUnitario { get; set; }
        public string OutrosDescontos { get; set; }
        public string Peso { get; set; }
        public string Observacao { get; set; }
        public bool Excluir { get; set; }
    }
}
