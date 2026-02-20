namespace Dominio.Relatorios.Embarcador.DataSource.Logistica
{
    public class OrdemColetaGuaritaPedido
    {
        public int CodigoPedido { get; set; }
        public string Numero { get; set; }
        public string DestinatarioNome { get; set; }
        public string DestinatarioCPFCNPJ { get; set; }
        public string DestinatarioIE { get; set; }
        public string DestinatarioCidade { get; set; }
        public string DestinatarioEstado { get; set; }
        public string DestinatarioCEP { get; set; }
        public string DestinatarioTelefone { get; set; }
        public string DestinatarioEmail { get; set; }
        public string DestinatarioEndereco { get; set; }
        public decimal Peso { get; set; }
        public int NumeroPallets { get; set; }
        public decimal PesoPallets { get; set; }
        public string Codigo { get; set; }
        public string Descricao { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal ValorTotal { get; set; }
        public decimal PesoUnitario { get; set; }
        public decimal PesoTotal { get; set; }
        public decimal QuantidadeEmbalagem { get; set; }
        public decimal PesoTotalEmbalagem { get; set; }
        public decimal Quantidade { get; set; }
        public decimal QuantidadePlanejada { get; set; }
    }
}
