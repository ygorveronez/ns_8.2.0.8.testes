using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Servicos
{
    public class EFrete
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public EFrete(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Globais 

        public void EmitirCIOT(int codigoCIOT)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Repositorio.CIOTSigaFacil repCIOT = new Repositorio.CIOTSigaFacil(_unitOfWork);

            Dominio.Entidades.CIOTSigaFacil ciot = repCIOT.BuscarPorCodigo(codigoCIOT);

            string token = this.Login(ciot);

            if (!string.IsNullOrWhiteSpace(token) &&
                IntegrarMotorista(ref ciot, token) &&
                IntegrarProprietario(ref ciot, token) &&
                IntegrarVeiculos(ref ciot, token))
            {
                IntegrarOperacaoTransporte(ref ciot, token);
            }

            repCIOT.Atualizar(ciot);

            this.Logout(ciot, token);
        }

        public void CancelarCIOT(int codigoCIOT)
        {
            Repositorio.CIOTSigaFacil repCIOT = new Repositorio.CIOTSigaFacil(_unitOfWork);

            Dominio.Entidades.CIOTSigaFacil ciot = repCIOT.BuscarPorCodigo(codigoCIOT);

            string token = Login(ciot);

            ServicoEFrete.PEF.CancelarOperacaoTransporteRequest requisicao = new ServicoEFrete.PEF.CancelarOperacaoTransporteRequest()
            {
                CodigoIdentificacaoOperacao = ciot.NumeroCIOT + "/" + ciot.CodigoVerificadorCIOT,
                Integrador = ciot.Empresa.Configuracao.CodigoIntegradorEFrete,
                Motivo = ciot.MotivoCancelamento,
                Token = token,
                Versao = 1
            };

            ServicoEFrete.PEF.PefServiceSoapClient svcPEF = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(_unitOfWork).ObterClient<ServicoEFrete.PEF.PefServiceSoapClient, ServicoEFrete.PEF.PefServiceSoap>(TipoWebServiceIntegracao.EFrete_Pef, out Servicos.Models.Integracao.InspectorBehavior inspector);

            ServicoEFrete.PEF.CancelarOperacaoTransporteResponse retorno = svcPEF.CancelarOperacaoTransporte(requisicao);
            AdicionaLog(inspector, ciot);

            if (retorno.Sucesso)
            {
                ciot.Status = Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Cancelado;
                ciot.CodigoRetornoCancelamento = "0";
                ciot.MensagemRetornoCancelamento = "Operação realizada com sucesso.";
            }
            else
            {
                ciot.CodigoRetornoCancelamento = "9";
                ciot.MensagemRetornoCancelamento = retorno.Excecao?.Mensagem;
            }

            repCIOT.Atualizar(ciot);

            Logout(ciot, token);
        }

        public void EncerrarCIOT(int codigoCIOT)
        {
            Repositorio.CIOTSigaFacil repCIOT = new Repositorio.CIOTSigaFacil(_unitOfWork);
            Repositorio.CTeCIOTSigaFacil repCTeCIOT = new Repositorio.CTeCIOTSigaFacil(_unitOfWork);

            Dominio.Entidades.CIOTSigaFacil ciot = repCIOT.BuscarPorCodigo(codigoCIOT);
            List<Dominio.Entidades.CTeCIOTSigaFacil> ctes = repCTeCIOT.BuscarPorCIOT(ciot.Codigo);

            string token = Login(ciot);

            ServicoEFrete.PEF.EncerrarOperacaoTransporteRequest requisicao = new ServicoEFrete.PEF.EncerrarOperacaoTransporteRequest()
            {
                CodigoIdentificacaoOperacao = ciot.NumeroCIOT + "/" + ciot.CodigoVerificadorCIOT,
                Integrador = ciot.Empresa.Configuracao.CodigoIntegradorEFrete,
                Versao = 2,
                Impostos = ObterImpostos(ciot, ctes),
                PesoCarga = (from obj in ctes select obj.PesoBruto).Sum(),
                Token = token
            };

            ServicoEFrete.PEF.PefServiceSoapClient svcPEF = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(_unitOfWork).ObterClient<ServicoEFrete.PEF.PefServiceSoapClient, ServicoEFrete.PEF.PefServiceSoap>(TipoWebServiceIntegracao.EFrete_Pef, out Servicos.Models.Integracao.InspectorBehavior inspector);

            ServicoEFrete.PEF.EncerrarOperacaoTransporteResponse retorno = svcPEF.EncerrarOperacaoTransporte(requisicao);

            AdicionaLog(inspector, ciot);

            if (retorno.Sucesso)
            {
                ciot.Status = Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Encerrado;
                ciot.CodigoRetornoCancelamento = "0";
                ciot.MensagemRetornoCancelamento = "Operação realizada com sucesso.";
            }
            else
            {
                ciot.CodigoRetornoCancelamento = "9";
                ciot.MensagemRetornoCancelamento = retorno.Excecao?.Mensagem;
            }

            repCIOT.Atualizar(ciot);
        }

        public bool AbrirCIOT(int codigoCIOT)
        {
            try
            {
                System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

                Repositorio.CIOTSigaFacil repCIOT = new Repositorio.CIOTSigaFacil(_unitOfWork);

                Dominio.Entidades.CIOTSigaFacil ciot = repCIOT.BuscarPorCodigo(codigoCIOT);

                string token = this.Login(ciot);

                if (!string.IsNullOrWhiteSpace(token) && IntegrarMotorista(ref ciot, token) && IntegrarProprietario(ref ciot, token) && IntegrarVeiculos(ref ciot, token))
                {
                    if (!AdicionarOperacaoTransporteAbertura(ref ciot, token))
                        return false;
                }
                else return false;

                repCIOT.Atualizar(ciot);

                this.Logout(ciot, token);

                return true;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "eFrete");
                return false;
            }
        }

        public bool EncerrarCIOTAberto(int codigoCIOT)
        {
            try
            {
                Repositorio.CIOTSigaFacil repCIOT = new Repositorio.CIOTSigaFacil(_unitOfWork);
                Repositorio.CTeCIOTSigaFacil repCTeCIOT = new Repositorio.CTeCIOTSigaFacil(_unitOfWork);

                Dominio.Entidades.CIOTSigaFacil ciot = repCIOT.BuscarPorCodigo(codigoCIOT);
                List<Dominio.Entidades.CTeCIOTSigaFacil> ctes = repCTeCIOT.BuscarPorCIOT(ciot.Codigo);

                string token = Login(ciot);

                ServicoEFrete.PEF.AdicionarViagemRequest requisicao = new ServicoEFrete.PEF.AdicionarViagemRequest()
                {
                    CodigoIdentificacaoOperacao = ciot.NumeroCIOT + "/" + ciot.CodigoVerificadorCIOT,
                    Integrador = ciot.Empresa.Configuracao.CodigoIntegradorEFrete,
                    NaoAdicionarParcialmente = false,
                    Pagamentos = ObterPagamentosEncerramento(ciot, ctes),
                    Token = token,
                    Versao = 3,
                    Viagens = ObterViagensEncerramento(ciot, ctes)
                };

                ServicoEFrete.PEF.PefServiceSoapClient svcPEF = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(_unitOfWork).ObterClient<ServicoEFrete.PEF.PefServiceSoapClient, ServicoEFrete.PEF.PefServiceSoap>(TipoWebServiceIntegracao.EFrete_Pef, out Servicos.Models.Integracao.InspectorBehavior inspector);

                ServicoEFrete.PEF.AdicionarViagemResponse retorno = svcPEF.AdicionarViagem(requisicao);
                AdicionaLog(inspector, ciot);

                if (retorno.Sucesso || (retorno.Excecao != null && retorno.Excecao.Mensagem.Contains("Todas as viagens já estavam cadastradas")))
                {
                    ServicoEFrete.PEF.EncerrarOperacaoTransporteRequest requisicaoEncerramento = new ServicoEFrete.PEF.EncerrarOperacaoTransporteRequest()
                    {
                        CodigoIdentificacaoOperacao = ciot.NumeroCIOT + "/" + ciot.CodigoVerificadorCIOT,
                        Integrador = ciot.Empresa.Configuracao.CodigoIntegradorEFrete,
                        Versao = 2,
                        Impostos = ObterImpostosEncerramento(ciot),
                        PesoCarga = (from obj in ctes select obj.PesoBruto).Sum(),
                        Token = token
                    };

                    ServicoEFrete.PEF.EncerrarOperacaoTransporteResponse retornoEncerramento = svcPEF.EncerrarOperacaoTransporte(requisicaoEncerramento);

                    if (retornoEncerramento.Sucesso)
                    {
                        ciot.Status = Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Encerrado;
                        ciot.CodigoRetornoCancelamento = "0";
                        ciot.MensagemRetornoCancelamento = "Operação realizada com sucesso.";

                        repCIOT.Atualizar(ciot);
                        return true;
                    }
                    else
                    {
                        ciot.CodigoRetornoCancelamento = "9";
                        ciot.MensagemRetornoCancelamento = retornoEncerramento.Excecao?.Mensagem;
                        ciot.Status = Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Rejeitado_Evento;

                        repCIOT.Atualizar(ciot);
                        return false;
                    }
                }
                else
                {
                    ciot.CodigoRetornoCancelamento = "9";
                    ciot.MensagemRetornoCancelamento = retorno.Excecao?.Mensagem;
                    ciot.Status = Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Rejeitado_Evento;

                    repCIOT.Atualizar(ciot);
                    return false;
                }


            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "eFrete");
                return false;
            }
        }

        public bool IntegrarMotorista(ref Dominio.Entidades.CIOTSigaFacil ciot, string token)
        {
            ServicoEFrete.Motorista.MotoristasServiceSoapClient svcMotorista = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(_unitOfWork).ObterClient<ServicoEFrete.Motorista.MotoristasServiceSoapClient, ServicoEFrete.Motorista.MotoristasServiceSoap>(TipoWebServiceIntegracao.EFrete_Motoristas, out Servicos.Models.Integracao.InspectorBehavior inspector);

            ServicoEFrete.Motorista.GravarRequest requisicao = new ServicoEFrete.Motorista.GravarRequest()
            {
                CNH = long.Parse(ciot.Motorista.NumeroHabilitacao),
                CPF = long.Parse(ciot.Motorista.CPF),
                DataNascimento = ciot.Motorista.DataNascimento,
                Endereco = new ServicoEFrete.Motorista.Endereco()
                {
                    Bairro = ciot.Motorista.Bairro,
                    CEP = ciot.Motorista.CEP,
                    CodigoMunicipio = ciot.Motorista.Localidade.CodigoIBGE,
                    Complemento = ciot.Motorista.Complemento,
                    Numero = "S/N",
                    Rua = ciot.Motorista.Endereco
                },
                Integrador = ciot.Empresa.Configuracao.CodigoIntegradorEFrete,
                Nome = ciot.Motorista.Nome,
                Telefones = new ServicoEFrete.Motorista.Telefones()
                {
                    Celular = new ServicoEFrete.Motorista.Telefone()
                    {
                        DDD = this.PegaDDD(ciot.Motorista.Telefone),
                        Numero = this.PegaNumero(ciot.Motorista.Telefone)
                    }
                },
                Token = token,
                Versao = 2
            };

            ServicoEFrete.Motorista.GravarResponse retorno = svcMotorista.Gravar(requisicao);

            if (!retorno.Sucesso)
            {
                ciot.Status = Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Rejeitado;
                ciot.CodigoRetorno = "9";
                ciot.MensagemRetorno = retorno.Excecao?.Mensagem;
            }

            return retorno.Sucesso;
        }

        public bool IntegrarProprietario(ref Dominio.Entidades.CIOTSigaFacil ciot, string token)
        {
            ServicoEFrete.Proprietario.ProprietariosServiceSoapClient svcProprietario = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(_unitOfWork).ObterClient<ServicoEFrete.Proprietario.ProprietariosServiceSoapClient, ServicoEFrete.Proprietario.ProprietariosServiceSoap>(TipoWebServiceIntegracao.EFrete_Proprietarios, out Servicos.Models.Integracao.InspectorBehavior inspector);

            ServicoEFrete.Proprietario.GravarRequest requisicao = new ServicoEFrete.Proprietario.GravarRequest()
            {
                CNPJ = (long)ciot.Transportador.CPF_CNPJ,
                Endereco = new ServicoEFrete.Proprietario.Endereco()
                {
                    Bairro = ciot.Transportador.Bairro,
                    CEP = ciot.Transportador.CEP,
                    CodigoMunicipio = ciot.Transportador.Localidade.CodigoIBGE,
                    Complemento = ciot.Transportador.Complemento,
                    Numero = ciot.Transportador.Numero,
                    Rua = ciot.Transportador.Endereco
                },
                Integrador = ciot.Empresa.Configuracao.CodigoIntegradorEFrete,
                RazaoSocial = ciot.Transportador.Nome,
                RNTRC = ciot.Veiculo.Tipo == "T" ? ciot.Veiculo.RNTRC : int.Parse(ciot.Empresa.RegistroANTT),
                //Telefones = new ServicoEFrete.Proprietario.Telefones()
                //{
                //    Celular = new ServicoEFrete.Proprietario.Telefone()
                //    {
                //        DDD = int.Parse(Utilidades.String.OnlyNumbers(ciot.Transportador.Telefone1?.Split(' ')[0])),
                //        Numero = int.Parse(Utilidades.String.OnlyNumbers(ciot.Transportador.Telefone1?.Split(' ')[1]))
                //    }
                //},
                Token = token,
                TipoPessoa = ciot.Transportador.Tipo == "F" ? ServicoEFrete.Proprietario.TipoPessoa.Fisica : ServicoEFrete.Proprietario.TipoPessoa.Juridica,
                Versao = 3
            };

            string telefone = Utilidades.String.OnlyNumbers(ciot.Transportador.Telefone1);

            if (!string.IsNullOrWhiteSpace(telefone))
            {
                //Verifica se é celular:
                if (Utilidades.String.OnlyNumbers(telefone.Remove(0, 2)).Substring(0, 1) == "9")
                {
                    requisicao.Telefones = new ServicoEFrete.Proprietario.Telefones()
                    {
                        Celular = new ServicoEFrete.Proprietario.Telefone()
                        {
                            DDD = int.Parse(Utilidades.String.OnlyNumbers(telefone.Substring(0, 2))),
                            Numero = int.Parse(Utilidades.String.OnlyNumbers(telefone.Remove(0, 2)))
                        }
                    };
                }
                else
                {
                    requisicao.Telefones = new ServicoEFrete.Proprietario.Telefones()
                    {
                        Fixo = new ServicoEFrete.Proprietario.Telefone()
                        {
                            DDD = int.Parse(Utilidades.String.OnlyNumbers(telefone.Substring(0, 2))),
                            Numero = int.Parse(Utilidades.String.OnlyNumbers(telefone.Remove(0, 2)))
                        }
                    };
                }
            }

            string telefone2 = Utilidades.String.OnlyNumbers(ciot.Transportador.Telefone2);

            if (!string.IsNullOrWhiteSpace(telefone2))
            {
                //Verifica se é celular:
                if (Utilidades.String.OnlyNumbers(telefone2.Remove(0, 2)).Substring(0, 1) == "9")
                {
                    requisicao.Telefones = new ServicoEFrete.Proprietario.Telefones()
                    {
                        Celular = new ServicoEFrete.Proprietario.Telefone()
                        {
                            DDD = int.Parse(Utilidades.String.OnlyNumbers(telefone2.Substring(0, 2))),
                            Numero = int.Parse(Utilidades.String.OnlyNumbers(telefone2.Remove(0, 2)))
                        }
                    };
                }
                else
                {
                    requisicao.Telefones = new ServicoEFrete.Proprietario.Telefones()
                    {
                        Fixo = new ServicoEFrete.Proprietario.Telefone()
                        {
                            DDD = int.Parse(Utilidades.String.OnlyNumbers(telefone2.Substring(0, 2))),
                            Numero = int.Parse(Utilidades.String.OnlyNumbers(telefone2.Remove(0, 2)))
                        }
                    };
                }
            }

            ServicoEFrete.Proprietario.GravarResponse retorno = svcProprietario.Gravar(requisicao);

            if (!retorno.Sucesso)
            {
                ciot.Status = Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Rejeitado;
                ciot.CodigoRetorno = "9";
                ciot.MensagemRetorno = retorno.Excecao?.Mensagem;
            }

            return retorno.Sucesso;
        }

        public bool IntegrarVeiculos(ref Dominio.Entidades.CIOTSigaFacil ciot, string token)
        {
            ServicoEFrete.Veiculo.GravarRequest requisicao = ObterRequisicaoVeiculo(ciot.Veiculo, ciot, token);

            ServicoEFrete.Veiculo.VeiculosServiceSoapClient svcVeiculo = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(_unitOfWork).ObterClient<ServicoEFrete.Veiculo.VeiculosServiceSoapClient, ServicoEFrete.Veiculo.VeiculosServiceSoap>(TipoWebServiceIntegracao.EFrete_Veiculos, out Servicos.Models.Integracao.InspectorBehavior inspector);

            ServicoEFrete.Veiculo.GravarResponse retorno = svcVeiculo.Gravar(requisicao);

            AdicionaLog(inspector, ciot);

            if (!retorno.Sucesso)
            {
                ciot.Status = Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Rejeitado;
                ciot.CodigoRetorno = "9";
                ciot.MensagemRetorno = retorno.Excecao?.Mensagem;

                return retorno.Sucesso;
            }

            for (var i = 0; i < ciot.Veiculo.VeiculosVinculados.Count; i++)
            {
                requisicao = ObterRequisicaoVeiculo(ciot.Veiculo.VeiculosVinculados[i], ciot, token);
                retorno = svcVeiculo.Gravar(requisicao);

                if (!retorno.Sucesso)
                {
                    ciot.Status = Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Rejeitado;
                    ciot.CodigoRetorno = "9";
                    ciot.MensagemRetorno = retorno.Excecao?.Mensagem;

                    return retorno.Sucesso;
                }
            }

            return true;
        }

        #endregion

        #region Métodos Abrir Encerrar CIOT

        private bool AdicionarOperacaoTransporteAbertura(ref Dominio.Entidades.CIOTSigaFacil ciot, string token)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
            Dominio.Entidades.Empresa empresaMatriz = ciot.Empresa.Configuracao.EmpresaMatrizCIOT;//repEmpresa.BuscarEmpresaMatriz(ciot.Empresa);

            ServicoEFrete.PEF.AdicionarOperacaoTransporteRequest requisicao = null;

            int codigoNCMNatureza = 0;
            ushort codigoCarga = 0;
            if (ciot.CategoriaTransportador != Dominio.Enumeradores.CategoriaTransportadorANTT.TAC)
            {
                int.TryParse(ciot.NaturezaCarga.CodigoNatureza, out codigoNCMNatureza);
                ushort.TryParse(ciot.NaturezaCarga.CodigoNatureza, out codigoCarga);
            }

            if (empresaMatriz == null)
            {
                requisicao = new ServicoEFrete.PEF.AdicionarOperacaoTransporteRequest()
                {
                    TipoViagem = ciot.CategoriaTransportador == Dominio.Enumeradores.CategoriaTransportadorANTT.TAC ? ServicoEFrete.PEF.TipoViagem.TAC_Agregado : ServicoEFrete.PEF.TipoViagem.Padrao,
                    Integrador = ciot.Empresa.Configuracao.CodigoIntegradorEFrete,
                    Versao = 7,

                    BloquearNaoEquiparado = ciot.Empresa.Configuracao.BloquearNaoEquiparadoEFrete,
                    MatrizCNPJ = (ciot.Empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao) ? 13969629000196 : long.Parse(ciot.Empresa.CNPJ),
                    IdOperacaoCliente = ciot.Codigo.ToString(),
                    DataFimViagem = ciot.DataTerminoViagem,
                    Contratado = ObterContratado(ciot),
                    Motorista = ObterMotorista(ciot),
                    Contratante = ObterContratante(ciot),
                    Veiculos = ObterVeiculos(ciot),
                    EntregaDocumentacao = ServicoEFrete.PEF.TipoEntregaDocumentacao.RedeCredenciada,
                    CodigoNCMNaturezaCarga = codigoNCMNatureza,
                    Token = token
                };
                if (ciot.TipoPagamento != Dominio.ObjetosDeValor.Enumerador.TipoPagamentoCIOT.SemPgto)
                    requisicao.EmissaoGratuita = ciot.Empresa.Configuracao.EmissaoGratuitaEFrete;
                else
                    requisicao.TipoPagamento = ServicoEFrete.PEF.TipoPagamento.eFRETE;
            }
            else
            {
                requisicao = new ServicoEFrete.PEF.AdicionarOperacaoTransporteRequest()
                {
                    TipoViagem = ciot.CategoriaTransportador == Dominio.Enumeradores.CategoriaTransportadorANTT.TAC ? ServicoEFrete.PEF.TipoViagem.TAC_Agregado : ServicoEFrete.PEF.TipoViagem.Padrao,
                    Integrador = ciot.Empresa.Configuracao.CodigoIntegradorEFrete,
                    Versao = 7,
                    BloquearNaoEquiparado = ciot.Empresa.Configuracao.BloquearNaoEquiparadoEFrete,
                    MatrizCNPJ = (ciot.Empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao) ? 13969629000196 : long.Parse(empresaMatriz.CNPJ),
                    FilialCNPJ = (ciot.Empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao) ? 13969629000196 : long.Parse(ciot.Empresa.CNPJ),
                    IdOperacaoCliente = ciot.Codigo.ToString(),
                    DataFimViagem = ciot.DataTerminoViagem,
                    Contratado = ObterContratado(ciot),
                    Motorista = ObterMotorista(ciot),
                    Contratante = ObterContratante(ciot),
                    Veiculos = ObterVeiculos(ciot),
                    EntregaDocumentacao = ServicoEFrete.PEF.TipoEntregaDocumentacao.RedeCredenciada,
                    CodigoNCMNaturezaCarga = codigoNCMNatureza,
                    Token = token
                };
                if (ciot.TipoPagamento != Dominio.ObjetosDeValor.Enumerador.TipoPagamentoCIOT.SemPgto)
                    requisicao.EmissaoGratuita = ciot.Empresa.Configuracao.EmissaoGratuitaEFrete;
                else
                    requisicao.TipoPagamento = ServicoEFrete.PEF.TipoPagamento.eFRETE;
            }

            if (requisicao.TipoViagem == ServicoEFrete.PEF.TipoViagem.Padrao)
            {
                requisicao.DestinacaoComercial = true;
                requisicao.AltoDesempenho = false;
                requisicao.FreteRetorno = false;
                requisicao.DistanciaRetorno = 0;
                requisicao.CodigoTipoCarga = codigoCarga; //Na emissão com TipoViagem Padrão seu preenchimento é obrigatório. Na emissão com TipoViagem TAC_Agregado o campo não deve ser preenchido
            }

            if (ciot.Empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao)
                requisicao.MatrizCNPJ = 13969629000196;

            ServicoEFrete.PEF.PefServiceSoapClient svcPEF = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(_unitOfWork).ObterClient<ServicoEFrete.PEF.PefServiceSoapClient, ServicoEFrete.PEF.PefServiceSoap>(TipoWebServiceIntegracao.EFrete_Pef, out Servicos.Models.Integracao.InspectorBehavior inspector);

            ServicoEFrete.PEF.AdicionarOperacaoTransporteResponse retorno = svcPEF.AdicionarOperacaoTransporte(requisicao);

            AdicionaLog(inspector, ciot);

            if (retorno.Sucesso)
            {
                ciot.Status = Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Aberto;
                ciot.NumeroCIOT = Utilidades.String.Left(retorno.CodigoIdentificacaoOperacao, 12);
                ciot.CodigoVerificadorCIOT = Utilidades.String.Right(retorno.CodigoIdentificacaoOperacao, 4);
                ciot.MensagemRetorno = "Operação de transporte adicionada com sucesso.";
            }
            else
            {
                ciot.Status = Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Rejeitado;
                ciot.CodigoRetorno = "9";
                ciot.MensagemRetorno = retorno.Excecao?.Mensagem;
            }

            return retorno.Sucesso;
        }

        private ServicoEFrete.PEF.Pagamento2[] ObterPagamentosEncerramento(Dominio.Entidades.CIOTSigaFacil ciot, List<Dominio.Entidades.CTeCIOTSigaFacil> ctes)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            List<ServicoEFrete.PEF.Pagamento2> pagamentos = new List<ServicoEFrete.PEF.Pagamento2>();

            decimal valorAdiantamento = ciot.ValorAdiantamento;
            decimal valorQuitacao = ciot.ValorQuitacao;

            int numeroPagamento = 0;

            if (valorAdiantamento > 0)
            {
                numeroPagamento += 1;
                ServicoEFrete.PEF.Pagamento2 pagamentoAdiantamento = new ServicoEFrete.PEF.Pagamento2()
                {
                    Categoria = ciot.CategoriaTransportador == Dominio.Enumeradores.CategoriaTransportadorANTT.TAC ? ServicoEFrete.PEF.CategoriaPagamento.SemCategoria : ServicoEFrete.PEF.CategoriaPagamento.Adiantamento,
                    DataDeLiberacao = DateTime.Today,
                    Documento = ciot.Numero + " / " + ciot.NumeroCIOT,
                    IdPagamentoCliente = numeroPagamento.ToString(),
                    Valor = valorAdiantamento
                };
                if (ciot.TipoPagamento != Dominio.ObjetosDeValor.Enumerador.TipoPagamentoCIOT.SemPgto)
                {
                    pagamentoAdiantamento.TipoPagamento = ciot.TipoPagamento == Dominio.ObjetosDeValor.Enumerador.TipoPagamentoCIOT.Cartao ? ServicoEFrete.PEF.TipoPagamento.eFRETE : ServicoEFrete.PEF.TipoPagamento.TransferenciaBancaria;
                    pagamentoAdiantamento.InformacoesBancarias = this.ObterInformacoesBancarias(ciot);
                }
                else pagamentoAdiantamento.TipoPagamento = ServicoEFrete.PEF.TipoPagamento.eFRETE;

                pagamentos.Add(pagamentoAdiantamento);
            }

            numeroPagamento += 1;

            ServicoEFrete.PEF.Pagamento2 pagamento = new ServicoEFrete.PEF.Pagamento2()
            {
                Categoria = ciot.CategoriaTransportador == Dominio.Enumeradores.CategoriaTransportadorANTT.TAC ? ServicoEFrete.PEF.CategoriaPagamento.SemCategoria : ServicoEFrete.PEF.CategoriaPagamento.Quitacao,
                DataDeLiberacao = DateTime.Today,
                Documento = ciot.Numero + " / " + ciot.NumeroCIOT,
                IdPagamentoCliente = numeroPagamento.ToString(),
                Valor = valorQuitacao
            };
            if (ciot.TipoPagamento != Dominio.ObjetosDeValor.Enumerador.TipoPagamentoCIOT.SemPgto)
            {
                pagamento.TipoPagamento = ciot.TipoPagamento == Dominio.ObjetosDeValor.Enumerador.TipoPagamentoCIOT.Cartao ? ServicoEFrete.PEF.TipoPagamento.eFRETE : ServicoEFrete.PEF.TipoPagamento.TransferenciaBancaria;
                pagamento.InformacoesBancarias = this.ObterInformacoesBancarias(ciot);
            }
            else pagamento.TipoPagamento = ServicoEFrete.PEF.TipoPagamento.eFRETE;
            pagamentos.Add(pagamento);

            return pagamentos.ToArray();
        }

        private ServicoEFrete.PEF.Viagem2[] ObterViagensEncerramento(Dominio.Entidades.CIOTSigaFacil ciot, List<Dominio.Entidades.CTeCIOTSigaFacil> ctes)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            //if (ciot.CategoriaTransportador != Dominio.Enumeradores.CategoriaTransportadorANTT.TAC)
            //    return null;

            List<ServicoEFrete.PEF.Viagem2> viagens = new List<ServicoEFrete.PEF.Viagem2>();

            for (var i = 0; i < ctes.Count; i++)
            {
                ServicoEFrete.PEF.Viagem2 viagem = new ServicoEFrete.PEF.Viagem2()
                {
                    CodigoMunicipioDestino = ctes[i].CTe.LocalidadeTerminoPrestacao.CodigoIBGE,
                    CepDestino = Utilidades.String.OnlyNumbers(ctes[i].CTe.LocalidadeTerminoPrestacao.CEP),
                    CodigoMunicipioOrigem = ctes[i].CTe.LocalidadeInicioPrestacao.CodigoIBGE,
                    CepOrigem = Utilidades.String.OnlyNumbers(ctes[i].CTe.LocalidadeInicioPrestacao.CEP),
                    DocumentoViagem = ctes[i].CTe.Numero + " / " + ctes[i].CTe.Serie.Numero,
                    NotasFiscais = this.ObterNotasFiscaisEncerramento(ciot, ctes[i], ctes.Count()),
                    Valores = this.ObterValoresViagemEncerramento(ciot, ctes[i], ctes.Count())
                };

                if (ciot.TipoPagamento != Dominio.ObjetosDeValor.Enumerador.TipoPagamentoCIOT.SemPgto)
                {
                    viagem.TipoPagamento = ciot.TipoPagamento == Dominio.ObjetosDeValor.Enumerador.TipoPagamentoCIOT.Deposito ? ServicoEFrete.PEF.TipoPagamento.TransferenciaBancaria : ServicoEFrete.PEF.TipoPagamento.eFRETE;
                    viagem.InformacoesBancarias = this.ObterInformacoesBancarias(ciot);
                }
                else viagem.TipoPagamento = ServicoEFrete.PEF.TipoPagamento.eFRETE;

                viagens.Add(viagem);
            }

            return viagens.ToArray();
        }

        private ServicoEFrete.PEF.NotaFiscal2[] ObterNotasFiscaisEncerramento(Dominio.Entidades.CIOTSigaFacil ciot, Dominio.Entidades.CTeCIOTSigaFacil cte, int totalCTes)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            List<ServicoEFrete.PEF.NotaFiscal2> notasFiscais = new List<ServicoEFrete.PEF.NotaFiscal2>();

            notasFiscais.Add(new ServicoEFrete.PEF.NotaFiscal2()
            {
                CodigoNCMNaturezaCarga = int.Parse(ciot.NaturezaCarga.CodigoNatureza),
                Data = cte.CTe.Documentos.First()?.DataEmissao,
                ValorTotal = cte.ValorTotalMercadoria,
                Numero = cte.CTe.Documentos.First()?.Numero,
                Serie = cte.CTe.Documentos.First()?.SerieOuSerieDaChave,
                QuantidadeDaMercadoriaNoEmbarque = cte.PesoBruto,
                UnidadeDeMedidaDaMercadoria = ServicoEFrete.PEF.UnidadeDeMedidaDaMercadoria1.Kg,
                TipoDeCalculo = ciot.TipoQuebra == Dominio.Enumeradores.TipoQuebra.Integral ? ServicoEFrete.PEF.ViagemTipoDeCalculo1.QuebraIntegral :
                                                                                    ciot.TipoQuebra == Dominio.Enumeradores.TipoQuebra.Parcial ? ServicoEFrete.PEF.ViagemTipoDeCalculo1.QuebraSomenteUltrapassado :
                                                                                    ServicoEFrete.PEF.ViagemTipoDeCalculo1.SemQuebra,
                ValorDaMercadoriaPorUnidade = Math.Round(cte.ValorMercadoriaKG, 2, MidpointRounding.ToEven),
                ValorDoFretePorUnidadeDeMercadoria = Math.Round(ciot.ValorTarifaFrete > 0 ? ciot.ValorTarifaFrete / totalCTes : 0, 2, MidpointRounding.ToEven),
                ToleranciaDePerdaDeMercadoria = new ServicoEFrete.PEF.ToleranciaDePerdaDeMercadoria1()
                {
                    Tipo = ciot.TipoTolerancia == Dominio.Enumeradores.TipoTolerancia.Percentual ? ServicoEFrete.PEF.TipoToleranciaDePerdaDeMercadoria1.Porcentagem :
                                                                               ciot.TipoTolerancia == Dominio.Enumeradores.TipoTolerancia.Peso ? ServicoEFrete.PEF.TipoToleranciaDePerdaDeMercadoria1.ValorAbsoluto :
                                                                               ServicoEFrete.PEF.TipoToleranciaDePerdaDeMercadoria1.Nenhum,
                    Valor = ciot.PercentualTolerancia
                },
                DescricaoDaMercadoria = cte.CTe.ProdutoPredominante
            });

            return notasFiscais.ToArray();
        }

        private ServicoEFrete.PEF.ViagemValores ObterValoresViagemEncerramento(Dominio.Entidades.CIOTSigaFacil ciot, Dominio.Entidades.CTeCIOTSigaFacil cte, int totalCTes)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            ServicoEFrete.PEF.ViagemValores valores = new ServicoEFrete.PEF.ViagemValores()
            {
                Pedagio = ciot.ValorPedagio > 0 ? Math.Round(ciot.ValorPedagio / totalCTes, 2, MidpointRounding.ToEven) : 0,
                Seguro = ciot.ValorSeguro > 0 ? Math.Round(ciot.ValorSeguro / totalCTes, 2, MidpointRounding.ToEven) : 0,
                TotalDeAdiantamento = ciot.ValorAdiantamento > 0 ? Math.Round(ciot.ValorAdiantamento / totalCTes, 2, MidpointRounding.ToEven) : 0,
                TotalDeQuitacao = ciot.ValorQuitacao > 0 ? Math.Round(ciot.ValorQuitacao / totalCTes, 2, MidpointRounding.ToEven) : 0,
                TotalOperacao = ciot.ValorOperacao > 0 ? Math.Round(ciot.ValorOperacao / totalCTes, 2, MidpointRounding.ToEven) : 0,
                TotalViagem = ciot.ValorOperacao > 0 ? Math.Round(ciot.ValorOperacao / totalCTes, 2, MidpointRounding.ToEven) : 0
            };

            return valores;
        }

        private ServicoEFrete.PEF.Impostos ObterImpostosEncerramento(Dominio.Entidades.CIOTSigaFacil ciot)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            ServicoEFrete.PEF.Impostos impostos = new ServicoEFrete.PEF.Impostos()
            {
                INSS = ciot.ValorINSS,
                IRRF = ciot.ValorIRRF,
                SestSenat = ciot.ValorSEST + ciot.ValorSENAT
            };

            return impostos;
        }

        #endregion

        #region Métodos Emitir CIOT

        private bool AdicionarViagem(ref Dominio.Entidades.CIOTSigaFacil ciot, List<Dominio.Entidades.CTeCIOTSigaFacil> ctes, string token)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            ServicoEFrete.PEF.AdicionarViagemRequest requisicao = new ServicoEFrete.PEF.AdicionarViagemRequest()
            {
                CodigoIdentificacaoOperacao = ciot.NumeroCIOT + "/" + ciot.CodigoVerificadorCIOT,
                Integrador = ciot.Empresa.Configuracao.CodigoIntegradorEFrete,
                NaoAdicionarParcialmente = false,
                Pagamentos = ObterPagamentosTAC(ciot, ctes),
                Token = token,
                Versao = 3,
                Viagens = ObterViagensTAC(ciot, ctes)
            };

            ServicoEFrete.PEF.PefServiceSoapClient svcPEF = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(_unitOfWork).ObterClient<ServicoEFrete.PEF.PefServiceSoapClient, ServicoEFrete.PEF.PefServiceSoap>(TipoWebServiceIntegracao.EFrete_Pef, out Servicos.Models.Integracao.InspectorBehavior inspector);

            ServicoEFrete.PEF.AdicionarViagemResponse retorno = svcPEF.AdicionarViagem(requisicao);

            if (retorno.Sucesso)
            {
                ciot.Status = Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Autorizado;
                ciot.CodigoRetorno = "0";
                ciot.MensagemRetorno = "Operação realizada com sucesso.";
                //ciot.NumeroCIOT = retorno.CodigoIdentificacaoOperacao;
            }
            else
            {
                ciot.Status = Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Rejeitado;
                ciot.CodigoRetorno = "9";
                ciot.MensagemRetorno = retorno.Excecao?.Mensagem;
            }

            return retorno.Sucesso;
        }

        private bool IntegrarOperacaoTransporte(ref Dominio.Entidades.CIOTSigaFacil ciot, string token)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Repositorio.CTeCIOTSigaFacil repCTeCIOT = new Repositorio.CTeCIOTSigaFacil(_unitOfWork);

            List<Dominio.Entidades.CTeCIOTSigaFacil> ctes = repCTeCIOT.BuscarPorCIOT(ciot.Codigo);

            if (ciot.CategoriaTransportador == Dominio.Enumeradores.CategoriaTransportadorANTT.TAC && !string.IsNullOrWhiteSpace(ciot.NumeroCIOT))
                return AdicionarViagem(ref ciot, ctes, token);

            ServicoEFrete.PEF.AdicionarOperacaoTransporteRequest requisicao = new ServicoEFrete.PEF.AdicionarOperacaoTransporteRequest()
            {
                TipoViagem = ciot.CategoriaTransportador == Dominio.Enumeradores.CategoriaTransportadorANTT.TAC ? ServicoEFrete.PEF.TipoViagem.TAC_Agregado : ServicoEFrete.PEF.TipoViagem.Padrao,
                Integrador = ciot.Empresa.Configuracao.CodigoIntegradorEFrete,
                Versao = 7,
                BloquearNaoEquiparado = ciot.Empresa.Configuracao.BloquearNaoEquiparadoEFrete,
                MatrizCNPJ = (ciot.Empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao) ? 13969629000196 : long.Parse(ciot.Empresa.CNPJ),
                IdOperacaoCliente = ciot.Codigo.ToString(),
                DataFimViagem = ciot.DataTerminoViagem,
                Impostos = ObterImpostos(ciot, ctes),
                Contratado = ObterContratado(ciot),
                Motorista = ObterMotorista(ciot),
                Contratante = ObterContratante(ciot),
                Veiculos = ObterVeiculos(ciot),
                EntregaDocumentacao = ServicoEFrete.PEF.TipoEntregaDocumentacao.RedeCredenciada,
                Token = token,
                TipoEmbalagem = ServicoEFrete.PEF.TipoEmbalagem.Caixa,
                TomadorServico = ObterTomador(ciot, ctes)
            };

            if (ciot.TipoPagamento != Dominio.ObjetosDeValor.Enumerador.TipoPagamentoCIOT.SemPgto)
                requisicao.EmissaoGratuita = ciot.Empresa.Configuracao.EmissaoGratuitaEFrete;
            else
                requisicao.TipoPagamento = ServicoEFrete.PEF.TipoPagamento.eFRETE;

            if (requisicao.TipoViagem == ServicoEFrete.PEF.TipoViagem.Padrao)
            {
                int codigoNCMNatureza = 0;
                ushort codigoCarga = 0;
                int.TryParse(ciot.NaturezaCarga.CodigoNatureza, out codigoNCMNatureza);
                if (ciot.TipoIntegradora != Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraCIOT.EFrete) //Orientação da eFrete é enviar zero
                    ushort.TryParse(ciot.NaturezaCarga.CodigoNatureza, out codigoCarga);


                requisicao.CodigoNCMNaturezaCarga = codigoNCMNatureza;
                requisicao.DataInicioViagem = ciot.DataInicioViagem;
                requisicao.PesoCarga = (from obj in ctes select obj.PesoBruto).Sum();
                requisicao.Viagens = this.ObterViagens(ciot, ctes);
                requisicao.Destinatario = this.ObterDestinatario(ciot, ctes);
                requisicao.Pagamentos = ObterPagamentos(ciot, ctes);
                requisicao.DestinacaoComercial = true;
                requisicao.AltoDesempenho = false;
                requisicao.FreteRetorno = false;
                requisicao.DistanciaRetorno = 0;
                requisicao.CodigoTipoCarga = codigoCarga; //Na emissão com TipoViagem Padrão seu preenchimento é obrigatório. Na emissão com TipoViagem TAC_Agregado o campo não deve ser preenchido
            }

            ServicoEFrete.PEF.PefServiceSoapClient svcPEF = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(_unitOfWork).ObterClient<ServicoEFrete.PEF.PefServiceSoapClient, ServicoEFrete.PEF.PefServiceSoap>(TipoWebServiceIntegracao.EFrete_Pef, out Servicos.Models.Integracao.InspectorBehavior inspector);

            ServicoEFrete.PEF.AdicionarOperacaoTransporteResponse retorno = svcPEF.AdicionarOperacaoTransporte(requisicao);
            AdicionaLog(inspector, ciot);

            if (retorno.Sucesso)
            {
                ciot.NumeroCIOT = Utilidades.String.Left(retorno.CodigoIdentificacaoOperacao, 12);
                ciot.CodigoVerificadorCIOT = Utilidades.String.Right(retorno.CodigoIdentificacaoOperacao, 4);

                if (requisicao.TipoViagem == ServicoEFrete.PEF.TipoViagem.TAC_Agregado)
                {
                    AdicionarViagem(ref ciot, ctes, token);
                }
                else
                {
                    ciot.Status = Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Autorizado;
                    ciot.CodigoRetorno = "0";
                    ciot.MensagemRetorno = "Operação realizada com sucesso.";
                }

            }
            else
            {
                ciot.Status = Dominio.ObjetosDeValor.Enumerador.StatusCIOT.Rejeitado;
                ciot.CodigoRetorno = "9";
                ciot.MensagemRetorno = retorno.Excecao?.Mensagem;
            }

            return retorno.Sucesso;
        }

        private ServicoEFrete.Veiculo.GravarRequest ObterRequisicaoVeiculo(Dominio.Entidades.Veiculo veiculo, Dominio.Entidades.CIOTSigaFacil ciot, string token)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            ServicoEFrete.Veiculo.GravarRequest requisicao = new ServicoEFrete.Veiculo.GravarRequest()
            {
                Integrador = ciot.Empresa.Configuracao.CodigoIntegradorEFrete,
                Versao = 1,
                Token = token,
                Veiculo = new ServicoEFrete.Veiculo.Veiculo()
                {
                    AnoFabricacao = veiculo.AnoFabricacao,
                    AnoModelo = veiculo.AnoModelo,
                    CapacidadeKg = veiculo.CapacidadeKG,
                    CapacidadeM3 = veiculo.CapacidadeM3,
                    Chassi = veiculo.Chassi,
                    CodigoMunicipio = ciot.Transportador.Localidade.CodigoIBGE,
                    Cor = "Não disponível",
                    Marca = veiculo.Marca?.Descricao,
                    Modelo = veiculo.Modelo?.Descricao,
                    NumeroDeEixos = veiculo.TipoDoVeiculo?.NumeroEixos,
                    Placa = veiculo.Placa,
                    Renavam = long.Parse(veiculo.Renavam),
                    RNTRC = veiculo.Tipo == "T" ? veiculo.RNTRC : !string.IsNullOrWhiteSpace(veiculo.Empresa?.RegistroANTT ?? "") ? int.Parse(veiculo.Empresa?.RegistroANTT) : veiculo.RNTRC,
                    Tara = veiculo.Tara,
                    TipoCarroceria = ObterTipoCarroceria(veiculo.TipoCarroceria),
                    TipoRodado = ObterTipoRodado(veiculo.TipoRodado)
                }
            };

            return requisicao;
        }

        private ServicoEFrete.Veiculo.TipoCarroceria ObterTipoCarroceria(string tipoCarroceria)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

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
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

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

        private ServicoEFrete.PEF.Contratante ObterContratante(Dominio.Entidades.CIOTSigaFacil ciot)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            ServicoEFrete.PEF.Contratante contratante = new ServicoEFrete.PEF.Contratante()
            {
                ResponsavelPeloPagamento = true,
                RNTRC = int.Parse(ciot.Empresa.RegistroANTT)
            };

            if (ciot.Empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao)
            {
                contratante.CpfOuCnpj = 13969629000196;
                contratante.Endereco = new ServicoEFrete.PEF.Endereco()
                {
                    Bairro = "CENTO",
                    CEP = "89802-130",
                    CodigoMunicipio = 4204202,
                    Complemento = "SALA 1005",
                    Numero = "427",
                    Rua = "AV. PORTO ALEGRE"
                };
                contratante.NomeOuRazaoSocial = "TECCHAPECO SISTEMAS";
            }
            else
            {
                contratante.CpfOuCnpj = long.Parse(ciot.Empresa.CNPJ);
                contratante.Endereco = new ServicoEFrete.PEF.Endereco()
                {
                    Bairro = ciot.Empresa.Bairro,
                    CEP = ciot.Empresa.CEP,
                    CodigoMunicipio = ciot.Empresa.Localidade.CodigoIBGE,
                    Complemento = ciot.Empresa.Complemento,
                    Numero = ciot.Empresa.Numero,
                    Rua = ciot.Empresa.Endereco
                };
                contratante.NomeOuRazaoSocial = ciot.Empresa.RazaoSocial;
            }

            return contratante;
        }

        private ServicoEFrete.PEF.Destinatario ObterDestinatario(Dominio.Entidades.CIOTSigaFacil ciot, List<Dominio.Entidades.CTeCIOTSigaFacil> ctes)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Dominio.Entidades.ParticipanteCTe destinatarioCTe = ctes[0].CTe.Recebedor != null ? ctes[0].CTe.Recebedor : ctes[0].CTe.Destinatario;

            ServicoEFrete.PEF.Destinatario destinatario = new ServicoEFrete.PEF.Destinatario()
            {
                CpfOuCnpj = long.Parse(destinatarioCTe.CPF_CNPJ),
                NomeOuRazaoSocial = destinatarioCTe.Nome,
                ResponsavelPeloPagamento = false,
                Endereco = new ServicoEFrete.PEF.Endereco()
                {
                    Bairro = destinatarioCTe.Bairro,
                    CEP = destinatarioCTe.CEP,
                    CodigoMunicipio = destinatarioCTe.Localidade.CodigoIBGE,
                    Complemento = destinatarioCTe.Complemento,
                    Numero = destinatarioCTe.Numero,
                    Rua = destinatarioCTe.Endereco
                }
            };

            return destinatario;
        }

        private ServicoEFrete.PEF.Motorista ObterMotorista(Dominio.Entidades.CIOTSigaFacil ciot)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            ServicoEFrete.PEF.Motorista motorista = new ServicoEFrete.PEF.Motorista()
            {
                Celular = new ServicoEFrete.PEF.Telefone()
                {
                    DDD = int.Parse(Utilidades.String.OnlyNumbers(ciot.Motorista.Telefone?.Split(' ')[0])),
                    Numero = int.Parse(Utilidades.String.OnlyNumbers(ciot.Motorista.Telefone?.Split(' ')[1]))
                },
                CNH = long.Parse(ciot.Motorista.NumeroHabilitacao),
                CpfOuCnpj = long.Parse(ciot.Motorista.CPF)
            };

            return motorista;
        }

        private ServicoEFrete.PEF.Contratado ObterContratado(Dominio.Entidades.CIOTSigaFacil ciot)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            ServicoEFrete.PEF.Contratado contratado = new ServicoEFrete.PEF.Contratado()
            {
                CpfOuCnpj = (long)ciot.Transportador.CPF_CNPJ,
                RNTRC = ciot.Veiculo.RNTRC
            };

            return contratado;
        }

        private ServicoEFrete.PEF.Viagem[] ObterViagens(Dominio.Entidades.CIOTSigaFacil ciot, List<Dominio.Entidades.CTeCIOTSigaFacil> ctes)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            if (ciot.CategoriaTransportador == Dominio.Enumeradores.CategoriaTransportadorANTT.TAC)
                return null;

            List<ServicoEFrete.PEF.Viagem> viagens = new List<ServicoEFrete.PEF.Viagem>();

            ServicoEFrete.PEF.Viagem viagem = new ServicoEFrete.PEF.Viagem()
            {
                CodigoMunicipioDestino = ciot.Destino.CodigoIBGE,
                CepDestino = Utilidades.String.OnlyNumbers(ciot.Destino.CEP),
                CodigoMunicipioOrigem = ciot.Origem.CodigoIBGE,
                CepOrigem = Utilidades.String.OnlyNumbers(ciot.Origem.CEP),
                DocumentoViagem = ctes[0].CTe.Numero + " / " + ctes[0].CTe.Serie.Numero,
                NotasFiscais = this.ObterNotasFiscais(ciot, ctes),
                Valores = this.ObterValoresViagem(ciot, ctes),
                DistanciaPercorrida = 1
            };
            if (ciot.TipoPagamento != Dominio.ObjetosDeValor.Enumerador.TipoPagamentoCIOT.SemPgto)
            {
                viagem.TipoPagamento = ciot.TipoPagamento == Dominio.ObjetosDeValor.Enumerador.TipoPagamentoCIOT.Deposito ? ServicoEFrete.PEF.TipoPagamento.TransferenciaBancaria : ServicoEFrete.PEF.TipoPagamento.eFRETE;
                viagem.InformacoesBancarias = this.ObterInformacoesBancarias(ciot);
            }
            else viagem.TipoPagamento = ServicoEFrete.PEF.TipoPagamento.eFRETE;

            viagens.Add(viagem);

            return viagens.ToArray();
        }

        private ServicoEFrete.PEF.Viagem2[] ObterViagensTAC(Dominio.Entidades.CIOTSigaFacil ciot, List<Dominio.Entidades.CTeCIOTSigaFacil> ctes)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            if (ciot.CategoriaTransportador != Dominio.Enumeradores.CategoriaTransportadorANTT.TAC)
                return null;

            List<ServicoEFrete.PEF.Viagem2> viagens = new List<ServicoEFrete.PEF.Viagem2>();

            ServicoEFrete.PEF.Viagem2 viagem = new ServicoEFrete.PEF.Viagem2()
            {
                CodigoMunicipioDestino = ciot.Destino.CodigoIBGE,
                CepDestino = Utilidades.String.OnlyNumbers(ciot.Destino.CEP),
                CodigoMunicipioOrigem = ciot.Origem.CodigoIBGE,
                CepOrigem = Utilidades.String.OnlyNumbers(ciot.Origem.CEP),
                DocumentoViagem = ctes[0].CTe.Numero + " / " + ctes[0].CTe.Serie.Numero,
                NotasFiscais = this.ObterNotasFiscaisTAC(ciot, ctes),
                Valores = this.ObterValoresViagem(ciot, ctes)
            };
            if (ciot.TipoPagamento != Dominio.ObjetosDeValor.Enumerador.TipoPagamentoCIOT.SemPgto)
            {
                viagem.TipoPagamento = ciot.TipoPagamento == Dominio.ObjetosDeValor.Enumerador.TipoPagamentoCIOT.Deposito ? ServicoEFrete.PEF.TipoPagamento.TransferenciaBancaria : ServicoEFrete.PEF.TipoPagamento.eFRETE;
                viagem.InformacoesBancarias = this.ObterInformacoesBancarias(ciot);
            }
            else viagem.TipoPagamento = ServicoEFrete.PEF.TipoPagamento.eFRETE;

            viagens.Add(viagem);

            return viagens.ToArray();
        }

        private ServicoEFrete.PEF.Pagamento[] ObterPagamentos(Dominio.Entidades.CIOTSigaFacil ciot, List<Dominio.Entidades.CTeCIOTSigaFacil> ctes)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            List<ServicoEFrete.PEF.Pagamento> pagamentos = new List<ServicoEFrete.PEF.Pagamento>();

            decimal valorAdiantamento = (from obj in ctes select obj.ValorAdiantamento).Sum();
            decimal valorQuitacao = (from obj in ctes select obj.ValorFrete + obj.ValorPedagio + obj.ValorSeguro + obj.ValorINSS + obj.ValorIRRF + obj.ValorSENAT + obj.ValorSEST).Sum();

            if (valorAdiantamento > 0)
            {
                ServicoEFrete.PEF.Pagamento pagamentoAdiantamento = new ServicoEFrete.PEF.Pagamento()
                {
                    Categoria = ServicoEFrete.PEF.CategoriaPagamento.Adiantamento,
                    DataDeLiberacao = ciot.DataInicioViagem,
                    Documento = ctes[0].CTe.Numero + " / " + ctes[0].CTe.Serie.Numero,
                    IdPagamentoCliente = "1",//ciot.Codigo.ToString(),
                    Valor = valorAdiantamento
                };
                if (ciot.TipoPagamento != Dominio.ObjetosDeValor.Enumerador.TipoPagamentoCIOT.SemPgto)
                {
                    pagamentoAdiantamento.InformacoesBancarias = this.ObterInformacoesBancarias(ciot);
                    pagamentoAdiantamento.TipoPagamento = ciot.TipoPagamento == Dominio.ObjetosDeValor.Enumerador.TipoPagamentoCIOT.Cartao ? ServicoEFrete.PEF.TipoPagamento.eFRETE : ServicoEFrete.PEF.TipoPagamento.TransferenciaBancaria;
                }
                else pagamentoAdiantamento.TipoPagamento = ServicoEFrete.PEF.TipoPagamento.eFRETE;

                pagamentos.Add(pagamentoAdiantamento);
            }

            ServicoEFrete.PEF.Pagamento pagamento = new ServicoEFrete.PEF.Pagamento()
            {
                Categoria = ServicoEFrete.PEF.CategoriaPagamento.Quitacao,
                DataDeLiberacao = ciot.DataTerminoViagem,
                Documento = ctes[0].CTe.Numero + " / " + ctes[0].CTe.Serie.Numero,
                IdPagamentoCliente = "2",//ciot.Codigo.ToString(),
                Valor = valorQuitacao
            };
            if (ciot.TipoPagamento != Dominio.ObjetosDeValor.Enumerador.TipoPagamentoCIOT.SemPgto)
            {
                pagamento.InformacoesBancarias = this.ObterInformacoesBancarias(ciot);
                pagamento.TipoPagamento = ciot.TipoPagamento == Dominio.ObjetosDeValor.Enumerador.TipoPagamentoCIOT.Cartao ? ServicoEFrete.PEF.TipoPagamento.eFRETE : ServicoEFrete.PEF.TipoPagamento.TransferenciaBancaria;
            }
            else pagamento.TipoPagamento = ServicoEFrete.PEF.TipoPagamento.eFRETE;

            pagamentos.Add(pagamento);

            return pagamentos.ToArray();
        }

        private ServicoEFrete.PEF.Pagamento2[] ObterPagamentosTAC(Dominio.Entidades.CIOTSigaFacil ciot, List<Dominio.Entidades.CTeCIOTSigaFacil> ctes)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            List<ServicoEFrete.PEF.Pagamento2> pagamentos = new List<ServicoEFrete.PEF.Pagamento2>();

            decimal valorAdiantamento = (from obj in ctes select obj.ValorAdiantamento).Sum();
            decimal valorQuitacao = (from obj in ctes select obj.ValorFrete + obj.ValorPedagio + obj.ValorSeguro + obj.ValorINSS + obj.ValorIRRF + obj.ValorSENAT + obj.ValorSEST).Sum();

            if (valorAdiantamento > 0)
            {

                ServicoEFrete.PEF.Pagamento2 pagamentoAdiantamento = new ServicoEFrete.PEF.Pagamento2()
                {
                    Categoria = ServicoEFrete.PEF.CategoriaPagamento.SemCategoria,
                    DataDeLiberacao = ciot.DataInicioViagem,
                    Documento = ctes[0].CTe.Numero + " / " + ctes[0].CTe.Serie.Numero,
                    IdPagamentoCliente = "1",//ciot.Codigo.ToString(),
                    Valor = valorAdiantamento
                };
                if (ciot.TipoPagamento != Dominio.ObjetosDeValor.Enumerador.TipoPagamentoCIOT.SemPgto)
                {
                    pagamentoAdiantamento.InformacoesBancarias = this.ObterInformacoesBancarias(ciot);
                    pagamentoAdiantamento.TipoPagamento = ciot.TipoPagamento == Dominio.ObjetosDeValor.Enumerador.TipoPagamentoCIOT.Cartao ? ServicoEFrete.PEF.TipoPagamento.eFRETE : ServicoEFrete.PEF.TipoPagamento.TransferenciaBancaria;
                }
                else pagamentoAdiantamento.TipoPagamento = ServicoEFrete.PEF.TipoPagamento.eFRETE;

                pagamentos.Add(pagamentoAdiantamento);
            }

            ServicoEFrete.PEF.Pagamento2 pagamento = new ServicoEFrete.PEF.Pagamento2()
            {
                Categoria = ServicoEFrete.PEF.CategoriaPagamento.SemCategoria,
                DataDeLiberacao = ciot.DataTerminoViagem,
                Documento = ctes[0].CTe.Numero + " / " + ctes[0].CTe.Serie.Numero,
                IdPagamentoCliente = "2",//ciot.Codigo.ToString(),
                Valor = valorQuitacao
            };
            if (ciot.TipoPagamento != Dominio.ObjetosDeValor.Enumerador.TipoPagamentoCIOT.SemPgto)
            {
                pagamento.InformacoesBancarias = this.ObterInformacoesBancarias(ciot);
                pagamento.TipoPagamento = ciot.TipoPagamento == Dominio.ObjetosDeValor.Enumerador.TipoPagamentoCIOT.Cartao ? ServicoEFrete.PEF.TipoPagamento.eFRETE : ServicoEFrete.PEF.TipoPagamento.TransferenciaBancaria;
            }
            else pagamento.TipoPagamento = ServicoEFrete.PEF.TipoPagamento.eFRETE;

            pagamentos.Add(pagamento);

            return pagamentos.ToArray();
        }

        private ServicoEFrete.PEF.Impostos ObterImpostos(Dominio.Entidades.CIOTSigaFacil ciot, List<Dominio.Entidades.CTeCIOTSigaFacil> ctes)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            ServicoEFrete.PEF.Impostos impostos = new ServicoEFrete.PEF.Impostos()
            {
                INSS = (from obj in ctes select obj.ValorINSS).Sum(),
                IRRF = (from obj in ctes select obj.ValorIRRF).Sum(),
                SestSenat = (from obj in ctes select obj.ValorSEST + obj.ValorSENAT).Sum()
            };

            return impostos;
        }

        private ServicoEFrete.PEF.TomadorServico ObterTomador(Dominio.Entidades.CIOTSigaFacil ciot, List<Dominio.Entidades.CTeCIOTSigaFacil> ctes)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            ServicoEFrete.PEF.TomadorServico tomador = new ServicoEFrete.PEF.TomadorServico()
            {
                CpfOuCnpj = long.Parse(Utilidades.String.OnlyNumbers(ctes.FirstOrDefault().CTe.Tomador.CPF_CNPJ)),
                Endereco = new ServicoEFrete.PEF.Endereco()
                {
                    Bairro = ctes.FirstOrDefault().CTe.Tomador.Bairro,
                    CEP = ctes.FirstOrDefault().CTe.Tomador.CEP,
                    CodigoMunicipio = ctes.FirstOrDefault().CTe.Tomador.Localidade.CodigoIBGE,
                    Complemento = ctes.FirstOrDefault().CTe.Tomador.Complemento,
                    Numero = ctes.FirstOrDefault().CTe.Tomador.Numero,
                    Rua = ctes.FirstOrDefault().CTe.Tomador.Endereco
                },
                EMail = ctes.FirstOrDefault().CTe.Tomador.Email,
                NomeOuRazaoSocial = ctes.FirstOrDefault().CTe.Tomador.Nome,
                ResponsavelPeloPagamento = true,
                //Telefones = new ServicoEFrete.PEF.Telefones()
                //{
                //    Fixo = new ServicoEFrete.PEF.Telefone()
                //    {
                //        DDD = int.Parse(Utilidades.String.OnlyNumbers(ctes.FirstOrDefault().CTe.Tomador.Telefone1?.Length > 2 ? ctes.FirstOrDefault().CTe.Tomador.Telefone1.Substring(0,2) : "0")),
                //        Numero = int.Parse(Utilidades.String.OnlyNumbers(ctes.FirstOrDefault().CTe.Tomador.Telefone1?.Length > 2 ? ctes.FirstOrDefault().CTe.Tomador.Telefone1.Substring(2, ctes.FirstOrDefault().CTe.Tomador.Telefone1.Length - 2) : "0"))
                //    }
                //}
            };

            return tomador;
        }

        private ServicoEFrete.PEF.InformacoesBancarias ObterInformacoesBancarias(Dominio.Entidades.CIOTSigaFacil ciot)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Repositorio.DadosCliente repDadosCliente = new Repositorio.DadosCliente(_unitOfWork);

            Dominio.Entidades.DadosCliente dadosTransportador = repDadosCliente.Buscar(ciot.Empresa.Codigo, ciot.Transportador.CPF_CNPJ);

            if (ciot.TipoPagamento == Dominio.ObjetosDeValor.Enumerador.TipoPagamentoCIOT.Cartao || dadosTransportador == null)
                return null;

            ServicoEFrete.PEF.InformacoesBancarias informacoesBancarias = new ServicoEFrete.PEF.InformacoesBancarias();

            informacoesBancarias.Agencia = dadosTransportador.Agencia + "-" + dadosTransportador.DigitoAgencia;
            informacoesBancarias.Conta = dadosTransportador.NumeroConta;
            informacoesBancarias.InstituicaoBancaria = dadosTransportador.Banco != null ? string.Format("{0:000}", dadosTransportador.Banco.Numero) : string.Empty;
            informacoesBancarias.TipoConta = dadosTransportador.TipoConta == Dominio.ObjetosDeValor.Enumerador.TipoConta.Corrente ? ServicoEFrete.PEF.TipoConta.ContaCorrente : ServicoEFrete.PEF.TipoConta.ContaPoupanca;

            return informacoesBancarias;
        }

        private ServicoEFrete.PEF.NotaFiscal[] ObterNotasFiscais(Dominio.Entidades.CIOTSigaFacil ciot, List<Dominio.Entidades.CTeCIOTSigaFacil> ctes)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            List<ServicoEFrete.PEF.NotaFiscal> notasFiscais = (from cte in ctes
                                                               select new ServicoEFrete.PEF.NotaFiscal()
                                                               {
                                                                   CodigoNCMNaturezaCarga = int.Parse(ciot.NaturezaCarga.CodigoNatureza),
                                                                   Data = cte.CTe.Documentos.First()?.DataEmissao,
                                                                   ValorTotal = cte.ValorTotalMercadoria,
                                                                   Numero = cte.CTe.Documentos.First()?.Numero,
                                                                   Serie = cte.CTe.Documentos.First()?.SerieOuSerieDaChave,
                                                                   QuantidadeDaMercadoriaNoEmbarque = cte.PesoBruto,
                                                                   UnidadeDeMedidaDaMercadoria = ServicoEFrete.PEF.UnidadeDeMedidaDaMercadoria.Kg,
                                                                   TipoDeCalculo = cte.TipoQuebra == Dominio.Enumeradores.TipoQuebra.Integral ? ServicoEFrete.PEF.ViagemTipoDeCalculo.QuebraIntegral :
                                                                                   cte.TipoQuebra == Dominio.Enumeradores.TipoQuebra.Parcial ? ServicoEFrete.PEF.ViagemTipoDeCalculo.QuebraSomenteUltrapassado :
                                                                                   ServicoEFrete.PEF.ViagemTipoDeCalculo.SemQuebra,
                                                                   ValorDaMercadoriaPorUnidade = Math.Round(cte.ValorMercadoriaKG, 2, MidpointRounding.ToEven),
                                                                   ValorDoFretePorUnidadeDeMercadoria = Math.Round(cte.ValorTarifaFrete, 2, MidpointRounding.ToEven),
                                                                   ToleranciaDePerdaDeMercadoria = new ServicoEFrete.PEF.ToleranciaDePerdaDeMercadoria()
                                                                   {
                                                                       Tipo = cte.TipoTolerancia == Dominio.Enumeradores.TipoTolerancia.Percentual ? ServicoEFrete.PEF.TipoToleranciaDePerdaDeMercadoria.Porcentagem :
                                                                              cte.TipoTolerancia == Dominio.Enumeradores.TipoTolerancia.Peso ? ServicoEFrete.PEF.TipoToleranciaDePerdaDeMercadoria.ValorAbsoluto :
                                                                              ServicoEFrete.PEF.TipoToleranciaDePerdaDeMercadoria.Nenhum,
                                                                       Valor = cte.PercentualTolerancia
                                                                   },
                                                                   DiferencaDeFrete = new ServicoEFrete.PEF.DiferencaDeFrete()
                                                                   {
                                                                       Base = ServicoEFrete.PEF.DiferencaFreteBaseCalculo.QuantidadeDesembarque,
                                                                       Tipo = cte.TipoQuebra == Dominio.Enumeradores.TipoQuebra.SemQuebra ? ServicoEFrete.PEF.DiferencaFreteTipo.SemDiferenca :
                                                                              cte.TipoQuebra == Dominio.Enumeradores.TipoQuebra.Parcial ? ServicoEFrete.PEF.DiferencaFreteTipo.SomenteUltrapassado : ServicoEFrete.PEF.DiferencaFreteTipo.Integral,
                                                                       Tolerancia = new ServicoEFrete.PEF.DiferencaFreteTolerancia()
                                                                       {
                                                                           Tipo = cte.TipoTolerancia == Dominio.Enumeradores.TipoTolerancia.Percentual ? ServicoEFrete.PEF.TipoProporcao.Porcentagem :
                                                                                  cte.TipoTolerancia == Dominio.Enumeradores.TipoTolerancia.Peso ? ServicoEFrete.PEF.TipoProporcao.Absoluto :
                                                                                  ServicoEFrete.PEF.TipoProporcao.Nenhum,
                                                                           Valor = cte.PercentualTolerancia
                                                                       },
                                                                       MargemGanho = new ServicoEFrete.PEF.DiferencaFreteMargem()
                                                                       {
                                                                           Tipo = cte.TipoTolerancia == Dominio.Enumeradores.TipoTolerancia.Percentual ? ServicoEFrete.PEF.TipoProporcao.Porcentagem :
                                                                                  cte.TipoTolerancia == Dominio.Enumeradores.TipoTolerancia.Peso ? ServicoEFrete.PEF.TipoProporcao.Absoluto :
                                                                                  ServicoEFrete.PEF.TipoProporcao.Nenhum,
                                                                           Valor = cte.PercentualToleranciaSuperior
                                                                       },
                                                                       MargemPerda = new ServicoEFrete.PEF.DiferencaFreteMargem()
                                                                       {
                                                                           Tipo = cte.TipoTolerancia == Dominio.Enumeradores.TipoTolerancia.Percentual ? ServicoEFrete.PEF.TipoProporcao.Porcentagem :
                                                                                  cte.TipoTolerancia == Dominio.Enumeradores.TipoTolerancia.Peso ? ServicoEFrete.PEF.TipoProporcao.Absoluto :
                                                                                  ServicoEFrete.PEF.TipoProporcao.Nenhum,
                                                                           Valor = cte.PercentualTolerancia
                                                                       }
                                                                   },
                                                                   DescricaoDaMercadoria = cte.CTe.ProdutoPredominante
                                                               }).ToList();

            return notasFiscais.ToArray();
        }

        private ServicoEFrete.PEF.NotaFiscal2[] ObterNotasFiscaisTAC(Dominio.Entidades.CIOTSigaFacil ciot, List<Dominio.Entidades.CTeCIOTSigaFacil> ctes)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            List<ServicoEFrete.PEF.NotaFiscal2> notasFiscais = (from cte in ctes
                                                                select new ServicoEFrete.PEF.NotaFiscal2()
                                                                {
                                                                    CodigoNCMNaturezaCarga = int.Parse(ciot.NaturezaCarga.CodigoNatureza),
                                                                    Data = cte.CTe.Documentos.First()?.DataEmissao,
                                                                    ValorTotal = cte.ValorTotalMercadoria,
                                                                    Numero = cte.CTe.Documentos.First()?.Numero,
                                                                    Serie = cte.CTe.Documentos.First()?.SerieOuSerieDaChave,
                                                                    QuantidadeDaMercadoriaNoEmbarque = cte.PesoBruto,
                                                                    UnidadeDeMedidaDaMercadoria = ServicoEFrete.PEF.UnidadeDeMedidaDaMercadoria1.Kg,
                                                                    TipoDeCalculo = cte.TipoQuebra == Dominio.Enumeradores.TipoQuebra.Integral ? ServicoEFrete.PEF.ViagemTipoDeCalculo1.QuebraIntegral :
                                                                                    cte.TipoQuebra == Dominio.Enumeradores.TipoQuebra.Parcial ? ServicoEFrete.PEF.ViagemTipoDeCalculo1.QuebraSomenteUltrapassado :
                                                                                    ServicoEFrete.PEF.ViagemTipoDeCalculo1.SemQuebra,
                                                                    ValorDaMercadoriaPorUnidade = Math.Round(cte.ValorMercadoriaKG, 2, MidpointRounding.ToEven),
                                                                    ValorDoFretePorUnidadeDeMercadoria = Math.Round(cte.ValorTarifaFrete, 2, MidpointRounding.ToEven),
                                                                    ToleranciaDePerdaDeMercadoria = new ServicoEFrete.PEF.ToleranciaDePerdaDeMercadoria1()
                                                                    {
                                                                        Tipo = cte.TipoTolerancia == Dominio.Enumeradores.TipoTolerancia.Percentual ? ServicoEFrete.PEF.TipoToleranciaDePerdaDeMercadoria1.Porcentagem :
                                                                               cte.TipoTolerancia == Dominio.Enumeradores.TipoTolerancia.Peso ? ServicoEFrete.PEF.TipoToleranciaDePerdaDeMercadoria1.ValorAbsoluto :
                                                                               ServicoEFrete.PEF.TipoToleranciaDePerdaDeMercadoria1.Nenhum,
                                                                        Valor = cte.PercentualTolerancia
                                                                    },
                                                                    DescricaoDaMercadoria = cte.CTe.ProdutoPredominante
                                                                }).ToList();

            return notasFiscais.ToArray();
        }

        private ServicoEFrete.PEF.ViagemValores ObterValoresViagem(Dominio.Entidades.CIOTSigaFacil ciot, List<Dominio.Entidades.CTeCIOTSigaFacil> ctes)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            ServicoEFrete.PEF.ViagemValores valores = new ServicoEFrete.PEF.ViagemValores()
            {
                Pedagio = (from obj in ctes select obj.ValorPedagio).Sum(),
                Seguro = (from obj in ctes select obj.ValorSeguro).Sum(),
                TotalDeAdiantamento = (from obj in ctes select obj.ValorAdiantamento).Sum(),
                TotalDeQuitacao = (from obj in ctes select obj.ValorFrete + obj.ValorPedagio + obj.ValorSeguro + obj.ValorINSS + obj.ValorIRRF + obj.ValorSENAT + obj.ValorSEST).Sum(),
                TotalOperacao = (from obj in ctes select obj.ValorFrete + obj.ValorPedagio + obj.ValorSeguro + obj.ValorAdiantamento + obj.ValorIRRF + obj.ValorSENAT + obj.ValorSEST).Sum(),
                TotalViagem = (from obj in ctes select obj.ValorFrete + obj.ValorPedagio + obj.ValorSeguro + obj.ValorAdiantamento + obj.ValorIRRF + obj.ValorSENAT + obj.ValorSEST).Sum()
            };

            return valores;
        }

        private ServicoEFrete.PEF.Veiculo[] ObterVeiculos(Dominio.Entidades.CIOTSigaFacil ciot)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            List<ServicoEFrete.PEF.Veiculo> veiculos = new List<ServicoEFrete.PEF.Veiculo>();

            veiculos.Add(new ServicoEFrete.PEF.Veiculo() { Placa = ciot.Veiculo.Placa });

            veiculos.AddRange(from obj in ciot.Veiculo.VeiculosVinculados select new ServicoEFrete.PEF.Veiculo() { Placa = obj.Placa });

            return veiculos.ToArray();
        }

        private string Login(Dominio.Entidades.CIOTSigaFacil ciot)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Repositorio.CIOTSigaFacil repCIOT = new Repositorio.CIOTSigaFacil(_unitOfWork);
            Servicos.Models.Integracao.InspectorBehavior inspector = new Servicos.Models.Integracao.InspectorBehavior();

            try
            {
                ServicoEFrete.Logon.LogonServiceSoapClient svcLogon = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(_unitOfWork).ObterClient<ServicoEFrete.Logon.LogonServiceSoapClient, ServicoEFrete.Logon.LogonServiceSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.EFrete_Logon, out inspector);

                ServicoEFrete.Logon.LoginRequest request = new ServicoEFrete.Logon.LoginRequest()
                {
                    Integrador = ciot.Empresa.Configuracao.CodigoIntegradorEFrete,
                    Senha = ciot.Empresa.Configuracao.SenhaEFrete,
                    Usuario = ciot.Empresa.Configuracao.UsuarioEFrete,
                    Versao = 1
                };

                ServicoEFrete.Logon.LoginResponse retorno = svcLogon.Login(request);

                AdicionaLog(inspector, ciot);

                if (!retorno.Sucesso)
                {
                    ciot.CodigoRetorno = "9";
                    ciot.MensagemRetorno = retorno.Excecao?.Mensagem;

                    repCIOT.Atualizar(ciot);

                    return null;
                }
                else
                {
                    if (retorno.Token != null)
                        return retorno.Token;
                    else
                    {
                        ciot.CodigoRetorno = "9";
                        ciot.MensagemRetorno = "Login e-frete não retornou token de acesso.";
                        if (retorno.Excecao != null && retorno.Excecao.Mensagem != "")
                            ciot.MensagemRetorno = "Login e-frete não retornou token de acesso. " + retorno.Excecao.Mensagem;
                        repCIOT.Atualizar(ciot);

                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "eFrete");

                ciot.CodigoRetorno = "9";
                ciot.MensagemRetorno = "Falha ao tentar efetuar o login.";
                repCIOT.Atualizar(ciot);

                AdicionaLog(inspector, ciot);

                return null;
            }
        }

        private void Logout(Dominio.Entidades.CIOTSigaFacil ciot, string token)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            ServicoEFrete.Logon.LogonServiceSoapClient svcLogon = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(_unitOfWork).ObterClient<ServicoEFrete.Logon.LogonServiceSoapClient, ServicoEFrete.Logon.LogonServiceSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.EFrete_Logon);

            ServicoEFrete.Logon.LogoutRequest request = new ServicoEFrete.Logon.LogoutRequest()
            {
                Integrador = ciot.Empresa.Configuracao.CodigoIntegradorEFrete,
                Token = token,
                Versao = 1
            };

            svcLogon.Logout(request);
        }

        private int PegaDDD(string numero)
        {
            int retorno = 0;
            string[] explodido = numero.Split(' ');

            if (explodido.Length > 1)
                int.TryParse(Utilidades.String.OnlyNumbers(explodido[0]), out retorno);

            return retorno;
        }
        
        private int PegaNumero(string numero)
        {
            int retorno = 0;
            string[] explodido = numero.Split(' ');

            if (explodido.Length >= 2)
                int.TryParse(Utilidades.String.OnlyNumbers(explodido[1]), out retorno);

            return retorno;
        }

        private void AdicionaLog(Servicos.Models.Integracao.InspectorBehavior inspector, Dominio.Entidades.CIOTSigaFacil ciot)
        {
            Repositorio.CIOTSigaFacilLogXML repCIOTSigaFacilLogXML = new Repositorio.CIOTSigaFacilLogXML(_unitOfWork);

            Dominio.Entidades.CIOTSigaFacilLogXML log = new Dominio.Entidades.CIOTSigaFacilLogXML()
            {
                CIOT = ciot,
                DataHora = DateTime.Now,
                Requisicao = inspector.LastRequestXML,
                Resposta = inspector.LastResponseXML
            };

            repCIOTSigaFacilLogXML.Inserir(log);
        }

        #endregion
    }
}
