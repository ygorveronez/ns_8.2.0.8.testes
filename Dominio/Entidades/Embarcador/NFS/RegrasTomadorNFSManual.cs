namespace Dominio.Entidades.Embarcador.NFS
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRA_NFS_MANUAL_TOMADOR", EntityName = "RegrasTomadorNFSManual", Name = "Dominio.Entidades.Embarcador.NFS.RegrasTomadorNFSManual", NameType = typeof(RegrasTomadorNFSManual))]
    public class RegrasTomadorNFSManual : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RMN_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegrasAutorizacaoNFSManual", Column = "RAN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RegrasAutorizacaoNFSManual RegrasAutorizacaoNFSManual { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Name = "Ordem", Column = "RMN_ORDEM", TypeType = typeof(int), NotNull = true)]
        public virtual int Ordem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Condicao", Column = "RMN_CONDICAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoNFSManual Condicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Juncao", Column = "RMN_JUNCAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoNFSManual Juncao { get; set; }


        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Tomador { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Tomador?.Descricao ?? string.Empty;
            }
        }

    }

}
