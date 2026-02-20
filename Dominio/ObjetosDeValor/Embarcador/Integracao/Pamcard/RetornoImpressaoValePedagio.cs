namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Pamcard
{
    public class RetornoImpressaoValePedagio
    {
        public int MensagemCodigo { get; set; }

        public string MensagemDescricao { get; set; }

        public string ViagemNumeroCartao { get; set; }

        public string ViagemNumeroCartaoPortadorDocumento { get; set; }

        public string ViagemTipoCartaoPortadorDocumento { get; set; }

        public string ViagemNomePortadorCartao { get; set; }

        public string ViagemTipoCartao { get; set; }

        public string ViagemDataPartida { get; set; }

        public string ViagemNomeCidadeDestino { get; set; }

        public string ViagemNomeEstadoDestino { get; set; }

        public string ViagemNomePaisDestino { get; set; }

        public int ViagemDigito { get; set; }

        public int ViagemDocumentoQuantidade { get; set; }

        public int ViagemFavorecidoQuantidade { get; set; }

        public string ViagemID { get; set; }

        public string ViagemOrigemCidadeNome { get; set; }

        public string ViagemOrigemEstadoNome { get; set; }

        public string ViagemOrigemPaisNome { get; set; }

        public int ViagemParcelaQuantidade { get; set; }

        public int ViagemPedagioKM { get; set; }

        public string ViagemPedagioOrigem { get; set; }

        public decimal ViagemPedagioPracaQuantidade { get; set; }

        public string ViagemPedagioSolucaoID { get; set; }

        public string ViagemPedagioTempoPercurso { get; set; }

        public decimal ViagemPedagioValor { get; set; }

        public int ViagemPontoQuantidade { get; set; }

        public string ViagemStatus { get; set; }

        public string ViagemUnidadeDocumentoNumero { get; set; }

        public string ViagemUnidadeDocumentoTipo { get; set; }

        public string ViagemVaflexIndicador { get; set; }

        public decimal ViagemValor { get; set; }

        public string ViagemVeiculoCategoria { get; set; }

        public string ViagemVeiculoPlaca { get; set; }

    }
}
