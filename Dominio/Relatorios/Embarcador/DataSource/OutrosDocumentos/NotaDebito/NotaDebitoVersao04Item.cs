namespace Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.NotaDebito
{
    public sealed class NotaDebitoVersao04Item
    {
        public string CodigoCargaEmbarcador { get; set; }

        public string Descricao { get; set; }

        public string NfeCteOrigem { get; set; }

        public int Numero { get; set; }

        public string NumeroCteOrigem { get; set; }

        public string Ordens { get; set; }

        public decimal PrecoUnitario { get; set; }

        public int Quantidade { get; set; }

        public decimal ValorTotal { get; set; }
    }
}
