using System;

namespace Dominio.Entidades.Embarcador.Escrituracao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_LOTE_ESCRITURACAO_EDI_INTEGRACAO", EntityName = "LoteEscrituracaoEDIIntegracao", Name = "Dominio.Entidades.Embarcador.NFS.LoteEscrituracaoEDIIntegracao", NameType = typeof(LoteEscrituracaoEDIIntegracao))]
    public class LoteEscrituracaoEDIIntegracao : Integracao.Integracao, IEquatable<LoteEscrituracaoEDIIntegracao>
    {
        public LoteEscrituracaoEDIIntegracao() { }
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "LEI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LayoutEDI", Column = "LAY_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.LayoutEDI LayoutEDI { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeArquivo", Column = "LEI_NOME_ARQUIVO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string NomeArquivo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LoteEscrituracao", Column = "LES_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual LoteEscrituracao LoteEscrituracao { get; set; }
        
        /// <summary>
        /// Se o edi é do transportador armazena aqui pois pode usar esse dado como parametro para algumas regras.
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }

        /// <summary>
        /// Indica que inciou a tentativa de envio externa (FTP, E-mail, etc).
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "IniciouConexaoExterna", Column = "INT_INICIOU_CONEXAO_EXTERNA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IniciouConexaoExterna { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.LayoutEDI?.Descricao ?? string.Empty;
            }
        }

        public virtual bool Equals(LoteEscrituracaoEDIIntegracao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
