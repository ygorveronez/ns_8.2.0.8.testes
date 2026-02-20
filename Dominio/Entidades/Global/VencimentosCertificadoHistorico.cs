using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_VENCIMENTO_CERTIFICADO_HISTORICO", EntityName = "VencimentoCertificadoHistorico", Name = "Dominio.Entidades.VencimentoCertificadoHistorico", NameType = typeof(VencimentoCertificadoHistorico))]
    public class VencimentoCertificadoHistorico : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "VCH_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "VencimentoCertificado", Column = "VC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual VencimentoCertificado VencimentoCertificado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "VCH_DATA_LANCAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataHora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "VCH_DETALHES", Type = "StringClob", NotNull = false)]
        public virtual string Detalhes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "VCH_TIPO", TypeType = typeof(Enumeradores.TipoHistorico), NotNull = false)]
        public virtual Enumeradores.TipoHistorico Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "VCH_STATUS_VENDA", TypeType = typeof(Enumeradores.StatusVendaCertificado), NotNull = false)]
        public virtual Enumeradores.StatusVendaCertificado? StatusVenda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NivelSatisfacao", Column = "VCH_SATISFACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.NivelSatisfacao), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.NivelSatisfacao? NivelSatisfacao { get; set; }
    }
}
