namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoAE
    {
        Sucesso = 1,
        Falha = 2,
    }

    public static class SituacaoAEHelper
    {
        public static string ObterDescricao(this SituacaoAE situacao)
        {
            switch (situacao)
            {
                case SituacaoAE.Sucesso: return "AE gerada com sucesso";
                case SituacaoAE.Falha: return "Falha na geração da AE";
                default: return string.Empty;
            }
        }
    }
}
