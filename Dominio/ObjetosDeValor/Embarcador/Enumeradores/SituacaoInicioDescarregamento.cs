namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoInicioDescarregamento
    {
        AguardandoInicioDescarregamento = 1,
        DescarregamentoIniciado = 2
    }

    public static class SituacaoInicioDescarregamentoHelper
    {
        public static string ObterDescricao(this SituacaoInicioDescarregamento situacao)
        {
            switch (situacao)
            {
                case SituacaoInicioDescarregamento.AguardandoInicioDescarregamento: return "Aguardando Início do Descarregamento";
                case SituacaoInicioDescarregamento.DescarregamentoIniciado: return "Descarregamento Iniciado";
                default: return string.Empty;
            }
        }
    }
}
