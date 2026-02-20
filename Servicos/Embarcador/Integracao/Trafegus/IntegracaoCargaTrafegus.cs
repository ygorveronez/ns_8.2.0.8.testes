using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus;
using Newtonsoft.Json;

namespace Servicos.Embarcador.Integracao.Trafegus
{
    public class IntegracaoCargaTrafegus
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;

        #endregion Atributos Globais

        #region Construtores

        public IntegracaoCargaTrafegus(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            _unitOfWork = unitOfWork;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
        }
        #endregion Construtores


        #region Métodos Públicos
        public bool EnviarMotorista(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao)
        {
            if (cargaIntegracao?.Carga == null)
                return false;

            Repositorio.Embarcador.Configuracoes.IntegracaoTrafegus repConfiguracaoIntegracaoTrafegus = new Repositorio.Embarcador.Configuracoes.IntegracaoTrafegus(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrafegus configuracaoIntegracaoTrafegus = repConfiguracaoIntegracaoTrafegus.Buscar();

            string usuario = configuracaoIntegracaoTrafegus.Usuario;
            string senha = configuracaoIntegracaoTrafegus.Senha;

            foreach (Dominio.Entidades.Usuario motoristaCarga in cargaIntegracao.Carga.Motoristas)
            {
                string url = ObterURLMetodo("motorista", motoristaCarga.CPF);

                string retorno = ObterJson(url, usuario, senha);

                dynamic dynRetorno = JsonConvert.DeserializeObject<dynamic>(retorno);

                if (string.IsNullOrEmpty((dynRetorno?.motorista?.ToString() ?? string.Empty)))
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.CadastroMotorista cadastroMotorista = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.CadastroMotorista();
                    cadastroMotorista.motorista = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.Motorista>
                    {
                        new Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.Motorista
                        {
                            cpf_moto = motoristaCarga.CPF,
                            cpf_motorista = motoristaCarga.CPF,
                            nome = motoristaCarga.Nome
                        }
                    };

                    string jsonRequest = JsonConvert.SerializeObject(cadastroMotorista);

                    string urlPost = ObterURLMetodo("motorista", "");

                    string jsonResponse = ExecutarMetodo("POST", usuario, senha, urlPost, jsonRequest);

                    Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.IntegarcaoRetorno integracaoRetorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.IntegarcaoRetorno>(jsonResponse);

                    if (integracaoRetorno?.error?.Count > 0)
                    {
                        string msgErro = "Erro ao enviar motorista";

                        AtualizarSituacaoIntegracao(cargaIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao, msgErro, "", jsonRequest, jsonResponse);

                        return false;
                    }
                }
            }

            return true;
        }

        public bool EnviarVeiculo(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao)
        {
            if (cargaIntegracao?.Carga == null)
                return false;

            Repositorio.Embarcador.Configuracoes.IntegracaoTrafegus repConfiguracaoIntegracaoTrafegus = new Repositorio.Embarcador.Configuracoes.IntegracaoTrafegus(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrafegus configuracaoIntegracaoTrafegus = repConfiguracaoIntegracaoTrafegus.Buscar();

            string usuario = configuracaoIntegracaoTrafegus.Usuario;
            string senha = configuracaoIntegracaoTrafegus.Senha;

            List<Dominio.Entidades.Veiculo> listaVeiculos = new List<Dominio.Entidades.Veiculo>
            {
                cargaIntegracao.Carga.Veiculo
            };

            foreach (Dominio.Entidades.Veiculo reboque in cargaIntegracao.Carga.VeiculosVinculados)
            {
                listaVeiculos.Add(reboque);
            }

            foreach (Dominio.Entidades.Veiculo veiculo in listaVeiculos)
            {
                string url = ObterURLMetodo("veiculo", veiculo.Placa);

                string retorno = ObterJson(url, usuario, senha);

                dynamic dynRetorno = JsonConvert.DeserializeObject<dynamic>(retorno);

                if (string.IsNullOrEmpty((dynRetorno?.veiculo?.ToString() ?? string.Empty)))
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.CadastroVeiculo cadastroVeiculo = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.CadastroVeiculo();
                    cadastroVeiculo.veiculo = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.Veiculo>
                    {
                        new Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.Veiculo
                        {
                            placa = veiculo.Placa,
                            tipo_veiculo = veiculo.Placa == cargaIntegracao.Carga.Veiculo.Placa ? 3 : 1 //3 Cavalo; 1 Carrega
                        }
                    };

                    string jsonRequest = JsonConvert.SerializeObject(cadastroVeiculo);

                    string urlPost = ObterURLMetodo("veiculo", "");

                    string jsonResponse = ExecutarMetodo("POST", usuario, senha, urlPost, jsonRequest);

                    Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.IntegarcaoRetorno integracaoRetorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.IntegarcaoRetorno>(jsonResponse);

                    if (integracaoRetorno?.error?.Count > 0)
                    {
                        string msgErro = "Erro ao enviar veículo";

                        AtualizarSituacaoIntegracao(cargaIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao, msgErro, "", jsonRequest, jsonResponse);

                        return false;
                    }
                }
            }

            return true;
        }

        public bool EnviarTransportador(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao)
        {
            if (cargaIntegracao?.Carga == null)
                return false;

            Repositorio.Embarcador.Configuracoes.IntegracaoTrafegus repConfiguracaoIntegracaoTrafegus = new Repositorio.Embarcador.Configuracoes.IntegracaoTrafegus(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrafegus configuracaoIntegracaoTrafegus = repConfiguracaoIntegracaoTrafegus.Buscar();

            string usuario = configuracaoIntegracaoTrafegus.Usuario;

            string senha = configuracaoIntegracaoTrafegus.Senha;

            string url = ObterURLMetodo("transportador", cargaIntegracao.Carga.Empresa.CNPJ_SemFormato);

            string retorno = ObterJson(url, usuario, senha);

            dynamic dynRetorno = !string.IsNullOrEmpty(retorno) ? JsonConvert.DeserializeObject<dynamic>(retorno) : null;

            if (string.IsNullOrEmpty((dynRetorno?.transportador?.ToString() ?? string.Empty)))
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.CadastroTransportador cadastroTransportador = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.CadastroTransportador();
                cadastroTransportador.transportador = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.Transportador>
                {
                        new Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.Transportador
                        {
                            documento_transportador = cargaIntegracao?.Carga?.Empresa?.CNPJ ?? string.Empty,
                            nome = cargaIntegracao.Carga?.Empresa?.Descricao ?? string.Empty
                        }
                };

                string jsonRequest = JsonConvert.SerializeObject(cadastroTransportador);

                string urlPost = ObterURLMetodo("transportador", "");

                string jsonResponse = ExecutarMetodo("POST", usuario, senha, urlPost, jsonRequest);

                Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.IntegarcaoRetorno integracaoRetorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.IntegarcaoRetorno>(jsonResponse);

                if (integracaoRetorno?.error?.Count > 0)
                {
                    string msgErro = "Erro ao enviar transportador";

                    AtualizarSituacaoIntegracao(cargaIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao, msgErro, "", jsonRequest, jsonResponse);

                    return false;
                }
            }

            return true;
        }

        public Dominio.Entidades.Cliente ObterOrigemCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (carga == null)
                return null;

            Dominio.Entidades.Cliente origem = (from cargaPedido in carga.Pedidos where cargaPedido != null && cargaPedido.Expedidor != null select cargaPedido.Expedidor).FirstOrDefault();

            if (origem != null)
                return origem;

            origem = (from cargaPedido in carga.Pedidos where cargaPedido != null && cargaPedido.Pedido.Remetente != null select cargaPedido.Pedido.Remetente).FirstOrDefault();

            return origem;
        }

        public bool EnviarFilial(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao)
        {
            if (cargaIntegracao?.Carga == null)
                return false;

            Repositorio.Embarcador.Configuracoes.IntegracaoTrafegus repConfiguracaoIntegracaoTrafegus = new Repositorio.Embarcador.Configuracoes.IntegracaoTrafegus(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrafegus configuracaoIntegracaoTrafegus = repConfiguracaoIntegracaoTrafegus.Buscar();

            string usuario = configuracaoIntegracaoTrafegus.Usuario;
            string senha = configuracaoIntegracaoTrafegus.Senha;

            Dominio.Entidades.Cliente origem = ObterOrigemCarga(cargaIntegracao.Carga);

            if (origem == null)
                return false;

            string url = ObterURLMetodo("transportador", origem.CPF_CNPJ_SemFormato);

            string retorno = ObterJson(url, usuario, senha);

            dynamic dynRetorno = JsonConvert.DeserializeObject<dynamic>(retorno);

            if (string.IsNullOrEmpty((dynRetorno?.transportador?.ToString() ?? string.Empty)))
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.CadastroTransportador cadastroTransportador = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.CadastroTransportador();
                cadastroTransportador.transportador = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.Transportador>
                {
                    new Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.Transportador
                    {
                        documento_transportador = cargaIntegracao?.Carga?.Empresa?.CNPJ ?? string.Empty,
                        nome = string.IsNullOrWhiteSpace(origem.NomeFantasia) ? origem.Nome : origem.NomeFantasia
                    }
                };

                string jsonRequest = JsonConvert.SerializeObject(cadastroTransportador);

                string urlPost = ObterURLMetodo("transportador", "");

                string jsonResponse = ExecutarMetodo("POST", usuario, senha, urlPost, jsonRequest);

                Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.IntegarcaoRetorno integracaoRetorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.IntegarcaoRetorno>(jsonResponse);

                if (integracaoRetorno?.error?.Count > 0)
                {
                    string msgErro = "Erro ao enviar transportador";

                    AtualizarSituacaoIntegracao(cargaIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao, msgErro, "", jsonRequest, jsonResponse);

                    return false;
                }
            }

            return true;
        }

        public void IntegrarCarga(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao)
        {
            string jSonRequest = "";
            string jSonResponse = "";

            cargaIntegracao.DataIntegracao = DateTime.Now;
            cargaIntegracao.NumeroTentativas++;

            try
            {
                Repositorio.Embarcador.Configuracoes.IntegracaoTrafegus repConfiguracaoIntegracaoTrafegus = new Repositorio.Embarcador.Configuracoes.IntegracaoTrafegus(_unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrafegus configuracaoIntegracaoTrafegus = repConfiguracaoIntegracaoTrafegus.Buscar();

                VerificarConfiguracaoIntegracao(configuracaoIntegracaoTrafegus);

                if (!IntegrarCadastros(cargaIntegracao))
                    return;

                Repositorio.Embarcador.Cargas.Carga repcarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.IntegracaoViagem integracaoViagem = ObterViagem(cargaIntegracao.Carga);

                bool atualizacao = false;

                Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.IntegarcaoRetorno retorno;
                if (string.IsNullOrEmpty(cargaIntegracao.Carga.CodigoIntegracaoViagem))
                {
                    retorno = EnviarViagem(integracaoViagem, ref jSonRequest, ref jSonResponse);

                    if (retorno.error.Count == 0)
                    {
                        cargaIntegracao.Carga.CodigoIntegracaoViagem = retorno?.sucesso?.FirstOrDefault().cod_viagem;
                        repcarga.Atualizar(cargaIntegracao.Carga);
                    }
                }
                else
                {
                    atualizacao = true;
                    retorno = AtualizarViagem(integracaoViagem, cargaIntegracao.Carga.CodigoIntegracaoViagem, ref jSonRequest, ref jSonResponse);
                }

                bool sucesso = (retorno?.error == null || retorno.error.Count == 0) || retorno?.sucesso?.Count > 0;
                string mensagem = sucesso ? "Integração gerada com sucesso" : (retorno?.error != null ? String.Join(",", (from erro in retorno.error select erro.mensagem)) : "erro ao integrar viagem");
                string viagem = sucesso ? $"Viagem: {retorno?.sucesso?.FirstOrDefault().cod_viagem}" : string.Empty;

                if (atualizacao)
                    viagem = $"Viagem: {cargaIntegracao.Carga.CodigoIntegracaoViagem}";

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacao = sucesso ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                AtualizarSituacaoIntegracao(cargaIntegracao, situacao, mensagem, viagem, jSonRequest, jSonResponse);
            }
            catch (ServicoException ex)
            {
                AtualizarSituacaoIntegracao(cargaIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao, ex.Message, string.Empty, jSonRequest, jSonResponse);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                AtualizarSituacaoIntegracao(cargaIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao, "Erro ao realizar integração", string.Empty, jSonRequest, jSonResponse);
            }
        }

        public void CancelarCarga(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cargaCancelamentoIntegracao, string stringConexao)
        {
            string jSonRequest = "";
            string jSonResponse = "";

            cargaCancelamentoIntegracao.DataIntegracao = DateTime.Now;
            cargaCancelamentoIntegracao.NumeroTentativas++;

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.IntegarcaoRetorno retorno = AtualizarStatusViagem(cargaCancelamentoIntegracao.Codigo, ref jSonRequest, ref jSonResponse);

                string mensagem = "Integração gerada com sucesso";
                if (retorno.error.Count == 0)
                {
                    string viagem = $"Viagem: {retorno?.sucesso?.FirstOrDefault().cod_viagem}";
                    AtualizarSituacaoCancelamentoIntegracao(cargaCancelamentoIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado, mensagem, viagem, jSonRequest, jSonResponse);
                }
                else
                {
                    mensagem = String.Join(",", (from erro in retorno.error select erro.mensagem));
                    throw new ServicoException(mensagem);
                }

            }
            catch (ServicoException ex)
            {
                AtualizarSituacaoCancelamentoIntegracao(cargaCancelamentoIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao, ex.Message, string.Empty, jSonRequest, jSonResponse);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                AtualizarSituacaoCancelamentoIntegracao(cargaCancelamentoIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao, "Erro ao cancelar integração", string.Empty, jSonRequest, jSonResponse);
            }
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.Veiculo> ObterVeiculos(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.Veiculo> viagemVeiculos = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.Veiculo>
            {
                new Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.Veiculo { placa = carga.Veiculo.Placa }
            };

            foreach (Dominio.Entidades.Veiculo reboque in carga.VeiculosVinculados)
            {
                viagemVeiculos.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.Veiculo { placa = reboque.Placa });
            }

            return viagemVeiculos;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.Motorista> ObterMotoristas(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.Motorista> viagemMotoristas = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.Motorista>();

            foreach (Dominio.Entidades.Usuario motoristaCarga in carga.Motoristas)
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.Motorista motorista = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.Motorista
                {
                    cpf_moto = motoristaCarga.CPF,
                };

                viagemMotoristas.Add(motorista);
            }

            return viagemMotoristas;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.Locais> ObterLocais(List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes, List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> cargaRotaFretePontosPassagem)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.Locais> viagemLocais = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.Locais>();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem ponto in cargaRotaFretePontosPassagem)
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.EnderecoViagem enderecoPrincipal = ObterEnderecoPrincipal(ponto);

                Locais locais = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.Locais
                {
                    vloc_descricao = ponto.Descricao,
                    logradouro = enderecoPrincipal?.Logradouro ?? string.Empty,
                    complemento = enderecoPrincipal?.Complemento ?? string.Empty,
                    cep = enderecoPrincipal?.Cep ?? string.Empty,
                    numero = enderecoPrincipal?.Numero ?? string.Empty,
                    bairro = enderecoPrincipal?.Bairro ?? string.Empty,
                    cida_descricao_ibge = enderecoPrincipal?.CidadeDescricao ?? string.Empty,
                    sigla_estado = enderecoPrincipal?.SiglaEstado ?? string.Empty,
                    pais = enderecoPrincipal?.PaisDescricao ?? string.Empty,
                    cnpj = ponto.Cliente?.CPF_CNPJ_SemFormato ?? string.Empty,
                    refe_latitude = ponto.Latitude,
                    refe_longitude = ponto.Longitude,
                    conhecimentos = ObterConhecimento(cargaCTes, ponto?.Cliente?.CPF_CNPJ_SemFormato ?? string.Empty),
                    tipo_parada = (int)ponto.TipoPontoPassagem
                };

                viagemLocais.Add(locais);
            }

            return viagemLocais;
        }

        public Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.Origem ObterOrigem(Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem origem)
        {
            if (origem == null)
            {
                return null;
            }

            Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.EnderecoViagem enderecoPrincipal = ObterEnderecoPrincipal(origem);

            Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.Origem viagemOrigem = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.Origem()
            {
                vloc_descricao = origem?.Descricao ?? string.Empty,
                logradouro = enderecoPrincipal?.Logradouro ?? string.Empty,
                complemento = enderecoPrincipal?.Complemento ?? string.Empty,
                cep = enderecoPrincipal?.Cep ?? string.Empty,
                numero = enderecoPrincipal?.Numero ?? string.Empty,
                bairro = enderecoPrincipal?.Bairro ?? string.Empty,
                cida_descricao_ibge = enderecoPrincipal?.CidadeDescricao ?? string.Empty,
                sigla_estado = enderecoPrincipal?.SiglaEstado ?? string.Empty,
                pais = enderecoPrincipal?.PaisDescricao ?? string.Empty,
                refe_latitude = origem.Latitude,
                refe_longitude = origem.Longitude,
                cnpj = enderecoPrincipal.CNPJ ?? string.Empty
            };

            return viagemOrigem;
        }

        public Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.Destino ObterDestino(Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem destino, List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes)
        {
            if (destino == null)
            {
                return null;
            }

            Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.EnderecoViagem enderecoPrincipal = ObterEnderecoPrincipal(destino);

            Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.Destino viagemDestino = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.Destino()
            {
                vloc_descricao = destino.Descricao,
                logradouro = enderecoPrincipal?.Logradouro ?? string.Empty,
                complemento = enderecoPrincipal?.Complemento ?? string.Empty,
                cep = enderecoPrincipal?.Cep ?? string.Empty,
                numero = enderecoPrincipal?.Numero ?? string.Empty,
                bairro = enderecoPrincipal?.Bairro ?? string.Empty,
                cida_descricao_ibge = enderecoPrincipal?.CidadeDescricao ?? string.Empty,
                sigla_estado = enderecoPrincipal?.SiglaEstado ?? string.Empty,
                pais = enderecoPrincipal?.PaisDescricao ?? string.Empty,
                refe_latitude = destino.Latitude,
                refe_longitude = destino.Longitude,
                cnpj = enderecoPrincipal?.CNPJ ?? string.Empty,
                conhecimentos = ObterConhecimento(cargaCTes, destino?.Cliente?.CPF_CNPJ_SemFormato ?? string.Empty)
            };

            return viagemDestino;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.Terminai> ObterTerminais(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.Terminai> viagemTerminais = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.Terminai>();
            {
                new Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.Terminai
                {
                    term_numero_terminal = carga.Veiculo?.NumeroEquipamentoRastreador ?? string.Empty,
                    tecn_tecnologia = carga.Veiculo?.TecnologiaRastreador?.Codigo.ToString() ?? string.Empty,
                };
            }

            foreach (Dominio.Entidades.Veiculo reboque in carga.VeiculosVinculados)
            {
                viagemTerminais.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.Terminai
                {
                    term_numero_terminal = reboque.NumeroEquipamentoRastreador ?? string.Empty,
                    tecn_tecnologia = reboque.TecnologiaRastreador?.Codigo.ToString() ?? string.Empty
                });
            }

            return viagemTerminais;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.NotasFiscais> ObterNotasFiscais(List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes)
        {
            List<Dominio.Entidades.DocumentosCTE> cargaDocumentosCTE = cargaCTes.SelectMany(obj => obj.CTe.Documentos).ToList();

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.NotasFiscais> notasFiscais = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.NotasFiscais>();
            {
                foreach (Dominio.Entidades.DocumentosCTE documentoCTe in cargaDocumentosCTE)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.NotasFiscais notaFiscal = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.NotasFiscais
                    {
                        vnfi_numero = documentoCTe.Numero ?? string.Empty,
                        vnfi_pedido = documentoCTe.NumeroPedido ?? string.Empty,
                        vnfi_valor = documentoCTe.Valor.ToString() ?? string.Empty,
                        vnfi_data_fat = documentoCTe.DataEmissao.ToString() ?? string.Empty
                    };

                    notasFiscais.Add(notaFiscal);
                }
            }

            return notasFiscais;
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private string ObterURLMetodo(string metodo, string valor)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoTrafegus repConfiguracaoIntegracaoTrafegus = new Repositorio.Embarcador.Configuracoes.IntegracaoTrafegus(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrafegus configuracaoIntegracaoTrafegus = repConfiguracaoIntegracaoTrafegus.Buscar();

            string barra = configuracaoIntegracaoTrafegus.Url.EndsWith("/") ? "" : "/";
            string urlWebService = configuracaoIntegracaoTrafegus.Url + barra + metodo;

            if (!string.IsNullOrEmpty(valor))
                urlWebService += $"/{valor}";

            return urlWebService;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.Conhecimento> ObterConhecimento(List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes, string CNPJ)
        {
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> listaConhecimentosDestino = cargaCTes.Where(obj => obj.CTe.Destinatario?.CPF_CNPJ == CNPJ).ToList();

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.Conhecimento> listaConhecimentoTrafegus = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.Conhecimento>();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe conhecimento in listaConhecimentosDestino)
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.Conhecimento conhecimentoTrafegus = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.Conhecimento
                {
                    vlco_cpf_cnpj = CNPJ.ToString(),
                    vlco_numero = conhecimento.CTe.Numero.ToString(),
                    vlco_valor = Convert.ToDouble(conhecimento.CTe.ValorAReceber),
                    notas_fiscais = ObterNotasFiscais(cargaCTes)
                };

                listaConhecimentoTrafegus.Add(conhecimentoTrafegus);
            }

            return listaConhecimentoTrafegus;
        }
        private Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.IntegracaoViagem ObterViagem(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Cargas.CargaRotaFrete repCargaRotaFrete = new Repositorio.Embarcador.Cargas.CargaRotaFrete(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem repCargaRotaFretePontosPassagem = new Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoTrafegus repConfiguracaoIntegracaoTrafegus = new Repositorio.Embarcador.Configuracoes.IntegracaoTrafegus(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.Viagem viagem = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.Viagem();
            Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete = repCargaRotaFrete.BuscarPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> cargaRotaFretePontosPassagem = repCargaRotaFretePontosPassagem.BuscarPorCargaRotaFrete(cargaRotaFrete.Codigo);

            Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem origem = cargaRotaFretePontosPassagem.FirstOrDefault();
            cargaRotaFretePontosPassagem.Remove(origem);

            Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem destino = cargaRotaFretePontosPassagem.LastOrDefault();
            cargaRotaFretePontosPassagem.Remove(destino);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrafegus configuracaoIntegracaoTrafegus = repConfiguracaoIntegracaoTrafegus.Buscar();
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);
            Dominio.Entidades.Cliente origemCarga = ObterOrigemCarga(carga);

            viagem.documento_transportador = carga.Empresa.CNPJ_SemFormato;
            viagem.viag_codigo_externo = carga.Codigo.ToString();
            viagem.viag_ttra_codigo = carga.TipoOperacao?.CodigoIntegracaoGerenciadoraRisco ?? "6";

            if (_tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                viagem.cnpj_emba = !string.IsNullOrWhiteSpace(configuracaoIntegracaoTrafegus.CNPJEmbarcador) ? configuracaoIntegracaoTrafegus.CNPJEmbarcador : origemCarga.CPF_CNPJ_SemFormato;
            }

            viagem.veiculos = ObterVeiculos(carga);
            viagem.motoristas = ObterMotoristas(carga);
            viagem.terminais = ObterTerminais(carga);
            viagem.coordenadas = cargaRotaFrete.PolilinhaRota;
            viagem.viag_distancia = Convert.ToDouble(carga.DadosSumarizados.Distancia);

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repositorioCargaCte.BuscarPorCarga(carga.Codigo, true, false, 0, 0, false);

            viagem.origem = ObterOrigem(origem);
            viagem.destino = ObterDestino(destino, cargaCTes);
            viagem.locais = ObterLocais(cargaCTes, cargaRotaFretePontosPassagem);

            viagem.viag_pgpg_codigo = configuracaoIntegracaoTrafegus.PGR;
            viagem.viag_numero_manifesto = string.Join(",", string.Join(",", (from MDFe in carga?.CargaMDFes select MDFe?.Descricao).ToList()));
            viagem.viag_peso_total = carga?.DadosSumarizados?.PesoTotal != null ? Convert.ToDouble(carga.DadosSumarizados.PesoTotal) : 0;

            if (carga?.TipoDeCarga?.ControlaTemperatura ?? false)
            {
                viagem.temperatura = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.Temperatura
                {
                    descricao = carga?.TipoDeCarga?.FaixaDeTemperatura?.Descricao ?? string.Empty,
                    de = carga?.TipoDeCarga?.FaixaDeTemperatura?.FaixaInicial != null ? Convert.ToInt32(carga?.TipoDeCarga?.FaixaDeTemperatura?.FaixaInicial ?? 0) : 0,
                    ate = carga?.TipoDeCarga?.FaixaDeTemperatura?.FaixaFinal != null ? Convert.ToInt32(carga?.TipoDeCarga?.FaixaDeTemperatura?.FaixaFinal ?? 0) : 0
                };
            }

            decimal totalCarga = repPedidoXMLNotaFiscal.ObterValorTotalPorCarga(carga.Codigo);

            viagem.viag_valor_carga = Convert.ToDouble(totalCarga);
            viagem.viag_descricao_carga = carga?.TipoDeCarga?.Descricao;
            viagem.viag_previsao_inicio = carga.DataInicialPrevisaoCarregamento.ToString();
            viagem.viag_previsao_fim = carga.DataPrevisaoTerminoCarga.ToString();
            viagem.alterar_pgr_edicao_sm = "N";

            int.TryParse(carga.Rota?.CodigoIntegracaoGerenciadoraRisco ?? string.Empty, out int codigoIntegracaoGerenciadora);

            viagem.rota_codigo = codigoIntegracaoGerenciadora;

            Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.IntegracaoViagem IntegracaoViagem = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.IntegracaoViagem
            {
                vincularEmpresaLocal = false,
                viagem = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.Viagem>()
            };

            IntegracaoViagem.viagem.Add(viagem);

            return IntegracaoViagem;
        }
        private Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.IntegarcaoRetorno AtualizarStatusViagem(int IdViagem, ref string jsonRequest, ref string jsonResponse)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoTrafegus repConfiguracaoIntegracaoTrafegus = new Repositorio.Embarcador.Configuracoes.IntegracaoTrafegus(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrafegus configuracaoIntegracaoTrafegus = repConfiguracaoIntegracaoTrafegus.Buscar();

            string usuario = configuracaoIntegracaoTrafegus.Usuario;
            string senha = configuracaoIntegracaoTrafegus.Senha;
            string urlViagem = "viagem";
            string barra = configuracaoIntegracaoTrafegus.Url.EndsWith("/") ? "" : "/";
            string urlWebServiceViagem = configuracaoIntegracaoTrafegus.Url + barra + urlViagem + $"/{IdViagem}";
            Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.IntegracaoStatusViagem integracaoStatusViagem = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.IntegracaoStatusViagem()
            {
                status_viagem = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.StatusViagem { id_novo_status = 5 }
            };

            jsonRequest = JsonConvert.SerializeObject(integracaoStatusViagem);
            jsonResponse = ExecutarMetodo("PUT", usuario, senha, urlWebServiceViagem, jsonRequest);

            return JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.IntegarcaoRetorno>(jsonResponse);
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.IntegarcaoRetorno AtualizarViagem(Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.IntegracaoViagem integracaoViagem, string IdViagem, ref string jsonRequest, ref string jsonResponse)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoTrafegus repConfiguracaoIntegracaoTrafegus = new Repositorio.Embarcador.Configuracoes.IntegracaoTrafegus(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrafegus configuracaoIntegracaoTrafegus = repConfiguracaoIntegracaoTrafegus.Buscar();

            string usuario = configuracaoIntegracaoTrafegus.Usuario;
            string senha = configuracaoIntegracaoTrafegus.Senha;
            string url = ObterURLMetodo("viagem", IdViagem);

            Viagem viagem = integracaoViagem.viagem.FirstOrDefault();

            jsonRequest = JsonConvert.SerializeObject(viagem);
            jsonResponse = ExecutarMetodo("PUT", usuario, senha, url, jsonRequest);

            try
            {
                return JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.IntegarcaoRetorno>(jsonResponse);
            }
            catch (Exception e)
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.Error erro = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.Error();
                erro.mensagem = jsonResponse;

                Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.IntegarcaoRetorno retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.IntegarcaoRetorno();
                retorno.error = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.Error>();
                retorno.error.Add(erro);

                Servicos.Log.TratarErro(e);
                Servicos.Log.TratarErro(jsonRequest);

                return retorno;
            }
        }

        private string ObterJson(string urlWebService, string usuario, string senha)
        {
            try
            {
                WebRequest request = HttpWebRequest.Create(urlWebService);
                request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes($"{usuario}:{senha}"));
                request.Method = "GET";
                string contents = "";

                using (System.Net.HttpWebResponse resp = (System.Net.HttpWebResponse)request.GetResponse())
                {
                    if (resp.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        using (Stream responseStream = resp.GetResponseStream())
                        using (StreamReader responseStreamReader = new StreamReader(responseStream))
                        {
                            contents = responseStreamReader.ReadToEnd();
                        }
                    }

                    return contents;
                }
            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e);
                return null;
            }
        }

        private string ExecutarMetodo(string Metodo, string usuario, string senha, string url, string json)
        {
            WebRequest request = HttpWebRequest.Create(url);
            request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes($"{usuario}:{senha}"));
            request.ContentType = "application/json";
            request.Method = Metodo;

            using (StreamWriter streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(json);
            }

            HttpWebResponse httpResponse = (HttpWebResponse)request.GetResponse();
            using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                return streamReader.ReadToEnd();
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.IntegarcaoRetorno EnviarViagem(Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.IntegracaoViagem integracaoViagem, ref string jsonRequest, ref string jsonResponse)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoTrafegus repConfiguracaoIntegracaoTrafegus = new Repositorio.Embarcador.Configuracoes.IntegracaoTrafegus(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrafegus configuracaoIntegracaoTrafegus = repConfiguracaoIntegracaoTrafegus.Buscar();

            string usuario = configuracaoIntegracaoTrafegus.Usuario;
            string senha = configuracaoIntegracaoTrafegus.Senha;
            string urlViagem = "viagem";
            string barra = configuracaoIntegracaoTrafegus.Url.EndsWith("/") ? "" : "/";
            string urlWebServiceViagem = configuracaoIntegracaoTrafegus.Url + barra + urlViagem;

            //Teste
            //usuario = "WSWS";
            //senha = "1010";
            //urlWebServiceViagem = @"http://186.250.92.150:9090/ws_rest/public/api/viagem";

            jsonRequest = JsonConvert.SerializeObject(integracaoViagem);
            jsonResponse = ExecutarMetodo("POST", usuario, senha, urlWebServiceViagem, jsonRequest);

            return JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.IntegarcaoRetorno>(jsonResponse);
        }

        private void AtualizarSituacaoIntegracao(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacao, string mensagem, string protocolo, string jSonRequest, string jSonResponse)
        {
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            cargaIntegracao.SituacaoIntegracao = situacao;
            cargaIntegracao.ProblemaIntegracao = mensagem;
            cargaIntegracao.Protocolo = protocolo;

            servicoArquivoTransacao.Adicionar(cargaIntegracao, jSonRequest, jSonResponse, "json");
            repCargaCargaIntegracao.Atualizar(cargaIntegracao);
        }

        private void AtualizarSituacaoCancelamentoIntegracao(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cargaCancelamentoIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacao, string mensagem, string protocolo, string jSonRequest, string jSonResponse)
        {
            Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao repCargaCargaCancelamentoIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            cargaCancelamentoIntegracao.SituacaoIntegracao = situacao;
            cargaCancelamentoIntegracao.ProblemaIntegracao = mensagem;
            cargaCancelamentoIntegracao.Protocolo = protocolo;

            servicoArquivoTransacao.Adicionar(cargaCancelamentoIntegracao, jSonRequest, jSonResponse, "json");
            repCargaCargaCancelamentoIntegracao.Atualizar(cargaCancelamentoIntegracao);
        }

        private bool IntegrarCadastros(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao)
        {
            bool motorista = EnviarMotorista(cargaIntegracao);

            if (!motorista)
                return false;

            bool transportador = EnviarTransportador(cargaIntegracao);

            if (!transportador)
                return false;

            if (_tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                bool filial = EnviarFilial(cargaIntegracao);

                if (!filial)
                    return false;
            }

            bool veiculo = EnviarVeiculo(cargaIntegracao);

            if (!veiculo)
                return false;

            return true;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.EnderecoViagem ObterEnderecoPrincipal(Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem pontoPassagem)
        {
            if (pontoPassagem.ClienteOutroEndereco != null)
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.EnderecoViagem enderecoPrincipal = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.EnderecoViagem
                {
                    Logradouro = pontoPassagem.ClienteOutroEndereco.Endereco ?? string.Empty,
                    Complemento = pontoPassagem.ClienteOutroEndereco.Complemento ?? string.Empty,
                    Cep = pontoPassagem.ClienteOutroEndereco.CEP ?? string.Empty,
                    Numero = pontoPassagem.ClienteOutroEndereco.Numero ?? string.Empty,
                    Bairro = pontoPassagem.ClienteOutroEndereco.Bairro ?? string.Empty,
                    CidadeDescricao = pontoPassagem.ClienteOutroEndereco.Localidade?.Descricao ?? string.Empty,
                    SiglaEstado = pontoPassagem.ClienteOutroEndereco.Localidade?.Estado?.Sigla ?? string.Empty,
                    PaisDescricao = pontoPassagem.ClienteOutroEndereco.Localidade?.Pais?.Descricao ?? string.Empty,
                    CNPJ = pontoPassagem.Cliente?.CPF_CNPJ_SemFormato ?? string.Empty
                };

                return enderecoPrincipal;
            }

            if (pontoPassagem.Cliente != null)
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.EnderecoViagem enderecoPrincipal = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.EnderecoViagem
                {
                    Logradouro = pontoPassagem.Cliente.Endereco ?? string.Empty,
                    Complemento = pontoPassagem.Cliente.Complemento ?? string.Empty,
                    Cep = pontoPassagem.Cliente.CEP ?? string.Empty,
                    Numero = pontoPassagem.Cliente.Numero ?? string.Empty,
                    Bairro = pontoPassagem.Cliente.Bairro ?? string.Empty,
                    CidadeDescricao = pontoPassagem.Cliente.Localidade?.Descricao ?? string.Empty,
                    SiglaEstado = pontoPassagem.Cliente.Localidade?.Estado?.Sigla ?? string.Empty,
                    PaisDescricao = pontoPassagem.Cliente.Localidade?.Pais?.Descricao ?? string.Empty,
                    CNPJ = pontoPassagem.Cliente.CPF_CNPJ_SemFormato ?? string.Empty
                };

                return enderecoPrincipal;
            }

            if (pontoPassagem.Localidade != null)
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.EnderecoViagem enderecoPrincipal = new Dominio.ObjetosDeValor.Embarcador.Integracao.Trafegus.EnderecoViagem
                {
                    Logradouro = string.Empty,
                    Complemento = string.Empty,
                    Cep = pontoPassagem.Localidade.CEP ?? string.Empty,
                    Numero = string.Empty,
                    Bairro = string.Empty,
                    CidadeDescricao = pontoPassagem.Localidade.Descricao ?? string.Empty,
                    SiglaEstado = pontoPassagem.Localidade.Estado?.Sigla ?? string.Empty,
                    PaisDescricao = pontoPassagem.Localidade.Pais?.Descricao ?? string.Empty,
                    CNPJ = string.Empty
                };

                return enderecoPrincipal;
            }

            return null;
        }

        private void VerificarConfiguracaoIntegracao(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrafegus configuracaoIntegracaoTrafegus)
        {
            if (!configuracaoIntegracaoTrafegus.PossuiIntegracao || configuracaoIntegracaoTrafegus == null)
                throw new ServicoException("Não existe configuração de integração disponível para a Trafegus");

            if (string.IsNullOrWhiteSpace(configuracaoIntegracaoTrafegus.Usuario))
                throw new ServicoException("O Usuário deve estar preenchido na configuração de integração da Trafegus");

            if (string.IsNullOrWhiteSpace(configuracaoIntegracaoTrafegus.Senha))
                throw new ServicoException("A Senha deve estar preenchida na configuração de integração da Trafegus");

            if (string.IsNullOrWhiteSpace(configuracaoIntegracaoTrafegus.Url))
                throw new ServicoException("Não existe URL de integração configurada para Trafegus");

            if (configuracaoIntegracaoTrafegus.PGR == 0)
                throw new ServicoException("PGR deve estar preenchido na configuração de integração da Trafegus");
        }

        #endregion Métodos Privados
    }
}
