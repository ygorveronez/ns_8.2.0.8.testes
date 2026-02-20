using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Veiculos
{
    [CustomAuthorize("Veiculos/SegmentoVeiculo")]
    public class SegmentoVeiculoController : BaseController
    {
		#region Construtores

		public SegmentoVeiculoController(Conexao conexao) : base(conexao) { }

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
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 70, Models.Grid.Align.left, true);

                if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "DescricaoAtivo", 10, Models.Grid.Align.center, false);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.Embarcador.Veiculos.SegmentoVeiculo repSegmentoVeiculo = new Repositorio.Embarcador.Veiculos.SegmentoVeiculo(unidadeTrabalho);

                List<Dominio.Entidades.Embarcador.Veiculos.SegmentoVeiculo> listaSegmentoVeiculo = repSegmentoVeiculo.Consultar(descricao, situacao, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repSegmentoVeiculo.ContarConsulta(descricao, situacao));

                grid.AdicionaRows((from p in listaSegmentoVeiculo select new { p.Codigo, p.Descricao, p.DescricaoAtivo }).ToList());

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





        [AllowAuthenticate]
        public async Task<IActionResult> ObterTodos()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                bool.TryParse(Request.Params("OpcaoSemGrupo"), out bool opcaoSemGrupo);

                Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioVeiculo filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioVeiculo()
                {
                    Ativo = !string.IsNullOrEmpty(Request.Params("Ativo")) ? (Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa)int.Parse(Request.Params("Ativo")) : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo,
                    Tipo = "P"
                };

                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeDeTrabalho);

                List<Dominio.Entidades.Veiculo> veiculos = repVeiculo.ConsultarVeiculos(filtrosPesquisa, "Placa", "asc", 0, 3000);

                var retorno = (from obj in veiculos select new { value = obj.Codigo, text = obj.Placa }).ToList();

                if (opcaoSemGrupo)
                    retorno.Insert(0, new { value = -1, text = "Sem Veículos" });

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                bool ativo;
                bool.TryParse(Request.Params("Ativo"), out ativo);

                string descricao = Request.Params("Descricao");

                decimal valorMinimo;
                decimal.TryParse(Request.Params("ValorMinimo"), out valorMinimo);

                decimal MetaMensal;
                decimal.TryParse(Request.Params("MetaMensal"), out MetaMensal);

                Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unidadeTrabalho);
                Repositorio.Embarcador.Veiculos.SegmentoVeiculo repSegmentoVeiculo = new Repositorio.Embarcador.Veiculos.SegmentoVeiculo(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Veiculos.SegmentoVeiculo segmentoVeiculo = new Dominio.Entidades.Embarcador.Veiculos.SegmentoVeiculo();

                segmentoVeiculo.Ativo = ativo;
                segmentoVeiculo.Descricao = descricao;
                segmentoVeiculo.ValorMinimo = valorMinimo;
                segmentoVeiculo.MetaMensal = MetaMensal;

                repSegmentoVeiculo.Inserir(segmentoVeiculo, Auditado);

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
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                bool ativo;
                bool.TryParse(Request.Params("Ativo"), out ativo);

                decimal valorMinimo;
                decimal.TryParse(Request.Params("ValorMinimo"), out valorMinimo);

                decimal MetaMensal;
                decimal.TryParse(Request.Params("MetaMensal"), out MetaMensal);

                string descricao = Request.Params("Descricao");

                Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unidadeTrabalho);
                Repositorio.Embarcador.Veiculos.SegmentoVeiculo repSegmentoVeiculo = new Repositorio.Embarcador.Veiculos.SegmentoVeiculo(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Veiculos.SegmentoVeiculo segmentoVeiculo = repSegmentoVeiculo.BuscarPorCodigo(codigo, true);

                segmentoVeiculo.Ativo = ativo;
                segmentoVeiculo.Descricao = descricao;
                segmentoVeiculo.ValorMinimo = valorMinimo;
                segmentoVeiculo.MetaMensal = MetaMensal;

                repSegmentoVeiculo.Atualizar(segmentoVeiculo, Auditado);

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

                Repositorio.Embarcador.Veiculos.SegmentoVeiculo repSegmentoVeiculo = new Repositorio.Embarcador.Veiculos.SegmentoVeiculo(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Veiculos.SegmentoVeiculo segmentoVeiculo = repSegmentoVeiculo.BuscarPorCodigo(codigo);

                var retorno = new
                {
                    segmentoVeiculo.Ativo,
                    segmentoVeiculo.Codigo,
                    segmentoVeiculo.Descricao,
                    segmentoVeiculo.ValorMinimo,
                    segmentoVeiculo.MetaMensal
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

                Repositorio.Embarcador.Veiculos.SegmentoVeiculo repSegmentoVeiculo = new Repositorio.Embarcador.Veiculos.SegmentoVeiculo(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Veiculos.SegmentoVeiculo segmentoVeiculo = repSegmentoVeiculo.BuscarPorCodigo(codigo);

                repSegmentoVeiculo.Deletar(segmentoVeiculo, Auditado);

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
