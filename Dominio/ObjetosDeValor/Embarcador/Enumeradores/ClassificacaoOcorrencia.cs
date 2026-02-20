namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum ClassificacaoOcorrencia
    {
        DeslocamentoTraking = 1,
        FreteNegociado = 2
    }

    public static class ClassificacaoOcorrenciaHelper
    {
        public static string ObterDescricao(this ClassificacaoOcorrencia tipo)
        {
            switch (tipo)
            {
                case ClassificacaoOcorrencia.DeslocamentoTraking: return "Deslocamento Traking";
                case ClassificacaoOcorrencia.FreteNegociado: return "Frete Negociado";
                default: return string.Empty;
            }
        }
    }
}
