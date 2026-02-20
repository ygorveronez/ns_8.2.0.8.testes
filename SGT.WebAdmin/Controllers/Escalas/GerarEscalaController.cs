using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;

namespace SGT.WebAdmin.Controllers.Escalas.GerarEscala
{
    [CustomAuthorize("Escalas/GerarEscala")]
    public class GerarEscalaController : BaseController
    {
		#region Construtores

		public GerarEscalaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> AdicionarEscala()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                DateTime dataEscala = Request.GetDateTimeParam("DataEscala");
                Repositorio.Embarcador.Escalas.GeracaoEscala repositorioGeracaoEscala = new Repositorio.Embarcador.Escalas.GeracaoEscala(unitOfWork);
                Dominio.Entidades.Embarcador.Escalas.GeracaoEscala geracaoEscalaDuplicada = repositorioGeracaoEscala.BuscarPorData(dataEscala);

                if (geracaoEscalaDuplicada != null)
                    throw new ControllerException($"Já existe uma escala para o dia {dataEscala.ToString("dd/MM/yyyy")}");

                Dominio.Entidades.Embarcador.Escalas.GeracaoEscala geracaoEscala = new Dominio.Entidades.Embarcador.Escalas.GeracaoEscala()
                {
                    Criador = this.Usuario,
                    DataEscala = Request.GetDateTimeParam("DataEscala"),
                    DataGerada = DateTime.Now,
                    NumeroEscala = repositorioGeracaoEscala.ObterProximoNumero(),
                    GerarParaTodosOsCentros = true,
                    Observacao = Request.GetStringParam("Observacao"),
                    SituacaoEscala = SituacaoEscala.EmCriacao
                };

                repositorioGeracaoEscala.Inserir(geracaoEscala, Auditado);

                SalvarProdutos(geracaoEscala, unitOfWork);
                SalvarRestricoesModeloVeicular(geracaoEscala, unitOfWork);
                GerarExpedicaoEscala(geracaoEscala, unitOfWork);

                repositorioGeracaoEscala.Atualizar(geracaoEscala);

                unitOfWork.CommitChanges();

                return new JsonpResult(geracaoEscala.Codigo);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao criar a escala.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AtualizarExpedicao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Escalas.GeracaoEscala repositorioGeracaoEscala = new Repositorio.Embarcador.Escalas.GeracaoEscala(unitOfWork);
                Dominio.Entidades.Embarcador.Escalas.GeracaoEscala geracaoEscala = repositorioGeracaoEscala.BuscarPorCodigo(codigo, true);

                if (geracaoEscala == null)
                    throw new ControllerException("Não foi possível encontrar o registro");

                if (geracaoEscala.SituacaoEscala != SituacaoEscala.EmCriacao)
                    throw new ControllerException("A atual situção da geração de escala não permite atualizar a expedição");

                AtualizarExpedicoesEscala(geracaoEscala, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(ObterExpedicoesEscala(geracaoEscala, unitOfWork));
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar a expedição.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AtualizarVeiculoEscala()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Escalas.GeracaoEscala repositorioGeracaoEscala = new Repositorio.Embarcador.Escalas.GeracaoEscala(unitOfWork);
                Dominio.Entidades.Embarcador.Escalas.GeracaoEscala geracaoEscala = repositorioGeracaoEscala.BuscarPorCodigo(codigo, true);

                if (geracaoEscala == null)
                    throw new ControllerException("Não foi possível encontrar o registro");

                if (geracaoEscala.SituacaoEscala != SituacaoEscala.AgVeiculos)
                    throw new ControllerException("A atual situção da geração de escala não permite a atualizar dos veículos da escala");

                AtualizarOrigensDestinosEscala(geracaoEscala, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(ObterOrigensDestinosEscala(geracaoEscala, unitOfWork));
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar os veículos da escala.");
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
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Escalas.GeracaoEscala repositorioGeracaoEscala = new Repositorio.Embarcador.Escalas.GeracaoEscala(unitOfWork);
                Dominio.Entidades.Embarcador.Escalas.GeracaoEscala geracaoEscala = repositorioGeracaoEscala.BuscarPorCodigo(codigo);

                if (geracaoEscala == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                var dadosEscalaRetornar = new
                {
                    geracaoEscala.Codigo,
                    geracaoEscala.NumeroEscala,
                    geracaoEscala.SituacaoEscala,
                    DadosEscala = new
                    {
                        geracaoEscala.Codigo,
                        DataEscala = geracaoEscala.DataEscala.ToString("dd/MM/yyyy"),
                        geracaoEscala.Observacao,
                        Produtos = (
                            from produto in geracaoEscala.Produtos
                            select new
                            {
                                produto.Codigo,
                                produto.Descricao
                            }
                        ).ToList(),
                        ModelosRestricaoRodagem = (
                            from restricaoRodagem in geracaoEscala.GeracaoEscalaRestricoesModeloVeicular
                            select new
                            {
                                restricaoRodagem.Codigo,
                                CodigoModeloVeicularCarga = restricaoRodagem.ModeloVeicularCarga.Codigo,
                                DescricaoModeloVeicularCarga = restricaoRodagem.ModeloVeicularCarga.Descricao,
                                HoraInicioRestricao = restricaoRodagem.HoraInicioRestricao.ToString(@"hh\:mm"),
                                HoraFimRestricao = restricaoRodagem.HoraFimRestricao.ToString(@"hh\:mm"),
                                HorarioRestricao = $"{restricaoRodagem.HoraInicioRestricao.ToString(@"hh\:mm")} até {restricaoRodagem.HoraFimRestricao.ToString(@"hh\:mm")}"
                            }
                        ).ToList()
                    },
                    ExpedicoesEscala = ObterExpedicoesEscala(geracaoEscala, unitOfWork),
                    OrigensDestinosEscala = ObterOrigensDestinosEscala(geracaoEscala, unitOfWork)
                };

                return new JsonpResult(dadosEscalaRetornar);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarProdutosPorDataEscala()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                DateTime dataEscala = Request.GetDateTimeParam("DataEscala");
                DiaSemana diaSemana = DiaSemanaHelper.ObterDiaSemana(dataEscala);
                Repositorio.Embarcador.Logistica.ExpedicaoCarregamento repositorioExpedicaoCarregamento = new Repositorio.Embarcador.Logistica.ExpedicaoCarregamento(unitOfWork);
                List<Dominio.Entidades.Embarcador.Logistica.ExpedicaoCarregamento> expedicoesCarregamento = repositorioExpedicaoCarregamento.BuscarPorDiaSemana(diaSemana);
                List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador> produtos = expedicoesCarregamento.Select(o => o.ProdutoEmbarcador).Distinct().ToList();

                var produtosRetornar = (
                    from o in produtos
                    select new
                    {
                        o.Codigo,
                        o.Descricao
                    }
                ).ToList();

                return new JsonpResult(produtosRetornar);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os produtos por data da escala.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarVeiculosEmEscala()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Escalas.GeracaoEscala repositorioGeracaoEscala = new Repositorio.Embarcador.Escalas.GeracaoEscala(unitOfWork);
                Dominio.Entidades.Embarcador.Escalas.GeracaoEscala geracaoEscala = repositorioGeracaoEscala.BuscarPorCodigo(codigo);

                if (geracaoEscala == null)
                    return new JsonpResult(false, "Não foi possível encontrar o registro.");

                Repositorio.Embarcador.Escalas.EscalaVeiculoEscalado repositorioEscalaVeiculoEscalado = new Repositorio.Embarcador.Escalas.EscalaVeiculoEscalado(unitOfWork);
                Repositorio.Embarcador.Escalas.EscalaVeiculo repositorioEscalaVeiculo = new Repositorio.Embarcador.Escalas.EscalaVeiculo(unitOfWork);
                List<Dominio.Entidades.Embarcador.Escalas.EscalaVeiculo> listaEscalaVeiculo = repositorioEscalaVeiculo.BuscarPorVeiculoEmEscala();
                List<Dominio.Entidades.Embarcador.Escalas.EscalaVeiculoEscalado> listaEscalaVeiculoEscalado = repositorioEscalaVeiculoEscalado.BuscarPorGeracaoEscala(geracaoEscala.Codigo);

                var listaEscalaVeiculoRetornar = (
                    from escalaVeiculo in listaEscalaVeiculo
                    select ObterVeiculosEmEscala(escalaVeiculo, listaEscalaVeiculoEscalado)
                ).ToList();

                return new JsonpResult(listaEscalaVeiculoRetornar);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os veículos em escala.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Exportar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Escalas.GeracaoEscala repositorioGeracaoEscala = new Repositorio.Embarcador.Escalas.GeracaoEscala(unitOfWork);
                Dominio.Entidades.Embarcador.Escalas.GeracaoEscala geracaoEscala = repositorioGeracaoEscala.BuscarPorCodigo(codigo, true);

                if (geracaoEscala == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro");

                if ((geracaoEscala.SituacaoEscala != SituacaoEscala.AgVeiculos) && (geracaoEscala.SituacaoEscala != SituacaoEscala.Finalizada))
                    return new JsonpResult(false, true, "A atual situção da geração de escala não permite a exportação das informações");

                Dominio.Enumeradores.TipoArquivoRelatorio tipoArquivoRelatorio = Request.GetEnumParam<Dominio.Enumeradores.TipoArquivoRelatorio>("Tipo");


                byte[] arquivo = ReportRequest.WithType(ReportType.GeracaoEscala)
                    .WithExecutionType(ExecutionType.Async)
                    .AddExtraData("CodigoGeracaoEscala", codigo)
                    .AddExtraData("tipoArquivoRelatorio", tipoArquivoRelatorio)
                    .CallReport()
                    .GetContentFile();
                
                if (arquivo == null)
                    throw new ControllerException("Não foi possível exportar as informações.");

                if (tipoArquivoRelatorio == Dominio.Enumeradores.TipoArquivoRelatorio.PDF)
                    return Arquivo(arquivo, "application/pdf", "Geração de Escala.pdf");

                return Arquivo(arquivo, "application/msexcel", "Geração de Escala.xls");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar as informações.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> FinalizarEscala()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Escalas.GeracaoEscala repositorioGeracaoEscala = new Repositorio.Embarcador.Escalas.GeracaoEscala(unitOfWork);
                Dominio.Entidades.Embarcador.Escalas.GeracaoEscala geracaoEscala = repositorioGeracaoEscala.BuscarPorCodigo(codigo, true);

                if (geracaoEscala == null)
                    throw new ControllerException("Não foi possível encontrar o registro");

                if (geracaoEscala.SituacaoEscala != SituacaoEscala.AgVeiculos)
                    throw new ControllerException("A atual situção da geração de escala não permite a finalização");

                geracaoEscala.SituacaoEscala = SituacaoEscala.Finalizada;

                repositorioGeracaoEscala.Atualizar(geracaoEscala, Auditado);

                GerarEscalasVeiculos(geracaoEscala, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao finalizar a escala.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> FinalizarExpedicao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Escalas.GeracaoEscala repositorioGeracaoEscala = new Repositorio.Embarcador.Escalas.GeracaoEscala(unitOfWork);
                Dominio.Entidades.Embarcador.Escalas.GeracaoEscala geracaoEscala = repositorioGeracaoEscala.BuscarPorCodigo(codigo, true);

                if (geracaoEscala == null)
                    throw new ControllerException("Não foi possível encontrar o registro");

                if (geracaoEscala.SituacaoEscala != SituacaoEscala.EmCriacao)
                    throw new ControllerException("A atual situção da geração de escala não permite finalizar a expedição");

                geracaoEscala.SituacaoEscala = SituacaoEscala.AgVeiculos;

                repositorioGeracaoEscala.Atualizar(geracaoEscala, Auditado);

                GerarExpedicaoParaVeiculos(geracaoEscala, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao finalizar a expedição.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Número da Escala", "NumeroEscala", 20, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Produtos", "Produtos", 30, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Data da Escala", "DataEscala", 20, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Situação", "DescricaoSituacao", 10, Models.Grid.Align.center, false);

                Dominio.ObjetosDeValor.Embarcador.Escalas.FiltroPesquisaGeracaoEscala filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                Repositorio.Embarcador.Escalas.GeracaoEscala repositorioGeracaoEscala = new Repositorio.Embarcador.Escalas.GeracaoEscala(unitOfWork);
                int totalRegistros = repositorioGeracaoEscala.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Escalas.GeracaoEscala> listaGeracaoEscala = (totalRegistros > 0) ? repositorioGeracaoEscala.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Escalas.GeracaoEscala>();

                var listaGeracaoEscalaRetornar = (
                    from p in listaGeracaoEscala
                    select new
                    {
                        p.Codigo,
                        p.NumeroEscala,
                        Produtos = p.Produtos.Count > 0 ? string.Join(",", (from obj in p.Produtos select obj.CodigoProdutoEmbarcador + " - " + obj.Descricao)) : "",
                        DataEscala = p.DataEscala.ToString("dd/MM/yyyy"),
                        DescricaoSituacao = p.SituacaoEscala.ObterDescricao()
                    }
                ).ToList();

                grid.AdicionaRows(listaGeracaoEscalaRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
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

        public async Task<IActionResult> SugerirVeiculosEscalados()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Escalas.GeracaoEscala repositorioGeracaoEscala = new Repositorio.Embarcador.Escalas.GeracaoEscala(unitOfWork);
                Dominio.Entidades.Embarcador.Escalas.GeracaoEscala geracaoEscala = repositorioGeracaoEscala.BuscarPorCodigo(codigo);

                if (geracaoEscala == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                DiaSemana diaSemana = DiaSemanaHelper.ObterDiaSemana(geracaoEscala.DataEscala);
                Repositorio.Embarcador.Escalas.EscalaOrigemDestino repositorioEscalaOrigemDestino = new Repositorio.Embarcador.Escalas.EscalaOrigemDestino(unitOfWork);
                Repositorio.Embarcador.Escalas.EscalaVeiculo repositorioEscalaVeiculo = new Repositorio.Embarcador.Escalas.EscalaVeiculo(unitOfWork);
                Repositorio.Embarcador.Escalas.EscalaVeiculoEscalado repositorioEscalaVeiculoEscalado = new Repositorio.Embarcador.Escalas.EscalaVeiculoEscalado(unitOfWork);
                Repositorio.Embarcador.Logistica.ExpedicaoCarregamento repositorioExpedicaoCarregamento = new Repositorio.Embarcador.Logistica.ExpedicaoCarregamento(unitOfWork);
                Repositorio.Embarcador.Produtos.GrupoProdutoTipoCarga repositorioGrupoProdutoTipoCarga = new Repositorio.Embarcador.Produtos.GrupoProdutoTipoCarga(unitOfWork);
                Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);

                List<Dominio.Entidades.Embarcador.Escalas.GeracaoEscalaRestricaoModeloVeicular> restricoes = geracaoEscala.GeracaoEscalaRestricoesModeloVeicular.ToList();
                List<Dominio.Entidades.Embarcador.Escalas.EscalaOrigemDestino> origensDestinos = repositorioEscalaOrigemDestino.BuscarPorGeracaoEscala(geracaoEscala.Codigo);
                List<Dominio.Entidades.Embarcador.Escalas.EscalaVeiculo> listaEscalaVeiculo = repositorioEscalaVeiculo.BuscarPorVeiculoEmEscala();
                List<Dominio.Entidades.Embarcador.Escalas.EscalaVeiculo> listaEscalaVeiculoSugerir = listaEscalaVeiculo.ToList();
                List<Dominio.ObjetosDeValor.Embarcador.Escalas.GeracaoEscalaHorarioCarregamento> listaHorarioCarregamento = new List<Dominio.ObjetosDeValor.Embarcador.Escalas.GeracaoEscalaHorarioCarregamento>();
                Dictionary<string, int> listaEscalaVeiculoEscalado = new Dictionary<string, int>();
                List<dynamic> listaOrigemDestino = new List<dynamic>();

                foreach (Dominio.Entidades.Embarcador.Escalas.EscalaOrigemDestino origemDestino in origensDestinos)
                {
                    dynamic origemDestinoRetornar = new ExpandoObject();

                    origemDestinoRetornar.Codigo = origemDestino.Codigo;
                    origemDestinoRetornar.Origem = origemDestino.CentroCarregamento.Descricao;
                    origemDestinoRetornar.Destino = origemDestino.ClienteDestino?.Descricao ?? "";
                    origemDestinoRetornar.Quantidade = origemDestino.Quantidade.ToString("n2");
                    origemDestinoRetornar.Produto = origemDestino.ExpedicaoEscala.ProdutoEmbarcador?.Descricao ?? "";
                    origemDestinoRetornar.UnidadeMedida = origemDestino.ExpedicaoEscala.ProdutoEmbarcador?.Unidade?.Descricao ?? "Unidade";
                    origemDestinoRetornar.VeiculosEscalados = new List<dynamic>();

                    listaOrigemDestino.Add(origemDestinoRetornar);

                    Dominio.Entidades.Embarcador.Produtos.GrupoProdutoTipoCarga grupoProdutoTipoCarga = repositorioGrupoProdutoTipoCarga.ConsultarPorGrupoProduto(origemDestino.ExpedicaoEscala.ProdutoEmbarcador.GrupoProduto?.Codigo ?? 0).FirstOrDefault();

                    if (grupoProdutoTipoCarga == null)
                        continue;

                    Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeCarregamento excecoesDia = (from o in origemDestino.CentroCarregamento.ExcecoesCapacidadeCarregamento where o.Data == geracaoEscala.DataEscala.Date select o).FirstOrDefault();
                    List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento> periodosCarregamento = (excecoesDia != null) ? excecoesDia.PeriodosCarregamento.OrderBy(o => o.HoraInicio).ToList() : (from o in origemDestino.CentroCarregamento.PeriodosCarregamento where o.Dia == diaSemana orderby o.HoraInicio select o).ToList();

                    if (periodosCarregamento.Count() == 0)
                        continue;

                    decimal quantidadeTotal = 0m;
                    List<Dominio.Entidades.Embarcador.Logistica.ExpedicaoCarregamentoModeloVeicularCarga> modelosVeicularesCargaExclusivo = repositorioExpedicaoCarregamento.BuscarPorProdutoECentro(
                        origemDestino.ExpedicaoEscala.ProdutoEmbarcador.Codigo, origemDestino.CentroCarregamento.Codigo, origemDestino.ClienteDestino.CPF_CNPJ, diaSemana
                    );

                    for (int i = 0; i < listaEscalaVeiculoSugerir.Count; i++)
                    {
                        Dominio.Entidades.Embarcador.Escalas.EscalaVeiculo escalaVeiculo = listaEscalaVeiculoSugerir[i];
                        Dominio.Entidades.Veiculo veiculo = escalaVeiculo.Veiculo;

                        if ((modelosVeicularesCargaExclusivo.Count > 0) && !modelosVeicularesCargaExclusivo.Any(o => o.ModeloVeicularCarga.Codigo == veiculo.ModeloVeicularCarga.Codigo))
                            continue;

                        repositorioEscalaVeiculoEscalado.VerificarRotaEscalaAnteriorPossuiMesmaClassificacao(veiculo.Codigo, geracaoEscala.DataEscala, RotaFreteClasse.Um);
                        repositorioEscalaVeiculoEscalado.VerificarRotaEscalaAnteriorPossuiMesmaClassificacao(veiculo.Codigo, geracaoEscala.DataEscala, RotaFreteClasse.Dois);

                        if ((origemDestino.Rota?.Classificacao != null) && repositorioEscalaVeiculoEscalado.VerificarRotaEscalaAnteriorPossuiMesmaClassificacao(veiculo.Codigo, geracaoEscala.DataEscala, origemDestino.Rota.Classificacao.Classe))
                            continue;

                        TimeSpan? horarioCarregamento = ObterHorarioCarregamento(listaHorarioCarregamento, periodosCarregamento, restricoes, origemDestino, veiculo, grupoProdutoTipoCarga.TipoDeCarga, configuracao, unitOfWork);

                        if (!horarioCarregamento.HasValue)
                            continue;
                        
                        Dominio.Entidades.Usuario veiculoMotorista = repVeiculoMotorista.BuscarMotoristaPrincipal(veiculo.Codigo);

                        var veiculoEscalado = new
                        {
                            Codigo = Guid.NewGuid().ToString(),
                            CodigoEmpresa = veiculo.Empresa?.Codigo ?? 0,
                            CodigoModeloVeicularCarga = veiculo.ModeloVeicularCarga.Codigo,
                            CodigoMotorista = veiculoMotorista?.Codigo ?? 0,
                            CodigoVeiculo = veiculo.Codigo,
                            ModeloVeicularCarga = veiculo.ModeloVeicularCarga.Descricao,
                            Quantidade = veiculo.ModeloVeicularCarga.CapacidadePesoTransporte.ToString("n2"),
                            Empresa = veiculo.Empresa?.RazaoSocial ?? "",
                            Veiculo = veiculo.Placa,
                            HoraCarregamento = horarioCarregamento.Value.ToString(@"hh\:mm"),
                            Motorista = veiculoMotorista?.Nome ?? ""
                        };

                        origemDestinoRetornar.VeiculosEscalados.Add(veiculoEscalado);
                        listaEscalaVeiculoEscalado.Add(veiculoEscalado.Codigo, veiculo.Codigo);

                        listaEscalaVeiculoSugerir.RemoveAt(i);
                        i--;

                        quantidadeTotal += veiculo.ModeloVeicularCarga.CapacidadePesoTransporte;

                        if (quantidadeTotal >= origemDestino.Quantidade)
                            break;
                    }
                }

                var listaEscalaVeiculoRetornar = (
                    from escalaVeiculo in listaEscalaVeiculo
                    select ObterVeiculosEmEscala(escalaVeiculo, listaEscalaVeiculoEscalado)
                ).ToList();

                return new JsonpResult(new
                {
                    ListaVeiculo = listaEscalaVeiculoRetornar,
                    OrigensDestinos = listaOrigemDestino
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao sugerir os veículos escalados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Escalas.FiltroPesquisaGeracaoEscala ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Escalas.FiltroPesquisaGeracaoEscala()
            {
                CodigoCentroCarregamento = Request.GetIntParam("CentroCarregamento"),
                CodigoProduto = Request.GetIntParam("Produto"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicio"),
                DataLimite = Request.GetNullableDateTimeParam("DataFim"),
                NumeroEscala = Request.GetIntParam("NumeroEscala"),
                SituacaoEscala = Request.GetNullableEnumParam<SituacaoEscala>("Situacao")
            };
        }

        private dynamic ObterVeiculosEmEscala(Dominio.Entidades.Embarcador.Escalas.EscalaVeiculo escalaVeiculo, List<Dominio.Entidades.Embarcador.Escalas.EscalaVeiculoEscalado> listaEscalaVeiculoEscalado)
        {
            List<string> listaCodigoEscalaVeiculoEscalado = (from o in listaEscalaVeiculoEscalado where o.Veiculos.Any(v => v.Codigo == escalaVeiculo.Veiculo.Codigo) select o.Codigo.ToString()).ToList();

            return ObterVeiculosEmEscala(escalaVeiculo, listaCodigoEscalaVeiculoEscalado);
        }

        private dynamic ObterVeiculosEmEscala(Dominio.Entidades.Embarcador.Escalas.EscalaVeiculo escalaVeiculo, Dictionary<string, int> listaEscalaVeiculoEscalado)
        {
            List<string> listaCodigoEscalaVeiculoEscalado = (from o in listaEscalaVeiculoEscalado where o.Value == escalaVeiculo.Veiculo.Codigo select o.Key).ToList();

            return ObterVeiculosEmEscala(escalaVeiculo, listaCodigoEscalaVeiculoEscalado);
        }

        private dynamic ObterVeiculosEmEscala(Dominio.Entidades.Embarcador.Escalas.EscalaVeiculo escalaVeiculo, List<string> listaCodigoEscalaVeiculoEscalado)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);
            Dominio.Entidades.Usuario veiculoMotorista = repVeiculoMotorista.BuscarMotoristaPrincipal(escalaVeiculo.Veiculo.Codigo);

            return new
            {
                CodigoVeiculo = escalaVeiculo.Veiculo.Codigo,
                CodigoEmpresa = escalaVeiculo.Veiculo.Empresa?.Codigo ?? 0,
                CodigoModeloVeicularCarga = escalaVeiculo.Veiculo.ModeloVeicularCarga?.Codigo ?? 0,
                CodigoMotorista = veiculoMotorista?.Codigo ?? 0,
                CodigosEscalaVeiculoEscalado = listaCodigoEscalaVeiculoEscalado,
                Quantidade = escalaVeiculo.Veiculo.ModeloVeicularCarga?.CapacidadePesoTransporte.ToString("n2") ?? "0,00",
                Empresa = escalaVeiculo.Veiculo.Empresa?.Descricao ?? "",
                ModeloVeicularCarga = escalaVeiculo.Veiculo.ModeloVeicularCarga?.Descricao ?? "",
                Motorista = veiculoMotorista?.Nome ?? "",
                Veiculo = escalaVeiculo.Veiculo.Placa,
                DT_RowId = escalaVeiculo.Veiculo.Codigo,
                DT_RowColor = (listaCodigoEscalaVeiculoEscalado.Count > 0) ? "#ffcc99" : "#ffffff"
            };
        }

        #endregion

        #region Métodos Privados da Etapa Dados da Escala

        private void GerarExpedicaoEscala(Dominio.Entidades.Embarcador.Escalas.GeracaoEscala geracaoEscala, Repositorio.UnitOfWork unitOfWork)
        {
            DiaSemana diaSemana = DiaSemanaHelper.ObterDiaSemana(geracaoEscala.DataEscala);
            Repositorio.Embarcador.Logistica.CentroDescarregamento repositorioCentroDescarregamento = new Repositorio.Embarcador.Logistica.CentroDescarregamento(unitOfWork);
            Repositorio.Embarcador.Logistica.ExpedicaoCarregamento repositorioExpedicaoCarregamento = new Repositorio.Embarcador.Logistica.ExpedicaoCarregamento(unitOfWork);
            Repositorio.Embarcador.Escalas.ExpedicaoEscala repositorioExpedicaoEscala = new Repositorio.Embarcador.Escalas.ExpedicaoEscala(unitOfWork);
            Repositorio.Embarcador.Escalas.ExpedicaoEscalaDestino repositorioExpedicaoEscalaDestino = new Repositorio.Embarcador.Escalas.ExpedicaoEscalaDestino(unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoEmbarcador in geracaoEscala.Produtos)
            {
                List<Dominio.Entidades.Embarcador.Logistica.ExpedicaoCarregamento> expedicoesCarregamento = repositorioExpedicaoCarregamento.BuscarPorProduto(produtoEmbarcador.Codigo, diaSemana);

                if (expedicoesCarregamento.Count == 0)
                    continue;

                IEnumerable<Dominio.Entidades.Embarcador.Logistica.CentroCarregamento> centrosCarregamento = expedicoesCarregamento.Select(o => o.CentroCarregamento).Distinct();
                List<double> listaCpfCnpjClienteDestino = (from o in expedicoesCarregamento select o.ClienteDestino.CPF_CNPJ).Distinct().ToList();
                List<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento> centrosDescarregamento = repositorioCentroDescarregamento.BuscarPorDestinatarios(listaCpfCnpjClienteDestino);

                foreach (Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento in centrosCarregamento)
                {
                    List<Dominio.Entidades.Embarcador.Logistica.ExpedicaoCarregamento> expedicoesCarregamentoPorCentroCarregamento = (
                        from o in expedicoesCarregamento
                        where o.CentroCarregamento.Codigo == centroCarregamento.Codigo
                        select o
                    ).ToList();

                    int quantidadeTotal = expedicoesCarregamentoPorCentroCarregamento.Sum(o => o.Quantidade);
                    Dominio.Entidades.Embarcador.Escalas.ExpedicaoEscala expedicaoEscala = new Dominio.Entidades.Embarcador.Escalas.ExpedicaoEscala()
                    {
                        CentroCarregamento = centroCarregamento,
                        GeracaoEscala = geracaoEscala,
                        ProdutoEmbarcador = produtoEmbarcador,
                        Quantidade = quantidadeTotal,
                        QuantidadeOriginal = quantidadeTotal
                    };

                    repositorioExpedicaoEscala.Inserir(expedicaoEscala, Auditado);

                    foreach (Dominio.Entidades.Embarcador.Logistica.ExpedicaoCarregamento expedicaoCarregamento in expedicoesCarregamentoPorCentroCarregamento)
                    {
                        Dominio.Entidades.Embarcador.Escalas.ExpedicaoEscalaDestino expedicaoEscalaDestino = new Dominio.Entidades.Embarcador.Escalas.ExpedicaoEscalaDestino()
                        {
                            ClienteDestino = expedicaoCarregamento.ClienteDestino,
                            ExpedicaoEscala = expedicaoEscala,
                            Localidade = expedicaoCarregamento.ClienteDestino.Localidade,
                            Quantidade = expedicaoCarregamento.Quantidade
                        };

                        expedicaoEscalaDestino.CentroDescarregamento = (
                            from o in centrosDescarregamento
                            where o.Destinatario.CPF_CNPJ == expedicaoCarregamento.ClienteDestino.CPF_CNPJ
                            select o
                        ).FirstOrDefault();

                        repositorioExpedicaoEscalaDestino.Inserir(expedicaoEscalaDestino, Auditado);
                    }
                }
            }
        }

        private void SalvarProdutos(Dominio.Entidades.Embarcador.Escalas.GeracaoEscala dadosEscala, Repositorio.UnitOfWork unitOfWork)
        {
            dynamic produtos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Produtos"));

            if (produtos.Count == 0)
                return;

            Repositorio.Embarcador.Produtos.ProdutoEmbarcador repositorioProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork);
            dadosEscala.Produtos = new List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador>();

            foreach (var produto in produtos)
            {
                Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoSalvar = repositorioProdutoEmbarcador.BuscarPorCodigo(((string)produto.Codigo).ToInt()) ?? throw new ControllerException("Produto não encontrado");

                dadosEscala.Produtos.Add(produtoSalvar);
            }
        }

        private void SalvarRestricoesModeloVeicular(Dominio.Entidades.Embarcador.Escalas.GeracaoEscala dadosEscala, Repositorio.UnitOfWork unitOfWork)
        {
            dynamic modelosRestricaoRodagem = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ModelosRestricaoRodagem"));

            if (modelosRestricaoRodagem.Count == 0)
                return;

            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repositorioModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
            Repositorio.Embarcador.Escalas.GeracaoEscalaRestricaoModeloVeicular repositorioGeracaoEscalaRestricaoModeloVeicular = new Repositorio.Embarcador.Escalas.GeracaoEscalaRestricaoModeloVeicular(unitOfWork);

            foreach (var modeloRestricaoRodagem in modelosRestricaoRodagem)
            {
                Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga = repositorioModeloVeicularCarga.BuscarPorCodigo(((string)modeloRestricaoRodagem.CodigoModeloVeicularCarga).ToInt()) ?? throw new ControllerException("Modelo veicular não encontrado");
                Dominio.Entidades.Embarcador.Escalas.GeracaoEscalaRestricaoModeloVeicular geracaoEscalaRestricaoModeloVeicular = new Dominio.Entidades.Embarcador.Escalas.GeracaoEscalaRestricaoModeloVeicular()
                {
                    GeracaoEscala = dadosEscala,
                    HoraFimRestricao = ((string)modeloRestricaoRodagem.HoraFimRestricao).ToTime(),
                    HoraInicioRestricao = ((string)modeloRestricaoRodagem.HoraInicioRestricao).ToTime(),
                    ModeloVeicularCarga = modeloVeicularCarga
                };

                repositorioGeracaoEscalaRestricaoModeloVeicular.Inserir(geracaoEscalaRestricaoModeloVeicular, Auditado);
            }
        }

        #endregion

        #region Métodos Privados da Etapa Expedição

        private void AdicionarOuAtualizarExpedicoesEscalaAlteradas(Dominio.Entidades.Embarcador.Escalas.GeracaoEscala geracaoEscala, List<Dominio.Entidades.Embarcador.Escalas.ExpedicaoEscala> expedicoesEscala, dynamic expedicoesEscalaAtualizar, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Escalas.ExpedicaoEscala repositorioExpedicaoEscala = new Repositorio.Embarcador.Escalas.ExpedicaoEscala(unitOfWork);
            Repositorio.Embarcador.Escalas.ExpedicaoEscalaDestino repositorioExpedicaoEscalaDestino = new Repositorio.Embarcador.Escalas.ExpedicaoEscalaDestino(unitOfWork);
            Repositorio.Embarcador.Logistica.CentroCarregamento repositorioCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unitOfWork);
            Repositorio.Embarcador.Produtos.ProdutoEmbarcador repositorioProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork);
            Repositorio.Cliente repositorioClienteDestino = new Repositorio.Cliente(unitOfWork);

            foreach (var expedicaoEscala in expedicoesEscalaAtualizar)
            {
                int? codigo = ((string)expedicaoEscala.Codigo).ToNullableInt();
                Dominio.Entidades.Embarcador.Escalas.ExpedicaoEscala expedicaoEscalaSalvar = null;

                if (!codigo.HasValue)
                {
                    expedicaoEscalaSalvar = new Dominio.Entidades.Embarcador.Escalas.ExpedicaoEscala()
                    {
                        CentroCarregamento = repositorioCentroCarregamento.BuscarPorCodigo(((string)expedicaoEscala.CentroCarregamento).ToInt()) ?? throw new ControllerException("Centro de carregamento não encontrado"),
                        GeracaoEscala = geracaoEscala,
                        ProdutoEmbarcador = repositorioProdutoEmbarcador.BuscarPorCodigo(((string)expedicaoEscala.Produto).ToInt()) ?? throw new ControllerException("Produto não encontrado"),
                        Quantidade = ((string)expedicaoEscala.Quantidade).ToDecimal(),
                        QuantidadeOriginal = ((string)expedicaoEscala.Quantidade).ToDecimal()
                    };

                    repositorioExpedicaoEscala.Inserir(expedicaoEscalaSalvar);
                }
                else
                {
                    expedicaoEscalaSalvar = (from o in expedicoesEscala where o.Codigo == codigo.Value select o).FirstOrDefault() ?? throw new ControllerException("Expedição não encontrada");
                    expedicaoEscalaSalvar.Quantidade = ((string)expedicaoEscala.Quantidade).ToDecimal();

                    repositorioExpedicaoEscala.Atualizar(expedicaoEscalaSalvar);
                }

                foreach (var destino in expedicaoEscala.Destinos)
                {
                    int? codigoDestino = ((string)destino.Codigo).ToNullableInt();

                    if (!codigoDestino.HasValue)
                    {
                        Dominio.Entidades.Embarcador.Escalas.ExpedicaoEscalaDestino expedicaoEscalaDestino = new Dominio.Entidades.Embarcador.Escalas.ExpedicaoEscalaDestino()
                        {
                            ClienteDestino = repositorioClienteDestino.BuscarPorCPFCNPJ(((string)destino.CodigoClienteDestino).ToDouble()) ?? throw new ControllerException("Destino não encontrado"),
                            ExpedicaoEscala = expedicaoEscalaSalvar,
                            Quantidade = ((string)destino.Quantidade).ToDecimal()
                        };

                        repositorioExpedicaoEscalaDestino.Inserir(expedicaoEscalaDestino);
                    }
                    else
                    {
                        Dominio.Entidades.Embarcador.Escalas.ExpedicaoEscalaDestino expedicaoEscalaDestino = (from o in expedicaoEscalaSalvar.ExpedicaoEscalaDestinos where o.Codigo == codigoDestino.Value select o).FirstOrDefault() ?? throw new ControllerException("Destino não encontrado");
                        expedicaoEscalaDestino.Quantidade = ((string)destino.Quantidade).ToDecimal();

                        repositorioExpedicaoEscalaDestino.Atualizar(expedicaoEscalaDestino);
                    }
                }
            }
        }

        private void AtualizarExpedicoesEscala(Dominio.Entidades.Embarcador.Escalas.GeracaoEscala geracaoEscala, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Escalas.ExpedicaoEscala repositorioExpedicaoEscala = new Repositorio.Embarcador.Escalas.ExpedicaoEscala(unitOfWork);
            List<Dominio.Entidades.Embarcador.Escalas.ExpedicaoEscala> expedicoesEscala = repositorioExpedicaoEscala.BuscarPorGeracaoEscala(geracaoEscala.Codigo);
            dynamic expedicoesEscalaAtualizar = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ExpedicoesEscala"));

            ExcluirExpedicoesEscalaRemovidas(geracaoEscala, expedicoesEscala, expedicoesEscalaAtualizar, unitOfWork);
            AdicionarOuAtualizarExpedicoesEscalaAlteradas(geracaoEscala, expedicoesEscala, expedicoesEscalaAtualizar, unitOfWork);
        }

        private void ExcluirExpedicoesEscalaRemovidas(Dominio.Entidades.Embarcador.Escalas.GeracaoEscala geracaoEscala, List<Dominio.Entidades.Embarcador.Escalas.ExpedicaoEscala> expedicoesEscala, dynamic expedicoesEscalaAtualizar, Repositorio.UnitOfWork unitOfWork)
        {
            if ((expedicoesEscala == null) || (expedicoesEscala.Count == 0))
                return;

            Repositorio.Embarcador.Escalas.ExpedicaoEscalaDestino repositorioExpedicaoEscalaDestino = new Repositorio.Embarcador.Escalas.ExpedicaoEscalaDestino(unitOfWork);
            List<int> listaCodigoAtualizado = new List<int>();

            foreach (var expedicaoEscala in expedicoesEscalaAtualizar)
                listaCodigoAtualizado.Add(((string)expedicaoEscala.Codigo).ToInt());

            Repositorio.Embarcador.Escalas.ExpedicaoEscala repositorioExpedicaoEscala = new Repositorio.Embarcador.Escalas.ExpedicaoEscala(unitOfWork);
            List<Dominio.Entidades.Embarcador.Escalas.ExpedicaoEscala> listaExpedicaoEscalaRemover = (from o in expedicoesEscala where !listaCodigoAtualizado.Contains(o.Codigo) select o).ToList();

            foreach (Dominio.Entidades.Embarcador.Escalas.ExpedicaoEscala expedicaoEscala in listaExpedicaoEscalaRemover)
            {
                expedicaoEscala.ExpedicaoEscalaDestinos = null;
                repositorioExpedicaoEscala.Deletar(expedicaoEscala);
            }

            foreach (var expedicaoEscala in expedicoesEscalaAtualizar)
            {
                int codigoExpedicaoEscala = ((string)expedicaoEscala.Codigo).ToInt();
                Dominio.Entidades.Embarcador.Escalas.ExpedicaoEscala expedicaoEscalaAtualizar = (from o in expedicoesEscala where o.Codigo == codigoExpedicaoEscala select o).FirstOrDefault();

                if (expedicaoEscalaAtualizar == null)
                    continue;

                List<int> listaCodigoDestinoAtualizado = new List<int>();

                foreach (var destino in expedicaoEscala.Destinos)
                    listaCodigoDestinoAtualizado.Add(((string)destino.Codigo).ToInt());

                List<Dominio.Entidades.Embarcador.Escalas.ExpedicaoEscalaDestino> listaExpedicaoEscalaDestinoRemover = (from o in expedicaoEscalaAtualizar.ExpedicaoEscalaDestinos where !listaCodigoDestinoAtualizado.Contains(o.Codigo) select o).ToList();

                foreach (var destino in listaExpedicaoEscalaDestinoRemover)
                    repositorioExpedicaoEscalaDestino.Deletar(destino);
            }
        }

        private void GerarExpedicaoParaVeiculos(Dominio.Entidades.Embarcador.Escalas.GeracaoEscala geracaoEscala, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Escalas.GeracaoEscala repositorioGeracaoEscala = new Repositorio.Embarcador.Escalas.GeracaoEscala(unitOfWork);
            Repositorio.Embarcador.Escalas.ExpedicaoEscalaDestino repositorioExpedicaoEscalaDestino = new Repositorio.Embarcador.Escalas.ExpedicaoEscalaDestino(unitOfWork);
            Repositorio.Embarcador.Escalas.ExpedicaoEscala repositorioExpedicaoEscala = new Repositorio.Embarcador.Escalas.ExpedicaoEscala(unitOfWork);
            Repositorio.RotaFrete repositorioRotaFrete = new Repositorio.RotaFrete(unitOfWork);
            List<Dominio.Entidades.Embarcador.Escalas.ExpedicaoEscala> listaExpedicaoEscala = repositorioExpedicaoEscala.BuscarPorGeracaoEscala(geracaoEscala.Codigo);
            List<Dominio.Entidades.Embarcador.Escalas.ExpedicaoEscalaDestino> listaExpedicaoEscalaDestino = repositorioExpedicaoEscalaDestino.BuscarPorGeracaoEscala(geracaoEscala.Codigo);

            Repositorio.Embarcador.Escalas.EscalaOrigemDestino repositorioEscalaOrigemDestino = new Repositorio.Embarcador.Escalas.EscalaOrigemDestino(unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Escalas.ExpedicaoEscala expedicaoEscala in listaExpedicaoEscala)
            {
                if (expedicaoEscala.Quantidade == 0)
                    continue;

                List<Dominio.Entidades.Embarcador.Escalas.ExpedicaoEscalaDestino> listaExpedicaoEscalaDestinoPorExpedicaoEscala = (from o in listaExpedicaoEscalaDestino where o.ExpedicaoEscala.Codigo == expedicaoEscala.Codigo select o).ToList();

                foreach (Dominio.Entidades.Embarcador.Escalas.ExpedicaoEscalaDestino expedicaoEscalaDestino in listaExpedicaoEscalaDestinoPorExpedicaoEscala)
                {
                    if (expedicaoEscalaDestino.Quantidade > 0)
                    {
                        Dominio.Entidades.Embarcador.Escalas.EscalaOrigemDestino escalaOrigemDestino = new Dominio.Entidades.Embarcador.Escalas.EscalaOrigemDestino()
                        {
                            CentroCarregamento = expedicaoEscala.CentroCarregamento,
                            CentroDescarregamento = expedicaoEscalaDestino.CentroDescarregamento,
                            ClienteDestino = expedicaoEscalaDestino.ClienteDestino,
                            ExpedicaoEscala = expedicaoEscala,
                            Localidade = expedicaoEscalaDestino.Localidade,
                            Quantidade = expedicaoEscalaDestino.Quantidade,
                            Rota = ObterRotaFrete(expedicaoEscalaDestino, unitOfWork)
                        };

                        repositorioEscalaOrigemDestino.Inserir(escalaOrigemDestino, Auditado);
                    }
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, expedicaoEscala, null, "Finalizou Geração de Escala.", unitOfWork);
            }

            Servicos.Auditoria.Auditoria.Auditar(Auditado, geracaoEscala, null, "Finalizou Geração de Escala.", unitOfWork);
        }

        private dynamic ObterExpedicoesEscala(Dominio.Entidades.Embarcador.Escalas.GeracaoEscala geracaoEscala, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Escalas.ExpedicaoEscala repositorioExpedicaoEscala = new Repositorio.Embarcador.Escalas.ExpedicaoEscala(unitOfWork);
            List<Dominio.Entidades.Embarcador.Escalas.ExpedicaoEscala> expedicoesEscala = repositorioExpedicaoEscala.BuscarPorGeracaoEscala(geracaoEscala.Codigo);

            return (
                from expedicaoEscala in expedicoesEscala
                select new
                {
                    expedicaoEscala.Codigo,
                    Quantidade = expedicaoEscala.Quantidade.ToString("n2"),
                    CentroCarregamento = new
                    {
                        expedicaoEscala.CentroCarregamento.Codigo,
                        expedicaoEscala.CentroCarregamento.Descricao
                    },
                    Produto = new
                    {
                        expedicaoEscala.ProdutoEmbarcador.Codigo,
                        expedicaoEscala.ProdutoEmbarcador.Descricao,
                        UnidadeMedida = expedicaoEscala.ProdutoEmbarcador.Unidade?.Descricao ?? "Unidade"
                    },
                    Destinos = (
                        from destino in expedicaoEscala.ExpedicaoEscalaDestinos
                        select new
                        {
                            destino.Codigo,
                            CodigoClienteDestino = destino.ClienteDestino?.CPF_CNPJ ?? 0,
                            ClienteDestino = destino.ClienteDestino?.Descricao ?? "",
                            Quantidade = destino.Quantidade.ToString("n2")
                        }
                    ).ToList()
                }
            ).ToList();
        }

        private Dominio.Entidades.RotaFrete ObterRotaFrete(Dominio.Entidades.Embarcador.Escalas.ExpedicaoEscalaDestino expedicaoEscalaDestino, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);
            Dominio.Entidades.Cliente remetente = repositorioCliente.BuscarPorCPFCNPJ(expedicaoEscalaDestino.ExpedicaoEscala.CentroCarregamento.Filial.CNPJ.ToDouble());

            if (remetente == null)
                return null;

            List<Dominio.Entidades.Cliente> destinatarios = new List<Dominio.Entidades.Cliente>() { expedicaoEscalaDestino.ClienteDestino };
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteLocalidadeOrdem> localidadesDestino = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteLocalidadeOrdem>();
            List<Dominio.Entidades.Estado> estadosDestino = new List<Dominio.Entidades.Estado>();

            if (expedicaoEscalaDestino.ClienteDestino.Localidade != null)
            {
                localidadesDestino.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteLocalidadeOrdem() { Localidade = expedicaoEscalaDestino.ClienteDestino.Localidade });
                estadosDestino.Add(expedicaoEscalaDestino.ClienteDestino.Localidade.Estado);
            }

            List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaDestinatarioOrdem> destinatariosOrdenados = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaDestinatarioOrdem>();
            destinatariosOrdenados.AddRange(from destinatario in destinatarios select new Dominio.ObjetosDeValor.Embarcador.Logistica.RotaDestinatarioOrdem() { Cliente = destinatario });

            return new Repositorio.RotaFrete(unitOfWork).BuscarPorOrigemEDestinos(remetente?.Localidade, remetente, destinatariosOrdenados, localidadesDestino, estadosDestino).FirstOrDefault();
        }

        #endregion

        #region Métodos Privados da Etapa Veículos

        private void AtualizarOrigensDestinosEscalaAlteradas(Dominio.Entidades.Embarcador.Escalas.GeracaoEscala geracaoEscala, List<Dominio.Entidades.Embarcador.Escalas.EscalaOrigemDestino> origensDestinos, dynamic origensDestinosEscala, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Escalas.EscalaVeiculoEscalado repositorioEscalaVeiculoEscalado = new Repositorio.Embarcador.Escalas.EscalaVeiculoEscalado(unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repositorioModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Usuario repositorioMotorista = new Repositorio.Usuario(unitOfWork);
            Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(unitOfWork);

            foreach (var origemDestinoEscala in origensDestinosEscala)
            {
                int codigo = ((string)origemDestinoEscala.Codigo).ToInt();
                Dominio.Entidades.Embarcador.Escalas.EscalaOrigemDestino origemDestinoAtualizar = (from o in origensDestinos where o.Codigo == codigo select o).FirstOrDefault() ?? throw new ControllerException("Expedição não encontrada");

                foreach (var veiculoEscalado in origemDestinoEscala.VeiculosEscalados)
                {
                    int? codigoVeiculoEscalado = ((string)veiculoEscalado.Codigo).ToNullableInt();
                    TimeSpan horaCarregamento = ((string)veiculoEscalado.HoraCarregamento).ToTime();
                    Dominio.Entidades.Usuario motorista = repositorioMotorista.BuscarPorCodigo(((string)veiculoEscalado.CodigoMotorista).ToInt()) ?? throw new ControllerException("Motorista não encontrado");

                    if (!codigoVeiculoEscalado.HasValue)
                    {
                        Dominio.Entidades.Empresa empresa = repositorioEmpresa.BuscarPorCodigo(((string)veiculoEscalado.CodigoEmpresa).ToInt()) ?? throw new ControllerException("Empresa não encontrada");
                        Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga = repositorioModeloVeicularCarga.BuscarPorCodigo(((string)veiculoEscalado.CodigoModeloVeicularCarga).ToInt()) ?? throw new ControllerException("Modelo veicular de carga não encontrado");
                        Dominio.Entidades.Veiculo veiculo = repositorioVeiculo.BuscarPorCodigo(((string)veiculoEscalado.CodigoVeiculo).ToInt()) ?? throw new ControllerException("Veículo não encontrado");

                        Dominio.Entidades.Embarcador.Escalas.EscalaVeiculoEscalado veiculoEscaladoAdicionar = new Dominio.Entidades.Embarcador.Escalas.EscalaVeiculoEscalado()
                        {
                            Empresa = empresa,
                            EscalaOrigemDestino = origemDestinoAtualizar,
                            HoraCarregamento = horaCarregamento,
                            ModeloVeicularCarga = modeloVeicularCarga,
                            Motoristas = new List<Dominio.Entidades.Usuario>() { motorista },
                            Quantidade = ((string)veiculoEscalado.Quantidade).ToDecimal(),
                            Veiculos = new List<Dominio.Entidades.Veiculo>() { veiculo }
                        };

                        repositorioEscalaVeiculoEscalado.Inserir(veiculoEscaladoAdicionar);
                    }
                    else
                    {
                        Dominio.Entidades.Embarcador.Escalas.EscalaVeiculoEscalado veiculoEscaladoAtualizar = (from o in origemDestinoAtualizar.EscalaVeiculosEscalados where o.Codigo == codigoVeiculoEscalado.Value select o).FirstOrDefault() ?? throw new ControllerException("Veículo escalado não encontrado");

                        if (veiculoEscaladoAtualizar.Motoristas == null)
                            veiculoEscaladoAtualizar.Motoristas = new List<Dominio.Entidades.Usuario>();

                        veiculoEscaladoAtualizar.HoraCarregamento = horaCarregamento;
                        veiculoEscaladoAtualizar.Motoristas.Clear();
                        veiculoEscaladoAtualizar.Motoristas.Add(motorista);

                        repositorioEscalaVeiculoEscalado.Atualizar(veiculoEscaladoAtualizar);
                    }
                }
            }
        }

        private void AtualizarOrigensDestinosEscala(Dominio.Entidades.Embarcador.Escalas.GeracaoEscala geracaoEscala, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Escalas.EscalaOrigemDestino repositorioEscalaOrigemDestino = new Repositorio.Embarcador.Escalas.EscalaOrigemDestino(unitOfWork);
            List<Dominio.Entidades.Embarcador.Escalas.EscalaOrigemDestino> origensDestinos = repositorioEscalaOrigemDestino.BuscarPorGeracaoEscala(geracaoEscala.Codigo);
            dynamic origensDestinosEscala = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("OrigensDestinosEscala"));

            ExcluirVeiculosEscalaRemovidos(geracaoEscala, origensDestinos, origensDestinosEscala, unitOfWork);
            AtualizarOrigensDestinosEscalaAlteradas(geracaoEscala, origensDestinos, origensDestinosEscala, unitOfWork);
        }

        private void ExcluirVeiculosEscalaRemovidos(Dominio.Entidades.Embarcador.Escalas.GeracaoEscala geracaoEscala, List<Dominio.Entidades.Embarcador.Escalas.EscalaOrigemDestino> origensDestinos, dynamic origensDestinosEscala, Repositorio.UnitOfWork unitOfWork)
        {
            if (origensDestinos.Count == 0)
                return;

            Repositorio.Embarcador.Escalas.EscalaVeiculoEscalado repositorioEscalaVeiculoEscalado = new Repositorio.Embarcador.Escalas.EscalaVeiculoEscalado(unitOfWork);

            foreach (var origemDestinoEscala in origensDestinosEscala)
            {
                int codigoOrigemDestinoEscala = ((string)origemDestinoEscala.Codigo).ToInt();
                Dominio.Entidades.Embarcador.Escalas.EscalaOrigemDestino origemDestinoEscalaAtualizar = (from o in origensDestinos where o.Codigo == codigoOrigemDestinoEscala select o).FirstOrDefault();

                if (origemDestinoEscalaAtualizar == null)
                    continue;

                List<int> listaCodigoVeiculoEscaladoAtualizado = new List<int>();

                foreach (var veiculoEscalado in origemDestinoEscala.VeiculosEscalados)
                    listaCodigoVeiculoEscaladoAtualizado.Add(((string)veiculoEscalado.Codigo).ToInt());

                List<Dominio.Entidades.Embarcador.Escalas.EscalaVeiculoEscalado> listaVeiculoEscaladoRemover = (from o in origemDestinoEscalaAtualizar.EscalaVeiculosEscalados where !listaCodigoVeiculoEscaladoAtualizado.Contains(o.Codigo) select o).ToList();

                foreach (var veiculoEscaladoRemover in listaVeiculoEscaladoRemover)
                {
                    veiculoEscaladoRemover.Motoristas?.Clear();
                    veiculoEscaladoRemover.Veiculos?.Clear();
                    repositorioEscalaVeiculoEscalado.Deletar(veiculoEscaladoRemover);
                }
            }
        }

        private void GerarEscalasVeiculos(Dominio.Entidades.Embarcador.Escalas.GeracaoEscala geracaoEscala, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Escalas.EscalaVeiculoEscalado repositorioEscalaVeiculoEscalado = new Repositorio.Embarcador.Escalas.EscalaVeiculoEscalado(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Produtos.GrupoProdutoTipoCarga repositorioGrupoProdutoTipoCarga = new Repositorio.Embarcador.Produtos.GrupoProdutoTipoCarga(unitOfWork);
            Repositorio.Embarcador.Escalas.EscalaOrigemDestino repositorioEscalaOrigemDestino = new Repositorio.Embarcador.Escalas.EscalaOrigemDestino(unitOfWork);
            Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork);
            Servicos.Embarcador.Logistica.CargaJanelaCarregamento servicoCargaJanelaCarregamento = new Servicos.Embarcador.Logistica.CargaJanelaCarregamento(unitOfWork);
            Servicos.Embarcador.Carga.CargaDadosSumarizados servicoCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);
            Servicos.Embarcador.Logistica.PrazoSituacaoCarga servicoPrazoSituacaoCarga = new Servicos.Embarcador.Logistica.PrazoSituacaoCarga(unitOfWork);
            List<Dominio.Entidades.Embarcador.Escalas.EscalaOrigemDestino> origensDestinos = repositorioEscalaOrigemDestino.BuscarPorGeracaoEscala(geracaoEscala.Codigo);

            foreach (Dominio.Entidades.Embarcador.Escalas.EscalaOrigemDestino escalaOrigemDestino in origensDestinos)
            {
                foreach (Dominio.Entidades.Embarcador.Escalas.EscalaVeiculoEscalado escalaVeiculoEscalado in escalaOrigemDestino.EscalaVeiculosEscalados)
                {
                    Dominio.Entidades.Embarcador.Cargas.Carga carga = new Dominio.Entidades.Embarcador.Cargas.Carga();

                    carga.CargaDePreCarga = true;
                    carga.Empresa = escalaVeiculoEscalado.Empresa;
                    carga.ModeloVeicularCarga = escalaVeiculoEscalado.ModeloVeicularCarga;
                    carga.Filial = escalaVeiculoEscalado.EscalaOrigemDestino.CentroCarregamento.Filial;
                    carga.Motoristas = new List<Dominio.Entidades.Usuario>() { escalaVeiculoEscalado.Motoristas.FirstOrDefault() };
                    carga.Rota = escalaVeiculoEscalado.EscalaOrigemDestino.Rota;

                    if (escalaVeiculoEscalado.EscalaOrigemDestino.ExpedicaoEscala.ProdutoEmbarcador?.GrupoProduto != null)
                    {
                        List<Dominio.Entidades.Embarcador.Produtos.GrupoProdutoTipoCarga> grupoProdutoTiposCarga = repositorioGrupoProdutoTipoCarga.ConsultarPorGrupoProduto(escalaVeiculoEscalado.EscalaOrigemDestino.ExpedicaoEscala.ProdutoEmbarcador.GrupoProduto.Codigo);

                        carga.TipoDeCarga = (from obj in grupoProdutoTiposCarga where obj.Posicao == 1 select obj.TipoDeCarga).Distinct().FirstOrDefault();
                    }

                    Dominio.Entidades.Veiculo veiculoEscala = escalaVeiculoEscalado.Veiculos.FirstOrDefault();

                    if (veiculoEscala.IsTipoVeiculoTracao())
                    {
                        carga.Veiculo = veiculoEscala;
                        carga.VeiculosVinculados = veiculoEscala.VeiculosVinculados?.ToList();
                    }
                    else
                    {
                        Dominio.Entidades.Veiculo tracao = veiculoEscala.VeiculosTracao?.FirstOrDefault();

                        if (tracao != null)
                        {
                            carga.Veiculo = tracao;
                            carga.VeiculosVinculados = new List<Dominio.Entidades.Veiculo>() { veiculoEscala };
                        }
                        else
                            carga.Veiculo = veiculoEscala;
                    }

                    repositorioCarga.Inserir(carga, Auditado);

                    escalaVeiculoEscalado.CargaJanelaCarregamento = servicoCargaJanelaCarregamento.AdicionarPorCarga(carga.Codigo, TipoServicoMultisoftware);

                    if (escalaVeiculoEscalado.CargaJanelaCarregamento != null)
                    {
                        escalaVeiculoEscalado.CargaJanelaCarregamento.Excedente = false;
                        servicoCargaJanelaCarregamento.AlterarSituacao(escalaVeiculoEscalado.CargaJanelaCarregamento, SituacaoCargaJanelaCarregamento.ProntaParaCarregamento);
                    }

                    servicoFluxoGestaoPatio.Adicionar(carga, TipoServicoMultisoftware, escalaVeiculoEscalado.CargaJanelaCarregamento);
                    repositorioEscalaVeiculoEscalado.Atualizar(escalaVeiculoEscalado);
                }
            }
        }

        private bool IsHorarioCarregamentoDisponivel(List<Dominio.ObjetosDeValor.Embarcador.Escalas.GeracaoEscalaHorarioCarregamento> listaHorarioCarregamento, Dominio.ObjetosDeValor.Embarcador.Escalas.GeracaoEscalaHorarioCarregamento geracaoEscalaHorarioCarregamento, Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento periodoCarregamento)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Escalas.GeracaoEscalaHorarioCarregamento> listaHorarioCarregamentoConflitante = (
                from o in listaHorarioCarregamento
                where (
                    (
                        (geracaoEscalaHorarioCarregamento.InicioCarregamento >= o.InicioCarregamento && geracaoEscalaHorarioCarregamento.InicioCarregamento < o.TerminoCarregamento) ||
                        (geracaoEscalaHorarioCarregamento.TerminoCarregamento > o.InicioCarregamento && geracaoEscalaHorarioCarregamento.TerminoCarregamento <= o.TerminoCarregamento)
                    ) ||
                    (
                        (o.InicioCarregamento >= geracaoEscalaHorarioCarregamento.InicioCarregamento && o.InicioCarregamento < geracaoEscalaHorarioCarregamento.TerminoCarregamento) ||
                        (o.TerminoCarregamento > geracaoEscalaHorarioCarregamento.InicioCarregamento && o.TerminoCarregamento <= geracaoEscalaHorarioCarregamento.TerminoCarregamento)
                    )
                )
                select o
            ).ToList();

            return listaHorarioCarregamentoConflitante.Count() < periodoCarregamento.CapacidadeCarregamentoSimultaneo;
        }

        private bool IsPossuiRestricaoRodagem(Dominio.Entidades.Embarcador.Escalas.GeracaoEscalaRestricaoModeloVeicular restricao, Dominio.ObjetosDeValor.Embarcador.Escalas.GeracaoEscalaHorarioCarregamento geracaoEscalaHorarioCarregamento)
        {
            if (restricao == null)
                return false;

            return (
                (
                    (geracaoEscalaHorarioCarregamento.InicioCarregamento >= restricao.HoraInicioRestricao && geracaoEscalaHorarioCarregamento.InicioCarregamento < restricao.HoraFimRestricao) ||
                    (geracaoEscalaHorarioCarregamento.TerminoCarregamento > restricao.HoraInicioRestricao && geracaoEscalaHorarioCarregamento.TerminoCarregamento <= restricao.HoraFimRestricao)
                ) ||
                (
                    (restricao.HoraInicioRestricao >= geracaoEscalaHorarioCarregamento.InicioCarregamento && restricao.HoraInicioRestricao < geracaoEscalaHorarioCarregamento.TerminoCarregamento) ||
                    (restricao.HoraFimRestricao > geracaoEscalaHorarioCarregamento.InicioCarregamento && restricao.HoraFimRestricao <= geracaoEscalaHorarioCarregamento.TerminoCarregamento)
                )
            );
        }

        private TimeSpan? ObterHorarioCarregamento(List<Dominio.ObjetosDeValor.Embarcador.Escalas.GeracaoEscalaHorarioCarregamento> listaHorarioCarregamento, List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento> periodosCarregamento, List<Dominio.Entidades.Embarcador.Escalas.GeracaoEscalaRestricaoModeloVeicular> restricoes, Dominio.Entidades.Embarcador.Escalas.EscalaOrigemDestino origemDestino, Dominio.Entidades.Veiculo veiculo, Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta servicoCargaJanelaCarregamentoConsulta = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta(unitOfWork, configuracao);

            List<Dominio.ObjetosDeValor.Embarcador.Escalas.GeracaoEscalaHorarioCarregamento> listaHorarioCarregamentoPorCentroCarregamento = (
                from o in listaHorarioCarregamento
                where o.CodigoCentroCarregamento == origemDestino.CentroCarregamento.Codigo
                orderby o.TerminoCarregamento ascending
                select o
            ).ToList();

            Dominio.Entidades.Embarcador.Escalas.GeracaoEscalaRestricaoModeloVeicular restricaoPorModeloVeicularCarga = (
                from o in restricoes
                where o.ModeloVeicularCarga.Codigo == veiculo.ModeloVeicularCarga.Codigo
                select o
            ).FirstOrDefault();

            foreach (Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento periodoCarregamento in periodosCarregamento)
            {
                int tempoCarregamento = servicoCargaJanelaCarregamentoConsulta.ObterTempoCarregamento(tipoCarga, veiculo.ModeloVeicularCarga, origemDestino.CentroCarregamento, periodoCarregamento.HoraInicio);

                if (tempoCarregamento == 0)
                    return null;

                Dominio.ObjetosDeValor.Embarcador.Escalas.GeracaoEscalaHorarioCarregamento geracaoEscalaHorarioCarregamento = new Dominio.ObjetosDeValor.Embarcador.Escalas.GeracaoEscalaHorarioCarregamento()
                {
                    CodigoCentroCarregamento = origemDestino.CentroCarregamento.Codigo,
                    InicioCarregamento = periodoCarregamento.HoraInicio,
                    TerminoCarregamento = periodoCarregamento.HoraInicio.Add(TimeSpan.FromMinutes(tempoCarregamento))
                };

                foreach (Dominio.ObjetosDeValor.Embarcador.Escalas.GeracaoEscalaHorarioCarregamento horarioCarregamento in listaHorarioCarregamentoPorCentroCarregamento)
                {
                    if (IsHorarioCarregamentoDisponivel(listaHorarioCarregamentoPorCentroCarregamento, geracaoEscalaHorarioCarregamento, periodoCarregamento))
                        break;

                    tempoCarregamento = servicoCargaJanelaCarregamentoConsulta.ObterTempoCarregamento(tipoCarga, veiculo.ModeloVeicularCarga, origemDestino.CentroCarregamento, horarioCarregamento.TerminoCarregamento);
                    geracaoEscalaHorarioCarregamento.InicioCarregamento = horarioCarregamento.TerminoCarregamento;
                    geracaoEscalaHorarioCarregamento.TerminoCarregamento = geracaoEscalaHorarioCarregamento.InicioCarregamento.Add(TimeSpan.FromMinutes(tempoCarregamento));
                }

                if (IsPossuiRestricaoRodagem(restricaoPorModeloVeicularCarga, geracaoEscalaHorarioCarregamento))
                {
                    List<Dominio.ObjetosDeValor.Embarcador.Escalas.GeracaoEscalaHorarioCarregamento> listaHorarioCarregamentoPorCentroCarregamentoPosteriorRestricao = (
                        from o in listaHorarioCarregamentoPorCentroCarregamento
                        where o.InicioCarregamento >= restricaoPorModeloVeicularCarga.HoraFimRestricao || o.TerminoCarregamento > restricaoPorModeloVeicularCarga.HoraFimRestricao
                        orderby o.TerminoCarregamento ascending
                        select o
                    ).ToList();

                    tempoCarregamento = servicoCargaJanelaCarregamentoConsulta.ObterTempoCarregamento(tipoCarga, veiculo.ModeloVeicularCarga, origemDestino.CentroCarregamento, restricaoPorModeloVeicularCarga.HoraFimRestricao);
                    geracaoEscalaHorarioCarregamento.InicioCarregamento = restricaoPorModeloVeicularCarga.HoraFimRestricao;
                    geracaoEscalaHorarioCarregamento.TerminoCarregamento = geracaoEscalaHorarioCarregamento.InicioCarregamento.Add(TimeSpan.FromMinutes(tempoCarregamento));

                    foreach (Dominio.ObjetosDeValor.Embarcador.Escalas.GeracaoEscalaHorarioCarregamento horarioCarregamento in listaHorarioCarregamentoPorCentroCarregamentoPosteriorRestricao)
                    {
                        if (IsHorarioCarregamentoDisponivel(listaHorarioCarregamentoPorCentroCarregamentoPosteriorRestricao, geracaoEscalaHorarioCarregamento, periodoCarregamento))
                            break;

                        tempoCarregamento = servicoCargaJanelaCarregamentoConsulta.ObterTempoCarregamento(tipoCarga, veiculo.ModeloVeicularCarga, origemDestino.CentroCarregamento, horarioCarregamento.TerminoCarregamento);
                        geracaoEscalaHorarioCarregamento.InicioCarregamento = horarioCarregamento.TerminoCarregamento;
                        geracaoEscalaHorarioCarregamento.TerminoCarregamento = geracaoEscalaHorarioCarregamento.InicioCarregamento.Add(TimeSpan.FromMinutes(tempoCarregamento));
                    }
                }

                if (geracaoEscalaHorarioCarregamento.TerminoCarregamento <= periodoCarregamento.HoraTermino.Add(TimeSpan.FromMinutes(periodoCarregamento.ToleranciaExcessoTempo)))
                {
                    listaHorarioCarregamento.Add(geracaoEscalaHorarioCarregamento);
                    return geracaoEscalaHorarioCarregamento.InicioCarregamento;
                }
            }

            return null;
        }

        private dynamic ObterOrigensDestinosEscala(Dominio.Entidades.Embarcador.Escalas.GeracaoEscala geracaoEscala, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Escalas.EscalaOrigemDestino repositorioEscalaOrigemDestino = new Repositorio.Embarcador.Escalas.EscalaOrigemDestino(unitOfWork);
            List<Dominio.Entidades.Embarcador.Escalas.EscalaOrigemDestino> origensDestinos = repositorioEscalaOrigemDestino.BuscarPorGeracaoEscala(geracaoEscala.Codigo);

            return (
                from origemDestino in origensDestinos
                select new
                {
                    origemDestino.Codigo,
                    Origem = origemDestino.CentroCarregamento.Descricao,
                    Destino = origemDestino.ClienteDestino?.Descricao ?? "",
                    Quantidade = origemDestino.Quantidade.ToString("n2"),
                    Produto = origemDestino.ExpedicaoEscala.ProdutoEmbarcador?.Descricao ?? "",
                    UnidadeMedida = origemDestino.ExpedicaoEscala.ProdutoEmbarcador?.Unidade?.Descricao ?? "Unidade",
                    VeiculosEscalados = (
                        from veiculoEscalado in origemDestino.EscalaVeiculosEscalados
                        select new
                        {
                            veiculoEscalado.Codigo,
                            CodigoEmpresa = veiculoEscalado.Empresa?.Codigo ?? 0,
                            CodigoModeloVeicularCarga = veiculoEscalado.ModeloVeicularCarga.Codigo,
                            CodigoMotorista = veiculoEscalado.Motoristas?.FirstOrDefault()?.Codigo ?? 0,
                            CodigoVeiculo = veiculoEscalado.Veiculos?.FirstOrDefault()?.Codigo ?? 0,
                            ModeloVeicularCarga = veiculoEscalado.ModeloVeicularCarga.Descricao,
                            Quantidade = veiculoEscalado.Quantidade.ToString("n2"),
                            Empresa = veiculoEscalado.Empresa?.RazaoSocial ?? "",
                            Veiculo = veiculoEscalado.Veiculos?.FirstOrDefault()?.Placa ?? "",
                            HoraCarregamento = veiculoEscalado.HoraCarregamento.ToString(@"hh\:mm"),
                            Motorista = veiculoEscalado.Motoristas?.FirstOrDefault()?.Nome ?? ""
                        }
                    ).ToList()
                }
            ).ToList();
        }

        #endregion

        #region Métodos Privados da Exportacao

        

        #endregion
    }
}
