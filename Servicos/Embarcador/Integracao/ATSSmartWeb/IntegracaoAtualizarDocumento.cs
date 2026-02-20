using DFe.Utils;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb;
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

        public bool IntegrarAtualizarDocumentos(ref Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            string jsonRequisicao = "";
            string jsonRetorno = "";

            bool sucesso = false;

            try
            {
                object request = this.obterAtualizarDocumentos(cargaIntegracao.Carga);
                Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.retornoWebService retWS = transmitir("GestaoSolicitacaoMonitoramentoIntegracao/AtualizarDocumentos", request);

                jsonRequisicao = retWS.jsonRequisicao;
                jsonRetorno = retWS.jsonRetorno;

                if (retWS.SituacaoIntegracao == SituacaoIntegracao.Integrado)
                    sucesso = true;
                else
                    throw new ServicoException(retWS.ProblemaIntegracao);

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

                cargaIntegracao.ProblemaIntegracao = "Erro ao tentar integrar atualização dos documentos com a ATS Smart Web";
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }

            servicoArquivoTransacao.Adicionar(cargaIntegracao, jsonRequisicao, jsonRetorno, "json", "Integração de atualização dos documentos");

            repCargaIntegracao.Atualizar(cargaIntegracao);

            return sucesso;
        }
        #endregion

        #region Métodos Privados


        private Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envAtualizarDocumento obterAtualizarDocumentos(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envAtualizarDocumento retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envAtualizarDocumento();

            retorno.Manifestos = this.obterManifestos(carga);

            return retorno;
        }
        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envManifesto> obterManifestos(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(_unitOfWork);
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envManifesto> retorno = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envManifesto>();
            List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> listaManifestos = repCargaMDFe.BuscarPorCarga(carga.Codigo);

            if (listaManifestos == null || listaManifestos.Count == 0)
                throw new Exception(@"Carga não possuí manifestos");

            foreach (var manifesto in listaManifestos)
            {
                if (manifesto.MDFe.Status != Dominio.Enumeradores.StatusMDFe.Autorizado && manifesto.MDFe.Status != Dominio.Enumeradores.StatusMDFe.Encerrado)
                    continue;

                Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envManifesto retornoManifesto = new Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envManifesto();
                retornoManifesto.CTEs = this.obterCTEs(carga, manifesto.MDFe);
                retornoManifesto.CodigoExterno = carga.Codigo.ToString();
                retornoManifesto.DataEmissao = manifesto.MDFe.DataEmissao?.ToString("yyyy-MM-ddTHH:mm:ssZ") ?? null;

                retorno.Add(retornoManifesto);
            }


            return retorno;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envCTE> obterCTEs(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe)
        {
            Repositorio.DocumentoMunicipioDescarregamentoMDFe repDocumentoMunicipioDescarregamentoMDFe = new Repositorio.DocumentoMunicipioDescarregamentoMDFe(_unitOfWork);
            Repositorio.InformacaoCargaCTE repInformacaoCarga = new Repositorio.InformacaoCargaCTE(_unitOfWork);

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envCTE> retorno = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envCTE>();
            List<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe> listaDocumentos = repDocumentoMunicipioDescarregamentoMDFe.BuscarPorMDFe(mdfe.Codigo);

            if (listaDocumentos == null || listaDocumentos.Count == 0)
                throw new Exception(@"MDFe não possuí CTes");

            foreach (var documento in listaDocumentos)
            {
                if (documento.CTe == null)
                    continue;

                if (documento.CTe.Status != "A" && documento.CTe.Status != "I")
                    continue;

                Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envCTE retornoCTe = new Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envCTE();

                retornoCTe.NumeroCTE = documento.CTe.Numero;
                retornoCTe.SerieCTE = documento.CTe.Serie?.Numero;
                retornoCTe.Quantidade = repInformacaoCarga.ObterQuantidadeUnitaria(documento.CTe.Codigo);
                retornoCTe.Peso = documento.CTe.Peso ;
                retornoCTe.ValorMercadoria = documento.CTe.ValorTotalMercadoria;
                retornoCTe.ValorServico = documento.CTe.ValorPrestacaoServico;
                retornoCTe.DataEmissao = documento.CTe.DataEmissao?.ToString("yyyy-MM-ddTHH:mm:ssZ") ?? null;
                retornoCTe.Status = null;
                retornoCTe.CodigoExterno = documento.CTe.Codigo.ToString();
                retornoCTe.NotasFiscais = this.obterNotasFiscais(carga, documento.CTe);

                retorno.Add(retornoCTe);
            }
            
            return retorno;
        }
        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envNotaFiscal> obterNotasFiscais(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCTe.BuscarPorCargaECTe(carga.Codigo,cte.Codigo);
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envNotaFiscal> retorno = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envNotaFiscal>();
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe> listaCargaPedidoXMLNotaFiscalCTe = repCargaPedidoXMLNotaFiscalCTe.BuscarPorCargaCTe(cargaCTe.Codigo);

            if (listaCargaPedidoXMLNotaFiscalCTe == null || listaCargaPedidoXMLNotaFiscalCTe.Count == 0)
                throw new Exception(@"CTe não possuí notas fiscais");

            foreach (var cargaPedidoXMLNotaFiscalCTe in listaCargaPedidoXMLNotaFiscalCTe)
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envNotaFiscal retornoNotaFiscal = new Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envNotaFiscal();

                int.TryParse(cargaPedidoXMLNotaFiscalCTe.PedidoXMLNotaFiscal.XMLNotaFiscal.Serie, out int serie);

                retornoNotaFiscal.Numero = cargaPedidoXMLNotaFiscalCTe.PedidoXMLNotaFiscal.XMLNotaFiscal.Numero;
                retornoNotaFiscal.Serie = serie;
                retornoNotaFiscal.Valor = cargaPedidoXMLNotaFiscalCTe.PedidoXMLNotaFiscal.XMLNotaFiscal.Valor;
                retornoNotaFiscal.Shipment = null;
                retornoNotaFiscal.NotaFiscalProdutos = this.obterNotaFiscalProdutos(cargaPedidoXMLNotaFiscalCTe.PedidoXMLNotaFiscal.XMLNotaFiscal, cte);

                retorno.Add(retornoNotaFiscal);
            }

            return retorno;
        }
        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envNotaFiscalProduto> obterNotaFiscalProdutos(Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notaFiscal, Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envNotaFiscalProduto> retorno = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envNotaFiscalProduto>();

            envNotaFiscalProduto produto = new envNotaFiscalProduto();
            produto.Produto = this.obterProduto(notaFiscal,cte);

            retorno.Add(produto);

            return retorno;
        }
        private Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envProdutoNF obterProduto(Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notaFiscal, Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envProdutoNF retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envProdutoNF();

            retorno.CodigoExterno = notaFiscal.Codigo.ToString();
            retorno.MetragemCubica = notaFiscal.MetrosCubicos;
            retorno.Nome = notaFiscal.Produto ?? cte.ProdutoPredominante ?? "Diversos";
            retorno.Litros = notaFiscal.Volumes;

            return retorno;
        }

        #endregion
    }
}
