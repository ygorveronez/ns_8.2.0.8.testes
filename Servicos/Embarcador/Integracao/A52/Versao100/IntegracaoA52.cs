using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Servicos.Embarcador.Integracao.A52.V100
{
    public class IntegracaoA52
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoA52 _configuracaoIntegracao;

        #endregion Atributos Globais

        #region Construtores

        public IntegracaoA52(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoA52 configuracaoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            _configuracaoIntegracao = configuracaoIntegracao;
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Públicos

        public void IntegrarCarga(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            cargaIntegracao.NumeroTentativas += 1;
            cargaIntegracao.DataIntegracao = DateTime.Now;

            if (!PossuiIntegracaoA52(_configuracaoIntegracao))
            {
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = "Não existe configuração de integração disponível para a A52.";
                repCargaIntegracao.Atualizar(cargaIntegracao);
                return;
            }

            HttpClient client = ObterClient(_configuracaoIntegracao);

            if (ObterToken(cargaIntegracao, client, _configuracaoIntegracao) &&
                IntegrarViagem(cargaIntegracao, client, _configuracaoIntegracao, configuracaoTMS))
            {
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                cargaIntegracao.ProblemaIntegracao = "Viagem inserida com sucesso.";
            }

            repCargaIntegracao.Atualizar(cargaIntegracao);
        }

        public void CancelarCarga(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cargaCancelamentoIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao repCargaCancelamentoCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            cargaCancelamentoIntegracao.NumeroTentativas += 1;
            cargaCancelamentoIntegracao.DataIntegracao = DateTime.Now;

            if (!PossuiIntegracaoA52(_configuracaoIntegracao))
            {
                cargaCancelamentoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaCancelamentoIntegracao.ProblemaIntegracao = "Não existe configuração de integração disponível para a A52.";
                repCargaCancelamentoCargaIntegracao.Atualizar(cargaCancelamentoIntegracao);
                return;
            }

            Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao = repCargaIntegracao.BuscarPorCargaETipoIntegracao(cargaCancelamentoIntegracao.CargaCancelamento.Carga.Codigo, cargaCancelamentoIntegracao.TipoIntegracao.Codigo);

            if (cargaIntegracao == null ||
               cargaIntegracao.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao)
            {
                cargaCancelamentoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                cargaCancelamentoIntegracao.ProblemaIntegracao = "Não existe integração realizada com sucesso para a A52 nesta carga.";

                repCargaCancelamentoCargaIntegracao.Atualizar(cargaCancelamentoIntegracao);

                return;
            }

            if (cargaIntegracao.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao ||
                cargaIntegracao.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno)
            {
                cargaCancelamentoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaCancelamentoIntegracao.ProblemaIntegracao = "A integração de viagem não foi concluída na carga, aguarde e reenvie a integração de cancelamento.";

                repCargaCancelamentoCargaIntegracao.Atualizar(cargaCancelamentoIntegracao);

                return;
            }

            if (string.IsNullOrWhiteSpace(cargaIntegracao.Protocolo))
            {
                cargaCancelamentoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaCancelamentoIntegracao.ProblemaIntegracao = "O protocolo de integração da viagem não existe, não sendo possível efetuar o cancelamento.";

                repCargaCancelamentoCargaIntegracao.Atualizar(cargaCancelamentoIntegracao);

                return;
            }

            HttpClient client = ObterClient(_configuracaoIntegracao);

            if (ObterToken(cargaCancelamentoIntegracao, client, _configuracaoIntegracao) &&
                IntegrarCancelamentoViagem(cargaCancelamentoIntegracao, cargaIntegracao.Protocolo, client, _configuracaoIntegracao, configuracaoTMS))
            {
                cargaCancelamentoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                cargaCancelamentoIntegracao.ProblemaIntegracao = "Viagem cancelada com sucesso.";
            }

            repCargaIntegracao.Atualizar(cargaIntegracao);
        }

        public void IntegrarTrocaMotorista(Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao veiculoIntegracao)
        {
            Repositorio.Embarcador.Veiculos.VeiculoIntegracao repVeiculoIntegracao = new Repositorio.Embarcador.Veiculos.VeiculoIntegracao(_unitOfWork);

            veiculoIntegracao.NumeroTentativas += 1;
            veiculoIntegracao.DataIntegracao = DateTime.Now;

            if (!PossuiIntegracaoA52(_configuracaoIntegracao))
            {
                veiculoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                veiculoIntegracao.ProblemaIntegracao = "Não existe configuração de integração disponível para a A52.";
                repVeiculoIntegracao.Atualizar(veiculoIntegracao);
                return;
            }

            HttpClient client = ObterClient(_configuracaoIntegracao);

            if (ObterToken(veiculoIntegracao, client, _configuracaoIntegracao) &&
                IntegrarTrocaMotorista(veiculoIntegracao, client, _configuracaoIntegracao))
            {
                veiculoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                veiculoIntegracao.ProblemaIntegracao = "Registro integrado com sucesso.";
            }

            repVeiculoIntegracao.Atualizar(veiculoIntegracao);
        }

        public void IntegrarCargaDadosTransporte(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(_unitOfWork);

            cargaDadosTransporteIntegracao.NumeroTentativas += 1;
            cargaDadosTransporteIntegracao.DataIntegracao = DateTime.Now;

            if (!PossuiIntegracaoA52(_configuracaoIntegracao))
            {
                cargaDadosTransporteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaDadosTransporteIntegracao.ProblemaIntegracao = "Não existe configuração de integração disponível para a A52.";
                repCargaDadosTransporteIntegracao.Atualizar(cargaDadosTransporteIntegracao);
                return;
            }

            HttpClient client = ObterClient(_configuracaoIntegracao);

            if (ObterToken(cargaDadosTransporteIntegracao, client, _configuracaoIntegracao) &&
                IntegrarCargaDadosTransporte(cargaDadosTransporteIntegracao, client, _configuracaoIntegracao))
            {
                cargaDadosTransporteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                cargaDadosTransporteIntegracao.ProblemaIntegracao = "Registro integrado com sucesso.";
            }

            repCargaDadosTransporteIntegracao.Atualizar(cargaDadosTransporteIntegracao);
        }

        public void IntegrarSituacaoColaborador(Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracao situacaoColaborador, Dominio.Entidades.Veiculo veiculo)
        {
            Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracao repColaboradorSituacaoLancamentoIntegracao = new Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracao(_unitOfWork);

            situacaoColaborador.NumeroTentativas += 1;
            situacaoColaborador.DataIntegracao = DateTime.Now;

            if (!PossuiIntegracaoA52(_configuracaoIntegracao))
            {
                situacaoColaborador.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                situacaoColaborador.ProblemaIntegracao = "Não existe configuração de integração disponível para a A52.";
                repColaboradorSituacaoLancamentoIntegracao.Atualizar(situacaoColaborador);
                return;
            }

            HttpClient client = ObterClient(_configuracaoIntegracao);

            if (ObterToken(situacaoColaborador, client, _configuracaoIntegracao) &&
                IntegrarSituacaoColaborador(situacaoColaborador, veiculo, client, _configuracaoIntegracao))
            {
                situacaoColaborador.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                situacaoColaborador.ProblemaIntegracao = "Registro integrado com sucesso.";
            }

            repColaboradorSituacaoLancamentoIntegracao.Atualizar(situacaoColaborador);
        }

        public void IntegrarPedido(Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao pedidoIntegracao)
        {
            Repositorio.Embarcador.Pedidos.PedidoIntegracao repPedidoIntegracao = new Repositorio.Embarcador.Pedidos.PedidoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            pedidoIntegracao.Tentativas += 1;
            pedidoIntegracao.DataEnvio = DateTime.Now;

            if (!PossuiIntegracaoA52(_configuracaoIntegracao))
            {
                pedidoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                pedidoIntegracao.ProblemaIntegracao = "Não existe configuração de integração disponível para a A52.";
                repPedidoIntegracao.Atualizar(pedidoIntegracao);
                return;
            }

            HttpClient client = ObterClient(_configuracaoIntegracao);

            if (!ObterToken(pedidoIntegracao, client, _configuracaoIntegracao))
            {
                pedidoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                pedidoIntegracao.ProblemaIntegracao = "Problemas na autenticação ao obter o token!";
                repPedidoIntegracao.Atualizar(pedidoIntegracao);
                return;
            }

            if (IntegrarPedido(pedidoIntegracao, client, _configuracaoIntegracao, configuracaoTMS))
            {
                pedidoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                pedidoIntegracao.ProblemaIntegracao = "Pedido incluído/atualizado com sucesso.";

            }

            repPedidoIntegracao.Atualizar(pedidoIntegracao);
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private bool IntegrarCliente(out string codigoCliente, Dominio.Entidades.ParticipanteCTe cliente, Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, HttpClient client, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoA52 configuracaoIntegracao)
        {
            string jsonRequest = null,
                   jsonResponse = null;

            bool sucesso = false;

            codigoCliente = null;

            if (cliente == null)
                return true;

            Dominio.Entidades.ClienteDadosIntegracao clienteDadosIntegracao = null;

            if (cliente.Cliente != null)
                clienteDadosIntegracao = new Repositorio.ClienteDadosIntegracao(_unitOfWork).BuscaPorTipoIntegracao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.A52, cliente.Cliente.CPF_CNPJ);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.A52.Cliente clienteIntegrar = new Dominio.ObjetosDeValor.Embarcador.Integracao.A52.Cliente()
                {
                    ds_bairro = cliente.Bairro,
                    ds_cidade = cliente.Localidade?.Descricao,
                    ds_cnpjcpf = cliente.CPF_CNPJ_SemFormato,
                    ds_endereco = cliente.Endereco,
                    ds_fantasia = cliente.NomeFantasia,
                    ds_nome = cliente.Nome,
                    ds_numero = cliente.Numero,
                    vl_latitude = cliente.Cliente?.Latitude,
                    vl_longitude = cliente.Cliente?.Longitude,
                    cd_cidade_ibge = cliente.Localidade?.CodigoIBGE.ToString(),
                    ds_uf = cliente.Localidade?.Estado?.Sigla
                };

                jsonRequest = JsonConvert.SerializeObject(clienteIntegrar, Formatting.Indented);

                StringContent content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");

                string url = $"{ObterURL(configuracaoIntegracao.URL, "clientes")}{(clienteDadosIntegracao != null ? $"?id={clienteDadosIntegracao.CodigoIntegracao}" : "")}";

                HttpResponseMessage result = clienteDadosIntegracao == null ? client.PostAsync(url, content).Result : client.PutAsync(url, content).Result;

                jsonResponse = result.Content?.ReadAsStringAsync().Result;

                dynamic retorno = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

                if (result.IsSuccessStatusCode && retorno != null)
                {
                    if (!string.IsNullOrWhiteSpace((string)retorno.cd_cliente))
                    {
                        codigoCliente = (string)retorno.cd_cliente;

                        if (clienteDadosIntegracao == null)
                            AdicionarDadosIntegracaoCliente(cliente.Cliente, codigoCliente);

                        cargaIntegracao.ProblemaIntegracao = "Cliente cadastrado com sucesso.";

                        sucesso = true;
                    }
                    else if (!string.IsNullOrWhiteSpace((string)retorno.message))
                    {
                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                        cargaIntegracao.ProblemaIntegracao = (string)retorno.message;
                    }
                    else
                    {
                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                        cargaIntegracao.ProblemaIntegracao = "Não foi possível cadastrar o cliente, verifique os arquivos de integração.";
                    }
                }
                else
                {
                    cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    cargaIntegracao.ProblemaIntegracao = "Não foi possível cadastrar o cliente, verifique os arquivos de integração.";
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao cadastrar o cliente.";
            }

            SalvarArquivosIntegracao(cargaIntegracao, jsonRequest, jsonResponse);

            return sucesso;
        }

        private void AdicionarDadosIntegracaoCliente(Dominio.Entidades.Cliente cliente, string codigoIntegracao)
        {
            if (cliente == null)
                return;

            Repositorio.ClienteDadosIntegracao repClienteDadosIntegracao = new Repositorio.ClienteDadosIntegracao(_unitOfWork);

            Dominio.Entidades.ClienteDadosIntegracao clienteDadosIntegracao = repClienteDadosIntegracao.BuscaPorTipoIntegracao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.A52, cliente.CPF_CNPJ);

            if (clienteDadosIntegracao == null)
            {
                clienteDadosIntegracao = new Dominio.Entidades.ClienteDadosIntegracao()
                {
                    Cliente = cliente,
                    Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.A52,
                    CodigoIntegracao = codigoIntegracao
                };

                repClienteDadosIntegracao.Inserir(clienteDadosIntegracao);
            }
        }

        private bool IntegrarVeiculo(out string codigoVeiculo, Dominio.Entidades.Veiculo veiculo, Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, HttpClient client, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoA52 configuracaoIntegracao)
        {
            string jsonRequest = null,
                   jsonResponse = null;

            bool sucesso = false;

            codigoVeiculo = null;

            if (veiculo == null)
                return true;

            try
            {
                Repositorio.Embarcador.Veiculos.TecnologiaRastreadorCodigoIntegracao repTecnologiaRastreadorCodigoIntegracao = new Repositorio.Embarcador.Veiculos.TecnologiaRastreadorCodigoIntegracao(_unitOfWork);
                Dominio.Entidades.Embarcador.Veiculos.TecnologiaRastreadorCodigoIntegracao tecnologiaRastreadorCodigoIntegracao = null;

                if (veiculo.PossuiRastreador && veiculo.TecnologiaRastreador != null)
                    tecnologiaRastreadorCodigoIntegracao = repTecnologiaRastreadorCodigoIntegracao.BuscarPorTecnologiaRastreadorETipoIntegracao(veiculo.TecnologiaRastreador, cargaIntegracao.TipoIntegracao);

                Dominio.ObjetosDeValor.Embarcador.Integracao.A52.Veiculo veiculoIntegrar = new Dominio.ObjetosDeValor.Embarcador.Integracao.A52.Veiculo()
                {
                    nr_placa = veiculo.Placa_Formatada,
                    nr_frota = veiculo.NumeroFrota,
                    cd_mct = veiculo.PossuiRastreador ? veiculo.NumeroEquipamentoRastreador : null,
                    cd_tecnologia = tecnologiaRastreadorCodigoIntegracao?.CodigoIntegracao,
                    cd_tipo = veiculo.ModeloVeicularCarga?.TipoVeiculoA52 ?? "1",
                    cd_vinculo = veiculo.Tipo == "T" ? "2" : "1"
                };

                jsonRequest = JsonConvert.SerializeObject(veiculoIntegrar, Formatting.Indented);

                StringContent content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");

                HttpResponseMessage result = client.PostAsync(ObterURL(configuracaoIntegracao.URL, "veiculos"), content).Result;

                jsonResponse = result.Content?.ReadAsStringAsync().Result;

                dynamic retorno = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

                if (result.IsSuccessStatusCode && retorno != null)
                {
                    if (!string.IsNullOrWhiteSpace((string)retorno.cd_veiculo))
                    {
                        codigoVeiculo = (string)retorno.cd_veiculo;

                        cargaIntegracao.ProblemaIntegracao = "Veículo cadastrado com sucesso.";

                        sucesso = true;
                    }
                    else if (!string.IsNullOrWhiteSpace((string)retorno.message))
                    {
                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                        cargaIntegracao.ProblemaIntegracao = (string)retorno.message;
                    }
                    else
                    {
                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                        cargaIntegracao.ProblemaIntegracao = "Não foi possível cadastrar o veículo, verifique os arquivos de integração.";
                    }
                }
                else
                {
                    cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    cargaIntegracao.ProblemaIntegracao = "Não foi possível cadastrar o veículo, verifique os arquivos de integração.";
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao cadastrar o veículo.";
            }

            SalvarArquivosIntegracao(cargaIntegracao, jsonRequest, jsonResponse);

            return sucesso;
        }

        private bool IntegrarMotorista(out string codigoMotorista, Dominio.Entidades.Usuario motorista, Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, HttpClient client, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoA52 configuracaoIntegracao)
        {
            string jsonRequest = null,
                   jsonResponse = null;

            bool sucesso = false;

            codigoMotorista = null;

            if (motorista == null)
                return true;

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.A52.Motorista motoristaIntegrar = new Dominio.ObjetosDeValor.Embarcador.Integracao.A52.Motorista()
                {
                    ds_cpf = motorista.CPF,
                    ds_nome = motorista.Nome,
                    cd_vinculo = motorista.TipoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotorista.Terceiro ? "2" : "1",
                    cd_modalidade = "1"
                };

                jsonRequest = JsonConvert.SerializeObject(motoristaIntegrar, Formatting.Indented);

                StringContent content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");

                HttpResponseMessage result = client.PostAsync(ObterURL(configuracaoIntegracao.URL, "motoristas"), content).Result;

                jsonResponse = result.Content?.ReadAsStringAsync().Result;

                dynamic retorno = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

                if (result.IsSuccessStatusCode && retorno != null)
                {
                    if (!string.IsNullOrWhiteSpace((string)retorno.cd_motorista))
                    {
                        codigoMotorista = (string)retorno.cd_motorista;

                        cargaIntegracao.ProblemaIntegracao = "Motorista cadastrado com sucesso.";

                        sucesso = true;
                    }
                    else if (!string.IsNullOrWhiteSpace((string)retorno.message))
                    {
                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                        cargaIntegracao.ProblemaIntegracao = (string)retorno.message;
                    }
                    else
                    {
                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                        cargaIntegracao.ProblemaIntegracao = "Não foi possível cadastrar o motorista, verifique os arquivos de integração.";
                    }
                }
                else
                {
                    cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    cargaIntegracao.ProblemaIntegracao = "Não foi possível cadastrar o motorista, verifique os arquivos de integração.";
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao cadastrar o motorista.";
            }

            SalvarArquivosIntegracao(cargaIntegracao, jsonRequest, jsonResponse);

            return sucesso;
        }

        private bool IntegrarViagem(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, HttpClient client, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoA52 configuracaoIntegracao, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS)
        {
            string jsonRequest = null,
                   jsonResponse = null;

            bool sucesso = false;

            try
            {
                Repositorio.Embarcador.Cargas.CargaRotaFrete repCargaRotaFrete = new Repositorio.Embarcador.Cargas.CargaRotaFrete(_unitOfWork);

                string codigoVeiculo = null;
                List<string> codigosCarretas = new List<string>();
                List<string> codigosMotoristas = new List<string>();

                if (cargaIntegracao.Carga.Veiculo != null && !IntegrarVeiculo(out codigoVeiculo, cargaIntegracao.Carga.Veiculo, cargaIntegracao, client, configuracaoIntegracao))
                    return false;

                foreach (Dominio.Entidades.Veiculo carreta in cargaIntegracao.Carga.VeiculosVinculados)
                {
                    if (!IntegrarVeiculo(out string codigoCarreta, carreta, cargaIntegracao, client, configuracaoIntegracao))
                        return false;
                    else
                        codigosCarretas.Add(codigoCarreta);
                }

                foreach (Dominio.Entidades.Usuario motorista in cargaIntegracao.Carga.Motoristas)
                {
                    if (!IntegrarMotorista(out string codigoMotorista, motorista, cargaIntegracao, client, configuracaoIntegracao))
                        return false;
                    else
                        codigosMotoristas.Add(codigoMotorista);
                }

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = null;

                if (configuracaoIntegracao.UtilizarDataAgendamentoPedido)
                    cargaPedidos = cargaIntegracao.Carga.Pedidos.OrderBy(o => o.Pedido.DataAgendamento).ToList();
                else
                    cargaPedidos = cargaIntegracao.Carga.Pedidos.OrderBy(o => o.Pedido.DataPrevisaoSaida).ToList();

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoPrimeiro = cargaPedidos.FirstOrDefault();
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoUltimo = cargaPedidos.LastOrDefault();

                int tempoEntrega = cargaIntegracao.Carga.TipoOperacao?.TempoEntregaA52 ?? 0;

                if (tempoEntrega <= 0)
                    tempoEntrega = 120;

                DateTime? dataAgendamento = cargaPedidos.Max(o => o.Pedido.DataAgendamento);
                DateTime? dataPrevisaoInicio = cargaPedidoPrimeiro.Pedido.DataPrevisaoSaida;

                Dominio.ObjetosDeValor.Embarcador.Integracao.A52.Viagem viagem = new Dominio.ObjetosDeValor.Embarcador.Integracao.A52.Viagem()
                {
                    cd_carreta1 = codigosCarretas.Count > 0 ? codigosCarretas[0] : null,
                    cd_carreta2 = codigosCarretas.Count > 1 ? codigosCarretas[1] : null,
                    cd_carreta3 = codigosCarretas.Count > 2 ? codigosCarretas[2] : null,
                    cd_motorista1 = codigosMotoristas.Count > 0 ? codigosMotoristas[0] : null,
                    cd_motorista2 = codigosMotoristas.Count > 1 ? codigosMotoristas[1] : null,
                    cd_veiculo = codigoVeiculo,
                    cd_viagem_cliente = cargaIntegracao.Carga.Codigo.ToString(),
                    nr_carga = cargaIntegracao.Carga.CodigoCargaEmbarcador,
                    nr_pedido = cargaPedidoPrimeiro?.Pedido.Numero.ToString(),
                    cidade_destino_ibge = cargaPedidoUltimo.Destino.CodigoIBGE.ToString(),
                    cidade_origem_ibge = cargaPedidoPrimeiro.Origem.CodigoIBGE.ToString(),
                    dt_prev_chegada = dataAgendamento?.ToString("yyyy-MM-ddTHH:mm:ss"),
                    dt_ini_prev = dataPrevisaoInicio?.ToString("yyyy-MM-ddTHH:mm:ss"),
                    uf_destino = cargaPedidoUltimo.Destino.Estado.Sigla,
                    uf_origem = cargaPedidoPrimeiro.Origem.Estado.Sigla,
                    vl_frete = cargaIntegracao.Carga.ValorFreteAPagar,
                    cd_tipo = cargaIntegracao.Carga.TipoOperacao.TipoA52.ToString(),
                    km_percurso = cargaIntegracao.Carga.DadosSumarizados?.Distancia.ToString() ?? string.Empty,
                    cd_tipo_operacao = cargaIntegracao.Carga.TipoOperacao.TipoOperacaoA52.ToString(),
                    entregas = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.A52.ViagemEntrega>(),
                };

                Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete = repCargaRotaFrete.BuscarPorCarga(cargaIntegracao.Carga.Codigo);

                if (cargaIntegracao.Carga.Rota != null && cargaRotaFrete != null && !string.IsNullOrWhiteSpace(cargaRotaFrete.PolilinhaRota))
                {
                    viagem.rota = new Dominio.ObjetosDeValor.Embarcador.Integracao.A52.ViagemRota()
                    {
                        cd_identificador = cargaIntegracao.Carga.Rota.Codigo.ToString(),
                        polilinha = cargaRotaFrete.PolilinhaRota,
                        cd_tipo_rota = "1"
                    };
                }

                DateTime? dataPrevisaoChegada = null;

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in cargaIntegracao.Carga.CargaCTes)
                {
                    Dominio.Entidades.ParticipanteCTe clienteRemetente = cargaCTe.CTe.Remetente;
                    Dominio.Entidades.ParticipanteCTe clienteDestinatario = cargaCTe.CTe.Destinatario;
                    Dominio.Entidades.ParticipanteCTe clienteExpedidor = cargaCTe.CTe.Expedidor;
                    Dominio.Entidades.ParticipanteCTe clienteRecebedor = cargaCTe.CTe.Recebedor;

                    string codigoClienteExpedidor = null, codigoClienteRecebedor = null;

                    if (!IntegrarCliente(out string codigoClienteRemetente, clienteRemetente, cargaIntegracao, client, configuracaoIntegracao))
                        return false;

                    if (!IntegrarCliente(out string codigoClienteDestinatario, clienteDestinatario, cargaIntegracao, client, configuracaoIntegracao))
                        return false;

                    if (clienteExpedidor != null && !IntegrarCliente(out codigoClienteExpedidor, clienteExpedidor, cargaIntegracao, client, configuracaoIntegracao))
                        return false;

                    if (clienteRecebedor != null && !IntegrarCliente(out codigoClienteRecebedor, clienteRecebedor, cargaIntegracao, client, configuracaoIntegracao))
                        return false;


                    if (configuracaoIntegracao.AplicarRegraLocalPalletizacao)
                    {

                        if(cargaCTe.CTe.Recebedor?.CPF_CNPJ_SemFormato != null)
                        {
                            codigoClienteRecebedor = cargaCTe.CTe.Recebedor.CPF_CNPJ_SemFormato.ToString();
                        }
                        else if(cargaCTe.NotasFiscais.Any())
                        {

                            codigoClienteRecebedor = cargaCTe.NotasFiscais.FirstOrDefault()?.PedidoXMLNotaFiscal?.CargaPedido?.Pedido?.LocalPaletizacao?.CPF_CNPJ_SemFormato.ToString() ?? string.Empty;
                        }                        
                      
                    }

                    DateTime? dataPrevisaoChegadaPedido = cargaCTe.NotasFiscais.Select(o => o.PedidoXMLNotaFiscal.CargaPedido.Pedido.DataAgendamento).FirstOrDefault();

                    if (dataPrevisaoChegadaPedido.HasValue && (!dataPrevisaoChegada.HasValue || dataPrevisaoChegada < dataPrevisaoChegadaPedido))
                        dataPrevisaoChegada = dataPrevisaoChegadaPedido;

                    Dominio.ObjetosDeValor.Embarcador.Integracao.A52.ViagemEntrega entrega = new Dominio.ObjetosDeValor.Embarcador.Integracao.A52.ViagemEntrega()
                    {
                        cd_cliente_origem = codigoClienteRemetente,
                        cd_cliente_destino = codigoClienteDestinatario,
                        cd_cliente_expedidor = codigoClienteExpedidor,
                        cd_cliente_recebedor = codigoClienteRecebedor,
                        cd_tipo_carga = cargaIntegracao.Carga.TipoOperacao.TipoCargaA52.ToString(),
                        m3_cte = cargaCTe.CTe.MetrosCubicos,
                        nr_cte = cargaCTe.CTe.Numero,
                        serie_cte = cargaCTe.CTe.Serie.Numero.ToString(),
                        peso_cte = cargaCTe.CTe.Peso,
                        dt_prev_chegada = dataPrevisaoChegadaPedido?.ToString("yyyy-MM-ddTHH:mm:ss"),
                        notasfiscais = cargaCTe.CTe.Documentos.Select(notaFiscal => new Dominio.ObjetosDeValor.Embarcador.Integracao.A52.ViagemEntregaNotaFiscal()
                        {
                            nr_notafiscal = notaFiscal.NumeroOuNumeroDaChave,
                            ds_serie = notaFiscal.SerieOuSerieDaChave
                        }).ToList()
                    };

                    viagem.entregas.Add(entrega);
                }

                if (dataPrevisaoChegada.HasValue)
                {
                    dataPrevisaoChegada = dataPrevisaoChegada.Value.AddMinutes(tempoEntrega);

                    viagem.dt_fim_prev = dataPrevisaoChegada.Value.ToString("yyyy-MM-ddTHH:mm:ss");
                }

                if (cargaIntegracao.Carga.TipoOperacao?.IntegrarPedidoA52 ?? false)
                {
                    viagem.cargas = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.A52.Carga>();

                    Repositorio.Embarcador.Pedidos.PedidoIntegracao repPedidoIntegracao = new Repositorio.Embarcador.Pedidos.PedidoIntegracao(_unitOfWork);
                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao> pedidosIntegracao = repPedidoIntegracao.BuscarIntegracoesPorPedidos(cargaIntegracao.Carga.Pedidos.Select(o => o.Pedido.Codigo).ToList());

                    foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao pedidoIntegracao in pedidosIntegracao)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Integracao.A52.Carga carga = new Dominio.ObjetosDeValor.Embarcador.Integracao.A52.Carga
                        {
                            cd_carga = pedidoIntegracao.CodigoIntegracaoIntegradora ?? string.Empty,
                        };

                        viagem.cargas.Add(carga);
                    }
                }

                jsonRequest = JsonConvert.SerializeObject(viagem, Formatting.Indented);

                StringContent content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");

                HttpResponseMessage result = client.PostAsync(ObterURL(configuracaoIntegracao.URL, "viagens"), content).Result;

                jsonResponse = result.Content?.ReadAsStringAsync().Result;

                dynamic retorno = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

                if (result.IsSuccessStatusCode && retorno != null)
                {
                    if (!string.IsNullOrWhiteSpace((string)retorno.message))
                    {
                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                        cargaIntegracao.ProblemaIntegracao = (string)retorno.message;
                    }
                    else if (!string.IsNullOrWhiteSpace((string)retorno.cd_viagem))
                    {
                        sucesso = true;

                        cargaIntegracao.Protocolo = (string)retorno.cd_viagem;
                        cargaIntegracao.ProblemaIntegracao = "Viagem inserida com sucesso.";
                    }
                    else
                    {
                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                        cargaIntegracao.ProblemaIntegracao = "Não foi possível inserir a viagem, verifique os arquivos de integração.";
                    }
                }
                else
                {
                    cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    cargaIntegracao.ProblemaIntegracao = "Não foi possível inserir a viagem, verifique os arquivos de integração.";
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao inserir a viagem.";
            }

            SalvarArquivosIntegracao(cargaIntegracao, jsonRequest, jsonResponse);

            return sucesso;
        }

        private bool IntegrarCancelamentoViagem(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cargaCancelamentoIntegracao, string protocolo, HttpClient client, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoA52 configuracaoIntegracao, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS)
        {
            string jsonRequest = null,
                   jsonResponse = null;

            bool sucesso = false;

            try
            {
                StringContent content = new StringContent("", Encoding.UTF8, "application/json");
                HttpResponseMessage result = client.PutAsync(ObterURL(configuracaoIntegracao.URL, "cancelar_viagem") + $"?id={protocolo}", content).Result;

                jsonResponse = result.Content?.ReadAsStringAsync().Result;

                dynamic retorno = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

                if (result.IsSuccessStatusCode && retorno != null)
                {
                    string mensagem = (string)retorno.message;

                    if (mensagem == "Viagem cancelada com sucesso")
                    {
                        sucesso = true;

                        cargaCancelamentoIntegracao.ProblemaIntegracao = "Viagem cancelada com sucesso.";
                    }
                    else if (!string.IsNullOrWhiteSpace(mensagem))
                    {
                        cargaCancelamentoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                        cargaCancelamentoIntegracao.ProblemaIntegracao = mensagem;
                    }
                    else
                    {
                        cargaCancelamentoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                        cargaCancelamentoIntegracao.ProblemaIntegracao = "Não foi possível cancelar a viagem, verifique os arquivos de integração.";
                    }
                }
                else
                {
                    cargaCancelamentoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    cargaCancelamentoIntegracao.ProblemaIntegracao = "Não foi possível cancelar a viagem, verifique os arquivos de integração.";
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                cargaCancelamentoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaCancelamentoIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao cancelar a viagem.";
            }

            SalvarArquivosIntegracao(cargaCancelamentoIntegracao, jsonRequest, jsonResponse);

            return sucesso;
        }

        private bool IntegrarTrocaMotorista(Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao veiculoIntegracao, HttpClient client, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoA52 configuracaoIntegracao)
        {
            string jsonRequest = null,
                   jsonResponse = null;

            bool sucesso = false;

            try
            {
                string mensagem = IntegrarMacros(veiculoIntegracao.Veiculo, veiculoIntegracao.TipoIntegracao, client, configuracaoIntegracao, out jsonRequest, out jsonResponse);

                veiculoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                veiculoIntegracao.ProblemaIntegracao = mensagem;

                sucesso = true;
            }
            catch (ServicoException ex)
            {
                veiculoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                veiculoIntegracao.ProblemaIntegracao = ex.Message;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                veiculoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                veiculoIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração.";
            }

            SalvarArquivosIntegracao(veiculoIntegracao, jsonRequest, jsonResponse);

            return sucesso;
        }

        private bool IntegrarCargaDadosTransporte(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao, HttpClient client, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoA52 configuracaoIntegracao)
        {
            string jsonRequest = null,
                   jsonResponse = null;

            bool sucesso = false;

            try
            {
                string mensagem = IntegrarMacros(cargaDadosTransporteIntegracao.Carga.Veiculo, cargaDadosTransporteIntegracao.TipoIntegracao, client, configuracaoIntegracao, out jsonRequest, out jsonResponse);

                cargaDadosTransporteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                cargaDadosTransporteIntegracao.ProblemaIntegracao = mensagem;

                sucesso = true;
            }
            catch (ServicoException ex)
            {
                cargaDadosTransporteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaDadosTransporteIntegracao.ProblemaIntegracao = ex.Message;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                cargaDadosTransporteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaDadosTransporteIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração.";
            }

            SalvarArquivosIntegracao(cargaDadosTransporteIntegracao, jsonRequest, jsonResponse);

            return sucesso;
        }

        public string IntegrarMacros(Dominio.Entidades.Veiculo veiculo, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao, HttpClient client, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoA52 configuracaoIntegracao, out string jsonRequest, out string jsonResponse)
        {
            Repositorio.Embarcador.Veiculos.TecnologiaRastreadorCodigoIntegracao repTecnologiaRastreadorCodigoIntegracao = new Repositorio.Embarcador.Veiculos.TecnologiaRastreadorCodigoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(_unitOfWork);

            Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista veiculoMotorista = repVeiculoMotorista.BuscarVeiculoMotoristaPrincipal(veiculo.Codigo);

            if (veiculoMotorista == null || veiculoMotorista.Motorista == null)
                throw new ServicoException("Nenhum motorista vinculado ao veículo.");

            Dominio.Entidades.Embarcador.Veiculos.TecnologiaRastreadorCodigoIntegracao tecnologiaRastreadorCodigoIntegracao = null;

            if (veiculo.PossuiRastreador && veiculo.TecnologiaRastreador != null)
                tecnologiaRastreadorCodigoIntegracao = repTecnologiaRastreadorCodigoIntegracao.BuscarPorTecnologiaRastreadorETipoIntegracao(veiculo.TecnologiaRastreador, tipoIntegracao);

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.A52.Macros> macrosIntegrar = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.A52.Macros>() {
                    new Dominio.ObjetosDeValor.Embarcador.Integracao.A52.Macros() {
                        tecnologia = tecnologiaRastreadorCodigoIntegracao?.CodigoIntegracao,
                        veiculo = veiculo.NumeroEquipamentoRastreador,
                        datahora = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        latitude = "0.0",
                        longitude = "0.0",
                        codigomacro = "99",
                        descricaomacro = Utilidades.String.OnlyNumbers(veiculoMotorista.Motorista.CPF)
                    }
                };

            jsonRequest = JsonConvert.SerializeObject(macrosIntegrar, Formatting.Indented);

            StringContent content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");

            bool urlNova = !string.IsNullOrWhiteSpace(configuracaoIntegracao.URLNova);
            string url = urlNova ? configuracaoIntegracao.URLNova : configuracaoIntegracao.URL;

            HttpResponseMessage result = client.PostAsync(ObterURL(url, "macros"), content).Result;

            jsonResponse = result.Content?.ReadAsStringAsync().Result;

            dynamic retorno = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

            if (result.IsSuccessStatusCode && retorno != null)
            {
                if (urlNova)
                    return "Macro enviado com sucesso";
                else if (!string.IsNullOrWhiteSpace((string)retorno.message) && retorno.message == "Macro inserida")
                    return (string)retorno.message;
                else
                    throw new ServicoException("Não foi possível realizar a integração, verifique os arquivos de integração.");
            }
            else
                throw new ServicoException("Não foi possível realizar a integração, verifique os arquivos de integração.");
        }

        private string IntegrarOcorrencia(Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracao situacaoColaborador, Dominio.Entidades.Veiculo veiculo, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao, HttpClient client, string url, out string jsonRequest, out string jsonResponse)
        {
            Repositorio.Embarcador.Veiculos.TecnologiaRastreadorCodigoIntegracao repTecnologiaRastreadorCodigoIntegracao = new Repositorio.Embarcador.Veiculos.TecnologiaRastreadorCodigoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(_unitOfWork);

            if (situacaoColaborador.ColaboradorLancamento == null || situacaoColaborador.ColaboradorLancamento.Colaborador == null || situacaoColaborador.ColaboradorLancamento.ColaboradorSituacao == null || string.IsNullOrWhiteSpace(situacaoColaborador.ColaboradorLancamento.ColaboradorSituacao.CodigoIntegracao))
                throw new ServicoException("Não foi informado a situação para a geração da ocorrência.");

            Dominio.ObjetosDeValor.Embarcador.Integracao.A52.Ocorrencias ocorrencias = new Dominio.ObjetosDeValor.Embarcador.Integracao.A52.Ocorrencias()
            {
                cpf = situacaoColaborador.ColaboradorLancamento.Colaborador.CPF,
                placa = veiculo?.Placa ?? "",
                status = situacaoColaborador.ColaboradorLancamento.SituacaoLancamento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoColaborador.Cancelado ? "239" : situacaoColaborador.ColaboradorLancamento.ColaboradorSituacao.CodigoIntegracao,
                data_inicial = situacaoColaborador.ColaboradorLancamento.DataInicial.ToString("yyyy-MM-dd HH:mm:ss"),
                data_final = situacaoColaborador.ColaboradorLancamento.DataFinal.ToString("yyyy-MM-dd HH:mm:ss")
            };

            jsonRequest = JsonConvert.SerializeObject(ocorrencias, Formatting.Indented);

            StringContent content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");

            HttpResponseMessage result = client.PostAsync(ObterURL(url, "ocorrencias"), content).Result;

            jsonResponse = result.Content?.ReadAsStringAsync().Result;

            dynamic retorno = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

            if (result.IsSuccessStatusCode)
                return "Situação do motorista integrado com sucesso.";
            else
                throw new ServicoException("Não foi possível realizar a integração, verifique os arquivos de integração. Serviço da A52 retornou falha.");
        }

        private bool IntegrarSituacaoColaborador(Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracao situacaoColaborador, Dominio.Entidades.Veiculo veiculo, HttpClient client, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoA52 configuracaoIntegracao)
        {
            string jsonRequest = null,
                   jsonResponse = null;

            bool sucesso = false;

            try
            {
                string mensagem = IntegrarOcorrencia(situacaoColaborador, veiculo, situacaoColaborador.TipoIntegracao, client, configuracaoIntegracao.URL, out jsonRequest, out jsonResponse);

                situacaoColaborador.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                situacaoColaborador.ProblemaIntegracao = mensagem;

                sucesso = true;
            }
            catch (ServicoException ex)
            {
                situacaoColaborador.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                situacaoColaborador.ProblemaIntegracao = ex.Message;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                situacaoColaborador.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                situacaoColaborador.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração.";
            }

            SalvarArquivosIntegracao(situacaoColaborador, jsonRequest, jsonResponse);

            return sucesso;
        }

        private bool IntegrarPedido(Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao pedidoIntegracao, HttpClient client, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoA52 configuracaoIntegracao, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS)
        {
            bool primeiroEnvio = string.IsNullOrWhiteSpace(pedidoIntegracao.CodigoIntegracaoIntegradora);
            string status;
            string servico;

            if (primeiroEnvio)
            {
                status = "6";
                servico = "coleta";
            }
            else
            {
                status = "7";
                servico = $"coleta?ID={pedidoIntegracao.CodigoIntegracaoIntegradora} ";
            }

            string jsonRequest = null, jsonResponse = null;
            Dominio.ObjetosDeValor.Embarcador.Integracao.A52.ColetaPedido coletaPedido = ObterPedido(pedidoIntegracao.Pedido, status);

            try
            {
                jsonRequest = JsonConvert.SerializeObject(coletaPedido, Formatting.Indented);
                StringContent content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage result = client.PostAsync(ObterURL(configuracaoIntegracao.URLNova, servico), content).Result;

                jsonResponse = result.Content?.ReadAsStringAsync().Result;
                Dominio.ObjetosDeValor.Embarcador.Integracao.A52.ColetaPedidoRetorno retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.A52.ColetaPedidoRetorno>(jsonResponse);

                if (!primeiroEnvio)
                {
                    dynamic retorno2 = JsonConvert.DeserializeObject<dynamic>(jsonResponse);
                    if (retorno2 == null)
                    {
                        return true;//Simplesmente o request de exemplo da especificação técnica não faz muito sentido.. Quando enviado com o id da carga na segunda vez para atualizar, retorna "null", segundo o manual isso é sucesso...
                    }
                }

                if (result.IsSuccessStatusCode && retorno == null)
                    return false;

                if (primeiroEnvio)
                    pedidoIntegracao.CodigoIntegracaoIntegradora = retorno.Carga;

            }
            catch (ServicoException ex)
            {
                pedidoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                pedidoIntegracao.ProblemaIntegracao = ex.Message;
                SalvarArquivosIntegracao(pedidoIntegracao, jsonRequest, jsonResponse);
                return false;

            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                pedidoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                pedidoIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração do pedido com A52";
                SalvarArquivosIntegracao(pedidoIntegracao, jsonRequest, jsonResponse);
                return false;
            }

            SalvarArquivosIntegracao(pedidoIntegracao, jsonRequest, jsonResponse);

            return true;
        }

        #endregion Métodos Privados

        #region Métodos Privados - Obter Token

        private bool ObterToken(Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracao situacaoColaborador, HttpClient client, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoA52 configuracaoIntegracao)
        {
            string jsonRequest = null,
                   jsonResponse = null;

            bool sucesso = false;

            try
            {
                string mensagem = ObterToken(client, configuracaoIntegracao, out jsonRequest, out jsonResponse);

                situacaoColaborador.ProblemaIntegracao = mensagem;
                sucesso = true;
            }
            catch (ServicoException ex)
            {
                situacaoColaborador.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                situacaoColaborador.ProblemaIntegracao = ex.Message;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                situacaoColaborador.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                situacaoColaborador.ProblemaIntegracao = "Ocorreu uma falha ao efetuar o login.";
            }

            SalvarArquivosIntegracao(situacaoColaborador, jsonRequest, jsonResponse);

            return sucesso;
        }

        private bool ObterToken(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, HttpClient client, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoA52 configuracaoIntegracao)
        {
            string jsonRequest = null,
                   jsonResponse = null;

            bool sucesso = false;

            try
            {
                string mensagem = ObterToken(client, configuracaoIntegracao, out jsonRequest, out jsonResponse);

                cargaIntegracao.ProblemaIntegracao = mensagem;
                sucesso = true;
            }
            catch (ServicoException ex)
            {
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = ex.Message;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao efetuar o login.";
            }

            SalvarArquivosIntegracao(cargaIntegracao, jsonRequest, jsonResponse);

            return sucesso;
        }

        private bool ObterToken(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cargaCancelamentoIntegracao, HttpClient client, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoA52 configuracaoIntegracao)
        {
            string jsonRequest = null,
                   jsonResponse = null;

            bool sucesso = false;

            try
            {
                string mensagem = ObterToken(client, configuracaoIntegracao, out jsonRequest, out jsonResponse);

                cargaCancelamentoIntegracao.ProblemaIntegracao = mensagem;
                sucesso = true;
            }
            catch (ServicoException ex)
            {
                cargaCancelamentoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaCancelamentoIntegracao.ProblemaIntegracao = ex.Message;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                cargaCancelamentoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaCancelamentoIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao efetuar o login.";
            }

            SalvarArquivosIntegracao(cargaCancelamentoIntegracao, jsonRequest, jsonResponse);

            return sucesso;
        }

        private bool ObterToken(Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao veiculoIntegracao, HttpClient client, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoA52 configuracaoIntegracao)
        {
            string jsonRequest = null,
                   jsonResponse = null;

            bool sucesso = false;

            try
            {
                string mensagem = ObterToken(client, configuracaoIntegracao, out jsonRequest, out jsonResponse);

                veiculoIntegracao.ProblemaIntegracao = mensagem;
                sucesso = true;
            }
            catch (ServicoException ex)
            {
                veiculoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                veiculoIntegracao.ProblemaIntegracao = ex.Message;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                veiculoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                veiculoIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao efetuar o login.";
            }

            SalvarArquivosIntegracao(veiculoIntegracao, jsonRequest, jsonResponse);

            return sucesso;
        }

        private bool ObterToken(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao, HttpClient client, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoA52 configuracaoIntegracao)
        {
            string jsonRequest = null,
                   jsonResponse = null;

            bool sucesso = false;

            try
            {
                string mensagem = ObterToken(client, configuracaoIntegracao, out jsonRequest, out jsonResponse);

                cargaDadosTransporteIntegracao.ProblemaIntegracao = mensagem;
                sucesso = true;
            }
            catch (ServicoException ex)
            {
                cargaDadosTransporteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaDadosTransporteIntegracao.ProblemaIntegracao = ex.Message;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                cargaDadosTransporteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaDadosTransporteIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao efetuar o login.";
            }

            SalvarArquivosIntegracao(cargaDadosTransporteIntegracao, jsonRequest, jsonResponse);

            return sucesso;
        }

        private bool ObterToken(Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao pedidoIntegracao, HttpClient client, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoA52 configuracaoIntegracao)
        {
            string jsonRequest = null,
                   jsonResponse = null;

            bool sucesso = false;

            try
            {
                string mensagem = ObterToken(client, configuracaoIntegracao, out jsonRequest, out jsonResponse);

                pedidoIntegracao.ProblemaIntegracao = mensagem;
                sucesso = true;
            }
            catch (ServicoException ex)
            {
                pedidoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                pedidoIntegracao.ProblemaIntegracao = ex.Message;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                pedidoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                pedidoIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao efetuar o login.";
            }

            SalvarArquivosIntegracao(pedidoIntegracao, jsonRequest, jsonResponse);

            return sucesso;
        }

        private string ObterToken(HttpClient client, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoA52 configuracaoIntegracao, out string jsonRequest, out string jsonResponse)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.A52.Login login = new Dominio.ObjetosDeValor.Embarcador.Integracao.A52.Login()
            {
                CpfCnpj = configuracaoIntegracao.CPFCNPJ,
                Senha = configuracaoIntegracao.Senha
            };

            jsonRequest = JsonConvert.SerializeObject(login, Formatting.Indented);

            StringContent content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");

            //string url = !string.IsNullOrWhiteSpace(configuracaoIntegracao.URLNova) ? configuracaoIntegracao.URLNova : configuracaoIntegracao.URL;

            HttpResponseMessage result = client.PostAsync(ObterURL(configuracaoIntegracao.URL, "login"), content).Result;

            jsonResponse = result.Content?.ReadAsStringAsync().Result;

            dynamic retorno = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

            if (result.IsSuccessStatusCode && retorno != null)
            {
                string token = (string)retorno.token;

                if (!string.IsNullOrWhiteSpace(token))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    return "Login efetuado com sucesso.";
                }
                else
                    throw new ServicoException("Token não retornado, verifique os arquivos de integração.");
            }
            else
                throw new ServicoException("Não foi possível realizar o login, verifique os arquivos de integração.");
        }

        #endregion Métodos Privados - Obter Token

        #region Métodos Privados - Configurações

        private HttpClient ObterClient(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoA52 configuracaoIntegracao)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoA52));

            client.BaseAddress = new Uri(configuracaoIntegracao.URL);
            client.Timeout = TimeSpan.FromMinutes(5);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }

        private string ObterURL(string urlBase, string servico)
        {
            if (string.IsNullOrWhiteSpace(urlBase))
                return urlBase;

            if (!urlBase.EndsWith("/"))
                urlBase += "/";

            urlBase += servico + "/";

            return urlBase;
        }

        #endregion Métodos Privados - Configurações

        #region Métodos Privados - Salvar Arquivos

        private void SalvarArquivosIntegracao(Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracao situacaoColaborador, string jsonRequisicao, string jsonRetorno)
        {
            Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracaoArquivo arquivoIntegracao = AdicionarArquivoTransacaoSituacao(situacaoColaborador, jsonRequisicao, jsonRetorno, situacaoColaborador.ProblemaIntegracao);

            if (arquivoIntegracao == null)
                return;

            //if (situacaoColaborador.ArquivosTransacao == null)
            //    situacaoColaborador.ArquivosTransacao = new List<Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracaoArquivo>();

            //situacaoColaborador.ArquivosTransacao.Add(arquivoIntegracao);
        }

        private void SalvarArquivosIntegracao(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, string jsonRequisicao, string jsonRetorno)
        {
            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = AdicionarArquivoTransacao(jsonRequisicao, jsonRetorno, cargaIntegracao.ProblemaIntegracao);

            if (arquivoIntegracao == null)
                return;

            if (cargaIntegracao.ArquivosTransacao == null)
                cargaIntegracao.ArquivosTransacao = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();

            cargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
        }

        private void SalvarArquivosIntegracao(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cargaCancelamentoIntegracao, string jsonRequisicao, string jsonRetorno)
        {
            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = AdicionarArquivoTransacao(jsonRequisicao, jsonRetorno, cargaCancelamentoIntegracao.ProblemaIntegracao);

            if (arquivoIntegracao == null)
                return;

            if (cargaCancelamentoIntegracao.ArquivosTransacao == null)
                cargaCancelamentoIntegracao.ArquivosTransacao = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();

            cargaCancelamentoIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
        }

        private void SalvarArquivosIntegracao(Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao veiculoIntegracao, string jsonRequisicao, string jsonRetorno)
        {
            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = AdicionarArquivoTransacao(jsonRequisicao, jsonRetorno, veiculoIntegracao.ProblemaIntegracao);

            if (arquivoIntegracao == null)
                return;

            if (veiculoIntegracao.ArquivosTransacao == null)
                veiculoIntegracao.ArquivosTransacao = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();

            veiculoIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
        }

        private void SalvarArquivosIntegracao(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao, string jsonRequisicao, string jsonRetorno)
        {
            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = AdicionarArquivoTransacao(jsonRequisicao, jsonRetorno, cargaDadosTransporteIntegracao.ProblemaIntegracao);

            if (arquivoIntegracao == null)
                return;

            if (cargaDadosTransporteIntegracao.ArquivosTransacao == null)
                cargaDadosTransporteIntegracao.ArquivosTransacao = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();

            cargaDadosTransporteIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
        }

        private void SalvarArquivosIntegracao(Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao pedidoIntegracao, string jsonRequisicao, string jsonRetorno)
        {
            Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracaoArquivo arquivoIntegracao = AdicionarArquivoTransacaoPedido(jsonRequisicao, jsonRetorno, pedidoIntegracao.ProblemaIntegracao);

            if (arquivoIntegracao == null)
                return;

            if (pedidoIntegracao.ArquivosTransacao == null)
                pedidoIntegracao.ArquivosTransacao = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracaoArquivo>();

            pedidoIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
        }

        private Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracaoArquivo AdicionarArquivoTransacaoSituacao(Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracao colaboradorSituacaoLancamentoIntegracao, string jsonRequisicao, string jsonRetorno, string mensagem)
        {
            if (string.IsNullOrWhiteSpace(jsonRequisicao) && string.IsNullOrWhiteSpace(jsonRetorno))
                return null;

            Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracaoArquivo repositorioCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracaoArquivo(_unitOfWork);
            Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracaoArquivo()
            {
                ArquivoRequisicao = ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequisicao, "json", _unitOfWork),
                ArquivoResposta = ArquivoIntegracao.SalvarArquivoIntegracao(jsonRetorno, "json", _unitOfWork),
                Data = DateTime.Now,
                Mensagem = mensagem,
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento,
                ColaboradorSituacaoLancamentoIntegracao = colaboradorSituacaoLancamentoIntegracao
            };

            repositorioCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

            return arquivoIntegracao;
        }

        private Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo AdicionarArquivoTransacao(string jsonRequisicao, string jsonRetorno, string mensagem)
        {
            if (string.IsNullOrWhiteSpace(jsonRequisicao) && string.IsNullOrWhiteSpace(jsonRetorno))
                return null;

            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repositorioCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo()
            {
                ArquivoRequisicao = ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequisicao, "json", _unitOfWork),
                ArquivoResposta = ArquivoIntegracao.SalvarArquivoIntegracao(jsonRetorno, "json", _unitOfWork),
                Data = DateTime.Now,
                Mensagem = mensagem,
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento
            };

            repositorioCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

            return arquivoIntegracao;
        }

        private Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracaoArquivo AdicionarArquivoTransacaoPedido(string jsonRequisicao, string jsonRetorno, string mensagem)
        {
            if (string.IsNullOrWhiteSpace(jsonRequisicao) && string.IsNullOrWhiteSpace(jsonRetorno))
                return null;

            Repositorio.Embarcador.Pedidos.PedidoIntegracaoArquivo repositorioCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Pedidos.PedidoIntegracaoArquivo(_unitOfWork);
            Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracaoArquivo()
            {
                ArquivoRequisicao = ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequisicao, "json", _unitOfWork),
                ArquivoResposta = ArquivoIntegracao.SalvarArquivoIntegracao(jsonRetorno, "json", _unitOfWork),
                Data = DateTime.Now,
                Mensagem = mensagem,
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento
            };

            repositorioCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

            return arquivoIntegracao;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.A52.ColetaPedido ObterPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, string status)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Integracao.A52.ColetaPedido()
            {
                Carreta = pedido.Veiculos.Count > 0 ? string.Join(", ", pedido.Veiculos.Select(o => o.Placa)) : null,
                DataFimPrevisao = pedido.DataPrevisaoChegadaDestinatario?.ToString("yyyy-MM-ddTHH:mm:ss") ?? null,
                DataInicioPrevisao = pedido.DataPrevisaoInicioViagem?.ToString("yyyy-MM-ddTHH:mm:ss") ?? null,
                Destinatario = pedido.Destinatario?.CPF_CNPJ_SemFormato ?? null,
                Embarcador = pedido.Empresa?.CNPJ ?? null,
                Expedidor = pedido.Expedidor?.CPF_CNPJ_SemFormato ?? null,
                Motorista = pedido.Motoristas.Count > 0 ? string.Join(", ", pedido.Motoristas.Select(o => o.CPF_Formatado)) : null,
                Observacao = pedido.ObservacaoInterna ?? null,
                PesoNFe = pedido.PesoTotal,
                QuantidadeEntregas = pedido.QuantidadeNotasFiscais,
                Recebedor = pedido.Recebedor?.CPF_CNPJ_SemFormato ?? null,
                Remetente = pedido.Remetente?.CPF_CNPJ_SemFormato ?? null,
                Rota = ObterPedidoRota(pedido),
                Status = status,
                TipoCarga = pedido.TipoCarga?.Descricao ?? null,
                TipoOperacao = pedido.TipoOperacao?.Descricao ?? null,
                Valor = pedido.ValorTotalNotasFiscais,
                Veiculo = pedido.VeiculoTracao?.Placa ?? null,
                Volume = pedido.QtVolumes
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.A52.ColetaPedidoRota ObterPedidoRota(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Integracao.A52.ColetaPedidoRota()
            {
                Identificador = pedido.RotaFrete?.Descricao ?? null,
                Polilinha = pedido.RotaFrete?.PolilinhaRota ?? null,
                Pontos = pedido.RotaFrete?.PontoPassagemPreDefinido.Count > 0 ? string.Join(", ", pedido.RotaFrete.PontoPassagemPreDefinido.Select(o => o.Descricao)) : null,
                TipoRota = pedido.RotaFrete?.TipoRota.ObterDescricao() ?? null
            };
        }

        #endregion Métodos Privados - Salvar Arquivos

        #region Métodos Privados - Configurações

        private bool PossuiIntegracaoA52(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoA52 configuracaoIntegracao)
        {
            return !(configuracaoIntegracao == null || string.IsNullOrWhiteSpace(configuracaoIntegracao.CPFCNPJ) || string.IsNullOrWhiteSpace(configuracaoIntegracao.Senha) || string.IsNullOrWhiteSpace(configuracaoIntegracao.URLNova));
        }

        #endregion Métodos Privados - Configurações
    }
}
