namespace Dominio.Entidades.Embarcador.Chamados
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRA_CHAMADO_REGIAO_DESTINO", EntityName = "RegrasChamadosRegiaoDestino", Name = "Dominio.Entidades.Embarcador.Chamados.RegrasChamadosRegiaoDestino", NameType = typeof(RegrasChamadosRegiaoDestino))]
    public class RegrasChamadosRegiaoDestino : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RCD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegrasAnaliseChamados", Column = "RAC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Chamados.RegrasAnaliseChamados RegrasAnaliseChamados { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Regiao", Column = "REG_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Localidades.Regiao Regiao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ordem", Column = "RCD_ORDEM", TypeType = typeof(int), NotNull = true)]
        public virtual int Ordem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Condicao", Column = "RCD_CONDICAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria Condicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Juncao", Column = "RCD_JUNCAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria Juncao { get; set; }
        
        public virtual string Descricao
        {
            get
            {
                return this.Regiao.Descricao;
            }
        }
    }
}