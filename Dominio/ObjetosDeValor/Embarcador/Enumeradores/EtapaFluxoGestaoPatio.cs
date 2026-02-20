namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum EtapaFluxoGestaoPatio
    {
        Todas = 0,
        InformarDoca = 5,
        ChegadaVeiculo = 9,
        Guarita = 10,
        CheckList = 20,
        TravamentoChave = 30,
        Expedicao = 40,
        LiberacaoChave = 50,
        Faturamento = 60,
        InicioViagem = 70,
        Posicao = 80,
        Entregas = 81,
        ChegadaLoja = 90,
        DeslocamentoPatio = 91,
        SaidaLoja = 100,
        FimViagem = 110,
        InicioHigienizacao = 120,
        FimHigienizacao = 130,
        InicioCarregamento = 140,
        FimCarregamento = 150,
        SolicitacaoVeiculo = 160,
        InicioDescarregamento = 170,
        FimDescarregamento = 180,
        DocumentoFiscal = 190,
        DocumentosTransporte = 200,
        MontagemCarga = 210,
        SeparacaoMercadoria = 220,
        AvaliacaoDescarga = 230
    }

    public static class EtapaFluxoGestaoPatioHelper
    {
        public static string ObterDescricao(this EtapaFluxoGestaoPatio etapa)
        {
            switch (etapa)
            {
                case EtapaFluxoGestaoPatio.CheckList: return Localization.Resources.Enumeradores.EtapaFluxoGestaoPatio.CheckList;
                case EtapaFluxoGestaoPatio.ChegadaLoja: return Localization.Resources.Enumeradores.EtapaFluxoGestaoPatio.ChegadaDestinatario;
                case EtapaFluxoGestaoPatio.Guarita: return Localization.Resources.Enumeradores.EtapaFluxoGestaoPatio.ChegadaPortaria;
                case EtapaFluxoGestaoPatio.ChegadaVeiculo: return Localization.Resources.Enumeradores.EtapaFluxoGestaoPatio.ChegadaVeiculo;
                case EtapaFluxoGestaoPatio.DeslocamentoPatio: return Localization.Resources.Enumeradores.EtapaFluxoGestaoPatio.DeslocamentoPatio;
                case EtapaFluxoGestaoPatio.InformarDoca: return Localization.Resources.Enumeradores.EtapaFluxoGestaoPatio.Doca;
                case EtapaFluxoGestaoPatio.DocumentosTransporte: return Localization.Resources.Enumeradores.EtapaFluxoGestaoPatio.DocumentosTransporte;
                case EtapaFluxoGestaoPatio.DocumentoFiscal: return Localization.Resources.Enumeradores.EtapaFluxoGestaoPatio.DocumentoFiscal;
                case EtapaFluxoGestaoPatio.Entregas: return Localization.Resources.Enumeradores.EtapaFluxoGestaoPatio.Entregas;
                case EtapaFluxoGestaoPatio.Expedicao: return Localization.Resources.Enumeradores.EtapaFluxoGestaoPatio.Expedicao;
                case EtapaFluxoGestaoPatio.Faturamento: return Localization.Resources.Enumeradores.EtapaFluxoGestaoPatio.Faturamento;
                case EtapaFluxoGestaoPatio.FimHigienizacao: return Localization.Resources.Enumeradores.EtapaFluxoGestaoPatio.FimHigienizacao;
                case EtapaFluxoGestaoPatio.FimViagem: return Localization.Resources.Enumeradores.EtapaFluxoGestaoPatio.FimViagem;
                case EtapaFluxoGestaoPatio.FimCarregamento: return Localization.Resources.Enumeradores.EtapaFluxoGestaoPatio.FimCarregamento;
                case EtapaFluxoGestaoPatio.FimDescarregamento: return Localization.Resources.Enumeradores.EtapaFluxoGestaoPatio.FimDescarregamento;
                case EtapaFluxoGestaoPatio.LiberacaoChave: return Localization.Resources.Enumeradores.EtapaFluxoGestaoPatio.LiberacaoChave;
                case EtapaFluxoGestaoPatio.InicioHigienizacao: return Localization.Resources.Enumeradores.EtapaFluxoGestaoPatio.InicioHigienizacao;
                case EtapaFluxoGestaoPatio.InicioViagem: return Localization.Resources.Enumeradores.EtapaFluxoGestaoPatio.InicioViagem;
                case EtapaFluxoGestaoPatio.InicioCarregamento: return Localization.Resources.Enumeradores.EtapaFluxoGestaoPatio.InicioCarregamento;
                case EtapaFluxoGestaoPatio.InicioDescarregamento: return Localization.Resources.Enumeradores.EtapaFluxoGestaoPatio.InicioDescarregamento;
                case EtapaFluxoGestaoPatio.MontagemCarga: return Localization.Resources.Enumeradores.EtapaFluxoGestaoPatio.MontagemCarga;
                case EtapaFluxoGestaoPatio.Posicao: return Localization.Resources.Enumeradores.EtapaFluxoGestaoPatio.Posicao;
                case EtapaFluxoGestaoPatio.SaidaLoja: return Localization.Resources.Enumeradores.EtapaFluxoGestaoPatio.SaidaDestinatario;
                case EtapaFluxoGestaoPatio.SeparacaoMercadoria: return Localization.Resources.Enumeradores.EtapaFluxoGestaoPatio.SeparacaoMercadoria;
                case EtapaFluxoGestaoPatio.AvaliacaoDescarga: return Localization.Resources.Enumeradores.EtapaFluxoGestaoPatio.AvaliacaoDescarga;
                case EtapaFluxoGestaoPatio.SolicitacaoVeiculo: return Localization.Resources.Enumeradores.EtapaFluxoGestaoPatio.SolicitacaoVeiculo;
                case EtapaFluxoGestaoPatio.TravamentoChave: return Localization.Resources.Enumeradores.EtapaFluxoGestaoPatio.TravamentoChave;
                case EtapaFluxoGestaoPatio.Todas: return Localization.Resources.Enumeradores.EtapaFluxoGestaoPatio.Todas;
                default: return string.Empty;
            }
        }
    }
}
