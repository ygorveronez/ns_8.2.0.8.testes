using System;

namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TABELA_FRETE_CLIENTE_ALTERACAO", EntityName = "TabelaFreteClienteAlteracao", Name = "Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteAlteracao", NameType = typeof(TabelaFreteClienteAlteracao))]
    public class TabelaFreteClienteAlteracao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TCA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TCA_DATA_ALTERACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataAlteracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaFreteAlteracao", Column = "TFA_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TabelaFreteAlteracao TabelaFreteAlteracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaFreteCliente", Column = "TFC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TabelaFreteCliente TabelaFreteCliente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }
    }
}
