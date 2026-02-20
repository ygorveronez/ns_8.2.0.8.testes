namespace Dominio.ObjetosDeValor.Embarcador.Integracao.SAD
{
    public sealed class RetornoIntegracaoSADAgendamentoColetaPedido
    {
        public int CodigoPedido { get; set; }
        public string NumeroPedido { get; set; }
        public string Mensagem { get; set; }
        public string Senha { get; set; }
        public bool Sucesso { get; set; }
        public string CorLinha { get; set; }
        public string CorFonte { get; set; }
    }
}
