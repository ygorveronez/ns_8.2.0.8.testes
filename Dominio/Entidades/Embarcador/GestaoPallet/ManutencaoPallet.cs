using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.GestaoPallet
{
	[NHibernate.Mapping.Attributes.Class(0, Table = "T_MANUTENCAO_PALLET", EntityName = "ManutencaoPallet", Name = "Dominio.Entidades.Embarcador.GestaoPallet.ManutencaoPallet", NameType = typeof(ManutencaoPallet))]
	public class ManutencaoPallet : EntidadeBase, Interfaces.Embarcador.Entidade.IEntidade
	{
		public ManutencaoPallet()
		{
			DataCriacao = DateTime.Now;
		}

		[NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MNP_CODIGO")]
		[NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
		public virtual int Codigo { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "DataCriacao", Column = "MNP_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = false)]
		public virtual DateTime DataCriacao { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadePallets", Column = "MNP_QUANTIDADE_PALLET", TypeType = typeof(int), NotNull = true)]
		public virtual int QuantidadePallets { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "TipoManutencaoPallet", Column = "MNP_TIPO_MANUTENCAO_PALLET", TypeType = typeof(TipoManutencaoPallet), NotNull = false)]
		public virtual TipoManutencaoPallet TipoManutencaoPallet { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "TipoMovimentacao", Column = "MNP_TIPO_MOVIMENTACAO", TypeType = typeof(TipoEntradaSaida), NotNull = true)]
		public virtual TipoEntradaSaida TipoMovimentacao { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "MNP_OBSERVACAO", TypeType = typeof(string), Length = 400, NotNull = false)]
		public virtual string Observacao { get; set; }

		[NHibernate.Mapping.Attributes.ManyToOne(0, Class = "XMLNotaFiscal", Column = "NFX_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
		public virtual Pedidos.XMLNotaFiscal XMLNotaFiscal { get; set; }

		[NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
		public virtual Cargas.Carga Carga { get; set; }

		[NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
		public virtual Filiais.Filial Filial { get; set; }

		[NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ControleEstoquePallet", Column = "CPT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
		public virtual ControleEstoquePallet ControleEstoquePallet { get; set; }

		public virtual string Descricao
		{
			get { return $"{TipoMovimentacao.ObterDescricao()} da Manutenção de Pallet"; }
		}
	}
}