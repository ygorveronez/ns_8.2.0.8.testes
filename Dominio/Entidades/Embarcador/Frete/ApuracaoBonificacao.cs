using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores.Frete;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_APURACAO_BONIFICACAO", EntityName = "ApuracaoBonificacao", Name = "Dominio.Entidades.Embarcador.Frete.ApuracaoBonificacao", NameType = typeof(ApuracaoBonificacao))]
    public class ApuracaoBonificacao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "APB_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "APB_ANO", TypeType = typeof(int), NotNull = true)]
        public virtual int Ano { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "APB_MES", TypeType = typeof(Mes), NotNull = true)]
        public virtual Mes Mes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "APB_NUMERO", TypeType = typeof(int), NotNull = true)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "APB_SITUACAO", TypeType = typeof(SituacaoApuracaoBonificacao), NotNull = true)]
        public virtual SituacaoApuracaoBonificacao Situacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCriacao", Column = "APB_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataCriacao { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "RegrasApuracao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_APURACAO_BONIFICACAO_REGRA_APURACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "APB_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "BonificacaoTransportador", Column = "BNT_CODIGO")]
        public virtual ICollection<BonificacaoTransportador> RegrasApuracao { get; set; }
    }
}