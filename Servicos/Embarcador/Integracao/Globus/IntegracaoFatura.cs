using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Integracao.Globus.Fatura;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Servicos.Embarcador.Integracao.Globus
{
    public partial class IntegracaoGlobus
    {
        public void IntegrarFatura(Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao faturaIntegracao)
        {
            Repositorio.Embarcador.Fatura.FaturaIntegracao repositorioFaturaIntegracao = new Repositorio.Embarcador.Fatura.FaturaIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            string jsonRequisicao = "";
            string jsonRetorno = "";

            try
            {
                faturaIntegracao.DataEnvio = DateTime.Now;
                faturaIntegracao.Tentativas++;

                if (_configuracaoIntegracao.ShortCodeFinanceiro == 0 || String.IsNullOrEmpty(_configuracaoIntegracao.TokenFinanceiro) || String.IsNullOrEmpty(_configuracaoIntegracao.URLWebServiceFinanceiro))
                    throw new Exception("Processo abortado, configuração não encontrada ou incompleta!");

                var request = ConverterFaturaCTe(faturaIntegracao);

                var retWS = this.Transmitir(request, string.Concat("api/ContasReceber/SubstituirDocumento?shortCode=", _configuracaoIntegracao.ShortCodeFinanceiro, "&token=", _configuracaoIntegracao.TokenFinanceiro, "&inscricaoEmpresa=", faturaIntegracao.Fatura.Empresa.CNPJ), null, _configuracaoIntegracao.URLWebServiceFinanceiro);

                if (retWS != null && retWS.SituacaoIntegracao == SituacaoIntegracao.Integrado && !String.IsNullOrEmpty(retWS.jsonRetorno))
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Globus.retornoWebServiceSuccess retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Globus.retornoWebServiceSuccess>(retWS.jsonRetorno);

                    faturaIntegracao.CodigoExternoRetornoIntegracao = retorno?.data?.idExterno ?? "0";
                }

                faturaIntegracao.SituacaoIntegracao = retWS.SituacaoIntegracao;
                faturaIntegracao.MensagemRetorno = retWS.ProblemaIntegracao;
                jsonRequisicao = retWS.jsonRequisicao;
                jsonRetorno = retWS.jsonRetorno;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                faturaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

                String message = excecao.Message;
                if (message.Length > 300)
                {
                    message = message.Substring(0, 300);
                }
                faturaIntegracao.MensagemRetorno = message;
            }

            SalvarArquivosIntegracaoFatura(faturaIntegracao, jsonRequisicao, jsonRetorno);

            repositorioFaturaIntegracao.Atualizar(faturaIntegracao);
        }

        private FaturaEnvio ConverterFaturaCTe(Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao faturaIntegracao)
        {
            Repositorio.Embarcador.Fatura.FaturaIntegracao repositorioFaturaIntegracao = new Repositorio.Embarcador.Fatura.FaturaIntegracao(_unitOfWork);

            int numeroTitulo = repositorioFaturaIntegracao.ObterNumeroDoTitulo(faturaIntegracao.Fatura.Codigo);
            List<DocumentoSubstituido> documentosSubstituidos = ObterCodigosIntegracaoCTe(faturaIntegracao.Fatura);

            FaturaEnvio faturaRetorno = new FaturaEnvio();

            faturaRetorno.InscricaoCliente = faturaIntegracao.Fatura?.Cliente?.CPF_CNPJ_SemFormato ?? "";
            faturaRetorno.CodigoInternoCliente = 0;
            faturaRetorno.CodigoTipoDocumento = _configuracaoIntegracao.CodigoIntegrarComContasReceber ?? "";
            faturaRetorno.Serie = "1";
            faturaRetorno.NumeroDocumento = numeroTitulo.ToString().PadLeft(10, '0') ?? "";
            faturaRetorno.Emissao = faturaIntegracao.Fatura?.DataInicial?.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") ?? "";
            faturaRetorno.Vencimento = faturaIntegracao.Fatura?.DataFinal?.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") ?? "";
            faturaRetorno.Observacao = faturaIntegracao.Fatura?.ObservacaoFatura.Length > 100 ? "Tamanho máximo de 100 caracteres ultrapassado!" : !string.IsNullOrEmpty(faturaIntegracao.Fatura.ObservacaoFatura) ? faturaIntegracao.Fatura.ObservacaoFatura : "";
            faturaRetorno.Usuario = _configuracaoIntegracao.Usuario;
            faturaRetorno.IntegrarContabilidade = false;
            faturaRetorno.CodigoHistoricoContabil = 0;
            faturaRetorno.ItemDocumento = new ItemDocumento()
            {
                CodigoTipoReceita = faturaIntegracao.Fatura.TipoMovimentoUso.CodigoIntegracao,
                Observacao = faturaIntegracao.Fatura.ObservacaoFatura ?? ""
            };
            faturaRetorno.DocumentoSubstituido = documentosSubstituidos;

            return faturaRetorno;
        }

        private List<DocumentoSubstituido> ObterCodigosIntegracaoCTe(Dominio.Entidades.Embarcador.Fatura.Fatura fatura)
        {
            Repositorio.Embarcador.Cargas.CargaCTeIntegracao repositorioCargaCTeIntegracao = new Repositorio.Embarcador.Cargas.CargaCTeIntegracao(_unitOfWork);
            List<DocumentoSubstituido> codigosIntegracaoCTe = new List<DocumentoSubstituido>();

            foreach (Dominio.Entidades.Embarcador.Fatura.FaturaDocumento documento in fatura.Documentos)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao> integracaoCTe = repositorioCargaCTeIntegracao.BuscarPorCTe(documento.Documento.CTe.Codigo);

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao integracao in integracaoCTe)
                {
                    if (integracaoCTe != null && (integracao.CargaCTe?.CTe?.Codigo == documento.Documento?.CTe?.Codigo))
                    {
                        DocumentoSubstituido doc = new DocumentoSubstituido()
                        {
                            Codigo = integracao.CodigoExternoRetornoIntegracao
                        };

                        codigosIntegracaoCTe.Add(doc);
                    }
                    else
                    {
                        throw new Exception($"O CTe {documento.Documento.CTe.Numero} adicionado não possui integração!");
                    }
                }
            }

            if (codigosIntegracaoCTe.IsNullOrEmpty())
                throw new Exception("Não foi encontrado nenhum registro de integração dos CT-es que compõem a fatura!");

            return codigosIntegracaoCTe;
        }

        private void SalvarArquivosIntegracaoFatura(Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao faturaIntegracao, string arquivoRequisicao, string arquivoRetorno)
        {
            Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoArquivo faturaIntegracaoArquivo = AdicionarArquivoTransacaoFatura(arquivoRequisicao, arquivoRetorno, faturaIntegracao.MensagemRetorno);

            if (faturaIntegracaoArquivo == null)
                return;

            if (faturaIntegracao.ArquivosIntegracao == null)
                faturaIntegracao.ArquivosIntegracao = new List<Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoArquivo>();

            faturaIntegracao.ArquivosIntegracao.Add(faturaIntegracaoArquivo);
        }

        private Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoArquivo AdicionarArquivoTransacaoFatura(string arquivoRequisicao, string arquivoRetorno, string mensagem)
        {
            if (string.IsNullOrWhiteSpace(arquivoRequisicao) && string.IsNullOrWhiteSpace(arquivoRetorno))
                return null;

            Repositorio.Embarcador.Fatura.FaturaIntegracaoArquivo repositorioFaturaIntegracaoArquivo = new Repositorio.Embarcador.Fatura.FaturaIntegracaoArquivo(_unitOfWork);
            Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoArquivo faturaIntegracaoArquivo = new Dominio.Entidades.Embarcador.Fatura.FaturaIntegracaoArquivo()
            {
                ArquivoRequisicao = ArquivoIntegracao.SalvarArquivoIntegracao(arquivoRequisicao, "json", _unitOfWork),
                ArquivoResposta = ArquivoIntegracao.SalvarArquivoIntegracao(arquivoRetorno, "json", _unitOfWork),
                Data = DateTime.Now,
                Mensagem = mensagem,
                Tipo = TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento
            };

            repositorioFaturaIntegracaoArquivo.Inserir(faturaIntegracaoArquivo);

            return faturaIntegracaoArquivo;
        }

    }
}
