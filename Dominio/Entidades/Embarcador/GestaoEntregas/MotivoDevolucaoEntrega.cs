namespace Dominio.Entidades.Embarcador.GestaoEntregas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MOTIVO_DEVOLUCAO_ENTREGA", EntityName = "MotivoDevolucaoEntrega", Name = "Dominio.Entidades.Embarcador.GestaoEntregas.MotivoDevolucaoEntrega", NameType = typeof(MotivoDevolucaoEntrega))]
    public class MotivoDevolucaoEntrega : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MDE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "MDE_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "MDE_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObrigarFoto", Column = "MDE_OBRIGAR_FOTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ObrigarFoto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MotivoChamado", Column = "MCH_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Chamados.MotivoChamado MotivoChamado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "MDE_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EntregaParcialSuperAppId", Column = "MDE_ENTREGA_PARCIAL_SUPER_APP_ID", TypeType = typeof(string), Length = 24, NotNull = false)]
        public virtual string EntregaParcialSuperAppId { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoEntregaSuperAppId", Column = "MDE_NAO_ENTREGA_SUPER_APP_ID", TypeType = typeof(string), Length = 24, NotNull = false)]
        public virtual string NaoEntregaSuperAppId { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ChecklistSuperApp", Column = "CSA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.SuperApp.ChecklistSuperApp ChecklistSuperApp { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeOcorrenciaDeCTe", Column = "OCO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.TipoDeOcorrenciaDeCTe TipoOcorrencia { get; set; }

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

        public virtual bool Equals(MotivoDevolucaoEntrega other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
