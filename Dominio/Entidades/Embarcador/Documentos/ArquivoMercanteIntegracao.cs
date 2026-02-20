using Dominio.Interfaces.Embarcador.Integracao;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Documentos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ARQUIVO_MERCANTE_INTEGRACAO", EntityName = "ArquivoMercanteIntegracao", Name = "Dominio.Entidades.Embarcador.Documentos.ArquivoMercanteIntegracao", NameType = typeof(ArquivoMercanteIntegracao))]
    public class ArquivoMercanteIntegracao : Integracao.Integracao, IIntegracaoComArquivo<ArquivoMercanteIntegracaoArquivo>, IEquatable<ArquivoMercanteIntegracao>
    {
        public ArquivoMercanteIntegracao() { }
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "AMI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.ConhecimentoDeTransporteEletronico ConhecimentoDeTransporteEletronico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Protocolo", Column = "AMI_PROTOCOLO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Protocolo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AMI_MENSAGEM", TypeType = typeof(string), Length = 10000, NotNull = false)]
        public virtual string Mensagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescricaoTipo", Column = "AMI_DESCRICAO_TIPO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string DescricaoTipo { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ArquivosTransacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ARQUIVO_MERCANTE_INTEGRACAO_ARQUIVO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "AMI_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaCTeIntegracaoArquivo", Column = "AMA_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Documentos.ArquivoMercanteIntegracaoArquivo> ArquivosTransacao { get; set; }

        public virtual bool Equals(ArquivoMercanteIntegracao other)
        {
            return (other.Codigo == this.Codigo);
        }
    }
}
