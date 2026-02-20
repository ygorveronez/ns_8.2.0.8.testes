namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoInicioCarregamento
    {
        AguardandoInicioCarregamento = 1,
        CarregamentoIniciado = 2
    }

    public static class SituacaoInicioCarregamentoHelper
    {
        public static string ObterDescricao(this SituacaoInicioCarregamento situacao)
        {
            switch (situacao)
            {
                case SituacaoInicioCarregamento.AguardandoInicioCarregamento: return "Aguardando Início do Carregamento";
                case SituacaoInicioCarregamento.CarregamentoIniciado: return "Carregamento Iniciado";
                default: return string.Empty;
            }
        }
    }
}