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

        public bool IntegrarTransportador(Dominio.Entidades.Cliente proprietario, Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidade, Repositorio.UnitOfWork unitOfWork, out string mensagemErro, out int? idTransportador)
        {
            mensagemErro = null;
            idTransportador = null;
            bool sucesso = false;

            if (proprietario == null)
                return true;

            if (modalidade == null)
            {
                mensagemErro = "A modalidade do transportador não está configurada.";
                return false;
            }

            try
            {
                this.ObterToken(out mensagemErro);
                if (string.IsNullOrWhiteSpace(this.token))
                    return false;

                #region Buscar Transportador

                string urlConsulta = $"{this.urlWebService}mso-cargo-cadastrounico-transportador/api/Transportador/consultarporcpfcnpj?cpfCnpj={proprietario.CPF_CNPJ_SemFormato}";
                string jsonRetornoConsulta = "";

                HttpClient requisicaoConsulta = CriarRequisicao(urlConsulta);
                HttpResponseMessage retornoRequisicaoConsulta = requisicaoConsulta.GetAsync(urlConsulta).Result;
                jsonRetornoConsulta = retornoRequisicaoConsulta.Content.ReadAsStringAsync().Result;

                bool bIncluir = false;
                if (retornoRequisicaoConsulta.StatusCode == HttpStatusCode.BadRequest)
                    bIncluir = true;
                else if (!retornoRequisicaoConsulta.IsSuccessStatusCode)
                    throw new ServicoException($"Ocorreu uma falha ao consultar o cadastro do transportador: {retornoRequisicaoConsulta.StatusCode}");

                Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.retTransportador retornoConsulta = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.retTransportador>(jsonRetornoConsulta);

                #endregion

                #region Buscar Cartão Transportador

                int? idCartao = null;
                if (!string.IsNullOrEmpty(modalidade.NumeroCartao))
                {
                    if (!this.BuscarCartao(modalidade.NumeroCartao, unitOfWork, out idCartao, out mensagemErro))
                        throw new ServicoException(mensagemErro);
                }

                #endregion

                #region Atualizar/Incluir Transportador

                string jsonRequisicao = "";
                string jsonRetorno = "";
                string urlIncluirAtualizar = $"{this.urlWebService}mso-cargo-cadastrounico-transportador/api/Transportador";

                Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.envTransportador enviarTransportador = ObterObjTransportador(proprietario, modalidade, retornoConsulta, idCartao, unitOfWork);

                HttpClient requisicao = CriarRequisicao(urlIncluirAtualizar);
                jsonRequisicao = JsonConvert.SerializeObject(enviarTransportador, Formatting.Indented);
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage retornoRequisicao = null;

                if (bIncluir)
                    retornoRequisicao = requisicao.PostAsync(urlIncluirAtualizar, conteudoRequisicao).Result;
                else
                    retornoRequisicao = requisicao.PutAsync(urlIncluirAtualizar, conteudoRequisicao).Result;

                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                if (!retornoRequisicao.IsSuccessStatusCode)
                {
                    if (string.IsNullOrEmpty(jsonRetorno))
                        throw new ServicoException($"Ocorreu uma falha ao efetuar o cadastro do transportador: {retornoRequisicao.StatusCode}");
                    else
                        throw new ServicoException($"Ocorreu uma falha ao efetuar o cadastro do transp  ortador: {jsonRetorno}");
                }

                if (retornoConsulta == null)
                    retornoConsulta = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.retTransportador>(jsonRetorno);

                idTransportador = retornoConsulta.id;
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

                mensagemErro = "Ocorreu uma falha ao integrar os dados do transportador do CIOT.";
                sucesso = false;
            }

            return sucesso;
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.envTransportador ObterObjTransportador(Dominio.Entidades.Cliente cliente, Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidade, Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.retTransportador retornoConsulta, int? idCartao, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.envTransportador retorno = new Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.envTransportador();

            if (retornoConsulta != null)
                retorno.id = retornoConsulta.id;

            retorno.documento = cliente.CPF_CNPJ_SemFormato;
            retorno.documentoTipoID = cliente.Tipo == "F" ? Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.enumPessoaDocumentoTipo.CPF : Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.enumPessoaDocumentoTipo.CNPJ;
            retorno.razaoSocial = cliente.Nome;
            retorno.nomeFantasia = cliente.NomeFantasia;
            retorno.inscricaoMunicipal = cliente.InscricaoMunicipal;
            retorno.inscricaoEstadual = cliente.InscricaoST;
            retorno.inssSimplificado = false;
            retorno.numeroRNTRC = string.IsNullOrEmpty(modalidade.RNTRC) ? null : modalidade.RNTRC.PadLeft(9, '0');
            retorno.dataEmissaoRNTRC = modalidade.DataEmissaoRNTRC != null ? ((DateTime)modalidade.DataEmissaoRNTRC).ToString("u", new System.Globalization.CultureInfo("pt-BR")).Replace(" ", "T") : null;
            retorno.dataVencimentoRNTRC = modalidade.DataVencimentoRNTRC != null ? ((DateTime)modalidade.DataVencimentoRNTRC).ToString("u", new System.Globalization.CultureInfo("pt-BR")).Replace(" ", "T") : null;
            retorno.dataUltimaConsultaRNTRC = null;
            retorno.validadoANTT = false;
            retorno.numeroDependente = null;
            retorno.tipoRNTRC = null;
            retorno.cartaoID = idCartao;
            retorno.CartaoCombustivelID = null;
            retorno.contatos = this.ObterObjTransportadorContato(cliente);

            return retorno;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.TransportadorContato> ObterObjTransportadorContato(Dominio.Entidades.Cliente cliente)
        {
            List<Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.TransportadorContato> retorno = new List<Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.TransportadorContato>();
            Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.TransportadorContato contato = new Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.TransportadorContato();

            contato.nome = null;
            contato.area = null;
            contato.email = null;
            contato.codigoPais = null;
            contato.celular1 = null;
            contato.celular2 = null;
            contato.telefone1 = null;
            contato.telefone2 = null;
            retorno.Add(contato);

            return null;
        }

        #endregion
    }
}