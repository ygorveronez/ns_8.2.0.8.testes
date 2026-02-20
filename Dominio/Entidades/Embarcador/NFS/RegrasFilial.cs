namespace Dominio.Entidades.Embarcador.NFS
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRA_NFS_MANUAL_FILIAL", EntityName = "RegrasFilialNFSManual", Name = "Dominio.Entidades.Embarcador.NFS.RegrasFilialNFSManual", NameType = typeof(RegrasFilialNFSManual))]
    public class RegrasFilialNFSManual : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RFN_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegrasAutorizacaoNFSManual", Column = "RAN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RegrasAutorizacaoNFSManual RegrasAutorizacaoNFSManual { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Name = "Ordem", Column = "RFN_ORDEM", TypeType = typeof(int), NotNull = true)]
        public virtual int Ordem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Condicao", Column = "RFN_CONDICAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoNFSManual), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoNFSManual Condicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Juncao", Column = "RFN_JUNCAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoNFSManual), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoNFSManual Juncao { get; set; }


        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Filiais.Filial Filial { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Filial?.Descricao ?? string.Empty;
            }
        }

    }

}