using System;
using System.Collections.Generic;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Configuration;

namespace Dominio.Entidades.Embarcador.Financeiro
{
    /// <summary>
    /// Entidade que guarda a conciliação que o transportador cria em seu portal.
    /// No final, ela gerará uma anuência.
    /// </summary>
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONCILIACAO_TRANSPORTADOR", EntityName = "ConciliacaoTransportador", Name = "Dominio.Entidades.Embarcador.Financeiro.ConciliacaoTransportador", NameType = typeof(ConciliacaoTransportador))]
    public class ConciliacaoTransportador : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "COT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoConciliacaoTransportador", Column = "COT_SITUACAO", TypeType = typeof(SituacaoConciliacaoTransportador), NotNull = true)]
        public virtual SituacaoConciliacaoTransportador SituacaoConciliacaoTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicial", Column = "COT_DATA_INICIAL", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFinal", Column = "COT_DATA_FINAL", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataFinal { get; set; }

        /// <summary>
        /// Se nas configurações gerais o tipo do cnpj for Raiz, esse campo guarda a raiz do CNPJ que vai juntar todas os Transportadores
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "RaizCnpj", Column = "COT_RAIZ_CNPJ", TypeType = typeof(string), Length = 40, NotNull = false)]
        public virtual string RaizCnpj { get; set; }

        /// <summary>
        /// O momento em que está disponível no sistema a assinatura da anuência. É a data final mais a quantiade de dias de constestação
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAnuenciaDisponivel", Column = "COT_DATA_ANUENCIA_DISPONIVEL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataAnuenciaDisponivel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAssinaturaAnuencia", Column = "COT_DATA_ASSINATURA_ANUENCIA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAssinaturaAnuencia { get; set; }

        /// <summary>
        /// Salva a periodicidade da conciliação. É o mesmo valor que está nas configurações globais (mas lá ele pode mudar no futuro)
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "Periodicidade", Column = "COT_PERIODICIDADE", TypeType = typeof(PeriodicidadeConciliacaoTransportador), NotNull = true)]
        public virtual PeriodicidadeConciliacaoTransportador Periodicidade { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Transportadores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONCILIACAO_TRANSPORTADOR_EMPRESA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "COT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Empresa", Column = "EMP_CODIGO")]
        public virtual IList<Empresa> Transportadores { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Table = "T_CTE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "COT_CODIGO")]
        [NHibernate.Mapping.Attributes.OneToMany(2, Class = "ConhecimentoDeTransporteEletronico")]
        public virtual IList<Dominio.Entidades.ConhecimentoDeTransporteEletronico> CTes { get; set; }

        public virtual string Descricao
        {
            get { return Codigo.ToString(); }
        }

        public virtual string PeriodoFormatado
        {
            get { return DataInicial.ToString("dd/MM/yyyy") + " até " + DataFinal.ToString("dd/MM/yyyy"); }
        }

        public virtual Empresa PrimeiroTransportador
        {
            get
            {
                return Transportadores.Count > 0 ? Transportadores[0] : null;
            }
        }

        public virtual bool Assinado
        {
            get
            {
                return DataAssinaturaAnuencia.HasValue && DataAssinaturaAnuencia.Value != DateTime.MinValue;
            }
        }

    }
}
