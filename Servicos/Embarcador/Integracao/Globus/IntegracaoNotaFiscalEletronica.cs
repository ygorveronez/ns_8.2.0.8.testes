using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Integracao.Globus.NotaFiscalEletronica;
using Newtonsoft.Json;
using System;

namespace Servicos.Embarcador.Integracao.Globus
{
    public partial class IntegracaoGlobus
    {
        #region Métodos Públicos

        public void IntegrarNotaFiscalEletronica(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalEletronicaIntegracao notaFiscalEletronicaIntegracao)
        {
            Repositorio.Embarcador.NotaFiscal.NotaFiscalEletronicaIntegracao repositorioNotaFiscalEletronicaIntegracao = new Repositorio.Embarcador.NotaFiscal.NotaFiscalEletronicaIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            string jsonRequisicao = "";
            string jsonRetorno = "";

            try
            {
                notaFiscalEletronicaIntegracao.DataIntegracao = DateTime.Now;
                notaFiscalEletronicaIntegracao.NumeroTentativas++;

                var configuracaoIntegracao = ObterConfiguracaoComunicacao(null, notaFiscalEletronicaIntegracao.NotaFiscal, null);

                string token = ObterToken(configuracaoIntegracao.EndPointToken, configuracaoIntegracao.URLWebService, configuracaoIntegracao.ShortCode, configuracaoIntegracao.Token, true);

                var request = ConverterNotaFiscalEletronica(notaFiscalEletronicaIntegracao.NotaFiscal);

                var retWS = this.Transmitir(request, configuracaoIntegracao.EndPoint, token, configuracaoIntegracao.URLWebService);

                if (retWS != null && retWS.SituacaoIntegracao == SituacaoIntegracao.Integrado && !String.IsNullOrEmpty(retWS.jsonRetorno))
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Globus.retornoWebServiceSuccess retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Globus.retornoWebServiceSuccess>(retWS.jsonRetorno);
                    notaFiscalEletronicaIntegracao.CodigoExternoRetornoIntegracao = retorno?.data?.idExterno ?? "0";
                }

                notaFiscalEletronicaIntegracao.SituacaoIntegracao = retWS.SituacaoIntegracao;
                notaFiscalEletronicaIntegracao.ProblemaIntegracao = retWS.ProblemaIntegracao;
                jsonRequisicao = retWS.jsonRequisicao;
                jsonRetorno = retWS.jsonRetorno;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                notaFiscalEletronicaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

                String message = excecao.Message;
                if (message.Length > 300)
                {
                    message = message.Substring(0, 300);
                }
                notaFiscalEletronicaIntegracao.ProblemaIntegracao = message;
            }

            servicoArquivoTransacao.Adicionar(notaFiscalEletronicaIntegracao, jsonRequisicao, jsonRetorno, "json");

            repositorioNotaFiscalEletronicaIntegracao.Atualizar(notaFiscalEletronicaIntegracao);
        }

        public void IntegrarCancelamentoNotaFiscalEletronica(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalEletronicaIntegracao notaFiscalEletronicaIntegracao)
        {
            Repositorio.Embarcador.NotaFiscal.NotaFiscalEletronicaIntegracao repositorioNotaFiscalEletronicaIntegracao = new Repositorio.Embarcador.NotaFiscal.NotaFiscalEletronicaIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            string jsonRequisicao = "";
            string jsonRetorno = "";
            string mensagemErro = "";
            try
            {
                notaFiscalEletronicaIntegracao.DataIntegracao = DateTime.Now;
                notaFiscalEletronicaIntegracao.NumeroTentativas++;

                this.ValidarEnvioIntegracao(notaFiscalEletronicaIntegracao.NotaFiscal, TipoAcaoIntegracao.Cancelamento);

                var configuracaoIntegracao = ObterConfiguracaoComunicacao(null, notaFiscalEletronicaIntegracao.NotaFiscal, null);

                string token = ObterToken(configuracaoIntegracao.EndPointToken, configuracaoIntegracao.URLWebService, configuracaoIntegracao.ShortCode, configuracaoIntegracao.Token, true);

                var request = ConverterNotaFiscalEletronica(notaFiscalEletronicaIntegracao.NotaFiscal, true);

                var retWS = Transmitir(request, configuracaoIntegracao.EndPoint, token, configuracaoIntegracao.URLWebService);

                if (retWS != null && retWS.SituacaoIntegracao == SituacaoIntegracao.Integrado && !String.IsNullOrEmpty(retWS.jsonRetorno))
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Globus.retornoWebServiceSuccess retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Globus.retornoWebServiceSuccess>(retWS.jsonRetorno);

                    notaFiscalEletronicaIntegracao.CodigoExternoRetornoIntegracao = retorno?.data?.idExterno ?? "0";
                }

                notaFiscalEletronicaIntegracao.SituacaoIntegracao = retWS.SituacaoIntegracao;
                notaFiscalEletronicaIntegracao.ProblemaIntegracao = notaFiscalEletronicaIntegracao.CodigoExternoRetornoIntegracao != null ? "Cancelamento integrado com sucesso!" : retWS.ProblemaIntegracao;
                jsonRequisicao = retWS.jsonRequisicao;
                jsonRetorno = retWS.jsonRetorno;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                notaFiscalEletronicaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

                String message = excecao.Message;
                if (message.Length > 300)
                {
                    message = message.Substring(0, 300);
                }
                notaFiscalEletronicaIntegracao.ProblemaIntegracao = message;
            }

            servicoArquivoTransacao.Adicionar(notaFiscalEletronicaIntegracao, jsonRequisicao, jsonRetorno, "json");

            repositorioNotaFiscalEletronicaIntegracao.Atualizar(notaFiscalEletronicaIntegracao);
        }

        #endregion

        #region Métodos Privados
        private NotaFiscalEletronicaEnvio ConverterNotaFiscalEletronica(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal notaFiscalEletronica, bool cancelamento = false)
        {
            string conteudoXML = ObterNotaFiscalEletronicaXML(notaFiscalEletronica);

            NotaFiscalEletronicaEnvio notaFiscalEletronicaRetorno = new NotaFiscalEletronicaEnvio();

            notaFiscalEletronicaRetorno.InscricaoEmpresa = notaFiscalEletronica.Empresa.CNPJ ?? "";
            notaFiscalEletronicaRetorno.Garagem = notaFiscalEletronica.Empresa?.CodigoIntegracao ?? "0";
            notaFiscalEletronicaRetorno.TipoDocumento = 55;
            notaFiscalEletronicaRetorno.Serie = notaFiscalEletronica.EmpresaSerie.Numero.ToString() ?? "";
            notaFiscalEletronicaRetorno.Conhecimento = notaFiscalEletronica.Numero;
            notaFiscalEletronicaRetorno.Fase = cancelamento == true ? 2 : 1;
            notaFiscalEletronicaRetorno.Sitema = "ESF";
            notaFiscalEletronicaRetorno.Usuario = _configuracaoIntegracao.Usuario ?? "";
            notaFiscalEletronicaRetorno.ChaveDeAcesso = notaFiscalEletronica.Chave ?? "";
            notaFiscalEletronicaRetorno.DataEnvio = notaFiscalEletronica.DataProcessamento?.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") ?? "";
            notaFiscalEletronicaRetorno.ConteudoXML = conteudoXML ?? "";
            notaFiscalEletronicaRetorno.Protocolo = notaFiscalEletronica.Protocolo ?? "";
            notaFiscalEletronicaRetorno.DataProtocolo = Utilidades.XML.ObterConteudoTag(conteudoXML, "dhRecbto");
            notaFiscalEletronicaRetorno.Status = cancelamento == true ? "C" : "A";
            notaFiscalEletronicaRetorno.Recibo = Utilidades.XML.ObterConteudoTag(conteudoXML, "cStat");
            notaFiscalEletronicaRetorno.MensagemRecibo = Utilidades.XML.ObterConteudoTag(conteudoXML, "xMotivo");
            notaFiscalEletronicaRetorno.DataEmissao = notaFiscalEletronica.DataEmissao?.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") ?? "";
            notaFiscalEletronicaRetorno.Versao = notaFiscalEletronica.VersaoNFe.ToString() ?? "";
            notaFiscalEletronicaRetorno.Situacao = notaFiscalEletronica.TipoEmissao.ToString() ?? "";

            return notaFiscalEletronicaRetorno;
        }

        private string ObterNotaFiscalEletronicaXML(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal notaFiscalEletronica)
        {

            if (notaFiscalEletronica.Codigo > 0)
            {
                Repositorio.Embarcador.NotaFiscal.NotaFiscalArquivos repNotaFiscalArquivos = new Repositorio.Embarcador.NotaFiscal.NotaFiscalArquivos(_unitOfWork);
                Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalArquivos arquivo = repNotaFiscalArquivos.BuscarPorNota(notaFiscalEletronica.Codigo);

                if (arquivo != null && !string.IsNullOrWhiteSpace(arquivo.XMLDistribuicao))
                {
                    return arquivo.XMLDistribuicao;
                }
            }

            throw new Exception("Não foi encontrado o arquivo XML da nota selecionada.");
        }
        private void ValidarEnvioIntegracao(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal notaFiscalEletronica, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoIntegracao tipoAcaoIntegracao)
        {
            Repositorio.Embarcador.NotaFiscal.NotaFiscalEletronicaIntegracao repositorioNotaFiscalEletronicaIntegracao = new Repositorio.Embarcador.NotaFiscal.NotaFiscalEletronicaIntegracao(_unitOfWork);
            Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalEletronicaIntegracao notaFiscalEletronicaIntegracao = repositorioNotaFiscalEletronicaIntegracao.BuscarIntegracaoPorCodigoNotaFiscal(notaFiscalEletronica.Codigo);

            if (tipoAcaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoIntegracao.Cancelamento)
            {
                if (notaFiscalEletronicaIntegracao.CodigoExternoRetornoIntegracao == null)
                    throw new Exception(@"Nota fiscal eletrônica não foi integrada com sucesso, portanto não é possível cancelamento");
            }

        }

        #endregion
    }
}
