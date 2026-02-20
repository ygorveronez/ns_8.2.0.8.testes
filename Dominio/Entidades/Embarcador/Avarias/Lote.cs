using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dominio.Entidades.Embarcador.Avarias
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_LOTE_AVARIA", EntityName = "Lote", Name = "Dominio.Entidades.Embarcador.Avarias.Lote", NameType = typeof(Lote))]
    public class Lote : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "LAV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "LAV_NUMERO", TypeType = typeof(int), NotNull = true)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "LAV_DATA_GERACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataGeracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "LAV_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoLote), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoLote Situacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Criador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Transportador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MotivoAvaria", Column = "MAV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual MotivoAvaria MotivoAvaria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "LAV_ETAPA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.EtapaLote), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.EtapaLote Etapa { get; set; }

        //[NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TempoEtapaLote", Column = "TEL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        //public virtual Dominio.Entidades.Embarcador.Avarias.TempoEtapaLote TempoEtapa { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Avarias", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_SOLICITACAO_AVARIA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "LAV_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "SolicitacaoAvaria", Column = "SAV_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria> Avarias { get; set; }

        #region Propriedades Virtuais

        public virtual string Descricao
        {
            get
            {
                return this.Numero.ToString();
            }
        }

        public virtual decimal ValorLote
        {
            get
            {
                return this.ValorAvarias - this.ValorDescontos;
            }
        }

        public virtual decimal ValorDescontos
        {
            get
            {
                return (from obj in Avarias select obj).Sum(o => o.ValorDesconto);
            }
        }

        public virtual decimal ValorAvarias
        {
            get
            {
                return (from obj in Avarias select obj).Sum(o => o.ValorAvaria);
            }
        }

        public virtual string DescricaoSituacao
        {
            get { return Situacao.ObterDescricao(); }
        }

        public virtual string DescricaoEtapa
        {
            get { return Etapa.ObterDescricao(); }
        }

        public virtual bool Equals(Lote other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        #endregion
    }
}
