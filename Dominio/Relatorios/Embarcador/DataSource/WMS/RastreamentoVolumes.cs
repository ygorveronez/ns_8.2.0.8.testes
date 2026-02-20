using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.WMS
{
    public class RastreamentoVolumes
    {
        public string NumeroPedidoEmbarcador { get; set; }
        public string Carga { get; set; }
        public DateTime DataPedido { get; set; }
        public string DataPedidoFormatada
        {
            get
            {
                return DataPedido > DateTime.MinValue ? DataPedido.ToString("dd/MM/yyyy") : string.Empty;
            }
        }
        public DateTime DataRecebimento { get; set; }
        public string DataRecebimentoFormatada
        {
            get
            {
                return DataRecebimento > DateTime.MinValue ? DataRecebimento.ToString("dd/MM/yyyy") : string.Empty;
            }
        }
        public string UsuarioRecebimento { get; set; }
        public int NumeroNF { get; set; }
        public int Serie { get; set; }
        public decimal Quantidade { get; set; }
        public decimal PesoLiquido { get; set; }
        public decimal PesoBruto { get; set; }
        public decimal ValorMercadoria { get; set; }
        public TipoRecebimentoMercadoria TipoNF { get; set; }
        public string TipoNFDescricao {
            get
            {
                return TipoNF.ObterDescricao();
            }
        }

        public decimal QtdItensRecebidos { get; set; }
        public decimal QtdItensFaltantes { get; set; }
        public string Ocorrencias { get; set; }
        public DateTime DataArmazen { get; set; }
        public string DataArmazenFormatada
        {
            get
            {
                return DataArmazen > DateTime.MinValue ? DataArmazen.ToString("dd/MM/yyyy") : string.Empty;
            }
        }

        public string OperadorArmazen { get; set; }
        public string ProdutoEmbarcador { get; set; }
        public string Deposito { get; set; }
        public string Bloco { get; set; }
        public string Rua { get; set; }
        public string Posicao { get; set; }
        public string Local { get; set; }
        public string CodigoBarras { get; set; }
        public string EtiquetaMasterPallet { get; set; }
        public string Descricao { get; set; }
        public DateTime DataVencimento { get; set; }
        public string DataVencimentoFormatada
        {
            get
            {
                return DataVencimento > DateTime.MinValue ? DataVencimento.ToString("dd/MM/yyyy") : string.Empty;
            }
        }
        public string TipoMercadoria { get; set; }
        public string UN { get; set; }
        public decimal QtdLote { get; set; }
        public string RegistroOcorrencia { get; set; }
        public decimal QtdDevolvida { get; set; }
        public decimal QtdDisponivel { get; set; }
        public DateTime DataExpedicao { get; set; }
        public string DataExpedicaoFormatada
        {
            get
            {
                return DataExpedicao > DateTime.MinValue ? DataExpedicao.ToString("dd/MM/yyyy") : string.Empty;
            }
        }
        public string UsuarioExpedicao { get; set; }
        public string EtiquetaExpedicao { get; set; }
        public decimal Volumes { get; set; }
        public decimal Embarcados { get; set; }
        public decimal Falta { get; set; }
        public string Remetente { get; set; }
        public string UfRemetente { get; set; }
        public string Destinatario { get; set; }
        public string UfDestinatario { get; set; }
        public string Expedidor { get; set; }
        public string UfExpedidor { get; set; }
        public string CargaTransbordo { get; set; }
        public DateTime DataConferencia { get; set; }
        public string DataConferenciaFormatada
        {
            get
            {
                return DataConferencia > DateTime.MinValue ? DataConferencia.ToString("dd/MM/yyyy") : string.Empty;
            }
        }
        public DateTime DataEmbarque { get; set; }
        public string DataEmbarqueFormatada
        {
            get
            {
                return DataEmbarque > DateTime.MinValue ? DataEmbarque.ToString("dd/MM/yyyy") : string.Empty;
            }
        }
        public string MDFe { get; set; }
        public string TipoOperacao { get; set; }
        public string Veiculo { get; set; }
        public string Transportador { get; set; }
        public string Motorista { get; set; }
        public string PlacaVeiculo { get; set; }
        public DateTime ChegadaFilial { get; set; }
        public string ChegadaFilialFormatada
        {
            get
            {
                return ChegadaFilial > DateTime.MinValue ? ChegadaFilial.ToString("dd/MM/yyyy") : string.Empty;
            }
        }
    }
}
