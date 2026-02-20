using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;


namespace SGT.WebAdmin.Controllers.Notificacoes
{
    [CustomAuthorize("Notificacoes/AlertaEmail")]
    public class AlertaEmailController : BaseController
    {
		#region Construtores

		public AlertaEmailController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Notificacoes.AlertaEmail repositorioalertaEmail = new Repositorio.Embarcador.Notificacoes.AlertaEmail(unitOfWork);
                Dominio.Entidades.Embarcador.Notificacoes.AlertaEmail alertaEmail = new Dominio.Entidades.Embarcador.Notificacoes.AlertaEmail();

                PreencherAlertaEmail(alertaEmail, unitOfWork);

                unitOfWork.Start();

                if (!VerificarDuplicidade(alertaEmail, repositorioalertaEmail))
                    repositorioalertaEmail.Inserir(alertaEmail, Auditado);
                else
                    throw new ControllerException("Já existe um alerta com esses dados");


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
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar os dados.");
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
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Notificacoes.AlertaEmail repositorioalertaEmail = new Repositorio.Embarcador.Notificacoes.AlertaEmail(unitOfWork);
                Dominio.Entidades.Embarcador.Notificacoes.AlertaEmail alertaEmail = repositorioalertaEmail.BuscarPorCodigo(codigo, auditavel: true);

                if (alertaEmail == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro");

                PreencherAlertaEmail(alertaEmail, unitOfWork);

                unitOfWork.Start();

                if (!VerificarDuplicidade(alertaEmail, repositorioalertaEmail))
                    repositorioalertaEmail.Atualizar(alertaEmail, Auditado);
                else
                    throw new ControllerException("Impossivel atualizar já existe um alerta com esses dados");

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
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
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
                Repositorio.Embarcador.Notificacoes.AlertaEmail repositorioalertaEmail = new Repositorio.Embarcador.Notificacoes.AlertaEmail(unitOfWork);
                Dominio.Entidades.Embarcador.Notificacoes.AlertaEmail alertaEmail = repositorioalertaEmail.BuscarPorCodigo(codigo, auditavel: true);

                if (alertaEmail == null)
                    throw new ControllerException(Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                repositorioalertaEmail.Deletar(alertaEmail, Auditado);

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

                if (ExcessaoPorPossuirDependeciasNoBanco(excecao))
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelExcluirRegistro);

                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExcluir);
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
                Repositorio.Embarcador.Notificacoes.AlertaEmail repositorioalertaEmail = new Repositorio.Embarcador.Notificacoes.AlertaEmail(unitOfWork);
                Dominio.Entidades.Embarcador.Notificacoes.AlertaEmail alertaEmail = repositorioalertaEmail.BuscarPorCodigo(codigo, auditavel: false);

                if (alertaEmail == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro");

                return new JsonpResult(new
                {
                    alertaEmail.Codigo,
                    alertaEmail.Descricao,
                    DataHoraInicio = alertaEmail.DataHoraInicio.ToString("dd/MM/yyyy HH:mm"),
                    DataHoraFim = alertaEmail.DataHoraFim?.ToString("dd/MM/yyyy HH:mm"),
                    alertaEmail.NumeroRepeticoes,
                    alertaEmail.PeriodoNotificacoes,
                    ListaUsuario = (
                        from usuario in alertaEmail.Usuarios
                        select new
                        {
                            usuario.Codigo,
                            usuario.Descricao
                        }).ToList(),
                    ListaSetor = (
                        from setor in alertaEmail.Setor
                        select new
                        {
                            setor.Codigo,
                            setor.Descricao
                        }).ToList(),
                    ListaPortfolio = (
                        from portfolio in alertaEmail.Portfolio
                        select new
                        {
                            portfolio.Codigo,
                            portfolio.Descricao
                        }).ToList(),
                    ListaIrregularidade = (
                        from irregularidade in alertaEmail.Irregularidade
                        select new
                        {
                            irregularidade.Codigo,
                            irregularidade.Descricao
                        }).ToList()
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

        #endregion

        #region Métodos Privados
        private bool VerificarDuplicidade(Dominio.Entidades.Embarcador.Notificacoes.AlertaEmail alertaEmail, Repositorio.Embarcador.Notificacoes.AlertaEmail repositorioalertaEmail)
        {
            return repositorioalertaEmail.ExisteDuplicado(alertaEmail);
        }

        private void PreencherAlertaEmail(Dominio.Entidades.Embarcador.Notificacoes.AlertaEmail alertaEmail, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Setor repSetor = new Repositorio.Setor(unitOfWork);
            Repositorio.Embarcador.GerenciamentoIrregularidades.PortfolioModuloControle repPortfolio = new Repositorio.Embarcador.GerenciamentoIrregularidades.PortfolioModuloControle(unitOfWork);
            Repositorio.Embarcador.GerenciamentoIrregularidades.Irregularidade repIrregularidade = new Repositorio.Embarcador.GerenciamentoIrregularidades.Irregularidade(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);


            List<int> codigosSetor = Request.GetListParam<int>("Setores");
            List<int> codigosPortfolio = Request.GetListParam<int>("Portfolios");
            List<int> codigosIrregularidade = Request.GetListParam<int>("Irregularidades");
            List<int> codigosUsuario = Request.GetListParam<int>("Usuarios");

            List<Dominio.Entidades.Setor> listaSetor = codigosSetor.Count > 0 ? repSetor.BuscarPorCodigos(codigosSetor) : new List<Dominio.Entidades.Setor>();
            List<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.PortfolioModuloControle> listaPortfolio = codigosPortfolio.Count > 0 ? repPortfolio.BuscarPorCodigos(codigosPortfolio) : new List<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.PortfolioModuloControle>();
            List<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.Irregularidade> listaIrregularidade = codigosIrregularidade.Count > 0 ? repIrregularidade.BuscarPorCodigos(codigosIrregularidade) : new List<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.Irregularidade>();
            List<Dominio.Entidades.Usuario> listaUsuario = codigosUsuario.Count > 0 ? repUsuario.BuscarPorCodigos(codigosUsuario) : new List<Dominio.Entidades.Usuario>();

            alertaEmail.Descricao = Request.GetStringParam("Descricao");
            alertaEmail.DataHoraInicio = Request.GetDateTimeParam("DataHoraInicio");
            alertaEmail.DataHoraFim = Request.GetNullableDateTimeParam("DataHoraFim");
            alertaEmail.NumeroRepeticoes= Request.GetIntParam("NumeroRepeticoes");
            alertaEmail.PeriodoNotificacoes = Request.GetEnumParam<EnumIntervaloAlertaEmail>("PeriodoNotificacoes");
            alertaEmail.Setor = listaSetor;
            alertaEmail.Portfolio = listaPortfolio;
            alertaEmail.Irregularidade = listaIrregularidade;
            alertaEmail.Usuarios = listaUsuario;        
        }

        private Dominio.ObjetosDeValor.Embarcador.Notificacao.FiltroPesquisaAlertaEmail ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Notificacao.FiltroPesquisaAlertaEmail()
            {
                Descricao = Request.GetStringParam("Descricao"),
                DataHoraInicio = Request.GetNullableDateTimeParam("DataHoraInicio"),
                DataHoraFim = Request.GetNullableDateTimeParam("DataHoraFim"),
                Setor = Request.GetListParam<int>("Setor"),
                Portfolio = Request.GetListParam<int>("Portfolio"),
                Irregularidade = Request.GetListParam<int>("Irregularidade")
            };
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Notificacao.FiltroPesquisaAlertaEmail filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 50, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Data Inicial", "DataHoraInicio", 50, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Data Final", "DataHoraFim", 50, Models.Grid.Align.left, false);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);

                Repositorio.Embarcador.Notificacoes.AlertaEmail repositorioalertaEmail = new Repositorio.Embarcador.Notificacoes.AlertaEmail(unitOfWork);
                int totalRegistros = repositorioalertaEmail.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Notificacoes.AlertaEmail> listaAlertaEmail = (totalRegistros > 0) ? repositorioalertaEmail.Consultar(parametrosConsulta, filtrosPesquisa) : new List<Dominio.Entidades.Embarcador.Notificacoes.AlertaEmail>();

                var listaAlertaEmailRetornar = (
                    from alertaEmail in listaAlertaEmail
                    select new
                    {
                        alertaEmail.Codigo,
                        alertaEmail.Descricao,
                        alertaEmail.DataHoraInicio,
                        alertaEmail.DataHoraFim
                    }
                ).ToList();

                grid.AdicionaRows(listaAlertaEmailRetornar);
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
            if (propriedadeOrdenar == "DescricaoAtivo")
                return "Ativo";

            return propriedadeOrdenar;
        }
        #endregion
    }
}
