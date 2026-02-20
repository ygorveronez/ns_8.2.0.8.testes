namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoPlanejamentoPedido
    {
        Pendente = 0,
        EntregueAoMotorista = 1,
        MotoristaNoLocalCarregamento = 2,
        Problema = 3
    }

    public static class SituacaoPlanejamentoPedidoHelper
    {
        public static string ObterCorLinha(this SituacaoPlanejamentoPedido situacao)
        {
            switch (situacao)
            {
                case SituacaoPlanejamentoPedido.EntregueAoMotorista: return "#fff2cc";
                case SituacaoPlanejamentoPedido.MotoristaNoLocalCarregamento: return "#92D050";
                case SituacaoPlanejamentoPedido.Pendente: return "#ffffff";
                case SituacaoPlanejamentoPedido.Problema: return "#ffff00";
                default: return string.Empty;
            }
        }

        public static string ObterDescricao(this SituacaoPlanejamentoPedido situacao)
        {
            switch (situacao)
            {
                case SituacaoPlanejamentoPedido.EntregueAoMotorista: return "Entregue ao Motorista";
                case SituacaoPlanejamentoPedido.MotoristaNoLocalCarregamento: return "CT-e Emitido";
                case SituacaoPlanejamentoPedido.Pendente: return "Pendente";
                case SituacaoPlanejamentoPedido.Problema: return "OK";
                default: return string.Empty;
            }
        }
    }
}
