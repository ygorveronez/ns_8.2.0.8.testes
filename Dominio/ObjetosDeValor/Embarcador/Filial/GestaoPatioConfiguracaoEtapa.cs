namespace Dominio.ObjetosDeValor.Embarcador.Filial
{
    public sealed class GestaoPatioConfiguracaoEtapa
    {
        public bool? BloquearEdicaoDadosTransporteJanelaTransportador { get; set; }

        public bool? BloquearEdicaoVeiculosCarga { get; set; }

        public Enumeradores.EtapaFluxoGestaoPatio Etapa { get; set; }

        public int Ordem { get; set; }

        public bool PossuiFiliaisExclusivas { get; set; }

        public Enumeradores.SituacaoConfirmacaoEtapaFluxoGestaoPatio SituacaoConfirmacao { get; set; }
    }
}
