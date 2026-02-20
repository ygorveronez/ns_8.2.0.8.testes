using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Escrituracao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_LOTE_ESCRITURACAO", EntityName = "LoteEscrituracao", Name = "Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracao", NameType = typeof(LoteEscrituracao))]
    public class LoteEscrituracao : EntidadeBase
    {

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "LES_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "LES_NUMERO", TypeType = typeof(int), NotNull = false)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "LES_GERACAO_LOTE", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataGeracaoLote { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "LES_DATA_INICIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicial { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "LES_DATA_FIM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFinal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Filiais.Filial Filial { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_TOMADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Tomador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloDocumentoFiscal", Column = "MOD_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.ModeloDocumentoFiscal ModeloDocumentoFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "LES_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteEscrituracao), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteEscrituracao Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "DocumentosEscrituracao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_DOCUMENTO_ESCRITURACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "LES_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "DocumentoEscrituracao", Column = "DES_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracao> DocumentosEscrituracao { get; set; }


        [NHibernate.Mapping.Attributes.Bag(0, Name = "Integracoes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_LOTE_ESCRITURACAO_INTEGRACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "LES_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "LoteEscrituracaoIntegracao", Column = "ILN_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoIntegracao> Integracoes { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Numero.ToString();
            }
        }

        public virtual string DescricaoSituacao
        {
            get
            {
                switch (Situacao)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteEscrituracao.AgIntegracao:
                        return "Ag. Integração";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteEscrituracao.FalhaIntegracao:
                        return "Falha na Integração";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteEscrituracao.EmCriacao:
                        return "Em Criação";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteEscrituracao.Finalizado:
                        return "Finalizado";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteEscrituracao.Cancelado:
                        return "Cancelado";
                    default:
                        return "";
                }
            }
        }

    }
}
