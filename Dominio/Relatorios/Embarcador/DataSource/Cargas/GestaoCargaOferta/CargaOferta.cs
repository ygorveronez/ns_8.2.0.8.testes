using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Cargas.GestaoCargaOferta
{
    public class CargaOferta
    {
        #region Propriedades
        public long Codigo { get; set; }
        public int CodigoCarga { get; set; }
        public string NumeroCarga { get; set; }
        public string Placa { get; set; }
        public string Motorista { get; set; }
        public decimal? ValorFrete { get; set; }
        public decimal? Quilometragem { get; set; }
        public DateTime? DataCarregamento { get; set; }
        public DateTime? DataPrevisaoEntrega { get; set; }
        public DateTime? DataOferta { get; set; }
        public DateTime? DataFimOferta { get; set; }
        public DateTime? DataOfertaAceite { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao SituacaoIntegracao { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga SituacaoCarga { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaOferta SituacaoCargaOferta { get; set; }
        public string Destinatario { get; set; }
        public string Remetente { get; set; }
        public string Transportadores { get; set; }
        public string Destino { get; set; }
        public string Origem { get; set; }

        #endregion Propriedades

        #region Propriedades Com Regras

        public string DataCarregamentoFormatada
        {
            get { return this.DataCarregamento.HasValue ? this.DataCarregamento?.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataPrevisaoEntregaFormatada
        {
            get { return this.DataPrevisaoEntrega.HasValue ? this.DataPrevisaoEntrega?.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataOfertaFormatada
        {
            get { return this.DataOferta.HasValue ? this.DataOferta?.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataOfertaAceiteFormatada
        {
            get { return this.DataOfertaAceite.HasValue ? this.DataOfertaAceite?.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string SituacaoCargaOfertaFormatada
        {
            get { return this.SituacaoCargaOferta.ObterDescricao(); }
        }

        public string SituacaoCargaFormatada
        {
            get { return this.SituacaoCarga.ObterDescricao(); }
        }

        public string SituacaoIntegracaoFormatada
        {
            get { return this.SituacaoIntegracao.ObterDescricao(); }
        }

        public string ValorFreteFormatada
        {
            get
            {
                if (this.ValorFrete != 0)
                    return $"R$ {this.ValorFrete:F2}";
                else
                    return string.Empty;
            }
        }

        public string QuilometragemFormatada
        {
            get
            {
                if (this.Quilometragem.HasValue && this.Quilometragem != 0)
                    return $"{this.Quilometragem:F2}";
                else
                    return string.Empty;
            }
        }

        #endregion Propriedades Com Regras
    }
}