using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pedidos
{
    [CustomAuthorize("Pedidos/ClassificacaoRiscoONU")]
    public class ClassificacaoRiscoONUController : BaseController
    {
		#region Construtores

		public ClassificacaoRiscoONUController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);

                string descricao = Request.Params("Descricao");
                string numeroONU = Request.Params("NumeroONU");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo = !string.IsNullOrEmpty(Request.Params("Ativo")) ? (Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa)int.Parse(Request.Params("Ativo")) : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;


                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 50, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("ONU", "NumeroONU", 20, Models.Grid.Align.left, true);
                if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Situação", "DescricaoAtivo", 10, Models.Grid.Align.center, false);                
                grid.AdicionarCabecalho("ClasseRisco", false);
                grid.AdicionarCabecalho("RiscoSubsidiario", false);
                grid.AdicionarCabecalho("NumeroRisco", false);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.Embarcador.Pedidos.ClassificacaoRiscoONU repClassificacaoRiscoONU = new Repositorio.Embarcador.Pedidos.ClassificacaoRiscoONU(unidadeDeTrabalho);

                List<Dominio.Entidades.Embarcador.Pedidos.ClassificacaoRiscoONU> listaClassificacaoRiscoONU = repClassificacaoRiscoONU.Consultar(numeroONU, descricao, ativo, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repClassificacaoRiscoONU.ContarConsulta(numeroONU, descricao, ativo));

                var retorno = (from obj in listaClassificacaoRiscoONU
                               select new
                               {
                                   Codigo = obj.Codigo,
                                   Descricao = obj.Descricao,
                                   obj.NumeroONU,
                                   DescricaoAtivo = obj.DescricaoAtivo,                                   
                                   obj.ClasseRisco,
                                   obj.RiscoSubsidiario,
                                   obj.NumeroRisco
                               }).ToList();
                grid.AdicionaRows(retorno);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaONUS()
        {
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(_conexao.AdminStringConexao);

            try
            {
                string numeroONU = Utilidades.String.OnlyNumbers(Request.Params("Descricao"));
                string descricao = Request.Params("NumeroONU");


                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("ONU", "Descricao", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Descrição", "NumeroONU", 60, Models.Grid.Align.left, true);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                AdminMultisoftware.Repositorio.Produtos.ONU repONU = new AdminMultisoftware.Repositorio.Produtos.ONU(unitOfWork);

                List<AdminMultisoftware.Dominio.Entidades.Produtos.ONU> listaONU = repONU.ConsultarONU(numeroONU, descricao, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repONU.ContarConsultaONU(numeroONU, descricao));

                var lista = (from p in listaONU select new { p.Codigo, NumeroONU = p.Descricao, Descricao = p.Numero }).ToList();

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
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unidadeDeTrabalho.Start();
                Repositorio.Embarcador.Pedidos.ClassificacaoRiscoONU repClassificacaoRiscoONU = new Repositorio.Embarcador.Pedidos.ClassificacaoRiscoONU(unidadeDeTrabalho);


                Dominio.Entidades.Embarcador.Pedidos.ClassificacaoRiscoONU classificacaoRiscoONU = new Dominio.Entidades.Embarcador.Pedidos.ClassificacaoRiscoONU();

                classificacaoRiscoONU.Ativo = bool.Parse(Request.Params("Ativo"));
                classificacaoRiscoONU.Descricao = Request.Params("Descricao");
                classificacaoRiscoONU.ClasseRisco = Request.Params("ClasseRisco");
                classificacaoRiscoONU.GrupoEmbarcado = Request.Params("GrupoEmbarcado");
                classificacaoRiscoONU.NumeroONU = Request.Params("NumeroONU");
                classificacaoRiscoONU.NumeroRisco = Request.Params("NumeroRisco");
                classificacaoRiscoONU.RiscoSubsidiario = Request.Params("RiscoSubsidiario");
                classificacaoRiscoONU.ProvisoesEspeciais = Request.Params("ProvisoesEspeciais");                
                classificacaoRiscoONU.EmbalagemInstrucao = Request.Params("EmbalagemInstrucao");
                classificacaoRiscoONU.EmbalagemProvisoesEspeciais = Request.Params("EmbalagemProvisoesEspeciais");
                classificacaoRiscoONU.TanqueInstrucao = Request.Params("TanqueInstrucao");
                classificacaoRiscoONU.TanqueProvisoesEspeciais = Request.Params("TanqueProvisoesEspeciais");

                decimal limiteKGVeiculo = 0, limiteLitroEmbalagem = 0;
                decimal.TryParse(Request.Params("LimiteKGVeiculo"), out limiteKGVeiculo);
                decimal.TryParse(Request.Params("LimiteLitroEmbalagemInterna"), out limiteLitroEmbalagem);
                classificacaoRiscoONU.LimiteKGVeiculo = limiteKGVeiculo;
                classificacaoRiscoONU.LimiteLitroEmbalagemInterna = limiteLitroEmbalagem;

                repClassificacaoRiscoONU.Inserir(classificacaoRiscoONU, Auditado);
                unidadeDeTrabalho.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();
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
                unidadeDeTrabalho.Start();
                Repositorio.Embarcador.Pedidos.ClassificacaoRiscoONU repClassificacaoRiscoONU = new Repositorio.Embarcador.Pedidos.ClassificacaoRiscoONU(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Pedidos.ClassificacaoRiscoONU classificacaoRiscoONU = repClassificacaoRiscoONU.BuscarPorCodigo(int.Parse(Request.Params("Codigo")), true);

                classificacaoRiscoONU.Ativo = bool.Parse(Request.Params("Ativo"));
                classificacaoRiscoONU.Descricao = Request.Params("Descricao");
                classificacaoRiscoONU.ClasseRisco = Request.Params("ClasseRisco");
                classificacaoRiscoONU.GrupoEmbarcado = Request.Params("GrupoEmbarcado");
                classificacaoRiscoONU.NumeroONU = Request.Params("NumeroONU");
                classificacaoRiscoONU.NumeroRisco = Request.Params("NumeroRisco");
                classificacaoRiscoONU.RiscoSubsidiario = Request.Params("RiscoSubsidiario");
                classificacaoRiscoONU.ProvisoesEspeciais = Request.Params("ProvisoesEspeciais");
                classificacaoRiscoONU.EmbalagemInstrucao = Request.Params("EmbalagemInstrucao");
                classificacaoRiscoONU.EmbalagemProvisoesEspeciais = Request.Params("EmbalagemProvisoesEspeciais");
                classificacaoRiscoONU.TanqueInstrucao = Request.Params("TanqueInstrucao");
                classificacaoRiscoONU.TanqueProvisoesEspeciais = Request.Params("TanqueProvisoesEspeciais");

                decimal limiteKGVeiculo = 0, limiteLitroEmbalagem = 0;
                decimal.TryParse(Request.Params("LimiteKGVeiculo"), out limiteKGVeiculo);
                decimal.TryParse(Request.Params("LimiteLitroEmbalagemInterna"), out limiteLitroEmbalagem);
                classificacaoRiscoONU.LimiteKGVeiculo = limiteKGVeiculo;
                classificacaoRiscoONU.LimiteLitroEmbalagemInterna = limiteLitroEmbalagem;

                repClassificacaoRiscoONU.Atualizar(classificacaoRiscoONU, Auditado);
                unidadeDeTrabalho.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();
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
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Pedidos.ClassificacaoRiscoONU repClassificacaoRiscoONU = new Repositorio.Embarcador.Pedidos.ClassificacaoRiscoONU(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Pedidos.ClassificacaoRiscoONU classificacaoRiscoONU = repClassificacaoRiscoONU.BuscarPorCodigo(codigo);
                var retorno = new
                {
                    classificacaoRiscoONU.Ativo,
                    classificacaoRiscoONU.Codigo,
                    classificacaoRiscoONU.Descricao,
                    classificacaoRiscoONU.ClasseRisco,
                    classificacaoRiscoONU.GrupoEmbarcado,
                    classificacaoRiscoONU.NumeroRisco,
                    classificacaoRiscoONU.RiscoSubsidiario,
                    NumeroONU = !string.IsNullOrWhiteSpace(classificacaoRiscoONU.NumeroONU) ? new { Codigo = classificacaoRiscoONU.NumeroONU, Descricao = classificacaoRiscoONU.NumeroONU } : null,
                    classificacaoRiscoONU.ProvisoesEspeciais,
                    LimiteKGVeiculo = classificacaoRiscoONU.LimiteKGVeiculo.ToString("n3"),
                    LimiteLitroEmbalagemInterna = classificacaoRiscoONU.LimiteLitroEmbalagemInterna.ToString("n3"),
                    classificacaoRiscoONU.EmbalagemInstrucao,
                    classificacaoRiscoONU.EmbalagemProvisoesEspeciais,
                    classificacaoRiscoONU.TanqueInstrucao,
                    classificacaoRiscoONU.TanqueProvisoesEspeciais
                };

                unidadeDeTrabalho.Dispose();

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                unidadeDeTrabalho.Dispose();

                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Pedidos.ClassificacaoRiscoONU repClassificacaoRiscoONU = new Repositorio.Embarcador.Pedidos.ClassificacaoRiscoONU(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Pedidos.ClassificacaoRiscoONU classificacaoRiscoONU = repClassificacaoRiscoONU.BuscarPorCodigo(codigo);

                repClassificacaoRiscoONU.Deletar(classificacaoRiscoONU, Auditado);

                unidadeDeTrabalho.Dispose();

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
        }

        #endregion
    }
}
