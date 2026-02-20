namespace Dominio.Relatorios.Embarcador.DataSource.Cargas.PlanoViagem
{
    public sealed class PlanoViagemEntrega
    {
        public string Cidade { get; set; }

        public string DataEntrega { get; set; }

        public string DataEntregaPrevista { get; set; }

        public string DistanciaAcumulada { get; set; }

        public string DistanciaParcial { get; set; }

        public string Endereco { get; set; }

        public int Ordem { get; set; }

        public string PontoParada { get; set; }

        public string Tipo { get; set; }

        public string UF { get; set; }
    }
}
