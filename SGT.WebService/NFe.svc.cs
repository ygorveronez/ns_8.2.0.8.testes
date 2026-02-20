using CoreWCF;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.WebService.Carga;
using System.Text;

namespace SGT.WebService
{
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any, IncludeExceptionDetailInFaults = true)]
    public class NFe(IServiceProvider serviceProvider) : WebServiceBase(serviceProvider), INFe
    {
        #region Métodos de Consulta

        public Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal>> BuscarNotasFiscaisVinculadas(int? protocoloCarga, int? protocoloPedido, string chaveCTe, int? inicio, int? limite)
        {
            ValidarToken();

            protocoloCarga ??= 0;
            protocoloPedido ??= 0;
            inicio ??= 0;
            limite ??= 0;

            Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal>> retorno = new Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal>>();
            retorno.Status = true;
            retorno.Objeto = new Paginacao<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal>();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            Servicos.Embarcador.NFe.NFe serNFe = new Servicos.Embarcador.NFe.NFe(unitOfWork);
            try
            {
                try
                {
                    if (limite == 0)
                    {
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Status = false;
                        retorno.Mensagem = "O limite não pode ser 0.";
                    }
                    else if (limite <= 100)
                    {
                        Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
                        Repositorio.DocumentosCTE repDocumentosCTe = new Repositorio.DocumentosCTE(unitOfWork);
                        Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                        Dominio.Entidades.Embarcador.Cargas.Carga carga = null;
                        if (protocoloCarga > 0)
                            carga = repCarga.BuscarPorProtocolo((int)protocoloCarga);

                        retorno.Objeto.Itens = new List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal>();
                        Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoEmbarcador = null;
                        List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> notasFiscais = null;
                        bool consultaFeeder = false;
                        if (carga != null && carga.Pedidos != null && carga.Pedidos.Count > 0 && carga.Pedidos.Any(p => p.TipoPropostaMultimodal == TipoPropostaMultimodal.Feeder))
                        {
                            notasFiscais = repPedidoXMLNotaFiscal.ConsultarNotasVinculadas((int)protocoloCarga, (int)protocoloPedido, chaveCTe, 0, 0);
                            consultaFeeder = true;
                        }
                        else
                            notasFiscais = repPedidoXMLNotaFiscal.ConsultarNotasVinculadas((int)protocoloCarga, (int)protocoloPedido, chaveCTe, (int)inicio, (int)limite);

                        string ultimoProdutoPredominante = "";
                        int qtdNotasDuplicadas = 0;
                        foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal in notasFiscais)
                        {
                            string produtoPredominante = "";
                            if (pedidoXMLNotaFiscal.CargaPedido != null && pedidoXMLNotaFiscal.CargaPedido.Pedido != null && !string.IsNullOrWhiteSpace(pedidoXMLNotaFiscal.CargaPedido.Pedido.ProdutoPredominante))
                                produtoPredominante = pedidoXMLNotaFiscal.CargaPedido.Pedido.ProdutoPredominante;
                            else if (pedidoXMLNotaFiscal.CargaPedido != null && pedidoXMLNotaFiscal.CargaPedido.Pedido != null && pedidoXMLNotaFiscal.CargaPedido.Pedido.Produtos != null && pedidoXMLNotaFiscal.CargaPedido.Pedido.Produtos.Count > 0)
                                produtoPredominante = pedidoXMLNotaFiscal.CargaPedido.Pedido.Produtos.FirstOrDefault().Produto?.Descricao ?? "";
                            if (!string.IsNullOrWhiteSpace(produtoPredominante))
                                ultimoProdutoPredominante = produtoPredominante;
                            if (produtoEmbarcador == null && pedidoXMLNotaFiscal.CargaPedido != null && pedidoXMLNotaFiscal.CargaPedido.Pedido != null && pedidoXMLNotaFiscal.CargaPedido.Pedido.Produtos != null && pedidoXMLNotaFiscal.CargaPedido.Pedido.Produtos.Count > 0)
                                produtoEmbarcador = pedidoXMLNotaFiscal.CargaPedido.Pedido.Produtos.FirstOrDefault().Produto;

                            Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal nf = serNFe.ConverterXMLEmNota(pedidoXMLNotaFiscal.XMLNotaFiscal, produtoPredominante, unitOfWork, produtoEmbarcador, pedidoXMLNotaFiscal.CTes?.ToList() ?? null, pedidoXMLNotaFiscal.CargaPedido?.Pedido?.Container?.Codigo ?? 0, pedidoXMLNotaFiscal.CargaPedido.Pedido?.Protocolo ?? 0);
                            List<string> codigoContainer = new List<string>();
                            if (nf.Containeres != null)
                                codigoContainer = nf.Containeres.Select(c => c.Numero).ToList();

                            if (retorno.Objeto.Itens != null && codigoContainer.Count > 0 && retorno.Objeto.Itens.Count > 0 && !string.IsNullOrWhiteSpace(nf.Chave) && retorno.Objeto.Itens.Any(c => c.Chave == nf.Chave && c.Containeres.Any(o => codigoContainer.Contains(o.Numero))))
                            {
                                qtdNotasDuplicadas++;
                                continue;
                            }
                            else if (retorno.Objeto.Itens != null && codigoContainer.Count > 0 && retorno.Objeto.Itens.Count > 0 && string.IsNullOrWhiteSpace(nf.Chave) && nf.Numero > 0 && retorno.Objeto.Itens.Any(c => c.Numero == nf.Numero && c.Containeres.Any(o => codigoContainer.Contains(o.Numero))))
                            {
                                qtdNotasDuplicadas++;
                                continue;
                            }
                            retorno.Objeto.Itens.Add(nf);
                        }
                        retorno.Objeto.NumeroTotalDeRegistro = repPedidoXMLNotaFiscal.ContarNotasVinculadas((int)protocoloCarga, (int)protocoloPedido, chaveCTe);
                        int qtdDocumentosSemXML = protocoloCarga > 0 || protocoloPedido > 0 ? repDocumentosCTe.ContarConsultarNotasVinculadas((int)protocoloCarga, (int)protocoloPedido, chaveCTe) : 0;

                        if ((qtdDocumentosSemXML + retorno.Objeto.NumeroTotalDeRegistro) < (inicio + limite) && qtdDocumentosSemXML > 0)
                        {
                            List<Dominio.Entidades.DocumentosCTE> documentos = repDocumentosCTe.ConsultarNotasVinculadas((int)protocoloCarga, (int)protocoloPedido, chaveCTe);
                            foreach (Dominio.Entidades.DocumentosCTE doc in documentos)
                            {
                                Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal nf = serNFe.ConverterXMLEmNota(doc, ultimoProdutoPredominante, unitOfWork, produtoEmbarcador, doc.CTE);
                                List<string> codigoContainer = new List<string>();
                                if (nf.Containeres != null)
                                    codigoContainer = nf.Containeres.Select(c => c.Numero).ToList();

                                if (retorno.Objeto.Itens != null && codigoContainer.Count > 0 && retorno.Objeto.Itens.Count > 0 && !string.IsNullOrWhiteSpace(nf.Chave) && retorno.Objeto.Itens.Any(c => c.Chave == nf.Chave && c.Containeres.Any(o => codigoContainer.Contains(o.Numero))))
                                {
                                    qtdNotasDuplicadas++;
                                    continue;
                                }
                                else if (retorno.Objeto.Itens != null && codigoContainer.Count > 0 && retorno.Objeto.Itens.Count > 0 && string.IsNullOrWhiteSpace(nf.Chave) && nf.Numero > 0 && retorno.Objeto.Itens.Any(c => c.Numero == nf.Numero && c.Containeres.Any(o => codigoContainer.Contains(o.Numero))))
                                {
                                    qtdNotasDuplicadas++;
                                    continue;
                                }

                                retorno.Objeto.Itens.Add(nf);
                            }
                        }

                        if (consultaFeeder)
                        {
                            List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal> novasNotas = new List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal>();
                            novasNotas = retorno.Objeto.Itens.Skip((int)inicio).Take((int)limite).ToList();
                            retorno.Objeto.Itens = new List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal>();
                            retorno.Objeto.Itens.AddRange(novasNotas);
                        }

                        retorno.Objeto.NumeroTotalDeRegistro += qtdDocumentosSemXML;
                        retorno.Objeto.NumeroTotalDeRegistro = retorno.Objeto.NumeroTotalDeRegistro - qtdNotasDuplicadas;

                        Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou notas fiscais vinculadas confirmado no destinatário", unitOfWork);
                    }
                    else
                    {
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Status = false;
                        retorno.Mensagem = "O limite não pode ser maior que 100";
                    }
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                    retorno.Mensagem = "Ocorreu uma falha ao consultar as notas fiscais vinculadas confirmado";
                }
                finally
                {
                    unitOfWork.Dispose();
                }

                retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                return retorno;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao consultar as notas fiscais vinculadas confirmado";
                return retorno;
            }
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.NFe.CanhotoAvulso>> BuscarCanhotosAvulsos(int protocoloIntegracaoCarga, int? inicio, int? limite)
        {
            ValidarToken();

            if (inicio == null) inicio = 0;
            if (limite == null) limite = 50;

            Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.NFe.CanhotoAvulso>> retorno = new Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.NFe.CanhotoAvulso>>();
            retorno.Status = true;
            retorno.Objeto = new Paginacao<Dominio.ObjetosDeValor.Embarcador.NFe.CanhotoAvulso>();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            Servicos.WebService.Pessoas.Pessoa serPessoa = new Servicos.WebService.Pessoas.Pessoa(unitOfWork);
            try
            {
                if (limite <= 100)
                {
                    Repositorio.Embarcador.Canhotos.CanhotoAvulso repCanhotoAvulso = new Repositorio.Embarcador.Canhotos.CanhotoAvulso(unitOfWork);
                    Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

                    Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();
                    List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotos = repCanhotoAvulso.ConsultarPorProtocoloCarga(
                        protocoloIntegracaoCarga,
                        "Codigo",
                        "desc",
                        (int)inicio,
                        (int)limite
                    );

                    retorno.Objeto.Itens = new List<Dominio.ObjetosDeValor.Embarcador.NFe.CanhotoAvulso>();


                    foreach (Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto in canhotos)
                    {
                        Dominio.ObjetosDeValor.Embarcador.NFe.CanhotoAvulso canhotoAvulso = new Dominio.ObjetosDeValor.Embarcador.NFe.CanhotoAvulso();
                        canhotoAvulso.Numero = canhoto.Numero;
                        canhotoAvulso.DataEmissao = canhoto.DataEmissao.ToString("dd/MM/yyyy HH:mm:ss");
                        canhotoAvulso.SituacaoCanhoto = canhoto.SituacaoCanhoto;
                        canhotoAvulso.Destinatario = serPessoa.ConverterObjetoPessoa(canhoto.Destinatario);
                        string mensage = "";
                        byte[] pdf = Servicos.Embarcador.Canhotos.Canhoto.GerarCanhotoAvulso(canhoto.Codigo, unitOfWork, out mensage);
                        if (pdf == null)
                            canhotoAvulso.PDF = mensage;
                        else
                        {
                            if (configuracao.UtilizarCodificacaoUTF8ConversaoPDF)
                                canhotoAvulso.PDF = Convert.ToBase64String(Encoding.Convert(Encoding.GetEncoding("ISO-8859-1"), Encoding.UTF8, pdf));
                            else
                                canhotoAvulso.PDF = Convert.ToBase64String(pdf);
                        }
                        retorno.Objeto.Itens.Add(canhotoAvulso);
                    }
                    //retorno.Objeto.NumeroTotalDeRegistro = repCanhotoAvulso.ContarConsultaPorCarga(protocoloIntegracaoCarga);
                    retorno.Objeto.NumeroTotalDeRegistro = repCanhotoAvulso.ContarConsultaPorProtocoloCarga(protocoloIntegracaoCarga);

                    Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou canhotos avulsos", unitOfWork);
                }
                else
                {
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Status = false;
                    retorno.Mensagem = "O limite não pode ser maior que 100";
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao consultar os canhotos avulsos";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>> BuscarCargasAguardandoNotasFiscais(int? inicio, int? limite, string codigoTipoOperacao)
        {
            Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>> retorno = new Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>>();
            retorno.Status = true;
            retorno.Objeto = new Paginacao<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            Dominio.Entidades.WebService.Integradora integradora = ValidarToken();

            inicio ??= 0;
            limite ??= 0;

            try
            {
                if (limite <= 100)
                {
                    Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

                    Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                    Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repositorioConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(unitOfWork);

                    Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
                    Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebService = repositorioConfiguracaoWebService.BuscarConfiguracaoPadrao();

                    bool naoRetornarCargasCanceladas = configuracaoWebService.NaoRetornarCargasCanceladasMetodoBuscarPendetesNotasFiscais;

                    //todo: fixo para não dar problema na tirol, ver para criar variavel pois a piracanjuba reclama.
                    limite = 0;
                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargaPedido = repCargaPedido.BuscarPendetesNotasFiscais(configuracaoTMS.ExigeInformarCienciaDoEnvioDasNotasAntesDeEmitirDocumentos, integradora.GrupoPessoas?.Codigo ?? 0, (int)inicio, (int)limite, codigoTipoOperacao, naoRetornarCargasCanceladas);
                    retorno.Objeto.NumeroTotalDeRegistro = repCargaPedido.ContarPendetesNotasFiscais(integradora.GrupoPessoas?.Codigo ?? 0, codigoTipoOperacao);
                    retorno.Objeto.Itens = new List<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>();

                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in listaCargaPedido)
                    {
                        Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo = new Dominio.ObjetosDeValor.WebService.Carga.Protocolos();
                        protocolo.protocoloIntegracaoCarga = naoRetornarCargasCanceladas ? cargaPedido.Carga.Codigo : cargaPedido.CargaOrigem.Protocolo; //cargaPedido.Carga.Codigo;
                        protocolo.protocoloIntegracaoPedido = cargaPedido.Pedido.Protocolo; //cargaPedido.Pedido.Codigo;
                        retorno.Objeto.Itens.Add(protocolo);
                    }
                    retorno.Status = true;

                    Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou cargas aguardando notas fiscais", unitOfWork);
                }
                else
                {
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Status = false;
                    retorno.Mensagem = "O limite não pode ser maior que 100";
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao consultar as Cargas aguardando Notas Fiscais";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal>> BuscarNotasFiscaisComRecebimentoConfirmadoNoDestinatario(int? inicio, int? limite)
        {
            ValidarToken();

            if (inicio == null) inicio = 0;
            if (limite == null) limite = 50;

            Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal>> retorno = new Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal>>();
            retorno.Status = true;
            retorno.Objeto = new Paginacao<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal>();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            Servicos.Embarcador.NFe.NFe serNFe = new Servicos.Embarcador.NFe.NFe(unitOfWork);

            try
            {
                if (limite <= 100)
                {
                    Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);

                    retorno.Objeto.Itens = new List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal>();
                    List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotosNotasFiscais = repCanhoto.ConsultarCanhotosNFeEntreguesAguardandoIntegracao(
                        (int)inicio,
                        (int)limite
                    );

                    foreach (Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto in canhotosNotasFiscais)
                    {
                        Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal nf = serNFe.ConverterCanhotoEmNota(canhoto, unitOfWork);
                        retorno.Objeto.Itens.Add(nf);
                    }
                    retorno.Objeto.NumeroTotalDeRegistro = repCanhoto.ContarCanhotosNFeEntreguesAguardandoIntegracao();

                    Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou notas fiscais com recebimento confirmado no destinatário", unitOfWork);
                }
                else
                {
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Status = false;
                    retorno.Mensagem = "O limite não pode ser maior que 100";
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao consultar as notas fiscais com recebimento confirmado";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<Dominio.ObjetosDeValor.Embarcador.NFe.CanhotoNotaFiscal> BuscarCanhoto(int protocolo)
        {
            ValidarToken();

            Retorno<Dominio.ObjetosDeValor.Embarcador.NFe.CanhotoNotaFiscal> retorno = new Retorno<Dominio.ObjetosDeValor.Embarcador.NFe.CanhotoNotaFiscal>();
            retorno.Status = true;
            retorno.Objeto = new Dominio.ObjetosDeValor.Embarcador.NFe.CanhotoNotaFiscal();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            Servicos.WebService.Pessoas.Pessoa serPessoa = new Servicos.WebService.Pessoas.Pessoa(unitOfWork);
            try
            {
                Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
                Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = repCanhoto.BuscarPorCodigo(protocolo);

                if (canhoto != null)
                {
                    if (canhoto.SituacaoDigitalizacaoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto.Digitalizado)
                    {
                        string extensao = System.IO.Path.GetExtension(canhoto.NomeArquivo).ToLower();
                        string caminho = Servicos.Embarcador.Canhotos.Canhoto.CaminhoCanhoto(canhoto, unitOfWork);
                        string fileLocation = Utilidades.IO.FileStorageService.Storage.Combine(caminho, canhoto.GuidNomeArquivo + extensao);
                        string caminhoOriginal = Utilidades.IO.FileStorageService.Storage.Combine(caminho, canhoto.NomeArquivo);

                        byte[] bufferCanhoto = null;

                        if (Utilidades.IO.FileStorageService.Storage.Exists(fileLocation))
                        {
                            bufferCanhoto = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(fileLocation);
                            if (bufferCanhoto != null)
                            {
                                //retorno.Objeto.Arquivo = Convert.ToBase64String(Encoding.Convert(Encoding.GetEncoding("ISO-8859-1"), Encoding.UTF8, bufferCanhoto));
                                retorno.Objeto.Arquivo = Convert.ToBase64String(bufferCanhoto);
                                retorno.Objeto.Extensao = extensao;
                                retorno.Status = true;

                                Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou canhoto nota fiscal", unitOfWork);
                            }
                            else
                            {
                                retorno.Status = false;
                                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                                retorno.Mensagem = "Não foi possivel carregar canhoto.";
                            }
                        }
                        else
                        {
                            retorno.Status = false;
                            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                            retorno.Mensagem = "Digitalização do canhoto não localizado.";
                        }
                    }
                    else
                    {
                        retorno.Status = false;
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Mensagem = "Situação da digitalização (" + canhoto.DescricaoDigitalizacao + ") não permite retorno.";
                    }
                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "Canhoto não localizado.";
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao consultar os canhotos avulsos";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<Dominio.ObjetosDeValor.Embarcador.NFe.CanhotoNotaFiscal> BuscarCanhotoPorChaveNFe(string chaveNFe)
        {
            ValidarToken();

            Retorno<Dominio.ObjetosDeValor.Embarcador.NFe.CanhotoNotaFiscal> retorno = new Retorno<Dominio.ObjetosDeValor.Embarcador.NFe.CanhotoNotaFiscal>();
            retorno.Status = true;
            retorno.Objeto = new Dominio.ObjetosDeValor.Embarcador.NFe.CanhotoNotaFiscal();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            Servicos.WebService.Pessoas.Pessoa serPessoa = new Servicos.WebService.Pessoas.Pessoa(unitOfWork);
            try
            {
                Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
                Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = repCanhoto.BuscarPorChave(Utilidades.String.OnlyNumbers(chaveNFe));

                if (canhoto != null)
                {
                    if (canhoto.SituacaoDigitalizacaoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto.Digitalizado)
                    {
                        string extensao = System.IO.Path.GetExtension(canhoto.NomeArquivo).ToLower();
                        string caminho = Servicos.Embarcador.Canhotos.Canhoto.CaminhoCanhoto(canhoto, unitOfWork);
                        string fileLocation = Utilidades.IO.FileStorageService.Storage.Combine(caminho, canhoto.GuidNomeArquivo + extensao);
                        string caminhoOriginal = Utilidades.IO.FileStorageService.Storage.Combine(caminho, canhoto.NomeArquivo);

                        byte[] bufferCanhoto = null;

                        if (Utilidades.IO.FileStorageService.Storage.Exists(fileLocation))
                        {
                            bufferCanhoto = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(fileLocation);
                            if (bufferCanhoto != null)
                            {
                                //retorno.Objeto.Arquivo = Convert.ToBase64String(Encoding.Convert(Encoding.GetEncoding("ISO-8859-1"), Encoding.UTF8, bufferCanhoto));
                                retorno.Objeto.Arquivo = Convert.ToBase64String(bufferCanhoto);
                                retorno.Objeto.Extensao = extensao;
                                retorno.Status = true;

                                Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou canhoto nota fiscal", unitOfWork);
                            }
                            else
                            {
                                retorno.Status = false;
                                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                                retorno.Mensagem = "Não foi possivel carregar canhoto.";
                            }
                        }
                        else
                        {
                            retorno.Status = false;
                            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                            retorno.Mensagem = "Digitalização do canhoto não localizado.";
                        }
                    }
                    else
                    {
                        retorno.Status = false;
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Mensagem = "Situação da digitalização (" + canhoto.DescricaoDigitalizacao + ") não permite retorno.";
                    }
                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "Canhoto não localizado para a NFe chave " + chaveNFe + " .";
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao consultar os canhotos avulsos";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<RetornoSolicitacaoCancelamento> SolicitarCancelamentoDoPedido(int protocoloIntegracaoPedido, string motivoDoCancelamento)
        {
            Servicos.Log.TratarErro("SolicitarCancelamentoDoPedido - protocoloIntegracaoPedido: " + protocoloIntegracaoPedido.ToString());

            ValidarToken();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            string erro = string.Empty;

            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repositorioPedido.BuscarPorProtocolo(protocoloIntegracaoPedido);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();

                if (configuracao.TrocarPreCargaPorCarga)
                {
                    Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

                    if (repositorioCargaPedido.ExistePorPedidoPorProtocolo(protocoloIntegracaoPedido, cargasAtivas: false))
                        throw new WebServiceException("O pedido já está vinculado à uma carga, não sendo possível realizar a exclusão do mesmo.");

                    Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repositorioCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(unitOfWork);

                    if (repositorioCarregamentoPedido.ExisteCarregamentoAtivoPorPedidoPorProtocolo(protocoloIntegracaoPedido))
                        throw new WebServiceException("O pedido já está vinculado à um carregamento, não sendo possível realizar a exclusão do mesmo.");
                }
                else
                    Servicos.Embarcador.Pedido.Pedido.RemoverPedidoCancelado(pedido, unitOfWork, TipoServicoMultisoftware, Auditado, configuracao, configuracaoGeralCarga);

                if (!Servicos.Embarcador.Pedido.Pedido.CancelarPedido(out erro, pedido, TipoPedidoCancelamento.Cancelamento, null, motivoDoCancelamento, unitOfWork, TipoServicoMultisoftware, Auditado, configuracao, Cliente))
                    throw new WebServiceException(erro);

                unitOfWork.CommitChanges();

                return Retorno<RetornoSolicitacaoCancelamento>.CriarRetornoSucesso(RetornoSolicitacaoCancelamento.Cancelada);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return Retorno<RetornoSolicitacaoCancelamento>.CriarRetornoDadosInvalidos(excecao.Message, RetornoSolicitacaoCancelamento.CancelamentoRejeitado);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return Retorno<RetornoSolicitacaoCancelamento>.CriarRetornoExcecao(!string.IsNullOrWhiteSpace(erro) ? erro : "Ocorreu uma falha ao solicitar o cancelamento do pedido", RetornoSolicitacaoCancelamento.CancelamentoRejeitado);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.NFe.DadosNotaFiscal>> BuscarNotasPorNumeroCarga(List<string> numerosCarga, string dataCriacaoCargaInicial, string dataCriacaoCargaFinal, string codigoIntegracaoFilial, string destinatario, string codigoTipoOperacao)
        {
            ValidarToken();

            Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.NFe.DadosNotaFiscal>> retorno = new Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.NFe.DadosNotaFiscal>>();
            retorno.Status = true;
            retorno.Objeto = new Paginacao<Dominio.ObjetosDeValor.WebService.NFe.DadosNotaFiscal>();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            Servicos.Embarcador.NFe.NFe serNFe = new Servicos.Embarcador.NFe.NFe(unitOfWork);
            try
            {
                try
                {

                    Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
                    Repositorio.DocumentosCTE repDocumentosCTe = new Repositorio.DocumentosCTE(unitOfWork);
                    Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                    DateTime.TryParseExact(dataCriacaoCargaInicial, "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime dataInicial);
                    DateTime.TryParseExact(dataCriacaoCargaFinal, "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime dataFinal);

                    List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = repCarga.BuscarPorListaCarga(numerosCarga, codigoIntegracaoFilial, dataInicial, dataFinal, codigoTipoOperacao);

                    retorno.Objeto.Itens = new List<Dominio.ObjetosDeValor.WebService.NFe.DadosNotaFiscal>();

                    if (cargas.Count > 0)
                    {
                        List<int> codigosCargas = (from o in cargas select o.Codigo).ToList();
                        List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> notasFiscais = repPedidoXMLNotaFiscal.ConsultarNotasVinculadasPorListaCargas(codigosCargas);
                        for (int i = 0; i < notasFiscais.Count; i++)
                        {
                            double.TryParse(destinatario, out double cnpjDestinatario);
                            if ((string.IsNullOrWhiteSpace(destinatario)) || (notasFiscais[i].XMLNotaFiscal.Destinatario.CPF_CNPJ == cnpjDestinatario || notasFiscais[i].XMLNotaFiscal.Destinatario.CodigoIntegracao == destinatario))
                            {
                                Dominio.ObjetosDeValor.WebService.NFe.DadosNotaFiscal notaFiscal = new Dominio.ObjetosDeValor.WebService.NFe.DadosNotaFiscal();
                                notaFiscal.Numero = notasFiscais[i].XMLNotaFiscal.Numero;
                                notaFiscal.Serie = notasFiscais[i].XMLNotaFiscal.SerieOuSerieDaChave;
                                notaFiscal.Chave = notasFiscais[i].XMLNotaFiscal.Chave;
                                notaFiscal.CodigoIntegracaoEmitente = !string.IsNullOrWhiteSpace(notasFiscais[i].XMLNotaFiscal.Emitente?.CodigoIntegracao) ? notasFiscais[i].XMLNotaFiscal.Emitente?.CodigoIntegracao : notasFiscais[i].XMLNotaFiscal.Emitente?.CPF_CNPJ_SemFormato;
                                notaFiscal.CodigoIntegracaoDestinatario = !string.IsNullOrWhiteSpace(notasFiscais[i].XMLNotaFiscal.Destinatario?.CodigoIntegracao) ? notasFiscais[i].XMLNotaFiscal.Destinatario?.CodigoIntegracao : notasFiscais[i].XMLNotaFiscal.Destinatario?.CPF_CNPJ_SemFormato;
                                notaFiscal.NumeroCarga = notasFiscais[i].CargaPedido.Carga.CodigoCargaEmbarcador;

                                retorno.Objeto.Itens.Add(notaFiscal);
                            }
                        }

                        retorno.Objeto.NumeroTotalDeRegistro = notasFiscais.Count;
                    }
                    else
                    {
                        retorno.Status = false;
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Mensagem = "Não foi localizada uma carga com os parâmetros enviados.";
                    }

                    Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou notas fiscais por carga", unitOfWork);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                    retorno.Mensagem = "Ocorreu uma falha ao consultar as notas fiscais vinculadas confirmado";
                }
                finally
                {
                    unitOfWork.Dispose();
                }

                retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                return retorno;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao consultar as notas fiscais vinculadas confirmado";
                return retorno;
            }
        }

        #endregion

        #region Métodos Envio Arquivos

        public Retorno<string> EnviarArquivoXMLNFe(Stream arquivo)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<string>.CreateFrom(new Servicos.WebService.NFe.NotaFiscal(unitOfWork, TipoServicoMultisoftware, Auditado, Cliente).EnviarArquivoXMLNFe(arquivo));
            });
        }

        public Retorno<string> EnviarStreamImagemCanhoto(Stream imagem)
        {
            ValidarToken();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                Retorno<string> retorno = new Retorno<string>();
                Servicos.Embarcador.Mobile.Canhotos.Canhotos serCanhoto = new Servicos.Embarcador.Mobile.Canhotos.Canhotos();
                retorno.Status = true;
                try
                {
                    string tokenImagem = "";
                    retorno.Mensagem = serCanhoto.SalvarImagemCanhoto(imagem, out tokenImagem, unitOfWork);
                    if (!string.IsNullOrWhiteSpace(retorno.Mensagem))
                    {
                        retorno.Status = false;
                        retorno.Objeto = "";
                    }
                    else
                    {
                        retorno.Objeto = tokenImagem;
                    }
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                    retorno.Status = false;
                    retorno.Mensagem = "Ocorreu uma falha ao enviar a imagem do Canhoto";
                }

                retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                return retorno;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new Retorno<string>() { Mensagem = "Ocorreu uma falha ao enviar a imagem do canhoto.", Status = false, CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica };
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<string> EnviarStreamImagemOcorrencia(Stream imagem)
        {
            ValidarToken();
            try
            {
                Retorno<string> retorno = new Retorno<string>();
                Servicos.Embarcador.Mobile.Ocorrencias.Ocorrencia serOcorrencia = new Servicos.Embarcador.Mobile.Ocorrencias.Ocorrencia();
                retorno.Status = true;
                try
                {
                    string tokenImagem = "";
                    retorno.Mensagem = serOcorrencia.SalvarImagem(imagem, out tokenImagem, null);
                    if (!string.IsNullOrWhiteSpace(retorno.Mensagem))
                    {
                        retorno.Status = false;
                        retorno.Objeto = "";
                    }
                    else
                    {
                        retorno.Objeto = tokenImagem;
                    }
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                    retorno.Status = false;
                    retorno.Mensagem = "Ocorreu uma falha ao enviar a imagem do Canhoto";
                }

                retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                return retorno;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new Retorno<string>() { Mensagem = "Ocorreu uma falha ao enviar a imagem do canhoto.", Status = false, CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica };
            }
        }

        public Retorno<bool> EnviarImagemCanhotoLeituraOCR(int usuario, string tokenImagem)
        {
            ValidarToken();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Servicos.Embarcador.Mobile.Canhotos.Canhotos serCanhoto = new Servicos.Embarcador.Mobile.Canhotos.Canhotos();
            Retorno<bool> retorno = new Retorno<bool>();
            retorno.Status = true;
            try
            {
                Servicos.Embarcador.Canhotos.LeitorOCR srvLeitorOCR = new Servicos.Embarcador.Canhotos.LeitorOCR(unitOfWork);
                retorno.Mensagem = serCanhoto.EnviarImagemCanhotoLeituraOCR(tokenImagem, usuario, ClienteAcesso.Cliente.Codigo, unitOfWork);
                if (!string.IsNullOrWhiteSpace(retorno.Mensagem))
                {
                    retorno.Status = false;
                    retorno.Objeto = false;
                }
                else
                {
                    string caminhoRaiz = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRaiz;
                    string apiKey = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().APIOCRKey;
                    string apilink = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().APIOCRLink;

                    string[] nomeSplit = tokenImagem.Split('_');

                    Dominio.Entidades.Embarcador.Canhotos.ControleLeituraImagemCanhoto controleLeituraImagemCanhoto = new Dominio.Entidades.Embarcador.Canhotos.ControleLeituraImagemCanhoto
                    {
                        Data = DateTime.Now,
                        GuidArquivo = nomeSplit[0] + "_" + nomeSplit[1] + "_" + nomeSplit[2] + ".jpg",
                        NumeroDocumento = "",
                        MensagemRetorno = "",
                        NomeArquivo = nomeSplit[2] + ".jpg",
                        SituacaoleituraImagemCanhoto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoleituraImagemCanhoto.AgProcessamento,
                        Extensao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ExtensaoArquivo.JPG
                    };

                    srvLeitorOCR.DefinirAPI(apilink, apiKey);
                    srvLeitorOCR.InterpretarCanhotoPendente(ref controleLeituraImagemCanhoto, caminhoRaiz, unitOfWork, TipoServicoMultisoftware);

                    if (!string.IsNullOrWhiteSpace(controleLeituraImagemCanhoto.MensagemRetorno))
                    {
                        retorno.Mensagem = controleLeituraImagemCanhoto.MensagemRetorno;
                        retorno.Status = false;
                        retorno.Objeto = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Status = false;
                retorno.Mensagem = "Ocorreu uma falha ao enviar a imagem para leitura OCR";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<bool> EnviarImagemCanhoto(string latitude, string longitude, int? usuario, int? canhoto, string tokenImagem, DateTime? dataEntregaNotaCliente, string chaveNFe, int? numeroNota, int? serieNota, string cnpjCpfEmissor)
        {
            Dominio.Entidades.WebService.Integradora integradora = ValidarToken();

            usuario ??= 0;
            canhoto ??= 0;
            numeroNota ??= 0;
            serieNota ??= 0;

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

            Servicos.Embarcador.Mobile.Canhotos.Canhotos serCanhoto = new Servicos.Embarcador.Mobile.Canhotos.Canhotos();
            Retorno<bool> retorno = new Retorno<bool>();
            retorno.Status = true;
            try
            {
                if (integradora != null && integradora.Empresa != null)
                    usuario = repUsuario.BuscarPorEmpresa(integradora.Empresa.Codigo, "U")?.FirstOrDefault()?.Codigo ?? 0;

                retorno.Mensagem = serCanhoto.EnviarImagemCanhoto(latitude, longitude, (int)canhoto, tokenImagem, (int)usuario, ClienteAcesso.Cliente.Codigo, dataEntregaNotaCliente, unitOfWork, chaveNFe, (int)numeroNota, (int)serieNota, cnpjCpfEmissor);

                if (!string.IsNullOrWhiteSpace(retorno.Mensagem))
                {
                    retorno.Status = false;
                    retorno.Objeto = false;
                }
                else
                {
                    unitOfWork.CommitChanges();
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Status = false;
                retorno.Mensagem = "Ocorreu uma falha ao confirmar o envio da imagem do Canhoto";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<bool> EnviarDigitalizacaoCanhoto(Dominio.ObjetosDeValor.Embarcador.NFe.Canhoto canhotoIntegracao)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                Dominio.ObjetosDeValor.WebService.Retorno<int> retorno = new Servicos.WebService.Canhoto.Canhoto(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, "").EnviarDigitalizacaoCanhotoAsync(canhotoIntegracao, integradora, default).GetAwaiter().GetResult();

                return Retorno<bool>.CreateFrom(new Dominio.ObjetosDeValor.WebService.Retorno<bool>
                {
                    Status = retorno.Status,
                    Mensagem = retorno.Mensagem,
                    CodigoMensagem = retorno.CodigoMensagem,
                    DataRetorno = retorno.DataRetorno,
                    Objeto = retorno.Objeto > 0
                });
            });
        }


        public Retorno<bool> EnviarImagemOcorrencia(int? usuario, int? ocorrencia, string tokenImagem)
        {
            ValidarToken();

            usuario ??= 0;
            ocorrencia ??= 0;

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Servicos.Embarcador.Mobile.Ocorrencias.Ocorrencia serOcorrencia = new Servicos.Embarcador.Mobile.Ocorrencias.Ocorrencia();
            Retorno<bool> retorno = new Retorno<bool>();
            retorno.Status = true;
            try
            {
                retorno.Mensagem = serOcorrencia.EnviarImagemOcorrencia((int)ocorrencia, tokenImagem, (int)usuario, ClienteAcesso.Cliente.Codigo, unitOfWork);
                if (!string.IsNullOrWhiteSpace(retorno.Mensagem))
                {
                    retorno.Status = false;
                    retorno.Objeto = false;
                }
            }
            catch (ServicoException excecao)
            {
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                retorno.Status = false;
                retorno.Mensagem = excecao.Message;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Status = false;
                retorno.Mensagem = "Ocorreu uma falha ao confirmar o envio da imagem da ocorrencia";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.NFe.Canhoto>> BuscarCanhotosNotasFiscaisDigitalizados(int? inicio, int? limite)
        {
            Dominio.Entidades.WebService.Integradora integradora = ValidarToken();

            inicio ??= 0;
            limite ??= 0;

            Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.NFe.Canhoto>> retorno = new Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.NFe.Canhoto>>();
            retorno.Mensagem = "";
            retorno.Objeto = new Paginacao<Dominio.ObjetosDeValor.Embarcador.NFe.Canhoto>();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                Repositorio.Embarcador.TorreControle.ConfiguracaoQualidadeEntrega repositorioConfiguracaoEntregaQualidade = new Repositorio.Embarcador.TorreControle.ConfiguracaoQualidadeEntrega(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto repConfigCanhoto = new(unitOfWork);
                Dominio.Entidades.Embarcador.TorreControle.ConfiguracaoQualidadeEntrega configuracaoQualidadeEntrega = repositorioConfiguracaoEntregaQualidade.BuscarConfiguracaoPadraoAsync().Result;

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebService = repConfiguracaoWebService.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto configuracaoCanhoto = repConfigCanhoto.BuscarConfiguracaoPadrao();

                if (limite <= 100)
                {
                    retorno.Objeto.Itens = new List<Dominio.ObjetosDeValor.Embarcador.NFe.Canhoto>();
                    List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotos = repCanhoto.BuscarCanhotosNotasFiscaisPendentesIntegracaoDigitalizacao(
                        configuracao.UtilizaPgtoCanhoto,
                        configuracao.RetornarCanhotosViaIntegracaoEmQualquerSituacao,
                        (int)inicio,
                        (int)limite,
                        integradora?.Empresa?.Codigo ?? 0,
                        configuracaoQualidadeEntrega,
                        configuracaoCanhoto.RetornarSomenteCanhotoComNFeEntregueEmBuscarCanhotosNotasFiscaisDigitalizados
                    );

                    foreach (Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto in canhotos)
                    {
                        string extensao = System.IO.Path.GetExtension(canhoto.NomeArquivo).ToLower();
                        string caminho = Servicos.Embarcador.Canhotos.Canhoto.CaminhoCanhoto(canhoto, unitOfWork);
                        string fileLocation = Utilidades.IO.FileStorageService.Storage.Combine(caminho, canhoto.GuidNomeArquivo + extensao);

                        string dataEmissaoNotaFiscal = null;
                        string numeroNotaFiscal = null;
                        string situacaoEntregaNotaFiscal = null;

                        Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = null;

                        if (canhoto.XMLNotaFiscal != null)
                        {
                            cargaEntrega = repCargaEntrega.BuscarUltimaEntregaConfirmadaPorNotaFiscal(canhoto.XMLNotaFiscal.Codigo);
                            dataEmissaoNotaFiscal = canhoto.XMLNotaFiscal.DataEmissao.ToString("dd/MM/yyyy HH:mm:ss");
                            numeroNotaFiscal = canhoto.XMLNotaFiscal.Numero.ToString();
                            situacaoEntregaNotaFiscal = canhoto.XMLNotaFiscal.SituacaoEntregaNotaFiscal.ObterDescricao();
                        }

                        byte[] bufferCanhoto = null;
                        if (!configuracaoWebService.NaoRetornarImagemCanhoto && Utilidades.IO.FileStorageService.Storage.Exists(fileLocation))
                            bufferCanhoto = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(fileLocation);

                        Dominio.ObjetosDeValor.Embarcador.NFe.Canhoto canhotoNF = new()
                        {
                            NumeroCanhoto = canhoto.Numero,
                            Protocolo = canhoto.Codigo,
                            ChaveAcesso = canhoto.XMLNotaFiscal?.Chave,
                            DataEmissaoNotaFiscal = dataEmissaoNotaFiscal,
                            NumeroNotaFiscal = numeroNotaFiscal,
                            DataEnvioCanhoto = canhoto.DataEnvioCanhoto.ToString("dd/MM/yyyy HH:mm:ss"),
                            SituacaoCanhoto = canhoto.SituacaoCanhoto,
                            SituacaoDigitalizacaoCanhoto = canhoto.SituacaoDigitalizacaoCanhoto,
                            DataDigitalizacao = canhoto.DataDigitalizacao.HasValue ? canhoto.DataDigitalizacao.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty,
                            Observacao = canhoto.Observacao,
                            ImagemCanhotoBase64 = bufferCanhoto != null ? Convert.ToBase64String(bufferCanhoto) : "",
                            NomeImagemCanhoto = canhoto.NomeArquivo,
                            TipoCanhoto = canhoto.TipoCanhoto,
                            Latitude = !string.IsNullOrEmpty(canhoto.Latitude) ? canhoto.Latitude : "",
                            Longitude = !string.IsNullOrEmpty(canhoto.Longitude) ? canhoto.Longitude : "",
                            DataEntregaNota = (canhoto.DataEntregaNotaCliente ?? cargaEntrega?.DataConfirmacao ?? canhoto.DataEnvioCanhoto).ToString("dd/MM/yyyy HH:mm:ss"),
                            CodigoIntegracaoFilial = canhoto.Filial != null ? canhoto.Filial.CodigoFilialEmbarcador : string.Empty,
                            CodigoIntegracaoDestinatario = canhoto.Destinatario != null ? canhoto.Destinatario.CodigoIntegracao : string.Empty,
                            SituacaoNotaFiscal = situacaoEntregaNotaFiscal,
                        };
                        retorno.Objeto.Itens.Add(canhotoNF);
                    }

                    retorno.Objeto.NumeroTotalDeRegistro = repCanhoto.ContarCanhotosNotasFiscaisPendentesIntegracaoDigitalizacao(configuracao.UtilizaPgtoCanhoto, configuracao.RetornarCanhotosViaIntegracaoEmQualquerSituacao, integradora?.Empresa?.Codigo ?? 0, configuracaoQualidadeEntrega);
                    retorno.Status = true;
                    Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou canhotos das notas pendentes de integração da digitalizacao", unitOfWork);
                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "O limite não pode ser maior que 100";
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Status = false;
                retorno.Mensagem = "Ocorreu uma falha ao consultar os canhotos digitalizados";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.NFe.Canhoto>> BuscarCanhotosDigitalizadoseAgAprovacao(int? inicio, int? limite)
        {
            Dominio.Entidades.WebService.Integradora integradora = ValidarToken();

            inicio ??= 0;
            limite ??= 0;

            Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.NFe.Canhoto>> retorno = new Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.NFe.Canhoto>>();
            retorno.Mensagem = "";
            retorno.Objeto = new Paginacao<Dominio.ObjetosDeValor.Embarcador.NFe.Canhoto>();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebService = repConfiguracaoWebService.BuscarConfiguracaoPadrao();

                if (limite <= 100)
                {
                    retorno.Objeto.Itens = new List<Dominio.ObjetosDeValor.Embarcador.NFe.Canhoto>();
                    List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotos = repCanhoto.BuscarCanhotosDigitalizadoseAgAprovacao(
                        configuracao.UtilizaPgtoCanhoto,
                        (int)inicio,
                        (int)limite,
                        integradora.Empresa?.Codigo ?? 0
                    );

                    foreach (Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto in canhotos)
                    {
                        string extensao = System.IO.Path.GetExtension(canhoto.NomeArquivo).ToLower();
                        string caminho = Servicos.Embarcador.Canhotos.Canhoto.CaminhoCanhoto(canhoto, unitOfWork);
                        string fileLocation = Utilidades.IO.FileStorageService.Storage.Combine(caminho, canhoto.GuidNomeArquivo + extensao);

                        byte[] bufferCanhoto = null;
                        if (!configuracaoWebService.NaoRetornarImagemCanhoto && Utilidades.IO.FileStorageService.Storage.Exists(fileLocation))
                            bufferCanhoto = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(fileLocation);

                        Dominio.ObjetosDeValor.Embarcador.NFe.Canhoto canhotoNF = new Dominio.ObjetosDeValor.Embarcador.NFe.Canhoto()
                        {
                            NumeroCanhoto = canhoto.Numero,
                            Protocolo = canhoto.Codigo,
                            ChaveAcesso = canhoto.XMLNotaFiscal?.Chave ?? string.Empty,
                            DataEmissaoNotaFiscal = canhoto.XMLNotaFiscal?.DataEmissao.ToString("dd/MM/yyyy HH:mm:ss"),
                            NumeroNotaFiscal = canhoto.XMLNotaFiscal?.Numero.ToString(),
                            DataEnvioCanhoto = canhoto.DataEnvioCanhoto.ToString("dd/MM/yyyy HH:mm:ss"),
                            SituacaoCanhoto = canhoto.SituacaoCanhoto,
                            SituacaoDigitalizacaoCanhoto = canhoto.SituacaoDigitalizacaoCanhoto,
                            Observacao = canhoto.Observacao,
                            ImagemCanhotoBase64 = bufferCanhoto != null ? Convert.ToBase64String(bufferCanhoto) : "",
                            NomeImagemCanhoto = canhoto.NomeArquivo ?? "",
                            TipoCanhoto = canhoto.TipoCanhoto,
                            Latitude = !string.IsNullOrEmpty(canhoto.Latitude) ? canhoto.Latitude : "",
                            Longitude = !string.IsNullOrEmpty(canhoto.Longitude) ? canhoto.Longitude : "",
                            DataEntregaNota = canhoto.DataEntregaNotaCliente.HasValue ? canhoto.DataEntregaNotaCliente.Value.ToString("dd/MM/yyyy HH:mm:ss") : "",
                            CodigoIntegracaoFilial = canhoto.Filial != null ? canhoto.Filial.CodigoFilialEmbarcador : string.Empty,
                            CodigoIntegracaoDestinatario = canhoto.Destinatario != null ? canhoto.Destinatario.CodigoIntegracao : string.Empty,
                        };
                        retorno.Objeto.Itens.Add(canhotoNF);
                    }

                    retorno.Objeto.NumeroTotalDeRegistro = repCanhoto.ContarCanhotosDigitalizadoseAgAprovacao(configuracao.UtilizaPgtoCanhoto, integradora.Empresa?.Codigo ?? 0);
                    retorno.Status = true;
                    Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou canhotos das notas pendentes de integração da digitalizacao", unitOfWork);
                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "O limite não pode ser maior que 100";
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Status = false;
                retorno.Mensagem = "Ocorreu uma falha ao consultar os canhotos digitalizados";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<bool> ConfirmarIntegracaoDigitalizacaoCanhotoNotasFiscais(int protocolo)
        {
            ValidarToken();

            Retorno<bool> retorno = new Retorno<bool>();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
            try
            {
                Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = repCanhoto.BuscarPorCodigo(protocolo, true);

                if (canhoto != null)
                {
                    if (!canhoto.DigitalizacaoIntegrada)
                    {
                        canhoto.DigitalizacaoIntegrada = true;
                        repCanhoto.Atualizar(canhoto);
                        retorno.Objeto = true;
                        retorno.Status = true;
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, canhoto, "Confirmou integração da digitalização.", unitOfWork);
                    }
                    else
                    {
                        Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                        Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                        if (!configuracaoTMS.RetornosDuplicidadeWSSubstituirPorSucesso)
                        {
                            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DuplicidadeDaRequisicao;
                            retorno.Status = true;
                            retorno.Mensagem = "A confirmação da integração já foi realizada anteriormente.";
                        }
                        else
                        {
                            retorno.Objeto = true;
                            retorno.Status = true;
                        }
                    }
                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "Não foi encontrado um canhoto para o protocolo informado";
                }
            }
            catch (Exception ex)
            {
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                retorno.Mensagem = "A carga informada não existe no Multi Embarcador";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;

        }

        public Retorno<bool> EnvioNFeColeta(string numeroOS, string numeroContainer, decimal? taraContainer, List<string> listaLacres, List<string> listaChaves)
        {
            ValidarToken();

            Retorno<bool> retorno = new Retorno<bool>();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                if (!string.IsNullOrWhiteSpace(numeroOS) && !string.IsNullOrWhiteSpace(numeroContainer) && listaChaves != null && listaChaves.Count > 0 && listaLacres != null && listaLacres.Count > 0)
                {
                    Servicos.Log.TratarErro(
                        (" Número da OS: " + (!string.IsNullOrWhiteSpace(numeroOS) ? numeroOS : " Não informado")) +
                        (" Número do Container: " + (!string.IsNullOrWhiteSpace(numeroContainer) ? numeroContainer : " Não informado")) +
                        (" Tara: " + (taraContainer > 0 ? taraContainer.Value.ToString("n2") : " Não informado")), "EnvioNFeColeta");

                    Repositorio.Embarcador.Pedidos.Container repContainer = new Repositorio.Embarcador.Pedidos.Container(unitOfWork);
                    Repositorio.Embarcador.Pedidos.ColetaNotaFiscal repColetaNotaFiscal = new Repositorio.Embarcador.Pedidos.ColetaNotaFiscal(unitOfWork);
                    Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
                    Servicos.Embarcador.Documentos.ConsultaDocumento srvConsultaDocumento = new Servicos.Embarcador.Documentos.ConsultaDocumento(unitOfWork, TipoServicoMultisoftware, Auditado);
                    Servicos.WebService.Carga.Pedido serPedidoWS = new Servicos.WebService.Carga.Pedido(unitOfWork);

                    Dominio.Entidades.Embarcador.Pedidos.Container container = repContainer.BuscarPorNumero(numeroContainer);
                    if (container == null)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Carga.Container containerIntegracao = new Dominio.ObjetosDeValor.Embarcador.Carga.Container()
                        {
                            Atualizar = false,
                            CodigoIntegracao = 0,
                            Descricao = numeroContainer,
                            InativarCadastro = false,
                            Lacre1 = listaLacres != null && listaLacres.Count > 0 && listaLacres.Count > 1 ? listaLacres[0] : "",
                            Lacre2 = listaLacres != null && listaLacres.Count > 0 && listaLacres.Count > 2 ? listaLacres[1] : "",
                            Lacre3 = listaLacres != null && listaLacres.Count > 0 && listaLacres.Count > 3 ? listaLacres[3] : "",
                            Numero = numeroContainer,
                            PesoLiquido = 0m,
                            Tara = (int)taraContainer,
                            TipoContainer = null,
                            TipoPropriedade = TipoPropriedadeContainer.Nenhum,
                            DencidadeProduto = 0m,
                            Volume = 0m
                        };
                        StringBuilder stMensagem = new StringBuilder();
                        container = serPedidoWS.SalvarContainer(containerIntegracao, ref stMensagem, Auditado);
                    }

                    Dominio.Entidades.Embarcador.Pedidos.ColetaNotaFiscal coletaNotaFiscal = new Dominio.Entidades.Embarcador.Pedidos.ColetaNotaFiscal()
                    {
                        ColetaProcessada = false,
                        NumeroContainer = numeroContainer,
                        NumeroOS = numeroOS,
                        Container = container,
                        TaraContainer = (decimal)taraContainer
                    };

                    coletaNotaFiscal.Chaves = new List<string>();
                    coletaNotaFiscal.Lacres = new List<string>();
                    coletaNotaFiscal.Notas = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();

                    Servicos.Log.TratarErro("Chaves: " + string.Join(", ", listaChaves.ToList()), "EnvioNFeColeta");
                    Servicos.Log.TratarErro("Lacres: " + string.Join(", ", listaLacres.ToList()), "EnvioNFeColeta");

                    foreach (string chave in listaChaves)
                    {
                        if (string.IsNullOrWhiteSpace(chave) || !Utilidades.Validate.ValidarChaveNFe(Utilidades.String.OnlyNumbers(chave)))
                        {
                            Servicos.Log.TratarErro("A chave informada (" + chave + ") está inválida.", "EnvioNFeColeta");
                            retorno.Status = false;
                            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                            retorno.Mensagem = "A chave informada (" + chave + ") está inválida.";
                            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                            return retorno;
                        }
                        else
                            coletaNotaFiscal.Chaves.Add(Utilidades.String.OnlyNumbers(chave));
                    }
                    foreach (string lacre in listaLacres)
                    {
                        if (string.IsNullOrWhiteSpace(lacre))
                        {
                            Servicos.Log.TratarErro("O lacre informado (" + lacre + ") está inválido.", "EnvioNFeColeta");
                            retorno.Status = false;
                            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                            retorno.Mensagem = "O lacre informado (" + lacre + ") está inválido.";
                            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                            return retorno;
                        }
                        else
                            coletaNotaFiscal.Lacres.Add(lacre);
                    }

                    unitOfWork.Start();

                    foreach (string chave in coletaNotaFiscal.Chaves)
                    {
                        Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = null;

                        xmlNotaFiscal = srvConsultaDocumento.ObterNotaFiscal(chave);

                        if (xmlNotaFiscal == null)
                            xmlNotaFiscal = srvConsultaDocumento.ObterNotaFiscalPorDocumentoDestinado(chave, TipoServicoMultisoftware);

                        if (xmlNotaFiscal == null)
                        {
                            try
                            {
                                xmlNotaFiscal = srvConsultaDocumento.ObterNotaFiscalPorSerpro(chave);
                            }
                            catch (Exception ex)
                            {
                                xmlNotaFiscal = null;
                                Servicos.Log.TratarErro(ex, "EnvioNFeColeta");
                            }
                        }

                        if (xmlNotaFiscal == null)
                        {
                            Servicos.Log.TratarErro("A chave informada (" + chave + ") não foi localizada na base.", "EnvioNFeColeta");
                            retorno.Status = false;
                            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                            retorno.Mensagem = "Falha parcial. A interface foi recebida, porém a NF (XMLs) (" + chave + ") não estão na base do MTMS.";
                            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

                            unitOfWork.Rollback();

                            return retorno;
                        }
                        else
                        {
                            xmlNotaFiscal.FormaIntegracao = FormaIntegracao.OKColeta;
                            repXMLNotaFiscal.Atualizar(xmlNotaFiscal);

                            coletaNotaFiscal.Notas.Add(xmlNotaFiscal);
                        }
                    }

                    repColetaNotaFiscal.Inserir(coletaNotaFiscal);

                    retorno.Objeto = true;
                    retorno.Status = true;
                    retorno.Mensagem = "Sucesso. A interface foi recebida e todas as NFs (XMLs) estão na base do MTMS.";
                    Servicos.Log.TratarErro("Registro iserido com sucesso.", "EnvioNFeColeta");

                    unitOfWork.CommitChanges();
                }
                else
                {
                    Servicos.Log.TratarErro("Campos obrigatórios não informados, favor verifique na documentação disponível.", "EnvioNFeColeta");
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "Campos obrigatórios não informados, favor verifique na documentação disponível.";
                }
            }
            catch (Exception ex)
            {
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex, "EnvioNFeColeta");
                retorno.Mensagem = "Outras falhas. A interface não foi recebida com sucesso.";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<string> EnviarXMLNotaFiscal(Stream arquivo)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<string>.CreateFrom(new Servicos.WebService.NFe.NotaFiscal(unitOfWork, TipoServicoMultisoftware, Auditado, Cliente).EnviarXMLNotaFiscal(arquivo));
            });
        }

        #endregion

        #region Métodos de Requisição

        public Retorno<bool> ConfirmarRecebimentoNotaFiscalComRecebimentoConfirmadoNoDestinatario(int protocoloNFe)
        {

            ValidarToken();

            Retorno<bool> retorno = new Retorno<bool>();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            retorno.Status = true;
            try
            {
                Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = repXMLNotaFiscal.BuscarPorCodigo(protocoloNFe);
                if (xmlNotaFiscal != null)
                {
                    if (!xmlNotaFiscal.RetornoNotaIntegrada)
                    {
                        xmlNotaFiscal.RetornoNotaIntegrada = true;
                        repXMLNotaFiscal.Atualizar(xmlNotaFiscal);
                        retorno.Objeto = true;

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, xmlNotaFiscal, "Confirmou recebimento de Nota fiscal com recebimento confirmado no destinatário", unitOfWork);
                    }
                    else
                    {
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DuplicidadeDaRequisicao;
                        retorno.Mensagem = "A confirmação do recebimento da nota fiscal recebida no destinatario já foi feito.";
                    }
                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "O protocólo da nota informada não existe na base da Multisoftware";
                }
            }
            catch (Exception ex)
            {
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                Servicos.Log.TratarErro(ex);
                retorno.Mensagem = "Ocorreu uma falha ao tentar enviar a confirmação do recebimento das notas fiscais com recebimento confirmado do destinatário.";
            }
            finally
            {
                unitOfWork.Dispose();
            }
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<bool> ConfirmarRecebimentoCargaAguardandoNotasFiscais(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo)
        {

            ValidarToken();

            Retorno<bool> retorno = new Retorno<bool>();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                unitOfWork.Start();
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                //Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCargaEPedido(protocolo.protocoloIntegracaoCarga, protocolo.protocoloIntegracaoPedido);
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorProtocoloCargaOrigemEProtocoloPedido(protocolo.protocoloIntegracaoCarga, protocolo.protocoloIntegracaoPedido);

                if (cargaPedido != null)
                {
                    if (!cargaPedido.CienciaDoEnvioDaNotaInformado)
                    {
                        cargaPedido.CienciaDoEnvioDaNotaInformado = true;
                        repCargaPedido.Atualizar(cargaPedido);
                        retorno.Objeto = true;
                        retorno.Status = true;

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaPedido, "Confirmou recebimento de carga aguardando notas fiscais", unitOfWork);

                        unitOfWork.CommitChanges();
                    }
                    else
                    {
                        unitOfWork.Rollback();
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DuplicidadeDaRequisicao;
                        retorno.Mensagem = "A confirmação do recebimento da carga aguardando Notas Fiscais já foi feito.";
                    }
                }
                else
                {
                    if (protocolo.protocoloIntegracaoCarga > 0 && protocolo.protocoloIntegracaoPedido == 0)
                    {
                        List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedidos = repCargaPedido.BuscarPorCargaOrigem(protocolo.protocoloIntegracaoCarga);
                        if (cargasPedidos != null && cargasPedidos.Count > 0)
                        {
                            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPed in cargasPedidos)
                            {
                                cargaPed.CienciaDoEnvioDaNotaInformado = true;
                                repCargaPedido.Atualizar(cargaPed);
                                retorno.Objeto = true;
                                retorno.Status = true;
                                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaPed, "Confirmou recebimento de carga aguardando notas fiscais", unitOfWork);
                            }

                            unitOfWork.CommitChanges();
                        }
                        else
                        {
                            unitOfWork.Rollback();
                            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                            retorno.Mensagem = "Os protocolos informados informada não existe no Multi Embarcador";
                        }
                    }
                    else
                    {
                        unitOfWork.Rollback();
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Mensagem = "Os protocolos informados informada não existe no Multi Embarcador";
                    }

                }
            }
            catch (Exception ex)
            {
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                retorno.Mensagem = "Ocorreu uma falha ao tentar enviar a confirmação do recebimento das Carga Aguardando Notas Fiscais";
            }
            finally
            {
                unitOfWork.Dispose();
            }
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<bool> ExcluirNotaFiscalPorChave(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo, string chaveNFe)
        {
            Dominio.Entidades.WebService.Integradora integradora = ValidarToken();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            Retorno<bool> retorno = new Retorno<bool>();
            List<string> caminhosXMLTemp = new List<string>();
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            retorno.Mensagem = "";
            try
            {
                if (protocolo.protocoloIntegracaoCarga > 0 || protocolo.protocoloIntegracaoPedido > 0)
                {
                    Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                    Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                    Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

                    Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);

                    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorProtocoloCargaOrigemEProtocoloPedido(protocolo.protocoloIntegracaoCarga, protocolo.protocoloIntegracaoPedido);
                    Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xMLNotaFiscal = repXMLNotaFiscal.BuscarPorChave(chaveNFe);
                    if (xMLNotaFiscal != null)
                    {
                        if (cargaPedido != null)
                        {
                            if (xMLNotaFiscal != null)
                            {
                                unitOfWork.Start();
                                string retornoIntegracao = Servicos.WebService.NFe.NotaFiscal.ExcluirNotaFiscal(xMLNotaFiscal, cargaPedido, Auditado, TipoServicoMultisoftware, unitOfWork);
                                if (string.IsNullOrWhiteSpace(retornoIntegracao))
                                {
                                    retorno.Objeto = true;
                                    retorno.Status = true;
                                    unitOfWork.CommitChanges();
                                }
                                else
                                {
                                    retorno.Objeto = true;
                                    retorno.Status = false;
                                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                                    unitOfWork.Rollback();
                                }
                            }
                            else
                            {
                                retorno.Status = false;
                                retorno.Mensagem = "Não foi localizada a nota fiscal. ";
                                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                            }
                        }
                        else
                        {
                            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repPedido.BuscarPorCodigo(protocolo.protocoloIntegracaoPedido);
                            if (pedido != null)
                            {
                                if (pedido.NotasFiscais.Count > 0)
                                {
                                    if (pedido.NotasFiscais.Contains(xMLNotaFiscal))
                                    {
                                        pedido.NotasFiscais.Remove(xMLNotaFiscal);
                                        repPedido.Atualizar(pedido);
                                    }
                                    else
                                    {
                                        retorno.Status = false;
                                        retorno.Mensagem = "Protocolos informados são inválidos. ";
                                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                                    }
                                }
                                else
                                {
                                    retorno.Status = false;
                                    retorno.Mensagem = "Protocolos informados são inválidos. ";
                                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                                }
                            }
                            else
                            {
                                retorno.Status = false;
                                retorno.Mensagem = "Protocolos informados são inválidos. ";
                                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                            }
                        }
                    }
                    else
                    {
                        retorno.Status = false;
                        retorno.Mensagem = "Protocolos informados são inválidos. ";
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    }
                }
                else
                {
                    retorno.Status = false;
                    retorno.Mensagem = "Nota Fiscal não encontrada. ";
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                }

            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                retorno.Mensagem = excecao.Message;
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao excluir a Nota Fiscal";
            }
            finally
            {
                unitOfWork.Dispose();
            }
            return retorno;
        }

        public Retorno<bool> ExcluirNotaFiscal(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo, int? protocoloNotaFiscal)
        {
            Dominio.Entidades.WebService.Integradora integradora = ValidarToken();

            protocoloNotaFiscal ??= 0;

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            Retorno<bool> retorno = new Retorno<bool>();
            List<string> caminhosXMLTemp = new List<string>();
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            retorno.Mensagem = "";
            try
            {
                if (protocolo.protocoloIntegracaoCarga > 0 || protocolo.protocoloIntegracaoPedido > 0)
                {
                    Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                    Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);

                    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorProtocoloCargaOrigemEProtocoloPedido(protocolo.protocoloIntegracaoCarga, protocolo.protocoloIntegracaoPedido);

                    if (cargaPedido != null)
                    {
                        Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xMLNotaFiscal = repXMLNotaFiscal.BuscarPorCodigo((int)protocoloNotaFiscal);

                        if (xMLNotaFiscal != null)
                        {
                            unitOfWork.Start();
                            string retornoIntegracao = Servicos.WebService.NFe.NotaFiscal.ExcluirNotaFiscal(xMLNotaFiscal, cargaPedido, Auditado, TipoServicoMultisoftware, unitOfWork);
                            if (string.IsNullOrWhiteSpace(retornoIntegracao))
                            {
                                retorno.Objeto = true;
                                retorno.Status = true;
                                unitOfWork.CommitChanges();
                            }
                            else
                            {
                                retorno.Objeto = true;
                                retorno.Status = false;
                                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                                unitOfWork.Rollback();
                            }
                        }
                        else
                        {
                            retorno.Status = false;
                            retorno.Mensagem = "Não foi localizada a nota fiscal. ";
                            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        }
                    }
                    else
                    {
                        retorno.Status = false;
                        retorno.Mensagem = "Protocolos informados são inválidos. ";
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;

                    }
                }
                else
                {
                    retorno.Status = false;
                    retorno.Mensagem = "Não foi localizado o pedido. ";
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                }

            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                retorno.Mensagem = excecao.Message;
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao excluir a Nota Fiscal";
            }
            finally
            {
                unitOfWork.Dispose();
            }
            return retorno;
        }

        public Retorno<bool> RemoverNotaFiscal(Dominio.ObjetosDeValor.WebService.Carga.NotaFiscal notaFiscal)
        {
            Dominio.Entidades.WebService.Integradora integradora = ValidarToken();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Servicos.Embarcador.Pedido.OcorrenciaPedido servOcorrenciaPedido = new Servicos.Embarcador.Pedido.OcorrenciaPedido(unitOfWork);

            Retorno<bool> retorno = new Retorno<bool>();
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            retorno.Mensagem = "";
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xMLNotaFiscal = repXMLNotaFiscal.BuscarPorChave(notaFiscal.Chave);

                if (string.IsNullOrEmpty(notaFiscal.Chave))
                    throw new WebServiceException($"Chave da nota não enviada");

                if (xMLNotaFiscal == null)
                    throw new WebServiceException($"Nota não encontrada");


                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();

                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> listaPedidos = repPedido.BuscarPorNotaFiscal(xMLNotaFiscal.Codigo);
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> listaPedidoXmlNotaFiscal = repPedidoXMLNotaFiscal.BuscarPorNotaFiscal(xMLNotaFiscal.Codigo);

                // Remove de todas cargas
                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXmlNotaFiscal in listaPedidoXmlNotaFiscal)
                {
                    if (pedidoXmlNotaFiscal.CargaPedido == null)
                    {
                        continue;
                    }

                    if (pedidoXmlNotaFiscal.CargaPedido.Carga.SituacaoCarga != SituacaoCarga.AgNFe)
                    {
                        throw new WebServiceException($"Erro na carga {pedidoXmlNotaFiscal.CargaPedido.Carga.CodigoCargaEmbarcador}: Notas só podem ser removidas de cargas na situação Aguardando NFes");
                    }

                    string retornoIntegracao = Servicos.WebService.NFe.NotaFiscal.ExcluirNotaFiscal(xMLNotaFiscal, pedidoXmlNotaFiscal.CargaPedido, Auditado, TipoServicoMultisoftware, unitOfWork);

                    if (retornoIntegracao != "")
                    {
                        throw new WebServiceException($"Erro na carga {pedidoXmlNotaFiscal.CargaPedido.Carga.CodigoCargaEmbarcador}: " + retornoIntegracao);
                    }
                }

                // Remove de todos os pedidos
                foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in listaPedidos)
                {
                    pedido.NotasFiscais.Remove(xMLNotaFiscal);
                    servOcorrenciaPedido.ProcessarOcorrenciaPedido(EventoColetaEntrega.FaturamentoCancelado, pedido, configuracao, Cliente);
                }

                unitOfWork.CommitChanges();
                retorno.Objeto = true;
                retorno.Status = true;
            }
            catch (BaseException ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = ex.Message;
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao remover a Nota Fiscal";
            }
            finally
            {
                unitOfWork.Dispose();
            }
            return retorno;
        }

        public Retorno<bool> InformarNotasFiscaisPorNumeroControle(List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal> notasFiscais, string numeroControle)
        {
            Servicos.Log.TratarErro($"InformarNotasFiscaisPorNumeroControle - notasFiscais {(notasFiscais != null ? Newtonsoft.Json.JsonConvert.SerializeObject(notasFiscais) : string.Empty)}", "Request");
            Servicos.Log.TratarErro($"InformarNotasFiscaisPorNumeroControle - numeroControle: " + numeroControle);

            Dominio.Entidades.WebService.Integradora integradora = ValidarToken();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();

            Retorno<bool> retorno = new Retorno<bool>
            {
                DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                Mensagem = ""
            };

            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosCompativeis = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();
                if (notasFiscais.Count == 0)
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "Não foi informado notas fiscais.";
                }
                else
                {
                    cargaPedidos = repCargaPedido.BuscarPorNumeroControlePedido(numeroControle);

                    List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga> situacaoCargaPermitidaAdicionarNotas = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga>() {
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.ProntoTransporte,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.LiberadoPagamento ,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgIntegracao,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransbordo,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada,
                    };

                    if (cargaPedidos.Count == 0)
                    {
                        retorno.Status = false;
                        retorno.CodigoMensagem = CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Mensagem = "Não foi localizado carga para o protocolo de pedido (" + numeroControle + ") informado.";
                    }
                    else
                    {

                        cargas = cargaPedidos.Select(obj => obj.Carga).ToList();

                        if (cargas.Any(obj => !situacaoCargaPermitidaAdicionarNotas.Contains(obj.SituacaoCarga)))
                        {
                            retorno.Status = false;
                            retorno.CodigoMensagem = CodigoMensagemRetorno.DadosInvalidos;
                            retorno.Mensagem = "Carga não esta em transporte para aceitar notas. Controle: (" + numeroControle + ").";
                        }
                        else
                        {
                            bool tudocerto = true;
                            retorno.Objeto = true;
                            retorno.Status = true;
                            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.Sucesso;

                            //encontrar pedido correspondente
                            foreach (Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal notaEnviada in notasFiscais)
                            {
                                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoCompativel = cargaPedidos.Where(x => x.Pedido.Destinatario.CPF_CNPJ_Formatado == notaEnviada.Destinatario.CPFCNPJ && x.Pedido.Remetente.CPF_CNPJ_Formatado == notaEnviada.Emitente.CPFCNPJ).FirstOrDefault();
                                if (cargaPedidoCompativel == null)
                                {
                                    retorno.CodigoMensagem = CodigoMensagemRetorno.DadosInvalidos;
                                    retorno.Mensagem = $"Não foi localizado um pedido compatível com a nota fiscal " + notaEnviada.Numero + "  para o número de controle informado. ";

                                    tudocerto = false;
                                    Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaPedidos.Select(x => x.Carga).FirstOrDefault();
                                    Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, retorno.Mensagem, unitOfWork);

                                    break;
                                }
                                else
                                    cargaPedidosCompativeis.Add(cargaPedidoCompativel);
                            }

                            unitOfWork.CommitChanges();

                            if (tudocerto)
                            {
                                unitOfWork.Start();

                                Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaPedidosCompativeis.Select(x => x.Carga).FirstOrDefault();

                                //obter pedidos que nao foram encontradas notas compativeis remover da carga e gravar auditoria;
                                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosNaoEncontradosEmNotas = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>(); // cargaPedidos.Where(c => cargaPedidosCompativeis.Any(d => c.Pedido.Codigo != d.Pedido.Codigo)).ToList();

                                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoNumeroControle in cargaPedidos)
                                {
                                    bool encontrou = false;
                                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargapedidocompativel in cargaPedidosCompativeis)
                                    {
                                        if (cargaPedidoNumeroControle.Codigo == cargapedidocompativel.Codigo)
                                        {
                                            encontrou = true;
                                            break;
                                        }
                                    }

                                    if (!encontrou)
                                        cargaPedidosNaoEncontradosEmNotas.Add(cargaPedidoNumeroControle);
                                }

                                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargapedidoNaoEncotrado in cargaPedidosNaoEncontradosEmNotas)
                                {
                                    Servicos.Embarcador.Carga.CargaPedido.RemoverPedidoCarga(cargapedidoNaoEncotrado.Carga, cargapedidoNaoEncotrado, configuracao, TipoServicoMultisoftware, unitOfWork, configuracaoGeralCarga);
                                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cargapedidoNaoEncotrado.Carga, null, $"Removeu o pedido {cargapedidoNaoEncotrado.Pedido.NumeroPedidoEmbarcador} por não receber notas correspondentes via integração", unitOfWork);
                                }

                                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargapedidoCompativel in cargaPedidosCompativeis)
                                {
                                    List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal> listaNotasPedido = notasFiscais.Where(x => x.Destinatario.CPFCNPJ == cargapedidoCompativel.Pedido.Destinatario.CPF_CNPJ_Formatado && x.Emitente.CPFCNPJ == cargapedidoCompativel.Pedido.Remetente.CPF_CNPJ_Formatado).ToList();

                                    string ret = Servicos.WebService.NFe.NotaFiscal.IntegrarNotaFiscal(cargapedidoCompativel, listaNotasPedido, null, null, configuracao, TipoServicoMultisoftware, Auditado, integradora, unitOfWork, numeroControle);

                                    if (string.IsNullOrEmpty(ret))
                                        Servicos.Auditoria.Auditoria.Auditar(Auditado, cargapedidoCompativel.Carga, null, $"Integrou notas fiscais com sucesso via integração", unitOfWork);
                                    else
                                    {
                                        retorno.Status = false;
                                        retorno.CodigoMensagem = CodigoMensagemRetorno.FalhaGenerica;
                                        retorno.Mensagem = ret;
                                        break;
                                    }
                                }

                                if (retorno.Status == true)
                                {
                                    //criar entidade fechamento carga
                                    Servicos.Embarcador.Carga.CargaFechamento servCargaFechamento = new Servicos.Embarcador.Carga.CargaFechamento(unitOfWork);
                                    servCargaFechamento.GerarCargaFechamento(carga);
                                }

                                if (retorno.Status == true)
                                    unitOfWork.CommitChanges();
                                else
                                    unitOfWork.Rollback();

                            }
                            else
                                retorno.Status = false;
                        }
                    }
                }

            }
            catch (ServicoException ex)
            {
                unitOfWork.Rollback();
                retorno.Status = false;
                retorno.CodigoMensagem = CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = ex.Message;
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao enviar a(s) NF-e(s)";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            return retorno;
        }

        public Retorno<bool> InformarDadosContabeisNotaFiscal(List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal> notasFiscais)
        {
            Servicos.Log.TratarErro($"InformarDadosContabeisNotaFiscal - notasFiscais {(notasFiscais != null ? Newtonsoft.Json.JsonConvert.SerializeObject(notasFiscais) : string.Empty)}", "Request");

            Dominio.Entidades.WebService.Integradora integradora = ValidarToken();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            Retorno<bool> retorno = new Retorno<bool>
            {
                DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                Mensagem = ""
            };

            try
            {
                unitOfWork.Start();
                Servicos.WebService.NFe.NotaFiscal.IntegrarNotaFiscal(notasFiscais, TipoServicoMultisoftware, Auditado, unitOfWork);
                unitOfWork.CommitChanges();

                retorno.Status = true;
                retorno.CodigoMensagem = CodigoMensagemRetorno.Sucesso;
            }
            catch (ServicoException ex)
            {
                unitOfWork.Rollback();
                retorno.Status = false;
                retorno.CodigoMensagem = CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = ex.Message;
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao enviar a(s) NF-e(s)";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            return retorno;
        }

        public Retorno<bool> IntegrarDadosNotasFiscais(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo, List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal> listaNotasFiscais, Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracaoCompleta cargaIntegracaoCompleta)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.WebService.NFe.NotaFiscal(unitOfWork, TipoServicoMultisoftware, Auditado, Cliente).IntegrarDadosNotasFiscais(protocolo, listaNotasFiscais, integradora, cargaIntegracaoCompleta));
            });
        }

        public Retorno<bool> IntegrarNotasFiscais(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo, List<Dominio.ObjetosDeValor.Embarcador.NFe.TokenNF> TokensXMLNotasFiscais)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.WebService.NFe.NotaFiscal(unitOfWork, TipoServicoMultisoftware, Auditado, Cliente).IntegrarNotasFiscais(protocolo, TokensXMLNotasFiscais, integradora));
            });
        }

        public Retorno<bool> IntegrarNotasFiscaisComAverbacaoeValePedagio(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo, List<Dominio.ObjetosDeValor.Embarcador.NFe.TokenNF> TokensXMLNotasFiscais, Dominio.ObjetosDeValor.Embarcador.Carga.Averbacao Averbacao, Dominio.ObjetosDeValor.MDFe.ValePedagio ValePedagio, Dominio.ObjetosDeValor.MDFe.CIOT Ciot, Dominio.ObjetosDeValor.MDFe.InformacoesPagamentoPedido informacoesPagamento)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.WebService.NFe.NotaFiscal(unitOfWork, TipoServicoMultisoftware, Auditado, Cliente).IntegrarNotasFiscaisComAverbacaoeValePedagio(protocolo, TokensXMLNotasFiscais, Averbacao, ValePedagio, integradora, Ciot, informacoesPagamento));
            });
        }

        public Retorno<int> IntegrarNotaFiscal(Dominio.ObjetosDeValor.Embarcador.NFe.TokenNF tokenXML)
        {
            ValidarToken();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            Retorno<int> retorno = new Retorno<int>();
            List<string> caminhosXMLTemp = new List<string>();
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            retorno.Mensagem = "";

            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFisca = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
                Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();

                Servicos.Embarcador.Canhotos.Canhoto serCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(unitOfWork);
                Servicos.Embarcador.NFe.NFe serNFe = new Servicos.Embarcador.NFe.NFe(unitOfWork);
                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                string path = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivosIntegracao;

                string caminho = Utilidades.IO.FileStorageService.Storage.Combine(path, string.Concat(tokenXML.Token, ".xml"));

                if (Utilidades.IO.FileStorageService.Storage.Exists(caminho))
                {
                    System.IO.StreamReader reader = new StreamReader(Utilidades.IO.FileStorageService.Storage.OpenRead(caminho));
                    Servicos.NFe svcNFe = new Servicos.NFe(unitOfWork);

                    Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(unitOfWork);

                    Dominio.ObjetosDeValor.Embarcador.NotaFiscal.DadosNFe nfXml = null;
                    Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebservice = repConfiguracaoWebService.BuscarConfiguracaoPadrao();

                    try
                    {
                        nfXml = svcNFe.ObterDocumentoPorXML(reader.BaseStream, unitOfWork);
                    }
                    catch (Exception)
                    {
                        retorno.Mensagem = "O xml enviado não é de uma nota fiscal autorizada.";
                        retorno.Status = false;
                        Servicos.Log.TratarErro("O xml enviado não é de uma nota fiscal autorizada, avulsa.");
                        reader.Dispose();
                    }

                    if (nfXml != null)
                    {
                        Servicos.Log.TratarErro("IntegrarNotaFiscal - Chave: " + nfXml.Chave);

                        if (!serNFe.BuscarDadosNotaFiscal(out string erro, out Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal, reader, unitOfWork, nfXml, true, false, false, TipoServicoMultisoftware, configuracao.ImportarEmailCliente, configuracao.UtilizarValorFreteNota, null, null, null, configuracaoWebservice?.CadastroAutomaticoPessoaExterior ?? false))
                        {
                            unitOfWork.Rollback();

                            retorno.Status = false;
                            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                            retorno.Mensagem += erro;

                            return retorno;
                        }

                        Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscalExiste = repXMLNotaFisca.BuscarPorChave(xmlNotaFiscal.Chave);

                        if (xmlNotaFiscalExiste != null)
                        {
                            retorno.Objeto = xmlNotaFiscalExiste.Codigo;
                            retorno.Status = false;
                            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DuplicidadeDaRequisicao;
                            retorno.Mensagem += "Esta nota já foi integrada com a Multisoftware.";

                            if (retorno.Status)
                                Utilidades.IO.FileStorageService.Storage.Delete(caminho);

                            unitOfWork.Rollback();

                            return retorno;
                        }

                        xmlNotaFiscal.SemCarga = true;
                        if (configuracao.NotaUnicaEmCargas)
                        {
                            List<string> chaves = new List<string>();
                            chaves.Add(xmlNotaFiscal.Chave);

                            List<int> numerosExistentes = repXMLNotaFisca.BuscarNotasAtivasPorChave(chaves, ignorarReentrega: true);

                            if (numerosExistentes.Count > 0)
                            {
                                xmlNotaFiscal.SemCarga = false;

                                List<string> numerosCargas = repXMLNotaFisca.BuscarCargasAtivasPorChave(chaves, ignorarReentrega: true);
                                Servicos.Log.TratarErro($"As notas fiscais ({string.Join(", ", numerosExistentes.ToList())}) já estão vinculadas a outras cargas ({string.Join(", ", numerosCargas.ToList())})");
                            }
                        }

                        xmlNotaFiscal.TipoNotaFiscalIntegrada = tokenXML.TipoNotaFiscalIntegrada;

                        if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                        {
                            xmlNotaFiscal.Filial = repFilial.BuscarPorCNPJ(xmlNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida ? xmlNotaFiscal.Emitente.CPF_CNPJ_SemFormato : xmlNotaFiscal.Destinatario.CPF_CNPJ_SemFormato);
                        }

                        repXMLNotaFisca.Inserir(xmlNotaFiscal);

                        if (tokenXML.TipoNotaFiscalIntegrada == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscalIntegrada.RemessaPallet)
                        {
                            if (xmlNotaFiscal.Empresa == null)
                            {
                                retorno.Status = false;
                                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                                if (!string.IsNullOrWhiteSpace(xmlNotaFiscal.CNPJTranposrtador))
                                    retorno.Mensagem += "Transportador não encontrado (CNPJ: " + xmlNotaFiscal.CNPJTranposrtador + "). O transportador é necessário para adicionar uma nota fiscal de remessa de pallet.";
                                else
                                    retorno.Mensagem += "As notas de remessa de pallet devem possuir o CNPJ do transportador informado em seu XML para serem integradas.";

                                unitOfWork.Rollback();

                                if (retorno.Status)
                                    Utilidades.IO.FileStorageService.Storage.Delete(caminho);

                                return retorno;
                            }

                            List<Dominio.ObjetosDeValor.Embarcador.NFe.Produtos> produtosPallet = nfXml.Produtos;
                            if (produtosPallet != null)
                            {
                                decimal quantidadePallets = produtosPallet.Sum(obj => obj.QuantidadeComercial);

                                Dominio.Entidades.Embarcador.Filiais.Filial filial = xmlNotaFiscal.Filial;

                                if (filial == null)
                                    filial = repFilial.BuscarMatriz();

                                new Servicos.Embarcador.Pallets.DevolucaoPallets(unitOfWork).AdicionarPallets(null, xmlNotaFiscal, filial, xmlNotaFiscal.Empresa, (int)quantidadePallets, TipoServicoMultisoftware);
                            }
                        }
                        else
                        {
                            Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto repConfiguracaoCanhoto = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto(unitOfWork);
                            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto configuracaoCanhoto = repConfiguracaoCanhoto.BuscarConfiguracaoPadrao();

                            serCanhoto.SalvarCanhotoNota(xmlNotaFiscal, null, null, new List<Dominio.Entidades.Usuario>(), TipoServicoMultisoftware, configuracao, unitOfWork, configuracaoCanhoto);
                        }

                        retorno.Objeto = xmlNotaFiscal.Codigo;
                        retorno.Status = true;

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, xmlNotaFiscal, "Integrou nota fiscal " + xmlNotaFiscal.Numero, unitOfWork);

                        unitOfWork.CommitChanges();
                    }
                    else
                    {
                        unitOfWork.Rollback();
                    }
                    if (retorno.Status)
                        Utilidades.IO.FileStorageService.Storage.Delete(caminho);
                }
                else
                {
                    retorno.Objeto = 0;
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem += "O Token informado não existe mais físicamente no servidor, por favor, envie o XML da nota novamente e receba um novo token";
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                retorno.Objeto = 0;
                retorno.Status = false;
                retorno.Mensagem = "Ocorreu uma falha ao enviar o xml da Nota";
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
            }
            finally
            {
                unitOfWork.Dispose();
            }
            return retorno;
        }

        public Retorno<bool> InformarCancelamentoNotaFiscal(int protocoloNFe)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.WebService.NFe.NotaFiscal(unitOfWork, TipoServicoMultisoftware, Auditado, Cliente).InformarCancelamentoNotaFiscal(protocoloNFe));
            });
        }

        public Retorno<bool> IntegrarNotaFiscalPorPedido(Dominio.ObjetosDeValor.WebService.Carga.Pedido Pedido, Dominio.ObjetosDeValor.Embarcador.NFe.TokenNF TokenXMLNotaFiscal)
        {
            Servicos.Log.TratarErro("IntegrarNotaFiscalPorPedido - Pedido: " + (Pedido != null ? Newtonsoft.Json.JsonConvert.SerializeObject(Pedido) : string.Empty), "Request");
            Servicos.Log.TratarErro("IntegrarNotaFiscalPorPedido - TokenXMLNotaFiscal: " + (TokenXMLNotaFiscal != null ? Newtonsoft.Json.JsonConvert.SerializeObject(TokenXMLNotaFiscal) : string.Empty), "Request");

            ValidarToken();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            Retorno<bool> retorno = new Retorno<bool>();
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            retorno.Mensagem = "";
            try
            {
                unitOfWork.Start();
                Servicos.Embarcador.Pedido.OcorrenciaPedido servOcorrenciaPedido = new Servicos.Embarcador.Pedido.OcorrenciaPedido(unitOfWork);

                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia repConfiguracaoOCorrencia = new Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoOcorrencia configuracaoOcorrencia = repConfiguracaoOCorrencia.BuscarConfiguracaoPadrao();
                Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebservice = repConfiguracaoWebService.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoEntidade = repPedido.BuscarPorCodigo(Pedido.Protocolo);

                if (pedidoEntidade == null && !string.IsNullOrWhiteSpace(Pedido.NumeroPedido))
                {
                    pedidoEntidade = repPedido.BuscarPorNumeroPedidoEmbarcador(Pedido.NumeroPedido);
                }

                if (pedidoEntidade == null)
                {
                    throw new WebServiceException("Pedido não encontrado");
                }

                string path = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivosIntegracao;
                string caminho = Utilidades.IO.FileStorageService.Storage.Combine(path, string.Concat(TokenXMLNotaFiscal.Token, ".xml"));

                if (!Utilidades.IO.FileStorageService.Storage.Exists(caminho))
                {
                    throw new WebServiceException("Token da nota fiscal inválido");
                }

                StreamReader reader = new StreamReader(Utilidades.IO.FileStorageService.Storage.OpenRead(caminho));
                Servicos.NFe svcNFe = new Servicos.NFe(unitOfWork);
                Servicos.Embarcador.NFe.NFe serNFe = new Servicos.Embarcador.NFe.NFe(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.NotaFiscal.DadosNFe nfXml = null;

                nfXml = svcNFe.ObterDocumentoPorXML(reader.BaseStream, unitOfWork, false);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();

                // Criar a nota através do token que foi passado
                if (!serNFe.BuscarDadosNotaFiscal(out string erro, out Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal, reader, unitOfWork, nfXml, true, false, false, TipoServicoMultisoftware, configuracao.ImportarEmailCliente, configuracao.UtilizarValorFreteNota, null, null, null, configuracaoWebservice?.CadastroAutomaticoPessoaExterior ?? false))
                {
                    throw new WebServiceException("Erro ao criar a Nota fiscal: " + erro);
                }

                if (configuracaoOcorrencia != null && configuracaoOcorrencia.EnviarXMLDANFEClienteOcorrenciaPedido)
                {
                    //vamos salvar o DANFE da nota, para posterior envio por email, caso configurado. (obs nao é possivel fazer isso em um servico)
                    int MesNota = xmlNotaFiscal.DataEmissao.Date.Month;
                    string caminhosalvarDanfe = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoDocumentosFiscaisEmbarcador;

                    string caminhoDANFESalvar = Utilidades.IO.FileStorageService.Storage.Combine(caminhosalvarDanfe, "NFe", "nfeProc", "DANFE", MesNota.ToString());

                    string caminhoDANFE = Utilidades.IO.FileStorageService.Storage.Combine(caminhosalvarDanfe, "NFe", "nfeProc", "DANFE", MesNota.ToString(), xmlNotaFiscal.Chave + ".pdf");

                    Zeus.Embarcador.ZeusNFe.Zeus z = new Zeus.Embarcador.ZeusNFe.Zeus();
                    string problemas = z.GerarDANFENFeDocumentoDestinados(caminho, caminhoDANFE, unitOfWork);

                    if (!string.IsNullOrEmpty(problemas))
                        Servicos.Log.TratarErro(problemas);
                }

                // Adiciona a nota criada no pedido
                if (!pedidoEntidade.NotasFiscais.Contains(xmlNotaFiscal))
                {
                    pedidoEntidade.NotasFiscais.Add(xmlNotaFiscal);
                }
                else
                {
                    throw new WebServiceException("Essa NF já está atrelada ao pedido");
                }

                // Se tiver carga atrelada ao pedido e tiver em ag nota fiscal, atrelar à carga também. Se não, cancelar
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedido = repCargaPedido.BuscarPorPedido(pedidoEntidade.Codigo);

                if (cargasPedido.Count == 0)
                    servOcorrenciaPedido.ProcessarOcorrenciaPedido(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EventoColetaEntrega.PedidoFaturado, pedidoEntidade, configuracao, null);

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargasPedido)
                {
                    if (cargaPedido.Carga.SituacaoCarga == SituacaoCarga.AgNFe || cargaPedido.Carga.SituacaoCarga == SituacaoCarga.Nova)
                    {
                        Servicos.Embarcador.Pedido.NotaFiscal serNotaFislca = new Servicos.Embarcador.Pedido.NotaFiscal(unitOfWork);
                        serNotaFislca.InserirNotaCargaPedido(xmlNotaFiscal, cargaPedido, TipoServicoMultisoftware, TipoNotaFiscal.Venda, configuracao, false, out bool alteradoTipoDeCarga, Auditado);
                    }
                    else
                    {
                        throw new WebServiceException("Esse pedido já está atrelado a uma carga que não pode mais receber notas fiscais");
                    }
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, pedidoEntidade, "Integrou notas fiscais", unitOfWork);

                repPedido.Atualizar(pedidoEntidade);
                unitOfWork.CommitChanges();

                retorno.Objeto = true;
            }
            catch (BaseException ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                ArmazenarLogIntegracao(TokenXMLNotaFiscal, unitOfWork);

                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = ex.Message;
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                ArmazenarLogIntegracao(TokenXMLNotaFiscal, unitOfWork);

                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao integrar a nota fiscal por pedido";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            return retorno;
        }

        public Retorno<bool> SolicitarNotasFiscais(int protocoloCarga)
        {
            ValidarToken();

            Retorno<bool> retorno = new Retorno<bool>();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            retorno.Status = true;
            try
            {
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorProtocolo(protocoloCarga);
                if (carga != null)
                {
                    if (carga.SituacaoCarga == SituacaoCarga.AgTransportador)
                    {
                        Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();

                        Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                        unitOfWork.Start();

                        servicoCarga.SolicitarNotasFiscais(carga, configuracao, TipoServicoMultisoftware, Auditado, unitOfWork);

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, "Solicitou notas fiscais por WebService", unitOfWork);

                        unitOfWork.CommitChanges();

                        retorno.CodigoMensagem = CodigoMensagemRetorno.Sucesso;
                        retorno.Mensagem = string.Empty;
                        retorno.Objeto = true;
                    }
                    else
                    {
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Mensagem = "A situação da carga (" + carga.DescricaoSituacaoCarga + ") não permite solicitar as notas fiscais.";
                    }
                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "O protocolo da carga informada não existe";
                }
            }
            catch (ServicoException es)
            {
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                Servicos.Log.TratarErro(es);
                retorno.Mensagem = es.Message;
            }
            catch (Exception ex)
            {
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                Servicos.Log.TratarErro(ex);
                retorno.Mensagem = "Falha ao solicitar notas fiscais";
            }
            finally
            {
                unitOfWork.Dispose();
            }
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<bool> VincularNotaFiscal(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo, List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscalChave> listaNotasFiscaisChaves)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.WebService.NFe.NotaFiscal(unitOfWork, TipoServicoMultisoftware, Auditado, Cliente).VincularNotaFiscal(protocolo, listaNotasFiscaisChaves, integradora));
            });
        }

        public Retorno<bool> ConfirmarEtapaNFe(Dominio.ObjetosDeValor.Embarcador.NFe.ConfirmarEtapaNFe confirmarEtapaNFe)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(
                    new Servicos.WebService.NFe.NotaFiscal(unitOfWork, TipoServicoMultisoftware, Auditado, Cliente, WebServiceConsultaCTe)
                        .ConfirmarEtapaNFe(confirmarEtapaNFe)
                );
            });
        }

        #endregion

        #region Métodos Privados

        //private void AjustarModalidadePagamentosCargaPedido(ref Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Repositorio.UnitOfWork unitOfWork)
        //{
        //    if (cargaPedido.Pedido.UsarTipoPagamentoNF)
        //    {
        //        Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
        //        Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
        //        Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete? modalidadePagamentoFrete = repPedidoXMLNotaFiscal.BuscarModalidadeDeFretePadraoPorCargaPedido(cargaPedido.Codigo);
        //        if (modalidadePagamentoFrete.HasValue && modalidadePagamentoFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.NaoDefinido)
        //        {
        //            cargaPedido.Pedido.TipoPagamento = (Dominio.Enumeradores.TipoPagamento)modalidadePagamentoFrete;

        //            repPedido.Atualizar(cargaPedido.Pedido);

        //            if (cargaPedido.Pedido.TipoPagamento == Dominio.Enumeradores.TipoPagamento.A_Pagar)
        //                cargaPedido.TipoTomador = Dominio.Enumeradores.TipoTomador.Destinatario;
        //            else if (cargaPedido.Pedido.TipoPagamento == Dominio.Enumeradores.TipoPagamento.A_Pagar)
        //                cargaPedido.TipoTomador = Dominio.Enumeradores.TipoTomador.Remetente;
        //        }


        //    }
        //}

        private string ValidarPesoEValorNFe(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, decimal pesoNaNFs, decimal valorTotalNFs, string NumerosNF, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPedidoProduto repCargaPedidoProduto = new Repositorio.Embarcador.Cargas.CargaPedidoProduto(unitOfWork);

            string retorno = "";
            if (Math.Round(cargaPedido.Peso, 3, MidpointRounding.AwayFromZero) != Math.Round(pesoNaNFs, 3, MidpointRounding.AwayFromZero))
            {
                retorno += "O peso total (" + pesoNaNFs.ToString("n3") + ")  da(s) nota(s) fiscal(s) " + NumerosNF + " é diferente do informado na pedido (" + cargaPedido.Peso.ToString("n3") + ")";
            }

            bool fazerArredondamentoPorItem = true; //todo: em clientes futuros ver se o arredondamento para validação com o valor da nota é por item ou total.
            decimal valorCargaPedido = 0;
            if (fazerArredondamentoPorItem)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidoProdutos = repCargaPedidoProduto.BuscarPorCargaPedido(cargaPedido.Codigo);
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto cargaPedidoProduto in cargaPedidoProdutos)
                {
                    valorCargaPedido += Math.Round((cargaPedidoProduto.ValorUnitarioProduto * cargaPedidoProduto.Quantidade), 2, MidpointRounding.AwayFromZero);
                }
            }
            else
                valorCargaPedido = repCargaPedidoProduto.ObterValorTotalPorCargaPedido(cargaPedido.Codigo);

            //if (somarValorPaletesNotaAoPedido) //todo: ver se remove o valor dos paletes no pedido quando o valor não estiver na nota
            valorCargaPedido += cargaPedido.Pedido.ValorTotalPaletes;

            decimal valorCarga = Math.Round(valorCargaPedido, 2, MidpointRounding.AwayFromZero);
            decimal valorNFs = Math.Round(valorTotalNFs, 2, MidpointRounding.AwayFromZero);

            decimal tolerancia = (decimal)0.01; // todo: ver clientes futuros para setar a tolerancia conforme a sua exigencia.

            if (configuracao != null && configuracao.ValidarValorCargaAoAdicionarNFe)
            {
                if (valorNFs != valorCarga && valorNFs != (valorCarga - tolerancia) && valorNFs != (valorCarga + tolerancia))
                {
                    retorno += "O valor total (" + valorTotalNFs.ToString("n2") + ")  dos itens na(s) nota(s) fiscal(s)  " + NumerosNF + "  é diferente do informado na pedido (" + valorCargaPedido.ToString("n2") + ")";
                }
            }
            return retorno;
        }

        private void registrarDivergenciaNotaIntegracao(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, string divergencia, Repositorio.UnitOfWork unitOfWork)
        {

            try
            {
                Repositorio.Embarcador.Pedidos.PedidoDivergenciaNotaIntegracao repPedidoDivergenciaNotaIntegracao = new Repositorio.Embarcador.Pedidos.PedidoDivergenciaNotaIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.PedidoDivergenciaNotaIntegracao pedidoDivergenciaNotaIntegracao = new Dominio.Entidades.Embarcador.Pedidos.PedidoDivergenciaNotaIntegracao();
                pedidoDivergenciaNotaIntegracao.Pedido = pedido;
                pedidoDivergenciaNotaIntegracao.Divergencia = divergencia;
                pedidoDivergenciaNotaIntegracao.DataDivergencia = DateTime.Now;
                repPedidoDivergenciaNotaIntegracao.Inserir(pedidoDivergenciaNotaIntegracao);
            }
            catch (Exception)
            {
                //enterra a exceção, poís é apenas para nível de informação das integração da prosyst e já está mudando na tirol, por isso quando a prosyst sair, tiramos esse método
            }

        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override Dominio.ObjetosDeValor.Enumerador.OrigemAuditado ObterOrigemAuditado()
        {
            return Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.WebServiceNFe;
        }

        #endregion
    }
}
