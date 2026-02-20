namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoCanhoto
    {
        Todas = 0,
        Pendente = 1,
        Justificado = 2,
        RecebidoFisicamente = 3,
        Extraviado = 4,
        EntregueMotorista = 5,
        EnviadoCliente = 6,
        RecebidoCliente = 7,
        Cancelado = 8
    }

    public static class SituacaoCanhotoHelper
    {
        public static string ObterCorFonte(this SituacaoCanhoto situacao)
        {
            switch (situacao)
            {
                case SituacaoCanhoto.Extraviado: return "#FFF";
                default: return "";
            }
        }

        public static string ObterCorLinha(this SituacaoCanhoto situacao)
        {
            switch (situacao)
            {
                case SituacaoCanhoto.Justificado: return "#fcf8e3";
                case SituacaoCanhoto.RecebidoFisicamente: return "#dff0d8";
                case SituacaoCanhoto.Extraviado: return "#031634";
                default: return "";
            }
        }

        public static string ObterDescricao(this SituacaoCanhoto situacaoCanhoto)
        {
            switch (situacaoCanhoto)
            {
                case SituacaoCanhoto.Todas: return "";
                case SituacaoCanhoto.Pendente: return "Pendente";
                case SituacaoCanhoto.Justificado: return "Justificado";
                case SituacaoCanhoto.RecebidoFisicamente: return "Recebido Físicamente";
                case SituacaoCanhoto.Extraviado: return "Extraviado";
                case SituacaoCanhoto.EntregueMotorista: return "Entregue pelo Motorista";
                case SituacaoCanhoto.RecebidoCliente: return "Recebido pelo Cliente";
                case SituacaoCanhoto.EnviadoCliente: return "Enviado ao Cliente";
                case SituacaoCanhoto.Cancelado: return "Cancelado";
                default: return "Nenhuma";
            }
        }
    }
}
