using System;

namespace Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_GESTAO_DADOS_COLETA_DADOS_NFE", EntityName = "GestaoDadosColetaDadosNFe", Name = "Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaDadosNFe", NameType = typeof(GestaoDadosColetaDadosNFe))]
    public class GestaoDadosColetaDadosNFe : EntidadeBase
    {
        #region Propriedades

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "GDN_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GestaoDadosColeta", Column = "GDC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual GestaoDadosColeta GestaoDadosColeta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GDC_GUID_ARQUIVO", TypeType = typeof(string), Length = 40, NotNull = false)]
        public virtual string GuidArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OrigemFoto", Column = "GDN_ORIGEM_FOTO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemFotoDadosNFEGestaoDadosColeta), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemFotoDadosNFEGestaoDadosColeta OrigemFoto { get; set; }

        #endregion Propriedades

        #region Propriedades da Aprovação

        [NHibernate.Mapping.Attributes.Property(0, Name = "Chave", Column = "GDN_CHAVE", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Chave { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "GDN_NUMERO", TypeType = typeof(int), NotNull = false)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Serie", Column = "GDN_SERIE", TypeType = typeof(string), Length = 3, NotNull = false)]
        public virtual string Serie { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEmissao", Column = "GDN_DATA_EMISSAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Peso", Column = "GDN_PESO", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal Peso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Volumes", Column = "GDN_VOLUMES", TypeType = typeof(int), NotNull = false)]
        public virtual int Volumes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "GDN_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_REMETENTE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Emitente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_DESTINATARIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Destinatario { get; set; }

        #endregion Propriedades da Aprovação
    }
}
