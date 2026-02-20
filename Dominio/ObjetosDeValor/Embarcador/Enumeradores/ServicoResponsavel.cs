namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum ServicoResponsavel
    {
        Embarcador = 1,
        Transporador = 2
    }

    public static class ServicoResponsavelHelper
    {
        public static string ObterDescricao(this ServicoResponsavel servicoResponsavel)
        {
            switch (servicoResponsavel)
            {
                case ServicoResponsavel.Embarcador: return "Embarcador";
                case ServicoResponsavel.Transporador: return "Transportador";
                default: return string.Empty;
            }
        }
    }
}
