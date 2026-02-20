namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoCargaJanelaCarregamento
    {
        Carregamento = 0,
        Descarregamento = 1
    }

    public static class TipoCargaJanelaCarregamentoHelper
    {
        public static string ObterDescricao(this TipoCargaJanelaCarregamento tipo)
        {
            switch (tipo)
            {
                case TipoCargaJanelaCarregamento.Carregamento: return "Carregamento";
                case TipoCargaJanelaCarregamento.Descarregamento: return "Descarregamento";
                default: return string.Empty;
            }
        }
    }
}
