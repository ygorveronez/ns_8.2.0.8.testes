namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy
{
    public class ChecklistConfiguracoes
    {
        public string? HelperTextEtapaChecklist { get; set; }
        public string? TipoAlertaEtapaChecklist { get; set; }
        public string? TituloAlertaEtapaChecklist { get; set; }
        public string? DescricaoAlertaEtapaChecklist { get; set; }
        public string? PlaceHolderEtapaChecklist { get; set; }
        public int? ValorMinimoEtapaChecklist { get; set; }
        public int? ValorMaximoEtapaChecklist { get; set; }
        public int? QuantidadeMinimaEtapaChecklist { get; set; }
        public int? QuantidadeMaximaEtapaChecklist { get; set; }
        public bool? GaleriaHabilitadaEtapaChecklist { get; set; }
        public bool? PermitirPausarEtapaChecklist { get; set; }
        public int? TempoEsperaEtapaChecklist { get; set; }
        public string? TipoProcessamentoImagemEtapaChecklist { get; set; }
        public double? ThresholdEtapaChecklist { get; set; }
        public string? ModoValidacaoImagemEtapaChecklist { get; set; }
        public bool? UtilizarMascaraImagemValidatorEtapaChecklist { get; set; }
        public bool? LocalizacaoBloquearAvancoEtapa { get; set; }
        public bool? LocalizacaoPodeAvancarForaRaio { get; set;}
        public bool? LocalizacaoObrigarImagemComprovacao { get; set; }
        public bool? UsarDataAtualComoInicial { get; set; }

        public bool? MetadadosImagemMostrarLogo { get; set; }
        public bool? MetadadosImagemMostrarData { get; set; }
        public bool? MetadadosImagemMostrarHora { get; set; }
        public bool? MetadadosImagemMostrarLocalizacao { get; set; }
        public bool? MetadadosImagemMostrarNomeMotorista { get; set; }
        public bool? UtilizarNumerosDecimais { get; set; }
        public int? ValidacaoAdicionalTexto { get; set; }
    }
}