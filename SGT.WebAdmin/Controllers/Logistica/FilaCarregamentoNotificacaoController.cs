using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/FilaCarregamentoNotificacao")]
    public class FilaCarregamentoNotificacaoController : BaseController
    {
		#region Construtores

		public FilaCarregamentoNotificacaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Notificacoes.NotificacaoMobile repositorio = new Repositorio.Embarcador.Notificacoes.NotificacaoMobile(unitOfWork);
                Dominio.Entidades.Embarcador.Notificacoes.NotificacaoMobile notificacao = new Dominio.Entidades.Embarcador.Notificacoes.NotificacaoMobile()
                {
                    CentroCarregamento = ObterCentroCarregamento(unitOfWork),
                    Assunto = Request.GetNullableStringParam("Assunto") ?? throw new ControllerException("O assunto deve ser informado"),
                    Mensagem = Request.GetNullableStringParam("Mensagem") ?? throw new ControllerException("A mensagem deve ser informada"),
                    Data = DateTime.Now,
                    TipoLancamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLancamentoNotificacaoMobile.Manual
                };

                repositorio.Inserir(notificacao);

                List<Dominio.Entidades.Embarcador.Notificacoes.NotificacaoMobileUsuario> listaNotificacaoUsuario = AdicionarUsuarios(unitOfWork, notificacao);

                unitOfWork.CommitChanges();

                string stringConexao = _conexao.StringConexao;

                Task.Factory.StartNew(() => NotificarMensagemAdicionada(stringConexao, listaNotificacaoUsuario));

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

                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarMotoristas()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisaMotoristas());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Notificacoes.NotificacaoMobile repositorio = new Repositorio.Embarcador.Notificacoes.NotificacaoMobile(unitOfWork);
                Dominio.Entidades.Embarcador.Notificacoes.NotificacaoMobile notificacao = repositorio.BuscarPorCodigo(codigo, auditavel: false);

                if (notificacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    notificacao.Codigo,
                    notificacao.Assunto,
                    CentroCarregamento = new { Codigo = notificacao.CentroCarregamento?.Codigo ?? 0, Descricao = notificacao.CentroCarregamento?.Descricao ?? "" },
                    Data = notificacao.Data.ToString("dd/MM/yyyy HH:mm"),
                    notificacao.Mensagem
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
                var grid = ObterGridPesquisa();
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

        private List<Dominio.Entidades.Embarcador.Notificacoes.NotificacaoMobileUsuario> AdicionarUsuarios(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Notificacoes.NotificacaoMobile notificacao)
        {
            List<int> listaCodigoMotorista = Request.GetListParam<int>("Motorista");
            List<Dominio.Entidades.Usuario> motoristas = new List<Dominio.Entidades.Usuario>();
            List<Dominio.Entidades.Embarcador.Notificacoes.NotificacaoMobileUsuario> listaNotificacaoUsuario = new List<Dominio.Entidades.Embarcador.Notificacoes.NotificacaoMobileUsuario>();

            if (listaCodigoMotorista.Count() > 0)
            {
                foreach (int codigoMotorista in listaCodigoMotorista)
                    motoristas.Add(ObterMotorista(unitOfWork, codigoMotorista));
            }
            else
            {
                Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta();
                Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaMotoristaMobile filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaMotoristaMobile()
                {
                    CodigoCentroCarregamento = Request.GetIntParam("CentroCarregamento"),
                    CodigoModeloVeicularCarga = Request.GetIntParam("ModeloVeicularCarga"),
                    Transportador = ObterTransportador(unitOfWork)
                };

                motoristas = repositorioUsuario.ConsultarMotoristaMobile(filtrosPesquisa, parametrosConsulta);
            }

            if (motoristas.Count() > 0)
            {
                Repositorio.Embarcador.Notificacoes.NotificacaoMobileUsuario repositorio = new Repositorio.Embarcador.Notificacoes.NotificacaoMobileUsuario(unitOfWork);

                foreach (var motorista in motoristas)
                {
                    Dominio.Entidades.Embarcador.Notificacoes.NotificacaoMobileUsuario notificacaoUsuario = new Dominio.Entidades.Embarcador.Notificacoes.NotificacaoMobileUsuario()
                    {
                        Usuario = motorista,
                        Notificacao = notificacao,
                    };

                    repositorio.Inserir(notificacaoUsuario);
                    listaNotificacaoUsuario.Add(notificacaoUsuario);
                }
            }
            else
                throw new ControllerException("Não foi encontrado nenhum motorista para enviar a notificação.");

            return listaNotificacaoUsuario;
        }

        private void NotificarMensagemAdicionada(string stringConexao, List<Dominio.Entidades.Embarcador.Notificacoes.NotificacaoMobileUsuario> listaNotificacaoUsuario)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

            try
            {
                
                Repositorio.Embarcador.Notificacoes.NotificacaoMobileUsuario repositorio = new Repositorio.Embarcador.Notificacoes.NotificacaoMobileUsuario(unitOfWork);
                Servicos.Embarcador.HubsMobile.NotificacaoMobile servicoNotificacao = new Servicos.Embarcador.HubsMobile.NotificacaoMobile();

                foreach (var notificacaoUsuario in listaNotificacaoUsuario)
                {
                    bool notificacaoEnviada = servicoNotificacao.NotificarMensagemAdicionada(notificacaoUsuario.Usuario.CPF);

                    if (notificacaoEnviada)
                    {
                        notificacaoUsuario.Enviada = true;

                        repositorio.Atualizar(notificacaoUsuario);
                    }
                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private Dominio.Entidades.Embarcador.Logistica.CentroCarregamento ObterCentroCarregamento(Repositorio.UnitOfWork unitOfWork)
        {
            int codigoCentroCarregamento = Request.GetIntParam("CentroCarregamento");

            if (codigoCentroCarregamento > 0)
            {
                Repositorio.Embarcador.Logistica.CentroCarregamento repositorioCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unitOfWork);

                return repositorioCentroCarregamento.BuscarPorCodigo(codigoCentroCarregamento) ?? throw new ControllerException("Centro de carregamento não encontrado.");
            }

            return null;
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
                grid.AdicionarCabecalho("Data", "Data", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Assunto", "Assunto", 65, Models.Grid.Align.left, false);

                Dominio.ObjetosDeValor.Embarcador.Notificacao.FiltroPesquisaNofificacaoMobile filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Notificacao.FiltroPesquisaNofificacaoMobile()
                {
                    Assunto = Request.GetStringParam("Assunto"),
                    CodigoCentroCarregamento = Request.GetIntParam("CentroCarregamento"),
                    CodigoUsuario = Request.GetIntParam("Motorista"),
                    DataInicio = Request.GetNullableDateTimeParam("DataInicio"),
                    DataLimite = Request.GetNullableDateTimeParam("DataLimite"),
                    TipoLancamento = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLancamentoNotificacaoMobile>("Tipo")
                };

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                Repositorio.Embarcador.Notificacoes.NotificacaoMobile repositorio = new Repositorio.Embarcador.Notificacoes.NotificacaoMobile(unitOfWork);
                int totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Notificacoes.NotificacaoMobile> listaNotificacao = totalRegistros > 0 ? repositorio.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Notificacoes.NotificacaoMobile>();

                var listaNotificacaoRetornar = (
                    from notificacao in listaNotificacao
                    select new
                    {
                        notificacao.Codigo,
                        Data = notificacao.Data.ToString("dd/MM/yyyy HH:mm"),
                        notificacao.Assunto
                    }
                ).ToList();

                grid.AdicionaRows(listaNotificacaoRetornar);
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

        private Models.Grid.Grid ObterGridPesquisaMotoristas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CPF", "CpfMotorista", 20, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Motorista", "NomeMotorista", 50, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Enviada", "Enviada", 15, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Data Leitura", "DataLeitura", 15, Models.Grid.Align.center, false);

                int codigo = Request.GetIntParam("Codigo");
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenarMotoristas);
                Repositorio.Embarcador.Notificacoes.NotificacaoMobileUsuario repositorio = new Repositorio.Embarcador.Notificacoes.NotificacaoMobileUsuario(unitOfWork);
                int totalRegistros = repositorio.ContarConsultaPorNotificacao(codigo);
                List<Dominio.Entidades.Embarcador.Notificacoes.NotificacaoMobileUsuario> listaNotificacaoUsuario = totalRegistros > 0 ? repositorio.ConsultarPorNotificacao(codigo, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Notificacoes.NotificacaoMobileUsuario>();

                var listaNotificacaoUsuarioRetornar = (
                    from notificacaoUsuario in listaNotificacaoUsuario
                    select new
                    {
                        notificacaoUsuario.Codigo,
                        CpfMotorista = notificacaoUsuario.Usuario.CPF_Formatado,
                        DataLeitura = notificacaoUsuario.DataLeitura?.ToString("dd/MM/yyyy HH:mm") ?? "",
                        Enviada = notificacaoUsuario.DescricaoEnviada,
                        NomeMotorista = notificacaoUsuario.Usuario.Nome
                    }
                ).ToList();

                grid.AdicionaRows(listaNotificacaoUsuarioRetornar);
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

        private Dominio.Entidades.Usuario ObterMotorista(Repositorio.UnitOfWork unitOfWork, int codigoMotorista)
        {
            Repositorio.Usuario repositorioMotorista = new Repositorio.Usuario(unitOfWork);

            return repositorioMotorista.BuscarPorCodigo(codigoMotorista) ?? throw new ControllerException("Motorista não encontrado.");
        }

        private string ObterPropriedadeOrdenarMotoristas(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "NomeMotorista")
                return "Usuario.Nome";

            return propriedadeOrdenar;
        }

        private Dominio.Entidades.Empresa ObterTransportador(Repositorio.UnitOfWork unitOfWork)
        {
            if ((TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe) || (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe))
                return Usuario.Empresa;
            else if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                int codigoEmpresa = Request.GetIntParam("Transportador");

                if (codigoEmpresa > 0)
                {
                    Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);

                    return repositorioEmpresa.BuscarPorCodigo(codigoEmpresa);
                }
            }

            return null;
        }

        #endregion
    }
}
