using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Avarias
{
	public sealed class DadosPesquisaLotesPendentes
    {
		#region Propriedades

		public int Codigo { get; set; }

		public int Numero { get; set; }

		public DateTime DataAbertura { get; set; }

		public SituacaoLote Situacao { get; set; }

		public EtapaLote Etapa { get; set; }

		public string Criador { get; set; }

		public string Responsavel { get; set; }

		public string Filial { get; set; }

		public string Tipo { get; set; }

		public string CNPJ { get; set; }

		public string CodigoEmpresa { get; set; }

		public string RazaoSocial { get; set; }

		public double TotalHoras { get; set; }

		public decimal ValorDescontos { get; set; }

		public decimal ValorAvarias { get; set; }

		#endregion

		#region Propriedades com Regras

		public string DataAberturaFormatada
		{
			get { return DataAbertura.ToString("dd/MM/yyyy"); }
		}

		public string SituacaoFormatada
		{
			get { return Situacao.ObterDescricao(); }
		}

		public string EtapaFormatada
		{
			get { return Etapa.ObterDescricao(); }
		}

		public string CNPJEmpresa
		{
			get
			{
				return this.Tipo != null && this.Tipo.Equals("F")
					? string.Format(@"{0:000\.000\.000\-00}", long.Parse(this.CNPJ))
					: this.Tipo != null && this.Tipo.Equals("E")
						? string.Empty
						: string.Format(@"{0:00\.000\.000\/0000\-00}", long.Parse(this.CNPJ));
			}
		}

		public string Empresa
		{
			get
			{
				return $"{(string.IsNullOrWhiteSpace(this.CodigoEmpresa) ? "" : this.CodigoEmpresa + " ")}{(string.IsNullOrWhiteSpace(this.RazaoSocial) ? "" : this.RazaoSocial + " ")}";
			}
		}

		public string TempoEtapa
		{
			get { return this.TotalHoras.ToString("n1").Replace(',', '.') + "h"; }
		}

		public string Valor
		{
			get { return (this.ValorAvarias - this.ValorDescontos).ToString("n2"); }
		}

		#endregion
	}
}

