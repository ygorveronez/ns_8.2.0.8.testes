using System;
using System.Collections.Generic;
using System.Linq;


namespace Dominio.Entidades.Embarcador.Escrituracao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PROVISAO", EntityName = "Provisao", Name = "Dominio.Entidades.Embarcador.Escrituracao.Provisao", NameType = typeof(Provisao))]
    public class Provisao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PRV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "PRV_NUMERO", TypeType = typeof(int), NotNull = false)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeDocsProvisao", Column = "PRV_QUANTIDADE_DOCUMENTOS_PROVISAO", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeDocsProvisao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorProvisao", Column = "PRV_VALOR_PROVISAO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorProvisao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaOcorrencia", Column = "COC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia CargaOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PRV_TIPO_LOCAL_PRESTACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoLocalPrestacao), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocalPrestacao TipoLocalPrestacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PRV_DATA_LANCAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataLancamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PRV_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataCriacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PRV_DATA_INICIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PRV_DATA_FIM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PRV_DATA_ULTIMO_INTENTO_FECHAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataUltimoIntentoFechamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraEscrituracao", Column = "RES_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RegraEscrituracao RegraEscrituracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Filiais.Filial Filial { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_TOMADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Tomador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerandoMovimentoFinanceiroProvisao", Column = "PRV_GERANDO_MOVIMENTO_FINANCEIRO_PROVISAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerandoMovimentoFinanceiroProvisao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MotivoRejeicaoFechamentoProvisao", Column = "PRV_MOTIVO_REJEICAO_FECHAMENTO_PROVISAO", TypeType = typeof(string), NotNull = false, Length = 1000)]
        public virtual string MotivoRejeicaoFechamentoProvisao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GeradoManualmente", Column = "PRV_GERADO_MANUALMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GeradoManualmente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PRV_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisao), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisao Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "DocumentosProvisao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_DOCUMENTO_PROVISAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PRV_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "DocumentoProvisao", Column = "DPV_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao> DocumentosProvisao { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Transportadores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PROVISAO_TRANSPORTADOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PRV_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Empresa", Column = "EMP_CODIGO")]
        public virtual ICollection<Empresa> Transportadores { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PRV_TIPO_PROVISAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoProvisao), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoProvisao TipoProvisao { get; set; }

        public virtual string Descricao
        {
            get { return this.Numero.ToString(); }
        }

        public virtual string DescricaoSituacao
        {
            get
            {
                switch (Situacao)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisao.AgIntegracao:
                        return "Ag. Integração";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisao.FalhaIntegracao:
                        return "Falha na Integração";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisao.EmAlteracao:
                        return "Em Alteração";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisao.Finalizado:
                        return "Finalizado";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisao.Cancelado:
                        return "Cancelado";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisao.EmFechamento:
                        return "Em Fechamento";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisao.PendenciaFechamento:
                        return "Pendência no Fechamento";
                    //case ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisao.EmIntegracao:
                    //    return "Em Integração";
                    default:
                        return "";
                }
            }
        }

        public virtual string DescricaoTransportadores
        {
            get
            {
                if (Transportadores?.Count > 1)
                    return "Múltiplos registros selecionados";

                return Transportadores?.FirstOrDefault()?.Descricao ?? "";
            }
        }
    }
}
