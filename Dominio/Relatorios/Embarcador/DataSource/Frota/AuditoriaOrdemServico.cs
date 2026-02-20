namespace Dominio.Relatorios.Embarcador.DataSource.Frota
{
    public class AuditoriaOrdemServico
    {
        public int Codigo { get; set; }
        public int Numero { get; set; }
        public string Placa { get; set; }
        public string Observacao { get; set; }
        public string Nome { get; set; }
        public int QuilometragemAtual { get; set; }
        public int HorimetroAtual { get; set; }
        public decimal CustoTotalProduto { get; set; }
    }
}
