using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.PagamentoMotorista
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PAGAMENTO_MOTORISTA_TMS", EntityName = "PagamentoMotoristaTMS", Name = "Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS", NameType = typeof(PagamentoMotoristaTMS))]
    public class PagamentoMotoristaTMS : EntidadeBase, IEquatable<PagamentoMotoristaTMS>, Interfaces.Embarcador.Entidade.IEntidade
    {
        public PagamentoMotoristaTMS() { }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PAM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "PAM_NUMERO", TypeType = typeof(int), NotNull = false)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "PAM_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataPagamento", Column = "PAM_DATA_PAGAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "PAM_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "PAM_OBSERVACAO", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoPagamentoMotorista", Column = "PAM_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista SituacaoPagamentoMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EtapaPagamentoMotorista", Column = "PAM_ETAPA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.EtapaPagamentoMotorista), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.EtapaPagamentoMotorista EtapaPagamentoMotorista { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_MOTORISTA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Motorista { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PagamentoMotoristaTipo", Column = "PMT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PagamentoMotoristaTipo PagamentoMotoristaTipo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PlanoConta", Column = "PLA_CODIGO_DEBITO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.PlanoConta PlanoDeContaDebito { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PlanoConta", Column = "PLA_CODIGO_CREDITO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.PlanoConta PlanoDeContaCredito { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_TITULO_PAGAR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente PessoaTituloPagar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataVencimentoTituloPagar", Column = "PAM_DATA_VENCIMENTO_TITULO_PAGAR", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataVencimentoTituloPagar { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Titulo", Column = "TIT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.Titulo Titulo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SaldoDescontado", Column = "PAM_SALDO_DESCONTADO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal SaldoDescontado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "AcertoViagem", Column = "ACV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Acerto.AcertoViagem AcertoViagem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Chamado", Column = "CHA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Chamados.Chamado Chamado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MoedaCotacaoBancoCentral", Column = "PAM_MOEDA_COTACAO_BANCO_CENTRAL", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral? MoedaCotacaoBancoCentral { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataBaseCRT", Column = "PAM_DATA_BASE_CRT", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataBaseCRT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorMoedaCotacao", Column = "PAM_VALOR_MOEDA_COTACAO", TypeType = typeof(decimal), Scale = 10, Precision = 21, NotNull = false)]
        public virtual decimal ValorMoedaCotacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorOriginalMoedaEstrangeira", Column = "PAM_VALOR_ORIGINAL_MOEDA_ESTRANGEIRA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorOriginalMoedaEstrangeira { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoViagem", Column = "PAM_CODIGO_VIAGEM", TypeType = typeof(int), NotNull = false)]
        public virtual int CodigoViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PagamentoLiberado", Column = "PAM_PAGAMENTO_LIBERADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PagamentoLiberado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Terceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorAdiantamento", Column = "PAM_VALOR_ADIANTAMENTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorAdiantamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAdiantamento", Column = "PAM_DATA_ADIANTAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAdiantamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataSaldo", Column = "PAM_DATA_SALDO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataSaldo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SaldoDiariaMotorista", Column = "PAM_SALDO_DIARIA_MOTORISTA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal SaldoDiariaMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "PagamentoMotoristaAutorizacoes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PAGAMENTO_MOTORISTA_TMS_AUTORIZACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PAM_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PagamentoMotoristaAutorizacao", Column = "PMA_CODIGO")]
        public virtual ICollection<PagamentoMotoristaAutorizacao> PagamentoMotoristaAutorizacoes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEfetivacao", Column = "PAM_DATA_EFETIVACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEfetivacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Pendente", Column = "PAM_PENDENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? Pendente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAM_BASE_CALCULO_INSS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal BaseCalculoINSS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAM_ALIQUOTA_INSS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaINSS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAM_VALOR_INSS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorINSS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAM_BASE_CALCULO_SEST", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal BaseCalculoSEST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAM_ALIQUOTA_SEST", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaSEST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAM_VALOR_SEST", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorSEST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAM_BASE_CALCULO_SENAT", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal BaseCalculoSENAT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAM_ALIQUOTA_SENAT", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaSENAT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAM_VALOR_SENAT", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorSENAT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAM_BASE_CALCULO_IRRF", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal BaseCalculoIRRF { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAM_ALIQUOTA_IRRF", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaIRRF { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAM_VALOR_IRRF", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorIRRF { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAM_BASE_CALCULO_IRRF_SEM_DESCONTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal BaseCalculoIRRFSemDesconto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAM_BASE_CALCULO_IRRF_SEM_ACUMULO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal BaseCalculoIRRFSemAcumulo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAM_VALOR_IRRF_SEM_DESCONTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorIRRFSemDesconto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAM_VALOR_IRRF_PERIODO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorIRRFPeriodo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAM_ALIQUOTA_INSS_PATRONAL", Scale = 2, Precision = 5, TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal AliquotaINSSPatronal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAM_VALOR_INSS_PATRONAL", Scale = 2, Precision = 18, TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ValorINSSPatronal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAM_ALIQUOTA_PIS", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaPIS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAM_ALIQUOTA_COFINS", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaCOFINS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracaoTributaria", Column = "PAM_CODIGO_INTEGRACAO_TRIBUTARIA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string CodigoIntegracaoTributaria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeDependentes", Column = "PAM_QUANTIDADE_DEPENDENTES", TypeType = typeof(int), NotNull = false)]
        public virtual int? QuantidadeDependentes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAM_VALOR_POR_DEPENDENTE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorPorDependente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAM_VALOR_TOTAL_DEPENDENTES", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalDependentes { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }


        #region Propriedades Virtuais

        /// <summary>
        /// Consulta baixa de título em até 2 negociações a partir do título original para saber o status
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusFinanceiro", Formula = @"SUBSTRING((SELECT DISTINCT ', ' + 
                                                                                    CASE 
	                                                                                    WHEN T.TIT_STATUS = 1 THEN 'Aberto' 
	                                                                                    WHEN T.TIT_VALOR_PAGO < T.TIT_VALOR_ORIGINAL THEN 
		                                                                                    CASE
			                                                                                    WHEN ISNULL((SELECT COUNT(1) FROM T_TITULO TT WHERE TT.PAM_CODIGO = T.PAM_CODIGO AND TT.TIT_STATUS <> 3 AND TT.TIT_STATUS <> 4), 0) >= 1 THEN 'Renegociado' 
                                                                                                WHEN ISNULL((SELECT COUNT(1) FROM T_TITULO_BAIXA_AGRUPADO TBA JOIN T_TITULO_BAIXA_NEGOCIACAO TBN ON TBN.TIB_CODIGO = TBA.TIB_CODIGO JOIN T_TITULO TT ON TT.TBN_CODIGO = TBN.TBN_CODIGO WHERE TT.TIT_STATUS <> 3 AND TT.TIT_STATUS <> 4 AND TBA.TIT_CODIGO = T.TIT_CODIGO), 0) >= 1 THEN 'Renegociado' 
                                                                                                
                                                                                                WHEN ISNULL(
                                                                                                            (SELECT COUNT(1)
                                                                                                            FROM T_TITULO_BAIXA_NEGOCIACAO TBN
                                                                                                            JOIN T_TITULO TT ON TT.TBN_CODIGO = TBN.TBN_CODIGO
                                                                                                            WHERE TT.TIT_STATUS <> 3 AND TT.TIT_STATUS <> 4
                                                                                                                AND TBN.TIB_CODIGO IN
                                                                                                                (SELECT tituloBaixa.TIB_CODIGO
                                                                                                                    FROM T_TITULO_BAIXA tituloBaixa
                                                                                                                    JOIN T_TITULO_BAIXA_AGRUPADO TBA ON TBA.TIB_CODIGO = tituloBaixa.TIB_CODIGO
                                                                                                                    WHERE tituloBaixa.TIB_SITUACAO <> 4 AND TBA.TIT_CODIGO = T.TIT_CODIGO)), 0) >= 1 THEN 'Renegociado'
                                                                                                WHEN ISNULL(
                                                                                                            (SELECT COUNT(1)
                                                                                                            FROM T_TITULO_BAIXA_NEGOCIACAO TBNN
                                                                                                            JOIN T_TITULO TTT ON TTT.TBN_CODIGO = TBNN.TBN_CODIGO
                                                                                                            WHERE TTT.TIT_STATUS <> 3 AND TTT.TIT_STATUS <> 4
                                                                                                                AND TBNN.TIB_CODIGO IN
                                                                                                                (SELECT tituloBaixa2.TIB_CODIGO
                                                                                                                    FROM T_TITULO_BAIXA tituloBaixa2
                                                                                                                    JOIN T_TITULO_BAIXA_AGRUPADO TBAA ON TBAA.TIB_CODIGO = tituloBaixa2.TIB_CODIGO
                                                                                                                    WHERE tituloBaixa2.TIB_SITUACAO <> 4
                                                                                                                    AND TBAA.TIT_CODIGO IN
                                                                                                                        (SELECT TT.TIT_CODIGO
                                                                                                                        FROM T_TITULO_BAIXA_NEGOCIACAO TBN
                                                                                                                        JOIN T_TITULO TT ON TT.TBN_CODIGO = TBN.TBN_CODIGO
                                                                                                                        WHERE TBN.TIB_CODIGO IN
                                                                                                                            (SELECT tituloBaixa.TIB_CODIGO
                                                                                                                            FROM T_TITULO_BAIXA tituloBaixa
                                                                                                                            JOIN T_TITULO_BAIXA_AGRUPADO TBA ON TBA.TIB_CODIGO = tituloBaixa.TIB_CODIGO
                                                                                                                            WHERE tituloBaixa.TIB_SITUACAO <> 4 AND TBA.TIT_CODIGO = T.TIT_CODIGO)))), 0) >= 1 THEN 'Renegociado'

			                                                                                    ELSE 'Pago' 
		                                                                                    END
	                                                                                    ELSE 'Pago' 
                                                                                    END
                                                                                    FROM T_TITULO T
                                                                                    JOIN T_PAGAMENTO_MOTORISTA_TMS D ON D.PAM_CODIGO = T.PAM_CODIGO
                                                                                    WHERE T.TIT_STATUS <> 4 AND D.PAM_CODIGO = PAM_CODIGO FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string StatusFinanceiro { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Numero.ToString() + " - " + this.Motorista?.Descricao;
            }
        }

        public virtual string DescricaoSituacao
        {
            get { return SituacaoPagamentoMotorista.ObterDescricao(); }
        }

        public virtual string DescricaoEtapa
        {
            get
            {
                if (this.EtapaPagamentoMotorista == ObjetosDeValor.Embarcador.Enumeradores.EtapaPagamentoMotorista.AgAutorizacao)
                    return "Ag. Aprovação";
                else if (this.EtapaPagamentoMotorista == ObjetosDeValor.Embarcador.Enumeradores.EtapaPagamentoMotorista.Iniciada)
                    return "Iniciada";
                else if (this.EtapaPagamentoMotorista == ObjetosDeValor.Embarcador.Enumeradores.EtapaPagamentoMotorista.Integracao)
                    return "Integração";
                else
                    return "";
            }
        }

        public virtual decimal TotalPagamento(bool naoDescontarValorSaldoMotorista)
        {
            if (naoDescontarValorSaldoMotorista)
                return this.Valor;
            else
                return this.Valor - this.SaldoDescontado;
        }

        public virtual bool Equals(PagamentoMotoristaTMS other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        #endregion
    }
}
