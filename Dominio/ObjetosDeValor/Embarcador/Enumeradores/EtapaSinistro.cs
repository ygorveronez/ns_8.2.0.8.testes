namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum EtapaSinistro
    {
        Dados = 1,
        Documentacao = 2,
        Manutencao = 3,
        IndicacaoPagador = 4,
        Acompanhamento = 5
    }

    public static class EtapaSinistroHelper
    {
        public static string ObterDescricao(this EtapaSinistro etapa)
        {
            switch (etapa)
            {
                case EtapaSinistro.Dados: return "Dados";
                case EtapaSinistro.Documentacao: return "Documentação";
                case EtapaSinistro.Manutencao: return "Manutenção";
                case EtapaSinistro.IndicacaoPagador: return "Indicação Pagador";
                case EtapaSinistro.Acompanhamento: return "Acompanhamento";
                default: return string.Empty;
            }
        }
    }
}
