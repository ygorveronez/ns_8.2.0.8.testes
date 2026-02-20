using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Enumerador;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Escrituracao
{
    [CustomAuthorize("Escrituracao/SaldoContratoFreteCliente")]
    public class SaldoContratoFreteClienteController : BaseController
    {
        #region Construtores

        public SaldoContratoFreteClienteController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                return new JsonpResult(ObterGridPesquisa());
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

        public async Task<IActionResult> PesquisaExtrato()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                return new JsonpResult(ObterGridExtrato());
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

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Escrituracao.ContratoFreteCliente repContratoFreteCliente = new Repositorio.Embarcador.Escrituracao.ContratoFreteCliente(unitOfWork);
                Dominio.Entidades.Embarcador.Escrituracao.ContratoFreteCliente contratoFreteCliente = repContratoFreteCliente.BuscarPorCodigo(codigo, false);

                if (contratoFreteCliente == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");


                return new JsonpResult(new
                {
                    Cliente = new
                    {
                        contratoFreteCliente.Cliente.Codigo,
                        contratoFreteCliente.Cliente.Descricao,
                    },
                    ContratoFechado = contratoFreteCliente.Fechado,
                    SaldoFinalContrato = contratoFreteCliente.SaldoAtual.ToString(),
                    Status = contratoFreteCliente.Fechado ? "Fechado" : "Aberto",
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

        public async Task<IActionResult> FecharContrato()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Escrituracao.ContratoFreteCliente repContratoFreteCliente = new Repositorio.Embarcador.Escrituracao.ContratoFreteCliente(unitOfWork);
                Dominio.Entidades.Embarcador.Escrituracao.ContratoFreteCliente contratoFreteCliente = repContratoFreteCliente.BuscarPorCodigo(codigo, true);
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Escrituracao/SaldoContratoFreteCliente");

                if (contratoFreteCliente == null)
                    return new JsonpResult(false, "Registro não encontrado.");

                if (contratoFreteCliente.Fechado)
                    return new JsonpResult(true, "Este contrato já está fechado.");

                if (!Usuario.UsuarioAdministrador && !permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Alterar))
                    throw new ControllerException(Localization.Resources.Gerais.Geral.UsuarioSemPermissaoParaExecutarEssaAcao);

                contratoFreteCliente.Fechado = true;
                contratoFreteCliente.DataFim = DateTime.Now;   

                repContratoFreteCliente.Atualizar(contratoFreteCliente, Auditado);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao encerrar o contrato.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados 

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho(Localization.Resources.Escrituracao.SaldoContratoFreteCliente.NumeroContrato, "NumeroContrato", 30, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Escrituracao.SaldoContratoFreteCliente.Descricao, "Descricao", 30, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Escrituracao.SaldoContratoFreteCliente.Status, "Status", 30, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Escrituracao.SaldoContratoFreteCliente.Cliente, "Cliente", 30, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Escrituracao.SaldoContratoFreteCliente.TipoOperacao, "TipoOperacao", 30, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Escrituracao.SaldoContratoFreteCliente.DataInicioContrato, "DataInicioContrato", 30, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Escrituracao.SaldoContratoFreteCliente.DataFimContrato, "DataFinalContrato", 30, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Escrituracao.SaldoContratoFreteCliente.SaldoOriginal, "SaldoOriginal", 30, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Escrituracao.SaldoContratoFreteCliente.SaldoAtual, "SaldoAtual", 30, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Escrituracao.SaldoContratoFreteCliente.ValorDebitado, "ValorDebitado", 30, Models.Grid.Align.left, false);


            Repositorio.Embarcador.Escrituracao.ContratoFreteCliente repContratoFreteCliente = new Repositorio.Embarcador.Escrituracao.ContratoFreteCliente(unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaSaldoContratoFreteCliente filtrosPesquisa = ObterFiltrosPesquisa();

            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

            int totalRegistros = repContratoFreteCliente.ContarConsultaContratos(filtrosPesquisa);

            List<Dominio.Entidades.Embarcador.Escrituracao.ContratoFreteCliente> listaContratoFreteCliente = totalRegistros > 0 ? repContratoFreteCliente.ConsultarContratos(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Escrituracao.ContratoFreteCliente>();

            var lista = (from p in listaContratoFreteCliente
                         select new
                         {
                             p.Codigo,
                             p.NumeroContrato,
                             p.Descricao,
                             Cliente = p.Cliente.Nome,
                             TipoOperacao = p.TipoOperacao.Descricao,
                             DataInicioContrato = p.DataInicio.ToString("dd/MM/yyyy"),
                             DataFinalContrato = p.DataFim.ToString("dd/MM/yyyy"),
                             SaldoOriginal = p.ValorContrato.ToString(),
                             SaldoAtual = p.SaldoAtual.ToString(),
                             ValorDebitado = (p.ValorContrato - p.SaldoAtual).ToString(),
							 Status = p.Fechado ? "Fechado" : "Aberto"
						 }).ToList();

            grid.AdicionaRows(lista);
            grid.setarQuantidadeTotal(totalRegistros);

            return grid;
        }

        private Models.Grid.Grid ObterGridExtrato()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Escrituracao.MovimentoContratoFreteCliente repMovimentoContratoFreteCliente = new Repositorio.Embarcador.Escrituracao.MovimentoContratoFreteCliente(unitOfWork);
            Repositorio.Embarcador.Escrituracao.ContratoFreteCliente repContratoFreteCliente = new Repositorio.Embarcador.Escrituracao.ContratoFreteCliente(unitOfWork);

            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            int codigoContrato = Request.GetIntParam("Codigo");

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho(Localization.Resources.Escrituracao.SaldoContratoFreteCliente.NumeroContrato, "NumeroContrato", 30, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Escrituracao.SaldoContratoFreteCliente.Carga, "Carga", 30, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Escrituracao.SaldoContratoFreteCliente.ModeloVeicular, "ModeloVeicular", 30, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Escrituracao.SaldoContratoFreteCliente.Data, "Data", 30, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Escrituracao.SaldoContratoFreteCliente.Filial, "Filial", 30, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Escrituracao.SaldoContratoFreteCliente.Transportador, "Transportador", 30, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Escrituracao.SaldoContratoFreteCliente.ValorMovimento, "Valor", 30, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Escrituracao.SaldoContratoFreteCliente.TipoMovimento, "Tipo", 30, Models.Grid.Align.left, false);


            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
            Dominio.Entidades.Embarcador.Escrituracao.ContratoFreteCliente contratoFreteCliente = repContratoFreteCliente.BuscarPorCodigo(codigoContrato, false);

            List<Dominio.Entidades.Embarcador.Escrituracao.MovimentoContratoFreteCliente> listaMovimentos = new List<Dominio.Entidades.Embarcador.Escrituracao.MovimentoContratoFreteCliente>();

            if (codigoContrato > 0)
                listaMovimentos = repMovimentoContratoFreteCliente.BuscarPorCodigoContrato(codigoContrato, parametrosConsulta);

            var lista = (from p in listaMovimentos
                         select new
                         {
                             p.Codigo,
                             Data = p.Data.ToString() ?? string.Empty,
                             NumeroContrato = contratoFreteCliente.Descricao ?? string.Empty,
                             Carga = p.Carga.CodigoCargaEmbarcador ?? string.Empty,
                             ModeloVeicular = p.Carga.ModeloVeicularCarga.Descricao ?? string.Empty,
                             Filial = p.Carga.Filial.Descricao ?? string.Empty,
                             Transportador = p.Carga.Empresa.RazaoSocial ?? string.Empty,
                             Valor = p.Valor.ToString() ?? string.Empty,
                             Tipo = p.TipoMovimentoContrato.ObterDescricao() ?? string.Empty,
                         }).ToList();

            grid.AdicionaRows(lista);
            grid.setarQuantidadeTotal(listaMovimentos.Count);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaSaldoContratoFreteCliente ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaSaldoContratoFreteCliente()
            {
                Cliente = Request.GetDoubleParam("Cliente"),
                Transportador = Request.GetIntParam("Transportador"),
                CodigoContrato = Request.GetIntParam("NumeroContrato"),
                NumeroCarga = Request.GetIntParam("Carga"),
                DataInicialContrato = Request.GetDateTimeParam("DataInicioContrato"),
                DataFinalContrato = Request.GetDateTimeParam("DataFimContrato"),
            };
        }
        #endregion
    }
}
