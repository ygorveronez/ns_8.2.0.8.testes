using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Carga
{
    public class NotaFiscal
    {
        public int Numero { get; set; }
        public string Serie { get; set; }
        public string Chave { get; set; }
        public decimal Valor { get; set; }
        public decimal ValorProdutos { get; set; }
        public decimal ValorFrete { get; set; }
        public int Volumes { get; set; }
        public decimal Peso { get; set; }
        public DateTime DataEmissao { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal TipoOperacaoNotaFiscal { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Emitente { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Destinatario { get; set; }
        public bool PossuiCTe { get; set; }
        public bool PossuiNFS { get; set; }
        public bool PossuiNFSManual { get; set; }
        public List<Embarcador.Pedido.Produto> Produtos { get; set; }
    }
}
