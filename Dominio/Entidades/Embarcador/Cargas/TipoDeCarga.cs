using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TIPO_DE_CARGA", EntityName = "TipoDeCarga", Name = "Dominio.Entidades.Embarcador.Cargas.TipoDeCarga", NameType = typeof(TipoDeCarga))]
    public class TipoDeCarga : EntidadeBase, IEquatable<TipoDeCarga>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TCG_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoTipoCargaEmbarcador", Column = "TCG_CODIGO_TIPO_CARGA_EMBARCADOR", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string CodigoTipoCargaEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "TCG_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        /// <summary>
        /// Tipo de tempo de descarga. O padrão é por ModeloVeicular, mas pode ser por peso também
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoTempoDescarga", Column = "TCG_TIPO_TEMPO_DESCARGA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTempoDescargaTipoCarga), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTempoDescargaTipoCarga TipoTempoDescarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ModalProposta", Column = "TCG_MODAL_PROPOSTA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalPropostaMultimodal), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalPropostaMultimodal ModalProposta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ControlaTemperatura", Column = "TCG_CONTROLA_TEMPERATURA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ControlaTemperatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExigeVeiculoRastreado", Column = "TCG_EXIGE_VEICULO_RASTREADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExigeVeiculoRastreado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "TCG_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "GRP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pessoas.GrupoPessoas GrupoPessoas { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Pessoa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FaixaTemperatura", Column = "FTE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual FaixaTemperatura FaixaDeTemperatura { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "IdentificacaoMercadoriaKrona", Column = "IMK_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Integracao.IdentificacaoMercadoriaKrona IdentificacaoMercadoriaKrona { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IdentificacaoMercadoriaInfolog", Column = "TCG_IDENTIFICACAO_MERCADORIA_INFOLOG", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string IdentificacaoMercadoriaInfolog { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TCG_NCM", TypeType = typeof(string), Length = 8, NotNull = false)]
        public virtual string NCM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TCG_NBS", TypeType = typeof(string), Length = 8, NotNull = false)]
        public virtual string NBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiCargaPerigosa", Column = "TCG_POSSUI_CARGA_PERIGOSA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? PossuiCargaPerigosa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ClasseONU", Column = "TCG_CLASSE_ONU", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string ClasseONU { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SequenciaONU", Column = "TCG_SEQUENCIA_ONU", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string SequenciaONU { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoPsnONU", Column = "TCG_CODIGO_PSN_ONU", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string CodigoPsnONU { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoONU", Column = "TCG_OBSERVACAO_ONU", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string ObservacaoONU { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IndisponivelMontagemCarregamento", Column = "TCG_INDISPONIVEL_MONTAGEM_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IndisponivelMontagemCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BloquearLiberacaoParaTransportadores", Column = "TCG_BLOQUEAR_LIBERACAO_PARA_TRANSPORTADORES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearLiberacaoParaTransportadores { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoPermitirFornecedorEscolherNoAgendamento", Column = "TCG_NAO_PERMITIR_FORNECEDOR_ESCOLHER_NO_AGENDAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirFornecedorEscolherNoAgendamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoValidarDataCheckList", Column = "TCG_NAO_VALIDAR_DATA_CHECKLIST", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoValidarDataCheckList { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Paletizado", Column = "TCG_PALETIZADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Paletizado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoCargaMDFe", Column = "TCG_TIPO_CARGA_MDFE", TypeType = typeof(Dominio.Enumeradores.TipoCargaMDFe), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoCargaMDFe? TipoCargaMDFe { get; set; }

        /// <summary>
        /// Atributos a ser utilizado para Criar um tipo de carga principal, onde novos tipos de cargas poderam estar relacionados a ela.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "Principal", Column = "TCG_PRINCIPAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Principal { get; set; }

        /// <summary>
        /// Identificador do Tipo de carga principal.
        /// Ex: CARGA SECA PRINC
        ///           SECA 01
        ///           SECA 02
        ///     REFRIGERADA
        ///             PIZZA
        ///             RERIGERADA 02.
        ///             REFRIGERANTE 
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeCarga", Column = "TCG_CODIGO_PRINCIPAL", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoDeCarga TipoCargaPrincipal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TCG_PRODUTO_PREDOMINANTE", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string ProdutoPredominante { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TCG_PRIORIDADE_CARGA", TypeType = typeof(int), NotNull = false)]
        public virtual int PrioridadeCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TCG_CODIGO_ERP", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string CodigoERP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValidarLicencasNCM", Column = "TCG_VALIDAR_LICENCAS_NCM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidarLicencasNCM { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "TiposLicenca", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TIPO_CARGA_TIPO_LICENCA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TCG_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Licenca", Column = "LIC_CODIGO")]
        public virtual ICollection<Configuracoes.Licenca> TiposLicenca { get; set; }

        [Obsolete("Está será removida de acordo com a tarefa #51351, o e-mail de aviso foi encaminhado dia 10/10, FAVOR NÃO USAR A MESMA!")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoPermitirEmissaoDeCargasEtapaFreteComEsteTipo", Column = "TCG_NAO_PERMITIR_EMISSAO_DE_CARGAS_ETAPA_FRETE_COM_ESTE_TIPO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirEmissaoDeCargasEtapaFreteComEsteTipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoNaturezaCIOT", Column = "TCG_CODIGO_NATUREZA_CIOT", TypeType = typeof(int), NotNull = false)]
        public virtual int CodigoNaturezaCIOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoDeCargaEFrete", Column = "TCG_TIPO_DE_CARGA_E_FRETE", TypeType = typeof(int), NotNull = false)]
        public virtual int TipoDeCargaEFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BloquearMontagemCargaComPedidoProvisorio", Column = "TCG_BLOQUEAR_MONTAGEM_CARGA_COM_PEDIDO_PROVISORIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearMontagemCargaComPedidoProvisorio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ImprimirTabelaTemperaturaNoVersoCTe", Column = "TCG_IMPRIMIR_TABELA_TEMPERATURA_VERSO_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ImprimirTabelaTemperaturaNoVersoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnviarEmailConfirmacaoAgendamentoQuandoSituacaoAgendamentoForFinalizado", Column = "TCG_ENVIAR_EMAIL_CONFIRMACAO_AGENDAMENTO_QUANDO_SITUACAO_AGENDAMENTO_FOR_FINALIZADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarEmailConfirmacaoAgendamentoQuandoSituacaoAgendamentoForFinalizado { get; set; }

        public virtual bool Equals(TipoDeCarga other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        public virtual string DescricaoAtivo
        {
            get
            {
                if (this.Ativo)
                    return "Ativo";
                else
                    return "Inativo";
            }
        }
    }
}
