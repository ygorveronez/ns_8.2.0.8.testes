namespace Dominio.ObjetosDeValor.WebService.Pedido
{
    public class RetornoCotacao
    {
        public long ProtocoloCotacao { get; set; }
        public ValorCotacao ValorCotacao { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco EnderecoDestino { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa Transportador { get; set; }
        public int PrazoEntrega { get; set; }
        public decimal DistanciaRaioKM { get; set; }
        public string DataPrazoEntrega { get; set; }
        public string DataPrevisaoColeta { get; set; }
        public string CodigoIntegracaoTabelaFreteCliente { get; set; }
        public string CanalEntrega { get; set; }
    }
}
