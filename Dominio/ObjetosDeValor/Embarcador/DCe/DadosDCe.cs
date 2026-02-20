using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.DCe
{ 
    public class DadosDCe
    {
        public string Chave { get; set; }
        public string CNPJCPFEmitente { get; set; }
        public string NomeEmitente { get; set; }
        public string CNPJCPFDestinatario { get; set; }
        public string IEDestinatario { get; set; }
        public string NomeDestinatario { get; set; }
        public string EnderecoEmitente { get; set; }
        public string EnderecoDestinatario { get; set; }
        public decimal ValorTotal { get; set; }
        public string DataEmissao { get; set; }
        public string Modelo { get; set; }
        public string Serie { get; set; }
        public string Numero { get; set; }
        public string CNPJTransportador { get; set; }
        public string ModalidadeTransporte { get; set; }
        public List<Produto> Produtos { get; set; }
        public string Empresa { get; set; }
        public string BairroEmitente { get; set; }
        public string BairroDestinatario { get; set; }
        public string CidadeEmitente { get; set; }
        public string CidadeDestinatario { get; set; }
        public string CEPEmitente { get; set; }
        public string CEPDestinatario { get; set; }
        public string ComplementoEmitente { get; set; }
        public string ComplementoDestinatario { get; set; }
        public string NumeroEnderecoEmitente { get; set; }
        public string NumeroEnderecoDestinatario { get; set; }
        public string PaisDestinatario { get; set; }
        public string PaisEmitente { get; set; }
        public string TipoDestinatario { get; set; }
        public string TipoEmitente { get; set; }
        public string CodigoCidadeEmitente { get; set; }
        public string CodigoCidadeDestinatario { get; set; }
    }
    public class Produto
    {
        public string Descricao { get; set; }
        public decimal Quantidade { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal ValorTotal { get; set; }
        public string NCM { get; set; }
        public string InfAdProd { get; set; } 
    }
    
}
