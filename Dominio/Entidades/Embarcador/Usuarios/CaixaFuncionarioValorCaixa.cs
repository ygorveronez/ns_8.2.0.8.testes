using System;

namespace Dominio.Entidades.Embarcador.Usuarios
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CAIXA_FUNCIONARIO_VALOR_CAIXA", EntityName = "CaixaFuncionarioValorCaixa", Name = "Dominio.Entidades.Embarcador.Usuarios.CaixaFuncionarioValorCaixa", NameType = typeof(CaixaFuncionarioValorCaixa))]
    public class CaixaFuncionarioValorCaixa : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CFV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CaixaFuncionario", Column = "CAF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CaixaFuncionario CaixaFuncionario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "CFV_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "CFV_DESCRICAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataLancamento", Column = "CFV_DATA_LANCAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataLancamento { get; set; }
    }
}
