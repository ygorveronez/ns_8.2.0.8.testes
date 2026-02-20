namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum MonitoramentoStatusViagem
    {
        Todos = -1,
        SemViagem = 0,
        EmViagem = 1,
        Retornando = 2,
    }

    public static class MonitoramentoStatusViagemHelper
    {
        public static string ObterDescricao(this MonitoramentoStatusViagem status)
        {
            switch (status)
            {
                case MonitoramentoStatusViagem.SemViagem: return "Sem viagem";
                case MonitoramentoStatusViagem.EmViagem: return "Em viagem";
                case MonitoramentoStatusViagem.Retornando: return "Retornando";
                default: return string.Empty;
            }
        }
    }
}



