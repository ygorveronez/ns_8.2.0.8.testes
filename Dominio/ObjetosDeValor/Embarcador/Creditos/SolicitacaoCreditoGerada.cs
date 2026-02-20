namespace Dominio.ObjetosDeValor.Embarcador.Creditos
{
    public class SolicitacaoCreditoGerada
    {
        public Dominio.Entidades.Embarcador.Creditos.SolicitacaoCredito SolicitacaoCredito { set; get; }
        public bool GerouSolicitacao { set; get; }
        public string MensagemRetorno { set; get; }
    }
}
