using Dominio.Excecoes.Embarcador;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

namespace Servicos.Embarcador.CIOT
{
    public partial class Ambipar
    {
        #region Métodos Globais

        public bool IntegrarVeiculo(Dominio.Entidades.Veiculo veiculo, bool carreta, Dominio.Entidades.Cliente TransportadorCIOT, int? idTransportador, Repositorio.UnitOfWork unitOfWork, out string mensagemErro, out int? idVeiculo)
        {
            mensagemErro = null;
            idVeiculo = null;
            bool sucesso = false;

            try
            {
                if (carreta && veiculo == null)
                    return true;

                this.ObterToken(out mensagemErro);
                if (string.IsNullOrWhiteSpace(this.token))
                    return false;

                #region Buscar Veículo/Carreta

                string urlConsulta = string.Empty;
                string jsonRetornoConsulta = "";

                if (!carreta)
                    urlConsulta = $"{this.urlWebService}mso-cargo-cadastrounico-transportador/api/Veiculo/ConsultarPlacaTransportadorID?placa={veiculo.Placa}&TransportadorID={idTransportador}";
                else
                    urlConsulta = $"{this.urlWebService}mso-cargo-cadastrounico-transportador/api/Carreta/ConsultarCarretaPorEmbarcador?placa={veiculo.Placa}&TransportadorID={idTransportador}";

                HttpClient requisicaoConsulta = CriarRequisicao(urlConsulta);
                HttpResponseMessage retornoRequisicaoConsulta = requisicaoConsulta.GetAsync(urlConsulta).Result;
                jsonRetornoConsulta = retornoRequisicaoConsulta.Content.ReadAsStringAsync().Result;

                bool bIncluir = false;
                if (retornoRequisicaoConsulta.StatusCode == HttpStatusCode.BadRequest)
                    bIncluir = true;
                else if (!retornoRequisicaoConsulta.IsSuccessStatusCode)
                    throw new ServicoException($"Ocorreu uma falha ao consultar o cadastro do veículo: {retornoRequisicaoConsulta.StatusCode}");

                Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.retVeiculo retornoConsulta = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.retVeiculo>(jsonRetornoConsulta);

                #endregion

                #region Atualizar/Incluir Veículo/Carreta

                string jsonRequisicao = "";
                string jsonRetorno = "";
                string urlIncluirAtualizar = string.Empty;

                if (!carreta)
                {
                    urlIncluirAtualizar = $"{this.urlWebService}mso-cargo-cadastrounico-transportador/api/Veiculo";
                    Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.envVeiculo enviarVeiculo = ObterObjVeiculo(veiculo, idTransportador);
                    jsonRequisicao = JsonConvert.SerializeObject(enviarVeiculo, Formatting.Indented);
                }
                else
                {
                    urlIncluirAtualizar = $"{this.urlWebService}mso-cargo-cadastrounico-transportador/api/Carreta";
                    Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.envCarreta enviarCarreta = ObterObjCarreta(veiculo, idTransportador);
                    jsonRequisicao = JsonConvert.SerializeObject(enviarCarreta, Formatting.Indented);
                }

                HttpClient requisicao = CriarRequisicao(urlIncluirAtualizar);
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage retornoRequisicao = null;

                if (bIncluir)
                    retornoRequisicao = requisicao.PostAsync(urlIncluirAtualizar, conteudoRequisicao).Result;
                else
                    retornoRequisicao = requisicao.PutAsync(urlIncluirAtualizar, conteudoRequisicao).Result;

                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                if (!retornoRequisicao.IsSuccessStatusCode)
                {
                    string mensagemRetorno = string.Empty;
                    if (string.IsNullOrEmpty(jsonRetorno))
                        mensagemRetorno = $"Ocorreu uma falha ao efetuar o cadastro do veículo: {retornoRequisicao.StatusCode}";
                    else
                        mensagemRetorno = $"Ocorreu uma falha ao efetuar o cadastro do veículo: {jsonRetorno}";

                    if (bIncluir)
                        throw new ServicoException(mensagemRetorno);
                    else
                        Servicos.Log.TratarErro(mensagemRetorno);
                }

                if (retornoConsulta == null)
                    retornoConsulta = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.retVeiculo>(jsonRetorno);

                idVeiculo = retornoConsulta.id;
                sucesso = true;

                #endregion
            }
            catch (ServicoException ex)
            {
                mensagemErro = ex.Message;
                sucesso = false;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                mensagemErro = "Ocorreu uma falha ao integrar os dados do veículo do CIOT.";
                sucesso = false;
            }

            return sucesso;
        }

        public bool IntegrarCarretas(List<Dominio.Entidades.Veiculo> carretas, Dominio.Entidades.Cliente TransportadorCIOT, int? idTransportador, Repositorio.UnitOfWork unitOfWork, out string mensagemErro, out List<int> idsCarreta)
        {
            mensagemErro = string.Empty;
            idsCarreta = null;

            if (carretas == null || carretas.Count == 0)
                return true;

            foreach (Dominio.Entidades.Veiculo carreta in carretas)
            {
                int? idTransportadorCarreta = null;
                Dominio.Entidades.Cliente transportadorCarreta = null;

                if (TransportadorCIOT.CPF_CNPJ == carreta.Proprietario.CPF_CNPJ)
                {
                    transportadorCarreta = TransportadorCIOT;
                    idTransportadorCarreta = idTransportador;
                }
                else
                {
                    transportadorCarreta = carreta.Proprietario;
                    Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro = Servicos.Embarcador.CIOT.CIOT.ObterModalidadeTransportador(transportadorCarreta, _unitOfWork);

                    if (!IntegrarTransportador(transportadorCarreta, modalidadeTerceiro, _unitOfWork, out mensagemErro, out idTransportador))
                        return false;
                }

                int? idCarreta = null;

                if (!IntegrarVeiculo(carreta, true, transportadorCarreta, idTransportadorCarreta, _unitOfWork, out mensagemErro, out idCarreta))
                    return false;

                if (idCarreta != null)
                {
                    if (idsCarreta == null)
                        idsCarreta = new List<int>();

                    idsCarreta.Add((int)idCarreta);
                }
            }

            return true;
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.envVeiculo ObterObjVeiculo(Dominio.Entidades.Veiculo veiculo, int? idTransportador)
        {
            Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.envVeiculo retorno = new Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.envVeiculo();

            retorno.transportadorID = idTransportador;
            retorno.veiculoTipoID = Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.enumVeiculoTipo.Truck;
            retorno.rastreadorTipoID =  Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.enumRastreadorTipo.Sem_Rastreador;
            retorno.placa = veiculo.Placa;
            retorno.quantidadeEixos = veiculo.ModeloVeicularCarga?.NumeroEixos ?? 0;
            retorno.modalidade = (veiculo.ModeloVeicularCarga?.NumeroEixos.Value ?? 2) == 2 ? Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.enumModalidade.Simples : Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.enumModalidade.Duplo;
            retorno.anoFabricacao = veiculo.AnoFabricacao > 0 ? Convert.ToDateTime($"01/01/{veiculo.AnoFabricacao}").ToString("u", new System.Globalization.CultureInfo("pt-BR")).Replace(" ", "T") : null;
            retorno.codigoRenavam = veiculo.Renavam;
            retorno.numeroChassis = veiculo.Chassi;
            retorno.corPredominante = veiculo.CorVeiculo?.Descricao;
            retorno.cidadeDaPlaca = veiculo.LocalidadeEmplacamento?.Descricao;
            retorno.estadoDaPlaca = veiculo.Estado?.Sigla;
            retorno.codigoRastreador = null;
            retorno.frotaPropria = false;
            retorno.permitirValePedagioCartao = true;

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.envCarreta ObterObjCarreta(Dominio.Entidades.Veiculo veiculo, int? idTransportador)
        {
            Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.envCarreta retorno = new Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.envCarreta();

            retorno.transportadorID = idTransportador;
            retorno.carretaTipoID = this.BuscarTipoCarroceriaAmbipar(veiculo.TipoCarroceria);
            retorno.placa = veiculo.Placa;
            retorno.quantidadeEixos = veiculo.ModeloVeicularCarga?.NumeroEixos ?? 0;
            retorno.anoFabricacao = veiculo.AnoFabricacao > 0 ? Convert.ToDateTime($"01/01/{veiculo.AnoFabricacao}").ToString("u", new System.Globalization.CultureInfo("pt-BR")).Replace(" ", "T") : null;
            retorno.codigoRenavam = veiculo.Renavam;
            retorno.numeroChassis = veiculo.Chassi;
            retorno.corPredominante = veiculo.CorVeiculo?.Descricao;
            retorno.cidadeDaPlaca = null;
            retorno.estadoDaPlaca = veiculo.Estado?.Sigla;
            retorno.peso = veiculo.CapacidadeKG;
            retorno.volume = 0;
            retorno.tara = veiculo.Tara;
            retorno.frotaPropria = false;

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.enumCarretaTipo BuscarTipoCarroceriaAmbipar(string tipoCarroceria)
        {
            Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.enumCarretaTipo retorno = Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.enumCarretaTipo.Nao_Aplicavel;

            /// 00 - NAO APLICADO
            /// 01 - ABERTA
            /// 02 - FECHADA / BAU
            /// 03 - GRANEL
            /// 04 - PORTA CONTAINER
            /// 05 - SIDER

            if (tipoCarroceria == "01")
                retorno = Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.enumCarretaTipo.Aberta;
            else if (tipoCarroceria == "02")
                retorno = Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.enumCarretaTipo.Fechada_Baú;
            else if (tipoCarroceria == "03")
                retorno = Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.enumCarretaTipo.Granelera;
            else if (tipoCarroceria == "04")
                retorno = Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.enumCarretaTipo.Porta_Container;
            else if (tipoCarroceria == "05")
                retorno = Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.enumCarretaTipo.Sider;

            return retorno;
        }

        #endregion
    }
}