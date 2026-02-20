using System;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_POSICAO_ALERTA_SENSOR", EntityName = "PosicaoAlertaSensor", Name = "Dominio.Entidades.Embarcador.Logistica.PosicaoAlertaSensor", NameType = typeof(PosicaoAlertaSensor))]

    public class PosicaoAlertaSensor : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "PAS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual Int64 Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataVeiculo", Column = "PAS_DATA_VEICULO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCadastro", Column = "PAS_DATA_CADASTRO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataCadastro { get; set; }

        //[Obsolete("Removida, Utilizar DataCadastro, Latitude e Longitude")]
        //[NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Posicao", Column = "POS_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        //public virtual Dominio.Entidades.Embarcador.Logistica.Posicao Posicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Latitude", Column = "PAS_LATITUDE", TypeType = typeof(double), NotNull = false)]
        public virtual double Latitude { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Longitude", Column = "PAS_LONGITUDE", TypeType = typeof(double), NotNull = false)]
        public virtual double Longitude { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoSensor", Column = "PAS_TIPO_SENSOR", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSensor), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSensor TipoSensor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorSensor", Column = "PAS_VALOR_SENSOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValorSensor { get; set; }

    }
}
