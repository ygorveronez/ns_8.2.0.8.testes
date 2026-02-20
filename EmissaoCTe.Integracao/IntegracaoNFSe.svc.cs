using System;
using System.Collections.Generic;
using System.Linq;

namespace EmissaoCTe.Integracao
{
    public class IntegracaoNFSe : IIntegracaoNFSe
    {
        #region Métodos Globais

        public Retorno<RetornoNFSe> BuscarPorCodigoNFSe(int codigoNFSe, Dominio.Enumeradores.TipoIntegracaoNFSe tipoIntegracao, Dominio.Enumeradores.TipoRetornoIntegracao tipoRetorno, string token)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.IntegracaoNFSe repIntegracao = new Repositorio.IntegracaoNFSe(unidadeDeTrabalho);
                List<Dominio.Entidades.IntegracaoNFSe> listaIntegracoes = repIntegracao.BuscarPorNFSeETipo(codigoNFSe, tipoIntegracao);

                Retorno<RetornoNFSe> retorno = new Retorno<RetornoNFSe> { Mensagem = "Retorno realizado com sucesso.", Status = true };

                if (listaIntegracoes.Count() > 0)
                {
                    if (listaIntegracoes[0].NFSe.Empresa.EmpresaPai.Configuracao != null && listaIntegracoes[0].NFSe.Empresa.EmpresaPai.Configuracao.TokenIntegracaoCTe != token)
                        return new Retorno<RetornoNFSe>() { Mensagem = "Token de acesso inválido.", Status = false };

                    RetornoNFSe retornoNFSe = new RetornoNFSe()
                    {
                        CodigoNFSe = listaIntegracoes[0].NFSe.Codigo,
                        CodigoVerificacao = listaIntegracoes[0].NFSe.CodigoVerificacao,
                        CNPJEmitente = listaIntegracoes[0].NFSe.Empresa.CNPJ,
                        Tipo = listaIntegracoes[0].Tipo,
                        NomeArquivo = listaIntegracoes[0].NomeArquivo,
                        Arquivo = listaIntegracoes[0].NFSe.Status == Dominio.Enumeradores.StatusNFSe.Rejeicao ? listaIntegracoes[0].Arquivo : string.Empty,
                        StatusNFSe = listaIntegracoes[0].NFSe.Status,
                        DataEmissao = listaIntegracoes[0].NFSe.DataEmissao.ToString("dd/MM/yyyy HH:mm:ss"),
                        NumeroNFSe = listaIntegracoes[0].NFSe.Numero,
                        NumeroNFSePrefeitura = !string.IsNullOrWhiteSpace(listaIntegracoes[0].NFSe.NumeroPrefeitura) ? listaIntegracoes[0].NFSe.NumeroPrefeitura : listaIntegracoes[0].NFSe.Numero.ToString(),
                        SerieNFSe = listaIntegracoes[0].NFSe.Serie.Numero,
                        MensagemRetorno = listaIntegracoes[0].NFSe.RPS != null ? string.Concat(listaIntegracoes[0].NFSe.RPS.CodigoRetorno, " - ", listaIntegracoes[0].NFSe.RPS.MensagemRetorno) : string.Empty,
                        NumeroProtocolo = listaIntegracoes[0].NFSe.RPS != null ? listaIntegracoes[0].NFSe.RPS.Protocolo : string.Empty,
                        JustificativaCancelamento = listaIntegracoes[0].NFSe.Status == Dominio.Enumeradores.StatusNFSe.Cancelado ? listaIntegracoes[0].NFSe.JustificativaCancelamento : string.Empty,
                        ValoresNFSe = new Dominio.ObjetosDeValor.NFSe.ValoresNFSe()
                        {
                            BaseCalculoISS = Math.Round(listaIntegracoes[0].NFSe.BaseCalculoISS, 2, MidpointRounding.ToEven), 
                            AliquotaISS = listaIntegracoes[0].NFSe.AliquotaISS,
                            ValorISS = Math.Round(listaIntegracoes[0].NFSe.ValorISS, 2, MidpointRounding.ToEven),
                            ValorNFSe = Math.Round(listaIntegracoes[0].NFSe.ValorServicos, 2, MidpointRounding.ToEven)
                        }
                    };

                    this.ObterArquivosRetornoIntegracaoNFSe(ref retornoNFSe, listaIntegracoes[0], tipoRetorno, unidadeDeTrabalho);

                    retorno.Objeto = retornoNFSe;
                }
                else
                {
                    Repositorio.NFSe repNFSe = new Repositorio.NFSe(unidadeDeTrabalho);
                    Dominio.Entidades.NFSe nfse = repNFSe.BuscarPorCodigo(codigoNFSe);

                    if (nfse != null)
                    {
                        if (nfse.Empresa.EmpresaPai.Configuracao != null && nfse.Empresa.EmpresaPai.Configuracao.TokenIntegracaoCTe != token)
                            return new Retorno<RetornoNFSe>() { Mensagem = "Token de acesso inválido.", Status = false };


                        RetornoNFSe retornoNFSe = new RetornoNFSe()
                        {
                            CodigoNFSe = nfse.Codigo,
                            CodigoVerificacao = nfse.CodigoVerificacao,
                            CNPJEmitente = nfse.Empresa.CNPJ,
                            Tipo = nfse.Status == Dominio.Enumeradores.StatusNFSe.Cancelado ? Dominio.Enumeradores.TipoIntegracaoNFSe.Cancelamento : Dominio.Enumeradores.TipoIntegracaoNFSe.Emissao,
                            NomeArquivo = string.Empty,
                            Arquivo = string.Empty,
                            StatusNFSe = nfse.Status,
                            DataEmissao = nfse.DataEmissao.ToString("dd/MM/yyyy HH:mm:ss"),
                            NumeroNFSe = nfse.Numero,
                            NumeroNFSePrefeitura = !string.IsNullOrWhiteSpace(nfse.NumeroPrefeitura) ? nfse.NumeroPrefeitura : nfse.Numero.ToString(),
                            SerieNFSe = nfse.Numero,
                            MensagemRetorno = nfse.RPS != null ? string.Concat(nfse.RPS.CodigoRetorno, " - ", nfse.RPS.MensagemRetorno) : string.Empty,
                            NumeroProtocolo = nfse.RPS != null ? nfse.RPS.Protocolo : string.Empty,
                            JustificativaCancelamento = nfse.Status == Dominio.Enumeradores.StatusNFSe.Cancelado ? nfse.JustificativaCancelamento : string.Empty
                        };

                        this.ObterArquivosRetornoNFSe(ref retornoNFSe, nfse, tipoRetorno, tipoIntegracao, unidadeDeTrabalho);

                        retorno.Objeto = retornoNFSe;
                    }
                }


                return retorno;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new Retorno<RetornoNFSe>() { Mensagem = "Ocorreu uma falha ao obter os dados das integrações.", Status = false };
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void ObterArquivosRetornoIntegracaoNFSe(ref RetornoNFSe retorno, Dominio.Entidades.IntegracaoNFSe integracao, Dominio.Enumeradores.TipoRetornoIntegracao tipoRetorno, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Servicos.NFSe servicoNFSe = new Servicos.NFSe(unidadeDeTrabalho);

            if (tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.Todos || tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.XML || tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.XML_PDF)
            {
                if (integracao.NFSe.Status == Dominio.Enumeradores.StatusNFSe.Autorizado && integracao.Tipo == Dominio.Enumeradores.TipoIntegracaoNFSe.Emissao)
                {
                    retorno.XML = servicoNFSe.ObterXMLString(integracao.NFSe.Codigo, Dominio.Enumeradores.TipoXMLNFSe.Autorizacao, unidadeDeTrabalho);
                }
                else if (integracao.NFSe.Status == Dominio.Enumeradores.StatusNFSe.Cancelado && integracao.Tipo == Dominio.Enumeradores.TipoIntegracaoNFSe.Cancelamento)
                {
                    retorno.XML = servicoNFSe.ObterXMLString(integracao.NFSe.Codigo, Dominio.Enumeradores.TipoXMLNFSe.Cancelamento);
                }
            }

            if (tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.Todos || tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.PDF || tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.XML_PDF)
            {
                if (integracao.NFSe.Status == Dominio.Enumeradores.StatusNFSe.Autorizado && integracao.Tipo == Dominio.Enumeradores.TipoIntegracaoNFSe.Emissao)
                {
                    retorno.PDF = servicoNFSe.ObterDANFSEString(integracao.NFSe.Codigo, unidadeDeTrabalho);
                }
            }
        }

        private void ObterArquivosRetornoNFSe(ref RetornoNFSe retorno, Dominio.Entidades.NFSe nfse, Dominio.Enumeradores.TipoRetornoIntegracao tipoRetorno, Dominio.Enumeradores.TipoIntegracaoNFSe tipoIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Servicos.NFSe servicoNFSe = new Servicos.NFSe(unidadeDeTrabalho);

            if (tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.Todos || tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.XML || tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.XML_PDF)
            {
                if (nfse.Status == Dominio.Enumeradores.StatusNFSe.Autorizado && tipoIntegracao == Dominio.Enumeradores.TipoIntegracaoNFSe.Emissao)
                {
                    retorno.XML = servicoNFSe.ObterXMLString(nfse.Codigo, Dominio.Enumeradores.TipoXMLNFSe.Autorizacao, unidadeDeTrabalho);
                }
                else if (nfse.Status == Dominio.Enumeradores.StatusNFSe.Cancelado && tipoIntegracao == Dominio.Enumeradores.TipoIntegracaoNFSe.Cancelamento)
                {
                    retorno.XML = servicoNFSe.ObterXMLString(nfse.Codigo, Dominio.Enumeradores.TipoXMLNFSe.Cancelamento);
                }
            }

            if (tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.Todos || tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.PDF || tipoRetorno == Dominio.Enumeradores.TipoRetornoIntegracao.XML_PDF)
            {
                if (nfse.Status == Dominio.Enumeradores.StatusNFSe.Autorizado && tipoIntegracao == Dominio.Enumeradores.TipoIntegracaoNFSe.Emissao)
                {
                    retorno.PDF = servicoNFSe.ObterDANFSEString(nfse.Codigo, unidadeDeTrabalho);
                }
            }
        }

        #endregion
    }
}
