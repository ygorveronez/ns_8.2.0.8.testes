namespace Dominio.Relatorios.Embarcador.DataSource.Cargas.FichaMotorista
{
    public class FichaMotorista
    {
        public string Data { get; set; }

        public string NomeMatriz { get; set; }
        public string EnderecoMatriz { get; set; }
        public string LocalidadeMatriz { get; set; }

        public string NomeEmpresa { get; set; }
        public string PrimeiroTelefone { get; set; }
        public string PrimeiroEmail { get; set; }

        public string NomeMotorista { get; set; }
        public string RGMotorista { get; set; }
        public string NrCNHRegistroMotorista { get; set; }
        public string CategoriaCNHMotorista { get; set; }
        public string NrCNHDocumentoMotorista { get; set; }
        public string NomePaiMotorista { get; set; }
        public string CidadeNascimentoMotorista { get; set; }
        public string CidadeMotorista { get; set; }
        public string EnderecoMotorista { get; set; }
        public string DataEmissaoRGMotorista { get; set; }
        public string DataValidadeRGMotorista { get; set; }
        public string DataPrimeiraHabilitacaoMotorista { get; set; }
        public string NomeMaeMotorista { get; set; }
        public string EstadoNascimentoMotorista { get; set; }
        public string BairroMotorista { get; set; }
        public string TelefoneMotorista { get; set; }
        public string CPFMotorista { get; set; }
        public string DataNascimentoMotorista { get; set; }
        public string NrPISMotorista { get; set; }
        public string CidadaEmissaoPISMotorista { get; set; }
        public string EstadoCivilMotorista { get; set; }
        public string CEPMotorista { get; set; }
        public string CodigoIntegracaoMotorista { get; set; }

        #region Veiculo
        public string PlacaVeiculo { get; set; }
        public string TaraVeiculo { get; set; }
        public string MarcaModeloVeiculo { get; set; }
        public string TipoVeiculo { get; set; }
        public string NrRenavamVeiculo { get; set; }
        public string ComprimentoVeiculo { get; set; }
        public string NrChassisVeiculo { get; set; }
        public string LarguraVeiculo { get; set; }
        public string EspecieTipoVeiculo { get; set; }
        public string CidadeEstadoVeiculo { get; set; }
        public string RNTRCANTTVeiculo { get; set; }
        public string NrEixosVeiculo { get; set; }
        public string CorVeiculo { get; set; }
        public string AnoFabModeloVeiculo { get; set; }

        #endregion

        #region ProprietarioVeiculo
        public string NomeProprietarioVeiculo { get; set; }
        public string EnderecoProprietarioVeiculo { get; set; }
        public string BairroProprietarioVeiculo { get; set; }
        public string CPFCNPJProprietarioVeiculo { get; set; }
        public string CidadeEstadoProprietarioVeiculo { get; set; }
        public string INSSProprietarioVeiculo { get; set; }
        public string CEPProprietarioVeiculo { get; set; }
        public string EmailProprietarioVeiculo { get; set; }
        public string TelefoneProprietarioVeiculo { get; set; }
        #endregion

    }
}
