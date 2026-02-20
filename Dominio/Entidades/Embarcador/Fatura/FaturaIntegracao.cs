using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Fatura
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FATURA_INTEGRACAO", EntityName = "FaturaIntegracao", Name = "Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao", NameType = typeof(FaturaIntegracao))]
    public class FaturaIntegracao : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao>
    {
        public FaturaIntegracao() { }
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FAI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Fatura", Column = "FAT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Fatura.Fatura Fatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeArquivo", Column = "FAI_NOME_ARQUIVO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string NomeArquivo { get; set; }

        /// <summary>
        /// Indica que inciou a tentativa de envio externa (FTP, E-mail, etc).
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "IniciouConexaoExterna", Column = "FAI_INICIOU_CONEXAO_EXTERNA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IniciouConexaoExterna { get; set; }

        /// <summary>
        /// Se tiver o tempo informado faz o reenvio automaticamente da integração após o tempo determinado (somente quando a tentativa anterior estiver integrada).
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "ReenviarAutomaticamenteOutraVezAposMinutos", Column = "FAI_REENVIAR_AUTOMATICAMENTE_APOS_MINUTOS", TypeType = typeof(int), NotNull = false)]
        public virtual int ReenviarAutomaticamenteOutraVezAposMinutos { get; set; }


        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LayoutEDI", Column = "LAY_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual LayoutEDI LayoutEDI { get; set; }
        
        /// <summary>
        /// Se o edi é do transportador armazena aqui pois pode usar esse dado como parametro para algumas regras.
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoIntegracao", Column = "FAI_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao SituacaoIntegracao { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoIntegracaoFatura", Column = "FAI_TIPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoFatura), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoFatura TipoIntegracaoFatura { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoIntegracao", Column = "TPI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.TipoIntegracao TipoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tentativas", Column = "FAI_TENTATIVAS", TypeType = typeof(int), NotNull = false)]
        public virtual int Tentativas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEnvio", Column = "FAI_DATA_ENVIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEnvio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FAI_MENSAGEM_RETORNO", TypeType = typeof(string), Length = 10000, NotNull = false)]
        public virtual string MensagemRetorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FAI_CODIGO_INTEGRACAO_INTEGRADORA", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string CodigoIntegracaoIntegradora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FAI_USAR_CST", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UsarCST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FAI_MODELO_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ModeloCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FAI_TIPO_IMPOSTO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string TipoImposto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoExternoRetornoIntegracao", Column = "FAI_CODIGO_EXTERNO_RETORNO_INTEGRACAO", TypeType = typeof(string), NotNull = false)]
        public virtual string CodigoExternoRetornoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_FATURA_INTEGRACAO_INTEGRACAO_ARQUIVO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "FAI_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "FaturaIntegracaoArquivo", Column = "FIA_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoArquivo> ArquivosIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_FATURA_INTEGRACAO_CST_INTEGRACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "FAI_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "FaturaIntegracaoCST", Column = "FCS_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoCST> CSTs { get; set; }

        public virtual List<int> CodigosCTes { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.LayoutEDI?.Descricao ?? DescricaoTipoIntegracao;
            }
        }
        //[NHibernate.Mapping.Attributes.Property(0, Name = "ArquivoEnvio", Column = "FAI_ARQUIVO_ENVIO", Type = "StringClob", NotNull = false)]
        //public virtual string ArquivoEnvio { get; set; }

        //[NHibernate.Mapping.Attributes.Property(0, Name = "ArquivoRetorno", Column = "FAI_ARQUIVO_RETORNO", Type = "StringClob", NotNull = false)]
        //public virtual string ArquivoRetorno { get; set; }

        //public virtual string DescricaoRetornoIntegracao
        //{
        //    get
        //    {
        //        switch (this.RetornoIntegracao)
        //        {
        //            case ObjetosDeValor.Embarcador.Enumeradores.RetornoIntegracao.Aguardando:
        //                return "Aguardando";
        //            case ObjetosDeValor.Embarcador.Enumeradores.RetornoIntegracao.Erro:
        //                return "Erro";
        //            case ObjetosDeValor.Embarcador.Enumeradores.RetornoIntegracao.Rejeicao:
        //                return "Rejeição";
        //            case ObjetosDeValor.Embarcador.Enumeradores.RetornoIntegracao.Sucesso:
        //                return "Sucesso";
        //            default:
        //                return "";
        //        }
        //    }
        //}

        public virtual string DescricaoSituacaoIntegracao
        {
            get
            {
                switch (this.SituacaoIntegracao)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao:
                        return "Aguardando Integração";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno:
                        return "Aguardando Retorno";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado:
                        return "Integrado";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao:
                        return "Problemas com a Integração";
                    default:
                        return "";
                }
            }
        }

        public virtual string DescricaoTipoIntegracao
        {
            get
            {
                return TipoIntegracao?.DescricaoTipo ?? string.Empty;
            }
        }

        public virtual bool Equals(FaturaIntegracao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
