namespace Dominio.ObjetosDeValor.Enumerador
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
                case AcaoBancoDados.Insert: return Localization.Resources.Enumeradores.AcaoBancoDados.Adicionado;
                case AcaoBancoDados.Update: return Localization.Resources.Enumeradores.AcaoBancoDados.Atualizado;
                case AcaoBancoDados.Delete: return Localization.Resources.Enumeradores.AcaoBancoDados.Excluido;
                default: return Localization.Resources.Enumeradores.AcaoBancoDados.RegistroDeAcao;
            }
        }
    }
}
