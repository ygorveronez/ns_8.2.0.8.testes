using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.RegraAutorizacao
{
    public abstract class AutorizacaoController<TAprovacao, TRegra, TOrigem> : BaseController
        where TAprovacao : Dominio.Entidades.Embarcador.RegraAutorizacao.AprovacaoAlcada<TOrigem, TRegra>, new()
        where TRegra : Dominio.Entidades.Embarcador.RegraAutorizacao.RegraAutorizacao
        where TOrigem : Dominio.Entidades.EntidadeBase, Dominio.Interfaces.Embarcador.Entidade.IEntidade
    {
		#region Construtores

		public AutorizacaoController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Métodos Globais

		public async Task<IActionResult> Aprovar()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                var codigo = Request.GetIntParam("Codigo");
                var repositorioAprovacao = new Repositorio.Embarcador.RegraAutorizacao.AprovacaoAlcada<TAprovacao, TRegra, TOrigem>(unitOfWork);
                var aprovacao = repositorioAprovacao.BuscarPorCodigo(codigo);

                if (aprovacao == null)
                    return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados.");

                if (aprovacao.Situacao != SituacaoAlcadaRegra.Pendente)
                    return new JsonpResult(false, "A situação da aprovação não permite alterações.");

                if (!aprovacao.IsPermitirAprovacaoOuReprovacao(this.Usuario.Codigo))
                    return new JsonpResult(false, "Aprovação não permite alterações.");

                Aprovar(aprovacao, unitOfWork);

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
                return new JsonpResult(false, "Ocorreu uma falha ao aprovar a regra.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AprovarMultiplasRegras()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                var codigo = Request.GetIntParam("Codigo");
                var repositorioAprovacao = new Repositorio.Embarcador.RegraAutorizacao.AprovacaoAlcada<TAprovacao, TRegra, TOrigem>(unitOfWork);
                var aprovacoesPendentes = repositorioAprovacao.BuscarPendentes(codigo, this.Usuario.Codigo);

                foreach (var aprovacao in aprovacoesPendentes)
                {
                    if (aprovacao.IsPermitirAprovacaoOuReprovacao(this.Usuario.Codigo))
                        Aprovar(aprovacao, unitOfWork);
                }

                var origem = (from aprovacao in aprovacoesPendentes select aprovacao.OrigemAprovacao).FirstOrDefault();
                var regras = (from aprovacao in aprovacoesPendentes select aprovacao.RegraAutorizacao).ToList();

                if (origem != null)
                    VerificarSituacaoOrigem(origem, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(new
                {
                    RegrasModificadas = aprovacoesPendentes.Count(),
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
                return new JsonpResult(false, "Ocorreu uma falha ao aprovar as regras.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public virtual IActionResult AprovarMultiplosItens()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                var codigosOrigem = ObterCodigosOrigensSelecionadas(unitOfWork);
                var aprovacoes = ObterAprovacoesPendentesPorOrigem(codigosOrigem, this.Usuario.Codigo, unitOfWork);

                foreach (var aprovacao in aprovacoes)
                {
                    if (aprovacao.IsPermitirAprovacaoOuReprovacao(this.Usuario.Codigo))
                        Aprovar(aprovacao, unitOfWork);
                }

                var origensRegrasAprovadas = (from aprovacao in aprovacoes select aprovacao.OrigemAprovacao).Distinct();

                foreach (var origem in origensRegrasAprovadas)
                {
                    VerificarSituacaoOrigem(origem, unitOfWork);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(new
                {
                    RegrasModificadas = aprovacoes.Count(),
                    Codigos = codigosOrigem
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
                return new JsonpResult(false, "Ocorreu uma falha ao aprovar as regras.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> VerificarExigeCustoExtraMultiplasCargas()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var codigosOrigem = ObterCodigosOrigensSelecionadas(unitOfWork);
                var aprovacoes = ObterAprovacoesPendentesPorOrigem(codigosOrigem, this.Usuario.Codigo, unitOfWork);

                var regras = (from aprovacao in aprovacoes select aprovacao.RegraAutorizacao).ToList();

                bool exigirInformarJustificativaCustoExtraCadastrado = false;
                foreach (var regra in regras)
                {
                    exigirInformarJustificativaCustoExtraCadastrado = (bool)regra.GetType().GetProperty("ExigirInformarJustificativaCustoExtraCadastrado").GetValue(regra);
                    if (exigirInformarJustificativaCustoExtraCadastrado) break;
                }

                if (exigirInformarJustificativaCustoExtraCadastrado)
                    return new JsonpResult(true);
                else 
                    return new JsonpResult(false, "");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> VerificarExigeCustoExtraMultiplosItens()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var codigo = Request.GetIntParam("Codigo");
                var repositorioAprovacao = new Repositorio.Embarcador.RegraAutorizacao.AprovacaoAlcada<TAprovacao, TRegra, TOrigem>(unitOfWork);
                var aprovacoesPendentes = repositorioAprovacao.BuscarPendentes(codigo, this.Usuario.Codigo);

                var regras = (from aprovacao in aprovacoesPendentes select aprovacao.RegraAutorizacao).ToList();

                bool exigirInformarJustificativaCustoExtraCadastrado = false;
                foreach (var regra in regras)
                {
                    exigirInformarJustificativaCustoExtraCadastrado = (bool)regra.GetType().GetProperty("ExigirInformarJustificativaCustoExtraCadastrado").GetValue(regra);
                    if (exigirInformarJustificativaCustoExtraCadastrado) break;
                }

                if (exigirInformarJustificativaCustoExtraCadastrado)
                    return new JsonpResult(true);
                else
                    return new JsonpResult(false, "");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "");
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
                int codigoOrigem = Request.GetIntParam("Codigo");
                Repositorio.RepositorioBase<TOrigem> repositorioOrigem = new Repositorio.RepositorioBase<TOrigem>(unitOfWork);
                TOrigem origem = repositorioOrigem.BuscarPorCodigo(codigoOrigem, auditavel: false);

                if (origem == null)
                    return new JsonpResult(false, true, "Ocorreu uma falha ao buscar os dados.");

                int codigoUsuario = Request.GetIntParam("UsuarioDelegado");
                Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(unitOfWork);
                Dominio.Entidades.Usuario usuarioDelegado = repositorioUsuario.BuscarPorCodigo(codigoUsuario);

                if (usuarioDelegado == null)
                    return new JsonpResult(false, true, "Ocorreu uma falha ao buscar o usuário.");

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador(unitOfWork);

                if ((configuracaoEmbarcador?.NaoExibirOpcaoParaDelegar ?? false) || !IsPermitirDelegar(origem))
                    return new JsonpResult(false, true, "Operação não permitida.");

                if ((configuracaoEmbarcador?.NaoPermitirDelegarAoUsuarioLogado ?? false) && Usuario.Codigo == usuarioDelegado.Codigo)
                    return new JsonpResult(false, true, "Não é permitido delegar para você mesmo.");

                Repositorio.Embarcador.RegraAutorizacao.AprovacaoAlcada<TAprovacao, TRegra, TOrigem> repositorioAprovacao = new Repositorio.Embarcador.RegraAutorizacao.AprovacaoAlcada<TAprovacao, TRegra, TOrigem>(unitOfWork);

                if (repositorioAprovacao.BuscarDesbloqueada(codigoOrigem, codigoUsuario).Count > 0)
                    return new JsonpResult(false, true, "O usuário selecionado já é o responsável.");

                TAprovacao aprovacao = new TAprovacao();

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

                Repositorio.Embarcador.RegraAutorizacao.AprovacaoAlcada<TAprovacao, TRegra, TOrigem> repositorioAprovacao = new Repositorio.Embarcador.RegraAutorizacao.AprovacaoAlcada<TAprovacao, TRegra, TOrigem>(unitOfWork);
                Repositorio.RepositorioBase<TOrigem> repositorioOrigem = new Repositorio.RepositorioBase<TOrigem>(unitOfWork);
                List<int> codigosOrigem = ObterCodigosOrigensSelecionadas(unitOfWork);

                unitOfWork.Start();

                foreach (int codigoOrigem in codigosOrigem)
                {
                    TOrigem origem = repositorioOrigem.BuscarPorCodigo(codigoOrigem, auditavel: false);

                    if (
                        (origem != null) &&
                        IsPermitirDelegar(origem) &&
                        (repositorioAprovacao.BuscarDesbloqueada(codigoOrigem, codigoUsuario).Count == 0)
                    )
                    {
                        TAprovacao aprovacao = new TAprovacao();

                        aprovacao.DataCriacao = DateTime.Now;
                        aprovacao.Delegada = true;
                        aprovacao.OrigemAprovacao = origem;
                        aprovacao.Situacao = SituacaoAlcadaRegra.Pendente;
                        aprovacao.Usuario = usuarioDelegado;

                        repositorioAprovacao.Inserir(aprovacao);
                    }
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
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
                var grid = ObterGridExportarPesquisa();

                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

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
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                int codigoUsuario = Request.GetIntParam("Usuario");

                var grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoUsuario", false);
                grid.AdicionarCabecalho("PodeAprovar", false);
                grid.AdicionarCabecalho("ExigirInformarJustificativaCustoExtraCadastrado", false);
                grid.AdicionarCabecalho("Regra", "Regra", 30, Models.Grid.Align.left, false);

                if (codigoUsuario > 0)
                    grid.AdicionarCabecalho("Usuario", false);
                else
                    grid.AdicionarCabecalho("Usuário", "Usuario", 15, Models.Grid.Align.left, false);

                grid.AdicionarCabecalho("Situação", "Situacao", 10, Models.Grid.Align.left, false);

                var repositorioAprovacao = new Repositorio.Embarcador.RegraAutorizacao.AprovacaoAlcada<TAprovacao, TRegra, TOrigem>(unitOfWork);
                var aprovacoes = repositorioAprovacao.BuscarDesbloqueada(codigo, codigoUsuario);
                var lista = (
                    from aprovacao in aprovacoes
                    select new
                    {
                        aprovacao.Codigo,
                        CodigoUsuario = aprovacao.Usuario?.Codigo ?? 0,
                        Regra = aprovacao.Descricao,
                        Situacao = aprovacao.Situacao.ObterDescricao(),
                        Usuario = aprovacao.Usuario?.Nome + (aprovacao.Data.HasValue ? " - " + aprovacao.Data.Value.ToString("dd/MM/yyyy HH:mm") : ""),
                        PodeAprovar = aprovacao.IsPermitirAprovacaoOuReprovacao(this.Usuario.Codigo),
                        DT_RowColor = aprovacao.Situacao.ObterCorGrid(),
                        ExigirInformarJustificativaCustoExtraCadastrado = aprovacao.RegraAutorizacao?.GetType()?.GetProperty("ExigirInformarJustificativaCustoExtraCadastrado")?.GetValue(aprovacao.RegraAutorizacao) ?? false,
                    }
                ).ToList();

                grid.setarQuantidadeTotal(lista.Count());
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

        public async Task<IActionResult> Reprovar()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                var repositorioAprovacao = new Repositorio.Embarcador.RegraAutorizacao.AprovacaoAlcada<TAprovacao, TRegra, TOrigem>(unitOfWork);
                TAprovacao aprovacao = repositorioAprovacao.BuscarPorCodigo(codigo);

                if (!aprovacao.IsPermitirAprovacaoOuReprovacao(this.Usuario.Codigo))
                    return new JsonpResult(false, "Aprovação não permite alterações.");

                unitOfWork.Start();

                PreencherDadosRejeicaoAprovacao(aprovacao, unitOfWork);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, aprovacao.OrigemAprovacao, $"{Auditado.Usuario.Nome} reprovou a regra. Motivo: {aprovacao.Motivo}", unitOfWork);

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

        public async Task<IActionResult> ReprovarMultiplosItens()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                var repositorioAprovacao = new Repositorio.Embarcador.RegraAutorizacao.AprovacaoAlcada<TAprovacao, TRegra, TOrigem>(unitOfWork);
                var codigosOrigem = ObterCodigosOrigensSelecionadas(unitOfWork);
                var aprovacoes = ObterAprovacoesPendentesPorOrigem(codigosOrigem, this.Usuario.Codigo, unitOfWork);

                foreach (var aprovacao in aprovacoes)
                {
                    if (aprovacao.IsPermitirAprovacaoOuReprovacao(this.Usuario.Codigo))
                    {
                        PreencherDadosRejeicaoAprovacao(aprovacao, unitOfWork);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, aprovacao.OrigemAprovacao, null, $"{Auditado.Usuario.Nome} reprovou a regra. Motivo: {aprovacao.Motivo}", unitOfWork);

                        repositorioAprovacao.Atualizar(aprovacao);
                    }
                }

                var origensRegrasReprovadas = (from aprovacao in aprovacoes select aprovacao.OrigemAprovacao).Distinct();

                foreach (var origem in origensRegrasReprovadas)
                {
                    VerificarSituacaoOrigem(origem, unitOfWork);
                }

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
                return new JsonpResult(false, "Ocorreu uma falha ao reprovar múltiplos registros.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Globais Abstratos

        public abstract IActionResult BuscarPorCodigo();

        #endregion

        #region Métodos Privados

        private void Aprovar(TAprovacao aprovacaoAlcada, Repositorio.UnitOfWork unitOfWork)
        {
            var repositorioAprovacao = new Repositorio.Embarcador.RegraAutorizacao.AprovacaoAlcada<TAprovacao, TRegra, TOrigem>(unitOfWork);

            aprovacaoAlcada.Data = DateTime.Now;
            aprovacaoAlcada.Situacao = SituacaoAlcadaRegra.Aprovada;

            repositorioAprovacao.Atualizar(aprovacaoAlcada);

            Servicos.Auditoria.Auditoria.Auditar(Auditado, aprovacaoAlcada.OrigemAprovacao, $"{Auditado.Usuario.Nome} aprovou a regra", unitOfWork);
        }

        private List<TAprovacao> ObterAprovacoesPendentesPorOrigem(List<int> codigosOrigem, int codigoUsuario, Repositorio.UnitOfWork unitOfWork)
        {
            var repositorioAprovacao = new Repositorio.Embarcador.RegraAutorizacao.AprovacaoAlcada<TAprovacao, TRegra, TOrigem>(unitOfWork);
            var aprovacoes = new List<TAprovacao>();

            foreach (var codigoOrigem in codigosOrigem)
            {
                var regras = repositorioAprovacao.BuscarPendentes(codigoOrigem, codigoUsuario);

                aprovacoes.AddRange(regras);
            }

            return aprovacoes;
        }

        #endregion

        #region Métodos Protegidos

        protected Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ObterConfiguracaoEmbarcador(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            return repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
        }

        protected virtual Models.Grid.Grid ObterGridExportarPesquisa()
        {
            return ObterGridPesquisa();
        }

        protected SituacaoRegrasAutorizacao ObterSituacaoRegrasAutorizacao(int codigoOrigem, Repositorio.UnitOfWork unitOfWork)
        {
            var repositorioAprovacao = new Repositorio.Embarcador.RegraAutorizacao.AprovacaoAlcada<TAprovacao, TRegra, TOrigem>(unitOfWork);
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

        protected virtual void PreencherDadosRejeicaoAprovacao(TAprovacao aprovacao, Repositorio.UnitOfWork unitOfWork)
        {
            string motivo = Request.GetStringParam("Motivo");

            if (string.IsNullOrWhiteSpace(motivo))
                throw new ControllerException("Motivo é obrigatório.");

            aprovacao.Data = DateTime.Now;
            aprovacao.Situacao = SituacaoAlcadaRegra.Rejeitada;
            aprovacao.Motivo = motivo;
        }

        #endregion

        #region Métodos Protegidos Abstratos

        protected abstract bool IsPermitirDelegar(TOrigem origem);

        protected abstract List<int> ObterCodigosOrigensSelecionadas(Repositorio.UnitOfWork unitOfWork);

        protected abstract Models.Grid.Grid ObterGridPesquisa();

        protected abstract void VerificarSituacaoOrigem(TOrigem origem, Repositorio.UnitOfWork unitOfWork);

        #endregion
    }

}