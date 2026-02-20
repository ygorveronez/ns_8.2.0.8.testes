using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Repositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Ocorrencia
{
    public class Ocorrencia : ServicoBase
    {
        #region Construtores

        public Ocorrencia() : base() { }

        #endregion

        #region Métodos Públicos

        public static void ProcessarImportarOcorrencia(Repositorio.UnitOfWork unitOfWork, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
        {
            Repositorio.Embarcador.Ocorrencias.ImportarOcorrencia repImportarOcorrencia = new Repositorio.Embarcador.Ocorrencias.ImportarOcorrencia(unitOfWork);

            List<Dominio.Entidades.Embarcador.Ocorrencias.ImportarOcorrencia> integracaoesPendente = repImportarOcorrencia.BuscarPorSituacao(SituacaoImportarOcorrencia.AgIntegracao);
            if (integracaoesPendente != null && integracaoesPendente.Count > 0)
            {
                foreach (var inter in integracaoesPendente)
                {
                    Dominio.Entidades.Embarcador.Ocorrencias.ImportarOcorrencia integracao = repImportarOcorrencia.BuscarPorCodigo(inter.Codigo);
                    ProcessarIntegracao(unitOfWork, stringConexao, integracao, tipoServicoMultisoftware, clienteMultisoftware);
                }

            }
        }

        public static async Task<string> ValidarDadosOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, Dominio.Entidades.Embarcador.Ocorrencias.ImportarOcorrencia importarOcorrencia, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTEs, string stringConexao, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.CargaOcorrencia.Ocorrencia srvOcorrencia = new Servicos.Embarcador.CargaOcorrencia.Ocorrencia(unitOfWork);

            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Transportadores.TransportadorConfiguracaoNFSe repTransportadorConfiguracaoNFSe = new Repositorio.Embarcador.Transportadores.TransportadorConfiguracaoNFSe(unitOfWork);
            Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = await repConfiguracaoTMS.BuscarConfiguracaoPadraoAsync();

            if (ocorrencia.TipoOcorrencia.ClientesBloqueados != null && ocorrencia.TipoOcorrencia.ClientesBloqueados.Count > 0 && cargaCTEs.Count > 0)
            {
                foreach (Dominio.Entidades.Embarcador.Ocorrencias.ClientesBloqueados clienteBloqueado in ocorrencia.TipoOcorrencia.ClientesBloqueados)
                {
                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in cargaCTEs)
                    {
                        if (cargaCTe.CTe != null)
                        {
                            if (clienteBloqueado.TipoCliente == Dominio.Enumeradores.TipoTomador.Remetente)
                            {
                                if (clienteBloqueado.Cliente.CPF_CNPJ_SemFormato == cargaCTe.CTe.Remetente.CPF_CNPJ_SemFormato)
                                {
                                    string clienteCodigoIntegracao = string.IsNullOrWhiteSpace(clienteBloqueado.Cliente.CodigoIntegracao) ? string.Empty : " (" + clienteBloqueado.Cliente.CodigoIntegracao + ")";
                                    return ($"Cliente origem {clienteBloqueado.Cliente.CPF_CNPJ_SemFormato}{clienteCodigoIntegracao} {clienteBloqueado.Cliente.Nome} não permitido para lançamento da ocorrência.");

                                }
                            }
                            else if (clienteBloqueado.TipoCliente == Dominio.Enumeradores.TipoTomador.Destinatario)
                            {
                                if (clienteBloqueado.Cliente.CPF_CNPJ_SemFormato == cargaCTe.CTe.Destinatario.CPF_CNPJ_SemFormato)
                                {
                                    string clienteCodigoIntegracao = string.IsNullOrWhiteSpace(clienteBloqueado.Cliente.CodigoIntegracao) ? string.Empty : " (" + clienteBloqueado.Cliente.CodigoIntegracao + ")";
                                    return ($"Cliente destino {clienteBloqueado.Cliente.CPF_CNPJ_SemFormato}{clienteCodigoIntegracao} {clienteBloqueado.Cliente.Nome} não permitido para lançamento da ocorrência.");

                                }
                            }
                        }
                    }
                }
            }

            if (ocorrencia.OrigemOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemOcorrencia.PorCarga)
            {
                if (!ocorrencia.ComplementoValorFreteCarga)
                {
                    if (cargaCTEs.Count == 0)
                    {
                        return ("É obrigatório selecionar ao menos um CT-e para gerar a ocorrência.");

                    }

                    if (ocorrencia.TipoOcorrencia.NaoPermiteSelecionarTodosCTes)
                    {
                        List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> todosCtesCarga = await srvOcorrencia.BuscarCTesSelecionadosOuCargas(new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaMontarConsultaCtes(), ocorrencia.Carga, configuracao, unitOfWork, string.Empty, true, 0, 0, tipoServicoMultisoftware, null, 0);
                        if (cargaCTEs.Count == todosCtesCarga.Count && todosCtesCarga.Count > 1)
                        {
                            return ($"Não é permitido selecionar todos CTes, favor selecionar apenas o CTe que teve a ocorrência de {ocorrencia.TipoOcorrencia.Descricao}");

                        }
                    }
                }
                else
                {
                    if (ocorrencia.ValorOcorrencia <= 0m && tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    {
                        return ("É obrigatório informar um valor para a ocorrência.");

                    }

                    if (ocorrencia.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova &&
                        ocorrencia.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe &&
                        ocorrencia.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete)
                    {
                        return ("A situação da carga não permite que a ocorrência de complemento do valor do frete seja adicionada.");

                    }
                }
            }

            if (ocorrencia.OrigemOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemOcorrencia.PorPeriodo)
            {
                if (!ocorrencia.TipoOcorrencia.OcorrenciaDestinadaFranquias)
                {
                    if (ocorrencia.Cargas.Count == 0)
                    {
                        return ("O período selecionado não possui nenhuma carga com documento.");

                    }

                    if (ocorrencia.Cargas.FirstOrDefault().Empresa.EmissaoDocumentosForaDoSistema)
                    {
                        return ("A emissão por periodo só é permitida para transportadores que emitem no Multiembarcador.");

                    }
                }
                else
                {
                    if (ocorrencia.ContratoFrete == null)
                    {
                        return ("É obrigatorio informar o contrato de frete para gerar a ocorrência.");

                    }
                }
            }

            if (ocorrencia.ComponenteFrete != null && ocorrencia.ComponenteFrete.TipoComponenteFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS)
            {
                if (ocorrencia.ValorICMS <= 0m && ocorrencia.AliquotaICMS <= 0m)
                {
                    return ("É necessário informar um valor ou alíquota de ICMS para a emissão de complemento de ICMS.");

                }

                if (ocorrencia.OrigemOcorrenciaPorPeriodo)
                {
                    return ("A emissão de complementos de ICMS não é permitida para ocorrências por período.");

                }

                if (cargaCTEs.Count > 1 && ocorrencia.ValorICMS > 0m)
                {
                    return ("Selecione apenas um CT-e para emissão de complemento de ICMS com valor de ICMS. Caso necessário selecionar mais de um, utilize somente a alíquota.");

                }
            }

            if (ocorrencia.TipoOcorrencia.BloqueiaOcorrenciaDuplicada)
            {
                if (ocorrencia.OrigemOcorrenciaPorPeriodo)
                {
                    // Validacao de ocorrencia por periodo
                    if (importarOcorrencia != null && !srvOcorrencia.ValidaSeExisteOcorrenciaPorPeriodo(ocorrencia, out string erro, unitOfWork, importarOcorrencia.Usuario))
                    {
                        return (erro);

                    }
                }
                else
                {
                    // Validacao de ocorrencia por CTe
                    if (!srvOcorrencia.ValidaSeExisteOcorrenciaPorCTe(cargaCTEs, ocorrencia, out string erro, unitOfWork, tipoServicoMultisoftware))
                    {
                        return (erro);

                    }
                }
            }

            if (ocorrencia.TipoOcorrencia.BloquearOcorrenciaDuplicadaCargaMesmoMDFe)
            {
                if (!srvOcorrencia.ValidaOcorrenciaDuplicadaCargaMDFe(ocorrencia.Carga, ocorrencia, out string erro, unitOfWork))
                {
                    return (erro);

                }
            }

            if (!srvOcorrencia.SetaModeloDocumentoFiscal(ref ocorrencia, cargaCTEs, out string erroModeloDocumento, unitOfWork, tipoServicoMultisoftware))
            {
                return (erroModeloDocumento);

            }

            if (ocorrencia.OrigemOcorrenciaPorPeriodo)
            {
                // Valida emitente
                if (ocorrencia.TipoOcorrencia.TipoEmissaoIntramunicipal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoIntramunicipal.SempreNFSe)
                {
                    if (ocorrencia.Emitente == null)
                    {
                        return ("Emitente não selecionado.");

                    }

                    Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorEmpresa(ocorrencia.Emitente.Codigo);
                    if (transportadorConfiguracaoNFSe == null)
                    {
                        return ("Emitente não possui configuração para emitir NFSe.");

                    }
                }
            }

            Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal = ocorrencia.TipoOcorrencia.ModeloDocumentoFiscal;
            Dominio.Entidades.Empresa empresaOcorrencia = ocorrencia.ObterEmitenteOcorrencia();
            Dominio.Entidades.EmpresaSerie empresaSerieModelo = null;
            if (modeloDocumentoFiscal?.Abreviacao == "ND" && configuracao.NumeroSerieNotaDebitoPadrao > 0)
            {
                if (empresaOcorrencia == null)
                {
                    return ("Transportador não selecionado.");

                }
                if (modeloDocumentoFiscal.Series.Count > 0)
                    empresaSerieModelo = (from obj in modeloDocumentoFiscal.Series where obj.Empresa.Codigo == empresaOcorrencia.Codigo select obj).FirstOrDefault();
                if (empresaSerieModelo == null)
                {
                    return ("Transportador sem série cadastrada para o documento ND.");

                }
            }
            else if (modeloDocumentoFiscal?.Abreviacao == "NC" && configuracao.NumeroSerieNotaCreditoPadrao > 0)
            {
                if (empresaOcorrencia == null)
                {
                    return ("Transportador não selecionado.");

                }
                if (modeloDocumentoFiscal.Series.Count > 0)
                    empresaSerieModelo = (from obj in modeloDocumentoFiscal.Series where obj.Empresa.Codigo == empresaOcorrencia.Codigo select obj).FirstOrDefault();
                if (empresaSerieModelo == null)
                {
                    return ("Transportador sem série cadastrada para o documento NC.");

                }
            }

            return string.Empty;
        }

        #endregion

        #region Métodos Privados

        private static void ProcessarIntegracao(Repositorio.UnitOfWork unitOfWork, string stringConexao, Dominio.Entidades.Embarcador.Ocorrencias.ImportarOcorrencia importarOcorrencia, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
        {
            try
            {
                Repositorio.Embarcador.Ocorrencias.ImportarOcorrencia repImportarOcorrencia = new Repositorio.Embarcador.Ocorrencias.ImportarOcorrencia(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
                Servicos.Embarcador.CargaOcorrencia.Ocorrencia srvOcorrencia = new Servicos.Embarcador.CargaOcorrencia.Ocorrencia(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                unitOfWork.Start();

                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia = new Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia();

                PreencherEntidade(ref ocorrencia, importarOcorrencia, unitOfWork);

                if (ocorrencia.TipoOcorrencia == null)
                    importarOcorrencia.RetornoImportacao = "É obrigatório selecionar o tipo de ocorrência. ";

                if (ocorrencia.ValorOcorrencia > 10000000m)
                    importarOcorrencia.RetornoImportacao = ("O valor da ocorrência não pode ser maior que R$ 10.000.000,00. ");

                if (ocorrencia.TipoOcorrencia.ExigirInformarObservacao && string.IsNullOrWhiteSpace(ocorrencia.Observacao))
                    importarOcorrencia.RetornoImportacao = ("É obrigatório informar uma observação. ");

                if (ocorrencia.TipoOcorrencia.OcorrenciaExclusivaParaIntegracao)
                    importarOcorrencia.RetornoImportacao = ("Tipo de Ocorrência é excluisiva para integração. ");

                if (ocorrencia.OrigemOcorrenciaPorPeriodo && ((!ocorrencia.PeriodoInicio.HasValue || !ocorrencia.PeriodoFim.HasValue) || ocorrencia.PeriodoInicio.Value > ocorrencia.PeriodoFim.Value))
                    importarOcorrencia.RetornoImportacao = "Período selecionado é inválido. ";

                if (string.IsNullOrWhiteSpace(importarOcorrencia.RetornoImportacao))
                {
                    ocorrencia.OrigemOcorrencia = ocorrencia.TipoOcorrencia.OrigemOcorrencia;

                    List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTEs = repCargaCTe.BuscarPorCTeCarga(importarOcorrencia.CTe?.Codigo ?? 0, importarOcorrencia.Carga?.Codigo ?? 0);

                    if (cargaCTEs == null || cargaCTEs.Count == 0)
                        importarOcorrencia.RetornoImportacao = "Nenhum CT-e localizado. ";

                    if (cargaCTEs.Any(obj => obj.CargaCTeFilialEmissora != null))
                        ocorrencia.EmiteComplementoFilialEmissora = true;

                    ocorrencia.IncluirICMSFrete = cargaCTEs.Count > 0 ? (cargaCTEs.FirstOrDefault().CTe.IncluirICMSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim ? true : false) : false;

                    string codigoCFOPIntegracao = string.Empty;
                    if (ocorrencia != null && ocorrencia.TipoOcorrencia != null && cargaCTEs != null && cargaCTEs.Count > 0)
                    {
                        Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = (from o in cargaCTEs where o.CTe != null && o.CargaCTeComplementoInfo == null select o.CTe).FirstOrDefault();

                        if (cte != null && cte.LocalidadeInicioPrestacao.Estado.Sigla == cte.LocalidadeTerminoPrestacao.Estado.Sigla)
                        {
                            if (cte.ValorICMS == 0 && !string.IsNullOrWhiteSpace(ocorrencia.TipoOcorrencia.CodigoIntegracaoCFOPEstadualIsento))
                                codigoCFOPIntegracao = ocorrencia.TipoOcorrencia.CodigoIntegracaoCFOPEstadualIsento;
                            else
                                codigoCFOPIntegracao = !string.IsNullOrWhiteSpace(ocorrencia.TipoOcorrencia.CodigoIntegracaoCFOPEstadual) ? ocorrencia.TipoOcorrencia.CodigoIntegracaoCFOPEstadual : string.Empty;
                        }
                        else if (cte != null)
                        {
                            if (cte.ValorICMS == 0 && !string.IsNullOrWhiteSpace(ocorrencia.TipoOcorrencia.CodigoIntegracaoCFOPInterestadualIsento))
                                codigoCFOPIntegracao = ocorrencia.TipoOcorrencia.CodigoIntegracaoCFOPInterestadualIsento;
                            else
                                codigoCFOPIntegracao = !string.IsNullOrWhiteSpace(ocorrencia.TipoOcorrencia.CodigoIntegracaoCFOPInterestadual) ? ocorrencia.TipoOcorrencia.CodigoIntegracaoCFOPInterestadual : !string.IsNullOrWhiteSpace(ocorrencia.TipoOcorrencia.CodigoIntegracaoCFOPEstadual) ? ocorrencia.TipoOcorrencia.CodigoIntegracaoCFOPEstadual : string.Empty;
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(codigoCFOPIntegracao))
                        ocorrencia.CFOP = codigoCFOPIntegracao;

                    if (ocorrencia.TipoOcorrencia.DataOcorrenciaIgualDataCTeComplementado)
                        ocorrencia.DataOcorrencia = cargaCTEs.FirstOrDefault()?.CTe.DataEmissao ?? ocorrencia.DataOcorrencia;

                    srvOcorrencia.SetaEmitenteOcorrencia(ref ocorrencia, unitOfWork, tipoServicoMultisoftware, importarOcorrencia.Usuario);

                    string msgRetorno = ValidarDadosOcorrencia(ocorrencia, importarOcorrencia, tipoServicoMultisoftware, cargaCTEs, stringConexao, unitOfWork).Result;

                    if (!string.IsNullOrWhiteSpace(msgRetorno))
                        importarOcorrencia.RetornoImportacao += msgRetorno + " ";

                    if (string.IsNullOrWhiteSpace(importarOcorrencia.RetornoImportacao))
                    {
                        repOcorrencia.Inserir(ocorrencia);

                        Servicos.Embarcador.Integracao.IntegracaoOcorrencia.AdicionarIntegracoesOcorrencia(ocorrencia, cargaCTEs, unitOfWork);

                        if (ocorrencia.TipoOcorrencia.OcorrenciaPorQuantidade)
                            ocorrencia.ValorOcorrencia = ocorrencia.Quantidade * ocorrencia.TipoOcorrencia.Valor;

                        string mensagemRetorno = string.Empty;
                        if (!srvOcorrencia.FluxoGeralOcorrencia(ref ocorrencia, cargaCTEs, null, ref mensagemRetorno, unitOfWork, tipoServicoMultisoftware, importarOcorrencia.Usuario, configuracao, clienteMultisoftware, "", true))
                            importarOcorrencia.RetornoImportacao += mensagemRetorno;

                        repOcorrencia.Atualizar(ocorrencia);
                    }
                }

                if (!string.IsNullOrWhiteSpace(importarOcorrencia.RetornoImportacao))
                    importarOcorrencia.SituacaoImportarOcorrencia = SituacaoImportarOcorrencia.Falha;
                else
                {
                    importarOcorrencia.SituacaoImportarOcorrencia = SituacaoImportarOcorrencia.Finalizada;
                    importarOcorrencia.CargaOcorrencia = ocorrencia;
                }

                repImportarOcorrencia.Atualizar(importarOcorrencia);

                unitOfWork.CommitChanges();
                unitOfWork.FlushAndClear();
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                unitOfWork.FlushAndClear();
                Servicos.Log.TratarErro(ex, "ProcessarImportarOcorrencia");
            }
        }

        private static void PreencherEntidade(ref Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, Dominio.Entidades.Embarcador.Ocorrencias.ImportarOcorrencia importarOcorrencia, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
            Repositorio.TipoDeOcorrenciaDeCTe repTipoOcorrencia = new TipoDeOcorrenciaDeCTe(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new ConhecimentoDeTransporteEletronico(unitOfWork);

            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(importarOcorrencia.CTe?.Codigo ?? 0);

            ocorrencia.CTeTerceiro = null;
            ocorrencia.Veiculo = null;
            ocorrencia.Carga = importarOcorrencia.Carga;
            ocorrencia.Responsavel = null;
            ocorrencia.Quantidade = 0;
            ocorrencia.ComponenteFrete = importarOcorrencia.ComponenteFrete;
            ocorrencia.TipoOcorrencia = repTipoOcorrencia.BuscarPorCodigo(importarOcorrencia.TipoOcorrencia?.Codigo ?? 0);
            ocorrencia.PercentualAcresciomoValor = ocorrencia.TipoOcorrencia.PercentualAcrescimo;
            ocorrencia.Tomador = null;
            ocorrencia.DataOcorrencia = importarOcorrencia.DataOcorrencia.Value;
            ocorrencia.PeriodoInicio = importarOcorrencia.DataOcorrencia;
            ocorrencia.PeriodoFim = importarOcorrencia.DataOcorrencia;
            ocorrencia.Observacao = importarOcorrencia.Observacao;
            ocorrencia.NumeroOcorrenciaCliente = "";
            ocorrencia.NumeroOcorrencia = Servicos.Embarcador.CargaOcorrencia.OcorrenciaSequencial.GetInstance().ObterProximoNumeroSequencial(unitOfWork);
            ocorrencia.DataAlteracao = DateTime.Now;
            ocorrencia.DataFinalizacaoEmissaoOcorrencia = DateTime.Now;
            ocorrencia.ObservacaoCTe = importarOcorrencia.ObservacaoImpressa;
            ocorrencia.Usuario = importarOcorrencia.Usuario;
            ocorrencia.SituacaoOcorrencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Finalizada;
            ocorrencia.ValorICMS = 0m;
            ocorrencia.BaseCalculoICMS = 0m;
            ocorrencia.AliquotaICMS = 0m;
            ocorrencia.DTNatura = null;
            ocorrencia.ComplementoValorFreteCarga = ocorrencia.TipoOcorrencia?.OcorrenciaComplementoValorFreteCarga ?? false;
            ocorrencia.NomeRecebedor = "";
            ocorrencia.TipoDocumentoRecebedor = "";
            ocorrencia.NumeroDocumentoRecebedor = "";
            ocorrencia.NotificarDebitosAtivos = false;
            ocorrencia.CTeEmitidoNoEmbarcador = false;
            ocorrencia.ContratoFrete = null;
            ocorrencia.DataEvento = null;
            ocorrencia.Filial = null;
            ocorrencia.NaoGerarDocumento = ocorrencia.TipoOcorrencia?.NaoGerarDocumento ?? false;
            ocorrencia.UsuarioResponsavelAprovacao = importarOcorrencia.Usuario;
            if (!string.IsNullOrWhiteSpace(importarOcorrencia.CST))
                ocorrencia.CSTICMS = importarOcorrencia.CST;

            if (ocorrencia.TipoOcorrencia.TomadorTipoOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TomadorTipoOcorrencia.Outros && ocorrencia.Tomador == null && ocorrencia.TipoOcorrencia.OutroTomador != null)
            {
                ocorrencia.Responsavel = Dominio.Enumeradores.TipoTomador.Outros;
                ocorrencia.Tomador = ocorrencia.TipoOcorrencia.OutroTomador;
            }

            ocorrencia.ModeloDocumentoFiscal = cte?.ModeloDocumentoFiscal ?? null;
            ocorrencia.ValorOcorrencia = importarOcorrencia.ValorOcorrencia;
            ocorrencia.AliquotaICMS = importarOcorrencia.AliquotaICMS;
            ocorrencia.ValorOcorrenciaOriginal = importarOcorrencia.ValorOcorrencia;
            if (ocorrencia.AliquotaICMS > 0)
            {
                ocorrencia.BaseCalculoICMS = Math.Round((importarOcorrencia.ValorOcorrencia * 100) / ocorrencia.AliquotaICMS, 2, MidpointRounding.ToEven);
                ocorrencia.ValorICMS = importarOcorrencia.ValorOcorrencia;
                //if (ocorrencia.ComponenteFrete != null && ocorrencia.ComponenteFrete.TipoComponenteFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS)
                //{
                //    ocorrencia.ValorOcorrencia = 0;
                //    ocorrencia.ValorOcorrenciaOriginal = 0;
                //}
            }

            if (ocorrencia.ComponenteFrete != null && ocorrencia.ComponenteFrete.SomarComponenteFreteLiquido)
                ocorrencia.ValorOcorrenciaLiquida = ocorrencia.ValorOcorrencia;
        }

        #endregion
    }
}
