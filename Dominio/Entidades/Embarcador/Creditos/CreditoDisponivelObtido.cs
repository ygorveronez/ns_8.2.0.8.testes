using System;

namespace Dominio.Entidades.Embarcador.Creditos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CREDITO_DISPONIVEL_OBTIDO", EntityName = "CreditoDisponivelObtido", Name = "Dominio.Entidades.Embarcador.Creditos.CreditoDisponivelObtido", NameType = typeof(CreditoDisponivelObtido))]
    public class CreditoDisponivelObtido : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Creditos.CreditoDisponivelObtido>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CDO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CreditoDisponivel", Column = "CDI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CreditoDisponivel CreditoDisponivel { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataObtencao", Column = "CDO_DATA_OBTENCAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataObtencao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorObtido", Column = "CDO_VALOR_OBTENCAO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorObtido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorProvisinado", Column = "CDO_VALOR_PROVISIONADO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorProvisinado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoCreditoObtido", Column = "CDO_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoCreditoObtido), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoCreditoObtido SituacaoCreditoObtido { get; set; }

        public virtual bool Equals(CreditoDisponivelObtido other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
