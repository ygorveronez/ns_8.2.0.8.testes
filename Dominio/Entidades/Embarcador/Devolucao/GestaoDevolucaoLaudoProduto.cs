using System;

namespace Dominio.Entidades.Embarcador.Devolucao
{
	[NHibernate.Mapping.Attributes.Class(0, Table = "T_GESTAO_DEVOLUCAO_LAUDO_PRODUTO", EntityName = "GestaoDevolucaoLaudoProduto", Name = "Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoLaudoProduto", NameType = typeof(GestaoDevolucaoLaudoProduto))]

	public class GestaoDevolucaoLaudoProduto : EntidadeBase
	{
		#region Atributos

		[NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "DLP_CODIGO")]
		[NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
		public virtual long Codigo { get; set; }

		[NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GestaoDevolucaoLaudo", Column = "GDL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
		public virtual GestaoDevolucaoLaudo Laudo { get; set; }

		[NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ProdutoEmbarcador", Column = "PRO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
		public virtual Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador Produto { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "ProdutoDescricao", Column = "DLP_PRODUTO_DESCRICAO", TypeType = typeof(string), Length = 250, NotNull = false)]
		public virtual string ProdutoDescricao { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeOrigem", Column = "DLP_QUANTIDADE_ORIGEM", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
		public virtual decimal QuantidadeOrigem { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeDevolvida", Column = "DLP_QUANTIDADE_DEVOLVIDA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
		public virtual decimal QuantidadeDevolvida { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeAvariada", Column = "DLP_QUANTIDADE_AVARIADA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
		public virtual decimal QuantidadeAvariada { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "ValorAvariado", Column = "DLP_VALOR_AVARIADO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
		public virtual decimal ValorAvariado { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeSobras", Column = "DLP_QUANTIDADE_SOBRAS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
		public virtual decimal QuantidadeSobras { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "ValorSobras", Column = "DLP_VALOR_SOBRAS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
		public virtual decimal ValorSobras { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeSemCondicao", Column = "DLP_QUANTIDADE_SEM_CONDICAO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
		public virtual decimal QuantidadeSemCondicao { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "ValorSemCondicao", Column = "DLP_VALOR_SEM_CONDICAO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
		public virtual decimal ValorSemCondicao { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeFalta", Column = "DLP_QUANTIDADE_FALTA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
		public virtual decimal QuantidadeFalta { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "ValorFalta", Column = "DLP_VALOR_FALTA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
		public virtual decimal ValorFalta { get; set; }

        [Obsolete("Campo não será mais utilizado.")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "NotaDebito", Column = "DLP_NOTA_DEBITO", TypeType = typeof(string), NotNull = false, Length = 100)]
		public virtual string NotaDebito { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeDescarte", Column = "DLP_QUANTIDADE_DESCARTE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
		public virtual decimal QuantidadeDescarte { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeManutencao", Column = "DLP_QUANTIDADE_MANUTENCAO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
		public virtual decimal QuantidadeManutencao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotal", Column = "DLP_VALOR_TOTAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotal { get; set; }

        #endregion

        #region Atributos Virtuais
        public virtual string Descricao
		{
			get
			{
				return string.Empty;
			}
		}
		#endregion
	}
}
