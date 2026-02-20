namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum IndicativoColetaEntrega
    {
        NaoInformado = 0,
        Coleta = 1,
        Entrega = 2
    }

    public static class IndicativoColetaEntregaHelper
    {
        public static string ObterDescricao(this IndicativoColetaEntrega indicativoColetaEntrega)
        {
            switch (indicativoColetaEntrega)
            {
                case IndicativoColetaEntrega.NaoInformado:
                    return "Não informado";

                case IndicativoColetaEntrega.Coleta:
                    return "Coleta";

                case IndicativoColetaEntrega.Entrega:
                    return "Entrega";

                default:
                    return string.Empty;
            }
        }
    }
}
