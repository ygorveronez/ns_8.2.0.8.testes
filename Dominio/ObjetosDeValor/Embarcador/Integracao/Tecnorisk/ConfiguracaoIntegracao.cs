namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Tecnorisk
{
    public sealed class ConfiguracaoIntegracao
    {
        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string Url { get; set; }

        public int PGRId { get; set; }

        public int IDPropriedadeMonitoramento { get; set; }
        public int CargaMercadoria { get; set; }
    }
}
