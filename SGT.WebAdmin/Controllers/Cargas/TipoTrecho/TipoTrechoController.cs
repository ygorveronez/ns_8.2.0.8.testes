using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using Servicos.Cache;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;


namespace SGT.WebAdmin.Controllers.Cargas
{
    [CustomAuthorize("Cargas/TipoTrecho")]
    public class TipoTrechoController : BaseController
    {
		#region Construtores

		public TipoTrechoController(Conexao conexao) : base(conexao) { }

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
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.TipoTrecho repositorioTipoTrecho = new Repositorio.Embarcador.Cargas.TipoTrecho(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.TipoTrecho TipoTrecho = new Dominio.Entidades.Embarcador.Cargas.TipoTrecho();

                PreencherTipoTrecho(TipoTrecho, unitOfWork);

                unitOfWork.Start();

                if (!VerificarDuplicidade(TipoTrecho, repositorioTipoTrecho))
                {
                    repositorioTipoTrecho.Inserir(TipoTrecho, Auditado);
                    Infrastructure.Services.Cache.CacheProvider.Instance.Remove("TipoTrechos");
                }
                else
                    throw new ControllerException("Já existe um Tipo de Trecho com esses dados");


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
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar os dados.");
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
                Repositorio.Embarcador.Cargas.TipoTrecho repositorioTipoTrecho = new Repositorio.Embarcador.Cargas.TipoTrecho(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.TipoTrecho TipoTrecho = repositorioTipoTrecho.BuscarPorCodigo(codigo, auditavel: true);

                if (TipoTrecho == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro");

                PreencherTipoTrecho(TipoTrecho, unitOfWork);

                unitOfWork.Start();

                if (!VerificarDuplicidade(TipoTrecho, repositorioTipoTrecho))
                {
                    repositorioTipoTrecho.Atualizar(TipoTrecho, Auditado);
                    Infrastructure.Services.Cache.CacheProvider.Instance.Remove("TipoTrechos");
                }
                else
                    throw new ControllerException("Impossivel atualizar já existe um Tipo de Trecho com esses dados");

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
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
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
                Repositorio.Embarcador.Cargas.TipoTrecho repositorioTipoTrecho = new Repositorio.Embarcador.Cargas.TipoTrecho(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.TipoTrecho TipoTrecho = repositorioTipoTrecho.BuscarPorCodigo(codigo, auditavel: true);

                if (TipoTrecho == null)
                    throw new ControllerException(Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                repositorioTipoTrecho.Deletar(TipoTrecho, Auditado);
                Infrastructure.Services.Cache.CacheProvider.Instance.Remove("TipoTrechos");
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

                if (ExcessaoPorPossuirDependeciasNoBanco(excecao))
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelExcluirRegistro);

                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExcluir);
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
                Repositorio.Embarcador.Cargas.TipoTrecho repositorioTipoTrecho = new Repositorio.Embarcador.Cargas.TipoTrecho(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.TipoTrecho TipoTrecho = repositorioTipoTrecho.BuscarPorCodigo(codigo, auditavel: false);

                if (TipoTrecho == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro");

                return new JsonpResult(new
                {
                    TipoTrecho.Codigo,
                    TipoTrecho.Descricao,
                    Situacao = TipoTrecho.Ativo,
                    ListaTiposOperacao = (
                        from tipoOperacao in TipoTrecho.TiposOperacao
                        select new
                        {
                            tipoOperacao.Codigo,
                            tipoOperacao.Descricao
                        }).ToList(),
                    ListaCategoriasOrigem = (
                        from catOrigem in TipoTrecho.CategoriasOrigem
                        select new
                        {
                            catOrigem.Codigo,
                            catOrigem.Descricao
                        }).ToList(),
                    ListaCategoriasDestino = (
                        from catDestino in TipoTrecho.CategoriasDestino
                        select new
                        {
                            catDestino.Codigo,
                            catDestino.Descricao
                        }).ToList(),
                    ListaCategoriasExpedidor = (
                        from catExpedidor in TipoTrecho.CategoriasExpedidor
                        select new
                        {
                            catExpedidor.Codigo,
                            catExpedidor.Descricao
                        }).ToList(),
                    ListaCategoriasRecebedor = (
                        from catRecebedor in TipoTrecho.CategoriasRecebedor
                        select new
                        {
                            catRecebedor.Codigo,
                            catRecebedor.Descricao
                        }).ToList(),
                    ListaModelosVeiculares = (
                        from mv in TipoTrecho.ModelosVeiculares
                        select new
                        {
                            mv.Codigo,
                            mv.Descricao
                        }).ToList(),
                    ListaClientesOrigem = (
                        from mv in TipoTrecho.ClientesOrigem
                        select new
                        {
                            mv.Codigo,
                            mv.Descricao
                        }).ToList(),
                    ListaClientesDestino = (
                        from mv in TipoTrecho.ClientesDestino
                        select new
                        {
                            mv.Codigo,
                            mv.Descricao
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

        #endregion

        #region Métodos Privados
        private bool VerificarDuplicidade(Dominio.Entidades.Embarcador.Cargas.TipoTrecho TipoTrecho, Repositorio.Embarcador.Cargas.TipoTrecho repositorioTipoTrecho)
        {
            return repositorioTipoTrecho.ExisteDuplicado(TipoTrecho);
        }

        private void PreencherTipoTrecho(Dominio.Entidades.Embarcador.Cargas.TipoTrecho TipoTrecho, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacao repTO = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.Embarcador.Pessoas.CategoriaPessoa repCatPessoa = new Repositorio.Embarcador.Pessoas.CategoriaPessoa(unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModVeic = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);
            List<int> codigosTO = Request.GetListParam<int>("CodigosTiposOperacao");
            List<int> codigosCatOrigem = Request.GetListParam<int>("CodigosCategoriasOrigem");
            List<int> codigosCatDestino = Request.GetListParam<int>("CodigosCategoriasDestino");
            List<int> codigosCatRecebedor = Request.GetListParam<int>("CodigosCategoriasRecebedor");
            List<int> codigosCatExpedidor = Request.GetListParam<int>("CodigosCategoriasExpedidor");
            List<int> codigosMV = Request.GetListParam<int>("CodigosModelosVeiculares");
            List<double> codigosCO = Request.GetListParam<double>("CodigosClientesOrigem");
            List<double> codigosCD = Request.GetListParam<double>("CodigosClientesDestino");
            List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> tiposOperacao = codigosTO.Count > 0 ? repTO.BuscarPorCodigos(codigosTO) : new List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();
            List<Dominio.Entidades.Embarcador.Pessoas.CategoriaPessoa> categoriasOrigem = codigosCatOrigem.Count > 0 ? repCatPessoa.BuscarPorCodigos(codigosCatOrigem) : new List<Dominio.Entidades.Embarcador.Pessoas.CategoriaPessoa>();
            List<Dominio.Entidades.Embarcador.Pessoas.CategoriaPessoa> categoriasDestino = codigosCatDestino.Count > 0 ? repCatPessoa.BuscarPorCodigos(codigosCatDestino) : new List<Dominio.Entidades.Embarcador.Pessoas.CategoriaPessoa>();
            List<Dominio.Entidades.Embarcador.Pessoas.CategoriaPessoa> categoriasRecebedor = codigosCatRecebedor.Count > 0 ? repCatPessoa.BuscarPorCodigos(codigosCatRecebedor) : new List<Dominio.Entidades.Embarcador.Pessoas.CategoriaPessoa>();
            List<Dominio.Entidades.Embarcador.Pessoas.CategoriaPessoa> categoriasExpedidor = codigosCatExpedidor.Count > 0 ? repCatPessoa.BuscarPorCodigos(codigosCatExpedidor) : new List<Dominio.Entidades.Embarcador.Pessoas.CategoriaPessoa>();
            List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> modelosVeiculares = codigosMV.Count > 0 ? repModVeic.BuscarPorCodigos(codigosMV) : new List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>();
            List<Dominio.Entidades.Cliente> clientesOrigem = codigosCO.Count > 0 ? repositorioCliente.BuscarPorCodigos(codigosCO) : new List<Dominio.Entidades.Cliente>();
            List<Dominio.Entidades.Cliente> clientesDestino = codigosCD.Count > 0 ? repositorioCliente.BuscarPorCodigos(codigosCD) : new List<Dominio.Entidades.Cliente>();
            TipoTrecho.Descricao = Request.GetStringParam("Descricao");
            TipoTrecho.Ativo = Request.GetBoolParam("Situacao");
            TipoTrecho.TiposOperacao = tiposOperacao;
            TipoTrecho.CategoriasOrigem = categoriasOrigem;
            TipoTrecho.CategoriasDestino = categoriasDestino;
            TipoTrecho.CategoriasRecebedor = categoriasRecebedor;
            TipoTrecho.CategoriasExpedidor = categoriasExpedidor;
            TipoTrecho.ModelosVeiculares = modelosVeiculares;
            TipoTrecho.ClientesOrigem = clientesOrigem;
            TipoTrecho.ClientesDestino = clientesDestino;
        }

        private Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaTipoTrecho ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaTipoTrecho()
            {
                Descricao = Request.GetStringParam("Descricao"),
                Situacao = Request.GetEnumParam<SituacaoAtivoPesquisa>("Situacao")
            };
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaTipoTrecho filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 50, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Situação", "Situacao", 50, Models.Grid.Align.left, false);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);

                Repositorio.Embarcador.Cargas.TipoTrecho repositorioTipoTrecho = new Repositorio.Embarcador.Cargas.TipoTrecho(unitOfWork);
                int totalRegistros = repositorioTipoTrecho.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Cargas.TipoTrecho> listaTipoTrecho = (totalRegistros > 0) ? repositorioTipoTrecho.Consultar(parametrosConsulta, filtrosPesquisa) : new List<Dominio.Entidades.Embarcador.Cargas.TipoTrecho>();

                var listaTipoTrechoRetornar = (
                    from TipoTrecho in listaTipoTrecho
                    select new
                    {
                        TipoTrecho.Codigo,
                        TipoTrecho.Descricao,
                        Situacao = TipoTrecho.Ativo ? SituacaoAtivoPesquisa.Ativo.ObterDescricao() : SituacaoAtivoPesquisa.Inativo.ObterDescricao()
                    }
                ).ToList();

                grid.AdicionaRows(listaTipoTrechoRetornar);
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
            if (propriedadeOrdenar == "DescricaoAtivo")
                return "Ativo";

            return propriedadeOrdenar;
        }
        #endregion
    }
}
