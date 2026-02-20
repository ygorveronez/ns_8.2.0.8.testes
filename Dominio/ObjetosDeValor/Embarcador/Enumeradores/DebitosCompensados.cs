namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum DebitosCompensados
    {
        SemDefinicao = 0,
        DescontoAvarias = 1,
        DescontoIrregulariades = 2,
        CTeNFe = 3
    }
    public static class DebitosCompensadosHelper
    {
        public static string ObterDescricao(this DebitosCompensados debito)
        {
            switch (debito)
            {
                case DebitosCompensados.DescontoAvarias:
                    return "Desconto de Avarias";
                case DebitosCompensados.DescontoIrregulariades:
                    return "Desconto de Irregularidades";
                case DebitosCompensados.CTeNFe:
                    return "CT-e/NFS";
                default:
                    return "";
            }
        }

        public static DebitosCompensados ObterTipoDebitoCompensado(string chave, string tipo)
        {
            if ((chave == "21" && tipo == "KM") || (chave == "21" && tipo == "KA"))
                return DebitosCompensados.DescontoAvarias;


            if ((chave == "21" && tipo == "RE"))
                return DebitosCompensados.DescontoIrregulariades;

            if ((chave == "31" && tipo == "RE") || (chave == "31" && tipo == "KA"))
                return DebitosCompensados.DescontoAvarias;

            return DebitosCompensados.SemDefinicao;
        }
    }
}
