namespace Dominio.Entidades.Embarcador.Canhotos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONTROLE_NOTIFICACAO_CANHOTO", EntityName = "ControleNotificacao", Name = "Dominio.Entidades.Embarcador.Canhotos.ControleNotificacao", NameType = typeof(ControleNotificacao))]
    public class ControleNotificacao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CNC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Transportador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Filiais.Filial Filial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CNC_FINALIZADO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Finalizado { get; set; }
    }
}
