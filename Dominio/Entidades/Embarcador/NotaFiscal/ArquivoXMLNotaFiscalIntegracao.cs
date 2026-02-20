using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.NotaFiscal
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ARQUIVO_XML_NOTA_FISCAL_IMPORTACAO", EntityName = "ArquivoXMLNotaFiscalIntegracao", Name = "Dominio.Entidades.Embarcador.NotaFiscal.ArquivoXMLNotaFiscalIntegracao", NameType = typeof(ArquivoXMLNotaFiscalIntegracao))]
    public class ArquivoXMLNotaFiscalIntegracao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "AXI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeArquivo", Column = "AXI_NOME_ARQUIVO", TypeType = typeof(string), Length = 500, NotNull = true)]
        public virtual string NomeArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "AXI_SITUACAO", TypeType = typeof(SituacaoProcessamentoRegistro), NotNull = true)]
        public virtual SituacaoProcessamentoRegistro Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tentativas", Column = "AXI_TENTATIVAS", TypeType = typeof(int), NotNull = true)]
        public virtual int Tentativas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TentativasLiberacao", Column = "AXI_TENTATIVAS_LIBERACAO", TypeType = typeof(int), NotNull = true)]
        public virtual int TentativasLiberacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Mensagem", Column = "AXI_MENSAGEM", TypeType = typeof(string), NotNull = false, Length = 500)]
        public virtual string Mensagem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Integradora", Column = "INT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Entidades.WebService.Integradora Integradora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IP", Column = "AXI_IP", TypeType = typeof(string), Length = 255, NotNull = false)]
        public virtual string IP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataRecebimento", Column = "AXI_DATA_RECEBIMENTO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataRecebimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Chave", Column = "AXI_CHAVE", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Chave { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AXI_PROTOCOLO_PEDIDO", TypeType = typeof(int), NotNull = false)]
        public virtual int ProtocoloPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataTentativa", Column = "AXI_DATA_TENTATIVA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataTentativa { get; set; }

        public virtual string Descricao
        {
            get
            {
                return "";
            }
        }

    }
}
