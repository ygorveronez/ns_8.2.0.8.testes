using System;

namespace Dominio.Entidades.Embarcador.Terceiros
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TAXA_TERCEIRO", EntityName = "TaxaTerceiro", Name = "Dominio.Entidades.Embarcador.Terceiros.TaxaTerceiro", NameType = typeof(TaxaTerceiro))]
    public class TaxaTerceiro : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TAT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "TAT_DESCRICAO", TypeType = typeof(string), Length = 500, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "TAT_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "TAT_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "VigenciaInicial", Column = "TAT_VIGENCIA_INICIAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? VigenciaInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "VigenciaFinal", Column = "TAT_VIGENCIA_FINAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? VigenciaFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoTaxaTerceiro", Column = "TAT_TIPO_TAXA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoTaxaTerceiro), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoTaxaTerceiro TipoTaxaTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_TERCEIRO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Terceiro { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Justificativa", Column = "JUS_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Fatura.Justificativa Justificativa { get; set; }

        public virtual string DescricaoAtivo
        {
            get { return Ativo ? "Ativo" : "Inativo"; }
        }
    }
}
