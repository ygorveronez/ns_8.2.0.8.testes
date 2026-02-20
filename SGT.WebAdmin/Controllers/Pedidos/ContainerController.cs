using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Pedidos
{
    [CustomAuthorize("Pedidos/Container", "Cargas/Carga")]
    public class ContainerController : BaseController
    {
        #region Construtores

        public ContainerController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                return ObterGridPesquisa(unitOfWork);
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

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                return ObterGridPesquisa(unitOfWork, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Embarcador.Pedido.Pedido serPedido = new Servicos.Embarcador.Pedido.Pedido(unitOfWork);

            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Pedidos.Container repContainer = new Repositorio.Embarcador.Pedidos.Container(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.Container container = new Dominio.Entidades.Embarcador.Pedidos.Container();

                Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = repConfiguracaoPedido.BuscarConfiguracaoPadrao();

                PreencherContainer(container, unitOfWork);
                repContainer.Inserir(container, Auditado);

                if ((configuracaoPedido?.ValidarCadastroContainerPelaFormulaGlobal ?? false) && container.TipoPropriedade != TipoPropriedadeContainer.Soc && !string.IsNullOrWhiteSpace(container.Numero))
                {
                    if (!serPedido.ValidarDigitoContainerNumero(container.Numero))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, Localization.Resources.Consultas.Container.NumeroDoContainerEstaInvalidoDeAcordoComSeuDigitoVerificado);
                    }
                }

                if (repContainer.ValidarDuplicidadeContainer(container.Numero, container.Codigo))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, Localization.Resources.Consultas.Container.JaExisteUmContainerCadastradoComEsteNumero);
                }

                unitOfWork.CommitChanges();

                var dynContainer = new
                {
                    container.Codigo,
                    container.Descricao,
                    container.CodigoIntegracao,
                    container.Status,
                    container.Numero,
                    PesoLiquido = container.PesoLiquido.ToString("n2"),
                    Tara = container.Tara.ToString("n2"),
                    Valor = container.Valor.ToString("n2"),
                    TipoContainer = container.ContainerTipo != null ? new { container.ContainerTipo.Codigo, container.ContainerTipo.Descricao } : null,
                    MetrosCubicos = container.MetrosCubicos.ToString("n2"),
                    container.TipoPropriedade,
                    container.TipoCarregamentoNavio
                };

                return new JsonpResult(dynContainer, true, Localization.Resources.Gerais.Geral.Sucesso);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, Localization.Resources.Consultas.Container.OcorreuUmaFalhaAoAdicionarVerifiqueSeJaNaoExisteOutroContainerComMesmaNumeracao);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Embarcador.Pedido.Pedido serPedido = new Servicos.Embarcador.Pedido.Pedido(unitOfWork);
            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Pedidos.Container repContainer = new Repositorio.Embarcador.Pedidos.Container(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.Container container = repContainer.BuscarPorCodigo(codigo, true);

                Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = repConfiguracaoPedido.BuscarConfiguracaoPadrao();


                PreencherContainer(container, unitOfWork);
                repContainer.Atualizar(container, Auditado);

                if (repContainer.ValidarDuplicidadeContainer(container.Numero, container.Codigo))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, "Já existe um container cadastrado com este número.");
                }

                if ((configuracaoPedido?.ValidarCadastroContainerPelaFormulaGlobal ?? false) && container.TipoPropriedade != TipoPropriedadeContainer.Soc && !string.IsNullOrWhiteSpace(container.Numero))
                {
                    if (!serPedido.ValidarDigitoContainerNumero(container.Numero))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, Localization.Resources.Consultas.Container.NumeroDoContainerEstaInvalidoDeAcordoComSeuDigitoVerificado);
                    }
                }
                if (container.Tara <= 0)
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, "Favor informe a tara do container.");
                }

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar, verifique se não existe outro container com a mesma numeração.");
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
                Repositorio.Embarcador.Pedidos.Container repContainer = new Repositorio.Embarcador.Pedidos.Container(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.Container container = repContainer.BuscarPorCodigo(codigo, false);

                var dynContainer = new
                {
                    container.Codigo,
                    container.Descricao,
                    container.CodigoIntegracao,
                    container.Status,
                    container.Numero,
                    PesoLiquido = container.PesoLiquido.ToString("n2"),
                    Tara = container.Tara.ToString("n2"),
                    Valor = container.Valor.ToString("n2"),
                    TipoContainer = container.ContainerTipo != null ? new { container.ContainerTipo.Codigo, container.ContainerTipo.Descricao } : null,
                    MetrosCubicos = container.MetrosCubicos.ToString("n2"),
                    container.TipoPropriedade,
                    container.TipoCarregamentoNavio
                };

                return new JsonpResult(dynContainer);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Pedidos.Container repContainer = new Repositorio.Embarcador.Pedidos.Container(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.Container container = repContainer.BuscarPorCodigo(codigo, true);

                if (container == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                repContainer.Deletar(container, Auditado);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.NaoFoiPossivelExcluirRegistro);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherContainer(Dominio.Entidades.Embarcador.Pedidos.Container container, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.ContainerTipo repTipoContainer = new Repositorio.Embarcador.Pedidos.ContainerTipo(unitOfWork);

            bool.TryParse(Request.Params("Status"), out bool status);
            int.TryParse(Request.Params("TipoContainer"), out int tipoContainer);

            decimal.TryParse(Request.Params("PesoLiquido"), out decimal pesoLiquido);
            decimal.TryParse(Request.Params("Tara"), out decimal tara);
            decimal.TryParse(Request.Params("Valor"), out decimal valor);
            decimal.TryParse(Request.Params("MetrosCubicos"), out decimal metrosCubicos);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCarregamentoNavio tipoCarregamentoNavio = Request.GetEnumParam<TipoCarregamentoNavio>("TipoCarregamentoNavio");


            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropriedadeContainer tipoPropriedade;
            Enum.TryParse(Request.Params("TipoPropriedade"), out tipoPropriedade);

            string descricao = Request.Params("Descricao");
            string codigoIntegracao = Request.Params("CodigoIntegracao");
            string numero = Request.Params("Numero");

            container.Descricao = string.IsNullOrWhiteSpace(descricao) ? numero : descricao;
            container.CodigoIntegracao = codigoIntegracao;
            container.Status = string.IsNullOrWhiteSpace(descricao) ? true : status;
            container.PesoLiquido = pesoLiquido;
            container.Tara = tara;
            container.Valor = valor;
            container.TipoPropriedade = tipoPropriedade;
            container.TipoCarregamentoNavio = tipoCarregamentoNavio;
            container.ContainerTipo = tipoContainer > 0 ? repTipoContainer.BuscarPorCodigo(tipoContainer, false) : null;
            container.MetrosCubicos = metrosCubicos;
            container.DataUltimaAtualizacao = DateTime.Now;
            container.Integrado = false;

            if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                if (numero.Contains(" "))
                {
                    throw new ControllerException("O Número do Container não pode conter Espaços!");
                }
                else if (numero.Length != 11)
                {
                    throw new ControllerException("O Número do Container deve ter exatamente 11 Dígitos!");
                }
            }
            container.Numero = Utilidades.String.SanitizeString(numero);
        }

        private IActionResult ObterGridPesquisa(Repositorio.UnitOfWork unitOfWork, bool exportacao = false)
        {
            string descricao = Request.Params("Descricao");
            string codigoIntegracao = Request.Params("CodigoIntegracao");
            string numero = Request.Params("Numero");
            int tipoContainer = Request.GetIntParam("ContainerTipo");
            double localContainer = Request.GetDoubleParam("Local");
            SituacaoAtivoPesquisa status = Request.GetEnumParam("Status", SituacaoAtivoPesquisa.Ativo);
            StatusColetaContainer? statusColetaContainer = Request.GetNullableEnumParam<StatusColetaContainer>("StatusColetaContainer");

            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Armador", false);
            grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 30, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Armador", "ClienteArmador", 40, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Numero, "Numero", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.CodigoIntegracao, "CodigoIntegracao", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Consultas.Container.TipoDeContainer, "TipoContainer", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Tara", false);
            grid.AdicionarCabecalho("Propriedade", "TipoPropriedade", 10, Models.Grid.Align.left, true);


            if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Status, "DescricaoStatus", 10, Models.Grid.Align.center, false);

            Repositorio.Embarcador.Pedidos.Container repContainer = new Repositorio.Embarcador.Pedidos.Container(unitOfWork);
            int countContainers = repContainer.ContarConsulta(descricao, codigoIntegracao, numero, tipoContainer, localContainer, status, statusColetaContainer);
            IList<Dominio.ObjetosDeValor.Embarcador.Container.RetornoConsultaContainer> containers = countContainers > 0 ? repContainer.Consultar(descricao, codigoIntegracao, numero, tipoContainer, localContainer, status, statusColetaContainer, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite) : new List<Dominio.ObjetosDeValor.Embarcador.Container.RetornoConsultaContainer>();
            grid.setarQuantidadeTotal(countContainers);

            var lista = (from p in containers
                         select new
                         {
                             p.Codigo,
                             p.Armador,
                             p.Descricao,
                             p.ClienteArmador,
                             p.Numero,
                             p.CodigoIntegracao,
                             p.DescricaoStatus,
                             TipoContainer = p.DescricaoTipoContainer ?? "",
                             Tara = Utilidades.String.OnlyNumbers(p.Tara.ToString("n0")),
                             TipoPropriedade = p.TipoPropriedade.ObterDescricao(),

                         }).ToList();

            grid.AdicionaRows(lista);

            if (exportacao)
            {
                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            else
            {
                return new JsonpResult(grid);
            }

        }

        #endregion
    }
}
