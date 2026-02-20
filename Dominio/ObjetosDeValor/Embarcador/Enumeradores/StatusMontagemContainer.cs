namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum StatusMontagemContainer
    {
        Todos = 0,
        Aberto = 1,
        Finalizado = 2,
        Expedido = 3
    }

    public static class StatusMontagemContainerHelper
    {
        public static string ObterDescricao(this StatusMontagemContainer status)
        {
            switch (status)
            {
                case StatusMontagemContainer.Aberto: return "Aberto";
                case StatusMontagemContainer.Finalizado: return "Finalizado";
                case StatusMontagemContainer.Expedido: return "Expedido";
                default: return string.Empty;
            }
        }
    }
}
