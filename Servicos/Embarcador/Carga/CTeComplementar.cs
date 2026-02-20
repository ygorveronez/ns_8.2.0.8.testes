using Dominio.Entidades.Embarcador.Configuracao;
using Dominio.Entidades.Embarcador.Frete;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Repositorio;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Carga
{
    public class CTeComplementar : ServicoBase
    {
        #region Construtores

        Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador = null;
        public CTeComplementar(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public string EmitirDocumentoComplementar(List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> cargaCTesComplementaresInfo, Repositorio.UnitOfWork unitOfWork, string webServiceConsultaCTe, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string webServiceOracle = "", Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal = null)
        {
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfoContaContabilContabilizacao repCargaCTeComplementoInfoContaContabilContabilizacao = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfoContaContabilContabilizacao(unitOfWork);
            string mensagem = "";
            Servicos.CTe servicoCte = new Servicos.CTe(unitOfWork);
            Servicos.NFSe servicoNFSe = new Servicos.NFSe(unitOfWork);

            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> documentosParaEmissao = new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfoContaContabilContabilizacao> cargaCTeComplementoInfoContaContabilContabilizacao = repCargaCTeComplementoInfoContaContabilContabilizacao.BuscarPorCodigos((from obj in cargaCTesComplementaresInfo select obj.Codigo).ToList());

            Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia repositorioConfiguracaoOcorrencia = new Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoOcorrencia configuracaoOcorrencia = repositorioConfiguracaoOcorrencia.BuscarConfiguracaoPadrao();
            Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraICMS = null;



            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCteComplementarInfo in cargaCTesComplementaresInfo)
            {
                unitOfWork.Start();

                if (cargaCteComplementarInfo.CargaOcorrencia != null)
                {
                    regraICMS = obterRegraICMS(configuracaoOcorrencia, cargaCteComplementarInfo.CargaOcorrencia.TipoOcorrencia, cargaCteComplementarInfo.ComplementoFilialEmissora, !cargaCteComplementarInfo.ComplementoFilialEmissora);

                    if (regraICMS != null)
                        cargaCteComplementarInfo.CST = regraICMS.CST;

                    OrigemOcorrencia metodoEmissao = cargaCteComplementarInfo.CargaOcorrencia.OrigemOcorrencia;

                    if (cargaCteComplementarInfo.CargaOcorrencia.TipoOcorrencia.OcorrenciaComVeiculo)
                        metodoEmissao = OrigemOcorrencia.PorCarga;

                    if (cargaCteComplementarInfo.CargaOcorrencia.TipoOcorrencia.OcorrenciaDestinadaFranquias)
                        metodoEmissao = OrigemOcorrencia.PorContrato;

                    switch (metodoEmissao)
                    {
                        case OrigemOcorrencia.PorContrato:
                            EmitirDocumentoComplementarPorContrato(cargaCteComplementarInfo, ref documentosParaEmissao, tipoServicoMultisoftware, ref mensagem, unitOfWork, cargaCTeComplementoInfoContaContabilContabilizacao, regraICMS);
                            break;
                        // TODO: DIVIDIR ESSE FLUXO EM DOIS
                        case OrigemOcorrencia.PorCarga:
                        case OrigemOcorrencia.PorPeriodo:
                            // É um complemento de CargaDocumentoParaEmissaoNFSManual e ainda não foi gerado
                            if (cargaCteComplementarInfo.CargaDocumentoParaEmissaoNFSManualComplementado != null && cargaCteComplementarInfo.CargaDocumentoParaEmissaoNFSManualGerado == null)
                            {
                                EmitirComplementoDocumentoParaEmissaoNFSManual(cargaCteComplementarInfo, ref mensagem, unitOfWork);
                            }
                            else
                            {
                                EmitirDocumentoComplementarPorCargas(cargaCteComplementarInfo, ref documentosParaEmissao, tipoServicoMultisoftware, ref mensagem, unitOfWork, cargaCTeComplementoInfoContaContabilContabilizacao);
                            }

                            break;
                    }
                }
                else if (cargaCteComplementarInfo.FechamentoFrete != null)
                    EmitirDocumentoComplementarPorFechamento(cargaCteComplementarInfo, ref documentosParaEmissao, tipoServicoMultisoftware, ref mensagem, unitOfWork, modeloDocumentoFiscal, cargaCTeComplementoInfoContaContabilContabilizacao);

                if (cargaCteComplementarInfo.CargaCTeComplementado?.CargaCTeComplementoInfoProduto != null)
                {
                    Repositorio.Embarcador.Cargas.CTeProduto repCTeProduto = new Repositorio.Embarcador.Cargas.CTeProduto(unitOfWork);

                    foreach (Dominio.Entidades.Embarcador.Cargas.CTeProduto cteProduto in cargaCteComplementarInfo.CargaCTeComplementado?.CargaCTeComplementoInfoProduto?.CTeProdutos)
                    {
                        cteProduto.CTe = cargaCteComplementarInfo?.CTe ?? null;
                        repCTeProduto.Atualizar(cteProduto);
                    }
                }

                unitOfWork.CommitChanges();
            }

            //unitOfWork.CommitChanges();

            bool adicionarFilaConsulta = false;

            adicionarFilaConsulta = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().AdicionarCTesFilaConsulta.Value;

            foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cte in documentosParaEmissao)
            {
                if (cte != null && cte.Status == "E")
                {
                    if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)
                    {
                        if (!servicoCte.Emitir(cte.Codigo, 0, unitOfWork))
                            mensagem += "O CT-e nº " + cte.Numero.ToString() + " da empresa " + cte.Empresa.CNPJ + " foi salvo, porém, ocorreu uma falha ao emiti-lo.";
                        else if (adicionarFilaConsulta)
                            servicoCte.AdicionarCTeNaFilaDeConsulta(cte, unitOfWork);
                    }
                    else if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe)
                    {
                        if (!servicoNFSe.EmitirNFSe(cte.Codigo, unitOfWork))
                            mensagem += "A NFS-e nº " + cte.Numero.ToString() + " da empresa " + cte.Empresa.CNPJ + " foi salvo, porém, ocorreu uma falha ao emiti-la.";
                    }
                }
            }

            return mensagem;
        }

        public string CriarCargaCTeComplementoInfo(List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTEs, decimal valorComplemento, string observacaoCTe, Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, bool incluirICMSFrete, Dominio.Entidades.Embarcador.Cargas.CargaComplementoFrete cargaComplementoFrete, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicosMultiSoftware, bool complementoICMS)
        {
            return CriarCargaCTeComplementoInfo(cargaCTEs, null, valorComplemento, observacaoCTe, ocorrencia, incluirICMSFrete, cargaComplementoFrete, componenteFrete, unitOfWork, tipoServicosMultiSoftware, complementoICMS, TipoEmissaoDocumentoOcorrencia.Todos);
        }

        public string CriarCargaCTeComplementoInfo(List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTEs, List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> listaCargaDocumentoParaEmissaoNFSManual, decimal valorComplemento, string observacaoCTe, Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, bool incluirICMSFrete, Dominio.Entidades.Embarcador.Cargas.CargaComplementoFrete cargaComplementoFrete, ComponenteFrete componenteFrete, UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicosMultiSoftware, bool complementoICMS, TipoEmissaoDocumentoOcorrencia tipoEmissaoDocumentoOcorrencia, bool permitirAbrirOcorrenciaAposPrazoSolicitacao = false, TipoRateioOcorrenciaLote? tipoRateioOcorrenciaLote = null, bool emitirDocumentoParaFilialEmissoraComPreCTe = false)
        {
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = ObterConfiguracaoEmbarcador(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> cargaCTesComplementaresInfo = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>();
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> cargaCTesComplementaresInfoFilialEmissora = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>();
            Servicos.Embarcador.Carga.RateioCTeComplementar serFreteRateio = new RateioCTeComplementar(unitOfWork);

            if (ocorrencia == null)
                return string.Empty;

            // Gerando os complementos das cargasCTes
            if (ocorrencia.TipoOcorrencia.GerarApenasUmComplemento && cargaCTEs != null && cargaCTEs.Count > 0)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = cargaCTEs.FirstOrDefault();
                int diasPrazo = ObterDiasPrazoSolicitacaoOcorrencia(ocorrencia.TipoOcorrencia, cargaCTe.Carga, configuracao);
                if (verificaPermissaoEmissaoAposPrazo(cargaCTe, ocorrencia.TipoOcorrencia, configuracao, diasPrazo, permitirAbrirOcorrenciaAposPrazoSolicitacao))
                {
                    GerarComplementosInfoPorCargaCte(cargaCTe, configuracao, ocorrencia, permitirAbrirOcorrenciaAposPrazoSolicitacao, tipoEmissaoDocumentoOcorrencia, cargaComplementoFrete, incluirICMSFrete, componenteFrete, observacaoCTe, ref cargaCTesComplementaresInfoFilialEmissora, ref cargaCTesComplementaresInfo, out String erro, unitOfWork, emitirDocumentoParaFilialEmissoraComPreCTe);
                    if (erro != null)
                        return erro;
                }
                else
                    return "Não é possível emitir CT-es complementares após " + int.Parse((diasPrazo).ToString()) + " dias da emissão (Verifique a opção dos dias para emissão de CT-e complementar cadastrado).";

            }
            else
            {
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in cargaCTEs)
                {
                    GerarComplementosInfoPorCargaCte(cargaCTe, configuracao, ocorrencia, permitirAbrirOcorrenciaAposPrazoSolicitacao, tipoEmissaoDocumentoOcorrencia, cargaComplementoFrete, incluirICMSFrete, componenteFrete, observacaoCTe, ref cargaCTesComplementaresInfoFilialEmissora, ref cargaCTesComplementaresInfo, out String erro, unitOfWork, emitirDocumentoParaFilialEmissoraComPreCTe);
                    if (erro != null)
                        return erro;
                }
            }

            // Gerando os complementos das CargaDocumentoParaEmissaoNFSManual
            if (ocorrencia.TipoOcorrencia.GerarApenasUmComplemento && listaCargaDocumentoParaEmissaoNFSManual != null && cargaCTEs.Count == 0 && listaCargaDocumentoParaEmissaoNFSManual.Count > 0)
            {
                var documentoParaEmissaoNFSManual = listaCargaDocumentoParaEmissaoNFSManual.FirstOrDefault();
                GerarComplementosInfoPorDocumentoParaEmissaoNFSManual(documentoParaEmissaoNFSManual, configuracao, ocorrencia, permitirAbrirOcorrenciaAposPrazoSolicitacao, tipoEmissaoDocumentoOcorrencia, cargaComplementoFrete, incluirICMSFrete, componenteFrete, observacaoCTe, ref cargaCTesComplementaresInfoFilialEmissora, ref cargaCTesComplementaresInfo, out String erro, unitOfWork);
                if (erro != null)
                    return erro;
            }
            else if (listaCargaDocumentoParaEmissaoNFSManual != null)
            {
                foreach (var documentoParaEmissaoNFSManual in listaCargaDocumentoParaEmissaoNFSManual)
                {
                    GerarComplementosInfoPorDocumentoParaEmissaoNFSManual(documentoParaEmissaoNFSManual, configuracao, ocorrencia, permitirAbrirOcorrenciaAposPrazoSolicitacao, tipoEmissaoDocumentoOcorrencia, cargaComplementoFrete, incluirICMSFrete, componenteFrete, observacaoCTe, ref cargaCTesComplementaresInfoFilialEmissora, ref cargaCTesComplementaresInfo, out String erro, unitOfWork);
                    if (erro != null)
                        return erro;

                }
            }

            // Rateando o frete entre os complementos
            if (complementoICMS || ocorrencia != null && ocorrencia.ComplementoValorFreteCarga)
                return string.Empty;
            else
            {
                string retorno = string.Empty;

                if (cargaCTesComplementaresInfo.Count() > 0)
                    retorno = serFreteRateio.RatearValorDoFrenteEntreCTesComplementares(valorComplemento, cargaCTesComplementaresInfo, unitOfWork, configuracao, tipoServicosMultiSoftware, ocorrencia.Moeda ?? MoedaCotacaoBancoCentral.Real, ocorrencia.ValorCotacaoMoeda ?? 0m, ocorrencia.ValorTotalMoeda ?? 0m, ocorrencia.TipoOcorrencia.TipoRateio, tipoRateioOcorrenciaLote);

                if (cargaCTesComplementaresInfoFilialEmissora.Count() > 0 && string.IsNullOrWhiteSpace(retorno))
                    retorno = serFreteRateio.RatearValorDoFrenteEntreCTesComplementares(valorComplemento, cargaCTesComplementaresInfoFilialEmissora, unitOfWork, configuracao, tipoServicosMultiSoftware, MoedaCotacaoBancoCentral.Real, 0m, 0m, ocorrencia.TipoOcorrencia.TipoRateio, tipoRateioOcorrenciaLote);

                return retorno;
            }
        }

        public string CriaComplementoInfoContrato(decimal valorComplemento, string observacaoCTe, Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, bool incluirICMSFrete, Dominio.Entidades.Embarcador.Cargas.CargaComplementoFrete cargaComplementoFrete, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            string mensagem = "";

            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);
            Servicos.Embarcador.Carga.RateioCTeComplementar serFreteRateio = new RateioCTeComplementar(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTeComplementoInfo = new Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo
            {
                CargaCTeComplementado = null,
                CargaOcorrencia = ocorrencia,
                CargaComplementoFrete = cargaComplementoFrete,
                IncluirICMSFrete = incluirICMSFrete, //cargaCTe.CTe.IncluirICMSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim ? true : false; Verificar tombini para salvar na ocorrência se vai incluir ou não
                ComplementoIntegradoEmbarcador = false,
                ComponenteFrete = componenteFrete,
                ObservacaoCTe = observacaoCTe,
                ValorComplemento = valorComplemento,
            };


            mensagem = serFreteRateio.CalcularImpostosComplementoInfo(ref cargaCTeComplementoInfo, tipoServicoMultisoftware, unitOfWork, configuracao);

            if (string.IsNullOrWhiteSpace(mensagem))
                repCargaCTeComplementoInfo.Inserir(cargaCTeComplementoInfo);

            return mensagem;
        }

        public string CriaComplementoInfoPeriodo(decimal valorComplemento, string observacaoCTe, Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, bool incluirICMSFrete, Dominio.Entidades.Embarcador.Cargas.CargaComplementoFrete cargaComplementoFrete, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, bool permitirAbrirOcorrenciaAposPrazoSolicitacao = false)
        {
            int diasPrazo = ObterDiasPrazoSolicitacaoOcorrencia(ocorrencia.TipoOcorrencia, ocorrencia.Carga, configuracao);

            if (ocorrencia.PeriodoInicio.HasValue && (diasPrazo == 0 || ocorrencia.PeriodoInicio.Value.AddDays(diasPrazo) >= DateTime.Now || permitirAbrirOcorrenciaAposPrazoSolicitacao))
            {
                Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);
                Servicos.Embarcador.Carga.RateioCTeComplementar serFreteRateio = new RateioCTeComplementar(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTeComplementoInfo = new Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo
                {
                    CargaCTeComplementado = null,
                    CargaOcorrencia = ocorrencia,
                    CargaComplementoFrete = cargaComplementoFrete,
                    IncluirICMSFrete = incluirICMSFrete,
                    ComplementoIntegradoEmbarcador = false,
                    ComponenteFrete = componenteFrete,
                    ObservacaoCTe = observacaoCTe,
                    ValorComplemento = valorComplemento,
                };

                string mensagem = serFreteRateio.CalcularImpostosComplementoInfo(ref cargaCTeComplementoInfo, tipoServicoMultisoftware, unitOfWork, configuracao);

                if (string.IsNullOrWhiteSpace(mensagem))
                    repCargaCTeComplementoInfo.Inserir(cargaCTeComplementoInfo);

                return mensagem;
            }
            else
                return "Não é possível emitir CT-es complementares após " + int.Parse(diasPrazo.ToString()) + " dias do período inicial (Verifique a opção dos dias para emissão de CT-e complementar cadastrado).";
        }

        public static string CriaComplementoInfoDocumentosAgrupados(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, decimal valorOcorrencia, string observacaoCTe, bool incluirICMSFrete, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaSumarizado repCargaOcorrenciaSumarizado = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaSumarizado(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);
            Servicos.Embarcador.Carga.RateioCTeComplementar serFreteRateio = new RateioCTeComplementar(unitOfWork);

            // Busca todos agrupamentos
            if (repCargaOcorrenciaSumarizado.ContarCargasOcorrencia(ocorrencia.Codigo) > 0)
            {
                IList<Dominio.ObjetosDeValor.Embarcador.Carga.CargaOcorrenciaDocumentosAgrupados> documentosAgrupados = repCargaOcorrenciaSumarizado.ConsultarDocumentosAgrupadosDaOcorrencia(ocorrencia.Codigo, "", "", 0, 0);

                // Rateia o valor da ocorrencia
                int quantidadeComplementos = documentosAgrupados.Count;
                decimal valorComplementoRateado = 0;
                if (quantidadeComplementos > 0)
                    valorComplementoRateado = Math.Round((valorOcorrencia / quantidadeComplementos), 2, MidpointRounding.AwayFromZero);
                else
                    valorComplementoRateado = Math.Round((valorOcorrencia), 2, MidpointRounding.AwayFromZero);

                decimal diferenteRateio = valorOcorrencia - (quantidadeComplementos * valorComplementoRateado);

                // Itera todos
                for (int i = 0, s = documentosAgrupados.Count(); i < s; i++)
                {
                    Dominio.ObjetosDeValor.Embarcador.Carga.CargaOcorrenciaDocumentosAgrupados agrupamento = documentosAgrupados[i];

                    // Busca documentos não agrupados
                    List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> documentos = repCargaOcorrenciaSumarizado.ConsultarDocumentosCargasPorContratoEVeiculoDaOcorrencia(ocorrencia.Codigo, agrupamento.CnpjRemetente, agrupamento.CnpjDestinatario, agrupamento.ModeloDocumento, "", "", 0, 0);
                    if (documentos.Count > 0)
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaCTe cteComplementado = documentos.FirstOrDefault();

                        string observacaoFreteDespesa = string.Empty;
                        if (ocorrencia.PercentualAcresciomoValor > 0 && ocorrencia.ValorOcorrencia > 0 && ocorrencia.OrigemOcorrencia == OrigemOcorrencia.PorContrato)
                        {
                            decimal valorFreteDespesa = Math.Round((ocorrencia.ValorOcorrencia / quantidadeComplementos), 2, MidpointRounding.ToEven);
                            observacaoFreteDespesa = "Frete Despesa: R$" + String.Format("{0:0.##}", valorFreteDespesa, cultura) + " / Valor utilizado para calculo conforme contrato configurado;";
                        }

                        // Gera complemento pro primeiro documento
                        Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTeComplementoInfo = new Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo
                        {
                            CargaCTeComplementado = cteComplementado,
                            CargaOcorrencia = ocorrencia,
                            IncluirICMSFrete = incluirICMSFrete,
                            IndicadorCTeGlobalizado = (cteComplementado.CTe?.IndicadorGlobalizado == Dominio.Enumeradores.OpcaoSimNao.Sim) ? true : false,
                            ComplementoIntegradoEmbarcador = false,
                            ComponenteFrete = componenteFrete,
                            ObservacaoCTe = string.IsNullOrWhiteSpace(observacaoCTe) ? observacaoFreteDespesa : string.Concat(observacaoFreteDespesa, observacaoCTe),
                            ValorComplemento = valorComplementoRateado + (i == 0 ? diferenteRateio : 0) // Adicionar a diferença rateio no primeiro complementendo 
                        };

                        string mensagem = serFreteRateio.CalcularImpostosComplementoInfo(ref cargaCTeComplementoInfo, tipoServicoMultisoftware, unitOfWork, configuracao);

                        if (string.IsNullOrWhiteSpace(mensagem))
                            repCargaCTeComplementoInfo.Inserir(cargaCTeComplementoInfo);
                        else
                            return mensagem;

                    }
                }
            }
            return "";
        }

        public void ImportarCTesComplementaresParaOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicosMultiSoftware)
        {
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento repCargaOcorrenciaDocumento = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTeComplementar = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

            List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento> cargaOcorrenciaDocumentos = repCargaOcorrenciaDocumento.BuscarPorOcorrencia(ocorrencia.Codigo);

            foreach (Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento cargaOcorrenciaDocumento in cargaOcorrenciaDocumentos)
            {
                if (cargaOcorrenciaDocumento.CTeImportado != null)
                {
                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cteImportado = repCTe.BuscarPorCodigo(cargaOcorrenciaDocumento.CTeImportado.Codigo);

                    Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTeComplementoInfo = new Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo
                    {
                        CargaCTeComplementado = cargaOcorrenciaDocumento.CargaCTe,
                        CargaOcorrencia = ocorrencia,
                        IncluirICMSFrete = ocorrencia.IncluirICMSFrete,
                        ComplementoIntegradoEmbarcador = false,
                        ComponenteFrete = ocorrencia.ComponenteFrete,
                        ObservacaoCTe = !string.IsNullOrWhiteSpace(cteImportado.ObservacoesGerais) ? cteImportado.ObservacoesGerais : "",
                        ValorComplemento = cteImportado.ValorAReceber,
                        CTe = cteImportado
                    };

                    repCargaCTeComplementoInfo.Inserir(cargaCTeComplementoInfo);

                    Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = new Dominio.Entidades.Embarcador.Cargas.CargaCTe
                    {
                        Carga = ocorrencia.Carga,
                        CargaOrigem = ocorrencia.Carga,
                        CTe = cteImportado,
                        SistemaEmissor = SistemaEmissor.OutrosEmissores,
                        CargaCTeComplementoInfo = cargaCTeComplementoInfo
                    };

                    repCargaCTeComplementar.Inserir(cargaCTe);

                    if (tipoServicosMultiSoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                        CriarComponentesComplemento(cargaCTe, unitOfWork);
                    else
                        CriarComponentePadraoComplemento(cargaCTeComplementoInfo, cargaCTe, unitOfWork);

                    CriarVeiculosCTeImportado(ocorrencia, cteImportado, unitOfWork);
                    CriarMotoristasCTeImportado(ocorrencia, cteImportado, unitOfWork);

                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe> cargaPedidosXMLNotaFiscaisCTeComplementado = repCargaPedidoXMLNotaFiscalCTe.BuscarCargaPedidoXMLNotaFiscalCTePorCargaCTe(cargaCTeComplementoInfo.CargaCTeComplementado.Codigo);
                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe cargaPedidoXMLNotaFiscalCTeComplementado in cargaPedidosXMLNotaFiscaisCTeComplementado)
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe cargaPedidoXMLNotaFiscalCTE = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe
                        {
                            CargaCTe = cargaCTe,
                            PedidoXMLNotaFiscal = cargaPedidoXMLNotaFiscalCTeComplementado.PedidoXMLNotaFiscal
                        };

                        repCargaPedidoXMLNotaFiscalCTe.Inserir(cargaPedidoXMLNotaFiscalCTE);
                    }
                    cteImportado.CentroResultadoFaturamento = cargaOcorrenciaDocumento.CargaCTe?.CTe?.CentroResultadoFaturamento;
                    cteImportado.PossuiPedidoSubstituicao = cargaOcorrenciaDocumento.CargaCTe?.CTe?.PossuiPedidoSubstituicao ?? false;

                    repCTe.Atualizar(cteImportado);
                }
            }
        }

        public void PreencherCamposImpostoIBSCBS(Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCteComplementarInfo, Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS impostoIBSCBS)
        {
            cargaCteComplementarInfo.SetarRegraOutraAliquota(impostoIBSCBS.CodigoOutraAliquota);
            cargaCteComplementarInfo.CSTIBSCBS = impostoIBSCBS.CST;
            cargaCteComplementarInfo.ClassificacaoTributariaIBSCBS = impostoIBSCBS.ClassificacaoTributaria;
            cargaCteComplementarInfo.BaseCalculoIBSCBS = impostoIBSCBS.BaseCalculo;

            cargaCteComplementarInfo.AliquotaIBSEstadual = impostoIBSCBS.AliquotaIBSEstadual;
            cargaCteComplementarInfo.PercentualReducaoIBSEstadual = impostoIBSCBS.PercentualReducaoIBSEstadual;
            cargaCteComplementarInfo.ValorIBSEstadual = Math.Round(impostoIBSCBS.ValorIBSEstadual, 3, MidpointRounding.AwayFromZero);

            cargaCteComplementarInfo.AliquotaIBSMunicipal = impostoIBSCBS.AliquotaIBSMunicipal;
            cargaCteComplementarInfo.PercentualReducaoIBSMunicipal = impostoIBSCBS.PercentualReducaoIBSMunicipal;
            cargaCteComplementarInfo.ValorIBSMunicipal = Math.Round(impostoIBSCBS.ValorIBSMunicipal, 3, MidpointRounding.AwayFromZero);

            cargaCteComplementarInfo.AliquotaCBS = impostoIBSCBS.AliquotaCBS;
            cargaCteComplementarInfo.PercentualReducaoCBS = impostoIBSCBS.PercentualReducaoCBS;
            cargaCteComplementarInfo.ValorCBS = Math.Round(impostoIBSCBS.ValorCBS, 3, MidpointRounding.AwayFromZero);
        }

        #endregion

        #region Métodos Privados

        private void GerarComplementosInfoPorCargaCte(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe, ConfiguracaoTMS configuracao, Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, bool permitirAbrirOcorrenciaAposPrazoSolicitacao, TipoEmissaoDocumentoOcorrencia tipoEmissaoDocumentoOcorrencia, Dominio.Entidades.Embarcador.Cargas.CargaComplementoFrete cargaComplementoFrete, bool incluirICMSFrete, ComponenteFrete componenteFrete, string observacaoCTe, ref List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> cargaCTesComplementaresInfoFilialEmissora, ref List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> cargaCTesComplementaresInfo, out string erro, UnitOfWork unitOfWork, bool emitirDocumentoParaFilialEmissoraComPreCTe)
        {
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);

            erro = null;
            int diasPrazo = ObterDiasPrazoSolicitacaoOcorrencia(ocorrencia.TipoOcorrencia, cargaCTe.Carga, configuracao);
            bool verificacaoPermissaoPrazosCargaCTes = verificaPermissaoEmissaoAposPrazo(cargaCTe, ocorrencia.TipoOcorrencia, configuracao, diasPrazo, permitirAbrirOcorrenciaAposPrazoSolicitacao);

            if (!verificacaoPermissaoPrazosCargaCTes)
            {
                erro = "Não é possível emitir CT-es complementares após " + int.Parse((diasPrazo).ToString()) + " dias da emissão (Verifique a opção dos dias para emissão de CT-e complementar cadastrado).";
                return;
            }

            bool gerarComplementoCte = (
                (tipoEmissaoDocumentoOcorrencia == TipoEmissaoDocumentoOcorrencia.Todos) ||
                (tipoEmissaoDocumentoOcorrencia == TipoEmissaoDocumentoOcorrencia.SomenteSubcontratada) ||
                (cargaCTe.CargaCTeFilialEmissora == null && cargaCTe.CargaCTeSubContratacaoFilialEmissora == null)
            );

            if (gerarComplementoCte)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTeComplementoInfo = new Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo()
                {
                    CargaCTeComplementado = cargaCTe,
                    CargaOcorrencia = ocorrencia,
                    CargaComplementoFrete = cargaComplementoFrete,
                    IncluirICMSFrete = incluirICMSFrete, //cargaCTe.CTe.IncluirICMSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim ? true : false; Verificar tombini para salvar na ocorrência se vai incluir ou não
                    ComplementoIntegradoEmbarcador = false,
                    ComplementoFilialEmissora = false,
                    IndicadorCTeGlobalizado = cargaCTe.CTe?.IndicadorGlobalizado == Dominio.Enumeradores.OpcaoSimNao.Sim,
                    ComponenteFrete = componenteFrete,
                    ObservacaoCTe = observacaoCTe,
                    ProvisaoPelaNotaFiscal = cargaCTe.CargaCTeFilialEmissora != null
                };

                repCargaCTeComplementoInfo.Inserir(cargaCTeComplementoInfo);
                cargaCTesComplementaresInfo.Add(cargaCTeComplementoInfo);
            }

            bool gerarComplementoCteFilialEmissora = (
                (tipoEmissaoDocumentoOcorrencia == TipoEmissaoDocumentoOcorrencia.Todos || tipoEmissaoDocumentoOcorrencia == TipoEmissaoDocumentoOcorrencia.SomenteFilialEmissora) &&
                (cargaCTe.CargaCTeFilialEmissora != null)
            );

            if (gerarComplementoCteFilialEmissora)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTeComplementoInfoFilialEmissora = new Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo()
                {
                    CargaCTeComplementado = cargaCTe.CargaCTeFilialEmissora,
                    CargaOcorrencia = ocorrencia,
                    CargaComplementoFrete = cargaComplementoFrete,
                    IncluirICMSFrete = incluirICMSFrete, //cargaCTe.CTe.IncluirICMSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim ? true : false; Verificar tombini para salvar na ocorrência se vai incluir ou não
                    ComplementoIntegradoEmbarcador = false,
                    ComponenteFrete = componenteFrete,
                    ComplementoFilialEmissora = true,
                    ObservacaoCTe = observacaoCTe
                };

                repCargaCTeComplementoInfo.Inserir(cargaCTeComplementoInfoFilialEmissora);
                cargaCTesComplementaresInfoFilialEmissora.Add(cargaCTeComplementoInfoFilialEmissora);
            }

            if (emitirDocumentoParaFilialEmissoraComPreCTe
                && tipoEmissaoDocumentoOcorrencia == TipoEmissaoDocumentoOcorrencia.SomenteFilialEmissora
                && cargaCTe.CargaCTeFilialEmissora == null
                && cargaCTe.CargaCTeSubContratacaoFilialEmissora != null
                && cargaCTe.CargaCTeSubContratacaoFilialEmissora.CargaCTeFilialEmissora != null)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTeComplementoInfoFilialEmissora = new Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo()
                {
                    CargaCTeComplementado = cargaCTe.CargaCTeSubContratacaoFilialEmissora.CargaCTeFilialEmissora,
                    CargaOcorrencia = ocorrencia,
                    CargaComplementoFrete = cargaComplementoFrete,
                    IncluirICMSFrete = incluirICMSFrete,
                    ComplementoIntegradoEmbarcador = false,
                    ComponenteFrete = componenteFrete,
                    ComplementoFilialEmissora = true,
                    ObservacaoCTe = observacaoCTe
                };

                repCargaCTeComplementoInfo.Inserir(cargaCTeComplementoInfoFilialEmissora);
                cargaCTesComplementaresInfoFilialEmissora.Add(cargaCTeComplementoInfoFilialEmissora);
            }
        }

        private void GerarComplementosInfoPorDocumentoParaEmissaoNFSManual(Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual documentoParaEmissaoNFSManual, ConfiguracaoTMS configuracao, Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, bool permitirAbrirOcorrenciaAposPrazoSolicitacao, TipoEmissaoDocumentoOcorrencia tipoEmissaoDocumentoOcorrencia, Dominio.Entidades.Embarcador.Cargas.CargaComplementoFrete cargaComplementoFrete, bool incluirICMSFrete, ComponenteFrete componenteFrete, string observacaoCTe, ref List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> cargaCTesComplementaresInfoFilialEmissora, ref List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> cargaCTesComplementaresInfo, out string erro, UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaDadosSumarizados repCargaDadosSumarizados = new Repositorio.Embarcador.Cargas.CargaDadosSumarizados(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);

            erro = null;
            int diasPrazo = ObterDiasPrazoSolicitacaoOcorrencia(ocorrencia.TipoOcorrencia, documentoParaEmissaoNFSManual.Carga, configuracao);
            bool verificacaoPermissaoPrazos = verificaPermissaoEmissaoAposPrazo(documentoParaEmissaoNFSManual, configuracao, diasPrazo, permitirAbrirOcorrenciaAposPrazoSolicitacao);

            if (!verificacaoPermissaoPrazos)
            {
                erro = "Não é possível emitir Docs para NFS Manual complementares após " + int.Parse(diasPrazo.ToString()) + " dias da emissão (Verifique a opção dos dias para emissão de CT-e complementar cadastrado).";
                return;
            }

            bool gerarComplemento = (
                (tipoEmissaoDocumentoOcorrencia == TipoEmissaoDocumentoOcorrencia.Todos)
                || (tipoEmissaoDocumentoOcorrencia == TipoEmissaoDocumentoOcorrencia.SomenteSubcontratada)
            );

            if (gerarComplemento)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTeComplementoInfo = new Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo()
                {
                    CargaDocumentoParaEmissaoNFSManualComplementado = documentoParaEmissaoNFSManual,
                    CargaOcorrencia = ocorrencia,
                    CargaComplementoFrete = cargaComplementoFrete,
                    IncluirICMSFrete = incluirICMSFrete, //cargaCTe.CTe.IncluirICMSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim ? true : false; Verificar tombini para salvar na ocorrência se vai incluir ou não
                    ComplementoIntegradoEmbarcador = false,
                    ComplementoFilialEmissora = false,
                    IndicadorCTeGlobalizado = documentoParaEmissaoNFSManual.CargaCTe?.CTe?.IndicadorGlobalizado == Dominio.Enumeradores.OpcaoSimNao.Sim,
                    ComponenteFrete = componenteFrete,
                    ObservacaoCTe = observacaoCTe,
                };

                repCargaCTeComplementoInfo.Inserir(cargaCTeComplementoInfo);
                cargaCTesComplementaresInfo.Add(cargaCTeComplementoInfo);

                // Setar na ocorrência para especificar que possui NFS Manual
                if (documentoParaEmissaoNFSManual.Carga != null)
                {
                    documentoParaEmissaoNFSManual.Carga.DadosSumarizados.PossuiNFSManual = true;
                    repCargaDadosSumarizados.Atualizar(documentoParaEmissaoNFSManual.Carga.DadosSumarizados);
                }
                cargaCTeComplementoInfo.CargaOcorrencia.PossuiNFSManual = true;
                cargaCTeComplementoInfo.CargaOcorrencia.NFSManualPendenteGeracao = true;
                repCargaOcorrencia.Atualizar(cargaCTeComplementoInfo.CargaOcorrencia);

            }
        }

        private void CriarComponentesComplemento(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ComponentePrestacaoCTE repComponentePrestacaoCTe = new Repositorio.ComponentePrestacaoCTE(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete repCargaCTeComponentesFrete = new Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete(unitOfWork);

            List<Dominio.Entidades.ComponentePrestacaoCTE> componentesPrestacaoCTes = repComponentePrestacaoCTe.BuscarPorCTe(cargaCTe.CTe.Codigo);

            foreach (Dominio.Entidades.ComponentePrestacaoCTE componentePrestacaoCTE in componentesPrestacaoCTes)
            {
                if (componentePrestacaoCTE.ComponenteFrete == null)
                    continue;

                Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete cargaCTeComponenteFrete = new Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete
                {
                    TipoComponenteFrete = componentePrestacaoCTE.ComponenteFrete.TipoComponenteFrete,
                    ValorComponente = componentePrestacaoCTE.Valor,
                    CargaCTe = cargaCTe,
                    ComponenteFrete = componentePrestacaoCTE.ComponenteFrete
                };

                repCargaCTeComponentesFrete.Inserir(cargaCTeComponenteFrete);
            }
        }

        private void CriarComponentePadraoComplemento(Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTeComplementoInfo, Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete repCargaCTeComponentesFrete = new Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete cargaCTeComponenteFrete = new Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete
            {
                TipoComponenteFrete = cargaCTeComplementoInfo.ComponenteFrete.TipoComponenteFrete,
                ValorComponente = cargaCTeComplementoInfo.ValorComplemento,
                CargaCTe = cargaCTe,
                ComponenteFrete = cargaCTeComplementoInfo.ComponenteFrete
            };

            repCargaCTeComponentesFrete.Inserir(cargaCTeComponenteFrete);
        }

        private void CriarVeiculosCTeImportado(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, Dominio.Entidades.ConhecimentoDeTransporteEletronico cteImportado, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.VeiculoCTE repVeiculoCTe = new Repositorio.VeiculoCTE(unitOfWork);

            List<Dominio.Entidades.VeiculoCTE> veiculosCTe = repVeiculoCTe.BuscarPorCTe(cteImportado.Codigo);

            foreach (Dominio.Entidades.VeiculoCTE veiculoCTe in veiculosCTe)
                repVeiculoCTe.Deletar(veiculoCTe);

            if (ocorrencia.Carga?.Veiculo != null)
            {
                Dominio.Entidades.VeiculoCTE veiculoCTe = new Dominio.Entidades.VeiculoCTE()
                {
                    CTE = cteImportado,
                    Veiculo = ocorrencia.Carga.Veiculo,
                    ImportadoCarga = true
                };

                veiculoCTe.SetarDadosVeiculo(ocorrencia.Carga.Veiculo);

                repVeiculoCTe.Inserir(veiculoCTe);
            }

            foreach (Dominio.Entidades.Veiculo reboque in ocorrencia.Carga.VeiculosVinculados)
            {
                Dominio.Entidades.VeiculoCTE veiculoCTe = new Dominio.Entidades.VeiculoCTE()
                {
                    CTE = cteImportado,
                    Veiculo = reboque,
                    ImportadoCarga = true
                };

                veiculoCTe.SetarDadosVeiculo(reboque);

                repVeiculoCTe.Inserir(veiculoCTe);
            }
        }

        private void CriarMotoristasCTeImportado(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, Dominio.Entidades.ConhecimentoDeTransporteEletronico cteImportado, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.MotoristaCTE repMotoristaCTe = new Repositorio.MotoristaCTE(unitOfWork);

            List<Dominio.Entidades.MotoristaCTE> motoristasCTe = repMotoristaCTe.BuscarPorCTe(cteImportado.Codigo);

            foreach (Dominio.Entidades.MotoristaCTE motoristaCTe in motoristasCTe)
                repMotoristaCTe.Deletar(motoristaCTe);

            if (ocorrencia.Carga?.Motoristas != null)
            {
                foreach (Dominio.Entidades.Usuario motoristaCarga in ocorrencia.Carga.Motoristas)
                {
                    Dominio.Entidades.MotoristaCTE motoristaCTe = new Dominio.Entidades.MotoristaCTE()
                    {
                        CTE = cteImportado,
                        CPFMotorista = motoristaCarga.CPF,
                        NomeMotorista = motoristaCarga.Nome,
                    };

                    repMotoristaCTe.Inserir(motoristaCTe);
                }
            }
        }

        private bool EmitirDocumentoComplementarPorFechamento(Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCteComplementarInfo, ref List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> DocumentosParaEmissao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, ref string mensagem, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal, List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfoContaContabilContabilizacao> cargasCTeComplementoInfoContaContabilContabilizacao)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete repCargaCTeComponentesFrete = new Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaOcorrenciaCTeComplementarGlobalizadoNotaFiscal repCargaOcorrenciaCTeComplementarGlobalizadoNotaFiscal = new Repositorio.Embarcador.Cargas.CargaOcorrenciaCTeComplementarGlobalizadoNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Fechamento.FechamentoFrete repFechamentoFrete = new Repositorio.Embarcador.Fechamento.FechamentoFrete(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);

            CTe serCte = new CTe(unitOfWork);
            Servicos.Embarcador.Carga.ISS serCargaISS = new Servicos.Embarcador.Carga.ISS(unitOfWork);
            Servicos.Embarcador.Carga.ICMS serICMS = new ICMS(unitOfWork);
            Servicos.Embarcador.Carga.CTeComplementar servicoCTeComplementar = new Servicos.Embarcador.Carga.CTeComplementar(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();

            // Se ja foi gerado CTe ou se a origem da ocorrencia foi por carga mas não ha nenhuma carga na ocorrencia
            if (cargaCteComplementarInfo.CTe != null)
                return false;


            Dominio.Entidades.ConhecimentoDeTransporteEletronico cteComplementado = cargaCteComplementarInfo.CargaCTeComplementado.CTe;
            Dominio.Entidades.Empresa empresa = cteComplementado.Empresa;
            Dominio.ObjetosDeValor.CTe.CTe cte = new Dominio.ObjetosDeValor.CTe.CTe
            {
                Documentos = new List<Dominio.ObjetosDeValor.CTe.Documento>()
            };

            if (modeloDocumentoFiscal == null)
                modeloDocumentoFiscal = cteComplementado.ModeloDocumentoFiscal;

            if (empresa == null)
                throw new Exception("Nenhuma empresa para emissão.");

            if (cteComplementado == null)
                throw new Exception("Nenhum documento anterior encontrado.");

            bool complementoICMS = false;

            cargaCteComplementarInfo.ValorComplemento = Math.Abs(cargaCteComplementarInfo.ValorComplemento);

            Dominio.Entidades.EmpresaSerie empresaSerieModelo = null;
            if (modeloDocumentoFiscal != null)
            {  //Busca a série configurada para o modelo do documento fiscal
                modeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorId(modeloDocumentoFiscal.Codigo);
                if (modeloDocumentoFiscal.Series.Count > 0)
                    empresaSerieModelo = (from obj in modeloDocumentoFiscal.Series where obj.Empresa.Codigo == empresa.Codigo select obj).FirstOrDefault();
            }

            if (empresaSerieModelo != null)
                cte.Serie = empresaSerieModelo.Numero;
            else if (empresa.Configuracao != null && empresa.Configuracao.SerieCTeComplementar != null)
                cte.Serie = empresa.Configuracao.SerieCTeComplementar.Numero;
            else
                cte.Serie = cteComplementado.Serie.Numero;

            foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal in cteComplementado.XMLNotaFiscais)
            {
                Dominio.ObjetosDeValor.CTe.Documento docNF = serCte.BuscarDocumentoCTe(xmlNotaFiscal, empresa.TipoAmbiente, configuracaoEmbarcador);
                cte.Documentos.Add(docNF);
            }

            List<Dominio.ObjetosDeValor.Embarcador.CTe.CTe> ctesAnteriores = ObterCTesAnteriores(cteComplementado);

            cte.ValorTotalMercadoria = cteComplementado.ValorTotalMercadoria;
            cte.ProdutoPredominante = cteComplementado.ProdutoPredominante;
            List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico> componenteFreteDinamicos = new List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico>();

            if (cargaCteComplementarInfo.ComponenteFrete != null)
            {
                if (!complementoICMS)
                {
                    Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico componenteFreteDinamico = new Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico();
                    componenteFreteDinamico.ComponenteFrete = cargaCteComplementarInfo.ComponenteFrete;
                    componenteFreteDinamico.OutraDescricaoCTe = cargaCteComplementarInfo.ComponenteFrete.DescricaoCTe;
                    componenteFreteDinamico.IncluirBaseCalculoImposto = cargaCteComplementarInfo.IncluirICMSFrete;
                    componenteFreteDinamico.IncluirIntegralmenteContratoFreteTerceiro = false;
                    componenteFreteDinamico.TipoComponenteFrete = cargaCteComplementarInfo.FechamentoFrete.Contrato.ComponenteFreteValorContrato?.TipoComponenteFrete ?? cargaCteComplementarInfo.ComponenteFrete.TipoComponenteFrete;
                    componenteFreteDinamico.TipoValor = TipoCampoValorTabelaFrete.ValorFixo;
                    componenteFreteDinamico.ValorComponente = cargaCteComplementarInfo.ValorComplemento;
                    componenteFreteDinamicos.Add(componenteFreteDinamico);
                }
            }
            else
            {
                if (!complementoICMS)
                    cte.ValorFrete += cargaCteComplementarInfo.ValorComplemento;
            }

            cte.ValorAReceber += cte.ValorFrete;
            cte.ValorTotalPrestacaoServico += cte.ValorFrete;

            cte.CIOT = cteComplementado.CIOT;
            cte.indicadorIETomador = cteComplementado.IndicadorIETomador != null && cteComplementado.IndicadorIETomador == IndicadorIE.ContribuinteICMS ? IndicadorIE.ContribuinteICMS :
                                     cteComplementado.IndicadorIETomador != null && cteComplementado.IndicadorIETomador == IndicadorIE.ContribuinteIsento ? IndicadorIE.ContribuinteIsento :
                                     cteComplementado.IndicadorIETomador != null && cteComplementado.IndicadorIETomador == IndicadorIE.NaoContribuinte ? IndicadorIE.NaoContribuinte :
                                     IndicadorIE.ContribuinteICMS;
            cte.indicadorGlobalizado = cteComplementado.IndicadorGlobalizado;


            Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraICMS = BuscarRegraICMS(cargaCteComplementarInfo, empresa, cteComplementado.LocalidadeInicioPrestacao, tipoServicoMultisoftware, unitOfWork, out mensagem, configuracaoEmbarcador, complementoICMS);
            if (!string.IsNullOrWhiteSpace(mensagem))
                return false;

            if (configuracaoEmbarcador.Pais == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPais.Exterior && regraICMS.CFOP == 0)
            {
                Repositorio.CFOP repCFOP = new Repositorio.CFOP(unitOfWork);
                regraICMS.CFOP = repCFOP.BuscarPrimeiroPorTipo(Dominio.Enumeradores.TipoCFOP.Saida)?.CodigoCFOP ?? 0;
            }

            if (!regraICMS.IncluirICMSBC && complementoICMS)
            {
                if (configuracaoEmbarcador.UtilizarRegraICMSParaDescontarValorICMS)
                {
                    if (regraICMS.DescontarICMSDoValorAReceber)
                        cte.ValorAReceber = regraICMS.ValorICMS + regraICMS.ValorPis + regraICMS.ValorCofins;
                }
                else if (regraICMS.CST == "60")
                    cte.ValorAReceber = regraICMS.ValorICMS + regraICMS.ValorPis + regraICMS.ValorCofins;
            }

            Dominio.ObjetosDeValor.Embarcador.ISS.RegraISS regraISS = BuscarRegraISS(cargaCteComplementarInfo, tipoServicoMultisoftware, unitOfWork, out mensagem, configuracaoEmbarcador);
            Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS impostoIBSCBS = servicoCTeComplementar.ObterRetornoImpostoIBSCBS(cargaCteComplementarInfo);

            Dominio.Enumeradores.TipoPagamento tipoPagamento = cargaCteComplementarInfo.TipoPagamento;
            Dominio.Entidades.Cliente tomador = cargaCteComplementarInfo.TomadorPagador;
            Dominio.Enumeradores.TipoTomador tipoTomador = cargaCteComplementarInfo.TipoTomador;

            Dominio.Entidades.Cliente remetente = repCliente.BuscarPorCPFCNPJ(double.Parse(cteComplementado.Remetente.CPF_CNPJ));
            Dominio.Entidades.Cliente destinatario = repCliente.BuscarPorCPFCNPJ(double.Parse(cteComplementado.Destinatario.CPF_CNPJ));
            Dominio.Entidades.Cliente expedidor = null;
            Dominio.Entidades.Cliente recebedor = null;

            if (cteComplementado.Expedidor != null)
                expedidor = repCliente.BuscarPorCPFCNPJ(double.Parse(cteComplementado.Expedidor.CPF_CNPJ));

            if (cteComplementado.Recebedor != null)
                recebedor = repCliente.BuscarPorCPFCNPJ(double.Parse(cteComplementado.Recebedor.CPF_CNPJ));

            Dominio.Enumeradores.TipoCTE tipoCTe = cargaCteComplementarInfo.TipoCTE;
            Dominio.Enumeradores.TipoServico tipoServico = cargaCteComplementarInfo.TipoServico;

            cte.ChaveCTESubstituicaoComplementar = cteComplementado.Chave;

            Dominio.Entidades.Localidade origem = cteComplementado.LocalidadeInicioPrestacao;
            Dominio.Entidades.Localidade destino = cteComplementado.LocalidadeTerminoPrestacao;

            if (modeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe || modeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS)//todo: quando for nota de serviço sempre usar o destino como localidade final, rever isso, pois pode ser que a prestão seja prestada em outro município.
                origem = destino;

            List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga> quantidades = ObterQuantidades(cteComplementado);
            List<Dominio.ObjetosDeValor.Embarcador.CTe.Seguro> seguros = ObterSeguros(cteComplementado);

            string observacaoCTe = cargaCteComplementarInfo.ObservacaoCTe ?? "";


            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = null;
            List<string> rotas = new List<string>();
            int tipoEnvio = 0;

            Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaCteComplementarInfo.CargaCTeComplementado.Carga;

            List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.ConfiguracaoContabil> configuracoes = Servicos.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil.ConverterCargaCTeComplementoInfoContaContabilContabilizacao(cargaCteComplementarInfo, cargasCTeComplementoInfoContaContabilContabilizacao);
            Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = serCte.GerarCTe(carga, null, cte, empresa, remetente, destinatario, tomador, expedidor, recebedor, origem, destino, null, null, tipoPagamento, tipoTomador, quantidades, componenteFreteDinamicos, observacaoCTe, null, pedido, rotas, seguros, regraICMS, regraISS, impostoIBSCBS, tipoServicoMultisoftware, modeloDocumentoFiscal, tipoServico, tipoCTe, ctesAnteriores, tipoEnvio, true, null, true, unitOfWork, configuracaoEmbarcador, configuracaoGeralCarga, cargaCteComplementarInfo.CentroResultado, cargaCteComplementarInfo.CentroResultadoDestinatario, cargaCteComplementarInfo.ItemServico, cargaCteComplementarInfo.CentroResultadoEscrituracao, cargaCteComplementarInfo.CentroResultadoICMS, cargaCteComplementarInfo.CentroResultadoPIS, cargaCteComplementarInfo.CentroResultadoCOFINS, cargaCteComplementarInfo.ValorMaximoCentroContabilizacao, configuracoes, cteComplementado.XMLNotaFiscais.ToList(), null, configuracaoEmbarcador.DescricaoComponenteImpostoCTe, configuracaoEmbarcador.DescricaoComponentePadraoCTe);

            cargaCteComplementarInfo.CTe = cargaCTe.CTe;
            cargaCteComplementarInfo.PreCTe = cargaCTe.PreCTe;

            if (cargaCTe.CTe != null && cteComplementado != null)
            {
                Repositorio.Embarcador.CTe.CTeDocumentoOriginario repDocumentoOriginario = new Repositorio.Embarcador.CTe.CTeDocumentoOriginario(unitOfWork);

                Dominio.Entidades.Embarcador.CTe.CTeDocumentoOriginario documentoOriginario = new Dominio.Entidades.Embarcador.CTe.CTeDocumentoOriginario()
                {
                    CTe = cargaCTe.CTe,
                    Chave = cteComplementado.Chave,
                    Numero = cteComplementado.Numero,
                    Serie = cteComplementado.Serie?.Numero.ToString(),
                    DataEmissao = cteComplementado.DataEmissao
                };

                repDocumentoOriginario.Inserir(documentoOriginario);
            }

            repCargaCTeComplementoInfo.Atualizar(cargaCteComplementarInfo);

            cargaCTe.CargaCTeComplementoInfo = cargaCteComplementarInfo;

            if (!DocumentosParaEmissao.Contains(cargaCTe.CTe))
                DocumentosParaEmissao.Add(cargaCTe.CTe);


            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe> cargaPedidosXMLNotaFiscaisCTeComplementado = null;

            if (cargaCteComplementarInfo.CargaCTeComplementado != null)
                cargaPedidosXMLNotaFiscaisCTeComplementado = repCargaPedidoXMLNotaFiscalCTe.BuscarCargaPedidoXMLNotaFiscalCTePorCargaCTe(cargaCteComplementarInfo.CargaCTeComplementado.Codigo);
            else
                cargaPedidosXMLNotaFiscaisCTeComplementado = repCargaPedidoXMLNotaFiscalCTe.BuscarCargaPedidoXMLNotaFiscalCTePorCTe(cteComplementado.Codigo);

            if ((cargaCteComplementarInfo.CargaOcorrencia?.Codigo > 0) && (cargaCteComplementarInfo.CargaCTeComplementado?.Codigo > 0) && (cteComplementado?.IndicadorGlobalizado == Dominio.Enumeradores.OpcaoSimNao.Sim))
            {
                List<int> codigosNotaFiscalCTeComplementarGlobalizado = repCargaOcorrenciaCTeComplementarGlobalizadoNotaFiscal.BuscarXMLNotaFiscalPorCargaOcorrenciaECargaCTe(cargaCteComplementarInfo.CargaOcorrencia.Codigo, cargaCteComplementarInfo.CargaCTeComplementado.Codigo);

                if (codigosNotaFiscalCTeComplementarGlobalizado.Count > 0)
                    cargaPedidosXMLNotaFiscaisCTeComplementado = cargaPedidosXMLNotaFiscaisCTeComplementado.Where(o => codigosNotaFiscalCTeComplementarGlobalizado.Contains(o.PedidoXMLNotaFiscal.XMLNotaFiscal.Codigo)).ToList();
            }

            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete repPedidoXMLNotaFiscalComponenteFrete = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete(unitOfWork);

            if (cargaPedidosXMLNotaFiscaisCTeComplementado != null && cargaPedidosXMLNotaFiscaisCTeComplementado.Count > 0)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe ultimaCargaPedidoXMLNotaFiscalCTeComplementado = cargaPedidosXMLNotaFiscaisCTeComplementado.Last();
                decimal totalComponente = 0;
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe cargaPedidoXMLNotaFiscalCTeComplementado in cargaPedidosXMLNotaFiscaisCTeComplementado)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe cargaPedidoXMLNotaFiscalCTE = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe
                    {
                        CargaCTe = cargaCTe,
                        PedidoXMLNotaFiscal = cargaPedidoXMLNotaFiscalCTeComplementado.PedidoXMLNotaFiscal
                    };
                    repCargaPedidoXMLNotaFiscalCTe.Inserir(cargaPedidoXMLNotaFiscalCTE);


                    decimal valorCompoenteRateado = Math.Round((cargaCteComplementarInfo.ValorComplemento / cargaPedidosXMLNotaFiscaisCTeComplementado.Count), 2, MidpointRounding.AwayFromZero);
                    totalComponente += valorCompoenteRateado;

                    Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete = cargaCTe?.Carga?.TabelaFrete;
                    bool destacarComponenteTabelaFrete = Servicos.Embarcador.Carga.Frete.DestacarComponenteTabelaFrete(tabelaFrete, cargaCteComplementarInfo.ComponenteFrete);

                    Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete pedidoXMLNotaFiscalComponenteFrete = new Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete()
                    {
                        PedidoXMLNotaFiscal = cargaPedidoXMLNotaFiscalCTeComplementado.PedidoXMLNotaFiscal,
                        TipoComponenteFrete = cargaCteComplementarInfo.ComponenteFrete?.TipoComponenteFrete ?? TipoComponenteFrete.OUTROS,
                        ComponenteFrete = cargaCteComplementarInfo.ComponenteFrete,
                        AcrescentaValorTotalAReceber = cargaCteComplementarInfo.ComponenteFrete?.AcrescentaValorTotalAReceber ?? false,
                        DescontarValorTotalAReceber = cargaCteComplementarInfo.ComponenteFrete?.DescontarValorTotalAReceber ?? false,
                        NaoSomarValorTotalAReceber = (destacarComponenteTabelaFrete ? tabelaFrete?.NaoSomarValorTotalAReceber : cargaCteComplementarInfo.ComponenteFrete?.NaoSomarValorTotalAReceber) ?? false,
                        DescontarDoValorAReceberValorComponente = (destacarComponenteTabelaFrete ? tabelaFrete?.DescontarDoValorAReceberValorComponente : cargaCteComplementarInfo.ComponenteFrete?.DescontarValorTotalAReceber) ?? false,
                        DescontarDoValorAReceberOICMSDoComponente = tabelaFrete?.DescontarDoValorAReceberOICMSDoComponente ?? false,
                        ValorICMSComponenteDestacado = tabelaFrete?.ValorICMSComponenteDestacado ?? 0,
                        NaoSomarValorTotalPrestacao = (destacarComponenteTabelaFrete ? tabelaFrete?.NaoSomarValorTotalPrestacao : cargaCteComplementarInfo.ComponenteFrete?.NaoSomarValorTotalPrestacao) ?? false,
                        IncluirBaseCalculoICMS = cargaCteComplementarInfo.IncluirICMSFrete,
                        TipoValor = TipoCampoValorTabelaFrete.ValorFixo,
                        IncluirIntegralmenteContratoFreteTerceiro = false
                    };

                    if (cargaPedidoXMLNotaFiscalCTeComplementado.Codigo == ultimaCargaPedidoXMLNotaFiscalCTeComplementado.Codigo)
                    {
                        decimal diferenca = cargaCteComplementarInfo.ValorComplemento - totalComponente;
                        valorCompoenteRateado += diferenca;
                    }
                    pedidoXMLNotaFiscalComponenteFrete.ValorComponente = valorCompoenteRateado;
                    repPedidoXMLNotaFiscalComponenteFrete.Inserir(pedidoXMLNotaFiscalComponenteFrete);
                }
            }
            return true;
        }

        private bool EmitirDocumentoComplementarPorCargas(Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCteComplementarInfo, ref List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> DocumentosParaEmissao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, ref string mensagem, Repositorio.UnitOfWork unitOfWork, List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfoContaContabilContabilizacao> cargasCTeComplementoInfoContaContabilContabilizacao)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete repCargaCTeComponentesFrete = new Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaOcorrenciaCTeComplementarGlobalizadoNotaFiscal repCargaOcorrenciaCTeComplementarGlobalizadoNotaFiscal = new Repositorio.Embarcador.Cargas.CargaOcorrenciaCTeComplementarGlobalizadoNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Rateio.RateioFormula repFormulaRateio = new Repositorio.Embarcador.Rateio.RateioFormula(unitOfWork);

            Dominio.Entidades.Embarcador.Rateio.RateioFormula rateioFormula = null;

            if (cargaCteComplementarInfo.CargaOcorrencia?.TipoOcorrencia.TipoRateio != null && cargaCteComplementarInfo.CargaOcorrencia.TipoOcorrencia.TipoRateio.Value != ParametroRateioFormula.todos)
                rateioFormula = repFormulaRateio.BuscarPorTipo(cargaCteComplementarInfo.CargaOcorrencia.TipoOcorrencia.TipoRateio.Value);

            Servicos.Embarcador.Carga.RateioFormula svcRateio = new RateioFormula(unitOfWork);

            CTe serCte = new CTe(unitOfWork);
            Servicos.Embarcador.Carga.ISS serCargaISS = new Servicos.Embarcador.Carga.ISS(unitOfWork);
            Servicos.Embarcador.Carga.ICMS serICMS = new ICMS(unitOfWork);
            Servicos.Embarcador.Carga.CTeComplementar servicoCTeComplementar = new Servicos.Embarcador.Carga.CTeComplementar(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();

            // Se ja foi gerado CTe ou se a origem da ocorrencia foi por carga mas não ha nenhuma carga na ocorrencia
            if (cargaCteComplementarInfo.CTe != null || (cargaCteComplementarInfo.CargaOcorrencia.OrigemOcorrencia == OrigemOcorrencia.PorCarga && cargaCteComplementarInfo.CargaOcorrencia.Carga == null))
                return false;

            Dominio.Entidades.Empresa empresa = null;
            if (cargaCteComplementarInfo.CargaOcorrencia.OrigemOcorrencia == OrigemOcorrencia.PorCarga)
            {
                if (!cargaCteComplementarInfo.ComplementoFilialEmissora)
                {
                    if ((tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && cargaCteComplementarInfo.CargaCTeComplementado.CTe.Empresa != null) || (cargaCteComplementarInfo.CargaOcorrencia.Usuario != null && cargaCteComplementarInfo.CargaOcorrencia.Usuario.TipoAcesso == Dominio.Enumeradores.TipoAcesso.Terceiro) || cargaCteComplementarInfo.CargaOcorrencia.TipoOcorrencia.OcorrenciaTerceiros)
                        empresa = cargaCteComplementarInfo.CargaCTeComplementado.CTe.Empresa;
                    else
                        empresa = cargaCteComplementarInfo.CargaOcorrencia.Carga.Empresa;
                }
                else
                    empresa = cargaCteComplementarInfo.CargaCTeComplementado.CTe.Empresa;
            }
            else
                empresa = cargaCteComplementarInfo.CargaOcorrencia.Emitente;

            if (empresa == null)
                throw new Exception("Nenhuma empresa para emissão.");

            Dominio.Entidades.ConhecimentoDeTransporteEletronico cteComplementado = null;
            Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal = null;
            Dominio.ObjetosDeValor.CTe.CTe cte = new Dominio.ObjetosDeValor.CTe.CTe
            {
                Documentos = new List<Dominio.ObjetosDeValor.CTe.Documento>()
            };

            if (cargaCteComplementarInfo.CargaCTeComplementado != null)
            {
                if (cargaCteComplementarInfo.CargaCTeComplementado.CTe != null)
                {
                    if (cargaCteComplementarInfo.CargaCTeComplementado.CTe.TipoCTE != Dominio.Enumeradores.TipoCTE.Complemento)
                        cteComplementado = cargaCteComplementarInfo.CargaCTeComplementado.CTe;
                    else
                        cteComplementado = repCTe.BuscarPorChaveComFetch(cargaCteComplementarInfo.CargaCTeComplementado.CTe.ChaveCTESubComp);
                }
            }
            else if (cargaCteComplementarInfo.CargaOcorrencia.Cargas != null && cargaCteComplementarInfo.CargaOcorrencia.Cargas.Count > 0)
            {
                //var cteComLocalidaDoTomador = from o in cargaCteComplementarInfo.CargaOcorrencia.Cargas.FirstOrDefault().CargaCTes
                //                              where o.CTe != null && o.CargaCTeComplementoInfo == null 
                //                              && o.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.NFS
                //                              select o.CTe;
                //cteComplementado = cteComLocalidaDoTomador.FirstOrDefault();

                var cteComLocalidaDoTomador = from obj in cargaCteComplementarInfo.CargaOcorrencia.Cargas
                                              where obj.CargaCTes.Any(o => o.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.NFS)
                                              select obj.CargaCTes.FirstOrDefault().CTe;

                cteComplementado = cteComLocalidaDoTomador.FirstOrDefault();
            }

            if (cteComplementado == null)
                throw new ServicoException("Nenhum documento anterior encontrado.");

            bool ocorrenciaPossuiDocumentoFiscalOutros = cargaCteComplementarInfo.CargaOcorrencia.ModeloDocumentoFiscal?.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.Outros;
            bool shouldGerarDocumentoFiscal = cteComplementado.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.NFS || ocorrenciaPossuiDocumentoFiscalOutros;

            if (shouldGerarDocumentoFiscal)
            {
                bool complementoICMS = false;
                bool gerarControlesNFSManual = false;

                if (cargaCteComplementarInfo.ComponenteFrete != null && cargaCteComplementarInfo.ComponenteFrete.TipoComponenteFrete == TipoComponenteFrete.ICMS)
                    complementoICMS = true;

                if (cargaCteComplementarInfo.CargaOcorrencia != null && cargaCteComplementarInfo.CargaOcorrencia.ModeloDocumentoFiscal != null)
                {
                    if (cargaCteComplementarInfo.CargaOcorrencia.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS)
                    {
                        modeloDocumentoFiscal = cargaCteComplementarInfo.CargaOcorrencia.ModeloDocumentoFiscalEmissaoMunicipal;
                        gerarControlesNFSManual = true;
                    }
                    else
                        modeloDocumentoFiscal = cargaCteComplementarInfo.CargaOcorrencia.ModeloDocumentoFiscal;
                }
                else if (cargaCteComplementarInfo.CargaOcorrencia?.Carga != null)
                {
                    modeloDocumentoFiscal = ObterModeloDocumentoFiscalPorCarga(cargaCteComplementarInfo, cteComplementado, tipoServicoMultisoftware, unitOfWork);
                }
                else
                {
                    modeloDocumentoFiscal = cteComplementado.ModeloDocumentoFiscal;
                }

                Dominio.Entidades.EmpresaSerie empresaSerieModelo = null;
                if (modeloDocumentoFiscal != null)
                {  //Busca a série configurada para o modelo do documento fiscal
                    modeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorId(modeloDocumentoFiscal.Codigo);
                    if (modeloDocumentoFiscal.Series.Count > 0)
                        empresaSerieModelo = (from obj in modeloDocumentoFiscal.Series where obj.Empresa.Codigo == empresa.Codigo select obj).FirstOrDefault();
                }

                if (empresaSerieModelo != null)
                    cte.Serie = empresaSerieModelo.Numero;
                else if (cargaCteComplementarInfo.CargaOcorrencia != null && cargaCteComplementarInfo.CargaOcorrencia.Emitente != null && cargaCteComplementarInfo.CargaOcorrencia.Emitente.Configuracao != null && cargaCteComplementarInfo.CargaOcorrencia.Emitente.Configuracao.SerieCTeComplementar != null)
                    cte.Serie = cargaCteComplementarInfo.CargaOcorrencia.Emitente.Configuracao.SerieCTeComplementar.Numero;
                else if (empresa.Configuracao != null && empresa.Configuracao.SerieCTeComplementar != null)
                    cte.Serie = empresa.Configuracao.SerieCTeComplementar.Numero;

                if (cteComplementado.XMLNotaFiscais.Count > 0)
                {
                    foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal in cteComplementado.XMLNotaFiscais)
                    {
                        Dominio.ObjetosDeValor.CTe.Documento docNF = serCte.BuscarDocumentoCTe(xmlNotaFiscal, empresa.TipoAmbiente, configuracaoEmbarcador);
                        cte.Documentos.Add(docNF);
                    }
                }
                else if (cteComplementado.Documentos.Count > 0)
                {
                    foreach (Dominio.Entidades.DocumentosCTE documentoCTe in cteComplementado.Documentos)
                    {
                        Dominio.ObjetosDeValor.CTe.Documento docNF = serCte.BuscarDocumentoCTe(documentoCTe, empresa.TipoAmbiente, configuracaoEmbarcador);
                        cte.Documentos.Add(docNF);
                    }
                }

                List<Dominio.ObjetosDeValor.Embarcador.CTe.CTe> ctesAnteriores = ObterCTesAnteriores(cteComplementado);

                cte.ValorTotalMercadoria = cteComplementado.ValorTotalMercadoria;
                cte.ProdutoPredominante = cteComplementado.ProdutoPredominante;

                List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico> componentesFrete = new List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico>();

                if (!complementoICMS)
                {
                    cte.Moeda = cargaCteComplementarInfo.Moeda;
                    cte.ValorCotacaoMoeda = cargaCteComplementarInfo.ValorCotacaoMoeda;
                    cte.ValorTotalMoeda = cargaCteComplementarInfo.ValorTotalMoeda;
                    cte.ValorAReceber += cargaCteComplementarInfo.ValorComplemento;
                    cte.ValorFrete += cargaCteComplementarInfo.ValorComplemento;
                    cte.ValorTotalPrestacaoServico += cargaCteComplementarInfo.ValorComplemento;

                    if (cargaCteComplementarInfo.CargaOcorrencia?.TipoOcorrencia.AdicionarPISCOFINS ?? false)
                    {
                        //cte.ValorAReceber += cargaCteComplementarInfo.ValorPISCOFINS;
                        //cte.ValorTotalPrestacaoServico += cargaCteComplementarInfo.ValorPISCOFINS;
                        componentesFrete.Add(new Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico()
                        {
                            TipoComponenteFrete = TipoComponenteFrete.OUTROS,
                            ValorComponente = cargaCteComplementarInfo.ValorPISCOFINS,
                            IncluirBaseCalculoImposto = true,
                            TipoValor = TipoCampoValorTabelaFrete.AumentoValor,
                            OutraDescricaoCTe = "PIS/COFINS",
                            IncluirIntegralmenteContratoFreteTerceiro = false
                        });
                    }
                }

                cte.TipoCTeIntegracao = cargaCteComplementarInfo.CargaOcorrencia?.TipoOcorrencia?.TipoCTeIntegracao;
                cte.TipoConhecimentoProceda = cargaCteComplementarInfo.CargaOcorrencia?.TipoOcorrencia?.TipoConhecimentoProceda;
                cte.CaracteristicaTransporte = cargaCteComplementarInfo.CargaOcorrencia?.TipoOcorrencia?.CaracteristicaAdicionalCTe;
                cte.CIOT = cteComplementado.CIOT;
                cte.indicadorIETomador = cteComplementado.IndicadorIETomador != null && cteComplementado.IndicadorIETomador == IndicadorIE.ContribuinteICMS ? IndicadorIE.ContribuinteICMS :
                                         cteComplementado.IndicadorIETomador != null && cteComplementado.IndicadorIETomador == IndicadorIE.ContribuinteIsento ? IndicadorIE.ContribuinteIsento :
                                         cteComplementado.IndicadorIETomador != null && cteComplementado.IndicadorIETomador == IndicadorIE.NaoContribuinte ? IndicadorIE.NaoContribuinte :
                                         IndicadorIE.ContribuinteICMS;
                cte.indicadorGlobalizado = cteComplementado.IndicadorGlobalizado;

                Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraICMS = BuscarRegraICMS(cargaCteComplementarInfo, empresa, cteComplementado.LocalidadeInicioPrestacao, tipoServicoMultisoftware, unitOfWork, out mensagem, configuracaoEmbarcador, complementoICMS);
                if (!string.IsNullOrWhiteSpace(mensagem))
                    return false;

                if (configuracaoEmbarcador.Pais == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPais.Exterior && regraICMS.CFOP == 0)
                {
                    Repositorio.CFOP repCFOP = new Repositorio.CFOP(unitOfWork);
                    regraICMS.CFOP = repCFOP.BuscarPrimeiroPorTipo(Dominio.Enumeradores.TipoCFOP.Saida)?.CodigoCFOP ?? 0;
                }

                if (!regraICMS.IncluirICMSBC && complementoICMS)
                {
                    if (configuracaoEmbarcador.UtilizarRegraICMSParaDescontarValorICMS)
                    {
                        if (regraICMS.DescontarICMSDoValorAReceber)
                            cte.ValorAReceber = regraICMS.ValorICMS + regraICMS.ValorPis + regraICMS.ValorCofins;
                    }
                    else if (regraICMS.CST == "60")
                        cte.ValorAReceber = regraICMS.ValorICMS + regraICMS.ValorPis + regraICMS.ValorCofins;
                }

                Dominio.ObjetosDeValor.Embarcador.ISS.RegraISS regraISS = BuscarRegraISS(cargaCteComplementarInfo, tipoServicoMultisoftware, unitOfWork, out mensagem, configuracaoEmbarcador);
                Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS impostoIBSCBS = servicoCTeComplementar.ObterRetornoImpostoIBSCBS(cargaCteComplementarInfo);

                Dominio.Enumeradores.TipoPagamento tipoPagamento = cargaCteComplementarInfo.TipoPagamento;
                Dominio.Entidades.Cliente tomador = cargaCteComplementarInfo.TomadorPagador;
                Dominio.Enumeradores.TipoTomador tipoTomador = cargaCteComplementarInfo.TipoTomador;

                double cnpjDestinatario = 0;

                if (cteComplementado.Destinatario != null && cteComplementado.Destinatario?.Cliente != null)
                    cnpjDestinatario = cteComplementado.Destinatario.Cliente.CPF_CNPJ;
                else if (!string.IsNullOrWhiteSpace(cteComplementado.Destinatario?.CPF_CNPJ))
                    cnpjDestinatario = double.Parse(cteComplementado.Destinatario.CPF_CNPJ);

                Dominio.Entidades.Cliente remetente = null;
                Dominio.Entidades.Cliente destinatario = null;
                Dominio.Entidades.Cliente expedidor = null;
                Dominio.Entidades.Cliente recebedor = null;

                if (cteComplementado.Remetente != null)
                    remetente = cteComplementado.Remetente.Cliente ?? repCliente.BuscarPorCPFCNPJ(double.Parse(cteComplementado.Remetente.CPF_CNPJ));

                if (cnpjDestinatario > 0)
                    destinatario = repCliente.BuscarPorCPFCNPJ(cnpjDestinatario);

                if (cteComplementado.Expedidor != null && cteComplementado.Expedidor.Cliente != null)
                    expedidor = repCliente.BuscarPorCPFCNPJ(cteComplementado.Expedidor.Cliente.CPF_CNPJ);
                else if (!string.IsNullOrWhiteSpace(cteComplementado.Expedidor?.CPF_CNPJ))
                    expedidor = repCliente.BuscarPorCPFCNPJ(double.Parse(cteComplementado.Expedidor.CPF_CNPJ));

                if (cteComplementado.Recebedor != null && cteComplementado.Recebedor.Cliente != null)
                    recebedor = repCliente.BuscarPorCPFCNPJ(cteComplementado.Recebedor.Cliente.CPF_CNPJ);
                else if (!string.IsNullOrWhiteSpace(cteComplementado.Recebedor?.CPF_CNPJ))
                    recebedor = repCliente.BuscarPorCPFCNPJ(double.Parse(cteComplementado.Recebedor.CPF_CNPJ));

                Dominio.Enumeradores.TipoCTE tipoCTe = cargaCteComplementarInfo.TipoCTE;
                Dominio.Enumeradores.TipoServico tipoServico = cargaCteComplementarInfo.TipoServico;

                cte.ChaveCTESubstituicaoComplementar = cteComplementado.Chave;

                Dominio.Entidades.Localidade origem = cteComplementado.LocalidadeInicioPrestacao;
                Dominio.Entidades.Localidade destino = cteComplementado.LocalidadeTerminoPrestacao;
                Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco pedidoEnderecoDestinatario = ObterPedidoEndereco(cteComplementado.Destinatario);
                Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco pedidoEnderecoRemetente = ObterPedidoEndereco(cteComplementado.Remetente);

                if (cargaCteComplementarInfo.CargaOcorrencia.Responsavel.HasValue) //cargaCteComplementarInfo.CargaOcorrencia.Responsavel.Value.ToString() != "9"
                {
                    if (cargaCteComplementarInfo.CargaOcorrencia.Responsavel.Value != Dominio.Enumeradores.TipoTomador.NaoInformado && cargaCteComplementarInfo.CargaOcorrencia.Responsavel.Value != cteComplementado.TipoTomador)
                    {
                        cte.ChaveCTESubstituicaoComplementar = string.Empty;
                        if (cte.Documentos.Count == 0)
                        {
                            Dominio.ObjetosDeValor.CTe.Documento documento = new Dominio.ObjetosDeValor.CTe.Documento
                            {
                                Tipo = Dominio.Enumeradores.TipoDocumentoCTe.Outros,
                                Descricao = "COMPLEMENTO",
                                Numero = cteComplementado.Numero.ToString(),
                                ModeloDocumentoFiscal = "99",
                                DataEmissao = cteComplementado.DataEmissao.Value.ToString("dd/MM/yyyy"),
                                Valor = 0
                            };
                            cte.Documentos.Add(documento);
                        }
                    }
                }

                if (empresa.Codigo != cteComplementado.Empresa.Codigo && cargaCteComplementarInfo.CargaOcorrencia.Carga != null)
                {
                    cte.ChaveCTESubstituicaoComplementar = string.Empty;

                    if (cargaCteComplementarInfo.CargaOcorrencia.Carga.CargaTransbordo && tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                        origem = cargaCteComplementarInfo.CargaOcorrencia.Carga.Pedidos.FirstOrDefault().Origem;
                }

                if (gerarControlesNFSManual || modeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS || modeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe)//todo: quando for nota de serviço sempre usar o destino como localidade final, rever isso, pois pode ser que a prestão seja prestada em outro município.
                    origem = destino;


                List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga> quantidades = ObterQuantidades(cteComplementado);
                List<Dominio.ObjetosDeValor.Embarcador.CTe.Seguro> seguros = ObterSeguros(cteComplementado);

                string observacaoCTe = BuscarObservacaoPadraoOcorrencia(cargaCteComplementarInfo, cteComplementado, modeloDocumentoFiscal, unitOfWork);

                serCte.ObterDescricoesComponentesPadrao(tomador, cargaCteComplementarInfo.CargaOcorrencia?.Carga, configuracaoEmbarcador, unitOfWork, out string descricaoComponenteValorFrete, out string descricaoComponenteValorICMS);

                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = null;
                List<string> rotas = new List<string>();
                int tipoEnvio = 0;

                Dominio.Entidades.Embarcador.Cargas.Carga carga = null;
                if (cargaCteComplementarInfo.CargaOcorrencia.Carga != null)
                    carga = cargaCteComplementarInfo.CargaOcorrencia.Carga;
                else
                    carga = cargaCteComplementarInfo.CargaOcorrencia.Cargas.FirstOrDefault();

                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargasCTes = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = cargaCteComplementarInfo.CargaCTeComplementado != null ? repCargaPedidoXMLNotaFiscalCTe.BuscarPrimeiraCargaPedidoPorCargaCTe(cargaCteComplementarInfo.CargaCTeComplementado.Codigo) : null;
                pedido = cargaPedido?.Pedido;

                if (cargaCteComplementarInfo.CargaOcorrencia.OrigemOcorrencia == OrigemOcorrencia.PorPeriodo)
                {
                    List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.ConfiguracaoContabil> configuracoes = Servicos.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil.ConverterCargaCTeComplementoInfoContaContabilContabilizacao(cargaCteComplementarInfo, cargasCTeComplementoInfoContaContabilContabilizacao);
                    cargasCTes = serCte.GerarCTe(cargaCteComplementarInfo, cargaCteComplementarInfo.CargaOcorrencia.Cargas.ToList(), empresa, cte, remetente, destinatario, tomador, expedidor, recebedor, origem, destino, null, null, tipoPagamento, tipoTomador, quantidades, new List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico>(), observacaoCTe, pedido, rotas, seguros, regraICMS, regraISS, impostoIBSCBS, tipoServicoMultisoftware, modeloDocumentoFiscal, tipoServico, tipoCTe, ctesAnteriores, tipoEnvio, true, 0, unitOfWork, cteComplementado.XMLNotaFiscais.ToList(), descricaoComponenteValorFrete, descricaoComponenteValorICMS, configuracaoEmbarcador, configuracaoGeralCarga, cargaCteComplementarInfo.CentroResultado, cargaCteComplementarInfo.CentroResultadoDestinatario, cargaCteComplementarInfo.ItemServico, cargaCteComplementarInfo.CentroResultadoEscrituracao, cargaCteComplementarInfo.CentroResultadoICMS, cargaCteComplementarInfo.CentroResultadoPIS, cargaCteComplementarInfo.CentroResultadoCOFINS, cargaCteComplementarInfo.ValorMaximoCentroContabilizacao, configuracoes, cteComplementado);
                    cargaCteComplementarInfo.CTe = cargasCTes.FirstOrDefault().CTe;
                }
                else
                {
                    List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.ConfiguracaoContabil> configuracoes = Servicos.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil.ConverterCargaCTeComplementoInfoContaContabilContabilizacao(cargaCteComplementarInfo, cargasCTeComplementoInfoContaContabilContabilizacao);

                    Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = serCte.GerarCTe(carga.CargaAgrupamento != null ? carga.CargaAgrupamento : carga, cargaPedido, cte, empresa, remetente, destinatario, tomador, expedidor, recebedor, origem, destino, pedidoEnderecoRemetente, pedidoEnderecoDestinatario, tipoPagamento, tipoTomador, quantidades, componentesFrete, observacaoCTe, null, pedido, rotas, seguros, regraICMS, regraISS, impostoIBSCBS, tipoServicoMultisoftware, modeloDocumentoFiscal, tipoServico, tipoCTe, ctesAnteriores, tipoEnvio, true, null, true, unitOfWork, configuracaoEmbarcador, configuracaoGeralCarga, cargaCteComplementarInfo.CentroResultado, cargaCteComplementarInfo.CentroResultadoDestinatario, cargaCteComplementarInfo.ItemServico, cargaCteComplementarInfo.CentroResultadoEscrituracao, cargaCteComplementarInfo.CentroResultadoICMS, cargaCteComplementarInfo.CentroResultadoPIS, cargaCteComplementarInfo.CentroResultadoCOFINS, cargaCteComplementarInfo.ValorMaximoCentroContabilizacao, configuracoes, cteComplementado.XMLNotaFiscais.ToList(), null, "IMPOSTOS", "FRETE VALOR", null, cteComplementado, null, 0d, null, cteComplementado.Remetente, cteComplementado.Destinatario, cteComplementado.Expedidor, cteComplementado.Recebedor, naoGerarDocumentoAnterior: false, ocorrencia: cargaCteComplementarInfo.CargaOcorrencia);
                    cargaCTe.CargaOrigem = carga;
                    cargasCTes.Add(cargaCTe);
                    cargaCteComplementarInfo.CTe = cargaCTe.CTe;
                    cargaCteComplementarInfo.PreCTe = cargaCTe.PreCTe;
                    new Servicos.Embarcador.Documentos.GestaoDocumento(unitOfWork, configuracaoEmbarcador).DefinirCargaCTeComplementoInfoPorPreCTe(cargaCteComplementarInfo, tipoServicoMultisoftware);
                    //cargaCteComplementarInfo

                    if (cargaCTe.CTe != null && cteComplementado != null)
                    {
                        Repositorio.Embarcador.CTe.CTeDocumentoOriginario repDocumentoOriginario = new Repositorio.Embarcador.CTe.CTeDocumentoOriginario(unitOfWork);

                        Dominio.Entidades.Embarcador.CTe.CTeDocumentoOriginario documentoOriginario = new Dominio.Entidades.Embarcador.CTe.CTeDocumentoOriginario()
                        {
                            CTe = cargaCTe.CTe,
                            Chave = cteComplementado.Chave,
                            Numero = cteComplementado.Numero,
                            Serie = cteComplementado.Serie?.Numero.ToString(),
                            DataEmissao = cteComplementado.DataEmissao
                        };

                        repDocumentoOriginario.Inserir(documentoOriginario);
                    }
                }

                repCargaCTeComplementoInfo.Atualizar(cargaCteComplementarInfo);

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in cargasCTes)
                {
                    cargaCTe.CargaCTeComplementoInfo = cargaCteComplementarInfo;

                    if (cargaCTe.CTe != null)
                    {
                        if (cargaCteComplementarInfo.ComponenteFrete.ImprimirDescricaoComponenteEmComplementos)
                        {
                            cargaCteComplementarInfo.CTe.DescricaoComplemento = cargaCteComplementarInfo.ComponenteFrete.ImprimirOutraDescricaoCTe ? cargaCteComplementarInfo.ComponenteFrete.DescricaoCTe : cargaCteComplementarInfo.ComponenteFrete.DescricaoComponente;
                            repCTe.Atualizar(cargaCteComplementarInfo.CTe);
                        }

                        if (!DocumentosParaEmissao.Contains(cargaCTe.CTe))
                            DocumentosParaEmissao.Add(cargaCTe.CTe);
                    }
                    else
                    {
                        if (cargaCteComplementarInfo.CargaOcorrencia.Carga != null)
                        {
                            cargaCteComplementarInfo.CargaOcorrencia.Carga.AgImportacaoCTe = false;
                            repCarga.Atualizar(cargaCteComplementarInfo.CargaOcorrencia.Carga);
                        }
                        if (!(carga?.Empresa?.EmiteNFSeOcorrenciaForaEmbarcador ?? false))
                            cargaCteComplementarInfo.CargaOcorrencia.AgImportacaoCTe = true;
                        repCargaOcorrencia.Atualizar(cargaCteComplementarInfo.CargaOcorrencia);
                    }

                    Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete = cargaCTe?.Carga?.TabelaFrete;
                    bool destacarComponenteTabelaFrete = Servicos.Embarcador.Carga.Frete.DestacarComponenteTabelaFrete(tabelaFrete, cargaCteComplementarInfo.ComponenteFrete);

                    Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete cargaCTeComponenteFrete = new Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete
                    {
                        CargaCTe = cargaCTe,
                        TipoComponenteFrete = cargaCteComplementarInfo.ComponenteFrete.TipoComponenteFrete,
                        ComponenteFrete = cargaCteComplementarInfo.ComponenteFrete,
                        ValorComponente = cargaCteComplementarInfo.ValorComplemento,
                        AcrescentaValorTotalAReceber = cargaCteComplementarInfo.ComponenteFrete?.AcrescentaValorTotalAReceber ?? false,
                        DescontarValorTotalAReceber = cargaCteComplementarInfo.ComponenteFrete?.DescontarValorTotalAReceber ?? false,
                        NaoSomarValorTotalAReceber = (destacarComponenteTabelaFrete ? tabelaFrete.NaoSomarValorTotalAReceber : cargaCteComplementarInfo.ComponenteFrete?.NaoSomarValorTotalAReceber) ?? false,
                        DescontarDoValorAReceberValorComponente = (destacarComponenteTabelaFrete ? tabelaFrete.DescontarDoValorAReceberValorComponente : cargaCteComplementarInfo.ComponenteFrete?.DescontarValorTotalAReceber) ?? false,
                        DescontarDoValorAReceberOICMSDoComponente = tabelaFrete?.DescontarDoValorAReceberOICMSDoComponente ?? false,
                        ValorICMSComponenteDestacado = tabelaFrete?.ValorICMSComponenteDestacado ?? 0,
                        NaoSomarValorTotalPrestacao = (destacarComponenteTabelaFrete ? tabelaFrete.NaoSomarValorTotalPrestacao : cargaCteComplementarInfo.ComponenteFrete?.NaoSomarValorTotalPrestacao) ?? false,
                        IncluirBaseCalculoICMS = cargaCteComplementarInfo.IncluirICMSFrete,
                        TipoValor = TipoCampoValorTabelaFrete.ValorFixo,
                        Moeda = cargaCteComplementarInfo.Moeda,
                        ValorCotacaoMoeda = cargaCteComplementarInfo.ValorCotacaoMoeda,
                        ValorTotalMoeda = cargaCteComplementarInfo.ValorTotalMoeda,
                        IncluirIntegralmenteContratoFreteTerceiro = false
                    };

                    repCargaCTeComponentesFrete.Inserir(cargaCTeComponenteFrete);

                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe> cargaPedidosXMLNotaFiscaisCTeComplementado = null;

                    if (cargaCteComplementarInfo.CargaCTeComplementado != null)
                        cargaPedidosXMLNotaFiscaisCTeComplementado = repCargaPedidoXMLNotaFiscalCTe.BuscarCargaPedidoXMLNotaFiscalCTePorCargaCTe(cargaCteComplementarInfo.CargaCTeComplementado.Codigo);
                    else
                        cargaPedidosXMLNotaFiscaisCTeComplementado = repCargaPedidoXMLNotaFiscalCTe.BuscarCargaPedidoXMLNotaFiscalCTePorCTe(cteComplementado.Codigo);

                    if ((cargaCteComplementarInfo.CargaOcorrencia?.Codigo > 0) && (cargaCteComplementarInfo.CargaCTeComplementado?.Codigo > 0) && (cteComplementado?.IndicadorGlobalizado == Dominio.Enumeradores.OpcaoSimNao.Sim))
                    {
                        List<int> codigosNotaFiscalCTeComplementarGlobalizado = repCargaOcorrenciaCTeComplementarGlobalizadoNotaFiscal.BuscarXMLNotaFiscalPorCargaOcorrenciaECargaCTe(cargaCteComplementarInfo.CargaOcorrencia.Codigo, cargaCteComplementarInfo.CargaCTeComplementado.Codigo);

                        if (codigosNotaFiscalCTeComplementarGlobalizado.Count > 0)
                            cargaPedidosXMLNotaFiscaisCTeComplementado = cargaPedidosXMLNotaFiscaisCTeComplementado.Where(o => codigosNotaFiscalCTeComplementarGlobalizado.Contains(o.PedidoXMLNotaFiscal.XMLNotaFiscal.Codigo)).ToList();
                    }

                    Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete repPedidoXMLNotaFiscalComponenteFrete = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete(unitOfWork);

                    if (cargaPedidosXMLNotaFiscaisCTeComplementado != null && cargaPedidosXMLNotaFiscaisCTeComplementado.Count > 0)
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe ultimaCargaPedidoXMLNotaFiscalCTeComplementado = cargaPedidosXMLNotaFiscaisCTeComplementado.Last();

                        decimal totalComponente = 0m, valorTotalMoedaRateado = 0m, valorTotalCte = 0m;
                        decimal pesoTotal = (from obj in cargaPedidosXMLNotaFiscaisCTeComplementado select obj.PedidoXMLNotaFiscal.XMLNotaFiscal.Peso).Sum();
                        decimal pesoTotalLiquido = (from obj in cargaPedidosXMLNotaFiscaisCTeComplementado select obj.PedidoXMLNotaFiscal.XMLNotaFiscal.Peso).Sum();
                        decimal metrosCubicosTotal = (from obj in cargaPedidosXMLNotaFiscaisCTeComplementado select obj.PedidoXMLNotaFiscal.XMLNotaFiscal.MetrosCubicos).Sum();
                        decimal valorTotal = (from obj in cargaPedidosXMLNotaFiscaisCTeComplementado select obj.PedidoXMLNotaFiscal.XMLNotaFiscal.Valor).Sum();
                        int volumesTotais = (from obj in cargaPedidosXMLNotaFiscaisCTeComplementado select obj.PedidoXMLNotaFiscal.XMLNotaFiscal.Volumes).Sum();

                        decimal pesoTotalParaCalculoFatorCubagem = 0;
                        if (rateioFormula?.ParametroRateioFormula == ParametroRateioFormula.PorPesoUtilizandoFatorCubagem)
                            pesoTotalParaCalculoFatorCubagem = svcRateio.ObterPesoTotalCubadoFatorCubagem((from obj in cargaPedidosXMLNotaFiscaisCTeComplementado select obj.PedidoXMLNotaFiscal).ToList());

                        foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe cargaPedidoXMLNotaFiscalCTeComplementado in cargaPedidosXMLNotaFiscaisCTeComplementado)
                        {
                            decimal valorComponenteRateado = 0m, valorMoedaRateado = 0m;
                            decimal valorCTeRateado = 0m;
                            decimal pesoParaCalculoFatorCubagem = 0m;

                            if (rateioFormula?.ParametroRateioFormula == ParametroRateioFormula.PorPesoUtilizandoFatorCubagem)
                                pesoParaCalculoFatorCubagem = svcRateio.ObterPesoCubadoFatorCubagem(rateioFormula?.ParametroRateioFormula, cargaPedidoXMLNotaFiscalCTeComplementado.PedidoXMLNotaFiscal.CargaPedido.TipoUsoFatorCubagemRateioFormula, cargaPedidoXMLNotaFiscalCTeComplementado.PedidoXMLNotaFiscal.CargaPedido.FatorCubagemRateioFormula ?? 0, cargaPedidoXMLNotaFiscalCTeComplementado.PedidoXMLNotaFiscal.XMLNotaFiscal.Volumes, cargaPedidoXMLNotaFiscalCTeComplementado.PedidoXMLNotaFiscal.XMLNotaFiscal.Peso, cargaPedidoXMLNotaFiscalCTeComplementado.PedidoXMLNotaFiscal.XMLNotaFiscal.MetrosCubicos);

                            decimal valorRateioOriginal = 0;

                            if (cargaCteComplementarInfo.Moeda.HasValue && cargaCteComplementarInfo.Moeda != MoedaCotacaoBancoCentral.Real)
                            {
                                valorMoedaRateado = svcRateio.AplicarFormulaRateio(rateioFormula, (cargaCteComplementarInfo.ValorTotalMoeda ?? 0m), cargaPedidosXMLNotaFiscaisCTeComplementado.Count, 0, pesoTotal, cargaPedidoXMLNotaFiscalCTeComplementado.PedidoXMLNotaFiscal.XMLNotaFiscal.Peso, cargaPedidoXMLNotaFiscalCTeComplementado.PedidoXMLNotaFiscal.XMLNotaFiscal.Valor, valorTotal, 0, TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, cargaPedidoXMLNotaFiscalCTeComplementado.PedidoXMLNotaFiscal.XMLNotaFiscal.MetrosCubicos, metrosCubicosTotal, 0, false, cargaPedidoXMLNotaFiscalCTeComplementado.PedidoXMLNotaFiscal.XMLNotaFiscal.PesoLiquido, pesoTotalLiquido, cargaPedidoXMLNotaFiscalCTeComplementado.PedidoXMLNotaFiscal.XMLNotaFiscal.Volumes, volumesTotais, null, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem);

                                valorTotalMoedaRateado += valorMoedaRateado;

                                if (cargaPedidoXMLNotaFiscalCTeComplementado.Codigo == ultimaCargaPedidoXMLNotaFiscalCTeComplementado.Codigo)
                                    valorMoedaRateado += (cargaCteComplementarInfo.ValorTotalMoeda ?? 0m) - valorTotalMoedaRateado;

                                valorComponenteRateado = Math.Round(valorMoedaRateado * (cargaCteComplementarInfo.ValorCotacaoMoeda ?? 0m), 2, MidpointRounding.AwayFromZero);
                            }
                            else
                            {
                                valorComponenteRateado = svcRateio.AplicarFormulaRateio(rateioFormula, cargaCteComplementarInfo.ValorComplemento, cargaPedidosXMLNotaFiscaisCTeComplementado.Count, 0, pesoTotal, cargaPedidoXMLNotaFiscalCTeComplementado.PedidoXMLNotaFiscal.XMLNotaFiscal.Peso, cargaPedidoXMLNotaFiscalCTeComplementado.PedidoXMLNotaFiscal.XMLNotaFiscal.Valor, valorTotal, 0, TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, cargaPedidoXMLNotaFiscalCTeComplementado.PedidoXMLNotaFiscal.XMLNotaFiscal.MetrosCubicos, metrosCubicosTotal, 0, false, cargaPedidoXMLNotaFiscalCTeComplementado.PedidoXMLNotaFiscal.XMLNotaFiscal.PesoLiquido, pesoTotalLiquido, cargaPedidoXMLNotaFiscalCTeComplementado.PedidoXMLNotaFiscal.XMLNotaFiscal.Volumes, volumesTotais, null, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem);
                                valorCTeRateado = svcRateio.AplicarFormulaRateio(rateioFormula, (cargaCteComplementarInfo.ValorTotalMoeda ?? 0m), cargaPedidosXMLNotaFiscaisCTeComplementado.Count, 0, pesoTotal, cargaPedidoXMLNotaFiscalCTeComplementado.PedidoXMLNotaFiscal.XMLNotaFiscal.Peso, cargaPedidoXMLNotaFiscalCTeComplementado.PedidoXMLNotaFiscal.XMLNotaFiscal.Valor, valorTotal, 0, TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, cargaPedidoXMLNotaFiscalCTeComplementado.PedidoXMLNotaFiscal.XMLNotaFiscal.MetrosCubicos, metrosCubicosTotal, 0, false, cargaPedidoXMLNotaFiscalCTeComplementado.PedidoXMLNotaFiscal.XMLNotaFiscal.PesoLiquido, pesoTotalLiquido, cargaPedidoXMLNotaFiscalCTeComplementado.PedidoXMLNotaFiscal.XMLNotaFiscal.Volumes, volumesTotais, null, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem);

                                totalComponente += valorComponenteRateado;

                                if (cargaPedidoXMLNotaFiscalCTeComplementado.Codigo == ultimaCargaPedidoXMLNotaFiscalCTeComplementado.Codigo)
                                    valorComponenteRateado += cargaCteComplementarInfo.ValorComplemento - totalComponente;
                            }

                            valorCTeRateado = svcRateio.AplicarFormulaRateio(rateioFormula, cte.ValorAReceber, cargaPedidosXMLNotaFiscaisCTeComplementado.Count, 0, pesoTotal, cargaPedidoXMLNotaFiscalCTeComplementado.PedidoXMLNotaFiscal.XMLNotaFiscal.Peso, cargaPedidoXMLNotaFiscalCTeComplementado.PedidoXMLNotaFiscal.XMLNotaFiscal.Valor, valorTotal, 0, TipoCampoValorTabelaFrete.ValorFixo, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, cargaPedidoXMLNotaFiscalCTeComplementado.PedidoXMLNotaFiscal.XMLNotaFiscal.MetrosCubicos, metrosCubicosTotal, 0, false, cargaPedidoXMLNotaFiscalCTeComplementado.PedidoXMLNotaFiscal.XMLNotaFiscal.PesoLiquido, pesoTotalLiquido, cargaPedidoXMLNotaFiscalCTeComplementado.PedidoXMLNotaFiscal.XMLNotaFiscal.Volumes, volumesTotais, null, false, pesoParaCalculoFatorCubagem, pesoTotalParaCalculoFatorCubagem);
                            valorTotalCte += valorCTeRateado;

                            if (cargaPedidoXMLNotaFiscalCTeComplementado.Codigo == ultimaCargaPedidoXMLNotaFiscalCTeComplementado.Codigo)
                                valorCTeRateado += cte.ValorAReceber - valorTotalCte;


                            Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe cargaPedidoXMLNotaFiscalCTE = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe
                            {
                                CargaCTe = cargaCTe,
                                PedidoXMLNotaFiscal = cargaPedidoXMLNotaFiscalCTeComplementado.PedidoXMLNotaFiscal,
                                ValorComplemento = valorCTeRateado
                            };

                            repCargaPedidoXMLNotaFiscalCTe.Inserir(cargaPedidoXMLNotaFiscalCTE);

                            Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFreteComplementado = cargaCTe?.Carga?.TabelaFrete;
                            bool destacarComponenteTabelaFreteComplementado = Servicos.Embarcador.Carga.Frete.DestacarComponenteTabelaFrete(carga.TabelaFrete, cargaCteComplementarInfo.ComponenteFrete);

                            Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete pedidoXMLNotaFiscalComponenteFrete = new Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete()
                            {
                                PedidoXMLNotaFiscal = cargaPedidoXMLNotaFiscalCTeComplementado.PedidoXMLNotaFiscal,
                                TipoComponenteFrete = cargaCteComplementarInfo.ComponenteFrete.TipoComponenteFrete,
                                ComponenteFrete = cargaCteComplementarInfo.ComponenteFrete,
                                AcrescentaValorTotalAReceber = cargaCteComplementarInfo.ComponenteFrete?.AcrescentaValorTotalAReceber ?? false,
                                DescontarValorTotalAReceber = cargaCteComplementarInfo.ComponenteFrete?.DescontarValorTotalAReceber ?? false,
                                NaoSomarValorTotalAReceber = (destacarComponenteTabelaFreteComplementado ? tabelaFreteComplementado.NaoSomarValorTotalAReceber : cargaCteComplementarInfo.ComponenteFrete?.NaoSomarValorTotalAReceber) ?? false,
                                DescontarDoValorAReceberValorComponente = (destacarComponenteTabelaFreteComplementado ? tabelaFreteComplementado.DescontarDoValorAReceberValorComponente : cargaCteComplementarInfo.ComponenteFrete?.DescontarValorTotalAReceber) ?? false,
                                DescontarDoValorAReceberOICMSDoComponente = tabelaFreteComplementado?.DescontarDoValorAReceberOICMSDoComponente ?? false,
                                ValorICMSComponenteDestacado = tabelaFreteComplementado?.ValorICMSComponenteDestacado ?? 0,
                                NaoSomarValorTotalPrestacao = (destacarComponenteTabelaFreteComplementado ? tabelaFreteComplementado.NaoSomarValorTotalPrestacao : cargaCteComplementarInfo.ComponenteFrete?.NaoSomarValorTotalPrestacao) ?? false,
                                IncluirBaseCalculoICMS = cargaCteComplementarInfo.IncluirICMSFrete,
                                TipoValor = TipoCampoValorTabelaFrete.ValorFixo,
                                ValorComponente = valorComponenteRateado,
                                Moeda = cargaCteComplementarInfo.Moeda,
                                ValorCotacaoMoeda = cargaCteComplementarInfo.ValorCotacaoMoeda,
                                ValorTotalMoeda = valorMoedaRateado,
                                IncluirIntegralmenteContratoFreteTerceiro = false
                            };

                            repPedidoXMLNotaFiscalComponenteFrete.Inserir(pedidoXMLNotaFiscalComponenteFrete);
                        }
                    }
                }

                return true;
            }
            else
            {
                Servicos.Embarcador.Carga.NFS svcNFS = new NFS(unitOfWork);

                if (cargaCteComplementarInfo.CargaOcorrencia.OrigemOcorrencia == OrigemOcorrencia.PorPeriodo)
                    svcNFS.CriarCargaCTePendenteEmissaoManualDeNFSManual(cargaCteComplementarInfo.CargaOcorrencia.Cargas.ToList(), cteComplementado.LocalidadeInicioPrestacao, cargaCteComplementarInfo, cargaCteComplementarInfo.CargaOcorrencia, unitOfWork);
                else
                    svcNFS.CriarCargaCTePendenteEmissaoManualDeNFSManual(cargaCteComplementarInfo.CargaCTeComplementado?.Carga, cargaCteComplementarInfo.CargaCTeComplementado, cteComplementado.LocalidadeInicioPrestacao, cargaCteComplementarInfo, cargaCteComplementarInfo.CargaOcorrencia, unitOfWork);
                return true;
            }
        }

        private void EmitirComplementoDocumentoParaEmissaoNFSManual(Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCteComplementarInfo, ref string mensagem, UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual repCargaDocumentoParaEmissaoNFSManual = new Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);

            // Duplicar o documento complementado com o novo valor e setar a ocorrência e documento pai
            Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual complemento = cargaCteComplementarInfo.CargaDocumentoParaEmissaoNFSManualComplementado.Clonar();
            complemento.CargaOcorrencia = cargaCteComplementarInfo.CargaOcorrencia;
            complemento.CargaDocumentoParaEmissaoNFSManualOcorrenciaOrigem = cargaCteComplementarInfo.CargaDocumentoParaEmissaoNFSManualComplementado;
            complemento.ValorFrete = cargaCteComplementarInfo.ValorComplemento;
            complemento.PercentualAliquotaISS = cargaCteComplementarInfo.PercentualAliquotaISS;
            complemento.BaseCalculoISS = cargaCteComplementarInfo.BaseCalculoISS;
            complemento.ValorISS = cargaCteComplementarInfo.ValorISS;
            complemento.ValorPrestacaoServico = cargaCteComplementarInfo.ValorComplemento;
            complemento.ValorRetencaoISS = cargaCteComplementarInfo.ValorRetencaoISS;
            complemento.ValorTotalMoeda = cargaCteComplementarInfo.ValorTotalMoeda;
            complemento.CargaCTeComplementoInfo = cargaCteComplementarInfo;
            complemento.CTe = null;
            complemento.LancamentoNFSManual = null;
            repCargaDocumentoParaEmissaoNFSManual.Inserir(complemento);

            cargaCteComplementarInfo.CargaDocumentoParaEmissaoNFSManualGerado = complemento;
            repCargaCTeComplementoInfo.Atualizar(cargaCteComplementarInfo);
        }

        private bool EmitirDocumentoComplementarPorContrato(Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCteComplementarInfo, ref List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> DocumentosParaEmissao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, ref string mensagem, Repositorio.UnitOfWork unitOfWork, List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfoContaContabilContabilizacao> cargasCTeComplementoInfoContaContabilContabilizacao, Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraICMS)
        {
            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);

            Servicos.Embarcador.Carga.CTe serCte = new Servicos.Embarcador.Carga.CTe(unitOfWork);
            Servicos.Embarcador.Carga.ISS serCargaISS = new Servicos.Embarcador.Carga.ISS(unitOfWork);
            Servicos.Embarcador.Carga.CTeComplementar servicoCTeComplementar = new Servicos.Embarcador.Carga.CTeComplementar(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();

            if (cargaCteComplementarInfo.CTe != null)
                return false;

            Dominio.Entidades.Empresa empresa = cargaCteComplementarInfo.CargaOcorrencia.Emitente;
            if (empresa == null)
                throw new Exception("Nenhuma empresa para emissão.");

            Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal = null;
            Dominio.ObjetosDeValor.CTe.CTe nfse = new Dominio.ObjetosDeValor.CTe.CTe();

            bool complementoICMS = false;
            if (cargaCteComplementarInfo.ComponenteFrete != null && cargaCteComplementarInfo.ComponenteFrete.TipoComponenteFrete == TipoComponenteFrete.ICMS)
                complementoICMS = true;

            if (cargaCteComplementarInfo.CargaOcorrencia.ModeloDocumentoFiscal != null)
                modeloDocumentoFiscal = cargaCteComplementarInfo.CargaOcorrencia.ModeloDocumentoFiscal;

            Dominio.Entidades.EmpresaSerie empresaSerieModelo = null;
            if (modeloDocumentoFiscal != null)
            {  //Busca a série configurada para o modelo do documento fiscal
                modeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorId(modeloDocumentoFiscal.Codigo);
                if (modeloDocumentoFiscal.Series.Count > 0)
                    empresaSerieModelo = (from obj in modeloDocumentoFiscal.Series where obj.Empresa.Codigo == empresa.Codigo select obj).FirstOrDefault();
            }

            if (empresaSerieModelo != null)
                nfse.Serie = empresaSerieModelo.Numero;
            else if (empresa != null && empresa.Configuracao != null && empresa.Configuracao.SerieCTeComplementar != null)
                nfse.Serie = empresa.Configuracao.SerieCTeComplementar.Numero;

            if (!complementoICMS)
            {
                nfse.ValorAReceber += cargaCteComplementarInfo.ValorComplemento;
                nfse.ValorFrete += cargaCteComplementarInfo.ValorComplemento;
                nfse.ValorTotalPrestacaoServico += cargaCteComplementarInfo.ValorComplemento;
            }

            string observacaoCTe = BuscarObservacaoPadraoOcorrencia(cargaCteComplementarInfo, null, modeloDocumentoFiscal, unitOfWork);

            if (regraICMS == null)
                regraICMS = new Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS();

            Dominio.ObjetosDeValor.Embarcador.ISS.RegraISS regraISS = BuscarRegraISS(cargaCteComplementarInfo, tipoServicoMultisoftware, unitOfWork, out mensagem, configuracaoEmbarcador);
            if (!string.IsNullOrWhiteSpace(mensagem))
                throw new Exception(mensagem);

            Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS impostoIBSCBS = servicoCTeComplementar.ObterRetornoImpostoIBSCBS(cargaCteComplementarInfo);

            Dominio.Entidades.Cliente tomador = cargaCteComplementarInfo.TomadorPagador;
            Dominio.Enumeradores.TipoTomador tipoTomador = cargaCteComplementarInfo.TipoTomador;

            Dominio.Enumeradores.TipoPagamento tipoPagamento = cargaCteComplementarInfo.TipoPagamento;
            Dominio.Enumeradores.TipoCTE tipoCTe = cargaCteComplementarInfo.TipoCTE;
            Dominio.Enumeradores.TipoServico tipoServico = cargaCteComplementarInfo.TipoServico;
            List<string> rotas = new List<string>();
            int tipoEnvio = 0;

            Dominio.Entidades.Cliente destinatario = tomador;
            Dominio.Entidades.Cliente remetente = tomador;
            Dominio.Entidades.Cliente expedidor = null;
            Dominio.Entidades.Cliente recebedor = null;

            Dominio.Entidades.Localidade origem = tomador?.Localidade;
            Dominio.Entidades.Localidade destino = origem;

            List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga> quantidades = new List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga>();
            List<Dominio.ObjetosDeValor.Embarcador.CTe.CTe> ctesAnteriores = null;
            List<Dominio.ObjetosDeValor.Embarcador.CTe.Seguro> seguros = new List<Dominio.ObjetosDeValor.Embarcador.CTe.Seguro>();

            serCte.ObterDescricoesComponentesPadrao(tomador, null, configuracaoEmbarcador, unitOfWork, out string descricaoComponenteValorFrete, out string descricaoComponenteValorICMS);

            List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.ConfiguracaoContabil> configuracoes = Servicos.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil.ConverterCargaCTeComplementoInfoContaContabilContabilizacao(cargaCteComplementarInfo, cargasCTeComplementoInfoContaContabilContabilizacao);
            serCte.GerarCTe(cargaCteComplementarInfo, cargaCteComplementarInfo.CargaOcorrencia.Cargas.ToList(), empresa, nfse, remetente, destinatario, tomador, expedidor, recebedor, origem, destino, null, null, tipoPagamento, tipoTomador, quantidades, new List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico>(), observacaoCTe, null, rotas, seguros, regraICMS, regraISS, impostoIBSCBS, tipoServicoMultisoftware, modeloDocumentoFiscal, tipoServico, tipoCTe, ctesAnteriores, tipoEnvio, true, 0, unitOfWork, null, descricaoComponenteValorFrete, descricaoComponenteValorICMS, configuracaoEmbarcador, configuracaoGeralCarga, cargaCteComplementarInfo.CentroResultado, cargaCteComplementarInfo.CentroResultadoDestinatario, cargaCteComplementarInfo.ItemServico, cargaCteComplementarInfo.CentroResultadoEscrituracao, cargaCteComplementarInfo.CentroResultadoICMS, cargaCteComplementarInfo.CentroResultadoPIS, cargaCteComplementarInfo.CentroResultadoCOFINS, cargaCteComplementarInfo.ValorMaximoCentroContabilizacao, configuracoes);
            repCargaCTeComplementoInfo.Atualizar(cargaCteComplementarInfo);

            if (cargaCteComplementarInfo.CTe != null)
            {
                if (cargaCteComplementarInfo.ComponenteFrete.ImprimirDescricaoComponenteEmComplementos)
                {
                    cargaCteComplementarInfo.CTe.DescricaoComplemento = cargaCteComplementarInfo.ComponenteFrete.ImprimirOutraDescricaoCTe ? cargaCteComplementarInfo.ComponenteFrete.DescricaoCTe : cargaCteComplementarInfo.ComponenteFrete.DescricaoComponente;
                    repCTe.Atualizar(cargaCteComplementarInfo.CTe);
                }

                if (!DocumentosParaEmissao.Contains(cargaCteComplementarInfo.CTe))
                    DocumentosParaEmissao.Add(cargaCteComplementarInfo.CTe);
            }
            else
            {
                cargaCteComplementarInfo.CargaOcorrencia.AgImportacaoCTe = true;
                repCargaOcorrencia.Atualizar(cargaCteComplementarInfo.CargaOcorrencia);
            }

            return true;
        }

        private string BuscarObservacaoPadraoOcorrencia(Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaOcorrenciaComplementoInfo, Dominio.Entidades.ConhecimentoDeTransporteEletronico cteComplementado, Dominio.Entidades.ModeloDocumentoFiscal modeloDocumento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            string observacaoCTe = "";

            if (!string.IsNullOrWhiteSpace(cargaOcorrenciaComplementoInfo.ObservacaoCTe))
            {
                if (!string.IsNullOrWhiteSpace(observacaoCTe))
                    observacaoCTe = string.Concat(observacaoCTe, "; ", cargaOcorrenciaComplementoInfo.ObservacaoCTe);
                else
                    observacaoCTe = string.Concat(cargaOcorrenciaComplementoInfo.ObservacaoCTe);
            }
            else if (!string.IsNullOrWhiteSpace(cargaOcorrenciaComplementoInfo.CargaOcorrencia.ObservacaoCTe))
            {
                if (!string.IsNullOrWhiteSpace(observacaoCTe))
                    observacaoCTe = string.Concat(observacaoCTe, "; ", cargaOcorrenciaComplementoInfo.CargaOcorrencia.ObservacaoCTe);
                else
                    observacaoCTe = string.Concat(cargaOcorrenciaComplementoInfo.CargaOcorrencia.ObservacaoCTe);
            }

            if (cargaOcorrenciaComplementoInfo.CargaOcorrencia.TipoOcorrencia != null)
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia repositorioConfiguracaoOcorrencia = new Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoOcorrencia configuracaoOcorrencia = repositorioConfiguracaoOcorrencia.BuscarPrimeiroRegistro();

                string descricaoOcorrencia = configuracaoOcorrencia.NaoImprimirTipoOcorrenciaNaObservacaoCTeComplementar ? "" : $"; Ocorrência: {cargaOcorrenciaComplementoInfo.CargaOcorrencia.TipoOcorrencia.Descricao}: {cargaOcorrenciaComplementoInfo.CargaOcorrencia.NumeroOcorrencia.ToString()}";

                if (!string.IsNullOrWhiteSpace(observacaoCTe))
                    observacaoCTe = string.Concat(observacaoCTe, descricaoOcorrencia);
                else
                    observacaoCTe = descricaoOcorrencia;
            }

            if (cteComplementado != null && cargaOcorrenciaComplementoInfo.CargaOcorrencia.OrigemOcorrencia == OrigemOcorrencia.PorCarga)
            {
                Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaOcorrenciaComplementoInfo.CargaOcorrencia.Carga;

                if (!string.IsNullOrWhiteSpace(observacaoCTe))
                    observacaoCTe = string.Concat(observacaoCTe, "; CT-e emitido como complemento ao CT-e ", cteComplementado.Chave);
                else
                    observacaoCTe = string.Concat("CT-e emitido como complemento ao CT-e ", cteComplementado.Chave);

                if (!string.IsNullOrWhiteSpace(observacaoCTe))
                    observacaoCTe = string.Concat(observacaoCTe, "; Documento se destina a complementar valores de documento já emitido nº " + cteComplementado.Numero.ToString("D") + " no dia " + (cteComplementado.DataEmissao.HasValue ? cteComplementado.DataEmissao.Value.ToString("dd/MM/yyyy") : ""));
                else
                    observacaoCTe = string.Concat("; Documento se destina a complementar valores de documento já emitido nº " + cteComplementado.Numero.ToString("D") + " no dia " + (cteComplementado.DataEmissao.HasValue ? cteComplementado.DataEmissao.Value.ToString("dd/MM/yyyy") : ""));

                try
                {
                    string numeroNotasComSerie = cteComplementado.NumeroNotasComSerie;
                    if (!string.IsNullOrEmpty(numeroNotasComSerie))
                        observacaoCTe = string.Concat(observacaoCTe, "; Notas: ", numeroNotasComSerie);
                }
                catch (Exception)
                {
                }

                if (!string.IsNullOrWhiteSpace(cteComplementado.ObservacoesGerais) && (cargaOcorrenciaComplementoInfo?.CargaOcorrencia?.TipoOcorrencia?.CopiarObservacoesDoCTeDeOrigemAoGerarCTeComplementar ?? false))
                {
                    if (!string.IsNullOrWhiteSpace(observacaoCTe))
                        observacaoCTe = string.Concat(observacaoCTe, $"; Observações: {cteComplementado.ObservacoesGerais}");
                    else
                        observacaoCTe = string.Concat($"; Observações: {cteComplementado.ObservacoesGerais}");
                }

                if (carga != null && !string.IsNullOrWhiteSpace(carga.CodigoCargaEmbarcador))
                    observacaoCTe = string.Concat(observacaoCTe, "; Carga: ", carga.CodigoCargaEmbarcador);
            }

            if (!string.IsNullOrWhiteSpace(cargaOcorrenciaComplementoInfo.CargaOcorrencia.ContaContabil))
            {
                if (!string.IsNullOrWhiteSpace(observacaoCTe))
                    observacaoCTe = string.Concat(observacaoCTe, "; Conta Contábil: ", cargaOcorrenciaComplementoInfo.CargaOcorrencia.ContaContabil);
                else
                    observacaoCTe = string.Concat("Conta Contábil: ", cargaOcorrenciaComplementoInfo.CargaOcorrencia.ContaContabil);
            }

            if (!string.IsNullOrWhiteSpace(cargaOcorrenciaComplementoInfo.CargaOcorrencia.CFOP))
            {
                if (!string.IsNullOrWhiteSpace(observacaoCTe))
                    observacaoCTe = string.Concat(observacaoCTe, "; CFOP: ", cargaOcorrenciaComplementoInfo.CargaOcorrencia.CFOP);
                else
                    observacaoCTe = string.Concat("CFOP: ", cargaOcorrenciaComplementoInfo.CargaOcorrencia.CFOP);
            }

            if (cteComplementado != null && cteComplementado.Veiculos != null && cteComplementado.Veiculos.Count > 0 && cargaOcorrenciaComplementoInfo.CargaOcorrencia.OrigemOcorrencia == OrigemOcorrencia.PorCarga)
            {
                if (!string.IsNullOrWhiteSpace(cteComplementado.Veiculos.FirstOrDefault()?.Placa))
                {
                    if (!string.IsNullOrWhiteSpace(observacaoCTe))
                        observacaoCTe = string.Concat(observacaoCTe, "; Placa: ", cteComplementado.Veiculos.FirstOrDefault()?.Placa);
                    else
                        observacaoCTe = string.Concat("Placa: ", cteComplementado.Veiculos.FirstOrDefault()?.Placa);

                    Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorPlaca(cteComplementado.Veiculos.FirstOrDefault()?.Placa);
                    if (veiculo != null && veiculo.Proprietario != null)
                    {
                        if (!string.IsNullOrWhiteSpace(observacaoCTe))
                            observacaoCTe = string.Concat(observacaoCTe, "; CNPJ: ", veiculo.Proprietario.CPF_CNPJ_SemFormato, ";");
                        else
                            observacaoCTe = string.Concat("CNPJ: ", veiculo.Proprietario.CPF_CNPJ_SemFormato, ";");
                    }
                    else if (veiculo != null && veiculo.Proprietario == null && veiculo.Empresa != null && cargaOcorrenciaComplementoInfo.CargaOcorrencia.Usuario != null && (cargaOcorrenciaComplementoInfo.CargaOcorrencia.Usuario.TipoAcesso == Dominio.Enumeradores.TipoAcesso.Terceiro || cargaOcorrenciaComplementoInfo.CargaOcorrencia.TipoOcorrencia.OcorrenciaTerceiros))
                    {
                        if (!string.IsNullOrWhiteSpace(observacaoCTe))
                            observacaoCTe = string.Concat(observacaoCTe, "; CNPJ: ", veiculo.Empresa.CNPJ, ";");
                        else
                            observacaoCTe = string.Concat("CNPJ: ", veiculo.Empresa.CNPJ, ";");
                    }
                    else if (cteComplementado.Veiculos.Count > 1)
                    {
                        Dominio.Entidades.Veiculo veiculo2 = repVeiculo.BuscarPorPlaca(cteComplementado.Veiculos[1].Placa);
                        if (veiculo2 != null && veiculo2.Proprietario != null)
                        {
                            if (!string.IsNullOrWhiteSpace(observacaoCTe))
                                observacaoCTe = string.Concat(observacaoCTe, "; CNPJ: ", veiculo2.Proprietario.CPF_CNPJ_SemFormato, ";");
                            else
                                observacaoCTe = string.Concat("CNPJ: ", veiculo2.Proprietario.CPF_CNPJ_SemFormato, ";");
                        }
                        else if (veiculo2 != null && veiculo2.Proprietario == null && veiculo2.Empresa != null && cargaOcorrenciaComplementoInfo.CargaOcorrencia.Usuario != null && (cargaOcorrenciaComplementoInfo.CargaOcorrencia.Usuario.TipoAcesso == Dominio.Enumeradores.TipoAcesso.Terceiro || cargaOcorrenciaComplementoInfo.CargaOcorrencia.TipoOcorrencia.OcorrenciaTerceiros))
                        {
                            if (!string.IsNullOrWhiteSpace(observacaoCTe))
                                observacaoCTe = string.Concat(observacaoCTe, "; CNPJ: ", veiculo2.Empresa.CNPJ, ";");
                            else
                                observacaoCTe = string.Concat("CNPJ: ", veiculo2.Empresa.CNPJ, ";");
                        }
                    }
                }
            }

            if (cargaOcorrenciaComplementoInfo.CargaOcorrencia.OrigemOcorrenciaPorPeriodo)
            {
                // Quinzena
                string quinzena = (cargaOcorrenciaComplementoInfo.CargaOcorrencia.PeriodoInicio.Value.Day > 15 ? "Segunda" : "Primeira");

                // Mes
                System.Globalization.DateTimeFormatInfo dtfi = System.Globalization.CultureInfo.CreateSpecificCulture("pt-BR").DateTimeFormat;
                int mesValor = cargaOcorrenciaComplementoInfo.CargaOcorrencia.PeriodoInicio.Value.Month;
                string mes = dtfi.GetMonthName(mesValor).ToLower();
                mes = char.ToUpper(mes[0]) + mes.Substring(1);

                // Ano
                string ano = cargaOcorrenciaComplementoInfo.CargaOcorrencia.PeriodoInicio.Value.ToString("yyyy");

                // Ex: Primeira Quinzena Janeiro 2018. Ocorrência Ajudantes: 1111.
                string observacaoOcorrenciaPorPeriodo = String.Concat(quinzena, " Quinzena de ", mes, " ", ano);
                if (!string.IsNullOrWhiteSpace(observacaoCTe))
                    observacaoCTe = string.Concat(observacaoCTe, "; " + observacaoOcorrenciaPorPeriodo);
                else
                    observacaoCTe = observacaoOcorrenciaPorPeriodo;

                if (!observacaoCTe.Contains("CFOP: "))
                {
                    if ((modeloDocumento.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS || modeloDocumento.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe) && !string.IsNullOrWhiteSpace(cargaOcorrenciaComplementoInfo.CargaOcorrencia.TipoOcorrencia.CodigoIntegracaoCFOPNFSe))
                    {
                        if (!string.IsNullOrWhiteSpace(observacaoCTe))
                            observacaoCTe = string.Concat(observacaoCTe, "; CFOP: ", cargaOcorrenciaComplementoInfo.CargaOcorrencia.TipoOcorrencia.CodigoIntegracaoCFOPNFSe);
                        else
                            observacaoCTe = string.Concat("CFOP: ", cargaOcorrenciaComplementoInfo.CargaOcorrencia.TipoOcorrencia.CodigoIntegracaoCFOPNFSe);
                    }
                    else if (modeloDocumento.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe && !string.IsNullOrWhiteSpace(cargaOcorrenciaComplementoInfo.CargaOcorrencia.TipoOcorrencia.CodigoIntegracaoCFOPEstadual))
                    {
                        if (!string.IsNullOrWhiteSpace(observacaoCTe))
                            observacaoCTe = string.Concat(observacaoCTe, "; CFOP: ", cargaOcorrenciaComplementoInfo.CargaOcorrencia.TipoOcorrencia.CodigoIntegracaoCFOPEstadual);
                        else
                            observacaoCTe = string.Concat("CFOP: ", cargaOcorrenciaComplementoInfo.CargaOcorrencia.TipoOcorrencia.CodigoIntegracaoCFOPEstadual);
                    }
                }

                if (cargaOcorrenciaComplementoInfo.CargaOcorrencia.Veiculo != null)
                {
                    if (!string.IsNullOrWhiteSpace(observacaoCTe))
                        observacaoCTe = string.Concat(observacaoCTe, "; Placa: ", cargaOcorrenciaComplementoInfo.CargaOcorrencia.Veiculo.Placa);
                    else
                        observacaoCTe = string.Concat("Placa: ", cargaOcorrenciaComplementoInfo.CargaOcorrencia.Veiculo.Placa);

                    Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorPlaca(cargaOcorrenciaComplementoInfo.CargaOcorrencia.Veiculo.Placa);
                    if (veiculo != null && veiculo.Proprietario != null)
                    {
                        if (!string.IsNullOrWhiteSpace(observacaoCTe))
                            observacaoCTe = string.Concat(observacaoCTe, "; CNPJ: ", veiculo.Proprietario.CPF_CNPJ_SemFormato, ";");
                        else
                            observacaoCTe = string.Concat("CNPJ: ", veiculo.Proprietario.CPF_CNPJ_SemFormato, ";");
                    }
                }
            }

            if (cargaOcorrenciaComplementoInfo.Moeda.HasValue && cargaOcorrenciaComplementoInfo.Moeda != MoedaCotacaoBancoCentral.Real)
            {
                string obsMoeda = $"Valor em {cargaOcorrenciaComplementoInfo.Moeda.Value.ObterDescricao()}: {(cargaOcorrenciaComplementoInfo.ValorTotalMoeda ?? 0m):n2}. Cotação: {(cargaOcorrenciaComplementoInfo.ValorCotacaoMoeda ?? 0m):n10}.";

                if (!string.IsNullOrWhiteSpace(observacaoCTe))
                    observacaoCTe += ". " + obsMoeda;
                else
                    observacaoCTe = obsMoeda;
            }

            return observacaoCTe;
        }

        private Dominio.ObjetosDeValor.Embarcador.ISS.RegraISS BuscarRegraISS(Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTeComplementoInfo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, out string mensagem, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfoContaContabilContabilizacao repCargaCTeComplementoInfoContaContabilContabilizacao = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfoContaContabilContabilizacao(unitOfWork);
            Servicos.Embarcador.Carga.RateioCTeComplementar serFreteRateio = new RateioCTeComplementar(unitOfWork);
            mensagem = "";

            if (cargaCTeComplementoInfo.TomadorPagador == null || tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                repCargaCTeComplementoInfoContaContabilContabilizacao.DeletarPorComplementoInfo(cargaCTeComplementoInfo.Codigo);
                mensagem = serFreteRateio.CalcularImpostosComplementoInfo(ref cargaCTeComplementoInfo, tipoServicoMultisoftware, unitOfWork, configuracao);
                repCargaCTeComplementoInfo.Atualizar(cargaCTeComplementoInfo);
            }

            Dominio.ObjetosDeValor.Embarcador.ISS.RegraISS regraISS = new Dominio.ObjetosDeValor.Embarcador.ISS.RegraISS
            {
                AliquotaISS = cargaCTeComplementoInfo.PercentualAliquotaISS,
                IncluirISSBaseCalculo = cargaCTeComplementoInfo.IncluirISSBaseCalculo,
                PercentualRetencaoISS = cargaCTeComplementoInfo.PercentualRetencaoISS,
                ValorBaseCalculoISS = cargaCTeComplementoInfo.BaseCalculoISS,
                ValorISS = cargaCTeComplementoInfo.ValorISS,
                ValorRetencaoISS = cargaCTeComplementoInfo.ValorRetencaoISS,

                ReterIR = cargaCTeComplementoInfo.ReterIR,
                AliquotaIR = cargaCTeComplementoInfo.AliquotaIR,
                BaseCalculoIR = cargaCTeComplementoInfo.BaseCalculoIR,
                ValorIR = cargaCTeComplementoInfo.ValorIR
            };

            return regraISS;
        }

        private Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS BuscarRegraICMS(Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTeComplementoInfo, Dominio.Entidades.Empresa empresa, Dominio.Entidades.Localidade origem, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, out string mensagem, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, bool complementoICMS)
        {
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfoContaContabilContabilizacao repCargaCTeComplementoInfoContaContabilContabilizacao = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfoContaContabilContabilizacao(unitOfWork);
            Servicos.Embarcador.Carga.RateioCTeComplementar serFreteRateio = new RateioCTeComplementar(unitOfWork);
            mensagem = "";

            // if temporario, caso algum ct-e esteja em etapa de emissão e sem imposto, depois de um tempo pode remover e deixar apenas usando oque foi calculado anteriormente.
            if (cargaCTeComplementoInfo.TomadorPagador == null)
            {
                repCargaCTeComplementoInfoContaContabilContabilizacao.DeletarPorComplementoInfo(cargaCTeComplementoInfo.Codigo);
                mensagem = serFreteRateio.CalcularImpostosComplementoInfo(ref cargaCTeComplementoInfo, tipoServicoMultisoftware, unitOfWork, configuracao);
                repCargaCTeComplementoInfo.Atualizar(cargaCTeComplementoInfo);
            }

            bool descontarICMSValorAReceber = false;
            bool naoReduzirRetencaoICMSDoValorDaPrestacao = false;
            bool zerarValorICMS = false;
            if (!complementoICMS)
            {
                //Verifica seo CTe anterior possui descontado o valor do ICMS
                descontarICMSValorAReceber = cargaCTeComplementoInfo.CargaCTeComplementado != null ? cargaCTeComplementoInfo.CargaCTeComplementado.CTe.ValorICMS > 0 && cargaCTeComplementoInfo.CargaCTeComplementado.CTe.ValorAReceber + cargaCTeComplementoInfo.CargaCTeComplementado.CTe.ValorICMS == cargaCTeComplementoInfo.CargaCTeComplementado.CTe.ValorPrestacaoServico : cargaCTeComplementoInfo.CTe != null ? cargaCTeComplementoInfo.CTe.ValorICMS > 0 && (cargaCTeComplementoInfo.CTe.ValorAReceber + cargaCTeComplementoInfo.CTe.ValorICMS == cargaCTeComplementoInfo.CTe.ValorPrestacaoServico) : false;

                naoReduzirRetencaoICMSDoValorDaPrestacao = cargaCTeComplementoInfo.CargaCTeComplementado != null ?
                    descontarICMSValorAReceber && cargaCTeComplementoInfo.CargaCTeComplementado.CTe.ValorICMS > 0 &&
                    cargaCTeComplementoInfo.CargaCTeComplementado.CTe.BaseCalculoICMS != cargaCTeComplementoInfo.CargaCTeComplementado.CTe.ValorPrestacaoServico &&
                    (cargaCTeComplementoInfo.CargaCTeComplementado.CTe.ValorAReceber + cargaCTeComplementoInfo.CargaCTeComplementado.CTe.ValorICMS == cargaCTeComplementoInfo.CargaCTeComplementado.CTe.ValorPrestacaoServico) &&
                    (cargaCTeComplementoInfo.CargaCTeComplementado.CTe.ValorAReceber < cargaCTeComplementoInfo.CargaCTeComplementado.CTe.ValorPrestacaoServico) && cargaCTeComplementoInfo.CargaCTeComplementado.CTe.CST == "60"
                    :
                    cargaCTeComplementoInfo.CTe != null ? descontarICMSValorAReceber &&
                    cargaCTeComplementoInfo.CTe.BaseCalculoICMS != cargaCTeComplementoInfo.CTe.ValorPrestacaoServico &&
                    cargaCTeComplementoInfo.CTe.ValorICMS > 0 && (cargaCTeComplementoInfo.CTe.ValorAReceber + cargaCTeComplementoInfo.CTe.ValorICMS == cargaCTeComplementoInfo.CTe.ValorPrestacaoServico) &&
                    (cargaCTeComplementoInfo.CTe.ValorAReceber < cargaCTeComplementoInfo.CTe.ValorPrestacaoServico) && cargaCTeComplementoInfo.CTe.CST == "60"
                    :
                    false;

                //#6252 CTe anterior possuia regra de ICMS que zerava o valor do ICMS, criado validação para quando CTe anterior esta com base de calculo de ICMS zerada zera também do CTe complementar
                if (cargaCTeComplementoInfo?.CargaOcorrencia?.ComponenteFrete?.TipoComponenteFrete != TipoComponenteFrete.ICMS)
                    zerarValorICMS = cargaCTeComplementoInfo.CargaCTeComplementado != null && !(cargaCTeComplementoInfo.CargaOcorrencia?.TipoOcorrencia?.BuscarCSTQuandoDocumentoOrigemIsento ?? false) && cargaCTeComplementoInfo.CargaCTeComplementado.CTe.BaseCalculoICMS == 0;

                if (!zerarValorICMS && configuracao.UtilizarPedagioBaseCalculoIcmsCteComplementarPorRegraEstado)
                {
                    Servicos.Embarcador.Carga.ICMS servicoIcms = new Servicos.Embarcador.Carga.ICMS();
                    zerarValorICMS = !servicoIcms.IncluirComponenteFreteBaseCalculoIcms(cargaCTeComplementoInfo.ComponenteFrete.TipoComponenteFrete, empresa, origem?.Estado.Sigla ?? "", unitOfWork);
                }
            }


            Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraICMS = new Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS
            {
                Aliquota = cargaCTeComplementoInfo.PercentualAliquota,
                AliquotaPis = cargaCTeComplementoInfo.AliquotaPis,
                AliquotaCofins = cargaCTeComplementoInfo.AliquotaCofins,
                ValorPis = cargaCTeComplementoInfo.ValorPis,
                ValorCofins = cargaCTeComplementoInfo.ValorCofins,
                CST = cargaCTeComplementoInfo.CST,
                PercentualReducaoBC = cargaCTeComplementoInfo.PercentualReducaoBC,
                ObservacaoCTe = "",
                PercentualInclusaoBC = cargaCTeComplementoInfo.PercentualIncluirBaseCalculo,
                IncluirICMSBC = cargaCTeComplementoInfo.IncluirICMSFrete,
                ValorBaseCalculoICMS = zerarValorICMS ? 0 : cargaCTeComplementoInfo.BaseCalculoICMS,
                ValorBaseCalculoPISCOFINS = zerarValorICMS ? 0 : cargaCTeComplementoInfo.BaseCalculoICMS,
                PercentualCreditoPresumido = cargaCTeComplementoInfo.PercentualCreditoPresumido,
                ValorCreditoPresumido = cargaCTeComplementoInfo.ValorCreditoPresumido,
                ValorICMS = zerarValorICMS ? 0 : cargaCTeComplementoInfo.ValorICMS,
                CFOP = cargaCTeComplementoInfo.CFOP?.CodigoCFOP ?? 0,
                DescontarICMSDoValorAReceber = descontarICMSValorAReceber,
                CodigoRegra = cargaCTeComplementoInfo.RegraICMS?.Codigo ?? 0,
                NaoReduzirRetencaoICMSDoValorDaPrestacao = cargaCTeComplementoInfo.RegraICMS?.NaoReduzirRetencaoICMSDoValorDaPrestacao ?? naoReduzirRetencaoICMSDoValorDaPrestacao
            };

            return regraICMS;
        }

        private Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS ObterRetornoImpostoIBSCBS(Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCteComplementarInfo)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS()
            {
                CodigoOutraAliquota = cargaCteComplementarInfo.OutrasAliquotas?.Codigo ?? 0,
                CST = cargaCteComplementarInfo.CSTIBSCBS,
                ClassificacaoTributaria = cargaCteComplementarInfo.ClassificacaoTributariaIBSCBS,
                BaseCalculo = cargaCteComplementarInfo.BaseCalculoIBSCBS,

                AliquotaIBSEstadual = cargaCteComplementarInfo.AliquotaIBSEstadual,
                PercentualReducaoIBSEstadual = cargaCteComplementarInfo.PercentualReducaoIBSEstadual,
                ValorIBSEstadual = cargaCteComplementarInfo.ValorIBSEstadual,

                AliquotaIBSMunicipal = cargaCteComplementarInfo.AliquotaIBSMunicipal,
                PercentualReducaoIBSMunicipal = cargaCteComplementarInfo.PercentualReducaoIBSMunicipal,
                ValorIBSMunicipal = cargaCteComplementarInfo.ValorIBSMunicipal,

                AliquotaCBS = cargaCteComplementarInfo.AliquotaCBS,
                PercentualReducaoCBS = cargaCteComplementarInfo.PercentualReducaoCBS,
                ValorCBS = cargaCteComplementarInfo.ValorCBS
            };
        }

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ObterConfiguracaoEmbarcador(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            if (_configuracaoEmbarcador != null)
                return _configuracaoEmbarcador;

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
            return _configuracaoEmbarcador = configuracaoEmbarcador;
        }

        private Dominio.Entidades.ModeloDocumentoFiscal ObterModeloDocumentoFiscalPorCarga(Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCteComplementarInfo, Dominio.Entidades.ConhecimentoDeTransporteEletronico cteComplementado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            CargaPedido serCargaPedido = new CargaPedido(unitOfWork);

            Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);

            Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal = null;
            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoTransbordo = cargaCteComplementarInfo.CargaOcorrencia.Carga.Pedidos.FirstOrDefault();
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador(unitOfWork);

            bool emitirCTeNormal = cargaCteComplementarInfo.TipoCTE == Dominio.Enumeradores.TipoCTE.Normal;
            bool cargaTransbordo = cargaCteComplementarInfo.CargaOcorrencia.Carga.CargaTransbordo;
            bool gerarNFSeParaComplementosTomadorIgualDestinatario = cargaCteComplementarInfo.CargaOcorrencia?.TipoOcorrencia?.GerarNFSeParaComplementosTomadorIgualDestinatario ?? false;
            bool tomadorIgualDestinatario = cargaCteComplementarInfo.CargaCTeComplementado?.CTe?.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario;

            if (emitirCTeNormal && cargaTransbordo && tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {

                serCargaPedido.VerificarQuaisDocumentosDeveEmitir(
                    cargaCteComplementarInfo.CargaOcorrencia.Carga,
                    cargaPedidoTransbordo,
                    cargaPedidoTransbordo.Origem,
                    cteComplementado.LocalidadeTerminoPrestacao,
                    tipoServicoMultisoftware,
                    unitOfWork,
                    out bool possuiCTe, out bool possuiNFS, out bool possuiNFSManual, out Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoIntramunicipal,
                    configuracaoEmbarcador,
                    out bool sempreDisponibilizarDocumentoNFSManual
                );

                if (modeloDocumentoIntramunicipal != null)
                    modeloDocumentoFiscal = modeloDocumentoIntramunicipal;
                else if (possuiCTe)
                    modeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorTipoDocumento(Dominio.Enumeradores.TipoDocumento.CTe);
                else if (possuiNFS)
                    modeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorTipoDocumento(Dominio.Enumeradores.TipoDocumento.NFSe);
                else if (possuiNFSManual)
                    modeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorTipoDocumento(Dominio.Enumeradores.TipoDocumento.NFS);
            }
            else if (gerarNFSeParaComplementosTomadorIgualDestinatario && tomadorIgualDestinatario)
                modeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorTipoDocumento(Dominio.Enumeradores.TipoDocumento.NFSe);
            else
                modeloDocumentoFiscal = cteComplementado.ModeloDocumentoFiscal;


            return modeloDocumentoFiscal;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga> ObterQuantidades(Dominio.Entidades.ConhecimentoDeTransporteEletronico cteComplementado)
        {
            List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga> quantidades = new List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga>();

            foreach (Dominio.Entidades.InformacaoCargaCTE quantidadeAnterior in cteComplementado.QuantidadesCarga)
            {
                Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga quantidadeCarga = new Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga();
                quantidadeCarga.Medida = quantidadeAnterior.DescricaoUnidadeMedida;
                quantidadeCarga.Quantidade = quantidadeAnterior.Quantidade;
                Dominio.Enumeradores.UnidadeMedida un;
                Enum.TryParse<Dominio.Enumeradores.UnidadeMedida>(quantidadeAnterior.UnidadeMedida, out un);
                quantidadeCarga.Unidade = un;

                quantidades.Add(quantidadeCarga);
            }
            return quantidades;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.CTe.CTe> ObterCTesAnteriores(Dominio.Entidades.ConhecimentoDeTransporteEletronico cteComplementado)
        {
            Servicos.WebService.Empresa.Empresa serEmpresa = new Servicos.WebService.Empresa.Empresa(_unitOfWork);
            List<Dominio.ObjetosDeValor.Embarcador.CTe.CTe> ctesAnteriores = new List<Dominio.ObjetosDeValor.Embarcador.CTe.CTe>();

            if (cteComplementado.DocumentosTransporteAnterior != null)
            {
                foreach (Dominio.Entidades.DocumentoDeTransporteAnteriorCTe documentoTransportaAnterior in cteComplementado.DocumentosTransporteAnterior)
                {
                    int.TryParse(documentoTransportaAnterior.Numero, out int numero);
                    Dominio.ObjetosDeValor.Embarcador.CTe.CTe objCteAnterior = new Dominio.ObjetosDeValor.Embarcador.CTe.CTe
                    {
                        Emitente = serEmpresa.ConverterObjetoEmpresa(documentoTransportaAnterior.Emissor),
                        Chave = documentoTransportaAnterior.Chave,
                        Numero = numero
                    };
                    ctesAnteriores.Add(objCteAnterior);
                }
            }
            return ctesAnteriores;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.CTe.Seguro> ObterSeguros(Dominio.Entidades.ConhecimentoDeTransporteEletronico cteComplementado)
        {
            List<Dominio.ObjetosDeValor.Embarcador.CTe.Seguro> seguros = new List<Dominio.ObjetosDeValor.Embarcador.CTe.Seguro>();
            foreach (Dominio.Entidades.SeguroCTE seguroCTe in cteComplementado.Seguros)
            {
                Dominio.ObjetosDeValor.Embarcador.CTe.Seguro seguro = new Dominio.ObjetosDeValor.Embarcador.CTe.Seguro()
                {
                    Apolice = seguroCTe.NumeroApolice,
                    Averbacao = seguroCTe.NumeroAverbacao,
                    ResponsavelSeguro = seguroCTe.Tipo,
                    Seguradora = seguroCTe.NomeSeguradora,
                    Valor = seguroCTe.Valor
                };
                seguros.Add(seguro);
            }
            return seguros;
        }

        private Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco ObterPedidoEndereco(Dominio.Entidades.ParticipanteCTe participanteCTe)
        {
            if (participanteCTe == null || participanteCTe.Exterior)
                return null;

            return new Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco()
            {
                Bairro = participanteCTe.Bairro,
                CEP = participanteCTe.CEP,
                Complemento = participanteCTe.Complemento,
                Endereco = participanteCTe.Endereco,
                IE_RG = participanteCTe.IE_RG,
                Localidade = participanteCTe.Localidade,
                Numero = participanteCTe.Numero,
                Telefone = participanteCTe.Telefone1
            };
        }

        private bool verificaPermissaoEmissaoAposPrazo(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe, Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoOcorrencia, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, int diasPrazo, bool permitirAbrirOcorrenciaAposPrazoSolicitacao = false)
        {
            if (cargaCTe.CTe.DataEmissao.HasValue &&
                (configuracao.PrazoSolicitacaoOcorrencia == 0 || cargaCTe.CTe.DataEmissao.Value.AddDays(diasPrazo) >= DateTime.Now || permitirAbrirOcorrenciaAposPrazoSolicitacao))
                return true;

            return false;
        }

        private bool verificaPermissaoEmissaoAposPrazo(Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual cargaDocumentoParaEmissaoNFSManual, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, int diasPrazo, bool permitirAbrirOcorrenciaAposPrazoSolicitacao = false)
        {

            if (cargaDocumentoParaEmissaoNFSManual.DataEmissao != DateTime.MinValue &&
                (configuracao.PrazoSolicitacaoOcorrencia == 0 || cargaDocumentoParaEmissaoNFSManual.DataEmissao.AddDays(diasPrazo) >= DateTime.Now || permitirAbrirOcorrenciaAposPrazoSolicitacao))
                return true;

            return false;
        }

        private int ObterDiasPrazoSolicitacaoOcorrencia(Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoOcorrencia, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            int diasPrazo = 0;
            if (tipoOcorrencia.PrazoSolicitacaoOcorrencia > 0)
                diasPrazo = tipoOcorrencia.PrazoSolicitacaoOcorrencia;
            else if ((carga?.TipoOperacao?.PrazoSolicitacaoOcorrencia ?? 0) > 0)
                diasPrazo = carga.TipoOperacao.PrazoSolicitacaoOcorrencia;
            else if (configuracao.PrazoSolicitacaoOcorrencia > 0)
                diasPrazo = configuracao.PrazoSolicitacaoOcorrencia;

            return diasPrazo;
        }

        private Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS obterRegraICMS(Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoOcorrencia configuracaoOcorrencia, Dominio.Entidades.TipoDeOcorrenciaDeCTe TipoOcorrencia, bool FilialEmissora, bool Subcontratada)
        {
            Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS RegraICMS = null;
            if (configuracaoOcorrencia.PermitirDefinirCSTnoTipoDeOcorrencia)
            {
                if (FilialEmissora &&
                    (TipoOcorrencia.TipoEmissaoDocumentoOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoDocumentoOcorrencia.Todos ||
                    TipoOcorrencia.TipoEmissaoDocumentoOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoDocumentoOcorrencia.SomenteFilialEmissora)
                    && (TipoOcorrencia?.CSTFilialEmissora ?? "") != "")
                {
                    RegraICMS = new Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS();
                    RegraICMS.CST = TipoOcorrencia.CSTFilialEmissora;
                }

                if (Subcontratada &&
                    (TipoOcorrencia.TipoEmissaoDocumentoOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoDocumentoOcorrencia.Todos ||
                    TipoOcorrencia.TipoEmissaoDocumentoOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoDocumentoOcorrencia.SomenteSubcontratada)
                    && (TipoOcorrencia?.CSTSubContratada ?? "") != "")
                {
                    RegraICMS = new Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS();
                    RegraICMS.CST = TipoOcorrencia.CSTSubContratada;
                }
            }
            return RegraICMS;
        }

        #endregion
    }
}
