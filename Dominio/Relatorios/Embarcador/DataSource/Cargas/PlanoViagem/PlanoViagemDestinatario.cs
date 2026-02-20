namespace Dominio.Relatorios.Embarcador.DataSource.Cargas.PlanoViagem
{
    public sealed class PlanoViagemDestinatario
    {
        public string Consignatario { get; set; }

        public string DataPrevisaoChegada{ get; set; }

        public string Destinatario { get; set; }

        public int Ordem { get; set; }
    }
}
