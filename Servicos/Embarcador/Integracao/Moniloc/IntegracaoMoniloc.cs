
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Servicos.Embarcador.Integracao.Moniloc
{
    public class IntegracaoMoniloc
    {
        #region Atributos
        readonly private Repositorio.UnitOfWork _unitOfWork;
        #endregion

        #region Constructor
        public IntegracaoMoniloc(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region Metodos Publicos
        public void IntegrarCarregamento(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao carregamentoIntegracao)
        {
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao repositorioCarregamentoIntegracao = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoUnilever repositorioUnilever = new Repositorio.Embarcador.Configuracoes.IntegracaoUnilever(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            carregamentoIntegracao.NumeroTentativas += 1;
            carregamentoIntegracao.DataIntegracao = DateTime.Now;

            string jsonRequest = "";
            string jsonResponse = "";

            try
            {
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMoniloc integracaoMoniloc = ObterConfiguracaoIntegracaoMoniloc();

                if (string.IsNullOrEmpty(integracaoMoniloc.PortaFTP))
                    throw new ServicoException("Porta FTP não configurada");

                if (string.IsNullOrEmpty(integracaoMoniloc.DiretorioEnvioCVA))
                    throw new ServicoException("Diretorio Envio CVA não configurada");

                (MemoryStream, string) arquivo = ObterArquivo(carregamentoIntegracao.Carregamento);
                StreamReader reader = new StreamReader(arquivo.Item1);

                string mensagemError = string.Empty;
                jsonRequest = reader.ReadToEnd();

                Servicos.FTP.EnviarArquivo(arquivo.Item1, arquivo.Item2, integracaoMoniloc.HostFTP, integracaoMoniloc.PortaFTP, integracaoMoniloc.DiretorioEnvioCVA, integracaoMoniloc.UsuarioFTP, integracaoMoniloc.SenhaFTP, integracaoMoniloc.FTPPassivo, integracaoMoniloc.SSL, out mensagemError, integracaoMoniloc.SFTP);

                if (!string.IsNullOrWhiteSpace(mensagemError))
                {
                    carregamentoIntegracao.ProblemaIntegracao = mensagemError;
                    carregamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                }
                else
                {
                    carregamentoIntegracao.ProblemaIntegracao = "Integração feita com sucesso";
                    carregamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                }

                servicoArquivoTransacao.Adicionar(carregamentoIntegracao, jsonRequest, jsonResponse, "json");
            }
            catch (ServicoException exection)
            {
                carregamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                carregamentoIntegracao.ProblemaIntegracao = exection.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
                carregamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                carregamentoIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a Moniloc";
                servicoArquivoTransacao.Adicionar(carregamentoIntegracao, jsonRequest, jsonResponse, "json");
            }

            repositorioCarregamentoIntegracao.Atualizar(carregamentoIntegracao);
        }

        public void ProcessarArquivoConsumo(System.IO.Stream arquivo, string arquivoDisponivel, string stringConexao, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Moniloc.RecebimentoPedido> recebimentosPedido = LerArquivoConsumo(arquivo);

            Servicos.Embarcador.Pedido.Pedido svcPedido = new Servicos.Embarcador.Pedido.Pedido(_unitOfWork);

            foreach (var recebimentoPedido in recebimentosPedido)
                svcPedido.GerarPedidosConsumoSemCarga(recebimentoPedido, arquivoDisponivel, _unitOfWork, auditado);
        }

        public void ProcessarArquivoConsumoExtra(System.IO.Stream arquivo, string arquivoDisponivel, string stringConexao, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Moniloc.RecebimentoPedidoExtra> recebimentosPedidosExtras = LerArquivoConsumoExtra(arquivo);

            Servicos.Embarcador.Pedido.Pedido svcPedido = new Servicos.Embarcador.Pedido.Pedido(_unitOfWork);

            foreach (var recebimentoPedidoExtra in recebimentosPedidosExtras)
                svcPedido.GerarPedidosConsumoExtraSemCarga(recebimentoPedidoExtra, arquivoDisponivel, _unitOfWork, auditado);
        }

        public void IntegrarCarga(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaIntegracaoPendentes)
        {
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repositorioCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoMoniloc repositorioIntegracaoMoniloc = new Repositorio.Embarcador.Configuracoes.IntegracaoMoniloc(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
            bool localizouArquivo = false;

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMoniloc integracaoMoniloc = repositorioIntegracaoMoniloc.BuscarPrimeiroRegistro();

            cargaIntegracaoPendentes.NumeroTentativas += 1;
            cargaIntegracaoPendentes.DataIntegracao = DateTime.Now;

            try
            {
                if (integracaoMoniloc == null || !integracaoMoniloc.PossuiIntegracaoMoniloc)
                    throw new ServicoException("Integração da moniloc não encontrada!");

                string caminhoDocumentosOUTPUT = CaminhoArquivoRetornoMoniloc();

                string erro = string.Empty;
                Servicos.FTP.DownloadArquivosPasta(integracaoMoniloc.HostFTP, integracaoMoniloc.PortaFTP, integracaoMoniloc.DiretorioRetornoCVA, integracaoMoniloc.UsuarioFTP, integracaoMoniloc.SenhaFTP, integracaoMoniloc.FTPPassivo, integracaoMoniloc.SSL, caminhoDocumentosOUTPUT, out erro, integracaoMoniloc.SFTP, false, "", false, false, true);

                if (!string.IsNullOrWhiteSpace(erro))
                    throw new ServicoException("Erro ao baixar arquivos OUTPUT do FTP: " + erro);

                IEnumerable<string> arquivos = Utilidades.IO.FileStorageService.Storage.GetFiles(caminhoDocumentosOUTPUT, "*.txt", SearchOption.TopDirectoryOnly).AsParallel();
                
                foreach (string arquivo in arquivos)
                {
                    string fileName = Path.GetFileName(arquivo);
                    string textoArquivo = "";
                    string numeroCarga = "";
                    string codigoRetorno = "";

                    using MemoryStream memoryStream = new MemoryStream();

                    using (Stream fileStream = Utilidades.IO.FileStorageService.Storage.OpenRead(arquivo))
                    {
                        fileStream.CopyTo(memoryStream);
                        textoArquivo = Encoding.UTF8.GetString(memoryStream.ToArray());
                    }
                    if (!fileName.StartsWith("RN"))
                        Servicos.FTP.EnviarArquivo(memoryStream, fileName, integracaoMoniloc.HostFTP, integracaoMoniloc.PortaFTP, integracaoMoniloc.DiretorioRetornoCVA, integracaoMoniloc.UsuarioFTP, integracaoMoniloc.SenhaFTP, integracaoMoniloc.FTPPassivo, integracaoMoniloc.SSL, out string mensagemRetorno, integracaoMoniloc.SFTP);
                    else
                    {
                        string[] lines = Utilidades.IO.FileStorageService.Storage.ReadLines(arquivo).ToArray();
                        foreach (string line in lines)
                        {
                            if (line.Substring(0, 2) == "01")
                            {
                                numeroCarga = line.Substring(7, 10).Trim();
                                codigoRetorno = line.Substring(2, 3).Trim();
                                break;
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(numeroCarga) && !string.IsNullOrWhiteSpace(codigoRetorno))
                        {
                            Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarCargaPorCodigoEmbarcador(numeroCarga);
                            if (carga != null)
                            {
                                if (carga.Codigo == cargaIntegracaoPendentes.Carga.Codigo)
                                {
                                    localizouArquivo = true;
                                    if (codigoRetorno == "000" || codigoRetorno == "005" || codigoRetorno == "0" || codigoRetorno == "5")
                                    {
                                        erro = "";
                                        Servicos.FTP.DeletarArquivo(integracaoMoniloc.HostFTP, integracaoMoniloc.PortaFTP, integracaoMoniloc.DiretorioRetornoCVA, integracaoMoniloc.UsuarioFTP, integracaoMoniloc.SenhaFTP, integracaoMoniloc.FTPPassivo, integracaoMoniloc.SSL, fileName, out erro, integracaoMoniloc.SFTP);
                                        if (erro == "")
                                        {
                                            cargaIntegracaoPendentes.ProblemaIntegracao = string.Empty;
                                            cargaIntegracaoPendentes.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                                        }
                                        else
                                        {
                                            cargaIntegracaoPendentes.ProblemaIntegracao = $"Erro ao excluir arquivo {fileName} no diretorio moniloc.";
                                            cargaIntegracaoPendentes.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                                        }
                                    }
                                    else
                                    {
                                        cargaIntegracaoPendentes.ProblemaIntegracao = "Retorno da moniloc: " + codigoRetorno;
                                        cargaIntegracaoPendentes.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                                    }
                                    servicoArquivoTransacao.Adicionar(cargaIntegracaoPendentes, textoArquivo, "", "txt");
                                    break;
                                }
                            }
                        }
                    }
                    MoverParaPastaProcessados(Guid.NewGuid().ToString() + fileName, caminhoDocumentosOUTPUT, arquivo);
                    Utilidades.IO.FileStorageService.Storage.Delete(arquivo);
                }
            }
            catch (ServicoException exception)
            {
                Servicos.Log.TratarErro($"Falha ao tentar integrar carga: {exception.Message}", "Moniloc");

                cargaIntegracaoPendentes.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracaoPendentes.ProblemaIntegracao = exception.Message;
            }
            catch (Exception exception)
            {
                Servicos.Log.TratarErro($"Falha ao tentar integrar carga: {exception.Message}", "Moniloc");

                cargaIntegracaoPendentes.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracaoPendentes.ProblemaIntegracao = $"Falha ao tentar consultar retorno da Moniloc: {exception.Message}";
            }

            if (!localizouArquivo)
            {
                cargaIntegracaoPendentes.ProblemaIntegracao = "Não foi possível localizar o arquivo de retorno da Moniloc, favor verifique e tente novamente.";
                cargaIntegracaoPendentes.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }
            repositorioCargaDadosTransporteIntegracao.Atualizar(cargaIntegracaoPendentes);
        }

        #endregion

        #region Metodos Privados

        private ValueTuple<MemoryStream, string> ObterArquivo(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento)
        {
            MemoryStream memoryStream = new MemoryStream();
            StreamWriter arquivoEnvio = new StreamWriter(memoryStream);


            CultureInfo cultura = new CultureInfo("pt-BR");
            cultura.NumberFormat.NumberDecimalSeparator = ".";
            cultura.NumberFormat.NumberGroupSeparator = ",";
            Repositorio.Embarcador.Pedidos.PedidoProduto repositorioPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(_unitOfWork);
            //Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repositorioCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(_unitOfWork);

            Dominio.Entidades.Usuario motorista = carregamento?.Motoristas?.FirstOrDefault();
            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = carregamento?.Pedidos?.FirstOrDefault()?.Pedido;
            Dominio.Entidades.Cliente destinatario = pedido.Remetente;

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> pedidoProdutos = repositorioPedidoProduto.BuscarPorPedido(pedido?.Codigo ?? 0);

            string codigoPlanta = !string.IsNullOrEmpty(pedido?.Tomador?.CodigoAlternativo) ? pedido?.Tomador?.CodigoAlternativo : !string.IsNullOrEmpty(pedido?.Tomador?.CodigoAlternativo) ? pedido?.Tomador?.CodigoAlternativo : string.Empty;
            string cargaEmbarcador = carregamento?.CargasFrete?.FirstOrDefault()?.CodigoCargaEmbarcador ?? string.Empty;
            string numeroCVA = ObterTextoVariavel(cargaEmbarcador, 10, caracterPreichimento: " ", permiteNulo: false);
            string codigoTransportador = ObterTextoVariavel("N780", 4, caracterPreichimento: " ", permiteNulo: false);
            string placaCarreta = ObterTextoVariavel(carregamento?.Veiculo?.Placa ?? string.Empty, 7, caracterPreichimento: " ", permiteNulo: false);
            string cpfMotorista = ObterTextoVariavel(motorista?.CPF ?? string.Empty, 14, caracterPreichimento: " ", permiteNulo: false);
            string chegadaPrevista = ObterTextoVariavel(pedido?.PrevisaoEntrega?.ToString("dd/MM/yyyyhh:mm:ss") ?? string.Empty, 18, caracterPreichimento: " ", permiteNulo: false);
            string tipoVeiculo = ObterTextoVariavel(carregamento?.ModeloVeicularCarga?.CodigoIntegracao ?? string.Empty, 4, caracterPreichimento: " ", permiteNulo: false);
            string tipoViagem = ObterTextoVariavel(carregamento?.TipoOperacao?.CodigoIntegracao ?? string.Empty, 3, caracterPreichimento: " ", permiteNulo: false);
            string codigoTipoSolicitacionCVA = ObterTextoVariavel(carregamento?.TipoDeCarga?.CodigoTipoCargaEmbarcador ?? string.Empty, 5, caracterPreichimento: " ", permiteNulo: false);
            string rgMotorista = ObterTextoVariavel(motorista?.RG ?? string.Empty, 15, caracterPreichimento: " ", permiteNulo: false);
            string nomeMotorista = ObterTextoVariavel(motorista?.Nome ?? string.Empty, 50, caracterPreichimento: " ", permiteNulo: false);
            string numeroRota = ObterTextoVariavel(pedido?.RotaFrete?.CodigoIntegracao ?? string.Empty, 15, caracterPreichimento: " ", permiteNulo: false);
            string justiTipoVeiculo = ObterTextoVariavel(carregamento.ModeloVeicularCarga.Codigo.ToString(), 150, caracterPreichimento: " ", permiteNulo: false); // verificar
            string kmTotal = ObterTextoVariavel(pedido?.RotaFrete?.Quilometros.ToString("0.00", cultura) ?? string.Empty, 7, caracterPreichimento: " ", permiteNulo: false);
            string codigoFornecedor = ObterTextoVariavel(destinatario?.CodigoAlternativo ?? string.Empty, 4, caracterPreichimento: " ", permiteNulo: false);
            string previsaoChegada = ObterTextoVariavel(pedido?.PrevisaoEntrega?.ToString("dd/MM/yyyyhh:mm:ss") ?? string.Empty, 18, caracterPreichimento: " ", permiteNulo: false);
            string previsaoSaida = ObterTextoVariavel(pedido?.DataPrevisaoSaida?.ToString("dd/MM/yyyyhh:mm:ss") ?? string.Empty, 18, caracterPreichimento: " ", permiteNulo: false);
            string numeroSolicitacao = ObterTextoVariavel(pedido?.NumeroPedidoEmbarcador ?? string.Empty, 10, caracterPreichimento: " ", permiteNulo: false);

            string codigoEmbalagem = "";
            string tipoOperacao = ObterTextoVariavel("01", 2, caracterPreichimento: " ", permiteNulo: false);
            string linhaFinal = ObterTextoVariavel("05PRE", 5, caracterPreichimento: " ", permiteNulo: false);
            string numeroSolicitacaoCVA = ObterTextoVariavel(" ", 10, caracterPreichimento: " ", permiteNulo: false);
            string tipoGeracaoCVA = ObterTextoVariavel(pedido.CodigoPedidoCliente, 1, caracterPreichimento: " ", permiteNulo: false);
            string codigoJustificativaReagen = ObterTextoVariavel(" ", 3, caracterPreichimento: " ", permiteNulo: false);
            string observacaoJustificativaReage = ObterTextoVariavel(" ", 50, caracterPreichimento: " ", permiteNulo: false);
            string fluxo = ObterTextoVariavel("1", 1, caracterPreichimento: " ", permiteNulo: false);
            string nomeNavio = ObterTextoVariavel(" ", 30, caracterPreichimento: " ", permiteNulo: false);
            string numeroReserva = ObterTextoVariavel(" ", 30, caracterPreichimento: " ", permiteNulo: false);
            string pais = ObterTextoVariavel(" ", 30, caracterPreichimento: " ", permiteNulo: false);
            string numeroContainer = ObterTextoVariavel(" ", 20, caracterPreichimento: " ", permiteNulo: false);
            string conta = ObterTextoVariavel(" ", 8, caracterPreichimento: " ", permiteNulo: false);
            string setor = ObterTextoVariavel(" ", 4, caracterPreichimento: " ", permiteNulo: false);
            string tipoContainer = ObterTextoVariavel(" ", 2, caracterPreichimento: " ", permiteNulo: false);
            string statusCheioOUVazio = ObterTextoVariavel(" ", 1, caracterPreichimento: " ", permiteNulo: false);
            string reservardo = ObterTextoVariavel(" ", 34, caracterPreichimento: " ", permiteNulo: false);
            string coletaEntrega = ObterTextoVariavel(" ", 1, caracterPreichimento: " ", permiteNulo: false);
            string reservadoLinha2 = ObterTextoVariavel(" ", 100, caracterPreichimento: " ", permiteNulo: false);
            string reservadoLinha4 = ObterTextoVariavel(" ", 64, caracterPreichimento: " ", permiteNulo: false);

            string cvaPreliminar = ObterTextoVariavel(" ", 10, caracterPreichimento: " ", permiteNulo: false);
            string codigoFornecedorPrelimidar = ObterTextoVariavel(" ", 4, caracterPreichimento: " ", permiteNulo: false);

            string quantidadeRegistrosLinha1 = ObterTextoVariavel("1", 10, caracterPreichimento: "0", permiteNulo: false);
            string quantidadeRegistrosLinha2 = ObterTextoVariavel("1", 10, caracterPreichimento: "0", permiteNulo: false);
            string quantidadeRegistrosLinha3 = ObterTextoVariavel(pedidoProdutos.Count.ToString(), 10, caracterPreichimento: "0", permiteNulo: false);
            string quantidadeRegistrosLinha4 = ObterTextoVariavel("1", 10, caracterPreichimento: "0", permiteNulo: false);
            string quantidadeRegistrosLinha5 = ObterTextoVariavel("1", 10, caracterPreichimento: "0", permiteNulo: false);


            string linha1 = $"01{codigoPlanta}{numeroCVA}{codigoTransportador}{placaCarreta}{cpfMotorista}{chegadaPrevista}{tipoVeiculo}{tipoViagem}{codigoTipoSolicitacionCVA}{numeroSolicitacaoCVA}{tipoGeracaoCVA}{codigoJustificativaReagen}{observacaoJustificativaReage}{rgMotorista}{nomeMotorista}{numeroRota}{fluxo}{justiTipoVeiculo}{nomeNavio}{numeroReserva}{pais}{numeroContainer}{conta}{setor}{tipoContainer}{statusCheioOUVazio}{kmTotal}{reservardo}";
            string linha2 = $"02{codigoFornecedor}{previsaoChegada}{previsaoSaida}{codigoJustificativaReagen}{observacaoJustificativaReage}{coletaEntrega}{reservadoLinha2}";

            decimal quantidadeA = 0;
            StringBuilder linha3 = new StringBuilder();
            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoProduto pedidoProduto in pedidoProdutos)
            {
                string codigoProduto = ObterTextoVariavel(pedidoProduto.Produto.CodigoProdutoEmbarcador, 23, caracterPreichimento: " ", permiteNulo: false);
                string tipoOperacaoProduto = "01";
                string numeroSolicitacaoPedido = ObterTextoVariavel(pedidoProduto.Pedido.NumeroPedidoEmbarcador, 20, caracterPreichimento: " ", permiteNulo: false);


                string quantidadeProduto = ObterTextoVariavel(pedidoProduto.QuantidadePlanejada.ToString("0.000", cultura), 10, caracterPreichimento: " ", permiteNulo: false);
                string quantidadeConfirmada = ObterTextoVariavel(pedidoProduto.Quantidade.ToString("0.000", cultura), 10, caracterPreichimento: " ", permiteNulo: false);
                string dataEntregaRelease = ObterTextoVariavel(carregamento?.DataCriacao.ToString("dd/MM/yyyy") ?? string.Empty, 10, caracterPreichimento: " ", permiteNulo: false);
                string reservadoProduto = ObterTextoVariavel("", 90, caracterPreichimento: " ", permiteNulo: false);
                codigoEmbalagem = pedidoProduto.Produto.TipoEmbalagem.CodigoIntegracao;
                quantidadeA += pedidoProduto.Quantidade;
                quantidadeProduto = quantidadeProduto.Replace(",", ".");
                quantidadeConfirmada = quantidadeConfirmada.Replace(",", ".");
                string linha = $"03{codigoProduto}{tipoOperacaoProduto}{numeroSolicitacaoPedido}{quantidadeProduto}{quantidadeConfirmada}{dataEntregaRelease}{reservadoProduto}";
                linha3.Append(linha);
            }

            string valorConvertido = quantidadeA.ToString("0.000", cultura);
            string quantidadeEmbalagem = ObterTextoVariavel(valorConvertido, 10, caracterPreichimento: " ", permiteNulo: false);
            codigoEmbalagem = ObterTextoVariavel(codigoEmbalagem, 7, caracterPreichimento: " ", permiteNulo: false);

            string linha4 = $"04{codigoEmbalagem}{quantidadeEmbalagem}{tipoOperacao}{codigoFornecedor}{previsaoChegada}{previsaoSaida}{reservadoLinha4}";
            string linha5 = $"05PRE";
            string linha6 = $"06{quantidadeRegistrosLinha1}{quantidadeRegistrosLinha2}{quantidadeRegistrosLinha3}{quantidadeRegistrosLinha4}{quantidadeRegistrosLinha5}";

            arquivoEnvio.WriteLine(linha1);
            arquivoEnvio.WriteLine(linha2);
            arquivoEnvio.WriteLine(linha3.ToString());
            arquivoEnvio.WriteLine(linha4);
            arquivoEnvio.WriteLine(linha5);
            arquivoEnvio.WriteLine(linha6);
            arquivoEnvio.Flush();

            byte[] byteArquivo = memoryStream.ToArray();
            MemoryStream newMemoryStreamRetorno = new MemoryStream(byteArquivo);

            string nomeArquivo = $"T{carregamento.Empresa?.CodigoIntegracao}{DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss").Replace("-", "")}{carregamento.AutoSequenciaNumero}";

            return (newMemoryStreamRetorno, nomeArquivo);
        }

        private string ObterTextoVariavel(string valor, int tamanhoTexto, string caracterPreichimento, bool permiteNulo, bool alinhasEsqueda = false)
        {
            if (string.IsNullOrWhiteSpace(valor))
                valor = "";

            bool valorMismoTamanho = valor.Length == tamanhoTexto;
            string pattern = @"^[a-zA-Z0-9]+$";

            if (valorMismoTamanho)
                return valor;

            bool valorMenorTamanho = tamanhoTexto > valor.Length;
            bool isAlphanumerico = Regex.IsMatch(valor, pattern);

            if (!valorMenorTamanho)
                return Utilidades.String.Left(valor, tamanhoTexto);

            int quantidadeLetras = valor.Length;
            int iniciarApartir = tamanhoTexto - quantidadeLetras;
            StringBuilder valorSaida = new StringBuilder();

            for (int i = 0; i < tamanhoTexto; i++)
            {
                if (isAlphanumerico || alinhasEsqueda)
                {
                    if (i < quantidadeLetras)
                        valorSaida.Append(valor[i]);
                    else
                        valorSaida.Append(caracterPreichimento);
                }
                else
                {
                    if (i < iniciarApartir)
                        valorSaida.Append(caracterPreichimento);
                    else
                        valorSaida.Append(valor[i - iniciarApartir]);
                }
            }

            return valorSaida.ToString();
        }


        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMoniloc ObterConfiguracaoIntegracaoMoniloc()
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoMoniloc repositorioIntegracaoMoniloc = new Repositorio.Embarcador.Configuracoes.IntegracaoMoniloc(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMoniloc integracaMoniloc = repositorioIntegracaoMoniloc.BuscarPrimeiroRegistro();

            if (integracaMoniloc == null || !integracaMoniloc.PossuiIntegracaoMoniloc)
                throw new ServicoException("Configuração de integração com Moniloc não habilitada");

            if (string.IsNullOrEmpty(integracaMoniloc.UsuarioFTP) || string.IsNullOrEmpty(integracaMoniloc.SenhaFTP))
                throw new ServicoException("Credenciais de aunteicação não configurada corretamente");

            return integracaMoniloc;
        }

        private string ObterValorStringVariavel(string linha, ref int indiceLinha, int tamanhoRegistro)
        {
            if (indiceLinha + tamanhoRegistro > linha.Length)
                return "";

            string retornar = linha.Substring(indiceLinha, tamanhoRegistro);
            indiceLinha += tamanhoRegistro;

            return retornar.Trim();
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Moniloc.RecebimentoPedido> LerArquivoConsumo(System.IO.Stream arquivo)
        {
            System.IO.StreamReader stReaderArquivo = new StreamReader(arquivo);
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Moniloc.RecebimentoPedido> recebimentosPedido = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Moniloc.RecebimentoPedido>();
            Dominio.ObjetosDeValor.Embarcador.Integracao.Moniloc.RecebimentoPedido recebimentoPedido = null;

            while (!stReaderArquivo.EndOfStream)
            {
                var line = stReaderArquivo.ReadLine();
                int indiceLinha = 2;

                if (line.Substring(0, 2) == "01")
                {
                    recebimentoPedido = new Dominio.ObjetosDeValor.Embarcador.Integracao.Moniloc.RecebimentoPedido();
                    recebimentoPedido.Produtos = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Moniloc.DadosRecebimentoPedidoProduto>();
                    recebimentoPedido.Depositos = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Moniloc.DadosRecebimentoPedidoDeposito>();
                    recebimentoPedido.Coletas = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Moniloc.DadosRecebimentoPedidoColeta>();

                    recebimentoPedido.CodigoPlanta = ObterValorStringVariavel(line, ref indiceLinha, 2);
                    recebimentoPedido.CodigoFornecedor = ObterValorStringVariavel(line, ref indiceLinha, 4);
                    recebimentoPedido.LocalEntrega = ObterValorStringVariavel(line, ref indiceLinha, 7);
                    recebimentoPedido.Reservado = ObterValorStringVariavel(line, ref indiceLinha, 100);

                    recebimentosPedido.Add(recebimentoPedido);
                }

                if (line.Substring(0, 2) == "02")
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Moniloc.DadosRecebimentoPedidoProduto produto = new Dominio.ObjetosDeValor.Embarcador.Integracao.Moniloc.DadosRecebimentoPedidoProduto();

                    produto.CodigoProduto = ObterValorStringVariavel(line, ref indiceLinha, 23);
                    produto.QuantidadeSolicitada = ObterValorStringVariavel(line, ref indiceLinha, 10).ToDecimal();
                    produto.QuantidadeColetada = ObterValorStringVariavel(line, ref indiceLinha, 10).ToDecimal();
                    produto.DataEntrega = ObterValorStringVariavel(line, ref indiceLinha, 10).ToDateTime();
                    produto.DataColeta = ObterValorStringVariavel(line, ref indiceLinha, 10).ToDateTime();
                    produto.NumeroRelease = ObterValorStringVariavel(line, ref indiceLinha, 10);
                    produto.Embalagem = ObterValorStringVariavel(line, ref indiceLinha, 8);
                    produto.AlturaEmbalagem = ObterValorStringVariavel(line, ref indiceLinha, 10).ToDecimal();
                    produto.LarguraEmbalagem = ObterValorStringVariavel(line, ref indiceLinha, 10).ToDecimal();
                    produto.ComprimentoEmbalagem = ObterValorStringVariavel(line, ref indiceLinha, 10).ToDecimal();
                    produto.PesoEmbalagem = ObterValorStringVariavel(line, ref indiceLinha, 10).ToDecimal();
                    produto.QuantidadeProdutoEmbalagem = ObterValorStringVariavel(line, ref indiceLinha, 10).ToDecimal();
                    produto.PesoProduto = ObterValorStringVariavel(line, ref indiceLinha, 10).ToDecimal();
                    produto.EmpilhamentoMaximo = ObterValorStringVariavel(line, ref indiceLinha, 5).ToInt();

                    recebimentoPedido.Produtos.Add(produto);
                }

                if (line.Substring(0, 2) == "03")
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Moniloc.DadosRecebimentoPedidoDeposito deposito = new Dominio.ObjetosDeValor.Embarcador.Integracao.Moniloc.DadosRecebimentoPedidoDeposito();

                    deposito.CodigoDeposito = ObterValorStringVariavel(line, ref indiceLinha, 2);
                    deposito.NomeDeposito = ObterValorStringVariavel(line, ref indiceLinha, 50);

                    recebimentoPedido.Depositos.Add(deposito);
                }

                if (line.Substring(0, 2) == "04")
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Moniloc.DadosRecebimentoPedidoColeta coleta = new Dominio.ObjetosDeValor.Embarcador.Integracao.Moniloc.DadosRecebimentoPedidoColeta();

                    coleta.HoraColeta = ObterValorStringVariavel(line, ref indiceLinha, 5);
                    coleta.QuantidadeConfirmada = ObterValorStringVariavel(line, ref indiceLinha, 10).ToDecimal();
                    coleta.Destino = ObterValorStringVariavel(line, ref indiceLinha, 4);
                    coleta.TipoVeiculo = ObterValorStringVariavel(line, ref indiceLinha, 4);

                    recebimentoPedido.Coletas.Add(coleta);
                }
            }

            return recebimentosPedido;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Moniloc.RecebimentoPedidoExtra> LerArquivoConsumoExtra(System.IO.Stream arquivo)
        {
            System.IO.StreamReader stReaderArquivo = new StreamReader(arquivo);
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Moniloc.RecebimentoPedidoExtra> recebimentosPedidosExtras = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Moniloc.RecebimentoPedidoExtra>();
            Dominio.ObjetosDeValor.Embarcador.Integracao.Moniloc.RecebimentoPedidoExtra recebimentoPedidoExtra = null;

            while (!stReaderArquivo.EndOfStream)
            {
                var line = stReaderArquivo.ReadLine();
                int indiceLinha = 2;

                if (line.Substring(0, 2) == "01")
                {
                    recebimentoPedidoExtra = new Dominio.ObjetosDeValor.Embarcador.Integracao.Moniloc.RecebimentoPedidoExtra();
                    recebimentoPedidoExtra.Produtos = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Moniloc.DadosRecebimentoPedidoExtraProduto>();
                    recebimentoPedidoExtra.Embalagens = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Moniloc.DadosRecebimentoPedidoExtraEmbalagem>();

                    recebimentoPedidoExtra.NumeroSolicitacao = ObterValorStringVariavel(line, ref indiceLinha, 10);
                    recebimentoPedidoExtra.TipoGeracaoSolicitacao = ObterValorStringVariavel(line, ref indiceLinha, 4);
                    recebimentoPedidoExtra.TipoSolicitacao = ObterValorStringVariavel(line, ref indiceLinha, 7);
                    recebimentoPedidoExtra.Planta = ObterValorStringVariavel(line, ref indiceLinha, 2);
                    recebimentoPedidoExtra.Origem = ObterValorStringVariavel(line, ref indiceLinha, 4);
                    recebimentoPedidoExtra.Destino = ObterValorStringVariavel(line, ref indiceLinha, 4);
                    recebimentoPedidoExtra.DataCadastro = ObterValorStringVariavel(line, ref indiceLinha, 18).ToDateTime("dd/MM/yyyyHH:mm:ss");
                    recebimentoPedidoExtra.DataColeta = ObterValorStringVariavel(line, ref indiceLinha, 18).ToDateTime("dd/MM/yyyyHH:mm:ss");
                    recebimentoPedidoExtra.ResponsavelSolicitacao = ObterValorStringVariavel(line, ref indiceLinha, 1);
                    recebimentoPedidoExtra.TipoVeiculo = ObterValorStringVariavel(line, ref indiceLinha, 4);
                    recebimentoPedidoExtra.Transportadora = ObterValorStringVariavel(line, ref indiceLinha, 4);
                    recebimentoPedidoExtra.Conta = ObterValorStringVariavel(line, ref indiceLinha, 8);
                    recebimentoPedidoExtra.CentroDeCusto = ObterValorStringVariavel(line, ref indiceLinha, 4);
                    recebimentoPedidoExtra.FornecedorResponsavel = ObterValorStringVariavel(line, ref indiceLinha, 4);
                    recebimentoPedidoExtra.Motivo = ObterValorStringVariavel(line, ref indiceLinha, 3);
                    recebimentoPedidoExtra.Consolidar = ObterValorStringVariavel(line, ref indiceLinha, 1);
                    recebimentoPedidoExtra.Reservado = ObterValorStringVariavel(line, ref indiceLinha, 100);

                    recebimentosPedidosExtras.Add(recebimentoPedidoExtra);
                }

                if (line.Substring(0, 2) == "02")
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Moniloc.DadosRecebimentoPedidoExtraProduto produto = new Dominio.ObjetosDeValor.Embarcador.Integracao.Moniloc.DadosRecebimentoPedidoExtraProduto();

                    produto.CodigoProduto = ObterValorStringVariavel(line, ref indiceLinha, 23);
                    produto.QuantidadeSolicitada = ObterValorStringVariavel(line, ref indiceLinha, 10).ToDecimal();
                    produto.Embalagem = ObterValorStringVariavel(line, ref indiceLinha, 8);
                    produto.AlturaEmbalagem = ObterValorStringVariavel(line, ref indiceLinha, 10).ToDecimal();
                    produto.LarguraEmbalagem = ObterValorStringVariavel(line, ref indiceLinha, 10).ToDecimal();
                    produto.ComprimentoEmbalagem = ObterValorStringVariavel(line, ref indiceLinha, 10).ToDecimal();
                    produto.PesoEmbalagem = ObterValorStringVariavel(line, ref indiceLinha, 10).ToDecimal();
                    produto.QtdProdutoEmbalagem = ObterValorStringVariavel(line, ref indiceLinha, 10).ToDecimal();
                    produto.PesoProduto = ObterValorStringVariavel(line, ref indiceLinha, 10).ToDecimal();
                    produto.EmpilhamentoMaximo = ObterValorStringVariavel(line, ref indiceLinha, 5).ToInt();
                    produto.Palet = ObterValorStringVariavel(line, ref indiceLinha, 8);
                    produto.AlturaPalet = ObterValorStringVariavel(line, ref indiceLinha, 10).ToDecimal();
                    produto.LarguraPalet = ObterValorStringVariavel(line, ref indiceLinha, 10).ToDecimal();
                    produto.ComprimentoPalet = ObterValorStringVariavel(line, ref indiceLinha, 10).ToDecimal();
                    produto.PesoPalet = ObterValorStringVariavel(line, ref indiceLinha, 10).ToDecimal();
                    produto.QtdEmbalagemPorCamada = ObterValorStringVariavel(line, ref indiceLinha, 10).ToDecimal();
                    produto.CodigoOrigemDestino = ObterValorStringVariavel(line, ref indiceLinha, 4);
                    produto.HoraColetaEntrega = ObterValorStringVariavel(line, ref indiceLinha, 5);
                    produto.Reservado = ObterValorStringVariavel(line, ref indiceLinha, 100);

                    recebimentoPedidoExtra.Produtos.Add(produto);
                }

                if (line.Substring(0, 2) == "03")
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Moniloc.DadosRecebimentoPedidoExtraEmbalagem embalagem = new Dominio.ObjetosDeValor.Embarcador.Integracao.Moniloc.DadosRecebimentoPedidoExtraEmbalagem();

                    embalagem.CodigoEmbalagem = ObterValorStringVariavel(line, ref indiceLinha, 8);
                    embalagem.QuantidadeSolicitada = ObterValorStringVariavel(line, ref indiceLinha, 10).ToDecimal();
                    embalagem.AlturaEmbalagem = ObterValorStringVariavel(line, ref indiceLinha, 10).ToDecimal();
                    embalagem.LarguraEmbalagem = ObterValorStringVariavel(line, ref indiceLinha, 10).ToDecimal();
                    embalagem.ComprimentoEmbalagem = ObterValorStringVariavel(line, ref indiceLinha, 10).ToDecimal();
                    embalagem.PesoEmbalagem = ObterValorStringVariavel(line, ref indiceLinha, 10).ToDecimal();
                    embalagem.Reservado = ObterValorStringVariavel(line, ref indiceLinha, 100);

                    recebimentoPedidoExtra.Embalagens.Add(embalagem);
                }
            }

            return recebimentosPedidosExtras;
        }

        private string CaminhoArquivoRetornoMoniloc()
        {
            string caminho = Utilidades.IO.FileStorageService.Storage.Combine(Configuracoes.ConfigurationInstance.GetInstance(_unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivos, "RetornoMoniloc");

            return caminho;
        }
        private static void MoverParaPastaProcessados(string nomeArquivo, string caminhoArmazenamento, string fullName)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(Utilidades.IO.FileStorageService.Storage.ReadAllText(fullName));

            string caminhoCompleto = Utilidades.IO.FileStorageService.Storage.Combine(caminhoArmazenamento, "Processados", nomeArquivo);

            Utilidades.IO.FileStorageService.Storage.WriteAllBytes(caminhoCompleto, bytes);
        }

        #endregion
    }
}
