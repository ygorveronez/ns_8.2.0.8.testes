namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum StatusOrdemServicoPet
    {
        Aberto = 1,
        EmAndamento = 2,
        Finalizado = 3,
    }

    public static class StatusOrdemServicoPetHelper
    {
        public static string ObterDescricao(this StatusOrdemServicoPet status)
        {
            switch (status)
            {
                case StatusOrdemServicoPet.Aberto: return "Aberto";
                case StatusOrdemServicoPet.EmAndamento: return "Em andamento";
                case StatusOrdemServicoPet.Finalizado: return "Finalizado";
                default: return string.Empty;
            }
        }
    }
}
