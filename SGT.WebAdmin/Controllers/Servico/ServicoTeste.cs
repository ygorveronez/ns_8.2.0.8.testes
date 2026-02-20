using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;


namespace SGT.WebAdmin.Controllers.Servico
{
    public class ServicoTesteController : BaseController
    {
        #region Construtores

        public ServicoTesteController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [CustomAuthorize("ServicoTeste/Teste")]
        public async Task<IActionResult> Teste(CancellationToken cancellationToken)

        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdm = new AdminMultisoftware.Repositorio.UnitOfWork(_conexao.AdminStringConexao);
            try
            {
                await unitOfWork.StartAsync(cancellationToken);


                await unitOfWork.RollbackAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                await unitOfWork.RollbackAsync(cancellationToken);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }

            return null;
        }
        #endregion Métodos Globais

        #region Métodos Privados para Adicionar a Chamado no Método Global Teste

        private void TestarConexaoNFTP(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Fatura.FaturaIntegracao repFaturaIntegracao = new Repositorio.Embarcador.Fatura.FaturaIntegracao(unitOfWork);
            Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao integracaoPendente = repFaturaIntegracao.BuscarPorCodigo(710478);
            new Servicos.Embarcador.Integracao.NFTP.IntegracaoNFTP(unitOfWork).IntegrarFatura(integracaoPendente, "");

            //Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao repOcorrenciaCTeIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao(unitOfWork);
            //Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao ocorrenciaCTeIntegracaoPendente = repOcorrenciaCTeIntegracao.BuscarPorCodigo(177);
            //new Servicos.Embarcador.Integracao.NFTP.IntegracaoNFTP(unitOfWork).IntegrarOcorrencia(ocorrenciaCTeIntegracaoPendente, "");

            //Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork);
            //Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao integracaoPendente = repCargaCargaIntegracao.BuscarPorCodigo(17795);
            //new Servicos.Embarcador.Integracao.NFTP.IntegracaoNFTP(unitOfWork).IntegrarCarga(integracaoPendente, "");
        }

        private void TestarGerarOcorrenciaPorTracking(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repCargaEntrega.BuscarPorCodigo(718783);
            Servicos.Embarcador.CargaOcorrencia.OcorrenciaAutomaticaPorPeriodo serOcorrenciaAutomaticaPorPeriodo = new Servicos.Embarcador.CargaOcorrencia.OcorrenciaAutomaticaPorPeriodo(unitOfWork);

            serOcorrenciaAutomaticaPorPeriodo.GerarOcorrenciaPorTracking(cargaEntrega.Carga, (cargaEntrega.Fronteira ? cargaEntrega.Cliente?.CPF_CNPJ ?? 0 : 0), Dominio.ObjetosDeValor.Embarcador.Enumeradores.GatilhoFinalTraking.SaidaCliente, cargaEntrega.DataEntradaRaio.Value, cargaEntrega.DataSaidaRaio.Value, TipoServicoMultisoftware, Cliente, TipoAplicacaoGatilhoTracking.Entrega, cargaEntrega.DataAgendamento);
        }

        private void AjustarSequenciaNumeroChamado(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
            repChamado.AjustarSequenciaSeNecessario();
        }

        private void testarChamadaWSConsultacanhotosDigitalizados(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.WebService.Integradora repositorioIntegradora = new Repositorio.WebService.Integradora(unitOfWork);
            Dominio.Entidades.WebService.Integradora integradora = repositorioIntegradora.BuscarPorToken("e68e9a17bf744eefb563376871357a8b");

            Dominio.ObjetosDeValor.WebService.RequestPaginacao dadosRequest = new Dominio.ObjetosDeValor.WebService.RequestPaginacao();
            dadosRequest.Inicio = 0;
            dadosRequest.Limite = 50;

            var teste = new Servicos.WebService.Canhoto.Canhoto(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, _conexao.AdminStringConexao).BuscarCanhotosNotasFiscaisDigitalizados(dadosRequest, integradora);

        }

        private void ConsultarNFSePendenteMigrate(Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                int numeroTentativas = 10;
                double minutosACadaTentativa = 3;
                int numeroRegistrosPorVez = 15;
                Repositorio.RPSNFSe repRPSNFSe = new Repositorio.RPSNFSe(unitOfWork);

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTePendentes = repCTe.BuscarListaNFSePendentesIntegracaoMigrate(numeroTentativas, minutosACadaTentativa, "Codigo", "asc", numeroRegistrosPorVez);

                foreach (var cte in listaCTePendentes)
                {
                    if (cte.RPS != null)
                    {
                        Servicos.Embarcador.Integracao.Migrate.IntegracaoMigrate serMigrate = new Servicos.Embarcador.Integracao.Migrate.IntegracaoMigrate(unitOfWork);
                        serMigrate.ConsultarRetornoNFSe(cte, unitOfWork);

                        cte.RPS.DataUltimaConsulta = DateTime.Now;
                        cte.RPS.QuantidadeTentativaConsulta++;
                        repRPSNFSe.Atualizar(cte.RPS);
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        private void TestarIntegracaoA53(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork);
            var integracaoPendente = repCargaCargaIntegracao.BuscarPorCodigo(1250);
            new Servicos.Embarcador.Integracao.A52.IntegracaoA52(unitOfWork).IntegrarCarga(integracaoPendente);
        }

        private void TestarIntegracaoCassol(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(unitOfWork);
            var integracaoPendente = repCargaDadosTransporteIntegracao.BuscarPorCodigo(112);
            new Servicos.Embarcador.Integracao.Cassol.IntegracaoCassol(unitOfWork).IntegrarCargaDadosTransporte(integracaoPendente);
        }

        private void TestarIntegracaoVedacit(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(unitOfWork);
            var integracaoPendente = repCargaDadosTransporteIntegracao.BuscarPorCodigo(1143);
            new Servicos.Embarcador.Integracao.Vedacit.IntegracaoVedacit(unitOfWork).IntegrarCargaDadosTransporte(integracaoPendente);
        }

        private bool CompareJson(string json1, string json2)
        {
            var obj1 = JObject.Parse(json1);
            var obj2 = JObject.Parse(json2);

            return JToken.DeepEquals(obj1, obj2);
        }

        public string TestarHashJson(string json)
        {

            string caminhoArquivoUltimo = "C:\\Users\\FernandoMorh\\Downloads\\Ultimo\\ultimo.json";
            string caminhoArquivoPrimeiro = "C:\\Users\\FernandoMorh\\Downloads\\Primerio\\Primeiro.json";
            string caminhoArquivoQualquer = "C:\\Users\\FernandoMorh\\Downloads\\Ultimo\\qualquer.json";
            string requestUltimo = string.Empty;
            string requestPrimeiro = string.Empty;
            string requestQualquer = string.Empty;

            if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoArquivoUltimo))
                requestUltimo = Utilidades.IO.FileStorageService.Storage.ReadAllText(caminhoArquivoUltimo);

            Dominio.ObjetosDeValor.WebService.Rest.DocumentoTransporte documentoTransporteUltimo = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.WebService.Rest.DocumentoTransporte>(requestUltimo);

            if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoArquivoPrimeiro))
                requestPrimeiro = Utilidades.IO.FileStorageService.Storage.ReadAllText(caminhoArquivoPrimeiro);

            Dominio.ObjetosDeValor.WebService.Rest.DocumentoTransporte documentoTransportePrimeiro = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.WebService.Rest.DocumentoTransporte>(requestPrimeiro);

            if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoArquivoQualquer))
                requestQualquer = Utilidades.IO.FileStorageService.Storage.ReadAllText(caminhoArquivoQualquer);

            Dominio.ObjetosDeValor.WebService.Rest.DocumentoTransporte documentoTransporteQualquer = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.WebService.Rest.DocumentoTransporte>(requestQualquer);

            //var hashUltimo = TestarHashJson(Newtonsoft.Json.JsonConvert.SerializeObject(documentoTransporteUltimo));

            //var hashPrimerio = TestarHashJson(Newtonsoft.Json.JsonConvert.SerializeObject(documentoTransportePrimeiro));

            //var hashQualquer = TestarHashJson(Newtonsoft.Json.JsonConvert.SerializeObject(documentoTransporteQualquer));

            //bool igual = hashPrimerio == hashUltimo;

            //bool diferente = CompareJson(Newtonsoft.Json.JsonConvert.SerializeObject(documentoTransportePrimeiro), Newtonsoft.Json.JsonConvert.SerializeObject(documentoTransporteUltimo));


            string hash = string.Empty;

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(json));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2")); // formato hexadecimal
                }
                hash = builder.ToString();
            }

            return hash;
        }


        private void ProcessarIntegracaoSuperApp(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdm)
        {
            Servicos.Embarcador.SuperApp.IntegracaoSuperApp servicoIntegracaoSuperApp = new Servicos.Embarcador.SuperApp.IntegracaoSuperApp(unitOfWork, unitOfWorkAdm, Cliente);
            //servicoIntegracaoSuperApp.Iniciar();
            Servicos.Embarcador.SuperApp.IntegracaoSuperAppDemaisEventos servicoIntegracaoSuperAppDemaisEventos = new Servicos.Embarcador.SuperApp.IntegracaoSuperAppDemaisEventos(unitOfWork, unitOfWorkAdm, Cliente);
            //servicoIntegracaoSuperAppDemaisEventos.Iniciar();
        }

        private async Task ProcessarCargaEntregaEventoIntegracaoAsync(Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Carga.ControleEntrega.CargaEntregaEventoIntegracao servicoCargaEntregaEventointegracao = new(unitOfWork);
            await servicoCargaEntregaEventointegracao.ProcessarIntegracaoPendenteAsync(24);
        }

        private void ProcessarCargaIntegracaoSAP(Repositorio.UnitOfWork unitOfWork)
        {

            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao> integracoesPendentes = new List<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao>();
            integracoesPendentes.Add(repCargaCargaIntegracao.BuscarPorCargaETipoIntegracao(527, 10)); //Numero carga e tipo integracao
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = (from obj in integracoesPendentes select obj.Carga).Distinct().ToList();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao integracaoPendente in integracoesPendentes)
            {
                new Servicos.Embarcador.Integracao.SAP.IntegracaoSAP(unitOfWork).IntegrarCarga(integracaoPendente);
            }
        }

        private void CalcularDataAgendamentoeCarregamento(Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Carga.ControleEntrega.PrevisaoControleEntrega servpre = new Servicos.Embarcador.Carga.ControleEntrega.PrevisaoControleEntrega(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(3060);

            //servpre.CalcularDataAgendamentoColetaEntregaAutomatico(carga, unitOfWork);

            servpre.CalcularDataAgendamentoColetaAoFecharCargaAutomatico(carga, unitOfWork);
        }

        private void ControleCargaCalculoFrete(Repositorio.UnitOfWork unitOfWork, string stringConexao)
        {
            List<int> cargas = new List<int>();
            cargas.Add(538014);

            Servicos.Embarcador.Carga.Frete.CalcularFreteCargasPendentes(Dominio.Enumeradores.LoteCalculoFrete.Padrao, TipoServicoMultisoftware.MultiTMS, unitOfWork, _conexao.StringConexao, null, cargas);
        }

        private void VerificarCargasPendentesEmissao(Repositorio.UnitOfWork unitOfWork, string stringConexao)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            List<int> cargasPendeciaEmissao = new List<int>(); //repCarga.BuscarCargasPendentesEmissao(100, false);
            cargasPendeciaEmissao.Add(14207);

            for (var i = 0; i < cargasPendeciaEmissao.Count; i++)
            {
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(cargasPendeciaEmissao[i]);
                Servicos.Embarcador.Carga.Documentos serCargaDocumentos = new Servicos.Embarcador.Carga.Documentos(unitOfWork);

                serCargaDocumentos.VerificarPendeciasEmissaoDocumentosCarga(carga, carga.Empresa.Codigo, TipoServicoMultisoftware.MultiTMS, null, Auditado, unitOfWork);
            }
        }

        private void GerarAtendimentoPorGatilhoNaCarga(Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(2022);

            servicoCarga.GerarAtendimentoPorGatilhoNaCarga(carga, TipoMotivoChamadoGatilhoNaCarga.AoSalvarPlacaComModeloVeicularDiferenteDoPrevisto, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador, unitOfWork, Auditado);
        }

        private void TestarImportacaoMercadoLivre(Repositorio.UnitOfWork unitOfWork)
        {
            object lockObterPedido = new object();
            int codigoCargaIntegracaoMercadoLivre = 0;

            if (codigoCargaIntegracaoMercadoLivre == 0)
                return;

            Servicos.Embarcador.Integracao.MercadoLivre.IntegracaoMercadoLivre svcMercadoLivre = new Servicos.Embarcador.Integracao.MercadoLivre.IntegracaoMercadoLivre(Cliente, @"C:\ArquivosIntegracao", @"C:\ArquivosIntegracao", null, 15);
            svcMercadoLivre.ConsultarCargaIntegracaoMercadoLivrePendente(codigoCargaIntegracaoMercadoLivre, TipoServicoMultisoftware.MultiTMS, lockObterPedido, Auditado, unitOfWork, new CancellationToken());
        }

        private void GeocodificarClientes(Repositorio.UnitOfWork unitOfWork)
        {
            //#if DEBUG
            //            unitOfWork.Sessao.CreateSQLQuery("UPDATE T_CLIENTE SET CLI_LATIDUDE = null, CLI_GEOLOCALIZACAO_STATUS = 0 where CLI_CGCCPF in (75315333005178, 1928075017760, 5728108000119, 8001026000166, 1928075017760, 93209765048667, 2968656000470, 2968656000209, 75315333005178, 1928075017760, 93209765048667, 2968656000128, 75315333006220, 75315333006220, 10543788000136)").ExecuteUpdate();
            //#endif
            //SGT.WebAdmin.Models.Threads.ControleRoteirizacao.GerarIntegracaoCoordenadasClientes(unitOfWork);
        }

        private void ValidarRegrasNota(Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Pedido.NotaFiscal serCargaNotaFiscal = new Servicos.Embarcador.Pedido.NotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXmlNota = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmNota = repXmlNota.BuscarPorChave("29230513661609000153550010000053791765273529");
            Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmNota2 = repXmlNota.BuscarPorChave("29230513661609000153550010000053791765273529");

            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCodigo(790316, false);

            bool msgAlertaObservacao;
            bool notaFiscalEmOutraCarga;
            serCargaNotaFiscal.ValidarRegrasNota(xmNota, cargaPedido, TipoServicoMultisoftware, out msgAlertaObservacao, out notaFiscalEmOutraCarga);


        }

        private void ProvisionarNota(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoNota = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Escrituracao.DocumentoProvisao repDocumento = new Repositorio.Embarcador.Escrituracao.DocumentoProvisao(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual repDocumentoEmissaoNFSManual = new Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual(unitOfWork);

            List<int> cargasCod = new List<int>() { 227132, 227292, 227640, 229006, 229844 };

            foreach (var carga in cargasCod)
            {
                unitOfWork.Start();

                var pedidoNota = repPedidoNota.BuscarPorCarga(carga);
                var documentos = repDocumento.BuscarPorCarga(carga, true);
                //Essa busca de documentoProvisao só encontra docs aguardando provisao, se estiver provisionado ou liquidado em um pagamento e passar de novo no loop, vai gerar mais uma provisao pro mesmo pedidoXML
                List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> documentosEmissaoNFSCarga = repDocumentoEmissaoNFSManual.BuscarPorCarga(carga);

                foreach (var nota in pedidoNota)
                    Servicos.Embarcador.Escrituracao.DocumentoProvisao.AdicionarDocumentoParaProvisao(nota, false, TipoServicoMultisoftware, unitOfWork, documentos, documentosEmissaoNFSCarga);

                unitOfWork.FlushAndClear();
                unitOfWork.CommitChanges();
            }

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

        private void RealizarRateioUnilever(Repositorio.UnitOfWork unitOfWork)
        {
            //Servicos.Embarcador.Carga.RateioFrete serRateioNotaFiscal = new Servicos.Embarcador.Carga.RateioFrete(unitOfWork);

            var carga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork).BuscarPorCodigo(180455);
            //var cargaPedidosNaoEmitidos = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork).BuscarPorCarga(carga.Codigo);
            //var config = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork).BuscarConfiguracaoPadrao();
            //List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> cargaPedidosComponentesFreteCarga = new Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete(unitOfWork).BuscarPorCarga(carga.Codigo, false);

            //Aqui define o peso das notas, para posteriormente realizar o cálculo:
            //new Servicos.Embarcador.Carga.Carga(unitOfWork).ConfirmarEnvioDosDocumentos(carga, unitOfWork, unitOfWork.StringConexao, TipoServicoMultisoftware, config, Auditado, Cliente);

            //Calcular o valor de cada pedido:
            //serRateioNotaFiscal.RatearValorDoFrenteEntrePedidos(carga, cargaPedidosNaoEmitidos, config, false, unitOfWork,TipoServicoMultisoftware);
        }

        private void GerarDocumentoDestinadoIntegracao(Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unitOfWork);
                Repositorio.Embarcador.Documentos.DocumentoDestinadoIntegracao repositorioDocumentoDestinadoIntegracao = new Repositorio.Embarcador.Documentos.DocumentoDestinadoIntegracao(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroPesquisaDocumentoDestinadoEmpresa filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroPesquisaDocumentoDestinadoEmpresa()
                {
                    DataEmissaoInicial = DateTime.Now.AddDays(-13)
                };

                var documentosDestinados = repDocumentoDestinadoEmpresa.BuscarPorDatas(filtrosPesquisa);

                foreach (var documento in documentosDestinados)
                {
                    var integracao = repositorioDocumentoDestinadoIntegracao.BuscarPorCodigoETipoDocumentoDestinado((int)documento.Codigo, documento.TipoDocumento);
                    if (integracao == null)
                    {
                        new Servicos.Embarcador.Integracao.Unilever.DocumentoDestinado(unitOfWork).GerarIntegracaoDocumentoDestinado(documento, documento.TipoDocumento);
                    }
                }


            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
            }
        }

        private void EnviarEmailNaoConformidadeTransportador(Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                Servicos.Embarcador.NotaFiscal.NaoConformidade svcNaoConformidade = new Servicos.Embarcador.NotaFiscal.NaoConformidade(unitOfWork);

                svcNaoConformidade.VerificarNaoConformidadesPendentesDeEnvio();
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
            }
        }

        private void EnviarEmailResumoNCPendentes(Repositorio.UnitOfWork unitOfWork)
        {
            try
            {

                Servicos.Embarcador.NotaFiscal.NaoConformidade svcNaoConformidade = new Servicos.Embarcador.NotaFiscal.NaoConformidade(unitOfWork);

                svcNaoConformidade.VerificarNaoConformidadesPendentesDeEnvioResumo();

            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
            }
        }

        private void TestarIntegracaoInfolog(Repositorio.UnitOfWork unitOfWork)
        {
            var integracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork).BuscarPorCodigo(11650);
            new Servicos.Embarcador.Integracao.Infolog.IntegracaoInfolog(unitOfWork, TipoServicoMultisoftware).IntegrarCarga(integracao);
        }

        private void ConsultaCTeDestinado(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Empresa repoEmpresa = new Repositorio.Empresa(unitOfWork);
            string mensagemRetorno = "";
            var empresa = repoEmpresa.BuscarPorCNPJ("01615814006800");
            var url = empresa.Localidade.Estado.SefazCTe.UrlDistribuicaoDFe;
            if (empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao)
                url = empresa.Localidade.Estado.SefazCTeHomologacao.UrlDistribuicaoDFe;

            try
            {
                Servicos.Embarcador.Documentos.DocumentoDestinadoEmpresa.ObterDocumentosDestinadosCTeEmpresa(empresa.Codigo, url, _conexao.StringConexao, 0, ref mensagemRetorno, out string codigoStatusRetornoSefaz, null, TipoServicoMultisoftware.MultiEmbarcador);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }


        }

        private void TestarStatusDocumentoDestinadoUnilever(Repositorio.UnitOfWork unitOfWork)
        {

            Repositorio.Embarcador.Documentos.DocumentoDestinadoIntegracao repositorioDocumentoDestinadoIntegracao = new Repositorio.Embarcador.Documentos.DocumentoDestinadoIntegracao(unitOfWork);

            Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoIntegracao integracaoPendente = repositorioDocumentoDestinadoIntegracao.BuscarPorChaveTipoDocumentoDestinado("42240185401651000146670180000000031000000398", TipoDocumentoDestinadoEmpresa.CTeOSDestinadoTomador);
            //Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoIntegracao integracaoPendente = repositorioDocumentoDestinadoIntegracao.BuscarPorChaveTipoDocumentoDestinado("35231047888128000105570560000004507000045097", TipoDocumentoDestinadoEmpresa.CancelamentoCTe);
            Servicos.Embarcador.Integracao.Unilever.IntegracaoUnilever servicoUnilever = new Servicos.Embarcador.Integracao.Unilever.IntegracaoUnilever(unitOfWork);

            servicoUnilever.IntegrarXmlDocumentoDestinado(integracaoPendente);
        }

        private void TestarIntegracaoLogRisk(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork);
            var cargaIntegracao = repCargaIntegracao.BuscarPorCarga(102);

            Servicos.Embarcador.Integracao.Nstech.IntegracaoSM integracao = new Servicos.Embarcador.Integracao.Nstech.IntegracaoSM(TipoServicoMultisoftware, unitOfWork);
            integracao.IntegrarSM(cargaIntegracao.FirstOrDefault());
        }

        private void TestarIntegracoesUnilever(Repositorio.UnitOfWork unitOfWork)
        {

            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao REPcargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao = REPcargaDadosTransporteIntegracao.BuscarPorCodigo(113153);
            new Servicos.Embarcador.Integracao.Unilever.IntegracaoUnilever(unitOfWork).IntegrarDadosValePedagio(cargaDadosTransporteIntegracao);

            //new Servicos.Embarcador.Carga.Carga(unitOfWork).CriarRegistroIntegracaoConsolidado(carga, unitOfWork, Auditado, TipoServicoMultisoftware, WebServiceConsultaCTe);

            // new Servicos.Embarcador.Carga.Carga(unitOfWork).AvancarEtapaSubCargasConsolidado(carga, unitOfWork, Auditado, TipoServicoMultisoftware, "");

            //testar integracao
            //Repositorio.Embarcador.Cargas.CargaFreteIntegracao repCargafreteIntegracao = new Repositorio.Embarcador.Cargas.CargaFreteIntegracao(unitOfWork);
            //Dominio.Entidades.Embarcador.Cargas.CargaFreteIntegracao integracaoPendente = repCargafreteIntegracao.BuscarPorCodigo(12964, false);

            //new Servicos.Embarcador.Integracao.Unilever.IntegracaoUnilever(unitOfWork).IntegrarLinkNotas(integracaoPendente, unitOfWork, Auditado, TipoServicoMultisoftware, WebServiceConsultaCTe);

            //Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork);
            //Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao integracaoRelevanciaPendente = repCargaIntegracao.BuscarPorCodigo(809);

            //new Servicos.Embarcador.Integracao.Unilever.IntegracaoUnilever(unitOfWork).IntegrarRelevanciCustos(integracaoRelevanciaPendente);

        }

        private void finalizarEnvioNotas(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargaPedido = repCargaPedido.BuscarPorCarga(134943);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in listaCargaPedido)
            {
                decimal pesoNaNFs = 0;
                int volumes = 0;

                Dominio.Entidades.Embarcador.Cargas.CargaPedido carPedido = cargaPedido;

                string retornoFinalizacao = Servicos.WebService.NFe.NotaFiscal.FinalizarEnvioDasNotas(ref carPedido, pesoNaNFs, volumes, null, null, null, null, null, ConfiguracaoEmbarcador, TipoServicoMultisoftware, Auditado, null, unitOfWork);

            }

        }

        private void AdicionarDocumentoProvisao(int codigoProvisao, int codigoPedidoXMLNotaFiscal, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Escrituracao.Provisao repositorioProvisao = new Repositorio.Embarcador.Escrituracao.Provisao(unitOfWork);
            Repositorio.Embarcador.Escrituracao.DocumentoProvisao repositorioDocumentoProvisao = new Repositorio.Embarcador.Escrituracao.DocumentoProvisao(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Servicos.Embarcador.Escrituracao.Provisao servicoProvisao = new Servicos.Embarcador.Escrituracao.Provisao(unitOfWork);

            Dominio.Entidades.Embarcador.Escrituracao.Provisao provisao = repositorioProvisao.BuscarPorCodigo(codigoProvisao);
            Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = repositorioPedidoXMLNotaFiscal.BuscarPorCodigo(codigoPedidoXMLNotaFiscal);

            if (pedidoXMLNotaFiscal.CargaPedido.StageRelevanteCusto == null)
            {
                Repositorio.Embarcador.Pedidos.PedidoStage repositorioPedidoStage = new Repositorio.Embarcador.Pedidos.PedidoStage(unitOfWork);
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoStage> stagesPedido = repositorioPedidoStage.BuscarPorPedidoECargaStage(pedidoXMLNotaFiscal.CargaPedido.Pedido.Codigo, provisao.Carga.Codigo);

                pedidoXMLNotaFiscal.CargaPedido.StageRelevanteCusto = Servicos.Embarcador.Pedido.Stage.ObterStageMaisRelevante(stagesPedido);

                repositorioCargaPedido.Atualizar(pedidoXMLNotaFiscal.CargaPedido);
            }

            if (pedidoXMLNotaFiscal.CargaPedido.StageRelevanteCusto == null)
                return;

            Servicos.Embarcador.Escrituracao.DocumentoProvisao.AdicionarDocumentoParaProvisao(pedidoXMLNotaFiscal, xmlNotaFiscalFilialEmissora: false, TipoServicoMultisoftware, unitOfWork);

            Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao documentoProvisao = repositorioDocumentoProvisao.BuscarPorXMLNotaFiscalECarga(pedidoXMLNotaFiscal.XMLNotaFiscal.Codigo, provisao.Carga.Codigo);

            documentoProvisao.Provisao = provisao;
            documentoProvisao.Situacao = SituacaoProvisaoDocumento.EmFechamento;

            provisao.ValorProvisao += documentoProvisao.ValorProvisao;
            provisao.QuantidadeDocsProvisao += 1;
            provisao.Situacao = SituacaoProvisao.EmFechamento;
            provisao.GerandoMovimentoFinanceiroProvisao = true;

            repositorioDocumentoProvisao.Atualizar(documentoProvisao);
            repositorioProvisao.Atualizar(provisao);

            servicoProvisao.ProcessarProvisoesEmFechamento();
            servicoProvisao.GerarFechamentoProvisaoIndividual(provisao);
        }

        private void transferirctesCargas(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            Servicos.Embarcador.Carga.Carga servcarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(134003);

            servcarga.TransferirCtesEmitidosParaCargasDeSubTrecho(carga, unitOfWork);
        }

        private void TestarRateioFrete(Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Carga.RateioFrete serRateioFrete = new Servicos.Embarcador.Carga.RateioFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargapedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();
            //Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(107135);

            //List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargapedido.BuscarPorCarga(carga.Codigo);

            //serRateioFrete.RatearValorDoFrenteEntrePedidos(carga, cargaPedidos, configuracao, false, unitOfWork, TipoServicoMultisoftware);

        }

        private void TestarIntegracaoPesagem(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.PesagemIntegracao repPesagemIntegracao = new Repositorio.Embarcador.Logistica.PesagemIntegracao(unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.PesagemIntegracao integracaoPesagem = repPesagemIntegracao.BuscarPorCodigo(160);

            Servicos.Embarcador.Integracao.Deca.IntegracaoDeca svcDeca = new Servicos.Embarcador.Integracao.Deca.IntegracaoDeca(unitOfWork);
            svcDeca.ConsultarPesagensBalanca(integracaoPesagem);

            /*Dominio.Entidades.Embarcador.Logistica.Pesagem pesagem = integracaoPesagem.Pesagem;
            pesagem.PesoInicial = 2658;
            Servicos.Embarcador.Hubs.FluxoPatio hubFluxoPatio = new Servicos.Embarcador.Hubs.FluxoPatio();
            hubFluxoPatio.InformarPesagemInicialAtualizada(pesagem);*/
        }

        private void RecalcularFreteTabelaFrete(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(7261);

            Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repositorioConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = repositorioConfiguracaoPedido.BuscarConfiguracaoPadrao();

            Servicos.Embarcador.Carga.Frete servicoCargaFrete = new Servicos.Embarcador.Carga.Frete(unitOfWork, TipoServicoMultisoftware);
            servicoCargaFrete.RecalcularFreteTabelaFrete(carga, carga.TabelaFreteRota?.Codigo ?? 0, unitOfWork, ConfiguracaoEmbarcador, configuracaoPedido);
        }

        private void GerarEntregasCarga(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaRotaFrete repCargaRotaFrete = new Repositorio.Embarcador.Cargas.CargaRotaFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem repCargaRotaFretePontosPassagem = new Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(5313463);
            Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete = repCargaRotaFrete.BuscarPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> cargaPedidosXMLs = repPedidoXMLNotaFiscal.BuscarPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosPassagens = repCargaRotaFretePontosPassagem.BuscarPorCarga(carga.Codigo);

            Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.GerarCargaEntrega(carga, cargaPedidos, cargaPedidosXMLs, cargaRotaFrete, pontosPassagens, true, ConfiguracaoEmbarcador, unitOfWork, TipoServicoMultisoftware);
        }

        private void RoteirizarEgerarEntregasCarga(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(1186370);



            Servicos.Embarcador.Carga.CargaRotaFrete.GerarIntegracoesRoteirizacaoCarga(carga, unitOfWork, ConfiguracaoEmbarcador, TipoServicoMultisoftware, true);

        }

        private void GerarEntregaNota(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal repCargaEntregaNota = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(166968);
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaEntregaPedidos = repCargaEntregaPedido.BuscarPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidosXmlNotaFiscalCarga = repPedidoXMLNotaFiscal.BuscarPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> entregasSemNota = repCargaEntrega.BuscarEntregasSemNotaPorCarga(carga.Codigo);

            foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega entrega in entregasSemNota)
            {
                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaEntregaPedidoSemNota = cargaEntregaPedidos.Where(x => x.CargaEntrega.Codigo == entrega.Codigo).ToList();
                List<int> pedidos = cargaEntregaPedidoSemNota.Select(x => x.CargaPedido.Pedido.Codigo).ToList();

                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidosXmlNotaFiscalAdd = pedidosXmlNotaFiscalCarga.Where(x => pedidos.Contains(x.CargaPedido.Pedido.Codigo)).ToList();

                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal in pedidosXmlNotaFiscalAdd)
                {
                    Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal cargaEntregaNotaFiscal = new Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal
                    {
                        CargaEntrega = entrega,
                        PedidoXMLNotaFiscal = pedidoXMLNotaFiscal
                    };

                    repCargaEntregaNota.Inserir(cargaEntregaNotaFiscal);
                }
            }

        }

        private void SetarRotaCarga(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(222419);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotaFiscals = repPedidoXMLNotaFiscal.BuscarPorCarga(carga.Codigo);

            Servicos.Embarcador.Carga.RotaFrete.SetarRotaCarga(ref carga, cargaPedidos, pedidoXMLNotaFiscals, carga.Rota, ConfiguracaoEmbarcador, unitOfWork, TipoServicoMultisoftware);

        }

        private void SetarRotaFreteCarga(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(222419);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotaFiscals = repPedidoXMLNotaFiscal.BuscarPorCarga(carga.Codigo);

            Servicos.Embarcador.Carga.RotaFrete.SetarRotaFreteCarga(carga, cargaPedidos, ConfiguracaoEmbarcador, unitOfWork, TipoServicoMultisoftware, false);
        }

        private void FinalizarEntrega(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();

            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repCargaEntrega.BuscarPorCodigo(2732931);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(unitOfWork).BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacaoParametro = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork).BuscarPorCodigoFetch(cargaEntrega.Carga.TipoOperacao?.Codigo ?? 0);

            Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.FinalizarEntrega(cargaEntrega, DateTime.Now, new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint { Latitude = 0, Longitude = 0 }, null, 0, "", configuracao, TipoServicoMultisoftware, null, OrigemSituacaoEntrega.App, Cliente, unitOfWork, false, configuracaoControleEntrega, tipoOperacaoParametro);
        }

        //private void TestarZonasExclusaoRota(Repositorio.UnitOfWork unitOfWork)
        //{
        //    //Se chegou aqui.. é porque possui restrição, então vamos validar item a item...
        //    // Para identificar entre qual "Parada" existe a restrição.... para desviar...
        //    Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
        //    Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();
        //    Servicos.Embarcador.Logistica.Roteirizacao rota = new Servicos.Embarcador.Logistica.Roteirizacao(configuracaoIntegracao.ServidorRouteOSM);

        //    rota.Clear();
        //    //rota.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint() { Lat = -27.366129, Lng = -52.771614, Descricao = "Nonoai" }); // Nonoai..
        //    //rota.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint() { Lat = -26.955358, Lng = -52.534262, Descricao = "Xaxim" }); // Xaxim
        //    //rota.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint() { Lat = -26.869820, Lng = -52.398691, Descricao = "Xanxere" }); // Xxe
        //    //rota.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint() { Lat = -27.366129, Lng = -52.771614, Descricao = "Nonoai" }); // Nonoai..

        //    rota.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint() { Lat = -25.365704, Lng = -51.485539, Descricao = "Guarapuava-PR" });  // Guarapuava-PR..
        //    rota.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint() { Lat = -24.802892, Lng = -49.999338, Descricao = "Castro-PR" });      // Castro-PR..

        //    Servicos.Embarcador.Logistica.OpcoesRoteirizar opcoesRoteirizacao = new Servicos.Embarcador.Logistica.OpcoesRoteirizar { AteOrigem = false, Ordenar = false, PontosNaRota = false };
        //    var respostaTrecho = rota.Roteirizar(opcoesRoteirizacao);

        //    var novaResposta = Servicos.Embarcador.Carga.CargaRotaFrete.AnalisarGerarRoteirizacaoComDesvioZonaExclusao(respostaTrecho, unitOfWork);
        //}

        //private void TestarTrechoBalsaRota(Repositorio.UnitOfWork unitOfWork)
        //{
        //    //Se chegou aqui.. é porque possui restrição, então vamos validar item a item...
        //    // Para identificar entre qual "Parada" existe a restrição.... para desviar...
        //    Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
        //    Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();
        //    Servicos.Embarcador.Logistica.Roteirizacao rota = new Servicos.Embarcador.Logistica.Roteirizacao(configuracaoIntegracao.ServidorRouteOSM);

        //    rota.Clear();
        //    //rota.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint() { Lat = -27.366129, Lng = -52.771614, Descricao = "Nonoai" }); // Nonoai..
        //    //rota.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint() { Lat = -26.955358, Lng = -52.534262, Descricao = "Xaxim" }); // Xaxim
        //    //rota.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint() { Lat = -26.869820, Lng = -52.398691, Descricao = "Xanxere" }); // Xxe
        //    //rota.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint() { Lat = -27.366129, Lng = -52.771614, Descricao = "Nonoai" }); // Nonoai..

        //    rota.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint() { Lat = -23.565607, Lng = -46.644534, Descricao = "SÃO PAULO-SP" });  // Guarapuava-PR..
        //    rota.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint() { Lat = -3.091589, Lng = -60.009654, Descricao = "MANAUS-PA" });      // Castro-PR..

        //    Servicos.Embarcador.Logistica.OpcoesRoteirizar opcoesRoteirizacao = new Servicos.Embarcador.Logistica.OpcoesRoteirizar { AteOrigem = false, Ordenar = false, PontosNaRota = false };

        //    Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint wayPointDestinoRemovido = null;
        //    Dominio.Entidades.Embarcador.Logistica.TrechoBalsa trechoBalsa = Servicos.Embarcador.Carga.CargaRotaFrete.AnalisarGerarRoteirizacaoComTrechoBalsa(rota, ref wayPointDestinoRemovido, unitOfWork);

        //    var respostaTrecho = rota.Roteirizar(opcoesRoteirizacao);

        //    respostaTrecho = Servicos.Embarcador.Carga.CargaRotaFrete.AnalisarGerarRoteirizacaoAdicionarTrechoBalsa(respostaTrecho, trechoBalsa, wayPointDestinoRemovido, unitOfWork);

        //    var novaResposta = Servicos.Embarcador.Carga.CargaRotaFrete.AnalisarGerarRoteirizacaoComDesvioZonaExclusao(respostaTrecho, unitOfWork);
        //}

        private void TestarIntegracaoIntercab(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTeManualIntegracao repCargaCTeManualIntegracao = new Repositorio.Embarcador.Cargas.CargaCTeManualIntegracao(unitOfWork);
            Servicos.Embarcador.Integracao.Intercab.IntegracaoIntercab servicoIntegracaoIntercab = new Servicos.Embarcador.Integracao.Intercab.IntegracaoIntercab(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaCTeManualIntegracao cargaCTeManualIntegracao = repCargaCTeManualIntegracao.BuscarPorCodigo(4);

            servicoIntegracaoIntercab.IntegrarCargaCTeManual(cargaCTeManualIntegracao);
        }

        private void GerarPagamentoMotoristaEmbarcador(Repositorio.UnitOfWork unitOfWork)
        {
            unitOfWork.Start();

            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(7827);

            Servicos.Embarcador.Integracao.Diaria.DiariaMotorista servicoDiariaMotorista = new Servicos.Embarcador.Integracao.Diaria.DiariaMotorista(unitOfWork);
            servicoDiariaMotorista.GerarPagamentoMotoristaEmbarcador(carga, ConfiguracaoEmbarcador);

            unitOfWork.Rollback();
        }

        private void TesteComprarValePedagioDBTrans(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(unitOfWork);
            Servicos.Embarcador.Integracao.DBTrans.ValePedagio serValePedagioDBTrans = new Servicos.Embarcador.Integracao.DBTrans.ValePedagio(unitOfWork);

            unitOfWork.Start();

            Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio = repCargaValePedagio.BuscarPorCodigo(716);

            Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaValePedagio.Carga;

            if (cargaValePedagio.SituacaoValePedagio != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.Comprada)
                serValePedagioDBTrans.GerarCompraValePedagio(cargaValePedagio, carga, TipoServicoMultisoftware);

            unitOfWork.Rollback();
        }

        private void GerarCargaValePedagioPorRotaFrete(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

            unitOfWork.Start();

            Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(42029);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);

            Servicos.Embarcador.Carga.ValePedagio.CargaValePedagioRota.CriarCargaValePedagioPorRotaFrete(carga, cargaPedidos, ConfiguracaoEmbarcador, unitOfWork, TipoServicoMultisoftware);

            unitOfWork.Rollback();
        }

        public Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>> GerarEncaixeRetira()
        {
            string str = "{'Pedidos':[{'CodigoIntegracao':16813,'IdProposta':0,'IdLote':0,'Produtos':[{'CodigoIntegracao':'0107.00013.0015FD','Quantidade':5.000,'Peso':0.0},{'CodigoIntegracao':'0107.00000.0015FD','Quantidade':5.000,'Peso':0.0},{'CodigoIntegracao':'0107.00020.0015FD','Quantidade':5.000,'Peso':0.0},{'CodigoIntegracao':'0107.00045.0015FD','Quantidade':3.000,'Peso':0.0},{'CodigoIntegracao':'0001.00001.0020PA','Quantidade':240.000,'Peso':0.0}]},{'CodigoIntegracao':16814,'IdProposta':0,'IdLote':0,'Produtos':[{'CodigoIntegracao':'0630.18X50.0001CX','Quantidade':1.000,'Peso':0.0},{'CodigoIntegracao':'0529.00077.0001PC','Quantidade':12.000,'Peso':0.0},{'CodigoIntegracao':'0580.00077.0001CX','Quantidade':1.000,'Peso':0.0},{'CodigoIntegracao':'0498.99999.0001CX','Quantidade':1.000,'Peso':0.0},{'CodigoIntegracao':'0102.00001.0020PL','Quantidade':80.000,'Peso':0.0},{'CodigoIntegracao':'0003.00001.0020PL','Quantidade':30.000,'Peso':0.0},{'CodigoIntegracao':'0107.00015.0015FD','Quantidade':2.000,'Peso':0.0},{'CodigoIntegracao':'0107.00000.0015FD','Quantidade':2.000,'Peso':0.0}]},{'CodigoIntegracao':16815,'IdProposta':0,'IdLote':0,'Produtos':[{'CodigoIntegracao':'0110.00020.0002PT','Quantidade':8.000,'Peso':0.0}]},{'CodigoIntegracao':16816,'IdProposta':0,'IdLote':0,'Produtos':[{'CodigoIntegracao':'0001.00001.0020PA','Quantidade':378.000,'Peso':0.0}]}],'Transportador':{'Atividade':0,'CNPJ':null,'RazaoSocial':'PAULO ROBERTO','NomeFantasia':null,'CodigoIntegracao':'1158162   ','IE':null,'InscricaoMunicipal':null,'InscricaoST':null,'RNTRC':null,'SimplesNacional':false,'Emails':null,'EmissaoDocumentosForaDoSistema':false,'LiberacaoParaPagamentoAutomatico':false,'Endereco':{'Cidade':{'Codigo':0,'Descricao':null,'IBGE':0,'CodigoIntegracao':null,'SiglaUF':'BA','Pais':{'CodigoPais':0,'NomePais':null,'SiglaPais':'BR'},'Regiao':null,'CodigoDocumento':null,'Atualizar':false},'Logradouro':'RUA PORTO SEGURO','Numero':'20','Complemento':null,'CEP':'400000000','CEPSemFormato':null,'Bairro':null,'DDDTelefone':null,'Telefone':null,'Telefone2':null,'CodigoIntegracao':null,'InscricaoEstadual':null,'Latitude':null,'Longitude':null,'UF':'BA','CodigoTelefonicoPais':null},'DadosBancarios':null,'Certificado':null,'CodigoDocumento':null,'RegimeTributario':0,'Ativo':null},'DataHoraEncaixe':'20/09/2021 19:25:59','Motorista':{'Codigo':0,'CPF':'99543168504','CategoriaCNH':null,'CodigoIntegracao':'1158162','Nome':'PAULO ROBERTO','DataHabilitacao':null,'DataAdmissao':null,'DataNascimento':null,'DataValidadeGR':null,'tipoMotorista':0,'RG':null,'NumeroHabilitacao':null,'DadosBancarios':null,'Email':null,'DataVencimentoHabilitacao':null,'Endereco':{'Cidade':{'Codigo':0,'Descricao':null,'IBGE':0,'CodigoIntegracao':null,'SiglaUF':'BA','Pais':{'CodigoPais':0,'NomePais':null,'SiglaPais':'BR'},'Regiao':null,'CodigoDocumento':null,'Atualizar':false},'Logradouro':'RUA PORTO SEGURO','Numero':'20','Complemento':null,'CEP':'400000000','CEPSemFormato':null,'Bairro':null,'DDDTelefone':null,'Telefone':null,'Telefone2':null,'CodigoIntegracao':null,'InscricaoEstadual':null,'Latitude':null,'Longitude':null,'UF':'BA','CodigoTelefonicoPais':null},'Ativo':null,'MotivoBloqueio':null,'DataSuspensaoInicio':null,'DataSuspensaoFim':null,'Transportador':null,'NumeroCartao':null},'Veiculo':{'Placa':'PSW1F25','Renavam':null,'UF':null,'RNTC':null,'Tara':0,'CapacidadeKG':0,'CapacidadeM3':0,'NumeroFrota':null,'NumeroChassi':null,'NumeroMotor':null,'DataAquisicao':null,'AnoFabricacao':0,'AnoModelo':0,'Ativo':false,'Transportador':null,'TipoCarroceria':0,'TipoVeiculo':1,'TipoRodado':0,'ModeloVeicular':null,'GrupoPessoaSegmento':null,'TipoPropriedadeVeiculo':1,'Proprietario':null,'Motoristas':null,'Modelo':null,'Codigo':0,'Reboques':null,'XTexto':null,'XCampo':null,'MotivoBloqueio':null,'DataSuspensaoInicio':null,'DataSuspensaoFim':null,'PossuiTagValePedagio':null,'OrdemReboque':0,'DataRetiradaCtrn':null,'NumeroContainer':null,'TaraContainer':0,'MaxGross':0,'Anexos':null,'ValorContainerAverbacao':0.0},'TipoOperacao':{'CodigoIntegracao':'99','Descricao':null,'CNPJsDaOperacao':null,'BloquearEmissaoDosDestinatario':false,'BloquearEmissaoDeEntidadeSemCadastro':false,'TipoCobrancaMultimodal':0,'ModalPropostaMultimodal':0,'TipoServicoMultimodal':0,'TipoPropostaMultimodal':0,'CNPJsDestinatariosNaoAutorizados':null,'Atualizar':false},'ModeloVeicular':{'CodigoIntegracao':'2000867           ','Descricao':null,'TipoModeloVeicular':0,'DivisaoCapacidade':null},'Filial':null,'TipoCarga':null}";

            Dominio.ObjetosDeValor.WebService.Carregamento.EncaixeRetira.Carregamento carregamentoIntegracao = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.WebService.Carregamento.EncaixeRetira.Carregamento>(str);

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                unitOfWork.Start();

                Servicos.Embarcador.Pedido.RetiradaProduto servicoRetiradaProduto = new Servicos.Embarcador.Pedido.RetiradaProduto(unitOfWork, TipoServicoMultisoftware, null, configuracaoTMS, Auditado);
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento = servicoRetiradaProduto.PreencherCarregamentoIntegracao(carregamentoIntegracao);
                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = servicoRetiradaProduto.GerarCarga(carregamento.Codigo);

                List<Dominio.ObjetosDeValor.WebService.Carga.Protocolos> retorno = new List<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>() { };
                string mensagemRetornoCarga = "";

                foreach (var carga in cargas)
                {
                    mensagemRetornoCarga = $"CARGA:{carga.CodigoCargaEmbarcador}";

                    retorno.Add(new Dominio.ObjetosDeValor.WebService.Carga.Protocolos()
                    {
                        protocoloIntegracaoCarga = carga.Protocolo,
                    });
                }

                unitOfWork.CommitChanges();

                return Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>>.CriarRetornoSucesso(retorno);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);

                return Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);

                return Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>>.CriarRetornoExcecao($"Ocorreu uma falha ao gerar o carregamento");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private async Task AdicionarCargaAsync()
        {
            string strCarga = "{'ProtocoloCarga':0,'ProtocoloPedido':0,'NumeroCarga':'1771859','NumeroPreCarga':null,'IdentificacaoAdicional':null,'NumeroPedidoEmbarcador':'350671','CFOP':0,'ValorFreteCalculado':false,'Filial':{'CodigoIntegracao':'914','Descricao':null,'CNPJ':null,'TipoFilial':0,'CodigoAtividade':0,'Endereco':null,'Ativo':false},'FilialVenda':null,'TransportadoraEmitente':{'Atividade':0,'CNPJ':'23236527000115','RazaoSocial':null,'NomeFantasia':null,'CodigoIntegracao':null,'IE':null,'InscricaoMunicipal':null,'InscricaoST':null,'RNTRC':null,'SimplesNacional':false,'Emails':null,'EmissaoDocumentosForaDoSistema':false,'LiberacaoParaPagamentoAutomatico':false,'Endereco':null,'DadosBancarios':null,'Certificado':null,'CodigoDocumento':null,'RegimeTributario':0,'Ativo':null},'UsarOutroEnderecoOrigem':false,'Origem':null,'UsarOutroEnderecoDestino':false,'Destino':null,'RegiaoDestino':null,'Fronteira':null,'PontoPartida':null,'Remetente':{'ClienteExterior':false,'CPFCNPJ':'06057223036010','CodigoIntegracao':'914','TipoPessoa':1,'CodigoAtividade':3,'RGIE':'143121530','IM':null,'RazaoSocial':'SENDAS DISTRIBUIDORA DEP 914','NomeFantasia':'SENDAS 914 CD FEIRA DE SANTANA','CNAE':null,'Endereco':{'Cidade':{'Codigo':0,'Descricao':null,'IBGE':2910800,'CodigoIntegracao':null,'SiglaUF':null,'Pais':null,'Regiao':null,'CodigoDocumento':null,'Atualizar':false},'Logradouro':'AVENIDA EDUARDO FRÓES DA MOTA','Numero':'0','Complemento':'','CEP':'44021215','CEPSemFormato':null,'Bairro':'SOBRADINHO','DDDTelefone':null,'Telefone':'','Telefone2':null,'CodigoIntegracao':null,'InscricaoEstadual':'143121530','Latitude':null,'Longitude':null,'UF':null,'CodigoTelefonicoPais':null},'Email':null,'AtualizarEnderecoPessoa':false,'EmailFatura':null,'Codigo':null,'CodigoCategoria':null,'CodigoDocumento':null,'RNTRC':null,'NumeroCartaoCIOT':null,'GerarCIOT':null,'TipoFavorecidoCIOT':null,'TipoPagamentoCIOT':null,'GrupoPessoa':null,'ExigirNumeroControleCliente':false,'ExigirNumeroNumeroReferenciaCliente':false,'TipoEmissaoCTeDocumentosExclusivo':0,'ParametroRateioFormulaExclusivo':0,'AdicionarComoOutroEndereco':false,'InativarCliente':false,'CodigoDocumentoFornecedor':null,'Cliente':false,'Fornecedor':false,'RaioEmMetros':0,'RegimeTributario':0,'ExigeAgendamento':null},'Destinatario':{'ClienteExterior':false,'CPFCNPJ':'06057223031565','CodigoIntegracao':'66','TipoPessoa':1,'CodigoAtividade':3,'RGIE':'131710674','IM':null,'RazaoSocial':'SENDAS DISTRIBUIDORA S/A LJ66','NomeFantasia':'066 JUAZEIRO','CNAE':null,'Endereco':{'Cidade':{'Codigo':0,'Descricao':null,'IBGE':2918407,'CodigoIntegracao':null,'SiglaUF':null,'Pais':null,'Regiao':null,'CodigoDocumento':null,'Atualizar':false},'Logradouro':'AV SAO JOAO','Numero':'1947','Complemento':'','CEP':'48900441','CEPSemFormato':null,'Bairro':'JARDIM VITORIA','DDDTelefone':null,'Telefone':'1134115000','Telefone2':null,'CodigoIntegracao':null,'InscricaoEstadual':'131710674','Latitude':null,'Longitude':null,'UF':null,'CodigoTelefonicoPais':null},'Email':null,'AtualizarEnderecoPessoa':false,'EmailFatura':null,'Codigo':null,'CodigoCategoria':null,'CodigoDocumento':null,'RNTRC':null,'NumeroCartaoCIOT':null,'GerarCIOT':null,'TipoFavorecidoCIOT':null,'TipoPagamentoCIOT':null,'GrupoPessoa':null,'ExigirNumeroControleCliente':false,'ExigirNumeroNumeroReferenciaCliente':false,'TipoEmissaoCTeDocumentosExclusivo':0,'ParametroRateioFormulaExclusivo':0,'AdicionarComoOutroEndereco':false,'InativarCliente':false,'CodigoDocumentoFornecedor':null,'Cliente':false,'Fornecedor':false,'RaioEmMetros':0,'RegimeTributario':0,'ExigeAgendamento':null},'Tomador':null,'Recebedor':null,'Expedidor':null,'FreteRota':null,'CanalEntrega':{'Descricao':null,'CodigoIntegracao':'AN','Filial':null},'Deposito':null,'DataInicioCarregamento':'03/08/2021 18:00:11','DataFinalCarregamento':null,'DataTerminoCarregamento':null,'DataAgendamento':null,'DataColeta':null,'DataPrevisaoEntrega':null,'HoraPrevisaoEntrega':null,'PrevisaoEntregaTransportador':null,'EntregaAgendada':false,'SenhaAgendamentoEntrega':null,'DataCriacaoCarga':null,'DataCancelamentoCarga':null,'DataAnulacaoCarga':null,'DataUltimaLiberacao':null,'UsuarioCriacaoRemessa':null,'NumeroOrdem':null,'NumeroPaletes':0,'NumeroPaletesPagos':0.0,'NumeroSemiPaletes':0.0,'NumeroSemiPaletesPagos':0.0,'NumeroPaletesFracionado':0.0,'NumeroCombis':0.0,'NumeroCombisPagas':0.0,'PesoTotalPaletes':0.0,'ValorTotalPaletes':0.0,'PesoBruto':40.96,'PesoLiquido':0.0,'CubagemTotal':0.0,'QuantidadeVolumes':40,'Distancia':0.0,'Produtos':[{'CodigoProduto':'1115692','DescricaoProduto':'ALIM P/CAES BONZO AD 1KG CAR/CER','CodigoGrupoProduto':'BAZAR','CodigocEAN':null,'DescricaoGrupoProduto':'BAZAR','ValorUnitario':7.23359,'PesoUnitario':1.024,'Quantidade':2.0,'QuantidadePlanejada':0.0,'UnidadeMedida':null,'QuantidadeEmbalagem':10.0,'PesoTotalEmbalagem':10.24,'MetroCubito':0.0,'CodigoDocumentacao':null,'Observacao':null,'InativarCadastro':false,'Atualizar':true,'SetorLogistica':null,'ClasseLogistica':null,'CodigoNCM':null,'ProdutoLotes':null,'ProdutoDivisoesCapacidade':null,'QuantidadePallet':0.04762,'QuantidadePorCaixa':10,'QuantidadeCaixasVazias':0,'QuantidadeCaixasVaziasPlanejadas':0,'QuantidadeCaixaPorPallet':42,'Lastro':6.0,'Camada':7.0,'Altura':15.0,'Largura':35.0,'Comprimento':56.0,'LinhaSeparacao':null,'TipoEmbalagem':null,'PalletFechado':false,'CSTICMS':null,'OrigemMercadoria':null,'CodigoNFCI':null,'CanalDistribuicao':null,'SiglaModalidade':null,'Imuno':null},{'CodigoProduto':'1115692','DescricaoProduto':'ALIM P/CAES BONZO AD 1KG CAR/CER','CodigoGrupoProduto':'BAZAR','CodigocEAN':null,'DescricaoGrupoProduto':'BAZAR','ValorUnitario':7.23359,'PesoUnitario':1.024,'Quantidade':2.0,'QuantidadePlanejada':0.0,'UnidadeMedida':null,'QuantidadeEmbalagem':10.0,'PesoTotalEmbalagem':10.24,'MetroCubito':0.0,'CodigoDocumentacao':null,'Observacao':null,'InativarCadastro':false,'Atualizar':true,'SetorLogistica':null,'ClasseLogistica':null,'CodigoNCM':null,'ProdutoLotes':null,'ProdutoDivisoesCapacidade':null,'QuantidadePallet':0.04762,'QuantidadePorCaixa':10,'QuantidadeCaixasVazias':0,'QuantidadeCaixasVaziasPlanejadas':0,'QuantidadeCaixaPorPallet':42,'Lastro':6.0,'Camada':7.0,'Altura':15.0,'Largura':35.0,'Comprimento':56.0,'LinhaSeparacao':null,'TipoEmbalagem':null,'PalletFechado':false,'CSTICMS':null,'OrigemMercadoria':null,'CodigoNFCI':null,'CanalDistribuicao':null,'SiglaModalidade':null,'Imuno':null}],'ProdutoPredominante':null,'UtilizarTipoTomadorInformado':true,'TipoTomador':3,'TipoPagamento':0,'Motoristas':[{'Codigo':0,'CPF':'55261112028','CategoriaCNH':null,'CodigoIntegracao':null,'Nome':'DOMINGO SANTOS','DataHabilitacao':null,'DataAdmissao':null,'DataNascimento':null,'DataValidadeGR':null,'tipoMotorista':0,'RG':null,'NumeroHabilitacao':null,'DadosBancarios':null,'Email':null,'DataVencimentoHabilitacao':null,'Endereco':null,'Ativo':null,'MotivoBloqueio':null,'DataSuspensaoInicio':null,'DataSuspensaoFim':null,'Transportador':null,'NumeroCartao':null}],'Veiculo':{'Placa':'YYY1234','Renavam':null,'UF':null,'RNTC':null,'Tara':0,'CapacidadeKG':0,'CapacidadeM3':0,'NumeroFrota':null,'NumeroChassi':null,'NumeroMotor':null,'DataAquisicao':null,'AnoFabricacao':0,'AnoModelo':0,'Ativo':false,'Transportador':null,'TipoCarroceria':0,'TipoVeiculo':1,'TipoRodado':0,'ModeloVeicular':null,'GrupoPessoaSegmento':null,'TipoPropriedadeVeiculo':1,'Proprietario':null,'Motoristas':null,'Modelo':null,'Codigo':0,'Reboques':null,'XTexto':null,'XCampo':null,'MotivoBloqueio':null,'DataSuspensaoInicio':null,'DataSuspensaoFim':null,'PossuiTagValePedagio':null,'OrdemReboque':0,'DataRetiradaCtrn':null,'NumeroContainer':null,'TaraContainer':0,'MaxGross':0,'Anexos':null,'ValorContainerAverbacao':0.0},'VeiculoDaNota':null,'ModeloVeicular':null,'TipoOperacao':{'CodigoIntegracao':'TRANSF_CD_LOJA','Descricao':null,'CNPJsDaOperacao':null,'BloquearEmissaoDosDestinatario':false,'BloquearEmissaoDeEntidadeSemCadastro':false,'TipoCobrancaMultimodal':0,'ModalPropostaMultimodal':0,'TipoServicoMultimodal':0,'TipoPropostaMultimodal':0,'CNPJsDestinatariosNaoAutorizados':null,'Atualizar':false},'FuncionarioVendedor':null,'FuncionarioSupervisor':null,'FuncionarioGerente':null,'TipoCargaEmbarcador':{'CodigoIntegracao':'SECA','Descricao':null,'CNPJsDoTipoCargaNoEmbarcador':null,'ClasseONU':null,'SequenciaONU':null,'CodigoPSNONU':null,'ObservacaoONU':null},'ValorFrete':null,'FecharCargaAutomaticamente':false,'ViagemJaFoiFinalizada':false,'PedidoPallet':false,'Observacao':null,'ObservacaoInterna':null,'ObservacaoCTe':null,'CodigoAgrupamento':null,'ObservacaoLocalEntrega':null,'TipoRateioProdutos':0,'ImpressoraNumero':null,'Lacres':null,'ValorDescarga':0.0,'ValorPedagio':0.0,'NotasFiscais':[{'Protocolo':0,'Chave':'29210806057223036010553000000167951177969041','Rota':null,'Numero':16795,'Serie':'300','Modelo':'55','Valor':3837.17,'BaseCalculoICMS':0.0,'ValorICMS':0.0,'BaseCalculoST':0.0,'ValorFreteLiquido':0.0,'ValorFrete':0.0,'ValorST':0.0,'ValorTotalProdutos':0.0,'ValorSeguro':0.0,'ValorDesconto':0.0,'ValorImpostoImportacao':0.0,'ValorPIS':0.0,'ValorCOFINS':0.0,'ValorOutros':0.0,'ValorIPI':0.0,'PesoBruto':489.5,'PesoLiquido':452.944,'Cubagem':0.0,'MetroCubico':0.0,'VolumesTotal':50.0,'DataEmissao':'03/08/2021','NaturezaOP':null,'QuantidadePallets':0.0,'InformacoesComplementares':null,'TipoDocumento':null,'TipoDeCarga':null,'NumeroCarregamento':null,'ModalidadeFrete':0,'Emitente':null,'Destinatario':null,'Recebedor':null,'Expedidor':null,'Tomador':null,'Transportador':null,'Veiculo':null,'Volumes':null,'TipoCarga':null,'TipoOperacaoNotaFiscal':1,'SituacaoNFeSefaz':0,'Produtos':null,'Canhoto':null,'NumeroDT':null,'DataEmissaoDT':null,'CodigoIntegracaoCliente':null,'NumeroSolicitacao':null,'NumeroPedido':null,'NumeroRomaneio':null,'SubRota':null,'GrauRisco':null,'AliquotaICMS':0.0,'Observacao':null,'DataPrevisao':null,'KMRota':0.0,'ValorComponenteFreteCrossDocking':0.0,'ValorComponenteAdValorem':0.0,'ValorComponenteDescarga':0.0,'ValorComponentePedagio':0.0,'ValorComponenteAdicionalEntrega':0.0,'NCMPredominante':null,'CodigoProduto':null,'CFOPPredominante':null,'NumeroControleCliente':null,'NumeroCanhoto':null,'NumeroReferenciaEDI':null,'PINSuframa':null,'DescricaoMercadoria':null,'PesoAferido':0.0,'TipoOperacao':null,'ModeloVeicular':null,'ObsPlaca':null,'ObsTransporte':null,'ChaveCTe':null,'NumeroDocumentoEmbarcador':null,'NumeroTransporte':null,'DataHoraCriacaoEmbrcador':null,'IBGEInicioPrestacao':null,'PesoMiligrama':'489500000,0','DocumentoRecebidoViaNOTFIS':false,'Containeres':null,'ClassificacaoNFe':null}],'CTes':null,'BlocosCarregamento':null,'Distribuicoes':null,'Doca':null,'Averbacao':null,'Temperatura':null,'FaixaTemperatura':null,'Vendedor':null,'Ordem':null,'OrdemColetaProgramada':0,'PortoSaida':null,'PortoChegada':null,'Companhia':null,'Navio':null,'Reserva':null,'Resumo':null,'ETA':null,'TipoEmbarque':null,'ValorFreteCobradoCliente':0.0,'ValorCustoFrete':0.0,'ValorFreteInformativo':0.0,'CodigoIntegracaoRota':null,'DescricaoRota':null,'DataInclusaoPCP':null,'DataInclusaoBooking':null,'DeliveryTerm':null,'IdAutorizacao':null,'PossuiGenset':false,'TipoPedido':null,'OrdemEntrega':0,'OrdemColeta':0,'FreteNegociado':null,'PedidoTrocaNota':false,'NumeroPedidoTrocaNota':null,'NumeroCIOT':null,'NaoGlobalizarPedido':false,'Adicional1':null,'Adicional2':null,'Adicional3':null,'Adicional4':null,'Adicional5':null,'Adicional6':null,'Adicional7':null,'PermiteQuebraPedidoMultiplosCarregamentos':true,'ClienteAdicional':null,'Despachante':null,'ViaTransporte':null,'PortoViagemOrigem':null,'PortoViagemDestino':null,'InLand':null,'PagamentoMaritimo':null,'ClienteDonoContainer':null,'TipoProbe':null,'CargaPaletizada':false,'NavioViagem':null,'ETS':null,'FreeDeten':0,'NumeroEXP':null,'RefEXPTransferencia':null,'StatusEXP':null,'NumeroPedidoProvisorio':null,'Especie':null,'StatusPedidoEmbarcador':null,'AcondicionamentoCarga':null,'DataEstufagem':null,'Onda':null,'ClusterRota':null,'DataPrevisaoInicioViagem':null,'DataPrevisaoChegadaDestinatario':null,'NumeroPager':null,'DataInicioViagem':null,'RotaEmbarcador':null,'NumeroBooking':null,'Embarcador':null,'Viagem':null,'ViagemLongoCurso':null,'PortoOrigem':null,'PortoDestino':null,'TerminalPortoOrigem':null,'TerminalPortoDestino':null,'TipoContainerReserva':null,'ContemCargaPerigosa':false,'ContemCargaRefrigerada':false,'TemperaturaObservacao':null,'ValidarNumeroContainer':false,'PropostaComercial':null,'Transbordo':null,'CodigoOrdemServico':0,'NumeroOrdemServico':null,'Embarque':null,'MasterBL':null,'NumeroDIEmbarque':null,'ProvedorOS':null,'Container':null,'TaraContainer':0,'NumeroLacre1':null,'NumeroLacre2':null,'NumeroLacre3':null,'CodigoBooking':0,'NumeroBL':null,'NecessitaAverbacao':false,'CargaRefrigeradaPrecisaEnergia':false,'QuantidadeTipoContainerReserva':0,'FormaAverbacaoCTE':null,'PercentualADValorem':0.0,'TipoDocumentoAverbacao':null,'ObservacaoProposta':null,'TipoPropostaFeeder':null,'DescricaoTipoPropostaFeeder':null,'DescricaoCarrierNavioViagem':null,'RealizarCobrancaTaxaDocumentacao':false,'QuantidadeConhecimentosTaxaDocumentacao':0,'ValorTaxaDocumento':0.0,'ContainerADefinir':false,'ValorCusteioSVM':0.0,'QuantidadeContainerBooking':0,'EmpresaResponsavel':null,'CentroCusto':null,'CNPJsDestinatariosNaoAutorizados':null,'ParametroIdentificacaoCliente':null,'PedidoDeSVMTerceiro':false,'NumeroCE':null,'ValorTaxaFeeder':0.0,'TipoCalculoCargaFracionada':null,'CargaDePreCarga':false,'SituacaoCarga':0,'OperadorCargaNome':null,'OperadorCargaEmail':null,'OperadorCargaCPF':null,'NaoAtualizarDadosDoPedido':false,'IMOUnidade':null,'IMOClasse':null,'IMOSequencia':null,'NecessarioAjudante':false,'AntecipacaoICMS':false,'DiasItinerario':0,'DiasUteisPrazoTransportador':0,'NumeroEntregasFinais':0,'PossuiPendenciaRoteirizacao':false,'NumeroPedidoCliente':null,'IDPropostaTrizy':0,'IDLoteTrizy':0,'DadosTransporteMaritimo':null}";

            Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao>(strCarga);

            StringBuilder mensagemErro = new StringBuilder();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
                Servicos.Embarcador.Carga.CargaDadosSumarizados servicoCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);
                Servicos.Embarcador.Integracao.IndicadorIntegracaoNFe servicoIndicadorIntegracaoNFe = new Servicos.Embarcador.Integracao.IndicadorIntegracaoNFe(unitOfWork, configuracaoTMS);
                Servicos.Embarcador.Pedido.RegraNumeroPedidoEmbarcador servicoRegraNumeroPedidoEmbarcador = new Servicos.Embarcador.Pedido.RegraNumeroPedidoEmbarcador(unitOfWork);
                Dominio.Entidades.Embarcador.Filiais.Filial filial = null;
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = null;
                int codigoPersonalizado = 0;

                servicoRegraNumeroPedidoEmbarcador.DefinirNumeroPedidoEmbarcadorComRegraAsync(cargaIntegracao);

                Servicos.Log.TratarErro("2 - Iniciou Validacao " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "AdicionarCarga");
                bool ignorarTransportadorNaoCadastrado = true;
                Servicos.WebService.Carga.Carga.ValidarCamposIntegracaoCarga(cargaIntegracao, configuracaoTMS.ReplicarCadastroVeiculoIntegracaoTransportadorDiferente, ignorarTransportadorNaoCadastrado, configuracaoTMS.BuscarClientesCadastradosNaIntegracaoDaCarga, configuracaoTMS.UtilizarProdutosDiversosNaIntegracaoDaCarga, ref mensagemErro, unitOfWork, configuracaoTMS, TipoServicoMultisoftware, out codigoPersonalizado, out tipoOperacao, out filial);

                //if (mensagemErro.Length > 0)
                //{
                //    Servicos.Log.TratarErro($"Carga: {cargaIntegracao.NumeroCarga} Retornou essa mensagem (validação): {mensagemErro}");

                //    Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos> retorno = Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>.CriarRetornoDadosInvalidos(mensagemErro.ToString(), new Dominio.ObjetosDeValor.WebService.Carga.Protocolos() { protocoloIntegracaoCarga = 0, protocoloIntegracaoPedido = 0 });

                //    if (codigoPersonalizado > 0)
                //    {
                //        retorno.CodigoMensagem = codigoPersonalizado;
                //        AuditarRetornoDadosInvalidosCNPJTransportador(unitOfWork, mensagemErro.ToString(), cargaIntegracao.NumeroCarga);
                //    }
                //    else
                //        AuditarRetornoDadosInvalidos(unitOfWork, mensagemErro.ToString(), cargaIntegracao.NumeroCarga);

                //    servicoIndicadorIntegracaoNFe.AdicionarIntegracaoAutomaticaRejeitada(cargaIntegracao, mensagemErro.ToString());

                //    return retorno;
                //}

                //if (Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().TransacaoCTe == "Serializable")
                //    unitOfWork.Start(System.Data.IsolationLevel.Serializable);
                //else
                unitOfWork.Start();

                int protocoloCargaExistente = 0;
                int protocoloPedidoExistente = 0;

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Servicos.WebService.Carga.Carga servicoCargaWS = new Servicos.WebService.Carga.Carga(unitOfWork);
                Servicos.WebService.Carga.Pedido servicoPedidoWS = new Servicos.WebService.Carga.Pedido(unitOfWork);
                Servicos.WebService.Empresa.Empresa servicoEmpresa = new Servicos.WebService.Empresa.Empresa(unitOfWork);
                Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                Servicos.Embarcador.Veiculo.Veiculo servicoVeiculo = new Servicos.Embarcador.Veiculo.Veiculo(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = null;

                var objTransportador = cargaIntegracao.TransportadoraEmitente;
                if (string.IsNullOrEmpty(servicoEmpresa.ValidarCamposEmpresaIntegracao(objTransportador)) && !(await servicoEmpresa.EmpresaIntegracaoExisteAsync(objTransportador)))
                {
                    Servicos.Log.TratarErro("3 - Adicionando Transportador " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "AdicionarCarga");
                    try
                    {
                        servicoEmpresa.AdicionarOutAtualizarEmpresa(objTransportador, unitOfWork, Auditado, _conexao.AdminStringConexao);
                    }
                    catch (ServicoException ex)
                    {
                        mensagemErro.Append(ex.Message);
                    }
                }

                //if (mensagemErro.Length > 0)
                //{
                //    Servicos.Log.TratarErro($"Carga: {cargaIntegracao.NumeroCarga} Retornou essa mensagem: {mensagemErro}");
                //    unitOfWork.Rollback();

                //    AuditarRetornoDadosInvalidos(unitOfWork, mensagemErro.ToString(), cargaIntegracao.NumeroCarga);
                //    servicoIndicadorIntegracaoNFe.AdicionarIntegracaoAutomaticaRejeitada(cargaIntegracao, mensagemErro.ToString());
                //    return Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>.CriarRetornoDadosInvalidos(mensagemErro.ToString(), new Dominio.ObjetosDeValor.WebService.Carga.Protocolos() { protocoloIntegracaoCarga = 0, protocoloIntegracaoPedido = 0 });
                //}

                Servicos.Log.TratarErro("4 - Iniciou Criar Pedido " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "AdicionarCarga");
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = servicoPedidoWS.CriarPedido(cargaIntegracao, filial, tipoOperacao, ref mensagemErro, TipoServicoMultisoftware, ref protocoloPedidoExistente, ref protocoloCargaExistente, false, Auditado, configuracaoTMS, ClienteAcesso, _conexao.AdminStringConexao);
                Servicos.Log.TratarErro("5 - Pedido Criado " + pedido.Codigo + " " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "AdicionarCarga");

                if (mensagemErro.Length == 0 || protocoloPedidoExistente > 0)
                {
                    bool manterPreCargaComoCarga = (!configuracaoTMS.TrocarPreCargaPorCarga && !configuracaoTMS.UtilizarSequenciaNumeracaoCargasViaIntegracao && (protocoloPedidoExistente > 0) && pedido.PedidoDePreCarga && !string.IsNullOrWhiteSpace(cargaIntegracao.NumeroCarga));
                    bool permitirFecharCargaAutomaticamente = true;

                    if (manterPreCargaComoCarga)
                    {
                        servicoPedidoWS.AtualizarParticipantesPedido(ref pedido, ref cargaIntegracao, ref mensagemErro, TipoServicoMultisoftware, null);

                        cargaPedido = repositorioCargaPedido.BuscarCargaAtualPorPedido(pedido.Codigo);

                        if (cargaPedido != null)
                        {
                            servicoCargaWS.PreencherCargaPedidoOrigemDestino(ref cargaPedido, pedido, cargaIntegracao, ref mensagemErro, TipoServicoMultisoftware, unitOfWork);

                            if (!cargaPedido.Carga.CargaDePreCargaEmFechamento)
                            {
                                Dominio.Entidades.Embarcador.Cargas.Carga preCarga = cargaPedido.Carga;

                                preCarga.CargaDePreCargaEmFechamento = true;
                                preCarga.CodigoCargaEmbarcador = cargaIntegracao.NumeroCarga;

                                servicoCargaWS.PreecherDadosPreCarga(preCarga, pedido, tipoOperacao, cargaIntegracao, ref mensagemErro, TipoServicoMultisoftware, configuracaoTMS, unitOfWork, Auditado);
                                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargaPedidos = repositorioCargaPedido.BuscarPorCarga(preCarga.Codigo);
                                servicoCargaDadosSumarizados.AlterarDadosSumarizadosCarga(ref preCarga, listaCargaPedidos, configuracaoTMS, unitOfWork, TipoServicoMultisoftware);
                                repositorioCarga.Atualizar(preCarga);

                                if (configuracaoTMS.SistemaIntegracaoPadraoCarga > 0)
                                {
                                    Servicos.Embarcador.Integracao.IntegracaoCarga serIntegracaoCarga = new Servicos.Embarcador.Integracao.IntegracaoCarga(unitOfWork);
                                    serIntegracaoCarga.InformarIntegracaoCarga(preCarga, configuracaoTMS.SistemaIntegracaoPadraoCarga, unitOfWork);
                                }
                            }

                            if (cargaPedido.Carga.CargaAgrupada)
                            {
                                if (!cargaPedido.CargaOrigem.CargaDePreCargaEmFechamento)
                                {
                                    cargaPedido.CargaOrigem.CargaDePreCargaEmFechamento = true;
                                    repositorioCarga.Atualizar(cargaPedido.CargaOrigem);
                                }

                                permitirFecharCargaAutomaticamente = !repositorioCarga.ExistePreCargaComFechamentoNaoIniciadoPorCargaAgrupada(cargaPedido.Carga.Codigo);
                            }

                            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

                            pedido.PedidoDePreCarga = false;
                            pedido.CodigoCargaEmbarcador = cargaPedido.Carga.CodigoCargaEmbarcador;

                            repositorioPedido.Atualizar(pedido);
                        }
                    }

                    if (cargaPedido == null)
                    {
                        Servicos.Log.TratarErro("6 - Iniciou Produtos " + pedido.Codigo + " " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "AdicionarCarga");

                        if (protocoloPedidoExistente == 0)
                            new Servicos.WebService.Carga.ProdutosPedido(unitOfWork).AdicionarProdutosPedido(pedido, configuracaoTMS, cargaIntegracao, ref mensagemErro, unitOfWork, Auditado);

                        Servicos.Log.TratarErro("7 - Finalizou Produtos " + pedido.Codigo + " " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "AdicionarCarga");

                        if (cargaIntegracao.Transbordo != null && cargaIntegracao.Transbordo.Count > 0)
                            new Servicos.WebService.Carga.ProdutosPedido(unitOfWork).SalvarTransbordo(pedido, cargaIntegracao.Transbordo, ref mensagemErro, unitOfWork, unitOfWork.StringConexao, Auditado, configuracaoTMS.EncerrarMDFeAutomaticamente);

                        Servicos.Log.TratarErro("8 - inicou Criar Carga " + pedido.Codigo + " " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "AdicionarCarga");
                        cargaPedido = servicoCargaWS.CriarCarga(pedido, cargaIntegracao, ref protocoloPedidoExistente, ref mensagemErro, ref protocoloCargaExistente, unitOfWork, TipoServicoMultisoftware, false, false, Auditado, configuracaoTMS, ClienteAcesso, _conexao.AdminStringConexao, filial, tipoOperacao);

                        if (cargaPedido != null)
                        {
                            Servicos.Log.TratarErro("9 - Criou Carga " + cargaPedido.Carga.Codigo + " CP = " + cargaPedido.Codigo + " " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "AdicionarCarga");

                            //VincularCargaPreCarga(cargaPedido, configuracaoTMS, unitOfWork);

                            Servicos.Log.TratarErro("10 - Adicionar Produtos " + cargaPedido.Carga.Codigo + " CP = " + cargaPedido.Codigo + " " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "AdicionarCarga");
                            servicoCargaWS.AdicionarProdutosCarga(cargaPedido, cargaIntegracao, ref mensagemErro, unitOfWork, configuracaoTMS.UsarPesoProdutoSumarizacaoCarga);
                            Servicos.Log.TratarErro("11 - Criou Produtos  " + cargaPedido.Carga.Codigo + " CP = " + cargaPedido.Codigo + " " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "AdicionarCarga");

                            //CriarDependenciaProdutoSeNecessario(cargaIntegracao, cargaPedido, unitOfWork);
                        }
                    }

                    if ((cargaPedido != null) && cargaIntegracao.FecharCargaAutomaticamente && (mensagemErro.Length == 0) && permitirFecharCargaAutomaticamente)
                    {
                        if (configuracaoTMS.AgruparCargaAutomaticamente)
                            cargaPedido.Carga.AgruparCargaAutomaticamente = true;
                        else if (!configuracaoTMS.FecharCargaPorThread)
                        {
                            Servicos.Log.TratarErro("12 - Fechar Carga " + cargaPedido.Carga.Codigo + " CP = " + cargaPedido.Codigo + " " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "AdicionarCarga");
                            servicoCarga.FecharCarga(cargaPedido.Carga, unitOfWork, TipoServicoMultisoftware, null);
                            Servicos.Log.TratarErro("13 - Fechou Carga " + cargaPedido.Carga.Codigo + " CP = " + cargaPedido.Codigo + " " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "AdicionarCarga");

                            if (cargaPedido.Carga.CargaAgrupamento == null)
                                cargaPedido.Carga.CargaFechada = true;

                            repositorioCarga.Atualizar(cargaPedido.Carga);
                        }
                        else
                            cargaPedido.Carga.FechandoCarga = true;

                        repositorioCarga.Atualizar(cargaPedido.Carga);

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaPedido.Carga, "Solicitou o fechamento da carga. Protocolo " + cargaPedido.Carga.Codigo.ToString(), unitOfWork);
                    }

                    if (configuracaoTMS.PermitirAtualizarModeloVeicularCargaDoVeiculoNoWebService && !string.IsNullOrWhiteSpace(cargaIntegracao.ModeloVeicular?.CodigoIntegracao) && mensagemErro.Length == 0)
                    {
                        Dominio.Entidades.Veiculo veiculo = cargaPedido?.Carga?.Veiculo;

                        if (veiculo != null && cargaIntegracao.ModeloVeicular?.CodigoIntegracao != veiculo.ModeloVeicularCarga?.CodigoIntegracao)
                        {
                            servicoVeiculo.AtualizarModeloVeicularDoVeiculoCarga(veiculo, cargaIntegracao.ModeloVeicular, ref mensagemErro, unitOfWork);
                            Servicos.Log.TratarErro("14 - Atualizou Vínculo do MVC do veículo " + veiculo.Codigo + " para = " + cargaIntegracao.ModeloVeicular.CodigoIntegracao + " " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "AdicionarCarga");
                        }
                    }
                }

                if (mensagemErro.Length > 0)
                {
                    Servicos.Log.TratarErro($"Carga: {cargaIntegracao.NumeroCarga} Retornou essa mensagem: {mensagemErro.ToString()}");
                    unitOfWork.Rollback();

                    if ((protocoloCargaExistente > 0 && protocoloPedidoExistente > 0) || (protocoloPedidoExistente > 0 && string.IsNullOrWhiteSpace(cargaIntegracao.NumeroCarga)))
                    {
                        Servicos.Log.TratarErro($"protocoloCargaExistente: {protocoloCargaExistente} protocoloPedidoExistente: {protocoloPedidoExistente}");
                        bool retornarDuplicidade = true;

                        if (configuracaoTMS.RetornarFalhaAdicionarCargaSeExistirCancelamentoCargaEmAberto)
                        {
                            Repositorio.Embarcador.Cargas.CargaCancelamento repCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unitOfWork);
                            Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = repCargaCancelamento.BuscarPorProtocoloCarga(protocoloCargaExistente);

                            if (cargaCancelamento != null)
                                retornarDuplicidade = false;
                        }

                        if (retornarDuplicidade)
                        {
                            if (configuracaoTMS.RetornosDuplicidadeWSSubstituirPorSucesso)
                            {
                                if (cargaIntegracao.FecharCargaAutomaticamente && configuracaoTMS.FecharCargaPorThread && !configuracaoTMS.AgruparCargaAutomaticamente && protocoloCargaExistente > 0)
                                {
                                    Dominio.Entidades.Embarcador.Cargas.Carga cargaFechamento = repositorioCarga.BuscarPorProtocolo(protocoloCargaExistente);

                                    if (cargaFechamento != null && !cargaFechamento.CargaFechada)
                                    {
                                        cargaFechamento.FechandoCarga = true;
                                        repositorioCarga.Atualizar(cargaFechamento);
                                    }
                                }

                                //return Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>.CriarRetornoSucesso(new Dominio.ObjetosDeValor.WebService.Carga.Protocolos() { protocoloIntegracaoCarga = protocoloCargaExistente, protocoloIntegracaoPedido = protocoloPedidoExistente, ParametroIdentificacaoCliente = cargaIntegracao.ParametroIdentificacaoCliente });
                            }
                            else
                            {
                                //AuditarRetornoDuplicidadeDaRequisicao(unitOfWork, mensagemErro.ToString(), cargaIntegracao.NumeroCarga);
                                servicoIndicadorIntegracaoNFe.AdicionarIntegracaoAutomaticaRejeitada(cargaIntegracao, mensagemErro.ToString());
                                //return Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>.CriarRetornoDuplicidadeRequisicao(mensagemErro.ToString(), new Dominio.ObjetosDeValor.WebService.Carga.Protocolos() { protocoloIntegracaoCarga = protocoloCargaExistente, protocoloIntegracaoPedido = protocoloPedidoExistente, ParametroIdentificacaoCliente = cargaIntegracao.ParametroIdentificacaoCliente });
                            }
                        }
                        else
                        {
                            //AuditarRetornoDadosInvalidos(unitOfWork, mensagemErro.ToString(), cargaIntegracao.NumeroCarga);
                            servicoIndicadorIntegracaoNFe.AdicionarIntegracaoAutomaticaRejeitada(cargaIntegracao, mensagemErro.ToString());
                            //return Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>.CriarRetornoDadosInvalidos(mensagemErro.ToString(), new Dominio.ObjetosDeValor.WebService.Carga.Protocolos() { protocoloIntegracaoCarga = 0, protocoloIntegracaoPedido = 0 });
                        }
                    }
                    else
                    {
                        //AuditarRetornoDadosInvalidos(unitOfWork, mensagemErro.ToString(), cargaIntegracao.NumeroCarga);
                        servicoIndicadorIntegracaoNFe.AdicionarIntegracaoAutomaticaRejeitada(cargaIntegracao, mensagemErro.ToString());
                        //return Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>.CriarRetornoDadosInvalidos(mensagemErro.ToString(), new Dominio.ObjetosDeValor.WebService.Carga.Protocolos() { protocoloIntegracaoCarga = 0, protocoloIntegracaoPedido = 0 });
                    }
                }

                servicoIndicadorIntegracaoNFe.AdicionarIntegracaoAutomaticaComSucesso(cargaPedido);

                Servicos.Log.TratarErro("15 - Iniciou Commit " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "AdicionarCarga");
                unitOfWork.CommitChanges();
                Servicos.Log.TratarErro("16 - Finalizou Commit " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "AdicionarCarga");

                //Servicos.Embarcador.Carga.Frete.AlertarRotaNaoCadastradaPorPedido(pedido, unitOfWork, TipoServicoMultisoftware);
                //Servicos.Embarcador.Carga.Frete.AlertarFaltaTabelaFretePorPedido(pedido, unitOfWork, TipoServicoMultisoftware);

                if (cargaPedido != null && cargaPedido.Carga != null)
                    Servicos.Log.TratarErro($"AdicionarCarga Retorno: Protocolo carga = {cargaPedido.Carga.Codigo}, protocolo pedido = {pedido.Codigo}");
                else if (pedido != null)
                    Servicos.Log.TratarErro($"AdicionarCarga Retorno: Protocolo pedido = {pedido.Codigo}");

                Servicos.Log.TratarErro("17 - Retornou protocolos " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "AdicionarCarga");

                //if (protocoloPedidoExistente == 0 || configuracaoTMS.RetornosDuplicidadeWSSubstituirPorSucesso)
                //    return Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>.CriarRetornoSucesso(new Dominio.ObjetosDeValor.WebService.Carga.Protocolos() { protocoloIntegracaoCarga = cargaPedido != null && cargaPedido.Carga != null ? cargaPedido.Carga.Protocolo /*cargaPedido.Carga.Codigo*/ : 0, protocoloIntegracaoPedido = pedido?.Protocolo /*pedido?.Codigo*/ ?? 0 });
                //else
                //    return Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>.CriarRetornoDuplicidadeRequisicao(mensagemErro.ToString(), new Dominio.ObjetosDeValor.WebService.Carga.Protocolos() { protocoloIntegracaoCarga = cargaPedido != null && cargaPedido.Carga != null ? cargaPedido.Carga.Protocolo /*cargaPedido.Carga.Codigo*/ : 0, protocoloIntegracaoPedido = pedido?.Protocolo /*pedido?.Codigo*/ ?? 0 });
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro($"Carga: {cargaIntegracao.NumeroCarga} Retornou essa mensagem: {excecao.Message}");
                //ArmazenarLogIntegracao(cargaIntegracao);
                //return Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                Servicos.Log.TratarErro($"Carga: {cargaIntegracao.NumeroCarga} retornou exceção a seguir:");
                //ArmazenarLogIntegracao(cargaIntegracao);
                //return Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>.CriarRetornoExcecao($"Ocorreu uma falha ao obter os dados das integrações. {mensagemErro.ToString()}");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private void TesteRemoverPedido()
        {
            string strProtocolo = "{'protocoloIntegracaoCarga':1751,'protocoloIntegracaoPedido':9701,'Remetente':null,'Destinatario':null,'ParametroIdentificacaoCliente':null}";

            Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>(strProtocolo);

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedidos = repositorioCargaPedido.BuscarPorProtocoloCargaEProtocoloPedido(protocolo.protocoloIntegracaoCarga, protocolo.protocoloIntegracaoPedido);

                if ((cargasPedidos == null) || (cargasPedidos.Count == 0))
                    Retorno<bool>.CriarRetornoDadosInvalidos("Não foi possível encontrar nenhum pedido com os protocolos informados");

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracao.BuscarConfiguracaoPadrao();


                unitOfWork.Start();

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargasPedidos)
                {
                    if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    {
                        if (cargaPedido.Pedido.Container != null)
                            throw new WebServiceException("O pedido selecionado já possui container vinculado.");

                        Servicos.Embarcador.Carga.CargaPedido.RemoverPedidoVinculadoCarga(cargaPedido, unitOfWork, ConfiguracaoEmbarcador, TipoServicoMultisoftware, null, removerPedido: false);

                        List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosCarga = repositorioCargaPedido.BuscarPedidosPorCarga(cargaPedido.Carga.Codigo);

                        foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoCarga in pedidosCarga)
                        {
                            pedidoCarga.QuantidadeContainerBooking = pedidoCarga.QuantidadeContainerBooking - 1;

                            if (pedidoCarga.QuantidadeContainerBooking < 0)
                                pedidoCarga.QuantidadeContainerBooking = 0;

                            repositorioPedido.Atualizar(pedidoCarga);
                        }

                        Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(cargaPedido.Carga.Codigo);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, "Excluiu pedido vinculado por Integração.", unitOfWork);
                    }
                    else
                    {
                        Servicos.Embarcador.Carga.CargaPedido.RemoverPedidoCarga(cargaPedido.Carga, cargaPedido, configuracaoEmbarcador, TipoServicoMultisoftware, unitOfWork, configuracaoGeralCarga);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaPedido.Carga, null, $"Removeu o pedido {cargaPedido.Pedido.NumeroPedidoEmbarcador} via integração", unitOfWork);

                        cargaPedido.Carga.SituacaoRoteirizacaoCarga = SituacaoRoteirizacao.Aguardando;

                        repositorioCarga.Atualizar(cargaPedido.Carga);
                    }
                }

                unitOfWork.CommitChanges();

                Retorno<bool>.CriarRetornoSucesso(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao remover o pedido da carga");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private void TesteIntegrarDadosNotasFiscais()
        {
            string strProtocolo = "{'protocoloIntegracaoCarga':1787,'protocoloIntegracaoPedido':10752,'Remetente':null,'Destinatario':null,'ParametroIdentificacaoCliente':null}";

            string strNfs = "[{'Protocolo':0,'Chave':'31210461064838008622550010005287161335725976','Rota':null,'Numero':11581191,'Serie':'1','Modelo':'55','Valor':17326.50,'BaseCalculoICMS':17326.50,'ValorICMS':3118.77,'BaseCalculoST':0.0,'ValorFreteLiquido':0.0,'ValorFrete':0.0,'ValorST':0.0,'ValorTotalProdutos':17326.50,'ValorSeguro':0.0,'ValorDesconto':0.0,'ValorImpostoImportacao':0.0,'ValorPIS':0.0,'ValorCOFINS':0.0,'ValorOutros':0.0,'ValorIPI':0.0,'PesoBruto':16702.000,'PesoLiquido':16600.000,'Cubagem':0.0,'MetroCubico':0.0,'VolumesTotal':610.0,'DataEmissao':'23/04/2021 15:04:46','NaturezaOP':'Venda produção do estabelecimento /  Venda merc.adq.receb.de','QuantidadePallets':0.0,'InformacoesComplementares':'Aliquota de IPI conforme Decreto no.7.879 de 2012 art. 3o.Anexo III','ModalidadeFrete':1,'Emitente':{'ClienteExterior':false,'CPFCNPJ':'61064838008622','CodigoIntegracao':'SQ11','TipoPessoa':1,'CodigoAtividade':2,'RGIE':'0010261810316','IM':'1014996','RazaoSocial':'Saint-Gobain do Brasil Produtos Ind ustriais e para Construcao LTDA','NomeFantasia':null,'CNAE':null,'Endereco':{'Cidade':{'Codigo':0,'Descricao':'SANTA LUZIA','IBGE':3157807,'CodigoIntegracao':null,'SiglaUF':null,'Pais':{'CodigoPais':0,'NomePais':null,'SiglaPais':'BR'},'Regiao':null,'CodigoDocumento':null,'Atualizar':false},'Logradouro':'RUA QUARTZOLIT','Numero':'100','Complemento':'   ','CEP':'33040-257','CEPSemFormato':null,'Bairro':'SITIO BOA VISTA','DDDTelefone':null,'Telefone':'3130796690','Telefone2':null,'CodigoIntegracao':null,'InscricaoEstadual':null,'Latitude':null,'Longitude':null,'UF':null},'Email':'recebimento.nfe.sgb@saint-gobain.com','AtualizarEnderecoPessoa':true,'EmailFatura':null,'Codigo':null,'CodigoCategoria':null,'CodigoDocumento':null,'RNTRC':null,'NumeroCartaoCIOT':null,'GerarCIOT':null,'TipoFavorecidoCIOT':null,'TipoPagamentoCIOT':null,'GrupoPessoa':null,'ExigirNumeroControleCliente':false,'ExigirNumeroNumeroReferenciaCliente':false,'TipoEmissaoCTeDocumentosExclusivo':0,'ParametroRateioFormulaExclusivo':0,'AdicionarComoOutroEndereco':false,'InativarCliente':false,'CodigoDocumentoFornecedor':null,'Cliente':false,'Fornecedor':false,'RaioEmMetros':0},'Destinatario':{'ClienteExterior':false,'CPFCNPJ':'01016399000161','CodigoIntegracao':'0000157304','TipoPessoa':1,'CodigoAtividade':7,'RGIE':'0620314750028','IM':null,'RazaoSocial':'FUROFIX ENGENHARIA LTDA EPP ','NomeFantasia':'FUROFIX ENG LTDA','CNAE':null,'Endereco':{'Cidade':{'Codigo':0,'Descricao':'BELO HORIZONTE','IBGE':13106200,'CodigoIntegracao':null,'SiglaUF':null,'Pais':{'CodigoPais':0,'NomePais':null,'SiglaPais':'BR'},'Regiao':null,'CodigoDocumento':null,'Atualizar':false},'Logradouro':'R DAS CAMELIAS','Numero':'79','Complemento':'   ','CEP':'30421-236','CEPSemFormato':null,'Bairro':'NOVA SUISSA','DDDTelefone':null,'Telefone':'3133721816','Telefone2':null,'CodigoIntegracao':null,'InscricaoEstadual':null,'Latitude':null,'Longitude':null,'UF':null},'Email':null,'AtualizarEnderecoPessoa':true,'EmailFatura':null,'Codigo':null,'CodigoCategoria':null,'CodigoDocumento':null,'RNTRC':null,'NumeroCartaoCIOT':null,'GerarCIOT':null,'TipoFavorecidoCIOT':null,'TipoPagamentoCIOT':null,'GrupoPessoa':null,'ExigirNumeroControleCliente':false,'ExigirNumeroNumeroReferenciaCliente':false,'TipoEmissaoCTeDocumentosExclusivo':0,'ParametroRateioFormulaExclusivo':0,'AdicionarComoOutroEndereco':false,'InativarCliente':false,'CodigoDocumentoFornecedor':null,'Cliente':false,'Fornecedor':false,'RaioEmMetros':0},'Recebedor':null,'Expedidor':null,'Tomador':null,'Transportador':{'Atividade':0,'CNPJ':null,'RazaoSocial':'PROPRIO ','NomeFantasia':null,'CodigoIntegracao':'0001149957','IE':'ISENTO','InscricaoMunicipal':null,'InscricaoST':null,'RNTRC':null,'SimplesNacional':false,'Emails':null,'EmissaoDocumentosForaDoSistema':false,'LiberacaoParaPagamentoAutomatico':false,'Endereco':{'Cidade':{'Codigo':0,'Descricao':'SANTA LUZIA','IBGE':0,'CodigoIntegracao':null,'SiglaUF':null,'Pais':{'CodigoPais':0,'NomePais':null,'SiglaPais':'BR'},'Regiao':null,'CodigoDocumento':null,'Atualizar':false},'Logradouro':null,'Numero':null,'Complemento':'   ','CEP':null,'CEPSemFormato':null,'Bairro':null,'DDDTelefone':null,'Telefone':null,'Telefone2':null,'CodigoIntegracao':null,'InscricaoEstadual':null,'Latitude':null,'Longitude':null,'UF':null},'DadosBancarios':null,'Certificado':null,'CodigoDocumento':null},'Veiculo':null,'Volumes':null,'TipoCarga':null,'TipoOperacaoNotaFiscal':1,'SituacaoNFeSefaz':0,'Produtos':[{'CodigoProduto':'0001.00001.0030FD','DescricaoProduto':'CIMENTCOLA INT CINZA AC1 FD 30KG','CodigoGrupoProduto':null,'CodigocEAN':null,'DescricaoGrupoProduto':null,'ValorUnitario':0.0,'PesoUnitario':0.0,'Quantidade':480.000,'QuantidadePlanejada':0.0,'UnidadeMedida':'FD','QuantidadeEmbalagem':0.0,'PesoTotalEmbalagem':0.0,'MetroCubito':0.0,'CodigoDocumentacao':null,'Observacao':null,'InativarCadastro':false,'Atualizar':false,'SetorLogistica':null,'ClasseLogistica':null,'CodigoNCM':null,'ProdutoLotes':null,'ProdutoDivisoesCapacidade':null,'QuantidadePallet':0.0,'QuantidadePorCaixa':0,'QuantidadeCaixaPorPallet':0,'Lastro':0.0,'Camada':0.0,'Altura':0.0,'Largura':0.0,'Comprimento':0.0,'LinhaSeparacao':null,'TipoEmbalagem':null,'PalletFechado':false,'CSTICMS':null,'OrigemMercadoria':null,'CodigoNFCI':null,'CanalDistribuicao':null,'SiglaModalidade':null},{'CodigoProduto':'0528.00000.0012BD','DescricaoProduto':'RECUPERA RODAPE BRANCO BALDE 12KG','CodigoGrupoProduto':null,'CodigocEAN':null,'DescricaoGrupoProduto':null,'ValorUnitario':0.0,'PesoUnitario':0.0,'Quantidade':50.000,'QuantidadePlanejada':0.0,'UnidadeMedida':'BLD','QuantidadeEmbalagem':0.0,'PesoTotalEmbalagem':0.0,'MetroCubito':0.0,'CodigoDocumentacao':null,'Observacao':null,'InativarCadastro':false,'Atualizar':false,'SetorLogistica':null,'ClasseLogistica':null,'CodigoNCM':null,'ProdutoLotes':null,'ProdutoDivisoesCapacidade':null,'QuantidadePallet':0.0,'QuantidadePorCaixa':0,'QuantidadeCaixaPorPallet':0,'Lastro':0.0,'Camada':0.0,'Altura':0.0,'Largura':0.0,'Comprimento':0.0,'LinhaSeparacao':null,'TipoEmbalagem':null,'PalletFechado':false,'CSTICMS':null,'OrigemMercadoria':null,'CodigoNFCI':null,'CanalDistribuicao':null,'SiglaModalidade':null},{'CodigoProduto':'0001.00001.0020PA','DescricaoProduto':'CIMENTCOLA INT CINZA AC1 20KG PA','CodigoGrupoProduto':null,'CodigocEAN':null,'DescricaoGrupoProduto':null,'ValorUnitario':0.0,'PesoUnitario':0.0,'Quantidade':80.000,'QuantidadePlanejada':0.0,'UnidadeMedida':'SAC','QuantidadeEmbalagem':0.0,'PesoTotalEmbalagem':0.0,'MetroCubito':0.0,'CodigoDocumentacao':null,'Observacao':null,'InativarCadastro':false,'Atualizar':false,'SetorLogistica':null,'ClasseLogistica':null,'CodigoNCM':null,'ProdutoLotes':null,'ProdutoDivisoesCapacidade':null,'QuantidadePallet':0.0,'QuantidadePorCaixa':0,'QuantidadeCaixaPorPallet':0,'Lastro':0.0,'Camada':0.0,'Altura':0.0,'Largura':0.0,'Comprimento':0.0,'LinhaSeparacao':null,'TipoEmbalagem':null,'PalletFechado':false,'CSTICMS':null,'OrigemMercadoria':null,'CodigoNFCI':null,'CanalDistribuicao':null,'SiglaModalidade':null}],'Canhoto':null,'NumeroDT':null,'DataEmissaoDT':null,'CodigoIntegracaoCliente':null,'NumeroSolicitacao':null,'NumeroPedido':null,'NumeroRomaneio':null,'SubRota':null,'GrauRisco':null,'AliquotaICMS':0.0,'Observacao':null,'DataPrevisao':null,'KMRota':0.0,'ValorComponenteFreteCrossDocking':0.0,'ValorComponenteAdValorem':0.0,'ValorComponenteDescarga':0.0,'ValorComponentePedagio':0.0,'ValorComponenteAdicionalEntrega':0.0,'NCMPredominante':null,'CodigoProduto':null,'CFOPPredominante':null,'NumeroControleCliente':null,'NumeroCanhoto':null,'NumeroReferenciaEDI':null,'PINSuframa':null,'DescricaoMercadoria':null,'PesoAferido':0.0,'TipoOperacao':null,'ModeloVeicular':null,'ObsPlaca':null,'ObsTransporte':null,'ChaveCTe':null,'NumeroDocumentoEmbarcador':null,'NumeroTransporte':null,'DataHoraCriacaoEmbrcador':null,'IBGEInicioPrestacao':null,'PesoMiligrama':'16702000000,000','DocumentoRecebidoViaNOTFIS':false,'Containeres':null,'ClassificacaoNFe':null}]";

            Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>(strProtocolo);
            List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal> notasFiscais = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal>>(strNfs);

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            Retorno<bool> retorno = new Retorno<bool>();
            List<string> caminhosXMLTemp = new List<string>();
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            retorno.Mensagem = "";

            try
            {
                Repositorio.WebService.Integradora repIntegracadora = new Repositorio.WebService.Integradora(unitOfWork);
                Dominio.Entidades.WebService.Integradora integradora = repIntegracadora.BuscarPorCodigo(5);

                if (protocolo.protocoloIntegracaoCarga > 0 || protocolo.protocoloIntegracaoPedido > 0)
                {

                    Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                    Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                    Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                    Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFisca = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
                    Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

                    Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

                    Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();

                    Servicos.Embarcador.Canhotos.Canhoto serCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(unitOfWork);
                    Servicos.Embarcador.Pedido.Produto serProduto = new Servicos.Embarcador.Pedido.Produto(unitOfWork);
                    Servicos.Embarcador.Carga.CargaPedido serCargaPedido = new Servicos.Embarcador.Carga.CargaPedido(unitOfWork);
                    Servicos.WebService.Pessoas.Pessoa serPessoa = new Servicos.WebService.Pessoas.Pessoa(unitOfWork);
                    Servicos.Embarcador.Pedido.NotaFiscal serCargaNotaFiscal = new Servicos.Embarcador.Pedido.NotaFiscal(unitOfWork);

                    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorProtocoloCargaOrigemEProtocoloPedido(protocolo.protocoloIntegracaoCarga, protocolo.protocoloIntegracaoPedido);

                    if (cargaPedido == null && protocolo.protocoloIntegracaoCarga == 0 && protocolo.protocoloIntegracaoPedido > 0)
                        cargaPedido = repCargaPedido.BuscarCargaAtualPorProtocoloPedido(protocolo.protocoloIntegracaoPedido);

                    if (cargaPedido != null)
                    {
                        if (cargaPedido.SituacaoEmissao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNF.NFEnviada && (cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe || (!configuracao.NaoAceitarNotasNaEtapa1 && cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova) || (cargaPedido.Carga.CargaEmitidaParcialmente && (cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte || cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgImpressaoDocumentos))))
                        {
                            unitOfWork.Start();
                            string retornoIntegracao = Servicos.WebService.NFe.NotaFiscal.IntegrarNotaFiscal(cargaPedido, notasFiscais, null, null, configuracao, TipoServicoMultisoftware, Auditado, integradora, unitOfWork);
                            if (string.IsNullOrWhiteSpace(retornoIntegracao))
                            {
                                retorno.Objeto = true;
                                retorno.Status = true;
                                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.Sucesso;
                                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaPedido, "Integrou dados de notas fiscais", unitOfWork);
                                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaPedido.Carga, "Integrou dados de notas fiscais", unitOfWork);
                                unitOfWork.CommitChanges();
                            }
                            else
                            {
                                retorno.Mensagem = retornoIntegracao;
                                retorno.Objeto = true;
                                retorno.Status = false;
                                retorno.CodigoMensagem = retornoIntegracao.Contains("já foi enviada") ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DuplicidadeDaRequisicao : Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                                unitOfWork.Rollback();
                            }
                        }
                        else
                        {
                            if ((configuracao.NaoAceitarNotasNaEtapa1 && cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova)
                                || cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgTransportador
                                || cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada
                                || cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada
                                || cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada
                                || cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte
                                || cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgImpressaoDocumentos
                                || cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgIntegracao
                                || (cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete && cargaPedido.Carga.ExigeNotaFiscalParaCalcularFrete))
                            {
                                retorno.Status = false;

                                if (configuracao.NotaUnicaEmCargas && notasFiscais != null && notasFiscais.Count() == 1 && !string.IsNullOrWhiteSpace(notasFiscais.FirstOrDefault().Chave) && (repPedidoXMLNotaFiscal.BuscarPorChave(notasFiscais.FirstOrDefault().Chave) != null))
                                {
                                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DuplicidadeDaRequisicao;
                                    retorno.Mensagem = "A nota fiscal " + notasFiscais.FirstOrDefault().Chave + " já foi enviada.";
                                }
                                else
                                {
                                    if (cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada)
                                        retorno.CodigoMensagem = 313;
                                    else if (cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada
                                            || cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte
                                            || cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgImpressaoDocumentos
                                            || cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgIntegracao)
                                        retorno.CodigoMensagem = 314;
                                    else
                                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;

                                    retorno.Mensagem += "Não é possível enviar as notas fiscais para a carga em sua atual situação (" + cargaPedido.Carga.DescricaoSituacaoCarga + "). ";
                                }
                            }
                            else
                            {
                                if (cargaPedido.SituacaoEmissao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNF.NFEnviada && cargaPedido.Carga.CargaEmitidaParcialmente)
                                {
                                    retorno.Mensagem += "Para enviar as demais notas a carga precisa estar na situação em transporte.";
                                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                                    retorno.Status = false;
                                }
                                else
                                {
                                    retorno.Mensagem += "As notas fiscais já foram enviadas para esse pedido.";
                                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DuplicidadeDaRequisicao;
                                    if (!configuracao?.RetornosDuplicidadeWSSubstituirPorSucesso ?? false)
                                        retorno.Status = false;
                                    else
                                    {
                                        retorno.Objeto = true;
                                        retorno.Status = true;
                                    }
                                }

                            }
                        }
                    }
                    else if (protocolo.protocoloIntegracaoCarga == 0 && protocolo.protocoloIntegracaoPedido > 0)
                    {
                        Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repPedido.BuscarPorCodigo(protocolo.protocoloIntegracaoPedido);
                        if (pedido != null)
                        {
                            unitOfWork.Start();
                            Servicos.WebService.NFe.NotaFiscal.IntegrarNotaFiscal(pedido, notasFiscais, null, null, configuracao, TipoServicoMultisoftware, Auditado, integradora, unitOfWork);
                            retorno.Objeto = true;
                            retorno.Status = true;
                            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.Sucesso;
                            Servicos.Auditoria.Auditoria.Auditar(Auditado, pedido, "Integrou dados de notas fiscais", unitOfWork);
                            unitOfWork.CommitChanges();

                        }
                        else
                        {
                            retorno.Status = false;
                            retorno.Mensagem = "Protocolos informados são inválidos. ";
                            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        }
                    }
                    else
                    {
                        retorno.Status = false;
                        if (configuracao.NotaUnicaEmCargas && notasFiscais != null && notasFiscais.Count() == 1 && !string.IsNullOrWhiteSpace(notasFiscais.FirstOrDefault().Chave) && (repPedidoXMLNotaFiscal.BuscarPorChave(notasFiscais.FirstOrDefault().Chave) != null))
                        {
                            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DuplicidadeDaRequisicao;
                            retorno.Mensagem = "A nota fiscal " + notasFiscais.FirstOrDefault().Chave + " já foi enviada.";
                        }
                        else
                        {
                            retorno.Mensagem = "Protocolos informados são inválidos. ";
                            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        }
                    }
                }
                else
                {
                    retorno.Status = false;
                    retorno.Mensagem = "É obrigatório informar os protocolos de integração. ";
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                }
                if (retorno.Status)
                {
                    //foreach (string caminho in caminhosXMLTemp)
                    //    Utilidades.IO.FileStorageService.Storage.Delete(caminho);
                }

            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao enviar a(s) NF-e(s)";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            if (retorno.CodigoMensagem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos)
            {
                //falar com rafael da pelissari aqui está gerando muito log, fica tentando mandar sem parar para viagens de armazens
                //Servicos.Log.TratarErro(retorno.Mensagem);
                //Servicos.Log.TratarErro("Protocolo Pedido: " + protocolo != null ? protocolo.protocoloIntegracaoPedido.ToString() : "nulo" + "Protocolo Carga: " + protocolo != null ? protocolo.protocoloIntegracaoCarga.ToString() : "nulo" + " Retorno: " + retorno.Mensagem + " Status:" + retorno.Status.ToString());
            }

        }

        private void GerarRoteirizacaoMonitoramento(Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                var server = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork).Buscar();
                var cargaMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork).BuscarPorCodigo(5839);
                Servicos.Embarcador.Logistica.Monitoramento.ControleDistancia.ObterRespostaRoteirizacao(cargaMonitoramento, server.ServidorRouteOSM, unitOfWork);
            }
            catch (Exception ex)
            {

            }
        }

        private void GerarCargaEmLote(Repositorio.UnitOfWork unidadeDeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            //Repositorio.Embarcador.Cargas.MontagemCarga.MontagemCarregamento repMontagemCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.MontagemCarregamento(unidadeDeTrabalho);
            //try
            //{
            //    List<int> codigosCarregamento = repMontagemCarregamento.BusarCodigosCarregamento(0);

            //    if (codigosCarregamento.Count == 0)
            //        return;

            //    Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga.GerarCargaEmLote(codigosCarregamento, tipoServicoMultisoftware, null, unidadeDeTrabalho, 0, false, ClienteAcesso.URLAcesso);

            //    repMontagemCarregamento.DeletarTodos(new List<int> { 0 });

            //}
            //catch (Exception ex)
            //{
            //    Servicos.Log.TratarErro(ex);
            //    new Servicos.Embarcador.Hubs.MontagemCarga().InformarCargaEmLoteFinalizado("Erro ao gerar cargas", 0);
            //    repMontagemCarregamento.DeletarTodos(new List<int> { 0 });
            //}

        }

        private void GerarMontagemCargaAutomatica(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoPedido repMontagemCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoPedido(unidadeDeTrabalho);
            try
            {
                List<int> codigosPedidos = repMontagemCarregamentoPedido.BusarCodigosPedido(0);

                if (codigosPedidos.Count == 0)
                    return;

                new Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga(unidadeDeTrabalho).GerarCarregamentoAutomatico(codigosPedidos, 0, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);


            }
            catch (Exception ex)
            {

            }

        }

        //private void TesteCteAnteriorCargaColeta(Repositorio.UnitOfWork unitOfWork)
        //{

        //    Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

        //    var carga = repCarga.BuscarPorCodigo(29613);

        //    Servicos.Embarcador.Carga.Documentos serCargaDocumentos = new Servicos.Embarcador.Carga.Documentos();

        //    serCargaDocumentos.GerarNFeAnteriorCargaDeColeta(carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador, null, unitOfWork);
        //}

        private void TesteRecriarPedidos(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            var carga = repCarga.BuscarPorCodigo(139731);

            Servicos.Embarcador.Carga.Carga svcCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

            //svcCarga.AtualizarPedidosRecebimentoNFe(carga, configuracaoTMS, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador, unitOfWork);

            svcCarga.ConfirmarEnvioDosDocumentos(carga, unitOfWork, unitOfWork.StringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador, configuracaoTMS, Auditado, Cliente);

            //TesteRoteirizacao(unitOfWork);

        }

        private void AtualizarLocalDosPedidos(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repositorioConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = repositorioConfiguracaoPedido.BuscarConfiguracaoPadrao();

            var carga = repCarga.BuscarPorCodigo(14181);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);


            Servicos.Embarcador.Carga.CargaLocaisPrestacao serCargaLocaisPrestacao = new Servicos.Embarcador.Carga.CargaLocaisPrestacao(unitOfWork);


            Dominio.ObjetosDeValor.Embarcador.Carga.RetornoRotasCarga retornoRotas = serCargaLocaisPrestacao.VerificarEAjustarLocaisPrestacao(carga, cargaPedidos, unitOfWork, TipoServicoMultisoftware, configuracaoPedido);

            Servicos.Embarcador.Carga.CargaDadosSumarizados serCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);

            serCargaDadosSumarizados.AlterarDadosSumarizadosCarga(ref carga, cargaPedidos, configuracaoTMS, unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);


            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidosXMLNotaFiscal = repPedidoXMLNotaFiscal.BuscarPorCarga(carga.Codigo);


        }

        /*private void TesteIntegracaoIntelipost(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao repOcorrencia = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao(unitOfWork);

            Servicos.Embarcador.Integracao.Intelipost.IntegracaoOcorrencia servicoIntegracaoOcorrencia = new Servicos.Embarcador.Integracao.Intelipost.IntegracaoOcorrencia(unitOfWork);

            Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao ocorrencia = repOcorrencia.BuscarPorCodigo(1226);
            servicoIntegracaoOcorrencia.EnviarOcorrencia(ocorrencia);
        }*/

        //private void TesteGerarCoordenadasCliente(Repositorio.UnitOfWork unitOfWork)
        //{
        //    Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

        //    List<Dominio.Entidades.Cliente> clientes = repCliente.BuscarClientesSemCoordenada(10);

        //    Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
        //    Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();

        //    Servicos.Embarcador.Carga.CargaRotaFrete.AtualizarCoordenadas(clientes, unitOfWork, configuracaoIntegracao?.APIKeyGoogle, false);
        //}
        private void TesteIntegracaoTrafegus(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao integracao = repCargaCargaIntegracao.BuscarPorCodigo(3066);


            new Servicos.Embarcador.Integracao.Trafegus.IntegracaoCargaTrafegus(unitOfWork, TipoServicoMultisoftware).IntegrarCarga(integracao);
        }

        private void TesteIntegracaoDocumentoFiscais(Repositorio.UnitOfWork unitOfWork)
        {
            new Servicos.Embarcador.Carga.Carga(unitOfWork).ProcessarCargaEmProcessamentoDocumentosFiscais(unitOfWork, "", AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador, Auditado, Cliente);
        }

        private void TesteIntegracaoCarregamento(Repositorio.UnitOfWork unitOfWork)
        {
            new Servicos.Embarcador.Integracao.IntegracaoCarregamento(unitOfWork).VerificarIntegracoesPendentes();
        }

        private void TesteCanhotoAvulso(Repositorio.UnitOfWork unitOfWork)
        {
            var repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
            var canhoto = repCanhoto.BuscarPorCodigo(3984);

            //Servicos.Embarcador.Canhotos.CanhotoIntegracao.GerarIntegracaoDigitalizacaoCanhoto(canhoto, unitOfWork);

            //Servicos.Embarcador.Canhotos.CanhotoIntegracao.VerificarIntegracoesCanhotos(unitOfWork, "", AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);

        }

        private void TestarCargaMob(Repositorio.UnitOfWork unitOfWork)
        {
            //Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            //var cargas =  repCarga.BuscarPorCodigoEmbarcador("0001228563");

            //var carga = repCarga.BuscarCargaPorMotorista(9556);

            //Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.ConverterCargaControleEntrega(carga, null, unitOfWork);

        }

        private void IntegracaoCargaTrafegus(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork);
            var cargaIntegracao = repCargaIntegracao.BuscarPorCarga(1601).FirstOrDefault();


            new Servicos.Embarcador.Integracao.Trafegus.IntegracaoCargaTrafegus(unitOfWork, TipoServicoMultisoftware).IntegrarCarga(cargaIntegracao);

        }

        private void AtualizarDistancia(Repositorio.UnitOfWork unitOfWork)
        {
            //Codigo Carga--1745
            //Servicos.Embarcador.Logistica.Monitoramento.ControleDistancia controleDistancia = new Servicos.Embarcador.Logistica.Monitoramento.ControleDistancia();
            //controleDistancia.Iniciar(unitOfWork);

        }

        private void AtualizarStatus(Repositorio.UnitOfWork unitOfWork)
        {

            //Servicos.Embarcador.Logistica.Monitoramento.ControleStatusAtual controleStatusAtual = new Servicos.Embarcador.Logistica.Monitoramento.ControleStatusAtual();
            //controleStatusAtual.Iniciar(unitOfWork);
        }

        //private void CompararRotas(Repositorio.UnitOfWork unitOfWork)
        //{

        //    int cargaCodigo = Request.GetIntParam("carga");
        //    int veiculo = Request.GetIntParam("veiculo");
        //    int idEquipamento = Request.GetIntParam("IDEquipamento");

        //    Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
        //    Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento = null;

        //    Repositorio.Embarcador.Cargas.CargaRotaFrete repCargaRotaFrete = new Repositorio.Embarcador.Cargas.CargaRotaFrete(unitOfWork);
        //    Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete = null;
        //    if (cargaCodigo > 0)
        //    {
        //        monitoramento = repMonitoramento.BuscarUltimoPorCarga(cargaCodigo);
        //        if (monitoramento == null)
        //            cargaRotaFrete = repCargaRotaFrete.BuscarPorCarga(cargaCodigo);
        //    }


        //    List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> wayPointsPrevistos = Servicos.Embarcador.Logistica.Polilinha.Decodificar(monitoramento?.PolilinhaPrevista ?? cargaRotaFrete?.PolilinhaRota ?? string.Empty);
        //    List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> wayPointsRealizados = Servicos.Embarcador.Logistica.Polilinha.Decodificar(monitoramento?.PolilinhaRealizada ?? string.Empty);

        //    int totalPontosproximos = 0;

        //    double distanciaTotalPrevista = 0;
        //    double distanciaTotalRealizada = 0;

        //    Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPointPrevistoAnterior = null;
        //    Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPointRealizadoAnterior = null;

        //    foreach (Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPointPrevisto in wayPointsPrevistos)
        //    {
        //        if (wayPointPrevistoAnterior != null)
        //            distanciaTotalPrevista += Servicos.Embarcador.Logistica.Polilinha.CalcularDistancia(wayPointPrevisto.Latitude, wayPointPrevisto.Longitude, wayPointPrevistoAnterior.Latitude, wayPointPrevistoAnterior.Longitude);

        //        wayPointRealizadoAnterior = null;
        //        distanciaTotalRealizada = 0;
        //        double menorDistancia = 1000000;
        //        foreach (Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPointRealizado in wayPointsRealizados)
        //        {
        //            if (wayPointRealizadoAnterior != null)
        //                distanciaTotalRealizada += Servicos.Embarcador.Logistica.Polilinha.CalcularDistancia(wayPointRealizado.Latitude, wayPointRealizado.Longitude, wayPointRealizadoAnterior.Latitude, wayPointRealizadoAnterior.Longitude);

        //            double distanciaAtual = Servicos.Embarcador.Logistica.Polilinha.CalcularDistancia(wayPointPrevisto.Latitude, wayPointPrevisto.Longitude, wayPointRealizado.Latitude, wayPointRealizado.Longitude);
        //            if (distanciaAtual < menorDistancia)
        //                menorDistancia = distanciaAtual;

        //            wayPointRealizadoAnterior = wayPointRealizado;
        //        }

        //        if (menorDistancia < 50)
        //        {
        //            totalPontosproximos++;
        //        }

        //        wayPointPrevistoAnterior = wayPointPrevisto;
        //    }

        //}

        private void TesteProcessarArquivosRecebimento(Repositorio.UnitOfWork unitOfWork)
        {
            new Servicos.Embarcador.Integracao.KuehneNagel.IntegracaoKuehneNagel(unitOfWork).ProcessarArquivosRecebimento();
        }

        private void TesteIntegracaoFaturaMarfrig(Repositorio.UnitOfWork unitOfWork)
        {
            new Servicos.Embarcador.Integracao.Marfrig.IntegracaoMarfrig(unitOfWork).IntegrarCTeTituloReceber(new Repositorio.Embarcador.Integracao.IntegracaoMarfrigCteTitulosReceber(unitOfWork).BuscarPorCodigo(244718), false, this.Usuario);
        }

        private void TestePreviaCustoPorCarga(Repositorio.UnitOfWork unitOfWork)
        {
            new Servicos.Embarcador.Carga.Carga(unitOfWork).ObterPreviaCustoCarga(14, unitOfWork);
        }

        private void TesteEnvioEmail(Repositorio.UnitOfWork unitOfWork)
        {
            new Servicos.Embarcador.Carga.CargaCTe(unitOfWork).EnviarEmailPreviaDocumentosCargaCte(5426);
        }

        private void TesteIntegracaoComDocumentos(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            new Servicos.Embarcador.Integracao.IntegracaoCarga(unitOfWork).IniciarIntegracoesComDocumentos(repositorioCarga.BuscarPorCodigo(124530), unitOfWork, TipoServicoMultisoftware);
            unitOfWork.CommitChanges();
        }

        private void TestarIntegracaoTrizy(Repositorio.UnitOfWork unitOfWork)
        {
            List<int> codigos = new List<int>() { 2999 };
            foreach (int i in codigos)
            {
                Servicos.Embarcador.Integracao.Trizy.IntegracaoTrizy.IntegrarCargaAPPTrizy(new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork).BuscarPorCodigo(i), unitOfWork);
            }
            unitOfWork.Rollback();
        }

        private void TestarIntegracaoDadosTransporteTrizy(Repositorio.UnitOfWork unitOfWork)
        {
            List<int> codigos = new List<int>() { 100829 };
            foreach (int i in codigos)
            {
                Servicos.Embarcador.Integracao.Trizy.IntegracaoTrizy.IntegrarCargaDadosTransporteAPPTrizy(new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(unitOfWork).BuscarPorCodigo(i), unitOfWork);
            }
            unitOfWork.Rollback();
        }

        private void TestarIntegracaoBRK(Repositorio.UnitOfWork unitOfWork)
        {
            List<int> codigos = new List<int>() { 719 };
            foreach (int i in codigos)
            {
                Servicos.Embarcador.Integracao.BrasilRisk.IntegracaoBrasilRisk.IntegrarCarga(new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork).BuscarPorCodigo(i), unitOfWork, TipoServicoMultisoftware);
            }
            unitOfWork.Rollback();
        }

        private void ProcessatTipoOperacao(Repositorio.UnitOfWork unitOfWork, int codigoCarga = 125975)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga);
            new Servicos.Embarcador.Carga.Carga(unitOfWork).ProcessarRegrasTipoOperacao(ref carga, repositorioCargaPedido.BuscarPorCarga(codigoCarga), unitOfWork);
        }

        private void testarProcassamentoNotaFiscalDestinadoTXMLNOTA(Repositorio.UnitOfWork unitOfWork)
        {

            string nfeXML = "<nfeProc versao=\"4.00\" xmlns=\"http://www.portalfiscal.inf.br/nfe\"><NFe xmlns=\"http://www.portalfiscal.inf.br/nfe\"><infNFe Id=\"NFe35231001898598000493550030000239921388115989\" versao=\"4.00\"><ide><cUF>35</cUF><cNF>38811598</cNF><natOp>Venda de produção do estabelecimento</natOp><mod>55</mod><serie>3</serie><nNF>23992</nNF><dhEmi>2023-10-30T11:03:32-03:00</dhEmi><tpNF>1</tpNF><idDest>1</idDest><cMunFG>3545209</cMunFG><tpImp>1</tpImp><tpEmis>1</tpEmis><cDV>9</cDV><tpAmb>2</tpAmb><finNFe>1</finNFe><indFinal>0</indFinal><indPres>0</indPres><procEmi>0</procEmi><verProc>3.122.1.1</verProc></ide><emit><CNPJ>01898598000493</CNPJ><xNome>Stepan Quimica LTDA</xNome><xFant>Stepan Salto</xFant><enderEmit><xLgr>Rua Batalha do Tuiuti</xLgr><nro>1730</nro><xBairro>Distrito Industrial Lageado</xBairro><cMun>3545209</cMun><xMun>Salto</xMun><UF>SP</UF><CEP>13329422</CEP><xPais>Brasil</xPais><fone>1140279800</fone></enderEmit><IE>600194014110</IE><CRT>3</CRT></emit><dest><CNPJ>01615814006487</CNPJ><xNome>NF-E EMITIDA EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL</xNome><enderDest><xLgr>AV.DAS INDUSTRIAS PARTE III</xLgr><nro>315</nro><xBairro>DISTRITO INDUSTRIAL</xBairro><cMun>3556701</cMun><xMun>VINHEDO</xMun><UF>SP</UF><CEP>13280000</CEP><cPais>1058</cPais><xPais>Brasil</xPais><fone>1137037382</fone></enderDest><indIEDest>1</indIEDest><IE>714107820118</IE><email>nfe@stepan.com</email></dest><det nItem=\"1\"><prod><cProd>000000000001103093</cProd><cEAN>SEM GTIN</cEAN><xProd>AMMONYX LMDO             PQ292 DR90 191k</xProd><NCM>34024900</NCM><CFOP>5101</CFOP><uCom>KG</uCom><qCom>191.0000</qCom><vUnCom>17.2494240838</vUnCom><vProd>3294.64</vProd><cEANTrib>SEM GTIN</cEANTrib><uTrib>KG</uTrib><qTrib>191.0000</qTrib><vUnTrib>17.2494240838</vUnTrib><indTot>1</indTot><xPed>PMT 718</xPed><nFCI>A8E5FC16-45F2-41B1-AE75-C2748A71CF07</nFCI></prod><imposto><ICMS><ICMS00><orig>5</orig><CST>00</CST><modBC>3</modBC><vBC>3294.64</vBC><pICMS>18.0000</pICMS><vICMS>593.04</vICMS></ICMS00></ICMS><IPI><cEnq>999</cEnq><IPITrib><CST>50</CST><vBC>3294.64</vBC><pIPI>3.2500</pIPI><vIPI>107.08</vIPI></IPITrib></IPI><PIS><PISAliq><CST>01</CST><vBC>2701.60</vBC><pPIS>1.6500</pPIS><vPIS>44.58</vPIS></PISAliq></PIS><COFINS><COFINSAliq><CST>01</CST><vBC>2701.60</vBC><pCOFINS>7.6000</pCOFINS><vCOFINS>205.32</vCOFINS></COFINSAliq></COFINS></imposto></det><total><ICMSTot><vBC>3294.64</vBC><vICMS>593.04</vICMS><vICMSDeson>0.00</vICMSDeson><vFCP>0.00</vFCP><vBCST>0.00</vBCST><vST>0.00</vST><vFCPST>0.00</vFCPST><vFCPSTRet>0.00</vFCPSTRet><vProd>3294.64</vProd><vFrete>0.00</vFrete><vSeg>0.00</vSeg><vDesc>0.00</vDesc><vII>0.00</vII><vIPI>107.08</vIPI><vIPIDevol>0.00</vIPIDevol><vPIS>44.58</vPIS><vCOFINS>205.32</vCOFINS><vOutro>0.00</vOutro><vNF>3401.72</vNF></ICMSTot></total><transp><modFrete>1</modFrete><transporta><CNPJ>03887331000779</CNPJ><xNome>TEGMA CARGAS ESPECIAIS LTDA</xNome><IE>7120973410190</IE><xEnder>AV DURVAL ALVES PEREI 750, SALA DLI</xEnder><xMun>IGARAPE</xMun><UF>MG</UF></transporta></transp><cobr><fat><nFat>0000000000</nFat><vOrig>3401.72</vOrig><vDesc>0.00</vDesc><vLiq>3401.72</vLiq></fat><dup><nDup>001</nDup><dVenc>2023-11-30</dVenc><vDup>3401.72</vDup></dup></cobr><pag><detPag><tPag>15</tPag><vPag>3401.72</vPag></detPag></pag><infAdic><infCpl>condição de pagamento 45 DIAS - dias 16 e 30</infCpl></infAdic><compra><xPed>PMT 718</xPed></compra><infRespTec><CNPJ>00910509000171</CNPJ><xContato>Douglas de Souza Pereira</xContato><email>douglas.pereira@thomsonreuters.com</email><fone>1147009050</fone></infRespTec></infNFe><Signature xmlns=\"http://www.w3.org/2000/09/xmldsig#\"><SignedInfo><CanonicalizationMethod Algorithm=\"http://www.w3.org/TR/2001/REC-xml-c14n-20010315\" /><SignatureMethod Algorithm=\"http://www.w3.org/2000/09/xmldsig#rsa-sha1\" /><Reference URI=\"#NFe35231001898598000493550030000239921388115989\"><Transforms><Transform Algorithm=\"http://www.w3.org/2000/09/xmldsig#enveloped-signature\" /><Transform Algorithm=\"http://www.w3.org/TR/2001/REC-xml-c14n-20010315\" /></Transforms><DigestMethod Algorithm=\"http://www.w3.org/2000/09/xmldsig#sha1\" /><DigestValue>RL4SRh6ahlySfvbC46VSTk3vKgY=</DigestValue></Reference></SignedInfo><SignatureValue>ZC28RUFywYxFGvWoOtcLb50V6y0BvyjOsLUMza0366r6O8mKyN33+qekN+2pdycsi6yoafNOUq60kR8Z7THKs4aLjij++kmL76HzpTI1P4EgERBu7ZY0Xd1A1tcCenRbHIrcluk4Ynib1yALbwlLs6q7U/NsyLeCokSixMoETuJYpmLiwWeAaxqVDi9D+K9hG44bvbgzHtWHLKlCpWHZqpzmjJN24aWaTI4X0R9mg0T8FYenCWh3bMK9En7FKniKBXOHQuFzYDBmBXjJRYlAi5rpMH5EwHGBRe9eGcrwWRqnwYdZQIjaHC2XsdoQrU3fCt5YWwcZ1RZhchgJkpEmYg==</SignatureValue><KeyInfo><X509Data><X509Certificate>MIIH6TCCBdGgAwIBAgIIJVzPagg8s2YwDQYJKoZIhvcNAQELBQAwdTELMAkGA1UEBhMCQlIxEzARBgNVBAoMCklDUC1CcmFzaWwxNjA0BgNVBAsMLVNlY3JldGFyaWEgZGEgUmVjZWl0YSBGZWRlcmFsIGRvIEJyYXNpbCAtIFJGQjEZMBcGA1UEAwwQQUMgU0VSQVNBIFJGQiB2NTAeFw0yMzAyMjcxMTQyMDBaFw0yNDAyMjcxMTQxNTlaMIIBBjELMAkGA1UEBhMCQlIxCzAJBgNVBAgMAlNQMRIwEAYDVQQHDAlTYW8gUGF1bG8xEzARBgNVBAoMCklDUC1CcmFzaWwxNjA0BgNVBAsMLVNlY3JldGFyaWEgZGEgUmVjZWl0YSBGZWRlcmFsIGRvIEJyYXNpbCAtIFJGQjEWMBQGA1UECwwNUkZCIGUtQ05QSiBBMTEWMBQGA1UECwwNQUMgU0VSQVNBIFJGQjEXMBUGA1UECwwOMTQ2MDIyNjkwMDAxNTIxEzARBgNVBAsMClBSRVNFTkNJQUwxKzApBgNVBAMMIlNURVBBTiBRVUlNSUNBIExUREE6MDE4OTg1OTgwMDAxNDAwggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIBAQDZWuNyIwOzGXtYHsN+9LT5PfY6pUwsZvBoQvD7+A3L0y5cn1AaMnMZw34/9HUjqm6EIAUPB3mCPoECrShggtw/nXOX9LqHA4HfAX1+Q4QunAQSuDy2axM4P9JZow1oZ1T9lxaK4iqgSieikoTRGnjquEXWkoBiULMlg5m1sJFlwOaVUuUtwWIgwyfpF0wRChT4grdqa5usQMFO/THmQmiG3nRrrTH/P84r4XLw41CKlPolpjz10ydUJ6q69xgJyLVBjM5W6h09Lixh4CJumDz1Gp0xw5Af60gmfiHCpb+BHyr/KObgxNpLW+26hPoEErczBgZvwCGvEs73YwiR3RkzAgMBAAGjggLoMIIC5DAJBgNVHRMEAjAAMB8GA1UdIwQYMBaAFOzxQVFXqOY66V6zoCL5CIq1OoePMIGZBggrBgEFBQcBAQSBjDCBiTBIBggrBgEFBQcwAoY8aHR0cDovL3d3dy5jZXJ0aWZpY2Fkb2RpZ2l0YWwuY29tLmJyL2NhZGVpYXMvc2VyYXNhcmZidjUucDdiMD0GCCsGAQUFBzABhjFodHRwOi8vb2NzcC5jZXJ0aWZpY2Fkb2RpZ2l0YWwuY29tLmJyL3NlcmFzYXJmYnY1MIG4BgNVHREEgbAwga2BFEFQRVJESUdBT0BTVEVQQU4uQ09NoCEGBWBMAQMCoBgTFkVEVUFSRE8gTE9QRVMgRE8gQ09VVE+gGQYFYEwBAwOgEBMOMDE4OTg1OTgwMDAxNDCgPgYFYEwBAwSgNRMzMDMwODE5NzUyNTA0ODQ1MDgxNzAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwoBcGBWBMAQMHoA4TDDAwMDAwMDAwMDAwMDBxBgNVHSAEajBoMGYGBmBMAQIBDTBcMFoGCCsGAQUFBwIBFk5odHRwOi8vcHVibGljYWNhby5jZXJ0aWZpY2Fkb2RpZ2l0YWwuY29tLmJyL3JlcG9zaXRvcmlvL2RwYy9kZWNsYXJhY2FvLXJmYi5wZGYwHQYDVR0lBBYwFAYIKwYBBQUHAwIGCCsGAQUFBwMEMIGdBgNVHR8EgZUwgZIwSqBIoEaGRGh0dHA6Ly93d3cuY2VydGlmaWNhZG9kaWdpdGFsLmNvbS5ici9yZXBvc2l0b3Jpby9sY3Ivc2VyYXNhcmZidjUuY3JsMESgQqBAhj5odHRwOi8vbGNyLmNlcnRpZmljYWRvcy5jb20uYnIvcmVwb3NpdG9yaW8vbGNyL3NlcmFzYXJmYnY1LmNybDAdBgNVHQ4EFgQUZ1KSpVEDdZunwVaIuCbFuYcFwqgwDgYDVR0PAQH/BAQDAgXgMA0GCSqGSIb3DQEBCwUAA4ICAQAj6FNq/fshJ098TODSCVa+fOP0mE/Rp4xY7xH/kDXTCkbhzFBhXI2yUDgDQiSGA+GfvlqXJbw4AhZWo/PfZUdU/wVjIBKmKuDYTV8TODklfaY3+JeThkRokKS2NHUiewoWPQ85s0by1oiMteX7gAmSuX+QqjJxwbZVqZ4TsUkoU71znMTq7qhviVyWAIEM+H+VoHfu0JhslR9rTwgUq00WOaUFIyr+GnRTVDTT0ulBx39DsZNKy76w6XsR5kpV1OFSI1irfE82f57mpTUvKKgxDIJS5DhxeIW+BZ9wV4NXDZtVvpeC/qUkoZf9LtQohhqIS43BzdOxM7Oi6ed6E1enD5upzPL9ZrMku+6Kq+FFRgNDC8+bPCAIdAQTnDpSh+XKSbFerMAmIdV7ZqZ2hqDdvLYHD9/QFqjJ2yE8nzOebjVXcnXZjDzYsj4ujEF3DFHG4d9HZAyCBZPmm364Pj11vftfvZlrgTuJE61ga5AGhG0r3vXuP8sWjnUOjDgZpm2v42KHioevRJKeqxM35Lc7m++e+Ils6TbxH4cSLTIHPjoQG8jWicM0RUpnl4NGB3qbzkwhi9Cqxpj59mrbXhQ/xwp6HJu08A8wi57AK4QUM5hPDLt6sgDqVnaKOrhrUZ3f77ILEcd7zkyYhDJtrVwX54SFHaPhKLWZEHtHxy9ifw==</X509Certificate></X509Data></KeyInfo></Signature></NFe><protNFe versao=\"4.00\"><infProt Id=\"Id135230006175238\"><tpAmb>2</tpAmb><verAplic>SP_NFE_PL009_V4</verAplic><chNFe>35231001898598000493550030000239921388115989</chNFe><dhRecbto>2023-10-30T11:03:36-03:00</dhRecbto><nProt>135230006175238</nProt><digVal>RL4SRh6ahlySfvbC46VSTk3vKgY=</digVal><cStat>100</cStat><xMotivo>Autorizado o uso da NF-e</xMotivo></infProt></protNFe></nfeProc>";
            MultiSoftware.NFe.v400.NotaFiscalProcessada.TNfeProc nota = null;
            var serializer = new XmlSerializer(typeof(MultiSoftware.NFe.v400.NotaFiscalProcessada.TNfeProc));
            using (StringReader reader = new StringReader(nfeXML))
            {
                nota = (MultiSoftware.NFe.v400.NotaFiscalProcessada.TNfeProc)serializer.Deserialize(reader);
            }

            Repositorio.Empresa repoEmpresa = new Repositorio.Empresa(unitOfWork);
            var empresa = repoEmpresa.BuscarPorCNPJ(nota.NFe.infNFe.dest.Item);

            Servicos.Embarcador.NotaFiscal.NotaFiscalEletronica servnota = new Servicos.Embarcador.NotaFiscal.NotaFiscalEletronica();
            servnota.GerarRegistroNotaFiscal(nota, unitOfWork, empresa);
        }

        private void GerarCarregamentosSessao(Repositorio.UnitOfWork unitOfWork, string stringConexao)
        {
            int codigoSessao = 418;
            bool pedidosSemCarregamento = true;
            Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido sessaoRoteirizadorPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido(unitOfWork);
            var pedidosSessao = sessaoRoteirizadorPedido.PedidosSessaoRoteirizador(codigoSessao);
            List<int> codigosPedidos = (from o in pedidosSessao select o.Pedido.Codigo).ToList();
            if (pedidosSemCarregamento)
                codigosPedidos = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork).BuscarCodigosPorCodigosSemCarregamento(codigosPedidos, codigoSessao);

            Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga servicoMontagemCarga = new Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga(unitOfWork, null, stringConexao);
            servicoMontagemCarga.GerarCarregamentoAutomatico(codigosPedidos, codigoSessao, TipoServicoMultisoftware);
            unitOfWork.CommitChanges();
        }

        private void TesteIntegracaoCanhoto(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Canhotos.CanhotoIntegracao repCanhotoIntegracao = new Repositorio.Embarcador.Canhotos.CanhotoIntegracao(unitOfWork);
            new Servicos.Embarcador.Integracao.Comprovei.IntegracaoComprovei(unitOfWork).IntegrarIACanhoto(repCanhotoIntegracao.BuscarPorCodigo(216));
        }

        private void TesteDeGatilhoAlertaSensorTemperaturaComProblemaIntegracaoMonitoramento(Repositorio.UnitOfWork unitOfWork)
        {

            Servicos.Embarcador.Monitoramento.Eventos.SensorDeTemperaturaComProblema sensorDeTemperaturaComProblema = new Servicos.Embarcador.Monitoramento.Eventos.SensorDeTemperaturaComProblema(unitOfWork);

            //Dados Mock

            List<Dominio.ObjetosDeValor.Embarcador.Logistica.AlertaMonitor> listaAlertas = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.AlertaMonitor>();
            var alerta = ObterAlertaMonitorMock();

            listaAlertas.Add(alerta);

            var monitoramentoEvento = ObterMonitoramentoEventoMock(unitOfWork);
            var monitoramentoProcessarEvento = ObterMonitoramentoProcessarEventoMock();
            var monitoramento = ObterMonitoramentoMock(unitOfWork);

            monitoramento.Carga.TipoDeCarga.ControlaTemperatura = true;

            sensorDeTemperaturaComProblema.Processar(monitoramentoEvento, monitoramentoProcessarEvento, monitoramento, alertas: listaAlertas);

        }

        void TesteValePedagioEFrete(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio = repCargaValePedagio.BuscarPorCodigo(326705);
            Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(13014);

            Servicos.Embarcador.Integracao.EFrete.ValePedagio servicoValePedagioEFrete = new Servicos.Embarcador.Integracao.EFrete.ValePedagio(unitOfWork);
            servicoValePedagioEFrete.GerarCompraValePedagio(cargaValePedagio, carga, TipoServicoMultisoftware.MultiEmbarcador);
        }

        private void TesteFinalizarAbastecimentoAutomaticoViaIntegracao(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.WebService.Abastecimento.FinalizarAbastecimentoConecttec finalizarAbastecimento = new Dominio.ObjetosDeValor.WebService.Abastecimento.FinalizarAbastecimentoConecttec();

            finalizarAbastecimento.Price = 24;
            finalizarAbastecimento.AuthId = "33497416-0df2-422a-a252-f239cc2396a3";
            finalizarAbastecimento.ReserveId = "12cd566c-dbb9-4c7f-974f-1d4292455fc6";
            finalizarAbastecimento.Volume = 111;

            Servicos.WebService.Abastecimento.Abastecimento servicoAbastecimento = new Servicos.WebService.Abastecimento.Abastecimento(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, _conexao.AdminStringConexao);
            servicoAbastecimento.FinalizarAbastecimentoConecttec(finalizarAbastecimento);
        }

        private void TesteEmissaoCTe(Repositorio.UnitOfWork unitOfWork, string stringConexao)
        {
            Servicos.Embarcador.Carga.CTe cteServico = new Servicos.Embarcador.Carga.CTe(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(14185);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaPedidos = repCargaPedido.BuscarPorCarga(14185);

            string teste = cteServico.EmitirCTE(listaPedidos, carga, unitOfWork, TipoServicoMultisoftware.MultiEmbarcador, null, 0);

        }
        private async Task ReprocessarGeracaoOcorrenciaEventosEntrega(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Servicos.Embarcador.Carga.ControleEntrega.CargaEntregaEvento servicoCargaEntregaEvento = new(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repCargaEntregaPedido = new(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega repConfiguracaoOcorrenciaEntrega = new(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega repConfiguracaoControleEntrega = new(unitOfWork);
            Repositorio.TipoDeOcorrenciaDeCTe repTipoDeOcorrenciaDeCTe = new(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaEvento repCargaEntregaEvento = new(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.OcorrenciaTipoIntegracao repOcorrenciaTipoIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaTipoIntegracao(unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.EventoColetaEntrega eventoColetaEntrega = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EventoColetaEntrega.Confirma;

            IList<Dominio.ObjetosDeValor.Embarcador.Carga.CargaEntrega.CargaEntregaGenerico.BuscarReprocessarGeracaoOcorrenciaEventosEntrega> entregasPentende = repCargaEntrega.BuscarReprocessarGeracaoOcorrenciaEventosEntrega();
            List<Dominio.Entidades.TipoDeOcorrenciaDeCTe> tiposDeOcorrencias = repTipoDeOcorrenciaDeCTe.BuscarPorCodigos(entregasPentende.Select(e => e.CodigoOcorrencia).Distinct().ToList());
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega> configuracaoOcorrenciaEntregas = repConfiguracaoOcorrenciaEntrega.BuscarRegrasPorEvento(eventoColetaEntrega);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega = await repConfiguracaoControleEntrega.BuscarPrimeiroRegistroAsync();

            List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaEntrega.CargaEntregaGenerico.BuscarReprocessarGeracaoOcorrenciaEventosEntrega> listaTemp = new();
            int loteTamanho = 2000;

            int total = entregasPentende.Count;
            for (int i = 0; i < total; i += loteTamanho)
            {
                listaTemp = entregasPentende.Skip(i).Take(loteTamanho).ToList();

                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas = repCargaEntrega.BuscarPorCodigos(listaTemp.Select(e => e.CodigoEntrega).Distinct().ToList());
                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEvento> cargaEntregaEventos = repCargaEntregaEvento.BuscarPorCodigos(listaTemp.Select(e => e.CodigoCargaEntregaEvento).Distinct().ToList(), false);

                foreach (Dominio.ObjetosDeValor.Embarcador.Carga.CargaEntrega.CargaEntregaGenerico.BuscarReprocessarGeracaoOcorrenciaEventosEntrega entrega in listaTemp)
                {
                    await unitOfWork.StartAsync(cancellationToken);

                    Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = cargaEntregas.FirstOrDefault(cargaEntrega => cargaEntrega.Codigo == entrega.CodigoEntrega);
                    if (cargaEntrega == null)
                    {
                        await unitOfWork.RollbackAsync();
                        continue;
                    }

                    DateTime dataEvento = entrega.DataOcorrencia;
                    decimal? latitude = entrega.Latitude;
                    decimal? longitude = entrega.Longitude;
                    OrigemSituacaoEntrega origemSituacao = (OrigemSituacaoEntrega)entrega.OrigemOcorrencia;
                    Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoDeOcorrencia = tiposDeOcorrencias.FirstOrDefault(tipo => tipo.Codigo == entrega.CodigoOcorrencia);
                    //List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaEntregaPedido.BuscarPedidosPorCargaEntrega(cargaEntrega.Codigo);
                    //List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tiposIntegracao = repOcorrenciaTipoIntegracao.BuscarIntegracaoPorTipoOcorrencia(tipoDeOcorrencia.Codigo);

                    if (entrega.CodigoCargaEntregaEvento == 0)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntregaEvento cargaEntregaEvento = new()
                        {
                            Carga = cargaEntrega.CargaOrigem != null ? cargaEntrega.CargaOrigem : cargaEntrega.Carga,
                            CargaEntrega = cargaEntrega,
                            DataOcorrencia = dataEvento,
                            DataPosicao = dataEvento,
                            DataPrevisaoRecalculada = cargaEntrega.DataReprogramada,
                            EventoColetaEntrega = eventoColetaEntrega,
                            TipoDeOcorrencia = tipoDeOcorrencia,
                            Latitude = latitude,
                            Longitude = longitude,
                            Origem = origemSituacao,
                        };
                        servicoCargaEntregaEvento.GerarCargaEntregaEvento(cargaEntregaEvento, configuracaoControleEntrega, true);
                    }
                    //else
                    //{
                    //    Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEvento cargaEntregaEvento = cargaEntregaEventos.FirstOrDefault(e => e.Codigo == entrega.CodigoCargaEntregaEvento);
                    //    Servicos.Embarcador.Carga.ControleEntrega.CargaEntregaEventoIntegracao servicoCargaEntregaEventoIntegracao = new Servicos.Embarcador.Carga.ControleEntrega.CargaEntregaEventoIntegracao(unitOfWork);
                    //    servicoCargaEntregaEventoIntegracao.GerarIntegracoes(cargaEntregaEvento, tiposIntegracao);
                    //}

                    await unitOfWork.CommitChangesAsync();
                }
            }
        }

        private void TestarGeracaoCargasProximoTrechoRedespacho(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfig = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(14389);
            Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(14389);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfig.BuscarConfiguracaoPadrao();

            Dominio.Entidades.Embarcador.Cargas.Carga CargaGerada = Servicos.Embarcador.Carga.CargaDistribuidor.GerarCargaProximoTrecho(carga, carga.TipoOperacao, 0m, true, null, cargaPedidos, null, configuracao, true, carga.Redespacho, null, TipoServicoMultisoftware.MultiEmbarcador, unitOfWork, false, null, null);
        }
        private void TestarIntegracaoModeloVeicular(Repositorio.UnitOfWork unitOfWork, List<int> codigos)
        {
            foreach (int i in codigos)
            {
                var repositorioModeloVeicular = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
                var modeloVeicular = repositorioModeloVeicular.BuscarPorCodigo(i);
                Servicos.Embarcador.Integracao.Trizy.IntegracaoTrizy.IntegrarModeloVeicular(modeloVeicular, unitOfWork);
            }
            unitOfWork.CommitChanges();
        }

        private void TestarIntegracaoAverbacaoBradesco(Repositorio.UnitOfWork unitOfWork)
        {
            int tentativas = 0;
            Repositorio.AverbacaoCTe repositorioaverbacao = new Repositorio.AverbacaoCTe(unitOfWork);
            Dominio.Entidades.AverbacaoCTe averbacao = repositorioaverbacao.BuscarPorCodigo(335514);

            new Servicos.Embarcador.Integracao.Bradesco.IntegracaoBradescoAverbacao(unitOfWork).AverbarDocumento(averbacao.ApoliceSeguroAverbacao.ApoliceSeguro, averbacao, ref tentativas);
        }

        private void TestarIntegracaoCancelamentoAverbacao(Repositorio.UnitOfWork unitOfWork)
        {
            int tentativas = 0;
            Servicos.CTe servicoCte = new Servicos.CTe(unitOfWork);

            //servicoCte.CancelarAverbacaoOracle(335514, ref tentativas, unitOfWork, _conexao.StringConexao);
        }

        #endregion Métodos Privados para Adicionar a Chamado no Método Global Teste

        #region Objetos Mockeados

        private Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento ObterMonitoramentoEventoMock(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.MonitoramentoEvento repMonitoramentoEvento = new Repositorio.Embarcador.Logistica.MonitoramentoEvento(unitOfWork);
            return repMonitoramentoEvento.BuscarPorCodigo(10, false);
        }

        private Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento ObterMonitoramentoProcessarEventoMock()
        {
            DateTime dataVeiculoPosicao = new DateTime(2025, 2, 6, 12, 30, 0);

            var monitoramentoProcessarEvento = new Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento
            {
                CodigoMonitoramento = 1433,
                CodigoCarga = 17261,
                CodigoVeiculo = 3,
                SensorTemperaturaPosicao = false,
                DataVeiculoPosicao = dataVeiculoPosicao,
                LatitudePosicao = 0,
                LongitudePosicao = 0,

            };

            return monitoramentoProcessarEvento;
        }

        private Dominio.Entidades.Embarcador.Logistica.Monitoramento ObterMonitoramentoMock(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.Monitoramento repMovimento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
            return repMovimento.BuscarPorCodigo(1433, false);
        }

        private Dominio.ObjetosDeValor.Embarcador.Logistica.AlertaMonitor ObterAlertaMonitorMock()
        {
            var alerta = new Dominio.ObjetosDeValor.Embarcador.Logistica.AlertaMonitor
            {
                Codigo = 1,
                CodigoCarga = 17261,
                CodigoMonitoramentoEvento = 10,
                TipoAlerta = TipoAlerta.SensorTemperaturaComProblema,
                Status = AlertaMonitorStatus.EmTratativa,
                CodigoVeiculo = 3,
                DataCadastro = DateTime.Now,
                Data = DateTime.Now,
                TempoReprogramado = 60,
                DataFim = new DateTime(2025, 2, 6, 18, 30, 0),

            };

            return alerta;
        }

        #endregion
    }


}

#region Classes Teste

public class Retorno<T>
{
    #region Propriedades

    public T Objeto { get; set; }

    public bool Status { get; set; }

    public string DataRetorno { get; set; }

    public string Mensagem { get; set; }

    public int CodigoMensagem { get; set; }

    #endregion

    #region Construtores

    public static Retorno<T> CriarRetornoDadosInvalidos(string mensagem, int codigoMensagem)
    {
        return new Retorno<T>
        {
            CodigoMensagem = codigoMensagem,
            DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
            Mensagem = mensagem,
            Status = false
        };
    }

    public static Retorno<T> CriarRetornoDadosInvalidos(string mensagem, T objeto = default(T))
    {
        return new Retorno<T>
        {
            CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos,
            DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
            Mensagem = mensagem,
            Objeto = objeto,
            Status = false
        };
    }

    public static Retorno<T> CriarRetornoDuplicidadeRequisicao(string mensagem, T objeto = default(T))
    {
        return new Retorno<T>
        {
            CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DuplicidadeDaRequisicao,
            DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
            Mensagem = mensagem,
            Objeto = objeto,
            Status = false
        };
    }

    public static Retorno<T> CriarRetornoExcecao(string mensagem, T objeto = default(T))
    {
        return new Retorno<T>
        {
            CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica,
            DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
            Mensagem = mensagem,
            Objeto = objeto,
            Status = false
        };
    }

    public static Retorno<T> CriarRetornoSucesso(T objeto)
    {
        return new Retorno<T>
        {
            CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.Sucesso,
            DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
            Objeto = objeto,
            Status = true
        };
    }

    #endregion
}

#endregion
