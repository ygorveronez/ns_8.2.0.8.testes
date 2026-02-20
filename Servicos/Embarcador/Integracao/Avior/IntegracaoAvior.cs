using Dominio.Entidades.Embarcador.Cargas;
using Repositorio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Servicos.Embarcador.Integracao.Avior
{
    public class IntegracaoAvior : ServicoBase
    {
        public IntegracaoAvior(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        //PRODUTIVO
        //private string URLCTeAvior = "http://w360.aviorlogistics.com.br/tombini/ws_integracao/atribui_dados_cte_frete";
        //private string usuario = "multicte";
        //private string senha = "8ipZsjMKDJ4KO5kh16onUXz9Up95/c+9tAUKnFWEiwg2";

        //HOMOLOGATIVO
        //private string URLCTeAvior = "http://w360.no-ip.org:8080/ws_integracao/atribui_dados_cte_frete";
        //private string usuario = "multicte_teste";
        //private string senha = "8ipZsjMKDJ4KO5kh16onUXz9Up95/c+9tAUKnFWEiwg2";

        public void EnviarCTesAvior(ref Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao cargaCTeIntegracao, Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoAvon cargaIntegracaoAvon, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete repCargaCTeComponenteFrete = new Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.DocumentosCTE repDocumentoCTe = new DocumentosCTE(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();

            if (string.IsNullOrWhiteSpace(configuracaoIntegracao.URLAvior) || string.IsNullOrWhiteSpace(configuracaoIntegracao.UsuarioAvior) || string.IsNullOrWhiteSpace(configuracaoIntegracao.SenhaAvior))
            {
                cargaCTeIntegracao.ProblemaIntegracao = "Configuração para integração com o Avior inválida.";
                cargaCTeIntegracao.DataIntegracao = DateTime.Now;
                cargaCTeIntegracao.NumeroTentativas++;
                cargaCTeIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                return;
            }

            string urlCTeAvior = configuracaoIntegracao.URLAvior + "ws_integracao/atribui_dados_cte_frete";
            string mensagemErro = "";

            try
            {
                HttpWebRequest requestAvior = (HttpWebRequest)WebRequest.Create(urlCTeAvior);

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = cargaCTeIntegracao.CargaCTe.Carga.Pedidos.FirstOrDefault();
                Dominio.ObjetosDeValor.Embarcador.Integracao.Avior.CTe.Request aviorCte = new Dominio.ObjetosDeValor.Embarcador.Integracao.Avior.CTe.Request();

                aviorCte.login = configuracaoIntegracao.UsuarioAvior;
                aviorCte.senha = configuracaoIntegracao.SenhaAvior;
                aviorCte.Frete = new Dominio.ObjetosDeValor.Embarcador.Integracao.Avior.CTe.Frete();
                aviorCte.Frete.cnpj_filial = cargaCTeIntegracao.CargaCTe.Carga.Filial != null ? cargaCTeIntegracao.CargaCTe.Carga.Filial.CNPJ : cargaCTeIntegracao.CargaCTe.Carga.Empresa.CNPJ_SemFormato;
                aviorCte.Frete.numero_frete = cargaCTeIntegracao.CargaCTe.Carga.CodigoCargaEmbarcador;


                if (cargaIntegracaoAvon != null)
                { //quando é uma integração com Avon, deve-se enviar o número da Minuta e o valor total de apenas 1 CTe.
                    aviorCte.Frete.numero_cte = cargaIntegracaoAvon.NumeroMinuta.ToString();
                    aviorCte.Frete.valor_total_frete = repCargaCTe.BuscarValorFreteLiquidoPorCarga(cargaPedido.Carga.Codigo, "A").ToString("G");
                    aviorCte.Frete.valor_bruto = repCargaCTe.BuscarValorAReceberPorCarga(cargaPedido.Carga.Codigo, "A").ToString("G");
                    aviorCte.Frete.valor_icms = repCargaCTe.BuscarValorICMSPorCarga(cargaPedido.Carga.Codigo, "A").ToString("G");
                    aviorCte.Frete.valor_liquido = aviorCte.Frete.valor_total_frete;
                    aviorCte.Frete.valor_pedagio = repCargaCTeComponenteFrete.BuscarValorComponentePorCarga(cargaPedido.Carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PEDAGIO).ToString("G");
                    aviorCte.Frete.peso_carga = cargaCTeIntegracao.CargaCTe.Carga.Pedidos.Sum(o => o.Peso).ToString("G");
                    aviorCte.Frete.prc_icms = repCargaCTe.BuscarAliquotaICMSPorCarga(cargaPedido.Carga.Codigo, "A").ToString("G");
                    aviorCte.Frete.descricao_tipos_produtos = repCargaCTe.BuscarProdutoPredominantePorCarga(cargaPedido.Carga.Codigo, "A");
                    aviorCte.Frete.valor_mercadoria = repPedidoXMLNotaFiscal.BuscarValorTotalPorCarga(cargaPedido.Carga.Codigo).ToString("G");
                    aviorCte.Frete.valor_descarga = repCargaCTeComponenteFrete.BuscarValorComponentePorCarga(cargaPedido.Carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.DESCARGA).ToString("G");
                    aviorCte.Frete.qtde_volume_transportado = repCargaCTe.BuscarVolumesPorCarga(cargaPedido.Carga.Codigo, "A").ToString("F0");
                }
                else
                {
                    //quando retorna para o avior não é necessário retornar o valor integrar integral e rateado pelo CT-e, basta divividi o valor liquido do frete pelo numero de ctes e retornar igualmente para cada um.
                    //int numeroTotalCTes = repCargaCTe.ContarConhecimentoPorCarga(cargaCTeIntegracao.CargaCTe.Carga.Codigo);
                    aviorCte.Frete.numero_cte = cargaCTeIntegracao.CargaCTe.CTe.Numero.ToString();
                    aviorCte.Frete.valor_total_frete = cargaCTeIntegracao.CargaCTe.CTe.ValorFrete.ToString("G"); //(cargaCTeIntegracao.CargaCTe.Carga.ValorFreteLiquido / numeroTotalCTes).ToString("G");
                    aviorCte.Frete.valor_bruto = cargaCTeIntegracao.CargaCTe.CTe.ValorAReceber.ToString("G");
                    aviorCte.Frete.valor_icms = cargaCTeIntegracao.CargaCTe.CTe.ValorICMS.ToString("G");
                    aviorCte.Frete.valor_liquido = cargaCTeIntegracao.CargaCTe.CTe.ValorFrete.ToString("G");
                    aviorCte.Frete.valor_pedagio = (cargaCTeIntegracao.CargaCTe.Componentes?.Where(o => o.TipoComponenteFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PEDAGIO).Sum(o => o.ValorComponente) ?? 0m).ToString("G");
                    aviorCte.Frete.peso_carga = (cargaCTeIntegracao.CargaCTe.CTe.XMLNotaFiscais?.Sum(o => o.Peso) ?? 0m).ToString("G");
                    aviorCte.Frete.prc_icms = cargaCTeIntegracao.CargaCTe.CTe.AliquotaICMS.ToString("G");
                    aviorCte.Frete.descricao_tipos_produtos = cargaCTeIntegracao.CargaCTe.CTe.ProdutoPredominante;
                    aviorCte.Frete.valor_mercadoria = cargaCTeIntegracao.CargaCTe.CTe.ValorTotalMercadoria.ToString("G");
                    aviorCte.Frete.valor_descarga = (cargaCTeIntegracao.CargaCTe.Componentes?.Where(o => o.TipoComponenteFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.DESCARGA).Sum(o => o.ValorComponente) ?? 0m).ToString("G");
                    aviorCte.Frete.qtde_volume_transportado = cargaCTeIntegracao.CargaCTe.CTe.Volumes.ToString("F0");

                    //aviorCte.Frete.valor_total_frete = cargaCTeIntegracao.CargaCTe.CTe.ValorAReceber.ToString().Replace(",", ".");
                }

                string numeroNFe = repDocumentoCTe.BuscarPrimeiroNumeroPorCTe(cargaCTeIntegracao.CargaCTe.CTe.Codigo);

                if (!string.IsNullOrWhiteSpace(numeroNFe))
                {
                    aviorCte.Frete.Nfe = new Dominio.ObjetosDeValor.Embarcador.Integracao.Avior.CTe.NFe()
                    {
                        numero = numeroNFe
                    };
                }

                Dominio.Entidades.Cliente remetente = cargaPedido.Pedido.Remetente;
                Dominio.Entidades.Cliente expedidor = cargaPedido.Expedidor;
                Dominio.Entidades.Cliente recebedor = cargaPedido.Recebedor;
                Dominio.Entidades.Cliente destinatario = cargaPedido.Pedido.Destinatario;
                Dominio.Entidades.Cliente tomador = cargaPedido.ObterTomador();

                //if (cargaPedido.Expedidor != null)
                //    rementente = cargaPedido.Expedidor;

                //if (cargaPedido.Recebedor != null)
                //    destintario = cargaPedido.Recebedor;

                aviorCte.Frete.Remetente = ObterClienteAvior(remetente);
                aviorCte.Frete.Destinatario = ObterClienteAvior(destinatario);
                aviorCte.Frete.Cliente = ObterClienteAvior(expedidor);
                aviorCte.Frete.Entrega = ObterClienteAvior(recebedor);
                aviorCte.Frete.Tomador = ObterClienteAvior(tomador);

                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                string postData = "<?xml version='1.0' encoding='UTF-8'?>" + Servicos.XML.ConvertObjectToXMLString<Dominio.ObjetosDeValor.Embarcador.Integracao.Avior.CTe.Request>(aviorCte).Replace("\r\n", "");
                var data = Encoding.UTF8.GetBytes(postData);

                requestAvior.Method = "POST";
                requestAvior.AllowAutoRedirect = true;
                requestAvior.ContentType = "text/xml";
                requestAvior.Accept = "application/xml";
                requestAvior.ContentLength = data.Length;

                using (var stream = requestAvior.GetRequestStream())
                    stream.Write(data, 0, data.Length);

                using (var response = (HttpWebResponse)requestAvior.GetResponse())
                {
                    using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
                    {
                        string responseString = streamReader.ReadToEnd();

                        Dominio.ObjetosDeValor.Embarcador.Integracao.Avior.CTe.CteFrete cteFrete = Servicos.XML.ConvertXMLStringToObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Avior.CTe.CteFrete>(responseString);

                        if (cteFrete.sucesso != null)
                        {
                            cargaCTeIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                        }
                        else
                        {
                            cargaCTeIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                            mensagemErro = cteFrete.erro ?? "Erro não retornado pelo Avior";
                        }

                        arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(postData, "xml", unitOfWork);
                        arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(responseString, "xml", unitOfWork);
                        arquivoIntegracao.Data = DateTime.Now;
                        arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
                        arquivoIntegracao.Mensagem = mensagemErro;

                        repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                        cargaCTeIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                mensagemErro = "Falha ao enviar o CT-e " + cargaCTeIntegracao.CargaCTe.CTe.Numero + " para o WS do Avior";
                cargaCTeIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }

            cargaCTeIntegracao.ProblemaIntegracao = mensagemErro;
            cargaCTeIntegracao.DataIntegracao = DateTime.Now;
            cargaCTeIntegracao.NumeroTentativas++;
        }

        public static void CancelarCarga(CargaCancelamentoCargaIntegracao cargaCancelamentoCargaIntegracao, UnitOfWork unitOfWork, string stringConexao)
        {
            Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao repCargaCancelamentoCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();

            if (string.IsNullOrWhiteSpace(configuracaoIntegracao.URLAvior) || string.IsNullOrWhiteSpace(configuracaoIntegracao.UsuarioAvior) || string.IsNullOrWhiteSpace(configuracaoIntegracao.SenhaAvior))
            {
                cargaCancelamentoCargaIntegracao.ProblemaIntegracao = "Configuração para integração com o Avior inválida.";
                cargaCancelamentoCargaIntegracao.DataIntegracao = DateTime.Now;
                cargaCancelamentoCargaIntegracao.NumeroTentativas++;
                cargaCancelamentoCargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                repCargaCancelamentoCargaIntegracao.Atualizar(cargaCancelamentoCargaIntegracao);

                return;
            }

            string mensagem = string.Empty;
            bool retorno = false;
            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = null;

            string cnpjEmpresa = cargaCancelamentoCargaIntegracao.CargaCancelamento.Carga.Empresa.CNPJ;
            string numeroCarga = cargaCancelamentoCargaIntegracao.CargaCancelamento.Carga.CodigoCargaEmbarcador;

            if (cargaCancelamentoCargaIntegracao.CargaCancelamento.DuplicarCarga) //só entra aqui se tiver integração com a Avon, aí envia o cancelamento da minuta ao invés do CT-e
            {
                string numeroCTe = cargaCancelamentoCargaIntegracao.CargaCancelamento.Carga.IntegracoesAvon.FirstOrDefault()?.NumeroMinuta.ToString();

                retorno = EnviarCancelamentoCTe(out arquivoIntegracao, out mensagem, numeroCTe, cnpjEmpresa, numeroCarga, configuracaoIntegracao, unitOfWork);
            }
            else
            {
                retorno = EnviarCancelamentoCarga(out arquivoIntegracao, out mensagem, cnpjEmpresa, numeroCarga, configuracaoIntegracao, unitOfWork);
            }

            if (arquivoIntegracao != null)
                cargaCancelamentoCargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

            cargaCancelamentoCargaIntegracao.ProblemaIntegracao = mensagem;
            cargaCancelamentoCargaIntegracao.DataIntegracao = DateTime.Now;
            cargaCancelamentoCargaIntegracao.NumeroTentativas++;

            if (retorno)
                cargaCancelamentoCargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
            else
                cargaCancelamentoCargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

            repCargaCancelamentoCargaIntegracao.Atualizar(cargaCancelamentoCargaIntegracao);
        }

        public static void CancelarCTe(ref Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracao cargaCancelamentoCargaCTeIntegracao, UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracao repCargaCancelamentoCargaCTeIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracao(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();

            if (string.IsNullOrWhiteSpace(configuracaoIntegracao.URLAvior) || string.IsNullOrWhiteSpace(configuracaoIntegracao.UsuarioAvior) || string.IsNullOrWhiteSpace(configuracaoIntegracao.SenhaAvior))
            {
                cargaCancelamentoCargaCTeIntegracao.ProblemaIntegracao = "Configuração para integração com o Avior inválida.";
                cargaCancelamentoCargaCTeIntegracao.DataIntegracao = DateTime.Now;
                cargaCancelamentoCargaCTeIntegracao.NumeroTentativas++;
                cargaCancelamentoCargaCTeIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                repCargaCancelamentoCargaCTeIntegracao.Atualizar(cargaCancelamentoCargaCTeIntegracao);

                return;
            }

            bool retorno = EnviarCancelamentoCTe(out Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao, out string mensagem, cargaCancelamentoCargaCTeIntegracao.CargaCTe.CTe.Numero.ToString(), cargaCancelamentoCargaCTeIntegracao.CargaCTe.CTe.Empresa.CNPJ, cargaCancelamentoCargaCTeIntegracao.CargaCTe.Carga.CodigoCargaEmbarcador, configuracaoIntegracao, unitOfWork);

            if (arquivoIntegracao != null)
                cargaCancelamentoCargaCTeIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

            cargaCancelamentoCargaCTeIntegracao.ProblemaIntegracao = mensagem;
            cargaCancelamentoCargaCTeIntegracao.DataIntegracao = DateTime.Now;
            cargaCancelamentoCargaCTeIntegracao.NumeroTentativas++;

            if (retorno)
                cargaCancelamentoCargaCTeIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
            else
                cargaCancelamentoCargaCTeIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

            repCargaCancelamentoCargaCTeIntegracao.Atualizar(cargaCancelamentoCargaCTeIntegracao);
        }


        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Avior.CTe.Cliente ObterClienteAvior(Dominio.Entidades.Cliente cliente)
        {
            if (cliente == null)
                return null;

            Dominio.ObjetosDeValor.Embarcador.Integracao.Avior.CTe.Cliente clienteAvior = new Dominio.ObjetosDeValor.Embarcador.Integracao.Avior.CTe.Cliente
            {
                codigo_ibge_cidade = cliente.Localidade.CodigoIBGE.ToString(),
                cpf_cnpj = cliente.CPF_CNPJ_SemFormato,
                descricao_bairro = cliente.Bairro,
                descricao_cidade = cliente.Localidade.Descricao,
                descricao_pais = cliente.Localidade.Pais != null ? cliente.Localidade.Pais.Nome : "Brasil",
                descricao_uf = cliente.Localidade.Estado.Sigla,
                logradouro = cliente.Endereco,
                nome_fantasia = cliente.NomeFantasia,
                numero_endereco = cliente.Numero,
                razao_social = cliente.Nome
            };

            return clienteAvior;
        }

        private static bool EnviarCancelamentoCarga(out CargaCTeIntegracaoArquivo arquivoIntegracao, out string mensagem, string cnpjEmpresa, string numeroCarga, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);

            string urlCTeAvior = configuracaoIntegracao.URLAvior + "ws_integracao/salvar_fretes";
            bool retorno = false;
            string postData = string.Empty;
            string responseString = string.Empty;
            mensagem = string.Empty;
            arquivoIntegracao = null;

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Avior.Cancelamento.Carga.Request aviorCancelamento = new Dominio.ObjetosDeValor.Embarcador.Integracao.Avior.Cancelamento.Carga.Request
                {
                    login = configuracaoIntegracao.UsuarioAvior,
                    senha = configuracaoIntegracao.SenhaAvior,
                    Fretes = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Avior.Cancelamento.Carga.Frete>()
                    {
                        new Dominio.ObjetosDeValor.Embarcador.Integracao.Avior.Cancelamento.Carga.Frete()
                        {
                             acao = "cancelar",
                             cnpj_filial = configuracaoIntegracao.CNPJAvior, // "82809088000232", //cnpjEmpresa,
                             numero_frete = numeroCarga
                        }
                    }
                };

                postData = "<?xml version='1.0' encoding='UTF-8'?>" + Servicos.XML.ConvertObjectToXMLString(aviorCancelamento).Replace("\r\n", "");
                var data = Encoding.UTF8.GetBytes(postData);

                HttpWebRequest requestAvior = (HttpWebRequest)WebRequest.Create(urlCTeAvior);
                requestAvior.Method = "POST";
                requestAvior.AllowAutoRedirect = true;
                requestAvior.ContentType = "text/xml";
                requestAvior.Accept = "application/xml";
                requestAvior.ContentLength = data.Length;

                using (var stream = requestAvior.GetRequestStream())
                    stream.Write(data, 0, data.Length);

                using (var response = (HttpWebResponse)requestAvior.GetResponse())
                {
                    using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
                    {
                        responseString = streamReader.ReadToEnd();

                        Dominio.ObjetosDeValor.Embarcador.Integracao.Avior.Cancelamento.Carga.Response cteFrete = Servicos.XML.ConvertXMLStringToObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Avior.Cancelamento.Carga.Response>(responseString);

                        if (cteFrete != null && cteFrete.Fretes != null && cteFrete.Fretes.Count > 0 && cteFrete.Fretes.All(o => !string.IsNullOrWhiteSpace(o.id_frete_avior)))
                        {
                            retorno = true;
                            mensagem = "Integrado com sucesso.";
                        }
                        else
                        {
                            retorno = false;
                            mensagem = cteFrete?.Erro ?? "Erro não retornado pelo Avior.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                mensagem = "Falha ao enviar o cancelamento da carga " + numeroCarga + " para o WS do Avior";
                retorno = false;
            }

            if (!string.IsNullOrWhiteSpace(postData) || !string.IsNullOrWhiteSpace(responseString))
            {
                arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo
                {
                    ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(postData, "xml", unitOfWork),
                    ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(responseString, "xml", unitOfWork),
                    Data = DateTime.Now,
                    Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento,
                    Mensagem = mensagem
                };

                repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);
            }

            return retorno;
        }

        private static bool EnviarCancelamentoCTe(out Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao, out string mensagem, string numeroCTe, string cnpjEmpresa, string numeroCarga, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);

            string urlCTeAvior = configuracaoIntegracao.URLAvior + "ws_integracao/cancela_ctes_fretes";
            string postData = string.Empty;
            string responseString = string.Empty;
            bool retorno = false;
            mensagem = string.Empty;
            arquivoIntegracao = null;

            try
            {

                Dominio.ObjetosDeValor.Embarcador.Integracao.Avior.Cancelamento.CTe.Request aviorCancelamento = new Dominio.ObjetosDeValor.Embarcador.Integracao.Avior.Cancelamento.CTe.Request
                {
                    login = configuracaoIntegracao.UsuarioAvior,
                    senha = configuracaoIntegracao.SenhaAvior,
                    Ctes = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Avior.Cancelamento.CTe.Cte>()
                    {
                        new Dominio.ObjetosDeValor.Embarcador.Integracao.Avior.Cancelamento.CTe.Cte()
                        {
                            numero_cte = numeroCTe,
                            cnpj_unidade_empresa = configuracaoIntegracao.CNPJAvior, // "82809088000232",//cnpjEmpresa,
                            numero_frete = numeroCarga
                        }
                    }
                };

                postData = "<?xml version='1.0' encoding='UTF-8'?>" + Servicos.XML.ConvertObjectToXMLString(aviorCancelamento).Replace("\r\n", "");
                var data = Encoding.UTF8.GetBytes(postData);

                HttpWebRequest requestAvior = (HttpWebRequest)WebRequest.Create(urlCTeAvior);
                requestAvior.Method = "POST";
                requestAvior.AllowAutoRedirect = true;
                requestAvior.ContentType = "text/xml";
                requestAvior.Accept = "application/xml";
                requestAvior.ContentLength = data.Length;

                using (var stream = requestAvior.GetRequestStream())
                    stream.Write(data, 0, data.Length);

                using (var response = (HttpWebResponse)requestAvior.GetResponse())
                {
                    using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
                    {
                        responseString = streamReader.ReadToEnd();

                        Dominio.ObjetosDeValor.Embarcador.Integracao.Avior.Cancelamento.CTe.Response cteFrete = Servicos.XML.ConvertXMLStringToObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Avior.Cancelamento.CTe.Response>(responseString);

                        if (cteFrete.CTes != null && cteFrete.CTes != null && cteFrete.CTes.Count > 0 && cteFrete.CTes.All(p => p.status == true))
                        {
                            retorno = true;
                            mensagem = "Integrado com sucesso.";
                        }
                        else
                        {
                            retorno = false;

                            if (cteFrete.CTes != null && cteFrete.CTes.Count > 0)
                                mensagem = cteFrete.CTes[0].erro;
                            else
                                mensagem = cteFrete?.Erro;

                            if (string.IsNullOrWhiteSpace(mensagem))
                                mensagem = "Erro não retornado pelo Avior.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                mensagem = "Falha ao enviar o cancelamento do CT-e " + numeroCTe + " para o WS do Avior";
                retorno = false;
            }

            if (!string.IsNullOrWhiteSpace(postData) || !string.IsNullOrWhiteSpace(responseString))
            {
                arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo
                {
                    ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(postData, "xml", unitOfWork),
                    ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(responseString, "xml", unitOfWork),
                    Data = DateTime.Now,
                    Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento,
                    Mensagem = mensagem
                };

                repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);
            }

            return retorno;
        }

        #endregion

    }
}
