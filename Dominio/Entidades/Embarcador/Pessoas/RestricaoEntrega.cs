namespace Dominio.Entidades.Embarcador.Pessoas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_RESTRICAO_ENTREGA", EntityName = "RestricaoEntrega", Name = "Dominio.Entidades.Embarcador.Pessoas.RestricaoEntrega", NameType = typeof(RestricaoEntrega))]
    public class RestricaoEntrega : EntidadeBase
    {

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "REE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "REE_PRIMEIRA_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PrimeiraEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "REE_DESCRICAO", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "REE_EMAILS", TypeType = typeof(string), Length = 450, NotNull = false)]
        public virtual string Email { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "REE_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "REE_COR_VISUALIZACAO", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string CorVisualizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "REE_OBSERVACAO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "REE_ATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Ativo { get; set; }

        public virtual string DescricaoAtivo
        {
            get
            {
                if (this.Ativo)
                    return "Ativo";
                else if (this.Ativo)
                    return "Inativo";
                else
                    return "";
            }
        }

        public virtual bool Equals(ModalidadePessoas other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
