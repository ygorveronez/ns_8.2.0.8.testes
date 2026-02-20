using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.RegraAutorizacao
{
    public abstract class RegraAutorizacaoController<TRegra> : BaseController where TRegra : Dominio.Entidades.Embarcador.RegraAutorizacao.RegraAutorizacao
    {
		#region Construtores

		public RegraAutorizacaoController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Métodos Globais

		public async Task<IActionResult> BuscarPorCodigo()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var codigo = Request.GetIntParam("Codigo");
                var repositorio = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<TRegra>(unitOfWork);
                var regraAutorizacao = repositorio.BuscarPorCodigo(codigo);

                if (regraAutorizacao == null)
                    return new JsonpResult(false, "Não foi possível encontrar o registro.");

                return ObterRegraDinamicaRetornarPesquisaPorCodigo(regraAutorizacao);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao obter os dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                var codigo = Request.GetIntParam("Codigo");
                var repositorio = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacao<TRegra>(unitOfWork);
                var regraAutorizacao = repositorio.BuscarPorCodigo(codigo);

                if (regraAutorizacao == null)
                    return new JsonpResult(false, "Não foi possível encontrar o registro.");

                regraAutorizacao.LimparAprovadores();
                regraAutorizacao.LimparAlcadas();

                repositorio.Deletar(regraAutorizacao);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);

                if (ExcessaoPorPossuirDependeciasNoBanco(excecao))
                    return new JsonpResult(false, true, "Já existem informações vinculadas à regra.");

                return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
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
                var grid = ObterGridPesquisa();

                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

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

        #endregion

        #region Métodos Globais Abstratos

        public abstract IActionResult Adicionar();

        public abstract IActionResult Atualizar();

        #endregion

        #region Métodos Privados

        private List<TAlcada> ObterAlcadas<TAlcada, TPropriedade>(TRegra regraAutorizacao, string nomeParametroRegras, Action<dynamic, TAlcada> setarPropriedadeAlcada)
            where TAlcada : Dominio.Entidades.Embarcador.RegraAutorizacao.Alcada<TRegra, TPropriedade>, new()
        {
            string parametroRegras = Request.GetNullableStringParam(nomeParametroRegras);

            if (string.IsNullOrWhiteSpace(parametroRegras))
                return new List<TAlcada>();

            List<Dominio.ObjetosDeValor.Embarcador.Alcada.RegrasPorTipo> regrasPorTipo = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Alcada.RegrasPorTipo>>(parametroRegras);

            if (regrasPorTipo == null)
                throw new ControllerException("Erro ao converter os dados recebidos.");

            List<TAlcada> listaAlcadas = new List<TAlcada>();

            foreach (Dominio.ObjetosDeValor.Embarcador.Alcada.RegrasPorTipo regra in regrasPorTipo)
            {
                TAlcada alcada = new TAlcada();

                alcada.Codigo = ((string)Convert.ToString(regra.Codigo)).ToInt();
                alcada.Condicao = regra.Condicao;
                alcada.Juncao = regra.Juncao;
                alcada.Ordem = regra.Ordem;
                alcada.RegrasAutorizacao = regraAutorizacao;

                if (alcada.IsEntidadePropriedadeAlcada())
                    setarPropriedadeAlcada(regra.Entidade.Codigo, alcada);
                else
                    setarPropriedadeAlcada(regra.Valor, alcada);

                listaAlcadas.Add(alcada);
            }

            return listaAlcadas;
        }

        #endregion

        #region Métodos Protegidos

        protected void AdicionarAlcadas<TAlcada, TPropriedade>(Repositorio.UnitOfWork unitOfWork, TRegra regraAutorizacao, string nomeParametroRegras, Action<dynamic, TAlcada> setarPropriedadeAlcada)
            where TAlcada : Dominio.Entidades.Embarcador.RegraAutorizacao.Alcada<TRegra, TPropriedade>, new()
        {
            var repositorio = new Repositorio.Embarcador.RegraAutorizacao.Alcada<TAlcada, TRegra, TPropriedade>(unitOfWork);
            var listaAlcadasAdicionar = ObterAlcadas<TAlcada, TPropriedade>(regraAutorizacao, nomeParametroRegras, setarPropriedadeAlcada);

            foreach (var alcadaAdicionar in listaAlcadasAdicionar)
            {
                repositorio.Inserir(alcadaAdicionar, Auditado);
            }
        }

        protected void AtualizarAlcadas<TAlcada, TPropriedade>(Repositorio.UnitOfWork unitOfWork, TRegra regraAutorizacao, IList<TAlcada> listaAlcadas, string nomeParametroRegras, Action<dynamic, TAlcada> setarPropriedadeAlcada)
            where TAlcada : Dominio.Entidades.Embarcador.RegraAutorizacao.Alcada<TRegra, TPropriedade>, new()
        {
            var repositorio = new Repositorio.Embarcador.RegraAutorizacao.Alcada<TAlcada, TRegra, TPropriedade>(unitOfWork);
            var listaAlcadasRequisicao = ObterAlcadas<TAlcada, TPropriedade>(regraAutorizacao, nomeParametroRegras, setarPropriedadeAlcada);
            var listaAlcadasAdicionar = (from alcadaRequisicao in listaAlcadasRequisicao where alcadaRequisicao.Codigo == 0 select alcadaRequisicao);

            if (listaAlcadas?.Count() > 0)
            {
                var listaAlcadasRemover = new List<TAlcada>();

                foreach (var alcada in listaAlcadas)
                {
                    var alcadaAtualizar = (from alcadaRequisicao in listaAlcadasRequisicao where alcadaRequisicao.Codigo == alcada.Codigo select alcadaRequisicao).FirstOrDefault();

                    if (alcadaAtualizar == null)
                        listaAlcadasRemover.Add(alcada);
                    else
                    {
                        alcada.Initialize();

                        alcada.Condicao = alcadaAtualizar.Condicao;
                        alcada.Juncao = alcadaAtualizar.Juncao;
                        alcada.Ordem = alcadaAtualizar.Ordem;

                        setarPropriedadeAlcada(alcadaAtualizar.ObterValorPropriedadeAlcada(), alcada);

                        repositorio.Atualizar(alcada, Auditado);
                    }
                }

                foreach (var alcadaRemover in listaAlcadasRemover)
                {
                    repositorio.Deletar(alcadaRemover, Auditado);
                }
            }

            foreach (var alcadaAdicionar in listaAlcadasAdicionar)
            {
                repositorio.Inserir(alcadaAdicionar, Auditado);
            }
        }

        protected List<Dominio.Entidades.Usuario> ObterAprovadores(TRegra regraAutorizacao, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Usuario> listaAprovadores = new List<Dominio.Entidades.Usuario>();

            if (!string.IsNullOrWhiteSpace(Request.Params("Aprovadores")))
            {
                List<Dominio.ObjetosDeValor.Embarcador.Alcada.Aprovadores> aprovadores = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Alcada.Aprovadores>>(Request.Params("Aprovadores"));
                List<int> codigosUsuarios = (from aprovador in aprovadores select aprovador.Codigo).ToList();

                if (codigosUsuarios.Count > 0)
                    listaAprovadores = new Repositorio.Usuario(unitOfWork).BuscarUsuariosPorCodigos(codigosUsuarios.ToArray(), null);
            }

            int numeroAprovadores = listaAprovadores.Count();

            if (numeroAprovadores < regraAutorizacao.NumeroAprovadores)
                throw new ControllerException($"O número de aprovadores selecionados ({numeroAprovadores}) deve ser maior ou igual a {regraAutorizacao.NumeroAprovadores}");

            return listaAprovadores;
        }

        protected virtual Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Vigência", "Vigencia", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Status", "DescricaoAtivo", 15, Models.Grid.Align.left, true);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta((propriedadeOrdenar) => { return grid.header[grid.indiceColunaOrdena].data == "DescricaoAtivo" ? "Ativo" : grid.header[grid.indiceColunaOrdena].data; });
                Dominio.Entidades.Usuario usuario = null;
                int codigoAprovador = Request.GetIntParam("Aprovador");

                if (codigoAprovador > 0)
                {
                    Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(unitOfWork);

                    usuario = repositorioUsuario.BuscarPorCodigo(codigoAprovador);
                }

                Dominio.ObjetosDeValor.Embarcador.RegraAutorizacao.FiltroPesquisaRegraAutorizacaoPadrao filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.RegraAutorizacao.FiltroPesquisaRegraAutorizacaoPadrao()
                {
                    Aprovador = usuario,
                    DataInicial = Request.GetNullableDateTimeParam("DataInicio"),
                    DataLimite = Request.GetNullableDateTimeParam("DataFinal"),
                    Descricao = Request.Params("Descricao"),
                    Situacao = Request.GetEnumParam("Status", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                };

                Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacaoPadrao<TRegra> repositorio = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacaoPadrao<TRegra>(unitOfWork);
                int totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);
                List<TRegra> listaRegras = totalRegistros > 0 ? repositorio.Consultar(filtrosPesquisa, parametrosConsulta) : new List<TRegra>();

                var lista = (
                    from regra in listaRegras
                    select new
                    {
                        regra.Codigo,
                        regra.Descricao,
                        regra.DescricaoAtivo,
                        Vigencia = regra.Vigencia?.ToString("dd/MM/yyyy"),
                    }
                ).ToList();

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        protected List<Dominio.Entidades.Setor> ObterSetores(TRegra regraAutorizacao, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Setor> listaSetores = new List<Dominio.Entidades.Setor>();

            if (!string.IsNullOrWhiteSpace(Request.Params("Setores")))
            {
                List<Dominio.ObjetosDeValor.Embarcador.Alcada.Setores> setores = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Alcada.Setores>>(Request.Params("Setores"));
                List<int> codigosSetores = (from setor in setores select setor.Codigo).ToList();

                if (codigosSetores.Count > 0)
                    listaSetores = new Repositorio.Setor(unitOfWork).BuscarPorCodigos(codigosSetores);
            }

            int numeroUsuariosPorSetores = new Repositorio.Usuario(unitOfWork).ContarUsuariosPorSetores(listaSetores.Select(setor => setor.Codigo).ToList());

            if (numeroUsuariosPorSetores < regraAutorizacao.NumeroAprovadores)
                throw new ControllerException($"O número de aprovadores por setores selecionados ({numeroUsuariosPorSetores}) deve ser maior ou igual a {regraAutorizacao.NumeroAprovadores}");

            return listaSetores;
        }

        protected Dominio.ObjetosDeValor.Embarcador.Alcada.RegrasPorTipo ObterRegraPorTipo<TAlcada, TPropriedade>(TAlcada alcada)
            where TAlcada : Dominio.Entidades.Embarcador.RegraAutorizacao.Alcada<TRegra, TPropriedade>
        {
            Dominio.ObjetosDeValor.Embarcador.Alcada.Entidade entidadePropriedadeAlcada = null;
            dynamic valorPropriedadeAlcada = null;

            if (alcada.IsEntidadePropriedadeAlcada())
            {
                entidadePropriedadeAlcada = new Dominio.ObjetosDeValor.Embarcador.Alcada.Entidade
                {
                    Codigo = alcada.ObterValorPropriedadeAlcada(),
                    Descricao = alcada.Descricao
                };
            }
            else
                valorPropriedadeAlcada = alcada.Descricao;

            return new Dominio.ObjetosDeValor.Embarcador.Alcada.RegrasPorTipo()
            {
                Codigo = alcada.Codigo,
                Ordem = alcada.Ordem,
                Juncao = alcada.Juncao,
                Condicao = alcada.Condicao,
                Entidade = entidadePropriedadeAlcada,
                Valor = valorPropriedadeAlcada,
            };
        }

        protected virtual void PreencherAprovadores(TRegra regraAutorizacao, Repositorio.UnitOfWork unitOfWork)
        {
            regraAutorizacao.Aprovadores = ObterAprovadores(regraAutorizacao, unitOfWork);
        }

        protected void PreencherRegra(TRegra regraAutorizacao, Repositorio.UnitOfWork unitOfWork, Action<TRegra> PreencherDadosAdicionais)
        {
            regraAutorizacao.Descricao = Request.GetStringParam("Descricao");
            regraAutorizacao.Observacoes = Request.GetStringParam("Observacoes");
            regraAutorizacao.PrioridadeAprovacao = Request.GetIntParam("PrioridadeAprovacao");
            regraAutorizacao.Vigencia = Request.GetNullableDateTimeParam("Vigencia");
            regraAutorizacao.NumeroAprovadores = Request.GetIntParam("NumeroAprovadores");
            regraAutorizacao.Ativo = Request.GetBoolParam("Status");

            if (string.IsNullOrWhiteSpace(regraAutorizacao.Descricao))
                throw new ControllerException("Descrição é obrigatória.");

            PreencherDadosAdicionais(regraAutorizacao);
            PreencherAprovadores(regraAutorizacao, unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Abstratos

        protected abstract JsonpResult ObterRegraDinamicaRetornarPesquisaPorCodigo(TRegra regra);

        #endregion
    }
}