using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;


namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/Fronteira")]
    public class FronteiraController : BaseController
    {
		#region Construtores

		public FronteiraController(Conexao conexao) : base(conexao) { }

		#endregion


        #region Métodos Globais
        [AllowAuthenticate]
        [Obsolete]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                string descricao = Request.Params("Descricao");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo = !string.IsNullOrEmpty(Request.Params("Ativo")) ? (Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa)int.Parse(Request.Params("Ativo")) : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;

                int localidade = int.Parse(Request.Params("Localidade"));

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 35, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Fronteira.Localidade, "Localidade", 25, Models.Grid.Align.left, true);
                if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "DescricaoAtivo", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Latitude", false);
                grid.AdicionarCabecalho("Longitude", false);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdena == "Localidade")
                    propOrdena += ".Descricao";

                Repositorio.Embarcador.Logistica.Fronteira repFronteira = new Repositorio.Embarcador.Logistica.Fronteira(unitOfWork);
                List<Dominio.Entidades.Embarcador.Logistica.Fronteira> listaFronteira = repFronteira.Consultar(descricao, localidade, ativo, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repFronteira.ContarConsulta(descricao, localidade, ativo));
                var lista = (from p in listaFronteira
                             select new
                             {
                                 p.Codigo,
                                 p.Descricao,
                                 p.DescricaoAtivo,
                                 Localidade = p.Localidade != null ? p.Localidade.DescricaoCidadeEstado : "",
                                 Latitude = p.Localidade?.Latitude != null ? p.Localidade.Latitude : Convert.ToDecimal(0),
                                 Longitude = p.Localidade?.Longitude != null ? p.Localidade.Longitude : Convert.ToDecimal(0)
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

        [Obsolete]
        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Logistica.Fronteira repFronteira = new Repositorio.Embarcador.Logistica.Fronteira(unitOfWork);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.Fronteira fronteira = new Dominio.Entidades.Embarcador.Logistica.Fronteira();
                fronteira.Ativo = bool.Parse(Request.Params("Ativo"));
                fronteira.Descricao = Request.Params("Descricao");

                fronteira.FronteiraOutroLado = repFronteira.BuscarPorCodigo(int.Parse(Request.Params("FronteiraOutroLado")));

                fronteira.Localidade = repLocalidade.BuscarPorCodigo(int.Parse(Request.Params("Localidade")));
                fronteira.CodigoAduanaDestino = Request.GetStringParam("CodigoAduanaDestino");

                if (fronteira.Localidade.CodigoIBGE != 9999999 || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    fronteira.CodigoFronteiraEmbarcador = Request.Params("CodigoFronteiraEmbarcador");
                    repFronteira.Inserir(fronteira, Auditado);
                    return new JsonpResult(true);
                }
                else
                {
                    return new JsonpResult(false, true, "Não é possível selecionar uma localidade de exterior como cidade da fronteira.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [Obsolete]
        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Logistica.Fronteira repFronteira = new Repositorio.Embarcador.Logistica.Fronteira(unitOfWork);

                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.Fronteira fronteira = repFronteira.BuscarPorCodigo(int.Parse(Request.Params("Codigo")), true);
                fronteira.Ativo = bool.Parse(Request.Params("Ativo"));
                fronteira.Descricao = Request.Params("Descricao");
                fronteira.FronteiraOutroLado = repFronteira.BuscarPorCodigo(int.Parse(Request.Params("FronteiraOutroLado")));
                fronteira.Localidade = repLocalidade.BuscarPorCodigo(int.Parse(Request.Params("Localidade")));
                fronteira.CodigoFronteiraEmbarcador = Request.Params("CodigoFronteiraEmbarcador");
                fronteira.CodigoAduanaDestino = Request.GetStringParam("CodigoAduanaDestino");

                if (fronteira.Localidade.CodigoIBGE != 9999999 || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    repFronteira.Atualizar(fronteira, Auditado);
                    return new JsonpResult(true);
                }
                else
                {
                    return new JsonpResult(false, true, "Não é possível selecionar uma localidade de exterior como cidade da fronteira.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [Obsolete]
        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.Logistica.Fronteira repFronteira = new Repositorio.Embarcador.Logistica.Fronteira(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.Fronteira fronteira = repFronteira.BuscarPorCodigo(codigo);
                var dynFronteira = new
                {
                    fronteira.Codigo,
                    fronteira.CodigoFronteiraEmbarcador,
                    fronteira.CodigoAduanaDestino,
                    fronteira.Descricao,
                    fronteira.Ativo,
                    FronteiraOutroLado = new { Codigo = fronteira.FronteiraOutroLado != null ? fronteira.FronteiraOutroLado.Codigo : 0, Descricao = fronteira.FronteiraOutroLado != null ? fronteira.FronteiraOutroLado.Descricao : "" },
                    Localidade = new { Codigo = fronteira.Localidade != null ? fronteira.Localidade.Codigo : 0, Descricao = fronteira.Localidade != null ? fronteira.Localidade.DescricaoCidadeEstado : "" }
                };
                return new JsonpResult(dynFronteira);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [Obsolete]
        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = int.Parse(Request.Params("codigo"));
                Repositorio.Embarcador.Logistica.Fronteira repFronteira = new Repositorio.Embarcador.Logistica.Fronteira(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.Fronteira fronteira = repFronteira.BuscarPorCodigo(codigo);
                repFronteira.Deletar(fronteira, Auditado);
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseja mais utilizá-lo.");
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
    }
}
