namespace Dominio.Entidades.Embarcador.Chamados
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MOTIVO_RECUSA_CANCELAMENTO", EntityName = "MotivoRecusaCancelamento", Name = "Dominio.Entidades.Embarcador.Chamados.MotivoRecusaCancelamento", NameType = typeof(MotivoRecusaCancelamento))]
    public class MotivoRecusaCancelamento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MRC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MRC_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MRC_STATUS", TypeType = typeof(bool))]
        public virtual bool Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MRC_TIPO_MOTIVO_RECUSA_CANCELAMENTO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoMotivoRecusaCancelamento), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoMotivoRecusaCancelamento TipoMotivoRecusaCancelamento { get; set; }

        public virtual string DescricaoStatus
        {
            get
            {
                if (Status)
                    return "Ativo";
                else
                    return "Inativo";
            }
        }
    }
}
