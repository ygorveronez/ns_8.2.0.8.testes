namespace AdminMultisoftware.Dominio.Enumeradores
{
    public enum AcaoBancoDados
    {
        Insert = 0,
        Update = 1,
        Delete = 2,
        Registro = 3
    }

    public static class AcaoBancoDadosHelper
    {
        public static string ObterDescricao(this AcaoBancoDados acao)
        {
            switch (acao)
            {
                case AcaoBancoDados.Insert: return "Adicionado";
                case AcaoBancoDados.Update: return "Atualizado";
                case AcaoBancoDados.Delete: return "Excluído";
                default: return "Registro de Ação";
            }
        }
    }
}
