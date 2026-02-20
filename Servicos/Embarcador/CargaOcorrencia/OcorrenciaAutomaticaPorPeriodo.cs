using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servicos.Embarcador.CargaOcorrencia
{
    public sealed class OcorrenciaAutomaticaPorPeriodo
    {
        #region Atributos

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador;
        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public OcorrenciaAutomaticaPorPeriodo(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, configuracaoEmbarcador: null)
        {
        }

        public OcorrenciaAutomaticaPorPeriodo(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador)
        {
            _configuracaoEmbarcador = configuracaoEmbarcador;
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Privados

        //private async Task AdicionarOcorrenciaAsync(Dominio.Entidades.Embarcador.Ocorrencias.GatilhoGeracaoAutomaticaOcorrencia gatilhoGeracaoAutomatica, Dominio.Entidades.Embarcador.Cargas.Carga carga, DateTime dataInicio, DateTime dataFim, TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, double codigoFronteiraParqueamento, string stringConexao)
        //{
        //    Repositorio.UnitOfWork unitOfWorkAsync = new(stringConexao);
        //    AdicionarOcorrencia(gatilhoGeracaoAutomatica, carga, dataInicio, dataFim, tipoServicoMultisoftware, clienteMultisoftware, codigoFronteiraParqueamento, unitOfWorkAsync);
        //}

        private void AdicionarOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.GatilhoGeracaoAutomaticaOcorrencia gatilhoGeracaoAutomatica, Dominio.Entidades.Embarcador.Cargas.Carga carga, DateTime dataInicio, DateTime dataFim, TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, double codigoFronteiraParqueamento)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> listaCargaCTe = repCargaCTe.BuscarPorCarga(carga.Codigo, false, false, false, false, false, 0, 0, true, false, 0);

            if (listaCargaCTe.Count == 0)
                throw new ServicoException("É obrigatório ao menos um CT-e para gerar a ocorrência");

            Log.TratarErro($"gatilhoGeracaoAutomatica: {gatilhoGeracaoAutomatica.TipoOcorrencia.Descricao}; carga: {carga.Descricao}; dataInicio: {dataInicio}; dataFim: {dataFim}", "GATILHO");

            bool transacaoAtiva = _unitOfWork.IsActiveTransaction();

            try
            {
                if (!transacaoAtiva)
                    _unitOfWork.Start();

                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repositorioOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(_unitOfWork);
                if (gatilhoGeracaoAutomatica.NaoPermiteDuplicarOcorrencia && repositorioOcorrencia.ExisteOcorrenciaParaTipoOcorrenciaECarga(gatilhoGeracaoAutomatica.TipoOcorrencia.Codigo, carga.Codigo))
                {
                    Log.TratarErro($"Já existe uma ocorrência do tipo {gatilhoGeracaoAutomatica.TipoOcorrencia.Descricao} para a carga {carga.CodigoCargaEmbarcador}. Não será gerada nova ocorrência.", "GATILHO");

                    if (!transacaoAtiva)
                        _unitOfWork.Dispose();

                    throw new ServicoException($"Já existe uma ocorrência do tipo {gatilhoGeracaoAutomatica.TipoOcorrencia.Descricao} para a carga {carga.CodigoCargaEmbarcador}. Não será gerada nova ocorrência.");
                }

                Dominio.ObjetosDeValor.Embarcador.Ocorrencia.CalculoFreteOcorrencia calculoFreteOcorrencia = ObterCalculoFreteOcorrencia(carga, codigoFronteiraParqueamento, listaCargaCTe, gatilhoGeracaoAutomatica, dataInicio, dataFim, tipoServicoMultisoftware);

                if (calculoFreteOcorrencia == null)
                {
                    if (!transacaoAtiva)
                        _unitOfWork.Dispose();

                    return;
                }

                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia = new Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia
                {
                    Carga = carga,
                    CFOP = ObterCfopOcorrencia(gatilhoGeracaoAutomatica, listaCargaCTe),
                    ComplementoValorFreteCarga = gatilhoGeracaoAutomatica.TipoOcorrencia.OcorrenciaComplementoValorFreteCarga,
                    ComponenteFrete = gatilhoGeracaoAutomatica.TipoOcorrencia.ComponenteFrete,
                    DataAlteracao = DateTime.Now,
                    DataOcorrencia = DateTime.Now,
                    DataFinalizacaoEmissaoOcorrencia = DateTime.Now,
                    Emitente = null, // Transportador
                    EmiteComplementoFilialEmissora = listaCargaCTe.Any(o => o.CargaCTeFilialEmissora != null),
                    IncluirICMSFrete = calculoFreteOcorrencia.IncluirICMSFrete,
                    NumeroOcorrencia = Servicos.Embarcador.CargaOcorrencia.OcorrenciaSequencial.GetInstance().ObterProximoNumeroSequencial(_unitOfWork),
                    Observacao = gatilhoGeracaoAutomatica.ObterObservacaoOcorrencia(),
                    ObservacaoCTe = calculoFreteOcorrencia.ObservacaoCTe ?? "",
                    OrigemOcorrencia = gatilhoGeracaoAutomatica.TipoOcorrencia.OrigemOcorrencia,
                    PercentualAcresciomoValor = gatilhoGeracaoAutomatica.TipoOcorrencia.PercentualAcrescimo,
                    SituacaoOcorrencia = SituacaoOcorrencia.Finalizada,
                    TipoOcorrencia = gatilhoGeracaoAutomatica.TipoOcorrencia,
                    ValorOcorrencia = calculoFreteOcorrencia.ValorOcorrencia,
                    ValorOcorrenciaOriginal = calculoFreteOcorrencia.ValorOcorrencia,
                    GeradaPorGatilho = true,
                    ModeloDocumentoFiscal = gatilhoGeracaoAutomatica.TipoOcorrencia.ModeloDocumentoFiscal
                };

                Log.TratarErro($"gatilhoGeracaoAutomatica NOVA OCORRENCIA: {cargaOcorrencia.ValorOcorrencia}; carga: {carga.Descricao}; dataInicio: {dataInicio}; dataFim: {dataFim}", "GATILHO");

                //COMO A GERACAO É AUTOMATICA E CASO EXIGE MOTIVO NA OCORRENCIA, A SITUACAO DEVERA FICAR COMO AGInformacoes PARA O USUARIO COMPLETAR O MOTIVO POSTERIORMENTE;
                if (_configuracaoEmbarcador.ExigirMotivoOcorrencia)
                    cargaOcorrencia.SituacaoOcorrencia = SituacaoOcorrencia.AgInformacoes;

                if (cargaOcorrencia.ComponenteFrete?.SomarComponenteFreteLiquido ?? false)
                    cargaOcorrencia.ValorOcorrenciaLiquida = cargaOcorrencia.ValorOcorrencia;

                if (calculoFreteOcorrencia.PercentualAcrescimoValor > 0m)
                    cargaOcorrencia.PercentualAcresciomoValor = calculoFreteOcorrencia.PercentualAcrescimoValor;

                VerificarClienteBloqueadoParaGeracaoOcorrencia(gatilhoGeracaoAutomatica.TipoOcorrencia, listaCargaCTe);
                VerificarComplementoValorFreteCarga(cargaOcorrencia);
                DefinirModeloDocumentoFiscal(cargaOcorrencia, listaCargaCTe, tipoServicoMultisoftware);

                repositorioOcorrencia.Inserir(cargaOcorrencia);

                AdicionarOcorrenciaIntegracoes(cargaOcorrencia, listaCargaCTe);
                AdicionarOcorrenciaParametros(cargaOcorrencia, gatilhoGeracaoAutomatica, calculoFreteOcorrencia, dataInicio, dataFim);
                DefinirFluxoGeralOcorrencia(cargaOcorrencia, listaCargaCTe, clienteMultisoftware, tipoServicoMultisoftware);
                AdicionarOcorrenciaDocumentos(cargaOcorrencia, listaCargaCTe);

                repositorioOcorrencia.Atualizar(cargaOcorrencia);

                if (!transacaoAtiva)
                    _unitOfWork.CommitChanges();
            }
            catch (Exception ex)
            {
                Log.TratarErro($"ERRO adicionar Ocorrencia: {ex.Message}; carga: {carga.Descricao};", "GATILHO");

                if (!transacaoAtiva)
                    _unitOfWork.Rollback();

                throw;
            }
        }

        private void AdicionarOcorrenciaDocumentos(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia, List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> listaCargaCTe)
        {
            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento repositorioCargaOcorrenciaDocumento = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento> cargaOcorrenciaDocumentos = repositorioCargaOcorrenciaDocumento.BuscarPorOcorrencia(cargaOcorrencia.Codigo);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTE in listaCargaCTe)
            {
                if (cargaOcorrenciaDocumentos.Any(o => o.CargaCTe.Codigo == cargaCTE.Codigo))
                    continue;

                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento cargaOcorrenciaDocumento = new Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento
                {
                    CargaCTe = cargaCTE,
                    CargaOcorrencia = cargaOcorrencia,
                };

                repositorioCargaOcorrenciaDocumento.Inserir(cargaOcorrenciaDocumento);
            }
        }

        private void AdicionarOcorrenciaIntegracoes(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia, List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> listaCargaCTe)
        {
            Integracao.IntegracaoOcorrencia.AdicionarIntegracoesOcorrencia(cargaOcorrencia, listaCargaCTe, _unitOfWork);
        }

        private void AdicionarOcorrenciaParametros(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia, Dominio.Entidades.Embarcador.Ocorrencias.GatilhoGeracaoAutomaticaOcorrencia gatilhoGeracaoAutomatica, Dominio.ObjetosDeValor.Embarcador.Ocorrencia.CalculoFreteOcorrencia calculoFreteOcorrencia, DateTime dataInicio, DateTime dataFim)
        {
            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaParametros repositorioCargaOcorrenciaParametros = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaParametros(_unitOfWork);

            Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaParametros cargaOcorrencaParametros = new Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaParametros
            {
                CargaOcorrencia = cargaOcorrencia,
                ParametroOcorrencia = gatilhoGeracaoAutomatica.Parametro,
                DataInicio = dataInicio,
                DataFim = dataFim,
                TotalHoras = calculoFreteOcorrencia.HorasOcorrencia
            };

            repositorioCargaOcorrenciaParametros.Inserir(cargaOcorrencaParametros);
        }

        private void DefinirFluxoGeralOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia, List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> listaCargaCTe, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            string mensagemErroFluxoGeral = string.Empty;

            if (!new CargaOcorrencia.Ocorrencia().FluxoGeralOcorrencia(ref cargaOcorrencia, listaCargaCTe, null, ref mensagemErroFluxoGeral, _unitOfWork, tipoServicoMultisoftware, null, ObterConfiguracaoEmbarcador(), clienteMultisoftware, "", false))
            {
                Log.TratarErro($"PROBLEMAS DefinirFluxoGeralOcorrencia: {mensagemErroFluxoGeral}", "GATILHO");
                throw new ServicoException(mensagemErroFluxoGeral);
            }

        }

        private void DefinirModeloDocumentoFiscal(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia, List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> listaCargaCTe, TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Ocorrencia servicoOcorrenciaOcorrencia = new Ocorrencia();

            if (!servicoOcorrenciaOcorrencia.SetaModeloDocumentoFiscal(ref cargaOcorrencia, listaCargaCTe, out string erroModeloDocumento, _unitOfWork, tipoServicoMultisoftware))
                throw new ServicoException(erroModeloDocumento);
        }

        private Dominio.ObjetosDeValor.Embarcador.Ocorrencia.CalculoFreteOcorrencia ObterCalculoFreteOcorrencia(Dominio.Entidades.Embarcador.Cargas.Carga carga, double fronteiraParqueamento, List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> listaCargaCTe, Dominio.Entidades.Embarcador.Ocorrencias.GatilhoGeracaoAutomaticaOcorrencia gatilhoGeracaoAutomatica, DateTime dataInicio, DateTime dataFim, TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Carga.Ocorrencia servicoOcorrenciaCalcularValorFrete = new Carga.Ocorrencia();
            Dominio.ObjetosDeValor.Embarcador.Ocorrencia.ParametroCalcularValorOcorrencia parametrosCalcularValorOcorrencia = new Dominio.ObjetosDeValor.Embarcador.Ocorrencia.ParametroCalcularValorOcorrencia()
            {
                CodigoCarga = carga.Codigo,
                CodigoParametroPeriodo = gatilhoGeracaoAutomatica.Codigo,
                CodigoTipoOcorrencia = gatilhoGeracaoAutomatica.TipoOcorrencia.Codigo,
                DataFim = dataFim,
                DataInicio = dataInicio,
                Minutos = (int)(dataFim - dataInicio).TotalMinutes,
                DeducaoHoras = ObterHorasDeducaoPorGatilho(carga, gatilhoGeracaoAutomatica),
                ListaCargaCTe = listaCargaCTe,
                FronteiraOUParqueamento = fronteiraParqueamento
            };
            Log.TratarErro($"parametrosCalcularValorOcorrencia.DeducaoHoras: {parametrosCalcularValorOcorrencia.DeducaoHoras}; parametrosCalcularValorOcorrencia.Minutos: {parametrosCalcularValorOcorrencia.Minutos}", "GATILHO");

            if (parametrosCalcularValorOcorrencia.DeducaoHoras > 0 && parametrosCalcularValorOcorrencia.VerificarDeducaoHorasMaiorQueTempoTotal())
            {
                Log.TratarErro($"hora total <= 0. Não será gerado ocorrência", "GATILHO");
                return null;
            }

            return servicoOcorrenciaCalcularValorFrete.CalcularValorOcorrencia(parametrosCalcularValorOcorrencia, _unitOfWork, ObterConfiguracaoEmbarcador(), tipoServicoMultisoftware);
        }

        private bool GatilhoDeveDeduzirHorasPorHorasMinima(Dominio.Entidades.Embarcador.Ocorrencias.GatilhoGeracaoAutomaticaOcorrencia gatilhoGeracaoAutomatica)
        {
            if (gatilhoGeracaoAutomatica.UtilizarTempoCarregamentoComoHoraMinima)
                return false;

            if (!gatilhoGeracaoAutomatica.GatilhoInicialTraking.HasValue)
                return false;

            return (new List<GatilhoInicialTraking>() {
                GatilhoInicialTraking.PrevisaoDescarga,
                GatilhoInicialTraking.EntradaCliente,
                GatilhoInicialTraking.EntradaFronteira
            }).Contains(gatilhoGeracaoAutomatica.GatilhoInicialTraking.Value);
        }

        private bool GatilhoDeveDeduzirHorasPorTempoCarregamento(Dominio.Entidades.Embarcador.Ocorrencias.GatilhoGeracaoAutomaticaOcorrencia gatilhoGeracaoAutomatica)
        {
            if (gatilhoGeracaoAutomatica.UtilizarTempoCarregamentoComoHoraMinima)
                return true;

            if (gatilhoGeracaoAutomatica.GatilhoInicialFluxoPatio.HasValue && gatilhoGeracaoAutomatica.GatilhoInicialFluxoPatio.Value == TipoGatilhoOcorrenciaInicialFluxoPatio.PrevisaoCarregamento)
                return true;

            return false;
        }

        private string ObterCfopOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.GatilhoGeracaoAutomaticaOcorrencia gatilhoGeracaoAutomatica, List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> listaCargaCTe)
        {
            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = (from o in listaCargaCTe where o.CTe != null && o.CargaCTeComplementoInfo == null select o.CTe).FirstOrDefault();

            if (cte != null && cte.LocalidadeInicioPrestacao.Estado.Sigla == cte.LocalidadeTerminoPrestacao.Estado.Sigla)
            {
                if (cte.ValorICMS == 0 && !string.IsNullOrWhiteSpace(gatilhoGeracaoAutomatica.TipoOcorrencia.CodigoIntegracaoCFOPEstadualIsento))
                    return gatilhoGeracaoAutomatica.TipoOcorrencia.CodigoIntegracaoCFOPEstadualIsento;
                else
                    return !string.IsNullOrWhiteSpace(gatilhoGeracaoAutomatica.TipoOcorrencia.CodigoIntegracaoCFOPEstadual) ? gatilhoGeracaoAutomatica.TipoOcorrencia.CodigoIntegracaoCFOPEstadual : string.Empty;
            }

            if (cte != null)
            {
                if (cte.ValorICMS == 0 && !string.IsNullOrWhiteSpace(gatilhoGeracaoAutomatica.TipoOcorrencia.CodigoIntegracaoCFOPInterestadualIsento))
                    return gatilhoGeracaoAutomatica.TipoOcorrencia.CodigoIntegracaoCFOPInterestadualIsento;
                else
                    return !string.IsNullOrWhiteSpace(gatilhoGeracaoAutomatica.TipoOcorrencia.CodigoIntegracaoCFOPInterestadual) ? gatilhoGeracaoAutomatica.TipoOcorrencia.CodigoIntegracaoCFOPInterestadual : !string.IsNullOrWhiteSpace(gatilhoGeracaoAutomatica.TipoOcorrencia.CodigoIntegracaoCFOPEstadual) ? gatilhoGeracaoAutomatica.TipoOcorrencia.CodigoIntegracaoCFOPEstadual : string.Empty;
            }

            return string.Empty;
        }

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ObterConfiguracaoEmbarcador()
        {
            if (_configuracaoEmbarcador == null)
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
                _configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
            }

            return _configuracaoEmbarcador;
        }

        private void VerificarClienteBloqueadoParaGeracaoOcorrencia(Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoOcorrencia, List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> listaCargaCTe)
        {
            if ((tipoOcorrencia.ClientesBloqueados == null) || (tipoOcorrencia.ClientesBloqueados.Count == 0))
                return;

            foreach (Dominio.Entidades.Embarcador.Ocorrencias.ClientesBloqueados clienteBloqueado in tipoOcorrencia.ClientesBloqueados)
            {
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in listaCargaCTe)
                {
                    if (cargaCTe.CTe == null)
                        continue;

                    if (clienteBloqueado.TipoCliente == Dominio.Enumeradores.TipoTomador.Remetente)
                    {
                        if (clienteBloqueado.Cliente.CPF_CNPJ_SemFormato == cargaCTe.CTe.Remetente.CPF_CNPJ_SemFormato)
                            throw new ServicoException($"Cliente origem {clienteBloqueado.Cliente.CPF_CNPJ_SemFormato}{(!string.IsNullOrWhiteSpace(clienteBloqueado.Cliente.CodigoIntegracao) ? $" ({clienteBloqueado.Cliente.CodigoIntegracao}) " : " ")}{clienteBloqueado.Cliente.Nome} não permitido para lançamento da ocorrência.");
                    }
                    else if (clienteBloqueado.TipoCliente == Dominio.Enumeradores.TipoTomador.Destinatario)
                    {
                        if (clienteBloqueado.Cliente.CPF_CNPJ_SemFormato == cargaCTe.CTe.Destinatario.CPF_CNPJ_SemFormato)
                            throw new ServicoException($"Cliente destino {clienteBloqueado.Cliente.CPF_CNPJ_SemFormato}{(!string.IsNullOrWhiteSpace(clienteBloqueado.Cliente.CodigoIntegracao) ? $" ({clienteBloqueado.Cliente.CodigoIntegracao}) " : " ")}{clienteBloqueado.Cliente.Nome} não permitido para lançamento da ocorrência.");
                    }
                }
            }
        }

        private void VerificarComplementoValorFreteCarga(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia)
        {
            if (!cargaOcorrencia.TipoOcorrencia.OcorrenciaComplementoValorFreteCarga)
                return;

            if (
                cargaOcorrencia.Carga.SituacaoCarga != SituacaoCarga.Nova &&
                cargaOcorrencia.Carga.SituacaoCarga != SituacaoCarga.AgNFe &&
                cargaOcorrencia.Carga.SituacaoCarga != SituacaoCarga.CalculoFrete
            )
                throw new ServicoException("A situação da carga não permite que a ocorrência de complemento do valor do frete seja adicionada.");
        }

        private List<Dominio.Entidades.Embarcador.Ocorrencias.GatilhoGeracaoAutomaticaOcorrencia> FiltrarListaGatilhosPorFilialEmpresaTipoOperacao(List<Dominio.Entidades.Embarcador.Ocorrencias.GatilhoGeracaoAutomaticaOcorrencia> listaGatilhoGeracaoAutomatica, int codigoFilial, int codigoEmpresa, int codigoTipoOperacao)
        {
            if (listaGatilhoGeracaoAutomatica.Count == 0)
                return null;

            if (codigoFilial > 0 && listaGatilhoGeracaoAutomatica.Any(o => o.Filiais.Any(f => f.Codigo == codigoFilial)))
                listaGatilhoGeracaoAutomatica = listaGatilhoGeracaoAutomatica.Where(g => g.Filiais.Any(o => o.Codigo == codigoFilial)).ToList();
            else
                listaGatilhoGeracaoAutomatica = listaGatilhoGeracaoAutomatica.Where(g => g.Filiais.Count == 0).ToList();

            if (codigoEmpresa > 0 && listaGatilhoGeracaoAutomatica.Any(o => o.Transportadores.Any(t => t.Codigo == codigoEmpresa)))
                listaGatilhoGeracaoAutomatica = listaGatilhoGeracaoAutomatica.Where(g => g.Transportadores.Any(o => o.Codigo == codigoEmpresa)).ToList();
            else
                listaGatilhoGeracaoAutomatica = listaGatilhoGeracaoAutomatica.Where(g => g.Transportadores.Count == 0).ToList();

            if (codigoTipoOperacao > 0 && listaGatilhoGeracaoAutomatica.Any(o => o.TiposOperacoes.Any(t => t.Codigo == codigoTipoOperacao)))
                listaGatilhoGeracaoAutomatica = listaGatilhoGeracaoAutomatica.Where(g => g.TiposOperacoes.Any(o => o.Codigo == codigoTipoOperacao)).ToList();
            else
                listaGatilhoGeracaoAutomatica = listaGatilhoGeracaoAutomatica.Where(g => g.TiposOperacoes.Count == 0).ToList();

            return listaGatilhoGeracaoAutomatica.ToList();
        }

        private List<Dominio.Entidades.Embarcador.Ocorrencias.GatilhoGeracaoAutomaticaOcorrencia> ObterListaGatilhos(GatilhoFinalTraking gatilho, int codigoFilial, int codigoEmpresa, int codigoTipoOperacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAplicacaoGatilhoTracking tipoAplicacaoGatilhoTracking)
        {
            Repositorio.Embarcador.Ocorrencias.GatilhoGeracaoAutomaticaOcorrencia repositorioGatilhoGeracaoAutomaticaOcorrencia = new Repositorio.Embarcador.Ocorrencias.GatilhoGeracaoAutomaticaOcorrencia(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Ocorrencias.GatilhoGeracaoAutomaticaOcorrencia> listaGatilhoGeracaoAutomatica = repositorioGatilhoGeracaoAutomaticaOcorrencia.BuscarPorGatilhoAutomaticoFinalTracking(gatilho, codigoFilial, codigoEmpresa, codigoTipoOperacao, tipoAplicacaoGatilhoTracking);

            return FiltrarListaGatilhosPorFilialEmpresaTipoOperacao(listaGatilhoGeracaoAutomatica, codigoFilial, codigoEmpresa, codigoTipoOperacao);
        }

        private List<Dominio.Entidades.Embarcador.Ocorrencias.GatilhoGeracaoAutomaticaOcorrencia> ObterListaGatilhos(EtapaFluxoGestaoPatio etapaFluxoPatio, int codigoFilial, int codigoEmpresa, int codigoTipoOperacao)
        {
            Repositorio.Embarcador.Ocorrencias.GatilhoGeracaoAutomaticaOcorrencia repositorioGatilhoGeracaoAutomaticaOcorrencia = new Repositorio.Embarcador.Ocorrencias.GatilhoGeracaoAutomaticaOcorrencia(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Ocorrencias.GatilhoGeracaoAutomaticaOcorrencia> listaGatilhoGeracaoAutomatica = repositorioGatilhoGeracaoAutomaticaOcorrencia.BuscarPorGatilhoAutomaticoFinalFluxoPatio(etapaFluxoPatio, codigoFilial, codigoEmpresa, codigoTipoOperacao);

            return FiltrarListaGatilhosPorFilialEmpresaTipoOperacao(listaGatilhoGeracaoAutomatica, codigoFilial, codigoEmpresa, codigoTipoOperacao);
        }

        private DateTime? CalcularDataChegadaPatio(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            if (!fluxoGestaoPatio.DataChegadaVeiculoPrevista.HasValue)
                return null;

            DateTime dataChegadaPrevista = fluxoGestaoPatio.DataChegadaVeiculoPrevista.Value;

            if (fluxoGestaoPatio.DataChegadaVeiculo.HasValue && (fluxoGestaoPatio.DataChegadaVeiculo.Value <= dataChegadaPrevista))
                return dataChegadaPrevista;

            DateTime dataChegadaPraticada = fluxoGestaoPatio.DataChegadaVeiculo.Value;
            // Quando o veículo chega atrasado ( após a data e hora da sua programação de embarque), ele fica automaticamente programado para o próximo dia da sua chegada no mesmo horário que estava programado.
            // dataBaseProximaChegada = Data da chega + Hora prevista
            DateTime dataBaseProximaChegada = new DateTime(dataChegadaPraticada.Year, dataChegadaPraticada.Month, dataChegadaPraticada.Day, dataChegadaPrevista.Hour, dataChegadaPrevista.Minute, dataChegadaPrevista.Second);

            return CalcularProximaData(dataBaseProximaChegada);
        }

        private DateTime? CalcularDataPrevisaoChegadaDestinatario(Dominio.Entidades.Embarcador.Cargas.Carga carga, DateTime dataChegada)
        {
            DateTime? previsaoChegada = carga.DataPrevisaoTerminoCarga ?? carga.Carregamento?.DataDescarregamentoCarga;

            if (!previsaoChegada.HasValue)
                return null;

            if (dataChegada <= previsaoChegada.Value)
                return previsaoChegada;

            return CalcularProximaData(previsaoChegada.Value);
        }

        private DateTime? CalcularProximaData(DateTime data)
        {
            DiaSemana diaSemana = DiaSemanaHelper.ObterDiaSemana(data);

            if (diaSemana == DiaSemana.Sexta)
                data = data.AddDays(3);
            else if (diaSemana == DiaSemana.Sabado)
                data = data.AddDays(2);
            else
                data = data.AddDays(1);

            return data;
        }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Ocorrencias.GatilhoGeracaoAutomaticaOcorrencia> ObterListaGatilhos(TipoDataAlteracaoGatilho dataGatilho, int codigoFilial, int codigoEmpresa, int codigoTipoOperacao)
        {
            Repositorio.Embarcador.Ocorrencias.GatilhoGeracaoAutomaticaOcorrencia repositorioGatilhoGeracaoAutomaticaOcorrencia = new Repositorio.Embarcador.Ocorrencias.GatilhoGeracaoAutomaticaOcorrencia(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Ocorrencias.GatilhoGeracaoAutomaticaOcorrencia> listaGatilhoGeracaoAutomatica = repositorioGatilhoGeracaoAutomaticaOcorrencia.BuscarPorGatilhoAutomaticoAlteracaoData(dataGatilho, codigoFilial, codigoEmpresa, codigoTipoOperacao);

            return FiltrarListaGatilhosPorFilialEmpresaTipoOperacao(listaGatilhoGeracaoAutomatica, codigoFilial, codigoEmpresa, codigoTipoOperacao);
        }

        public int ObterHorasDeducaoPorGatilho(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Ocorrencias.GatilhoGeracaoAutomaticaOcorrencia gatilhoGeracaoAutomatica)
        {
            Log.TratarErro($"GatilhoDeveDeduzirHorasPorHorasMinima.DeducaoHoras: {GatilhoDeveDeduzirHorasPorHorasMinima(gatilhoGeracaoAutomatica)}; GatilhoDeveDeduzirHorasPorTempoCarregamento(gatilhoGeracaoAutomatica): {GatilhoDeveDeduzirHorasPorTempoCarregamento(gatilhoGeracaoAutomatica)}", "GATILHO");

            if (GatilhoDeveDeduzirHorasPorHorasMinima(gatilhoGeracaoAutomatica))
                return gatilhoGeracaoAutomatica.HorasMinimas;

            if (GatilhoDeveDeduzirHorasPorTempoCarregamento(gatilhoGeracaoAutomatica))
            {
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorCarga(carga.Codigo);

                Log.TratarErro($"janelaCarregamento?.Codigo: {cargaJanelaCarregamento?.Codigo}; janelaCarregamento?.TempoCarregamento: {cargaJanelaCarregamento?.TempoCarregamento}", "GATILHO");

                if (cargaJanelaCarregamento == null)
                    return 0;

                int tempoCarregamento = cargaJanelaCarregamento.TempoCarregamento;

                if (tempoCarregamento == 0)
                {
                    Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();
                    Logistica.CargaJanelaCarregamentoConsulta servicoCargaJanelaCarregamentoConsulta = new Logistica.CargaJanelaCarregamentoConsulta(_unitOfWork, configuracaoEmbarcador);

                    tempoCarregamento = servicoCargaJanelaCarregamentoConsulta.ObterTempoCarregamento(cargaJanelaCarregamento, cargaJanelaCarregamento.InicioCarregamento.TimeOfDay);
                }

                return tempoCarregamento / 60;
            }

            return 0;
        }

        public (DateTime? DataInicio, DateTime? DataFim) ObterDataInicioEFimGatilho(DateTime dataPrevista, DateTime dataEntrada, DateTime dataSaida)
        {
            if (dataPrevista == DateTime.MinValue)
                return ValueTuple.Create(dataEntrada, dataSaida);

            if (dataEntrada <= dataPrevista)
                return ValueTuple.Create(dataPrevista, dataSaida);

            // Quando o veículo chega atrasado ( após a data e hora da sua programação de embarque), ele fica automaticamente programado para o próximo dia da sua chegada no mesmo horário que estava programado.
            // dataBaseProximaChegada = Data da chega + Hora prevista
            DateTime dataBaseProximaChegada = new DateTime(dataEntrada.Year, dataEntrada.Month, dataEntrada.Day, dataPrevista.Hour, dataPrevista.Minute, dataPrevista.Second);

            DateTime? dataChegadaCalculada = CalcularProximaData(dataBaseProximaChegada);

            return ValueTuple.Create(dataChegadaCalculada, dataSaida);
        }

        public (DateTime? DataInicio, DateTime? DataFim) ObterDataInicioEFimGatilhoPorTracking(Dominio.Entidades.Embarcador.Ocorrencias.GatilhoGeracaoAutomaticaOcorrencia gatilho, Dominio.Entidades.Embarcador.Cargas.Carga carga, DateTime dataEntrada, DateTime dataSaida, DateTime? dataAgendamentoEntrega)
        {
            (DateTime? DataInicio, DateTime? DataFim) dados = ValueTuple.Create((DateTime?)null, (DateTime?)null);

            if (gatilho.GatilhoInicialTraking == GatilhoInicialTraking.PrevisaoDescarga)
            {
                DateTime? dataInicio = CalcularDataPrevisaoChegadaDestinatario(carga, dataEntrada);

                if (dataInicio.HasValue && (dataSaida - dataInicio.Value).TotalHours >= gatilho.HorasMinimas)
                {
                    dados.DataInicio = dataInicio;
                    dados.DataFim = dataSaida;
                }
            }
            else if (gatilho.GatilhoInicialTraking == GatilhoInicialTraking.EntradaFronteira)
            {
                if ((dataSaida - dataEntrada).TotalHours >= gatilho.HorasMinimas)
                {
                    dados.DataInicio = dataEntrada;
                    dados.DataFim = dataSaida;
                }
            }
            else if (gatilho.GatilhoInicialTraking == GatilhoInicialTraking.EntradaCliente)
            {

                if ((dataSaida - dataEntrada).TotalHours >= gatilho.HorasMinimas)
                {
                    dados.DataInicio = gatilho.ValidarDataAgendadaEntrega ? dataAgendamentoEntrega : dataEntrada;
                    dados.DataFim = dataSaida;
                }
            }
            else if (gatilho.GatilhoInicialTraking == GatilhoInicialTraking.InicioEntrega)
            {
                if ((dataSaida - dataEntrada).TotalHours >= gatilho.HorasMinimas)
                {
                    dados.DataInicio = dataEntrada;
                    dados.DataFim = dataSaida;
                }
            }

            return dados;
        }

        public (DateTime? DataInicio, DateTime? DataFim) ObterDataInicioEFimGatilhoPorGestaoPatio(Dominio.Entidades.Embarcador.Ocorrencias.GatilhoGeracaoAutomaticaOcorrencia gatilho, Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento janelaCarregamento, Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, DateTime dataFinalizacao)
        {
            (DateTime? DataInicio, DateTime? DataFim) dados = ValueTuple.Create((DateTime?)null, (DateTime?)null);
            int totalHorasPorChegadaPatio = (fluxoGestaoPatio?.DataChegadaVeiculo == null) ? 0 : (int)Math.Round((dataFinalizacao - fluxoGestaoPatio.DataChegadaVeiculo.Value).TotalHours, MidpointRounding.AwayFromZero);
            int totalHorasPorInicioCarregamento = (janelaCarregamento?.InicioCarregamento == null) ? 0 : (int)Math.Round((dataFinalizacao - janelaCarregamento.InicioCarregamento).TotalHours, MidpointRounding.AwayFromZero);

            if (gatilho.GatilhoInicialFluxoPatio == TipoGatilhoOcorrenciaInicialFluxoPatio.ChegadaPatio)
            {
                if (totalHorasPorChegadaPatio >= gatilho.HorasMinimas)
                {
                    dados.DataInicio = fluxoGestaoPatio.DataChegadaVeiculo.Value;
                    dados.DataFim = dados.DataInicio.Value.AddHours(totalHorasPorChegadaPatio);
                }
            }
            else if (gatilho.GatilhoInicialFluxoPatio == TipoGatilhoOcorrenciaInicialFluxoPatio.InicioCarregamento)
            {
                if (totalHorasPorInicioCarregamento >= gatilho.HorasMinimas)
                {
                    dados.DataInicio = janelaCarregamento.InicioCarregamento;
                    dados.DataFim = dados.DataInicio.Value.AddHours(totalHorasPorInicioCarregamento);
                }
            }
            else if (gatilho.GatilhoInicialFluxoPatio == TipoGatilhoOcorrenciaInicialFluxoPatio.PrevisaoCarregamento)
            {
                DateTime? dataInicio = CalcularDataChegadaPatio(fluxoGestaoPatio);
                DateTime? dataFim = new GestaoPatio.FluxoGestaoPatio(_unitOfWork).ObterDataEtapa(fluxoGestaoPatio, gatilho.GatilhoFinalFluxoPatio.Value);
                if (dataInicio.HasValue && dataFim.HasValue && (dataFim - dataInicio).Value.TotalHours > gatilho.HorasMinimas)
                {
                    dados.DataInicio = dataInicio;
                    dados.DataFim = dataFim;
                }
            }

            return dados;
        }

        public void GerarOcorrenciaPorTracking(Dominio.Entidades.Embarcador.Cargas.Carga carga, double codigoFronteiraParqueamento, GatilhoFinalTraking gatilhoFinal, DateTime dataEntrada, DateTime dataSaida, TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAplicacaoGatilhoTracking tipoAplicacaoGatilhoTracking, DateTime? dataAgendamentoEntrega)
        {
            int codigoFilial = carga.Filial?.Codigo ?? 0;
            int codigoEmpresa = carga.Empresa?.Codigo ?? 0;
            int codigoTipoOperacao = carga.TipoOperacao?.Codigo ?? 0;

            List<Dominio.Entidades.Embarcador.Ocorrencias.GatilhoGeracaoAutomaticaOcorrencia> listaGatilhoGeracaoAutomatica = ObterListaGatilhos(gatilhoFinal, codigoFilial, codigoEmpresa, codigoTipoOperacao, tipoAplicacaoGatilhoTracking);

            if (listaGatilhoGeracaoAutomatica == null || listaGatilhoGeracaoAutomatica.Count == 0)
                return;

            Dominio.Entidades.Embarcador.Ocorrencias.GatilhoGeracaoAutomaticaOcorrencia gatilhoGeracaoAutomaticaUtilizar = null;
            bool gerarOcorrenciaDataAgendada = true;
            DateTime? dataInicio = null;
            DateTime? dataFim = null;

            foreach (Dominio.Entidades.Embarcador.Ocorrencias.GatilhoGeracaoAutomaticaOcorrencia gatilhoGeracaoAutomatica in listaGatilhoGeracaoAutomatica)
            {
                (DateTime? DataInicio, DateTime? DataFim) dados = ObterDataInicioEFimGatilhoPorTracking(gatilhoGeracaoAutomatica, carga, dataEntrada, dataSaida, dataAgendamentoEntrega);

                if (dados.DataInicio.HasValue && dados.DataFim.HasValue)
                {
                    dataInicio = dados.DataInicio;
                    dataFim = dados.DataFim;
                    gatilhoGeracaoAutomaticaUtilizar = gatilhoGeracaoAutomatica;

                    if (gatilhoGeracaoAutomatica.ValidarDataAgendadaEntrega && dataEntrada > dataAgendamentoEntrega)
                        gerarOcorrenciaDataAgendada = false;

                    break;
                }
            }

            if (gatilhoGeracaoAutomaticaUtilizar != null && gerarOcorrenciaDataAgendada)
                AdicionarOcorrencia(gatilhoGeracaoAutomaticaUtilizar, carga, dataInicio.Value, dataFim.Value, tipoServicoMultisoftware, clienteMultisoftware, codigoFronteiraParqueamento);
        }

        public void GerarOcorrenciaPorFinalizacaoEtapaFluxoPatio(Dominio.Entidades.Embarcador.Cargas.Carga carga, EtapaFluxoGestaoPatio etapaFluxoPatio, DateTime dataFinalizacao, TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
        {
            int codigoFilial = carga.Filial?.Codigo ?? 0;
            int codigoEmpresa = carga.Empresa?.Codigo ?? 0;
            int codigoTipoOperacao = carga.TipoOperacao?.Codigo ?? 0;

            List<Dominio.Entidades.Embarcador.Ocorrencias.GatilhoGeracaoAutomaticaOcorrencia> listaGatilhoGeracaoAutomatica = ObterListaGatilhos(etapaFluxoPatio, codigoFilial, codigoEmpresa, codigoTipoOperacao);

            if (listaGatilhoGeracaoAutomatica == null || listaGatilhoGeracaoAutomatica.Count == 0)
                return;

            Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio = servicoFluxoGestaoPatio.ObterFluxoGestaoPatio(carga);

            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento janelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorCarga(carga.Codigo);

            Dominio.Entidades.Embarcador.Ocorrencias.GatilhoGeracaoAutomaticaOcorrencia gatilhoGeracaoAutomaticaUtilizar = null;
            DateTime? dataInicio = null;
            DateTime? dataFim = null;

            foreach (Dominio.Entidades.Embarcador.Ocorrencias.GatilhoGeracaoAutomaticaOcorrencia gatilhoGeracaoAutomatica in listaGatilhoGeracaoAutomatica)
            {
                (DateTime? DataInicio, DateTime? DataFim) dados = ObterDataInicioEFimGatilhoPorGestaoPatio(gatilhoGeracaoAutomatica, janelaCarregamento, fluxoGestaoPatio, dataFinalizacao);

                if (dados.DataInicio.HasValue && dados.DataFim.HasValue)
                {
                    dataInicio = dados.DataInicio;
                    dataFim = dados.DataFim;
                    gatilhoGeracaoAutomaticaUtilizar = gatilhoGeracaoAutomatica;
                    break;
                }
            }

            if ((gatilhoGeracaoAutomaticaUtilizar != null))
                AdicionarOcorrencia(gatilhoGeracaoAutomaticaUtilizar, carga, dataInicio.Value, dataFim.Value, tipoServicoMultisoftware, clienteMultisoftware, 0);
        }

        #endregion
    }
}
