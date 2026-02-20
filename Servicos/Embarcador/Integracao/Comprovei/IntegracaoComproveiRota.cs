using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Newtonsoft.Json;
using Servicos.ServicoComprovei.CriarRota;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Integracao.Comprovei
{
    public class IntegracaoComproveiRota
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos Globais

        #region Construtores

        public IntegracaoComproveiRota(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Publicos

        public void IntegrarCarga(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoComproveiRota configuracaoComprovei = new Repositorio.Embarcador.Configuracoes.IntegracaoComproveiRota(_unitOfWork).Buscar();

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;

            cargaIntegracao.NumeroTentativas += 1;
            cargaIntegracao.DataIntegracao = DateTime.Now;

            try
            {
                if (configuracaoComprovei == null)
                    throw new ServicoException("Falha ao buscar configuração da integração com a Comprovei");

                ServicoComprovei.CriarRota.WebServiceComproveiPortTypeClient client = ObterClient(configuracaoComprovei);

                Servicos.ServicoComprovei.CriarRota.uploadRouteRequest request = ObterRequest(cargaIntegracao.Carga, configuracaoComprovei);

                jsonRequest = JsonConvert.SerializeObject(request, Formatting.Indented);

                ServicoComprovei.CriarRota.uploadRouteResponse response = client.uploadRouteAsync(request).Result;

                if (string.IsNullOrEmpty(response?.protocolo))
                    throw new ServicoException(response.status);

                cargaIntegracao.Protocolo = response.protocolo;
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                cargaIntegracao.PendenteRetorno = true;
                cargaIntegracao.ProblemaIntegracao = response.status;

                jsonResponse = JsonConvert.SerializeObject(response, Formatting.Indented);
            }
            catch (ServicoException ex)
            {
                cargaIntegracao.ProblemaIntegracao = ex.Message;
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoComproveiRota");

                cargaIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a Comprovei";
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }

            servicoArquivoTransacao.Adicionar(cargaIntegracao, jsonRequest, jsonResponse, "json");

            repositorioCargaIntegracao.Atualizar(cargaIntegracao);

        }

        #endregion Métodos Publicos

        #region Métodos Privados

        private ServicoComprovei.CriarRota.WebServiceComproveiPortTypeClient ObterClient(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoComproveiRota configuracaoComprovei)
        {
            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(configuracaoComprovei.URL);
            System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.ReceiveTimeout = new TimeSpan(0, 5, 0);

            if (configuracaoComprovei.URL.StartsWith("https"))
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;
            else
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.TransportCredentialOnly;

            binding.Security.Transport.ClientCredentialType = System.ServiceModel.HttpClientCredentialType.Basic;

            ServicoComprovei.CriarRota.WebServiceComproveiPortTypeClient client = new ServicoComprovei.CriarRota.WebServiceComproveiPortTypeClient(binding, endpointAddress);
            client.ClientCredentials.UserName.UserName = configuracaoComprovei.Usuario;
            client.ClientCredentials.UserName.Password = configuracaoComprovei.Senha;

            return client;
        }

        private Servicos.ServicoComprovei.CriarRota.uploadRouteRequest ObterRequest(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoComproveiRota configuracaoComprovei)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoBase = repositorioCargaPedido.BuscarPrimeiroPorCargaSemFetch(carga.Codigo);

            Dominio.Entidades.Cliente expedidor = cargaPedidoBase.Expedidor;
            Dominio.Entidades.Cliente recebedor = cargaPedidoBase.Recebedor;

            Servicos.ServicoComprovei.CriarRota.uploadRouteRequest request = new ServicoComprovei.CriarRota.uploadRouteRequest()
            {
                Credenciais = new ServicoComprovei.CriarRota.Credenciais()
                {
                    Usuario = configuracaoComprovei.Usuario,
                    Senha = configuracaoComprovei.Senha
                },
                Rotas = new ServicoComprovei.CriarRota.RotasType()
                {
                    Rota = new ServicoComprovei.CriarRota.RotasTypeRota()
                    {
                        numero = carga.Numero.ToString(),
                        Data = carga.DataCriacaoCarga.ToString("yyyyMMdd"),

                        Transportadora = new ServicoComprovei.CriarRota.RotasTypeRotaTransportadora
                        {
                            Codigo = carga.Empresa?.CNPJ ?? string.Empty,
                            Razao = carga.Empresa?.RazaoSocial ?? string.Empty,
                        },

                        Motorista = new ServicoComprovei.CriarRota.RotasTypeRotaMotorista
                        {
                            Nome = carga.Motoristas[0]?.Nome ?? string.Empty,
                            Usuario = carga.Motoristas[0]?.CPF ?? string.Empty,
                            PlacaVeiculo = carga.Veiculo?.Placa ?? string.Empty,
                            Telefone = carga.Motoristas[0]?.Telefone ?? string.Empty,
                            TipoVeiculo = carga.Veiculo?.TipoVeiculo ?? string.Empty,
                        },

                        Base = ObterCampoRotaBase(expedidor, recebedor),

                        Paradas = ObterParadas(carga),
                    },
                },
                nomeArquivo = $"rota_{carga.Numero ?? string.Empty}.xml"
            };

            RotasTypeRotaTipoRota? tipoRotaCarga = ObterTipoRotaCarga(carga);

            if (tipoRotaCarga != null)
            {
                request.Rotas.Rota.TipoRotaSpecified = true;
                request.Rotas.Rota.TipoRota = tipoRotaCarga.Value;
            }

            return request;
        }

        private ServicoComprovei.CriarRota.RotasTypeRotaParada[] ObterParadas(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            List<ServicoComprovei.CriarRota.RotasTypeRotaParada> paradas = new List<ServicoComprovei.CriarRota.RotasTypeRotaParada>();

            Repositorio.Embarcador.Pedidos.XMLNotaFiscalProduto repositorioXMLNotaFiscalProduto = new Repositorio.Embarcador.Pedidos.XMLNotaFiscalProduto(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXmlNotaFiscais = repositorioPedidoXMLNotaFiscal.BuscarPorCargaSemFetch(carga.Codigo);

            List<int> notasFiscaisCodigo = pedidoXmlNotaFiscais.Select(x => x.XMLNotaFiscal.Codigo).ToList();

            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto> produtosNotas = repositorioXMLNotaFiscalProduto.BuscarPorNotaFiscais(notasFiscaisCodigo);

            int sequencia = 1;

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoNotas = pedidoXmlNotaFiscais.Where(x => x.CargaPedido.Codigo == cargaPedido.Codigo).ToList();

                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal nota in pedidoNotas)
                {
                    List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto> produtosNota = produtosNotas.Where(x => x.XMLNotaFiscal.Codigo == nota.XMLNotaFiscal.Codigo).ToList();

                    Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = nota.XMLNotaFiscal;

                    ServicoComprovei.CriarRota.RotasTypeRotaParada rota = new ServicoComprovei.CriarRota.RotasTypeRotaParada
                    {
                        numero = (sequencia++).ToString(),

                        Documento = new ServicoComprovei.CriarRota.RotasTypeRotaParadaDocumento()
                        {
                            ChaveNota = xmlNotaFiscal.Chave ?? string.Empty,
                            Peso = xmlNotaFiscal.Peso.ToString() ?? string.Empty,
                            Volume = xmlNotaFiscal.Volumes.ToString() ?? string.Empty,
                            ValorNota = xmlNotaFiscal.Valor.ToString() ?? string.Empty,
                            Serie = xmlNotaFiscal.Serie ?? string.Empty,
                            PesoLiquido = xmlNotaFiscal.PesoLiquido.ToString() ?? string.Empty,
                            Pedido = xmlNotaFiscal.NumeroPedido.ToString() ?? string.Empty,
                            Tipo = xmlNotaFiscal.TipoDocumento.ObterDescricao() ?? string.Empty,
                            Cubagem = xmlNotaFiscal.PesoCubado.ToString() ?? string.Empty,
                            Numero = xmlNotaFiscal.Numero.ToString() ?? string.Empty,
                            Romaneio = xmlNotaFiscal.NumeroRomaneio ?? string.Empty,
                            CnpjTransportador = xmlNotaFiscal.CNPJTranposrtador?.ObterSomenteNumeros() ?? string.Empty,
                            CnpjEmissor = xmlNotaFiscal.Emitente?.CPF_CNPJ.ToString() ?? string.Empty,
                            Cnpj = xmlNotaFiscal.Destinatario?.CPF_CNPJ.ToString() ?? string.Empty,
                            Filial = xmlNotaFiscal.Filial?.Descricao.ToString() ?? string.Empty,
                            Emissao = xmlNotaFiscal.DataEmissao.ToString("yyyyMMdd") ?? string.Empty,
                            Placa = xmlNotaFiscal.PlacaVeiculoNotaFiscal ?? string.Empty,
                        },

                        Tipo = carga.CargaColeta ? ServicoComprovei.CriarRota.RotasTypeRotaParadaTipo.C : ServicoComprovei.CriarRota.RotasTypeRotaParadaTipo.E,

                        Cliente = new ServicoComprovei.CriarRota.RotasTypeRotaParadaCliente
                        {
                            Bairro = xmlNotaFiscal.Destinatario?.Bairro ?? string.Empty,
                            CEP = xmlNotaFiscal.Destinatario?.CEP ?? string.Empty,
                            Cidade = xmlNotaFiscal.Destinatario?.Cidade ?? string.Empty,
                            Codigo = xmlNotaFiscal.Destinatario?.Codigo.ToString() ?? string.Empty,
                            CodigoIBGE = xmlNotaFiscal.Destinatario?.Localidade?.CodigoIBGE.ToString() ?? string.Empty,
                            Endereco = xmlNotaFiscal.Destinatario?.Endereco ?? string.Empty,
                            Email = xmlNotaFiscal.Destinatario?.Email ?? string.Empty,
                            Estado = xmlNotaFiscal.Destinatario?.Localidade?.Estado?.Sigla ?? string.Empty,
                            Contato = xmlNotaFiscal.Destinatario?.Nome ?? string.Empty,
                            Telefone = xmlNotaFiscal.Destinatario?.Telefone1 ?? string.Empty,
                            Pais = xmlNotaFiscal.Destinatario?.Localidade?.Estado?.Pais?.Sigla ?? string.Empty,
                            Razao = xmlNotaFiscal.Destinatario?.NomeCNPJ ?? string.Empty,

                        },

                        SKUs = pedidoNotas.Select(x => new RotasTypeRotaParadaSKU
                        {
                            Barcode = x.XMLNotaFiscal?.EtiquetaCodigoBarrasWMS(x.XMLNotaFiscal.Volumes) ?? string.Empty,
                            Volumes = x.XMLNotaFiscal?.Volumes.ToString() ?? string.Empty,
                            PesoBruto = x.XMLNotaFiscal?.Peso.ToString() ?? string.Empty,
                            PesoLiquido = x.XMLNotaFiscal?.PesoLiquido.ToString() ?? string.Empty,
                            Descricao = x.XMLNotaFiscal?.Descricao ?? string.Empty,
                            Qde = produtosNota?.Sum(x => x.Quantidade).ToString() ?? string.Empty,
                            Uom = "UN",
                            codigo = x.XMLNotaFiscal?.Codigo.ToString() ?? string.Empty,
                        }).ToArray()
                    };

                    paradas.Add(rota);
                }
            }

            return paradas.ToArray();
        }

        private RotasTypeRotaBase ObterCampoRotaBase(Dominio.Entidades.Cliente expedidor, Dominio.Entidades.Cliente recebedor)
        {
            if (expedidor == null || recebedor == null)
                return null;

            return new RotasTypeRotaBase
            {
                Origem = new RotasTypeRotaBaseOrigem
                {
                    codigo = expedidor.CodigoIntegracao ?? string.Empty,
                    Nome = expedidor.Nome ?? string.Empty,
                    Rua = expedidor.Endereco ?? string.Empty,
                    Numero = expedidor.Numero ?? string.Empty,
                    Complemento = expedidor.Complemento ?? string.Empty,
                    Bairro = expedidor.Bairro ?? string.Empty,
                    Cidade = expedidor.Localidade?.Descricao ?? string.Empty,
                    Estado = expedidor.Localidade?.Estado.Sigla ?? string.Empty,
                    CEP = expedidor.CEP ?? string.Empty,
                    Pais = expedidor.Localidade?.Pais?.Abreviacao ?? string.Empty,
                },

                Destino = new RotasTypeRotaBaseDestino
                {
                    codigo = recebedor.CodigoIntegracao ?? string.Empty,
                    Nome = recebedor.Nome ?? string.Empty,
                    Rua = recebedor.Endereco ?? string.Empty,
                    Numero = recebedor.Numero ?? string.Empty,
                    Complemento = recebedor.Complemento ?? string.Empty,
                    Bairro = recebedor.Bairro ?? string.Empty,
                    Cidade = recebedor.Localidade?.Descricao ?? string.Empty,
                    Estado = recebedor.Localidade?.Estado.Sigla ?? string.Empty,
                    CEP = recebedor.CEP ?? string.Empty,
                    Pais = recebedor.Localidade?.Pais?.Abreviacao ?? string.Empty,
                }
            };
        }

        private RotasTypeRotaTipoRota? ObterTipoRotaCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (carga.Redespacho != null)
                return ObterTipoRotaCarga(carga.Redespacho?.TipoOperacao?.ConfiguracaoCarga?.TipoRotaCarga ?? TipoRotaCarga.Nenhuma);

            return ObterTipoRotaCarga(carga.TipoOperacao?.ConfiguracaoCarga?.TipoRotaCarga ?? TipoRotaCarga.Nenhuma);
        }

        private RotasTypeRotaTipoRota? ObterTipoRotaCarga(TipoRotaCarga tipoRotaCarga)
        {
            switch (tipoRotaCarga)
            {
                case TipoRotaCarga.Distribuicao:
                    return RotasTypeRotaTipoRota.D;
                case TipoRotaCarga.Transbordo:
                    return RotasTypeRotaTipoRota.T;
                case TipoRotaCarga.Praca:
                    return RotasTypeRotaTipoRota.P;
                case TipoRotaCarga.Retorno:
                    return RotasTypeRotaTipoRota.R;
                default:
                    return null;
            }

        }

        #endregion Métodos Privados
    }
}
