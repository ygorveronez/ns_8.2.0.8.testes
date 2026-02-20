using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Newtonsoft.Json;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Financeiros.Despesa
{
    [CustomAuthorize("Financeiros/RateioDespesaVeiculo")]
    public class RateioDespesaVeiculoController : BaseController
    {
		#region Construtores

		public RateioDespesaVeiculoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Financeiro.Despesa.RateioDespesaVeiculo repRateioDespesaVeiculo = new Repositorio.Embarcador.Financeiro.Despesa.RateioDespesaVeiculo(unitOfWork);
                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
                Repositorio.Embarcador.Financeiro.MovimentoFinanceiro repMovimentoFinanceiro = new Repositorio.Embarcador.Financeiro.MovimentoFinanceiro(unitOfWork);
                Repositorio.Embarcador.Financeiro.ContratoFinanciamento repContratoFinanciamento = new Repositorio.Embarcador.Financeiro.ContratoFinanciamento(unitOfWork);
                Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS repDocumentoEntradaTMS = new Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS(unitOfWork);
                Repositorio.Embarcador.Financeiro.DocumentoEntradaItem repDocumentoEntradaItem = new Repositorio.Embarcador.Financeiro.DocumentoEntradaItem(unitOfWork);
                Repositorio.Embarcador.Frota.Infracao repInfracao = new Repositorio.Embarcador.Frota.Infracao(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculo rateioDespesaVeiculo = new Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculo();

                List<Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoValorVeiculo> listDespesaVeiculo = new List<Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoValorVeiculo>();

                PreencherEntidade(rateioDespesaVeiculo, unitOfWork);


                if (rateioDespesaVeiculo.SegmentosVeiculos?.Count > 0 && rateioDespesaVeiculo.Veiculos?.Count > 0)
                    return new JsonpResult(false, true, "Não é permitida a seleção de segmento e veículo no mesmo rateio.");

                if (rateioDespesaVeiculo.CentroResultados?.Count > 0 && rateioDespesaVeiculo.Veiculos?.Count > 0)
                    return new JsonpResult(false, true, "Não é permitida a seleção de centro de resultado e veículo no mesmo rateio.");

                if (rateioDespesaVeiculo.TipoDespesa == null)
                    return new JsonpResult(false, true, "O tipo de despesa é obrigatório.");

                if (rateioDespesaVeiculo.RatearDespesaUmaVezPorMes && (rateioDespesaVeiculo.DiaMesRateio <= 0 || rateioDespesaVeiculo.DiaMesRateio > 31))
                    return new JsonpResult(false, true, "Favor informe um dia válido para o lançamento do rateio.");

                int codigoTitulo = Request.GetIntParam("Titulo");
                int codigoMovimentoFinanceiro = Request.GetIntParam("MovimentoFinanceiro");
                int codigoContratoFinanciamento = Request.GetIntParam("ContratoFinanciamento");
                int codigoDocumentoEntradaTMS = Request.GetIntParam("DocumentoEntrada");
                int codigoInfracao = Request.GetIntParam("Infracao");

                bool multiplosTitulos = Request.GetBoolParam("MultiplosTitulos");

                rateioDespesaVeiculo.Titulo = codigoTitulo > 0 ? repTitulo.BuscarPorCodigo(codigoTitulo) : null;
                rateioDespesaVeiculo.MovimentoFinanceiro = codigoMovimentoFinanceiro > 0 ? repMovimentoFinanceiro.BuscarPorCodigo(codigoMovimentoFinanceiro) : null;
                rateioDespesaVeiculo.ContratoFinanciamento = codigoContratoFinanciamento > 0 ? repContratoFinanciamento.BuscarPorCodigo(codigoContratoFinanciamento) : null;
                rateioDespesaVeiculo.DocumentoEntradaTMS = codigoDocumentoEntradaTMS > 0 ? repDocumentoEntradaTMS.BuscarPorCodigo(codigoDocumentoEntradaTMS) : null;
                rateioDespesaVeiculo.Infracao = codigoInfracao > 0 ? repInfracao.BuscarPorCodigo(codigoInfracao) : null;

                if (rateioDespesaVeiculo.DocumentoEntradaTMS != null)
                {
                    decimal valorRateioItensDocumento = repDocumentoEntradaItem.ValorRateioDespesaVeiculo(rateioDespesaVeiculo.DocumentoEntradaTMS.Codigo);
                    if (Math.Round(valorRateioItensDocumento, 2) != Math.Round(rateioDespesaVeiculo.Valor, 2))
                        return new JsonpResult(false, true, "O valor da despesa não bate com o valor lançado nos itens do documento de entrada (" + valorRateioItensDocumento.ToString("n2") + ")");
                }

                unitOfWork.Start();

                rateioDespesaVeiculo.Origem = ObterOrigemRateioDespesaVeiculo(rateioDespesaVeiculo);

                repRateioDespesaVeiculo.Inserir(rateioDespesaVeiculo, Auditado);
                PreencherEntidadeDespesaVeiculo(rateioDespesaVeiculo, unitOfWork);
                PreencherEntidadeDespesaCentroResultado(rateioDespesaVeiculo, unitOfWork);


                Servicos.Embarcador.Financeiro.RateioDespesaVeiculo servicoRateioDespesaVeiculo = new Servicos.Embarcador.Financeiro.RateioDespesaVeiculo(unitOfWork);
                if (!servicoRateioDespesaVeiculo.RatearValorEntreVeiculos(out string erro, rateioDespesaVeiculo))
                    throw new ControllerException(erro);

                unitOfWork.CommitChanges();

                var dynRetorno = new
                {
                    AdicionadoAutomaticamente = codigoTitulo > 0 || codigoMovimentoFinanceiro > 0 || codigoContratoFinanciamento > 0 || codigoInfracao > 0 || codigoDocumentoEntradaTMS > 0 || multiplosTitulos
                };
                return new JsonpResult(dynRetorno);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
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

                Repositorio.Embarcador.Financeiro.Despesa.RateioDespesaVeiculo repRateioDespesaVeiculo = new Repositorio.Embarcador.Financeiro.Despesa.RateioDespesaVeiculo(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculo rateioDespesaVeiculo = repRateioDespesaVeiculo.BuscarPorCodigo(codigo, false);

                if (rateioDespesaVeiculo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    rateioDespesaVeiculo.Codigo,
                    DataInicial = rateioDespesaVeiculo.DataInicial.ToString("dd/MM/yyyy"),
                    DataFinal = rateioDespesaVeiculo.DataFinal.ToString("dd/MM/yyyy"),
                    rateioDespesaVeiculo.NumeroDocumento,
                    rateioDespesaVeiculo.RatearPeloPercentualFaturadoDoVeiculoNoPeriodo,
                    rateioDespesaVeiculo.RatearDespesaUmaVezPorMes,
                    rateioDespesaVeiculo.DiaMesRateio,
                    CentroResultadoRateio = new
                    {
                        Codigo = rateioDespesaVeiculo.CentroResultado?.Codigo ?? 0,
                        Descricao = rateioDespesaVeiculo.CentroResultado?.Descricao ?? string.Empty
                    },
                    TipoDespesa = new
                    {
                        Codigo = rateioDespesaVeiculo.TipoDespesa?.Codigo ?? 0,
                        Descricao = rateioDespesaVeiculo.TipoDespesa?.Descricao ?? string.Empty
                    },
                    Colaborador = new
                    {
                        Codigo = rateioDespesaVeiculo.Colaborador?.Codigo ?? 0,
                        Descricao = rateioDespesaVeiculo.Colaborador?.Nome ?? string.Empty,
                    },
                    Pessoa = new
                    {
                        Codigo = rateioDespesaVeiculo.Pessoa?.Codigo ?? 0,
                        Descricao = rateioDespesaVeiculo.Pessoa?.Nome ?? string.Empty,
                    },
                    rateioDespesaVeiculo.TipoDocumento,
                    Valor = rateioDespesaVeiculo.Valor.ToString("n2"),
                    SegmentosVeiculosDespesa = (from obj in rateioDespesaVeiculo.SegmentosVeiculos
                                                select new
                                                {
                                                    obj.Codigo,
                                                    obj.Descricao
                                                }).ToList(),
                    VeiculosDespesa = (from obj in rateioDespesaVeiculo.Veiculos
                                       select new
                                       {
                                           obj.Veiculo.Codigo,
                                           obj.Veiculo.Placa,
                                           ModeloVeicularCarga = obj.Veiculo.ModeloVeicularCarga?.Descricao ?? string.Empty,
                                           SegmentoVeiculo = obj.Veiculo.SegmentoVeiculo?.Descricao ?? string.Empty,
                                           NumeroFrota = obj.Veiculo.NumeroFrota,
                                           obj.Valor
                                       }).ToList(),
                    CentrosResultadoDespesa = (from obj in rateioDespesaVeiculo.CentroResultados
                                               select new
                                               {
                                                   obj.CentroResultado.Codigo,
                                                   obj.CentroResultado.Descricao,
                                                   obj.CentroResultado.Plano,
                                                   obj.Valor
                                               }).ToList()
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
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

                Repositorio.Embarcador.Financeiro.Despesa.RateioDespesaVeiculo repRateioDespesaVeiculo = new Repositorio.Embarcador.Financeiro.Despesa.RateioDespesaVeiculo(unitOfWork);
                Repositorio.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoLancamento repRateioDespesaVeiculoLancamento = new Repositorio.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoLancamento(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculo rateioDespesaVeiculo = repRateioDespesaVeiculo.BuscarPorCodigo(codigo, true);

                if (rateioDespesaVeiculo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                rateioDespesaVeiculo.Veiculos = null;
                rateioDespesaVeiculo.SegmentosVeiculos = null;
                rateioDespesaVeiculo.CentroResultados = null;

                repRateioDespesaVeiculoLancamento.DeletarPorRateioDespesaVeiculo(rateioDespesaVeiculo.Codigo);
                repRateioDespesaVeiculo.Deletar(rateioDespesaVeiculo, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AlterarValorRateioDespesaVeiculoLancamentoDia()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                long codigo = Request.GetLongParam("Codigo");

                decimal valor = Request.GetDecimalParam("Valor");

                Repositorio.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoLancamentoDia repRateioDespesaVeiculoLancamentoDia = new Repositorio.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoLancamentoDia(unitOfWork);
                Repositorio.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoLancamento repRateioDespesaVeiculoLancamento = new Repositorio.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoLancamento(unitOfWork);
                Repositorio.Embarcador.Financeiro.Despesa.RateioDespesaVeiculo repRateioDespesaVeiculo = new Repositorio.Embarcador.Financeiro.Despesa.RateioDespesaVeiculo(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoLancamentoDia rateioDespesaVeiculoLancamentoDia = repRateioDespesaVeiculoLancamentoDia.BuscarPorCodigo(codigo, true);

                if (rateioDespesaVeiculoLancamentoDia == null)
                    return new JsonpResult(false, true, "Registro não encontrado.");

                rateioDespesaVeiculoLancamentoDia.Lancamento.Initialize();
                rateioDespesaVeiculoLancamentoDia.Lancamento.RateioDespesa.Initialize();

                unitOfWork.Start();

                rateioDespesaVeiculoLancamentoDia.Valor = valor;

                repRateioDespesaVeiculoLancamentoDia.Atualizar(rateioDespesaVeiculoLancamentoDia, Auditado);

                rateioDespesaVeiculoLancamentoDia.Lancamento.Valor = repRateioDespesaVeiculoLancamentoDia.ObterTotalPorLancamento(rateioDespesaVeiculoLancamentoDia.Lancamento.Codigo);
                rateioDespesaVeiculoLancamentoDia.Lancamento.RateioDespesa.Valor = repRateioDespesaVeiculoLancamento.ObterTotalPorRateio(rateioDespesaVeiculoLancamentoDia.Lancamento.RateioDespesa.Codigo);

                repRateioDespesaVeiculo.Atualizar(rateioDespesaVeiculoLancamentoDia.Lancamento.RateioDespesa, Auditado);
                repRateioDespesaVeiculoLancamento.Atualizar(rateioDespesaVeiculoLancamentoDia.Lancamento, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(new
                {
                    Registro = ObterRegistroPesquisaDetalhadaLancamentoVeiculo(rateioDespesaVeiculoLancamentoDia),
                    ValorTotalRateio = rateioDespesaVeiculoLancamentoDia.Lancamento.RateioDespesa.Valor.ToString("n2")
                });
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao alterar o valor.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisa());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaLancamentoVeiculo()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisaLancamentoVeiculo());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaDetalhadaLancamentoVeiculo()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisaDetalhadaLancamentoVeiculo());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
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

                return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
        }

        [AllowAuthenticate]
        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> ExportarPesquisaLancamentoVeiculo()
        {
            try
            {
                Models.Grid.Grid grid = ObterGridPesquisaLancamentoVeiculo();

                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
        }

        [AllowAuthenticate]
        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> ExportarPesquisaDetalhadaLancamentoVeiculo()
        {
            try
            {
                Models.Grid.Grid grid = ObterGridPesquisaDetalhadaLancamentoVeiculo();

                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
        }

        public async Task<IActionResult> ConfiguracaoImportacao()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>();

            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = "Placa Veiculo", Propriedade = "PlacaVeiculo", Tamanho = 200, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 2, Descricao = "Valor", Propriedade = "Valor", Tamanho = 200, Obrigatorio = true, Regras = new List<string> { "required" } });

            return new JsonpResult(configuracoes.ToList());

        }

        public async Task<IActionResult> ObterDadosImportacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                string dados = Request.Params("Dados");

                List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha>>(dados);

                Servicos.Embarcador.Financeiro.RateioDespesaVeiculo servicoDespesaVeiculo = new Servicos.Embarcador.Financeiro.RateioDespesaVeiculo(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = servicoDespesaVeiculo.Importar(linhas, unitOfWork);

                return new JsonpResult(retornoImportacao);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao importar arquivo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConfiguracaoImportacaoCentroResultado()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>();

            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = "Centro de Resultado", Propriedade = "CentroResultado", Tamanho = 200, Obrigatorio = true, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 2, Descricao = "Número do Centro", Propriedade = "NumeroCentro", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 3, Descricao = "Valor", Propriedade = "Valor", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });

            return new JsonpResult(configuracoes.ToList());

        }

        public async Task<IActionResult> ObterDadosImportacaoCentroResultado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                string dados = Request.Params("Dados");

                List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha>>(dados);

                Servicos.Embarcador.Financeiro.RateioDespesaVeiculo servicoDespesaVeiculo = new Servicos.Embarcador.Financeiro.RateioDespesaVeiculo(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = servicoDespesaVeiculo.ImportarCentroResultado(linhas, unitOfWork);

                return new JsonpResult(retornoImportacao);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao importar arquivo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        #endregion

        #region Métodos Privados

        private dynamic ObterRegistroPesquisaDetalhadaLancamentoVeiculo(Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoLancamentoDia rateioDespesaVeiculoLancamentoDia)
        {
            return new
            {
                rateioDespesaVeiculoLancamentoDia.Codigo,
                Data = rateioDespesaVeiculoLancamentoDia.Data.ToString("dd/MM/yyyy"),
                Valor = rateioDespesaVeiculoLancamentoDia.Valor.ToString("n6"),
                rateioDespesaVeiculoLancamentoDia.Lancamento.Veiculo.Placa,
                SegmentoVeiculo = rateioDespesaVeiculoLancamentoDia.Lancamento.Veiculo.SegmentoVeiculo?.Descricao
            };
        }

        private Models.Grid.Grid ObterGridPesquisaDetalhadaLancamentoVeiculo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                long codigoLancamentoRateioDespesaVeiculo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoLancamento repRateioDespesaVeiculoLancamento = new Repositorio.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoLancamento(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoLancamento rateioDespesaVeiculoLancamento = repRateioDespesaVeiculoLancamento.BuscarPorCodigo(codigoLancamentoRateioDespesaVeiculo, false);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request) { header = new List<Models.Grid.Head>() };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Veículo", "Placa", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Segmento", "SegmentoVeiculo", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Dia", "Data", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Valor Rateado", "Valor", 12, Models.Grid.Align.left, true, true, false, false, true, new Models.Grid.EditableCell() { editable = true, type = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoColunaGrid.aDecimal, numberMask = new Models.Grid.NumberMask(6, true), maxlength = 15 });

                string propriedadeOrdenar = ObterPropriedadeOrdenar(grid.header[grid.indiceColunaOrdena].data);

                Repositorio.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoLancamentoDia repRateioDespesaVeiculoLancamentoDia = new Repositorio.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoLancamentoDia(unitOfWork);

                List<Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoLancamentoDia> listaRateioDespesaVeiculoLancamentoDia = repRateioDespesaVeiculoLancamentoDia.Consultar(codigoLancamentoRateioDespesaVeiculo, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);

                int totalRegistros = repRateioDespesaVeiculoLancamentoDia.ContarConsulta(codigoLancamentoRateioDespesaVeiculo);

                var retorno = listaRateioDespesaVeiculoLancamentoDia.Select(obj => ObterRegistroPesquisaDetalhadaLancamentoVeiculo(obj)).ToList();

                grid.AdicionaRows(retorno);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private Models.Grid.Grid ObterGridPesquisaLancamentoVeiculo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                long codigoRateioDespesaVeiculo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Financeiro.Despesa.RateioDespesaVeiculo repRateioDespesaVeiculo = new Repositorio.Embarcador.Financeiro.Despesa.RateioDespesaVeiculo(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculo rateioDespesaVeiculo = repRateioDespesaVeiculo.BuscarPorCodigo(codigoRateioDespesaVeiculo, false);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request) { header = new List<Models.Grid.Head>() };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Veículo", "Placa", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Segmento", "SegmentoVeiculo", 15, Models.Grid.Align.left, true);

                if (ConfiguracaoEmbarcador.UtilizarCentroResultadoNoRateioDespesaVeiculo)
                    grid.AdicionarCabecalho("Centro de Resultados", "CentroResultado", 15, Models.Grid.Align.left, true);

                if (rateioDespesaVeiculo.RatearPeloPercentualFaturadoDoVeiculoNoPeriodo)
                {
                    grid.AdicionarCabecalho("% Faturamento", "PercentualSobreFaturamentoTotal", 12, Models.Grid.Align.left, true);
                    grid.AdicionarCabecalho("Faturamento", "ValorFaturamento", 12, Models.Grid.Align.left, true);
                }

                grid.AdicionarCabecalho("Valor Rateado", "Valor", 12, Models.Grid.Align.left, true);

                string propriedadeOrdenar = ObterPropriedadeOrdenar(grid.header[grid.indiceColunaOrdena].data);

                Repositorio.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoLancamento repRateioDespesaVeiculoLancamento = new Repositorio.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoLancamento(unitOfWork);

                List<Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoLancamento> listaRateioDespesaVeiculoLancamento = repRateioDespesaVeiculoLancamento.Consultar(codigoRateioDespesaVeiculo, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);

                int totalRegistros = repRateioDespesaVeiculoLancamento.ContarConsulta(codigoRateioDespesaVeiculo);

                var retorno = (from obj in listaRateioDespesaVeiculoLancamento
                               select new
                               {
                                   obj.Codigo,
                                   ValorFaturamento = obj.ValorFaturamento.ToString("n2"),
                                   PercentualSobreFaturamentoTotal = obj.PercentualSobreFaturamentoTotal.ToString("n6"),
                                   Valor = obj.Valor.ToString("n6"),
                                   obj.Veiculo.Placa,
                                   SegmentoVeiculo = obj.Veiculo.SegmentoVeiculo?.Descricao,
                                   CentroResultado = obj.CentroResultado?.Descricao
                               }).ToList();

                grid.AdicionaRows(retorno);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRateioDespesaVeiculo filtroPesquisa = ObterFiltroPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request) { header = new List<Models.Grid.Head>() };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data Inicial", "DataInicial", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data Final", "DataFinal", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Nº Documento", "NumeroDocumento", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo Documento", "TipoDocumento", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo Despesa", "TipoDespesa", 25, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Valor", "Valor", 12, Models.Grid.Align.left, true);

                string propriedadeOrdenar = ObterPropriedadeOrdenar(grid.header[grid.indiceColunaOrdena].data);

                Repositorio.Embarcador.Financeiro.Despesa.RateioDespesaVeiculo repRateioDespesaVeiculo = new Repositorio.Embarcador.Financeiro.Despesa.RateioDespesaVeiculo(unitOfWork);

                List<Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculo> listaRateioDespesaVeiculo = repRateioDespesaVeiculo.Consultar(filtroPesquisa, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);

                int totalRegistros = repRateioDespesaVeiculo.ContarConsulta(filtroPesquisa);

                var retorno = (from rateioDespesaVeiculo in listaRateioDespesaVeiculo
                               select new
                               {
                                   rateioDespesaVeiculo.Codigo,
                                   DataInicial = rateioDespesaVeiculo.DataInicial.ToString("dd/MM/yyyy"),
                                   DataFinal = rateioDespesaVeiculo.DataFinal.ToString("dd/MM/yyyy"),
                                   rateioDespesaVeiculo.NumeroDocumento,
                                   rateioDespesaVeiculo.TipoDocumento,
                                   TipoDespesa = rateioDespesaVeiculo.TipoDespesa?.Descricao,
                                   Valor = rateioDespesaVeiculo.Valor.ToString("n2")
                               }).ToList();

                grid.AdicionaRows(retorno);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private new Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRateioDespesaVeiculo ObterFiltroPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRateioDespesaVeiculo()
            {
                CodigoTipoDespesa = Request.GetIntParam("TipoDespesa"),
                CodigoSegmentoVeiculo = Request.GetIntParam("SegmentoVeiculo"),
                CodigoVeiculo = Request.GetIntParam("Veiculo"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicial"),
                DataFinal = Request.GetNullableDateTimeParam("DataFinal"),
                NumeroDocumento = Request.GetStringParam("NumeroDocumento"),
                TipoDocumento = Request.GetStringParam("TipoDocumento"),
                RatearPeloPercentualFaturadoDoVeiculoNoPeriodo = Request.GetNullableBoolParam("RatearPeloPercentualFaturadoDoVeiculoNoPeriodo"),
                ValorInicial = Request.GetNullableDecimalParam("ValorInicial"),
                ValorFinal = Request.GetNullableDecimalParam("ValorFinal"),
                CodigoCentroResultado = Request.GetIntParam("CentroResultado"),
                CodigoColaborador = Request.GetIntParam("Colaborador"),
                CodigoPessoa = Request.GetDoubleParam("Pessoa"),
            };
        }

        private string ObterPropriedadeOrdenar(string propriedade)
        {
            return propriedade;
        }

        private void PreencherEntidade(Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculo rateioDespesaVeiculo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira repTipoDespesa = new Repositorio.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira(unitOfWork);
            Repositorio.Embarcador.Veiculos.SegmentoVeiculo repSegmentoVeiculo = new Repositorio.Embarcador.Veiculos.SegmentoVeiculo(unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unitOfWork);
            Repositorio.Usuario repColaborador = new Repositorio.Usuario(unitOfWork);
            Repositorio.Cliente repPessoa = new Repositorio.Cliente(unitOfWork);

            decimal valor = Request.GetDecimalParam("Valor");

            long codigoTipoDespesa = Request.GetLongParam("TipoDespesa");

            DateTime dataInicial = Request.GetDateTimeParam("DataInicial");
            DateTime dataFinal = Request.GetDateTimeParam("DataFinal");

            bool ratearPeloPercentualFaturadoVeiculo = Request.GetBoolParam("RatearPeloPercentualFaturadoVeiculo");
            bool ratearDespesaUmaVezPorMes = Request.GetBoolParam("RatearDespesaUmaVezPorMes");

            int diaMesRateio = Request.GetIntParam("DiaMesRateio");

            string numeroDocumento = Request.Params("NumeroDocumento");
            string tipoDocumento = Request.Params("TipoDocumento");

            int codigoCentroResultado = Request.GetIntParam("CentroResultadoRateio");
            int codigoColaborador = Request.GetIntParam("Colaborador");
            double codigoPessoa = Request.GetDoubleParam("Pessoa");

            List<int> segmentos = Request.GetListParam<int>("SegmentosVeiculosDespesa");


            rateioDespesaVeiculo.DataFinal = dataFinal;
            rateioDespesaVeiculo.DataInicial = dataInicial;
            rateioDespesaVeiculo.NumeroDocumento = numeroDocumento;
            rateioDespesaVeiculo.RatearPeloPercentualFaturadoDoVeiculoNoPeriodo = ratearPeloPercentualFaturadoVeiculo;
            rateioDespesaVeiculo.TipoDespesa = codigoTipoDespesa > 0 ? repTipoDespesa.BuscarPorCodigo(codigoTipoDespesa, false) : null;
            rateioDespesaVeiculo.TipoDocumento = tipoDocumento;
            rateioDespesaVeiculo.Valor = valor;
            rateioDespesaVeiculo.DiaMesRateio = diaMesRateio;
            rateioDespesaVeiculo.RatearDespesaUmaVezPorMes = ratearDespesaUmaVezPorMes;
            rateioDespesaVeiculo.Colaborador = codigoColaborador > 0 ? repColaborador.BuscarPorCodigo(codigoColaborador) : null;
            rateioDespesaVeiculo.Pessoa = codigoPessoa > 0 ? repPessoa.BuscarPorCPFCNPJ(codigoPessoa) : null;
            rateioDespesaVeiculo.CentroResultado = codigoCentroResultado > 0 ? repCentroResultado.BuscarPorCodigo(codigoCentroResultado) : null;

            if(rateioDespesaVeiculo.CentroResultado?.Veiculos.Count == 0)
                throw new ControllerException($"O centro de resultado {rateioDespesaVeiculo.CentroResultado?.Descricao} não possui veículos vinculados, não sendo possível realizar o rateio.");

            //Listas
            rateioDespesaVeiculo.SegmentosVeiculos = segmentos?.Count > 0 ? repSegmentoVeiculo.BuscarPorCodigo(segmentos) : null;
        }

        private OrigemRateioDespesaVeiculo ObterOrigemRateioDespesaVeiculo(Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculo rateioDespesaVeiculo)
        {
            if (rateioDespesaVeiculo.Titulo != null)
                return OrigemRateioDespesaVeiculo.Titulo;
            if (rateioDespesaVeiculo.MovimentoFinanceiro != null)
                return OrigemRateioDespesaVeiculo.MovimentoFinanceiro;
            if (rateioDespesaVeiculo.ContratoFinanciamento != null)
                return OrigemRateioDespesaVeiculo.ContratoFinanciamento;
            if (rateioDespesaVeiculo.DocumentoEntradaTMS != null)
                return OrigemRateioDespesaVeiculo.DocumentoEntrada;
            if (rateioDespesaVeiculo.Infracao != null)
                return OrigemRateioDespesaVeiculo.Infracao;

            return OrigemRateioDespesaVeiculo.Manual;
        }

        private void PreencherEntidadeDespesaVeiculo(Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculo rateioDespesaVeiculo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.Despesa.RateioDespesaVeiculo repRateioDespesaVeiculo = new Repositorio.Embarcador.Financeiro.Despesa.RateioDespesaVeiculo(unitOfWork);
            Repositorio.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoValorVeiculo repRateiValorVeiculo = new Repositorio.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoValorVeiculo(unitOfWork);
            dynamic veiculos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("VeiculosDespesa"));
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);

            if (rateioDespesaVeiculo.Veiculos != null && rateioDespesaVeiculo.Veiculos.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (dynamic veiculo in veiculos)
                    if (veiculo.Codigo != null)
                        codigos.Add((int)veiculo.Codigo);

                List<Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoValorVeiculo> listaDespesaVeiculoRemover = rateioDespesaVeiculo.Veiculos.Where(o => !codigos.Contains(o.Codigo)).ToList();

                for (var i = 0; i < listaDespesaVeiculoRemover.Count; i++)
                    rateioDespesaVeiculo.Veiculos.Remove(listaDespesaVeiculoRemover[i]);
            }

            if (rateioDespesaVeiculo.Veiculos == null || rateioDespesaVeiculo.Veiculos.Count == 0)
                rateioDespesaVeiculo.Veiculos = new List<Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoValorVeiculo>();

            foreach (var veiculo in veiculos)
            {
                if (rateioDespesaVeiculo.Veiculos.Any(o => o.Codigo == (int)veiculo.Codigo))
                    continue;

                Dominio.Entidades.Veiculo veiculoExistente = repVeiculo.BuscarPorCodigo((int)veiculo.Codigo, false);
                Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoValorVeiculo novoDespesaVeiculo = new Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoValorVeiculo()
                {
                    Veiculo = veiculoExistente,
                    DespesaVeiculo = rateioDespesaVeiculo,
                    Valor = !string.IsNullOrEmpty((string)veiculo.Valor) ? (decimal)veiculo.Valor : 0m 
                };
                repRateiValorVeiculo.Inserir(novoDespesaVeiculo, null);
            }
        }
        private void PreencherEntidadeDespesaCentroResultado(Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculo rateioDespesaVeiculo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.Despesa.RateioDespesaVeiculo repRateioDespesaVeiculo = new Repositorio.Embarcador.Financeiro.Despesa.RateioDespesaVeiculo(unitOfWork);
            Repositorio.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoValorCentroResultado repRateiValorCentroResultado = new Repositorio.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoValorCentroResultado(unitOfWork);
            dynamic centrosResultados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("CentrosResultadoDespesa"));
            Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unitOfWork);


            if (rateioDespesaVeiculo.CentroResultados != null && rateioDespesaVeiculo.CentroResultados.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (dynamic centrosResultado in centrosResultados)
                    if (centrosResultado.Codigo != null)
                        codigos.Add((int)centrosResultado.Codigo);

                List<Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoValorCentroResultado> listaDespesaVeiculoCentroResultadoRemover = rateioDespesaVeiculo.CentroResultados.Where(o => !codigos.Contains(o.Codigo)).ToList();

                for (var i = 0; i < listaDespesaVeiculoCentroResultadoRemover.Count; i++)
                    rateioDespesaVeiculo.CentroResultados.Remove(listaDespesaVeiculoCentroResultadoRemover[i]);
            }

            if (rateioDespesaVeiculo.CentroResultados == null || rateioDespesaVeiculo.CentroResultados.Count == 0)
                rateioDespesaVeiculo.CentroResultados = new List<Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoValorCentroResultado>();

            foreach (var centrosResultado in centrosResultados)
            {
                if (rateioDespesaVeiculo.CentroResultados.Any(o => o.Codigo == (int)centrosResultado.Codigo))
                    continue;

                Dominio.Entidades.Embarcador.Financeiro.CentroResultado CentroResultadoExistente = repCentroResultado.BuscarPorCodigo((int)centrosResultado.Codigo, false);
                
                Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoValorCentroResultado novoDespesaCentroResultado = new Dominio.Entidades.Embarcador.Financeiro.Despesa.RateioDespesaVeiculoValorCentroResultado()
                {
                    CentroResultado = CentroResultadoExistente,
                    DespesaVeiculo = rateioDespesaVeiculo,
                    Valor = !string.IsNullOrEmpty((string)centrosResultado.Valor) ?  (decimal)centrosResultado.Valor : 0m
                };
                repRateiValorCentroResultado.Inserir(novoDespesaCentroResultado);
            }
        }


        #endregion
    }
}
