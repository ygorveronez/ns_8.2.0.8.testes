namespace Dominio.ObjetosDeValor.Embarcador.Pedido
{
    public class TipoOperacao
    {
        public int Codigo { get; set; }
        public string CodigoEmbarcador { get; set; }
        public string Descricao { get; set; }
        public bool OperacaoDestinadaCTeComplementar { get; set; }
        public bool IgnorarRateioConfiguradoPorto { get; set; }
        public bool NaoExigeVeiculoParaEmissao { get; set; }
    }
}
