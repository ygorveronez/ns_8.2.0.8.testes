namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoParametroCriticidade
    {
        Gerencial = 1,
        CausaProblema = 2
    }

    public static class TipoParametroCriticidadeHelper
    {
        public static string ObterDescricao(this TipoParametroCriticidade tipo)
        {
            switch (tipo)
            {
                case TipoParametroCriticidade.Gerencial: return "Gerencial";
                case TipoParametroCriticidade.CausaProblema: return "Causa Problema";
                default: return "Não Definido";
            }
        }
    }
}
