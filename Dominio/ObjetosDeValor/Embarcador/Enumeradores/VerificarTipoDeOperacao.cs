namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum VerificarTipoDeOperacao
    {
        Todos = 0,
        NaoVerificar = 1,
        Algum = 2,
        Nenhum = 3
    }

    public static class VerificarTipoDeOperacaoHelper
    {
        public static string ObterDescricao(this VerificarTipoDeOperacao verificarTipoDeOperacao)
        {
            switch (verificarTipoDeOperacao)
            {
                case VerificarTipoDeOperacao.Todos: return "Todos";
                case VerificarTipoDeOperacao.NaoVerificar: return "Não verificar";
                case VerificarTipoDeOperacao.Algum: return "Algum dos tipos";
                case VerificarTipoDeOperacao.Nenhum: return "Nenhum dos tipos";
                default: return "";
            }
        }
    }
}
