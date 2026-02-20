using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Integracoes
{
    [CustomAuthorize("Integracoes/ConfiguracaoIntegracaoTecnologiaMonitoramento")]
    public class ConfiguracaoIntegracaoTecnologiaMonitoramentoController : BaseController
    {
		#region Construtores

		public ConfiguracaoIntegracaoTecnologiaMonitoramentoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = ObterGridPesquisa();

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> ExportarPesquisa()
        {
            try
            {
                Models.Grid.Grid grid = ObterGridPesquisa();

                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoGerarArquivo);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExportar);
            }
        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                SalvarConfiguracaoIntegracaoTecnologiaMonitoramento(true, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAdicionar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                SalvarConfiguracaoIntegracaoTecnologiaMonitoramento(false, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAtualizar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                long codigo = Request.GetLongParam("Codigo");

                Repositorio.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramento repConfiguracaoIntegracaoTecnologiaMonitoramento = new Repositorio.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramento(unitOfWork);
                Repositorio.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoConta repConfiguracaoIntegracaoTecnologiaMonitoramentoConta = new Repositorio.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoConta(unitOfWork);
                Repositorio.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorar repConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorar = new Repositorio.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorar(unitOfWork);
                Repositorio.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoOpcao repConfiguracaoIntegracaoTecnologiaMonitoramentoOpcao = new Repositorio.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoOpcao(unitOfWork);

                Dominio.Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramento configuracaoIntegracaoTecnologiaMonitoramento = repConfiguracaoIntegracaoTecnologiaMonitoramento.BuscarPorCodigo(codigo, false);
                List<Dominio.Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoConta> configuracaoIntegracaoTecnologiaMonitoramentoContas = repConfiguracaoIntegracaoTecnologiaMonitoramentoConta.BuscarPorConfiguracao(configuracaoIntegracaoTecnologiaMonitoramento);
                List<Dominio.Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorar> configuracaoIntegracaoTecnologiaMonitoramentoMonitorar = repConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorar.BuscarPorConfiguracao(configuracaoIntegracaoTecnologiaMonitoramento);
                List<Dominio.Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoOpcao> configuracaoIntegracaoTecnologiaMonitoramentoOpcoes = repConfiguracaoIntegracaoTecnologiaMonitoramentoOpcao.BuscarPorConfiguracao(configuracaoIntegracaoTecnologiaMonitoramento);

                return new JsonpResult(new
                {
                    configuracaoIntegracaoTecnologiaMonitoramento.Codigo,
                    configuracaoIntegracaoTecnologiaMonitoramento.Habilitada,
                    configuracaoIntegracaoTecnologiaMonitoramento.CodigoIntegracao,
                    configuracaoIntegracaoTecnologiaMonitoramento.Tipo,
                    configuracaoIntegracaoTecnologiaMonitoramento.MinutosDiferencaMinimaEntrePosicoes,
                    configuracaoIntegracaoTecnologiaMonitoramento.ProcessarSensores,
                    configuracaoIntegracaoTecnologiaMonitoramento.TempoSleepEntreContas,
                    configuracaoIntegracaoTecnologiaMonitoramento.TempoSleepThread,
                    Opcoes = configuracaoIntegracaoTecnologiaMonitoramentoOpcoes.Select(o => new
                    {
                        o.Codigo,
                        o.Value,
                        o.Key
                    }).ToList(),
                    Monitorar = configuracaoIntegracaoTecnologiaMonitoramentoMonitorar.Select(o => new
                    {
                        o.Codigo,
                        o.Value,
                        o.Key
                    }).ToList(),
                    Contas = configuracaoIntegracaoTecnologiaMonitoramentoContas.Select(o => new
                    {
                        o.Codigo,
                        o.Habilitada,
                        o.RastreadorId,
                        o.Usuario,
                        o.ParametrosAdicionais,
                        o.ArquivoControle,
                        o.BancoDeDados,
                        o.UsaPosicaoFrota,
                        o.BuscarDadosVeiculos,
                        o.Charset,
                        o.Diretorio,
                        o.Nome,
                        o.Porta,
                        o.Protocolo,
                        o.Senha,
                        o.Servidor,
                        o.SolicitanteId,
                        o.SolicitanteSenha,
                        o.TipoComunicacaoIntegracao,
                        o.URI
                    }).ToList()
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoBuscarPorCodigo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                long codigo = Request.GetLongParam("Codigo");

                Repositorio.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramento repConfiguracaoIntegracaoTecnologiaMonitoramento = new Repositorio.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramento(unitOfWork);
                Repositorio.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoConta repConfiguracaoIntegracaoTecnologiaMonitoramentoConta = new Repositorio.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoConta(unitOfWork);
                Repositorio.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorar repConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorar = new Repositorio.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorar(unitOfWork);
                Repositorio.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoOpcao repConfiguracaoIntegracaoTecnologiaMonitoramentoOpcao = new Repositorio.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoOpcao(unitOfWork);

                Dominio.Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramento configuracaoIntegracaoTecnologiaMonitoramento = repConfiguracaoIntegracaoTecnologiaMonitoramento.BuscarPorCodigo(codigo, false);

                unitOfWork.Start();

                repConfiguracaoIntegracaoTecnologiaMonitoramentoConta.DeletarPorConfiguracao(configuracaoIntegracaoTecnologiaMonitoramento);
                repConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorar.DeletarPorConfiguracao(configuracaoIntegracaoTecnologiaMonitoramento);
                repConfiguracaoIntegracaoTecnologiaMonitoramentoOpcao.DeletarPorConfiguracao(configuracaoIntegracaoTecnologiaMonitoramento);
                repConfiguracaoIntegracaoTecnologiaMonitoramento.Deletar(configuracaoIntegracaoTecnologiaMonitoramento);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();

                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, Localization.Resources.Cargas.TipoCarga.NaoFoiPossivelExcluirRegistroPoisMesmoJaPossuiVinculoComOutrosRecursosDoSistemaRecomendamosQueVoceInativeRegistroCasoNaoDesejaMaisUtilizaLo);
                else
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, Localization.Resources.Cargas.TipoCarga.OcorreuUmaFalhaAoExcluir);
                }
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void SalvarConfiguracaoIntegracaoTecnologiaMonitoramento(bool adicionar, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramento repConfiguracaoIntegracaoTecnologiaMonitoramento = new Repositorio.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramento(unitOfWork);

            Dominio.Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramento configuracaoIntegracaoTecnologiaMonitoramento;

            if (adicionar)
            {
                configuracaoIntegracaoTecnologiaMonitoramento = new Dominio.Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramento()
                {
                    Tipo = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>("Tipo")
                };
            }
            else
                configuracaoIntegracaoTecnologiaMonitoramento = repConfiguracaoIntegracaoTecnologiaMonitoramento.BuscarPorCodigo(Request.GetLongParam("Codigo"), true);

            configuracaoIntegracaoTecnologiaMonitoramento.Habilitada = Request.GetBoolParam("Habilitada");
            configuracaoIntegracaoTecnologiaMonitoramento.CodigoIntegracao = Request.GetStringParam("CodigoIntegracao");
            configuracaoIntegracaoTecnologiaMonitoramento.MinutosDiferencaMinimaEntrePosicoes = Request.GetIntParam("MinutosDiferencaMinimaEntrePosicoes");
            configuracaoIntegracaoTecnologiaMonitoramento.ProcessarSensores = Request.GetBoolParam("ProcessarSensores");
            configuracaoIntegracaoTecnologiaMonitoramento.TempoSleepEntreContas = Request.GetIntParam("TempoSleepEntreContas");
            configuracaoIntegracaoTecnologiaMonitoramento.TempoSleepThread = Request.GetIntParam("TempoSleepThread");

            if (adicionar)
                repConfiguracaoIntegracaoTecnologiaMonitoramento.Inserir(configuracaoIntegracaoTecnologiaMonitoramento, Auditado);
            else
                repConfiguracaoIntegracaoTecnologiaMonitoramento.Atualizar(configuracaoIntegracaoTecnologiaMonitoramento, Auditado);

            SalvarConfiguracaoIntegracaoTecnologiaMonitoramentoConta(adicionar, configuracaoIntegracaoTecnologiaMonitoramento, unitOfWork);
            SalvarConfiguracaoIntegracaoTecnologiaMonitoramentoOpcao(adicionar, configuracaoIntegracaoTecnologiaMonitoramento, unitOfWork);
            SalvarConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorar(adicionar, configuracaoIntegracaoTecnologiaMonitoramento, unitOfWork);
        }

        private void SalvarConfiguracaoIntegracaoTecnologiaMonitoramentoConta(bool adicionar, Dominio.Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramento configuracaoIntegracaoTecnologiaMonitoramento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoConta repConfiguracaoIntegracaoTecnologiaMonitoramentoConta = new Repositorio.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoConta(unitOfWork);

            List<Dominio.Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoConta> configuracoesIntegracaoTecnologiaMonitoramentoContas = new List<Dominio.Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoConta>();

            if (!adicionar)
                configuracoesIntegracaoTecnologiaMonitoramentoContas = repConfiguracaoIntegracaoTecnologiaMonitoramentoConta.BuscarPorConfiguracao(configuracaoIntegracaoTecnologiaMonitoramento);

            dynamic contas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Contas"));

            if (configuracoesIntegracaoTecnologiaMonitoramentoContas.Count > 0)
            {
                List<long> codigos = new List<long>();

                foreach (dynamic conta in contas)
                {
                    if (conta.Codigo != null)
                        codigos.Add((long)conta.Codigo);
                }

                List<Dominio.Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoConta> contasDeletar = configuracoesIntegracaoTecnologiaMonitoramentoContas.FindAll(o => !codigos.Contains(o.Codigo));

                for (var i = 0; i < contasDeletar.Count; i++)
                    repConfiguracaoIntegracaoTecnologiaMonitoramentoConta.Deletar(contasDeletar[i], Auditado);
            }

            foreach (dynamic conta in contas)
            {
                Dominio.Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoConta configuracaoIntegracaoTecnologiaMonitoramentoConta = null;
                long codigo = 0;

                if (conta.Codigo != null && long.TryParse((string)conta.Codigo, out codigo))
                    configuracaoIntegracaoTecnologiaMonitoramentoConta = repConfiguracaoIntegracaoTecnologiaMonitoramentoConta.BuscarPorCodigo(codigo, true);

                if (configuracaoIntegracaoTecnologiaMonitoramentoConta == null)
                {
                    configuracaoIntegracaoTecnologiaMonitoramentoConta = new Dominio.Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoConta
                    {
                        Configuracao = configuracaoIntegracaoTecnologiaMonitoramento
                    };
                }

                configuracaoIntegracaoTecnologiaMonitoramentoConta.ArquivoControle = (string)conta.ArquivoControle;
                configuracaoIntegracaoTecnologiaMonitoramentoConta.BancoDeDados = (string)conta.BancoDeDados;
                configuracaoIntegracaoTecnologiaMonitoramentoConta.BuscarDadosVeiculos = (bool)conta.BuscarDadosVeiculos;
                configuracaoIntegracaoTecnologiaMonitoramentoConta.Charset = (string)conta.Charset;
                configuracaoIntegracaoTecnologiaMonitoramentoConta.UsaPosicaoFrota = (bool)conta.UsaPosicaoFrota;
                configuracaoIntegracaoTecnologiaMonitoramentoConta.Diretorio = (string)conta.Diretorio;
                configuracaoIntegracaoTecnologiaMonitoramentoConta.Habilitada = (bool)conta.Habilitada;
                configuracaoIntegracaoTecnologiaMonitoramentoConta.Nome = (string)conta.Nome;
                configuracaoIntegracaoTecnologiaMonitoramentoConta.Servidor = (string)conta.Servidor;
                configuracaoIntegracaoTecnologiaMonitoramentoConta.ParametrosAdicionais = (string)conta.ParametrosAdicionais;
                configuracaoIntegracaoTecnologiaMonitoramentoConta.Porta = (int)conta.Porta;
                configuracaoIntegracaoTecnologiaMonitoramentoConta.Protocolo = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.Protocolo)(int)conta.Protocolo;
                configuracaoIntegracaoTecnologiaMonitoramentoConta.RastreadorId = (string)conta.RastreadorId;
                configuracaoIntegracaoTecnologiaMonitoramentoConta.Senha = (string)conta.Senha;
                configuracaoIntegracaoTecnologiaMonitoramentoConta.SolicitanteId = (string)conta.SolicitanteId;
                configuracaoIntegracaoTecnologiaMonitoramentoConta.SolicitanteSenha = (string)conta.SolicitanteSenha;
                configuracaoIntegracaoTecnologiaMonitoramentoConta.TipoComunicacaoIntegracao = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComunicacaoIntegracao)(int)conta.TipoComunicacaoIntegracao;
                configuracaoIntegracaoTecnologiaMonitoramentoConta.URI = (string)conta.URI;
                configuracaoIntegracaoTecnologiaMonitoramentoConta.Usuario = (string)conta.Usuario;

                if (configuracaoIntegracaoTecnologiaMonitoramentoConta.Codigo > 0)
                    repConfiguracaoIntegracaoTecnologiaMonitoramentoConta.Atualizar(configuracaoIntegracaoTecnologiaMonitoramentoConta, Auditado);
                else
                    repConfiguracaoIntegracaoTecnologiaMonitoramentoConta.Inserir(configuracaoIntegracaoTecnologiaMonitoramentoConta, Auditado);
            }
        }

        private void SalvarConfiguracaoIntegracaoTecnologiaMonitoramentoOpcao(bool adicionar, Dominio.Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramento configuracaoIntegracaoTecnologiaMonitoramento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoOpcao repConfiguracaoIntegracaoTecnologiaMonitoramentoOpcao = new Repositorio.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoOpcao(unitOfWork);

            List<Dominio.Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoOpcao> configuracoesIntegracaoTecnologiaMonitoramentoOpcoes = new List<Dominio.Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoOpcao>();

            if (!adicionar)
                configuracoesIntegracaoTecnologiaMonitoramentoOpcoes = repConfiguracaoIntegracaoTecnologiaMonitoramentoOpcao.BuscarPorConfiguracao(configuracaoIntegracaoTecnologiaMonitoramento);

            dynamic opcoes = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Opcoes"));

            if (configuracoesIntegracaoTecnologiaMonitoramentoOpcoes.Count > 0)
            {
                List<long> codigos = new List<long>();

                foreach (dynamic opcao in opcoes)
                {
                    if (opcao.Codigo != null)
                        codigos.Add((long)opcao.Codigo);
                }

                List<Dominio.Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoOpcao> opcoesDeletar = configuracoesIntegracaoTecnologiaMonitoramentoOpcoes.FindAll(o => !codigos.Contains(o.Codigo));

                for (var i = 0; i < opcoesDeletar.Count; i++)
                    repConfiguracaoIntegracaoTecnologiaMonitoramentoOpcao.Deletar(opcoesDeletar[i], Auditado);
            }

            foreach (dynamic opcao in opcoes)
            {
                Dominio.Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoOpcao configuracaoIntegracaoTecnologiaMonitoramentoOpcao = null;
                long codigo = 0;

                if (opcao.Codigo != null && long.TryParse((string)opcao.Codigo, out codigo))
                    configuracaoIntegracaoTecnologiaMonitoramentoOpcao = repConfiguracaoIntegracaoTecnologiaMonitoramentoOpcao.BuscarPorCodigo(codigo, true);

                if (configuracaoIntegracaoTecnologiaMonitoramentoOpcao == null)
                {
                    configuracaoIntegracaoTecnologiaMonitoramentoOpcao = new Dominio.Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoOpcao
                    {
                        Configuracao = configuracaoIntegracaoTecnologiaMonitoramento
                    };
                }

                configuracaoIntegracaoTecnologiaMonitoramentoOpcao.Key = (string)opcao.Key;
                configuracaoIntegracaoTecnologiaMonitoramentoOpcao.Value = (string)opcao.Value;

                if (configuracaoIntegracaoTecnologiaMonitoramentoOpcao.Codigo > 0)
                    repConfiguracaoIntegracaoTecnologiaMonitoramentoOpcao.Atualizar(configuracaoIntegracaoTecnologiaMonitoramentoOpcao, Auditado);
                else
                    repConfiguracaoIntegracaoTecnologiaMonitoramentoOpcao.Inserir(configuracaoIntegracaoTecnologiaMonitoramentoOpcao, Auditado);
            }
        }

        private void SalvarConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorar(bool adicionar, Dominio.Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramento configuracaoIntegracaoTecnologiaMonitoramento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorar repConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorar = new Repositorio.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorar(unitOfWork);

            List<Dominio.Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorar> configuracoesIntegracaoTecnologiaMonitoramentoMonitorar = new List<Dominio.Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorar>();

            if (!adicionar)
                configuracoesIntegracaoTecnologiaMonitoramentoMonitorar = repConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorar.BuscarPorConfiguracao(configuracaoIntegracaoTecnologiaMonitoramento);

            dynamic listaMonitorar = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Monitorar"));

            if (configuracoesIntegracaoTecnologiaMonitoramentoMonitorar.Count > 0)
            {
                List<long> codigos = new List<long>();

                foreach (dynamic opcao in listaMonitorar)
                {
                    if (opcao.Codigo != null)
                        codigos.Add((long)opcao.Codigo);
                }

                List<Dominio.Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorar> monitorarDeletar = configuracoesIntegracaoTecnologiaMonitoramentoMonitorar.FindAll(o => !codigos.Contains(o.Codigo));

                for (var i = 0; i < monitorarDeletar.Count; i++)
                    repConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorar.Deletar(monitorarDeletar[i], Auditado);
            }

            foreach (dynamic monitorar in listaMonitorar)
            {
                Dominio.Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorar configuracaoIntegracaoTecnologiaMonitoramentoMonitorar = null;
                long codigo = 0;

                if (monitorar.Codigo != null && long.TryParse((string)monitorar.Codigo, out codigo))
                    configuracaoIntegracaoTecnologiaMonitoramentoMonitorar = repConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorar.BuscarPorCodigo(codigo, true);

                if (configuracaoIntegracaoTecnologiaMonitoramentoMonitorar == null)
                {
                    configuracaoIntegracaoTecnologiaMonitoramentoMonitorar = new Dominio.Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorar
                    {
                        Configuracao = configuracaoIntegracaoTecnologiaMonitoramento
                    };
                }

                configuracaoIntegracaoTecnologiaMonitoramentoMonitorar.Key = (string)monitorar.Key;
                configuracaoIntegracaoTecnologiaMonitoramentoMonitorar.Value = (string)monitorar.Value;

                if (configuracaoIntegracaoTecnologiaMonitoramentoMonitorar.Codigo > 0)
                    repConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorar.Atualizar(configuracaoIntegracaoTecnologiaMonitoramentoMonitorar, Auditado);
                else
                    repConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorar.Inserir(configuracaoIntegracaoTecnologiaMonitoramentoMonitorar, Auditado);
            }
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Integracoes.FiltroPesquisaConfiguracaoIntegracaoTecnologiaMonitoramento filtroPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request) { header = new List<Models.Grid.Head>() };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Tipo", "Tipo", 60, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Habilitada", "Habilitada", 20, Models.Grid.Align.left, true);

                string propriedadeOrdenar = ObterPropriedadeOrdenar(grid.header[grid.indiceColunaOrdena].data);

                Repositorio.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramento repConfiguracaoIntegracaoTecnologiaMonitoramento = new Repositorio.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramento(unitOfWork);

                List<Dominio.Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramento> configuracoes = repConfiguracaoIntegracaoTecnologiaMonitoramento.Consultar(filtroPesquisa, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);

                int totalRegistros = repConfiguracaoIntegracaoTecnologiaMonitoramento.ContarConsulta(filtroPesquisa);

                var retorno = (from obj in configuracoes
                               select new
                               {
                                   obj.Codigo,
                                   Tipo = obj.Tipo.ObterDescricao(),
                                   Habilitada = obj.Habilitada.ObterDescricao()
                               }).ToList();

                grid.AdicionaRows(retorno);

                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracoes.FiltroPesquisaConfiguracaoIntegracaoTecnologiaMonitoramento ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Integracoes.FiltroPesquisaConfiguracaoIntegracaoTecnologiaMonitoramento()
            {
                Habilitada = Request.GetNullableBoolParam("Habilitada"),
                Tipo = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>("Tipo")
            };
        }

        private string ObterPropriedadeOrdenar(string propriedade)
        {
            return propriedade;
        }

        #endregion
    }
}
