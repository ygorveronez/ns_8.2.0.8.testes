using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Cargas.TaxaOcupacaoVeiculo
{
    public class TaxaOcupacaoVeiculo
    {
        public int Codigo { get; set; }
        public string NumeroCarga { get; set; }
        public DateTime DataCarregamento { get; set; }
        public string Transportador { get; set; }
        public string ModeloVeiculo { get; set; }
        public string TipoCarga { get; set; }
        public string NumeroPedido { get; set; }
        public string Veiculos { get; set; }
        public string Destinatario { get; set; }
        public string Destino { get; set; }
        public string Remetente { get; set; }
        public string Origem { get; set; }
        public int NumeroColetas { get; set; }
        public int NumeroEntregas { get; set; }
        public decimal CapacidadePesoVeiculo { get; set; }
        public decimal PesoCarga { get; set; }
        public decimal TaxaOcupacao { get; set; }
        public string Filial { get; set; }
        public string TipoOperacao { get; set; }
        private SituacaoCarga SituacaoCarga { get; set; }
        public int CodAgrupamento { get; set; }
        public string Recebedor { get; set; }
        public string Expedidor { get; set; }
        public DateTime DataCriacao { get; set; }

        public string DescricaoSituacaoCarga
        {
            get { return SituacaoCarga.ObterDescricao(); }
        }
    }
}