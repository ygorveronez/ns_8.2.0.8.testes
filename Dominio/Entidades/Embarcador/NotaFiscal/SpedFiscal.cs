using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.NotaFiscal
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_SPED_GERACAO", EntityName = "SpedFiscal", Name = "Dominio.Entidades.Embarcador.NotaFiscal.SpedFiscal", NameType = typeof(SpedFiscal))]
    public class SpedFiscal : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.NotaFiscal.SpedFiscal>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "SPG_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicial", Column = "SPG_DATA_INICIAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFinal", Column = "SPG_DATA_FINAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoMovimento", Column = "SPG_TIPO_MOVIMENTO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoSpedFiscal), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoSpedFiscal TipoMovimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ComExtensaoCFOP", Column = "SPG_COM_EXTENSAO_CFOP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ComExtensaoCFOP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ComNFSePropria", Column = "SPG_COM_NFSE_PROPRIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ComNFSePropria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ComIntentario", Column = "SPG_COM_INVENTARIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ComIntentario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ComRetorno", Column = "SPG_COM_RETORNO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string ComRetorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CaminhoArquivo", Column = "SPG_CAMINHO_ARQUIVO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string CaminhoArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusArquivo", Column = "SPG_STATUS_ARQUIVO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.StatusArquivoSpedFiscal), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.StatusArquivoSpedFiscal StatusArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInventario", Column = "SPG_DATA_INVENTARIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInventario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ComRessarcimentoICMS", Column = "SPG_COM_RESSARCIMENTO_ICMS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ComRessarcimentoICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicialFinalizacao", Column = "SPG_DATA_INICIAL_FINALIZACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicialFinalizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFinalFinalizacao", Column = "SPG_DATA_FINAL_FINALIZACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFinalFinalizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicialEntrada", Column = "SPG_DATA_INICIAL_ENTRADA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicialEntrada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFinalEntrada", Column = "SPG_DATA_FINAL_ENTRADA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFinalEntrada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ComD160", Column = "SPG_COM_D160", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ComD160 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ComD170", Column = "SPG_COM_D170", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ComD170 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ComBlocoK", Column = "SPG_COM_BLOCO_K", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ComBlocoK { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "DocumentoEntradaTMS", Column = "TDE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Financeiro.DocumentoEntradaTMS DocumentoEntrada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoDocumentoSPEDFiscal", Column = "SPG_SITUACAO_DOCUMENTO_SPED_FISCAL", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoSPEDFiscal), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoSPEDFiscal SituacaoDocumentoSPEDFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Documentos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_SPED_GERACAO_DOCUMENTO_ENTRADA_TMS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "SPG_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "DocumentoEntradaTMS", Column = "TDE_CODIGO")]
        public virtual ICollection<Financeiro.DocumentoEntradaTMS> Documentos { get; set; }

        public virtual string DescricaoTipo
        {
            get
            {
                switch (this.TipoMovimento)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoSpedFiscal.Todos:
                        return "Todos";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoSpedFiscal.Entrada:
                        return "Entrada";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoSpedFiscal.Saida:
                        return "Saída";
                    default:
                        return "";
                }
            }
        }

        public virtual string DescricaoStatus
        {
            get
            {
                switch (this.StatusArquivo)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.StatusArquivoSpedFiscal.AguardandoGeracao:
                        return "Aguardando Geração";
                    case ObjetosDeValor.Embarcador.Enumeradores.StatusArquivoSpedFiscal.Gerado:
                        return "Gerado";
                    case ObjetosDeValor.Embarcador.Enumeradores.StatusArquivoSpedFiscal.EmProcesso:
                        return "Em Processo";
                    case ObjetosDeValor.Embarcador.Enumeradores.StatusArquivoSpedFiscal.ErroValidacao:
                        return "Erro de Validação";
                    default:
                        return "";
                }
            }
        }
        public virtual string Descricao
        {
            get
            {
                return this.Codigo.ToString();
            }
        }

        public virtual bool Equals(SpedFiscal other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}