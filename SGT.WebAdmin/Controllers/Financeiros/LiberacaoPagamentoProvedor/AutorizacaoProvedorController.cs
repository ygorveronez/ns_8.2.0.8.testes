using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Financeiros.LiberacaoPagamentoProvedor
{
    [CustomAuthorize("Financeiros/LiberacaoPagamentoProvedor")]
    public class AutorizacaoProvedorController : BaseController
    {
		#region Construtores

		public AutorizacaoProvedorController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> RegrasAprovacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                int codigoAprovacaoAlcadaRegra = Request.GetIntParam("CodigoAprovacaoAlcadaRegra");
                int codigoUsuario = Request.GetIntParam("Usuario");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoUsuario", false);
                grid.AdicionarCabecalho("PodeAprovar", false);
                grid.AdicionarCabecalho("Regra", "Regra", 30, Models.Grid.Align.left, false);

                if (codigoUsuario > 0)
                    grid.AdicionarCabecalho("Usuario", false);
                else
                    grid.AdicionarCabecalho("Usuário", "Usuario", 15, Models.Grid.Align.left, false);

                grid.AdicionarCabecalho("Situação", "Situacao", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Justificativa", "Justificativa", 10, Models.Grid.Align.left, false);

                Repositorio.Embarcador.RegraAutorizacao.AprovacaoAlcada<Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.AprovacaoAlcadaPagamentoProvedor, Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.RegraPagamentoProvedor, Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedor> repositorioAprovacao = new Repositorio.Embarcador.RegraAutorizacao.AprovacaoAlcada<Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.AprovacaoAlcadaPagamentoProvedor, Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.RegraPagamentoProvedor, Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedor>(unitOfWork);
                List<Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.AprovacaoAlcadaPagamentoProvedor> aprovacoes = repositorioAprovacao.BuscarDesbloqueada(codigo, codigoUsuario);

                dynamic lista = (
                    from aprovacao in aprovacoes
                    select new
                    {
                        Codigo = aprovacao.Codigo,
                        CodigoUsuario = aprovacao.Usuario?.Codigo ?? 0,
                        Regra = aprovacao.Descricao,
                        Situacao = aprovacao.Situacao.ObterDescricao(),
                        Usuario = aprovacao.Usuario?.Nome + (aprovacao.Data.HasValue ? " - " + aprovacao.Data.Value.ToString("dd/MM/yyyy HH:mm") : ""),
                        PodeAprovar = aprovacao.IsPermitirAprovacaoOuReprovacao(this.Usuario.Codigo),
                        DT_RowColor = aprovacao.Situacao.ObterCorGrid(),
                        Justificativa = aprovacao.Situacao == SituacaoAlcadaRegra.Aprovada ? aprovacao.OrigemAprovacao.MotivoAprovacaoRegra : aprovacao.Situacao == SituacaoAlcadaRegra.Rejeitada ? aprovacao.OrigemAprovacao.MotivoRejeicaoRegra : string.Empty
                    }
                ).ToList();

                grid.setarQuantidadeTotal(lista.Count);
                grid.AdicionaRows(lista);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Delegar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("CodigoPagamentoProvedor");
                int codigoUsuario = Request.GetIntParam("UsuarioDelegado");


                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, "Você não possui permissão para executar essa ação.");

                Repositorio.RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedor> repositorioOrigem = new Repositorio.RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedor>(unitOfWork);
                Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedor origem = repositorioOrigem.BuscarPorCodigo(codigo, auditavel: false);

                if (origem == null)
                    return new JsonpResult(false, true, "Ocorreu uma falha ao buscar os dados.");

                Dominio.Entidades.Usuario usuarioDelegado = repositorioUsuario.BuscarPorCodigo(codigoUsuario);

                if (usuarioDelegado == null)
                    return new JsonpResult(false, true, "Ocorreu uma falha ao buscar o usuário.");

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador(unitOfWork);

                if (configuracaoEmbarcador?.NaoExibirOpcaoParaDelegar ?? false)
                    return new JsonpResult(false, true, "Operação não permitida.");

                if ((configuracaoEmbarcador?.NaoPermitirDelegarAoUsuarioLogado ?? false) && Usuario.Codigo == usuarioDelegado.Codigo)
                    return new JsonpResult(false, true, "Não é permitido delegar para você mesmo.");

                Repositorio.Embarcador.RegraAutorizacao.AprovacaoAlcada<Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.AprovacaoAlcadaPagamentoProvedor, Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.RegraPagamentoProvedor, Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedor> repositorioAprovacao = new Repositorio.Embarcador.RegraAutorizacao.AprovacaoAlcada<Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.AprovacaoAlcadaPagamentoProvedor, Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.RegraPagamentoProvedor, Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedor>(unitOfWork);

                if (repositorioAprovacao.BuscarDesbloqueada(codigo, codigoUsuario).Count > 0)
                    return new JsonpResult(false, true, "O usuário selecionado já é o responsável.");

                Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.AprovacaoAlcadaPagamentoProvedor aprovacao = new Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.AprovacaoAlcadaPagamentoProvedor();

                aprovacao.DataCriacao = DateTime.Now;
                aprovacao.Delegada = true;
                aprovacao.OrigemAprovacao = origem;
                aprovacao.Situacao = SituacaoAlcadaRegra.Pendente;
                aprovacao.Usuario = usuarioDelegado;

                repositorioAprovacao.Inserir(aprovacao);

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao delegar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Aprovar()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                var codigo = Request.GetIntParam("Codigo");
                string motivoAprovacao = Request.GetStringParam("Justificativa");

                Repositorio.Embarcador.RegraAutorizacao.AprovacaoAlcada<Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.AprovacaoAlcadaPagamentoProvedor, Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.RegraPagamentoProvedor, Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedor> repositorioAprovacao = new Repositorio.Embarcador.RegraAutorizacao.AprovacaoAlcada<Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.AprovacaoAlcadaPagamentoProvedor, Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.RegraPagamentoProvedor, Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedor>(unitOfWork);
                Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.AprovacaoAlcadaPagamentoProvedor aprovacao = repositorioAprovacao.BuscarPorCodigo(codigo);

                if (aprovacao == null)
                    return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados.");

                if (aprovacao.Situacao != SituacaoAlcadaRegra.Pendente)
                    return new JsonpResult(false, "A situação da aprovação não permite alterações.");

                if (!aprovacao.IsPermitirAprovacaoOuReprovacao(this.Usuario.Codigo))
                    return new JsonpResult(false, "Aprovação não permite alterações.");

                aprovacao.OrigemAprovacao.MotivoAprovacaoRegra = motivoAprovacao;

                Aprovar(aprovacao, unitOfWork);

                VerificarSituacaoOrigem(aprovacao.OrigemAprovacao, unitOfWork);

                aprovacao.OrigemAprovacao.EtapaLiberacaoPagamentoProvedor = EtapaLiberacaoPagamentoProvedor.Liberacao;
                aprovacao.OrigemAprovacao.SituacaoLiberacaoPagamentoProvedor = SituacaoLiberacaoPagamentoProvedor.Finalizada;

                List<Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedorCarga> pagamentoProvedorCargas = aprovacao.OrigemAprovacao.PagamentoProvedorCarga.ToList();

                if (aprovacao.OrigemAprovacao.TipoDocumentoProvedor == TipoDocumentoProvedor.NFSe)
                    CalcularRateioEntreCargas(pagamentoProvedorCargas, aprovacao.OrigemAprovacao.ValorProvedor);

                repositorioAprovacao.Atualizar(aprovacao);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao aprovar a regra.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Reprovar()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var codigo = Request.GetIntParam("Codigo");
                string motivoAprovacao = Request.GetStringParam("Justificativa");

                Repositorio.Embarcador.RegraAutorizacao.AprovacaoAlcada<Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.AprovacaoAlcadaPagamentoProvedor, Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.RegraPagamentoProvedor, Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedor> repositorioAprovacao = new Repositorio.Embarcador.RegraAutorizacao.AprovacaoAlcada<Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.AprovacaoAlcadaPagamentoProvedor, Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.RegraPagamentoProvedor, Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedor>(unitOfWork);
                Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.AprovacaoAlcadaPagamentoProvedor aprovacao = repositorioAprovacao.BuscarPorCodigo(codigo);

                if (!aprovacao.IsPermitirAprovacaoOuReprovacao(this.Usuario.Codigo))
                    return new JsonpResult(false, "Aprovação não permite alterações.");

                unitOfWork.Start();

                aprovacao.OrigemAprovacao.MotivoRejeicaoRegra = motivoAprovacao;

                Reprovar(aprovacao, unitOfWork);

                aprovacao.OrigemAprovacao.SituacaoLiberacaoPagamentoProvedor = SituacaoLiberacaoPagamentoProvedor.Rejeitada;

                repositorioAprovacao.Atualizar(aprovacao);

                VerificarSituacaoOrigem(aprovacao.OrigemAprovacao, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao reprovar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ObterConfiguracaoEmbarcador(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            return repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
        }

        private void Aprovar(Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.AprovacaoAlcadaPagamentoProvedor aprovacaoAlcada, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.RegraAutorizacao.AprovacaoAlcada<Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.AprovacaoAlcadaPagamentoProvedor, Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.RegraPagamentoProvedor, Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedor> repositorioAprovacao = new Repositorio.Embarcador.RegraAutorizacao.AprovacaoAlcada<Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.AprovacaoAlcadaPagamentoProvedor, Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.RegraPagamentoProvedor, Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedor>(unitOfWork);

            aprovacaoAlcada.Data = DateTime.Now;
            aprovacaoAlcada.Situacao = SituacaoAlcadaRegra.Aprovada;

            repositorioAprovacao.Atualizar(aprovacaoAlcada);

            Servicos.Auditoria.Auditoria.Auditar(Auditado, aprovacaoAlcada.OrigemAprovacao, $"{Auditado.Usuario.Nome} aprovou a regra. Motivo: {aprovacaoAlcada.OrigemAprovacao.MotivoAprovacaoRegra}", unitOfWork);
        }

        private void Reprovar(Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.AprovacaoAlcadaPagamentoProvedor aprovacaoAlcada, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.RegraAutorizacao.AprovacaoAlcada<Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.AprovacaoAlcadaPagamentoProvedor, Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.RegraPagamentoProvedor, Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedor> repositorioAprovacao = new Repositorio.Embarcador.RegraAutorizacao.AprovacaoAlcada<Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.AprovacaoAlcadaPagamentoProvedor, Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.RegraPagamentoProvedor, Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedor>(unitOfWork);

            aprovacaoAlcada.Data = DateTime.Now;
            aprovacaoAlcada.Situacao = SituacaoAlcadaRegra.Rejeitada;

            repositorioAprovacao.Atualizar(aprovacaoAlcada);

            Servicos.Auditoria.Auditoria.Auditar(Auditado, aprovacaoAlcada.OrigemAprovacao, $"{Auditado.Usuario.Nome} reprovou a regra. Motivo: {aprovacaoAlcada.OrigemAprovacao.MotivoRejeicaoRegra}", unitOfWork);
        }

        private void VerificarSituacaoOrigem(Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedor origem, Repositorio.UnitOfWork unitOfWork)
        {
            if (origem.SituacaoAlteracaoRegraPagamentoProvedor != SituacaoAlteracaoRegraPagamentoProvedor.AguardandoAprovacao)
                return;

            SituacaoRegrasAutorizacao situacaoRegrasAutorizacao = ObterSituacaoRegrasAutorizacao(origem.Codigo, unitOfWork);

            if (situacaoRegrasAutorizacao == SituacaoRegrasAutorizacao.Aguardando)
                return;

            Repositorio.Embarcador.Financeiro.PagamentoProvedor repositoriOPagamentoProvedor = new Repositorio.Embarcador.Financeiro.PagamentoProvedor(unitOfWork);
            Servicos.Embarcador.Financeiro.AprovacaoPagamentoProvedor servicoAprovacaoPagamentoProvedor = new Servicos.Embarcador.Financeiro.AprovacaoPagamentoProvedor(unitOfWork);

            if (situacaoRegrasAutorizacao == SituacaoRegrasAutorizacao.Aprovadas)
            {
                if (!servicoAprovacaoPagamentoProvedor.LiberarProximaPrioridadeAprovacao(origem, TipoServicoMultisoftware))
                    return;

                origem.SituacaoAlteracaoRegraPagamentoProvedor = SituacaoAlteracaoRegraPagamentoProvedor.Aprovada;

                Servicos.Auditoria.Auditoria.Auditar(Auditado, origem, "Configuração de regra aprovada", unitOfWork);
            }
            else
            {
                origem.SituacaoAlteracaoRegraPagamentoProvedor = SituacaoAlteracaoRegraPagamentoProvedor.Reprovada;

                Servicos.Auditoria.Auditoria.Auditar(Auditado, origem, "Configuração de regra reprovada", unitOfWork);
            }

            repositoriOPagamentoProvedor.Atualizar(origem);
        }

        private void CalcularRateioEntreCargas(List<Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedorCarga> pagamentoProvedorCargas, decimal valorProvedorEmitidaManualmente)
        {
            decimal valorTotalProvedor = 0;
            foreach (var pagamentoProvedorCarga in pagamentoProvedorCargas)
            {
                valorTotalProvedor += pagamentoProvedorCarga.Carga.ValorTotalProvedor;
            }

            foreach (var pagamentoProvedorCarga in pagamentoProvedorCargas)
            {
                pagamentoProvedorCarga.ValorRateado = (pagamentoProvedorCarga.Carga.ValorTotalProvedor / valorTotalProvedor) * valorProvedorEmitidaManualmente;
            }
        }

        private SituacaoRegrasAutorizacao ObterSituacaoRegrasAutorizacao(int codigoOrigem, Repositorio.UnitOfWork unitOfWork)
        {
            var repositorioAprovacao = new Repositorio.Embarcador.RegraAutorizacao.AprovacaoAlcada<Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.AprovacaoAlcadaPagamentoProvedor, Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.RegraPagamentoProvedor, Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedor>(unitOfWork);
            var regras = repositorioAprovacao.BuscarRegrasDesbloqueadas(codigoOrigem);

            foreach (var regra in regras)
            {
                int aprovacoes = repositorioAprovacao.ContarAprovacoes(codigoOrigem, regra.Codigo);
                int reprovacoes = repositorioAprovacao.ContarReprovacoes(codigoOrigem, regra.Codigo);
                int numeroAprovacoesNecessarias = repositorioAprovacao.BuscarNumeroAprovacoesNecessariasPorRegra(codigoOrigem, regra.Codigo);

                if (reprovacoes > 0)
                    return SituacaoRegrasAutorizacao.Reprovadas;

                if (aprovacoes < numeroAprovacoesNecessarias)
                    return SituacaoRegrasAutorizacao.Aguardando;
            }

            return SituacaoRegrasAutorizacao.Aprovadas;
        }

        #endregion
    }
}
