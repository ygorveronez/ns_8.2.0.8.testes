using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_CIOT", EntityName = "CargaCIOT", Name = "Dominio.Entidades.Embarcador.Cargas.CargaCIOT", NameType = typeof(CargaCIOT))]
    public class CargaCIOT : EntidadeBase
    {
        public CargaCIOT() { }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ContratoFrete", Column = "CFT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Terceiros.ContratoFrete ContratoFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_MOTORISTA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Motorista { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CIOT", Column = "CIO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Documentos.CIOT CIOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CargaAdicionadaAoCIOT", Column = "CCO_CARGA_ADICIONADA_CIOT", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CargaAdicionadaAoCIOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PesoBruto", Column = "CCO_PESO_BRUTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PesoBruto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorMercadoriaKG", Column = "CCO_VALOR_MERCADORIA_KG", TypeType = typeof(decimal), Scale = 2, Precision = 8, NotNull = false)]
        public virtual decimal ValorMercadoriaKG { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalMercadoria", Column = "CCO_VALOR_TOTAL_MERCADORIA", TypeType = typeof(decimal), Scale = 2, Precision = 10, NotNull = false)]
        public virtual decimal ValorTotalMercadoria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTarifaFrete", Column = "CCO_VALOR_TARIFA_FRETE", TypeType = typeof(decimal), Scale = 2, Precision = 8, NotNull = false)]
        public virtual decimal ValorTarifaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFrete", Column = "CCO_VALOR_FRETE", TypeType = typeof(decimal), Scale = 2, Precision = 8, NotNull = false)]
        public virtual decimal ValorFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoQuebra", Column = "CCO_TIPO_QUEBRA", TypeType = typeof(Enumeradores.TipoQuebra), NotNull = true)]
        public virtual Enumeradores.TipoQuebra TipoQuebra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoTolerancia", Column = "CCO_TIPO_TOLERANCIA", TypeType = typeof(Enumeradores.TipoTolerancia), NotNull = true)]
        public virtual Enumeradores.TipoTolerancia TipoTolerancia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualTolerancia", Column = "CCO_PERCENTUAL_TOLERANCIA", TypeType = typeof(decimal), Scale = 2, Precision = 8, NotNull = true)]
        public virtual decimal PercentualTolerancia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualToleranciaSuperior", Column = "CCO_PERCENTUAL_TOLERANCIA_SUPERIOR", TypeType = typeof(decimal), Scale = 2, Precision = 8, NotNull = true)]
        public virtual decimal PercentualToleranciaSuperior { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorAdiantamento", Column = "CCO_VALOR_ADIANTAMENTO", TypeType = typeof(decimal), Scale = 2, Precision = 8, NotNull = true)]
        public virtual decimal ValorAdiantamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorSeguro", Column = "CCO_VALOR_SEGURO", TypeType = typeof(decimal), Scale = 2, Precision = 8, NotNull = true)]
        public virtual decimal ValorSeguro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorPedagio", Column = "CCO_VALOR_PEDAGIO", TypeType = typeof(decimal), Scale = 2, Precision = 8, NotNull = true)]
        public virtual decimal ValorPedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorIRRF", Column = "CCO_VALOR_IRRF", TypeType = typeof(decimal), Scale = 2, Precision = 8, NotNull = true)]
        public virtual decimal ValorIRRF { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorINSS", Column = "CCO_VALOR_INSS", TypeType = typeof(decimal), Scale = 2, Precision = 8, NotNull = true)]
        public virtual decimal ValorINSS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorSENAT", Column = "CCO_VALOR_SENAT", TypeType = typeof(decimal), Scale = 2, Precision = 8, NotNull = true)]
        public virtual decimal ValorSENAT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorSEST", Column = "CCO_VALOR_SEST", TypeType = typeof(decimal), Scale = 2, Precision = 8, NotNull = true)]
        public virtual decimal ValorSEST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorOutrosDescontos", Column = "CCO_VALOR_OUTROS_DESCONTOS", TypeType = typeof(decimal), Scale = 2, Precision = 8, NotNull = true)]
        public virtual decimal ValorOutrosDescontos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CargaAberturaCIOT", Column = "CCO_PRIMEIRA_VIAGEM_CIOT", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CargaAberturaCIOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "CCO_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT? Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCO_PROTOCOLO_ABERTURA", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string ProtocoloAbertura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCO_PROTOCOLO_AUTORIZACAO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string ProtocoloAutorizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCO_MENSAGEM", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Mensagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCO_DATA_AUTORIZACAO_PAGAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAutorizacaoPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ArquivosTransacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_CIOT_INTEGRACAO_ARQUIVO_ARQUIVO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CIO_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaCIOTIntegracaoArquivo", Column = "CCI_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Cargas.CargaCIOTIntegracaoArquivo> ArquivosTransacao { get; set; }

        public virtual string Descricao
        {
            get
            {
                return (this.Carga?.Descricao ?? string.Empty) + " - " + (this.CIOT?.Descricao ?? string.Empty);
            }
        }

        public virtual bool Equals(CargaCIOT other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        public virtual string DescricaoSituacao
        {
            get
            {
                switch (this.Situacao)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Aberto:
                        return "Viagem Integrada";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Cancelado:
                        return "Cancelado";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Encerrado:
                        return "Encerrado";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.AgIntegracao:
                        return "Ag. Integração";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Pendencia:
                        return "Pendência Integração";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.PagamentoAutorizado:
                        return "Pagamento autorizado";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.AgLiberarViagem:
                        return "Ag. Liberar Viagem";
                    default:
                        return string.Empty;
                }
            }
        }
    }
}
