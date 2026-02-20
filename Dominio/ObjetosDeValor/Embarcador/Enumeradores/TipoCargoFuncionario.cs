namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoCargoFuncionario
    {
        Todos = 0,
        Outros = 1,
        Mecanico = 2
    }

    public static class TipoCargoFuncionarioHelper
    {
        public static string ObterDescricao(this TipoCargoFuncionario tipo)
        {
            switch (tipo)
            {
                case TipoCargoFuncionario.Outros: return "Outros";
                case TipoCargoFuncionario.Mecanico: return "Mecânico";
                default: return string.Empty;
            }
        }

        public static string ObterDescricaoPesquisa(this TipoCargoFuncionario tipo)
        {
            switch (tipo)
            {
                case TipoCargoFuncionario.Todos: return "Todos";
                case TipoCargoFuncionario.Outros: return "Outros";
                case TipoCargoFuncionario.Mecanico: return "Mecânico";
                default: return string.Empty;
            }
        }
    }
}
