namespace Dominio.Entidades.Embarcador.Escrituracao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MOTIVO_CANCELAMENTO_PAGAMENTO", EntityName = "MotivoCancelamentoPagamento", Name = "Dominio.Entidades.Embarcador.Escrituracao.MotivoCancelamentoPagamento", NameType = typeof(MotivoCancelamentoPagamento))]
    public class MotivoCancelamentoPagamento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MCP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCP_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCP_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCP_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacao { get; set; }


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

        public virtual bool Equals(MotivoCancelamentoPagamento other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
