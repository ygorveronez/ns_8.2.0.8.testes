using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Importacao;
using Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.Titulo;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Integracao.KMM
{
    public partial class IntegracaoKMM
    {
        #region Método publicos

        public void IntegrarTituloFinanceiro(Dominio.Entidades.Embarcador.Financeiro.TituloIntegracao tituloIntegracao)
        {
            Repositorio.Embarcador.Financeiro.TituloIntegracao repositorioTituloIntegracao = new Repositorio.Embarcador.Financeiro.TituloIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoKMM repositorioIntegracaoKMM = new Repositorio.Embarcador.Configuracoes.IntegracaoKMM(_unitOfWork);

            var configuracaoIntegracaoKMM = repositorioIntegracaoKMM.Buscar();

            string jsonRequisicao = "";
            string jsonRetorno = "";

            try
            {
                tituloIntegracao.DataIntegracao = DateTime.Now;
                tituloIntegracao.NumeroTentativas++;

                Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.Titulo.Titulo objetoTitulo = ObterTitulo(tituloIntegracao.Titulo, "INSERT");

                if (objetoTitulo != null)
                {
                    Hashtable request = new Hashtable
                    {
                        { "module", "M1076" },
                        { "operation", "manipulaFatura" },
                        { "parameters", objetoTitulo }
                    };

                    var retWS = this.Transmitir(configuracaoIntegracaoKMM, request);

                    tituloIntegracao.SituacaoIntegracao = retWS.SituacaoIntegracao;
                    tituloIntegracao.ProblemaIntegracao = retWS.ProblemaIntegracao;

                    jsonRequisicao = retWS.jsonRequisicao;
                    jsonRetorno = retWS.jsonRetorno;
                }
                else
                {
                    tituloIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    tituloIntegracao.ProblemaIntegracao = "Não foi possivel enviar integração,titulo não possui documento para envio ! ";
                }
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
            Repositorio.Embarcador.Configuracoes.IntegracaoKMM repositorioIntegracaoKMM = new Repositorio.Embarcador.Configuracoes.IntegracaoKMM(_unitOfWork);

            var configuracaoIntegracaoKMM = repositorioIntegracaoKMM.Buscar();

            string jsonRequisicao = "";
            string jsonRetorno = "";

            try
            {
                tituloIntegracao.DataIntegracao = DateTime.Now;
                tituloIntegracao.NumeroTentativas++;

                Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.Titulo.Titulo objetoTitulo = ObterTitulo(tituloIntegracao.Titulo, "DELETE");

                Hashtable request = new Hashtable
                {
                    { "module", "M1076" },
                    { "operation", "manipulaFatura" },
                    { "parameters", objetoTitulo }
                };

                var retWS = this.Transmitir(configuracaoIntegracaoKMM, request);

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
        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.Titulo.Titulo ObterTitulo(Dominio.Entidades.Embarcador.Financeiro.Titulo titulo, string operation)
        {
            var cte = titulo.Documentos != null ? titulo.Documentos.FirstOrDefault().CTe : null;

            if (cte == null)
                return null;

            string contaCorrenteFavorecido = null;
            string tipoDocumentoFiscal = "";

            if (titulo.GrupoPessoas.UtilizarCadastroContaBancaria)
                contaCorrenteFavorecido = titulo.GrupoPessoas.ContasBancarias.Where(x => x.ClientePortadorConta.Codigo == titulo.Pessoa.Codigo)?.FirstOrDefault()?.NumeroConta ?? null;
            else if (titulo.Pessoa.UtilizarCadastroContaBancaria)
                contaCorrenteFavorecido = titulo.Pessoa.ContasBancarias.Where(x => x.ClientePortadorConta.Codigo == titulo.Pessoa.Codigo)?.FirstOrDefault()?.NumeroConta ?? null;


            if (cte.ModeloDocumentoFiscal.Abreviacao.Equals("CT-e"))
                tipoDocumentoFiscal = "CTE";
            else if (cte.ModeloDocumentoFiscal.Abreviacao.Equals("NFS-e"))
                tipoDocumentoFiscal = "NFSE";
            else
                tipoDocumentoFiscal = cte?.ModeloDocumentoFiscal?.Abreviacao ?? "";

            var documentosFiscais = new List<DocumentoFiscal>();

            foreach (var doc in titulo.Documentos)
            {
                documentosFiscais.Add(new DocumentoFiscal
                {
                    IdDocumento = null,
                    Numero = doc.CTe.Numero.ToString() ?? "",
                    Serie = doc.CTe.Serie?.Numero.ToString() ?? ""
                });
            }

            var retorno = new Titulo
            {
                Operation = operation,
                TipoDocumentoFiscal = tipoDocumentoFiscal,
                TipoDocumentoFinanceiro = "CT",
                NumeroFatura = titulo.NumeroFatura.ToString() ?? "",
                IdExterno = titulo.Codigo,
                Organograma = cte?.CentroDeCustoViagem?.CodigoIntegracao ?? "",
                CCFavorecido = contaCorrenteFavorecido,
                DocumentosFiscais = documentosFiscais,
                Parcelas = new List<Parcela>
                {
                    new Parcela
                    {
                        NumeroParcela = titulo.FaturaParcela?.Sequencia ?? 0,
                        DataEmissao = titulo.FaturaParcela?.DataEmissao.ToString("yyyy-MM-dd HH:mm:ss") ?? null,
                        DataVencimento = titulo.FaturaParcela?.DataVencimento.ToString("yyyy-MM-dd HH:mm:ss") ?? null,
                        ValorMoeda = titulo.FaturaParcela?.ValorTotalMoeda ?? 0,
                        ValorReal = titulo.FaturaParcela?.Valor ?? 0,
                        Comentario = titulo.Observacao ?? "",
                        Rateio = null,
                        Boleto = titulo.FormaTitulo == FormaTitulo.Boleto ? new Boleto
                        {
                            CodigoBarras = titulo.LinhaDigitavelBoleto ?? "",
                            NossoNumero = titulo.NossoNumero ?? ""
                        } : null
                    }
                }
            };


            return retorno;
        }

        #endregion


    }
}
