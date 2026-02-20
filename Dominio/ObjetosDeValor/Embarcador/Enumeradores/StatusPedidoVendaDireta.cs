namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum StatusPedidoVendaDireta
    {
        Todos = 0,
        AgendadoFora = 1,
        Aprovado = 2,
        Baixado = 3,
        FaltaAgendar = 4,
        Agendado = 5,
        Contato1 = 6,
        Contato2 = 7,
        Contato3 = 8,
        Problema = 9,
        Reagendar = 10,
        ClienteBaixa = 11,
        AguardandoVerificacaoCertisign = 12
    }

    public static class StatusPedidoVendaDiretaHelper
    {
        public static string ObterDescricao(this StatusPedidoVendaDireta status)
        {
            switch (status)
            {
                case StatusPedidoVendaDireta.AgendadoFora: return "Agendado Fora";
                case StatusPedidoVendaDireta.Aprovado: return "Aprovado";
                case StatusPedidoVendaDireta.Baixado: return "Baixado";
                case StatusPedidoVendaDireta.FaltaAgendar: return "Falta Agendar";
                case StatusPedidoVendaDireta.Agendado: return "Agendado";
                case StatusPedidoVendaDireta.Contato1: return "Contato 1";
                case StatusPedidoVendaDireta.Contato2: return "Contato 2";
                case StatusPedidoVendaDireta.Contato3: return "Contato 3";
                case StatusPedidoVendaDireta.Problema: return "Problema";
                case StatusPedidoVendaDireta.Reagendar: return "Reagendar";
                case StatusPedidoVendaDireta.ClienteBaixa: return "Cliente Baixa";
                case StatusPedidoVendaDireta.AguardandoVerificacaoCertisign: return "Aguardando Verificação Certisign";
                default: return string.Empty;
            }
        }
    }
}
