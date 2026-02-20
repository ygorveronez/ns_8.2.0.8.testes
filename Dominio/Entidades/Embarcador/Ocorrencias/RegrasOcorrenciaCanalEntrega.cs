namespace Dominio.Entidades.Embarcador.Ocorrencias
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRA_OCORRENCIA_CANAL_ENTREGA", EntityName = "RegrasOcorrenciaCanalEntrega", Name = "Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaCanalEntrega", NameType = typeof(RegrasOcorrenciaCanalEntrega))]
    public class RegrasOcorrenciaCanalEntrega : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RFX_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegrasAutorizacaoOcorrencia", Column = "RAO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RegrasAutorizacaoOcorrencia RegrasAutorizacaoOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ordem", Column = "RFX_ORDEM", TypeType = typeof(int), NotNull = true)]
        public virtual int Ordem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Condicao", Column = "RFX_CONDICAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia Condicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Juncao", Column = "RFX_JUNCAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia Juncao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CanalEntrega", Column = "CNE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.CanalEntrega CanalEntrega { get; set; }

    }

}