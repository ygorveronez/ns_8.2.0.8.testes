using Dominio.Enumeradores;
using Dominio.ObjetosDeValor;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Pedidos
{
    [CustomAuthorize(new string[] { "ObterDadosPedido" }, "Pedidos/AcompanhamentoPedido")]
    public class AcompanhamentoPedidoController : BaseController
    {
        #region Construtores

        public AcompanhamentoPedidoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> ObterAcompanhamentoPedido(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedido filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);

                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork, cancellationToken);
                //Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta
                {
                    InicioRegistros = int.Parse(Request.Params("inicio")),
                    LimiteRegistros = int.Parse(Request.Params("limite")),
                    DirecaoOrdenar = "asc",
                    PropriedadeOrdenar = "Codigo"
                };

                int totalRegistros = await repositorioPedido.ContarConsultaAsync(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> listaPedido = totalRegistros > 0 ? await repositorioPedido.ConsultarAsync(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
                //List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorPedidos((from obj in listaPedido select obj.Codigo).ToList());
                //List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidosXMLNotaFiscal = repPedidoXMLNotaFiscal.BuscarPorPedidos((from obj in listaPedido select obj.Codigo).Distinct().ToList());

                var listaPedidoRetornar = (
                    from pedido in listaPedido
                    select ObterObjetoAcompanhamentoPedido(pedido, unitOfWork)
                ).ToList();

                return new JsonpResult(listaPedidoRetornar, totalRegistros);

            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterDadosPedido()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            AdminMultisoftware.Repositorio.UnitOfWork adminUnitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(_conexao.AdminStringConexao);

            int codigoPedido = Request.GetIntParam("codigoPedido");
            try
            {
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repPedido.BuscarPorCodigo(codigoPedido);
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidosXMLNotaFiscal = repPedidoXMLNotaFiscal.BuscarPorPedido(codigoPedido);
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repositorioCargaEntrega.BuscarPorPedido(pedido.Codigo);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorPedido(pedido.Codigo);

                string urlRastreamento = string.Empty;

                // Verifica se o pedido está em uma entrega. Pode ter o guid do código de rastreio antes de estar na entrega.
                if (cargaEntrega != null)
                {
                    string urlBase = Servicos.Embarcador.GestaoEntregas.NotificaoEntrega.ObterURLBase(Cliente.Codigo, Cliente.ClienteConfiguracao.TipoServicoMultisoftware, adminUnitOfWork, _conexao.AdminStringConexao, unitOfWork);
                    urlRastreamento = !string.IsNullOrWhiteSpace(pedido.CodigoRastreamento ?? "") ? Servicos.Embarcador.GestaoEntregas.NotificaoEntrega.ObterURLRastreamentoPedido(pedido.CodigoRastreamento, urlBase) : string.Empty;
                }

                var dynPedido = new
                {
                    pedido.Codigo,
                    Descricao = pedido.NumeroPedidoEmbarcador,
                    NumeroCarga = string.Join(", ", pedido.CodigoCargaEmbarcador),
                    CodigoCarga = carga?.Codigo ?? 0,
                    pedido.Numero,
                    PrevisaoEntrega = pedido.PrevisaoEntrega?.ToDateTimeString() ?? string.Empty,
                    pedido.NumeroPedidoEmbarcador,
                    PesoTotal = pedido.PesoTotal.ToString("n2"),
                    NumerosNotas = string.Join(", ", (from obj in pedidosXMLNotaFiscal select obj.XMLNotaFiscal.Numero).Distinct().ToList()),
                    Remetente = pedido.Remetente?.Descricao ?? pedido.GrupoPessoas?.Descricao ?? string.Empty,
                    Destinatario = pedido.Destinatario?.Descricao ?? string.Empty,
                    Destino = pedido.Destino?.DescricaoCidadeEstado ?? string.Empty,
                    Origem = pedido.Origem?.DescricaoCidadeEstado ?? string.Empty,
                    DataCarregamentoPedido = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor ? pedido?.DataInicialColeta?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty : pedido?.DataCarregamentoPedido?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                    URLRastreamentoEntrega = urlRastreamento,
                    NotasFiscais = (from obj in pedidosXMLNotaFiscal
                                    select new
                                    {
                                        obj.Codigo,
                                        obj.XMLNotaFiscal.Numero,
                                        obj.XMLNotaFiscal.Serie,
                                        Valor = obj.XMLNotaFiscal.Valor.ToString("n2"),
                                        Peso = obj.XMLNotaFiscal.Peso.ToString("n2"),
                                        Volumes = obj.XMLNotaFiscal.Volumes.ToString("n2")
                                    }).Distinct().ToList()
                    //Motorista = motoristas,
                    //Veiculo = placas,
                    //SituacaoPedido = pedido.DescricaoSituacaoPedido

                };



                return new JsonpResult(dynPedido);

            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
                adminUnitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterDadosColeta()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            int codigoPedido = Request.GetIntParam("codigoPedido");
            try
            {

                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repPedido.BuscarPorCodigo(codigoPedido);

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repCargaEntrega.BuscarPorPedido(codigoPedido, pedido.Remetente.CPF_CNPJ, true);

                if (cargaEntrega == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar uma coleta para este pedido");

                var retorno = new
                {
                    cargaEntrega.Codigo,
                    CodigoPedido = codigoPedido,
                    Programacao = pedido.DataInicialColeta?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                    Transportador = cargaEntrega?.Carga?.Empresa?.Descricao ?? string.Empty,
                    Veiculo = cargaEntrega.Carga?.DadosSumarizados?.Veiculos ?? string.Empty,
                    ModeloVeiculo = cargaEntrega.Carga?.ModeloVeicularCarga?.Descricao ?? string.Empty,
                    Motorista = cargaEntrega.Carga?.DadosSumarizados?.Motoristas ?? string.Empty,
                    Lacres = pedido.LacreContainerDois,
                    pedido.Observacao,
                    Situacao = cargaEntrega.DescricaoSituacao,
                    LocalColeta = new
                    {
                        Latitude = (cargaEntrega.Cliente?.Latitude ?? "").Replace(".", ",").ToDecimal(),
                        Longitude = (cargaEntrega.Cliente?.Longitude ?? "").Replace(".", ",").ToDecimal(),
                    },

                    Imagens = (from o in cargaEntrega.Fotos
                               select new
                               {
                                   o.Codigo,
                                   o.Latitude,
                                   o.Longitude,
                                   DataRecebimento = o.DataEnvioImagem.ToString("dd/MM/yyyy HH:mm"),
                                   Miniatura = Base64ImagemAnexo(o, unitOfWork)
                               }).ToList(),

                };

                return new JsonpResult(retorno);

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
        public async Task<IActionResult> ObterDadosTransporte()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            int codigoPedido = Request.GetIntParam("codigoPedido");
            try
            {
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarCargaAtualPorPedido(codigoPedido);

                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repCargaEntrega.BuscarPorCargaPedido(cargaPedido?.Codigo ?? 0);
                Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramentoCarga = repMonitoramento.BuscarUltimoPorCarga(cargaPedido?.Carga?.Codigo ?? 0);

                var dynDados = new
                {
                    DadosTransporte = new
                    {
                        Carga = cargaPedido?.Carga?.Codigo ?? 0,
                        Veiculo = cargaPedido?.Carga?.Veiculo?.Codigo ?? 0,
                        Monitoramento = monitoramentoCarga?.Codigo ?? 0
                    }
                };

                return new JsonpResult(dynDados);

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
        public async Task<IActionResult> ObterDadosEntrega()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            int codigoPedido = Request.GetIntParam("codigoPedido");
            try
            {
                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repPedido.BuscarPorCodigo(codigoPedido);

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repCargaEntrega.BuscarPorPedido(codigoPedido, pedido.Destinatario.CPF_CNPJ, coleta: false);

                if (cargaEntrega == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar um entrega para este pedido");

                Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);

                Servicos.Embarcador.Canhotos.Canhoto srvCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(unitOfWork);

                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidosXMLNotaFiscal = repPedidoXMLNotaFiscal.BuscarPorPedido(codigoPedido);

                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais = (from notas in pedidosXMLNotaFiscal select notas.XMLNotaFiscal).Distinct().ToList();

                List<int> codigosNotas = (from nota in notasFiscais select nota.Codigo).ToList();

                List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotos = repCanhoto.BuscarPorNFsComImagem(codigosNotas, cargaEntrega.Carga.Codigo);



                DateTime dataEntrega = cargaEntrega.DataReprogramada ?? cargaEntrega.DataPrevista ?? DateTime.Now;
                List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento> janelasDescarga = Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.ObterJanelaDescarregamento(cargaEntrega, dataEntrega, unitOfWork);

                var retorno = new
                {
                    cargaEntrega.Codigo,
                    CodigoPedido = codigoPedido,
                    Numero = cargaEntrega.Carga.CodigoCargaEmbarcador,
                    Destinatario = cargaEntrega.Cliente?.Descricao ?? string.Empty,
                    Localidade = cargaEntrega.Cliente?.Localidade.Descricao ?? string.Empty,
                    Situacao = cargaEntrega.DescricaoSituacao,
                    EnumSituacao = cargaEntrega.Situacao,
                    Transportador = cargaEntrega?.Carga?.Empresa?.Descricao ?? string.Empty,
                    Veiculo = cargaEntrega.Carga?.DadosSumarizados?.Veiculos ?? string.Empty,
                    ModeloVeiculo = cargaEntrega.Carga?.ModeloVeicularCarga?.Descricao ?? string.Empty,
                    Motorista = cargaEntrega.Carga?.DadosSumarizados?.Motoristas ?? string.Empty,
                    DataPrevisaoEntrega = cargaEntrega.DataPrevista?.ToString("dd/MM/yyyy HH:mm"), //cargaEntrega.DataEntregaReprogramada != null ? cargaEntrega.DataEntregaPrevista?.ToString("dd/MM/yyyy HH:mm") : (cargaEntrega.DataEntregaPrevista?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty),
                    DataEntrega = cargaEntrega.DataFim?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                    DataRejeitado = cargaEntrega.DataRejeitado?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                    Observacao = (cargaEntrega.Observacao ?? "").Replace("\n", "<br />"),
                    NomeRecebedor = cargaEntrega.Cliente?.Nome ?? string.Empty,
                    DocumentoRecebedor = cargaEntrega.Cliente?.CPF_CNPJ_Formatado ?? string.Empty,
                    JanelaDescarga = janelasDescarga != null ? string.Join(", ", (from janelaDescarga in janelasDescarga select janelaDescarga?.HoraInicio.ToString(@"hh\:mm") + " - " + janelaDescarga?.HoraTermino.ToString(@"hh\:mm"))) : string.Empty,
                    NotasFiscais = String.Join("", (from nota in notasFiscais select nota.Numero)),

                    AvaliacaoEfetuada = cargaEntrega.DataAvaliacao.HasValue,
                    DataAvaliacao = cargaEntrega.DataAvaliacao?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                    Avaliacao = cargaEntrega.AvaliacaoGeral ?? 0,
                    ObservacaoAvaliacao = (cargaEntrega.ObservacaoAvaliacao ?? "").Replace("\n", "<br />"),

                    LocalEntrega = new
                    {
                        Latitude = (cargaEntrega.Cliente?.Latitude ?? "").Replace(".", ",").ToDecimal(),
                        Longitude = (cargaEntrega.Cliente?.Longitude ?? "").Replace(".", ",").ToDecimal(),
                    },
                    Imagens = (from o in cargaEntrega.Fotos
                               select new
                               {
                                   o.Codigo,
                                   DataRecebimento = o.DataEnvioImagem.ToString("dd/MM/yyyy HH:mm"),
                                   Miniatura = Base64ImagemAnexo(o, unitOfWork)
                               }).ToList(),

                    ImagensCanhoto = (from canhoto in canhotos
                                      select new
                                      {
                                          canhoto.Codigo,
                                          canhoto.Numero,
                                          Miniatura = srvCanhoto.ObterMiniatura(canhoto, unitOfWork)
                                      }).ToList(),

                };



                return new JsonpResult(retorno);

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
        public async Task<IActionResult> SalvarDadosColeta()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);


            int codigoPedido = Request.GetIntParam("codigoPedido");
            try
            {
                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repPedido.BuscarPorCodigo(codigoPedido);

                if (pedido == null)
                    return new JsonpResult(false, true, "Pedido não encontrado");

                pedido.Observacao = Request.GetStringParam("Observacao");
                pedido.LacreContainerDois = Request.GetStringParam("Lacres");

                repPedido.Atualizar(pedido);

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }

        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterConfiguracaoGeral()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoPortalMultiClifor repositorioConfiguracaoPortalMultiClifor = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPortalMultiClifor(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPortalMultiClifor configuracaoPortalMultiClifor = repositorioConfiguracaoPortalMultiClifor.BuscarConfiguracaoPadrao();

                var retorno = new
                {
                    MostrarNoAcompanhamentoDePedidosDeslocamentoVazio = configuracaoPortalMultiClifor.MostrarNoAcompanhamentoDePedidosDeslocamentoVazio
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoBuscarConfiguracaoPadrao);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private string Base64ImagemAnexo(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaFoto foto, Repositorio.UnitOfWork unitOfWork)
        {
            string caminho = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "EntregaPedido" });
            return Base64Imagem(caminho, foto.NomeArquivo, foto.GuidArquivo);
        }

        private string Base64Imagem(string caminho, string nomeArquivo, string guidArquivo)
        {
            string extensao = Path.GetExtension(nomeArquivo);
            string arquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, guidArquivo + "-miniatura" + extensao);

            if (Utilidades.IO.FileStorageService.Storage.Exists(arquivo))
            {
                byte[] imageArray = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(arquivo);
                string base64ImageRepresentation = Convert.ToBase64String(imageArray);

                return base64ImageRepresentation;
            }
            else
            {
                return "";
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedido ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            string cpfcnpj = Utilidades.String.OnlyNumbers(Request.Params("Codigo"));
            double dCPFCNPJ = 0;
            if (!string.IsNullOrEmpty(cpfcnpj))
                dCPFCNPJ = Double.Parse(cpfcnpj);

            List<int> codigoFilial = Request.GetListParam<int>("Filial");
            int codigoTipoCarga = Request.GetIntParam("TipoCarga");
            int codigoMotorista = Request.GetIntParam("Motorista");
            int codigoVeiculo = Request.GetIntParam("Veiculo");
            int grupoPessoa = Usuario.ClienteFornecedor?.GrupoPessoas?.Codigo ?? 0;
            double remetente = Request.GetDoubleParam("Remetente");
            double destinatario = Request.GetDoubleParam("Destinatario");
            List<double> destinatarios = destinatario > 0 ? new List<double>() { destinatario } : Request.GetListParam<double>("Destinatario");
            List<int> codigoCentroResultado = Request.GetListParam<int>("CentroResultado");

            bool compartilharAcessoEntreGrupoPessoas = IsCompartilharAcessoEntreGrupoPessoas();

            Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedido filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedido()
            {
                CidadePoloDestino = Request.GetIntParam("CidadePoloDestino"),
                CidadePoloOrigem = Request.GetIntParam("CidadePoloOrigem"),
                CodigosFilial = codigoFilial.Count == 0 ? ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork) : codigoFilial,
                CodigosTipoCarga = codigoTipoCarga == 0 ? ObterListaCodigoTipoCargaPermitidosOperadorLogistica(unitOfWork) : new List<int>() { codigoTipoCarga },
                CodigosTipoOperacao = ObterListaCodigoTipoOperacaoPermitidosOperadorLogistica(unitOfWork),
                DataColeta = Request.GetNullableDateTimeParam("DataColeta"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicial"),
                DataLimite = Request.GetNullableDateTimeParam("DataFinal"),
                Destinatarios = destinatarios,
                CodigosCentroResultado = codigoCentroResultado,
                Destino = Request.GetIntParam("Destino"),
                GrupoPessoa = Request.GetIntParam("GrupoPessoa"),
                NumeroCarga = Request.GetStringParam("CodigoCargaEmbarcador"),
                NumeroNotasFiscais = ObterListaInteiros(Request.GetStringParam("NotaFiscal")),
                NumeroPedido = Request.GetIntParam("NumeroPedido"),
                NumeroPedidoEmbarcador = Request.GetStringParam("NumeroPedidoEmbarcador"),
                Origem = Request.GetIntParam("Origem"),
                PaisDestino = Request.GetIntParam("PaisDestino"),
                PaisOrigem = Request.GetIntParam("PaisOrigem"),
                Remetente = remetente,
                Situacao = Request.GetNullableEnumParam<SituacaoPedido>("Situacao"),
                SituacaoAcompanhamentoPedido = Request.GetNullableEnumParam<SituacaoAcompanhamentoPedido>("SituacaoAcompanhamentoPedido"),
                Tomador = Request.GetDoubleParam("Tomador"),
                CodigosMotorista = codigoMotorista > 0 ? new List<int>() { codigoMotorista } : null,
                CodigosVeiculo = codigoVeiculo > 0 ? new List<int>() { codigoVeiculo } : null,
                FiltarRementeDestinatarioPorTransportador = false,
                FiltrarPedidosCargaFechada = true,
                TipoServicoMultisoftware = TipoServicoMultisoftware,
                Placa = Request.GetStringParam("Placa"),
                NaoMostrarCargasDeslocamentoVazio = Request.GetBoolParam("NaoMostrarCargasDeslocamentoVazio"),
            };

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                filtrosPesquisa.FiltarRementeDestinatarioPorTransportador = true;
                filtrosPesquisa.CodigoTransportador = Usuario.Empresa.Codigo;
            }
            else if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
            {
                filtrosPesquisa.CompartilharAcessoEntreGrupoPessoas = compartilharAcessoEntreGrupoPessoas;
                filtrosPesquisa.VisualizarApenasParaPedidoDesteTomador = Usuario?.ClienteFornecedor?.VisualizarApenasParaPedidoDesteTomador ?? false;
                filtrosPesquisa.VisualizarPedidosApenasAlgunsDeterminadosGruposDePessoas = Usuario.ClienteFornecedor?.VisualizarPedidosApenasAlgunsDeterminadosGruposDePessoas ?? false;

                if (filtrosPesquisa.VisualizarPedidosApenasAlgunsDeterminadosGruposDePessoas)
                    filtrosPesquisa.CodigosGrupoPessoa = Usuario.ClienteFornecedor?.GruposPessoas.Select(o => o.Codigo).ToList();
                else if (compartilharAcessoEntreGrupoPessoas)
                    filtrosPesquisa.GrupoPessoa = grupoPessoa;
                else if (filtrosPesquisa.VisualizarApenasParaPedidoDesteTomador)
                    filtrosPesquisa.Tomador = Usuario.ClienteFornecedor?.CPF_CNPJ ?? 0;
                else
                    filtrosPesquisa.ClientePortal = Usuario.ClienteFornecedor?.CPF_CNPJ ?? 0;
            }

            return filtrosPesquisa;
        }

        private List<int> ObterListaInteiros(string param)
        {
            List<int> valores = new List<int>();

            foreach (string valor in param.Split(','))
            {
                if (int.TryParse(valor.Trim(), out int intValor))
                    valores.Add(intValor);
            }

            return valores;
        }

        private dynamic ObterObjetoAcompanhamentoPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorPedido(pedido.Codigo);
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotasFiscais = repositorioPedidoXMLNotaFiscal.BuscarPorPedido(pedido.Codigo);

            Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramentoCarga = repMonitoramento.BuscarUltimoPorCarga(carga?.Codigo ?? 0);

            return new
            {
                CodigoPedido = pedido.Codigo,
                //SituacaoPedido = pedido.DescricaoSituacaoPedido,
                Numero = pedido.NumeroPedidoEmbarcador,
                NotasFiscais = string.Join(", ", (from obj in pedidoXMLNotasFiscais select obj.XMLNotaFiscal.Numero).Distinct().ToList()),
                pedido.SituacaoAcompanhamentoPedido,
                DescricaoSituacaoAcompanhamentoPedido = pedido.SituacaoAcompanhamentoPedido.ObterDescricao(),
                DescricaoStatusMonitoramento = monitoramentoCarga != null && monitoramentoCarga.StatusViagem != null ? monitoramentoCarga.StatusViagem.Descricao : "",
                Remetente = pedido.Remetente?.Descricao ?? pedido.GrupoPessoas?.Descricao ?? string.Empty,
                Destinatario = pedido.Destinatario?.Descricao ?? "",
                ValorFrete = carga?.ValorFrete.ToString("n2") ?? string.Empty,
                Origem = pedido.Origem?.DescricaoCidadeEstado ?? string.Empty,
                Destino = pedido.Destino?.DescricaoCidadeEstado ?? string.Empty,
                TipoDeCarga = carga?.TipoDeCarga?.Descricao ?? string.Empty,
                Tracao = carga?.Veiculo?.Descricao ?? string.Empty,
                Reboques = carga?.VeiculosVinculados != null ? string.Join(", ", (from reboque in carga.VeiculosVinculados select reboque.Descricao).ToList()) : string.Empty,
                Motoristas = carga?.Motoristas != null && carga?.Motoristas.Count > 0 ? string.Join(", ", carga.Motoristas.Select(m => m.Descricao).ToList()) : string.Empty,
                DataColeta = pedido?.DataInicialColeta?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                PossuiPassagemAduanas = repositorioCargaEntrega.ExisteFronteirasPorCarga(carga?.Codigo ?? 0),
            };

        }

        #endregion
    }
}
