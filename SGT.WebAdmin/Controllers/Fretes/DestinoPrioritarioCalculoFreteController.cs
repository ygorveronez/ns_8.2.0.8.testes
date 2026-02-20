using Newtonsoft.Json;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Fretes
{
    [CustomAuthorize("Fretes/DestinoPrioritarioCalculoFrete")]
    public class DestinoPrioritarioCalculoFreteController : BaseController
    {
		#region Construtores

		public DestinoPrioritarioCalculoFreteController(Conexao conexao) : base(conexao) { }

		#endregion


        #region Métodos Globais
        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisa();

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                // Busca Dados
                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Seta valores na grid
                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
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
                // Instancia repositorios
                Repositorio.Embarcador.Frete.DestinoPrioritarioCalculoFrete repDestinoPrioritarioCalculoFrete = new Repositorio.Embarcador.Frete.DestinoPrioritarioCalculoFrete(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                // Busca informacoes
                Dominio.Entidades.Embarcador.Frete.DestinoPrioritarioCalculoFrete prioridade = repDestinoPrioritarioCalculoFrete.BuscarPorCodigo(codigo, true);

                if (prioridade == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Formata retorno
                var retorno = new
                {
                    Dados = new {
                        prioridade.Codigo,
                        prioridade.Descricao,
                        prioridade.Ativo,
                    },
                    TabelasFrete = (from t in prioridade.TabelasFrete
                                    select new
                                    {
                                        t.Codigo,
                                        t.Descricao,
                                    }).ToList(),

                    Localidades = (from l in prioridade.Prioridades()
                                   select new
                                   {
                                       l.Codigo,
                                       l.Ordem,
                                       l.Ativo,
                                       Localidade = new { l.Localidade.Codigo, l.Localidade.Descricao }
                                   }).ToList()
                };

                // Retorna informacoes
                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
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
                // Instancia repositorios
                Repositorio.Embarcador.Frete.DestinoPrioritarioCalculoFrete repDestinoPrioritarioCalculoFrete = new Repositorio.Embarcador.Frete.DestinoPrioritarioCalculoFrete(unitOfWork);
                
                // Parametros
                int codigo = Request.GetIntParam("Codigo");

                // Busca informacoes
                Dominio.Entidades.Embarcador.Frete.DestinoPrioritarioCalculoFrete prioridade = repDestinoPrioritarioCalculoFrete.BuscarPorCodigo(codigo, true);

                // Valida
                if (prioridade == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                prioridade.Descricao = Request.GetStringParam("Descricao");
                prioridade.Ativo = Request.GetBoolParam("Ativo");

                if (!ValidaEntidade(prioridade, out string erro))
                    return new JsonpResult(false, true, erro);

                // Preenche entidade com dados
                unitOfWork.Start();
                PreencheEntidade(ref prioridade, unitOfWork);
                repDestinoPrioritarioCalculoFrete.Atualizar(prioridade, Auditado);
                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Frete.DestinoPrioritarioCalculoFrete repDestinoPrioritarioCalculoFrete = new Repositorio.Embarcador.Frete.DestinoPrioritarioCalculoFrete(unitOfWork);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Frete.DestinoPrioritarioCalculoFrete prioridade = new Dominio.Entidades.Embarcador.Frete.DestinoPrioritarioCalculoFrete
                {
                    Descricao = Request.GetStringParam("Descricao"),
                    Ativo = Request.GetBoolParam("Ativo")
                };

                // Valida entidade
                if (!ValidaEntidade(prioridade, out string erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                unitOfWork.Start();
                repDestinoPrioritarioCalculoFrete.Inserir(prioridade, Auditado);
                PreencheEntidade(ref prioridade, unitOfWork);
                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
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
                // Instancia repositorios
                Repositorio.Embarcador.Frete.DestinoPrioritarioCalculoFrete repDestinoPrioritarioCalculoFrete = new Repositorio.Embarcador.Frete.DestinoPrioritarioCalculoFrete(unitOfWork);

                // Parametros
                int codigo = Request.GetIntParam("Codigo");

                // Busca informacoes
                Dominio.Entidades.Embarcador.Frete.DestinoPrioritarioCalculoFrete prioridade = repDestinoPrioritarioCalculoFrete.BuscarPorCodigo(codigo, true);

                // Valida
                if (prioridade == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Persiste dados
                unitOfWork.Start();
                repDestinoPrioritarioCalculoFrete.Deletar(prioridade, Auditado);
                unitOfWork.CommitChanges();

                // Retorna informacoes
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        #endregion

        #region Métodos Privados
        private Models.Grid.Grid GridPesquisa()
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            // Cabecalhos grid
            grid.Prop("Codigo");
            grid.Prop("Descricao").Nome("Descrição").Tamanho(40).Align(Models.Grid.Align.left);
            grid.Prop("Ativo").Nome("Situação").Tamanho(10).Align(Models.Grid.Align.left);

            return grid;
        }

        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Frete.DestinoPrioritarioCalculoFrete repDestinoPrioritarioCalculoFrete = new Repositorio.Embarcador.Frete.DestinoPrioritarioCalculoFrete(unitOfWork);

            // Dados do filtro
            string descricao = Request.GetStringParam("Descricao");
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa>("Ativo");

            // Consulta
            List<Dominio.Entidades.Embarcador.Frete.DestinoPrioritarioCalculoFrete> listaGrid = repDestinoPrioritarioCalculoFrete.Consultar(descricao, status, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repDestinoPrioritarioCalculoFrete.ContarConsulta(descricao, status);

            var lista = from obj in listaGrid
                        select new
                        {
                            obj.Codigo,
                            obj.Descricao,
                            Ativo = obj.DescricaoAtivo
                        };

            return lista.ToList();
        }

        private void PropOrdena(ref string propOrdenar)
        {
        }

        private bool ValidaEntidade(Dominio.Entidades.Embarcador.Frete.DestinoPrioritarioCalculoFrete prioridade, out string msgErro)
        {
            msgErro = "";

            if (string.IsNullOrWhiteSpace(prioridade.Descricao))
            {
                msgErro = "Descrição é obrigatória.";
                return false;
            }

            return true;
        }

        /* PreencheEntidade
         * Recebe uma instancia da entidade
         * Converte parametros recebido por request
         * Atribui a entidade
         */
        private void PreencheEntidade(ref Dominio.Entidades.Embarcador.Frete.DestinoPrioritarioCalculoFrete prioridade, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
            Repositorio.Embarcador.Frete.TabelaFrete repTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFrete(unitOfWork);
            Repositorio.Embarcador.Frete.DestinoPrioritarioCalculoFreteLocalidade repDestinoPrioritarioCalculoFreteLocalidade = new Repositorio.Embarcador.Frete.DestinoPrioritarioCalculoFreteLocalidade(unitOfWork);

            if (prioridade.TabelasFrete == null)
                prioridade.TabelasFrete = new List<Dominio.Entidades.Embarcador.Frete.TabelaFrete>();

            List<int> codigos = JsonConvert.DeserializeObject<List<int>>(Request.GetStringParam("TabelasFrete"));
            List<Dominio.Entidades.Embarcador.Frete.TabelaFrete> tabelas = repTabelaFrete.BuscarTabelasParaPrioridadeCalculo(codigos);

            prioridade.TabelasFrete.Clear();
            foreach (var tabela in tabelas)
            {
                if (!prioridade.TabelasFrete.Contains(tabela))
                    prioridade.TabelasFrete.Add(tabela);
            }

            
            List<dynamic> dynLocalidades = JsonConvert.DeserializeObject<List<dynamic>>(Request.GetStringParam("Localidades"));
            List<int> localidadesRecebidas = new List<int>();
            foreach(var dynLocalidade in dynLocalidades)
            {
                int locCodigo = (int)dynLocalidade.Localidade;
                bool ativo = (bool)dynLocalidade.Ativo;
                int ordem = (int)dynLocalidade.Ordem;

                Dominio.Entidades.Embarcador.Frete.DestinoPrioritarioCalculoFreteLocalidade localidade = repDestinoPrioritarioCalculoFreteLocalidade.BuscarPorLocalidadeEConfiguracao(prioridade.Codigo, locCodigo);

                if (localidade == null)
                {
                    localidade = new Dominio.Entidades.Embarcador.Frete.DestinoPrioritarioCalculoFreteLocalidade()
                    {
                        Localidade = repLocalidade.BuscarPorCodigo(locCodigo),
                        DestinoPrioritarioCalculoFrete = prioridade
                    };
                }
                else
                    localidade.Initialize();

                localidade.Ativo = ativo;
                localidade.Ordem = ordem;

                if (localidade.Codigo > 0)
                    repDestinoPrioritarioCalculoFreteLocalidade.Atualizar(localidade, Auditado);
                else
                    repDestinoPrioritarioCalculoFreteLocalidade.Inserir(localidade, Auditado);
                localidadesRecebidas.Add(locCodigo);
            }

            List<Dominio.Entidades.Embarcador.Frete.DestinoPrioritarioCalculoFreteLocalidade> localidadesRemover = repDestinoPrioritarioCalculoFreteLocalidade.BuscarLocalidadeParaRemover(prioridade.Codigo, localidadesRecebidas);
            foreach (var loc in localidadesRemover)
                repDestinoPrioritarioCalculoFreteLocalidade.Deletar(loc, Auditado);
        }
        #endregion
    }
}
