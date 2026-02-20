using System;

namespace Dominio.Entidades.Embarcador.Ocorrencias
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_VALOR_PARAMETRO_ESCOLTA_OCORRENCIA", EntityName = "ValorParametroEscoltaOcorrencia", Name = "Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroEscoltaOcorrencia", NameType = typeof(ValorParametroEscoltaOcorrencia))]
    public class ValorParametroEscoltaOcorrencia : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "VPE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ValorParametroOcorrencia", Column = "VPO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroOcorrencia ValorParametroOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeOcorrenciaDeCTe", Column = "OCO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoDeOcorrenciaDeCTe TipoOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ComponenteFrete", Column = "CFR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.ComponenteFrete ComponenteFrete { get; set; }

        //[NHibernate.Mapping.Attributes.Bag(0, Name = "Valores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_VALOR_PARAMETRO_ESCOLTA_VALOR")]
        //[NHibernate.Mapping.Attributes.Key(1, Column = "VPE_CODIGO")]
        //[NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ValorParametroEscoltaValor", Column = "PEV_CODIGO")]
        //public virtual IList<Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroEscoltaValor> Valores { get; set; }

        //[NHibernate.Mapping.Attributes.Property(0, Column = "VPE_HORA_CONTRATADO", TypeType = typeof(int), NotNull = false)]
        //public virtual int HoraContratado { get; set; false
        [NHibernate.Mapping.Attributes.Property(0, Column = "VPE_HORA_CONTRATADO", TypeType = typeof(TimeSpan), NotNull = true)]
        public virtual TimeSpan? HoraContratado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "VPE_VALOR_HORA_EXCEDENTE", TypeType = typeof(decimal), Scale = 2, Precision = 12, NotNull = false)]
        public virtual decimal ValorHoraExcedente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "VPE_KM_CONTRATADO", TypeType = typeof(int), NotNull = false)]
        public virtual int KmContratado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "VPE_VALOR_KM_EXCEDENTE", TypeType = typeof(decimal), Scale = 2, Precision = 12, NotNull = false)]
        public virtual decimal ValorKmExcedente { get; set; }


        public virtual string Descricao
        {
            get
            {
                return "Escolta - " + (this.ValorParametroOcorrencia?.Descricao ?? string.Empty);
            }
        }
    }
}
