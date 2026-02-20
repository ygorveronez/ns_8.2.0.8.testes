using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/TrechoBalsa")]
    public class TrechoBalsaController : BaseController
    {
		#region Construtores

		public TrechoBalsaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

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

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Logistica.TrechoBalsa repTrechoBalsa = new Repositorio.Embarcador.Logistica.TrechoBalsa(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.TrechoBalsa trechoBalsa = new Dominio.Entidades.Embarcador.Logistica.TrechoBalsa();

                PreencherDados(trechoBalsa, unitOfWork);

                if (!repTrechoBalsa.ValidarDuplicidadeCliente(trechoBalsa))
                    return new JsonpResult(false, true, "Já existe um registro com o mesmo Porto Origem e Destino informados.");

                repTrechoBalsa.Inserir(trechoBalsa, Auditado);

                SalvarTemposBalsa(trechoBalsa, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
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

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Logistica.TrechoBalsa repTrechoBalsa = new Repositorio.Embarcador.Logistica.TrechoBalsa(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.TrechoBalsa trechoBalsa = repTrechoBalsa.BuscarPorCodigo(codigo);

                if (trechoBalsa == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherDados(trechoBalsa, unitOfWork);

                if (!repTrechoBalsa.ValidarDuplicidadeCliente(trechoBalsa))
                    return new JsonpResult(false, true, "Já existe um registro com o mesmo Porto Origem e Destino informados.");

                repTrechoBalsa.Atualizar(trechoBalsa, Auditado);

                SalvarTemposBalsa(trechoBalsa, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
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

                Repositorio.Embarcador.Logistica.TrechoBalsa repTrechoBalsa = new Repositorio.Embarcador.Logistica.TrechoBalsa(unitOfWork);
                Repositorio.Embarcador.Logistica.TempoBalsa repTempoBalsa = new Repositorio.Embarcador.Logistica.TempoBalsa(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.TrechoBalsa trechoBalsa = repTrechoBalsa.BuscarPorCodigo(codigo);
                List<Dominio.Entidades.Embarcador.Logistica.TempoBalsa> listaTempoBalsa = repTempoBalsa.BuscarPorTrechoBalsa(trechoBalsa.Codigo);

                if (trechoBalsa == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    trechoBalsa.Codigo,
                    trechoBalsa.Descricao,
                    trechoBalsa.Distancia,
                    trechoBalsa.Area,
                    PortoOrigem = new
                    {
                        trechoBalsa.PortoOrigem?.Codigo,
                        trechoBalsa.PortoOrigem?.Descricao
                    },
                    PortoDestino = new
                    {
                        trechoBalsa.PortoDestino?.Codigo,
                        trechoBalsa.PortoDestino?.Descricao
                    },
                    ListaTempoBalsa = (from o in listaTempoBalsa
                                       select new
                                        {
                                            Codigo = o.Codigo,
                                            DataInicio = o.DataInicio.ToString("dd/MM/yyyy"),
                                            DataFinal = o.DataFinal.ToString("dd/MM/yyyy"),
                                            TempoGeral = o.TempoGeral
                                       }).ToList(),
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
                unitOfWork.Start();
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Logistica.TrechoBalsa repTrechoBalsa = new Repositorio.Embarcador.Logistica.TrechoBalsa(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.TrechoBalsa trechoBalsa = repTrechoBalsa.BuscarPorCodigo(codigo);

                if (trechoBalsa == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                repTrechoBalsa.Deletar(trechoBalsa, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                    unitOfWork.Rollback();
                if (ExcessaoPorPossuirDependeciasNoBanco(excecao))
                    return new JsonpResult(false, true, Localization.Resources.Cargas.TipoCarga.NaoFoiPossivelExcluirRegistroPoisMesmoJaPossuiVinculoComOutrosRecursosDoSistemaRecomendamosQueVoceInativeRegistroCasoNaoDesejaMaisUtilizaLo);
                else
                {
                    Servicos.Log.TratarErro(excecao);

                    return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
                }

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
                var grid = ObterGridPesquisa();

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

        #endregion

        #region Métodos Privados

        private void PreencherDados(Dominio.Entidades.Embarcador.Logistica.TrechoBalsa trechoBalsa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            double codigoPortoOrigem = Request.GetDoubleParam("PortoOrigem");
            double codigoPortoDestino = Request.GetDoubleParam("PortoDestino");

            var area = Request.Params("Area");

            trechoBalsa.Codigo = Request.GetIntParam("Codigo");
            trechoBalsa.Descricao = Request.GetStringParam("Descricao");
            trechoBalsa.Distancia = Request.GetDecimalParam("Distancia");
            trechoBalsa.Ativo = Request.GetBoolParam("Status");
            trechoBalsa.Area = area;
            trechoBalsa.PortoOrigem = repCliente.BuscarPorCPFCNPJ(codigoPortoOrigem);
            trechoBalsa.PortoDestino = repCliente.BuscarPorCPFCNPJ(codigoPortoDestino);
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Porto Origem", "PortoOrigem", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Porto Destino", "PortoDestino", 20, Models.Grid.Align.left, true);

                Repositorio.Embarcador.Logistica.TrechoBalsa repTrechoBalsa = new Repositorio.Embarcador.Logistica.TrechoBalsa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaTrechoBalsa filtrosPesquisa = ObterFiltrosPesquisa();

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                int totalRegistros = repTrechoBalsa.ContarConsulta(filtrosPesquisa);

                List<Dominio.Entidades.Embarcador.Logistica.TrechoBalsa> listaTrechoBalsa = totalRegistros > 0 ? repTrechoBalsa.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Logistica.TrechoBalsa>();

                var lista = (from p in listaTrechoBalsa
                             select new
                             {
                                 p.Codigo,
                                 p.Descricao,
                                 PortoOrigem = p.PortoOrigem.Descricao,
                                 PortoDestino = p.PortoDestino.Descricao,
                             }).ToList();

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            return propriedadeOrdenar;
        }

        private Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaTrechoBalsa ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaTrechoBalsa()
            {
                Descricao = Request.GetStringParam("Descricao"),
                CodigoPortoOrigem = Request.GetIntParam("PortoOrigem"),
                CodigoPortoDestino = Request.GetIntParam("PortoDestino"),
                Status = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa>("Status"),
            };
        }

        private void SalvarTemposBalsa(Dominio.Entidades.Embarcador.Logistica.TrechoBalsa trechoBalsa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.TempoBalsa repTempoBalsa = new Repositorio.Embarcador.Logistica.TempoBalsa(unitOfWork);

            var faixasAntigas = repTempoBalsa.BuscarPorTrechoBalsa(trechoBalsa.Codigo);

            foreach (var faixaAntiga in faixasAntigas)
            {
                repTempoBalsa.Deletar(faixaAntiga);
            }

            dynamic listaFaixas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ListaTempoBalsa"));

            foreach (var faixaDoFront in listaFaixas)
            {
                Dominio.Entidades.Embarcador.Logistica.TempoBalsa faixa = new Dominio.Entidades.Embarcador.Logistica.TempoBalsa();
                faixa.TrechoBalsa = trechoBalsa;
                faixa.DataInicio = Convert.ToDateTime((string)faixaDoFront.DataInicio);
                faixa.DataFinal = Convert.ToDateTime((string)faixaDoFront.DataFinal);
                faixa.TempoGeral = (int)faixaDoFront.TempoGeral;
                repTempoBalsa.Inserir(faixa);
            }
        }

        #endregion
    }
}
