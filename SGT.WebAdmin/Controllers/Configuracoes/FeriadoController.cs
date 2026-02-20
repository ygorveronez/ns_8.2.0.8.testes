using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Configuracoes
{
    [CustomAuthorize("Configuracoes/Feriado")]
    public class FeriadoController : BaseController
    {
		#region Construtores

		public FeriadoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string descricao = Request.Params("Descricao");
                string estado = Request.Params("Estado");
                string codigoIntegracao = Request.GetStringParam("CodigoIntegracao");

                if (estado == "0")
                    estado = string.Empty;

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo = !string.IsNullOrEmpty(Request.Params("Ativo")) ? (Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa)int.Parse(Request.Params("Ativo")) : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFeriado? tipoFeriado = null;
                if (Enum.TryParse(Request.Params("Tipo"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFeriado tipoFeriadoAux))
                    tipoFeriado = tipoFeriadoAux;

                int.TryParse(Request.Params("Localidade"), out int codigoLocalidade);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 12, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipo", 12, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Localidade/Estado", "LocalidadeEstado", 25, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Situação", "DescricaoAtivo", 12, Models.Grid.Align.left, false);

                Repositorio.Embarcador.Configuracoes.Feriado repFeriado = new Repositorio.Embarcador.Configuracoes.Feriado(unitOfWork);

                List<Dominio.Entidades.Embarcador.Configuracoes.Feriado> listaFeriado = repFeriado.Consultar(codigoIntegracao, descricao, tipoFeriado, ativo, estado, codigoLocalidade, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);

                grid.setarQuantidadeTotal(repFeriado.ContarConsulta(codigoIntegracao, descricao, tipoFeriado, ativo, estado, codigoLocalidade));

                grid.AdicionaRows((from p in listaFeriado
                                   select new
                                   {
                                       p.Codigo,
                                       p.Descricao,
                                       Data = p.DescricaoData,
                                       p.DescricaoAtivo,
                                       DescricaoTipo = p.Tipo.ObterDescricao(),
                                       LocalidadeEstado = p.Tipo == TipoFeriado.Municipal ? p.Localidade?.DescricaoCidadeEstado : p.Tipo == TipoFeriado.Estadual ? p.Estado?.Nome : string.Empty,

                                   }).ToList());

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
                bool.TryParse(Request.Params("Ativo"), out bool ativo);

                string descricao = Request.Params("Descricao");
                string estado = Request.Params("Estado");

                int.TryParse(Request.Params("Dia"), out int dia);
                int.TryParse(Request.Params("Mes"), out int mes);
                int.TryParse(Request.Params("Ano"), out int ano);
                int.TryParse(Request.Params("Localidade"), out int codigoLocalidade);

                Enum.TryParse(Request.Params("Tipo"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFeriado tipo);

                Repositorio.Embarcador.Configuracoes.Feriado repFeriado = new Repositorio.Embarcador.Configuracoes.Feriado(unitOfWork);
                Repositorio.Estado repEstado = new Repositorio.Estado(unitOfWork);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.Feriado feriado = new Dominio.Entidades.Embarcador.Configuracoes.Feriado();

                feriado.Descricao = descricao;
                feriado.Ativo = ativo;

                if (ano > 0)
                    feriado.Ano = ano;
                else
                    feriado.Ano = null;

                feriado.Dia = dia;
                feriado.Mes = mes;
                feriado.Tipo = tipo;
                feriado.CodigoIntegracao = Request.GetStringParam("CodigoIntegracao");
                feriado.Localidade = null;
                feriado.Estado = null;

                if (feriado.Tipo == TipoFeriado.Municipal)
                    feriado.Localidade = repLocalidade.BuscarPorCodigo(codigoLocalidade);
                else if (feriado.Tipo == TipoFeriado.Estadual)
                    feriado.Estado = repEstado.BuscarPorSigla(estado);

                repFeriado.Inserir(feriado, Auditado);

                return new JsonpResult(true);
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

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("Codigo"), out int codigo);

                bool.TryParse(Request.Params("Ativo"), out bool ativo);

                string descricao = Request.Params("Descricao");
                string estado = Request.Params("Estado");

                int.TryParse(Request.Params("Dia"), out int dia);
                int.TryParse(Request.Params("Mes"), out int mes);
                int.TryParse(Request.Params("Ano"), out int ano);
                int.TryParse(Request.Params("Localidade"), out int codigoLocalidade);

                Enum.TryParse(Request.Params("Tipo"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFeriado tipo);

                Repositorio.Embarcador.Configuracoes.Feriado repFeriado = new Repositorio.Embarcador.Configuracoes.Feriado(unitOfWork);
                Repositorio.Estado repEstado = new Repositorio.Estado(unitOfWork);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.Feriado feriado = repFeriado.BuscarPorCodigo(codigo, true);

                feriado.Descricao = descricao;
                feriado.Ativo = ativo;

                if (ano > 0)
                    feriado.Ano = ano;
                else
                    feriado.Ano = null;

                feriado.Dia = dia;
                feriado.Mes = mes;
                feriado.Tipo = tipo;
                feriado.CodigoIntegracao = Request.GetStringParam("CodigoIntegracao");
                feriado.Localidade = null;
                feriado.Estado = null;

                if (feriado.Tipo == TipoFeriado.Municipal)
                    feriado.Localidade = repLocalidade.BuscarPorCodigo(codigoLocalidade);
                else if (feriado.Tipo == TipoFeriado.Estadual)
                    feriado.Estado = repEstado.BuscarPorSigla(estado);

                repFeriado.Atualizar(feriado, Auditado);

                return new JsonpResult(true);

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

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Configuracoes.Feriado repFeriado = new Repositorio.Embarcador.Configuracoes.Feriado(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.Feriado feriado = repFeriado.BuscarPorCodigo(codigo, false);

                var retorno = new
                {
                    feriado.Codigo,
                    feriado.Ativo,
                    feriado.Descricao,
                    Ano = feriado.Ano?.ToString("D4") ?? string.Empty,
                    Dia = feriado.Dia.ToString("D2"),
                    Mes = feriado.Mes.ToString("D2"),
                    Estado = new
                    {
                        Codigo = feriado.Estado?.Sigla ?? "0",
                        Descricao = feriado.Estado?.Nome
                    },
                    Localidade = new
                    {
                        Codigo = feriado.Localidade?.Codigo,
                        Descricao = feriado.Localidade?.DescricaoCidadeEstado
                    },
                    feriado.Tipo,
                    feriado.CodigoIntegracao
                };

                return new JsonpResult(retorno);
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

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("Codigo"), out int codigo);

                Repositorio.Embarcador.Configuracoes.Feriado repFeriado = new Repositorio.Embarcador.Configuracoes.Feriado(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.Feriado feriado = repFeriado.BuscarPorCodigo(codigo, true);

                repFeriado.Deletar(feriado, Auditado);

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
