using Dominio.Entidades.Embarcador.Usuarios;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Importacao;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SGT.WebAdmin.Notifications;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Pessoas
{
    [CustomAuthorize("Pessoas/Usuario", "Pedidos/Pedido", "Pedidos/PedidoSVM")]
    public class UsuarioController : BaseController
    {
        private readonly IMediator _mediator;

        #region Construtores

        public UsuarioController(Conexao conexao, IMediator mediator) : base(conexao)
        {
            _mediator = mediator;
        }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisarOperadores()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string nome = Request.Params("Nome");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo = !string.IsNullOrEmpty(Request.Params("Ativo")) ? (Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa)int.Parse(Request.Params("Ativo")) : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;
                string status = "";
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoColaborador situacaoColaborador;
                Enum.TryParse(Request.Params("SituacaoColaborador"), out situacaoColaborador);

                bool apenasPodeAdicionarComplemento = false;

                bool.TryParse(Request.Params("ApenasPodeAdicionarComplemento"), out apenasPodeAdicionarComplemento);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descricao", false);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Usuario.Nome, "Nome", 50, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("CPF_CNPJ_Formatado", false);
                if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "DescricaoStatus", 8, Models.Grid.Align.center, false);
                else
                {
                    if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                        status = "A";
                    else
                        status = "I";
                }

                Repositorio.Embarcador.Operacional.OperadorLogistica repOperadorLogistica = new Repositorio.Embarcador.Operacional.OperadorLogistica(unitOfWork);

                List<Dominio.Entidades.Usuario> usuarios = repOperadorLogistica.ConsultarOperador(situacaoColaborador, nome, status, Dominio.Enumeradores.TipoAcesso.Embarcador, apenasPodeAdicionarComplemento, grid.inicio, grid.limite, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena);
                grid.setarQuantidadeTotal(repOperadorLogistica.ContarConsulta(situacaoColaborador, nome, status, Dominio.Enumeradores.TipoAcesso.Embarcador, apenasPodeAdicionarComplemento));

                var retorno = (from obj in usuarios
                               select new
                               {
                                   obj.Codigo,
                                   obj.Descricao,
                                   obj.Nome,
                                   obj.CPF_CNPJ_Formatado,
                                   obj.DescricaoStatus
                               }).ToList();

                grid.AdicionaRows(retorno);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisarMotorista()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                string nome = Request.Params("Nome");
                string cpfCnpj = Request.Params("CPF");

                bool.TryParse(Request.Params("Ativo"), out bool ativo);
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoColaborador situacaoColaborador;
                Enum.TryParse(Request.Params("SituacaoColaborador"), out situacaoColaborador);

                int empresa = 0;
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Nome, "Nome", 50, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Pessoas.Usuario.CPFBarraCNPJ, "CPF", 14, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Pessoas.Usuario.Veiculo, "Veiculo", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "Situacao", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Pessoas.Usuario.CodigoVeiculo, "CodigoVeiculo", 5, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho("CodigoContaMotorista", false);
                grid.AdicionarCabecalho("DescricaoContaMotorista", false);
                grid.AdicionarCabecalho("CodigoContaUsuario", false);
                grid.AdicionarCabecalho("DescricaoContaUsuario", false);
                grid.AdicionarCabecalho("Descricao", false);
                grid.AdicionarCabecalho("SaldoDiaria", false);

                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    empresa = this.Empresa.Codigo;

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenar == "Descricao")
                    propOrdenar = "Codigo";

                List<Dominio.Entidades.Usuario> usuarios = repUsuario.ConsultarMotoristas(situacaoColaborador, cpfCnpj, nome, empresa, 0, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotorista.Todos, ativo, grid.inicio, grid.limite, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena);
                var lista = (from p in usuarios
                             select new
                             {
                                 p.Codigo,
                                 p.Descricao,
                                 CPF = p.CPF_CNPJ_Formatado,
                                 Nome = p.Nome,
                                 Veiculo = repVeiculo.BuscarPorMotorista(p.Codigo) != null ? repVeiculo.BuscarPorMotorista(p.Codigo).Placa : string.Empty,
                                 CodigoVeiculo = repVeiculo.BuscarPorMotorista(p.Codigo) != null ? repVeiculo.BuscarPorMotorista(p.Codigo).Codigo : 0,
                                 CodigoContaMotorista = p.PlanoAcertoViagem?.Codigo ?? 0,
                                 DescricaoContaMotorista = p.PlanoAcertoViagem?.BuscarDescricao ?? string.Empty,
                                 CodigoContaUsuario = this.Usuario.PlanoConta?.Codigo ?? 0,
                                 DescricaoContaUsuario = this.Usuario.PlanoConta?.BuscarDescricao ?? string.Empty,
                                 SaldoDiaria = p.SaldoDiaria.ToString("n2"),
                                 Situacao = p.DescricaoStatus
                             }).ToList();

                grid.AdicionaRows(lista);

                grid.setarQuantidadeTotal(repUsuario.ContarConsultaMotoristas(situacaoColaborador, cpfCnpj, nome, empresa, 0, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotorista.Todos, ativo));
                grid.AdicionaRows(lista);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisarMotoristaProprio()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                TipoMotorista tipoMotorista = TipoMotorista.Proprio;

                string nome = Request.Params("Nome");
                string cpfCnpj = Request.Params("CPF");

                bool.TryParse(Request.Params("Ativo"), out bool ativo);
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoColaborador situacaoColaborador;
                Enum.TryParse(Request.Params("SituacaoColaborador"), out situacaoColaborador);

                int empresa = 0;
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Nome, "Nome", 50, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Pessoas.Usuario.CPFBarraCNPJ, "CPF", 14, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Pessoas.Usuario.Veiculo, "Veiculo", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "Situacao", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Pessoas.Usuario.CodigoVeiculo, "CodigoVeiculo", 5, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho("CodigoContaMotorista", false);
                grid.AdicionarCabecalho("DescricaoContaMotorista", false);
                grid.AdicionarCabecalho("CodigoContaUsuario", false);
                grid.AdicionarCabecalho("DescricaoContaUsuario", false);
                grid.AdicionarCabecalho("Descricao", false);
                grid.AdicionarCabecalho("SaldoDiaria", false);

                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    empresa = this.Empresa.Codigo;

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenar == "Descricao")
                    propOrdenar = "Codigo";

                List<Dominio.Entidades.Usuario> usuarios = repUsuario.ConsultarMotoristas(situacaoColaborador, cpfCnpj, nome, empresa, 0, tipoMotorista, ativo, grid.inicio, grid.limite, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena);
                var lista = (from p in usuarios
                             select new
                             {
                                 p.Codigo,
                                 p.Descricao,
                                 CPF = p.CPF_CNPJ_Formatado,
                                 Nome = p.Nome,
                                 Veiculo = repVeiculo.BuscarPorMotorista(p.Codigo) != null ? repVeiculo.BuscarPorMotorista(p.Codigo).Placa : string.Empty,
                                 CodigoVeiculo = repVeiculo.BuscarPorMotorista(p.Codigo) != null ? repVeiculo.BuscarPorMotorista(p.Codigo).Codigo : 0,
                                 CodigoContaMotorista = p.PlanoAcertoViagem?.Codigo ?? 0,
                                 DescricaoContaMotorista = p.PlanoAcertoViagem?.BuscarDescricao ?? string.Empty,
                                 CodigoContaUsuario = this.Usuario.PlanoConta?.Codigo ?? 0,
                                 DescricaoContaUsuario = this.Usuario.PlanoConta?.BuscarDescricao ?? string.Empty,
                                 SaldoDiaria = p.SaldoDiaria.ToString("n2"),
                                 Situacao = p.DescricaoStatus
                             }).ToList();

                grid.AdicionaRows(lista);

                grid.setarQuantidadeTotal(repUsuario.ContarConsultaMotoristas(situacaoColaborador, cpfCnpj, nome, empresa, 0, tipoMotorista, ativo));
                grid.AdicionaRows(lista);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisarAjudantes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                string nome = Request.Params("Nome");
                string cpfCnpj = Request.Params("CPF");

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoColaborador situacaoColaborador;
                Enum.TryParse(Request.Params("SituacaoColaborador"), out situacaoColaborador);

                int empresa = 0;
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Nome, "Nome", 50, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Pessoas.Usuario.CPFBarraCNPJ, "CPF", 14, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Pessoas.Usuario.Veiculo, "Veiculo", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "Situacao", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Pessoas.Usuario.CodigoVeiculo, "CodigoVeiculo", 5, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho("CodigoContaMotorista", false);
                grid.AdicionarCabecalho("DescricaoContaMotorista", false);
                grid.AdicionarCabecalho("CodigoContaUsuario", false);
                grid.AdicionarCabecalho("DescricaoContaUsuario", false);
                grid.AdicionarCabecalho("Descricao", false);
                grid.AdicionarCabecalho("SaldoDiaria", false);

                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    empresa = this.Empresa.Codigo;

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenar == "Descricao")
                    propOrdenar = "Codigo";

                List<Dominio.Entidades.Usuario> usuarios = repUsuario.ConsultarAjudantes(situacaoColaborador, cpfCnpj, nome, empresa, 0, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotorista.Todos, true, grid.inicio, grid.limite, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena);
                var lista = (from p in usuarios
                             select new
                             {
                                 p.Codigo,
                                 p.Descricao,
                                 CPF = p.CPF_CNPJ_Formatado,
                                 Nome = p.Nome,
                                 Veiculo = repVeiculo.BuscarPorMotorista(p.Codigo) != null ? repVeiculo.BuscarPorMotorista(p.Codigo).Placa : string.Empty,
                                 CodigoVeiculo = repVeiculo.BuscarPorMotorista(p.Codigo) != null ? repVeiculo.BuscarPorMotorista(p.Codigo).Codigo : 0,
                                 CodigoContaMotorista = p.PlanoAcertoViagem?.Codigo ?? 0,
                                 DescricaoContaMotorista = p.PlanoAcertoViagem?.BuscarDescricao ?? string.Empty,
                                 CodigoContaUsuario = this.Usuario.PlanoConta?.Codigo ?? 0,
                                 DescricaoContaUsuario = this.Usuario.PlanoConta?.BuscarDescricao ?? string.Empty,
                                 SaldoDiaria = p.SaldoDiaria.ToString("n2"),
                                 Situacao = p.DescricaoStatus
                             }).ToList();

                grid.AdicionaRows(lista);

                grid.setarQuantidadeTotal(repUsuario.ContarConsultaAjudantes(situacaoColaborador, cpfCnpj, nome, empresa, 0, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotorista.Todos, true));
                grid.AdicionaRows(lista);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisarTerceiros()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string nome = Request.Params("Descricao");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Usuario.Nome, "Nome", 64, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Usuario.CPFCNPJ, "CPF_CNPJ_Formatado", 14, Models.Grid.Align.left, false);

                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

                List<Dominio.Entidades.Usuario> usuarios = repUsuario.BuscarPorTipoAcesso(Dominio.Enumeradores.TipoAcesso.Terceiro, nome);
                grid.setarQuantidadeTotal(repUsuario.ContarPorTipoAcesso(Dominio.Enumeradores.TipoAcesso.Terceiro, nome));

                var retorno = (from obj in usuarios
                               select new
                               {
                                   obj.Codigo,
                                   obj.Nome,
                                   obj.CPF_CNPJ_Formatado
                               }).ToList();

                grid.AdicionaRows(usuarios);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaUsuario filtrosPesquisa = ObterFiltrosPesquisa();
                filtrosPesquisa.SomenteFuncionarios = false;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CPF", false);
                grid.AdicionarCabecalho("Descricao", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Nome, "Nome", 40, Models.Grid.Align.left, true);

                if (!Request.GetBoolParam("EsconderCPF"))
                    grid.AdicionarCabecalho("CPF_CNPJ_Formatado", false);

                grid.AdicionarCabecalho(Localization.Resources.Consultas.Funcionario.Login, "Login", 20, Models.Grid.Align.left, false, true, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Funcionario.Email, "Email", 20, Models.Grid.Align.left, false, true, true);

                if (
                    (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe) &&
                    (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin)
                )
                    grid.AdicionarCabecalho(Localization.Resources.Consultas.Funcionario.Perfil, "PerfilAcesso", 15, Models.Grid.Align.left, false, true, true);

                if (filtrosPesquisa.Ativo == SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "DescricaoStatus", 8, Models.Grid.Align.center, false, true, true);

                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                List<Dominio.Entidades.Usuario> listaUsuario = repUsuario.Consultar(filtrosPesquisa, grid.inicio, grid.limite, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena);
                grid.setarQuantidadeTotal(repUsuario.ContarConsulta(filtrosPesquisa));

                var retorno = (
                    from obj in listaUsuario
                    select new
                    {
                        obj.Codigo,
                        CPF = obj.CPF_CNPJ_Formatado,
                        obj.Descricao,
                        obj.Nome,
                        PerfilAcesso = obj.PerfilAcesso?.Descricao ?? "",
                        obj.CPF_CNPJ_Formatado,
                        obj.RG,
                        obj.Email,
                        obj.DescricaoStatus,
                        obj.Login
                    }
                ).ToList();

                grid.AdicionaRows(retorno);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaUsuariosEmbarcador()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaUsuario filtrosPesquisa = ObterFiltrosPesquisa();
                filtrosPesquisa.TipoAcesso = Dominio.Enumeradores.TipoAcesso.Embarcador;
                filtrosPesquisa.SomenteFuncionarios = true;
                filtrosPesquisa.TipoUsuario = TipoUsuario.Todos;

                int codigoCarga = Request.GetIntParam("Carga");
                Repositorio.Setor repSetor = new Repositorio.Setor(unitOfWork);
                filtrosPesquisa.CodigoSetor = codigoCarga > 0 ? repSetor.BuscarPorCarga(codigoCarga)?.Codigo ?? 0 : 0;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CPF", false);
                grid.AdicionarCabecalho("Descricao", false);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Usuario.Nome, "Nome", 28, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Usuario.CPFCNPJ, "CPF_CNPJ_Formatado", 13, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Usuario.Login, "Login", 13, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Usuario.Email, "Email", 27, Models.Grid.Align.left, false);

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe && TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin)
                    grid.AdicionarCabecalho(Localization.Resources.Consultas.Usuario.Perfil, "PerfilAcesso", 20, Models.Grid.Align.left, false);

                if (filtrosPesquisa.Ativo == SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "DescricaoStatus", 8, Models.Grid.Align.center, false);

                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                List<Dominio.Entidades.Usuario> listaUsuario = repUsuario.Consultar(filtrosPesquisa, grid.inicio, grid.limite, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena);
                grid.setarQuantidadeTotal(repUsuario.ContarConsulta(filtrosPesquisa));

                var retorno = (from obj in listaUsuario
                               select new
                               {
                                   obj.Codigo,
                                   CPF = obj.CPF_CNPJ_Formatado,
                                   obj.Descricao,
                                   obj.Nome,
                                   PerfilAcesso = obj.PerfilAcesso?.Descricao ?? "",
                                   obj.CPF_CNPJ_Formatado,
                                   obj.RG,
                                   obj.Email,
                                   obj.DescricaoStatus,
                                   obj.Login
                               }).ToList();

                grid.AdicionaRows(retorno);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        [AllowAuthenticate]
        public async Task<IActionResult> AlterarSenha()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.Pessoa.PoliticaSenha serPoliticaSenha = new Servicos.Embarcador.Pessoa.PoliticaSenha();
                Dominio.Entidades.Usuario usuario = this.Usuario;
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Embarcador.Usuarios.PoliticaSenha repPoliticaSenha = new Repositorio.Embarcador.Usuarios.PoliticaSenha(unitOfWork);
                Dominio.Entidades.Embarcador.Usuarios.PoliticaSenha politicaSenha = repPoliticaSenha.BuscarPoliticaPadrao();

                string senhaAtual = Request.Params("SenhaAtual");
                string senhaMD5 = "", senhaSHA256 = "";

                if (usuario.SenhaCriptografada)
                {
                    senhaSHA256 = Servicos.Criptografia.GerarHashSHA256(senhaAtual);
                    senhaMD5 = Servicos.Criptografia.GerarHashMD5(senhaAtual);
                }

                if (senhaAtual == usuario.Senha || senhaSHA256 == usuario.Senha || senhaMD5 == usuario.Senha)
                {
                    usuario.Senha = Request.Params("NovaSenha");

                    string retornoPoliticaSenha = "";

                    if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                        retornoPoliticaSenha = serPoliticaSenha.AplicarPoliticaSenha(ref usuario, politicaSenha, unitOfWork);

                    if (string.IsNullOrWhiteSpace(retornoPoliticaSenha))
                    {
                        repUsuario.Atualizar(usuario);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, usuario, null, Localization.Resources.Pessoas.Usuario.AtualizouSenha, unitOfWork);
                        return new JsonpResult(true);
                    }
                    else
                    {
                        return new JsonpResult(false, true, retornoPoliticaSenha);
                    }
                }
                else
                {
                    return new JsonpResult(false, true, Localization.Resources.Pessoas.Usuario.SenhaAtualEstaIncorreta);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Pessoas.Usuario.OcorreuUmaFalhaAoAtualizarSenha);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        public async Task<IActionResult> RedefinirSenha()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

                Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorCodigo(int.Parse(Request.Params("Codigo")));

                usuario.DataUltimaAlteracaoSenhaObrigatoria = null;
                usuario.AlterarSenhaAcesso = true;

                Servicos.Auditoria.Auditoria.Auditar(Auditado, usuario, null, Localization.Resources.Pessoas.Usuario.RedefiniuSenha, unitOfWork);

                repUsuario.Atualizar(usuario);
                unitOfWork.CommitChanges();

                var notifications = new UsuarioImportadoNotification(
                           usuario.Nome,
                           usuario.Email,
                           usuario.Senha,
                           usuario.Login
                       );

                await EnviarNotificacaoUsuarioImportadoAsync(notifications);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Pessoas.Usuario.OcorreuUmaFalhaAoRedefinirSenha);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(_conexao.AdminStringConexao);

            try
            {
                unitOfWork.Start();

                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Embarcador.Usuarios.FuncionarioFormulario repFuncionarioFormulario = new Repositorio.Embarcador.Usuarios.FuncionarioFormulario(unitOfWork);
                Repositorio.Embarcador.Usuarios.FuncionarioFormularioPermissaoPersonalizada repFuncionarioFormularioPermissaoPersonalizada = new Repositorio.Embarcador.Usuarios.FuncionarioFormularioPermissaoPersonalizada(unitOfWork);
                Repositorio.Embarcador.Usuarios.PoliticaSenha repPoliticaSenha = new Repositorio.Embarcador.Usuarios.PoliticaSenha(unitOfWork);
                Repositorio.Embarcador.Usuarios.FuncionarioSenhaAnterior repFuncionarioSenhaAnterior = new Repositorio.Embarcador.Usuarios.FuncionarioSenhaAnterior(unitOfWork);

                Servicos.Embarcador.Pessoa.PoliticaSenha serPoliticaSenha = new Servicos.Embarcador.Pessoa.PoliticaSenha();

                string cpfCnpjUsuario = Utilidades.String.OnlyNumbers(Request.Params("CPF"));

                Dominio.Entidades.Usuario usuarioValidacao = (!string.IsNullOrWhiteSpace(cpfCnpjUsuario) && cpfCnpjUsuario.Any(x => x != '0')) ? repUsuario.BuscarUsuarioPorCPFCNPJ(cpfCnpjUsuario, 0, this.Usuario.TipoAcesso) : null;
                if (usuarioValidacao != null)
                    return new JsonpResult(false, true, Localization.Resources.Pessoas.Usuario.JaExisteUsuarioCadastradoParaCPFBarraCNPJ + " " + cpfCnpjUsuario);

                Dominio.Entidades.Usuario usuario = new Dominio.Entidades.Usuario();

                Dominio.Enumeradores.TipoAcesso tipoAcesso = this.Usuario.TipoAcesso;
                usuario.CPF = cpfCnpjUsuario;

                PreencherUsuario(usuario, unitOfWork);
                PreencherDadosAdicionaisUsuario(usuario, unitOfWork);


                bool.TryParse(Request.Params("UsuarioMobile"), out bool usuarioUsaMobile);
                usuario.Senha = Request.Params("Senha") ?? string.Empty;
                usuario.Login = (Request.Params("Login") ?? string.Empty).ToLower().Trim();
                usuario.UsuarioAcessoBloqueado = bool.Parse(Request.Params("UsuarioAcessoBloqueado"));
                usuario.Status = Request.Params("Status");

                usuario.Tipo = "U";
                usuario.TipoAcesso = tipoAcesso;
                usuario.Empresa = this.Empresa;
                usuario.DataCadastro = DateTime.Now;
                Dominio.Entidades.Usuario usuarioExiste = repUsuario.BuscarPorLogin(usuario.Login, tipoAcesso);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    usuarioExiste = (usuarioExiste == null ? repUsuario.BuscarPorCPF(usuario.CPF) : usuarioExiste);

                Dominio.Entidades.Embarcador.Usuarios.PoliticaSenha politicaSenha = repPoliticaSenha.BuscarPoliticaPadraoPorServicoMultiSoftware(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);
                if (politicaSenha == null)
                    politicaSenha = repPoliticaSenha.BuscarPoliticaPadrao();

                string retornoPoliticaSenha = "";
                if (politicaSenha != null)
                {
                    if (string.IsNullOrWhiteSpace(usuario.Senha))
                        usuario.Senha = politicaSenha.CriarNovaSenha();

                    if (!politicaSenha.ExigirTrocaSenhaPrimeiroAcesso)
                    {
                        retornoPoliticaSenha = serPoliticaSenha.AplicarPoliticaSenha(ref usuario, politicaSenha, unitOfWork);
                        //usuario.SenhaAnterior = usuario.Senha;

                        usuario.DataUltimaAlteracaoSenhaObrigatoria = DateTime.Now;
                    }
                    else
                        usuario.AlterarSenhaAcesso = true;
                }
                if (string.IsNullOrWhiteSpace(retornoPoliticaSenha))
                {
                    if (usuarioExiste == null)
                    {
                        if (usuarioUsaMobile)
                        {
                            string novaSenhaMobile = ClienteExigeContraSenha() ? Request.GetStringParam("ContraSenhaMobile") : null;

                            string retorno = Servicos.Usuario.ConfigurarUsuarioMobile(ref usuario, novaSenhaMobile, ClienteAcesso, unitOfWorkAdmin);
                            if (!string.IsNullOrWhiteSpace(retorno))
                            {
                                unitOfWork.Rollback();
                                return new JsonpResult(false, retorno);
                            }
                        }

                        repUsuario.Inserir(usuario, Auditado);

                        Dominio.Entidades.Embarcador.Usuarios.FuncionarioSenhaAnterior funcionarioSenhaAnterior = new Dominio.Entidades.Embarcador.Usuarios.FuncionarioSenhaAnterior();
                        funcionarioSenhaAnterior.Senha = usuario.Senha;
                        funcionarioSenhaAnterior.Usuario = usuario;
                        funcionarioSenhaAnterior.SenhaCriptografada = usuario.SenhaCriptografada;
                        repFuncionarioSenhaAnterior.Inserir(funcionarioSenhaAnterior);

                        usuario.ModulosLiberados = new List<int>();
                        var jModulosUsuario = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ModulosUsuario"));
                        foreach (var jModulo in jModulosUsuario)
                        {
                            usuario.ModulosLiberados.Add((int)jModulo.CodigoModulo);
                        }

                        var jFormulariosUsuario = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("FormulariosUsuario"));
                        foreach (var jFormulario in jFormulariosUsuario)
                        {
                            Dominio.Entidades.Embarcador.Usuarios.FuncionarioFormulario funcionarioFormulario = new Dominio.Entidades.Embarcador.Usuarios.FuncionarioFormulario();
                            funcionarioFormulario.Usuario = usuario;
                            funcionarioFormulario.SomenteLeitura = (bool)jFormulario.SomenteLeitura;
                            funcionarioFormulario.CodigoFormulario = (int)jFormulario.CodigoFormulario;
                            repFuncionarioFormulario.Inserir(funcionarioFormulario);
                            foreach (var dynPermissao in jFormulario.PermissoesPersonalizadas)
                            {
                                Dominio.Entidades.Embarcador.Usuarios.FuncionarioFormularioPermissaoPersonalizada funcionarioFormularioPermissaoPersonalizada = new Dominio.Entidades.Embarcador.Usuarios.FuncionarioFormularioPermissaoPersonalizada();
                                funcionarioFormularioPermissaoPersonalizada.CodigoPermissao = (int)dynPermissao.CodigoPermissaoPersonalizada;
                                funcionarioFormularioPermissaoPersonalizada.FuncionarioFormulario = funcionarioFormulario;
                                repFuncionarioFormularioPermissaoPersonalizada.Inserir(funcionarioFormularioPermissaoPersonalizada);
                            }
                        }

                        usuario.ModulosLiberadosMobile = new List<int>();
                        var jModulosUsuarioMobile = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ModulosUsuarioMobile"));
                        foreach (var jModulo in jModulosUsuarioMobile)
                        {
                            usuario.ModulosLiberadosMobile.Add((int)jModulo.CodigoModulo);
                        }

                        bool usuarioOperador = bool.Parse(Request.Params("Operador"));
                        if (usuarioOperador)
                        {
                            Repositorio.Embarcador.Operacional.OperadorLogistica repOperadorLogistica = new Repositorio.Embarcador.Operacional.OperadorLogistica(unitOfWork);
                            Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica = new Dominio.Entidades.Embarcador.Operacional.OperadorLogistica();
                            operadorLogistica.Ativo = true;
                            operadorLogistica.SupervisorLogistica = Request.GetBoolParam("OperadorSupervisor");
                            operadorLogistica.PermiteAdicionarComplementosDeFrete = Request.GetBoolParam("PermiteAdicionarComplementosDeFrete");
                            operadorLogistica.PermitirVisualizarValorFreteTransportadoresInteressadosCarga = Request.GetBoolParam("PermitirVisualizarValorFreteTransportadoresInteressadosCarga");
                            operadorLogistica.PermitirAssumirCargasControleEntrega = Request.GetBoolParam("PermitirAssumirCargasControleEntrega");
                            operadorLogistica.Usuario = usuario;
                            repOperadorLogistica.Inserir(operadorLogistica);
                        }

                        SalvarLicencas(usuario, unitOfWork);
                        SalvarDadosBancarios(usuario, unitOfWork);
                        SalvarContatos(usuario, unitOfWork);
                        SalvarRepresentacoes(usuario, unitOfWork);
                        SalvarEPIs(usuario, unitOfWork);
                        SalvarMeta(usuario, unitOfWork);
                        SalvarClientesSetor(usuario, unitOfWork);

                        unitOfWork.CommitChanges();

                        var notifications = new UsuarioImportadoNotification(
                           usuario.Nome,
                           usuario.Email,
                           usuario.Senha,
                           usuario.Login
                       );

                        await EnviarNotificacaoUsuarioImportadoAsync(notifications);

                        return new JsonpResult(true);
                    }
                    else
                        throw new ControllerException(Localization.Resources.Pessoas.Usuario.NomeDeUsuarioJaInformado);
                }
                else
                    throw new ControllerException(retornoPoliticaSenha);
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
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAdicionar);
            }
            finally
            {
                unitOfWork.Dispose();
                unitOfWorkAdmin.Dispose();
            }
        }


        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(_conexao.AdminStringConexao);

            try
            {
                unitOfWork.Start();

                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Embarcador.Operacional.OperadorLogistica repOperadorLogistica = new Repositorio.Embarcador.Operacional.OperadorLogistica(unitOfWork);
                Repositorio.Embarcador.Usuarios.FuncionarioFormularioPermissaoPersonalizada repFuncionarioFormularioPermissaoPersonalizada = new Repositorio.Embarcador.Usuarios.FuncionarioFormularioPermissaoPersonalizada(unitOfWork);
                Repositorio.Embarcador.Usuarios.FuncionarioFormulario repFuncionarioFormulario = new Repositorio.Embarcador.Usuarios.FuncionarioFormulario(unitOfWork);
                Repositorio.Embarcador.Usuarios.PoliticaSenha repPoliticaSenha = new Repositorio.Embarcador.Usuarios.PoliticaSenha(unitOfWork);

                string cpfCnpjUsuario = Utilidades.String.OnlyNumbers(Request.Params("CPF"));
                int.TryParse(Request.Params("Codigo"), out int codigoUsuario);

                Dominio.Entidades.Usuario usuarioValidacao = (!string.IsNullOrWhiteSpace(cpfCnpjUsuario) && cpfCnpjUsuario.Any(x => x != '0')) ? repUsuario.BuscarUsuarioPorCPFCNPJ(cpfCnpjUsuario, codigoUsuario, this.Usuario.TipoAcesso) : null;
                if (usuarioValidacao != null)
                    return new JsonpResult(false, true, Localization.Resources.Pessoas.Usuario.JaExisteUsuarioCadastradoParaCPFBarraCNPJ + " " + cpfCnpjUsuario);
                Servicos.Embarcador.Pessoa.PoliticaSenha serPoliticaSenha = new Servicos.Embarcador.Pessoa.PoliticaSenha();

                Dominio.Enumeradores.TipoAcesso tipoAcesso = this.Usuario.TipoAcesso;
                Dominio.Entidades.Embarcador.Usuarios.PoliticaSenha politicaSenha = repPoliticaSenha.BuscarPoliticaPadraoPorServicoMultiSoftware(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);
                if (politicaSenha == null)
                    politicaSenha = repPoliticaSenha.BuscarPoliticaPadrao();

                Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorCodigo(codigoUsuario, true);

                usuario.CPF = cpfCnpjUsuario;

                PreencherUsuario(usuario, unitOfWork);
                PreencherDadosAdicionaisUsuario(usuario, unitOfWork);


                bool.TryParse(Request.Params("UsuarioMobile"), out bool usuarioUsaMobile);

                string senha = Request.Params("Senha");
                string senhaAntiga = usuario.Senha;

                string retornoPoliticaSenha = "";
                if (usuario.Senha != senha && !string.IsNullOrEmpty(senha))
                {
                    usuario.Senha = senha;
                    retornoPoliticaSenha = serPoliticaSenha.AplicarPoliticaSenha(ref usuario, politicaSenha, unitOfWork);

                    Repositorio.Embarcador.Usuarios.FuncionarioSenhaAnterior repFuncionarioSenhaAnterior = new Repositorio.Embarcador.Usuarios.FuncionarioSenhaAnterior(unitOfWork);
                    Dominio.Entidades.Embarcador.Usuarios.FuncionarioSenhaAnterior funcionarioSenhaAnterior = new Dominio.Entidades.Embarcador.Usuarios.FuncionarioSenhaAnterior();
                    funcionarioSenhaAnterior.Senha = usuario.Senha;
                    funcionarioSenhaAnterior.Usuario = usuario;
                    funcionarioSenhaAnterior.SenhaCriptografada = usuario.SenhaCriptografada;
                    repFuncionarioSenhaAnterior.Inserir(funcionarioSenhaAnterior);

                    usuario.DataUltimaAlteracaoSenhaObrigatoria = DateTime.Now;
                }

                if (!string.IsNullOrWhiteSpace(retornoPoliticaSenha))
                    throw new ControllerException(retornoPoliticaSenha);

                usuario.Login = Request.Params("Login").ToLower().Trim();
                usuario.UsuarioAcessoBloqueado = bool.Parse(Request.Params("UsuarioAcessoBloqueado"));
                usuario.Status = Request.Params("Status");
                usuario.TipoAcesso = tipoAcesso;

                Dominio.Entidades.Usuario usuarioExiste = repUsuario.BuscarPorLogin(usuario.Login, tipoAcesso, usuario.Codigo);

                if (usuarioExiste != null)
                    throw new ControllerException(Localization.Resources.Pessoas.Usuario.NomeDeUsuarioJaInformado);

                if (usuarioUsaMobile)
                {
                    string novaSenhaMobile = ClienteExigeContraSenha() ? Request.GetStringParam("ContraSenhaMobile") : null;

                    string retorno = Servicos.Usuario.ConfigurarUsuarioMobile(ref usuario, novaSenhaMobile, ClienteAcesso, unitOfWorkAdmin);
                    if (!string.IsNullOrWhiteSpace(retorno))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, retorno);
                    }
                }

                // Ainda não é possível ocultar campos específicos na auditoria
                // Esse trecho a baixo inibi que seja auditado a senha
                // Não altere até existir uma solução mais eficiente
                string senhaUsuario = usuario.Senha;

                usuario.Senha = senhaAntiga;
                repUsuario.Atualizar(usuario, Auditado);

                usuario.Senha = senhaUsuario;

                if (usuario.Empresa == null && tipoAcesso == Dominio.Enumeradores.TipoAcesso.Embarcador)
                    usuario.Empresa = Empresa;

                repUsuario.Atualizar(usuario);

                usuario.ModulosLiberados.Clear();
                var jModulosUsuario = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ModulosUsuario"));
                foreach (var jModulo in jModulosUsuario)
                {
                    usuario.ModulosLiberados.Add((int)jModulo.CodigoModulo);
                }

                List<Dominio.Entidades.Embarcador.Usuarios.FuncionarioFormulario> formulariosCadastrados = repFuncionarioFormulario.buscarPorUsuario(usuario.Codigo);
                foreach (Dominio.Entidades.Embarcador.Usuarios.FuncionarioFormulario formularioCadastrado in formulariosCadastrados)
                {
                    repFuncionarioFormulario.Deletar(formularioCadastrado);
                }

                var jFormulariosUsuario = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("FormulariosUsuario"));
                foreach (var jFormulario in jFormulariosUsuario)
                {
                    Dominio.Entidades.Embarcador.Usuarios.FuncionarioFormulario funcionarioFormulario = new Dominio.Entidades.Embarcador.Usuarios.FuncionarioFormulario();
                    funcionarioFormulario.Usuario = usuario;
                    funcionarioFormulario.SomenteLeitura = (bool)jFormulario.SomenteLeitura;
                    funcionarioFormulario.CodigoFormulario = (int)jFormulario.CodigoFormulario;
                    repFuncionarioFormulario.Inserir(funcionarioFormulario);
                    //var dynPermissoesPersonalizadas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string));
                    foreach (var dynPermissao in jFormulario.PermissoesPersonalizadas)
                    {
                        Dominio.Entidades.Embarcador.Usuarios.FuncionarioFormularioPermissaoPersonalizada funcionarioFormularioPermissaoPersonalizada = new Dominio.Entidades.Embarcador.Usuarios.FuncionarioFormularioPermissaoPersonalizada();
                        funcionarioFormularioPermissaoPersonalizada.CodigoPermissao = (int)dynPermissao.CodigoPermissaoPersonalizada;
                        funcionarioFormularioPermissaoPersonalizada.FuncionarioFormulario = funcionarioFormulario;
                        repFuncionarioFormularioPermissaoPersonalizada.Inserir(funcionarioFormularioPermissaoPersonalizada);
                    }
                }

                usuario.ModulosLiberadosMobile.Clear();
                var jModulosUsuarioMobile = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ModulosUsuarioMobile"));
                foreach (var jModulo in jModulosUsuarioMobile)
                {
                    usuario.ModulosLiberadosMobile.Add((int)jModulo.CodigoModulo);
                }

                Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica = repOperadorLogistica.BuscarPorUsuario(usuario.Codigo);

                bool usuarioOperador = Request.GetBoolParam("Operador");

                if (operadorLogistica != null)
                {
                    operadorLogistica.Ativo = usuarioOperador;
                    operadorLogistica.SupervisorLogistica = Request.GetBoolParam("OperadorSupervisor");
                    operadorLogistica.PermiteAdicionarComplementosDeFrete = Request.GetBoolParam("PermiteAdicionarComplementosDeFrete");
                    operadorLogistica.PermitirVisualizarValorFreteTransportadoresInteressadosCarga = Request.GetBoolParam("PermitirVisualizarValorFreteTransportadoresInteressadosCarga");
                    operadorLogistica.PermitirAssumirCargasControleEntrega = Request.GetBoolParam("PermitirAssumirCargasControleEntrega");

                    repOperadorLogistica.Atualizar(operadorLogistica);
                }
                else if (usuarioOperador)
                {
                    operadorLogistica = new Dominio.Entidades.Embarcador.Operacional.OperadorLogistica();

                    operadorLogistica.Ativo = usuarioOperador;
                    operadorLogistica.SupervisorLogistica = Request.GetBoolParam("OperadorSupervisor");
                    operadorLogistica.PermiteAdicionarComplementosDeFrete = Request.GetBoolParam("PermiteAdicionarComplementosDeFrete");
                    operadorLogistica.PermitirVisualizarValorFreteTransportadoresInteressadosCarga = Request.GetBoolParam("PermitirVisualizarValorFreteTransportadoresInteressadosCarga");
                    operadorLogistica.PermitirAssumirCargasControleEntrega = Request.GetBoolParam("PermitirAssumirCargasControleEntrega");
                    operadorLogistica.Usuario = usuario;

                    repOperadorLogistica.Inserir(operadorLogistica);
                }

                SalvarLicencas(usuario, unitOfWork);
                SalvarDadosBancarios(usuario, unitOfWork);
                SalvarContatos(usuario, unitOfWork);
                SalvarRepresentacoes(usuario, unitOfWork);
                SalvarEPIs(usuario, unitOfWork);
                SalvarMeta(usuario, unitOfWork);
                SalvarClientesSetor(usuario, unitOfWork);

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
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAtualizar);
            }
            finally
            {
                unitOfWork.Dispose();
                unitOfWorkAdmin.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.PaginaUsuario repPaginaUsuario = new Repositorio.PaginaUsuario(unitOfWork);
                Repositorio.Embarcador.Operacional.OperadorLogistica repOperadorLogistica = new Repositorio.Embarcador.Operacional.OperadorLogistica(unitOfWork);
                Repositorio.Embarcador.Usuarios.FuncionarioAnexo repFuncionarioAnexo = new Repositorio.Embarcador.Usuarios.FuncionarioAnexo(unitOfWork);
                Repositorio.Embarcador.Usuarios.FuncionarioEPI repositorioFuncionarioEPI = new Repositorio.Embarcador.Usuarios.FuncionarioEPI(unitOfWork);
                Repositorio.Embarcador.Usuarios.FuncionarioMetaVendaDireta repositorioFuncionarioMetaVendaDireta = new Repositorio.Embarcador.Usuarios.FuncionarioMetaVendaDireta(unitOfWork);

                Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorCodigo(codigo);
                if (usuario == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                List<Dominio.Entidades.Embarcador.Usuarios.FuncionarioAnexo> anexos = repFuncionarioAnexo.BuscarPorCodigoUsuario(codigo);
                Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistico = repOperadorLogistica.BuscarPorUsuario(usuario.Codigo);
                List<Dominio.Entidades.Embarcador.Usuarios.FuncionarioEPI> funcionarioEPIs = repositorioFuncionarioEPI.BuscarPorUsuario(usuario);
                List<Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioMetaVendaDireta> funcionarioMetas = repositorioFuncionarioMetaVendaDireta.BuscarPorUsuario(usuario);

                var dynUsuario = new
                {
                    usuario.Codigo,
                    usuario.Nome,
                    UsuarioMultisoftware = this.Usuario.UsuarioMultisoftware,
                    UsuarioLogadoAtendimento = this.Usuario.UsuarioAtendimento,
                    UsuarioAtendimento = usuario.UsuarioAtendimento,
                    TipoPessoa = (int)ConverteTipoPessoaParaStringParaEnumerador(usuario.TipoPessoa),
                    CPF = usuario.CPF_CNPJ_Formatado,
                    CPF_CNPJ = usuario.CPF_CNPJ_Formatado,
                    usuario.RG,
                    DataNascimento = usuario.DataNascimento != null ? usuario.DataNascimento.Value.ToString("dd/MM/yyyy") : "",
                    DataAdmissao = usuario.DataAdmissao != null ? usuario.DataAdmissao.Value.ToString("dd/MM/yyyy") : "",
                    DataFimPeriodoExperiencia = usuario.DataFimPeriodoExperiencia != null ? usuario.DataFimPeriodoExperiencia.Value.ToString("dd/MM/yyyy") : "",
                    usuario.Salario,
                    usuario.Telefone,
                    Filial = usuario.Filial == null ? null : new { usuario.Filial.Codigo, usuario.Filial.Descricao },
                    ClienteSetor = (
                        from obj in usuario.ClientesSetor
                        select new
                        {
                            Codigo = obj.Codigo,
                            Descricao = obj.Descricao,
                        }
                    ).ToList(),
                    SetorFuncionario = usuario.Setor != null ? new { usuario.Setor.Codigo, usuario.Setor.Descricao } : null,
                    Turno = usuario.Turno == null ? null : new { usuario.Turno.Codigo, usuario.Turno.Descricao },
                    CargoSetorTurno = usuario.CargoSetorTurno == null ? null : new { usuario.CargoSetorTurno.Codigo, usuario.CargoSetorTurno.Descricao },
                    CentroDeCustoSetorTurno = usuario.CentroDeCustoSetorTurno == null ? null : new { usuario.CentroDeCustoSetorTurno.Codigo, usuario.CentroDeCustoSetorTurno.Descricao },
                    usuario.NivelEscalationList,
                    Localidade = usuario.Localidade != null ? new { Codigo = usuario.Localidade.Codigo, Descricao = usuario.Localidade.DescricaoCidadeEstado } : null,
                    Cliente = usuario.Cliente != null ? new { Codigo = usuario.Cliente.CPF_CNPJ, Descricao = usuario.Cliente.Nome } : null,
                    PlanoConta = usuario.PlanoConta != null ? new { Codigo = usuario.PlanoConta.Codigo, Descricao = usuario.PlanoConta.Descricao } : null,
                    usuario.AssociarUsuarioCliente,
                    usuario.Endereco,
                    usuario.Complemento,
                    usuario.Bairro,
                    usuario.CEP,
                    usuario.NumeroEndereco,
                    usuario.TipoLogradouro,
                    usuario.EnderecoDigitado,
                    usuario.Latitude,
                    usuario.Longitude,
                    usuario.Email,
                    usuario.UsuarioAdministrador,
                    usuario.PermiteSalvarNovoRelatorio,
                    RegiaoNorte = usuario.RegiaoAcessoDashOperadorNorte,
                    RegiaoNordeste = usuario.RegiaoAcessoDashOperadorNordeste,
                    RegiaoSul = usuario.RegiaoAcessoDashOperadorSul,
                    RegiaoSudeste = usuario.RegiaoAcessoDashOperadorSudeste,
                    RegiaoCentroOeste = usuario.RegiaoAcessoDashOperadorCentroOeste,
                    usuario.PermiteTornarRelatorioPadrao,
                    usuario.UsuarioCallCenter,
                    usuario.PermitirAprovarNaoConformidade,
                    usuario.PermiteSalvarConfiguracoesRelatoriosParaTodos,
                    usuario.VisualizarGraficosIniciais,
                    usuario.VisualizarTitulosPagamentoSalario,
                    usuario.Login,
                    usuario.UsuarioAcessoBloqueado,
                    usuario.NotificadoExpedicao,
                    usuario.Status,
                    LiberarAuditoria = usuario.PermiteAuditar,
                    usuario.ExibirUsuarioAprovacao,
                    NotificacaoPorEmail = usuario.EnviarNotificacaoPorEmail,
                    Operador = operadorLogistico?.Ativo ?? false,
                    OperadorSupervisor = operadorLogistico?.SupervisorLogistica ?? false,
                    PermiteAdicionarComplementosDeFrete = operadorLogistico?.PermiteAdicionarComplementosDeFrete ?? false,
                    PermitirVisualizarValorFreteTransportadoresInteressadosCarga = operadorLogistico?.PermitirVisualizarValorFreteTransportadoresInteressadosCarga ?? false,
                    PermitirAssumirCargasControleEntrega = operadorLogistico?.PermitirAssumirCargasControleEntrega ?? false,
                    UsuarioMobile = usuario.CodigoMobile > 0,
                    PerfilAcesso = usuario.PerfilAcesso != null ? new { usuario.PerfilAcesso.Codigo, usuario.PerfilAcesso.Descricao } : null,
                    FormulariosUsuario = (from obj in usuario.FormulariosLiberados
                                          select new
                                          {
                                              obj.CodigoFormulario,
                                              obj.SomenteLeitura,
                                              PermissoesPersonalizadas = (from pp in obj.FuncionarioFormularioPermissaoesPersonalizadas
                                                                          select new
                                                                          {
                                                                              pp.CodigoPermissao
                                                                          }).ToList(),
                                          }).ToList(),
                    ModulosUsuario = (from codigoModulo in usuario.ModulosLiberados
                                      select new
                                      {
                                          CodigoModulo = codigoModulo
                                      }).ToList(),
                    GridUsuarioLicencas = (from obj in usuario.Licencas
                                           select new
                                           {
                                               obj.Codigo,
                                               obj.Descricao,
                                               obj.Numero,
                                               DataEmissao = obj.DataEmissao.Value.ToString("dd/MM/yyyy"),
                                               DataVencimento = obj.DataVencimento.Value.ToString("dd/MM/yyyy"),
                                               FormaAlerta = "[" + string.Join(",", obj.FormasAlerta.Select(o => (int)o).ToList()) + "]",
                                               obj.Status,
                                               DescricaoLicenca = obj.Licenca != null ? obj.Licenca.Descricao : string.Empty,
                                               CodigoLicenca = obj.Licenca != null ? obj.Licenca.Codigo : 0
                                           }).ToList(),
                    GridRepresentacoes = (from obj in usuario.Representacoes
                                          select new
                                          {
                                              obj.Codigo,
                                              obj.Descricao,
                                              obj.CPF_CNPJ,
                                          }).ToList(),
                    Anexos = (from obj in anexos
                              select new
                              {
                                  obj.Codigo,
                                  obj.Descricao,
                                  obj.NomeArquivo,
                                  NomeTela = "Usuario"
                              }).ToList(),
                    DataDemissao = usuario.DataDemissao != null ? usuario.DataDemissao.Value.ToString("dd/MM/yyyy") : string.Empty,
                    usuario.NumeroCTPS,
                    usuario.SerieCTPS,
                    TipoMovimentoComissao = usuario.TipoMovimentoComissao != null ? new { usuario.TipoMovimentoComissao.Codigo, usuario.TipoMovimentoComissao.Descricao } : null,
                    usuario.DiaComissao,
                    usuario.SituacaoColaborador,
                    usuario.CodigoIntegracao,
                    usuario.PISAdministrativo,
                    usuario.Cargo,
                    usuario.CBO,
                    usuario.NumeroMatricula,
                    HoraInicialAcesso = usuario.HoraInicialAcesso.HasValue ? usuario.HoraInicialAcesso.Value.ToString(@"hh\:mm") : string.Empty,
                    HoraFinalAcesso = usuario.HoraFinalAcesso.HasValue ? usuario.HoraFinalAcesso.Value.ToString(@"hh\:mm") : string.Empty,
                    UsuarioAdministradorMobile = usuario.UsuarioAdministradorMobile.HasValue ? usuario.UsuarioAdministradorMobile.Value : true,
                    PerfilAcessoMobile = usuario.PerfilAcessoMobile != null ? new { usuario.PerfilAcessoMobile.Codigo, usuario.PerfilAcessoMobile.Descricao } : null,
                    ModulosUsuarioMobile = (from codigoModuloMobile in usuario.ModulosLiberadosMobile
                                            select new
                                            {
                                                CodigoModulo = codigoModuloMobile
                                            }).ToList(),
                    FotoUsuario = ObterFotoBase64(codigo, unitOfWork),
                    GridUsuarioDadoBancarios = (from obj in usuario.DadosBancarios
                                                select new
                                                {
                                                    obj.Codigo,
                                                    CodigoBanco = obj.Banco != null ? obj.Banco.Codigo : 0,
                                                    DescricaoTipoContaBanco = obj.TipoContaBanco.ObterDescricao(),
                                                    Banco = obj.Banco != null ? obj.Banco.Descricao : string.Empty,
                                                    obj.Agencia,
                                                    obj.DigitoAgencia,
                                                    obj.NumeroConta,
                                                    obj.TipoContaBanco,
                                                    obj.ObservacaoConta
                                                }).ToList(),
                    GridUsuarioContatos = (from obj in usuario.Contatos
                                           select new
                                           {
                                               obj.Codigo,
                                               obj.Email,
                                               obj.Nome,
                                               obj.Telefone,
                                               obj.TipoParentesco,
                                               CPF = obj.CPF_Formatado,
                                               DataNascimento = obj.DataNascimento?.ToString("dd/MM/yyyy") ?? string.Empty
                                           }).ToList(),
                    Empresas = (from obj in usuario.Empresas
                                select new
                                {
                                    obj.Codigo,
                                    CNPJ = obj.CNPJ_Formatado,
                                    obj.RazaoSocial
                                }).ToList(),
                    ProvedoresUsuarios = (from obj in usuario.ClientesProvedores
                                          select new
                                          {
                                              obj.Codigo,
                                              obj.Descricao
                                          }).ToList(),
                    CentrosResultado = (from obj in usuario.CentrosResultado
                                        select new
                                        {
                                            obj.Codigo,
                                            obj.Descricao,
                                            obj.Plano
                                        }).ToList(),
                    usuario.Observacao,
                    usuario.EstadoCivil,
                    LocalidadeNascimento = usuario.LocalidadeNascimento != null ? new { Codigo = usuario.LocalidadeNascimento.Codigo, Descricao = usuario.LocalidadeNascimento.DescricaoCidadeEstado } : null,
                    usuario.TituloEleitoral,
                    usuario.ZonaEleitoral,
                    usuario.SecaoEleitoral,
                    usuario.CorRaca,
                    usuario.Escolaridade,
                    usuario.TipoComercial,
                    usuario.Aposentadoria,
                    EstadoCTPS = new { Codigo = usuario.EstadoCTPS?.Sigla ?? "", Descricao = usuario.EstadoCTPS?.Nome ?? "" },
                    Supervisor = new { Codigo = usuario.Supervisor?.Codigo ?? 0, Descricao = usuario.Supervisor?.Nome ?? "" },
                    Gerente = new { Codigo = usuario.Gerente?.Codigo ?? 0, Descricao = usuario.Gerente?.Nome ?? "" },
                    AreaContainer = new { Codigo = usuario.AreaContainer?.CPF_CNPJ ?? 0, Descricao = usuario.AreaContainer?.Descricao ?? "" },
                    DataExpedicaoCTPS = usuario.DataExpedicaoCTPS?.ToString("dd/MM/yyyy"),
                    usuario.NotificarOcorrenciaEntrega,
                    usuario.LimitarOperacaoPorEmpresa,
                    usuario.FormaAutenticacaoUsuario,
                    usuario.PermiteAssumirOcorrencia,
                    usuario.PermiteInserirDicas,
                    usuario.NotificarEtapasBidding,
                    usuario.PermitirAssumirAtendimentoManeiraSobreposta,
                    usuario.UsuarioUtilizaSegregacaoPorProvedor,
                    GridEPIs = (from epi in funcionarioEPIs
                                select new
                                {
                                    epi.Codigo,
                                    CodigoEPI = epi.EPI?.Codigo ?? 0,
                                    DescricaoEPI = epi.EPI?.Descricao ?? string.Empty,
                                    DataRepasse = epi.DataRepasse?.ToString("dd/MM/yyyy") ?? string.Empty,
                                    SerieEPI = epi.SerieEPI ?? string.Empty,
                                    Quantidade = epi.Quantidade,
                                }).ToList(),
                    GridMetas = (from epi in funcionarioMetas
                                 select new
                                 {
                                     epi.Codigo,
                                     DataInicial = epi.DataInicial.ToString("dd/MM/yyyy"),
                                     DataFinal = epi.DataFinal.ToString("dd/MM/yyyy"),
                                     Ativo = epi.Ativo,
                                     TipoMetaVendaDireta = epi.TipoMetaVendaDireta,
                                     PercentualMeta = epi.PercentualMeta.ToString("n2"),
                                     DescricaoTipoMetaVendaDireta = epi.TipoMetaVendaDireta.ObterDescricao(),
                                     DescricaoAtivo = epi.Ativo ? "Ativo" : "Inativo",
                                     DescricaoVigencia = epi.DataInicial.ToString("dd/MM/yyyy") + " até " + epi.DataFinal.ToString("dd/MM/yyyy")
                                 }).ToList()

                };

                return new JsonpResult(dynUsuario);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoBuscarPorCodigo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarDetalhesOperador()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Operacional.OperadorLogistica repOperadorLogistica = new Repositorio.Embarcador.Operacional.OperadorLogistica(unitOfWork);
                Dominio.Entidades.Usuario usuario = this.Usuario;

                Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistico = repOperadorLogistica.BuscarPorUsuario(usuario.Codigo);
                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                {
                    var dynUsuario = new
                    {
                        usuario.Codigo,
                        usuario.Nome,
                        Operador = operadorLogistico?.Ativo ?? false,
                        OperadorSupervisor = operadorLogistico?.SupervisorLogistica ?? false,
                        PermiteAdicionarComplementosDeFrete = operadorLogistico?.PermiteAdicionarComplementosDeFrete ?? false,
                        PermitirVisualizarValorFreteTransportadoresInteressadosCarga = operadorLogistico?.PermitirVisualizarValorFreteTransportadoresInteressadosCarga ?? false,
                        PermitirAssumirCargasControleEntrega = operadorLogistico?.PermitirAssumirCargasControleEntrega ?? false,
                        ClienteFornecedor = usuario.ClienteFornecedor?.CPF_CNPJ ?? 0,
                        NomeClienteFornecedor = usuario.ClienteFornecedor?.Descricao ?? ""
                    };
                    return new JsonpResult(dynUsuario);
                }
                else
                {
                    var dynUsuario = new
                    {
                        usuario.Codigo,
                        usuario.Nome,
                        Operador = true,
                        OperadorSupervisor = false,
                        PermiteAdicionarComplementosDeFrete = false,
                        PermitirVisualizarValorFreteTransportadoresInteressadosCarga = false
                    };
                    return new JsonpResult(dynUsuario);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Pessoas.Usuario.OcorreuUmaFalhaAoBuscarOsDetalhesDoOperador);
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

                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Embarcador.Operacional.OperadorLogistica repOperadorLogistica = new Repositorio.Embarcador.Operacional.OperadorLogistica(unitOfWork);
                Repositorio.Embarcador.Usuarios.FuncionarioSenhaAnterior repFuncionarioSenhaAnterior = new Repositorio.Embarcador.Usuarios.FuncionarioSenhaAnterior(unitOfWork);
                Repositorio.PaginaUsuario repPaginaUsuario = new Repositorio.PaginaUsuario(unitOfWork);
                Repositorio.Embarcador.Usuarios.FuncionarioEPI repositorioFuncionarioEPI = new Repositorio.Embarcador.Usuarios.FuncionarioEPI(unitOfWork);
                Repositorio.Embarcador.Usuarios.FuncionarioMetaVendaDireta repositorioFuncionarioMetaVendaDireta = new Repositorio.Embarcador.Usuarios.FuncionarioMetaVendaDireta(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorCodigo(codigo);
                Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica = repOperadorLogistica.BuscarPorUsuario(usuario.Codigo);

                List<Dominio.Entidades.PaginaUsuario> paginasUsuario = repPaginaUsuario.BuscarPorUsuario(usuario.Codigo);
                List<Dominio.Entidades.Embarcador.Usuarios.FuncionarioSenhaAnterior> senhas = repFuncionarioSenhaAnterior.BuscarPorUsuario(usuario.Codigo, 0, 100);
                List<Dominio.Entidades.Embarcador.Usuarios.FuncionarioEPI> funcionarioEPIs = repositorioFuncionarioEPI.BuscarPorUsuario(usuario);
                List<Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioMetaVendaDireta> funcionarioMetas = repositorioFuncionarioMetaVendaDireta.BuscarPorUsuario(usuario);

                foreach (Dominio.Entidades.Embarcador.Usuarios.FuncionarioSenhaAnterior senha in senhas)
                    repFuncionarioSenhaAnterior.Deletar(senha);

                foreach (Dominio.Entidades.PaginaUsuario paginaUsuario in paginasUsuario)
                    repPaginaUsuario.Deletar(paginaUsuario);

                foreach (Dominio.Entidades.Embarcador.Usuarios.FuncionarioEPI epi in funcionarioEPIs)
                    repositorioFuncionarioEPI.Deletar(epi);

                foreach (Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioMetaVendaDireta epi in funcionarioMetas)
                    repositorioFuncionarioMetaVendaDireta.Deletar(epi);

                if (operadorLogistica != null)
                    repOperadorLogistica.Deletar(operadorLogistica);

                repUsuario.Deletar(usuario, Auditado);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();

                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelExcluirRegistro);
                else
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExcluir);
                }
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> DadosSetorUsuarioLogado()
        {
            try
            {
                Dominio.Entidades.Usuario usuarioLogado = this.Usuario;

                return new JsonpResult(new
                {
                    Filial = usuarioLogado.Filial == null ? null : new { usuarioLogado.Filial.Codigo, usuarioLogado.Filial.Descricao },
                    Setor = usuarioLogado.Setor == null ? null : new { usuarioLogado.Setor.Codigo, usuarioLogado.Setor.Descricao },
                    Turno = usuarioLogado.Turno == null ? null : new { usuarioLogado.Turno.Codigo, usuarioLogado.Turno.Descricao }
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Pessoas.Usuario.OcorreuUmaFalhaAoBuscarOsDadosDoUsuario);
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> DadosUsuarioLogado()
        {
            try
            {
                Dominio.Entidades.Usuario usuarioLogado = this.Usuario;
                Dominio.Entidades.Empresa empresaLogada = usuarioLogado.Empresa;

                var dadosUsuarioLogado = new
                {
                    usuarioLogado.Codigo,
                    usuarioLogado.Nome,
                    usuarioLogado.PermitirAprovarNaoConformidade,

                    DataAtual = DateTime.Now.Date.ToString("dd/MM/yyyy"),
                    HoraAtual = string.Format("{0:00}:{1:00}", DateTime.Now.Hour, DateTime.Now.Minute),
                    DataHoraAtual = DateTime.Now.Date.ToString("dd/MM/yyyy") + " " + string.Format("{0:00}:{1:00}", DateTime.Now.Hour, DateTime.Now.Minute),

                    PerfilAcessoAdmin = usuarioLogado.PerfilAcesso?.PerfilAdministrador ?? false,
                    usuarioLogado.UsuarioAdministrador,

                    CodigoEmpresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? 0 : empresaLogada?.Codigo,
                    RazaoEmpresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? string.Empty : empresaLogada?.RazaoSocial,

                    TipoLancamentoFinanceiroSemOrcamento = empresaLogada?.TipoLancamentoFinanceiroSemOrcamento ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLancamentoFinanceiroSemOrcamento.Liberar,
                    Filial = new { Codigo = usuarioLogado.Filial?.Codigo ?? 0, Descricao = usuarioLogado.Filial?.Descricao ?? "" },

                    Empresa = new
                    {
                        Codigo = empresaLogada?.Codigo ?? 0,
                        Descricao = empresaLogada?.Descricao ?? "",
                        Matriz = empresaLogada != null && !empresaLogada.Matriz.Any(),

                        UtilizaIntegracaoDocumentosDestinado = empresaLogada?.UtilizaIntegracaoDocumentosDestinado ?? false,
                        HabilitaLancamentoProdutoLote = empresaLogada?.HabilitaLancamentoProdutoLote ?? false
                    }
                };

                return new JsonpResult(dadosUsuarioLogado);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Pessoas.Usuario.OcorreuUmaFalhaAoBuscarOsDadosDoUsuario);
            }
        }

        public async Task<IActionResult> AdicionarFoto()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorCodigo(codigo);

                if (usuario == null)
                    return new JsonpResult(false, true, Localization.Resources.Pessoas.Usuario.UsuarioNaoEncontrado);

                IList<Servicos.DTO.CustomFile> arquivos = HttpContext.GetFiles("ArquivoFoto");

                if (arquivos.Count <= 0)
                    return new JsonpResult(false, true, Localization.Resources.Pessoas.Usuario.NenhumaFotoSelecionadaParaAdicionar);

                Servicos.DTO.CustomFile arquivoFoto = arquivos.FirstOrDefault();
                string extensaoArquivo = System.IO.Path.GetExtension(arquivoFoto.FileName).ToLower();
                string caminho = ObterCaminhoArquivoFoto(unitOfWork);
                string nomeArquivoFotoExistente = Utilidades.IO.FileStorageService.Storage.GetFiles(caminho, $"{codigo}.*").FirstOrDefault();

                if (!string.IsNullOrWhiteSpace(nomeArquivoFotoExistente))
                    Utilidades.IO.FileStorageService.Storage.Delete(nomeArquivoFotoExistente);

                arquivoFoto.SaveAs(Utilidades.IO.FileStorageService.Storage.Combine(caminho, $"{codigo}{extensaoArquivo}"));

                Servicos.Auditoria.Auditoria.Auditar(Auditado, usuario, null, Localization.Resources.Pessoas.Usuario.AlterouFotoDoUsuario, unitOfWork);

                return new JsonpResult(new
                {
                    FotoUsuario = ObterFotoBase64(codigo, unitOfWork)
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Pessoas.Usuario.OcorreuUmaFalhaAoAdicionarFotoDoUsuario);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirFoto()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorCodigo(codigo);

                if (usuario == null)
                    return new JsonpResult(false, true, Localization.Resources.Pessoas.Usuario.UsuarioNaoEncontrado);

                string caminho = ObterCaminhoArquivoFoto(unitOfWork);
                string nomeArquivo = Utilidades.IO.FileStorageService.Storage.GetFiles(caminho, $"{codigo}.*").FirstOrDefault();

                if (string.IsNullOrWhiteSpace(nomeArquivo))
                    return new JsonpResult(false, true, Localization.Resources.Pessoas.Usuario.FotoNaoEncontrada);
                else
                    Utilidades.IO.FileStorageService.Storage.Delete(nomeArquivo);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, usuario, null, Localization.Resources.Pessoas.Usuario.RemoveuFoto, unitOfWork);

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Pessoas.Usuario.OcorreuUmaFalhaAoRemoverFotoDoUsuario);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConfiguracaoImportacao()
        {
            List<ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoUsuario();

            return new JsonpResult(configuracoes.ToList());
        }

        public async Task<IActionResult> Importar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(unitOfWork);

                Servicos.Embarcador.Pessoa.PoliticaSenha serPoliticaSenha = new Servicos.Embarcador.Pessoa.PoliticaSenha();

                Dominio.Entidades.Embarcador.Usuarios.PoliticaSenha politicaSenha = serPoliticaSenha.BuscarPoliticaSenha(unitOfWork, TipoServicoMultisoftware);

                unitOfWork.Start();

                List<Dominio.Entidades.Usuario> usuarios = new List<Dominio.Entidades.Usuario>();

                int importados = 0;

                RetornoImportacao retorno = Servicos.Embarcador.Importacao.Importacao.PreencherImportacaoManual(Request, usuarios, ((dados) =>
                {
                    Servicos.UsuarioImportacao servicoImportacaoUsuario = new Servicos.UsuarioImportacao(unitOfWork, TipoServicoMultisoftware, dados, this.Empresa, politicaSenha);

                    importados++;

                    return servicoImportacaoUsuario.ObterUsuarioImportar();
                }));

                if (retorno == null)
                    return new JsonpResult(false, "Ocorreu uma falha ao Importar o Arquivo.");

                foreach (Dominio.Entidades.Usuario usuario in usuarios)
                    repositorioUsuario.Inserir(usuario);


                unitOfWork.CommitChanges();

                // o correto aqui é a implementação do pattern "outbox pattern" o ideal é colocar isso talvez em uma fila etc e tals.
                if (importados > 0 && usuarios.Count > 0)
                {
                    var usuariosUnicos = usuarios
                        .GroupBy(u => new { u.Email, u.Login })
                        .Select(g => g.First())
                        .ToList();

                    var notifications = usuariosUnicos.Select(usuario => new UsuarioImportadoNotification(
                        usuario.Nome,
                        usuario.Email,
                        usuario.Senha,
                        usuario.Login
                    )).ToArray();

                    await EnviarNotificacaoUsuarioImportadoAsync(notifications);
                }

                retorno.Importados = importados;

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao Importar o Arquivo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        #endregion

        #region Métodos Privados

        private bool ClienteExigeContraSenha()
        {
            return (IsHomologacao ? Cliente.ClienteConfiguracaoHomologacao : Cliente.ClienteConfiguracao)?.ExigeContraSenha ?? false;
        }

        private async Task EnviarNotificacaoUsuarioImportadoAsync(params UsuarioImportadoNotification[] notifications)
        {
            var tasks = notifications.Select(notification =>
            {
                System.Diagnostics.Debug.WriteLine($"Publishing notification for user: {notification.NomeUsuario}");
                return _mediator.Publish(notification);
            });

            await Task.WhenAll(tasks);
        }

        private void PreencherUsuario(Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

            Repositorio.Embarcador.Financeiro.PlanoConta repPlanoConta = new Repositorio.Embarcador.Financeiro.PlanoConta(unitOfWork);
            Repositorio.Embarcador.Usuarios.PerfilAcesso repPerfilAcesso = new Repositorio.Embarcador.Usuarios.PerfilAcesso(unitOfWork);
            Repositorio.Embarcador.Usuarios.PerfilAcessoMobile repPerfilAcessoMobile = new Repositorio.Embarcador.Usuarios.PerfilAcessoMobile(unitOfWork);
            Repositorio.Setor repSetor = new Repositorio.Setor(unitOfWork);
            Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
            Repositorio.Embarcador.Filiais.Turno repositorioTurno = new Repositorio.Embarcador.Filiais.Turno(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Pessoas.Cargo repCargo = new Repositorio.Embarcador.Pessoas.Cargo(unitOfWork);
            Repositorio.Embarcador.Logistica.CentroCustoViagem repCentroDeCustoSetorTurno = new Repositorio.Embarcador.Logistica.CentroCustoViagem(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repConfiguracao.BuscarConfiguracaoPadrao();

            usuario.HoraInicialAcesso = Request.GetNullableTimeParam("HoraInicialAcesso");
            usuario.HoraFinalAcesso = Request.GetNullableTimeParam("HoraFinalAcesso");
            usuario.UsuarioAdministrador = Request.GetBoolParam("UsuarioAdministrador");
            usuario.PermiteSalvarNovoRelatorio = Request.GetBoolParam("PermiteSalvarNovoRelatorio");
            usuario.PermiteTornarRelatorioPadrao = Request.GetBoolParam("PermiteTornarRelatorioPadrao");
            usuario.PermiteSalvarConfiguracoesRelatoriosParaTodos = Request.GetBoolParam("PermiteSalvarConfiguracoesRelatoriosParaTodos");
            usuario.RegiaoAcessoDashOperadorNorte = Request.GetBoolParam("RegiaoNorte");
            usuario.RegiaoAcessoDashOperadorNordeste = Request.GetBoolParam("RegiaoNordeste");
            usuario.RegiaoAcessoDashOperadorSul = Request.GetBoolParam("RegiaoSul");
            usuario.RegiaoAcessoDashOperadorSudeste = Request.GetBoolParam("RegiaoSudeste");
            usuario.RegiaoAcessoDashOperadorCentroOeste = Request.GetBoolParam("RegiaoCentroOeste");

            if ((usuario.HoraInicialAcesso > TimeSpan.MinValue && usuario.HoraFinalAcesso == TimeSpan.MinValue) || (usuario.HoraInicialAcesso == TimeSpan.MinValue && usuario.HoraFinalAcesso > TimeSpan.MinValue))
                throw new ControllerException(Localization.Resources.Pessoas.Usuario.FavorInformarHorarioDeAcessoInicialFinalOuRemover);

            int codigoPerfilAcesso = Request.GetIntParam("PerfilAcesso");
            usuario.PerfilAcesso = repPerfilAcesso.BuscarPorCodigo(codigoPerfilAcesso);

            if (usuario.PerfilAcesso == null && !usuario.UsuarioAdministrador && configuracaoEmbarcador.ExigePerfilUsuario)
                throw new ControllerException(Localization.Resources.Pessoas.Usuario.ObrigatorioInformarUmPerfilDeUsuario);

            usuario.TipoPessoa = ConverteTipoPessoaEnumeradorParaString(Request.GetEnumParam<TipoPessoaCadastro>("TipoPessoa"));

            int codigoPerfilAcessoMobile = Request.GetIntParam("PerfilAcessoMobile");
            int codigoPlanoConta = Request.GetIntParam("PlanoConta");
            int codigoSetorFuncionario = Request.GetIntParam("SetorFuncionario");
            int codigoTipoMovimentoComissao = Request.GetIntParam("TipoMovimentoComissao");
            int codigoCargoSetorTurno = Request.GetIntParam("CargoSetorTurno");
            double cnpjCliente = Request.GetDoubleParam("Cliente");

            usuario.Nome = Request.Params("Nome");
            usuario.RG = Request.Params("RG");
            usuario.DataAdmissao = Request.GetNullableDateTimeParam("DataAdmissao");
            usuario.DataFimPeriodoExperiencia = Request.GetNullableDateTimeParam("DataFimPeriodoExperiencia");
            usuario.DataDemissao = Request.GetNullableDateTimeParam("DataDemissao");
            usuario.DataNascimento = Request.GetNullableDateTimeParam("DataNascimento");
            usuario.UsuarioAdministradorMobile = Request.GetBoolParam("UsuarioAdministradorMobile");
            usuario.UsuarioAtendimento = Request.GetBoolParam("UsuarioAtendimento");
            usuario.UsuarioCallCenter = Request.GetBoolParam("UsuarioCallCenter");
            usuario.PermitirAprovarNaoConformidade = Request.GetBoolParam("PermitirAprovarNaoConformidade");

            int codigoCentroDeCustoSetorTurno = Request.GetIntParam("CentroDeCustoSetorTurno");

            if (usuario.UsuarioAtendimento)
            {
                Dominio.Entidades.Usuario usuarioAtendimento = repUsuario.BuscarPorUsuarioAtendimento();
                if (usuarioAtendimento != null && usuarioAtendimento.Codigo != usuario.Codigo)
                    throw new ControllerException("já existe um usuário atendimento cadastrado na base.");
            }

            if (usuario.UsuarioCallCenter)
            {
                Dominio.Entidades.Usuario usuarioCallcenter = repUsuario.BuscarPorUsuarioCallCenter();
                if (usuarioCallcenter != null && usuarioCallcenter.Codigo != usuario.Codigo)
                    throw new ControllerException("já existe um usuário callcenter cadastrado na base.");
            }

            usuario.PerfilAcessoMobile = codigoPerfilAcessoMobile > 0 ? repPerfilAcessoMobile.BuscarPorCodigo(codigoPerfilAcessoMobile) : null;
            usuario.Filial = repositorioFilial.BuscarPorCodigo(Request.GetIntParam("Filial"));
            usuario.Setor = codigoSetorFuncionario > 0 ? repSetor.BuscarPorCodigo(codigoSetorFuncionario) : new Dominio.Entidades.Setor() { Codigo = 1 };
            usuario.Turno = repositorioTurno.BuscarPorCodigo(Request.GetIntParam("Turno"));
            usuario.NotificadoExpedicao = Request.GetBoolParam("NotificadoExpedicao");
            usuario.EnviarNotificacaoPorEmail = Request.GetBoolParam("NotificacaoPorEmail");
            usuario.VisualizarGraficosIniciais = Request.GetBoolParam("VisualizarGraficosIniciais");
            usuario.VisualizarTitulosPagamentoSalario = Request.GetBoolParam("VisualizarTitulosPagamentoSalario");
            usuario.Localidade = !string.IsNullOrWhiteSpace(Request.Params("Localidade")) && Request.Params("Localidade") != "0" ? new Dominio.Entidades.Localidade() { Codigo = int.Parse(Request.Params("Localidade")) } : this.Empresa?.Localidade != null ? new Dominio.Entidades.Localidade() { Codigo = this.Empresa.Localidade.Codigo } : null;
            usuario.Endereco = Request.Params("Endereco");
            usuario.Complemento = Request.Params("Complemento");
            usuario.Bairro = Request.Params("Bairro");
            usuario.CEP = Utilidades.String.OnlyNumbers(Request.Params("CEP"));
            usuario.NumeroEndereco = Request.Params("NumeroEndereco");
            usuario.TipoLogradouro = Request.GetEnumParam<TipoLogradouro>("TipoLogradouro");
            usuario.EnderecoDigitado = bool.Parse(Request.Params("EnderecoDigitado"));
            usuario.Latitude = Request.Params("Latitude");
            usuario.Longitude = Request.Params("Longitude");
            usuario.Salario = !string.IsNullOrEmpty(Request.Params("Salario")) ? decimal.Parse(Request.Params("Salario")) : 0;
            usuario.Telefone = Request.Params("Telefone");
            usuario.Email = Request.Params("Email").ToLower();
            usuario.AssociarUsuarioCliente = Request.GetBoolParam("AssociarUsuarioCliente");
            usuario.Cliente = cnpjCliente > 0 ? repCliente.BuscarPorCPFCNPJ(cnpjCliente) : null;
            usuario.PlanoConta = codigoPlanoConta > 0 ? repPlanoConta.BuscarPorCodigo(codigoPlanoConta) : null;
            usuario.PermiteAuditar = Request.GetBoolParam("LiberarAuditoria");
            usuario.ExibirUsuarioAprovacao = Request.GetBoolParam("ExibirUsuarioAprovacao");
            usuario.NumeroCTPS = Request.Params("NumeroCTPS");
            usuario.SerieCTPS = Request.Params("SerieCTPS");
            usuario.TipoMovimentoComissao = codigoTipoMovimentoComissao > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoComissao) : null;
            usuario.DiaComissao = Request.GetIntParam("DiaComissao");
            usuario.CodigoIntegracao = Request.Params("CodigoIntegracao");
            usuario.PISAdministrativo = Request.Params("PISAdministrativo");
            usuario.Cargo = Request.Params("Cargo");
            usuario.CBO = Request.Params("CBO");
            usuario.NumeroMatricula = Request.Params("NumeroMatricula");
            usuario.NotificarOcorrenciaEntrega = Request.GetBoolParam("NotificarOcorrenciaEntrega");
            usuario.LimitarOperacaoPorEmpresa = Request.GetBoolParam("LimitarOperacaoPorEmpresa");
            usuario.CargoSetorTurno = codigoCargoSetorTurno > 0 ? repCargo.BuscarPorCodigo(codigoCargoSetorTurno) : null;
            usuario.NivelEscalationList = Request.GetNullableEnumParam<EscalationList>("NivelEscalationList");
            usuario.CentroDeCustoSetorTurno = codigoCentroDeCustoSetorTurno > 0 ? repCentroDeCustoSetorTurno.BuscarPorCodigo(codigoCentroDeCustoSetorTurno) : null;

            SalvarEmpresas(usuario, unitOfWork);
            SalvarCentrosResultado(usuario, unitOfWork);
            SalvarProvedoresUsuario(usuario, unitOfWork);
        }

        private void PreencherDadosAdicionaisUsuario(Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
            Repositorio.Estado repEstado = new Repositorio.Estado(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);

            int codigoLocalidadeNascimento = Request.GetIntParam("LocalidadeNascimento");
            int codigoGerente = Request.GetIntParam("Gerente");
            int codigoSupervisor = Request.GetIntParam("Supervisor");
            double cpfCnpjAreaContainer = Request.GetDoubleParam("AreaContainer");
            string estadoCTPS = Request.GetStringParam("EstadoCTPS");

            usuario.Observacao = Request.GetStringParam("Observacao");
            usuario.TituloEleitoral = Request.GetStringParam("TituloEleitoral");
            usuario.ZonaEleitoral = Request.GetStringParam("ZonaEleitoral");
            usuario.SecaoEleitoral = Request.GetStringParam("SecaoEleitoral");
            usuario.DataExpedicaoCTPS = Request.GetNullableDateTimeParam("DataExpedicaoCTPS");
            usuario.PermiteAssumirOcorrencia = Request.GetBoolParam("PermiteAssumirOcorrencia");

            usuario.EstadoCivil = Request.GetNullableEnumParam<EstadoCivil>("EstadoCivil");
            usuario.CorRaca = Request.GetNullableEnumParam<CorRaca>("CorRaca");
            usuario.Escolaridade = Request.GetNullableEnumParam<Escolaridade>("Escolaridade");
            usuario.TipoComercial = Request.GetNullableEnumParam<TipoComercial>("TipoComercial");
            usuario.Aposentadoria = Request.GetNullableEnumParam<Aposentadoria>("Aposentadoria");
            usuario.FormaAutenticacaoUsuario = Request.GetNullableEnumParam<FormaAutenticacaoUsuario>("FormaAutenticacaoUsuario");
            usuario.AreaContainer = cpfCnpjAreaContainer > 0 ? repositorioCliente.BuscarPorCPFCNPJ(cpfCnpjAreaContainer) : null;

            usuario.Gerente = codigoGerente > 0 ? repUsuario.BuscarPorCodigo(codigoGerente) : null;
            usuario.Supervisor = codigoSupervisor > 0 ? repUsuario.BuscarPorCodigo(codigoSupervisor) : null;

            usuario.LocalidadeNascimento = codigoLocalidadeNascimento > 0 ? repLocalidade.BuscarPorCodigo(codigoLocalidadeNascimento) : null;
            usuario.EstadoCTPS = !string.IsNullOrWhiteSpace(estadoCTPS) ? repEstado.BuscarPorSigla(estadoCTPS) : null;
            usuario.PermiteInserirDicas = Request.GetBoolParam("PermiteInserirDicas");
            usuario.NotificarEtapasBidding = Request.GetBoolParam("NotificarEtapasBidding");
            usuario.PermitirAssumirAtendimentoManeiraSobreposta = Request.GetBoolParam("PermitirAssumirAtendimentoManeiraSobreposta");
            usuario.UsuarioUtilizaSegregacaoPorProvedor = Request.GetBoolParam("UsuarioUtilizaSegregacaoPorProvedor");
        }

        private Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaUsuario ObterFiltrosPesquisa()
        {
            Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaUsuario filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaUsuario()
            {
                Nome = Request.GetStringParam("Nome"),
                Usuario = Request.GetStringParam("Usuario"),
                TipoPessoa = ConverteTipoPessoaEnumeradorParaString(Request.GetEnumParam<TipoPessoaCadastro>("TipoPessoa")),
                CpfCnpj = Request.GetStringParam("CPF").ObterSomenteNumeros(),
                PerfilAcesso = Request.GetIntParam("PerfilAcesso"),
                IgnorarSituacaoMotorista = Request.GetBoolParam("IgnorarSituacaoMotorista"),
                SituacaoColaborador = Request.GetEnumParam<SituacaoColaborador>("SituacaoColaborador"),
                TipoUsuario = Request.GetEnumParam<TipoUsuario>("Tipo"),
                Ativo = Request.GetEnumParam("Ativo", SituacaoAtivoPesquisa.Ativo),
                CodigoEmpresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? Empresa.Codigo : 0,
                TipoCargoFuncionario = ConfiguracaoEmbarcador.ValidarCargoConsultaFuncionario ? Request.GetListEnumParam<TipoCargoFuncionario>("TipoCargoFuncionario") : null,
                UsuarioMultisoftware = this.Usuario.UsuarioMultisoftware,

                TipoComercial = Request.GetEnumParam<TipoComercial>("TipoComercial"),
                NumeroMatricula = Request.GetStringParam("NumeroMatricula"),
                CodigoIntegracao = Request.GetStringParam("CodigoIntegracao"),
                Localidade = Request.GetIntParam("Localidade"),
            };

            bool retornarTodosTiposAcesso = Request.GetBoolParam("RetornarTodosTiposAcesso");

            Dominio.Enumeradores.TipoAcesso? tipoAcesso = null;
            if (!retornarTodosTiposAcesso &&
                (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe) &&
                (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS))
                tipoAcesso = this.Usuario.TipoAcesso;

            if (filtrosPesquisa.Ativo == SituacaoAtivoPesquisa.Ativo)
                filtrosPesquisa.Status = "A";
            else if (filtrosPesquisa.Ativo == SituacaoAtivoPesquisa.Inativo)
                filtrosPesquisa.Status = "I";

            filtrosPesquisa.TipoAcesso = tipoAcesso;

            return filtrosPesquisa;
        }

        private void SalvarLicencas(Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Transportadores.MotoristaLicenca repLicencaMotorista = new Repositorio.Embarcador.Transportadores.MotoristaLicenca(unitOfWork);
            Repositorio.Embarcador.Configuracoes.Licenca repLicenca = new Repositorio.Embarcador.Configuracoes.Licenca(unitOfWork);

            dynamic dynLicencas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("GridUsuarioLicencas"));

            if (usuario.Licencas?.Count > 0)
            {
                List<int> codigos = new List<int>();
                foreach (dynamic licenca in dynLicencas)
                    if (licenca.Codigo != null)
                        codigos.Add((int)licenca.Codigo);

                List<Dominio.Entidades.Embarcador.Transportadores.MotoristaLicenca> licencasDeletar = (from obj in usuario.Licencas where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < licencasDeletar.Count; i++)
                    repLicencaMotorista.Deletar(licencasDeletar[i]);
            }
            else
                usuario.Licencas = new List<Dominio.Entidades.Embarcador.Transportadores.MotoristaLicenca>();

            foreach (dynamic dynLicenca in dynLicencas)
            {
                int codigoLicencaMotorista = ((string)dynLicenca.Codigo).ToInt();

                Dominio.Entidades.Embarcador.Transportadores.MotoristaLicenca licenca = codigoLicencaMotorista > 0 ? repLicencaMotorista.BuscarPorCodigo(codigoLicencaMotorista, false) : null;
                if (licenca == null)
                    licenca = new Dominio.Entidades.Embarcador.Transportadores.MotoristaLicenca();

                int.TryParse((string)dynLicenca.CodigoLicenca, out int codigoLicenca);
                DateTime.TryParse((string)dynLicenca.DataEmissao, out DateTime dataEmissao);
                DateTime.TryParse((string)dynLicenca.DataVencimento, out DateTime dataVencimento);
                Enum.TryParse((string)dynLicenca.Status, out StatusLicenca status);

                licenca.Licenca = codigoLicenca > 0 ? repLicenca.BuscarPorCodigo(codigoLicenca) : null;
                licenca.DataEmissao = dataEmissao;
                licenca.DataVencimento = dataVencimento;
                licenca.Descricao = (string)dynLicenca.Descricao;
                licenca.Numero = (string)dynLicenca.Numero;
                licenca.Motorista = usuario;
                licenca.Status = status;

                dynamic dynFormasAlerta = JsonConvert.DeserializeObject<dynamic>((string)dynLicenca.FormaAlerta);
                licenca.FormasAlerta = new List<ControleAlertaForma>();
                if (dynFormasAlerta?.Count > 0)
                {
                    foreach (dynamic codigoFormaAlerta in dynFormasAlerta)
                        licenca.FormasAlerta.Add(((string)codigoFormaAlerta).ToEnum<ControleAlertaForma>());
                }

                if (licenca.Codigo > 0)
                    repLicencaMotorista.Atualizar(licenca);
                else
                    repLicencaMotorista.Inserir(licenca);
            }
        }

        private void SalvarDadosBancarios(Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Banco repBanco = new Repositorio.Banco(unitOfWork);
            Repositorio.Embarcador.Transportadores.MotoristaDadoBancario repMotoristaDadoBancario = new Repositorio.Embarcador.Transportadores.MotoristaDadoBancario(unitOfWork);

            dynamic dynDadosBancarios = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("GridUsuarioDadoBancarios"));

            if (usuario.DadosBancarios != null && usuario.DadosBancarios.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var dadoBancario in dynDadosBancarios)
                    if (dadoBancario.Codigo != null)
                        codigos.Add((int)dadoBancario.Codigo);

                List<Dominio.Entidades.Embarcador.Transportadores.MotoristaDadoBancario> dadoBancarioMotoristaDeletar = (from obj in usuario.DadosBancarios where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < dadoBancarioMotoristaDeletar.Count; i++)
                    repMotoristaDadoBancario.Deletar(dadoBancarioMotoristaDeletar[i], Auditado);
            }
            else
                usuario.DadosBancarios = new List<Dominio.Entidades.Embarcador.Transportadores.MotoristaDadoBancario>();

            foreach (var dadoBancario in dynDadosBancarios)
            {
                Dominio.Entidades.Embarcador.Transportadores.MotoristaDadoBancario usuarioDadoBancario = dadoBancario.Codigo != null ? repMotoristaDadoBancario.BuscarPorCodigo((int)dadoBancario.Codigo, true) : null;
                if (usuarioDadoBancario == null)
                    usuarioDadoBancario = new Dominio.Entidades.Embarcador.Transportadores.MotoristaDadoBancario();

                int.TryParse((string)dadoBancario.CodigoBanco, out int codigoBanco);
                Enum.TryParse((string)dadoBancario.TipoContaBanco, out TipoContaBanco tipoContaBanco);

                usuarioDadoBancario.Agencia = (string)dadoBancario.Agencia;
                usuarioDadoBancario.DigitoAgencia = (string)dadoBancario.DigitoAgencia;
                usuarioDadoBancario.NumeroConta = (string)dadoBancario.NumeroConta;
                usuarioDadoBancario.ObservacaoConta = (string)dadoBancario.ObservacaoConta;
                usuarioDadoBancario.Motorista = usuario;
                usuarioDadoBancario.TipoContaBanco = tipoContaBanco;

                if (codigoBanco > 0)
                    usuarioDadoBancario.Banco = repBanco.BuscarPorCodigo(codigoBanco);
                else
                    usuarioDadoBancario.Banco = null;

                if (usuarioDadoBancario.Codigo > 0)
                    repMotoristaDadoBancario.Atualizar(usuarioDadoBancario, Auditado);
                else
                    repMotoristaDadoBancario.Inserir(usuarioDadoBancario, Auditado);
            }
        }

        private string ObterCaminhoArquivoFoto(Repositorio.UnitOfWork unitOfWork)
        {
            if (string.IsNullOrEmpty(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos))
                return string.Empty;

            return Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Foto", "Motorista" });

        }

        private string ObterFotoBase64(int codigoUsuario, Repositorio.UnitOfWork unitOfWork)
        {
            string caminho = ObterCaminhoArquivoFoto(unitOfWork);

            if (string.IsNullOrEmpty(caminho))
                return string.Empty;

            Utilidades.IO.FileStorageService.Storage.CreateIfNotExists(caminho);

            string nomeArquivo = Utilidades.IO.FileStorageService.Storage.GetFiles(caminho, $"{codigoUsuario}.*").FirstOrDefault();

            if (string.IsNullOrWhiteSpace(nomeArquivo))
                return "";

            byte[] imageArray = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(nomeArquivo);
            string base64ImageRepresentation = Convert.ToBase64String(imageArray);

            return base64ImageRepresentation;
        }

        private void SalvarContatos(Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Banco repBanco = new Repositorio.Banco(unitOfWork);
            Repositorio.Embarcador.Usuarios.FuncionarioContato repFuncionarioContato = new Repositorio.Embarcador.Usuarios.FuncionarioContato(unitOfWork);

            dynamic dynContatos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("GridUsuarioContatos"));

            if (usuario.Contatos != null && usuario.Contatos.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var dadoContato in dynContatos)
                    if (dadoContato.Codigo != null)
                        codigos.Add((int)dadoContato.Codigo);

                List<Dominio.Entidades.Embarcador.Usuarios.FuncionarioContato> contatoMotoristaDeletar = (from obj in usuario.Contatos where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < contatoMotoristaDeletar.Count; i++)
                    repFuncionarioContato.Deletar(contatoMotoristaDeletar[i], Auditado);
            }
            else
                usuario.Contatos = new List<Dominio.Entidades.Embarcador.Usuarios.FuncionarioContato>();

            foreach (var contato in dynContatos)
            {
                Dominio.Entidades.Embarcador.Usuarios.FuncionarioContato usuarioContato = contato.Codigo != null ? repFuncionarioContato.BuscarPorCodigo((int)contato.Codigo, true) : null;
                if (usuarioContato == null)
                    usuarioContato = new Dominio.Entidades.Embarcador.Usuarios.FuncionarioContato();

                Enum.TryParse((string)contato.TipoParentesco, out TipoParentesco tipoParentesco);

                usuarioContato.Nome = (string)contato.Nome;
                usuarioContato.Email = (string)contato.Email;
                usuarioContato.Telefone = (string)contato.Telefone;
                usuarioContato.Usuario = usuario;
                usuarioContato.TipoParentesco = tipoParentesco;
                usuarioContato.CPF = ((string)contato.CPF).ObterSomenteNumeros();
                usuarioContato.DataNascimento = ((string)contato.DataNascimento).ToNullableDateTime();

                if (usuarioContato.Codigo > 0)
                    repFuncionarioContato.Atualizar(usuarioContato, Auditado);
                else
                    repFuncionarioContato.Inserir(usuarioContato, Auditado);
            }
        }

        private void SalvarEmpresas(Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

            if (usuario.Empresas?.Count > 0)
                usuario.Empresas.Clear();
            else
                usuario.Empresas = new List<Dominio.Entidades.Empresa>();

            dynamic dynEmpresas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("Empresas"));
            foreach (dynamic dynEmpresa in dynEmpresas)
                usuario.Empresas.Add(repEmpresa.BuscarPorCodigo((int)dynEmpresa.Codigo));

            if (usuario.LimitarOperacaoPorEmpresa && usuario.Empresas.Count == 0)
                throw new ControllerException(Localization.Resources.Pessoas.Usuario.ObrigatorioInformarPeloMenosUmaEmpresaQuandoUsarOpcaoDeLimiteDeOperacaoPorEmpresaFilial);
        }

        private void SalvarProvedoresUsuario(Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);

            if (usuario.ClientesProvedores?.Count > 0)
                usuario.ClientesProvedores.Clear();
            else
                usuario.ClientesProvedores = new List<Dominio.Entidades.Cliente>();

            dynamic dynProvedores = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ProvedoresUsuarios"));
            foreach (dynamic dynProvedor in dynProvedores)
                usuario.ClientesProvedores.Add(repositorioCliente.BuscarPorCPFCNPJ((double)dynProvedor.Codigo));
        }

        private void SalvarClientesSetor(Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

            if (usuario.ClientesSetor?.Count > 0)
                usuario.ClientesSetor.Clear();
            else
                usuario.ClientesSetor = new List<Dominio.Entidades.Cliente>();

            List<double> cnpjClientesSetor = Request.GetListParam<double>("ClienteSetor");

            foreach (double cnpjCliente in cnpjClientesSetor)
            {
                var cliente = repCliente.BuscarPorCPFCNPJ(cnpjCliente);
                usuario.ClientesSetor.Add(cliente);
            }

            repUsuario.Atualizar(usuario);
        }

        private void SalvarRepresentacoes(Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

            dynamic dynRepresentacoes = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("GridRepresentacoes"));
            if (usuario.Representacoes == null)
                usuario.Representacoes = new List<Dominio.Entidades.Cliente>();

            usuario.Representacoes.Clear();

            foreach (var dadoRepresentacao in dynRepresentacoes)
            {
                if (dadoRepresentacao.CPF_CNPJ != null)
                {

                    var codigo = Utilidades.String.OnlyNumbers((string)dadoRepresentacao.CPF_CNPJ).ToLong();
                    var cliente = repCliente.BuscarPorCPFCNPJ(codigo);
                    usuario.Representacoes.Add(cliente);
                }
            }
            repUsuario.Atualizar(usuario);
        }

        private void SalvarCentrosResultado(Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unitOfWork);

            if (usuario.CentrosResultado == null)
                usuario.CentrosResultado = new List<Dominio.Entidades.Embarcador.Financeiro.CentroResultado>();
            else
                usuario.CentrosResultado.Clear();

            dynamic dynCentrosResultado = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("CentrosResultado"));
            foreach (dynamic dynCentroResultado in dynCentrosResultado)
            {
                Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultado = repCentroResultado.BuscarPorCodigo((int)dynCentroResultado.Codigo);
                usuario.CentrosResultado.Add(centroResultado);
            }
        }

        private string ConverteTipoPessoaEnumeradorParaString(TipoPessoaCadastro tipoPessoa)
        {
            return (tipoPessoa == TipoPessoaCadastro.Fisica) ? "F" : (tipoPessoa == TipoPessoaCadastro.Juridica) ? "J" : (tipoPessoa == TipoPessoaCadastro.Exterior) ? "E" : "";
        }

        private TipoPessoaCadastro ConverteTipoPessoaParaStringParaEnumerador(string tipoPessoa)
        {
            return (tipoPessoa == "F") ? TipoPessoaCadastro.Fisica : (tipoPessoa == "J") ? TipoPessoaCadastro.Juridica : (tipoPessoa == "E") ? TipoPessoaCadastro.Exterior : TipoPessoaCadastro.Todas;
        }

        private void SalvarEPIs(Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Usuarios.FuncionarioEPI repositorioFuncionarioEPI = new Repositorio.Embarcador.Usuarios.FuncionarioEPI(unitOfWork);
            Repositorio.Embarcador.Pessoas.EPI repositorioEPI = new Repositorio.Embarcador.Pessoas.EPI(unitOfWork);

            List<Dominio.Entidades.Embarcador.Usuarios.FuncionarioEPI> funcionarioEPIs = repositorioFuncionarioEPI.BuscarPorUsuario(usuario);

            dynamic dynEPIs = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("GridEPIs"));

            if (funcionarioEPIs.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var dadoContato in dynEPIs)
                    if (dadoContato.Codigo != null)
                        codigos.Add((int)dadoContato.Codigo);

                List<Dominio.Entidades.Embarcador.Usuarios.FuncionarioEPI> epiFuncionarioDeletar = (from obj in funcionarioEPIs where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < epiFuncionarioDeletar.Count; i++)
                    repositorioFuncionarioEPI.Deletar(epiFuncionarioDeletar[i], Auditado);
            }
            else
                funcionarioEPIs = new List<Dominio.Entidades.Embarcador.Usuarios.FuncionarioEPI>();

            foreach (var epi in dynEPIs)
            {
                Dominio.Entidades.Embarcador.Usuarios.FuncionarioEPI funcionarioEPI = epi.Codigo != null ? repositorioFuncionarioEPI.BuscarPorCodigo((int)epi.Codigo, true) : null;
                if (funcionarioEPI == null)
                    funcionarioEPI = new Dominio.Entidades.Embarcador.Usuarios.FuncionarioEPI();

                int.TryParse((string)epi.CodigoEPI, out int codigoEPI);
                int.TryParse((string)epi.Quantidade, out int quantidade);
                DateTime.TryParse((string)epi.DataRepasse, out DateTime dataRepasse);

                funcionarioEPI.EPI = codigoEPI > 0 ? repositorioEPI.BuscarPorCodigo(codigoEPI, false) : null;
                funcionarioEPI.Usuario = usuario;
                funcionarioEPI.DataRepasse = dataRepasse;
                funcionarioEPI.SerieEPI = (string)epi.SerieEPI;
                funcionarioEPI.Quantidade = quantidade;

                if (funcionarioEPI.Codigo > 0)
                    repositorioFuncionarioEPI.Atualizar(funcionarioEPI, Auditado);
                else
                    repositorioFuncionarioEPI.Inserir(funcionarioEPI, Auditado);
            }
        }
        private void SalvarMeta(Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Usuarios.FuncionarioMetaVendaDireta repositorioFuncionarioMetaVendaDireta = new Repositorio.Embarcador.Usuarios.FuncionarioMetaVendaDireta(unitOfWork);

            List<Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioMetaVendaDireta> funcionarioMetas = repositorioFuncionarioMetaVendaDireta.BuscarPorUsuario(usuario);

            dynamic dynMetas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("GridMetas"));

            if (funcionarioMetas.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var dadoContato in dynMetas)
                    if (dadoContato.Codigo != null)
                        codigos.Add((int)dadoContato.Codigo);

                List<Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioMetaVendaDireta> metaFuncionarioDeletar = (from obj in funcionarioMetas where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < metaFuncionarioDeletar.Count; i++)
                    repositorioFuncionarioMetaVendaDireta.Deletar(metaFuncionarioDeletar[i], Auditado);
            }
            else
                funcionarioMetas = new List<Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioMetaVendaDireta>();

            foreach (var meta in dynMetas)
            {
                Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioMetaVendaDireta funcionarioMeta = meta.Codigo != null ? repositorioFuncionarioMetaVendaDireta.BuscarPorCodigo((int)meta.Codigo, true) : null;
                if (funcionarioMeta == null)
                    funcionarioMeta = new Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioMetaVendaDireta();

                decimal.TryParse((string)meta.PercentualMeta, out decimal percentualMeta);
                DateTime.TryParse((string)meta.DataInicial, out DateTime dataInicial);
                DateTime.TryParse((string)meta.DataFinal, out DateTime dataFinal);
                bool.TryParse((string)meta.Ativo, out bool ativo);
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMetaVendaDireta tipoMetaVendaDireta = TipoMetaVendaDireta.Validacao;
                Enum.TryParse(Request.Params("TipoMetaVendaDireta"), out tipoMetaVendaDireta);

                funcionarioMeta.Funcionario = usuario;
                funcionarioMeta.DataInicial = dataInicial;
                funcionarioMeta.DataFinal = dataFinal;
                funcionarioMeta.PercentualMeta = percentualMeta;
                funcionarioMeta.Ativo = ativo;
                funcionarioMeta.TipoMetaVendaDireta = tipoMetaVendaDireta;
                funcionarioMeta.DataGeracao = DateTime.Now;

                if (funcionarioMeta.Codigo > 0)
                    repositorioFuncionarioMetaVendaDireta.Atualizar(funcionarioMeta, Auditado);
                else
                    repositorioFuncionarioMetaVendaDireta.Inserir(funcionarioMeta, Auditado);
            }
        }

        private List<ConfiguracaoImportacao> ConfiguracaoImportacaoUsuario()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            var configuracoes = new List<ConfiguracaoImportacao>();
            int tamanho = 150;

            configuracoes.Add(new ConfiguracaoImportacao() { Id = 1, Descricao = "Nome", Propriedade = "Nome", Tamanho = tamanho, CampoInformacao = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 2, Descricao = "CPF", Propriedade = "CPF", Tamanho = tamanho, CampoInformacao = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 3, Descricao = "Email", Propriedade = "Email", Tamanho = tamanho, CampoInformacao = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 4, Descricao = "Telefone", Propriedade = "Telefone", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 5, Descricao = "Setor", Propriedade = "Setor", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 6, Descricao = "Cargo", Propriedade = "Cargo", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 7, Descricao = "Nível Escalation", Propriedade = "NivelEscalation", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 8, Descricao = "Clientes", Propriedade = "Clientes", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 9, Descricao = "Usuario", Propriedade = "Usuario", Tamanho = tamanho, CampoInformacao = true });

            return configuracoes;
        }
        #endregion
    }
}
