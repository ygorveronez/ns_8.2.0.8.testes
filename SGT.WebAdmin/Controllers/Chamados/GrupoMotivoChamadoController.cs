using System;
using System.Collections.Generic;
using System.Linq;
using SGTAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Excecoes.Embarcador;

namespace SGT.WebAdmin.Controllers.Chamados
{
    [CustomAuthorize("Chamados/GrupoMotivoChamado")]
    public class GrupoMotivoChamadoController : BaseController
    {
		#region Construtores

		public GrupoMotivoChamadoController(Conexao conexao) : base(conexao) { }

		#endregion

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                return new JsonpResult(ObterGridPesquisa(unitOfWork));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
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
                Repositorio.Embarcador.Chamados.GrupoMotivoChamado repGrupoMotivoChamado = new Repositorio.Embarcador.Chamados.GrupoMotivoChamado(unitOfWork);
                Repositorio.Embarcador.Chamados.GrupoMotivoChamadoTipoIntegracao repGrupoMotivoChamadoTipoIntegracao = new Repositorio.Embarcador.Chamados.GrupoMotivoChamadoTipoIntegracao(unitOfWork);

                Dominio.Entidades.Embarcador.Chamados.GrupoMotivoChamado grupo = new Dominio.Entidades.Embarcador.Chamados.GrupoMotivoChamado();
                List<Dominio.Entidades.Embarcador.Chamados.GrupoMotivoChamadoTipoIntegracao> grupoMotivoChamadoTipoIntegracao = new List<Dominio.Entidades.Embarcador.Chamados.GrupoMotivoChamadoTipoIntegracao>();

                unitOfWork.Start();

                PreencherEntidade(grupo, grupoMotivoChamadoTipoIntegracao, unitOfWork);

                repGrupoMotivoChamado.Inserir(grupo, Auditado);

                foreach (var grupoTipoIntegracao in grupoMotivoChamadoTipoIntegracao)
                {
                    repGrupoMotivoChamadoTipoIntegracao.Inserir(grupoTipoIntegracao);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
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
                Repositorio.Embarcador.Chamados.GrupoMotivoChamado repGrupoMotivoChamado = new Repositorio.Embarcador.Chamados.GrupoMotivoChamado(unitOfWork);
                Repositorio.Embarcador.Chamados.GrupoMotivoChamadoTipoIntegracao repGrupoMotivoChamadoTipoIntegracao = new Repositorio.Embarcador.Chamados.GrupoMotivoChamadoTipoIntegracao(unitOfWork);

                int codigoGenero = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Chamados.GrupoMotivoChamado grupo = repGrupoMotivoChamado.BuscarPorCodigo(codigoGenero, false);
                List<Dominio.Entidades.Embarcador.Chamados.GrupoMotivoChamadoTipoIntegracao> grupoMotivoChamadoTipoIntegracao = repGrupoMotivoChamadoTipoIntegracao.BuscarPorGrupoMotivo(grupo.Codigo);

                if (grupo == null)
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                unitOfWork.Start();

                foreach (var grupoTipoIntegracao in grupoMotivoChamadoTipoIntegracao)
                {
                    repGrupoMotivoChamadoTipoIntegracao.Deletar(grupoTipoIntegracao);
                }
                grupoMotivoChamadoTipoIntegracao.Clear();

                PreencherEntidade(grupo, grupoMotivoChamadoTipoIntegracao, unitOfWork);

                repGrupoMotivoChamado.Inserir(grupo, Auditado);

                foreach (var grupoTipoIntegracao in grupoMotivoChamadoTipoIntegracao)
                {
                    repGrupoMotivoChamadoTipoIntegracao.Inserir(grupoTipoIntegracao);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
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
                Repositorio.Embarcador.Chamados.GrupoMotivoChamado repGrupoMotivo = new Repositorio.Embarcador.Chamados.GrupoMotivoChamado(unitOfWork);
                Repositorio.Embarcador.Chamados.GrupoMotivoChamadoTipoIntegracao repGrupoMotivoChamadoTI = new Repositorio.Embarcador.Chamados.GrupoMotivoChamadoTipoIntegracao(unitOfWork);

                int codigoGrupo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Chamados.GrupoMotivoChamado grupo = repGrupoMotivo.BuscarPorCodigo(codigoGrupo, false);
                List<Dominio.Entidades.Embarcador.Chamados.GrupoMotivoChamadoTipoIntegracao> grupoMotivoChamadoTipoIntegracao = repGrupoMotivoChamadoTI.BuscarPorGrupoMotivo(grupo.Codigo);
                if (grupo == null)
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);


                var retorno = new
                {
                    grupo.Codigo,
                    grupo.Descricao,
                    grupo.CodigoIntegracao,
                    grupo.Situacao,
                    grupo.GeraOcorrenciaNormal,
                    grupo.GeraCargaAvulsa,
                    grupo.NecessarioAprovacaoCriacaoCargaAvulsa,
                    grupo.GeraCargaReversa,
                    grupo.NaoPermiteLancamentoManual,
                    grupo.Sinistro,
                    grupo.RecebeOcorrenciaERP,

                    TiposIntegracoes = (from obj in grupoMotivoChamadoTipoIntegracao
                                        select new
                                        {
                                            CodigoSistemaIntegracao = (int)obj.TipoIntegracao,
                                            DescricaoSistemaIntegracao = obj.TipoIntegracao.ObterDescricao()
                                        }).ToList(),

                    TiposOperacao = (from obj in grupo.TiposOperacao
                                     select new
                                     {
                                         Codigo = obj.Codigo,
                                         Descricao = obj.Descricao
                                     }).ToList()
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAtualizar);
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
                Repositorio.Embarcador.Chamados.GrupoMotivoChamado repositorioGenero = new Repositorio.Embarcador.Chamados.GrupoMotivoChamado(unitOfWork);

                int codigoGenero = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Chamados.GrupoMotivoChamado grupo = repositorioGenero.BuscarPorCodigo(codigoGenero, false);

                if (grupo == null)
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                unitOfWork.Start();

                repositorioGenero.Deletar(grupo, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExcluir);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #region Métodos Privados

        private Models.Grid.Grid ObterGrid()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Código Integração", "CodigoIntegracao", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Descrição", "Descricao", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Situação", "Situacao", 25, Models.Grid.Align.left, false);

            return grid;
        }

        private Models.Grid.Grid ObterGridPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Chamados.GrupoMotivoChamado repositorioGenero = new Repositorio.Embarcador.Chamados.GrupoMotivoChamado(unitOfWork);
            Repositorio.Embarcador.Chamados.GrupoMotivoChamadoTipoIntegracao repGrupoMotivoChamadoTipoIntegracao = new Repositorio.Embarcador.Chamados.GrupoMotivoChamadoTipoIntegracao(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaGrupoMotivoChamado filtrosPesquisa = ObterFiltrosPesquisa();

            Models.Grid.Grid grid = ObterGrid();

            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);

            int totalRegistros = repositorioGenero.ContarConsulta(filtrosPesquisa);
            List<Dominio.Entidades.Embarcador.Chamados.GrupoMotivoChamado> listaGrid = totalRegistros > 0 ? repositorioGenero.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Chamados.GrupoMotivoChamado>();

            var lista = from obj in listaGrid
                        select new
                        {
                            obj.Codigo,
                            obj.CodigoIntegracao,
                            obj.Descricao,
                            Situacao = obj.Situacao ? SituacaoAtivoPesquisa.Ativo.ObterDescricao() : SituacaoAtivoPesquisa.Inativo.ObterDescricao(),
                        };

            grid.AdicionaRows(lista);
            grid.setarQuantidadeTotal(totalRegistros);

            return grid;
        }

        public Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaGrupoMotivoChamado ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaGrupoMotivoChamado()
            {
                Descricao = Request.GetStringParam("Descricao"),
                CodigoIntegracao = Request.GetStringParam("CodigoIntegracao"),
                Situacao = Request.GetEnumParam<SituacaoAtivoPesquisa>("Situacao"),
            };
        }

        private string ObterPropriedadeOrdenar(string propOrdenar)
        {
            return propOrdenar;
        }

        private void PreencherEntidade(Dominio.Entidades.Embarcador.Chamados.GrupoMotivoChamado grupo, List<Dominio.Entidades.Embarcador.Chamados.GrupoMotivoChamadoTipoIntegracao> grupoMotivoChamadoTipoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacao repTOP = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);

            grupo.Descricao = Request.GetStringParam("Descricao");
            grupo.CodigoIntegracao = Request.GetStringParam("CodigoIntegracao");
            grupo.Situacao = Request.GetBoolParam("Situacao");
            grupo.GeraOcorrenciaNormal = Request.GetBoolParam("GeraOcorrenciaNormal");
            grupo.GeraCargaAvulsa = Request.GetBoolParam("GeraCargaAvulsa");

            if (grupo.GeraCargaAvulsa)
            {
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao top = repTOP.BuscarPorCodigo(Request.GetIntParam("TipoOperacao"));
                grupo.TipoOperacao = top != null ? top : throw new ControllerException("É obrigatório informar o Tipo de Operação quando o Grupo gera Carga avulsa");
            }

            grupo.NecessarioAprovacaoCriacaoCargaAvulsa = Request.GetBoolParam("NecessarioAprovacaoCriacaoCargaAvulsa");
            grupo.GeraCargaReversa = Request.GetBoolParam("GeraCargaReversa");
            grupo.NaoPermiteLancamentoManual = Request.GetBoolParam("NaoPermiteLancamentoManual");
            grupo.Sinistro = Request.GetBoolParam("Sinistro");
            grupo.RecebeOcorrenciaERP = Request.GetBoolParam("RecebeOcorrenciaERP");

            foreach (TipoIntegracao tipo in Request.GetListEnumParam<TipoIntegracao>("TiposIntegracao"))
            {
                var grupoMotivoChamadoIntegracao = new Dominio.Entidades.Embarcador.Chamados.GrupoMotivoChamadoTipoIntegracao()
                {
                    TipoIntegracao = tipo,
                    GrupoMotivoChamado = grupo
                };

                grupoMotivoChamadoTipoIntegracao.Add(grupoMotivoChamadoIntegracao);
            }

            grupo.TiposOperacao = repTOP.BuscarPorCodigos(Request.GetListParam<int>("TiposOperacao"));
        }

        #endregion
    }
}
