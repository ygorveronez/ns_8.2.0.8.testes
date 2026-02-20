using System;
using System.Collections.Generic;
using System.Linq;

namespace Dominio.Entidades.Embarcador.Chamados
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CHAMADO_TMS", EntityName = "ChamadoTMS", Name = "Dominio.Entidades.Embarcador.Chamados.ChamadoTMS", NameType = typeof(ChamadoTMS))]
    public class ChamadoTMS : EntidadeBase, Interfaces.Embarcador.Entidade.IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CHT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CHT_NUMERO", NotNull = true)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CHT_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoChamadoTMS), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoChamadoTMS Situacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MotivoChamado", Column = "MCH_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual MotivoChamado MotivoChamado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_MOTORISTA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Motorista { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Autor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_RESPONSAVEL", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Responsavel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CHT_DATA_CRICAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataCriacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CHT_DATA_FINALIZACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFinalizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CelularMotorista", Column = "CHT_CELULAR_MOTORISTA", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string CelularMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroOrdemColeta", Column = "CHT_NUMERO_ORDEM_COLETA", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string NumeroOrdemColeta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CHT_FORMA_COBRANCA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.FormaCobrancaChamado), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.FormaCobrancaChamado FormaCobranca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeFormaCobranca", Column = "CHT_QUANTIDADE_FORMA_COBRANCA", TypeType = typeof(decimal), NotNull = false, Scale = 4, Precision = 18)]
        public virtual decimal QuantidadeFormaCobranca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorUnitario", Column = "CHT_VALOR_UNITARIO", TypeType = typeof(decimal), NotNull = false, Scale = 4, Precision = 18)]
        public virtual decimal ValorUnitario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotal", Column = "CHT_VALOR_TOTAL", TypeType = typeof(decimal), NotNull = false, Scale = 4, Precision = 18)]
        public virtual decimal ValorTotal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "CHT_OBSERVACAO", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string Observacao { get; set; }

        #region Forma de Pagamento

        [NHibernate.Mapping.Attributes.Property(0, Name = "MotoristaPagouSemAutorizacao", Column = "CHT_MOTORISTA_PAGOU_SEM_AUTORIZACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool MotoristaPagouSemAutorizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GestorLogisticaAutorizouPagamento", Column = "CHT_GESTOR_LOGISTICA_AUTORIZOU_PAGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GestorLogisticaAutorizouPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CHT_FORMA_PAGAMENTO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.FormaPagamentoChamado), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.FormaPagamentoChamado FormaPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorAdiantamento", Column = "CHT_VALOR_ADIANTAMENTO", TypeType = typeof(decimal), NotNull = false, Scale = 2, Precision = 15)]
        public virtual decimal ValorAdiantamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_OUTRO_MOTORISTA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario OutroMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeTerceiro", Column = "CHT_NOME_TERCEIRO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string NomeTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CnpjCpfTerceiro", Column = "CHT_CNPJ_CPF_TERCEIRO", TypeType = typeof(string), Length = 14, NotNull = false)]
        public virtual string CnpjCpfTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CHT_TIPO_CONTA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco TipoContaBanco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Agencia", Column = "CHT_AGENCIA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Agencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroConta", Column = "CHT_NUMERO_CONTA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string NumeroConta { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Banco", Column = "BCO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Banco Banco { get; set; }

        #endregion

        #region Etapa 2

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroOcorrencia", Column = "CHT_NUMERO_OCORRENCIA", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string NumeroOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CHA_DATA_RETORNO_AUTORIZACAO_CLIENTE", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataRetornoAutorizacaoCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CHT_SITUACAO_AUTORIZACAO_CLIENTE", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacaoClienteChamado), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacaoClienteChamado? SituacaoAutorizacaoCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorAprovacaoParcial", Column = "CHT_VALOR_APROVACAO_PARCIAL", TypeType = typeof(decimal), NotNull = false, Scale = 2, Precision = 15)]
        public virtual decimal ValorAprovacaoParcial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoAutorizacaoCliente", Column = "CHT_OBSERVACAO_AUTORIZACAO_CLIENTE", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string ObservacaoAutorizacaoCliente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ContatoGrupoPessoa", Column = "CGP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pessoas.ContatoGrupoPessoa ContatoGrupoPessoa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AssuntoEmail", Column = "CHT_ASSUNTO_EMAIL", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string AssuntoEmail { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CorpoEmail", Column = "CHT_CORPO_EMAIL", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string CorpoEmail { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MensagemOrientacaoMotorista", Column = "CHT_MENSAGEM_ORIENTACAO_MOTORISTA", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string MensagemOrientacaoMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CHA_DATA_ULTIMO_ENVIO_MENSAGEM_ORIENTACAO_MOTORISTA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataUltimoEnvioMensagemOrientacaoMotorista { get; set; }

        #endregion

        #region Etapa 3

        [NHibernate.Mapping.Attributes.Property(0, Column = "CHT_DATA_DOCUMENTO_RECEBIDO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataDocumentoRecebido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorRecibo", Column = "CHT_VALOR_RECIBO", TypeType = typeof(decimal), NotNull = false, Scale = 2, Precision = 15)]
        public virtual decimal ValorRecibo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroDocumento", Column = "CHT_NUMERO_DOCUMENTO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string NumeroDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoDocumento", Column = "CHT_OBSERVACAO_DOCUMENTO", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string ObservacaoDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NovoValorAutorizado", Column = "CHT_NOVO_VALOR_AUTORIZADO", TypeType = typeof(decimal), NotNull = false, Scale = 2, Precision = 15)]
        public virtual decimal NovoValorAutorizado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CHT_FORMA_AUTORIZACAO_PAGAMENTO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.FormaAutorizacaoPagamentoChamado), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.FormaAutorizacaoPagamentoChamado? FormaAutorizacaoPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "JustificativaAutorizacao", Column = "CHT_JUSTIFICATIVA_AUTORIZACAO", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string JustificativaAutorizacao { get; set; }

        #endregion

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Anexos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CHAMADO_TMS_ANEXO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CHT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ChamadoTMSAnexo", Column = "ANX_CODIGO")]
        public virtual IList<ChamadoTMSAnexo> Anexos { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AnexosEmail", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CHAMADO_TMS_ANEXO_EMAIL")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CHT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ChamadoTMSAnexoEmail", Column = "ANX_CODIGO")]
        public virtual IList<ChamadoTMSAnexoEmail> AnexosEmail { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AnexosAdiantamentoMotorista", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CHAMADO_TMS_ANEXO_ADIANTAMENTO_MOTORISTA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CHT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ChamadoTMSAnexoAdiantamentoMotorista", Column = "ANX_CODIGO")]
        public virtual IList<ChamadoTMSAnexoAdiantamentoMotorista> AnexosAdiantamentoMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AnexosDocumentoAnalise", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CHAMADO_TMS_ANEXO_DOCUMENTO_ANALISE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CHT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ChamadoTMSAnexoDocumentoAnalise", Column = "ANX_CODIGO")]
        public virtual IList<ChamadoTMSAnexoDocumentoAnalise> AnexosDocumentoAnalise { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "CTes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CHAMADO_TMS_CTE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CHT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ChamadoTMSCTe", Column = "CHC_CODIGO")]
        public virtual IList<ChamadoTMSCTe> CTes { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "CustosAdicional", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CHAMADO_TMS_CUSTO_ADICIONAL")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CHT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ChamadoTMSCustoAdicional", Column = "CHU_CODIGO")]
        public virtual IList<ChamadoTMSCustoAdicional> CustosAdicional { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Chapas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CHAMADO_TMS_CHAPA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CHT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ChamadoTMSChapa", Column = "CHP_CODIGO")]
        public virtual IList<ChamadoTMSChapa> Chapas { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AdiantamentosMotorista", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CHAMADO_TMS_ADIANTAMENTO_MOTORISTA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CHT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ChamadoTMSAdiantamentoMotorista", Column = "CHM_CODIGO")]
        public virtual IList<ChamadoTMSAdiantamentoMotorista> AdiantamentosMotorista { get; set; }

        public virtual string Descricao
        {
            get
            {
                return Numero.ToString();
            }
        }

        public virtual decimal ValorTotalDescarga
        {
            get
            {
                return CTes.Sum(c => c.ValorDescarga + c.CTe.ComponentesPrestacao.Sum(o => o.ValorDescarga));
            }
        }
    }
}
