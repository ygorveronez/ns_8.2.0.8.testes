namespace Dominio.Entidades.Embarcador.CTe
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONTROLE_GERACAO_MOVIMENTO_CTE_MANUAL", EntityName = "ControleGeracaoMovimentoCTeManual", Name = "Dominio.Entidades.Embarcador.CTe.ControleGeracaoMovimentoCTeManual", NameType = typeof(ControleGeracaoMovimentoCTeManual))]
    public class ControleGeracaoMovimentoCTeManual : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CGM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]

        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaCTe", Column = "CCT_CODIGO", NotNull = true, Unique = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.CargaCTe CargaCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGM_SITUACAO_GERAR", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string SituacaoCTeGerar { get; set; }
    }
}
