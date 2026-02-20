namespace Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_FUNCIONARIO_COMISSAO_VALOR", EntityName = "AlcadaComissao.AlcadaValor", Name = "Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.AlcadaValor", NameType = typeof(AlcadaValor))]
    public class AlcadaValor : Alcada.Alcada
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ACV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraFuncionarioComissao", Column = "RFC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RegraFuncionarioComissao RegraFuncionarioComissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ACV_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Valor { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Valor.ToString("n2");
            }
        }

        public virtual decimal PropriedadeAlcada
        {
            get
            {
                return this.Valor;
            }
            set
            {
                this.Valor = value;
            }
        }
    }
}
