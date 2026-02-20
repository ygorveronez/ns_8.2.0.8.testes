using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Contatos
{
    [CustomAuthorize("Contatos/PessoaContato")]
    public class PessoaContatoController : BaseController
    {
		#region Construtores

		public PessoaContatoController(Conexao conexao) : base(conexao) { }

		#endregion

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Contatos.PessoaContato repPessoaContato = new Repositorio.Embarcador.Contatos.PessoaContato(unitOfWork);

                // Manipula grids
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                // Cabecalhos grid
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Contato", "Contato", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("E-mail", "Email", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Telefone", "Telefone", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo de Contato", "DescricaoTipoContato", 20, Models.Grid.Align.left, false);

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                // Dados do filtro
                string nome = Request.Params("Nome");

                int.TryParse(Request.Params("GrupoPessoas"), out int codigoGrupoPessoas);

                double.TryParse(Request.Params("Pessoa"), out double cpfCnpjPessoa);

                bool.TryParse(Request.Params("ObrigatorioPessoaOuGrupo"), out bool obrigatorioPessoaOuGrupo);

                // Consulta
                List<Dominio.Entidades.Embarcador.Contatos.PessoaContato> listaGrid = repPessoaContato.Consultar(obrigatorioPessoaOuGrupo, cpfCnpjPessoa, codigoGrupoPessoas, nome, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repPessoaContato.ContarConsulta(obrigatorioPessoaOuGrupo, cpfCnpjPessoa, codigoGrupoPessoas, nome);

                var lista = from obj in listaGrid
                            select new
                            {
                                obj.Codigo,
                                obj.Contato,
                                obj.Email,
                                obj.Telefone,
                                obj.DescricaoTipoContato
                            };

                // Seta valores na grid e rotarna conteudo
                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(lista.ToList());

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

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("Codigo"), out int codigo);
                
                Repositorio.Embarcador.Contatos.PessoaContato repPessoaContato = new Repositorio.Embarcador.Contatos.PessoaContato(unitOfWork);

                Dominio.Entidades.Embarcador.Contatos.PessoaContato pessoaContato = repPessoaContato.BuscarPorCodigo(codigo, false);

                if (pessoaContato == null)
                    return new JsonpResult(false, true, "Contato não encontrado.");

                var retorno = new
                {
                    pessoaContato.Codigo,
                    pessoaContato.Ativo,
                    pessoaContato.Contato,
                    pessoaContato.Email,
                    pessoaContato.Telefone,
                    TipoContato = pessoaContato.TiposContato.Select(o => o.Codigo).ToList()
                };

                return new JsonpResult(retorno);
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
    }
}
