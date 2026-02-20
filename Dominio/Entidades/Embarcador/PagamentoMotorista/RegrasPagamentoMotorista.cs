using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.PagamentoMotorista
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRAS_PAGAMENTO_MOTORISTA", EntityName = "RegrasPagamentoMotorista", Name = "Dominio.Entidades.Embarcador.PagamentoMotorista.RegrasPagamentoMotorista", NameType = typeof(RegrasPagamentoMotorista))]
    public class RegrasPagamentoMotorista : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RPM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "RPM_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Vigencia", Column = "RPM_VIGENCIA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? Vigencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroAprovadores", Column = "RPM_NUMERO_APROVADORES", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroAprovadores { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacoes", Column = "RPM_OBSERVACOES", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacoes { get; set; }        

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorEmpresa", Column = "RPM_EMPRESA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RegraPorEmpresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorTipo", Column = "RPM_TIPO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RegraPorTipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorValor", Column = "RPM_VALOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RegraPorValor { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Aprovadores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRAS_PAGAMENTO_FUNCIONARIOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RPM_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Usuario", Column = "FUN_CODIGO")]
        public virtual ICollection<Usuario> Aprovadores { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "RegrasPagamentoMotoristaEmpresa", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_PAGAMENTO_EMPRESA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RPM_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RegrasPagamentoMotoristaEmpresa", Column = "RPE_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.PagamentoMotorista.RegrasPagamentoMotoristaEmpresa> RegrasPagamentoMotoristaEmpresa { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "RegrasPagamentoMotoristaValor", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_PAGAMENTO_VALOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RPM_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RegrasPagamentoMotoristaValor", Column = "RPV_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.PagamentoMotorista.RegrasPagamentoMotoristaValor> RegrasPagamentoMotoristaValor { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "RegrasPagamentoMotoristaTipo", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_PAGAMENTO_TIPO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RPM_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RegrasPagamentoMotoristaTipo", Column = "RPT_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.PagamentoMotorista.RegrasPagamentoMotoristaTipo> RegrasPagamentoMotoristaTipo { get; set; }        

        public virtual bool Equals(RegrasPagamentoMotorista other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }

}
