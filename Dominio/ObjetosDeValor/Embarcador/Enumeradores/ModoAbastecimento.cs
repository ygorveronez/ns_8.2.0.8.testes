namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum ModoAbastecimento
    {
        Interno = 1,
        //Externo = 2,
    }

    public static class ModoAbastecimentoHelper
    {
        public static string ObterDescricao(this ModoAbastecimento modoAbastecimento)
        {
            switch (modoAbastecimento)
            {
                case ModoAbastecimento.Interno: return "Interno";
                default: return string.Empty;
            }
        }
    }
}
