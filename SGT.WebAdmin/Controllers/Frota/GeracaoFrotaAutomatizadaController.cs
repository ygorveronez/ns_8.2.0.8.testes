using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Frota
{
    public class GeracaoFrotaAutomatizadaController : BaseController
    {
		#region Construtores

		public GeracaoFrotaAutomatizadaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Frotas.GeracaoFrotaAutomatizada repositorioGeracaoFrotaAutomatizada = new Repositorio.Embarcador.Frotas.GeracaoFrotaAutomatizada(unitOfWork);
                Dominio.Entidades.Embarcador.Frotas.GeracaoFrotaAutomatizada geracaoFrotaAutomatizada = new Dominio.Entidades.Embarcador.Frotas.GeracaoFrotaAutomatizada();

                geracaoFrotaAutomatizada.Descricao = Request.GetStringParam("Descricao");

                repositorioGeracaoFrotaAutomatizada.Inserir(geracaoFrotaAutomatizada, Auditado);

                AtualizarFiliais(geracaoFrotaAutomatizada, unitOfWork);
                AtualizarTipoOperacoes(geracaoFrotaAutomatizada, unitOfWork);
                AtualizarModelosVeicularesCarga(geracaoFrotaAutomatizada, unitOfWork);

                repositorioGeracaoFrotaAutomatizada.Atualizar(geracaoFrotaAutomatizada);

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
                int codigoGeracaoFrotaAutomatizada = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Frotas.GeracaoFrotaAutomatizada repositorioGeracaoFrotaAutomatizada = new Repositorio.Embarcador.Frotas.GeracaoFrotaAutomatizada(unitOfWork);
                Dominio.Entidades.Embarcador.Frotas.GeracaoFrotaAutomatizada geracaoFrotaAutomatizada = repositorioGeracaoFrotaAutomatizada.BuscarPorCodigo(codigoGeracaoFrotaAutomatizada, auditavel: true);

                if (geracaoFrotaAutomatizada == null)
                    return new JsonpResult(false, "Configuração Não Encontrada");

                geracaoFrotaAutomatizada.Descricao = Request.GetStringParam("Descricao");

                AtualizarFiliais(geracaoFrotaAutomatizada, unitOfWork);
                AtualizarTipoOperacoes(geracaoFrotaAutomatizada, unitOfWork);
                AtualizarModelosVeicularesCarga(geracaoFrotaAutomatizada, unitOfWork);

                repositorioGeracaoFrotaAutomatizada.Atualizar(geracaoFrotaAutomatizada, Auditado);

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
                int codigoGeracaoFrotaAutomatizada = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Frotas.GeracaoFrotaAutomatizada repositorioGeracaoFrotaAutomatizada = new Repositorio.Embarcador.Frotas.GeracaoFrotaAutomatizada(unitOfWork);
                Dominio.Entidades.Embarcador.Frotas.GeracaoFrotaAutomatizada geracaoFrotaAutomatizada = repositorioGeracaoFrotaAutomatizada.BuscarPorCodigo(codigoGeracaoFrotaAutomatizada);

                if (geracaoFrotaAutomatizada == null)
                    return new JsonpResult(false, "Configuração");

                dynamic retorno = new
                {
                    geracaoFrotaAutomatizada.Codigo,
                    geracaoFrotaAutomatizada.Descricao,
                    Filiais = (from obj in geracaoFrotaAutomatizada.Filiais
                               select new
                               {
                                   obj.Codigo,
                                   obj.Descricao
                               }).ToList(),
                    TipoOperacoes = (from obj in geracaoFrotaAutomatizada.TipoOperacoes
                               select new
                               {
                                   obj.Codigo,
                                   obj.Descricao
                               }).ToList(),
                    ModeloVeicular = (from obj in geracaoFrotaAutomatizada.ModelosVeicularesCarga
                               select new
                               {
                                   obj.Codigo,
                                   obj.Descricao
                               }).ToList(),
                };

                return new JsonpResult(retorno);
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
                int codigoGeracaoFrotaAutomatizada = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Frotas.GeracaoFrotaAutomatizada repositorioGeracaoFrotaAutomatizada = new Repositorio.Embarcador.Frotas.GeracaoFrotaAutomatizada(unitOfWork);
                Dominio.Entidades.Embarcador.Frotas.GeracaoFrotaAutomatizada geracaoFrotaAutomatizada = repositorioGeracaoFrotaAutomatizada.BuscarPorCodigo(codigoGeracaoFrotaAutomatizada);

                if (geracaoFrotaAutomatizada == null)
                    return new JsonpResult(false, "Configuração Não Encontrada");

                repositorioGeracaoFrotaAutomatizada.Deletar(geracaoFrotaAutomatizada, Auditado);

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
        public async Task<IActionResult> VerificarSeExisteConfiguracaoCadastrada()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                var repGeracaoFrotaAutomatizada = new Repositorio.Embarcador.Frotas.GeracaoFrotaAutomatizada(unitOfWork);
                var codFilial = Request.GetIntParam("CodigoFilial");
                var existe = repGeracaoFrotaAutomatizada.ExisteConfiguracaoCadastrada(codFilial);
                return new JsonpResult(existe);
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

        public async Task<IActionResult> GerarListaMensal()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigoGeracaoFrotaAutomatizada = Request.GetIntParam("CodigoGeracaoFrotaAutomatizada");
                int ano = Request.GetIntParam("Ano");
                int mes = Request.GetIntParam("Mes");
                Servicos.Embarcador.Frotas.PlanejamentoFrotaMes servicoSugestaoMensal = new Servicos.Embarcador.Frotas.PlanejamentoFrotaMes(unitOfWork, Auditado);

                servicoSugestaoMensal.GerarSugestaoFrota(codigoGeracaoFrotaAutomatizada, ano, mes);

                unitOfWork.CommitChanges();

                return new JsonpResult(true, "Lista mensal gerada com sucesso");
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
                return new JsonpResult(false, "Ocorreu uma falha ao gerar a lista mensal");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion Métodos Globais

        #region Métodos Privados

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string descricao = Request.GetStringParam("Descricao");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 50, Models.Grid.Align.left, true);
                
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Frotas.GeracaoFrotaAutomatizada repGeracaoFrotaAutomatizada = new Repositorio.Embarcador.Frotas.GeracaoFrotaAutomatizada(unitOfWork);
                List<Dominio.Entidades.Embarcador.Frotas.GeracaoFrotaAutomatizada> listaGeracaoFrotaAutomatizada = repGeracaoFrotaAutomatizada.Consultar(descricao, parametrosConsulta);
                int totalRegistros = repGeracaoFrotaAutomatizada.ContarConsulta(descricao);

                var listaGeracaoFrotaAutomatizadaRetornar = (
                    from GeracaoFrotaAutomatizada in listaGeracaoFrotaAutomatizada
                    select new
                    {
                        GeracaoFrotaAutomatizada.Codigo,
                        GeracaoFrotaAutomatizada.Descricao
                    }
                ).ToList();

                grid.AdicionaRows(listaGeracaoFrotaAutomatizadaRetornar);
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

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "Descricao")
                return "Descricao";

            return propriedadeOrdenar;
        }

        private void AtualizarFiliais(Dominio.Entidades.Embarcador.Frotas.GeracaoFrotaAutomatizada geracaoFrotaAutomatizada, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
            dynamic filiais = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Filiais"));

            if (geracaoFrotaAutomatizada.Filiais == null)
                geracaoFrotaAutomatizada.Filiais = new List<Dominio.Entidades.Embarcador.Filiais.Filial>();
            else
                geracaoFrotaAutomatizada.Filiais.Clear();

            foreach (var filial in filiais)
            {
                Dominio.Entidades.Embarcador.Filiais.Filial filialAdicionar = repFilial.BuscarPorCodigo((int)filial.Codigo) ?? throw new ControllerException("Filiar não encontrada");
                geracaoFrotaAutomatizada.Filiais.Add(filialAdicionar);
            }
        }

        private void AtualizarTipoOperacoes(Dominio.Entidades.Embarcador.Frotas.GeracaoFrotaAutomatizada geracaoFrotaAutomatizada, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            dynamic tipoOperacoes = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("TipoOperacoes"));

            if (geracaoFrotaAutomatizada.TipoOperacoes == null)
                geracaoFrotaAutomatizada.TipoOperacoes = new List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();
            else
                geracaoFrotaAutomatizada.TipoOperacoes.Clear();

            foreach (var tipoOperacao in tipoOperacoes)
            {
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacaoAdicionar = repTipoOperacao.BuscarPorCodigo((int)tipoOperacao.Codigo) ?? throw new ControllerException("Tipo de Operação não encontrada");
                geracaoFrotaAutomatizada.TipoOperacoes.Add(tipoOperacaoAdicionar);
            }
        }

        private void AtualizarModelosVeicularesCarga(Dominio.Entidades.Embarcador.Frotas.GeracaoFrotaAutomatizada geracaoFrotaAutomatizada, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
            dynamic modelosVeicularesCarga = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ModelosVeicular"));

            if (geracaoFrotaAutomatizada.ModelosVeicularesCarga == null)
                geracaoFrotaAutomatizada.ModelosVeicularesCarga = new List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>();
            else
                geracaoFrotaAutomatizada.ModelosVeicularesCarga.Clear();

            foreach (var modeloVeicular in modelosVeicularesCarga)
            {
                Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga tipoOperacaoAdicionar = repModeloVeicularCarga.BuscarPorCodigo((int)modeloVeicular.Codigo) ?? throw new ControllerException("Modelo Veicular Carga não encontrado");
                geracaoFrotaAutomatizada.ModelosVeicularesCarga.Add(tipoOperacaoAdicionar);
            }
        }

        #endregion Métodos Privados
    }
}

