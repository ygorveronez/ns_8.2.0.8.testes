namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum ModalPropostaMultimodal
    {
        Nenhum = 0,
        PortoPorta = 1,
        PortaPorto = 2,
        PortaPorta = 3,
        PortoPorto = 4        
    }

    public static class ModalPropostaMultimodalHelper
    {
        public static string ObterDescricao(this ModalPropostaMultimodal tipo)
        {
            switch (tipo)
            {
                case ModalPropostaMultimodal.Nenhum: return "Nenhum";
                case ModalPropostaMultimodal.PortoPorta: return "1 - Porto a Porta";
                case ModalPropostaMultimodal.PortaPorto: return "2 - Porta a Porto";
                case ModalPropostaMultimodal.PortaPorta: return "3 - Porta a Porta";
                case ModalPropostaMultimodal.PortoPorto: return "4 - Porto a Porto";
                default: return string.Empty;
            }
        }

        public static string ObterAbreviacao(this ModalPropostaMultimodal tipo)
        {
            switch (tipo)
            {
                case ModalPropostaMultimodal.Nenhum: return "Nenhum";
                case ModalPropostaMultimodal.PortoPorta: return "PO/PA";
                case ModalPropostaMultimodal.PortaPorto: return "PA/PO";
                case ModalPropostaMultimodal.PortaPorta: return "PA/PA";
                case ModalPropostaMultimodal.PortoPorto: return "PO/PO";
                default: return string.Empty;
            }
        }
        public static string ObterDescricaoCompleta(this ModalPropostaMultimodal tipo)
        {
            switch (tipo)
            {
                case ModalPropostaMultimodal.Nenhum: return "";
                case ModalPropostaMultimodal.PortoPorta: return "PORTO A PORTA";
                case ModalPropostaMultimodal.PortaPorto: return "PORTA A PORTO";
                case ModalPropostaMultimodal.PortaPorta: return "PORTA A PORTA";
                case ModalPropostaMultimodal.PortoPorto: return "PORTO A PORTO";
                default: return string.Empty;
            }
        }
    }
}
