namespace Dominio.Relatorios.Embarcador.DataSource.Financeiros
{
    public class TermoQuitacao
    {
        #region Termo de Quitação Unilateral

        #endregion
        public string RazaoSocialTransportador { get; set; }
        public string CNPJTransportador { get; set; }
        public string InscricaoEstadualTransportador { get; set; }
        public string EnderecoTransportador { get; set; }
        public string NumeroEnderecoTransportador { get; set; }
        public string ComplementoTransportador { get; set; }
        public string BairroTransportador { get; set; }
        public string CidadeTransportador { get; set; }
        public string EstadoTransportador { get; set; }
        public string UFTransportador { get; set; }
        public string CNPJFiliais { get; set; }
        public string DataFinalTermo { get; set; }
        public string DataCriacaoTermo { get; set; }

        public string CNPJFiliaisFormatado
        {
            get { return !string.IsNullOrWhiteSpace(CNPJFiliais) ? $"e filiais: {CNPJFiliais}" : string.Empty; }
        }

    }
}
