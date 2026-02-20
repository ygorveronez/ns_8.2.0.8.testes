using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga
{
    public class DadoProcessamentoPedidoMontagemCarga
    {
        // Parâmetros da função
        public Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }
        public Dominio.Entidades.Embarcador.Pedidos.Pedido Pedido { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaSqlPedidoProduto ObjetoMontagemCargaSqlPedidoProduto { get; set; }
        public List<int> NotasUtilizadas { get; set; }
        public Dominio.Entidades.Embarcador.Cargas.CargaPedido CargaPedido { get; set; }
        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> CargaPedidosPreCarga { get; set; }
        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido ConfiguracaoPedido { get; set; }
        public Dominio.Entidades.Embarcador.Pedidos.Stage Stage { get; set; }
        public AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware TipoServicoMultisoftware { get; set; }
        public NumeroReboque NumeroReboque { get; set; }
        public TipoCarregamentoPedido TipoCarregamentoPedido { get; set; }
        public string StringConexao { get; set; }
        public Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS Configuracao { get; set; }
        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMontagemCarga ConfiguracaoMontagemCarga { get; set; }
        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga ConfiguracaoGeralCarga { get; set; }
        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab IntegracaoIntercab { get; set; }
        public bool MontagemCargaPorPedidoProduto { get; set; }
        public bool UtilizarOutroEnderecoPedidoMesmoSePossuirRecebedor { get; set; }
        public bool? UtilizarDistribuidorPorRegiaoNaRegiaoDestino { get; set; }
        public bool PedidosDevolucao { get; set; }
        public bool PossuiRegraTomador { get; set; }
        public bool SetarOrdemEntrega { get; set; }
        public int Count { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado { get; set; }
        public DateTime DataInicialPrevisaoCarregamento { get; set; }
        public DateTime DataCarregamento { get; set; }
        public DateTime DataFinalPrevisaoCarregamento { get; set; }
        public DateTime? DataPrevistaChegadaCliente { get; set; }
        public Dominio.Entidades.Cliente Fronteira { get; set; }
        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoStage> PedidosStage { get; set; }
        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoNotaParcial> NotasParciais { get; set; }
        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParcial> CtesParciais { get; set; }
        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> ListaTodosPedidoProduto { get; set; }
        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoComponenteFrete> ListaTodosPedidosComponenteFrete { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.Carga.TotalizadorModeloDocumento> NotasPedidos { get; set; }
        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> RedespachosProximoTrecho { get; set; }
        public List<Dominio.Entidades.Embarcador.Cargas.Carga> CargaOrigemPedidos { get; set; }
        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades> CargaPedidoQuantidades { get; set; }
        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> ListaTodosPedidoProdutoCarregamento { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SaldoPedidoProduto> ProdutosPedidosNaoAtendido { get; set; }
        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> PedidosCarregamento { get; set; }
        public List<Dominio.Entidades.Embarcador.Filiais.EstadoDestinoEmpresaEmissora> EstadosDestinoEmpresaEmissora { get; set; }
        public List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro> ApolicesSeguro { get; set; }
        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> CarregamentoPedidos { get; set; }
        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> CargaPedidosComponentesCarga { get; set; }
        public List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> xMLNotaFiscals { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoRecolhimentoDestinatario> RecolhimentosDestinatarios { get; set; }
        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal> ListaNotasJaEnviadasDosPedidos { get; set; }
        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal> ListaTotalNotasJaEnviadas { get; set; }
        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal> ListaCarregamentoPedidoNotaFiscal { get; set; }
        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoAnexo> PedidosAnexos { get; set; }
        public List<Dominio.Entidades.Cliente> ClientesBloquearEmissaoDosDestinatarios { get; set; }
        public List<Dominio.Entidades.Cliente> TomadoresFilialEmissora { get; set; }
        public List<Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga> ClientesDescarga { get; set; }
        public List<Dominio.Entidades.Embarcador.ICMS.PedagioEstadoBaseCalculo> PedagioEstadosBaseCalculo { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.ICMS.TabelaAliquota> TabelaAliquotas { get; set; }        
    }
}
