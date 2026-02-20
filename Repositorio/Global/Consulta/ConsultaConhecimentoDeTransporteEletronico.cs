using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Repositorio.Embarcador.Consulta;
using Repositorio.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio
{
    sealed class ConsultaConhecimentoDeTransporteEletronico : Embarcador.Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaCteRelatorio>
    {
        #region Construtores

        public ConsultaConhecimentoDeTransporteEletronico() : base(tabela: "T_CTE as CTe ") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsCfop(StringBuilder joins)
        {
            if (!joins.Contains(" CFOP "))
                joins.Append(" left join T_CFOP CFOP on CFOP.CFO_CODIGO = CTe.CFO_CODIGO ");
        }

        private void SetarJoinsComplemento(StringBuilder joins)
        {
            if (!joins.Contains(" ComplementoInfo "))
                joins.Append(" left join T_CARGA_CTE_COMPLEMENTO_INFO ComplementoInfo on ComplementoInfo.CON_CODIGO = CTe.CON_CODIGO ");
        }

        private void SetarJoinsDestinatario(StringBuilder joins)
        {
            if (!joins.Contains(" DestinatarioCTe "))
                joins.Append(" left join T_CTE_PARTICIPANTE DestinatarioCTe on CTe.CON_DESTINATARIO_CTE = DestinatarioCTe.PCT_CODIGO ");
        }

        private void SetarJoinsDestinatarioCliente(StringBuilder joins)
        {
            SetarJoinsDestinatario(joins);

            if (!joins.Contains("ClienteDestinatario"))
                joins.Append(" left join T_CLIENTE ClienteDestinatario on ClienteDestinatario.CLI_CGCCPF = DestinatarioCTe.CLI_CODIGO ");
        }

        private void SetarJoinsDestinatarioLocalidade(StringBuilder joins)
        {
            SetarJoinsDestinatario(joins);

            if (!joins.Contains(" LocalidadeDestinatario "))
                joins.Append(" left join T_LOCALIDADES LocalidadeDestinatario on DestinatarioCTe.LOC_CODIGO = LocalidadeDestinatario.LOC_CODIGO ");
        }

        private void SetarJoinsDestinatarioGrupoPessoa(StringBuilder joins)
        {
            SetarJoinsDestinatarioCliente(joins);

            if (!joins.Contains(" GrupoPessoaDestinatario "))
                joins.Append(" left join T_GRUPO_PESSOAS GrupoPessoaDestinatario on GrupoPessoaDestinatario.GRP_CODIGO = ClienteDestinatario.GRP_CODIGO ");
        }

        private void SetarJoinsDestinatarioCategoria(StringBuilder joins)
        {
            SetarJoinsDestinatarioCliente(joins);

            if (!joins.Contains(" CategoriaDestinatario "))
                joins.Append(" left join T_CATEGORIA_PESSOA CategoriaDestinatario on CategoriaDestinatario.CTP_CODIGO = ClienteDestinatario.CTP_CODIGO ");
        }

        private void SetarJoinsDocumentoFaturamento(StringBuilder joins)
        {
            if (!joins.Contains(" DocumentoFaturamentoCTe "))
                joins.Append($" left join T_DOCUMENTO_FATURAMENTO DocumentoFaturamentoCTe on CTe.CON_CODIGO = DocumentoFaturamentoCTe.CON_CODIGO ");
        }

        private void SetarJoinsTermoPagamento(StringBuilder joins)
        {
            SetarJoinsDocumentoFaturamento(joins);

            if (!joins.Contains(" TermoPagamento "))
                joins.Append($" left join T_TERMOS_PAGAMENTO TermoPagamento on TermoPagamento.TPG_CODIGO = DocumentoFaturamentoCTe.TPG_CODIGO ");
        }

        private void SetarJoinsDocumentoFaturamentoCarga(StringBuilder joins)
        {
            if (!joins.Contains(" DocumentoFaturamentoCarga "))
                joins.Append(" left join T_DOCUMENTO_FATURAMENTO DocumentoFaturamentoCarga on DocumentoFaturamentoCarga.DFA_SITUACAO NOT IN (2, 3) and DocumentoFaturamentoCarga.CAR_CODIGO in (select CargaCTe.CAR_CODIGO_ORIGEM from T_CARGA_CTE CargaCTe where CargaCTe.CON_CODIGO = CTe.CON_CODIGO) ");
        }

        private void SetarJoinsExpedidor(StringBuilder joins)
        {
            if (!joins.Contains(" ExpedidorCTe "))
                joins.Append(" left join T_CTE_PARTICIPANTE ExpedidorCTe on CTe.CON_EXPEDIDOR_CTE = ExpedidorCTe.PCT_CODIGO ");
        }

        private void SetarJoinsExpedidorCliente(StringBuilder joins)
        {
            SetarJoinsExpedidor(joins);

            if (!joins.Contains(" ClienteExpedidor "))
                joins.Append(" left join T_CLIENTE ClienteExpedidor on ClienteExpedidor.CLI_CGCCPF = ExpedidorCTe.CLI_CODIGO ");
        }

        private void SetarJoinsExpedidorLocalidade(StringBuilder joins)
        {
            SetarJoinsExpedidor(joins);

            if (!joins.Contains(" LocalidadeExpedidor "))
                joins.Append(" left join T_LOCALIDADES LocalidadeExpedidor on ExpedidorCTe.LOC_CODIGO = LocalidadeExpedidor.LOC_CODIGO ");
        }

        private void SetarJoinsFatura(StringBuilder joins)
        {
            if (!joins.Contains(" Documento "))
                joins.Append(" LEFT OUTER JOIN T_DOCUMENTO_FATURAMENTO Documento on Documento.CON_CODIGO = CTe.CON_CODIGO AND Documento.DFA_SITUACAO <> 2 ");

            if (!joins.Contains(" FaturaDocumento "))
                joins.Append(" left OUTER JOIN T_FATURA_DOCUMENTO FaturaDocumento on FaturaDocumento.DFA_CODIGO = Documento.DFA_CODIGO ");

            if (!joins.Contains(" Fatura "))
                joins.Append(" LEFT OUTER JOIN T_FATURA Fatura on Fatura.FAT_CODIGO = FaturaDocumento.FAT_CODIGO ");
        }

        private void SetarJoinsFechamentoFrete(StringBuilder joins)
        {
            SetarJoinsComplemento(joins);

            if (!joins.Contains(" FechamentoFrete "))
                joins.Append(" left join T_FECHAMENTO_FRETE FechamentoFrete on FechamentoFrete.FEF_CODIGO = ComplementoInfo.FEF_CODIGO ");
        }

        private void SetarJoinsIntegracaoRecebimento(StringBuilder joins)
        {
            if (!joins.Contains(" IntegracaoCTeRecebimento "))
                joins.Append(" left join T_INTEGRACAO_CTE_RECEBIMENTO IntegracaoCTeRecebimento on IntegracaoCTeRecebimento.CON_CODIGO = CTe.CON_CODIGO and IntegracaoCTeRecebimento.ICR_TIPO = 0 ");
        }

        private void SetarJoinsLocalidadeFimPrestacao(StringBuilder joins)
        {
            if (!joins.Contains(" FimPrestacaoCTe "))
                joins.Append(" left join T_LOCALIDADES FimPrestacaoCTe on CTe.CON_LOCTERMINOPRESTACAO = FimPrestacaoCTe.LOC_CODIGO ");
        }

        private void SetarJoinsLocalidadeInicioPrestacao(StringBuilder joins)
        {
            if (!joins.Contains(" InicioPrestacaoCTe "))
                joins.Append(" left join T_LOCALIDADES InicioPrestacaoCTe on CTe.CON_LOCINICIOPRESTACAO = InicioPrestacaoCTe.LOC_CODIGO ");
        }

        private void SetarJoinsModeloDocumento(StringBuilder joins)
        {
            if (!joins.Contains(" ModeloDocumento "))
                joins.Append(" left join T_MODDOCFISCAL ModeloDocumento on CTe.CON_MODELODOC = ModeloDocumento.MOD_CODIGO ");
        }

        private void SetarJoinsOcorrencia(StringBuilder joins)
        {
            SetarJoinsComplemento(joins);

            if (!joins.Contains(" Ocorrencia "))
                joins.Append(" left join T_CARGA_OCORRENCIA Ocorrencia on Ocorrencia.COC_CODIGO = ComplementoInfo.COC_CODIGO ");
        }

        private void SetarJoinsTipoOcorrencia(StringBuilder joins)
        {
            SetarJoinsOcorrencia(joins);

            if (!joins.Contains(" TipoOcorrencia "))
                joins.Append(" left join T_OCORRENCIA TipoOcorrencia on TipoOcorrencia.OCO_CODIGO = Ocorrencia.OCO_CODIGO ");
        }

        private void SetarJoinsCTeTerceiroOcorrencia(StringBuilder joins)
        {
            SetarJoinsOcorrencia(joins);

            if (!joins.Contains(" CTeTerceiroOcorrencia "))
                joins.Append(" left join T_CTE_TERCEIRO CTeTerceiroOcorrencia on CTeTerceiroOcorrencia.CPS_CODIGO = Ocorrencia.CPS_CODIGO ");
        }

        private void SetarJoinsProvedor(StringBuilder joins)
        {
            if (!joins.Contains(" Provedor "))
                joins.Append(" left join T_CLIENTE Provedor on Provedor.CLI_CGCCPF = CTe.CON_CLIENTE_PROVEDOR_OS  ");
        }

        private void SetarJoinsRecebedor(StringBuilder joins)
        {
            if (!joins.Contains(" RecebedorCTe "))
                joins.Append(" left join T_CTE_PARTICIPANTE RecebedorCTe on CTe.CON_RECEBEDOR_CTE = RecebedorCTe.PCT_CODIGO ");
        }

        private void SetarJoinsRecebedorCliente(StringBuilder joins)
        {
            SetarJoinsRecebedor(joins);

            if (!joins.Contains(" ClienteRecebedor "))
                joins.Append(" left join T_CLIENTE ClienteRecebedor on ClienteRecebedor.CLI_CGCCPF = RecebedorCTe.CLI_CODIGO ");
        }

        private void SetarJoinsRecebedorLocalidade(StringBuilder joins)
        {
            SetarJoinsRecebedor(joins);

            if (!joins.Contains(" LocalidadeRecebedor "))
                joins.Append(" left join T_LOCALIDADES LocalidadeRecebedor on RecebedorCTe.LOC_CODIGO = LocalidadeRecebedor.LOC_CODIGO ");
        }

        private void SetarJoinsRemetente(StringBuilder joins)
        {
            if (!joins.Contains(" RemetenteCTe "))
                joins.Append(" left join T_CTE_PARTICIPANTE RemetenteCTe on CTe.CON_REMETENTE_CTE = RemetenteCTe.PCT_CODIGO ");
        }

        private void SetarJoinsRemetenteCliente(StringBuilder joins)
        {
            SetarJoinsRemetente(joins);

            if (!joins.Contains(" ClienteRemetente "))
                joins.Append(" left join T_CLIENTE ClienteRemetente on ClienteRemetente.CLI_CGCCPF = RemetenteCTe.CLI_CODIGO ");
        }

        private void SetarJoinsRemetenteLocalidade(StringBuilder joins)
        {
            SetarJoinsRemetente(joins);

            if (!joins.Contains(" LocalidadeRemetente "))
                joins.Append(" left join T_LOCALIDADES LocalidadeRemetente on RemetenteCTe.LOC_CODIGO = LocalidadeRemetente.LOC_CODIGO ");
        }

        private void SetarJoinsRemetenteGrupoPessoa(StringBuilder joins)
        {
            SetarJoinsRemetenteCliente(joins);

            if (!joins.Contains(" GrupoPessoaRemetente "))
                joins.Append(" left join T_GRUPO_PESSOAS GrupoPessoaRemetente on GrupoPessoaRemetente.GRP_CODIGO = ClienteRemetente.GRP_CODIGO ");
        }

        private void SetarJoinsRemetenteCategoria(StringBuilder joins)
        {
            SetarJoinsRemetenteCliente(joins);

            if (!joins.Contains(" CategoriaRemetente "))
                joins.Append(" left join T_CATEGORIA_PESSOA CategoriaRemetente on CategoriaRemetente.CTP_CODIGO = ClienteRemetente.CTP_CODIGO ");
        }

        private void SetarJoinsSerie(StringBuilder joins)
        {
            if (!joins.Contains(" Serie "))
                joins.Append(" left join T_EMPRESA_SERIE Serie on CTe.CON_SERIE = Serie.ESE_CODIGO ");
        }

        private void SetarJoinsTitulo(StringBuilder joins)
        {
            if (!joins.Contains(" TituloCTe "))
                joins.Append(" left join T_TITULO TituloCTe on TituloCTe.TIT_CODIGO = CTe.TIT_CODIGO ");
        }

        private void SetarJoinsTomador(StringBuilder joins)
        {
            if (!joins.Contains(" TomadorPagadorCTe "))
                joins.Append(" left join T_CTE_PARTICIPANTE TomadorPagadorCTe on CTe.CON_TOMADOR_PAGADOR_CTE = TomadorPagadorCTe.PCT_CODIGO ");
        }

        private void SetarJoinsTomadorCliente(StringBuilder joins)
        {
            SetarJoinsTomador(joins);

            if (!joins.Contains(" ClienteTomador "))
                joins.Append(" left join T_CLIENTE ClienteTomador on ClienteTomador.CLI_CGCCPF = TomadorPagadorCTe.CLI_CODIGO ");
        }

        private void SetarJoinsTomadorGrupo(StringBuilder joins)
        {
            SetarJoinsTomador(joins);

            if (!joins.Contains(" GrupoTomadorPagadorCTe "))
                joins.Append(" left join T_GRUPO_PESSOAS GrupoTomadorPagadorCTe on GrupoTomadorPagadorCTe.GRP_CODIGO = TomadorPagadorCTe.GRP_CODIGO ");
        }

        private void SetarJoinsTomadorLocalidade(StringBuilder joins)
        {
            SetarJoinsTomador(joins);

            if (!joins.Contains(" LocalidadeTomador "))
                joins.Append(" left join T_LOCALIDADES LocalidadeTomador on TomadorPagadorCTe.LOC_CODIGO = LocalidadeTomador.LOC_CODIGO ");
        }

        private void SetarJoinsTransportador(StringBuilder joins)
        {
            if (!joins.Contains(" TransportadorCTe "))
                joins.Append(" inner join T_EMPRESA TransportadorCTe on CTe.EMP_CODIGO = TransportadorCTe.EMP_CODIGO ");
        }

        private void SetarJoinsTransportadorPai(StringBuilder joins)
        {
            SetarJoinsTransportador(joins);

            if (!joins.Contains(" TransportadorPaiCTe "))
                joins.Append(" left join T_EMPRESA TransportadorPaiCTe on TransportadorCTe.EMP_EMPRESA = TransportadorPaiCTe.EMP_CODIGO ");
        }

        private void SetarJoinsTransportadorConfiguracao(StringBuilder joins)
        {
            SetarJoinsTransportador(joins);

            if (!joins.Contains(" TransportadorConfiguracao "))
                joins.Append(" left join T_CONFIG TransportadorConfiguracao ON TransportadorConfiguracao.COF_CODIGO = TransportadorCTe.COF_CODIGO");
        }

        private void SetarJoinsTransportadorPaiConfiguracao(StringBuilder joins)
        {
            SetarJoinsTransportadorPai(joins);

            if (!joins.Contains(" TransportadorPaiConfiguracao "))
                joins.Append(" left join T_CONFIG TransportadorPaiConfiguracao ON TransportadorPaiConfiguracao.COF_CODIGO = TransportadorPaiCTe.COF_CODIGO");
        }

        private void SetarJoinsDocumentoEscrituracao(StringBuilder joins)
        {
            if (!joins.Contains(" DocumentoEscrituracao "))
                joins.Append(" left join T_DOCUMENTO_ESCRITURACAO DocumentoEscrituracao ON DocumentoEscrituracao.CON_CODIGO = CTe.CON_CODIGO ");
        }

        private void SetarJoinsLoteEscrituracao(StringBuilder joins)
        {
            SetarJoinsDocumentoEscrituracao(joins);

            if (!joins.Contains(" LoteEscrituracao "))
                joins.Append(" left join T_LOTE_ESCRITURACAO LoteEscrituracao ON LoteEscrituracao.LES_CODIGO = DocumentoEscrituracao.LES_CODIGO ");
        }

        private void SetarJoinsPagamento(StringBuilder joins)
        {
            SetarJoinsDocumentoFaturamento(joins);

            if (!joins.Contains(" Pagamento "))
                joins.Append(" left join T_PAGAMENTO Pagamento on Pagamento.PAG_CODIGO = DocumentoFaturamentoCTe.PAG_CODIGO ");
        }

        private void SetarJoinsPortoOrigem(StringBuilder joins)
        {
            if (!joins.Contains(" PortoOrigem "))
                joins.Append(" left join T_PORTO PortoOrigem on PortoOrigem.POT_CODIGO = CTe.POT_CODIGO_ORIGEM ");
        }

        private void SetarJoinsPortoDestino(StringBuilder joins)
        {
            if (!joins.Contains(" PortoDestino "))
                joins.Append(" left join T_PORTO PortoDestino on PortoDestino.POT_CODIGO = CTe.POT_CODIGO_DESTINO ");
        }

        private void SetarJoinsPortoPassagemUm(StringBuilder joins)
        {
            if (!joins.Contains(" PortoPassagemUm "))
                joins.Append(" left join T_PORTO PortoPassagemUm on PortoPassagemUm.POT_CODIGO = CTe.POT_CODIGO_PASSAGEM_UM ");
        }

        private void SetarJoinsPortoPassagemDois(StringBuilder joins)
        {
            if (!joins.Contains(" PortoPassagemDois "))
                joins.Append(" left join T_PORTO PortoPassagemDois on PortoPassagemDois.POT_CODIGO = CTe.POT_CODIGO_PASSAGEM_DOIS ");
        }

        private void SetarJoinsPortoPassagemTres(StringBuilder joins)
        {
            if (!joins.Contains(" PortoPassagemTres "))
                joins.Append(" left join T_PORTO PortoPassagemTres on PortoPassagemTres.POT_CODIGO = CTe.POT_CODIGO_PASSAGEM_TRES ");
        }

        private void SetarJoinsPortoPassagemQuatro(StringBuilder joins)
        {
            if (!joins.Contains(" PortoPassagemQuatro "))
                joins.Append(" left join T_PORTO PortoPassagemQuatro on PortoPassagemQuatro.POT_CODIGO = CTe.POT_CODIGO_PASSAGEM_QUATRO ");
        }

        private void SetarJoinsPortoPassagemCinco(StringBuilder joins)
        {
            if (!joins.Contains(" PortoPassagemCinco "))
                joins.Append(" left join T_PORTO PortoPassagemCinco on PortoPassagemCinco.POT_CODIGO = CTe.POT_CODIGO_PASSAGEM_CINCO ");
        }

        private void SetarJoinsViagem(StringBuilder joins)
        {
            if (!joins.Contains(" Viagem "))
                joins.Append(" left join T_PEDIDO_VIAGEM_NAVIO Viagem on Viagem.PVN_CODIGO = CTe.CON_VIAGEM ");
        }
        private void SetarJoinsNavio(StringBuilder joins)
        {
            SetarJoinsViagem(joins);

            if (!joins.Contains(" Navio "))
                joins.Append(" left join T_NAVIO Navio on Navio.NAV_CODIGO = Viagem.NAV_CODIGO ");
        }

        private void SetarJoinsViagemSchedule(StringBuilder joins)
        {
            if (!joins.Contains(" ViagemSchedule "))
                joins.Append(" left join T_PEDIDO_VIAGEM_NAVIO_SCHEDULE ViagemSchedule on ViagemSchedule.PVN_CODIGO = CTe.CON_VIAGEM AND ViagemSchedule.POT_CODIGO_ATRACACAO = CTe.POT_CODIGO_ORIGEM ");
        }

        private void SetarJoinsPedidoViagemNavioSchedule(StringBuilder joins)
        {
            if (!joins.Contains(" PedidoViagemNavioSchedule "))
                joins.Append(" left join T_PEDIDO_VIAGEM_NAVIO_SCHEDULE PedidoViagemNavioSchedule on PedidoViagemNavioSchedule.PVS_CODIGO = CTe.PVS_CODIGO ");
        }

        private void SetarJoinsViagemScheduleDestino(StringBuilder joins)
        {
            if (!joins.Contains(" ViagemScheduleDestino "))
                joins.Append(" left join T_PEDIDO_VIAGEM_NAVIO_SCHEDULE ViagemScheduleDestino on ViagemScheduleDestino.PVN_CODIGO = CTe.CON_VIAGEM AND ViagemScheduleDestino.TTI_CODIGO_ATRACACAO = CTe.CON_TERMINAL_ORIGEM ");
        }

        private void SetarJoinsDocumentoEscrituracaoCancelamento(StringBuilder joins)
        {
            if (!joins.Contains(" DocumentoEscrituracaoCancelamento "))
                joins.Append(" left join T_DOCUMENTO_ESCRITURACAO_CANCELAMENTO DocumentoEscrituracaoCancelamento ON DocumentoEscrituracaoCancelamento.CON_CODIGO = CTe.CON_CODIGO ");
        }

        private void SetarJoinsLoteEscrituracaoCancelamento(StringBuilder joins)
        {
            SetarJoinsDocumentoEscrituracaoCancelamento(joins);

            if (!joins.Contains(" LoteEscrituracaoCancelamento "))
                joins.Append(" left join T_LOTE_ESCRITURACAO_CANCELAMENTO LoteEscrituracaoCancelamento ON LoteEscrituracaoCancelamento.LEC_CODIGO = DocumentoEscrituracaoCancelamento.LEC_CODIGO ");
        }

        private void SetarJoinsCTeVeiculo(StringBuilder joins)
        {
            if (!joins.Contains(" CteVeiculo "))
                joins.Append(" LEFT JOIN T_CTE_VEICULO CteVeiculo on CteVeiculo.CON_CODIGO = CTe.CON_CODIGO ");
        }

        private void SetarJoinsVeiculo(StringBuilder joins)
        {
            SetarJoinsCTeVeiculo(joins);

            if (!joins.Contains(" Veiculo "))
                joins.Append(" LEFT JOIN T_VEICULO Veiculo on Veiculo.VEI_CODIGO = CteVeiculo.VEI_CODIGO ");
        }

        private void SetarJoinsModeloVeiculo(StringBuilder joins)
        {
            SetarJoinsVeiculo(joins);

            if (!joins.Contains(" VeiculoModelo "))
                joins.Append(" LEFT JOIN T_VEICULO_MODELO VeiculoModelo on VeiculoModelo.VMO_CODIGO = Veiculo.VMO_CODIGO ");
        }

        private void SetarJoinsCentroResultado(StringBuilder joins)
        {
            if (!joins.Contains(" CentroResultado "))
                joins.Append(" LEFT OUTER JOIN T_CENTRO_RESULTADO CentroResultado on CentroResultado.CRE_CODIGO = CTE.CRE_CODIGO_FATURAMENTO OR exists(select tipoOperacaoPagamentos.CTP_CODIGO from T_CONFIGURACAO_TIPO_OPERACAO_PAGAMENTOS tipoOperacaoPagamentos inner join T_TIPO_OPERACAO tipoOperacao on tipoOperacao.CTP_CODIGO = tipoOperacaoPagamentos.CTP_CODIGO inner join T_CARGA carga on carga.TOP_CODIGO = tipoOperacao.TOP_CODIGO inner join T_CARGA_CTE cargaCTe on cargaCTe.CAR_CODIGO = carga.CAR_CODIGO where cargaCTe.CON_CODIGO = CTe.CON_CODIGO and tipoOperacaoPagamentos.CRE_CODIGO = CentroResultado.CRE_CODIGO)");
        }

        private void SetarJoinsRegraICMS(StringBuilder joins)
        {
            if (!joins.Contains(" RegraICMS "))
                joins.Append(" LEFT JOIN T_REGRA_ICMS RegraICMS ON RegraICMS.RIC_CODIGO = CTE.RIC_CODIGO ");
        }

        private void SetarJoinsTransportadorLocalidade(StringBuilder joins)
        {
            SetarJoinsTransportador(joins);

            if (!joins.Contains(" LocalidadeTransportador "))
                joins.Append(" left join T_LOCALIDADES LocalidadeTransportador on TransportadorCTe.LOC_CODIGO = LocalidadeTransportador.LOC_CODIGO ");
        }

        private void SetarJoinsJustificativaMercante(StringBuilder joins)
        {
            if (!joins.Contains(" JustificativaMercante "))
                joins.Append(" LEFT JOIN T_JUSTIFICATIVA_MERCANTE JustificativaMercante ON JustificativaMercante.JME_CODIGO = CTe.JME_CODIGO ");
        }

        private void SetarJoinsDocumentoOriginario(StringBuilder joins)
        {
            if (!joins.Contains(" DocumentoOriginario "))
                joins.Append(" left join T_CTE_DOCUMENTO_ORIGINARIO DocumentoOriginario on DocumentoOriginario.CON_CODIGO = CTe.CON_CODIGO ");
        }

        private void SetarJoinsCargaCte(StringBuilder joins)
        {
            if (!joins.Contains(" CargaCTe "))
                joins.Append(" left join T_CARGA_CTE CargaCTe on CargaCTe.CON_CODIGO = CTe.CON_CODIGO ");
        }

        private void SetarJoinsDocumentoProvisao(StringBuilder joins)
        {
            SetarJoinsCargaCte(joins);

            if (!joins.Contains(" DocumentoProvisao "))
                joins.Append(" left join T_DOCUMENTO_PROVISAO DocumentoProvisao on DocumentoProvisao.CAR_CODIGO = CargaCTe.CAR_CODIGO ");
        }

        private void SetarJoinsStages(StringBuilder joins)
        {
            SetarJoinsDocumentoProvisao(joins);

            if (!joins.Contains(" Stage "))
                joins.Append("join T_STAGE Stage on Stage.STA_CODIGO = DocumentoProvisao.STA_CODIGO ");
        }

        private void SetarJoinsCarga(StringBuilder joins)
        {
            SetarJoinsCargaCte(joins);

            if (!joins.Contains(" Carga "))
                joins.Append(" left join T_CARGA Carga on CargaCTe.CAR_CODIGO = Carga.CAR_CODIGO ");
        }

        private void SetarJoinsContratoFreteTerceiro(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" ContratoFreteTerceiro "))
                joins.Append(" left join T_CONTRATO_FRETE_TERCEIRO ContratoFreteTerceiro on Carga.CAR_CODIGO = ContratoFreteTerceiro.CAR_CODIGO ");

        }


        private void SetarJoinsMicDTA(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" MicDTA "))
                joins.Append(" left join T_CARGA_MIC_DTA MicDTA on Carga.CAR_CODIGO = MicDTA.CAR_CODIGO ");

        }

        private void SetarJoinsTransportadorMdfe(StringBuilder joins)
        {
            if (!joins.Contains(" TransportadorMdfe "))
                joins.Append(@" OUTER APPLY (
	                                SELECT emdf.[EMP_CNPJ] as CnpjMdfe FROM [T_CARGA_MDFE] cmdf
	                                INNER JOIN [T_MDFE] mdfe ON cmdf.[MDF_CODIGO] = mdfe.[MDF_CODIGO]
	                                INNER JOIN [T_EMPRESA] emdf ON mdfe.[EMP_CODIGO] = emdf.[EMP_CODIGO]
	                                INNER JOIN [T_CARGA_CTE] ccte ON ccte.[CON_CODIGO] = CTe.[CON_CODIGO]
	                                WHERE cmdf.[CAR_CODIGO] = ccte.[CAR_CODIGO]
                            ) as TransportadorMdfe ");
        }

        private void SetarJoinsCTeOriginal(StringBuilder joins)
        {
            if (!joins.Contains(" CTeComplementado "))
                joins.Append(" LEFT JOIN T_CTE CTeComplementado ON CTeComplementado.CON_CHAVECTE = CTe.CON_CHAVE_CTE_SUB_COMP and CTeComplementado.CON_CHAVECTE <> '' and CTe.CON_CHAVE_CTE_SUB_COMP <> ''  and CTeComplementado.CON_CHAVECTE is not null and CTe.CON_CHAVE_CTE_SUB_COMP is not null");
        }



        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaCteRelatorio filtrosPesquisa)
        {
            switch (propriedade)
            {
                case "AliquotaCOFINS":
                    if (!select.Contains(" AliquotaCOFINS,"))
                    {
                        select.Append("coalesce(TransportadorConfiguracao.COF_ALIQUOTA_COFINS, TransportadorPaiConfiguracao.COF_ALIQUOTA_COFINS, 0) AliquotaCOFINS, ");

                        groupBy.Append("TransportadorConfiguracao.COF_ALIQUOTA_COFINS, ");
                        groupBy.Append("TransportadorPaiConfiguracao.COF_ALIQUOTA_COFINS, ");

                        SetarJoinsTransportadorConfiguracao(joins);
                        SetarJoinsTransportadorPaiConfiguracao(joins);
                    }
                    break;
                case "ContaContabil":
                    if (!select.Contains(" ContaContabil,"))
                    {
                        select.Append("ClienteTomador.CLI_CONTA_FORNECEDOR_EBS ContaContabil, ");
                        groupBy.Append("ClienteTomador.CLI_CONTA_FORNECEDOR_EBS, ");


                        SetarJoinsTomador(joins);
                        SetarJoinsTomadorCliente(joins);
                    }
                    break;

                case "AliquotaPIS":
                    if (!select.Contains(" AliquotaPIS,"))
                    {
                        select.Append("coalesce(TransportadorConfiguracao.COF_ALIQUOTA_PIS, TransportadorPaiConfiguracao.COF_ALIQUOTA_PIS, 0) AliquotaPIS, ");

                        groupBy.Append("TransportadorConfiguracao.COF_ALIQUOTA_PIS, ");
                        groupBy.Append("TransportadorPaiConfiguracao.COF_ALIQUOTA_PIS, ");

                        SetarJoinsTransportadorConfiguracao(joins);
                        SetarJoinsTransportadorPaiConfiguracao(joins);
                    }
                    break;

                case "ValorCOFINS":
                    SetarSelect("AliquotaCOFINS", 0, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    SetarSelect("BaseCalculoICMS", 0, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    break;

                case "ValorPIS":
                    SetarSelect("AliquotaPIS", 0, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    SetarSelect("BaseCalculoICMS", 0, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    break;

                case "CNPJFilial":
                    if (!select.Contains(" CNPJFilial,"))
                    {
                        select.Append(
                            @"SUBSTRING((
                                select distinct ', ' + _filial.FIL_CNPJ
                                  from T_CARGA_CTE _cargaCTe 
                                 inner join T_CARGA _carga ON _carga.CAR_CODIGO = _cargaCTe.CAR_CODIGO_ORIGEM 
                                 inner join T_FILIAL _filial on _carga.FIL_CODIGO = _filial.FIL_CODIGO
                                 where _cargaCTe.CON_CODIGO = CTe.CON_CODIGO
                                   for xml path('')
                            ), 3, 1000) CNPJFilial, "
                        );

                        if (!groupBy.Contains(" CTe.CON_CODIGO"))
                            groupBy.Append(" CTe.CON_CODIGO, ");
                    }
                    break;

                case "Filial":
                    if (!select.Contains(" Filial,"))
                    {
                        select.Append(
                            @"SUBSTRING((
                                select distinct ', ' + _filial.FIL_DESCRICAO
                                  from T_CARGA_CTE _cargaCTe 
                                 inner join T_CARGA _carga ON _carga.CAR_CODIGO = _cargaCTe.CAR_CODIGO_ORIGEM 
                                 inner join T_FILIAL _filial on _carga.FIL_CODIGO = _filial.FIL_CODIGO
                                 where _cargaCTe.CON_CODIGO = CTe.CON_CODIGO
                                   for xml path('')
                            ), 3, 1000) Filial, "
                        );

                        if (!groupBy.Contains(" CTe.CON_CODIGO"))
                            groupBy.Append(" CTe.CON_CODIGO, ");
                    }
                    break;

                case "FilialVenda":
                    if (!select.Contains(" FilialVenda,"))
                    {
                        select.Append(
                            @"SUBSTRING((
                                select distinct ', ' + _filial.FIL_DESCRICAO
                                  from T_CARGA_CTE _cargaCTe 
                                 inner join T_CARGA _carga ON _carga.CAR_CODIGO = _cargaCTe.CAR_CODIGO_ORIGEM 
                                 inner join T_FILIAL _filial on _carga.FIL_CODIGO = _filial.FIL_CODIGO
                                 where _cargaCTe.CON_CODIGO = CTe.CON_CODIGO
                                   for xml path('')
                            ), 3, 1000) FilialVenda, "
                        );
                    }
                    break;

                case "NumeroCTe":
                    if (!select.Contains("NumeroCTe"))
                    {
                        select.Append("CTe.CON_NUM NumeroCTe, ");
                        groupBy.Append("CTe.CON_NUM, ");
                    }
                    break;

                case "ChaveCTe":
                    if (!select.Contains("ChaveCTe"))
                    {
                        select.Append("CTe.CON_CHAVECTE ChaveCTe, ");
                        groupBy.Append("CTe.CON_CHAVECTE, ");
                    }
                    break;

                case "Log":
                    if (!select.Contains("Log"))
                    {
                        select.Append("CTe.CON_LOG Log, ");
                        groupBy.Append("CTe.CON_LOG, ");
                    }
                    break;

                case "ProtocoloAutorizacao":
                    if (!select.Contains("ProtocoloAutorizacao"))
                    {
                        select.Append("CTe.CON_PROTOCOLO ProtocoloAutorizacao, ");
                        groupBy.Append("CTe.CON_PROTOCOLO, ");
                    }
                    break;

                case "ProtocoloInutilizacaoCancelamento":
                    if (!select.Contains("ProtocoloInutilizacaoCancelamento"))
                    {
                        select.Append("CTe.CON_PROTOCOLOCANINU ProtocoloInutilizacaoCancelamento, ");
                        groupBy.Append("CTe.CON_PROTOCOLOCANINU, ");
                    }
                    break;

                case "RetornoSefaz":
                    if (!select.Contains("RetornoSefaz"))
                    {
                        select.Append("CTe.CON_RETORNOCTE RetornoSefaz, ");
                        groupBy.Append("CTe.CON_RETORNOCTE, ");
                    }
                    break;

                case "SerieCTe":
                    if (!select.Contains("SerieCTe"))
                    {
                        select.Append("Serie.ESE_NUMERO SerieCTe, ");
                        groupBy.Append("Serie.ESE_NUMERO, ");

                        SetarJoinsSerie(joins);
                    }
                    break;

                case "DescricaoTipoServico":
                    if (!select.Contains("TipoServico"))
                    {
                        select.Append("CTe.CON_TIPO_SERVICO TipoServico, ");
                        groupBy.Append("CTe.CON_TIPO_SERVICO, ");
                    }
                    break;

                case "DescricaoTipoTomador":
                    if (!select.Contains("TipoTomador"))
                    {
                        select.Append("CTe.CON_TOMADOR TipoTomador, ");
                        groupBy.Append("CTe.CON_TOMADOR, ");
                    }
                    break;

                case "DescricaoTipoCTe":
                    if (!select.Contains("TipoCTe"))
                    {
                        select.Append("CTe.CON_TIPO_CTE TipoCTe, ");
                        groupBy.Append("CTe.CON_TIPO_CTE, ");
                    }
                    break;

                case "RPS":
                    if (!select.Contains("RPS"))
                    {
                        select.Append("RPS.RPS_NUMERO RPS, ");
                        groupBy.Append("RPS.RPS_NUMERO, ");
                        joins.Append(" left outer join T_NFSE_RPS RPS on CTe.RPS_CODIGO = RPS.RPS_CODIGO ");
                    }
                    break;

                case "StatusCTe":
                    if (!select.Contains("StatusCTe"))
                    {
                        select.Append(
                            @"StatusCTe = CASE 
		                        WHEN CTe.CON_STATUS = 'A' THEN 'Autorizado' 
		                        WHEN CTe.CON_STATUS = 'P' THEN 'Pendente' 
		                        WHEN CTe.CON_STATUS = 'E' THEN 'Enviado' 
		                        WHEN CTe.CON_STATUS = 'R' THEN 'Rejeitado' 
		                        WHEN CTe.CON_STATUS = 'C' THEN 'Cancelado' 
		                        WHEN CTe.CON_STATUS = 'I' THEN 'Inutilizado' 
		                        WHEN CTe.CON_STATUS = 'D' THEN 'Denegado' 
		                        WHEN CTe.CON_STATUS = 'S' THEN 'Em Digitação' 
		                        WHEN CTe.CON_STATUS = 'K' THEN 'Em Cancelamento' 
		                        WHEN CTe.CON_STATUS = 'L' THEN 'Em Inutilização' 
                                WHEN CTe.CON_STATUS = 'Z' AND CTe.CON_ANULADO_GERENCIALMENTE = 1 THEN 'Anulado Gerencial'
                                WHEN CTe.CON_STATUS = 'Z' THEN 'Anulado' 
		                        ELSE ''
                            END, "
                        );

                        groupBy.Append("CTe.CON_STATUS, ");
                        groupBy.Append("CTe.CON_ANULADO_GERENCIALMENTE, ");
                    }
                    break;

                case "NumeroCarga":
                    if (!select.Contains("NumeroCarga"))
                    {
                        select.Append("substring((select distinct ', ' + Carga.CAR_CODIGO_CARGA_EMBARCADOR from T_CARGA_CTE CargaCTe inner join T_CARGA Carga on Carga.CAR_CODIGO = CargaCTe.CAR_CODIGO_ORIGEM where CargaCTe.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 200) NumeroCarga, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "NumeroCargaAgrupamento":
                    if (!select.Contains("NumeroCargaAgrupamento"))
                    {
                        select.Append("substring((select distinct ', ' + Carga.CAR_CODIGO_CARGA_EMBARCADOR from T_CARGA_CTE CargaCTe inner join T_CARGA Carga on Carga.CAR_CODIGO = CargaCTe.CAR_CODIGO where CargaCTe.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 200) NumeroCargaAgrupamento, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "DataCriacaoCarga":
                    if (!select.Contains("DataCriacaoCarga"))
                    {
                        select.Append(
                            @"SUBSTRING((
                                SELECT DISTINCT ', ' + CONVERT(NVARCHAR(10), _carga.CAR_DATA_CRIACAO, 103) + ' ' + CONVERT(NVARCHAR(5), _carga.CAR_DATA_CRIACAO, 108)
                                  FROM T_CARGA_CTE _cargaCTe 
                                 INNER JOIN T_CARGA _carga ON _carga.CAR_CODIGO = _cargaCTe.CAR_CODIGO_ORIGEM 
                                 WHERE _cargaCTe.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 200
                            ) DataCriacaoCarga, "
                        );

                        if (!groupBy.Contains(" CTe.CON_CODIGO"))
                            groupBy.Append(" CTe.CON_CODIGO, ");
                    }
                    break;

                case "SituacaoCarga":
                case "SituacaoCargaFormatada":
                    if (!select.Contains(" SituacaoCarga,"))
                    {
                        select.Append(
                            @"substring((
                                select distinct ', ' + cast(_carga.CAR_SITUACAO as varchar(10))
                                  from T_CARGA_CTE _cargaCTe 
                                  join T_CARGA _carga on _carga.CAR_CODIGO = _cargaCTe.CAR_CODIGO_ORIGEM 
                                 where _cargaCTe.CON_CODIGO = CTe.CON_CODIGO
                                   for xml path('')), 3, 200
                            ) SituacaoCarga, "
                        );

                        if (!groupBy.Contains(" CTe.CON_CODIGO"))
                            groupBy.Append(" CTe.CON_CODIGO, ");
                    }
                    break;

                case "ModeloVeicular":
                    if (!select.Contains("ModeloVeicular"))
                    {
                        select.Append("substring((select distinct ', ' + Modelo.MVC_DESCRICAO from T_CARGA_CTE CargaCTe inner join T_CARGA Carga on Carga.CAR_CODIGO = CargaCTe.CAR_CODIGO_ORIGEM join T_MODELO_VEICULAR_CARGA Modelo on Modelo.MVC_CODIGO = Carga.MVC_CODIGO where CargaCTe.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 200) ModeloVeicular, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "PreCarga":
                    if (!select.Contains("PreCarga"))
                    {
                        select.Append("substring((select distinct ', ' + PreCarga.PCA_NUMERO_CARGA from T_CARGA_CTE CargaCTe inner join T_PRE_CARGA PreCarga on PreCarga.CAR_CODIGO = CargaCTe.CAR_CODIGO_ORIGEM where CargaCTe.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 200) PreCarga, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "Operador":
                    if (!select.Contains("Operador"))
                    {
                        select.Append("substring((select distinct ', ' + Operador.FUN_NOME from T_CARGA_CTE CargaCTe inner join T_CARGA Carga on Carga.CAR_CODIGO = CargaCTe.CAR_CODIGO_ORIGEM inner join T_FUNCIONARIO Operador on Carga.CAR_OPERADOR = Operador.FUN_CODIGO where CargaCTe.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 200) Operador, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "ContratoFrete":
                    if (!select.Contains("ContratoFrete"))
                    {
                        select.Append("substring((select distinct ', ' + ContratoFreteTransportador.CFT_NUMERO_EMBARCADOR + ' - ' + ContratoFreteTransportador.CFT_DESCRICAO from T_CARGA_CTE CargaCTe inner join T_CARGA Carga on Carga.CAR_CODIGO = CargaCTe.CAR_CODIGO_ORIGEM inner join T_CONTRATO_FRETE_TRANSPORTADOR ContratoFreteTransportador on ContratoFreteTransportador.CFT_CODIGO = Carga.CFT_CODIGO where CargaCTe.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 200) ContratoFrete, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "NumeroFechamentoFrete":
                    if (!select.Contains(" NumeroFechamentoFrete,"))
                    {
                        select.Append("FechamentoFrete.FEF_NUMERO NumeroFechamentoFrete, ");
                        groupBy.Append("FechamentoFrete.FEF_NUMERO, ");

                        SetarJoinsFechamentoFrete(joins);
                    }
                    break;

                case "NumeroPedido":
                    if (!select.Contains("NumeroPedido"))
                    {
                        select.Append(@"SUBSTRING(
                                        (
                                            SELECT ', ' + _pedido.PED_NUMERO_PEDIDO_EMBARCADOR
                                            FROM T_CARGA_CTE CargaCTe
                                            JOIN T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE CARGAPEDIDOXMLNOTAFISCALCTE ON CARGAPEDIDOXMLNOTAFISCALCTE.CCT_CODIGO = CargaCTe.CCT_CODIGO
                                            JOIN T_PEDIDO_XML_NOTA_FISCAL PEDIDOXMLNOTAFISCAL ON PEDIDOXMLNOTAFISCAL.PNF_CODIGO = CARGAPEDIDOXMLNOTAFISCALCTE.PNF_CODIGO
                                            JOIN T_CARGA_PEDIDO _cargaPedido ON _cargaPedido.CPE_CODIGO = PEDIDOXMLNOTAFISCAL.CPE_CODIGO
                                            JOIN T_PEDIDO _pedido ON _pedido.PED_CODIGO = _cargaPedido.PED_CODIGO
                                            WHERE CargaCTe.CON_CODIGO = CTe.CON_CODIGO
                                            GROUP BY _pedido.PED_NUMERO_PEDIDO_EMBARCADOR
                                            FOR XML PATH(''), TYPE
                                        ).value('.', 'NVARCHAR(MAX)'), 2, LEN((
                                            SELECT ', ' + _pedido.PED_NUMERO_PEDIDO_EMBARCADOR
                                            FROM T_CARGA_CTE CargaCTe
                                            JOIN T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE CARGAPEDIDOXMLNOTAFISCALCTE ON CARGAPEDIDOXMLNOTAFISCALCTE.CCT_CODIGO = CargaCTe.CCT_CODIGO
                                            JOIN T_PEDIDO_XML_NOTA_FISCAL PEDIDOXMLNOTAFISCAL ON PEDIDOXMLNOTAFISCAL.PNF_CODIGO = CARGAPEDIDOXMLNOTAFISCALCTE.PNF_CODIGO
                                            JOIN T_CARGA_PEDIDO _cargaPedido ON _cargaPedido.CPE_CODIGO = PEDIDOXMLNOTAFISCAL.CPE_CODIGO
                                            JOIN T_PEDIDO _pedido ON _pedido.PED_CODIGO = _cargaPedido.PED_CODIGO
                                            WHERE CargaCTe.CON_CODIGO = CTe.CON_CODIGO
                                            FOR XML PATH(''), TYPE
                                        ).value('.', 'NVARCHAR(MAX)'))
                                    ) AS NumeroPedido, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "NumeroPedidoInterno":
                    if (!select.Contains("NumeroPedidoInterno"))
                    {
                        select.Append("substring((select distinct ', ' + CONVERT(NVARCHAR(15), Pedido.PED_NUMERO) from T_CARGA_CTE CargaCTe inner join T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE CargaPedidoXMLNotaFiscalCTe on CargaPedidoXMLNotaFiscalCTe.CCT_CODIGO = CargaCTe.CCT_CODIGO inner join T_PEDIDO_XML_NOTA_FISCAL PedidoXMLNotaFiscal on PedidoXMLNotaFiscal.PNF_CODIGO = CargaPedidoXMLNotaFiscalCTe.PNF_CODIGO inner join T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = PedidoXMLNotaFiscal.CPE_CODIGO inner join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO where CargaCTe.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 200) NumeroPedidoInterno, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "CodigoReferencia":
                    if (!select.Contains("CodigoReferencia"))
                    {
                        select.Append("substring((select distinct ', ' + PedidoImportacao.PEI_CODIGO_REFERENCIA from T_CARGA_CTE CargaCTe inner join T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE CargaPedidoXMLNotaFiscalCTe on CargaPedidoXMLNotaFiscalCTe.CCT_CODIGO = CargaCTe.CCT_CODIGO inner join T_PEDIDO_XML_NOTA_FISCAL PedidoXMLNotaFiscal on PedidoXMLNotaFiscal.PNF_CODIGO = CargaPedidoXMLNotaFiscalCTe.PNF_CODIGO inner join T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = PedidoXMLNotaFiscal.CPE_CODIGO inner join T_PEDIDO_IMPORTACAO PedidoImportacao on PedidoImportacao.PED_CODIGO = CargaPedido.PED_CODIGO where CargaCTe.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 200) CodigoReferencia, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "CodigoImportacao":
                    if (!select.Contains("CodigoImportacao"))
                    {
                        select.Append("substring((select distinct ', ' + PedidoImportacao.PEI_CODIGO_IMPORTACAO from T_CARGA_CTE CargaCTe inner join T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE CargaPedidoXMLNotaFiscalCTe on CargaPedidoXMLNotaFiscalCTe.CCT_CODIGO = CargaCTe.CCT_CODIGO inner join T_PEDIDO_XML_NOTA_FISCAL PedidoXMLNotaFiscal on PedidoXMLNotaFiscal.PNF_CODIGO = CargaPedidoXMLNotaFiscalCTe.PNF_CODIGO inner join T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = PedidoXMLNotaFiscal.CPE_CODIGO inner join T_PEDIDO_IMPORTACAO PedidoImportacao on PedidoImportacao.PED_CODIGO = CargaPedido.PED_CODIGO where CargaCTe.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 200) CodigoImportacao, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "DataColeta":
                    if (!select.Contains("DataColeta"))
                    {
                        select.Append("substring((select distinct ', ' + CONVERT(NVARCHAR(10), Pedido.CAR_DATA_CARREGAMENTO_PEDIDO, 103) + ' ' + CONVERT(NVARCHAR(5), Pedido.CAR_DATA_CARREGAMENTO_PEDIDO, 108) from T_CARGA_CTE CargaCTe inner join T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE CargaPedidoXMLNotaFiscalCTe on CargaPedidoXMLNotaFiscalCTe.CCT_CODIGO = CargaCTe.CCT_CODIGO inner join T_PEDIDO_XML_NOTA_FISCAL PedidoXMLNotaFiscal on PedidoXMLNotaFiscal.PNF_CODIGO = CargaPedidoXMLNotaFiscalCTe.PNF_CODIGO inner join T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = PedidoXMLNotaFiscal.CPE_CODIGO inner join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO where CargaCTe.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 200) DataColeta, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "TipoDeCarga":
                    if (!select.Contains("TipoDeCarga"))
                    {
                        select.Append("substring((select distinct ', ' + TipoDeCarga.TCG_DESCRICAO from T_CARGA_CTE CargaCTe inner join T_CARGA Carga on Carga.CAR_CODIGO = CargaCTe.CAR_CODIGO inner join T_TIPO_DE_CARGA TipoDeCarga on TipoDeCarga.TCG_CODIGO = Carga.TCG_CODIGO where CargaCTe.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 200) TipoDeCarga, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "TipoOperacao":
                    if (!select.Contains("TipoOperacao"))
                    {
                        select.Append("substring((select distinct ', ' + TipoOperacao.TOP_DESCRICAO from T_CARGA_CTE CargaCTe inner join T_CARGA Carga on Carga.CAR_CODIGO = CargaCTe.CAR_CODIGO_ORIGEM inner join T_TIPO_OPERACAO TipoOperacao on TipoOperacao.TOP_CODIGO = Carga.TOP_CODIGO where CargaCTe.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 200) TipoOperacao, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "DataEntrega":
                    if (!select.Contains("DataEntrega"))
                    {
                        select.Append("CONVERT(NVARCHAR(10), CTe.CON_DATA_ENTREGA, 103) + ' ' + CONVERT(NVARCHAR(5), CTe.CON_DATA_ENTREGA, 108) DataEntrega, ");
                        groupBy.Append("CTe.CON_DATA_ENTREGA, ");
                    }
                    break;

                case "DataEmissao":
                case "AnoEmissao":
                case "MesEmissao":
                case "DataEmissaoFormatada":
                    if (!select.Contains("DataEmissao"))
                    {
                        select.Append("CTe.CON_DATAHORAEMISSAO DataEmissao, ");
                        groupBy.Append("CTe.CON_DATAHORAEMISSAO, ");
                    }
                    break;

                case "CpfMotorista":
                    if (!select.Contains("CpfMotorista"))
                    {
                        select.Append("substring((select ', ' + motoristaCTe1.CMO_CPF_MOTORISTA from T_CTE_MOTORISTA motoristaCTe1 where motoristaCTe1.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 1000) CpfMotorista, ");
                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "DataAutorizacao":
                case "DataAutorizacaoFormatada":
                    if (!select.Contains("DataAutorizacao"))
                    {
                        select.Append("CTe.CON_DATA_AUTORIZACAO DataAutorizacao, ");
                        groupBy.Append("CTe.CON_DATA_AUTORIZACAO, ");
                    }
                    break;

                case "DataCancelamento":
                    if (!select.Contains("DataCancelamento"))
                    {
                        select.Append("CASE CTe.CON_DATA_CANCELAMENTO WHEN NULL THEN '' ELSE convert(nvarchar(10), CTe.CON_DATA_CANCELAMENTO, 3) + ' ' + convert(nvarchar(10), CTe.CON_DATA_CANCELAMENTO, 108) END DataCancelamento, ");
                        groupBy.Append("CTe.CON_DATA_CANCELAMENTO, ");
                    }
                    break;

                case "DataAnulacao":
                    if (!select.Contains("DataAnulacao"))
                    {
                        select.Append("CASE CTe.CON_DATA_ANULACAO WHEN NULL THEN '' ELSE convert(nvarchar(10), CTe.CON_DATA_ANULACAO, 3) + ' ' + convert(nvarchar(10), CTe.CON_DATA_ANULACAO, 108) END DataAnulacao, ");
                        groupBy.Append("CTe.CON_DATA_ANULACAO, ");
                    }
                    break;

                case "DataImportacao":
                    if (!select.Contains("DataImportacao"))
                    {
                        select.Append("CASE IntegracaoCTeRecebimento.ICR_DATA WHEN NULL THEN '' ELSE convert(nvarchar(10), IntegracaoCTeRecebimento.ICR_DATA, 3) + ' ' + convert(nvarchar(10), IntegracaoCTeRecebimento.ICR_DATA, 108) END DataImportacao, ");
                        groupBy.Append("IntegracaoCTeRecebimento.ICR_DATA, ");

                        SetarJoinsIntegracaoRecebimento(joins);
                    }
                    break;

                case "DataVinculoCarga":
                    if (!select.Contains("DataVinculoCarga"))
                    {
                        select.Append("(select TOP 1 CASE CargaCTe.CCT_DATA_VINCULO_CARGA WHEN NULL THEN '' ELSE convert(nvarchar(10), CargaCTe.CCT_DATA_VINCULO_CARGA, 3) + ' ' + convert(nvarchar(10), CargaCTe.CCT_DATA_VINCULO_CARGA, 108) END from T_CARGA_CTE CargaCTe where CargaCTe.CON_CODIGO = CTe.CON_CODIGO) DataVinculoCarga, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "DescricaoStatusTitulo":
                    if (!select.Contains(" StatusTitulo, "))
                    {
                        select.Append("TituloCTe.TIT_STATUS StatusTitulo, ");
                        groupBy.Append("TituloCTe.TIT_STATUS, CTe.TIT_CODIGO, ");

                        SetarJoinsTitulo(joins);
                    }
                    break;

                case "DataVencimento":
                    if (!select.Contains(" DataVencimento,"))
                    {
                        select.Append("substring((select distinct ', ' + Convert(nvarchar(10), Titulo.TIT_DATA_VENCIMENTO, 103) from T_TITULO_DOCUMENTO TituloDocumento inner join T_TITULO Titulo on TituloDocumento.TIT_CODIGO = Titulo.TIT_CODIGO where Titulo.TIT_STATUS <> 4 and TituloDocumento.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 200) DataVencimento, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "NumeroPreFatura":
                    if (!select.Contains("NumeroPreFatura"))
                    {
                        select.Append("reverse(stuff(reverse((CASE WHEN Fatura.FAT_NUMERO_PRE_FATURA is null THEN '' ELSE CONVERT(nvarchar(20), Fatura.FAT_NUMERO_PRE_FATURA) + ', ' END) + isnull((select convert(nvarchar(20), Fatura.FAT_NUMERO_PRE_FATURA) + ', ' from T_DOCUMENTO_FATURAMENTO DocumentoFaturamento inner join T_FATURA_DOCUMENTO FaturaDocumento on DocumentoFaturamento.DFA_CODIGO = FaturaDocumento.DFA_CODIGO inner join T_FATURA Fatura on Fatura.FAT_CODIGO = FaturaDocumento.FAT_CODIGO where Fatura.FAT_SITUACAO <> 3 and (DocumentoFaturamento.CON_CODIGO = CTe.CON_CODIGO OR EXISTS (SELECT CAR_CODIGO FROM T_CARGA_CTE CargaCTe WHERE CargaCTe.CON_CODIGO = CTe.CON_CODIGO and cargaCte.CAR_CODIGO = DocumentoFaturamento.CAR_CODIGO)) for xml path('')), '')), 1, 2, '')) NumeroPreFatura, ");

                        if (!groupBy.Contains("Fatura.FAT_NUMERO_PRE_FATURA"))
                            groupBy.Append("Fatura.FAT_NUMERO_PRE_FATURA, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");

                        SetarJoinsFatura(joins);
                    }
                    break;

                case "CodigoRemetente":
                    if (!select.Contains(" CodigoRemetente, "))
                    {
                        select.Append("ClienteRemetente.CLI_CODIGO_INTEGRACAO CodigoRemetente, ");

                        if (!groupBy.Contains("ClienteRemetente.CLI_CODIGO_INTEGRACAO"))
                            groupBy.Append("ClienteRemetente.CLI_CODIGO_INTEGRACAO, ");

                        SetarJoinsRemetenteCliente(joins);
                    }
                    break;

                case "CPFCNPJRemetente":
                    if (!select.Contains(" CPFCNPJRemetente, "))
                    {
                        select.Append("RemetenteCTe.PCT_CPF_CNPJ CPFCNPJRemetente, ");

                        if (!groupBy.Contains("RemetenteCTe.PCT_CPF_CNPJ"))
                            groupBy.Append("RemetenteCTe.PCT_CPF_CNPJ, ");

                        SetarJoinsRemetente(joins);
                    }
                    break;

                case "IERemetente":
                    if (!select.Contains(" IERemetente, "))
                    {
                        select.Append("RemetenteCTe.PCT_IERG IERemetente, ");
                        groupBy.Append("RemetenteCTe.PCT_IERG, ");

                        SetarJoinsRemetente(joins);
                    }
                    break;

                case "Remetente":
                    if (!select.Contains(" Remetente, "))
                    {
                        select.Append("RemetenteCTe.PCT_NOME Remetente, ");

                        if (!groupBy.Contains("RemetenteCTe.PCT_NOME"))
                            groupBy.Append("RemetenteCTe.PCT_NOME, ");

                        SetarJoinsRemetente(joins);
                    }
                    break;

                case "LocalidadeRemetente":
                    if (!select.Contains(" LocalidadeRemetente, "))
                    {
                        select.Append("LocalidadeRemetente.LOC_DESCRICAO + '-' + LocalidadeRemetente.UF_SIGLA LocalidadeRemetente, ");

                        if (!groupBy.Contains("LocalidadeRemetente.LOC_DESCRICAO"))
                            groupBy.Append("LocalidadeRemetente.LOC_DESCRICAO, ");

                        if (!groupBy.Contains("LocalidadeRemetente.UF_SIGLA"))
                            groupBy.Append("LocalidadeRemetente.UF_SIGLA, ");

                        SetarJoinsRemetenteLocalidade(joins);
                    }
                    break;

                case "UFRemetente":
                    if (!select.Contains(" UFRemetente, "))
                    {
                        select.Append("LocalidadeRemetente.UF_SIGLA UFRemetente, ");

                        if (!groupBy.Contains("LocalidadeRemetente.UF_SIGLA"))
                            groupBy.Append("LocalidadeRemetente.UF_SIGLA, ");

                        SetarJoinsRemetenteLocalidade(joins);
                    }
                    break;

                case "CodigoEnderecoRemetente":
                    if (!select.Contains(" CodigoEnderecoRemetente, "))
                    {
                        select.Append("RemetenteCTe.PCT_CODIGO_ENDERECO_INTEGRACAO CodigoEnderecoRemetente, ");

                        if (!groupBy.Contains("RemetenteCTe.PCT_CODIGO_ENDERECO_INTEGRACAO"))
                            groupBy.Append("RemetenteCTe.PCT_CODIGO_ENDERECO_INTEGRACAO, ");

                        SetarJoinsRemetente(joins);
                    }
                    break;

                case "EnderecoRemetente":
                    if (!select.Contains(" EnderecoRemetente, "))
                    {
                        select.Append("RemetenteCTe.PCT_ENDERECO EnderecoRemetente, ");

                        if (!groupBy.Contains("RemetenteCTe.PCT_ENDERECO"))
                            groupBy.Append("RemetenteCTe.PCT_ENDERECO, ");

                        SetarJoinsRemetente(joins);
                    }
                    break;

                case "GrupoRemetente":
                    if (!select.Contains(" GrupoRemetente, "))
                    {
                        select.Append("GrupoPessoaRemetente.GRP_DESCRICAO GrupoRemetente, ");

                        if (!groupBy.Contains("GrupoPessoaRemetente.GRP_DESCRICAO"))
                            groupBy.Append("GrupoPessoaRemetente.GRP_DESCRICAO, ");

                        SetarJoinsRemetenteGrupoPessoa(joins);
                    }
                    break;

                case "CategoriaRemetente":
                    if (!select.Contains(" CategoriaRemetente, "))
                    {
                        select.Append("CategoriaRemetente.CTP_DESCRICAO CategoriaRemetente, ");

                        if (!groupBy.Contains("CategoriaRemetente.CTP_DESCRICAO"))
                            groupBy.Append("CategoriaRemetente.CTP_DESCRICAO, ");

                        SetarJoinsRemetenteCategoria(joins);
                    }
                    break;

                case "CodigoDocumentoRemetente":
                    if (!select.Contains(" CodigoDocumentoRemetente, "))
                    {
                        select.Append("ClienteRemetente.CLI_CODIGO_DOCUMENTO CodigoDocumentoRemetente, ");

                        if (!groupBy.Contains("ClienteRemetente.CLI_CODIGO_DOCUMENTO"))
                            groupBy.Append("ClienteRemetente.CLI_CODIGO_DOCUMENTO, ");

                        SetarJoinsRemetenteCliente(joins);
                    }
                    break;

                case "CodigoExpedidor":
                    if (!select.Contains(" CodigoExpedidor, "))
                    {
                        select.Append("ClienteExpedidor.CLI_CODIGO_INTEGRACAO CodigoExpedidor, ");

                        if (!groupBy.Contains("ClienteExpedidor.CLI_CODIGO_INTEGRACAO"))
                            groupBy.Append("ClienteExpedidor.CLI_CODIGO_INTEGRACAO, ");

                        SetarJoinsExpedidorCliente(joins);
                    }
                    break;

                case "CPFCNPJExpedidor":
                    if (!select.Contains(" CPFCNPJExpedidor, "))
                    {
                        select.Append("ExpedidorCTe.PCT_CPF_CNPJ CPFCNPJExpedidor, ");

                        if (!groupBy.Contains("ExpedidorCTe.PCT_CPF_CNPJ"))
                            groupBy.Append("ExpedidorCTe.PCT_CPF_CNPJ, ");

                        SetarJoinsExpedidor(joins);
                    }
                    break;

                case "IEExpedidor":
                    if (!select.Contains(" IEExpedidor, "))
                    {
                        select.Append("ExpedidorCTe.PCT_IERG IEExpedidor, ");
                        groupBy.Append("ExpedidorCTe.PCT_IERG, ");

                        SetarJoinsExpedidor(joins);
                    }
                    break;

                case "Expedidor":
                    if (!select.Contains(" Expedidor, "))
                    {
                        select.Append("ExpedidorCTe.PCT_NOME Expedidor, ");

                        if (!groupBy.Contains("ExpedidorCTe.PCT_NOME"))
                            groupBy.Append("ExpedidorCTe.PCT_NOME, ");

                        SetarJoinsExpedidor(joins);
                    }
                    break;

                case "LocalidadeExpedidor":
                    if (!select.Contains(" LocalidadeExpedidor, "))
                    {
                        select.Append("LocalidadeExpedidor.LOC_DESCRICAO + '-' + LocalidadeExpedidor.UF_SIGLA LocalidadeExpedidor, ");

                        if (!groupBy.Contains("LocalidadeExpedidor.LOC_DESCRICAO"))
                            groupBy.Append("LocalidadeExpedidor.LOC_DESCRICAO, ");

                        if (!groupBy.Contains("LocalidadeExpedidor.UF_SIGLA"))
                            groupBy.Append("LocalidadeExpedidor.UF_SIGLA, ");

                        SetarJoinsExpedidorLocalidade(joins);
                    }
                    break;

                case "UFExpedidor":
                    if (!select.Contains(" UFExpedidor, "))
                    {
                        select.Append("LocalidadeExpedidor.UF_SIGLA UFExpedidor, ");

                        if (!groupBy.Contains("LocalidadeExpedidor.UF_SIGLA"))
                            groupBy.Append("LocalidadeExpedidor.UF_SIGLA, ");

                        SetarJoinsExpedidorLocalidade(joins);
                    }
                    break;

                case "CodigoDocumentoExpedidor":
                    if (!select.Contains(" CodigoDocumentoExpedidor, "))
                    {
                        select.Append("ClienteExpedidor.CLI_CODIGO_DOCUMENTO CodigoDocumentoExpedidor, ");

                        if (!groupBy.Contains("ClienteExpedidor.CLI_CODIGO_DOCUMENTO"))
                            groupBy.Append("ClienteExpedidor.CLI_CODIGO_DOCUMENTO, ");

                        SetarJoinsExpedidorCliente(joins);
                    }
                    break;

                case "CodigoRecebedor":
                    if (!select.Contains(" CodigoRecebedor, "))
                    {
                        select.Append("ClienteRecebedor.CLI_CODIGO_INTEGRACAO CodigoRecebedor, ");

                        if (!groupBy.Contains("ClienteRecebedor.CLI_CODIGO_INTEGRACAO"))
                            groupBy.Append("ClienteRecebedor.CLI_CODIGO_INTEGRACAO, ");

                        SetarJoinsRecebedorCliente(joins);
                    }
                    break;

                case "CPFCNPJRecebedor":
                    if (!select.Contains(" CPFCNPJRecebedor, "))
                    {
                        select.Append("RecebedorCTe.PCT_CPF_CNPJ CPFCNPJRecebedor, ");

                        if (!groupBy.Contains("RecebedorCTe.PCT_CPF_CNPJ"))
                            groupBy.Append("RecebedorCTe.PCT_CPF_CNPJ, ");

                        SetarJoinsRecebedor(joins);
                    }
                    break;

                case "IERecebedor":
                    if (!select.Contains(" IERecebedor, "))
                    {
                        select.Append("RecebedorCTe.PCT_IERG IERecebedor, ");
                        groupBy.Append("RecebedorCTe.PCT_IERG, ");

                        SetarJoinsRecebedor(joins);
                    }
                    break;

                case "Recebedor":
                    if (!select.Contains(" Recebedor, "))
                    {
                        select.Append("RecebedorCTe.PCT_NOME Recebedor, ");

                        if (!groupBy.Contains("RecebedorCTe.PCT_NOME"))
                            groupBy.Append("RecebedorCTe.PCT_NOME, ");

                        SetarJoinsRecebedor(joins);
                    }
                    break;

                case "LocalidadeRecebedor":
                    if (!select.Contains(" LocalidadeRecebedor, "))
                    {
                        select.Append("LocalidadeRecebedor.LOC_DESCRICAO + '-' + LocalidadeRecebedor.UF_SIGLA LocalidadeRecebedor, ");

                        if (!groupBy.Contains("LocalidadeRecebedor.LOC_DESCRICAO"))
                            groupBy.Append("LocalidadeRecebedor.LOC_DESCRICAO, ");

                        if (!groupBy.Contains("LocalidadeRecebedor.UF_SIGLA"))
                            groupBy.Append("LocalidadeRecebedor.UF_SIGLA, ");

                        SetarJoinsRecebedorLocalidade(joins);
                    }
                    break;

                case "UFRecebedor":
                    if (!select.Contains(" UFRecebedor, "))
                    {
                        select.Append("LocalidadeRecebedor.UF_SIGLA UFRecebedor, ");

                        if (!groupBy.Contains("LocalidadeRecebedor.UF_SIGLA"))
                            groupBy.Append("LocalidadeRecebedor.UF_SIGLA, ");

                        SetarJoinsRecebedorLocalidade(joins);
                    }
                    break;

                case "CodigoDocumentoRecebedor":
                    if (!select.Contains(" CodigoDocumentoRecebedor, "))
                    {
                        select.Append("ClienteRecebedor.CLI_CODIGO_DOCUMENTO CodigoDocumentoRecebedor, ");

                        if (!groupBy.Contains("ClienteRecebedor.CLI_CODIGO_DOCUMENTO"))
                            groupBy.Append("ClienteRecebedor.CLI_CODIGO_DOCUMENTO, ");

                        SetarJoinsRecebedorCliente(joins);
                    }
                    break;

                case "CodigoDestinatario":
                    if (!select.Contains(" CodigoDestinatario, "))
                    {
                        if (filtrosPesquisa.RetornarDestinatarioDaNFeQuandoTipoForNFSeNoRelatorioDeCTes)
                        {
                            select.Append(@"
	                            case when MOD_NUM = 'NF'   then 
                                    ( select top 1   DestinatarioNF.CLI_CODIGO_INTEGRACAO  FROM T_XML_NOTA_FISCAL X
                                        INNER JOIN T_CTE_XML_NOTAS_FISCAIS CX ON CX.NFX_CODIGO = X.NFX_CODIGO
                                        INNER JOIN T_CLIENTE as DestinatarioNF on DestinatarioNF.CLI_CGCCPF = X.CLI_CODIGO_DESTINATARIO  
                                        WHERE CX.CON_CODIGO = CTe.CON_CODIGO )
                                else ClienteDestinatario.CLI_CODIGO_INTEGRACAO  end CodigoDestinatario,
                            ");
                        }
                        else
                            select.Append("ClienteDestinatario.CLI_CODIGO_INTEGRACAO CodigoDestinatario, ");

                        if (!groupBy.Contains("ClienteDestinatario.CLI_CODIGO_INTEGRACAO"))
                            groupBy.Append("ClienteDestinatario.CLI_CODIGO_INTEGRACAO, ");

                        if (!groupBy.Contains("ModeloDocumento.MOD_NUM"))
                            groupBy.Append("ModeloDocumento.MOD_NUM, ");

                        SetarJoinsModeloDocumento(joins);
                        SetarJoinsDestinatarioCliente(joins);
                    }
                    break;

                case "CPFCNPJDestinatario":
                    if (!select.Contains(" CPFCNPJDestinatario, "))
                    {
                        if (filtrosPesquisa.RetornarDestinatarioDaNFeQuandoTipoForNFSeNoRelatorioDeCTes)
                        {
                            select.Append(@"
	                            case when MOD_NUM = 'NF'   then 
                                ( select top 1   STR(DestinatarioNF.CLI_CGCCPF, 15, 0) FROM T_XML_NOTA_FISCAL X
                                        INNER JOIN T_CTE_XML_NOTAS_FISCAIS CX ON CX.NFX_CODIGO = X.NFX_CODIGO
                                        INNER JOIN T_CLIENTE as DestinatarioNF on DestinatarioNF.CLI_CGCCPF = X.CLI_CODIGO_DESTINATARIO  
                                        WHERE CX.CON_CODIGO = CTe.CON_CODIGO )
                                else DestinatarioCTe.PCT_CPF_CNPJ end CPFCNPJDestinatario,
                            ");
                        }
                        else
                            select.Append("DestinatarioCTe.PCT_CPF_CNPJ CPFCNPJDestinatario, ");

                        if (!groupBy.Contains("DestinatarioCTe.PCT_CPF_CNPJ"))
                            groupBy.Append("DestinatarioCTe.PCT_CPF_CNPJ, ");

                        if (!groupBy.Contains("ModeloDocumento.MOD_NUM"))
                            groupBy.Append("ModeloDocumento.MOD_NUM, ");

                        SetarJoinsModeloDocumento(joins);
                        SetarJoinsDestinatario(joins);
                    }
                    break;

                case "IEDestinatario":
                    if (!select.Contains(" IEDestinatario, "))
                    {
                        if (filtrosPesquisa.RetornarDestinatarioDaNFeQuandoTipoForNFSeNoRelatorioDeCTes)
                        {
                            select.Append(@"
	                            case when MOD_NUM = 'NF'   then 
                                    ( select top 1   DestinatarioNF.CLI_IERG FROM T_XML_NOTA_FISCAL X
                                        INNER JOIN T_CTE_XML_NOTAS_FISCAIS CX ON CX.NFX_CODIGO = X.NFX_CODIGO
                                        INNER JOIN T_CLIENTE as DestinatarioNF on DestinatarioNF.CLI_CGCCPF = X.CLI_CODIGO_DESTINATARIO  
                                        WHERE CX.CON_CODIGO = CTe.CON_CODIGO )
                                else DestinatarioCTe.PCT_IERG  end IEDestinatario,
                            ");
                        }
                        else
                            select.Append("DestinatarioCTe.PCT_IERG IEDestinatario, ");

                        groupBy.Append("DestinatarioCTe.PCT_IERG, ");

                        if (!groupBy.Contains("ModeloDocumento.MOD_NUM"))
                            groupBy.Append("ModeloDocumento.MOD_NUM, ");

                        SetarJoinsModeloDocumento(joins);
                        SetarJoinsDestinatario(joins);
                    }
                    break;

                case "Destinatario":
                    if (!select.Contains(" Destinatario, "))
                    {
                        if (filtrosPesquisa.RetornarDestinatarioDaNFeQuandoTipoForNFSeNoRelatorioDeCTes)
                        {
                            select.Append(@"
	                                case when MOD_NUM = 'NF'   then 
	                                    ( select top 1   DestinatarioNF.CLI_NOME  FROM T_XML_NOTA_FISCAL X
			                                INNER JOIN T_CTE_XML_NOTAS_FISCAIS CX ON CX.NFX_CODIGO = X.NFX_CODIGO
			                                INNER JOIN T_CLIENTE as DestinatarioNF on DestinatarioNF.CLI_CGCCPF = X.CLI_CODIGO_DESTINATARIO  
                                            WHERE CX.CON_CODIGO = CTe.CON_CODIGO )
	                                else DestinatarioCTe.PCT_NOME  end Destinatario, 
                            ");
                        }
                        else
                            select.Append("DestinatarioCTe.PCT_NOME Destinatario, ");

                        if (!groupBy.Contains("DestinatarioCTe.PCT_NOME"))
                            groupBy.Append("DestinatarioCTe.PCT_NOME, ");

                        if (!groupBy.Contains("ModeloDocumento.MOD_NUM"))
                            groupBy.Append("ModeloDocumento.MOD_NUM, ");

                        SetarJoinsModeloDocumento(joins);
                        SetarJoinsDestinatario(joins);
                    }
                    break;

                case "CodigoEnderecoDestinatario":
                    if (!select.Contains(" CodigoEnderecoDestinatario, "))
                    {
                        if (filtrosPesquisa.RetornarDestinatarioDaNFeQuandoTipoForNFSeNoRelatorioDeCTes)
                            select.Append(@" case when MOD_NUM = 'NF'   then  '' else DestinatarioCTe.PCT_CODIGO_ENDERECO_INTEGRACAO  end CodigoEnderecoDestinatario, ");
                        else
                            select.Append("DestinatarioCTe.PCT_CODIGO_ENDERECO_INTEGRACAO CodigoEnderecoDestinatario, ");

                        if (!groupBy.Contains("DestinatarioCTe.PCT_CODIGO_ENDERECO_INTEGRACAO"))
                            groupBy.Append("DestinatarioCTe.PCT_CODIGO_ENDERECO_INTEGRACAO, ");

                        if (!groupBy.Contains("ModeloDocumento.MOD_NUM"))
                            groupBy.Append("ModeloDocumento.MOD_NUM, ");

                        SetarJoinsModeloDocumento(joins);
                        SetarJoinsDestinatario(joins);
                    }
                    break;

                case "EnderecoDestinatario":
                    if (!select.Contains(" EnderecoDestinatario, "))
                    {
                        if (filtrosPesquisa.RetornarDestinatarioDaNFeQuandoTipoForNFSeNoRelatorioDeCTes)
                        {
                            select.Append(@"
	                            case when MOD_NUM = 'NF'   then 
                                    ( select top 1   DestinatarioNF.CLI_ENDERECO FROM T_XML_NOTA_FISCAL X
                                        INNER JOIN T_CTE_XML_NOTAS_FISCAIS CX ON CX.NFX_CODIGO = X.NFX_CODIGO
                                        INNER JOIN T_CLIENTE as DestinatarioNF on DestinatarioNF.CLI_CGCCPF = X.CLI_CODIGO_DESTINATARIO  
                                        WHERE CX.CON_CODIGO = CTe.CON_CODIGO )
                                else DestinatarioCTe.PCT_ENDERECO end EnderecoDestinatario,
                            ");
                        }
                        else
                            select.Append("DestinatarioCTe.PCT_ENDERECO EnderecoDestinatario, ");

                        if (!groupBy.Contains("DestinatarioCTe.PCT_ENDERECO"))
                            groupBy.Append("DestinatarioCTe.PCT_ENDERECO, ");

                        if (!groupBy.Contains("ModeloDocumento.MOD_NUM"))
                            groupBy.Append("ModeloDocumento.MOD_NUM, ");

                        SetarJoinsModeloDocumento(joins);
                        SetarJoinsDestinatario(joins);
                    }
                    break;

                case "BairroDestinatario":
                    if (!select.Contains(" BairroDestinatario, "))
                    {
                        if (filtrosPesquisa.RetornarDestinatarioDaNFeQuandoTipoForNFSeNoRelatorioDeCTes)
                        {
                            select.Append(@"
	                            case when MOD_NUM = 'NF'   then 
                                ( select top 1   DestinatarioNF.CLI_BAIRRO FROM T_XML_NOTA_FISCAL X
                                        INNER JOIN T_CTE_XML_NOTAS_FISCAIS CX ON CX.NFX_CODIGO = X.NFX_CODIGO
                                        INNER JOIN T_CLIENTE as DestinatarioNF on DestinatarioNF.CLI_CGCCPF = X.CLI_CODIGO_DESTINATARIO  
                                        WHERE CX.CON_CODIGO = CTe.CON_CODIGO )
                                else DestinatarioCTe.PCT_BAIRRO end BairroDestinatario,
                            ");
                        }
                        else
                            select.Append("DestinatarioCTe.PCT_BAIRRO BairroDestinatario, ");

                        if (!groupBy.Contains("DestinatarioCTe.PCT_BAIRRO"))
                            groupBy.Append("DestinatarioCTe.PCT_BAIRRO, ");

                        if (!groupBy.Contains("ModeloDocumento.MOD_NUM"))
                            groupBy.Append("ModeloDocumento.MOD_NUM, ");

                        SetarJoinsModeloDocumento(joins);
                        SetarJoinsDestinatario(joins);
                    }
                    break;

                case "CEPDestinatario":
                    if (!select.Contains(" CEPDestinatario, "))
                    {
                        if (filtrosPesquisa.RetornarDestinatarioDaNFeQuandoTipoForNFSeNoRelatorioDeCTes)
                        {
                            select.Append(@"
	                            case when MOD_NUM = 'NF'   then 
                                    ( select top 1   DestinatarioNF.CLI_CEP FROM T_XML_NOTA_FISCAL X
                                        INNER JOIN T_CTE_XML_NOTAS_FISCAIS CX ON CX.NFX_CODIGO = X.NFX_CODIGO
                                        INNER JOIN T_CLIENTE as DestinatarioNF on DestinatarioNF.CLI_CGCCPF = X.CLI_CODIGO_DESTINATARIO  
                                        WHERE CX.CON_CODIGO = CTe.CON_CODIGO )
                                else DestinatarioCTe.PCT_CEP end CEPDestinatario,
                            ");
                        }
                        else
                            select.Append("DestinatarioCTe.PCT_CEP CEPDestinatario, ");

                        if (!groupBy.Contains("DestinatarioCTe.PCT_CEP"))
                            groupBy.Append("DestinatarioCTe.PCT_CEP, ");

                        if (!groupBy.Contains("ModeloDocumento.MOD_NUM"))
                            groupBy.Append("ModeloDocumento.MOD_NUM, ");

                        SetarJoinsModeloDocumento(joins);
                        SetarJoinsDestinatario(joins);
                    }
                    break;

                case "LocalidadeDestinatario":
                    if (!select.Contains(" LocalidadeDestinatario, "))
                    {
                        if (filtrosPesquisa.RetornarDestinatarioDaNFeQuandoTipoForNFSeNoRelatorioDeCTes)
                        {
                            select.Append(@"
	                            case when MOD_NUM = 'NF'   then 
                                    ( select top 1   localidade.LOC_DESCRICAO + '-' + localidade.UF_SIGLA FROM T_XML_NOTA_FISCAL X
                                        INNER JOIN T_CTE_XML_NOTAS_FISCAIS CX ON CX.NFX_CODIGO = X.NFX_CODIGO
                                        INNER JOIN T_CLIENTE as DestinatarioNF on DestinatarioNF.CLI_CGCCPF = X.CLI_CODIGO_DESTINATARIO  
			                            INNER JOIN T_LOCALIDADES localidade on localidade.LOC_CODIGO = DestinatarioNF.LOC_CODIGO
                                        WHERE CX.CON_CODIGO = CTe.CON_CODIGO )
                                else LocalidadeDestinatario.LOC_DESCRICAO + '-' + LocalidadeDestinatario.UF_SIGLA end LocalidadeDestinatario,
                            ");
                        }
                        else
                            select.Append("LocalidadeDestinatario.LOC_DESCRICAO + '-' + LocalidadeDestinatario.UF_SIGLA LocalidadeDestinatario, ");

                        if (!groupBy.Contains("LocalidadeDestinatario.LOC_DESCRICAO"))
                            groupBy.Append("LocalidadeDestinatario.LOC_DESCRICAO, ");

                        if (!groupBy.Contains("LocalidadeDestinatario.UF_SIGLA"))
                            groupBy.Append("LocalidadeDestinatario.UF_SIGLA, ");

                        if (!groupBy.Contains("ModeloDocumento.MOD_NUM"))
                            groupBy.Append("ModeloDocumento.MOD_NUM, ");

                        SetarJoinsModeloDocumento(joins);
                        SetarJoinsDestinatarioLocalidade(joins);
                    }
                    break;

                case "UFDestinatario":
                    if (!select.Contains(" UFDestinatario, "))
                    {
                        if (filtrosPesquisa.RetornarDestinatarioDaNFeQuandoTipoForNFSeNoRelatorioDeCTes)
                        {
                            select.Append(@"
	                            case when MOD_NUM = 'NF'   then 
                                    ( select top 1   localidade.UF_SIGLA FROM T_XML_NOTA_FISCAL X
                                        INNER JOIN T_CTE_XML_NOTAS_FISCAIS CX ON CX.NFX_CODIGO = X.NFX_CODIGO
                                        INNER JOIN T_CLIENTE as DestinatarioNF on DestinatarioNF.CLI_CGCCPF = X.CLI_CODIGO_DESTINATARIO  
			                            INNER JOIN T_LOCALIDADES localidade on localidade.LOC_CODIGO = DestinatarioNF.LOC_CODIGO
                                        WHERE CX.CON_CODIGO = CTe.CON_CODIGO )
                                else LocalidadeDestinatario.UF_SIGLA end UFDestinatario, 
                            ");
                        }
                        else
                            select.Append("LocalidadeDestinatario.UF_SIGLA UFDestinatario, ");

                        if (!groupBy.Contains("LocalidadeDestinatario.UF_SIGLA"))
                            groupBy.Append("LocalidadeDestinatario.UF_SIGLA, ");

                        if (!groupBy.Contains("ModeloDocumento.MOD_NUM"))
                            groupBy.Append("ModeloDocumento.MOD_NUM, ");

                        SetarJoinsModeloDocumento(joins);
                        SetarJoinsDestinatarioLocalidade(joins);
                    }
                    break;

                case "GrupoDestinatario":
                    if (!select.Contains(" GrupoDestinatario, "))
                    {
                        if (filtrosPesquisa.RetornarDestinatarioDaNFeQuandoTipoForNFSeNoRelatorioDeCTes)
                        {
                            select.Append(@"
	                            case when MOD_NUM = 'NF'   then 
                                    ( select top 1   grupoPessoa.GRP_DESCRICAO FROM T_XML_NOTA_FISCAL X
                                        INNER JOIN T_CTE_XML_NOTAS_FISCAIS CX ON CX.NFX_CODIGO = X.NFX_CODIGO
                                        INNER JOIN T_CLIENTE as DestinatarioNF on DestinatarioNF.CLI_CGCCPF = X.CLI_CODIGO_DESTINATARIO  
			                            INNER JOIN T_GRUPO_PESSOAS grupoPessoa on grupoPessoa.GRP_CODIGO = DestinatarioNF.GRP_CODIGO 
                                        WHERE CX.CON_CODIGO = CTe.CON_CODIGO )
                                else GrupoPessoaDestinatario.GRP_DESCRICAO end GrupoDestinatario,                            ");
                        }
                        else
                            select.Append("GrupoPessoaDestinatario.GRP_DESCRICAO GrupoDestinatario, ");

                        if (!groupBy.Contains("GrupoPessoaDestinatario.GRP_DESCRICAO"))
                            groupBy.Append("GrupoPessoaDestinatario.GRP_DESCRICAO, ");

                        if (!groupBy.Contains("ModeloDocumento.MOD_NUM"))
                            groupBy.Append("ModeloDocumento.MOD_NUM, ");

                        SetarJoinsModeloDocumento(joins);
                        SetarJoinsDestinatarioGrupoPessoa(joins);
                    }
                    break;

                case "CategoriaDestinatario":
                    if (!select.Contains(" CategoriaDestinatario, "))
                    {
                        if (filtrosPesquisa.RetornarDestinatarioDaNFeQuandoTipoForNFSeNoRelatorioDeCTes)
                        {
                            select.Append(@"
	                            case when MOD_NUM = 'NF'   then 
                                    ( select top 1   categoria.CTP_DESCRICAO FROM T_XML_NOTA_FISCAL X
                                        INNER JOIN T_CTE_XML_NOTAS_FISCAIS CX ON CX.NFX_CODIGO = X.NFX_CODIGO
                                        INNER JOIN T_CLIENTE as DestinatarioNF on DestinatarioNF.CLI_CGCCPF = X.CLI_CODIGO_DESTINATARIO  
			                            INNER JOIN T_CATEGORIA_PESSOA categoria on categoria.CTP_CODIGO = DestinatarioNF.CTP_CODIGO 
                                        WHERE CX.CON_CODIGO = CTe.CON_CODIGO )
                                else CategoriaDestinatario.CTP_DESCRICAO end CategoriaDestinatario, 
                            ");
                        }
                        else
                            select.Append("CategoriaDestinatario.CTP_DESCRICAO CategoriaDestinatario, ");

                        if (!groupBy.Contains("CategoriaDestinatario.CTP_DESCRICAO"))
                            groupBy.Append("CategoriaDestinatario.CTP_DESCRICAO, ");

                        if (!groupBy.Contains("ModeloDocumento.MOD_NUM"))
                            groupBy.Append("ModeloDocumento.MOD_NUM, ");

                        SetarJoinsModeloDocumento(joins);
                        SetarJoinsDestinatarioCategoria(joins);
                    }
                    break;

                case "CodigoDocumentoDestinatario":
                    if (!select.Contains(" CodigoDocumentoDestinatario, "))
                    {
                        if (filtrosPesquisa.RetornarDestinatarioDaNFeQuandoTipoForNFSeNoRelatorioDeCTes)
                        {
                            select.Append(@"
	                                case when MOD_NUM = 'NF'   then 
	                                ( select top 1   DestinatarioNF.CLI_CODIGO_DOCUMENTO  FROM T_XML_NOTA_FISCAL X
			                                INNER JOIN T_CTE_XML_NOTAS_FISCAIS CX ON CX.NFX_CODIGO = X.NFX_CODIGO
			                                INNER JOIN T_CLIENTE as DestinatarioNF on DestinatarioNF.CLI_CGCCPF = X.CLI_CODIGO_DESTINATARIO  
                                            WHERE CX.CON_CODIGO = CTe.CON_CODIGO )
	                                else ClienteDestinatario.CLI_CODIGO_DOCUMENTO end CodigoDocumentoDestinatario, 
                            ");
                        }
                        else
                            select.Append("ClienteDestinatario.CLI_CODIGO_DOCUMENTO CodigoDocumentoDestinatario, ");

                        if (!groupBy.Contains("ClienteDestinatario.CLI_CODIGO_DOCUMENTO"))
                            groupBy.Append("ClienteDestinatario.CLI_CODIGO_DOCUMENTO, ");

                        if (!groupBy.Contains("ModeloDocumento.MOD_NUM"))
                            groupBy.Append("ModeloDocumento.MOD_NUM, ");

                        SetarJoinsModeloDocumento(joins);
                        SetarJoinsDestinatarioCliente(joins);
                    }
                    break;
                // ate aqui 

                case "CPFCNPJTomador":
                    if (!select.Contains(" CPFCNPJTomador, "))
                    {
                        select.Append("TomadorPagadorCTe.PCT_CPF_CNPJ CPFCNPJTomador, ");

                        if (!groupBy.Contains("TomadorPagadorCTe.PCT_CPF_CNPJ"))
                            groupBy.Append("TomadorPagadorCTe.PCT_CPF_CNPJ, ");

                        SetarJoinsTomador(joins);
                    }
                    break;

                case "CodigoTomador":
                    if (!select.Contains(" CodigoTomador, "))
                    {
                        select.Append("TomadorPagadorCTe.CLI_CODIGO CodigoTomador, ");

                        if (!groupBy.Contains("TomadorPagadorCTe.CLI_CODIGO"))
                            groupBy.Append("TomadorPagadorCTe.CLI_CODIGO, ");

                        SetarJoinsTomador(joins);
                    }
                    break;

                case "CodigoIntegracaoTomador":
                    if (!select.Contains(" CodigoIntegracaoTomador, "))
                    {
                        select.Append("TomadorPagadorCTe.PCT_CODIGO_INTEGRACAO CodigoIntegracaoTomador, ");

                        if (!groupBy.Contains("TomadorPagadorCTe.PCT_CODIGO_INTEGRACAO"))
                            groupBy.Append("TomadorPagadorCTe.PCT_CODIGO_INTEGRACAO, ");

                        SetarJoinsTomador(joins);
                    }
                    break;

                case "IETomador":
                    if (!select.Contains(" IETomador, "))
                    {
                        select.Append("TomadorPagadorCTe.PCT_IERG IETomador, ");
                        groupBy.Append("TomadorPagadorCTe.PCT_IERG, ");

                        SetarJoinsTomador(joins);
                    }
                    break;

                case "Tomador":
                    if (!select.Contains(" Tomador, "))
                    {
                        select.Append("TomadorPagadorCTe.PCT_NOME Tomador, ");

                        if (!groupBy.Contains("CTe.CON_TOMADOR_PAGADOR_CTE"))
                            groupBy.Append("CTe.CON_TOMADOR_PAGADOR_CTE, ");

                        if (!groupBy.Contains("TomadorPagadorCTe.PCT_NOME"))
                            groupBy.Append("TomadorPagadorCTe.PCT_NOME, ");

                        SetarJoinsTomador(joins);
                    }
                    break;

                case "GrupoTomador":
                    if (!select.Contains(" GrupoTomador, "))
                    {
                        select.Append("GrupoTomadorPagadorCTe.GRP_DESCRICAO GrupoTomador, ");

                        if (!groupBy.Contains("GrupoTomadorPagadorCTe.GRP_DESCRICAO"))
                            groupBy.Append("GrupoTomadorPagadorCTe.GRP_DESCRICAO, ");

                        SetarJoinsTomadorGrupo(joins);
                    }
                    break;

                case "UFTomador":
                    if (!select.Contains(" UFTomador, "))
                    {
                        select.Append("LocalidadeTomador.UF_SIGLA UFTomador, ");

                        if (!groupBy.Contains("LocalidadeTomador.UF_SIGLA"))
                            groupBy.Append("LocalidadeTomador.UF_SIGLA, ");

                        SetarJoinsTomadorLocalidade(joins);
                    }
                    break;

                case "CodigoDocumentoTomador":
                    if (!select.Contains(" CodigoDocumentoTomador, "))
                    {
                        select.Append("ClienteTomador.CLI_CODIGO_DOCUMENTO CodigoDocumentoTomador, ");

                        if (!groupBy.Contains("ClienteTomador.CLI_CODIGO_DOCUMENTO"))
                            groupBy.Append("ClienteTomador.CLI_CODIGO_DOCUMENTO, ");

                        SetarJoinsTomadorCliente(joins);
                    }
                    break;

                case "Rotas":
                    if (!select.Contains(" Rotas"))
                    {
                        select.Append(
                            @"substring((
                                select ', ' + ISNULL(X.NF_ROTA, '') 
                                  FROM T_XML_NOTA_FISCAL X
                                  JOIN T_CTE_XML_NOTAS_FISCAIS CX ON CX.NFX_CODIGO = X.NFX_CODIGO
                                 WHERE X.NF_ROTA IS NOT NULL AND X.NF_ROTA <> '' 
                                   AND CX.CON_CODIGO = CTe.CON_CODIGO
                                   for xml path('')
                            ), 3, 1000) Rotas,"
                        );

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "IBGEInicioPrestacao":
                    if (!select.Contains(" IBGEInicioPrestacao,"))
                    {
                        select.Append(" InicioPrestacaoCTe.LOC_IBGE IBGEInicioPrestacao, ");
                        groupBy.Append("InicioPrestacaoCTe.LOC_IBGE, ");

                        SetarJoinsLocalidadeInicioPrestacao(joins);
                    }
                    break;

                case "InicioPrestacao":
                    if (!select.Contains(" InicioPrestacao,"))
                    {
                        select.Append(" InicioPrestacaoCTe.LOC_DESCRICAO + '-' + InicioPrestacaoCTe.UF_SIGLA InicioPrestacao, ");
                        groupBy.Append("InicioPrestacaoCTe.LOC_DESCRICAO, ");

                        if (!groupBy.Contains("InicioPrestacaoCTe.UF_SIGLA"))
                            groupBy.Append("InicioPrestacaoCTe.UF_SIGLA, ");

                        SetarJoinsLocalidadeInicioPrestacao(joins);
                    }
                    break;

                case "UFInicioPrestacao":
                    if (!select.Contains("UFInicioPrestacao"))
                    {
                        select.Append(" InicioPrestacaoCTe.UF_SIGLA UFInicioPrestacao, ");

                        if (!groupBy.Contains("InicioPrestacaoCTe.UF_SIGLA"))
                            groupBy.Append("InicioPrestacaoCTe.UF_SIGLA, ");

                        SetarJoinsLocalidadeInicioPrestacao(joins);
                    }
                    break;

                case "IBGEFimPrestacao":
                    if (!select.Contains(" IBGEFimPrestacao,"))
                    {
                        select.Append(" FimPrestacaoCTe.LOC_IBGE IBGEFimPrestacao, ");
                        groupBy.Append("FimPrestacaoCTe.LOC_IBGE, ");

                        SetarJoinsLocalidadeFimPrestacao(joins);
                    }
                    break;

                case "FimPrestacao":
                    if (!select.Contains(" FimPrestacao,"))
                    {
                        select.Append(" FimPrestacaoCTe.LOC_DESCRICAO + '-' + FimPrestacaoCTe.UF_SIGLA FimPrestacao, ");
                        groupBy.Append("FimPrestacaoCTe.LOC_DESCRICAO, ");

                        if (!groupBy.Contains("FimPrestacaoCTe.UF_SIGLA"))
                            groupBy.Append("FimPrestacaoCTe.UF_SIGLA, ");

                        SetarJoinsLocalidadeFimPrestacao(joins);
                    }
                    break;

                case "UFFimPrestacao":
                    if (!select.Contains("UFFimPrestacao"))
                    {
                        select.Append(" FimPrestacaoCTe.UF_SIGLA UFFimPrestacao, ");

                        if (!groupBy.Contains("FimPrestacaoCTe.UF_SIGLA"))
                            groupBy.Append(" FimPrestacaoCTe.UF_SIGLA, ");

                        SetarJoinsLocalidadeFimPrestacao(joins);
                    }
                    break;

                case "NomeFantasiaTransportador":
                    if (!select.Contains("TransportadorCTe.EMP_FANTASIA"))
                    {
                        select.Append("TransportadorCTe.EMP_FANTASIA NomeFantasiaTransportador, ");
                        groupBy.Append("TransportadorCTe.EMP_FANTASIA, ");

                        SetarJoinsTransportador(joins);
                    }
                    break;

                case "RazaoSocialTransportador":
                    if (!select.Contains("TransportadorCTe.EMP_RAZAO"))
                    {
                        select.Append("TransportadorCTe.EMP_RAZAO RazaoSocialTransportador, ");
                        groupBy.Append("TransportadorCTe.EMP_RAZAO, ");

                        SetarJoinsTransportador(joins);
                    }
                    break;

                case "CNPJTransportadorFormatada":
                    if (!select.Contains("TransportadorCTe.EMP_CNPJ"))
                    {
                        select.Append("TransportadorCTe.EMP_CNPJ CNPJTransportador, ");
                        groupBy.Append("TransportadorCTe.EMP_CNPJ, ");

                        SetarJoinsTransportador(joins);
                    }
                    break;

                case "AbreviacaoModeloDocumentoFiscal":
                    if (!select.Contains("ModeloDocumento"))
                    {
                        select.Append("ModeloDocumento.MOD_ABREVIACAO AbreviacaoModeloDocumentoFiscal, ");
                        groupBy.Append("ModeloDocumento.MOD_ABREVIACAO, ");

                        SetarJoinsModeloDocumento(joins);
                    }
                    break;

                case "AliquotaISS":
                    if (!somenteContarNumeroRegistros && !select.Contains("AliquotaISS"))
                    {
                        select.Append("CTe.CON_ALIQUOTA_ISS AliquotaISS, ");

                        if (!groupBy.Contains("CTe.CON_ALIQUOTA_ISS"))
                            groupBy.Append("CTe.CON_ALIQUOTA_ISS, ");
                    }
                    break;

                case "AliquotaICMS":
                    if (!somenteContarNumeroRegistros && !select.Contains("AliquotaICMS"))
                    {
                        select.Append("CTe.CON_ALIQ_ICMS AliquotaICMS, ");

                        if (!groupBy.Contains("CTe.CON_ALIQ_ICMS"))
                            groupBy.Append("CTe.CON_ALIQ_ICMS, ");
                    }
                    break;

                case "ValorICMS":
                    if (!somenteContarNumeroRegistros && !select.Contains("ValorICMS"))
                        select.Append("SUM(CTe.CON_VAL_ICMS) ValorICMS, ");
                    break;

                case "BaseCalculoICMS":
                    if (!somenteContarNumeroRegistros && !select.Contains("BaseCalculoICMS"))
                        select.Append("SUM(CTe.CON_BC_ICMS) BaseCalculoICMS, ");
                    break;

                case "ValorISS":
                    if (!somenteContarNumeroRegistros && !select.Contains("ValorISS"))
                        select.Append("SUM(CTe.CON_VALOR_ISS) ValorISS, ");
                    break;

                case "ValorISSRetido":
                    if (!somenteContarNumeroRegistros && !select.Contains("ValorISSRetido"))
                        select.Append("SUM(CTe.CON_VALOR_ISS_RETIDO) ValorISSRetido, ");
                    break;

                case "ValorFrete":
                    if (!somenteContarNumeroRegistros && !select.Contains("ValorFrete"))
                    {
                        if (filtrosPesquisa.NaoExibirValorFreteCTeComplementar)
                            select.Append("(CASE CTe.CON_TIPO_CTE WHEN 1 THEN 0 ELSE CTe.CON_VALOR_FRETE END) ValorFrete, ");
                        else
                            select.Append("CTe.CON_VALOR_FRETE ValorFrete, ");

                        if (!groupBy.Contains("CTe.CON_VALOR_FRETE"))
                            groupBy.Append("CTe.CON_VALOR_FRETE, ");

                        if (!groupBy.Contains("CTe.CON_TIPO_CTE"))
                            groupBy.Append("CTe.CON_TIPO_CTE, ");
                    }
                    break;

                case "ValorReceber":
                    if (!somenteContarNumeroRegistros && !select.Contains("ValorReceber"))
                        select.Append("CTe.CON_VALOR_RECEBER ValorReceber, ");

                    if (!somenteContarNumeroRegistros && !groupBy.Contains("CTe.CON_VALOR_RECEBER"))
                        groupBy.Append("CTe.CON_VALOR_RECEBER, ");
                    break;

                case "ValorPrestacao":
                    if (!somenteContarNumeroRegistros && !select.Contains("ValorPrestacao"))
                        select.Append("SUM(CTe.CON_VALOR_PREST_SERVICO) ValorPrestacao, ");
                    break;

                case "ValorSemImposto":
                    if (!somenteContarNumeroRegistros && !select.Contains("ValorSemImposto"))
                        select.Append("SUM(CTe.CON_VALOR_PREST_SERVICO - CTe.CON_VAL_ICMS - CTe.CON_VALOR_ISS) ValorSemImposto, ");
                    break;

                case "ValorMercadoria":
                    if (!somenteContarNumeroRegistros && !select.Contains("ValorMercadoria"))
                        select.Append("SUM(CTe.CON_VALOR_TOTAL_MERC) ValorMercadoria, ");
                    break;

                case "CSTIBSCBS":
                    if (!select.Contains(" CSTIBSCBS, "))
                    {
                        select.Append("CTe.CON_CST_IBSCBS CSTIBSCBS, ");
                        groupBy.Append("CTe.CON_CST_IBSCBS, ");
                    }
                    break;

                case "ClassificacaoTributariaIBSCBS":
                    if (!select.Contains(" ClassificacaoTributariaIBSCBS, "))
                    {
                        select.Append("CTe.CON_CLASSIFICACAO_TRIBUTARIA_IBSCBS ClassificacaoTributariaIBSCBS, ");
                        groupBy.Append("CTe.CON_CLASSIFICACAO_TRIBUTARIA_IBSCBS, ");
                    }
                    break;

                case "BaseCalculoIBSCBS":
                    if (!select.Contains(" BaseCalculoIBSCBS, "))
                    {
                        select.Append("sum(CTe.CON_BASE_CALCULO_IBSCBS) BaseCalculoIBSCBS, ");
                    }
                    break;

                case "AliquotaIBSEstadual":
                    if (!select.Contains(" AliquotaIBSEstadual, "))
                    {
                        select.Append("sum(CTe.CON_ALIQUOTA_IBS_ESTADUAL) AliquotaIBSEstadual, ");
                    }
                    break;

                case "PercentualReducaoIBSEstadual":
                    if (!select.Contains(" PercentualReducaoIBSEstadual, "))
                    {
                        select.Append("sum(CTe.CON_PERCENTUAL_REDUCAO_IBS_ESTADUAL) PercentualReducaoIBSEstadual, ");
                    }
                    break;

                case "ValorIBSEstadual":
                    if (!select.Contains(" ValorIBSEstadual, "))
                    {
                        select.Append("sum(CTe.CON_VALOR_IBS_ESTADUAL) ValorIBSEstadual, ");
                    }
                    break;

                case "AliquotaIBSMunicipal":
                    if (!select.Contains(" AliquotaIBSMunicipal, "))
                    {
                        select.Append("sum(CTe.CON_ALIQUOTA_IBS_MUNICIPAL) AliquotaIBSMunicipal, ");
                    }
                    break;

                case "PercentualReducaoIBSMunicipal":
                    if (!select.Contains(" PercentualReducaoIBSMunicipal, "))
                    {
                        select.Append("sum(CTe.CON_PERCENTUAL_REDUCAO_IBS_MUNICIPAL) PercentualReducaoIBSMunicipal, ");
                    }
                    break;

                case "ValorIBSMunicipal":
                    if (!select.Contains(" ValorIBSMunicipal, "))
                    {
                        select.Append("sum(CTe.CON_VALOR_IBS_MUNICIPAL) ValorIBSMunicipal, ");
                    }
                    break;

                case "AliquotaCBS":
                    if (!select.Contains(" AliquotaCBS, "))
                    {
                        select.Append("sum(CTe.CON_ALIQUOTA_CBS) AliquotaCBS, ");
                    }
                    break;

                case "PercentualReducaoCBS":
                    if (!select.Contains(" PercentualReducaoCBS, "))
                    {
                        select.Append("sum(CTe.CON_PERCENTUAL_REDUCAO_CBS) PercentualReducaoCBS, ");
                    }
                    break;

                case "ValorCBS":
                    if (!select.Contains(" ValorCBS, "))
                    {
                        select.Append("sum(CTe.CON_VALOR_CBS) ValorCBS, ");
                    }
                    break;

                case "Veiculo":
                    if (!select.Contains(" Veiculo,"))
                    {
                        select.Append("substring((select ', ' + veiculo1.VEI_PLACA from T_CTE_VEICULO veiculoCTe1 inner join T_VEICULO veiculo1 on veiculoCTe1.VEI_CODIGO = veiculo1.VEI_CODIGO where veiculoCTe1.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 1000) Veiculo, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "VeiculoUltimoMDFe":
                    if (!select.Contains(" VeiculoUltimoMDFe,"))
                    {
                        select.Append("(select TOP 1 MDFeVeiculo.MDV_PLACA from T_MDFE_MUNICIPIO_DESCARREGAMENTO_DOC MDFeMunicipioDescarregamentoDoc inner join T_MDFE_MUNICIPIO_DESCARREGAMENTO MDFeMunicipioDescarregamento on MDFeMunicipioDescarregamento.MDD_CODIGO = MDFeMunicipioDescarregamentoDoc.MDD_CODIGO inner join T_MDFE MDFe on MDFe.MDF_CODIGO = MDFeMunicipioDescarregamento.MDF_CODIGO inner join T_MDFE_VEICULO MDFeVeiculo on MDFeVeiculo.MDF_CODIGO = MDFe.MDF_CODIGO WHERE CON_CODIGO = CTe.CON_CODIGO ORDER BY MDFe.MDF_DATA_EMISSAO DESC) VeiculoUltimoMDFe, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "ModeloVeiculoCarga":
                    if (!select.Contains(" ModeloVeiculoCarga "))
                    {
                        select.Append("substring((SELECT ', ' + MVC.MVC_DESCRICAO FROM T_CTE_VEICULO CteVei INNER JOIN T_VEICULO VEI ON CteVei.VEI_CODIGO = VEI.VEI_CODIGO LEFT JOIN T_MODELO_VEICULAR_CARGA MVC ON VEI.MVC_CODIGO = MVC.MVC_CODIGO WHERE CteVei.CON_CODIGO = CTe.CON_CODIGO for xml path('')),3, 1000) ModeloVeiculoCarga, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "NumeroUltimoMDFe":
                    if (!select.Contains(" NumeroUltimoMDFe,"))
                    {
                        select.Append("(select TOP 1 CONVERT(NVARCHAR(50), MDFe.MDF_NUMERO) from T_MDFE_MUNICIPIO_DESCARREGAMENTO_DOC MDFeMunicipioDescarregamentoDoc inner join T_MDFE_MUNICIPIO_DESCARREGAMENTO MDFeMunicipioDescarregamento on MDFeMunicipioDescarregamento.MDD_CODIGO = MDFeMunicipioDescarregamentoDoc.MDD_CODIGO inner join T_MDFE MDFe on MDFe.MDF_CODIGO = MDFeMunicipioDescarregamento.MDF_CODIGO WHERE CON_CODIGO = CTe.CON_CODIGO ORDER BY MDFe.MDF_DATA_EMISSAO DESC) NumeroUltimoMDFe, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "DescricaoUltimaOcorrencia":
                    if (!select.Contains(" DescricaoUltimaOcorrencia,"))
                    {
                        select.Append("(select TOP 1 Ocorrencia.OCO_DESCRICAO from T_CTE_OCORRENCIA OcorrenciaCTe inner join T_OCORRENCIA Ocorrencia on Ocorrencia.OCO_CODIGO = OcorrenciaCTe.OCO_CODIGO WHERE CON_CODIGO = CTe.CON_CODIGO ORDER BY OcorrenciaCTe.COC_DATA_OCORRENCIA DESC) DescricaoUltimaOcorrencia, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "DataOcorrenciaFinal":
                    if (!select.Contains(" DataOcorrenciaFinal,"))
                    {
                        select.Append("(select TOP 1 CONVERT(nvarchar(10), OcorrenciaCTe.COC_DATA_OCORRENCIA, 103) + ' ' + CONVERT(nvarchar(5), OcorrenciaCTe.COC_DATA_OCORRENCIA, 108) from T_CTE_OCORRENCIA OcorrenciaCTe inner join T_OCORRENCIA Ocorrencia on Ocorrencia.OCO_CODIGO = OcorrenciaCTe.OCO_CODIGO WHERE OcorrenciaCTe.CON_CODIGO = CTe.CON_CODIGO AND Ocorrencia.OCO_TIPO = 'F' ORDER BY OcorrenciaCTe.COC_DATA_OCORRENCIA DESC) DataOcorrenciaFinal, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "Ocorrencia":
                    if (!select.Contains(" Ocorrencia, "))
                        select.Append("substring((select distinct ', ' + CONVERT(varchar(100), CargaOcorrencia.COC_NUMERO_CONTRATO) from T_CARGA_CTE_COMPLEMENTO_INFO ComplementoInfo left join T_CARGA_OCORRENCIA CargaOcorrencia on CargaOcorrencia.COC_CODIGO = ComplementoInfo.COC_CODIGO WHERE ComplementoInfo.CON_CODIGO = Cte.CON_CODIGO for xml path('')), 3, 200) Ocorrencia, ");

                    break;

                case "TipoOcorrencia":
                    if (!select.Contains(" TipoOcorrencia, "))
                    {
                        select.Append(" TipoOcorrencia.OCO_DESCRICAO TipoOcorrencia, ");
                        groupBy.Append("TipoOcorrencia.OCO_DESCRICAO, ");

                        SetarJoinsTipoOcorrencia(joins);
                    }
                    break;

                case "NumeroCTeTerceiroOcorrencia":
                    if (!select.Contains(" NumeroCTeTerceiroOcorrencia, "))
                    {
                        select.Append("CAST(CTeTerceiroOcorrencia.CPS_NUMERO AS NVARCHAR(20)) NumeroCTeTerceiroOcorrencia, ");
                        groupBy.Append("CTeTerceiroOcorrencia.CPS_NUMERO, ");

                        SetarJoinsCTeTerceiroOcorrencia(joins);
                    }
                    break;

                case "NomeProprietarioVeiculo":
                    if (!select.Contains(" NomeProprietarioVeiculo "))
                    {
                        select.Append("(select TOP(1) ProprietarioVeiculoCTe.PVE_NOME from T_CTE_VEICULO_PROPRIETARIO ProprietarioVeiculoCTe inner join T_CTE_VEICULO VeiculoCTe on ProprietarioVeiculoCTe.PVE_CODIGO = VeiculoCTe.PVE_CODIGO where VeiculoCTe.CON_CODIGO = CTe.CON_CODIGO) NomeProprietarioVeiculo, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "SegmentoVeiculo":
                    if (!select.Contains(" SegmentoVeiculo "))
                    {
                        select.Append("substring((select ', ' + SegmentoVeiculo.VSE_DESCRICAO from T_CTE_VEICULO VeiculoCTe inner join T_VEICULO Veiculo on Veiculo.VEI_CODIGO = VeiculoCTe.VEI_CODIGO inner join T_VEICULO_SEGMENTO SegmentoVeiculo on SegmentoVeiculo.VSE_CODIGO = Veiculo.VSE_CODIGO where VeiculoCTe.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 1000) SegmentoVeiculo, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "Motorista":
                    if (!select.Contains("Motorista"))
                    {
                        select.Append("substring((select ', ' + motoristaCTe1.CMO_NOME_MOTORISTA from T_CTE_MOTORISTA motoristaCTe1 where motoristaCTe1.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 1000) Motorista, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "PesoKg":
                    if (!select.Contains("PesoKg"))
                    {
                        select.Append("CASE WHEN CTe.CON_PESO > 0 THEN CTe.CON_PESO ELSE (select SUM(pesoKgCTe.ICA_QTD) from T_CTE_INF_CARGA pesoKgCTe where pesoKgCTe.ICA_UN = '01' and pesoKgCTe.CON_CODIGO = CTe.CON_CODIGO ) END PesoKg, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");

                        if (!groupBy.Contains("CTe.CON_PESO,"))
                            groupBy.Append("CTe.CON_PESO, ");
                    }
                    break;

                case "PesoLiquidoKg":
                    if (!select.Contains("PesoLiquidoKg"))
                    {
                        select.Append("CTe.CON_PESO_LIQUIDO PesoLiquidoKg, ");

                        if (!groupBy.Contains("CTe.CON_PESO_LIQUIDO"))
                            groupBy.Append("CTe.CON_PESO_LIQUIDO, ");
                    }
                    break;

                case "Volumes":
                    if (!select.Contains("Volumes"))
                    {
                        select.Append(" CASE WHEN CTe.CON_VOLUMES > 0 THEN CONVERT(int, CTe.CON_VOLUMES) ELSE (SELECT SUM(NotasFiscaisCTe.NFC_VOLUME) FROM T_CTE_DOCS NotasFiscaisCTe WHERE NotasFiscaisCTe.CON_CODIGO = CTe.CON_CODIGO) END Volumes, ");

                        if (!groupBy.Contains("CTe.CON_VOLUMES"))
                            groupBy.Append("CTe.CON_VOLUMES, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "MetrosCubicos":
                    if (!select.Contains("MetrosCubicos"))
                    {
                        select.Append(" SUM(CTe.CON_METROS_CUBICOS) MetrosCubicos, ");
                    }
                    break;

                case "Pallets":
                    if (!select.Contains("Pallets"))
                    {
                        select.Append(" SUM(CTe.CON_PALLETS) Pallets, ");
                    }
                    break;

                case "Observacao":
                    if (!select.Contains("Observacao"))
                    {
                        select.Append("CTe.CON_OBSGERAIS Observacao, ");
                        groupBy.Append("CTe.CON_OBSGERAIS, ");
                    }
                    break;

                case "NumeroMinuta":
                    if (!select.Contains("NumeroMinuta"))
                    {
                        select.Append("COALESCE(DocumentoNatura.DTN_NUMERO, DocumentoAvon.MAV_NUMERO) NumeroMinuta, ");

                        joins.Append(" left outer join T_NATURA_DOCUMENTO_TRANSPORTE_NOTA_FISCAL NotaFiscalNatura on NotaFiscalNatura.NDT_CODIGO = (select TOP 1 NDT_CODIGO FROM T_NATURA_DOCUMENTO_TRANSPORTE_NOTA_FISCAL where CON_CODIGO = CTe.CON_CODIGO) ");
                        joins.Append(" left outer join T_NATURA_DOCUMENTO_TRANSPORTE DocumentoNatura on NotaFiscalNatura.DTN_CODIGO = DocumentoNatura.DTN_CODIGO ");
                        joins.Append(" left outer join T_AVON_MANIFESTO_DOCUMENTO NotaFiscalAvon on NotaFiscalAvon.CON_CODIGO = CTe.CON_CODIGO ");
                        joins.Append(" left outer join T_AVON_MANIFESTO DocumentoAvon on DocumentoAvon.MAV_CODIGO = NotaFiscalAvon.MAV_CODIGO ");

                        groupBy.Append("DocumentoAvon.MAV_NUMERO, DocumentoNatura.DTN_NUMERO, ");
                    }
                    break;

                case "NumeroNotaFiscal":
                    if (!select.Contains("NumeroNotaFiscal"))
                    {
                        select.Append("substring((select DISTINCT ', ' + notaFiscal1.NFC_NUMERO from T_CTE_DOCS notaFiscal1 join T_CARGA_CTE _cargaCte on _cargaCte.CON_CODIGO = notaFiscal1.CON_CODIGO LEFT OUTER JOIN T_CANHOTO_NOTA_FISCAL _canhoto ON _canhoto.CAR_CODIGO = _cargaCte.CAR_CODIGO where _cargaCte.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 100000) NumeroNotaFiscal, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "NumeroPedidoNotaFiscal":
                    if (!select.Contains("NumeroPedidoNotaFiscal"))
                    {
                        select.Append("substring((select DISTINCT ', ' + notaFiscal1.NFC_NUMERO_PEDIDO from T_CTE_DOCS notaFiscal1 join T_CARGA_CTE _cargaCte on _cargaCte.CON_CODIGO = notaFiscal1.CON_CODIGO LEFT OUTER JOIN T_CANHOTO_NOTA_FISCAL _canhoto ON _canhoto.CAR_CODIGO = _cargaCte.CAR_CODIGO where notaFiscal1.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 1000) NumeroPedidoNotaFiscal, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "ChaveNotaFiscal":
                    if (!select.Contains("ChaveNotaFiscal"))
                    {
                        select.Append("substring((select ', ' + notaFiscal2.NFC_CHAVENFE from T_CTE_DOCS notaFiscal2 where notaFiscal2.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 15000) ChaveNotaFiscal, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "DataNFEmissao":
                    if (!select.Contains(" DataNFEmissao, "))
                    {
                        select.Append("(select TOP 1 CASE notaFiscal1.NFC_DATAEMISSAO WHEN NULL THEN '' ELSE convert(nvarchar(10), notaFiscal1.NFC_DATAEMISSAO, 103) END from T_CTE_DOCS notaFiscal1 where notaFiscal1.CON_CODIGO = CTe.CON_CODIGO ORDER BY notaFiscal1.NFC_DATAEMISSAO) DataNFEmissao, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "NumeroDocumentoAnterior":
                    if (!select.Contains(" NumeroDocumentoAnterior, "))
                    {
                        select.Append("(select TOP 1 NFC_NUMERO from T_CTE_DOCS notaFiscal1 where notaFiscal1.CON_CODIGO = CTe.CON_CODIGO ORDER BY notaFiscal1.NFC_DATAEMISSAO) NumeroDocumentoAnterior, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "DescricaoTipoPagamento":
                    if (!select.Contains(" TipoPagamento, "))
                    {
                        select.Append("CTe.CON_PAGOAPAGAR as TipoPagamento, ");

                        if (!groupBy.Contains("CTe.CON_PAGOAPAGAR"))
                            groupBy.Append("CTe.CON_PAGOAPAGAR, ");
                    }
                    break;

                case "Frota":
                    if (!select.Contains(" Frota, "))
                    {
                        select.Append("reverse(stuff(reverse((select (case veiculo1.VEI_NUMERO_FROTA when null then '' when '' then '' else veiculo1.VEI_NUMERO_FROTA + ', ' end) from T_CTE_VEICULO veiculoCTe1 inner join T_VEICULO veiculo1 on veiculoCTe1.VEI_CODIGO = veiculo1.VEI_CODIGO where veiculoCTe1.CON_CODIGO = CTe.CON_CODIGO for xml path(''))), 1, 2, '')) Frota, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "Pago":
                    if (!select.Contains(" Pago"))
                    {
                        select.Append(@"(select top 1 
	                                    CASE
                                            WHEN Fatura.FAT_SITUACAO = 2 THEN 'Sim'
                                            ELSE 'Não'
                                        END Pago
	                                    from T_DOCUMENTO_FATURAMENTO Documento 
	                                    LEFT OUTER JOIN T_FATURA_DOCUMENTO FaturaDocumento ON FaturaDocumento.DFA_CODIGO = Documento.DFA_CODIGO
	                                    LEFT OUTER JOIN T_FATURA Fatura ON Fatura.FAT_CODIGO = FaturaDocumento.FAT_CODIGO 
	                                    where Documento.CON_CODIGO = CTe.CON_CODIGO AND Documento.DFA_SITUACAO <> 2
	                                    order by Documento.DFA_CODIGO desc
                                      ) Pago, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "CFOP":
                    if (!select.Contains(" CFOP"))
                    {
                        select.Append("CFOP.CFO_CFOP CFOP, ");
                        groupBy.Append("CFOP.CFO_CFOP, ");

                        SetarJoinsCfop(joins);
                    }
                    break;

                case "CSTFormatada":
                    if (!select.Contains(" CST"))
                    {
                        select.Append("CTe.CON_CST CST, ");
                        groupBy.Append("CTe.CON_CST, ");
                    }
                    break;

                case "DataPrevistaEntrega":
                    if (!select.Contains(" DataPrevistaEntrega"))
                    {
                        select.Append("substring((select distinct ', ' + convert(varchar, Pedido.PED_PREVISAO_ENTREGA, 103) + ' ' + SUBSTRING(convert(varchar, Pedido.PED_PREVISAO_ENTREGA, 8), 0, 6) from T_CARGA_CTE CargaCTe inner join T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE CargaPedidoXMLNotaFiscalCTe on CargaPedidoXMLNotaFiscalCTe.CCT_CODIGO = CargaCTe.CCT_CODIGO inner join T_PEDIDO_XML_NOTA_FISCAL PedidoXMLNotaFiscal on PedidoXMLNotaFiscal.PNF_CODIGO = CargaPedidoXMLNotaFiscalCTe.PNF_CODIGO inner join T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = PedidoXMLNotaFiscal.CPE_CODIGO inner join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO where CargaCTe.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 200) DataPrevistaEntrega, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "NumeroDTNatura":
                    if (!select.Contains(" NumeroDTNatura,"))
                    {
                        select.Append(
                            @"(
                                select top 1 CONVERT(NVARCHAR(50), DocumentoTransporteNatura.IDT_NUMERO)
                                  from T_INTEGRACAO_NATURA_DOCUMENTO_TRANSPORTE DocumentoTransporteNatura
                                 inner join T_INTEGRACAO_NATURA_DOCUMENTO_TRANSPORTE_NOTA_FISCAL DocumentoTransporteNaturaNotaFiscal on DocumentoTransporteNaturaNotaFiscal.IDT_CODIGO = DocumentoTransporteNatura.IDT_CODIGO
                                 inner join T_XML_NOTA_FISCAL XMLNotaFiscal on XMLNotaFiscal.NF_CHAVE = DocumentoTransporteNaturaNotaFiscal.INF_CHAVE
                                 inner join T_CTE_XML_NOTAS_FISCAIS XMLNotaFiscalCTe on XMLNotaFiscalCTe.NFX_CODIGO = XMLNotaFiscal.NFX_CODIGO 
                                 inner join T_CARGA_CTE CargaCTe on CargaCte.CON_CODIGO = XMLNotaFiscalCTe.CON_CODIGO
                                 inner join T_CARGA_INTEGRACAO_NATURA CartaIntegracaoNatura on CartaIntegracaoNatura.IDT_CODIGO = DocumentoTransporteNatura.IDT_CODIGO and CargaCte.CAR_CODIGO_ORIGEM = CartaIntegracaoNatura.CAR_CODIGO
                                 where XMLNotaFiscalCTe.CON_CODIGO = CTe.CON_CODIGO
                            ) NumeroDTNatura, "
                        );

                        if (!groupBy.Contains("CTe.CON_CODIGO,"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "KmRodado":
                    if (!select.Contains(" KmRodado,"))
                    {
                        select.Append(
                            @"(
                                select SUM(DadosSumarizados.CDS_DISTANCIA)
                                  from T_CARGA_CTE CargaCTe
                                  join T_CARGA Carga on CargaCTe.CAR_CODIGO = Carga.CAR_CODIGO
                                  join T_CARGA_DADOS_SUMARIZADOS DadosSumarizados ON DadosSumarizados.CDS_CODIGO = Carga.CDS_CODIGO
                                 where CargaCTe.CON_CODIGO = CTe.CON_CODIGO
                            ) KmRodado, "
                        );

                        if (!groupBy.Contains("CTe.CON_CODIGO,"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "ValorKMContrato":
                    if (!select.Contains(" ValorKMContrato,"))
                    {
                        select.Append(
                            @"(
                                select SUM(Carga.CAR_CONTRATO_FRETE_FRANQUIA_VALOR_POR_KM)
                                  from T_CARGA_CTE CargaCTe
                                  join T_CARGA Carga on CargaCTe.CAR_CODIGO = Carga.CAR_CODIGO
                                 where CargaCTe.CON_CODIGO = CTe.CON_CODIGO
                            ) ValorKMContrato, "
                        );

                        if (!groupBy.Contains("CTe.CON_CODIGO,"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "ValorKMExcedenteContrato":
                    if (!select.Contains(" ValorKMExcedenteContrato,"))
                    {
                        select.Append(
                            @"(
                                select SUM(Carga.CAR_CONTRATO_FRETE_FRANQUIA_VALOR_KM_EXCEDENTE)
                                  from T_CARGA_CTE CargaCTe
                                  join T_CARGA Carga on CargaCTe.CAR_CODIGO = Carga.CAR_CODIGO
                                 where CargaCTe.CON_CODIGO = CTe.CON_CODIGO
                            ) ValorKMExcedenteContrato, "
                        );

                        if (!groupBy.Contains("CTe.CON_CODIGO,"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "ValorFreteFranquiaKM":
                    if (!select.Contains(" ValorFreteFranquiaKM,"))
                    {
                        //select.Append("ContratoSaldoMesCTe. ValorFreteFranquiaKM, ");

                        select.Append(
                            @"(
                                select SUM(_contratoSaldoMesCte.CSC_VALOR)
                                  from T_CARGA_CTE _cargaCTe
                                  join T_CONTRATO_SALDO_MES_CTE _contratoSaldoMesCte on _cargaCTe.CCT_CODIGO = _contratoSaldoMesCte.CCT_CODIGO
                                 where _cargaCTe.CON_CODIGO = CTe.CON_CODIGO
                            ) ValorFreteFranquiaKM, "
                        );

                        if (!groupBy.Contains("CTe.CON_CODIGO,"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "ValorFreteFranquiaKMExcedido":
                    if (!select.Contains(" ValorFreteFranquiaKMExcedido,"))
                    {
                        //select.Append("ContratoSaldoMesCTe.CSC_VALOR_EXCEDENTE ValorFreteFranquiaKMExcedido, ");

                        select.Append(
                            @"(
                                select SUM(_contratoSaldoMesCte.CSC_VALOR_EXCEDENTE)
                                  from T_CARGA_CTE _cargaCTe
                                  join T_CONTRATO_SALDO_MES_CTE _contratoSaldoMesCte on _cargaCTe.CCT_CODIGO = _contratoSaldoMesCte.CCT_CODIGO
                                 where _cargaCTe.CON_CODIGO = CTe.CON_CODIGO
                            ) ValorFreteFranquiaKMExcedido, "
                        );

                        if (!groupBy.Contains("CTe.CON_CODIGO,"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "KmConsumido":
                    if (!select.Contains(" KmConsumido,"))
                    {
                        //select.Append("ContratoSaldoMesCTe.CSC_DISTANCIA KmConsumido, ");

                        select.Append(
                            @"(
                                select SUM(_contratoSaldoMesCte.CSC_DISTANCIA)
                                  from T_CARGA_CTE _cargaCTe
                                  join T_CONTRATO_SALDO_MES_CTE _contratoSaldoMesCte on _cargaCTe.CCT_CODIGO = _contratoSaldoMesCte.CCT_CODIGO
                                 where _cargaCTe.CON_CODIGO = CTe.CON_CODIGO
                            ) KmConsumido, "
                        );

                        if (!groupBy.Contains("CTe.CON_CODIGO,"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "KmConsumidoExcedente":
                    if (!select.Contains(" KmConsumidoExcedente,"))
                    {
                        //select.Append("ContratoSaldoMesCTe.CSC_DISTANCIA_EXCEDENTE KmConsumidoExcedente, ");

                        select.Append(
                            @"(
                                select SUM(_contratoSaldoMesCte.CSC_DISTANCIA_EXCEDENTE)
                                  from T_CARGA_CTE _cargaCTe
                                  join T_CONTRATO_SALDO_MES_CTE _contratoSaldoMesCte on _cargaCTe.CCT_CODIGO = _contratoSaldoMesCte.CCT_CODIGO
                                 where _cargaCTe.CON_CODIGO = CTe.CON_CODIGO
                            ) KmConsumidoExcedente, "
                        );

                        if (!groupBy.Contains("CTe.CON_CODIGO,"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "NumeroOCADocumentoOriginario":
                    if (!select.Contains(" NumeroOCADocumentoOriginario,"))
                    {
                        select.Append("substring((select distinct ', ' + Convert(nvarchar(20), DocumentoOriginario.CDO_NUMERO_OPERACIONAL_CONHECIMENTO_AEREO) from T_CTE_DOCUMENTO_ORIGINARIO DocumentoOriginario where DocumentoOriginario.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 200) NumeroOCADocumentoOriginario, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "NumeroDI":
                    if (!select.Contains(" NumeroDI,"))
                    {
                        select.Append("substring((select distinct ', ' + PedidoImportacao.PEI_NUMERO_DI from T_CARGA_CTE CargaCTe inner join T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE CargaPedidoXMLNotaFiscalCTe on CargaPedidoXMLNotaFiscalCTe.CCT_CODIGO = CargaCTe.CCT_CODIGO inner join T_PEDIDO_XML_NOTA_FISCAL PedidoXMLNotaFiscal on PedidoXMLNotaFiscal.PNF_CODIGO = CargaPedidoXMLNotaFiscalCTe.PNF_CODIGO inner join T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = PedidoXMLNotaFiscal.CPE_CODIGO inner join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO inner join T_PEDIDO_IMPORTACAO PedidoImportacao on PedidoImportacao.PED_CODIGO = Pedido.PED_CODIGO where CargaCTe.CON_CODIGO = CTe.CON_CODIGO and LEN(PedidoImportacao.PEI_NUMERO_DI) > 0 for xml path('')), 3, 200) NumeroDI, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;
                case "NumeroDTA":
                    if (!select.Contains(" NumeroDTA,"))
                    {
                        select.Append("substring((select distinct ', ' + Pedido.PED_NUMERO_DTA from T_CARGA_CTE CargaCTe inner join T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE CargaPedidoXMLNotaFiscalCTe on CargaPedidoXMLNotaFiscalCTe.CCT_CODIGO = CargaCTe.CCT_CODIGO inner join T_PEDIDO_XML_NOTA_FISCAL PedidoXMLNotaFiscal on PedidoXMLNotaFiscal.PNF_CODIGO = CargaPedidoXMLNotaFiscalCTe.PNF_CODIGO inner join T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = PedidoXMLNotaFiscal.CPE_CODIGO inner join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO where CargaCTe.CON_CODIGO = CTe.CON_CODIGO and LEN(Pedido.PED_NUMERO_DTA) > 0 for xml path('')), 3, 200) NumeroDTA, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "ValorValePedagio":
                    if (!select.Contains(" ValorValePedagio, "))
                    {
                        select.Append(
                            @"(
                                select sum(_integracaoValePedagio.CVP_VALOR_VALE_PEDAGIO)
                                  from T_CARGA_INTEGRACAO_VALE_PEDAGIO _integracaoValePedagio
                                  join T_CARGA_CTE _cargaCTe on _cargaCTe.CAR_CODIGO = _integracaoValePedagio.CAR_CODIGO
                                 where _cargaCTe.CON_CODIGO = CTe.CON_CODIGO
                            ) ValorValePedagio,"
                        );

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append(" CTe.CON_CODIGO, ");
                    }
                    break;
                case "NumeroValePedagio":
                    if (!select.Contains(" NumeroValePedagio, "))
                    {
                        select.Append(
                            @"substring((
                                select distinct ', ' + _integracaoValePedagio.CVP_NUMERO_VALE_PEDAGIO
                                  from T_CARGA_INTEGRACAO_VALE_PEDAGIO _integracaoValePedagio
                                  join T_CARGA_CTE _cargaCTe on _cargaCTe.CAR_CODIGO = _integracaoValePedagio.CAR_CODIGO
                                 where _cargaCTe.CON_CODIGO = CTe.CON_CODIGO
                                   for xml path('')
                            ), 3, 1000) NumeroValePedagio,"
                        );

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append(" CTe.CON_CODIGO, ");
                    }
                    break;


                case "ValorValePedagioManual":
                    if (!select.Contains(" ValorValePedagioManual, "))
                    {
                        select.Append(@"(select sum(_integracaoValePedagioManual.CVP_VALOR)
                                         from T_CARGA_VALE_PEDAGIO _integracaoValePedagioManual
                                         join T_CARGA_CTE _cargaCTe on _cargaCTe.CAR_CODIGO = _integracaoValePedagioManual.CAR_CODIGO
                                         where _cargaCTe.CON_CODIGO = CTe.CON_CODIGO) ValorValePedagioManual,"
                    );

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append(" CTe.CON_CODIGO, ");
                    }

                    break;

                case "NumeroValePedagioManual":
                    if (!select.Contains(" NumeroValePedagioManual, "))
                    {
                        select.Append(@"substring((select distinct ', ' + _integracaoValePedagioManual.CVP_NUMERO_COMPROVANTE
                                                   from T_CARGA_VALE_PEDAGIO _integracaoValePedagioManual
                                                   join T_CARGA_CTE _cargaCTe on _cargaCTe.CAR_CODIGO = _integracaoValePedagioManual.CAR_CODIGO
                                                   where _cargaCTe.CON_CODIGO = CTe.CON_CODIGO
                                                 for xml path('')), 3, 1000) NumeroValePedagioManual,"
                    );

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append(" CTe.CON_CODIGO, ");
                    }
                    break;

                case "TabelaFrete":
                    if (!select.Contains(" TabelaFrete, "))
                    {
                        select.Append(
                            @"SUBSTRING((
                                SELECT DISTINCT ', ' + TabelaFrete.TBF_DESCRICAO
                                        from T_TABELA_FRETE TabelaFrete
                                        inner join T_CARGA Carga on Carga.TBF_CODIGO = TabelaFrete.TBF_CODIGO
                                        inner join T_CARGA_CTE CargaCTe on CargaCTe.CAR_CODIGO = Carga.CAR_CODIGO 
                                 WHERE CargaCTe.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 1000) TabelaFrete, "
                        );

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append(" CTe.CON_CODIGO, ");
                    }
                    break;

                case "DataVigenciaTabelaFrete":
                    if (!select.Contains(" DataVigenciaTabelaFrete, "))
                    {
                        select.Append(
                            @"(SELECT TOP 1 
                                            convert(varchar, _tabelaFreteClienteVigencia.TFV_DATA_INICIAL, 103) + ' ' +
                                            CASE
                                                WHEN _tabelaFreteClienteVigencia.TFV_DATA_FINAL IS NOT NULL THEN 'até ' + convert(varchar, _tabelaFreteClienteVigencia.TFV_DATA_FINAL, 103)
                                                ELSE ''
                                            END
                                            FROM T_TABELA_FRETE_VIGENCIA _tabelaFreteClienteVigencia
                                            JOIN T_TABELA_FRETE_CLIENTE _tabelaFreteCliente on _tabelaFreteClienteVigencia.TFV_CODIGO = _tabelaFreteCliente.TFV_CODIGO
			                                JOIN T_CARGA_TABELA_FRETE_CLIENTE _cargaTabelaFreteCliente on _cargaTabelaFreteCliente.TFC_CODIGO = _tabelaFreteCliente.TFC_CODIGO
                                            inner join T_CARGA_CTE _cargaCTe on _cargaCTe.CAR_CODIGO = _cargaTabelaFreteCliente.CAR_CODIGO 
                                            WHERE _cargaCTe.CON_CODIGO = CTe.CON_CODIGO) DataVigenciaTabelaFrete, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append(" CTe.CON_CODIGO, ");
                    }
                    break;

                case "TabelaFreteCliente":
                    if (!select.Contains(" TabelaFreteCliente, "))
                    {
                        select.Append(
                                @"SUBSTRING((
                                SELECT DISTINCT ', ' + 
                                            (CASE WHEN TabelaFrete.TBF_TIPO_CALCULO = 0 THEN (SUBSTRING((SELECT DISTINCT ', ' + 
		                                            CAST((TFC_CODIGO_INTEGRACAO + ' - ' + TFC_DESCRICAO_ORIGEM + ' até ' + TFC_DESCRICAO_DESTINO) AS NVARCHAR(500))
			                                        FROM T_TABELA_FRETE_CLIENTE TabelaFreteCliente
			                                        JOIN T_CARGA_TABELA_FRETE_CLIENTE CargaTabelaFreteCliente on CargaTabelaFreteCliente.TFC_CODIGO = TabelaFreteCliente.TFC_CODIGO
			                                        WHERE CargaTabelaFreteCliente.CAR_CODIGO = Carga.CAR_CODIGO FOR XML PATH('')), 3, 3000)) --PorCarga
                                              --WHEN TabelaFrete.TBF_TIPO_CALCULO = 1 THEN () --PorPedido
                                              ELSE '' END)
                                        from T_TABELA_FRETE TabelaFrete
                                        inner join T_CARGA Carga on Carga.TBF_CODIGO = TabelaFrete.TBF_CODIGO
                                        inner join T_CARGA_CTE CargaCTe on CargaCTe.CAR_CODIGO = Carga.CAR_CODIGO 
                                 WHERE CargaCTe.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 5000) TabelaFreteCliente, "
                            );

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append(" CTe.CON_CODIGO, ");
                    }
                    break;

                case "NumeroEscrituracao":
                    if (!select.Contains(" NumeroEscrituracao, "))
                    {
                        select.Append("CAST(LoteEscrituracao.LES_NUMERO AS NVARCHAR(20)) NumeroEscrituracao, ");
                        groupBy.Append("LoteEscrituracao.LES_NUMERO, ");

                        SetarJoinsLoteEscrituracao(joins);
                    }
                    break;

                case "NumeroPagamento":
                    if (!select.Contains(" NumeroPagamento, "))
                    {
                        select.Append("CAST(Pagamento.PAG_NUMERO AS NVARCHAR(20)) NumeroPagamento, ");
                        groupBy.Append("Pagamento.PAG_NUMERO, ");

                        SetarJoinsPagamento(joins);
                    }
                    break;

                case "NumeroContabilizacao":
                    if (!select.Contains(" NumeroContabilizacao, "))
                    {
                        select.Append(
                            @"SUBSTRING((
                                SELECT DISTINCT ', ' + CAST(LoteContabilizacao.LCO_NUMERO AS NVARCHAR(20))
                                  FROM T_DOCUMENTO_EXPORTACAO_CONTABIL DocumentoExportacao 
                                 INNER JOIN T_LOTE_CONTABILIZACAO LoteContabilizacao ON LoteContabilizacao.LCO_CODIGO = DocumentoExportacao.LCO_CODIGO 
                                 WHERE DocumentoExportacao.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 1000) NumeroContabilizacao, "
                        );

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append(" CTe.CON_CODIGO, ");
                    }
                    break;

                case "NumeroBooking":
                    if (!select.Contains(" NumeroBooking, "))
                    {
                        select.Append("CTe.CON_NUMERO_BOOKING NumeroBooking, ");
                        groupBy.Append("CTe.CON_NUMERO_BOOKING, ");
                    }
                    break;

                case "NumeroOS":
                    if (!select.Contains(" NumeroOS, "))
                    {
                        select.Append("CTe.CON_NUMERO_OS NumeroOS, ");
                        groupBy.Append("CTe.CON_NUMERO_OS, ");
                    }
                    break;

                case "NumeroControle":
                    if (!select.Contains(" NumeroControle, "))
                    {
                        select.Append("CTe.CON_NUMERO_CONTROLE NumeroControle, ");
                        groupBy.Append("CTe.CON_NUMERO_CONTROLE, ");
                    }
                    break;

                case "TipoProposta":
                    if (!select.Contains(" TipoProposta, "))
                    {
                        select.Append(
                            @"SUBSTRING((
                                SELECT DISTINCT ', ' + case 
                                                            WHEN cargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL = 1 THEN 'Carga Fechada'
                                                            WHEN cargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL = 2 THEN 'Carga Fracionada'
                                                            WHEN cargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL = 3 THEN 'Feeder'
                                                            WHEN cargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL = 4 THEN 'VAS'
                                                            WHEN cargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL = 5 THEN 'Embarque Certo - Feeder'
                                                            WHEN cargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL = 6 THEN 'Embarque Certo - Cabotagem'
                                                            WHEN cargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL = 7 THEN 'No Show - Cabotagem'
                                                            WHEN cargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL = 8 THEN 'Faturamento - Contabilidade'
                                                            WHEN cargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL = 9 THEN 'Demurrage - Cabotagem'
                                                            WHEN cargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL = 10 THEN 'Detention - Cabotagem'
                                                       else '' end 
                                        from T_CARGA_PEDIDO cargaPedido 
                                        inner join T_CARGA_CTE CargaCTe on CargaCTe.CAR_CODIGO = cargaPedido.CAR_CODIGO 
                                 WHERE CargaCTe.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 1000) TipoProposta, "
                        );

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append(" CTe.CON_CODIGO, ");
                    }
                    break;

                case "NumeroProposta":
                    if (!select.Contains(" NumeroProposta, "))
                    {
                        select.Append(
                            @"SUBSTRING((
                                SELECT DISTINCT ', ' + Pedido.PED_CODIGO_PROPOSTA
                                        from T_PEDIDO Pedido 
                                        inner join T_CARGA_PEDIDO cargaPedido on cargaPedido.PED_CODIGO = Pedido.PED_CODIGO 
                                        inner join T_CARGA_CTE CargaCTe on CargaCTe.CAR_CODIGO = cargaPedido.CAR_CODIGO 
                                 WHERE CargaCTe.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 1000) NumeroProposta, "
                        );

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append(" CTe.CON_CODIGO, ");
                    }
                    break;

                case "TipoCarga":
                    if (!select.Contains(" TipoCarga, "))
                    {
                        select.Append(
                            @"SUBSTRING((
                                SELECT DISTINCT ', ' + TipoCarga.TCG_DESCRICAO
                                        from T_TIPO_DE_CARGA TipoCarga 
                                        inner join T_CARGA Carga on Carga.TCG_CODIGO = TipoCarga.TCG_CODIGO 
                                        inner join T_CARGA_CTE CargaCTe on CargaCTe.CAR_CODIGO = Carga.CAR_CODIGO 
                                 WHERE CargaCTe.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 1000) TipoCarga, "
                        );

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append(" CTe.CON_CODIGO, ");
                    }
                    break;

                case "QuantidadeNF":
                    if (!select.Contains(" QuantidadeNF, "))
                    {
                        select.Append(
                            @"  (SELECT COUNT (1) from T_CTE_DOCS cteDocs 
                                 WHERE cteDocs.CON_CODIGO = CTe.CON_CODIGO) QuantidadeNF, "
                        );

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append(" CTe.CON_CODIGO, ");
                    }
                    break;

                case "Viagem":
                    if (!select.Contains(" Viagem, "))
                    {
                        select.Append("Viagem.PVN_DESCRICAO Viagem, ");
                        groupBy.Append("Viagem.PVN_DESCRICAO, ");

                        SetarJoinsViagem(joins);
                    }
                    break;

                case "NumeroLacre":
                    if (!select.Contains(" NumeroLacre, "))
                    {
                        select.Append("substring(( ");
                        select.Append("    SELECT DISTINCT ");
                        select.Append("      CASE WHEN (cteContainer.CER_LACRE1 IS NOT NULL and RTRIM(cteContainer.CER_LACRE1) <> '') THEN cteContainer.CER_LACRE1 ELSE '' END ");
                        select.Append("    + CASE WHEN (cteContainer.CER_LACRE2 IS NOT NULL and RTRIM(cteContainer.CER_LACRE2) <> '') THEN ', ' + cteContainer.CER_LACRE2 ELSE '' END ");
                        select.Append("    + CASE WHEN (cteContainer.CER_LACRE3 IS NOT NULL and RTRIM(cteContainer.CER_LACRE3) <> '') THEN ', ' + cteContainer.CER_LACRE3 ELSE '' END ");
                        select.Append("      FROM T_CTE_CONTAINER cteContainer ");
                        select.Append("     where cteContainer.CON_CODIGO = CTe.CON_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) NumeroLacre, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append(" CTe.CON_CODIGO, ");
                    }
                    break;

                case "Tara":
                    if (!select.Contains(" Tara, "))
                    {
                        select.Append(
                            @"SUBSTRING((
                                SELECT DISTINCT ', ' + FORMAT(container.CTR_TARA, 'N2', 'pt-BR')
                                        from T_CONTAINER container 
                                        inner join T_CTE_CONTAINER cteContainer on cteContainer.CTR_CODIGO = container.CTR_CODIGO 
                                 WHERE cteContainer.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 1000) Tara, "
                        );

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append(" CTe.CON_CODIGO, ");
                    }
                    break;

                case "Container":
                    if (!select.Contains(" Container, "))
                    {
                        select.Append(
                            @"SUBSTRING((
                                    SELECT ', ' + DistinctContainers.CTR_DESCRICAO 
                                    FROM (
                                        SELECT DISTINCT container.CTR_DESCRICAO, container.CTR_DATA_ULTIMA_ATUALIZACAO
                                        FROM T_CONTAINER container 
                                        INNER JOIN T_CTE_CONTAINER cteContainer 
                                            ON cteContainer.CTR_CODIGO = container.CTR_CODIGO 
                                        WHERE cteContainer.CON_CODIGO = CTe.CON_CODIGO
                                    ) AS DistinctContainers
                                    ORDER BY DistinctContainers.CTR_DATA_ULTIMA_ATUALIZACAO  desc
                                    FOR XML PATH('')
                                ), 3, 1000) AS Container, "
                        );

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append(" CTe.CON_CODIGO, ");
                    }
                    break;

                case "TipoContainer":
                    if (!select.Contains(" TipoContainer, "))
                    {
                        select.Append(
                            @"SUBSTRING((
                                SELECT DISTINCT ', ' + containerTipo.CTI_DESCRICAO
                                        from T_CONTAINER_TIPO containerTipo
                                        inner join T_CONTAINER container on container.CTI_CODIGO = containerTipo.CTI_CODIGO 
                                        inner join T_CTE_CONTAINER cteContainer on cteContainer.CTR_CODIGO = container.CTR_CODIGO 
                                 WHERE cteContainer.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 1000) TipoContainer, "
                        );

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append(" CTe.CON_CODIGO, ");
                    }
                    break;

                case "NumeroFatura":
                    if (!select.Contains(" NumeroFatura, "))
                    {
                        select.Append("substring(( ");
                        select.Append("    SELECT DISTINCT ', ' + CAST((CASE WHEN F.FAT_NUMERO_FATURA_INTEGRACAO IS NULL OR F.FAT_NUMERO_FATURA_INTEGRACAO = 0 THEN F.FAT_NUMERO ELSE F.FAT_NUMERO_FATURA_INTEGRACAO END) AS NVARCHAR(20)) ");
                        select.Append("      FROM T_DOCUMENTO_FATURAMENTO DF ");
                        select.Append("      inner join T_FATURA_DOCUMENTO FD on FD.DFA_CODIGO = DF.DFA_CODIGO ");
                        select.Append("      inner join T_FATURA F on F.FAT_CODIGO = FD.FAT_CODIGO ");
                        select.Append("     where DF.CON_CODIGO = CTe.CON_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) NumeroFatura, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append(" CTe.CON_CODIGO, ");
                    }
                    break;

                case "DataFatura":
                    if (!select.Contains(" DataFatura, "))
                    {
                        select.Append("substring(( ");
                        select.Append("    SELECT DISTINCT ', ' + CONVERT(NVARCHAR(10), F.FAT_DATA_FATURA, 103) ");
                        select.Append("      FROM T_DOCUMENTO_FATURAMENTO DF ");
                        select.Append("      inner join T_FATURA_DOCUMENTO FD on FD.DFA_CODIGO = DF.DFA_CODIGO ");
                        select.Append("      inner join T_FATURA F on F.FAT_CODIGO = FD.FAT_CODIGO ");
                        select.Append("     where DF.CON_CODIGO = CTe.CON_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) DataFatura, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append(" CTe.CON_CODIGO, ");
                    }
                    break;

                case "NumeroBoleto":
                    if (!select.Contains(" NumeroBoleto, "))
                    {
                        select.Append("substring(( ");
                        select.Append("    SELECT DISTINCT ', ' + T.TIT_NOSSO_NUMERO ");
                        select.Append("      FROM T_DOCUMENTO_FATURAMENTO DF ");
                        select.Append("      inner join T_FATURA_DOCUMENTO FD on FD.DFA_CODIGO = DF.DFA_CODIGO ");
                        select.Append("      inner join T_FATURA F on F.FAT_CODIGO = FD.FAT_CODIGO ");
                        select.Append("      inner join T_FATURA_PARCELA FP on FP.FAT_CODIGO = F.FAT_CODIGO ");
                        select.Append("      inner join T_TITULO T on T.FAP_CODIGO = FP.FAP_CODIGO ");
                        select.Append("     where DF.CON_CODIGO = CTe.CON_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) NumeroBoleto, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append(" CTe.CON_CODIGO, ");
                    }
                    break;

                case "DataBoleto":
                    if (!select.Contains(" DataBoleto, "))
                    {
                        select.Append("substring(( ");
                        select.Append("    SELECT DISTINCT ', ' + CONVERT(NVARCHAR(10), T.TIT_DATA_EMISSAO, 103) ");
                        select.Append("      FROM T_DOCUMENTO_FATURAMENTO DF ");
                        select.Append("      inner join T_FATURA_DOCUMENTO FD on FD.DFA_CODIGO = DF.DFA_CODIGO ");
                        select.Append("      inner join T_FATURA F on F.FAT_CODIGO = FD.FAT_CODIGO ");
                        select.Append("      inner join T_FATURA_PARCELA FP on FP.FAT_CODIGO = F.FAT_CODIGO ");
                        select.Append("      inner join T_TITULO T on T.FAP_CODIGO = FP.FAP_CODIGO ");
                        select.Append("     where DF.CON_CODIGO = CTe.CON_CODIGO and T.TIT_NOSSO_NUMERO is not null and T.TIT_NOSSO_NUMERO <> '' ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) DataBoleto, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append(" CTe.CON_CODIGO, ");
                    }
                    break;

                case "PortoOrigem":
                    if (!select.Contains(" PortoOrigem, "))
                    {
                        select.Append("PortoOrigem.POT_DESCRICAO PortoOrigem, ");
                        groupBy.Append("PortoOrigem.POT_DESCRICAO, ");

                        SetarJoinsPortoOrigem(joins);
                    }
                    break;

                case "PortoDestino":
                    if (!select.Contains(" PortoDestino, "))
                    {
                        select.Append("PortoDestino.POT_DESCRICAO PortoDestino, ");
                        groupBy.Append("PortoDestino.POT_DESCRICAO, ");

                        SetarJoinsPortoDestino(joins);
                    }
                    break;

                case "PortoTransbordo":
                    if (!select.Contains(" PortoTransbordo, "))
                    {
                        select.Append(@"concat(PortoPassagemUm.POT_DESCRICAO
                                           , CASE WHEN PortoPassagemUm.POT_CODIGO IS NOT NULL THEN ', ' + PortoPassagemDois.POT_DESCRICAO ELSE '' END
                                           , CASE WHEN PortoPassagemDois.POT_CODIGO IS NOT NULL THEN ', ' + PortoPassagemTres.POT_DESCRICAO ELSE '' END
                                           , CASE WHEN PortoPassagemTres.POT_CODIGO IS NOT NULL THEN ', ' + PortoPassagemQuatro.POT_DESCRICAO ELSE '' END
                                           , CASE WHEN PortoPassagemQuatro.POT_CODIGO IS NOT NULL THEN ', ' + PortoPassagemCinco.POT_DESCRICAO ELSE '' END
                        ) PortoTransbordo, ");

                        groupBy.Append("PortoPassagemUm.POT_CODIGO, ");
                        groupBy.Append("PortoPassagemDois.POT_CODIGO, ");
                        groupBy.Append("PortoPassagemTres.POT_CODIGO, ");
                        groupBy.Append("PortoPassagemQuatro.POT_CODIGO, ");
                        groupBy.Append("PortoPassagemCinco.POT_CODIGO, ");

                        groupBy.Append("PortoPassagemUm.POT_DESCRICAO, ");
                        groupBy.Append("PortoPassagemDois.POT_DESCRICAO, ");
                        groupBy.Append("PortoPassagemTres.POT_DESCRICAO, ");
                        groupBy.Append("PortoPassagemQuatro.POT_DESCRICAO, ");
                        groupBy.Append("PortoPassagemCinco.POT_DESCRICAO, ");

                        SetarJoinsPortoPassagemUm(joins);
                        SetarJoinsPortoPassagemDois(joins);
                        SetarJoinsPortoPassagemTres(joins);
                        SetarJoinsPortoPassagemQuatro(joins);
                        SetarJoinsPortoPassagemCinco(joins);
                    }
                    break;

                case "NavioTransbordo":
                    if (!select.Contains(" NavioTransbordo, "))
                    {
                        select.Append(
                            @"SUBSTRING((
                                SELECT DISTINCT ', ' + navio.PVN_DESCRICAO
                                        from T_PEDIDO_VIAGEM_NAVIO navio
                                        inner join T_PEDIDO_TRANSBORDO pedidoTransbordo on pedidoTransbordo.PVN_CODIGO = navio.PVN_CODIGO 
                                        inner join T_CARGA_PEDIDO CargaPedido on CargaPedido.PED_CODIGO = pedidoTransbordo.PED_CODIGO 
                                        inner join T_CARGA_CTE CargaCTe on CargaCTe.CAR_CODIGO = CargaPedido.CAR_CODIGO 
                                 WHERE CargaCTe.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 1000) NavioTransbordo, "
                        );

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append(" CTe.CON_CODIGO, ");
                    }
                    break;

                case "PossuiCartaCorrecao":
                    if (!select.Contains(" PossuiCartaCorrecao, "))
                    {
                        select.Append("CASE WHEN CTe.CON_POSSUI_CARTA_CORRECAO = 1 THEN 'Sim' ELSE 'Não' END PossuiCartaCorrecao, ");
                        groupBy.Append("CTe.CON_POSSUI_CARTA_CORRECAO, ");
                    }
                    break;

                case "FoiAnulado":
                    if (!select.Contains(" FoiAnulado, "))
                    {
                        select.Append("CASE WHEN CTe.CON_POSSUI_ANULACAO_SUBSTITUICAO = 1 THEN 'Sim' ELSE 'Não' END FoiAnulado, ");
                        groupBy.Append("CTe.CON_POSSUI_ANULACAO_SUBSTITUICAO, ");
                    }
                    break;

                case "PossuiCTeComplementar":
                    if (!select.Contains(" PossuiCTeComplementar, "))
                    {
                        select.Append("CASE WHEN CTe.CON_POSSUI_CTE_COMPLEMENTAR = 1 THEN 'Sim' ELSE 'Não' END PossuiCTeComplementar, ");
                        groupBy.Append("CTe.CON_POSSUI_CTE_COMPLEMENTAR, ");
                    }
                    break;

                case "FoiSubstituido":
                    if (!select.Contains(" FoiSubstituido, "))
                    {
                        select.Append(@"CASE WHEN (select count(1) from t_cte _cte where _cte.CON_CHAVE_CTE_SUB_COMP = CTe.CON_CHAVECTE) > 0 THEN 'Sim' 
                                        ELSE 'Não' END FoiSubstituido, ");

                        if (!groupBy.Contains("CTe.CON_CHAVECTE"))
                            groupBy.Append(" CTe.CON_CHAVECTE, ");
                    }
                    break;

                case "ETA":
                    if (!select.Contains(" ETA, "))
                    {
                        select.Append("CONVERT(NVARCHAR(10), ViagemScheduleDestino.PVS_DATA_PREVISAO_CHEGADA_NAVIO, 103) + ' ' + CONVERT(NVARCHAR(5), ViagemScheduleDestino.PVS_DATA_PREVISAO_CHEGADA_NAVIO, 108) ETA, ");
                        groupBy.Append("ViagemScheduleDestino.PVS_DATA_PREVISAO_CHEGADA_NAVIO, ");

                        SetarJoinsViagemScheduleDestino(joins);
                    }
                    break;

                case "ETS":
                    if (!select.Contains(" ETS, "))
                    {
                        select.Append("CONVERT(NVARCHAR(10), ViagemScheduleDestino.PVS_DATA_PREVISAO_SAIDA_NAVIO, 103) + ' ' + CONVERT(NVARCHAR(5), ViagemScheduleDestino.PVS_DATA_PREVISAO_SAIDA_NAVIO, 108) ETS, ");
                        groupBy.Append("ViagemScheduleDestino.PVS_DATA_PREVISAO_SAIDA_NAVIO, ");

                        SetarJoinsViagemScheduleDestino(joins);
                    }
                    break;

                case "MotivoCancelamento":
                    if (!select.Contains(" MotivoCancelamento, "))
                    {
                        select.Append("CTe.CON_OBS_CANCELAMENTO MotivoCancelamento, ");
                        groupBy.Append("CTe.CON_OBS_CANCELAMENTO, ");
                    }
                    break;

                case "NumeroLoteCancelamento":
                    if (!select.Contains(" NumeroLoteCancelamento, "))
                    {
                        select.Append("CAST(LoteEscrituracaoCancelamento.LEC_NUMERO AS NVARCHAR(20)) NumeroLoteCancelamento, ");
                        groupBy.Append("LoteEscrituracaoCancelamento.LEC_NUMERO, ");

                        SetarJoinsLoteEscrituracaoCancelamento(joins);
                    }
                    break;

                case "AliquotaICMSInterna":
                    if (!somenteContarNumeroRegistros && !select.Contains("AliquotaICMSInterna"))
                        select.Append("SUM(CTe.CON_ALIQUOTA_ICMS_INTERNA) AliquotaICMSInterna, ");
                    break;

                case "PercentualICMSPartilha":
                    if (!somenteContarNumeroRegistros && !select.Contains("PercentualICMSPartilha"))
                        select.Append("AVG(CTe.CON_PERCENTUAL_ICMS_PARTILHA) PercentualICMSPartilha, ");
                    break;

                case "ValorICMSUFOrigem":
                    if (!somenteContarNumeroRegistros && !select.Contains("ValorICMSUFOrigem"))
                        select.Append("SUM(CTe.CON_VALOR_ICMS_UF_ORIGEM) ValorICMSUFOrigem, ");
                    break;

                case "ValorICMSUFDestino":
                    if (!somenteContarNumeroRegistros && !select.Contains("ValorICMSUFDestino"))
                        select.Append("SUM(CTe.CON_VALOR_ICMS_UF_DESTINO) ValorICMSUFDestino, ");
                    break;

                case "ValorICMSFCPFim":
                    if (!somenteContarNumeroRegistros && !select.Contains("ValorICMSFCPFim"))
                        select.Append("SUM(CTe.CON_VALOR_ICMS_FCP_DESTINO) ValorICMSFCPFim, ");
                    break;

                case "CaracteristicaTransporteCTe":
                    if (!select.Contains(" CaracteristicaTransporteCTe, "))
                    {
                        select.Append("CTe.CON_CARAC_TRANSP CaracteristicaTransporteCTe, ");
                        groupBy.Append("CTe.CON_CARAC_TRANSP, ");
                    }
                    break;

                case "ProdutoPredominante":
                    if (!select.Contains(" ProdutoPredominante, "))
                    {
                        select.Append("CTe.CON_PRODUTO_PRED ProdutoPredominante, ");
                        groupBy.Append("CTe.CON_PRODUTO_PRED, ");
                    }
                    break;

                case "CentroResultado":
                    if (!select.Contains(" CentroResultado, "))
                    {
                        select.Append("CentroResultado.CRE_PLANO_CONTABILIDADE + ' - ' + CentroResultado.CRE_DESCRICAO CentroResultado, ");

                        if (!groupBy.Contains("CentroResultado.CRE_DESCRICAO"))
                            groupBy.Append(" CentroResultado.CRE_DESCRICAO, ");

                        if (!groupBy.Contains("CentroResultado.CRE_PLANO_CONTABILIDADE"))
                            groupBy.Append(" CentroResultado.CRE_PLANO_CONTABILIDADE, ");

                        SetarJoinsCentroResultado(joins);
                    }
                    break;

                case "TipoServicoMultimodal":
                    if (!select.Contains(" TipoServicoMultimodal, "))
                    {
                        select.Append(
                                @"SUBSTRING((
                                SELECT DISTINCT ', ' + case 
                                                            WHEN cargaPedido.TBF_TIPO_SERVICO_MULTIMODAL = 1 THEN 'Normal'
                                                            WHEN cargaPedido.TBF_TIPO_SERVICO_MULTIMODAL = 2 THEN 'Subcontratação'
                                                            WHEN cargaPedido.TBF_TIPO_SERVICO_MULTIMODAL = 3 THEN 'Redespacho Intermediário'
                                                            WHEN cargaPedido.TBF_TIPO_SERVICO_MULTIMODAL = 4 THEN 'Vinculado Multimodal Terceiro'
                                                            WHEN cargaPedido.TBF_TIPO_SERVICO_MULTIMODAL = 5 THEN 'Vinculado Multimodal Próprio'
                                                            WHEN cargaPedido.TBF_TIPO_SERVICO_MULTIMODAL = 6 THEN 'Redespacho'
                                                       else '' end 
                                        from T_CARGA_PEDIDO cargaPedido 
                                        inner join T_CARGA_CTE CargaCTe on CargaCTe.CAR_CODIGO = cargaPedido.CAR_CODIGO 
                                 WHERE CargaCTe.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 1000) TipoServicoMultimodal, "
                        );

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append(" CTe.CON_CODIGO, ");
                    }
                    break;

                case "NumeroManifesto":
                    if (!select.Contains(" NumeroManifesto, "))
                    {
                        select.Append("CTe.CON_NUMERO_MANIFESTO NumeroManifesto, ");
                        groupBy.Append("CTe.CON_NUMERO_MANIFESTO, ");
                    }
                    break;

                case "NumeroManifestoFeeder":
                    if (!select.Contains(" NumeroManifestoFeeder, "))
                    {
                        select.Append(
                            @"SUBSTRING((
                                SELECT DISTINCT ', ' + pedido.PED_NUMERO_MANIFESTO_FEEDER 
                                        FROM T_PEDIDO pedido
                                        inner join T_CARGA_PEDIDO cargaPedido on cargaPedido.PED_CODIGO = pedido.PED_CODIGO 
                                        inner join T_CARGA_CTE CargaCTe on CargaCTe.CAR_CODIGO = cargaPedido.CAR_CODIGO 
                                 WHERE CargaCTe.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 1000) NumeroManifestoFeeder, "
                        );

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append(" CTe.CON_CODIGO, ");
                    }
                    break;

                case "NumeroCEMercante":
                    if (!select.Contains(" NumeroCEMercante, "))
                    {
                        select.Append("CTe.CON_NUMERO_CE_MERCANTE NumeroCEMercante, ");
                        groupBy.Append("CTe.CON_NUMERO_CE_MERCANTE, ");
                    }
                    break;

                case "NumeroCEANTAQ":
                    if (!select.Contains(" NumeroCEANTAQ, "))
                    {
                        select.Append(
                            @"SUBSTRING((
                                SELECT DISTINCT ', ' + pedido.PED_NUMERO_CE_FEEDER 
                                        FROM T_PEDIDO pedido
                                        inner join T_CARGA_PEDIDO cargaPedido on cargaPedido.PED_CODIGO = pedido.PED_CODIGO 
                                        inner join T_CARGA_CTE CargaCTe on CargaCTe.CAR_CODIGO = cargaPedido.CAR_CODIGO 
                                 WHERE CargaCTe.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 1000) NumeroCEANTAQ, "
                        );

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append(" CTe.CON_CODIGO, ");
                    }
                    break;

                case "DescricaoAfretamento":
                    if (!select.Contains(" Afretamento, "))
                    {
                        select.Append(@"ISNULL((SELECT TOP(1) Pedido.PED_EMBARQUE_AFRETAMENTO_FEEDER FROM T_PEDIDO Pedido 
                                            join T_CARGA_PEDIDO CargaPedido on CargaPedido.PED_CODIGO = Pedido.PED_CODIGO 
                                            join T_CARGA_CTE CargaCTe on CargaCTe.CAR_CODIGO = CargaPedido.CAR_CODIGO 
                                            where CargaCTe.CON_CODIGO = CTe.CON_CODIGO), 0) Afretamento, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append(" CTe.CON_CODIGO, ");
                    }
                    break;

                case "NumeroProtocoloANTAQ":
                    if (!select.Contains(" NumeroProtocoloANTAQ, "))
                    {
                        select.Append(
                            @"SUBSTRING((
                                SELECT DISTINCT ', ' + pedido.PED_PROTOCOLO_ANTAQ_FEEDER 
                                        FROM T_PEDIDO pedido
                                        inner join T_CARGA_PEDIDO cargaPedido on cargaPedido.PED_CODIGO = pedido.PED_CODIGO 
                                        inner join T_CARGA_CTE CargaCTe on CargaCTe.CAR_CODIGO = cargaPedido.CAR_CODIGO 
                                 WHERE CargaCTe.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 1000) NumeroProtocoloANTAQ, "
                        );

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append(" CTe.CON_CODIGO, ");
                    }
                    break;

                case "ProcImportacao":
                    if (!select.Contains(" ProcImportacao, "))
                    {
                        select.Append(@"ISNULL((SELECT TOP(1) Pedido.PED_ADICIONAL1 from T_PEDIDO Pedido 
                                        inner join T_CARGA_PEDIDO CargaPedido on CargaPedido.PED_CODIGO = pedido.PED_CODIGO 
                                        inner join T_CARGA_CTE CargaCTe on CargaCTe.CAR_CODIGO_ORIGEM = CargaPedido.CAR_CODIGO_ORIGEM 
                                        WHERE CargaCTe.CON_CODIGO = CTe.CON_CODIGO), '') ProcImportacao, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append(" CTe.CON_CODIGO, ");
                    }
                    break;

                case "RotaFrete":
                    if (!select.Contains(" RotaFrete, "))
                    {
                        select.Append(@"(SELECT TOP(1) RotaFrete.ROF_DESCRICAO from T_ROTA_FRETE RotaFrete 
                                        inner join T_CARGA Carga on Carga.ROF_CODIGO = RotaFrete.ROF_CODIGO 
                                        inner join T_CARGA_CTE CargaCTe on CargaCTe.CAR_CODIGO = Carga.CAR_CODIGO 
                                        WHERE CargaCTe.CON_CODIGO = CTe.CON_CODIGO and (Carga.CAR_CARGA_TRANSBORDO is null or Carga.CAR_CARGA_TRANSBORDO = 0)) RotaFrete, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append(" CTe.CON_CODIGO, ");
                    }
                    break;

                case "ValorSemTributo":
                    if (!somenteContarNumeroRegistros && !select.Contains(" ValorSemTributo, "))
                        select.Append("SUM(CTe.CON_VALOR_PREST_SERVICO - CTe.CON_VAL_ICMS - CTe.CON_VALOR_ISS - CTe.CON_VALOR_ICMS_UF_DESTINO - CTe.CON_VALOR_ICMS_FCP_DESTINO) ValorSemTributo, ");
                    break;

                case "NumeroCTeAnulacao":
                    if (!select.Contains(" NumeroCTeAnulacao, "))
                        select.Append(
                            @"SUBSTRING((SELECT DISTINCT ', ' + CAST(CTeGerado.CON_NUM AS NVARCHAR(20))
                                FROM T_CTE_RELACAO_DOCUMENTO CTeRelacao
                                JOIN T_CTE CTeGerado on CTeGerado.CON_CODIGO = CTeRelacao.CON_CODIGO_GERADO
                                WHERE CTeRelacao.CRD_TIPO_CTE_GERADO = 1 AND CTeRelacao.CON_CODIGO_ORIGINAL  = CTe.CON_CODIGO FOR XML PATH('')), 3, 1000) NumeroCTeAnulacao, ");

                    if (!groupBy.Contains("CTe.CON_CODIGO"))
                        groupBy.Append("CTe.CON_CODIGO, ");
                    break;

                case "NumeroControleCTeAnulacao":
                    if (!select.Contains(" NumeroControleCTeAnulacao, "))
                        select.Append(
                            @"SUBSTRING((SELECT DISTINCT ', ' + CTeGerado.CON_NUMERO_CONTROLE
                                FROM T_CTE_RELACAO_DOCUMENTO CTeRelacao
                                JOIN T_CTE CTeGerado on CTeGerado.CON_CODIGO = CTeRelacao.CON_CODIGO_GERADO
                                WHERE CTeRelacao.CRD_TIPO_CTE_GERADO = 1 AND CTeRelacao.CON_CODIGO_ORIGINAL = CTe.CON_CODIGO FOR XML PATH('')), 3, 1000) NumeroControleCTeAnulacao, ");

                    if (!groupBy.Contains("CTe.CON_CODIGO"))
                        groupBy.Append("CTe.CON_CODIGO, ");
                    break;

                case "NumeroCTeComplementar":
                    if (!select.Contains(" NumeroCTeComplementar, "))
                        select.Append(
                            @"SUBSTRING((SELECT DISTINCT ', ' + CAST(CTeGerado.CON_NUM AS NVARCHAR(20))
                                FROM T_CTE_RELACAO_DOCUMENTO CTeRelacao
                                JOIN T_CTE CTeGerado on CTeGerado.CON_CODIGO = CTeRelacao.CON_CODIGO_GERADO
                                WHERE CTeRelacao.CRD_TIPO_CTE_GERADO = 2 AND CTeRelacao.CON_CODIGO_ORIGINAL = CTe.CON_CODIGO FOR XML PATH('')), 3, 1000) NumeroCTeComplementar, ");

                    if (!groupBy.Contains("CTe.CON_CODIGO"))
                        groupBy.Append("CTe.CON_CODIGO, ");
                    break;

                case "NumeroControleCTeComplementar":
                    if (!select.Contains(" NumeroControleCTeComplementar, "))
                        select.Append(
                            @"SUBSTRING((SELECT DISTINCT ', ' + CTeGerado.CON_NUMERO_CONTROLE
                                FROM T_CTE_RELACAO_DOCUMENTO CTeRelacao
                                JOIN T_CTE CTeGerado on CTeGerado.CON_CODIGO = CTeRelacao.CON_CODIGO_GERADO
                                WHERE CTeRelacao.CRD_TIPO_CTE_GERADO = 2 AND CTeRelacao.CON_CODIGO_ORIGINAL  = CTe.CON_CODIGO FOR XML PATH('')), 3, 1000) NumeroControleCTeComplementar, ");

                    if (!groupBy.Contains("CTe.CON_CODIGO"))
                        groupBy.Append("CTe.CON_CODIGO, ");
                    break;

                case "NumeroCTeSubstituto":
                    if (!select.Contains(" NumeroCTeSubstituto, "))
                        select.Append(
                            @"SUBSTRING((SELECT DISTINCT ', ' + CAST(CTeGerado.CON_NUM AS NVARCHAR(20))
                                FROM T_CTE_RELACAO_DOCUMENTO CTeRelacao
                                JOIN T_CTE CTeGerado on CTeGerado.CON_CODIGO = CTeRelacao.CON_CODIGO_GERADO
                                WHERE CTeRelacao.CRD_TIPO_CTE_GERADO = 3 AND CTeRelacao.CON_CODIGO_ORIGINAL  = CTe.CON_CODIGO FOR XML PATH('')), 3, 1000) NumeroCTeSubstituto, ");

                    if (!groupBy.Contains("CTe.CON_CODIGO"))
                        groupBy.Append("CTe.CON_CODIGO, ");
                    break;

                case "NumeroControleCTeSubstituto":
                    if (!select.Contains(" NumeroControleCTeSubstituto, "))
                        select.Append(
                            @"SUBSTRING((SELECT DISTINCT ', ' + CTeGerado.CON_NUMERO_CONTROLE
                                FROM T_CTE_RELACAO_DOCUMENTO CTeRelacao
                                JOIN T_CTE CTeGerado on CTeGerado.CON_CODIGO = CTeRelacao.CON_CODIGO_GERADO
                                WHERE CTeRelacao.CRD_TIPO_CTE_GERADO = 3 AND CTeRelacao.CON_CODIGO_ORIGINAL  = CTe.CON_CODIGO FOR XML PATH('')), 3, 1000) NumeroControleCTeSubstituto, ");

                    if (!groupBy.Contains("CTe.CON_CODIGO"))
                        groupBy.Append("CTe.CON_CODIGO, ");
                    break;

                case "NumeroCTeDuplicado":
                    if (!select.Contains(" NumeroCTeDuplicado, "))
                        select.Append(
                            @"SUBSTRING((SELECT DISTINCT ', ' + CAST(CTeGerado.CON_NUM AS NVARCHAR(20))
                                FROM T_CTE_RELACAO_DOCUMENTO CTeRelacao
                                JOIN T_CTE CTeGerado on CTeGerado.CON_CODIGO = CTeRelacao.CON_CODIGO_GERADO
                                WHERE CTeRelacao.CRD_TIPO_CTE_GERADO = 4 AND CTeRelacao.CON_CODIGO_ORIGINAL  = CTe.CON_CODIGO FOR XML PATH('')), 3, 1000) NumeroCTeDuplicado, ");

                    if (!groupBy.Contains("CTe.CON_CODIGO"))
                        groupBy.Append("CTe.CON_CODIGO, ");
                    break;

                case "NumeroControleCTeDuplicado":
                    if (!select.Contains(" NumeroControleCTeDuplicado, "))
                        select.Append(
                            @"SUBSTRING((SELECT DISTINCT ', ' + CTeGerado.CON_NUMERO_CONTROLE
                                FROM T_CTE_RELACAO_DOCUMENTO CTeRelacao
                                JOIN T_CTE CTeGerado on CTeGerado.CON_CODIGO = CTeRelacao.CON_CODIGO_GERADO
                                WHERE CTeRelacao.CRD_TIPO_CTE_GERADO = 4 AND CTeRelacao.CON_CODIGO_ORIGINAL  = CTe.CON_CODIGO FOR XML PATH('')), 3, 1000) NumeroControleCTeDuplicado, ");

                    if (!groupBy.Contains("CTe.CON_CODIGO"))
                        groupBy.Append("CTe.CON_CODIGO, ");
                    break;

                case "NumeroCTeOriginal":
                    if (!select.Contains(" NumeroCTeOriginal, "))
                        select.Append(
                            @"COALESCE((SELECT STRING_AGG(CAST(CTeOriginal.CON_NUM AS NVARCHAR(20)), ', ')
			                            FROM T_CTE_RELACAO_DOCUMENTO CTeRelacao
				                            JOIN T_CTE CTeOriginal ON CTeOriginal.CON_CODIGO = CTeRelacao.CON_CODIGO_ORIGINAL
			                            WHERE CTeRelacao.CON_CODIGO_GERADO = CTe.CON_CODIGO), CAST(CTeComplementado.CON_NUM AS NVARCHAR(20))) AS NumeroCTeOriginal, ");

                    if (!groupBy.Contains("CTeComplementado.CON_NUM"))
                        groupBy.Append("CTeComplementado.CON_NUM,CTe.CON_CODIGO, ");

                    SetarJoinsCTeOriginal(joins);
                    break;

                case "NumeroControleCTeOriginal":
                    if (!select.Contains(" NumeroControleCTeOriginal, "))
                        select.Append(
                            @"SUBSTRING((SELECT DISTINCT ', ' + CTeOriginal.CON_NUMERO_CONTROLE
                                FROM T_CTE_RELACAO_DOCUMENTO CTeRelacao
                                JOIN T_CTE CTeOriginal on CTeOriginal.CON_CODIGO = CTeRelacao.CON_CODIGO_ORIGINAL 
                                WHERE CTeRelacao.CON_CODIGO_GERADO  = CTe.CON_CODIGO FOR XML PATH('')), 3, 1000) NumeroControleCTeOriginal, ");

                    if (!groupBy.Contains("CTe.CON_CODIGO"))
                        groupBy.Append("CTe.CON_CODIGO, ");
                    break;

                case "NumeroCIOT":
                    if (!select.Contains(" NumeroCIOT, "))
                    {
                        select.Append(
                            @"SUBSTRING((
                                SELECT DISTINCT ', ' + ciot.CIO_NUMERO 
                                        FROM T_CIOT ciot
                                        inner join T_CARGA_CIOT cargaCiot on cargaCiot.CIO_CODIGO = ciot.CIO_CODIGO 
                                        inner join T_CARGA_CTE CargaCTe on CargaCTe.CAR_CODIGO = cargaCiot.CAR_CODIGO 
                                 WHERE CargaCTe.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 1000) NumeroCIOT, "
                        );

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append(" CTe.CON_CODIGO, ");
                    }
                    break;

                case "NumeroDocumentoOriginario":
                    if (!select.Contains(" NumeroDocumentoOriginario, "))
                    {
                        select.Append(
                            @"SUBSTRING((
                                SELECT DISTINCT ', ' + CONVERT(NVARCHAR(50), documentoOriginario.CDO_NUMERO) + '-' + documentoOriginario.CDO_SERIE
                                        FROM T_CTE_DOCUMENTO_ORIGINARIO documentoOriginario
                                 WHERE documentoOriginario.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 1000) NumeroDocumentoOriginario, "
                        );

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append(" CTe.CON_CODIGO, ");
                    }
                    break;

                case "DataPagamento":
                case "DataPagamentoFormatada":
                    if (!select.Contains(" DataPagamento, "))
                    {
                        select.Append("TituloCTe.TIT_DATA_LIQUIDACAO DataPagamento, ");
                        if (!groupBy.Contains("TituloCTe.TIT_DATA_LIQUIDACAO"))
                            groupBy.Append("TituloCTe.TIT_DATA_LIQUIDACAO, ");

                        SetarJoinsTitulo(joins);
                    }
                    break;

                case "DataInicioViagem":
                    if (!select.Contains(" DataInicioViagem, "))
                    {
                        select.Append(
                            @"SUBSTRING((
                                SELECT DISTINCT ', ' + CONVERT(NVARCHAR(10), _carga.CAR_DATA_INICIO_VIAGEM, 103) + ' ' + CONVERT(NVARCHAR(5), _carga.CAR_DATA_INICIO_VIAGEM, 108)
                                  FROM T_CARGA_CTE _cargaCTe 
                                 INNER JOIN T_CARGA _carga ON _carga.CAR_CODIGO = _cargaCTe.CAR_CODIGO_ORIGEM 
                                 WHERE _cargaCTe.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 200) DataInicioViagem, "
                        );

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "DataFimViagem":
                    if (!select.Contains(" DataFimViagem, "))
                    {
                        select.Append(
                            @"SUBSTRING((
                                SELECT DISTINCT ', ' + CONVERT(NVARCHAR(10), _carga.CAR_DATA_FIM_VIAGEM, 103) + ' ' + CONVERT(NVARCHAR(5), _carga.CAR_DATA_FIM_VIAGEM, 108)
                                  FROM T_CARGA_CTE _cargaCTe 
                                 INNER JOIN T_CARGA _carga ON _carga.CAR_CODIGO = _cargaCTe.CAR_CODIGO_ORIGEM 
                                 WHERE _cargaCTe.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 200) DataFimViagem, "
                        );

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "Taxa":
                    if (!select.Contains(" Taxa, "))
                    {
                        select.Append(@"(SELECT MAX(pedido.PED_VALOR_TAXA_FEEDER)
                                            FROM T_PEDIDO pedido
                                            JOIN T_CARGA_PEDIDO cargaPedido ON cargaPedido.PED_CODIGO = pedido.PED_CODIGO
                                            JOIN T_CARGA_CTE cargaCTe ON cargaCTe.CAR_CODIGO = cargaPedido.CAR_CODIGO
                                            WHERE cargaCTe.CON_CODIGO = CTe.CON_CODIGO) Taxa, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "QuantidadeContainer":
                    if (!select.Contains(" QuantidadeContainer, "))
                    {
                        select.Append(
                            @"(SELECT COUNT(1) from T_CONTAINER container 
                                 inner join T_CTE_CONTAINER cteContainer on cteContainer.CTR_CODIGO = container.CTR_CODIGO 
                                 WHERE cteContainer.CON_CODIGO = CTe.CON_CODIGO) QuantidadeContainer, "
                        );

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append(" CTe.CON_CODIGO, ");
                    }
                    break;

                case "NumeroDocumentoRecebedor":
                    if (!select.Contains(" NumeroDocumentoRecebedor, "))
                    {
                        select.Append(@" Ocorrencia.COC_NUMERO_DOCUMENTO_RECEBEDOR NumeroDocumentoRecebedor, ");

                        if (!groupBy.Contains("Ocorrencia.COC_NUMERO_DOCUMENTO_RECEBEDOR"))
                            groupBy.Append(" Ocorrencia.COC_NUMERO_DOCUMENTO_RECEBEDOR, ");

                        SetarJoinsOcorrencia(joins);
                    }
                    break;

                case "NumeroPedidoCliente":
                    if (!select.Contains(" NumeroPedidoCliente, "))
                    {
                        select.Append(@"substring((select distinct ', ' + _pedido.PED_CODIGO_PEDIDO_CLIENTE FROM T_PEDIDO _pedido
                            INNER JOIN T_CARGA_PEDIDO _cargaPedido ON _cargaPedido.PED_CODIGO = _pedido.PED_CODIGO
                            INNER JOIN T_CARGA_CTE _cargaCTe ON _cargaCTe.CAR_CODIGO = _cargaPedido.CAR_CODIGO 
                             WHERE _cargaCTe.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 200) NumeroPedidoCliente, ");
                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "QuantidadeTotalProduto":
                    if (!somenteContarNumeroRegistros && !select.Contains(" QuantidadeTotalProduto "))
                    {
                        select.Append(
                            @"(
                                select round(sum(_xmlProduto.XFP_QUANTIDADE), 2)
                                  from T_CTE_XML_NOTAS_FISCAIS _nFe
                                  join T_XML_NOTA_FISCAL_PRODUTO _xmlProduto on _xmlProduto.NFX_CODIGO = _nFe.NFX_CODIGO 
                                 where _nFe.CON_CODIGO = CTe.CON_CODIGO
                            ) QuantidadeTotalProduto, "
                        );

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "DistanciaCargaAgrupada":
                    if (!select.Contains(" DistanciaCargaAgrupada,"))
                    {
                        select.Append("(select SUM(DadosSumarizados.CDS_DISTANCIA) from T_CARGA_CTE CargaCTe inner join T_CARGA Carga on CargaCTe.CAR_CODIGO = Carga.CAR_CODIGO inner join T_CARGA_DADOS_SUMARIZADOS DadosSumarizados ON DadosSumarizados.CDS_CODIGO = Carga.CDS_CODIGO where CargaCTe.CON_CODIGO = CTe.CON_CODIGO) DistanciaCargaAgrupada , ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "ModeloVeiculo":
                    if (!select.Contains(" ModeloVeiculo,"))
                    {
                        select.Append("substring((select ', ' + VeiculoModelo.VMO_DESCRICAO from T_CTE_VEICULO veiculoCTe1 inner join T_VEICULO veiculo1 on veiculoCTe1.VEI_CODIGO = veiculo1.VEI_CODIGO inner join T_VEICULO_MODELO VeiculoModelo on VeiculoModelo.VMO_CODIGO = veiculo1.VMO_CODIGO where veiculoCTe1.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 1000) ModeloVeiculo, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "TipoCarroceria":
                    if (!select.Contains(" TipoCarroceria,"))
                    {
                        select.Append("substring((select ', ' + ");
                        select.Append(" case ");
                        select.Append("   WHEN Veiculo.VEI_TIPO_CARROCERIA = '00' THEN 'Não Aplicado' ");
                        select.Append("   WHEN Veiculo.VEI_TIPO_CARROCERIA = '01' THEN 'Aberta' ");
                        select.Append("   WHEN Veiculo.VEI_TIPO_CARROCERIA = '02' THEN 'Fechada/Baú' ");
                        select.Append("   WHEN Veiculo.VEI_TIPO_CARROCERIA = '03' THEN 'Granel' ");
                        select.Append("   WHEN Veiculo.VEI_TIPO_CARROCERIA = '04' THEN 'Porta Container' ");
                        select.Append("   WHEN Veiculo.VEI_TIPO_CARROCERIA = '05' THEN 'Utilitário' ");
                        select.Append("   WHEN Veiculo.VEI_TIPO_CARROCERIA = '06' THEN 'Sider' ");
                        select.Append("     else '' end ");
                        select.Append("   from T_CTE_VEICULO CteVeiculo inner join T_VEICULO Veiculo on CteVeiculo.VEI_CODIGO = Veiculo.VEI_CODIGO where CteVeiculo.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 1000) TipoCarroceria, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "OperadorResponsavelCancelamento":
                    if (!select.Contains(" OperadorResponsavelCancelamento, "))
                    {
                        select.Append(" (SELECT top 1 OperadorCancelamento.FUN_NOME from T_CARGA_CTE CargaCTe ");
                        select.Append("     join T_CARGA Carga ON CargaCTe.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("     join T_CARGA_CANCELAMENTO CargaCancelamento ON CargaCancelamento.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("     join T_FUNCIONARIO OperadorCancelamento ON OperadorCancelamento.FUN_CODIGO = CargaCancelamento.FUN_CODIGO_OPERADOR_RESPONSAVEL ");
                        select.Append("     where CargaCTe.CON_CODIGO = CTe.CON_CODIGO) OperadorResponsavelCancelamento, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;
                case "UsuarioSolicitante":
                    if (!select.Contains(" UsuarioSolicitante, "))
                    {
                        select.Append(" (SELECT TOP 1 UsuarioSolicitante.FUN_NOME from T_CARGA_CTE CargaCTe ");
                        select.Append("     join T_CARGA Carga ON CargaCTe.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("     join T_CARGA_CANCELAMENTO CargaCancelamento ON CargaCancelamento.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("     join T_FUNCIONARIO UsuarioSolicitante ON UsuarioSolicitante.FUN_CODIGO = CargaCancelamento.FUN_CODIGO ");
                        select.Append("     where CargaCTe.CON_CODIGO = CTe.CON_CODIGO) UsuarioSolicitante, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;
                case "VeiculoTracao":
                    if (!select.Contains(" VeiculoTracao,"))
                    {
                        select.Append("substring((select ', ' + veiculo1.VEI_PLACA from T_CTE_VEICULO veiculoCTe1 inner join T_VEICULO veiculo1 on veiculoCTe1.VEI_CODIGO = veiculo1.VEI_CODIGO where veiculoCTe1.CON_CODIGO = CTe.CON_CODIGO and veiculo1.VEI_TIPOVEICULO = 0 for xml path('')), 3, 1000) VeiculoTracao, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "ChassiTracao":
                    if (!select.Contains(" ChassiTracao,"))
                    {
                        select.Append("substring((select ', ' + veiculo1.VEI_CHASSI from T_CTE_VEICULO veiculoCTe1 inner join T_VEICULO veiculo1 on veiculoCTe1.VEI_CODIGO = veiculo1.VEI_CODIGO where veiculoCTe1.CON_CODIGO = CTe.CON_CODIGO and veiculo1.VEI_TIPOVEICULO = 0 for xml path('')), 3, 1000) ChassiTracao, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "VeiculoReboque":
                    if (!select.Contains(" VeiculoReboque,"))
                    {
                        select.Append("substring((select ', ' + veiculo1.VEI_PLACA from T_CTE_VEICULO veiculoCTe1 inner join T_VEICULO veiculo1 on veiculoCTe1.VEI_CODIGO = veiculo1.VEI_CODIGO where veiculoCTe1.CON_CODIGO = CTe.CON_CODIGO and veiculo1.VEI_TIPOVEICULO = 1 for xml path('')), 3, 1000) VeiculoReboque, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "KMRota":
                    if (!select.Contains(" KMRota, "))
                    {
                        select.Append(@"(SELECT TOP(1) RotaFrete.ROF_QUILOMETROS from T_ROTA_FRETE RotaFrete 
                                        inner join T_CARGA Carga on Carga.ROF_CODIGO = RotaFrete.ROF_CODIGO 
                                        inner join T_CARGA_CTE CargaCTe on CargaCTe.CAR_CODIGO = Carga.CAR_CODIGO 
                                        WHERE CargaCTe.CON_CODIGO = CTe.CON_CODIGO and (Carga.CAR_CARGA_TRANSBORDO is null or Carga.CAR_CARGA_TRANSBORDO = 0)) KMRota, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append(" CTe.CON_CODIGO, ");
                    }
                    break;

                case "DataConfirmacaoDocumento":
                    if (!select.Contains(" DataConfirmacaoDocumento, "))
                    {
                        select.Append("substring((select distinct ', ' + CONVERT(NVARCHAR(10), Carga.CAR_DATA_INICIO_CONFIRMACAO_DOCUMENTOS_FISCAIS, 103)  + ' ' + SUBSTRING(CONVERT(NVARCHAR(10), Carga.CAR_DATA_INICIO_CONFIRMACAO_DOCUMENTOS_FISCAIS, 8), 1, 5) from T_CARGA_CTE CargaCTe inner join T_CARGA Carga on Carga.CAR_CODIGO = CargaCTe.CAR_CODIGO_ORIGEM where CargaCTe.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 200) DataConfirmacaoDocumento, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "RuaRecebedor":
                    if (!select.Contains(" RuaRecebedor, "))
                    {
                        select.Append("ClienteRecebedor.CLI_ENDERECO RuaRecebedor, ");

                        if (!groupBy.Contains("ClienteRecebedor.CLI_ENDERECO"))
                            groupBy.Append("ClienteRecebedor.CLI_ENDERECO, ");

                        SetarJoinsRecebedorCliente(joins);
                    }
                    break;

                case "NumeroRecebedor":
                    if (!select.Contains(" NumeroRecebedor, "))
                    {
                        select.Append("ClienteRecebedor.CLI_NUMERO NumeroRecebedor, ");

                        if (!groupBy.Contains("ClienteRecebedor.CLI_NUMERO"))
                            groupBy.Append("ClienteRecebedor.CLI_NUMERO, ");

                        SetarJoinsRecebedorCliente(joins);
                    }
                    break;

                case "BairroRecebedor":
                    if (!select.Contains(" BairroRecebedor, "))
                    {
                        select.Append("ClienteRecebedor.CLI_BAIRRO BairroRecebedor, ");

                        if (!groupBy.Contains("ClienteRecebedor.CLI_BAIRRO"))
                            groupBy.Append("ClienteRecebedor.CLI_BAIRRO, ");

                        SetarJoinsRecebedorCliente(joins);
                    }
                    break;

                case "LacresCargaLacre":
                    if (!select.Contains(" LacresCargaLacre, "))
                    {
                        select.Append("substring(");
                        select.Append("    (select distinct ', ' + CLA_NUMERO FROM T_CARGA_LACRE cargaLacre");
                        select.Append("    INNER JOIN T_CARGA _carga ON _carga.CAR_CODIGO = cargaLacre.CAR_CODIGO");
                        select.Append("    INNER JOIN T_CARGA_CTE _cargaCTe ON _cargaCTe.CAR_CODIGO = _carga.CAR_CODIGO");
                        select.Append("    WHERE cargaLacre.CAR_CODIGO = _cargaCTe.CAR_CODIGO and _cargaCTe.CON_CODIGO = CTe.CON_CODIGO");
                        select.Append("    FOR XML path('')), 3, 1000) as LacresCargaLacre, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "PalletsPedido":
                    if (!select.Contains(" PalletsPedido, "))
                    {
                        select.Append("(select sum(_pedido.PED_NUMERO_PALETES_FRACIONADO + _pedido.PED_NUMERO_PALETES) ");
                        select.Append("      FROM T_PEDIDO _pedido ");
                        select.Append("      INNER JOIN T_CARGA_PEDIDO _cargaPedido ON _cargaPedido.PED_CODIGO = _pedido.PED_CODIGO ");
                        select.Append("      INNER JOIN T_CARGA_CTE _cargaCTe ON _cargaCTe.CAR_CODIGO = _cargaPedido.CAR_CODIGO ");
                        select.Append("      WHERE _cargaCTe.CON_CODIGO = CTe.CON_CODIGO) PalletsPedido, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO,"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;
                case "RegraICMS":
                    if (!select.Contains(" RegraICMS, "))
                    {
                        select.Append("RegraICMS.RIC_DESCRICAO_REGRA RegraICMS, ");

                        if (!groupBy.Contains("RegraICMS.RIC_DESCRICAO_REGRA,"))
                            groupBy.Append("RegraICMS.RIC_DESCRICAO_REGRA, ");

                        SetarJoinsRegraICMS(joins);
                    }
                    break;

                case "DescricaoRegraICMS":
                    if (!select.Contains(" DescricaoRegraICMS, "))
                    {
                        select.Append("RegraICMS.RIC_DESCRICAO DescricaoRegraICMS, ");

                        if (!groupBy.Contains("RegraICMS.RIC_DESCRICAO,"))
                            groupBy.Append("RegraICMS.RIC_DESCRICAO, ");

                        SetarJoinsRegraICMS(joins);
                    }
                    break;

                case "CPFCNPJTerceiro":
                    if (!select.Contains(" CPFCNPJTerceiro, "))
                    {
                        select.Append(
                            @"SUBSTRING((
                                SELECT DISTINCT ', ' + LTRIM(STR(terceiro.CLI_CGCCPF, 25, 0))
                                        from T_CLIENTE terceiro
                                        inner join T_CARGA Carga on Carga.CLI_CGCCPF_TERCEIRO = terceiro.CLI_CGCCPF
                                        inner join T_CARGA_CTE CargaCTe on CargaCTe.CAR_CODIGO = Carga.CAR_CODIGO 
                                 WHERE CargaCTe.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 1000) CPFCNPJTerceiro, "
                        );

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append(" CTe.CON_CODIGO, ");
                    }
                    break;
                case "NomeTerceiro":
                    if (!select.Contains(" NomeTerceiro, "))
                    {
                        select.Append(
                            @"SUBSTRING((
                                SELECT DISTINCT ', ' + terceiro.CLI_NOME
                                        from T_CLIENTE terceiro
                                        inner join T_CARGA Carga on Carga.CLI_CGCCPF_TERCEIRO = terceiro.CLI_CGCCPF
                                        inner join T_CARGA_CTE CargaCTe on CargaCTe.CAR_CODIGO = Carga.CAR_CODIGO 
                                 WHERE CargaCTe.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 1000) NomeTerceiro, "
                        );

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append(" CTe.CON_CODIGO, ");
                    }
                    break;

                case "NumeroEXP":
                    if (!select.Contains(" NumeroEXP, "))
                    {
                        select.Append("substring(( ");
                        select.Append("    select distinct ', ' + Pedido.PED_NUMERO_EXP ");
                        select.Append("      from T_PEDIDO Pedido ");
                        select.Append("      join T_CARGA_PEDIDO CargaPedido on CargaPedido.PED_CODIGO = Pedido.PED_CODIGO ");
                        select.Append("      join T_CARGA_CTE CargaCTe on CargaCTe.CAR_CODIGO = CargaPedido.CAR_CODIGO ");
                        select.Append("     where CargaCTe.CON_CODIGO = CTe.CON_CODIGO ");
                        select.Append("       and isnull(Pedido.PED_NUMERO_EXP, '') <> '' ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) NumeroEXP, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append(" CTe.CON_CODIGO, ");
                    }
                    break;

                case "NumeroContainer":
                    if (!select.Contains(" NumeroContainer, "))
                    {
                        select.Append("substring(( ");
                        select.Append("    select distinct ', ' + Pedido.PED_NUMERO_CONTAINER ");
                        select.Append("      from T_PEDIDO Pedido ");
                        select.Append("      join T_CARGA_PEDIDO CargaPedido on CargaPedido.PED_CODIGO = Pedido.PED_CODIGO ");
                        select.Append("      join T_CARGA_CTE CargaCTe on CargaCTe.CAR_CODIGO = CargaPedido.CAR_CODIGO ");
                        select.Append("     where CargaCTe.CON_CODIGO = CTe.CON_CODIGO ");
                        select.Append("       and isnull(Pedido.PED_NUMERO_CONTAINER, '') <> '' ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) NumeroContainer, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append(" CTe.CON_CODIGO, ");
                    }
                    break;

                case "JustificativaNaoEnviarParaMercante":
                    if (!select.Contains(" JustificativaNaoEnviarParaMercante, "))
                    {
                        select.Append("CTe.CON_JJUSTIFICATIVA_NAO_ENVIAR_PARA_MERCANTE JustificativaNaoEnviarParaMercante, ");
                        groupBy.Append("CTe.CON_JJUSTIFICATIVA_NAO_ENVIAR_PARA_MERCANTE, ");
                    }
                    break;

                case "SituacaoCanhotoFormatada":
                    if (!select.Contains(" SituacaoCanhoto, "))
                    {
                        select.Append(@"substring((select DISTINCT ', ' + CAST(_canhoto.CNF_SITUACAO_CANHOTO AS VARCHAR(10))
                                                    from T_CANHOTO_NOTA_FISCAL _canhoto
                                                    join T_CARGA_CTE _cargaCte on _canhoto.CAR_CODIGO = _cargaCte.CAR_CODIGO 
                                                    where _cargaCte.CON_CODIGO = CTe.CON_CODIGO and _canhoto.CNF_SITUACAO_CANHOTO <> 8 for xml path('')), 3, 100000) SituacaoCanhoto, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "SituacaoPagamentoFormatada":
                    if (!select.Contains(" SituacaoPagamento, "))
                    {
                        select.Append("ISNULL(Pagamento.PAG_SITUACAO, 0) SituacaoPagamento, ");
                        groupBy.Append("Pagamento.PAG_SITUACAO, ");

                        SetarJoinsPagamento(joins);
                    }
                    break;

                case "DataOcorrenciaFormatada":
                    if (!select.Contains(" DataOcorrencia, "))
                    {
                        select.Append("Ocorrencia.COC_DATA_OCORRENCIA DataOcorrencia, ");
                        groupBy.Append("Ocorrencia.COC_DATA_OCORRENCIA, ");

                        SetarJoinsOcorrencia(joins);
                    }
                    break;

                case "DataAprovacaoPagamentoFormatada":
                    if (!select.Contains(" DataAprovacaoPagamento, "))
                    {
                        select.Append("(select max(AAL_DATA) from T_AUTORIZACAO_ALCADA_PAGAMENTO where PAG_CODIGO = Pagamento.PAG_CODIGO and AAL_SITUACAO = 1) DataAprovacaoPagamento, ");
                        groupBy.Append("Pagamento.PAG_CODIGO, ");

                        SetarJoinsPagamento(joins);
                    }
                    break;

                case "Anexos":
                    if (!select.Contains(" Anexos, "))
                    {
                        select.Append("CASE WHEN (select count(*) from T_CARGA_CTE CargaCTe inner join T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE CargaPedidoXMLNotaFiscalCTe on CargaPedidoXMLNotaFiscalCTe.CCT_CODIGO = CargaCTe.CCT_CODIGO inner join T_PEDIDO_XML_NOTA_FISCAL PedidoXMLNotaFiscal on PedidoXMLNotaFiscal.PNF_CODIGO = CargaPedidoXMLNotaFiscalCTe.PNF_CODIGO inner join T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = PedidoXMLNotaFiscal.CPE_CODIGO inner join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO inner join T_PEDIDO_ANEXO Anexo on Anexo.PED_CODIGO = Pedido.PED_CODIGO where CargaCTe.CON_CODIGO = CTe.CON_CODIGO) > 0 THEN 'Sim' ELSE 'Não' END Anexos, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "DescricaoAnexos":
                    if (!select.Contains(" DescricaoAnexos, "))
                    {
                        select.Append("substring((select distinct ', ' + Anexo.ANX_DESCRICAO from T_CARGA_CTE CargaCTe inner join T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE CargaPedidoXMLNotaFiscalCTe on CargaPedidoXMLNotaFiscalCTe.CCT_CODIGO = CargaCTe.CCT_CODIGO inner join T_PEDIDO_XML_NOTA_FISCAL PedidoXMLNotaFiscal on PedidoXMLNotaFiscal.PNF_CODIGO = CargaPedidoXMLNotaFiscalCTe.PNF_CODIGO inner join T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = PedidoXMLNotaFiscal.CPE_CODIGO inner join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO inner join T_PEDIDO_ANEXO Anexo on Anexo.PED_CODIGO = Pedido.PED_CODIGO where CargaCTe.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 1000) DescricaoAnexos, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "CodigoTabelaFreteCliente":
                    if (!select.Contains(" CodigoTabelaFreteCliente, "))
                    {
                        select.Append(
                                @"SUBSTRING((
                                SELECT DISTINCT ', ' + 
                                            (CASE WHEN TabelaFrete.TBF_TIPO_CALCULO = 0 THEN (SUBSTRING((SELECT DISTINCT ', ' + 
		                                            TFC_CODIGO_INTEGRACAO 
			                                        FROM T_TABELA_FRETE_CLIENTE TabelaFreteCliente
			                                        JOIN T_CARGA_TABELA_FRETE_CLIENTE CargaTabelaFreteCliente on CargaTabelaFreteCliente.TFC_CODIGO = TabelaFreteCliente.TFC_CODIGO
			                                        WHERE CargaTabelaFreteCliente.CAR_CODIGO = Carga.CAR_CODIGO FOR XML PATH('')), 3, 3000)) --PorCarga
                                              --WHEN TabelaFrete.TBF_TIPO_CALCULO = 1 THEN () --PorPedido
                                              ELSE '' END)
                                        from T_TABELA_FRETE TabelaFrete
                                        inner join T_CARGA Carga on Carga.TBF_CODIGO = TabelaFrete.TBF_CODIGO
                                        inner join T_CARGA_CTE CargaCTe on CargaCTe.CAR_CODIGO = Carga.CAR_CODIGO 
                                 WHERE CargaCTe.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 5000) CodigoTabelaFreteCliente, "
                            );

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append(" CTe.CON_CODIGO, ");
                    }
                    break;

                case "DataVencimentoTituloFormatada":
                    if (!select.Contains(" DataVencimentoTitulo, "))
                    {
                        select.Append("TituloCTe.TIT_DATA_VENCIMENTO DataVencimentoTitulo, ");

                        if (!groupBy.Contains("TituloCTe.TIT_DATA_VENCIMENTO"))
                            groupBy.Append("TituloCTe.TIT_DATA_VENCIMENTO, ");


                        SetarJoinsTitulo(joins);
                    }
                    break;

                case "UFTransportador":
                    if (!select.Contains(" UFTransportador, "))
                    {
                        select.Append("LocalidadeTransportador.UF_SIGLA UFTransportador, ");

                        if (!groupBy.Contains("LocalidadeTransportador.UF_SIGLA"))
                            groupBy.Append("LocalidadeTransportador.UF_SIGLA, ");

                        SetarJoinsTransportadorLocalidade(joins);
                    }
                    break;

                case "TipoProprietario":
                    if (!select.Contains(" TipoProprietario, "))
                    {
                        select.Append("SUBSTRING(( " +
                            "   SELECT ', ' + " +
                            "       CASE " +
                            "           WHEN CTeVei.CVE_TIPO_PROPRIEDADE = 'P' THEN 'Próprio' " +
                            "           WHEN CTeVei.CVE_TIPO_PROPRIEDADE = 'T' AND CTeVeiProp.PVE_TIPO = 0 THEN 'TAC Agregado' " +
                            "           WHEN CTeVei.CVE_TIPO_PROPRIEDADE = 'T' AND CTeVeiProp.PVE_TIPO = 1 THEN 'TAC Independente' " +
                            "           WHEN CTeVei.CVE_TIPO_PROPRIEDADE = 'T' AND CTeVeiProp.PVE_TIPO = 2 THEN 'Outros' " +
                            "           ELSE '' " +
                            "       END " +
                            "   FROM T_CTE_VEICULO CTeVei " +
                            "       LEFT JOIN T_CTE_VEICULO_PROPRIETARIO CTeVeiProp ON CTeVeiProp.PVE_CODIGO = CTeVei.PVE_CODIGO " +
                            "   WHERE CTeVei.CON_CODIGO = CTe.CON_CODIGO " +
                            "   FOR XML PATH('') " +
                            "), 3, 1000) TipoProprietario, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "JustificativaMotivoMercante":
                    if (!select.Contains(" JustificativaMotivoMercante, "))
                    {
                        select.Append("JustificativaMercante.JME_DESCRICAO JustificativaMotivoMercante, ");
                        groupBy.Append("JustificativaMercante.JME_DESCRICAO, ");

                        SetarJoinsJustificativaMercante(joins);
                    }
                    break;

                case "DescricaoTipoModal":
                    if (!select.Contains(" TipoModal, "))
                    {
                        select.Append("CTe.CON_TIPO_MODAL TipoModal, ");
                        groupBy.Append("CTe.CON_TIPO_MODAL, ");
                    }
                    break;

                case "NumeroContratoFreteTerceiro":
                    if (!select.Contains(" NumeroContratoFreteTerceiro, "))
                    {
                        select.Append(@"substring((select distinct ', ' + CAST(ContratoFreteTerceiro.CFT_NUMERO_CONTRATO AS VARCHAR(10)) from T_CARGA_CTE CargaCTe
                                            inner join T_CARGA Carga on Carga.CAR_CODIGO = CargaCTe.CAR_CODIGO_ORIGEM
                                            join T_CONTRATO_FRETE_TERCEIRO ContratoFreteTerceiro on Carga.CAR_CODIGO = ContratoFreteTerceiro.CAR_CODIGO
                                            where CargaCTe.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 200) NumeroContratoFreteTerceiro, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "SituacaoFaturaDescricao":
                    if (!select.Contains(" SituacaoFatura, "))
                    {
                        select.Append("Fatura.FAT_SITUACAO SituacaoFatura, ");
                        groupBy.Append("Fatura.FAT_SITUACAO, ");

                        SetarJoinsFatura(joins);
                    }
                    break;

                case "NumeroDocumentoOriginal":
                    if (!select.Contains(" NumeroDocumentoOriginal, "))
                    {
                        select.Append(@"COALESCE((SELECT STRING_AGG(CAST(CTeOriginal.CON_NUM AS NVARCHAR(20)), ', ')
			                                      FROM T_CTE_RELACAO_DOCUMENTO CTeRelacao
		  		                                      JOIN T_CTE CTeOriginal ON CTeOriginal.CON_CODIGO = CTeRelacao.CON_CODIGO_ORIGINAL
			                                      WHERE CTeRelacao.CON_CODIGO_GERADO = CTe.CON_CODIGO), CAST(CTeComplementado.CON_NUM AS NVARCHAR(20))) AS NumeroDocumentoOriginal, ");

                        if (!groupBy.Contains("CTeComplementado.CON_NUM"))
                            groupBy.Append("CTeComplementado.CON_NUM, ");

                        SetarJoinsCTeOriginal(joins);
                    }
                    break;

                case "ChaveCTeOriginal":
                    if (!select.Contains(" ChaveCTeOriginal, "))
                    {
                        select.Append(@"COALESCE((SELECT STRING_AGG(CTeComplementado.CON_CHAVECTE, ', ')
			                                      FROM T_CTE_RELACAO_DOCUMENTO CTeRelacao
				                                      JOIN T_CTE CTeComplementado ON CTeComplementado.CON_CODIGO = CTeRelacao.CON_CODIGO_ORIGINAL
			                                      WHERE CTeRelacao.CRD_TIPO_CTE_GERADO = 2 
				                                      AND CTeRelacao.CON_CODIGO_GERADO = CTe.CON_CODIGO), CTeComplementado.CON_CHAVECTE) AS ChaveCTeOriginal, ");

                        if (!groupBy.Contains("CTeComplementado.CON_CHAVECTE"))
                            groupBy.Append("CTeComplementado.CON_CHAVECTE, ");

                        SetarJoinsCTeOriginal(joins);
                    }
                    break;

                case "ValorReceberCTeOriginal":
                    if (!select.Contains(" ValorReceberCTeOriginal, "))
                    {
                        select.Append(
                            @"(SELECT SUM(CTeComplementado.CON_VALOR_RECEBER)
			                FROM T_CTE_RELACAO_DOCUMENTO CTeRelacao
			                JOIN T_CTE CTeComplementado on CTeComplementado.CON_CODIGO = CTeRelacao.CON_CODIGO_ORIGINAL
			                WHERE CTeRelacao.CRD_TIPO_CTE_GERADO = 2 AND CTeRelacao.CON_CODIGO_GERADO = CTe.CON_CODIGO) ValorReceberCTeOriginal, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "Terceiro":
                    if (!select.Contains(" Terceiro, "))
                    {
                        select.Append("SUBSTRING(( " +
                            "   SELECT ', ' + " +
                            "       CASE " +
                            "           WHEN Carga.CAR_FRETE_TERCEIRO = 1 THEN 'Sim' " +
                            "           ELSE 'Não' " +
                            "       END " +
                            "   FROM T_CARGA_CTE CargaCTe" +
                            "       INNER JOIN T_CARGA Carga ON Carga.CAR_CODIGO = CargaCTe.CAR_CODIGO " +
                            "   WHERE CargaCTe.CON_CODIGO = CTe.CON_CODIGO " +
                            "   FOR XML PATH('') " +
                            "), 3, 500) Terceiro, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "Redespacho":
                    if (!select.Contains(" Redespacho, "))
                    {
                        select.Append("SUBSTRING((" +
                            "   SELECT ', ' + " +
                            "       CASE " +
                            "           WHEN Carga.RED_CODIGO > 0 OR Carga.CAR_CONTRATACAO_CARGA IN (4,5) THEN 'Sim' " +
                            "           ELSE 'Não' " +
                            "       END " +
                            "   FROM T_CARGA_CTE CargaCTe " +
                            "       INNER JOIN T_CARGA Carga ON Carga.CAR_CODIGO = CargaCTe.CAR_CODIGO " +
                            "   WHERE CargaCTe.CON_CODIGO = CTe.CON_CODIGO " +
                            "   FOR XML PATH('')" +
                            "), 3, 500) Redespacho, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "CargaDataCarregamentoFormatada":
                    if (!select.Contains(" CargaDataCarregamento, "))
                    {
                        select.Append(@"(SELECT TOP(1) Carga.CAR_DATA_CARREGAMENTO from T_CARGA Carga 
                                        inner join T_CARGA_CTE CargaCTe on CargaCTe.CAR_CODIGO = Carga.CAR_CODIGO 
                                        WHERE CargaCTe.CON_CODIGO = CTe.CON_CODIGO) CargaDataCarregamento, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append(" CTe.CON_CODIGO, ");
                    }
                    break;

                case "FuncionarioResponsavel":
                    if (!select.Contains(" FuncionarioResponsavel, "))
                    {
                        select.Append(@"SUBSTRING(((SELECT distinct ', ' + funcionaro.FUN_NOME
                                       FROM T_CTE_VEICULO cteVeiculo
                                       JOIN T_FUNCIONARIO funcionaro ON cteVeiculo.FUN_CODIGO_RESPONSAVEL = funcionaro.FUN_CODIGO
                                       WHERE cteVeiculo.CON_CODIGO = CTe.CON_CODIGO FOR XML PATH(''))), 3, 1000) FuncionarioResponsavel, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append(" CTe.CON_CODIGO, ");
                    }
                    break;

                case "DescricaoExcecaoCab":
                    if (!select.Contains(" ExcecaoCab, "))
                    {
                        select.Append(@"(Select TOP 1 DadosSumarizados.CDS_EXCECAO_CAB
	                                    FROM T_CARGA_CTE CargaCTe
		                                JOIN T_CARGA Carga on CargaCTe.CAR_CODIGO = Carga.CAR_CODIGO
		                                JOIN T_CARGA_DADOS_SUMARIZADOS DadosSumarizados ON DadosSumarizados.CDS_CODIGO = Carga.CDS_CODIGO
                                        WHERE CargaCTe.CON_CODIGO = CTe.CON_CODIGO) ExcecaoCab, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append(" CTe.CON_CODIGO, ");
                    }
                    break;

                case "NumeroCTEAnterior":
                    if (!select.Contains("NumeroCTEAnterior"))
                    {
                        select.Append("DocumentoOriginario.CDO_NUMERO NumeroCTEAnterior, ");
                        groupBy.Append("DocumentoOriginario.CDO_NUMERO, ");

                        SetarJoinsDocumentoOriginario(joins);
                    }
                    break;

                case "ChaveCTEAnterior":
                    if (!select.Contains("ChaveCTEAnterior"))
                    {
                        select.Append("DocumentoOriginario.CDO_CHAVE ChaveCTEAnterior, ");
                        groupBy.Append("DocumentoOriginario.CDO_CHAVE, ");

                        SetarJoinsDocumentoOriginario(joins);
                    }
                    break;

                case "NumeroCarregamento":
                    if (!select.Contains("NumeroCarregamento"))
                    {
                        select.Append(@"SUBSTRING((
                                        select distinct( ', ' + PED_NUMERO_CARREGAMENTO) from T_CARGA_CTE cargacte inner join
                                        T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE cpnf on cpnf.CCT_CODIGO = cargacte.CCT_CODIGO
                                        inner join T_PEDIDO_XML_NOTA_FISCAL pnf on pnf.PNF_CODIGO = cpnf.PNF_CODIGO
                                        inner join T_CARGA_PEDIDO cargaPedido on cargaPedido.CPE_CODIGO = pnf.CPE_CODIGO
                                        inner join T_PEDIDO pedido on pedido.PED_CODIGO = cargaPedido.PED_CODIGO
                                        where cargacte.CAR_CODIGO = cargaPedido.CAR_CODIGO and cargacte.CON_CODIGO = CTe.CON_CODIGO
		                            	FOR XML path('')
                                        ), 3, 1000) NumeroCarregamento, ");
                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append(" CTe.CON_CODIGO, ");
                    }
                    break;

                case "ExisteEscrituracao":
                case "CodigoEscrituracao":
                    if (!select.Contains("CodigoEscrituracao,"))
                    {
                        select.Append("CTe.CON_CODIGO_ESCRITURACAO CodigoEscrituracao, ");
                        groupBy.Append("CTe.CON_CODIGO_ESCRITURACAO, ");
                    }
                    break;

                case "NumeroFolha":
                    if (!select.Contains("NumeroFolha,"))
                    {
                        select.Append(@"
                            case
                                when CTe.CON_STATUS = 'C' then ''
                                else substring((
                                    select distinct ', ' + _Stage.STA_NUMERO_FOLHA 
                                      from T_CARGA_CTE CargaCTe 
                                      join T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE CargaPedidoXMLNotaFiscalCTe on CargaPedidoXMLNotaFiscalCTe.CCT_CODIGO = CargaCTe.CCT_CODIGO 
                                      join T_PEDIDO_XML_NOTA_FISCAL PedidoXMLNotaFiscal on PedidoXMLNotaFiscal.PNF_CODIGO = CargaPedidoXMLNotaFiscalCTe.PNF_CODIGO 
                                      join T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = PedidoXMLNotaFiscal.CPE_CODIGO 
                                      join T_PEDIDO_STAGE PedidoStage on PedidoStage.PED_CODIGO = CargaPedido.PED_CODIGO 
                                      join T_STAGE _Stage on _Stage.STA_CODIGO = PedidoStage.STA_CODIGO 
                                     where CargaCTe.CON_CODIGO = CTe.CON_CODIGO
                                       for xml path('')
                                ), 3, 200)
                            end NumeroFolha, "
                        );

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append(" CTe.CON_CODIGO, ");

                        if (!groupBy.Contains("CTe.CON_STATUS"))
                            groupBy.Append(" CTe.CON_STATUS, ");
                    }
                    break;

                case "DataFolhaFormatada":
                case "DataFolha":
                    if (!select.Contains("DataFolha,"))
                    {
                        select.Append(@"
                            case
                                when CTe.CON_STATUS = 'C' then ''
                                else substring((
                                    select distinct ', ' + convert(varchar(10), _Stage.STA_DATA_FOLHA, 101)
                                      from T_CARGA_CTE CargaCTe 
                                      join T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE CargaPedidoXMLNotaFiscalCTe on CargaPedidoXMLNotaFiscalCTe.CCT_CODIGO = CargaCTe.CCT_CODIGO 
                                      join T_PEDIDO_XML_NOTA_FISCAL PedidoXMLNotaFiscal on PedidoXMLNotaFiscal.PNF_CODIGO = CargaPedidoXMLNotaFiscalCTe.PNF_CODIGO 
                                      join T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = PedidoXMLNotaFiscal.CPE_CODIGO 
                                      join T_PEDIDO_STAGE PedidoStage on PedidoStage.PED_CODIGO = CargaPedido.PED_CODIGO 
                                      join T_STAGE _Stage on _Stage.STA_CODIGO = PedidoStage.STA_CODIGO 
                                     where CargaCTe.CON_CODIGO = CTe.CON_CODIGO
                                       for xml path('')
                                ), 3, 200)
                            end DataFolha, "
                        );

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append(" CTe.CON_CODIGO, ");

                        if (!groupBy.Contains("CTe.CON_STATUS"))
                            groupBy.Append(" CTe.CON_STATUS, ");
                    }
                    break;

                case "FolhaCalculada":
                    if (!select.Contains("FolhaCalculada,"))
                    {
                        select.Append(@"
                            case
                                when CTe.CON_STATUS = 'C' then ''
                                else substring((
                                    select distinct ', ' + _Stage.STA_CALCULO 
                                      from T_CARGA_CTE CargaCTe 
                                      join T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE CargaPedidoXMLNotaFiscalCTe on CargaPedidoXMLNotaFiscalCTe.CCT_CODIGO = CargaCTe.CCT_CODIGO 
                                      join T_PEDIDO_XML_NOTA_FISCAL PedidoXMLNotaFiscal on PedidoXMLNotaFiscal.PNF_CODIGO = CargaPedidoXMLNotaFiscalCTe.PNF_CODIGO 
                                      join T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = PedidoXMLNotaFiscal.CPE_CODIGO 
                                      join T_PEDIDO_STAGE PedidoStage on PedidoStage.PED_CODIGO = CargaPedido.PED_CODIGO 
                                      join T_STAGE _Stage on _Stage.STA_CODIGO = PedidoStage.STA_CODIGO 
                                     where CargaCTe.CON_CODIGO = CTe.CON_CODIGO
                                       for xml path('')
                                ), 3, 200)
                            end FolhaCalculada, "
                        );

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append(" CTe.CON_CODIGO, ");

                        if (!groupBy.Contains("CTe.CON_STATUS"))
                            groupBy.Append(" CTe.CON_STATUS, ");
                    }
                    break;

                case "FolhaAtribuida":
                    if (!select.Contains("FolhaAtribuida,"))
                    {
                        select.Append(@"
                            case
                                when CTe.CON_STATUS = 'C' then ''
                                else substring((
                                    select distinct ', ' + _Stage.STA_ATRIBUIDO 
                                      from T_CARGA_CTE CargaCTe 
                                      join T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE CargaPedidoXMLNotaFiscalCTe on CargaPedidoXMLNotaFiscalCTe.CCT_CODIGO = CargaCTe.CCT_CODIGO 
                                      join T_PEDIDO_XML_NOTA_FISCAL PedidoXMLNotaFiscal on PedidoXMLNotaFiscal.PNF_CODIGO = CargaPedidoXMLNotaFiscalCTe.PNF_CODIGO 
                                      join T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = PedidoXMLNotaFiscal.CPE_CODIGO 
                                      join T_PEDIDO_STAGE PedidoStage on PedidoStage.PED_CODIGO = CargaPedido.PED_CODIGO 
                                      join T_STAGE _Stage on _Stage.STA_CODIGO = PedidoStage.STA_CODIGO 
                                     where CargaCTe.CON_CODIGO = CTe.CON_CODIGO
                                       for xml path('')
                                ), 3, 200)
                            end FolhaAtribuida, "
                        );

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append(" CTe.CON_CODIGO, ");

                        if (!groupBy.Contains("CTe.CON_STATUS"))
                            groupBy.Append(" CTe.CON_STATUS, ");
                    }
                    break;

                case "FolhaTransferida":
                    if (!select.Contains("FolhaTransferida,"))
                    {
                        select.Append(@"
                            case
                                when CTe.CON_STATUS = 'C' then ''
                                else substring((
                                    select distinct ', ' + _Stage.STA_TRANSFERIDO 
                                      from T_CARGA_CTE CargaCTe 
                                      join T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE CargaPedidoXMLNotaFiscalCTe on CargaPedidoXMLNotaFiscalCTe.CCT_CODIGO = CargaCTe.CCT_CODIGO 
                                      join T_PEDIDO_XML_NOTA_FISCAL PedidoXMLNotaFiscal on PedidoXMLNotaFiscal.PNF_CODIGO = CargaPedidoXMLNotaFiscalCTe.PNF_CODIGO 
                                      join T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = PedidoXMLNotaFiscal.CPE_CODIGO 
                                      join T_PEDIDO_STAGE PedidoStage on PedidoStage.PED_CODIGO = CargaPedido.PED_CODIGO 
                                      join T_STAGE _Stage on _Stage.STA_CODIGO = PedidoStage.STA_CODIGO 
                                     where CargaCTe.CON_CODIGO = CTe.CON_CODIGO
                                       for xml path('')
                                ), 3, 200)
                            end FolhaTransferida, "
                        );

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append(" CTe.CON_CODIGO, ");

                        if (!groupBy.Contains("CTe.CON_STATUS"))
                            groupBy.Append(" CTe.CON_STATUS, ");
                    }
                    break;

                case "FolhaCancelada":
                case "FolhaCanceladaFormatada":
                    if (!select.Contains("FolhaCancelada,"))
                    {
                        select.Append(@"
                            case
                                when CTe.CON_STATUS = 'C' then ''
                                else substring((
                                     select distinct ', ' + (case when _Stage.STA_CANCELADO = 1 then 'Sim' else 'Não' end)  
                                      from T_CARGA_CTE CargaCTe 
                                      join T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE CargaPedidoXMLNotaFiscalCTe on CargaPedidoXMLNotaFiscalCTe.CCT_CODIGO = CargaCTe.CCT_CODIGO 
                                      join T_PEDIDO_XML_NOTA_FISCAL PedidoXMLNotaFiscal on PedidoXMLNotaFiscal.PNF_CODIGO = CargaPedidoXMLNotaFiscalCTe.PNF_CODIGO 
                                      join T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = PedidoXMLNotaFiscal.CPE_CODIGO 
                                      join T_PEDIDO_STAGE PedidoStage on PedidoStage.PED_CODIGO = CargaPedido.PED_CODIGO 
                                      join T_STAGE _Stage on _Stage.STA_CODIGO = PedidoStage.STA_CODIGO 
                                     where CargaCTe.CON_CODIGO = CTe.CON_CODIGO
                                       for xml path('')
                                ), 3, 200)
                            end FolhaCancelada, "
                        );

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append(" CTe.CON_CODIGO, ");

                        if (!groupBy.Contains("CTe.CON_STATUS"))
                            groupBy.Append(" CTe.CON_STATUS, ");
                    }
                    break;

                case "FolhaInconsistente":
                case "FolhaInconsistenteFormatada":
                    if (!select.Contains("FolhaInconsistente,"))
                    {
                        select.Append(@"
                            case
                                when CTe.CON_STATUS = 'C' then ''
                                else substring((
                                    select distinct ', ' + (case when _Stage.STA_INCONSISTENTE = 1 then 'Sim' else 'Não' end) 
                                      from T_CARGA_CTE CargaCTe 
                                      join T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE CargaPedidoXMLNotaFiscalCTe on CargaPedidoXMLNotaFiscalCTe.CCT_CODIGO = CargaCTe.CCT_CODIGO 
                                      join T_PEDIDO_XML_NOTA_FISCAL PedidoXMLNotaFiscal on PedidoXMLNotaFiscal.PNF_CODIGO = CargaPedidoXMLNotaFiscalCTe.PNF_CODIGO 
                                      join T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = PedidoXMLNotaFiscal.CPE_CODIGO 
                                      join T_PEDIDO_STAGE PedidoStage on PedidoStage.PED_CODIGO = CargaPedido.PED_CODIGO 
                                      join T_STAGE _Stage on _Stage.STA_CODIGO = PedidoStage.STA_CODIGO 
                                     where CargaCTe.CON_CODIGO = CTe.CON_CODIGO
                                       for xml path('')
                                ), 3, 200)
                            end FolhaInconsistente, "
                        );

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append(" CTe.CON_CODIGO, ");

                        if (!groupBy.Contains("CTe.CON_STATUS"))
                            groupBy.Append(" CTe.CON_STATUS, ");
                    }
                    break;

                case "InconsistenciaFolha":
                    if (!select.Contains("InconsistenciaFolha,"))
                    {
                        select.Append(@"
                            case
                                when CTe.CON_STATUS = 'C' then ''
                                else substring((
                                    select distinct ', ' + _Stage.STA_MENSAGEM_RETORNO_ETAPA
                                      from T_CARGA_CTE CargaCTe 
                                      join T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE CargaPedidoXMLNotaFiscalCTe on CargaPedidoXMLNotaFiscalCTe.CCT_CODIGO = CargaCTe.CCT_CODIGO 
                                      join T_PEDIDO_XML_NOTA_FISCAL PedidoXMLNotaFiscal on PedidoXMLNotaFiscal.PNF_CODIGO = CargaPedidoXMLNotaFiscalCTe.PNF_CODIGO 
                                      join T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = PedidoXMLNotaFiscal.CPE_CODIGO 
                                      join T_PEDIDO_STAGE PedidoStage on PedidoStage.PED_CODIGO = CargaPedido.PED_CODIGO 
                                      join T_STAGE _Stage on _Stage.STA_CODIGO = PedidoStage.STA_CODIGO 
                                     where CargaCTe.CON_CODIGO = CTe.CON_CODIGO
                                       for xml path('')
                                ), 3, 200)
                            end InconsistenciaFolha, "
                        );

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append(" CTe.CON_CODIGO, ");

                        if (!groupBy.Contains("CTe.CON_STATUS"))
                            groupBy.Append(" CTe.CON_STATUS, ");
                    }
                    break;

                case "SubstituicaoDescricao":
                    if (!select.Contains("Substituicao,"))
                    {
                        select.Append("CTe.CON_POSSUI_PEDIDO_SUBSTITUICAO Substituicao, ");
                        groupBy.Append("CTe.CON_POSSUI_PEDIDO_SUBSTITUICAO, ");

                    }
                    break;
                case "Vendedor":
                    if (!select.Contains("Vendedor,"))
                    {
                        select.Append(@"substring(
                                            (
                                            select
                                                ', ' +
                                                f.FUN_NOME
                                            from
                                                T_CTE as _cte
                                            join T_CTE_PARTICIPANTE cp on cp.PCT_CODIGO = _cte.CON_TOMADOR_PAGADOR_CTE
                                            join T_GRUPO_PESSOAS gp on cp.GRP_CODIGO = gp.GRP_CODIGO
                                            join T_GRUPO_PESSOAS_FUNCIONARIO gpf on gpf.GRP_CODIGO = gp.GRP_CODIGO
                                            join T_FUNCIONARIO f on f.FUN_CODIGO = gpf.FUN_CODIGO
                                            where
                                                CTe.CON_CODIGO = _cte.CON_CODIGO for xml path('')
                                            ), 
                                            3, 
                                            1000
                                        ) Vendedor, "
                        );
                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append(" CTe.CON_CODIGO, ");
                    }
                    break;
                case "ValorFreteTerceiro":
                    if (!select.Contains("ValorFreteTerceiro,"))
                    {
                        select.Append(" ContratoFreteTerceiro.CFT_VALOR_FRETE_SUB_CONTRATACAO ValorFreteTerceiro, ");
                        groupBy.Append("ContratoFreteTerceiro.CFT_VALOR_FRETE_SUB_CONTRATACAO, ");

                        SetarJoinsContratoFreteTerceiro(joins);

                    }
                    break;
                case "NumeroCompleto":
                    if (!select.Contains("NumeroCompleto,"))
                    {
                        select.Append(@" substring(
                                            (
                                            select
                                                ', ' +
                                                CAST(cteComplementar.CON_NUM as varchar)
                                            from
                                                T_CTE as _cte
                                            join T_CTE_RELACAO_DOCUMENTO cteRelacao on cteRelacao.CON_CODIGO_ORIGINAL = _cte.CON_CODIGO

                                            join T_CTE cteComplementar on cteComplementar.CON_CODIGO = cteRelacao.CON_CODIGO_GERADO
                                            where
                                                CTe.CON_CODIGO = _cte.CON_CODIGO for xml path('')
                                            ), 
                                            3, 
                                            1000
                                        ) NumeroCompleto, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;
                case "Etapa":
                    if (!select.Contains("Etapa,"))
                    {
                        select.Append(@" substring((select  distinct ', ' + _stage.STA_NUMERO_STAGE from T_CARGA_CTE CargaCTe 
								join T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE _cargaPedidoCte on _cargaPedidoCte.CCT_CODIGO = CargaCTe.CCT_CODIGO 
								join T_PEDIDO_XML_NOTA_FISCAL _pedidoXmlNotaFiscal on _pedidoXmlNotaFiscal.PNF_CODIGO = _cargaPedidoCte.PNF_CODIGO
								join T_CARGA_PEDIDO _cargaPedido on _cargaPedido.CPE_CODIGO = _pedidoXmlNotaFiscal.CPE_CODIGO
								join T_STAGE _stage on _stage.STA_CODIGO =_cargaPedido.STA_CODIGO_RELEVANTE_CUSTO where 
							CargaCTe.CON_CODIGO = CTe.CON_CODIGO for xml path('')),3, 200) Etapa, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;


                case "Ordem":
                    if (!select.Contains(" Ordem, "))
                    {
                        select.Append(@"substring((select distinct ', ' + _pedido.PED_ORDEM FROM T_PEDIDO _pedido
                            INNER JOIN T_CARGA_PEDIDO _cargaPedido ON _cargaPedido.PED_CODIGO = _pedido.PED_CODIGO
                            INNER JOIN T_CARGA_CTE _cargaCTe ON _cargaCTe.CAR_CODIGO = _cargaPedido.CAR_CODIGO 
                             WHERE _cargaCTe.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 200) Ordem, ");
                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "NumeroMiro":
                    if (!select.Contains("NumeroMiro,"))
                    {
                        select.Append("DocumentoFaturamentoCTe.DFA_NUMERO_MIRO NumeroMiro, ");

                        if (!groupBy.Contains("DocumentoFaturamentoCTe.DFA_NUMERO_MIRO"))
                            groupBy.Append("DocumentoFaturamentoCTe.DFA_NUMERO_MIRO, ");

                        SetarJoinsDocumentoFaturamento(joins);
                    }
                    break;
                case "NumeroEstorno":
                    if (!select.Contains("NumeroEstorno,"))
                    {
                        select.Append("DocumentoFaturamentoCTe.DFA_NUMERO_ESTORNO NumeroEstorno, ");

                        if (!groupBy.Contains("DocumentoFaturamentoCTe.DFA_NUMERO_ESTORNO"))
                            groupBy.Append("DocumentoFaturamentoCTe.DFA_NUMERO_ESTORNO, ");

                        SetarJoinsDocumentoFaturamento(joins);
                    }
                    break;
                case "Bloqueio":
                    if (!select.Contains("Bloqueio,"))
                    {
                        select.Append("DocumentoFaturamentoCTe.DFA_BLOQUEIO Bloqueio, ");

                        if (!groupBy.Contains("DocumentoFaturamentoCTe.DFA_BLOQUEIO"))
                            groupBy.Append("DocumentoFaturamentoCTe.DFA_BLOQUEIO, ");

                        SetarJoinsDocumentoFaturamento(joins);
                    }
                    break;
                case "DataMiro":
                    if (!select.Contains("DataMiro,"))
                    {
                        select.Append("FORMAT(DocumentoFaturamentoCTe.DFA_DATA_MIRO, 'dd/MM/yyyy') DataMiro, ");

                        if (!groupBy.Contains("DocumentoFaturamentoCTe.DFA_DATA_MIRO"))
                            groupBy.Append("DocumentoFaturamentoCTe.DFA_DATA_MIRO, ");

                        SetarJoinsDocumentoFaturamento(joins);
                    }
                    break;
                case "Vencimento":
                    if (!select.Contains("Vencimento,"))
                    {
                        select.Append("FORMAT(DocumentoFaturamentoCTe.DFA_VENCIMENTO, 'dd/MM/yyyy') Vencimento, ");

                        if (!groupBy.Contains("DocumentoFaturamentoCTe.DFA_VENCIMENTO"))
                            groupBy.Append("DocumentoFaturamentoCTe.DFA_VENCIMENTO, ");

                        SetarJoinsDocumentoFaturamento(joins);
                    }
                    break;
                case "TermoPagamento":
                    if (!select.Contains("TermoPagamento,"))
                    {
                        select.Append("TermoPagamento.TPG_DESCRICAO TermoPagamento, ");

                        if (!groupBy.Contains("TermoPagamento.TPG_DESCRICAO"))
                            groupBy.Append("TermoPagamento.TPG_DESCRICAO, ");

                        SetarJoinsTermoPagamento(joins);
                    }
                    break;

                case "BCCTeSubstituido":
                    if (!select.Contains("BCCTeSubstituido,"))
                    {
                        select.Append(@"(
                                            SELECT 
                                                TOP(1) Origem.CON_BC_ICMS 
                                            FROM 
                                                T_CTE_RELACAO_DOCUMENTO CTeRelacaoDocumento
                                                JOIN T_CTE Origem on Origem.CON_CODIGO = CTeRelacaoDocumento.CON_CODIGO_ORIGINAL 
                                            WHERE 
                                                CTeRelacaoDocumento.CON_CODIGO_GERADO = CTe.CON_CODIGO
                                                AND CTe.CON_TIPO_CTE = 3
                                            ) BCCTeSubstituido, ");

                        if (!groupBy.Contains(" CTe.CON_CODIGO"))
                            groupBy.Append(" CTe.CON_CODIGO, ");
                    }
                    break;
                case "FreteInformadoManualmente":
                    if (!select.Contains("TipoFreteEscolhido,"))
                    {
                        select.Append(@"(
                                          SELECT TOP 1
                                            Carga.CAR_TIPO_FRETE_ESCOLHIDO
                                          FROM 
                                            T_CARGA_CTE CargaCTe 
                                            INNER JOIN T_CARGA Carga ON Carga.CAR_CODIGO = CargaCTe.CAR_CODIGO 
                                          WHERE 
                                            CargaCTe.CON_CODIGO = CTe.CON_CODIGO
                                            and Carga.CAR_TIPO_FRETE_ESCOLHIDO = 2
	                                    ) TipoFreteEscolhido,");

                        if (!groupBy.Contains(" CTe.CON_CODIGO"))
                            groupBy.Append(" CTe.CON_CODIGO, ");
                    }
                    break;

                case "PesoPedido":
                    if (!select.Contains("PesoPedido"))
                    {
                        select.Append(@"(
                                            SELECT SUM(_cargaPedido.PED_PESO)
                                            FROM
                                                T_CARGA_CTE CargaCTe 
                                                JOIN T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE _cargaPedidoCte ON _cargaPedidoCte.CCT_CODIGO = CargaCTe.CCT_CODIGO 
                                                JOIN T_PEDIDO_XML_NOTA_FISCAL _pedidoXmlNotaFiscal ON _pedidoXmlNotaFiscal.PNF_CODIGO = _cargaPedidoCte.PNF_CODIGO 
                                                JOIN T_CARGA_PEDIDO _cargaPedido ON _cargaPedido.CPE_CODIGO = _pedidoXmlNotaFiscal.CPE_CODIGO 
                                            WHERE 
                                                CargaCTe.CON_CODIGO = CTe.CON_CODIGO
                                        ) PesoPedido, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "DataOperacaoNavioFormatada":
                    if (!select.Contains("DataOperacaoNavio"))
                    {
                        select.Append("PedidoViagemNavioSchedule.PVS_DATA_PREVISAO_SAIDA_NAVIO DataOperacaoNavio, ");
                        groupBy.Append("PedidoViagemNavioSchedule.PVS_DATA_PREVISAO_SAIDA_NAVIO, ");

                        SetarJoinsPedidoViagemNavioSchedule(joins);
                    }
                    break;
                case "BookingReferente":
                    if (!select.Contains("BookingReferente"))
                    {
                        select.Append(@"substring((
                                                    select ', ' + DISTINCT_PEDIDO.PED_BOOKING_REFERENCE
                                                    from (
                                                        select distinct Pedido.PED_BOOKING_REFERENCE, Pedido.PED_DATA_ORDER
                                                        from T_CARGA_CTE CargaCTe
                                                        inner join T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE CargaPedidoXMLNotaFiscalCTe 
                                                            on CargaPedidoXMLNotaFiscalCTe.CCT_CODIGO = CargaCTe.CCT_CODIGO
                                                        inner join T_PEDIDO_XML_NOTA_FISCAL PedidoXMLNotaFiscal 
                                                            on PedidoXMLNotaFiscal.PNF_CODIGO = CargaPedidoXMLNotaFiscalCTe.PNF_CODIGO
                                                        inner join T_CARGA_PEDIDO CargaPedido 
                                                            on CargaPedido.CPE_CODIGO = PedidoXMLNotaFiscal.CPE_CODIGO
                                                        inner join T_PEDIDO Pedido 
                                                            on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
                                                        where CargaCTe.CON_CODIGO = CTe.CON_CODIGO
                                                    ) as DISTINCT_PEDIDO
                                                    order by DISTINCT_PEDIDO.PED_DATA_ORDER
                                                    for xml path('')
                                                ), 3, 200) as BookingReferente, ");


                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");

                    }
                    break;
                case "SerieNota":
                    if (!select.Contains("SerieNota"))
                    {
                        select.Append(@"substring((
                                            select distinct ' / ' + XmlNotaFiscal.NF_SERIE
                                            FROM T_CARGA_PEDIDO CargaPedido
                                                JOIN T_PEDIDO_XML_NOTA_FISCAL PedidoXmlNotaFiscal ON PedidoXmlNotaFiscal.CPE_CODIGO = CargaPedido.CPE_CODIGO
                                                JOIN T_XML_NOTA_FISCAL XmlNotaFiscal ON XmlNotaFiscal.NFX_CODIGO = PedidoXmlNotaFiscal.NFX_CODIGO
                                            WHERE CargaPedido.CAR_CODIGO = CargaCTe.CAR_CODIGO
                                        for xml path('')), 3, 100000) SerieNota, ");

                        if (!groupBy.Contains("CargaCTe.CAR_CODIGO"))
                            groupBy.Append("CargaCTe.CAR_CODIGO, ");

                        SetarJoinsCargaCte(joins);
                    }
                    break;
                case "DocFaturaSapFormatada":
                    if (!select.Contains("DocFaturaSap"))
                    {
                        select.Append("(SELECT TOP 1 SUBSTRING(CIA.CCA_MENSAGEM, 24, 38)\r\n FROM T_CARGA_CTE_INTEGRACAO_ARQUIVO CIA\r\n  INNER JOIN T_CARGA_CARGA_INTEGRACAO_ARQUIVO_ARQUIVO CIAA \r\n  ON CIAA.CCA_CODIGO = CIA.CCA_CODIGO\r\n   INNER JOIN T_CARGA_CARGA_INTEGRACAO CCI\r\n   ON CCI.CAI_CODIGO = CIAA.CAI_CODIGO\r\n   INNER JOIN T_CARGA_CTE cargaCTe\r\n    ON cargaCTe.CAR_CODIGO = CCI.CAR_CODIGO\r\n   WHERE  CIA.CCA_MENSAGEM LIKE 'Consulta CTe '+CAST(CTe.CON_NUM AS VARCHAR(15))+'  Doc.Fat %'\r\n AND CIA.CCA_MENSAGEM NOT LIKE '%Nenhum registro encont%'\r\n  AND CIA.CCA_MENSAGEM NOT LIKE '%O doc.vendas%'\r\n AND cargaCTe.CON_CODIGO = CTe.CON_CODIGO) AS DocFaturaSap, ");

                        if (!groupBy.Contains("CTe.CON_NUM"))
                            groupBy.Append("CTe.CON_NUM, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");

                    }
                    break;
                case "CodigoCentroDeCustoEmissor":
                    if (!select.Contains("CodigoCentroDeCustoEmissor"))
                    {
                        select.Append("(SELECT TOP 1 EMP_CODIGO_CENTRO_CUSTO AS CodigoCentroDeCustoEmissor FROM T_EMPRESA E INNER JOIN T_CARGA C ON C.EMP_CODIGO = E.EMP_CODIGO WHERE C.CAR_CODIGO =  CargaCTe.CAR_CODIGO) AS CodigoCentroDeCustoEmissor, ");

                        if (!groupBy.Contains("CargaCTe.CAR_CODIGO"))
                            groupBy.Append("CargaCTe.CAR_CODIGO, ");

                        SetarJoinsCargaCte(joins);

                    }
                    break;
                case "CodigoDocumentacaoNavio":
                    if (!select.Contains("CodigoDocumentacaoNavio,"))
                    {
                        select.Append("Navio.NAV_CODIGO_DOCUMENTO CodigoDocumentacaoNavio, ");

                        if (!groupBy.Contains("Navio.NAV_CODIGO_DOCUMENTO"))
                            groupBy.Append("Navio.NAV_CODIGO_DOCUMENTO, ");

                        SetarJoinsNavio(joins);
                    }
                    break;
                case "TipoOSConvertidoDescricao":
                    if (!select.Contains("TipoOSConvertido"))
                    {
                        // Este campo representa um Enum. Quando foi criado, não havia a opção nenhum/todos. 
                        // Como o campo é nullable, se estiver null, é convertido para 0. 
                        // No entanto, 0 não representa a opção nenhum/todos, mas sim a opção 2, 
                        // que foi criada posteriormente.
                        select.Append(@"( SELECT STRING_AGG(COALESCE(PED_TIPO_OS_CONVERTIDO, 4), ', ') 
                                      FROM 
                                        (
                                          SELECT DISTINCT Pedido.PED_TIPO_OS_CONVERTIDO
                                          FROM 
                                            T_CARGA_PEDIDO CargaPedido 
                                            LEFT JOIN T_PEDIDO Pedido ON Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
                                          WHERE 
                                            CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO
                                        ) AS DescricoesUnicas
                                    ) AS TipoOSConvertido, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;
                case "TipoOSDescricao":
                    if (!select.Contains("TipoOS, "))
                    {
                        // Este campo representa um Enum. Quando foi criado, não havia a opção nenhum/todos. 
                        // Como o campo é nullable, se estiver null, é convertido para 0. 
                        // No entanto, 0 não representa a opção nenhum/todos, mas sim a opção 4, 
                        // que foi criada posteriormente.
                        select.Append(@"( SELECT STRING_AGG(COALESCE(PED_TIPO_OS, 4), ', ') 
                                      FROM 
                                        (
                                          SELECT DISTINCT Pedido.PED_TIPO_OS
                                          FROM 
                                            T_CARGA_PEDIDO CargaPedido 
                                            LEFT JOIN T_PEDIDO Pedido ON Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
                                          WHERE 
                                            CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO
                                        ) AS DescricoesUnicas
                                    ) AS TipoOS, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;
                case "ProvedorOS":
                    if (!select.Contains("ProvedorOS"))
                    {
                        select.Append("Provedor.CLI_NOME ProvedorOS, ");

                        if (!groupBy.Contains("Provedor.CLI_NOME"))
                            groupBy.Append("Provedor.CLI_NOME, ");

                        SetarJoinsProvedor(joins);
                    }
                    break;
                case "CentroDeCustoViagemDescricao":
                    if (!select.Contains("CentroDeCustoViagemDescricao, "))
                    {
                        select.Append(@"( SELECT STRING_AGG(CCV_DESCRICAO, ', ') 
                                      FROM 
                                        (
                                          SELECT DISTINCT CentroDeCustoViagem.CCV_DESCRICAO
                                          FROM 
                                            T_CARGA_PEDIDO CargaPedido 
                                            LEFT JOIN T_PEDIDO Pedido ON CargaPedido.PED_CODIGO = Pedido.PED_CODIGO 
                                            LEFT JOIN T_CENTRO_CUSTO_VIAGEM CentroDeCustoViagem ON CentroDeCustoViagem.CCV_CODIGO = Pedido.CCV_CODIGO 
                                          WHERE 
                                            CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO
                                        ) AS DescricoesUnicas
                                    ) AS CentroDeCustoViagemDescricao, "
                        );

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;
                case "CnpjMdfeFormatada":
                    if (!select.Contains("TransportadorMdfe.CnpjMdfe"))
                    {
                        select.Append("TransportadorMdfe.CnpjMdfe, ");
                        groupBy.Append("TransportadorMdfe.CnpjMdfe, ");

                        SetarJoinsTransportadorMdfe(joins);
                    }
                    break;
                case "MicDTA":
                    if (!select.Contains("MicDTA"))
                    {
                        select.Append("MicDTA.CMD_NUMERO AS MicDTA, ");
                        groupBy.Append("MicDTA.CMD_NUMERO, ");

                        SetarJoinsMicDTA(joins);
                    }
                    break;
                case "NumeroCRT":
                    if (!select.Contains("NumeroCRT"))
                    {
                        select.Append("CTe.CON_NUMERO_CRT as NumeroCRT, ");
                        groupBy.Append("CTe.CON_NUMERO_CRT, ");
                    }
                    break;
                default:
                    if (!somenteContarNumeroRegistros && propriedade.Contains("ValorComponente"))
                    {
                        select.Append("(select SUM(CargaCTeComponenteFrete.CCC_VALOR_COMPONENTE) from T_CARGA_CTE_COMPONENTES_FRETE CargaCTeComponenteFrete inner join T_CARGA_CTE CargaCTe on CargaCTe.CCT_CODIGO = CargaCTeComponenteFrete.CCT_CODIGO where CargaCTe.CON_CODIGO = CTe.CON_CODIGO and CargaCTeComponenteFrete.CFR_CODIGO = " + codigoDinamico + ") " + propriedade + ", "); 

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }

                    if (!somenteContarNumeroRegistros && propriedade.Contains("LayoutArquivo"))
                    {
                        select.Append(
                            "SUBSTRING((" +
                            "    SELECT ', ' + ArquivoEDICTe.AEC_NOME_ARQUIVO " +
                            "      FROM T_ARQUIVO_EDI_CTE ArquivoEDICTe " +
                            "     WHERE ArquivoEDICTe.CON_CODIGO = CTe.CON_CODIGO AND ArquivoEDICTe.LAY_CODIGO = " + codigoDinamico + " " +
                            "       FOR XML PATH(''))" +
                            ", 3, 1000) " +
                            propriedade + ", "
                        );

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }

                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaCteRelatorio filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string pattern = "yyyy-MM-dd";

            where.Append(" and (CTe.CON_DESABILITADO is null OR CTe.CON_DESABILITADO = 0)");

            if (filtrosPesquisa.dataInicialEntrega != DateTime.MinValue)
                where.Append("  and CAST(CTe.CON_DATA_ENTREGA AS DATE) >= '" + filtrosPesquisa.dataInicialEntrega.ToString(pattern) + "'");

            if (filtrosPesquisa.dataFinalEntrega != DateTime.MinValue)
                where.Append("  and CAST(CTe.CON_DATA_ENTREGA AS DATE) <= '" + filtrosPesquisa.dataFinalEntrega.ToString(pattern) + "'");

            if (filtrosPesquisa.possuiDataEntrega.HasValue)
                where.Append("  and CTe.CON_DATA_ENTREGA is " + (filtrosPesquisa.possuiDataEntrega.Value ? "not" : "") + " null");

            if (filtrosPesquisa.PermiteGerarFaturamento.HasValue)
                where.Append("  and ModeloDocumento.MOD_NAO_GERAR_FATURAMENTO = " + (filtrosPesquisa.PermiteGerarFaturamento.Value ? "1" : "0"));

            if (filtrosPesquisa.numeroInicial > 0)
                where.Append("  and CTe.CON_NUM >= " + filtrosPesquisa.numeroInicial.ToString());

            if (filtrosPesquisa.numeroFinal > 0)
                where.Append("  and CTe.CON_NUM <= " + filtrosPesquisa.numeroFinal.ToString());

            if (filtrosPesquisa.dataInicialEmissao != DateTime.MinValue)
                where.Append("  and CTe.CON_DATAHORAEMISSAO >= '" + filtrosPesquisa.dataInicialEmissao.ToString(pattern) + "'");

            if (filtrosPesquisa.dataFinalEmissao != DateTime.MinValue)
                where.Append("  and CTe.CON_DATAHORAEMISSAO < '" + filtrosPesquisa.dataFinalEmissao.AddDays(1).ToString(pattern) + "'");

            if (filtrosPesquisa.dataInicialAutorizacao != DateTime.MinValue)
                where.Append("  and CAST(CTe.CON_DATA_AUTORIZACAO AS DATE) >= '" + filtrosPesquisa.dataInicialAutorizacao.ToString(pattern) + "'");

            if (filtrosPesquisa.dataFinalAutorizacao != DateTime.MinValue)
                where.Append("  and CAST(CTe.CON_DATA_AUTORIZACAO AS DATE) <= '" + filtrosPesquisa.dataFinalAutorizacao.ToString(pattern) + "'");

            if (filtrosPesquisa.dataInicialCancelamento != DateTime.MinValue)
                where.Append("  and CAST(CTe.CON_DATA_CANCELAMENTO AS DATE) >= '" + filtrosPesquisa.dataInicialCancelamento.ToString(pattern) + "'");

            if (filtrosPesquisa.dataFinalCancelamento != DateTime.MinValue)
                where.Append("  and CAST(CTe.CON_DATA_CANCELAMENTO AS DATE) <= '" + filtrosPesquisa.dataFinalCancelamento.ToString(pattern) + "'");

            if (filtrosPesquisa.dataInicialAnulacao != DateTime.MinValue)
                where.Append("  and CAST(CTe.CON_DATA_ANULACAO AS DATE) >= '" + filtrosPesquisa.dataInicialAnulacao.ToString(pattern) + "'");

            if (filtrosPesquisa.dataFinalAnulacao != DateTime.MinValue)
                where.Append("  and CAST(CTe.CON_DATA_ANULACAO AS DATE) <= '" + filtrosPesquisa.dataFinalAnulacao.ToString(pattern) + "'");

            if (filtrosPesquisa.CodigosTransportador?.Count > 0)
                where.Append($"  and CTe.EMP_CODIGO IN ({string.Join(", ", filtrosPesquisa.CodigosTransportador)})");

            if (filtrosPesquisa.modeloDocumento != null && filtrosPesquisa.modeloDocumento.Count() > 0)
                where.Append($" and CTe.CON_MODELODOC IN ({string.Join(",", filtrosPesquisa.modeloDocumento)})");

            if (filtrosPesquisa.tipoDocumentoCreditoDebito != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoCreditoDebito.Todos)
            {
                SetarJoinsModeloDocumento(joins);

                if (filtrosPesquisa.tipoDocumentoCreditoDebito == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoCreditoDebito.Debito)
                    where.Append("  and ModeloDocumento.MOD_TIPO_DOCUMENTO_CREDITO_DEBITO = 1");
                else
                    where.Append("  and ModeloDocumento.MOD_TIPO_DOCUMENTO_CREDITO_DEBITO != 1");
            }

            if (filtrosPesquisa.cpfCnpjTerceiro > 0)
                where.Append("  and (exists (SELECT CV.CON_CODIGO FROM T_CTE_VEICULO CV JOIN T_CTE_VEICULO_PROPRIETARIO P ON P.PVE_CODIGO = CV.PVE_CODIGO WHERE CTe.CON_CODIGO = CV.CON_CODIGO AND convert(float, P.PVE_CPF_CNPJ) = " + filtrosPesquisa.cpfCnpjTerceiro.ToString() + ")" + // SQL-INJECTION-SAFE
                             "    or exists (SELECT DISTINCT CargaCTe.CON_CODIGO FROM T_CARGA_CTE CargaCTe" +
                             "                           JOIN T_CARGA Carga on CargaCTe.CAR_CODIGO = Carga.CAR_CODIGO" +
                             "                           JOIN T_VEICULO Veiculo on Carga.CAR_VEICULO = Veiculo.VEI_CODIGO" +
                             "                          WHERE CargaCTe.CON_CODIGO = CTe.CON_CODIGO AND VEI_PROPRIETARIO = " + filtrosPesquisa.cpfCnpjTerceiro.ToString() + "))");

            if (filtrosPesquisa.codigoCarga?.Count > 0)
                where.Append("  and exists (select CargaCTe.CON_CODIGO from T_CARGA_CTE CargaCTe where CTe.CON_CODIGO = CargaCTe.CON_CODIGO " +
                    $"AND (CargaCTe.CAR_CODIGO_ORIGEM in " +
                    $"({string.Join(", ", filtrosPesquisa.codigoCarga)})" +
                    " or CargaCTe.CAR_CODIGO in " +
                    $"({string.Join(", ", filtrosPesquisa.codigoCarga)})))");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.preCarga))
                where.Append("  and exists (select CargaCTe.CON_CODIGO from T_CARGA_CTE CargaCTe inner join T_PRE_CARGA PreCarga on PreCarga.CAR_CODIGO = CargaCTe.CAR_CODIGO_ORIGEM where CTe.CON_CODIGO = CargaCTe.CON_CODIGO AND PreCarga.PCA_NUMERO_CARGA = '" + filtrosPesquisa.preCarga + "')"); // SQL-INJECTION-SAFE

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.pedido))
            {
                where.Append(@" and exists (select CargaCTe.CON_CODIGO from T_CARGA_CTE CargaCTe 
                            inner join T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE CargaPedidoXMLNotaFiscalCTe on CargaPedidoXMLNotaFiscalCTe.CCT_CODIGO = CargaCTe.CCT_CODIGO  
                            inner join T_PEDIDO_XML_NOTA_FISCAL PedidoXMLNotaFiscal on PedidoXMLNotaFiscal.PNF_CODIGO = CargaPedidoXMLNotaFiscalCTe.PNF_CODIGO
                            inner join T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = PedidoXMLNotaFiscal.CPE_CODIGO
                            inner join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
                            where CTe.CON_CODIGO = CargaCTe.CON_CODIGO AND Pedido.PED_NUMERO_PEDIDO_EMBARCADOR = '" + filtrosPesquisa.pedido + "')");
            }

            if (filtrosPesquisa.codigoContratoFrete > 0)
                where.Append("  and exists (select CargaCTe.CON_CODIGO from T_CARGA_CTE CargaCTe inner join T_CARGA Carga on CargaCTe.CAR_CODIGO_ORIGEM = Carga.CAR_CODIGO where CTe.CON_CODIGO = CargaCTe.CON_CODIGO AND Carga.CFT_CODIGO = " + filtrosPesquisa.codigoContratoFrete.ToString() + ")"); // SQL-INJECTION-SAFE

            if (filtrosPesquisa.codigosTipoOperacao?.Count > 0)
                where.Append($" and exists (select CargaCTe.CON_CODIGO from T_CARGA_CTE CargaCTe inner join T_CARGA Carga on CargaCTe.CAR_CODIGO_ORIGEM = Carga.CAR_CODIGO where CTe.CON_CODIGO = CargaCTe.CON_CODIGO AND (Carga.TOP_CODIGO in " +
                    $"({string.Join(", ", filtrosPesquisa.codigosTipoOperacao)}){(filtrosPesquisa.codigosTipoOperacao.Contains(-1) ? " or Carga.TOP_CODIGO is null" : "")}))");

            if (filtrosPesquisa.CodigosTipoOcorrencia.Count > 0)
                where.Append($" and exists (select ComplementoInfo.CON_CODIGO from T_CARGA_CTE_COMPLEMENTO_INFO ComplementoInfo left join T_CARGA_OCORRENCIA CargaOcorrencia on CargaOcorrencia.COC_CODIGO = ComplementoInfo.COC_CODIGO WHERE CTe.CON_CODIGO = ComplementoInfo.CON_CODIGO AND CargaOcorrencia.OCO_CODIGO in ({string.Join(",", filtrosPesquisa.CodigosTipoOcorrencia)} ))"); // SQL-INJECTION-SAFE

            if (filtrosPesquisa.pago.HasValue)
            {
                if (filtrosPesquisa.pago.Value)
                    where.Append("  and Fatura.FAT_SITUACAO = 2 ");
                else
                    where.Append("  and (Fatura.FAT_SITUACAO is null or Fatura.FAT_SITUACAO <> 2) ");

                SetarJoinsFatura(joins);
            }

            if (filtrosPesquisa.codigosFilial != null && filtrosPesquisa.codigosFilial.Count > 0 && filtrosPesquisa.codigosFilial.Any(codigo => codigo == -1))
            {
                SetarJoinsCarga(joins);

                where.Append($@" and (Carga.FIL_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosFiliais)}) OR EXISTS (   SELECT _cargaPedidoRecebedor.CAR_CODIGO 
                                                                                                                       FROM T_CARGA_PEDIDO _cargaPedidoRecebedor 
                                                                                                                       LEFT JOIN T_PEDIDO _pedido ON _pedido.PED_CODIGO = _cargaPedidoRecebedor.PED_CODIGO
                                                                                                                       WHERE Carga.CAR_CODIGO = _cargaPedidoRecebedor.CAR_CODIGO
                                                                                                                       AND _pedido.CLI_CODIGO_RECEBEDOR IN ({string.Join(",", filtrosPesquisa.CodigosRecebedores)})))");
            }
            else if (filtrosPesquisa.codigosFilial?.Count > 0)
                where.Append($" and exists (select CargaCTe.CON_CODIGO from T_CARGA_CTE CargaCTe inner join T_CARGA Carga on CargaCTe.CAR_CODIGO_ORIGEM = Carga.CAR_CODIGO where CTe.CON_CODIGO = CargaCTe.CON_CODIGO AND Carga.FIL_CODIGO in " +
                    $"({string.Join(",", filtrosPesquisa.codigosFilial)}))");

            if (filtrosPesquisa.codigosTipoCarga?.Count > 0)
                where.Append($" and exists (select CargaCTe.CON_CODIGO from T_CARGA_CTE CargaCTe inner join T_CARGA Carga on CargaCTe.CAR_CODIGO_ORIGEM = Carga.CAR_CODIGO where CTe.CON_CODIGO = CargaCTe.CON_CODIGO AND (Carga.TCG_CODIGO in ({string.Join(",", filtrosPesquisa.codigosTipoCarga)}){(filtrosPesquisa.codigosTipoCarga.Contains(-1) ? " or Carga.TCG_CODIGO is null)" : ")")})"); // SQL-INJECTION-SAFE

            if (filtrosPesquisa.codigoOrigem > 0)
                where.Append("  and CTe.CON_LOCINICIOPRESTACAO = " + filtrosPesquisa.codigoOrigem.ToString());

            if (filtrosPesquisa.codigoDestino > 0)
                where.Append("  and CTe.CON_LOCTERMINOPRESTACAO = " + filtrosPesquisa.codigoDestino.ToString());

            if (filtrosPesquisa.cpfCnpjRemetente > 0d)
            {
                where.Append("  and ClienteRemetente.CLI_CGCCPF = " + filtrosPesquisa.cpfCnpjRemetente.ToString("F0"));

                SetarJoinsRemetenteCliente(joins);
            }

            if (filtrosPesquisa.cpfCnpjDestinatarios != null && filtrosPesquisa.cpfCnpjDestinatarios.Count > 0)
            {
                where.Append("  and ClienteDestinatario.CLI_CGCCPF in (" + string.Join(",", filtrosPesquisa.cpfCnpjDestinatarios) + ')');

                SetarJoinsDestinatarioCliente(joins);
            }

            if (filtrosPesquisa.CpfCnpjTomadores.Count > 0)
            {
                where.Append("  and ClienteTomador.CLI_CGCCPF in (" + string.Join(",", filtrosPesquisa.CpfCnpjTomadores) + ')');

                SetarJoinsTomadorCliente(joins);
            }

            if (filtrosPesquisa.statusCTe != null && filtrosPesquisa.statusCTe.Count > 0)
            {
                if (filtrosPesquisa.statusCTe.Count == 1)
                {
                    if (filtrosPesquisa.statusCTe[0] == "G")
                        where.Append("  and CTe.CON_STATUS = 'Z' AND CTe.CON_ANULADO_GERENCIALMENTE = 1 ");
                    else
                        where.Append("  and CTe.CON_STATUS = '" + filtrosPesquisa.statusCTe[0] + "'");
                }
                else
                {
                    if (filtrosPesquisa.statusCTe.Contains("G"))
                        where.Append("  and (CTe.CON_STATUS in ('" + string.Join("', '", filtrosPesquisa.statusCTe) + "') OR CTe.CON_ANULADO_GERENCIALMENTE = 1) ");
                    else
                        where.Append("  and (CTe.CON_STATUS in ('" + string.Join("', '", filtrosPesquisa.statusCTe) + "') AND (CTe.CON_ANULADO_GERENCIALMENTE IS NULL OR CTe.CON_ANULADO_GERENCIALMENTE = 0)) ");
                }
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.nfe))
            {
                where.Append("  and exists (SELECT _notafiscal.CON_CODIGO FROM T_CTE_DOCS _notafiscal WHERE CTe.CON_CODIGO = _notafiscal.CON_CODIGO AND _notafiscal.NFC_NUMERO LIKE :NOTAFISCAL_NFC_NUMERO) "); 
                parametros.Add(new ParametroSQL("NOTAFISCAL_NFC_NUMERO", $"%{filtrosPesquisa.nfe}%"));
            }

            if (filtrosPesquisa.ctesNaoExistentesEmMinutas)
                where.Append("  and not exists (select DocumentoAvon.CON_CODIGO from T_AVON_MANIFESTO_DOCUMENTO DocumentoAvon where DocumentoAvon.CON_CODIGO is not null union all select DocumentoNatura.CON_CODIGO from T_NATURA_DOCUMENTO_TRANSPORTE_NOTA_FISCAL DocumentoNatura where CTe.CON_CODIGO = DocumentoAvon.CON_CODIGO AND DocumentoNatura.CON_CODIGO is not null)");

            if (filtrosPesquisa.ctesNaoExistentesEmFaturas)
                where.Append("  and not exists (select DocumentoNatura.CON_CODIGO from T_NATURA_DOCUMENTO_TRANSPORTE_NOTA_FISCAL DocumentoNatura inner join T_NATURA_FATURA_ITEM ItemFaturaNatura on ItemFaturaNatura.NDT_CODIGO = DocumentoNatura.NDT_CODIGO where DocumentoNatura.CON_CODIGO is not null union all select DocumentoAvon.CON_CODIGO from t_avon_manifesto_documento DocumentoAvon inner join T_AVON_FATURA_MANIFESTO ItemFaturaAvon on DocumentoAvon.MAV_CODIGO = ItemFaturaAvon.MAV_CODIGO where CTe.CON_CODIGO = DocumentoNatura.CON_CODIGO AND DocumentoAvon.CON_CODIGO is not null)");

            if (filtrosPesquisa.gruposPessoas != null && filtrosPesquisa.gruposPessoas.Count > 0)
            {
                where.Append("  and TomadorPagadorCTe.GRP_CODIGO in (" + string.Join(",", filtrosPesquisa.gruposPessoas) + ")");

                SetarJoinsTomador(joins);
            }

            if (filtrosPesquisa.gruposPessoasDiferente != null && filtrosPesquisa.gruposPessoasDiferente.Count > 0)
            {
                where.Append("  and (TomadorPagadorCTe.GRP_CODIGO not in (" + string.Join(",", filtrosPesquisa.gruposPessoasDiferente) + ") or TomadorPagadorCTe.GRP_CODIGO IS NULL)");

                SetarJoinsTomador(joins);
            }

            if (filtrosPesquisa.GruposPessoasRemetente != null && filtrosPesquisa.GruposPessoasRemetente.Count > 0)
            {
                where.Append("  and RemetenteCTe.GRP_CODIGO in (" + string.Join(",", filtrosPesquisa.GruposPessoasRemetente) + ")");

                SetarJoinsRemetente(joins);
            }

            if (filtrosPesquisa.tiposServicos != null && filtrosPesquisa.tiposServicos.Count > 0)
            {
                if (filtrosPesquisa.statusCTe.Count == 1)
                    where.Append("  and CTe.CON_TIPO_SERVICO = '" + filtrosPesquisa.tiposServicos[0] + "'");
                else
                    where.Append("  and CTe.CON_TIPO_SERVICO in ('" + string.Join("', '", filtrosPesquisa.tiposServicos) + "')");
            }

            if (filtrosPesquisa.tiposTomadores != null && filtrosPesquisa.tiposTomadores.Count > 0)
            {
                if (filtrosPesquisa.statusCTe.Count == 1)
                    where.Append("  and CTe.CON_TOMADOR = '" + filtrosPesquisa.tiposTomadores[0] + "'");
                else
                    where.Append("  and CTe.CON_TOMADOR in ('" + string.Join("', '", filtrosPesquisa.tiposTomadores) + "')");
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.estadoOrigem) && filtrosPesquisa.estadoOrigem != "0")
            {
                where.Append("  and InicioPrestacaoCTe.UF_SIGLA = '" + filtrosPesquisa.estadoOrigem + "'");

                SetarJoinsLocalidadeInicioPrestacao(joins);
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.estadoDestino) && filtrosPesquisa.estadoDestino != "0")
            {
                where.Append("  and FimPrestacaoCTe.UF_SIGLA = '" + filtrosPesquisa.estadoDestino + "'");

                SetarJoinsLocalidadeFimPrestacao(joins);
            }

            if (filtrosPesquisa.transportadorTerceiro != null)
            {
                where.Append("  and ((TransportadorCTe.EMP_CNPJ = '" + filtrosPesquisa.transportadorTerceiro.CPF_CNPJ.ToString("00000000000000") + "' or VeiculoVeiculoCTe.VEI_PROPRIETARIO  = '" + filtrosPesquisa.transportadorTerceiro.CPF_CNPJ.ToString() + "')" +
                             "    or exists (SELECT DISTINCT CargaCTe.CON_CODIGO FROM T_CARGA_CTE CargaCTe" +
                             "                           JOIN T_CARGA Carga on CargaCTe.CAR_CODIGO = Carga.CAR_CODIGO" +
                             "                           JOIN T_VEICULO Veiculo on Carga.CAR_VEICULO = Veiculo.VEI_CODIGO" +
                             "                          WHERE CTe.CON_CODIGO = CargaCTe.CON_CODIGO AND VEI_PROPRIETARIO = " + filtrosPesquisa.transportadorTerceiro.CPF_CNPJ.ToString() + "))"
                    );

                SetarJoinsTransportador(joins);

                if (!joins.Contains(" Veiculo "))
                    joins.Append(" left outer join T_CTE_VEICULO Veiculo on Veiculo.CVE_CODIGO = (SELECT TOP 1 CVE_CODIGO FROM T_CTE_VEICULO WHERE CON_CODIGO = CTe.CON_CODIGO) ");


                if (!joins.Contains(" VeiculoVeiculoCTe "))
                {
                    joins.Append(@"OUTER APPLY
                               (SELECT TOP 1 VEI_PLACA, VEI_PROPRIETARIO
                                 FROM T_VEICULO
                                JOIN T_EMPRESA ON T_VEICULO.EMP_CODIGO = T_EMPRESA.EMP_CODIGO
                                WHERE T_VEICULO.VEI_PLACA = Veiculo.CVE_PLACA and T_VEICULO.VEI_ATIVO = 1
                                order by T_VEICULO.VEI_PROPRIETARIO DESC
                              )VeiculoVeiculoCTe");
                }
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.tipoPropriedadeVeiculo))
            {
                if (filtrosPesquisa.tipoPropriedadeVeiculo == "O") //não se encontra nem em próprio e nem em terceiro "Outros"
                    where.Append("  and not exists (select vei_codigo from t_cte_veiculo vei where vei.CON_CODIGO = CTe.CON_CODIGO and vei.CVE_TIPO_VEICULO = 0 and vei.CVE_TIPO_PROPRIEDADE in ('P', 'T'))");
                else
                {
                    where.Append("  and exists (select vei_codigo from t_cte_veiculo vei where vei.CON_CODIGO = CTe.CON_CODIGO and vei.CVE_TIPO_VEICULO = 0 and vei.CVE_TIPO_PROPRIEDADE = :VEI_CVE_TIPO_PROPRIEDADE)"); 
                    parametros.Add(new ParametroSQL("VEI_CVE_TIPO_PROPRIEDADE", filtrosPesquisa.tipoPropriedadeVeiculo));
                }
            }

            if (filtrosPesquisa.placasVeiculos?.Count > 0)
            {
                where.Append(" and exists (select vei_codigo from t_cte_veiculo vei where vei.CON_CODIGO = CTe.CON_CODIGO and vei.CVE_PLACA ");

                if (filtrosPesquisa.placasVeiculos.Count == 1)
                {
                    where.Append($"= :VEI_CVE_PLACA)");
                    //where.Append($"= '{filtrosPesquisa.placasVeiculos[0]}')");
                    parametros.Add(new ParametroSQL("VEI_CVE_PLACA", filtrosPesquisa.placasVeiculos[0]));
                }
                else
                {
                    where.Append($"in (:VEI_CVE_PLACA))");
                    //where.Append($"in ({string.Join(",", filtrosPesquisa.placasVeiculos.Select(o => $"'{o}'"))}))");
                    parametros.Add(new ParametroSQL("VEI_CVE_PLACA", filtrosPesquisa.placasVeiculos));
                }
            }

            if (filtrosPesquisa.serie > 0)
            {
                where.Append("  and Serie.ESE_NUMERO = " + filtrosPesquisa.serie.ToString());

                SetarJoinsSerie(joins);
            }

            if (filtrosPesquisa.CodigosCentroResultado?.Count > 0)
            {
                where.Append($" AND (CTe.CRE_CODIGO_FATURAMENTO in ({string.Join(", ", filtrosPesquisa.CodigosCentroResultado)}) OR exists(select tipoOperacaoPagamentos.CTP_CODIGO from T_CONFIGURACAO_TIPO_OPERACAO_PAGAMENTOS tipoOperacaoPagamentos inner join T_TIPO_OPERACAO tipoOperacao on tipoOperacao.CTP_CODIGO = tipoOperacaoPagamentos.CTP_CODIGO inner join T_CARGA carga on carga.TOP_CODIGO = tipoOperacao.TOP_CODIGO inner join T_CARGA_CTE cargaCTe on cargaCTe.CAR_CODIGO = carga.CAR_CODIGO where cargaCTe.CON_CODIGO = CTe.CON_CODIGO and tipoOperacaoPagamentos.CRE_CODIGO in " +
                    $"({string.Join(", ", filtrosPesquisa.CodigosCentroResultado)})");

                if (filtrosPesquisa.codigosTipoOperacao?.Count > 0)
                    where.Append($" and carga.TOP_CODIGO in ({string.Join(", ", filtrosPesquisa.codigosTipoOperacao)})");

                where.Append("))");
            }

            if (filtrosPesquisa.FuncionarioResponsavel?.Count > 0)
                where.Append($"  and exists (select vei_codigo from t_cte_veiculo vei where vei.CON_CODIGO = CTe.CON_CODIGO and vei.FUN_CODIGO_RESPONSAVEL in ({string.Join(", ", filtrosPesquisa.FuncionarioResponsavel)})) "); // SQL-INJECTION-SAFE

            if (filtrosPesquisa.cteVinculadoACarga.HasValue && filtrosPesquisa.transportadorTerceiro == null)
                where.Append("  and " + (filtrosPesquisa.cteVinculadoACarga.Value ? "exists" : "not exists") + " (select _cargaCTe.CON_CODIGO from T_CARGA_CTE _cargaCTe WHERE CTe.CON_CODIGO = _cargaCTe.CON_CODIGO) ");

            if (filtrosPesquisa.cargaEmissaoFinalizada.HasValue)
            {
                where.Append("  and exists (select CargaCTe.CON_CODIGO from T_CARGA_CTE CargaCTe inner join T_CARGA Carga on Carga.CAR_CODIGO = CargaCTe.CAR_CODIGO_ORIGEM where CTe.CON_CODIGO = CargaCTe.CON_CODIGO AND Carga.CAR_DATA_FINALIZACAO_EMISSAO ");

                if (filtrosPesquisa.cargaEmissaoFinalizada.Value)
                    where.Append("  is not null)");
                else
                    where.Append("  is null)");
            }

            if (filtrosPesquisa.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
            {
                where.Append("  and not exists (select conhecimen14_.CON_CODIGO from T_CARGA_CTE_COMPLEMENTO_INFO cargacteco11_ inner join T_CARGA_OCORRENCIA cargaocorr12_ on cargacteco11_.COC_CODIGO=cargaocorr12_.COC_CODIGO inner join T_OCORRENCIA tipodeocor13_  on cargaocorr12_.OCO_CODIGO=tipodeocor13_.OCO_CODIGO  left outer join T_CTE conhecimen14_ on cargacteco11_.CON_CODIGO=conhecimen14_.CON_CODIGO where CTe.CON_CODIGO = conhecimen14_.CON_CODIGO AND tipodeocor13_.OCO_BLOQUEAR_VISUALIZACAO_PORTAL_TRANSPORTADOR=1 AND cargacteco11_.CON_CODIGO IS NOT NULL) ");
            }

            if (filtrosPesquisa.dataInicialImportacao != DateTime.MinValue || filtrosPesquisa.dataFinalImportacao != DateTime.MinValue)
            {
                if (filtrosPesquisa.dataInicialImportacao != DateTime.MinValue)
                    where.Append("  and CAST(IntegracaoCTeRecebimento.ICR_DATA AS DATE) >= '" + filtrosPesquisa.dataInicialImportacao.ToString(pattern) + "'");

                if (filtrosPesquisa.dataFinalImportacao != DateTime.MinValue)
                    where.Append("  and CAST(IntegracaoCTeRecebimento.ICR_DATA AS DATE) <= '" + filtrosPesquisa.dataFinalImportacao.ToString(pattern) + "'");

                SetarJoinsIntegracaoRecebimento(joins);
            }

            if (filtrosPesquisa.dataFinalFatura != DateTime.MinValue || filtrosPesquisa.dataInicialFatura != DateTime.MinValue)
            {
                if (filtrosPesquisa.dataInicialFatura != DateTime.MinValue)
                    where.Append("  and CAST(Fatura.FAT_DATA_FATURA AS DATE) >= '" + filtrosPesquisa.dataInicialFatura.ToString(pattern) + "'");

                if (filtrosPesquisa.dataFinalFatura != DateTime.MinValue)
                    where.Append("  and CAST(Fatura.FAT_DATA_FATURA AS DATE) <= '" + filtrosPesquisa.dataFinalFatura.ToString(pattern) + "'");

                SetarJoinsFatura(joins);
            }

            if (filtrosPesquisa.situacaoFatura.HasValue)
            {
                where.Append("  and Fatura.FAT_SITUACAO = " + filtrosPesquisa.situacaoFatura.Value.ToString("D"));

                SetarJoinsFatura(joins);
            }

            if (filtrosPesquisa.faturado.HasValue)
            {
                where.Append($" and (DocumentoFaturamentoCTe.DFA_SITUACAO IS NULL OR DocumentoFaturamentoCTe.DFA_SITUACAO NOT IN (2, 3)) {(filtrosPesquisa.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? "" : "and DocumentoFaturamentoCTe.DFA_SITUACAO = 5")}");

                if (filtrosPesquisa.faturado.Value)
                    where.Append(@" and ((DocumentoFaturamentoCTe.DFA_CODIGO is null and DocumentoFaturamentoCarga.DFA_CODIGO is null and CTe.FAT_CODIGO is not null) or 
                                     (DocumentoFaturamentoCTe.DFA_CODIGO is not null and DocumentoFaturamentoCTe.DFA_VALOR_A_FATURAR <= 0) or 
                                     (DocumentoFaturamentoCTe.DFA_CODIGO is null and DocumentoFaturamentoCarga.DFA_CODIGO is not null and DocumentoFaturamentoCarga.DFA_VALOR_A_FATURAR <= 0))");

                else
                    where.Append(@" and ((DocumentoFaturamentoCTe.DFA_CODIGO is null and DocumentoFaturamentoCarga.DFA_CODIGO is null and CTe.FAT_CODIGO is null) or 
                                     (DocumentoFaturamentoCTe.DFA_CODIGO is not null and DocumentoFaturamentoCTe.DFA_VALOR_A_FATURAR > 0) or
                                     (DocumentoFaturamentoCTe.DFA_CODIGO is null and DocumentoFaturamentoCarga.DFA_CODIGO is not null and DocumentoFaturamentoCarga.DFA_VALOR_A_FATURAR > 0))");

                SetarJoinsDocumentoFaturamento(joins); 
                SetarJoinsDocumentoFaturamentoCarga(joins);
            }

            if (filtrosPesquisa.codigoCFOP > 0)
            {
                where.Append("  and CFOP.CFO_CODIGO = " + filtrosPesquisa.codigoCFOP.ToString());

                SetarJoinsCfop(joins);
            }

            if (filtrosPesquisa.possuiNFSManual.HasValue)
                where.Append("  and " + (!filtrosPesquisa.possuiNFSManual.Value ? "not" : string.Empty) + " exists (select CargaDocumentoEmissaoNFSManual.NEM_CODIGO from T_CARGA_NFE_PARA_EMISSAO_NFS_MANUAL CargaDocumentoEmissaoNFSManual INNER JOIN T_CARGA_CTE CargaCTe ON CargaCTe.CCT_CODIGO = CargaDocumentoEmissaoNFSManual.CCT_CODIGO WHERE CargaCTe.CON_CODIGO = CTe.CON_CODIGO and CargaDocumentoEmissaoNFSManual.LNM_CODIGO is not null) ");

            if (filtrosPesquisa.CST?.Count > 0)
            {
                string tipos = string.Join(", ", from tipo in filtrosPesquisa.CST select tipo.ObterCSTParaRelatorio());
                where.Append($" and CTe.CON_CST in ({tipos})");
            }

            if (filtrosPesquisa.tiposCTe?.Count > 0)
                where.Append($" and CTe.CON_TIPO_CTE in ({string.Join(", ", filtrosPesquisa.tiposCTe.Select(o => o.ToString("D")))})");

            if (filtrosPesquisa.SegmentoVeiculo?.Count > 0)
                where.Append($" and exists (select VeiculoCTe.CON_CODIGO from T_CTE_VEICULO VeiculoCTe inner join T_VEICULO Veiculo on Veiculo.VEI_CODIGO = VeiculoCTe.VEI_CODIGO inner join T_VEICULO_SEGMENTO SegmentoVeiculo on SegmentoVeiculo.VSE_CODIGO = Veiculo.VSE_CODIGO where VeiculoCTe.CON_CODIGO = CTe.CON_CODIGO and SegmentoVeiculo.VSE_CODIGO in ({string.Join(",", filtrosPesquisa.SegmentoVeiculo)})) "); // SQL-INJECTION-SAFE

            if (filtrosPesquisa.TipoProposta.Count > 0)
                where.Append($@" and exists (select CargaCTe.CON_CODIGO from T_CARGA_CTE CargaCTe 
                                                    inner join T_CARGA Carga on Carga.CAR_CODIGO = CargaCTe.CAR_CODIGO 
                                                    inner join T_CARGA_PEDIDO CargaPedido on CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO 
                                                    where CTe.CON_CODIGO = CargaCTe.CON_CODIGO AND CargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL in ({string.Join(", ", filtrosPesquisa.TipoProposta.Select(o => o.ToString("D")))}))");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroBooking))
                where.Append($" and CTe.CON_NUMERO_BOOKING = '{filtrosPesquisa.NumeroBooking}'");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroOS))
                where.Append($" and CTe.CON_NUMERO_OS = '{filtrosPesquisa.NumeroOS}'");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroControle))
                where.Append($" and CTe.CON_NUMERO_CONTROLE = '{filtrosPesquisa.NumeroControle}'");

            if (filtrosPesquisa.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Todas)
                where.Append($@" and exists (select CargaCTe.CON_CODIGO from T_CARGA_CTE CargaCTe 
                                                    inner join T_CARGA Carga on Carga.CAR_CODIGO = CargaCTe.CAR_CODIGO 
                                                    where CTe.CON_CODIGO = CargaCTe.CON_CODIGO AND Carga.CAR_SITUACAO = ({filtrosPesquisa.SituacaoCarga.ToString("D")}))");

            if (filtrosPesquisa.SituacoesCargaMercante != null && filtrosPesquisa.SituacoesCargaMercante.Count > 0)
            {
                where.Append($" and exists (select CargaCTe.CON_CODIGO from T_CARGA_CTE CargaCTe inner join T_CARGA Carga on Carga.CAR_CODIGO = CargaCTe.CAR_CODIGO where CTe.CON_CODIGO = CargaCTe.CON_CODIGO AND (1 = 0 ");
                if (filtrosPesquisa.SituacoesCargaMercante.Any(o => o == SituacaoCargaMercante.Cancelada))
                    where.Append($" or Carga.CAR_SITUACAO = 13");
                if (filtrosPesquisa.SituacoesCargaMercante.Any(o => o == SituacaoCargaMercante.Anulada))
                    where.Append($" or Carga.CAR_SITUACAO = 18");
                if (filtrosPesquisa.SituacoesCargaMercante.Any(o => o == SituacaoCargaMercante.AguardandoEmissao))
                    where.Append($" or (Carga.CAR_SITUACAO = 5 and Carga.CAR_DATA_RECEBIMENTO_ULTIMA_NFE is not null)");
                if (filtrosPesquisa.SituacoesCargaMercante.Any(o => o == SituacaoCargaMercante.PendenteEmissaoCTe))
                    where.Append($" or (Carga.CAR_SITUACAO = 5 and Carga.CAR_DATA_RECEBIMENTO_ULTIMA_NFE is null)");
                if (filtrosPesquisa.SituacoesCargaMercante.Any(o => o == SituacaoCargaMercante.PendenteMDFe))
                    where.Append($" or (Carga.CAR_MDFE_AQUAVIARIO_VINCULADO != 1 and Carga.CAR_SITUACAO != 13 and Carga.CAR_SITUACAO != 18 and Carga.CAR_CODIGO in (select DISTINCT _cargaPedido.CAR_CODIGO from T_CARGA_PEDIDO _cargaPedido where _cargaPedido.TBF_TIPO_COBRANCA_MULTIMODAL = 5))");
                if (filtrosPesquisa.SituacoesCargaMercante.Any(o => o == SituacaoCargaMercante.PendenteMercante))
                    where.Append($" or (Carga.CAR_TODOS_CTES_COM_MERCANTE != 1 and Carga.CAR_SITUACAO != 13 and Carga.CAR_SITUACAO != 18 and Carga.CAR_CODIGO in (select DISTINCT _cargaPedido.CAR_CODIGO from T_CARGA_PEDIDO _cargaPedido where _cargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 4 or _cargaPedido.TBF_TIPO_SERVICO_MULTIMODAL = 4 or _cargaPedido.TBF_TIPO_SERVICO_MULTIMODAL = 5))");
                if (filtrosPesquisa.SituacoesCargaMercante.Any(o => o == SituacaoCargaMercante.PendenteFaturamento))
                    where.Append($" or (Carga.CAR_TODOS_CTES_FATURADOS != 1 and Carga.CAR_SITUACAO != 13 and Carga.CAR_SITUACAO != 18 and Carga.CAR_CODIGO in (select DISTINCT _cargaPedido.CAR_CODIGO from T_CARGA_PEDIDO _cargaPedido where _cargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 1 or _cargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 3 or _cargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 4 or _cargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL = 3))");
                if (filtrosPesquisa.SituacoesCargaMercante.Any(o => o == SituacaoCargaMercante.PendenteIntegracaoCTe))
                    where.Append($" or Carga.CAR_SITUACAO = 15");
                if (filtrosPesquisa.SituacoesCargaMercante.Any(o => o == SituacaoCargaMercante.PendenteIntegracaoFatura))
                    where.Append($" or (Carga.CAR_TODOS_CTES_FATURADOS_INTEGRADOS != 1 and Carga.CAR_SITUACAO != 13 and Carga.CAR_SITUACAO != 18 and Carga.CAR_CODIGO in (select DISTINCT _cargaPedido.CAR_CODIGO from T_CARGA_PEDIDO _cargaPedido where _cargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 1 or _cargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 3 or _cargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 4 or _cargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL = 3))");
                if (filtrosPesquisa.SituacoesCargaMercante.Any(o => o == SituacaoCargaMercante.PendenteSVM))
                {
                    where.Append($" or (Carga.CAR_SITUACAO != 13 and Carga.CAR_SITUACAO != 18 and Carga.CAR_CODIGO in (select DISTINCT _cargaPedido.CAR_CODIGO from T_CARGA_PEDIDO _cargaPedido where _cargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 3) and ");
                    where.Append($@" exists (
                    select
                        cargacte5_.CCT_CODIGO 
                    from
                        T_CARGA_CTE cargactes4_,
                        T_CARGA_CTE cargacte5_ 
                    left outer join
                        T_CTE conhecimen6_ 
                            on cargacte5_.CON_CODIGO=conhecimen6_.CON_CODIGO 
                    where
                        Carga.CAR_CODIGO=cargactes4_.CAR_CODIGO 
                        and cargactes4_.CCT_CODIGO=cargacte5_.CCT_CODIGO 
                        and  not (exists (select
                            ctesvmmult7_.CSM_CODIGO 
                        from
                            T_CTE_SVM_MULTIMODAL ctesvmmult7_ 
                        inner join
                            T_CTE conhecimen8_ 
                                on ctesvmmult7_.CON_CODIGO_SVM=conhecimen8_.CON_CODIGO 
                        inner join
                            T_CTE conhecimen9_ 
                                on ctesvmmult7_.CON_CODIGO_MULTIMODAL=conhecimen9_.CON_CODIGO 
                        where
                            conhecimen8_.CON_STATUS='A'
                            and conhecimen9_.CON_TIPO_CTE=0
                            and (conhecimen9_.CON_CODIGO=conhecimen6_.CON_CODIGO 
                            or (conhecimen9_.CON_CODIGO is null) 
                            and (conhecimen6_.CON_CODIGO is null)))))
                    )");

                }
                if (filtrosPesquisa.SituacoesCargaMercante.Any(o => o == SituacaoCargaMercante.ComErro))
                    where.Append($" or Carga.CAR_SITUACAO = 15 or Carga.CAR_SITUACAO = 6 or Carga.CAR_PROBLEMA_CTE = 1");
                if (filtrosPesquisa.SituacoesCargaMercante.Any(o => o == SituacaoCargaMercante.Finalizada))
                {
                    where.Append($" or (Carga.CAR_SITUACAO = 11 and ");
                    where.Append($" (Carga.CAR_TODOS_CTES_COM_MERCANTE = 1 or Carga.CAR_CODIGO not in (select DISTINCT _cargaPedido.CAR_CODIGO from T_CARGA_PEDIDO _cargaPedido where _cargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 4 or _cargaPedido.TBF_TIPO_SERVICO_MULTIMODAL = 4)) and ");
                    where.Append($" (Carga.CAR_TODOS_CTES_COM_MANIFESTO = 1 or Carga.CAR_CODIGO not in (select DISTINCT _cargaPedido.CAR_CODIGO from T_CARGA_PEDIDO _cargaPedido where _cargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 4 or _cargaPedido.TBF_TIPO_SERVICO_MULTIMODAL = 4)) and ");
                    where.Append($" (Carga.CAR_TODOS_CTES_FATURADOS = 1 or Carga.CAR_CODIGO not in (select DISTINCT _cargaPedido.CAR_CODIGO from T_CARGA_PEDIDO _cargaPedido where _cargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 1 or _cargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 3 or _cargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 4 or _cargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL = 3)) and ");
                    where.Append($" (Carga.CAR_MDFE_AQUAVIARIO_VINCULADO = 1 or Carga.CAR_CODIGO not in (select DISTINCT _cargaPedido.CAR_CODIGO from T_CARGA_PEDIDO _cargaPedido where _cargaPedido.TBF_TIPO_COBRANCA_MULTIMODAL = 5))) ");
                }
                where.Append($" ))");
            }

            if (filtrosPesquisa.CodigoViagem > 0)
                where.Append($" and CTe.CON_VIAGEM = {filtrosPesquisa.CodigoViagem}");

            if (filtrosPesquisa.CodigoPortoOrigem > 0)
                where.Append($" and CTe.POT_CODIGO_ORIGEM = {filtrosPesquisa.CodigoPortoOrigem}");

            if (filtrosPesquisa.CodigoPortoDestino > 0)
                where.Append($" and CTe.POT_CODIGO_DESTINO = {filtrosPesquisa.CodigoPortoDestino}");

            if (filtrosPesquisa.CodigoContainer > 0)
                where.Append($@" and exists (select containerCTe.CON_CODIGO from T_CTE_CONTAINER containerCTe 
                                                    where CTe.CON_CODIGO = containerCTe.CON_CODIGO AND containerCTe.CTR_CODIGO = ({filtrosPesquisa.CodigoContainer}))");

            if (filtrosPesquisa.CodigoMotorista > 0)
                where.Append($@" and exists (select motoristaCTe.CON_CODIGO from T_CTE_MOTORISTA motoristaCTe 
                                                    join T_FUNCIONARIO funcionario on funcionario.FUN_CPF = motoristaCTe.CMO_CPF_MOTORISTA
                                                    where CTe.CON_CODIGO = motoristaCTe.CON_CODIGO AND funcionario.FUN_CODIGO = ({filtrosPesquisa.CodigoMotorista}))");

            if (filtrosPesquisa.TipoServicoMultimodal.Count > 0)
                where.Append($@" and exists (select CargaCTe.CON_CODIGO from T_CARGA_CTE CargaCTe 
                                                    inner join T_CARGA Carga on Carga.CAR_CODIGO = CargaCTe.CAR_CODIGO 
                                                    inner join T_CARGA_PEDIDO CargaPedido on CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO
                                                    where CTe.CON_CODIGO = CargaCTe.CON_CODIGO AND CargaPedido.TBF_TIPO_SERVICO_MULTIMODAL in ({string.Join(", ", filtrosPesquisa.TipoServicoMultimodal.Select(o => o.ToString("D")))}))");

            if (filtrosPesquisa.VeioPorImportacao == Dominio.Enumeradores.OpcaoSimNaoPesquisa.Nao)
                where.Append($" and (CTe.CON_CTE_IMPORTADO_EMBARCADOR = 0 or CTe.CON_CTE_IMPORTADO_EMBARCADOR IS NULL)");
            else if (filtrosPesquisa.VeioPorImportacao == Dominio.Enumeradores.OpcaoSimNaoPesquisa.Sim)
                where.Append($" and CTe.CON_CTE_IMPORTADO_EMBARCADOR = 1");

            if (filtrosPesquisa.SomenteCTeSubstituido)
                where.Append(" and exists (select _cte.CON_CODIGO from T_CTE _cte where _cte.CON_CHAVE_CTE_SUB_COMP = CTe.CON_CHAVECTE)");

            if (filtrosPesquisa.AnuladoGerencialmente)
                where.Append(" and CTe.CON_STATUS = 'Z' and not exists(select CON_CODIGO_ORIGINAL  from T_CTE_RELACAO_DOCUMENTO where CON_CODIGO_ORIGINAL = CTe.CON_CODIGO )");

            if (filtrosPesquisa.ApenasCTeEnviadoMercante)
                where.Append($" and (CTe.CON_NAO_ENVIAR_PARA_MERCANTE = 0 or CTe.CON_NAO_ENVIAR_PARA_MERCANTE IS NULL)");

            if (filtrosPesquisa.DataInicialColeta != DateTime.MinValue)
            {
                where.Append(@" and exists (select CargaCTe.CON_CODIGO from T_CARGA_CTE CargaCTe 
                            inner join T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE CargaPedidoXMLNotaFiscalCTe on CargaPedidoXMLNotaFiscalCTe.CCT_CODIGO = CargaCTe.CCT_CODIGO  
                            inner join T_PEDIDO_XML_NOTA_FISCAL PedidoXMLNotaFiscal on PedidoXMLNotaFiscal.PNF_CODIGO = CargaPedidoXMLNotaFiscalCTe.PNF_CODIGO
                            inner join T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = PedidoXMLNotaFiscal.CPE_CODIGO
                            inner join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
                            where CTe.CON_CODIGO = CargaCTe.CON_CODIGO AND CAST(Pedido.CAR_DATA_CARREGAMENTO_PEDIDO AS DATE) >= '" + filtrosPesquisa.DataInicialColeta.ToString(pattern) + "')");
            }

            if (filtrosPesquisa.DataFinalColeta != DateTime.MinValue)
            {
                where.Append(@" and exists (select CargaCTe.CON_CODIGO from T_CARGA_CTE CargaCTe 
                            inner join T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE CargaPedidoXMLNotaFiscalCTe on CargaPedidoXMLNotaFiscalCTe.CCT_CODIGO = CargaCTe.CCT_CODIGO  
                            inner join T_PEDIDO_XML_NOTA_FISCAL PedidoXMLNotaFiscal on PedidoXMLNotaFiscal.PNF_CODIGO = CargaPedidoXMLNotaFiscalCTe.PNF_CODIGO
                            inner join T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = PedidoXMLNotaFiscal.CPE_CODIGO
                            inner join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
                            where CTe.CON_CODIGO = CargaCTe.CON_CODIGO AND CAST(Pedido.CAR_DATA_CARREGAMENTO_PEDIDO AS DATE) <= '" + filtrosPesquisa.DataFinalColeta.ToString(pattern) + "')");
            }

            if (filtrosPesquisa.DataPagamentoInicial != DateTime.MinValue)
            {
                where.Append($" and TituloCTe.TIT_DATA_LIQUIDACAO >= '{filtrosPesquisa.DataPagamentoInicial.ToString(pattern)}'");
                SetarJoinsTitulo(joins);
            }

            if (filtrosPesquisa.DataPagamentoFinal != DateTime.MinValue)
            {
                where.Append($" and TituloCTe.TIT_DATA_LIQUIDACAO <= '{filtrosPesquisa.DataPagamentoFinal.ToString(pattern)}'");
                SetarJoinsTitulo(joins);
            }

            if (filtrosPesquisa.DataVencimentoInicial != DateTime.MinValue)
            {
                where.Append($" and TituloCTe.TIT_DATA_VENCIMENTO >= '{filtrosPesquisa.DataVencimentoInicial.ToString(pattern)}'");
                SetarJoinsTitulo(joins);
            }

            if (filtrosPesquisa.DataVencimentoFinal != DateTime.MinValue)
            {
                where.Append($" and TituloCTe.TIT_DATA_VENCIMENTO <= '{filtrosPesquisa.DataVencimentoFinal.ToString(pattern)}'");
                SetarJoinsTitulo(joins);
            }

            if (!string.IsNullOrEmpty(filtrosPesquisa.ChaveCTe))
            {
                where.Append($" and CTe.CON_CHAVECTE = '" + filtrosPesquisa.ChaveCTe.Trim() + "'");
            }

            if (!string.IsNullOrEmpty(filtrosPesquisa.NumeroDocumentoRecebedor))
            {
                where.Append($" and Ocorrencia.COC_NUMERO_DOCUMENTO_RECEBEDOR = '" + filtrosPesquisa.NumeroDocumentoRecebedor.Trim() + "'");
                SetarJoinsOcorrencia(joins);
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroPedidoCliente))
            {
                where.Append(@" and exists (select CargaCTe.CON_CODIGO from T_CARGA_CTE CargaCTe 
                            inner join T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE CargaPedidoXMLNotaFiscalCTe on CargaPedidoXMLNotaFiscalCTe.CCT_CODIGO = CargaCTe.CCT_CODIGO  
                            inner join T_PEDIDO_XML_NOTA_FISCAL PedidoXMLNotaFiscal on PedidoXMLNotaFiscal.PNF_CODIGO = CargaPedidoXMLNotaFiscalCTe.PNF_CODIGO
                            inner join T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = PedidoXMLNotaFiscal.CPE_CODIGO
                            inner join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
                            where CTe.CON_CODIGO = CargaCTe.CON_CODIGO AND Pedido.PED_CODIGO_PEDIDO_CLIENTE = '" + filtrosPesquisa.NumeroPedidoCliente + "')");
            }

            if (filtrosPesquisa.codigosCTes?.Count > 0)
                where.Append($" and CTe.CON_CODIGO in ({string.Join(",", filtrosPesquisa.codigosCTes)}) ");

            if (filtrosPesquisa.ModeloVeiculo > 0)
            {
                where.Append($" and VeiculoModelo.VMO_CODIGO = " + filtrosPesquisa.ModeloVeiculo.ToString());
                SetarJoinsModeloVeiculo(joins);
            }

            if (filtrosPesquisa.TipoCarroceria.HasValue && filtrosPesquisa.TipoCarroceria.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCarroceria.Todos)
            {
                where.Append($" and Veiculo.VEI_TIPO_CARROCERIA = '0" + filtrosPesquisa.TipoCarroceria.Value.ToString("d") + "'");
                SetarJoinsVeiculo(joins);
            }

            if (filtrosPesquisa.DataConfirmacaoDocumentosInicial != DateTime.MinValue)
                where.Append($" and exists (select CargaCTe.CON_CODIGO from T_CARGA_CTE CargaCTe inner join T_CARGA Carga on Carga.CAR_CODIGO = CargaCTe.CAR_CODIGO_ORIGEM where CTe.CON_CODIGO = CargaCTe.CON_CODIGO AND CAST(Carga.CAR_DATA_INICIO_CONFIRMACAO_DOCUMENTOS_FISCAIS AS DATE) >= '{filtrosPesquisa.DataConfirmacaoDocumentosInicial.ToString(pattern)}') "); 

            if (filtrosPesquisa.DataConfirmacaoDocumentosFinal != DateTime.MinValue)
                where.Append($" and exists (select CargaCTe.CON_CODIGO from T_CARGA_CTE CargaCTe inner join T_CARGA Carga on Carga.CAR_CODIGO = CargaCTe.CAR_CODIGO_ORIGEM where CTe.CON_CODIGO = CargaCTe.CON_CODIGO AND CAST(Carga.CAR_DATA_INICIO_CONFIRMACAO_DOCUMENTOS_FISCAIS AS DATE) <= '{filtrosPesquisa.DataConfirmacaoDocumentosFinal.ToString(pattern)}') "); 

            if (filtrosPesquisa.TipoProprietarioVeiculo != null)
                where.Append($" AND exists (SELECT VEI_CODIGO FROM T_CTE_VEICULO Vei INNER JOIN T_CTE_VEICULO_PROPRIETARIO VeiProprietario ON VeiProprietario.PVE_CODIGO = Vei.PVE_CODIGO WHERE Vei.CON_CODIGO = CTe.CON_CODIGO and Vei.CVE_TIPO_VEICULO = 0 and Vei.CVE_TIPO_PROPRIEDADE = 'T' AND VeiProprietario.PVE_TIPO = {(int)filtrosPesquisa.TipoProprietarioVeiculo}) "); // SQL-INJECTION-SAFE

            if (filtrosPesquisa.TipoModal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Todos)
                where.Append($" AND CTe.CON_TIPO_MODAL = {filtrosPesquisa.TipoModal.ToString("d")}");

            if (filtrosPesquisa.CodigoContratoFreteTerceiro > 0)
                where.Append($@" AND  exists (select ContratoFreteTerceiro.CFT_NUMERO_CONTRATO from T_CARGA_CTE CargaCTe
                                            inner join T_CARGA Carga on Carga.CAR_CODIGO = CargaCTe.CAR_CODIGO_ORIGEM
                                            join T_CONTRATO_FRETE_TERCEIRO ContratoFreteTerceiro on Carga.CAR_CODIGO = ContratoFreteTerceiro.CAR_CODIGO
                                            where CargaCTe.CON_CODIGO = CTe.CON_CODIGO AND ContratoFreteTerceiro.CFT_CODIGO = {filtrosPesquisa.CodigoContratoFreteTerceiro})");

            if (filtrosPesquisa.Vendedor.Count > 0)
            {
                where.Append($@" AND EXISTS (select FUN_CPF 
                                   FROM T_CTE AS _cte 
                                   JOIN T_CTE_PARTICIPANTE cp on cp.PCT_CODIGO = _cte.CON_TOMADOR_PAGADOR_CTE 
                                   JOIN T_GRUPO_PESSOAS gp on cp.GRP_CODIGO = gp.GRP_CODIGO 
                                   JOIN T_GRUPO_PESSOAS_FUNCIONARIO gpf on gpf.GRP_CODIGO = gp.GRP_CODIGO 
                                   JOIN T_FUNCIONARIO f on f.FUN_CODIGO = gpf.FUN_CODIGO 
                                   where CTe.CON_CODIGO = _cte.CON_CODIGO 
                                   AND f.FUN_CODIGO IN ({string.Join(", ", filtrosPesquisa.Vendedor)})) "
                             );
            }

            if (filtrosPesquisa.CodigosRecebedores.Count > 0)
            {
                where.Append($" and RecebedorCTe.CLI_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosRecebedores)})");

                SetarJoinsRecebedor(joins);
            }

            if (filtrosPesquisa.TipoOSConvertido.Count > 0)
            {
                where.Append(" AND EXISTS (");
                where.Append("    SELECT 1 from T_CARGA_PEDIDO CargaPedido");
                where.Append("    LEFT JOIN T_PEDIDO Pedido ON CargaPedido.PED_CODIGO = Pedido.PED_CODIGO");
                where.Append("    WHERE CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO");
                where.Append($"   AND Pedido.PED_TIPO_OS_CONVERTIDO in ({string.Join(", ", filtrosPesquisa.TipoOSConvertido.Select(o => o.ToString("d")))}))");

                SetarJoinsCarga(joins);
            }

            if (filtrosPesquisa.TipoOS.Count > 0)
            {
                where.Append(" AND EXISTS (");
                where.Append("    SELECT 1 from T_CARGA_PEDIDO CargaPedido");
                where.Append("    LEFT JOIN T_PEDIDO Pedido ON CargaPedido.PED_CODIGO = Pedido.PED_CODIGO");
                where.Append("    WHERE CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO");
                where.Append($"   AND Pedido.PED_TIPO_OS in ({string.Join(", ", filtrosPesquisa.TipoOS.Select(o => o.ToString("d")))}))");

                SetarJoinsCarga(joins);
            }

            if (filtrosPesquisa.ProvedorOS > 0)
            {
                where.Append($" AND Cte.CON_CLIENTE_PROVEDOR_OS = '{filtrosPesquisa.ProvedorOS}'");
            }

            if (filtrosPesquisa.CentroDeCustoViagemCodigo > 0)
            {
                where.Append(" AND EXISTS (");
                where.Append("    SELECT 1 from T_CARGA_PEDIDO CargaPedido");
                where.Append("    LEFT JOIN T_PEDIDO Pedido ON CargaPedido.PED_CODIGO = Pedido.PED_CODIGO");
                where.Append("    WHERE CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO");
                where.Append($"   AND Pedido.CCV_CODIGO = {filtrosPesquisa.CentroDeCustoViagemCodigo})");

                SetarJoinsCarga(joins);
            }

            if (filtrosPesquisa.CNPJDivergenteCTeMDFe)
            {
                SetarJoinsTransportador(joins);
                SetarJoinsTransportadorMdfe(joins);
                where.Append($" AND TransportadorCTe.EMP_CNPJ <> TransportadorMdfe.CnpjMdfe");
            }

            if (filtrosPesquisa.TipoEmissao == TipoEmissao.EmissaoCarga || filtrosPesquisa.TipoEmissao == TipoEmissao.EmissaoManual)
            {
                int geradoManualmente = filtrosPesquisa.TipoEmissao == TipoEmissao.EmissaoManual ? 1 : 0;
                where.Append($" AND CTe.CON_GERADO_MANUALMENTE = '{geradoManualmente}' ");
            }

            if (filtrosPesquisa.TipoEmissao == TipoEmissao.EmissaoAgrupado)
                where.Append(@" 
                    AND EXISTS (
                        SELECT 1 
                        FROM T_CARGA_CTE_AGRUPADO_CTE agrupado 
                        WHERE agrupado.CON_CODIGO = cte.CON_CODIGO
                    )");

        }

        #endregion

        #region Métodos Públicos

        public string ObterSqlPesquisaNotasFiscais(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaCteRelatorio filtrosPesquisa)
        {
            StringBuilder groupBy = new StringBuilder();
            StringBuilder joins = new StringBuilder();
            StringBuilder sql = new StringBuilder();
            StringBuilder where = new StringBuilder();

            SetarWhere(filtrosPesquisa, where, joins, groupBy);

            sql.Append("select Documento.CON_CODIGO CodigoCTe, Documento.NFC_NUMERO Numero, Documento.NFC_VALOR Valor, Documento.NFC_CHAVENFE Chave, Documento.NFC_PESO Peso, Documento.NFC_SERIE Serie ");
            sql.Append("  from T_CTE_DOCS Documento ");
            sql.Append(" inner join T_CTE CTe on CTe.CON_CODIGO = Documento.CON_CODIGO ");
            sql.Append("  left join T_CARGA_CTE CargaCTe on CargaCTe.CON_CODIGO = CTe.CON_CODIGO ");
            sql.Append(joins.ToString());

            if (where.Length > 0)
                sql.Append($" where {where.ToString().Trim().Substring(3)} ");

            return sql.ToString();
        }

        #endregion
    }
}