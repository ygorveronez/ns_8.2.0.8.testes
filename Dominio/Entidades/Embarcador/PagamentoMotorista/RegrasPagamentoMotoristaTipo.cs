namespace Dominio.Entidades.Embarcador.PagamentoMotorista
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRA_PAGAMENTO_TIPO", EntityName = "RegrasPagamentoMotoristaTipo", Name = "Dominio.Entidades.Embarcador.Embarcador.RegrasPagamentoMotoristaTipo", NameType = typeof(RegrasPagamentoMotoristaTipo))]
    public class RegrasPagamentoMotoristaTipo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RPT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegrasPagamentoMotorista", Column = "RPM_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RegrasPagamentoMotorista RegrasPagamentoMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ordem", Column = "RPT_ORDEM", TypeType = typeof(int), NotNull = false)]
        public virtual int Ordem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Condicao", Column = "RPT_CONDICAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia Condicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Juncao", Column = "RPT_JUNCAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia Juncao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PagamentoMotoristaTipo", Column = "PMT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PagamentoMotoristaTipo PagamentoMotoristaTipo { get; set; }


    }

}
