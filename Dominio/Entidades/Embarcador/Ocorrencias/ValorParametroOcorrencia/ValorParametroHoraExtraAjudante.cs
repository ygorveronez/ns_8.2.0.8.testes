using System;

namespace Dominio.Entidades.Embarcador.Ocorrencias
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_VALOR_PARAMETRO_HORA_EXTRA_AJUDANTE", EntityName = "ValorParametroHoraExtraAjudante", Name = "Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroHoraExtraAjudante", NameType = typeof(ValorParametroHoraExtraAjudante))]
    public class ValorParametroHoraExtraAjudante : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PHA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ValorParametroHoraExtraOcorrencia", Column = "VPH_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroHoraExtraOcorrencia ValorParametroHoraExtraOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PHA_HORA_INICIAL", TypeType = typeof(TimeSpan), NotNull = true)]
        public virtual TimeSpan HoraInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PHA_HORA_FINAL", TypeType = typeof(TimeSpan), NotNull = true)]
        public virtual TimeSpan HoraFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PHA_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 12, NotNull = true)]
        public virtual decimal Valor { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Valor.ToString("n2") + " - " + this.HoraInicial.ToString(@"hh\:mm") + " - " + this.HoraFinal.ToString(@"hh\:mm");
            }
        }
    }
}
