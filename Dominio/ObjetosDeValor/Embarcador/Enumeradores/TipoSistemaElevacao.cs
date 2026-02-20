namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoSistemaElevacao
    {
        Elevador = 1,
        Escada = 2
    }

    public static class TipoSistemaElevacaoHelper
    {
        public static string ObterDescricao(this TipoSistemaElevacao tipoSistemaElevacao)
        {
            switch (tipoSistemaElevacao)
            {
                case TipoSistemaElevacao.Elevador: return "Elevador";
                case TipoSistemaElevacao.Escada: return "Escada";
                default: return string.Empty;
            }
        }
    }
}
