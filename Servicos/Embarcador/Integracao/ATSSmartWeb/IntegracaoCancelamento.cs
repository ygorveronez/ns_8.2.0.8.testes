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
        public void IntegrarCargaCancelamento(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cargaCancelamentoCargaIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            string jsonRequisicao = "";
            string jsonRetorno = "";

            try
            {
                if (string.IsNullOrWhiteSpace(_configuracaoIntegracaoATSSmartWeb?.URL))
                    throw new ServicoException("Não há URL configurada para a integração");

                if (string.IsNullOrWhiteSpace(_configuracaoIntegracaoATSSmartWeb?.CNPJCompany) || string.IsNullOrWhiteSpace(_configuracaoIntegracaoATSSmartWeb?.NomeCompany) || _configuracaoIntegracaoATSSmartWeb?.Localidade == null)
                    throw new ServicoException("Configuração da integração incompleta");


                cargaCancelamentoCargaIntegracao.NumeroTentativas += 1;
                cargaCancelamentoCargaIntegracao.DataIntegracao = DateTime.Now;

                Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.retornoWebService retWS = transmitir($"GestaoSolicitacaoMonitoramentoIntegracao/CancelaSMIntegracao?CodigoExterno={cargaCancelamentoCargaIntegracao.CargaCancelamento.Carga.Codigo}", null);

                if (retWS.SituacaoIntegracao == SituacaoIntegracao.Integrado)
                {
                    cargaCancelamentoCargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    cargaCancelamentoCargaIntegracao.ProblemaIntegracao = "Integração concluída com sucesso";
                }
                else
                {
                    cargaCancelamentoCargaIntegracao.ProblemaIntegracao = retWS.ProblemaIntegracao;
                    cargaCancelamentoCargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                }

                jsonRequisicao = retWS.jsonRequisicao;
                jsonRetorno = retWS.jsonRetorno;
                
            }
            catch (ServicoException excecao)
            {
                Log.TratarErro(excecao);

                String message = excecao.Message;
                if (message.Length > 300)
                {
                    message = message.Substring(0, 300);
                }
                cargaCancelamentoCargaIntegracao.ProblemaIntegracao = message;
                cargaCancelamentoCargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaCancelamentoCargaIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração do cancelamento da viagem com a ATS Smart Web";
                cargaCancelamentoCargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }

            servicoArquivoTransacao.Adicionar(cargaCancelamentoCargaIntegracao, jsonRequisicao, jsonRetorno, "json", "Integração cancelamento da Viagem");

            repCargaIntegracao.Atualizar(cargaCancelamentoCargaIntegracao);
        }
    }
}
