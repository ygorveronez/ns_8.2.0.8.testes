using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Servicos.Embarcador.Pedido;
using SGTAdmin.Controllers;
using System.Data.SqlClient;
using System.Net;
using System.Text;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize(new string[] { "Imprimir" }, "Logistica/AgendamentoColeta")]
    public class AgendamentoColetaController : BaseController
    {
        #region Construtores

        public AgendamentoColetaController(Conexao conexao) : base(conexao) { }

        #endregion

        private class RetornoArquivo
        {
            public int codigo { get; set; }
            public string nome { get; set; }
            public bool processada { get; set; }
            public string mensagem { get; set; }
        }

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = ObterGridPesquisa();

                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAgendamentoColeta filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);

                int totalRegistros = 0;

                var retorno = ExecutarPesquisa(filtrosPesquisa, ref totalRegistros, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                grid.AdicionaRows(retorno);
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

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = ObterGridPesquisa();

                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAgendamentoColeta filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);

                int totalRegistros = 0;

                var retorno = ExecutarPesquisa(filtrosPesquisa, ref totalRegistros, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                grid.AdicionaRows(retorno);

                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
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

        public async Task<IActionResult> BuscarDatasPermitidasAgendamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                dynamic pedidos = JsonConvert.DeserializeObject<dynamic>(Request.Params("Pedidos"));

                List<DateTime> datasInicias = new List<DateTime>();
                List<DateTime> datasFinais = new List<DateTime>();

                foreach (dynamic pedido in pedidos)
                {
                    datasInicias.Add(((string)pedido.DataInicioJanelaDescarga).ToDateTime());
                    datasFinais.Add(((string)pedido.DataFimJanelaDescarga).ToDateTime());
                }

                DateTime dataInicial = datasInicias.Min();
                DateTime dataFinal = datasFinais.Min();

                if (dataInicial > dataFinal)
                    return new JsonpResult(false, true, "Data inicial ultrapassa a data final.");

                return new JsonpResult(
                new
                {
                    DataInicial = dataInicial.ToString("dd/MM/yyyy HH:mm"),
                    DataFinal = dataFinal.ToString("dd/MM/yyyy HH:mm")
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar as datas permitidas.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> PesquisaNFe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
                Repositorio.Embarcador.Logistica.AgendamentoColeta repAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(unitOfWork);
                Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal repCarregamentoPedidoNotaFiscal = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");
                Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamento = repAgendamentoColeta.BuscarPorCodigo(codigo, false);

                if (codigo > 0 && agendamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("ExigeChaveVenda", false);
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Volumes", false);
                grid.AdicionarCabecalho("Pallets", false);
                grid.AdicionarCabecalho("MetrosCubicos", false);
                grid.AdicionarCabecalho("ChaveVenda", false);
                grid.AdicionarCabecalho("Número", "Numero", 6, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Chave", "Chave", 15, Models.Grid.Align.left, false, true, true, false, true);
                grid.AdicionarCabecalho("Emitente", "Emitente", 14, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Origem", "Origem", 9, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Destinatário", "Destinatario", 14, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Destino", "Destino", 9, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Modalidade Pgto.", "DescricaoModalidadeFrete", 5, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Peso", "Peso", 6, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Valor", "Valor", 6, Models.Grid.Align.left, true);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdenar == "Emitente" || propOrdenar == "Destinatario")
                    propOrdenar += ".Nome";
                else if (propOrdenar == "Destino")
                    propOrdenar = "Destinatario.Localidade.Descricao";
                else if (propOrdenar == "DescricaoModalidadeFrete")
                    propOrdenar = "ModalidadeFrete";
                propOrdenar = "XMLNotaFiscal." + propOrdenar;

                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> listaNotasFiscais = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();

                if (agendamento != null)
                {
                    if (agendamento.Pedido != null)
                    {
                        listaNotasFiscais = agendamento.Pedido.NotasFiscais.ToList();
                    }

                    if (!ConfiguracaoEmbarcador.ControlarAgendamentoSKU && agendamento.Carga?.Carregamento != null)
                    {
                        List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal> listaNotasEnviadas = repCarregamentoPedidoNotaFiscal.BuscarPorCarregamento(agendamento.Carga.Carregamento.Codigo);

                        listaNotasFiscais = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();
                        foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal notaFiscal in listaNotasEnviadas)
                            listaNotasFiscais.AddRange(notaFiscal.NotasFiscais.ToList());
                    }

                    if (listaNotasFiscais.Count == 0 && agendamento.Carga != null)
                    {
                        List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorCarga(agendamento.Carga.Codigo);
                        foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                        {
                            listaNotasFiscais.AddRange(cargaPedido.NotasFiscais.Select(nf => nf.XMLNotaFiscal).ToList());
                        }
                    }

                    if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
                        listaNotasFiscais = listaNotasFiscais.Where(obj => obj.Emitente.Codigo == Usuario.ClienteFornecedor.CPF_CNPJ).ToList();
                }

                var dynXmlNotaFiscal = (from obj in listaNotasFiscais
                                        where obj.nfAtiva == true
                                        select new
                                        {
                                            ExigeChaveVenda = agendamento.Carga?.TipoOperacao?.ExigeChaveVendaAntesConfirmarNotas ?? false,
                                            obj.Codigo,
                                            obj.Numero,
                                            obj.Chave,
                                            Emitente = obj.Emitente != null ? obj.Emitente.Descricao : "",
                                            Origem = obj.TipoOperacaoNotaFiscal == TipoOperacaoNotaFiscal.Saida ? obj.Emitente != null ? obj.Emitente.Localidade.DescricaoCidadeEstado : "" : obj.Destinatario != null ? obj.Destinatario.Localidade.DescricaoCidadeEstado : "",
                                            Destinatario = obj.Destinatario != null ? obj.Destinatario.Descricao : "",
                                            Destino = obj.TipoOperacaoNotaFiscal == TipoOperacaoNotaFiscal.Saida ? obj.Destinatario != null ? obj.Destinatario.Localidade.DescricaoCidadeEstado : "" : obj.Emitente != null ? obj.Emitente.Localidade.DescricaoCidadeEstado : "",
                                            obj.DescricaoModalidadeFrete,
                                            obj.Valor,
                                            Peso = obj.Peso.ToString("n3"),
                                            ChaveVenda = obj.ChaveVenda,
                                            obj.Volumes,
                                            obj.MetrosCubicos,
                                            Pallets = obj.QuantidadePallets
                                        }).ToList();

                grid.setarQuantidadeTotal(dynXmlNotaFiscal.Count);
                grid.AdicionaRows(dynXmlNotaFiscal);

                return new JsonpResult(grid);
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

        public async Task<IActionResult> PesquisaLacre()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
                Repositorio.Embarcador.Logistica.AgendamentoColeta repAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");
                Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamento = repAgendamentoColeta.BuscarPorCodigo(codigo, false);

                if (agendamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Lacre", "Numero", !ConfiguracaoEmbarcador.ExibirTipoLacre ? 80 : 20, Models.Grid.Align.left, true);

                Repositorio.Embarcador.Cargas.CargaLacre repCargaLacre = new Repositorio.Embarcador.Cargas.CargaLacre(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.CargaLacre> listaLacres = repCargaLacre.Consultar(agendamento.Carga?.Codigo ?? 0, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);

                var lista = (from p in listaLacres
                             select new
                             {
                                 p.Codigo,
                                 p.Numero,
                                 TipoLacre = p.TipoLacre?.Descricao ?? string.Empty,
                                 Cliente = p.Cliente?.Descricao ?? string.Empty
                             }).ToList();

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(repCargaLacre.ContarConsulta(agendamento.Carga?.Codigo ?? 0));

                return new JsonpResult(grid);
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

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Servicos.Embarcador.Logistica.CargaJanelaDescarregamento servicoJanelaDescarregamento = new Servicos.Embarcador.Logistica.CargaJanelaDescarregamento(unitOfWork);
                Servicos.Embarcador.Logistica.AgendamentoColeta servicoAgendamentoColeta = new Servicos.Embarcador.Logistica.AgendamentoColeta(unitOfWork);
                Servicos.Embarcador.Logistica.AgendamentoColeta servicoAgendamento = new Servicos.Embarcador.Logistica.AgendamentoColeta(unitOfWork);
                Servicos.Embarcador.Email.ConfiguracaoModeloEmail servicoConfiguracaoModeloEmail = new Servicos.Embarcador.Email.ConfiguracaoModeloEmail(_conexao.StringConexao, Usuario, Auditado);

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Logistica.AgendamentoColeta repositorioAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoAgendamentoColeta repositorioConfiguracaoAgendamentoColeta = new Repositorio.Embarcador.Configuracoes.ConfiguracaoAgendamentoColeta(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAgendamentoColeta configuracaoAgendamentoColeta = repositorioConfiguracaoAgendamentoColeta.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta = new Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta()
                {
                    EtapaAgendamentoColeta = ConfiguracaoEmbarcador.ControlarAgendamentoSKU ? EtapaAgendamentoColeta.AguardandoAceite : configuracaoAgendamentoColeta.RemoverEtapaAgendamentoAgendamentoColeta ? EtapaAgendamentoColeta.NFe : EtapaAgendamentoColeta.DadosTransporte,
                };

                PreencherEntidade(agendamentoColeta, unitOfWork);
                repositorioAgendamentoColeta.Inserir(agendamentoColeta, Auditado);

                if (ConfiguracaoEmbarcador.ControlarAgendamentoSKU)
                    SalvarPedidosAgendamento(agendamentoColeta, unitOfWork, Cliente, configuracaoGeralCarga);

                try
                {
                    if (agendamentoColeta.ApenasGerarPedido)
                    {
                        agendamentoColeta.Situacao = null;
                        agendamentoColeta.EtapaAgendamentoColeta = EtapaAgendamentoColeta.NFe;
                        agendamentoColeta.Pedido = AdicionarPedido(agendamentoColeta, unitOfWork);
                    }
                    else
                    {
                        agendamentoColeta.Situacao = SituacaoAgendamentoColeta.AguardandoConfirmacao;
                        agendamentoColeta.Carga = AdicionarCarga(agendamentoColeta, unitOfWork);
                        agendamentoColeta.CodigoControle = agendamentoColeta.Carga.Codigo;
                    }

                    repositorioAgendamentoColeta.Atualizar(agendamentoColeta);
                    servicoAgendamento.SubtrairSaldoVolumesPendentesPedidos(agendamentoColeta, unitOfWork);

                    if (!agendamentoColeta.ApenasGerarPedido)
                    {
                        servicoJanelaDescarregamento.Adicionar(agendamentoColeta, TipoServicoMultisoftware);

                        Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repositorioJanelaDescarga = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento(unitOfWork);
                        Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarga = repositorioJanelaDescarga.BuscarPorCarga(agendamentoColeta.Carga.Codigo);

                        if (cargaJanelaDescarga?.Situacao == SituacaoCargaJanelaDescarregamento.AguardandoConfirmacaoAgendamento)
                        {
                            agendamentoColeta.Carga.OcultarNoPatio = true;
                            repositorioCarga.Atualizar(agendamentoColeta.Carga);
                        }

                        Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repositorioCargaJanelaCarregamentoGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(unitOfWork);
                        Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita guarita = repositorioCargaJanelaCarregamentoGuarita.BuscarPorCarga(agendamentoColeta.Carga.Codigo);

                        if (guarita != null)
                        {
                            guarita.TipoChegadaGuarita = TipoChegadaGuarita.Descarregamento;
                            repositorioCargaJanelaCarregamentoGuarita.Atualizar(guarita);
                        }

                        if (!ConfiguracaoEmbarcador.NaoGerarSenhaAgendamento)
                            agendamentoColeta.Senha = servicoAgendamentoColeta.ObterSenhaAgendamentoColeta(cargaJanelaDescarga, agendamentoColeta, configuracaoAgendamentoColeta);
                    }

                    if (agendamentoColeta.TipoOperacao?.ConfiguracaoAgendamentoColetaEntrega?.RemoverEtapaAgendamentoDoAgendamentoColeta ?? false)
                    {
                        agendamentoColeta.EtapaAgendamentoColeta = EtapaAgendamentoColeta.NFe;
                        repositorioAgendamentoColeta.Atualizar(agendamentoColeta);
                    }

                    unitOfWork.CommitChanges();
                }
                catch (Exception excecao) when (excecao.InnerException is SqlException)
                {
                    Servicos.Log.TratarErro(excecao);

                    if (((SqlException)excecao.InnerException).Number != 2627)
                        throw;

                    unitOfWork.Rollback();

                    return new JsonpResult(new
                    {
                        ViolacaoUniqueKey = true
                    });
                }

                System.Threading.Tasks.Task.Factory.StartNew(() => EnviarEmailAgendamentoColetaAdicionado(agendamentoColeta));
                System.Threading.Tasks.Task.Factory.StartNew(() => EnviarEmailAgendamentoAdicionadoParaRemetentePedido(agendamentoColeta, Cliente));
                System.Threading.Tasks.Task.Factory.StartNew(() => servicoConfiguracaoModeloEmail.EnviarEmails(agendamentoColeta, TipoGatilhoNotificacao.AdicionarAgendamento));

                return new JsonpResult(new
                {
                    agendamentoColeta.Codigo
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
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AdicionarLacre()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                Repositorio.Embarcador.Cargas.CargaLacre repCargaLacre = new Repositorio.Embarcador.Cargas.CargaLacre(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Logistica.AgendamentoColeta repAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");
                Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamento = repAgendamentoColeta.BuscarPorCodigo(codigo, false);

                if (agendamento == null || agendamento.Carga == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                string numero = Request.GetStringParam("Numero");

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(agendamento.Carga.Codigo);

                serCarga.ValidarPermissaoAlterarDadosEtapaFrete(carga, unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CargaLacre cargaLacre = new Dominio.Entidades.Embarcador.Cargas.CargaLacre
                {
                    Carga = carga,
                    Numero = numero,
                };

                repCargaLacre.Inserir(cargaLacre, Auditado);

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirLacre()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaLacre repCargaLacre = new Repositorio.Embarcador.Cargas.CargaLacre(unitOfWork);
                Repositorio.Embarcador.Logistica.AgendamentoColeta repAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(unitOfWork);

                int codigoAgendamento = Request.GetIntParam("Agendamento");
                int codigo = Request.GetIntParam("Codigo");
                Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamento = repAgendamentoColeta.BuscarPorCodigo(codigoAgendamento, false);

                if (agendamento == null || agendamento.Carga == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Dominio.Entidades.Embarcador.Cargas.CargaLacre cargaLacre = repCargaLacre.BuscarPorCodigo(codigo);

                if (agendamento.Carga.Codigo != cargaLacre.Carga.Codigo)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                serCarga.ValidarPermissaoAlterarDadosEtapaFrete(cargaLacre.Carga, unitOfWork);

                repCargaLacre.Deletar(cargaLacre, Auditado);

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AtualizarTransportador()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Logistica.AgendamentoColeta repositorioAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();

                int codigo = Request.GetIntParam("Codigo");
                int codigoEmpresa = Request.GetIntParam("Transportador");

                Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta = repositorioAgendamentoColeta.BuscarPorCodigo(codigo, true);
                Dominio.Entidades.Empresa transportado = repositorioEmpresa.BuscarPorCodigo(codigoEmpresa);

                if (agendamentoColeta == null)
                    return new JsonpResult(false, "Agendamento não encontrado.");

                if (transportado == null)
                    return new JsonpResult(false, "Transportador não encontrado.");

                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorCarga(agendamentoColeta.Carga.Codigo);

                if (cargaJanelaCarregamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar a janela de carregamento.");

                Dominio.Entidades.Embarcador.Cargas.Carga carga = agendamentoColeta.Carga;

                agendamentoColeta.Transportador = transportado;
                repositorioAgendamentoColeta.Atualizar(agendamentoColeta, Auditado);

                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador = repositorioCargaJanelaCarregamentoTransportador.BuscarSemRejeicao(cargaJanelaCarregamento.Codigo, codigoEmpresa);

                if (
                   (cargaJanelaCarregamentoTransportador != null) &&
                   (
                       (cargaJanelaCarregamentoTransportador.Situacao == SituacaoCargaJanelaCarregamentoTransportador.AgAceite) ||
                       (cargaJanelaCarregamentoTransportador.Situacao == SituacaoCargaJanelaCarregamentoTransportador.AgConfirmacao) ||
                       (cargaJanelaCarregamentoTransportador.Situacao == SituacaoCargaJanelaCarregamentoTransportador.Confirmada && !ConfiguracaoEmbarcador.PermitirDisponibilizarCargaParaTransportador)
                   )
               )
                    return new JsonpResult(false, true, "A carga já está disponível para o transportador.");

                carga.DataAtualizacaoCarga = DateTime.Now;
                carga.Empresa = transportado;
                carga.Motoristas?.Clear();
                carga.RejeitadaPeloTransportador = false;
                carga.Veiculo = null;
                carga.VeiculosVinculados?.Clear();

                repositorioCarga.Atualizar(carga);

                Servicos.Embarcador.Logistica.CargaJanelaCarregamento servicoCargaJanelaCarregamento = new Servicos.Embarcador.Logistica.CargaJanelaCarregamento(unitOfWork, configuracaoEmbarcador);
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportador servicoCargaJanelaCarregamentoTransportador = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportador(unitOfWork, configuracaoEmbarcador);
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoNotificacao servicoCargaJanelaCarregamentoNotificacao = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoNotificacao(unitOfWork, configuracaoEmbarcador, null);

                if (cargaJanelaCarregamentoTransportador != null)
                {
                    cargaJanelaCarregamentoTransportador.Situacao = SituacaoCargaJanelaCarregamentoTransportador.AgConfirmacao;
                    repositorioCargaJanelaCarregamentoTransportador.Atualizar(cargaJanelaCarregamentoTransportador);
                    servicoCargaJanelaCarregamentoTransportador.SalvarHistoricoAlteracao(cargaJanelaCarregamentoTransportador, "Carga disponibilizada para o transportador confirmar os dados de transporte pelo agendamento de coleta", Usuario);
                    servicoCargaJanelaCarregamentoNotificacao.EnviarEmailCargaDisponibilizadaParaTransportador(cargaJanelaCarregamentoTransportador);
                }
                else
                {
                    cargaJanelaCarregamentoTransportador = new Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador()
                    {
                        CargaJanelaCarregamento = cargaJanelaCarregamento,
                        HorarioLiberacao = DateTime.Now,
                        PendenteCalcularFrete = ConfiguracaoEmbarcador.CalcularFreteCargaJanelaCarregamentoTransportador,
                        Situacao = SituacaoCargaJanelaCarregamentoTransportador.AgConfirmacao,
                        Transportador = transportado,
                        Tipo = TipoCargaJanelaCarregamentoTransportador.PorTipoTransportadorCarga
                    };

                    repositorioCargaJanelaCarregamentoTransportador.Inserir(cargaJanelaCarregamentoTransportador);
                    servicoCargaJanelaCarregamento.DefinirDataDisponibilizacaoTransportadores(cargaJanelaCarregamento);
                    servicoCargaJanelaCarregamentoTransportador.SalvarHistoricoAlteracao(cargaJanelaCarregamentoTransportador, "Carga disponibilizada para o transportador confirmar os dados de transporte pelo agendamento de coleta", Usuario);
                    servicoCargaJanelaCarregamentoNotificacao.EnviarEmailCargaDisponibilizadaParaTransportador(cargaJanelaCarregamentoTransportador);
                }

                servicoCargaJanelaCarregamento.AtualizarSituacao(cargaJanelaCarregamento, TipoServicoMultisoftware);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, $"Disponibilizou a carga para o transportador {transportado.Descricao}", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(new
                {
                    agendamentoColeta.Codigo
                });
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar a senha.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AtualizarInformacoesTransporte()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoAgendamentoColeta = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Logistica.AgendamentoColeta repositorioAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta = repositorioAgendamentoColeta.BuscarPorCodigo(codigoAgendamentoColeta);

                if (agendamentoColeta == null)
                    return new JsonpResult(false, true, "O registro não foi encontrado");

                Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);

                agendamentoColeta.TransportadorManual = Request.GetStringParam("TransportadorManual");
                agendamentoColeta.Placa = Request.GetStringParam("Placa");
                agendamentoColeta.Reboque = Request.GetStringParam("Reboque");
                agendamentoColeta.Motorista = Request.GetStringParam("Motorista");

                if (agendamentoColeta.Transportador == null)
                    agendamentoColeta.Transportador = repositorioEmpresa.BuscarEmpresaPadraoRetirada();

                if (agendamentoColeta.Transportador != null)
                {
                    if (!string.IsNullOrEmpty(agendamentoColeta.Placa))
                    {
                        agendamentoColeta.VeiculoSelecionado = repositorioVeiculo.BuscarPlaca(agendamentoColeta.Placa);
                        if (agendamentoColeta.VeiculoSelecionado == null)
                        {
                            Dominio.Entidades.Veiculo veiculo = new Dominio.Entidades.Veiculo()
                            {
                                Placa = agendamentoColeta.Placa,
                                Empresa = agendamentoColeta.Transportador,
                                Estado = agendamentoColeta.Transportador.Localidade?.Estado,
                                Ativo = true,
                                TipoVeiculo = "0"
                            };
                            repositorioVeiculo.Inserir(veiculo);
                            agendamentoColeta.VeiculoSelecionado = veiculo;
                        }
                    }

                    if (!string.IsNullOrEmpty(agendamentoColeta.Reboque))
                    {
                        agendamentoColeta.ReboqueSelecionado = repositorioVeiculo.BuscarPlaca(agendamentoColeta.Reboque);
                        if (agendamentoColeta.ReboqueSelecionado == null)
                        {
                            Dominio.Entidades.Veiculo veiculo = new Dominio.Entidades.Veiculo()
                            {
                                Placa = agendamentoColeta.Reboque,
                                Empresa = agendamentoColeta.Transportador,
                                Estado = agendamentoColeta.Transportador.Localidade?.Estado,
                                Ativo = true,
                                TipoVeiculo = "1"
                            };
                            repositorioVeiculo.Inserir(veiculo);
                            agendamentoColeta.ReboqueSelecionado = veiculo;
                        }
                        if (agendamentoColeta.VeiculoSelecionado != null && agendamentoColeta.ReboqueSelecionado != null)
                        {
                            agendamentoColeta.VeiculoSelecionado.VeiculosVinculados = new List<Dominio.Entidades.Veiculo>();
                            agendamentoColeta.VeiculoSelecionado.VeiculosVinculados.Add(agendamentoColeta.ReboqueSelecionado);
                            repositorioVeiculo.Atualizar(agendamentoColeta.VeiculoSelecionado);
                        }
                    }

                    if (!string.IsNullOrEmpty(agendamentoColeta.Motorista))
                    {
                        agendamentoColeta.MotoristaSelecionado = repositorioUsuario.BuscarPrimeiroMotoristaPorNome(agendamentoColeta.Motorista);
                        if (agendamentoColeta.MotoristaSelecionado == null)
                        {
                            Random rnd = new Random();
                            string cpf = Utilidades.String.OnlyNumbers(rnd.Next(999999).ToString("D"));
                            cpf = cpf.PadLeft(11, '1');
                            Dominio.Entidades.Usuario motorista = new Dominio.Entidades.Usuario()
                            {
                                Nome = agendamentoColeta.Motorista,
                                Empresa = agendamentoColeta.Transportador,
                                ExibirUsuarioAprovacao = false,
                                CPF = cpf,
                                Status = "A",
                                Tipo = "M"
                            };
                            repositorioUsuario.Inserir(motorista);
                            agendamentoColeta.MotoristaSelecionado = motorista;
                        }
                    }
                }

                if (agendamentoColeta.Carga != null)
                    AtualizarCarga(agendamentoColeta.Carga, agendamentoColeta, unitOfWork);

                repositorioAgendamentoColeta.Atualizar(agendamentoColeta);

                return new JsonpResult(true, true, "Informações atualizadas com sucesso");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar as informações de transporte");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> SalvarSenha()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Logistica.AgendamentoColeta repositorioAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(unitOfWork);

                int codigoCarga = Request.GetIntParam("Codigo");
                string senha = Request.GetStringParam("Senha");

                Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta = repositorioAgendamentoColeta.BuscarPorCarga(codigoCarga);

                if (agendamentoColeta == null)
                    return new JsonpResult(false, "Agendamento não encontrado.");

                agendamentoColeta.Senha = senha;

                repositorioAgendamentoColeta.Atualizar(agendamentoColeta);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar a senha.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Cancelar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            bool alertaExibido = Request.GetBoolParam("ExibirAlerta");

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Logistica.AgendamentoColeta repositorioAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta = repositorioAgendamentoColeta.BuscarPorCodigo(codigo, false);

                if (agendamentoColeta == null)
                    return new JsonpResult(false, "Agendamento não encontrado.");

                agendamentoColeta.Initialize();

                if (agendamentoColeta.EtapaAgendamentoColeta == EtapaAgendamentoColeta.Emissao)
                    return new JsonpResult(false, "Não é possível cancelar o agendamento na etapa atual.");

                if (agendamentoColeta.Cancelado)
                    return new JsonpResult(false, "O Agendamento já foi cancelado.");

                Repositorio.Embarcador.Logistica.AgendamentoColetaPedido repositorioAgendamentoColetaPedido = new Repositorio.Embarcador.Logistica.AgendamentoColetaPedido(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repositorioCargaJanelaDescarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento(unitOfWork);
                Servicos.Embarcador.Logistica.CargaJanelaDescarregamento servicoCargaJanelaDescarregamento = new Servicos.Embarcador.Logistica.CargaJanelaDescarregamento(unitOfWork, ConfiguracaoEmbarcador, Auditado);
                Servicos.Embarcador.Logistica.AgendamentoColeta servicoAgendamento = new Servicos.Embarcador.Logistica.AgendamentoColeta(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento = agendamentoColeta.Carga != null ? repositorioCargaJanelaDescarregamento.BuscarPorCarga(agendamentoColeta.Carga.Codigo) : null;
                bool isJanelaConfirmada = cargaJanelaDescarregamento == null ? false : cargaJanelaDescarregamento.Situacao == SituacaoCargaJanelaDescarregamento.AguardandoDescarregamento;
                List<int> codigosTipoCarga = new List<int>();

                if (agendamentoColeta.TipoCarga?.Codigo > 0)
                    codigosTipoCarga.Add(agendamentoColeta.TipoCarga.Codigo);
                else
                    codigosTipoCarga.AddRange(repositorioAgendamentoColetaPedido.BuscarCodigosTipoCargaDosPedidosPorAgendamentoColeta(agendamentoColeta.Codigo));

                string mensagemAuditoria = "Agendamento Coleta Cancelado pela tela de Agendamento.";

                if (!agendamentoColeta.ApenasGerarPedido)
                {
                    if (cargaJanelaDescarregamento != null)
                    {
                        cargaJanelaDescarregamento.Initialize();

                        if (DateTime.Now > cargaJanelaDescarregamento.InicioDescarregamento)
                            throw new ControllerException($"Não é possível cancelar a agenda pois a data atual é maior do que a data agendada.");

                        DateTime diaDoDescarregamento = cargaJanelaDescarregamento.InicioDescarregamento;
                        DateTime diaAtual = DateTime.Now;

                        Repositorio.Embarcador.Logistica.QuantidadePorTipoDeCargaDescarregamentoTipoCarga repositorioQuantidadePorTipoDeCargaDescarregamentoTipoDeCarga = new Repositorio.Embarcador.Logistica.QuantidadePorTipoDeCargaDescarregamentoTipoCarga(unitOfWork);
                        List<Dominio.Entidades.Embarcador.Logistica.QuantidadePorTipoDeCargaTipoDeCarga> quantidadePorTipoDeCargaTipoDeCarga = (codigosTipoCarga.Count > 0 && cargaJanelaDescarregamento.CentroDescarregamento != null) ? repositorioQuantidadePorTipoDeCargaDescarregamentoTipoDeCarga.BuscarPorTipoCargaDiaCentroDescarregamento(codigosTipoCarga, cargaJanelaDescarregamento.CentroDescarregamento.Codigo, diaDoDescarregamento) : new List<Dominio.Entidades.Embarcador.Logistica.QuantidadePorTipoDeCargaTipoDeCarga>();

                        foreach (int codigoTipoCarga in codigosTipoCarga)
                        {
                            int toleranciaAgendaConfirmada = quantidadePorTipoDeCargaTipoDeCarga.Count > 0 ? quantidadePorTipoDeCargaTipoDeCarga.Where(obj => obj.TipoCarga.Codigo == codigoTipoCarga && obj.QuantidadePorTipoDeCargaDescarregamento.Dia == ((DiaSemana)diaDoDescarregamento.DayOfWeek + 1)).Select(x => x.QuantidadePorTipoDeCargaDescarregamento.ToleranciaCancelamentoAgendaConfirmada).FirstOrDefault() ?? 24 : 24;
                            int toleranciaAgendaNaoConfirmada = quantidadePorTipoDeCargaTipoDeCarga.Count > 0 ? quantidadePorTipoDeCargaTipoDeCarga.Where(obj => obj.TipoCarga.Codigo == codigoTipoCarga && obj.QuantidadePorTipoDeCargaDescarregamento.Dia == ((DiaSemana)diaDoDescarregamento.DayOfWeek + 1)).Select(x => x.QuantidadePorTipoDeCargaDescarregamento.ToleranciaCancelamentoAgendaNaoConfirmada).FirstOrDefault() ?? 0 : 0;

                            double diferencaEntreDatasEmHoras = diaAtual.DifferenceOfHoursBetweenWorkDays(diaDoDescarregamento);

                            int tolerancia = cargaJanelaDescarregamento.Situacao != SituacaoCargaJanelaDescarregamento.AguardandoConfirmacaoAgendamento ? toleranciaAgendaConfirmada : toleranciaAgendaNaoConfirmada;
                            string mensagem = cargaJanelaDescarregamento.Situacao != SituacaoCargaJanelaDescarregamento.AguardandoConfirmacaoAgendamento ? "confirmados" : "não confirmados";

                            if (tolerancia > 0 && diferencaEntreDatasEmHoras < tolerancia && !alertaExibido && isJanelaConfirmada)
                                throw new ControllerException($"Para essa solicitação de cancelamento poderá ser cobrada Multa de “No Show”, pois o prazo limite para cancelamento de cobrança é de: {tolerancia} horas.", errorCode: Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.PrazoCancelamentoCarregamentoExcedidoPeloFornecedor);
                            else if (tolerancia > 0 && diferencaEntreDatasEmHoras < tolerancia && !isJanelaConfirmada)
                                throw new ControllerException($"Só é possível cancelar agendamentos {mensagem} no centro de descarregamento {cargaJanelaDescarregamento?.CentroDescarregamento?.Descricao} com, no mínimo, {toleranciaAgendaConfirmada} horas de antecedência.");
                        }
                    }

                    Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

                    bool possuiIntegracaoSAD = repositorioTipoIntegracao.ExistePorTipo(TipoIntegracao.SAD);
                    List<(string URL, int CodigoCentroDescarregamento)> urlsSad = new List<(string URL, int CodigoCentroDescarregamento)>();

                    if (possuiIntegracaoSAD && cargaJanelaDescarregamento != null)
                    {
                        Repositorio.Embarcador.Configuracoes.IntegracaoSAD repositorioSAD = new Repositorio.Embarcador.Configuracoes.IntegracaoSAD(unitOfWork);
                        List<int> codigosCentrosDescarregamento = cargaJanelaDescarregamento.CentroDescarregamento != null ? new List<int> { cargaJanelaDescarregamento.CentroDescarregamento.Codigo } : new List<int>();

                        if (codigosCentrosDescarregamento.Count > 0)
                            urlsSad = repositorioSAD.BuscarURLsCancelarAgendaPorCentrosDescarregamento(codigosCentrosDescarregamento);
                    }

                    bool gerarIntegracoes = !possuiIntegracaoSAD;
                    string urlSADCentroDescarregamento = (from obj in urlsSad where obj.CodigoCentroDescarregamento == cargaJanelaDescarregamento.CentroDescarregamento.Codigo select obj.URL).FirstOrDefault();

                    if (string.IsNullOrWhiteSpace(urlSADCentroDescarregamento))
                        urlSADCentroDescarregamento = (from obj in urlsSad where obj.CodigoCentroDescarregamento == 0 select obj.URL).FirstOrDefault();

                    if (!string.IsNullOrWhiteSpace(urlSADCentroDescarregamento))
                        gerarIntegracoes = true;

                    Dominio.ObjetosDeValor.Embarcador.Carga.CargaCancelamentoAdicionar cargaCancelamentoAdicionar = new Dominio.ObjetosDeValor.Embarcador.Carga.CargaCancelamentoAdicionar()
                    {
                        Carga = agendamentoColeta.Carga,
                        GerarIntegracoes = gerarIntegracoes,
                        MotivoCancelamento = "Cancelamento por Agendamento de Coleta",
                        TipoServicoMultisoftware = TipoServicoMultisoftware,
                        Usuario = this.Usuario
                    };

                    Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = Servicos.Embarcador.Carga.Cancelamento.GerarCargaCancelamento(cargaCancelamentoAdicionar, ConfiguracaoEmbarcador, unitOfWork);
                    Servicos.Embarcador.Carga.Cancelamento.SolicitarCancelamentoCarga(ref cargaCancelamento, unitOfWork, _conexao.StringConexao, TipoServicoMultisoftware);

                    if (cargaCancelamento.Situacao == SituacaoCancelamentoCarga.RejeicaoCancelamento)
                    {
                        unitOfWork.CommitChanges();
                        return new JsonpResult(false, cargaCancelamento.MensagemRejeicaoCancelamento);
                    }
                }
                else
                {
                    agendamentoColeta.EtapaAgendamentoColeta = EtapaAgendamentoColeta.NFeCancelada;
                    if (repCargaPedido.BuscarCargaAtualPorPedido(agendamentoColeta.Pedido.Codigo) != null)
                        return new JsonpResult(false, "Já existe uma carga com esse pedido. Não é possível cancelar o pedido");

                    agendamentoColeta.Pedido.SituacaoPedido = SituacaoPedido.Cancelado;
                    repPedido.Atualizar(agendamentoColeta.Pedido);
                }

                if (isJanelaConfirmada && alertaExibido)
                {
                    mensagemAuditoria = "Agendamento marcado como não comparecido pelo fornecedor.";
                    agendamentoColeta.Situacao = SituacaoAgendamentoColeta.NaoComparecimentoConfirmadoPeloFornecedor;

                    servicoCargaJanelaDescarregamento.AtualizarSituacao(cargaJanelaDescarregamento, SituacaoCargaJanelaDescarregamento.NaoComparecimentoConfirmadoPeloFornecedor);
                }
                else
                {
                    agendamentoColeta.Situacao = TipoServicoMultisoftware == TipoServicoMultisoftware.MultiEmbarcador ? SituacaoAgendamentoColeta.CanceladoEmbarcador : SituacaoAgendamentoColeta.Cancelado;
                    servicoCargaJanelaDescarregamento.AtualizarSituacao(cargaJanelaDescarregamento, SituacaoCargaJanelaDescarregamento.Cancelado);
                    repositorioCargaJanelaDescarregamento.Atualizar(cargaJanelaDescarregamento, Auditado);
                    agendamentoColeta.SetExternalChanges(cargaJanelaDescarregamento.GetCurrentChanges());
                }

                agendamentoColeta.DataCancelamento = DateTime.Now;
                repositorioAgendamentoColeta.Atualizar(agendamentoColeta, Auditado, null, mensagemAuditoria);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();

                if (excecao.ErrorCode == Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.PrazoCancelamentoCarregamentoExcedidoPeloFornecedor)
                    return new JsonpResult(new
                    {
                        ExibirAlerta = !alertaExibido,
                        Mensagem = excecao.Message
                    });

                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao solicitar o cancelamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Imprimir()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Logistica.AgendamentoColeta repAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repCargaJanelaDescarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamento = repAgendamentoColeta.BuscarPorCodigo(codigo, false);

                if (agendamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento = repCargaJanelaDescarregamento.BuscarPorCarga(agendamento.Carga?.Codigo ?? 0);

                byte[] arquivo = null;

                if (cargaJanelaDescarregamento.CentroDescarregamento?.UsarLayoutAgendamentoPorCaixaItem ?? false)
                    arquivo = new Servicos.Embarcador.Logistica.AgendamentoColeta(unitOfWork).ResumoAgendamentoColetaSams(agendamento, cargaJanelaDescarregamento);
                else
                    arquivo = new Servicos.Embarcador.Logistica.AgendamentoColeta(unitOfWork).ResumoAgendamentoColeta(agendamento, cargaJanelaDescarregamento);

                return Arquivo(arquivo, "application/pdf", "Resumo Agendamento - " + agendamento.Carga.CodigoCargaEmbarcador + ".pdf");
            }
            catch (ServicoException ex)
            {
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o arquivo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> InformarNotasFiscais()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Logistica.AgendamentoColeta repositorioAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

                Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta = repositorioAgendamentoColeta.BuscarPorCodigo(codigo, true);

                if (agendamentoColeta.EtapaAgendamentoColeta != EtapaAgendamentoColeta.DadosTransporte)
                    throw new Exception("Não é possível mudar a etapa.");

                agendamentoColeta.EtapaAgendamentoColeta = EtapaAgendamentoColeta.NFe;

                if (PermiteSalvarDadosTransporte(agendamentoColeta.Carga))
                {
                    if (agendamentoColeta.Carga.SituacaoRoteirizacaoCarga == SituacaoRoteirizacao.Aguardando)
                    {
                        Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracao.BuscarConfiguracaoPadrao();
                        Servicos.Embarcador.Carga.CargaRotaFrete.GerarIntegracoesRoteirizacaoCarga(agendamentoColeta.Carga, unitOfWork, configuracaoEmbarcador, TipoServicoMultisoftware);
                    }

                    servicoCarga.SolicitarNotasFiscais(agendamentoColeta.Carga, ConfiguracaoEmbarcador, TipoServicoMultisoftware, Auditado, unitOfWork);
                }

                if ((agendamentoColeta.Carga?.TipoOperacao?.FretePorContadoCliente ?? false) && (agendamentoColeta.Carga?.TipoOperacao?.NaoExigeConformacaoDasNotasEmissao ?? false))
                    EncaminharParaTransporte(agendamentoColeta, validarNotasFiscais: false, unitOfWork);

                repositorioAgendamentoColeta.Atualizar(agendamentoColeta, Auditado);

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
                return new JsonpResult(false, "Ocorreu uma falha ao informar as notas fiscais");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> EncaminharParaTransporte()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Logistica.AgendamentoColeta repositorioAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta = repositorioAgendamentoColeta.BuscarPorCodigo(codigo, true);

                if (agendamentoColeta == null)
                    throw new ControllerException("Não foi possível encontrar o registro.");

                EncaminharParaTransporte(agendamentoColeta, validarNotasFiscais: true, unitOfWork);

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
                return new JsonpResult(false, "Ocorreu uma falha ao avançar etapa.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConsultarCargaMDFe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaMDFeManual repCargaMDFeManual = new Repositorio.Embarcador.Cargas.CargaMDFeManual(unitOfWork);

                int codigoCarga = int.Parse(Request.Params("CodigoCarga"));

                Dominio.Enumeradores.StatusMDFe statusMDFe = Dominio.Enumeradores.StatusMDFe.Todos;


                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoMDFE", false);
                grid.AdicionarCabecalho("CodigoEmpresa", false);
                grid.AdicionarCabecalho("Status", false);
                grid.AdicionarCabecalho("Número", "Numero", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Serie", "Serie", 5, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Data da Emissão", "Emissao", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("UF Carregamento", "UFCarga", 18, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("UF Descarregamento", "UFDesgarga", 18, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("MDF-e Manual", "MDFeManual", 8, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Status", "DescricaoStatus", 8, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Retorno Sefaz", "RetornoSefaz", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("DataPrevisaoEncerramento", false);

                string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;

                propOrdenacao = "MDFe." + propOrdenacao;

                List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> cargaMDFes = repCargaMDFe.ConsultarMDFe(codigoCarga, statusMDFe, propOrdenacao, grid.dirOrdena, grid.inicio, grid.limite);

                grid.setarQuantidadeTotal(repCargaMDFe.ContarConsultaMDFe(codigoCarga, statusMDFe));
                var lista = (from obj in cargaMDFes
                             select new
                             {
                                 obj.Codigo,
                                 CodigoMDFE = obj.MDFe.Codigo,
                                 CodigoEmpresa = obj.MDFe.Empresa.Codigo,
                                 obj.MDFe.Numero,
                                 Serie = obj.MDFe.Serie.Numero,
                                 Emissao = obj.MDFe.DataEmissao.HasValue ? obj.MDFe.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm:ss") : "",
                                 UFCarga = obj.MDFe.EstadoCarregamento.Nome,
                                 UFDesgarga = obj.MDFe.EstadoDescarregamento.Nome,
                                 DescricaoStatus = obj.MDFe.DescricaoStatus,
                                 Status = obj.MDFe.Status,
                                 MDFeManual = repCargaMDFeManual.ExistePorMDFe(obj.MDFe.Codigo) ? "Sim" : "Não",
                                 //RetornoSefaz = obj.MDFe.MensagemRetornoSefaz,
                                 RetornoSefaz = (obj.MDFe.MensagemStatus == null ? (obj.MDFe.MensagemRetornoSefaz != null ? WebUtility.HtmlEncode(obj.MDFe.MensagemRetornoSefaz) : string.Empty) : obj.MDFe.MensagemStatus.MensagemDoErro),
                                 DT_RowColor = obj.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado ? "#dff0d8" : obj.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Rejeicao ? "rgba(193, 101, 101, 1)" : obj.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Cancelado ? "#777" : "",
                                 DT_FontColor = (obj.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Rejeicao || obj.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Cancelado) ? "#FFFFFF" : "",
                                 DataPrevisaoEncerramento = obj.MDFe.DataPrevisaoEncerramento.HasValue ? obj.MDFe.DataPrevisaoEncerramento.Value.ToString("dd/MM/yyyy HH:mm:ss") : ""
                             }).ToList();
                grid.AdicionaRows(lista);
                return new JsonpResult(grid);
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

        public async Task<IActionResult> ConsultarCargaCTe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas repModalidadeFornecedorPessoas = new Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            try
            {
                bool permiteDownloadDocumentos = true;

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
                {
                    Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoas modalidadeFornecedor = Usuario.ClienteFornecedor != null ? repModalidadeFornecedorPessoas.BuscarPorCliente(Usuario.ClienteFornecedor.CPF_CNPJ) : null;
                    permiteDownloadDocumentos = modalidadeFornecedor?.PermiteDownloadDocumentos ?? true;
                }

                int codigoCarga = Request.GetIntParam("CodigoCarga");
                int numeroNF = Request.GetIntParam("NumeroNF");
                int numeroDocumento = Request.GetIntParam("NumeroDocumento");
                int cargaPedido = Request.GetIntParam("CargaPedido");
                List<int> cargaPedidos = Request.GetListParam<int>("CargaPedidos");

                double destinatario = Request.GetDoubleParam("Destinatario");
                bool ocultarStatusCTe = Request.GetBoolParam("OcultarStatusCTe");
                bool ctesSubContratacaoFilialEmissora = Request.GetBoolParam("CTesSubContratacaoFilialEmissora");
                bool ctesSemSubContratacaoFilialEmissora = Request.GetBoolParam("CTesSemSubContratacaoFilialEmissora");

                string statusCTe = Request.GetStringParam("Status");
                string proprietarioVeiculo = string.Empty;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("SituacaoCTe", false);
                grid.AdicionarCabecalho("CodigoCTE", false);
                grid.AdicionarCabecalho("NumeroModeloDocumentoFiscal", false);
                grid.AdicionarCabecalho("TipoDocumentoEmissao", false);
                grid.AdicionarCabecalho("CodigoEmpresa", false);
                grid.AdicionarCabecalho("Número", "Numero", 8, Models.Grid.Align.center, true);
                if (ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal)
                    grid.AdicionarCabecalho("Nº Controle", "NumeroControle", 8, Models.Grid.Align.center, true);
                else
                    grid.AdicionarCabecalho("NumeroControle", false);

                grid.AdicionarCabecalho("Serie", "Serie", 5, Models.Grid.Align.center, true);

                if (ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal)
                    grid.AdicionarCabecalho("Doc.", "AbreviacaoModeloDocumentoFiscal", 6, Models.Grid.Align.center, true);
                else
                    grid.AdicionarCabecalho("Doc.", "AbreviacaoModeloDocumentoFiscal", 10, Models.Grid.Align.center, true);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    if (ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal)
                        grid.AdicionarCabecalho("T. Pagamento", "DescricaoTipoPagamento", 9, Models.Grid.Align.center, true);
                    else
                        grid.AdicionarCabecalho("T. Pagamento", "DescricaoTipoPagamento", 10, Models.Grid.Align.center, true);
                    grid.AdicionarCabecalho("Remetente", "Remetente", 18, Models.Grid.Align.left, true);
                }
                else if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Terceiros)
                {
                    grid.AdicionarCabecalho("Emissão", "DataEmissao", 10, Models.Grid.Align.left, true);
                    grid.AdicionarCabecalho("Remetente", "Remetente", 18, Models.Grid.Align.left, true);
                }

                if (ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal)
                    grid.AdicionarCabecalho("T. Modal", "DescricaoTipoModal", 8, Models.Grid.Align.center, true);
                else
                    grid.AdicionarCabecalho("DescricaoTipoModal", false);

                if (codigoCarga > 0)
                    grid.AdicionarCabecalho("T. Serviço", "DescricaoTipoServico", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Destinatário", "Destinatario", 18, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Destino", "Destino", 10, Models.Grid.Align.left, true);

                if (permiteDownloadDocumentos)
                    grid.AdicionarCabecalho("Valor a Receber", "ValorFrete", 8, Models.Grid.Align.right, true, false, false, false, true);

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Terceiros && !ocultarStatusCTe)
                {
                    grid.AdicionarCabecalho("CST", "CST", 4, Models.Grid.Align.right, false);
                    grid.AdicionarCabecalho("Alíquota", "Aliquota", 5, Models.Grid.Align.right, false);
                    grid.AdicionarCabecalho("Status", "Status", 8, Models.Grid.Align.center, true);
                    grid.AdicionarCabecalho("Retorno Sefaz", "RetornoSefaz", 13, Models.Grid.Align.left, false);
                }

                if (ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal)
                    grid.AdicionarCabecalho("Nº SVM", "NumeroControleSVM", 6, Models.Grid.Align.center, true);
                else
                    grid.AdicionarCabecalho("NumeroControleSVM", false);

                grid.AdicionarCabecalho("ContemContainer", false);
                grid.AdicionarCabecalho("ContainerADefinir", false);
                grid.AdicionarCabecalho("Observacao", false);

                string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdenacao == "Remetente" || propOrdenacao == "Destinatario")
                    propOrdenacao += ".Nome";
                if (propOrdenacao == "Destino")
                    propOrdenacao = "LocalidadeTerminoPrestacao.Descricao";

                if (propOrdenacao == "DescricaoTipoPagamento")
                    propOrdenacao = "TipoPagamento";

                if (propOrdenacao == "DescricaoTipoServico")
                    propOrdenacao = "TipoServico";

                if (propOrdenacao == "AbreviacaoModeloDocumentoFiscal")
                    propOrdenacao = "ModeloDocumentoFiscal.Abreviacao";

                propOrdenacao = "CTe." + propOrdenacao;


                ;

                bool containerADefinir = repCargaPedido.ContainerADefinirCarga(codigoCarga);
                List<int> empresasFilialEmissora = new List<int>();

                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaMontarConsultaCtes filtro = new()
                {
                    Carga = codigoCarga,
                    NumeroDocumento = numeroDocumento,
                    NumeroNF = numeroNF,
                    StatusCTe = !string.IsNullOrWhiteSpace(statusCTe) ? new string[] { statusCTe } : null,
                    ApenasCTesNormais = true,
                    CtesSubContratacaoFilialEmissora = ctesSubContratacaoFilialEmissora,
                    CtesSemSubContratacaoFilialEmissora = ctesSemSubContratacaoFilialEmissora,
                    EmpresasFilialEmissora = empresasFilialEmissora,
                    ProprietarioVeiculo = proprietarioVeiculo,
                    Destinatario = destinatario,
                    BuscarPorCargaOrigem = false,
                    RetornarPreCtes = false,
                };

                int quantidade = await repCargaCTe.ContarConsultaCTes(filtro);

                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = await repCargaCTe.ConsultarCTes(filtro,
                                                                                                         propOrdenacao,
                                                                                                         grid.dirOrdena,
                                                                                                         grid.inicio,
                                                                                                         grid.limite);

                var lista = (from obj in cargaCTes
                             select new
                             {
                                 obj.Codigo,
                                 CodigoCTE = obj.CTe.Codigo,
                                 obj.CTe.DescricaoTipoServico,
                                 obj.CTe.DescricaoTipoModal,
                                 NumeroModeloDocumentoFiscal = obj.CTe.ModeloDocumentoFiscal.Numero,
                                 TipoDocumentoEmissao = obj.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao,
                                 AbreviacaoModeloDocumentoFiscal = obj.CTe.ModeloDocumentoFiscal.Abreviacao,
                                 CodigoEmpresa = obj.CTe.Empresa.Codigo,
                                 obj.CTe.Numero,
                                 obj.CTe.NumeroControle,
                                 obj.CTe.NumeroControleSVM,
                                 obj.CTe.DataEmissao,
                                 SituacaoCTe = obj.CTe.Status,
                                 Serie = obj.CTe.Serie.Numero,
                                 obj.CTe.DescricaoTipoPagamento,
                                 Remetente = obj.CTe.Remetente != null ? obj.CTe.Remetente.Nome + (!obj.CTe.Remetente.Exterior ? " (" + obj.CTe.Remetente.CPF_CNPJ_Formatado + ")" : string.Empty) : string.Empty,
                                 Destinatario = obj.CTe.Destinatario != null ? obj.CTe.Destinatario.Cliente?.Descricao ?? obj.CTe.Destinatario.Nome : string.Empty,
                                 Destino = obj.CTe.LocalidadeTerminoPrestacao.DescricaoCidadeEstado,
                                 ValorFrete = obj.CTe.ModeloDocumentoFiscal?.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe ? obj.CTe.ValorPrestacaoServico.ToString("n2") : obj.CTe.ValorAReceber.ToString("n2"),
                                 obj.CTe.CST,
                                 Aliquota = obj.CTe.AliquotaICMS > 0 ? obj.CTe.AliquotaICMS.ToString("n2") : obj.CTe.AliquotaISS.ToString("n4"),
                                 Status = obj.CTe.DescricaoStatus,
                                 RetornoSefaz = obj.CTe.MensagemStatus != null ? obj.CTe.MensagemStatus.MensagemDoErro : !string.IsNullOrWhiteSpace(obj.CTe.MensagemRetornoSefaz) ? obj.CTe.MensagemRetornoSefaz != " - " ? System.Web.HttpUtility.HtmlEncode(obj.CTe.MensagemRetornoSefaz) : "" : "",
                                 ContemContainer = (obj.CTe.Containers != null && obj.CTe.Containers.Count > 0),
                                 ContainerADefinir = containerADefinir,
                                 Observacao = obj.CTe.ObservacoesGerais,
                                 DT_RowColor = !ocultarStatusCTe ? (string.IsNullOrWhiteSpace(statusCTe) ? (obj.CTe.PossuiCTeComplementar ? "#ddd8f0" : obj.CTe.PossuiAnulacaoSubstituicao ? "#f0e9d8" : obj.CTe.PossuiCartaCorrecao ? "#d8e4f0" : obj.CTe.Status == "A" ? "#dff0d8" : obj.CTe.Status == "R" ? "rgba(193, 101, 101, 1)" : (obj.CTe.Status == "C" || obj.CTe.Status == "I" || obj.CTe.Status == "D") ? "#777" : "") : "") : "",
                                 DT_FontColor = !ocultarStatusCTe ? (string.IsNullOrWhiteSpace(statusCTe) ? ((obj.CTe.Status == "R" || obj.CTe.Status == "C" || obj.CTe.Status == "I" || obj.CTe.Status == "D") ? "#FFFFFF" : "") : "") : "",
                             }).ToList();

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(quantidade);

                return new JsonpResult(grid);
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

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadLotePDF()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(unidadeTrabalho);

                string caminhoRelatorios = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoRelatorios;

                if (string.IsNullOrWhiteSpace(caminhoRelatorios))
                    return new JsonpResult(true, false, "O caminho para o download das DACTEs não está disponível. Contate o suporte técnico.");

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                if (carga == null)
                    return new JsonpResult(false, true, "Carga não encontrada.");

                List<int> ctes = repCargaCTe.BuscarCodigosCTesAutorizadosPorCarga(codigoCarga, false, false);

                List<int> mdfes = repCargaMDFe.BuscarCodigosMDFePorAutorizadosCarga(codigoCarga);

                List<int> valePedagios = repCargaValePedagio.BuscarCodigosValePedagiosSemPararPorCarga(codigoCarga);

                if (ctes.Count > 2000)
                    return new JsonpResult(false, true, "Não é permitido o download de mais de 2000 arquivos.");

                Servicos.CTe svcCTe = new Servicos.CTe(unidadeTrabalho);

                System.IO.MemoryStream arquivo = svcCTe.ObterLotePDFs(codigoCarga, ctes, mdfes, valePedagios, unidadeTrabalho, TipoServicoMultisoftware);

                return Arquivo(arquivo, "application/octet-stream", string.Concat(Utilidades.File.GetValidFilename(carga.CodigoCargaEmbarcador), "_Documentos.zip"));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do lote de PDF.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> AtualizarDadosTransporte()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Usuario repositorioMotorista = new Repositorio.Usuario(unitOfWork);
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta servicoCargaJanelaCarregamentoConsulta = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta(unitOfWork);

                int codigoCarga = Request.GetIntParam("Codigo");
                int codigoMotorista = Request.GetIntParam("Motorista");
                int codigoVeiculo = Request.GetIntParam("Veiculo");

                Dominio.Entidades.Usuario motorista = repositorioMotorista.BuscarPorCodigo(codigoMotorista);
                Dominio.Entidades.Veiculo veiculo = repositorioVeiculo.BuscarPorCodigo(codigoVeiculo);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga, true);

                if (carga == null)
                    throw new Exception("Não foi possível encontrar a carga");

                if (motorista != null)
                    carga.Motoristas.Add(motorista);

                if (veiculo != null)
                    carga.Veiculo = veiculo;

                repositorioCarga.Atualizar(carga, Auditado);

                SalvarDadosTransporte(carga, unitOfWork);

                unitOfWork.CommitChanges();

                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = servicoCargaJanelaCarregamentoConsulta.ObterCargaJanelaCarregamentoPorCarga(carga.Codigo);

                return new JsonpResult(new
                {
                    AceiteTransporte = new
                    {
                        Situacao = cargaJanelaCarregamento?.Situacao.ObterDescricao() ?? "",
                        Veiculo = carga.Veiculo?.Placa ?? "",
                        Modelo = carga.Veiculo?.Modelo?.Descricao ?? "",
                        Motorista = carga.Motoristas?.Count > 0 ? carga.Motoristas?.Last().Nome : "",
                        Reboque = carga.Veiculo?.VeiculosVinculados?.FirstOrDefault()?.Placa ?? ""
                    },
                    AvancarEtapa = (((carga.TipoOperacao?.SolicitarNotasFiscaisAoSalvarDadosTransportador ?? false) || ConfiguracaoEmbarcador.SolicitarNotasFiscaisAoSalvarDadosTransportador) && carga.SituacaoCarga == SituacaoCarga.AgTransportador && !carga.ExigeNotaFiscalParaCalcularFrete)
                });
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AdicionarDadosAgendamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Usuario repositorioMotorista = new Repositorio.Usuario(unitOfWork);
                Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);

                int codigoCarga = Request.GetIntParam("Codigo");
                int codigoMotorista = Request.GetIntParam("Motorista");
                int codigoVeiculo = Request.GetIntParam("Veiculo");
                int codigoTransportador = Request.GetIntParam("Transportador");

                Dominio.Entidades.Usuario motorista = codigoMotorista > 0 ? repositorioMotorista.BuscarPorCodigo(codigoMotorista) : null;
                Dominio.Entidades.Veiculo veiculo = codigoVeiculo > 0 ? repositorioVeiculo.BuscarPorCodigo(codigoVeiculo) : null;
                Dominio.Entidades.Empresa transportador = codigoTransportador > 0 ? repositorioEmpresa.BuscarPorCodigo(codigoTransportador) : null;
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga, true);

                if (carga == null)
                    throw new Exception("Não foi possível encontrar a carga");

                if (motorista != null)
                    carga.Motoristas.Add(motorista);

                if (veiculo != null)
                    carga.Veiculo = veiculo;

                if (transportador != null)
                    carga.Empresa = transportador;

                repositorioCarga.Atualizar(carga, Auditado);

                SalvarDadosTransporte(carga, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AtualizarNFe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Pedidos.XMLNotaFiscal repositorioXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = repositorioXmlNotaFiscal.BuscarPorCodigo(codigo);

                if (xmlNotaFiscal == null)
                    return new JsonpResult(false, "A NF-e não foi encontrada.");

                xmlNotaFiscal.ChaveVenda = Request.GetStringParam("ChaveVenda");
                xmlNotaFiscal.Peso = Request.GetDecimalParam("Peso");
                xmlNotaFiscal.Volumes = Request.GetIntParam("Volumes");
                xmlNotaFiscal.QuantidadePallets = Request.GetDecimalParam("Pallets");
                xmlNotaFiscal.MetrosCubicos = Request.GetDecimalParam("MetrosCubicos");

                repositorioXmlNotaFiscal.Atualizar(xmlNotaFiscal);

                unitOfWork.CommitChanges();

                return new JsonpResult(true, "NF-e atualizada com sucesso!");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar os dados da NF-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> EnviarNFe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<RetornoArquivo> retornoArquivos = new List<RetornoArquivo>();
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Logistica.AgendamentoColeta repositorioAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta = repositorioAgendamentoColeta.BuscarPorCodigo(codigo, false);

                if (agendamentoColeta == null)
                    return new JsonpResult(false, true, "Agendamento não encontrado.");

                if (agendamentoColeta.Situacao == SituacaoAgendamentoColeta.Cancelado)
                    return new JsonpResult(false, true, "Não é possível modificar as notas de um agendamento cancelado");

                if (agendamentoColeta.EtapaAgendamentoColeta != EtapaAgendamentoColeta.NFe)
                    return new JsonpResult(false, true, "Não é possível modificar as notas na atual etapa do agendamento");

                List<Servicos.DTO.CustomFile> files = HttpContext.GetFiles();

                if (files.Count == 0)
                    return new JsonpResult(false, true, "Não foi enviado o arquivo.");

                unitOfWork.Start();

                string mensagemRetorno = string.Empty;

                for (int i = 0; i < files.Count; i++)
                {
                    Servicos.DTO.CustomFile file = files[i];
                    string extensao = System.IO.Path.GetExtension(file.FileName).ToLower();
                    string nomeArquivo = System.IO.Path.GetFileName(file.FileName);

                    RetornoArquivo retornoArquivo = new RetornoArquivo
                    {
                        nome = file.FileName,
                        processada = true,
                        mensagem = "",
                        codigo = 0
                    };

                    if (extensao != ".xml")
                    {
                        retornoArquivo.processada = false;
                        retornoArquivo.mensagem = "A extensão do arquivo é inválida.";
                        mensagemRetorno = RetornarMensagem(mensagemRetorno, "Há arquivos com extenção inválida.");
                        retornoArquivos.Add(retornoArquivo);
                        continue;
                    }

                    try
                    {
                        var objetoNFe = MultiSoftware.NFe.Servicos.Leitura.Ler(file.InputStream);
                        if (objetoNFe == null)
                        {
                            retornoArquivo.processada = false;
                            retornoArquivo.mensagem = "O xml informado não é uma NF-e, por favor verifique.";
                            mensagemRetorno = RetornarMensagem(mensagemRetorno, "Há arquivos que não são nota fiscal.");
                            retornoArquivos.Add(retornoArquivo);
                            continue;
                        }

                        string retorno = ProcessarXMLNFe(file.InputStream, agendamentoColeta, unitOfWork, out bool msgAlertaObservacao);

                        if (!string.IsNullOrEmpty(retorno) && !msgAlertaObservacao)
                        {
                            retornoArquivo.processada = false;
                            retornoArquivo.mensagem = retorno;
                        }
                        else if (msgAlertaObservacao)
                        {
                            retornoArquivo.processada = true;
                            retornoArquivo.mensagem = retorno;
                        }
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro(ex);
                        retornoArquivo.processada = false;
                        retornoArquivo.mensagem = "Ocorreu uma falha ao enviar o xml, verifique se o arquivo é um documento fiscal válido. " + ex.Message;
                        mensagemRetorno = RetornarMensagem(mensagemRetorno, "Ocorreu falhas ao enviar os xmls, verifique se os arquivos são documentos fiscais válidos.");
                    }
                    finally
                    {
                        file.InputStream.Dispose();
                    }

                    Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = null;

                    if (agendamentoColeta.Pedido != null)
                        pedido = agendamentoColeta.Pedido;
                    else if (agendamentoColeta.Pedido == null && agendamentoColeta.Carga != null && agendamentoColeta.Carga.Pedidos?.Count > 0)
                        pedido = agendamentoColeta.Carga.Pedidos.Select(o => o.Pedido).FirstOrDefault();
                    else
                        mensagemRetorno = RetornarMensagem(mensagemRetorno, "Ocorreu falha ao vincular NF-e ao pedido. Pedido não econtrado.");//return new JsonpResult(false, "Ocorreu uma falha ao vincular NF-e ao pedido. Pedido não econtrado.");

                    repPedido.Atualizar(pedido);
                    retornoArquivos.Add(retornoArquivo);
                }

                unitOfWork.CommitChanges();
                return new JsonpResult(new
                {
                    Arquivos = retornoArquivos,
                    MensagemRetorno = mensagemRetorno
                });
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao enviar o NF-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirNFe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoAgendamento = Request.GetIntParam("Agendamento");
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Logistica.AgendamentoColeta repositorioAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(unitOfWork);
                Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal repCarregamentoPedidoNotaFiscal = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
                Repositorio.Embarcador.Integracao.IntegracaoAVIPED repIntegracaoAVIPED = new Repositorio.Embarcador.Integracao.IntegracaoAVIPED(unitOfWork);

                Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta = repositorioAgendamentoColeta.BuscarPorCodigo(codigoAgendamento, false);

                if (agendamentoColeta == null)
                    return new JsonpResult(false, true, "Agendamento não encontrado.");

                if (agendamentoColeta.Situacao == SituacaoAgendamentoColeta.Cancelado)
                    return new JsonpResult(false, true, "Não é possível modificar as notas de um agendamento cancelado");

                if (agendamentoColeta.EtapaAgendamentoColeta != EtapaAgendamentoColeta.NFe)
                    return new JsonpResult(false, true, "Não é possível modificar as notas na atual esapa do agendamento");

                unitOfWork.Start();

                if (agendamentoColeta.Pedido != null)
                {
                    foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xml in agendamentoColeta.Pedido.NotasFiscais)
                    {
                        if (xml.Codigo == codigo)
                        {
                            agendamentoColeta.Pedido.NotasFiscais.Remove(xml);
                            xml.nfAtiva = false;
                            repXMLNotaFiscal.Atualizar(xml);
                            break;
                        }
                    }

                    repPedido.Atualizar(agendamentoColeta.Pedido);
                }
                else if (!ConfiguracaoEmbarcador.ControlarAgendamentoSKU && agendamentoColeta.Carga?.Carregamento != null)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal> listaNotasEnviadas = repCarregamentoPedidoNotaFiscal.BuscarPorCarregamento(agendamentoColeta.Carga.Carregamento.Codigo);

                    foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal notaFiscal in listaNotasEnviadas)
                    {
                        if (!notaFiscal.NotasFiscais.Any(nf => nf.Codigo == codigo))
                            continue;

                        foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xml in notaFiscal.NotasFiscais)
                        {
                            if (xml.Codigo == codigo)
                            {
                                notaFiscal.NotasFiscais.Remove(xml);
                                xml.nfAtiva = false;
                                repXMLNotaFiscal.Atualizar(xml);
                                break;
                            }
                        }

                        repCarregamentoPedidoNotaFiscal.Atualizar(notaFiscal);
                        break;
                    }
                }
                else if (agendamentoColeta.Carga != null)
                {
                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotasFiscais = repositorioPedidoXMLNotaFiscal.BuscarPorCarga(agendamentoColeta.Carga.Codigo);

                    foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal notaFiscal in pedidoXMLNotasFiscais)
                    {
                        if (notaFiscal.XMLNotaFiscal.Codigo == codigo)
                        {
                            notaFiscal.XMLNotaFiscal.nfAtiva = false;
                            repXMLNotaFiscal.Atualizar(notaFiscal.XMLNotaFiscal);
                            repIntegracaoAVIPED.DeletarPorPedidoXMLNotaFiscal(notaFiscal.Codigo);
                            repositorioPedidoXMLNotaFiscal.Deletar(notaFiscal);
                            break;
                        }
                    }
                }

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao enviar o CT-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AdicionarCTe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<Servicos.DTO.CustomFile> files = HttpContext.GetFiles();

                if (files.Count <= 0)
                    throw new ControllerException("Nenhum arquivo enviado.");

                Servicos.DTO.CustomFile file = files[0];
                string extensao = System.IO.Path.GetExtension(file.FileName).ToLower();
                string nomeArquivo = System.IO.Path.GetFileName(file.FileName);

                if (!extensao.Equals(".xml"))
                    throw new ControllerException("A extensão do arquivo é inválida.");

                unitOfWork.Start();
                Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);
                Servicos.Embarcador.CTe.CTe servicoCTe = new Servicos.Embarcador.CTe.CTe(unitOfWork);

                int codigoAgendamento = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Logistica.AgendamentoColeta repositorioAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta = repositorioAgendamentoColeta.BuscarPorCodigo(codigoAgendamento);

                var cteLido = MultiSoftware.CTe.Servicos.Leitura.Ler(file.InputStream);

                if (cteLido == null)
                    throw new ControllerException("Não foi possível ler o arquivo.");

                Type tipoCTe = cteLido.GetType();

                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidosXmlNotaFiscal = repositorioPedidoXmlNotaFiscal.BuscarPorCarga(agendamentoColeta.Carga.Codigo);

                dynamic cteProc = null;

                if (tipoCTe == typeof(MultiSoftware.CTe.v200.ConhecimentoDeTransporteProcessado.cteProc))
                    cteProc = (MultiSoftware.CTe.v200.ConhecimentoDeTransporteProcessado.cteProc)cteLido;
                else if (tipoCTe == typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc))
                    cteProc = (MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc)cteLido;
                else
                    cteProc = (MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteProc)cteLido;

                Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte = servicoCTe.ConverterProcCTeParaCTe(cteProc);

                List<string> chavesNotasFiscais = new List<string>();

                if (cte.NFEs?.Count > 0)
                    chavesNotasFiscais = cte.NFEs?.Select(obj => obj.Chave).ToList();

                if (!pedidosXmlNotaFiscal.Any(pxml => chavesNotasFiscais.Contains(pxml.XMLNotaFiscal.Chave)))
                    throw new ControllerException("O CT-e enviado não faz referência a nenhum pedido da carga.");

                Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe repositorioCargaPedidoDocumentoCTe = new Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe(unitOfWork);
                Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repositorioCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.Localidade repositorioLocalidade = new Repositorio.Localidade(unitOfWork);

                Dominio.Entidades.Empresa empresa = repositorioEmpresa.BuscarPorCNPJ(cte.Emitente.CNPJ);

                if (empresa == null)
                {
                    empresa = new Dominio.Entidades.Empresa()
                    {
                        CNPJ = cte.Emitente.CNPJ,
                        Localidade = repositorioLocalidade.BuscarPorDescricao(cte.LocalidadeInicioPrestacao?.Descricao).FirstOrDefault(),
                        InscricaoEstadual = "",
                        EmissaoDocumentosForaDoSistema = true,
                        EmpresaPai = repositorioEmpresa.BuscarEmpresaPai()
                    };

                    repositorioEmpresa.Inserir(empresa);

                    svcCTe.ObterSerie(empresa, cte.Serie.ToInt(), Dominio.Enumeradores.TipoSerie.CTe, unitOfWork);
                }

                if (!empresa.EmissaoDocumentosForaDoSistema)
                    throw new ControllerException("A empresa não está configurada para emitir fora do sistema.");

                bool processou = Servicos.Embarcador.CTe.CTe.ProcessarXMLCTe(file.InputStream, unitOfWork, _conexao.StringConexao, TipoServicoMultisoftware, nomeArquivo, false, false, Auditado);
                if (!unitOfWork.IsActiveTransaction())
                    unitOfWork.Start();

                if (!processou)
                    throw new ControllerException("Não foi possível importar o XML no CTe.");

                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidosXMLNotaFiscalDoCTe = (from pxmlNf in pedidosXmlNotaFiscal where chavesNotasFiscais.Contains(pxmlNf.XMLNotaFiscal.Chave) select pxmlNf).ToList();
                Dominio.Entidades.ConhecimentoDeTransporteEletronico entidadeCTe = repositorioCTe.BuscarPorChave(cte.Chave);

                int codigoCTe = entidadeCTe.Codigo;

                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXmlNotaFiscal in pedidosXMLNotaFiscalDoCTe)
                {
                    if (!Servicos.Embarcador.CTe.CTEsImportados.VincularCTeACargaPedido(out string erro, codigoCTe, pedidoXmlNotaFiscal.CargaPedido.Codigo, false, unitOfWork, Auditado, ConfiguracaoEmbarcador))
                        throw new ControllerException($"Não foi possível vincular o CT-e à carga. {erro}");
                }

                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = new Dominio.Entidades.Embarcador.Cargas.CargaCTe()
                {
                    CargaOrigem = agendamentoColeta.Carga,
                    Carga = agendamentoColeta.Carga,
                    CTe = entidadeCTe
                };

                repositorioCargaCTe.Inserir(cargaCTe);

                Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repositorioCargaPedidoXmlNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);

                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXmlNotaFiscalDoCTe in pedidosXMLNotaFiscalDoCTe)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe cargaPedidoXmlNotaFiscalCTe = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe()
                    {
                        CargaCTe = cargaCTe,
                        PedidoXMLNotaFiscal = pedidoXmlNotaFiscalDoCTe
                    };

                    repositorioCargaPedidoXmlNotaFiscalCTe.Inserir(cargaPedidoXmlNotaFiscalCTe);
                }

                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedidos = repositorioCargaPedido.BuscarPorCarga(agendamentoColeta.Carga.Codigo);
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargasCTe = repositorioCargaCTe.BuscarPorCarga(agendamentoColeta.Carga.Codigo, buscarPorCargaOrigem: true);


                bool avancarCarga = false;

                if (agendamentoColeta.Carga.ExigeNotaFiscalParaCalcularFrete)
                    avancarCarga = agendamentoColeta.Carga.SituacaoCarga == SituacaoCarga.PendeciaDocumentos;
                else
                    avancarCarga = (agendamentoColeta.Carga.SituacaoCarga == SituacaoCarga.AgTransportador || agendamentoColeta.Carga.SituacaoCarga == SituacaoCarga.Nova);

                Repositorio.Embarcador.Pedidos.XMLNotaFiscal repositorioXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe> notasFiscaisCTes = cargasCTe.Where(obj => obj.NotasFiscais != null).SelectMany(obj => obj.NotasFiscais).ToList();

                if (!notasFiscaisCTes.SelectMany(obj => obj.CargaCTe.CTe.Chave).All(obj => pedidosXmlNotaFiscal.SelectMany(x => x.XMLNotaFiscal.Chave).Contains(obj)))
                    avancarCarga = false;

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> cargaPedidosComponentesFreteCarga = new Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete(unitOfWork).BuscarPorCarga(agendamentoColeta.Carga.Codigo, false);

                Servicos.Embarcador.Carga.RateioFrete servicoRateioFrete = new Servicos.Embarcador.Carga.RateioFrete();
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                agendamentoColeta.Carga.ValorFrete = cargasCTe.Sum(obj => obj.CTe?.ValorAReceber ?? 0);
                agendamentoColeta.Carga.ValorFreteAPagar = cargasCTe.Sum(obj => obj.CTe?.ValorAReceber ?? 0);
                agendamentoColeta.Carga.TipoFreteEscolhido = TipoFreteEscolhido.Embarcador;
                servicoRateioFrete.RatearValorDoFrenteEntrePedidos(agendamentoColeta.Carga, cargasPedidos, ConfiguracaoEmbarcador, false, unitOfWork, TipoServicoMultisoftware);

                if (avancarCarga)
                {
                    agendamentoColeta.Situacao = SituacaoAgendamentoColeta.Finalizado;
                    agendamentoColeta.Carga.PossuiPendencia = false;
                    agendamentoColeta.Carga.AutorizouTodosCTes = true;
                    agendamentoColeta.Carga.EmitindoCTes = false;
                    agendamentoColeta.Carga.Empresa = empresa;

                    repositorioAgendamentoColeta.Atualizar(agendamentoColeta);
                }

                repositorioCarga.Atualizar(agendamentoColeta.Carga);

                unitOfWork.CommitChanges();
                cte.Codigo = (from cargacte in cargasCTe where cargacte.CTe?.Numero == cte.Numero select cargacte.CTe?.Codigo ?? 0).FirstOrDefault();

                List<RetornoArquivo> arquivos = new List<RetornoArquivo>();

                arquivos.Add(new RetornoArquivo()
                {
                    codigo = cte.Codigo,
                    mensagem = "Arquivo processado com sucesso",
                    nome = file.FileName,
                    processada = true
                });

                return new JsonpResult(new
                {
                    Arquivos = arquivos
                });
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao enviar o XML do CT-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Logistica.AgendamentoColeta repositorioAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(unitOfWork);
                Repositorio.Embarcador.Logistica.AgendamentoColetaPedido repositorioAgendamentoColetaPedido = new Repositorio.Embarcador.Logistica.AgendamentoColetaPedido(unitOfWork);
                Repositorio.Embarcador.Logistica.AgendamentoColetaPedidoProduto repositorioAgendamentoColetaPedidoProduto = new Repositorio.Embarcador.Logistica.AgendamentoColetaPedidoProduto(unitOfWork);
                Repositorio.Embarcador.Logistica.AgendamentoColetaAnexo repositorioAgendamentoColetaAnexo = new Repositorio.Embarcador.Logistica.AgendamentoColetaAnexo(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repositorioCargaJanelaDescarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento(unitOfWork);
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta servicoCargaJanelaCarregamentoConsulta = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta(unitOfWork);
                Servicos.Embarcador.Logistica.AgendamentoColeta servicoAgendamentoColeta = new Servicos.Embarcador.Logistica.AgendamentoColeta(unitOfWork, TipoServicoMultisoftware);
                Repositorio.Embarcador.Logistica.DocumentoTransporte repDocumentoTrasnporte = new Repositorio.Embarcador.Logistica.DocumentoTransporte(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoProduto repositorioPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(unitOfWork);

                Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta = repositorioAgendamentoColeta.BuscarPorCodigo(codigo);

                if (agendamentoColeta == null)
                    return new JsonpResult(false, "Agendamento não encontrado.");

                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = servicoCargaJanelaCarregamentoConsulta.ObterCargaJanelaCarregamentoPorCarga(agendamentoColeta?.Carga?.Codigo ?? 0);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento = repositorioCargaJanelaDescarregamento.BuscarPorCarga(agendamentoColeta?.Carga?.Codigo ?? 0);

                List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido> listaPedidos = repositorioAgendamentoColetaPedido.BuscarPorAgendamentoColeta(agendamentoColeta.Codigo);
                List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedidoProduto> listaAgendamentoColetaPedidoProdutos = repositorioAgendamentoColetaPedidoProduto.BuscarPorCodigoAgendamentoColeta(agendamentoColeta.Codigo);
                List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaAnexo> listaAnexos = repositorioAgendamentoColetaAnexo.BuscarPorAgendamentoColeta(agendamentoColeta.Codigo);
                List<Dominio.Entidades.Embarcador.Logistica.DocumentoTransporte> listaDocumentosParaTransporte = repDocumentoTrasnporte.BuscarPorCodigoAgendamento(agendamentoColeta.Codigo);
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> pedidosProdutos = repositorioPedidoProduto.BuscarPorPedidos(listaPedidos.Select(o => o.Pedido.Codigo).ToList());

                return new JsonpResult(new
                {
                    Carga = new
                    {
                        PermiteInformarDadosTransporteEtapaCarga = (agendamentoColeta.Carga?.ExigeNotaFiscalParaCalcularFrete ?? false) ? agendamentoColeta.Carga?.SituacaoCarga == SituacaoCarga.Nova || agendamentoColeta.Carga?.SituacaoCarga == SituacaoCarga.AgNFe : agendamentoColeta.Carga?.SituacaoCarga == SituacaoCarga.Nova || agendamentoColeta.Carga?.SituacaoCarga == SituacaoCarga.CalculoFrete || agendamentoColeta.Carga?.SituacaoCarga == SituacaoCarga.AgNFe,
                        DataColeta = agendamentoColeta.DataColeta?.ToString(ConfiguracaoEmbarcador.ControlarAgendamentoSKU ? "dd/MM/yyyy" : "dd/MM/yyyy HH:mm"),
                        DataEntrega = agendamentoColeta.DataEntrega?.ToString(ConfiguracaoEmbarcador.ControlarAgendamentoSKU ? "dd/MM/yyyy" : "dd/MM/yyyy HH:mm"),
                        DataAgendamento = agendamentoColeta.DataAgendamento?.ToString("dd/MM/yyyy HH:mm"),
                        DataCriacao = agendamentoColeta.DataCriacao.HasValue ? agendamentoColeta.DataCriacao.Value.ToString(ConfiguracaoEmbarcador.ControlarAgendamentoSKU ? "dd/MM/yyyy" : "dd/MM/yyyy HH:mm") : agendamentoColeta.DataAgendamento?.ToString(ConfiguracaoEmbarcador.ControlarAgendamentoSKU ? "dd/MM/yyyy" : "dd/MM/yyyy HH:mm"),
                        NumeroCarga = agendamentoColeta.Carga?.CodigoCargaEmbarcador ?? string.Empty,
                        DataJanela = cargaJanelaDescarregamento != null ? cargaJanelaDescarregamento?.InicioDescarregamento.ToString("dd/MM/yyyy HH:mm") : "",
                        agendamentoColeta.Observacao,
                        HorarioInicioFaixa = agendamentoColeta.HoraInicioFaixa.HasValue ? agendamentoColeta.HoraInicioFaixa.Value.ToString(@"hh\:mm") : "",
                        HorarioLimiteFaixa = agendamentoColeta.HoraLimiteFaixa.HasValue ? agendamentoColeta.HoraLimiteFaixa.Value.ToString(@"hh\:mm") : "",
                        ObrigatorioInformarCTes = IsObrigatorioInformarCTes(agendamentoColeta, unitOfWork),
                        PedidoEmbarcador = agendamentoColeta.Carga != null ? string.Join(", ", (from o in agendamentoColeta.Carga.Pedidos select o.Pedido.NumeroPedidoEmbarcador)) :
                                   agendamentoColeta.Pedido != null ? agendamentoColeta.Pedido.NumeroPedidoEmbarcador : string.Empty,
                        TipoOperacao = new
                        {
                            Codigo = agendamentoColeta.TipoOperacao?.Codigo ?? 0,
                            Descricao = agendamentoColeta.TipoOperacao?.Descricao ?? ""
                        },
                        PortoOrigem = new
                        {
                            Codigo = agendamentoColeta.PortoOrigem?.Codigo ?? 0,
                            Descricao = agendamentoColeta.PortoOrigem?.Descricao ?? ""
                        },
                        PortoDestino = new
                        {
                            Codigo = agendamentoColeta.PortoDestino?.Codigo ?? 0,
                            Descricao = agendamentoColeta.PortoDestino?.Descricao ?? ""
                        },
                        agendamentoColeta.TransportadorManual,
                        agendamentoColeta.Reboque,
                        agendamentoColeta.Placa,
                        agendamentoColeta.Motorista,
                        TipoCarga = new
                        {
                            Codigo = agendamentoColeta.TipoCarga?.Codigo ?? 0,
                            Descricao = agendamentoColeta.TipoCarga?.Descricao ?? ""
                        },
                        ModeloVeicular = agendamentoColeta.ModeloVeicular == null ? null : new
                        {
                            agendamentoColeta.ModeloVeicular.Codigo,
                            agendamentoColeta.ModeloVeicular.Descricao
                        },
                        Filial = agendamentoColeta.Filial == null ? null : new
                        {
                            agendamentoColeta.Filial.Codigo,
                            agendamentoColeta.Filial.Descricao
                        },
                        CDDestino = new
                        {
                            Codigo = agendamentoColeta.CDDestino?.Codigo ?? 0,
                            Descricao = agendamentoColeta.CDDestino?.Descricao ?? ""
                        },
                        agendamentoColeta.EmailSolicitante,
                        agendamentoColeta.Peso,
                        agendamentoColeta.Volumes,
                        agendamentoColeta.UnidadeMedida,
                        Remetente = agendamentoColeta.Remetente == null ? null : new
                        {
                            agendamentoColeta.Remetente.CPF_CNPJ,
                            agendamentoColeta.Remetente.Descricao
                        },
                        Destinatario = new
                        {
                            agendamentoColeta.Destinatario.CPF_CNPJ,
                            agendamentoColeta.Destinatario.Descricao
                        },
                        Recebedor = agendamentoColeta.Recebedor == null ? null : new
                        {
                            agendamentoColeta.Recebedor.CPF_CNPJ,
                            agendamentoColeta.Recebedor.Descricao
                        },
                        Transportador = new
                        {
                            Codigo = agendamentoColeta.Transportador?.Codigo ?? 0,
                            Descricao = agendamentoColeta.Transportador?.Descricao ?? ""
                        },
                        Categoria = new
                        {
                            Codigo = agendamentoColeta.Categoria?.Codigo ?? 0,
                            Descricao = agendamentoColeta.Categoria?.Descricao ?? ""
                        },
                        agendamentoColeta.CargaPerigosa,
                        agendamentoColeta.EtapaAgendamentoColeta,
                        InformarDadosNotaCte = agendamentoColeta.TipoOperacao?.InformarDadosNotaCte ?? false,
                        ExigirQueCDDestinoSejaInformadoAgendamento = agendamentoColeta.TipoOperacao?.ConfiguracaoAgendamentoColetaEntrega?.ExigirQueCDDestinoSejaInformadoAgendamento ?? false,
                    },
                    DadosTransporte = new
                    {
                        Codigo = agendamentoColeta.Carga?.Codigo ?? 0
                    },
                    RetornoAgendamento = new
                    {
                        SenhaAgendamento = agendamentoColeta.Senha,
                        SituacaoCodigo = agendamentoColeta.Situacao,
                        Situacao = agendamentoColeta.Situacao?.ObterDescricao() ?? string.Empty,
                        DataSolicitada = agendamentoColeta.DataAgendamento?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                        DataProgramada = cargaJanelaDescarregamento?.InicioDescarregamento.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                        DataCancelamento = agendamentoColeta.DataCancelamento?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                        VolumesAgendados = agendamentoColeta.Volumes,
                        agendamentoColeta.Observacao,
                        NumeroCarga = agendamentoColeta.Carga?.Numero,
                        OperadorAgendamento = agendamentoColeta.Solicitante?.Nome ?? agendamentoColeta.Carga?.Operador?.Nome ?? string.Empty
                        ,
                    },
                    DadosAgendamento = new
                    {
                        Codigo = agendamentoColeta.Carga?.Codigo ?? 0,
                        Transportador = new
                        {
                            agendamentoColeta.Carga?.Empresa?.Codigo,
                            agendamentoColeta.Carga?.Empresa?.Descricao
                        },
                        Veiculo = new
                        {
                            agendamentoColeta.Carga?.Veiculo?.Codigo,
                            agendamentoColeta.Carga?.Veiculo?.Descricao
                        },
                        Motorista = new
                        {
                            agendamentoColeta.Carga?.Motoristas.FirstOrDefault()?.Codigo,
                            agendamentoColeta.Carga?.Motoristas.FirstOrDefault()?.Descricao
                        }
                    },
                    ControleAgendamento = new
                    {
                        CodigoCarga = agendamentoColeta.Carga?.Codigo ?? 0,
                        CodigoAgendamento = agendamentoColeta.Codigo,
                        agendamentoColeta.Codigo,
                        Etapa = agendamentoColeta.EtapaAgendamentoColeta,
                        agendamentoColeta.ApenasGerarPedido,
                        ForcarEtapaNFe = servicoAgendamentoColeta.IsForcarEtapaNFe(agendamentoColeta),
                        RemoverEtapaAgendamentoDoAgendamentoColeta = agendamentoColeta.TipoOperacao?.ConfiguracaoAgendamentoColetaEntrega?.RemoverEtapaAgendamentoDoAgendamentoColeta ?? false,
                        AgendamentoPedidosExistentes = agendamentoColeta.Remetente.Modalidades.Any(obj => obj.TipoModalidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalidade.Fornecedor && obj.ModalidadesFornecedores.Any(o => o.GerarAgendamentoSomentePedidosExistentes)),
                    },
                    AceiteTransporte = new
                    {
                        Situacao = cargaJanelaCarregamento?.Situacao.ObterDescricao() ?? "",
                        Veiculo = agendamentoColeta.Carga?.Veiculo?.Placa ?? "",
                        Modelo = agendamentoColeta.Carga?.ModeloVeicularCarga?.Descricao ?? "",
                        Motorista = agendamentoColeta.Carga?.Motoristas?.Count > 0 ? agendamentoColeta.Carga?.Motoristas?.First().Nome : "",
                        Reboque = ObterDescricaoReboqueCarga(agendamentoColeta.Carga)
                    },
                    ListaPedidos = (
                        from agendamentoColetaPedido in listaPedidos
                        select new
                        {
                            agendamentoColetaPedido.Codigo,
                            agendamentoColetaPedido.Pedido.NumeroPedidoEmbarcador,
                            TipoOperacao = agendamentoColetaPedido.Pedido.TipoOperacao?.Descricao ?? "",
                            DescricaoFilial = cargaJanelaDescarregamento?.CentroDescarregamento?.Descricao ?? "",
                            CodigoDestinatario = agendamentoColetaPedido.Pedido.Destinatario?.Codigo ?? 0,
                            TipoCarga = agendamentoColetaPedido.Pedido.TipoDeCarga?.Descricao ?? "",
                            TipoCargaCodigo = agendamentoColetaPedido.Pedido.TipoDeCarga?.Codigo ?? 0,
                            QtVolumes = ObterQuantidadeCaixasAgendamentoPedido(agendamentoColetaPedido, listaAgendamentoColetaPedidoProdutos),
                            SKU = ObterQuantidadeItensAgendamentoPedido(agendamentoColetaPedido, listaAgendamentoColetaPedidoProdutos, unitOfWork.StringConexao),
                            QtProdutos = ObterQuantidadeProdutosAgendamentoPedido(agendamentoColetaPedido, listaAgendamentoColetaPedidoProdutos),
                            Saldo = agendamentoColetaPedido.Pedido.SaldoVolumesRestante,
                            agendamentoColetaPedido.VolumesEnviar,
                            DataFimJanelaDescarga = agendamentoColetaPedido.Pedido.DataValidade?.ToString("dd/MM/yyyy") ?? "",
                            DataInicioJanelaDescarga = agendamentoColetaPedido.Pedido.DataInicioJanelaDescarga?.ToString("dd/MM/yyyy") ?? "",
                            CNPJRemetente = agendamentoColetaPedido.Pedido.Remetente?.CPF_CNPJ ?? 0,
                            CodigoIntegracaoProduto = ObterCodigoIntegracaoProduto(agendamentoColetaPedido, pedidosProdutos, unitOfWork.StringConexao),
                            DescricaoProduto = ObterDescricaoProduto(agendamentoColetaPedido, pedidosProdutos, unitOfWork.StringConexao),
                        }
                    ),
                    DocumentosParaTrasporte = (
                    from obj in listaDocumentosParaTransporte
                    select new
                    {
                        obj.Codigo,
                        NumeroNFE = obj.NumeroNF,
                        NumeroCTE = obj.NumeroCte,
                        CodigoFornecedor = obj.Fornecedor.Codigo,
                        Fornecedor = obj.Fornecedor.Descricao,
                        ChaveAcessoNFE = obj.ChaveNFe,
                        ChaveAcessoCTE = obj.ChaveCte,
                        obj.Observacao,
                        Status = obj.StatusDocumento.ObterDescricao(),
                        Peso = obj.Peso,
                        Volumen = obj.Volumen,
                        DT_RowClass = obj.StatusDocumento.ObterDescricao() == "OK" ? ClasseCorFundo.Sucess(IntensidadeCor._300) : ClasseCorFundo.Danger(IntensidadeCor._300)
                    }).ToList(),
                    ListaAnexos = (
                        from anexo in listaAnexos
                        select new
                        {
                            anexo.Codigo,
                            anexo.Descricao,
                            anexo.NomeArquivo
                        }
                    )
                }
                );

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar registro.");
            }
            finally
            {
                unitOfWork.Dispose();

            }
        }

        public async Task<IActionResult> BuscarPedidosAgendamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Logistica.AgendamentoColetaPedido repositorioAgendamentoColetaPedido = new Repositorio.Embarcador.Logistica.AgendamentoColetaPedido(unitOfWork);

                List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido> listaPedidos = repositorioAgendamentoColetaPedido.BuscarPorAgendamentoColeta(codigo);

                return new JsonpResult(new
                {
                    ListaPedidos = (
                        from agendamentoColetaPedido in listaPedidos
                        select new
                        {
                            agendamentoColetaPedido.Pedido.Codigo,
                            agendamentoColetaPedido.Pedido.NumeroPedidoEmbarcador,
                            Saldo = agendamentoColetaPedido.Pedido.SaldoVolumesRestante + agendamentoColetaPedido.VolumesEnviar,
                            VolumesEnviar = agendamentoColetaPedido.VolumesEnviar,
                            SKU = agendamentoColetaPedido.SKU,
                            DT_Enable = true,
                            DT_RowId = agendamentoColetaPedido.Pedido.Codigo
                        }
                    )
                });

            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os pedidos do agendamento.");
            }
            finally
            {
                unitOfWork.Dispose();

            }
        }

        public async Task<IActionResult> AlterarAgendamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Servicos.Embarcador.Logistica.CargaJanelaDescarregamento servicoJanelaDescarregamento = new Servicos.Embarcador.Logistica.CargaJanelaDescarregamento(unitOfWork);
                Repositorio.Embarcador.Logistica.AgendamentoColeta repositorioAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(unitOfWork);
                Repositorio.Embarcador.Logistica.AgendamentoColetaPedido repositorioAgendamentoColetaPedido = new Repositorio.Embarcador.Logistica.AgendamentoColetaPedido(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoAgendamentoColeta repositorioConfiguracaoAgendamentoColeta = new Repositorio.Embarcador.Configuracoes.ConfiguracaoAgendamentoColeta(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);

                int codigoAgendamento = Request.GetIntParam("CodigoAgendamento");

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta = repositorioAgendamentoColeta.BuscarPorCodigo(codigoAgendamento);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAgendamentoColeta configuracaoAgendamentoColeta = repositorioConfiguracaoAgendamentoColeta.BuscarPrimeiroRegistro();

                if (agendamentoColeta == null)
                    return new JsonpResult(true, "O registro não foi encontrado.");


                Servicos.Embarcador.Logistica.CargaJanelaDescarregamentoDisponibilidade servicoDisponibilidadeDescarregamento = new Servicos.Embarcador.Logistica.CargaJanelaDescarregamentoDisponibilidade(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repositorioDescarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento = repositorioDescarregamento.BuscarPorCarga(agendamentoColeta.Carga.Codigo);

                SalvarPedidosAgendamento(agendamentoColeta, unitOfWork, Cliente, configuracaoGeralCarga, cargaJanelaDescarregamento.Carga);

                List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido> agendamentoPedidos = repositorioAgendamentoColetaPedido.BuscarPedidosPorAgendamentoColeta(agendamentoColeta.Codigo);
                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = (from o in agendamentoPedidos select o.Pedido).ToList();

                int totalVolumes = agendamentoPedidos.Sum(o => o.VolumesEnviar);
                decimal totalValorVolumes = agendamentoPedidos.Sum(o => o.ValorVolumesEnviar);

                agendamentoColeta.Volumes = totalVolumes;
                agendamentoColeta.ValorTotalVolumes = totalValorVolumes;

                repositorioAgendamentoColeta.Atualizar(agendamentoColeta);

                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedido = repositorioCargaPedido.BuscarPorCarga(cargaJanelaDescarregamento.Carga.Codigo);

                foreach (Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido agendamentoPedido in agendamentoPedidos)
                {
                    if (cargasPedido.Any(cp => cp.Pedido.Codigo == agendamentoPedido.Pedido.Codigo))
                        continue;

                    Servicos.Embarcador.Carga.CargaPedido.AdicionarPedidoCarga(cargaJanelaDescarregamento.Carga, agendamentoPedido.Pedido, NumeroReboque.SemReboque, TipoCarregamentoPedido.Normal, ConfiguracaoEmbarcador, TipoServicoMultisoftware, unitOfWork, false);
                    EnviarEmailAdicaoPedido(agendamentoPedido, unitOfWork, Cliente);
                }

                try
                {
                    servicoDisponibilidadeDescarregamento.AlterarHorarioDescarregamento(cargaJanelaDescarregamento, cargaJanelaDescarregamento.InicioDescarregamento);
                }
                catch (ServicoException excecao)
                {
                    servicoDisponibilidadeDescarregamento.AlterarHorarioDescarregamentoSemVerificarDisponibilidade(cargaJanelaDescarregamento, cargaJanelaDescarregamento.InicioDescarregamento, true);
                }

                Servicos.Embarcador.Logistica.AgendamentoColeta servicoAgendamentoColeta = new Servicos.Embarcador.Logistica.AgendamentoColeta(unitOfWork, TipoServicoMultisoftware);
                Servicos.Embarcador.Logistica.CargaJanelaDescarregamento servicoCargaJanelaDescarregamento = new Servicos.Embarcador.Logistica.CargaJanelaDescarregamento(unitOfWork);

                agendamentoColeta.Senha = servicoAgendamentoColeta.ObterSenhaAgendamentoColeta(cargaJanelaDescarregamento, agendamentoColeta, configuracaoAgendamentoColeta);

                if (string.IsNullOrWhiteSpace(agendamentoColeta.Senha))
                {
                    agendamentoColeta.Situacao = SituacaoAgendamentoColeta.AguardandoGeracaoSenha;
                    servicoCargaJanelaDescarregamento.InserirHistoricoCargaJanelaDescarregamento(cargaJanelaDescarregamento, SituacaoCargaJanelaDescarregamento.AguardandoGeracaoSenha);
                    cargaJanelaDescarregamento.Situacao = SituacaoCargaJanelaDescarregamento.AguardandoGeracaoSenha;
                }
                else
                {
                    agendamentoColeta.Situacao = SituacaoAgendamentoColeta.Agendado;
                    servicoCargaJanelaDescarregamento.InserirHistoricoCargaJanelaDescarregamento(cargaJanelaDescarregamento, SituacaoCargaJanelaDescarregamento.AguardandoDescarregamento);
                    cargaJanelaDescarregamento.Situacao = SituacaoCargaJanelaDescarregamento.AguardandoDescarregamento;
                }

                repositorioDescarregamento.Atualizar(cargaJanelaDescarregamento);
                repositorioAgendamentoColeta.Atualizar(agendamentoColeta);

                servicoJanelaDescarregamento.AdicionarIntegracaoComAtualizacao(cargaJanelaDescarregamento.Carga, unitOfWork);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, agendamentoColeta, "Alterou agendamento", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar o agendamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConfiguracaoImportarPedidosPendentes()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>
            {
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = "Número do Pedido", Propriedade = "Pedido", Tamanho = 200 },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 2, Descricao = "Quantidade de caixas para enviar", Propriedade = "VolumeEnviar", Tamanho = 200 },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 3, Descricao = "Quantidade de itens", Propriedade = "SKU", Tamanho = 200 },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 4, Descricao = Localization.Resources.Pedidos.Pedido.CodigosProdutos, Propriedade = "CodigoIntegracaoProduto", Tamanho = 200 },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 5, Descricao = Localization.Resources.Pedidos.Pedido.DescricaoProdutos, Propriedade = "DescricaoProduto", Tamanho = 200 },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 6, Descricao = "Quantidade do Produto", Propriedade = "QuantidadeProdutoAgendamento", Tamanho = 200 }
            };
            return new JsonpResult(configuracoes.ToList());
        }

        public async Task<IActionResult> ImportarPedidosPendentes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string nome = Request.GetStringParam("Nome");
                string dados = Request.Params("Dados");

                List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha>>(dados);
                int total = linhas.Count;
                if (total == 0) return new JsonpResult(false, "Nenhuma linha encontrada na planilha");

                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoProduto repPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(unitOfWork);
                Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas repModalidadeFornecedorPessoas = new Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas(unitOfWork);
                Repositorio.Embarcador.Pedidos.AlteracaoPedidoProdutoAgendamento repAlteracaoPedidoProdutoAgendamento = new Repositorio.Embarcador.Pedidos.AlteracaoPedidoProdutoAgendamento(unitOfWork);
                Repositorio.Embarcador.Logistica.AgendamentoColetaPedidoProduto repAgendamentoColetaPedidoProduto = new Repositorio.Embarcador.Logistica.AgendamentoColetaPedidoProduto(unitOfWork);
                Repositorio.Embarcador.Logistica.CentroDescarregamento repCentroDescarregamento = new Repositorio.Embarcador.Logistica.CentroDescarregamento(unitOfWork);

                double cnpjRemetete = 0;
                int grupoPessoa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
                {
                    grupoPessoa = IsCompartilharAcessoEntreGrupoPessoas() && Usuario.ClienteFornecedor.GrupoPessoas != null ? Usuario.ClienteFornecedor.GrupoPessoas.Codigo : 0;
                    cnpjRemetete = grupoPessoa <= 0 ? Usuario.ClienteFornecedor.CPF_CNPJ : 0;
                }


                List<string> listFiltroCodigosPedidosPendentes = new List<string>();

                for (int i = 0; i < total; i++)
                {
                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colPedido = (from obj in linhas[i].Colunas where obj.NomeCampo == "Pedido" select obj).FirstOrDefault();
                    if (colPedido != null)
                        listFiltroCodigosPedidosPendentes.Add(colPedido.Valor);
                }

                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> listaPedido = repPedido.BuscarPendentesPorRemetente(cnpjRemetete, 0, grupoPessoa, listFiltroCodigosPedidosPendentes, null);
                var listaPedidoTemp = (
                    from pedido in listaPedido
                    select new
                    {
                        pedido.Codigo,
                        pedido.NumeroPedidoEmbarcador,
                        TipoOperacao = pedido.TipoOperacao?.Descricao ?? "",
                        Destinatario = pedido.Destinatario?.Descricao ?? "",
                        CodigoDestinatario = pedido.Destinatario?.Codigo ?? 0,
                        TipoCarga = pedido.TipoDeCarga?.Descricao ?? "",
                        TipoCargaCodigo = pedido.TipoDeCarga?.Codigo ?? 0,
                        pedido.QtVolumes,
                        Saldo = pedido.SaldoVolumesRestante,
                        SKU = (int)pedido.Produtos?.Count,
                        QtProdutos = pedido.Produtos?.Sum(x => x.Quantidade),
                        VolumesEnviar = pedido.SaldoVolumesRestante,
                        DataFimJanelaDescarga = ObterDataMaximaAgendamento(pedido.DataValidade),
                        DataCriacao = pedido.DataCriacao?.ToString("dd/MM/yyyy") ?? "",
                        DataInicioJanelaDescarga = pedido.DataInicioJanelaDescarga?.ToString("dd/MM/yyyy HH:mm") ?? "",
                        ProdutoPrincipal = pedido.ProdutoPrincipal?.Descricao ?? "",
                        DescricaoFilial = pedido?.Filial?.Descricao ?? "",
                        CNPJRemetente = pedido?.Remetente?.CPF_CNPJ.ToString() ?? "",
                        CodigoIntegracaoProduto = string.Join(", ", pedido.Produtos?.Select(x => x.Produto.CodigoProdutoEmbarcador.ToString())) ?? string.Empty,
                        DescricaoProduto = string.Join(", ", pedido.Produtos?.Select(x => x.Produto.Descricao)) ?? string.Empty,
                        UsarLayoutAgendamentoPorCaixaItem = repCentroDescarregamento.BuscarPorDestinatario(pedido.Destinatario.Codigo)?.UsarLayoutAgendamentoPorCaixaItem ?? false
                    }
                );
                var listaPedidoRetornar = listaPedidoTemp.ToList();

                List<int> listCodigosPedidosImportar = new List<int>();

                for (int i = 0; i < total; i++)
                {
                    dynamic pedido = null;
                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colPedido = (from obj in linhas[i].Colunas where obj.NomeCampo == "Pedido" select obj).FirstOrDefault();
                    if (colPedido != null)
                    {
                        pedido = listaPedidoRetornar.Find(x => x.NumeroPedidoEmbarcador == colPedido.Valor);
                        if (pedido != null)
                            listCodigosPedidosImportar.Add(pedido.Codigo);
                    }
                }

                List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedidoProduto> listAgendamentoColetaPedidoProduto = repAgendamentoColetaPedidoProduto.BuscarPorListaCodigoPedidoAgendado(listCodigosPedidosImportar);
                List<Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedidoProdutoAgendamento> listPedidoProdutoAgendamento = repAlteracaoPedidoProdutoAgendamento.BuscarPorAlteracaoPedidosNaoVinculado(listCodigosPedidosImportar);
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> listPedidoProdutos = repPedidoProduto.BuscarPorPedidos(listCodigosPedidosImportar);

                int importados = 0;
                int qtdeVolumeEnviar = 0;
                int qtdeSKU = 0;
                int quantidadeProdutos = 0;
                int codigoPedidoAntigo = 0;
                dynamic retornoLinhas = new List<dynamic>();
                for (int i = 0; i < total; i++)
                {
                    bool processou = true;
                    dynamic pedido = null;
                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colPedido = (from obj in linhas[i].Colunas where obj.NomeCampo == "Pedido" select obj).FirstOrDefault();
                    if (colPedido != null)
                    {
                        pedido = listaPedidoRetornar.Find(x => x.NumeroPedidoEmbarcador == colPedido.Valor);

                        if (pedido != null && codigoPedidoAntigo != pedido.Codigo)
                        {
                            codigoPedidoAntigo = pedido.Codigo;
                            qtdeVolumeEnviar = 0;
                            qtdeSKU = 0;
                            quantidadeProdutos = 0;
                        }
                    }

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colVolumesEnviar = (from obj in linhas[i].Colunas where obj.NomeCampo == "VolumeEnviar" select obj).FirstOrDefault();
                    if (colVolumesEnviar != null)
                    {
                        string somenteNumeros = Utilidades.String.OnlyNumbers(colVolumesEnviar.Valor);
                        if (!string.IsNullOrEmpty(somenteNumeros))
                        {
                            qtdeVolumeEnviar += Int32.Parse(somenteNumeros);
                        }
                    }

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colSKU = (from obj in linhas[i].Colunas where obj.NomeCampo == "SKU" select obj).FirstOrDefault();
                    if (colSKU != null)
                    {
                        string somenteNumeros = Utilidades.String.OnlyNumbers(colSKU.Valor);
                        if (!string.IsNullOrEmpty(somenteNumeros))
                        {
                            qtdeSKU += Int32.Parse(somenteNumeros);
                        }
                    }

                    if (pedido != null)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colProdutoaImportar = (from obj in linhas[i].Colunas where obj.NomeCampo == "CodigoIntegracaoProduto" select obj).FirstOrDefault();
                        if (colProdutoaImportar != null)
                        {
                            List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> listProdutosImportarPorPedido = listPedidoProdutos.FindAll(x => x.Pedido.NumeroPedidoEmbarcador == pedido.NumeroPedidoEmbarcador);
                            Dominio.Entidades.Embarcador.Pedidos.PedidoProduto pedidoProduto = listProdutosImportarPorPedido.Find(x => x.Produto.CodigoProdutoEmbarcador == ((string)colProdutoaImportar.Valor)?.Trim());
                            if (pedidoProduto != null)
                            {
                                int saldoProdutoAgendamento = (int)pedidoProduto.Quantidade;
                                if (listPedidoProdutoAgendamento != null && listPedidoProdutoAgendamento.Count > 0)
                                {
                                    foreach (Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedidoProdutoAgendamento pedidoProdutoAgendamentoDeletar in listPedidoProdutoAgendamento)
                                    {
                                        if (repAlteracaoPedidoProdutoAgendamento.BuscarPorCodigo(pedidoProdutoAgendamentoDeletar.Codigo) != null)
                                            repAlteracaoPedidoProdutoAgendamento.Deletar(pedidoProdutoAgendamentoDeletar);
                                    }
                                }

                                List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedidoProduto> listProdutosAgendadosDoPedido = listAgendamentoColetaPedidoProduto.FindAll(x => x.PedidoProduto.Codigo == pedidoProduto.Codigo);
                                if (listProdutosAgendadosDoPedido != null)
                                {
                                    foreach (Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedidoProduto produtoAgendado in listProdutosAgendadosDoPedido)
                                        saldoProdutoAgendamento -= produtoAgendado.Quantidade;
                                }

                                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colQuantidadeProdutoaImportar = (from obj in linhas[i].Colunas where obj.NomeCampo == "QuantidadeProdutoAgendamento" select obj).FirstOrDefault();
                                if (colQuantidadeProdutoaImportar != null)
                                {
                                    string somenteNumeros = Utilidades.String.OnlyNumbers(colQuantidadeProdutoaImportar.Valor);
                                    int quantidadeProdutoImportado = Int32.Parse(colQuantidadeProdutoaImportar.Valor);
                                    if (!string.IsNullOrEmpty(somenteNumeros) && saldoProdutoAgendamento >= quantidadeProdutoImportado)
                                    {
                                        saldoProdutoAgendamento = quantidadeProdutoImportado;
                                    }
                                }

                                Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedidoProdutoAgendamento pedidoProdutoAgendamento = new Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedidoProdutoAgendamento();

                                pedidoProdutoAgendamento.PedidoProduto = pedidoProduto;
                                pedidoProdutoAgendamento.NovaQuantidadeProduto = saldoProdutoAgendamento;
                                pedidoProdutoAgendamento.ImportadoPorPlanilha = true;

                                repAlteracaoPedidoProdutoAgendamento.Inserir(pedidoProdutoAgendamento);
                                quantidadeProdutos += (int)pedidoProdutoAgendamento.NovaQuantidadeProduto;
                                qtdeVolumeEnviar += (int)Math.Ceiling((decimal)quantidadeProdutos / Math.Max(pedidoProduto.Produto.QuantidadeCaixa, 1));
                            }
                            else
                                processou = false;
                        }
                    }

                    if (pedido != null)
                    {
                        retornoLinhas.Add(new
                        {
                            indice = i,
                            pedido = pedido,
                            codigo = pedido.Codigo,
                            VolumeEnviar = qtdeVolumeEnviar,
                            SKU = qtdeSKU,
                            QuantidadeProdutos = quantidadeProdutos,
                            processou = processou,
                            mensagemFalha = !processou ? "Ignorada" : "",
                            contar = true,
                            pedido.UsarLayoutAgendamentoPorCaixaItem
                        });
                        importados++;
                    }
                    else
                    {
                        retornoLinhas.Add(new
                        {
                            indice = i,
                            mensagemFalha = "Ignorada",
                            processou = false
                        });
                    }
                }
                return new JsonpResult(new
                {
                    Total = total,
                    Importados = importados,
                    Retornolinhas = retornoLinhas
                });
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

        public async Task<IActionResult> ObterDetalhesTipoCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repTipoOperacao.BuscarTipoOperacaoPorTipoDeCarga(codigo);

                var retorno = new
                {
                    Codigo = tipoOperacao?.Codigo ?? 0,
                    Descricao = tipoOperacao?.Descricao ?? string.Empty,
                    NaoObrigarInformarModeloVeicularAgendamento = tipoOperacao?.ConfiguracaoAgendamentoColetaEntrega?.NaoObrigarInformarModeloVeicularAgendamento ?? false,
                    NaoObrigarInformarTransportadorAgendamento = tipoOperacao?.ConfiguracaoAgendamentoColetaEntrega?.NaoObrigarInformarTransportadorAgendamento ?? false
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao obter os Detalhes do Tipo de Carga");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConfiguracaoImportacaoDocumentoTransporte()
        {
            try
            {
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoPlanilhaDocumentoTransporte();

                return new JsonpResult(configuracoes);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, true, Localization.Resources.Canhotos.Canhoto.OcorreuUmaFalhaAoObterAsConfiguracoesParaImportacao);
            }
        }

        public async Task<IActionResult> ObterDadosDocumentoTransportePlanilha()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao();

                retornoImportacao.Retornolinhas = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha>();

                List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoPlanilhaDocumentoTransporte();

                string erro = string.Empty;
                int contador = 0;
                string dados = Request.Params("Dados");

                List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha>>(dados);
                int totalLinhas = linhas.Count;
                List<Dominio.ObjetosDeValor.Embarcador.Logistica.DocumentoTransporte> retorno = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.DocumentoTransporte>();

                for (int i = 0; i < totalLinhas; i++)
                {
                    try
                    {
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha = linhas[i];
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNumeroNFe = linha.Colunas?.Where(o => o.NomeCampo == "NumeroNFE").FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNumeroCTe = linha.Colunas?.Where(o => o.NomeCampo == "NumeroCTE").FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colChaveAcessoCTE = linha.Colunas?.Where(o => o.NomeCampo == "ChaveAcessoCTE").FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colChaveAcessoNFE = linha.Colunas?.Where(o => o.NomeCampo == "ChaveAcessoNFE").FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colPeso = linha.Colunas?.Where(o => o.NomeCampo == "Peso").FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colVolumen = linha.Colunas?.Where(o => o.NomeCampo == "Volumen").FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colStatus = linha.Colunas?.Where(o => o.NomeCampo == "Status").FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colObservacao = linha.Colunas?.Where(o => o.NomeCampo == "Observacao").FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colFornecedor = linha.Colunas?.Where(o => o.NomeCampo == "Fornecedor").FirstOrDefault();

                        Dominio.ObjetosDeValor.Embarcador.Logistica.DocumentoTransporte documentoTransporte = new Dominio.ObjetosDeValor.Embarcador.Logistica.DocumentoTransporte();

                        if (colNumeroNFe == null || colNumeroNFe?.Valor == null)
                            throw new ControllerException("Valor NFe não informado");

                        if (colNumeroCTe == null || colNumeroCTe?.Valor == null)
                            throw new ControllerException("Valor CTe não informado");

                        if (colChaveAcessoCTE == null || colChaveAcessoCTE?.Valor == null)
                            throw new ControllerException("Valor Chave Acesso CTe não informado");

                        if (colChaveAcessoNFE == null || colChaveAcessoNFE?.Valor == null)
                            throw new ControllerException("Valor Chave Acesso NFe não informado");

                        //if(!Utilidades.Validate.ValidarChaveNFe(colChaveAcessoNFE?.Valor))
                        //    throw new ControllerException("Formato de chave NFe invalido");

                        if (colPeso == null || colPeso?.Valor == null)
                            throw new ControllerException("Valor Peso não informado");

                        if (colVolumen == null || colVolumen?.Valor == null)
                            throw new ControllerException("Valor Volumen não informado");

                        if (colStatus == null || colStatus?.Valor == null)
                            throw new ControllerException("Valor Status não informado");

                        if (colFornecedor == null || colFornecedor?.Valor == null)
                            throw new ControllerException("Valor Fornecedor não informado");

                        if (!Utilidades.Validate.ValidarCNPJ((string)colFornecedor?.Valor))
                            throw new ControllerException("CNPJ informado não é valido");

                        double cpnj = ((string)colFornecedor?.Valor).ToString().ObterSomenteNumeros().ToDouble();

                        Dominio.Entidades.Cliente fornecedor = repCliente.BuscarPorCPFCNPJ(cpnj);

                        if (fornecedor == null)
                            throw new ControllerException($"Não foi encontrador fornecendor com o Cnpj {cpnj}");

                        retorno.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.DocumentoTransporte()
                        {
                            NumeroCTE = (string)colNumeroCTe.Valor,
                            NumeroNFE = (string)colNumeroNFe.Valor,
                            ChaveAcessoCTE = (string)colChaveAcessoCTE.Valor,
                            ChaveAcessoNFE = (string)colChaveAcessoNFE.Valor,
                            CodigoFornecedor = fornecedor.Codigo.ToString(),
                            Fornecedor = fornecedor.Descricao,
                            Peso = (string)colPeso.Valor,
                            Volumen = (string)colVolumen.Valor,
                            Status = (string)colStatus.Valor.Trim() == "OK" ? "OK" : "NÃO OK",
                            Observacao = (string)colObservacao?.Valor ?? string.Empty,
                            DT_RowClass = (string)colStatus.Valor.Trim() == "OK" ? ClasseCorFundo.Sucess(IntensidadeCor._300) : ClasseCorFundo.Danger(IntensidadeCor._300)
                        });
                        contador++;
                        retornoImportacao.Retornolinhas.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = i, processou = true, mensagemFalha = "" });
                    }
                    catch (ControllerException exception)
                    {
                        retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha(exception.Message, i));
                    }
                }

                retornoImportacao.Retorno = retorno;
                retornoImportacao.Total = contador;
                retornoImportacao.Importados = contador;

                return new JsonpResult(retornoImportacao);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoImportarArquivo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> SalvarDocumentoParaTransporte()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Logistica.AgendamentoColeta repositorioAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta = repositorioAgendamentoColeta.BuscarPorCodigo(codigo, true);

                if (agendamentoColeta.EtapaAgendamentoColeta != EtapaAgendamentoColeta.DadosTransporte
                    && agendamentoColeta.EtapaAgendamentoColeta != EtapaAgendamentoColeta.DocumentoParaTransporte)
                    throw new Exception("Não é possível mudar a etapa.");

                agendamentoColeta.EtapaAgendamentoColeta = EtapaAgendamentoColeta.DocumentoParaTransporte;

                this.SalvarDocumentoParaTransporte(agendamentoColeta, unitOfWork);
                repositorioAgendamentoColeta.Atualizar(agendamentoColeta);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, ex.Message);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> FinalizarAgendamentoColeta()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
                Repositorio.Embarcador.Logistica.DocumentoTransporte repDocumentoTrasnporte = new Repositorio.Embarcador.Logistica.DocumentoTransporte(unitOfWork);
                Repositorio.Embarcador.Logistica.AgendamentoColeta repositorioAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta = repositorioAgendamentoColeta.BuscarPorCodigo(codigo, true);
                List<Dominio.Entidades.Embarcador.Logistica.DocumentoTransporte> listaDocumentosParaTransporte = repDocumentoTrasnporte.BuscarPorCodigoAgendamento(codigo);

                if (agendamentoColeta == null)
                    throw new ControllerException("Agendamento Coleta não encontado");

                if (listaDocumentosParaTransporte == null)
                    throw new ControllerException("Sem Documentos para transporte para Finalizar o agendamento");

                this.CadastroXMLNF(listaDocumentosParaTransporte, agendamentoColeta, unitOfWork);
                this.CadastroCtes(listaDocumentosParaTransporte, agendamentoColeta, unitOfWork);

                agendamentoColeta.Situacao = SituacaoAgendamentoColeta.Finalizado;
                agendamentoColeta.Carga.PossuiPendencia = false;
                agendamentoColeta.Carga.AutorizouTodosCTes = true;
                agendamentoColeta.Carga.EmitindoCTes = false;
                agendamentoColeta.Carga.SituacaoCarga = SituacaoCarga.EmTransporte;
                agendamentoColeta.Carga.DataMudouSituacaoParaEmTransporte = DateTime.Now;
                Servicos.Auditoria.Auditoria.Auditar(Auditado, agendamentoColeta.Carga, $"Alterou carga para situação {Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte.ObterDescricao()}", unitOfWork);

                repositorioAgendamentoColeta.Atualizar(agendamentoColeta);
                repositorioCarga.Atualizar(agendamentoColeta.Carga);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException exception)
            {
                Servicos.Log.TratarErro(exception);
                unitOfWork.Rollback();
                return new JsonpResult(false, exception.Message);
            }
            catch (Exception exception)
            {
                Servicos.Log.TratarErro(exception);
                unitOfWork.Rollback();
                return new JsonpResult(false, exception.Message);
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        public async Task<IActionResult> AlterarProdutos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Pedidos.AlteracaoPedidoProdutoAgendamento repAlteracaoPedidoProdutoAgendamento = new Repositorio.Embarcador.Pedidos.AlteracaoPedidoProdutoAgendamento(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoProduto repPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

                unitOfWork.Start();
                int codigoPedido = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repPedido.BuscarPorCodigo(codigoPedido);

                dynamic dynProdutos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Produtos"));

                if (dynProdutos == null || dynProdutos.Count == 0)
                    throw new ControllerException("Não é possível salvar sem produtos");

                if (pedido == null)
                    throw new ControllerException("Não foi possível encontrar o pedido");

                foreach (var produto in dynProdutos)
                {
                    if ((decimal)produto.Quantidade > (decimal)produto.QuantidadeOriginal)
                        throw new ServicoException("Quantidade digitada não pode ser maior que a quantidade original do produto no pedido.");

                    if ((decimal)produto.Quantidade < 0)
                        throw new ServicoException("Quantidade digitada não pode ser menor que 0.");
                }

                List<Dominio.ObjetosDeValor.Embarcador.Pedido.RetornoGridAgendamentoColetaAlterarProdutos> listaRetorno = new List<Dominio.ObjetosDeValor.Embarcador.Pedido.RetornoGridAgendamentoColetaAlterarProdutos>();
                List<Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedidoProdutoAgendamento> listPedidoProdutoAgendamento = repAlteracaoPedidoProdutoAgendamento.BuscarPorPedidoNaoVinculado(pedido.Codigo);

                foreach (var produto in dynProdutos)
                {
                    decimal novaQuantidadeProdutoAgendamento = (decimal)produto.Quantidade;
                    int quantidadeOriginal = (int)produto.QuantidadeOriginal;
                    bool inserirPedidoProdutoAgendamento = false;
                    int codigo = 0;
                    int.TryParse((string)produto.Codigo, out codigo);
                    Dominio.Entidades.Embarcador.Pedidos.PedidoProduto pedidoProduto = repPedidoProduto.BuscarPorCodigoFetch(codigo);

                    if (pedidoProduto == null)
                        throw new ControllerException("Ocorreu uma falha ao salvar os produtos");

                    Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedidoProdutoAgendamento pedidoProdutoAgendamento = listPedidoProdutoAgendamento.Find(x => x.PedidoProduto.Codigo == codigo);

                    if ((bool)produto.Removido || (decimal)produto.Quantidade == 0)
                    {
                        produto.Removido = true;
                        if (pedidoProdutoAgendamento != null)
                            repAlteracaoPedidoProdutoAgendamento.Deletar(pedidoProdutoAgendamento);

                        listaRetorno.Add(new Dominio.ObjetosDeValor.Embarcador.Pedido.RetornoGridAgendamentoColetaAlterarProdutos
                        {
                            Codigo = pedidoProduto.Codigo,
                            CodigoProduto = pedidoProduto.Produto.Codigo,
                            CodigoEmbarcador = pedidoProduto.Produto.CodigoProdutoEmbarcador ?? string.Empty,
                            Descricao = pedidoProduto.Produto?.Descricao ?? string.Empty,
                            Setor = pedidoProduto.Produto.GrupoProduto.Descricao,
                            Quantidade = produto.Quantidade,
                            QuantidadeOriginal = quantidadeOriginal,
                            QuantidadeCaixas = (int)Math.Ceiling((decimal)produto.Quantidade / Math.Max(pedidoProduto.Produto.QuantidadeCaixa, 1)),
                            Removido = true,
                            DT_Enable = true,
                            DT_RowColor = "#ebc3c3",
                            DT_RowId = pedidoProduto.Codigo
                        });
                        continue;
                    }

                    if (pedidoProdutoAgendamento == null)
                    {
                        inserirPedidoProdutoAgendamento = true;
                        pedidoProdutoAgendamento = new Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedidoProdutoAgendamento();
                    }

                    pedidoProdutoAgendamento.PedidoProduto = pedidoProduto;
                    pedidoProdutoAgendamento.NovaQuantidadeProduto = novaQuantidadeProdutoAgendamento;

                    if (inserirPedidoProdutoAgendamento)
                        repAlteracaoPedidoProdutoAgendamento.Inserir(pedidoProdutoAgendamento);
                    else
                        repAlteracaoPedidoProdutoAgendamento.Atualizar(pedidoProdutoAgendamento);

                    listaRetorno.Add(new Dominio.ObjetosDeValor.Embarcador.Pedido.RetornoGridAgendamentoColetaAlterarProdutos
                    {
                        Codigo = pedidoProduto.Codigo,
                        CodigoProduto = pedidoProduto.Produto.Codigo,
                        CodigoEmbarcador = pedidoProduto.Produto.CodigoProdutoEmbarcador ?? string.Empty,
                        Descricao = pedidoProduto.Produto?.Descricao ?? string.Empty,
                        Setor = pedidoProduto.Produto.GrupoProduto.Descricao,
                        Quantidade = novaQuantidadeProdutoAgendamento,
                        QuantidadeOriginal = quantidadeOriginal,
                        QuantidadeCaixas = (int)Math.Ceiling(novaQuantidadeProdutoAgendamento / Math.Max(pedidoProduto.Produto.QuantidadeCaixa, 1)),
                        Removido = false,
                        DT_Enable = true,
                        DT_RowColor = "",
                        DT_RowId = pedidoProduto.Codigo
                    });

                }
                unitOfWork.CommitChanges();

                int quantidadeCaixas = 0; int sku = 0; int quantidadeProdutos = 0;
                CalcularDadosRetorno(listaRetorno, ref quantidadeCaixas, ref sku, ref quantidadeProdutos);

                return new JsonpResult(new { ListaProdutos = listaRetorno, QuantidadeEnviar = quantidadeCaixas, SKU = sku, QuantidadeProdutos = quantidadeProdutos }, true, "Produtos alterados com sucesso");
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, excecao.Message);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                unitOfWork.Rollback();
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAtualizar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterConfiguracoesTelaAgendamentoColeta()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoAgendamentoColeta repositorioConfiguracaoAgendamentoColeta = new Repositorio.Embarcador.Configuracoes.ConfiguracaoAgendamentoColeta(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAgendamentoColeta configuracaoAgendamentoColeta = repositorioConfiguracaoAgendamentoColeta.BuscarPrimeiroRegistro();

                dynamic dyn = new
                {
                    MostrarTipoDeOperacaoNoPortalMultiEmbarcadorAgendamentoColeta = configuracaoAgendamentoColeta?.MostrarTipoDeOperacaoNoPortalMultiEmbarcadorAgendamentoColeta ?? false,
                };

                return new JsonpResult(dyn);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar a senha.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarTempoDeDescargaDaRota()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int minutosAdicionaisDaRota = 0;
                DateTime? dataCarregamento = Request.GetNullableDateTimeParam("DataCarregamento");
                if (!dataCarregamento.HasValue)
                    dataCarregamento = DateTime.Now;
                DateTime? dataEntrega = dataCarregamento;

                double.TryParse(Request.GetStringParam("Remetente"), out double codigoRemetente);
                double.TryParse(Request.GetStringParam("Destinatario"), out double codigoDestinatario);
                int.TryParse(Request.GetStringParam("ModeloVeicularCarga"), out int codigoModeloVeicularCarga);
                int.TryParse(Request.GetStringParam("TipoDeCarga"), out int codigoTipoDeCarga);

                Servicos.Embarcador.Carga.RotaFrete serRotaFrete = new Servicos.Embarcador.Carga.RotaFrete(unitOfWork);

                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Localidade repositorioLocalidade = new Repositorio.Localidade(unitOfWork);
                Repositorio.RotaFrete repositorioRotaFrete = new Repositorio.RotaFrete(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoAgendamentoColeta repConfiguracaoAgendamentoColeta = new Repositorio.Embarcador.Configuracoes.ConfiguracaoAgendamentoColeta(unitOfWork);

                Dominio.Entidades.Cliente remetente = repCliente.BuscarPorCPFCNPJ(codigoRemetente);
                Dominio.Entidades.Cliente destinatario = repCliente.BuscarPorCPFCNPJ(codigoDestinatario);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAgendamentoColeta configAgendamentoColeta = repConfiguracaoAgendamentoColeta.BuscarPrimeiroRegistro();

                Dominio.Entidades.RotaFrete rotaFrete = null;
                if (remetente != null && destinatario != null)
                    rotaFrete = repositorioRotaFrete.BuscarPorRemetenteDestinatario(remetente.CPF_CNPJ, destinatario.CPF_CNPJ);

                if (configAgendamentoColeta?.CalcularDataDeEntregaPorTempoDeDescargaDaRota ?? false)
                {
                    if (rotaFrete != null)
                        minutosAdicionaisDaRota = (int)rotaFrete.TempoDescarga.TotalMinutes;

                    if (minutosAdicionaisDaRota == 0)
                        minutosAdicionaisDaRota = configAgendamentoColeta.TempoPadraoDeDescargaMinutos;

                    dataEntrega = dataCarregamento.Value.AddMinutes(minutosAdicionaisDaRota);

                    if (rotaFrete != null)
                        dataEntrega = serRotaFrete.ObtemDataEntregaComRestricao(rotaFrete, dataEntrega, codigoModeloVeicularCarga, codigoTipoDeCarga);
                }

                return new JsonpResult(new
                {
                    Sucesso = dataEntrega.HasValue,
                    Mensagem = "Não há data disponível para entrega para a Rota definida. Verificaque o cadastro de Restrições da Rota de Frete.",
                    DataEntrega = dataEntrega.HasValue ? dataEntrega.Value.ToString("dd/MM/yyyy HH:mm:ss") : "",
                    MinutosAdicionaisDaRota = minutosAdicionaisDaRota
                });
            }
            catch (ControllerException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao obter os dados da rota de frete.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void EncaminharParaTransporte(Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta, bool validarNotasFiscais, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.AgendamentoColeta repositorioAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaNFeAnexo repositorioCargaNFeAnexo = new Repositorio.Embarcador.Cargas.CargaNFeAnexo(unitOfWork);
            Repositorio.Embarcador.Logistica.AgendamentoColetaAnexo repositorioAgendamentoColetaAnexo = new Repositorio.Embarcador.Logistica.AgendamentoColetaAnexo(unitOfWork);

            if (agendamentoColeta.EtapaAgendamentoColeta != EtapaAgendamentoColeta.NFe)
                throw new ControllerException("Não é possível avançar a etapa.");

            if (agendamentoColeta.Pedido == null && agendamentoColeta.Carga != null)
            {
                if (validarNotasFiscais)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorCarga(agendamentoColeta.Carga.Codigo);

                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                    {
                        if (cargaPedido.NotasFiscais.Count == 0)
                            throw new ControllerException("Não foram informadas as notas fiscais do pedido.");

                        if (cargaPedido.NotasFiscais.Where(a => a.XMLNotaFiscal.nfAtiva).Any(notaFiscal => notaFiscal.XMLNotaFiscal.Peso == 0))
                            throw new ControllerException("É necessário que todas as notas estejam com o peso informado para prosseguir.");
                    }
                }

                if (agendamentoColeta.Carga.SituacaoCarga == SituacaoCarga.AgNFe)
                {
                    List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaAnexo> agendamentoColetaAnexos = repositorioAgendamentoColetaAnexo.BuscarPorAgendamentoColeta(agendamentoColeta.Codigo);
                    if (repositorioCargaNFeAnexo.ContarPorCarga(agendamentoColeta.Carga.Codigo) <= 0 && agendamentoColetaAnexos.Count > 0)
                    {
                        EnviarAnexoAgendamentoColetaParaNFe(agendamentoColeta, agendamentoColetaAnexos, unitOfWork);
                    }

                    if (!Servicos.Embarcador.Carga.Carga.AvancarEtapaDocumentosEmissaoCarga(out string erro, agendamentoColeta.Carga.Codigo, false, ConfiguracaoEmbarcador, TipoServicoMultisoftware, unitOfWork, null, Usuario, Auditado))
                        throw new ControllerException(erro);
                }
                else
                {
                    agendamentoColeta.Carga.DataInicioEmissaoDocumentos = DateTime.Now;
                    agendamentoColeta.Carga.DataEnvioUltimaNFe = DateTime.Now;

                    repositorioCarga.Atualizar(agendamentoColeta.Carga);
                }
            }
            else if (agendamentoColeta.Pedido != null)
            {
                if (validarNotasFiscais)
                {
                    if (agendamentoColeta.Pedido.NotasFiscais == null || agendamentoColeta.Pedido.NotasFiscais.Count == 0)
                        throw new ControllerException("Não foram informadas as notas fiscais do pedido.");

                    if (agendamentoColeta.Pedido.NotasFiscais.Where(a => a.nfAtiva).Any(notaFiscal => notaFiscal.Peso == 0))
                        throw new ControllerException("É necessário que todas as notas estejam com o peso informado para prosseguir.");

                    decimal pesoTotal = agendamentoColeta.Pedido.NotasFiscais.Sum(x => x.Peso);
                    decimal capacidade = agendamentoColeta.Carga?.ModeloVeicularCarga?.CapacidadePesoTransporte ?? agendamentoColeta.ModeloVeicular?.CapacidadePesoTransporte ?? 0m;

                    if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor && pesoTotal > capacidade)
                        throw new ControllerException($"O peso total das notas fiscais ({pesoTotal.ToString("n2")}) é maior que a capacidade do modelo veicular ({capacidade.ToString("n2")}).");
                }
            }

            Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento repositorioConfiguracaoJanelaCarregamento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento configuracaoJanelaCarregamento = repositorioConfiguracaoJanelaCarregamento.BuscarPrimeiroRegistro();

            if (!ConfiguracaoEmbarcador.ControlarAgendamentoSKU && !configuracaoJanelaCarregamento.ExibirOpcaoMultiModalAgendamentoColeta)
                agendamentoColeta.Situacao = SituacaoAgendamentoColeta.AguardandoCTes;

            agendamentoColeta.EtapaAgendamentoColeta = EtapaAgendamentoColeta.Emissao;

            repositorioAgendamentoColeta.Atualizar(agendamentoColeta, Auditado);

            if (agendamentoColeta.Transportador != null && !agendamentoColeta.ApenasGerarPedido)
            {
                Servicos.Embarcador.Notificacao.NotificacaoEmpresa servicoNotificacaoEmpresa = new Servicos.Embarcador.Notificacao.NotificacaoEmpresa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoEmpresa notificacaoEmailEmpresa = new Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoEmpresa()
                {
                    AssuntoEmail = "Carga Aguardando Autorização para Emissão dos Documentos",
                    CabecalhoMensagem = "Carga Aguardando Autorização para Emissão dos Documentos",
                    Empresa = agendamentoColeta.Transportador,
                    Mensagem = $"Olá {agendamentoColeta.Transportador.NomeFantasia}, a carga {agendamentoColeta.Carga.CodigoCargaEmbarcador} está aguardando Autorização para Emissão dos Documentos."
                };

                servicoNotificacaoEmpresa.GerarNotificacaoEmail(notificacaoEmailEmpresa);
            }
        }

        private string ProcessarXMLNFe(System.IO.Stream xml, Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta, Repositorio.UnitOfWork unitOfWork, out bool msgAlertaObservacao)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoNF = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebservice = repConfiguracaoWebService.BuscarConfiguracaoPadrao();

            msgAlertaObservacao = false;
            string retorno = "";

            xml.Position = 0;
            Servicos.Embarcador.NFe.NFe serNFe = new Servicos.Embarcador.NFe.NFe(unitOfWork);
            System.IO.StreamReader stReaderXML = new System.IO.StreamReader(xml);

            if (!serNFe.BuscarDadosNotaFiscal(out string erro, out Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal, stReaderXML, unitOfWork, null, true, false, false, TipoServicoMultisoftware, false, ConfiguracaoEmbarcador?.UtilizarValorFreteNota ?? false, null, null, null, configuracaoWebservice?.CadastroAutomaticoPessoaExterior ?? false))
                return erro;

            if (!ConfiguracaoEmbarcador.ControlarAgendamentoSKU && this.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor && agendamentoColeta.Pedido != null)
            {
                if (xmlNotaFiscal.Destinatario?.Codigo != agendamentoColeta.Pedido?.Destinatario?.Codigo)
                    return $"O destinatário da nota ({xmlNotaFiscal.Destinatario.Descricao}) não é o mesmo do pedido ({agendamentoColeta.Pedido.Destinatario.Descricao}).";

                if (xmlNotaFiscal.Emitente?.Codigo != agendamentoColeta.Pedido?.Remetente?.Codigo)
                    return $"O remetente da nota ({xmlNotaFiscal.Emitente.Descricao}) não é o mesmo do pedido ({agendamentoColeta.Pedido.Remetente.Descricao}).";
            }

            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);

            if (xmlNotaFiscal.Codigo == 0)
            {
                xmlNotaFiscal.DataRecebimento = DateTime.Now;
                repXmlNotaFiscal.Inserir(xmlNotaFiscal);
            }
            else
                repXmlNotaFiscal.Atualizar(xmlNotaFiscal);

            if (string.IsNullOrEmpty(retorno) || msgAlertaObservacao)
            {
                PedidoXMLNotaFiscal servicoPedidoXMLNotaFiscal = new PedidoXMLNotaFiscal(unitOfWork, ConfiguracaoEmbarcador);

                xmlNotaFiscal.SemCarga = false;

                servicoPedidoXMLNotaFiscal.ArmazenarProdutosXML(xmlNotaFiscal.XML, xmlNotaFiscal, Auditado, TipoServicoMultisoftware, agendamentoColeta.Pedido);

                if (agendamentoColeta.Pedido != null)
                    agendamentoColeta.Pedido.NotasFiscais.Add(xmlNotaFiscal);
                else if (agendamentoColeta.Carga != null)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorCarga(agendamentoColeta.Carga.Codigo);
                    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = cargaPedidos != null && cargaPedidos.Count > 0 ? cargaPedidos.FirstOrDefault() : null;

                    if (cargaPedido != null)
                    {
                        Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = repositorioPedidoNF.BuscarPorCargaPedidoEXMLNotaFiscal(cargaPedido.Codigo, xmlNotaFiscal.Codigo);

                        if (pedidoXMLNotaFiscal == null)
                        {
                            pedidoXMLNotaFiscal = new Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal()
                            {
                                XMLNotaFiscal = xmlNotaFiscal,
                                CargaPedido = cargaPedido
                            };

                            repositorioPedidoNF.Inserir(pedidoXMLNotaFiscal);
                        }

                    }
                }

                msgAlertaObservacao = true;
            }

            return retorno;
        }

        private void CalcularDadosRetorno(dynamic listaProdutos, ref int quantidadeCaixas, ref int sku, ref int quantidadeProdutos)
        {
            foreach (var produto in listaProdutos)
            {
                if (!produto.Removido)
                {
                    if ((int)produto.Quantidade > 0)
                    {
                        sku++;
                        quantidadeProdutos += (int)produto.Quantidade;
                    }
                    quantidadeCaixas += (int)produto.QuantidadeCaixas;
                }
            }
        }

        private dynamic ObterAgendamentoColeta(Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta, List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> listaCargaJanelaDescarregamento, List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> listaCargaJanelaCarregamento, List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido> listaAgendamentoPedido, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargaPedido)
        {
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento = (from o in listaCargaJanelaDescarregamento where o.Carga != null && o.Carga.Codigo == (agendamentoColeta.Carga?.Codigo ?? 0) select o)?.FirstOrDefault() ?? null;
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = (from o in listaCargaJanelaCarregamento where o.Carga != null && o.Carga.Codigo == (agendamentoColeta.Carga?.Codigo ?? 0) select o)?.FirstOrDefault() ?? null;

            return new
            {
                agendamentoColeta.Codigo,
                Carga = agendamentoColeta.Carga?.CodigoCargaEmbarcador,
                PedidoEmbarcador = ObterDescricaoPedidos(agendamentoColeta, listaCargaPedido, listaAgendamentoPedido),
                DataAgendamento = agendamentoColeta.DataAgendamento?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataColeta = agendamentoColeta.DataColeta?.ToString("dd/MM/yyyy") ?? "",
                DataEntrega = agendamentoColeta.DataEntrega?.ToString(ConfiguracaoEmbarcador.ControlarAgendamentoSKU ? "dd/MM/yyyy" : "dd/MM/yyyy HH:mm") ?? "",
                DataCriacao = agendamentoColeta.DataCriacao?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataConfirmada = (agendamentoColeta.Situacao == SituacaoAgendamentoColeta.Agendado || agendamentoColeta.Situacao == SituacaoAgendamentoColeta.Finalizado) ? cargaJanelaDescarregamento?.InicioDescarregamento.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                Destinatario = agendamentoColeta.Destinatario?.Descricao ?? "",
                DescricaoFilial = cargaJanelaDescarregamento?.CentroDescarregamento?.Descricao,
                Remetente = agendamentoColeta.Remetente?.Descricao ?? "",
                TipoCarga = agendamentoColeta.TipoCarga?.Descricao ?? "",
                Etapa = agendamentoColeta.DescricaoEtapa,
                Situacao = agendamentoColeta.DescricaoSituacao,
                SituacaoJanela = cargaJanelaCarregamento?.Situacao.ObterDescricao() ?? "",
                SituacaoJanelaDescarregamento = cargaJanelaDescarregamento?.Situacao.ObterDescricao() ?? "",
                EtapaCarga = agendamentoColeta.Carga?.SituacaoCarga.ObterDescricao() ?? "",
                agendamentoColeta.Observacao,
                agendamentoColeta.Senha,
                Recebedor = agendamentoColeta.Recebedor?.Descricao ?? string.Empty
            };
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Logistica.AgendamentoColeta repositorioAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(unitOfWork);
            Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas repModalidadeFornecedorPessoas = new Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento repositorioConfiguracaoJanelaCarregamento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento configuracaoJanelaCarregamento = repositorioConfiguracaoJanelaCarregamento.BuscarPrimeiroRegistro();

            bool compartilharAcessoEntreGrupoPessoas = IsCompartilharAcessoEntreGrupoPessoas();

            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Carga", "Carga", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Ped. Embarcador", "PedidoEmbarcador", 20, Models.Grid.Align.left, false, true);
            if (ConfiguracaoEmbarcador.ControlarAgendamentoSKU)
                grid.AdicionarCabecalho("Data Solicitada", "DataAgendamento", 15, Models.Grid.Align.center, true);
            else if (configuracaoJanelaCarregamento.SugerirDataEntregaAgendamentoColeta && TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
            {
                grid.AdicionarCabecalho("Data Entrega", "DataEntrega", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Data Criação", "DataCriacao", 15, Models.Grid.Align.center, true);
            }
            else
                grid.AdicionarCabecalho("Data Coleta", "DataColeta", 15, Models.Grid.Align.center, true);

            grid.AdicionarCabecalho("Data Agendada", "DataConfirmada", 15, Models.Grid.Align.center, false, ConfiguracaoEmbarcador.ControlarAgendamentoSKU);
            if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor || compartilharAcessoEntreGrupoPessoas)
                grid.AdicionarCabecalho("Remetente", "Remetente", 35, Models.Grid.Align.left, true);

            string descricaoColunaDestinatario = configuracaoJanelaCarregamento.ExibirOpcaoMultiModalAgendamentoColeta ? "Terminal de Entrega" : "Destinatário";
            grid.AdicionarCabecalho(descricaoColunaDestinatario, "Destinatario", 35, Models.Grid.Align.left, true, !ConfiguracaoEmbarcador.ControlarAgendamentoSKU);

            grid.AdicionarCabecalho("Descrição da Filial", "DescricaoFilial", 35, Models.Grid.Align.left, true, ConfiguracaoEmbarcador.ControlarAgendamentoSKU);
            grid.AdicionarCabecalho("Tipo de Carga", "TipoCarga", 20, Models.Grid.Align.left, true, !configuracaoJanelaCarregamento.ExibirOpcaoMultiModalAgendamentoColeta);
            grid.AdicionarCabecalho("Etapa Agendamento", "Etapa", 20, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Etapa Carga", "EtapaCarga", 20, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Situação Agendamento", "Situacao", 20, Models.Grid.Align.left, true, ConfiguracaoEmbarcador.ControlarAgendamentoSKU);
            grid.AdicionarCabecalho("Situação Janela de Carregamento", "SituacaoJanela", 20, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Situação Janela de Descarregamento", "SituacaoJanelaDescarregamento", 20, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Observação", "Observacao", 20, Models.Grid.Align.left, false, ConfiguracaoEmbarcador.ControlarAgendamentoSKU);
            grid.AdicionarCabecalho("Senha", "Senha", 20, Models.Grid.Align.left, false, ConfiguracaoEmbarcador.ControlarAgendamentoSKU);
            grid.AdicionarCabecalho("Recebedor", "Recebedor", 20, Models.Grid.Align.left, false, true);

            return grid;
        }

        private dynamic ExecutarPesquisa(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAgendamentoColeta filtrosPesquisa, ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.AgendamentoColeta repositorioAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repositorioCargaJanelaDescarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento(unitOfWork);
            Repositorio.Embarcador.Logistica.AgendamentoColetaPedido repositorioAgendamentoColetaPedido = new Repositorio.Embarcador.Logistica.AgendamentoColetaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta servicoCargaJanelaCarregamentoConsulta = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta(unitOfWork);
            totalRegistros = repositorioAgendamentoColeta.ContarConsulta(filtrosPesquisa);
            List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta> listaAgendamentos = null;
            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> listaCargaJanelaDescarregamento = null;
            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> listaCargaJanelaCarregamento = null;
            List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido> listaAgendamentoPedido = null;
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargaPedido = null;

            if (totalRegistros > 0)
            {
                listaAgendamentos = repositorioAgendamentoColeta.Consultar(filtrosPesquisa, propOrdenar, dirOrdena, inicio, limite);
                List<int> codigosCargas = listaAgendamentos.Where(o => o.Carga != null).Select(o => o.Carga.Codigo).ToList();
                listaCargaJanelaDescarregamento = repositorioCargaJanelaDescarregamento.BuscarPorCarga(codigosCargas);
                listaCargaJanelaCarregamento = servicoCargaJanelaCarregamentoConsulta.ObterCargasJanelaCarregamentoPorCargas(codigosCargas);
                listaAgendamentoPedido = ConfiguracaoEmbarcador.ControlarAgendamentoSKU ? repositorioAgendamentoColetaPedido.BuscarPorCodigosAgendamentoColeta(listaAgendamentos.Select(x => x.Codigo).ToList()) : new List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido>();
                listaCargaPedido = !ConfiguracaoEmbarcador.ControlarAgendamentoSKU ? repositorioCargaPedido.BuscarPorCargasSemFetch(listaAgendamentos.Where(obj => obj.Carga != null).Select(x => x.Carga.Codigo).ToList()) : new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            }
            else
            {
                listaAgendamentos = new List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta>();
                listaCargaJanelaDescarregamento = new List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento>();
                listaCargaJanelaCarregamento = new List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>();
                listaAgendamentoPedido = new List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido>();
                listaCargaPedido = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            }

            var retorno = (
                from agendamentoColeta in listaAgendamentos
                select ObterAgendamentoColeta(agendamentoColeta, listaCargaJanelaDescarregamento, listaCargaJanelaCarregamento, listaAgendamentoPedido, listaCargaPedido)
            );

            return retorno.ToList();
        }

        private void EnviarEmailAgendamentoColetaAdicionado(Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova);

            if (agendamentoColeta.Transportador == null || agendamentoColeta.TipoCarga == null)
                return;

            Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repositorioTipoOperacao.BuscarTipoOperacaoPorTipoDeCarga(agendamentoColeta.TipoCarga.Codigo);

            if (tipoOperacao == null || string.IsNullOrWhiteSpace(tipoOperacao.EmailAgendamentoColeta))
                return;

            Servicos.Embarcador.Logistica.AgendamentoColeta servicoAgendamentoColeta = new Servicos.Embarcador.Logistica.AgendamentoColeta(unitOfWork);
            servicoAgendamentoColeta.EnviarEmailAgendamentoColeta(agendamentoColeta, tipoOperacao.EmailAgendamentoColeta);
        }

        private string ObterDescricaoPedidos(Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamento, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargasPedido, List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido> listaPedidosAgendamento)
        {
            if (ConfiguracaoEmbarcador.ControlarAgendamentoSKU)
                return string.Join(", ", listaPedidosAgendamento.Where(obj => obj.AgendamentoColeta.Codigo == agendamento.Codigo).Select(obj => obj.Pedido.NumeroPedidoEmbarcador));

            if (agendamento.Carga == null && agendamento.Pedido != null)
                return agendamento.Pedido.NumeroPedidoEmbarcador;
            else if (agendamento.Carga == null && agendamento.Pedido == null)
                return string.Empty;

            return string.Join(", ", listaCargasPedido.Where(obj => obj.Carga != null && obj.Carga.Codigo == (agendamento?.Carga?.Codigo ?? null))?.Select(obj => obj.Pedido.NumeroPedidoEmbarcador)) ?? "";
        }

        private Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAgendamentoColeta ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas repModalidadeFornecedorPessoas = new Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento repositorioConfiguracaoJanelaCarregamento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento configuracaoJanelaCarregamento = repositorioConfiguracaoJanelaCarregamento.BuscarPrimeiroRegistro();

            bool compartilharAcessoEntreGrupoPessoas = IsCompartilharAcessoEntreGrupoPessoas();
            double remetente = Request.GetDoubleParam("Remetente");
            int grupoPessoa = Usuario.ClienteFornecedor?.GrupoPessoas?.Codigo ?? 0;

            Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAgendamentoColeta filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAgendamentoColeta()
            {
                TipoCarga = Request.GetIntParam("TipoCarga"),
                Destinatario = Request.GetDoubleParam("Destinatario"),
                Recebedor = Request.GetDoubleParam("Recebedor"),
                DataColeta = Request.GetNullableDateTimeParam("DataColeta"),
                DataEntrega = Request.GetNullableDateTimeParam("DataEntrega"),
                DataCriacao = Request.GetNullableDateTimeParam("DataCriacao"),
                DataAgendamento = Request.GetNullableDateTimeParam("DataAgendamento"),
                Situacao = Request.GetNullableEnumParam<SituacaoCargaJanelaCarregamento>("Situacao"),
                Etapa = Request.GetNullableEnumParam<SituacaoCarga>("Etapa"),
                Carga = Request.GetStringParam("Carga"),
                SituacaoJanelaDescarregamento = Request.GetNullableEnumParam<SituacaoCargaJanelaDescarregamento>("SituacaoJanelaDescarregamento"),
                Senha = Request.GetStringParam("Senha"),
                Pedido = Request.GetStringParam("Pedido"),
                TipoOperacoes = Request.GetListParam<int>("TipoOperacao"),
                PedidoEmbarcador = Request.GetStringParam("PedidoEmbarcador"),
                OcultarDescargaCancelada = Request.GetBoolParam("OcultarDescargaCancelada")
            };

            if (!ConfiguracaoEmbarcador.ControlarAgendamentoSKU)
                filtrosPesquisa.CodigoRemetente = remetente;

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
            {
                if (compartilharAcessoEntreGrupoPessoas)
                {
                    filtrosPesquisa.CodigoGrupoPessoas = remetente == 0 ? grupoPessoa : 0;
                    filtrosPesquisa.CodigoRemetente = filtrosPesquisa.CodigoGrupoPessoas > 0 ? 0 : this.Usuario.ClienteFornecedor?.CPF_CNPJ ?? 0;
                }
                else
                    filtrosPesquisa.CodigoRemetente = Usuario.ClienteFornecedor?.CPF_CNPJ ?? 0d;

                Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoas modalidadeFornecedor = Usuario.ClienteFornecedor != null ? repModalidadeFornecedorPessoas.BuscarPorCliente(Usuario.ClienteFornecedor.CPF_CNPJ) : null;
                filtrosPesquisa.TipoCargas = modalidadeFornecedor?.TipoCargas.Select(o => o.Codigo).ToList();
                filtrosPesquisa.TipoOperacoes = modalidadeFornecedor?.TipoOperacoes.Select(o => o.Codigo).ToList();
                filtrosPesquisa.ModelosVeiculares = modalidadeFornecedor?.ModelosVeicular.Select(o => o.Codigo).ToList();
                filtrosPesquisa.Transportadores = !configuracaoJanelaCarregamento.ExibirOpcaoMultiModalAgendamentoColeta ? modalidadeFornecedor?.Transportadores.Select(o => o.Codigo).ToList() : null;
            }
            else if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
            {
                if (Empresa.Matriz.Any())
                    filtrosPesquisa.CodigoEmpresaLogada = Empresa.Matriz.FirstOrDefault().Codigo;
                else
                    filtrosPesquisa.CodigoEmpresaLogada = Empresa.Codigo;
            }

            return filtrosPesquisa;
        }

        private void PreencherEntidade(Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento repositorioConfiguracaoJanelaCarregamento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento(unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repositorioModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
            Repositorio.Embarcador.Logistica.CentroDistribuicao repCentroDistribuicao = new Repositorio.Embarcador.Logistica.CentroDistribuicao(unitOfWork);
            Repositorio.Embarcador.Logistica.AgendamentoColeta repAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoDeCarga repositorioTipoCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
            Repositorio.Embarcador.Pedidos.Porto repPorto = new Repositorio.Embarcador.Pedidos.Porto(unitOfWork);
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
            Repositorio.UnidadeDeMedida repUnidadeDeMedida = new Repositorio.UnidadeDeMedida(unitOfWork);
            Repositorio.Embarcador.Cargas.Categoria repositorioCategoria = new Repositorio.Embarcador.Cargas.Categoria(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento configuracaoJanelaCarregamento = repositorioConfiguracaoJanelaCarregamento.BuscarPrimeiroRegistro();
            Dominio.Entidades.Cliente destinatario;

            dynamic pedidos = JsonConvert.DeserializeObject<dynamic>(Request.Params("Pedidos"));
            dynamic anexos = JsonConvert.DeserializeObject<dynamic>(Request.Params("Anexo"));

            if (!ConfiguracaoEmbarcador.ControlarAgendamentoSKU)
            {
                destinatario = repositorioCliente.BuscarPorCPFCNPJ(Request.GetDoubleParam("Destinatario"));
                if (Request.GetDoubleParam("Recebedor") > 0)
                    agendamentoColeta.Recebedor = repositorioCliente.BuscarPorCPFCNPJ(Request.GetDoubleParam("Recebedor"));
                else
                    agendamentoColeta.Recebedor = null;
            }
            else
            {
                destinatario = repositorioCliente.BuscarPorCPFCNPJ((double)pedidos[0].CodigoDestinatario);
            }

            agendamentoColeta.CargaPerigosa = Request.GetBoolParam("CargaPerigosa");
            agendamentoColeta.DataAgendamento = ConfiguracaoEmbarcador.ControlarAgendamentoSKU ? Request.GetNullableDateTimeParam("DataAgendamento") : DateTime.Now;

            if (agendamentoColeta.CargaPerigosa && (anexos == null || anexos.Count <= 0))
                throw new ControllerException("A carga está marcada como Carga Perigosa, é obrigatório inserir um anexo");

            if (agendamentoColeta.DataAgendamento.HasValue && agendamentoColeta.DataAgendamento.Value.Date < DateTime.Now.Date)
                throw new ControllerException("A data agendada não pode ser menor que a data atual.");

            agendamentoColeta.DataEntrega = Request.GetNullableDateTimeParam("DataEntrega");
            agendamentoColeta.DataColeta = Request.GetNullableDateTimeParam("DataColeta");
            agendamentoColeta.Volumes = Request.GetIntParam("Volumes");
            agendamentoColeta.Peso = Request.GetDecimalParam("Peso");
            agendamentoColeta.Observacao = Request.GetStringParam("Observacao");
            agendamentoColeta.Remetente = ObterRemetente(unitOfWork);
            agendamentoColeta.EmailSolicitante = Request.GetStringParam("EmailSolicitante");
            agendamentoColeta.UnidadeMedida = Request.GetStringParam("UnidadeMedida");

            int codigoFilial = Request.GetIntParam("Filial");

            agendamentoColeta.Filial = codigoFilial > 0 ? repositorioFilial.BuscarPorCodigo(codigoFilial) : null;

            if (Usuario?.ClienteFornecedor?.GrupoPessoas?.UtilizarParametrizacaoDeHorariosNoAgendamento ?? false)
            {
                TimeSpan? horarioInicioFaixa = Request.GetNullableTimeParam("HorarioInicioFaixa");
                TimeSpan? horarioLimiteFaixa = Request.GetNullableTimeParam("HorarioLimiteFaixa");

                if (horarioInicioFaixa.HasValue)
                    agendamentoColeta.HoraInicioFaixa = horarioInicioFaixa.Value;

                if (horarioLimiteFaixa.HasValue)
                    agendamentoColeta.HoraLimiteFaixa = horarioLimiteFaixa.Value;

                if (horarioInicioFaixa.HasValue && horarioLimiteFaixa.HasValue)
                {
                    if (horarioInicioFaixa.Value.TotalMinutes > horarioLimiteFaixa.Value.TotalMinutes || horarioInicioFaixa.Value.TotalMinutes == horarioLimiteFaixa.Value.TotalMinutes)
                        throw new ControllerException("A hora de inicio da faixa deve ser menor que a hora limite.");
                }
            }

            int codigoModeloVeicular = Request.GetIntParam("ModeloVeicular");
            int codigoPortoOrigem = Request.GetIntParam("PortoOrigem");
            int codigoPortoDestino = Request.GetIntParam("PortoDestino");
            int codigoTipoCarga = Request.GetIntParam("TipoCarga");
            int codigoTipoOperacao = Request.GetIntParam("TipoOperacao");
            int codigoTransportador = Request.GetIntParam("Transportador");
            int codigoCDDestino = Request.GetIntParam("CDDestino");
            int codigoCategoria = Request.GetIntParam("Categoria");

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
            {
                codigoTransportador = Empresa.Codigo;

                if (codigoTransportador == 0)
                    throw new ControllerException("Não foi possível encontrar o transportador logado.");
            }

            agendamentoColeta.Destinatario = destinatario;
            agendamentoColeta.ModeloVeicular = (codigoModeloVeicular > 0) ? repositorioModeloVeicularCarga.BuscarPorCodigo(codigoModeloVeicular) : null;
            agendamentoColeta.Transportador = codigoTransportador > 0 ? repositorioEmpresa.BuscarPorCodigo(codigoTransportador) : null;
            agendamentoColeta.CodigoControle = 1;
            agendamentoColeta.AgendamentoPai = false;
            agendamentoColeta.DataCriacao = Request.GetNullableDateTimeParam("DataCriacao");
            agendamentoColeta.PortoOrigem = (codigoPortoOrigem > 0) ? repPorto.BuscarPorCodigo(codigoPortoOrigem) : null;
            agendamentoColeta.PortoDestino = (codigoPortoDestino > 0) ? repPorto.BuscarPorCodigo(codigoPortoDestino) : null;
            agendamentoColeta.Reboque = Request.GetStringParam("Reboque");
            agendamentoColeta.Placa = Request.GetStringParam("Placa");
            agendamentoColeta.Motorista = Request.GetStringParam("Motorista");
            agendamentoColeta.TransportadorManual = Request.GetStringParam("TransportadorManual");
            agendamentoColeta.TipoOperacao = (codigoTipoOperacao > 0) ? repTipoOperacao.BuscarPorCodigo(codigoTipoOperacao) : null;
            agendamentoColeta.CDDestino = (codigoCDDestino > 0) ? repCentroDistribuicao.BuscarPorCodigo(codigoCDDestino) : null;
            agendamentoColeta.Solicitante = this.Usuario;
            agendamentoColeta.Categoria = codigoCategoria > 0 ? repositorioCategoria.BuscarPorCodigo(codigoCategoria, false) : null;

            if ((agendamentoColeta.TipoOperacao == null) && (codigoTipoCarga > 0))
                agendamentoColeta.TipoOperacao = repTipoOperacao.BuscarTipoOperacaoPorTipoDeCarga(codigoTipoCarga);

            if ((TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor) && !ConfiguracaoEmbarcador.ControlarAgendamentoSKU)
                agendamentoColeta.TipoCarga = agendamentoColeta.TipoOperacao?.TipoDeCargaPadraoOperacao;
            else
                agendamentoColeta.TipoCarga = (codigoTipoCarga > 0) ? repositorioTipoCarga.BuscarPorCodigo(codigoTipoCarga) : null;

            agendamentoColeta.ApenasGerarPedido = agendamentoColeta.TipoOperacao?.AgendamentoGeraApenasPedido ?? false;

            if (configuracaoJanelaCarregamento.ExibirOpcaoMultiModalAgendamentoColeta)
            {
                if (agendamentoColeta.Transportador == null)
                    agendamentoColeta.Transportador = repositorioEmpresa.BuscarEmpresaPadraoRetirada();
                if (agendamentoColeta.Transportador != null && !string.IsNullOrEmpty(agendamentoColeta.Placa))
                {
                    agendamentoColeta.VeiculoSelecionado = repVeiculo.BuscarPlaca(agendamentoColeta.Placa);
                    if (agendamentoColeta.VeiculoSelecionado == null)
                    {
                        Dominio.Entidades.Veiculo veiculo = new Dominio.Entidades.Veiculo()
                        {
                            Placa = agendamentoColeta.Placa,
                            Empresa = agendamentoColeta.Transportador,
                            Estado = agendamentoColeta.Transportador.Localidade?.Estado,
                            Ativo = true,
                            TipoVeiculo = "0"
                        };
                        repVeiculo.Inserir(veiculo);
                        agendamentoColeta.VeiculoSelecionado = veiculo;
                    }
                }
                if (agendamentoColeta.Transportador != null && !string.IsNullOrEmpty(agendamentoColeta.Reboque))
                {
                    agendamentoColeta.ReboqueSelecionado = repVeiculo.BuscarPlaca(agendamentoColeta.Reboque);
                    if (agendamentoColeta.ReboqueSelecionado == null)
                    {
                        Dominio.Entidades.Veiculo veiculo = new Dominio.Entidades.Veiculo()
                        {
                            Placa = agendamentoColeta.Reboque,
                            Empresa = agendamentoColeta.Transportador,
                            Estado = agendamentoColeta.Transportador.Localidade?.Estado,
                            Ativo = true,
                            TipoVeiculo = "1"
                        };
                        repVeiculo.Inserir(veiculo);
                        agendamentoColeta.ReboqueSelecionado = veiculo;
                    }
                    if (agendamentoColeta.VeiculoSelecionado != null && agendamentoColeta.ReboqueSelecionado != null)
                    {
                        agendamentoColeta.VeiculoSelecionado.VeiculosVinculados = new List<Dominio.Entidades.Veiculo>();
                        agendamentoColeta.VeiculoSelecionado.VeiculosVinculados.Add(agendamentoColeta.ReboqueSelecionado);
                        repVeiculo.Atualizar(agendamentoColeta.VeiculoSelecionado);
                    }
                }
                if (agendamentoColeta.Transportador != null && !string.IsNullOrEmpty(agendamentoColeta.Motorista))
                {
                    agendamentoColeta.MotoristaSelecionado = repUsuario.BuscarPrimeiroMotoristaPorNome(agendamentoColeta.Motorista);
                    if (agendamentoColeta.MotoristaSelecionado == null)
                    {
                        Random rnd = new Random();
                        string cpf = Utilidades.String.OnlyNumbers(rnd.Next(999999).ToString("D"));
                        cpf = cpf.PadLeft(11, '1');
                        Dominio.Entidades.Usuario mot = new Dominio.Entidades.Usuario()
                        {
                            Nome = agendamentoColeta.Motorista,
                            Empresa = agendamentoColeta.Transportador,
                            ExibirUsuarioAprovacao = false,
                            CPF = cpf,
                            Status = "A",
                            Tipo = "M"
                        };
                        repUsuario.Inserir(mot);
                        agendamentoColeta.MotoristaSelecionado = mot;
                    }
                }
            }

            agendamentoColeta.Sequencia = Servicos.Embarcador.Logistica.AgendamentoColetaSequencial.GetInstance().ObterProximoNumeroSequencial(unitOfWork);
        }

        private string ObterDescricaoReboqueCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (carga?.Veiculo?.IsTipoVeiculoReboque() ?? false)
                return $"{carga.Veiculo.Placa} ({carga.Veiculo.ModeloVeicularCarga?.Descricao})";

            if (carga?.VeiculosVinculados?.Count > 0)
                return $"{carga.VeiculosVinculados.FirstOrDefault().Placa} ({carga.VeiculosVinculados.FirstOrDefault().ModeloVeicularCarga?.Descricao})";

            if (carga?.Veiculo?.VeiculosVinculados?.Count > 0)
                return $"{carga?.Veiculo?.VeiculosVinculados.FirstOrDefault().Placa} ({carga?.Veiculo?.VeiculosVinculados.FirstOrDefault().ModeloVeicularCarga?.Descricao})";

            return string.Empty;
        }

        private Dominio.Entidades.Cliente ObterRemetente(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
            {
                bool compartilharAcessoEntreGrupoPessoas = IsCompartilharAcessoEntreGrupoPessoas();

                if (compartilharAcessoEntreGrupoPessoas)
                {
                    Dominio.Entidades.Cliente remetente = repCliente.BuscarPorCPFCNPJ(Request.GetDoubleParam("Remetente"));
                    return (Usuario.ClienteFornecedor.GrupoPessoas?.Clientes?.Contains(remetente) ?? false) ? remetente : Usuario.ClienteFornecedor;
                }

                return Usuario.ClienteFornecedor;
            }

            return repCliente.BuscarPorCPFCNPJ(Request.GetDoubleParam("Remetente"));
        }

        private Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao PreencherObjetoCargaIntegracao(Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta, Dominio.Entidades.Embarcador.Filiais.Filial filial, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Servicos.WebService.Carga.TipoOperacao serWSTipoOperacao = new Servicos.WebService.Carga.TipoOperacao(unitOfWork.StringConexao);
            Servicos.WebService.Carga.Carga serCarga = new Servicos.WebService.Carga.Carga(unitOfWork);
            Servicos.Embarcador.Carga.Carga servicoCarga = new(unitOfWork);

            string numeroCarga = servicoCarga.ObterNumeroCargaAdicionar(filial);
            string codigoFilialEmbarcador = filial.CodigoFilialEmbarcador;
            int numeroPedidoEmbarcador = ConfiguracaoEmbarcador.UtilizarNumeroPreCargaPorFilial ? repositorioPedido.ObterProximoCodigo(filial) : repositorioPedido.ObterProximoCodigo();

            Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao = new Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao
            {
                Filial = new Dominio.ObjetosDeValor.Embarcador.Filial.Filial() { CodigoIntegracao = codigoFilialEmbarcador },
                NumeroCarga = numeroCarga,
                NumeroPedidoEmbarcador = numeroPedidoEmbarcador.ToString(),
                DataPrevisaoChegadaDestinatario = agendamentoColeta.DataColeta?.ToString("dd/MM/yyyy HH:mm:ss"),
                ModeloVeicular = agendamentoColeta.ModeloVeicular != null ? new Dominio.ObjetosDeValor.Embarcador.Carga.ModeloVeicular() { CodigoIntegracao = agendamentoColeta.ModeloVeicular.CodigoIntegracao } : null,
                Recebedor = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa() { CPFCNPJ = agendamentoColeta.Recebedor?.CPF_CNPJ_SemFormato },
                Destinatario = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa() { CPFCNPJ = agendamentoColeta.Destinatario.CPF_CNPJ_SemFormato },
                Remetente = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa() { CPFCNPJ = agendamentoColeta.Remetente.CPF_CNPJ_SemFormato },
                TipoCargaEmbarcador = agendamentoColeta.TipoCarga != null ? new Dominio.ObjetosDeValor.Embarcador.Carga.TipoCargaEmbarcador() { CodigoIntegracao = agendamentoColeta.TipoCarga.CodigoTipoCargaEmbarcador } : agendamentoColeta.TipoOperacao?.TipoDeCargaPadraoOperacao != null ? new Dominio.ObjetosDeValor.Embarcador.Carga.TipoCargaEmbarcador() { CodigoIntegracao = agendamentoColeta.TipoOperacao.TipoDeCargaPadraoOperacao.CodigoTipoCargaEmbarcador } : null,
                ProdutoPredominante = new Dominio.ObjetosDeValor.Embarcador.Pedido.Produto() { DescricaoProduto = "Diversos", CodigoProduto = "Diversos" },
                DataColeta = agendamentoColeta.DataColeta.HasValue ? $"{agendamentoColeta.DataColeta:dd/MM/yyyy} 00:00:00" : string.Empty,
                DataPrevisaoEntrega = agendamentoColeta.DataEntrega?.ToDateTimeString(true) ?? string.Empty,
                PesoBruto = agendamentoColeta.Peso,
                QuantidadeVolumes = agendamentoColeta.Volumes,
                TransportadoraEmitente = agendamentoColeta.Transportador != null ? new Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa() { CNPJ = agendamentoColeta.Transportador.CNPJ } : null,
                DataInicioCarregamento = agendamentoColeta.DataColeta?.ToDateTimeString(true),
                PortoDestino = serCarga.ConverterObjetoPorto(agendamentoColeta.PortoDestino),
                PortoOrigem = serCarga.ConverterObjetoPorto(agendamentoColeta.PortoOrigem),
                TipoOperacao = serWSTipoOperacao.ConverterObjetoTipoOperacao(agendamentoColeta.TipoOperacao),
                Veiculo = agendamentoColeta.VeiculoSelecionado != null ? (new Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo() { Placa = agendamentoColeta.VeiculoSelecionado.Placa }) : null
            };

            if (agendamentoColeta.MotoristaSelecionado != null)
            {
                cargaIntegracao.Motoristas = new List<Dominio.ObjetosDeValor.Embarcador.Carga.Motorista>();
                cargaIntegracao.Motoristas.Add(new Dominio.ObjetosDeValor.Embarcador.Carga.Motorista() { CPF = agendamentoColeta.MotoristaSelecionado.CPF });
            }

            if (agendamentoColeta.Recebedor == null)
                cargaIntegracao.Recebedor = null;

            return cargaIntegracao;
        }

        private Dominio.Entidades.Embarcador.Cargas.Carga AdicionarCarga(Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
            Dominio.Entidades.Embarcador.Filiais.Filial filial = null;

            if (ConfiguracaoEmbarcador.ControlarAgendamentoSKU)
                filial = repositorioFilial.BuscarPorCNPJ(agendamentoColeta.Destinatario.CPF_CNPJ_SemFormato);
            else
                filial = agendamentoColeta.Filial ?? repositorioFilial.BuscarPorCNPJ(agendamentoColeta.Destinatario.CPF_CNPJ_SemFormato);

            if (filial == null)
                throw new ControllerException($"Não foi possível localizar a filial.");

            if (ConfiguracaoEmbarcador.ControlarAgendamentoSKU)
                return GerarCargaDosPedidos(agendamentoColeta, unitOfWork);
            else
                return GerarPedidoECarga(agendamentoColeta, filial, unitOfWork);
        }

        private Dominio.Entidades.Embarcador.Pedidos.Pedido AdicionarPedido(Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Dominio.Entidades.Embarcador.Filiais.Filial filial = null;

            if (ConfiguracaoEmbarcador.ControlarAgendamentoSKU)
                filial = repositorioFilial.BuscarPorCNPJ(agendamentoColeta.Destinatario.CPF_CNPJ_SemFormato);
            else
                filial = agendamentoColeta.Filial ?? repositorioFilial.BuscarPorCNPJ(agendamentoColeta.Destinatario.CPF_CNPJ_SemFormato);

            if (filial == null)
                throw new ControllerException($"Não foi possível localizar a filial.");

            Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao = PreencherObjetoCargaIntegracao(agendamentoColeta, filial, unitOfWork);
            Servicos.WebService.Carga.Pedido servicoPedidoWS = new Servicos.WebService.Carga.Pedido(unitOfWork);

            int codigoCargaExistente = 0;
            int protocoloPedidoExistente = 0;

            StringBuilder mensagemErro = new StringBuilder();
            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = servicoPedidoWS.CriarPedido(cargaIntegracao, filial, agendamentoColeta.TipoOperacao, ref mensagemErro, TipoServicoMultisoftware, ref protocoloPedidoExistente, ref codigoCargaExistente, buscarCargaPorTransportador: false, ignorarPedidosInseridosManualmente: true, configuracaoTMS: ConfiguracaoEmbarcador);

            if (mensagemErro.Length > 0)
                throw new ControllerException(mensagemErro.ToString());

            pedido.DataInicialColeta = agendamentoColeta.DataColeta;
            pedido.PrevisaoEntrega = agendamentoColeta.DataEntrega;

            repositorioPedido.Atualizar(pedido);

            return pedido;
        }

        private Dominio.Entidades.Embarcador.Cargas.Carga GerarCargaDosPedidos(Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Logistica.AgendamentoColeta repAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(unitOfWork);
            Repositorio.Embarcador.Logistica.AgendamentoColetaPedido repAgendamentoColetaPedido = new Repositorio.Embarcador.Logistica.AgendamentoColetaPedido(unitOfWork);

            List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido> agendamentoPedidos = repAgendamentoColetaPedido.BuscarPedidosPorAgendamentoColeta(agendamentoColeta.Codigo);
            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = (from o in agendamentoPedidos select o.Pedido).ToList();
            int totalVolumes = agendamentoPedidos.Sum(o => o.VolumesEnviar);
            decimal totalValorVolumes = agendamentoPedidos.Sum(o => o.ValorVolumesEnviar);

            string mensagemErroCriarCarga = Servicos.Embarcador.Pedido.Pedido.CriarCarga(out Dominio.Entidades.Embarcador.Cargas.Carga carga, pedidos, unitOfWork, TipoServicoMultisoftware, Cliente, ConfiguracaoEmbarcador, forcarGeracaoCarga: true, cadastroPedido: false, adicionarJanelaDescarregamento: false, gerarAgendamentoColeta: false, origemDoAgendamento: true, agendamentoColeta: agendamentoColeta);

            if (!string.IsNullOrWhiteSpace(mensagemErroCriarCarga))
                throw new ControllerException(mensagemErroCriarCarga);

            foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidos)
                repositorioPedido.Atualizar(pedido);

            //Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarCargaPedidoPorPedido(pedidos.FirstOrDefault().Codigo);
            //Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaPedido.Carga;

            agendamentoColeta.Volumes = totalVolumes;
            agendamentoColeta.ValorTotalVolumes = totalValorVolumes;
            carga.DadosSumarizados.ValorTotalMercadoriaPedidos = agendamentoColeta.ValorTotalVolumes;
            carga.DadosSumarizados.QuantidadeVolumes = agendamentoColeta.Volumes;
            carga.DadosSumarizados.VolumesTotal = agendamentoColeta.Volumes;
            carga.DadosSumarizados.PossuiProdutoPerigoso = agendamentoColeta.CargaPerigosa;
            carga.ModeloVeicularCarga = agendamentoColeta.ModeloVeicular;
            carga.TipoOperacao = agendamentoColeta.TipoOperacao;
            carga.PortoOrigem = agendamentoColeta.PortoOrigem;
            carga.PortoDestino = agendamentoColeta.PortoDestino;

            repCarga.Atualizar(carga);
            repAgendamentoColeta.Atualizar(agendamentoColeta);

            return carga;
        }

        private Dominio.Entidades.Embarcador.Cargas.Carga GerarPedidoECarga(Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta, Dominio.Entidades.Embarcador.Filiais.Filial filial, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao = PreencherObjetoCargaIntegracao(agendamentoColeta, filial, unitOfWork);

            Servicos.WebService.Carga.Pedido servicoPedidoWS = new Servicos.WebService.Carga.Pedido(unitOfWork);
            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            Servicos.WebService.Carga.Carga servicoCargaWS = new Servicos.WebService.Carga.Carga(unitOfWork);
            Servicos.WebService.Carga.ProdutosPedido servicoProdutoPedido = new Servicos.WebService.Carga.ProdutosPedido(unitOfWork);
            Servicos.Embarcador.Carga.RateioFrete servicoRateioFrete = new Servicos.Embarcador.Carga.RateioFrete(unitOfWork);

            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

            int codigoCargaExistente = 0;
            int protocoloPedidoExistente = 0;

            StringBuilder mensagemErro = new StringBuilder();
            Dominio.Entidades.Embarcador.Cargas.Carga carga = null;
            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = servicoPedidoWS.CriarPedido(cargaIntegracao, filial, agendamentoColeta.TipoOperacao, ref mensagemErro, TipoServicoMultisoftware, ref protocoloPedidoExistente, ref codigoCargaExistente, buscarCargaPorTransportador: false, ignorarPedidosInseridosManualmente: true, configuracaoTMS: ConfiguracaoEmbarcador);

            if (mensagemErro.Length > 0)
                throw new ControllerException(mensagemErro.ToString());

            servicoProdutoPedido.AdicionarProdutosPedido(pedido, ConfiguracaoEmbarcador, cargaIntegracao, ref mensagemErro, unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = servicoCargaWS.CriarCarga(pedido, cargaIntegracao, ref protocoloPedidoExistente, ref mensagemErro, ref codigoCargaExistente, unitOfWork, TipoServicoMultisoftware, false, false, Auditado, ConfiguracaoEmbarcador, null, "", filial, agendamentoColeta.TipoOperacao);

            if (cargaPedido != null)
            {
                servicoRateioFrete.GerarComponenteICMS(cargaPedido, false, unitOfWork);

                if (cargaPedido.CargaPedidoFilialEmissora)
                    servicoRateioFrete.GerarComponenteICMS(cargaPedido, true, unitOfWork);

                servicoRateioFrete.GerarComponenteISS(cargaPedido, false, unitOfWork);
                servicoCargaWS.AdicionarProdutosCarga(cargaPedido, cargaIntegracao, ref mensagemErro, unitOfWork, false);
                carga = cargaPedido.Carga;
            }

            if (mensagemErro.Length > 0)
                throw new ControllerException(mensagemErro.ToString());

            servicoCarga.FecharCarga(carga, unitOfWork, TipoServicoMultisoftware, this.Cliente, recriarRotas: false, adicionarJanelaDescarregamento: false, adicionarJanelaCarregamento: true, validarDados: false, gerarAgendamentoColeta: false);

            carga.DadosSumarizados.QuantidadeVolumes = agendamentoColeta.Volumes;
            carga.DadosSumarizados.ValorTotalMercadoriaPedidos = agendamentoColeta.ValorTotalVolumes;
            carga.CargaFechada = true;
            carga.NumeroSequenciaCarga = cargaIntegracao.NumeroCarga.ToInt();
            carga.ModeloVeicularCarga = agendamentoColeta.ModeloVeicular;

            Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento repositorioConfiguracaoJanelaCarregamento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento configuracaoJanelaCarregamento = repositorioConfiguracaoJanelaCarregamento.BuscarPrimeiroRegistro();

            bool avancarEtapaCarga = (agendamentoColeta.TipoOperacao?.ConfiguracaoAgendamentoColetaEntrega?.ObrigarInformarCTePortalFornecedor ?? false) || (configuracaoJanelaCarregamento.ExibirOpcaoMultiModalAgendamentoColeta && agendamentoColeta.EtapaAgendamentoColeta == EtapaAgendamentoColeta.NFe && agendamentoColeta.VeiculoSelecionado != null && agendamentoColeta.MotoristaSelecionado != null && agendamentoColeta.Transportador != null && carga.ModeloVeicularCarga != null);

            if (avancarEtapaCarga && carga.ExigeNotaFiscalParaCalcularFrete && (carga.SituacaoCarga == SituacaoCarga.AgTransportador || carga.SituacaoCarga == SituacaoCarga.Nova))
                carga.SituacaoCarga = SituacaoCarga.AgNFe;

            repositorioCarga.Atualizar(carga);
            repositorioPedido.Atualizar(pedido);

            return carga;
        }

        private void SalvarPedidosAgendamento(Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga, Dominio.Entidades.Embarcador.Cargas.Carga carga = null)
        {
            Repositorio.Embarcador.Logistica.AgendamentoColetaPedido repositorioAgendamentoColetaPedido = new Repositorio.Embarcador.Logistica.AgendamentoColetaPedido(unitOfWork);
            Repositorio.Embarcador.Logistica.AgendamentoColetaPedidoProduto repositorioAgendamentoColetaPedidoProduto = new Repositorio.Embarcador.Logistica.AgendamentoColetaPedidoProduto(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.AlteracaoPedidoProdutoAgendamento repositorioAlteracaoPedidoProdutoAgendado = new Repositorio.Embarcador.Pedidos.AlteracaoPedidoProdutoAgendamento(unitOfWork);

            dynamic objPedidos = JsonConvert.DeserializeObject<dynamic>(Request.Params("Pedidos"));

            List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido> agendamentoColetaPedidos = repositorioAgendamentoColetaPedido.BuscarPorAgendamentoColeta(agendamentoColeta.Codigo);
            List<int> codigosPedidos = new List<int>();
            List<int> codigos = new List<int>();

            foreach (dynamic objPedido in objPedidos)
                codigos.Add((int)objPedido.Codigo);

            List<Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedidoProdutoAgendamento> listAlteracaoPedidoProdutoAgendamento = repositorioAlteracaoPedidoProdutoAgendado.BuscarPorAlteracaoPedidosNaoVinculado(codigos);
            List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedidoProduto> listaAgendamentoColetaPedidoProdutos = repositorioAgendamentoColetaPedidoProduto.BuscarPorListaCodigoPedido(codigos);

            foreach (dynamic pedido in objPedidos)
            {
                bool atualizar = true;

                int codigoPedido = ((string)pedido.Codigo).ToInt();
                int volumesEnviar = ((string)pedido.VolumesEnviar).ToInt();
                var listPedidoProdutoAlteradoAgendamento = listAlteracaoPedidoProdutoAgendamento.FindAll(x => x.PedidoProduto.Pedido.Codigo == codigoPedido);
                Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido agendamentoColetaPedido = (from o in agendamentoColetaPedidos where o.Pedido.Codigo == codigoPedido select o).FirstOrDefault();

                codigosPedidos.Add(codigoPedido);

                if (agendamentoColetaPedido == null)
                {
                    atualizar = false;
                    agendamentoColetaPedido = new Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido();
                }

                agendamentoColetaPedido.AgendamentoColeta = agendamentoColeta;
                agendamentoColetaPedido.Pedido = repositorioPedido.BuscarPorCodigo(codigoPedido);
                agendamentoColetaPedido.VolumesEnviar = volumesEnviar;
                agendamentoColetaPedido.ValorVolumesEnviar = (agendamentoColetaPedido.Pedido.ValorTotalNotasFiscais / Math.Max(agendamentoColetaPedido.Pedido.QtVolumes, 1)) * volumesEnviar;
                agendamentoColetaPedido.SKU = (int)pedido.SKU;

                List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedidoProduto> listAgendamentoColetaPedidoProduto = listaAgendamentoColetaPedidoProdutos.Where(x => x.AgendamentoColetaPedido.Pedido.Codigo == agendamentoColetaPedido.Pedido.Codigo).ToList();
                Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedidoProduto agendamentoColetaPedidoProduto = null;

                if (agendamentoColetaPedido.SKU <= 0)
                    throw new ControllerException("A quantidade de itens de cada pedido selecionado deve ser maior que 0.");

                if (agendamentoColetaPedido.VolumesEnviar <= 0)
                    throw new ControllerException("A quantidade de volumes de cada pedido selecionado deve ser maior que 0.");

                if (agendamentoColetaPedido.Pedido.DataInicioJanelaDescarga.HasValue && agendamentoColeta.DataAgendamento.HasValue && agendamentoColetaPedido.Pedido.DataInicioJanelaDescarga.Value.Date > agendamentoColeta.DataAgendamento.Value.Date)
                    throw new ControllerException("A data de agendamento não pode ser inferior a data da janela do pedido (" + agendamentoColetaPedido.Pedido.DataInicioJanelaDescarga.Value.ToString("dd/MM/yyyy") + ")");

                if (listPedidoProdutoAlteradoAgendamento.Count == 0)
                {
                    foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoProduto pedidoProduto in agendamentoColetaPedido.Pedido.Produtos)
                    {
                        int saldoProdutoAgendamentoColeta = (int)pedidoProduto.Quantidade;

                        List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedidoProduto> listProdutosAgendadosDoPedido = listAgendamentoColetaPedidoProduto.FindAll(x => x.PedidoProduto.Codigo == pedidoProduto.Codigo && ValidaCanceladaDesagendada(x.AgendamentoColetaPedido.AgendamentoColeta, unitOfWork));

                        if (listProdutosAgendadosDoPedido != null)
                        {
                            foreach (Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedidoProduto produtoAgendado in listProdutosAgendadosDoPedido)
                                saldoProdutoAgendamentoColeta -= produtoAgendado.Quantidade;
                        }

                        if (saldoProdutoAgendamentoColeta > 0)
                        {
                            Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedidoProdutoAgendamento pedidoProdutoAgendamento = new Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedidoProdutoAgendamento();

                            pedidoProdutoAgendamento.PedidoProduto = pedidoProduto;
                            pedidoProdutoAgendamento.NovaQuantidadeProduto = saldoProdutoAgendamentoColeta;

                            repositorioAlteracaoPedidoProdutoAgendado.Inserir(pedidoProdutoAgendamento);
                            listPedidoProdutoAlteradoAgendamento.Add(pedidoProdutoAgendamento);
                        }

                        if (saldoProdutoAgendamentoColeta < 0)
                            throw new ControllerException("A quantidade de volumes alterados não pode ser maior que o saldo de volume do pedido.");
                    }
                }

                if (listPedidoProdutoAlteradoAgendamento.Count > 0)
                {
                    int volumePedidoAgendamento = (int)Math.Ceiling(listPedidoProdutoAlteradoAgendamento.Sum(x => x.NovaQuantidadeProduto / Math.Max(x.PedidoProduto.Produto.QuantidadeCaixa, 1)));

                    if (volumePedidoAgendamento > agendamentoColetaPedido.Pedido.SaldoVolumesRestante && !atualizar)
                        throw new ControllerException("A quantidade de volumes alterados não pode ser maior que o saldo de volume do pedido.");

                    agendamentoColetaPedido.VolumesEnviar = volumePedidoAgendamento;
                    agendamentoColetaPedido.ValorVolumesEnviar = (agendamentoColetaPedido.Pedido.ValorTotalNotasFiscais / Math.Max(agendamentoColetaPedido.Pedido.QtVolumes, 1)) * volumePedidoAgendamento;
                }

                foreach (var pedidoProdutoAlteradoAgendamento in listPedidoProdutoAlteradoAgendamento)
                {
                    agendamentoColetaPedidoProduto = listAgendamentoColetaPedidoProduto.Find(x => x.PedidoProduto.Codigo == pedidoProdutoAlteradoAgendamento.PedidoProduto.Codigo && x.AgendamentoColetaPedido.AgendamentoColeta.Codigo == agendamentoColetaPedido.Codigo);
                    bool inserirAgendamentoColetaPedidoProduto = agendamentoColetaPedidoProduto == null;

                    if (inserirAgendamentoColetaPedidoProduto)
                        agendamentoColetaPedidoProduto = new Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedidoProduto();

                    agendamentoColetaPedidoProduto.AgendamentoColetaPedido = agendamentoColetaPedido;
                    agendamentoColetaPedidoProduto.PedidoProduto = pedidoProdutoAlteradoAgendamento.PedidoProduto;
                    agendamentoColetaPedidoProduto.Quantidade = (int)pedidoProdutoAlteradoAgendamento.NovaQuantidadeProduto;
                    agendamentoColetaPedidoProduto.QuantidadeDeCaixas = (int)Math.Ceiling((decimal)agendamentoColetaPedidoProduto.Quantidade / Math.Max(agendamentoColetaPedidoProduto.PedidoProduto.Produto.QuantidadeCaixa, 1));

                    if (inserirAgendamentoColetaPedidoProduto)
                        listAgendamentoColetaPedidoProduto.Add(agendamentoColetaPedidoProduto);

                    pedidoProdutoAlteradoAgendamento.AgendamentoColeta = agendamentoColeta;
                    repositorioAlteracaoPedidoProdutoAgendado.Atualizar(pedidoProdutoAlteradoAgendamento);
                }

                repositorioPedido.Atualizar(agendamentoColetaPedido.Pedido);

                if (agendamentoColetaPedido.Codigo > 0)
                    repositorioAgendamentoColetaPedido.Atualizar(agendamentoColetaPedido);
                else
                    repositorioAgendamentoColetaPedido.Inserir(agendamentoColetaPedido);

                foreach (var agendamentoProduto in listAgendamentoColetaPedidoProduto)
                {
                    if (agendamentoProduto.Codigo == 0)
                        repositorioAgendamentoColetaPedidoProduto.Inserir(agendamentoProduto);
                    else
                        repositorioAgendamentoColetaPedidoProduto.Atualizar(agendamentoProduto);
                }
            }

            List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido> agendamentoColetaPedidoExcluidos = (from o in agendamentoColetaPedidos where !codigosPedidos.Contains(o.Pedido.Codigo) select o).ToList();

            foreach (Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido agendamentoColetaPedido in agendamentoColetaPedidoExcluidos)
            {
                var pedidoRemovido = agendamentoColetaPedido;

                repositorioPedido.Atualizar(agendamentoColetaPedido.Pedido);
                repositorioAgendamentoColetaPedido.Deletar(agendamentoColetaPedido);

                EnviarEmailCancelamentoPedido(pedidoRemovido, unitOfWork, cliente);

                if (carga != null)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCargaEPedido(carga.Codigo, agendamentoColetaPedido.Pedido.Codigo);

                    Servicos.Embarcador.Carga.CargaPedido.RemoverPedidoCarga(carga, cargaPedido, ConfiguracaoEmbarcador, TipoServicoMultisoftware, unitOfWork, configuracaoGeralCarga);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, $"Removeu o pedido {cargaPedido.Pedido.NumeroPedidoEmbarcador}", unitOfWork);
                }
            }
        }

        private bool ValidaCanceladaDesagendada(Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta, Repositorio.UnitOfWork unitOfWork)
        {
            if (agendamentoColeta == null) return true;

            Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repositorioCargaJanelaDescarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento(unitOfWork);

            if (repositorioCargaJanelaDescarregamento.ExisteJanelaDescarregamentoCanceladaPorCarga(agendamentoColeta.Carga.Codigo))
                return false;

            if (agendamentoColeta.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada)
                return false;

            if (agendamentoColeta.Carga?.CargaCancelamento != null)
                return false;

            if (agendamentoColeta.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAgendamentoColeta.NaoComparecimento || agendamentoColeta.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAgendamentoColeta.NaoComparecimentoConfirmadoPeloFornecedor || agendamentoColeta.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAgendamentoColeta.CargaDevolvida)
                return false;

            return true;
        }

        private void AtualizarCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Servicos.Embarcador.Carga.CargaMotorista servicoCargaMotorista = new Servicos.Embarcador.Carga.CargaMotorista(unitOfWork);

            carga.Veiculo = agendamentoColeta.VeiculoSelecionado;

            if (carga.VeiculosVinculados == null)
                carga.VeiculosVinculados = new List<Dominio.Entidades.Veiculo>();

            carga.VeiculosVinculados.Clear();
            carga.VeiculosVinculados.Add(agendamentoColeta.ReboqueSelecionado);

            servicoCargaMotorista.AtualizarMotoristas(carga, new List<Dominio.Entidades.Usuario> { agendamentoColeta.MotoristaSelecionado });

            bool avancarEtapaCarga = agendamentoColeta.VeiculoSelecionado != null && agendamentoColeta.MotoristaSelecionado != null && agendamentoColeta.Transportador != null && carga.ModeloVeicularCarga != null;

            if (avancarEtapaCarga && carga.ExigeNotaFiscalParaCalcularFrete && (carga.SituacaoCarga == SituacaoCarga.AgTransportador || carga.SituacaoCarga == SituacaoCarga.Nova))
                carga.SituacaoCarga = SituacaoCarga.AgNFe;

            repositorioCarga.Atualizar(carga);
        }

        private bool IsObrigatorioInformarCTes(Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta, Repositorio.UnitOfWork unitOfWork)
        {
            if (agendamentoColeta.ApenasGerarPedido)
                return false;

            if (!(agendamentoColeta.Carga?.TipoOperacao?.FretePorContadoCliente ?? false))
                return false;

            if (!(agendamentoColeta.Carga?.TipoOperacao?.ConfiguracaoAgendamentoColetaEntrega?.ObrigarInformarCTePortalFornecedor ?? false))
                return false;

            Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargasCTe = repositorioCargaCTe.BuscarPorCarga(agendamentoColeta.Carga.Codigo);
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidosXmlNotaFiscal = repositorioPedidoXMLNotaFiscal.BuscarPorCarga(agendamentoColeta.Carga.Codigo);

            return !(cargasCTe.Count == pedidosXmlNotaFiscal.Count);
        }

        private void SalvarDadosTransporte(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            bool dadosTransporteInformados = (carga.Veiculo != null && carga.Motoristas.Count > 0 && carga.Empresa != null);

            if (!dadosTransporteInformados)
                return;

            if (PermiteSalvarDadosTransporte(carga))
            {
                string mensagemErro = string.Empty;
                Servicos.Embarcador.Carga.Carga svcCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Carga.CargaDadosTransporte dadosTransporte = new Dominio.ObjetosDeValor.Embarcador.Carga.CargaDadosTransporte()
                {
                    Carga = carga,
                    CodigoEmpresa = carga.Empresa?.Codigo ?? 0,
                    CodigoModeloVeicular = carga.ModeloVeicularCarga?.Codigo ?? 0,
                    CodigoReboque = carga.VeiculosVinculados?.Count > 0 ? carga.VeiculosVinculados.FirstOrDefault().Codigo : 0,
                    CodigoTipoCarga = carga.TipoDeCarga?.Codigo ?? 0,
                    CodigoTipoOperacao = carga.TipoOperacao?.Codigo ?? 0,
                    CodigoTracao = carga.Veiculo.Codigo,
                    CodigoMotorista = carga.Motoristas.FirstOrDefault().Codigo,
                    SalvarDadosTransporteSemSolicitarNFes = false,

                };

                svcCarga.SalvarDadosTransporteCarga(dadosTransporte, out mensagemErro, Usuario, false, TipoServicoMultisoftware, WebServiceConsultaCTe, Cliente, Auditado, unitOfWork);

                //if (((carga.TipoOperacao?.SolicitarNotasFiscaisAoSalvarDadosTransportador ?? false) || ConfiguracaoEmbarcador.SolicitarNotasFiscaisAoSalvarDadosTransportador) && carga.SituacaoCarga == SituacaoCarga.AgTransportador && !carga.ExigeNotaFiscalParaCalcularFrete)
                //    svcCarga.SolicitarNotasFiscais(carga, ConfiguracaoEmbarcador, TipoServicoMultisoftware, Auditado, unitOfWork);
            }
        }

        private bool PermiteSalvarDadosTransporte(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            return (carga.SituacaoCarga == SituacaoCarga.Nova && carga.ExigeNotaFiscalParaCalcularFrete) || (carga.SituacaoCarga == SituacaoCarga.AgTransportador && !carga.ExigeNotaFiscalParaCalcularFrete);
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> ConfiguracaoImportacaoPlanilhaDocumentoTransporte()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>();

            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = "Nº NFe", Propriedade = "NumeroNFE", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 2, Descricao = "Nº CTe", Propriedade = "NumeroCTE", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 3, Descricao = "Chave Acesso CTe", Propriedade = "ChaveAcessoCTE", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 4, Descricao = "Chave Acesso NFe", Propriedade = "ChaveAcessoNFE", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 5, Descricao = "Peso", Propriedade = "Peso", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 6, Descricao = "Volumen", Propriedade = "Volumen", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 7, Descricao = "Status", Propriedade = "Status", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 8, Descricao = "Observacao", Propriedade = "Observacao", Tamanho = 100, Obrigatorio = false });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 9, Descricao = "Fornecedor", Propriedade = "Fornecedor", Tamanho = 100, Obrigatorio = false });

            return configuracoes;
        }

        private Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha RetornarFalhaLinha(string mensagem, int indice)
        {
            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retorno = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = indice, mensagemFalha = mensagem, processou = false };
            return retorno;
        }

        private void SalvarDocumentoParaTransporte(Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.DocumentoTransporte repDocumentoTrasnporte = new Repositorio.Embarcador.Logistica.DocumentoTransporte(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            List<Dominio.Entidades.Embarcador.Logistica.DocumentoTransporte> listaDocumentosParaTransporte = repDocumentoTrasnporte.BuscarPorCodigoAgendamento(agendamentoColeta.Codigo);

            dynamic listaDocumentoTransporte = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("DocumentoParaTransporte"));

            if (listaDocumentosParaTransporte != null && listaDocumentosParaTransporte.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (dynamic documento in listaDocumentoTransporte)
                    if (documento.Codigo != null)
                        codigos.Add((int)documento.Codigo);

                List<Dominio.Entidades.Embarcador.Logistica.DocumentoTransporte> documentoTransporteRemover = listaDocumentosParaTransporte.Where(o => !codigos.Contains(o.Codigo)).ToList();

                for (var i = 0; i < documentoTransporteRemover.Count; i++)
                    repDocumentoTrasnporte.Deletar(documentoTransporteRemover[i]);
            }
            else
                listaDocumentosParaTransporte = new List<Dominio.Entidades.Embarcador.Logistica.DocumentoTransporte>();

            foreach (var documento in listaDocumentoTransporte)
            {
                if (listaDocumentosParaTransporte.Any(o => o.Codigo == (int)documento.Codigo))
                    continue;

                Dominio.Entidades.Embarcador.Logistica.DocumentoTransporte documentoTransporte = repDocumentoTrasnporte.BuscarPorCodigo((int)documento.Codigo);

                if (documentoTransporte == null)
                    documentoTransporte = new Dominio.Entidades.Embarcador.Logistica.DocumentoTransporte();

                Dominio.Entidades.Cliente fornecedor = repCliente.BuscarPorCPFCNPJ((double)documento.CodigoFornecedor);
                if (fornecedor == null)
                    continue;

                documentoTransporte.NumeroNF = (int)documento.NumeroNFE;
                documentoTransporte.NumeroCte = (int)documento.NumeroCTE;
                documentoTransporte.Fornecedor = fornecedor;
                documentoTransporte.ChaveNFe = (string)documento.ChaveAcessoCTE;
                documentoTransporte.ChaveCte = (string)documento.ChaveAcessoNFE;
                documentoTransporte.Peso = (decimal)documento.Peso;
                documentoTransporte.Volumen = (int)documento.Volumen;
                documentoTransporte.Agendamento = agendamentoColeta;

                string status = (string)documento.Status;
                documentoTransporte.StatusDocumento = status == "OK" ? StatusDocumento.OK : StatusDocumento.NAOOK;
                documentoTransporte.Observacao = (string)documento.Observacao;

                if (documentoTransporte.Codigo == 0)
                    repDocumentoTrasnporte.Inserir(documentoTransporte);
                else
                    repDocumentoTrasnporte.Atualizar(documentoTransporte);
            }

        }

        private void CadastroXMLNF(List<Dominio.Entidades.Embarcador.Logistica.DocumentoTransporte> listaDocumento, Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta, Repositorio.UnitOfWork unitOfWork)
        {

            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            Servicos.Embarcador.Pedido.NotaFiscal serCargaNotaFiscal = new Servicos.Embarcador.Pedido.NotaFiscal(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoTMS.BuscarConfiguracaoPadrao();
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedido = repCargaPedido.BuscarPorProtocoloCarga(agendamentoColeta.Carga?.Protocolo ?? 0);

            foreach (Dominio.Entidades.Embarcador.Logistica.DocumentoTransporte documentoTransporte in listaDocumento)
            {
                Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal novaNotaFiscal = new Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal()
                {
                    SemCarga = false,
                    Numero = documentoTransporte?.NumeroNF ?? 0,
                    Chave = documentoTransporte?.ChaveNFe ?? string.Empty,
                    Emitente = documentoTransporte?.Fornecedor ?? null,
                    Peso = documentoTransporte?.Peso ?? 0m,
                    Volumes = documentoTransporte?.Volumen ?? 0,
                    Observacao = documentoTransporte?.Observacao ?? string.Empty,
                    DataRecebimento = DateTime.Now,
                    NumeroPedido = agendamentoColeta?.Pedido?.Numero ?? 0,
                    XML = string.Empty,
                    DataEmissao = DateTime.Now,
                    TipoDocumento = TipoDocumento.NotaFiscal,
                    TipoOperacaoNotaFiscal = TipoOperacaoNotaFiscal.Saida,
                    CNPJTranposrtador = documentoTransporte?.Fornecedor?.CPF_CNPJ_Formatado ?? string.Empty,
                    PlacaVeiculoNotaFiscal = ""

                };

                repXmlNotaFiscal.Inserir(novaNotaFiscal);

                bool alteradoTipoCarga = false;
                serCargaNotaFiscal.InserirNotaCargaPedido(novaNotaFiscal, cargaPedido.FirstOrDefault(), TipoServicoMultisoftware, TipoNotaFiscal.Venda, configuracaoEmbarcador, false, out alteradoTipoCarga, Auditado);

                if (agendamentoColeta.Pedido != null)
                    agendamentoColeta.Pedido.NotasFiscais.Add(novaNotaFiscal);
            }

        }

        private void CadastroCtes(List<Dominio.Entidades.Embarcador.Logistica.DocumentoTransporte> listaDocumento, Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPreCte repCargaPreCte = new Repositorio.Embarcador.Cargas.CargaPreCte(unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Logistica.DocumentoTransporte documentoTransporte in listaDocumento)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPreCte novaPreCte = new Dominio.Entidades.Embarcador.Cargas.CargaPreCte()
                {
                    Carga = agendamentoColeta.Carga,
                    ChaveCte = documentoTransporte.ChaveCte,
                    NumeroCte = documentoTransporte.NumeroCte,
                    StatusDocumento = documentoTransporte.StatusDocumento
                };

                repCargaPreCte.Inserir(novaPreCte);
            }


        }

        private string RetornarMensagem(string mensagem, string mensagemNova)
        {
            if (mensagem.Contains(mensagemNova))
                return mensagem;

            return string.IsNullOrWhiteSpace(mensagem) ? mensagemNova : mensagem += $"; {mensagemNova}";
        }

        private void EnviarAnexoAgendamentoColetaParaNFe(Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta, List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaAnexo> anexos, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Cargas.CargaNFeAnexo, Dominio.Entidades.Embarcador.Cargas.Carga> repositorioAnexo = new Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Cargas.CargaNFeAnexo, Dominio.Entidades.Embarcador.Cargas.Carga>(unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaAnexo anexo in anexos)
            {
                string caminhoArquivo = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", typeof(Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta).Name });
                string caminhoArquivoNFe = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", typeof(Dominio.Entidades.Embarcador.Cargas.Carga).Name });
                string guidArquivo = anexo.GuidArquivo;
                string extensaoArquivo = System.IO.Path.GetExtension(anexo.NomeArquivo).ToLower();
                string arquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminhoArquivo, guidArquivo + extensaoArquivo);

                if (Utilidades.IO.FileStorageService.Storage.Exists(arquivo))
                {
                    string guidArquivoNFe = string.Empty;
                    string arquivoNFe = string.Empty;

                    int tentativas = 0;

                    do
                    {
                        guidArquivoNFe = Guid.NewGuid().ToString().Replace("-", "");
                        arquivoNFe = Utilidades.IO.FileStorageService.Storage.Combine(caminhoArquivoNFe, guidArquivoNFe + extensaoArquivo);

                        tentativas++;
                    } while (Utilidades.IO.FileStorageService.Storage.Exists(arquivoNFe) && tentativas < 5);

                    if (!Utilidades.IO.FileStorageService.Storage.Exists(arquivoNFe))
                    {
                        Utilidades.IO.FileStorageService.Storage.Copy(arquivo, arquivoNFe);

                        Dominio.Entidades.Embarcador.Cargas.CargaNFeAnexo anexoAgendamentoColeta = new Dominio.Entidades.Embarcador.Cargas.CargaNFeAnexo()
                        {
                            EntidadeAnexo = agendamentoColeta.Carga,
                            Descricao = anexo.Descricao,
                            GuidArquivo = guidArquivo,
                            NomeArquivo = anexo.NomeArquivo
                        };

                        repositorioAnexo.Inserir(anexoAgendamentoColeta, Auditado);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, agendamentoColeta.Carga, null, $"Adicionou o arquivo {anexo.NomeArquivo} pelo agendamento de coleta.", unitOfWork);
                    }
                }
            }
        }

        private void EnviarEmailCancelamentoPedido(Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido pedidoRemovido, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            Repositorio.Embarcador.Logistica.AgendamentoColetaPedido repositorioAgendamentoColetaPedido = new Repositorio.Embarcador.Logistica.AgendamentoColetaPedido(unitOfWork);
            List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido> agendamentoColetaPedido = repositorioAgendamentoColetaPedido.BuscarPorAgendamentoColeta(pedidoRemovido.AgendamentoColeta.Codigo);

            if (agendamentoColetaPedido.Count == 0)
                return;

            List<string> emails = new List<string>();

            foreach (string email in agendamentoColetaPedido.Select(o => o.Pedido.Remetente.Email).Distinct())
                emails.Add(email);

            Servicos.Embarcador.Logistica.AgendamentoColeta servicoAgendamentoColeta = new Servicos.Embarcador.Logistica.AgendamentoColeta(unitOfWork);
            servicoAgendamentoColeta.EnviarEmailCancelamentoAgendamentoPedido(pedidoRemovido, emails, cliente);
        }

        private void EnviarEmailAgendamentoAdicionadoParaRemetentePedido(Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova);

            Repositorio.Embarcador.Logistica.AgendamentoColetaPedido repositorioAgendamentoColetaPedido = new Repositorio.Embarcador.Logistica.AgendamentoColetaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido> agendamentoColetaPedido = repositorioAgendamentoColetaPedido.BuscarPorAgendamentoColeta(agendamentoColeta.Codigo);


            if (agendamentoColetaPedido.Count == 0)
                return;

            List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido> listaAgendamentoPedido = ConfiguracaoEmbarcador.ControlarAgendamentoSKU ? repositorioAgendamentoColetaPedido.BuscarPorAgendamentoColeta(agendamentoColeta.Codigo) : new List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido>();
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargaPedido = !ConfiguracaoEmbarcador.ControlarAgendamentoSKU ? repositorioCargaPedido.BuscarPorCargasSemFetch(agendamentoColeta.Carga.Codigo) : new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            string numerosPedidos = ObterDescricaoPedidos(agendamentoColeta, listaCargaPedido, listaAgendamentoPedido);

            List<string> emails = new List<string>();

            foreach (string email in agendamentoColetaPedido.Select(o => o.Pedido.Remetente.Email).Distinct())
                emails.Add(email);

            Servicos.Embarcador.Logistica.AgendamentoColeta servicoAgendamentoColeta = new Servicos.Embarcador.Logistica.AgendamentoColeta(unitOfWork);
            servicoAgendamentoColeta.EnviarEmailAgendamentoAdicionadoParaRemetentePedido(emails, numerosPedidos, cliente);
        }

        private void EnviarEmailAdicaoPedido(Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido pedidoAdicionado, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            Repositorio.Embarcador.Logistica.AgendamentoColetaPedido repositorioAgendamentoColetaPedido = new Repositorio.Embarcador.Logistica.AgendamentoColetaPedido(unitOfWork);
            List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido> agendamentoColetaPedido = repositorioAgendamentoColetaPedido.BuscarPorAgendamentoColeta(pedidoAdicionado.AgendamentoColeta.Codigo);

            if (agendamentoColetaPedido.Count == 0)
                return;

            List<string> emails = new List<string>();

            foreach (string email in agendamentoColetaPedido.Select(o => o.Pedido.Remetente.Email).Distinct())
                emails.Add(email);

            Servicos.Embarcador.Logistica.AgendamentoColeta servicoAgendamentoColeta = new Servicos.Embarcador.Logistica.AgendamentoColeta(unitOfWork);
            servicoAgendamentoColeta.EnviarEmailAdicaoAgendamentoPedido(pedidoAdicionado, emails, cliente);
        }

        private string ObterCodigoIntegracaoProduto(Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido agendamentoPedido, List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> pedidosProdutos, string stringConexao)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Logistica.CentroDescarregamento repositorioCentroDescarregamento = new Repositorio.Embarcador.Logistica.CentroDescarregamento(unitOfWork);

            if (repositorioCentroDescarregamento.BuscarPorDestinatario(agendamentoPedido.Pedido.Destinatario.Codigo).UsarLayoutAgendamentoPorCaixaItem)
            {
                string codigo = agendamentoPedido.Pedido.ProdutoPrincipal?.CodigoProdutoEmbarcador;

                if (!string.IsNullOrWhiteSpace(codigo))
                    return codigo;

                return pedidosProdutos.Find(o => o.Pedido.Codigo == agendamentoPedido.Pedido.Codigo)?.Produto?.CodigoProdutoEmbarcador ?? string.Empty;
            }
            else
            {
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> produtos = pedidosProdutos.Where(o => o.Pedido.Codigo == agendamentoPedido.Pedido.Codigo).ToList();

                return string.Join(", ", produtos.Select(x => x.Produto.CodigoProdutoEmbarcador));
            }
        }

        private string ObterDescricaoProduto(Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido agendamentoPedido, List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> pedidosProdutos, string stringConexao)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Logistica.CentroDescarregamento repositorioCentroDescarregamento = new Repositorio.Embarcador.Logistica.CentroDescarregamento(unitOfWork);

            if (repositorioCentroDescarregamento.BuscarPorDestinatario(agendamentoPedido.Pedido.Destinatario.Codigo).UsarLayoutAgendamentoPorCaixaItem)
            {
                string descricao = agendamentoPedido.Pedido.ProdutoPrincipal?.GrupoProduto.Descricao;

                if (!string.IsNullOrWhiteSpace(descricao))
                    return descricao;

                return pedidosProdutos.Find(o => o.Pedido.Codigo == agendamentoPedido.Pedido.Codigo)?.Produto?.GrupoProduto.Descricao ?? string.Empty;
            }
            else
            {
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> produtos = pedidosProdutos.Where(o => o.Pedido.Codigo == agendamentoPedido.Pedido.Codigo).ToList();

                return string.Join(", ", produtos.Select(x => x.Produto.Descricao));
            }
        }

        private int ObterQuantidadeCaixasAgendamentoPedido(Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido agendamentoColetaPedido, List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedidoProduto> listAgendamentoColetaPedidoProduto)
        {
            int quantidadeCaixas = 0;
            if (listAgendamentoColetaPedidoProduto.Count > 0)
            {
                List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedidoProduto> produtos = listAgendamentoColetaPedidoProduto.Where(o => o.AgendamentoColetaPedido.Pedido.Codigo == agendamentoColetaPedido.Pedido.Codigo).ToList();
                quantidadeCaixas = (int)produtos.Sum(o => o.QuantidadeDeCaixas);
            }
            return quantidadeCaixas;
        }

        private int ObterQuantidadeItensAgendamentoPedido(Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido agendamentoColetaPedido, List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedidoProduto> listAgendamentoColetaPedidoProduto, string stringConexao)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

            Repositorio.Embarcador.Logistica.CentroDescarregamento centroDescarregamento = new Repositorio.Embarcador.Logistica.CentroDescarregamento(unitOfWork);

            if (!centroDescarregamento.BuscarPorDestinatario(agendamentoColetaPedido.Pedido.Destinatario.Codigo).UsarLayoutAgendamentoPorCaixaItem)
            {
                int quantidadeItens = 0;
                if (listAgendamentoColetaPedidoProduto.Count > 0)
                {
                    List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedidoProduto> produtos = listAgendamentoColetaPedidoProduto.Where(o => o.AgendamentoColetaPedido.Pedido.Codigo == agendamentoColetaPedido.Pedido.Codigo).ToList();
                    quantidadeItens = produtos.Where(o => o.Quantidade > 0).ToList().Count;
                }
                return quantidadeItens;
            }
            else return 1;
        }

        private int ObterQuantidadeProdutosAgendamentoPedido(Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido agendamentoColetaPedido, List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedidoProduto> listAgendamentoColetaPedidoProduto)
        {
            int quantidadeProdutos = 0;
            if (listAgendamentoColetaPedidoProduto.Count > 0)
            {
                List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedidoProduto> produtos = listAgendamentoColetaPedidoProduto.Where(o => o.AgendamentoColetaPedido.Pedido.Codigo == agendamentoColetaPedido.Pedido.Codigo).ToList();
                quantidadeProdutos = produtos.Sum(o => o.Quantidade);
            }
            return quantidadeProdutos;
        }

        private string ObterDataMaximaAgendamento(DateTime? data)
        {
            if (!data.HasValue)
                return string.Empty;

            int year = data.Value.Year;
            int month = data.Value.Month;
            int day = data.Value.Day;

            return string.Format("{0:D2}/{1:D2}/{2:D4} 23:59:59", day, month, year);
        }

        #endregion
    }
}
