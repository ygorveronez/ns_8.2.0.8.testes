namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoPlanejamentoPedidoTMS
    {
        Pendente = 0,
        CheckListOK = 1,
        CargaGerouDocumentacao = 2,
        CargaPossuiAcertoAberto = 3,
        PassouPelaGuarita = 4,
        CargaCanceladaAnulada = 5,
        AvisoAoMotorista = 6,
        MotoristaCiente = 7,
        Devolucao = 8
    }

    public static class SituacaoPlanejamentoPedidoTMSHelper
    {
        public static string ObterCorLinha(this SituacaoPlanejamentoPedidoTMS situacao)
        {
            switch (situacao)
            {
                case SituacaoPlanejamentoPedidoTMS.Pendente: return CorGrid.Branco;
                case SituacaoPlanejamentoPedidoTMS.CheckListOK: return CorGrid.Green;
                case SituacaoPlanejamentoPedidoTMS.CargaGerouDocumentacao: return CorGrid.Blue;
                case SituacaoPlanejamentoPedidoTMS.CargaPossuiAcertoAberto: return CorGrid.Orange;
                case SituacaoPlanejamentoPedidoTMS.PassouPelaGuarita: return CorGrid.Cyan;
                case SituacaoPlanejamentoPedidoTMS.CargaCanceladaAnulada: return CorGrid.Red;
                case SituacaoPlanejamentoPedidoTMS.AvisoAoMotorista: return CorGrid.Purple;
                case SituacaoPlanejamentoPedidoTMS.MotoristaCiente: return CorGrid.Magenta;
                case SituacaoPlanejamentoPedidoTMS.Devolucao: return CorGrid.Yellow;
                default: return string.Empty;
            }
        }

        public static string ObterDescricao(this SituacaoPlanejamentoPedidoTMS situacao)
        {
            switch (situacao)
            {
                case SituacaoPlanejamentoPedidoTMS.Pendente: return "Pendente";
                case SituacaoPlanejamentoPedidoTMS.CheckListOK: return "Check List OK";
                case SituacaoPlanejamentoPedidoTMS.CargaGerouDocumentacao: return "Carga Gerou Documentação";
                case SituacaoPlanejamentoPedidoTMS.CargaPossuiAcertoAberto: return "Carga Possui Acerto Aberto";
                case SituacaoPlanejamentoPedidoTMS.PassouPelaGuarita: return "Passou pela Guarita";
                case SituacaoPlanejamentoPedidoTMS.CargaCanceladaAnulada: return "Carga Cancelada/Anulada";
                case SituacaoPlanejamentoPedidoTMS.AvisoAoMotorista: return "Aviso ao Motorista";
                case SituacaoPlanejamentoPedidoTMS.MotoristaCiente: return "Motorista Ciente";
                case SituacaoPlanejamentoPedidoTMS.Devolucao: return "Devolução";
                default: return string.Empty;
            }
        }
    }
}
