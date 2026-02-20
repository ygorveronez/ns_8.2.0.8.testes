namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum StatusContainerEMP
    {
        Todos = 0,
        Pendente = 1,
        Finalizado = 2
    }

    public static class StatusContainerEMPHelper
    {
        public static string ObterDescricao(this StatusContainerEMP status)
        {
            switch (status)
            {
                case StatusContainerEMP.Pendente: return "Pendente";
                case StatusContainerEMP.Finalizado: return "Finalizado";
                default: return string.Empty;
            }
        }
    }
}
