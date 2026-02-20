namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoRetornoDadosFrete
    {
        FreteValido = 1,
        ProblemaCalcularFrete = 2,
        RotaNaoEncontrada = 3,
        CalculandoFrete = 4
    }

    public static class SituacaoRetornoDadosFreteHelper
    {
        public static string ObterDescricao(this SituacaoRetornoDadosFrete situacao)
        {
            switch (situacao)
            {
                case SituacaoRetornoDadosFrete.FreteValido: return "Frete";
                case SituacaoRetornoDadosFrete.ProblemaCalcularFrete: return "Problema ao Calcular Frete (Recalcular) ";
                case SituacaoRetornoDadosFrete.RotaNaoEncontrada: return "Rota Não Encontrada";
                case SituacaoRetornoDadosFrete.CalculandoFrete: return "Calculando Frete";
                default: return string.Empty;
            }
        }
    }
}

