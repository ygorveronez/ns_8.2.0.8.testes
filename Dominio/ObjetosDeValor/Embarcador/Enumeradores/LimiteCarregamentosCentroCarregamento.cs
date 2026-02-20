namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum LimiteCarregamentosCentroCarregamento
    {
        TempoCarregamento = 1,
        QuantidadeDocas = 2,
    }

    public static class LimiteCarregamentosCentroCarregamentoHelper
    {
        public static string ObterDescricao(this LimiteCarregamentosCentroCarregamento forma)
        {
            switch (forma)
            {
                case LimiteCarregamentosCentroCarregamento.TempoCarregamento: return "Tempo de Carregamento";
                case LimiteCarregamentosCentroCarregamento.QuantidadeDocas: return "Quantidade de Docas";
                default: return string.Empty;
            }
        }
    }
}
