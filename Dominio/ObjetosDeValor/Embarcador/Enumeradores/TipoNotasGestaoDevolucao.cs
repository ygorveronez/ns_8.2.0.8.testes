namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoNotasGestaoDevolucao
    {
        Mercadoria = 1,
        Pallet = 2,
        Mista = 3,
    }

    public static class TipoNotasGestaoDevolucaoHelper
    {
        public static string ObterDescricao(this TipoNotasGestaoDevolucao tipoNota)
        {
            switch (tipoNota)
            {
                case TipoNotasGestaoDevolucao.Mercadoria: return "Mercadoria";
                case TipoNotasGestaoDevolucao.Pallet: return "Pallet";
                case TipoNotasGestaoDevolucao.Mista: return "Mista";
                default: return string.Empty;
            }
        }
    }
}
