using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Integracao;
using System;
using System.Linq;
using System.Text;

namespace Servicos.Embarcador.Integracao.Diageo
{
    public class IntegracaoDiageo
    {
        #region Atributo

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly AdminMultisoftware.Repositorio.UnitOfWork _adminUnitOfWork;

        #endregion Atributo

        #region Construtores

        public IntegracaoDiageo(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdminMultisoftrware)
        {
            _unitOfWork = unitOfWork;
            _adminUnitOfWork = unitOfWorkAdminMultisoftrware;
        }

        #endregion Construtores

        #region Métodos Públicos

        public Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta IntegrarOcorrenciaPedidoFTP(Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega integracao)
        {
            HttpRequisicaoResposta httpRequisicaoResposta = new HttpRequisicaoResposta()
            {
                conteudoRequisicao = string.Empty,
                extensaoRequisicao = "xml",
                conteudoResposta = string.Empty,
                extensaoResposta = "json",
                sucesso = false,
                mensagem = string.Empty
            };

            try
            {
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDiageo integracaoDiageo = ObterConfiguracaoIntegracao();

                string xmlDados = BuscarDadosPedidoOcorrencia(integracao);

                System.IO.MemoryStream arquivo = new System.IO.MemoryStream(Encoding.ASCII.GetBytes(xmlDados));

                string mensagemErro = "";
                if (Servicos.FTP.EnviarArquivo(arquivo, "Status_" + integracao.Carga.CodigoCargaEmbarcador.ToString() + ".xml", integracaoDiageo.Endereco, integracaoDiageo.Porta, integracaoDiageo.DiretorioInbound, integracaoDiageo.Usuario, integracaoDiageo.Senha, integracaoDiageo.Passivo, integracaoDiageo.SSL, out mensagemErro, integracaoDiageo.UtilizarSFTP))
                {
                    httpRequisicaoResposta.sucesso = true;
                    httpRequisicaoResposta.conteudoRequisicao = xmlDados;
                    httpRequisicaoResposta.mensagem = "Integrado com sucesso.";
                }
                else
                {
                    throw new ServicoException($"Problema ao integrar com FTP Diageo. Erro: {mensagemErro}");
                }
            }
            catch (ServicoException excecao)
            {
                httpRequisicaoResposta.mensagem = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                httpRequisicaoResposta.mensagem = "Problema ao tentar integrar com FTP Diageo.";
            }

            return httpRequisicaoResposta;
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDiageo ObterConfiguracaoIntegracao()
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoDiageo repIntegracaoDiageo = new Repositorio.Embarcador.Configuracoes.IntegracaoDiageo(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDiageo integracaoDiageo = repIntegracaoDiageo.Buscar();

            if (integracaoDiageo == null)
                throw new ServicoException("Falha ao buscar configuração de integração com a Diageo");
            if (!integracaoDiageo.PossuiIntegracao)
                throw new ServicoException("A integração com a Diageo não está ativa");

            return integracaoDiageo;
        }

        private string BuscarDadosPedidoOcorrencia(Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega integracao)
        {
            Dominio.Entidades.Veiculo veiculoTracao = integracao.Carga?.Veiculo ?? new Dominio.Entidades.Veiculo();
            Dominio.Entidades.Veiculo reboque = integracao.Carga?.VeiculosVinculados?.FirstOrDefault() ?? new Dominio.Entidades.Veiculo();
            Dominio.Entidades.Embarcador.Logistica.PosicaoAtual posicao = veiculoTracao?.PosicaoAtual?.LastOrDefault() ?? new Dominio.Entidades.Embarcador.Logistica.PosicaoAtual();

            AdminMultisoftware.Repositorio.Localidades.LocalidadeGeo repLocalidadeGeo = new AdminMultisoftware.Repositorio.Localidades.LocalidadeGeo(_adminUnitOfWork);
            AdminMultisoftware.Dominio.ObjetosDeValor.Localidades.LocalidadeGeo localidadeGeo = repLocalidadeGeo.BuscarLocalidadesPorPosicao((decimal)posicao.Latitude, (decimal)posicao.Longitude, false).FirstOrDefault();

            int stopNumber = 0;

            if (integracao.TipoDeOcorrencia.TipoAplicacaoColetaEntrega == TipoAplicacaoColetaEntrega.Coleta)
                stopNumber = 1;
            else if (integracao.TipoDeOcorrencia.TipoAplicacaoColetaEntrega == TipoAplicacaoColetaEntrega.Entrega)
                stopNumber = 2;

            DateTime eventDateTime = integracao.DataOcorrencia;

            if (!string.IsNullOrEmpty(integracao.TipoDeOcorrencia.CodigoIntegracaoAuxiliar) && integracao.TipoDeOcorrencia.CodigoIntegracaoAuxiliar.ToUpper().Contains("X6"))
            {
                var monitoramento = integracao.Carga?.Monitoramento?.Where(x => x.UltimaPosicao.Data != null).OrderByDescending(x => x.UltimaPosicao.Data).FirstOrDefault();

                if (monitoramento?.UltimaPosicao.Data != null)
                    eventDateTime = (DateTime)monitoramento?.UltimaPosicao.Data;
            }

            Dominio.ObjetosDeValor.Embarcador.Integracao.FTPDiageo.ShipmentStatus xml = new Dominio.ObjetosDeValor.Embarcador.Integracao.FTPDiageo.ShipmentStatus()
            {
                Header = new Dominio.ObjetosDeValor.Embarcador.Integracao.FTPDiageo.Header()
                {
                    TransactionID = "T1234",
                    LoadNumber = integracao.Pedido.NumeroPedidoEmbarcador,
                    DateTime = integracao.DataOcorrencia,
                    CarrierCode = "T6089613",
                    CarrierReference = "PRO12345",
                    SCAC = "ABCD",
                    Purpose = "Status",
                    MoveType = "OIDL",
                    Tractor = veiculoTracao?.Placa ?? "",
                    Trailer = reboque?.Placa ?? ""
                },
                StatusDetails = new Dominio.ObjetosDeValor.Embarcador.Integracao.FTPDiageo.StatusDetails()
                {
                    ReferenceNumbers = new Dominio.ObjetosDeValor.Embarcador.Integracao.FTPDiageo.ReferenceNumbers()
                    {
                        ReferenceNumber = new Dominio.ObjetosDeValor.Embarcador.Integracao.FTPDiageo.ReferenceNumber()
                        {
                            type = "BOL",
                            value = integracao.Carga.Numero
                        },
                    },
                    Stop = new Dominio.ObjetosDeValor.Embarcador.Integracao.FTPDiageo.Stop()
                    {

                        StopNumber = stopNumber,
                        SequenceNumber = 0,
                        Event = new Dominio.ObjetosDeValor.Embarcador.Integracao.FTPDiageo.Event()
                        {
                            EventCode = integracao.TipoDeOcorrencia.CodigoIntegracaoAuxiliar,
                            EventDescription = integracao.TipoDeOcorrencia.Descricao,
                            EventDateTime = eventDateTime,
                            Exception = new Dominio.ObjetosDeValor.Embarcador.Integracao.FTPDiageo.Exception()
                            {
                                ExceptionReasonCode = "",
                                ExceptionDescription = ""
                            }
                        },
                        EventLocation = new Dominio.ObjetosDeValor.Embarcador.Integracao.FTPDiageo.EventLocation()
                        {
                            ID = "W13245",
                            Name = "Company",
                            Address = new Dominio.ObjetosDeValor.Embarcador.Integracao.FTPDiageo.Address()
                            {
                                AddressLineOne = "",
                                AddressLineTwo = "",
                                City = localidadeGeo?.nome,
                                StateOrProvince = localidadeGeo?.uf_sigla,
                                Country = "",
                                PostalCode = ""
                            },
                            Latitude = posicao.Latitude,
                            Longitude = posicao.Longitude
                        }
                    }
                }
            };

            return Utilidades.XML.Serializar(xml);
        }

        #endregion Métodos Privados
    }
}
