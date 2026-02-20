namespace Dominio.Entidades.Embarcador.Seguros
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_AVERBACAO_ATM", EntityName = "AverbacaoATM", Name = "Dominio.Entidades.Embarcador.Seguros.AverbacaoATM", NameType = typeof(AverbacaoATM))]
    public class AverbacaoATM : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CAA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ApoliceSeguro", Column = "APS_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro ApoliceSeguro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAA_CODIGO_ATM", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string CodigoATM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAA_USUARIO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAA_SENHA", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Senha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAA_AVERBA_COMO_EMBARCADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AverbaComoEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CAA_AVERBAR_NFE_QUANDO_CARGA_POSSUIR_NFS_MANUAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AverbarNFeQuandoCargaPossuirNFSManual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "VersaoLayoutATMOutrosDocumentos", Column = "CAA_VERSAO_LAYOUT_ATM_OUTROS_DOCUMENTOS", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.EnumVersaoLayoutATM), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.EnumVersaoLayoutATM? VersaoLayoutATMOutrosDocumentos { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.ApoliceSeguro.Descricao;
            }
        }
    }
}
