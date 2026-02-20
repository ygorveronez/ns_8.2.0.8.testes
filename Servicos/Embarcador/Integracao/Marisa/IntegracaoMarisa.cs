using Amazon.S3;
using Amazon.S3.Model;
using CsvHelper;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;

namespace Servicos.Embarcador.Integracao
{
    public class IntegracaoMarisa
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly string _urlAcessoCliente;

        #endregion Atributos

        #region Constructor

        public IntegracaoMarisa(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, urlAcessoCliente: "") { }

        public IntegracaoMarisa(Repositorio.UnitOfWork unitOfWork, string urlAcessoCliente)
        {
            _unitOfWork = unitOfWork;
            _urlAcessoCliente = urlAcessoCliente;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta IntegrarOcorrenciasColetaEntrega(List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais, Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao integracao)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta httpRequisicaoResposta = new Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta()
            {
                conteudoRequisicao = string.Empty,
                extensaoRequisicao = "json",
                conteudoResposta = string.Empty,
                extensaoResposta = "json",
                sucesso = false,
                mensagem = string.Empty,
            };

            if (notasFiscais == null)
            {
                httpRequisicaoResposta.mensagem = "Nenhuma NFe localizada.";
                return httpRequisicaoResposta;
            }

            Repositorio.Embarcador.Configuracoes.IntegracaoMarisa repIntegracaoMarisa = new Repositorio.Embarcador.Configuracoes.IntegracaoMarisa(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMarisa integracaoMarisa = repIntegracaoMarisa.BuscarDadosIntegracao();

            if (integracaoMarisa == null || !integracaoMarisa.PossuiIntegracaoMarisa)
            {
                httpRequisicaoResposta.mensagem = "Não possui Integração com Marisa ou está desativada.";
                return httpRequisicaoResposta;
            }

            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;

            try
            {
                HttpClient cliente = ObterCliente(integracaoMarisa.Usuario, integracaoMarisa.Senha);
                jsonRequest = ObterObjetoRequisicao(notasFiscais, integracao);

                StringContent content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                var result = cliente.PostAsync(integracaoMarisa.Url, content).Result;

                jsonResponse = result.Content.ReadAsStringAsync().Result;

                httpRequisicaoResposta.conteudoRequisicao = jsonRequest;
                httpRequisicaoResposta.conteudoResposta = jsonResponse;
                httpRequisicaoResposta.httpStatusCode = result.StatusCode;

                if (result.StatusCode != HttpStatusCode.OK)
                    throw new ServicoException("Ocorreu uma falha ao tentar fazer a integração");

                httpRequisicaoResposta.sucesso = true;
            }
            catch (ServicoException excecao)
            {
                Log.TratarErro("Request: " + jsonRequest, "IntegracaoMarisa");
                Log.TratarErro("Response: " + jsonResponse, "IntegracaoMarisa");
                Log.TratarErro(excecao, "IntegracaoMarisa");

                httpRequisicaoResposta.mensagem = excecao.Message;
            }

            return httpRequisicaoResposta;
        }

        public void IntegrarTabelaFrete(Dominio.Entidades.Embarcador.Frete.TabelaFreteIntegrarAlteracaoIntegracao tabelaFreteIntegracao)
        {
            Repositorio.Embarcador.Frete.TabelaFreteIntegrarAlteracaoIntegracao repositorioTabelaFreteIntegracao = new Repositorio.Embarcador.Frete.TabelaFreteIntegrarAlteracaoIntegracao(_unitOfWork);
            byte[] planilhaRequisicao = null;

            tabelaFreteIntegracao.DataIntegracao = DateTime.Now;
            tabelaFreteIntegracao.NumeroTentativas += 1;

            try
            {
                Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete = tabelaFreteIntegracao.TabelaFreteIntegrarAlteracao.TabelaFrete;

                if (string.IsNullOrWhiteSpace(tabelaFrete.Descricao))
                    throw new ServicoException("A descrição da tabela de frete deve ser informado.");

                Repositorio.Embarcador.Configuracoes.IntegracaoMarisa repositorioIntegracaoMarisa = new Repositorio.Embarcador.Configuracoes.IntegracaoMarisa(_unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMarisa integracaoMarisa = repositorioIntegracaoMarisa.BuscarDadosIntegracao();

                if ((integracaoMarisa == null) || !integracaoMarisa.PossuiIntegracaoMarisa)
                    throw new ServicoException("Não existe configuração de integração ativa para a Marisa.");

                if (string.IsNullOrWhiteSpace(integracaoMarisa.EnderecoIntegracaoTabelaMarisa) || string.IsNullOrWhiteSpace(integracaoMarisa.UsuarioIntegracaoTabelaMarisa) || string.IsNullOrWhiteSpace(integracaoMarisa.SenhaIntegracaoTabelaMarisa))
                    throw new ServicoException("Não existe configuração de integração disponível para a Marisa.");

                planilhaRequisicao = ObterCsvTabelaFreteCliente(tabelaFrete);

                List<string> diretorios = integracaoMarisa.EnderecoIntegracaoTabelaMarisa.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                string diretorio = diretorios[0];
                string subdiretorios = string.Empty;

                if (diretorios.Count > 1)
                {
                    diretorios.RemoveAt(0);
                    subdiretorios = $"{string.Join("/", diretorios)}/";
                }

                using (AmazonS3Client clienteRequisicao = new AmazonS3Client(integracaoMarisa.UsuarioIntegracaoTabelaMarisa, integracaoMarisa.SenhaIntegracaoTabelaMarisa, Amazon.RegionEndpoint.USEast1))
                using (MemoryStream arquivoExcelEmMemoria = new MemoryStream())
                {
                    arquivoExcelEmMemoria.Write(planilhaRequisicao, 0, planilhaRequisicao.Length);

                    PutObjectRequest dadosRequisicao = new PutObjectRequest()
                    {
                        InputStream = arquivoExcelEmMemoria,
                        Key = $"{subdiretorios}{tabelaFrete.Descricao}.csv",
                        BucketName = diretorio,
                    };

                    clienteRequisicao.PutObjectAsync(dadosRequisicao);
                }

                tabelaFreteIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                tabelaFreteIntegracao.ProblemaIntegracao = "Integração realizada com sucesso";
            }
            catch (ServicoException excecao)
            {
                tabelaFreteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                tabelaFreteIntegracao.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                tabelaFreteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                tabelaFreteIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a Marisa";
            }

            repositorioTabelaFreteIntegracao.Atualizar(tabelaFreteIntegracao);
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private HttpClient ObterCliente(string usuario, string senha)
        {
            HttpClient cliente = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoMarisa));

            cliente.SetBasicAuthentication(usuario, senha);
            cliente.DefaultRequestHeaders.Accept.Clear();
            cliente.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return cliente;
        }

        #endregion Métodos Privados

        #region Métodos Privados - Integração de Ocorrências de Coleta/Entrega

        private string ObterObjetoRequisicao(List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notas, Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao integracao)
        {
            var ocorrencia = integracao.PedidoOcorrenciaColetaEntrega.TipoDeOcorrencia;

            if (ocorrencia == null)
                return string.Empty;

            string codigoRastreamento = Servicos.Embarcador.GestaoEntregas.NotificaoEntrega.ObterCodigoRastreamentoPedido(integracao.PedidoOcorrenciaColetaEntrega.Pedido, _unitOfWork);
            string linkOcorrencia = Servicos.Embarcador.GestaoEntregas.NotificaoEntrega.ObterURLRastreamentoPedido(codigoRastreamento, _urlAcessoCliente);


            Dominio.ObjetosDeValor.Embarcador.Integracao.Marisa.RequestMarisa requestMarisa = new Dominio.ObjetosDeValor.Embarcador.Integracao.Marisa.RequestMarisa()
            {
                RequestMarisaNumeroPedidosExternos = ObterRequestMarisaNumeroPedidosExternos(integracao),
                NumeroVolumes = string.Empty,
                NumeroRastreio = integracao.PedidoOcorrenciaColetaEntrega.Pedido?.NumeroRastreioCorreios ?? string.Empty,
                NumeroPedidoVenda = integracao.PedidoOcorrenciaColetaEntrega.Pedido?.NumeroPedidoEmbarcador ?? string.Empty,
                RequestMarisaNotaFiscais = ObterRequestMarisaNotaFiscal(integracao),
                DataEntregaEstimada = string.Empty,
                NumeroPedido = integracao.PedidoOcorrenciaColetaEntrega.Pedido?.NumeroPedidoEmbarcador ?? string.Empty,
                URLRastreioPedido = linkOcorrencia,
                RequestMarisaHistorico = ObterRequestMarisaHistorico(integracao)
            };

            return JsonConvert.SerializeObject(requestMarisa, Formatting.Indented);
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Marisa.RequestMarisaNotaFiscal> ObterRequestMarisaNotaFiscal(Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao integracao)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Marisa.RequestMarisaNotaFiscal> requestMarisaNotaFiscalLista = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Marisa.RequestMarisaNotaFiscal>();

            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> listaPedidoXMLNotaFiscal = repositorioPedidoXMLNotaFiscal.BuscarNotasFiscaisPorPedido(integracao.PedidoOcorrenciaColetaEntrega.Pedido.Codigo);

            foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal in listaPedidoXMLNotaFiscal)
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Marisa.RequestMarisaNotaFiscal requestMarisaNotaFiscal = new Dominio.ObjetosDeValor.Embarcador.Integracao.Marisa.RequestMarisaNotaFiscal()
                {
                    NumeroNotaFiscal = xmlNotaFiscal.Numero.ToString(),
                    ChaveAcessoNotaFiscal = xmlNotaFiscal.Chave,
                    SerieNotaFiscal = xmlNotaFiscal.Serie
                };

                requestMarisaNotaFiscalLista.Add(requestMarisaNotaFiscal);
            }

            return requestMarisaNotaFiscalLista;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Marisa.RequestMarisaNumeroPedidosExternos> ObterRequestMarisaNumeroPedidosExternos(Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao integracao)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Marisa.RequestMarisaNumeroPedidosExternos> RequestMarisaNumeroPedidosExternosLista = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Marisa.RequestMarisaNumeroPedidosExternos>();

            Dominio.ObjetosDeValor.Embarcador.Integracao.Marisa.RequestMarisaNumeroPedidosExternos RequestMarisaNumeroPedidosExternos = new Dominio.ObjetosDeValor.Embarcador.Integracao.Marisa.RequestMarisaNumeroPedidosExternos()
            {
                Pedidos = integracao?.PedidoOcorrenciaColetaEntrega?.Pedido?.NumeroPedidoEmbarcador ?? string.Empty
            };

            RequestMarisaNumeroPedidosExternosLista.Add(RequestMarisaNumeroPedidosExternos);

            return RequestMarisaNumeroPedidosExternosLista;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Marisa.RequestMarisaHistorico ObterRequestMarisaHistorico(Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao integracao)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Marisa.RequestMarisaHistorico requestMarisaHistorico = new Dominio.ObjetosDeValor.Embarcador.Integracao.Marisa.RequestMarisaHistorico()
            {
                RequestMarisaExtra = ObterRequestMarisaExtra(),
                StatusFornecedorRemetente = string.Empty,
                MensagemSprinter = string.Empty,
                HashRequest = string.Empty,
                MensagemFornecedor = integracao.PedidoOcorrenciaColetaEntrega.TipoDeOcorrencia?.Descricao ?? string.Empty,
                HistoricoStatusVolumePedidoRemessa = string.Empty,
                DataEvento = integracao?.PedidoOcorrenciaColetaEntrega?.DataOcorrencia.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") ?? string.Empty,
                OrigemRequest = string.Empty,
                CriadoEm = string.Empty,
                RequestMarisaLocalidadeCliente = ObterRequestMarisaRequestMarisaLocalidadeCliente(integracao),
                Anexos = new Dominio.ObjetosDeValor.Embarcador.Integracao.Marisa.RequestMarisaAnexo(),
                StatusVolumePedidoRemessaLocalizado = string.Empty,
                MacroStatus = integracao.PedidoOcorrenciaColetaEntrega.TipoDeOcorrencia?.DescricaoAuxiliar ?? string.Empty,
                CodigoMicroStatus = integracao.PedidoOcorrenciaColetaEntrega.TipoDeOcorrencia?.CodigoIntegracao ?? string.Empty,
                DataDeEvento = string.Empty,
                StatusTracking = string.Empty,
                IdVolumeRemessaPedido = string.Empty,
                DataEventoIso = integracao.PedidoOcorrenciaColetaEntrega?.DataOcorrencia.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") ?? string.Empty,
                RequestMarisaMicroStatusRemessaPedido = ObterRequestMarisaMicroStatusRemessaPedido(integracao)
            };

            return requestMarisaHistorico;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Marisa.RequestMarisaExtra ObterRequestMarisaExtra()
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Marisa.RequestMarisaExtra requestMarisaExtra = new Dominio.ObjetosDeValor.Embarcador.Integracao.Marisa.RequestMarisaExtra()
            {
                TipoHistorico = "Tracking"
            };

            return requestMarisaExtra;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Marisa.RequestMarisaLocalidadeCliente ObterRequestMarisaRequestMarisaLocalidadeCliente(Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao integracao)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Marisa.RequestMarisaLocalidadeCliente requestMarisaLocalidadeCliente = new Dominio.ObjetosDeValor.Embarcador.Integracao.Marisa.RequestMarisaLocalidadeCliente()
            {
                Cidade = integracao.PedidoOcorrenciaColetaEntrega.Alvo?.Cidade ?? string.Empty,
                Bairro = integracao.PedidoOcorrenciaColetaEntrega.Alvo?.Bairro ?? string.Empty,
                Adicionais = integracao.PedidoOcorrenciaColetaEntrega.Alvo?.Complemento ?? string.Empty,
                Referencia = integracao.PedidoOcorrenciaColetaEntrega.Alvo?.Observacao ?? string.Empty,
                Descricao = string.Empty,
                Numero = integracao.PedidoOcorrenciaColetaEntrega.Alvo?.Numero ?? string.Empty,
                Longitude = string.Empty,
                Endereco = integracao.PedidoOcorrenciaColetaEntrega.Alvo?.Endereco ?? string.Empty,
                Latitude = integracao.PedidoOcorrenciaColetaEntrega.Alvo?.Latitude ?? string.Empty,
                CodigoEstado = integracao.PedidoOcorrenciaColetaEntrega.Alvo?.Localidade?.Estado?.Sigla ?? string.Empty,
                CodigoPostal = integracao.PedidoOcorrenciaColetaEntrega.Alvo?.CEP ?? string.Empty
            };

            return requestMarisaLocalidadeCliente;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Marisa.RequestMarisaMicroStatusRemessaPedido ObterRequestMarisaMicroStatusRemessaPedido(Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao integracao)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Marisa.RequestMarisaMicroStatusRemessaPedido requestMarisaMicroStatusRemessaPedido = new Dominio.ObjetosDeValor.Embarcador.Integracao.Marisa.RequestMarisaMicroStatusRemessaPedido()
            {
                CodigoMicroStatus = integracao.PedidoOcorrenciaColetaEntrega.TipoDeOcorrencia?.CodigoIntegracao ?? string.Empty,
                Codigo = string.Empty,
                Descricao = string.Empty,
                DescricaoMicroStatus = integracao.PedidoOcorrenciaColetaEntrega.TipoDeOcorrencia?.Descricao ?? string.Empty,
                StatusVolumeRemessaLocalizado = string.Empty,
                MacroStatus = integracao.PedidoOcorrenciaColetaEntrega.TipoDeOcorrencia?.DescricaoAuxiliar ?? string.Empty,
                IdStatusVolumeRemessaOrigem = string.Empty,
                EmNome = string.Empty,
                IdStatusVolumePedidoRemessa = string.Empty,
                DescricaoMicroStatusNome = integracao.PedidoOcorrenciaColetaEntrega.TipoDeOcorrencia?.Descricao ?? string.Empty
            };

            return requestMarisaMicroStatusRemessaPedido;
        }

        #endregion Métodos Privados - Integração de Ocorrências de Coleta/Entrega

        #region Métodos Privados - Integração de Tabela de Frete Cliente

        private byte[] ObterCsvTabelaFreteCliente(Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete)
        {
            List<(string DescricaoColuna, string NomePropriedade)> definicoesColunas = ObterDefinicoesColunas();
            Repositorio.Embarcador.Frete.TabelaFreteCliente repositorioTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(_unitOfWork);
            IList<Dominio.ObjetosDeValor.Embarcador.Integracao.Marisa.TabelaFreteCliente.TabelaFreteClienteValores> valores = repositorioTabelaFreteCliente.BuscarPorIntegracaoMarisa(tabelaFrete.Codigo);

            using (var arquivoEmMemoria = new MemoryStream())
            using (var escritorArquivo = new StreamWriter(arquivoEmMemoria, Encoding.UTF8))
            using (CsvWriter escritorCsv = new CsvWriter(escritorArquivo))
            {
                escritorCsv.Configuration.Delimiter = ";";
                escritorCsv.Configuration.CultureInfo = new System.Globalization.CultureInfo("pt-BR");

                foreach ((string DescricaoColuna, string NomePropriedade) definicaoColuna in definicoesColunas)
                    escritorCsv.WriteField(definicaoColuna.DescricaoColuna);

                escritorCsv.NextRecord();

                foreach (Dominio.ObjetosDeValor.Embarcador.Integracao.Marisa.TabelaFreteCliente.TabelaFreteClienteValores valor in valores)
                {
                    Type tipoValor = valor.GetType();

                    foreach ((string DescricaoColuna, string NomePropriedade) definicaoColuna in definicoesColunas)
                    {
                        PropertyInfo propriedade = string.IsNullOrWhiteSpace(definicaoColuna.NomePropriedade) ? null : tipoValor.GetProperty(definicaoColuna.NomePropriedade);

                        escritorCsv.WriteField((string)propriedade?.GetValue(valor) ?? "0");
                    }

                    escritorCsv.NextRecord();
                }

                return arquivoEmMemoria.ToArray();
            }
        }

        private List<(string DescricaoColuna, string NomePropriedade)> ObterDefinicoesColunas()
        {
            List<(string DescricaoColuna, string NomePropriedade)> definicoesColunas = new List<(string DescricaoColuna, string NomePropriedade)>();

            definicoesColunas.Add(ValueTuple.Create("CD_TRANSPORTADORA", "CodigoIntegracaoTabelaFrete"));
            definicoesColunas.Add(ValueTuple.Create("CD_PARAM_TABELA_FRETE", "ParametroTabelaFrete"));
            definicoesColunas.Add(ValueTuple.Create("CD_TABELA_FRETE", ""));
            definicoesColunas.Add(ValueTuple.Create("CD_REGIAO", ""));
            definicoesColunas.Add(ValueTuple.Create("CD_ROTA", ""));
            definicoesColunas.Add(ValueTuple.Create("CD_TIPO_TABELA", ""));
            definicoesColunas.Add(ValueTuple.Create("CD_TIPO_ENTREGA", ""));
            definicoesColunas.Add(ValueTuple.Create("CD_MEIO_TRANSPORTE", ""));
            definicoesColunas.Add(ValueTuple.Create("QT_DIAS_ENTREGA", "CepDestinoPrazoDiasUteisDescricao"));
            definicoesColunas.Add(ValueTuple.Create("FATOR_CUBAGEM", "FatorCubagemDescricao"));
            definicoesColunas.Add(ValueTuple.Create("PS_CUBADO", ""));
            definicoesColunas.Add(ValueTuple.Create("LOC_CEP_INI", "CepDestinoInicial"));
            definicoesColunas.Add(ValueTuple.Create("LOC_CEP_FIM", "CepDestinoFinal"));
            definicoesColunas.Add(ValueTuple.Create("VL_INI", "PesoInicialDescricao"));
            definicoesColunas.Add(ValueTuple.Create("VL_FIM", "PesoFinalDescricao"));
            definicoesColunas.Add(ValueTuple.Create("VLR_FRETE", "ValorPesoDescricao"));
            definicoesColunas.Add(ValueTuple.Create("VLR_EXCEDENTE", "ValorPesoExcedenteDescricao"));
            definicoesColunas.Add(ValueTuple.Create("VLR_ITR", ""));
            definicoesColunas.Add(ValueTuple.Create("VLR_TDE", ""));
            definicoesColunas.Add(ValueTuple.Create("VLR_CTO", ""));
            definicoesColunas.Add(ValueTuple.Create("VLR_DESPACHO", ""));
            definicoesColunas.Add(ValueTuple.Create("VLR_SEFAZ", ""));
            definicoesColunas.Add(ValueTuple.Create("VLR_PEDAGIO_FRACAO", ""));
            definicoesColunas.Add(ValueTuple.Create("VLR_PEDAGIO", ""));
            definicoesColunas.Add(ValueTuple.Create("VLR_ADEME", ""));
            definicoesColunas.Add(ValueTuple.Create("ID_PERC_TX_AD_VALOREN", "PercentualAdValoremDescricao"));
            definicoesColunas.Add(ValueTuple.Create("VLR_TX_AD_VALOREN", "ValorAdValoremDescricao"));
            definicoesColunas.Add(ValueTuple.Create("VLR_TX_AD_VALOREN_MIN", ""));
            definicoesColunas.Add(ValueTuple.Create("ID_PERC_TX_GRIS", "PercentualGrisDescricao"));
            definicoesColunas.Add(ValueTuple.Create("VLR_TX_GRIS", "ValorGrisDescricao"));
            definicoesColunas.Add(ValueTuple.Create("VLR_TX_GRIS_MIN", ""));
            definicoesColunas.Add(ValueTuple.Create("ID_PERC_TX_REENTREGA", ""));
            definicoesColunas.Add(ValueTuple.Create("VLR_TX_REENTREGA", ""));
            definicoesColunas.Add(ValueTuple.Create("ID_PERC_TX_DEVOLUCAO", ""));
            definicoesColunas.Add(ValueTuple.Create("VLR_TX_DEVOLUCAO", ""));
            definicoesColunas.Add(ValueTuple.Create("VLR_AVISO_RECEBIMENTO", ""));
            definicoesColunas.Add(ValueTuple.Create("VLR_ENVIO_OCORRENCIA", ""));
            definicoesColunas.Add(ValueTuple.Create("VLR_LIMITE_EMBARQUE", ""));
            definicoesColunas.Add(ValueTuple.Create("ID_PERC_TX_SEG_FLUVIAL", ""));
            definicoesColunas.Add(ValueTuple.Create("VLR_TX_SEG_FLUVIAL", ""));
            definicoesColunas.Add(ValueTuple.Create("ID_PERC_TX_DESEMB_SUFRAMA", ""));
            definicoesColunas.Add(ValueTuple.Create("VLR_TX_DESEMB_SUFRAMA", ""));
            definicoesColunas.Add(ValueTuple.Create("VLR_OUTROS", ""));
            definicoesColunas.Add(ValueTuple.Create("PERC_ICMS", "PercentualIcmsDescricao"));
            definicoesColunas.Add(ValueTuple.Create("ID_ATIVO", "Ativo"));
            definicoesColunas.Add(ValueTuple.Create("DT_ADDROW", ""));
            definicoesColunas.Add(ValueTuple.Create("DT_UPDROW", ""));

            return definicoesColunas;
        }

        #endregion Métodos Privados - Integração de Tabela de Frete Cliente
    }
}
