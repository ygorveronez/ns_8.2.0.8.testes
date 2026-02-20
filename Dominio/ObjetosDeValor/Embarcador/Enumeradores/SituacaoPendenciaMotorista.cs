namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoPendenciaMotorista
    {
        Todos = 0,
        Ativo = 1,
        Estornado = 2,
    }
    public static class SituacaoPendenciaMotoristaHelper
    {
        public static string ObterDescricao(this SituacaoPendenciaMotorista situacao)
        {
            switch (situacao)
            {
                case SituacaoPendenciaMotorista.Todos: return "Todos";
                case SituacaoPendenciaMotorista.Ativo: return "Ativo";
                case SituacaoPendenciaMotorista.Estornado: return "Estornado";

                default: return string.Empty;
            }
        }
    }
}
