using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace SGT.WebAdmin.Controllers.Pedidos
{
    [CustomAuthorize("Pedidos/RegrasPedido")]
    public class RegrasPedidoController : BaseController
    {
		#region Construtores

		public RegrasPedidoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region ObjetosJson
        private class ObjetoEntidade
        {
            public dynamic Codigo { get; set; } // dynamic pois o codigo pode ser também um cpf/cnpj
            public string Descricao { get; set; }
        }
        private class ObjetoAprovadores
        {
            public int Codigo { get; set; }
            public string Nome { get; set; }
        }
        private class RegrasPorTipo
        {
            public dynamic Codigo { get; set; }
            public int Ordem { get; set; }
            public Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia Condicao { get; set; }
            public Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia Juncao { get; set; }
            public ObjetoEntidade Entidade { get; set; }
            public dynamic Valor { get; set; }
        }
        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 35, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Vigência", "Vigencia", 15, Models.Grid.Align.center, true);

                // Converte parametros
                int codigoAprovador = 0;
                int.TryParse(Request.Params("Aprovador"), out codigoAprovador);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoOcorrencia etapaAutorizacaoOcorrencia;
                Enum.TryParse(Request.Params("EtapaAutorizacao"), out etapaAutorizacaoOcorrencia);

                DateTime dataInicioAux, dataFimAux;
                DateTime? dataInicio = null, dataFim = null;

                if (DateTime.TryParseExact(Request.Params("DataInicio"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicioAux))
                    dataInicio = dataInicioAux;

                if (DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFimAux))
                    dataFim = dataFimAux;

                string descricao = !string.IsNullOrWhiteSpace(Request.Params("Descricao")) ? Request.Params("Descricao") : "";

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                // Instancia repositorios                
                Repositorio.Embarcador.Pedidos.RegrasPedido repRegrasPedido = new Repositorio.Embarcador.Pedidos.RegrasPedido(unitOfWork);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

                Dominio.Entidades.Usuario aprovador = repUsuario.BuscarPorCodigo(codigoAprovador);

                List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedido> listaRegras = repRegrasPedido.ConsultarRegras(dataInicio, dataFim, aprovador, descricao, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repRegrasPedido.ContarConsultaRegras(dataInicio, dataFim, aprovador, descricao));

                var lista = (from obj in listaRegras
                             select new
                             {
                                 obj.Codigo,
                                 Descricao = !string.IsNullOrWhiteSpace(obj.Descricao) ? obj.Descricao : string.Empty,
                                 Vigencia = obj.Vigencia.HasValue ? obj.Vigencia.Value.ToString("dd/MM/yyyy") : string.Empty,
                             }).ToList();

                grid.AdicionaRows(lista);
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

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                // Instancia Repositorios/Entidade
                Repositorio.Embarcador.Pedidos.RegrasPedido repRegrasPedido = new Repositorio.Embarcador.Pedidos.RegrasPedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.RegrasPedidoGrupoPessoa repRegrasPedidoGrupoPessoa = new Repositorio.Embarcador.Pedidos.RegrasPedidoGrupoPessoa(unitOfWork);
                Repositorio.Embarcador.Pedidos.RegrasPedidoModeloVeicular repRegrasPedidoModeloVeicular = new Repositorio.Embarcador.Pedidos.RegrasPedidoModeloVeicular(unitOfWork);
                Repositorio.Embarcador.Pedidos.RegrasPedidoSituacaoColaborador repRegrasPedidoSituacaoColaborador = new Repositorio.Embarcador.Pedidos.RegrasPedidoSituacaoColaborador(unitOfWork);
                Repositorio.Embarcador.Pedidos.RegrasPedidoTipoCarga repRegrasPedidoTipoCarga = new Repositorio.Embarcador.Pedidos.RegrasPedidoTipoCarga(unitOfWork);
                Repositorio.Embarcador.Pedidos.RegrasPedidoTipoOperacao repRegrasPedidoTipoOperacao = new Repositorio.Embarcador.Pedidos.RegrasPedidoTipoOperacao(unitOfWork);
                Repositorio.Embarcador.Pedidos.RegrasPedidoValorFrete repRegrasPedidoValorFrete = new Repositorio.Embarcador.Pedidos.RegrasPedidoValorFrete(unitOfWork);
                Repositorio.Embarcador.Pedidos.RegrasPedidoDistancia repRegrasPedidoDistancia = new Repositorio.Embarcador.Pedidos.RegrasPedidoDistancia(unitOfWork);
                Repositorio.Embarcador.Pedidos.RegrasPedidoPercentualDiferencaFreteLiquidoParaFreteTerceiro repositorioRegrasPedidoPercentualDiferencaFreteLiquidoParaFreteTerceiro = new Repositorio.Embarcador.Pedidos.RegrasPedidoPercentualDiferencaFreteLiquidoParaFreteTerceiro(unitOfWork);

                // Nova entidade
                Dominio.Entidades.Embarcador.Pedidos.RegrasPedido regrasPedidos = new Dominio.Entidades.Embarcador.Pedidos.RegrasPedido();
                List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoGrupoPessoa> regrasPedidoGrupoPessoa = new List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoGrupoPessoa>();
                List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoModeloVeicular> regrasPedidoModeloVeicular = new List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoModeloVeicular>();
                List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoSituacaoColaborador> regrasPedidoSituacaoColaborador = new List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoSituacaoColaborador>();
                List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoTipoCarga> regrasPedidoTipoCarga = new List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoTipoCarga>();
                List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoTipoOperacao> regrasPedidoTipoOperacao = new List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoTipoOperacao>();
                List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoValorFrete> regrasPedidoValorFrete = new List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoValorFrete>();
                List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoDistancia> regrasPedidoDistancia = new List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoDistancia>();
                List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoPercentualDiferencaFreteLiquidoParaFreteTerceiro> regrasPedidoPercentualDiferencaFreteLiquidoParaFreteTerceiro = new List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoPercentualDiferencaFreteLiquidoParaFreteTerceiro>();

                // Preenche a entidade
                PreencherEntidade(ref regrasPedidos, unitOfWork);

                List<string> erros = new List<string>();
                // Validar entidade
                if (!ValidarEntidade(regrasPedidos, out erros))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, String.Join("<br>", erros));
                }

                try
                {
                    PreencherTodasRegras(ref regrasPedidos, ref regrasPedidoGrupoPessoa, ref regrasPedidoModeloVeicular, ref regrasPedidoSituacaoColaborador, ref regrasPedidoTipoCarga, ref regrasPedidoTipoOperacao, ref regrasPedidoValorFrete, ref regrasPedidoDistancia, ref regrasPedidoPercentualDiferencaFreteLiquidoParaFreteTerceiro, ref erros, unitOfWork);
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, ex.Message);
                }

                // Insere Entidade
                repRegrasPedido.Inserir(regrasPedidos, Auditado);

                // Insere regras
                for (var i = 0; i < regrasPedidoGrupoPessoa.Count(); i++) repRegrasPedidoGrupoPessoa.Inserir(regrasPedidoGrupoPessoa[i]);
                for (var i = 0; i < regrasPedidoTipoCarga.Count(); i++) repRegrasPedidoTipoCarga.Inserir(regrasPedidoTipoCarga[i]);
                for (var i = 0; i < regrasPedidoTipoOperacao.Count(); i++) repRegrasPedidoTipoOperacao.Inserir(regrasPedidoTipoOperacao[i]);
                for (var i = 0; i < regrasPedidoModeloVeicular.Count(); i++) repRegrasPedidoModeloVeicular.Inserir(regrasPedidoModeloVeicular[i]);
                for (var i = 0; i < regrasPedidoSituacaoColaborador.Count(); i++) repRegrasPedidoSituacaoColaborador.Inserir(regrasPedidoSituacaoColaborador[i]);
                for (var i = 0; i < regrasPedidoValorFrete.Count(); i++) repRegrasPedidoValorFrete.Inserir(regrasPedidoValorFrete[i]);
                for (var i = 0; i < regrasPedidoDistancia.Count(); i++) repRegrasPedidoDistancia.Inserir(regrasPedidoDistancia[i]);
                for (var i = 0; i < regrasPedidoPercentualDiferencaFreteLiquidoParaFreteTerceiro.Count(); i++) repositorioRegrasPedidoPercentualDiferencaFreteLiquidoParaFreteTerceiro.Inserir(regrasPedidoPercentualDiferencaFreteLiquidoParaFreteTerceiro[i]);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
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

                // Instancia Repositorios/Entidade
                Repositorio.Embarcador.Pedidos.RegrasPedido repRegrasPedido = new Repositorio.Embarcador.Pedidos.RegrasPedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.RegrasPedidoGrupoPessoa repRegrasPedidoGrupoPessoa = new Repositorio.Embarcador.Pedidos.RegrasPedidoGrupoPessoa(unitOfWork);
                Repositorio.Embarcador.Pedidos.RegrasPedidoModeloVeicular repRegrasPedidoModeloVeicular = new Repositorio.Embarcador.Pedidos.RegrasPedidoModeloVeicular(unitOfWork);
                Repositorio.Embarcador.Pedidos.RegrasPedidoSituacaoColaborador repRegrasPedidoSituacaoColaborador = new Repositorio.Embarcador.Pedidos.RegrasPedidoSituacaoColaborador(unitOfWork);
                Repositorio.Embarcador.Pedidos.RegrasPedidoTipoCarga repRegrasPedidoTipoCarga = new Repositorio.Embarcador.Pedidos.RegrasPedidoTipoCarga(unitOfWork);
                Repositorio.Embarcador.Pedidos.RegrasPedidoTipoOperacao repRegrasPedidoTipoOperacao = new Repositorio.Embarcador.Pedidos.RegrasPedidoTipoOperacao(unitOfWork);
                Repositorio.Embarcador.Pedidos.RegrasPedidoValorFrete repRegrasPedidoValorFrete = new Repositorio.Embarcador.Pedidos.RegrasPedidoValorFrete(unitOfWork);
                Repositorio.Embarcador.Pedidos.RegrasPedidoDistancia repRegrasPedidoDistancia = new Repositorio.Embarcador.Pedidos.RegrasPedidoDistancia(unitOfWork);
                Repositorio.Embarcador.Pedidos.RegrasPedidoPercentualDiferencaFreteLiquidoParaFreteTerceiro repositorioRegrasPedidoPercentualDiferencaFreteLiquidoParaFreteTerceiro = new Repositorio.Embarcador.Pedidos.RegrasPedidoPercentualDiferencaFreteLiquidoParaFreteTerceiro(unitOfWork);

                // Nova entidade
                // Codigo da busca 
                int codigoRegra = 0;
                int.TryParse(Request.Params("Codigo"), out codigoRegra);

                // Busca entidade
                Dominio.Entidades.Embarcador.Pedidos.RegrasPedido regrasPedidos = repRegrasPedido.BuscarPorCodigo(codigoRegra, true);

                if (regrasPedidos == null)
                    return new JsonpResult(false, "Não foi possível buscar a regra.");


                #region BuscaRegras
                List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoGrupoPessoa> regrasPedidoGrupoPessoa = repRegrasPedidoGrupoPessoa.BuscarPorRegras(codigoRegra);
                List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoModeloVeicular> regrasPedidoModeloVeicular = repRegrasPedidoModeloVeicular.BuscarPorRegras(codigoRegra);
                List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoSituacaoColaborador> regrasPedidoSituacaoColaborador = repRegrasPedidoSituacaoColaborador.BuscarPorRegras(codigoRegra);
                List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoTipoCarga> regrasPedidoTipoCarga = repRegrasPedidoTipoCarga.BuscarPorRegras(codigoRegra);
                List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoTipoOperacao> regrasPedidoTipoOperacao = repRegrasPedidoTipoOperacao.BuscarPorRegras(codigoRegra);
                List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoValorFrete> regrasPedidoValorFrete = repRegrasPedidoValorFrete.BuscarPorRegras(codigoRegra);
                List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoDistancia> regrasPedidoDistancia = repRegrasPedidoDistancia.BuscarPorRegras(codigoRegra);
                List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoPercentualDiferencaFreteLiquidoParaFreteTerceiro> regrasPedidoPercentualDiferencaFreteLiquidoParaFreteTerceiro = repositorioRegrasPedidoPercentualDiferencaFreteLiquidoParaFreteTerceiro.BuscarPorRegras(codigoRegra);
                #endregion


                #region DeletaRegras
                //for (var i = 0; i < regrasPedidoGrupoPessoa.Count(); i++) repRegrasPedidoGrupoPessoa.Deletar(regrasPedidoGrupoPessoa[i]);
                //for (var i = 0; i < regrasPedidoTipoCarga.Count(); i++) repRegrasPedidoTipoCarga.Deletar(regrasPedidoTipoCarga[i]);
                //for (var i = 0; i < regrasPedidoTipoOperacao.Count(); i++) repRegrasPedidoTipoOperacao.Deletar(regrasPedidoTipoOperacao[i]);
                //for (var i = 0; i < regrasPedidoModeloVeicular.Count(); i++) repRegrasPedidoModeloVeicular.Deletar(regrasPedidoModeloVeicular[i]);
                //for (var i = 0; i < regrasPedidoValorFrete.Count(); i++) repRegrasPedidoValorFrete.Deletar(regrasPedidoValorFrete[i]);
                #endregion

                #region NovasRegras
                //regrasPedidoGrupoPessoa = new List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoGrupoPessoa>();
                //regrasPedidoModeloVeicular = new List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoModeloVeicular>();
                //regrasPedidoTipoCarga = new List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoTipoCarga>();
                //regrasPedidoTipoOperacao = new List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoTipoOperacao>();
                //regrasPedidoValorFrete = new List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoValorFrete>();
                #endregion


                // Preenche a entidade
                PreencherEntidade(ref regrasPedidos, unitOfWork);

                List<string> erros = new List<string>();
                // Validar entidade
                if (!ValidarEntidade(regrasPedidos, out erros))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, String.Join("<br>", erros));
                }


                try
                {
                    PreencherTodasRegras(ref regrasPedidos, ref regrasPedidoGrupoPessoa, ref regrasPedidoModeloVeicular, ref regrasPedidoSituacaoColaborador, ref regrasPedidoTipoCarga, ref regrasPedidoTipoOperacao, ref regrasPedidoValorFrete, ref regrasPedidoDistancia, ref regrasPedidoPercentualDiferencaFreteLiquidoParaFreteTerceiro, ref erros, unitOfWork);
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, ex.Message);
                }

                // Insere regras
                //for (var i = 0; i < regrasPedidoGrupoPessoa.Count(); i++) repRegrasPedidoGrupoPessoa.Inserir(regrasPedidoGrupoPessoa[i]);
                //for (var i = 0; i < regrasPedidoTipoCarga.Count(); i++) repRegrasPedidoTipoCarga.Inserir(regrasPedidoTipoCarga[i]);
                //for (var i = 0; i < regrasPedidoTipoOperacao.Count(); i++) repRegrasPedidoTipoOperacao.Inserir(regrasPedidoTipoOperacao[i]);
                //for (var i = 0; i < regrasPedidoModeloVeicular.Count(); i++) repRegrasPedidoModeloVeicular.Inserir(regrasPedidoModeloVeicular[i]);
                //for (var i = 0; i < regrasPedidoValorFrete.Count(); i++) repRegrasPedidoValorFrete.Inserir(regrasPedidoValorFrete[i]);

                #region Insere Regras
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = new List<Dominio.Entidades.Auditoria.HistoricoPropriedade>();

                bool inseriuCriterio = false;
                for (var i = 0; i < regrasPedidoGrupoPessoa.Count(); i++)
                {
                    if (regrasPedidoGrupoPessoa[i].Codigo == 0)
                    {
                        repRegrasPedidoGrupoPessoa.Inserir(regrasPedidoGrupoPessoa[i]);
                        inseriuCriterio = true;
                    }
                    else if (regrasPedidoGrupoPessoa[i].GetChanges() != null)
                    {
                        alteracoes.AddRange(regrasPedidoGrupoPessoa[i].GetChanges());
                        repRegrasPedidoGrupoPessoa.Atualizar(regrasPedidoGrupoPessoa[i]);
                    }
                    else
                        repRegrasPedidoGrupoPessoa.Deletar(regrasPedidoGrupoPessoa[i]);
                }
                if (inseriuCriterio)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, regrasPedidos, null, "Adicionou um critério de Grupo Pessoa na regra.", unitOfWork);

                inseriuCriterio = false;
                for (var i = 0; i < regrasPedidoTipoCarga.Count(); i++)
                {
                    if (regrasPedidoTipoCarga[i].Codigo == 0)
                    {
                        repRegrasPedidoTipoCarga.Inserir(regrasPedidoTipoCarga[i]);
                        inseriuCriterio = true;
                    }
                    else if (regrasPedidoTipoCarga[i].GetChanges() != null)
                    {
                        alteracoes.AddRange(regrasPedidoTipoCarga[i].GetChanges());
                        repRegrasPedidoTipoCarga.Atualizar(regrasPedidoTipoCarga[i]);
                    }
                    else
                        repRegrasPedidoTipoCarga.Deletar(regrasPedidoTipoCarga[i]);
                }
                if (inseriuCriterio)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, regrasPedidos, null, "Adicionou um critério de Tipo de Carga na regra.", unitOfWork);

                inseriuCriterio = false;
                for (var i = 0; i < regrasPedidoTipoOperacao.Count(); i++)
                {
                    if (regrasPedidoTipoOperacao[i].Codigo == 0)
                    {
                        repRegrasPedidoTipoOperacao.Inserir(regrasPedidoTipoOperacao[i]);
                        inseriuCriterio = true;
                    }
                    else if (regrasPedidoTipoOperacao[i].GetChanges() != null)
                    {
                        alteracoes.AddRange(regrasPedidoTipoOperacao[i].GetChanges());
                        repRegrasPedidoTipoOperacao.Atualizar(regrasPedidoTipoOperacao[i]);
                    }
                    else
                        repRegrasPedidoTipoOperacao.Deletar(regrasPedidoTipoOperacao[i]);
                }
                if (inseriuCriterio)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, regrasPedidos, null, "Adicionou um critério de Tipo de Operação na regra.", unitOfWork);

                inseriuCriterio = false;
                for (var i = 0; i < regrasPedidoModeloVeicular.Count(); i++)
                {
                    if (regrasPedidoModeloVeicular[i].Codigo == 0)
                    {
                        repRegrasPedidoModeloVeicular.Inserir(regrasPedidoModeloVeicular[i]);
                        inseriuCriterio = true;
                    }
                    else if (regrasPedidoModeloVeicular[i].GetChanges() != null)
                    {
                        alteracoes.AddRange(regrasPedidoModeloVeicular[i].GetChanges());
                        repRegrasPedidoModeloVeicular.Atualizar(regrasPedidoModeloVeicular[i]);
                    }
                    else
                        repRegrasPedidoModeloVeicular.Deletar(regrasPedidoModeloVeicular[i]);
                }
                if (inseriuCriterio)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, regrasPedidos, null, "Adicionou um critério de Modelo Veicular na regra.", unitOfWork);

                inseriuCriterio = false;
                for (var i = 0; i < regrasPedidoSituacaoColaborador.Count(); i++)
                {
                    if (regrasPedidoSituacaoColaborador[i].Codigo == 0)
                    {
                        repRegrasPedidoSituacaoColaborador.Inserir(regrasPedidoSituacaoColaborador[i]);
                        inseriuCriterio = true;
                    }
                    else if (regrasPedidoSituacaoColaborador[i].GetChanges() != null)
                    {
                        alteracoes.AddRange(regrasPedidoSituacaoColaborador[i].GetChanges());
                        repRegrasPedidoSituacaoColaborador.Atualizar(regrasPedidoSituacaoColaborador[i]);
                    }
                    else
                        repRegrasPedidoSituacaoColaborador.Deletar(regrasPedidoSituacaoColaborador[i]);
                }
                if (inseriuCriterio)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, regrasPedidos, null, "Adicionou um critério de Situação do Colaborador na regra.", unitOfWork);

                inseriuCriterio = false;
                for (var i = 0; i < regrasPedidoValorFrete.Count(); i++)
                {
                    if (regrasPedidoValorFrete[i].Codigo == 0)
                    {
                        repRegrasPedidoValorFrete.Inserir(regrasPedidoValorFrete[i]);
                        inseriuCriterio = true;
                    }
                    else if (regrasPedidoValorFrete[i].GetChanges() != null)
                    {
                        alteracoes.AddRange(regrasPedidoValorFrete[i].GetChanges());
                        repRegrasPedidoValorFrete.Atualizar(regrasPedidoValorFrete[i]);
                    }
                    else
                        repRegrasPedidoValorFrete.Deletar(regrasPedidoValorFrete[i]);
                }
                if (inseriuCriterio)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, regrasPedidos, null, "Adicionou um critério de Valor do Frete na regra.", unitOfWork);

                inseriuCriterio = false;
                for (var i = 0; i < regrasPedidoDistancia.Count(); i++)
                {
                    if (regrasPedidoDistancia[i].Codigo == 0)
                    {
                        repRegrasPedidoDistancia.Inserir(regrasPedidoDistancia[i]);
                        inseriuCriterio = true;
                    }
                    else if (regrasPedidoDistancia[i].GetChanges() != null)
                    {
                        alteracoes.AddRange(regrasPedidoDistancia[i].GetChanges());
                        repRegrasPedidoDistancia.Atualizar(regrasPedidoDistancia[i]);
                    }
                    else
                        repRegrasPedidoDistancia.Deletar(regrasPedidoDistancia[i]);
                }
                if (inseriuCriterio)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, regrasPedidos, null, "Adicionou um critério de Distância na regra.", unitOfWork);

                inseriuCriterio = false;
                for (var i = 0; i < regrasPedidoPercentualDiferencaFreteLiquidoParaFreteTerceiro.Count(); i++)
                {
                    if (regrasPedidoPercentualDiferencaFreteLiquidoParaFreteTerceiro[i].Codigo == 0)
                    {
                        repositorioRegrasPedidoPercentualDiferencaFreteLiquidoParaFreteTerceiro.Inserir(regrasPedidoPercentualDiferencaFreteLiquidoParaFreteTerceiro[i]);
                        inseriuCriterio = true;
                    }
                    else if (regrasPedidoPercentualDiferencaFreteLiquidoParaFreteTerceiro[i].GetChanges() != null)
                    {
                        alteracoes.AddRange(regrasPedidoPercentualDiferencaFreteLiquidoParaFreteTerceiro[i].GetChanges());
                        repositorioRegrasPedidoPercentualDiferencaFreteLiquidoParaFreteTerceiro.Atualizar(regrasPedidoPercentualDiferencaFreteLiquidoParaFreteTerceiro[i]);
                    }
                    else
                        repositorioRegrasPedidoPercentualDiferencaFreteLiquidoParaFreteTerceiro.Deletar(regrasPedidoPercentualDiferencaFreteLiquidoParaFreteTerceiro[i]);
                }
                if (inseriuCriterio)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, regrasPedidos, null, "Adicionou um critério de Percentual de Diferença do Frete Líquido para o Frete com Terceiro na regra.", unitOfWork);

                // Atualiza Entidade
                repRegrasPedido.Atualizar(regrasPedidos, Auditado);
                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, regrasPedidos, alteracoes, "Alterou os critérios da regra.", unitOfWork);
                #endregion

                // Atualiza Entidade

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
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
                // Instancia Repositorios/Entidade

                Repositorio.Embarcador.Pedidos.RegrasPedido repRegrasPedido = new Repositorio.Embarcador.Pedidos.RegrasPedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.RegrasPedidoGrupoPessoa repRegrasPedidoGrupoPessoa = new Repositorio.Embarcador.Pedidos.RegrasPedidoGrupoPessoa(unitOfWork);
                Repositorio.Embarcador.Pedidos.RegrasPedidoModeloVeicular repRegrasPedidoModeloVeicular = new Repositorio.Embarcador.Pedidos.RegrasPedidoModeloVeicular(unitOfWork);
                Repositorio.Embarcador.Pedidos.RegrasPedidoSituacaoColaborador repRegrasPedidoSituacaoColaborador = new Repositorio.Embarcador.Pedidos.RegrasPedidoSituacaoColaborador(unitOfWork);
                Repositorio.Embarcador.Pedidos.RegrasPedidoTipoCarga repRegrasPedidoTipoCarga = new Repositorio.Embarcador.Pedidos.RegrasPedidoTipoCarga(unitOfWork);
                Repositorio.Embarcador.Pedidos.RegrasPedidoTipoOperacao repRegrasPedidoTipoOperacao = new Repositorio.Embarcador.Pedidos.RegrasPedidoTipoOperacao(unitOfWork);
                Repositorio.Embarcador.Pedidos.RegrasPedidoValorFrete repRegrasPedidoValorFrete = new Repositorio.Embarcador.Pedidos.RegrasPedidoValorFrete(unitOfWork);
                Repositorio.Embarcador.Pedidos.RegrasPedidoDistancia repRegrasPedidoDistancia = new Repositorio.Embarcador.Pedidos.RegrasPedidoDistancia(unitOfWork);
                Repositorio.Embarcador.Pedidos.RegrasPedidoPercentualDiferencaFreteLiquidoParaFreteTerceiro repositorioRegrasPedidoPercentualDiferencaFreteLiquidoParaFreteTerceiro = new Repositorio.Embarcador.Pedidos.RegrasPedidoPercentualDiferencaFreteLiquidoParaFreteTerceiro(unitOfWork);

                // Codigo da busca 
                int codigoRegra = 0;
                int.TryParse(Request.Params("Codigo"), out codigoRegra);

                // Busca entidade
                Dominio.Entidades.Embarcador.Pedidos.RegrasPedido regrasPedidos = repRegrasPedido.BuscarPorCodigo(codigoRegra);

                if (regrasPedidos == null)
                    return new JsonpResult(false, "Não foi possível buscar a regra.");

                #region BuscaRegras
                List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoGrupoPessoa> regrasPedidoGrupoPessoa = repRegrasPedidoGrupoPessoa.BuscarPorRegras(codigoRegra);
                List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoModeloVeicular> regrasPedidoModeloVeicular = repRegrasPedidoModeloVeicular.BuscarPorRegras(codigoRegra);
                List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoSituacaoColaborador> regrasPedidoSituacaoColaborador = repRegrasPedidoSituacaoColaborador.BuscarPorRegras(codigoRegra);
                List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoTipoCarga> regrasPedidoTipoCarga = repRegrasPedidoTipoCarga.BuscarPorRegras(codigoRegra);
                List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoTipoOperacao> regrasPedidoTipoOperacao = repRegrasPedidoTipoOperacao.BuscarPorRegras(codigoRegra);
                List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoValorFrete> regrasPedidoValorFrete = repRegrasPedidoValorFrete.BuscarPorRegras(codigoRegra);
                List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoDistancia> regrasPedidoDistancia = repRegrasPedidoDistancia.BuscarPorRegras(codigoRegra);
                List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoPercentualDiferencaFreteLiquidoParaFreteTerceiro> regrasPedidoPercentualDiferencaFreteLiquidoParaFreteTerceiro = repositorioRegrasPedidoPercentualDiferencaFreteLiquidoParaFreteTerceiro.BuscarPorRegras(codigoRegra);
                #endregion


                var dynRegra = new
                {
                    regrasPedidos.Codigo,
                    regrasPedidos.NumeroAprovadores,
                    Vigencia = regrasPedidos.Vigencia.HasValue ? regrasPedidos.Vigencia.Value.ToString("dd/MM/yyyy") : string.Empty,
                    Descricao = !string.IsNullOrWhiteSpace(regrasPedidos.Descricao) ? regrasPedidos.Descricao : string.Empty,
                    Observacao = !string.IsNullOrWhiteSpace(regrasPedidos.Observacoes) ? regrasPedidos.Observacoes : string.Empty,

                    Aprovadores = (from o in regrasPedidos.Aprovadores select new { o.Codigo, o.Nome }).ToList(),

                    RegraPorGrupoPessoa = regrasPedidos.RegraPorGrupoPessoa,
                    GrupoPessoa = (from obj in regrasPedidoGrupoPessoa select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoGrupoPessoa>(obj, "GrupoPessoas", "Descricao")).ToList(),

                    RegraPorTipoCarga = regrasPedidos.RegraPorTipoCarga,
                    TipoCarga = (from obj in regrasPedidoTipoCarga select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoTipoCarga>(obj, "TipoDeCarga", "Descricao")).ToList(),

                    RegraPorTipoOperacao = regrasPedidos.RegraPorTipoOperacao,
                    TipoOperacao = (from obj in regrasPedidoTipoOperacao select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoTipoOperacao>(obj, "TipoOperacao", "Descricao")).ToList(),

                    RegraPorModeloVeicular = regrasPedidos.RegraPorModeloVeicular,
                    ModeloVeicular = (from obj in regrasPedidoModeloVeicular select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoModeloVeicular>(obj, "ModeloVeicularCarga", "Descricao")).ToList(),

                    RegraPorSituacaoColaborador = regrasPedidos.RegraPorSituacaoColaborador,
                    SituacaoColaborador = (from obj in regrasPedidoSituacaoColaborador select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoSituacaoColaborador>(obj, "ColaboradorSituacao", "Descricao")).ToList(),

                    RegraPorValorFrete = regrasPedidos.RegraPorValorFrete,
                    ValorFrete = (from obj in regrasPedidoValorFrete select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoValorFrete>(obj, "Valor", "Valor", true)).ToList(),

                    RegraPorDistancia = regrasPedidos.RegraPorDistancia,
                    Distancia = (from obj in regrasPedidoDistancia select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoDistancia>(obj, "Distancia", "Distancia", true)).ToList(),

                    RegraPorDiferencaFreteLiquidoParaFreteTerceiro = regrasPedidos.RegraPorDiferencaFreteLiquidoParaFreteTerceiro,
                    DiferencaFreteLiquidoParaFreteTerceiro = (from obj in regrasPedidoPercentualDiferencaFreteLiquidoParaFreteTerceiro select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoPercentualDiferencaFreteLiquidoParaFreteTerceiro>(obj, "DiferencaFreteLiquidoParaFreteTerceiro", "DiferencaFreteLiquidoParaFreteTerceiro", true)).ToList()
                };

                return new JsonpResult(dynRegra);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar.");
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
                // Instancia Repositorios/Entidade
                Repositorio.Embarcador.Pedidos.RegrasPedido repRegrasPedido = new Repositorio.Embarcador.Pedidos.RegrasPedido(unitOfWork);

                // Codigo da busca 
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca entidade
                Dominio.Entidades.Embarcador.Pedidos.RegrasPedido regrasPedidos = repRegrasPedido.BuscarPorCodigo(codigo);

                if (regrasPedidos == null)
                    return new JsonpResult(false, "Não foi possível buscar a regra.");

                // Inicia transicao
                unitOfWork.Start();

                regrasPedidos.Aprovadores.Clear();
                regrasPedidos.RegrasPedidoGrupoPessoa.Clear();
                regrasPedidos.RegrasPedidoModeloVeicular.Clear();
                regrasPedidos.RegrasPedidoSituacaoColaborador.Clear();
                regrasPedidos.RegrasPedidoTipoCarga.Clear();
                regrasPedidos.RegrasPedidoTipoOperacao.Clear();
                regrasPedidos.RegrasPedidoValorFrete.Clear();

                repRegrasPedido.Deletar(regrasPedidos);

                // Comita alteracoes
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, "Já existem solicitações vinculadas à regra.");
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion


        #region Métodos Privados
        private void PreencherEntidade(ref Dominio.Entidades.Embarcador.Pedidos.RegrasPedido regrasPedidos, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia Repositorios/Entidade
            Repositorio.Embarcador.Pedidos.RegrasPedido repPedidos = new Repositorio.Embarcador.Pedidos.RegrasPedido(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);


            // Converte parametros
            string descricao = !string.IsNullOrWhiteSpace(Request.Params("Descricao")) ? Request.Params("Descricao") : string.Empty;
            string observacao = !string.IsNullOrWhiteSpace(Request.Params("Observacao")) ? Request.Params("Observacao") : string.Empty;

            DateTime dataVigenciaAux;
            DateTime? dataVigencia = null;

            if (DateTime.TryParseExact(Request.Params("Vigencia"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataVigenciaAux))
                dataVigencia = dataVigenciaAux;

            int numeroAprovadores = 0;
            int.TryParse(Request.Params("NumeroAprovadores"), out numeroAprovadores);

            bool usarRegraPorGrupoPessoa;
            bool.TryParse(Request.Params("RegraPorGrupoPessoa"), out usarRegraPorGrupoPessoa);
            bool usarRegraPorTipoCarga;
            bool.TryParse(Request.Params("RegraPorTipoCarga"), out usarRegraPorTipoCarga);
            bool usarRegraPorTipoOperacao;
            bool.TryParse(Request.Params("RegraPorTipoOperacao"), out usarRegraPorTipoOperacao);
            bool usarRegraPorModeloVeicular;
            bool.TryParse(Request.Params("RegraPorModeloVeicular"), out usarRegraPorModeloVeicular);
            bool usarRegraPorSituacaoColaborador;
            bool.TryParse(Request.Params("RegraPorSituacaoColaborador"), out usarRegraPorSituacaoColaborador);
            bool usarRegraPorValorFrete;
            bool.TryParse(Request.Params("RegraPorValorFrete"), out usarRegraPorValorFrete);
            bool usarRegraPorDistancia;
            bool.TryParse(Request.Params("RegraPorDistancia"), out usarRegraPorDistancia);
            bool usarRegraPorDiferencaFreteLiquidoParaFreteTerceiro = Request.GetBoolParam("RegraPorDiferencaFreteLiquidoParaFreteTerceiro");

            List<int> codigosUsuarios = new List<int>();
            if (!string.IsNullOrWhiteSpace(Request.Params("Aprovadores")))
            {
                List<ObjetoAprovadores> dynAprovadores = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ObjetoAprovadores>>(Request.Params("Aprovadores"));

                for (var i = 0; i < dynAprovadores.Count(); i++)
                    codigosUsuarios.Add(dynAprovadores[i].Codigo);
            }
            List<Dominio.Entidades.Usuario> listaAprovadores = repUsuario.BuscarUsuariosPorCodigos(codigosUsuarios.ToArray(), null);

            // Seta na entidade
            regrasPedidos.Descricao = descricao;
            regrasPedidos.Observacoes = observacao;
            regrasPedidos.Vigencia = dataVigencia;
            regrasPedidos.NumeroAprovadores = numeroAprovadores;
            regrasPedidos.Aprovadores = listaAprovadores;

            regrasPedidos.RegraPorGrupoPessoa = usarRegraPorGrupoPessoa;
            regrasPedidos.RegraPorTipoCarga = usarRegraPorTipoCarga;
            regrasPedidos.RegraPorTipoOperacao = usarRegraPorTipoOperacao;
            regrasPedidos.RegraPorModeloVeicular = usarRegraPorModeloVeicular;
            regrasPedidos.RegraPorSituacaoColaborador = usarRegraPorSituacaoColaborador;
            regrasPedidos.RegraPorValorFrete = usarRegraPorValorFrete;
            regrasPedidos.RegraPorDistancia = usarRegraPorDistancia;
            regrasPedidos.RegraPorDiferencaFreteLiquidoParaFreteTerceiro = usarRegraPorDiferencaFreteLiquidoParaFreteTerceiro;
        }



        private void PreencherTodasRegras(ref Dominio.Entidades.Embarcador.Pedidos.RegrasPedido regrasPedidos, ref List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoGrupoPessoa> regrasPedidoGrupoPessoa, ref List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoModeloVeicular> regrasPedidoModeloVeicular, ref List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoSituacaoColaborador> regrasPedidoSituacaoColaborador, ref List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoTipoCarga> regrasPedidoTipoCarga, ref List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoTipoOperacao> regrasPedidoTipoOperacao, ref List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoValorFrete> regrasPedidoValorFrete, ref List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoDistancia> regrasPedidoDistancia, ref List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoPercentualDiferencaFreteLiquidoParaFreteTerceiro> regrasPedidoPercentualDiferencaFreteLiquidoParaFreteTerceiro, ref List<string> errosRegras, Repositorio.UnitOfWork unitOfWork)
        {
            // Erros de validacao
            List<string> erros = new List<string>();

            #region Grupo Pessoa
            // Preenche as regras apenas se a flag de uso for verdadeira
            if (regrasPedidos.RegraPorGrupoPessoa)
            {
                // Preenche regra
                try
                {
                    PreencherEntidadeRegra("GrupoPessoas", "RegrasGrupoPessoa", false, ref regrasPedidoGrupoPessoa, ref regrasPedidos, ((codigo) =>
                    {
                        Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);

                        int codigoInt = 0;
                        int.TryParse(codigo.ToString(), out codigoInt);

                        return repGrupoPessoas.BuscarPorCodigo(codigoInt);
                    }));
                }
                catch (Exception e)
                {
                    Servicos.Log.TratarErro(e);
                    errosRegras.Add("GrupoPessoas");
                }

                erros = new List<string>();
                // Valida regra
                if (!ValidarEntidadeRegra("Grupo de Pessoas", "GrupoPessoas", regrasPedidoGrupoPessoa, out erros))
                    throw new Exception(String.Join("<br>", erros));
            }
            else
            {
                regrasPedidoGrupoPessoa = new List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoGrupoPessoa>();
            }
            #endregion

            #region Tipo Carga
            // Preenche as regras apenas se a flag de uso for verdadeira
            if (regrasPedidos.RegraPorTipoCarga)
            {
                // Preenche regra
                try
                {
                    PreencherEntidadeRegra("TipoDeCarga", "RegrasTipoCarga", false, ref regrasPedidoTipoCarga, ref regrasPedidos, ((codigo) =>
                    {
                        Repositorio.Embarcador.Cargas.TipoDeCarga repTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);

                        int codigoInt = 0;
                        int.TryParse(codigo.ToString(), out codigoInt);

                        return repTipoDeCarga.BuscarPorCodigo(codigoInt);
                    }));
                }
                catch (Exception e)
                {
                    Servicos.Log.TratarErro(e);
                    errosRegras.Add("Tipo Carga");
                }

                erros = new List<string>();
                // Valida regra
                if (!ValidarEntidadeRegra("Tipo Carga", "TipoDeCarga", regrasPedidoTipoCarga, out erros))
                    throw new Exception(String.Join("<br>", erros));
            }
            else
            {
                regrasPedidoTipoCarga = new List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoTipoCarga>();
            }
            #endregion

            #region Tipo Operacao
            // Preenche as regras apenas se a flag de uso for verdadeira
            if (regrasPedidos.RegraPorTipoOperacao)
            {
                // Preenche regra
                try
                {
                    PreencherEntidadeRegra("TipoOperacao", "RegrasTipoOperacao", false, ref regrasPedidoTipoOperacao, ref regrasPedidos, ((codigo) =>
                    {
                        Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);

                        int codigoInt = 0;
                        int.TryParse(codigo.ToString(), out codigoInt);

                        return repTipoOperacao.BuscarPorCodigo(codigoInt);
                    }));
                }
                catch (Exception e)
                {
                    Servicos.Log.TratarErro(e);
                    errosRegras.Add("Tipo Operação");
                }

                erros = new List<string>();
                // Valida regra
                if (!ValidarEntidadeRegra("Tipo Operação", "TipoOperacao", regrasPedidoTipoOperacao, out erros))
                    throw new Exception(String.Join("<br>", erros));
            }
            else
            {
                regrasPedidoTipoOperacao = new List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoTipoOperacao>();
            }
            #endregion

            #region Modelo Veicular
            // Preenche as regras apenas se a flag de uso for verdadeira
            if (regrasPedidos.RegraPorModeloVeicular)
            {
                // Preenche regra
                try
                {
                    PreencherEntidadeRegra("ModeloVeicularCarga", "RegrasModeloVeicular", false, ref regrasPedidoModeloVeicular, ref regrasPedidos, ((codigo) =>
                    {
                        Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);

                        int codigoInt = 0;
                        int.TryParse(codigo.ToString(), out codigoInt);

                        return repModeloVeicularCarga.BuscarPorCodigo(codigoInt);
                    }));
                }
                catch (Exception e)
                {
                    Servicos.Log.TratarErro(e);
                    errosRegras.Add("Modelo Veícular");
                }

                erros = new List<string>();
                // Valida regra
                if (!ValidarEntidadeRegra("Modelo Veícular", "ModeloVeicularCarga", regrasPedidoModeloVeicular, out erros))
                    throw new Exception(String.Join("<br>", erros));
            }
            else
            {
                regrasPedidoModeloVeicular = new List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoModeloVeicular>();
            }
            #endregion

            #region Situação do Colaborador
            // Preenche as regras apenas se a flag de uso for verdadeira
            if (regrasPedidos.RegraPorSituacaoColaborador)
            {
                // Preenche regra
                try
                {
                    PreencherEntidadeRegra("ColaboradorSituacao", "RegrasSituacaoColaborador", false, ref regrasPedidoSituacaoColaborador, ref regrasPedidos, ((codigo) =>
                    {
                        Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorSituacao repColaboradorSituacao = new Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorSituacao(unitOfWork);

                        int codigoInt = 0;
                        int.TryParse(codigo.ToString(), out codigoInt);

                        return repColaboradorSituacao.BuscarPorCodigo(codigoInt);
                    }));
                }
                catch (Exception e)
                {
                    Servicos.Log.TratarErro(e);
                    errosRegras.Add("Situação do Colaborador");
                }

                erros = new List<string>();
                // Valida regra
                if (!ValidarEntidadeRegra("Situação do Colaborador", "ColaboradorSituacao", regrasPedidoSituacaoColaborador, out erros))
                    throw new Exception(String.Join("<br>", erros));
            }
            else
            {
                regrasPedidoSituacaoColaborador = new List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoSituacaoColaborador>();
            }
            #endregion


            #region Valor Frete
            // Preenche as regras apenas se a flag de uso for verdadeira
            if (regrasPedidos.RegraPorValorFrete)
            {
                // Preenche regra
                try
                {
                    PreencherEntidadeRegra("Valor", "RegrasValorFrete", true, ref regrasPedidoValorFrete, ref regrasPedidos);
                }
                catch (Exception e)
                {
                    Servicos.Log.TratarErro(e);
                    errosRegras.Add("Valor do Frete");
                }

                erros = new List<string>();
                // Valida regra
                if (!ValidarEntidadeRegra("Valor do Frete", "Valor", regrasPedidoValorFrete, out erros))
                    throw new Exception(String.Join("<br>", erros));
            }
            else
            {
                regrasPedidoValorFrete = new List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoValorFrete>();
            }
            #endregion

            #region Distância
            // Preenche as regras apenas se a flag de uso for verdadeira
            if (regrasPedidos?.RegraPorDistancia ?? false)
            {
                // Preenche regra
                try
                {
                    PreencherEntidadeRegra("Distancia", "RegrasDistancia", true, ref regrasPedidoDistancia, ref regrasPedidos);
                }
                catch (Exception e)
                {
                    Servicos.Log.TratarErro(e);
                    errosRegras.Add("Distância");
                }

                erros = new List<string>();
                // Valida regra
                if (!ValidarEntidadeRegra("Distância", "Distancia", regrasPedidoDistancia, out erros))
                    throw new Exception(String.Join("<br>", erros));
            }
            else
            {
                regrasPedidoDistancia = new List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoDistancia>();
            }
            #endregion

            #region Percentual de Diferença do Frete Líquido para o Frete com Terceiro
            // Preenche as regras apenas se a flag de uso for verdadeira
            if (regrasPedidos?.RegraPorDiferencaFreteLiquidoParaFreteTerceiro ?? false)
            {
                // Preenche regra
                try
                {
                    PreencherEntidadeRegra("DiferencaFreteLiquidoParaFreteTerceiro", "RegrasDiferencaFreteLiquidoParaFreteTerceiro", true, ref regrasPedidoPercentualDiferencaFreteLiquidoParaFreteTerceiro, ref regrasPedidos);
                }
                catch (Exception e)
                {
                    Servicos.Log.TratarErro(e);
                    errosRegras.Add("Percentual de Diferença do Frete Líquido para o Frete com Terceiro");
                }

                erros = new List<string>();
                // Valida regra
                if (!ValidarEntidadeRegra("Percentual de Diferença do Frete Líquido para o Frete com Terceiro", "DiferencaFreteLiquidoParaFreteTerceiro", regrasPedidoPercentualDiferencaFreteLiquidoParaFreteTerceiro, out erros))
                    throw new Exception(String.Join("<br>", erros));
            }
            else
            {
                regrasPedidoPercentualDiferencaFreteLiquidoParaFreteTerceiro = new List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoPercentualDiferencaFreteLiquidoParaFreteTerceiro>();
            }
            #endregion
        }

        private void PreencherEntidadeRegra<T>(string nomePropriedade, string parametroJson, bool usarDynamic, ref List<T> regrasPorTipo, ref Dominio.Entidades.Embarcador.Pedidos.RegrasPedido regrasPedidos, Func<dynamic, object> lambda = null) where T : Dominio.Entidades.EntidadeBase
        {
            /* Descricao
             * RegrasAutorizacaoOcorrencia é passado com ref, pois é vinculado a regra específica (RegraPorTipo) e após inserir no banco, a referencia permanece com o Codigo válido
             * 
             * Esse método facilita a instancia de novas regras, já que todas possuem o mesmo padra
             * - RegraOcorrencia (Entidade Pai)
             * - Ordem
             * - Codicao
             * - Juncao
             * - TIPO
             * 
             * Esse último, é instanciado com o retorno do callback, já que é o único parametro que é modificado
             * Mas quando não for uma enteidade, mas um valor simples, basta usar a flag usarDynamic = true,
             * Fazendo isso é setado o valor que vem no RegrasPorTipo.Valor
             */

            // Converte json (com o parametro get)
            List<RegrasPorTipo> dynRegras = Newtonsoft.Json.JsonConvert.DeserializeObject<List<RegrasPorTipo>>(Request.Params(parametroJson));

            if (dynRegras == null)
                throw new Exception("Erro ao converter os dados recebidos.");

            // Variavel auxiliar
            PropertyInfo prop;

            // Itera retornos
            for (var i = 0; i < dynRegras.Count(); i++)
            {
                int.TryParse(dynRegras[i].Codigo.ToString(), out int codigoRegra);
                int indexRegraNaLista = -1;

                // Instancia o objeto T (T não possui construor new)
                T regra = default(T);
                if (codigoRegra > 0)
                {
                    for (int j = 0; j < regrasPorTipo.Count; j++)
                        if ((int)((dynamic)regrasPorTipo[j]).Codigo == codigoRegra)
                        {
                            indexRegraNaLista = j;
                            break;
                        }
                }

                if (indexRegraNaLista >= 0)
                {
                    regra = regrasPorTipo[indexRegraNaLista];
                    regra.Initialize();
                }
                else
                    regra = Activator.CreateInstance<T>();

                // Seta as propriedas da entidade
                //prop = regra.GetType().GetProperty("Codigo", BindingFlags.Public | BindingFlags.Instance);
                //prop.SetValue(regra, codigoRegra, null);

                prop = regra.GetType().GetProperty("RegrasPedido", BindingFlags.Public | BindingFlags.Instance);
                prop.SetValue(regra, regrasPedidos, null);

                prop = regra.GetType().GetProperty("Ordem", BindingFlags.Public | BindingFlags.Instance);
                prop.SetValue(regra, dynRegras[i].Ordem, null);

                prop = regra.GetType().GetProperty("Condicao", BindingFlags.Public | BindingFlags.Instance);
                prop.SetValue(regra, dynRegras[i].Condicao, null);

                prop = regra.GetType().GetProperty("Juncao", BindingFlags.Public | BindingFlags.Instance);
                prop.SetValue(regra, dynRegras[i].Juncao, null);

                if (!usarDynamic)
                {
                    // Executa lambda
                    var result = dynRegras[i].Entidade != null ? lambda(dynRegras[i].Entidade.Codigo) : null;

                    prop = regra.GetType().GetProperty(nomePropriedade, BindingFlags.Public | BindingFlags.Instance);
                    prop.SetValue(regra, result, null);
                }
                else
                {
                    prop = regra.GetType().GetProperty(nomePropriedade, BindingFlags.Public | BindingFlags.Instance);
                    if (prop.PropertyType.Name.Equals("Decimal"))
                    {
                        decimal valorDecimal = 0;
                        decimal.TryParse(dynRegras[i].Valor.ToString(), out valorDecimal);

                        prop.SetValue(regra, valorDecimal, null);
                    }
                    else
                    {
                        prop.SetValue(regra, dynRegras[i].Valor, null);
                    }
                }

                // Adiciona lista de retorno
                if (indexRegraNaLista >= 0)
                    regrasPorTipo[indexRegraNaLista] = regra;
                else
                    regrasPorTipo.Add(regra);
            }

        }

        private bool ValidarEntidade(Dominio.Entidades.Embarcador.Pedidos.RegrasPedido regrasPedidos, out List<string> erros)
        {
            erros = new List<string>();

            if (string.IsNullOrWhiteSpace(regrasPedidos.Descricao))
                erros.Add("Descrição é obrigatória.");

            if (regrasPedidos.NumeroAprovadores < 0)
                erros.Add("Número de Aprovadores é obrigatória.");

            if (regrasPedidos.Aprovadores.Count() < regrasPedidos.NumeroAprovadores)
                erros.Add("O número de aprovadores selecionados deve ser maior ou igual a " + regrasPedidos.NumeroAprovadores.ToString());

            return erros.Count() == 0;
        }

        private bool ValidarEntidadeRegra<T>(string nomeRegra, string nomePropriedade, List<T> regrasProTipo, out List<string> erros)
        {
            erros = new List<string>();

            if (regrasProTipo.Count() == 0)
                erros.Add("Nenhuma regra " + nomeRegra + " cadastrada.");
            else
            {
                // Variavel auxiliar
                PropertyInfo prop;

                // Itera validacao
                for (var i = 0; i < regrasProTipo.Count(); i++)
                {
                    var regra = regrasProTipo[i];
                    prop = regra.GetType().GetProperty(nomePropriedade, BindingFlags.Public | BindingFlags.Instance);

                    if (prop.GetValue(regra) == null)
                        erros.Add(nomeRegra + " da regra é obrigatório.");
                }
            }

            return erros.Count() == 0;
        }

        private RegrasPorTipo RetornaRegraPorTipoDyn<T>(dynamic obj, string paramentro, string paramentroDescricaoValor, bool usarValor = false)
        {
            // Variavel auxiliar
            PropertyInfo prop;


            prop = obj.GetType().GetProperty("Codigo", BindingFlags.Public | BindingFlags.Instance);
            int codigo = prop.GetValue(obj);

            prop = obj.GetType().GetProperty("Ordem", BindingFlags.Public | BindingFlags.Instance);
            int ordem = prop.GetValue(obj);

            prop = obj.GetType().GetProperty("Juncao", BindingFlags.Public | BindingFlags.Instance);
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia juncao = prop.GetValue(obj);

            prop = obj.GetType().GetProperty("Condicao", BindingFlags.Public | BindingFlags.Instance);
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia condicao = prop.GetValue(obj);


            ObjetoEntidade objetoEntidade = new ObjetoEntidade();
            dynamic valor = null;
            if (!usarValor)
            {
                prop = obj.GetType().GetProperty(paramentro, BindingFlags.Public | BindingFlags.Instance);
                dynamic entidade = prop.GetValue(obj);

                prop = entidade.GetType().GetProperty("Codigo", BindingFlags.Public | BindingFlags.Instance);
                dynamic codigoEntidade = prop.GetValue(entidade);

                prop = entidade.GetType().GetProperty(paramentroDescricaoValor, BindingFlags.Public | BindingFlags.Instance);
                string descricaoEntidade = prop.GetValue(entidade);

                objetoEntidade.Codigo = codigoEntidade;
                objetoEntidade.Descricao = descricaoEntidade;
            }
            else
            {
                prop = obj.GetType().GetProperty(paramentroDescricaoValor, BindingFlags.Public | BindingFlags.Instance);
                valor = prop.GetValue(obj);
            }

            RegrasPorTipo restorno = new RegrasPorTipo()
            {
                Codigo = codigo,
                Ordem = ordem,
                Juncao = juncao,
                Condicao = condicao,
                Entidade = objetoEntidade,
                Valor = valor,
            };
            return restorno;
        }
        #endregion
    }
}
