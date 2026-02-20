namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum Quinzena
    {
        Primeira = 0,
        Segunda = 1
    }

    public static class QuinzenaHelper
    {
        public static string ObterDescricao(this Quinzena quinzena)
        {
            switch (quinzena)
            {
                case Quinzena.Primeira: return "Primeira";
                case Quinzena.Segunda: return "Segunda";
                default: return string.Empty;
            }
        }
    }
}
