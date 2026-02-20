namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum InformacaoOrigemMovimentacaoContainer
    {
        VincularRICCarga = 1,
        ProcessarMonitoramentoEmCarregamento = 2,
        AtualizacaoControleContainer = 3,
        MovimentacaoControleContainer = 4,
        VincularContainerCarga = 5,
        AutomaticamenteAvancoEtapaDocumentos = 6,
        AutomaticamenteConfirmarDocumentos = 7,
        IniciarViagemRedespacho = 8,
        ConfirmarEntregaContainer = 9,
        ConfirmarColetaContainer = 10,
        AlterarStatusMonitoramento = 11,
        InformarEmbarqueContainer = 12,
        TrocarPreCargaCarga = 13,
        CancelarControleContainer = 14,
        AutomaticamenteContainerInformadoPedidoImportacao = 15,
        AoRemoverContainerCarga = 16,
        AoIniciarViagem = 17,
    }

    public static class InformacaoOrigemMovimentacaoContainerHelper
    {
        public static string ObterDescricao(this InformacaoOrigemMovimentacaoContainer info)
        {
            switch (info)
            {
                case InformacaoOrigemMovimentacaoContainer.VincularRICCarga: return "Ao vincular RIC na carga";
                case InformacaoOrigemMovimentacaoContainer.ProcessarMonitoramentoEmCarregamento: return "Ao processar monitoramento da carga em area de Carregamento";
                case InformacaoOrigemMovimentacaoContainer.AtualizacaoControleContainer: return "Atualização do Controle Container Manualmente";
                case InformacaoOrigemMovimentacaoContainer.MovimentacaoControleContainer: return "Movimentação do Container Manualmente";
                case InformacaoOrigemMovimentacaoContainer.VincularContainerCarga: return "Ao vincular container a carga";
                case InformacaoOrigemMovimentacaoContainer.AutomaticamenteAvancoEtapaDocumentos: return "Automaticamente Ao avançar etapa documentos da carga";
                case InformacaoOrigemMovimentacaoContainer.AutomaticamenteConfirmarDocumentos: return "Automaticamente Ao confirmar envio dos Documentos da carga";
                case InformacaoOrigemMovimentacaoContainer.IniciarViagemRedespacho: return "Ao iniciar viagem em Carga de Redespacho";
                case InformacaoOrigemMovimentacaoContainer.ConfirmarEntregaContainer: return "Ao confirmar entrega container";
                case InformacaoOrigemMovimentacaoContainer.ConfirmarColetaContainer: return "Ao confirmar coleta container";
                case InformacaoOrigemMovimentacaoContainer.AlterarStatusMonitoramento: return "Ao alterar status monitoramento manualmente";
                case InformacaoOrigemMovimentacaoContainer.InformarEmbarqueContainer: return "Ao informar embarque do container";
                case InformacaoOrigemMovimentacaoContainer.TrocarPreCargaCarga: return "Ao trocar pré-Carga para Carga";
                case InformacaoOrigemMovimentacaoContainer.CancelarControleContainer: return "Ao cancelar controle de container";
                case InformacaoOrigemMovimentacaoContainer.AutomaticamenteContainerInformadoPedidoImportacao: return "Automaticamente pedido com container informado na importação";
                case InformacaoOrigemMovimentacaoContainer.AoRemoverContainerCarga: return "Ao remover container da carga manualmente";
                case InformacaoOrigemMovimentacaoContainer.AoIniciarViagem: return "Automaticamente ao Iniciar viagem";

                default: return string.Empty;
            }
        }
    }
}