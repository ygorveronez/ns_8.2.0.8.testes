namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Carrefour
{
    public sealed class RetornoRequisicao
    {
        public string CodigoRecibo { get; set; }

        public string MensagemErro { get; set; }

        public string ResultadoOperacao { get; set; }

        public bool ResultadoOperacaoSucesso
        {
            get { return ResultadoOperacao == "1"; }
        }
    }
}
