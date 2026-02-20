using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb;
using Dominio.ObjetosDeValor.Enumerador;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Integracao.ATSSmartWeb
{
    public partial class IntegracaoATSSmartWeb
    {
        #region Metodos Publicos

        public bool IntegrarMotorista(ref Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, Dominio.Entidades.Usuario motorista)
        {
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            bool sucesso = false;

            string jsonRequisicao = "";
            string jsonRetorno = "";

            try
            {
                object request = this.obterMotorista(motorista, cargaIntegracao.Carga);
                Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.retornoWebService retWS = transmitir("CadastroPessoaIntegracao/Integrar", request);

                jsonRequisicao = retWS.jsonRequisicao;
                jsonRetorno = retWS.jsonRetorno;

                if (retWS.SituacaoIntegracao == SituacaoIntegracao.Integrado)
                    sucesso = true;
                else
                    throw new ServicoException($"Motorista {motorista.CPF} - " + retWS.ProblemaIntegracao);

            }
            catch (ServicoException excecao)
            {
                Log.TratarErro(excecao);

                String message = excecao.Message;
                if (message.Length > 300)
                {
                    message = message.Substring(0, 300);
                }
                cargaIntegracao.ProblemaIntegracao = message;
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaIntegracao.ProblemaIntegracao = "Erro ao tentar integrar motorista com a ATS Smart Web";
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }

            servicoArquivoTransacao.Adicionar(cargaIntegracao, jsonRequisicao, jsonRetorno, "json", "Integração de Motorista");

            repCargaIntegracao.Atualizar(cargaIntegracao);

            return sucesso;
        }

        public bool IntegrarMotorista(ref Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao, Dominio.Entidades.Usuario motorista)
        {
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            bool sucesso = false;

            string jsonRequisicao = "";
            string jsonRetorno = "";

            try
            {
                object request = this.obterMotorista(motorista, cargaDadosTransporteIntegracao.Carga);
                Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.retornoWebService retWS = transmitir("CadastroPessoaIntegracao/Integrar", request);

                jsonRequisicao = retWS.jsonRequisicao;
                jsonRetorno = retWS.jsonRetorno;

                if (retWS.SituacaoIntegracao == SituacaoIntegracao.Integrado)
                    sucesso = true;
                else
                    throw new ServicoException($"Motorista {motorista.CPF} - " + retWS.ProblemaIntegracao);

            }
            catch (ServicoException excecao)
            {
                Log.TratarErro(excecao);

                String message = excecao.Message;
                if (message.Length > 300)
                {
                    message = message.Substring(0, 300);
                }
                cargaDadosTransporteIntegracao.ProblemaIntegracao = message;
                cargaDadosTransporteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaDadosTransporteIntegracao.ProblemaIntegracao = "Erro ao tentar integrar motorista com a ATS Smart Web";
                cargaDadosTransporteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }

            servicoArquivoTransacao.Adicionar(cargaDadosTransporteIntegracao, jsonRequisicao, jsonRetorno, "json", "Integração de Motorista");

            repCargaDadosTransporteIntegracao.Atualizar(cargaDadosTransporteIntegracao);

            return sucesso;
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envMotorista obterMotorista(Dominio.Entidades.Usuario motorista, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envMotorista retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envMotorista();

            retorno.Nome = motorista.Nome;
            retorno.CPF_CNPJ = motorista.CPF;
            retorno.CodigoExterno = motorista.CPF.ToString();
            retorno.Condutor = true;
            retorno.Vinculo = motorista.TipoMotorista == TipoMotorista.Proprio ? 1 : 3;
            retorno.Cidade = motorista.Localidade?.Descricao ?? "";
            retorno.UF = this.obterCodigoDeUF(motorista.Localidade?.Estado?.Sigla ?? "");
            retorno.RegistroCNH = motorista.NumeroRegistroHabilitacao;
            retorno.Categoria = motorista.Categoria;
            retorno.ValidadeCnh = motorista.DataVencimentoHabilitacao?.ToString("yyyy-MM-dd") ?? null;
            retorno.DataAdmissao = motorista.DataAdmissao?.ToString("yyyy-MM-dd") ?? null;
            retorno.Periodico = null;
            retorno.Demissao = motorista.DataDemissao?.ToString("yyyy-MM-dd") ?? null;
            retorno.Pan = null;
            retorno.Pamcary = null;
            retorno.NivelEscolar = motorista.Escolaridade?.ObterDescricao();
            retorno.Email = motorista.Email;
            retorno.Matricula = motorista.NumeroMatricula;

            retorno.Complemento = this.obterComplemento(motorista);
            retorno.FisicaComplemento = this.obterFisicaComplemento(motorista);
            retorno.Endereco = this.obterEndereco(motorista);

            if(motorista.TipoMotorista == TipoMotorista.Terceiro)
                retorno.Empresa = this.obterEmpresa(motorista);
            else
                retorno.Empresa = this.obterEmpresa(motorista.Empresa ?? carga.Empresa);

            return retorno;
        }
        private int obterCodigoDeUF(string siglaUF)
        {
            switch (siglaUF)
            {
                case "AC": return 1;
                case "AL": return 2;
                case "AP": return 3;
                case "AM": return 4;
                case "BA": return 5;
                case "CE": return 6;
                case "DF": return 7;
                case "ES": return 8;
                case "GO": return 9;
                case "MA": return 10;
                case "MT": return 11;
                case "MS": return 12;
                case "MG": return 13;
                case "PA": return 14;
                case "PB": return 15;
                case "PR": return 16;
                case "PE": return 17;
                case "PI": return 18;
                case "RJ": return 19;
                case "RN": return 20;
                case "RS": return 21;
                case "RO": return 22;
                case "RR": return 23;
                case "SC": return 24;
                case "SP": return 25;
                case "SE": return 26;
                case "TO": return 27;
                case "EX": return 28;
                default: return 0;
            };
        }
        private Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envComplemento obterComplemento(Dominio.Entidades.Usuario motorista)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envComplemento retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envComplemento();

            if (motorista == null)
                return null;

            retorno.Telefone = motorista.Telefone;

            return retorno;
        }
        private Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envComplemento obterComplemento(Dominio.Entidades.Cliente pessoa)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envComplemento retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envComplemento();

            if (pessoa == null)
                return null;

            retorno.Telefone = pessoa.Telefone1;

            return retorno;
        }
        private Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envComplemento obterComplemento(Dominio.Entidades.Empresa empresa)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envComplemento retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envComplemento();

            if (empresa == null)
                return null;

            retorno.Telefone = empresa.Telefone;

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envFisicaComplemento obterFisicaComplemento(Dominio.Entidades.Usuario motorista)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envFisicaComplemento retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envFisicaComplemento();

            if (motorista == null)
                return null;

            retorno.DataNascimento = motorista.DataNascimento?.ToString("yyyy-MM-dd") ?? null;
            retorno.Filiacao = motorista.FiliacaoMotoristaMae;
            retorno.RG = motorista.RG;
            retorno.OrgaoExpedidor = motorista.OrgaoEmissorRG.HasValue ? motorista.OrgaoEmissorRG.Value.ObterDescricao() : null;

            return retorno;
        }
        private Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envFisicaComplemento obterFisicaComplemento(Dominio.Entidades.Cliente pessoa)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envFisicaComplemento retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envFisicaComplemento();

            if (pessoa == null)
                return null;

            retorno.DataNascimento = pessoa.DataNascimento?.ToString("yyyy-MM-dd") ?? null;
            retorno.Filiacao = null;
            retorno.RG = pessoa.IE_RG;
            retorno.OrgaoExpedidor = pessoa.OrgaoEmissorRG.HasValue ? pessoa.OrgaoEmissorRG.Value.ObterDescricao() : null;

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envEndereco obterEndereco(Dominio.Entidades.Usuario motorista)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envEndereco retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envEndereco();

            if (motorista == null)
                return null;

            retorno.Bairro = motorista.Bairro;
            retorno.CEP = motorista.CEP;
            retorno.Logradouro = motorista.Endereco;
            retorno.Numero = motorista.NumeroEndereco;

            return retorno;
        }
        private Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envEndereco obterEndereco(Dominio.Entidades.Cliente pessoa)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envEndereco retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envEndereco();

            if (pessoa == null)
                return null;

            retorno.Bairro = pessoa.Bairro;
            retorno.CEP = pessoa.CEP;
            retorno.Logradouro = pessoa.Endereco;
            retorno.Numero = pessoa.Numero;

            return retorno;
        }
        private Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envEndereco obterEndereco(Dominio.Entidades.Empresa empresa)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envEndereco retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envEndereco();

            if (empresa == null)
                return null;

            retorno.Bairro = empresa.Bairro;
            retorno.CEP = empresa.Bairro;
            retorno.Logradouro = empresa.Endereco;
            retorno.Numero = empresa.Numero;

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envParticipante obterEmpresa(Dominio.Entidades.Empresa empresa)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envParticipante retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envParticipante();

            if (empresa == null)
                return null;

            retorno.Cidade = empresa.Localidade?.Descricao ?? "";
            retorno.CPF_CNPJ = empresa.CNPJ_Formatado;
            retorno.CodigoExterno = empresa.Codigo.ToString();
            retorno.Condutor = false;
            retorno.Nome = empresa.RazaoSocial;
            retorno.UF = obterCodigoDeUF(empresa.Localidade?.Estado?.Sigla ?? "");
            retorno.Complemento = this.obterComplemento(empresa);
            retorno.Endereco = this.obterEndereco(empresa);
            retorno.JuridicaComplemento = this.obterJuridicaComplemento(empresa);
            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envParticipante obterEmpresa(Dominio.Entidades.Cliente pessoa)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envParticipante retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envParticipante();

            if (pessoa == null)
                return null;

            retorno.Cidade = pessoa.Localidade?.Descricao ?? "";
            retorno.CPF_CNPJ = pessoa.CPF_CNPJ_SemFormato;
            retorno.CodigoExterno = pessoa.Codigo.ToString();
            retorno.Condutor = false;
            retorno.Nome = pessoa.Nome;
            retorno.UF = obterCodigoDeUF(pessoa.Localidade?.Estado?.Sigla ?? "");
            retorno.Complemento = this.obterComplemento(pessoa);
            retorno.Endereco = this.obterEndereco(pessoa);
            retorno.JuridicaComplemento = this.obterJuridicaComplemento(pessoa);
            return retorno;
        }
        private Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envParticipante obterEmpresa(Dominio.Entidades.Usuario usuario)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envParticipante retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envParticipante();

            if (usuario == null)
                return null;

            retorno.Cidade = usuario.Localidade?.Descricao ?? "";
            retorno.CPF_CNPJ = usuario.CPF;
            retorno.CodigoExterno = usuario.CPF.ToString();
            retorno.Condutor = false;
            retorno.Nome = usuario.Nome;
            retorno.UF = obterCodigoDeUF(usuario.Localidade?.Estado?.Sigla ?? "");
            retorno.Complemento = this.obterComplemento(usuario);
            retorno.Endereco = this.obterEndereco(usuario);
            retorno.JuridicaComplemento = null;
            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envJuridicaComplemento obterJuridicaComplemento(Dominio.Entidades.Empresa empresa)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envJuridicaComplemento retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envJuridicaComplemento();

            if (empresa == null)
                return null;

            retorno.InscricaoEstadual = empresa.InscricaoEstadual;
            retorno.NomeFantasia = empresa.NomeFantasia;

            return retorno;
        }
        private Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envJuridicaComplemento obterJuridicaComplemento(Dominio.Entidades.Cliente pessoa)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envJuridicaComplemento retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envJuridicaComplemento();

            if (pessoa == null)
                return null;

            retorno.InscricaoEstadual = pessoa.IE_RG;
            retorno.NomeFantasia = pessoa.NomeFantasia;

            return retorno;
        }
        #endregion
    }
}
