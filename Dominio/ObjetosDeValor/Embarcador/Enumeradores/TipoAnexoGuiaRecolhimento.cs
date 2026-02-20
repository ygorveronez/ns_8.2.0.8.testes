namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoAnexoGuiaRecolhimento
    {
        Guia = 1,
        Comprovante = 2
    }

    public static class TipoAnexoGuiaRecolhimentoHelper
    {
        public static string ObterDescricao(this TipoAnexoGuiaRecolhimento tipo)
        {
            switch (tipo)
            {
                case TipoAnexoGuiaRecolhimento.Guia: return "Guia";
                case TipoAnexoGuiaRecolhimento.Comprovante: return "Comprovante";
                default: return string.Empty;
            }
        }
    }
}
