using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio
{
    sealed class ConsultaConhecimentoDeTransporteEletronicoSubcontratado : Embarcador.Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioCTesSubcontratados>
    {
        #region Construtores

        public ConsultaConhecimentoDeTransporteEletronicoSubcontratado() : base(tabela: "T_SUBCONTRATACAO_DOCUMENTOS as SubcontratacaoDocumento ") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsCTeOriginal(StringBuilder joins)
        {
            if (!joins.Contains(" CTeOriginal "))
                joins.Append(" left join T_CTE CTeOriginal on CTeOriginal.CON_CODIGO = SubcontratacaoDocumento.CON_CODIGO ");
        }

        private void SetarJoinsSubcontratacao(StringBuilder joins)
        {
            if (!joins.Contains(" Subcontratacao "))
                joins.Append(" join T_SUBCONTRATACAO Subcontratacao ON Subcontratacao.SUB_CODIGO = SubcontratacaoDocumento.SUB_CODIGO ");
        }

        private void SetarJoinsCTeSubcontratado(StringBuilder joins)
        {
            SetarJoinsSubcontratacao(joins);

            if (!joins.Contains(" CTeSubcontratado "))
                joins.Append(" join T_CTE CTeSubcontratado ON CTeSubcontratado.CON_CODIGO = Subcontratacao.CON_CODIGO_SUBCONTRATACAO ");
        }

        private void SetarJoinsEmpresaSerieOriginal(StringBuilder joins)
        {
            SetarJoinsCTeOriginal(joins);

            if (!joins.Contains(" EmpresaSerieOriginal "))
                joins.Append(" left join T_EMPRESA_SERIE EmpresaSerieOriginal on EmpresaSerieOriginal.ESE_CODIGO = CTeOriginal.CON_SERIE ");
        }

        private void SetarJoinsEmpresaSerieSubcontratado(StringBuilder joins)
        {
            SetarJoinsCTeSubcontratado(joins);

            if (!joins.Contains(" EmpresaSerieSubcontratado "))
                joins.Append(" left join T_EMPRESA_SERIE EmpresaSerieSubcontratado on EmpresaSerieSubcontratado.ESE_CODIGO = CTeSubcontratado.CON_SERIE ");
        }

        private void SetarJoinsModeloDocumentoFiscal(StringBuilder joins)
        {
            SetarJoinsCTeOriginal(joins);

            if (!joins.Contains(" ModeloDocumentoFiscal "))
                joins.Append(" left join T_MODDOCFISCAL ModeloDocumentoFiscal on ModeloDocumentoFiscal.MOD_CODIGO = CTeOriginal.CON_MODELODOC ");
        }

        private void SetarJoinsTransportador(StringBuilder joins)
        {
            if (!joins.Contains(" TransportadorCTe "))
                joins.Append(" left join T_EMPRESA TransportadorCTe on TransportadorCTe.EMP_CODIGO = CTeOriginal.EMP_CODIGO ");
        }

        private void SetarJoinsTransportadorSubcontratado(StringBuilder joins)
        {
            SetarJoinsCTeSubcontratado(joins);

            if (!joins.Contains(" TransportadorCTeSubcontratado "))
                joins.Append(" left join T_EMPRESA TransportadorCTeSubcontratado on TransportadorCTeSubcontratado.EMP_CODIGO = CTeSubcontratado.EMP_CODIGO ");
        }

        private void SetarJoinsCfop(StringBuilder joins)
        {
            if (!joins.Contains(" CFOP "))
                joins.Append(" left join T_CFOP CFOP on CFOP.CFO_CODIGO = CTeOriginal.CFO_CODIGO ");
        }

        private void SetarJoinsComplemento(StringBuilder joins)
        {
            if (!joins.Contains(" ComplementoInfo "))
                joins.Append(" left join T_CARGA_CTE_COMPLEMENTO_INFO ComplementoInfo on ComplementoInfo.CON_CODIGO = CTeOriginal.CON_CODIGO ");
        }

        private void SetarJoinsDestinatario(StringBuilder joins)
        {
            if (!joins.Contains(" DestinatarioCTe "))
                joins.Append(" left join T_CTE_PARTICIPANTE DestinatarioCTe on CTeOriginal.CON_DESTINATARIO_CTE = DestinatarioCTe.PCT_CODIGO ");
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

        private void SetarJoinsDocumentoFaturamento(StringBuilder joins, Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioCTesSubcontratados filtrosPesquisa)
        {
            if (!joins.Contains(" DocumentoFaturamentoCTe "))
                joins.Append($" left join T_DOCUMENTO_FATURAMENTO DocumentoFaturamentoCTe on CTeOriginal.CON_CODIGO = DocumentoFaturamentoCTe.CON_CODIGO {(filtrosPesquisa.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? "" : "and DocumentoFaturamentoCTe.DFA_SITUACAO = 5")}");
        }

        private void SetarJoinsExpedidor(StringBuilder joins)
        {
            if (!joins.Contains(" ExpedidorCTe "))
                joins.Append(" left join T_CTE_PARTICIPANTE ExpedidorCTe on CTeOriginal.CON_EXPEDIDOR_CTE = ExpedidorCTe.PCT_CODIGO ");
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
            if (!joins.Contains(" Fatura "))
                joins.Append(" left join T_FATURA Fatura on Fatura.FAT_CODIGO = CTeOriginal.FAT_CODIGO ");
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
                joins.Append(" left join T_INTEGRACAO_CTE_RECEBIMENTO IntegracaoCTeRecebimento on IntegracaoCTeRecebimento.CON_CODIGO = CTeOriginal.CON_CODIGO and IntegracaoCTeRecebimento.ICR_TIPO = 0 ");
        }

        private void SetarJoinsLocalidadeFimPrestacao(StringBuilder joins)
        {
            if (!joins.Contains(" FimPrestacaoCTe "))
                joins.Append(" left join T_LOCALIDADES FimPrestacaoCTe on CTeOriginal.CON_LOCTERMINOPRESTACAO = FimPrestacaoCTe.LOC_CODIGO ");
        }

        private void SetarJoinsLocalidadeInicioPrestacao(StringBuilder joins)
        {
            if (!joins.Contains(" InicioPrestacaoCTe "))
                joins.Append(" left join T_LOCALIDADES InicioPrestacaoCTe on CTeOriginal.CON_LOCINICIOPRESTACAO = InicioPrestacaoCTe.LOC_CODIGO ");
        }

        private void SetarJoinsModeloDocumento(StringBuilder joins)
        {
            if (!joins.Contains(" ModeloDocumento "))
                joins.Append(" left join T_MODDOCFISCAL ModeloDocumento on CTeOriginal.CON_MODELODOC = ModeloDocumento.MOD_CODIGO ");
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

        private void SetarJoinsRecebedor(StringBuilder joins)
        {
            if (!joins.Contains(" RecebedorCTe "))
                joins.Append(" left join T_CTE_PARTICIPANTE RecebedorCTe on CTeOriginal.CON_RECEBEDOR_CTE = RecebedorCTe.PCT_CODIGO ");
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
                joins.Append(" left join T_CTE_PARTICIPANTE RemetenteCTe on CTeOriginal.CON_REMETENTE_CTE = RemetenteCTe.PCT_CODIGO ");
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
                joins.Append(" left join T_EMPRESA_SERIE Serie on CTeOriginal.CON_SERIE = Serie.ESE_CODIGO ");
        }

        private void SetarJoinsTitulo(StringBuilder joins)
        {
            if (!joins.Contains(" TituloCTe "))
                joins.Append(" left join T_TITULO TituloCTe on TituloCTe.TIT_CODIGO = CTeOriginal.TIT_CODIGO ");
        }

        private void SetarJoinsTomador(StringBuilder joins)
        {
            if (!joins.Contains(" TomadorPagadorCTe "))
                joins.Append(" left join T_CTE_PARTICIPANTE TomadorPagadorCTe on CTeOriginal.CON_TOMADOR_PAGADOR_CTE = TomadorPagadorCTe.PCT_CODIGO ");
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
                joins.Append(" left join T_DOCUMENTO_ESCRITURACAO DocumentoEscrituracao ON DocumentoEscrituracao.CON_CODIGO = CTeOriginal.CON_CODIGO ");
        }

        private void SetarJoinsLoteEscrituracao(StringBuilder joins)
        {
            SetarJoinsDocumentoEscrituracao(joins);

            if (!joins.Contains(" LoteEscrituracao "))
                joins.Append(" left join T_LOTE_ESCRITURACAO LoteEscrituracao ON LoteEscrituracao.LES_CODIGO = DocumentoEscrituracao.LES_CODIGO ");
        }

        private void SetarJoinsPagamento(StringBuilder joins, Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioCTesSubcontratados filtrosPesquisa)
        {
            SetarJoinsDocumentoFaturamento(joins, filtrosPesquisa);

            if (!joins.Contains(" Pagamento "))
                joins.Append(" left join T_PAGAMENTO Pagamento on Pagamento.PAG_CODIGO = DocumentoFaturamentoCTe.PAG_CODIGO ");
        }

        private void SetarJoinsPortoOrigem(StringBuilder joins)
        {
            if (!joins.Contains(" PortoOrigem "))
                joins.Append(" left join T_PORTO PortoOrigem on PortoOrigem.POT_CODIGO = CTeOriginal.POT_CODIGO_ORIGEM ");
        }

        private void SetarJoinsPortoDestino(StringBuilder joins)
        {
            if (!joins.Contains(" PortoDestino "))
                joins.Append(" left join T_PORTO PortoDestino on PortoDestino.POT_CODIGO = CTeOriginal.POT_CODIGO_DESTINO ");
        }

        private void SetarJoinsPortoPassagemUm(StringBuilder joins)
        {
            if (!joins.Contains(" PortoPassagemUm "))
                joins.Append(" left join T_PORTO PortoPassagemUm on PortoPassagemUm.POT_CODIGO = CTeOriginal.POT_CODIGO_PASSAGEM_UM ");
        }

        private void SetarJoinsPortoPassagemDois(StringBuilder joins)
        {
            if (!joins.Contains(" PortoPassagemDois "))
                joins.Append(" left join T_PORTO PortoPassagemDois on PortoPassagemDois.POT_CODIGO = CTeOriginal.POT_CODIGO_PASSAGEM_DOIS ");
        }

        private void SetarJoinsPortoPassagemTres(StringBuilder joins)
        {
            if (!joins.Contains(" PortoPassagemTres "))
                joins.Append(" left join T_PORTO PortoPassagemTres on PortoPassagemTres.POT_CODIGO = CTeOriginal.POT_CODIGO_PASSAGEM_TRES ");
        }

        private void SetarJoinsPortoPassagemQuatro(StringBuilder joins)
        {
            if (!joins.Contains(" PortoPassagemQuatro "))
                joins.Append(" left join T_PORTO PortoPassagemQuatro on PortoPassagemQuatro.POT_CODIGO = CTeOriginal.POT_CODIGO_PASSAGEM_QUATRO ");
        }

        private void SetarJoinsPortoPassagemCinco(StringBuilder joins)
        {
            if (!joins.Contains(" PortoPassagemCinco "))
                joins.Append(" left join T_PORTO PortoPassagemCinco on PortoPassagemCinco.POT_CODIGO = CTeOriginal.POT_CODIGO_PASSAGEM_CINCO ");
        }

        private void SetarJoinsViagem(StringBuilder joins)
        {
            if (!joins.Contains(" Viagem "))
                joins.Append(" left join T_PEDIDO_VIAGEM_NAVIO Viagem on Viagem.PVN_CODIGO = CTeOriginal.CON_VIAGEM ");
        }

        private void SetarJoinsViagemScheduleDestino(StringBuilder joins)
        {
            if (!joins.Contains(" ViagemScheduleDestino "))
                joins.Append(" left join T_PEDIDO_VIAGEM_NAVIO_SCHEDULE ViagemScheduleDestino on ViagemScheduleDestino.PVN_CODIGO = CTeOriginal.CON_VIAGEM AND ViagemScheduleDestino.TTI_CODIGO_ATRACACAO = CTeOriginal.CON_TERMINAL_ORIGEM ");
        }

        private void SetarJoinsDocumentoEscrituracaoCancelamento(StringBuilder joins)
        {
            if (!joins.Contains(" DocumentoEscrituracaoCancelamento "))
                joins.Append(" left join T_DOCUMENTO_ESCRITURACAO_CANCELAMENTO DocumentoEscrituracaoCancelamento ON DocumentoEscrituracaoCancelamento.CON_CODIGO = CTeOriginal.CON_CODIGO ");
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
                joins.Append(" LEFT JOIN T_CTE_VEICULO CteVeiculo on CteVeiculo.CON_CODIGO = CTeOriginal.CON_CODIGO ");
        }

        private void SetarJoinsVeiculo(StringBuilder joins)
        {
            SetarJoinsCTeVeiculo(joins);

            if (!joins.Contains(" Veiculo "))
                joins.Append(" LEFT JOIN T_VEICULO Veiculo on Veiculo.VEI_CODIGO = CteVeiculo.VEI_CODIGO ");
        }

        private void SetarJoinsCentroResultado(StringBuilder joins)
        {
            if (!joins.Contains(" CentroResultado "))
                joins.Append(" LEFT OUTER JOIN T_CENTRO_RESULTADO CentroResultado on CentroResultado.CRE_CODIGO = CTeOriginal.CRE_CODIGO_FATURAMENTO ");
        }

        private void SetarJoinsRegraICMS(StringBuilder joins)
        {
            if (!joins.Contains(" RegraICMS "))
                joins.Append(" LEFT JOIN T_REGRA_ICMS RegraICMS ON RegraICMS.RIC_CODIGO = CTeOriginal.RIC_CODIGO ");
        }


        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioCTesSubcontratados filtrosPesquisa)
        {
            switch (propriedade)
            {
                case "NumeroCTe":
                    if (!select.Contains(" NumeroCTe,"))
                    {
                        select.Append("CTeOriginal.CON_NUM as NumeroCTe, ");
                        groupBy.Append("CTeOriginal.CON_NUM, ");

                        SetarJoinsCTeOriginal(joins);
                    }
                    break;

                case "SerieCTe":
                    if (!select.Contains(" SerieCTe,"))
                    {
                        select.Append("EmpresaSerieOriginal.ESE_NUMERO as SerieCTe, ");
                        groupBy.Append("EmpresaSerieOriginal.ESE_NUMERO, ");

                        SetarJoinsEmpresaSerieOriginal(joins);
                    }
                    break;

                case "NumeroCarga":
                    if (!select.Contains(" NumeroCarga,"))
                    {
                        select.Append(@"SUBSTRING (
                                            (
                                             SELECT DISTINCT ', ' + _carga.CAR_CODIGO_CARGA_EMBARCADOR
                                               FROM T_CARGA _carga
                                               JOIN T_CARGA_CTE _cargaCTe ON _carga.CAR_CODIGO = _cargaCTe.CAR_CODIGO
                                              WHERE _cargaCTe.CON_CODIGO = CTeOriginal.CON_CODIGO
                                                FOR XML PATH('')
                                            ), 
                                        3, 1000) as NumeroCarga, ");
                        groupBy.Append("CTeOriginal.CON_CODIGO, ");

                        SetarJoinsCTeOriginal(joins);
                    }
                    break;

                case "Modelo":
                    if (!select.Contains(" Modelo,"))
                    {
                        select.Append("ModeloDocumentoFiscal.MOD_TIPO_DOCUMENTO_EMISSAO as Modelo, ");
                        groupBy.Append("ModeloDocumentoFiscal.MOD_TIPO_DOCUMENTO_EMISSAO, ");

                        SetarJoinsModeloDocumentoFiscal(joins);
                    }
                    break;

                case "CTeSubcontratacao":
                    if (!select.Contains(" CTeSubcontratacao,"))
                    {
                        select.Append("CTeSubcontratado.CON_NUM as CTeSubcontratacao, ");
                        groupBy.Append("CTeSubcontratado.CON_NUM, ");

                        SetarJoinsCTeSubcontratado(joins);
                    }
                    break;

                case "SerieCTeSubcontracao":
                    if (!select.Contains(" SerieCTeSubcontracao,"))
                    {
                        select.Append("EmpresaSerieSubcontratado.ESE_NUMERO as SerieCTeSubcontracao, ");
                        groupBy.Append("EmpresaSerieSubcontratado.ESE_NUMERO, ");

                        SetarJoinsEmpresaSerieSubcontratado(joins);
                    }
                    break;

                case "ChaveCTeSubcontratacao":
                    if (!select.Contains(" ChaveCTeSubcontratacao,"))
                    {
                        select.Append("CTeSubcontratado.CON_CHAVECTE as ChaveCTeSubcontratacao, ");
                        groupBy.Append("CTeSubcontratado.CON_CHAVECTE, ");

                        SetarJoinsCTeSubcontratado(joins);
                    }
                    break;

                case "ObservacaoCTeSubcontratacao":
                    if (!select.Contains(" ObservacaoCTeSubcontratacao,"))
                    {
                        select.Append("CTeSubcontratado.CON_OBSGERAIS as ObservacaoCTeSubcontratacao, ");
                        groupBy.Append("CTeSubcontratado.CON_OBSGERAIS, ");

                        SetarJoinsCTeSubcontratado(joins);
                    }
                    break;

                case "NomeFantasiaTransportadorSubcontratacao":
                    if (!select.Contains(" NomeFantasiaTransportadorSubcontratacao, "))
                    {
                        select.Append("TransportadorCTeSubcontratado.EMP_FANTASIA NomeFantasiaTransportadorSubcontratacao, ");
                        groupBy.Append("TransportadorCTeSubcontratado.EMP_FANTASIA, ");

                        SetarJoinsTransportadorSubcontratado(joins);
                    }
                    break;

                case "RazaoSocialTransportadorSubcontratacao":
                    if (!select.Contains(" RazaoSocialTransportadorSubcontratacao, "))
                    {
                        select.Append("TransportadorCTeSubcontratado.EMP_RAZAO RazaoSocialTransportadorSubcontratacao, ");
                        groupBy.Append("TransportadorCTeSubcontratado.EMP_RAZAO, ");

                        SetarJoinsTransportadorSubcontratado(joins);
                    }
                    break;

                case "CNPJTransportadorSubcontratacaoFormatada":
                    if (!select.Contains(" CNPJTransportadorSubcontratacao, "))
                    {
                        select.Append("TransportadorCTeSubcontratado.EMP_CNPJ CNPJTransportadorSubcontratacao, ");
                        groupBy.Append("TransportadorCTeSubcontratado.EMP_CNPJ, ");

                        SetarJoinsTransportadorSubcontratado(joins);
                    }
                    break;

                case "ValorICMSSubcontratacao":
                    if (!select.Contains(" ValorICMSSubcontratacao, "))
                    {
                        select.Append("CTeSubcontratado.CON_VAL_ICMS as ValorICMSSubcontratacao, ");
                        groupBy.Append("CTeSubcontratado.CON_VAL_ICMS, ");

                        SetarJoinsCTeSubcontratado(joins);
                    }
                    break;

                case "CSTIBSCBS":
                    if (!select.Contains(" CSTIBSCBS, "))
                    {
                        select.Append("CTeSubcontratado.CON_CST_IBSCBS CSTIBSCBS, ");
                        groupBy.Append("CTeSubcontratado.CON_CST_IBSCBS, ");
                    }
                    break;

                case "ClassificacaoTributariaIBSCBS":
                    if (!select.Contains(" ClassificacaoTributariaIBSCBS, "))
                    {
                        select.Append("CTeSubcontratado.CON_CLASSIFICACAO_TRIBUTARIA_IBSCBS ClassificacaoTributariaIBSCBS, ");
                        groupBy.Append("CTeSubcontratado.CON_CLASSIFICACAO_TRIBUTARIA_IBSCBS, ");
                    }
                    break;

                case "BaseCalculoIBSCBS":
                    if (!select.Contains(" BaseCalculoIBSCBS, "))
                    {
                        select.Append("sum(CTeSubcontratado.CON_BASE_CALCULO_IBSCBS) BaseCalculoIBSCBS, ");
                    }
                    break;

                case "AliquotaIBSEstadual":
                    if (!select.Contains(" AliquotaIBSEstadual, "))
                    {
                        select.Append("sum(CTeSubcontratado.CON_ALIQUOTA_IBS_ESTADUAL) AliquotaIBSEstadual, ");
                    }
                    break;

                case "PercentualReducaoIBSEstadual":
                    if (!select.Contains(" PercentualReducaoIBSEstadual, "))
                    {
                        select.Append("sum(CTeSubcontratado.CON_PERCENTUAL_REDUCAO_IBS_ESTADUAL) PercentualReducaoIBSEstadual, ");
                    }
                    break;

                case "ValorIBSEstadual":
                    if (!select.Contains(" ValorIBSEstadual, "))
                    {
                        select.Append("sum(CTeSubcontratado.CON_VALOR_IBS_ESTADUAL) ValorIBSEstadual, ");
                    }
                    break;

                case "AliquotaIBSMunicipal":
                    if (!select.Contains(" AliquotaIBSMunicipal, "))
                    {
                        select.Append("sum(CTeSubcontratado.CON_ALIQUOTA_IBS_MUNICIPAL) AliquotaIBSMunicipal, ");
                    }
                    break;

                case "PercentualReducaoIBSMunicipal":
                    if (!select.Contains(" PercentualReducaoIBSMunicipal, "))
                    {
                        select.Append("sum(CTeSubcontratado.CON_PERCENTUAL_REDUCAO_IBS_MUNICIPAL) PercentualReducaoIBSMunicipal, ");
                    }
                    break;

                case "ValorIBSMunicipal":
                    if (!select.Contains(" ValorIBSMunicipal, "))
                    {
                        select.Append("sum(CTeSubcontratado.CON_VALOR_IBS_MUNICIPAL) ValorIBSMunicipal, ");
                    }
                    break;

                case "AliquotaCBS":
                    if (!select.Contains(" AliquotaCBS, "))
                    {
                        select.Append("sum(CTeSubcontratado.CON_ALIQUOTA_CBS) AliquotaCBS, ");
                    }
                    break;

                case "PercentualReducaoCBS":
                    if (!select.Contains(" PercentualReducaoCBS, "))
                    {
                        select.Append("sum(CTeSubcontratado.CON_PERCENTUAL_REDUCAO_CBS) PercentualReducaoCBS, ");
                    }
                    break;

                case "ValorCBS":
                    if (!select.Contains(" ValorCBS, "))
                    {
                        select.Append("sum(CTeSubcontratado.CON_VALOR_CBS) ValorCBS, ");
                    }
                    break;

                case "ValorFreteSubcontratacao":
                    if (!select.Contains(" ValorFreteSubcontratacao, "))
                    {
                        select.Append("CTeSubcontratado.CON_VALOR_FRETE as ValorFreteSubcontratacao, ");
                        groupBy.Append("CTeSubcontratado.CON_VALOR_FRETE, ");

                        SetarJoinsCTeSubcontratado(joins);
                    }
                    break;

                case "ValorReceberSubcontratacao":
                    if (!select.Contains(" ValorReceberSubcontratacao, "))
                    {
                        select.Append("CTeSubcontratado.CON_VALOR_RECEBER as ValorReceberSubcontratacao, ");
                        groupBy.Append("CTeSubcontratado.CON_VALOR_RECEBER, ");

                        SetarJoinsCTeSubcontratado(joins);
                    }
                    break;

                case "ValorTotalSemImpostoSubcontratacao":
                    if (!select.Contains(" ValorTotalSemImpostoSubcontratacao, "))
                    {
                        select.Append("SUM(CTeSubcontratado.CON_VALOR_PREST_SERVICO - CTeSubcontratado.CON_VAL_ICMS - CTeSubcontratado.CON_VALOR_ISS) as ValorTotalSemImpostoSubcontratacao, ");
                        groupBy.Append("CTeSubcontratado.CON_VALOR_PREST_SERVICO, CTeSubcontratado.CON_VALOR_ISS, CTeSubcontratado.CON_VAL_ICMS, ");

                        SetarJoinsCTeSubcontratado(joins);
                    }
                    break;

                case "ValorPrestacaoSubcontratacao":
                    if (!somenteContarNumeroRegistros && !select.Contains("ValorPrestacaoSubcontratacao"))
                    {
                        select.Append("SUM(CTeSubcontratado.CON_VALOR_PREST_SERVICO) ValorPrestacaoSubcontratacao, ");

                        SetarJoinsCTeSubcontratado(joins);
                    }
                    break;

                case "ValorICMS":
                    if (!select.Contains(" ValorICMS,"))
                    {
                        select.Append("CTeOriginal.CON_VAL_ICMS as ValorICMS, ");
                        groupBy.Append("CTeOriginal.CON_VAL_ICMS, ");
                    }
                    break;

                case "AliquotaISS":
                    if (!select.Contains(" AliquotaISS,"))
                    {
                        select.Append("CTeOriginal.CON_ALIQUOTA_ISS as AliquotaISS, ");
                        groupBy.Append("CTeOriginal.CON_ALIQUOTA_ISS, ");
                    }
                    break;

                case "ValorISS":
                    if (!select.Contains(" ValorISS,"))
                    {
                        select.Append("CTeOriginal.CON_VALOR_ISS as ValorISS, ");
                        groupBy.Append("CTeOriginal.CON_VALOR_ISS, ");
                    }
                    break;

                case "ValorFrete":
                    if (!select.Contains(" ValorFrete,"))
                    {
                        select.Append("CTeOriginal.CON_VALOR_FRETE as ValorFrete, ");
                        groupBy.Append("CTeOriginal.CON_VALOR_FRETE, ");
                    }
                    break;

                case "ValorReceber":
                    if (!select.Contains(" ValorReceber,"))
                    {
                        select.Append("CTeOriginal.CON_VALOR_RECEBER as ValorReceber, ");
                        groupBy.Append("CTeOriginal.CON_VALOR_RECEBER, ");
                    }
                    break;

                case "ValorTotalSemImposto":
                    if (!select.Contains(" ValorTotalSemImposto,"))
                    {
                        select.Append("SUM(CTeOriginal.CON_VALOR_PREST_SERVICO - CTeOriginal.CON_VAL_ICMS - CTeOriginal.CON_VALOR_ISS) as ValorTotalSemImposto, ");
                        groupBy.Append("CTeOriginal.CON_VALOR_PREST_SERVICO, CTeOriginal.CON_VALOR_ISS, CTeOriginal.CON_VAL_ICMS, ");
                    }
                    break;

                case "Veiculo":
                    if (!select.Contains(" Veiculo,"))
                    {
                        select.Append(@" SUBSTRING(
                                                   (
                                                    SELECT DISTINCT ', ' + _veiculo.VEI_PLACA
                                                      FROM T_CTE_VEICULO _cteVeiculo
                                                      JOIN T_VEICULO _veiculo ON _veiculo.VEI_CODIGO = _cteVeiculo.VEI_CODIGO
                                                     WHERE _cteVeiculo.CON_CODIGO = CTeOriginal.CON_CODIGO
                                                       FOR XML PATH('')
                                                   ), 3, 1000
                                                  ) as Veiculo, ");
                        groupBy.Append("CTeOriginal.CON_CODIGO, ");

                        SetarJoinsCTeOriginal(joins);
                    }
                    break;
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
                                 where _cargaCTe.CON_CODIGO = CTeOriginal.CON_CODIGO
                                   for xml path('')
                            ), 3, 1000) CNPJFilial, "
                        );

                        if (!groupBy.Contains(" CTeOriginal.CON_CODIGO"))
                            groupBy.Append(" CTeOriginal.CON_CODIGO, ");
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
                                 where _cargaCTe.CON_CODIGO = CTeOriginal.CON_CODIGO
                                   for xml path('')
                            ), 3, 1000) Filial, "
                        );

                        if (!groupBy.Contains(" CTeOriginal.CON_CODIGO"))
                            groupBy.Append(" CTeOriginal.CON_CODIGO, ");
                    }
                    break;

                case "ChaveCTe":
                    if (!select.Contains("ChaveCTe"))
                    {
                        select.Append("CTeOriginal.CON_CHAVECTE ChaveCTe, ");
                        groupBy.Append("CTeOriginal.CON_CHAVECTE, ");
                    }
                    break;

                case "Log":
                    if (!select.Contains("Log"))
                    {
                        select.Append("CTeOriginal.CON_LOG Log, ");
                        groupBy.Append("CTeOriginal.CON_LOG, ");
                    }
                    break;

                case "ProtocoloAutorizacao":
                    if (!select.Contains("ProtocoloAutorizacao"))
                    {
                        select.Append("CTeOriginal.CON_PROTOCOLO ProtocoloAutorizacao, ");
                        groupBy.Append("CTeOriginal.CON_PROTOCOLO, ");
                    }
                    break;

                case "ProtocoloInutilizacaoCancelamento":
                    if (!select.Contains("ProtocoloInutilizacaoCancelamento"))
                    {
                        select.Append("CTeOriginal.CON_PROTOCOLOCANINU ProtocoloInutilizacaoCancelamento, ");
                        groupBy.Append("CTeOriginal.CON_PROTOCOLOCANINU, ");
                    }
                    break;

                case "RetornoSefaz":
                    if (!select.Contains("RetornoSefaz"))
                    {
                        select.Append("CTeOriginal.CON_RETORNOCTE RetornoSefaz, ");
                        groupBy.Append("CTeOriginal.CON_RETORNOCTE, ");
                    }
                    break;

                case "DescricaoTipoServico":
                    if (!select.Contains("TipoServico"))
                    {
                        select.Append("CTeOriginal.CON_TIPO_SERVICO TipoServico, ");
                        groupBy.Append("CTeOriginal.CON_TIPO_SERVICO, ");
                    }
                    break;

                case "DescricaoTipoTomador":
                    if (!select.Contains("TipoTomador"))
                    {
                        select.Append("CTeOriginal.CON_TOMADOR TipoTomador, ");
                        groupBy.Append("CTeOriginal.CON_TOMADOR, ");
                    }
                    break;

                case "DescricaoTipoCTe":
                    if (!select.Contains("TipoCTe"))
                    {
                        select.Append("CTeOriginal.CON_TIPO_CTE TipoCTe, ");
                        groupBy.Append("CTeOriginal.CON_TIPO_CTE, ");
                    }
                    break;

                case "RPS":
                    if (!select.Contains("RPS"))
                    {
                        select.Append("RPS.RPS_NUMERO RPS, ");
                        groupBy.Append("RPS.RPS_NUMERO, ");
                        joins.Append(" left outer join T_NFSE_RPS RPS on CTeOriginal.RPS_CODIGO = RPS.RPS_CODIGO ");
                    }
                    break;

                case "StatusCTe":
                    if (!select.Contains("StatusCTe"))
                    {
                        select.Append(
                            @"StatusCTe = CASE CTeOriginal.CON_STATUS 
		                        WHEN 'A' THEN 'Autorizado' 
		                        WHEN 'P' THEN 'Pendente' 
		                        WHEN 'E' THEN 'Enviado' 
		                        WHEN 'R' THEN 'Rejeitado' 
		                        WHEN 'C' THEN 'Cancelado' 
		                        WHEN 'I' THEN 'Inutilizado' 
		                        WHEN 'D' THEN 'Denegado' 
		                        WHEN 'S' THEN 'Em Digitação' 
		                        WHEN 'K' THEN 'Em Cancelamento' 
		                        WHEN 'L' THEN 'Em Inutilização' 
                                WHEN 'Z' THEN 'Anulado' 
		                        ELSE ''
                            END, "
                        );

                        groupBy.Append("CTeOriginal.CON_STATUS, ");
                    }
                    break;

                case "NumeroCargaAgrupamento":
                    if (!select.Contains("NumeroCargaAgrupamento"))
                    {
                        select.Append("substring((select distinct ', ' + Carga.CAR_CODIGO_CARGA_EMBARCADOR from T_CARGA_CTE CargaCTe inner join T_CARGA Carga on Carga.CAR_CODIGO = CargaCTe.CAR_CODIGO where CargaCTe.CON_CODIGO = CTeOriginal.CON_CODIGO for xml path('')), 3, 200) NumeroCargaAgrupamento, ");

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append("CTeOriginal.CON_CODIGO, ");
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
                                 WHERE _cargaCTe.CON_CODIGO = CTeOriginal.CON_CODIGO for xml path('')), 3, 200
                            ) DataCriacaoCarga, "
                        );

                        if (!groupBy.Contains(" CTeOriginal.CON_CODIGO"))
                            groupBy.Append(" CTeOriginal.CON_CODIGO, ");
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
                                 where _cargaCTe.CON_CODIGO = CTeOriginal.CON_CODIGO
                                   for xml path('')), 3, 200
                            ) SituacaoCarga, "
                        );

                        if (!groupBy.Contains(" CTeOriginal.CON_CODIGO"))
                            groupBy.Append(" CTeOriginal.CON_CODIGO, ");
                    }
                    break;

                case "ModeloVeicular":
                    if (!select.Contains("ModeloVeicular"))
                    {
                        select.Append("substring((select distinct ', ' + Modelo.MVC_DESCRICAO from T_CARGA_CTE CargaCTe inner join T_CARGA Carga on Carga.CAR_CODIGO = CargaCTe.CAR_CODIGO_ORIGEM join T_MODELO_VEICULAR_CARGA Modelo on Modelo.MVC_CODIGO = Carga.MVC_CODIGO where CargaCTe.CON_CODIGO = CTeOriginal.CON_CODIGO for xml path('')), 3, 200) ModeloVeicular, ");

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append("CTeOriginal.CON_CODIGO, ");
                    }
                    break;

                case "PreCarga":
                    if (!select.Contains("PreCarga"))
                    {
                        select.Append("substring((select distinct ', ' + PreCarga.PCA_NUMERO_CARGA from T_CARGA_CTE CargaCTe inner join T_PRE_CARGA PreCarga on PreCarga.CAR_CODIGO = CargaCTe.CAR_CODIGO_ORIGEM where CargaCTe.CON_CODIGO = CTeOriginal.CON_CODIGO for xml path('')), 3, 200) PreCarga, ");

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append("CTeOriginal.CON_CODIGO, ");
                    }
                    break;

                case "Operador":
                    if (!select.Contains("Operador"))
                    {
                        select.Append("substring((select distinct ', ' + Operador.FUN_NOME from T_CARGA_CTE CargaCTe inner join T_CARGA Carga on Carga.CAR_CODIGO = CargaCTe.CAR_CODIGO_ORIGEM inner join T_FUNCIONARIO Operador on Carga.CAR_OPERADOR = Operador.FUN_CODIGO where CargaCTe.CON_CODIGO = CTeOriginal.CON_CODIGO for xml path('')), 3, 200) Operador, ");

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append("CTeOriginal.CON_CODIGO, ");
                    }
                    break;

                case "ContratoFrete":
                    if (!select.Contains("ContratoFrete"))
                    {
                        select.Append("substring((select distinct ', ' + ContratoFreteTransportador.CFT_NUMERO_EMBARCADOR + ' - ' + ContratoFreteTransportador.CFT_DESCRICAO from T_CARGA_CTE CargaCTe inner join T_CARGA Carga on Carga.CAR_CODIGO = CargaCTe.CAR_CODIGO_ORIGEM inner join T_CONTRATO_FRETE_TRANSPORTADOR ContratoFreteTransportador on ContratoFreteTransportador.CFT_CODIGO = Carga.CFT_CODIGO where CargaCTe.CON_CODIGO = CTeOriginal.CON_CODIGO for xml path('')), 3, 200) ContratoFrete, ");

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append("CTeOriginal.CON_CODIGO, ");
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
                        select.Append("substring((select distinct ', ' + Pedido.PED_NUMERO_PEDIDO_EMBARCADOR from T_CARGA_CTE CargaCTe inner join T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE CargaPedidoXMLNotaFiscalCTe on CargaPedidoXMLNotaFiscalCTe.CCT_CODIGO = CargaCTe.CCT_CODIGO inner join T_PEDIDO_XML_NOTA_FISCAL PedidoXMLNotaFiscal on PedidoXMLNotaFiscal.PNF_CODIGO = CargaPedidoXMLNotaFiscalCTe.PNF_CODIGO inner join T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = PedidoXMLNotaFiscal.CPE_CODIGO inner join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO where CargaCTe.CON_CODIGO = CTeOriginal.CON_CODIGO for xml path('')), 3, 200) NumeroPedido, ");

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append("CTeOriginal.CON_CODIGO, ");
                    }
                    break;

                case "NumeroPedidoInterno":
                    if (!select.Contains("NumeroPedidoInterno"))
                    {
                        select.Append("substring((select distinct ', ' + CONVERT(NVARCHAR(15), Pedido.PED_NUMERO) from T_CARGA_CTE CargaCTe inner join T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE CargaPedidoXMLNotaFiscalCTe on CargaPedidoXMLNotaFiscalCTe.CCT_CODIGO = CargaCTe.CCT_CODIGO inner join T_PEDIDO_XML_NOTA_FISCAL PedidoXMLNotaFiscal on PedidoXMLNotaFiscal.PNF_CODIGO = CargaPedidoXMLNotaFiscalCTe.PNF_CODIGO inner join T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = PedidoXMLNotaFiscal.CPE_CODIGO inner join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO where CargaCTe.CON_CODIGO = CTeOriginal.CON_CODIGO for xml path('')), 3, 200) NumeroPedidoInterno, ");

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append("CTeOriginal.CON_CODIGO, ");
                    }
                    break;

                case "CodigoReferencia":
                    if (!select.Contains("CodigoReferencia"))
                    {
                        select.Append("substring((select distinct ', ' + PedidoImportacao.PEI_CODIGO_REFERENCIA from T_CARGA_CTE CargaCTe inner join T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE CargaPedidoXMLNotaFiscalCTe on CargaPedidoXMLNotaFiscalCTe.CCT_CODIGO = CargaCTe.CCT_CODIGO inner join T_PEDIDO_XML_NOTA_FISCAL PedidoXMLNotaFiscal on PedidoXMLNotaFiscal.PNF_CODIGO = CargaPedidoXMLNotaFiscalCTe.PNF_CODIGO inner join T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = PedidoXMLNotaFiscal.CPE_CODIGO inner join T_PEDIDO_IMPORTACAO PedidoImportacao on PedidoImportacao.PED_CODIGO = CargaPedido.PED_CODIGO where CargaCTe.CON_CODIGO = CTeOriginal.CON_CODIGO for xml path('')), 3, 200) CodigoReferencia, ");

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append("CTeOriginal.CON_CODIGO, ");
                    }
                    break;

                case "CodigoImportacao":
                    if (!select.Contains("CodigoImportacao"))
                    {
                        select.Append("substring((select distinct ', ' + PedidoImportacao.PEI_CODIGO_IMPORTACAO from T_CARGA_CTE CargaCTe inner join T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE CargaPedidoXMLNotaFiscalCTe on CargaPedidoXMLNotaFiscalCTe.CCT_CODIGO = CargaCTe.CCT_CODIGO inner join T_PEDIDO_XML_NOTA_FISCAL PedidoXMLNotaFiscal on PedidoXMLNotaFiscal.PNF_CODIGO = CargaPedidoXMLNotaFiscalCTe.PNF_CODIGO inner join T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = PedidoXMLNotaFiscal.CPE_CODIGO inner join T_PEDIDO_IMPORTACAO PedidoImportacao on PedidoImportacao.PED_CODIGO = CargaPedido.PED_CODIGO where CargaCTe.CON_CODIGO = CTeOriginal.CON_CODIGO for xml path('')), 3, 200) CodigoImportacao, ");

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append("CTeOriginal.CON_CODIGO, ");
                    }
                    break;

                case "DataColeta":
                    if (!select.Contains("DataColeta"))
                    {
                        select.Append("substring((select distinct ', ' + CONVERT(NVARCHAR(10), Pedido.CAR_DATA_CARREGAMENTO_PEDIDO, 103) + ' ' + CONVERT(NVARCHAR(5), Pedido.CAR_DATA_CARREGAMENTO_PEDIDO, 108) from T_CARGA_CTE CargaCTe inner join T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE CargaPedidoXMLNotaFiscalCTe on CargaPedidoXMLNotaFiscalCTe.CCT_CODIGO = CargaCTe.CCT_CODIGO inner join T_PEDIDO_XML_NOTA_FISCAL PedidoXMLNotaFiscal on PedidoXMLNotaFiscal.PNF_CODIGO = CargaPedidoXMLNotaFiscalCTe.PNF_CODIGO inner join T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = PedidoXMLNotaFiscal.CPE_CODIGO inner join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO where CargaCTe.CON_CODIGO = CTeOriginal.CON_CODIGO for xml path('')), 3, 200) DataColeta, ");

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append("CTeOriginal.CON_CODIGO, ");
                    }
                    break;

                case "TipoDeCarga":
                    if (!select.Contains("TipoDeCarga"))
                    {
                        select.Append("substring((select distinct ', ' + TipoDeCarga.TCG_DESCRICAO from T_CARGA_CTE CargaCTe inner join T_CARGA Carga on Carga.CAR_CODIGO = CargaCTe.CAR_CODIGO inner join T_TIPO_DE_CARGA TipoDeCarga on TipoDeCarga.TCG_CODIGO = Carga.TCG_CODIGO where CargaCTe.CON_CODIGO = CTeOriginal.CON_CODIGO for xml path('')), 3, 200) TipoDeCarga, ");

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append("CTeOriginal.CON_CODIGO, ");
                    }
                    break;

                case "TipoOperacao":
                    if (!select.Contains("TipoOperacao"))
                    {
                        select.Append("substring((select distinct ', ' + TipoOperacao.TOP_DESCRICAO from T_CARGA_CTE CargaCTe inner join T_CARGA Carga on Carga.CAR_CODIGO = CargaCTe.CAR_CODIGO_ORIGEM inner join T_TIPO_OPERACAO TipoOperacao on TipoOperacao.TOP_CODIGO = Carga.TOP_CODIGO where CargaCTe.CON_CODIGO = CTeOriginal.CON_CODIGO for xml path('')), 3, 200) TipoOperacao, ");

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append("CTeOriginal.CON_CODIGO, ");
                    }
                    break;

                case "DataEntrega":
                    if (!select.Contains("DataEntrega"))
                    {
                        select.Append("CONVERT(NVARCHAR(10), CTeOriginal.CON_DATA_ENTREGA, 103) + ' ' + CONVERT(NVARCHAR(5), CTeOriginal.CON_DATA_ENTREGA, 108) DataEntrega, ");
                        groupBy.Append("CTeOriginal.CON_DATA_ENTREGA, ");
                    }
                    break;

                case "DataEmissao":
                case "AnoEmissao":
                case "MesEmissao":
                case "DataEmissaoFormatada":
                    if (!select.Contains("DataEmissao"))
                    {
                        select.Append("CTeOriginal.CON_DATAHORAEMISSAO DataEmissao, ");
                        groupBy.Append("CTeOriginal.CON_DATAHORAEMISSAO, ");
                    }
                    break;

                case "CpfMotorista":
                    if (!select.Contains("CpfMotorista"))
                    {
                        select.Append("substring((select ', ' + motoristaCTe1.CMO_CPF_MOTORISTA from T_CTE_MOTORISTA motoristaCTe1 where motoristaCTe1.CON_CODIGO = CTeOriginal.CON_CODIGO for xml path('')), 3, 1000) CpfMotorista, ");
                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append("CTeOriginal.CON_CODIGO, ");
                    }
                    break;

                case "DataAutorizacao":
                case "DataAutorizacaoFormatada":
                    if (!select.Contains("DataAutorizacao"))
                    {
                        select.Append("CTeOriginal.CON_DATA_AUTORIZACAO DataAutorizacao, ");
                        groupBy.Append("CTeOriginal.CON_DATA_AUTORIZACAO, ");
                    }
                    break;

                case "DataCancelamento":
                    if (!select.Contains("DataCancelamento"))
                    {
                        select.Append("CASE CTeOriginal.CON_DATA_CANCELAMENTO WHEN NULL THEN '' ELSE convert(nvarchar(10), CTeOriginal.CON_DATA_CANCELAMENTO, 3) + ' ' + convert(nvarchar(10), CTeOriginal.CON_DATA_CANCELAMENTO, 108) END DataCancelamento, ");
                        groupBy.Append("CTeOriginal.CON_DATA_CANCELAMENTO, ");
                    }
                    break;

                case "DataAnulacao":
                    if (!select.Contains("DataAnulacao"))
                    {
                        select.Append("CASE CTeOriginal.CON_DATA_ANULACAO WHEN NULL THEN '' ELSE convert(nvarchar(10), CTeOriginal.CON_DATA_ANULACAO, 3) + ' ' + convert(nvarchar(10), CTeOriginal.CON_DATA_ANULACAO, 108) END DataAnulacao, ");
                        groupBy.Append("CTeOriginal.CON_DATA_ANULACAO, ");
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
                        select.Append("(select TOP 1 CASE CargaCTe.CCT_DATA_VINCULO_CARGA WHEN NULL THEN '' ELSE convert(nvarchar(10), CargaCTe.CCT_DATA_VINCULO_CARGA, 3) + ' ' + convert(nvarchar(10), CargaCTe.CCT_DATA_VINCULO_CARGA, 108) END from T_CARGA_CTE CargaCTe where CargaCTe.CON_CODIGO = CTeOriginal.CON_CODIGO) DataVinculoCarga, ");

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append("CTeOriginal.CON_CODIGO, ");
                    }
                    break;

                case "DescricaoStatusTitulo":
                    if (!select.Contains(" StatusTitulo, "))
                    {
                        select.Append("TituloCTe.TIT_STATUS StatusTitulo, ");
                        groupBy.Append("TituloCTe.TIT_STATUS, CTeOriginal.TIT_CODIGO, ");

                        SetarJoinsTitulo(joins);
                    }
                    break;

                case "DataVencimento":
                    if (!select.Contains(" DataVencimento,"))
                    {
                        select.Append("substring((select distinct ', ' + Convert(nvarchar(10), Titulo.TIT_DATA_VENCIMENTO, 103) from T_TITULO_DOCUMENTO TituloDocumento inner join T_TITULO Titulo on TituloDocumento.TIT_CODIGO = Titulo.TIT_CODIGO where Titulo.TIT_STATUS <> 4 and TituloDocumento.CON_CODIGO = CTeOriginal.CON_CODIGO for xml path('')), 3, 200) DataVencimento, ");

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append("CTeOriginal.CON_CODIGO, ");
                    }
                    break;

                case "NumeroPreFatura":
                    if (!select.Contains("NumeroPreFatura"))
                    {
                        select.Append("reverse(stuff(reverse((CASE WHEN Fatura.FAT_NUMERO_PRE_FATURA is null THEN '' ELSE CONVERT(nvarchar(20), Fatura.FAT_NUMERO_PRE_FATURA) + ', ' END) + isnull((select convert(nvarchar(20), Fatura.FAT_NUMERO_PRE_FATURA) + ', ' from T_DOCUMENTO_FATURAMENTO DocumentoFaturamento inner join T_FATURA_DOCUMENTO FaturaDocumento on DocumentoFaturamento.DFA_CODIGO = FaturaDocumento.DFA_CODIGO inner join T_FATURA Fatura on Fatura.FAT_CODIGO = FaturaDocumento.FAT_CODIGO where Fatura.FAT_SITUACAO <> 3 and (DocumentoFaturamento.CON_CODIGO = CTeOriginal.CON_CODIGO or DocumentoFaturamento.CAR_CODIGO in (select CAR_CODIGO from T_CARGA_CTE CargaCTe where CargaCTe.CON_CODIGO = CTeOriginal.CON_CODIGO)) for xml path('')), '')), 1, 2, '')) NumeroPreFatura, ");

                        if (!groupBy.Contains("Fatura.FAT_NUMERO_PRE_FATURA"))
                            groupBy.Append("Fatura.FAT_NUMERO_PRE_FATURA, ");

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append("CTeOriginal.CON_CODIGO, ");

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
                        select.Append("ClienteDestinatario.CLI_CODIGO_INTEGRACAO CodigoDestinatario, ");

                        if (!groupBy.Contains("ClienteDestinatario.CLI_CODIGO_INTEGRACAO"))
                            groupBy.Append("ClienteDestinatario.CLI_CODIGO_INTEGRACAO, ");

                        SetarJoinsDestinatarioCliente(joins);
                    }
                    break;

                case "CPFCNPJDestinatario":
                    if (!select.Contains(" CPFCNPJDestinatario, "))
                    {
                        select.Append("DestinatarioCTe.PCT_CPF_CNPJ CPFCNPJDestinatario, ");

                        if (!groupBy.Contains("DestinatarioCTe.PCT_CPF_CNPJ"))
                            groupBy.Append("DestinatarioCTe.PCT_CPF_CNPJ, ");

                        SetarJoinsDestinatario(joins);
                    }
                    break;

                case "IEDestinatario":
                    if (!select.Contains(" IEDestinatario, "))
                    {
                        select.Append("DestinatarioCTe.PCT_IERG IEDestinatario, ");
                        groupBy.Append("DestinatarioCTe.PCT_IERG, ");

                        SetarJoinsDestinatario(joins);
                    }
                    break;

                case "Destinatario":
                    if (!select.Contains(" Destinatario, "))
                    {
                        select.Append("DestinatarioCTe.PCT_NOME Destinatario, ");

                        if (!groupBy.Contains("DestinatarioCTe.PCT_NOME"))
                            groupBy.Append("DestinatarioCTe.PCT_NOME, ");

                        SetarJoinsDestinatario(joins);
                    }
                    break;

                case "CodigoEnderecoDestinatario":
                    if (!select.Contains(" CodigoEnderecoDestinatario, "))
                    {
                        select.Append("DestinatarioCTe.PCT_CODIGO_ENDERECO_INTEGRACAO CodigoEnderecoDestinatario, ");

                        if (!groupBy.Contains("DestinatarioCTe.PCT_CODIGO_ENDERECO_INTEGRACAO"))
                            groupBy.Append("DestinatarioCTe.PCT_CODIGO_ENDERECO_INTEGRACAO, ");

                        SetarJoinsDestinatario(joins);
                    }
                    break;

                case "EnderecoDestinatario":
                    if (!select.Contains(" EnderecoDestinatario, "))
                    {
                        select.Append("DestinatarioCTe.PCT_ENDERECO EnderecoDestinatario, ");

                        if (!groupBy.Contains("DestinatarioCTe.PCT_ENDERECO"))
                            groupBy.Append("DestinatarioCTe.PCT_ENDERECO, ");

                        SetarJoinsDestinatario(joins);
                    }
                    break;

                case "BairroDestinatario":
                    if (!select.Contains(" BairroDestinatario, "))
                    {
                        select.Append("DestinatarioCTe.PCT_BAIRRO BairroDestinatario, ");

                        if (!groupBy.Contains("DestinatarioCTe.PCT_BAIRRO"))
                            groupBy.Append("DestinatarioCTe.PCT_BAIRRO, ");

                        SetarJoinsDestinatario(joins);
                    }
                    break;

                case "CEPDestinatario":
                    if (!select.Contains(" CEPDestinatario, "))
                    {
                        select.Append("DestinatarioCTe.PCT_CEP CEPDestinatario, ");

                        if (!groupBy.Contains("DestinatarioCTe.PCT_CEP"))
                            groupBy.Append("DestinatarioCTe.PCT_CEP, ");

                        SetarJoinsDestinatario(joins);
                    }
                    break;

                case "LocalidadeDestinatario":
                    if (!select.Contains(" LocalidadeDestinatario, "))
                    {
                        select.Append("LocalidadeDestinatario.LOC_DESCRICAO + '-' + LocalidadeDestinatario.UF_SIGLA LocalidadeDestinatario, ");

                        if (!groupBy.Contains("LocalidadeDestinatario.LOC_DESCRICAO"))
                            groupBy.Append("LocalidadeDestinatario.LOC_DESCRICAO, ");

                        if (!groupBy.Contains("LocalidadeDestinatario.UF_SIGLA"))
                            groupBy.Append("LocalidadeDestinatario.UF_SIGLA, ");

                        SetarJoinsDestinatarioLocalidade(joins);
                    }
                    break;

                case "UFDestinatario":
                    if (!select.Contains(" UFDestinatario, "))
                    {
                        select.Append("LocalidadeDestinatario.UF_SIGLA UFDestinatario, ");

                        if (!groupBy.Contains("LocalidadeDestinatario.UF_SIGLA"))
                            groupBy.Append("LocalidadeDestinatario.UF_SIGLA, ");

                        SetarJoinsDestinatarioLocalidade(joins);
                    }
                    break;

                case "GrupoDestinatario":
                    if (!select.Contains(" GrupoDestinatario, "))
                    {
                        select.Append("GrupoPessoaDestinatario.GRP_DESCRICAO GrupoDestinatario, ");

                        if (!groupBy.Contains("GrupoPessoaDestinatario.GRP_DESCRICAO"))
                            groupBy.Append("GrupoPessoaDestinatario.GRP_DESCRICAO, ");

                        SetarJoinsDestinatarioGrupoPessoa(joins);
                    }
                    break;

                case "CategoriaDestinatario":
                    if (!select.Contains(" CategoriaDestinatario, "))
                    {
                        select.Append("CategoriaDestinatario.CTP_DESCRICAO CategoriaDestinatario, ");

                        if (!groupBy.Contains("CategoriaDestinatario.CTP_DESCRICAO"))
                            groupBy.Append("CategoriaDestinatario.CTP_DESCRICAO, ");

                        SetarJoinsDestinatarioCategoria(joins);
                    }
                    break;

                case "CodigoDocumentoDestinatario":
                    if (!select.Contains(" CodigoDocumentoDestinatario, "))
                    {
                        select.Append("ClienteDestinatario.CLI_CODIGO_DOCUMENTO CodigoDocumentoDestinatario, ");

                        if (!groupBy.Contains("ClienteDestinatario.CLI_CODIGO_DOCUMENTO"))
                            groupBy.Append("ClienteDestinatario.CLI_CODIGO_DOCUMENTO, ");

                        SetarJoinsDestinatarioCliente(joins);
                    }
                    break;

                case "CPFCNPJTomador":
                    if (!select.Contains(" CPFCNPJTomador, "))
                    {
                        select.Append("TomadorPagadorCTe.PCT_CPF_CNPJ CPFCNPJTomador, ");

                        if (!groupBy.Contains("TomadorPagadorCTe.PCT_CPF_CNPJ"))
                            groupBy.Append("TomadorPagadorCTe.PCT_CPF_CNPJ, ");

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

                        if (!groupBy.Contains("CTeOriginal.CON_TOMADOR_PAGADOR_CTE"))
                            groupBy.Append("CTeOriginal.CON_TOMADOR_PAGADOR_CTE, ");

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
                                   AND CX.CON_CODIGO = CTeOriginal.CON_CODIGO
                                   for xml path('')
                            ), 3, 1000) Rotas,"
                        );

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append("CTeOriginal.CON_CODIGO, ");
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

                case "AliquotaICMS":
                    if (!somenteContarNumeroRegistros && !select.Contains("AliquotaICMS"))
                    {
                        select.Append("CTeOriginal.CON_ALIQ_ICMS AliquotaICMS, ");

                        if (!groupBy.Contains("CTeOriginal.CON_ALIQ_ICMS"))
                            groupBy.Append("CTeOriginal.CON_ALIQ_ICMS, ");
                    }
                    break;

                case "BaseCalculoICMS":
                    if (!somenteContarNumeroRegistros && !select.Contains("BaseCalculoICMS"))
                        select.Append("SUM(CTeOriginal.CON_BC_ICMS) BaseCalculoICMS, ");
                    break;

                case "ValorISSRetido":
                    if (!somenteContarNumeroRegistros && !select.Contains("ValorISSRetido"))
                        select.Append("SUM(CTeOriginal.CON_VALOR_ISS_RETIDO) ValorISSRetido, ");
                    break;

                case "ValorPrestacao":
                    if (!somenteContarNumeroRegistros && !select.Contains("ValorPrestacao"))
                        select.Append("SUM(CTeOriginal.CON_VALOR_PREST_SERVICO) ValorPrestacao, ");
                    break;

                case "ValorSemImposto":
                    if (!somenteContarNumeroRegistros && !select.Contains("ValorSemImposto"))
                        select.Append("SUM(CTeOriginal.CON_VALOR_PREST_SERVICO - CTeOriginal.CON_VAL_ICMS - CTeOriginal.CON_VALOR_ISS) ValorSemImposto, ");
                    break;

                case "ValorMercadoria":
                    if (!somenteContarNumeroRegistros && !select.Contains("ValorMercadoria"))
                        select.Append("SUM(CTeOriginal.CON_VALOR_TOTAL_MERC) ValorMercadoria, ");
                    break;

                case "VeiculoUltimoMDFe":
                    if (!select.Contains(" VeiculoUltimoMDFe,"))
                    {
                        select.Append("(select TOP 1 MDFeVeiculo.MDV_PLACA from T_MDFE_MUNICIPIO_DESCARREGAMENTO_DOC MDFeMunicipioDescarregamentoDoc inner join T_MDFE_MUNICIPIO_DESCARREGAMENTO MDFeMunicipioDescarregamento on MDFeMunicipioDescarregamento.MDD_CODIGO = MDFeMunicipioDescarregamentoDoc.MDD_CODIGO inner join T_MDFE MDFe on MDFe.MDF_CODIGO = MDFeMunicipioDescarregamento.MDF_CODIGO inner join T_MDFE_VEICULO MDFeVeiculo on MDFeVeiculo.MDF_CODIGO = MDFe.MDF_CODIGO WHERE CON_CODIGO = CTeOriginal.CON_CODIGO ORDER BY MDFe.MDF_DATA_EMISSAO DESC) VeiculoUltimoMDFe, ");

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append("CTeOriginal.CON_CODIGO, ");
                    }
                    break;

                case "NumeroUltimoMDFe":
                    if (!select.Contains(" NumeroUltimoMDFe,"))
                    {
                        select.Append("(select TOP 1 CONVERT(NVARCHAR(50), MDFe.MDF_NUMERO) from T_MDFE_MUNICIPIO_DESCARREGAMENTO_DOC MDFeMunicipioDescarregamentoDoc inner join T_MDFE_MUNICIPIO_DESCARREGAMENTO MDFeMunicipioDescarregamento on MDFeMunicipioDescarregamento.MDD_CODIGO = MDFeMunicipioDescarregamentoDoc.MDD_CODIGO inner join T_MDFE MDFe on MDFe.MDF_CODIGO = MDFeMunicipioDescarregamento.MDF_CODIGO WHERE CON_CODIGO = CTeOriginal.CON_CODIGO ORDER BY MDFe.MDF_DATA_EMISSAO DESC) NumeroUltimoMDFe, ");

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append("CTeOriginal.CON_CODIGO, ");
                    }
                    break;

                case "DescricaoUltimaOcorrencia":
                    if (!select.Contains(" DescricaoUltimaOcorrencia,"))
                    {
                        select.Append("(select TOP 1 Ocorrencia.OCO_DESCRICAO from T_CTE_OCORRENCIA OcorrenciaCTe inner join T_OCORRENCIA Ocorrencia on Ocorrencia.OCO_CODIGO = OcorrenciaCTe.OCO_CODIGO WHERE CON_CODIGO = CTeOriginal.CON_CODIGO ORDER BY OcorrenciaCTe.COC_DATA_OCORRENCIA DESC) DescricaoUltimaOcorrencia, ");

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append("CTeOriginal.CON_CODIGO, ");
                    }
                    break;

                case "DataOcorrenciaFinal":
                    if (!select.Contains(" DataOcorrenciaFinal,"))
                    {
                        select.Append("(select TOP 1 CONVERT(nvarchar(10), OcorrenciaCTe.COC_DATA_OCORRENCIA, 103) + ' ' + CONVERT(nvarchar(5), OcorrenciaCTe.COC_DATA_OCORRENCIA, 108) from T_CTE_OCORRENCIA OcorrenciaCTe inner join T_OCORRENCIA Ocorrencia on Ocorrencia.OCO_CODIGO = OcorrenciaCTe.OCO_CODIGO WHERE OcorrenciaCTe.CON_CODIGO = CTeOriginal.CON_CODIGO AND Ocorrencia.OCO_TIPO = 'F' ORDER BY OcorrenciaCTe.COC_DATA_OCORRENCIA DESC) DataOcorrenciaFinal, ");

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append("CTeOriginal.CON_CODIGO, ");
                    }
                    break;

                case "Ocorrencia":
                    if (!select.Contains(" Ocorrencia, "))
                    {

                        select.Append("CONVERT(varchar(100), Ocorrencia.COC_NUMERO_CONTRATO) Ocorrencia, ");
                        groupBy.Append("Ocorrencia.COC_NUMERO_CONTRATO, ");

                        SetarJoinsOcorrencia(joins);
                    }
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
                        select.Append("(select TOP(1) ProprietarioVeiculoCTe.PVE_NOME from T_CTE_VEICULO_PROPRIETARIO ProprietarioVeiculoCTe inner join T_CTE_VEICULO VeiculoCTe on ProprietarioVeiculoCTe.PVE_CODIGO = VeiculoCTe.PVE_CODIGO where VeiculoCTe.CON_CODIGO = CTeOriginal.CON_CODIGO) NomeProprietarioVeiculo, ");

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append("CTeOriginal.CON_CODIGO, ");
                    }
                    break;

                case "SegmentoVeiculo":
                    if (!select.Contains(" SegmentoVeiculo "))
                    {
                        select.Append("substring((select ', ' + SegmentoVeiculo.VSE_DESCRICAO from T_CTE_VEICULO VeiculoCTe inner join T_VEICULO Veiculo on Veiculo.VEI_CODIGO = VeiculoCTe.VEI_CODIGO inner join T_VEICULO_SEGMENTO SegmentoVeiculo on SegmentoVeiculo.VSE_CODIGO = Veiculo.VSE_CODIGO where VeiculoCTe.CON_CODIGO = CTeOriginal.CON_CODIGO for xml path('')), 3, 1000) SegmentoVeiculo, ");

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append("CTeOriginal.CON_CODIGO, ");
                    }
                    break;

                case "Motorista":
                    if (!select.Contains("Motorista"))
                    {
                        select.Append("substring((select ', ' + motoristaCTe1.CMO_NOME_MOTORISTA from T_CTE_MOTORISTA motoristaCTe1 where motoristaCTe1.CON_CODIGO = CTeOriginal.CON_CODIGO for xml path('')), 3, 1000) Motorista, ");

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append("CTeOriginal.CON_CODIGO, ");
                    }
                    break;

                case "PesoKg":
                    if (!select.Contains("PesoKg"))
                    {
                        select.Append("CASE WHEN CTeOriginal.CON_PESO > 0 THEN CTeOriginal.CON_PESO ELSE (select SUM(pesoKgCTe.ICA_QTD) from T_CTE_INF_CARGA pesoKgCTe where pesoKgCTe.ICA_UN = '01' and pesoKgCTe.CON_CODIGO = CTeOriginal.CON_CODIGO ) END PesoKg, ");

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append("CTeOriginal.CON_CODIGO, ");

                        if (!groupBy.Contains("CTeOriginal.CON_PESO,"))
                            groupBy.Append("CTeOriginal.CON_PESO, ");
                    }
                    break;

                case "PesoLiquidoKg":
                    if (!select.Contains("PesoLiquidoKg"))
                    {
                        select.Append("CTeOriginal.CON_PESO_LIQUIDO PesoLiquidoKg, ");

                        if (!groupBy.Contains("CTeOriginal.CON_PESO_LIQUIDO"))
                            groupBy.Append("CTeOriginal.CON_PESO_LIQUIDO, ");
                    }
                    break;

                case "Volumes":
                    if (!select.Contains("Volumes"))
                    {
                        select.Append(" CASE WHEN CTeOriginal.CON_VOLUMES > 0 THEN CONVERT(int, CON_VOLUMES) ELSE (SELECT SUM(NotasFiscaisCTe.NFC_VOLUME) FROM T_CTE_DOCS NotasFiscaisCTe WHERE NotasFiscaisCTe.CON_CODIGO = CTeOriginal.CON_CODIGO) END Volumes, ");

                        if (!groupBy.Contains("CTeOriginal.CON_VOLUMES"))
                            groupBy.Append("CTeOriginal.CON_VOLUMES, ");

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append("CTeOriginal.CON_CODIGO, ");
                    }
                    break;

                case "MetrosCubicos":
                    if (!select.Contains("MetrosCubicos"))
                    {
                        select.Append(" SUM(CTeOriginal.CON_METROS_CUBICOS) MetrosCubicos, ");
                    }
                    break;

                case "Pallets":
                    if (!select.Contains("Pallets"))
                    {
                        select.Append(" SUM(CTeOriginal.CON_PALLETS) Pallets, ");
                    }
                    break;

                case "Observacao":
                    if (!select.Contains("Observacao"))
                    {
                        select.Append("CTeOriginal.CON_OBSGERAIS Observacao, ");
                        groupBy.Append("CTeOriginal.CON_OBSGERAIS, ");
                    }
                    break;

                case "NumeroMinuta":
                    if (!select.Contains("NumeroMinuta"))
                    {
                        select.Append("COALESCE(DocumentoNatura.DTN_NUMERO, DocumentoAvon.MAV_NUMERO) NumeroMinuta, ");

                        joins.Append(" left outer join T_NATURA_DOCUMENTO_TRANSPORTE_NOTA_FISCAL NotaFiscalNatura on NotaFiscalNatura.NDT_CODIGO = (select TOP 1 NDT_CODIGO FROM T_NATURA_DOCUMENTO_TRANSPORTE_NOTA_FISCAL where CON_CODIGO = CTeOriginal.CON_CODIGO) ");
                        joins.Append(" left outer join T_NATURA_DOCUMENTO_TRANSPORTE DocumentoNatura on NotaFiscalNatura.DTN_CODIGO = DocumentoNatura.DTN_CODIGO ");
                        joins.Append(" left outer join T_AVON_MANIFESTO_DOCUMENTO NotaFiscalAvon on NotaFiscalAvon.CON_CODIGO = CTeOriginal.CON_CODIGO ");
                        joins.Append(" left outer join T_AVON_MANIFESTO DocumentoAvon on DocumentoAvon.MAV_CODIGO = NotaFiscalAvon.MAV_CODIGO ");

                        groupBy.Append("DocumentoAvon.MAV_NUMERO, DocumentoNatura.DTN_NUMERO, ");
                    }
                    break;

                case "NumeroNotaFiscal":
                    if (!select.Contains("NumeroNotaFiscal"))
                    {
                        select.Append("substring((select ', ' + notaFiscal1.NFC_NUMERO from T_CTE_DOCS notaFiscal1 where notaFiscal1.CON_CODIGO = CTeOriginal.CON_CODIGO for xml path('')), 3, 100000) NumeroNotaFiscal, ");

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append("CTeOriginal.CON_CODIGO, ");
                    }
                    break;

                case "NumeroPedidoNotaFiscal":
                    if (!select.Contains("NumeroPedidoNotaFiscal"))
                    {
                        select.Append("substring((select ', ' + notaFiscal1.NFC_NUMERO_PEDIDO from T_CTE_DOCS notaFiscal1 where notaFiscal1.CON_CODIGO = CTeOriginal.CON_CODIGO for xml path('')), 3, 1000) NumeroPedidoNotaFiscal, ");

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append("CTeOriginal.CON_CODIGO, ");
                    }
                    break;

                case "ChaveNotaFiscal":
                    if (!select.Contains("ChaveNotaFiscal"))
                    {
                        select.Append("substring((select ', ' + notaFiscal2.NFC_CHAVENFE from T_CTE_DOCS notaFiscal2 where notaFiscal2.CON_CODIGO = CTeOriginal.CON_CODIGO for xml path('')), 3, 5000) ChaveNotaFiscal, ");

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append("CTeOriginal.CON_CODIGO, ");
                    }
                    break;

                case "DataNFEmissao":
                    if (!select.Contains(" DataNFEmissao, "))
                    {
                        select.Append("(select TOP 1 CASE notaFiscal1.NFC_DATAEMISSAO WHEN NULL THEN '' ELSE convert(nvarchar(10), notaFiscal1.NFC_DATAEMISSAO, 103) END from T_CTE_DOCS notaFiscal1 where notaFiscal1.CON_CODIGO = CTeOriginal.CON_CODIGO ORDER BY notaFiscal1.NFC_DATAEMISSAO) DataNFEmissao, ");

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append("CTeOriginal.CON_CODIGO, ");
                    }
                    break;

                case "NumeroDocumentoAnterior":
                    if (!select.Contains(" NumeroDocumentoAnterior, "))
                    {
                        select.Append("(select TOP 1 NFC_NUMERO from T_CTE_DOCS notaFiscal1 where notaFiscal1.CON_CODIGO = CTeOriginal.CON_CODIGO ORDER BY notaFiscal1.NFC_DATAEMISSAO) NumeroDocumentoAnterior, ");

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append("CTeOriginal.CON_CODIGO, ");
                    }
                    break;

                case "DescricaoTipoPagamento":
                    if (!select.Contains(" TipoPagamento, "))
                    {
                        select.Append("CTeOriginal.CON_PAGOAPAGAR as TipoPagamento, ");

                        if (!groupBy.Contains("CTeOriginal.CON_PAGOAPAGAR"))
                            groupBy.Append("CTeOriginal.CON_PAGOAPAGAR, ");
                    }
                    break;

                case "Frota":
                    if (!select.Contains(" Frota, "))
                    {
                        select.Append("reverse(stuff(reverse((select (case veiculo1.VEI_NUMERO_FROTA when null then '' when '' then '' else veiculo1.VEI_NUMERO_FROTA + ', ' end) from T_CTE_VEICULO veiculoCTe1 inner join T_VEICULO veiculo1 on veiculoCTe1.VEI_CODIGO = veiculo1.VEI_CODIGO where veiculoCTe1.CON_CODIGO = CTeOriginal.CON_CODIGO for xml path(''))), 1, 2, '')) Frota, ");

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append("CTeOriginal.CON_CODIGO, ");
                    }
                    break;

                case "Pago":
                    if (!select.Contains(" Pago"))
                    {
                        select.Append("CASE WHEN Fatura.FAT_SITUACAO = 2 THEN 'Sim' ELSE 'Não' END Pago, ");

                        if (!groupBy.Contains("Fatura.FAT_SITUACAO"))
                            groupBy.Append("Fatura.FAT_SITUACAO, ");

                        SetarJoinsFatura(joins);
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
                        select.Append("CTeOriginal.CON_CST CST, ");
                        groupBy.Append("CTeOriginal.CON_CST, ");
                    }
                    break;

                case "DataPrevistaEntrega":
                    if (!select.Contains(" DataPrevistaEntrega"))
                    {
                        select.Append("substring((select distinct ', ' + convert(varchar, Pedido.PED_PREVISAO_ENTREGA, 103) + ' ' + SUBSTRING(convert(varchar, Pedido.PED_PREVISAO_ENTREGA, 8), 0, 6) from T_CARGA_CTE CargaCTe inner join T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE CargaPedidoXMLNotaFiscalCTe on CargaPedidoXMLNotaFiscalCTe.CCT_CODIGO = CargaCTe.CCT_CODIGO inner join T_PEDIDO_XML_NOTA_FISCAL PedidoXMLNotaFiscal on PedidoXMLNotaFiscal.PNF_CODIGO = CargaPedidoXMLNotaFiscalCTe.PNF_CODIGO inner join T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = PedidoXMLNotaFiscal.CPE_CODIGO inner join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO where CargaCTe.CON_CODIGO = CTeOriginal.CON_CODIGO for xml path('')), 3, 200) DataPrevistaEntrega, ");

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append("CTeOriginal.CON_CODIGO, ");
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
                                 where XMLNotaFiscalCTe.CON_CODIGO = CTeOriginal.CON_CODIGO
                            ) NumeroDTNatura, "
                        );

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO,"))
                            groupBy.Append("CTeOriginal.CON_CODIGO, ");
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
                                 where CargaCTe.CON_CODIGO = CTeOriginal.CON_CODIGO
                            ) KmRodado, "
                        );

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO,"))
                            groupBy.Append("CTeOriginal.CON_CODIGO, ");
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
                                 where CargaCTe.CON_CODIGO = CTeOriginal.CON_CODIGO
                            ) ValorKMContrato, "
                        );

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO,"))
                            groupBy.Append("CTeOriginal.CON_CODIGO, ");
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
                                 where CargaCTe.CON_CODIGO = CTeOriginal.CON_CODIGO
                            ) ValorKMExcedenteContrato, "
                        );

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO,"))
                            groupBy.Append("CTeOriginal.CON_CODIGO, ");
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
                                 where _cargaCTe.CON_CODIGO = CTeOriginal.CON_CODIGO
                            ) ValorFreteFranquiaKM, "
                        );

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO,"))
                            groupBy.Append("CTeOriginal.CON_CODIGO, ");
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
                                 where _cargaCTe.CON_CODIGO = CTeOriginal.CON_CODIGO
                            ) ValorFreteFranquiaKMExcedido, "
                        );

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO,"))
                            groupBy.Append("CTeOriginal.CON_CODIGO, ");
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
                                 where _cargaCTe.CON_CODIGO = CTeOriginal.CON_CODIGO
                            ) KmConsumido, "
                        );

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO,"))
                            groupBy.Append("CTeOriginal.CON_CODIGO, ");
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
                                 where _cargaCTe.CON_CODIGO = CTeOriginal.CON_CODIGO
                            ) KmConsumidoExcedente, "
                        );

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO,"))
                            groupBy.Append("CTeOriginal.CON_CODIGO, ");
                    }
                    break;

                case "NumeroOCADocumentoOriginario":
                    if (!select.Contains(" NumeroOCADocumentoOriginario,"))
                    {
                        select.Append("substring((select distinct ', ' + Convert(nvarchar(20), DocumentoOriginario.CDO_NUMERO_OPERACIONAL_CONHECIMENTO_AEREO) from T_CTE_DOCUMENTO_ORIGINARIO DocumentoOriginario where DocumentoOriginario.CON_CODIGO = CTeOriginal.CON_CODIGO for xml path('')), 3, 200) NumeroOCADocumentoOriginario, ");

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append("CTeOriginal.CON_CODIGO, ");
                    }
                    break;

                case "NumeroDI":
                    if (!select.Contains(" NumeroDI,"))
                    {
                        select.Append("substring((select distinct ', ' + PedidoImportacao.PEI_NUMERO_DI from T_CARGA_CTE CargaCTe inner join T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE CargaPedidoXMLNotaFiscalCTe on CargaPedidoXMLNotaFiscalCTe.CCT_CODIGO = CargaCTe.CCT_CODIGO inner join T_PEDIDO_XML_NOTA_FISCAL PedidoXMLNotaFiscal on PedidoXMLNotaFiscal.PNF_CODIGO = CargaPedidoXMLNotaFiscalCTe.PNF_CODIGO inner join T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = PedidoXMLNotaFiscal.CPE_CODIGO inner join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO inner join T_PEDIDO_IMPORTACAO PedidoImportacao on PedidoImportacao.PED_CODIGO = Pedido.PED_CODIGO where CargaCTe.CON_CODIGO = CTeOriginal.CON_CODIGO and LEN(PedidoImportacao.PEI_NUMERO_DI) > 0 for xml path('')), 3, 200) NumeroDI, ");

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append("CTeOriginal.CON_CODIGO, ");
                    }
                    break;
                case "NumeroDTA":
                    if (!select.Contains(" NumeroDTA,"))
                    {
                        select.Append("substring((select distinct ', ' + Pedido.PED_NUMERO_DTA from T_CARGA_CTE CargaCTe inner join T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE CargaPedidoXMLNotaFiscalCTe on CargaPedidoXMLNotaFiscalCTe.CCT_CODIGO = CargaCTe.CCT_CODIGO inner join T_PEDIDO_XML_NOTA_FISCAL PedidoXMLNotaFiscal on PedidoXMLNotaFiscal.PNF_CODIGO = CargaPedidoXMLNotaFiscalCTe.PNF_CODIGO inner join T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = PedidoXMLNotaFiscal.CPE_CODIGO inner join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO where CargaCTe.CON_CODIGO = CTeOriginal.CON_CODIGO and LEN(Pedido.PED_NUMERO_DTA) > 0 for xml path('')), 3, 200) NumeroDTA, ");

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append("CTeOriginal.CON_CODIGO, ");
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
                                 where _cargaCTe.CON_CODIGO = CTeOriginal.CON_CODIGO
                            ) ValorValePedagio,"
                        );

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append(" CTeOriginal.CON_CODIGO, ");
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
                                 where _cargaCTe.CON_CODIGO = CTeOriginal.CON_CODIGO
                                   for xml path('')
                            ), 3, 1000) NumeroValePedagio,"
                        );

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append(" CTeOriginal.CON_CODIGO, ");
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
                                 WHERE CargaCTe.CON_CODIGO = CTeOriginal.CON_CODIGO for xml path('')), 3, 1000) TabelaFrete, "
                        );

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append(" CTeOriginal.CON_CODIGO, ");
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
                                 WHERE CargaCTe.CON_CODIGO = CTeOriginal.CON_CODIGO for xml path('')), 3, 5000) TabelaFreteCliente, "
                            );

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append(" CTeOriginal.CON_CODIGO, ");
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

                        SetarJoinsPagamento(joins, filtrosPesquisa);
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
                                 WHERE DocumentoExportacao.CON_CODIGO = CTeOriginal.CON_CODIGO for xml path('')), 3, 1000) NumeroContabilizacao, "
                        );

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append(" CTeOriginal.CON_CODIGO, ");
                    }
                    break;

                case "NumeroBooking":
                    if (!select.Contains(" NumeroBooking, "))
                    {
                        select.Append("CTeOriginal.CON_NUMERO_BOOKING NumeroBooking, ");
                        groupBy.Append("CTeOriginal.CON_NUMERO_BOOKING, ");
                    }
                    break;

                case "NumeroOS":
                    if (!select.Contains(" NumeroOS, "))
                    {
                        select.Append("CTeOriginal.CON_NUMERO_OS NumeroOS, ");
                        groupBy.Append("CTeOriginal.CON_NUMERO_OS, ");
                    }
                    break;

                case "NumeroControle":
                    if (!select.Contains(" NumeroControle, "))
                    {
                        select.Append("CTeOriginal.CON_NUMERO_CONTROLE NumeroControle, ");
                        groupBy.Append("CTeOriginal.CON_NUMERO_CONTROLE, ");
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
                                 WHERE CargaCTe.CON_CODIGO = CTeOriginal.CON_CODIGO for xml path('')), 3, 1000) TipoProposta, "
                        );

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append(" CTeOriginal.CON_CODIGO, ");
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
                                 WHERE CargaCTe.CON_CODIGO = CTeOriginal.CON_CODIGO for xml path('')), 3, 1000) NumeroProposta, "
                        );

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append(" CTeOriginal.CON_CODIGO, ");
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
                                 WHERE CargaCTe.CON_CODIGO = CTeOriginal.CON_CODIGO for xml path('')), 3, 1000) TipoCarga, "
                        );

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append(" CTeOriginal.CON_CODIGO, ");
                    }
                    break;

                case "QuantidadeNF":
                    if (!select.Contains(" QuantidadeNF, "))
                    {
                        select.Append(
                            @"  (SELECT COUNT (1) from T_CTE_DOCS cteDocs 
                                 WHERE cteDocs.CON_CODIGO = CTeOriginal.CON_CODIGO) QuantidadeNF, "
                        );

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append(" CTeOriginal.CON_CODIGO, ");
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
                        select.Append("     where cteContainer.CON_CODIGO = CTeOriginal.CON_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) NumeroLacre, ");

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append(" CTeOriginal.CON_CODIGO, ");
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
                                 WHERE cteContainer.CON_CODIGO = CTeOriginal.CON_CODIGO for xml path('')), 3, 1000) Tara, "
                        );

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append(" CTeOriginal.CON_CODIGO, ");
                    }
                    break;

                case "Container":
                    if (!select.Contains(" Container, "))
                    {
                        select.Append(
                            @"SUBSTRING((
                                SELECT DISTINCT ', ' + container.CTR_DESCRICAO
                                        from T_CONTAINER container 
                                        inner join T_CTE_CONTAINER cteContainer on cteContainer.CTR_CODIGO = container.CTR_CODIGO 
                                 WHERE cteContainer.CON_CODIGO = CTeOriginal.CON_CODIGO for xml path('')), 3, 1000) Container, "
                        );

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append(" CTeOriginal.CON_CODIGO, ");
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
                                 WHERE cteContainer.CON_CODIGO = CTeOriginal.CON_CODIGO for xml path('')), 3, 1000) TipoContainer, "
                        );

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append(" CTeOriginal.CON_CODIGO, ");
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
                        select.Append("     where DF.CON_CODIGO = CTeOriginal.CON_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) NumeroFatura, ");

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append(" CTeOriginal.CON_CODIGO, ");
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
                        select.Append("     where DF.CON_CODIGO = CTeOriginal.CON_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) DataFatura, ");

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append(" CTeOriginal.CON_CODIGO, ");
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
                        select.Append("     where DF.CON_CODIGO = CTeOriginal.CON_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) NumeroBoleto, ");

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append(" CTeOriginal.CON_CODIGO, ");
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
                        select.Append("     where DF.CON_CODIGO = CTeOriginal.CON_CODIGO and T.TIT_NOSSO_NUMERO is not null and T.TIT_NOSSO_NUMERO <> '' ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) DataBoleto, ");

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append(" CTeOriginal.CON_CODIGO, ");
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
                                 WHERE CargaCTe.CON_CODIGO = CTeOriginal.CON_CODIGO for xml path('')), 3, 1000) NavioTransbordo, "
                        );

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append(" CTeOriginal.CON_CODIGO, ");
                    }
                    break;

                case "PossuiCartaCorrecao":
                    if (!select.Contains(" PossuiCartaCorrecao, "))
                    {
                        select.Append("CASE WHEN CTeOriginal.CON_POSSUI_CARTA_CORRECAO = 1 THEN 'Sim' ELSE 'Não' END PossuiCartaCorrecao, ");
                        groupBy.Append("CTeOriginal.CON_POSSUI_CARTA_CORRECAO, ");
                    }
                    break;

                case "FoiAnulado":
                    if (!select.Contains(" FoiAnulado, "))
                    {
                        select.Append("CASE WHEN CTeOriginal.CON_POSSUI_ANULACAO_SUBSTITUICAO = 1 THEN 'Sim' ELSE 'Não' END FoiAnulado, ");
                        groupBy.Append("CTeOriginal.CON_POSSUI_ANULACAO_SUBSTITUICAO, ");
                    }
                    break;

                case "PossuiCTeComplementar":
                    if (!select.Contains(" PossuiCTeComplementar, "))
                    {
                        select.Append("CASE WHEN CTeOriginal.CON_POSSUI_CTE_COMPLEMENTAR = 1 THEN 'Sim' ELSE 'Não' END PossuiCTeComplementar, ");
                        groupBy.Append("CTeOriginal.CON_POSSUI_CTE_COMPLEMENTAR, ");
                    }
                    break;

                case "FoiSubstituido":
                    if (!select.Contains(" FoiSubstituido, "))
                    {
                        select.Append(@"CASE WHEN (select count(1) from t_cte _cte where _cte.CON_CHAVE_CTE_SUB_COMP = CTeOriginal.CON_CHAVECTE) > 0 THEN 'Sim' 
                                        ELSE 'Não' END FoiSubstituido, ");

                        if (!groupBy.Contains("CTeOriginal.CON_CHAVECTE"))
                            groupBy.Append(" CTeOriginal.CON_CHAVECTE, ");
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
                        select.Append("CTeOriginal.CON_OBS_CANCELAMENTO MotivoCancelamento, ");
                        groupBy.Append("CTeOriginal.CON_OBS_CANCELAMENTO, ");
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
                        select.Append("SUM(CTeOriginal.CON_ALIQUOTA_ICMS_INTERNA) AliquotaICMSInterna, ");
                    break;

                case "PercentualICMSPartilha":
                    if (!somenteContarNumeroRegistros && !select.Contains("PercentualICMSPartilha"))
                        select.Append("AVG(CTeOriginal.CON_PERCENTUAL_ICMS_PARTILHA) PercentualICMSPartilha, ");
                    break;

                case "ValorICMSUFOrigem":
                    if (!somenteContarNumeroRegistros && !select.Contains("ValorICMSUFOrigem"))
                        select.Append("SUM(CTeOriginal.CON_VALOR_ICMS_UF_ORIGEM) ValorICMSUFOrigem, ");
                    break;

                case "ValorICMSUFDestino":
                    if (!somenteContarNumeroRegistros && !select.Contains("ValorICMSUFDestino"))
                        select.Append("SUM(CTeOriginal.CON_VALOR_ICMS_UF_DESTINO) ValorICMSUFDestino, ");
                    break;

                case "ValorICMSFCPFim":
                    if (!somenteContarNumeroRegistros && !select.Contains("ValorICMSFCPFim"))
                        select.Append("SUM(CTeOriginal.CON_VALOR_ICMS_FCP_DESTINO) ValorICMSFCPFim, ");
                    break;

                case "CaracteristicaTransporteCTe":
                    if (!select.Contains(" CaracteristicaTransporteCTe, "))
                    {
                        select.Append("CTeOriginal.CON_CARAC_TRANSP CaracteristicaTransporteCTe, ");
                        groupBy.Append("CTeOriginal.CON_CARAC_TRANSP, ");
                    }
                    break;

                case "ProdutoPredominante":
                    if (!select.Contains(" ProdutoPredominante, "))
                    {
                        select.Append("CTeOriginal.CON_PRODUTO_PRED ProdutoPredominante, ");
                        groupBy.Append("CTeOriginal.CON_PRODUTO_PRED, ");
                    }
                    break;

                case "CentroResultado":
                    if (!select.Contains(" CentroResultado, "))
                    {
                        select.Append("CentroResultado.CRE_DESCRICAO CentroResultado, ");

                        if (!groupBy.Contains("CentroResultado.CRE_DESCRICAO, "))
                            groupBy.Append(" CentroResultado.CRE_DESCRICAO, ");

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
                                 WHERE CargaCTe.CON_CODIGO = CTeOriginal.CON_CODIGO for xml path('')), 3, 1000) TipoServicoMultimodal, "
                        );

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append(" CTeOriginal.CON_CODIGO, ");
                    }
                    break;

                case "NumeroManifesto":
                    if (!select.Contains(" NumeroManifesto, "))
                    {
                        select.Append("CTeOriginal.CON_NUMERO_MANIFESTO NumeroManifesto, ");
                        groupBy.Append("CTeOriginal.CON_NUMERO_MANIFESTO, ");
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
                                 WHERE CargaCTe.CON_CODIGO = CTeOriginal.CON_CODIGO for xml path('')), 3, 1000) NumeroManifestoFeeder, "
                        );

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append(" CTeOriginal.CON_CODIGO, ");
                    }
                    break;

                case "NumeroCEMercante":
                    if (!select.Contains(" NumeroCEMercante, "))
                    {
                        select.Append("CTeOriginal.CON_NUMERO_CE_MERCANTE NumeroCEMercante, ");
                        groupBy.Append("CTeOriginal.CON_NUMERO_CE_MERCANTE, ");
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
                                 WHERE CargaCTe.CON_CODIGO = CTeOriginal.CON_CODIGO for xml path('')), 3, 1000) NumeroCEANTAQ, "
                        );

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append(" CTeOriginal.CON_CODIGO, ");
                    }
                    break;

                case "DescricaoAfretamento":
                    if (!select.Contains(" Afretamento, "))
                    {
                        select.Append(@"ISNULL((SELECT TOP(1) Pedido.PED_EMBARQUE_AFRETAMENTO_FEEDER FROM T_PEDIDO Pedido 
                                            join T_CARGA_PEDIDO CargaPedido on CargaPedido.PED_CODIGO = Pedido.PED_CODIGO 
                                            join T_CARGA_CTE CargaCTe on CargaCTe.CAR_CODIGO = CargaPedido.CAR_CODIGO 
                                            where CargaCTe.CON_CODIGO = CTeOriginal.CON_CODIGO), 0) Afretamento, ");

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append(" CTeOriginal.CON_CODIGO, ");
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
                                 WHERE CargaCTe.CON_CODIGO = CTeOriginal.CON_CODIGO for xml path('')), 3, 1000) NumeroProtocoloANTAQ, "
                        );

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append(" CTeOriginal.CON_CODIGO, ");
                    }
                    break;

                case "ProcImportacao":
                    if (!select.Contains(" ProcImportacao, "))
                    {
                        select.Append(@"ISNULL((SELECT TOP(1) Pedido.PED_ADICIONAL1 from T_PEDIDO Pedido 
                                        inner join T_CARGA_PEDIDO CargaPedido on CargaPedido.PED_CODIGO = pedido.PED_CODIGO 
                                        inner join T_CARGA_CTE CargaCTe on CargaCTe.CAR_CODIGO_ORIGEM = CargaPedido.CAR_CODIGO_ORIGEM 
                                        WHERE CargaCTe.CON_CODIGO = CTeOriginal.CON_CODIGO), '') ProcImportacao, ");

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append(" CTeOriginal.CON_CODIGO, ");
                    }
                    break;

                case "RotaFrete":
                    if (!select.Contains(" RotaFrete, "))
                    {
                        select.Append(@"(SELECT TOP(1) RotaFrete.ROF_DESCRICAO from T_ROTA_FRETE RotaFrete 
                                        inner join T_CARGA Carga on Carga.ROF_CODIGO = RotaFrete.ROF_CODIGO 
                                        inner join T_CARGA_CTE CargaCTe on CargaCTe.CAR_CODIGO = Carga.CAR_CODIGO 
                                        WHERE CargaCTe.CON_CODIGO = CTeOriginal.CON_CODIGO and (Carga.CAR_CARGA_TRANSBORDO is null or Carga.CAR_CARGA_TRANSBORDO = 0)) RotaFrete, ");

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append(" CTeOriginal.CON_CODIGO, ");
                    }
                    break;

                case "ValorSemTributo":
                    if (!somenteContarNumeroRegistros && !select.Contains(" ValorSemTributo, "))
                        select.Append("SUM(CTeOriginal.CON_VALOR_PREST_SERVICO - CTeOriginal.CON_VAL_ICMS - CTeOriginal.CON_VALOR_ISS - CTeOriginal.CON_VALOR_ICMS_UF_DESTINO - CTeOriginal.CON_VALOR_ICMS_FCP_DESTINO) ValorSemTributo, ");
                    break;

                case "NumeroCTeAnulacao":
                    if (!select.Contains(" NumeroCTeAnulacao, "))
                        select.Append(
                            @"SUBSTRING((SELECT DISTINCT ', ' + CAST(CTeGerado.CON_NUM AS NVARCHAR(20))
                                FROM T_CTE_RELACAO_DOCUMENTO CTeRelacao
                                JOIN T_CTE CTeGerado on CTeGerado.CON_CODIGO = CTeRelacao.CON_CODIGO_GERADO
                                WHERE CTeRelacao.CRD_TIPO_CTE_GERADO = 1 AND CTeRelacao.CON_CODIGO_ORIGINAL  = CTeOriginal.CON_CODIGO FOR XML PATH('')), 3, 1000) NumeroCTeAnulacao, ");

                    if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                        groupBy.Append("CTeOriginal.CON_CODIGO, ");
                    break;

                case "NumeroControleCTeAnulacao":
                    if (!select.Contains(" NumeroControleCTeAnulacao, "))
                        select.Append(
                            @"SUBSTRING((SELECT DISTINCT ', ' + CTeGerado.CON_NUMERO_CONTROLE
                                FROM T_CTE_RELACAO_DOCUMENTO CTeRelacao
                                JOIN T_CTE CTeGerado on CTeGerado.CON_CODIGO = CTeRelacao.CON_CODIGO_GERADO
                                WHERE CTeRelacao.CRD_TIPO_CTE_GERADO = 1 AND CTeRelacao.CON_CODIGO_ORIGINAL = CTeOriginal.CON_CODIGO FOR XML PATH('')), 3, 1000) NumeroControleCTeAnulacao, ");

                    if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                        groupBy.Append("CTeOriginal.CON_CODIGO, ");
                    break;

                case "NumeroCTeComplementar":
                    if (!select.Contains(" NumeroCTeComplementar, "))
                        select.Append(
                            @"SUBSTRING((SELECT DISTINCT ', ' + CAST(CTeGerado.CON_NUM AS NVARCHAR(20))
                                FROM T_CTE_RELACAO_DOCUMENTO CTeRelacao
                                JOIN T_CTE CTeGerado on CTeGerado.CON_CODIGO = CTeRelacao.CON_CODIGO_GERADO
                                WHERE CTeRelacao.CRD_TIPO_CTE_GERADO = 2 AND CTeRelacao.CON_CODIGO_ORIGINAL = CTeOriginal.CON_CODIGO FOR XML PATH('')), 3, 1000) NumeroCTeComplementar, ");

                    if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                        groupBy.Append("CTeOriginal.CON_CODIGO, ");
                    break;

                case "NumeroControleCTeComplementar":
                    if (!select.Contains(" NumeroControleCTeComplementar, "))
                        select.Append(
                            @"SUBSTRING((SELECT DISTINCT ', ' + CTeGerado.CON_NUMERO_CONTROLE
                                FROM T_CTE_RELACAO_DOCUMENTO CTeRelacao
                                JOIN T_CTE CTeGerado on CTeGerado.CON_CODIGO = CTeRelacao.CON_CODIGO_GERADO
                                WHERE CTeRelacao.CRD_TIPO_CTE_GERADO = 2 AND CTeRelacao.CON_CODIGO_ORIGINAL  = CTeOriginal.CON_CODIGO FOR XML PATH('')), 3, 1000) NumeroControleCTeComplementar, ");

                    if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                        groupBy.Append("CTeOriginal.CON_CODIGO, ");
                    break;

                case "NumeroCTeSubstituto":
                    if (!select.Contains(" NumeroCTeSubstituto, "))
                        select.Append(
                            @"SUBSTRING((SELECT DISTINCT ', ' + CAST(CTeGerado.CON_NUM AS NVARCHAR(20))
                                FROM T_CTE_RELACAO_DOCUMENTO CTeRelacao
                                JOIN T_CTE CTeGerado on CTeGerado.CON_CODIGO = CTeRelacao.CON_CODIGO_GERADO
                                WHERE CTeRelacao.CRD_TIPO_CTE_GERADO = 3 AND CTeRelacao.CON_CODIGO_ORIGINAL  = CTeOriginal.CON_CODIGO FOR XML PATH('')), 3, 1000) NumeroCTeSubstituto, ");

                    if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                        groupBy.Append("CTeOriginal.CON_CODIGO, ");
                    break;

                case "NumeroControleCTeSubstituto":
                    if (!select.Contains(" NumeroControleCTeSubstituto, "))
                        select.Append(
                            @"SUBSTRING((SELECT DISTINCT ', ' + CTeGerado.CON_NUMERO_CONTROLE
                                FROM T_CTE_RELACAO_DOCUMENTO CTeRelacao
                                JOIN T_CTE CTeGerado on CTeGerado.CON_CODIGO = CTeRelacao.CON_CODIGO_GERADO
                                WHERE CTeRelacao.CRD_TIPO_CTE_GERADO = 3 AND CTeRelacao.CON_CODIGO_ORIGINAL  = CTeOriginal.CON_CODIGO FOR XML PATH('')), 3, 1000) NumeroControleCTeSubstituto, ");

                    if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                        groupBy.Append("CTeOriginal.CON_CODIGO, ");
                    break;

                case "NumeroCTeDuplicado":
                    if (!select.Contains(" NumeroCTeDuplicado, "))
                        select.Append(
                            @"SUBSTRING((SELECT DISTINCT ', ' + CAST(CTeGerado.CON_NUM AS NVARCHAR(20))
                                FROM T_CTE_RELACAO_DOCUMENTO CTeRelacao
                                JOIN T_CTE CTeGerado on CTeGerado.CON_CODIGO = CTeRelacao.CON_CODIGO_GERADO
                                WHERE CTeRelacao.CRD_TIPO_CTE_GERADO = 4 AND CTeRelacao.CON_CODIGO_ORIGINAL  = CTeOriginal.CON_CODIGO FOR XML PATH('')), 3, 1000) NumeroCTeDuplicado, ");

                    if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                        groupBy.Append("CTeOriginal.CON_CODIGO, ");
                    break;

                case "NumeroControleCTeDuplicado":
                    if (!select.Contains(" NumeroControleCTeDuplicado, "))
                        select.Append(
                            @"SUBSTRING((SELECT DISTINCT ', ' + CTeGerado.CON_NUMERO_CONTROLE
                                FROM T_CTE_RELACAO_DOCUMENTO CTeRelacao
                                JOIN T_CTE CTeGerado on CTeGerado.CON_CODIGO = CTeRelacao.CON_CODIGO_GERADO
                                WHERE CTeRelacao.CRD_TIPO_CTE_GERADO = 4 AND CTeRelacao.CON_CODIGO_ORIGINAL  = CTeOriginal.CON_CODIGO FOR XML PATH('')), 3, 1000) NumeroControleCTeDuplicado, ");

                    if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                        groupBy.Append("CTeOriginal.CON_CODIGO, ");
                    break;

                case "NumeroCTeOriginal":
                    if (!select.Contains(" NumeroCTeOriginal, "))
                        select.Append(
                            @"SUBSTRING((SELECT DISTINCT ', ' + CAST(CTeOriginal.CON_NUM AS NVARCHAR(20))
                                FROM T_CTE_RELACAO_DOCUMENTO CTeRelacao
                                JOIN T_CTE CTeOriginal on CTeOriginal.CON_CODIGO = CTeRelacao.CON_CODIGO_ORIGINAL 
                                WHERE CTeRelacao.CON_CODIGO_GERADO  = CTeOriginal.CON_CODIGO FOR XML PATH('')), 3, 1000) NumeroCTeOriginal, ");

                    if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                        groupBy.Append("CTeOriginal.CON_CODIGO, ");
                    break;

                case "NumeroControleCTeOriginal":
                    if (!select.Contains(" NumeroControleCTeOriginal, "))
                        select.Append(
                            @"SUBSTRING((SELECT DISTINCT ', ' + CTeOriginal.CON_NUMERO_CONTROLE
                                FROM T_CTE_RELACAO_DOCUMENTO CTeRelacao
                                JOIN T_CTE CTeOriginal on CTeOriginal.CON_CODIGO = CTeRelacao.CON_CODIGO_ORIGINAL 
                                WHERE CTeRelacao.CON_CODIGO_GERADO  = CTeOriginal.CON_CODIGO FOR XML PATH('')), 3, 1000) NumeroControleCTeOriginal, ");

                    if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                        groupBy.Append("CTeOriginal.CON_CODIGO, ");
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
                                 WHERE CargaCTe.CON_CODIGO = CTeOriginal.CON_CODIGO for xml path('')), 3, 1000) NumeroCIOT, "
                        );

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append(" CTeOriginal.CON_CODIGO, ");
                    }
                    break;

                case "NumeroDocumentoOriginario":
                    if (!select.Contains(" NumeroDocumentoOriginario, "))
                    {
                        select.Append(
                            @"SUBSTRING((
                                SELECT DISTINCT ', ' + CONVERT(NVARCHAR(50), documentoOriginario.CDO_NUMERO) + '-' + documentoOriginario.CDO_SERIE
                                        FROM T_CTE_DOCUMENTO_ORIGINARIO documentoOriginario
                                 WHERE documentoOriginario.CON_CODIGO = CTeOriginal.CON_CODIGO for xml path('')), 3, 1000) NumeroDocumentoOriginario, "
                        );

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append(" CTeOriginal.CON_CODIGO, ");
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
                                 WHERE _cargaCTe.CON_CODIGO = CTeOriginal.CON_CODIGO for xml path('')), 3, 200) DataInicioViagem, "
                        );

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append("CTeOriginal.CON_CODIGO, ");
                    }
                    break;

                case "Taxa":
                    if (!select.Contains(" Taxa, "))
                    {
                        select.Append(@"(SELECT MAX(pedido.PED_VALOR_TAXA_FEEDER)
                                            FROM T_PEDIDO pedido
                                            JOIN T_CARGA_PEDIDO cargaPedido ON cargaPedido.PED_CODIGO = pedido.PED_CODIGO
                                            JOIN T_CARGA_CTE cargaCTe ON cargaCTe.CAR_CODIGO = cargaPedido.CAR_CODIGO
                                            WHERE cargaCTe.CON_CODIGO = CTeOriginal.CON_CODIGO) Taxa, ");

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append("CTeOriginal.CON_CODIGO, ");
                    }
                    break;

                case "QuantidadeContainer":
                    if (!select.Contains(" QuantidadeContainer, "))
                    {
                        select.Append(
                            @"(SELECT COUNT(1) from T_CONTAINER container 
                                 inner join T_CTE_CONTAINER cteContainer on cteContainer.CTR_CODIGO = container.CTR_CODIGO 
                                 WHERE cteContainer.CON_CODIGO = CTeOriginal.CON_CODIGO) QuantidadeContainer, "
                        );

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append(" CTeOriginal.CON_CODIGO, ");
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
                        select.Append("substring((select distinct ', ' + Pedido.PED_CODIGO_PEDIDO_CLIENTE from T_CARGA_CTE CargaCTe inner join T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE CargaPedidoXMLNotaFiscalCTe on CargaPedidoXMLNotaFiscalCTe.CCT_CODIGO = CargaCTe.CCT_CODIGO inner join T_PEDIDO_XML_NOTA_FISCAL PedidoXMLNotaFiscal on PedidoXMLNotaFiscal.PNF_CODIGO = CargaPedidoXMLNotaFiscalCTe.PNF_CODIGO inner join T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = PedidoXMLNotaFiscal.CPE_CODIGO inner join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO where CargaCTe.CON_CODIGO = CTeOriginal.CON_CODIGO for xml path('')), 3, 200) NumeroPedidoCliente, ");

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append("CTeOriginal.CON_CODIGO, ");
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
                                 where _nFe.CON_CODIGO = CTeOriginal.CON_CODIGO
                            ) QuantidadeTotalProduto, "
                        );

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append("CTeOriginal.CON_CODIGO, ");
                    }
                    break;

                case "DistanciaCargaAgrupada":
                    if (!select.Contains(" DistanciaCargaAgrupada,"))
                    {
                        select.Append("(select SUM(DadosSumarizados.CDS_DISTANCIA) from T_CARGA_CTE CargaCTe inner join T_CARGA Carga on CargaCTe.CAR_CODIGO = Carga.CAR_CODIGO inner join T_CARGA_DADOS_SUMARIZADOS DadosSumarizados ON DadosSumarizados.CDS_CODIGO = Carga.CDS_CODIGO where CargaCTe.CON_CODIGO = CTeOriginal.CON_CODIGO) DistanciaCargaAgrupada , ");

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append("CTeOriginal.CON_CODIGO, ");
                    }
                    break;

                case "ModeloVeiculo":
                    if (!select.Contains(" ModeloVeiculo,"))
                    {
                        select.Append("substring((select ', ' + VeiculoModelo.VMO_DESCRICAO from T_CTE_VEICULO veiculoCTe1 inner join T_VEICULO veiculo1 on veiculoCTe1.VEI_CODIGO = veiculo1.VEI_CODIGO inner join T_VEICULO_MODELO VeiculoModelo on VeiculoModelo.VMO_CODIGO = veiculo1.VMO_CODIGO where veiculoCTe1.CON_CODIGO = CTeOriginal.CON_CODIGO for xml path('')), 3, 1000) ModeloVeiculo, ");

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append("CTeOriginal.CON_CODIGO, ");
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
                        select.Append("   from T_CTE_VEICULO CteVeiculo inner join T_VEICULO Veiculo on CteVeiculo.VEI_CODIGO = Veiculo.VEI_CODIGO where CteVeiculo.CON_CODIGO = CTeOriginal.CON_CODIGO for xml path('')), 3, 1000) TipoCarroceria, ");

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append("CTeOriginal.CON_CODIGO, ");
                    }
                    break;

                case "OperadorResponsavelCancelamento":
                    if (!select.Contains(" OperadorResponsavelCancelamento, "))
                    {
                        select.Append(" (SELECT top 1 OperadorCancelamento.FUN_NOME from T_CARGA_CTE CargaCTe ");
                        select.Append("     join T_CARGA Carga ON CargaCTe.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("     join T_CARGA_CANCELAMENTO CargaCancelamento ON CargaCancelamento.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("     join T_FUNCIONARIO OperadorCancelamento ON OperadorCancelamento.FUN_CODIGO = CargaCancelamento.FUN_CODIGO_OPERADOR_RESPONSAVEL ");
                        select.Append("     where CargaCTe.CON_CODIGO = CTeOriginal.CON_CODIGO) OperadorResponsavelCancelamento, ");

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append("CTeOriginal.CON_CODIGO, ");
                    }
                    break;
                case "UsuarioSolicitante":
                    if (!select.Contains(" UsuarioSolicitante, "))
                    {
                        select.Append(" (SELECT UsuarioSolicitante.FUN_NOME from T_CARGA_CTE CargaCTe ");
                        select.Append("     join T_CARGA Carga ON CargaCTe.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("     join T_CARGA_CANCELAMENTO CargaCancelamento ON CargaCancelamento.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("     join T_FUNCIONARIO UsuarioSolicitante ON UsuarioSolicitante.FUN_CODIGO = CargaCancelamento.FUN_CODIGO ");
                        select.Append("     where CargaCTe.CON_CODIGO = CTeOriginal.CON_CODIGO) UsuarioSolicitante, ");

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append("CTeOriginal.CON_CODIGO, ");
                    }
                    break;
                case "VeiculoTracao":
                    if (!select.Contains(" VeiculoTracao,"))
                    {
                        select.Append("substring((select ', ' + veiculo1.VEI_PLACA from T_CTE_VEICULO veiculoCTe1 inner join T_VEICULO veiculo1 on veiculoCTe1.VEI_CODIGO = veiculo1.VEI_CODIGO where veiculoCTe1.CON_CODIGO = CTeOriginal.CON_CODIGO and veiculo1.VEI_TIPOVEICULO = 0 for xml path('')), 3, 1000) VeiculoTracao, ");

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append("CTeOriginal.CON_CODIGO, ");
                    }
                    break;

                case "VeiculoReboque":
                    if (!select.Contains(" VeiculoReboque,"))
                    {
                        select.Append("substring((select ', ' + veiculo1.VEI_PLACA from T_CTE_VEICULO veiculoCTe1 inner join T_VEICULO veiculo1 on veiculoCTe1.VEI_CODIGO = veiculo1.VEI_CODIGO where veiculoCTe1.CON_CODIGO = CTeOriginal.CON_CODIGO and veiculo1.VEI_TIPOVEICULO = 1 for xml path('')), 3, 1000) VeiculoReboque, ");

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append("CTeOriginal.CON_CODIGO, ");
                    }
                    break;

                case "KMRota":
                    if (!select.Contains(" KMRota, "))
                    {
                        select.Append(@"(SELECT TOP(1) RotaFrete.ROF_QUILOMETROS from T_ROTA_FRETE RotaFrete 
                                        inner join T_CARGA Carga on Carga.ROF_CODIGO = RotaFrete.ROF_CODIGO 
                                        inner join T_CARGA_CTE CargaCTe on CargaCTe.CAR_CODIGO = Carga.CAR_CODIGO 
                                        WHERE CargaCTe.CON_CODIGO = CTeOriginal.CON_CODIGO and (Carga.CAR_CARGA_TRANSBORDO is null or Carga.CAR_CARGA_TRANSBORDO = 0)) KMRota, ");

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append(" CTeOriginal.CON_CODIGO, ");
                    }
                    break;

                case "DataConfirmacaoDocumento":
                    if (!select.Contains(" DataConfirmacaoDocumento, "))
                    {
                        select.Append("substring((select distinct ', ' + CONVERT(NVARCHAR(10), Carga.CAR_DATA_INICIO_CONFIRMACAO_DOCUMENTOS_FISCAIS, 103)  + ' ' + SUBSTRING(CONVERT(NVARCHAR(10), Carga.CAR_DATA_INICIO_CONFIRMACAO_DOCUMENTOS_FISCAIS, 8), 1, 5) from T_CARGA_CTE CargaCTe inner join T_CARGA Carga on Carga.CAR_CODIGO = CargaCTe.CAR_CODIGO_ORIGEM where CargaCTe.CON_CODIGO = CTeOriginal.CON_CODIGO for xml path('')), 3, 200) DataConfirmacaoDocumento, ");

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append("CTeOriginal.CON_CODIGO, ");
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
                        select.Append("    WHERE cargaLacre.CAR_CODIGO = _cargaCTe.CAR_CODIGO and _cargaCTe.CON_CODIGO = CTeOriginal.CON_CODIGO");
                        select.Append("    FOR XML path('')), 3, 1000) as LacresCargaLacre, ");

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO"))
                            groupBy.Append("CTeOriginal.CON_CODIGO, ");
                    }
                    break;

                case "PalletsPedido":
                    if (!select.Contains(" PalletsPedido, "))
                    {
                        select.Append("(select sum(_pedido.PED_NUMERO_PALETES_FRACIONADO + _pedido.PED_NUMERO_PALETES) ");
                        select.Append("      FROM T_PEDIDO _pedido ");
                        select.Append("      INNER JOIN T_CARGA_PEDIDO _cargaPedido ON _cargaPedido.PED_CODIGO = _pedido.PED_CODIGO ");
                        select.Append("      INNER JOIN T_CARGA_CTE _cargaCTe ON _cargaCTe.CAR_CODIGO = _cargaPedido.CAR_CODIGO ");
                        select.Append("      WHERE _cargaCTe.CON_CODIGO = CTeOriginal.CON_CODIGO) PalletsPedido, ");

                        if (!groupBy.Contains("CTeOriginal.CON_CODIGO,"))
                            groupBy.Append("CTeOriginal.CON_CODIGO, ");
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
                case "DescricaoStatusCTeSubcontratado":
                    if (!select.Contains(" StatusCTeSubcontratado,"))
                    {
                        select.Append("CTeSubcontratado.CON_STATUS as StatusCTeSubcontratado, ");
                        groupBy.Append("CTeSubcontratado.CON_STATUS, ");

                        SetarJoinsCTeSubcontratado(joins);
                    }
                    break;

            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioCTesSubcontratados filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string pattern = "yyyy-MM-dd";

            SetarJoinsCTeSubcontratado(joins);
            SetarJoinsCTeOriginal(joins);

            if (filtrosPesquisa.DataInicialEmissao.HasValue)
                where.Append($"  and CAST(CTeSubcontratado.CON_DATAHORAEMISSAO AS DATE) >= '{filtrosPesquisa.DataInicialEmissao.Value.ToString(pattern)}'");

            if (filtrosPesquisa.DataFinalEmissao.HasValue)
                where.Append($"  and CAST(CTeSubcontratado.CON_DATAHORAEMISSAO AS DATE) <= '{filtrosPesquisa.DataFinalEmissao.Value.ToString(pattern)}'");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroCarga))
            {
                where.Append($@" and CTeOriginal.CON_CODIGO in (
                                                        select CargaCTe.CON_CODIGO from T_CARGA_CTE CargaCTe 
                                                         where CargaCTe.CAR_CODIGO_ORIGEM in 
                                                               (
                                                                SELECT CAR_CODIGO FROM T_CARGA WHERE CAR_CODIGO_CARGA_EMBARCADOR = '{filtrosPesquisa.NumeroCarga}'
                                                               )
                                                       ) ");
            }

            if (filtrosPesquisa.TransportadorTerceiro > 0)
            {
                where.Append($@"  and (
                                        (
                                         TransportadorCTeSubcontratado.EMP_CNPJ = '{filtrosPesquisa.TransportadorTerceiro.ToString("00000000000000")}' or VeiculoVeiculoCTe.VEI_PROPRIETARIO  = {filtrosPesquisa.TransportadorTerceiro.ToString()}
                                        )
                                   or CTeSubcontratado.CON_CODIGO in (
                                                                      SELECT DISTINCT CargaCTe.CON_CODIGO FROM T_CARGA_CTE CargaCTe
                                                                        JOIN T_CARGA Carga on CargaCTe.CAR_CODIGO = Carga.CAR_CODIGO
                                                                        JOIN T_VEICULO Veiculo on Carga.CAR_VEICULO = Veiculo.VEI_CODIGO
                                                                        WHERE VEI_PROPRIETARIO = {filtrosPesquisa.TransportadorTerceiro.ToString()}
                                                                     )
                                     )"
                    );

                SetarJoinsTransportadorSubcontratado(joins);

                if (!joins.Contains(" Veiculo "))
                    joins.Append(" left outer join T_CTE_VEICULO Veiculo on Veiculo.CVE_CODIGO = (SELECT TOP 1 CVE_CODIGO FROM T_CTE_VEICULO WHERE CON_CODIGO = CTeSubcontratado.CON_CODIGO) ");


                if (!joins.Contains(" VeiculoVeiculoCTe "))
                {
                    joins.Append(@"OUTER APPLY
                               (SELECT TOP 1 VEI_PLACA, VEI_PROPRIETARIO
                                 FROM T_VEICULO
                                JOIN T_EMPRESA ON T_VEICULO.EMP_CODIGO = T_EMPRESA.EMP_CODIGO
                                WHERE T_VEICULO.VEI_PLACA = Veiculo.CVE_PLACA and T_VEICULO.VEI_ATIVO = 1
                                order by T_VEICULO.VEI_PROPRIETARIO DESC
                              ) VeiculoVeiculoCTe ");
                }
            }

            if (filtrosPesquisa.CodigosTransportadores?.Count > 0)
                where.Append($"  and CTeSubcontratado.EMP_CODIGO IN ({string.Join(", ", filtrosPesquisa.CodigosTransportadores)})");

            if (filtrosPesquisa.CodigoEmpresaCTeOriginal > 0)
                where.Append($"  and CTeOriginal.EMP_CODIGO = {filtrosPesquisa.CodigoEmpresaCTeOriginal}");

            if (filtrosPesquisa.CodigoEmpresaCTeSubcontratado > 0)
                where.Append($"  and CTeSubcontratado.EMP_CODIGO = {filtrosPesquisa.CodigoEmpresaCTeSubcontratado}");
        }

        #endregion
    }
}