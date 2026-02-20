using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;

namespace Servicos.Embarcador.CIOT
{
    public partial class Ambipar
    {
        #region Métodos Globais

        public bool IntegrarMotorista(Dominio.Entidades.Cliente proprietario, Dominio.Entidades.Usuario motorista, Dominio.Entidades.Cliente TransportadorCIOT, int? idTransportador, Repositorio.UnitOfWork unitOfWork, out string mensagemErro, out int? idMotorista)
        {
            mensagemErro = null;
            idMotorista = null;
            bool sucesso = false;

            try
            {
                this.ObterToken(out mensagemErro);
                if (string.IsNullOrWhiteSpace(this.token))
                    return false;

                #region Buscar Motorista

                string urlConsulta = $"{this.urlWebService}mso-cargo-cadastrounico-transportador/api/Motorista/consultarporcpfcnpj?numerocnpjcpf={motorista.CPF}&transportadorID={idTransportador}";
                string jsonRetornoConsulta = "";

                HttpClient requisicaoConsulta = CriarRequisicao(urlConsulta);
                HttpResponseMessage retornoRequisicaoConsulta = requisicaoConsulta.GetAsync(urlConsulta).Result;
                jsonRetornoConsulta = retornoRequisicaoConsulta.Content.ReadAsStringAsync().Result;

                bool bIncluir = false;
                if (retornoRequisicaoConsulta.StatusCode == HttpStatusCode.BadRequest)
                    bIncluir = true;
                else if (!retornoRequisicaoConsulta.IsSuccessStatusCode)
                    throw new ServicoException($"Ocorreu uma falha ao consultar o cadastro do motorista: {retornoRequisicaoConsulta.StatusCode}");

                Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.retMotorista retornoConsulta = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.retMotorista>(jsonRetornoConsulta);

                #endregion

                #region Buscar Cartão Motorista

                int? idCartao = null;
                if (!string.IsNullOrEmpty(motorista.NumeroCartao))
                {
                    if (!this.BuscarCartao(motorista.NumeroCartao, unitOfWork, out idCartao, out mensagemErro))
                        throw new ServicoException(mensagemErro);
                }

                #endregion

                #region Atualizar/Incluir Motorista

                string jsonRequisicao = "";
                string jsonRetorno = "";
                string urlIncluirAtualizar = $"{this.urlWebService}mso-cargo-cadastrounico-transportador/api/Motorista";

                Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.envMotorista enviarMotorista = ObterObjMotorista(motorista, retornoConsulta, proprietario, idTransportador, idCartao);

                HttpClient requisicao = CriarRequisicao(urlIncluirAtualizar);
                jsonRequisicao = JsonConvert.SerializeObject(enviarMotorista, Formatting.Indented);
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
                        mensagemRetorno = $"Ocorreu uma falha ao efetuar o cadastro do motorista: {retornoRequisicao.StatusCode}";
                    else
                        mensagemRetorno = $"Ocorreu uma falha ao efetuar o cadastro do motorista: {jsonRetorno}";

                    if (bIncluir)
                        throw new ServicoException(mensagemRetorno);
                    else
                        Servicos.Log.TratarErro(mensagemRetorno);
                }

                if (retornoConsulta == null)
                    retornoConsulta = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.retMotorista>(jsonRetorno);

                idMotorista = retornoConsulta.id;
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

                mensagemErro = "Ocorreu uma falha ao integrar os dados do motorista do CIOT.";
                sucesso = false;
            }

            return sucesso;
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.envMotorista ObterObjMotorista(Dominio.Entidades.Usuario motorista, Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.retMotorista retornoConsulta, Dominio.Entidades.Cliente contratado, int? idTransportador, int? idCartao)
        {
            Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.envMotorista retorno = new Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.envMotorista();

            if (retornoConsulta != null)
            {
                retorno.id = retornoConsulta.id;
                retorno.pessoaID = retornoConsulta.pessoaID;
                retorno.dataInclusao = (System.DateTime.Now).ToString("u", new System.Globalization.CultureInfo("pt-BR")).Replace(" ", "T");
            }

            retorno.nome = motorista.Nome;
            retorno.documento = motorista.CPF.PadLeft(11, '0');
            retorno.pessoaDocumentoTipoID = enumPessoaMotoristaDocumentoTipo.CPF;
            retorno.cartaoID = idCartao;
            retorno.cartaoCombustivelID = null;
            retorno.motorista = this.ObterObjDadosMotorista(motorista, contratado, idTransportador, retornoConsulta);
            retorno.pessoaDocumentos = this.ObterObjPessoaDocumentos(motorista, retornoConsulta);
            retorno.pessoaContatos = this.ObterObjPessoaContatos(motorista, retornoConsulta);

            return retorno;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.motorista> ObterObjDadosMotorista(Dominio.Entidades.Usuario motorista, Dominio.Entidades.Cliente contratado, int? idTransportador, Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.retMotorista retornoConsulta)
        {
            List <Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.motorista> retorno = new List<Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.motorista>();
            Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.motorista mot = new Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.motorista();

            if (retornoConsulta != null)
            {
                mot.id = retornoConsulta.motorista?.FirstOrDefault().id ?? 0;
                mot.pessoaID = retornoConsulta.pessoaID;
                mot.dataInclusao = (System.DateTime.Now).ToString("u", new System.Globalization.CultureInfo("pt-BR")).Replace(" ", "T");
            }

            mot.sexo = this.ObterDescricaoSexoAmbipar(motorista.Sexo ?? Dominio.ObjetosDeValor.Enumerador.Sexo.NaoInformado);
            mot.nomeMae = motorista.FiliacaoMotoristaMae;
            mot.nomePai = motorista.FiliacaoMotoristaPai;
            mot.dataNascimento = motorista.DataNascimento != null ? ((DateTime)motorista.DataNascimento).ToString("u", new System.Globalization.CultureInfo("pt-BR")).Replace(" ", "T") : null;
            if (!string.IsNullOrEmpty(motorista?.LocalidadeNascimento?.Descricao))
                mot.naturalidade = $"{motorista?.LocalidadeNascimento?.Descricao} {motorista.LocalidadeNascimento?.Estado?.Sigla}";
            mot.ufNascimento = motorista?.LocalidadeNascimento?.Estado?.Sigla;
            mot.transportadorID = idTransportador;

            retorno.Add(mot);
            return retorno;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.pessoaDocumento> ObterObjPessoaDocumentos(Dominio.Entidades.Usuario motorista, Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.retMotorista retornoConsulta)
        {
            List<Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.pessoaDocumento> retorno = null;

            if (!string.IsNullOrEmpty(motorista.RG))
            {
                if (retorno == null)
                    retorno = new List<Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.pessoaDocumento>();

                Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.pessoaDocumento documento = new Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.pessoaDocumento();

                if (retornoConsulta != null)
                {
                    documento.id = 0;
                    documento.pessoaID = retornoConsulta.pessoaID;
                    documento.dataInclusao = (System.DateTime.Now).ToString("u", new System.Globalization.CultureInfo("pt-BR")).Replace(" ", "T");
                }

                documento.pessoaDocumentoTipoID = enumPessoaMotoristaDocumentoTipo.RG;
                documento.numeroDocumento = motorista.RG;
                documento.dataEmissao = motorista.DataEmissaoRG != null ? ((DateTime)motorista.DataEmissaoRG).ToString("u", new System.Globalization.CultureInfo("pt-BR")).Replace(" ", "T") : null;
                documento.dataValidade = motorista.DataEmissaoRG != null ? ((DateTime)motorista.DataEmissaoRG).AddYears(10).ToString("u", new System.Globalization.CultureInfo("pt-BR")).Replace(" ", "T") : null;
                documento.orgaoEmissor = Dominio.ObjetosDeValor.Enumerador.OrgaoEmissorRGHelper.ObterDescricao(motorista.OrgaoEmissorRG ?? Dominio.ObjetosDeValor.Enumerador.OrgaoEmissorRG.Nenhum);
                documento.ufEmissor = motorista?.EstadoRG?.Sigla;
                retorno.Add(documento);
            }

            if (!string.IsNullOrEmpty(motorista.NumeroHabilitacao))
            {
                if (retorno == null)
                    retorno = new List<Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.pessoaDocumento>();

                Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.pessoaDocumento documento = new Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.pessoaDocumento();

                if (retornoConsulta != null)
                {
                    documento.id = 0;
                    documento.pessoaID = retornoConsulta.pessoaID;
                    documento.dataInclusao = (System.DateTime.Now).ToString("u", new System.Globalization.CultureInfo("pt-BR")).Replace(" ", "T");
                }

                documento.pessoaDocumentoTipoID = enumPessoaMotoristaDocumentoTipo.CNH;
                documento.numeroDocumento = motorista.NumeroHabilitacao.PadLeft(11, '0');
                documento.dataEmissao = motorista.DataHabilitacao != null ? ((DateTime)motorista.DataHabilitacao).ToString("u", new System.Globalization.CultureInfo("pt-BR")).Replace(" ", "T") : null;
                documento.dataValidade = motorista.DataVencimentoHabilitacao != null ? ((DateTime)motorista.DataVencimentoHabilitacao).AddYears(10).ToString("u", new System.Globalization.CultureInfo("pt-BR")).Replace(" ", "T") : null;
                documento.orgaoEmissor = "SNT"; //Secretária Nacional de Transito
                documento.ufEmissor = motorista?.UFEmissaoCNH?.Sigla;
                retorno.Add(documento);
            }

            return retorno;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.pessoaContato> ObterObjPessoaContatos(Dominio.Entidades.Usuario motorista, Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.retMotorista retornoConsulta)
        {
            List<Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.pessoaContato> retorno = new List<Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.pessoaContato>();
            Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.pessoaContato pessoaContato = new Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.pessoaContato();

            if (retornoConsulta != null)
            {
                pessoaContato.id = 0;
                pessoaContato.pessoaID = retornoConsulta.pessoaID;
            }

            pessoaContato.email = motorista?.Email;
            pessoaContato.codigoPais = 55;
            pessoaContato.celular1 = this.ObterTelefoneAmbipar(null, motorista.Celular);
            pessoaContato.celular2 = null;
            pessoaContato.telefone1 = this.ObterTelefoneAmbipar(null, motorista.Telefone);
            pessoaContato.telefone2 = null;
            retorno.Add(pessoaContato);

            return retorno;
        }

        private string ObterDescricaoSexoAmbipar(Dominio.ObjetosDeValor.Enumerador.Sexo sexo)
        {
            string retorno = null;

            if (sexo == Dominio.ObjetosDeValor.Enumerador.Sexo.Masculino)
                retorno = "Masculino";
            else if (sexo == Dominio.ObjetosDeValor.Enumerador.Sexo.Feminino)
                retorno = "Feminino";

            return retorno;
        }

        private string ObterTelefoneAmbipar(string paissigla, string telefoneCadastro)
        {
            string ddd = null;
            string telefone = null;

            Regex buscarDDD = new Regex(@"^\((?<ddd>[0-9]{2})\) ?(?<fone>[0-9-]+)");
            var ret = buscarDDD.Match(telefoneCadastro);
            if (ret.Success)
            {
                ddd = ret.Groups["ddd"].Value;
                telefone = Regex.Replace(ret.Groups["fone"].Value, "[^0-9]", "");
            }
            else
            {
                string telefoneCompleto = Regex.Replace(telefoneCadastro, "[^0-9]", "");

                if (!string.IsNullOrEmpty(telefoneCompleto))
                {
                    if (telefoneCompleto.Length >= 10)
                    {
                        ddd = telefoneCompleto.Substring(0, 2);
                        telefone = telefoneCompleto.Substring(2, (telefoneCompleto.Length - 2));
                    }
                    else
                    {
                        ddd = null;
                        telefone = telefoneCompleto;
                    }
                }
            }

            string ddi = string.Empty;
            string dddret = string.Empty;
            string prefixo_mais_numero = string.Empty;
            string prefixo = string.Empty;
            string numero = string.Empty;

            if (!string.IsNullOrEmpty(telefone))
            {
                ddi = paissigla == "BRASIL" ? "55" : null;
                dddret = ddd;
                prefixo_mais_numero = telefone;

                if (telefone.Length >= 9)
                {
                    prefixo = telefone.Substring(0, 5);
                    numero = telefone.Substring(5, (telefone.Length - 5));
                }
                else if (telefone.Length >= 5)
                {
                    prefixo = telefone.Substring(0, 4);
                    numero = telefone.Substring(4, (telefone.Length - 4));
                }
                else
                {
                    prefixo = null;
                    numero = telefone;
                }
            }

            return $"{dddret}{numero}";
        }

        #endregion
    }
}