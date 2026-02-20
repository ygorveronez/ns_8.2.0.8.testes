namespace Dominio.Relatorios.Embarcador.DataSource.Terceiros
{
    public class RomaneioEntregaItem
    {
        public string CPFCNPJRemetente { get; set; }
        public string NomeRemetente { get; set; }
        public int NumeroCTe { get; set; }
        public int NumeroNF { get; set; }
        public string NumeroPedido { get; set; }
        public string CPFCNPJDestinatario { get; set; }
        public string NomeDestinatario { get; set; }
        public string EnderecoDestinatario { get; set; }
        public string CidadeDestino { get; set; }
        public string UFDestino { get; set; }
        public int Volumes { get; set; }
        public decimal Peso { get; set; }
    }
}
