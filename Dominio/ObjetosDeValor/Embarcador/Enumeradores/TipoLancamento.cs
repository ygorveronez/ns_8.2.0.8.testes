namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoLancamento
    {
        Automatico = 0,
        Manual = 1
    }

    public static class TipoLancamentoHelper
    {
        public static string ObterDescricao(this TipoLancamento tipoLancamento)
        {
            return (tipoLancamento == TipoLancamento.Automatico) ? "Automático" : "Manual";
        }
    }
}
