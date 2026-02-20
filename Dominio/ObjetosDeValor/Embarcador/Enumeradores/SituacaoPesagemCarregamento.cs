namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoPesagemCarregamento
    {
        NaoInformada = 1,
        Aprovada = 2,
        Excedida = 3,
        Reprovada = 4
    }

    public static class SituacaoPesagemCarregamentoHelper
    {
        public static string ObterCorLinha(this SituacaoPesagemCarregamento situacao)
        {
            switch (situacao)
            {
                case SituacaoPesagemCarregamento.Aprovada: return "#9dde88ad";
                case SituacaoPesagemCarregamento.Excedida: return "#FFFF00";
                case SituacaoPesagemCarregamento.Reprovada: return "#FF6347";
                default: return string.Empty;
            }
        }
    }
}
