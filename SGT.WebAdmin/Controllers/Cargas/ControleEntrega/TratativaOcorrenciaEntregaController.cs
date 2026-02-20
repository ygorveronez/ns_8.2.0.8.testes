using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
namespace SGT.WebAdmin.Controllers.Cargas.ControleEntrega
{
    [CustomAuthorize("Cargas/TratativaOcorrenciaEntrega")]
    public class TratativaOcorrenciaEntregaController : BaseController
    {
		#region Construtores

		public TratativaOcorrenciaEntregaController(Conexao conexao) : base(conexao) { }

		#endregion


        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.ControleEntrega.TratativaOcorrenciaEntrega repTratativaOcorrenciaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.TratativaOcorrenciaEntrega(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.TratativaOcorrenciaEntrega tratativaOcorrenciaEntrega = new Dominio.Entidades.Embarcador.Cargas.ControleEntrega.TratativaOcorrenciaEntrega();

                PreencherEntidade(tratativaOcorrenciaEntrega, unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.TratativaOcorrenciaEntrega tratativaExiste = repTratativaOcorrenciaEntrega.ValidarSeExiste(tratativaOcorrenciaEntrega.DevolucaoParcial, tratativaOcorrenciaEntrega.TipoDeOcorrencia.Codigo, tratativaOcorrenciaEntrega.TratativaDevolucao);

                if (tratativaExiste == null)
                {
                    unitOfWork.Start();
                    repTratativaOcorrenciaEntrega.Inserir(tratativaOcorrenciaEntrega, Auditado);
                    unitOfWork.CommitChanges();
                    return new JsonpResult(true);
                }
                else
                {
                    return new JsonpResult(false, true, "Já existe uma tratativa criada para os parâmetros informados.");
                }

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
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.ControleEntrega.TratativaOcorrenciaEntrega repTratativaOcorrenciaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.TratativaOcorrenciaEntrega(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.TratativaOcorrenciaEntrega tratativaOcorrenciaEntrega = repTratativaOcorrenciaEntrega.BuscarPorCodigo(codigo, true);

                if (tratativaOcorrenciaEntrega == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherEntidade(tratativaOcorrenciaEntrega, unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.TratativaOcorrenciaEntrega tratativaExiste = repTratativaOcorrenciaEntrega.ValidarSeExiste(tratativaOcorrenciaEntrega.DevolucaoParcial, tratativaOcorrenciaEntrega.TipoDeOcorrencia.Codigo, tratativaOcorrenciaEntrega.TratativaDevolucao);
                if (tratativaExiste == null || tratativaExiste.Codigo == tratativaOcorrenciaEntrega.Codigo)
                {
                    unitOfWork.Start();
                    repTratativaOcorrenciaEntrega.Atualizar(tratativaOcorrenciaEntrega, Auditado);
                    unitOfWork.CommitChanges();
                    return new JsonpResult(true);
                }
                else
                {
                    return new JsonpResult(false, true, "Já existe uma configuração criada para os parametros informados.");
                }
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

                Repositorio.Embarcador.Cargas.ControleEntrega.TratativaOcorrenciaEntrega repTratativaOcorrenciaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.TratativaOcorrenciaEntrega(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.TratativaOcorrenciaEntrega tratativaOcorrenciaEntrega = repTratativaOcorrenciaEntrega.BuscarPorCodigo(codigo, false);

                if (tratativaOcorrenciaEntrega == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    tratativaOcorrenciaEntrega.Codigo,
                    tratativaOcorrenciaEntrega.TratativaDevolucao,
                    tratativaOcorrenciaEntrega.DevolucaoParcial,
                    TipoOcorrencia = new
                    {
                        tratativaOcorrenciaEntrega.TipoDeOcorrencia.Codigo,
                        tratativaOcorrenciaEntrega.TipoDeOcorrencia.Descricao
                    }
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
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.ControleEntrega.TratativaOcorrenciaEntrega reptratativaOcorrenciaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.TratativaOcorrenciaEntrega(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.TratativaOcorrenciaEntrega tratativaOcorrenciaEntrega = reptratativaOcorrenciaEntrega.BuscarPorCodigo(codigo, true);

                if (tratativaOcorrenciaEntrega == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();
                reptratativaOcorrenciaEntrega.Deletar(tratativaOcorrenciaEntrega, Auditado);
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
        public async Task<IActionResult> BuscarTiposTratativas()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega> tipos = null;
                if (!string.IsNullOrWhiteSpace(Request.Params("Tipos")))
                    tipos = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega>>(Request.Params("Tipos"));

                Repositorio.Embarcador.Chamados.TratativasAnaliseDevolucao repTratativasAnaliseDevolucao = new Repositorio.Embarcador.Chamados.TratativasAnaliseDevolucao(unidadeDeTrabalho);

                List<Dominio.Entidades.Embarcador.Chamados.TratativasAnaliseDevolucao> trataticasDevolucao = repTratativasAnaliseDevolucao.BuscarPorTipos(tipos);

                return new JsonpResult((from obj in trataticasDevolucao
                                        orderby obj.TratativaDevolucao
                                        select new
                                        {
                                            Codigo = obj.TratativaDevolucao,
                                            Descricao = obj.TratativaDevolucao.ObterDescricaoTratativaDevolucao()
                                        }).ToList()); ;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao obter os tipos de integração.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherEntidade(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.TratativaOcorrenciaEntrega configuracaoOcorrenciaEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.TipoDeOcorrenciaDeCTe repTipoDeOcorrenciaDeCTe = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);
            configuracaoOcorrenciaEntrega.DevolucaoParcial = Request.GetBoolParam("TipoDevolucao");
            configuracaoOcorrenciaEntrega.TipoDeOcorrencia = repTipoDeOcorrenciaDeCTe.BuscarPorCodigo(Request.GetIntParam("TipoOcorrencia"));
            configuracaoOcorrenciaEntrega.TratativaDevolucao = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega>("Tratativa");
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

                int tipoOcorrencia = Request.GetIntParam("TipoOcorrencia");

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Tipo de Ocorrência", "TipoOcorrencia", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tratativa", "Tratativa", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo Devolução", "DevolucaoParcial", 5, Models.Grid.Align.left, true);

                string propriedadeOrdenar = ObterPropriedadeOrdenar(grid.header[grid.indiceColunaOrdena].data);

                Repositorio.Embarcador.Cargas.ControleEntrega.TratativaOcorrenciaEntrega repConfiguracaoOcorrenciaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.TratativaOcorrenciaEntrega(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.TratativaOcorrenciaEntrega> listaConfiguracaoOcorrenciaEntrega = repConfiguracaoOcorrenciaEntrega.Consultar(tipoOcorrencia, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repConfiguracaoOcorrenciaEntrega.ContarConsulta(tipoOcorrencia);

                var retorno = listaConfiguracaoOcorrenciaEntrega.Select(tratativa => new
                {
                    tratativa.Codigo,
                    Tratativa = tratativa.TratativaDevolucao.ObterDescricaoTratativaDevolucao(),
                    DevolucaoParcial = tratativa.DevolucaoParcial ? "Parcial" : "Total",
                    TipoOcorrencia = tratativa.TipoDeOcorrencia.Descricao
                }).ToList();

                grid.AdicionaRows(retorno);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch
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
            if (propriedadeOrdenar == "DescricaoAtivo")
                return "Ativo";

            return propriedadeOrdenar;
        }

        #endregion
    }
}
