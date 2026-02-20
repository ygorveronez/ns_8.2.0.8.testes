using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.SAD
{
    public sealed class RetornoIntegracaoSADAgendamentoColeta
    {
        public string NumeroAgenda { get; set; }
        public string Mensagem { get; set; }
        public int NumeroPedidos { get; set; }
        public List<RetornoIntegracaoSADAgendamentoColetaPedido> Pedidos { get; set; }
        public string CorLinha { get; set; }
        public string CorFonte { get; set; }
        public bool Sucesso { get; set; }
    }
}
