namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum LocalVinculo
    {
        Todos = 0, 
        Pedido = 1,
        Carga = 2,
        Planejamento = 3,
        FilaCarregamento = 4,
    }

    public static class LocalVinculoHelper
    {
        public static string ObterDescricao(this LocalVinculo localVinculo)
        {
            switch (localVinculo)
            {
                case LocalVinculo.Pedido : return "Pedido";
                case LocalVinculo.Carga : return "Carga";
                case LocalVinculo.Planejamento : return "Planejamento";
                case LocalVinculo.FilaCarregamento : return "Fila de Carregamento";
                default: return string.Empty;
            }
        }
    }
}
