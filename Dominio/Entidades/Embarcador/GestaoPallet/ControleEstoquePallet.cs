using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.GestaoPallet
{
	[NHibernate.Mapping.Attributes.Class(0, Table = "T_CONTROLE_ESTOQUE_PALLET", EntityName = "ControleEstoquePallet", Name = "Dominio.Entidades.Embarcador.GestaoPallet.ControleEstoquePallet", NameType = typeof(ControleEstoquePallet))]
	public class ControleEstoquePallet : EntidadeBase
	{
		[NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CPT_CODIGO")]
		[NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
		public virtual int Codigo { get; set; }

		[NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
		public virtual Filiais.Filial Filial { get; set; }

		[NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
		public virtual Empresa Transportador { get; set; }

		[NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
		public virtual Cliente Cliente { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeTotalPallets", Column = "CPT_QUANTIDADE_TOTAL_PALLETS", TypeType = typeof(int), NotNull = true)]
		public virtual int QuantidadeTotalPallets { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "ResponsavelPallet", Column = "CPT_RESPONSAVEL_PALLET", TypeType = typeof(ResponsavelPallet), NotNull = true)]
		public virtual ResponsavelPallet ResponsavelPallet { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "TipoEstoquePallet", Column = "CPT_TIPO_ESTOQUE_PALLET", TypeType = typeof(TipoEstoquePallet), NotNull = false)]
		public virtual TipoEstoquePallet TipoEstoquePallet { get; set; }

		public virtual string Descricao
		{
			get { return $"Controle Estoque {TipoEstoquePallet.ObterDescricao()} {ResponsavelPallet.ObterDescricao()}"; }
		}
	}
}