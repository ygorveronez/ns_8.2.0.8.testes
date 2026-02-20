using System;
using System.Collections.Generic;
using Dominio.Interfaces.Embarcador.Entidade;


namespace Dominio.Entidades.Embarcador.Escrituracao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PROVISAO_CANCELAMENTO", EntityName = "CancelamentoProvisao", Name = "Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisao", NameType = typeof(CancelamentoProvisao))]
    public class CancelamentoProvisao : EntidadeBase, IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CPV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "CPV_NUMERO", TypeType = typeof(int), NotNull = false)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeDocsProvisao", Column = "CPV_QUANTIDADE_DOCUMENTOS_PROVISAO", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeDocsProvisao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CancelamentoProvisaoContraPartida", Column = "CPV_CANCELAMENTO_PROVISAO_CONTRA_PARTIDA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CancelamentoProvisaoContraPartida { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorCancelamentoProvisao", Column = "CPV_VALOR_CANCELAMENTO_PROVISAO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorCancelamentoProvisao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPV_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataCriacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPV_DATA_INICIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPV_DATA_FIM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPV_DATA_LANCAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataLancamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaOcorrencia", Column = "COC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia CargaOcorrencia { get; set; }


        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Filiais.Filial Filial { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_TOMADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Tomador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "GerandoMovimentoFinanceiro", Column = "CPV_GERANDO_MOVIMENTO_FINANCEIRO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerandoMovimentoFinanceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MotivoRejeicaoCancelamentoFechamentoProvisao", Column = "CPV_MOTIVO_REJEICAO_CANCELAMENTO_FECHAMENTO_PROVISAO", TypeType = typeof(string), NotNull = false, Length = 1000)]
        public virtual string MotivoRejeicaoCancelamentoFechamentoProvisao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPV_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoProvisao), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoProvisao Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "DocumentosProvisao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_DOCUMENTO_PROVISAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CPV_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "DocumentoProvisao", Column = "DPV_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao> DocumentosProvisao { get; set; }


        [NHibernate.Mapping.Attributes.Bag(0, Name = "Integracoes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CANCELAMENTO_PROVISAO_INTEGRACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CPV_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CancelamentoProvisaoIntegracao", Column = "CIN_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisaoIntegracao> Integracoes { get; set; }

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
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoProvisao.Todos:
                        return "";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoProvisao.EmCancelamento:
                        return "Em Cancelamento";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoProvisao.PendenciaCancelamento:
                        return "Pendência Cancelamento";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoProvisao.AgIntegracao:
                        return "Ag. Integração";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoProvisao.EmIntegracao:
                        return "Em Integração";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoProvisao.FalhaIntegracao:
                        return "Falha na integração";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoProvisao.Cancelado:
                        return "Cancelado";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoProvisao.Estornado:
                        return "Estornado";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoProvisao.SemRegraAprovacao:
                        return "Sem Regra de Aprovação";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoProvisao.SolicitacaoAprovada:
                        return "Solicitação Aprovada";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoProvisao.SolicitacaoReprovada:
                        return "Solicitação Reprovada";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoProvisao.AgAprovacaoSolicitacao:
                        return "Ag. Aprovação";  
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoProvisao.NaoProcessado:
                        return "Não processado";
                    default:
                        return "";
                }
            }
        }

    }
}
