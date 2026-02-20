using System;
using System.Collections.Generic;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CTE_AVERBACAO", EntityName = "AverbacaoCTe", Name = "Dominio.Entidades.AverbacaoCTe", NameType = typeof(AverbacaoCTe))]
    public class AverbacaoCTe : EntidadeBase
    {
        public AverbacaoCTe() { }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "AVE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConhecimentoDeTransporteEletronico CTe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "XMLNotaFiscal", Column = "NFX_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal XMLNotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaDocumentoParaEmissaoNFSManual", Column = "NEM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual CargaDocumentoParaEmissaoNFSManual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Protocolo", Column = "AVE_PROTOCOLO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Protocolo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AVE_TENTATIVAS_INTEGRACAO", TypeType = typeof(int), NotNull = false)]
        public virtual int tentativasIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoRetorno", Column = "AVE_CODIGO_RETORNO", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string CodigoRetorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MensagemRetorno", Column = "AVE_MENSAGEM_RETORNO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string MensagemRetorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "AVE_CODIGO_INTEGRACAO", TypeType = typeof(int), NotNull = false)]
        public virtual int CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataRetorno", Column = "AVE_DATA_RETORNO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataRetorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "AVE_TIPO", TypeType = typeof(Enumeradores.TipoAverbacaoCTe), NotNull = true)]
        public virtual Enumeradores.TipoAverbacaoCTe Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "AVE_STATUS", TypeType = typeof(Enumeradores.StatusAverbacaoCTe), NotNull = true)]
        public virtual Enumeradores.StatusAverbacaoCTe Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AVE_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoAverbacaoFechamento), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoAverbacaoFechamento SituacaoFechamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AVE_ADICIONAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Adicional { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AVE_IOF", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal IOF { get; set; }

        /// <summary>
        /// Atributo usado pelo MultiCTe
        /// MultiEmbarcador usar ApoliceSeguroAverbacao.ApoliceSeguro.SeguradoraAverbacao
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "SeguradoraAverbacao", Column = "AVE_SEGURADORA", TypeType = typeof(Dominio.Enumeradores.IntegradoraAverbacao), NotNull = false)]
        public virtual Dominio.Enumeradores.IntegradoraAverbacao SeguradoraAverbacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Averbacao", Column = "AVE_AVERBACAO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Averbacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ApoliceSeguroAverbacao", Column = "CPA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Seguros.ApoliceSeguroAverbacao ApoliceSeguroAverbacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaCancelamento", Column = "CAC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaCancelamento CargaCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AVE_DESCONTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Desconto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AVE_PERCENTUAL", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal? Percentual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Forma", Column = "AVE_FORMA", TypeType = typeof(Enumeradores.FormaAverbacaoCTE), NotNull = false)]
        public virtual Enumeradores.FormaAverbacaoCTE Forma { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AverbacaoImportada", Column = "AVE_AVERBACAO_IMPORTADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? AverbacaoImportada { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ArquivosTransacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_AVERBACAO_INTEGRACAO_ARQUIVO_ARQUIVO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "AVE_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AverbacaoIntegracaoArquivo", Column = "AIA_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Averbacao.AverbacaoIntegracaoArquivo> ArquivosTransacao { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ArquivosTransacaoCancelamento", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_AVERBACAO_INTEGRACAO_ARQUIVO_CANCELAMENTO_ARQUIVO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "AVE_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AverbacaoIntegracaoArquivo", Column = "AIA_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Averbacao.AverbacaoIntegracaoArquivo> ArquivosTransacaoCancelamento { get; set; }

        public virtual string DescricaoSituacaoFechamento
        {
            get
            {
                switch (SituacaoFechamento)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoAverbacaoFechamento.EmAberto:
                        return "Em Aberto";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoAverbacaoFechamento.EmFechamento:
                        return "Em Fechamento";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoAverbacaoFechamento.Finalizada:
                        return "Finalizada";
                    default:
                        return "";
                }
            }
        }

        public virtual string DescricaoTipo
        {
            get
            {
                switch (this.Tipo)
                {
                    case Enumeradores.TipoAverbacaoCTe.Autorizacao:
                        return "Autorização";
                    case Enumeradores.TipoAverbacaoCTe.Cancelamento:
                        return "Cancelamento";
                    default:
                        return string.Empty;
                }
            }
        }

        public virtual string DescricaoStatus
        {
            get
            {
                switch (this.Status)
                {
                    case Enumeradores.StatusAverbacaoCTe.Rejeicao:
                        return "Rejeição";
                    case Enumeradores.StatusAverbacaoCTe.Sucesso:
                        return "Averbado";
                    case Enumeradores.StatusAverbacaoCTe.Pendente:
                        return "Pendente";
                    case Enumeradores.StatusAverbacaoCTe.Enviado:
                        return "Enviado";
                    case Enumeradores.StatusAverbacaoCTe.Cancelado:
                        return "Cancelado";
                    case Enumeradores.StatusAverbacaoCTe.AgEmissao:
                        return "Ag. Emissão";
                    case Enumeradores.StatusAverbacaoCTe.AgCancelamento:
                        return "Ag. Cancelamento";
                    default:
                        return string.Empty;
                }
            }
        }

        public virtual string DescricaoForma
        {
            get
            {
                switch (this.Forma)
                {
                    case Enumeradores.FormaAverbacaoCTE.Definitiva:
                        return "Definitiva";
                    case Enumeradores.FormaAverbacaoCTE.Provisoria:
                        return "Provisoria";
                    default:
                        return "Definitiva";
                }
            }
        }

        public virtual string DescricaoSeguradoraAverbacao
        {
            get
            {
                switch (this.SeguradoraAverbacao)
                {
                    case Enumeradores.IntegradoraAverbacao.NaoDefinido:
                        return "Não Definido";
                    case Enumeradores.IntegradoraAverbacao.ATM:
                        return "ATM";
                    case Enumeradores.IntegradoraAverbacao.Quorum:
                        return "Quoruom";
                    case Enumeradores.IntegradoraAverbacao.PortoSeguro:
                        return "Porto Seguro";
                    default:
                        return string.Empty;
                }
            }
        }

        public virtual string Descricao
        {
            get
            {
                return this.Averbacao;// + " - " + (this.CTe?.Descricao ?? string.Empty);
            }
        }
    }
}
