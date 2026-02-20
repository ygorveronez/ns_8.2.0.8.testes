using Dominio.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Relatorios
{
    public class EspelhoColeta
    {
        public int Codigo { get; set; }
        public string CPFCNPJDestinatario { get; set; }
        public string NomeDestinatario { get; set; }
        public string TelefoneDestinatario { get; set; }
        public string EnderecoDestinatario { get; set; }
        public string CPFCNPJRemetente { get; set; }
        public string NomeRemetente { get; set; }
        public string TelefoneRemetente { get; set; }
        public string EnderecoRemetente { get; set; }
        public string CPFCNPJTomador { get; set; }
        public string NomeTomador { get; set; }
        public string Origem { get; set; }
        public string Destino { get; set; }
        public int Numero { get; set; }
        public DateTime? DataInicial { get; set; }
        public DateTime? DataFinal { get; set; }
        public DateTime? DataEntrega { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido Situacao { get; set; }
        public string TipoCarga { get; set; }
        public string TipoColeta { get; set; }
        public decimal ValorNFs { get; set; }
        public decimal ValorFrete { get; set; }
        public decimal Peso { get; set; }
        public TipoPagamento TipoPagamento { get; set; }
        public string Observacao { get; set; }
        public string ObservacaoCTe { get; set; }
        public int QtVolumes { get; set; }
        public int NumeroNotaCliente { get; set; }
        public string CodigoPedidoCliente { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.RequisitanteColeta Requisitante { get; set; }

        public string DescricaoSituacao
        {
            get
            {
                return this.Situacao.ToString("G");
            }
        }

        public string DescricaoTipoPagamento
        {
            get { return TipoPagamento.ObterDescricao(); }
        }

        public string DescricaoRequisitante
        {
            get
            {
                switch (this.Requisitante)
                {
                    case Embarcador.Enumeradores.RequisitanteColeta.Destinatario:
                        return "Destinatário";
                    case Embarcador.Enumeradores.RequisitanteColeta.Outros:
                        return "Outros";
                    case Embarcador.Enumeradores.RequisitanteColeta.Remetente:
                        return "Remetente";
                    default:
                        return string.Empty;
                }
            }
        }
    }
}
