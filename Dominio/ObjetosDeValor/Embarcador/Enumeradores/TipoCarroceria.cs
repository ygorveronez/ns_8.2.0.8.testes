namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoCarroceria
    {
        NaoAplicavel = 0,
        Aberta = 1,
        FechadaBau = 2,
        Graneleira = 3,
        PortaContainer = 4,
        Utilitario = 5,
        Sider = 6,
        Todos = 99
    }

    public static class TipoCarroceriaHelper
    {
        public static string ObterDescricao(this TipoCarroceria situacao)
        {
            switch (situacao)
            {
                case TipoCarroceria.NaoAplicavel: return "Não Aplicável";
                case TipoCarroceria.Aberta: return "Aberta";
                case TipoCarroceria.FechadaBau: return "Fechada/Baú";
                case TipoCarroceria.Graneleira: return "Graneleira";
                case TipoCarroceria.PortaContainer: return "Porta Container";
                case TipoCarroceria.Utilitario: return "Utilitário";
                case TipoCarroceria.Sider: return "Sider";
                default: return null;
            }
        }
    }
}
