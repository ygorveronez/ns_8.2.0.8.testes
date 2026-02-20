namespace Dominio.ObjetosDeValor.Embarcador.Pessoas
{
    public class ClienteComplementar
    {
        public int Codigo { get; set; }
        public string Matriz { get; set; }
        public string EscritorioVendas { get; set; }
        public double CpfCnpjCliente { get; set; }
        public string ParticionamentoVeiculo { get; set; }
        public string DescricaoParticionamentoVeiculo { get; set; }
        public string MatrizReferencia { get; set; }
        public bool? SegundaRemessa { get; set; }
        public  bool? ExclusividadeEntrega { get; set; }
        public  string Paletizacao { get; set; }
        public  bool? ClienteStrechado { get; set; }
        public  string Agendamento { get; set; }
        public  bool? ClienteComMulta { get; set; }
        public  string CapacidadeRecebimento { get; set; }
        public  decimal CustoDescarga { get; set; }
        public  string TipoCusto { get; set; }
        public  string Ajudantes { get; set; }
        public  string PagamentoDescarga { get; set; }
        public  string DescricaoPagamentoDescarga { get; set; }
        public  string AlturaRecebimento { get; set; }
        public  string DescricaoAlturaRecebimento { get; set; }
        public  string RestricaoCarregamento { get; set; }
        public  string DescricaoRestricaoCarregamento { get; set; }
        public  string ComposicaoPalete { get; set; }
        public  string DescricaoComposicaoPalete { get; set; }
        public  string SegundaFeira { get; set; }
        public  string TercaFeira { get; set; }
        public  string QuartaFeira { get; set; }
        public  string QuintaFeira { get; set; }
        public  string SextaFeira { get; set; }
        public  string Sabado { get; set; }
        public  string Domingo { get; set; }
    }
}
