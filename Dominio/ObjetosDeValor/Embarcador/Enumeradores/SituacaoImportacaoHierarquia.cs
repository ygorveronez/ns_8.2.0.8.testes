namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoImportacaoHierarquia
    {
        Sucesso = 1,
        Falha = 2
    }

    public static class SituacaoImportacaoHierarquiaHelper
    {
        public static string ObterDescricao(this SituacaoImportacaoHierarquia situacao)
        {
            switch (situacao)
            {
                case SituacaoImportacaoHierarquia.Sucesso: return "Sucesso";
                case SituacaoImportacaoHierarquia.Falha: return "Falha";
                default: return string.Empty;
            }
        }
    }
}
