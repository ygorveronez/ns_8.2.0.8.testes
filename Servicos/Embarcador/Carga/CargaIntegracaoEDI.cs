using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Carga
{
    public class CargaIntegracaoEDI
    {
        #region Métodos Públicos

        public static void VerificarCargaIntegracaoEDINotifis(Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoLayoutEDI layoutEDITipoOperacao = null;
            if (carga.TipoOperacao != null)
            {

                if (carga.TipoOperacao.LayoutsEDI != null)
                    layoutEDITipoOperacao = (from obj in carga.TipoOperacao.LayoutsEDI where (obj.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.NOTFIS || obj.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.NOTFIS_NOVA_IMPORTACAO) && obj.LayoutEDI.Status == "A" select obj).FirstOrDefault();

                if (layoutEDITipoOperacao != null)
                    //possui, devemos gerar um registro na integracao EDI, 
                    AdicionarCargaIntegracaoEDI(carga, unitOfWork, layoutEDITipoOperacao);
                else
                    // nao possui, vamos deletar caso o usuario tenha trocado o tipo Operacao e antes tinha...
                    ExcluirCargaIntegracaoEDI(carga, unitOfWork);

            }

            // Gera a integração NOTIFIS dos EDIs do transportador quando a carga é criada fora do ME
            Dominio.Entidades.Embarcador.Transportadores.TransportadorLayoutEDI layoutEDITransportador = null;
            if (carga.Empresa?.EmissaoDocumentosForaDoSistema == true && carga.Empresa?.TransportadorLayoutsEDI != null)
            {
                layoutEDITransportador = (from obj in carga.Empresa.TransportadorLayoutsEDI
                                          where (obj.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.NOTFIS || obj.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.NOTFIS_NOVA_IMPORTACAO) && obj.LayoutEDI.Status == "A"
                                          select obj).FirstOrDefault();

                if (layoutEDITransportador != null && layoutEDITransportador.LayoutEDI.Codigo != layoutEDITipoOperacao?.LayoutEDI.Codigo)
                {
                    AdicionarCargaIntegracaoEDIComTransportador(carga, carga.Empresa, unitOfWork, layoutEDITransportador);
                }
            }
        }

        public async static Task VerificarCargaIntegracaoEDINotifisAsync(Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoLayoutEDI layoutEDITipoOperacao = null;
            if (carga.TipoOperacao != null)
            {

                if (carga.TipoOperacao.LayoutsEDI != null)
                    layoutEDITipoOperacao = (from obj in carga.TipoOperacao.LayoutsEDI where (obj.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.NOTFIS || obj.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.NOTFIS_NOVA_IMPORTACAO) && obj.LayoutEDI.Status == "A" select obj).FirstOrDefault();

                if (layoutEDITipoOperacao != null)
                    AdicionarCargaIntegracaoEDI(carga, unitOfWork, layoutEDITipoOperacao);
                else
                    ExcluirCargaIntegracaoEDI(carga, unitOfWork);

            }

            Dominio.Entidades.Embarcador.Transportadores.TransportadorLayoutEDI layoutEDITransportador = null;
            if (carga.Empresa?.EmissaoDocumentosForaDoSistema == true && carga.Empresa?.TransportadorLayoutsEDI != null)
            {
                layoutEDITransportador = (from obj in carga.Empresa.TransportadorLayoutsEDI
                                          where (obj.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.NOTFIS || obj.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.NOTFIS_NOVA_IMPORTACAO) && obj.LayoutEDI.Status == "A"
                                          select obj).FirstOrDefault();

                if (layoutEDITransportador != null && layoutEDITransportador.LayoutEDI.Codigo != layoutEDITipoOperacao?.LayoutEDI.Codigo)
                {
                    AdicionarCargaIntegracaoEDIComTransportador(carga, carga.Empresa, unitOfWork, layoutEDITransportador);
                }
            }
        }

        public static Dominio.ObjetosDeValor.EDI.Notfis.EDINotFis ConverterCargaEmNotFis(Dominio.Entidades.Embarcador.Cargas.Carga Carga, Dominio.Entidades.LayoutEDI layoutEDI, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscalProduto repXMLNotaFiscalProduto = new Repositorio.Embarcador.Pedidos.XMLNotaFiscalProduto(unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> notasFiscais = repPedidoXMLNotaFiscal.BuscarPorCarga(Carga.Codigo);

            if (notasFiscais.Count == 0)
                return null;

            Servicos.WebService.Pessoas.Pessoa serPessoa = new WebService.Pessoas.Pessoa(unitOfWork);

            Dominio.ObjetosDeValor.EDI.Notfis.EDINotFis notfis = new Dominio.ObjetosDeValor.EDI.Notfis.EDINotFis();

            Dominio.Entidades.Cliente emitente = notasFiscais.FirstOrDefault()?.XMLNotaFiscal.Emitente;
            Dominio.Entidades.Cliente expedidor = null;

            if (layoutEDI.ConsiderarDadosExpedidorECTe && notasFiscais.FirstOrDefault()?.CargaPedido.Expedidor != null)
                expedidor = notasFiscais.FirstOrDefault()?.CargaPedido.Expedidor;

            notfis.Emitente = expedidor != null ? expedidor.Nome + (layoutEDI.IncluirCNPJEmitenteArquivoEDI ? " " + expedidor.CPF_CNPJ_SemFormato : string.Empty) : emitente.Nome + (layoutEDI.IncluirCNPJEmitenteArquivoEDI ? " " + emitente.CPF_CNPJ_SemFormato : string.Empty);
            notfis.Destinatario = Carga.Empresa.RazaoSocial;
            notfis.CabecalhoDocumento = new Dominio.ObjetosDeValor.EDI.Notfis.CabecalhoDocumento();
            notfis.CabecalhoDocumento.Embarcadores = new List<Dominio.ObjetosDeValor.EDI.Notfis.Embarcador>();
            notfis.CabecalhoDocumento.Totais = new Dominio.ObjetosDeValor.EDI.Notfis.Totais();

            List<Dominio.Entidades.Cliente> embarcadores = (from obj in notasFiscais select obj.XMLNotaFiscal.Emitente).Distinct().ToList();
            foreach (Dominio.Entidades.Cliente cliEmbarcador in embarcadores)
            {
                Dominio.ObjetosDeValor.EDI.Notfis.Embarcador embarcador = new Dominio.ObjetosDeValor.EDI.Notfis.Embarcador();
                embarcador.Pessoa = serPessoa.ConverterObjetoPessoaCodIntegracao(cliEmbarcador);
                embarcador.DataEmbarqueMercadoria = Carga.DataCriacaoCarga;
                embarcador.Destinatarios = new List<Dominio.ObjetosDeValor.EDI.Notfis.Destinatario>();
                embarcador.NumeroDT = Carga.CodigoCargaEmbarcador;

                List<Dominio.Entidades.Cliente> destinatarios = (from obj in notasFiscais where obj.XMLNotaFiscal.Emitente.CPF_CNPJ == cliEmbarcador.CPF_CNPJ select obj.XMLNotaFiscal.Destinatario).Distinct().ToList();
                foreach (Dominio.Entidades.Cliente cliDestinatario in destinatarios)
                {
                    Dominio.ObjetosDeValor.EDI.Notfis.Destinatario destinatario = new Dominio.ObjetosDeValor.EDI.Notfis.Destinatario();
                    destinatario.Pessoa = serPessoa.ConverterObjetoPessoaCodIntegracao(cliDestinatario, Carga.CodigoCargaEmbarcador);
                    destinatario.NotasFiscais = new List<Dominio.ObjetosDeValor.EDI.Notfis.NotaFiscal>();

                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> notasDestinatario = (from obj in notasFiscais where obj.XMLNotaFiscal.Destinatario.CPF_CNPJ == cliDestinatario.CPF_CNPJ select obj).Distinct().ToList();

                    foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXml in notasDestinatario)
                    {
                        Dominio.ObjetosDeValor.EDI.Notfis.NotaFiscal Nota = new Dominio.ObjetosDeValor.EDI.Notfis.NotaFiscal();

                        Nota.ResponsavelFrete = new Dominio.ObjetosDeValor.EDI.Notfis.ResponsavelFrete();

                        if (layoutEDI.ConsiderarDadosExpedidorECTe && expedidor != null)
                        {
                            Nota.ResponsavelFrete.Pessoa = serPessoa.ConverterObjetoPessoaCodIntegracao(expedidor);

                            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = pedidoXml.CTes.FirstOrDefault(cte => cte.CargaCTe.CTe != null)?.CargaCTe.CTe;

                            if (cte != null)
                            {
                                Nota.CTe = new Dominio.ObjetosDeValor.Embarcador.CTe.CTe();
                                Nota.CTe.Numero = cte.Numero;
                                Nota.ResponsavelFrete.ChaveCTe = cte.Chave;
                            }
                        }
                        else
                            Nota.ResponsavelFrete.Pessoa = serPessoa.ConverterObjetoPessoaCodIntegracao(pedidoXml.CargaPedido.ObterTomador());

                        Nota.NFe = ConverterXmlNotaFiscalEmNota(pedidoXml, unitOfWork);
                        if (Nota.NFe.ModalidadeFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.A_Pagar)
                            Nota.CondicaoPagamento = "F";
                        else
                            Nota.CondicaoPagamento = "C";

                        Nota.Mercadorias = new List<Dominio.ObjetosDeValor.EDI.Notfis.Mercadoria>();
                        List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto> produtos = repXMLNotaFiscalProduto.BuscarPorNotaFiscal(pedidoXml.XMLNotaFiscal.Codigo);

                        int posicaoMercadoria = 1;
                        int totalProdutos = produtos.Count;

                        Dominio.ObjetosDeValor.EDI.Notfis.Mercadoria mercadoria = new Dominio.ObjetosDeValor.EDI.Notfis.Mercadoria();
                        for (int i = 0; i < produtos.Count; i++)
                        {
                            Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto produtoXMLNota = produtos[i];
                            if (posicaoMercadoria == 1)
                            {
                                mercadoria.DadosMercadoria1 = new Dominio.ObjetosDeValor.EDI.Notfis.DadosMercadoria();
                                mercadoria.DadosMercadoria1.QuantidadeVolume = produtoXMLNota.Quantidade;
                                mercadoria.DadosMercadoria1.Acondicionamento = produtoXMLNota.UnidadeMedida;
                                mercadoria.DadosMercadoria1.Mercadoria = produtoXMLNota.Produto.Descricao;
                            }
                            else if (posicaoMercadoria == 2)
                            {
                                mercadoria.DadosMercadoria2 = new Dominio.ObjetosDeValor.EDI.Notfis.DadosMercadoria();
                                mercadoria.DadosMercadoria2.QuantidadeVolume = produtoXMLNota.Quantidade;
                                mercadoria.DadosMercadoria2.Acondicionamento = produtoXMLNota.UnidadeMedida;
                                mercadoria.DadosMercadoria2.Mercadoria = produtoXMLNota.Produto.Descricao;
                            }
                            else if (posicaoMercadoria == 3)
                            {
                                mercadoria.DadosMercadoria3 = new Dominio.ObjetosDeValor.EDI.Notfis.DadosMercadoria();
                                mercadoria.DadosMercadoria3.QuantidadeVolume = produtoXMLNota.Quantidade;
                                mercadoria.DadosMercadoria3.Acondicionamento = produtoXMLNota.UnidadeMedida;
                                mercadoria.DadosMercadoria3.Mercadoria = produtoXMLNota.Produto.Descricao;
                            }
                            else if (posicaoMercadoria == 4)
                            {
                                mercadoria.DadosMercadoria4 = new Dominio.ObjetosDeValor.EDI.Notfis.DadosMercadoria();
                                mercadoria.DadosMercadoria4.QuantidadeVolume = produtoXMLNota.Quantidade;
                                mercadoria.DadosMercadoria4.Acondicionamento = produtoXMLNota.UnidadeMedida;
                                mercadoria.DadosMercadoria4.Mercadoria = produtoXMLNota.Produto.Descricao;
                            }

                            if (posicaoMercadoria == 4 || totalProdutos == i + 1)
                            {
                                Dominio.ObjetosDeValor.EDI.Notfis.Mercadoria mercadoriaClone = new Dominio.ObjetosDeValor.EDI.Notfis.Mercadoria();
                                mercadoriaClone.DadosMercadoria1 = mercadoria.DadosMercadoria1;
                                mercadoriaClone.DadosMercadoria2 = mercadoria.DadosMercadoria2;
                                mercadoriaClone.DadosMercadoria3 = mercadoria.DadosMercadoria3;
                                mercadoriaClone.DadosMercadoria4 = mercadoria.DadosMercadoria4;

                                Nota.Mercadorias.Add(mercadoriaClone);
                                posicaoMercadoria = 0;
                                mercadoria = new Dominio.ObjetosDeValor.EDI.Notfis.Mercadoria();
                            }
                            posicaoMercadoria++;
                        }

                        Nota.NumeroRomaneio = pedidoXml.XMLNotaFiscal.Numero.ToString();
                        Nota.Produtos = new List<Dominio.ObjetosDeValor.Embarcador.NFe.Produtos>();
                        Dominio.ObjetosDeValor.Embarcador.NFe.Produtos produto = new Dominio.ObjetosDeValor.Embarcador.NFe.Produtos();
                        produto.QuantidadeComercial = 0;
                        produto.Descricao = pedidoXml.XMLNotaFiscal.Produto;
                        Nota.NFe.NaturezaOP = pedidoXml.XMLNotaFiscal.NaturezaOP;
                        Nota.Placa = Carga.VeiculosVinculados?.LastOrDefault()?.Placa ?? (Carga.Veiculo?.Placa ?? "");
                        produto.ValorTotal = pedidoXml.XMLNotaFiscal.ValorTotalProdutos;
                        Nota.Produtos.Add(produto);
                        Nota.ComplementoNotaFiscal = new Dominio.ObjetosDeValor.EDI.Notfis.ComplementoNotaFiscal();
                        Nota.ComplementoNotaFiscal.CodigoPerfilFiscal = pedidoXml.XMLNotaFiscal.CFOP;
                        destinatario.NotasFiscais.Add(Nota);
                        notfis.CabecalhoDocumento.Totais.PesoTotal += pedidoXml.Peso;
                        notfis.CabecalhoDocumento.Totais.QuantidadeTotal += pedidoXml.XMLNotaFiscal.Volumes;
                        notfis.CabecalhoDocumento.Totais.ValorTotal += pedidoXml.XMLNotaFiscal.ValorTotalProdutos;
                        notfis.CabecalhoDocumento.Totais.ValorTotalFrete += pedidoXml.ValorFrete;
                    }

                    embarcador.Destinatarios.Add(destinatario);
                }

                notfis.CabecalhoDocumento.Embarcadores.Add(embarcador);
            }

            return notfis;
        }

        #endregion

        #region Métodos Privados

        public static Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal ConverterXmlNotaFiscalEmNota(Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal PedidoXmlNotaFiscal, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.WebService.Pessoas.Pessoa serPessoa = new WebService.Pessoas.Pessoa(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal notaFiscal = new Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal();
            notaFiscal.Chave = PedidoXmlNotaFiscal.XMLNotaFiscal.Chave;
            notaFiscal.DataEmissao = PedidoXmlNotaFiscal.XMLNotaFiscal.DataEmissao != DateTime.MinValue ? PedidoXmlNotaFiscal.XMLNotaFiscal.DataEmissao.ToString("ddMMyyyy") : DateTime.Now.ToString("ddMMyyyy");
            notaFiscal.Destinatario = serPessoa.ConverterObjetoPessoaCodIntegracao(PedidoXmlNotaFiscal.XMLNotaFiscal.Destinatario);
            notaFiscal.Emitente = serPessoa.ConverterObjetoPessoaCodIntegracao(PedidoXmlNotaFiscal.XMLNotaFiscal.Emitente);
            notaFiscal.InformacoesComplementares = "";
            notaFiscal.ModalidadeFrete = PedidoXmlNotaFiscal.XMLNotaFiscal.ModalidadeFrete == ModalidadePagamentoFrete.Pago ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.Pago : PedidoXmlNotaFiscal.XMLNotaFiscal.ModalidadeFrete == ModalidadePagamentoFrete.A_Pagar ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.A_Pagar : Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.NaoDefinido;
            notaFiscal.Modelo = "55";
            notaFiscal.NaturezaOP = "";
            notaFiscal.Numero = PedidoXmlNotaFiscal.XMLNotaFiscal.Numero;
            notaFiscal.PesoBruto = PedidoXmlNotaFiscal.XMLNotaFiscal.Peso;
            notaFiscal.PesoLiquido = PedidoXmlNotaFiscal.XMLNotaFiscal.PesoLiquido;
            notaFiscal.Protocolo = PedidoXmlNotaFiscal.XMLNotaFiscal.Codigo;
            notaFiscal.Rota = "";
            notaFiscal.NumeroPedido = PedidoXmlNotaFiscal.CargaPedido.Pedido.NumeroPedidoEmbarcador;

            if (!string.IsNullOrWhiteSpace(PedidoXmlNotaFiscal.CargaPedido.Pedido.CodigoPedidoCliente))
            {
                if (PedidoXmlNotaFiscal.CargaPedido.Pedido.CodigoPedidoCliente.Length > 7)
                    notaFiscal.Ultimos7DigitosNumeroPedido = PedidoXmlNotaFiscal.CargaPedido.Pedido.CodigoPedidoCliente.Substring(PedidoXmlNotaFiscal.CargaPedido.Pedido.CodigoPedidoCliente.Length - 7, 7);
                if (PedidoXmlNotaFiscal.CargaPedido.Pedido.CodigoPedidoCliente.Length == 7)
                    notaFiscal.Ultimos7DigitosNumeroPedido = PedidoXmlNotaFiscal.CargaPedido.Pedido.CodigoPedidoCliente;
            }

            notaFiscal.Serie = PedidoXmlNotaFiscal.XMLNotaFiscal.Serie.ToString();
            notaFiscal.SituacaoNFeSefaz = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNFeSefaz.Autorizada;
            notaFiscal.TipoOperacaoNotaFiscal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida;
            notaFiscal.Valor = PedidoXmlNotaFiscal.XMLNotaFiscal.Valor;
            notaFiscal.ValorFrete = PedidoXmlNotaFiscal.ValorFrete + PedidoXmlNotaFiscal.ValorTotalComponentes;
            notaFiscal.ValorTotalProdutos = PedidoXmlNotaFiscal.XMLNotaFiscal.ValorTotalProdutos;
            notaFiscal.VolumesTotal = PedidoXmlNotaFiscal.XMLNotaFiscal.Volumes;
            notaFiscal.Cubagem = PedidoXmlNotaFiscal.XMLNotaFiscal.MetrosCubicos > 0 ? PedidoXmlNotaFiscal.XMLNotaFiscal.MetrosCubicos : PedidoXmlNotaFiscal.CargaPedido.Pedido.CubagemTotal;

            return notaFiscal;
        }

        private static void AdicionarCargaIntegracaoEDI(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoLayoutEDI tipoOperacaoLayoutEDI)
        {

            Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDI repControleIntegracaoCargaEDI = new Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDI(unitOfWork);

            string extesao = tipoOperacaoLayoutEDI.LayoutEDI.ExtensaoArquivo;

            Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI arquivoGeracao = new Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI()
            {
                Data = DateTime.Now,
                MensagemRetorno = "",
                NumeroDT = "",
                SituacaoIntegracaoCargaEDI = SituacaoIntegracaoCargaEDI.Integrado,
                ArquivoImportacaoPedido = true,
                GuidArquivo = Guid.NewGuid().ToString(),
                NomeArquivo = Servicos.Embarcador.Integracao.IntegracaoEDI.ObterNomenclaturaLayoutEDI(carga.Codigo, tipoOperacaoLayoutEDI.LayoutEDI.Nomenclatura, carga.Empresa, null, carga.CodigoCargaEmbarcador, DateTime.Now) + (!string.IsNullOrEmpty(extesao) ? extesao : ".txt"),
                LayoutEDI = tipoOperacaoLayoutEDI.LayoutEDI,
                Cargas = new List<Dominio.Entidades.Embarcador.Cargas.Carga>() { carga },
                Transportador = carga.Empresa,
                TipoIntegracao = tipoOperacaoLayoutEDI.TipoIntegracao
            };

            repControleIntegracaoCargaEDI.Inserir(arquivoGeracao);
        }

        /// <summary>
        /// Cria o ControleIntegracaoCargaEDI para o Transportador, que vai ser integrado nas Threads. Por Exemplo, olhar o serviço IntegracoControleIntegracaoCargaEDI.
        /// </summary>
        private static void AdicionarCargaIntegracaoEDIComTransportador(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Transportadores.TransportadorLayoutEDI transportadorLayoutEDI)
        {
            Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDI repControleIntegracaoCargaEDI = new Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDI(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

            string extesao = transportadorLayoutEDI.LayoutEDI.ExtensaoArquivo;

            Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI arquivoGeracao = new Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI()
            {
                Data = DateTime.Now,
                MensagemRetorno = "",
                NumeroDT = "",
                SituacaoIntegracaoCargaEDI = SituacaoIntegracaoCargaEDI.AgIntegracao,
                ArquivoImportacaoPedido = true,
                GuidArquivo = Guid.NewGuid().ToString(),
                NomeArquivo = Servicos.Embarcador.Integracao.IntegracaoEDI.ObterNomenclaturaLayoutEDI(carga.Codigo, transportadorLayoutEDI.LayoutEDI.Nomenclatura, carga.Empresa, null, carga.CodigoCargaEmbarcador, DateTime.Now) + (!string.IsNullOrEmpty(extesao) ? extesao : ".txt"),
                LayoutEDI = transportadorLayoutEDI.LayoutEDI,
                Cargas = new List<Dominio.Entidades.Embarcador.Cargas.Carga>() { carga },
                Transportador = empresa,
                TipoIntegracao = transportadorLayoutEDI.TipoIntegracao
            };

            repControleIntegracaoCargaEDI.Inserir(arquivoGeracao);
        }

        private static void ExcluirCargaIntegracaoEDI(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDI repControleIntegracaoCargaEDI = new Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDI(unitOfWork);
            List<Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI> ListaIntegracaoEDICarga = repControleIntegracaoCargaEDI.ConsultarPorCargaComLayoutEDI(carga.Codigo);
            if (ListaIntegracaoEDICarga != null && ListaIntegracaoEDICarga.Count > 0)
                foreach (var IntegracaoCarga in ListaIntegracaoEDICarga)
                {
                    IntegracaoCarga.Cargas.Clear();
                    repControleIntegracaoCargaEDI.Deletar(IntegracaoCarga);
                }
        }

        #endregion
    }
}
