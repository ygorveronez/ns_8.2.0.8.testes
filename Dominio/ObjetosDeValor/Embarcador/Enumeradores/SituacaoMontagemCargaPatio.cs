namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoMontagemCargaPatio
    {
        AguardandoMontagemCarga = 1,
        MontagemCargaFinalizada = 2
    }

    public static class SituacaoMontagemCargaPatioHelper
    {
        public static string ObterDescricao(this SituacaoMontagemCargaPatio situacao)
        {
            switch (situacao)
            {
                case SituacaoMontagemCargaPatio.AguardandoMontagemCarga: return "Aguardando Montagem de Carga";
                case SituacaoMontagemCargaPatio.MontagemCargaFinalizada: return "Montagem de carga Finalizada";
                default: return string.Empty;
            }
        }
    }
}
