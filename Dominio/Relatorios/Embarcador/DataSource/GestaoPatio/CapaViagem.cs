namespace Dominio.Relatorios.Embarcador.DataSource.GestaoPatio
{
    public class CapaViagem
    {
        public string DataViagem { get; set; }
        public string Roteiro { get; set; }
        public string PlacaVeiculo { get; set; }
        public string Transportadora { get; set; }
        public string Motorista { get; set; }
        public string Viagem { get; set; }
        
        public string RoteiroDestino { get; set; }
        public string Descricao { get; set; }

        public string Observacao { get; set; }
        public string Temperatura { get; set; }
        public string DataPrimeiraEntrega { get; set; }

        public string ValorTotalNFs { get; set; }
        public string TotalPesoLiquido { get; set; }
        public string TotalPesoBruto { get; set; }
        public string TotalPesoPallet { get; set; }

        public string PalletsQuantidadeSaida { get; set; }
        public string PalletsQuantidadeEntrada { get; set; }
        public string DataHoraEntrada { get; set; }
        public string DataLiberacaoChave { get; set; }
        public string Mensagem { get; set; }
    }
}
