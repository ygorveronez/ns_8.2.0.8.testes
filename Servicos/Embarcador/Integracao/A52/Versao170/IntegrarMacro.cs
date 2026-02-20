using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Integracao.A52.V170;
using Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Pedidos;
using Dominio.ObjetosDeValor.Embarcador.Integracao.KuehneNagel;
using Utilidades.Extensions;

namespace Servicos.Embarcador.Integracao.A52.V170
{
    public partial class IntegracaoA52
    {

        #region Métodos Públicos

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

            string jsonEnvio = string.Empty;
            string jsonRetorno = string.Empty;
            string mensagemErro = string.Empty;

            if (ObterToken(out mensagemErro) &&
                IntegrarMacros(veiculoIntegracao.Veiculo, veiculoIntegracao.TipoIntegracao, out jsonEnvio, out jsonRetorno, out mensagemErro))
            {
                veiculoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                veiculoIntegracao.ProblemaIntegracao = "Registro integrado com sucesso.";
            }
            else
            {
                veiculoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                veiculoIntegracao.ProblemaIntegracao = mensagemErro;
            }

            SalvarArquivosIntegracao(veiculoIntegracao, jsonEnvio, jsonRetorno);

            repVeiculoIntegracao.Atualizar(veiculoIntegracao);
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private bool IntegrarMacros(Dominio.Entidades.Veiculo veiculo, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao, out string jsonEnvio, out string jsonRetorno, out string mensagemErro)
        {
            Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(_unitOfWork);
            Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista veiculoMotorista = repVeiculoMotorista.BuscarVeiculoMotoristaPrincipal(veiculo.Codigo);

            jsonEnvio = string.Empty;
            jsonRetorno = string.Empty;

            if (veiculoMotorista == null || veiculoMotorista.Motorista == null)
            {
                mensagemErro = "Nenhum motorista vinculado ao veículo.";
                return false;
            }

            bool sucesso = false;
            mensagemErro = string.Empty;

            try
            {
                object envioWS = ObterMacro(veiculo, tipoIntegracao, veiculoMotorista);

                //Transmite o arquivo
                retornoWebService retornoWS = retornoWS = this.TransmitirRepom(enumTipoWS.POST, envioWS, "macros", this.tokenAutenticacao);

                jsonEnvio = retornoWS.jsonEnvio;
                jsonRetorno = retornoWS.jsonRetorno;

                if (retornoWS.erro && !string.IsNullOrEmpty(retornoWS.jsonRetorno))
                {
                    retError retorno = null;

                    try
                    {
                        retorno = retornoWS.jsonRetorno.FromJson<retError>();
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao deserializar retorno JSON A52 macro: {ex.ToString()}", "CatchNoAction");
                    }

                    if (retorno == null)
                        mensagemErro = string.Format("Message: Ocorreu uma falha ao enviar a macro; RetornoWS {0}.", retornoWS.jsonRetorno);
                    else
                    {
                        string mensagem = null;
                        retRota retRota = null;

                        int count = 0;
                        foreach (object message in retorno.message)
                        {
                            count++;

                            if (count == 1)
                                mensagem = message.ToString();
                        }

                        if (string.IsNullOrEmpty(mensagem))
                            mensagemErro = "Ocorreu uma falha ao enviar a macro.";
                        else if (retRota == null)
                            mensagemErro = string.Format("Message: Ocorreu uma falha ao enviar a macro; RetornoWS {0}.", mensagem);
                    }
                }
                else if (retornoWS.erro)
                    mensagemErro = "Ocorreu uma falha ao enviar a macro.";
                else
                    sucesso = true;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                mensagemErro = "Não foi possível realizar a integração, verifique os arquivos de integração.";
            }

            return sucesso;
        }

        private envMacro ObterMacro(Dominio.Entidades.Veiculo veiculo, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao, Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista veiculoMotorista)
        {
            Repositorio.Embarcador.Veiculos.TecnologiaRastreadorCodigoIntegracao repTecnologiaRastreadorCodigoIntegracao = new Repositorio.Embarcador.Veiculos.TecnologiaRastreadorCodigoIntegracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Veiculos.TecnologiaRastreadorCodigoIntegracao tecnologiaRastreadorCodigoIntegracao = null;

            if (veiculo.PossuiRastreador && veiculo.TecnologiaRastreador != null)
                tecnologiaRastreadorCodigoIntegracao = repTecnologiaRastreadorCodigoIntegracao.BuscarPorTecnologiaRastreadorETipoIntegracao(veiculo.TecnologiaRastreador, tipoIntegracao);

            envMacro retorno = new envMacro();
            retorno.idIdent = null;

            int idTecnologia;
            if (!string.IsNullOrEmpty(tecnologiaRastreadorCodigoIntegracao?.CodigoIntegracao) && int.TryParse(tecnologiaRastreadorCodigoIntegracao?.CodigoIntegracao, out idTecnologia))
                retorno.idTecnologia = idTecnologia;

            retorno.mct = veiculo.NumeroEquipamentoRastreador;
            retorno.dataHora = DateTime.Now.ToString("u", new System.Globalization.CultureInfo("pt-BR")).Replace(" ", "T");
            retorno.latitude = 0;
            retorno.longitude = 0;
            retorno.idMacro = 99;
            retorno.textoMacro = Utilidades.String.OnlyNumbers(veiculoMotorista.Motorista.CPF);
            return retorno;
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

        #endregion Métodos Privados
    }
}
