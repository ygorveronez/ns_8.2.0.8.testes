using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.ProgramacaoCarga
{
    [CustomAuthorize(new string[] { "DownloadHistoricoCarga", "ObterResumoSugestoes" }, "Cargas/ProgramacaoCarga")]
    public class ProgramacaoCargaController : BaseController
    {
		#region Construtores

		public ProgramacaoCargaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> AdicionarSugestao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                DateTime? dataProgramacaoInicial = Request.GetNullableDateTimeParam("DataProgramacaoInicial");
                DateTime? dataProgramacaoFinal = Request.GetNullableDateTimeParam("DataProgramacaoFinal");

                if (!dataProgramacaoInicial.HasValue)
                    throw new ControllerException("Data de pré planejamento inicial deve ser informada.");

                if (!dataProgramacaoFinal.HasValue)
                    throw new ControllerException("Data de pré planejamento final deve ser informada.");

                List<DateTime> datasProgramacao = dataProgramacaoInicial.Value.DatesBetweenWithoutWeekend(dataProgramacaoFinal.Value);

                if (datasProgramacao.Count == 0)
                    throw new ControllerException($"Nenhum dia útil foi encontrado de {dataProgramacaoInicial.Value.ToDateString()} até {dataProgramacaoFinal.Value.ToDateString()}.");

                Repositorio.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCarga repositorioSugestaoProgramacaoCarga = new Repositorio.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCarga(unitOfWork);
                Servicos.Embarcador.Carga.ProgramacaoCarga.ProgramacaoCarga servicoProgramacaoCarga = new Servicos.Embarcador.Carga.ProgramacaoCarga.ProgramacaoCarga(unitOfWork, Auditado);
                Dominio.ObjetosDeValor.Embarcador.Carga.ProgramacaoCarga.SugestaoProgramacaoCarga sugestaoProgramacaoCargaBase = new Dominio.ObjetosDeValor.Embarcador.Carga.ProgramacaoCarga.SugestaoProgramacaoCarga()
                {
                    CodigoFilial = Request.GetIntParam("Filial"),
                    CodigoModeloVeicularCarga = Request.GetIntParam("ModeloVeicularCarga"),
                    CodigoTipoCarga = Request.GetIntParam("TipoCarga"),
                    CodigoTipoOperacao = Request.GetIntParam("TipoOperacao"),
                    CodigosDestinos = Request.GetListParam<int>("Destinos"),
                    CodigosRegioesDestino = Request.GetListParam<int>("RegioesDestino"),
                    Quantidade = Request.GetDecimalParam("Quantidade"),
                    QuantidadeValidada = Request.GetIntParam("Quantidade"),
                    SiglasEstadosDestino = Request.GetListParam<string>("EstadosDestino"),
                };
                int totalSugestoesAdicionadas = 0;

                foreach (DateTime dataProgramacao in datasProgramacao)
                {
                    Dominio.ObjetosDeValor.Embarcador.Carga.ProgramacaoCarga.SugestaoProgramacaoCarga sugestaoProgramacaoCarga = sugestaoProgramacaoCargaBase.Clonar();

                    sugestaoProgramacaoCarga.Data = dataProgramacao;

                    if (repositorioSugestaoProgramacaoCarga.ExistePorDataProgramacao(sugestaoProgramacaoCarga.CodigoFilial, sugestaoProgramacaoCarga.CodigoModeloVeicularCarga, sugestaoProgramacaoCarga.CodigoTipoCarga, sugestaoProgramacaoCarga.CodigoTipoOperacao, sugestaoProgramacaoCarga.Data))
                        continue;

                    servicoProgramacaoCarga.AdicionarSugestaoProgramacaoCarga(sugestaoProgramacaoCarga);
                    totalSugestoesAdicionadas++;
                }

                if (totalSugestoesAdicionadas == 0)
                    throw new ControllerException("Sugestões de pré planejamento já cadastradas para o período informado.");

                unitOfWork.CommitChanges();

                return new JsonpResult(new
                {
                    TotalSugestoesAdicionadas = totalSugestoesAdicionadas
                });
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
                return new JsonpResult(false, "Ocorreu uma falha ao gerar as sugestões de pré planejamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AlterarSugestao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigoSugestaoSelecionada = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCarga repositorioSugestaoProgramacaoCarga = new Repositorio.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCarga sugestaoProgramacaoCarga = repositorioSugestaoProgramacaoCarga.BuscarPorCodigo(codigoSugestaoSelecionada, auditavel: true);

                if (sugestaoProgramacaoCarga == null)
                    throw new ControllerException("Não foi possível encontrar o registro.");

                if (sugestaoProgramacaoCarga.Situacao != SituacaoSugestaoProgramacaoCarga.Gerada)
                    throw new ControllerException("A situação da sugestão de pré planejamento não permite o cancelamento.");

                sugestaoProgramacaoCarga.QuantidadeValidada = Request.GetIntParam("QuantidadeValidada");

                if (sugestaoProgramacaoCarga.QuantidadeValidada <= 0)
                    throw new ControllerException("A quantidade validada deve ser superior a zero.");

                repositorioSugestaoProgramacaoCarga.Atualizar(sugestaoProgramacaoCarga);
                Servicos.Auditoria.Auditoria.AuditarComAlteracoesRealizadas(Auditado, sugestaoProgramacaoCarga, sugestaoProgramacaoCarga.GetChanges(), "Alterada a quantidade validada.", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
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
                return new JsonpResult(false, "Ocorreu uma falha ao alterar a sugestão de pré planejamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> CancelarMultiplasSugestoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                var listaSugestoesSelecionadas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensSelecionados"));
                Repositorio.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCarga repositorioSugestaoProgramacaoCarga = new Repositorio.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCarga(unitOfWork);
                Servicos.Embarcador.Carga.ProgramacaoCarga.ProgramacaoCarga servicoProgramacaoCarga = new Servicos.Embarcador.Carga.ProgramacaoCarga.ProgramacaoCarga(unitOfWork, Auditado);
                int totalSugestoesCanceladas = 0;

                foreach (var sugestaoSelecionada in listaSugestoesSelecionadas)
                {
                    int codigoSugestaoSelecionada = (int)sugestaoSelecionada.Codigo;
                    Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCarga sugestaoProgramacaoCarga = repositorioSugestaoProgramacaoCarga.BuscarPorCodigo(codigoSugestaoSelecionada, auditavel: false);

                    if ((sugestaoProgramacaoCarga == null) || (sugestaoProgramacaoCarga.Situacao != SituacaoSugestaoProgramacaoCarga.Gerada))
                        continue;

                    servicoProgramacaoCarga.CancelarSugestaoProgramacaoCarga(sugestaoProgramacaoCarga);
                    totalSugestoesCanceladas++;
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(new
                {
                    TotalSugestoesCanceladas = totalSugestoesCanceladas
                });
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
                return new JsonpResult(false, "Ocorreu uma falha ao cancelar as sugestões de pré planejamento selecionadas.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> CancelarSugestao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigoSugestaoSelecionada = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCarga repositorioSugestaoProgramacaoCarga = new Repositorio.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCarga sugestaoProgramacaoCarga = repositorioSugestaoProgramacaoCarga.BuscarPorCodigo(codigoSugestaoSelecionada, auditavel: false);

                if (sugestaoProgramacaoCarga == null)
                    throw new ControllerException("Não foi possível encontrar o registro.");

                if (sugestaoProgramacaoCarga.Situacao != SituacaoSugestaoProgramacaoCarga.Gerada)
                    throw new ControllerException("A situação da sugestão de pré planejamento não permite o cancelamento.");

                new Servicos.Embarcador.Carga.ProgramacaoCarga.ProgramacaoCarga(unitOfWork, Auditado).CancelarSugestaoProgramacaoCarga(sugestaoProgramacaoCarga);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
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
                return new JsonpResult(false, "Ocorreu uma falha ao cancelar a sugestão de pré planejamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DownloadHistoricoCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid()
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Número da Carga", "CodigoCargaEmbarcador", 15, Models.Grid.Align.center);
                grid.AdicionarCabecalho("Filial", "Filial", 15, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Cidades de Destino", "CidadesDestino", 15, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Estados de Destino", "EstadosDestino", 15, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Regiões de Destino", "RegioesDestino", 15, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Data de Criação", "DataCriacaoCargaFormatada", 10, Models.Grid.Align.center);
                grid.AdicionarCabecalho("Data de Finalização da Emissão", "DataFinalizacaoEmissaoFormatada", 10, Models.Grid.Align.center);
                grid.AdicionarCabecalho("Transportador", "Transportador", 15, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Veículo", "Veiculo", 10, Models.Grid.Align.center);
                grid.AdicionarCabecalho("Tipo de Carga", "TipoCarga", 15, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Tipo de Operação", "TipoOperacao", 15, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Modelo Veicular de Carga", "ModeloVeicularCarga", 15, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Peso", "PesoFormatado", 10, Models.Grid.Align.left);

                int codigoSugestaoProgramacaoCarga = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCarga repositorioSugestaoProgramacaoCarga = new Repositorio.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCarga sugestaoProgramacaoCarga = repositorioSugestaoProgramacaoCarga.BuscarPorCodigo(codigoSugestaoProgramacaoCarga, auditavel: false);

                if (sugestaoProgramacaoCarga == null)
                    throw new ControllerException("Não foi possível encontrar o registro");

                Repositorio.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCargaHistoricoCarga repositorioHistoricoCarga = new Repositorio.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCargaHistoricoCarga(unitOfWork);
                IList<Dominio.ObjetosDeValor.Embarcador.Carga.ProgramacaoCarga.SugestaoProgramacaoCargaHistoricoCarga> historicos = repositorioHistoricoCarga.BuscarPorSugestaoProgramacaoCarga(codigoSugestaoProgramacaoCarga);

                grid.AdicionaRows(historicos);
                grid.setarQuantidadeTotal(historicos.Count);

                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"Sugestao_dia_{sugestaoProgramacaoCarga.DataProgramacao:dd_MM_yyyy}.{grid.extensaoCSV}");

                return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (BaseException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
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

        public async Task<IActionResult> GerarSugestoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Dominio.ObjetosDeValor.Embarcador.Carga.ProgramacaoCarga.ParametroSugestaoProgramacaoCarga parametroSugestaoProgramacaoCarga = new Dominio.ObjetosDeValor.Embarcador.Carga.ProgramacaoCarga.ParametroSugestaoProgramacaoCarga()
                {
                    CodigoConfiguracaoProgramacaoCarga = Request.GetIntParam("ConfiguracaoProgramacaoCarga"),
                    DataHistoricoFinal = Request.GetDateTimeParam("DataHistoricoFinal"),
                    DataHistoricoInicial = Request.GetDateTimeParam("DataHistoricoInicial"),
                    DataProgramacaoFinal = Request.GetDateTimeParam("DataProgramacaoFinal"),
                    DataProgramacaoInicial = Request.GetDateTimeParam("DataProgramacaoInicial")
                };
                
                new Servicos.Embarcador.Carga.ProgramacaoCarga.ProgramacaoCarga(unitOfWork, Auditado).GerarSugestoesProgramacaoCarga(parametroSugestaoProgramacaoCarga);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
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
                return new JsonpResult(false, "Ocorreu uma falha ao gerar as sugestões de pré planejamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterResumoSugestoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Carga.ProgramacaoCarga.FiltroPesquisaSugestaoProgramacaoCarga filtrosPesquisa = ObterFiltrosPesquisa();
                Repositorio.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCarga repositorioSugestaoProgramacaoCarga = new Repositorio.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCarga(unitOfWork);
                List<(int Codigo, string ModeloveicularCarga, string TipoCarga, decimal QuantidadeSugerida, int QuantidadeValidada)> listaResumo = repositorioSugestaoProgramacaoCarga.BuscarParaResumo(filtrosPesquisa);
                List<dynamic> resumosRetornar = new List<dynamic>();
                decimal quantidadeSugeridaTotal = 0m;
                int quantidadeValidadaTotal = 0;

                if (listaResumo.Count > 0)
                {
                    Repositorio.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCargaDestino repositorioSugestaoProgramacaoCargaDestino = new Repositorio.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCargaDestino(unitOfWork);
                    Repositorio.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCargaEstadoDestino repositorioSugestaoProgramacaoCargaEstadoDestino = new Repositorio.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCargaEstadoDestino(unitOfWork);
                    Repositorio.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCargaRegiaoDestino repositorioSugestaoProgramacaoCargaRegiaoDestino = new Repositorio.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCargaRegiaoDestino(unitOfWork);
                    List<int> codigosSugestoes = listaResumo.Select(resumo => resumo.Codigo).ToList();
                    List<(int CodigoSugestaoProgramacaoCarga, string Destino)> destinos = repositorioSugestaoProgramacaoCargaDestino.BuscarPorSugestoesProgramacaoCarga(codigosSugestoes);
                    List<(int CodigoSugestaoProgramacaoCarga, string Estado)> estadosDestino = repositorioSugestaoProgramacaoCargaEstadoDestino.BuscarPorSugestoesProgramacaoCarga(codigosSugestoes);
                    List<(int CodigoSugestaoProgramacaoCarga, string Regiao)> regioesDestino = repositorioSugestaoProgramacaoCargaRegiaoDestino.BuscarPorSugestoesProgramacaoCarga(codigosSugestoes);
                    List<string> modelosVeicularesCarga = listaResumo.Select(resumo => resumo.ModeloveicularCarga).Distinct().ToList();

                    foreach (string modeloVeicularCarga in modelosVeicularesCarga)
                    {
                        List<(int Codigo, string ModeloveicularCarga, string TipoCarga, decimal QuantidadeSugerida, int QuantidadeValidada)> listaResumoPorModeloVeicularCarga = listaResumo.Where(resumo => resumo.ModeloveicularCarga == modeloVeicularCarga).ToList();
                        List<string> tiposCarga = listaResumoPorModeloVeicularCarga.Select(resumo => resumo.TipoCarga).Distinct().ToList();

                        foreach (string tipoCarga in tiposCarga)
                        {
                            List<(int Codigo, string ModeloveicularCarga, string TipoCarga, decimal QuantidadeSugerida, int QuantidadeValidada)> listaResumoPorTipoCarga = listaResumoPorModeloVeicularCarga.Where(resumo => resumo.TipoCarga == tipoCarga).ToList();
                            Dictionary<string, List<(decimal QuantidadeSugerida, int Quantidadevalidada)>> listaResumoPorDestinoNaoinformado = new Dictionary<string, List<(decimal QuantidadeSugerida, int Quantidadevalidada)>>();
                            Dictionary<string, List<(decimal QuantidadeSugerida, int Quantidadevalidada)>> listaResumoPorCidadeDestino = new Dictionary<string, List<(decimal QuantidadeSugerida, int Quantidadevalidada)>>();
                            Dictionary<string, List<(decimal QuantidadeSugerida, int Quantidadevalidada)>> listaResumoPorEstadoDestino = new Dictionary<string, List<(decimal QuantidadeSugerida, int Quantidadevalidada)>>();
                            Dictionary<string, List<(decimal QuantidadeSugerida, int Quantidadevalidada)>> listaResumoPorRegiaoDestino = new Dictionary<string, List<(decimal QuantidadeSugerida, int Quantidadevalidada)>>();

                            foreach ((int Codigo, string ModeloveicularCarga, string TipoCarga, decimal QuantidadeSugerida, int QuantidadeValidada) resumo in listaResumoPorTipoCarga)
                            {
                                string cidadeDestino = string.Join(", ", destinos.Where(destino => destino.CodigoSugestaoProgramacaoCarga == resumo.Codigo).OrderBy(destino => destino.Destino).Select(destino => destino.Destino.Trim()));
                                string estadoDestino = string.Join(", ", estadosDestino.Where(estado => estado.CodigoSugestaoProgramacaoCarga == resumo.Codigo).OrderBy(estado => estado.Estado).Select(estado => estado.Estado.Trim()));
                                string regiaoDestino = string.Join(", ", regioesDestino.Where(regiao => regiao.CodigoSugestaoProgramacaoCarga == resumo.Codigo).OrderBy(regiao => regiao.Regiao).Select(regiao => regiao.Regiao.Trim()));

                                if (!string.IsNullOrWhiteSpace(cidadeDestino))
                                {
                                    if (listaResumoPorCidadeDestino.Keys.Contains(cidadeDestino))
                                        listaResumoPorCidadeDestino[cidadeDestino].Add(ValueTuple.Create(resumo.QuantidadeSugerida, resumo.QuantidadeValidada));
                                    else
                                        listaResumoPorCidadeDestino.Add(cidadeDestino, new List<(decimal QuantidadeSugerida, int Quantidadevalidada)>() { ValueTuple.Create(resumo.QuantidadeSugerida, resumo.QuantidadeValidada) });
                                }
                                else if (!string.IsNullOrWhiteSpace(estadoDestino))
                                {
                                    if (listaResumoPorEstadoDestino.Keys.Contains(estadoDestino))
                                        listaResumoPorEstadoDestino[estadoDestino].Add(ValueTuple.Create(resumo.QuantidadeSugerida, resumo.QuantidadeValidada));
                                    else
                                        listaResumoPorEstadoDestino.Add(estadoDestino, new List<(decimal QuantidadeSugerida, int Quantidadevalidada)>() { ValueTuple.Create(resumo.QuantidadeSugerida, resumo.QuantidadeValidada) });
                                }
                                else if (!string.IsNullOrWhiteSpace(regiaoDestino))
                                {
                                    if (listaResumoPorRegiaoDestino.Keys.Contains(regiaoDestino))
                                        listaResumoPorRegiaoDestino[regiaoDestino].Add(ValueTuple.Create(resumo.QuantidadeSugerida, resumo.QuantidadeValidada));
                                    else
                                        listaResumoPorRegiaoDestino.Add(regiaoDestino, new List<(decimal QuantidadeSugerida, int Quantidadevalidada)>() { ValueTuple.Create(resumo.QuantidadeSugerida, resumo.QuantidadeValidada) });
                                }
                                else
                                {
                                    if (listaResumoPorDestinoNaoinformado.Keys.Contains(""))
                                        listaResumoPorDestinoNaoinformado[""].Add(ValueTuple.Create(resumo.QuantidadeSugerida, resumo.QuantidadeValidada));
                                    else
                                        listaResumoPorDestinoNaoinformado.Add("", new List<(decimal QuantidadeSugerida, int Quantidadevalidada)>() { ValueTuple.Create(resumo.QuantidadeSugerida, resumo.QuantidadeValidada) });
                                }
                            }

                            IEnumerable<KeyValuePair<string, List<(decimal QuantidadeSugerida, int Quantidadevalidada)>>> listaResumoPorDestino = listaResumoPorDestinoNaoinformado
                                .Concat(listaResumoPorCidadeDestino)
                                .Concat(listaResumoPorEstadoDestino)
                                .Concat(listaResumoPorRegiaoDestino);

                            foreach (KeyValuePair<string, List<(decimal QuantidadeSugerida, int Quantidadevalidada)>> resumoPorDestino in listaResumoPorDestino)
                            {
                                decimal quantidadeSugerida = resumoPorDestino.Value.Sum(quantidade => quantidade.QuantidadeSugerida);
                                int quantidadeValidada = resumoPorDestino.Value.Sum(quantidade => quantidade.Quantidadevalidada);

                                quantidadeSugeridaTotal += quantidadeSugerida;
                                quantidadeValidadaTotal += quantidadeValidada;

                                resumosRetornar.Add(new
                                {
                                    ModeloVeicular = modeloVeicularCarga,
                                    TipoCarga = tipoCarga,
                                    Destino = resumoPorDestino.Key,
                                    QuantidadeSugerida = quantidadeSugerida.ToString("n2"),
                                    QuantidadeValidada = quantidadeValidada.ToString("n0")
                                });
                            }
                        }
                    }
                }

                return new JsonpResult(new {
                    ListaResumida = resumosRetornar,
                    TotalSugerido = quantidadeSugeridaTotal.ToString("n2"),
                    TotalValidado = quantidadeValidadaTotal.ToString("n0")
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao obter o resumo das sugestões de pré planejamento.");
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

        public async Task<IActionResult> PublicarMultiplasSugestoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                var listaSugestoesSelecionadas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensSelecionados"));
                Repositorio.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCarga repositorioSugestaoProgramacaoCarga = new Repositorio.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCarga(unitOfWork);
                Servicos.Embarcador.Carga.ProgramacaoCarga.ProgramacaoCarga servicoProgramacaoCarga = new Servicos.Embarcador.Carga.ProgramacaoCarga.ProgramacaoCarga(unitOfWork, Auditado);
                int totalSugestoesPublicadas = 0;

                foreach (var sugestaoSelecionada in listaSugestoesSelecionadas)
                {
                    int codigoSugestaoSelecionada = (int)sugestaoSelecionada.Codigo;
                    Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCarga sugestaoProgramacaoCarga = repositorioSugestaoProgramacaoCarga.BuscarPorCodigo(codigoSugestaoSelecionada, auditavel: false);

                    if ((sugestaoProgramacaoCarga == null) || (sugestaoProgramacaoCarga.Situacao != SituacaoSugestaoProgramacaoCarga.Gerada))
                        continue;

                    servicoProgramacaoCarga.PublicarSugestaoProgramacaoCarga(sugestaoProgramacaoCarga, TipoServicoMultisoftware);
                    totalSugestoesPublicadas++;
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(new
                {
                    TotalSugestoesPublicadas = totalSugestoesPublicadas
                });
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
                return new JsonpResult(false, "Ocorreu uma falha ao publicar as sugestões de pré planejamento selecionadas.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> PublicarSugestao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigoSugestaoSelecionada = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCarga repositorioSugestaoProgramacaoCarga = new Repositorio.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCarga sugestaoProgramacaoCarga = repositorioSugestaoProgramacaoCarga.BuscarPorCodigo(codigoSugestaoSelecionada, auditavel: false);

                if (sugestaoProgramacaoCarga == null)
                    throw new ControllerException("Não foi possível encontrar o registro.");

                if (sugestaoProgramacaoCarga.Situacao != SituacaoSugestaoProgramacaoCarga.Gerada)
                    throw new ControllerException("A situação da sugestão de pré planejamento não permite a publicação.");

                new Servicos.Embarcador.Carga.ProgramacaoCarga.ProgramacaoCarga(unitOfWork, Auditado).PublicarSugestaoProgramacaoCarga(sugestaoProgramacaoCarga, TipoServicoMultisoftware);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
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
                return new JsonpResult(false, "Ocorreu uma falha ao publicar a sugestão de pré planejamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion Métodos Globais

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Carga.ProgramacaoCarga.FiltroPesquisaSugestaoProgramacaoCarga ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Carga.ProgramacaoCarga.FiltroPesquisaSugestaoProgramacaoCarga()
            {
                CodigoConfiguracaoProgramacaoCarga = Request.GetIntParam("ConfiguracaoProgramacaoCarga"),
                CodigosFilial = Request.GetListParam<int>("Filial"),
                CodigosModeloVeicularCarga = Request.GetListParam<int>("ModeloVeicularCarga"),
                CodigosTipoCarga = Request.GetListParam<int>("TipoCarga"),
                CodigosTipoOperacao = Request.GetListParam<int>("TipoOperacao"),
                DataProgramacaoFinal = Request.GetNullableDateTimeParam("DataProgramacaoFinal"),
                DataProgramacaoInicial = Request.GetNullableDateTimeParam("DataProgramacaoInicial"),
                Situacao = Request.GetListEnumParam<SituacaoSugestaoProgramacaoCarga>("Situacao")
            };
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho(propriedade: "Codigo", visivel: false);
                grid.AdicionarCabecalho(propriedade: "Situacao", visivel: false);
                grid.AdicionarCabecalho(descricao: "Data", propriedade: "DataProgramacao", tamanho: 8, alinhamento: Models.Grid.Align.center, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Configuração de Pré Planejamento", propriedade: "ConfiguracaoProgramacaoCarga", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Cidades de Destino", propriedade: "Destinos", tamanho: 20, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                grid.AdicionarCabecalho(descricao: "Estados de Destino", propriedade: "EstadosDestino", tamanho: 20, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                grid.AdicionarCabecalho(descricao: "Regiões de Destino", propriedade: "RegioesDestino", tamanho: 20, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                grid.AdicionarCabecalho(descricao: "Filial", propriedade: "Filial", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Tipo de Carga", propriedade: "TipoCarga", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Tipo de Operação", propriedade: "TipoOperacao", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Modelo Veicular de Carga", propriedade: "ModeloVeicularCarga", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Situação", propriedade: "DescricaoSituacao", tamanho: 10, alinhamento: Models.Grid.Align.center, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Quantidade Sugerida", propriedade: "Quantidade", tamanho: 10, alinhamento: Models.Grid.Align.center, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Quantidade Validada", propriedade: "QuantidadeValidada", tamanho: 10, alinhamento: Models.Grid.Align.center, permiteOrdenacao: true).Editable(new Models.Grid.EditableCell(TipoColunaGrid.aInt, 9));

                Dominio.ObjetosDeValor.Embarcador.Carga.ProgramacaoCarga.FiltroPesquisaSugestaoProgramacaoCarga filtrosPesquisa = ObterFiltrosPesquisa();
                Repositorio.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCarga repositorioSugestaoProgramacaoCarga = new Repositorio.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCarga(unitOfWork);
                int totalRegistros = repositorioSugestaoProgramacaoCarga.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCarga> listaSugestao = new List<Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCarga>();
                List<(int CodigoSugestaoProgramacaoCarga, string Destino)> listaDestino = new List<(int CodigoSugestaoProgramacaoCarga, string Destino)>();
                List<(int CodigoSugestaoProgramacaoCarga, string Estado)> listaEstadoDestino = new List<(int CodigoSugestaoProgramacaoCarga, string Estado)>();
                List<(int CodigoSugestaoProgramacaoCarga, string Regiao)> listaRegiaoDestino = new List<(int CodigoSugestaoProgramacaoCarga, string Regiao)>();

                if (totalRegistros > 0)
                {
                    Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenarOuAgrupar);
                    Repositorio.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCargaDestino repositorioDestino = new Repositorio.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCargaDestino(unitOfWork);
                    Repositorio.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCargaEstadoDestino repositorioEstadoDestino = new Repositorio.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCargaEstadoDestino(unitOfWork);
                    Repositorio.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCargaRegiaoDestino repositorioRegiaoDestino = new Repositorio.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCargaRegiaoDestino(unitOfWork);

                    listaSugestao = repositorioSugestaoProgramacaoCarga.Consultar(filtrosPesquisa, parametrosConsulta);
                    List<int> codigosSugestaoProgramacaoCarga = listaSugestao.Select(sugestao => sugestao.Codigo).ToList();
                    listaDestino = repositorioDestino.BuscarPorSugestoesProgramacaoCarga(codigosSugestaoProgramacaoCarga);
                    listaEstadoDestino = repositorioEstadoDestino.BuscarPorSugestoesProgramacaoCarga(codigosSugestaoProgramacaoCarga);
                    listaRegiaoDestino = repositorioRegiaoDestino.BuscarPorSugestoesProgramacaoCarga(codigosSugestaoProgramacaoCarga);
                }

                var listaSugestaoRetornar = (
                    from sugestao in listaSugestao
                    select new
                    {
                        sugestao.Codigo,
                        sugestao.Situacao,
                        DataProgramacao = sugestao.DataProgramacao.ToDateString(),
                        ConfiguracaoProgramacaoCarga = sugestao.ConfiguracaoProgramacaoCarga?.Descricao ?? "",
                        Filial = sugestao.Filial?.Descricao ?? "",
                        Destinos = string.Join(", ", listaDestino.Where(destino => destino.CodigoSugestaoProgramacaoCarga == sugestao.Codigo).Select(destino => destino.Destino.Trim())),
                        EstadosDestino = string.Join(", ", listaEstadoDestino.Where(estadoDestino => estadoDestino.CodigoSugestaoProgramacaoCarga == sugestao.Codigo).Select(estadoDestino => estadoDestino.Estado.Trim())),
                        RegioesDestino = string.Join(", ", listaRegiaoDestino.Where(regiaoDestino => regiaoDestino.CodigoSugestaoProgramacaoCarga == sugestao.Codigo).Select(regiaoDestino => regiaoDestino.Regiao.Trim())),
                        TipoCarga = sugestao.TipoCarga?.Descricao ?? "",
                        TipoOperacao = sugestao.TipoOperacao?.Descricao ?? "",
                        ModeloVeicularCarga = sugestao.ModeloVeicularCarga?.Descricao ?? "",
                        DescricaoSituacao = sugestao.Situacao.ObterDescricao(),
                        Quantidade = sugestao.Quantidade.ToString("n2"),
                        QuantidadeValidada = sugestao.QuantidadeValidada.ToString("n0"),
                        DT_FontColor = sugestao.Situacao.ObterCorFonte(),
                        DT_RowColor = sugestao.Situacao.ObterCorLinha()
                    }
                ).ToList();

                grid.AdicionaRows(listaSugestaoRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
        }

        private string ObterPropriedadeOrdenarOuAgrupar(string propriedade)
        {
            if (propriedade == "DescricaoSituacao")
                return "Situacao";

            return propriedade;
        }

        #endregion Métodos Privados
    }
}
