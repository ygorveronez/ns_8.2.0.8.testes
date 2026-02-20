namespace Dominio.ObjetosDeValor.Embarcador.Integracao
{
    public sealed class FiltroPesquisaControleIntegracaoCargaEDI
    {
        public int CodigoCarga { get; set; }

        public bool TelaCarga { get; set; }

        public string CodigoCargaEmbarcador { get; set; }

        public int CodigoTransportador { get; set; }

        public System.DateTime? DataFinal { get; set; }

        public System.DateTime? DataInicial { get; set; }

        public System.TimeSpan? HoraFinal { get; set; }

        public System.TimeSpan? HoraInicial { get; set; }

        public string IDOC { get; set; }

        public string NomeArquivo { get; set; }

        public string Placa { get; set; }

        public Enumeradores.SituacaoIntegracaoCargaEDI Situacao { get; set; }
    }
}
