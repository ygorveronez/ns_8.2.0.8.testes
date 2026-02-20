namespace Dominio.ObjetosDeValor.Embarcador.Ocorrencia
{
    public class RetornoDetalhesOcorrencia
    {
        public int NumeroOcorrencia { get; set; }
        public string Data { get; set; }
        public string Observacao { get; set; }        
        public string Situacao { get; set; }
        public decimal ValorOcorrencia { get; set; }
        public string CodigoIntegracaoTipoOcorrencia { get; set; }
        public string DescricaoTipoOcorrencia { get; set; }
        public string NumeroCarga { get; set; }
        public string CodigoIntegracaoFilial { get; set; }
        public string DescricaoFilial { get; set; }
        public string CPFSolicitante { get; set; }
        public string NomeSolicitante { get; set; }
        public string CPFAprovador { get; set; }
        public string NomeAprovador { get; set; }
        public string CodigoMotivoAprovacao { get; set; }
        public string DescricaoMotivoAprovacao { get; set; }
        public string CentroCusto { get; set; }

    }
}
