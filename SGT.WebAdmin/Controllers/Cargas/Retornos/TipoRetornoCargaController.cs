using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.Retornos
{
    [CustomAuthorize("Cargas/TipoRetornoCarga")]
    public class TipoRetornoCargaController : BaseController
    {
		#region Construtores

		public TipoRetornoCargaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.Entidades.Embarcador.Cargas.Retornos.TipoRetornoCarga tipoRetornoCarga = new Dominio.Entidades.Embarcador.Cargas.Retornos.TipoRetornoCarga();

                PreencherEntidade(tipoRetornoCarga, unitOfWork);

                unitOfWork.Start();

                new Repositorio.Embarcador.Cargas.Retornos.TipoRetornoCarga(unitOfWork).Inserir(tipoRetornoCarga, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAdicionar);
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
                Repositorio.Embarcador.Cargas.Retornos.TipoRetornoCarga repositorioTipoRetornoCarga = new Repositorio.Embarcador.Cargas.Retornos.TipoRetornoCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Retornos.TipoRetornoCarga tipoRetornoCarga = repositorioTipoRetornoCarga.BuscarPorCodigo(codigo, auditavel: true);

                if (tipoRetornoCarga == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                PreencherEntidade(tipoRetornoCarga, unitOfWork);

                unitOfWork.Start();

                repositorioTipoRetornoCarga.Atualizar(tipoRetornoCarga, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAtualizar);
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
                Repositorio.Embarcador.Cargas.Retornos.TipoRetornoCarga repositorioTipoRetornoCarga = new Repositorio.Embarcador.Cargas.Retornos.TipoRetornoCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Retornos.TipoRetornoCarga tipoRetornoCarga = repositorioTipoRetornoCarga.BuscarPorCodigo(codigo);

                if (tipoRetornoCarga == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                return new JsonpResult(new
                {
                    tipoRetornoCarga.Codigo,
                    tipoRetornoCarga.CodigoIntegracao,
                    tipoRetornoCarga.Descricao,
                    tipoRetornoCarga.ExigeClienteColeta,
                    tipoRetornoCarga.GerarCargaDeColeta,
                    tipoRetornoCarga.Tipo,
                    TipoOperacao = new { Codigo = tipoRetornoCarga.TipoOperacao?.Codigo ?? 0, Descricao = tipoRetornoCarga.TipoOperacao?.Descricao ?? "" },
                    TipoOperacaoCargaColeta = new { Codigo = tipoRetornoCarga.TipoOperacaoCargaColeta?.Codigo ?? 0, Descricao = tipoRetornoCarga.TipoOperacaoCargaColeta?.Descricao ?? "" },
                    tipoRetornoCarga.Ativo
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoBuscarPorCodigo);
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
                int codigo = int.Parse(Request.Params("codigo"));
                Repositorio.Embarcador.Cargas.Retornos.TipoRetornoCarga repTipoRetornoCarga = new Repositorio.Embarcador.Cargas.Retornos.TipoRetornoCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Retornos.TipoRetornoCarga tipoRetornoCarga = repTipoRetornoCarga.BuscarPorCodigo(codigo);
                repTipoRetornoCarga.Deletar(tipoRetornoCarga, Auditado);
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, Localization.Resources.Cargas.TipoRetornoCarga.NaoFoiPossivelExcluirRegistro);
                else
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExcluir);
                }
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
                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaTipoRetornoCarga filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("ExigeClienteColeta", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 80, Models.Grid.Align.left, true);

                if (filtrosPesquisa.SituacaoAtivo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "DescricaoAtivo", 20, Models.Grid.Align.center, false);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                Repositorio.Embarcador.Cargas.Retornos.TipoRetornoCarga repositorioTipoRetornoCarga = new Repositorio.Embarcador.Cargas.Retornos.TipoRetornoCarga(unitOfWork);
                int totalRegistros = repositorioTipoRetornoCarga.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Cargas.Retornos.TipoRetornoCarga> listaTipoRetornoCarga = totalRegistros > 0 ? repositorioTipoRetornoCarga.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Cargas.Retornos.TipoRetornoCarga>();

                var listaRetornar = (
                    from tipo in listaTipoRetornoCarga
                    select new
                    {
                        tipo.Codigo,
                        tipo.Descricao,
                        tipo.ExigeClienteColeta,
                        tipo.DescricaoAtivo
                    }
                ).ToList();

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(listaRetornar);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaRetornoCargaTipo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaTipoRetornoCarga filtrosPesquisa = ObterFiltrosPesquisaRetornoCargaTipo();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta()
                {
                    DirecaoOrdenar = "asc",
                    PropriedadeOrdenar = "Descricao"
                };
                Repositorio.Embarcador.Cargas.Retornos.TipoRetornoCarga repositorioTipoRetornoCarga = new Repositorio.Embarcador.Cargas.Retornos.TipoRetornoCarga(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.Retornos.TipoRetornoCarga> listaTipoRetornoCarga = repositorioTipoRetornoCarga.Consultar(filtrosPesquisa, parametrosConsulta);

                var listaRetornar = (
                    from tipo in listaTipoRetornoCarga
                    select new
                    {
                        value = tipo.Codigo,
                        text = tipo.Descricao
                    }
                ).ToList();

                return new JsonpResult(listaRetornar);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaTipoRetornoCarga ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaTipoRetornoCarga()
            {
                CodigoTipoOperacao = Request.GetIntParam("TipoOperacao"),
                Descricao = Request.GetStringParam("Descricao"),
                SituacaoAtivo = Request.GetEnumParam("Ativo", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaTipoRetornoCarga ObterFiltrosPesquisaRetornoCargaTipo()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaTipoRetornoCarga()
            {
                Tipo = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.RetornoCargaTipo>("Tipo"),
                SituacaoAtivo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo
            };
        }

        private void PreencherEntidade(Dominio.Entidades.Embarcador.Cargas.Retornos.TipoRetornoCarga tipoRetornoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            int codigoTipoOperacao = Request.GetIntParam("TipoOperacao");
            int codigoTipoOperacaoCargaColeta = Request.GetIntParam("TipoOperacaoCargaColeta");

            tipoRetornoCarga.Ativo = Request.GetBoolParam("Ativo");
            tipoRetornoCarga.CodigoIntegracao = Request.GetStringParam("CodigoIntegracao");
            tipoRetornoCarga.Descricao = Request.GetStringParam("Descricao");
            tipoRetornoCarga.ExigeClienteColeta = Request.GetBoolParam("ExigeClienteColeta");
            tipoRetornoCarga.GerarCargaDeColeta = Request.GetBoolParam("GerarCargaDeColeta");
            tipoRetornoCarga.Tipo = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.RetornoCargaTipo>("Tipo");
            tipoRetornoCarga.TipoOperacao = (codigoTipoOperacao > 0) ? new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork).BuscarPorCodigo(codigoTipoOperacao) : null;   
            tipoRetornoCarga.TipoOperacaoCargaColeta = (codigoTipoOperacaoCargaColeta > 0) ? new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork).BuscarPorCodigo(codigoTipoOperacaoCargaColeta) : null;
        }

        #endregion
    }
}
