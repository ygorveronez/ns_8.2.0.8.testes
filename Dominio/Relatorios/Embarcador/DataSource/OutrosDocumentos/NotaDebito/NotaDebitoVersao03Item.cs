namespace Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.NotaDebito
{
    public sealed class NotaDebitoVersao03Item
    {
        public string Descricao { get; set; }

        public string NfeCteOrigem { get; set; }

        public int Numero { get; set; }

        public string NumeroCteOrigem { get; set; }

        public decimal PrecoUnitario { get; set; }

        public int Quantidade { get; set; }

        public string SerieCteOrigem { get; set; }

        public decimal ValorTotal { get; set; }
    }
}
