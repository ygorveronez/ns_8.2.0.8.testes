namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoModeloVeicularCarga
    {
        SemModeloVeicular = -2,
        Todos = -1,
        Geral = 1,
        Reboque = 2,
        Tracao = 3
    }

    public static class TipoModeloVeicularCargaHelper
    {
        public static string ObterDescricao(this TipoModeloVeicularCarga tipo)
        {
            switch (tipo)
            {
                case TipoModeloVeicularCarga.SemModeloVeicular: return "Sem modelo veicular";
                case TipoModeloVeicularCarga.Todos: return "Todos";
                case TipoModeloVeicularCarga.Geral: return "Geral";
                case TipoModeloVeicularCarga.Reboque: return "Reboque";
                case TipoModeloVeicularCarga.Tracao: return "Tração";
                default: return string.Empty;
            }
        }
    }
}
