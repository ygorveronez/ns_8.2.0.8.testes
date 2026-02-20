namespace Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega
{
    public class ConfiguracaoCarga
    {
        public bool ObrigarFotoNaDevolucao { get; set; }
        public bool ObrigarFotoNaEntrega { get; set; }
        public bool ExigeSenhaClienteRecebimento { get; set; }
        public int NumeroTentativasSenhaClientePermitidas { get; set; }
        public bool ForcarPreenchimentoSequencialMobile { get; set; }
        public bool PermiteQRCodeMobile { get; set; }
        public bool ObrigarFotoCanhoto { get; set; }
        public bool ObrigarAssinaturaEntrega { get; set; }
        public bool ObrigarDadosRecebedor { get; set; }
        public string ServerChatURL { get; set; }
        public bool NaoPermiteRejeitarEntrega { get; set; }
        public bool PermiteImpressaoMobile { get; set; }
        public bool ExibirCalculadoraMobile { get; set; }
        public bool PermiteRetificarMobile { get; set; }
        public bool HabilitarControleFluxoNFeDevolucaoChamado { get; set; }
        public bool ObrigarJustificativaSolicitacoesForaAreaCliente { get; set; }
        public bool PermiteFotos { get; set; }
        public int QuantidadeMinimasFotos { get; set; }
        public bool PermiteEventos { get; set; }
        public bool PermiteChat { get; set; }
        public bool PermiteSAC { get; set; }
        public bool ObrigarAssinaturaProdutor { get; set; }
        public bool PermiteCanhotoModoManual { get; set; }
        public bool PermiteConfirmarEntrega { get; set; }
        public bool BloquearRastreamento { get; set; }
        public bool PermiteEntregaParcial { get; set; }
        public bool ControlarTempoEntrega { get; set; }
        public bool ExibirRelatorio { get; set; }
        public bool NaoRetornarColetas { get; set; }

        public bool DevolucaoProdutosPorPeso { get; set; }

        public bool AtualizarCargaAutomaticamente { get; set; }
        public bool PermiteConfirmarChegadaEntrega { get; set; }
        public bool PermiteConfirmarChegadaColeta { get; set; }
        public bool ControlarTempoColeta { get; set; }
        public bool NaoUtilizarProdutosNaColeta { get; set; }
        public bool PermitirVisualisarProgramacaoAntesViagem { get; set; }
        public bool ObrigarHandlingUnit { get; set; }
        public bool ObrigarChavesNfe { get; set; }
        public bool SolicitarReconhecimentoFacialDoRecebedor { get; set; }
        public bool PermiteEnviarNotasComplementaresAposEmissaoDocumentosTransporte { get; set; }
        public bool ValidarCapacidadeMaximaNoApp { get; set; }
        public bool PermiteFotosColeta { get; set; }
        public int QuantidadeMinimasFotosColeta { get; set; }
    }
}
