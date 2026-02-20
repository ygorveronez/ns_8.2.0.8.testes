using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pessoas
{
    [CustomAuthorize("Pessoas/ContatoGrupoPessoa")]
    public class ContatoGrupoPessoaController : BaseController
    {
		#region Construtores

		public ContatoGrupoPessoaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaContatoGrupoPessoa filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 60, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Grupo de Pessoa", "GrupoPessoa", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("AssuntoEmailChamado", false);
                grid.AdicionarCabecalho("CorpoEmailChamado", false);
                grid.AdicionarCabecalho("MensagemPadraoOrientacaoMotorista", false);

                Repositorio.Embarcador.Pessoas.ContatoGrupoPessoa repContatoGrupoPessoa = new Repositorio.Embarcador.Pessoas.ContatoGrupoPessoa(unitOfWork);
                List<Dominio.Entidades.Embarcador.Pessoas.ContatoGrupoPessoa> contatoGrupoPessoas = repContatoGrupoPessoa.Consultar(filtrosPesquisa, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repContatoGrupoPessoa.ContarConsulta(filtrosPesquisa));

                var lista = (from p in contatoGrupoPessoas
                             select new
                             {
                                 p.Codigo,
                                 p.Descricao,
                                 GrupoPessoa = p.GrupoPessoa.Descricao,
                                 p.GrupoPessoa.AssuntoEmailChamado,
                                 p.GrupoPessoa.CorpoEmailChamado,
                                 p.GrupoPessoa.MensagemPadraoOrientacaoMotorista
                             }).ToList();

                grid.AdicionaRows(lista);
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
                unitOfWork.Start();

                Repositorio.Embarcador.Pessoas.ContatoGrupoPessoa repContatoGrupoPessoa = new Repositorio.Embarcador.Pessoas.ContatoGrupoPessoa(unitOfWork);
                Dominio.Entidades.Embarcador.Pessoas.ContatoGrupoPessoa contatoGrupoPessoa = new Dominio.Entidades.Embarcador.Pessoas.ContatoGrupoPessoa();

                PreencherContatoGrupoPessoa(contatoGrupoPessoa, unitOfWork);

                repContatoGrupoPessoa.Inserir(contatoGrupoPessoa, Auditado);

                SalvarContatos(contatoGrupoPessoa, unitOfWork);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
        }

        public async Task<IActionResult> Atualizar()
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Pessoas.ContatoGrupoPessoa repContatoGrupoPessoa = new Repositorio.Embarcador.Pessoas.ContatoGrupoPessoa(unitOfWork);
                Dominio.Entidades.Embarcador.Pessoas.ContatoGrupoPessoa contatoGrupoPessoa = repContatoGrupoPessoa.BuscarPorCodigo(codigo, true);

                PreencherContatoGrupoPessoa(contatoGrupoPessoa, unitOfWork);

                repContatoGrupoPessoa.Atualizar(contatoGrupoPessoa, Auditado);

                SalvarContatos(contatoGrupoPessoa, unitOfWork);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Pessoas.ContatoGrupoPessoa repContatoGrupoPessoa = new Repositorio.Embarcador.Pessoas.ContatoGrupoPessoa(unitOfWork);
                Dominio.Entidades.Embarcador.Pessoas.ContatoGrupoPessoa contatoGrupoPessoa = repContatoGrupoPessoa.BuscarPorCodigo(codigo, false);

                var dynContatoGrupoPessoa = new
                {
                    contatoGrupoPessoa.Codigo,
                    contatoGrupoPessoa.Descricao,
                    GrupoPessoa = contatoGrupoPessoa.GrupoPessoa != null ? new { contatoGrupoPessoa.GrupoPessoa.Codigo, contatoGrupoPessoa.GrupoPessoa.Descricao } : null,
                    Contatos = (from obj in contatoGrupoPessoa.Contatos
                                select new
                                {
                                    obj.Codigo,
                                    obj.Nome,
                                    obj.Email,
                                    obj.Celular,
                                    obj.Telefone
                                }).ToList()
                };

                return new JsonpResult(dynContatoGrupoPessoa);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Pessoas.ContatoGrupoPessoa repContatoGrupoPessoa = new Repositorio.Embarcador.Pessoas.ContatoGrupoPessoa(unitOfWork);
                Dominio.Entidades.Embarcador.Pessoas.ContatoGrupoPessoa contatoGrupoPessoa = repContatoGrupoPessoa.BuscarPorCodigo(codigo, true);

                if (contatoGrupoPessoa == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                repContatoGrupoPessoa.Deletar(contatoGrupoPessoa, Auditado);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, false, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseja mais utilizá-lo.");
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

        #region Métodos Privados

        private void PreencherContatoGrupoPessoa(Dominio.Entidades.Embarcador.Pessoas.ContatoGrupoPessoa contatoGrupoPessoa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoa = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);

            int grupoPessoa = Request.GetIntParam("GrupoPessoa");

            contatoGrupoPessoa.Descricao = Request.GetStringParam("Descricao");
            contatoGrupoPessoa.GrupoPessoa = grupoPessoa > 0 ? repGrupoPessoa.BuscarPorCodigo(grupoPessoa) : null;
        }

        private Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaContatoGrupoPessoa ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaContatoGrupoPessoa()
            {
                Descricao = Request.GetStringParam("Descricao"),
                CodigoGrupoPessoa = Request.GetIntParam("GrupoPessoa")
            };
        }

        private void SalvarContatos(Dominio.Entidades.Embarcador.Pessoas.ContatoGrupoPessoa contatoGrupoPessoa, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Pessoas.ContatoGrupoPessoaDado repContatoGrupoPessoaDado = new Repositorio.Embarcador.Pessoas.ContatoGrupoPessoaDado(unidadeTrabalho);

            dynamic contatos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Contatos"));
            if (contatoGrupoPessoa.Contatos != null && contatoGrupoPessoa.Contatos.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var contato in contatos)
                    if (contato.Codigo != null)
                        codigos.Add((int)contato.Codigo);

                List<Dominio.Entidades.Embarcador.Pessoas.ContatoGrupoPessoaDado> contatoDeletar = (from obj in contatoGrupoPessoa.Contatos where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < contatoDeletar.Count; i++)
                    repContatoGrupoPessoaDado.Deletar(contatoDeletar[i]);
            }
            else
                contatoGrupoPessoa.Contatos = new List<Dominio.Entidades.Embarcador.Pessoas.ContatoGrupoPessoaDado>();

            foreach (var contato in contatos)
            {
                Dominio.Entidades.Embarcador.Pessoas.ContatoGrupoPessoaDado contatoGrupoPessoaDado = contato.Codigo != null ? repContatoGrupoPessoaDado.BuscarPorCodigo((int)contato.Codigo, false) : null;
                if (contatoGrupoPessoaDado == null)
                    contatoGrupoPessoaDado = new Dominio.Entidades.Embarcador.Pessoas.ContatoGrupoPessoaDado();

                contatoGrupoPessoaDado.ContatoGrupoPessoa = contatoGrupoPessoa;
                contatoGrupoPessoaDado.Nome = (string)contato.Nome;
                contatoGrupoPessoaDado.Email = (string)contato.Email;
                contatoGrupoPessoaDado.Telefone = Utilidades.String.OnlyNumbers((string)contato.Telefone);
                contatoGrupoPessoaDado.Celular = Utilidades.String.OnlyNumbers((string)contato.Celular);

                if (contatoGrupoPessoaDado.Codigo > 0)
                    repContatoGrupoPessoaDado.Atualizar(contatoGrupoPessoaDado);
                else
                    repContatoGrupoPessoaDado.Inserir(contatoGrupoPessoaDado);
            }
        }

        #endregion
    }
}
