using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/TermoQuitacao")]
    public class TermoQuitacaoController : BaseController
    {
		#region Construtores

		public TermoQuitacaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, true, "O termo de quitação não pode ser adicionado pelo acesso de transportador");

                unitOfWork.Start();

                Dominio.Entidades.Empresa transportador = ObterTransportador(unitOfWork);
                Repositorio.Embarcador.Logistica.TermoQuitacao repositorioTermoQuitacao = new Repositorio.Embarcador.Logistica.TermoQuitacao(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.TermoQuitacao.TermoQuitacao termoQuitacao = new Dominio.Entidades.Embarcador.Logistica.TermoQuitacao.TermoQuitacao()
                {
                    DataBase = Request.GetDateTimeParam("DataBase"),
                    DataCriacao = DateTime.Now,
                    Numero = repositorioTermoQuitacao.BuscarProximoNumero(),
                    Observacao = Request.GetNullableStringParam("Observacao"),
                    Situacao = SituacaoTermoQuitacao.AguardandoAceiteTransportador,
                    Transportador = transportador
                };

                repositorioTermoQuitacao.Inserir(termoQuitacao, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(new
                {
                    termoQuitacao.Codigo
                });
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar o termo de quitação.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Aprovar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, true, "O termo de quitação deve ser aprovado pelo acesso de transportador");

                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Logistica.TermoQuitacao repositorioTermoQuitacao = new Repositorio.Embarcador.Logistica.TermoQuitacao(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.TermoQuitacao.TermoQuitacao termoQuitacao = repositorioTermoQuitacao.BuscarPorCodigo(codigo);

                if (termoQuitacao == null)
                    throw new ControllerException("Não foi possível encontrar o registro.");

                if ((termoQuitacao.Situacao != SituacaoTermoQuitacao.AguardandoAceiteTransportador) && (termoQuitacao.Situacao != SituacaoTermoQuitacao.AprovacaoRejeitada))
                    throw new ControllerException("A situação atual do termo de quitação não permite a aprovação.");

                Servicos.Embarcador.Logistica.TermoQuitacao ServicosTermoQuitacao = new Servicos.Embarcador.Logistica.TermoQuitacao(unitOfWork);

                termoQuitacao.Initialize();
                termoQuitacao.ObservacaoTransportador = Request.GetNullableStringParam("Observacao");

                ServicosTermoQuitacao.CriarAprovacao(termoQuitacao, TipoServicoMultisoftware);
                repositorioTermoQuitacao.Atualizar(termoQuitacao);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, termoQuitacao, termoQuitacao.GetChanges(), "Aprovado pelo transportador", unitOfWork);

                if (termoQuitacao.Situacao == SituacaoTermoQuitacao.Finalizado)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, termoQuitacao, $"Termo de quitação finalizado", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
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
                return new JsonpResult(false, "Ocorreu uma falha ao aprovar o termo de quitação.");
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
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, true, "O termo de quitação não pode ser atualizado pelo acesso de transportador");

                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Logistica.TermoQuitacao repositorioTermoQuitacao = new Repositorio.Embarcador.Logistica.TermoQuitacao(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.TermoQuitacao.TermoQuitacao termoQuitacao = repositorioTermoQuitacao.BuscarPorCodigo(codigo);

                if (termoQuitacao == null)
                    throw new ControllerException("Não foi possível encontrar o registro.");

                if (termoQuitacao.Situacao != SituacaoTermoQuitacao.AceiteTransportadorRejeitado)
                    throw new ControllerException("A situação atual do termo de quitação não permite a atualização.");

                termoQuitacao.Initialize();
                termoQuitacao.DataBase = Request.GetDateTimeParam("DataBase");
                termoQuitacao.Observacao = Request.GetNullableStringParam("Observacao");
                termoQuitacao.Situacao = SituacaoTermoQuitacao.AguardandoAceiteTransportador;

                repositorioTermoQuitacao.Atualizar(termoQuitacao, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar o termo de quitação.");
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
                Repositorio.Embarcador.Logistica.TermoQuitacao repositorio = new Repositorio.Embarcador.Logistica.TermoQuitacao(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.TermoQuitacao.TermoQuitacao termoQuitacao = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (termoQuitacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    termoQuitacao.Codigo,
                    termoQuitacao.Situacao,
                    DadosTermoQuitacao = new
                    {
                        termoQuitacao.Codigo,
                        DataBase = termoQuitacao.DataBase.ToString("dd/MM/yyyy"),
                        termoQuitacao.Numero,
                        Observacao = termoQuitacao.Observacao ?? "",
                        Transportador = new { termoQuitacao.Transportador.Codigo, termoQuitacao.Transportador.Descricao }
                    },
                    DadosAceiteTransportador = new
                    {
                        termoQuitacao.Codigo,
                        Observacao = termoQuitacao.ObservacaoTransportador ?? "",
                        ObservacaoAnterior = termoQuitacao.ObservacaoTransportador ?? ""
                    },
                    Anexos = (
                        from o in termoQuitacao.Anexos
                        select new
                        {
                            o.Codigo,
                            o.Descricao,
                            o.NomeArquivo
                        }
                    ).ToList(),
                    AnexosTransportador = (
                        from o in termoQuitacao.AnexosTransportador
                        select new
                        {
                            o.Codigo,
                            o.Descricao,
                            o.NomeArquivo
                        }
                    ).ToList(),
                    ResumoAprovacao = ObterResumoAprovacao(unitOfWork, termoQuitacao)
                });
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

        public async Task<IActionResult> DetalhesAutorizacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Logistica.AlcadasTermoQuitacao.AprovacaoAlcadaTermoQuitacao repositorioAprovacao = new Repositorio.Embarcador.Logistica.AlcadasTermoQuitacao.AprovacaoAlcadaTermoQuitacao(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.AlcadasTermoQuitacao.AprovacaoAlcadaTermoQuitacao autorizacao = repositorioAprovacao.BuscarPorCodigo(codigo);

                if (autorizacao == null)
                    return new JsonpResult(false, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    autorizacao.Codigo,
                    Regra = autorizacao.Descricao,
                    Situacao = autorizacao.Situacao.ObterDescricao(),
                    Usuario = autorizacao.Usuario?.Nome ?? string.Empty,
                    PodeAprovar = autorizacao.IsPermitirAprovacaoOuReprovacao(this.Usuario.Codigo),
                    Data = autorizacao.Data.HasValue ? autorizacao.Data.Value.ToString("dd/MM/yyyy") : string.Empty,
                    Motivo = string.IsNullOrWhiteSpace(autorizacao.Motivo) ? string.Empty : autorizacao.Motivo,
                });
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

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            try
            {
                Models.Grid.Grid grid = ObterGridPesquisa();
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

        public async Task<IActionResult> PesquisaAutorizacoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Regra", false);
                grid.AdicionarCabecalho("Data", false);
                grid.AdicionarCabecalho("Motivo", false);
                grid.AdicionarCabecalho("Usuário", "Usuario", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Prioridade", "PrioridadeAprovacao", 5, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Situação", "Situacao", 5, Models.Grid.Align.center, false);

                int codigo = Request.GetIntParam("Codigo");
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                Repositorio.Embarcador.Logistica.AlcadasTermoQuitacao.AprovacaoAlcadaTermoQuitacao repositorioAprovacao = new Repositorio.Embarcador.Logistica.AlcadasTermoQuitacao.AprovacaoAlcadaTermoQuitacao(unitOfWork);
                int totalRegistros = repositorioAprovacao.ContarAutorizacoes(codigo);
                List<Dominio.Entidades.Embarcador.Logistica.AlcadasTermoQuitacao.AprovacaoAlcadaTermoQuitacao> listaAutorizacao = (totalRegistros > 0) ? repositorioAprovacao.ConsultarAutorizacoes(codigo, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Logistica.AlcadasTermoQuitacao.AprovacaoAlcadaTermoQuitacao>();

                var lista = (
                    from autorizacao in listaAutorizacao
                    select new
                    {
                        autorizacao.Codigo,
                        PrioridadeAprovacao = autorizacao.RegraAutorizacao?.PrioridadeAprovacao ?? 0,
                        Situacao = autorizacao.Situacao.ObterDescricao(),
                        Usuario = autorizacao.Usuario?.Nome,
                        Regra = autorizacao.Descricao,
                        Data = autorizacao.Data.HasValue ? autorizacao.Data.ToString() : string.Empty,
                        Motivo = string.IsNullOrWhiteSpace(autorizacao.Motivo) ? string.Empty : autorizacao.Motivo,
                        DT_RowColor = autorizacao.ObterCorGrid()
                    }
                ).ToList();

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

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

        public async Task<IActionResult> ReprocessarRegras()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Logistica.TermoQuitacao repositorio = new Repositorio.Embarcador.Logistica.TermoQuitacao(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.TermoQuitacao.TermoQuitacao termoQuitacao = repositorio.BuscarPorCodigo(codigo);

                if (termoQuitacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (termoQuitacao.Situacao != SituacaoTermoQuitacao.SemRegraAprovacao)
                    return new JsonpResult(false, true, "A situação não permite esta operação.");

                Servicos.Embarcador.Logistica.TermoQuitacao servicoTermoQuitacao = new Servicos.Embarcador.Logistica.TermoQuitacao(unitOfWork);

                servicoTermoQuitacao.CriarAprovacao(termoQuitacao, TipoServicoMultisoftware);
                repositorio.Atualizar(termoQuitacao);

                unitOfWork.CommitChanges();

                return new JsonpResult(termoQuitacao.Situacao != SituacaoTermoQuitacao.SemRegraAprovacao);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, false, "Ocorreu uma falha ao reprocessar as regras.");
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
                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, true, "O termo de quitação deve ser reprovado pelo acesso de transportador");

                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Logistica.TermoQuitacao repositorioTermoQuitacao = new Repositorio.Embarcador.Logistica.TermoQuitacao(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.TermoQuitacao.TermoQuitacao termoQuitacao = repositorioTermoQuitacao.BuscarPorCodigo(codigo);

                if (termoQuitacao == null)
                    throw new ControllerException("Não foi possível encontrar o registro.");

                if ((termoQuitacao.Situacao != SituacaoTermoQuitacao.AguardandoAceiteTransportador) && (termoQuitacao.Situacao != SituacaoTermoQuitacao.AprovacaoRejeitada))
                    throw new ControllerException("A situação atual do termo de quitação não permite a reprovação.");

                termoQuitacao.Initialize();
                termoQuitacao.ObservacaoTransportador = Request.GetNullableStringParam("Observacao");
                termoQuitacao.Situacao = SituacaoTermoQuitacao.AceiteTransportadorRejeitado;

                repositorioTermoQuitacao.Atualizar(termoQuitacao);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, termoQuitacao, termoQuitacao.GetChanges(), "Reprovado pelo transportador", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao reprovar o termo de quitação.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaTermoQuitacao ObterFiltrosPesquisa()
        {
            Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaTermoQuitacao filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaTermoQuitacao()
            {
                CodigoTransportador = Request.GetIntParam("Transportador"),
                DataBaseInicial = Request.GetNullableDateTimeParam("DataBaseInicial"),
                DataBaseLimite = Request.GetNullableDateTimeParam("DataBaseLimite"),
                Numero = Request.GetIntParam("Numero"),
                Situacao = Request.GetNullableEnumParam<SituacaoTermoQuitacao>("Situacao")
            };

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                filtrosPesquisa.CodigoTransportador = this.Usuario.Empresa.Codigo;

            return filtrosPesquisa;
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Número", "Numero", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Data Base", "DataBase", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Transportador", "Transportador", 40, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "Situacao", 20, Models.Grid.Align.center, true);

                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaTermoQuitacao filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Logistica.TermoQuitacao repositorio = new Repositorio.Embarcador.Logistica.TermoQuitacao(unitOfWork);
                int totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Logistica.TermoQuitacao.TermoQuitacao> listaTermoQuitacao = (totalRegistros > 0) ? repositorio.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Logistica.TermoQuitacao.TermoQuitacao>();

                var listaTermoQuitacaoRetornar = (
                    from termoQuitacao in listaTermoQuitacao
                    select new
                    {
                        termoQuitacao.Codigo,
                        termoQuitacao.Numero,
                        DataBase = termoQuitacao.DataBase.ToString("dd/MM/yyyy"),
                        Situacao = termoQuitacao.Situacao.ObterDescricao(),
                        Transportador = termoQuitacao.Transportador.Descricao
                    }
                ).ToList();

                grid.AdicionaRows(listaTermoQuitacaoRetornar);
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

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "Transportador")
                return "Transportador.RazaoSocial";

            return propriedadeOrdenar;
        }

        private dynamic ObterResumoAprovacao(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Logistica.TermoQuitacao.TermoQuitacao termoQuitacao)
        {
            Repositorio.Embarcador.Logistica.AlcadasTermoQuitacao.AprovacaoAlcadaTermoQuitacao repositorioAprovacao = new Repositorio.Embarcador.Logistica.AlcadasTermoQuitacao.AprovacaoAlcadaTermoQuitacao(unitOfWork);
            int aprovacoes = repositorioAprovacao.ContarAprovacoes(termoQuitacao.Codigo);
            int aprovacoesNecessarias = repositorioAprovacao.ContarAprovacoesNecessarias(termoQuitacao.Codigo);
            int reprovacoes = repositorioAprovacao.ContarReprovacoes(termoQuitacao.Codigo);

            return new
            {
                termoQuitacao.Codigo,
                AprovacoesNecessarias = aprovacoesNecessarias,
                Aprovacoes = aprovacoes,
                Reprovacoes = reprovacoes,
                Situacao = termoQuitacao.Situacao.ObterDescricao(),
            };
        }

        private Dominio.Entidades.Empresa ObterTransportador(Repositorio.UnitOfWork unitOfWork)
        {
            int codigoTransportador = Request.GetIntParam("Transportador");

            if (codigoTransportador == 0)
                throw new ControllerException("O transportador deve ser informado.");

            Repositorio.Empresa repositorioTransportador = new Repositorio.Empresa(unitOfWork);
            Dominio.Entidades.Empresa transportador = repositorioTransportador.BuscarPorCodigo(codigoTransportador) ?? throw new ControllerException("Não foi possível encontrar o transportador informado.");

            return transportador;
        }

        #endregion
    }
}
