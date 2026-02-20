namespace Dominio.Relatorios.Embarcador.DataSource.OutrosDocumentos.NotaDebito
{
    public class NotaDebitoV2_Item
    {
        public int NumeroItem { get; set; }

        public string Drescricao { get; set; }

        public decimal PrecoUnitario { get; set; }

        public int Quantidade { get; set; }

        public decimal ValorTotal { get; set; }
    }
}
