using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pessoas
{
    [CustomAuthorize("Pessoas/EPI")]
    public class EPIController : BaseController
    {
		#region Construtores

		public EPIController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Pessoas.EPI repositorioEPI = new Repositorio.Embarcador.Pessoas.EPI(unitOfWork);
                
                Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaEPI filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 40, Models.Grid.Align.left, true);

                if (filtrosPesquisa.Ativo == SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Status", "DescricaoAtivo", 15, Models.Grid.Align.center, false);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Entidades.Embarcador.Pessoas.EPI> epi = repositorioEPI.Consultar(filtrosPesquisa, parametrosConsulta);
                grid.setarQuantidadeTotal(repositorioEPI.ContarConsulta(filtrosPesquisa));

                var lista = (from p in epi
                             select new
                             {
                                 p.Codigo,
                                 p.Descricao,
                                 p.DescricaoAtivo
                             }).ToList();

                grid.AdicionaRows(lista);
                return new JsonpResult(grid);
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

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Pessoas.EPI repositorioEPI = new Repositorio.Embarcador.Pessoas.EPI(unitOfWork);
                Dominio.Entidades.Embarcador.Pessoas.EPI epi = repositorioEPI.BuscarPorCodigo(codigo, false);

                if (epi == null)
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                var dynEPI = new
                {
                    epi.Codigo,
                    epi.Descricao,
                    epi.Ativo,
                    TipoEPI = new 
                    { 
                        Codigo = epi.TipoEPI?.Codigo ?? 0, 
                        Descricao = epi.TipoEPI?.Descricao ?? string.Empty,  
                        Tamanho = epi.TipoEPI?.Tamanho ?? false,
                        DiasRevisao = epi.TipoEPI?.DiasRevisao ?? false,
                        DiasValidade = epi.TipoEPI?.DiasValidade ?? false,
                        Descartavel = epi.TipoEPI?.Descartavel ?? false,
                        InstrucaoUso = epi.TipoEPI?.InstrucaoUso ?? false,
                        Valor = epi.TipoEPI?.Valor ?? false,
                        NumeroCertificado = epi.TipoEPI?.NumeroCertificado ?? false,
                        Caracteristica = epi.TipoEPI?.Caracteristica ?? false,
                    },
                    epi.Tamanho,
                    epi.DiasRevisao,
                    epi.DiasValidade,
                    epi.Descartavel,
                    epi.InstrucaoUso,
                    epi.Valor,
                    epi.NumeroCertificado,
                    epi.Caracteristica
                };

                return new JsonpResult(dynEPI);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoBuscar);
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

                Repositorio.Embarcador.Pessoas.EPI repositorioEPI = new Repositorio.Embarcador.Pessoas.EPI(unitOfWork);
                Dominio.Entidades.Embarcador.Pessoas.EPI epi = new Dominio.Entidades.Embarcador.Pessoas.EPI();

                PreencherEPI(epi, unitOfWork);

                repositorioEPI.Inserir(epi, Auditado);

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
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Pessoas.EPI repositorioEPI = new Repositorio.Embarcador.Pessoas.EPI(unitOfWork);
                Dominio.Entidades.Embarcador.Pessoas.EPI epi = repositorioEPI.BuscarPorCodigo(codigo, true);

                if (epi == null)
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                PreencherEPI(epi, unitOfWork);

                repositorioEPI.Atualizar(epi, Auditado);

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

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Pessoas.EPI repositorioEPI = new Repositorio.Embarcador.Pessoas.EPI(unitOfWork);
                Dominio.Entidades.Embarcador.Pessoas.EPI epi = repositorioEPI.BuscarPorCodigo(codigo, true);

                if (epi == null)
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                repositorioEPI.Deletar(epi, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelExcluirRegistro);
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

        #endregion

        #region Métodos Privados

        private void PreencherEPI(Dominio.Entidades.Embarcador.Pessoas.EPI epi, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pessoas.TipoEPI repositorioTipoEPI = new Repositorio.Embarcador.Pessoas.TipoEPI(unitOfWork);

            epi.Descricao = Request.GetStringParam("Descricao");
            epi.Ativo = Request.GetBoolParam("Ativo");

            int codigoTipoEPI = Request.GetIntParam("TipoEPI");
            epi.TipoEPI = codigoTipoEPI > 0 ? repositorioTipoEPI.BuscarPorCodigo(codigoTipoEPI, false) : null;

            epi.Tamanho = (epi.TipoEPI?.Tamanho ?? false)  ? Request.GetIntParam("Tamanho") : 0;
            epi.DiasRevisao = (epi.TipoEPI?.DiasRevisao ?? false) ? Request.GetIntParam("DiasRevisao") : 0;
            epi.DiasValidade = (epi.TipoEPI?.DiasValidade ?? false) ? Request.GetIntParam("DiasValidade") : 0;
            epi.Descartavel = (epi.TipoEPI?.Descartavel ?? false) ? Request.GetBoolParam("Descartavel") : false;
            epi.InstrucaoUso = (epi.TipoEPI?.InstrucaoUso ?? false) ? Request.GetStringParam("InstrucaoUso") : string.Empty;
            epi.Valor = (epi.TipoEPI?.Valor ?? false) ? Request.GetDecimalParam("Valor") : 0m;
            epi.NumeroCertificado = (epi.TipoEPI?.NumeroCertificado ?? false) ? Request.GetStringParam("NumeroCertificado") : string.Empty;
            epi.Caracteristica = (epi.TipoEPI?.Caracteristica ?? false) ? Request.GetStringParam("Caracteristica") : string.Empty;
        }

        private Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaEPI ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaEPI()
            {
                Descricao = Request.GetStringParam("Descricao"),
                Ativo = Request.GetEnumParam<SituacaoAtivoPesquisa>("Ativo")
            };
        }

        #endregion
    }
}
