using System;
using System.Collections.Generic;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Integracao
{
    public abstract class Integracao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoIntegracao", Column = "TPI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.TipoIntegracao TipoIntegracao { get; set; }

        [Obsolete("Será removido. Utilizar o TipoIntegracao.")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "SistemaIntegracao", Column = "INT_SISTEMA_INTEGRACAO", TypeType = typeof(SistemaIntegracao), NotNull = true)]
        public virtual SistemaIntegracao SistemaIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataIntegracao", Column = "INT_DATA_INTEGRACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoIntegracao", Column = "INT_SITUACAO_INTEGRACAO", TypeType = typeof(SituacaoIntegracao), NotNull = true)]
        public virtual SituacaoIntegracao SituacaoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroTentativas", Column = "INT_NUMERO_TENTATIVAS", TypeType = typeof(int), NotNull = true)]
        public virtual int NumeroTentativas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ProblemaIntegracao", Column = "INT_PROBLEMA_INTEGRACAO", Type = "StringClob", NotNull = true)]
        public virtual string ProblemaIntegracao { get; set; }

        /// <summary>
        /// armazena temporariamente os codigos do CT-es que estão contidos nessa integração para vínculo posterior com o arquivo.
        /// </summary>
        public virtual List<int> CodigosCTes { get; set; }

        public virtual string DescricaoSituacaoIntegracao
        {
            get { return SituacaoIntegracao.ObterDescricao(); }
        }
    }
}
