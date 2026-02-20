using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Veiculos
{
    [CustomAuthorize("Veiculos/ModeloCarroceria")]
    public class ModeloCarroceriaController : BaseController
    {
		#region Construtores

		public ModeloCarroceriaController(Conexao conexao) : base(conexao) { }

		#endregion

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string descricao = Request.Params("Descricao");

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa situacao = !string.IsNullOrEmpty(Request.Params("Ativo")) ? (Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa)int.Parse(Request.Params("Ativo")) : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("ObrigatorioInformarDataValidadeAdicionalCarroceria", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 50, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.ModeloCarroceria.AdicionalFrete, "PercentualAdicionalFrete", 20, Models.Grid.Align.left, true);

                if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Situação", "DescricaoAtivo", 10, Models.Grid.Align.center, false);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.Embarcador.Veiculos.ModeloCarroceria repModeloCarroceria = new Repositorio.Embarcador.Veiculos.ModeloCarroceria(unidadeTrabalho);

                List<Dominio.Entidades.Embarcador.Veiculos.ModeloCarroceria> listaModeloCarroceria = repModeloCarroceria.Consultar(descricao, situacao, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);

                grid.setarQuantidadeTotal(repModeloCarroceria.ContarConsulta(descricao, situacao));

                grid.AdicionaRows((
                    from p in listaModeloCarroceria
                    select new {
                        p.Codigo,
                        p.Descricao,
                        PercentualAdicionalFrete = p.PercentualAdicionalFrete.ToString("n2"),
                        p.DescricaoAtivo,
                        p.Prioridade,
                        p.ObrigatorioInformarDataValidadeAdicionalCarroceria,
                    }
                ).ToList());

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoComponenteFrete;
                int.TryParse(Request.Params("ComponenteFrete"), out codigoComponenteFrete);

                bool ativo;
                bool.TryParse(Request.Params("Ativo"), out ativo);

                decimal percentualAdicionalFrete;
                decimal.TryParse(Request.Params("PercentualAdicionalFrete"), out percentualAdicionalFrete);

                string descricao = Request.Params("Descricao");

                Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unidadeTrabalho);
                Repositorio.Embarcador.Veiculos.ModeloCarroceria repModeloCarroceria = new Repositorio.Embarcador.Veiculos.ModeloCarroceria(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Veiculos.ModeloCarroceria modeloCarroceria = new Dominio.Entidades.Embarcador.Veiculos.ModeloCarroceria();

                modeloCarroceria.Ativo = ativo;
                modeloCarroceria.Descricao = descricao;
                modeloCarroceria.PercentualAdicionalFrete = percentualAdicionalFrete;
                modeloCarroceria.Prioridade = Request.GetIntParam("Prioridade");
                modeloCarroceria.ObrigatorioInformarDataValidadeAdicionalCarroceria = Request.GetBoolParam("ObrigatorioInformarDataValidadeAdicionalCarroceria");
                modeloCarroceria.ComponenteFrete = codigoComponenteFrete > 0 ? repComponenteFrete.BuscarPorCodigo(codigoComponenteFrete) : null;
                modeloCarroceria.CodigoIntegracao = Request.Params("CodigoIntegracao");

				repModeloCarroceria.Inserir(modeloCarroceria, Auditado);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo, codigoComponenteFrete;
                int.TryParse(Request.Params("Codigo"), out codigo);
                int.TryParse(Request.Params("ComponenteFrete"), out codigoComponenteFrete);

                bool ativo;
                bool.TryParse(Request.Params("Ativo"), out ativo);

                decimal percentualAdicionalFrete;
                decimal.TryParse(Request.Params("PercentualAdicionalFrete"), out percentualAdicionalFrete);

                string descricao = Request.Params("Descricao");

                Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unidadeTrabalho);
                Repositorio.Embarcador.Veiculos.ModeloCarroceria repModeloCarroceria = new Repositorio.Embarcador.Veiculos.ModeloCarroceria(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Veiculos.ModeloCarroceria modeloCarroceria = repModeloCarroceria.BuscarPorCodigo(codigo, true);

                modeloCarroceria.Ativo = ativo;
                modeloCarroceria.Descricao = descricao;
                modeloCarroceria.PercentualAdicionalFrete = percentualAdicionalFrete;
                modeloCarroceria.Prioridade = Request.GetIntParam("Prioridade");
                modeloCarroceria.ObrigatorioInformarDataValidadeAdicionalCarroceria = Request.GetBoolParam("ObrigatorioInformarDataValidadeAdicionalCarroceria");
                modeloCarroceria.ComponenteFrete = codigoComponenteFrete > 0 ? repComponenteFrete.BuscarPorCodigo(codigoComponenteFrete) : null;
				modeloCarroceria.CodigoIntegracao = Request.Params("CodigoIntegracao");

				repModeloCarroceria.Atualizar(modeloCarroceria, Auditado);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Veiculos.ModeloCarroceria repModeloCarroceria = new Repositorio.Embarcador.Veiculos.ModeloCarroceria(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Veiculos.ModeloCarroceria modeloCarroceria = repModeloCarroceria.BuscarPorCodigo(codigo);

                var retorno = new
                {
                    modeloCarroceria.Ativo,
                    modeloCarroceria.Codigo,
                    modeloCarroceria.Descricao,
                    PercentualAdicionalFrete = modeloCarroceria.PercentualAdicionalFrete.ToString("n2"),
                    modeloCarroceria.Prioridade,
                    modeloCarroceria.CodigoIntegracao,
                    modeloCarroceria.ObrigatorioInformarDataValidadeAdicionalCarroceria,
                    ComponenteFrete = new
                    {
                        Codigo = modeloCarroceria.ComponenteFrete?.Codigo,
                        Descricao = modeloCarroceria.ComponenteFrete?.Descricao
                    }
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
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Veiculos.ModeloCarroceria repModeloCarroceria = new Repositorio.Embarcador.Veiculos.ModeloCarroceria(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Veiculos.ModeloCarroceria modeloCarroceria = repModeloCarroceria.BuscarPorCodigo(codigo);

                repModeloCarroceria.Deletar(modeloCarroceria, Auditado);

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
                unidadeTrabalho.Dispose();
            }
        }
    }
}
