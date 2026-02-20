namespace Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido
{
    public class TipoOperacao
    {
        public int Codigo { get; set; }

        public string Descricao { get; set; }

        public string Observacao { get; set; }

        public bool NaoExigeRoteirizacaoMontagemCarga { get; set; }

        public bool PermitirInformarRecebedorMontagemCarga { get; set; }

        public ConfiguracaoTipoOperacaoCarga ConfiguracaoCarga { get; set; }

        public ConfiguracaoTipoOperacaoMobile ConfiguracaoMobile { get; set; }

        public ConfiguracaoTipoOperacaoMontagemCarga ConfiguracaoMontagemCarga { get; set; }
    }
}
