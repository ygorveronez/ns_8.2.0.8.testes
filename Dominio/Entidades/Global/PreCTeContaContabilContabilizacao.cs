using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PRE_CTE_CONTA_CONTABIL_CONTABILIZACAO", DynamicUpdate = true, EntityName = "PreCTeContaContabilContabilizacao", Name = "Dominio.Entidades.PreCTeContaContabilContabilizacao", NameType = typeof(PreCTeContaContabilContabilizacao))]
    public class PreCTeContaContabilContabilizacao : EntidadeBase, IEquatable<PreCTeContaContabilContabilizacao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PreConhecimentoDeTransporteEletronico", Column = "PCO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.PreConhecimentoDeTransporteEletronico PreCTe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PlanoConta", Column = "PLA_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.PlanoConta PlanoConta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoContaContabil", Column = "CCI_TIPO_CONTA_CONTABIL", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil TipoContaContabil { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoContabilizacao", Column = "CCI_TIPO_CONTABILIZACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoContabilizacao), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoContabilizacao TipoContabilizacao { get; set; }

        public virtual bool Equals(PreCTeContaContabilContabilizacao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }


        public virtual string Descricao
        {
            get
            {
                return PlanoConta.Descricao;
            }
        }
    }
}
