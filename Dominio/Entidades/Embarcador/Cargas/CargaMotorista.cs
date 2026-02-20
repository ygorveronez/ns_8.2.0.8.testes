namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_MOTORISTA", EntityName = "CargaMotorista", Name = "Dominio.Entidades.Embarcador.Cargas.CargaMotorista", NameType = typeof(CargaMotorista))]
    public class CargaMotorista : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TCM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "CAR_MOTORISTA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Motorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NotificacaoAtualizacaoCargaPendente", Column = "CMR_NOTIFICACAO_ATUALIZACAO_CARGA_PENDENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NotificacaoAtualizacaoCargaPendente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRM_PERCENTUAL_EXECUCAO", TypeType = typeof(decimal), Scale = 2, Precision = 5, NotNull = false)]
        public virtual decimal? PercentualExecucao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "InseridoPelaCargaMDFeManual", Column = "CRM_CARGA_MDFE_MANUAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool InseridoPelaCargaMDFeManual { get; set; }

        public virtual string Descricao
        {
            get
            {
                return (this.Carga?.Descricao ?? string.Empty) + " - " + (this.Motorista?.Descricao ?? string.Empty);
            }
        }
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaMotorista Clonar()
        {
            return (Dominio.Entidades.Embarcador.Cargas.CargaMotorista)this.MemberwiseClone();
        }
    }
}
