using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
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

        public bool IntegrarVeiculo(ref Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, Dominio.Entidades.Veiculo veiculo)
        {
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            string jsonRequisicao = "";
            string jsonRetorno = "";

            bool sucesso = false;

            try
            {
                object request = this.obterVeiculo(veiculo, cargaIntegracao.Carga);
                Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.retornoWebService retWS = transmitir("CadastroVeiculoIntegracao/Integrar", request);

                jsonRequisicao = retWS.jsonRequisicao;
                jsonRetorno = retWS.jsonRetorno;

                if (retWS.SituacaoIntegracao == SituacaoIntegracao.Integrado)
                    sucesso = true;
                else
                    throw new ServicoException($"Veículo {veiculo.Placa} - " + retWS.ProblemaIntegracao);

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

                cargaIntegracao.ProblemaIntegracao = "Erro ao tentar integrar veículo com a ATS Smart Web";
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }

            servicoArquivoTransacao.Adicionar(cargaIntegracao, jsonRequisicao, jsonRetorno, "json", "Integração de Veículo");

            repCargaIntegracao.Atualizar(cargaIntegracao);

            return sucesso;
        }
        public bool IntegrarVeiculo(ref Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao, Dominio.Entidades.Veiculo veiculo)
        {
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            string jsonRequisicao = "";
            string jsonRetorno = "";

            bool sucesso = false;

            try
            {
                object request = this.obterVeiculo(veiculo, cargaDadosTransporteIntegracao.Carga);
                Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.retornoWebService retWS = transmitir("CadastroVeiculoIntegracao/Integrar", request);

                jsonRequisicao = retWS.jsonRequisicao;
                jsonRetorno = retWS.jsonRetorno;

                if (retWS.SituacaoIntegracao == SituacaoIntegracao.Integrado)
                    sucesso = true;
                else
                    throw new ServicoException($"Veículo {veiculo.Placa} - " + retWS.ProblemaIntegracao);

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

                cargaDadosTransporteIntegracao.ProblemaIntegracao = "Erro ao tentar integrar veículo com a ATS Smart Web";
                cargaDadosTransporteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }

            servicoArquivoTransacao.Adicionar(cargaDadosTransporteIntegracao, jsonRequisicao, jsonRetorno, "json", "Integração de Veículo");

            repCargaDadosTransporteIntegracao.Atualizar(cargaDadosTransporteIntegracao);

            return sucesso;
        }
        public bool IntegrarVeiculosVinculados(ref Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, Dominio.Entidades.Veiculo tracao)
        {

            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            bool sucesso = false;

            try
            {
                if (tracao.VeiculosVinculados == null || tracao.VeiculosVinculados.Count == 0)
                    return true;

                foreach (var veiculo in tracao.VeiculosVinculados)
                {
                    object request = this.obterVeiculo(veiculo, cargaIntegracao.Carga);
                    Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.retornoWebService retWS = transmitir("CadastroVeiculoIntegracao/Integrar", request);

                    servicoArquivoTransacao.Adicionar(cargaIntegracao, retWS.jsonRequisicao, retWS.jsonRetorno, "json", "Integração de Veículos Vinculados a carga");

                    if (retWS.SituacaoIntegracao == SituacaoIntegracao.Integrado)
                        sucesso = true;
                    else
                        throw new ServicoException($"Veículo {veiculo.Placa} - " + retWS.ProblemaIntegracao);

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

                cargaIntegracao.ProblemaIntegracao = "Erro ao tentar integrar veículos vinculados a carga com a ATS Smart Web";
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }

            repCargaIntegracao.Atualizar(cargaIntegracao);

            return sucesso;
        }

        public bool IntegrarVeiculosVinculados(ref Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao, Dominio.Entidades.Veiculo tracao)
        {

            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            bool sucesso = false;

            try
            {
                if (tracao.VeiculosVinculados == null || tracao.VeiculosVinculados.Count == 0)
                    return true;

                foreach (var veiculo in tracao.VeiculosVinculados)
                {
                    object request = this.obterVeiculo(veiculo, cargaDadosTransporteIntegracao.Carga);
                    Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.retornoWebService retWS = transmitir("CadastroVeiculoIntegracao/Integrar", request);

                    servicoArquivoTransacao.Adicionar(cargaDadosTransporteIntegracao, retWS.jsonRequisicao, retWS.jsonRetorno, "json", "Integração de Veículos Vinculados a carga");

                    if (retWS.SituacaoIntegracao == SituacaoIntegracao.Integrado)
                        sucesso = true;
                    else
                        throw new ServicoException($"Veículo {veiculo.Placa} - " + retWS.ProblemaIntegracao);
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

                cargaDadosTransporteIntegracao.ProblemaIntegracao = "Erro ao tentar integrar veículos vinculados a carga com a ATS Smart Web";
                cargaDadosTransporteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }

            repCargaDadosTransporteIntegracao.Atualizar(cargaDadosTransporteIntegracao);

            return sucesso;
        }
        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envVeiculo obterVeiculo(Dominio.Entidades.Veiculo veiculo, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envVeiculo retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envVeiculo();

            if (veiculo == null)
                return null;

            retorno.Placa = veiculo.Placa;
            retorno.Manutencao = false;
            retorno.Proprietario = this.obterProprietario(veiculo, carga);
            retorno.Tipo = this.obterTipo(veiculo);
            retorno.Caracteristicas = this.obterCaracteristicas(veiculo);
            
            if(veiculo.PossuiRastreador)
                retorno.Rastreador = this.obterRastreador(veiculo);

            return retorno;
        }
        private Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envProprietario obterProprietario(Dominio.Entidades.Veiculo veiculo, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envProprietario retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envProprietario();

            if(veiculo.Tipo == "P")
            {
                retorno.Nome = veiculo.EmpresaFilial?.RazaoSocial ?? carga.Empresa?.RazaoSocial ?? "";
                retorno.CPF_CNPJ = veiculo.EmpresaFilial?.CNPJ_SemFormato ?? carga.Empresa?.CNPJ_SemFormato ?? "";
                retorno.CodigoExterno = veiculo.EmpresaFilial?.Codigo.ToString() ?? carga.Empresa?.Codigo.ToString() ?? "";
                retorno.Condutor = false;
                retorno.Cidade = veiculo.EmpresaFilial?.Localidade?.Descricao ?? carga.Empresa?.Localidade?.Descricao ?? "";
                retorno.UF = obterCodigoDeUF(veiculo.EmpresaFilial?.Localidade?.Estado?.Sigla ?? carga.Empresa?.Localidade?.Estado?.Sigla ?? "");
                retorno.Complemento = this.obterComplemento(veiculo.EmpresaFilial ?? carga.Empresa);
                retorno.Endereco = this.obterEndereco(veiculo.EmpresaFilial ?? carga.Empresa);
                retorno.JuridicaComplemento = this.obterJuridicaComplemento(veiculo.EmpresaFilial ?? carga.Empresa);
            }
            else
            {
                retorno.Nome = veiculo.Proprietario?.Nome ?? "";
                retorno.CPF_CNPJ = veiculo.Proprietario?.CPF_CNPJ_SemFormato ?? "";
                retorno.CodigoExterno = veiculo.Proprietario?.CPF_CNPJ_SemFormato.ToString() ?? "";
                retorno.Condutor = false;
                retorno.Cidade = veiculo.Proprietario?.Localidade?.Descricao ?? "";
                retorno.UF = obterCodigoDeUF(veiculo.Proprietario?.Localidade?.Estado?.Sigla ?? "");
                retorno.Complemento = this.obterComplemento(veiculo.Proprietario);
                retorno.Endereco = this.obterEndereco(veiculo.Proprietario);

                if(veiculo.Proprietario.Tipo.Equals("J"))
                    retorno.JuridicaComplemento = this.obterJuridicaComplemento(veiculo.Proprietario);

            }
            

            return retorno;
        }
        private Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envTipo obterTipo(Dominio.Entidades.Veiculo veiculo)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envTipo retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envTipo();

            retorno.Nome = veiculo.DescricaoTipo;
            retorno.Sigla = veiculo.DescricaoTipoRodado;
            retorno.Tracao = veiculo.TipoVeiculo == "0";

            return retorno;
        }
        private Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envCaracteristicas obterCaracteristicas(Dominio.Entidades.Veiculo veiculo)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envCaracteristicas retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envCaracteristicas();

            retorno.AnoFabricacao = veiculo.AnoFabricacao;
            retorno.AnoModelo = veiculo.AnoModelo;
            retorno.Chassi = veiculo.Chassi;
            retorno.Cor = veiculo.Cor;
            retorno.Frota = veiculo.NumeroFrota;
            retorno.Marca = veiculo.Marca?.Descricao ?? "";
            retorno.Modelo = veiculo.Modelo?.Descricao ?? "";

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envRastreador obterRastreador(Dominio.Entidades.Veiculo veiculo)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envRastreador retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envRastreador();

            retorno.Numero = veiculo?.NumeroEquipamentoRastreador;
            retorno.TecnologiaRastreamento = this.ObterCodigoRastreador(veiculo?.TecnologiaRastreador?.TipoIntegracao);
            retorno.TipoComunicacao = int.Parse(veiculo?.TipoComunicacaoRastreador?.CodigoIntegracao ?? "0");

            return retorno;
        }

        private int ObterCodigoRastreador(TipoIntegracao? tipoIntegracao)
        {

            if (tipoIntegracao == null)
                return 0;


            switch (tipoIntegracao)
            {
                case TipoIntegracao.ATSLog: return 0;
                case TipoIntegracao.OnixSat: return 1;
                case TipoIntegracao.Sascar: return 2;
                case TipoIntegracao.Omnilink: return 7;
                case TipoIntegracao.Autotrac: return 18;
                case TipoIntegracao.Sighra: return 27;
                case TipoIntegracao.Positron: return 42;
                case TipoIntegracao.Ravex: return 48;
                case TipoIntegracao.MixTelematics: return 51;
                case TipoIntegracao.Getrak: return 63;
                case TipoIntegracao.Maxtrack: return 74;
                default: return 0; // Código inválido ou rastreador não mapeado
            };
        }

        #endregion
    }
}
