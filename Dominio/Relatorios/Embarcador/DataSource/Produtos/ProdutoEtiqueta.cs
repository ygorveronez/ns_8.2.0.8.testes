namespace Dominio.Relatorios.Embarcador.DataSource.Produtos
{
    public class ProdutoEtiqueta
    {
        public byte[] QRCode { get; set; }

        public string LocalArmazenamento { get; set; }

        public int Codigo { get; set; }

        public string Descripcao { get; set; }
    }
}