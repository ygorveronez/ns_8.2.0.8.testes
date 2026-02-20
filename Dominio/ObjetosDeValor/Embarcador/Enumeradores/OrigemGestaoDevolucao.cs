namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum OrigemGestaoDevolucao
    {
        AdicionarPendenciaFinanceira = 1,
        AdicionarNotaDevolucao = 2,
        Atendimento = 3,
        PortalTransportador = 4,
        PortalCliente = 5,
        PortalEmbarcador = 6,
    }

    public static class OrigemGestaoDevolucaoHelper
    {
        public static string ObterDescricao(this OrigemGestaoDevolucao OrigemGestaoDevolucao)
        {
            switch (OrigemGestaoDevolucao)
            {
                case OrigemGestaoDevolucao.AdicionarPendenciaFinanceira: return "Adicionar Pendência Finaiceira";
                case OrigemGestaoDevolucao.AdicionarNotaDevolucao: return "Adicionar Nota Devolução";
                case OrigemGestaoDevolucao.Atendimento: return "Atendimento";
                case OrigemGestaoDevolucao.PortalTransportador: return "Portal do Transportador";
                case OrigemGestaoDevolucao.PortalCliente: return "Portal do Cliente";
                case OrigemGestaoDevolucao.PortalEmbarcador: return "Portal do Embarcador";
                default: return string.Empty;
            }
        }
    }
}