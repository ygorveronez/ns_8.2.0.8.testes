using Dominio.Entidades.Embarcador.Cargas;
using Dominio.Interfaces.Embarcador.Integracao;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Documentos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_DOCUMENTO_DESTINADO_INTEGRACAO", EntityName = "DocumentoDestinadoIntegracao", Name = "Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoIntegracao", NameType = typeof(DocumentoDestinadoIntegracao))]
    public class DocumentoDestinadoIntegracao: Integracao.Integracao, IIntegracaoComArquivo<CargaCTeIntegracaoArquivo>, IEquatable<DocumentoDestinadoIntegracao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "DD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "DocumentoDestinadoEmpresa", Column = "DDE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual DocumentoDestinadoEmpresa DocumentoDestinadoEmpresa { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ArquivosTransacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_DOCUMENTO_DESTINADO_INTEGRACAO_ARQUIVO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "DD_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaCTeIntegracaoArquivo", Column = "CCA_CODIGO")]
        public virtual ICollection<CargaCTeIntegracaoArquivo> ArquivosTransacao { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Column = "DDI_EXISTE_XML_COMPLETO_DOCUMENTO_DESTINADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? ExisteXmlCompletoDocumentoDestinado { get; set; }

        public virtual bool Equals(DocumentoDestinadoIntegracao other)
        {
            return other.Codigo == this.Codigo;
        }
    }
}
