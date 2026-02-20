namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoMotivoAtendimento
    {
        Atendimento = 0,
        Devolucao = 1,
        Reentrega = 2,
        Retencao = 3,
        RetencaoOrigem = 4,
        ReentregarMesmaCarga = 8 //no javascript colocaram outros tipos;
    }

    public static class TipoMotivoAtendimentoHelper
    {
        public static string ObterDescricao(this TipoMotivoAtendimento tipoMotivoAtendimento)
        {
            switch (tipoMotivoAtendimento)
            {
                case TipoMotivoAtendimento.Atendimento: return Localization.Resources.Enumeradores.TipoMotivoAtendimento.Atendimento;
                case TipoMotivoAtendimento.Devolucao: return Localization.Resources.Enumeradores.TipoMotivoAtendimento.Devolucao;
                case TipoMotivoAtendimento.Reentrega: return Localization.Resources.Enumeradores.TipoMotivoAtendimento.Reentrega;
                case TipoMotivoAtendimento.Retencao: return Localization.Resources.Enumeradores.TipoMotivoAtendimento.Retencao;
                case TipoMotivoAtendimento.RetencaoOrigem: return Localization.Resources.Enumeradores.TipoMotivoAtendimento.RetencaoOrigem;
                case TipoMotivoAtendimento.ReentregarMesmaCarga: return Localization.Resources.Enumeradores.TipoMotivoAtendimento.ReentregarMesmaCarga;
                default: return string.Empty;
            }
        }
    }
}
