namespace Dominio.ObjetosDeValor.Embarcador.Terceiros
{
    public class RetornoAutorizacaoPagamento
    {
        public int CodigoCIOT { get; set; }
        public bool Sucesso { get; set; }
        public string MensagemRetorno { get; set; }
    }
}
