using System;

namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TABELA_FRETE_AJUSTE_LOG", EntityName = "AjusteTabelaFreteLog", Name = "Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteLog", NameType = typeof(AjusteTabelaFreteLog))]
    public class AjusteTabelaFreteLog : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TAL_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "AjusteTabelaFrete", Column = "TFA_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual AjusteTabelaFrete AjusteTabelaFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "TAL_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "TAL_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoAjusteTabelaFrete), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoAjusteTabelaFrete Situacao { get; set; }
    }
}
