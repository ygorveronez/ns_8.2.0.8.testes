namespace Dominio.Relatorios.Embarcador.DataSource.Cargas.PlanoViagem
{
    public sealed class PlanoViagemDocumento
    {
        public string Destino { get; set; }

        public string Numero { get; set; }

        public int Ordem { get; set; }

        public string Tipo { get; set; }

        public string ClassificacaoNFe { get; set; }
    }
}
