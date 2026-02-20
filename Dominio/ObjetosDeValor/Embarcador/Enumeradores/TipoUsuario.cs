namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoUsuario
    {
        Todos = 0,
        Funcionarios = 1,
        Pessoas = 2
    }

    public static class TipoUsuarioHelper
    {
        public static string ObterDescricao(this TipoUsuario situacao)
        {
            switch (situacao)
            {
                case TipoUsuario.Funcionarios: return "Funcionários";
                case TipoUsuario.Pessoas: return "Pessoas";
                default: return string.Empty;
            }
        }
    }
}
