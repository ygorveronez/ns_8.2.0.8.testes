using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Entidades.Embarcador.Configuracoes;
using Dominio.Entidades.Embarcador.Pedidos;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Enumerador;
using Dominio.ObjetosDeValor.Relatorios;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Repositorio;
using Servicos.Extensions;
using SGTAdmin.Controllers;
using System.Text;

namespace SGT.WebAdmin.Controllers.Pedidos
{
    [CustomAuthorize(new string[] { "ConsultarAutorizacoes", "GerarRelatorio", "GerarRelatorioPlanoViagem", "ObterConfiguracoesGeraisPedido", "ObterQuantidadeDeStages" }, "Pedidos/Pedido", "Cargas/Carga", "Pessoas/Usuario", "Pedidos/AcompanhamentoPedido", "Pedidos/PedidoAnexo", "Pedidos/PlanejamentoPedidoTMS", "TorreControle/AcompanhamentoCarga", "Logistica/MonitoramentoNovo")]
    public class PedidoController : BaseController
    {
        #region Construtores

        public PedidoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaEmpresaResponsavel()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                string descricao = Request.Params("Descricao");
                string codigoIntegracao = Request.Params("CodigoIntegracao");


                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 50, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.Pedido.CodigoIntegracao, "CodigoIntegracao", 20, Models.Grid.Align.left, true);


                Repositorio.Embarcador.Pedidos.PedidoEmpresaResponsavel repPedidoEmpresaResponsavel = new Repositorio.Embarcador.Pedidos.PedidoEmpresaResponsavel(unitOfWork);
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoEmpresaResponsavel> navios = repPedidoEmpresaResponsavel.Consultar(descricao, codigoIntegracao, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repPedidoEmpresaResponsavel.ContarConsulta(descricao, codigoIntegracao));

                var lista = (from p in navios
                             select new
                             {
                                 p.Codigo,
                                 p.Descricao,
                                 p.CodigoIntegracao
                             }).ToList();

                grid.AdicionaRows(lista);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Pedidos.Pedido.OcorreuUmaFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaCentroCusto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                string descricao = Request.Params("Descricao");
                string codigoIntegracao = Request.Params("CodigoIntegracao");


                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 50, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.Pedido.CodigoIntegracao, "CodigoIntegracao", 20, Models.Grid.Align.left, true);

                Repositorio.Embarcador.Pedidos.PedidoCentroCusto repPedidoCentroCusto = new Repositorio.Embarcador.Pedidos.PedidoCentroCusto(unitOfWork);
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoCentroCusto> navios = repPedidoCentroCusto.Consultar(descricao, codigoIntegracao, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repPedidoCentroCusto.ContarConsulta(descricao, codigoIntegracao));

                var lista = (from p in navios
                             select new
                             {
                                 p.Codigo,
                                 p.Descricao,
                                 p.CodigoIntegracao
                             }).ToList();

                grid.AdicionaRows(lista);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Pedidos.Pedido.OcorreuUmaFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaAguardandoNotasFiscais()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string codigoPedidoEmbarcador = Request.Params("CodigoPedidoEmbarcador");

                double remetente = float.Parse(Request.Params("Remetente"));
                double destinatario = float.Parse(Request.Params("Destinatario"));
                int pedido = 0; // int.Parse(Request.Params("Pedido"));

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoCargaPedido", false);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.Pedido.DescricaoPedido, "NumeroPedidoEmbarcador", 5, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Carga, "CodigoCargaEmbarcador", 5, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.Pedido.Filial, "Filial", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Remetente, "Remetente", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Origem, "Origem", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Destinatario, "Destinatario", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Destino, "Destino", 15, Models.Grid.Align.left, true);
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdenar == "NumeroPedidoEmbarcador")
                    propOrdenar = "Pedido.NumeroPedidoEmbarcador";

                if (propOrdenar == "Pedido")
                    propOrdenar = "Pedido.Pedido.Descricao";

                if (propOrdenar == "Remetente")
                    propOrdenar = "Pedido.Remetente.Nome";

                if (propOrdenar == "Destino")
                    propOrdenar = "Pedido.Destino.Descricao";

                if (propOrdenar == "Origem")
                    propOrdenar = "Pedido.Origem.Descricao";


                if (propOrdenar == "Destinatario")
                    propOrdenar = "Pedido.Cliente.Nome";

                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarCargaPedidosPorSituacaoCarga(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe, codigoPedidoEmbarcador, pedido, remetente, destinatario, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repCargaPedido.ContarCargaPedidosPorSituacaoCarga(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe, codigoPedidoEmbarcador, pedido, remetente, destinatario));

                var lista = (from p in cargaPedidos
                             select new
                             {
                                 CodigoCargaPedido = p.Codigo,
                                 p.Pedido.Codigo,
                                 CodigoCargaEmbarcador = p.Carga.CodigoCargaEmbarcador,
                                 NumeroPedidoEmbarcador = p.Pedido.NumeroPedidoEmbarcador,
                                 Filial = p.Pedido.Filial.Descricao,
                                 Remetente = p.Pedido.Remetente.Descricao,
                                 Origem = p.Origem.DescricaoCidadeEstado,
                                 Destino = p.Destino.DescricaoCidadeEstado,
                                 Destinatario = p.Pedido.Destinatario.Descricao,
                                 DT_RowColor = p.SituacaoEmissao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNF.NFEnviada ? "#dff0d8" : ""
                             }).ToList();

                grid.AdicionaRows(lista);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Pedidos.Pedido.OcorreuUmaFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConsultarAutorizacoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = GridConsultarAutorizacoes(unitOfWork);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Pedidos.Pedido.OcorreuUmaFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterDataColeta()
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                double codigoRementete = Request.GetDoubleParam("Remetente");

                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

                Repositorio.Embarcador.Logistica.JanelaColeta repJanelaColeta = new Repositorio.Embarcador.Logistica.JanelaColeta(unitOfWork);

                Dominio.Entidades.Cliente remetente = repCliente.BuscarPorCPFCNPJ(codigoRementete);


                Dominio.Entidades.Embarcador.Logistica.JanelaColeta janelaColeta;

                janelaColeta = repJanelaColeta.BuscarPorCliente(codigoRementete);

                if (janelaColeta == null)
                    janelaColeta = repJanelaColeta.BuscarPorLocalidade(remetente?.Localidade?.Codigo ?? 0);

                if (janelaColeta == null)
                    janelaColeta = repJanelaColeta.BuscarPorEstado(remetente?.Localidade?.Estado?.Sigla ?? string.Empty);

                if (janelaColeta == null)
                    return new JsonpResult(null);


                DateTime dataBase = DateTime.Now.Date;
                DateTime? dataColeta = null;

                for (var i = 1; i <= 7; i++)
                {
                    dataBase = dataBase.AddDays(1);
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana diaSemana = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana)dataBase.DayOfWeek + 1;

                    var periodoSemana = (from periodo in janelaColeta.PeriodosColeta where periodo.Dia == diaSemana select periodo).FirstOrDefault();

                    if (periodoSemana != null)
                    {
                        dataColeta = dataBase.AddSeconds(periodoSemana.HoraInicio.TotalSeconds);
                        break;
                    }
                }

                var result = new { DataColeta = dataColeta?.ToString("dd/MM/yyyy HH:mm") ?? null };

                return new JsonpResult(result);

            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Pedidos.Pedido.OcorreuUmaFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Pedidos.SituacaoComercialPedido repSituacaoComercialPedido = new Repositorio.Embarcador.Pedidos.SituacaoComercialPedido(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = repConfiguracaoGeral.BuscarConfiguracaoPadrao();
                Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedido filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descricao", false);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.RetiradaPedidoLista.Carga, "NumeroCarga", 10, Models.Grid.Align.center, false);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                    grid.AdicionarCabecalho(Localization.Resources.Pedidos.RetiradaPedidoLista.Sessao, "SessaoRoteirizador", 10, Models.Grid.Align.center, false);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    grid.AdicionarCabecalho(Localization.Resources.Pedidos.RetiradaPedidoLista.DescricaoPedido, "Numero", 8, Models.Grid.Align.center, true);
                else
                    grid.AdicionarCabecalho(Localization.Resources.Pedidos.RetiradaPedidoLista.SequencialPedido, "Numero", 8, Models.Grid.Align.center, true);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
                    grid.AdicionarCabecalho(Localization.Resources.Pedidos.RetiradaPedidoLista.DescricaoPedido, "NumeroPedidoEmbarcador", 12, Models.Grid.Align.center, false);
                else
                    grid.AdicionarCabecalho(Localization.Resources.Pedidos.RetiradaPedidoLista.Remetente, "Remetente", 18, Models.Grid.Align.left, true);

                grid.AdicionarCabecalho(Localization.Resources.Pedidos.RetiradaPedidoLista.Origem, "Origem", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.RetiradaPedidoLista.Destinatario, "Destinatario", 18, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.RetiradaPedidoLista.Destino, "Destino", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.RetiradaPedidoLista.Carregamento, "DataCarregamentoPedido", 10, Models.Grid.Align.left, true);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    grid.AdicionarCabecalho(Localization.Resources.Pedidos.RetiradaPedidoLista.Motorista, "Motorista", 10, Models.Grid.Align.left, false);
                    grid.AdicionarCabecalho(Localization.Resources.Pedidos.RetiradaPedidoLista.Veiculo, "Veiculo", 10, Models.Grid.Align.left, false);
                }

                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "SituacaoPedido", 10, Models.Grid.Align.center, true);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador && repSituacaoComercialPedido.ExisteSituacaoComercialPedido())
                    grid.AdicionarCabecalho(Localization.Resources.Consultas.SituacaoComercialPedido.SituacaoComercialDoPedido, "SituacaoComercialPedido", 10, Models.Grid.Align.center, true);

                if (configuracaoGeral.HabilitarFuncionalidadesProjetoGollum)
                {
                    grid.AdicionarCabecalho(Localization.Resources.Pedidos.RetiradaPedidoLista.CategoriaOS, "CategoriaOS", 10, Models.Grid.Align.left, false);
                    grid.AdicionarCabecalho(Localization.Resources.Pedidos.RetiradaPedidoLista.TipoOSConvertido, "TipoOSConvertido", 10, Models.Grid.Align.left, false);
                }

                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido repositorioSessaoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork, cancellationToken);

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                int totalRegistros = await repositorioPedido.ContarConsultaAsync(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> listaPedido = totalRegistros > 0 ? await repositorioPedido.ConsultarAsync(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();

                IList<Dominio.ObjetosDeValor.Embarcador.Pedido.TipoCarga> cargasPedidos = new List<Dominio.ObjetosDeValor.Embarcador.Pedido.TipoCarga>();
                IList<Dominio.ObjetosDeValor.Embarcador.Carga.TotalizadorModeloDocumento> sessoesPedidos = new List<Dominio.ObjetosDeValor.Embarcador.Carga.TotalizadorModeloDocumento>();

                List<Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoMotorista> motoristas = new List<Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoMotorista>();
                List<Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoVeiculo> veiculos = new List<Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoVeiculo>();

                if (listaPedido?.Count > 0)
                {
                    List<int> codigosPedidos = (from item in listaPedido select item.Codigo).ToList();
                    cargasPedidos = repositorioCargaPedido.BuscarNumeroCargasPorPedidos(codigosPedidos);
                    sessoesPedidos = repositorioSessaoPedido.BuscarSessoesPorPedidos(codigosPedidos);
                    motoristas = repositorioPedido.BuscarMotoristasPorPedidos(codigosPedidos);
                    veiculos = repositorioPedido.BuscarPlacasPorPedidos(codigosPedidos);
                }

                var listaPedidoRetornar = (
                    from pedido in listaPedido
                    select new
                    {
                        pedido.Codigo,
                        Descricao = pedido.NumeroPedidoEmbarcador,
                        NumeroCarga = string.Join(", ", (from cp in cargasPedidos where cp.Codigo == pedido.Codigo select cp.CodigoEmbarcador).ToList()), //repositorioCargaPedido.BuscarNumeroCargasPorPedido(pedido.Codigo)),
                        SessaoRoteirizador = string.Join(", ", (from cp in sessoesPedidos where cp.Codigo == pedido.Codigo select cp.Total).ToList()), //repositorioSessaoPedido.BuscarSessoesPorPedido(pedido.Codigo)),
                        pedido.Numero,
                        pedido.NumeroPedidoEmbarcador,
                        Remetente = pedido.Remetente?.Descricao ?? pedido.GrupoPessoas?.Descricao ?? string.Empty,
                        Destinatario = pedido.Destinatario?.Descricao ?? string.Empty,
                        Destino = pedido.Destino?.DescricaoCidadeEstado ?? string.Empty,
                        Origem = pedido.Origem?.DescricaoCidadeEstado ?? string.Empty,
                        DataCarregamentoPedido = pedido.DataCarregamentoPedido?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                        Motorista = string.Join(", ", motoristas.Where(p => p.CodigoPedido == pedido.Codigo).Select(o => o.Motorista).ToList()),
                        Veiculo = ObterPlacasPedido(pedido, veiculos),
                        SituacaoPedido = pedido.DescricaoSituacaoPedido,
                        SituacaoComercialPedido = pedido.SituacaoComercialPedido?.Descricao ?? string.Empty,
                        CategoriaOS = pedido.CategoriaOS?.ObterDescricao() ?? string.Empty,
                        TipoOSConvertido = pedido.TipoOSConvertido?.ObterDescricao() ?? string.Empty
                    }
                ).ToList();

                grid.AdicionaRows(listaPedidoRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Pedidos.Pedido.OcorreuUmaFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> SalvarStatusCanceladoAposVinculoCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");

                if (codigo == 0)
                    throw new ControllerException(Localization.Resources.Pedidos.Pedido.CodigoDoPedidoEInvalido);

                unitOfWork.Start();

                Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repositorioConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = repositorioConfiguracaoPedido.BuscarConfiguracaoPadrao();

                if (!configuracaoPedido.PermitirMudarStatusPedidoParaCanceladoAposVinculoCarga)
                    throw new ControllerException(Localization.Resources.Pedidos.Pedido.AcaoNaoEPermitida);

                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

                bool existeNota = repositorioPedidoXMLNotaFiscal.VerificarSeExisteNotaPorPedido(codigo);

                if (!existeNota)
                    throw new ControllerException(Localization.Resources.Pedidos.Pedido.NaoExisteNotaParaOPedido);

                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repositorioPedido.BuscarPorCodigo(codigo);

                if (pedido == null)
                    throw new ControllerException(Localization.Resources.Pedidos.Pedido.OPedidoNaoFoiEncontrado);

                pedido.CanceladoAposVinculoCarga = true;
                repositorioPedido.Atualizar(pedido);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, pedido, Localization.Resources.Pedidos.Pedido.StatusSalvoComoCanceladoAposVinculoComACarga, unitOfWork);

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
                Servicos.Log.TratarErro(excecao);
                unitOfWork.Rollback();
                return new JsonpResult(false, Localization.Resources.Pedidos.Pedido.OcorreuUmProblemaAoMudarStatusDoPedidoParaCancelado);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaDisponiveis(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedido filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);

                bool retornarSaldoPesoProdutos = Request.GetBoolParam("RetornarSaldoPesoProdutosPedido");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("PesoTotal", false);
                grid.AdicionarCabecalho("PesoSaldoRestante", false);
                grid.AdicionarCabecalho("PesoSaldoProdutos", false);
                grid.AdicionarCabecalho("Descricao", false);
                grid.AdicionarCabecalho("PedidoDestinadoAFilial", false);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    grid.AdicionarCabecalho(Localization.Resources.Consultas.Pedido.DescricaoPedido, "Numero", 12, Models.Grid.Align.center, false);
                    grid.AdicionarCabecalho(Localization.Resources.Consultas.Pedido.DescricaoPedido + " Embarcador", "NumeroPedidoEmbarcador", 12, Models.Grid.Align.center, false);
                }
                else
                {
                    grid.AdicionarCabecalho(Localization.Resources.Consultas.Pedido.DescricaoPedido, "NumeroPedidoEmbarcador", 12, Models.Grid.Align.center, false);
                }

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                {
                    grid.AdicionarCabecalho(Localization.Resources.Consultas.Pedido.Remetente, "Remetente", 18, Models.Grid.Align.left, true);
                    grid.AdicionarCabecalho(Localization.Resources.Consultas.Pedido.Origem, "Origem", 15, Models.Grid.Align.left, true);
                }

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                {
                    grid.AdicionarCabecalho("IncoTerm", "TipoPagamento", 18, Models.Grid.Align.left, false);
                    grid.AdicionarCabecalho("Tipo de Operação", "TipoOperacao", 15, Models.Grid.Align.left, false);
                }

                grid.AdicionarCabecalho(Localization.Resources.Consultas.Pedido.Destinatario, "Destinatario", 18, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Pedido.Destino, "Destino", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Pedido.TipoDeCarga, "TipoDeCarga", 10, Models.Grid.Align.left, true);

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    grid.AdicionarCabecalho(Localization.Resources.Consultas.Pedido.Carregamento, "DataCarregamentoPedido", 10, Models.Grid.Align.left, true);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                {
                    grid.AdicionarCabecalho("Data de Criação de Remessa ", "DataCriacaoPedido", 15, Models.Grid.Align.left, false);
                    grid.AdicionarCabecalho("M²", "MetroCubagem", 5, Models.Grid.Align.left, false);

                    if (retornarSaldoPesoProdutos)
                        grid.AdicionarCabecalho("Peso Saldo dos Produtos", "PesoSaldoProdutos", 10, Models.Grid.Align.left, false);
                    else
                        grid.AdicionarCabecalho("Peso", "PesoTotal", 15, Models.Grid.Align.left, false);

                    grid.AdicionarCabecalho("Valor NF", "ValorNF", 10, Models.Grid.Align.left, false);
                }


                if (filtrosPesquisa.PedidoParaReentrega)
                    filtrosPesquisa.Situacao = null;
                else
                {
                    filtrosPesquisa.Situacao = SituacaoPedido.Aberto;
                    if (!filtrosPesquisa.ProgramaComSessaoRoteirizador)
                        filtrosPesquisa.PedidoSemCargaPedido = true;
                }

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
                    filtrosPesquisa.OcultarPedidosRetiradaProdutos = true;

                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork, cancellationToken);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                filtrosPesquisa.CodigoPedidoMinimo = 0;

                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> listaPedido = new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
                List<int> codigosDosPedidosDestinadosAFiliais = new List<int>();
                List<int> codigosPedidos = new List<int>();
                int totalRegistros = 0;

                List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SaldoPedidoProduto> produtos = new List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SaldoPedidoProduto>();

                if (Request.GetBoolParam("AdicionarMultiplosPedidos"))
                {
                    filtrosPesquisa.NumeroCarregamentoPedido = Request.GetStringParam("NumeroCarregamentoPedido");

                    totalRegistros = !string.IsNullOrEmpty(filtrosPesquisa.NumeroCarregamentoPedido) ? await repositorioPedido.ContarConsultaAsync(filtrosPesquisa) : 0;

                    listaPedido = totalRegistros > 0 ? await repositorioPedido.ConsultarAsync(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
                    codigosPedidos = totalRegistros > 0 ? (from o in listaPedido select o.Codigo).ToList() : new List<int>();
                    codigosDosPedidosDestinadosAFiliais = totalRegistros > 0 ? repositorioPedido.BuscarCodigosDosPedidosDestinadosAFiliais(codigosPedidos) : new List<int>();
                }
                else
                {
                    totalRegistros = await repositorioPedido.ContarConsultaAsync(filtrosPesquisa);

                    listaPedido = totalRegistros > 0 ? await repositorioPedido.ConsultarAsync(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
                    codigosPedidos = totalRegistros > 0 ? (from o in listaPedido select o.Codigo).ToList() : new List<int>();
                    codigosDosPedidosDestinadosAFiliais = totalRegistros > 0 ? repositorioPedido.BuscarCodigosDosPedidosDestinadosAFiliais(codigosPedidos) : new List<int>();
                }

                if (retornarSaldoPesoProdutos)
                    produtos = Servicos.Embarcador.Carga.MontagemCarga.SaldoPedidoProduto.ObterSaldoPedidoProdutos(codigosPedidos, unitOfWork);

                var listaPedidoRetornar = (
                    from pedido in listaPedido
                    select new
                    {
                        pedido.Codigo,
                        pedido.PesoTotal,
                        pedido.PesoSaldoRestante,
                        Descricao = (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? (pedido.NumeroPedidoEmbarcador == string.Empty ? pedido.Numero.ToString() : pedido.NumeroPedidoEmbarcador) : pedido.NumeroPedidoEmbarcador),
                        //NumeroCarga = pedido.CodigoCargaEmbarcador, //string.Join(", ", repositorioCargaPedido.BuscarNumeroCargasPorPedido(pedido.Codigo)),
                        pedido.Numero,
                        pedido.NumeroPedidoEmbarcador,
                        Remetente = pedido.Remetente?.Descricao ?? pedido.GrupoPessoas?.Descricao ?? string.Empty,
                        Destinatario = pedido.Destinatario?.Descricao ?? string.Empty,
                        Destino = pedido.Destino?.DescricaoCidadeEstado ?? string.Empty,
                        Origem = pedido.Origem?.DescricaoCidadeEstado ?? string.Empty,
                        DataCarregamentoPedido = pedido.DataCarregamentoPedido?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                        PedidoDestinadoAFilial = codigosDosPedidosDestinadosAFiliais.Contains(pedido.Codigo),
                        TipoPagamento = pedido.TipoPagamento != Dominio.Enumeradores.TipoPagamento.Outros ? pedido.TipoPagamento == Dominio.Enumeradores.TipoPagamento.A_Pagar ? Dominio.Enumeradores.TipoCondicaoPagamentoHelper.ObterDescricao(Dominio.Enumeradores.TipoCondicaoPagamento.FOB) : Dominio.Enumeradores.TipoCondicaoPagamentoHelper.ObterDescricao(Dominio.Enumeradores.TipoCondicaoPagamento.CIF) : string.Empty,
                        MetroCubagem = pedido.CubagemTotal,
                        TipoOperacao = pedido.TipoOperacao?.Descricao ?? "",
                        DataCriacaoPedido = pedido.DataCriacao,
                        ValorNF = pedido.ValorTotalNotasFiscais,
                        //Motorista = string.Join(", ", repositorioPedido.BuscarMotoristas(pedido.Codigo)),
                        //Veiculo = string.Join(", ", repositorioPedido.BuscarPlacas(pedido.Codigo)),
                        //SituacaoPedido = pedido.DescricaoSituacaoPedido
                        PesoSaldoProdutos = !retornarSaldoPesoProdutos || produtos.Count <= 0 ? 0m : produtos.Where(x => x.CodigoPedido == pedido.Codigo && x.Qtde > 0).Sum(x => x.SaldoQtde * (x.Peso / x.Qtde)),
                        TipoCarga = pedido.TipoCarga?.Descricao ?? string.Empty,
                        TipoDeCarga = pedido.TipoDeCarga?.Descricao ?? string.Empty,
                    }
                ).ToList();

                grid.AdicionaRows(listaPedidoRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Pedidos.Pedido.OcorreuUmaFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaGridPendentes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = ObterGridPedidosPendentes();

                int totalRegistros = 0;

                var listaPedidoRetornar = ExecutarPesquisaPendentes(ref totalRegistros, grid.ObterParametrosConsulta(), unitOfWork);

                grid.AdicionaRows(listaPedidoRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Pedidos.Pedido.OcorreuUmaFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisaPendentes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

                Models.Grid.Grid grid = ObterGridPedidosPendentes();

                string numeroPedidos = Request.GetStringParam("NumeroPedidoFiltro").Replace(" ", "");
                double cnpjDestinatario = Request.GetDoubleParam("DestinatarioFiltro");

                grid.AdicionaRows(repositorioPedido.GerarRelatorioPedidosPendentes(numeroPedidos, cnpjDestinatario));

                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, Localization.Resources.Pedidos.Pedido.OcorreuUmaFalhaAoGerarOArquivo);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Pedidos.Pedido.OcorreuUmaFalhaAoExportar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaDisponiveisParaTroca(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();

                Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedido filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedido()
                {
                    CodigosFilial = ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork),
                    CodigosTipoCarga = ObterListaCodigoTipoCargaPermitidosOperadorLogistica(unitOfWork),
                    CodigosTipoOperacao = ObterListaCodigoTipoOperacaoPermitidosOperadorLogistica(unitOfWork),
                    FiltrarPorParteDoNumero = configuracaoEmbarcador?.FiltrarCargasPorParteDoNumero ?? false,
                    CodigoCarga = Request.GetIntParam("Carga"),
                    OcultarPedidosRetiradaProdutos = true,
                    ExibirPedidosExpedidor = configuracaoEmbarcador.NaoGerarCarregamentoRedespacho,
                    NumeroPedidoEmbarcador = Request.GetStringParam("NumeroPedidoEmbarcador"),
                    OrdernarPorPrioridade = (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                };

                if (filtrosPesquisa.CodigoCarga == 0)
                {
                    filtrosPesquisa.PedidoSemCarregamento = ConfiguracaoEmbarcador.FiltrarPorPedidoSemCarregamentoNaMontagemCarga;
                    filtrosPesquisa.PedidoSemCarga = true;
                    filtrosPesquisa.OcultarPedidosProvisorios = true;
                    filtrosPesquisa.Situacao = SituacaoPedido.Aberto;

                    int codigoFilial = Request.GetIntParam("Filial");

                    if (codigoFilial > 0)
                        filtrosPesquisa.CodigosFilial = new List<int>() { codigoFilial };
                }

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoFilial", false);
                grid.AdicionarCabecalho("CodigoTipoCarga", false);
                grid.AdicionarCabecalho("Descricao", false);
                grid.AdicionarCabecalho("DescricaoFilial", false);
                grid.AdicionarCabecalho("DescricaoTipoCarga", false);
                grid.AdicionarCabecalho("PedidoDestinadoAFilial", false);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.Pedido.DescricaoPedido, "NumeroPedidoEmbarcador", 12, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Remetente, "Remetente", 18, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Origem, "Origem", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Destinatario, "Destinatario", 18, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Destino, "Destino", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.Pedido.Carregamento, "DataCarregamentoPedido", 10, Models.Grid.Align.left, true);

                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork, cancellationToken);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                int totalRegistros = await repositorioPedido.ContarConsultaAsync(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> listaPedido = totalRegistros > 0 ? await repositorioPedido.ConsultarAsync(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
                List<int> codigosPedidos = totalRegistros > 0 ? (from o in listaPedido select o.Codigo).ToList() : new List<int>();
                List<int> codigosDosPedidosDestinadosAFiliais = totalRegistros > 0 ? repositorioPedido.BuscarCodigosDosPedidosDestinadosAFiliais(codigosPedidos) : new List<int>();

                var listaPedidoRetornar = (
                    from pedido in listaPedido
                    select new
                    {
                        pedido.Codigo,
                        CodigoFilial = pedido.Filial?.Codigo ?? 0,
                        CodigoTipoCarga = pedido.TipoCarga?.Codigo ?? 0,
                        Descricao = pedido.NumeroPedidoEmbarcador,
                        DescricaoFilial = pedido.Filial?.Descricao ?? "",
                        DescricaoTipoCarga = pedido.TipoCarga?.Descricao ?? "",
                        //NumeroCarga = string.Join(", ", repositorioCargaPedido.BuscarNumeroCargasPorPedido(pedido.Codigo)),
                        pedido.Numero,
                        pedido.NumeroPedidoEmbarcador,
                        Remetente = pedido.Remetente?.Descricao ?? pedido.GrupoPessoas?.Descricao ?? string.Empty,
                        Destinatario = pedido.Destinatario?.Descricao ?? string.Empty,
                        Destino = pedido.Destino?.DescricaoCidadeEstado ?? string.Empty,
                        Origem = pedido.Origem?.DescricaoCidadeEstado ?? string.Empty,
                        DataCarregamentoPedido = pedido.DataCarregamentoPedido?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                        PedidoDestinadoAFilial = codigosDosPedidosDestinadosAFiliais.Contains(pedido.Codigo),
                        //Motorista = string.Join(", ", repositorioPedido.BuscarMotoristas(pedido.Codigo)),
                        //Veiculo = string.Join(", ", repositorioPedido.BuscarPlacas(pedido.Codigo)),
                        //SituacaoPedido = pedido.DescricaoSituacaoPedido
                    }
                ).ToList();

                grid.AdicionaRows(listaPedidoRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Pedidos.Pedido.OcorreuUmaFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterHistoricoPedido()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoPedido = Request.GetIntParam("pedido");

                Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega repPedidoOcorrenciaColetaEntrega = new Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega(unitOfWork);
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega> pedidoOcorrenciaColetaEntrega = repPedidoOcorrenciaColetaEntrega.BuscarPorPedido(codigoPedido);


                return new JsonpResult(ObterGridHistoricoPedido(pedidoOcorrenciaColetaEntrega));
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Pedidos.Pedido.OcorreuUmaFalhaAoConsultar);
            }

            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterHistoricoAduanaPedido()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoPedido = Request.GetIntParam("pedido");

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorPedido(codigoPedido);

                if (carga != null)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregaFronteiras = repCargaEntrega.BuscarFronteirasPorCarga(carga.Codigo);

                    return new JsonpResult(ObterGridHistoricoFronteirasPedido(cargaEntregaFronteiras));
                }
                else
                    return new JsonpResult(false, "Não foi localizado carga para o pedido");

            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Pedidos.Pedido.OcorreuUmaFalhaAoConsultar);
            }

            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ValidarCotacaoTabelaFrete()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<double> listaCpfCnpjDestinatario = ObterListaCpfCnpjDestinatario();

                foreach (double cpfCnpjDestinatario in listaCpfCnpjDestinatario)
                {
                    if (!ValidarCotacaoTabelaFrete(unitOfWork, cpfCnpjDestinatario))
                    {
                        return new JsonpResult(new
                        {
                            TabelaFreteCompativel = false,
                        });
                    }
                }

                return new JsonpResult(new
                {
                    TabelaFreteCompativel = true,
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Pedidos.Pedido.OcorreuUmaFalhaAoValidarCotacoes);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ImportarDocas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                string NumeroDoca = Request.GetStringParam("NumeroDoca");
                Repositorio.Embarcador.Integracao.IntegracaoDocas repIntegracaoDocas = new Repositorio.Embarcador.Integracao.IntegracaoDocas(unitOfWork);
                List<Dominio.Entidades.Embarcador.Integracao.IntegracaoDocas> docas = repIntegracaoDocas.ConsultarTodos();
                (new Servicos.Embarcador.Integracao.VTEX.IntegracaoVtex(unitOfWork)).IntegracaoDeDocasVtex(true, docas);
                int _TempoDeDoca = repIntegracaoDocas.BuscarPorCodigoIntegracao(NumeroDoca)?.TempoMedioCarregamento ?? 0;
                return new JsonpResult(new
                {
                    CodigoPedido = Request.GetStringParam("NumeroPedidoEmbarcador"),
                    NumeroPedidoEmbarcador = Request.GetStringParam("NumeroPedidoEmbarcador"),
                    TempoDeDoca = _TempoDeDoca
                });
            }
            catch (BaseException excecao)
            {
                //unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                //unitOfWork.Rollback();
                return new JsonpResult(false, Localization.Resources.Pedidos.Pedido.OcorreuUmaFalhaAoAdicionar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Adicionar(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Pedidos/Pedido");
                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosAdicionados = new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Financeiro.Titulo repositorioTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Financeiro.DocumentoFaturamento repositorioDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Configuracoes.Integracao repositorioIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Pedidos.PedidoCarregamentoSituacao repositorioPedidoCarregamentoSituacao = new Repositorio.Embarcador.Pedidos.PedidoCarregamentoSituacao(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repositorioConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Pedidos.TipoOperacaoIntegracao repositorioTipoOperacaoIntegracao = new Repositorio.Embarcador.Pedidos.TipoOperacaoIntegracao(unitOfWork);

                Servicos.Embarcador.CIOT.Pagbem servicoPagbem = new Servicos.Embarcador.CIOT.Pagbem();
                Servicos.Embarcador.Frota.ValePedagio servicoValePedagio = new Servicos.Embarcador.Frota.ValePedagio(unitOfWork);
                Servicos.Embarcador.Pedido.OcorrenciaPedido servOcorrenciaPedido = new Servicos.Embarcador.Pedido.OcorrenciaPedido(unitOfWork);
                Servicos.Embarcador.PreCarga.PreCarga servicoPreCarga = new Servicos.Embarcador.PreCarga.PreCarga(unitOfWork);
                Servicos.Embarcador.Frota.OrdemServicoVeiculoManutencao ordemServicoVeiculoManutencao = new Servicos.Embarcador.Frota.OrdemServicoVeiculoManutencao(unitOfWork);
                Servicos.Embarcador.Carga.HistoricoVinculo servicoHistorico = new Servicos.Embarcador.Carga.HistoricoVinculo(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = await repositorioConfiguracaoPedido.BuscarConfiguracaoPadraoAsync();
                Dominio.Entidades.Embarcador.Configuracoes.Integracao integracao = repositorioIntegracao.Buscar();
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoBase = ObterPedidoBase(unitOfWork);
                PreencherDadosGeraisCargaPedido(pedidoBase, unitOfWork);
                string numeroPedidoEmbarcador = Request.GetStringParam("NumeroPedidoEmbarcador");
                int mercadoLivreRota = 0;
                int.TryParse(Request.Params("MercadoLivreRota"), out mercadoLivreRota);
                pedidoBase.Rota = mercadoLivreRota;
                pedidoBase.Facility = Request.Params("MercadoLivreFacility");

                List<int> listaModelosVeiculares = ObterListaModelosVeiculares();
                List<double> listaCpfCnpjDestinatario = ObterListaCpfCnpjDestinatario();
                List<double> listaCpfCnpjRemetente = ObterListaCpfCnpjRemetente();
                string descricaoTipoMultiplasEntidades = ObterDescricaoTipoMultiplasEntidades(listaCpfCnpjDestinatario, listaCpfCnpjRemetente);

                if ((listaCpfCnpjDestinatario.Count == 1) && (listaCpfCnpjRemetente.Count == 1))
                {
                    pedidoBase.NumeroPedidoEmbarcador = numeroPedidoEmbarcador;

                    if (!string.IsNullOrWhiteSpace(pedidoBase.NumeroPedidoEmbarcador) && (pedidoBase.Filial != null))
                    {
                        if (repositorioPedido.BuscarPorNumeroEmbarcador(pedidoBase.NumeroPedidoEmbarcador, pedidoBase.Filial.Codigo, false) != null)
                            throw new ControllerException(Localization.Resources.Pedidos.Pedido.JaExisteUmpedidoCadastradoParaEsseCodigoDeEmbarcador);
                    }
                }
                else if (!string.IsNullOrWhiteSpace(numeroPedidoEmbarcador))
                    throw new ControllerException(string.Format(Localization.Resources.Pedidos.Pedido.CodigoEmbarcadorNaoDeveSerInformadoParaPedidoComMultiplos, descricaoTipoMultiplasEntidades));

                ordemServicoVeiculoManutencao.VeiculoIndisponivelParaTransporte(pedidoBase);

                if (ConfiguracaoEmbarcador.BloquearDatasRetroativasPedido)
                {
                    if (pedidoBase.DataCarregamentoPedido.HasValue && pedidoBase.DataCarregamentoPedido.Value > DateTime.MinValue && pedidoBase.DataCarregamentoPedido.Value.Date < DateTime.Now.Date)
                        throw new ControllerException(Localization.Resources.Pedidos.Pedido.DataDaColetaNaoPodeSerMenorQueADataAtual);

                    if (pedidoBase.DataInicialColeta.HasValue && pedidoBase.DataInicialColeta.Value > DateTime.MinValue && pedidoBase.DataInicialColeta.Value.Date < DateTime.Now.Date)
                        throw new ControllerException(Localization.Resources.Pedidos.Pedido.DataDaColetaNaoPodeSerMenorQueADataAtual);

                    if (pedidoBase.DataPrevisaoSaida.HasValue && pedidoBase.DataPrevisaoSaida.Value > DateTime.MinValue && pedidoBase.DataPrevisaoSaida.Value.Date < DateTime.Now.Date)
                        throw new ControllerException(Localization.Resources.Pedidos.Pedido.DataPrevisaoSaidaNaoPodeSerMenorQueDataAtual);

                    if (pedidoBase.PrevisaoEntrega.HasValue && pedidoBase.PrevisaoEntrega.Value > DateTime.MinValue && pedidoBase.PrevisaoEntrega.Value.Date < DateTime.Now.Date)
                        throw new ControllerException(Localization.Resources.Pedidos.Pedido.DataPrevisaoEntregaNaoPodeSerMenorQueDataAtual);

                    if (pedidoBase.DataInicialViagemExecutada.HasValue && pedidoBase.DataInicialViagemExecutada.Value > DateTime.MinValue && pedidoBase.DataInicialViagemExecutada.Value.Date < DateTime.Now.Date)
                        throw new ControllerException(Localization.Resources.Pedidos.Pedido.DataInicialViajemNaoPodeSerMenorQueDataAtual);

                    if (pedidoBase.DataFinalViagemExecutada.HasValue && pedidoBase.DataFinalViagemExecutada.Value > DateTime.MinValue && pedidoBase.DataFinalViagemExecutada.Value.Date < DateTime.Now.Date)
                        throw new ControllerException(Localization.Resources.Pedidos.Pedido.DataFinalViajemNaoPodeSerMenorQueDataAtual);

                    if (pedidoBase.DataInicialViagemFaturada.HasValue && pedidoBase.DataInicialViagemFaturada.Value > DateTime.MinValue && pedidoBase.DataInicialViagemFaturada.Value.Date < DateTime.Now.Date)
                        throw new ControllerException(Localization.Resources.Pedidos.Pedido.DataInicialViajemFaturadaNaoPodeSerMenorQueDataAtual);

                    if (pedidoBase.DataFinalViagemFaturada.HasValue && pedidoBase.DataFinalViagemFaturada.Value > DateTime.MinValue && pedidoBase.DataFinalViagemFaturada.Value.Date < DateTime.Now.Date)
                        throw new ControllerException(Localization.Resources.Pedidos.Pedido.DataFinalViajemFaturadaNaoPodeSerMenorQueDataAtual);

                    if (pedidoBase.DataTerminoCarregamento.HasValue && pedidoBase.DataTerminoCarregamento.Value.Date < DateTime.Now.Date)
                        throw new ControllerException(Localization.Resources.Pedidos.Pedido.DataFinalViajemFaturadaNaoPodeSerMenorQueDataAtual);
                }

                if (pedidoBase.Empresa != null && pedidoBase.Empresa.Status != "A")
                    throw new ControllerException(Localization.Resources.Pedidos.Pedido.NaoEPossivelCriarPedidoComEmpresaInativa);

                if (pedidoBase.DataAgendamento.HasValue && pedidoBase.DataAgendamento.Value.Date < DateTime.Now.Date)
                    throw new ControllerException(Localization.Resources.Pedidos.Pedido.DataCarregamentoNaoPodeSerMenorQueDataAtual);

                if (!this.ValidarTabelaFrete(unitOfWork, pedidoBase))
                    throw new ControllerException(Localization.Resources.Pedidos.Pedido.PedidoNaoPossuiTabelaFreteConfigurada);

                Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoEmbarcador = ObterProdutoEmbarcador(unitOfWork);

                if (produtoEmbarcador != null)
                    pedidoBase.ProdutoPredominante = produtoEmbarcador.Descricao;

                await unitOfWork.StartAsync();

                if (TipoServicoMultisoftware == TipoServicoMultisoftware.MultiEmbarcador)
                    PreencherCodigoCargaEmbarcador(unitOfWork, pedidoBase);

                bool possuiPermissao = this.Usuario.UsuarioAdministrador || permissoesPersonalizadas == null || permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Pedido_PermiteCriarPedidoTomadorSemCredito);
                StringBuilder mensagemBloqueioFinanceiro = new StringBuilder();
                foreach (double cpfCnpjDestinatario in listaCpfCnpjDestinatario)
                {
                    foreach (double cpfCnpjRemetente in listaCpfCnpjRemetente)
                    {
                        Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = pedidoBase.Clonar();

                        pedido.Numero = repositorioPedido.BuscarProximoNumero();

                        if (string.IsNullOrEmpty(pedido.NumeroPedidoEmbarcador) && ConfiguracaoEmbarcador.GerarAutomaticamenteNumeroPedidoEmbarcardorNaoInformado)
                        {
                            if (ConfiguracaoEmbarcador.UtilizarNumeroPreCargaPorFilial)
                                pedido.NumeroSequenciaPedido = repositorioPedido.ObterProximoCodigo(pedido.Filial);
                            else
                                pedido.NumeroSequenciaPedido = repositorioPedido.ObterProximoCodigo();

                            pedido.NumeroPedidoEmbarcador = pedido.NumeroSequenciaPedido.ToString();
                        }

                        PreencherDadosDestinatario(unitOfWork, pedido, cpfCnpjDestinatario, configuracaoPedido);
                        PreencherDadosRemetente(unitOfWork, pedido, cpfCnpjRemetente, configuracaoPedido);

                        if ((listaCpfCnpjDestinatario.Count == 1) && (listaCpfCnpjRemetente.Count == 1))
                        {
                            if (!Servicos.Embarcador.Carga.CargaPedido.ValidarNumeroPedidoEmbarcador(out string mensagemErro, pedido.NumeroPedidoEmbarcador, pedido.ObterTomador(), pedido.TipoOperacao, ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal))
                                throw new ControllerException(mensagemErro);
                        }
                        else
                        {
                            if (ConfiguracaoEmbarcador.NumeroCargaSequencialUnico)
                                pedido.NumeroSequenciaPedido = repositorioPedido.ObterProximoCodigo();
                            else
                                pedido.NumeroSequenciaPedido = repositorioPedido.ObterProximoCodigo(pedido.Filial);

                            pedido.NumeroPedidoEmbarcador = pedido.NumeroSequenciaPedido.ToString();
                        }

                        PreencherDadosRotaFrete(unitOfWork, pedido);
                        EnviarEmailRestricoesDescarga(unitOfWork, pedido);
                        pedido.SituacaoAcompanhamentoPedido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcompanhamentoPedido.AgColeta;

                        if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && repositorioPedido.ContemPedidoMesmoNumero(pedido.Numero))
                            pedido.Numero = repositorioPedido.BuscarProximoNumero();

                        SalvarListaMotorista(ref pedido, unitOfWork, permissoesPersonalizadas);

                        repositorioPedido.Inserir(pedido, Auditado);

                        pedido.Protocolo = pedido.Codigo;

                        if (VerificarRegrasPedido(pedido, TipoServicoMultisoftware, unitOfWork))
                        {
                            pedido.SituacaoPedido = SituacaoPedido.AutorizacaoPendente;
                            pedido.EtapaPedido = EtapaPedido.AgAutorizacao;
                        }
                        else
                        {
                            pedido.SituacaoPedido = SituacaoPedido.Aberto;
                            pedido.EtapaPedido = EtapaPedido.Finalizada;
                        }

                        Dominio.Entidades.Embarcador.Pedidos.PedidoCarregamentoSituacao pedidoCarregamentoSituacao = new Dominio.Entidades.Embarcador.Pedidos.PedidoCarregamentoSituacao();

                        pedidoCarregamentoSituacao.SituacaoAtualPedidoRetirada = pedido.SituacaoAtualPedidoRetirada;
                        pedidoCarregamentoSituacao.DataCriacaoPedido = DateTime.Now;
                        pedidoCarregamentoSituacao.DataLiberacaoComercial = DateTime.Now;
                        pedidoCarregamentoSituacao.DataLiberacaoFinanceira = DateTime.Now;
                        await repositorioPedidoCarregamentoSituacao.InserirAsync(pedidoCarregamentoSituacao);

                        pedido.PedidoCarregamentoSituacao = pedidoCarregamentoSituacao;

                        repositorioPedido.Atualizar(pedido);

                        string retornoParcial = SalvarNotasParciaisPedido(pedido, unitOfWork);
                        if (!string.IsNullOrWhiteSpace(retornoParcial))
                            throw new ControllerException(retornoParcial);

                        SalvarCtesParciaisPedido(pedido, unitOfWork);
                        SalvarDIPedido(pedido, unitOfWork);
                        if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                        {
                            Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco pedidoEnderecoDestino = new Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco();
                            string msgRetorno = SalvarDadosMultimodal(ref pedido, ref pedidoEnderecoDestino, unitOfWork);
                            if (!string.IsNullOrWhiteSpace(msgRetorno))
                                throw new ControllerException(msgRetorno);

                            if (pedido.PedidoViagemNavio != null && pedido.Container != null)
                            {
                                if (pedido.TipoOperacao == null || pedido.TipoOperacao.TipoPropostaMultimodal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.VAS)
                                {
                                    Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoExistente = repositorioPedido.ValidarViagemContainer(pedido.PedidoViagemNavio.Codigo, pedido.Container.Codigo, pedido.Codigo);
                                    if (pedidoExistente != null && !configuracaoPedido.NaoValidarMesmaViagemEMesmoContainer)
                                        throw new ControllerException(string.Format(Localization.Resources.Pedidos.Pedido.JaExisteOutroPedidoContendoMesmoContainerParaMesmoNavio, pedidoExistente.Numero.ToString("D"), pedidoExistente.NumeroBooking));
                                }
                            }
                        }

                        if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                        {
                            if (pedido.Produtos == null)
                                pedido.Produtos = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>();

                            if (produtoEmbarcador != null)
                            {
                                Repositorio.Embarcador.Pedidos.PedidoProduto repositorioPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(unitOfWork);
                                Dominio.Entidades.Embarcador.Pedidos.PedidoProduto pedidoProduto = new Dominio.Entidades.Embarcador.Pedidos.PedidoProduto();

                                pedidoProduto.Pedido = pedido;
                                pedidoProduto.PesoTotalEmbalagem = 0;
                                pedidoProduto.PesoUnitario = 0;
                                pedidoProduto.Produto = produtoEmbarcador;
                                pedidoProduto.Quantidade = 0;
                                pedidoProduto.QuantidadeEmbalagem = 0;
                                pedidoProduto.ValorProduto = 0;

                                repositorioPedidoProduto.Inserir(pedidoProduto);
                            }
                        }

                        decimal totalCubagem = 0, totalPeso = 0, totalPalet = 0;

                        SalvarListaProdutos(ref pedido, unitOfWork, ref totalCubagem, ref totalPeso, ref totalPalet, false);
                        SalvarListaCliente(pedido, unitOfWork);
                        await SalvarListaFronteirasAsync(pedido, unitOfWork);
                        SalvarComponentesFrete(pedido, unitOfWork);
                        SalvarAcrescimoDesconto(pedido, unitOfWork);
                        SalvarContatos(pedido, unitOfWork);
                        SalvarPedidoAdicionais(pedido, unitOfWork);
                        SalvarPedidoEcommerce(pedido, unitOfWork);

                        if (ConfiguracaoEmbarcador.SolicitarValorFretePorTonelada && pedido.TipoOperacao != null && pedido.RotaFrete != null && !string.IsNullOrWhiteSpace(pedido.RotaFrete.CodigoIntegracaoValePedagio) && pedido.ValorPedagioRota == 0)
                        {
                            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);

                            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoPagbem integracaoPagbem = servicoValePedagio.ObterIntegracaoPagbem(pedido.TipoOperacao, null, null, true, TipoServicoMultisoftware);
                            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();

                            if (integracaoPagbem != null && configuracaoIntegracao != null)
                            {
                                int.TryParse(pedido.RotaFrete.CodigoIntegracaoValePedagio, out int codigoRoteiro);
                                int qtdEixos = pedido.ModeloVeicularCarga?.NumeroEixos.Value ?? 0;
                                qtdEixos = integracaoPagbem.QuantidadeEixosPadraoValePedagio > 0 ? integracaoPagbem.QuantidadeEixosPadraoValePedagio : qtdEixos;//solititação NA
                                if (codigoRoteiro > 0 && integracaoPagbem != null)
                                {
                                    pedido.ValorPedagioRota = servicoPagbem.RetornarValorPedagio(integracaoPagbem, codigoRoteiro, qtdEixos, unitOfWork, out string mensagemRetorno);
                                    if (pedido.ValorPedagioRota > 0)
                                    {
                                        int qtdEixosPadrao = configuracaoIntegracao.QuantidadeEixosPadrao;
                                        if (qtdEixosPadrao <= 0)
                                            qtdEixosPadrao = 32;
                                        pedido.ValorFreteToneladaTerceiro = Math.Round(pedido.ValorFreteToneladaTerceiro - (pedido.ValorPedagioRota / qtdEixosPadrao), 2, MidpointRounding.ToEven);
                                    }
                                }
                            }
                        }

                        if (pedido.PedidoIntegradoEmbarcador && pedido.ColetaEmProdutorRural)
                        {
                            if ((listaCpfCnpjDestinatario.Count > 1) || (listaCpfCnpjRemetente.Count > 1))
                                throw new ControllerException(string.Format(Localization.Resources.Pedidos.Pedido.NaoEPossivelGerarPreCargaParaPedidosComMultiplos, descricaoTipoMultiplasEntidades));

                            if (!servicoPreCarga.CriarPreCarga(out string erro, pedido, true, TipoServicoMultisoftware, Usuario))
                                throw new ControllerException(erro);
                        }

                        if (totalCubagem > 0)
                            pedido.CubagemTotal = totalCubagem;

                        if (totalPeso > 0)
                        {
                            pedido.PesoTotal = totalPeso;
                            pedido.PesoSaldoRestante = totalPeso;
                        }

                        if (totalPalet > 0)
                            pedido.NumeroPaletesFracionado = totalPalet;

                        SalvarNovoPercursoEstados(pedido, unitOfWork);

                        if (pedido.TipoOperacao != null && pedido.TipoOperacao.UtilizarFatorCubagem && pedido.TipoOperacao.FatorCubagem.HasValue)
                            pedido.PesoCubado = pedido.CubagemTotal * pedido.TipoOperacao.FatorCubagem.Value;

                        if (pedido.Cotacao)
                            pedido.ValorFreteCotado = Servicos.Embarcador.Carga.Frete.CalcularFretePorPedido(pedido, unitOfWork, _conexao.StringConexao, TipoServicoMultisoftware, ConfiguracaoEmbarcador);

                        pedido.PalletSaldoRestante = (pedido.NumeroPaletes + pedido.NumeroPaletesFracionado);

                        repositorioPedido.Atualizar(pedido);

                        if (ConfiguracaoEmbarcador.ValidarLimiteCreditoNoPedido)
                        {
                            Dominio.Entidades.Cliente tomador = pedido.ObterTomador();
                            if (tomador != null)
                            {
                                decimal valorPendente = repositorioDocumentoFaturamento.ObterValorTotalNaoPagoPorPessoaOuGrupoPessoas(tomador.GrupoPessoas?.Codigo ?? 0, tomador.CPF_CNPJ);
                                if (valorPendente >= tomador.ValorLimiteFaturamento || (tomador.GrupoPessoas != null && valorPendente >= tomador.GrupoPessoas.ValorLimiteFaturamento))
                                {
                                    mensagemBloqueioFinanceiro = new StringBuilder();
                                    decimal valorMaximoPendentePagamento = valorPendente >= tomador.ValorLimiteFaturamento ? tomador.ValorLimiteFaturamento.Value : (tomador.GrupoPessoas?.ValorLimiteFaturamento.Value ?? 0m);
                                    mensagemBloqueioFinanceiro.Append(tomador.Descricao);

                                    if (tomador.GrupoPessoas != null)
                                        mensagemBloqueioFinanceiro.Append($" ({tomador.GrupoPessoas.Descricao})");

                                    mensagemBloqueioFinanceiro.Append(string.Format(Localization.Resources.Pedidos.Pedido.PossuiUmValorAPagarMaiorQuePermitido, valorPendente.ToString("n2"), valorMaximoPendentePagamento.ToString("n2")));

                                    if (!possuiPermissao)
                                        throw new ControllerException(mensagemBloqueioFinanceiro.ToString());
                                }
                                DateTime? dataUltimoVencimento = repositorioTitulo.BuscarVencimentoMaisAntigoPorPessoaOuGrupoPessoas(tomador.GrupoPessoas?.Codigo ?? 0, tomador.CPF_CNPJ);
                                if (dataUltimoVencimento.HasValue && dataUltimoVencimento.Value > DateTime.MinValue)
                                {
                                    double diferencaDia = (DateTime.Now.Date - dataUltimoVencimento.Value.Date).TotalDays;
                                    if ((int)diferencaDia > 0 && (int)diferencaDia >= tomador.DiasEmAbertoAposVencimento || (tomador.GrupoPessoas != null && (int)diferencaDia >= tomador.GrupoPessoas.DiasEmAbertoAposVencimento))
                                    {
                                        mensagemBloqueioFinanceiro = new StringBuilder();
                                        int diaMaximoPendentePagamento = (int)diferencaDia >= tomador.DiasEmAbertoAposVencimento ? tomador.DiasEmAbertoAposVencimento.Value : (tomador.GrupoPessoas?.DiasEmAbertoAposVencimento.Value ?? 0);
                                        mensagemBloqueioFinanceiro.Append(tomador.Descricao);

                                        if (tomador.GrupoPessoas != null)
                                            mensagemBloqueioFinanceiro.Append($" ({tomador.GrupoPessoas.Descricao})");

                                        mensagemBloqueioFinanceiro.Append(string.Format(Localization.Resources.Pedidos.Pedido.PossuiTitutloEmAbertoComQuantiaDe, diferencaDia.ToString("n0"), diaMaximoPendentePagamento.ToString("n0")));

                                        if (!possuiPermissao)
                                            throw new ControllerException(mensagemBloqueioFinanceiro.ToString());
                                    }
                                }

                                if (valorPendente >= tomador.GrupoPessoas?.ValorLimiteFaturamento && !pedido.TipoOperacao.DeslocamentoVazio)
                                {
                                    decimal valorMaximoPendentePagamento = tomador.GrupoPessoas?.ValorLimiteFaturamento ?? 0m;

                                    StringBuilder mensagem = new StringBuilder();

                                    mensagem.Append(tomador.Descricao);

                                    if (tomador.GrupoPessoas != null)
                                        mensagem.Append($" ({tomador.GrupoPessoas.Descricao})");

                                    mensagem.Append($" possui um valor a pagar ({valorPendente.ToString("n2")}) maior que o limite permitido ({valorMaximoPendentePagamento.ToString("n2")}), não sendo possível prosseguir com o pedido. Entre em contato com o financeiro para mais informações!");

                                    throw new ControllerException(mensagem.ToString());
                                }
                            }
                        }

                        servOcorrenciaPedido.ProcessarOcorrenciaPedido(EventoColetaEntrega.PedidoGerado, pedido, ConfiguracaoEmbarcador, this.Cliente);

                        ValidarRegrasPedido(pedido, unitOfWork, permissoesPersonalizadas);

                        pedidosAdicionados.Add(pedido);
                    }
                }

                AdicionarNotasFiscais(pedidoBase, pedidosAdicionados, unitOfWork);

                if (IsGerarCargaAutomaticamente(pedidoBase) && pedidosAdicionados.Any(o => o.SituacaoPedido != SituacaoPedido.AutorizacaoPendente) && pedidosAdicionados.Any(o => o.SituacaoPedido != SituacaoPedido.AgAprovacao))
                {
                    if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    {
                        PreencherCodigoCargaEmbarcador(unitOfWork, pedidoBase);

                        foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoAdicionado in pedidosAdicionados)
                        {
                            pedidoAdicionado.CodigoCargaEmbarcador = pedidoBase.CodigoCargaEmbarcador;
                            pedidoAdicionado.AdicionadaManualmente = pedidoBase.AdicionadaManualmente;

                            repositorioPedido.Atualizar(pedidoAdicionado);
                        }
                    }

                    CriarCarga(unitOfWork, pedidosAdicionados, ConfiguracaoEmbarcador);

                    foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidosAdicionados)
                        PreencherDadosRotaFreteClienteDeslocamento(unitOfWork, pedido);

                    if (configuracaoPedido.ExigirRotaRoteirizadaNoPedido && (pedidosAdicionados.Any(o => o.RotaFrete == null) || pedidosAdicionados.Any(o => o.RotaFrete.SituacaoDaRoteirizacao != SituacaoRoteirizacao.Concluido)))
                        return new JsonpResult(false, true, "Necessário que a rota esteja roteirizada, favor verificar o cadastro da rota.");

                }

                foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidosAdicionados)
                {
                    NotificarJanelaCarregamento(pedido, unitOfWork);
                    AtualizarDatasPrevisaoColetaEntrega(pedido, unitOfWork);

                    if ((pedido.GrupoPessoas != null && pedido.GrupoPessoas.TornarPedidosPrioritarios) || (pedido.Remetente != null && pedido.Remetente.GrupoPessoas != null && pedido.Remetente.GrupoPessoas.TornarPedidosPrioritarios))
                        GerarNotificacaoPedidoNovo(unitOfWork, pedido, TipoServicoMultisoftware);

                    try
                    {
                        string erros = string.Empty;
                        servicoHistorico.InserirHistoricoVinculo(unitOfWork, ref erros, LocalVinculo.Pedido, pedido.VeiculoTracao, pedido.Veiculos, pedido.Motoristas, DateTime.Now, null, pedido, null);
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro(ex);
                    }
                }

                if (integracao?.PossuiIntegracaoTrizy ?? false)
                {
                    foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidosAdicionados)
                    {
                        GerarIntegracaoTrizy(pedido, unitOfWork);
                    }
                }

                if (integracao != null && integracao.PossuiIntegracaoAX && !integracao.NaoRealizarIntegracaoPedido)
                {
                    foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidosAdicionados)
                    {
                        GerarIntegracaoAX(pedido, unitOfWork);
                    }
                }

                if (pedidoBase.TipoOperacao != null && repositorioTipoOperacaoIntegracao.ExisteTipoOperacaoIntegracao(TipoIntegracao.Routeasy, pedidoBase.TipoOperacao.Codigo))
                    new Servicos.Embarcador.Integracao.IntegracaoPedidoRoterizador(unitOfWork).AdicionarParaIntegracaoAutomaticamente(pedidosAdicionados.Select(o => o.Codigo).ToList(), TipoRoteirizadorIntegracao.EnviarPedido);

                GerarIntegracoesPedidos(pedidosAdicionados, unitOfWork);

                unitOfWork.CommitChanges();

                Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoRetorno = pedidosAdicionados.Count == 1 ? pedidosAdicionados.FirstOrDefault() : null;

                if (pedidoRetorno != null && possuiPermissao && mensagemBloqueioFinanceiro != null && mensagemBloqueioFinanceiro.Length > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, pedidoRetorno, null, string.Format(Localization.Resources.Pedidos.Pedido.UsuarioLiberouPedidoSemCredito, mensagemBloqueioFinanceiro.ToString()), unitOfWork);

                return new JsonpResult(new
                {
                    Codigo = pedidoRetorno?.Codigo ?? 0,
                    Numero = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? pedidoRetorno?.Numero.ToString() ?? string.Empty : pedidoRetorno?.NumeroPedidoEmbarcador ?? string.Empty,
                    NumeroCarga = pedidoRetorno != null ? string.Join(", ", repositorioCargaPedido.BuscarNumeroCargasPorPedido(pedidoRetorno.Codigo)) : string.Empty
                });
            }
            catch (ControllerException ex)
            {
                await unitOfWork.RollbackAsync();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (BaseException excecao)
            {
                await unitOfWork.RollbackAsync();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                await unitOfWork.RollbackAsync();
                return new JsonpResult(false, Localization.Resources.Pedidos.Pedido.OcorreuUmaFalhaAoAdicionar);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                bool.TryParse(Request.Params("Cotacao"), out bool cotacao);

                string codigoPedidoCliente = Request.Params("CodigoPedidoCliente");

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Pedidos/Pedido");
                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Embarcador.Pedidos.Porto repPorto = new Repositorio.Embarcador.Pedidos.Porto(unitOfWork);
                Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoEndereco repPedidoEndereco = new Repositorio.Embarcador.Pedidos.PedidoEndereco(unitOfWork);
                Repositorio.Embarcador.Pessoas.ClienteOutroEndereco repClienteOutroEndereco = new Repositorio.Embarcador.Pessoas.ClienteOutroEndereco(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Cargas.TipoDeCarga repTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
                Repositorio.Embarcador.WMS.Deposito repDeposito = new Repositorio.Embarcador.WMS.Deposito(unitOfWork);
                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoProduto repPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(unitOfWork);
                Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedidoProduto repCargaPedidoProduto = new Repositorio.Embarcador.Cargas.CargaPedidoProduto(unitOfWork);
                Repositorio.RotaFrete repRotaFrete = new Repositorio.RotaFrete(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                Servicos.Embarcador.Pedido.Pedido serPedido = new Servicos.Embarcador.Pedido.Pedido(unitOfWork);
                Servicos.Embarcador.CIOT.Pagbem serPagbem = new Servicos.Embarcador.CIOT.Pagbem();
                Servicos.Embarcador.Frota.ValePedagio servicoValePedagio = new Servicos.Embarcador.Frota.ValePedagio(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoTerminalImportacao repTipoTerminalImportacao = new Repositorio.Embarcador.Pedidos.TipoTerminalImportacao(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega repConfiguracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repConfiguracaoCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(unitOfWork);
                Repositorio.Embarcador.Pedidos.Navio repNavio = new Repositorio.Embarcador.Pedidos.Navio(unitOfWork);
                Repositorio.Embarcador.Pedidos.CanalEntrega repCanalEntrega = new Repositorio.Embarcador.Pedidos.CanalEntrega(unitOfWork);
                Repositorio.Embarcador.Pedidos.CanalVenda repositorioCanalVenda = new Repositorio.Embarcador.Pedidos.CanalVenda(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoTipoPagamento repPedidoTipoPagamento = new Repositorio.Embarcador.Pedidos.PedidoTipoPagamento(unitOfWork);
                Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unitOfWork);
                Repositorio.Embarcador.Logistica.CentroCarregamento repCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unitOfWork);
                Repositorio.Embarcador.Localidades.Regiao repRegiao = new Repositorio.Embarcador.Localidades.Regiao(unitOfWork);
                Repositorio.Embarcador.Logistica.CentroCustoViagem repCentroCustoViagem = new Repositorio.Embarcador.Logistica.CentroCustoViagem(unitOfWork);
                Repositorio.Embarcador.Escrituracao.ContratoFreteCliente repContratoFreteCliente = new Repositorio.Embarcador.Escrituracao.ContratoFreteCliente(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfigTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Pedidos.ContainerTipo repContainerTipo = new Repositorio.Embarcador.Pedidos.ContainerTipo(unitOfWork);

                var repMotivoPedido = new Repositorio.Embarcador.Pedidos.MotivoPedido(unitOfWork);

                Servicos.Embarcador.PreCarga.PreCarga servicoPreCarga = new Servicos.Embarcador.PreCarga.PreCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = repConfiguracaoPedido.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoCarga = repConfiguracaoCarga.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega = repConfiguracaoControleEntrega.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repConfigTMS.BuscarConfiguracaoPadrao();
                Servicos.Embarcador.Frota.OrdemServicoVeiculoManutencao ordemServicoVeiculoManutencao = new Servicos.Embarcador.Frota.OrdemServicoVeiculoManutencao(unitOfWork);
                Servicos.Embarcador.Carga.HistoricoVinculo serHistorico = new Servicos.Embarcador.Carga.HistoricoVinculo(unitOfWork);

                double cpfCnpjPontoPartida = Request.GetDoubleParam("PontoPartida");
                long codigoPedidoTipoPagamento = Request.GetLongParam("PedidoTipoPagamento");
                int codigoCentroResultado = Request.GetIntParam("CentroResultado");
                int codigoCentroCarregamento = Request.GetIntParam("CentroCarregamento");
                List<int> listaModelosVeiculares = ObterListaModelosVeiculares();

                int.TryParse(Request.Params("Codigo"), out int codigo);

                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = await repPedido.BuscarPorCodigoAsync(codigo, true);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    if (repCargaPedido.PossuiCargaInaptaAAlteracao(pedido.Codigo))
                        return new JsonpResult(false, true, Localization.Resources.Pedidos.Pedido.PedidoEstaVinculadoAUmaCarga);
                }

                if (!this.ValidarTabelaFrete(unitOfWork, pedido))
                    return new JsonpResult(false, true, Localization.Resources.Pedidos.Pedido.PedidoNaoPossuiTabelaFreteConfigurada);

                unitOfWork.Start();

                Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica = ObterOperadorLogistica(unitOfWork);

                if (ConfiguracaoEmbarcador.UtilizarLocalidadePrestacaoPedido)
                {
                    int codigoLocalidadeInicioPrestacao = Request.GetIntParam("LocalidadeInicioPrestacao");
                    int codigoLocalidadeTerminoPrestacao = Request.GetIntParam("LocalidadeTerminoPrestacao");

                    pedido.LocalidadeInicioPrestacao = codigoLocalidadeInicioPrestacao > 0 ? repLocalidade.BuscarPorCodigo(codigoLocalidadeInicioPrestacao) : null;
                    pedido.LocalidadeTerminoPrestacao = codigoLocalidadeTerminoPrestacao > 0 ? repLocalidade.BuscarPorCodigo(codigoLocalidadeTerminoPrestacao) : null;
                }
                else
                {
                    pedido.LocalidadeInicioPrestacao = null;
                    pedido.LocalidadeTerminoPrestacao = null;
                }

                pedido.PontoPartida = cpfCnpjPontoPartida > 0D ? repCliente.BuscarPorCPFCNPJ(cpfCnpjPontoPartida) : null;
                pedido.ProdutoPredominante = Request.GetStringParam("ProdutoPredominante");
                pedido.CentroResultado = codigoCentroResultado > 0 ? repCentroResultado.BuscarPorCodigo(codigoCentroResultado) : null;
                pedido.CentroCarregamento = codigoCentroCarregamento > 0 ? repCentroCarregamento.BuscarPorCodigo(codigoCentroCarregamento) : null;

                pedido.Distancia = Request.GetDecimalParam("Distancia");
                pedido.PedidoTipoPagamento = codigoPedidoTipoPagamento > 0L ? repPedidoTipoPagamento.BuscarPorCodigo(codigoPedidoTipoPagamento, false) : null;
                pedido.CodigoPedidoCliente = codigoPedidoCliente;
                pedido.PossuiCarga = Request.GetBoolParam("PossuiCarregamento");
                pedido.ValorCarga = Request.GetNullableDecimalParam("ValorCarregamento");
                pedido.PossuiDescarga = Request.GetBoolParam("PossuiDescarga");
                pedido.ValorDescarga = Request.GetNullableDecimalParam("ValorDescarga");
                pedido.PossuiDeslocamento = Request.GetBoolParam("PossuiDeslocamento");
                pedido.ValorDeslocamento = Request.GetNullableDecimalParam("ValorDeslocamento");
                pedido.PossuiDiaria = Request.GetBoolParam("PossuiDiaria");
                pedido.ValorDiaria = Request.GetNullableDecimalParam("ValorDiaria");
                pedido.QuantidadeNotasFiscais = Request.GetIntParam("QuantidadeNotasFiscais");
                pedido.ObservacaoInterna = Request.GetNullableStringParam("ObservacaoInterna");
                pedido.DeclaracaoObservacaoCRT = Request.GetNullableStringParam("DeclaracaoObservacaoCRT");
                pedido.EntregaAgendada = Request.GetBoolParam("EntregaAgendada");
                pedido.QuebraMultiplosCarregamentos = Request.GetBoolParam("QuebraMultiplosCarregamentos");
                pedido.QuantidadeVolumesPrevios = Request.GetIntParam("QuantidadeVolumesPrevios");
                var codigoMotivoPedido = int.TryParse(Request.Params("MotivoPedido"), out int cod) ? cod : 0;
                var motivoPedido = repMotivoPedido.BuscarPorCodigo(codigoMotivoPedido);
                pedido.MotivoPedido = motivoPedido;

                DateTime dataBaseCRT;
                DateTime.TryParseExact(Request.Params("DataBaseCRT"), "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out dataBaseCRT);
                if (dataBaseCRT != DateTime.MinValue)
                    pedido.DataBaseCRT = dataBaseCRT;
                else
                    pedido.DataBaseCRT = null;

                DateTime dataFinalColeta;
                DateTime.TryParseExact(Request.Params("DataFinalColeta"), "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out dataFinalColeta);
                if (dataFinalColeta != DateTime.MinValue)
                    pedido.DataFinalColeta = dataFinalColeta;

                DateTime dataInicialColeta;
                DateTime.TryParseExact(Request.Params("DataInicialColeta"), "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out dataInicialColeta);
                if (dataInicialColeta != DateTime.MinValue)
                    pedido.DataInicialColeta = dataInicialColeta;

                DateTime dataPrevisaoSaida;
                DateTime.TryParseExact(Request.Params("DataPrevisaoSaida"), "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out dataPrevisaoSaida);
                if (dataPrevisaoSaida != DateTime.MinValue)
                    pedido.DataPrevisaoSaida = dataPrevisaoSaida;

                DateTime dataColeta;
                DateTime.TryParseExact(Request.Params("DataColeta"), "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out dataColeta);
                if (dataColeta != DateTime.MinValue)
                {
                    pedido.DataCarregamentoPedido = dataColeta;
                    //if (!pedido.DataInicialColeta.HasValue)
                    pedido.DataInicialColeta = dataColeta;
                }
                else
                    pedido.DataCarregamentoPedido = pedido.DataInicialColeta;

                DateTime previsaoEntrega;
                DateTime.TryParseExact(Request.Params("PrevisaoEntrega"), "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out previsaoEntrega);
                if (previsaoEntrega != DateTime.MinValue)
                    pedido.PrevisaoEntrega = previsaoEntrega;
                else
                {
                    DateTime.TryParseExact(Request.Params("DataPrevisaoEntrega"), "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out previsaoEntrega);
                    if (previsaoEntrega != DateTime.MinValue)
                        pedido.PrevisaoEntrega = previsaoEntrega;

                }

                if (dataPrevisaoSaida != DateTime.MinValue && previsaoEntrega != DateTime.MinValue && dataPrevisaoSaida > previsaoEntrega)
                    throw new ControllerException(Localization.Resources.Pedidos.Pedido.DataPrevisaoSaidaEstaMaiorQueDataPrevisaoRetorno);

                pedido.DataAgendamento = Request.GetNullableDateTimeParam("DataAgendamento");

                if (pedido.DataAgendamento.HasValue && pedido.DataAgendamento.Value.Date < DateTime.Now.Date)
                    throw new ControllerException("A data do agendamento não pode ser menor que a data atual.");

                if (pedido.Empresa != null && pedido.Empresa.Status != "A")
                    throw new ControllerException(Localization.Resources.Pedidos.Pedido.NaoEPossivelCriarPedidoComEmpresaInativa);

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repositorioCargaEntrega.BuscarPorPedido(pedido.Codigo);

                if (cargaEntrega != null)
                {
                    Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.AtualizarDataAgendamentoPorPedido(new List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> { cargaEntrega }, new List<Dominio.Entidades.Embarcador.Pedidos.Pedido> { pedido }, unitOfWork);

                    if ((configuracaoControleEntrega?.UtilizarPrevisaoEntregaPedidoComoDataPrevista) ?? false && (configuracaoEmbarcador?.PermitirAtualizarPrevisaoEntregaPedidoControleEntrega ?? false))
                        Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.AtualizarDataPrevisaoEntregaPorEntregaPedido(pedido, cargaEntrega, unitOfWork);
                }

                pedido.DataCarregamentoCarga = Request.GetNullableDateTimeParam("DataCarregamento");
                pedido.DataInicialViagemFaturada = Request.GetNullableDateTimeParam("DataInicialViagemFaturada");
                pedido.DataFinalViagemFaturada = Request.GetNullableDateTimeParam("DataFinalViagemFaturada");
                pedido.DataValidade = Request.GetNullableDateTimeParam("DataValidade");
                pedido.DataInicioJanelaDescarga = Request.GetNullableDateTimeParam("DataInicioJanelaDescarga");
                pedido.DataUltimaLiberacao = Request.GetNullableDateTimeParam("DataUltimaLiberacao");
                pedido.RotaEmbarcador = Request.GetStringParam("RotaEmbarcador");
                pedido.UsuarioCriacaoRemessa = Request.GetStringParam("UsuarioCriacaoRemessa");
                pedido.NumeroOrdem = Request.GetStringParam("NumeroOrdem");
                pedido.RegiaoDestino = repRegiao.BuscarPorCodigo(Request.GetIntParam("RegiaoDestino"));

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    if (ConfiguracaoEmbarcador.InformarDataViagemExecutadaPedido)
                    {
                        pedido.DataInicialViagemExecutada = Request.GetNullableDateTimeParam("DataInicialViagemExecutada");
                        pedido.DataFinalViagemExecutada = Request.GetNullableDateTimeParam("DataFinalViagemExecutada");

                        if (!pedido.DataInicialViagemExecutada.HasValue)
                            pedido.DataInicialViagemExecutada = pedido.DataPrevisaoSaida;

                        if (!pedido.DataFinalViagemExecutada.HasValue)
                            pedido.DataFinalViagemExecutada = pedido.PrevisaoEntrega;
                    }
                    else
                    {
                        if (!pedido.DataInicialViagemExecutada.HasValue)
                            pedido.DataInicialViagemExecutada = pedido.DataPrevisaoSaida;

                        if (!pedido.DataFinalViagemExecutada.HasValue)
                            pedido.DataFinalViagemExecutada = pedido.PrevisaoEntrega;
                    }

                    if (!pedido.DataInicialViagemFaturada.HasValue)
                        pedido.DataInicialViagemFaturada = pedido.DataPrevisaoSaida;

                    if (!pedido.DataFinalViagemFaturada.HasValue)
                        pedido.DataFinalViagemFaturada = pedido.PrevisaoEntrega;

                    if (pedido.DataFinalViagemFaturada < pedido.DataInicialViagemFaturada)
                        throw new ControllerException(Localization.Resources.Pedidos.Pedido.DataInicialViajemFaturadaEMaiorQueDataFinalViajemFaturada);
                }

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa tipoPessoa;
                Enum.TryParse(Request.Params("TipoPessoa"), out tipoPessoa);
                pedido.TipoPessoa = tipoPessoa;

                Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco pedidoEnderecoOrigem = new Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco();
                Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco pedidoEnderecoDestino = new Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco();

                double remetente = 0;
                double.TryParse(Request.Params("Remetente"), out remetente);

                bool mudouRemetente = (pedido.Remetente?.CPF_CNPJ ?? 0) != remetente;

                if (remetente > 0)
                {
                    pedido.Remetente = repCliente.BuscarPorCPFCNPJ(remetente);
                    serPedido.PreecherEnderecoPedido(ref pedidoEnderecoOrigem, pedido.Remetente);
                }
                else
                    pedido.Remetente = null;

                int grupoPessoa = 0;
                int.TryParse(Request.Params("GrupoPessoa"), out grupoPessoa);
                if (grupoPessoa > 0)
                {
                    pedido.GrupoPessoas = repGrupoPessoas.BuscarPorCodigo(grupoPessoa);
                }
                else
                    pedido.GrupoPessoas = null;

                int codigoVeiculo = Request.GetIntParam("Veiculo");

                bool alterouVeiculo = !((pedido.VeiculoTracao?.Codigo ?? 0) == codigoVeiculo) && !(pedido.Veiculos?.Any(o => (o?.Codigo ?? 0) == codigoVeiculo) ?? true);

                if ((!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_PermiteInserirAlterarVeiculoNaCargaOuPedido) && !this.Usuario.UsuarioAdministrador) && alterouVeiculo)
                    throw new ControllerException(Localization.Resources.Pedidos.Pedido.UsuarioNaoTemAcessoParaAlterarVeiculo);
                else
                    SetarVeiculosPedido(pedido, unitOfWork);


                ordemServicoVeiculoManutencao.VeiculoIndisponivelParaTransporte(pedido);

                pedido.ValorFreteNegociado = Request.GetDecimalParam("ValorFreteNegociado");
                pedido.ValorFreteTransportadorTerceiro = Request.GetDecimalParam("ValorFreteTransportadorTerceiro");

                pedido.ValorFreteToneladaNegociado = Request.GetDecimalParam("ValorFreteToneladaNegociado");
                pedido.ValorFreteToneladaTerceiro = Request.GetDecimalParam("ValorFreteToneladaTerceiro");

                int destino = Request.GetIntParam("Destino", 0);
                if (destino > 0)
                    pedido.Destino = repLocalidade.BuscarPorCodigo(destino);

                double destinatario = 0;
                double.TryParse(Request.Params("Destinatario"), out destinatario);
                bool mudouDestinatario = destinatario != (pedido.Destinatario?.CPF_CNPJ ?? 0);
                if (destinatario > 0)
                {
                    pedido.Destinatario = repCliente.BuscarPorCPFCNPJ(destinatario);
                    serPedido.PreecherEnderecoPedido(ref pedidoEnderecoDestino, pedido.Destinatario);
                    pedido.DestinatarioNaoInformado = false;
                }
                else
                {
                    pedido.Destinatario = null;
                    pedido.DestinatarioNaoInformado = true;
                }


                double expedidor = 0;
                double.TryParse(Request.Params("Expedidor"), out expedidor);
                if (expedidor > 0)
                {
                    pedido.Expedidor = repCliente.BuscarPorCPFCNPJ(expedidor);

                    if (configuracaoPedido.UtilizarEnderecoExpedidorRecebedorPedido)
                        serPedido.PreecherEnderecoPedido(ref pedidoEnderecoOrigem, pedido.Expedidor);
                }
                else
                    pedido.Expedidor = null;

                double recebedor = Request.GetDoubleParam("Recebedor");

                if (recebedor > 0)
                {
                    pedido.Recebedor = repCliente.BuscarPorCPFCNPJ(recebedor);
                    if (configuracaoPedido.UtilizarEnderecoExpedidorRecebedorPedido)
                        serPedido.PreecherEnderecoPedido(ref pedidoEnderecoDestino, pedido.Recebedor);
                }
                else
                    pedido.Recebedor = null;

                if (bool.Parse(Request.Params("UsarOutroEnderecoOrigem")))
                {
                    pedido.UsarOutroEnderecoOrigem = true;
                    if (int.Parse(Request.Params("LocalidadeClienteOrigem")) > 0)
                    {
                        pedidoEnderecoOrigem.ClienteOutroEndereco = repClienteOutroEndereco.BuscarPorCodigo(int.Parse(Request.Params("LocalidadeClienteOrigem")));
                        PreecherOutroEnderecoPedido(ref pedidoEnderecoOrigem);
                    }
                    else
                    {
                        pedidoEnderecoOrigem.Bairro = Request.Params("BairroOrigem");
                        pedidoEnderecoOrigem.CEP = Request.Params("CEPOrigem");
                        pedidoEnderecoOrigem.Localidade = repLocalidade.BuscarPorCodigo(int.Parse(Request.Params("Origem")));

                        pedidoEnderecoOrigem.Complemento = Request.Params("ComplementoOrigem");
                        pedidoEnderecoOrigem.Endereco = Request.Params("EnderecoOrigem");
                        pedidoEnderecoOrigem.Numero = Request.Params("NumeroOrigem");
                        pedidoEnderecoOrigem.Telefone = Request.Params("Telefone1Origem");
                        pedidoEnderecoOrigem.IE_RG = Request.Params("RGIE1Origem");
                    }
                }
                else
                    pedido.UsarOutroEnderecoOrigem = false;

                int serie = 0;
                int.TryParse(Request.Params("SerieCTe"), out serie);
                if (serie > 0)
                {
                    pedido.EmpresaSerie = new Dominio.Entidades.EmpresaSerie() { Codigo = serie };
                }
                else
                    pedido.EmpresaSerie = null;

                bool pedidoTransbordo = false;
                bool.TryParse(Request.Params("PedidoTransbordo"), out pedidoTransbordo);

                pedido.DisponibilizarPedidoParaColeta = Request.GetBoolParam("DisponibilizarPedidoParaColeta");

                if (bool.Parse(Request.Params("UsarOutroEnderecoDestino")))
                {

                    pedido.UsarOutroEnderecoDestino = true;
                    if (int.Parse(Request.Params("LocalidadeClienteDestino")) > 0)
                    {
                        pedidoEnderecoDestino.ClienteOutroEndereco = repClienteOutroEndereco.BuscarPorCodigo(int.Parse(Request.Params("LocalidadeClienteDestino")));
                        PreecherOutroEnderecoPedido(ref pedidoEnderecoDestino);
                    }
                    else
                    {
                        pedidoEnderecoDestino.Bairro = Request.Params("BairroDestino");
                        pedidoEnderecoDestino.CEP = Request.Params("CEPDestino");
                        pedidoEnderecoDestino.Localidade = repLocalidade.BuscarPorCodigo(int.Parse(Request.Params("Destino")));

                        pedidoEnderecoDestino.Complemento = Request.Params("ComplementoDestino");
                        pedidoEnderecoDestino.Endereco = Request.Params("EnderecoDestino");
                        pedidoEnderecoDestino.Numero = Request.Params("NumeroDestino");
                        pedidoEnderecoDestino.Telefone = Request.Params("Telefone1Destino");
                        pedidoEnderecoDestino.IE_RG = Request.Params("RGIE1Destino");
                    }
                }
                else
                {
                    pedido.UsarOutroEnderecoDestino = false;
                }

                pedido.PedidoTransbordo = pedidoTransbordo;

                if (pedidoEnderecoOrigem.Localidade != null)
                    repPedidoEndereco.Inserir(pedidoEnderecoOrigem);
                if (pedidoEnderecoDestino.Localidade != null)
                    repPedidoEndereco.Inserir(pedidoEnderecoDestino);

                bool recriarRota = false;
                if (pedido.Origem != null && pedidoEnderecoOrigem.Localidade != null && pedido.Destino != null && pedidoEnderecoDestino.Localidade != null)
                {
                    if (pedido.Origem.Codigo != pedidoEnderecoOrigem.Localidade.Codigo || pedido.Destino.Codigo != pedidoEnderecoDestino.Localidade.Codigo)
                        recriarRota = true;
                }

                if (pedidoEnderecoOrigem.Localidade != null)
                {
                    pedido.Origem = pedidoEnderecoOrigem.Localidade;
                    pedido.EnderecoOrigem = pedidoEnderecoOrigem;
                }
                else
                {
                    int origem = 0;
                    int.TryParse(Request.Params("Origem"), out origem);
                    if (origem > 0)
                    {
                        if (pedido.Origem.Codigo != origem)
                            recriarRota = true;

                        pedido.Origem = repLocalidade.BuscarPorCodigo(origem);
                    }
                    else
                        pedido.Origem = null;
                }

                if (pedidoEnderecoDestino.Localidade != null)
                {
                    pedido.Destino = pedidoEnderecoDestino.Localidade;
                    pedido.EnderecoDestino = pedidoEnderecoDestino;
                }

                PreencherDadosRotaFrete(unitOfWork, pedido);

                if (!pedido.GerarAutomaticamenteCargaDoPedido)
                    pedido.GerarAutomaticamenteCargaDoPedido = bool.Parse(Request.Params("GerarAutomaticamenteCargaDoPedido"));
                else
                {
                    if (bool.Parse(Request.Params("GerarAutomaticamenteCargaDoPedido")) == false)
                        throw new ControllerException(Localization.Resources.Pedidos.Pedido.UmaCargaJaFoiGeradaParaEssePedido);
                }

                pedido.ImprimirObservacaoCTe = bool.Parse(Request.Params("ImprimirObservacaoCTe"));

                Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoEmbarcador = null;
                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    int codigoProdutoEmbarcador = 0;
                    int.TryParse(Request.Params("ProdutoEmbarcador"), out codigoProdutoEmbarcador);
                    produtoEmbarcador = repProdutoEmbarcador.BuscarPorCodigo(codigoProdutoEmbarcador);

                    if (produtoEmbarcador != null)
                    {
                        if ((produtoEmbarcador.Cliente != null && produtoEmbarcador.Cliente.CPF_CNPJ != pedido.Remetente.CPF_CNPJ) && (produtoEmbarcador.GrupoPessoas != null && (pedido.Remetente.GrupoPessoas != null && pedido.Remetente.GrupoPessoas.Codigo != produtoEmbarcador.GrupoPessoas.Codigo)))
                            throw new ControllerException(Localization.Resources.Pedidos.Pedido.ProdutoInformadoNaoPertenceAoTomadorDoPedido);

                        pedido.ProdutoPredominante = produtoEmbarcador.Descricao;
                    }
                }

                int filial = 0;
                int.TryParse(Request.Params("Filial"), out filial);
                if (filial > 0)
                {
                    pedido.Filial = repFilial.BuscarPorCodigo(filial);
                }
                int empresa = 0;
                int.TryParse(Request.Params("Empresa"), out empresa);
                if (empresa > 0)
                {
                    pedido.Empresa = repEmpresa.BuscarPorCodigo(empresa);
                }

                decimal diferencaPallet = 0;
                int novaQtdePallet = 0;
                decimal novaQtdePalletFracionado = 0;

                if (!string.IsNullOrWhiteSpace(Request.Params("Pallets")))
                    novaQtdePallet = int.Parse(Request.Params("Pallets"));
                else
                    novaQtdePallet = 0;

                diferencaPallet += (pedido.NumeroPaletes - novaQtdePallet);
                pedido.NumeroPaletes = novaQtdePallet;

                if (!string.IsNullOrWhiteSpace(Request.Params("PalletsFracionado")))
                    novaQtdePalletFracionado = decimal.Parse(Request.Params("PalletsFracionado"));
                else
                    novaQtdePalletFracionado = 0;

                diferencaPallet += (novaQtdePalletFracionado - pedido.NumeroPaletesFracionado);
                pedido.NumeroPaletesFracionado = novaQtdePalletFracionado;

                pedido.PalletSaldoRestante += diferencaPallet;

                pedido.NumeroPedidoEmbarcador = Request.Params("NumeroPedidoEmbarcador");
                pedido.Observacao = Request.Params("Observacao");
                pedido.ObservacaoCTe = (operadorLogistica?.TelaPedidosResumido ?? false) ? Request.GetNullableStringParam("ObservacaoAbaPedido") : Request.GetNullableStringParam("ObservacaoCTe");
                pedido.Temperatura = Request.Params("Temperatura");
                pedido.SenhaAgendamento = Request.Params("SenhaAgendamento");
                pedido.SenhaAgendamentoCliente = Request.Params("SenhaAgendamentoCliente");
                pedido.PedidoIntegradoEmbarcador = false;
                pedido.Cotacao = cotacao;
                pedido.NumeroPaletesPagos = Request.GetDecimalParam("NumeroPaletesPagos");
                pedido.NumeroSemiPaletes = Request.GetDecimalParam("NumeroSemiPaletes");
                pedido.NumeroSemiPaletesPagos = Request.GetDecimalParam("NumeroSemiPaletesPagos");
                pedido.NumeroCombis = Request.GetDecimalParam("NumeroCombis");
                pedido.NumeroCombisPagas = Request.GetDecimalParam("NumeroCombisPagas");
                pedido.PedidoRestricaoData = Request.GetBoolParam("PedidoRestricaoData");
                pedido.CustoFrete = Request.GetStringParam("CustoFrete");

                int.TryParse(Request.Params("CanalEntrega"), out int codigoCanalEntrega);
                pedido.CanalEntrega = codigoCanalEntrega > 0 ? repCanalEntrega.BuscarPorCodigo(codigoCanalEntrega) : null;

                int codigoCanalVenda = Request.GetIntParam("CanalVenda");
                pedido.CanalVenda = codigoCanalVenda > 0 ? repositorioCanalVenda.BuscarPorCodigo(codigoCanalVenda) : null;

                int codigoContratoFreteCliente = Request.GetIntParam("NumeroContratoFreteCliente");
                pedido.ContratoFreteCliente = codigoContratoFreteCliente > 0 ? repContratoFreteCliente.BuscarPorCodigo(codigoContratoFreteCliente, false) : null;

                int codigoCentroDeCustoViagem = Request.GetIntParam("CentroDeCustoViagem");
                pedido.CentroDeCustoViagem = codigoCentroDeCustoViagem > 0 ? repCentroCustoViagem.BuscarPorCodigo(codigoCentroDeCustoViagem) : null;

                int codigoBalsa = Request.GetIntParam("Balsa");
                pedido.Balsa = codigoBalsa > 0 ? repNavio.BuscarPorCodigo(codigoBalsa) : null;

                int deposito = 0;
                int.TryParse(Request.Params("Deposito"), out deposito);
                if (deposito > 0)
                {
                    pedido.Deposito = repDeposito.BuscarPorCodigo(deposito);
                }
                else
                    pedido.Deposito = null;

                if (pedido.DisponibilizarPedidoParaColeta)
                    pedido.RecebedorColeta = repCliente.BuscarPorCPFCNPJ(Request.GetDoubleParam("RecebedorColeta"));

                Servicos.Embarcador.Pedido.Pedido servicoPedido = new Servicos.Embarcador.Pedido.Pedido(unitOfWork);
                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedido = repCargaPedido.BuscarPorPedidoComCargaAtiva(pedido.Codigo);
                if (cargasPedido.Count > 0)
                {
                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargasPedido)
                    {
                        if (!serCarga.VerificarSeCargaEstaNaLogistica(cargaPedido.Carga, TipoServicoMultisoftware) || mudouRemetente || mudouDestinatario)
                        {
                            int numeroNotas = 0;
                            if (cargaPedido.TipoContratacaoCarga == TipoContratacaoCarga.Normal)
                                numeroNotas = repPedidoXMLNotaFiscal.ContarXMLPorCargaPedido(cargaPedido.Codigo);
                            else
                                numeroNotas = repPedidoCTeParaSubContratacao.ContarCTesPorCargaPedido(cargaPedido.Codigo, true);

                            if (numeroNotas > 0)
                                throw new ControllerException(string.Format(Localization.Resources.Pedidos.Pedido.NaoEPossivelAtualizarEstePedidoPoisOMesmo, cargaPedido.Carga.DescricaoSituacaoCarga));
                        }
                        if (cargaPedido.ModalPropostaMultimodal == ModalPropostaMultimodal.PortoPorto)
                            new Servicos.Embarcador.Integracao.Intercab.IntegracaoIntercab(unitOfWork).ValidarPermissaoCancelarCarga(cargaPedido.Carga);

                        List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidoProdutos = repCargaPedidoProduto.BuscarPorCargaPedido(cargaPedido.Codigo);
                        foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto cargaPedidoProduto in cargaPedidoProdutos)
                            repCargaPedidoProduto.Deletar(cargaPedidoProduto);

                        if (cargaPedido.Carga.Empresa != null && pedido.Empresa != null)
                            repCanhoto.SetarTransportadorCanhotos(cargaPedido.Carga.Codigo, pedido.Empresa.Codigo);

                        cargaPedido.Carga.DataAtualizacaoCarga = DateTime.Now; //#37394
                        repCarga.Atualizar(cargaPedido.Carga);
                    }
                }

                pedido.GerarAutomaticamenteCargaDoPedido = bool.Parse(Request.Params("GerarAutomaticamenteCargaDoPedido"));
                pedido.PedidoSubContratado = bool.Parse(Request.Params("PedidoSubContratado"));

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.RequisitanteColeta requisitante;
                Enum.TryParse(Request.Params("Requisitante"), out requisitante);

                pedido.Requisitante = requisitante;

                bool viagemJaOcorreu = false;
                bool.TryParse(Request.Params("ViagemJaOcorreu"), out viagemJaOcorreu);
                if (viagemJaOcorreu)
                    pedido.SituacaoPedido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Finalizado;
                else
                    pedido.SituacaoPedido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Aberto;

                int tipoCarga = 0;
                int.TryParse(Request.Params("TipoCarga"), out tipoCarga);
                if (tipoCarga > 0)
                {
                    pedido.TipoDeCarga = repTipoDeCarga.BuscarPorCodigo(tipoCarga);
                }
                else
                    pedido.TipoDeCarga = null;

                if (ConfiguracaoEmbarcador.UtilizarMultiplosModelosVeicularesPedido)
                {
                    if (pedido.ModelosVeiculares != null)
                        pedido.ModelosVeiculares.Clear();
                    else
                        pedido.ModelosVeiculares = new List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>();
                    foreach (var modeloVeicularCarga in listaModelosVeiculares)
                    {
                        if (modeloVeicularCarga > 0)
                        {
                            pedido.ModelosVeiculares.Add(repModeloVeicularCarga.BuscarPorCodigo(modeloVeicularCarga));
                            pedido.ModeloVeicularCarga = repModeloVeicularCarga.BuscarPorCodigo(modeloVeicularCarga);
                        }
                    }
                }
                else
                {
                    int modeloVeicularCarga = 0;
                    int.TryParse(Request.Params("ModeloVeicularCarga"), out modeloVeicularCarga);
                    int codigoModeloAnterior = pedido.ModeloVeicularCarga?.Codigo ?? 0;

                    if (modeloVeicularCarga > 0)
                    {
                        if (codigoModeloAnterior > 0 &&
                            codigoModeloAnterior != modeloVeicularCarga &&
                            TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS &&
                            !this.Usuario.UsuarioAdministrador &&
                            !permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Pedido_PermitirAlterarModeloVeicularNoPedido))
                        {
                            return new JsonpResult(false, true, "Você não possui permissão para alterar o modelo veicular.");
                        }
                        else
                        {
                            pedido.ModeloVeicularCarga = repModeloVeicularCarga.BuscarPorCodigo(modeloVeicularCarga);
                        }
                    }
                    else
                        pedido.ModeloVeicularCarga = null;
                }

                int tipoColeta = 0;
                int.TryParse(Request.Params("TipoColeta"), out tipoColeta);
                if (tipoColeta > 0)
                {
                    pedido.TipoColeta = new Dominio.Entidades.TipoColeta() { Codigo = tipoColeta };
                }
                else
                    pedido.TipoColeta = null;

                int tipoOperacao = 0;
                int.TryParse(Request.Params("TipoOperacao"), out tipoOperacao);
                if (tipoOperacao > 0)
                {
                    pedido.TipoOperacao = repTipoOperacao.BuscarPorCodigo(tipoOperacao);

                    if ((pedido.TipoOperacao?.UsarConfiguracaoEmissao ?? false) && !string.IsNullOrWhiteSpace(pedido.TipoOperacao.ObservacaoCTe) && (!pedido.ObservacaoCTe?.ToLower().Contains(pedido.TipoOperacao.ObservacaoCTe.ToLower()) ?? true))
                        pedido.ObservacaoCTe += string.Concat(" ", pedido.TipoOperacao.ObservacaoCTe);
                }
                else
                    pedido.TipoOperacao = null;

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    if (pedido.TipoOperacao != null && !pedido.TipoOperacao.GeraCargaAutomaticamente)
                    {
                        pedido.GerarAutomaticamenteCargaDoPedido = false;

                        if (cargasPedido.Count == 0)
                        {
                            pedido.PedidoTotalmenteCarregado = false;
                            //TODO: PPC - Adicionado log temporário para identificar problema de retorno de saldo de pedido.
                            Servicos.Log.TratarErro(string.Format(Localization.Resources.Pedidos.Pedido.PedidoLiberouSaldoPedido, pedido.NumeroPedidoEmbarcador, pedido.PesoSaldoRestante, pedido.PesoTotal, pedido.PedidoTotalmenteCarregado), "SaldoPedido");
                        }
                    }
                }

                if (!(pedido.TipoOperacao?.ConfiguracaoAgendamentoColetaEntrega?.UtilizarDataSaidaGuaritaComoTerminoCarregamento ?? false))
                    pedido.DataTerminoCarregamento = Request.GetNullableDateTimeParam("DataTerminoCarregamento");

                if (pedido.TipoOperacao?.UtilizarDeslocamentoPedido ?? false)
                {
                    double cpfCnpjClienteDeslocamento = Request.GetDoubleParam("ClienteDeslocamento");

                    if (cpfCnpjClienteDeslocamento > 0d)
                        pedido.ClienteDeslocamento = repCliente.BuscarPorCPFCNPJ(cpfCnpjClienteDeslocamento);

                    if (pedido.ClienteDeslocamento == null)
                        throw new ControllerException(Localization.Resources.Pedidos.Pedido.ClienteDeslocamentoEObrigatorioParaTipoDeOperacaoInformado);
                }
                else
                    pedido.ClienteDeslocamento = null;

                //Embarcador deve sempre olhar o campo na tela do Pedido para definir se gera ou não bool.Parse(Request.Params("GerarAutomaticamenteCargaDoPedido"))
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS || (configuracaoCarga?.UtilizarConfiguracaoTipoOperacaoGeracaoCargaPorPedido ?? false))
                    if (!pedido.GerarAutomaticamenteCargaDoPedido && pedido.TipoOperacao != null && pedido.TipoOperacao.GeraCargaAutomaticamente)
                        pedido.GerarAutomaticamenteCargaDoPedido = true;

                Dominio.Enumeradores.TipoPagamento tipoPagamento;
                Enum.TryParse(Request.Params("TipoPagamento"), out tipoPagamento);

                PreencherDadosGeraisCargaPedido(pedido, unitOfWork);

                pedido.TipoPagamento = tipoPagamento;

                double.TryParse(Request.Params("Tomador"), out double tomador);

                if (tomador > 0d)
                    pedido.Tomador = repCliente.BuscarPorCPFCNPJ(tomador);
                else
                    pedido.Tomador = null;

                pedido.UsarTipoTomadorPedido = Request.GetBoolParam("UsarTipoTomadorPedido");
                if (pedido.UsarTipoTomadorPedido)
                {
                    pedido.TipoTomador = Request.GetEnumParam<Dominio.Enumeradores.TipoTomador>("TipoTomador");
                    if (pedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros && pedido.Tomador == null)
                    {
                        unitOfWork.Rollback();
                        throw new ControllerException(Localization.Resources.Pedidos.Pedido.QuandoTipoTomadorForOutrosObrigatorioInformarTomador);
                    }
                }
                else
                {
                    if (pedido.TipoPagamento == Dominio.Enumeradores.TipoPagamento.Outros)
                        pedido.TipoTomador = Dominio.Enumeradores.TipoTomador.Outros;
                    else if (pedido.TipoPagamento == Dominio.Enumeradores.TipoPagamento.A_Pagar)
                        pedido.TipoTomador = Dominio.Enumeradores.TipoTomador.Destinatario;
                    else
                        pedido.TipoTomador = Dominio.Enumeradores.TipoTomador.Remetente;
                }

                double.TryParse(Request.Params("ResponsavelRedespacho"), out double responsavelRedespacho);

                if (responsavelRedespacho > 0d)
                    pedido.ResponsavelRedespacho = repCliente.BuscarPorCPFCNPJ(responsavelRedespacho);
                else
                    pedido.ResponsavelRedespacho = null;

                pedido.FuncionarioVendedor = repUsuario.BuscarPorCodigo(Request.GetIntParam("FuncionarioVendedor"));
                pedido.FuncionarioSupervisor = repUsuario.BuscarPorCodigo(Request.GetIntParam("FuncionarioSupervisor"));
                pedido.FuncionarioGerente = repUsuario.BuscarPorCodigo(Request.GetIntParam("FuncionarioGerente"));
                pedido.ValorFreteInformativo = Request.GetDecimalParam("ValorFreteInformativo");
                pedido.Adicional1 = Request.GetStringParam("ProcImportacao");
                pedido.Safra = Request.GetStringParam("Safra");
                pedido.PercentualAdiantamentoTerceiro = Request.GetDecimalParam("PercentualAdiantamentoTerceiro");
                pedido.PercentualMinimoAdiantamentoTerceiro = Request.GetDecimalParam("PercentualMinimoAdiantamentoTerceiro");
                pedido.PercentualMaximoAdiantamentoTerceiro = Request.GetDecimalParam("PercentualMaximoAdiantamentoTerceiro");
                pedido.PesoLiquidoTotal = Request.GetDecimalParam("PesoLiquidoTotal");
                pedido.NumeroRastreioCorreios = Request.GetStringParam("NumeroRastreioCorreios");
                pedido.PesoTotalPaletes = Request.GetDecimalParam("PesoTotalPaletes");
                pedido.PossuiCargaPerigosa = Request.GetBoolParam("ContemCargaPerigosaDescricao");
                pedido.PedidoLiberadoPortalRetira = Request.GetBoolParam("PedidoLiberadoPortalRetira");
                pedido.ContemCargaRefrigerada = Request.GetBoolParam("ContemCargaRefrigeradaDescricao");

                pedido.DataAlocacaoPedido = Request.GetNullableDateTimeParam("DataAlocacaoPedido");
                pedido.GrossSales = Request.GetDecimalParam("ValorGross");
                pedido.PossuiIsca = Request.GetBoolParam("PossuiIsca");
                pedido.Substituicao = Request.GetNullableBoolParam("Substituicao");

                expedidor = 0;
                double.TryParse(Request.Params("Expedidor"), out expedidor);
                if (expedidor > 0)
                {
                    pedido.Expedidor = repCliente.BuscarPorCPFCNPJ(expedidor);
                    if (pedido.Expedidor != null)
                        servicoPedido.PreecherEnderecoPedido(ref pedidoEnderecoOrigem, pedido.Expedidor);
                }

                double localPaletizacao = 0;
                double.TryParse(Request.Params("LocalPaletizacao"), out localPaletizacao);
                if (localPaletizacao > 0)
                    pedido.LocalPaletizacao = repCliente.BuscarPorCPFCNPJ(localPaletizacao);
                else
                    pedido.LocalPaletizacao = null;


                int mercadoLivreRota = 0;
                int.TryParse(Request.Params("MercadoLivreRota"), out mercadoLivreRota);
                pedido.Rota = mercadoLivreRota;
                pedido.Facility = Request.Params("MercadoLivreFacility");

                bool.TryParse(Request.Params("NecessarioReentrega"), out bool necessarioReentrega);
                bool.TryParse(Request.Params("Rastreado"), out bool rastreado);
                bool.TryParse(Request.Params("GerenciamentoRisco"), out bool gerenciamentoRisco);
                bool.TryParse(Request.Params("Escolta"), out bool escoltaArmada);
                bool.TryParse(Request.Params("Seguro"), out bool seguro);
                bool.TryParse(Request.Params("Ajudante"), out bool ajudante);
                bool.TryParse(Request.Params("DespachoTransitoAduaneiro"), out bool despachoTransitoAduaneiro);

                decimal.TryParse(Request.Params("ValorTotalCarga"), out decimal valorTotalCarga);

                int.TryParse(Request.Params("QtdAjudantes"), out int qtdAjudantes);

                pedido.NecessarioReentrega = necessarioReentrega;
                pedido.Rastreado = rastreado;
                pedido.GerenciamentoRisco = gerenciamentoRisco;
                pedido.EscoltaArmada = Request.GetBoolParam("Escolta");
                pedido.QtdEscolta = Request.GetIntParam("QtdEscolta");
                pedido.DespachoTransitoAduaneiro = despachoTransitoAduaneiro;
                pedido.NumeroDTA = Request.GetStringParam("NumeroDTA");
                pedido.Seguro = seguro;
                pedido.Ajudante = ajudante;
                pedido.ValorTotalCarga = valorTotalCarga;
                pedido.QtdAjudantes = qtdAjudantes;

                string numeroContainer = Request.Params("NumeroContainer");
                string numeroBL = Request.Params("NumeroBL");
                string numeroNavio = Request.Params("NumeroNavio");
                string enderecoEntregaImportacao = Request.Params("EnderecoEntregaImportacao");
                string bairroEntregaImportacao = Request.Params("BairroEntregaImportacao");
                string cepEntregaImportacao = Request.Params("CEPEntregaImportacao");
                string armadorImportacao = Request.Params("ArmadorImportacao");

                DateTime dataVencimentoArmazenamentoImportacao;
                DateTime.TryParse(Request.Params("DataVencimentoArmazenamentoImportacao"), out dataVencimentoArmazenamentoImportacao);

                int codigoPorto = 0;
                int codigoTipoTerminalImportacao = 0;
                int codigoLocalidadeEntregaImportacao = 0;

                int.TryParse(Request.Params("Porto"), out codigoPorto);
                int.TryParse(Request.Params("TipoTerminalImportacao"), out codigoTipoTerminalImportacao);
                int.TryParse(Request.Params("LocalidadeEntregaImportacao"), out codigoLocalidadeEntregaImportacao);
                int qtdEntregas;

                int.TryParse(Request.Params("QtdEntregas"), out qtdEntregas);
                decimal.TryParse(Request.Params("CubagemTotalTMS"), out decimal cubagemTotalTMS);
                decimal.TryParse(Request.Params("PesoTotalCarga"), out decimal pesoTotalCarga);
                decimal.TryParse(Request.Params("CubagemTotal"), out decimal cubagemTotal);
                decimal.TryParse(Request.Params("ValorTotalNotasFiscais"), out decimal valorTotalNotasFiscais);

                pedido.QtdEntregas = qtdEntregas;

                if (pedido.QtdEntregas == 0)
                    pedido.QtdEntregas = 1;

                pedido.PesoTotal = pesoTotalCarga;

                if (cargasPedido.Count == 0)
                    pedido.PesoSaldoRestante = pesoTotalCarga;

                if (cubagemTotalTMS > 0m)
                    pedido.CubagemTotal = cubagemTotalTMS;
                else if (cubagemTotal > 0m)
                    pedido.CubagemTotal = cubagemTotal;
                else
                    pedido.CubagemTotal = 0m;

                if (valorTotalNotasFiscais > 0m)
                    pedido.ValorTotalNotasFiscais = valorTotalNotasFiscais;
                pedido.ValorPedagioRota = Request.GetDecimalParam("ValorPedagioRota");
                pedido.NumeroContainer = numeroContainer;
                pedido.NumeroControle = Request.GetStringParam("NumeroControle");
                pedido.LacreContainerDois = Request.GetStringParam("LacreContainerDois");
                pedido.LacreContainerTres = Request.GetStringParam("LacreContainerTres");
                pedido.LacreContainerUm = Request.GetStringParam("LacreContainerUm");
                pedido.TaraContainer = Request.GetStringParam("TaraContainer");
                pedido.QtVolumes = Request.GetIntParam("QtVolumes");
                pedido.SaldoVolumesRestante = Request.GetIntParam("QtVolumes");
                pedido.DataOrder = Request.GetNullableDateTimeParam("DataOrder");
                pedido.DataChip = Request.GetNullableDateTimeParam("DataChip");
                pedido.DataCancel = Request.GetNullableDateTimeParam("DataCancel");
                pedido.NumeroBL = numeroBL;
                pedido.NumeroNavio = numeroNavio;
                pedido.EnderecoEntregaImportacao = enderecoEntregaImportacao;
                pedido.BairroEntregaImportacao = bairroEntregaImportacao;
                pedido.CEPEntregaImportacao = cepEntregaImportacao;
                pedido.ArmadorImportacao = armadorImportacao;
                pedido.DataPrevisaoTerminoCarregamento = Request.GetNullableDateTimeParam("DataPrevisaoTerminoCarregamento");
                pedido.ContainerTipoReservaFluxoContainer = repContainerTipo.BuscarPorCodigo(Request.GetIntParam("ContainerTipoReservaFluxoContainer"));

                if (dataVencimentoArmazenamentoImportacao > DateTime.MinValue)
                    pedido.DataVencimentoArmazenamentoImportacao = dataVencimentoArmazenamentoImportacao;
                else
                    pedido.DataVencimentoArmazenamentoImportacao = null;

                if (codigoPorto > 0)
                    pedido.Porto = repPorto.BuscarPorCodigo(codigoPorto);
                else
                    pedido.Porto = null;

                if (codigoTipoTerminalImportacao > 0)
                    pedido.TipoTerminalImportacao = repTipoTerminalImportacao.BuscarPorCodigo(codigoTipoTerminalImportacao);
                else
                    pedido.TipoTerminalImportacao = null;

                if (codigoLocalidadeEntregaImportacao > 0)
                    pedido.LocalidadeEntregaImportacao = repLocalidade.BuscarPorCodigo(codigoLocalidadeEntregaImportacao);
                else
                    pedido.LocalidadeEntregaImportacao = null;

                pedido.UltimaAtualizacao = DateTime.Now;

                if (pedido.Destinatario != null && pedido.Destinatario.ClienteDescargas.Count > 0 && pedido.Destinatario.ClienteDescargas.FirstOrDefault().RestricoesDescarga.Count > 0)
                    pedido.RestricoesDescarga = pedido.Destinatario.ClienteDescargas.FirstOrDefault().RestricoesDescarga.ToList();
                else
                    pedido.RestricoesDescarga.Clear();

                Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoExiste = null;
                if (!string.IsNullOrWhiteSpace(pedido.NumeroPedidoEmbarcador) && pedido.Filial != null)
                    pedidoExiste = repPedido.BuscarPorNumeroEmbarcador(pedido.NumeroPedidoEmbarcador, pedido.Filial.Codigo, false);

                SalvarListaMotorista(ref pedido, unitOfWork, permissoesPersonalizadas);
                SalvarListaCliente(pedido, unitOfWork);
                string retornoParcial = SalvarNotasParciaisPedido(pedido, unitOfWork);
                if (!string.IsNullOrWhiteSpace(retornoParcial))
                    throw new ControllerException(retornoParcial);

                SalvarCtesParciaisPedido(pedido, unitOfWork);
                SalvarDIPedido(pedido, unitOfWork);
                SalvarContatos(pedido, unitOfWork);
                SalvarComponentesFrete(pedido, unitOfWork);
                SalvarAcrescimoDesconto(pedido, unitOfWork);
                SalvarPedidoAdicionais(pedido, unitOfWork);
                SalvarPedidoEcommerce(pedido, unitOfWork);
                await SalvarListaFronteirasAsync(pedido, unitOfWork);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    string msgRetorno = SalvarDadosMultimodal(ref pedido, ref pedidoEnderecoDestino, unitOfWork);
                    if (!string.IsNullOrWhiteSpace(msgRetorno))
                        throw new ControllerException(msgRetorno);

                    if (pedido.PedidoViagemNavio != null && pedido.Container != null)
                    {
                        if (pedido.TipoOperacao == null || pedido.TipoOperacao.TipoPropostaMultimodal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.VAS)
                        {
                            Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoExistente = repPedido.ValidarViagemContainer(pedido.PedidoViagemNavio.Codigo, pedido.Container.Codigo, pedido.Codigo);
                            if (pedidoExistente != null && !configuracaoPedido.NaoValidarMesmaViagemEMesmoContainer)
                                throw new ControllerException(string.Format(Localization.Resources.Pedidos.Pedido.JaExisteOutroPedidoContendoMesmoContainerParaMesmoNavio, pedidoExistente.Numero.ToString("D"), pedidoExistente.NumeroBooking));
                        }
                    }
                }

                decimal totalCubagem = 0, totalPeso = 0, totalPalet = 0;

                SalvarListaProdutos(ref pedido, unitOfWork, ref totalCubagem, ref totalPeso, ref totalPalet, true);
                RemoverPendenciaDeProdutoSeNecessario(pedido, unitOfWork);

                if (ConfiguracaoEmbarcador.SolicitarValorFretePorTonelada && pedido.TipoOperacao != null && pedido.RotaFrete != null && !string.IsNullOrWhiteSpace(pedido.RotaFrete.CodigoIntegracaoValePedagio) && pedido.ValorPedagioRota == 0)
                {
                    Dominio.Entidades.Embarcador.Configuracoes.IntegracaoPagbem integracaoPagbem = servicoValePedagio.ObterIntegracaoPagbem(pedido.TipoOperacao, null, null, true, TipoServicoMultisoftware);
                    if (integracaoPagbem != null)
                    {
                        int.TryParse(pedido.RotaFrete.CodigoIntegracaoValePedagio, out int codigoRoteiro);
                        int qtdEixos = pedido.ModeloVeicularCarga?.NumeroEixos.Value ?? 0;
                        qtdEixos = integracaoPagbem.QuantidadeEixosPadraoValePedagio > 0 ? integracaoPagbem.QuantidadeEixosPadraoValePedagio : qtdEixos;
                        if (codigoRoteiro > 0 && integracaoPagbem != null)
                            pedido.ValorPedagioRota = serPagbem.RetornarValorPedagio(integracaoPagbem, codigoRoteiro, qtdEixos, unitOfWork, out string mensagemRetorno);
                    }
                }

                if (totalCubagem > 0m)
                    pedido.CubagemTotal = totalCubagem;

                if (totalPeso > 0m)
                {
                    pedido.PesoTotal = totalPeso;

                    if (cargasPedido.Count == 0)
                        pedido.PesoSaldoRestante = totalPeso;
                }

                if (totalPalet > 0m)
                {
                    diferencaPallet = (totalPalet - pedido.NumeroPaletesFracionado);
                    pedido.NumeroPaletesFracionado = totalPalet;
                    pedido.PalletSaldoRestante += diferencaPallet;
                }

                if (pedidoExiste == null || pedido.Codigo == pedidoExiste.Codigo)
                {
                    if (!Servicos.Embarcador.Carga.CargaPedido.ValidarNumeroPedidoEmbarcador(out string erro, pedido.NumeroPedidoEmbarcador, pedido.ObterTomador(), pedido.TipoOperacao, ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal))
                        throw new ControllerException(erro);

                    ValidarRegrasPedido(pedido, unitOfWork, permissoesPersonalizadas);

                    if (pedido.TipoOperacao != null && pedido.TipoOperacao.UtilizarFatorCubagem && pedido.TipoOperacao.FatorCubagem.HasValue)
                        pedido.PesoCubado = pedido.CubagemTotal * pedido.TipoOperacao.FatorCubagem.Value;

                    if (VerificarRegrasPedido(pedido, TipoServicoMultisoftware, unitOfWork))
                    {
                        pedido.SituacaoPedido = SituacaoPedido.AutorizacaoPendente;
                        pedido.EtapaPedido = EtapaPedido.AgAutorizacao;
                    }
                    else
                    {
                        pedido.SituacaoPedido = SituacaoPedido.Aberto;
                        pedido.EtapaPedido = EtapaPedido.Finalizada;
                    }

                    repPedido.Atualizar(pedido, Auditado);

                    if (!ConfiguracaoEmbarcador.UtilizarIntegracaoPedido)
                    {
                        pedido.PedidoIntegradoEmbarcador = true;

                        if (pedido.ColetaEmProdutorRural)
                        {
                            if (!servicoPreCarga.CriarPreCarga(out erro, pedido, true, TipoServicoMultisoftware, Usuario))
                                throw new ControllerException(erro);
                        }
                        else if (!pedido.Cotacao)
                        {
                            if (cargasPedido.Count == 0)
                            {
                                string vRetorno = "";
                                if (TipoServicoMultisoftware == TipoServicoMultisoftware.MultiEmbarcador)
                                    PreencherCodigoCargaEmbarcador(unitOfWork, pedido);

                                if (pedido.SituacaoPedido != SituacaoPedido.AutorizacaoPendente && pedido.SituacaoPedido != SituacaoPedido.AgAprovacao)
                                {
                                    if (TipoServicoMultisoftware == TipoServicoMultisoftware.MultiTMS)
                                        PreencherCodigoCargaEmbarcador(unitOfWork, pedido);

                                    vRetorno = Servicos.Embarcador.Pedido.Pedido.CriarCarga(pedido, unitOfWork, TipoServicoMultisoftware, Cliente, ConfiguracaoEmbarcador);
                                }

                                string retorno = vRetorno;

                                if (!string.IsNullOrWhiteSpace(retorno))
                                    throw new ControllerException(retorno);

                                repPedido.Atualizar(pedido);
                            }
                            else
                            {
                                Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                                servicoCarga.AtualizarCargaPorPedido(pedido, cargasPedido[0].Carga, recriarRota, TipoServicoMultisoftware, ClienteMultisoftware: Cliente, unitOfWork, ConfiguracaoEmbarcador, Auditado, true);

                                repPedido.Atualizar(pedido);
                            }
                        }
                    }

                    AtualizarPercursoEstados(pedido, unitOfWork);

                    if (pedido.Cotacao)
                        pedido.ValorFreteCotado = Servicos.Embarcador.Carga.Frete.CalcularFretePorPedido(pedido, unitOfWork, _conexao.StringConexao, TipoServicoMultisoftware, ConfiguracaoEmbarcador);

                    repPedido.Atualizar(pedido);

                    PreencherDadosRotaFreteClienteDeslocamento(unitOfWork, pedido);
                    AtualizarIntegracaoSILEMP(repCargaPedido, repCargaDadosTransporteIntegracao, pedido);

                    try
                    {
                        string erros = string.Empty;
                        serHistorico.InserirHistoricoVinculo(unitOfWork, ref erros, LocalVinculo.Pedido, pedido.VeiculoTracao, pedido.Veiculos, pedido.Motoristas, DateTime.Now, null, pedido, null);
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro(ex);
                    }

                    unitOfWork.CommitChanges();

                    NotificarJanelaCarregamento(pedido, unitOfWork);

                    return new JsonpResult(new { pedido.Codigo });
                }
                else
                    throw new ControllerException(Localization.Resources.Pedidos.Pedido.JaExisteUmpedidoCadastradoParaEsseCodigoDeEmbarcador);
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
                return new JsonpResult(false, Localization.Resources.Pedidos.Pedido.OcorreuUmaFalhaAoAtualizar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private void AtualizarIntegracaoSILEMP(Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido, Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaDadosTransporteIntegracao, Pedido pedido)
        {
            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPrimeiraPorPedido(pedido.Codigo);

            if (cargaPedido != null && (cargaPedido.Carga.TipoOperacao?.ConfiguracaoEMP?.AtivarIntegracaoComSIL ?? false) && repCargaDadosTransporteIntegracao.ExistePorCargaETipoIntegracao(cargaPedido.Carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.EMP))
            {
                Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao = repCargaDadosTransporteIntegracao.BuscarPorCargaETipoIntegracao(cargaPedido.Carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.EMP);

                if (cargaDadosTransporteIntegracao != null)
                {
                    cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                    repCargaDadosTransporteIntegracao.Atualizar(cargaDadosTransporteIntegracao);
                }
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            UnitOfWork unitOfWork = new UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                bool duplicar = Request.GetBoolParam("Duplicar");
                bool duplicarParaDevolucaoTotal = Request.GetBoolParam("DuplicarParaDevolucaoTotal");
                bool duplicarParaDevolucaoParcial = Request.GetBoolParam("DuplicarParaDevolucaoParcial");

                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repositorioConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork);
                Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacaoAnexo repositorioTipoOperacaoAnexo = new Repositorio.Embarcador.Pedidos.TipoOperacaoAnexo(unitOfWork);

                ConfiguracaoPedido configuracaoPedido = await repositorioConfiguracaoPedido.BuscarConfiguracaoPadraoAsync();
                Pedido pedido = await repPedido.BuscarPorCodigoAsync(codigo);

                if (pedido == null)
                    return new JsonpResult(false, true, Localization.Resources.Pedidos.Pedido.NaoFoipossivelEncontrarRegistro);

                if (duplicar && TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && pedido.TipoOperacao != null && !pedido.TipoOperacao.Ativo)
                    return new JsonpResult(false, true, Localization.Resources.Pedidos.Pedido.NaoEPossivelDuplicarDevidoTipoOperecaoEstarInativo);

                Dominio.Entidades.Veiculo veiculo = pedido.VeiculoTracao ?? pedido.Veiculos.FirstOrDefault();

                bool possuiNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork).PedidoPossuiNotaFiscal(pedido.Codigo);
                List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoAnexo> anexosTipoOperacao = pedido.TipoOperacao != null ? repositorioTipoOperacaoAnexo.BuscarPorCodigoTipoOperacao(pedido.TipoOperacao.Codigo) : new List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoAnexo>();

                Dominio.Entidades.Usuario veiculoMotorista = null;
                if (veiculo != null)
                    veiculoMotorista = repVeiculoMotorista.BuscarMotoristaPrincipal(veiculo.Codigo);

                string LinkRastreio = "";
                if (!string.IsNullOrWhiteSpace(pedido.CodigoRastreamento))
                {
                    AdminMultisoftware.Repositorio.UnitOfWork UnitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(_conexao.AdminStringConexao);
                    string urlBase = Servicos.Embarcador.GestaoEntregas.NotificaoEntrega.ObterURLBase(Cliente.Codigo, TipoServicoMultisoftware, UnitOfWorkAdmin, _conexao.AdminStringConexao, unitOfWork);
                    LinkRastreio = Servicos.Embarcador.GestaoEntregas.NotificaoEntrega.ObterURLRastreamentoPedido(pedido.CodigoRastreamento, urlBase);
                    UnitOfWorkAdmin.Dispose();
                }

                dynamic destinatario = ObterDadosDestinatario(pedido, duplicarParaDevolucaoParcial);
                dynamic remetente = ObterDadosRemetente(pedido, duplicarParaDevolucaoParcial);
                dynamic recebedor = ObterDadosRecebedor(pedido, duplicarParaDevolucaoTotal);
                dynamic expedidor = ObterDadosExpedidor(pedido, duplicarParaDevolucaoTotal);

                dynamic dynPedido = new
                {
                    Codigo = duplicar ? 0 : pedido.Codigo,
                    pedido.ColetaEmProdutorRural,
                    CamposSecundariosObrigatoriosPedido = pedido.TipoOperacao?.CamposSecundariosObrigatoriosPedido ?? false,
                    Rota = new { Codigo = pedido.RotaFrete?.Codigo ?? 0, Descricao = pedido.RotaFrete?.Descricao ?? string.Empty },
                    pedido.CodigoCargaEmbarcador,
                    pedido.ObservacaoEmissaRemetente,
                    pedido.ObservacaoEmissaoDestinatario,
                    DataCarregamentoPedido = pedido.DataCarregamentoPedido?.ToString("dd/MM/yyyy HH:mm") ?? "",
                    DataColeta = pedido.DataCarregamentoPedido?.ToString("dd/MM/yyyy HH:mm") ?? "",
                    pedido.TipoPessoa,
                    DataFinalColeta = pedido.DataFinalColeta?.ToString("dd/MM/yyyy HH:mm") ?? "",
                    DataInicialColeta = pedido.DataInicialColeta?.ToString("dd/MM/yyyy HH:mm") ?? "",
                    Terceiro = pedido.Terceiro != null ? new { Codigo = pedido.Terceiro.CPF_CNPJ, Descricao = pedido.Terceiro.Descricao } : null,
                    Origem = pedido.Origem != null ? new { pedido.Origem.Codigo, Descricao = pedido.Origem.DescricaoCidadeEstado } : null,
                    Destino = pedido.Destino != null ? new { Codigo = pedido.Destino.Codigo, Descricao = pedido.Destino.DescricaoCidadeEstado } : null,
                    Destinatario = destinatario,
                    Remetente = remetente,
                    Expedidor = expedidor,
                    Recebedor = recebedor,
                    Filial = pedido.Filial != null ? new { pedido.Filial.Codigo, pedido.Filial.Descricao } : null,
                    Empresa = pedido.Empresa != null ? new { pedido.Empresa.Codigo, pedido.Empresa.Descricao } : null,
                    Serie = pedido.EmpresaSerie != null ? new { Codigo = pedido.EmpresaSerie != null ? pedido.EmpresaSerie.Codigo : 0, Descricao = pedido.EmpresaSerie != null ? pedido.EmpresaSerie.Numero.ToString() : "" } : null,
                    TotalPallets = (pedido.NumeroPaletes + pedido.NumeroPaletesFracionado).ToString("n3"),
                    PalletsFracionado = pedido.NumeroPaletesFracionado > 0 ? pedido.NumeroPaletesFracionado.ToString("n3") : "",
                    pedido.NumeroPedidoEmbarcador,
                    Observacao = pedido.Observacao != null ? pedido.Observacao : "",
                    ObservacaoEntrega = pedido.ObservacaoEntrega != null ? pedido.ObservacaoEntrega : "",
                    pedido.ProdutoPredominante,
                    PesoTotalCarga = pedido.PesoTotal > 0 ? pedido.PesoTotal.ToString("n2") : "",
                    PesoLiquidoTotal = pedido.PesoLiquidoTotal > 0 ? pedido.PesoLiquidoTotal.ToString("n2") : "",
                    PesoTotalPaletes = pedido.PesoTotalPaletes > 0 ? pedido.PesoTotalPaletes.ToString("n2") : "",
                    LocalPaletizacao = new { Codigo = pedido.LocalPaletizacao != null ? pedido.LocalPaletizacao.Codigo : 0, Descricao = pedido.LocalPaletizacao != null ? pedido.LocalPaletizacao.Descricao : "" },
                    ContemCargaRefrigeradaDescricao = pedido.ContemCargaRefrigerada.ObterDescricao(),
                    PossuiCargaPerigosaDescricao = pedido.PossuiCargaPerigosa.ObterDescricao(),
                    NumeroRastreioCorreios = pedido.NumeroRastreioCorreios != null ? pedido.NumeroRastreioCorreios : "",
                    CubagemTotal = pedido.CubagemTotal > 0 ? pedido.CubagemTotal.ToString("n6") : "",
                    PrevisaoEntrega = pedido.PrevisaoEntrega?.ToString("dd/MM/yyyy HH:mm") ?? "",
                    DataCarregamento = pedido.DataCarregamentoCarga?.ToDateTimeString() ?? string.Empty,
                    GrupoPessoa = pedido.GrupoPessoas != null ? new { Codigo = pedido.GrupoPessoas.Codigo, Descricao = pedido.GrupoPessoas.Descricao } : null,
                    TipoOperacao = new
                    {
                        Codigo = pedido.TipoOperacao?.Codigo ?? 0,
                        Descricao = pedido.TipoOperacao?.Descricao ?? "",
                        UtilizarDeslocamentoPedido = pedido.TipoOperacao?.UtilizarDeslocamentoPedido ?? false,
                        DestinatarioObrigatorio = !pedido.TipoOperacao?.PermiteGerarPedidoSemDestinatario ?? true,
                        ApresentarSaldoProduto = Servicos.Embarcador.Carga.MontagemCarga.SaldoPedidoProduto.ApresentarSaldoProdutoGridPedido(pedido, pedido.TipoOperacao)
                    },
                    OperacaoDeImportacaoExportacao = pedido.TipoOperacao != null ? pedido.TipoOperacao.OperacaoDeImportacaoExportacao : false,
                    NaoExigeVeiculoParaEmissao = pedido.TipoOperacao?.NaoExigeVeiculoParaEmissao ?? false,
                    Veiculo = veiculo != null ? new { Codigo = veiculo.Codigo, Descricao = BuscarPlacas(pedido) } : null,
                    ListaReboques = ConfiguracaoEmbarcador.PermitirSelecionarReboquePedido ? pedido.Veiculos.Select(o => new { o.Codigo, Placa = ConfiguracaoEmbarcador.ConcatenarFrotaPlaca ? o.PlacaConcatenada : o.Placa, ModeloVeicular = o.ModeloVeicularCarga?.Descricao ?? string.Empty, NumeroFrota = o.NumeroFrota ?? string.Empty }).ToList() : null,
                    ProdutoEmbarcador = pedido.Produtos != null && pedido.Produtos.Count > 0 ? new { Codigo = pedido.Produtos.First().Produto.Codigo, Descricao = pedido.Produtos.First().Produto.Descricao } : null,
                    pedido.GerarUmCTEPorNFe,
                    SituacaoPedido = duplicar ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Aberto : pedido.SituacaoPedido,
                    DescricaoEmpresa = pedido.Empresa?.Descricao ?? "",
                    TipoColeta = pedido.TipoColeta != null ? new { Codigo = pedido.TipoColeta != null ? pedido.TipoColeta.Codigo : 0, Descricao = pedido.TipoColeta != null ? pedido.TipoColeta.Descricao : "" } : null,
                    TipoCarga = pedido.TipoDeCarga != null ? new { Codigo = pedido.TipoDeCarga != null ? pedido.TipoDeCarga.Codigo : 0, Descricao = pedido.TipoDeCarga != null ? pedido.TipoDeCarga.Descricao : "" } : null,
                    ModeloVeicularCarga = pedido.ModeloVeicularCarga != null ? new { Codigo = pedido.ModeloVeicularCarga.Codigo, Descricao = pedido.ModeloVeicularCarga.Descricao } : new { Codigo = 0, Descricao = "" },
                    ModelosVeicularesCarga = pedido.ModelosVeiculares.Select(o => new { o.Codigo, Descricao = o.Descricao }).ToList(),
                    pedido.TipoTomador,
                    pedido.UsarTipoTomadorPedido,
                    UsarOutroEnderecoOrigem = pedido.UsarOutroEnderecoOrigem,
                    OutroEnderecoOrigem = pedido.EnderecoOrigem?.ClienteOutroEndereco == null ? null : CriarObjetoDetalheOutroEndereco(pedido.EnderecoOrigem),
                    LocalidadeClienteOrigem = pedido.EnderecoOrigem != null ? new
                    {
                        Codigo = pedido.EnderecoOrigem.ClienteOutroEndereco?.Codigo ?? 0,
                        Descricao = pedido.EnderecoOrigem.ClienteOutroEndereco?.Localidade?.DescricaoCidadeEstado ?? "",
                    } : null,
                    BairroOrigem = pedido.EnderecoOrigem != null ? pedido.EnderecoOrigem.Bairro : "",
                    CEPOrigem = pedido.EnderecoOrigem != null ? pedido.EnderecoOrigem.CEP : "",
                    NumeroOrigem = pedido.EnderecoOrigem != null ? pedido.EnderecoOrigem.Numero : "",
                    ComplementoOrigem = pedido.EnderecoOrigem != null ? pedido.EnderecoOrigem.Complemento : "",
                    EnderecoOrigem = pedido.EnderecoOrigem != null ? pedido.EnderecoOrigem.Endereco : "",
                    Telefone1Origem = pedido.EnderecoOrigem != null ? pedido.EnderecoOrigem.Telefone : "",
                    RGIE1Origem = pedido.EnderecoDestino != null ? pedido.EnderecoDestino.IE_RG : "",
                    CidadePoloOrigem = pedido.Origem != null ? new { Codigo = pedido.Origem.LocalidadePolo != null ? pedido.Origem.LocalidadePolo.Codigo : 0, Descricao = pedido.Origem.LocalidadePolo != null ? pedido.Origem.LocalidadePolo.DescricaoCidadeEstado : "" } : null,
                    PaisOrigem = pedido.Origem != null ? pedido.Origem.Pais != null ? pedido.Origem.Pais.Nome : "" : "",
                    IBGEOrigem = pedido.Origem != null ? pedido.Origem.CodigoIBGE : 0,
                    UFOrigem = pedido.Origem != null ? pedido.Origem.Estado.Sigla : "",
                    UsarOutroEnderecoDestino = pedido.UsarOutroEnderecoDestino,
                    OutroEnderecoDestino = pedido.EnderecoDestino?.ClienteOutroEndereco == null ? null : CriarObjetoDetalheOutroEndereco(pedido.EnderecoDestino),
                    LocalidadeClienteDestino = pedido.EnderecoDestino != null ? new
                    {
                        Codigo = pedido.EnderecoDestino.ClienteOutroEndereco?.Codigo ?? 0,
                        Descricao = pedido.EnderecoDestino.ClienteOutroEndereco?.Localidade?.DescricaoCidadeEstado ?? "",
                    } : null,
                    BairroDestino = pedido.EnderecoDestino != null ? pedido.EnderecoDestino.Bairro : "",
                    CEPDestino = pedido.EnderecoDestino != null ? pedido.EnderecoDestino.CEP : "",
                    NumeroDestino = pedido.EnderecoDestino != null ? pedido.EnderecoDestino.Numero : "",
                    ComplementoDestino = pedido.EnderecoDestino != null ? pedido.EnderecoDestino.Complemento : "",
                    EnderecoDestino = pedido.EnderecoDestino != null ? pedido.EnderecoDestino.Endereco : "",
                    Telefone1Destino = pedido.EnderecoDestino != null ? pedido.EnderecoDestino.Telefone : "",
                    RGIE1Destino = pedido.EnderecoDestino != null ? pedido.EnderecoDestino.IE_RG : "",
                    CidadePoloDestino = pedido.Destino != null ? new { Codigo = pedido.Destino.LocalidadePolo != null ? pedido.Destino.LocalidadePolo.Codigo : 0, Descricao = pedido.Destino.LocalidadePolo != null ? pedido.Destino.LocalidadePolo.DescricaoCidadeEstado : "" } : null,
                    PaisDestino = pedido.Destino != null && pedido.Destino.Pais != null ? pedido.Destino.Pais.Nome : "",
                    IBGEDestino = pedido.Destino != null ? pedido.Destino.CodigoIBGE : 0,
                    UFDestino = pedido.Destino != null ? pedido.Destino.Estado.Sigla : "",
                    ClienteDeslocamento = new { Codigo = pedido.ClienteDeslocamento?.CPF_CNPJ ?? 0d, Descricao = pedido.ClienteDeslocamento?.Descricao ?? "" },
                    pedido.NumeroBL,
                    pedido.NumeroNavio,
                    Porto = pedido.Porto?.Descricao,
                    CodigoPorto = pedido.Porto?.Codigo,
                    TipoTerminalImportacao = pedido.TipoTerminalImportacao?.Descricao,
                    CodigoTipoTerminalImportacao = pedido.TipoTerminalImportacao?.Codigo,
                    pedido.EnderecoEntregaImportacao,
                    pedido.BairroEntregaImportacao,
                    pedido.CEPEntregaImportacao,
                    LocalidadeEntregaImportacao = pedido.LocalidadeEntregaImportacao?.Descricao,
                    CodigoLocalidadeEntregaImportacao = pedido.LocalidadeEntregaImportacao?.Codigo,
                    DataVencimentoArmazenamentoImportacao = pedido.DataVencimentoArmazenamentoImportacao.HasValue ? pedido.DataVencimentoArmazenamentoImportacao.Value.ToString("dd/MM/yyyy") : string.Empty,
                    pedido.ArmadorImportacao,
                    pedido.DescricaoSituacaoPedido,
                    pedido.DescricaoEtapaPedido,
                    NomesMotoristas = pedido.NomeMotoristas,
                    NumeroPedido = duplicar ? "" : TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador ? (pedido.Numero > 0 ? pedido.Numero.ToString() : string.Join(", ", pedido.CargasPedido.Select(o => o.CodigoCargaEmbarcador).Distinct())) : pedido.CodigoCargaEmbarcador,

                    Navio = pedido.Navio?.Descricao ?? "",
                    CodigoNavio = pedido.Navio?.Codigo ?? 0,

                    PortoDestino = pedido.PortoDestino?.Descricao ?? "",
                    CodigoPortoDestino = pedido.PortoDestino?.Codigo ?? 0,

                    TerminalOrigem = pedido.TerminalOrigem?.Descricao ?? "",
                    CodigoTerminalOrigem = pedido.TerminalOrigem?.Codigo ?? 0,

                    TerminalDestino = pedido.TerminalDestino?.Descricao ?? "",
                    CodigoTerminalDestino = pedido.TerminalDestino?.Codigo ?? 0,

                    DirecaoViagemMultimodal = pedido.DirecaoViagemMultimodal,

                    Container = pedido.Container?.Descricao ?? "",
                    CodigoContainer = pedido.Container?.Codigo ?? 0,
                    //Container = new { Codigo = pedido.Container?.Codigo ?? 0, Descricao = pedido.Container?.Descricao ?? string.Empty },

                    ContainerTipoReserva = pedido.ContainerTipoReserva?.Descricao ?? "",
                    CodigoContainerTipoReserva = pedido.ContainerTipoReserva?.Codigo ?? 0,

                    LacreContainerUmMultimodal = pedido.LacreContainerUm,
                    LacreContainerDoisMultimodal = pedido.LacreContainerDois,
                    LacreContainerTresMultimodal = pedido.LacreContainerTres,
                    TaraContainerMultimodal = pedido.TaraContainer,
                    TempoDeDoca = pedido?.TempoDeDoca ?? 0,
                    NumeroDoca = pedido?.NumeroDoca ?? "",

                    pedido.CanceladoAposVinculoCarga,

                    CodigoMotoristaVeiculo = veiculoMotorista?.Codigo ?? 0,
                    NomeMotoristaVeiculo = veiculoMotorista?.Nome ?? string.Empty,

                    CentroResultado = new { Codigo = pedido.CentroResultado?.Codigo ?? 0, Descricao = pedido.CentroResultado?.Descricao ?? string.Empty },
                    CentroCarregamento = new { Codigo = pedido.CentroCarregamento?.Codigo ?? 0, Descricao = pedido.CentroCarregamento?.Descricao ?? string.Empty },

                    PermiteCancelarAposVinculoCarga = possuiNotaFiscal && (configuracaoPedido?.PermitirMudarStatusPedidoParaCanceladoAposVinculoCarga ?? false) && !pedido.CanceladoAposVinculoCarga,


                    DadosProduto = new
                    {
                        ExibirValorUnitarioDoProduto = pedido.PedidosCarga?.FirstOrDefault()?.Carga?.TipoOperacao?.TipoOperacaoExibeValorUnitarioDoProduto ?? false,
                        PossuiIdDemandaProdutos = pedido.Produtos.Any(o => !string.IsNullOrWhiteSpace(o.IdDemanda))
                    },

                    LinkRastreio = LinkRastreio,

                    Ocorrencias = ObterDadosOcorrenciaColetaEntregaPedido(pedido, LinkRastreio, unitOfWork),

                    GridTransbordo = ObterDadosTransbordoPedido(pedido, duplicar),

                    ComponentesFrete = ObterDadosComponentesFretePedido(pedido, duplicar),

                    PassagemPercursoEstado = ObterDadosPassagensPedido(pedido, duplicar, unitOfWork),

                    ProdutosEmbarcador = ObterDadosProdutosPedido(pedido, duplicar, unitOfWork),

                    ONUsProdutosEmbarcador = ObterDadosONUProdutosPedido(pedido, duplicar, unitOfWork),

                    Anexos = ObterDadosAnexosPedido(pedido, duplicar),

                    AvaliacaoEntrega = ObterDadosAvaliacaoControleEntrega(pedido, unitOfWork, duplicar),

                    AcrescimoDesconto = ObterListaAcrescimoDesconto(pedido, unitOfWork, duplicar),

                    Contatos = ObterListaContatos(pedido, unitOfWork, duplicar),

                    Historico = configuracaoPedido.ExibirAuditoriaPedidos ? ObterDadosHistoricoPedido(pedido, unitOfWork) : null,

                    AnexosTipoOperacao = (
                        from anx in anexosTipoOperacao
                        select new
                        {
                            anx.Codigo,
                            anx.Descricao,
                            anx.NomeArquivo,
                        }
                    ).ToList(),
                    CentroDeCustoViagem = new { Codigo = pedido.CentroDeCustoViagem?.Codigo ?? 0, Descricao = pedido.CentroDeCustoViagem?.Descricao ?? string.Empty },
                    Balsa = new { Codigo = pedido.Balsa?.Codigo ?? 0, Descricao = pedido.Balsa?.Descricao ?? string.Empty }
                };

                return new JsonpResult(dynPedido);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Pedidos.Pedido.OcorreuUmaFalhaAoBuscarPorCodigo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigoParteComplementar1()
        {
            UnitOfWork unitOfWork = new UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");
                bool duplicar = Request.GetBoolParam("Duplicar");
                bool duplicarPedidoParaDevolucaoPacial = Request.GetBoolParam("DuplicarParaDevolucaoParcial");

                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                var pedido = repPedido.BuscarPorCodigo(codigo);

                if (pedido == null)
                    return new JsonpResult(false, true, Localization.Resources.Pedidos.Pedido.NaoFoipossivelEncontrarRegistro);

                if (duplicar && TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && pedido.TipoOperacao != null && !pedido.TipoOperacao.Ativo)
                    return new JsonpResult(false, true, Localization.Resources.Pedidos.Pedido.NaoEPossivelDuplicarDevidoTipoOperecaoEstarInativo);


                string LinkRastreio = "";
                if (!string.IsNullOrWhiteSpace(pedido.CodigoRastreamento))
                {
                    AdminMultisoftware.Repositorio.UnitOfWork UnitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(_conexao.AdminStringConexao);
                    string urlBase = Servicos.Embarcador.GestaoEntregas.NotificaoEntrega.ObterURLBase(Cliente.Codigo, TipoServicoMultisoftware, UnitOfWorkAdmin, _conexao.AdminStringConexao, unitOfWork);
                    LinkRastreio = Servicos.Embarcador.GestaoEntregas.NotificaoEntrega.ObterURLRastreamentoPedido(pedido.CodigoRastreamento, urlBase);
                    UnitOfWorkAdmin.Dispose();
                }

                var retorno = new
                {
                    GridDI = ObterDadosDIPedido(pedido, duplicar),
                    Resumo = ObterDadosResumoPedido(pedido, unitOfWork),
                    DadosAutorizacao = ObterDadosAutorizacaoPedido(pedido, unitOfWork),
                    GridDestinatarioBloqueado = ObterDadosDestinatariosBloqueadosPedido(pedido),
                    ListaMotoristas = ObterDadosMotoristasPedido(pedido),
                    ListaClientes = ObterDadosClientesPedido(pedido),
                    ListaFronteiras = await ObterListaFronteirasAsync(pedido, unitOfWork),
                    NotasParciais = ObterDadosNotasParciaisPedido(pedido),
                    NotasFiscais = ObterDadosNotasFiscaisPedido(pedido, duplicarPedidoParaDevolucaoPacial),
                    CtesParciais = ObterDadosCTesParciaisPedido(pedido),
                    Cotacoes = ObterListaCotacoesPedido(pedido, unitOfWork),
                    Stages = ObterStagesPedido(pedido),
                    DadosAdicionais = ObterDadosAdicionaisPedido(pedido, duplicar, LinkRastreio, unitOfWork),
                    DadosEcommerce = ObterDadosEcommercePedido(pedido, unitOfWork),
                    ContainerAdicional = ObterDadosContainer(pedido),

                };
                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao tentar Obter os dados complementares do pedido");

            }
        }

        public async Task<IActionResult> CancelarPorCodigo(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(unitOfWork, cancellationToken);

                Servicos.Embarcador.Pedido.OcorrenciaPedido servOcorrenciaPedido = new Servicos.Embarcador.Pedido.OcorrenciaPedido(unitOfWork);

                // Parametros
                int codigo = Request.GetIntParam("Codigo");

                string motivoCancelamento = Request.Params("Motivo");

                // Busca informacoes
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = await repPedido.BuscarPorCodigoAsync(codigo, true);

                // Valida
                if (pedido == null)
                    return new JsonpResult(false, true, Localization.Resources.Pedidos.Pedido.NaoFoipossivelEncontrarRegistro);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    if (await repCargaPedido.ExistePorPedidoAsync(pedido.Codigo))
                        return new JsonpResult(false, true, Localization.Resources.Pedidos.Pedido.PedidoJaEstaVinculadoAUmaCargaNaoSendoPossivel);

                    if (pedido.SituacaoPedido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Aberto &&
                        pedido.SituacaoPedido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.AgAprovacao &&
                        pedido.SituacaoPedido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.AutorizacaoPendente &&
                        pedido.SituacaoPedido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Rejeitado)
                        return new JsonpResult(false, true, Localization.Resources.Pedidos.Pedido.SituacaoPedidoNaoPermiteCancelamento);
                }
                else if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                {
                    if (await repCargaPedido.ExistePorPedidoAsync(pedido.Codigo))
                        return new JsonpResult(false, true, Localization.Resources.Pedidos.Pedido.PedidoJaEstaVinculadoAUmaCargaNaoSendoPossivel);

                    if (await repCarregamentoPedido.ExisteCarregamentoAtivoPorPedidoAsync(pedido.Codigo))
                        return new JsonpResult(false, true, Localization.Resources.Pedidos.Pedido.PedidoJaEstaVinculadoAUmCarregamentoNaoSendoPossivel);
                }

                if (ConfiguracaoEmbarcador.NaoPermitirExclusaoPedido == true && (string.IsNullOrWhiteSpace(motivoCancelamento) && motivoCancelamento.Trim().Length <= 20))
                    return new JsonpResult(false, true, Localization.Resources.Pedidos.Pedido.MotivoDoCancelamentoDeveTerMaisDeVinteCaracteres);

                await unitOfWork.StartAsync(cancellationToken);

                pedido.SituacaoPedido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Cancelado;
                pedido.MotivoCancelamento = motivoCancelamento;

                await repPedido.AtualizarAsync(pedido, Auditado);

                servOcorrenciaPedido.ProcessarOcorrenciaPedido(EventoColetaEntrega.PedidoCancelado, pedido, ConfiguracaoEmbarcador, this.Cliente);

                if (pedido.SituacaoRoteirizadorIntegracao == SituacaoRoteirizadorIntegracao.Integrado)
                    new Servicos.Embarcador.Integracao.IntegracaoPedidoRoterizador(unitOfWork, Auditado).AdicionarEIntegrarPedidos([pedido.Codigo], TipoRoteirizadorIntegracao.CancelarPedido);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Pedidos.Pedido.OcorreuUmaFalhaAoRemoverDados);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.MotivoCancelamentoPedido repMotivoCancelamentoPedido = new Repositorio.Embarcador.Pedidos.MotivoCancelamentoPedido(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(unitOfWork);
                Servicos.Embarcador.Pedido.OcorrenciaPedido servOcorrenciaPedido = new Servicos.Embarcador.Pedido.OcorrenciaPedido(unitOfWork);

                // Parametros
                int codigo = Request.GetIntParam("Codigo");

                // Busca informacoes
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repPedido.BuscarPorCodigo(codigo, true);

                // Valida
                if (pedido == null)
                    return new JsonpResult(false, true, Localization.Resources.Pedidos.Pedido.NaoFoipossivelEncontrarRegistro);

                if (repCargaPedido.ExistePorPedido(pedido.Codigo))
                    return new JsonpResult(false, true, Localization.Resources.Pedidos.Pedido.PedidoJaEstaVinculadoAUmaCargaNaoSendoPossivelRealizarExclusao);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentos = repCarregamentoPedido.BuscarPorPedido(pedido.Codigo);

                    if (carregamentos.Any(o => o.Carregamento.SituacaoCarregamento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarregamento.EmMontagem
                    || o.Carregamento.SituacaoCarregamento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarregamento.AguardandoAprovacaoSolicitacao
                    ))
                        return new JsonpResult(false, true, string.Format(Localization.Resources.Pedidos.Pedido.PedidoJaEstaVinculadoAUmCarregamentoNaoSendoPossivelRealizarExclusao, carregamentos.FirstOrDefault().Carregamento.NumeroCarregamento));

                    if (carregamentos.Count > 0)
                        return new JsonpResult(new { PossuiDependencias = true });

                    unitOfWork.Start();
                    repPedido.Deletar(pedido, Auditado);
                    unitOfWork.CommitChanges();
                }
                else if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    int motivo = Request.GetIntParam("Motivo");

                    unitOfWork.Start();
                    pedido.SituacaoPedido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Cancelado;
                    pedido.ControleNumeracao = pedido.Codigo;
                    pedido.MotivoCancelamentoPedido = motivo > 0 ? repMotivoCancelamentoPedido.BuscarPorCodigo(motivo) : null;
                    pedido.UsuarioCancelamento = Auditado.Usuario;

                    CancelarIntegracoesDoPedido(pedido, unitOfWork);
                    servOcorrenciaPedido.ProcessarOcorrenciaPedido(EventoColetaEntrega.PedidoCancelado, pedido, ConfiguracaoEmbarcador, this.Cliente);

                    repPedido.Atualizar(pedido, Auditado);
                    unitOfWork.CommitChanges();
                }

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

                // Comentado para cancelar o pedido sempre que ocorrer uma exceção (ex: timeout)
                //if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                //{
                return new JsonpResult(new { PossuiDependencias = true });
                //}

                //return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> GerarRelatorio()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Pedidos/PlanejamentoPedidoTMS");

                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unitOfWork);
                Servicos.Embarcador.Pedido.ImpressaoPedido serImpressaoPedido = new Servicos.Embarcador.Pedido.ImpressaoPedido(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);

                bool impressaoCarga = false, impressaoCarregamento = false, ordemColeta = false;
                bool.TryParse(Request.Params("Carga"), out impressaoCarga);
                bool.TryParse(Request.Params("Carregamento"), out impressaoCarregamento);
                bool.TryParse(Request.Params("OrdemColeta"), out ordemColeta);
                bool.TryParse(Request.Params("PlanejamentoPedido"), out bool planejamentoPedido);

                if (planejamentoPedido && !permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteImprimirOrdemDeColeta))
                    return new JsonpResult(false, true, Localization.Resources.Pedidos.Pedido.VoceNaoPossuiPermissaoParaCompletarEssaAcao);

                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = null;
                Dominio.Entidades.Embarcador.Cargas.Carga carga = null;
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento = null;

                if (impressaoCarregamento)
                    carregamento = repCarregamento.BuscarPorCodigo(codigo);
                else if (!impressaoCarga)
                    pedido = repPedido.BuscarPorCodigo(codigo);
                else
                    carga = repCarga.BuscarPorCodigo(codigo);

                if (pedido == null && carga == null && carregamento == null)
                    return new JsonpResult(false, false, Localization.Resources.Pedidos.Pedido.FavorSelecioneUmPedidoCargaCarregamento);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                {
                    byte[] arquivo = GerarRelatorioEmbarcador(pedido, unitOfWork, out string msg, false, 0);

                    return Arquivo(arquivo, "application/pdf", "Ordem de Coleta GTS - " + string.Join(", ", pedido.CargasPedido.Select(o => o.CodigoCargaEmbarcador).Distinct()) + ".pdf");
                }
                else
                {
                    if (!serImpressaoPedido.GerarRelatorioTMS(planejamentoPedido, pedido, impressaoCarregamento, out string msg, impressaoCarga, carga, codigo, ordemColeta, false, _conexao.StringConexao, TipoServicoMultisoftware, Cliente.NomeFantasia, this.Usuario, false, out string guidRelatorio, out string fileName))
                        return new JsonpResult(false, false, msg);
                }

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, Localization.Resources.Pedidos.Pedido.OcorreuUmaFalhaAoGerarRelatorio);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> GerarRelatorioPlanoViagem()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);
                bool impressaoCarga = false, impressaoCarregamento = false, ordemColeta = false;
                bool.TryParse(Request.Params("Carga"), out impressaoCarga);
                bool.TryParse(Request.Params("Carregamento"), out impressaoCarregamento);
                bool.TryParse(Request.Params("OrdemColeta"), out ordemColeta);

                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = null;
                Dominio.Entidades.Embarcador.Cargas.Carga carga = null;
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento = null;

                if (impressaoCarregamento)
                    carregamento = repCarregamento.BuscarPorCodigo(codigo);
                else if (!impressaoCarga)
                    pedido = repPedido.BuscarPorCodigo(codigo);
                else
                    carga = repCarga.BuscarPorCodigo(codigo);

                bool planoViagem = true;

                if (pedido == null && carga == null && carregamento == null)
                    return new JsonpResult(false, false, Localization.Resources.Pedidos.Pedido.FavorSelecioneUmPedidoCargaCarregamento);

                byte[] arquivo = GerarRelatorioEmbarcador(pedido, unitOfWork, out string msg, planoViagem, carga?.Codigo ?? 0);
                return Arquivo(arquivo, "application/pdf", string.Format(Localization.Resources.Pedidos.Pedido.PlanoDeViajemPDF, (carga?.CodigoCargaEmbarcador ?? "")));

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, Localization.Resources.Pedidos.Pedido.OcorreuUmaFalhaAoGerarRelatorio);
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ValidaPedidoDuplicado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");
                int origem = Request.GetIntParam("Origem");
                string numeroPedidoEmbarcador = Request.GetStringParam("NumeroPedidoEmbarcador");
                int codigoVeiculo = Request.GetIntParam("Veiculo");
                int codigoTipoOperacao = Request.GetIntParam("TipoOperacao");

                DateTime? dataColeta = Request.GetNullableDateTimeParam("DataColeta");
                DateTime? dataInicialColeta = Request.GetNullableDateTimeParam("DataInicialColeta");
                if (!dataColeta.HasValue)
                    dataColeta = dataInicialColeta;

                Dominio.Entidades.Localidade destino = null;
                double recebedor = Request.GetDoubleParam("Recebedor");
                double destinatario = Request.GetDoubleParam("Destinatario");

                if (recebedor > 0)
                    destino = repCliente.BuscarPorCPFCNPJ(recebedor)?.Localidade ?? null;
                else
                    destino = repCliente.BuscarPorCPFCNPJ(destinatario)?.Localidade ?? null;

                if (ConfiguracaoEmbarcador.SolicitarConfirmacaoPedidoDuplicado && repPedido.ContemPedidoMesmaOrigemDestinoVeiculoData(origem, destino?.Codigo ?? 0, codigoVeiculo, dataColeta, codigo))
                    return new JsonpResult(new { PedidoDuplicado = true });

                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = null;

                if (codigoTipoOperacao > 0)
                {
                    tipoOperacao = repTipoOperacao.BuscarPorCodigo(codigoTipoOperacao);

                    if (repPedido.BuscarNumeroPedidoEmbarcadorDuplicado(numeroPedidoEmbarcador, codigoTipoOperacao) && (tipoOperacao?.NotificarCasoNumeroPedidoForExistente ?? false))
                        return new JsonpResult(new { NumeroPedidoEmbarcadorDuplicado = true });
                }

                return new JsonpResult(new { PedidoDuplicado = false, NumeroPedidoEmbarcadorDuplicado = false });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Pedidos.Pedido.OcorreuUmaFalhaAoValidarPedidoDuplicado);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ValidaPedidoSemMotoristaVeiculo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);

                int codigoVeiculo = Request.GetIntParam("CodigoVeiculo");
                bool contemMotorista = Request.GetBoolParam("ContemMotoristas");

                bool contemTracao = true;
                if (codigoVeiculo > 0)
                {
                    Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(codigoVeiculo);
                    if (veiculo.TipoVeiculo == "1" && veiculo.VeiculosTracao.Count == 0)
                        contemTracao = false;
                }

                if (ConfiguracaoEmbarcador.SolicitarConfirmacaoPedidoSemMotoristaVeiculo)
                {
                    if (!contemMotorista && codigoVeiculo == 0)
                        return new JsonpResult(new { SolicitaConfirmacao = true, MensagemConfirmacao = Localization.Resources.Pedidos.Pedido.PedidoEstaSendoSalvoSemVeiculoEMotorista });
                    else if (codigoVeiculo == 0)
                        return new JsonpResult(new { SolicitaConfirmacao = true, MensagemConfirmacao = Localization.Resources.Pedidos.Pedido.PedidoEstaSendoSalvoSemVeiculo });
                    else if (!contemMotorista && !contemTracao)
                        return new JsonpResult(new { SolicitaConfirmacao = true, MensagemConfirmacao = Localization.Resources.Pedidos.Pedido.PedidoEstaSendoSalvoSemMotoristaERoboque });
                    else if (!contemMotorista)
                        return new JsonpResult(new { SolicitaConfirmacao = true, MensagemConfirmacao = Localization.Resources.Pedidos.Pedido.PedidoEstaSendoSalvoSemMotorista });
                    else if (!contemTracao)
                        return new JsonpResult(new { SolicitaConfirmacao = true, MensagemConfirmacao = Localization.Resources.Pedidos.Pedido.ReboqueDoVeiculoSelecionadoPeidoNaoPossuiTracao });
                }

                return new JsonpResult(new { SolicitaConfirmacao = false, MensagemConfirmacao = "" });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Pedidos.Pedido.OcorreuUmaFalhaAoValidarPedidoSemMotoristaVeiculo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterDetalhesTipoOperacaoPedido()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoTipoOperacao = Request.GetIntParam("TipoOperacao");

                Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repTipoOperacao.BuscarPorCodigo(codigoTipoOperacao);

                if (tipoOperacao == null)
                    return new JsonpResult(false, true, Localization.Resources.Pedidos.Pedido.TipoDeOperacaoNaoFoiEncontrado);

                Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultado = repCentroResultado.BuscarPorTipoOperacao(tipoOperacao);

                var retorno = new
                {
                    CentroResultado = centroResultado != null ? new { centroResultado.Codigo, centroResultado.Descricao } : null,
                    ProdutosEmbaracador = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? ObterDadosProdutosPedidoPorTipoOperacao(tipoOperacao) : null,
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Pedidos.Pedido.OcorreuUmaFalhaAoObterDetalhesDoTipoOperacao);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterCondicaoPedidoPorTomador()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                double tomador = Request.GetDoubleParam("Tomador");
                int codigoTipoOperacao = Request.GetIntParam("TipoOperacao");

                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);

                Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(tomador);
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repTipoOperacao.BuscarPorCodigo(codigoTipoOperacao);

                if (cliente == null)
                    return new JsonpResult(false, true, Localization.Resources.Pedidos.Pedido.NaoFoiPossivelEncontrarTomador);

                Dominio.Entidades.Embarcador.Pedidos.PedidoTipoPagamento pedidoTipoPagamento = cliente.PedidoTipoPagamento;
                Dominio.Entidades.Embarcador.Financeiro.TipoPagamentoRecebimento formaPagamento = null;
                int? diasDePrazoFatura = 0;

                if (tipoOperacao != null)
                {
                    if (formaPagamento == null)
                        formaPagamento = tipoOperacao.ConfiguracaoTipoOperacaoFatura?.FormaPagamento;
                    if (diasDePrazoFatura == null || diasDePrazoFatura == 0)
                        diasDePrazoFatura = tipoOperacao.ConfiguracaoTipoOperacaoFatura?.DiasDePrazoFatura;
                }

                if (formaPagamento == null)
                    formaPagamento = cliente.FormaPagamento;

                if (diasDePrazoFatura == null || diasDePrazoFatura == 0)
                    diasDePrazoFatura = cliente.DiasDePrazoFatura;

                if (cliente.GrupoPessoas != null)
                {
                    if (pedidoTipoPagamento == null)
                        pedidoTipoPagamento = cliente.GrupoPessoas.PedidoTipoPagamento;
                    if (formaPagamento == null)
                        formaPagamento = cliente.GrupoPessoas.FormaPagamento;
                    if (diasDePrazoFatura == null || diasDePrazoFatura == 0)
                        diasDePrazoFatura = cliente.GrupoPessoas.DiasDePrazoFatura;
                }

                var retorno = new
                {
                    Codigo = pedidoTipoPagamento?.Codigo ?? 0,
                    Descricao = pedidoTipoPagamento?.Descricao ?? string.Empty,

                    FormaPagamento = formaPagamento?.Descricao ?? string.Empty,
                    DiasDePrazoFatura = diasDePrazoFatura ?? 0
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Pedidos.Pedido.OcorreuUmaFalhaAoObterCondicaoDoPedido);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterConfiguracoesGeraisPedido()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repositorioConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = repositorioConfiguracaoPedido.BuscarConfiguracaoPadrao();

                return new JsonpResult(new
                {
                    ImportarOcorrenciasDePedidosPorPlanilhas = configuracaoPedido?.ImportarOcorrenciasDePedidosPorPlanilhas ?? false,
                    AtualizarCamposPedidoPorPlanilha = configuracaoPedido?.AtualizarCamposPedidoPorPlanilha ?? false,
                    HabilitarBIDTransportePedido = configuracaoPedido?.HabilitarBIDTransportePedido ?? false,
                    UsarFatorConversaoProdutoEmPedidoPaletizado = configuracaoPedido?.UsarFatorConversaoProdutoEmPedidoPaletizado ?? false,
                    PermitirSelecionarCentroDeCarregamentoNoPedido = configuracaoPedido?.PermitirSelecionarCentroDeCarregamentoNoPedido ?? false,
                });
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao obter as Configurações Gerais dos Pedidos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterQuantidadeDeStages()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Pedidos.Stage repositorioStage = new Repositorio.Embarcador.Pedidos.Stage(unitOfWork);
                Repositorio.Embarcador.Pedidos.StageAgrupamento repositorioAgrupamento = new Repositorio.Embarcador.Pedidos.StageAgrupamento(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga);
                int quantidadeStages = repositorioStage.BuscarQuantidadeStageNumaCarga(codigoCarga);

                if (carga == null)
                    return new JsonpResult(false, "Carga não encontrada");

                return new JsonpResult(new
                {
                    Quantidade = quantidadeStages,
                    TipoPreChekin = carga?.TipoOperacao?.TipoConsolidacao != EnumTipoConsolidacao.NaoConsolida ? true : false,
                    //TipoMilkrun = carga?.TipoOperacao?.TipoConsolidacao == EnumTipoConsolidacao.NaoConsolida && carga?.TipoOperacao?.ExigeNotaFiscalParaCalcularFrete == false ? true : false,
                    CargaFilha = repositorioAgrupamento.EstaCargaFoiGeradoPorUmAgrupamento(carga.Codigo),
                    InformarLacreNosDadosTransporte = carga?.TipoOperacao?.ConfiguracaoCarga?.InformarLacreNosDadosTransporte ?? false,
                });

            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao obter as Configurações Gerais dos Pedidos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterListaStagesAgrupadas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("Codigo");
                bool cargaGerada = Request.GetBoolParam("PorCargaGerada", false);

                Repositorio.Embarcador.Pedidos.StageAgrupamento repistorioStageAgrupamento = new Repositorio.Embarcador.Pedidos.StageAgrupamento(unitOfWork);
                Repositorio.Embarcador.Pedidos.Stage repositorioStage = new Repositorio.Embarcador.Pedidos.Stage(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoStage repositorioPedidoStage = new Repositorio.Embarcador.Pedidos.PedidoStage(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                List<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento> stageAgrupamentos = new List<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento>();

                if (cargaGerada)
                    stageAgrupamentos = repistorioStageAgrupamento.BuscarPorCargaGerada(codigoCarga);
                else
                    stageAgrupamentos = repistorioStageAgrupamento.BuscarPorCargaDt(codigoCarga);

                Dominio.Entidades.Embarcador.Cargas.Carga existeCarga = repositorioCarga.BuscarPorCodigo(codigoCarga);

                if (stageAgrupamentos == null || stageAgrupamentos.Count == 0 || existeCarga == null)
                    return new JsonpResult(false, "Carga não possui agrupamento");

                dynamic stageAgrupadasFormatadas = new
                {
                    StagesColetas = new List<dynamic>(),
                    StagesEntregas = new List<dynamic>(),
                    StagesTransferencia = new List<dynamic>()
                };

                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = existeCarga.TipoOperacao;

                foreach (var obj in stageAgrupamentos)
                {
                    List<Dominio.Entidades.Embarcador.Pedidos.Stage> stages = repositorioStage.BuscarporAgrupamento(obj.Codigo);

                    if (stages.Count() <= 0)
                        continue;
                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoStage> pedisoStage = repositorioPedidoStage.BuscarPorStage(stages.FirstOrDefault().Codigo);
                    string recebedor = pedisoStage.Any(x => x.Stage.Recebedor != null) ? pedisoStage.Where(x => x.Stage.Recebedor != null).Select(x => x.Stage.Recebedor.Descricao).FirstOrDefault() : pedisoStage.Select(x => x.Pedido.Destinatario).FirstOrDefault().Descricao;
                    string expedidor = pedisoStage.Any(x => x.Stage.Expedidor != null) ? pedisoStage.Where(x => x.Stage.Expedidor != null).Select(x => x.Stage.Expedidor.Descricao).FirstOrDefault() : pedisoStage.Select(x => x.Pedido.Remetente).FirstOrDefault().Descricao;

                    List<Vazio> tipoPercusoStages = stages.Select(s => s.TipoPercurso).ToList();

                    dynamic agrupamento = new
                    {
                        obj.Codigo,
                        DT_Enable = true,
                        NumeroStage = string.Join(", ", stages.Select(s => s.NumeroStage).ToList()),
                        TipoPercuso = tipoPercusoStages,
                        Fornecedor = expedidor,
                        Endereco = string.IsNullOrWhiteSpace(recebedor) ? "" : recebedor,
                        Motorista = obj?.Motorista?.Nome ?? string.Empty,
                        CodigoMotorista = obj?.Motorista?.Codigo ?? 0,
                        CodigoReboque = obj?.Reboque?.Codigo ?? 0,
                        Reboque = obj?.Reboque?.Placa ?? string.Empty,
                        CodigoSegundoReboque = obj?.SegundoReboque?.Codigo ?? 0,
                        SegundoReboque = obj?.SegundoReboque?.Placa ?? string.Empty,
                        CodigoPlaca = obj?.Veiculo?.Codigo ?? 0,
                        Placa = obj?.Veiculo?.Placa ?? string.Empty,
                        Frete = obj.ValorFreteTotal > 0 ? obj.ValorFreteTotal.ToString("N2") : obj.MensagemRetornoDadosFrete,
                        FalhaCalculoFrete = string.IsNullOrEmpty(obj.MensagemRetornoDadosFrete) ? false : true,
                        CargaDT = obj.CargaDT.Codigo
                    };


                    if (tipoOperacao.TipoConsolidacao == EnumTipoConsolidacao.AutorizacaoEmissao)
                    {
                        stageAgrupadasFormatadas.StagesColetas.Add(agrupamento);
                        continue;
                    }

                    if (tipoPercusoStages.Any(s => s == Vazio.PercursoPreliminar))
                    {
                        stageAgrupadasFormatadas.StagesColetas.Add(agrupamento);
                        continue;
                    }

                    if (tipoPercusoStages.Any(s => s == Vazio.PercursoPrincipal))
                    {
                        stageAgrupadasFormatadas.StagesTransferencia.Add(agrupamento);
                        continue;
                    }

                    if (tipoPercusoStages.Any(s => s == Vazio.PercursoSubSeQuente))
                    {
                        stageAgrupadasFormatadas.StagesEntregas.Add(agrupamento);
                        continue;
                    }

                    stageAgrupadasFormatadas.StagesColetas.Add(agrupamento);
                }

                return new JsonpResult(stageAgrupadasFormatadas);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao obter as Configurações Gerais dos Pedidos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> SalvarPlacaEMotoristaStageAgrupada()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();
                int codigoAgrupamento = Request.GetIntParam("CodigoAgrupamento");
                int codigoPlaca = Request.GetIntParam("CodigoPlaca");
                int codigoReboque = Request.GetIntParam("CodigoReboque");
                int codigoSegundoReboque = Request.GetIntParam("CodigoSegundoReboque");
                int codigoMotorista = Request.GetIntParam("CodigoMotorista");
                string cpfMotorista = Request.GetStringParam("CpfMotorista");
                string placa = Request.GetStringParam("Placa");
                string placaReboque = Request.GetStringParam("Reboque");
                string placaSegundoReboque = Request.GetStringParam("SegundoReboque");

                Repositorio.Embarcador.Pedidos.StageAgrupamento repostitorioStageAgrupamento = new Repositorio.Embarcador.Pedidos.StageAgrupamento(unitOfWork);
                Repositorio.Embarcador.Veiculos.VeiculoMotorista repositorioVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);
                Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Usuario repositorioMotorista = new Repositorio.Usuario(unitOfWork);
                Repositorio.Embarcador.Pedidos.Stage repositorioStage = new Repositorio.Embarcador.Pedidos.Stage(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento stageAgrupamento = repostitorioStageAgrupamento.BuscarPorCodigo(codigoAgrupamento, false);

                if (stageAgrupamento == null)
                    return new JsonpResult(false, "Agrupamento Não Encontrado");

                Dominio.Entidades.Veiculo veiculo = null;
                Dominio.Entidades.Veiculo reboque = null;
                Dominio.Entidades.Veiculo segundoReboque = null;

                if (codigoPlaca > 0 || !string.IsNullOrEmpty(placa))
                {
                    veiculo = repositorioVeiculo.BuscarPorCodigo(codigoPlaca);

                    if (veiculo == null)
                        veiculo = repositorioVeiculo.BuscarPlaca(placa);

                    if (stageAgrupamento.Veiculo == null && veiculo == null)
                        return new JsonpResult(false, "Veiculo informado não encontrado");

                    stageAgrupamento.Veiculo = veiculo;
                    if (stageAgrupamento.CargaDT.Veiculo == null)
                        stageAgrupamento.CargaDT.Veiculo = veiculo;
                }
                else
                    stageAgrupamento.Veiculo = null;

                if (codigoReboque > 0)
                    reboque = repositorioVeiculo.BuscarPorCodigo(codigoReboque);
                else if (!string.IsNullOrEmpty(placaReboque))
                    reboque = repositorioVeiculo.BuscarPlaca(placaReboque);

                if (codigoSegundoReboque > 0)
                    segundoReboque = repositorioVeiculo.BuscarPorCodigo(codigoSegundoReboque);
                else if (!string.IsNullOrEmpty(placaSegundoReboque))
                    segundoReboque = repositorioVeiculo.BuscarPlaca(placaSegundoReboque);

                if (reboque != null && reboque.TipoVeiculo == "1")
                {
                    stageAgrupamento.Reboque = reboque;

                    if (!stageAgrupamento.CargaDT.VeiculosVinculados.Any(v => v.Codigo == reboque.Codigo))
                    {
                        stageAgrupamento.CargaDT.VeiculosVinculados.Clear();
                        stageAgrupamento.CargaDT.VeiculosVinculados = new List<Dominio.Entidades.Veiculo>() { reboque };
                    }
                }
                else if (reboque != null && reboque.TipoVeiculo == "0")
                    return new JsonpResult(true, false, $"O tipo de veiculo para a {reboque.Placa} não é de reboque.");
                else
                    stageAgrupamento.Reboque = null;

                if (segundoReboque != null && segundoReboque.TipoVeiculo == "1")
                {
                    stageAgrupamento.SegundoReboque = segundoReboque;

                    if (!stageAgrupamento.CargaDT.VeiculosVinculados.Any(v => v.Codigo == segundoReboque.Codigo))
                        stageAgrupamento.CargaDT.VeiculosVinculados.Add(segundoReboque);
                }
                else
                    stageAgrupamento.SegundoReboque = null;

                if (codigoMotorista > 0 || !string.IsNullOrEmpty(cpfMotorista))
                {
                    Dominio.Entidades.Usuario motorista = repositorioMotorista.BuscarPorCodigo(codigoMotorista);

                    if (motorista == null)
                        motorista = repositorioMotorista.BuscarPorCPF(cpfMotorista);

                    if (stageAgrupamento.Motorista == null && motorista == null)
                        return new JsonpResult(false, "Motorista informado não encontrado");

                    if (motorista != null)
                    {
                        stageAgrupamento.Motorista = motorista;
                        if (!stageAgrupamento.CargaDT.Motoristas.Any(o => o.Codigo == motorista.Codigo))
                            new Servicos.Embarcador.Carga.CargaMotorista(unitOfWork).AtualizarMotorista(stageAgrupamento.CargaDT, motorista);
                    }
                }
                else
                    stageAgrupamento.Motorista = null;

                if (veiculo != null && stageAgrupamento.Motorista == null)
                {
                    Dominio.Entidades.Usuario motorista = repositorioVeiculoMotorista.BuscarMotoristaPrincipal(veiculo.Codigo);
                    if (motorista != null)
                        stageAgrupamento.Motorista = motorista;
                }

                repostitorioStageAgrupamento.Atualizar(stageAgrupamento);
                repositorioCarga.Atualizar(stageAgrupamento.CargaDT);

                if (stageAgrupamento.PlacasConfirmadas)
                    new Servicos.Embarcador.Integracao.Unilever.IntegracaoUnilever(unitOfWork).GerarIntegracoes(stageAgrupamento.CargaDT, new List<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento>() { stageAgrupamento });


                unitOfWork.CommitChanges();

                return new JsonpResult(new
                {
                    stageAgrupamento.Codigo,
                    DT_Enable = true,
                    NumeroStage = string.Join(",", repositorioStage.ObterNumerosStagesPorAgrupamento(stageAgrupamento.Codigo)),
                    Fornecedor = stageAgrupamento?.Expedidor != null ? stageAgrupamento.Expedidor.Descricao : string.Empty,
                    Endereco = stageAgrupamento?.Expedidor != null ? stageAgrupamento.Expedidor.Endereco : string.Empty,
                    Placa = stageAgrupamento?.Veiculo?.Placa ?? string.Empty,
                    Motorista = stageAgrupamento?.Motorista?.Nome ?? string.Empty,
                    CodigoMotorista = stageAgrupamento?.Motorista?.Codigo ?? 0,
                    CodigoPlaca = stageAgrupamento?.Veiculo?.Codigo ?? 0,
                    CodigoReboque = stageAgrupamento?.Reboque?.Codigo ?? 0,
                    Reboque = stageAgrupamento?.Reboque?.Placa ?? string.Empty,
                    CodigoSegundoReboque = stageAgrupamento?.SegundoReboque?.Codigo ?? 0,
                    SegundoReboque = stageAgrupamento?.SegundoReboque?.Placa ?? string.Empty
                });
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao obter as Configurações Gerais dos Pedidos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ValidarTipoTomadorComTipoOperacao()
        {
            bool configuracaoValida = false;
            bool usarConfiguracaoEmissao = false;
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes tipoEmissaoCTeParticipantes = TipoEmissaoCTeParticipantes.Normal;
            Enum.TryParse(Request.Params("TipoTomador"), out Dominio.Enumeradores.TipoTomador tipoTomador);
            if (tipoTomador != Dominio.Enumeradores.TipoTomador.Expedidor &&
                tipoTomador != Dominio.Enumeradores.TipoTomador.Recebedor)
                return new JsonpResult(false, true, Localization.Resources.Pedidos.Pedido.TipoTomadorCompativel);

            int.TryParse(Request.Params("GrupoPessoa"), out int grupoPessoa);
            double.TryParse(Request.Params("Remetente"), out double remetente);

            //valida tipo de operacao > pessoa > grupo de pessoa, quando compativel nao considera as outras
            if (int.TryParse(Request.Params("TipoOperacao"), out int tipoOperacaoParam) && tipoOperacaoParam > 0)
            {
                Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repTipoOperacao.BuscarPorCodigo(tipoOperacaoParam);
                usarConfiguracaoEmissao = tipoOperacao.UsarConfiguracaoEmissao;
                tipoEmissaoCTeParticipantes = tipoOperacao.TipoEmissaoCTeParticipantes;
                configuracaoValida = ValidarRegraTomador(tipoTomador, usarConfiguracaoEmissao, tipoEmissaoCTeParticipantes);
            }
            if (!configuracaoValida && remetente > 0)
            {
                Repositorio.Cliente repRemetente = new Repositorio.Cliente(unitOfWork);
                Dominio.Entidades.Cliente remetentePedido = repRemetente.BuscarPorCPFCNPJSemFetch(remetente);
                usarConfiguracaoEmissao = remetentePedido.NaoUsarConfiguracaoEmissaoGrupo; // se nao usa a config do grupo entao ira usar do remetente/pessoa logo essa flag deve estar marcada para usar a configuracao da pessoa
                tipoEmissaoCTeParticipantes = remetentePedido.TipoEmissaoCTeParticipantes;
                configuracaoValida = ValidarRegraTomador(tipoTomador, usarConfiguracaoEmissao, tipoEmissaoCTeParticipantes);

                //se a config do remente nao era compativel mas ele tem um grupo de pessoas e usa a config de grupo entao valida por ela
                if (!configuracaoValida && (remetentePedido.GrupoPessoas != null && !remetentePedido.NaoUsarConfiguracaoEmissaoGrupo))
                    grupoPessoa = remetentePedido.GrupoPessoas.Codigo;

            }
            if (!configuracaoValida && grupoPessoa > 0)
            {
                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = repGrupoPessoas.BuscarPorCodigo(grupoPessoa);
                usarConfiguracaoEmissao = true;
                tipoEmissaoCTeParticipantes = grupoPessoas.TipoEmissaoCTeParticipantes;
                configuracaoValida = ValidarRegraTomador(tipoTomador, usarConfiguracaoEmissao, tipoEmissaoCTeParticipantes);
            }


            if (configuracaoValida)
            {
                return new JsonpResult(false, true, Localization.Resources.Pedidos.Pedido.TipoTomadorCompativel);
            }
            else
            {
                return new JsonpResult(false, false, Localization.Resources.Pedidos.Pedido.TomadorPrecisaConfiguracaoCompativel);
            }

        }

        public async Task<IActionResult> AlterarObservacaoPedido()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                if (TipoServicoMultisoftware == TipoServicoMultisoftware.MultiCTe)
                    throw new ControllerException(Localization.Resources.Pedidos.Pedido.AcaoNaoEPermitida);

                int codigo = Request.GetIntParam("Codigo");
                string observacao = Request.GetStringParam("Observacao");

                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = await repositorioPedido.BuscarPorCodigoAsync(codigo);

                if (pedido == null)
                    throw new ControllerException(Localization.Resources.Pedidos.Pedido.NaoFoipossivelEncontrarRegistro);

                Servicos.Embarcador.Pedido.Pedido servicoPedido = new Servicos.Embarcador.Pedido.Pedido(unitOfWork);

                unitOfWork.Start();
                servicoPedido.AdicionarPedidoObservacao(pedido, DateTime.Now, Usuario, observacao, unitOfWork);
                unitOfWork.CommitChanges();


                return new JsonpResult(true, Localization.Resources.Gerais.Geral.AdicionadaComSucesso);
            }
            catch (ControllerException excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Pedidos.Pedido.OcorreuUmaFalhaAoAtualizar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaPedidosCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("CodigoCarga");
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                Models.Grid.GridPreferencias gridPreferencias = new Models.Grid.GridPreferencias(unitOfWork, "Pedido/PesquisaPedidosCarga", "grid-selecao-multiplos-pedidos");
                grid.AplicarPreferenciasGrid(gridPreferencias.ObterPreferenciaGrid(this.Usuario.Codigo, grid.modelo));

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descricao", false);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.RetiradaPedidoLista.Carga, "NumeroCarga", 10, Models.Grid.Align.center, false);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    grid.AdicionarCabecalho(Localization.Resources.Pedidos.RetiradaPedidoLista.DescricaoPedido, "Numero", 8, Models.Grid.Align.center, true);
                else
                    grid.AdicionarCabecalho(Localization.Resources.Pedidos.RetiradaPedidoLista.SequencialPedido, "Numero", 8, Models.Grid.Align.center, true);

                grid.AdicionarCabecalho(Localization.Resources.Pedidos.RetiradaPedidoLista.DescricaoPedido, "NumeroPedidoEmbarcador", 12, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.RetiradaPedidoLista.Origem, "Origem", 12, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.RetiradaPedidoLista.Destino, "Destino", 12, Models.Grid.Align.center, true);

                if (carga == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                List<Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Pedido> dadosPedidos = await repositorioPedido.BuscarDadosPedidosPorCargaAsync(carga.Codigo);
                int totalRegistros = dadosPedidos.Count;

                var listaPedidoRetornar = (
                    from pedido in dadosPedidos
                    select new
                    {
                        Codigo = pedido.Codigo,
                        Descricao = pedido.NumeroPedidoEmbarcador,
                        NumeroCarga = carga.Numero,
                        Numero = pedido.Numero,
                        NumeroPedidoEmbarcador = pedido.NumeroPedidoEmbarcador,
                        Origem = pedido.Expedidor == null ? pedido.Origem?.DescricaoCidadeEstado ?? "" : pedido.Expedidor.Endereco.Localidade.DescricaoCidadeEstado,
                        Destino = pedido.Destino?.DescricaoCidadeEstado ?? ""
                    }
                ).ToList();

                grid.AdicionaRows(listaPedidoRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Pedidos.Pedido.OcorreuUmaFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion Métodos Globais

        #region Métodos Globais - Importações

        public async Task<IActionResult> ConfiguracaoImportacao()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Servicos.Embarcador.Pedido.Pedido servicoPedido = new Servicos.Embarcador.Pedido.Pedido(unitOfWork);
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = servicoPedido.ConfiguracaoImportacaoPedido(unitOfWork, TipoServicoMultisoftware);

                return new JsonpResult(configuracoes.OrderBy(c => c.Descricao).ToList());
            }
        }

        public async Task<IActionResult> ConfiguracaoImportacaoClientes()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ObterConfiguracaoImportacaoClientes();

            return new JsonpResult(configuracoes.ToList());
        }

        public async Task<IActionResult> ConfiguracaoImportacaoISISReturn()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ObterConfiguracaoImportacaoISISReturn();

            return new JsonpResult(configuracoes.ToList());
        }

        public async Task<IActionResult> ConfiguracaoImportacaoDadosNota()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ObterConfiguracaoImportacaoDadosNota();

            return new JsonpResult(configuracoes.ToList());
        }

        public async Task<IActionResult> ConfiguracaoImportacaoOcorrenciaPedido()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Servicos.Embarcador.Pedido.Pedido servicoPedido = new Servicos.Embarcador.Pedido.Pedido(unitOfWork);
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = servicoPedido.ConfiguracaoImportacaoOcorrenciaPorPedido(unitOfWork, TipoServicoMultisoftware);

                return new JsonpResult(configuracoes.ToList());
            }
        }

        public async Task<IActionResult> ConfiguracaoImportacaoAtualizacaoPedido()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Servicos.Embarcador.Pedido.Pedido servicoPedido = new Servicos.Embarcador.Pedido.Pedido(unitOfWork);
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = servicoPedido.ConfiguracaoImportacaoAtualizarPedido(unitOfWork);

                return new JsonpResult(configuracoes.ToList());
            }
        }

        public async Task<IActionResult> ConfiguracaoImportacaoBIDTransporte()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Servicos.Embarcador.Pedido.Pedido servicoPedido = new Servicos.Embarcador.Pedido.Pedido(unitOfWork);
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = servicoPedido.ConfiguracaoImportacaoBIDTransportePedido(unitOfWork);

                return new JsonpResult(configuracoes.ToList());
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisaCliente()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.Prop("Codigo");
                grid.Prop("CPF_CNPJ").Nome("CNPJ").Tamanho(30).Align(Models.Grid.Align.center);
                grid.Prop("Nome").Nome("Nome").Tamanho(40).Align(Models.Grid.Align.left);
                grid.Prop("Localidade").Nome("Cidade").Tamanho(20).Align(Models.Grid.Align.left);

                string objClientes = Request.Params("Clientes");

                List<double> cnpjsClientes = Newtonsoft.Json.JsonConvert.DeserializeObject<List<double>>(objClientes);
                Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);
                List<Dominio.Entidades.Cliente> clientes = repositorioCliente.BuscarPorVariosCPFCNPJ(cnpjsClientes);

                var lista = (
                    from o in clientes
                    select new Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoCliente
                    {
                        CPF_CNPJ = o.CPF_CNPJ_Formatado,
                        Nome = o.Nome,
                        Localidade = o.Localidade.DescricaoCidadeEstado
                    }
                ).ToList();

                grid.AdicionaRows(lista);

                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo == null)
                    return new JsonpResult(false, Localization.Resources.Pedidos.Pedido.OcorreuUmaFalhaAoGerarOArquivo);

                return Arquivo(bArquivo, "csv", "Clientes pedido.csv");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Pedidos.Pedido.OcorreuUmaFalhaAoExportar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Importar(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.Pedido.Pedido servicoPedido = new Servicos.Embarcador.Pedido.Pedido(unitOfWork);
                Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica = ObterOperadorLogistica(unitOfWork);

                string dados = Request.GetStringParam("Dados");
                var parametros = JsonConvert.DeserializeObject<dynamic>(Request.GetStringParam("Parametro"));
                List<(string Carga, Dominio.Entidades.Embarcador.Pedidos.MotivoImportacaoPedidoAtrasada Motivo)> motivosAtraso = ObterListaMotivosAtrasos(parametros.MotivosImportacaoAtrasada, unitOfWork);

                (string Nome, string Guid) arquivoGerador = ValueTuple.Create(Request.GetStringParam("Nome") ?? string.Empty, Request.GetStringParam("ArquivoSalvoComo") ?? string.Empty);

                servicoPedido.SetMotivosAtraso(motivosAtraso);

                Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = servicoPedido.ImportarPedido(dados, arquivoGerador, Usuario, operadorLogistica, TipoServicoMultisoftware, Cliente, Auditado, _conexao.AdminStringConexao, unitOfWork, cancellationToken);

                return new JsonpResult(retornoImportacao);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Pedidos.Pedido.OcorreuUmaFalhaAoImportarArquivo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ImportarClientes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                // Repositorios
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);

                // Configucarção de importacao
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ObterConfiguracaoImportacaoClientes();

                // Lista integrada em cada linha
                List<Dictionary<string, dynamic>> dadosLinhas = new List<Dictionary<string, dynamic>>();

                // Entidade para importacao
                List<Dominio.Entidades.Cliente> clientes = new List<Dominio.Entidades.Cliente>();

                // Chama serviço de importação
                Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retorno = Servicos.Embarcador.Importacao.Importacao.ImportarInformacoes(Request, configuracoes, ref clientes, ref dadosLinhas, out string erro, ((dicionario) =>
                {
                    Dominio.Entidades.Cliente cliente = null;

                    if (!dicionario.TryGetValue("CNPJ", out dynamic strCnpj)) strCnpj = "0";
                    strCnpj = Utilidades.String.OnlyNumbers(strCnpj);

                    double.TryParse((string)strCnpj, out double cnpj);
                    if (cnpj > 0)
                        cliente = repCliente.BuscarPorCPFCNPJ(cnpj);

                    return cliente;
                }));

                if (!string.IsNullOrWhiteSpace(erro))
                    return new JsonpResult(false, true, erro);

                if (retorno == null)
                    return new JsonpResult(false, Localization.Resources.Pedidos.Pedido.OcorreuUmaFalhaAoImportarArquivo);

                retorno.Importados = (from obj in clientes where obj.Codigo > 0 select obj.CPF_CNPJ).Count();
                retorno.Retorno = (from obj in clientes
                                   where obj.Codigo > 0
                                   select new
                                   {
                                       Codigo = obj.CPF_CNPJ,
                                       CPF_CNPJ = obj.CPF_CNPJ_Formatado,
                                       Localidade = obj.Localidade.DescricaoCidadeEstado,
                                       Nome = obj.Nome
                                   }).ToList();

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Pedidos.Pedido.OcorreuUmaFalhaAoImportarArquivo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ImportarISISReturn()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ObterConfiguracaoImportacaoISISReturn();
                List<Dictionary<string, dynamic>> dadosLinhas = new List<Dictionary<string, dynamic>>();

                Repositorio.Embarcador.Pedidos.PedidoAdicional repositorioPedidoAdicional = new Repositorio.Embarcador.Pedidos.PedidoAdicional(unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

                List<(Dominio.Entidades.Embarcador.Pedidos.Pedido Pedido, int IsisReturn)> listaRetorno = new List<(Dominio.Entidades.Embarcador.Pedidos.Pedido Pedido, int IsisReturn)>();

                Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retorno = Servicos.Embarcador.Importacao.Importacao.ImportarInformacoes(Request, configuracoes, ref listaRetorno, ref dadosLinhas, out string erro, ((dicionario) =>
                {
                    Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = null;

                    if (!dicionario.TryGetValue("NumeroNotaFiscal", out dynamic numeroNotaFiscal))
                        numeroNotaFiscal = "0";

                    int.TryParse((string)numeroNotaFiscal, out int numeroNf);

                    if (!dicionario.TryGetValue("NumeroIsisReturn", out dynamic dynIsisReturn))
                        dynIsisReturn = "0";

                    int.TryParse((string)dynIsisReturn, out int isisReturn);

                    if (numeroNf > 0)
                        pedido = repositorioPedido.BuscarPedidoDeDevolucaoPorNumeroNotaFiscal(numeroNf);

                    if (pedido != null)
                    {
                        Dominio.Entidades.Embarcador.Pedidos.PedidoAdicional pedidoAdicional = repositorioPedidoAdicional.BuscarPorPedido(pedido.Codigo);
                        if (pedidoAdicional == null)
                            pedidoAdicional = new Dominio.Entidades.Embarcador.Pedidos.PedidoAdicional()
                            {
                                Pedido = pedido
                            };

                        pedidoAdicional.ISISReturn = isisReturn;

                        if (pedidoAdicional.Codigo > 0)
                            repositorioPedidoAdicional.Atualizar(pedidoAdicional);
                        else
                            repositorioPedidoAdicional.Inserir(pedidoAdicional);
                    }

                    return (pedido, isisReturn);
                }));

                if (!string.IsNullOrWhiteSpace(erro))
                    return new JsonpResult(false, true, erro);

                if (retorno == null)
                    return new JsonpResult(false, Localization.Resources.Pedidos.Pedido.OcorreuUmaFalhaAoImportarArquivo);

                retorno.Importados = (from obj in listaRetorno where obj.IsisReturn > 0 && obj.Pedido != null select obj).Count();
                retorno.Retorno = (from obj in listaRetorno
                                   where obj.IsisReturn > 0
                                   && obj.Pedido != null
                                   select new
                                   {
                                       obj.Pedido.NumeroPedidoEmbarcador,
                                       obj.IsisReturn
                                   }).ToList();

                unitOfWork.CommitChanges();

                return new JsonpResult(retorno);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Pedidos.Pedido.OcorreuUmaFalhaAoImportarArquivo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ImportarDadosNota()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                //List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ConfigurarImportacaoCentroDescarregamento(unitOfWork);
                string dados = Request.Params("Dados");
                //  Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = Servicos.Embarcador.Logistica.CentroDescarregamento.ImportarCentroCarregamento(dados, this.Usuario, configuracoes, this.OperadorLogistica, TipoServicoMultisoftware, Auditado, _conexao.AdminStringConexao, unitOfWork);
                //return new JsonpResult(retornoImportacao);

                return null;
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Pedidos.Pedido.OcorreuUmaFalhaAoImportarArquivo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> VerificarPedidosPreImportacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.Pedido.Pedido serPedido = new Servicos.Embarcador.Pedido.Pedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.MotivoImportacaoPedidoAtrasada repMotivoImportacaoPedidoAtrasada = new Repositorio.Embarcador.Pedidos.MotivoImportacaoPedidoAtrasada(unitOfWork);

                string dados = Request.GetStringParam("Dados");

                List<string> cargasAtrasadas = new List<string>();

                if (repMotivoImportacaoPedidoAtrasada.PossuiMotivoAtivo())
                    cargasAtrasadas = serPedido.VerificarPedidosPreImportacao(dados, TipoServicoMultisoftware, Auditado, _conexao.AdminStringConexao, unitOfWork);

                return new JsonpResult(new
                {
                    CargasAtrasadas = cargasAtrasadas.Distinct().ToList()
                });
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Pedidos.Pedido.OcorreuUmaFalhaAoImportarArquivo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ImportarOcorrenciaPedido()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.Pedido.Pedido servicoPedido = new Servicos.Embarcador.Pedido.Pedido(unitOfWork);

                string dados = Request.GetStringParam("Dados");
                var parametros = JsonConvert.DeserializeObject<dynamic>(Request.GetStringParam("Parametro"));
                List<(string Carga, Dominio.Entidades.Embarcador.Pedidos.MotivoImportacaoPedidoAtrasada Motivo)> motivosAtraso = ObterListaMotivosAtrasos(parametros.MotivosImportacaoAtrasada, unitOfWork);

                (string Nome, string Guid) arquivoGerador = ValueTuple.Create(Request.GetStringParam("Nome") ?? string.Empty, Request.GetStringParam("ArquivoSalvoComo") ?? string.Empty);

                servicoPedido.SetMotivosAtraso(motivosAtraso);

                Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = servicoPedido.ImportarPedidoOcorrencia(dados, arquivoGerador, Usuario, TipoServicoMultisoftware, Cliente, Auditado, _conexao.AdminStringConexao, unitOfWork);

                return new JsonpResult(retornoImportacao);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao importar arquivo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterStagesPedido()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");
                int carga = Request.GetIntParam("Carga");
                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repPedido.BuscarPorCodigo(codigo);

                if (pedido == null)
                    return new JsonpResult(false, true, Localization.Resources.Pedidos.Pedido.NaoFoipossivelEncontrarRegistro);

                dynamic dynPedido = new
                {
                    Stages = ObterStagesPedido(pedido, carga)
                };

                return new JsonpResult(dynPedido);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Pedidos.Pedido.OcorreuUmaFalhaAoBuscarPorCodigo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RecalcularFreteAgrupamentoStage()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");
                int carga = Request.GetIntParam("Carga");
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Pedidos.StageAgrupamento repStageAgrupamento = new Repositorio.Embarcador.Pedidos.StageAgrupamento(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga cargaDT = repCarga.BuscarPorCodigo(carga, false);
                Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento agrupamento = repStageAgrupamento.BuscarPorCodigo(codigo, false);

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfigTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repConfigTMS.BuscarConfiguracaoPadrao();

                if (cargaDT == null || agrupamento == null)
                    return new JsonpResult(false, true, Localization.Resources.Pedidos.Pedido.NaoFoipossivelEncontrarRegistro);

                new Servicos.Embarcador.Pedido.CalculoFreteStagePedidoAgrupado(unitOfWork, TipoServicoMultisoftware, configuracaoEmbarcador).ReprocessarFreteAgrupamento(cargaDT, agrupamento);

                return new JsonpResult(true, "");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Pedidos.Pedido.OcorreuUmaFalhaAoBuscarPorCodigo);
            }
            finally
            {
                unitOfWork.Dispose();
            }


        }

        public async Task<IActionResult> SetarPedidoCritico()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoPedido = Request.GetIntParam("Codigo");
                int carga = Request.GetIntParam("Carga");
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repositorioCargaPedido.BuscarCargaPedidoPorPedido(codigoPedido);

                if (cargaPedido == null)
                    return new JsonpResult(false, true, Localization.Resources.Pedidos.Pedido.NaoFoipossivelEncontrarRegistro);

                Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                servicoCarga.SetarPedidoCritico(cargaPedido, Auditado, unitOfWork);

                var retorno = new
                {
                    CargaCritica = cargaPedido.Carga.CargaCritica ?? false,
                    PedidoCritico = cargaPedido.Pedido.PedidoCritico ?? false
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Pedidos.Pedido.OcorreuUmaFalhaAoBuscarPorCodigo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterPedidosOutrasCargas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                IList<Dominio.ObjetosDeValor.Embarcador.Carga.AcompanhamentoCarga.PedidoCargaVinculada> pedidos = repCarga.BuscarPedidoCargaVinculada(Request.GetIntParam("Carga"));

                return new JsonpResult(
                    from o in pedidos
                    select new
                    {
                        Codigo = o.codigo,
                        DT_RowId = o.codigo,
                        NumeroPedido = o.numeroPedido,
                        Carga = o.cargas,
                        Destintario = o.cliente
                    }); ;
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar as entregas da carga.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ImportarAtualizacaoPedido()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.Pedido.Pedido servicoPedido = new Servicos.Embarcador.Pedido.Pedido(unitOfWork);

                string dados = Request.GetStringParam("Dados");


                if (servicoPedido.VerificarSeExistePedidosDuplicados(dados))
                {
                    return new JsonpResult(false, "O arquivo contém pedidos duplicados");
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = servicoPedido.ImportarAtualizacaoPedido(dados, Auditado, unitOfWork);

                return new JsonpResult(retornoImportacao);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao importar arquivo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ImportarBIDTransportePedido()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.Pedido.Pedido servicoPedido = new Servicos.Embarcador.Pedido.Pedido(unitOfWork);

                string dados = Request.GetStringParam("Dados");

                Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = servicoPedido.ImportarBIDTransportePedido(dados, Auditado, unitOfWork);

                return new JsonpResult(retornoImportacao);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao importar arquivo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarHistoricoPedidoObservacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Pedidos.PedidoObservacao repositorioPedidoObservacao = new Repositorio.Embarcador.Pedidos.PedidoObservacao(unitOfWork);

                int codigoPedido = Request.GetIntParam("Codigo");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Observacao", false);
                grid.AdicionarCabecalho("Autor", "Usuario", 10, Models.Grid.Align.left, false, false, false, false, true);
                grid.AdicionarCabecalho("Data", "DataHoraInclusao", 10, Models.Grid.Align.left, false, false, false, false, true);

                List<Dominio.Entidades.Embarcador.Pedidos.PedidoObservacao> pedidoObservacoes = repositorioPedidoObservacao.BuscarPorPedido(codigoPedido);

                var retorno = (from observacao in pedidoObservacoes
                               select new
                               {
                                   observacao.Codigo,
                                   observacao.Observacao,
                                   DataHoraInclusao = observacao.DataHoraInclusao.ToString("dd/MM/yyyy HH:mm"),
                                   Usuario = observacao.Usuario.Descricao,
                               }).ToList();

                grid.AdicionaRows(retorno);
                grid.setarQuantidadeTotal(retorno.Count);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao consultar o historico de observações do pedido.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion Métodos Globais - Importações

        #region "   Liberar Pedidos   "

        [AllowAuthenticate]
        public async Task<IActionResult> ObterPedidosLiberarAgendamentoPortalRetira()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repositorioConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = repositorioConfiguracaoPedido.BuscarConfiguracaoPadrao();

                List<int> codigosFiliais = Request.GetListParam<int>("Filial");
                int situacao = Request.GetIntParam("Liberado");
                DateTime dataPedido = DateTime.Now.Date;
                TimeSpan time = Request.GetTimeParam("Hora");

                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = repositorioPedido.BuscarPedidosLiberacaoPortalAgendamentoRetira(codigosFiliais, dataPedido, time, situacao, configuracaoPedido.NaoExibirPedidosDoDiaAgendamentoPedidos);

                return new JsonpResult((
                    from o in pedidos
                    select new
                    {
                        Codigo = o.Codigo,
                        DT_RowId = o.Codigo,
                        Filial = o.Filial?.Descricao ?? string.Empty,
                        NumeroPedidoEmbarcador = o.NumeroPedidoEmbarcador,
                        DataCriacao = o.DataCriacao.Value.ToString("dd/MM/yyyy HH:mm:ss"),
                        Liberado = (!configuracaoPedido.NaoExibirPedidosDoDiaAgendamentoPedidos ? "SIM" : (o.PedidoLiberadoPortalRetira ? "SIM" : "NÃO"))
                    }).ToList());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar as entregas da carga.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> LiberarPedidos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<int> codigos = Request.GetListParam<int>("Codigos");

                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

                unitOfWork.Start();

                foreach (int codigoPedido in codigos)
                {
                    Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repositorioPedido.BuscarPorCodigo(codigoPedido);
                    pedido.PedidoLiberadoPortalRetira = true;
                    repositorioPedido.Atualizar(pedido);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true, true, "Pedidos liberados com sucesso.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Pedidos.Pedido.OcorreuUmaFalhaAoBuscarPorCodigo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void CancelarIntegracoesDoPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, UnitOfWork unitOfWork)
        {
            //verificar se possui integracoes que geram integracoes de pedidos
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> TipoIntegracoesGeramIntegracaoPedidos = repTipoIntegracao.BuscarTipoIntegracaoQueGeraIntegracaoPedido();

            foreach (Dominio.Entidades.Embarcador.Cargas.TipoIntegracao integracao in TipoIntegracoesGeramIntegracaoPedidos)
            {
                GerarIntegracaoCancelamentoPedido(pedido, integracao, unitOfWork);
            }
        }

        private void GerarIntegracoesPedidos(List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos, UnitOfWork unitOfWork)
        {
            //verificar se possui integracoes que geram integracoes de pedidos
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoIntegracao repPedidoIntegracao = new Repositorio.Embarcador.Pedidos.PedidoIntegracao(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> TipoIntegracoesGeramIntegracaoPedidos = repTipoIntegracao.BuscarTipoIntegracaoQueGeraIntegracaoPedido();
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao> integracaoesPendente = repPedidoIntegracao.BuscarPendentesIntegracao();

            Servicos.Embarcador.Pedido.Pedido servicoPedido = new(unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Cargas.TipoIntegracao integracao in TipoIntegracoesGeramIntegracaoPedidos)
            {
                foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidos)
                {
                    if (integracaoesPendente.Any(o => o.Pedido.Codigo == pedido.Codigo))
                        return;

                    servicoPedido.GerarIntegracaoAsync(pedido, integracao).GetAwaiter().GetResult();
                }
            }
        }

        private void GerarIntegracaoCancelamentoPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao TipoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoIntegracao repPedidoIntegracao = new Repositorio.Embarcador.Pedidos.PedidoIntegracao(unitOfWork);
            Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao pedidoIntegracao = new Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao()
            {
                Pedido = pedido,
                SituacaoIntegracao = SituacaoIntegracao.AgIntegracao,
                Tentativas = 0,
                TipoIntegracao = TipoIntegracao,
                ProblemaIntegracao = string.Empty,
                IntegracaoCancelamento = true,
                DataEnvio = DateTime.Now
            };

            repPedidoIntegracao.Inserir(pedidoIntegracao);
        }

        private dynamic ObterDadosAdicionaisPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, bool duplicar, string LinkRastreio, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoAdicional repositorioPedidoAdicionais = new Repositorio.Embarcador.Pedidos.PedidoAdicional(unitOfWork);
            Dominio.Entidades.Embarcador.Pedidos.PedidoAdicional pedidoAdicionais = repositorioPedidoAdicionais.BuscarPorPedido(pedido.Codigo);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoRestricaoDiaEntrega repositorioPedidoRestricaoDiaEntrega = new Repositorio.Embarcador.Pedidos.PedidoRestricaoDiaEntrega(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioXmlNotafistal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoRestricaoDiaEntrega> pedidoRestricaoDiasEntrega = repositorioPedidoRestricaoDiaEntrega.BuscarPorPedido(pedido.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargaPedido = repCargaPedido.BuscarPorPedido(pedido.Codigo);
            decimal valor = repositorioXmlNotafistal.ObterValorTotalPorCarga(pedido.Codigo);
            DateTime dataEmissaoPrimeiraNFE = repositorioXmlNotafistal.BuscarDataEmissaoPrimeiraNFE(pedido.Codigo);

            // ANTES DE ADICIONAR SEU CAMPO, VERIFIQUE SE ELE NÃO SE ENCAIXE NUM DOS GRUPOS NO FINAL DESTE OBJETO
            return new
            {
                LinkRastreio,
                pedido.ProdutoPredominante,
                pedido.Distancia,
                CodigoPedidoCliente = pedido.CodigoPedidoCliente ?? "",
                Companhia = pedido.Companhia ?? "",
                CubagemTotalTMS = pedido.CubagemTotal.ToString("n3"),
                pedido.LacreContainerDois,
                pedido.LacreContainerTres,
                pedido.LacreContainerUm,
                pedido.NumeroContainer,
                PesoLiquidoTotal = pedido.PesoLiquidoTotal.ToString("n2"),
                PesoTotalPaletes = pedido.PesoTotalPaletes.ToString("n2"),
                LocalPaletizacao = new { Codigo = pedido.LocalPaletizacao != null ? pedido.LocalPaletizacao.Codigo : 0, Descricao = pedido.LocalPaletizacao != null ? pedido.LocalPaletizacao.Descricao : "" },
                NumeroRastreioCorreios = pedido.NumeroRastreioCorreios != null ? pedido.NumeroRastreioCorreios : "",
                pedido.NumeroControle,
                NumeroPaletesPagos = pedido.NumeroPaletesPagos.ToString("n3"),
                NumeroSemiPaletes = pedido.NumeroSemiPaletes.ToString("n3"),
                NumeroSemiPaletesPagos = pedido.NumeroSemiPaletesPagos.ToString("n3"),
                NumeroCombis = pedido.NumeroCombis.ToString("n3"),
                NumeroCombisPagas = pedido.NumeroCombisPagas.ToString("n3"),
                CodigoAgrupamentoCarregamento = pedido?.CodigoAgrupamentoCarregamento ?? string.Empty,
                NumeroContratoFreteCliente = new
                {
                    pedido.ContratoFreteCliente?.Codigo,
                    pedido.ContratoFreteCliente?.Descricao
                },
                pedido.NumeroDTA,
                NumeroNavio = pedido.NumeroNavio ?? "",
                ObservacaoCTe = pedido.ObservacaoCTe != null ? pedido.ObservacaoCTe : "",
                ObservacaoAbaPedido = pedido.ObservacaoCTe ?? "",
                pedido.ObservacaoInterna,
                pedido.DeclaracaoObservacaoCRT,
                pedido.ObservacaoEntrega,
                Ordem = pedido.Ordem ?? "",
                Pallets = pedido.NumeroPaletes.ToString(),
                PortoSaida = pedido.PortoSaida ?? "",
                PortoChegada = pedido.PortoChegada ?? "",
                UsuarioCriacaoRemessa = pedido.UsuarioCriacaoRemessa ?? string.Empty,
                pedido.RotaEmbarcador,
                pedido.NumeroOrdem,
                RegiaoDestino = new { Codigo = pedido.RegiaoDestino?.Codigo ?? 0, Descricao = pedido.RegiaoDestino?.Descricao ?? string.Empty },
                pedido.QuantidadeNotasFiscais,
                QtdAjudantes = pedido.QtdAjudantes > 0 ? pedido.QtdAjudantes.ToString("n0") : string.Empty,
                QtdEntregas = pedido.QtdEntregas.ToString("n0"),
                QtdEscolta = pedido.QtdEscolta > 0 ? pedido.QtdEscolta.ToString("n0") : string.Empty,
                pedido.QtVolumes,
                pedido.QuantidadeVolumesPrevios,
                pedido.Requisitante,
                Reserva = pedido.Reserva ?? "",
                ResponsavelRedespacho = new { Codigo = pedido.ResponsavelRedespacho?.CPF_CNPJ ?? 0, Descricao = pedido.ResponsavelRedespacho?.Nome ?? "" },
                FuncionarioVendedor = new { Codigo = pedido.FuncionarioVendedor?.Codigo ?? 0, Descricao = pedido.FuncionarioVendedor?.Nome ?? "" },
                FuncionarioSupervisor = new { Codigo = pedido.FuncionarioSupervisor?.Codigo ?? 0, Descricao = pedido.FuncionarioSupervisor?.Nome ?? "" },
                FuncionarioGerente = new { Codigo = pedido.FuncionarioGerente?.Codigo ?? 0, Descricao = pedido.FuncionarioGerente?.Nome ?? "" },
                CentroResultadoEmbarcador = new { Codigo = pedido.CentroResultadoEmbarcador?.Codigo ?? 0, Descricao = pedido.CentroResultadoEmbarcador?.Descricao ?? "" },
                pedido.ElementoPEP,
                ContaContabil = new { Codigo = pedido.ContaContabil?.Codigo ?? 0, Descricao = pedido.ContaContabil?.Descricao ?? "" },
                Expedidor = pedido.Expedidor != null ? new { Codigo = pedido.Expedidor != null ? pedido.Expedidor.CPF_CNPJ : 0, Descricao = pedido.Expedidor != null ? pedido.Expedidor.Descricao : "" } : null,
                Resumo = pedido.Resumo ?? "",
                Transportador = new { Codigo = pedido.Empresa?.Codigo ?? 0, Descricao = pedido.Empresa?.Descricao ?? "" },
                pedido.SenhaAgendamento,
                pedido.SenhaAgendamentoCliente,
                SerieCTe = new { Codigo = pedido.EmpresaSerie?.Codigo ?? 0, Descricao = pedido.EmpresaSerie?.Numero.ToString() ?? "" },
                pedido.TaraContainer,
                pedido.Temperatura,
                TipoEmbarque = pedido.TipoEmbarque ?? "",
                pedido.TipoPagamento,
                Tomador = new { Codigo = pedido.Tomador?.CPF_CNPJ ?? 0, Descricao = pedido.Tomador?.Descricao ?? "" },
                ValorCarregamento = pedido.ValorCarga,
                pedido.ValorDescarga,
                pedido.ValorDeslocamento,
                pedido.ValorDiaria,
                ValorFreteCotado = pedido.ValorFreteCotado.ToString("n2"),
                ValorFreteNegociado = pedido.ValorFreteNegociado > 0 ? pedido.ValorFreteNegociado.ToString("n2") : "",
                ValorFreteTransportadorTerceiro = pedido.ValorFreteTransportadorTerceiro > 0m ? pedido.ValorFreteTransportadorTerceiro.ToString("n2") : "",
                ValorFreteToneladaNegociado = pedido.ValorFreteToneladaNegociado > 0 ? pedido.ValorFreteToneladaNegociado.ToString("n2") : "",
                ValorFreteToneladaTerceiro = pedido.ValorFreteToneladaTerceiro > 0m ? pedido.ValorFreteToneladaTerceiro.ToString("n2") : "",
                ValorTotalCarga = pedido.ValorTotalCarga.ToString("n2"),
                ValorTotalNotasFiscais = pedido.ValorTotalNotasFiscais > 0 ? pedido.ValorTotalNotasFiscais.ToString("n2") : "",
                ValorPedagioRota = pedido.ValorPedagioRota > 0 ? pedido.ValorPedagioRota.ToString("n2") : "",
                Vendedor = pedido.Vendedor ?? "",
                RecebedorColeta = pedido.RecebedorColeta != null ? new { Codigo = pedido.RecebedorColeta != null ? pedido.RecebedorColeta.CPF_CNPJ : 0, Descricao = pedido.RecebedorColeta != null ? pedido.RecebedorColeta.Descricao : "" } : null,
                ValorAdValorem = pedido.ValorAdValorem.ToString("n3"),
                pedido.IDBAF,
                pedido.ValorBAF,
                pedido.NumeroBooking,
                pedido.NumeroOS,
                pedido.NumeroProposta,
                ProvedorOS = pedido.ProvedorOS != null ? new { Codigo = pedido.ProvedorOS != null ? pedido.ProvedorOS.CPF_CNPJ : 0, Descricao = pedido.ProvedorOS != null ? pedido.ProvedorOS.Descricao : "" } : null,
                PedidoViagemNavio = pedido.PedidoViagemNavio != null ? new { Codigo = pedido.PedidoViagemNavio != null ? pedido.PedidoViagemNavio.Codigo : 0, Descricao = pedido.PedidoViagemNavio != null ? pedido.PedidoViagemNavio.Descricao : "" } : null,
                PedidoEmpresaResponsavel = pedido.PedidoEmpresaResponsavel != null ? new { Codigo = pedido.PedidoEmpresaResponsavel != null ? pedido.PedidoEmpresaResponsavel.Codigo : 0, Descricao = pedido.PedidoEmpresaResponsavel != null ? pedido.PedidoEmpresaResponsavel.Descricao : "" } : null,
                PedidoCentroCusto = pedido.PedidoCentroCusto != null ? new { Codigo = pedido.PedidoCentroCusto != null ? pedido.PedidoCentroCusto.Codigo : 0, Descricao = pedido.PedidoCentroCusto != null ? pedido.PedidoCentroCusto.Descricao : "" } : null,
                pedido.FormaAverbacaoCTE,
                ValorFreteInformativo = pedido.ValorFreteInformativo.ToString("n2"),
                ProcImportacao = pedido.Adicional1,
                CanalEntrega = pedido.CanalEntrega != null ? new { pedido.CanalEntrega.Codigo, pedido.CanalEntrega.Descricao } : null,
                CanalVenda = new { Codigo = pedido.CanalVenda?.Codigo ?? 0, Descricao = pedido.CanalVenda?.Descricao ?? string.Empty },
                Deposito = pedido.Deposito != null ? new { pedido.Deposito.Codigo, pedido.Deposito.Descricao } : null,
                pedido.TipoTomador,
                PedidoTipoPagamento = new { Descricao = pedido.PedidoTipoPagamento?.Descricao ?? string.Empty, Codigo = pedido.PedidoTipoPagamento?.Codigo ?? 0 },
                pedido.NumeroEXP,
                PontoPartida = new { Descricao = pedido.PontoPartida?.Descricao ?? string.Empty, Codigo = pedido.PontoPartida?.Codigo ?? 0 },
                LocalidadeInicioPrestacao = new { Codigo = pedido.LocalidadeInicioPrestacao?.Codigo ?? 0, Descricao = pedido.LocalidadeInicioPrestacao?.DescricaoCidadeEstado },
                LocalidadeTerminoPrestacao = new { Codigo = pedido.LocalidadeTerminoPrestacao?.Codigo ?? 0, Descricao = pedido.LocalidadeTerminoPrestacao?.DescricaoCidadeEstado },
                pedido.ValorCustoFrete,
                pedido.Safra,
                PercentualAdiantamentoTerceiro = pedido.PercentualAdiantamentoTerceiro.ToString("n2"),
                PercentualMinimoAdiantamentoTerceiro = pedido.PercentualMinimoAdiantamentoTerceiro.ToString("n2"),
                PercentualMaximoAdiantamentoTerceiro = pedido.PercentualMaximoAdiantamentoTerceiro.ToString("n2"),
                pedido.Observacao,
                ValoNFe = valor.ToString("n2"),
                ValorGross = pedido.GrossSales.ToString("n2"),
                NumeroPedidoICT = pedidoAdicionais?.NumeroPedidoICT ?? "",
                CondicaoExpedicao = pedidoAdicionais?.CondicaoExpedicao ?? "",
                GrupoFreteMaterial = pedidoAdicionais?.GrupoFreteMaterial ?? "",
                RestricaoEntrega = pedidoAdicionais?.RestricaoEntrega ?? "",
                NumeroPedidoVinculado = pedidoAdicionais?.NumeroPedidoVinculado ?? string.Empty,
                QtdAjudantesCarga = pedidoAdicionais?.QtdAjudantesCarga ?? null,
                QtdAjudantesDescarga = pedidoAdicionais?.QtdAjudantesDescarga ?? null,
                QtdIsca = pedido?.QtdIsca ?? null,
                IndicadorPOF = pedidoAdicionais?.IndicadorPOF ?? "",
                ISISReturn = pedidoAdicionais?.ISISReturn ?? 0,
                ProcessamentoEspecial = new { Codigo = pedidoAdicionais?.ProcessamentoEspecial?.Codigo ?? 0, Descricao = pedidoAdicionais?.ProcessamentoEspecial?.Descricao ?? "" },
                PeriodoEntrega = new { Codigo = pedidoAdicionais?.PeriodoEntrega?.Codigo ?? 0, Descricao = pedidoAdicionais?.PeriodoEntrega?.Descricao ?? "" },
                HorarioEntrega = new { Codigo = pedidoAdicionais?.HorarioEntrega?.Codigo ?? 0, Descricao = pedidoAdicionais?.HorarioEntrega?.Descricao ?? "" },
                DetalheEntrega = new { Codigo = pedidoAdicionais?.DetalheEntrega?.Codigo ?? 0, Descricao = pedidoAdicionais?.DetalheEntrega?.Descricao ?? "" },
                NumeroPedidoOrigem = new { Codigo = pedidoAdicionais?.PedidoOrigem?.Codigo ?? 0, Descricao = pedidoAdicionais?.PedidoOrigem?.Descricao ?? "" },
                RestricaoDiasEntrega = string.Join(", ", (from obj in pedidoRestricaoDiasEntrega select Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemanaHelper.ObterDescricaoResumida(obj.Dia)).ToList()),
                IndicativoColetaEntrega = pedidoAdicionais?.IndicativoColetaEntrega ?? IndicativoColetaEntrega.NaoInformado,
                TipoServico = pedidoAdicionais?.TipoServico ?? string.Empty,
                NumeroAutorizacaoColetaEntrega = pedidoAdicionais?.NumeroAutorizacaoColetaEntrega ?? string.Empty,
                ClientePropostaComercial = new { Codigo = pedidoAdicionais?.ClientePropostaComercial?.CPF_CNPJ ?? 0d, Descricao = pedidoAdicionais?.ClientePropostaComercial?.Descricao ?? "" },
                TipoSeguro = pedidoAdicionais?.TipoSeguro ?? string.Empty,
                NumeroOSMae = pedidoAdicionais?.NumeroOSMae ?? string.Empty,
                NumeroProtocoloIntegracaoPedido = pedido.Protocolo,
                NumeroProtocoloIntegracaoCarga = string.Join(", ", from obj in listaCargaPedido where obj.CargaOrigem.SituacaoCarga != SituacaoCarga.Cancelada && obj.CargaOrigem.SituacaoCarga != SituacaoCarga.Anulada && obj.CargaOrigem.Protocolo > 0 select obj.CargaOrigem.Protocolo),
                CentroDeCustoViagem = new { Codigo = pedido.CentroDeCustoViagem?.Codigo ?? 0, Descricao = pedido.CentroDeCustoViagem?.Descricao ?? string.Empty },
                Balsa = new { Codigo = pedido.Balsa?.Codigo ?? 0, Descricao = pedido.Balsa?.Descricao ?? string.Empty },
                pedido.ValorCobrancaFreteCombinado,
                pedido.CategoriaOS,
                pedido.ValorTotalProvedor,
                TipoOS = pedido.TipoOS == null ? TipoOS.NaoInformado : pedido.TipoOS,
                TipoOSConvertido = pedido.TipoOSConvertido == null ? TipoOSConvertido.NaoInformado : pedido.TipoOSConvertido,
                // Adicione seu campo aqui se ele for um DateTime ou a string de um DateTime
                DatasHorarios = ObterDatasHorariosDadosAdicionais(pedido, pedidoAdicionais, dataEmissaoPrimeiraNFE),
                // Adicione seu campo aqui se ele for um simples booleano
                Validacoes = ObterValidacoesDadosAdicionais(pedido, pedidoAdicionais, duplicar),
                MercadoLivreRota = pedido.Rota,
                MercadoLivreFacility = pedido.Facility,
                Incoterm = pedidoAdicionais?.Incoterm,
                TransitoAduaneiro = pedidoAdicionais?.TransitoAduaneiro ?? TransitoAduaneiro.Sim,
                NotificacaoCRT = new { Descricao = pedidoAdicionais?.NotificacaoCRT?.Descricao ?? string.Empty, Codigo = pedidoAdicionais?.NotificacaoCRT?.Codigo ?? 0 },
                DtaRotaPrazoTransporte = pedidoAdicionais?.DtaRotaPrazoTransporte ?? string.Empty,
                TipoEmbalagem = new { Descricao = pedidoAdicionais?.TipoEmbalagem?.Descricao ?? string.Empty, Codigo = pedidoAdicionais?.TipoEmbalagem?.Codigo ?? 0 },
                DetalheMercadoria = pedidoAdicionais?.DetalheMercadoria ?? string.Empty,
            };
        }

        private dynamic ObterDadosContainer(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            return new
            {
                pedido.LacreContainerDois,
                pedido.LacreContainerTres,
                pedido.LacreContainerUm,
                pedido.NumeroContainer,
                ContainerTipoReservaFluxoContainer = new { Codigo = pedido.ContainerTipoReservaFluxoContainer?.Codigo ?? 0, Descricao = pedido.ContainerTipoReservaFluxoContainer?.Descricao ?? string.Empty },
                pedido.NumeroBooking,
                pedido.NumeroOS,
                pedido.TaraContainer,
                DataChip = pedido.DataChip.HasValue ? pedido.DataChip.Value.ToString("dd/MM/yyyy") : string.Empty,
                DataCancel = pedido.DataCancel.HasValue ? pedido.DataCancel.Value.ToString("dd/MM/yyyy") : string.Empty,

            };
        }

        private dynamic ObterDatasHorariosDadosAdicionais(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.Entidades.Embarcador.Pedidos.PedidoAdicional pedidoAdicionais, DateTime dataEmissaoPrimeiraNFE)
        {
            return new
            {
                DataCancel = pedido.DataCancel.HasValue ? pedido.DataCancel.Value.ToString("dd/MM/yyyy") : string.Empty,
                DataChip = pedido.DataChip.HasValue ? pedido.DataChip.Value.ToString("dd/MM/yyyy") : string.Empty,
                DataETA = pedido.DataETA.HasValue ? pedido.DataETA.Value.ToString("dd/MM/yyyy HH:mm") : "",
                DataInicialViagemExecutada = pedido.DataInicialViagemExecutada?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataInicialViagemFaturada = pedido.DataInicialViagemFaturada?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataFinalViagemExecutada = pedido.DataFinalViagemExecutada?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataFinalViagemFaturada = pedido.DataFinalViagemFaturada?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataValidade = pedido.DataValidade?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataInicioJanelaDescarga = pedido.DataInicioJanelaDescarga?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataOrder = pedido.DataOrder.HasValue ? pedido.DataOrder.Value.ToString("dd/MM/yyyy") : string.Empty,
                DataPrevisaoEntrega = pedido.PrevisaoEntrega?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataPrevisaoSaida = pedido.DataPrevisaoSaida?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataTerminoCarregamento = pedido.DataTerminoCarregamento?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                DataAgendamento = pedido.DataAgendamento?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                DataAlocacaoPedido = pedido.DataAlocacaoPedido?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                DataUltimaLiberacao = pedido.DataUltimaLiberacao?.ToDateTimeString() ?? string.Empty,
                DataBaseCRT = pedido.DataBaseCRT?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                PrevisaoEntregaTransportador = pedido.PrevisaoEntregaTransportador.HasValue ? pedido.PrevisaoEntregaTransportador.Value.ToString("dd/MM/yyyy") : string.Empty,
                DataCriacaoRemessa = pedidoAdicionais?.DataCriacaoRemessa?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                DataCriacaoVenda = pedidoAdicionais?.DataCriacaoVenda?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                DataPrevisaoTerminoCarregamento = pedido.DataPrevisaoTerminoCarregamento?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                DataEmissaoNFe = dataEmissaoPrimeiraNFE != DateTime.MinValue && dataEmissaoPrimeiraNFE != null ? dataEmissaoPrimeiraNFE.ToString("dd/MM/yyyy HH:mm") : "",
            };
        }

        private dynamic ObterValidacoesDadosAdicionais(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.Entidades.Embarcador.Pedidos.PedidoAdicional pedidoAdicionais, bool duplicar)
        {
            return new
            {
                pedido.Ajudante,
                pedido.QtdAjudantes,
                pedido.Cotacao,
                pedido.DespachoTransitoAduaneiro,
                pedido.DisponibilizarPedidoParaColeta,
                Escolta = pedido.EscoltaArmada,
                pedido.EscoltaMunicipal,
                pedido.GerarAutomaticamenteCargaDoPedido,
                pedido.EntregaAgendada,
                pedido.QuebraMultiplosCarregamentos,
                ExecaoCab = pedidoAdicionais?.ExecaoCab ?? false,
                PedidoPaletizado = pedidoAdicionais?.PedidoPaletizado ?? false,
                pedido.GerenciamentoRisco,
                pedido.ImprimirObservacaoCTe,
                pedido.NecessarioReentrega,
                ContemCargaRefrigeradaDescricao = pedido.ContemCargaRefrigerada,
                PossuiCargaPerigosaDescricao = pedido.PossuiCargaPerigosa,
                pedido.PedidoBloqueado,
                pedido.PedidoRestricaoData,
                pedido.PedidoSubContratado,
                pedido.PedidoTransbordo,
                PossuiCarregamento = pedido.PossuiCarga,
                pedido.PossuiDescarga,
                pedido.PossuiDeslocamento,
                pedido.PossuiDiaria,
                pedido.Rastreado,
                pedido.Seguro,
                ViagemJaOcorreu = !duplicar && pedido.SituacaoPedido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Finalizado,
                pedido.NecessitaAverbacaoAutomatica,
                pedido.PossuiCargaPerigosa,
                pedido.PedidoLiberadoPortalRetira,
                pedido.ContemCargaRefrigerada,
                pedido.NecessitaEnergiaContainerRefrigerado,
                pedido.ValidarDigitoVerificadorContainer,
                pedido.PedidoSVM,
                pedido.PedidoDeSVMTerceiro,
                pedido.ContainerADefinir,
                pedido.UsarTipoTomadorPedido,
                pedido.PossuiIsca,
                pedido.CustoFrete,
                AjudanteCarga = pedidoAdicionais?.AjudanteCarga ?? false,
                AjudanteDescarga = pedidoAdicionais?.AjudanteDescarga ?? false,
                EssePedidopossuiPedidoBonificacao = pedidoAdicionais?.EssePedidopossuiPedidoBonificacao ?? false,
                EssePedidopossuiPedidoVenda = pedidoAdicionais?.EssePedidopossuiPedidoVenda ?? false,
                ProdutoVolumoso = pedidoAdicionais?.ProdutoVolumoso ?? false,
                pedido.Substituicao,
            };
        }

        private dynamic ObterDadosAutorizacaoPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoAutorizacao repPedidoAutorizacao = new Repositorio.Embarcador.Pedidos.PedidoAutorizacao(unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.PedidoAutorizacao autorizacao = repPedidoAutorizacao.BuscarAutorizacaoPedido(pedido.Codigo);

            return new
            {
                SituacaoSolicitacao = SituacaoSolicitacao(pedido),
                DescricaoSituacao = pedido.DescricaoSituacaoPedido,
                DataEmissao = pedido.DataCarregamentoPedido.HasValue ? pedido.DataCarregamentoPedido.Value.ToString("dd/MM/yyyy") : string.Empty,
                Solicitado = pedido.Usuario?.Nome,
                Motorista = pedido.NomeMotoristas,
                Peso = pedido.PesoTotal.ToString("n2"),
                QtdEntregas = pedido.QtdEntregas.ToString("n0"),
                TipoCarga = pedido.TipoDeCarga?.Descricao ?? string.Empty,
                ModeloVeicular = pedido.ModeloVeicularCarga?.Descricao ?? string.Empty,
                Remetente = pedido.GrupoPessoas != null ? pedido.GrupoPessoas.Descricao : pedido.Remetente != null ? pedido.Remetente.Nome : string.Empty,
                Destinatario = pedido.Destinatario?.Nome ?? string.Empty,
                ValorNegociado = pedido.ValorFreteNegociado.ToString("n2"),
                ValorFrete = pedido.ValorFreteNegociado.ToString("n2"),
                Observacao = pedido.Observacao,
                DataRetorno = autorizacao?.Data.ToString("dd/MM/yyyy") ?? string.Empty,
                Creditor = autorizacao?.Usuario.Nome,
                ValorLiberado = autorizacao != null && autorizacao.Situacao == SituacaoOcorrenciaAutorizacao.Aprovada ? pedido.ValorFreteNegociado.ToString("n2") : string.Empty,
                RetornoSolicitacao = autorizacao?.Motivo ?? string.Empty,
                ComRegraAutorizacao = autorizacao != null
            };
        }

        private dynamic ObterDadosOcorrenciaColetaEntregaPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, string LinkRastreio, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega repPedidoOcorrenciaColetaEntrega = new Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega(unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega> pedidoOcorrenciasColetaEntrega = repPedidoOcorrenciaColetaEntrega.BuscarPorPedido(pedido.Codigo);

            return pedidoOcorrenciasColetaEntrega.Select(o => new
            {
                o.Codigo,
                DataOcorrencia = o.DataOcorrencia.ToString("dd/MM/yyyy HH:mm"),
                o.Observacao,
                o.TipoDeOcorrencia.Descricao,
                Cliente = o.Alvo?.Descricao ?? "",
                o.Pacote,
                o.Volumes,
                o.ObservacaoOcorrencia

            }).ToList();

        }

        private dynamic ObterDadosResumoPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoCancelamento repPedidoCancelamento = new Repositorio.Embarcador.Pedidos.PedidoCancelamento(unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.PedidoCancelamento cancelamento = repPedidoCancelamento.BuscarPorPedido(pedido.Codigo);

            return new
            {
                DataColeta = pedido.DataCarregamentoPedido?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataPrevisao = pedido.DataPrevisaoSaida?.ToString("dd/MM/yyyy HH:mm") ?? "",
                Etapa = pedido.DescricaoEtapaPedido,
                MotivoCancelamentoWS = (cancelamento != null && cancelamento.Tipo == TipoPedidoCancelamento.Cancelamento) ? cancelamento.MotivoCancelamento : "",
                Motoristas = pedido.NomeMotoristas,
                NumeroPedido = pedido.NumeroPedidoEmbarcador, //duplicar ? "" : TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador ? (pedido.Numero > 0 ? pedido.Numero.ToString() : string.Join(", ", pedido.CargasPedido.Select(o => o.CodigoCargaEmbarcador).Distinct())) : pedido.CodigoCargaEmbarcador,
                Remetente = pedido.Remetente != null ? pedido.Remetente.Descricao : pedido.GrupoPessoas?.Descricao,
                Situacao = pedido.DescricaoSituacaoPedido,
                TipoOperacao = pedido.TipoOperacao?.Descricao ?? "",
                Veiculo = BuscarPlacasResumo(pedido),
                NumeroTransporte = string.Join(", ", (from obj in pedido.NotasFiscais select obj.NumeroTransporte).ToList()),
                DataAgendamento = pedido.DataAgendamento?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataInclusaoPedido = pedido.DataCriacao?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataCriacaoPedidoERP = pedido.DataDeCriacaoPedidoERP?.ToString("dd/MM/yyyy HH:mm") ?? "",
                SituacaoComercialPedido = pedido.SituacaoComercialPedido?.Descricao ?? "",
                SituacaoComercialPedidoBolinha = pedido.SituacaoComercialPedido?.Cor ?? "#FFFFFF",
                MotivoCancelamento = pedido.SituacaoPedido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Cancelado ? pedido.MotivoCancelamentoPedido?.Descricao ?? pedido.MotivoCancelamento : null,
            };
        }

        private dynamic ObterDadosTransbordoPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, bool duplicar)
        {
            return pedido.PedidosTransbordo.Select(obj => new
            {
                Codigo = duplicar ? 0 : obj.Codigo,
                SequenciaTransbordo = obj.Sequencia,
                PortoTransbordo = obj.Porto?.Descricao ?? "",
                CodigoPortoTransbordo = obj.Porto?.Codigo ?? 0,
                NavioTransbordo = obj.Navio?.Descricao ?? "",
                CodigoNavioTransbordo = obj.Navio?.Codigo ?? 0,
                TerminalTransbordo = obj.Terminal?.Descricao ?? "",
                CodigoTerminalTransbordo = obj.Terminal?.Codigo ?? 0,
                PedidoViagemNavioTransbordo = obj.PedidoViagemNavio?.Descricao ?? "",
                CodigoPedidoViagemNavioTransbordo = obj.PedidoViagemNavio?.Codigo ?? 0,
            }).ToList();
        }

        private dynamic ObterDadosDIPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, bool duplicar)
        {
            return pedido.PedidoImportacao.Select(obj => new
            {
                Codigo = duplicar ? 0 : obj.Codigo,
                obj.CodigoImportacao,
                obj.CodigoReferencia,
                obj.NumeroDI,
                Peso = obj.Peso.ToString("n2"),
                ValorCarga = obj.ValorCarga.ToString("n2"),
                Volume = obj.Volume.ToString("n2")
            }).ToList();
        }

        private dynamic ObterDadosComponentesFretePedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, bool duplicar)
        {
            return pedido.PedidosComponente
                         .Where(obj => duplicar ? obj.ComponenteFrete.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ADVALOREM : true)
                         .Select(obj => new
                         {
                             Codigo = duplicar ? 0 : obj.Codigo,
                             ComponenteFrete = new { Descricao = obj.ComponenteFrete?.Descricao ?? "", Codigo = obj.ComponenteFrete?.Codigo ?? 0 },
                             Percentual = obj.Percentual.ToString("n3"),
                             Valor = obj.ValorComponente.ToString("n2")
                         }).ToList();
        }

        private dynamic ObterDadosDestinatariosBloqueadosPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            return pedido.DestinatariosBloqueados.Select(o => new
            {
                CNPJCPFDestinatarioBloqueado = o
            }).ToList();
        }

        private dynamic ObterDadosMotoristasPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            return pedido.Motoristas.OrderBy(obj => obj.Nome).Select(obj => new
            {
                obj.Codigo,
                CPF = obj.CPF_Formatado,
                obj.Nome
            }).ToList();
        }

        private dynamic ObterDadosClientesPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            return pedido.Clientes.OrderBy(o => o.Nome).Select(obj => new
            {
                obj.Codigo,
                CPF_CNPJ = obj.CPF_CNPJ_Formatado,
                obj.Nome,
                Localidade = obj?.Localidade?.DescricaoCidadeEstado ?? string.Empty
            }).ToList();
        }

        private dynamic ObterDadosProdutosPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, bool duplicar, Repositorio.UnitOfWork unitOfWork)
        {
            bool retornarSaldoProduto = Servicos.Embarcador.Carga.MontagemCarga.SaldoPedidoProduto.ApresentarSaldoProdutoGridPedido(pedido, pedido.TipoOperacao);
            List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SaldoPedidoProduto> saldoProdutos = new List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SaldoPedidoProduto>();
            if (retornarSaldoProduto && !duplicar)
                saldoProdutos = Servicos.Embarcador.Carga.MontagemCarga.SaldoPedidoProduto.ObterSaldoPedidoProdutos(pedido.Codigo, unitOfWork);

            return pedido.Produtos.Select(obj => new
            {
                Codigo = duplicar ? 0 : obj.Codigo,
                AlturaCM = obj.AlturaCM.ToString("n6"),
                ComprimentoCM = obj.ComprimentoCM.ToString("n6"),
                LarguraCM = obj.LarguraCM.ToString("n6"),
                CodigoProdutoEmbarcador = obj.Produto != null ? obj.Produto.Codigo : 0,
                PesoTotal = obj.PesoUnitario * obj.Quantidade,
                ValorUnitario = obj.ValorProduto.ToString("n2"),
                PrecoUnitario = obj.PrecoUnitario.ToString("n3"),
                CodigoProdutoEmbarcadorIntegracao = obj.Produto != null ? obj.Produto.CodigoProdutoEmbarcador : "",
                Descricao = obj.Produto != null ? obj.Produto.Descricao : string.Empty,
                Quantidade = obj.Quantidade.ToString("n3"),
                QuantidadePlanejada = obj.QuantidadePlanejada.ToString("n3"),
                SiglaUnidade = obj.Produto != null ? obj.Produto.SiglaUnidade : string.Empty,
                Peso = obj.PesoUnitario.ToString("n3"),
                QuantidadePalets = obj.QuantidadePalet.ToString("n3"),
                obj.PalletFechado,
                MetrosCubico = obj.MetroCubico.ToString("n6"),
                obj.Observacao,
                obj.QuantidadeUnidadePorCaixa,
                UnidadeMedidaSecundaria = obj.UnidadeMedidaSecundaria ?? "",
                QuantidadeSecundaria = obj.QuantidadeSecundaria.ToString("n3"),
                obj.QuantidadeCaixaPorPallet,
                CodigoLinhaSeparacao = obj?.LinhaSeparacao?.Codigo ?? 0,
                DescricaoLinhaSeparacao = obj?.LinhaSeparacao?.Descricao ?? "",
                CamposPersonalizados = obj.CamposPersonalizados ?? "",
                LinhaSeparacao = new
                {
                    Codigo = obj?.LinhaSeparacao?.Codigo ?? 0,
                    Descricao = obj?.LinhaSeparacao?.Descricao ?? ""
                },
                CodigoEnderecoProduto = obj?.EnderecoProduto?.Codigo ?? 0,
                DescricaoEnderecoProduto = obj?.EnderecoProduto?.Descricao ?? "",
                EnderecoProduto = new
                {
                    Codigo = obj?.EnderecoProduto?.Codigo ?? 0,
                    Descricao = obj?.EnderecoProduto?.Descricao ?? ""
                },
                ApresentarSaldoProduto = retornarSaldoProduto,
                Organizacao = obj?.CodigoOrganizacao ?? "",
                Setor = obj?.Setor ?? "",
                Canal = obj?.Canal ?? "",
                SaldoQuantidade = (!retornarSaldoProduto ? obj.Quantidade : (from saldo in saldoProdutos
                                                                             where saldo.CodigoPedidoProduto == obj.Codigo
                                                                             select saldo.SaldoQtde).FirstOrDefault()).ToString("n3"),
                IdDemanda = obj.IdDemanda ?? string.Empty,
                CodigoFilialEmbarcador = obj.FilialArmazem?.Filial?.CodigoFilialEmbarcador ?? "",
                Armazem = new
                {
                    Codigo = obj.FilialArmazem?.Codigo ?? 0,
                    Descricao = obj.FilialArmazem?.Descricao ?? "",
                    CodigoIntegracao = obj.FilialArmazem?.CodigoIntegracao ?? ""
                }

            }).ToList();
        }

        private dynamic ObterDadosProdutosPedidoPorTipoOperacao(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao)
        {
            var produtos = tipoOperacao?.ProdutosPadroes.Select(obj => new
            {
                AlturaCM = obj.Produto.AlturaCM.ToString("n6"),
                ComprimentoCM = obj.Produto.ComprimentoCM.ToString("n6"),
                LarguraCM = obj.Produto.LarguraCM.ToString("n6"),
                CodigoProdutoEmbarcador = obj.Produto != null ? obj.Produto.Codigo : 0,
                PesoTotal = "0",
                ValorUnitario = "0",
                CodigoProdutoEmbarcadorIntegracao = obj.Produto != null ? obj.Produto.CodigoProdutoEmbarcador : "",
                Descricao = obj.Produto != null ? obj.Produto.Descricao : string.Empty,
                Quantidade = "0",
                QuantidadePlanejada = "0",
                Peso = obj.Produto.PesoUnitario.ToString("n3"),
                QuantidadePalets = "0",
                PalletFechado = "0",
                MetrosCubico = obj.Produto.MetroCubito.ToString("n6"),
                obj.Produto.Observacao,
                QuantidadeUnidadePorCaixa = "0",
                SaldoQuantidade = "0",
                obj.Produto.QuantidadeCaixaPorPallet,
                PrecoUnitario = "0",
                LinhaSeparacao = obj?.Produto?.LinhaSeparacao?.Descricao ?? "",
                CodigoLinhaSeparacao = obj?.Produto?.LinhaSeparacao?.Codigo ?? 0,
                IdDemanda = "",
                Codigo = Guid.NewGuid().ToString().Replace("-", ""),
                CodigoEnderecoProduto = "",
                EnderecoProduto = "",
                Organizacao = "",
                Canal = "",
                Setor = "",
                CodigoFilialEmbarcador = "",
                CodigoIntegracaoArmazem = ""
                //LinhaSeparacao = new
                //{
                //    Codigo = obj.Produto?.LinhaSeparacao?.Codigo ?? 0,
                //    Descricao = obj.Produto?.LinhaSeparacao?.Descricao ?? ""
                //}
            }).ToList();

            if (produtos.Count > 0)
                return produtos;

            return null;
        }

        private dynamic ObterDadosONUProdutosPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, bool duplicar, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoProdutoONU repPedidoProdutoONU = new Repositorio.Embarcador.Pedidos.PedidoProdutoONU(unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoProdutoONU> pedidoProdutosONU = repPedidoProdutoONU.BuscarPorPedido(pedido.Codigo);

            return pedidoProdutosONU.Select(obj => new
            {
                Codigo = duplicar ? 0 : obj.Codigo,
                CodigoProdutoEmbarcador = obj.PedidoProduto.Codigo,
                CodigoClassificacaoRiscoONU = obj.ClassificacaoRiscoONU.Codigo,
                Descricao = obj.ClassificacaoRiscoONU.Descricao,
                Numero = obj.ClassificacaoRiscoONU.NumeroONU,
                ClasseRisco = obj.ClassificacaoRiscoONU.ClasseRisco,
                RiscoSubsidiario = obj.ClassificacaoRiscoONU.RiscoSubsidiario,
                NumeroRisco = obj.ClassificacaoRiscoONU.NumeroRisco,
                Observacao = obj.Observacao
            }).ToList();
        }

        private dynamic ObterDadosPassagensPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, bool duplicar, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoLocaisPrestacaoPassagens repPassagemPercursoEstado = new Repositorio.Embarcador.Pedidos.PedidoLocaisPrestacaoPassagens(unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoLocaisPrestacaoPassagens> passagensPercursoEstado = repPassagemPercursoEstado.BuscarPorPedido(pedido.Codigo);

            return passagensPercursoEstado.Select(obj => new
            {
                Codigo = duplicar ? 0 : obj.Codigo,
                EstadoDePassagem = obj.EstadoDePassagem.Sigla,
                Ordem = obj.Posicao
            }).ToList();
        }

        private dynamic ObterDadosAnexosPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, bool duplicar)
        {
            return pedido.Anexos.Select(o => new
            {
                Codigo = duplicar ? 0 : o.Codigo,
                o.Descricao,
                o.NomeArquivo,
            }).ToList();
        }

        private dynamic ObterDadosNotasParciaisPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            return pedido.PedidoNotasParciais.Select(obj => new
            {
                Codigo = obj.Numero,
                NumeroNFe = obj.Numero
            }).ToList();
        }

        private dynamic ObterDadosNotasFiscaisPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, bool duplicarPedidoParaDevolucaoPacial)
        {

            if (duplicarPedidoParaDevolucaoPacial)
                return new List<object>();

            return pedido.NotasFiscais.Select(obj => new
            {
                Codigo = obj.Codigo,
                obj.Numero,
                obj.Chave,
                Peso = obj.Peso.ToString("n2"),
                Valor = obj.Valor.ToString("n2"),
                DataEmissao = obj.DataEmissao.ToString("dd/MM/yyyy")
            }).ToList();
        }

        private dynamic ObterDadosCTesParciaisPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            return pedido.PedidoCtesParciais.Select(o => new
            {
                o.Codigo,
                o.Numero
            }).ToList();
        }

        private dynamic ObterListaAcrescimoDesconto(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Repositorio.UnitOfWork unitOfWork, bool duplicar)
        {
            Repositorio.Embarcador.Pedidos.PedidoAcrescimoDesconto repPedidoAcrescimoDesconto = new Repositorio.Embarcador.Pedidos.PedidoAcrescimoDesconto(unitOfWork);
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoAcrescimoDesconto> acrescimosDescontos = repPedidoAcrescimoDesconto.BuscarPorPedido(pedido.Codigo);

            return acrescimosDescontos.Select(o => new
            {
                Codigo = !duplicar ? o.Codigo : 0,
                Valor = o.Valor.ToString("n2"),
                o.Observacao,
                Justificativa = new
                {
                    o.Justificativa.Codigo,
                    o.Justificativa.Descricao
                },
                Tipo = o.DescricaoTipoJustificativa,
                Aplicacao = o.DescricaoAplicacaoValor,
            }).ToList();
        }

        private dynamic ObterListaContatos(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Repositorio.UnitOfWork unitOfWork, bool duplicar)
        {
            Repositorio.Embarcador.Pedidos.PedidoContato repPedidoContato = new Repositorio.Embarcador.Pedidos.PedidoContato(unitOfWork);
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoContato> contatos = repPedidoContato.BuscarPorPedido(pedido.Codigo);

            return contatos.Select(o => new
            {
                Codigo = !duplicar ? o.Codigo : 0,
                o.Contato,
                o.Email,
                Situacao = o.Ativo,
                DescricaoSituacao = o.Ativo ? Localization.Resources.Gerais.Geral.Ativo : Localization.Resources.Gerais.Geral.Inativo,
                o.Telefone,
                TipoContato = o.TiposContato.Select(x => x.Codigo).ToList(),
                DescricaoTipoContato = o.TiposContato.Select(x => x.Descricao).ToList(),
                CPF = o.CPF.ObterCpfFormatado(),
            }).ToList();
        }

        private async Task<dynamic> ObterListaFronteirasAsync(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoFronteira repositorioPedidoFronteira = new Repositorio.Embarcador.Pedidos.PedidoFronteira(unitOfWork);

            List<Dominio.Entidades.Cliente> listaFronteiras = await repositorioPedidoFronteira.BuscarFronteirasPorPedidoAsync(pedido.Codigo);

            if (pedido.Fronteira != null)
            {
                if (!listaFronteiras.Contains(pedido.Fronteira))
                {
                    listaFronteiras.Add(pedido.Fronteira);
                }
            }

            return listaFronteiras.Select(o => new
            {
                CPF_CNPJ = o.CPF_CNPJ,
                Descricao = o.Descricao ?? "",
            }).ToList();
        }

        private dynamic ObterDadosAvaliacaoControleEntrega(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Repositorio.UnitOfWork unitOfWork, bool duplicar)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);

            if (duplicar)
                return null;

            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repCargaEntrega.BuscarPorPedido(pedido.Codigo);
            if (cargaEntrega == null || !cargaEntrega.DataAvaliacao.HasValue)
                return null;

            return new
            {
                DataAvaliacao = cargaEntrega.DataAvaliacao.Value.ToDateTimeString(),
                Avaliacao = cargaEntrega.AvaliacaoGeral,
                TipoAvaliacaoGeral = cargaEntrega.AvaliacaoGeral.HasValue,
                Questionario = cargaEntrega.Avaliacoes.OrderBy(o => o.Ordem).Select(o => new { o.Codigo, o.Titulo, o.Conteudo, o.Resposta }).ToList(),
                ObservacaoAvaliacao = (cargaEntrega.ObservacaoAvaliacao ?? string.Empty).Replace("\n", "<br />"),
                MotivoAvaliacao = cargaEntrega.MotivoAvaliacao?.Descricao ?? string.Empty
            };
        }

        private dynamic ObterDadosEcommercePedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoEcommerce repositorioPedidoEcommerce = new Repositorio.Embarcador.Pedidos.PedidoEcommerce(unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.PedidoEcommerce pedidoEcommerce = repositorioPedidoEcommerce.BuscarPorPedido(pedido.Codigo);

            if (pedidoEcommerce == null)
                return null;

            return new
            {
                pedidoEcommerce.AlturaPedido,
                pedidoEcommerce.LarguaPedido,
                pedidoEcommerce.ComprimentoPedido,
                pedidoEcommerce.DiametroPedido,
                pedidoEcommerce.CategoriaPrincipalProduto,
                pedidoEcommerce.SerieNFe,
                pedidoEcommerce.ChaveAcessoNFe,
                pedidoEcommerce.NaturezaGeralMercadorias,
                pedidoEcommerce.TipoGeralMercadorias,
                pedidoEcommerce.PrazoEntregaLoja,
                pedidoEcommerce.TipoFrete,
                pedidoEcommerce.DataPagamentoPedido,
                pedidoEcommerce.ModalidadeEntrega,
                pedidoEcommerce.CodigoTabelaFreteSistemaFIS,
                pedidoEcommerce.CFOPPredominanteNFe
            };
        }

        private dynamic ObterDadosHistoricoPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Auditoria.HistoricoObjeto repositorioHistoricoObjeto = new Repositorio.Auditoria.HistoricoObjeto(unitOfWork);

            List<string> listaPropriedades = new List<string>()
            {
                "Empresa",
                "Destinatario",
                "Remetente",
                "PrevisaoEntrega",
                "DataInicialColeta",
            };

            IList<Dominio.Relatorios.Embarcador.DataSource.Pedidos.PedidoHistorico> historicoObjeto = repositorioHistoricoObjeto.BuscarPropriedadesObjetos(pedido.Codigo, "Pedido", listaPropriedades);

            return historicoObjeto.Select(o => new
            {
                o.Codigo,
                Usuario = o.Usuario,
                DataHora = o.DataHora.ToString("dd/MM/yyyy HH:mm:ss"),
                CampoAlterado = o.Propriedade,
                ValorAnterior = o.ValorAnterior,
                ValorAtual = o.ValorAtual,
            }).ToList();

        }

        private void ValidarRegrasPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Repositorio.UnitOfWork unitOfWork, List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas)
        {
            Repositorio.Embarcador.Veiculos.LicencaVeiculo repLicencaVeiculo = new Repositorio.Embarcador.Veiculos.LicencaVeiculo(unitOfWork);
            Repositorio.Embarcador.Transportadores.MotoristaLicenca repMotoristaLicenca = new Repositorio.Embarcador.Transportadores.MotoristaLicenca(unitOfWork);
            Repositorio.Embarcador.Pessoas.PessoaLicenca repPessoaLicenca = new Repositorio.Embarcador.Pessoas.PessoaLicenca(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repositorioConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = repositorioConfiguracaoPedido.BuscarConfiguracaoPadrao();

            Dominio.Entidades.Cliente tomador = pedido.ObterTomador();
            if (ConfiguracaoEmbarcador.ValidarExistenciaDeConfiguracaoFaturaDoTomador && tomador != null)
            {
                bool contemConfiguracao = tomador.GerarFaturamentoAVista || tomador.GerarTituloPorDocumentoFiscal || tomador.GerarTituloAutomaticamente;
                if (!contemConfiguracao && tomador.DiasSemanaFatura != null && tomador.DiasSemanaFatura.Count > 0)
                    contemConfiguracao = true;
                if (!contemConfiguracao && tomador.DiasMesFatura != null && tomador.DiasMesFatura.Count > 0)
                    contemConfiguracao = true;
                if (!contemConfiguracao && tomador.DiasDePrazoFatura > 0)
                    contemConfiguracao = true;
                if (!contemConfiguracao && tomador.GrupoPessoas != null)
                {
                    contemConfiguracao = tomador.GrupoPessoas.GerarFaturamentoAVista || tomador.GrupoPessoas.GerarTituloPorDocumentoFiscal || tomador.GrupoPessoas.GerarTituloAutomaticamente;
                    if (!contemConfiguracao && tomador.GrupoPessoas.DiasSemanaFatura != null && tomador.GrupoPessoas.DiasSemanaFatura.Count > 0)
                        contemConfiguracao = true;
                    if (!contemConfiguracao && tomador.GrupoPessoas.DiasMesFatura != null && tomador.GrupoPessoas.DiasMesFatura.Count > 0)
                        contemConfiguracao = true;
                    if (!contemConfiguracao && tomador.GrupoPessoas.DiasDePrazoFatura > 0)
                        contemConfiguracao = true;
                }
                if (!contemConfiguracao)
                    throw new ControllerException(string.Format(Localization.Resources.Pedidos.Pedido.TomadorNaoPossuiConfiguracaoDeFatura, tomador.Descricao));
            }

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                bool tracaoProprio = false;
                bool tracaoTerceiro = false;
                bool temReboqueProprio = false;
                bool temReboqueTerceiro = false;

                List<int> codigosVeiculos = new List<int>();
                if (pedido.Veiculos?.Count > 0)
                {
                    codigosVeiculos.AddRange(pedido.Veiculos.Select(o => o.Codigo));
                    foreach (var reboque in pedido?.Veiculos)
                    {
                        if (reboque.VeiculosVinculados?.Count > 0)
                            codigosVeiculos.AddRange(reboque.VeiculosVinculados.Select(o => o.Codigo));
                    }

                    temReboqueProprio = pedido.Veiculos.Where(o => o.Tipo.Equals("P")).Any();
                    temReboqueTerceiro = pedido.Veiculos.Where(o => o.Tipo.Equals("T")).Any();
                }

                if (pedido.VeiculoTracao != null)
                {
                    tracaoProprio = pedido.VeiculoTracao.Tipo.Equals("P");
                    tracaoTerceiro = pedido.VeiculoTracao.Tipo.Equals("T");

                    codigosVeiculos.Add(pedido.VeiculoTracao.Codigo);
                    if (pedido.VeiculoTracao.VeiculosVinculados?.Count > 0)
                        codigosVeiculos.AddRange(pedido.VeiculoTracao.VeiculosVinculados.Select(o => o.Codigo));
                }

                // feito o codigo preservando o funcionamento anterior (com TipoLicenca.Geral) e adicionando as novas validações 
                if (pedido.TipoDeCarga != null && pedido.TipoDeCarga.ValidarLicencasNCM && pedido.TipoDeCarga.TiposLicenca != null && pedido.TipoDeCarga.TiposLicenca.Count > 0)
                {
                    StringBuilder mensagemErroLicenca = new StringBuilder();
                    bool possuiPendenciaLicenca = false;

                    foreach (var licenca in pedido.TipoDeCarga.TiposLicenca)
                    {
                        if (licenca.Tipo == TipoLicenca.Pessoa)
                        {
                            if (pedido.Remetente != null && !repPessoaLicenca.ContemLicencaValida(licenca.Codigo, DateTime.Now, pedido.Remetente.CPF_CNPJ))
                            {
                                possuiPendenciaLicenca = true;
                                mensagemErroLicenca.AppendLine($"O remetente {pedido.Remetente.Descricao} não possui a licença {licenca.Descricao} válida.</br>");
                            }
                            if (pedido.Destinatario != null && !repPessoaLicenca.ContemLicencaValida(licenca.Codigo, DateTime.Now, pedido.Destinatario.CPF_CNPJ))
                            {
                                possuiPendenciaLicenca = true;
                                mensagemErroLicenca.AppendLine($"O destinatário {pedido.Destinatario.Descricao} não possui a licença {licenca.Descricao} válida.</br>");
                            }
                            if (pedido.Recebedor != null && !repPessoaLicenca.ContemLicencaValida(licenca.Codigo, DateTime.Now, pedido.Recebedor.CPF_CNPJ))
                            {
                                possuiPendenciaLicenca = true;
                                mensagemErroLicenca.AppendLine($"O recebedor {pedido.Recebedor.Descricao} não possui a licença {licenca.Descricao} válida.</br>");
                            }
                            if (pedido.Expedidor != null && !repPessoaLicenca.ContemLicencaValida(licenca.Codigo, DateTime.Now, pedido.Expedidor.CPF_CNPJ))
                            {
                                possuiPendenciaLicenca = true;
                                mensagemErroLicenca.AppendLine($"O expedidor {pedido.Expedidor.Descricao} não possui a licença {licenca.Descricao} válida.</br>");
                            }
                            if (pedido.ObterTomador() != null && !repPessoaLicenca.ContemLicencaValida(licenca.Codigo, DateTime.Now, pedido.ObterTomador().CPF_CNPJ))
                            {
                                possuiPendenciaLicenca = true;
                                mensagemErroLicenca.AppendLine($"O tomador {pedido.ObterTomador().Descricao} não possui a licença {licenca.Descricao} válida.</br>");
                            }
                        }

                        if (licenca.Tipo == TipoLicenca.Geral || licenca.Tipo == TipoLicenca.Motorista)
                        {
                            foreach (Dominio.Entidades.Usuario motorista in pedido.Motoristas)
                            {
                                if (!repMotoristaLicenca.ContemLicencaValida(licenca.Codigo, DateTime.Now, motorista.CPF, true))
                                {
                                    possuiPendenciaLicenca = true;
                                    mensagemErroLicenca.AppendLine($"O motorista {motorista.Nome} não possui a licença {licenca.Descricao} válida.</br>");
                                }
                            }
                        }

                        if (licenca.Tipo == TipoLicenca.Geral || licenca.Tipo == TipoLicenca.Veiculo)
                        {
                            if (pedido.VeiculoTracao != null && !repLicencaVeiculo.ContemLicencaValida(licenca.Codigo, DateTime.Now, pedido.VeiculoTracao.Codigo, true))
                            {
                                possuiPendenciaLicenca = true;
                                mensagemErroLicenca.AppendLine($"O veículo {pedido.VeiculoTracao.Placa} não possui a licença {licenca.Descricao} válida.</br>");
                            }

                            if (pedido.Veiculos.Count() > 0)
                            {
                                foreach (var veiculo in pedido.Veiculos)
                                {
                                    if (!repLicencaVeiculo.ContemLicencaValida(licenca.Codigo, DateTime.Now, veiculo.Codigo, true))
                                    {
                                        possuiPendenciaLicenca = true;
                                        mensagemErroLicenca.AppendLine($"O veículo {veiculo.Placa} não possui a licença {licenca.Descricao} válida.</br>");
                                    }
                                }
                            }
                        }

                        if (licenca.Tipo == TipoLicenca.Tracao)
                        {
                            if (pedido.VeiculoTracao.VeiculosVinculados != null
                                && pedido.VeiculoTracao.VeiculosVinculados.Any(veiculo => !repLicencaVeiculo.ContemLicencaValida(licenca.Codigo, DateTime.Now, veiculo.Codigo, true))
                                && pedido.VeiculoTracao.IsTipoVeiculoTracao())
                            {
                                possuiPendenciaLicenca = true;
                                mensagemErroLicenca.AppendLine($"O veículo {pedido.VeiculoTracao.Placa} não possui a licença {licenca.Descricao} válida.</br>");
                            }

                            if (pedido.Veiculos.Count() > 0)
                            {
                                foreach (var veiculo in pedido.Veiculos)
                                {
                                    if (!repLicencaVeiculo.ContemLicencaValida(licenca.Codigo, DateTime.Now, veiculo.Codigo, true))
                                    {
                                        possuiPendenciaLicenca = true;
                                        mensagemErroLicenca.AppendLine($"O veí­culo {veiculo.Placa} não possui a licença {licenca.Descricao} válida.</br>");
                                    }
                                }
                            }
                        }

                        if (licenca.Tipo == TipoLicenca.Reboque)
                        {
                            if (pedido.VeiculoTracao.VeiculosVinculados != null
                                && pedido.VeiculoTracao.VeiculosVinculados.Any(veiculo => !repLicencaVeiculo.ContemLicencaValida(licenca.Codigo, DateTime.Now, veiculo.Codigo, true))
                                && pedido.VeiculoTracao.IsTipoVeiculoReboque())
                            {
                                possuiPendenciaLicenca = true;
                                mensagemErroLicenca.AppendLine($"O veí­culo {pedido.VeiculoTracao.Placa} não possui a licença  {licenca.Descricao}  válida.</br>");
                            }

                            if (pedido.Veiculos.Count() > 0)
                            {
                                foreach (var veiculo in pedido.Veiculos)
                                {
                                    if (!repLicencaVeiculo.ContemLicencaValida(licenca.Codigo, DateTime.Now, veiculo.Codigo, true))
                                    {
                                        possuiPendenciaLicenca = true;
                                        mensagemErroLicenca.AppendLine($"O veí­culo {veiculo.Placa} não possui a licença {licenca.Descricao} válida.</br>");
                                    }
                                }
                            }
                        }
                    }

                    if (possuiPendenciaLicenca)
                    {
                        throw new ServicoException(mensagemErroLicenca.ToString());
                    }
                }
                else
                {
                    codigosVeiculos = codigosVeiculos.Distinct().ToList();
                    Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo licenca = repLicencaVeiculo.BuscarLicencaParaBloqueioPedido(codigosVeiculos);
                    if (licenca != null)
                        if (licenca.Status == StatusLicenca.Vencido)
                        {
                            throw new ControllerException($"O Veículo {licenca.Veiculo.Placa_Formatada} possui a licença {licenca.Descricao} de número {licenca.Numero} com o status {licenca.Status.ObterDescricao() ?? ""}, favor verifique antes de prosseguir com o planejamento do pedido.");
                        }
                        else
                        {
                            throw new ControllerException($"O Veículo {licenca.Veiculo.Placa_Formatada} possui a licença {licenca.Descricao} de número {licenca.Numero} vencida na data {licenca.DataVencimento?.ToString("dd/MM/yyyy") ?? ""}, favor verifique antes de prosseguir com o planejamento do pedido.");
                        }

                    List<int> codigosMotoristas = pedido.Motoristas.Select(o => o.Codigo).ToList();
                    Dominio.Entidades.Embarcador.Transportadores.MotoristaLicenca licencaMotorista = repMotoristaLicenca.BuscarLicencaParaBloqueioPedido(codigosMotoristas);
                    if (licencaMotorista != null)
                        throw new ControllerException($"O Motorista {licencaMotorista.Motorista.Nome} possui a licença {licencaMotorista.Descricao} de número {licencaMotorista.Numero} vencida na data {licencaMotorista.DataVencimento?.ToString("dd/MM/yyyy") ?? ""}, favor verifique antes de prosseguir com o pedido.");

                }

                if (Usuario.LimitarOperacaoPorEmpresa && pedido.Empresa != null && !Usuario.Empresas.Contains(pedido.Empresa))
                    throw new ControllerException(Localization.Resources.Pedidos.Pedido.VoceNaoPossuiPermissaoParaUtilizarEmpresaSelecionada);

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Pedido_PermiteInserirVeiculoProprio) && (tracaoProprio || temReboqueProprio))
                    throw new ControllerException(Localization.Resources.Pedidos.Pedido.VoceNaoPossuiPermissaoParaInserirVeiculoProprio);

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Pedido_PermiteInserirVeiculoTerceiro) && (tracaoTerceiro || temReboqueTerceiro))
                    throw new ControllerException(Localization.Resources.Pedidos.Pedido.VoceNaoPossuiPermissaoParaInserirVeiculoTerceiro);

            }

            if (configuracaoPedido.UtilizarBloqueioPessoasGrupoApenasParaTomadorDoPedido)
            {
                if (pedido.Tomador != null && pedido.Tomador.GrupoPessoas != null && pedido.Tomador.GrupoPessoas.Bloqueado)
                    throw new ControllerException(string.Format(Localization.Resources.Pedidos.Pedido.TomadorBloqueado, pedido.Tomador.GrupoPessoas.MotivoBloqueio));
            }
            else
            {
                if (pedido.GrupoPessoas != null && pedido.GrupoPessoas.Bloqueado)
                    throw new ControllerException(string.Format(Localization.Resources.Pedidos.Pedido.PessoasBloqueadas, pedido.GrupoPessoas.MotivoBloqueio));

                if (pedido.Remetente != null && pedido.Remetente.Bloqueado)
                    throw new ControllerException(string.Format(Localization.Resources.Pedidos.Pedido.RemetenteBloqueado, pedido.Remetente.MotivoBloqueio));

                if (pedido.Destinatario != null && pedido.Destinatario.Bloqueado)
                    throw new ControllerException(string.Format(Localization.Resources.Pedidos.Pedido.DestinatarioBloqueado, pedido.Destinatario.MotivoBloqueio));

                if (pedido.Remetente != null && pedido.Remetente.GrupoPessoas != null && pedido.Remetente.GrupoPessoas.Bloqueado)
                    throw new ControllerException(string.Format(Localization.Resources.Pedidos.Pedido.PessoasRemetenteBloqueado, pedido.Remetente.GrupoPessoas.MotivoBloqueio));

                if (pedido.Destinatario != null && pedido.Destinatario.GrupoPessoas != null && pedido.Destinatario.GrupoPessoas.Bloqueado)
                    throw new ControllerException(string.Format(Localization.Resources.Pedidos.Pedido.PessoasDestinatarioBloqueado, pedido.Destinatario.GrupoPessoas.MotivoBloqueio));
            }

            if (configuracaoPedido.PermitirCriarPedidoApenasMotoristaSituacaoTrabalhando && pedido.Motoristas?.Count > 0 && pedido.Motoristas.Any(o => o.SituacaoColaborador.HasValue && o.SituacaoColaborador != SituacaoColaborador.Trabalhando))
                throw new ControllerException(Localization.Resources.Pedidos.Pedido.MotoristaAdicionadoNaoEstaTrabalhando);

            if (Servicos.Embarcador.Pedido.Pedido.TomadorPossuiPendenciaFinanceira(pedido, TipoServicoMultisoftware, out string mensagemErro))
                throw new ControllerException(mensagemErro);

        }

        private string ObterPlacasPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, List<Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoVeiculo> veiculos)
        {
            string placasPadrao = string.Join(", ", veiculos.Where(p => p.CodigoPedido == pedido.Codigo).Select(p => p.Placa).ToList());

            if (ConfiguracaoEmbarcador.PermitirSelecionarReboquePedido)
            {
                string placaTracao = pedido.VeiculoTracao?.Placa;

                if (string.IsNullOrWhiteSpace(placaTracao))
                    return placasPadrao;
                else if (!string.IsNullOrWhiteSpace(placasPadrao))
                    return placaTracao + ", " + placasPadrao;
                else
                    return placaTracao;
            }

            return placasPadrao;
        }

        private string BuscarPlacasResumo(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            string placas = "";

            if (ConfiguracaoEmbarcador.PermitirSelecionarReboquePedido)
            {
                if (pedido.VeiculoTracao != null)
                {
                    placas = pedido.VeiculoTracao.Placa;

                    if (pedido.VeiculoTracao.ModeloVeicularCarga != null)
                        placas += " (" + pedido.VeiculoTracao.ModeloVeicularCarga.Descricao + ")";

                    placas += ", ";
                }
            }

            foreach (Dominio.Entidades.Veiculo veiculo in pedido.Veiculos)
            {
                placas += veiculo.Placa;

                if (veiculo.ModeloVeicularCarga != null)
                    placas += " (" + veiculo.ModeloVeicularCarga.Descricao + ")";

                placas += ", ";
            }

            if (!string.IsNullOrWhiteSpace(placas))
                placas = placas.Remove(placas.Length - 2, 2);

            return placas;
        }

        private string BuscarPlacas(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            string placas = "";

            if (ConfiguracaoEmbarcador.PermitirSelecionarReboquePedido)
            {
                if (pedido.VeiculoTracao != null)
                {
                    placas = pedido.VeiculoTracao.Placa;

                    if (pedido.VeiculoTracao.ModeloVeicularCarga != null)
                        placas += " (" + pedido.VeiculoTracao.ModeloVeicularCarga.Descricao + ")";

                    if (ConfiguracaoEmbarcador.ConcatenarFrotaPlaca)
                        placas += $" ({pedido.VeiculoTracao.NumeroFrota})";
                }
            }
            else
            {
                Dominio.Entidades.Veiculo ultimoVeiculo = pedido.Veiculos.LastOrDefault();
                foreach (Dominio.Entidades.Veiculo veiculo in pedido.Veiculos)
                {
                    placas += veiculo.Placa;

                    if (veiculo.ModeloVeicularCarga != null)
                        placas += " (" + veiculo.ModeloVeicularCarga.Descricao + ")";

                    if (ultimoVeiculo.Codigo != veiculo.Codigo)
                        placas += ", ";

                }
            }

            return placas;
        }

        private void SetarVeiculosPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);

            int codigoVeiculo = Request.GetIntParam("Veiculo");

            if (pedido.Veiculos == null)
                pedido.Veiculos = new List<Dominio.Entidades.Veiculo>();
            else
                pedido.Veiculos.Clear();

            Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo servicoFilaCarregamentoVeiculo = new Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo(unitOfWork, OrigemAlteracaoFilaCarregamento.Sistema);
            Dominio.Entidades.Veiculo veiculo = codigoVeiculo > 0 ? repVeiculo.BuscarPorCodigo(codigoVeiculo) : null;
            Dominio.Entidades.Veiculo veiculoSubstituido = new Dominio.Entidades.Veiculo();
            List<Dominio.Entidades.Veiculo> veiculosSubstituidos = new List<Dominio.Entidades.Veiculo>();

            bool alterouVeiculo = !((pedido.VeiculoTracao?.Codigo ?? 0) == codigoVeiculo) && !(pedido.Veiculos?.Any(o => (o?.Codigo ?? 0) == codigoVeiculo) ?? true);

            if (alterouVeiculo)
            {
                veiculoSubstituido = pedido.VeiculoTracao;
                veiculosSubstituidos.AddRange(pedido.Veiculos);
            }

            if (ConfiguracaoEmbarcador.PermitirSelecionarReboquePedido)
            {
                List<int> codigosReboques = Request.GetListParam<int>("ListaReboques");

                pedido.VeiculoTracao = veiculo;
                pedido.Veiculos = codigosReboques.Count > 0 ? repVeiculo.BuscarPorCodigo(codigosReboques) : new List<Dominio.Entidades.Veiculo>();
            }
            else
            {
                if (veiculo != null)
                    pedido.Veiculos.Add(veiculo);
            }

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                if (veiculoSubstituido != null)
                    servicoFilaCarregamentoVeiculo.RealocarVeiculoNaFila(veiculoSubstituido.Codigo, TipoServicoMultisoftware);

                if (veiculosSubstituidos.Count > 0)
                {
                    foreach (var veiculoS in veiculosSubstituidos)
                    {
                        servicoFilaCarregamentoVeiculo.RealocarVeiculoNaFila(veiculoS.Codigo, TipoServicoMultisoftware);
                    }

                }
                unitOfWork.Start();
            }
        }

        private void CriarCarga(Repositorio.UnitOfWork unitOfWork, List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            string mensagemErroCriarCarga = Servicos.Embarcador.Pedido.Pedido.CriarCarga(out Dominio.Entidades.Embarcador.Cargas.Carga carga, pedidos, unitOfWork, TipoServicoMultisoftware, Cliente, configuracao, false, true);

            if (!string.IsNullOrWhiteSpace(mensagemErroCriarCarga))
                throw new ControllerException(mensagemErroCriarCarga);

            if (carga == null)
                return;

            foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidos)
            {
                repositorioPedido.Atualizar(pedido);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, pedido, string.Format(Localization.Resources.Cargas.Carga.AdicionouPedidoCarga, pedido.CodigoCargaEmbarcador), unitOfWork);
            }
        }

        private void AdicionarXMLNotasFiscais(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);

            if (pedido.NotasFiscais == null)
                pedido.NotasFiscais = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();

            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais = repXMLNotaFiscal.BuscarPorNumeroControlePedido(pedido.NumeroControle);
            foreach (var nota in notasFiscais)
                if (!pedido.NotasFiscais.Contains(nota)) pedido.NotasFiscais.Add(nota);

            repositorioPedido.Atualizar(pedido);
        }

        private void AdicionarXMLNotasFiscaisViculadasNoPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Repositorio.UnitOfWork unitOfWork)
        {

            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);

            dynamic notasFiscais = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("NotasFiscais"));

            if (notasFiscais.Count > 0)
            {
                if (pedido.NotasFiscais == null)
                    pedido.NotasFiscais = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();

                foreach (var notaFiscal in notasFiscais)
                {
                    int codigoNota = ((string)notaFiscal.Codigo).ToInt();

                    Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = repXMLNotaFiscal.BuscarPorCodigo(codigoNota);

                    if (!pedido.NotasFiscais.Contains(xmlNotaFiscal))
                        pedido.NotasFiscais.Add(xmlNotaFiscal);

                    repositorioPedido.Atualizar(pedido);
                }
            }

        }

        private void EnviarEmailRestricoesDescarga(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            if ((pedido.Destinatario?.ClienteDescargas?.Count > 0) && (pedido.Destinatario.ClienteDescargas.FirstOrDefault().RestricoesDescarga.Count > 0))
            {
                pedido.RestricoesDescarga = pedido.Destinatario.ClienteDescargas.FirstOrDefault().RestricoesDescarga.ToList();

                foreach (var restricao in pedido.RestricoesDescarga)
                {
                    if (!string.IsNullOrWhiteSpace(restricao.Email))
                    {
                        Servicos.Embarcador.Pedido.Pedido servicoPedido = new Servicos.Embarcador.Pedido.Pedido(unitOfWork);
                        List<string> emails = restricao.Email.Split(';').ToList();

                        if (!servicoPedido.EnviarRelatorioDetalhesPedidoPorEmail(emails, pedido, restricao, unitOfWork, out string mensagem))
                            Servicos.Log.TratarErro(mensagem, "EmailRestricao");
                    }
                }
            }
        }

        private bool IsGerarCargaAutomaticamente(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            return (pedido.PedidoIntegradoEmbarcador && !pedido.ColetaEmProdutorRural && !pedido.Cotacao);
        }

        private string ObterDescricaoTipoMultiplasEntidades(List<double> listaCpfCnpjDestinatario, List<double> listaCpfCnpjRemetente)
        {
            if ((listaCpfCnpjDestinatario.Count > 1) && (listaCpfCnpjRemetente.Count > 1))
                return Localization.Resources.Pedidos.Pedido.DestinatariosRemetentes;

            if (listaCpfCnpjDestinatario.Count > 1)
                return Localization.Resources.Pedidos.Pedido.Destinatarios;

            if (listaCpfCnpjRemetente.Count > 1)
                return Localization.Resources.Gerais.Geral.Remetente;

            return string.Empty;
        }

        private List<int> ObterListaModelosVeiculares()
        {
            List<int> listaModelosVeiculares = Request.GetListParam<int>("ModelosVeicularesCarga");

            if (listaModelosVeiculares.Count == 0)
                listaModelosVeiculares.Add(0);

            return listaModelosVeiculares;
        }

        private List<double> ObterListaCpfCnpjDestinatario()
        {
            double cpfCnpjDestinatario = Request.GetDoubleParam("Destinatario");
            List<double> listaCpfCnpjDestinatario = cpfCnpjDestinatario > 0d ? new List<double>() { cpfCnpjDestinatario } : Request.GetListParam<double>("Destinatarios");

            if (listaCpfCnpjDestinatario.Count == 0)
                listaCpfCnpjDestinatario.Add(0d);

            return listaCpfCnpjDestinatario;
        }

        private List<double> ObterListaCpfCnpjRemetente()
        {
            double cpfCnpjRemetente = Request.GetDoubleParam("Remetente");
            List<double> listaCpfCnpjRemetente = cpfCnpjRemetente > 0d ? new List<double>() { cpfCnpjRemetente } : Request.GetListParam<double>("Remetentes");

            if (listaCpfCnpjRemetente.Count == 0)
                listaCpfCnpjRemetente.Add(0d);

            return listaCpfCnpjRemetente;
        }

        private dynamic ObterListaCotacoesPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.CotacaoPedido.CotacaoPedido repositorioCotacaoPedido = new Repositorio.Embarcador.CotacaoPedido.CotacaoPedido(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repositorioConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork);

            List<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido> cotacoesPedido = repositorioCotacaoPedido.BuscarPorCodigoPedido(pedido.Codigo);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = repositorioConfiguracaoPedido.BuscarConfiguracaoPadrao();

            return cotacoesPedido.Select(o => new
            {
                Codigo = o.Codigo,
                IDOferta = configuracaoPedido.HabilitarBIDTransportePedido ? o.CodigoIntegracao : o.Codigo.ToString(),
                Transportador = o.Empresa.Descricao,
                Servico = o.TipoOperacao?.Descricao ?? "",
                PrecoFrete = o.ValorCotacao.ToString("n2"),
                ValorFrete = o.ValorFrete.ToString("n2"),
                Prazo = o.LeadTimeEntrega.ToString(),
                Validade = o.DataCriacao.HasValue ? o.DataCriacao.Value.AddDays(10).ToString("dd/MM/yyyy HH:mm") : "",
                DataColetaPrevista = o.DataColetaPrevista.HasValue ? o.DataColetaPrevista.Value.ToString("dd/MM/yyyy") : "",
                DataPrazoEntrega = o.Previsao.HasValue ? o.Previsao.Value.ToString("dd/MM/yyyy") : "",
                KMTotal = o.KMTotal.ToString("n0"),
                ValorFreteTonelada = o.ValorFreteTonelada.ToString("n2"),
                AplicarICMS = o.IncluirValorICMSBaseCalculo != null ? (o.IncluirValorICMSBaseCalculo ? "Sim" : "Não") : string.Empty,
                CanalVenda = o.Pedido?.CanalVenda?.Descricao ?? string.Empty,
            }).ToList();
        }

        private void PreencherCodigoCargaEmbarcador(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            if (IsGerarCargaAutomaticamente(pedido) && pedido.GerarAutomaticamenteCargaDoPedido)
            {
                if (!ConfiguracaoEmbarcador.NumeroCargaSequencialUnico)
                    pedido.CodigoCargaEmbarcador = Servicos.Embarcador.Cargas.CargaSequencial.GetInstance().ObterProximoNumeroSequencial(unitOfWork, pedido.Filial?.Codigo ?? 0).ToString();
                else
                    pedido.CodigoCargaEmbarcador = Servicos.Embarcador.Cargas.CargaSequencial.GetInstance().ObterProximoNumeroSequencial(unitOfWork).ToString();

                pedido.AdicionadaManualmente = true;
            }
        }

        private void PreencherDadosDestinatario(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, double cpfCnpjDestinatario, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido)
        {
            Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco pedidoEnderecoDestino = new Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco();
            Servicos.Embarcador.Pedido.Pedido servicoPedido = new Servicos.Embarcador.Pedido.Pedido(unitOfWork);

            if (cpfCnpjDestinatario > 0d)
            {
                Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);
                pedido.Destinatario = repositorioCliente.BuscarPorCPFCNPJ(cpfCnpjDestinatario);
                if (pedido?.Destinatario?.GerarPedidoBloqueado ?? false)
                    pedido.PedidoBloqueado = true;
                servicoPedido.PreecherEnderecoPedido(ref pedidoEnderecoDestino, pedido.Destinatario);
            }
            else
                pedido.DestinatarioNaoInformado = true;

            if (pedido.Recebedor != null && configuracaoPedido.UtilizarEnderecoExpedidorRecebedorPedido)
                servicoPedido.PreecherEnderecoPedido(ref pedidoEnderecoDestino, pedido.Recebedor);

            if (pedido.UsarOutroEnderecoDestino)
            {
                int codigoLocalidadeClienteDestino = Request.GetIntParam("LocalidadeClienteDestino");

                if (codigoLocalidadeClienteDestino > 0)
                {
                    Repositorio.Embarcador.Pessoas.ClienteOutroEndereco repClienteOutroEndereco = new Repositorio.Embarcador.Pessoas.ClienteOutroEndereco(unitOfWork);
                    pedidoEnderecoDestino.ClienteOutroEndereco = repClienteOutroEndereco.BuscarPorCodigo(codigoLocalidadeClienteDestino);

                    PreecherOutroEnderecoPedido(ref pedidoEnderecoDestino);
                }
                else
                {
                    Repositorio.Localidade repositorioLocalidade = new Repositorio.Localidade(unitOfWork);

                    pedidoEnderecoDestino.Bairro = Request.GetStringParam("BairroDestino");
                    pedidoEnderecoDestino.CEP = Request.GetStringParam("CEPDestino");
                    pedidoEnderecoDestino.Localidade = repositorioLocalidade.BuscarPorCodigo(Request.GetIntParam("Destino"));
                    pedidoEnderecoDestino.Complemento = Request.GetStringParam("ComplementoDestino");
                    pedidoEnderecoDestino.Endereco = Request.GetStringParam("EnderecoDestino");
                    pedidoEnderecoDestino.Numero = Request.GetStringParam("NumeroDestino");
                    pedidoEnderecoDestino.Telefone = Request.GetStringParam("Telefone1Destino");
                    pedidoEnderecoDestino.IE_RG = Request.GetStringParam("RGIE1Destino");
                }
            }

            if (pedidoEnderecoDestino.Localidade != null)
            {
                Repositorio.Embarcador.Pedidos.PedidoEndereco repositorioPedidoEndereco = new Repositorio.Embarcador.Pedidos.PedidoEndereco(unitOfWork);
                repositorioPedidoEndereco.Inserir(pedidoEnderecoDestino);

                pedido.Destino = pedidoEnderecoDestino.Localidade;
                pedido.EnderecoDestino = pedidoEnderecoDestino;
            }
        }

        private void PreencherDadosRemetente(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, double cpfCnpjRemetente, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido)
        {
            Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco pedidoEnderecoOrigem = new Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco();
            Servicos.Embarcador.Pedido.Pedido servicoPedido = new Servicos.Embarcador.Pedido.Pedido(unitOfWork);

            if (cpfCnpjRemetente > 0d)
            {
                Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);
                pedido.Remetente = repositorioCliente.BuscarPorCPFCNPJ(cpfCnpjRemetente);
                servicoPedido.PreecherEnderecoPedido(ref pedidoEnderecoOrigem, pedido.Remetente);
            }

            if (pedido.Expedidor != null && configuracaoPedido.UtilizarEnderecoExpedidorRecebedorPedido)
                servicoPedido.PreecherEnderecoPedido(ref pedidoEnderecoOrigem, pedido.Expedidor);

            if (pedido.UsarOutroEnderecoOrigem)
            {
                int codigoLocalidadeClienteOrigem = Request.GetIntParam("LocalidadeClienteOrigem");

                if (codigoLocalidadeClienteOrigem > 0)
                {
                    Repositorio.Embarcador.Pessoas.ClienteOutroEndereco repositorioClienteOutroEndereco = new Repositorio.Embarcador.Pessoas.ClienteOutroEndereco(unitOfWork);
                    pedidoEnderecoOrigem.ClienteOutroEndereco = repositorioClienteOutroEndereco.BuscarPorCodigo(codigoLocalidadeClienteOrigem);

                    PreecherOutroEnderecoPedido(ref pedidoEnderecoOrigem);
                }
                else
                {
                    Repositorio.Localidade repositorioLocalidade = new Repositorio.Localidade(unitOfWork);

                    pedidoEnderecoOrigem.Bairro = Request.Params("BairroOrigem");
                    pedidoEnderecoOrigem.CEP = Request.Params("CEPOrigem");
                    pedidoEnderecoOrigem.Localidade = repositorioLocalidade.BuscarPorCodigo(Request.GetIntParam("Origem"));
                    pedidoEnderecoOrigem.Complemento = Request.Params("ComplementoOrigem");
                    pedidoEnderecoOrigem.Endereco = Request.Params("EnderecoOrigem");
                    pedidoEnderecoOrigem.Numero = Request.Params("NumeroOrigem");
                    pedidoEnderecoOrigem.Telefone = Request.Params("Telefone1Origem");
                    pedidoEnderecoOrigem.IE_RG = Request.Params("RGIE1Origem");
                }
            }

            if (pedidoEnderecoOrigem.Localidade != null)
            {
                Repositorio.Embarcador.Pedidos.PedidoEndereco repositorioPedidoEndereco = new Repositorio.Embarcador.Pedidos.PedidoEndereco(unitOfWork);
                repositorioPedidoEndereco.Inserir(pedidoEnderecoOrigem);

                pedido.Origem = pedidoEnderecoOrigem.Localidade;
                pedido.EnderecoOrigem = pedidoEnderecoOrigem;
            }
        }

        private void PreencherDadosRotaFrete(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            Repositorio.RotaFrete repositorioRotaFrete = new Repositorio.RotaFrete(unitOfWork);
            int codigoRotaFrete = Request.GetIntParam("Rota");

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && codigoRotaFrete > 0)
                pedido.RotaFrete = repositorioRotaFrete.BuscarPorCodigo(codigoRotaFrete);
            else if (pedido.Destino != null && pedido.Origem != null)
            {
                if (codigoRotaFrete > 0)
                    pedido.RotaFrete = repositorioRotaFrete.BuscarPorCodigo(codigoRotaFrete);

                if (pedido.RotaFrete == null)
                    pedido.RotaFrete = repositorioRotaFrete.BuscarPorOrigemDestino(pedido.Origem, pedido.Destino, true);

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    if (pedido.RotaFrete == null)
                        pedido.RotaFrete = repositorioRotaFrete.BuscarPorEstado(pedido.Destino.Estado.Sigla, true);

                    if ((pedido.RotaFrete != null) && (pedido.RotaFrete.Distribuidor != null))
                    {
                        if (pedido.Expedidor == null)
                            pedido.Expedidor = pedido.RotaFrete.Distribuidor;

                        if (pedido.Recebedor == null)
                            pedido.Recebedor = pedido.RotaFrete.Distribuidor;
                    }
                }
            }
        }

        private void PreencherDadosRotaFreteClienteDeslocamento(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            Dominio.Entidades.RotaFrete rotaFreteClienteDeslocamento = ObterRotaFreteClienteDeslocamento(unitOfWork, pedido);

            if (rotaFreteClienteDeslocamento != null)
            {
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargaPedido = repositorioCargaPedido.BuscarPorPedido(pedido.Codigo);

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in listaCargaPedido)
                {
                    cargaPedido.Carga.RotaClienteDeslocamento = rotaFreteClienteDeslocamento;
                    cargaPedido.Carga.DeslocamentoQuilometros = rotaFreteClienteDeslocamento.Quilometros;

                    repositorioCarga.Atualizar(cargaPedido.Carga);
                }
            }
        }

        private Dominio.Entidades.Embarcador.Pedidos.Pedido ObterPedidoBase(Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Pedido.Pedido servicoPedido = new Servicos.Embarcador.Pedido.Pedido(unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = new Dominio.Entidades.Embarcador.Pedidos.Pedido();

            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoTipoPagamento repPedidoTipoPagamento = new Repositorio.Embarcador.Pedidos.PedidoTipoPagamento(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoEndereco repPedidoEndereco = new Repositorio.Embarcador.Pedidos.PedidoEndereco(unitOfWork);
            Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unitOfWork);
            Repositorio.Embarcador.Logistica.CentroCarregamento repCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
            Repositorio.Embarcador.Pedidos.Porto repPorto = new Repositorio.Embarcador.Pedidos.Porto(unitOfWork);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
            Repositorio.Embarcador.Localidades.Regiao repRegiao = new Repositorio.Embarcador.Localidades.Regiao(unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repConfiguracaoCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);
            Repositorio.Embarcador.Escrituracao.ContratoFreteCliente repContratoFreteCliente = new Repositorio.Embarcador.Escrituracao.ContratoFreteCliente(unitOfWork);
            Repositorio.Embarcador.Pedidos.ContainerTipo repContainerTipo = new Repositorio.Embarcador.Pedidos.ContainerTipo(unitOfWork);
            Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica = ObterOperadorLogistica(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repConfiguracaoCarga.BuscarPrimeiroRegistro();

            long codigoPedidoTipoPagamento = Request.GetLongParam("PedidoTipoPagamento");
            List<int> listaModelosVeiculares = ObterListaModelosVeiculares();

            if (ConfiguracaoEmbarcador.UtilizarLocalidadePrestacaoPedido)
            {
                int codigoLocalidadeInicioPrestacao = Request.GetIntParam("LocalidadeInicioPrestacao");
                int codigoLocalidadeTerminoPrestacao = Request.GetIntParam("LocalidadeTerminoPrestacao");

                pedido.LocalidadeInicioPrestacao = codigoLocalidadeInicioPrestacao > 0 ? repLocalidade.BuscarPorCodigo(codigoLocalidadeInicioPrestacao) : null;
                pedido.LocalidadeTerminoPrestacao = codigoLocalidadeTerminoPrestacao > 0 ? repLocalidade.BuscarPorCodigo(codigoLocalidadeTerminoPrestacao) : null;
            }

            pedido.ProdutoPredominante = Request.GetStringParam("ProdutoPredominante");
            pedido.DataAgendamento = Request.GetNullableDateTimeParam("DataAgendamento");
            pedido.DataCarregamentoCarga = Request.GetNullableDateTimeParam("DataCarregamento");
            pedido.PossuiCarga = Request.GetBoolParam("PossuiCarregamento");
            pedido.ValorCarga = Request.GetNullableDecimalParam("ValorCarregamento");
            pedido.PossuiDescarga = Request.GetBoolParam("PossuiDescarga");
            pedido.ValorDescarga = Request.GetNullableDecimalParam("ValorDescarga");
            pedido.PossuiDeslocamento = Request.GetBoolParam("PossuiDeslocamento");
            pedido.ValorDeslocamento = Request.GetNullableDecimalParam("ValorDeslocamento");
            pedido.PossuiDiaria = Request.GetBoolParam("PossuiDiaria");
            pedido.ValorDiaria = Request.GetNullableDecimalParam("ValorDiaria");
            pedido.QuantidadeNotasFiscais = Request.GetIntParam("QuantidadeNotasFiscais");
            pedido.ObservacaoInterna = Request.GetNullableStringParam("ObservacaoInterna");
            pedido.DeclaracaoObservacaoCRT = Request.GetNullableStringParam("DeclaracaoObservacaoCRT");

            pedido.ObservacaoEmissaRemetente = Request.GetNullableStringParam("ObservacaoEmissaRemetente");
            pedido.ObservacaoEmissaoDestinatario = Request.GetNullableStringParam("ObservacaoEmissaoDestinatario");

            pedido.DataPrevisaoSaida = Request.GetNullableDateTimeParam("DataPrevisaoSaida");
            pedido.PrevisaoEntrega = Request.GetNullableDateTimeParam("PrevisaoEntrega") ?? Request.GetNullableDateTimeParam("DataPrevisaoEntrega");
            pedido.DataFinalViagemFaturada = Request.GetNullableDateTimeParam("DataFinalViagemFaturada") ?? pedido.PrevisaoEntrega;
            pedido.DataInicialViagemFaturada = Request.GetNullableDateTimeParam("DataInicialViagemFaturada") ?? pedido.DataPrevisaoSaida;

            if (pedido.DataPrevisaoSaida.HasValue && pedido.PrevisaoEntrega.HasValue && (pedido.DataPrevisaoSaida.Value > pedido.PrevisaoEntrega.Value))
                throw new ControllerException(Localization.Resources.Pedidos.Pedido.DataPrevisaoSaidaEstaMaiorQueDataPrevisaoRetorno);

            if (pedido.DataFinalViagemFaturada < pedido.DataInicialViagemFaturada)
                throw new ControllerException(Localization.Resources.Pedidos.Pedido.DataInicialViajemFaturadaEMaiorQueDataFinalViajemFaturada);

            pedido.Distancia = Request.GetDecimalParam("Distancia");
            pedido.PedidoTipoPagamento = codigoPedidoTipoPagamento > 0L ? repPedidoTipoPagamento.BuscarPorCodigo(codigoPedidoTipoPagamento, false) : null;
            pedido.AdicionadaManualmente = true;
            pedido.Ajudante = Request.GetBoolParam("Ajudante");
            pedido.ArmadorImportacao = Request.GetStringParam("ArmadorImportacao");
            pedido.BairroEntregaImportacao = Request.GetStringParam("BairroEntregaImportacao");
            pedido.CEPEntregaImportacao = Request.GetStringParam("CEPEntregaImportacao");
            pedido.CodigoPedidoCliente = Request.GetStringParam("CodigoPedidoCliente");
            pedido.Cotacao = Request.GetBoolParam("Cotacao");
            pedido.CubagemTotal = Request.GetDecimalParam("CubagemTotal");
            pedido.DataFinalColeta = Request.GetNullableDateTimeParam("DataFinalColeta");

            if (ConfiguracaoEmbarcador.InformarDataViagemExecutadaPedido)
            {
                pedido.DataInicialViagemExecutada = Request.GetNullableDateTimeParam("DataInicialViagemExecutada");
                pedido.DataFinalViagemExecutada = Request.GetNullableDateTimeParam("DataFinalViagemExecutada");

                if (!pedido.DataInicialViagemExecutada.HasValue)
                    pedido.DataInicialViagemExecutada = pedido.DataPrevisaoSaida;

                if (!pedido.DataFinalViagemExecutada.HasValue)
                    pedido.DataFinalViagemExecutada = pedido.PrevisaoEntrega;
            }
            else
            {
                pedido.DataInicialViagemExecutada = pedido.DataPrevisaoSaida;
                pedido.DataFinalViagemExecutada = pedido.PrevisaoEntrega;
            }

            pedido.DataValidade = Request.GetNullableDateTimeParam("DataValidade");
            pedido.DataInicioJanelaDescarga = Request.GetNullableDateTimeParam("DataInicioJanelaDescarga");
            pedido.DataInicialColeta = Request.GetNullableDateTimeParam("DataInicialColeta");
            pedido.DataVencimentoArmazenamentoImportacao = Request.GetNullableDateTimeParam("DataVencimentoArmazenamentoImportacao");
            pedido.DespachoTransitoAduaneiro = Request.GetBoolParam("DespachoTransitoAduaneiro");
            pedido.NumeroDTA = Request.GetStringParam("NumeroDTA");
            pedido.EnderecoEntregaImportacao = Request.GetStringParam("EnderecoEntregaImportacao");
            pedido.EscoltaArmada = Request.GetBoolParam("Escolta");
            pedido.QtdEscolta = Request.GetIntParam("QtdEscolta");
            pedido.GerarAutomaticamenteCargaDoPedido = (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador) ? Request.GetBoolParam("GerarAutomaticamenteCargaDoPedido") : true;
            pedido.GerenciamentoRisco = Request.GetBoolParam("GerenciamentoRisco");
            pedido.ImprimirObservacaoCTe = Request.GetBoolParam("ImprimirObservacaoCTe");
            pedido.LacreContainerDois = Request.GetStringParam("LacreContainerDois");
            pedido.LacreContainerTres = Request.GetStringParam("LacreContainerTres");
            pedido.LacreContainerUm = Request.GetStringParam("LacreContainerUm");
            pedido.ContainerTipoReservaFluxoContainer = repContainerTipo.BuscarPorCodigo(Request.GetIntParam("ContainerTipoReservaFluxoContainer"));
            pedido.NecessarioReentrega = Request.GetBoolParam("NecessarioReentrega");
            pedido.EntregaAgendada = Request.GetBoolParam("EntregaAgendada");
            pedido.QuebraMultiplosCarregamentos = Request.GetBoolParam("QuebraMultiplosCarregamentos");
            pedido.NumeroBL = Request.GetStringParam("NumeroBL");
            pedido.NumeroContainer = Request.GetStringParam("NumeroContainer");
            pedido.NumeroControle = Request.GetStringParam("NumeroControle");
            pedido.PesoLiquidoTotal = Request.GetDecimalParam("PesoLiquidoTotal");
            pedido.PesoTotalPaletes = Request.GetDecimalParam("PesoTotalPaletes");
            pedido.PossuiCargaPerigosa = Request.GetBoolParam("PossuiCargaPerigosaDescricao");
            pedido.NumeroRastreioCorreios = Request.GetStringParam("NumeroRastreioCorreios");
            pedido.ContemCargaRefrigerada = Request.GetBoolParam("ContemCargaRefrigeradaDescricao");
            pedido.NumeroNavio = Request.GetStringParam("NumeroNavio");
            pedido.NumeroPaletes = Request.GetIntParam("Pallets");
            pedido.NumeroPaletesFracionado = Request.GetDecimalParam("PalletsFracionado");
            pedido.PalletSaldoRestante = pedido.TotalPallets;
            pedido.NumeroPaletesPagos = Request.GetDecimalParam("NumeroPaletesPagos");
            pedido.NumeroSemiPaletes = Request.GetDecimalParam("NumeroSemiPaletes");
            pedido.NumeroSemiPaletesPagos = Request.GetDecimalParam("NumeroSemiPaletesPagos");
            pedido.NumeroCombis = Request.GetDecimalParam("NumeroCombis");
            pedido.NumeroCombisPagas = Request.GetDecimalParam("NumeroCombisPagas");
            pedido.Observacao = Request.GetStringParam("Observacao");
            string observacaoCte = (operadorLogistica?.TelaPedidosResumido ?? false) ? Request.GetNullableStringParam("ObservacaoAbaPedido") : Request.GetNullableStringParam("ObservacaoCTe");
            pedido.ObservacaoCTe = observacaoCte ?? ConfiguracaoEmbarcador.ObservacaoCTePadraoEmbarcador;
            pedido.PedidoIntegradoEmbarcador = !ConfiguracaoEmbarcador.UtilizarIntegracaoPedido;
            pedido.PedidoSubContratado = Request.GetBoolParam("PedidoSubContratado");
            pedido.PedidoTransbordo = Request.GetBoolParam("PedidoTransbordo");
            pedido.DisponibilizarPedidoParaColeta = Request.GetBoolParam("DisponibilizarPedidoParaColeta");
            pedido.QtdAjudantes = Request.GetIntParam("QtdAjudantes");
            pedido.Rastreado = Request.GetBoolParam("Rastreado");
            pedido.Requisitante = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.RequisitanteColeta>("Requisitante");
            pedido.Seguro = Request.GetBoolParam("Seguro");
            pedido.SenhaAgendamento = Request.GetStringParam("SenhaAgendamento");
            pedido.SenhaAgendamentoCliente = Request.GetStringParam("SenhaAgendamentoCliente");
            pedido.SituacaoPedido = Request.GetBoolParam("ViagemJaOcorreu") ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Finalizado : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Aberto;
            pedido.TaraContainer = Request.GetStringParam("TaraContainer");
            pedido.QtVolumes = Request.GetIntParam("QtVolumes");
            pedido.SaldoVolumesRestante = Request.GetIntParam("QtVolumes");
            pedido.DataOrder = Request.GetNullableDateTimeParam("DataOrder");
            pedido.DataChip = Request.GetNullableDateTimeParam("DataChip");
            pedido.DataCancel = Request.GetNullableDateTimeParam("DataCancel");
            pedido.Temperatura = Request.GetStringParam("Temperatura");
            pedido.TipoPagamento = Request.GetEnumParam<Dominio.Enumeradores.TipoPagamento>("TipoPagamento");
            pedido.TipoPessoa = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa>("TipoPessoa");
            pedido.UltimaAtualizacao = DateTime.Now;
            pedido.DataCadastro = DateTime.Now;
            pedido.UsarOutroEnderecoDestino = Request.GetBoolParam("UsarOutroEnderecoDestino");
            pedido.UsarOutroEnderecoOrigem = Request.GetBoolParam("UsarOutroEnderecoOrigem");
            pedido.Usuario = this.Usuario;
            pedido.Autor = this.Usuario;
            pedido.ValorFreteNegociado = Request.GetDecimalParam("ValorFreteNegociado");
            pedido.ValorFreteTransportadorTerceiro = Request.GetDecimalParam("ValorFreteTransportadorTerceiro");
            pedido.ValorFreteToneladaNegociado = Request.GetDecimalParam("ValorFreteToneladaNegociado");
            pedido.ValorFreteToneladaTerceiro = Request.GetDecimalParam("ValorFreteToneladaTerceiro");
            pedido.ValorTotalCarga = Request.GetDecimalParam("ValorTotalCarga");
            pedido.ValorTotalNotasFiscais = Request.GetDecimalParam("ValorTotalNotasFiscais");
            pedido.ValorPedagioRota = Request.GetDecimalParam("ValorPedagioRota");
            pedido.DataUltimaLiberacao = Request.GetNullableDateTimeParam("DataUltimaLiberacao");
            pedido.RotaEmbarcador = Request.GetStringParam("RotaEmbarcador");
            pedido.UsuarioCriacaoRemessa = Request.GetStringParam("UsuarioCriacaoRemessa");
            pedido.NumeroOrdem = Request.GetStringParam("NumeroOrdem");
            pedido.RegiaoDestino = repRegiao.BuscarPorCodigo(Request.GetIntParam("RegiaoDestino"));
            pedido.QuantidadeVolumesPrevios = Request.GetIntParam("QuantidadeVolumesPrevios");

            pedido.DataAlocacaoPedido = Request.GetNullableDateTimeParam("DataAlocacaoPedido");
            pedido.GrossSales = Request.GetDecimalParam("ValorGross");
            pedido.PossuiIsca = Request.GetBoolParam("PossuiIsca");
            pedido.Substituicao = Request.GetNullableBoolParam("Substituicao");

            DateTime dataBaseCRT;
            DateTime.TryParseExact(Request.Params("DataBaseCRT"), "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out dataBaseCRT);
            if (dataBaseCRT != DateTime.MinValue)
                pedido.DataBaseCRT = dataBaseCRT;
            else
                pedido.DataBaseCRT = null;

            DateTime? dataColeta = Request.GetNullableDateTimeParam("DataColeta");

            if (dataColeta.HasValue)
            {
                pedido.DataCarregamentoPedido = dataColeta;
                pedido.DataInicialColeta = dataColeta;
            }
            else
                pedido.DataCarregamentoPedido = pedido.DataInicialColeta;

            pedido.QtdEntregas = Request.GetIntParam("QtdEntregas");

            if (pedido.QtdEntregas == 0)
                pedido.QtdEntregas = 1;

            decimal pesoTotalCarga = Request.GetDecimalParam("PesoTotalCarga");

            pedido.PesoTotal = pesoTotalCarga;
            pedido.PesoSaldoRestante = pesoTotalCarga;

            decimal cubagemTotalTMS = Request.GetDecimalParam("CubagemTotalTMS");

            if (cubagemTotalTMS > 0)
                pedido.CubagemTotal = cubagemTotalTMS;

            int codigoFilial = Request.GetIntParam("Filial");
            if (codigoFilial > 0)
            {
                Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                pedido.Filial = repositorioFilial.BuscarPorCodigo(codigoFilial);
            }

            int.TryParse(Request.Params("GrupoPessoa"), out int grupoPessoa);
            if (grupoPessoa > 0)
            {
                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
                pedido.GrupoPessoas = repGrupoPessoas.BuscarPorCodigo(grupoPessoa);
            }

            int.TryParse(Request.Params("NumeroContratoFreteCliente"), out int codigoContratoFreteCliente);
            if (codigoContratoFreteCliente > 0)
                pedido.ContratoFreteCliente = repContratoFreteCliente.BuscarPorCodigo(codigoContratoFreteCliente, false);

            int.TryParse(Request.Params("Origem"), out int origem);
            if (origem > 0)
                pedido.Origem = repLocalidade.BuscarPorCodigo(origem);

            int destino = Request.GetIntParam("Destino", 0);
            if (destino > 0)
                pedido.Destino = repLocalidade.BuscarPorCodigo(destino);

            SetarVeiculosPedido(pedido, unitOfWork);

            double expedidor = 0;
            double.TryParse(Request.Params("Expedidor"), out expedidor);
            if (expedidor > 0)
                pedido.Expedidor = repCliente.BuscarPorCPFCNPJ(expedidor);

            Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco pedidoEnderecoDestino = new Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco();
            double recebedor = Request.GetDoubleParam("Recebedor");

            if (recebedor > 0)
            {
                pedido.Recebedor = repCliente.BuscarPorCPFCNPJ(recebedor);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    servicoPedido.PreecherEnderecoPedido(ref pedidoEnderecoDestino, pedido.Recebedor);
            }
            else
                pedido.Recebedor = null;

            if (pedidoEnderecoDestino.Localidade != null)
                repPedidoEndereco.Inserir(pedidoEnderecoDestino);

            if (pedidoEnderecoDestino.Localidade != null)
            {
                pedido.Destino = pedidoEnderecoDestino.Localidade;
                pedido.EnderecoDestino = pedidoEnderecoDestino;
            }

            if (pedido.DisponibilizarPedidoParaColeta)
                pedido.RecebedorColeta = repCliente.BuscarPorCPFCNPJ(Request.GetDoubleParam("RecebedorColeta"));


            int serie = 0;
            int.TryParse(Request.Params("SerieCTe"), out serie);
            if (serie > 0)
            {
                pedido.EmpresaSerie = new Dominio.Entidades.EmpresaSerie() { Codigo = serie };
            }

            int empresa = 0;
            int.TryParse(Request.Params("Empresa"), out empresa);
            if (empresa > 0)
            {
                pedido.Empresa = repEmpresa.BuscarPorCodigo(empresa);
            }

            int tipoCarga = 0;
            int.TryParse(Request.Params("TipoCarga"), out tipoCarga);
            if (tipoCarga > 0)
            {
                Repositorio.Embarcador.Cargas.TipoDeCarga repTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
                pedido.TipoDeCarga = repTipoDeCarga.BuscarPorCodigo(tipoCarga);
            }

            if (ConfiguracaoEmbarcador.UtilizarMultiplosModelosVeicularesPedido)
            {
                if (pedido.ModelosVeiculares != null)
                    pedido.ModelosVeiculares.Clear();
                else
                    pedido.ModelosVeiculares = new List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>();

                foreach (var modeloVeicularCarga in listaModelosVeiculares)
                {
                    if (modeloVeicularCarga > 0)
                    {
                        pedido.ModelosVeiculares.Add(repModeloVeicularCarga.BuscarPorCodigo(modeloVeicularCarga));
                        pedido.ModeloVeicularCarga = repModeloVeicularCarga.BuscarPorCodigo(modeloVeicularCarga);
                    }
                }
            }
            else
            {
                int modeloVeicularCarga = 0;
                int.TryParse(Request.Params("ModeloVeicularCarga"), out modeloVeicularCarga);
                if (modeloVeicularCarga > 0)
                {
                    pedido.ModeloVeicularCarga = repModeloVeicularCarga.BuscarPorCodigo(modeloVeicularCarga);
                }
            }

            int tipoColeta = 0;
            int.TryParse(Request.Params("TipoColeta"), out tipoColeta);
            if (tipoColeta > 0)
            {
                pedido.TipoColeta = new Dominio.Entidades.TipoColeta() { Codigo = tipoColeta };
            }

            int tipoOperacao = 0;
            int.TryParse(Request.Params("TipoOperacao"), out tipoOperacao);
            if (tipoOperacao > 0)
            {
                Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                pedido.TipoOperacao = repTipoOperacao.BuscarPorCodigo(tipoOperacao);

                if ((pedido.TipoOperacao?.UsarConfiguracaoEmissao ?? false) && !string.IsNullOrWhiteSpace(pedido.TipoOperacao.ObservacaoCTe) && (!pedido.ObservacaoCTe?.ToLower().Contains(pedido.TipoOperacao.ObservacaoCTe.ToLower()) ?? true))
                    pedido.ObservacaoCTe += string.Concat(" ", pedido.TipoOperacao.ObservacaoCTe);
            }

            if (!(pedido.TipoOperacao?.ConfiguracaoAgendamentoColetaEntrega?.UtilizarDataSaidaGuaritaComoTerminoCarregamento ?? false))
                pedido.DataTerminoCarregamento = Request.GetNullableDateTimeParam("DataTerminoCarregamento");

            if (pedido.TipoOperacao?.UtilizarDeslocamentoPedido ?? false)
            {
                double cpfCnpjClienteDeslocamento = Request.GetDoubleParam("ClienteDeslocamento");

                if (cpfCnpjClienteDeslocamento > 0d)
                    pedido.ClienteDeslocamento = repCliente.BuscarPorCPFCNPJ(cpfCnpjClienteDeslocamento);

                if (pedido.ClienteDeslocamento == null)
                    throw new ControllerException(Localization.Resources.Pedidos.Pedido.ClienteDeslocamentoEObrigatorioParaTipoDeOperacaoInformado);
            }
            else
                pedido.ClienteDeslocamento = null;

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                if (pedido.TipoOperacao != null)
                {
                    if (!pedido.TipoOperacao.GeraCargaAutomaticamente)
                    {
                        pedido.GerarAutomaticamenteCargaDoPedido = false;
                        pedido.PedidoTotalmenteCarregado = false;
                        //TODO: PPC - Adicionado log temporário para identificar problema de retorno de saldo de pedido.
                        Servicos.Log.TratarErro(string.Format(Localization.Resources.Pedidos.Pedido.PedidoLiberouSaldoPesoTotal, pedido.NumeroPedidoEmbarcador, pedido.PesoSaldoRestante, pedido.PesoTotal, pedido.PedidoTotalmenteCarregado), "SaldoPedido");
                    }
                    pedido.ColetaEmProdutorRural = pedido.TipoOperacao.ColetaEmProdutorRural;
                }
            }

            //Embarcador deve sempre olhar o campo na tela do Pedido para definir se gera ou não bool.Parse(Request.Params("GerarAutomaticamenteCargaDoPedido"))
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS || (configuracaoGeralCarga?.UtilizarConfiguracaoTipoOperacaoGeracaoCargaPorPedido ?? false))
                if (!pedido.GerarAutomaticamenteCargaDoPedido && pedido.TipoOperacao != null && pedido.TipoOperacao.GeraCargaAutomaticamente)
                    pedido.GerarAutomaticamenteCargaDoPedido = true;

            double.TryParse(Request.Params("Tomador"), out double tomador);
            if (tomador > 0d)
                pedido.Tomador = repCliente.BuscarPorCPFCNPJ(tomador);

            pedido.UsarTipoTomadorPedido = Request.GetBoolParam("UsarTipoTomadorPedido");
            if (pedido.UsarTipoTomadorPedido)
            {
                pedido.TipoTomador = Request.GetEnumParam<Dominio.Enumeradores.TipoTomador>("TipoTomador");
                if (pedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros && pedido.Tomador == null)
                {
                    unitOfWork.Rollback();
                    throw new ControllerException(Localization.Resources.Pedidos.Pedido.QuandoTipoTomadorForOutrosObrigatorioInformarTomador);
                }
            }
            else
            {
                if (pedido.TipoPagamento == Dominio.Enumeradores.TipoPagamento.Outros)
                    pedido.TipoTomador = Dominio.Enumeradores.TipoTomador.Outros;
                else if (pedido.TipoPagamento == Dominio.Enumeradores.TipoPagamento.A_Pagar)
                    pedido.TipoTomador = Dominio.Enumeradores.TipoTomador.Destinatario;
                else
                    pedido.TipoTomador = Dominio.Enumeradores.TipoTomador.Remetente;

            }

            pedido.FuncionarioVendedor = repUsuario.BuscarPorCodigo(Request.GetIntParam("FuncionarioVendedor"));
            pedido.FuncionarioSupervisor = repUsuario.BuscarPorCodigo(Request.GetIntParam("FuncionarioSupervisor"));
            pedido.FuncionarioGerente = repUsuario.BuscarPorCodigo(Request.GetIntParam("FuncionarioGerente"));
            pedido.ValorFreteInformativo = Request.GetDecimalParam("ValorFreteInformativo");
            pedido.Adicional1 = Request.GetStringParam("ProcImportacao");
            pedido.Safra = Request.GetStringParam("Safra");
            pedido.PercentualAdiantamentoTerceiro = Request.GetDecimalParam("PercentualAdiantamentoTerceiro");
            pedido.PercentualMinimoAdiantamentoTerceiro = Request.GetDecimalParam("PercentualMinimoAdiantamentoTerceiro");
            pedido.PercentualMaximoAdiantamentoTerceiro = Request.GetDecimalParam("PercentualMaximoAdiantamentoTerceiro");

            double responsavelRedespacho = 0;
            double.TryParse(Request.Params("ResponsavelRedespacho"), out responsavelRedespacho);
            if (responsavelRedespacho > 0)
                pedido.ResponsavelRedespacho = repCliente.BuscarPorCPFCNPJ(responsavelRedespacho);
            else
                pedido.ResponsavelRedespacho = null;

            int codigoPorto = 0;
            int.TryParse(Request.Params("Porto"), out codigoPorto);
            if (codigoPorto > 0)
                pedido.Porto = repPorto.BuscarPorCodigo(codigoPorto);
            else
                pedido.Porto = null;

            int codigoTipoTerminalImportacao = 0;
            int.TryParse(Request.Params("TipoTerminalImportacao"), out codigoTipoTerminalImportacao);
            if (codigoTipoTerminalImportacao > 0)
            {
                Repositorio.Embarcador.Pedidos.TipoTerminalImportacao repTipoTerminalImportacao = new Repositorio.Embarcador.Pedidos.TipoTerminalImportacao(unitOfWork);
                pedido.TipoTerminalImportacao = repTipoTerminalImportacao.BuscarPorCodigo(codigoTipoTerminalImportacao);
            }

            int codigoLocalidadeEntregaImportacao = 0;
            int.TryParse(Request.Params("LocalidadeEntregaImportacao"), out codigoLocalidadeEntregaImportacao);
            if (codigoLocalidadeEntregaImportacao > 0)
                pedido.LocalidadeEntregaImportacao = repLocalidade.BuscarPorCodigo(codigoLocalidadeEntregaImportacao);
            else
                pedido.LocalidadeEntregaImportacao = null;

            int.TryParse(Request.Params("CanalEntrega"), out int codigoCanalEntrega);
            if (codigoCanalEntrega > 0)
            {
                Repositorio.Embarcador.Pedidos.CanalEntrega repCanalEntrega = new Repositorio.Embarcador.Pedidos.CanalEntrega(unitOfWork);
                pedido.CanalEntrega = repCanalEntrega.BuscarPorCodigo(codigoCanalEntrega);
            }
            else
                pedido.CanalEntrega = null;

            int codigoCanalVenda = Request.GetIntParam("CanalVenda");
            if (codigoCanalVenda > 0)
            {
                Repositorio.Embarcador.Pedidos.CanalVenda repositorioCanalVenda = new Repositorio.Embarcador.Pedidos.CanalVenda(unitOfWork);
                pedido.CanalVenda = repositorioCanalVenda.BuscarPorCodigo(codigoCanalVenda);
            }
            else
                pedido.CanalVenda = null;

            int deposito = 0;
            int.TryParse(Request.Params("Deposito"), out deposito);
            if (deposito > 0)
            {
                Repositorio.Embarcador.WMS.Deposito repDeposito = new Repositorio.Embarcador.WMS.Deposito(unitOfWork);
                pedido.Deposito = repDeposito.BuscarPorCodigo(deposito);
            }
            else
                pedido.Deposito = null;

            int codigoCentroResultado = Request.GetIntParam("CentroResultado");
            pedido.CentroResultado = codigoCentroResultado > 0 ? repCentroResultado.BuscarPorCodigo(codigoCentroResultado) : null;

            int codigoCentroCarregamento = Request.GetIntParam("CentroCarregamento");
            pedido.CentroCarregamento = codigoCentroCarregamento > 0 ? repCentroCarregamento.BuscarPorCodigo(codigoCentroCarregamento) : null;

            double cpfCnpjPontoPartida = Request.GetDoubleParam("PontoPartida");

            pedido.PontoPartida = cpfCnpjPontoPartida > 0D ? repCliente.BuscarPorCPFCNPJ(cpfCnpjPontoPartida) : null;

            pedido.SituacaoAtualPedidoRetirada = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtualPedidoRetirada.LiberacaoFinanceira;

            pedido.CentroDeCustoViagem = null;

            int CentroDeCustoViagem = Request.GetIntParam("CentroDeCustoViagem");
            if (CentroDeCustoViagem > 0)
            {
                var repositorio = new Repositorio.Embarcador.Logistica.CentroCustoViagem(unitOfWork);
                pedido.CentroDeCustoViagem = repositorio.BuscarPorCodigo(CentroDeCustoViagem);
            }

            pedido.Balsa = null;

            int codigoBalsa = Request.GetIntParam("Balsa");
            if (codigoBalsa > 0)
            {
                var repositorioNavio = new Repositorio.Embarcador.Pedidos.Navio(unitOfWork);
                pedido.Balsa = repositorioNavio.BuscarPorCodigo(codigoBalsa);
            }

            pedido.DataPrevisaoTerminoCarregamento = Request.GetNullableDateTimeParam("DataPrevisaoTerminoCarregamento");

            double localPaletizacao = 0;
            double.TryParse(Request.Params("LocalPaletizacao"), out localPaletizacao);
            if (localPaletizacao > 0)
                pedido.LocalPaletizacao = repCliente.BuscarPorCPFCNPJ(localPaletizacao);
            else
                pedido.LocalPaletizacao = null;

            int mercadoLivreRota = 0;
            int.TryParse(Request.Params("MercadoLivreRota"), out mercadoLivreRota);
            pedido.Rota = mercadoLivreRota;
            pedido.Facility = Request.Params("MercadoLivreFacility");


            return pedido;
        }

        private void PreencherDadosGeraisCargaPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.CentroResultado repositorioCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unitOfWork);
            Repositorio.Embarcador.Financeiro.PlanoConta repositorioPlanoConta = new Repositorio.Embarcador.Financeiro.PlanoConta(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao repositorioCargaPedidoContaContabilContabilizacao = new Repositorio.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao(unitOfWork);

            pedido.ValorCobrancaFreteCombinado = Request.GetNullableDecimalParam("ValorCobrancaFreteCombinado");
            if (pedido.ValorCobrancaFreteCombinado == 0)
                pedido.ValorCobrancaFreteCombinado = null;

            int codigoCentroResultado = Request.GetIntParam("CentroResultadoEmbarcador");
            int codigoPlanoConta = Request.GetIntParam("ContaContabil");

            bool utilizaPEPCentroCusto = pedido.TipoOperacao?.TipoOperacaoUtilizaCentroDeCustoPEP ?? false;
            bool utilizaContaRazao = pedido.TipoOperacao?.TipoOperacaoUtilizaContaRazao ?? false;

            if (utilizaPEPCentroCusto)
            {
                pedido.CentroResultadoEmbarcador = codigoCentroResultado > 0 ? repositorioCentroResultado.BuscarPorCodigo(codigoCentroResultado) : null;
                pedido.ElementoPEP = Request.GetStringParam("ElementoPEP");
            }

            if (utilizaContaRazao)
                pedido.ContaContabil = codigoPlanoConta > 0 ? repositorioPlanoConta.BuscarPorCodigo(codigoPlanoConta) : null;

            pedido.Usuario = Usuario;

            if (pedido.Codigo > 0)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedido = repositorioCargaPedido.BuscarPorPedido(pedido.Codigo);

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargasPedido)
                {

                    if (utilizaPEPCentroCusto)
                    {
                        cargaPedido.CentroResultado = pedido.CentroResultadoEmbarcador;
                        cargaPedido.ElementoPEP = pedido.ElementoPEP;

                        repositorioCargaPedido.Atualizar(cargaPedido);
                    }

                    if (utilizaContaRazao && pedido.ContaContabil != null)
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao contaContabil = repositorioCargaPedidoContaContabilContabilizacao.BuscarFirstOrDefaultPorCargaPedido(cargaPedido.Codigo);

                        if (contaContabil == null)
                            contaContabil = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao();

                        contaContabil.CargaPedido = cargaPedido;
                        contaContabil.PlanoConta = pedido.ContaContabil;
                        contaContabil.TipoContabilizacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContabilizacao.Debito;
                        contaContabil.TipoContaContabil = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.TotalReceber;

                        if (contaContabil.Codigo > 0)
                            repositorioCargaPedidoContaContabilContabilizacao.Atualizar(contaContabil);
                        else
                            repositorioCargaPedidoContaContabilContabilizacao.Inserir(contaContabil);
                    }
                }
            }
        }

        private Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador ObterProdutoEmbarcador(Repositorio.UnitOfWork unitOfWork)
        {
            if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                int codigoProdutoEmbarcador = Request.GetIntParam("ProdutoEmbarcador");

                if (codigoProdutoEmbarcador > 0)
                {
                    Repositorio.Embarcador.Produtos.ProdutoEmbarcador repositorioProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork);

                    return repositorioProdutoEmbarcador.BuscarPorCodigo(codigoProdutoEmbarcador);
                }
            }

            return null;
        }

        private Dominio.Entidades.RotaFrete ObterRotaFreteClienteDeslocamento(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            if (pedido.TipoOperacao?.UtilizarDeslocamentoPedido ?? false)
            {
                if (pedido.Remetente == null)
                    throw new ControllerException(Localization.Resources.Pedidos.Pedido.ClienteRemetenteNaoInformado);

                Repositorio.RotaFrete repositorioRotaFrete = new Repositorio.RotaFrete(unitOfWork);
                Dominio.Entidades.RotaFrete rotaFreteClienteDeslocamento = repositorioRotaFrete.BuscarPorRemetenteDestinatario(pedido.ClienteDeslocamento.CPF_CNPJ, pedido.Remetente?.CPF_CNPJ ?? 0d);

                if (rotaFreteClienteDeslocamento == null)
                {
                    List<Dominio.Entidades.Cliente> remetentes = new List<Dominio.Entidades.Cliente>();
                    remetentes.Add(pedido.ClienteDeslocamento);

                    List<Dominio.Entidades.Cliente> destinatarios = new List<Dominio.Entidades.Cliente>();
                    destinatarios.Add(pedido.Remetente);

                    List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaDestinatarioOrdem> destinatariosOrdenados = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaDestinatarioOrdem>();
                    destinatariosOrdenados.AddRange(from obj in destinatarios
                                                    select new Dominio.ObjetosDeValor.Embarcador.Logistica.RotaDestinatarioOrdem()
                                                    {
                                                        Cliente = obj,
                                                        Ordem = 0
                                                    });

                    rotaFreteClienteDeslocamento = Servicos.Embarcador.Carga.RotaFrete.GerarRota(remetentes, destinatariosOrdenados, unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDescricaoRota.CodigoRota, null, Auditado);
                    if (rotaFreteClienteDeslocamento == null)
                        throw new ControllerException(Localization.Resources.Pedidos.Pedido.RotaDeFreteEntreClienteDeslocamentoERemetenteNaoEncontrada);
                }

                return rotaFreteClienteDeslocamento;
            }

            return null;
        }

        private Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedido ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            int codigoFilial = Request.GetIntParam("Filial");
            int codigoTipoCarga = Request.GetIntParam("TipoCarga");
            int codigoCargaDestino = Request.GetIntParam("CargaDestino");
            int codigoCanalEntrega = Request.GetIntParam("CanalEntrega");
            List<int> codigosCanalEntrega = Request.GetListParam<int>("CodigosCanalEntrega");
            if (codigoCanalEntrega > 0 && codigosCanalEntrega.Count == 0)
                codigosCanalEntrega.Add(codigoCanalEntrega);
            int codigoMotorista = Request.GetIntParam("Motorista");
            int codigoVeiculo = Request.GetIntParam("Veiculo");
            int codigoTransportador = Request.GetIntParam("Transportador");

            List<int> codigosTransportador = Request.GetListParam<int>("Transportador");

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repositorioConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repositorioConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoPortalMultiClifor repConfiguracaoPortalMultiClifor = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPortalMultiClifor(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);



            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica = ObterOperadorLogistica(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = repositorioConfiguracaoGeral.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = repositorioConfiguracaoPedido.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPortalMultiClifor configuracaoPortalMultiClifor = repConfiguracaoPortalMultiClifor.BuscarConfiguracaoPadrao();

            if (codigosTransportador.Count > 0)
                codigosTransportador.AddRange(repositorioEmpresa.BuscarCodigosFiliaisVinculadas(codigosTransportador));

            Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedido filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedido()
            {
                CidadePoloDestino = Request.GetIntParam("CidadePoloDestino"),
                CidadePoloOrigem = Request.GetIntParam("CidadePoloOrigem"),
                CodigosFilial = codigoFilial == 0 ? ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork) : new List<int>() { codigoFilial },
                CodigosTipoCarga = codigoTipoCarga == 0 ? ObterListaCodigoTipoCargaPermitidosOperadorLogistica(unitOfWork) : new List<int>() { codigoTipoCarga },
                CodigosTipoOperacao = ObterListaCodigoTipoOperacaoPermitidosOperadorLogistica(unitOfWork),
                DataColeta = Request.GetNullableDateTimeParam("DataColeta"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicio"),
                DataLimite = Request.GetNullableDateTimeParam("DataFim"),
                Destinatario = Request.GetDoubleParam("Destinatario"),
                Expedidor = Request.GetDoubleParam("Expedidor"),
                Recebedor = Request.GetDoubleParam("Recebedor"),
                Destino = Request.GetIntParam("Destino"),
                FiltrarPorParteDoNumero = configuracaoEmbarcador?.FiltrarCargasPorParteDoNumero ?? false,
                GrupoPessoa = Request.GetIntParam("GrupoPessoa"),
                NumeroCarga = Request.GetStringParam("CodigoCargaEmbarcador"),
                NumeroNotaFiscal = Request.GetIntParam("NotaFiscal"),
                NumeroPedido = Request.GetIntParam("NumeroPedido"),
                NumeroPedidoEmbarcador = Request.GetStringParam("NumeroPedidoEmbarcador"),
                Origem = Request.GetIntParam("Origem"),
                PaisDestino = Request.GetIntParam("PaisDestino"),
                PaisOrigem = Request.GetIntParam("PaisOrigem"),
                Remetente = Request.GetDoubleParam("Remetente"),
                Situacao = Request.GetNullableEnumParam<SituacaoPedido>("Situacao"),
                Tomador = Request.GetDoubleParam("Tomador"),
                CodigosCanalEntrega = codigosCanalEntrega,
                NumeroBooking = Request.GetStringParam("NumeroBooking"),
                NumeroOS = Request.GetStringParam("NumeroOS"),
                PedidoEmpresaResponsavel = Request.GetIntParam("PedidoEmpresaResponsavel"),
                PedidoCentroCusto = Request.GetIntParam("PedidoCentroCusto"),
                Container = Request.GetIntParam("Container"),
                TipoOperacao = Request.GetIntParam("TipoOperacao"),
                ProvedorOS = Request.GetDoubleParam("ProvedorOS"),
                CodigoFuncionarioVendedor = Request.GetIntParam("FuncionarioVendedor"),
                NumeroEXP = Request.GetStringParam("NumeroEXP"),
                PedidoParaReentrega = Request.GetBoolParam("PedidosParaReentrega"),
                CodigosMotorista = codigoMotorista > 0 ? new List<int>() { codigoMotorista } : null,
                CodigosVeiculo = codigoVeiculo > 0 ? new List<int>() { codigoVeiculo } : null,
                ProcImportacao = Request.GetStringParam("ProcImportacao"),
                PrevisaoDataInicial = Request.GetNullableDateTimeParam("PrevisaoDataInicial"),
                TipoServicoMultisoftware = TipoServicoMultisoftware,
                NumeroCotacao = Request.GetIntParam("NumeroCotacao"),
                NumeroProtocoloIntegracaoPedido = Request.GetIntParam("NumeroProtocoloIntegracaoPedido"),
                CodigosTransportador = codigosTransportador,
                //PrevisaoDataFinal = Request.GetNullableDateTimeParam("PrevisaoDataFinal"),
                ProgramaComSessaoRoteirizador = Request.GetBoolParam("ProgramaComSessaoRoteirizador"),
                NumeroTransporte = Request.GetStringParam("NumeroTransporte"),
                NumeroCarregamento = Request.GetStringParam("NumeroCarregamento"),
                SomentePedidosComNota = Request.GetBoolParam("PedidoComNotas"),
                NaoExibirPedidosDoDia = Request.GetBoolParam("NaoExibirPedidosDoDia"),
                NumeroCarregamentoPedido = Request.GetStringParam("NumeroCarregamentoPedido"),
                FiltrarPorMultiplosRegistros = configuracaoPedido?.PermitirConsultaMassivaDePedidos ?? false,
                NumeroOrdem = Request.GetStringParam("NumeroOrdem"),
                FiltrarCargasPorParteDoNumero = configuracaoEmbarcador?.FiltrarCargasPorParteDoNumero ?? false,
                SituacaoComercialPedido = Request.GetIntParam("SituacaoComercialPedido"),
                BloquearSituacaoComercialPedido = false,
                CategoriaOS = Request.GetListEnumParam<CategoriaOS>("CategoriaOS"),
                TipoOSConvertido = Request.GetListEnumParam<TipoOSConvertido>("TipoOSConvertido"),
                UsuarioUtilizaSegregacaoPorProvedor = Usuario.UsuarioUtilizaSegregacaoPorProvedor,
                CodigosProvedores = Usuario.UsuarioUtilizaSegregacaoPorProvedor ? Usuario.ClientesProvedores.Select(o => o.CPF_CNPJ).ToList() : new List<double>(),
                CodigoCarga = Request.GetIntParam("CodigoCarga")
            };

            if (filtrosPesquisa.Recebedor == 0)
                filtrosPesquisa.ListaRecebedor = ObterListaCnpjCpfRecebedorPermitidosOperadorLogistica(unitOfWork);

            if (filtrosPesquisa.Expedidor == 0)
                filtrosPesquisa.ListaExpedidor = ObterListaCnpjCpfExpedidorPermitidosOperadorLogistica(unitOfWork);

            if (codigoCargaDestino > 0)
            {
                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosCargaDestino = repCargaPedido.BuscarPedidosPorCarga(codigoCargaDestino);
                filtrosPesquisa.NumeroControlesPedido = pedidosCargaDestino.Where(o => !string.IsNullOrWhiteSpace(o.NumeroControle)).Select(o => o.NumeroControle).Distinct().ToList();
            }

            if (operadorLogistica?.TelaPedidosResumido == true)
            {
                filtrosPesquisa.CodigosUsuario = new List<int>() {
                    Usuario.Codigo
                };
            }

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
            {
                if (Empresa.Matriz.Any())
                    filtrosPesquisa.TransportadoraMatriz = Empresa.Matriz.FirstOrDefault().Codigo;
                else
                    filtrosPesquisa.TransportadoraMatriz = Empresa.Codigo;
            }

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
            {
                bool filtrarPorRemetente = ClienteConfiguracao?.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS || (configuracaoPortalMultiClifor?.FiltrarPedidosPorRemetenteRetiradaProduto ?? false);
                bool compartilharAcessoEntreGrupoPessoas = IsCompartilharAcessoEntreGrupoPessoas();
                int codigoGrupoPessoa = Usuario.ClienteFornecedor?.GrupoPessoas?.Codigo ?? 0;

                Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> tiposOperacoes = repositorioTipoOperacao.BuscarTipoOperacoesRetirada();

                filtrosPesquisa.ProgramaComSessaoRoteirizador = Request.GetBoolParam("ProgramaComSessaoRoteirizador");
                filtrosPesquisa.CodigosTipoOperacao = (from obj in tiposOperacoes select obj.Codigo).ToList();

                if (filtrosPesquisa.CodigosTipoOperacao.Count > 0)
                    filtrosPesquisa.FiltroRetirada = true;

                if (compartilharAcessoEntreGrupoPessoas)
                {
                    if (filtrarPorRemetente)
                        filtrosPesquisa.GrupoPessoa = codigoGrupoPessoa;
                    else if (filtrosPesquisa.FiltroRetirada)
                        filtrosPesquisa.CodigosGrupoPessoaRetirada = new List<int> { codigoGrupoPessoa };
                    else
                        filtrosPesquisa.GrupoPessoaDestinatario = codigoGrupoPessoa;
                }
                else
                {
                    if (Usuario.Empresa != null && !TipoComercialGerente())
                    {
                        if (Usuario.Empresa.EmpresaPai != null)
                            filtrosPesquisa.Transportador = Usuario.Empresa.Codigo;
                    }

                    if (this.Usuario.ClienteFornecedor != null && !TipoComercialGerente())
                    {
                        if (filtrarPorRemetente)
                            filtrosPesquisa.Remetente = this.Usuario.ClienteFornecedor.CPF_CNPJ;
                        else if (filtrosPesquisa.FiltroRetirada)
                            filtrosPesquisa.CodigosRemetenteDestinatarioRetirada = new List<double> { this.Usuario.ClienteFornecedor.CPF_CNPJ };
                        else
                            filtrosPesquisa.Destinatario = this.Usuario.ClienteFornecedor.CPF_CNPJ;
                    }

                    if (this.Usuario.TipoComercial == TipoComercial.Vendedor)
                        filtrosPesquisa.CodigoFuncionarioVendedor = this.Usuario.Codigo;
                }

                if (configuracaoGeral.FiltrarPedidosSemFiltroPorFilialNoPortalDoFornecedor)
                    filtrosPesquisa.CodigosFilial = new List<int>();

                // #43685 - Pedidos totalmente carregado aparecendo para coleta... sem produtos..
                //filtrosPesquisa.PedidoSemCarga = true;
            }

            if ((this.Usuario?.LimitarOperacaoPorEmpresa ?? false) && (configuracaoGeral?.AtivarConsultaSegregacaoPorEmpresa ?? false) && this.Usuario?.Empresas != null && this.Usuario?.Empresas.Count > 0)
            {
                filtrosPesquisa.CodigosEmpresa = this.Usuario.Empresas.Select(o => o.Codigo).ToList();
            }
            if (codigoTransportador > 0)
                filtrosPesquisa.CodigoTransportador = codigoTransportador;

            return filtrosPesquisa;
        }

        private bool TipoComercialGerente()
        {
            return Usuario.TipoComercial == TipoComercial.Gerente;
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "Remetente")
                return "Remetente.Nome";

            if (propriedadeOrdenar == "Destinatario")
                return "Destinatario.Nome";

            return propriedadeOrdenar;
        }

        private void GerarNotificacaoPedidoNovo(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Configuracoes.ControleAlerta repControleAlerta = new Repositorio.Embarcador.Configuracoes.ControleAlerta(unitOfWork);
            Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork);
            Repositorio.Embarcador.Configuracoes.Alerta repAlerta = new Repositorio.Embarcador.Configuracoes.Alerta(unitOfWork);
            Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(unitOfWork.StringConexao, null, tipoServicoMultisoftware, string.Empty);

            Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo();

            List<Dominio.Entidades.Embarcador.Configuracoes.ControleAlerta> controlesPedido = repControleAlerta.BuscarControlesPorTela(ControleAlertaTela.Pedido);

            if (controlesPedido == null || controlesPedido.Count == 0)
                return;

            foreach (Dominio.Entidades.Embarcador.Configuracoes.ControleAlerta controle in controlesPedido)
            {
                string descricaoAlerta = string.Format(Localization.Resources.Pedidos.Pedido.NovoPedidoPrioritarioRecebido, pedido.Numero.ToString("n0"), (pedido.GrupoPessoas != null ? pedido.GrupoPessoas.Descricao : pedido.Remetente != null && pedido.Remetente.GrupoPessoas != null ? pedido.Remetente.GrupoPessoas.Descricao : string.Empty));

                Dominio.Entidades.Embarcador.Configuracoes.Alerta alerta = new Dominio.Entidades.Embarcador.Configuracoes.Alerta();
                alerta.CodigoEntidade = pedido.Codigo;
                alerta.Descricao = descricaoAlerta;
                alerta.Empresa = null;
                alerta.Funcionario = controle.Funcionario;
                alerta.Ocultar = false;
                alerta.TelaAlerta = ControleAlertaTela.Pedido;
                alerta.Data = DateTime.Now;
                alerta.FormasAlerta = new List<ControleAlertaForma>();

                if (controle.FormasAlerta.Contains(ControleAlertaForma.Notificacao))
                {
                    alerta.FormasAlerta.Add(ControleAlertaForma.Notificacao);
                    serNotificacao.GerarNotificacao(controle.Funcionario, pedido.Codigo, "Pedidos/Pedido", descricaoAlerta, IconesNotificacao.agConfirmacao, TipoNotificacao.todas, tipoServicoMultisoftware, unitOfWork);
                }

                if (controle.FormasAlerta.Contains(ControleAlertaForma.Email))
                {
                    alerta.FormasAlerta.Add(ControleAlertaForma.Email);

                    if (!string.IsNullOrWhiteSpace(alerta.Funcionario.Email))
                    {
                        List<string> emails = new List<string>();
                        emails.Add(alerta.Funcionario.Email);
                        Servicos.Email.EnviarEmail(email.Email, email.Email, email.Senha, null, emails.ToArray(), null, Localization.Resources.Pedidos.Pedido.AlertaPedidoPrioritarioRecebido, descricaoAlerta, email.Smtp, out string mensagemErro,
                            email.DisplayEmail, null, "", email.RequerAutenticacaoSmtp, "", email.PortaSmtp, unitOfWork);
                    }
                }

                if (controle.FormasAlerta.Contains(ControleAlertaForma.PainelAlerta))
                    alerta.FormasAlerta.Add(ControleAlertaForma.PainelAlerta);

                repAlerta.Inserir(alerta);
            }
        }

        private void NotificarJanelaCarregamento(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedido = repCargaPedido.BuscarPorPedido(pedido.Codigo);

            if (cargaPedido.Count > 0)
            {
                int codigoCarga = cargaPedido.First().Carga.Codigo;
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repCargaJanelaCarregamento.BuscarPorCarga(codigoCarga);

                if (cargaJanelaCarregamento != null)
                    new Servicos.Embarcador.Hubs.JanelaCarregamento().InformarJanelaCarregamentoAtualizada(cargaJanelaCarregamento);
            }
        }

        private void AtualizarDatasPrevisaoColetaEntrega(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Repositorio.UnitOfWork unitOfWork)
        {
            if (pedido?.DataCarregamentoPedido == null)
                Servicos.Embarcador.Pedido.Pedido.GerarPrevisaoColetaEntrega(pedido, unitOfWork);
        }

        private void GerarIntegracaoTrizy(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoIntegracao repPedidoIntegracao = new Repositorio.Embarcador.Pedidos.PedidoIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao pedidoIntegracao = new Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao()
            {
                Pedido = pedido,
                SituacaoIntegracao = SituacaoIntegracao.AgIntegracao,
                Tentativas = 0,
                TipoIntegracao = repTipoIntegracao.BuscarPorTipo(TipoIntegracao.Trizy),
                ProblemaIntegracao = string.Empty,
                DataEnvio = DateTime.Now
            };

            repPedidoIntegracao.Inserir(pedidoIntegracao);
        }

        private void GerarIntegracaoAX(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoIntegracao repPedidoIntegracao = new Repositorio.Embarcador.Pedidos.PedidoIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao pedidoIntegracao = new Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao()
            {
                Pedido = pedido,
                SituacaoIntegracao = SituacaoIntegracao.AgIntegracao,
                Tentativas = 0,
                TipoIntegracao = repTipoIntegracao.BuscarPorTipo(TipoIntegracao.AX),
                ProblemaIntegracao = string.Empty,
                DataEnvio = DateTime.Now
            };

            repPedidoIntegracao.Inserir(pedidoIntegracao);
        }

        private void SalvarComponenteMultimodal(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete tipoComponenteFrete, ref string msgRetorno, decimal valorComponente)
        {
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoComponenteFrete repPedidoComponenteFrete = new Repositorio.Embarcador.Pedidos.PedidoComponenteFrete(unitOfWork);

            msgRetorno = "";
            Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete = null;
            if (tipoComponenteFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ADVALOREM)
                componenteFrete = repComponenteFrete.BuscarPorTipoComponenteFrete(tipoComponenteFrete);
            else if (tipoComponenteFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.OUTROS)
                componenteFrete = repComponenteFrete.buscarPorCodigoEmbarcador("BAF");
            else
                msgRetorno = Localization.Resources.Pedidos.Pedido.TipoDeComponenteNaoConfigurado;

            if (componenteFrete != null)
            {
                Dominio.Entidades.Embarcador.Pedidos.PedidoComponenteFrete pedidoComponenteFrete = repPedidoComponenteFrete.BuscarPorCompomente(pedido.Codigo, componenteFrete.TipoComponenteFrete, componenteFrete, false);
                bool inserir = false;
                if (pedidoComponenteFrete == null)
                {
                    pedidoComponenteFrete = new Dominio.Entidades.Embarcador.Pedidos.PedidoComponenteFrete();
                    inserir = true;
                }
                pedidoComponenteFrete.ComponenteFrete = componenteFrete;

                if (componenteFrete.ImprimirOutraDescricaoCTe)
                    pedidoComponenteFrete.OutraDescricaoCTe = componenteFrete.DescricaoCTe;

                pedidoComponenteFrete.ComponenteFilialEmissora = false;
                pedidoComponenteFrete.Pedido = pedido;
                pedidoComponenteFrete.TipoComponenteFrete = componenteFrete.TipoComponenteFrete;

                bool incluirICMSFreteInformadoManualmente = ConfiguracaoEmbarcador.IncluirICMSFreteInformadoManualmente;
                if (ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal)
                {
                    incluirICMSFreteInformadoManualmente = ConfiguracaoEmbarcador.IncluirICMSFreteInformadoManualmente;
                    if (pedido.TipoOperacao != null && pedido.TipoOperacao.NaoIncluirICMSFrete)
                        incluirICMSFreteInformadoManualmente = false;
                }

                if (tipoComponenteFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ADVALOREM)
                {
                    pedidoComponenteFrete.TipoValor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.PercentualSobreValorNotaFiscal;
                    pedidoComponenteFrete.IncluirBaseCalculoICMS = incluirICMSFreteInformadoManualmente;
                    pedidoComponenteFrete.IncluirIntegralmenteContratoFreteTerceiro = false;
                    pedidoComponenteFrete.Percentual = valorComponente;
                    if (pedido.ValorTotalNotasFiscais > 0)
                        pedidoComponenteFrete.ValorComponente = ((valorComponente / 100) * pedido.ValorTotalNotasFiscais);
                    else
                        pedidoComponenteFrete.ValorComponente = (decimal)0;
                }
                else
                {
                    pedidoComponenteFrete.IncluirBaseCalculoICMS = incluirICMSFreteInformadoManualmente;
                    pedidoComponenteFrete.IncluirIntegralmenteContratoFreteTerceiro = false;
                    pedidoComponenteFrete.Percentual = 0;
                    pedidoComponenteFrete.ValorComponente = valorComponente;
                }

                if (inserir)
                    repPedidoComponenteFrete.Inserir(pedidoComponenteFrete);
                else
                    repPedidoComponenteFrete.Atualizar(pedidoComponenteFrete);
            }
            else
                msgRetorno = Localization.Resources.Pedidos.Pedido.NaoExisteUmComponenteDeFreteCadastrado;
        }

        private void SalvarNovoPercursoEstados(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!string.IsNullOrWhiteSpace(Request.Params("PassagemPercursoEstado")))
            {
                Repositorio.Embarcador.Pedidos.PedidoLocaisPrestacaoPassagens repPassagemPercursoEstado = new Repositorio.Embarcador.Pedidos.PedidoLocaisPrestacaoPassagens(unidadeDeTrabalho);
                dynamic passagensPercursoEstado = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("PassagemPercursoEstado"));
                foreach (var dynPassagemPercursoEstado in passagensPercursoEstado)
                {
                    Dominio.Entidades.Embarcador.Pedidos.PedidoLocaisPrestacaoPassagens percursoEstado = new Dominio.Entidades.Embarcador.Pedidos.PedidoLocaisPrestacaoPassagens();
                    percursoEstado.Posicao = (int)dynPassagemPercursoEstado.Ordem;
                    percursoEstado.Pedido = pedido;
                    percursoEstado.EstadoDePassagem = new Dominio.Entidades.Estado()
                    {
                        Sigla = (string)dynPassagemPercursoEstado.EstadoDePassagem
                    };
                    repPassagemPercursoEstado.Inserir(percursoEstado);
                }
            }
        }

        private void AtualizarPercursoEstados(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!string.IsNullOrWhiteSpace(Request.Params("PassagemPercursoEstado")))
            {
                Repositorio.Embarcador.Pedidos.PedidoLocaisPrestacaoPassagens repPassagemPercursoEstado = new Repositorio.Embarcador.Pedidos.PedidoLocaisPrestacaoPassagens(unidadeDeTrabalho);

                dynamic passagensPercursoEstado = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("PassagemPercursoEstado"));
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoLocaisPrestacaoPassagens> passagensAtivos = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoLocaisPrestacaoPassagens>();
                foreach (var dynPassagemPercursoEstado in passagensPercursoEstado)
                {
                    Dominio.Entidades.Embarcador.Pedidos.PedidoLocaisPrestacaoPassagens passagem = repPassagemPercursoEstado.BuscarPorCodigo((int)dynPassagemPercursoEstado.Codigo);
                    if (passagem == null)
                    {
                        passagem = new Dominio.Entidades.Embarcador.Pedidos.PedidoLocaisPrestacaoPassagens();
                        passagem.Posicao = (int)dynPassagemPercursoEstado.Ordem;
                        passagem.Pedido = pedido;
                        passagem.EstadoDePassagem = new Dominio.Entidades.Estado() { Sigla = (string)dynPassagemPercursoEstado.EstadoDePassagem };
                        repPassagemPercursoEstado.Inserir(passagem);
                    }
                    else
                    {
                        passagem.Posicao = (int)dynPassagemPercursoEstado.Ordem;
                        passagem.Pedido = pedido;
                        passagem.EstadoDePassagem = new Dominio.Entidades.Estado() { Sigla = (string)dynPassagemPercursoEstado.EstadoDePassagem };
                        repPassagemPercursoEstado.Atualizar(passagem);
                    }
                    passagensAtivos.Add(passagem);
                }

                List<Dominio.Entidades.Embarcador.Pedidos.PedidoLocaisPrestacaoPassagens> passagensPercursosEstadosSalvosNoBanco = repPassagemPercursoEstado.BuscarPorPedido(pedido.Codigo);
                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoLocaisPrestacaoPassagens percursoSalvoNoBanco in passagensPercursosEstadosSalvosNoBanco)
                {
                    if (!passagensAtivos.Exists(obj => obj.Codigo == percursoSalvoNoBanco.Codigo))
                    {
                        repPassagemPercursoEstado.Deletar(percursoSalvoNoBanco);
                    }
                }
            }
        }

        private void SalvarListaCliente(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);
            if (pedido.Clientes == null)
                pedido.Clientes = new List<Dominio.Entidades.Cliente>();
            pedido.Clientes.Clear();

            dynamic listaCliente = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaClientes"));
            if (listaCliente != null)
            {
                foreach (var cliente in listaCliente)
                {
                    double codigo = 0;
                    double.TryParse((string)cliente.cliente.Codigo, out codigo);
                    if (codigo > 0)
                    {
                        Dominio.Entidades.Cliente cli = repCliente.BuscarPorCPFCNPJ(codigo);
                        pedido.Clientes.Add(cli);
                    }
                }
            }

        }

        private void SalvarListaMotorista(ref Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Repositorio.UnitOfWork unidadeDeTrabalho, List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas)
        {
            Repositorio.Usuario repMotorista = new Repositorio.Usuario(unidadeDeTrabalho);
            if (pedido.Motoristas == null)
                pedido.Motoristas = new List<Dominio.Entidades.Usuario>();

            dynamic listaMotorista = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaMotoristas"));

            if (listaMotorista == null)
                return;

            List<int> codigosNovos = new List<int>();
            List<int> codigosAntigos = pedido.Motoristas.Select(o => o.Codigo).ToList();

            foreach (var motorista in listaMotorista)
            {
                int codigo = 0;
                int.TryParse((string)motorista.Motorista.Codigo, out codigo);
                codigosNovos.Add(codigo);
            }

            if (!(codigosAntigos.All(codigo => codigosNovos.Contains(codigo)) && codigosNovos.All(codigo => codigosAntigos.Contains(codigo))))
                if (!this.Usuario.UsuarioAdministrador && !permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Pedido_PermitirAlteracaoMotorista))
                    throw new ControllerException("Usuário não tem permissão de informar ou alterar o motorista do pedido.");

            pedido.Motoristas.Clear();

            foreach (var motorista in listaMotorista)
            {
                int codigo = 0;
                int.TryParse((string)motorista.Motorista.Codigo, out codigo);
                if (codigo > 0)
                {
                    Dominio.Entidades.Usuario mot = repMotorista.BuscarPorCodigo(codigo);
                    pedido.Motoristas.Add(mot);
                }
            }

        }

        private void SalvarListaProdutos(ref Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Repositorio.UnitOfWork unidadeDeTrabalho, ref decimal totalCubagem, ref decimal totalPeso, ref decimal totalPalet, bool atualizando)
        {
            totalCubagem = 0;
            totalPeso = 0;
            totalPalet = 0;

            Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unidadeDeTrabalho);
            Repositorio.Embarcador.Pedidos.PedidoProduto repPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(unidadeDeTrabalho);
            Repositorio.Embarcador.Pedidos.PedidoProdutoONU repPedidoProdutoONU = new Repositorio.Embarcador.Pedidos.PedidoProdutoONU(unidadeDeTrabalho);
            Repositorio.Embarcador.Pedidos.ClassificacaoRiscoONU repClassificacaoRiscoONU = new Repositorio.Embarcador.Pedidos.ClassificacaoRiscoONU(unidadeDeTrabalho);
            Repositorio.Embarcador.Cargas.CargaPedidoProduto repCargaPedidoProduto = new Repositorio.Embarcador.Cargas.CargaPedidoProduto(unidadeDeTrabalho);
            Repositorio.Embarcador.Pedidos.LinhaSeparacao repLinhaSeparacao = new Repositorio.Embarcador.Pedidos.LinhaSeparacao(unidadeDeTrabalho);
            Repositorio.Embarcador.Produtos.EnderecoProduto repositorioEnderecoProduto = new Repositorio.Embarcador.Produtos.EnderecoProduto(unidadeDeTrabalho);
            Repositorio.Embarcador.Filiais.FilialArmazem repositorioFilialArmazem = new Repositorio.Embarcador.Filiais.FilialArmazem(unidadeDeTrabalho);

            List<int> codigosProdutos = new List<int>();
            List<int> codigosONUs = new List<int>();

            int qtdVolumes = 0;
            bool volumesAlterados = false;
            string produtoPredominante = "";
            dynamic listaProdutos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaProdutos"));


            if (listaProdutos != null)
            {
                bool controleSaldoProduto = false;
                List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SaldoPedidoProduto> saldoProdutos = new List<Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SaldoPedidoProduto>();
                if (atualizando)
                {
                    controleSaldoProduto = Servicos.Embarcador.Carga.MontagemCarga.SaldoPedidoProduto.ApresentarSaldoProdutoGridPedido(pedido, pedido.TipoOperacao);
                    saldoProdutos = Servicos.Embarcador.Carga.MontagemCarga.SaldoPedidoProduto.ObterSaldoPedidoProdutos(pedido.Codigo, unidadeDeTrabalho);
                }

                foreach (var produto in listaProdutos)
                {
                    string codigo = (string)produto.Produto.Codigo;

                    int.TryParse((string)produto.Produto.CodigoProdutoEmbarcador, out int codigoProduto);
                    int codigoLinhaSeparacao = 0;
                    try
                    {
                        int.TryParse((string)produto.Produto.LinhaSeparacao.Codigo, out codigoLinhaSeparacao);
                    }
                    catch
                    {
                        int.TryParse((string)produto.Produto.CodigoLinhaSeparacao, out codigoLinhaSeparacao);
                    }
                    int codigoEnderecoProduto = 0;
                    try
                    {
                        int.TryParse((string)produto.Produto.EnderecoProduto.Codigo, out codigoEnderecoProduto);
                    }
                    catch
                    {
                        int.TryParse((string)produto.Produto.CodigoEnderecoProduto, out codigoEnderecoProduto);
                    }
                    int.TryParse((string)produto.Produto.Codigo, out int codigoInterno);

                    decimal.TryParse((string)produto.Produto.AlturaCM, out decimal alturaCM);
                    decimal.TryParse((string)produto.Produto.ComprimentoCM, out decimal comprimentoCM);
                    decimal.TryParse((string)produto.Produto.LarguraCM, out decimal larguraCM);
                    decimal.TryParse((string)produto.Produto.MetrosCubico, out decimal metrosCubico);
                    decimal.TryParse((string)produto.Produto.Peso, out decimal peso);
                    decimal.TryParse((string)produto.Produto.Quantidade, out decimal quantidade);
                    decimal.TryParse((string)produto.Produto.QuantidadePlanejada, out decimal quantidadePlanejada);
                    decimal.TryParse((string)produto.Produto.QuantidadeSecundaria, out decimal quantidadeSecundaria);
                    decimal.TryParse((string)produto.Produto.QuantidadePalets, out decimal quantidadePalets);
                    decimal.TryParse((string)produto.Produto.PrecoUnitario, out decimal precoUnitario);
                    bool.TryParse((string)produto.Produto.PalletFechado, out bool palletFechado);
                    int.TryParse((string)produto.Produto.QuantidadeUnidadePorCaixa, out int quantidadeUnidadePorCaixa);
                    int.TryParse((string)produto.Produto.QuantidadeCaixaPorPallet, out int quantidadeCaixaPorPalete);
                    string organizacao = Request.GetStringParam("Organizacao");
                    string canal = Request.GetStringParam("Canal");
                    string setor = Request.GetStringParam("Setor");
                    int codigoArmazem = ((string)produto.Produto?.Armazem?.Codigo).ToInt();

                    Dominio.Entidades.Embarcador.Pedidos.PedidoProduto pedidoProduto;

                    if (codigoInterno > 0)
                        pedidoProduto = repPedidoProduto.BuscarPorCodigo(codigoInterno);
                    else
                        pedidoProduto = new Dominio.Entidades.Embarcador.Pedidos.PedidoProduto();

                    // #43334, não vamos deixar atualizar o saldo do produto para ficar negativo.. 
                    // Quando o produto já está parcialmente carregado.
                    if (codigoInterno > 0 && controleSaldoProduto)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.SaldoPedidoProduto saldoPedidoProduto = (from saldo in saldoProdutos
                                                                                                                       where saldo.CodigoPedidoProduto == codigoInterno
                                                                                                                       select saldo).FirstOrDefault();

                        if ((saldoPedidoProduto?.QtdeCarregado ?? 0) > quantidade)
                            throw new ControllerException($"A quantidade do produto {saldoPedidoProduto.Produto} não pode ser inferior a {saldoPedidoProduto.QtdeCarregado.ToString("n3")}, pois o mesmo já possui essa quantidade em carregamento(s).");
                    }

                    if (pedidoProduto.Quantidade != quantidade)
                        volumesAlterados = true;

                    pedidoProduto.AlturaCM = alturaCM;
                    pedidoProduto.ComprimentoCM = comprimentoCM;
                    pedidoProduto.LarguraCM = larguraCM;
                    pedidoProduto.MetroCubico = metrosCubico;
                    pedidoProduto.Observacao = (string)produto.Produto.Observacao;
                    pedidoProduto.Pedido = pedido;
                    pedidoProduto.PesoUnitario = peso;
                    pedidoProduto.PrecoUnitario = precoUnitario;
                    pedidoProduto.ValorProduto = precoUnitario;
                    pedidoProduto.Produto = repProdutoEmbarcador.BuscarPorCodigo(codigoProduto);
                    pedidoProduto.Quantidade = quantidade;
                    pedidoProduto.QuantidadePlanejada = quantidadePlanejada;
                    pedidoProduto.QuantidadeSecundaria = quantidadeSecundaria;
                    pedidoProduto.QuantidadePalet = quantidadePalets;
                    pedidoProduto.PalletFechado = palletFechado;
                    pedidoProduto.LinhaSeparacao = codigoLinhaSeparacao > 0 ? repLinhaSeparacao.BuscarPorCodigo(codigoLinhaSeparacao) : null;
                    pedidoProduto.EnderecoProduto = codigoEnderecoProduto > 0 ? repositorioEnderecoProduto.BuscarPorCodigo(codigoEnderecoProduto) : null;
                    pedidoProduto.QuantidadeCaixaPorPallet = quantidadeCaixaPorPalete;
                    pedidoProduto.QuantidadeUnidadePorCaixa = quantidadeUnidadePorCaixa;
                    pedidoProduto.Canal = canal;
                    pedidoProduto.CodigoOrganizacao = organizacao;
                    pedidoProduto.Setor = setor;
                    pedidoProduto.FilialArmazem = codigoArmazem > 0 ? repositorioFilialArmazem.BuscarPorCodigo(codigoArmazem, false) : null;
                    pedidoProduto.CamposPersonalizados = (string)produto.Produto.CamposPersonalizados;
                    pedidoProduto.UnidadeMedidaSecundaria = (string)produto.Produto.UnidadeMedidaSecundaria;
                    totalPalet += quantidadePalets;
                    totalPeso += peso * quantidade;
                    totalCubagem += metrosCubico * quantidade;

                    if (!produtoPredominante.Contains(pedidoProduto.Produto.Descricao))
                        produtoPredominante += pedidoProduto.Produto.Descricao + " ";

                    if (pedidoProduto.Quantidade > 0)
                        qtdVolumes += (int)pedidoProduto.Quantidade;

                    if (codigoInterno > 0)
                        repPedidoProduto.Atualizar(pedidoProduto);
                    else
                        repPedidoProduto.Inserir(pedidoProduto);

                    codigosProdutos.Add(pedidoProduto.Codigo);

                    dynamic listaONUs = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaONUs"));

                    if (listaONUs != null)
                    {
                        foreach (var onu in listaONUs)
                        {
                            if (codigo == (string)onu.ONU.CodigoProdutoEmbarcador)
                            {
                                int codigoONU = 0;
                                int.TryParse((string)onu.ONU.Codigo, out codigoONU);

                                Dominio.Entidades.Embarcador.Pedidos.PedidoProdutoONU produtoONU;
                                if (codigoONU > 0)
                                    produtoONU = repPedidoProdutoONU.BuscarPorCodigo(codigoONU);
                                else
                                    produtoONU = new Dominio.Entidades.Embarcador.Pedidos.PedidoProdutoONU();

                                int codigoClassificacaoRiscoONU = 0;
                                int.TryParse((string)onu.ONU.CodigoClassificacaoRiscoONU, out codigoClassificacaoRiscoONU);
                                if (codigoClassificacaoRiscoONU > 0)
                                    produtoONU.ClassificacaoRiscoONU = repClassificacaoRiscoONU.BuscarPorCodigo(codigoClassificacaoRiscoONU);

                                produtoONU.Observacao = (string)onu.ONU.Observacao;
                                produtoONU.PedidoProduto = pedidoProduto;

                                if (codigoONU > 0)
                                    repPedidoProdutoONU.Atualizar(produtoONU);
                                else
                                    repPedidoProdutoONU.Inserir(produtoONU);

                                codigosONUs.Add(produtoONU.Codigo);
                            }
                        }
                    }

                    pedido.MaiorAlturaProdutoEmCentimetros = Math.Max(pedido.MaiorAlturaProdutoEmCentimetros, pedidoProduto.AlturaCM);
                    pedido.MaiorLarguraProdutoEmCentimetros = Math.Max(pedido.MaiorLarguraProdutoEmCentimetros, pedidoProduto.LarguraCM);
                    pedido.MaiorComprimentoProdutoEmCentimetros = Math.Max(pedido.MaiorComprimentoProdutoEmCentimetros, pedidoProduto.ComprimentoCM);
                    pedido.MaiorVolumeProdutoEmCentimetros = Math.Max(pedido.MaiorVolumeProdutoEmCentimetros, (pedidoProduto.AlturaCM + pedidoProduto.LarguraCM + pedidoProduto.ComprimentoCM));
                }
            }

            produtoPredominante = produtoPredominante.Trim();

            if (produtoPredominante.Length >= 150)
                produtoPredominante = produtoPredominante.Substring(0, 149);

            if (!string.IsNullOrWhiteSpace(produtoPredominante))
                pedido.ProdutoPredominante = produtoPredominante;

            if (qtdVolumes > 0 && volumesAlterados)
            {
                pedido.QtVolumes = qtdVolumes;
                pedido.SaldoVolumesRestante = qtdVolumes;
            }

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoProdutoONU> listaPedidoProdutoONU = repPedidoProdutoONU.BuscarPorPedido(pedido.Codigo);

            codigosONUs = listaPedidoProdutoONU.Where(obj => !codigosONUs.Contains(obj.Codigo)).Select(obj => obj.Codigo).ToList();

            for (int j = 0; j < codigosONUs.Count; j++)
            {
                Dominio.Entidades.Embarcador.Pedidos.PedidoProdutoONU deletar = repPedidoProdutoONU.BuscarPorCodigo(codigosONUs[j]);
                repPedidoProdutoONU.Deletar(deletar);
            }

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> listaPedidoProduto = repPedidoProduto.BuscarPorPedido(pedido.Codigo);

            codigosProdutos = listaPedidoProduto.Where(obj => !codigosProdutos.Contains(obj.Codigo)).Select(obj => obj.Codigo).ToList();

            if (codigosProdutos.Count > 0)
                pedido.ItensAtualizados = true;

            for (int j = 0; j < codigosProdutos.Count; j++)
            {
                Dominio.Entidades.Embarcador.Pedidos.PedidoProduto deletar = repPedidoProduto.BuscarPorCodigo(codigosProdutos[j]);
                repPedidoProduto.Deletar(deletar);
            }
        }

        private void SalvarCtesParciaisPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoCTeParcial repositorioPedidoCTeParcial = new Repositorio.Embarcador.Pedidos.PedidoCTeParcial(unitOfWork);
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParcial> listaCteParcialExistentes = repositorioPedidoCTeParcial.BuscarPorPedido(pedido.Codigo);

            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParcial cteParcial in listaCteParcialExistentes)
                repositorioPedidoCTeParcial.Deletar(cteParcial);

            dynamic ctesParciais = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("CtesParciais"));

            if (ctesParciais.Count > 0)
            {
                foreach (var cteParcial in ctesParciais)
                {
                    Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParcial pedidoCteParcial = new Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParcial()
                    {
                        Pedido = pedido,
                        Numero = ((string)cteParcial.Numero).ToInt()
                    };

                    repositorioPedidoCTeParcial.Inserir(pedidoCteParcial);
                }
            }
        }

        private string SalvarNotasParciaisPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Repositorio.UnitOfWork unitOfWork)
        {
            string retorno = "";
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoNotaParcial repositorioPedidoNotaParcial = new Repositorio.Embarcador.Pedidos.PedidoNotaParcial(unitOfWork);
            Servicos.Embarcador.Pedido.OcorrenciaPedido servOcorrenciaPedido = new Servicos.Embarcador.Pedido.OcorrenciaPedido(unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoNotaParcial> listaNotaParcialExistente = repositorioPedidoNotaParcial.BuscarPorPedido(pedido.Codigo);

            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoNotaParcial notaParcial in listaNotaParcialExistente)
                repositorioPedidoNotaParcial.Deletar(notaParcial);

            dynamic notasParciais = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("NotasParciais"));

            if (notasParciais.Count > 0)
            {
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoNotaParcial> pedidosAdd = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoNotaParcial>();
                foreach (var notaParcial in notasParciais)
                {
                    int numeroNota = ((string)notaParcial.NumeroNFe).ToInt();
                    Dominio.Entidades.Embarcador.Pedidos.PedidoNotaParcial pedidoNotaParcialExiste = repositorioPedidoNotaParcial.BuscarPorIntegradaOutroPedido(pedido.Remetente?.CPF_CNPJ ?? 0, numeroNota, pedido.Filial?.Codigo ?? 0);

                    if (pedidoNotaParcialExiste != null)
                    {
                        if (pedidoNotaParcialExiste.Pedido.CargasPedido.Count > 0)
                            return string.Format(Localization.Resources.Pedidos.Pedido.NotaFiscalFoiIntegrado, numeroNota.ToString());
                        else
                        {
                            pedidoNotaParcialExiste.Pedido.SituacaoPedido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Cancelado;
                            repPedido.Atualizar(pedidoNotaParcialExiste.Pedido);

                            servOcorrenciaPedido.ProcessarOcorrenciaPedido(EventoColetaEntrega.PedidoCancelado, pedido, ConfiguracaoEmbarcador, this.Cliente);
                        }
                    }

                    if (!pedidosAdd.Any(obj => obj.Numero == numeroNota))
                    {
                        Dominio.Entidades.Embarcador.Pedidos.PedidoNotaParcial pedidoNotaParcial = new Dominio.Entidades.Embarcador.Pedidos.PedidoNotaParcial()
                        {
                            Pedido = pedido,
                            Numero = numeroNota,
                            NumeroPedido = "",
                            DataCriacao = DateTime.Now
                        };

                        pedidosAdd.Add(pedidoNotaParcial);

                        repositorioPedidoNotaParcial.Inserir(pedidoNotaParcial);
                    }
                }
            }
            return retorno;

        }

        private string SalvarDadosMultimodal(ref Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, ref Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco pedidoEnderecoDestino, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoComponenteFrete repPedidoComponenteFrete = new Repositorio.Embarcador.Pedidos.PedidoComponenteFrete(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoTransbordo repPedidoTransbordo = new Repositorio.Embarcador.Pedidos.PedidoTransbordo(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoViagemNavio repPedidoViagemNavio = new Repositorio.Embarcador.Pedidos.PedidoViagemNavio(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoEmpresaResponsavel repPedidoEmpresaResponsavel = new Repositorio.Embarcador.Pedidos.PedidoEmpresaResponsavel(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCentroCusto repPedidoCentroCusto = new Repositorio.Embarcador.Pedidos.PedidoCentroCusto(unitOfWork);
            Repositorio.Embarcador.Pedidos.Navio repNavio = new Repositorio.Embarcador.Pedidos.Navio(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoTerminalImportacao repTipoTerminalImportacao = new Repositorio.Embarcador.Pedidos.TipoTerminalImportacao(unitOfWork);
            Repositorio.Embarcador.Pedidos.Container repContainer = new Repositorio.Embarcador.Pedidos.Container(unitOfWork);
            Repositorio.Embarcador.Pedidos.ContainerTipo repContainerTipo = new Repositorio.Embarcador.Pedidos.ContainerTipo(unitOfWork);

            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Pedidos.Porto repPorto = new Repositorio.Embarcador.Pedidos.Porto(unitOfWork);
            Servicos.Embarcador.Pedido.Pedido serPedido = new Servicos.Embarcador.Pedido.Pedido(unitOfWork);

            double recebedor = Request.GetDoubleParam("Recebedor");

            if (recebedor > 0)
            {
                pedido.Recebedor = repCliente.BuscarPorCPFCNPJ(recebedor);
                serPedido.PreecherEnderecoPedido(ref pedidoEnderecoDestino, pedido.Recebedor);
            }
            else
                pedido.Recebedor = null;

            if (pedidoEnderecoDestino != null && pedidoEnderecoDestino.Localidade != null)
            {
                pedido.Destino = pedidoEnderecoDestino.Localidade;
                pedido.EnderecoDestino = pedidoEnderecoDestino;
            }

            double terceiro = 0;
            double.TryParse(Request.Params("Terceiro"), out terceiro);
            if (terceiro > 0)
            {
                pedido.Terceiro = repCliente.BuscarPorCPFCNPJ(terceiro);
            }
            else
                pedido.Terceiro = null;

            double provedorOS = 0;
            double.TryParse(Request.Params("ProvedorOS"), out provedorOS);
            if (provedorOS > 0)
            {
                pedido.ProvedorOS = repCliente.BuscarPorCPFCNPJ(provedorOS);
            }
            else
                pedido.ProvedorOS = null;

            int portoDestino = 0;
            int.TryParse(Request.Params("PortoDestino"), out portoDestino);
            if (portoDestino > 0)
            {
                pedido.PortoDestino = repPorto.BuscarPorCodigo(portoDestino);
            }
            else
                pedido.PortoDestino = null;

            int pedidoViagemNavio = 0;
            int.TryParse(Request.Params("PedidoViagemNavio"), out pedidoViagemNavio);
            if (pedidoViagemNavio > 0)
            {
                pedido.PedidoViagemNavio = repPedidoViagemNavio.BuscarPorCodigo(pedidoViagemNavio);
            }
            else
                pedido.PedidoViagemNavio = null;

            int pedidoEmpresaResponsavel = 0;
            int.TryParse(Request.Params("PedidoEmpresaResponsavel"), out pedidoEmpresaResponsavel);
            if (pedidoEmpresaResponsavel > 0)
            {
                pedido.PedidoEmpresaResponsavel = repPedidoEmpresaResponsavel.BuscarPorCodigo(pedidoEmpresaResponsavel);
            }
            else
                pedido.PedidoEmpresaResponsavel = null;

            int pedidoCentroCusto = 0;
            int.TryParse(Request.Params("PedidoCentroCusto"), out pedidoCentroCusto);
            if (pedidoCentroCusto > 0)
            {
                pedido.PedidoCentroCusto = repPedidoCentroCusto.BuscarPorCodigo(pedidoCentroCusto);
            }
            else
                pedido.PedidoCentroCusto = null;

            int navio = 0;
            int.TryParse(Request.Params("Navio"), out navio);
            if (navio > 0)
            {
                pedido.Navio = repNavio.BuscarPorCodigo(navio);
            }
            else
                pedido.Navio = null;

            int terminalOrigem = 0;
            int.TryParse(Request.Params("TerminalOrigem"), out terminalOrigem);
            if (terminalOrigem > 0)
            {
                pedido.TerminalOrigem = repTipoTerminalImportacao.BuscarPorCodigo(terminalOrigem);
            }
            else
                pedido.TerminalOrigem = null;

            int terminalDestino = 0;
            int.TryParse(Request.Params("TerminalDestino"), out terminalDestino);
            if (terminalDestino > 0)
            {
                pedido.TerminalDestino = repTipoTerminalImportacao.BuscarPorCodigo(terminalDestino);
            }
            else
                pedido.TerminalDestino = null;

            int container = 0;
            int.TryParse(Request.Params("Container"), out container);
            if (container > 0)
            {
                pedido.Container = repContainer.BuscarPorCodigo(container);
            }
            else
                pedido.Container = null;

            int containerTipoReserva = 0;
            int.TryParse(Request.Params("ContainerTipoReserva"), out containerTipoReserva);
            if (containerTipoReserva > 0)
            {
                pedido.ContainerTipoReserva = repContainerTipo.BuscarPorCodigo(containerTipoReserva);
            }
            else
                pedido.ContainerTipoReserva = null;

            if (Request.Params("LacreContainerUmMultimodal").Length > 0)
                pedido.LacreContainerUm = Request.GetStringParam("LacreContainerUmMultimodal");

            if (Request.Params("LacreContainerDoisMultimodal").Length > 0)
                pedido.LacreContainerDois = Request.GetStringParam("LacreContainerDoisMultimodal");

            if (Request.Params("LacreContainerTresMultimodal").Length > 0)
                pedido.LacreContainerTres = Request.GetStringParam("LacreContainerTresMultimodal");

            if (Request.Params("TaraContainerMultimodal").Length > 0)
                pedido.TaraContainer = Request.GetStringParam("TaraContainerMultimodal");

            int idBAF = 0;
            int.TryParse(Request.Params("IDBAF"), out idBAF);
            pedido.IDBAF = idBAF;

            decimal valorAdValorem = 0;
            decimal.TryParse(Request.Params("ValorAdValorem"), out valorAdValorem);
            pedido.ValorAdValorem = valorAdValorem;
            decimal valorBAF = 0;
            decimal.TryParse(Request.Params("ValorBAF"), out valorBAF);
            pedido.ValorBAF = valorBAF;

            pedido.NumeroBooking = Request.GetStringParam("NumeroBooking");
            pedido.NumeroOS = Request.GetStringParam("NumeroOS");
            pedido.NumeroProposta = Request.GetStringParam("NumeroProposta");

            pedido.NecessitaAverbacaoAutomatica = Request.GetBoolParam("NecessitaAverbacaoAutomatica");
            pedido.PossuiCargaPerigosa = Request.GetBoolParam("PossuiCargaPerigosa");
            pedido.PedidoLiberadoPortalRetira = Request.GetBoolParam("PedidoLiberadoPortalRetira");
            pedido.ContemCargaRefrigerada = Request.GetBoolParam("ContemCargaRefrigerada");
            pedido.NecessitaEnergiaContainerRefrigerado = Request.GetBoolParam("NecessitaEnergiaContainerRefrigerado");
            pedido.ValidarDigitoVerificadorContainer = Request.GetBoolParam("ValidarDigitoVerificadorContainer");
            pedido.PedidoSVM = Request.GetBoolParam("PedidoSVM");
            pedido.PedidoDeSVMTerceiro = Request.GetBoolParam("PedidoDeSVMTerceiro");
            pedido.ContainerADefinir = Request.GetBoolParam("ContainerADefinir");

            if (!pedido.PedidoDeSVMTerceiro && pedido.TipoOperacao != null && pedido.TipoOperacao.TipoServicoMultimodal == TipoServicoMultimodal.VinculadoMultimodalTerceiro)
                pedido.PedidoDeSVMTerceiro = true;
            else if (!pedido.PedidoDeSVMTerceiro && pedido.GrupoPessoas != null && pedido.GrupoPessoas.TipoServicoMultimodal == TipoServicoMultimodal.VinculadoMultimodalTerceiro)
                pedido.PedidoDeSVMTerceiro = true;
            else if (!pedido.PedidoDeSVMTerceiro && pedido.Remetente != null && pedido.Remetente.TipoServicoMultimodal == TipoServicoMultimodal.VinculadoMultimodalTerceiro)
                pedido.PedidoDeSVMTerceiro = true;

            if (pedido.PedidoDeSVMTerceiro)
                pedido.PedidoSVM = false;

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.DirecaoViagemMultimodal direcaoViagemMultimodal;
            Enum.TryParse(Request.Params("DirecaoViagemMultimodal"), out direcaoViagemMultimodal);
            pedido.DirecaoViagemMultimodal = direcaoViagemMultimodal;

            Dominio.Enumeradores.FormaAverbacaoCTE formaAverbacaoCTE;
            Enum.TryParse(Request.Params("FormaAverbacaoCTE"), out formaAverbacaoCTE);
            pedido.FormaAverbacaoCTE = formaAverbacaoCTE;

            pedido.ValorFreteFilialEmissora = pedido.ValorFreteNegociado;
            pedido.ValorFreteAReceber = pedido.ValorFreteNegociado;
            pedido.ValorFreteToneladaNegociado = pedido.ValorFreteToneladaNegociado;
            pedido.ValorFreteToneladaTerceiro = pedido.ValorFreteToneladaTerceiro;

            string retorno = "";

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo> pedidosTransbordos = repPedidoTransbordo.BuscarPorPedido(pedido.Codigo);

            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo pedidoTransbordoExistente in pedidosTransbordos)
                repPedidoTransbordo.Deletar(pedidoTransbordoExistente);

            dynamic dynTransbordo = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("GridTransbordo"));
            if (dynTransbordo.Count > 0)
            {
                foreach (var di in dynTransbordo)
                {
                    int sequenciaTransbordo = 0;
                    int.TryParse((string)di.Transbordo.SequenciaTransbordo, out sequenciaTransbordo);

                    int codigoNavio = 0;
                    int.TryParse((string)di.Transbordo.CodigoNavioTransbordo, out codigoNavio);

                    int codigoPedidoViagemNavio = 0;
                    int.TryParse((string)di.Transbordo.CodigoPedidoViagemNavioTransbordo, out codigoPedidoViagemNavio);

                    int codigoTerminalTransbordo = 0;
                    int.TryParse((string)di.Transbordo.CodigoTerminalTransbordo, out codigoTerminalTransbordo);

                    int codigoPorto = 0;
                    int.TryParse((string)di.Transbordo.CodigoPortoTransbordo, out codigoPorto);

                    Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo transbordo = new Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo();

                    transbordo.Pedido = pedido;
                    transbordo.Sequencia = sequenciaTransbordo;
                    transbordo.Navio = codigoNavio > 0 ? repNavio.BuscarPorCodigo(codigoNavio) : null;
                    transbordo.Porto = codigoPorto > 0 ? repPorto.BuscarPorCodigo(codigoPorto) : null;
                    transbordo.Terminal = codigoTerminalTransbordo > 0 ? repTipoTerminalImportacao.BuscarPorCodigo(codigoTerminalTransbordo) : null;
                    transbordo.PedidoViagemNavio = codigoPedidoViagemNavio > 0 ? repPedidoViagemNavio.BuscarPorCodigo(codigoPedidoViagemNavio) : null;

                    if (transbordo.Navio == null && transbordo.PedidoViagemNavio != null && transbordo.PedidoViagemNavio.Navio != null)
                        transbordo.Navio = transbordo.PedidoViagemNavio.Navio;

                    repPedidoTransbordo.Inserir(transbordo);
                }
            }

            //Componentes
            //List<Dominio.Entidades.Embarcador.Pedidos.PedidoComponenteFrete> pedidosComponenteFrete = repPedidoComponenteFrete.BuscarPorPedido(pedido.Codigo, false);
            //foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoComponenteFrete componente in pedidosComponenteFrete)
            //{
            //    repPedidoComponenteFrete.Deletar(componente);
            //}

            //dynamic dynComponente = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("GridComponente"));
            //if (dynComponente.Count > 0)
            //{
            //    foreach (var di in dynComponente)
            //    {
            //        int codigoComponente = 0;
            //        int.TryParse((string)di.Componente.CodigoComponente, out codigoComponente);

            //        decimal valor = 0;
            //        decimal.TryParse((string)di.Componente.Valor, out valor);

            //        Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete = repComponenteFrete.BuscarPorCodigo(codigoComponente);
            //        Dominio.Entidades.Embarcador.Pedidos.PedidoComponenteFrete pedidoComponenteFrete = new Dominio.Entidades.Embarcador.Pedidos.PedidoComponenteFrete();

            //        pedidoComponenteFrete.ComponenteFrete = componenteFrete;

            //        if (componenteFrete.ImprimirOutraDescricaoCTe)
            //            pedidoComponenteFrete.OutraDescricaoCTe = componenteFrete.DescricaoCTe;

            //        pedidoComponenteFrete.ComponenteFilialEmissora = false;
            //        pedidoComponenteFrete.Pedido = pedido;
            //        pedidoComponenteFrete.TipoComponenteFrete = componenteFrete.TipoComponenteFrete;

            //        bool incluirICMSFreteInformadoManualmente = ConfiguracaoEmbarcador.IncluirICMSFreteInformadoManualmente;
            //        if (ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal)
            //        {
            //            incluirICMSFreteInformadoManualmente = ConfiguracaoEmbarcador.IncluirICMSFreteInformadoManualmente;
            //            if (pedido.TipoOperacao != null && pedido.TipoOperacao.NaoIncluirICMSFrete)
            //                incluirICMSFreteInformadoManualmente = false;
            //        }

            //        if (pedidoComponenteFrete.TipoComponenteFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ADVALOREM)
            //        {
            //            pedidoComponenteFrete.TipoValor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.PercentualSobreValorNotaFiscal;
            //            pedidoComponenteFrete.IncluirBaseCalculoICMS = incluirICMSFreteInformadoManualmente;
            //            pedidoComponenteFrete.Percentual = valor;
            //            pedidoComponenteFrete.ValorComponente = (valor * pedido.ValorTotalNotasFiscais);
            //        }
            //        else
            //        {
            //            pedidoComponenteFrete.IncluirBaseCalculoICMS = incluirICMSFreteInformadoManualmente;
            //            pedidoComponenteFrete.Percentual = 0;
            //            pedidoComponenteFrete.ValorComponente = valor;
            //        }

            //        repPedidoComponenteFrete.Inserir(pedidoComponenteFrete);
            //    }
            //}

            //Destinatários Bloqueados
            if (pedido.DestinatariosBloqueados == null)
                pedido.DestinatariosBloqueados = new List<string>();
            else
                pedido.DestinatariosBloqueados.Clear();
            var dynDestinatarioBloqueado = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("GridDestinatarioBloqueado"));
            foreach (var destinatarioBloqueado in dynDestinatarioBloqueado)
            {
                pedido.DestinatariosBloqueados.Add((string)destinatarioBloqueado.CNPJCPFDestinatarioBloqueado);
            }

            if (string.IsNullOrWhiteSpace(retorno) && pedido.ValorBAF > 0)
                SalvarComponenteMultimodal(pedido, unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.OUTROS, ref retorno, pedido.ValorBAF);
            if (string.IsNullOrWhiteSpace(retorno) && pedido.ValorAdValorem > 0)
                SalvarComponenteMultimodal(pedido, unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ADVALOREM, ref retorno, pedido.ValorAdValorem);

            if (pedido.Container != null && pedido.Container.ContainerTipo != null && pedido.ContainerTipoReserva != null && !string.IsNullOrWhiteSpace(pedido.Container.ContainerTipo.CodigoIntegracao) && !string.IsNullOrWhiteSpace(pedido.ContainerTipoReserva.CodigoIntegracao))
            {
                if (pedido.Container.ContainerTipo.CodigoIntegracao != pedido.ContainerTipoReserva.CodigoIntegracao)
                {
                    retorno = Localization.Resources.Pedidos.Pedido.NaoEPossivelSelecionarUmContainerComTipo;
                }
            }

            return retorno;
        }

        private string SalvarDIPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoImportacao repPedidoImportacao = new Repositorio.Embarcador.Pedidos.PedidoImportacao(unitOfWork);
            string retorno = "";

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoImportacao> pedidoImportacaos = repPedidoImportacao.BuscarPorPedido(pedido.Codigo);

            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoImportacao pedidoImportacaoExistente in pedidoImportacaos)
                repPedidoImportacao.Deletar(pedidoImportacaoExistente);

            dynamic dynDI = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("GridDI") ?? string.Empty);
            if (dynDI != null && dynDI.Count > 0)
            {
                foreach (var di in dynDI)
                {
                    decimal valorCarga = 0;
                    decimal.TryParse((string)di.DI.ValorCarga, out valorCarga);
                    decimal volume = 0;
                    decimal.TryParse((string)di.DI.Volume, out volume);
                    decimal peso = 0;
                    decimal.TryParse((string)di.DI.Peso, out peso);

                    string numeroDI = (string)di.DI.NumeroDI;
                    string codigoImportacao = (string)di.DI.CodigoImportacao;
                    string codigoReferencia = (string)di.DI.CodigoReferencia;

                    Dominio.Entidades.Embarcador.Pedidos.PedidoImportacao importacao = new Dominio.Entidades.Embarcador.Pedidos.PedidoImportacao();
                    importacao.Pedido = pedido;
                    importacao.CodigoImportacao = codigoImportacao;
                    importacao.CodigoReferencia = codigoReferencia;
                    importacao.NumeroDI = numeroDI;
                    importacao.Peso = peso;
                    importacao.ValorCarga = valorCarga;
                    importacao.Volume = volume;

                    repPedidoImportacao.Inserir(importacao);
                }
            }
            return retorno;
        }

        private string AdicionarProdutosPeloProdutoPredominante(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Repositorio.UnitOfWork unitOfWork)
        {
            //todo: rever essa regra do produto do pedido, criar algo que possa ser adicionado o produto 
            string retorno = "";

            Servicos.ProdutoEmbarcador servicoProdutoEmbarcador = new Servicos.ProdutoEmbarcador(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoProduto repPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(unitOfWork);

            if (carga != null)
            {
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedidoProduto repCargaPedidoProduto = new Repositorio.Embarcador.Cargas.CargaPedidoProduto(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCargaEPedido(carga.Codigo, pedido.Codigo);
                if (cargaPedido != null)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidoProdutos = repCargaPedidoProduto.BuscarPorCargaPedido(cargaPedido.Codigo);
                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto cargaPedidoProduto in cargaPedidoProdutos)
                        repCargaPedidoProduto.Deletar(cargaPedidoProduto);
                }
                else
                {
                    retorno = Localization.Resources.Pedidos.Pedido.NaoFoiPossivelLocalizarVinculo;
                }
            }
            return retorno;
        }

        private void PreecherOutroEnderecoPedido(ref Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco pedidoEndereco)
        {
            pedidoEndereco.Bairro = pedidoEndereco.ClienteOutroEndereco.Bairro;
            pedidoEndereco.CEP = pedidoEndereco.ClienteOutroEndereco.CEP;
            pedidoEndereco.Localidade = pedidoEndereco.ClienteOutroEndereco.Localidade;

            pedidoEndereco.Complemento = pedidoEndereco.ClienteOutroEndereco.Complemento;
            pedidoEndereco.Endereco = pedidoEndereco.ClienteOutroEndereco.Endereco;
            pedidoEndereco.Numero = pedidoEndereco.ClienteOutroEndereco.Numero;
            pedidoEndereco.Telefone = pedidoEndereco.ClienteOutroEndereco.Telefone;
            pedidoEndereco.IE_RG = pedidoEndereco.ClienteOutroEndereco.IE_RG;
        }

        private bool VerificarRegrasPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedido> listaFiltrada = Servicos.Embarcador.Pedido.Pedido.VerificarRegrasPedido(pedido, unitOfWork);

            if (listaFiltrada.Count() > 0)
            {
                RemoverRegrasAntigas(pedido, unitOfWork);
                Servicos.Embarcador.Pedido.Pedido.CriarRegrasAutorizacao(listaFiltrada, pedido, this.Usuario, tipoServicoMultisoftware, _conexao.StringConexao, unitOfWork);
                return true;
            }

            return false;
        }

        private string TituloRegra(Dominio.Entidades.Embarcador.Pedidos.PedidoAutorizacao regra)
        {
            return regra.RegrasPedido?.Descricao;
        }

        private Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSolicitacaoCredito SituacaoSolicitacao(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            if (pedido.SituacaoPedido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.AgAprovacao || pedido.SituacaoPedido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.AutorizacaoPendente)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSolicitacaoCredito.AgLiberacao;
            else if (pedido.SituacaoPedido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Rejeitado)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSolicitacaoCredito.Rejeitado;
            else if (pedido.SituacaoPedido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Finalizado)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSolicitacaoCredito.Utilizado;
            else
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSolicitacaoCredito.Todos;
        }

        private Models.Grid.Grid GridConsultarAutorizacoes(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.Pedido repPedidos = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoAutorizacao repPedidoAutorizacao = new Repositorio.Embarcador.Pedidos.PedidoAutorizacao(unitOfWork);

            int codOcorrencia = int.Parse(Request.Params("Codigo"));

            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Usuário", "Usuario", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Situação", "Situacao", 5, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Regra", false);
            grid.AdicionarCabecalho("Data", false);
            grid.AdicionarCabecalho("Motivo", false);
            grid.AdicionarCabecalho("Justificativa", false);
            grid.AdicionarCabecalho("DT_RowColor", false);
            grid.AdicionarCabecalho("DT_FontColor", false);

            string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoAutorizacao> listaPedidoAutorizacao = repPedidoAutorizacao.ConsultarAutorizacoesPorPedido(codOcorrencia, propOrdenacao, grid.dirOrdena, grid.inicio, grid.limite);
            grid.setarQuantidadeTotal(repPedidoAutorizacao.ContarConsultarAutorizacoesPorPedido(codOcorrencia));

            var lista = (from obj in listaPedidoAutorizacao
                         select new
                         {
                             obj.Codigo,
                             Situacao = obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Pendente ? Localization.Resources.Gerais.Geral.Pendente : obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Aprovada ? Localization.Resources.Pedidos.Pedido.Aprovada : obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Rejeitada ? Localization.Resources.Pedidos.Pedido.Rejeitada : string.Empty,
                             Usuario = obj.Usuario?.Nome,
                             Regra = TituloRegra(obj),
                             Data = obj.Data != null ? obj.Data.ToString() : string.Empty,
                             Motivo = !string.IsNullOrWhiteSpace(obj.Motivo) ? obj.Motivo : string.Empty,
                             Justificativa = obj.Motivo,
                             DT_RowColor = obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Aprovada ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Verde : obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Rejeitada ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Vermelho : obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Pendente ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Amarelo : "",
                             DT_FontColor = obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Rejeitada ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Branco : ""
                         }).ToList();
            grid.AdicionaRows(lista);

            return grid;
        }

        private byte[] GerarRelatorioEmbarcador(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Repositorio.UnitOfWork unitOfWork, out string msg, bool planoViagem, int codigoCarga)
        {
            msg = "";
            var report = ReportRequest.WithType(ReportType.GerarRelatorioEmbarcador)
                .WithExecutionType(ExecutionType.Sync)
                .AddExtraData("codigoPedido", pedido == null ? -1 : pedido.Codigo)
                .AddExtraData("planoViagem", planoViagem)
                .AddExtraData("codigoCarga", codigoCarga)
                .AddExtraData("CodigoUsuario", Usuario.Codigo)
                .CallReport();

            if (!string.IsNullOrWhiteSpace(report.ErrorMessage))
                msg = report.ErrorMessage;

            return report.GetContentFile();
        }

        private bool ValidarCotacaoTabelaFrete(Repositorio.UnitOfWork unitOfWork, double cpfCnpjDestinatario)
        {
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoDeCarga repTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
            Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
            Repositorio.RotaFrete repRotaFrete = new Repositorio.RotaFrete(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);

            DateTime? dataInicialColeta = null;
            if (DateTime.TryParseExact(Request.Params("DataInicialColeta"), "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime dataInicialColetaAux))
                dataInicialColeta = dataInicialColetaAux;

            DateTime? dataPrevisaoSaida = null;
            if (DateTime.TryParseExact(Request.Params("DataPrevisaoSaida"), "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime dataPrevisaoSaidaAux))
                dataPrevisaoSaida = dataPrevisaoSaidaAux;

            DateTime? dataColeta = null;
            if (DateTime.TryParseExact(Request.Params("DataColeta"), "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime dataColetaAux))
                dataColeta = dataColetaAux;

            DateTime? dataPrevisaoEntrega = null;
            if (DateTime.TryParseExact(Request.Params("PrevisaoEntrega"), "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime dataPrevisaoEntregaAux))
                dataPrevisaoEntrega = dataPrevisaoEntregaAux;

            if (dataColeta.HasValue && !dataInicialColeta.HasValue)
                dataInicialColeta = dataColeta;

            DateTime? dataInicialViagemFaturada = Request.GetNullableDateTimeParam("DataInicialViagemFaturada");
            DateTime? dataFinalViagemFaturada = Request.GetNullableDateTimeParam("DataFinalViagemFaturada");

            if (!dataInicialViagemFaturada.HasValue)
                dataInicialViagemFaturada = dataPrevisaoSaida;

            if (!dataFinalViagemFaturada.HasValue)
                dataFinalViagemFaturada = dataPrevisaoEntrega;

            bool.TryParse(Request.Params("DespachoTransitoAduaneiro"), out bool despachoTransitoAduaneiro);
            bool.TryParse(Request.Params("Escolta"), out bool escolta);
            bool.TryParse(Request.Params("GerenciamentoRisco"), out bool gerenciamentoRisco);
            bool.TryParse(Request.Params("NecessarioReentrega"), out bool necessarioReentrega);
            bool.TryParse(Request.Params("Rastreado"), out bool rastreado);

            int.TryParse(Request.Params("Filial"), out int filial);
            int.TryParse(Request.Params("ModeloVeicularCarga"), out int modeloVeicularCarga);
            int.TryParse(Request.Params("QtdEntregas"), out int qtdEntregas);
            int.TryParse(Request.Params("Pallets"), out int numeroPaletes);
            int.TryParse(Request.Params("TipoCarga"), out int tipoCarga);
            int.TryParse(Request.Params("TipoOperacao"), out int tipoOperacao);
            int.TryParse(Request.Params("Veiculo"), out int veiculo);
            int quantidadeEscolta = Request.GetIntParam("QtdEscolta");

            decimal.TryParse(Request.Params("QtdAjudantes"), out decimal qtdAjudantes);
            decimal.TryParse(Request.Params("PalletsFracionado"), out decimal numeroPaletesFracionado);
            decimal.TryParse(Request.Params("PesoTotalCarga"), out decimal pesoTotal);
            decimal.TryParse(Request.Params("CubagemTotal"), out decimal cubagemTotal);
            decimal.TryParse(Request.Params("ValorTotalNotasFiscais"), out decimal valorTotalNotasFiscais);

            double.TryParse(Request.Params("Remetente"), out double CPFCNPJremetente);
            double.TryParse(Request.Params("Tomador"), out double CPFCNPJTomador);
            double cpfCnpjClienteDeslocamento = Request.GetDoubleParam("ClienteDeslocamento");

            Enum.TryParse(Request.Params("TipoPagamento"), out Dominio.Enumeradores.TipoPagamento tipoPagamento);

            double.TryParse(Request.Params("Expedidor"), out double CPFCNPJexpedidor);
            double.TryParse(Request.Params("Recebedor"), out double CPFCNPJrecebedor);


            Dominio.Entidades.Cliente remetente = CPFCNPJremetente > 0 ? repCliente.BuscarPorCPFCNPJ(CPFCNPJremetente) : null;
            Dominio.Entidades.Cliente clienteDeslocamento = cpfCnpjClienteDeslocamento > 0 ? repCliente.BuscarPorCPFCNPJ(cpfCnpjClienteDeslocamento) : null;
            Dominio.Entidades.Cliente destinatario = cpfCnpjDestinatario > 0 ? repCliente.BuscarPorCPFCNPJ(cpfCnpjDestinatario) : null;
            Dominio.Entidades.Cliente tomador = CPFCNPJTomador > 0 ? repCliente.BuscarPorCPFCNPJ(CPFCNPJTomador) : null;
            Dominio.Entidades.Cliente expedidor = CPFCNPJexpedidor > 0 ? repCliente.BuscarPorCPFCNPJ(CPFCNPJexpedidor) : null;
            Dominio.Entidades.Cliente recebedor = CPFCNPJrecebedor > 0 ? repCliente.BuscarPorCPFCNPJ(CPFCNPJrecebedor) : null;

            Dominio.Entidades.Localidade destino = null;
            Dominio.Entidades.RotaFrete rotaFrete = null;
            Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco pedidoEnderecoDestino = new Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco();

            int.TryParse(Request.Params("LocalidadeClienteDestino"), out int localidadeClienteDestino);
            int.TryParse(Request.Params("Destino"), out int codigoDestino);
            if (localidadeClienteDestino > 0)
            {
                Repositorio.Embarcador.Pessoas.ClienteOutroEndereco repClienteOutroEndereco = new Repositorio.Embarcador.Pessoas.ClienteOutroEndereco(unitOfWork);
                pedidoEnderecoDestino.ClienteOutroEndereco = repClienteOutroEndereco.BuscarPorCodigo(localidadeClienteDestino);

                PreecherOutroEnderecoPedido(ref pedidoEnderecoDestino);
            }
            else
                pedidoEnderecoDestino.Localidade = repLocalidade.BuscarPorCodigo(codigoDestino);

            if (pedidoEnderecoDestino.Localidade != null)
                destino = pedidoEnderecoDestino.Localidade;

            if (destino != null)
            {
                rotaFrete = repRotaFrete.BuscarPorLocalidade(destino, true);
                if (rotaFrete == null)
                    rotaFrete = repRotaFrete.BuscarPorEstado(destino.Estado.Sigla, true);
            }

            int volumesPedidoProdutos = 0;
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                dynamic listaProdutos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaProdutos"));
                if (listaProdutos != null)
                {
                    foreach (var produto in listaProdutos)
                    {
                        decimal.TryParse((string)produto.Produto.Quantidade, out decimal quantidade);
                        volumesPedidoProdutos += (int)quantidade;
                    }
                }

            }

            Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametrosCalculo = new Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete()
            {
                DataColeta = dataInicialColeta,
                DataFinalViagem = dataFinalViagemFaturada,
                DataInicialViagem = dataInicialViagemFaturada,
                DataVigencia = DateTime.Now,
                DespachoTransitoAduaneiro = despachoTransitoAduaneiro,
                Empresa = null,
                EscoltaArmada = escolta,
                QuantidadeEscolta = quantidadeEscolta,
                Filial = repFilial.BuscarPorCodigo(filial),
                GerenciamentoRisco = gerenciamentoRisco,
                ModeloVeiculo = repModeloVeicularCarga.BuscarPorCodigo(modeloVeicularCarga),
                NecessarioReentrega = necessarioReentrega,
                NumeroAjudantes = qtdAjudantes,
                NumeroEntregas = qtdEntregas,
                NumeroDeslocamento = Request.GetDecimalParam("ValorDeslocamento"),
                NumeroDiarias = Request.GetDecimalParam("ValorDiaria"),
                NumeroPallets = numeroPaletes + numeroPaletesFracionado,
                Peso = pesoTotal,
                PesoCubado = cubagemTotal,
                PesoPaletizado = 0,
                PossuiRestricaoTrafego = (remetente != null && remetente.PossuiRestricaoTrafego) || (destinatario != null && destinatario.PossuiRestricaoTrafego),
                QuantidadeNotasFiscais = 1,
                Quantidades = new List<Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFreteQuantidade>()
                {
                    new Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFreteQuantidade()
                    {
                        Quantidade = pesoTotal,
                        UnidadeMedida = Dominio.Enumeradores.UnidadeMedida.KG
                    }
                },
                Rastreado = rastreado,
                Rota = rotaFrete,
                TipoCarga = repTipoDeCarga.BuscarPorCodigo(tipoCarga),
                TipoOperacao = repTipoOperacao.BuscarPorCodigo(tipoOperacao),
                ValorNotasFiscais = valorTotalNotasFiscais,
                Veiculo = repVeiculo.BuscarPorCodigo(veiculo),
                Volumes = volumesPedidoProdutos,
                ModelosUtilizadosEmissao = new List<Dominio.Entidades.ModeloDocumentoFiscal>() { null },
                PagamentoTerceiro = false
            };

            if (rotaFrete != null && rotaFrete.Fronteiras != null && rotaFrete.Fronteiras.Count > 0)
            {
                parametrosCalculo.Fronteiras = new List<Dominio.Entidades.Cliente>();
                parametrosCalculo.Fronteiras = rotaFrete.Fronteiras.Select(c => c.Cliente).ToList();
            }

            parametrosCalculo.Destinatarios = new List<Dominio.Entidades.Cliente>();
            parametrosCalculo.Destinos = new List<Dominio.Entidades.Localidade>();

            if (destinatario != null)
            {
                parametrosCalculo.Destinatarios.Add(destinatario);

                if (recebedor != null)
                {
                    if (recebedor.Localidade != null)
                        parametrosCalculo.Destinos.Add(recebedor.Localidade);
                }
                else
                {
                    if (destinatario.Localidade != null)
                        parametrosCalculo.Destinos.Add(destinatario.Localidade);
                }
            }

            if (expedidor != null)
                parametrosCalculo.Origens = expedidor != null ? new List<Dominio.Entidades.Localidade>() { expedidor.Localidade } : new List<Dominio.Entidades.Localidade>();
            else
                parametrosCalculo.Origens = remetente != null ? new List<Dominio.Entidades.Localidade>() { remetente.Localidade } : new List<Dominio.Entidades.Localidade>();

            if (clienteDeslocamento != null)
                parametrosCalculo.Remetentes = clienteDeslocamento != null ? new List<Dominio.Entidades.Cliente>() { clienteDeslocamento } : new List<Dominio.Entidades.Cliente>();
            else
                parametrosCalculo.Remetentes = remetente != null ? new List<Dominio.Entidades.Cliente>() { remetente } : new List<Dominio.Entidades.Cliente>();

            Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal = null;

            if (remetente != null && remetente.Localidade.Codigo == codigoDestino)
                modeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorTipoDocumento(Dominio.Enumeradores.TipoDocumento.NFSe);
            else
                modeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorTipoDocumento(Dominio.Enumeradores.TipoDocumento.CTe);

            if (modeloDocumentoFiscal != null)
                parametrosCalculo.QuantidadeEmissoesPorModeloDocumento.Add(modeloDocumentoFiscal, 1);

            if (tipoPagamento == Dominio.Enumeradores.TipoPagamento.Pago)
            {
                parametrosCalculo.GrupoPessoas = remetente?.GrupoPessoas;
                parametrosCalculo.Tomador = remetente;
            }
            else if (tipoPagamento == Dominio.Enumeradores.TipoPagamento.A_Pagar)
            {
                parametrosCalculo.GrupoPessoas = destinatario?.GrupoPessoas;
                parametrosCalculo.Tomador = destinatario;
            }
            else
            {
                parametrosCalculo.GrupoPessoas = tomador?.GrupoPessoas;
                parametrosCalculo.Tomador = tomador;
            }

            Servicos.Embarcador.Carga.FreteCliente svcFreteCliente = new Servicos.Embarcador.Carga.FreteCliente(unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dadosCalculoFrete = new Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete();

            List<Dominio.Entidades.Embarcador.Frete.TabelaFrete> tabelasFrete = Servicos.Embarcador.Carga.Frete.ObterTabelasFrete(parametrosCalculo, false, out StringBuilder mensagem, unitOfWork, TipoServicoMultisoftware, 0);
            if (Servicos.Embarcador.Carga.Frete.ValidarQuantidadeTabelaFreteDisponivel(ref dadosCalculoFrete, tabelasFrete))
            {
                Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente = svcFreteCliente.ObterTabelasFrete(ref mensagem, parametrosCalculo, tabelasFrete[0], TipoServicoMultisoftware).FirstOrDefault();

                if (tabelaFreteCliente != null)
                    return true;
            }

            return false;
        }

        private Models.Grid.Grid ObterGridHistoricoPedido(List<Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega> pedidoOcorrenciaColetaEntrega)
        {
            try
            {

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.Pedido.Data, "Data", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.Pedido.Historico, "Historico", 40, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.Pedido.Local, "Local", 40, Models.Grid.Align.left, false);


                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                var listaRetornar = (
                    from historico in pedidoOcorrenciaColetaEntrega
                    select new
                    {
                        Codigo = historico.Codigo,
                        Data = historico.DataOcorrencia.ToString("dd/MM/yyyy HH:mm"),
                        Historico = Servicos.Embarcador.GestaoEntregas.NotificaoEntrega.ObterDescricaoPortalOcorrencia(historico.TipoDeOcorrencia, historico.Carga, historico.Pedido.Destinatario, historico.Pedido.Remetente),
                        Local = historico.Alvo?.Descricao ?? ""
                    }
                ).ToList();

                listaRetornar.Skip(parametrosConsulta.InicioRegistros);
                listaRetornar.Take(parametrosConsulta.LimiteRegistros);

                var totalRegistros = listaRetornar.Count();

                grid.AdicionaRows(listaRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                throw;
            }
        }

        private Models.Grid.Grid ObterGridPedidosPendentes()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("CodigoDestinatario", false);
            grid.AdicionarCabecalho("CodigoFilial", false);
            grid.AdicionarCabecalho("VolumesEnviar", false);
            grid.AdicionarCabecalho("SKU", false);
            grid.AdicionarCabecalho("TipoCarga", false);
            grid.AdicionarCabecalho("TipoCargaCodigo", false);
            grid.AdicionarCabecalho(Localization.Resources.Pedidos.Pedido.DescricaoPedido, "NumeroPedidoEmbarcador", 12, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho(Localization.Resources.Pedidos.Pedido.Modalidade, "Modalidade", 12, Models.Grid.Align.center, false, TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor);
            grid.AdicionarCabecalho(Localization.Resources.Pedidos.Pedido.TipoOperacao, "TipoOperacao", 18, Models.Grid.Align.left, false, TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor);
            grid.AdicionarCabecalho(Localization.Resources.Pedidos.Pedido.DescricaoFilial, "DescricaoFilial", 18, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Pedidos.Pedido.QtdCaixas, "QtVolumes", 15, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Pedidos.Pedido.QuantidadeDeProdutos, "QtProdutos", 15, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Pedidos.Pedido.DataInicioJanela, "DataInicioJanelaDescarga", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Pedidos.Pedido.DataFimJanela, "DataFimJanelaDescarga", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Pedidos.Pedido.Saldo, "Saldo", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Pedidos.Pedido.GrupoProduto, "GrupoProduto", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Pedidos.Pedido.Categoria, "Categoria", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("CNPJRemetente", false);
            grid.AdicionarCabecalho(Localization.Resources.Pedidos.Pedido.CodigosProdutos, "CodigoIntegracaoProduto", 10, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Pedidos.Pedido.DescricaoProdutos, "DescricaoProduto", 10, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("UsarLayoutAgendamentoPorCaixaItem", false);

            return grid;
        }

        private dynamic ExecutarPesquisaPendentes(ref int totalRegistros, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametros, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.AlteracaoPedidoProdutoAgendamento repAlteracaoPedidoProdutoAgendamento = new Repositorio.Embarcador.Pedidos.AlteracaoPedidoProdutoAgendamento(unitOfWork);
            Repositorio.Embarcador.Logistica.AgendamentoColetaPedidoProduto repAgendamentoColetaPedidoProduto = new Repositorio.Embarcador.Logistica.AgendamentoColetaPedidoProduto(unitOfWork);

            parametros.PropriedadeOrdenar = "DataCriacao";

            double cnpjRemetete = 0;
            int grupoPessoa = 0;
            string numeroPedidos = Request.GetStringParam("NumeroPedidoFiltro").Replace(" ", "");
            double cnpjDestinatario = Request.GetDoubleParam("DestinatarioFiltro");
            List<string> pedidos = string.IsNullOrWhiteSpace(numeroPedidos) ? new List<string>() : numeroPedidos.Split(',').ToList();
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
            {
                grupoPessoa = IsCompartilharAcessoEntreGrupoPessoas() && Usuario.ClienteFornecedor.GrupoPessoas != null ? Usuario.ClienteFornecedor.GrupoPessoas.Codigo : 0;
                cnpjRemetete = grupoPessoa <= 0 && Usuario.ClienteFornecedor != null ? Usuario.ClienteFornecedor.CPF_CNPJ : 0;
            }

            totalRegistros = repositorioPedido.ContarBuscarPendentesPorRemetente(cnpjRemetete, cnpjDestinatario, grupoPessoa, pedidos);

            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> listaPedido = totalRegistros > 0 ? repositorioPedido.BuscarPendentesPorRemetente(cnpjRemetete, cnpjDestinatario, grupoPessoa, pedidos, parametros) : new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();

            List<double> codigosDestinatarios = listaPedido
                .Where(obj => obj.Destinatario != null)
                .Select(obj => obj.Destinatario.CPF_CNPJ)
                .Distinct()
                .ToList();

            List<Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedidoProdutoAgendamento> listaPedidoProdutoAgendamento = new List<Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedidoProdutoAgendamento>();
            List<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento> centrosDescarregamento = new List<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento>();
            List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedidoProduto> listAgendamentoColetaPedidoProduto = new List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedidoProduto>();

            if (totalRegistros > 0)
            {
                centrosDescarregamento = new Repositorio.Embarcador.Logistica.CentroDescarregamento(unitOfWork).BuscarPorDestinatarios(codigosDestinatarios);

                for (int i = 0; i < totalRegistros; i += 2000)
                {
                    List<int> codigosPedidos = listaPedido.Select(x => x.Codigo).Skip(i).Take(2000).ToList();
                    listaPedidoProdutoAgendamento.AddRange(repAlteracaoPedidoProdutoAgendamento.BuscarPorAlteracaoPedidosNaoVinculado(codigosPedidos));
                    listAgendamentoColetaPedidoProduto.AddRange(repAgendamentoColetaPedidoProduto.BuscarPorListaCodigoPedidoAgendado(codigosPedidos));
                }
            }

            var listaPedidoRetornar = (
                from pedido in listaPedido
                select new Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoPendente()
                {
                    Codigo = pedido.Codigo,
                    NumeroPedidoEmbarcador = pedido.NumeroPedidoEmbarcador,
                    TipoOperacao = pedido.TipoOperacao?.Descricao ?? "",
                    DescricaoFilial = (from o in centrosDescarregamento where o.Destinatario.Codigo == pedido.Destinatario?.Codigo orderby o.Filial != null && o.Filial.Codigo == pedido.Filial?.Codigo descending select o.Descricao).FirstOrDefault() ?? "",
                    UsarLayoutAgendamentoPorCaixaItem = (from o in centrosDescarregamento where o.Destinatario.Codigo == pedido.Destinatario?.Codigo orderby o.Filial != null && o.Filial.Codigo == pedido.Filial?.Codigo descending select o.UsarLayoutAgendamentoPorCaixaItem)?.FirstOrDefault() ?? false,
                    CodigoDestinatario = pedido.Destinatario?.Codigo ?? 0,
                    CodigoFilial = pedido.Filial?.Codigo ?? 0,
                    TipoCarga = pedido.TipoDeCarga?.Descricao ?? "",
                    TipoCargaCodigo = pedido.TipoDeCarga?.Codigo ?? 0,
                    QtVolumes = pedido.QtVolumes,
                    Saldo = pedido.SaldoVolumesRestante,
                    GrupoProduto = (pedido.Produtos == null || !pedido.Produtos.Any()) ? string.Empty : string.Join(", ", pedido.Produtos.Select(x => x.Produto.GrupoProduto.Descricao).Distinct()),
                    Modalidade = pedido.TipoDeCarga?.Descricao ?? "",
                    VolumesEnviar = pedido.QtVolumes,
                    SKU = ObterQuantidadeDeItensPedido(pedido, listAgendamentoColetaPedidoProduto.Where(x => x.PedidoProduto.Pedido.Codigo == pedido.Codigo).ToList(), listaPedidoProdutoAgendamento.Where(x => x.PedidoProduto.Pedido.Codigo == pedido.Codigo).ToList(), unitOfWork),
                    QtProdutos = ObterQuantidadeDeProdutosPedido(pedido, listAgendamentoColetaPedidoProduto.Where(x => x.PedidoProduto.Pedido.Codigo == pedido.Codigo).ToList(), listaPedidoProdutoAgendamento.Where(x => x.PedidoProduto.Pedido.Codigo == pedido.Codigo).ToList(), unitOfWork),
                    DataFimJanelaDescarga = pedido.DataValidade.HasValue ? (pedido.DataValidade.Value.TimeOfDay == DateTime.MinValue.TimeOfDay) ? pedido.DataValidade.Value.Add(DateTime.MaxValue.TimeOfDay).ToString("dd/MM/yyyy HH:mm") : pedido.DataValidade.Value.ToString("dd/MM/yyyy HH:mm") : "",
                    DataCriacao = pedido.DataCriacao?.ToString("dd/MM/yyyy") ?? "",
                    DataInicioJanelaDescarga = pedido.DataInicioJanelaDescarga?.ToString("dd/MM/yyyy HH:mm") ?? "",
                    Categoria = pedido.ProdutoPrincipal?.Descricao ?? "",
                    CNPJRemetente = pedido.Remetente?.CPF_CNPJ ?? 0,
                    CodigoIntegracaoProduto = string.Join(", ", pedido.Produtos?.Select(x => x.Produto.CodigoProdutoEmbarcador.ToString())) ?? string.Empty,
                    DescricaoProduto = string.Join(", ", pedido.Produtos?.Select(x => x.Produto.Descricao)) ?? string.Empty
                }
            );

            return listaPedidoRetornar.ToList();
        }

        private void SalvarAcrescimoDesconto(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoAcrescimoDesconto repPedidoAcrescimoDesconto = new Repositorio.Embarcador.Pedidos.PedidoAcrescimoDesconto(unitOfWork);
            Repositorio.Embarcador.Fatura.Justificativa repJustificativa = new Repositorio.Embarcador.Fatura.Justificativa(unitOfWork);

            dynamic dynAcrescimosDescontos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaAcrescimoDesconto"));
            List<int> codigosAcrescimosDescontos = new List<int>();

            foreach (var dynAcrescimoDesconto in dynAcrescimosDescontos)
            {
                int codigo = ((string)dynAcrescimoDesconto.Codigo).ToInt();

                Dominio.Entidades.Embarcador.Pedidos.PedidoAcrescimoDesconto pedidoAcrescimoDesconto = repPedidoAcrescimoDesconto.BuscarPorCodigo(codigo, true);

                if (pedidoAcrescimoDesconto == null)
                    pedidoAcrescimoDesconto = new Dominio.Entidades.Embarcador.Pedidos.PedidoAcrescimoDesconto();

                pedidoAcrescimoDesconto.Pedido = pedido;
                pedidoAcrescimoDesconto.Justificativa = repJustificativa.BuscarPorCodigo(((string)dynAcrescimoDesconto.Justificativa.Codigo).ToInt()) ?? throw new ControllerException("Justificativa é obrigatória");
                pedidoAcrescimoDesconto.TipoJustificativa = pedidoAcrescimoDesconto.Justificativa.TipoJustificativa;
                pedidoAcrescimoDesconto.AplicacaoValor = pedidoAcrescimoDesconto.Justificativa.AplicacaoValorContratoFrete ?? AplicacaoValorJustificativaContratoFrete.NoTotal;
                pedidoAcrescimoDesconto.Observacao = (string)dynAcrescimoDesconto.Observacao;
                pedidoAcrescimoDesconto.Valor = ((string)dynAcrescimoDesconto.Valor).ToDecimal();

                if (pedidoAcrescimoDesconto.Codigo > 0)
                {
                    repPedidoAcrescimoDesconto.Atualizar(pedidoAcrescimoDesconto);

                    List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = pedidoAcrescimoDesconto.GetChanges();
                    if (alteracoes.Count > 0)
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, pedido, alteracoes, string.Format(Localization.Resources.Pedidos.Pedido.AlterouAcrescDescContratoTerceiro, pedidoAcrescimoDesconto.Justificativa.Descricao), unitOfWork);
                }
                else
                {
                    repPedidoAcrescimoDesconto.Inserir(pedidoAcrescimoDesconto);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, pedido, null, string.Format(Localization.Resources.Pedidos.Pedido.AdicionouAcrescDescContratoTerceiro, pedidoAcrescimoDesconto.Justificativa.Descricao), unitOfWork);
                }

                codigosAcrescimosDescontos.Add(pedidoAcrescimoDesconto.Codigo);
            }

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoAcrescimoDesconto> acrescimosDescontosDeletar = repPedidoAcrescimoDesconto.BuscarPorPedido(pedido.Codigo).Where(o => !codigosAcrescimosDescontos.Contains(o.Codigo)).ToList();
            foreach (var acrescimoDescontoDeletar in acrescimosDescontosDeletar)
            {
                Servicos.Auditoria.Auditoria.Auditar(Auditado, pedido, null, string.Format(Localization.Resources.Pedidos.Pedido.RemoveuAcrescDescContratoTerceiro, acrescimoDescontoDeletar.Justificativa.Descricao), unitOfWork);
                repPedidoAcrescimoDesconto.Inserir(acrescimoDescontoDeletar);
            }
        }

        private void SalvarContatos(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoContato repPedidoContato = new Repositorio.Embarcador.Pedidos.PedidoContato(unitOfWork);
            Repositorio.Embarcador.Contatos.TipoContato repTipoContato = new Repositorio.Embarcador.Contatos.TipoContato(unitOfWork);

            dynamic dynContatos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaContatos"));
            List<int> codigosContatos = new List<int>();

            foreach (var dynContato in dynContatos)
            {
                int codigo = ((string)dynContato.Codigo).ToInt();

                Dominio.Entidades.Embarcador.Pedidos.PedidoContato pedidoContato = repPedidoContato.BuscarPorCodigo(codigo, false);

                if (pedidoContato == null)
                    pedidoContato = new Dominio.Entidades.Embarcador.Pedidos.PedidoContato();

                pedidoContato.Pedido = pedido;
                pedidoContato.Contato = (string)dynContato.Contato;
                pedidoContato.Telefone = (string)dynContato.Telefone;
                pedidoContato.CPF = ((string)dynContato.CPF).ObterSomenteNumeros();
                pedidoContato.Ativo = (bool)dynContato.Situacao;
                pedidoContato.Email = (string)dynContato.Email;

                if (pedidoContato.TiposContato == null)
                    pedidoContato.TiposContato = new List<Dominio.Entidades.Embarcador.Contatos.TipoContato>();
                else
                    pedidoContato.TiposContato.Clear();

                if (dynContato.TipoContato != null)
                {
                    foreach (var codigoTipoContato in dynContato.TipoContato)
                        pedidoContato.TiposContato.Add(repTipoContato.BuscarPorCodigo((int)codigoTipoContato));
                }

                if (pedidoContato.Codigo > 0)
                    repPedidoContato.Atualizar(pedidoContato);
                else
                    repPedidoContato.Inserir(pedidoContato);

                codigosContatos.Add(pedidoContato.Codigo);
            }

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoContato> contatosDeletar = repPedidoContato.BuscarPorPedido(pedido.Codigo).Where(o => !codigosContatos.Contains(o.Codigo)).ToList();
            foreach (var contatoDeletar in contatosDeletar)
                repPedidoContato.Deletar(contatoDeletar);
        }

        private void SalvarComponentesFrete(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoComponenteFrete repPedidoComponenteFrete = new Repositorio.Embarcador.Pedidos.PedidoComponenteFrete(unitOfWork);
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);

            dynamic componentesFrete = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaComponentesFrete"));

            if (pedido.PedidosComponente != null && pedido.PedidosComponente.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var componenteFrete in componentesFrete)
                {
                    int codigo = 0;
                    if (componenteFrete.Codigo != null && int.TryParse((string)componenteFrete.Codigo, out codigo))
                        codigos.Add(codigo);
                }

                List<Dominio.Entidades.Embarcador.Pedidos.PedidoComponenteFrete> componentesDeletar = (from obj in pedido.PedidosComponente where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < componentesDeletar.Count; i++)
                {
                    Dominio.Entidades.Embarcador.Pedidos.PedidoComponenteFrete componenteDeletar = componentesDeletar[i];

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, pedido, null, string.Format(Localization.Resources.Pedidos.Pedido.RemoveuComponenteFrete, componenteDeletar.Descricao), unitOfWork);

                    repPedidoComponenteFrete.Deletar(componenteDeletar);
                }
            }

            foreach (var componenteFrete in componentesFrete)
            {
                Dominio.Entidades.Embarcador.Pedidos.PedidoComponenteFrete componenteFretePedido = null;

                int codigo = 0;

                if (componenteFrete.Codigo != null && int.TryParse((string)componenteFrete.Codigo, out codigo))
                    componenteFretePedido = repPedidoComponenteFrete.BuscarPorCodigo(codigo, true);

                if (componenteFretePedido == null)
                    componenteFretePedido = new Dominio.Entidades.Embarcador.Pedidos.PedidoComponenteFrete();

                componenteFretePedido.Pedido = pedido;
                componenteFretePedido.ComponenteFilialEmissora = false;
                componenteFretePedido.ComponenteFrete = repComponenteFrete.BuscarPorCodigo((int)componenteFrete.ComponenteFrete.Codigo);
                componenteFretePedido.TipoComponenteFrete = componenteFretePedido.ComponenteFrete.TipoComponenteFrete;

                if (componenteFretePedido.ComponenteFrete.ImprimirOutraDescricaoCTe)
                    componenteFretePedido.OutraDescricaoCTe = componenteFretePedido.ComponenteFrete.DescricaoCTe;

                bool incluirICMSFreteInformadoManualmente = ConfiguracaoEmbarcador.IncluirICMSFreteInformadoManualmente;

                if (ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal)
                {
                    incluirICMSFreteInformadoManualmente = ConfiguracaoEmbarcador.IncluirICMSFreteInformadoManualmente;
                    if (pedido.TipoOperacao?.NaoIncluirICMSFrete ?? false)
                        incluirICMSFreteInformadoManualmente = false;
                }

                if (componenteFretePedido.ComponenteFrete.TipoValor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.PercentualSobreValorNotaFiscal)
                {
                    componenteFretePedido.TipoValor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.PercentualSobreValorNotaFiscal;
                    componenteFretePedido.IncluirIntegralmenteContratoFreteTerceiro = false;
                    componenteFretePedido.IncluirBaseCalculoICMS = incluirICMSFreteInformadoManualmente;
                    componenteFretePedido.Percentual = Utilidades.Decimal.Converter((string)componenteFrete.Percentual);
                    componenteFretePedido.ValorComponente = (componenteFretePedido.Percentual * pedido.ValorTotalNotasFiscais);
                }
                else
                {
                    componenteFretePedido.IncluirIntegralmenteContratoFreteTerceiro = false;
                    componenteFretePedido.IncluirBaseCalculoICMS = incluirICMSFreteInformadoManualmente;
                    componenteFretePedido.Percentual = 0;
                    componenteFretePedido.ValorComponente = Utilidades.Decimal.Converter((string)componenteFrete.Valor);
                }

                if (componenteFretePedido.Codigo > 0)
                {
                    repPedidoComponenteFrete.Atualizar(componenteFretePedido);

                    List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = componenteFretePedido.GetChanges();

                    if (alteracoes.Count > 0)
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, pedido, alteracoes, string.Format(Localization.Resources.Pedidos.Pedido.AlterouComponenteFrente, componenteFretePedido.Descricao), unitOfWork);
                }
                else
                {
                    repPedidoComponenteFrete.Inserir(componenteFretePedido);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, pedido, null, string.Format(Localization.Resources.Pedidos.Pedido.AdicionouComponenteFrente, componenteFretePedido.Descricao), unitOfWork);
                }
            }
        }

        private void SalvarPedidoAdicionais(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoAdicional repositorioPedidoAdicionais = new Repositorio.Embarcador.Pedidos.PedidoAdicional(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Produtos.TipoEmbalagem repositorioTipoEmbalagem = new Repositorio.Embarcador.Produtos.TipoEmbalagem(unitOfWork);
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.PedidoAdicional pedidoAdicionais = repositorioPedidoAdicionais.BuscarPorPedido(pedido.Codigo);

            if (pedidoAdicionais == null)
                pedidoAdicionais = new Dominio.Entidades.Embarcador.Pedidos.PedidoAdicional() { Pedido = pedido };
            else
                pedidoAdicionais.Initialize();

            double CodigoClientePropostaComercial = Request.GetDoubleParam("ClientePropostaComercial");
            int codigoNumeroPedidoOrigem = Request.GetIntParam("NumeroPedidoOrigem");
            double cpfCnpjNotificacaoCRT = Request.GetDoubleParam("NotificacaoCRT");
            int codigoTipoEmbalagem = Request.GetIntParam("TipoEmbalagem");

            pedidoAdicionais.NumeroPedidoICT = Request.GetStringParam("NumeroPedidoICT");
            pedidoAdicionais.CondicaoExpedicao = Request.GetStringParam("CondicaoExpedicao");
            pedidoAdicionais.GrupoFreteMaterial = Request.GetStringParam("GrupoFreteMaterial");
            pedidoAdicionais.RestricaoEntrega = Request.GetStringParam("RestricaoEntrega");
            pedidoAdicionais.DataCriacaoRemessa = Request.GetNullableDateTimeParam("DataCriacaoRemessa");
            pedidoAdicionais.DataCriacaoVenda = Request.GetNullableDateTimeParam("DataCriacaoVenda");
            pedidoAdicionais.IndicadorPOF = Request.GetStringParam("IndicadorPOF");
            pedidoAdicionais.ISISReturn = Request.GetIntParam("ISISReturn");
            pedidoAdicionais.IndicativoColetaEntrega = Request.GetEnumParam<IndicativoColetaEntrega>("IndicativoColetaEntrega");
            pedidoAdicionais.TipoServico = Request.GetStringParam("TipoServico");
            pedidoAdicionais.NumeroAutorizacaoColetaEntrega = Request.GetStringParam("NumeroAutorizacaoColetaEntrega");
            pedidoAdicionais.ClientePropostaComercial = repositorioCliente.BuscarPorCPFCNPJ(CodigoClientePropostaComercial);
            pedidoAdicionais.TipoSeguro = Request.GetStringParam("TipoSeguro");
            pedidoAdicionais.NumeroOSMae = Request.GetStringParam("NumeroOSMae");
            pedidoAdicionais.ExecaoCab = Request.GetBoolParam("ExecaoCab");
            pedidoAdicionais.PedidoPaletizado = Request.GetBoolParam("PedidoPaletizado");
            pedidoAdicionais.EssePedidopossuiPedidoBonificacao = Request.GetBoolParam("EssePedidopossuiPedidoBonificacao");
            pedidoAdicionais.EssePedidopossuiPedidoVenda = Request.GetBoolParam("EssePedidopossuiPedidoVenda");
            pedidoAdicionais.NumeroPedidoVinculado = Request.GetStringParam("NumeroPedidoVinculado");
            pedidoAdicionais.AjudanteCarga = Request.GetBoolParam("AjudanteCarga");
            pedidoAdicionais.QtdAjudantesCarga = Request.GetIntParam("QtdAjudantesCarga");
            pedidoAdicionais.AjudanteDescarga = Request.GetBoolParam("AjudanteDescarga");
            pedidoAdicionais.QtdAjudantesDescarga = Request.GetIntParam("QtdAjudantesDescarga");
            pedidoAdicionais.PedidoOrigem = repositorioPedido.BuscarPorCodigo(codigoNumeroPedidoOrigem);
            pedidoAdicionais.Incoterm = Request.GetEnumParam<EnumIncotermPedido>("Incoterm");
            pedidoAdicionais.TransitoAduaneiro = Request.GetEnumParam<TransitoAduaneiro>("TransitoAduaneiro");
            pedidoAdicionais.NotificacaoCRT = cpfCnpjNotificacaoCRT > 0D ? repositorioCliente.BuscarPorCPFCNPJ(cpfCnpjNotificacaoCRT) : null;
            pedidoAdicionais.DtaRotaPrazoTransporte = Request.GetNullableStringParam("DtaRotaPrazoTransporte");
            pedidoAdicionais.TipoEmbalagem = codigoTipoEmbalagem > 0 ? repositorioTipoEmbalagem.BuscarPorCodigo(codigoTipoEmbalagem) : null;
            pedidoAdicionais.DetalheMercadoria = Request.GetNullableStringParam("DetalheMercadoria");

            pedido.PossuiIsca = Request.GetBoolParam("PossuiIsca");
            pedido.QtdIsca = Request.GetIntParam("QtdIsca");
            pedido.PedidoBloqueado = Request.GetBoolParam("PedidoBloqueado");
            pedido.CategoriaOS = Request.GetNullableEnumParam<CategoriaOS>("CategoriaOS");
            pedido.TipoOS = Request.GetNullableEnumParam<TipoOS>("TipoOS");
            pedido.TipoOSConvertido = Request.GetNullableEnumParam<TipoOSConvertido>("TipoOSConvertido");

            if (pedidoAdicionais.Codigo > 0)
            {
                repositorioPedidoAdicionais.Atualizar(pedidoAdicionais, Auditado);
                pedido.SetExternalChanges(pedidoAdicionais.GetCurrentChanges());
            }
            else
            {
                if (pedidoAdicionais.QtdAjudantesCarga > 0 || pedidoAdicionais.QtdAjudantesDescarga > 0)
                    repositorioPedidoAdicionais.Inserir(pedidoAdicionais);

                if (string.IsNullOrEmpty(pedidoAdicionais.NumeroPedidoICT) && string.IsNullOrEmpty(pedidoAdicionais.CondicaoExpedicao) && string.IsNullOrEmpty(pedidoAdicionais.GrupoFreteMaterial) && string.IsNullOrEmpty(pedidoAdicionais.RestricaoEntrega) &&
                    pedidoAdicionais.DataCriacaoRemessa == null && pedidoAdicionais.DataCriacaoVenda == null && string.IsNullOrEmpty(pedidoAdicionais.IndicadorPOF) && pedidoAdicionais.IndicativoColetaEntrega == IndicativoColetaEntrega.NaoInformado &&
                    string.IsNullOrEmpty(pedidoAdicionais.TipoServico) && string.IsNullOrEmpty(pedidoAdicionais.NumeroAutorizacaoColetaEntrega) && pedidoAdicionais.ClientePropostaComercial == null && string.IsNullOrEmpty(pedidoAdicionais.TipoSeguro) &&
                    string.IsNullOrEmpty(pedidoAdicionais.NumeroOSMae) && string.IsNullOrEmpty(pedidoAdicionais.NumeroPedidoVinculado) && !pedidoAdicionais.EssePedidopossuiPedidoVenda && !pedidoAdicionais.EssePedidopossuiPedidoBonificacao &&
                    (pedidoAdicionais.ISISReturn == 0) && !pedidoAdicionais.TransitoAduaneiro.HasValue && !pedidoAdicionais.Incoterm.HasValue && pedidoAdicionais.NotificacaoCRT == null)
                    return;

                repositorioPedidoAdicionais.Inserir(pedidoAdicionais);
            }
        }

        private void SalvarPedidoEcommerce(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoEcommerce repositorioPedidoEcommerce = new Repositorio.Embarcador.Pedidos.PedidoEcommerce(unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.PedidoEcommerce pedidoEcommerce = repositorioPedidoEcommerce.BuscarPorPedido(pedido.Codigo);

            if (pedidoEcommerce == null)
                pedidoEcommerce = new Dominio.Entidades.Embarcador.Pedidos.PedidoEcommerce() { Pedido = pedido };

            pedidoEcommerce.AlturaPedido = Request.GetDecimalParam("AlturaPedido");
            pedidoEcommerce.LarguaPedido = Request.GetDecimalParam("LarguaPedido");
            pedidoEcommerce.ComprimentoPedido = Request.GetDecimalParam("ComprimentoPedido");
            pedidoEcommerce.DiametroPedido = Request.GetDecimalParam("DiametroPedido");
            pedidoEcommerce.CategoriaPrincipalProduto = Request.GetStringParam("CategoriaPrincipalProduto");
            pedidoEcommerce.SerieNFe = Request.GetStringParam("SerieNFe");
            pedidoEcommerce.ChaveAcessoNFe = Request.GetStringParam("ChaveAcessoNFe");
            pedidoEcommerce.NaturezaGeralMercadorias = Request.GetStringParam("NaturezaGeralMercadorias");
            pedidoEcommerce.TipoGeralMercadorias = Request.GetStringParam("TipoGeralMercadorias");
            pedidoEcommerce.PrazoEntregaLoja = Request.GetIntParam("PrazoEntregaLoja");
            pedidoEcommerce.TipoFrete = Request.GetStringParam("TipoFrete");
            pedidoEcommerce.DataPagamentoPedido = Request.GetNullableDateTimeParam("DataPagamentoPedido");
            pedidoEcommerce.ModalidadeEntrega = Request.GetStringParam("ModalidadeEntrega");
            pedidoEcommerce.CodigoTabelaFreteSistemaFIS = Request.GetIntParam("CodigoTabelaFreteSistemaFIS");
            pedidoEcommerce.CFOPPredominanteNFe = Request.GetStringParam("CFOPPredominanteNFe");

            if (pedidoEcommerce.Codigo > 0)
                repositorioPedidoEcommerce.Atualizar(pedidoEcommerce);
            else
                repositorioPedidoEcommerce.Inserir(pedidoEcommerce);
        }

        private dynamic CriarObjetoDetalheOutroEndereco(PedidoEndereco pedidoEndereco)
        {
            string endereco = pedidoEndereco.ClienteOutroEndereco.Endereco ?? "";
            if (!string.IsNullOrWhiteSpace(pedidoEndereco.ClienteOutroEndereco.Numero)) endereco += ", " + pedidoEndereco.ClienteOutroEndereco.Numero;
            if (!string.IsNullOrWhiteSpace(pedidoEndereco.ClienteOutroEndereco.Bairro)) endereco += " - " + pedidoEndereco.ClienteOutroEndereco.Bairro;
            if (!string.IsNullOrWhiteSpace(pedidoEndereco.ClienteOutroEndereco.Complemento)) endereco += " (" + pedidoEndereco.ClienteOutroEndereco.Complemento + ")";

            string local = (pedidoEndereco.ClienteOutroEndereco.Localidade?.DescricaoCidadeEstado ?? "") + ", " + (pedidoEndereco.ClienteOutroEndereco.Localidade?.Pais?.Nome ?? "");
            if (pedidoEndereco.ClienteOutroEndereco.CEP != "") local += " CEP: " + pedidoEndereco.ClienteOutroEndereco.CEP;

            return new
            {
                Endereco = endereco,
                Localidade = local ?? "",
                CidadePolo = pedidoEndereco.ClienteOutroEndereco.Localidade?.LocalidadePolo?.DescricaoCidadeEstado ?? "",
                Telefone = pedidoEndereco.Telefone.ObterTelefoneFormatado(),
            };
        }

        /// <summary>
        /// Quando o pedido é criado pelo método AdicionarCarga no Cargas.svc, 
        /// tem casos que ele pode estar sem produtos. Se não tiver nenhum, é criado uma pendência
        /// com uma MensagemAlerta, que é removida agora.
        /// </summary>
        private void RemoverPendenciaDeProdutoSeNecessario(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Repositorio.UnitOfWork unitOfWork)
        {
            dynamic listaProdutos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaProdutos"));

            if ((listaProdutos == null) || (listaProdutos.Count == 0))
                return;

            Servicos.Embarcador.Carga.MensagemAlertaCarga servicoMensagemAlerta = new Servicos.Embarcador.Carga.MensagemAlertaCarga(unitOfWork);

            foreach (var carga in pedido.CargasPedido)
                servicoMensagemAlerta.Remover(carga, TipoMensagemAlerta.CargaSemProdutos);
        }

        private dynamic ObterStagesPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, int cargaDt = 0)
        {
            List<SituacaoCarga> situacaoNaoPermitidas = new List<SituacaoCarga>()
            {
                SituacaoCarga.Anulada,
                SituacaoCarga.Cancelada
            };

            if (cargaDt > 0)
            {
                return (from obj in pedido.StagesPedido
                        where !situacaoNaoPermitidas.Contains(obj.Stage.CargaDT.SituacaoCarga) && obj.Stage.CargaDT.Codigo == cargaDt
                        select new
                        {
                            obj.Stage.Codigo,
                            obj.Stage.NumeroStage,
                            Expedidor = obj?.Stage?.Expedidor?.Descricao ?? string.Empty,
                            Recebedor = obj?.Stage?.Recebedor?.Descricao ?? string.Empty,
                            ModeloVeicularCarga = obj?.Stage?.ModeloVeicularCarga?.Descricao ?? string.Empty,
                            obj.Stage?.Distancia,
                            obj.Stage?.OrdemEntrega,
                            TipoModal = obj?.Stage?.TipoModal.ObterDescricao() ?? string.Empty,
                            CanalEntrega = obj?.Stage?.CanalEntrega?.Descricao ?? string.Empty,
                            CanalVenda = obj?.Stage?.CanalVenda?.Descricao ?? string.Empty,
                            RelevanciaCusto = obj?.Stage?.RelevanciaCusto ?? false ? "Sim" : "Não",
                            TipoPercurso = obj?.Stage?.TipoPercurso.ObterDescricao() ?? string.Empty,
                            obj.Stage?.Agrupamento,
                            obj.Stage?.NumeroVeiculo,
                            NaoPossuiValePedagio = obj?.Stage?.NaoPossuiValePedagio ?? false ? "Sim" : "Não",
                            StatusVP = obj.Stage.StatusVPEmbarcador
                        }).ToList();
            }
            else
            {
                return (from obj in pedido.StagesPedido
                        where !situacaoNaoPermitidas.Contains(obj.Stage.CargaDT.SituacaoCarga)
                        select new
                        {
                            obj.Stage.Codigo,
                            obj.Stage.NumeroStage,
                            Expedidor = obj?.Stage?.Expedidor?.Descricao ?? string.Empty,
                            Recebedor = obj?.Stage?.Recebedor?.Descricao ?? string.Empty,
                            ModeloVeicularCarga = obj?.Stage?.ModeloVeicularCarga?.Descricao ?? string.Empty,
                            obj.Stage?.Distancia,
                            obj.Stage?.OrdemEntrega,
                            TipoModal = obj?.Stage?.TipoModal.ObterDescricao() ?? string.Empty,
                            CanalEntrega = obj?.Stage?.CanalEntrega?.Descricao ?? string.Empty,
                            CanalVenda = obj?.Stage?.CanalVenda?.Descricao ?? string.Empty,
                            RelevanciaCusto = obj?.Stage?.RelevanciaCusto ?? false ? "Sim" : "Não",
                            TipoPercurso = obj?.Stage?.TipoPercurso.ObterDescricao() ?? string.Empty,
                            obj.Stage?.Agrupamento,
                            obj.Stage?.NumeroVeiculo,
                            NaoPossuiValePedagio = obj?.Stage?.NaoPossuiValePedagio ?? false ? "Sim" : "Não",
                            StatusVP = obj.Stage.StatusVPEmbarcador
                        }).ToList();
            }
        }

        private void RemoverRegrasAntigas(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoAutorizacao repPedidoAutorizacao = new Repositorio.Embarcador.Pedidos.PedidoAutorizacao(unitOfWork);
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoAutorizacao> regraAutorizacaoPedido = repPedidoAutorizacao.BuscarAutorizacoesPorPedido(pedido.Codigo);

            if (regraAutorizacaoPedido != null && regraAutorizacaoPedido.Count > 0)
            {
                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoAutorizacao regra in regraAutorizacaoPedido)
                    repPedidoAutorizacao.Deletar(regra);
            }

        }


        private Models.Grid.Grid ObterGridHistoricoFronteirasPedido(List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> fronteiras)
        {
            try
            {

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.Pedido.Fronteira, "Descricao", 20, Models.Grid.Align.left, false);


                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                var listaRetornar = (
                    from entrega in fronteiras
                    select new
                    {
                        Codigo = entrega.Codigo,
                        Descricao = entrega.Cliente.Descricao,
                    }
                ).ToList();

                listaRetornar.Skip(parametrosConsulta.InicioRegistros);
                listaRetornar.Take(parametrosConsulta.LimiteRegistros);

                var totalRegistros = listaRetornar.Count();

                grid.AdicionaRows(listaRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                throw;
            }
        }

        private bool ValidarTabelaFrete(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Pedidos.Pedido pPedido)
        {
            if (pPedido.TipoOperacao?.ConfiguracaoPedido?.BloquearInclusaoAlteracaoPedidosNaoTenhamTabelaFreteConfigurada ?? false)
            {
                List<double> listaCpfCnpjDestinatario = ObterListaCpfCnpjDestinatario();

                foreach (double cpfCnpjDestinatario in listaCpfCnpjDestinatario)
                {
                    if (!ValidarCotacaoTabelaFrete(unitOfWork, cpfCnpjDestinatario))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool ValidarRegraTomador(Dominio.Enumeradores.TipoTomador tipoTomador, bool usarConfiguracaoEmissao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes tipoEmissaoCTeParticipantes)
        {
            if (tipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor)
            {
                if (!usarConfiguracaoEmissao ||
                    (tipoEmissaoCTeParticipantes != TipoEmissaoCTeParticipantes.ComExpedidor &&
                     tipoEmissaoCTeParticipantes != TipoEmissaoCTeParticipantes.ComExpedidorERecebedor))
                {
                    return false;
                }

            }
            else if (tipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor)
            {
                if (!usarConfiguracaoEmissao ||
                    (tipoEmissaoCTeParticipantes != TipoEmissaoCTeParticipantes.ComRecebedor &&
                     tipoEmissaoCTeParticipantes != TipoEmissaoCTeParticipantes.ComExpedidorERecebedor))
                {
                    return false;
                }
            }
            return true;
        }

        private dynamic ObterDadosRecebedor(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, bool duplicarParaDevolucaoTotal)
        {
            dynamic recebedor;

            if (duplicarParaDevolucaoTotal)
                recebedor = new { Codigo = pedido.Destinatario?.CPF_CNPJ ?? 0d, Descricao = pedido.Destinatario?.Descricao ?? "", codigoIBGE = pedido.Destinatario?.Localidade?.CodigoIBGE.ToString() ?? "" };
            else
                recebedor = new { Codigo = pedido.Recebedor?.CPF_CNPJ ?? 0d, Descricao = pedido.Recebedor?.Descricao ?? "", codigoIBGE = pedido.Recebedor?.Localidade?.CodigoIBGE.ToString() ?? "" };

            return recebedor;
        }

        private dynamic ObterDadosExpedidor(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, bool duplicarParaDevolucaoTotal)
        {
            dynamic expedidor;

            if (duplicarParaDevolucaoTotal)
                expedidor = new { Codigo = pedido.Remetente?.CPF_CNPJ ?? 0d, Descricao = pedido.Remetente?.Descricao ?? "", codigoIBGE = pedido.Remetente?.Localidade?.CodigoIBGE.ToString() ?? "" };
            else
                expedidor = new { Codigo = pedido.Expedidor?.CPF_CNPJ ?? 0d, Descricao = pedido.Expedidor?.Descricao ?? "", codigoIBGE = pedido.Expedidor?.Localidade?.CodigoIBGE.ToString() ?? "" };

            return expedidor;
        }

        private dynamic ObterDadosRemetente(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, bool duplicarParaDevolucaoParcial)
        {
            dynamic remetente;

            if (duplicarParaDevolucaoParcial)
                remetente = new { Codigo = pedido.Destinatario?.CPF_CNPJ ?? 0d, Descricao = pedido.Destinatario?.Descricao ?? "" };
            else
                remetente = new { Codigo = pedido.Remetente?.CPF_CNPJ ?? 0d, Descricao = pedido.Remetente?.Descricao ?? "" };

            return remetente;
        }

        private dynamic ObterDadosDestinatario(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, bool duplicarParaDevolucaoParcial)
        {
            dynamic destinatario;

            if (duplicarParaDevolucaoParcial)
                destinatario = new { Codigo = pedido.Remetente?.CPF_CNPJ ?? 0d, Descricao = pedido.Remetente?.Descricao ?? "" };
            else
                destinatario = new { Codigo = pedido.Destinatario?.CPF_CNPJ ?? 0d, Descricao = pedido.Destinatario?.Descricao ?? "" };

            return destinatario;
        }

        private void AdicionarNotasFiscais(Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoBase, List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosAdicionados, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Pedido.OcorrenciaPedido servOcorrenciaPedido = new Servicos.Embarcador.Pedido.OcorrenciaPedido(unitOfWork);
            bool duplicarPedidoParaDevolucaoTotal = Request.GetBoolParam("DuplicarPedidoParaDevolucaoTotal");

            if (duplicarPedidoParaDevolucaoTotal)
            {
                if (pedidosAdicionados.Count > 0)
                {
                    foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidosAdicionados)
                    {
                        AdicionarXMLNotasFiscaisViculadasNoPedido(pedido, unitOfWork);
                        if (pedido.NotasFiscais.Count > 0)
                            servOcorrenciaPedido.ProcessarOcorrenciaPedido(EventoColetaEntrega.PedidoFaturado, pedido, ConfiguracaoEmbarcador, this.Cliente);
                    }
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(pedidoBase.NumeroControle))
                {
                    if (pedidosAdicionados.Count > 0)
                    {
                        foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidosAdicionados)
                        {
                            AdicionarXMLNotasFiscais(pedido, unitOfWork);
                            if (pedido.NotasFiscais.Count > 0)
                                servOcorrenciaPedido.ProcessarOcorrenciaPedido(EventoColetaEntrega.PedidoFaturado, pedido, ConfiguracaoEmbarcador, this.Cliente);
                        }
                    }
                    else
                    {
                        AdicionarXMLNotasFiscais(pedidoBase, unitOfWork);
                        if (pedidoBase.NotasFiscais.Count > 0)
                            servOcorrenciaPedido.ProcessarOcorrenciaPedido(EventoColetaEntrega.PedidoFaturado, pedidoBase, ConfiguracaoEmbarcador, this.Cliente);
                    }
                }
            }
        }

        private async Task SalvarListaFronteirasAsync(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Repositorio.UnitOfWork _unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoFronteira repositorioPedidoFronteira = new Repositorio.Embarcador.Pedidos.PedidoFronteira(_unitOfWork);
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(_unitOfWork);

            dynamic dynListaFronteiras = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaFronteiras"));

            if (dynListaFronteiras == null)
                return;

            List<double> fronteirasAtuais = await repositorioPedidoFronteira.BuscarCPFCNPJFronteirasPorPedidoAsync(pedido.Codigo);

            if (pedido.Fronteira != null)
            {
                if (!fronteirasAtuais.Contains(pedido.Fronteira.CPF_CNPJ))
                {
                    fronteirasAtuais.Add(pedido.Fronteira.CPF_CNPJ);
                    pedido.Fronteira = null;
                }
            }

            List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade> alteracoes = new List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade>();
            List<double> codigos = new List<double>();

            foreach (dynamic dynFronteira in dynListaFronteiras)
            {
                double codigo = 0;
                double.TryParse((string)dynFronteira.Fronteira.Codigo, out codigo);
                codigos.Add(codigo);
            }

            List<double> fronteirasDeletarCodigos = fronteirasAtuais.Where(o => !codigos.Contains(o)).ToList();

            if (fronteirasDeletarCodigos.Count() > 0)
            {
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoFronteira> fronteirasDeletar = await repositorioPedidoFronteira.BuscarFronteirasPorPedidoCPFCNPJAsync(fronteirasDeletarCodigos, pedido.Codigo);

                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoFronteira fronteira in fronteirasDeletar)
                {
                    alteracoes.Add(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                    {
                        Propriedade = "Fronteiras",
                        De = fronteira.Descricao ?? "",
                        Para = ""
                    });

                    repositorioPedidoFronteira.Deletar(fronteira);
                }
            }

            foreach (dynamic dynFronteira in dynListaFronteiras)
            {
                double codigoFronteira = ((string)dynFronteira.Fronteira.Codigo).ToDouble();
                bool jaExiste = fronteirasAtuais.Contains(codigoFronteira);

                if (!jaExiste)
                {
                    Dominio.Entidades.Embarcador.Pedidos.PedidoFronteira fronteiraInserir = new Dominio.Entidades.Embarcador.Pedidos.PedidoFronteira()
                    {
                        Pedido = pedido,
                        Fronteira = await repositorioCliente.BuscarFronteiraPorCPFCNPJAsync(codigoFronteira)
                    };

                    repositorioPedidoFronteira.Inserir(fronteiraInserir);

                    alteracoes.Add(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                    {
                        Propriedade = "Fronteiras",
                        De = "",
                        Para = fronteiraInserir.Descricao ?? ""
                    });
                }
                pedido.SetExternalChanges(alteracoes);
            }
        }

        #endregion Métodos Privados

        #region Métodos Privados - Importação

        private List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> ObterConfiguracaoImportacaoClientes()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>
            {
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = "CNPJ", Propriedade = "CNPJ", Tamanho = 150, CampoInformacao = true, Obrigatorio = true, Regras = new List<string> { "required" } }
            };

            return configuracoes;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> ObterConfiguracaoImportacaoISISReturn()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>
            {
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = "Número ISIS Return", Propriedade = "NumeroIsisReturn", Tamanho = 50, CampoInformacao = true, Obrigatorio = true, Regras = new List<string> { "required" } },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 2, Descricao = "Número Nota Fiscal", Propriedade = "NumeroNotaFiscal", Tamanho = 50, CampoInformacao = true, Obrigatorio = true, Regras = new List<string> { "required" } }
            };

            return configuracoes;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> ObterConfiguracaoImportacaoDadosNota()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>
            {
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = "Número nota ", Propriedade = "NumeroNota", Tamanho = 200 },
            };

            return configuracoes;
        }

        private List<(string Carga, Dominio.Entidades.Embarcador.Pedidos.MotivoImportacaoPedidoAtrasada Motivo)> ObterListaMotivosAtrasos(dynamic dynMotivos, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.MotivoImportacaoPedidoAtrasada repMotivoImportacaoPedidoAtrasada = new Repositorio.Embarcador.Pedidos.MotivoImportacaoPedidoAtrasada(unitOfWork);

            if (dynMotivos == null)
                return null;

            if (!repMotivoImportacaoPedidoAtrasada.PossuiMotivoAtivo())
                return null;

            List<(string Carga, Dominio.Entidades.Embarcador.Pedidos.MotivoImportacaoPedidoAtrasada Motivo)> motivos = new List<(string Carga, Dominio.Entidades.Embarcador.Pedidos.MotivoImportacaoPedidoAtrasada Motivo)>();

            foreach (var dynMotivo in dynMotivos)
            {
                int codigoMotivo = ((string)dynMotivo.Motivo).ToInt();
                Dominio.Entidades.Embarcador.Pedidos.MotivoImportacaoPedidoAtrasada motivo = repMotivoImportacaoPedidoAtrasada.BuscarPorCodigo(codigoMotivo);
                motivos.Add(ValueTuple.Create((string)dynMotivo.Carga, motivo));
            }

            return motivos;
        }

        private int ObterQuantidadeDeItensPedido(Pedido pedido, List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedidoProduto> listAgendamentoColetaPedidoProduto, List<Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedidoProdutoAgendamento> listaPedidoProdutoAgendamento, Repositorio.UnitOfWork unitOfWork)
        {
            int quantidadeDeItens = 0;
            Servicos.Embarcador.Pedido.PedidoProduto servicoPedidoProduto = new Servicos.Embarcador.Pedido.PedidoProduto(unitOfWork);
            foreach (PedidoProduto pedidoProduto in pedido.Produtos)
                quantidadeDeItens += servicoPedidoProduto.ObterSaldoQuantidadeItensPendentesAgendamentoColeta(pedidoProduto, listAgendamentoColetaPedidoProduto, listaPedidoProdutoAgendamento);
            return quantidadeDeItens;
        }

        private int ObterQuantidadeDeProdutosPedido(Pedido pedido, List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedidoProduto> listAgendamentoColetaPedidoProduto, List<Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedidoProdutoAgendamento> listaPedidoProdutoAgendamento, Repositorio.UnitOfWork unitOfWork)
        {
            int quantidadeDeProdutos = 0;
            Servicos.Embarcador.Pedido.PedidoProduto servicoPedidoProduto = new Servicos.Embarcador.Pedido.PedidoProduto(unitOfWork);
            foreach (PedidoProduto pedidoProduto in pedido.Produtos)
                quantidadeDeProdutos += servicoPedidoProduto.ObterSaldoQuantidadeProdutosPendentesAgendamentoColeta(pedidoProduto, listAgendamentoColetaPedidoProduto, listaPedidoProdutoAgendamento);
            return quantidadeDeProdutos;
        }

        #endregion Métodos Privados - Importação
    }
}
