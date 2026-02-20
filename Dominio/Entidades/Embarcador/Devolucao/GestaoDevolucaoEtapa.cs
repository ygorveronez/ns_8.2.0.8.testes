using System;

namespace Dominio.Entidades.Embarcador.Devolucao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_GESTAO_DEVOLUCAO_ETAPA", EntityName = "GestaoDevolucaoEtapa", Name = "Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoEtapa", NameType = typeof(GestaoDevolucaoEtapa))]

    public class GestaoDevolucaoEtapa : EntidadeBase
    {
        #region Atributos
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "GDE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual long Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GestaoDevolucao", Column = "GDV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual GestaoDevolucao GestaoDevolucao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Etapa", Column = "GDE_ETAPA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaGestaoDevolucao), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaGestaoDevolucao Etapa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoEtapa", Column = "GDE_SITUACAO_ETAPA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEtapaGestaoDevolucao), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEtapaGestaoDevolucao SituacaoEtapa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ordem", Column = "GDE_ORDEM", TypeType = typeof(int), NotNull = true)]
        public virtual int Ordem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicio", Column = "GDE_DATA_INICIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFim", Column = "GDE_DATA_FIM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFim { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "GDE_OBSERVACAO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Observacao { get; set; }
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
