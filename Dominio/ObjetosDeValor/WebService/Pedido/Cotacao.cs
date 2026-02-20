using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Pedido
{
    public class Cotacao
    {
        public string NumeroCarrinho { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Expedidor { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Destinatario { get; set; }
        //public Dominio.ObjetosDeValor.Embarcador.Filial.Filial Filial { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pedido.TipoOperacao TipoOperacao { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco EnderecoDestino { get; set; }
        public List<Embarcador.Pedido.Produto> Produtos { get; set; }
        public decimal ValorTotalMercadoria { get; set; }


    }
}
