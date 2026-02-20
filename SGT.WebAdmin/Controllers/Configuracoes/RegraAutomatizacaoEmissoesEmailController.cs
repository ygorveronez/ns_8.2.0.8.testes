using Dominio.Entidades.Embarcador.Pedidos;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Configuracoes
{
    [CustomAuthorize("Configuracoes/RegraAutomatizacaoEmissoesEmail")]
    public class RegraAutomatizacaoEmissoesEmailController : BaseController
    {
		#region Construtores

		public RegraAutomatizacaoEmissoesEmailController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Configuracoes.FiltroPesquisaRegraAutomatizacaoEmissoesEmail filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo Operação", "TipoOperacao", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("E-mail Destino", "EmailDestino", 20, Models.Grid.Align.left, true);

                if (filtrosPesquisa.Ativo == SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Status", "DescricaoAtivo", 15, Models.Grid.Align.center, false);

                Repositorio.Embarcador.Configuracoes.RegraAutomatizacaoEmissoesEmail repositorioRegraAutomatizacaoEmissoesEmail = new Repositorio.Embarcador.Configuracoes.RegraAutomatizacaoEmissoesEmail(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Entidades.Embarcador.Configuracoes.RegraAutomatizacaoEmissoesEmail> regrasAutomatizacaoEmissoesEmail = repositorioRegraAutomatizacaoEmissoesEmail.Consultar(filtrosPesquisa, parametrosConsulta);
                grid.setarQuantidadeTotal(repositorioRegraAutomatizacaoEmissoesEmail.ContarConsulta(filtrosPesquisa));

                var lista = (from p in regrasAutomatizacaoEmissoesEmail
                             select new
                             {
                                 p.Codigo,
                                 p.Descricao,
                                 p.EmailDestino,
                                 TipoOperacao = p.TipoOperacao.Descricao,
                                 p.DescricaoAtivo
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

                Repositorio.Embarcador.Configuracoes.RegraAutomatizacaoEmissoesEmail repositorioRegraAutomatizacaoEmissoesEmail = new Repositorio.Embarcador.Configuracoes.RegraAutomatizacaoEmissoesEmail(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.RegraAutomatizacaoEmissoesEmail regraAutomatizacaoEmissoesEmail = new Dominio.Entidades.Embarcador.Configuracoes.RegraAutomatizacaoEmissoesEmail();

                PreencherRegraAutomatizacaoEmissoesEmail(regraAutomatizacaoEmissoesEmail, unitOfWork);

                repositorioRegraAutomatizacaoEmissoesEmail.Inserir(regraAutomatizacaoEmissoesEmail, Auditado);

                SalvarRemetentes(regraAutomatizacaoEmissoesEmail, unitOfWork);
                SalvarDestinatario(regraAutomatizacaoEmissoesEmail, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
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
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Configuracoes.RegraAutomatizacaoEmissoesEmail repositorioRegraAutomatizacaoEmissoesEmail = new Repositorio.Embarcador.Configuracoes.RegraAutomatizacaoEmissoesEmail(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.RegraAutomatizacaoEmissoesEmail regraAutomatizacaoEmissoesEmail = repositorioRegraAutomatizacaoEmissoesEmail.BuscarPorCodigo(codigo, true);

                if (regraAutomatizacaoEmissoesEmail == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherRegraAutomatizacaoEmissoesEmail(regraAutomatizacaoEmissoesEmail, unitOfWork);

                repositorioRegraAutomatizacaoEmissoesEmail.Atualizar(regraAutomatizacaoEmissoesEmail, Auditado);

                SalvarRemetentes(regraAutomatizacaoEmissoesEmail, unitOfWork);
                SalvarDestinatario(regraAutomatizacaoEmissoesEmail, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
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
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Configuracoes.RegraAutomatizacaoEmissoesEmail repositorioRegraAutomatizacaoEmissoesEmail = new Repositorio.Embarcador.Configuracoes.RegraAutomatizacaoEmissoesEmail(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.RegraAutomatizacaoEmissoesEmail regraAutomatizacaoEmissoesEmail = repositorioRegraAutomatizacaoEmissoesEmail.BuscarPorCodigo(codigo, false);

                if (regraAutomatizacaoEmissoesEmail == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                var dynRegraAutomatizacaoEmissoesEmail = new
                {
                    regraAutomatizacaoEmissoesEmail.Codigo,
                    regraAutomatizacaoEmissoesEmail.Descricao,
                    regraAutomatizacaoEmissoesEmail.EmailDestino,
                    TipoOperacao = new { Codigo = regraAutomatizacaoEmissoesEmail.TipoOperacao?.Codigo ?? 0, Descricao = regraAutomatizacaoEmissoesEmail.TipoOperacao?.Descricao ?? string.Empty },
                    regraAutomatizacaoEmissoesEmail.Ativo,
                    Remetentes = (from obj in regraAutomatizacaoEmissoesEmail.Remetentes
                                  select new
                                  {
                                      Codigo = obj.Remetente.Codigo,
                                      Descricao = obj.Remetente.Descricao
                                  }).ToList(),
                    Destinatarios = (from obj in regraAutomatizacaoEmissoesEmail.Destinatarios
                                     select new
                                     {
                                         Codigo = obj.Destinatario.Codigo,
                                         Descricao = obj.Destinatario.Descricao
                                     }).ToList()
                };

                return new JsonpResult(dynRegraAutomatizacaoEmissoesEmail);
            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
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
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Configuracoes.RegraAutomatizacaoEmissoesEmail repositorioRegraAutomatizacaoEmissoesEmail = new Repositorio.Embarcador.Configuracoes.RegraAutomatizacaoEmissoesEmail(unitOfWork);
                Repositorio.Embarcador.Configuracoes.RegraAutomatizacaoEmissoesEmailRemetente repositorioRegraAutomatizacaoEmissoesEmailRemetente = new Repositorio.Embarcador.Configuracoes.RegraAutomatizacaoEmissoesEmailRemetente(unitOfWork);
                Repositorio.Embarcador.Configuracoes.RegraAutomatizacaoEmissoesEmailDestinatario repositorioRegraAutomatizacaoEmissoesEmailDestinatario = new Repositorio.Embarcador.Configuracoes.RegraAutomatizacaoEmissoesEmailDestinatario(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.RegraAutomatizacaoEmissoesEmail regraAutomatizacaoEmissoesEmail = repositorioRegraAutomatizacaoEmissoesEmail.BuscarPorCodigo(codigo, true);

                if (regraAutomatizacaoEmissoesEmail == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                repositorioRegraAutomatizacaoEmissoesEmail.Deletar(regraAutomatizacaoEmissoesEmail, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
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

        #region Métodos Privados

        private void PreencherRegraAutomatizacaoEmissoesEmail(Dominio.Entidades.Embarcador.Configuracoes.RegraAutomatizacaoEmissoesEmail regraAutomatizacaoEmissoesEmail, Repositorio.UnitOfWork unitOfWork)
        {
            int codigoTipoOperacao = Request.GetIntParam("TipoOperacao");

            Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            TipoOperacao tipoOperacao = repositorioTipoOperacao.BuscarPorCodigo(codigoTipoOperacao);

            regraAutomatizacaoEmissoesEmail.Descricao = Request.GetStringParam("Descricao");
            regraAutomatizacaoEmissoesEmail.EmailDestino = Request.GetStringParam("EmailDestino");
            regraAutomatizacaoEmissoesEmail.TipoOperacao = tipoOperacao;
            regraAutomatizacaoEmissoesEmail.Ativo = Request.GetBoolParam("Ativo");
        }

        private Dominio.ObjetosDeValor.Embarcador.Configuracoes.FiltroPesquisaRegraAutomatizacaoEmissoesEmail ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Configuracoes.FiltroPesquisaRegraAutomatizacaoEmissoesEmail()
            {
                Descricao = Request.GetStringParam("Descricao"),
                Ativo = Request.GetEnumParam("Ativo", SituacaoAtivoPesquisa.Ativo)
            };
        }

        private void SalvarRemetentes(Dominio.Entidades.Embarcador.Configuracoes.RegraAutomatizacaoEmissoesEmail regraAutomatizacaoEmissoesEmail, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.RegraAutomatizacaoEmissoesEmailRemetente repositorioRegraAutomatizacaoEmissoesEmailRemetente = new Repositorio.Embarcador.Configuracoes.RegraAutomatizacaoEmissoesEmailRemetente(unitOfWork);
            Repositorio.Cliente repositorioRemetente = new Repositorio.Cliente(unitOfWork);

            dynamic remetentes = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Remetentes"));

            List<Dominio.Entidades.Embarcador.Configuracoes.RegraAutomatizacaoEmissoesEmailRemetente> listaRegraAutomatizacaoEmissoesEmailRemetente = repositorioRegraAutomatizacaoEmissoesEmailRemetente.BuscarPorRegraAutomatizacaoEmissoesEmail(regraAutomatizacaoEmissoesEmail.Codigo);

            if (listaRegraAutomatizacaoEmissoesEmailRemetente.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var remetente in remetentes)
                    if (remetente.Codigo != null)
                        codigos.Add((int)remetente.Codigo);

                List<Dominio.Entidades.Embarcador.Configuracoes.RegraAutomatizacaoEmissoesEmailRemetente> listaRegraAutomatizacaoEmissoesEmailRemetenteDeletar = (from obj in listaRegraAutomatizacaoEmissoesEmailRemetente where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < listaRegraAutomatizacaoEmissoesEmailRemetenteDeletar.Count; i++)
                    repositorioRegraAutomatizacaoEmissoesEmailRemetente.Deletar(listaRegraAutomatizacaoEmissoesEmailRemetenteDeletar[i]);
            }

            foreach (var remetente in remetentes)
            {
                double clienteRemetente = ((string)remetente.Codigo).ToDouble();
                Dominio.Entidades.Embarcador.Configuracoes.RegraAutomatizacaoEmissoesEmailRemetente regraAutomatizacaoEmissoesEmailRemetente = clienteRemetente > 0 ? repositorioRegraAutomatizacaoEmissoesEmailRemetente.BuscarPorRegraAutomatizacaoEmissoesEmailERemetente(regraAutomatizacaoEmissoesEmail.Codigo, clienteRemetente) : null;

                if (regraAutomatizacaoEmissoesEmailRemetente == null)
                {
                    regraAutomatizacaoEmissoesEmailRemetente = new Dominio.Entidades.Embarcador.Configuracoes.RegraAutomatizacaoEmissoesEmailRemetente();

                    regraAutomatizacaoEmissoesEmailRemetente.RegraAutomatizacaoEmissoesEmail = regraAutomatizacaoEmissoesEmail;
                    regraAutomatizacaoEmissoesEmailRemetente.Remetente = clienteRemetente > 0 ? repositorioRemetente.BuscarPorCPFCNPJ(clienteRemetente) : null;

                    repositorioRegraAutomatizacaoEmissoesEmailRemetente.Inserir(regraAutomatizacaoEmissoesEmailRemetente);
                }
            }
        }

        private void SalvarDestinatario(Dominio.Entidades.Embarcador.Configuracoes.RegraAutomatizacaoEmissoesEmail regraAutomatizacaoEmissoesEmail, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.RegraAutomatizacaoEmissoesEmailDestinatario repositorioRegraAutomatizacaoEmissoesEmailDestinatario = new Repositorio.Embarcador.Configuracoes.RegraAutomatizacaoEmissoesEmailDestinatario(unitOfWork);
            Repositorio.Cliente repositorioDestinatario = new Repositorio.Cliente(unitOfWork);

            dynamic destinatarios = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Destinatarios"));

            List<Dominio.Entidades.Embarcador.Configuracoes.RegraAutomatizacaoEmissoesEmailDestinatario> listaRegraAutomatizacaoEmissoesEmailDestinatario = repositorioRegraAutomatizacaoEmissoesEmailDestinatario.BuscarPorRegraAutomatizacaoEmissoesEmail(regraAutomatizacaoEmissoesEmail.Codigo);

            if (listaRegraAutomatizacaoEmissoesEmailDestinatario.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var destinatario in destinatarios)
                    if (destinatario.Codigo != null)
                        codigos.Add((int)destinatario.Codigo);

                List<Dominio.Entidades.Embarcador.Configuracoes.RegraAutomatizacaoEmissoesEmailDestinatario> listaRegraAutomatizacaoEmissoesEmailDestinatarioDeletar = (from obj in listaRegraAutomatizacaoEmissoesEmailDestinatario where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < listaRegraAutomatizacaoEmissoesEmailDestinatarioDeletar.Count; i++)
                    repositorioRegraAutomatizacaoEmissoesEmailDestinatario.Deletar(listaRegraAutomatizacaoEmissoesEmailDestinatarioDeletar[i]);
            }

            foreach (var destinatario in destinatarios)
            {
                double clienteDestinatario = ((string)destinatario.Codigo).ToDouble();
                Dominio.Entidades.Embarcador.Configuracoes.RegraAutomatizacaoEmissoesEmailDestinatario regraAutomatizacaoEmissoesEmailDestinatario = clienteDestinatario > 0 ? repositorioRegraAutomatizacaoEmissoesEmailDestinatario.BuscarPorRegraAutomatizacaoEmissoesEmailEDestinatario(regraAutomatizacaoEmissoesEmail.Codigo, clienteDestinatario) : null;

                if (regraAutomatizacaoEmissoesEmailDestinatario == null)
                {
                    regraAutomatizacaoEmissoesEmailDestinatario = new Dominio.Entidades.Embarcador.Configuracoes.RegraAutomatizacaoEmissoesEmailDestinatario();

                    regraAutomatizacaoEmissoesEmailDestinatario.RegraAutomatizacaoEmissoesEmail = regraAutomatizacaoEmissoesEmail;
                    regraAutomatizacaoEmissoesEmailDestinatario.Destinatario = clienteDestinatario > 0 ? repositorioDestinatario.BuscarPorCPFCNPJ(clienteDestinatario) : null;

                    repositorioRegraAutomatizacaoEmissoesEmailDestinatario.Inserir(regraAutomatizacaoEmissoesEmailDestinatario);
                }
            }
        }

        #endregion
    }
}
