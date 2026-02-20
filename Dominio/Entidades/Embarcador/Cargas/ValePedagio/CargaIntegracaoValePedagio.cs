using Dominio.Interfaces.Embarcador.Entidade;
using Dominio.Interfaces.Embarcador.Integracao;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Cargas.ValePedagio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_INTEGRACAO_VALE_PEDAGIO", EntityName = "CargaIntegracaoValePedagio", Name = "Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoValePedagio", NameType = typeof(CargaIntegracaoValePedagio))]
    public class CargaIntegracaoValePedagio : Integracao.Integracao, IEquatable<CargaIntegracaoValePedagio>, IIntegracaoComArquivo<CargaValePedagioIntegracaoArquivo>, IEntidade
    {
        public CargaIntegracaoValePedagio()
        {
        }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CVP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoValePedagio", Column = "CVP_SITUACAO_VALE_PEDAGIO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio), NotNull = true)]
        public virtual SituacaoValePedagio SituacaoValePedagio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaCancelamento", Column = "CAC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaCancelamento CargaCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroValePedagio", Column = "CVP_NUMERO_VALE_PEDAGIO", TypeType = typeof(string), Length = 30, NotNull = false)]
        public virtual string NumeroValePedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IdCompraValePedagio", Column = "CVP_ID_COMPRA_VALE_PEDAGIO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string IdCompraValePedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorValePedagio", Column = "CVP_VALOR_VALE_PEDAGIO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorValePedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao1", Column = "CVP_OBSERVACAO_1", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Observacao1 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao2", Column = "CVP_OBSERVACAO_2", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Observacao2 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao3", Column = "CVP_OBSERVACAO_3", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Observacao3 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao4", Column = "CVP_OBSERVACAO_4", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Observacao4 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao5", Column = "CVP_OBSERVACAO_5", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Observacao5 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao6", Column = "CVP_OBSERVACAO_6", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Observacao6 { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RotaTemporaria", Column = "CVP_ROTA_TEMPORARIA", TypeType = typeof(string), Length = 44, NotNull = false)]
        public virtual string RotaTemporaria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracaoValePedagio", Column = "CVP_CODIGO_INTEGRACAO_VALE_PEDAGIO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string CodigoIntegracaoValePedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoRota", Column = "CVP_TIPO_ROTA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoRotaSemParar), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoRotaSemParar TipoRota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoCompra", Column = "CVP_TIPO_COMPRA", TypeType = typeof(Dominio.Enumeradores.TipoCompraValePedagio), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoCompraValePedagio TipoCompra { get; set; }

        /// <summary>
        /// Seria o vale de retorno, em casos onde compra separado do registro de ida, possibilitando ter eixos suspensos na volta.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CVP_VALE_PEDAGIO_RETORNO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool CompraComEixosSuspensos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoRoteiro", Column = "CVP_CODIGO_ROTEIRO", TypeType = typeof(int), NotNull = false)]
        public virtual int CodigoRoteiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoPercurso", Column = "CVP_CODIGO_PERCURSO", TypeType = typeof(int), NotNull = false)]
        public virtual int CodigoPercurso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeEixos", Column = "CVP_QUANTIDADE_EIXOS", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeEixos { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ArquivosTransacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_VALE_PEDAGIO_INTEGRACAO_ARQUIVO_ARQUIVO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CVP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaValePedagioIntegracaoArquivo", Column = "CVI_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo> ArquivosTransacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RecebidoPorIntegracao", Column = "CVP_RECEBIDO_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RecebidoPorIntegracao { get; set; }

        /// <summary>
        /// Rota exclusica do vale pedágio (Rota com a flag RotaExclusivaCompraValePedagio)
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RotaFrete", Column = "ROF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.RotaFrete RotaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CVP_VALIDACAO_COMPRA_REMOVEU_COMPONENTES", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ValidaCompraRemoveuComponentes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeTransportador", Column = "CVP_NOME_TRANSPORTADOR", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string NomeTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPercursoVP", Column = "CVP_TIPO_PERCURSO_VP", TypeType = typeof(TipoRotaFrete), NotNull = false)]
        public virtual TipoRotaFrete? TipoPercursoVP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CnpjMeioPagamento", Column = "CVP_CNPJ_MEIO_PAGAMENTO", TypeType = typeof(string), Length = 14, NotNull = false)]
        public virtual string CnpjMeioPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PedagioIntegradoEmbarcador", Column = "CVP_PEDAGIO_INTEGRADO_EMBARCADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PedagioIntegradoEmbarcador { get; set; }

        /// <summary>
        /// No caso da DigitalCom apenas um registro realiza a compra, o segundo é apenas para guardar a informação
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaIntegracaoValePedagio", Column = "CVP_CODIGO_VALE_PEDAGIO_COMPRA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CargaIntegracaoValePedagio CargaIntegracaoValePedagioCompra { get; set; }

        /// <summary>
        /// ID da emissão do pedágio perante a ANTT
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoEmissaoValePedagioANTT", Column = "CVP_CODIGO_EMISSAO_VALE_PEDAGIO_ANTT", TypeType = typeof(string), Length = 30, NotNull = false)]
        public virtual string CodigoEmissaoValePedagioANTT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OperacaoContainer", Column = "CVP_OPERACAO_CONTAINER", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? OperacaoContainer { get; set; }

        #region Propriedades Virtuais

        public virtual CargaIntegracaoValePedagio Clonar()
        {
            return (CargaIntegracaoValePedagio)this.MemberwiseClone();
        }

        public virtual string Descricao
        {
            get
            {
                return NumeroValePedagio + " - " + (this.Carga?.Descricao ?? string.Empty);
            }
        }

        public virtual string DescricaoSituacaoValePedagio
        {
            get { return SituacaoValePedagio.ObterDescricao(); }
        }

        public virtual bool Equals(CargaIntegracaoValePedagio other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        public virtual string TipoCompraDescricao
        {
            get
            {
                switch (TipoCompra)
                {
                    case Dominio.Enumeradores.TipoCompraValePedagio.Cartao:
                        return Localization.Resources.Enumeradores.TipoCompraValePedagio.Cartao;
                    case Dominio.Enumeradores.TipoCompraValePedagio.Tag:
                        return Localization.Resources.Enumeradores.TipoCompraValePedagio.Tag;
                    case Dominio.Enumeradores.TipoCompraValePedagio.Cupom:
                        return Localization.Resources.Enumeradores.TipoCompraValePedagio.Cupom;
                    default:
                        return string.Empty;
                }
            }
        }

        #endregion
    }
}
