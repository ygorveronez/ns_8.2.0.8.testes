using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Chamados
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRAS_ANALISE_CHAMADOS", EntityName = "RegrasAnaliseChamados", Name = "Dominio.Entidades.Embarcador.Chamados.RegrasAnaliseChamados", NameType = typeof(RegrasAnaliseChamados))]
    public class RegrasAnaliseChamados : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RAC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RAC_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RAC_VIGENCIA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? Vigencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RAC_OBSERVACOES", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacoes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RAC_FILIAL", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorFilial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RAC_MOTIVO_CHAMADA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorMotivoChamado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RAC_REGIAO_DESTINO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RegraPorRegiaoDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RAC_CARGA_DESCARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RegraPorCargaDescarga { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Aprovadores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRAS_ANALISE_FUNCIONARIOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Usuario", Column = "FUN_CODIGO")]
        public virtual ICollection<Usuario> Aprovadores { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "RegrasMotivoChamado", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_CHAMADO_MOTIVO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RegrasMotivoChamado", Column = "RCM_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Chamados.RegrasMotivoChamado> RegrasMotivoChamado { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "RegrasFilial", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_CHAMADO_FILIAL")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RegrasChamadosFilial", Column = "RCF_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Chamados.RegrasChamadosFilial> RegrasFilial { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "RegrasRegiaoDestino", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_CHAMADO_REGIAO_DESTINO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RegrasChamadosRegiaoDestino", Column = "RCD_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Chamados.RegrasChamadosRegiaoDestino> RegrasRegiaoDestino { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "RegrasCargaDescarga", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_CHAMADO_CARGA_DESCARGA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RegrasChamadosCargaDescarga", Column = "RCDE_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Chamados.RegrasChamadosCargaDescarga> RegrasCargaDescarga { get; set; }

        public virtual bool Equals(RegrasAnaliseChamados other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}