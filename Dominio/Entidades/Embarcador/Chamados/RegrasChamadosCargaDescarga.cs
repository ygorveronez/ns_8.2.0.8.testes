using Dominio.Entidades.Embarcador.Bidding;

namespace Dominio.Entidades.Embarcador.Chamados
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRA_CHAMADO_CARGA_DESCARGA", EntityName = "RegrasChamadosCargaDescarga", Name = "Dominio.Entidades.Embarcador.Chamados.RegrasChamadosCargaDescarga", NameType = typeof(RegrasChamadosCargaDescarga))]
    public class RegrasChamadosCargaDescarga : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RCDE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegrasAnaliseChamados", Column = "RAC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Chamados.RegrasAnaliseChamados RegrasAnaliseChamados { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ordem", Column = "RCDE_ORDEM", TypeType = typeof(int), NotNull = true)]
        public virtual int Ordem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Condicao", Column = "RCDE_CONDICAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria Condicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Juncao", Column = "RCDE_JUNCAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria Juncao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValidarValorCarga", Column = "RCDE_VALIDAR_VALOR_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidarValorCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValidarValorDescarga", Column = "RCDE_VALIDAR_VALOR_DESCARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidarValorDescarga { get; set; }
    }
}