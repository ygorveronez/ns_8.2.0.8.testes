using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Seguros
{
    [CustomAuthorize("Seguros/Seguradora")]
    public class SeguradoraController : BaseController
    {
		#region Construtores

		public SeguradoraController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string nome = Request.Params("Nome");

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa? ativo = null;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativoAux;
                if (Enum.TryParse(Request.Params("Ativo"), out ativoAux))
                    ativo = ativoAux;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Nome, "Nome", 75, Models.Grid.Align.left, true);

                if (ativo.HasValue && ativo.Value == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "DescricaoAtivo", 10, Models.Grid.Align.center, false);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;



                Repositorio.Embarcador.Seguros.Seguradora repSeguradora = new Repositorio.Embarcador.Seguros.Seguradora(unidadeDeTrabalho);

                List<Dominio.Entidades.Embarcador.Seguros.Seguradora> listaSeguradoras = repSeguradora.Consultar(nome, ativo, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);

                grid.setarQuantidadeTotal(repSeguradora.ContarConsulta(nome, ativo));

                var retorno = (from obj in listaSeguradoras
                               select new
                               {
                                   obj.Codigo,
                                   obj.Nome,
                                   obj.DescricaoAtivo
                               }).ToList();

                grid.AdicionaRows(retorno);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string nome = Request.Params("Nome"),
                       observacao = Request.Params("Observacao");

                bool ativo;
                bool.TryParse(Request.Params("Ativo"), out ativo);

                double clienteSeguradora = 0;
                double.TryParse(Request.Params("ClienteSeguradora"), out clienteSeguradora);


                Repositorio.Embarcador.Seguros.Seguradora repSeguradora = new Repositorio.Embarcador.Seguros.Seguradora(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Seguros.Seguradora seguradora = new Dominio.Entidades.Embarcador.Seguros.Seguradora();

                seguradora.ClienteSeguradora = new Dominio.Entidades.Cliente() { CPF_CNPJ = clienteSeguradora };
                seguradora.Ativo = ativo;
                seguradora.Nome = nome;
                seguradora.Observacao = observacao;

                repSeguradora.Inserir(seguradora, Auditado);

                var retorno = new
                {
                    seguradora.Codigo,
                    seguradora.Nome
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                bool ativo;
                bool.TryParse(Request.Params("Ativo"), out ativo);

                string nome = Request.Params("Nome"),
                       observacao = Request.Params("Observacao");

                double clienteSeguradora = 0;
                double.TryParse(Request.Params("ClienteSeguradora"), out clienteSeguradora);

                Repositorio.Embarcador.Seguros.Seguradora repSeguradora = new Repositorio.Embarcador.Seguros.Seguradora(unidadeDeTrabalho);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Seguros.Seguradora seguradora = repSeguradora.BuscarPorCodigo(codigo, true);

                seguradora.ClienteSeguradora = repCliente.BuscarPorCPFCNPJ(clienteSeguradora);

                if (seguradora == null)
                    return new JsonpResult(false, "Seguradora não encontrada.");

                seguradora.Ativo = ativo;
                seguradora.Nome = nome;
                seguradora.Observacao = observacao;

                repSeguradora.Atualizar(seguradora, Auditado);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Seguros.Seguradora repSeguradora = new Repositorio.Embarcador.Seguros.Seguradora(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Seguros.Seguradora seguradora = repSeguradora.BuscarPorCodigo(codigo);

                var retorno = new
                {
                    ClienteSeguradora = seguradora.ClienteSeguradora != null ? new { Codigo = seguradora.ClienteSeguradora.CPF_CNPJ, Descricao = seguradora.ClienteSeguradora.Descricao } : new { Codigo = (double)0, Descricao = "" },
                    seguradora.Ativo,
                    seguradora.Codigo,
                    seguradora.Nome,
                    seguradora.Observacao
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
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Seguros.Seguradora repSeguradora = new Repositorio.Embarcador.Seguros.Seguradora(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Seguros.Seguradora seguradora = repSeguradora.BuscarPorCodigo(codigo);

                repSeguradora.Deletar(seguradora, Auditado);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Dispose();

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
                unidadeDeTrabalho.Dispose();
            }
        }

        #endregion
    }
}
