using System;

namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_AJUSTE_TABELA_FRETE_PROCESSAMENTO_VALORES", EntityName = "AjusteTabelaFreteProcessamentoValores", Name = "Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete", NameType = typeof(AjusteTabelaFreteProcessamentoValores))]
    public class AjusteTabelaFreteProcessamentoValores : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PVA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PVA_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "AjusteTabelaFrete", Column = "TFA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual AjusteTabelaFrete AjusteTabelaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PVA_ITENS_AJUSTE", Type = "StringClob", NotNull = false)]
        public virtual string ItensAjuste { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }
    }
}