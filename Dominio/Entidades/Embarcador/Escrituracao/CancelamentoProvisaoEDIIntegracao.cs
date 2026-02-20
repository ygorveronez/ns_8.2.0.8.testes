using System;

namespace Dominio.Entidades.Embarcador.Escrituracao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CANCELAMENTO_PROVISAO_EDI_INTEGRACAO", EntityName = "CancelamentoProvisaoEDIIntegracao", Name = "Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisaoEDIIntegracao", NameType = typeof(CancelamentoProvisaoEDIIntegracao))]
    public class CancelamentoProvisaoEDIIntegracao : Integracao.Integracao, IEquatable<CancelamentoProvisaoEDIIntegracao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CPE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LayoutEDI", Column = "LAY_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.LayoutEDI LayoutEDI { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeArquivo", Column = "CPE_NOME_ARQUIVO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string NomeArquivo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CancelamentoProvisao", Column = "CPV_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CancelamentoProvisao CancelamentoProvisao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SequenciaIntegracao", Column = "CPE_SEQUENCIA_INTEGRACAO", TypeType = typeof(int), NotNull = false)]
        public virtual int SequenciaIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPE_DATA_ULTIMO_DOWNLOAD", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataUltimoDownload { get; set; }

        /// <summary>
        /// Indica que inciou a tentativa de envio externa (FTP, E-mail, etc).
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "IniciouConexaoExterna", Column = "INT_INICIOU_CONEXAO_EXTERNA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IniciouConexaoExterna { get; set; }

        /// <summary>
        /// Se o edi é do transportador armazena aqui pois pode usar esse dado como parametro para algumas regras.
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.LayoutEDI?.Descricao ?? string.Empty;
            }
        }

        public virtual bool Equals(CancelamentoProvisaoEDIIntegracao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        public virtual int ObterSequencia()
        {
            if (!this.DataUltimoDownload.HasValue || this.DataUltimoDownload.Value.Date < DateTime.Today)
                this.SequenciaIntegracao = 0;

            this.DataUltimoDownload = DateTime.Now;

            return ++this.SequenciaIntegracao;
        }
    }
}
