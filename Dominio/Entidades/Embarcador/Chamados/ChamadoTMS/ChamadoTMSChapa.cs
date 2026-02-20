namespace Dominio.Entidades.Embarcador.Chamados
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CHAMADO_TMS_CHAPA", EntityName = "ChamadoTMSChapa", Name = "Dominio.Entidades.Embarcador.Chamados.ChamadoTMSChapa", NameType = typeof(ChamadoTMSChapa))]
    public class ChamadoTMSChapa : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CHP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ChamadoTMS", Column = "CHT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ChamadoTMS Chamado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Nome", Column = "CHP_NOME", TypeType = typeof(string), Length = 60, NotNull = false)]
        public virtual string Nome { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CPF", Column = "CHP_CPF", TypeType = typeof(string), Length = 11, NotNull = false)]
        public virtual string CPF { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Telefone", Column = "CHP_TELEFONE", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string Telefone { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Nome + " - " + (this.Chamado?.Descricao ?? string.Empty);
            }
        }
    }
}
