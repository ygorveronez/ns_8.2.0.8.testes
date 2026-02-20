namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_MDFE_MANUAL_PERCENTUAL_MOTORISTA", EntityName = "CargaMDFeManualPercentualMotorista", Name = "Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualPercentualMotorista", NameType = typeof(CargaMDFeManualPercentualMotorista))]
    public class CargaMDFeManualPercentualMotorista : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PMM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Motorista { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaMDFeManual", Column = "CMM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual CargaMDFeManual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PMM_PERCENTUAL_EXECUCAO", TypeType = typeof(decimal), Scale = 2, Precision = 5, NotNull = false)]
        public virtual decimal? PercentualExecucao { get; set; }

        public virtual Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualPercentualMotorista Clonar()
        {
            return (Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualPercentualMotorista)this.MemberwiseClone();
        }
    }
}
