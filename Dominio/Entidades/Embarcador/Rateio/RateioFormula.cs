using System;

namespace Dominio.Entidades.Embarcador.Rateio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_RATEIO_FORMULA", EntityName = "RateioFormula", Name = "Dominio.Entidades.Embarcador.Rateio.RateioFormula", NameType = typeof(RateioFormula))]
    public class RateioFormula : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Rateio.RateioFormula>
    {
        public RateioFormula() { }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RFO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "RFO_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ParametroRateioFormula", Column = "RFO_PARAMETRO_RATEIO_FORMULA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula ParametroRateioFormula { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "RFO_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RatearPrimeiroIgualmenteEntrePedidos", Column = "RFO_RATEAR_PRIMEIRO_IGUALMENTE_ENTRE_PEDIDOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RatearPrimeiroIgualmenteEntrePedidos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RFO_ARREDONDAR_PARA_NUMERO_PAR_MAIS_PROXIMO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ArredondarParaNumeroParMaisProximo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RFO_RATEAR_EM_BLOCO_DE_EMISSAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RatearEmBlocoDeEmissao { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "PercentualAcrescentarPesoTotalCarga", Column = "RFO_PERCENTUAL_ACRESCENTAR_PESO_TOTAL_CARGA", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
		public virtual decimal PercentualAcrescentarPesoTotalCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RFO_EXIGIR_CONFERENCIA_MANUAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigirConferenciaManual { get; set; }

        public virtual string DescricaoAtivo
        {
            get
            {
                if (this.Ativo)
                    return "Ativo";
                else
                    return "Inativo";
            }
        }

        public virtual bool Equals(RateioFormula other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
