using System;

namespace Dominio.Entidades.Embarcador.Ocorrencias
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_VALOR_PARAMETRO_ESTADIA_VEICULO", EntityName = "ValorParametroEstadiaVeiculo", Name = "Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroEstadiaVeiculo", NameType = typeof(ValorParametroEstadiaVeiculo))]
    public class ValorParametroEstadiaVeiculo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PSV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ValorParametroEstadiaOcorrencia", Column = "VPS_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroEstadiaOcorrencia ValorParametroEstadiaOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga ModeloVeicular { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PSV_HORA_INICIAL", TypeType = typeof(TimeSpan), NotNull = true)]
        public virtual TimeSpan HoraInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PSV_HORA_FINAL", TypeType = typeof(TimeSpan), NotNull = true)]
        public virtual TimeSpan HoraFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PSV_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 12, NotNull = true)]
        public virtual decimal Valor { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.ModeloVeicular?.Descricao ?? string.Empty;
            }
        }
    }
}
