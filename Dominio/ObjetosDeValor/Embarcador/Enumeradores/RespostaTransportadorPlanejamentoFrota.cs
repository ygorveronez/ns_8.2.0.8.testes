namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum RespostaTransportadorPlanejamentoFrota
    {
        Pendente = 0,
        Confirmado = 1,
        Rejeitado = 2
    }

    public static class RespostaTransportadorPlanejamentoFrotaHelper
    {
        public static string ObterCorFonte(this RespostaTransportadorPlanejamentoFrota situacao)
        {
            switch (situacao)
            {
                case RespostaTransportadorPlanejamentoFrota.Rejeitado: return "#ffffff";
                default: return "#212529";
            }
        }

        public static string ObterCorLinha(this RespostaTransportadorPlanejamentoFrota situacao)
        {
            switch (situacao)
            {
                case RespostaTransportadorPlanejamentoFrota.Pendente: return "#91a8ee";
                case RespostaTransportadorPlanejamentoFrota.Confirmado: return "#80ff80";
                case RespostaTransportadorPlanejamentoFrota.Rejeitado: return "#ff6666";
                default: return string.Empty;
            }
        }

        public static string ObterDescricao(this RespostaTransportadorPlanejamentoFrota situacao)
        {
            switch (situacao)
            {
                case RespostaTransportadorPlanejamentoFrota.Pendente: return "Pendente";
                case RespostaTransportadorPlanejamentoFrota.Confirmado: return "Confirmado";
                case RespostaTransportadorPlanejamentoFrota.Rejeitado: return "Rejeitado";
                default: return string.Empty;
            }
        }
    }
}
