using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Ocorrencias
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_OCORRENCIA_EDI_INTEGRACAO", EntityName = "OcorrenciaEDIIntegracao", Name = "Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaEDIIntegracao", NameType = typeof(OcorrenciaEDIIntegracao))]
    public class OcorrenciaEDIIntegracao : Integracao.Integracao, IEquatable<OcorrenciaEDIIntegracao>
    {
        public OcorrenciaEDIIntegracao() {}

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "COI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LayoutEDI", Column = "LAY_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.LayoutEDI LayoutEDI { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeArquivo", Column = "COI_NOME_ARQUIVO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string NomeArquivo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaOcorrencia", Column = "COC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CargaOcorrencia CargaOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SequenciaIntegracao", Column = "COI_SEQUENCIA_INTEGRACAO", TypeType = typeof(int), NotNull = false)]
        public virtual int SequenciaIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_DATA_ULTIMO_DOWNLOAD", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataUltimoDownload { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COI_FILIAL_EMISSORA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "XMLNotaFiscal", Column = "NFX_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal XMLNotaFiscal { get; set; }

        /// <summary>
        /// Indica que inciou a tentativa de envio externa (FTP, E-mail, etc).
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "IniciouConexaoExterna", Column = "INT_INICIOU_CONEXAO_EXTERNA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IniciouConexaoExterna { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Pedidos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_OCORRENCIA_EDI_INTEGRACAO_PEDIDO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "COI_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaPedido", Column = "CPE_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Cargas.CargaPedido> Pedidos { get; set; }

        public virtual bool Equals(OcorrenciaEDIIntegracao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
        public virtual string Descricao
        {
            get
            {
                return this.LayoutEDI?.Descricao ?? string.Empty;
            }
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
