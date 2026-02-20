using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_MOTIVO_PARADA_CENTRO", EntityName = "MotivoParadaCentro", Name = "Dominio.Entidades.Embarcador.Logistica.MotivoParadaCentro", NameType = typeof(MotivoParadaCentro))]
    public class MotivoParadaCentro : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MPC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroCarregamento", Column = "CEC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.CentroCarregamento CentroCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MPC_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MPC_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MPC_DATA_INICIO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataInicio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MPC_DATA_FIM", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataFim { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MPC_QUANTIDADE_PARADA", TypeType = typeof(int), NotNull = true)]
        public virtual int QuantidadeParada { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "TiposOperacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_MOTIVO_PARADA_CENTRO_TIPO_OPERACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "MPC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "MotivoParadaCentroTipoOperacao", Column = "MPT_CODIGO")]
        public virtual ICollection<MotivoParadaCentroTipoOperacao> TiposOperacao { get; set; }


        public virtual string DescricaoAtivo => Ativo ? Localization.Resources.Gerais.Geral.Ativo : Localization.Resources.Gerais.Geral.Inativo;

        public virtual string DescricaoPeriodo => $"{DataInicio.ToDateTimeString()} - {DataFim.ToDateTimeString()}";
    }
}
