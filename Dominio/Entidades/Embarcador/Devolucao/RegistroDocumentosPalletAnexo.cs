using System;

namespace Dominio.Entidades.Embarcador.Devolucao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_GESTAO_DEVOLUCAO_REGISTRO_DOCUMENTO_PALLET_ANEXO", EntityName = "RegistroDocumentosPalletAnexo", Name = "Dominio.Entidades.Embarcador.Devolucao.RegistroDocumentosPalletAnexo", NameType = typeof(RegistroDocumentosPalletAnexo))]
    public class RegistroDocumentosPalletAnexo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "GRD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual long Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GestaoDevolucao", Column = "GDV_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual GestaoDevolucao GestaoDevolucao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRD_DESCRICAO", Type = "String", Length = 400, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRD_NOME_ARQUIVO", Type = "String", Length = 200, NotNull = true)]
        public virtual string NomeArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRD_GUID_ARQUIVO", Type = "String", Length = 40, NotNull = true)]
        public virtual string GuidArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRD_DATA_ENVIO_ARQUIVO", Type = "DateTime", NotNull = true)]
        public virtual DateTime DataEnvioArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PagamentoRealizado", Column = "GRD_PAGAMENTO_REALIZADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PagamentoRealizado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DadosXMLNota", Column = "GRD_DADOS_XML", Type = "StringClob", NotNull = false, Lazy = true)]
        public virtual string DadosXMLNota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoRegistroPermutaPallet", Column = "GRD_TIPO_REGISTRO_PERMUTA_PALLET", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRegistroPermutaPallet), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRegistroPermutaPallet TipoRegistroPermutaPallet { get; set; }

        public virtual string ExtensaoArquivo
        {
            get { return System.IO.Path.GetExtension(NomeArquivo).ToLower().Replace(".", ""); }
        }
    }
}
