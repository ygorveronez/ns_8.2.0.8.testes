using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga
{
    public class MontagemCargaParametros
    {
        public decimal pesoMaximo { get; set; }
        public decimal palletMaximo { get; set; }
        public decimal cubagemMaximo { get; set; }
        public int codigoSessaoRoteirizador { get; set; }
        public TipoMontagemCarregamentoPedidoProduto tipoMontagemCarregamentoPedidoProduto { get; set; }
        public PrioridadeMontagemCarregamentoPedidoProduto prioridadeMontagemCarregamentoPedidoProduto { get; set; }
        public TipoStatusEstoqueMontagemCarregamentoPedidoProduto tipoStatusEstoqueMontagemCarregamentoPedidoProduto { get; set; }
        public bool filaMontagemPedidoProdutoResumida { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaGrupoPedido> listaGrupoPedidos { get; set; }
        public List<Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacaoAgrupa> linhasSeparacaoAgrupa { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.NivelQuebraProdutoRoteirizar nivelQuebraProdutoRoteirizar { get; set; }
    }
}
