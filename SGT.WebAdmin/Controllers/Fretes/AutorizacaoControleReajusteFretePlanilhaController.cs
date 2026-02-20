using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Fretes
{
    [CustomAuthorize(new string[] { "RegrasAprovacao" }, "Fretes/AutorizacaoControleReajusteFretePlanilha")]
    public class AutorizacaoControleReajusteFretePlanilhaController : BaseController
    {
		#region Construtores

		public AutorizacaoControleReajusteFretePlanilhaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Aprovar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, "Você não possui permissão para executar essa ação.");

                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha repositorioAprovacao = new Repositorio.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha aprovacao = repositorioAprovacao.BuscarPorCodigo(codigo);

                if (aprovacao == null)
                    return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados.");

                if (aprovacao.Situacao != SituacaoAlcadaRegra.Pendente)
                    return new JsonpResult(false, "A situação da aprovação não permite alterações.");

                if (!aprovacao.IsPermitirAprovacaoOuReprovacao(this.Usuario.Codigo))
                    return new JsonpResult(false, "Aprovação não permite alterações.");

                Aprovar(aprovacao, unitOfWork);
                VerificarSituacaoControle(aprovacao.ControleReajusteFretePlanilha, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
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

        public async Task<IActionResult> AprovarMultiplasLinhas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, "Você não possui permissão para executar essa ação.");

                unitOfWork.Start();

                List<int> codigosControlesReajusteFretePlanilha = ObterCodigosControlesReajusteFretePlanilhaSelecionados(unitOfWork);
                List<Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha> aprovacoes = ObterAprovacoesPendentes(codigosControlesReajusteFretePlanilha, Usuario.Codigo, unitOfWork);

                foreach (Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha aprovacao in aprovacoes)
                {
                    if (aprovacao.IsPermitirAprovacaoOuReprovacao(Usuario.Codigo))
                        Aprovar(aprovacao, unitOfWork);
                }

                IEnumerable<Dominio.Entidades.Embarcador.Frete.ControleReajusteFretePlanilha> controlesReajusteFretePlanilha = (from aprovacao in aprovacoes select aprovacao.ControleReajusteFretePlanilha).Distinct();

                foreach (Dominio.Entidades.Embarcador.Frete.ControleReajusteFretePlanilha controleReajusteFretePlanilha in controlesReajusteFretePlanilha)
                    VerificarSituacaoControle(controleReajusteFretePlanilha, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(new
                {
                    RegrasModificadas = aprovacoes.Count()
                });
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao aprovar as solicitações.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AprovarMultiplasRegras()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, "Você não possui permissão para executar essa ação.");

                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha repositorioAprovacao = new Repositorio.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha(unitOfWork);
                List<Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha> aprovacoesPendentes = repositorioAprovacao.BuscarPendentes(codigo, Usuario.Codigo);

                foreach (Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha aprovacao in aprovacoesPendentes)
                {
                    if (aprovacao.IsPermitirAprovacaoOuReprovacao(Usuario.Codigo))
                        Aprovar(aprovacao, unitOfWork);
                }

                Dominio.Entidades.Embarcador.Frete.ControleReajusteFretePlanilha controleReajusteFretePlanilha = (from aprovacao in aprovacoesPendentes select aprovacao.ControleReajusteFretePlanilha).FirstOrDefault();

                if (controleReajusteFretePlanilha != null)
                    VerificarSituacaoControle(controleReajusteFretePlanilha, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(new
                {
                    RegrasModificadas = aprovacoesPendentes.Count()
                });
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao aprovar as regras.");
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
                Repositorio.Embarcador.Frete.ControleReajusteFretePlanilha repositorioControleReajusteFretePlanilha = new Repositorio.Embarcador.Frete.ControleReajusteFretePlanilha(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.ControleReajusteFretePlanilha controle = repositorioControleReajusteFretePlanilha.BuscarPorCodigo(codigo);

                if (controle == null)
                    return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");

                return new JsonpResult(new
                {
                    controle.Codigo,
                    controle.Numero,
                    Solicitante = controle.Usuario?.Nome ?? "",
                    TipoOperacao = controle.TipoOperacao.Descricao,
                    Filial = controle.Filial.Descricao,
                    Empresa = controle.Empresa?.Descricao ?? "",
                    controle.Observacao,
                    controle.Situacao
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
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
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Frete.ControleReajusteFretePlanilha repositorioControleReajusteFretePlanilha = new Repositorio.Embarcador.Frete.ControleReajusteFretePlanilha(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.ControleReajusteFretePlanilha controleReajusteFretePlanilha = repositorioControleReajusteFretePlanilha.BuscarPorCodigo(codigo);

                if (controleReajusteFretePlanilha == null)
                    return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados.");

                int codigoUsuario = Request.GetIntParam("UsuarioDelegado");
                Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(unitOfWork);
                Dominio.Entidades.Usuario usuarioDelegado = repositorioUsuario.BuscarPorCodigo(codigoUsuario);

                if (usuarioDelegado == null)
                    return new JsonpResult(false, true, "Ocorreu uma falha ao buscar o usuário.");

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador(unitOfWork);

                if ((configuracaoEmbarcador?.NaoExibirOpcaoParaDelegar ?? false) || IsPermitirDelegar(controleReajusteFretePlanilha))
                    return new JsonpResult(false, true, "Operação não permitida.");

                if ((configuracaoEmbarcador?.NaoPermitirDelegarAoUsuarioLogado ?? false) && Usuario.Codigo == usuarioDelegado.Codigo)
                    return new JsonpResult(false, true, "Não é permitido delegar para você mesmo.");

                Repositorio.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha repositorioAprovacao = new Repositorio.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha(unitOfWork);

                if (repositorioAprovacao.BuscarPorControleEUsuario(codigo, codigoUsuario).Count > 0)
                    return new JsonpResult(false, true, "O usuário selecionado já é o responsável.");

                Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha aprovacao = new Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha();

                aprovacao.DataCriacao = DateTime.Now;
                aprovacao.Delegada = true;
                aprovacao.ControleReajusteFretePlanilha = controleReajusteFretePlanilha;
                aprovacao.Situacao = SituacaoAlcadaRegra.Pendente;
                aprovacao.Usuario = usuarioDelegado;

                repositorioAprovacao.Inserir(aprovacao);

                return new JsonpResult(true);
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

        public async Task<IActionResult> DelegarMultiplas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoUsuario = Request.GetIntParam("UsuarioDelegado");
                Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(unitOfWork);
                Dominio.Entidades.Usuario usuarioDelegado = repositorioUsuario.BuscarPorCodigo(codigoUsuario);

                if (usuarioDelegado == null)
                    return new JsonpResult(false, true, "Ocorreu uma falha ao buscar o usuário.");

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador(unitOfWork);

                if (configuracaoEmbarcador?.NaoExibirOpcaoParaDelegar ?? false)
                    return new JsonpResult(false, true, "Operação não permitida.");

                if ((configuracaoEmbarcador?.NaoPermitirDelegarAoUsuarioLogado ?? false) && Usuario.Codigo == usuarioDelegado.Codigo)
                    return new JsonpResult(false, true, "Não é permitido delegar para você mesmo.");

                Repositorio.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha repositorioAprovacao = new Repositorio.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha(unitOfWork);
                Repositorio.Embarcador.Frete.ControleReajusteFretePlanilha repositorioControleReajusteFretePlanilha = new Repositorio.Embarcador.Frete.ControleReajusteFretePlanilha(unitOfWork);
                List<int> codigosControlesReajusteFretePlanilha = ObterCodigosControlesReajusteFretePlanilhaSelecionados(unitOfWork);

                unitOfWork.Start();

                foreach (int codigoControleReajusteFretePlanilha in codigosControlesReajusteFretePlanilha)
                {
                    Dominio.Entidades.Embarcador.Frete.ControleReajusteFretePlanilha controleReajusteFretePlanilha = repositorioControleReajusteFretePlanilha.BuscarPorCodigo(codigoControleReajusteFretePlanilha);

                    if (
                        (controleReajusteFretePlanilha != null) &&
                        IsPermitirDelegar(controleReajusteFretePlanilha) &&
                        (repositorioAprovacao.BuscarPorControleEUsuario(codigoControleReajusteFretePlanilha, codigoUsuario).Count == 0)
                    )
                    {
                        Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha aprovacao = new Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha();

                        aprovacao.DataCriacao = DateTime.Now;
                        aprovacao.Delegada = true;
                        aprovacao.ControleReajusteFretePlanilha = controleReajusteFretePlanilha;
                        aprovacao.Situacao = SituacaoAlcadaRegra.Pendente;
                        aprovacao.Usuario = usuarioDelegado;

                        repositorioAprovacao.Inserir(aprovacao);
                    }
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao delegar múltiplos registros.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            try
            {
                Models.Grid.Grid grid = ObterGridPesquisa();
                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);

                return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisa());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        public async Task<IActionResult> RegrasAprovacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                int codigoUsuario = Request.GetIntParam("Usuario");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Regra", "Regra", 30, Models.Grid.Align.left, false);

                if (codigoUsuario > 0)
                    grid.AdicionarCabecalho("Usuario", false);
                else
                    grid.AdicionarCabecalho("Usuário", "Usuario", 15, Models.Grid.Align.left, false);

                grid.AdicionarCabecalho("Situação", "Situacao", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("PodeAprovar", false);

                Repositorio.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha repositorioAprovacao = new Repositorio.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha(unitOfWork);
                List<Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha> aprovacoes = repositorioAprovacao.BuscarPorControleEUsuario(codigo, codigoUsuario);

                var aprovacoesRetornar = (
                    from aprovacao in aprovacoes
                    select new
                    {
                        aprovacao.Codigo,
                        Regra = aprovacao.Descricao,
                        Situacao = aprovacao.Situacao.ObterDescricao(),
                        Usuario = aprovacao.Usuario.Nome,
                        PodeAprovar = aprovacao.IsPermitirAprovacaoOuReprovacao(codigoUsuario),
                        DT_RowColor = aprovacao.Situacao.ObterCorGrid()
                    }
                ).ToList();

                grid.AdicionaRows(aprovacoesRetornar);
                grid.setarQuantidadeTotal(aprovacoesRetornar.Count());

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

        public async Task<IActionResult> Reprovar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, "Você não possui permissão para executar essa ação.");

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha repositorioAprovacao = new Repositorio.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha aprovacao = repositorioAprovacao.BuscarPorCodigo(codigo);

                if (!aprovacao.IsPermitirAprovacaoOuReprovacao(Usuario.Codigo))
                    return new JsonpResult(false, "Aprovação não permite alterações.");

                unitOfWork.Start();

                PreencherDadosRejeicaoAprovacao(aprovacao);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, aprovacao.ControleReajusteFretePlanilha, $"{Usuario.Nome} reprovou a regra. Motivo: {aprovacao.Motivo}", unitOfWork);
                repositorioAprovacao.Atualizar(aprovacao);
                VerificarSituacaoControle(aprovacao.ControleReajusteFretePlanilha, unitOfWork);

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
                return new JsonpResult(false, "Ocorreu uma falha ao reprovar o reajuste.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReprovarMultiplasLinhas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, "Você não possui permissão para executar essa ação.");

                unitOfWork.Start();

                Repositorio.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha repositorioAprovacao = new Repositorio.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha(unitOfWork);
                List<int> codigosControlesReajusteFretePlanilha = ObterCodigosControlesReajusteFretePlanilhaSelecionados(unitOfWork);
                List<Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha> aprovacoes = ObterAprovacoesPendentes(codigosControlesReajusteFretePlanilha, Usuario.Codigo, unitOfWork);

                foreach (Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha aprovacao in aprovacoes)
                {
                    if (aprovacao.IsPermitirAprovacaoOuReprovacao(Usuario.Codigo))
                    {
                        PreencherDadosRejeicaoAprovacao(aprovacao);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, aprovacao.ControleReajusteFretePlanilha, null, $"{Usuario.Nome} reprovou a regra. Motivo: {aprovacao.Motivo}", unitOfWork);
                        repositorioAprovacao.Atualizar(aprovacao);
                    }
                }

                IEnumerable<Dominio.Entidades.Embarcador.Frete.ControleReajusteFretePlanilha> controlesReajusteFretePlanilha = (from aprovacao in aprovacoes select aprovacao.ControleReajusteFretePlanilha).Distinct();

                foreach (Dominio.Entidades.Embarcador.Frete.ControleReajusteFretePlanilha controleReajusteFretePlanilha in controlesReajusteFretePlanilha)
                    VerificarSituacaoControle(controleReajusteFretePlanilha, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(new
                {
                    RegrasModificadas = aprovacoes.Count()
                });
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
                return new JsonpResult(false, "Ocorreu uma falha ao reprovar os reajustes.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion Métodos Globais

        #region Métodos Privados

        private void Aprovar(Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha aprovacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha repositorioAprovacao = new Repositorio.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha(unitOfWork);

            aprovacao.Data = DateTime.Now;
            aprovacao.Situacao = SituacaoAlcadaRegra.Aprovada;

            repositorioAprovacao.Atualizar(aprovacao);
            Servicos.Auditoria.Auditoria.Auditar(Auditado, aprovacao.ControleReajusteFretePlanilha, $"{Usuario.Nome} aprovou a regra", unitOfWork);
        }

        private bool IsPermitirDelegar(Dominio.Entidades.Embarcador.Frete.ControleReajusteFretePlanilha controle)
        {
            return controle.Situacao == SituacaoControleReajusteFretePlanilha.AgAprovacao;
        }

        private void NotificarAlteracaoControle(Dominio.Entidades.Embarcador.Frete.ControleReajusteFretePlanilha controle, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Notificacao.Notificacao servicoNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(_conexao.StringConexao, null, TipoServicoMultisoftware, string.Empty);
            bool aprovado = controle.Situacao == SituacaoControleReajusteFretePlanilha.Aprovado;
            string mensagem = string.Format(Localization.Resources.Fretes.AutorizacaoControleReajusteFretePlanilha.ControleFreteFoi, controle.Numero, (aprovado ? Localization.Resources.Gerais.Geral.Aprovado : Localization.Resources.Gerais.Geral.Rejeitado));
            IconesNotificacao icone = aprovado ? IconesNotificacao.confirmado : IconesNotificacao.rejeitado;

            servicoNotificacao.GerarNotificacao(controle.Usuario, this.Usuario, controle.Codigo, "Fretes/ControleReajusteFretePlanilha", mensagem, icone, TipoNotificacao.credito, TipoServicoMultisoftware, unitOfWork);
        }

        private List<Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha> ObterAprovacoesPendentes(List<int> codigosControlesReajusteFretePlanilha, int codigoUsuario, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha repositorioAprovacao = new Repositorio.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha(unitOfWork);
            List<Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha> aprovacoes = new List<Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha>();

            foreach (int codigoControlesReajusteFretePlanilha in codigosControlesReajusteFretePlanilha)
            {
                List<Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha> regras = repositorioAprovacao.BuscarPendentes(codigoControlesReajusteFretePlanilha, codigoUsuario);

                aprovacoes.AddRange(regras);
            }

            return aprovacoes;
        }

        private List<int> ObterCodigosControlesReajusteFretePlanilhaSelecionados(Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Embarcador.Frete.ControleReajusteFretePlanilha> controlesReajusteFretePlanilha;
            var selecionarTodos = Request.GetBoolParam("SelecionarTodos");

            if (selecionarTodos)
            {
                var listaItensNaoSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensNaoSelecionados"));
                Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaControleReajusteFretePlanilhaAprovacao filtrosPesquisa = ObterFiltrosPesquisa();
                Repositorio.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha repositorioAprovacaoAlcada = new Repositorio.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha(unitOfWork);

                controlesReajusteFretePlanilha = repositorioAprovacaoAlcada.Consultar(filtrosPesquisa, new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta() { PropriedadeOrdenar = "Codigo" });

                foreach (var itemNaoSelecionado in listaItensNaoSelecionados)
                    controlesReajusteFretePlanilha.Remove(new Dominio.Entidades.Embarcador.Frete.ControleReajusteFretePlanilha() { Codigo = (int)itemNaoSelecionado.Codigo });
            }
            else
            {
                Repositorio.Embarcador.Frete.ControleReajusteFretePlanilha repositorioControleReajusteFretePlanilha = new Repositorio.Embarcador.Frete.ControleReajusteFretePlanilha(unitOfWork);
                var listaItensSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensSelecionados"));

                controlesReajusteFretePlanilha = new List<Dominio.Entidades.Embarcador.Frete.ControleReajusteFretePlanilha>();

                foreach (var itemSelecionado in listaItensSelecionados)
                    controlesReajusteFretePlanilha.Add(repositorioControleReajusteFretePlanilha.BuscarPorCodigo((int)itemSelecionado.Codigo));
            }

            return (from controleReajusteFretePlanilha in controlesReajusteFretePlanilha select controleReajusteFretePlanilha.Codigo).ToList();
        }

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ObterConfiguracaoEmbarcador(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            return repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
        }

        private Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaControleReajusteFretePlanilhaAprovacao ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaControleReajusteFretePlanilhaAprovacao()
            {
                CodigoEmpresa = Request.GetIntParam("Empresa"),
                CodigoFilial= Request.GetIntParam("Filial"),
                CodigoTipoOperacao = Request.GetIntParam("TipoOperacao"),
                CodigoUsuario = Request.GetIntParam("Usuario"),
                DataFinal = Request.GetNullableDateTimeParam("DataFinal"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicial"),
                Numero = Request.GetIntParam("Numero"),
                Situacao = Request.GetNullableEnumParam<SituacaoControleReajusteFretePlanilha>("Situacao")
            };
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.Prop("Codigo");
                grid.Prop("Numero").Nome("Número").Tamanho(7);
                grid.Prop("Data").Nome("Data").Tamanho(7).Align(Models.Grid.Align.center);
                grid.Prop("TipoOperacao").Nome("Tipo de Operação").Tamanho(15);
                grid.Prop("Empresa").Nome("Transportador").Tamanho(15);
                grid.Prop("Filial").Nome("Filial").Tamanho(15);
                grid.Prop("Situacao").Nome("Situação").Tamanho(10);
                grid.Prop("Aprovadores").Nome("Aprovadores").Tamanho(10);

                Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaControleReajusteFretePlanilhaAprovacao filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                Repositorio.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha repositorioAprovacaoAlcada = new Repositorio.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha(unitOfWork);
                int totalRegistros = repositorioAprovacaoAlcada.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Frete.ControleReajusteFretePlanilha> controlesReajusteFretePlanilha = (totalRegistros > 0) ? repositorioAprovacaoAlcada.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Frete.ControleReajusteFretePlanilha>();

                var controlesReajusteFretePlanilhaRetornar = (
                    from controleReajusteFretePlanilha in controlesReajusteFretePlanilha
                    select new
                    {
                        controleReajusteFretePlanilha.Codigo,
                        Data = controleReajusteFretePlanilha.DataCriacao.ToString("dd/MM/yyyy"),
                        Numero = controleReajusteFretePlanilha.Numero.ToString(),
                        TipoOperacao = controleReajusteFretePlanilha.TipoOperacao.Descricao,
                        Empresa = controleReajusteFretePlanilha.Empresa?.Descricao ?? "",
                        Filial = controleReajusteFretePlanilha.Filial.Descricao,
                        Situacao = controleReajusteFretePlanilha.DescricaoSituacao,
                        Aprovadores = string.Join(", ", controleReajusteFretePlanilha?.ControleAutorizacoes?.Select(obj => obj.Usuario.Nome))
                    }
                ).ToList();

                grid.AdicionaRows(controlesReajusteFretePlanilhaRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
        }

      
        private SituacaoRegrasAutorizacao ObterSituacaoRegrasAutorizacao(Dominio.Entidades.Embarcador.Frete.ControleReajusteFretePlanilha controle, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha repositorioAprovacao = new Repositorio.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha(unitOfWork);
            List<Dominio.Entidades.Embarcador.Frete.RegraControleReajusteFretePlanilha> regras = repositorioAprovacao.BuscarRegras(controle.Codigo);

            foreach (Dominio.Entidades.Embarcador.Frete.RegraControleReajusteFretePlanilha regra in regras)
            {
                int aprovacoes = repositorioAprovacao.ContarAprovacoes(controle.Codigo, regra.Codigo);
                int reprovacoes = repositorioAprovacao.ContarReprovacoes(controle.Codigo, regra.Codigo);
                int numeroAprovacoesNecessarias = repositorioAprovacao.BuscarNumeroAprovacoesNecessariasPorRegra(controle.Codigo, regra.Codigo);

                if (reprovacoes > 0)
                    return SituacaoRegrasAutorizacao.Reprovadas;

                if (aprovacoes < numeroAprovacoesNecessarias)
                    return SituacaoRegrasAutorizacao.Aguardando;
            }

            return SituacaoRegrasAutorizacao.Aprovadas;
        }

        private void PreencherDadosRejeicaoAprovacao(Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha aprovacao)
        {
            string motivo = Request.GetStringParam("Motivo");

            if (string.IsNullOrWhiteSpace(motivo))
                throw new ControllerException("Motivo é obrigatório.");

            aprovacao.Data = DateTime.Now;
            aprovacao.Situacao = SituacaoAlcadaRegra.Rejeitada;
            aprovacao.Motivo = motivo;
        }

        private void VerificarSituacaoControle(Dominio.Entidades.Embarcador.Frete.ControleReajusteFretePlanilha controle, Repositorio.UnitOfWork unitOfWork)
        {
            if (controle.Situacao != SituacaoControleReajusteFretePlanilha.AgAprovacao)
                return;

            SituacaoRegrasAutorizacao situacaoRegrasAutorizacao = ObterSituacaoRegrasAutorizacao(controle, unitOfWork);

            if (situacaoRegrasAutorizacao == SituacaoRegrasAutorizacao.Aguardando)
                return;

            Repositorio.Embarcador.Frete.ControleReajusteFretePlanilha repositorioControleReajusteFretePlanilha = new Repositorio.Embarcador.Frete.ControleReajusteFretePlanilha(unitOfWork);

            if (situacaoRegrasAutorizacao == SituacaoRegrasAutorizacao.Aprovadas)
                controle.Situacao = SituacaoControleReajusteFretePlanilha.Aprovado;
            else
                controle.Situacao = SituacaoControleReajusteFretePlanilha.Rejeitado;

            repositorioControleReajusteFretePlanilha.Atualizar(controle);
            NotificarAlteracaoControle(controle, unitOfWork);
        }

        #endregion Métodos Privados
    }
}
