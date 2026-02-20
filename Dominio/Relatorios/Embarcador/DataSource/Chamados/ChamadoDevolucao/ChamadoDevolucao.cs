using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Relatorios.Embarcador.DataSource.Chamados.ChamadoDevolucao
{
    public class ChamadoDevolucao
    {
		#region Propriedades

		public int Codigo { get; set; }

		public int NumeroCte { get; set; }

		public int NfDevolucao { get; set; }

		public int NfOrigem { get; set; }

		public string MotivoDevolucao { get; set; }

		private SituacaoChamado Situacao { get; set; }

		public bool DevolucaoParcial { get; set; }

		public string Origens { get; set; }

		public string Destinos { get; set; }

		public string Veiculos { get; set; }

		public decimal QuantidadeDevolucao { get; set; }

		public decimal ValorDevolucao { get; set; }

		public decimal ValorTotalMercadorias { get; set; }

		public int NumeroAtendimento { get; set; }
		
		public DateTime DataAberturaAtendimento { get; set; }
		
		public string NumeroCarga { get; set; }
		
		public string GrupoTomadorCarga { get; set; }
		
		public string ResponsavelAtendimento { get; set; }
        
		public decimal QuantidadeDevolvida { get; set; }

        #endregion

        #region Propriedades com Regras

        public string TipoDevolucao
		{
			//get { return Situacao.ObterDescricao() != "Aberto" ? DevolucaoParcial ? TipoColetaEntregaDevolucao.Parcial.ObterDescricao() : TipoColetaEntregaDevolucao.Total.ObterDescricao() : string.Empty; }
			get { return DevolucaoParcial ? TipoColetaEntregaDevolucao.Parcial.ObterDescricao() : TipoColetaEntregaDevolucao.Total.ObterDescricao();  }
		}

        public string DataAberturaAtendimentoFormatado
        {
            get { return DataAberturaAtendimento != DateTime.MinValue ? DataAberturaAtendimento.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        #endregion
    }
}