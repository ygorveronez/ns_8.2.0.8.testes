namespace Dominio.Entidades.Embarcador.Avarias
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRA_AVARIA_MOTIVO", EntityName = "RegrasMotivoAvaria", Name = "Dominio.Entidades.Embarcador.Avarias.RegrasMotivoAvaria", NameType = typeof(RegrasMotivoAvaria))]
    public class RegrasMotivoAvaria : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RMA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegrasAutorizacaoAvaria", Column = "RAA_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RegrasAutorizacaoAvaria RegrasAutorizacaoAvaria { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Name = "Ordem", Column = "RMA_ORDEM", TypeType = typeof(int), NotNull = true)]
        public virtual int Ordem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Condicao", Column = "RMA_CONDICAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria Condicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Juncao", Column = "RMA_JUNCAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria Juncao { get; set; }


        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MotivoAvaria", Column = "MAV_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual MotivoAvaria MotivoAvaria { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.MotivoAvaria?.Descricao ?? string.Empty;
            }
        }
    }

}