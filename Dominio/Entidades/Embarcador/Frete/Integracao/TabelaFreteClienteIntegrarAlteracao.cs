using System;

namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TABELA_FRETE_CLIENTE_INTEGRAR_ALTERACAO", EntityName = "TabelaFreteClienteIntegrarAlteracao", Name = "Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteIntegrarAlteracao", NameType = typeof(TabelaFreteClienteIntegrarAlteracao))]
    public class TabelaFreteClienteIntegrarAlteracao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TCI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TCI_DATA_ALTERACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataAlteracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaFreteIntegrarAlteracao", Column = "TIA_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TabelaFreteIntegrarAlteracao TabelaFreteIntegrarAlteracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaFreteCliente", Column = "TFC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TabelaFreteCliente TabelaFreteCliente { get; set; }
    }
}
