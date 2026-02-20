using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pessoas
{
    [CustomAuthorize("Pessoas/TipoEPI")]
    public class TipoEPIController : BaseController
    {
		#region Construtores

		public TipoEPIController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Pessoas.TipoEPI repositorioTipoEPI = new Repositorio.Embarcador.Pessoas.TipoEPI(unitOfWork);
                
                Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaTipoEPI filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 40, Models.Grid.Align.left, true);

                if (filtrosPesquisa.Ativo == SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Status", "DescricaoAtivo", 15, Models.Grid.Align.center, false);

                grid.AdicionarCabecalho("Tamanho", false);
                grid.AdicionarCabecalho("DiasRevisao", false);
                grid.AdicionarCabecalho("DiasValidade", false);
                grid.AdicionarCabecalho("Descartavel", false);
                grid.AdicionarCabecalho("InstrucaoUso", false);
                grid.AdicionarCabecalho("Valor", false);
                grid.AdicionarCabecalho("NumeroCertificado", false);
                grid.AdicionarCabecalho("Caracteristica", false);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                
                List<Dominio.Entidades.Embarcador.Pessoas.TipoEPI> tiposEPI = repositorioTipoEPI.Consultar(filtrosPesquisa, parametrosConsulta);
                grid.setarQuantidadeTotal(repositorioTipoEPI.ContarConsulta(filtrosPesquisa));

                var lista = (from p in tiposEPI
                             select new
                             {
                                 p.Codigo,
                                 p.Descricao,
                                 p.DescricaoAtivo,
                                 p.Tamanho,
                                 p.DiasRevisao,
                                 p.DiasValidade,
                                 p.Descartavel,
                                 p.InstrucaoUso,
                                 p.Valor,
                                 p.NumeroCertificado,
                                 p.Caracteristica
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

                Repositorio.Embarcador.Pessoas.TipoEPI repositorioTipoEPI = new Repositorio.Embarcador.Pessoas.TipoEPI(unitOfWork);
                Dominio.Entidades.Embarcador.Pessoas.TipoEPI tipoEPI = repositorioTipoEPI.BuscarPorCodigo(codigo, false);

                if (tipoEPI == null)
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                var dynTipoEPI = new
                {
                    tipoEPI.Codigo,
                    tipoEPI.Descricao,
                    tipoEPI.Ativo,
                    MarcaEPI = new { Codigo = tipoEPI.MarcaEPI?.Codigo ?? 0, Descricao = tipoEPI.MarcaEPI?.Descricao ?? string.Empty },
                    tipoEPI.Tamanho,
                    tipoEPI.DiasRevisao,
                    tipoEPI.DiasValidade,
                    tipoEPI.Descartavel,
                    tipoEPI.InstrucaoUso,
                    tipoEPI.Valor,
                    tipoEPI.NumeroCertificado,
                    tipoEPI.Caracteristica
                };

                return new JsonpResult(dynTipoEPI);
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

                Repositorio.Embarcador.Pessoas.TipoEPI repositorioTipoEPI = new Repositorio.Embarcador.Pessoas.TipoEPI(unitOfWork);
                Dominio.Entidades.Embarcador.Pessoas.TipoEPI tipoEPI = new Dominio.Entidades.Embarcador.Pessoas.TipoEPI();

                PreencherTipoEPI(tipoEPI, unitOfWork);

                repositorioTipoEPI.Inserir(tipoEPI, Auditado);

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

                Repositorio.Embarcador.Pessoas.TipoEPI repositorioTipoEPI = new Repositorio.Embarcador.Pessoas.TipoEPI(unitOfWork);
                Dominio.Entidades.Embarcador.Pessoas.TipoEPI tipoEPI = repositorioTipoEPI.BuscarPorCodigo(codigo, true);

                if (tipoEPI == null)
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                PreencherTipoEPI(tipoEPI, unitOfWork);

                repositorioTipoEPI.Atualizar(tipoEPI, Auditado);

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

                Repositorio.Embarcador.Pessoas.TipoEPI repositorioTipoEPI = new Repositorio.Embarcador.Pessoas.TipoEPI(unitOfWork);
                Dominio.Entidades.Embarcador.Pessoas.TipoEPI tipoEPI = repositorioTipoEPI.BuscarPorCodigo(codigo, true);

                if (tipoEPI == null)
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                repositorioTipoEPI.Deletar(tipoEPI, Auditado);

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

        private void PreencherTipoEPI(Dominio.Entidades.Embarcador.Pessoas.TipoEPI tipoEPI, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pessoas.MarcaEPI repositorioMarcaEPI = new Repositorio.Embarcador.Pessoas.MarcaEPI(unitOfWork);

            tipoEPI.Descricao = Request.GetStringParam("Descricao");
            tipoEPI.Ativo = Request.GetBoolParam("Ativo");

            int codigoMarcaEPI = Request.GetIntParam("MarcaEPI");
            tipoEPI.MarcaEPI = codigoMarcaEPI > 0 ? repositorioMarcaEPI.BuscarPorCodigo(codigoMarcaEPI, false) : null;

            tipoEPI.Tamanho = Request.GetBoolParam("Tamanho");
            tipoEPI.DiasRevisao = Request.GetBoolParam("DiasRevisao");
            tipoEPI.DiasValidade = Request.GetBoolParam("DiasValidade");
            tipoEPI.Descartavel = Request.GetBoolParam("Descartavel");
            tipoEPI.InstrucaoUso = Request.GetBoolParam("InstrucaoUso");
            tipoEPI.Valor = Request.GetBoolParam("Valor");
            tipoEPI.NumeroCertificado = Request.GetBoolParam("NumeroCertificado");
            tipoEPI.Caracteristica = Request.GetBoolParam("Caracteristica");
        }

        private Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaTipoEPI ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaTipoEPI()
            {
                Descricao = Request.GetStringParam("Descricao"),
                Ativo = Request.GetEnumParam<SituacaoAtivoPesquisa>("Ativo")
            };
        }

        #endregion
    }
}
