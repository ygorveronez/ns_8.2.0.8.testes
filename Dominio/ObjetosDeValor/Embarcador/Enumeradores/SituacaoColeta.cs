namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoColeta
    {
        NaoEntregue = 0,
        EmCliente = 1,
        Entregue = 2,
        Rejeitado = 3
    }

    public static class SituacaoColetaHelper
    {
        public static string ObterDescricao(this SituacaoColeta situacaoEnterga)
        {
            switch (situacaoEnterga)
            {
                case SituacaoColeta.NaoEntregue: return "Não Entregue";
                case SituacaoColeta.EmCliente: return "Em Cliente";
                case SituacaoColeta.Entregue: return "Entregue";
                case SituacaoColeta.Rejeitado: return "Rejeitado";
                default: return "";
            }
        }
    }
}
