using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Controllers.Pedidos
{
    [CustomAuthorize("Pedidos/Navio")]
    public class NavioController : BaseController
    {
		#region Construtores

		public NavioController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                return new JsonpResult(ObterGridPesquisa());
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

        public async Task<IActionResult> ExportarPesquisa()
        {
            try
            {
                Models.Grid.Grid grid = ObterGridPesquisa();
                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoGerar);
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

                Repositorio.Embarcador.Pedidos.Navio repNavio = new Repositorio.Embarcador.Pedidos.Navio(unitOfWork);
                Repositorio.Embarcador.Configuracoes.IntegracaoEMP repConfiguracaoIntegracaoEMP = new Repositorio.Embarcador.Configuracoes.IntegracaoEMP(unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.Navio navio = new Dominio.Entidades.Embarcador.Pedidos.Navio();
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP configuracaoIntegracaoEMP = repConfiguracaoIntegracaoEMP.Buscar();

                if (configuracaoIntegracaoEMP?.AtivarIntegracaoRecebimentoNavioEMP ?? false)
                {
                    if (repNavio.PossuiNavioAtivoMesmoIMO(Request.GetStringParam("CodigoIMO")))
                        return new JsonpResult(false, true, "Já existe um navio cadastrado com o mesmo código IMO.");

                    if (repNavio.PossuiNavioAtivoMesmoVesselID(Request.GetStringParam("NavioID")))
                        return new JsonpResult(false, true, "Já existe um navio cadastrado com o mesmo ID.");
                }

                PreencherNavio(navio, unitOfWork);

                if (!string.IsNullOrWhiteSpace(navio.CodigoEmbarcacao))
                {
                    if (repNavio.ContemNavioMesmoNumeroEmbarcacao(navio.CodigoEmbarcacao, navio.Codigo))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, "Já existe um navio cadastrado com o mesmo número da embarcação.");
                    }
                }

                repNavio.Inserir(navio, Auditado);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
        }

        public async Task<IActionResult> Atualizar()
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Pedidos.Navio repNavio = new Repositorio.Embarcador.Pedidos.Navio(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.Navio navio = repNavio.BuscarPorCodigo(codigo, true);

                PreencherNavio(navio, unitOfWork);

                var codigoNavio = navio.CodigoNavio?.Replace(" ", "");
                if (!string.IsNullOrWhiteSpace(codigoNavio) && codigoNavio.Length != 3)
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, $"O campo 'Código do Navio' precisa ter 3 dígitos. Verifique o valor digitado: {navio.CodigoNavio}.");
                }
                if (!string.IsNullOrWhiteSpace(navio.CodigoEmbarcacao))
                {
                    if (repNavio.ContemNavioMesmoNumeroEmbarcacao(navio.CodigoEmbarcacao, navio.Codigo))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, "Já existe um navio cadastrado com o mesmo número da embarcação.");
                    }
                }

                repNavio.Atualizar(navio, Auditado);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Pedidos.Navio repNavio = new Repositorio.Embarcador.Pedidos.Navio(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.Navio navio = repNavio.BuscarPorCodigo(codigo, false);

                string CodigoNavio;

                var dynNavio = new
                {
                    navio.Codigo,
                    navio.Descricao,
                    navio.CodigoIntegracao,
                    navio.CodigoDocumento,
                    navio.Status,
                    navio.Irin,
                    navio.CodigoEmbarcacao,
                    navio.TipoEmbarcacao,
                    navio.CodigoIMO,
                    CapacidadePlug = navio.CapacidadePlug.ToString("n4"),
                    CapacidadeTeus = navio.CapacidadeTeus.ToString("n4"),
                    CapacidadeTons = navio.CapacidadeTons.ToString("n4"),
                    navio.NavioID,
                    navio.CodigoOperador,
                    CodigoNavio = navio.CodigoNavio?.Replace(" ", ""),
                    Operadores = navio.Operadores != null && navio.Operadores.Count > 0 ?
                          (
                          from o in navio.Operadores
                          select new
                          {
                              o.Codigo,
                              o.CodigoIntegracao,
                              o.CodigoOperador,
                              o.IdOperador,
                              Status = o.Status.ObterDescricao(),
                              DataAtivo = o.DataAtivo?.ToString("dd/MM/yyyy") ?? "",
                              DataInativo = o.DataInativo?.ToString("dd/MM/yyyy") ?? ""
                          }
                    ).ToList() : null
                };

                return new JsonpResult(dynNavio);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Pedidos.Navio repNavio = new Repositorio.Embarcador.Pedidos.Navio(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.Navio navio = repNavio.BuscarPorCodigo(codigo, true);

                if (navio == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                repNavio.Deletar(navio, Auditado);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, false, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseja mais utilizá-lo.");
                else
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
                }
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaNavio ObterFiltrosPesquisa()
        {
            Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaNavio filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaNavio()
            {
                CodigoIntegracao = Request.GetStringParam("CodigoIntegracao"),
                Descricao = Request.GetStringParam("Descricao"),
                Status = Request.GetEnumParam("Status", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo),
                TipoEmbarcacao = Request.GetEnumParam("TipoEmbarcacao", Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmbarcacao.Nenhum)
            };
            return filtrosPesquisa;
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaNavio filtrosPesquisa = ObterFiltrosPesquisa();

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 40, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Código Integração", "CodigoIntegracao", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Irin", "Irin", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("CapacidadePlug", false);
                grid.AdicionarCabecalho("CapacidadeTeus", false);
                grid.AdicionarCabecalho("CapacidadeTons", false);

                if (filtrosPesquisa.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Status", "DescricaoStatus", 10, Models.Grid.Align.center, false);

                Repositorio.Embarcador.Pedidos.Navio repositorioNavio = new Repositorio.Embarcador.Pedidos.Navio(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                int totalRegistros = repositorioNavio.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Pedidos.Navio> navios = repositorioNavio.Consultar(filtrosPesquisa, parametrosConsulta);

                grid.setarQuantidadeTotal(repositorioNavio.ContarConsulta(filtrosPesquisa));

                var lista = (from p in navios
                             select new
                             {
                                 p.Codigo,
                                 p.Descricao,
                                 p.CodigoIntegracao,
                                 p.DescricaoStatus,
                                 p.Irin,
                                 CapacidadePlug = p.CapacidadePlug.ToString("n4"),
                                 CapacidadeTeus = p.CapacidadeTeus.ToString("n4"),
                                 CapacidadeTons = p.CapacidadeTons.ToString("n4")
                             }).ToList();

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
        }

        private void PreencherNavio(Dominio.Entidades.Embarcador.Pedidos.Navio navio, Repositorio.UnitOfWork unitOfWork)
        {
            bool.TryParse(Request.Params("Status"), out bool status);

            string descricao = Request.Params("Descricao");
            string codigoIntegracao = Request.Params("CodigoIntegracao");
            string irin = Request.Params("Irin");
            string codigoEmbarcacao = Request.Params("CodigoEmbarcacao");

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmbarcacao tipoEmbarcacao;
            Enum.TryParse(Request.Params("TipoEmbarcacao"), out tipoEmbarcacao);

            navio.Descricao = descricao;
            navio.CodigoIntegracao = codigoIntegracao;
            navio.CodigoDocumento = Request.GetStringParam("CodigoDocumento");
            navio.CodigoIMO = Request.GetStringParam("CodigoIMO");
            navio.Status = status;
            navio.Irin = irin;
            navio.CodigoEmbarcacao = codigoEmbarcacao;
            navio.TipoEmbarcacao = tipoEmbarcacao;
            navio.Integrado = false;
            navio.CapacidadePlug = Request.GetDecimalParam("CapacidadePlug");
            navio.CapacidadeTeus = Request.GetDecimalParam("CapacidadeTeus");
            navio.CapacidadeTons = Request.GetDecimalParam("CapacidadeTons");
            navio.NavioID = Request.GetStringParam("NavioID");
            navio.CodigoOperador = Request.GetStringParam("CodigoOperador");
            navio.CodigoNavio = Request?.GetStringParam("CodigoNavio").Replace(" ", "");

        }

        #endregion
    }
}
