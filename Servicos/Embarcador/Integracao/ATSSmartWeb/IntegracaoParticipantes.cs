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

        public bool IntegrarParticipantes(ref Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            bool sucesso = false;
            try
            {
                List<Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envParticipante> participantes = this.obterParticipantes(cargaIntegracao.Carga);

                foreach (var participante in participantes)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.retornoWebService retWS = transmitir("CadastroPessoaIntegracao/Integrar", participante);

                    servicoArquivoTransacao.Adicionar(cargaIntegracao, retWS.jsonRequisicao, retWS.jsonRetorno, "json", "Integração de Participante");

                    if (retWS.SituacaoIntegracao == SituacaoIntegracao.Integrado)
                        sucesso = true;
                    else
                        throw new ServicoException($" {participante.CPF_CNPJ} - " + retWS.ProblemaIntegracao);

                }
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

            repCargaIntegracao.Atualizar(cargaIntegracao);

            return sucesso;
        }

        public bool IntegrarParticipantes(ref Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            bool sucesso = false;

            try
            {
                List<Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envParticipante> participantes = this.obterParticipantes(cargaDadosTransporteIntegracao.Carga);

                foreach (var participante in participantes)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.retornoWebService retWS = transmitir("CadastroPessoaIntegracao/Integrar", participante);

                    servicoArquivoTransacao.Adicionar(cargaDadosTransporteIntegracao, retWS.jsonRequisicao, retWS.jsonRetorno, "json", "Integração de Participante");

                    if (retWS.SituacaoIntegracao == SituacaoIntegracao.Integrado)
                        sucesso = true;
                    else
                        throw new ServicoException($" {participante.CPF_CNPJ} - " + retWS.ProblemaIntegracao);

                }
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

            repCargaDadosTransporteIntegracao.Atualizar(cargaDadosTransporteIntegracao);

            return sucesso;
        }

        #endregion

        #region Métodos Privados
        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envParticipante> obterParticipantes(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envParticipante> retorno = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envParticipante>();

            Dominio.Entidades.Veiculo veiculo = carga.Veiculo;

            if (veiculo == null)
                veiculo = carga.VeiculosVinculados.FirstOrDefault();

            if (veiculo == null)
                throw new ServicoException(@"Veículo não definido na carga");

            if (veiculo.Tipo.Equals("P"))
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envParticipante participante = new Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envParticipante();
                participante.Nome = veiculo.EmpresaFilial?.RazaoSocial ?? carga.Empresa?.RazaoSocial ?? "";
                participante.CPF_CNPJ = veiculo.EmpresaFilial?.CNPJ_SemFormato ?? carga.Empresa?.CNPJ_SemFormato ?? "";
                participante.CodigoExterno = veiculo.EmpresaFilial?.Codigo.ToString() ?? carga.Empresa?.Codigo.ToString() ?? "";
                participante.Condutor = false;
                participante.Cidade = veiculo.EmpresaFilial?.Localidade?.Descricao ?? carga.Empresa?.Localidade?.Descricao ?? "";
                participante.UF = obterCodigoDeUF(veiculo.EmpresaFilial?.Localidade?.Estado?.Sigla ?? carga.Empresa?.Localidade?.Estado?.Sigla ?? "");
                participante.JuridicaComplemento = this.obterJuridicaComplemento(veiculo.EmpresaFilial ?? carga.Empresa);
                participante.Complemento = this.obterComplemento(veiculo.EmpresaFilial ?? carga.Empresa);
                retorno.Add(participante);
            }
            else
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envParticipante participante = new Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envParticipante();
                participante.Nome = veiculo.Proprietario?.Nome ?? "";
                participante.CPF_CNPJ = veiculo.Proprietario?.CPF_CNPJ_SemFormato ?? "";
                participante.CodigoExterno = veiculo.Proprietario?.CPF_CNPJ_SemFormato.ToString() ?? "";
                participante.Condutor = false;
                participante.Cidade = veiculo.Proprietario?.Localidade?.Descricao ?? "";
                participante.UF = obterCodigoDeUF(veiculo.Proprietario?.Localidade?.Estado?.Sigla ?? "");
                participante.JuridicaComplemento = this.obterJuridicaComplemento(veiculo.Proprietario);
                participante.Complemento = this.obterComplemento(veiculo.Proprietario); 
                retorno.Add(participante);
            }

            return retorno;
        }
        #endregion
    }
}
