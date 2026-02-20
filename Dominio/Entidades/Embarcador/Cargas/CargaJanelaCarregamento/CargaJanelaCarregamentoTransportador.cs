using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CARGA_JANELA_CARREGAMENTO_TRANSPORTADOR", EntityName = "CargaJanelaCarregamentoTransportador", Name = "Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador", NameType = typeof(CargaJanelaCarregamentoTransportador))]
    public class CargaJanelaCarregamentoTransportador : EntidadeBase
    {
        public CargaJanelaCarregamentoTransportador() { }

        #region Propriedades

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "JCT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaJanelaCarregamento", Column = "CJC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CargaJanelaCarregamento CargaJanelaCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaJanelaCarregamentoTransportadorDadosTransporte", Column = "JTD_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CargaJanelaCarregamentoTransportadorDadosTransporte DadosTransporte { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario UsuarioResponsavel { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Transportador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_TERCEIRO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Terceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "JCT_SITUACAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaJanelaCarregamentoTransportador), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaJanelaCarregamentoTransportador Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "JCT_TIPO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCargaJanelaCarregamentoTransportador), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoCargaJanelaCarregamentoTransportador Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HorarioCarregamento", Column = "JCT_HORARIO_CARREGAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? HorarioCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HorarioLiberacao", Column = "JCT_HORARIO_LIBERACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? HorarioLiberacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HorarioLimiteConfirmarCarga", Column = "JCT_HORARIO_LIMITE_CONFIRMAR_CARGA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? HorarioLimiteConfirmarCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Bloqueada", Column = "JCT_BLOQUEADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Bloqueada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "JCT_PRIMEIRO_TRANSPORTADOR_OFERTADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PrimeiroTransportadorOfertado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmailEnviado", Column = "JCT_EMAIL_ENVIADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EmailEnviado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmailTempoAceiteExpirandoEnviado", Column = "JCT_EMAIL_TEMPO_ACEITE_EXPIRANDO_ENVIADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EmailTempoAceiteExpirandoEnviado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "JCT_POSSUI_FRETE_CALCULADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiFreteCalculado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "JCT_FRETE_CALCULADO_COM_PROBLEMAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FreteCalculadoComProblemas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "JCT_PENDENTE_CALCULAR_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PendenteCalcularFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "JCT_VALOR_FRETE_TABELA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorFreteTabela { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFreteTransportador", Column = "JCT_VALOR_FRETE_TRANSPORTADOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorFreteTransportador { get; set; }
        
        [Obsolete("Campo passou para a CargaJanelaCarregamento")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "Rodada", Column = "JCT_NUMERO_RODADA", TypeType = typeof(int), NotNull = false)]
        public virtual int Rodada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ordem", Column = "JCT_ORDEM", TypeType = typeof(int), NotNull = false)]
        public virtual int Ordem { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "JCT_NUMERO_ALTERACOES_HORARIO_REALIZADAS", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroAlteracoesHorarioRealizadas { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "JCT_NUMERO_REJEICOES_MANUAIS", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroRejeicoesManuais { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaFreteCliente", Column = "TFC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Frete.TabelaFreteCliente TabelaFreteCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInteresse", Column = "JCT_DATA_INTERESSE", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInteresse { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCargaContratada", Column = "JCT_DATA_CARGA_CONTRATADA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCargaContratada { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "JCT_OBSERVACAO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string Observacao { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "JCT_MOTIVO_REJEICAO_CARGA", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string MotivoRejeicaoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "JCT_SEM_INTERESSE_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? SemInteresseTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Componentes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_COMPONENTE_CARGA_JANELA_CARREGAMENTO_TRANSPORTADOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "JCT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaJanelaCarregamentoTransportadorComponente", Column = "JTC_CODIGO")]
        public virtual ICollection<CargaJanelaCarregamentoTransportadorComponente> Componentes { get; set; }

        public virtual string Descricao
        {
            get { return this.CargaJanelaCarregamento?.Descricao ?? string.Empty; }
        }

        #endregion Propriedades

        #region Métodos Públicos

        public virtual string ObterCorLinha()
        {
            return Bloqueada ? "#afb8bf" : Tipo.ObterCorLinha();
        }

        public virtual decimal ObterValorFreteTabelaComComponentes()
        {
            if (!PendenteCalcularFrete && PossuiFreteCalculado && !FreteCalculadoComProblemas)
                return ValorFreteTabela + Componentes.Sum(c => c.Valor);

            return 0m;
        }

        public virtual decimal ObterValorFreteTransportador()
        {
            if (PendenteCalcularFrete || (PossuiFreteCalculado && FreteCalculadoComProblemas))
                return 0m;

            if (PossuiFreteCalculado && !FreteCalculadoComProblemas)
                return ValorFreteTabela + Componentes.Sum(c => c.Valor);

            if (ValorFreteTransportador > 0)
                return ValorFreteTransportador;

            return CargaJanelaCarregamento.Carga.ValorFreteAPagar;
        }

        #endregion Métodos Públicos
    }
}
