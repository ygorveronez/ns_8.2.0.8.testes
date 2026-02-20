using System;

namespace Dominio.Entidades.Embarcador.Frota
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TMS_PNEU_DIMENSAO", EntityName = "Frota.DimensaoPneu", Name = "Dominio.Entidades.Embarcador.Frota.DimensaoPneu", NameType = typeof(DimensaoPneu))]
    public class DimensaoPneu : EntidadeBase, IEquatable<DimensaoPneu>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PDM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Aplicacao", Column = "PDM_APLICACAO", TypeType = typeof(string), Length = 50, NotNull = true)]
        public virtual string Aplicacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Aro", Column = "PDM_ARO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal Aro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PDM_LARGURA", TypeType = typeof(int), NotNull = true)]
        public virtual int Largura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PDM_PERFIL", TypeType = typeof(int), NotNull = true)]
        public virtual int Perfil { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Radial", Column = "PDM_RADIAL", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Radial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "PDM_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        public virtual string Descricao
        {
            get { return Aplicacao; }
        }

        public virtual string DescricaoAtivo
        {
            get { return this.Ativo ? "Ativo" : "Inativo"; }
        }

        public virtual bool Equals(DimensaoPneu other)
        {
            return (other.Codigo == this.Codigo);
        }
    }
}
