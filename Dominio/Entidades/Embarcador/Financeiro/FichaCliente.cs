namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CLIENTE_FICHA", EntityName = "FichaCliente", Name = "Dominio.Entidades.Embarcador.Financeiro.FichaCliente", NameType = typeof(FichaCliente))]
    public class FichaCliente : EntidadeBase, Interfaces.Embarcador.Entidade.IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CFI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SaldoAtual", Column = "CFI_SALDO_ATUAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal SaldoAtual { get; set; }

        public virtual string Descricao
        { 
            get
            {
                return this.Cliente.Descricao;
            }
        }

    }
}
