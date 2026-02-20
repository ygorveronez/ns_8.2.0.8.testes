using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Collections.Generic;
using System.Collections;


namespace Servicos.Embarcador.Integracao.KMM
{
    public partial class IntegracaoKMM
    {

        #region Métodos Públicos
        public void CancelarNFS(Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe nfsManualCancelamentoIntegracaoCTe)
        {
            Repositorio.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe repositorioIntegracao = new Repositorio.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe(_unitOfWork);

            Repositorio.Embarcador.Configuracoes.IntegracaoKMM repositorioIntegracaoKMM = new Repositorio.Embarcador.Configuracoes.IntegracaoKMM(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKMM configuracaoIntegracaoKMM = repositorioIntegracaoKMM.Buscar();

            if (!(configuracaoIntegracaoKMM?.PossuiIntegracao ?? false))
            {
                nfsManualCancelamentoIntegracaoCTe.DataIntegracao = DateTime.Now;
                nfsManualCancelamentoIntegracaoCTe.NumeroTentativas++;
                nfsManualCancelamentoIntegracaoCTe.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                nfsManualCancelamentoIntegracaoCTe.ProblemaIntegracao = "Não possui configuração para KMMM";

                repositorioIntegracao.Atualizar(nfsManualCancelamentoIntegracaoCTe);

                return;
            }

            if (nfsManualCancelamentoIntegracaoCTe.NFSManualCancelamento.LancamentoNFSManual.DadosNFS == null)
            {
                nfsManualCancelamentoIntegracaoCTe.DataIntegracao = DateTime.Now;
                nfsManualCancelamentoIntegracaoCTe.NumeroTentativas++;
                nfsManualCancelamentoIntegracaoCTe.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                nfsManualCancelamentoIntegracaoCTe.ProblemaIntegracao = "A nota fiscal não está informada.";

                repositorioIntegracao.Atualizar(nfsManualCancelamentoIntegracaoCTe);
                return;
            }

            String token = RecuperarToken(configuracaoIntegracaoKMM);
            string url = $"{configuracaoIntegracaoKMM.URL}";

            string jsonRequisicao = "";
            string jsonRetorno = "";

            Hashtable body = new Hashtable
            {
                { "num_nota_fiscal", nfsManualCancelamentoIntegracaoCTe.NFSManualCancelamento.LancamentoNFSManual.DadosNFS.Numero },
                { "serie", nfsManualCancelamentoIntegracaoCTe.NFSManualCancelamento.LancamentoNFSManual.DadosNFS.Serie },
                { "cnpj_emitente", nfsManualCancelamentoIntegracaoCTe.NFSManualCancelamento.LancamentoNFSManual.Filial.CNPJ },
                { "data_emissao", nfsManualCancelamentoIntegracaoCTe.NFSManualCancelamento.LancamentoNFSManual.DataCriacao.ToString("dd/mm/yyyy") }
            };

            Hashtable request = new Hashtable
            {
                { "module", "M325.INTEGRACAO" },
                { "operation", "cancelarDocumento" },
                { "parameters", body }
            };

            try
            {
                HttpClient requisicao = CriarRequisicao(url, token);

                nfsManualCancelamentoIntegracaoCTe.DataIntegracao = DateTime.Now;
                nfsManualCancelamentoIntegracaoCTe.NumeroTentativas++;

                jsonRequisicao = JsonConvert.SerializeObject(request, Formatting.Indented);

                StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage retornoRequisicao = requisicao.PostAsync(url, conteudoRequisicao).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                if (retornoRequisicao.StatusCode == HttpStatusCode.OK)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.RetornoPadrao retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.RetornoPadrao>(jsonRetorno);

                    if (!retorno.Success)
                        throw new ServicoException($"{retorno.Message}\n{retorno.Details}");

                    nfsManualCancelamentoIntegracaoCTe.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    nfsManualCancelamentoIntegracaoCTe.ProblemaIntegracao = retorno.Result["mensagem"].ToString();
                }
                else if (jsonRetorno.IndexOf("message") >= 0)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.RetornoPadrao retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.RetornoPadrao>(jsonRetorno);
                    if (!retorno.Success)
                        throw new ServicoException($"{retorno.Message.Substring(0, 20)}");
                }
                else
                    throw new ServicoException($"Falha ao conectar no WS KMM: {retornoRequisicao.StatusCode}");
            }

            catch (ServicoException ex)
            {
                nfsManualCancelamentoIntegracaoCTe.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                nfsManualCancelamentoIntegracaoCTe.ProblemaIntegracao = ex.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                nfsManualCancelamentoIntegracaoCTe.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                nfsManualCancelamentoIntegracaoCTe.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração da KMM";
            }

            AdicionarArquivoTransacaoIntegracaoNFSManual(nfsManualCancelamentoIntegracaoCTe, jsonRequisicao, jsonRetorno, _unitOfWork);
            repositorioIntegracao.Atualizar(nfsManualCancelamentoIntegracaoCTe);
        }

        #endregion


        #region Métodos Públicos
        private static void AdicionarArquivoTransacaoIntegracaoNFSManual(Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe nfsManualCancelamentoIntegracaoCTe, string jsonRequisicao, string jsonRetorno, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoArquivo arquivoIntegracao = AdicionarArquivoTransacao(jsonRequisicao, jsonRetorno, nfsManualCancelamentoIntegracaoCTe.DataIntegracao, nfsManualCancelamentoIntegracaoCTe.ProblemaIntegracao, unitOfWork);

            if (arquivoIntegracao == null)
                return;

            if (nfsManualCancelamentoIntegracaoCTe.ArquivosTransacao == null)
                nfsManualCancelamentoIntegracaoCTe.ArquivosTransacao = new List<Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoArquivo>();

            nfsManualCancelamentoIntegracaoCTe.ArquivosTransacao.Add(arquivoIntegracao);
        }

        private static Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoArquivo AdicionarArquivoTransacao(string jsonRequisicao, string jsonRetorno, DateTime data, string mensagem, Repositorio.UnitOfWork unitOfWork)
        {
            if (string.IsNullOrWhiteSpace(jsonRequisicao) && string.IsNullOrWhiteSpace(jsonRetorno))
                return null;

            Repositorio.Embarcador.NFS.NFSManualIntegracaoArquivo repositorio = new Repositorio.Embarcador.NFS.NFSManualIntegracaoArquivo(unitOfWork);
            Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoArquivo()
            {
                ArquivoRequisicao = ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequisicao, "json", unitOfWork),
                ArquivoResposta = ArquivoIntegracao.SalvarArquivoIntegracao(jsonRetorno, "json", unitOfWork),
                Data = data,
                Mensagem = mensagem,
                Tipo = TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento
            };

            repositorio.Inserir(arquivoIntegracao);

            return arquivoIntegracao;
        }

        #endregion
    }
}
