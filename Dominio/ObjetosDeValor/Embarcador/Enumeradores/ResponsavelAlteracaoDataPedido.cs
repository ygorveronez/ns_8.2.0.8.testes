namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum ResponsavelAlteracaoDataPedido
    {
       Embarcador = 1,
       Transportador = 2
    }

    public static class ResponsavelAlteracaoDataPedidoHelper
    {
        public static string ObterDescricao(this ResponsavelAlteracaoDataPedido t)
        {
            switch (t)
            {
                case ResponsavelAlteracaoDataPedido.Embarcador: return "Embarcador";
                case ResponsavelAlteracaoDataPedido.Transportador: return "Transportador";
                default: return string.Empty;
            }
        }
    }
}
