using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Chamados
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRAS_ATENDIMENTO_CHAMADOS", EntityName = "RegrasAtendimentoChamados", Name = "Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoChamados", NameType = typeof(RegrasAtendimentoChamados))]
    public class RegrasAtendimentoChamados : EntidadeBase
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

        [NHibernate.Mapping.Attributes.Property(0, Column = "RAC_FILIAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RegraPorFilial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RAC_CANAL_VENDA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RegraPorCanalVenda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RAC_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RegraPorTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RAC_TIPO_OPERACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RegraPorTipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RAC_ESTADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RegraPorEstado { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Aprovadores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRAS_ATENDIMENTO_FUNCIONARIOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Usuario", Column = "FUN_CODIGO")]
        public virtual ICollection<Usuario> Aprovadores { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "RegrasFilial", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_ATENDIMENTO_FILIAL")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RegrasAtendimentoFilial", Column = "RCF_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoFilial> RegrasFilial { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "RegrasCanalVenda", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_ATENDIMENTO_CANAL_VENDA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RegrasAtendimentoCanalVenda", Column = "RCF_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoCanalVenda> RegrasCanalVenda { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "RegrasEstado", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_ATENDIMENTO_ESTADO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RegrasAtendimentoEstado", Column = "RCF_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoEstado> RegrasEstado { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "RegrasTipoOperacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_ATENDIMENTO_TIPO_OPERACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RegrasAtendimentoTipoOperacao", Column = "RCF_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoTipoOperacao> RegrasTipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "RegrasTransportador", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_ATENDIMENTO_TRANSPORTADOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RegrasAtendimentoTransportador", Column = "RCF_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoTransportador> RegrasTransportador { get; set; }

        public virtual bool Equals(RegrasAtendimentoChamados other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}