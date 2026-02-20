using System;

namespace Dominio.Entidades.Embarcador.Filiais
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_GESTAO_PATIO", EntityName = "ConfiguracaoGestaoPatio", Name = "Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio", NameType = typeof(ConfiguracaoGestaoPatio))]
    public class ConfiguracaoGestaoPatio : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CGP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        #region Propriedades - Descrição das Etapas

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_DESCRICAO_INFORMAR_DOCA_CARREGAMENTO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string InformarDocaCarregamentoDescricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_DESCRICAO_MONTAGEM_CARGA", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string MontagemCargaDescricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_DESCRICAO_CHEGADA_VEICULO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string ChegadaVeiculoDescricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_DESCRICAO_GUARITA_ENTRADA", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string GuaritaEntradaDescricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_DESCRICAO_CHECK_LIST", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string CheckListDescricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_DESCRICAO_TRAVA_CHAVE", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string TravaChaveDescricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_DESCRICAO_EXPEDICAO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string ExpedicaoDescricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_DESCRICAO_LIBERA_CHAVE", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string LiberaChaveDescricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_DESCRICAO_CONTROLAR_FATURAMENTO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string FaturamentoDescricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_DESCRICAO_GUARITA_SAIDA", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string GuaritaSaidaDescricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_DESCRICAO_POSICAO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string PosicaoDescricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_DESCRICAO_CHEGADA_LOJA", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string ChegadaLojaDescricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_DESCRICAO_DESLOCAMENTO_PATIO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string DeslocamentoPatioDescricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_DESCRICAO_DOCUMENTO_FISCAL", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string DocumentoFiscalDescricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_DESCRICAO_DOCUMENTOS_TRANSPORTE", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string DocumentosTransporteDescricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_DESCRICAO_SAIDA_LOJA", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string SaidaLojaDescricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_DESCRICAO_FIM_VIAGEM", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string FimViagemDescricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_DESCRICAO_ENTREGAS", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string EntregasDescricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_DESCRICAO_INICIO_CARREGAMENTO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string InicioCarregamentoDescricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_DESCRICAO_INICIO_DESCARREGAMENTO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string InicioDescarregamentoDescricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_DESCRICAO_FIM_CARREGAMENTO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string FimCarregamentoDescricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_DESCRICAO_FIM_DESCARREGAMENTO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string FimDescarregamentoDescricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_DESCRICAO_INICIO_HIGIENIZACAO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string InicioHigienizacaoDescricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_DESCRICAO_FIM_HIGIENIZACAO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string FimHigienizacaoDescricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_DESCRICAO_SEPARACAO_MERCADORIA", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string SeparacaoMercadoriaDescricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_DESCRICAO_AVALIACAO_DESCARGA", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string AvaliacaoDescargaDescricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_DESCRICAO_SOLICITACAO_VEICULO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string SolicitacaoVeiculoDescricao { get; set; }

        #endregion Propriedades - Descrição das Etapas

        #region Propriedades - Permissão de QR Code das Etapas

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_INFORMAR_DOCA_CARREGAMENTO_PERMITE_QR_CODE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool InformarDocaCarregamentoPermiteQRCode { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_MONTAGEM_CARGA_PERMITE_QR_CODE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool MontagemCargaPermiteQRCode { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_CHEGADA_VEICULO_PERMITE_QR_CODE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ChegadaVeiculoPermiteQRCode { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_GUARITA_ENTRADA_PERMITE_QR_CODE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GuaritaEntradaPermiteQRCode { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_CHECK_LIST_PERMITE_QR_CODE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CheckListPermiteQRCode { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_TRAVA_CHAVE_PERMITE_QR_CODE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool TravaChavePermiteQRCode { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_EXPEDICAO_PERMITE_QR_CODE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExpedicaoPermiteQRCode { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_LIBERA_CHAVE_PERMITE_QR_CODE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LiberaChavePermiteQRCode { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_FATURAMENTO_PERMITE_QR_CODE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FaturamentoPermiteQRCode { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_GUARITA_SAIDA_PERMITE_QR_CODE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GuaritaSaidaPermiteQRCode { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_POSICAO_PERMITE_QR_CODE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PosicaoPermiteQRCode { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_CHEGADA_LOJA_PERMITE_QR_CODE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ChegadaLojaPermiteQRCode { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_DESLOCAMENTO_PATIO_PERMITE_QR_CODE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DeslocamentoPatioPermiteQRCode { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_DOCUMENTO_FISCAL_PERMITE_QR_CODE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DocumentoFiscalPermiteQRCode { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_DOCUMENTOS_TRANSPORTE_PERMITE_QR_CODE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DocumentosTransportePermiteQRCode { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_SAIDA_LOJA_PERMITE_QR_CODE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SaidaLojaPermiteQRCode { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_FIM_VIAGEM_PERMITE_QR_CODE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FimViagemPermiteQRCode { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_INICIO_CARREGAMENTO_PERMITE_QR_CODE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool InicioCarregamentoPermiteQRCode { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_INICIO_DESCARREGAMENTO_PERMITE_QR_CODE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool InicioDescarregamentoPermiteQRCode { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_FIM_CARREGAMENTO_PERMITE_QR_CODE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FimCarregamentoPermiteQRCode { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_FIM_DESCARREGAMENTO_PERMITE_QR_CODE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FimDescarregamentoPermiteQRCode { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_INICIO_HIGIENIZACAO_PERMITE_QR_CODE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool InicioHigienizacaoPermiteQRCode { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_FIM_HIGIENIZACAO_PERMITE_QR_CODE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FimHigienizacaoPermiteQRCode { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_SEPARACAO_MERCADORIA_PERMITE_QR_CODE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SeparacaoMercadoriaPermiteQRCode { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_SOLICITACAO_VEICULO_PERMITE_QR_CODE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SolicitacaoVeiculoPermiteQRCode { get; set; }

        #endregion Propriedades - Permissão de QR Code das Etapas

        #region Propriedades - Permissão de Voltar das Etapas

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_INFORMAR_DOCA_CARREGAMENTO_PERMITE_VOLTAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool InformarDocaCarregamentoPermiteVoltar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_MONTAGEM_CARGA_PERMITE_VOLTAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool MontagemCargaPermiteVoltar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_CHEGADA_VEICULO_PERMITE_VOLTAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ChegadaVeiculoPermiteVoltar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_GUARITA_ENTRADA_PERMITE_VOLTAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GuaritaEntradaPermiteVoltar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_CHECK_LIST_PERMITE_VOLTAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CheckListPermiteVoltar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_CHECK_LIST_UTILIZAR_CATEGORIA_REBOQUE_CONFORME_MODELO_VEICULAR_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarCategoriaDeReboqueConformeModeloVeicularCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_TRAVA_CHAVE_PERMITE_VOLTAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool TravaChavePermiteVoltar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_EXPEDICAO_PERMITE_VOLTAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExpedicaoPermiteVoltar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_LIBERA_CHAVE_PERMITE_VOLTAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LiberaChavePermiteVoltar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_FATURAMENTO_PERMITE_VOLTAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FaturamentoPermiteVoltar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_GUARITA_SAIDA_PERMITE_VOLTAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GuaritaSaidaPermiteVoltar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GuaritaSaidaTipoIntegracao", Column = "CGP_GUARITA_SAIDA_TIPO_INTEGRACAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao GuaritaSaidaTipoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_POSICAO_PERMITE_VOLTAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PosicaoPermiteVoltar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_CHEGADA_LOJA_PERMITE_VOLTAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ChegadaLojaPermiteVoltar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_DESLOCAMENTO_PATIO_PERMITE_VOLTAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DeslocamentoPatioPermiteVoltar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_DOCUMENTO_FISCAL_PERMITE_VOLTAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DocumentoFiscalPermiteVoltar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_DOCUMENTOS_TRANSPORTE_PERMITE_VOLTAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DocumentosTransportePermiteVoltar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_SAIDA_LOJA_PERMITE_VOLTAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SaidaLojaPermiteVoltar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_FIM_VIAGEM_PERMITE_VOLTAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FimViagemPermiteVoltar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_INICIO_CARREGAMENTO_PERMITE_VOLTAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool InicioCarregamentoPermiteVoltar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_INICIO_DESCARREGAMENTO_PERMITE_VOLTAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool InicioDescarregamentoPermiteVoltar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_FIM_CARREGAMENTO_PERMITE_VOLTAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FimCarregamentoPermiteVoltar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_FIM_DESCARREGAMENTO_PERMITE_VOLTAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FimDescarregamentoPermiteVoltar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_INICIO_HIGIENIZACAO_PERMITE_VOLTAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool InicioHigienizacaoPermiteVoltar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_FIM_HIGIENIZACAO_PERMITE_VOLTAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FimHigienizacaoPermiteVoltar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_SEPARACAO_MERCADORIA_PERMITE_VOLTAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SeparacaoMercadoriaPermiteVoltar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_SOLICITACAO_VEICULO_PERMITE_VOLTAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SolicitacaoVeiculoPermiteVoltar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_AVALIACAO_DESCARGA_PERMITE_VOLTAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AvaliacaoDescargaPermiteVoltar { get; set; }

        #endregion Propriedades - Permissão de Voltar das Etapas

        #region Propriedades - Notificações no app

        [NHibernate.Mapping.Attributes.Property(0, Column = "INFORMAR_DOCA_CARREGAMENTO_NOTIFICAR_MOTORISTA_APP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool InformarDocaCarregamentoNotificarMotoristaApp { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MONTAGEM_CARGA_NOTIFICAR_MOTORISTA_APP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool MontagemCargaNotificarMotoristaApp { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CHEGADA_VEICULO_NOTIFICAR_MOTORISTA_APP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ChegadaVeiculoNotificarMotoristaApp { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MONTAGEM_CARGA_PERMITE_GERAR_ATENDIMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool MontagemCargaPermiteGerarAtendimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GUARITA_ENTRADA_NOTIFICAR_MOTORISTA_APP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GuaritaEntradaNotificarMotoristaApp { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CHECK_LIST_NOTIFICAR_MOTORISTA_APP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CheckListNotificarMotoristaApp { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TRAVA_CHAVE_NOTIFICAR_MOTORISTA_APP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool TravaChaveNotificarMotoristaApp { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TRAVA_CHAVE_PERMITE_GERAR_ATENDIMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool TravaChavePermiteGerarAtendimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "EXPEDICAO_NOTIFICAR_MOTORISTA_APP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExpedicaoNotificarMotoristaApp { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "LIBERA_CHAVE_NOTIFICAR_MOTORISTA_APP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LiberaChaveNotificarMotoristaApp { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FATURAMENTO_NOTIFICAR_MOTORISTA_APP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FaturamentoNotificarMotoristaApp { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FATURAMENTO_PERMITE_IMPRIMIR_CAPA_VIAGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FaturamentoPermiteImprimirCapaViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FATURAMENTO_MENSAGEM_CAPA_VIAGEM", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string FaturamentoMensagemCapaViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "POSICAO_NOTIFICAR_MOTORISTA_APP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PosicaoNotificarMotoristaApp { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CHEGADA_LOJA_NOTIFICAR_MOTORISTA_APP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ChegadaLojaNotificarMotoristaApp { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DESLOCAMENTO_PATIO_NOTIFICAR_MOTORISTA_APP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DeslocamentoPatioNotificarMotoristaApp { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DOCUMENTO_FISCAL_NOTIFICAR_MOTORISTA_APP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DocumentoFiscalNotificarMotoristaApp { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DOCUMENTOS_TRANSPORTE_NOTIFICAR_MOTORISTA_APP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DocumentosTransporteNotificarMotoristaApp { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SAIDA_LOJA_NOTIFICAR_MOTORISTA_APP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SaidaLojaNotificarMotoristaApp { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FIM_VIAGEM_NOTIFICAR_MOTORISTA_APP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FimViagemNotificarMotoristaApp { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "INICIO_HIGIENIZACAO_NOTIFICAR_MOTORISTA_APP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool InicioHigienizacaoNotificarMotoristaApp { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FIM_HIGIENIZACAO_NOTIFICAR_MOTORISTA_APP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FimHigienizacaoNotificarMotoristaApp { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "INICIO_CARREGAMENTO_NOTIFICAR_MOTORISTA_APP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool InicioCarregamentoNotificarMotoristaApp { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "INICIO_DESCARREGAMENTO_NOTIFICAR_MOTORISTA_APP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool InicioDescarregamentoNotificarMotoristaApp { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "InicioCarregamentoTipoIntegracao", Column = "INICIO_CARREGAMENTO_TIPO_INTEGRACAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao InicioCarregamentoTipoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FIM_CARREGAMENTO_NOTIFICAR_MOTORISTA_APP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FimCarregamentoNotificarMotoristaApp { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FimCarregamentoTipoIntegracao", Column = "FIM_CARREGAMENTO_TIPO_INTEGRACAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao FimCarregamentoTipoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FIM_DESCARREGAMENTO_NOTIFICAR_MOTORISTA_APP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FimDescarregamentoNotificarMotoristaApp { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SEPARACAO_MERCADORIA_NOTIFICAR_MOTORISTA_APP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SeparacaoMercadoriaNotificarMotoristaApp { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SOLICITACAO_VEICULO_NOTIFICAR_MOTORISTA_APP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SolicitacaoVeiculoNotificarMotoristaApp { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GUARITA_SAIDA_NOTIFICAR_MOTORISTA_APP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GuaritaSaidaNotificarMotoristaApp { get; set; }

        #endregion Propriedades - Notificações no app

        [Obsolete("Configuração passou a ser utilizada por filial.", true)]
        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_GUARITA_ENTRADA_PERMITE_INFORMACOES_PESAGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GuaritaEntradaPermiteInformacoesPesagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GuaritaEntradaAction", Column = "CGP_GUARITA_ENTRADA_ACTION", TypeType = typeof(string), NotNull = false)]
        virtual public string GuaritaEntradaAction { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GuaritaEntradaTipoIntegracao", Column = "CGP_GUARITA_ENTRADA_TIPO_INTEGRACAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao GuaritaEntradaTipoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_SOLICITACAO_VEICULO_PERMITE_ENVIO_SMS_MOTORISTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SolicitacaoVeiculoPermiteEnvioSMSMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_CHECK_LIST_PERMITE_FINALIZAR_SEM_PREENCHER", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CheckListPermiteSalvarSemPreencher { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ChecklistTipoIntegracao", Column = "CGP_CHECKLIST_TIPO_INTEGRACAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao ChecklistTipoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_CHEGADA_VEICULO_PERMITE_AVANCAR_AUTOMATICAMENTE_APOS_INFORMAR_DADOS_TRANSPORTE_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ChegadaVeiculoPermiteAvancarAutomaticamenteAposInformarDadosTransporteCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_CHEGADA_VEICULO_PERMITE_INFORMAR_COM_ETAPA_BLOQUEADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ChegadaVeiculoPermiteInformarComEtapaBloqueada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ChegadaVeiculoAction", Column = "CGP_CHEGADA_VEICULO_ACTION", TypeType = typeof(string), NotNull = false)]
        virtual public string ChegadaVeiculoAction { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ChegadaVeiculoTipoIntegracao", Column = "CGP_CHEGADA_VEICULO_TIPO_INTEGRACAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao ChegadaVeiculoTipoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_FATURAMENTO_PERMITE_AVANCAR_AUTOMATICAMENTE_APOS_GERAR_DOCUMENTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FaturamentoPermiteAvancarAutomaticamenteAposGerarDocumentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_DOCUMENTOS_TRANSPORTE_PERMITE_AVANCAR_AUTOMATICAMENTE_APOS_GERAR_DOCUMENTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DocumentosTransportePermiteAvancarAutomaticamenteAposGerarDocumentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DocumentosTransporteTipoIntegracao", Column = "CGP_DOCUMENTOS_TRANSPORTE_TIPO_INTEGRACAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao DocumentosTransporteTipoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_FATURAMENTO_PERMITE_SOLICITAR_NOTAS_FISCAIS_ETAPA_BLOQUEADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FaturamentoPermiteSolicitarNotasFiscaisEtapaBloqueada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_INFORMAR_DOCA_CARREGAMENTO_UTILIZAR_LOCAL_CARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool InformarDocaCarregamentoUtilizarLocalCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_INICIO_HIGIENIZACAO_PERMITE_AVANCAR_AUTOMATICAMENTE_COM_VEICULOS_HIGIENIZADOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool InicioHigienizacaoPermiteAvancarAutomaticamenteComVeiculosHigienizados { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_FIM_HIGIENIZACAO_PERMITE_AVANCAR_AUTOMATICAMENTE_COM_VEICULOS_HIGIENIZADOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FimHigienizacaoPermiteAvancarAutomaticamenteComVeiculosHigienizados { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_NAO_PERMITIR_INFORMAR_MAIS_DE_UM_VEICULO_POR_VEZ_NA_DOCA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoPermitirInformarMaisDeUmVeiculoPorVezNaDoca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_EXIBIR_COMPROVANTE_SAIDA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirComprovanteSaida { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_INICIAR_VIAGEM_SEM_GUARITA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IniciarViagemSemGuarita { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_HABILITAR_OBSERVACAO_ETAPA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarObservacaoEtapa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_EXIBIR_DETALHES_IDENTIFICACAO_FLUXO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirDetalhesIdentificacaoFluxo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_OCULTAR_FLUXO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool OcultarFluxoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_DOCA_DETALHADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DocaDetalhada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_OCULTAR_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool OcultarTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_HABILITAR_PRE_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarPreCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_EXIBIR_TEMPO_PREVISTO_E_REALIZADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirTempoPrevistoERealizado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_PERMITIR_REJEICAO_FLUXO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirRejeicaoFluxo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_LISTAR_CARGAS_CANCELADAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ListarCargasCanceladas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_IDENTIFICACAO_FLUXO_EXIBIR_ORIGEM_X_DESTINOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IdentificacaoFluxoExibirOrigemXDestinos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_IDENTIFICACAO_FLUXO_EXIBIR_TIPO_OPERACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IdentificacaoFluxoExibirTipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_IDENTIFICACAO_FLUXO_EXIBIR_CODIGO_INTEGRACAO_FILIAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IdentificacaoFluxoExibirCodigoIntegracaoFilial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_EXIBIR_SIGLA_FILIAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ExibirSiglaFilial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_IDENTIFICACAO_FLUXO_EXIBIR_MODELO_VEICULAR_CARGA_VEICULO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IdentificacaoFluxoExibirModeloVeicularCargaVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_SEMPRE_EXIBIR_PREVISTO_X_REALIZADO_E_DIFERENCA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SempreExibirPrevistoXRealizadoEDiferenca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_SEMPRE_ATUALIZAR_DATA_PREVISTA_AO_ALTERAR_HORARIO_CARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SempreAtualizarDataPrevistaAoAlterarHorarioCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_VIEW_FLUXO_PATIO_TABELADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ViewFluxoPatioTabelado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Macro", Column = "MCR_MACRO_INICIO_VIAGEM", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Veiculos.Macro MacroInicioViagem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Macro", Column = "MCR_MACRO_CHEGADA_DESTINATARIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Veiculos.Macro MacroChegadaDestinatario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Macro", Column = "MCR_MACRO_SAIDA_DESTINATARIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Veiculos.Macro MacroSaidaDestinatario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Macro", Column = "MCR_MACRO_FIM_VIAGEM", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Veiculos.Macro MacroFimViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_RELATORIO_FLUXO_HORARIO_QUANTIDADE_BAIXA", TypeType = typeof(int), NotNull = false)]
        public virtual int RelatorioFluxoHorarioQuantidadeBaixa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_RELATORIO_FLUXO_HORARIO_QUANTIDADE_NORMAL", TypeType = typeof(int), NotNull = false)]
        public virtual int RelatorioFluxoHorarioQuantidadeNormal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_RELATORIO_FLUXO_HORARIO_QUANTIDADE_ALTA", TypeType = typeof(int), NotNull = false)]
        public virtual int RelatorioFluxoHorarioQuantidadeAlta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_VISUALIZAR_PLACA_REBOQUE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool VisualizarPlacaReboque { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_VISUALIZAR_PLACA_TRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool VisualizarPlacaTracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_PERMITE_CANCELAR_FLUXO_PATIO_ATUAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteCancelarFluxoPatioAtual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_AVANCAR_CARGA_AGRUPADA_APENAS_COM_AS_CARGAS_FILHAS_AVANCADAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AvancarCargaAgrupadaApenasComAsCargasFilhasAvancadas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_GERAR_OCORRENCIA_PEDIDO_ETAPA_DOCA_CARREGAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarOcorrenciaPedidoEtapaDocaCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_INICIAR_FLUXO_PATIO_SOMENTE_COM_CARREGAMENTO_AGENDADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IniciarFluxoPatioSomenteComCarregamentoAgendado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeOcorrenciaDeCTe", Column = "OCO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.TipoDeOcorrenciaDeCTe TipoDeOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_OBRIGATORIO_INFORMAR_DATA_INCIAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ObrigatorioInformarDataInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_INTEGRAR_FLUXO_PATIO_WMS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegrarFluxoPatioWMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_UTILIZAR_FLUXO_PATIO_CARGA_CANCELADA_AO_REENVIAR_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarFluxoPatioCargaCanceladaAoReenviarCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_MONTAGEM_CARGA_CODIGO_CONTROLE", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string MontagemCargaCodigoControle { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_CHEGADA_VEICULO_PERMITE_GERAR_ATENDIMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteGerarAtendimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_DOCUMENTO_FISCAL_PERMITE_AVANCAR_AUTOMATICAMENTE_APOS_NOTAS_FISCAIS_INSERIDAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DocumentoFiscalPermiteAvancarAutomaticamenteAposNotasFiscaisInseridas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_GUARITA_SAIDA_INICIAR_VIAGEM_CONTROLE_ENTREGA_FINALIZAR_ETAPA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GuaritaSaidaIniciarViagemControleEntregaAoFinalizarEtapa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_MONTAGEM_CARGA_PERMITE_ANTECIPAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool MontagemCargaPermiteAntecipar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_CHEGADA_VEICULO_PERMITE_ANTECIPAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ChegadaVeiculoPermiteAntecipar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_GUARITA_ENTRADA_PERMITE_ANTECIPAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GuaritaEntradaPermiteAntecipar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_CHECKLIST_PERMITE_ANTECIPAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ChecklistPermiteAntecipar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_INFORMAR_DOCA_PERMITE_ANTECIPAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool InformarDocaPermiteAntecipar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "InformarDocaCarregamentoTipoIntegracao", Column = "CGP_INFORMAR_DOCA_CARREGAMENTO_TIPO_INTEGRACAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao InformarDocaCarregamentoTipoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SolicitacaoVeiculoTipoIntegracao", Column = "CGP_SOLICITACAO_VEICULO_TIPO_INTEGRACAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao SolicitacaoVeiculoTipoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_FIM_CARREGAMENTO_PERMITE_AVANCAR_SOMENTE_DADOS_TRANSPORTE_INFORMADOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FimCarregamentoPermiteAvancarSomenteDadosTransporteInformados { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_UTILIZAR_DATA_PREVISTA_ETAPA_ATUAL_ATIVAR_ALERTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarDataPrevistaEtapaAtualAtivarAlerta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CGP_GERAR_FLUXO_DESTINO_ANTES_FINALIZAR_ORIGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarFluxoDestinoAntesFinalizarOrigem { get; set; }


        public virtual string Descricao
        {
            get { return "Configuração Gestão Patio"; }
        }

        #region Métodos Públicos

        public virtual bool IsNotificarMotoristaApp(ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio etapaFluxo)
        {
            switch (etapaFluxo)
            {
                case ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.CheckList: return CheckListNotificarMotoristaApp;
                case ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.ChegadaLoja: return ChegadaLojaNotificarMotoristaApp;
                case ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.ChegadaVeiculo: return ChegadaVeiculoNotificarMotoristaApp;
                case ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.DeslocamentoPatio: return DeslocamentoPatioNotificarMotoristaApp;
                case ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.DocumentoFiscal: return DocumentoFiscalNotificarMotoristaApp;
                case ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.DocumentosTransporte: return DocumentosTransporteNotificarMotoristaApp;
                case ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.Expedicao: return ExpedicaoNotificarMotoristaApp;
                case ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.Faturamento: return FaturamentoNotificarMotoristaApp;
                case ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.FimCarregamento: return FimCarregamentoNotificarMotoristaApp;
                case ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.FimDescarregamento: return FimDescarregamentoNotificarMotoristaApp;
                case ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.FimHigienizacao: return FimHigienizacaoNotificarMotoristaApp;
                case ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.FimViagem: return FimViagemNotificarMotoristaApp;
                case ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.Guarita: return GuaritaEntradaNotificarMotoristaApp;
                case ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.InformarDoca: return InformarDocaCarregamentoNotificarMotoristaApp;
                case ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.InicioCarregamento: return InicioCarregamentoNotificarMotoristaApp;
                case ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.InicioDescarregamento: return InicioDescarregamentoNotificarMotoristaApp;
                case ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.InicioHigienizacao: return InicioHigienizacaoNotificarMotoristaApp;
                case ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.InicioViagem: return GuaritaSaidaNotificarMotoristaApp;
                case ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.LiberacaoChave: return LiberaChaveNotificarMotoristaApp;
                case ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.MontagemCarga: return MontagemCargaNotificarMotoristaApp;
                case ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.Posicao: return PosicaoNotificarMotoristaApp;
                case ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.SaidaLoja: return SaidaLojaNotificarMotoristaApp;
                case ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.SeparacaoMercadoria: return SeparacaoMercadoriaNotificarMotoristaApp;
                case ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.SolicitacaoVeiculo: return SolicitacaoVeiculoNotificarMotoristaApp;
                case ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.TravamentoChave: return TravaChaveNotificarMotoristaApp;
                default: return false;
            }
        }

        public virtual bool IsPermiteVoltar(ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio etapaFluxo)
        {
            switch (etapaFluxo)
            {
                case ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.CheckList: return CheckListPermiteVoltar;
                case ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.ChegadaLoja: return ChegadaLojaPermiteVoltar;
                case ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.ChegadaVeiculo: return ChegadaVeiculoPermiteVoltar;
                case ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.DeslocamentoPatio: return DeslocamentoPatioPermiteVoltar;
                case ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.DocumentoFiscal: return DocumentoFiscalPermiteVoltar;
                case ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.DocumentosTransporte: return DocumentosTransportePermiteVoltar;
                case ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.Expedicao: return ExpedicaoPermiteVoltar;
                case ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.Faturamento: return FaturamentoPermiteVoltar;
                case ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.FimCarregamento: return FimCarregamentoPermiteVoltar;
                case ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.FimDescarregamento: return FimDescarregamentoPermiteVoltar;
                case ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.FimHigienizacao: return FimHigienizacaoPermiteVoltar;
                case ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.FimViagem: return FimViagemPermiteVoltar;
                case ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.Guarita: return GuaritaEntradaPermiteVoltar;
                case ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.InformarDoca: return InformarDocaCarregamentoPermiteVoltar;
                case ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.InicioCarregamento: return InicioCarregamentoPermiteVoltar;
                case ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.InicioDescarregamento: return InicioDescarregamentoPermiteVoltar;
                case ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.InicioHigienizacao: return InicioHigienizacaoPermiteVoltar;
                case ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.InicioViagem: return GuaritaSaidaPermiteVoltar;
                case ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.LiberacaoChave: return LiberaChavePermiteVoltar;
                case ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.MontagemCarga: return MontagemCargaPermiteVoltar;
                case ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.Posicao: return PosicaoPermiteVoltar;
                case ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.SaidaLoja: return SaidaLojaPermiteVoltar;
                case ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.SeparacaoMercadoria: return SeparacaoMercadoriaPermiteVoltar;
                case ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.SolicitacaoVeiculo: return SolicitacaoVeiculoPermiteVoltar;
                case ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.TravamentoChave: return TravaChavePermiteVoltar;
                case ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.AvaliacaoDescarga: return AvaliacaoDescargaPermiteVoltar;
                default: return false;
            }
        }

        public virtual bool IsPermiteAnteciparEtapa(ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio etapaFluxo)
        {
            switch (etapaFluxo)
            {
                case ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.CheckList: return ChecklistPermiteAntecipar;
                case ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.ChegadaVeiculo: return ChegadaVeiculoPermiteAntecipar;
                case ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.Guarita: return GuaritaEntradaPermiteAntecipar;
                case ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.MontagemCarga: return MontagemCargaPermiteAntecipar;
                case ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.InformarDoca: return InformarDocaPermiteAntecipar;
                default: return false;
            }
        }

        #endregion Métodos Públicos
    }
}
