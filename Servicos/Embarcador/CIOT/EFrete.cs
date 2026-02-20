using Dominio.ObjetosDeValor.Embarcador.CIOT;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Repositorio;
using Servicos.ServicoEFrete.FaturamentoTransportadora;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Servicos.Embarcador.CIOT
{
    public class EFrete
    {
        #region Métodos Globais 

        public bool IntegrarMotoristaPendenteIntegracao(Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao integracao, out string mensagemErro, UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.CIOT.CIOTEFrete configuracao = ObterConfiguracaoEFrete(integracao.ConfiguracaoCIOT, unitOfWork);

            string token = Login(configuracao, unitOfWork, out mensagemErro);
            bool sucessoLogin = string.IsNullOrWhiteSpace(mensagemErro);
            bool sucesso = false;

            if (sucessoLogin && IntegrarMotorista(null, integracao.Motorista, integracao.ConfiguracaoCIOT, token, out mensagemErro, unitOfWork))
                sucesso = true;
            else
                sucesso = false;

            Logout(configuracao, token, unitOfWork);

            return sucesso;
        }

        public bool IntegrarVeiculoPendenteIntegracao(Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao integracao, out string mensagemErro, UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.CIOT.CIOTEFrete configuracao = ObterConfiguracaoEFrete(integracao.ConfiguracaoCIOT, unitOfWork);

            string token = Login(configuracao, unitOfWork, out mensagemErro);
            bool sucessoLogin = string.IsNullOrWhiteSpace(mensagemErro);
            bool sucesso = false;

            if (sucessoLogin && IntegrarVeiculoEProprietario(integracao.Veiculo, integracao.ConfiguracaoCIOT, token, unitOfWork, out mensagemErro))
                sucesso = true;
            else
                sucesso = false;

            Logout(configuracao, token, unitOfWork);

            return sucesso;
        }

        public Dominio.Entidades.Embarcador.CIOT.CIOTEFrete ObterConfiguracaoEFrete(Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT configuracaoCIOT, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.CIOT.CIOTEFrete repCIOTEFrete = new Repositorio.Embarcador.CIOT.CIOTEFrete(unidadeTrabalho);
            Dominio.Entidades.Embarcador.CIOT.CIOTEFrete configuracao = repCIOTEFrete.BuscarPorConfiguracaoCIOT(configuracaoCIOT.Codigo);
            return configuracao;
        }

        public SituacaoRetornoCIOT AbrirCIOT(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Repositorio.UnitOfWork unidadeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaMotorista repCargaMotoriwsta = new Repositorio.Embarcador.Cargas.CargaMotorista(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unidadeTrabalho);
            Dominio.Entidades.Embarcador.CIOT.CIOTEFrete configuracao = ObterConfiguracaoEFrete(ciot.ConfiguracaoCIOT, unidadeTrabalho);

            string mensagemErro;

            ciot.Contratante = configuracao.MatrizEFrete;
            ciot.Operadora = Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.eFrete;
            ciot.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Pendencia;
            string token = Login(configuracao, unidadeTrabalho, out mensagemErro);

            bool sucesso = false;

            List<Dominio.Entidades.Veiculo> veiculosCIOT = new List<Dominio.Entidades.Veiculo>() { ciot.Veiculo };

            if (ciot.VeiculosVinculados.Any())
                veiculosCIOT.AddRange(ciot.VeiculosVinculados);

            if (ciot.Motorista == null)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaCIOT> cargaCIOTs = repCargaCIOT.BuscarPorCIOTAgSerAdicionado(ciot.Codigo);
                if (cargaCIOTs != null && cargaCIOTs.Count > 0)
                {
                    var motoristasCarga = repCargaMotoriwsta.BuscarPorCarga(cargaCIOTs.FirstOrDefault().Carga.Codigo);
                    if (motoristasCarga != null && motoristasCarga.Count > 0)
                        ciot.Motorista = motoristasCarga.FirstOrDefault().Motorista;
                }
            }


            if (!string.IsNullOrWhiteSpace(token))
            {
                var proprietarioVeiculo = veiculosCIOT != null && veiculosCIOT.Count > 0 && veiculosCIOT.FirstOrDefault().Proprietario != null ? veiculosCIOT.FirstOrDefault().Proprietario : ciot.Transportador;

                if (IntegrarProprietario(ciot, proprietarioVeiculo, configuracao, token, unidadeTrabalho, out mensagemErro))
                {
                    if (IntegrarVeiculos(veiculosCIOT, ciot, configuracao, token, out mensagemErro, unidadeTrabalho))
                    {
                        if (IntegrarMotorista(ciot, ciot.Motorista, ciot.ConfiguracaoCIOT, token, out mensagemErro, unidadeTrabalho))
                        {
                            Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro = Servicos.Embarcador.CIOT.CIOT.ObterModalidadeTransportador(ciot.Transportador, unidadeTrabalho);

                            if (ciot.CIOTPorPeriodo)
                            {
                                if (AdicionarOperacaoTransporteSemViagem(ref ciot, ciot.Contratante, configuracao, token, veiculosCIOT, ciot.Motorista, unidadeTrabalho, out mensagemErro))
                                    sucesso = true;
                            }
                            else
                            {
                                if (IntegrarOperacaoTransporte(ref ciot, ciot.Contratante, configuracao, token, veiculosCIOT, ciot.Motorista, modalidadeTerceiro, unidadeTrabalho, out mensagemErro, tipoServicoMultisoftware))
                                    sucesso = true;
                            }
                        }
                    }
                }
            }

            Logout(configuracao, token, unidadeTrabalho);

            if (!sucesso)
            {
                ciot.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Pendencia;
                ciot.Mensagem = mensagemErro;
            }
            else
                ciot.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Aberto;

            if (ciot.Codigo > 0)
                repCIOT.Atualizar(ciot);
            else
                repCIOT.Inserir(ciot);

            return sucesso ? SituacaoRetornoCIOT.Autorizado : SituacaoRetornoCIOT.ProblemaIntegracao;
        }

        public SituacaoRetornoCIOT AdicionarViagem(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, Repositorio.UnitOfWork unidadeTrabalho, out string mensagemErro, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            Dominio.Entidades.Embarcador.Documentos.CIOT ciot = cargaCIOT.CIOT;

            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unidadeTrabalho);

            Dominio.Entidades.Embarcador.CIOT.CIOTEFrete configuracao = ObterConfiguracaoEFrete(ciot.ConfiguracaoCIOT, unidadeTrabalho);

            List<Dominio.Entidades.Embarcador.Cargas.CargaCIOT> cargaCIOTs = repCargaCIOT.BuscarPorCIOTAgSerAdicionado(ciot.Codigo);

            if (cargaCIOTs.Count <= 0)
            {
                mensagemErro = null;
                return SituacaoRetornoCIOT.ProblemaIntegracao;
            }

            string token = Login(configuracao, unidadeTrabalho, out mensagemErro);

            SituacaoRetornoCIOT retAdicionarViagem = SituacaoRetornoCIOT.ProblemaIntegracao;


            if (!string.IsNullOrWhiteSpace(token))
            {
                ServicoEFrete.PEF.AdicionarViagemRequest requisicao = new ServicoEFrete.PEF.AdicionarViagemRequest()
                {
                    CodigoIdentificacaoOperacao = ciot.Numero + "/" + ciot.CodigoVerificador,
                    Integrador = configuracao.CodigoIntegradorEFrete,
                    NaoAdicionarParcialmente = false,
                    Pagamentos = ObterPagamentosTAC(ciot, cargaCIOTs, configuracao, tipoServicoMultisoftware),
                    Token = token,
                    Versao = 3,
                    Viagens = ObterViagensTAC(ciot, cargaCIOTs, unidadeTrabalho, configuracao)
                };

                ServicoEFrete.PEF.PefServiceSoapClient svcPEF = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unidadeTrabalho).ObterClient<ServicoEFrete.PEF.PefServiceSoapClient, ServicoEFrete.PEF.PefServiceSoap>(TipoWebServiceIntegracao.EFrete_Pef);

                ServicoEFrete.PEF.AdicionarViagemResponse retorno = svcPEF.AdicionarViagem(requisicao);

                if (retorno.Sucesso)
                {
                    Repositorio.Embarcador.Documentos.CIOTCTe repCTeCIOT = new Repositorio.Embarcador.Documentos.CIOTCTe(unidadeTrabalho);

                    foreach (Dominio.Entidades.Embarcador.Documentos.CIOTCTe cteCIOT in ciot.CTes)
                    {
                        cteCIOT.Integrado = true;
                        repCTeCIOT.Atualizar(cteCIOT);
                    }
                    retAdicionarViagem = SituacaoRetornoCIOT.Autorizado;
                    mensagemErro = null;
                }
                else
                {
                    mensagemErro = retorno.Excecao?.Mensagem;
                }

                Logout(configuracao, token, unidadeTrabalho);
            }

            if (retAdicionarViagem == SituacaoRetornoCIOT.Autorizado)
            {
                cargaCIOT.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Aberto;
                cargaCIOT.Mensagem = "CIOT processado com sucesso.";
            }
            else
            {
                cargaCIOT.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Pendencia;
                cargaCIOT.Mensagem = mensagemErro;
            }

            repCargaCIOT.Atualizar(cargaCIOT);


            return retAdicionarViagem;
        }

        public bool IntegrarMovimentoFinanceiro(out string mensagemErro, Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, Dominio.Entidades.Embarcador.Fatura.Justificativa justificativa, decimal valorMovimento, Repositorio.UnitOfWork unitOfWork)
        {
            mensagemErro = null;

            try
            {
                Repositorio.Embarcador.Cargas.CargaCIOT repositorioCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);

                Dominio.Entidades.Embarcador.CIOT.CIOTEFrete configuracaoEFrete = ObterConfiguracaoEFrete(cargaCIOT.CIOT.ConfiguracaoCIOT, unitOfWork);

                if (cargaCIOT == null)
                {
                    mensagemErro = "Não foi possível localizar a carga vinculada ao CIOT.";
                    return false;
                }

                string token = Login(configuracaoEFrete, unitOfWork, out mensagemErro);

                if (string.IsNullOrWhiteSpace(token))
                    return false;

                bool sucesso = ProcessarMovimentoFinanceiro(cargaCIOT.CIOT, cargaCIOT, valorMovimento, justificativa, configuracaoEFrete, token, unitOfWork, out mensagemErro);

                Logout(configuracaoEFrete, token, unitOfWork);

                return sucesso;
            }
            catch
            {
                mensagemErro = $"Erro ao integrar movimento financeiro";
                return false;
            }
        }

        public bool CancelarCIOT(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unidadeTrabalho, out string mensagemErro)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unidadeTrabalho);
            Dominio.Entidades.Embarcador.CIOT.CIOTEFrete configuracao = ObterConfiguracaoEFrete(ciot.ConfiguracaoCIOT, unidadeTrabalho);

            string token = Login(configuracao, unidadeTrabalho, out mensagemErro);

            ServicoEFrete.PEF.CancelarOperacaoTransporteRequest requisicao = new ServicoEFrete.PEF.CancelarOperacaoTransporteRequest()
            {
                CodigoIdentificacaoOperacao = ciot.Numero + "/" + ciot.CodigoVerificador,
                Integrador = configuracao.CodigoIntegradorEFrete,
                Motivo = tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador ? "CANCELAMENTO GERADO PELO EMBARCADOR" : string.IsNullOrWhiteSpace(ciot.MotivoCancelamento) ? "Cancelamento por erro operacional." : ciot.MotivoCancelamento,
                Token = token,
                Versao = 1
            };

            ServicoEFrete.PEF.PefServiceSoapClient svcPEF = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unidadeTrabalho).ObterClient<ServicoEFrete.PEF.PefServiceSoapClient, ServicoEFrete.PEF.PefServiceSoap>(TipoWebServiceIntegracao.EFrete_Pef);

            ServicoEFrete.PEF.CancelarOperacaoTransporteResponse retorno = svcPEF.CancelarOperacaoTransporte(requisicao);

            bool sucesso = false;

            if (retorno.Sucesso)
            {
                ciot.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Cancelado;
                ciot.Mensagem = "Cancelamento realizado com sucesso.";
                ciot.DataCancelamento = DateTime.Now;
                ciot.ProtocoloCancelamento = retorno.Protocolo;
                mensagemErro = null;
                sucesso = true;
            }
            else
            {
                mensagemErro = retorno.Excecao?.Mensagem;
            }

            Logout(configuracao, token, unidadeTrabalho);

            return sucesso;
        }

        public bool EncerrarCIOT(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Repositorio.UnitOfWork unidadeTrabalho, out string mensagemErro)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unidadeTrabalho);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unidadeTrabalho);

            Dominio.Entidades.Embarcador.CIOT.CIOTEFrete configuracao = ObterConfiguracaoEFrete(ciot.ConfiguracaoCIOT, unidadeTrabalho);

            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unidadeTrabalho);
            List<Dominio.Entidades.Embarcador.Cargas.CargaCIOT> cargaCIOTs = repCargaCIOT.BuscarPorCIOT(ciot.Codigo);

            string token = Login(configuracao, unidadeTrabalho, out mensagemErro);


            ServicoEFrete.PEF.PefServiceSoapClient svcPEF = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unidadeTrabalho).ObterClient<ServicoEFrete.PEF.PefServiceSoapClient, ServicoEFrete.PEF.PefServiceSoap>(TipoWebServiceIntegracao.EFrete_Pef);
            //Conforme retorno da eFrete método foi descontinuado
            //Servicos.ServicoEFrete.PEF.RegistrarPagamentoQuitacaoRequest quitacaoRequest = new ServicoEFrete.PEF.RegistrarPagamentoQuitacaoRequest()
            //{
            //    CodigoIdentificacaoOperacao = ciot.Numero + "/" + ciot.CodigoVerificador,
            //    Integrador = configuracao.CodigoIntegradorEFrete,
            //    Versao = 1,
            //    Token = token,
            //    TokenCompra = "123456", // em produção utiliza o token no celular, ou os 6 primeiros caractes do cpf ou cnpj do proprietario.
            //    NotasFiscais = ObterNotasFiscais5(ciot, cargaCIOTs, unidadeTrabalho),
            //};

            //Servicos.ServicoEFrete.PEF.RegistrarPagamentoQuitacaoResponse retornoAdicionarPagamentoResponse = svcPEF.RegistrarPagamentoQuitacao(quitacaoRequest);

            //if (!retornoAdicionarPagamentoResponse.Sucesso)
            //{
            //    string mensagem = retornoAdicionarPagamentoResponse.Excecao?.Mensagem;
            //    if (mensagem.Contains("saldo"))
            //        retornoAdicionarPagamentoResponse.Sucesso = true;
            //}


            bool sucesso = false;

            //if (retornoAdicionarPagamentoResponse.Sucesso)
            //{
            ServicoEFrete.PEF.EncerrarOperacaoTransporteRequest requisicao = new ServicoEFrete.PEF.EncerrarOperacaoTransporteRequest()
            {
                CodigoIdentificacaoOperacao = ciot.Numero + "/" + ciot.CodigoVerificador,
                Integrador = configuracao.CodigoIntegradorEFrete,
                Versao = 2,
                //Impostos = ObterImpostos(ciot),
                PesoCarga = repPedidoXMLNotaFiscal.BuscarPesoPorCarga((from obj in cargaCIOTs select obj.Carga.Codigo).Distinct().ToList()),
                Token = token
            };


            ServicoEFrete.PEF.EncerrarOperacaoTransporteResponse retorno = svcPEF.EncerrarOperacaoTransporte(requisicao);


            if (retorno.Sucesso)
            {
                ciot.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Encerrado;
                ciot.Mensagem = "Encerramento realizado com sucesso.";
                ciot.DataEncerramento = DateTime.Now;
                ciot.ProtocoloEncerramento = retorno.Protocolo;
                mensagemErro = null;
                sucesso = true;
            }
            else
            {
                mensagemErro = retorno.Excecao?.Mensagem;
            }
            //}
            //else
            //{
            //    mensagemErro = retornoAdicionarPagamentoResponse.Excecao?.Mensagem;
            //}


            Logout(configuracao, token, unidadeTrabalho);

            return sucesso;
        }

        public bool IntegrarMotorista(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Dominio.Entidades.Usuario motorista, Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT configuracaoCIOT, string token, out string mensagemErro, Repositorio.UnitOfWork unitOfWork)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            ServicoEFrete.Motorista.MotoristasServiceSoapClient svcMotorista = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<ServicoEFrete.Motorista.MotoristasServiceSoapClient, ServicoEFrete.Motorista.MotoristasServiceSoap>(TipoWebServiceIntegracao.EFrete_Motoristas, out Servicos.Models.Integracao.InspectorBehavior inspector);

            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo repCIOTIntegracaoArquivo = new Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo(unitOfWork);

            Dominio.Entidades.Embarcador.CIOT.CIOTEFrete configuracao = ObterConfiguracaoEFrete(configuracaoCIOT, unitOfWork);

            if (motorista == null)
            {
                mensagemErro = "O motorista está inválido.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(motorista.Celular) || !motorista.Celular.Contains(" "))
            {
                mensagemErro = "O telefone celular do motorista " + motorista.Nome + " está inválido.";
                return false;
            }

            if (motorista.Localidade == null)
            {
                mensagemErro = "A localidade do motorista " + motorista.Nome + " é obrigatória.";
                return false;
            }

            if (!motorista.DataNascimento.HasValue)
            {
                mensagemErro = "Data de nascimento do motorista " + motorista.Nome + " é obrigatória.";
                return false;
            }

            string numeroHabilitacao = Utilidades.String.OnlyNumbers(motorista.NumeroHabilitacao);
            if (string.IsNullOrWhiteSpace(numeroHabilitacao))
            {
                mensagemErro = "O número da habilitação do motorista " + motorista.Nome + " está inválido.";
                return false;
            }

            ServicoEFrete.Motorista.GravarRequest requisicao = new ServicoEFrete.Motorista.GravarRequest()
            {
                CNH = long.Parse(numeroHabilitacao),
                CPF = long.Parse(Utilidades.String.OnlyNumbers(motorista.CPF)),
                DataNascimento = motorista.DataNascimento,
                Endereco = new ServicoEFrete.Motorista.Endereco()
                {
                    Bairro = motorista.Bairro,
                    CEP = motorista.CEP,
                    CodigoMunicipio = motorista.Localidade.CodigoIBGE,
                    Complemento = motorista.Complemento,
                    Numero = "S/N",
                    Rua = motorista.Endereco
                },
                Integrador = configuracao.CodigoIntegradorEFrete,
                Nome = motorista.Nome,
                Telefones = new ServicoEFrete.Motorista.Telefones()
                {
                    Celular = new ServicoEFrete.Motorista.Telefone()
                    {
                        DDD = int.Parse(Utilidades.String.OnlyNumbers(motorista.Celular?.Split(' ')[0])),
                        Numero = int.Parse(Utilidades.String.OnlyNumbers(motorista.Celular?.Split(' ')[1]))
                    }
                },
                Token = token,
                Versao = 2
            };

            ServicoEFrete.Motorista.GravarResponse retorno = svcMotorista.Gravar(requisicao);

            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo integracaoArquivo = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();

            if (!retorno.Sucesso)
            {
                mensagemErro = retorno.Excecao?.Mensagem;
                integracaoArquivo.Mensagem = mensagemErro;
            }
            else
            {
                mensagemErro = null;
                integracaoArquivo.Mensagem = "Motorista integrado com sucesso.";
            }

            Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao arquivosTransacao = (from o in motorista.Integracoes where o.TipoIntegracao != null && o.ConfiguracaoCIOT != null && o.ConfiguracaoCIOT.Codigo == configuracaoCIOT.Codigo select o).FirstOrDefault();
            if (arquivosTransacao != null)
            {
                integracaoArquivo.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(XML.ConvertObjectToXMLString(requisicao), "xml", unitOfWork);
                integracaoArquivo.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(XML.ConvertObjectToXMLString(retorno), "xml", unitOfWork);
                integracaoArquivo.Data = DateTime.Now;
                integracaoArquivo.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento;

                repCargaCTeIntegracaoArquivo.Inserir(integracaoArquivo);
                arquivosTransacao.ArquivosTransacao.Add(integracaoArquivo);
            }

            if (ciot != null)
            {
                Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo ciotIntegracaoArquivo = new Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo();

                ciotIntegracaoArquivo.Mensagem = retorno.Sucesso ? "Motorista integrado com sucesso." : retorno.Excecao?.Mensagem;
                ciotIntegracaoArquivo.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(XML.ConvertObjectToXMLString(inspector.LastRequestXML), "xml", unitOfWork);
                ciotIntegracaoArquivo.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(XML.ConvertObjectToXMLString(inspector.LastResponseXML), "xml", unitOfWork);
                ciotIntegracaoArquivo.Data = DateTime.Now;
                ciotIntegracaoArquivo.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento;

                repCIOTIntegracaoArquivo.Inserir(ciotIntegracaoArquivo);
                ciot.ArquivosTransacao.Add(ciotIntegracaoArquivo);
            }

            return retorno.Sucesso;
        }

        public bool IntegrarClienteProprietario(Dominio.Entidades.Cliente proprietario, Dominio.Entidades.Embarcador.CIOT.CIOTEFrete configuracao, Repositorio.UnitOfWork unitOfWork, out string mensagemErro)
        {
            mensagemErro = "";
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                string token = Login(configuracao, unitOfWork, out mensagemErro);
                bool sucessoLogin = string.IsNullOrWhiteSpace(mensagemErro);

                if (IntegrarProprietario(null, proprietario, configuracao, token, unitOfWork, out mensagemErro))
                {
                    Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas repModalidadeTransportador = new Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas(unitOfWork);
                    Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidade = repModalidadeTransportador.BuscarPorPessoa(proprietario.CPF_CNPJ);
                    int rntrc = 0;
                    int.TryParse(modalidade.RNTRC, out rntrc);

                    ServicoEFrete.Proprietario.ProprietariosServiceSoapClient svcProprietario = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<ServicoEFrete.Proprietario.ProprietariosServiceSoapClient, ServicoEFrete.Proprietario.ProprietariosServiceSoap>(TipoWebServiceIntegracao.EFrete_Proprietarios);

                    ServicoEFrete.Proprietario.ObterResponse retornoConsulta = svcProprietario.Obter(new ServicoEFrete.Proprietario.ObterRequest()
                    {
                        CNPJ = (long)proprietario.CPF_CNPJ,
                        RNTRC = rntrc,
                        Token = token,
                        Versao = 2,
                        Integrador = configuracao.CodigoIntegradorEFrete
                    });

                    if (retornoConsulta.Sucesso) //Proprietário já existe na e-Frete
                    {
                        return retornoConsulta.Proprietario.TACouEquiparado;
                    }
                }
                Logout(configuracao, token, unitOfWork);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
            return false;
        }

        public bool IntegrarProprietario(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Dominio.Entidades.Cliente proprietario, Dominio.Entidades.Embarcador.CIOT.CIOTEFrete configuracao, string token, Repositorio.UnitOfWork unidadeTrabalho, out string mensagemErro)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas repModalidadeTransportador = new Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas(unidadeTrabalho);
            Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo repCIOTIntegracaoArquivo = new Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidade = repModalidadeTransportador.BuscarPorPessoa(proprietario.CPF_CNPJ);

            if (modalidade == null)
            {
                mensagemErro = "A modalidade do proprietário não está configurada.";
                return false;
            }

            ServicoEFrete.Proprietario.ProprietariosServiceSoapClient svcProprietario = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unidadeTrabalho).ObterClient<ServicoEFrete.Proprietario.ProprietariosServiceSoapClient, ServicoEFrete.Proprietario.ProprietariosServiceSoap>(TipoWebServiceIntegracao.EFrete_Proprietarios, out Servicos.Models.Integracao.InspectorBehavior inspector);

            int rntrc = 0;
            int.TryParse(modalidade.RNTRC, out rntrc);

#if DEBUG
            //ServicoEFrete.Proprietario.ObterResponse retornoConsulta = svcProprietario.Obter(new ServicoEFrete.Proprietario.ObterRequest()
            //{
            //    CNPJ = (long)proprietario.CPF_CNPJ,
            //    RNTRC = rntrc,
            //    Token = token,
            //    Versao = 2,
            //    Integrador = configuracao.CodigoIntegradorEFrete
            //});

            //if (retornoConsulta.Sucesso) //Proprietário já existe na e-Frete
            //{
            //    mensagemErro = null;
            //    return true;
            //}
#endif

            string telefone = Utilidades.String.OnlyNumbers(proprietario.Telefone1);

            if (string.IsNullOrWhiteSpace(telefone))
            {
                mensagemErro = "O telefone do transportador está inválido.";
                return false;
            }

            bool ciotTestes = (!ciot.CargaCIOT?.FirstOrDefault().Carga.FreteDeTerceiro ?? true) && ciot.CargaCIOT?.FirstOrDefault().Carga.Empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao;

            ServicoEFrete.Proprietario.GravarRequest requisicao = new ServicoEFrete.Proprietario.GravarRequest()
            {
                CNPJ = !ciotTestes ? (long)proprietario.CPF_CNPJ : 07201103000169,
                Endereco = new ServicoEFrete.Proprietario.Endereco()
                {
                    Bairro = proprietario.Bairro,
                    CEP = proprietario.CEP,
                    CodigoMunicipio = proprietario.Localidade.CodigoIBGE,
                    Complemento = proprietario.Complemento,
                    Numero = proprietario.Numero,
                    Rua = proprietario.Endereco
                },
                Integrador = configuracao.CodigoIntegradorEFrete,
                RazaoSocial = proprietario.Nome,
                RNTRC = !ciotTestes ? rntrc : 11029129,
                Telefones = new ServicoEFrete.Proprietario.Telefones()
                {
                    Fixo = new ServicoEFrete.Proprietario.Telefone()
                    {
                        DDD = int.Parse(Utilidades.String.OnlyNumbers(telefone.Substring(0, 2))),
                        Numero = int.Parse(Utilidades.String.OnlyNumbers(telefone.Remove(0, 2)))
                    }
                },
                Token = token,
                TipoPessoa = proprietario.Tipo == "F" ? ServicoEFrete.Proprietario.TipoPessoa.Fisica : ServicoEFrete.Proprietario.TipoPessoa.Juridica,
                Versao = 3
            };

            ServicoEFrete.Proprietario.GravarResponse retorno = svcProprietario.Gravar(requisicao);

            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo integracaoArquivo = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();

            if (!retorno.Sucesso)
                mensagemErro = retorno.Excecao?.Mensagem;
            else
            {
                mensagemErro = null;
                integracaoArquivo.Mensagem = "Operação efetuada com sucesso.";
            }

            if (ciot != null)
            {
                Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo ciotIntegracaoArquivo = new Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo();

                ciotIntegracaoArquivo.Mensagem = string.IsNullOrWhiteSpace(mensagemErro) ? "Transportador/proprietário integrado com sucesso." : mensagemErro;
                ciotIntegracaoArquivo.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(XML.ConvertObjectToXMLString(inspector.LastRequestXML), "xml", unidadeTrabalho);
                ciotIntegracaoArquivo.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(XML.ConvertObjectToXMLString(inspector.LastResponseXML), "xml", unidadeTrabalho);
                ciotIntegracaoArquivo.Data = DateTime.Now;
                ciotIntegracaoArquivo.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento;

                repCIOTIntegracaoArquivo.Inserir(ciotIntegracaoArquivo);

                if (ciot.ArquivosTransacao == null)
                    ciot.ArquivosTransacao = new List<Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo>();

                ciot.ArquivosTransacao.Add(ciotIntegracaoArquivo);
            }

            return retorno.Sucesso;
        }

        public bool IntegrarVeiculos(List<Dominio.Entidades.Veiculo> veiculosTransportador, Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Dominio.Entidades.Embarcador.CIOT.CIOTEFrete configuracao, string token, out string mensagemErro, Repositorio.UnitOfWork unidadeTrabalho)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo repCIOTIntegracaoArquivo = new Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo(unidadeTrabalho);

            bool ciotTestes = (!ciot.CargaCIOT?.FirstOrDefault().Carga.FreteDeTerceiro ?? true) && ciot.CargaCIOT?.FirstOrDefault().Carga.Empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao;

            foreach (Dominio.Entidades.Veiculo veiculo in veiculosTransportador)
            {
                ServicoEFrete.Veiculo.GravarRequest requisicao = ObterRequisicaoVeiculo(veiculo, ciot.Transportador.Localidade, configuracao, token, ciotTestes);

                ServicoEFrete.Veiculo.VeiculosServiceSoapClient svcVeiculo = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unidadeTrabalho).ObterClient<ServicoEFrete.Veiculo.VeiculosServiceSoapClient, ServicoEFrete.Veiculo.VeiculosServiceSoap>(TipoWebServiceIntegracao.EFrete_Veiculos, out Servicos.Models.Integracao.InspectorBehavior inspector);

                ServicoEFrete.Veiculo.GravarResponse retorno = svcVeiculo.Gravar(requisicao);

                if (!retorno.Sucesso)
                {
                    mensagemErro = "Erro ao integrar o veículo " + veiculo.Placa + ": " + retorno.Excecao?.Mensagem;
                    return retorno.Sucesso;
                }

                Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo ciotIntegracaoArquivo = new Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo();

                ciotIntegracaoArquivo.Mensagem = retorno.Sucesso ? "Veículo " + veiculo.Placa + " integrado com sucesso." : "Erro ao integrar o veículo " + veiculo.Placa + ": " + retorno.Excecao?.Mensagem;
                ciotIntegracaoArquivo.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(XML.ConvertObjectToXMLString(inspector.LastRequestXML), "xml", unidadeTrabalho);
                ciotIntegracaoArquivo.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(XML.ConvertObjectToXMLString(inspector.LastResponseXML), "xml", unidadeTrabalho);
                ciotIntegracaoArquivo.Data = DateTime.Now;
                ciotIntegracaoArquivo.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento;

                repCIOTIntegracaoArquivo.Inserir(ciotIntegracaoArquivo);
                ciot.ArquivosTransacao.Add(ciotIntegracaoArquivo);

                if (!ciotTestes)
                {
                    ICollection<Dominio.Entidades.Veiculo> veiculosVinculadosCarga = ciot.CargaCIOT?.FirstOrDefault().Carga.VeiculosVinculados ?? null;

                    if (veiculosVinculadosCarga != null && veiculosVinculadosCarga.Count > 0)
                    {
                        foreach (Dominio.Entidades.Veiculo veiculoVinculado in veiculosVinculadosCarga)
                        {
                            requisicao = ObterRequisicaoVeiculo(veiculoVinculado, ciot.Transportador.Localidade, configuracao, token);
                            retorno = svcVeiculo.Gravar(requisicao);

                            if (!retorno.Sucesso)
                            {
                                mensagemErro = "Erro ao integrar o veículo " + veiculoVinculado.Placa + ": " + retorno.Excecao?.Mensagem;
                                return retorno.Sucesso;
                            }

                            ciotIntegracaoArquivo.Mensagem = retorno.Sucesso ? "Veículo " + veiculoVinculado.Placa + " integrado com sucesso." : "Erro ao integrar o veículo " + veiculoVinculado.Placa + ": " + retorno.Excecao?.Mensagem;
                            ciotIntegracaoArquivo.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(XML.ConvertObjectToXMLString(inspector.LastRequestXML), "xml", unidadeTrabalho);
                            ciotIntegracaoArquivo.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(XML.ConvertObjectToXMLString(inspector.LastResponseXML), "xml", unidadeTrabalho);
                            ciotIntegracaoArquivo.Data = DateTime.Now;
                            ciotIntegracaoArquivo.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento;

                            repCIOTIntegracaoArquivo.Inserir(ciotIntegracaoArquivo);
                            ciot.ArquivosTransacao.Add(ciotIntegracaoArquivo);
                        }
                    }
                }
            }

            mensagemErro = null;
            return true;
        }

        public bool GerarRegistroDeDesembarque(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Repositorio.UnitOfWork unidadeTrabalho, out string mensagemErro)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unidadeTrabalho);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unidadeTrabalho);
            Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo repCIOTIntegracaoArquivo = new Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo(unidadeTrabalho);

            Dominio.Entidades.Embarcador.CIOT.CIOTEFrete configuracao = ObterConfiguracaoEFrete(ciot.ConfiguracaoCIOT, unidadeTrabalho);

            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unidadeTrabalho);
            List<Dominio.Entidades.Embarcador.Cargas.CargaCIOT> cargaCIOTs = repCargaCIOT.BuscarPorCIOT(ciot.Codigo);

            string token = Login(configuracao, unidadeTrabalho, out mensagemErro);

            ServicoEFrete.PEF.PefServiceSoapClient svcPEF = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unidadeTrabalho).ObterClient<ServicoEFrete.PEF.PefServiceSoapClient, ServicoEFrete.PEF.PefServiceSoap>(TipoWebServiceIntegracao.EFrete_Pef, out Servicos.Models.Integracao.InspectorBehavior inspector);

            bool sucesso = false;

            ServicoEFrete.PEF.RegistrarQuantidadeDaMercadoriaNoDesembarqueRequest requisicao = new ServicoEFrete.PEF.RegistrarQuantidadeDaMercadoriaNoDesembarqueRequest()
            {
                CodigoIdentificacaoOperacao = ciot.Numero + "/" + ciot.CodigoVerificador,
                Integrador = configuracao.CodigoIntegradorEFrete,
                Versao = 1,
                NotasFiscais = ObterNotasFiscais4(ciot, cargaCIOTs, unidadeTrabalho),
                Token = token
            };

            ServicoEFrete.PEF.RegistrarQuantidadeDaMercadoriaNoDesembarqueResponse retorno = svcPEF.RegistrarQuantidadeDaMercadoriaNoDesembarque(requisicao);

            Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo ciotIntegracaoArquivo = new Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo();

            if (retorno.Sucesso)
            {
                ciotIntegracaoArquivo.Mensagem = "Registro de desembarque realizado com sucesso.";
                ciot.Mensagem = "Registro de desembarque realizado com sucesso.";
                ciot.DataEncerramento = DateTime.Now;
                mensagemErro = null;
                sucesso = true;
            }
            else
            {
                ciotIntegracaoArquivo.Mensagem = retorno.Excecao?.Mensagem;
                mensagemErro = retorno.Excecao?.Mensagem;
            }

            ciotIntegracaoArquivo.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", unidadeTrabalho);
            ciotIntegracaoArquivo.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", unidadeTrabalho);
            ciotIntegracaoArquivo.Data = DateTime.Now;
            ciotIntegracaoArquivo.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento;
            repCIOTIntegracaoArquivo.Inserir(ciotIntegracaoArquivo);
            ciot.ArquivosTransacao.Add(ciotIntegracaoArquivo);
            repCIOT.Atualizar(ciot);

            Logout(configuracao, token, unidadeTrabalho);

            return sucesso;
        }


        private bool IntegrarOperacaoTransporte(ref Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Dominio.Entidades.Empresa empresa, Dominio.Entidades.Embarcador.CIOT.CIOTEFrete configuracao, string token, List<Dominio.Entidades.Veiculo> veiculosTransportador, Dominio.Entidades.Usuario motorista, Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidade, Repositorio.UnitOfWork unidadeTrabalho, out string mensagemErro, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unidadeTrabalho);
                Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo repCIOTIntegracaoArquivo = new Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo(unidadeTrabalho);

                List<Dominio.Entidades.Embarcador.Cargas.CargaCIOT> cargaCIOTs = repCargaCIOT.BuscarPorCIOT(ciot.Codigo);

                Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoasTipoPagamentoCIOT repModalidadeTransportadoraPessoasTipoPagamentoCIOT = new Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoasTipoPagamentoCIOT(unidadeTrabalho);
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT? tipoPagamentoCIOTConfiguracao = repModalidadeTransportadoraPessoasTipoPagamentoCIOT.BuscarTipoPagamentoPorOperadora(modalidade.Codigo, OperadoraCIOT.eFrete);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoProprietarioVeiculo tipoProprietarioVeiculo = modalidade?.TipoTransportador ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoProprietarioVeiculo.Outros;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT tipoPagamentoCIOT = tipoPagamentoCIOTConfiguracao ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT.SemPgto;

                if (configuracao.TipoPagamento.HasValue)
                    tipoPagamentoCIOT = ObterTipoPagamentoCIOT(configuracao.TipoPagamento.Value);

                ServicoEFrete.PEF.AdicionarOperacaoTransporteRequest requisicao = new ServicoEFrete.PEF.AdicionarOperacaoTransporteRequest()
                {
                    TipoViagem = ServicoEFrete.PEF.TipoViagem.Padrao, //tipoProprietarioVeiculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoProprietarioVeiculo.TACAgregado ? ServicoEFrete.PEF.TipoViagem.TAC_Agregado : ServicoEFrete.PEF.TipoViagem.Padrao,
                    Integrador = configuracao.CodigoIntegradorEFrete, //ciot.Contratante.Configuracao.CodigoIntegradorEFrete,
                    Versao = 7,
                    BloquearNaoEquiparado = ciot.Contratante?.Configuracao.BloquearNaoEquiparadoEFrete ?? false,
                    MatrizCNPJ = ciot.Contratante != null ? long.Parse(ciot.Contratante.CNPJ) : long.Parse(configuracao.MatrizEFrete.CNPJ),
                    IdOperacaoCliente = ciot.Codigo.ToString(),
                    DataFimViagem = ciot.DataFinalViagem,
                    Impostos = ObterImpostos(cargaCIOTs, unidadeTrabalho),
                    Contratado = ObterContratado(ciot, unidadeTrabalho),
                    Motorista = ObterMotorista(motorista, out mensagemErro),
                    Contratante = ObterContratante(configuracao, empresa),
                    Veiculos = ObterVeiculos(ciot, veiculosTransportador, unidadeTrabalho),
                    EntregaDocumentacao = ServicoEFrete.PEF.TipoEntregaDocumentacao.Cliente,
                    Token = token,
                    TomadorServico = ObterTomador(cargaCIOTs.FirstOrDefault().Carga),
                };

                if (ciot.Contratante != null && (modalidade == null || tipoPagamentoCIOTConfiguracao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT.SemPgto))
                {
                    if (tipoPagamentoCIOTConfiguracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT.Transferencia)
                        requisicao.TipoPagamento = ServicoEFrete.PEF.TipoPagamento.TransferenciaBancaria;
                    else
                        requisicao.EmissaoGratuita = ciot.Contratante.Configuracao.EmissaoGratuitaEFrete;
                }
                else
                    requisicao.TipoPagamento = ServicoEFrete.PEF.TipoPagamento.Outros;

                if (configuracao.TipoPagamento.HasValue)
                    requisicao.TipoPagamento = ObterTipoPagamentoEFrete(configuracao.TipoPagamento.Value);

                if (configuracao.EmissaoGratuita.HasValue)
                    requisicao.EmissaoGratuita = configuracao.EmissaoGratuita.Value;

                if (requisicao.TipoViagem == ServicoEFrete.PEF.TipoViagem.Padrao)
                {
                    requisicao.CodigoNCMNaturezaCarga = (cargaCIOTs.FirstOrDefault().Carga?.TipoDeCarga?.CodigoNaturezaCIOT ?? 0) > 0 ? cargaCIOTs.FirstOrDefault().Carga.TipoDeCarga.CodigoNaturezaCIOT : 1; //int.Parse(ciot..CodigoNatureza);
                    requisicao.TipoEmbalagem = ServicoEFrete.PEF.TipoEmbalagem.Caixa;
                    requisicao.DataInicioViagem = ciot.DataAbertura;
                    requisicao.Viagens = this.ObterViagens(ciot, cargaCIOTs, modalidade, unidadeTrabalho, configuracao, tipoPagamentoCIOTConfiguracao);
                    requisicao.PesoCarga = (from obj in requisicao.Viagens select obj.NotasFiscais.Sum(nf => nf.QuantidadeDaMercadoriaNoEmbarque)).Sum(); //(from obj in cargaCIOTs select obj.PesoBruto).Sum();
                    requisicao.Destinatario = this.ObterDestinatario(ciot, cargaCIOTs);
                    requisicao.Pagamentos = ObterPagamentos(ciot, tipoPagamentoCIOT, cargaCIOTs, tipoServicoMultisoftware, unidadeTrabalho);
                    requisicao.DestinacaoComercial = true;
                    requisicao.AltoDesempenho = false;
                    requisicao.FreteRetorno = false;
                    requisicao.DistanciaRetorno = cargaCIOTs.FirstOrDefault().Carga.Distancia > 0 ? (uint)cargaCIOTs.FirstOrDefault().Carga.Distancia : (uint)cargaCIOTs.FirstOrDefault().Carga.DadosSumarizados.Distancia;
                    requisicao.CodigoTipoCarga = (cargaCIOTs.FirstOrDefault().Carga?.TipoDeCarga?.TipoDeCargaEFrete ?? 0) > 0 ? (ushort?)cargaCIOTs.FirstOrDefault().Carga.TipoDeCarga.TipoDeCargaEFrete : (ushort?)(configuracao.CodigoTipoCarga != null ? configuracao.CodigoTipoCarga.Value : 1); //Na emissão com TipoViagem Padrão seu preenchimento é obrigatório. Na emissão com TipoViagem TAC_Agregado o campo não deve ser preenchido
                }

                ServicoEFrete.PEF.PefServiceSoapClient svcPEF = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unidadeTrabalho).ObterClient<ServicoEFrete.PEF.PefServiceSoapClient, ServicoEFrete.PEF.PefServiceSoap>(TipoWebServiceIntegracao.EFrete_Pef, out Servicos.Models.Integracao.InspectorBehavior inspector);

                ServicoEFrete.PEF.AdicionarOperacaoTransporteResponse retorno = svcPEF.AdicionarOperacaoTransporte(requisicao);

                Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo ciotIntegracaoArquivo = new Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo();

                if (retorno.Sucesso)
                {
                    ciot.Numero = Utilidades.String.Left(retorno.CodigoIdentificacaoOperacao, 12);
                    ciot.CodigoVerificador = Utilidades.String.Right(retorno.CodigoIdentificacaoOperacao, 4);
                    ciot.Mensagem = "Operação de transporte adicionada com sucesso.";
                    ciot.DataAbertura = DateTime.Now;
                    mensagemErro = "";
                    ciotIntegracaoArquivo.Mensagem = "Operação de transporte adicionada com sucesso.";
                }
                else
                {
                    ciot.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Pendencia;
                    ciot.Mensagem = retorno.Excecao?.Mensagem;
                    mensagemErro = ciot.Mensagem;
                    ciotIntegracaoArquivo.Mensagem = mensagemErro;
                }

                ciotIntegracaoArquivo.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", unidadeTrabalho);
                ciotIntegracaoArquivo.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", unidadeTrabalho);
                ciotIntegracaoArquivo.Data = DateTime.Now;
                ciotIntegracaoArquivo.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento;
                repCIOTIntegracaoArquivo.Inserir(ciotIntegracaoArquivo);
                ciot.ArquivosTransacao.Add(ciotIntegracaoArquivo);

                return retorno.Sucesso;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                mensagemErro = "Ocorreu uma falha ao integrar o CIOT";
                return false;
            }

        }


        private ServicoEFrete.PEF.Pagamento[] ObterPagamentos(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT tipoPagamentoCIOT, List<Dominio.Entidades.Embarcador.Cargas.CargaCIOT> cargaCIOTs, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unidadeTrabalho)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            List<ServicoEFrete.PEF.Pagamento> pagamentos = new List<ServicoEFrete.PEF.Pagamento>();

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGeralEFrete configuracaoIntegracaoEFrete = new Repositorio.Embarcador.Configuracoes.IntegracaoGeralEFrete(unidadeTrabalho).Buscar();

            decimal valorAdiantamento = tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? cargaCIOTs.Sum(obj => obj.ContratoFrete.ValorAdiantamento) : cargaCIOTs.Sum(obj => obj.ValorAdiantamento);
            decimal valorQuitacao = tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? cargaCIOTs.Sum(obj => obj.ContratoFrete.SaldoAReceber) : cargaCIOTs.Sum(obj => obj.ValorFrete - obj.ValorAdiantamento);

            if (configuracaoIntegracaoEFrete.DeduzirImpostosValorTotalFrete)
            {
                ServicoEFrete.PEF.ViagemValores valores = ObterImpostoDeduzidoValorTotalFrete(cargaCIOTs);
                valorAdiantamento = valores.TotalDeAdiantamento;
                valorQuitacao = valores.TotalDeQuitacao;
            }

            Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = cargaCIOTs.OrderBy(o => o.Codigo).FirstOrDefault();

            if (valorAdiantamento > 0)
            {
                var pagamentoAdiantamento = new ServicoEFrete.PEF.Pagamento()
                {
                    Categoria = ServicoEFrete.PEF.CategoriaPagamento.Adiantamento,
                    DataDeLiberacao = ciot.DataAbertura.Value,
                    Documento = cargaCIOT.Carga.CodigoCargaEmbarcador,
                    IdPagamentoCliente = $"{cargaCIOT.Codigo}_1",
                    InformacoesBancarias = tipoPagamentoCIOT == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT.SemPgto || tipoPagamentoCIOT == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT.Cartao ? null : this.ObterInformacoesBancarias(ciot),
                    Valor = valorAdiantamento
                };

                if (tipoPagamentoCIOT != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT.SemPgto)
                {
                    if (tipoPagamentoCIOT == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT.Transferencia)
                        pagamentoAdiantamento.TipoPagamento = ServicoEFrete.PEF.TipoPagamento.TransferenciaBancaria;
                    else
                        pagamentoAdiantamento.TipoPagamento = ServicoEFrete.PEF.TipoPagamento.eFRETE;
                }

                pagamentos.Add(pagamentoAdiantamento);
            }

            var pagamentoQuitacao = new ServicoEFrete.PEF.Pagamento()
            {
                Categoria = ServicoEFrete.PEF.CategoriaPagamento.Quitacao,
                DataDeLiberacao = ciot.DataFinalViagem,
                Documento = cargaCIOT.Carga.CodigoCargaEmbarcador,
                IdPagamentoCliente = $"{cargaCIOT.Codigo}_2",
                InformacoesBancarias = tipoPagamentoCIOT == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT.SemPgto || tipoPagamentoCIOT == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT.Cartao ? null : this.ObterInformacoesBancarias(ciot),
                Valor = valorQuitacao
            };

            if (tipoPagamentoCIOT != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT.SemPgto)
            {
                if (tipoPagamentoCIOT == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT.Transferencia)
                    pagamentoQuitacao.TipoPagamento = ServicoEFrete.PEF.TipoPagamento.TransferenciaBancaria;
                else
                    pagamentoQuitacao.TipoPagamento = ServicoEFrete.PEF.TipoPagamento.eFRETE;
            }

            pagamentos.Add(pagamentoQuitacao);

            return pagamentos.ToArray();
        }

        private ServicoEFrete.PEF.Viagem[] ObterViagens(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, List<Dominio.Entidades.Embarcador.Cargas.CargaCIOT> cargaCIOTs, Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidade, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.CIOT.CIOTEFrete configuracao, TipoPagamentoCIOT? tipoPagamentoCIOT)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = cargaCIOTs.FirstOrDefault().Carga.Pedidos.FirstOrDefault();

            string cepDestino = cargaPedido.Recebedor != null ? cargaPedido.Recebedor.CEP : cargaPedido.Pedido.Destinatario != null ? cargaPedido.Pedido.Destinatario.CEP : cargaPedido.Destino.CEP;
            string cepOrigem = cargaPedido.Expedidor != null ? cargaPedido.Expedidor.CEP : cargaPedido.Pedido.Remetente != null ? cargaPedido.Pedido.Remetente.CEP : cargaPedido.Origem.CEP;

            List<ServicoEFrete.PEF.Viagem> viagens = new List<ServicoEFrete.PEF.Viagem>();
            viagens.Add(new ServicoEFrete.PEF.Viagem()
            {
                CodigoMunicipioDestino = cargaPedido.Destino.CodigoIBGE,
                CepDestino = Utilidades.String.OnlyNumbers(cepDestino),
                CodigoMunicipioOrigem = cargaPedido.Origem.CodigoIBGE,
                CepOrigem = Utilidades.String.OnlyNumbers(cepOrigem),
                DocumentoViagem = cargaPedido.Carga.CodigoCargaEmbarcador,
                TipoPagamento = configuracao.TipoPagamento.HasValue ? ObterTipoPagamentoEFrete(configuracao.TipoPagamento.Value) : (modalidade != null && tipoPagamentoCIOT == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT.Transferencia ? ServicoEFrete.PEF.TipoPagamento.TransferenciaBancaria : ServicoEFrete.PEF.TipoPagamento.eFRETE),
                InformacoesBancarias = configuracao.TipoPagamento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoeFrete.eFrete ? null : this.ObterInformacoesBancarias(ciot),
                NotasFiscais = this.ObterNotasFiscais(ciot, cargaCIOTs, unitOfWork),
                Valores = this.ObterValoresViagem(cargaCIOTs, unitOfWork),
                DistanciaPercorrida = cargaPedido.Carga.Distancia > 0 ? (uint)cargaPedido.Carga.Distancia : cargaPedido.Carga.DadosSumarizados.Distancia > 0 ? (uint)cargaPedido.Carga.DadosSumarizados.Distancia : 1
            });

            return viagens.ToArray();
        }

        private ServicoEFrete.PEF.Viagem2[] ObterViagensTAC(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, List<Dominio.Entidades.Embarcador.Cargas.CargaCIOT> cargaCIOTs, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.CIOT.CIOTEFrete configuracao)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = cargaCIOTs.FirstOrDefault().Carga.Pedidos.FirstOrDefault();

            string cepDestino = cargaPedido.Recebedor != null ? cargaPedido.Recebedor.CEP : cargaPedido.Pedido.Destinatario != null ? cargaPedido.Pedido.Destinatario.CEP : cargaPedido.Destino.CEP;
            string cepOrigem = cargaPedido.Expedidor != null ? cargaPedido.Expedidor.CEP : cargaPedido.Pedido.Remetente != null ? cargaPedido.Pedido.Remetente.CEP : cargaPedido.Origem.CEP;

            List<ServicoEFrete.PEF.Viagem2> viagens = new List<ServicoEFrete.PEF.Viagem2>();
            viagens.Add(new ServicoEFrete.PEF.Viagem2()
            {
                CodigoMunicipioDestino = cargaPedido.Destino.CodigoIBGE,
                CepDestino = Utilidades.String.OnlyNumbers(cepDestino),
                CodigoMunicipioOrigem = cargaPedido.Origem.CodigoIBGE,
                CepOrigem = Utilidades.String.OnlyNumbers(cepOrigem),
                DocumentoViagem = cargaPedido.Carga.CodigoCargaEmbarcador,
                TipoPagamento = configuracao.TipoPagamento.HasValue ? ObterTipoPagamentoEFrete(configuracao.TipoPagamento.Value) : ServicoEFrete.PEF.TipoPagamento.eFRETE,
                InformacoesBancarias = configuracao.TipoPagamento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoeFrete.eFrete ? null : this.ObterInformacoesBancarias(ciot),
                NotasFiscais = this.ObterNotasFiscaisTAC(ciot, cargaCIOTs, unitOfWork),
                Valores = this.ObterValoresViagem(cargaCIOTs, unitOfWork)
            });

            return viagens.ToArray();
        }

        private ServicoEFrete.PEF.ViagemValores ObterValoresViagem(List<Dominio.Entidades.Embarcador.Cargas.CargaCIOT> cargaCIOTs, UnitOfWork unitOfWork)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGeralEFrete configuracaoIntegracaoEFrete = new Repositorio.Embarcador.Configuracoes.IntegracaoGeralEFrete(unitOfWork).Buscar();

            if (configuracaoIntegracaoEFrete.DeduzirImpostosValorTotalFrete)
                return ObterImpostoDeduzidoValorTotalFrete(cargaCIOTs);


            decimal totalOperacaoViagem = (from obj in cargaCIOTs select obj.ValorFrete + obj.ValorSeguro + obj.ValorIRRF + obj.ValorSENAT + obj.ValorSEST).Sum();

            ServicoEFrete.PEF.ViagemValores valores = new ServicoEFrete.PEF.ViagemValores()
            {
                Pedagio = 0,
                Seguro = (from obj in cargaCIOTs select obj.ValorSeguro).Sum(),
                TotalDeAdiantamento = (from obj in cargaCIOTs select obj.ValorAdiantamento).Sum(),
                TotalDeQuitacao = (from obj in cargaCIOTs select obj.ValorFrete - obj.ValorAdiantamento).Sum(),
                TotalOperacao = totalOperacaoViagem,
                TotalViagem = totalOperacaoViagem
            };

            if (!cargaCIOTs.FirstOrDefault().ContratoFrete.NaoSomarValorPedagio)
            {
                valores.Pedagio = (from obj in cargaCIOTs select obj.ContratoFrete.ValorPedagio).Sum();
                valores.TotalDeQuitacao = (from obj in cargaCIOTs select obj.ValorFrete - obj.ValorAdiantamento + obj.ContratoFrete.ValorPedagio).Sum();
            }

            return valores;
        }

        private ServicoEFrete.PEF.ViagemValores ObterImpostoDeduzidoValorTotalFrete(List<Dominio.Entidades.Embarcador.Cargas.CargaCIOT> cargaCIOTs)
        {

            decimal totalOperacao = Math.Floor(((from obj in cargaCIOTs select obj.ValorFrete + obj.ValorSeguro - obj.ContratoFrete?.ValorIRRF - obj.ContratoFrete?.ValorSENAT - obj.ContratoFrete?.ValorSEST - obj.ContratoFrete?.ValorINSS).Sum() ?? 0) * 100) / 100;

            ServicoEFrete.PEF.ViagemValores valores = new ServicoEFrete.PEF.ViagemValores()
            {
                Pedagio = Math.Floor((from obj in cargaCIOTs select obj.ContratoFrete.ValorPedagio).Sum() * 100) / 100,
                Seguro = Math.Floor((from obj in cargaCIOTs select obj.ValorSeguro).Sum() * 100) / 100,
                TotalOperacao = totalOperacao,
                TotalViagem = totalOperacao
            };

            if (!cargaCIOTs.FirstOrDefault().ContratoFrete.NaoSomarValorPedagio)
                valores.Pedagio = Math.Floor((from obj in cargaCIOTs select obj.ContratoFrete.ValorPedagio).Sum() * 100) / 100;


            valores.TotalDeAdiantamento = Math.Floor(valores.TotalViagem * cargaCIOTs.FirstOrDefault().ContratoFrete?.PercentualAdiantamento ?? 0 / 100 * 100) / 100;
            valores.TotalDeQuitacao = valores.TotalViagem - valores.TotalDeAdiantamento;

            return valores;
        }
        private ServicoEFrete.PEF.NotaFiscal5[] ObterNotasFiscais5(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, List<Dominio.Entidades.Embarcador.Cargas.CargaCIOT> cargaCIOTs, Repositorio.UnitOfWork unitOfWork)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotaFiscals = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            List<ServicoEFrete.PEF.NotaFiscal5> notasFiscais = new List<ServicoEFrete.PEF.NotaFiscal5>();
            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT in cargaCIOTs)
            {
                pedidoXMLNotaFiscals.AddRange(repPedidoXMLNotaFiscal.BuscarPorCarga(cargaCIOT.Carga.Codigo));

                List<ServicoEFrete.PEF.NotaFiscal5> notasFiscaisCarga = (from pedidoXMLNotaFiscal in pedidoXMLNotaFiscals
                                                                         select new ServicoEFrete.PEF.NotaFiscal5()
                                                                         {
                                                                             QuantidadeDaMercadoriaNoDesembarque = pedidoXMLNotaFiscal.XMLNotaFiscal.Peso,
                                                                             Numero = pedidoXMLNotaFiscal.XMLNotaFiscal.Numero.ToString(),
                                                                             Serie = pedidoXMLNotaFiscal.XMLNotaFiscal.Serie.ToString()
                                                                         }).ToList();

                notasFiscais.AddRange(notasFiscaisCarga);
            }
            return notasFiscais.ToArray();
        }

        private ServicoEFrete.PEF.NotaFiscal4[] ObterNotasFiscais4(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, List<Dominio.Entidades.Embarcador.Cargas.CargaCIOT> cargaCIOTs, Repositorio.UnitOfWork unitOfWork)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotaFiscals = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            List<ServicoEFrete.PEF.NotaFiscal4> notasFiscais = new List<ServicoEFrete.PEF.NotaFiscal4>();
            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT in cargaCIOTs)
            {
                pedidoXMLNotaFiscals.AddRange(repPedidoXMLNotaFiscal.BuscarPorCarga(cargaCIOT.Carga.Codigo));

                List<ServicoEFrete.PEF.NotaFiscal4> notasFiscaisCarga = (from pedidoXMLNotaFiscal in pedidoXMLNotaFiscals
                                                                         select new ServicoEFrete.PEF.NotaFiscal4()
                                                                         {
                                                                             QuantidadeDaMercadoriaNoDesembarque = pedidoXMLNotaFiscal.XMLNotaFiscal.Peso,
                                                                             Numero = pedidoXMLNotaFiscal.XMLNotaFiscal.Numero.ToString(),
                                                                             Serie = pedidoXMLNotaFiscal.XMLNotaFiscal.Serie.ToString()
                                                                         }).ToList();

                notasFiscais.AddRange(notasFiscaisCarga);
            }
            return notasFiscais.ToArray();
        }

        private ServicoEFrete.PEF.NotaFiscal2[] ObterNotasFiscaisTAC(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, List<Dominio.Entidades.Embarcador.Cargas.CargaCIOT> cargaCIOTs, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            List<ServicoEFrete.PEF.NotaFiscal2> notasFiscais = new List<ServicoEFrete.PEF.NotaFiscal2>();


            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotaFiscals = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT in cargaCIOTs)
            {
                pedidoXMLNotaFiscals.AddRange(repPedidoXMLNotaFiscal.BuscarPorCarga(cargaCIOT.Carga.Codigo));

                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal in pedidoXMLNotaFiscals)
                {
                    notasFiscais.Add(new ServicoEFrete.PEF.NotaFiscal2()
                    {
                        CodigoNCMNaturezaCarga = (cargaCIOT.Carga?.TipoDeCarga?.CodigoNaturezaCIOT ?? 0) > 0 ? cargaCIOT.Carga.TipoDeCarga.CodigoNaturezaCIOT : 1,
                        Data = pedidoXMLNotaFiscal.XMLNotaFiscal.DataEmissao,
                        ValorTotal = pedidoXMLNotaFiscal.XMLNotaFiscal.Valor,
                        Numero = pedidoXMLNotaFiscal.XMLNotaFiscal.Numero.ToString(),
                        Serie = pedidoXMLNotaFiscal.XMLNotaFiscal.SerieOuSerieDaChave,
                        QuantidadeDaMercadoriaNoEmbarque = pedidoXMLNotaFiscal.XMLNotaFiscal.Peso,
                        UnidadeDeMedidaDaMercadoria = ServicoEFrete.PEF.UnidadeDeMedidaDaMercadoria1.Kg,
                        TipoDeCalculo = ServicoEFrete.PEF.ViagemTipoDeCalculo1.SemQuebra,
                        ValorDaMercadoriaPorUnidade = Math.Round(pedidoXMLNotaFiscal.XMLNotaFiscal.Valor / pedidoXMLNotaFiscal.XMLNotaFiscal.Peso, 2),
                        ValorDoFretePorUnidadeDeMercadoria = Math.Round(cargaCIOT.Carga.ValorFrete / pedidoXMLNotaFiscal.XMLNotaFiscal.Peso, 2),
                        ToleranciaDePerdaDeMercadoria = new ServicoEFrete.PEF.ToleranciaDePerdaDeMercadoria1()
                        {
                            Tipo = ServicoEFrete.PEF.TipoToleranciaDePerdaDeMercadoria1.Nenhum,
                            Valor = 0
                        }
                    });
                }
            }



            return notasFiscais.ToArray();
        }


        private ServicoEFrete.PEF.NotaFiscal[] ObterNotasFiscais(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, List<Dominio.Entidades.Embarcador.Cargas.CargaCIOT> cargaCIOTs, Repositorio.UnitOfWork unitOfWork)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotaFiscals = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            List<ServicoEFrete.PEF.NotaFiscal> notasFiscais = new List<ServicoEFrete.PEF.NotaFiscal>();
            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT in cargaCIOTs)
            {
                pedidoXMLNotaFiscals.AddRange(repPedidoXMLNotaFiscal.BuscarPorCarga(cargaCIOT.Carga.Codigo));

                List<ServicoEFrete.PEF.NotaFiscal> notasFiscaisCarga = (from pedidoXMLNotaFiscal in pedidoXMLNotaFiscals
                                                                        select new ServicoEFrete.PEF.NotaFiscal()
                                                                        {
                                                                            CodigoNCMNaturezaCarga = (cargaCIOT.Carga?.TipoDeCarga?.CodigoNaturezaCIOT ?? 0) > 0 ? cargaCIOT.Carga.TipoDeCarga.CodigoNaturezaCIOT : 1, //int.Parse(ciot.NaturezaCarga.CodigoNatureza),
                                                                            Data = cargaCIOT.Carga.DataInicioGeracaoCTes.Value,
                                                                            ValorTotal = pedidoXMLNotaFiscal.XMLNotaFiscal.ValorTotalProdutos,
                                                                            Numero = pedidoXMLNotaFiscal.XMLNotaFiscal.Numero.ToString(),
                                                                            Serie = pedidoXMLNotaFiscal.XMLNotaFiscal.Serie.ToString(),
                                                                            QuantidadeDaMercadoriaNoEmbarque = pedidoXMLNotaFiscal.XMLNotaFiscal.Peso, //cargaCIOT.PesoBruto,
                                                                            UnidadeDeMedidaDaMercadoria = ServicoEFrete.PEF.UnidadeDeMedidaDaMercadoria.Kg,
                                                                            TipoDeCalculo = cargaCIOT.TipoQuebra == Dominio.Enumeradores.TipoQuebra.Integral ? ServicoEFrete.PEF.ViagemTipoDeCalculo.QuebraIntegral :
                                                                                            cargaCIOT.TipoQuebra == Dominio.Enumeradores.TipoQuebra.Parcial ? ServicoEFrete.PEF.ViagemTipoDeCalculo.QuebraSomenteUltrapassado :
                                                                                            ServicoEFrete.PEF.ViagemTipoDeCalculo.SemQuebra,
                                                                            ValorDaMercadoriaPorUnidade = Math.Round(cargaCIOT.ValorMercadoriaKG, 2, MidpointRounding.ToEven),
                                                                            ValorDoFretePorUnidadeDeMercadoria = Math.Round(cargaCIOT.ValorTarifaFrete, 2, MidpointRounding.ToEven),
                                                                            ToleranciaDePerdaDeMercadoria = new ServicoEFrete.PEF.ToleranciaDePerdaDeMercadoria()
                                                                            {
                                                                                Tipo = cargaCIOT.TipoTolerancia == Dominio.Enumeradores.TipoTolerancia.Percentual ? ServicoEFrete.PEF.TipoToleranciaDePerdaDeMercadoria.Porcentagem :
                                                                                       cargaCIOT.TipoTolerancia == Dominio.Enumeradores.TipoTolerancia.Peso ? ServicoEFrete.PEF.TipoToleranciaDePerdaDeMercadoria.ValorAbsoluto :
                                                                                       ServicoEFrete.PEF.TipoToleranciaDePerdaDeMercadoria.Nenhum,
                                                                                Valor = cargaCIOT.PercentualTolerancia
                                                                            },
                                                                            DiferencaDeFrete = new ServicoEFrete.PEF.DiferencaDeFrete()
                                                                            {
                                                                                Base = ServicoEFrete.PEF.DiferencaFreteBaseCalculo.QuantidadeDesembarque,
                                                                                Tipo = cargaCIOT.TipoQuebra == Dominio.Enumeradores.TipoQuebra.SemQuebra ? ServicoEFrete.PEF.DiferencaFreteTipo.SemDiferenca :
                                                                                       cargaCIOT.TipoQuebra == Dominio.Enumeradores.TipoQuebra.Parcial ? ServicoEFrete.PEF.DiferencaFreteTipo.SomenteUltrapassado : ServicoEFrete.PEF.DiferencaFreteTipo.Integral,
                                                                                Tolerancia = new ServicoEFrete.PEF.DiferencaFreteTolerancia()
                                                                                {
                                                                                    Tipo = cargaCIOT.TipoTolerancia == Dominio.Enumeradores.TipoTolerancia.Percentual ? ServicoEFrete.PEF.TipoProporcao.Porcentagem :
                                                                                           cargaCIOT.TipoTolerancia == Dominio.Enumeradores.TipoTolerancia.Peso ? ServicoEFrete.PEF.TipoProporcao.Absoluto :
                                                                                           ServicoEFrete.PEF.TipoProporcao.Nenhum,
                                                                                    Valor = cargaCIOT.PercentualTolerancia
                                                                                },
                                                                                MargemGanho = new ServicoEFrete.PEF.DiferencaFreteMargem()
                                                                                {
                                                                                    Tipo = cargaCIOT.TipoTolerancia == Dominio.Enumeradores.TipoTolerancia.Percentual ? ServicoEFrete.PEF.TipoProporcao.Porcentagem :
                                                                                           cargaCIOT.TipoTolerancia == Dominio.Enumeradores.TipoTolerancia.Peso ? ServicoEFrete.PEF.TipoProporcao.Absoluto :
                                                                                           ServicoEFrete.PEF.TipoProporcao.Nenhum,
                                                                                    Valor = cargaCIOT.PercentualToleranciaSuperior
                                                                                },
                                                                                MargemPerda = new ServicoEFrete.PEF.DiferencaFreteMargem()
                                                                                {
                                                                                    Tipo = cargaCIOT.TipoTolerancia == Dominio.Enumeradores.TipoTolerancia.Percentual ? ServicoEFrete.PEF.TipoProporcao.Porcentagem :
                                                                                           cargaCIOT.TipoTolerancia == Dominio.Enumeradores.TipoTolerancia.Peso ? ServicoEFrete.PEF.TipoProporcao.Absoluto :
                                                                                           ServicoEFrete.PEF.TipoProporcao.Nenhum,
                                                                                    Valor = cargaCIOT.PercentualTolerancia
                                                                                }
                                                                            }
                                                                        }).ToList();

                notasFiscais.AddRange(notasFiscaisCarga);
            }
            return notasFiscais.ToArray();
        }

        private ServicoEFrete.PEF.Impostos ObterImpostos(List<Dominio.Entidades.Embarcador.Cargas.CargaCIOT> cargaCIOTs, Repositorio.UnitOfWork unitOfWork)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGeralEFrete configuracaoIntegracaoEFrete = new Repositorio.Embarcador.Configuracoes.IntegracaoGeralEFrete(unitOfWork).Buscar();

            ServicoEFrete.PEF.Impostos impostos = new ServicoEFrete.PEF.Impostos();
            impostos.INSS = (from obj in cargaCIOTs select (configuracaoIntegracaoEFrete.EnviarImpostosNaIntegracaoDoCIOT ? obj?.ContratoFrete?.ValorINSS ?? 0 : obj.ValorINSS)).Sum();
            impostos.IRRF = (from obj in cargaCIOTs select (configuracaoIntegracaoEFrete.EnviarImpostosNaIntegracaoDoCIOT ? obj?.ContratoFrete?.ValorIRRF ?? 0 : obj.ValorIRRF)).Sum();
            impostos.SestSenat = (from obj in cargaCIOTs select (configuracaoIntegracaoEFrete.EnviarImpostosNaIntegracaoDoCIOT ? (obj?.ContratoFrete?.ValorSEST ?? 0) + (obj?.ContratoFrete?.ValorSENAT ?? 0) : obj.ValorSEST + obj.ValorSENAT)).Sum();

            return impostos;
        }

        private ServicoEFrete.PEF.TomadorServico ObterTomador(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Dominio.Entidades.Cliente clienteTomador = carga.Pedidos.FirstOrDefault().ObterTomador();

            ServicoEFrete.PEF.TomadorServico tomador = new ServicoEFrete.PEF.TomadorServico()
            {
                CpfOuCnpj = long.Parse(Utilidades.String.OnlyNumbers(clienteTomador.CPF_CNPJ_SemFormato)),
                Endereco = new ServicoEFrete.PEF.Endereco()
                {
                    Bairro = clienteTomador.Bairro,
                    CEP = clienteTomador.CEP,
                    CodigoMunicipio = clienteTomador.Localidade.CodigoIBGE,
                    Complemento = clienteTomador.Complemento,
                    Numero = clienteTomador.Numero,
                    Rua = clienteTomador.Endereco
                },
                EMail = clienteTomador.Email,
                NomeOuRazaoSocial = clienteTomador.Nome,
                ResponsavelPeloPagamento = true
            };

            return tomador;
        }

        public bool IntegrarVeiculoEProprietario(Dominio.Entidades.Veiculo veiculo, Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT configuracaoCIOT, string token, Repositorio.UnitOfWork unidadeTrabalho, out string mensagemErro)
        {

            Dominio.Entidades.Embarcador.CIOT.CIOTEFrete configuracao = ObterConfiguracaoEFrete(configuracaoCIOT, unidadeTrabalho);

            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unidadeTrabalho);

            if (veiculo.Proprietario != null && !IntegrarProprietario(null, veiculo.Proprietario, configuracao, token, unidadeTrabalho, out mensagemErro))
            {
                mensagemErro = "Integração Proprietário: " + mensagemErro;
                return false;
            }

            ServicoEFrete.Veiculo.GravarRequest requisicao = ObterRequisicaoVeiculo(veiculo, veiculo.Empresa.Localidade, configuracao, token);
            ServicoEFrete.Veiculo.VeiculosServiceSoapClient svcVeiculo = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unidadeTrabalho).ObterClient<ServicoEFrete.Veiculo.VeiculosServiceSoapClient, ServicoEFrete.Veiculo.VeiculosServiceSoap>(TipoWebServiceIntegracao.EFrete_Veiculos);
            ServicoEFrete.Veiculo.GravarResponse retorno = svcVeiculo.Gravar(requisicao);

            Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao arquivosTransacao = (from o in veiculo.Integracoes where o.TipoIntegracao != null && o.ConfiguracaoCIOT.Codigo == configuracaoCIOT.Codigo select o).FirstOrDefault();
            if (arquivosTransacao != null)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo integracaoArquivo = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo
                {
                    Mensagem = retorno.Sucesso ? "Operação efetuada com sucesso." : retorno.Excecao?.Mensagem,
                    ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(XML.ConvertObjectToXMLString(requisicao), "xml", unidadeTrabalho),
                    ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(XML.ConvertObjectToXMLString(retorno), "xml", unidadeTrabalho),
                    Data = DateTime.Now,
                    Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento
                };

                repCargaCTeIntegracaoArquivo.Inserir(integracaoArquivo);
                arquivosTransacao.ArquivosTransacao.Add(integracaoArquivo);
            }

            if (!retorno.Sucesso)
            {
                mensagemErro = "Erro ao integrar o veículo " + veiculo.Placa + ": " + retorno.Excecao?.Mensagem;
                return retorno.Sucesso;
            }           

            mensagemErro = null;
            return true;
        }

        public byte[] ObterOperacaoTransportePdf(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, Repositorio.UnitOfWork UnitOfWork, out string mensagemErro)
        {
            Repositorio.Embarcador.Configuracoes.Integracao repConfiguracao = new Repositorio.Embarcador.Configuracoes.Integracao(UnitOfWork);

            ServicoEFrete.PEF.PefServiceSoapClient svcPEF = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(UnitOfWork).ObterClient<ServicoEFrete.PEF.PefServiceSoapClient, ServicoEFrete.PEF.PefServiceSoap>(TipoWebServiceIntegracao.EFrete_Pef);

            Dominio.Entidades.Embarcador.CIOT.CIOTEFrete configuracao = ObterConfiguracaoEFrete(cargaCIOT.ContratoFrete.ConfiguracaoCIOT, UnitOfWork);

            if (configuracao == null || cargaCIOT.CIOT == null)
            {
                mensagemErro = "Configuração não encontrada";
                return null;
            }

            string token = Login(configuracao, UnitOfWork, out mensagemErro);

            ServicoEFrete.PEF.ObterOperacaoTransportePdfRequest requisicao = new ServicoEFrete.PEF.ObterOperacaoTransportePdfRequest()
            {
                CodigoIdentificacaoOperacao = (!string.IsNullOrWhiteSpace(cargaCIOT.CIOT.Numero) ? cargaCIOT.CIOT.Numero : "") + "/" + (!string.IsNullOrWhiteSpace(cargaCIOT.CIOT.CodigoVerificador) ? cargaCIOT.CIOT.CodigoVerificador : ""),
                Token = token,
                DocumentoViagem = "",
                Integrador = configuracao.CodigoIntegradorEFrete,
                Versao = 1,
            };

            ServicoEFrete.PEF.ObterOperacaoTransportePdfResponse retorno = svcPEF.ObterOperacaoTransportePdf(requisicao);

            Logout(configuracao, token, UnitOfWork);

            if (!retorno.Sucesso)
            {
                mensagemErro = retorno.Excecao?.Mensagem;
                return null;
            }

            return retorno.Pdf;
        }

        public static List<Dominio.ObjetosDeValor.Embarcador.CIOT.FaturamentoEFrete> ObterFaturasPorIntervaloDeDatas(Dominio.Entidades.Embarcador.CIOT.CIOTEFrete configuracao, DateTime dataInicial, DateTime dataFinal, Dominio.Entidades.Empresa contratante, string _tipoData, Repositorio.UnitOfWork UnitOfWork, out string mensagemErro)
        {

            Repositorio.Embarcador.Configuracoes.Integracao repConfiguracao = new Repositorio.Embarcador.Configuracoes.Integracao(UnitOfWork);
            ServicoEFrete.FaturamentoTransportadora.FaturamentoTransportadoraServiceSoapClient svcFat = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(UnitOfWork).ObterClient<ServicoEFrete.FaturamentoTransportadora.FaturamentoTransportadoraServiceSoapClient, ServicoEFrete.FaturamentoTransportadora.FaturamentoTransportadoraServiceSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.EFrete_FaturamentoTransportadora);

            // Dados Requisicao
            ServicoEFrete.FaturamentoTransportadora.FaturamentoTransportadoraObterFaturasPorIntervaloDeDatasRequestTipoData tipoData = _tipoData.ToEnum<ServicoEFrete.FaturamentoTransportadora.FaturamentoTransportadoraObterFaturasPorIntervaloDeDatasRequestTipoData>();


            List<string> matrizes = new List<string>();
            if (contratante != null)
                matrizes.Add(contratante.CNPJ_SemFormato); //configuracao.MatrizEFrete.CNPJ_SemFormato);

            string token = Login(configuracao, UnitOfWork, out mensagemErro);

            ServicoEFrete.FaturamentoTransportadora.FaturamentoTransportadoraObterFaturasPorIntervaloDeDatasRequest requisicao = new ServicoEFrete.FaturamentoTransportadora.FaturamentoTransportadoraObterFaturasPorIntervaloDeDatasRequest()
            {
                Token = token,
                Integrador = configuracao.CodigoIntegradorEFrete,
                Versao = 1,
                TipoData = tipoData,
                DataInicial = dataInicial,
                DataFinal = dataFinal,
                MatrizCNPJ = matrizes.ToArray()
            };

            Servicos.ServicoEFrete.FaturamentoTransportadora.FaturamentoTransportadoraObterFaturasPorIntervaloDeDatasResponse retorno = svcFat.ObterFaturasPorIntervaloDeDatas(requisicao);

            Logout(configuracao, token, UnitOfWork);

            if (!retorno.Sucesso)
            {
                mensagemErro = retorno.Excecao?.Mensagem;
                return null;
            }

            return ConverteRetornoEFreteEmObjetoValor(retorno.Faturas.ToList());
        }

        public void ObterFaturasTransportadores(Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT configuracaoCIOT, DateTime dataInicial, DateTime dataFinal, UnitOfWork unitOfWork)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);
            Repositorio.Embarcador.Documentos.FaturamentoCIOT repFaturamentoCIOT = new Repositorio.Embarcador.Documentos.FaturamentoCIOT(unitOfWork);
            Repositorio.Embarcador.Documentos.FaturamentoCIOTItem repFaturamentoCIOTItem = new Repositorio.Embarcador.Documentos.FaturamentoCIOTItem(unitOfWork);
            Repositorio.Embarcador.Documentos.FaturamentoCIOTOutro repFaturamentoCIOTOutro = new Repositorio.Embarcador.Documentos.FaturamentoCIOTOutro(unitOfWork);
            Repositorio.Embarcador.Documentos.FaturamentoCIOTPagamento repFaturamentoCIOTPagamento = new Repositorio.Embarcador.Documentos.FaturamentoCIOTPagamento(unitOfWork);

            Repositorio.Embarcador.CIOT.ConfiguracaoCIOT repConfiguracaoCIOT = new Repositorio.Embarcador.CIOT.ConfiguracaoCIOT(unitOfWork);


            List<Dominio.Entidades.Empresa> empresas = repEmpresa.BuscarPorCodigoConfiguracaoCIOT(configuracaoCIOT.Codigo);

            for (int i = 0, c = empresas.Count; i < c; i++)
            {
                Dominio.Entidades.Empresa empresa = empresas[i];
                Dominio.Entidades.Embarcador.CIOT.CIOTEFrete configuracao = ObterConfiguracaoEFrete(configuracaoCIOT, unitOfWork);
                List<Dominio.ObjetosDeValor.Embarcador.CIOT.FaturamentoEFrete> faturas = ObterFaturasPorIntervaloDeDatas(configuracao, dataInicial, dataFinal, empresa, "0", unitOfWork, out string erro);

                if (faturas == null)
                {
                    Servicos.Log.TratarErro("Erro: " + erro + "\nEmpresa: " + empresa.CNPJ_SemFormato, "FaturaCIOT");
                    continue;
                }

                // Itera faturas
                for (int j = 0, d = faturas.Count; j < d; j++)
                {
                    Dominio.ObjetosDeValor.Embarcador.CIOT.FaturamentoEFrete fatura = faturas[j];
                    Dominio.Entidades.Embarcador.Documentos.FaturamentoCIOT faturaCIOT = repFaturamentoCIOT.BuscarPorNumero(fatura.Numero);

                    // Verifica se numero já não esta na base
                    if (faturaCIOT == null)
                    {
                        unitOfWork.Start();

                        // Insere fatura
                        faturaCIOT = new Dominio.Entidades.Embarcador.Documentos.FaturamentoCIOT()
                        {
                            Fechamento = fatura.Fechamento,
                            Vencimento = fatura.Vencimento,
                            Transportadora = repEmpresa.BuscarPorCNPJ(fatura.Transportadora),
                            Numero = fatura.Numero,
                            Taxa = fatura.Taxa,
                            Tarifa = fatura.Tarifa,
                            Tipo = fatura.Tipo,
                            Status = fatura.Status,
                        };

                        if (faturaCIOT.Transportadora == null)
                        {
                            unitOfWork.Rollback();
                            Servicos.Log.TratarErro("CNPJ não cadastrado: " + fatura.Transportadora + "\nEmpresa: " + empresa.CNPJ_SemFormato, "FaturaCIOT");
                            continue;
                        }

                        repFaturamentoCIOT.Inserir(faturaCIOT);

                        bool encontrouCiot = true;

                        // Insere Itens
                        foreach (var item in fatura.Itens)
                        {
                            Dominio.Entidades.Embarcador.Documentos.FaturamentoCIOTItem faturaItemCIOT = new Dominio.Entidades.Embarcador.Documentos.FaturamentoCIOTItem()
                            {
                                FaturamentoCIOT = faturaCIOT,
                                DataDeclaracao = item.DataDeclaracao,
                                CIOT = repCIOT.BuscarPorNumeroECodigoVerificador(item.CIOT.Split('/')[0], item.CIOT.Split('/')[1]),
                                IdOperacaoCliente = item.IdOperacaoCliente,
                                Adiantamento = item.Adiantamento,
                                Livre = item.Livre,
                                Estadia = item.Estadia,
                                Quitacao = item.Quitacao,
                                Frota = item.Frota,
                                Servico = item.Servico,
                                Id = item.Transacao.Id,
                                Recibo = item.Transacao.Recibo,
                                Data = item.Transacao.Data,
                                DocumentoViagem = item.Transacao.DocumentoViagem,
                                QuantidadeDaMercadoriaNoDesembarque = item.Transacao.QuantidadeDaMercadoriaNoDesembarque,
                                ValorDiferencaDeFrete = item.Transacao.ValorDiferencaDeFrete,
                                ValorQuebraDeFrete = item.Transacao.ValorQuebraDeFrete,
                                Tipo = item.Tipo,
                            };

                            if (faturaItemCIOT.CIOT == null)
                            {
                                unitOfWork.Rollback();
                                Servicos.Log.TratarErro("CIOT não encontrado: " + item.CIOT + "\nEmpresa: " + empresa.CNPJ_SemFormato, "FaturaCIOT");
                                encontrouCiot = false;
                                break;
                            }

                            repFaturamentoCIOTItem.Inserir(faturaItemCIOT);
                        }

                        if (encontrouCiot)
                        {
                            // Insere Outros
                            foreach (var outro in fatura.Outros)
                            {
                                Dominio.Entidades.Embarcador.Documentos.FaturamentoCIOTOutro faturaOutroCIOT = new Dominio.Entidades.Embarcador.Documentos.FaturamentoCIOTOutro()
                                {
                                    FaturamentoCIOT = faturaCIOT,
                                    Data = outro.Data,
                                    Valor = outro.Valor,
                                    Tipo = outro.Tipo,
                                    TipoLancamento = outro.TipoLancamento,
                                    Documento = outro.Documento,
                                    Detalhes = outro.Detalhes
                                };

                                repFaturamentoCIOTOutro.Inserir(faturaOutroCIOT);
                            }

                            // Insere Pagamentos
                            foreach (var pagamento in fatura.Pagamentos)
                            {
                                Dominio.Entidades.Embarcador.Documentos.FaturamentoCIOTPagamento faturaPagamentoCIOT = new Dominio.Entidades.Embarcador.Documentos.FaturamentoCIOTPagamento()
                                {
                                    FaturamentoCIOT = faturaCIOT,
                                    Data = pagamento.Data,
                                    Valor = pagamento.Valor,
                                    Juros = pagamento.Juros,
                                    Multa = pagamento.Multa,
                                };

                                repFaturamentoCIOTPagamento.Inserir(faturaPagamentoCIOT);
                            }

                            unitOfWork.CommitChanges();
                        }
                    }
                }
            }


        }
        #endregion

        #region Métodos Privados

        private bool ProcessarMovimentoFinanceiro(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, decimal valorMovimento, Dominio.Entidades.Embarcador.Fatura.Justificativa justificativa, Dominio.Entidades.Embarcador.CIOT.CIOTEFrete configuracaoEFrete, string token, Repositorio.UnitOfWork unitOfWork, out string mensagemErro)
        {
            mensagemErro = null;
            var inspector = new InspectorBehavior();
            ServicoEFrete.PEF.AdicionarPagamentoRequest requisicao = new ServicoEFrete.PEF.AdicionarPagamentoRequest
            {
                CodigoIdentificacaoOperacao = $"{ciot.Numero}/{ciot.CodigoVerificador}",
                Integrador = configuracaoEFrete.CodigoIntegradorEFrete,
                Pagamentos = ObterPagamentosMovimentacaoFinanceira(ciot, cargaCIOT, valorMovimento, justificativa, configuracaoEFrete),
                Token = token,
                Versao = 3
            };

            ServicoEFrete.PEF.PefServiceSoapClient svcPEF = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<ServicoEFrete.PEF.PefServiceSoapClient, ServicoEFrete.PEF.PefServiceSoap>(TipoWebServiceIntegracao.EFrete_Pef);
            svcPEF.Endpoint.EndpointBehaviors.Add(inspector);
            ServicoEFrete.PEF.AdicionarPagamentoResponse retorno = svcPEF.AdicionarPagamento(requisicao);

            Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo ciotIntegracaoArquivo = new Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo
            {
                ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", unitOfWork),
                ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", unitOfWork),
                Data = DateTime.Now,
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento,
                Mensagem = retorno.Sucesso ? "Registro de pagamento avulso adicional concluída com sucesso." : retorno.Excecao?.Mensagem
            };

            Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo repCIOTIntegracaoArquivo = new Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo(unitOfWork);
            repCIOTIntegracaoArquivo.Inserir(ciotIntegracaoArquivo);
            ciot.ArquivosTransacao.Add(ciotIntegracaoArquivo);

            if (!retorno.Sucesso)
            {
                mensagemErro = retorno.Excecao?.Mensagem;
                return false;
            }

            return true;
        }

        private ServicoEFrete.PEF.TipoPagamento ObterTipoPagamentoEFrete(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoeFrete tipoPagamento)
        {
            switch (tipoPagamento)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoeFrete.TransferenciaBancaria: return ServicoEFrete.PEF.TipoPagamento.TransferenciaBancaria;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoeFrete.eFrete: return ServicoEFrete.PEF.TipoPagamento.eFRETE;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoeFrete.Parceiro: return ServicoEFrete.PEF.TipoPagamento.Parceiro;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoeFrete.Outros: return ServicoEFrete.PEF.TipoPagamento.Outros;
                default: return ServicoEFrete.PEF.TipoPagamento.eFRETE;
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT ObterTipoPagamentoCIOT(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoeFrete tipoPagamento)
        {
            switch (tipoPagamento)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoeFrete.TransferenciaBancaria: return Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT.Transferencia;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoeFrete.eFrete: return Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT.Cartao;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoeFrete.Parceiro: return Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT.Deposito;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoeFrete.Outros: return Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT.SemPgto;
                default: return Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT.SemPgto;
            }
        }

        private bool AdicionarOperacaoTransporteSemViagem(ref Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Dominio.Entidades.Empresa empresa, Dominio.Entidades.Embarcador.CIOT.CIOTEFrete configuracao, string token, List<Dominio.Entidades.Veiculo> veiculosTransportador, Dominio.Entidades.Usuario motorista, Repositorio.UnitOfWork unidadeTrabalho, out string mensagemErro)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            ServicoEFrete.PEF.Motorista motoristaEfrete = ObterMotorista(motorista, out mensagemErro);

            Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas repModalidadeTransportador = new Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas(unidadeTrabalho);
            Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidade = repModalidadeTransportador.BuscarPorPessoa(ciot.Transportador.CPF_CNPJ);

            Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoasTipoPagamentoCIOT repModalidadeTransportadoraPessoasTipoPagamentoCIOT = new Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoasTipoPagamentoCIOT(unidadeTrabalho);
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT? tipoPagamentoCIOT = repModalidadeTransportadoraPessoasTipoPagamentoCIOT.BuscarTipoPagamentoPorOperadora(modalidade.Codigo, OperadoraCIOT.eFrete);

            if (string.IsNullOrEmpty(mensagemErro))
            {
                ServicoEFrete.PEF.AdicionarOperacaoTransporteRequest requisicao = new ServicoEFrete.PEF.AdicionarOperacaoTransporteRequest()
                {
                    TipoViagem = ServicoEFrete.PEF.TipoViagem.TAC_Agregado,
                    Integrador = configuracao.CodigoIntegradorEFrete,
                    Versao = 7,
                    BloquearNaoEquiparado = true,
                    MatrizCNPJ = long.Parse(configuracao.MatrizEFrete.CNPJ),
                    IdOperacaoCliente = ciot.Codigo.ToString(),
                    DataFimViagem = ciot.DataFinalViagem,
                    Contratado = ObterContratado(ciot, unidadeTrabalho),
                    Motorista = motoristaEfrete,
                    Contratante = ObterContratante(configuracao, empresa),
                    Veiculos = ObterVeiculos(ciot, veiculosTransportador, unidadeTrabalho),
                    EntregaDocumentacao = ServicoEFrete.PEF.TipoEntregaDocumentacao.Cliente,
                    CodigoNCMNaturezaCarga = 0,
                    //DestinacaoComercial = true, //Na emissão com TipoViagem Padrão seu preenchimento é obrigatório. Na emissão com TipoViagem TAC_Agregado o campo não deve ser preenchido
                    //AltoDesempenho = false, //Na emissão com TipoViagem Padrão seu preenchimento é obrigatório. Na emissão com TipoViagem TAC_Agregado o campo não deve ser preenchido
                    //FreteRetorno = false, //Na emissão com TipoViagem Padrão seu preenchimento é obrigatório. Na emissão com TipoViagem TAC_Agregado o campo não deve ser preenchido
                    //DistanciaRetorno = 0, //Na emissão com TipoViagem Padrão seu preenchimento é obrigatório. Na emissão com TipoViagem TAC_Agregado o campo não deve ser preenchido
                    //CodigoTipoCarga = 0, //Na emissão com TipoViagem Padrão seu preenchimento é obrigatório. Na emissão com TipoViagem TAC_Agregado o campo não deve ser preenchido
                    Token = token
                };

                if (ciot.Contratante.Configuracao != null && (modalidade == null || tipoPagamentoCIOT != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT.SemPgto))
                    requisicao.EmissaoGratuita = ciot.Contratante.Configuracao.EmissaoGratuitaEFrete;
                else
                    requisicao.TipoPagamento = ServicoEFrete.PEF.TipoPagamento.Outros;

                if (configuracao.TipoPagamento.HasValue)
                    requisicao.TipoPagamento = ObterTipoPagamentoEFrete(configuracao.TipoPagamento.Value);

                if (configuracao.EmissaoGratuita.HasValue)
                    requisicao.EmissaoGratuita = configuracao.EmissaoGratuita;

                //Removido pois na Frimesa a eFrete passou que não pode enviar este CNPJ, portanto se precisar deve ser coonfigurado na Matriz das configurações da EFrete
                //if (empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao)
                //    requisicao.MatrizCNPJ = 45543915082137;

                ServicoEFrete.PEF.PefServiceSoapClient svcPEF = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unidadeTrabalho).ObterClient<ServicoEFrete.PEF.PefServiceSoapClient, ServicoEFrete.PEF.PefServiceSoap>(TipoWebServiceIntegracao.EFrete_Pef);

                ServicoEFrete.PEF.AdicionarOperacaoTransporteResponse retorno = svcPEF.AdicionarOperacaoTransporte(requisicao);

                if (retorno.Sucesso)
                {
                    ciot.Numero = Utilidades.String.Left(retorno.CodigoIdentificacaoOperacao, 12);
                    ciot.CodigoVerificador = Utilidades.String.Right(retorno.CodigoIdentificacaoOperacao, 4);
                    ciot.Mensagem = "Operação de transporte adicionada com sucesso.";
                    ciot.DataAbertura = DateTime.Now;
                    mensagemErro = null;
                }
                else
                {
                    mensagemErro = retorno.Excecao?.Mensagem;
                }

                return retorno.Sucesso;
            }
            else
            {
                return false;
            }
        }

        private ServicoEFrete.Veiculo.GravarRequest ObterRequisicaoVeiculo(Dominio.Entidades.Veiculo veiculo, Dominio.Entidades.Localidade localidade, Dominio.Entidades.Embarcador.CIOT.CIOTEFrete configuracao, string token, bool ciotTestes = false)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicoEFrete.Veiculo.GravarRequest requisicao = new ServicoEFrete.Veiculo.GravarRequest()
            {
                Integrador = configuracao.CodigoIntegradorEFrete,
                Versao = 1,
                Token = token,
                Veiculo = new ServicoEFrete.Veiculo.Veiculo()
                {
                    AnoFabricacao = veiculo.AnoFabricacao,
                    AnoModelo = veiculo.AnoModelo,
                    CapacidadeKg = veiculo.CapacidadeKG,
                    CapacidadeM3 = veiculo.CapacidadeM3,
                    Chassi = !ciotTestes ? veiculo.Chassi : "9BSTH4X2Z03216533",
                    CodigoMunicipio = localidade.CodigoIBGE,
                    Cor = "Não disponível",
                    Marca = veiculo.Marca?.Descricao,
                    Modelo = veiculo.Modelo?.Descricao,
                    NumeroDeEixos = veiculo.TipoDoVeiculo?.NumeroEixos,
                    Placa = !ciotTestes ? veiculo.Placa : "AJI9860",
                    Renavam = !ciotTestes ? long.Parse(veiculo.Renavam) : 739180479,
                    RNTRC = !ciotTestes ? (veiculo.Proprietario != null ? veiculo.RNTRC : !string.IsNullOrWhiteSpace(veiculo.Empresa?.RegistroANTT ?? "") ? int.Parse(veiculo.Empresa?.RegistroANTT) : veiculo.RNTRC) : 11029129,
                    Tara = veiculo.Tara,
                    TipoCarroceria = ObterTipoCarroceria(veiculo.TipoCarroceria),
                    TipoRodado = ObterTipoRodado(veiculo.TipoRodado)
                }
            };

            return requisicao;
        }

        private ServicoEFrete.Veiculo.TipoCarroceria ObterTipoCarroceria(string tipoCarroceria)
        {
            switch (tipoCarroceria)
            {
                case "00":
                    return ServicoEFrete.Veiculo.TipoCarroceria.NaoAplicavel;
                case "01":
                    return ServicoEFrete.Veiculo.TipoCarroceria.Aberta;
                case "02":
                    return ServicoEFrete.Veiculo.TipoCarroceria.FechadaOuBau;
                case "03":
                    return ServicoEFrete.Veiculo.TipoCarroceria.Granelera;
                case "04":
                    return ServicoEFrete.Veiculo.TipoCarroceria.PortaContainer;
                case "05":
                    return ServicoEFrete.Veiculo.TipoCarroceria.Sider;
                default:
                    return ServicoEFrete.Veiculo.TipoCarroceria.NaoAplicavel;
            }
        }

        private ServicoEFrete.Veiculo.TipoRodado ObterTipoRodado(string tipoRodado)
        {
            switch (tipoRodado)
            {
                case "00":
                    return ServicoEFrete.Veiculo.TipoRodado.NaoAplicavel;
                case "01":
                    return ServicoEFrete.Veiculo.TipoRodado.Truck;
                case "02":
                    return ServicoEFrete.Veiculo.TipoRodado.Toco;
                case "03":
                    return ServicoEFrete.Veiculo.TipoRodado.Cavalo;
                default:
                    return ServicoEFrete.Veiculo.TipoRodado.NaoAplicavel;
            }
        }

        private ServicoEFrete.PEF.Contratante ObterContratante(Dominio.Entidades.Embarcador.CIOT.CIOTEFrete configuracao, Dominio.Entidades.Empresa empresa)
        {
            ServicoEFrete.PEF.Contratante contratante = new ServicoEFrete.PEF.Contratante()
            {
                CpfOuCnpj = long.Parse(configuracao.MatrizEFrete.CNPJ),
                Endereco = new ServicoEFrete.PEF.Endereco()
                {
                    Bairro = configuracao.MatrizEFrete.Bairro,
                    CEP = configuracao.MatrizEFrete.CEP,
                    CodigoMunicipio = configuracao.MatrizEFrete.Localidade.CodigoIBGE,
                    Complemento = configuracao.MatrizEFrete.Complemento,
                    Numero = configuracao.MatrizEFrete.Numero,
                    Rua = configuracao.MatrizEFrete.Endereco
                },
                NomeOuRazaoSocial = configuracao.MatrizEFrete.RazaoSocial,
                ResponsavelPeloPagamento = true,
                RNTRC = int.Parse(configuracao.MatrizEFrete.RegistroANTT)
            };

            //if (empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao)
            //{
            //    contratante.CpfOuCnpj = 45543915082137;
            //    contratante.Endereco = new ServicoEFrete.PEF.Endereco()
            //    {
            //        Bairro = "SANTA FE",
            //        CEP = "06278-010",
            //        CodigoMunicipio = 3534401,
            //        Complemento = "ANDAR 2 SALA 2",
            //        Numero = "322",
            //        Rua = "AVENIDA DOUTOR MAURO LINDEMBERG MONTEIRO"
            //    };
            //    contratante.NomeOuRazaoSocial = "CARREFOUR COMERCIO E INDUSTRIA LTDA";
            //}

            return contratante;
        }

        private ServicoEFrete.PEF.Destinatario ObterDestinatario(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, List<Dominio.Entidades.Embarcador.Cargas.CargaCIOT> cargaCIOTs)
        {

            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = cargaCIOTs.FirstOrDefault().Carga.Pedidos.FirstOrDefault();

            Dominio.Entidades.Cliente cliente = cargaPedido.Pedido.Recebedor != null ? cargaPedido.Pedido.Recebedor : cargaPedido.Pedido.Destinatario;

            ServicoEFrete.PEF.Destinatario destinatario = new ServicoEFrete.PEF.Destinatario()
            {
                CpfOuCnpj = long.Parse(cliente.CPF_CNPJ_SemFormato),
                NomeOuRazaoSocial = cliente.Nome,
                ResponsavelPeloPagamento = false,
                Endereco = new ServicoEFrete.PEF.Endereco()
                {
                    Bairro = cliente.Bairro,
                    CEP = cliente.CEP,
                    CodigoMunicipio = cliente.Localidade.CodigoIBGE,
                    Complemento = cliente.Complemento,
                    Numero = cliente.Numero,
                    Rua = cliente.Endereco
                }
            };

            return destinatario;
        }

        private ServicoEFrete.PEF.Motorista ObterMotorista(Dominio.Entidades.Usuario motorista, out string mensagem)
        {
            mensagem = "";
            if (motorista.Celular?.Split(' ').Length > 1)
            {
                ServicoEFrete.PEF.Motorista mot = new ServicoEFrete.PEF.Motorista()
                {
                    Celular = new ServicoEFrete.PEF.Telefone()
                    {
                        DDD = int.Parse(Utilidades.String.OnlyNumbers(motorista.Celular?.Split(' ')[0])),
                        Numero = int.Parse(Utilidades.String.OnlyNumbers(motorista.Celular?.Split(' ')[1]))
                    },
                    CNH = long.Parse(motorista.NumeroHabilitacao),
                    CpfOuCnpj = long.Parse(motorista.CPF)
                };

                return mot;
            }
            else
            {
                mensagem = "É obrigatório que o motorista tenha um telefone com DDD cadastrado";
                return null;
            }
        }

        private ServicoEFrete.PEF.Contratado ObterContratado(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas repModalidadeTransportador = new Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidade = repModalidadeTransportador.BuscarPorPessoa(ciot.Transportador.CPF_CNPJ);

            ServicoEFrete.Proprietario.ProprietariosServiceSoapClient svcProprietario = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unidadeTrabalho).ObterClient<ServicoEFrete.Proprietario.ProprietariosServiceSoapClient, ServicoEFrete.Proprietario.ProprietariosServiceSoap>(TipoWebServiceIntegracao.EFrete_Proprietarios);

            bool ciotTestes = (!ciot.CargaCIOT?.FirstOrDefault().Carga.FreteDeTerceiro ?? true) && ciot.CargaCIOT?.FirstOrDefault().Carga.Empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao;

            if (ciotTestes)
            {
                ServicoEFrete.PEF.Contratado contratado = new ServicoEFrete.PEF.Contratado()
                {
                    CpfOuCnpj = 07201103000169,
                    RNTRC = 11029129
                };

                return contratado;
            }
            else
            {
                int rntrc = 0;
                int.TryParse(modalidade.RNTRC, out rntrc);

                ServicoEFrete.PEF.Contratado contratado = new ServicoEFrete.PEF.Contratado()
                {
                    CpfOuCnpj = (long)ciot.Transportador.CPF_CNPJ,
                    RNTRC = rntrc
                };

                return contratado;
            }
        }

        private ServicoEFrete.PEF.Pagamento2[] ObterPagamentosTAC(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, List<Dominio.Entidades.Embarcador.Cargas.CargaCIOT> cargaCIOTs, Dominio.Entidades.Embarcador.CIOT.CIOTEFrete configuracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            List<ServicoEFrete.PEF.Pagamento2> pagamentos = new List<ServicoEFrete.PEF.Pagamento2>();

            if (ciot.CTes.Count > 0)
            {
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT in cargaCIOTs)
                {
                    decimal valorAdiantamento = tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? cargaCIOT.ContratoFrete.ValorAdiantamento : cargaCIOT.ValorAdiantamento;
                    decimal valorQuitacao = tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? cargaCIOT.ContratoFrete.SaldoAReceber : cargaCIOT.ValorFrete - cargaCIOT.ValorAdiantamento;

                    if (valorAdiantamento > 0)
                    {
                        ServicoEFrete.PEF.Pagamento2 pagamentoAdiantamento = new ServicoEFrete.PEF.Pagamento2()
                        {
                            Categoria = ServicoEFrete.PEF.CategoriaPagamento.SemCategoria,
                            DataDeLiberacao = ciot.DataAbertura.Value,
                            Documento = cargaCIOT.Carga.CodigoCargaEmbarcador,
                            IdPagamentoCliente = $"{cargaCIOT.Codigo}_1",
                            InformacoesBancarias = configuracao.TipoPagamento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoeFrete.eFrete ? null : this.ObterInformacoesBancarias(ciot),
                            TipoPagamento = configuracao.TipoPagamento.HasValue ? ObterTipoPagamentoEFrete(configuracao.TipoPagamento.Value) : ServicoEFrete.PEF.TipoPagamento.TransferenciaBancaria,
                            Valor = valorAdiantamento
                        };

                        pagamentos.Add(pagamentoAdiantamento);
                    }

                    pagamentos.Add(new ServicoEFrete.PEF.Pagamento2()
                    {
                        Categoria = ServicoEFrete.PEF.CategoriaPagamento.SemCategoria,
                        DataDeLiberacao = ciot.DataFinalViagem,
                        Documento = cargaCIOT.Carga.CodigoCargaEmbarcador,
                        IdPagamentoCliente = $"{cargaCIOT.Codigo}_2",
                        InformacoesBancarias = configuracao.TipoPagamento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoeFrete.eFrete ? null : this.ObterInformacoesBancarias(ciot),
                        TipoPagamento = configuracao.TipoPagamento.HasValue ? ObterTipoPagamentoEFrete(configuracao.TipoPagamento.Value) : ServicoEFrete.PEF.TipoPagamento.TransferenciaBancaria,
                        Valor = valorQuitacao
                    });
                }
            }

            return pagamentos.ToArray();
        }

        private ServicoEFrete.PEF.Pagamento3[] ObterPagamentosMovimentacaoFinanceira(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, decimal valorMovimento, Dominio.Entidades.Embarcador.Fatura.Justificativa justificativa, Dominio.Entidades.Embarcador.CIOT.CIOTEFrete configuracao)
        {
            List<ServicoEFrete.PEF.Pagamento3> pagamentos = new List<ServicoEFrete.PEF.Pagamento3>();

            pagamentos.Add(new ServicoEFrete.PEF.Pagamento3()
            {
                Categoria = ServicoEFrete.PEF.CategoriaPagamento.SemCategoria,
                DataDeLiberacao = ciot.DataFinalViagem,
                Documento = cargaCIOT.Carga.CodigoCargaEmbarcador,
                IdPagamentoCliente = $"{cargaCIOT.Codigo}_3",
                InformacoesBancarias = configuracao.TipoPagamento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoeFrete.eFrete ? null : this.ObterInformacoesBancarias(ciot),
                TipoPagamento = configuracao.TipoPagamento.HasValue ? ObterTipoPagamentoEFrete(configuracao.TipoPagamento.Value) : ServicoEFrete.PEF.TipoPagamento.TransferenciaBancaria,
                InformacaoAdicional = justificativa.Descricao,
                Valor = valorMovimento
            });


            return pagamentos.ToArray();
        }

        private ServicoEFrete.PEF.Impostos ObterImpostos(Dominio.Entidades.Embarcador.Documentos.CIOT ciot)
        {
            ServicoEFrete.PEF.Impostos impostos = new ServicoEFrete.PEF.Impostos()
            {
                INSS = 0m,
                IRRF = 0m,
                SestSenat = 0m
            };

            return impostos;
        }

        private ServicoEFrete.PEF.InformacoesBancarias ObterInformacoesBancarias(Dominio.Entidades.Embarcador.Documentos.CIOT ciot)
        {
            ServicoEFrete.PEF.InformacoesBancarias informacoesBancarias = new ServicoEFrete.PEF.InformacoesBancarias();

            informacoesBancarias.Agencia = ciot.Transportador.Agencia + "-" + ciot.Transportador.DigitoAgencia;
            informacoesBancarias.Conta = ciot.Transportador.NumeroConta;
            informacoesBancarias.InstituicaoBancaria = ciot.Transportador.Banco != null ? string.Format("{0:000}", ciot.Transportador.Banco.Numero) : string.Empty;
            informacoesBancarias.TipoConta = ciot.Transportador.TipoContaBanco == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco.Corrente ? ServicoEFrete.PEF.TipoConta.ContaCorrente : ServicoEFrete.PEF.TipoConta.ContaPoupanca;

            return informacoesBancarias;
        }

        private ServicoEFrete.PEF.Veiculo[] ObterVeiculos(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, List<Dominio.Entidades.Veiculo> veiculosTransportador, Repositorio.UnitOfWork unidadeTrabalho)
        {
            List<ServicoEFrete.PEF.Veiculo> veiculos = new List<ServicoEFrete.PEF.Veiculo>();

            bool ciotTestes = (!ciot.CargaCIOT?.FirstOrDefault().Carga.FreteDeTerceiro ?? true) && ciot.CargaCIOT?.FirstOrDefault().Carga.Empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao;

            if (ciotTestes)
            {
                veiculos.Add(new ServicoEFrete.PEF.Veiculo() { Placa = "AJI9860" });
            }
            else
            {
                foreach (Dominio.Entidades.Veiculo veiculo in veiculosTransportador)
                {
                    veiculos.Add(new ServicoEFrete.PEF.Veiculo() { Placa = veiculo.Placa });
                    //veiculos.AddRange(from obj in veiculo.VeiculosVinculados select new ServicoEFrete.PEF.Veiculo() { Placa = obj.Placa });
                }
            }

            return veiculos.ToArray();
        }

        private static string Login(Dominio.Entidades.Embarcador.CIOT.CIOTEFrete configuracao, UnitOfWork unidadeTrabalho, out string mensagemErro)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            ServicoEFrete.Logon.LogonServiceSoapClient svcLogon = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unidadeTrabalho).ObterClient<ServicoEFrete.Logon.LogonServiceSoapClient, ServicoEFrete.Logon.LogonServiceSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.EFrete_Logon);

            if (configuracao == null)
            {
                mensagemErro = "Não foi encontrado nenhuma configuração de CIOT";
                return null;
            }

            ServicoEFrete.Logon.LoginRequest request = new ServicoEFrete.Logon.LoginRequest()
            {
                Integrador = configuracao.CodigoIntegradorEFrete,
                Senha = configuracao.SenhaEFrete,
                Usuario = configuracao.UsuarioEFrete,
                Versao = 1
            };

            ServicoEFrete.Logon.LoginResponse retorno = svcLogon.Login(request);

            if (!retorno.Sucesso)
            {
                mensagemErro = retorno.Excecao?.Mensagem;
                return null;
            }

            if (retorno.Token != null)
            {
                mensagemErro = null;
                return retorno.Token;
            }

            mensagemErro = "O login no e-Frete não retornou token de acesso. " + (retorno.Excecao?.Mensagem ?? string.Empty);
            return null;
        }

        private static void Logout(Dominio.Entidades.Embarcador.CIOT.CIOTEFrete configuracao, string token, Repositorio.UnitOfWork unitOfWork)
        {
            ServicoEFrete.Logon.LogonServiceSoapClient svcLogon = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<ServicoEFrete.Logon.LogonServiceSoapClient, ServicoEFrete.Logon.LogonServiceSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.EFrete_Logon);

            ServicoEFrete.Logon.LogoutRequest request = new ServicoEFrete.Logon.LogoutRequest()
            {
                Integrador = configuracao.CodigoIntegradorEFrete,
                Token = token,
                Versao = 1
            };

            svcLogon.Logout(request);
        }

        private static List<FaturamentoEFrete> ConverteRetornoEFreteEmObjetoValor(List<FaturamentoTransportadoraFatura> faturas)
        {
            return (from o in faturas
                    select new FaturamentoEFrete
                    {
                        Fechamento = o.Fechamento,
                        Vencimento = o.Vencimento,
                        Transportadora = o.Transportadora,
                        Numero = o.Numero,
                        Taxa = o.Taxa,
                        Tarifa = o.Tarifa,
                        Tipo = ConverteFaturamentoEFreteTipo(o.Tipo),
                        Status = ConverteFaturamentoTransportadoraTipoStatus(o.Status),
                        Itens = (from i in o.Itens
                                 select new FaturamentoEFreteItem
                                 {
                                     DataDeclaracao = i.DataDeclaracao,
                                     CIOT = i.CIOT,
                                     IdOperacaoCliente = i.IdOperacaoCliente,
                                     Adiantamento = i.Adiantamento,
                                     Livre = i.Livre,
                                     Estadia = i.Estadia,
                                     Quitacao = i.Quitacao,
                                     Frota = i.Frota,
                                     Servico = i.Servico,
                                     Tipo = ConverteFaturamentoEFreteTipo(i.Tipo),
                                     Transacao = new FaturamentoEFreteItemTransacao
                                     {
                                         Id = i.Transacao.Id,
                                         Recibo = i.Transacao.Recibo,
                                         Data = i.Transacao.Data,
                                         DocumentoViagem = i.Transacao.DocumentoViagem,
                                         QuantidadeDaMercadoriaNoDesembarque = i.Transacao.QuantidadeDaMercadoriaNoDesembarque,
                                         ValorDiferencaDeFrete = i.Transacao.ValorDiferencaDeFrete,
                                         ValorQuebraDeFrete = i.Transacao.ValorQuebraDeFrete,
                                     }
                                 }).ToList(),

                        Outros = (from i in o.Outros
                                  select new FaturamentoEFreteOutro
                                  {
                                      Data = i.Data,
                                      Valor = i.Valor,
                                      Tipo = i.Tipo,
                                      TipoLancamento = i.TipoLancamento,
                                      Documento = i.Documento,
                                      Detalhes = i.Detalhes
                                  }).ToList(),

                        Pagamentos = (from i in o.Pagamentos
                                      select new FaturamentoEFretePagamento
                                      {
                                          Data = i.Data,
                                          Valor = i.Valor,
                                          Juros = i.Juros,
                                          Multa = i.Multa,
                                      }).ToList(),
                    }).ToList();
        }

        private static Dominio.ObjetosDeValor.Embarcador.Enumeradores.FaturamentoEFreteItemTipo ConverteFaturamentoEFreteTipo(FaturamentoTransportadoraFaturaItemTipo tipo)
        {
            switch (tipo)
            {
                case FaturamentoTransportadoraFaturaItemTipo.PosPago:
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.FaturamentoEFreteItemTipo.PosPago;
                default:
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.FaturamentoEFreteItemTipo.PrePago;
            }
        }

        private static Dominio.ObjetosDeValor.Embarcador.Enumeradores.FaturamentoEFreteTipo ConverteFaturamentoEFreteTipo(FaturamentoTransportadoraTipoFatura tipo)
        {
            switch (tipo)
            {
                case FaturamentoTransportadoraTipoFatura.Adiantamentos:
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.FaturamentoEFreteTipo.Adiantamentos;
                case FaturamentoTransportadoraTipoFatura.Frota:
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.FaturamentoEFreteTipo.Frota;
                case FaturamentoTransportadoraTipoFatura.PrePago:
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.FaturamentoEFreteTipo.PrePago;
                default:
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.FaturamentoEFreteTipo.Quitacao;
            }
        }

        private static Dominio.ObjetosDeValor.Embarcador.Enumeradores.FaturamentoEFreteStatus ConverteFaturamentoTransportadoraTipoStatus(FaturamentoTransportadoraTipoStatus tipo)
        {
            switch (tipo)
            {
                case FaturamentoTransportadoraTipoStatus.Aberto:
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.FaturamentoEFreteStatus.Aberto;
                case FaturamentoTransportadoraTipoStatus.Cancelado:
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.FaturamentoEFreteStatus.Cancelado;
                default:
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.FaturamentoEFreteStatus.Encerrado;
            }
        }
        #endregion
    }
}
