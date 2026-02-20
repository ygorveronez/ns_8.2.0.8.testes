using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;
using Utilidades.Extensions;
using Dominio.Entidades.Embarcador.Pessoas;
using System.Threading.Tasks;

namespace SGT.WebAdmin.Controllers.Pedidos
{
    [CustomAuthorize("Pedidos/PlanejamentoPedidoTMS")]
    public class PlanejamentoPedidoTMSController : BaseController
    {
		#region Construtores

		public PlanejamentoPedidoTMSController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedido filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                DataPlanejamentoPedidoTMS tipoDataAgrupamento = Request.GetEnumParam<DataPlanejamentoPedidoTMS>("TipoDataAgrupamento");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };
                grid.limite = 200;

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("SituacaoPlanejamentoPedidoTMS", false);
                grid.AdicionarCabecalho("PossuiVeiculo", false);
                grid.AdicionarCabecalho("PossuiMotorista", false);
                grid.AdicionarCabecalho("PossuiModeloVeicular", false);
                grid.AdicionarCabecalho("DataEstado", false);
                grid.AdicionarCabecalho("NumeroCelularMotorista", false);
                grid.AdicionarCabecalho("MotoristaCiente", false);
                grid.AdicionarCabecalho("NecessitaInformarPlacaCarregamento", false);
                grid.AdicionarCabecalho("TipoOperacaoCodigo", false);
                grid.AdicionarCabecalho("CodidoCarga", false);
                grid.AdicionarCabecalho("Carga", "CodigoCargaEmbarcador", 7, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Nº Embarcador", "NumeroPedidoEmbarcador", 5, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Tomador", "Tomador", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Origem", "Origem", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Destino", "Destino", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Destino Recebedor", "DestinoRecebedor", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Modelo Veicular", "ModeloVeicularCarga", 7, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo Propriedade Veículo", "TipoPropriedadeVeiculo", 8, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho("Observação", "ObservacaoInterna", 7, Models.Grid.Align.left, true, false, false, false, true, new Models.Grid.EditableCell(TipoColunaGrid.aString, 150));
                grid.AdicionarCabecalho("Frota", "NumeroFrota", 7, Models.Grid.Align.left, false, false, false, false, true, new Models.Grid.EditableCell(TipoColunaGrid.aString, 10));
                grid.AdicionarCabecalho("Nº Rota", "NumeroRota", 7, Models.Grid.Align.left, false, false, false, false, true, new Models.Grid.EditableCell(TipoColunaGrid.aString, 10));
                grid.AdicionarCabecalho("Motorista", "Motorista", 8, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Veículo", "Veiculo", 8, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Proprietário", "Proprietario", 8, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho("Hora Coleta", "HoraColeta", 7, Models.Grid.Align.center, true, false);
                grid.AdicionarCabecalho("Data Coleta", "DataColeta", 7, Models.Grid.Align.center, true, false);
                grid.AdicionarCabecalho("Tratativa", "Tratativa", 8, Models.Grid.Align.center, false, false);
                grid.AdicionarCabecalho("Data Previsão Saída", "DataPrevisaoSaida", 8, Models.Grid.Align.center, false, false);
                grid.AdicionarCabecalho("Data Previsão Entrega/Retorno", "PrevisaoEntrega", 8, Models.Grid.Align.center, false, false);
                grid.AdicionarCabecalho("Centro Resultado", "CentroResultado", 10, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho("Tipo da Carga", "TipoDeCarga", 10, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho("Peso", "Peso", 8, Models.Grid.Align.right, false, false);
                grid.AdicionarCabecalho("Local de Coleta", "LocalColeta", 10, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho("Local de Entrega", "LocalEntrega", 10, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho("Tipo de Operação", "TipoOperacao", 10, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho("Fronteira do Pedido", "FronteiraPedido", 10, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho("Remetente", "Remetente", 10, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho("Destinatário", "Destinatario", 10, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho("Qtd. Pallets Total", "QtdPallets", 10, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho("Qtd. Pallets Saldo", "QtdSaldoPallets", 10, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho("Qtd. Pallets Carregado", "QtdPalletsCarregado", 10, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho("Gestor", "Gestor", 10, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho("Número Container", "Container", 10, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho("Tipo de Container", "TipoDeContainer", 10, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho("Tara Container", "TaraContainer", 10, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho("Lacre Container 1", "LacreContainerUm", 10, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho("Lacre Container 2", "LacreContainerDois", 10, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho("Lacre Container 3", "LacreContainerTres", 10, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho("Número Booking", "NumeroBooking", 10, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho("Navio", "Navio", 10, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho("Valor total da Mercadoria", "ValorTotalMercadoria", 10, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho("Indicativo Coleta/Entrega", "IndicativoColetaEntrega", 10, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho("Categoria OS", "CategoriaOSDescricao", 10, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho("Tipo OS Convertido", "TipoOSConvertidoDescricao", 10, Models.Grid.Align.left, false, false);

                if (new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork).ExistePorTipo(TipoIntegracao.A52))
                    grid.AdicionarCabecalho("Situação Integracao", "Integracao", 10, Models.Grid.Align.left, false, false);

                grid.AdicionarCabecalho("Pedidos", "NumeroPedido", 10, Models.Grid.Align.left, false, false);

                Models.Grid.GridPreferencias gridPreferencias = new Models.Grid.GridPreferencias(unitOfWork, "PlanejamentoPedidoTMS/Pesquisa", "grid-pesquisa-pedidos");
                grid.AplicarPreferenciasGrid(gridPreferencias.ObterPreferenciaGrid(this.Usuario.Codigo, grid.modelo));

                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenarPlanejamentoPedido);

                if (parametrosConsulta.PropriedadeOrdenar == "Codigo")
                    parametrosConsulta.DirecaoOrdenar = "asc";

                if (tipoDataAgrupamento == DataPlanejamentoPedidoTMS.DataAgendamento)
                    parametrosConsulta.PropriedadeOrdenar = $"CONVERT(VARCHAR, pedido.PED_DATA_AGENDAMENTO, 101), estado.UF_NOME, pais.PAI_NOME, {parametrosConsulta.PropriedadeOrdenar}";
                else
                    parametrosConsulta.PropriedadeOrdenar = $"CONVERT(VARCHAR, pedido.CAR_DATA_CARREGAMENTO_PEDIDO, 101), estado.UF_NOME, pais.PAI_NOME, {parametrosConsulta.PropriedadeOrdenar}";

                if(filtrosPesquisa.UsuarioUtilizaSegregacaoPorProvedor && filtrosPesquisa.CodigosProvedores.Count == 0)
                {
                    grid.AdicionaRows(new List<dynamic>());
                    grid.setarQuantidadeTotal(0);

                    return grid;
                }

                int totalRegistros = repositorioPedido.ContarConsultaPlanejamentoPedido(filtrosPesquisa);

                IList<Dominio.Relatorios.Embarcador.DataSource.Pedidos.PlanejamentoPedidoTMS> listaPedido = totalRegistros > 0 ? repositorioPedido.ConsultarPlanejamentoPedido(filtrosPesquisa, false, parametrosConsulta) : new List<Dominio.Relatorios.Embarcador.DataSource.Pedidos.PlanejamentoPedidoTMS>();

                List<dynamic> listaPedidoRetornar = (
                    from pedido in listaPedido
                    select ObterPedido(pedido, unitOfWork, repositorioPedido, tipoDataAgrupamento)
                ).ToList();

                grid.AdicionaRows(listaPedidoRetornar);
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

        public async Task<IActionResult> ObterDisponibilidade()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Nº Frota", "NumeroFrota", 2, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Reboque", "Reboque", 3, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Motorista", "Motorista", 4, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Modelo de Carga", "ModeloVeicularCarga", 4, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo Carroceria", "TipoCarroceria", 4, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo Plotagem", "TipoPlotagem", 4, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo Propriedade", "Tipo", 3, Models.Grid.Align.left, true, false, false, true, true);
                grid.AdicionarCabecalho("Placa", "PlacaVeiculo", 3, Models.Grid.Align.left, true, false, false, true, false);
                grid.AdicionarCabecalho("CPF Motorista", "CPFMotorista", 3, Models.Grid.Align.left, true, false, false, true, false);

                Models.Grid.GridPreferencias gridPreferencias = new Models.Grid.GridPreferencias(unitOfWork, "PlanejamentoPedidoTMS/ObterDisponibilidade", "grid-planejamento-pedido-disponibilidade");
                grid.AplicarPreferenciasGrid(gridPreferencias.ObterPreferenciaGrid(this.Usuario.Codigo, grid.modelo));

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaVeiculo filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaVeiculo()
                {
                    NumeroFrota = Request.GetStringParam("NumeroFrotaDisponibilidade"),
                    CodigoMotorista = Request.GetIntParam("MotoristaDisponibilidade"),
                    SituacaoVeiculo = SituacaoVeiculo.Disponivel,
                    TipoPropriedade = Request.GetStringParam("TipoPropriedadeVeiculo")
                };

                Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);

                int totalRegistros = repositorioVeiculo.ContarConsultaEmbarcador(filtrosPesquisa);
                List<Dominio.Entidades.Veiculo> listaVeiculo = totalRegistros > 0 ? repositorioVeiculo.ConsultarEmbarcador(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Veiculo>();

                var lista = (from p in listaVeiculo
                             select new
                             {
                                 p.Codigo,
                                 p.NumeroFrota,
                                 Reboque = string.Join(", ", p.VeiculosVinculados.Select(o => o.Placa)),
                                 Motorista = repVeiculoMotorista.BuscarMotoristaPrincipal(p.Codigo)?.Descricao ?? string.Empty,
                                 ModeloVeicularCarga = p.ModeloVeicularCarga?.Descricao ?? string.Empty,
                                 TipoCarroceria = p.DescricaoTipoCarroceria,
                                 TipoPlotagem = p.TipoPlotagem?.Descricao ?? string.Empty,
                                 Tipo = p.DescricaoTipo,
                                 PlacaVeiculo = p.Placa_Formatada,
                                 CPFMotorista = repVeiculoMotorista.BuscarMotoristaPrincipal(p.Codigo)?.CPF ?? string.Empty,
                             }).OrderBy(o => o.NumeroFrota).ToList();

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(lista);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao obter disponibilidade");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> SimularFrete()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Servicos.Embarcador.Carga.ICMS svcICMS = new Servicos.Embarcador.Carga.ICMS(unitOfWork);
            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repositorioPedido.BuscarPorCodigo(codigo);


                DateTime? dataColeta = pedido.DataInicialColeta;
                DateTime? dataFinalViagem = pedido.PrevisaoEntrega;
                DateTime? dataInicialViagem = pedido.DataPrevisaoSaida;
                DateTime dataVigencia = pedido.DataInicialColeta ?? DateTime.MinValue;

                double cnpjClienteAtivo = pedido.Remetente.Codigo;
                double cnpjClienteInativo = 0;
                double cnpjDestinatario = pedido.Destinatario.Codigo;

                int codigoLocalidadeDestino = pedido.Destino?.Codigo ?? 0;
                int codigoEmpresa = 0;
                int codigoLocalidadeOrigem = pedido.Origem?.Codigo ?? 0;
                int codigoGrupoPessoa = pedido.GrupoPessoas?.Codigo ?? 0;
                int codigoModeloVeicular = pedido.ModeloVeicularCarga?.Codigo ?? 0;
                int numeroEntregas = pedido.QtdEntregas;
                int numeroPedidos = 1;
                int quantidadeNotasFiscais = 0;
                int codigoTipoDeCarga = pedido.TipoDeCarga?.Codigo ?? 0;
                int codigoTipoOperacao = pedido.TipoOperacao?.Codigo ?? 0;

                decimal distancia = pedido.Distancia;
                decimal numeroAjudantes = pedido.QtdAjudantes;
                decimal numeroDeslocamento = pedido.ValorDeslocamento ?? 0;
                decimal numeroDiarias = pedido.ValorDiaria ?? 0;
                decimal numeroPallets = pedido.NumeroPaletesFracionado;
                decimal peso = pedido.PesoTotal;
                decimal pesoCubado = pedido.PesoTotal;
                decimal valorNotasFiscais = pedido.ValorTotalNotasFiscais;
                decimal volumes = pedido.QtVolumes;
                decimal pesoTotal = pedido.PesoTotal;

                bool escoltaArmada = pedido.EscoltaArmada;
                bool gerenciamentoRisco = pedido.GerenciamentoRisco;
                bool rastreado = pedido.Rastreado;
                bool pagamentoTerceiro = Request.GetBoolParam("PagamentoTerceiro");

                Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dadosFrete = Servicos.Embarcador.Carga.Frete.CalcularFretePorCotacaoPedido(unitOfWork, _conexao.StringConexao, TipoServicoMultisoftware,
                   dataColeta, dataFinalViagem, dataInicialViagem, dataVigencia, cnpjClienteAtivo, cnpjClienteInativo, codigoLocalidadeDestino, codigoEmpresa,
                   codigoLocalidadeOrigem, codigoGrupoPessoa, codigoModeloVeicular,
                   distancia, escoltaArmada, gerenciamentoRisco, numeroAjudantes, numeroEntregas, numeroDeslocamento, numeroDiarias, numeroPallets, numeroPedidos,
                   peso, pesoCubado, quantidadeNotasFiscais, rastreado, cnpjDestinatario, codigoTipoDeCarga, codigoTipoOperacao,
                   valorNotasFiscais, volumes, pesoTotal, ConfiguracaoEmbarcador, pagamentoTerceiro);

                if (dadosFrete == null)
                {
                    return new JsonpResult(false, "Não foi possível simular o valor do frete.");
                }

                if(!string.IsNullOrEmpty(dadosFrete.MensagemRetorno))
                {
                    return new JsonpResult(false, dadosFrete.MensagemRetorno);
                }

                return new JsonpResult(new
                {
                    NaoIncluirICMSFrete = pedido?.TipoOperacao?.NaoIncluirICMSFrete ?? false,
                    ValorFrete = dadosFrete.ValorFrete.ToString("n2"),
                    BaseCalculoICMS = dadosFrete.BaseCalculoICMS.ToString("n2"),
                    ValorComponentes = (from obj in dadosFrete.Componentes where obj.SomarComponenteFreteLiquido || obj.DescontarComponenteFreteLiquido select obj.DescontarComponenteFreteLiquido ? obj.ValorComponente * -1 : obj.ValorComponente).Sum().ToString("n2"),
                    TabelaFrete = dadosFrete.TabelaFrete?.Descricao ?? "",
                    AliquotaICMS = (pagamentoTerceiro ? 0m : svcICMS.ObterAliquotaICMSPedido(pedido, unitOfWork)).ToString("n2"),
                    compomentes = (from p in dadosFrete.Componentes
                                   select new
                                   {
                                       Codigo = p.ID,
                                       Componente = p.ComponenteFrete.Descricao,
                                       Valor = p.ValorComponente.ToString("n2"),
                                       Percentual = p.Percentual.ToString("n2"),
                                       ValorTotal = p.ValorComponente.ToString("n2")
                                   }).ToList()
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao simular o valor do frete.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterTotalVeiculosAlocados()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(unitOfWork);
                int Quantidade = repositorioVeiculo.ContarVeiculosNaoDisponiveis();

                return new JsonpResult(new { Quantidade });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar os veículos alocados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DuplicarPedido()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                int quantidadeDuplicar = Request.GetIntParam("QuantidadeDuplicar");
                DateTime dataPedido = Request.GetDateTimeParam("DataPedido");

                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repPedido.BuscarPorCodigo(codigo);

                if (pedido == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o pedido.");

                if (pedido.Cotacao)
                    return new JsonpResult(false, true, "Não é possível duplicar um pedido de cotação.");

                if (pedido.ColetaEmProdutorRural)
                    return new JsonpResult(false, true, "Não é possível duplicar um pedido de coleta em produtor rural.");

                unitOfWork.Start();


                string retorno = DuplicarPedido(pedido, dataPedido, quantidadeDuplicar, unitOfWork);

                if (!string.IsNullOrEmpty(retorno))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, retorno);
                };

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
                return new JsonpResult(false, "Ocorreu uma falha ao alterar os dados do pedido.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DuplicarPedidosSelecionados()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = ObterPlanejamentoPedido(unitOfWork);

                int quantidadeDuplicar = Request.GetIntParam("QuantidadeDuplicar");
                DateTime dataPedido = Request.GetDateTimeParam("DataPedido");

                foreach (var pedido in pedidos)
                {
                    if (pedido == null)
                        return new JsonpResult(false, true, "Não foi possível encontrar o pedido.");

                    if (pedido.Cotacao)
                        return new JsonpResult(false, true, "Não é possível duplicar um pedido de cotação.");

                    if (pedido.ColetaEmProdutorRural)
                        return new JsonpResult(false, true, "Não é possível duplicar um pedido de coleta em produtor rural.");
                }

                foreach (var pedido in pedidos)
                {
                    unitOfWork.Start();

                    string retorno = DuplicarPedido(pedido, dataPedido, quantidadeDuplicar, unitOfWork);

                    if (!string.IsNullOrEmpty(retorno))
                        throw new ControllerException(retorno);

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
                return new JsonpResult(false, "Ocorreu uma falha ao alterar os dados do pedido.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AlterarDadosPedido()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorLancamento repositorioColaboradorLancamento = new Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorLancamento(unitOfWork);
            Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);
            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.RotaFrete repRotaFrete = new Repositorio.RotaFrete(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repositorioPedido.BuscarPorCodigo(codigo, auditavel: true);
                Dominio.Relatorios.Embarcador.DataSource.Pedidos.PlanejamentoPedidoTMS planejamentoPedidoTMS = new Dominio.Relatorios.Embarcador.DataSource.Pedidos.PlanejamentoPedidoTMS();

                if (pedido == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o pedido.");

                pedido.ObservacaoInterna = Request.GetNullableStringParam("ObservacaoInterna");
                string numeroFrota = Request.GetNullableStringParam("NumeroFrota");
                string numeroRota = Request.GetNullableStringParam("NumeroRota");
                DataPlanejamentoPedidoTMS tipoDataAgrupamento = Request.GetEnumParam<DataPlanejamentoPedidoTMS>("TipoDataAgrupamento");

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Pedidos/PlanejamentoPedidoTMS");

                Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(unitOfWork);

                List<Dominio.Entidades.Veiculo> veiculos = !string.IsNullOrWhiteSpace(numeroFrota) ? repositorioVeiculo.BuscarPorNumeroDaFrota(numeroFrota) : null;

                string rotaPedido = pedido.RotaFrete?.CodigoIntegracao ?? "";
                string frotaPedido = ObterFrota(pedido, unitOfWork);
                bool alterouFrota = frotaPedido != numeroFrota && numeroFrota != null;
                bool alterouRota = rotaPedido != numeroRota && numeroRota != null;

                unitOfWork.Start();

                if (alterouRota && !string.IsNullOrWhiteSpace(numeroRota))
                {
                    Dominio.Entidades.RotaFrete rotaFrete = repRotaFrete.BuscarPorCodigoIntegracao(numeroRota);
                    if (!rotaFrete?.Ativo ?? true)
                        return new JsonpResult(false, true, "Não foi possível encontrar a Rota de Frete.");
                    if (rotaFrete != null)
                        pedido.RotaFrete = rotaFrete;
                }
                repositorioPedido.Atualizar(pedido, Auditado);

                if (alterouFrota)
                {
                    if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteInformarAlterarRemoverVeiculo))
                        return new JsonpResult(false, true, "Usuário não tem permissão alterar o veículo.");

                    if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    {
                        Dominio.Entidades.Veiculo veiculo = veiculos?.FirstOrDefault() ?? new Dominio.Entidades.Veiculo();

                        if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteInserirVeiculoProprio) && (veiculo?.Tipo?.Equals("P") ?? false))
                            return new JsonpResult(false, true, "Usuário não tem permissão para definir veículos do tipo Próprio.");

                        if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteInserirVeiculoTerceiro) && (veiculo?.Tipo?.Equals("T") ?? false))
                            return new JsonpResult(false, true, "Usuário não tem permissão para definir veículos do tipo Terceiro.");
                    }

                    if (veiculos?.FirstOrDefault() != null)
                    {
                        var motorista = repVeiculoMotorista.BuscarMotoristaPrincipal(veiculos.FirstOrDefault().Codigo);

                        if (pedido != null && pedido.PrevisaoEntrega != null && motorista != null)
                        {
                            Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorLancamento colaboradorLancamento = repositorioColaboradorLancamento.BuscarPorCodigoMotoristaCorrente(motorista.Codigo, pedido.PrevisaoEntrega.Value);
                            if (colaboradorLancamento != null)
                                return new JsonpResult(false, true, $"Não foi fazer a alteração, pois o motorista {motorista.Nome}, estará de " + colaboradorLancamento.ColaboradorSituacao.Descricao + ".");
                        }
                        SalvarVeiculoNoPedido(veiculos?.FirstOrDefault(), pedido, unitOfWork);
                    }
                    else
                    {
                        RemoverVeiculoDoPedido(pedido, unitOfWork);
                        RemoverMotoristaDoPedido(pedido, unitOfWork);
                    }
                }

                unitOfWork.CommitChanges();

                /// aqui consulta planejamento
                Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedido filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedido()
                {
                    CodigoPedido = pedido.Codigo
                };
                IList<Dominio.Relatorios.Embarcador.DataSource.Pedidos.PlanejamentoPedidoTMS> listaPedido = repositorioPedido.ConsultarPlanejamentoPedido(filtrosPesquisa, false, null);

                return new JsonpResult(ObterPedido(listaPedido.FirstOrDefault(), unitOfWork, repositorioPedido, tipoDataAgrupamento), true, string.Empty);
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
                return new JsonpResult(false, "Ocorreu uma falha ao alterar os dados do pedido.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AlterarSituacaoPlanejamentoPedido()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Pedidos/PlanejamentoPedidoTMS");

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repositorioPedido.BuscarPorCodigo(codigo, auditavel: true);

                if (pedido == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o pedido.");

                SituacaoPlanejamentoPedidoTMS? situacaoPlanejamentoPedido = Request.GetNullableEnumParam<SituacaoPlanejamentoPedidoTMS>("SituacaoPlanejamentoPedidoTMS");

                if (!situacaoPlanejamentoPedido.HasValue)
                    return new JsonpResult(false, true, "Situação de planejamento do pedido não informada.");

                if (situacaoPlanejamentoPedido.Value == pedido.SituacaoPlanejamentoPedidoTMS)
                    return new JsonpResult(false, true, "A situação de planejamento do pedido informada é igual a atual. Não é possível realizar a alteração.");

                if (situacaoPlanejamentoPedido.Value == SituacaoPlanejamentoPedidoTMS.Pendente && !permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteInformarPendente))
                    return new JsonpResult(false, true, "Usuário não tem permissão para completar essa ação.");

                if (situacaoPlanejamentoPedido.Value == SituacaoPlanejamentoPedidoTMS.Devolucao && !permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteInformarDevolucao))
                    return new JsonpResult(false, true, "Usuário não tem permissão para completar essa ação.");

                pedido.SituacaoPlanejamentoPedidoTMS = situacaoPlanejamentoPedido.Value;
                if (pedido.SituacaoPlanejamentoPedidoTMS == SituacaoPlanejamentoPedidoTMS.Pendente)
                {
                    pedido.MotoristaCiente = false;
                    pedido.MotoristaAvisado = false;
                }

                repositorioPedido.Atualizar(pedido, Auditado);

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao alterar os dados do pedido.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DefinirVeiculo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorLancamento repositorioColaboradorLancamento = new Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorLancamento(unitOfWork);
            Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);
            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Pedidos/PlanejamentoPedidoTMS");

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteInformarAlterarRemoverVeiculo))
                    return new JsonpResult(false, true, "Usuário não tem permissão para definir o veículo.");

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repositorioPedido.BuscarPorCodigo(codigo);
                Servicos.Embarcador.Frota.OrdemServicoVeiculoManutencao ordemServicoVeiculoManutencao = new Servicos.Embarcador.Frota.OrdemServicoVeiculoManutencao(unitOfWork);
                Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo servicoFilaCarregamentoVeiculo = new Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo(unitOfWork, OrigemAlteracaoFilaCarregamento.Sistema);

                if (pedido == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o pedido.");

                int codigoVeiculo = Request.GetIntParam("Veiculo");

                Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(unitOfWork);
                Dominio.Entidades.Veiculo veiculo = repositorioVeiculo.BuscarPorCodigo(codigoVeiculo);

                var motorista = repVeiculoMotorista.BuscarMotoristaPrincipal(veiculo.Codigo);

                if (pedido != null && pedido.PrevisaoEntrega != null)
                {
                    Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorLancamento colaboradorLancamento = repositorioColaboradorLancamento.BuscarPorCodigoMotoristaCorrente(motorista.Codigo, pedido.PrevisaoEntrega.Value);
                    if (colaboradorLancamento != null)
                        return new JsonpResult(false, true, $"Não foi fazer a alteração, pois o motorista {motorista.Nome}, estará de " + colaboradorLancamento.ColaboradorSituacao.Descricao + ".");
                }


                if (veiculo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o veículo.");

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteInserirVeiculoProprio) && veiculo.Tipo.Equals("P"))
                        throw new ControllerException("Usuário não tem permissão para definir veículos do tipo Próprio.");

                    if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteInserirVeiculoTerceiro) && veiculo.Tipo.Equals("T"))
                        throw new ControllerException("Usuário não tem permissão para definir veículos do tipo Terceiro.");
                }

                ordemServicoVeiculoManutencao.VeiculoIndisponivelParaTransporte(pedido);

                ValidarRegrasLicenca(null, new List<int>() { codigoVeiculo }, unitOfWork);

                unitOfWork.Start();
                Dominio.Entidades.Veiculo veiculoSubstituido = new Dominio.Entidades.Veiculo();
                List<Dominio.Entidades.Veiculo> veiculoSubstituidos = new List<Dominio.Entidades.Veiculo>();

                if (pedido.VeiculoTracao != null && veiculo != null && pedido.VeiculoTracao.Codigo == veiculo.Codigo)
                {
                    veiculoSubstituido = pedido.VeiculoTracao;
                    veiculoSubstituidos.AddRange(pedido.Veiculos);
                }

                SalvarVeiculoNoPedido(veiculo, pedido, unitOfWork);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    if (veiculoSubstituido != null)
                        servicoFilaCarregamentoVeiculo.RealocarVeiculoNaFila(veiculoSubstituido.Codigo, TipoServicoMultisoftware);

                    if (veiculoSubstituidos.Count > 0)
                    {
                        foreach (var veiculoS in veiculoSubstituidos)
                        {
                            servicoFilaCarregamentoVeiculo.RealocarVeiculoNaFila(veiculoS.Codigo, TipoServicoMultisoftware);
                        }
                    }
                }

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
                return new JsonpResult(false, "Ocorreu uma falha ao definir o veículo do pedido.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DefinirMotorista()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorLancamento repositorioColaboradorLancamento = new Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorLancamento(unitOfWork);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Pedidos/PlanejamentoPedidoTMS");

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteInformarAlterarRemoverMotorista))
                    return new JsonpResult(false, true, "Usuário não tem permissão para definir um Motorista.");

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repositorioPedido.BuscarPorCodigo(codigo);

                if (pedido == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o pedido.");

                if (!(this.Usuario.UsuarioAdministrador || permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteDefinirVeiculoMotoristaLicencaVencida)))
                    return new JsonpResult(false, "Seu usuário não possui permissão para alterar o Motorista.");

                int codigoMotorista = Request.GetIntParam("Motorista");

                Repositorio.Usuario repMotorista = new Repositorio.Usuario(unitOfWork);
                Dominio.Entidades.Usuario motorista = repMotorista.BuscarPorCodigo(codigoMotorista);

                if (motorista == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o motorista.");


                if (pedido != null && pedido.PrevisaoEntrega != null)
                {
                    Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorLancamento colaboradorLancamento = repositorioColaboradorLancamento.BuscarPorCodigoMotoristaCorrente(codigoMotorista, pedido.PrevisaoEntrega.Value);
                    if (colaboradorLancamento != null)
                        return new JsonpResult(false, true, "Não foi fazer a alteração, pois o motorista estará de " + colaboradorLancamento.ColaboradorSituacao.Descricao + ".");
                }

                ValidarRegrasLicenca(new List<int>() { codigoMotorista }, null, unitOfWork);

                unitOfWork.Start();

                if (pedido.Motoristas == null)
                    pedido.Motoristas = new List<Dominio.Entidades.Usuario>();
                else
                    pedido.Motoristas.Clear();

                pedido.MotoristaCiente = false;
                pedido.Motoristas.Add(motorista);
                //AtualizarCargaPorPedido(pedido, unitOfWork);
                repositorioPedido.Atualizar(pedido);

                AtualizarCarga(pedido.Codigo, unitOfWork);

                Servicos.Embarcador.Integracao.IntegracaoPedido.ReenviarIntegracaoPedidos(pedido.Codigo, unitOfWork);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, pedido, $"Definido o motorista {motorista.Nome} para o pedido.", unitOfWork);

                foreach (Dominio.Entidades.Veiculo veiculo in pedido.Veiculos)
                {
                    Dominio.Entidades.Usuario veiculoMotorista = repVeiculoMotorista.BuscarMotoristaPrincipal(veiculo.Codigo);

                    if (veiculoMotorista == null || veiculoMotorista.Codigo != motorista.Codigo)
                    {
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, veiculo, $"Removido motorista principal.", unitOfWork);
                        repVeiculoMotorista.DeletarMotoristaPrincipal(veiculo.Codigo);
                        Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista VeiculoMotoristaPrincipal = new Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista
                        {
                            CPF = motorista.CPF,
                            Motorista = motorista,
                            Nome = motorista.Nome,
                            Veiculo = veiculo,
                            Principal = true
                        };

                        repVeiculoMotorista.Inserir(VeiculoMotoristaPrincipal, Auditado);
                    }
                }
                if (pedido.VeiculoTracao != null)
                {
                    var motoristaPrincipal = repVeiculoMotorista.BuscarVeiculoMotoristaPrincipal(pedido.VeiculoTracao.Codigo);
                    if (motoristaPrincipal != null)
                    {
                        motoristaPrincipal.Motorista = motorista;
                        motoristaPrincipal.CPF = motorista.CPF;
                        motoristaPrincipal.Nome = motorista.Nome;
                        repVeiculoMotorista.Atualizar(motoristaPrincipal);
                    }
                    else
                    {
                        motoristaPrincipal.Motorista = motorista;
                        motoristaPrincipal.CPF = motorista.CPF;
                        motoristaPrincipal.Nome = motorista.Nome;
                        motoristaPrincipal.Principal = true;
                        motoristaPrincipal.Veiculo = pedido.VeiculoTracao;
                        repVeiculoMotorista.Inserir(motoristaPrincipal);
                    }

                }

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
                return new JsonpResult(false, "Ocorreu uma falha ao definir o motorista do pedido.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RemoverVeiculo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repositorioPedido.BuscarPorCodigo(codigo);

                if (pedido == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o pedido.");

                unitOfWork.Start();

                RemoverVeiculoDoPedido(pedido, unitOfWork);

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
                return new JsonpResult(false, "Ocorreu uma falha ao remover o veículo do pedido.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RemoverMotorista()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Pedidos/PlanejamentoPedidoTMS");

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteInformarAlterarRemoverMotorista))
                    return new JsonpResult(false, true, "Usuário não tem permissão para remover o Motorista.");

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repositorioPedido.BuscarPorCodigo(codigo);

                if (pedido == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o pedido.");

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorPedido(pedido.Codigo);

                unitOfWork.Start();

                RemoverMotoristaDoPedido(pedido, unitOfWork);

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
                return new JsonpResult(false, "Ocorreu uma falha ao remover o motorista do pedido.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> GerarCargasPedidos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Pedidos/PlanejamentoPedidoTMS");

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteGerarCarga))
                    return new JsonpResult(false, true, "Usuário não tem permissão para gerar carga.");

                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = ObterPlanejamentoPedido(unitOfWork);
                pedidos = pedidos.Where(o => o.CargasPedido.Count == 0).ToList();

                if (pedidos.Count == 0)
                    return new JsonpResult(false, true, "Nenhum pedido sem carga foi selecionado.");

                unitOfWork.Start();

                string retorno = Servicos.Embarcador.Pedido.Pedido.CriarCarga(out Dominio.Entidades.Embarcador.Cargas.Carga carga, pedidos, unitOfWork, TipoServicoMultisoftware, null, ConfiguracaoEmbarcador, true, true, true, true, false, null, true);

                if (!string.IsNullOrEmpty(retorno))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, retorno);
                };

                if (carga?.TipoOperacao?.ConfiguracaoCarga?.EnviarEmailAoGerarCargaPlanejamentoPedidosDetalhado ?? false)
                {
                    Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                    var pedidosCarga = repositorioCargaPedido.BuscarPedidosPorCarga(carga.Codigo);
                    EnviarEmailOrdemColeta(pedidosCarga, unitOfWork);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true, true, "Carga " + carga.CodigoCargaEmbarcador + " gerada com sucesso!");
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(true, false, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar cargas dos pedidos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> EnviarEmailTomadorPedidosSelecionados()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Pedidos/PlanejamentoPedidoTMS");

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteEnviarEmailTomador))
                    return new JsonpResult(false, true, "Usuário não tem permissão para enviar e-mail para o tomador.");

                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = ObterPlanejamentoPedido(unitOfWork);

                if (pedidos.Count == 0)
                    return new JsonpResult(false, true, "Nenhum pedido foi selecionado.");

                foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidos)
                {
                    string email = pedido.ObterTomador()?.Email ?? string.Empty;
                    EnviarEmail(email, "E-mail referente ao seu Pedido", "Planejamento de Pedidos", "", unitOfWork);
                }

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao enviar e-mails para os tomadores.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> EnviarEmailOrdemColeta()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Pedidos/PlanejamentoPedidoTMS");

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteEnviarOrdemColetaTomador))
                    return new JsonpResult(false, true, "Usuário não tem permissão para enviar Ordem de Coleta ao tomador");

                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = ObterPlanejamentoPedido(unitOfWork);

                if (pedidos == null || pedidos.Count == 0)
                    return new JsonpResult(true, false, "Nenhum pedido selecionado.");

                EnviarEmailOrdemColeta(pedidos, unitOfWork);

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao enviar e-mail.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> EnviarEmailCheckList()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Pedidos/PlanejamentoPedidoTMS");

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteEnviarEmailCheckList))
                    return new JsonpResult(false, true, "Usuário não tem permissão para enviar e-mail para o Check List.");

                string email = Request.GetStringParam("Email");
                string assunto = Request.GetStringParam("Assunto");

                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = null;
                string arquivo = GerarRelatorioPedidosSelecionados(unitOfWork, out pedidos);

                EnviarEmail(email, "", assunto, arquivo, unitOfWork);

                Repositorio.Embarcador.Pedidos.PedidoTratativa repositorioPedidoTratativa = new Repositorio.Embarcador.Pedidos.PedidoTratativa(unitOfWork);

                DateTime dataEnvioEmail = DateTime.Now;
                foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidos)
                {
                    Dominio.Entidades.Embarcador.Pedidos.PedidoTratativa pedidoTratativa = new Dominio.Entidades.Embarcador.Pedidos.PedidoTratativa()
                    {
                        Data = dataEnvioEmail,
                        TextoLivre = "Enviou e-mail para o checklist",
                        Usuario = Usuario,
                        Pedido = pedido
                    };

                    repositorioPedidoTratativa.Inserir(pedidoTratativa);
                }

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao enviar e-mail.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DefinirTipoOperacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Pedidos/PlanejamentoPedidoTMS");

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteAlterarTipoDeOperacao))
                    return new JsonpResult(false, true, "Usuário não tem permissão para completar essa ação.");

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repositorioPedido.BuscarPorCodigo(codigo);

                if (pedido == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o pedido.");

                int codigoTipoOperacao = Request.GetIntParam("TipoOperacao");

                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repTipoOperacao.BuscarPorCodigo(codigoTipoOperacao);

                if (tipoOperacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o tipo da operação.");

                unitOfWork.Start();

                pedido.TipoOperacao = tipoOperacao;

                //AtualizarCargaPorPedido(pedido, unitOfWork);

                repositorioPedido.Atualizar(pedido);

                AtualizarCarga(pedido.Codigo, unitOfWork);
                Servicos.Embarcador.Integracao.IntegracaoPedido.ReenviarIntegracaoPedidos(pedido.Codigo, unitOfWork);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, pedido, $"Definido o tipo da operação {tipoOperacao.Descricao} para o pedido.", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
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
                return new JsonpResult(false, "Ocorreu uma falha ao definir o tipo da operação do pedido.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DefinirModeloVeicular()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Pedidos/PlanejamentoPedidoTMS");

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteSubstituirModeloVeicular))
                    return new JsonpResult(false, true, "Usuário não tem permissão para completar essa ação.");

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.ModeloVeicularCarga repositorioModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repositorioPedido.BuscarPorCodigo(codigo);

                if (pedido == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o pedido.");

                int codigoModeloVeicular = Request.GetIntParam("ModeloVeicular");

                Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga = repositorioModeloVeicularCarga.BuscarPorCodigo(codigoModeloVeicular);

                if (modeloVeicularCarga == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o modelo veicular.");

                if (pedido.ModeloVeicularCarga?.Codigo == modeloVeicularCarga.Codigo)
                    return new JsonpResult(false, true, "Não é possível definir novamente o mesmo modelo veicular.");

                unitOfWork.Start();

                pedido.ModeloVeicularCarga = modeloVeicularCarga;

                //AtualizarCargaPorPedido(pedido, unitOfWork);

                repositorioPedido.Atualizar(pedido);

                AtualizarCarga(pedido.Codigo, unitOfWork);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, pedido, $"Definido o modelo veicular {modeloVeicularCarga.Descricao} para o pedido.", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
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
                return new JsonpResult(false, "Ocorreu uma falha ao definir o modelo veicular do pedido.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RegistrarAvisoAoMotorista()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Pedidos/PlanejamentoPedidoTMS");

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteGerarAvisoMotorista))
                    return new JsonpResult(false, true, "Você não tem permissão para executar essa ação.");

                int codigo = Request.GetIntParam("Codigo");
                bool motoristaCiente = Request.GetBoolParam("MotoristaCiente");

                Repositorio.Embarcador.Pedidos.PedidoMotoristaAviso repositorioPedidoMotoristaAviso = new Repositorio.Embarcador.Pedidos.PedidoMotoristaAviso(unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repositorioPedido.BuscarPorCodigo(codigo);

                if (pedido == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o pedido.");

                if (pedido.MotoristaCiente)
                    return new JsonpResult(false, true, "Motorista já está ciente.");

                if (motoristaCiente && !permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteGerarMotoristaCiente))
                    return new JsonpResult(false, true, "Você não tem permissão para executar essa ação.");

                unitOfWork.Start();

                Dominio.Entidades.Embarcador.Pedidos.PedidoMotoristaAviso pedidoMotoristaAviso = new Dominio.Entidades.Embarcador.Pedidos.PedidoMotoristaAviso()
                {
                    Data = DateTime.Now,
                    NumeroSequencia = repositorioPedidoMotoristaAviso.BuscarProximaSequencia(pedido.Codigo),
                    Ciente = motoristaCiente,
                    Usuario = Usuario,
                    Pedido = pedido
                };

                repositorioPedidoMotoristaAviso.Inserir(pedidoMotoristaAviso);

                if (motoristaCiente)
                {
                    pedido.MotoristaCiente = true;
                    pedido.SituacaoPlanejamentoPedidoTMS = SituacaoPlanejamentoPedidoTMS.MotoristaCiente;
                }
                else
                {
                    pedido.MotoristaAvisado = true;
                    pedido.SituacaoPlanejamentoPedidoTMS = SituacaoPlanejamentoPedidoTMS.AvisoAoMotorista;
                }

                repositorioPedido.Atualizar(pedido);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao registrar o aviso ao motorista.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RegistrarTratativa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                string novaInformacao = Request.GetStringParam("NovaInformacao");

                Repositorio.Embarcador.Pedidos.PedidoTratativa repositorioPedidoTratativa = new Repositorio.Embarcador.Pedidos.PedidoTratativa(unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repositorioPedido.BuscarPorCodigo(codigo);

                if (pedido == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o pedido.");

                unitOfWork.Start();

                Dominio.Entidades.Embarcador.Pedidos.PedidoTratativa pedidoTratativa = new Dominio.Entidades.Embarcador.Pedidos.PedidoTratativa()
                {
                    Data = DateTime.Now,
                    TextoLivre = novaInformacao,
                    Usuario = Usuario,
                    Pedido = pedido
                };

                repositorioPedidoTratativa.Inserir(pedidoTratativa);

                if (!pedido.ComTratativa)
                {
                    pedido.ComTratativa = true;
                    repositorioPedido.Atualizar(pedido);
                }

                unitOfWork.CommitChanges();

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorPedido(codigo);
                if (carga != null)
                {
                    Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(_conexao.StringConexao, null, TipoServicoMultisoftware, string.Empty);

                    string nota = string.Format(Localization.Resources.Pedidos.PlanejamentoPedidoTMS.AdicionadoTratativaCarga, carga.CodigoCargaEmbarcador);
                    if (carga.Operador != null)
                        serNotificacao.GerarNotificacao(carga.Operador, Usuario, pedido.Codigo, "Pedidos/PlanejamentoPedidoTMS", nota, IconesNotificacao.atencao, TipoNotificacao.alerta, TipoServicoMultisoftware, unitOfWork);
                }

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao registrar a tratativa.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarDetalhesAviso()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Pedidos/PlanejamentoPedidoTMS");

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteAbrirDetalhesAviso))
                    return new JsonpResult(false, true, "Usuário não tem permissão para completar essa ação.");

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Pedidos.PedidoMotoristaAviso repositorioPedidoMotoristaAviso = new Repositorio.Embarcador.Pedidos.PedidoMotoristaAviso(unitOfWork);

                List<Dominio.Entidades.Embarcador.Pedidos.PedidoMotoristaAviso> avisos = repositorioPedidoMotoristaAviso.BuscarPorPedido(codigo);

                dynamic retorno = new
                {
                    Avisos = (from obj in avisos
                              select new
                              {
                                  obj.Codigo,
                                  obj.NumeroSequencia,
                                  Registro = (obj.Ciente ? "Motorista avisado: " : $"{obj.NumeroSequencia}ª Tentativa: ") + obj.Data.ToDateTimeString() + " - " + obj.Usuario.Nome
                              }).ToList()
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os detalhes dos avisos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarDetalhesTratativas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Pedidos/PlanejamentoPedidoTMS");

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteAbrirTratativas))
                    return new JsonpResult(false, true, "Usuário não tem permissão completar essa ação.");

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Pedidos.PedidoTratativa repositorioPedidoTratativa = new Repositorio.Embarcador.Pedidos.PedidoTratativa(unitOfWork);

                List<Dominio.Entidades.Embarcador.Pedidos.PedidoTratativa> tratativas = repositorioPedidoTratativa.BuscarPorPedido(codigo);

                int numeroSequencia = 1;
                dynamic retorno = new
                {
                    Codigo = codigo,
                    Tratativas = (from obj in tratativas
                                  select new
                                  {
                                      obj.Codigo,
                                      NumeroSequencia = numeroSequencia++,
                                      Registro = obj.RegistroFormatado
                                  }).ToList()
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os detalhes dos avisos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AlterarDataPrevisaoSaida()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Pedidos/PlanejamentoPedidoTMS");

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteAlterarDataDePrevisaoDeSaida))
                    return new JsonpResult(false, true, "Usuário não tem permissão para completar essa ação.");

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repositorioPedido.BuscarPorCodigo(codigo, auditavel: true);

                if (pedido == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o pedido.");

                unitOfWork.Start();

                pedido.DataPrevisaoSaida = Request.GetDateTimeParam("DataPrevisaoSaida");

                if (pedido.DataPrevisaoSaida.HasValue && pedido.PrevisaoEntrega.HasValue && (pedido.DataPrevisaoSaida.Value > pedido.PrevisaoEntrega.Value))
                    throw new ControllerException("Data da previsão da saída está maior que a data de previsão de retorno.");

                if (ConfiguracaoEmbarcador.BloquearDatasRetroativasPedido)
                {
                    if (pedido.DataPrevisaoSaida.Value.Date < DateTime.Now.Date)
                        throw new ControllerException("A data de previsão de saída não pode ser menor que a data atual.");
                }

                if (!pedido.DataInicialViagemExecutada.HasValue)
                    pedido.DataInicialViagemExecutada = pedido.DataPrevisaoSaida;

                if (!pedido.DataInicialViagemFaturada.HasValue)
                    pedido.DataInicialViagemFaturada = pedido.DataPrevisaoSaida;

                repositorioPedido.Atualizar(pedido, Auditado);

                AtualizarCarga(pedido.Codigo, unitOfWork);

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
                return new JsonpResult(false, "Ocorreu uma falha ao alterar a data de previsão de saída do pedido.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DefinirTipoCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Pedidos/PlanejamentoPedidoTMS");

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteAlterarTipoDeCarga))
                    return new JsonpResult(false, true, "Usuário não tem permissão para completar essa ação.");

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.TipoDeCarga repTipoCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repositorioPedido.BuscarPorCodigo(codigo);

                if (pedido == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o pedido.");

                int codigoTipoCarga = Request.GetIntParam("TipoCarga");

                Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga = repTipoCarga.BuscarPorCodigo(codigoTipoCarga);

                if (tipoCarga == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o tipo de carga.");

                unitOfWork.Start();

                pedido.TipoDeCarga = tipoCarga;

                repositorioPedido.Atualizar(pedido);

                AtualizarCarga(pedido.Codigo, unitOfWork);
                Servicos.Embarcador.Integracao.IntegracaoPedido.ReenviarIntegracaoPedidos(pedido.Codigo, unitOfWork);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, pedido, $"Definido o tipo da carga {tipoCarga.Descricao} para o pedido.", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
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
                return new JsonpResult(false, "Ocorreu uma falha ao definir o tipo da carga do pedido.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> SalvarEnviarEscala()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                Servicos.Embarcador.Carga.MensagemAlertaCarga servicoMensagemAlerta = new Servicos.Embarcador.Carga.MensagemAlertaCarga(unitOfWork);
                Servicos.Embarcador.Notificacao.NotificacaoMTrack servicoNotificacaoMTrack = new Servicos.Embarcador.Notificacao.NotificacaoMTrack(unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repositorioPedido.BuscarPorCodigo(codigo);

                if (pedido == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o pedido.");

                unitOfWork.Start();

                pedido.MotoristaEscala = Request.GetStringParam("MotoristaEscala");
                pedido.DataComparecerEscala = Request.GetDateTimeParam("DataComparecerEscala");
                pedido.AceiteMotorista = Request.GetEnumParam<EnumAceiteMotorista>("AceiteMotorista");

                repositorioPedido.Atualizar(pedido);

                bool motoristaPrecisaConfirmarCarga = false;
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorPedido(codigo);
                if (!string.IsNullOrWhiteSpace(pedido.MotoristaEscala) && (carga?.TipoOperacao?.ConfiguracaoMobile?.NecessarioConfirmacaoMotorista ?? false) && carga.Motoristas.Count > 0 && carga.SituacaoCarga == SituacaoCarga.Nova)
                {
                    TimeSpan tempoLimite = carga.TipoOperacao?.ConfiguracaoMobile?.TempoLimiteConfirmacaoMotorista ?? new TimeSpan();

                    if (tempoLimite.TotalSeconds > 0)
                    {
                        if (carga.DataLimiteConfirmacaoMotorista.HasValue && servicoMensagemAlerta.IsMensagemSemConfirmacao(carga, TipoMensagemAlerta.CargaSemConfirmacaoMotorista))
                            carga.DataLimiteConfirmacaoMotorista = DateTime.Now.Add(tempoLimite);
                        else
                        {
                            carga.DataLimiteConfirmacaoMotorista = DateTime.Now.Add(tempoLimite);
                            servicoMensagemAlerta.Adicionar(carga, TipoMensagemAlerta.CargaSemConfirmacaoMotorista, "Carga aguardando a confirmação do motorista");
                            motoristaPrecisaConfirmarCarga = true;
                        }

                        repositorioCarga.Atualizar(carga);
                    }
                }

                unitOfWork.CommitChanges();

                if (motoristaPrecisaConfirmarCarga)
                    servicoNotificacaoMTrack.NotificarMudancaCarga(carga, carga.Motoristas.ToList(), AdminMultisoftware.Dominio.Enumeradores.MobileHubs.CargaMotoristaNecessitaConfirmar, true, 0, pedido);

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
                return new JsonpResult(false, "Ocorreu uma falha ao enviar a escala ao motorista.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarEscala()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Pedidos/PlanejamentoPedidoTMS");

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteEnviarEscala))
                    return new JsonpResult(false, true, "Usuário não tem permissão para completar essa ação.");

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repositorioPedido.BuscarPorCodigo(codigo);

                if (pedido == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o pedido.");

                dynamic dynEscala = new
                {
                    DataComparecerEscala = pedido.DataComparecerEscala?.ToString("dd/MM/yyyy HH:mm"),
                    AceiteMotorista = pedido.AceiteMotorista,
                };

                return new JsonpResult(dynEscala);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar a escala do motorista.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ValidarVeiculoOutroPedidoOuCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoVeiculo = Request.GetIntParam("Veiculo");

                Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(unitOfWork);
                Dominio.Entidades.Veiculo veiculo = repositorioVeiculo.BuscarPorCodigo(codigoVeiculo);

                if (veiculo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o veículo.");

                string mensagemRetorno = ValidarVeiculoDuplicadoPedidoCargaAberto(veiculo, unitOfWork);

                if (!string.IsNullOrEmpty(mensagemRetorno))
                    return new JsonpResult(true, true, mensagemRetorno);

                return new JsonpResult(true);
            }

            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar o veículo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ValidarSeFrotaVeiculoOutroPedidoOuCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(unitOfWork);

                string frotaVeiculo = Request.GetStringParam("NumeroFrota");
                List<Dominio.Entidades.Veiculo> veiculos = repositorioVeiculo.BuscarPorNumeroDaFrota(frotaVeiculo);
                Dominio.Entidades.Veiculo veiculo = repositorioVeiculo.BuscarPorPlaca(frotaVeiculo);

                if (veiculo == null && veiculos.Count == 0)
                    return new JsonpResult(false, true, "Não foi possível encontrar o veículo.");

                Dominio.Entidades.Veiculo veiculoRetornar = veiculos.Count > 0 ? veiculos.FirstOrDefault() : veiculo;
                string mensagemRetorno = ValidarVeiculoDuplicadoPedidoCargaAberto(veiculoRetornar, unitOfWork);

                if (!string.IsNullOrEmpty(mensagemRetorno))
                    return new JsonpResult(true, true, mensagemRetorno);

                return new JsonpResult(true);
            }

            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar o veículo.");
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

        #endregion

        #region Métodos Privados

        private dynamic ObterPedido(Dominio.Relatorios.Embarcador.DataSource.Pedidos.PlanejamentoPedidoTMS planejamentoPedido, Repositorio.UnitOfWork unitOfWork, Repositorio.Embarcador.Pedidos.Pedido repositorioPedido, DataPlanejamentoPedidoTMS tipoDataAgrupamento)
        {

            string data = tipoDataAgrupamento == DataPlanejamentoPedidoTMS.DataAgendamento ? (planejamentoPedido.DataAgendamento?.ToString("dd/MM/yyyy") ?? string.Empty) : (planejamentoPedido.DataCarregamentoPedido?.ToString("dd/MM/yyyy") ?? string.Empty);

            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repPedido.BuscarPorCodigo(planejamentoPedido.Codigo);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

            bool possuiCarga = (!string.IsNullOrWhiteSpace(planejamentoPedido.CodigoCargaEmbarcador));
            var cargaCodigo = repositorioCargaPedido.BuscarPorPedido(pedido.Codigo).FirstOrDefault()?.Carga?.Codigo;

            return new
            {
                planejamentoPedido.Codigo,
                CodigoCargaEmbarcador = planejamentoPedido.CodigoCargaEmbarcador,
                planejamentoPedido.NumeroPedidoEmbarcador,
                Destino = planejamentoPedido.DestinoFormatado ?? string.Empty,
                DestinoRecebedor = planejamentoPedido.DestinoRecebedor ?? string.Empty,
                Origem = planejamentoPedido.Origem ?? string.Empty,
                Motorista = planejamentoPedido.Motoristas ?? string.Empty,
                PossuiMotorista = planejamentoPedido.Motoristas != null,
                NumeroFrota = pedido == null ? "" : ObterFrota(pedido, unitOfWork, possuiCarga, true),
                NumeroRota = planejamentoPedido.NumeroRota ?? string.Empty,
                Veiculo = pedido == null ? "" : ObterPlacas(pedido, unitOfWork, possuiCarga, true),
                Proprietario = planejamentoPedido.Proprietario ?? planejamentoPedido.ProprietarioVeiculoTracao ?? string.Empty,
                PossuiVeiculo = pedido == null ? false : pedido.Veiculos?.Count > 0 || pedido.VeiculoTracao != null,
                SituacaoPlanejamentoPedidoTMS = planejamentoPedido.SituacaoPlanejamentoPedidoTMSFormatada ?? string.Empty,
                ObservacaoInterna = planejamentoPedido.ObservacaoInterna ?? string.Empty,
                ModeloVeicularCarga = planejamentoPedido.ModeloVeicularCarga ?? string.Empty,
                PossuiModeloVeicular = planejamentoPedido.ModeloVeicularCarga != null,
                Tomador = planejamentoPedido.ObterTomador ?? string.Empty,
                DataEstado = data + " - " + ((planejamentoPedido.UF ?? string.Empty) == "EX" ? "IMPORTAÇÃO" : (planejamentoPedido.Estado ?? string.Empty)) + (planejamentoPedido.CodigoPais > 0 ? " (" + planejamentoPedido.NomePais + ")" : string.Empty),
                NumeroCelularMotorista = planejamentoPedido.NumeroCelularMotorista ?? string.Empty,
                MotoristaCiente = planejamentoPedido.MotoristaCienteFormatado ?? string.Empty,
                HoraColeta = planejamentoPedido.DataCarregamentoPedido?.ToString("HH:mm") ?? string.Empty,
                DataColeta = planejamentoPedido.DataCarregamentoPedido?.ToDateTimeString() ?? string.Empty,
                planejamentoPedido.Tratativa,
                DataPrevisaoSaida = planejamentoPedido.DataPrevisaoSaida?.ToDateTimeString() ?? string.Empty,
                PrevisaoEntrega = planejamentoPedido.PrevisaoEntrega?.ToDateTimeString() ?? string.Empty,
                CentroResultado = planejamentoPedido.CentroResultado ?? string.Empty,
                TipoDeCarga = planejamentoPedido.TipoDeCarga ?? string.Empty,
                Peso = planejamentoPedido.Peso.ToString("n2") ?? 0.ToString("n2"),
                LocalColeta = planejamentoPedido.LocalColetaFormatado ?? string.Empty,
                LocalEntrega = planejamentoPedido.LocalEntregaFormatado ?? string.Empty,
                TipoOperacao = planejamentoPedido.TipoOperacao ?? string.Empty,
                TipoOperacaoCodigo = planejamentoPedido.TipoOperacaoCodigo,
                FronteiraPedido = planejamentoPedido.DescricaoFronteira ?? string.Empty,
                Gestor = planejamentoPedido.Gestor ?? string.Empty,
                ObservacaoTipoOperacao = planejamentoPedido.TipoOperacaoObservacao ?? string.Empty,
                Remetente = planejamentoPedido.RemetenteCPFFormatado ?? string.Empty,
                Destinatario = planejamentoPedido.DestinatarioFormatado ?? string.Empty,
                QtdPallets = planejamentoPedido.NumeroPaletesFracionado.ToString("n2") ?? 0.ToString("n2"),
                QtdSaldoPallets = planejamentoPedido.PalletSaldoRestante.ToString("n2") ?? 0.ToString("n2"),
                QtdPalletsCarregado = planejamentoPedido.SomaPallet.ToString("n2") ?? 0.ToString("n2"),
                Integracao = planejamentoPedido.SituacaoIntegracao ?? string.Empty,
                Container = planejamentoPedido.Container ?? string.Empty,
                TipoDeContainer = planejamentoPedido.TipoContainerDescricao ?? string.Empty,
                TaraContainer = planejamentoPedido.TaraContainer ?? string.Empty,
                LacreContainerUm = planejamentoPedido.LacreContainerUm ?? string.Empty,
                LacreContainerDois = planejamentoPedido.LacreContainerDois ?? string.Empty,
                LacreContainerTres = planejamentoPedido.LacreContainerTres ?? string.Empty,
                NumeroBooking = planejamentoPedido.NumeroBooking ?? string.Empty,
                Navio = planejamentoPedido.NumeroNavio ?? string.Empty,
                ValorTotalMercadoria = planejamentoPedido.ValorTotalNotas.ToString("n2") ?? 0.ToString("n2"),
                IndicativoColetaEntrega = planejamentoPedido.IndicativoColetaEntregaDescricao ?? string.Empty,
                NumeroPedido = pedido == null ? "" : pedido.Numero.ToString(),
                CategoriaOSDescricao = planejamentoPedido.CategoriaOSDescricao ?? string.Empty,
                TipoOSConvertidoDescricao = planejamentoPedido.TipoOSConvertidoDescricao ?? string.Empty,
                NecessitaInformarPlacaCarregamento = (pedido.TipoOperacao?.ConfiguracaoCarga?.NecessitaInformarPlacaCarregamento ?? false) && repositorioCargaPedido.BuscarPorPedido(pedido.Codigo).Count > 0 ? true : false,
                TipoPropriedadeVeiculo = planejamentoPedido.TipoPropriedadeVeiculo == "T" ? "TERCEIRO" : planejamentoPedido.TipoPropriedadeVeiculo == "P" ? "PRÓPRIO" : string.Empty,

                DT_RowColor = planejamentoPedido.DT_RowColor,
                DT_FontColor = planejamentoPedido.DT_FontColor,
                DT_Enable = true,
                CodigoCarga = cargaCodigo
            };
        }

        private string ObterPlacas(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Repositorio.UnitOfWork unitOfWork, bool possuiCarga = false, bool realizouValidacaoSePossuiCarga = false)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

            if (!realizouValidacaoSePossuiCarga)
                possuiCarga = repositorioCargaPedido.ExistePorPedido(pedido.Codigo);

            if (possuiCarga)
                return string.Join(", ", repositorioCargaPedido.BuscarPlacasVeiculosCargasPorPedido(pedido.Codigo));

            string retorno = "";
            if (pedido.Veiculos.Count == 1)
            {
                Dominio.Entidades.Veiculo veiculo = pedido.Veiculos.FirstOrDefault();
                string placa = veiculo.Placa;

                if (veiculo.VeiculosVinculados.Count > 0)
                    placa += ", " + string.Join(", ", veiculo.VeiculosVinculados.Select(o => o.Placa));

                retorno = placa;
            }
            else
                retorno = string.Join(", ", pedido.Veiculos.Select(o => o.Placa));

            if (pedido.VeiculoTracao != null)
                retorno = pedido.VeiculoTracao.Placa + (string.IsNullOrWhiteSpace(retorno) ? "" : ", " + retorno);

            return retorno;
        }

        private string ObterFrota(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Repositorio.UnitOfWork unitOfWork, bool possuiCarga = false, bool realizouValidacaoSePossuiCarga = false)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

            if (!realizouValidacaoSePossuiCarga)
                possuiCarga = repositorioCargaPedido.ExistePorPedido(pedido.Codigo);

            if (possuiCarga)
                return string.Join(", ", repositorioCargaPedido.BuscarNumeroFrotasVeiculosCargasPorPedido(pedido.Codigo));

            string retorno = "";
            if (pedido.Veiculos != null && pedido.Veiculos.Count == 1)
            {
                Dominio.Entidades.Veiculo veiculo = pedido.Veiculos.FirstOrDefault();
                List<string> numeroFrota = new List<string>() { veiculo.NumeroFrota };

                if (veiculo.VeiculosVinculados != null)
                    numeroFrota.AddRange(veiculo.VeiculosVinculados.Select(o => o.NumeroFrota));

                retorno = string.Join(", ", numeroFrota.Where(o => !string.IsNullOrWhiteSpace(o)));
            }
            else if (pedido.Veiculos != null)
                retorno = string.Join(", ", pedido.Veiculos.Where(o => !string.IsNullOrWhiteSpace(o.NumeroFrota)).Select(o => o.NumeroFrota));

            if (pedido.VeiculoTracao != null)
                retorno = pedido.VeiculoTracao.NumeroFrota + (string.IsNullOrWhiteSpace(retorno) ? "" : ", " + retorno);

            return retorno;
        }

        private string ObterSituacaoIntegracao(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoIntegracao repPedidoIntegracao = new Repositorio.Embarcador.Pedidos.PedidoIntegracao(unitOfWork);
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao> pedidoIntegracaoes = repPedidoIntegracao.BuscarPorPedido(pedido.Codigo);

            if (pedidoIntegracaoes == null && pedidoIntegracaoes.Count <= 0)
                return "";

            return string.Join(", ", pedidoIntegracaoes.Select(o => o.SituacaoIntegracao.ObterDescricao()));

        }

        private Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedido ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repositorioConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = repositorioConfiguracaoGeral.BuscarConfiguracaoPadrao();

            List<int> codigosFilial = Request.GetListParam<int>("Filial");
            List<int> codigosTipoCarga = Request.GetListParam<int>("TipoCarga");
            List<int> codigosTipoOperacao = Request.GetListParam<int>("TipoOperacao");

            Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedido filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedido()
            {
                CodigosFilial = codigosFilial.Count == 0 ? ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork) : codigosFilial,
                CodigosTipoCarga = codigosTipoCarga.Count == 0 ? ObterListaCodigoTipoCargaPermitidosOperadorLogistica(unitOfWork) : codigosTipoCarga,
                CodigosTipoOperacao = codigosTipoOperacao.Count == 0 ? ObterListaCodigoTipoOperacaoPermitidosOperadorLogistica(unitOfWork) : codigosTipoOperacao,
                CodigosTipoOperacaoDiferenteDe = Request.GetListParam<int>("TipoOperacaoDiferenteDe"),
                CodigosCidadePoloDestino = Request.GetListParam<int>("CidadePoloDestino"),
                CodigosCidadePoloOrigem = Request.GetListParam<int>("CidadePoloOrigem"),
                DataColeta = Request.GetNullableDateTimeParam("DataColeta"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicio"),
                DataLimite = Request.GetNullableDateTimeParam("DataFim"),
                Destinatarios = Request.GetListParam<double>("Destinatario"),
                CodigosDestino = Request.GetListParam<int>("Destino"),
                CodigosGrupoPessoa = Request.GetListParam<int>("GrupoPessoa"),
                NumeroCarga = Request.GetStringParam("CodigoCargaEmbarcador"),
                NumeroNotaFiscal = Request.GetIntParam("NotaFiscal"),
                NumeroPedido = Request.GetIntParam("NumeroPedido"),
                NumeroPedidoEmbarcador = Request.GetStringParam("NumeroPedidoEmbarcador"),
                CodigosOrigem = Request.GetListParam<int>("Origem"),
                CodigosPaisDestino = Request.GetListParam<int>("PaisDestino"),
                CodigosPaisOrigem = Request.GetListParam<int>("PaisOrigem"),
                Remetentes = Request.GetListParam<double>("Remetente"),
                Situacao = Request.GetNullableEnumParam<SituacaoPedido>("Situacao"),
                TipoPessoa = Request.GetEnumParam<TipoPessoa>("TipoPessoa"),
                SituacaoPlanejamentoPedidoTMS = Request.GetListEnumParam<SituacaoPlanejamentoPedidoTMS>("SituacaoPlanejamentoPedidoTMS"),
                Tomadores = Request.GetListParam<double>("Tomador"),
                CodigosMotorista = Request.GetListParam<int>("Motorista"),
                CodigosGestores = Request.GetListParam<int>("Gestor"),
                CodigoOperador = Request.GetIntParam("Operador"),
                CodigosVeiculo = Request.GetListParam<int>("Veiculo"),
                EstadosOrigem = Request.GetListParam<string>("EstadoOrigem"),
                EstadosDestino = Request.GetListParam<string>("EstadoDestino"),
                TipoPropriedadeVeiculo = Request.GetStringParam("TipoPropriedadeVeiculo"),
                VinculoCarga = Request.GetListEnumParam<PedidosVinculadosCarga>("VinculoCarga"),
                CargaPerigosa = Request.GetNullableEnumParam<Dominio.Enumeradores.OpcaoSimNaoPesquisa>("CargaPerigosa"),
                CodigosCentroResultado = Request.GetListParam<int>("CentroResultado"),
                CodigosFuncionarioResponsavel = Request.GetListParam<int>("FuncionarioResponsavel"),
                CodigosModelosVeicularesCarga = Request.GetListParam<int>("ModeloVeicularCarga"),
                CodigosSegmentosVeiculos = Request.GetListParam<int>("SegmentoVeiculo"),
                CodigosEmpresa = new List<int>(),
                AceiteMotorista = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumAceiteMotorista>("AceiteMotorista"),
                TipoServicoMultisoftware = TipoServicoMultisoftware,
                UsarTipoTomadorPedido = false,
                FiltrarPedidosPorSegragacaoEmpresasFiliaisVinculadasAoUsuario = (this.Usuario?.LimitarOperacaoPorEmpresa ?? false) && (configuracaoGeral?.AtivarConsultaSegregacaoPorEmpresa ?? false) && this.Usuario?.Empresas != null && this.Usuario?.Empresas.Count > 0,
                UsuarioUtilizaSegregacaoPorProvedor = Usuario.UsuarioUtilizaSegregacaoPorProvedor,
                CategoriaOS = Request.GetListEnumParam<CategoriaOS>("CategoriaOS"),
                TipoOSConvertido = Request.GetListEnumParam<TipoOSConvertido>("TipoOSConvertido"),
                AceitaContratarTerceiros = Request.GetNullableEnumParam<Dominio.Enumeradores.OpcaoSimNaoPesquisa>("AceitaContratarTerceiros"),
            };

            if (filtrosPesquisa.FiltrarPedidosPorSegragacaoEmpresasFiliaisVinculadasAoUsuario)
                filtrosPesquisa.CodigosEmpresa = this.Usuario.Empresas.Select(o => o.Codigo).ToList();

            if (filtrosPesquisa.UsuarioUtilizaSegregacaoPorProvedor)
                filtrosPesquisa.CodigosProvedores = Usuario.ClientesProvedores.Select(x => x.CPF_CNPJ).ToList();

            return filtrosPesquisa;
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "Remetente")
                return "Remetente.Nome";

            if (propriedadeOrdenar == "Destinatario")
                return "Destinatario.Nome";

            if (propriedadeOrdenar == "HoraColeta")
                return "DataCarregamentoPedido";

            return propriedadeOrdenar;
        }

        private string ObterPropriedadeOrdenarPlanejamentoPedido(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "Remetente")
                return "remetente.CLI_NOME";

            if (propriedadeOrdenar == "Destinatario")
                return "destinatario.CLI_NOME";

            if (propriedadeOrdenar == "HoraColeta")
                return "pedido.CAR_DATA_CARREGAMENTO_PEDIDO";

            return propriedadeOrdenar;
        }

        private string DuplicarPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, DateTime dataPedido, int quantidadeDuplicar, Repositorio.UnitOfWork unitOfWork)
        {

            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoEndereco repPedidoEndereco = new Repositorio.Embarcador.Pedidos.PedidoEndereco(unitOfWork);

            Servicos.Embarcador.Pedido.Pedido svcPedido = new Servicos.Embarcador.Pedido.Pedido();

            for (var i = 0; i < quantidadeDuplicar; i++)
            {
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoNovo = pedido.Clonar();
                pedido.ControleNumeracao = pedido.Codigo;
                repPedido.Atualizar(pedido);

                Utilidades.Object.DefinirListasGenericasComoNulas(pedidoNovo);

                pedidoNovo.SituacaoPlanejamentoPedidoTMS = SituacaoPlanejamentoPedidoTMS.Pendente;
                pedidoNovo.SituacaoPedido = SituacaoPedido.Aberto;
                pedidoNovo.Numero = repPedido.BuscarProximoNumero();
                pedidoNovo.DataCarregamentoPedido = dataPedido;
                pedidoNovo.ObservacaoInterna = null;
                pedidoNovo.Ordem = null;
                pedidoNovo.NumeroPedidoEmbarcador = null;
                pedidoNovo.CodigoCargaEmbarcador = null;
                pedidoNovo.DataCriacao = DateTime.Now;
                pedidoNovo.DataFinalColeta = null;
                pedidoNovo.DataFinalViagemExecutada = null;
                pedidoNovo.DataFinalViagemFaturada = null;
                pedidoNovo.DataInicialColeta = null;
                pedidoNovo.DataInicialViagemExecutada = null;
                pedidoNovo.DataInicialViagemFaturada = null;
                pedidoNovo.DataPrevisaoChegadaDestinatario = null;
                pedidoNovo.DataPrevisaoSaida = null;
                pedidoNovo.DataPrevisaoSaidaDestinatario = null;
                pedidoNovo.DataVencimentoArmazenamentoImportacao = null;
                pedidoNovo.PrevisaoEntrega = null;
                pedidoNovo.EnderecoDestino = null;
                pedidoNovo.EnderecoOrigem = null;
                pedidoNovo.SituacaoAcompanhamentoPedido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcompanhamentoPedido.AgColeta;
                pedidoNovo.MotoristaCiente = false;
                pedidoNovo.MotoristaAvisado = false;

                svcPedido.PreencherCodigoCargaEmbarcador(pedidoNovo, ConfiguracaoEmbarcador, unitOfWork);

                repPedido.Inserir(pedidoNovo);

                if (pedido.EnderecoOrigem != null)
                {
                    Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco enderecoOrigem = pedido.EnderecoOrigem.Clonar();

                    Utilidades.Object.DefinirListasGenericasComoNulas(enderecoOrigem);

                    repPedidoEndereco.Inserir(enderecoOrigem);

                    pedidoNovo.EnderecoOrigem = enderecoOrigem;
                }

                if (pedido.EnderecoDestino != null)
                {
                    Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco enderecoDestino = pedido.EnderecoDestino.Clonar();

                    Utilidades.Object.DefinirListasGenericasComoNulas(enderecoDestino);

                    repPedidoEndereco.Inserir(enderecoDestino);

                    pedidoNovo.EnderecoDestino = enderecoDestino;
                }

                if (!ConfiguracaoEmbarcador.UtilizarIntegracaoPedido)
                {
                    unitOfWork.Clear(pedidoNovo);

                    pedidoNovo = repPedido.BuscarPorCodigo(pedidoNovo.Codigo);

                    pedidoNovo.PedidoIntegradoEmbarcador = true;

                    string retorno = Servicos.Embarcador.Pedido.Pedido.CriarCarga(pedidoNovo, unitOfWork, TipoServicoMultisoftware, null, ConfiguracaoEmbarcador);

                    if (string.IsNullOrWhiteSpace(retorno))
                        repPedido.Atualizar(pedidoNovo);
                    else
                    {
                        return retorno;
                    }
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, pedidoNovo, "Criou o pedido à partir da duplicação pela tela de planejamento de pedidos.", unitOfWork);
            }

            return string.Empty;
        }

        private void SalvarVeiculoNoPedido(Dominio.Entidades.Veiculo veiculo, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);

            Dominio.Entidades.Usuario veiculoMotorista = repVeiculoMotorista.BuscarMotoristaPrincipal(veiculo.Codigo);

            if (pedido.Veiculos == null)
                pedido.Veiculos = new List<Dominio.Entidades.Veiculo>();
            else
                pedido.Veiculos.Clear();

            if (pedido.Motoristas == null)
                pedido.Motoristas = new List<Dominio.Entidades.Usuario>();

            if (ConfiguracaoEmbarcador.PermitirSelecionarReboquePedido)
            {
                if (veiculo.TipoVeiculo == "0")
                    pedido.VeiculoTracao = veiculo;
                else
                {
                    if (veiculo.VeiculosTracao != null && veiculo.VeiculosTracao.Count > 0)
                        pedido.VeiculoTracao = veiculo.VeiculosTracao.FirstOrDefault();
                    else
                        pedido.VeiculoTracao = veiculo;
                }

                if (pedido.VeiculoTracao?.VeiculosVinculados != null)
                {
                    foreach (Dominio.Entidades.Veiculo reboque in pedido.VeiculoTracao.VeiculosVinculados)
                        pedido.Veiculos.Add(reboque);
                }
            }
            else
            {
                pedido.Veiculos.Add(veiculo);
            }

            if (veiculoMotorista != null)
                pedido.Motoristas.Add(veiculoMotorista);
            else if (pedido.VeiculoTracao != null)
            {
                veiculoMotorista = repVeiculoMotorista.BuscarMotoristaPrincipal(pedido.VeiculoTracao.Codigo);
                if (veiculoMotorista != null)
                {
                    pedido.Motoristas.Clear();
                    pedido.Motoristas.Add(veiculoMotorista);
                }
            }

            List<int> codigosMotoristas = pedido.Motoristas.Select(o => o.Codigo).ToList();
            List<int> codigosVeiculos = pedido.Veiculos.Select(o => o.Codigo).ToList();
            if (pedido.VeiculoTracao != null)
                codigosVeiculos.Add(pedido.VeiculoTracao.Codigo);
            ValidarRegrasLicenca(codigosMotoristas, codigosVeiculos, unitOfWork);

            //AtualizarCargaPorPedido(pedido, unitOfWork);
            pedido.MotoristaCiente = false;
            repositorioPedido.Atualizar(pedido);

            AtualizarCarga(pedido.Codigo, unitOfWork);
            Servicos.Embarcador.Integracao.IntegracaoPedido.ReenviarIntegracaoPedidos(pedido.Codigo, unitOfWork);

            Servicos.Auditoria.Auditoria.Auditar(Auditado, pedido, $"Definido o veículo {veiculo.Placa_Formatada} para o pedido", unitOfWork);
        }

        private void RemoverVeiculoDoPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo servicoFilaCarregamentoVeiculo = new Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo(unitOfWork, OrigemAlteracaoFilaCarregamento.Sistema);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorPedido(pedido.Codigo);

            var veiculoRemovido = pedido.VeiculoTracao;
            var veiculosRemovidos = pedido.Veiculos;

            pedido.Veiculos.Clear();
            pedido.VeiculoTracao = null;

            repositorioPedido.Atualizar(pedido);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                if (cargaPedido.Carga.SituacaoCarga != SituacaoCarga.Nova && cargaPedido.Carga.SituacaoCarga != SituacaoCarga.AgNFe)
                    throw new ControllerException($"Não é possível remover o veículo pois a carga já está na etapa {cargaPedido.Carga.SituacaoCarga.ObterDescricao()}.");

                cargaPedido.Carga.Veiculo = null;
                cargaPedido.Carga.VeiculosVinculados.Clear();

                repCarga.Atualizar(cargaPedido.Carga);
            }

            AtualizarCarga(pedido.Codigo, unitOfWork);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                if (veiculoRemovido != null)
                    servicoFilaCarregamentoVeiculo.RealocarVeiculoNaFila(veiculoRemovido.Codigo, TipoServicoMultisoftware);

                if (veiculosRemovidos.Count > 0)
                {
                    foreach (var veiculo in veiculosRemovidos)
                    {
                        servicoFilaCarregamentoVeiculo.RealocarVeiculoNaFila(veiculo.Codigo, TipoServicoMultisoftware);
                    }

                }

            }

            Servicos.Auditoria.Auditoria.Auditar(Auditado, pedido, $"Removeu o veículo do pedido.", unitOfWork);
        }

        private void RemoverMotoristaDoPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorPedido(pedido.Codigo);

            pedido.Motoristas.Clear();
            pedido.MotoristaCiente = false;

            repositorioPedido.Atualizar(pedido);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                if (cargaPedido.Carga.SituacaoCarga != SituacaoCarga.Nova && cargaPedido.Carga.SituacaoCarga != SituacaoCarga.AgNFe)
                    throw new ControllerException($"Não é possível remover o veículo pois a carga já está na etapa {cargaPedido.Carga.SituacaoCarga.ObterDescricao()}.");

                cargaPedido.Carga.Motoristas.Clear();

                repCarga.Atualizar(cargaPedido.Carga);
            }

            AtualizarCarga(pedido.Codigo, unitOfWork);

            Servicos.Auditoria.Auditoria.Auditar(Auditado, pedido, $"Removeu o motorista do pedido.", unitOfWork);
        }

        private void AtualizarCargaPorPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

            servicoCarga.AtualizarCargaPorPedido(pedido, TipoServicoMultisoftware, unitOfWork, ConfiguracaoEmbarcador, ClienteMultisoftware: Cliente, Auditado, true);
        }

        private List<Dominio.Entidades.Embarcador.Pedidos.Pedido> ObterPlanejamentoPedido(Repositorio.UnitOfWork unitOfWork)
        {
            var listaItensSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensSelecionados"));

            List<int> codigos = new List<int>();

            foreach (var item in listaItensSelecionados)
            {
                codigos.Add((int)item.Codigo);
            }

            Repositorio.Embarcador.Pedidos.Pedido repPedidos = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

            return repPedidos.BuscarPorCodigos(codigos);
        }

        private void EnviarEmail(string emailRecebimento, string mensagem, string assunto, string caminhoArquivoAnexo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork);
            Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo();

            if (email == null)
                throw new ControllerException("Não há um e-mail configurado para realizar o envio.");

            if (string.IsNullOrWhiteSpace(emailRecebimento))
                throw new ControllerException("Não foi informado os e-mails para recebimento.");

            if (!string.IsNullOrWhiteSpace(email.MensagemRodape))
                mensagem += "<br/>" + "<br/>" + "<br/>" + email.MensagemRodape.Replace("#qLinha#", "<br/>");
            string mensagemErro = "Erro ao enviar e-mail";

            List<string> emails = new List<string>();
            emails.AddRange(emailRecebimento.Split(';').ToList());

            List<Attachment> anexos = new List<Attachment>();

            if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoArquivoAnexo))
            {
                Attachment anexo = new Attachment(caminhoArquivoAnexo);
                anexo.Name = System.IO.Path.GetFileName(caminhoArquivoAnexo);
                anexos.Add(anexo);
            }

            emails = emails.Distinct().ToList();
            bool sucesso = Servicos.Email.EnviarEmail(email.Email, email.Email, email.Senha, null, emails.ToArray(), null, assunto, mensagem, email.Smtp, out mensagemErro, email.DisplayEmail, anexos, "", email.RequerAutenticacaoSmtp, "", email.PortaSmtp, unitOfWork);
            if (!sucesso)
                throw new Exception("Problemas ao enviar por e-mail: " + mensagemErro);
        }

        private string GerarRelatorioPedidosSelecionados(Repositorio.UnitOfWork unitOfWork, out List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos)
        {
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            pedidos = ObterPlanejamentoPedido(unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosOrdenados = pedidos.OrderBy(o => o.DataCarregamentoPedido).ToList();

            List<Dominio.Relatorios.Embarcador.DataSource.Pedidos.PlanejamentoPedido> dsPlanejamentoPedido = new List<Dominio.Relatorios.Embarcador.DataSource.Pedidos.PlanejamentoPedido>();
            foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidosOrdenados)
            {
                string dataFormatada = pedido?.DataCarregamentoPedido?.ToString("dd/MM/yyyy") ?? string.Empty;
                Dominio.Relatorios.Embarcador.DataSource.Pedidos.PlanejamentoPedido planejamento = new Dominio.Relatorios.Embarcador.DataSource.Pedidos.PlanejamentoPedido
                {
                    Data = dataFormatada,
                    Destino = pedido.Destino?.DescricaoCidadeEstado ?? string.Empty,
                    Origem = pedido.Origem?.DescricaoCidadeEstado ?? string.Empty,
                    ModeloVeicular = pedido.ModeloVeicularCarga?.Descricao ?? string.Empty,
                    Quantidade = pedido.Ordem,
                    Motorista = string.Join(", ", repositorioPedido.BuscarMotoristas(pedido.Codigo)),
                    Veiculo = ObterPlacas(pedido, unitOfWork),
                    NumeroCarga = pedido.CodigoCargaEmbarcador,
                    NumeroPedido = pedido.Numero.ToString("D")
                };

                dsPlanejamentoPedido.Add(planejamento);
            }

            if (dsPlanejamentoPedido.Count == 0)
                throw new ControllerException("Nenhum pedido selecionado");

            var relatorio = ReportRequest.WithType(ReportType.PlanejamentoPedido)
                .WithExecutionType(ExecutionType.Sync)
                .AddExtraData("dataSet", dsPlanejamentoPedido.ToJson())
                .CallReport()
                .GetContentFile();

            string pastaRelatorios = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRelatoriosEmbarcador.ConvertToOSPlatformPath();

            string nomeArquivo = Utilidades.IO.FileStorageService.Storage.Combine(pastaRelatorios, $"Pedidos_{DateTime.Now.ToString("dd-MM-yyyy_h-mm")}.pdf");

            Utilidades.IO.FileStorageService.Storage.WriteAllBytes(nomeArquivo, relatorio);

            return nomeArquivo;
        }

        private void AtualizarCarga(int codigoPedido, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repPedido.BuscarPorCodigo(codigoPedido);
            Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorPedido(codigoPedido);

            if (carga != null)
            {
                servicoCarga.AtualizarCargaPorPedido(pedido, carga, false, TipoServicoMultisoftware, Cliente, unitOfWork, ConfiguracaoEmbarcador, Auditado, true);
                repPedido.Atualizar(pedido);
            }
        }

        private void ValidarRegrasLicenca(List<int> codigosMotoristas, List<int> codigosVeiculos, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Transportadores.MotoristaLicenca repMotoristaLicenca = new Repositorio.Embarcador.Transportadores.MotoristaLicenca(unitOfWork);
            Repositorio.Embarcador.Veiculos.LicencaVeiculo repLicencaVeiculo = new Repositorio.Embarcador.Veiculos.LicencaVeiculo(unitOfWork);

            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Pedidos/PlanejamentoPedidoTMS");

            if (Usuario.UsuarioAdministrador || permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.PlanejamentoPedidoTMS_PermiteDefinirVeiculoMotoristaLicencaVencida))
                return;

            if (codigosMotoristas?.Count > 0)
            {
                Dominio.Entidades.Embarcador.Transportadores.MotoristaLicenca licencaMotorista = repMotoristaLicenca.BuscarLicencaParaBloqueioPlanejamentoPedido(codigosMotoristas);
                if (licencaMotorista != null)
                    throw new ControllerException($"O Motorista {licencaMotorista.Motorista.Nome} possui a licença {licencaMotorista.Descricao} de número {licencaMotorista.Numero} vencida na data {licencaMotorista.DataVencimento?.ToString("dd/MM/yyyy") ?? ""}, favor verifique antes de prosseguir com o planejamento do pedido.");
            }

            if (codigosVeiculos?.Count > 0)
            {
                Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo licenca = repLicencaVeiculo.BuscarLicencaParaBloqueioPlanejamentoPedido(codigosVeiculos);
                if (licenca != null)
                    if(licenca.Status == StatusLicenca.Vencido)
                    {
                        throw new ControllerException($"O Veículo {licenca.Veiculo.Placa_Formatada} possui a licença {licenca.Descricao} de número {licenca.Numero} com o status {licenca.Status.ObterDescricao() ?? ""}, favor verifique antes de prosseguir com o planejamento do pedido.");
                    }
                    else
                    {
                        throw new ControllerException($"O Veículo {licenca.Veiculo.Placa_Formatada} possui a licença {licenca.Descricao} de número {licenca.Numero} vencida na data {licenca.DataVencimento?.ToString("dd/MM/yyyy") ?? ""}, favor verifique antes de prosseguir com o planejamento do pedido.");
                    }
            }
        }

        private void EnviarEmailOrdemColeta(List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Servicos.Embarcador.Pedido.ImpressaoPedido serImpressaoPedido = new Servicos.Embarcador.Pedido.ImpressaoPedido(unitOfWork);

            DateTime dataEnvioEmail = DateTime.Now;
            foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidos)
            {
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorPedido(pedido.Codigo);
                if (serImpressaoPedido.GerarRelatorioTMS(true, pedido, false, out string msg, (carga == null ? false : true), carga, pedido.Codigo, true, false, _conexao.StringConexao, TipoServicoMultisoftware, Cliente.NomeFantasia, this.Usuario, false, out string guidRelatorio, out string fileName))
                {
                    if (!string.IsNullOrWhiteSpace(guidRelatorio))
                    {
                        //unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
                        Repositorio.Embarcador.Pessoas.ClienteOutroEmail repEmail = new Repositorio.Embarcador.Pessoas.ClienteOutroEmail(unitOfWork);
                        string pastaRelatorios = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRelatoriosEmbarcador.ConvertToOSPlatformPath();
                        string caminhoArquivo = Utilidades.IO.FileStorageService.Storage.Combine(pastaRelatorios, guidRelatorio);
                        string caminhoArquivoFileName = Utilidades.IO.FileStorageService.Storage.Combine(pastaRelatorios, fileName.Replace("-", ""));

                        Dominio.Entidades.Cliente tomador = pedido.ObterTomador();

                        if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoArquivo + ".pdf") && tomador != null)
                        {
                            List<Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEmail> emails = repEmail.BuscarPorCNPJCPFClienteTipo(tomador.CPF_CNPJ, TipoEmail.Coleta);

                            if (pedido.TipoTomador != Dominio.Enumeradores.TipoTomador.Remetente)
                            {
                                List<Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEmail> emailsRemetente = repEmail.BuscarPorCNPJCPFClienteTipo(pedido.Remetente.CPF_CNPJ, TipoEmail.Coleta);
                                emails.AddRange(emailsRemetente);
                            }

                            if (tomador.GrupoPessoas != null)
                            {
                                Repositorio.Embarcador.Pessoas.GrupoPessoasEmailDocumento repGrupoPessoasDocumentos = new Repositorio.Embarcador.Pessoas.GrupoPessoasEmailDocumento(unitOfWork);
                                List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasModeloDocumentoEmail> emailsGrupoPessoas = repGrupoPessoasDocumentos.BuscarTodosPorEntidade(tomador.GrupoPessoas);

                                if (emailsGrupoPessoas.Count > 0 && emailsGrupoPessoas.Where(x => x.ModeloDocumentoFiscal.Numero == "20").Any())
                                {
                                    var emailGrupoPessoas = emailsGrupoPessoas.Where(x => x.ModeloDocumentoFiscal.Numero == "20").FirstOrDefault();

                                    emails.Add(new ClienteOutroEmail
                                    {
                                        Email = emailGrupoPessoas.Emails
                                    });
                                }
                            }

                            if (emails != null && emails.Count > 0)
                            {
                                string emailsEnvio = string.Join(";", emails.Select(c => c.Email).ToList());

                                string assuntoemail = $"ORDEM DE COLETA - {pedido.Motoristas.FirstOrDefault()?.Nome ?? ""} - {(pedido.Empresa?.Descricao ?? "")} - REF. {pedido.NumeroPedidoEmbarcador}";
                                string mensagememail = $"ORDEM DE COLETA - {pedido.Motoristas.FirstOrDefault()?.Nome ?? ""} - {(pedido.Empresa?.Descricao ?? "")} - REF. {pedido.NumeroPedidoEmbarcador}";

                                EnviarEmail(emailsEnvio, mensagememail, assuntoemail, (caminhoArquivo + ".pdf"), unitOfWork);
                                Servicos.Auditoria.Auditoria.Auditar(Auditado, pedido, $"Enviou Ordem de Coleta para o tomador.", unitOfWork);
                            }
                        }
                        else if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoArquivoFileName) && tomador != null)
                        {
                            List<Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEmail> emails = repEmail.BuscarPorCNPJCPFClienteTipo(tomador.CPF_CNPJ, TipoEmail.Coleta);

                            if (pedido.TipoTomador != Dominio.Enumeradores.TipoTomador.Remetente)
                            {
                                List<Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEmail> emailsRemetente = repEmail.BuscarPorCNPJCPFClienteTipo(pedido.Remetente.CPF_CNPJ, TipoEmail.Coleta);
                                emails.AddRange(emailsRemetente);
                            }

                            if(tomador.GrupoPessoas != null)
                            {
                                Repositorio.Embarcador.Pessoas.GrupoPessoasEmailDocumento repGrupoPessoasDocumentos = new Repositorio.Embarcador.Pessoas.GrupoPessoasEmailDocumento(unitOfWork);
                                List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasModeloDocumentoEmail> emailsGrupoPessoas = repGrupoPessoasDocumentos.BuscarTodosPorEntidade(tomador.GrupoPessoas);

                                if (emailsGrupoPessoas.Count > 0 && emailsGrupoPessoas.Where(x => x.ModeloDocumentoFiscal.Numero == "20").Any())
                                {
                                    var emailGrupoPessoas = emailsGrupoPessoas.Where(x => x.ModeloDocumentoFiscal.Numero == "20").FirstOrDefault();

                                    emails.Add(new ClienteOutroEmail
                                    {
                                        Email = emailGrupoPessoas.Emails
                                    });
                                }
                            }

                            if (emails != null && emails.Count > 0)
                            {
                                string emailsEnvio = string.Join(";", emails.Select(c => c.Email).ToList());

                                string assuntoemail = $"ORDEM DE COLETA - {pedido.Motoristas.FirstOrDefault()?.Nome ?? ""} - {(pedido.Empresa?.Descricao ?? "")} - REF. {pedido.NumeroPedidoEmbarcador}";
                                string mensagememail = $"ORDEM DE COLETA - {pedido.Motoristas.FirstOrDefault()?.Nome ?? ""} - {(pedido.Empresa?.Descricao ?? "")} - REF. {pedido.NumeroPedidoEmbarcador}";

                                EnviarEmail(emailsEnvio, mensagememail, assuntoemail, (caminhoArquivoFileName), unitOfWork);
                                Servicos.Auditoria.Auditoria.Auditar(Auditado, pedido, $"Enviou Ordem de Coleta para o tomador.", unitOfWork);
                            }
                        }
                    }
                    else
                        throw new ControllerException("Não foi possível gerar a ordem de coleto para o tomador, favor realize a operação manual.");
                }
                else
                    throw new ControllerException("Não foi possível gerar a ordem de coleto para o tomador, favor realize a operação manual.");
            }
        }

        private string ValidarVeiculoDuplicadoPedidoCargaAberto(Dominio.Entidades.Veiculo veiculo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = repConfiguracaoPedido.BuscarConfiguracaoPadrao();

            if (!configuracaoPedido.NaoPermitirInformarVeiculoDuplicadoPedidoCargaAberta)
                return null;

            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repPedido.ObterPrimeiroPedidoPorVeiculoESituacao(veiculo.Placa, SituacaoPedido.Aberto);

            if (pedido != null)
                return "Essa placa já está vinculada a um pedido sem carga! Deseja prosseguir com a inclusão deste veículo mesmo assim?";

            List<Dominio.Entidades.Embarcador.Cargas.Carga> carga = repCarga.BuscaCargaEmAbertoPorVeiculo(veiculo.Placa);

            if (carga.Count > 0)
                return "Essa placa já está vinculada a uma carga aberta! Deseja prosseguir com a inclusão deste veículo mesmo assim?";

            return null;
        }

        #endregion
    }
}
