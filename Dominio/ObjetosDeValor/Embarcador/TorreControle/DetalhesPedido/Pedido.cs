using Dominio.Entidades.Embarcador.Pedidos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dominio.ObjetosDeValor.Embarcador.TorreControle.DetalhesPedido
{
    public class Pedido
    {
        #region Métodos Públicos

        public int Codigo { get; set; }

        public int CodigoCargaPedido { get; set; }

        public int CodigoCarga { get; set; }

        public string NumeroPedidoEmbarcador { get; set; }

        public int ProtocoloIntegracao { get; set; }

        public DateTime? DataEntrega { get; set; }

        public DateTime? DataPrevisaoEntrega { get; set; }

        public Filial Filial { get; set; }

        public TipoOperacao TipoOperacao { get; set; }

        public Carga Carga { get; set; }

        public ModeloVeicular ModeloVeicular { get; set; }

        public Cliente Destinatario { get; set; }

        public Cliente Remetente { get; set; }

        public List<Produto> Produtos { get; set; }

        public List<NotaFiscal> NotasFiscais { get; set; }

        public List<Ocorrencia> Ocorrencias { get; set; }
        public List<Ocorrencia> OcorrenciasComerciais { get; set; }

        public CargaEntrega CargaEntrega { get; set; }

        public string EscritorioVenda { get; set; }

        public string EquipeVendas { get; set; }

        public string TipoMercadoria { get; set; }

        public string CanalVenda { get; set; }

        public string Observacao { get; set; }

        public decimal Peso { get; set; }
        public string CanalEntrega { get; set; }
        public string Adicional7 { get; set; }
        public bool PedidoCritico { get; set; }

        #endregion

        #region Propriedades com regras 

        public string DataFaturamento
        {
            get { return NotasFiscais.FirstOrDefault()?.DataEmissao?.ToDateTimeString(); }
        }

        public string DataEntregaFormatada
        {
            get
            {
                return DataEntrega.HasValue ? DataEntrega.Value.ToDateTimeString() : string.Empty;
            }
        }

        public string PesoTotal
        {
            get { return Peso.ToString("n3"); }
        }

        #endregion
    }
}
