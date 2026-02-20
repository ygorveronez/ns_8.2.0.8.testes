namespace Dominio.ObjetosDeValor.WebService.Monitoramento
{
    public sealed class Cliente
    {
        public string Nome { get; set; }

        public string Coordenada { get; set; }

        public string TpRastreador { get; set; }

        public string DataPrevista { get; set; }

        public string HoraPrevista { get; set; }

        public string TpPrevisao { get; set; }

        public string DataEntrada { get; set; }

        public string HoraEntrada { get; set; }

        public string DataSaida { get; set; }

        public string HoraSaida { get; set; }

        public string Ocorrencia { get; set; }

        public string Ordem { get; set; }
    }
}