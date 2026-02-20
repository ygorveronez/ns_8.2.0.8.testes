namespace Dominio.MSMQ
{
    public enum MSMQQueue
    {
        SGTWebAdmin = 1,
        MultiCTe = 2,
        SGTMobile = 3,
        Terceiros = 4,
        Fornecedor = 8,
        TransportadorTerceiro = 12,
        HUBOfertas = 13,
    }

    public static class MSMQQueueHelper
    {
        public static string GetDescription(this MSMQQueue project)
        {
            switch (project)
            {
                case MSMQQueue.SGTWebAdmin:
                    return "SGT.WebAdmin";
                case MSMQQueue.MultiCTe:
                    return "MultiCTe";
                case MSMQQueue.SGTMobile:
                    return "SGT.Mobile";
                case MSMQQueue.Terceiros:
                    return "Terceiros";
                case MSMQQueue.Fornecedor:
                    return "Fornecedor";
                case MSMQQueue.HUBOfertas:
                    return "HUB.Ofertas";
                default:
                    return "SGT.WebAdmin";
            }
        }
    }
}
