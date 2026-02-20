using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Integracao.Globus;
using Dominio.ObjetosDeValor.Embarcador.Integracao.Globus.TituloFinanceiro;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Integracao.Globus
{
    public partial class IntegracaoGlobus
    {
        public void IntegrarTituloFinanceiro(Dominio.Entidades.Embarcador.Financeiro.TituloIntegracao tituloIntegracao)
        {
            Repositorio.Embarcador.Financeiro.TituloIntegracao repositorioTituloIntegracao = new Repositorio.Embarcador.Financeiro.TituloIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            string jsonRequisicao = "";
            string jsonRetorno = "";

            try
            {
                tituloIntegracao.DataIntegracao = DateTime.Now;
                tituloIntegracao.NumeroTentativas++;

                if (_configuracaoIntegracao.ShortCodeFinanceiro == 0 || String.IsNullOrEmpty(_configuracaoIntegracao.TokenFinanceiro) || String.IsNullOrEmpty(_configuracaoIntegracao.URLWebServiceFinanceiro))
                    throw new Exception("Processo abortado, configuração não encontrada ou incompleta!");

                var request = ConverterTitulo(tituloIntegracao.Titulo);

                var retWS = this.Transmitir(request, string.Concat("api/ContasPagar/Documento?shortCode=", _configuracaoIntegracao.ShortCodeFinanceiro, "&token=", _configuracaoIntegracao.TokenFinanceiro), null, _configuracaoIntegracao.URLWebServiceFinanceiro);

                if (retWS != null && retWS.SituacaoIntegracao == SituacaoIntegracao.Integrado && !String.IsNullOrEmpty(retWS.jsonRetorno))
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Globus.retornoWebServiceSuccess retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Globus.retornoWebServiceSuccess>(retWS.jsonRetorno);

                    tituloIntegracao.CodigoExternoRetornoIntegracao = retorno?.data?.idExterno ?? "0";
                }

                tituloIntegracao.SituacaoIntegracao = retWS.SituacaoIntegracao;
                tituloIntegracao.ProblemaIntegracao = retWS.ProblemaIntegracao;
                jsonRequisicao = retWS.jsonRequisicao;
                jsonRetorno = retWS.jsonRetorno;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                tituloIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

                String message = excecao.Message;
                if (message.Length > 300)
                {
                    message = message.Substring(0, 300);
                }
                tituloIntegracao.ProblemaIntegracao = message;
            }

            servicoArquivoTransacao.Adicionar(tituloIntegracao, jsonRequisicao, jsonRetorno, "json");

            repositorioTituloIntegracao.Atualizar(tituloIntegracao);
        }

        public void IntegrarTituloFinanceiroAlteracao(Dominio.Entidades.Embarcador.Financeiro.TituloIntegracao tituloIntegracao)
        {
            Repositorio.Embarcador.Financeiro.TituloIntegracao repositorioTituloIntegracao = new Repositorio.Embarcador.Financeiro.TituloIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            string jsonRequisicao = "";
            string jsonRetorno = "";

            try
            {
                tituloIntegracao.DataIntegracao = DateTime.Now;
                tituloIntegracao.NumeroTentativas++;

                if (_configuracaoIntegracao.ShortCodeFinanceiro == 0 || String.IsNullOrEmpty(_configuracaoIntegracao.TokenFinanceiro) || String.IsNullOrEmpty(_configuracaoIntegracao.URLWebServiceFinanceiro))
                    throw new Exception("Processo abortado, configuração não encontrada ou incompleta!");

                this.ValidarEnvioIntegracao(tituloIntegracao.Titulo, tituloIntegracao.TipoAcaoIntegracao);

                string codigoExternoCriacao = repositorioTituloIntegracao.BuscarPorTituloETipoIntegracaoEAcao(tituloIntegracao.Titulo.Codigo, TipoIntegracao.Globus, TipoAcaoIntegracao.Criacao)?.CodigoExternoRetornoIntegracao ?? null;

                var request = ConverterTituloAlteracao(tituloIntegracao.Titulo, codigoExternoCriacao);

                var retWS = this.Transmitir(request, string.Concat("api/ContasPagar/Documento?shortCode=", _configuracaoIntegracao.ShortCodeFinanceiro, "&token=", _configuracaoIntegracao.TokenFinanceiro), null, _configuracaoIntegracao.URLWebServiceFinanceiro, enumTipoWS.PATCH);

                if (retWS != null && retWS.SituacaoIntegracao == SituacaoIntegracao.Integrado && !String.IsNullOrEmpty(retWS.jsonRetorno))
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Globus.retornoWebServiceSuccess retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Globus.retornoWebServiceSuccess>(retWS.jsonRetorno);

                    tituloIntegracao.CodigoExternoRetornoIntegracao = retorno?.data?.idExterno ?? "0";
                }

                tituloIntegracao.SituacaoIntegracao = retWS.SituacaoIntegracao;
                tituloIntegracao.ProblemaIntegracao = retWS.ProblemaIntegracao;
                jsonRequisicao = retWS.jsonRequisicao;
                jsonRetorno = retWS.jsonRetorno;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                tituloIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

                String message = excecao.Message;
                if (message.Length > 300)
                {
                    message = message.Substring(0, 300);
                }
                tituloIntegracao.ProblemaIntegracao = message;
            }

            servicoArquivoTransacao.Adicionar(tituloIntegracao, jsonRequisicao, jsonRetorno, "json");

            repositorioTituloIntegracao.Atualizar(tituloIntegracao);
        }

        public void IntegrarTituloFinanceiroCancelamento(Dominio.Entidades.Embarcador.Financeiro.TituloIntegracao tituloIntegracao)
        {
            Repositorio.Embarcador.Financeiro.TituloIntegracao repositorioTituloIntegracao = new Repositorio.Embarcador.Financeiro.TituloIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            string jsonRequisicao = "";
            string jsonRetorno = "";

            try
            {
                tituloIntegracao.DataIntegracao = DateTime.Now;
                tituloIntegracao.NumeroTentativas++;

                if (_configuracaoIntegracao.ShortCodeFinanceiro == 0 || String.IsNullOrEmpty(_configuracaoIntegracao.TokenFinanceiro) || String.IsNullOrEmpty(_configuracaoIntegracao.URLWebServiceFinanceiro))
                    throw new Exception("Processo abortado, configuração não encontrada ou incompleta!");

                this.ValidarEnvioIntegracao(tituloIntegracao.Titulo, tituloIntegracao.TipoAcaoIntegracao);

                string codigoExternoCriacao = repositorioTituloIntegracao.BuscarPorTituloETipoIntegracaoEAcao(tituloIntegracao.Titulo.Codigo,TipoIntegracao.Globus, TipoAcaoIntegracao.Criacao)?.CodigoExternoRetornoIntegracao ?? null;

                var request = ConverterTituloCancelamento(codigoExternoCriacao);

                var retWS = this.Transmitir(request, string.Concat("api/ContasPagar/Documento?shortCode=", _configuracaoIntegracao.ShortCodeFinanceiro, "&token=", _configuracaoIntegracao.TokenFinanceiro), null, _configuracaoIntegracao.URLWebServiceFinanceiro, enumTipoWS.DELETE);

                if (retWS != null && retWS.SituacaoIntegracao == SituacaoIntegracao.Integrado && !String.IsNullOrEmpty(retWS.jsonRetorno))
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Globus.retornoWebServiceSuccess retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Globus.retornoWebServiceSuccess>(retWS.jsonRetorno);

                    tituloIntegracao.CodigoExternoRetornoIntegracao = retorno?.data?.idExterno ?? "0";
                }

                tituloIntegracao.SituacaoIntegracao = retWS.SituacaoIntegracao;
                tituloIntegracao.ProblemaIntegracao = retWS.ProblemaIntegracao;
                jsonRequisicao = retWS.jsonRequisicao;
                jsonRetorno = retWS.jsonRetorno;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                tituloIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

                String message = excecao.Message;
                if (message.Length > 300)
                {
                    message = message.Substring(0, 300);
                }
                tituloIntegracao.ProblemaIntegracao = message;
            }

            servicoArquivoTransacao.Adicionar(tituloIntegracao, jsonRequisicao, jsonRetorno, "json");

            repositorioTituloIntegracao.Atualizar(tituloIntegracao);
        }

        private TituloFinanceiroEnvio ConverterTitulo(Dominio.Entidades.Embarcador.Financeiro.Titulo titulo)
        {
            Repositorio.Embarcador.Financeiro.FormasTitulo repFormasTitulos = new Repositorio.Embarcador.Financeiro.FormasTitulo(_unitOfWork);

            TituloFinanceiroEnvio tituloRetorno = new TituloFinanceiroEnvio();
            tituloRetorno.InscricaoEmpresa = titulo.Empresa?.CNPJ ?? "";
            tituloRetorno.TipoDocumento = repFormasTitulos.BuscarPorDescricao(titulo.FormaTitulo.ObterDescricao())?.CodigoIntegracao.ToString() ?? "";
            tituloRetorno.Emissao = titulo.DataEmissao?.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") ?? "";
            tituloRetorno.NumeroParcela = titulo.Sequencia;
            tituloRetorno.Serie = "0";
            tituloRetorno.Vencimento = titulo.DataVencimento?.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") ?? "";
            tituloRetorno.Sistema = _configuracaoIntegracao.SistemaIntegrarComContasPagar;
            tituloRetorno.Usuario = _configuracaoIntegracao.Usuario;
            tituloRetorno.Entrada = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            tituloRetorno.InscricaoFornecedor = titulo.Fornecedor?.CPF_CNPJ_SemFormato ?? "";
            tituloRetorno.NumeroDocumento = titulo.Codigo.ToString();
            tituloRetorno.Acrescimo = titulo.Acrescimo;
            tituloRetorno.Desconto = titulo.Desconto;
            tituloRetorno.Observacao = titulo.Observacao ?? "";
            tituloRetorno.IntegrarContabilidade = true;
            tituloRetorno.Despesas = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Globus.TituloFinanceiro.DespesaTitulo> {
                new Dominio.ObjetosDeValor.Embarcador.Integracao.Globus.TituloFinanceiro.DespesaTitulo
                {
                    Despesa = titulo.TipoMovimento?.CodigoIntegracao ?? "",
                    Valor = titulo.ValorOriginal,
                    Observacao = titulo.Observacao,
                    ContaContabil = 641, //provisorio
                    PlanoContabil = 1 //provisorio
                } 
            };

            return tituloRetorno;
        }

        private Hashtable ConverterTituloCancelamento(string codigoDocumentoIntegracao)
        {
            Hashtable tituloRetorno = new Hashtable();

            int codigoIntegracao = 0;
            int.TryParse(codigoDocumentoIntegracao, out codigoIntegracao);

            if (codigoIntegracao == 0)
                throw new Exception("Código do documento não está preenchido");

            tituloRetorno.Add("CodigoDocumento", codigoDocumentoIntegracao);
            tituloRetorno.Add("Usuario", _configuracaoIntegracao.Usuario);

            return tituloRetorno;
        }

        private Hashtable ConverterTituloAlteracao(Dominio.Entidades.Embarcador.Financeiro.Titulo titulo, string codigoDocumentoIntegracao)
        {
            Hashtable tituloRetorno = new Hashtable();

            int codigoIntegracao = 0;
            int.TryParse(codigoDocumentoIntegracao, out codigoIntegracao);

            if (codigoIntegracao == 0)
                throw new Exception("Código do documento não está preenchido");

            tituloRetorno.Add("CodigoDocumento", codigoDocumentoIntegracao);
            tituloRetorno.Add("Vencimento", titulo.DataVencimento?.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") ?? "");
            tituloRetorno.Add("Observacao", titulo.Observacao ?? "");
            tituloRetorno.Add("Usuario", _configuracaoIntegracao.Usuario);

            return tituloRetorno;
        }

        private void ValidarEnvioIntegracao(Dominio.Entidades.Embarcador.Financeiro.Titulo titulo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoIntegracao tipoAcaoIntegracao)
        {
            Repositorio.Embarcador.Financeiro.TituloIntegracao repositorioTituloIntegracao = new Repositorio.Embarcador.Financeiro.TituloIntegracao(_unitOfWork);

            if (tipoAcaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoIntegracao.Cancelamento || tipoAcaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoIntegracao.Alteracao)
            {
                if ((repositorioTituloIntegracao.BuscarPorTituloETipo(titulo.Codigo, TipoIntegracao.Globus, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoIntegracao.Criacao)?.SituacaoIntegracao ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao)
                    != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado)
                    throw new Exception(@"Criação do titulo não foi integrado com sucesso, apenas após isso para integrar alteração e cancelamento");
            }
        }
    }
}
