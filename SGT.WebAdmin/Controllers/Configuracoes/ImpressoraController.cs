using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Configuracoes
{
    [CustomAuthorize("Configuracoes/Impressora")]
    public class ImpressoraController : BaseController
    {
		#region Construtores

		public ImpressoraController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string impressora = Request.Params("Impressora");                
                string documento = Request.Params("Documento");

                int.TryParse(Request.Params("Unidade"), out int unidade);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo = !string.IsNullOrEmpty(Request.Params("Ativo")) ? (Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa)int.Parse(Request.Params("Ativo")) : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Impressora", "NomeImpressora", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Unidade", "NumeroDaUnidade", 12, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Documento", "Documento", 12, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Status", "Status", 12, Models.Grid.Align.left, false);

                Repositorio.Impressora repImpressora = new Repositorio.Impressora(unitOfWork);
                List<Dominio.Entidades.Impressora> listaImpressora = repImpressora.Consultar(unidade, impressora, ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo ? "A" : "I", documento, grid.inicio, grid.limite);

                grid.setarQuantidadeTotal(repImpressora.ContarConsulta(unidade, impressora, ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo ? "A" : "I", documento));

                grid.AdicionaRows((from p in listaImpressora
                                   select new
                                   {
                                       p.Codigo,
                                       p.NomeImpressora,
                                       p.NumeroDaUnidade,
                                       Documento = p.Documento == "C" ? "CTe/MDFe" : p.Documento == "N" ? "NFe/Boleto" : "Todos",
                                       Status = p.Status == "A" ? "Ativo" : "Inativo"
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

                string nomeImpressora = Request.Params("Impressora");
                string documento = Request.Params("Documento");
                string codigoIntegracao = Request.Params("CodigoIntegracao");

                int.TryParse(Request.Params("Unidade"), out int unidade);
                Repositorio.Impressora repImpressora = new Repositorio.Impressora(unitOfWork);

                Dominio.Entidades.Impressora impressoraValidacao = repImpressora.BuscarPorUnidadeDocumento(unidade, codigoIntegracao, documento, "A");
                if (ativo && impressoraValidacao != null)
                    return new JsonpResult(false, "Já existe uma impressora Ativa para a unidade/documento.");

                Dominio.Entidades.Impressora impressora = new Dominio.Entidades.Impressora();

                impressora.NomeImpressora = nomeImpressora;
                impressora.NumeroDaUnidade = unidade;
                impressora.Documento = documento;
                impressora.Status = ativo ? "A" : "I";
                impressora.CodigoIntegracao = codigoIntegracao;

                repImpressora.Inserir(impressora, Auditado);

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

                string nomeImpressora = Request.Params("Impressora");
                string documento = Request.Params("Documento");
                string codigoIntegracao = Request.Params("CodigoIntegracao");

                int.TryParse(Request.Params("Unidade"), out int unidade);
                Repositorio.Impressora repImpressora = new Repositorio.Impressora(unitOfWork);

                Dominio.Entidades.Impressora impressoraValidacao = repImpressora.BuscarPorUnidadeDocumento(unidade, codigoIntegracao, documento, "A");
                if (ativo && codigo > 0 && impressoraValidacao != null && impressoraValidacao.Codigo != codigo)
                    return new JsonpResult(false, "Já existe uma impressora Ativa para a unidade/documento.");

                Dominio.Entidades.Impressora impressora = repImpressora.BuscarPorCodigo(codigo, true);

                if (impressora == null)
                    return new JsonpResult(false, "Impressora não localizada.");

                impressora.NomeImpressora = nomeImpressora;
                impressora.NumeroDaUnidade = unidade;
                impressora.Documento = documento;
                impressora.Status = ativo ? "A" : "I";
                impressora.CodigoIntegracao = codigoIntegracao;

                repImpressora.Atualizar(impressora, Auditado);

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

                Repositorio.Impressora repImpressora = new Repositorio.Impressora(unitOfWork);
                Dominio.Entidades.Impressora impressora = repImpressora.BuscarPorCodigo(codigo);

                if (impressora == null)
                    return new JsonpResult(false, "Impressora não localizada.");

                var retorno = new
                {
                    impressora.Codigo,
                    Ativo = impressora.Status == "A",
                    Impressora = impressora.NomeImpressora,
                    Unidade = impressora.NumeroDaUnidade.ToString(),
                    impressora.Documento,
                    impressora.CodigoIntegracao
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

                Repositorio.Impressora repImpressora = new Repositorio.Impressora(unitOfWork);
                Dominio.Entidades.Impressora impressora = repImpressora.BuscarPorCodigo(codigo);

                repImpressora.Deletar(impressora, Auditado);

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
