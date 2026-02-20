namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum AtividadeFilial
    {
        Outra = 0,
        ServicosDaMesma = 1,
        Industrial = 2,
        Comercial = 3,
        PrestadoraDeServico = 4,
        DistribuidoraDeEnergia = 5,
        ProdutorRural = 6,
        NaoContribuinte = 7,
    }

    public static class AtividadeFilialHelper
    {
        public static string ObterDescricao(this AtividadeFilial AtividadeFilial)
        {
            switch (AtividadeFilial)
            {
                case AtividadeFilial.ServicosDaMesma: return Localization.Resources.Enumeradores.Atividade.ServicosMesma;
                case AtividadeFilial.Industrial: return Localization.Resources.Enumeradores.Atividade.Industrial;
                case AtividadeFilial.Comercial: return Localization.Resources.Enumeradores.Atividade.Comercial;
                case AtividadeFilial.PrestadoraDeServico: return Localization.Resources.Enumeradores.Atividade.PrestadoraServico;
                case AtividadeFilial.DistribuidoraDeEnergia: return Localization.Resources.Enumeradores.Atividade.DistribuidoraEnergia;
                case AtividadeFilial.ProdutorRural: return Localization.Resources.Enumeradores.Atividade.ProdutorRural;
                case AtividadeFilial.NaoContribuinte: return Localization.Resources.Enumeradores.Atividade.NaoContribuinte;
                default: return "Outra";
            }
        }
    }
}
