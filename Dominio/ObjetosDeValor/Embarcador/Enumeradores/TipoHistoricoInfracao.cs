namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoHistoricoInfracao
    {
        Pago = 1,
        Recusto = 2,
        Deferido = 3,
        Indeferido = 4,
        EmAberto = 5,
        EmProcesso = 6,
        EnvioRecibo = 7,
        ReciboAssinado = 8,
        Finalizado = 9
    }

    public static class TipoHistoricoInfracaoHelper
    {
        public static string ObterDescricao(this TipoHistoricoInfracao tipo)
        {
            switch (tipo)
            {
                case TipoHistoricoInfracao.Pago: return "Pago";
                case TipoHistoricoInfracao.Recusto: return "Recusto";
                case TipoHistoricoInfracao.Deferido: return "Deferido";
                case TipoHistoricoInfracao.Indeferido: return "Indeferido";
                case TipoHistoricoInfracao.EmAberto: return "Em Aberto";
                case TipoHistoricoInfracao.EmProcesso: return "Em Processo";
                case TipoHistoricoInfracao.EnvioRecibo: return "Envio de Recibo";
                case TipoHistoricoInfracao.ReciboAssinado: return "Recibo Assinado";
                case TipoHistoricoInfracao.Finalizado: return "Finalizado";
                default: return string.Empty;
            }
        }
    }
}
