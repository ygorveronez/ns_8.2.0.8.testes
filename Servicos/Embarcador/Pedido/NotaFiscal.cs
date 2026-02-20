using AdminMultisoftware.Repositorio;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Repositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Pedido
{
    public class NotaFiscal : ServicoBase
    {
        #region Construtores

        public NotaFiscal(Repositorio.UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken) { }

        #endregion Construtores

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Cargas.CargaPedido CriarCargaPorNotaFiscal(Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xMLNotaFiscal, ref string mensagem, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string stringConexao)
        {
            StringBuilder stMensagem = new StringBuilder();
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaLocaisPrestacao repCargaLocaisPrestacao = new Repositorio.Embarcador.Cargas.CargaLocaisPrestacao(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(_unitOfWork).BuscarConfiguracaoPadrao();

            Servicos.WebService.Carga.Pedido serPedidoWS = new Servicos.WebService.Carga.Pedido(_unitOfWork);
            Servicos.WebService.Carga.Carga serCargaWS = new Servicos.WebService.Carga.Carga(_unitOfWork);
            Servicos.WebService.Carga.ProdutosPedido serProdutoPedidoWS = new Servicos.WebService.Carga.ProdutosPedido(_unitOfWork);
            Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(_unitOfWork);
            Servicos.Embarcador.Carga.CargaLocaisPrestacao serCargaLocaisPrestacao = new Servicos.Embarcador.Carga.CargaLocaisPrestacao(_unitOfWork);
            Servicos.Embarcador.Carga.Rota serCargaRota = new Servicos.Embarcador.Carga.Rota(_unitOfWork);

            int codigoCargaExistente = 0;
            int protocoloPedidoExistente = 0;

            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = null;

            Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao = ConverterXMLNotaEmCargaIntegracao(xMLNotaFiscal, configuracaoTMS.UtilizarValorFreteNota);

            if (string.IsNullOrWhiteSpace(xMLNotaFiscal.CNPJTranposrtador))
            {
                mensagem = "A nota fiscal (" + xMLNotaFiscal.Chave + ") não possui transportador informado ";
                return null;
            }

            Dominio.Entidades.Embarcador.Filiais.Filial filial = repFilial.buscarPorCodigoEmbarcador(cargaIntegracao.Filial?.CodigoIntegracao ?? "");
            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = (cargaIntegracao.TipoOperacao != null) ? repTipoOperacao.BuscarPorCodigoIntegracao(cargaIntegracao.TipoOperacao.CodigoIntegracao) : null;

            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = serPedidoWS.CriarPedido(cargaIntegracao, filial, tipoOperacao, ref stMensagem, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador, ref protocoloPedidoExistente, ref codigoCargaExistente, false);
            if (stMensagem.Length == 0 || protocoloPedidoExistente > 0)
            {
                if (protocoloPedidoExistente == 0)
                    serProdutoPedidoWS.AdicionarProdutosPedido(pedido, configuracaoTMS, cargaIntegracao, ref stMensagem, _unitOfWork);

                cargaPedido = serCargaWS.CriarCarga(pedido, cargaIntegracao, ref protocoloPedidoExistente, ref stMensagem, ref codigoCargaExistente, _unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador, false, false, null, configuracaoTMS, null, "", filial, tipoOperacao);
                int codCarga = cargaPedido != null ? cargaPedido.Carga.Codigo : 0;

                List<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao> cargaLocaisPrestacao = codCarga > 0 ? repCargaLocaisPrestacao.BuscarPorCarga(codCarga) : new List<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao>();

                if (cargaPedido != null)
                {
                    if (!cargaPedido.Carga.SituacaoCarga.IsSituacaoCargaNaoEmitida())
                        throw new ServicoException($"A situação atual da carga ({cargaPedido.Carga.SituacaoCarga.ObterDescricao()}) não permite adicionar pedidos.");

                    bool criouNovo = true;
                    serCargaLocaisPrestacao.CriarLocalPrestacao(cargaPedido, cargaLocaisPrestacao, _unitOfWork, out criouNovo);
                    if (criouNovo)
                        serCargaRota.CriarRota(cargaPedido.Carga, tipoServicoMultisoftware, _unitOfWork, configuracaoPedido);

                    serCargaWS.AdicionarProdutosCarga(cargaPedido, cargaIntegracao, ref stMensagem, _unitOfWork, configuracaoTMS.UsarPesoProdutoSumarizacaoCarga);
                    if (cargaIntegracao.FecharCargaAutomaticamente)
                    {
                        if (!cargaPedido.Carga.DataCarregamentoCarga.HasValue)
                            cargaPedido.Carga.DataCarregamentoCarga = DateTime.Now;

                        serCarga.FecharCarga(cargaPedido.Carga, _unitOfWork, tipoServicoMultisoftware, null);
                        Servicos.Log.TratarErro("12 - Fechou Carga (" + cargaPedido.Carga.Codigo + ") " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "FechamentoCarga");
                        cargaPedido.Carga.CargaFechada = true;

                        if (cargaPedido.Carga.ExigeNotaFiscalParaCalcularFrete && !(cargaPedido.Carga.TipoOperacao?.PermiteImportarDocumentosManualmente ?? false) && !(cargaPedido.Carga.TipoOperacao?.ExigirConfirmacaoDadosTransportadorAvancarCarga ?? false))
                        {
                            cargaPedido.Carga.MotivoPendenciaFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoPendenciaFrete.NenhumPendencia;
                            cargaPedido.Carga.PossuiPendencia = false;
                            cargaPedido.Carga.MotivoPendencia = "";
                            cargaPedido.Carga.CalculandoFrete = true;
                            cargaPedido.Carga.DataInicioCalculoFrete = DateTime.Now;
                            cargaPedido.Carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete;
                            Servicos.Log.TratarErro("Atualizou a situação para calculo frete 2 Carga: " + cargaPedido.Carga.CodigoCargaEmbarcador, "AtualizouSituacaoCalculoFrete");
                        }

                        if (!cargaPedido.Carga.DataEnvioUltimaNFe.HasValue && cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova)
                        {
                            if (!cargaPedido.Carga.TipoOperacao.PermiteImportarDocumentosManualmente || cargaPedido.Carga.TipoOperacao.NaoExigeConformacaoDasNotasEmissao)
                            {
                                cargaPedido.Carga.DataEnvioUltimaNFe = DateTime.Now;
                                cargaPedido.Carga.DataRecebimentoUltimaNFe = DateTime.Now;
                                cargaPedido.SituacaoEmissao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNF.NFEnviada;
                                repCargaPedido.Atualizar(cargaPedido);
                            }
                        }

                        repCarga.Atualizar(cargaPedido.Carga);
                    }
                }
            }

            if (stMensagem.Length > 0)
                mensagem = stMensagem.ToString();

            return cargaPedido;
        }

        public void VincularXMLNotaFiscal(Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, bool adicionadoViaEmail, bool finalizarEnvioDocumentosCarga)
        {
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial repositorioCargaPedidoXMLNotaFiscalParcial = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial(_unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repositorioXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaChaveNfe repCargaEntregaChavesNfe = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaChaveNfe(_unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLnotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Servicos.Embarcador.Carga.CargaPedido servicoCargaPedido = new Servicos.Embarcador.Carga.CargaPedido(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial> cargaPedidoXMLNotaFiscaisParciais = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial>();
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosExiste = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            Servicos.Log.TratarErro($"Buscando Parciais Chave: {xmlNotaFiscal.Chave}  - {DateTime.Now.ToString("dd/MM/yyyy HH:mm:sss")}", $"ProcessamentoArquivoXMLNotaFiscalIntegracao");

            if (repTipoIntegracao.ExistePorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Unilever))
                cargaPedidoXMLNotaFiscaisParciais = repositorioCargaPedidoXMLNotaFiscalParcial.BuscarPorChave(xmlNotaFiscal.Chave);
            else
                cargaPedidoXMLNotaFiscaisParciais = repositorioCargaPedidoXMLNotaFiscalParcial.BuscarPorNumeroOuNumeroPedidoSemCarga(xmlNotaFiscal.Numero, xmlNotaFiscal.NumeroPedido, xmlNotaFiscal.NumeroDT, xmlNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida ? xmlNotaFiscal.Emitente.CPF_CNPJ : xmlNotaFiscal.Destinatario.CPF_CNPJ, xmlNotaFiscal.Chave);

            Servicos.Log.TratarErro($"VincularXMLNotaFiscal  Parciais Encontrados  {cargaPedidoXMLNotaFiscaisParciais.Count()} {DateTime.Now.ToString("dd/MM/yyyy HH:mm:sss")}", $"ProcessamentoArquivoXMLNotaFiscalIntegracao");

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial cargaPedidoXMLNotaFiscalParcial in cargaPedidoXMLNotaFiscaisParciais)
            {
                if (cargaPedidoXMLNotaFiscalParcial != null)
                {
                    Servicos.Log.TratarErro($"Encontrou Parcial {cargaPedidoXMLNotaFiscalParcial.Codigo} {DateTime.Now.ToString("dd/MM/yyyy HH:mm:sss")}", $"ProcessamentoArquivoXMLNotaFiscalIntegracao");

                    if (cargaPedidoXMLNotaFiscalParcial.Status == StatusNfe.Cancelado)
                    {
                        Servicos.Log.TratarErro($"Parcial CANCELADO {cargaPedidoXMLNotaFiscalParcial.Codigo} {DateTime.Now.ToString("dd/MM/yyyy HH:mm:sss")}", $"ProcessamentoArquivoXMLNotaFiscalIntegracao");

                        Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repositorioCargaPedido.BuscarPorCodigo(cargaPedidoXMLNotaFiscalParcial.CargaPedido.Codigo);

                        bool existNotaParcial = repositorioCargaPedidoXMLNotaFiscalParcial.VerificarSeExisteNotaParcialSemNotaParaCargaPedido(cargaPedidoXMLNotaFiscalParcial.CargaPedido.Codigo);

                        if (cargaPedido.Carga.SituacaoCarga == SituacaoCarga.AgNFe && existNotaParcial)
                            cargaPedido.SituacaoEmissao = SituacaoNF.AgEnvioNF;
                        else if (cargaPedido.Carga.SituacaoCarga == SituacaoCarga.AgNFe && !existNotaParcial && repPedidoXMLnotaFiscal.VerificarSeExistePorCargaPedidoComNota(cargaPedido.Codigo))
                            cargaPedido.SituacaoEmissao = SituacaoNF.NFEnviada;
                        else if (cargaPedido.Carga.SituacaoCarga == SituacaoCarga.AgNFe && !existNotaParcial && !repPedidoXMLnotaFiscal.VerificarSeExistePorCargaPedidoComNota(cargaPedido.Codigo))
                            cargaPedido.SituacaoEmissao = SituacaoNF.AgEnvioNF;

                        repositorioCargaPedido.Atualizar(cargaPedido);

                        if (cargaPedido.Carga.SituacaoCarga == SituacaoCarga.CalculoFrete && cargaPedido.SituacaoEmissao == SituacaoNF.AgEnvioNF)
                        {
                            cargaPedido.Carga.SituacaoCarga = SituacaoCarga.AgNFe;
                            cargaPedido.Carga.CalculandoFrete = false;
                            cargaPedido.Carga.DataInicioCalculoFrete = null;
                            cargaPedido.Carga.DataEnvioUltimaNFe = null;

                            Servicos.Auditoria.Auditoria.Auditar(auditado, cargaPedido.Carga, "Retorno da carga para Notas Fiscais ao receber nota com parcial cancelado", _unitOfWork);

                            repCarga.Atualizar(cargaPedido.Carga);
                        }

                        if (!cargaPedidosExiste.Contains(cargaPedidoXMLNotaFiscalParcial.CargaPedido))
                            cargaPedidosExiste.Add(cargaPedidoXMLNotaFiscalParcial.CargaPedido);

                        continue;
                    }
                    else
                    {

                        cargaPedidoXMLNotaFiscalParcial.XMLNotaFiscal = xmlNotaFiscal;
                        xmlNotaFiscal.TipoNotaFiscalIntegrada = cargaPedidoXMLNotaFiscalParcial.TipoNotaFiscalIntegrada;

                        repositorioCargaPedidoXMLNotaFiscalParcial.Atualizar(cargaPedidoXMLNotaFiscalParcial);
                        repositorioXMLNotaFiscal.Atualizar(xmlNotaFiscal);

                        Servicos.Log.TratarErro($"Inserindo XML Nota Fiscal Parcial: {cargaPedidoXMLNotaFiscalParcial.Codigo} XmlNotaFiscal: {xmlNotaFiscal.Codigo} - {DateTime.Now.ToString("dd/MM/yyyy HH:mm:sss")}", $"ProcessamentoArquivoXMLNotaFiscalIntegracao");
                        InserirNotaCargaPedido(xmlNotaFiscal, cargaPedidoXMLNotaFiscalParcial.CargaPedido, tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.Venda, configuracaoEmbarcador, false, out bool alteradoTipoDeCarga);

                        if (!cargaPedidosExiste.Contains(cargaPedidoXMLNotaFiscalParcial.CargaPedido))
                            cargaPedidosExiste.Add(cargaPedidoXMLNotaFiscalParcial.CargaPedido);

                        if (auditado != null)
                            Auditoria.Auditoria.Auditar(auditado, xmlNotaFiscal, adicionadoViaEmail ? "Adicionado via e-mail" : "Adicionado via importação", _unitOfWork);
                    }
                }
                else
                {
                    //todo: feito temporariamente para valer só para a GPA, tem que validar como isso vai ficar nos outros TMS
                    if (configuracaoEmbarcador.NumeroCargaSequencialUnico)
                    {
                        xmlNotaFiscal.SemCarga = true;
                        repositorioXMLNotaFiscal.Atualizar(xmlNotaFiscal);
                    }
                }
            }

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidosExiste)
            {
                if (!repositorioCargaPedidoXMLNotaFiscalParcial.VerificarSeExisteNotaParcialSemNotaParaCargaPedido(cargaPedido.Codigo))
                {
                    cargaPedido.SituacaoEmissao = SituacaoNF.NFEnviada;
                    repositorioCargaPedido.Atualizar(cargaPedido);

                    Servicos.Log.TratarErro($"Carga Pedido NFEnviada : {cargaPedido.Codigo} XmlNotaFiscal: {xmlNotaFiscal.Codigo} - {DateTime.Now.ToString("dd/MM/yyyy HH:mm:sss")}", $"ProcessamentoArquivoXMLNotaFiscalIntegracao");
                }
                else
                    Servicos.Log.TratarErro($"Ainda parcial sem Nota Situacao CargaPedido : {cargaPedido.Codigo} XmlNotaFiscal: {xmlNotaFiscal.Codigo} - {DateTime.Now.ToString("dd/MM/yyyy HH:mm:sss")}", $"ProcessamentoArquivoXMLNotaFiscalIntegracao");
            }

            if (xmlNotaFiscal != null && !string.IsNullOrWhiteSpace(xmlNotaFiscal.Chave))
            {
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaChaveNfe cargaEntregaChaveNfe = repCargaEntregaChavesNfe.BuscarPorChave(xmlNotaFiscal.Chave);
                if (cargaEntregaChaveNfe != null && cargaEntregaChaveNfe.CargaEntrega != null && xmlNotaFiscal.Emitente != null && xmlNotaFiscal.Destinatario != null)
                {
                    Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido cargaEntregaPedido = repCargaEntregaPedido.BuscarPorCargaEntregaERemetenteDestinatario(cargaEntregaChaveNfe.CargaEntrega.Codigo, xmlNotaFiscal.Emitente.CPF_CNPJ, xmlNotaFiscal.Destinatario.CPF_CNPJ);
                    if (cargaEntregaPedido != null)
                    {
                        InserirNotaCargaPedido(xmlNotaFiscal, cargaEntregaPedido.CargaPedido, tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.Venda, configuracaoEmbarcador, false, out bool alteradoTipoDeCarga);

                        if (!cargaPedidosExiste.Contains(cargaEntregaPedido.CargaPedido))
                            cargaPedidosExiste.Add(cargaEntregaPedido.CargaPedido);

                        if (auditado != null)
                            Auditoria.Auditoria.Auditar(auditado, xmlNotaFiscal, adicionadoViaEmail ? "Adicionado via e-mail" : "Adicionado via importação", _unitOfWork);
                    }
                }
                else if (xmlNotaFiscal.Codigo == 0)
                    repositorioXMLNotaFiscal.Inserir(xmlNotaFiscal);
            }
            else if (xmlNotaFiscal.Codigo == 0)
                repositorioXMLNotaFiscal.Inserir(xmlNotaFiscal);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoexiste in cargaPedidosExiste)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = cargaPedidoexiste;
                if (cargaPedido.SituacaoEmissao == SituacaoNF.NFEnviada)
                {
                    decimal pesoNaNFspesoNaNFs = repPedidoXMLnotaFiscal.BuscarPesoPorCarga(cargaPedido.Carga.Codigo);
                    int volumes = repPedidoXMLnotaFiscal.BuscarVolumesPorCarga(cargaPedido.Carga.Codigo);
                    string retornoFinalizacao = Servicos.WebService.NFe.NotaFiscal.FinalizarEnvioDasNotas(ref cargaPedido, pesoNaNFspesoNaNFs, volumes, null, null, null, null, null, configuracaoEmbarcador, tipoServicoMultisoftware, auditado, null, _unitOfWork);

                    if (string.IsNullOrWhiteSpace(retornoFinalizacao))
                    {
                        Servicos.Auditoria.Auditoria.Auditar(auditado, cargaPedido, "Integrou notas fiscais", _unitOfWork);
                        Servicos.Auditoria.Auditoria.Auditar(auditado, cargaPedido.Carga, "Integrou notas fiscais", _unitOfWork);
                    }
                }
            }

            if (cargaPedidosExiste.Count == 0)
            {
                xmlNotaFiscal.SemCarga = true;
                repositorioXMLNotaFiscal.Atualizar(xmlNotaFiscal);
            }
        }

        public bool VerificarSeCargaPossuiNotaCanceladaPeloEmitente(int carga)
        {
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
            return repPedidoXMLNotaFiscal.VerificarSeCargaPossuiNotaCanceladaPeloEmitente(carga);
        }

        public List<int> ObterNumerosNotasCanceladasPeloEmitente(int carga)
        {
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);

            return repPedidoXMLNotaFiscal.BuscarNumerosNotasCanceladasPeloEmitente(carga);
        }

        public void SalvarEventoNFParaEmissaoCancelada(string chave)
        {
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = repPedidoXMLNotaFiscal.BuscarNotaFiscalSemEmissaoPorChave(chave);

            if (pedidoXMLNotaFiscal != null)
            {
                Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(_unitOfWork);
                pedidoXMLNotaFiscal.XMLNotaFiscal.CanceladaPeloEmitente = true;
                repXMLNotaFiscal.Atualizar(pedidoXMLNotaFiscal.XMLNotaFiscal);
            }
        }

        public int BuscarNumeroDeEntregasPorNF(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Servicos.Embarcador.Carga.CTe serCargaCte = new Servicos.Embarcador.Carga.CTe(this._unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos tipoEmissaoDocumentos = cargaPedidos.First().TipoRateio;
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes tipoEmissaoCTeParticipantes = cargaPedidos.First().TipoEmissaoCTeParticipantes;

            int numeroEntregas = 1;

            if (serCargaCte.VerificarSePercursoDestinoSeraPorNota(tipoEmissaoDocumentos, tipoEmissaoCTeParticipantes, tipoServicoMultisoftware))
            {
                numeroEntregas = repPedidoXMLNotaFiscal.ContarNumeroDeDiferentesDestinatarioSaida(cargaPedidos);
                numeroEntregas += repPedidoXMLNotaFiscal.ContarNumeroDeDiferentesDestinatarioEntrada(cargaPedidos);
            }
            else if (serCargaCte.VerificarSePercursoDestinoSeraPorPedido(tipoEmissaoDocumentos, tipoEmissaoCTeParticipantes))
            {
                Servicos.Embarcador.Carga.CargaDadosSumarizados serCargaDadosSumarizados = new Carga.CargaDadosSumarizados(_unitOfWork);
                numeroEntregas = serCargaDadosSumarizados.BuscarNumeroDeEntregasPorPedido(cargaPedidos, _unitOfWork);
            }

            return numeroEntregas;
        }

        public void PreencherDadosContabeisXMLNotaFiscal(Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal, Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal notaFiscal)
        {
            if (notaFiscal.Contabilizacao == null) return;

            Repositorio.Embarcador.Pedidos.XMLNotaFiscalContabilizacao repXMLNotaFiscalContabilizacao = new Repositorio.Embarcador.Pedidos.XMLNotaFiscalContabilizacao(_unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalContabilizacao contabilizacaoNotaFiscal = repXMLNotaFiscalContabilizacao.BuscarPorXMLNotaFiscal(xmlNotaFiscal.Codigo);

            if (contabilizacaoNotaFiscal == null)
            {
                contabilizacaoNotaFiscal = new Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalContabilizacao()
                {
                    XMLNotaFiscal = xmlNotaFiscal,
                };
            }

            contabilizacaoNotaFiscal.NumeroRecebimento = notaFiscal.Contabilizacao.NumeroRecebimento;
            contabilizacaoNotaFiscal.CFOPEntrada = notaFiscal.Contabilizacao.CFOPEntrada;
            contabilizacaoNotaFiscal.CodigoEmpresa = notaFiscal.Contabilizacao.CodigoEmpresa;
            contabilizacaoNotaFiscal.NomeEmpresa = notaFiscal.Contabilizacao.NomeEmpresa;
            contabilizacaoNotaFiscal.DataContabilizacao = ObterDataPorFormatoEntrada(notaFiscal.Contabilizacao.DataContabilizacao);
            contabilizacaoNotaFiscal.DataRecebimentoFisico = ObterDataPorFormatoEntrada(notaFiscal.Contabilizacao.DataRecebimentoFisico);
            contabilizacaoNotaFiscal.CodigoTransacaoRecebimento = notaFiscal.Contabilizacao.CodigoTransacaoRecebimento;
            contabilizacaoNotaFiscal.TransacaoRecebimento = notaFiscal.Contabilizacao.TransacaoRecebimento;
            contabilizacaoNotaFiscal.ReversaoRecebimento = notaFiscal.Contabilizacao.ReversaoRecebimento;
            contabilizacaoNotaFiscal.CalcularPisCofins = notaFiscal.Contabilizacao.CalcularPisCofins;

            if (!string.IsNullOrEmpty(notaFiscal.Contabilizacao.ContaTransacao))
                contabilizacaoNotaFiscal.ContaTransacao = notaFiscal.Contabilizacao.ContaTransacao;

            contabilizacaoNotaFiscal.OrdemVenda = notaFiscal.Contabilizacao.OrdemVenda;
            contabilizacaoNotaFiscal.UC = notaFiscal.Contabilizacao.UC;
            contabilizacaoNotaFiscal.DataOrdemVenda = ObterDataPorFormatoEntrada(notaFiscal.Contabilizacao.DataOrdemVenda);
            contabilizacaoNotaFiscal.CodigoUnicoNF = notaFiscal.Contabilizacao.CodigoUnicoNF;
            contabilizacaoNotaFiscal.EstruturaVenda = notaFiscal.Contabilizacao.EstruturaVenda;
            contabilizacaoNotaFiscal.Especie = notaFiscal.Contabilizacao.Especie;
            contabilizacaoNotaFiscal.ItemFrete = notaFiscal.Contabilizacao.ItemFrete;
            contabilizacaoNotaFiscal.ContaContabil = notaFiscal.Contabilizacao.ContaContabil;
            contabilizacaoNotaFiscal.Mercado = notaFiscal.Contabilizacao.Mercado;
            contabilizacaoNotaFiscal.Diretoria = notaFiscal.Contabilizacao.Diretoria;
            contabilizacaoNotaFiscal.DescricaoUC = notaFiscal.Contabilizacao.DescricaoUC;
            contabilizacaoNotaFiscal.Pedagio = notaFiscal.Contabilizacao.Pedagio;
            contabilizacaoNotaFiscal.DescricaoTransacao = notaFiscal.Contabilizacao.DescTransacao;

            if (!string.IsNullOrWhiteSpace(notaFiscal.Contabilizacao.ContaTransacao))
            {
                string[] segmentos = notaFiscal.Contabilizacao.ContaTransacao.Split('.');

                for (var i = 1; i <= segmentos.Length; i++)
                {
                    if (i == 2)
                        contabilizacaoNotaFiscal.CIA = segmentos[i].Substring(0, 2);

                    if (i == 3)
                        contabilizacaoNotaFiscal.CodContaContabil = segmentos[i];
                }
                ;
            }

            if (contabilizacaoNotaFiscal.Codigo == 0)
                repXMLNotaFiscalContabilizacao.Inserir(contabilizacaoNotaFiscal);
            else
                repXMLNotaFiscalContabilizacao.Atualizar(contabilizacaoNotaFiscal);
        }

        public Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal PreencherParaXMLNotaFiscal(ref Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal, Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal notaFiscal, Dominio.Entidades.Empresa empresa, Dominio.Entidades.Embarcador.Filiais.Filial filial, ref string mensagem)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);

            xmlNotaFiscal.TipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.NFe;
            xmlNotaFiscal.BaseCalculoICMS = notaFiscal.BaseCalculoICMS;
            xmlNotaFiscal.Chave = Utilidades.String.OnlyNumbers(notaFiscal.Chave);
            xmlNotaFiscal.TipoEmissao = Utilidades.Chave.ObterTipoEmissao(xmlNotaFiscal.Chave).ToString().ToEnum<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoNotaFiscal>();
            xmlNotaFiscal.DocumentoRecebidoViaNOTFIS = notaFiscal.DocumentoRecebidoViaNOTFIS;
            xmlNotaFiscal.TipoDeCarga = notaFiscal.TipoDeCarga;
            xmlNotaFiscal.NumeroCarregamento = notaFiscal.NumeroCarregamento;

            if (xmlNotaFiscal.FormaIntegracao == FormaIntegracao.OKColeta && (xmlNotaFiscal.DocumentoRecebidoViaNOTFIS || xmlNotaFiscal.DocumentoRecebidoViaEmail || xmlNotaFiscal.DocumentoRecebidoViaFTP))
                xmlNotaFiscal.FormaIntegracao = FormaIntegracao.ClienteFTPOKColeta;
            if (xmlNotaFiscal.FormaIntegracao == FormaIntegracao.OKColeta && xmlNotaFiscal.NotaJaEstavaNaBase)
                xmlNotaFiscal.FormaIntegracao = FormaIntegracao.OKColetaManual;
            else if (xmlNotaFiscal.NotaJaEstavaNaBase && (xmlNotaFiscal.DocumentoRecebidoViaNOTFIS || xmlNotaFiscal.DocumentoRecebidoViaEmail || xmlNotaFiscal.DocumentoRecebidoViaFTP))
                xmlNotaFiscal.FormaIntegracao = FormaIntegracao.ClienteFTPManual;
            else if (xmlNotaFiscal.DocumentoRecebidoViaNOTFIS || xmlNotaFiscal.DocumentoRecebidoViaEmail || xmlNotaFiscal.DocumentoRecebidoViaFTP)
                xmlNotaFiscal.FormaIntegracao = FormaIntegracao.ClienteFTP;
            else if (xmlNotaFiscal.FormaIntegracao == FormaIntegracao.OKColeta)
                xmlNotaFiscal.FormaIntegracao = FormaIntegracao.OKColeta;
            else
                xmlNotaFiscal.FormaIntegracao = FormaIntegracao.Manual;

            DateTime dataEmissao = DateTime.MinValue;

            if (!string.IsNullOrWhiteSpace(notaFiscal.DataEmissao))
            {
                DateTime.TryParseExact(notaFiscal.DataEmissao, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dataEmissao);

                if (notaFiscal.DataEmissao.Length == 10)
                    DateTime.TryParseExact(notaFiscal.DataEmissao, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissao);

                if (notaFiscal.DataEmissao.Length == 8)
                    DateTime.TryParseExact(notaFiscal.DataEmissao, "ddMMyyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissao);
            }

            if (dataEmissao != DateTime.MinValue)
                xmlNotaFiscal.DataEmissao = dataEmissao;
            else
                xmlNotaFiscal.DataEmissao = DateTime.Now;

            Servicos.Cliente servicoCliente = new Servicos.Cliente(_unitOfWork.StringConexao);
            Dominio.ObjetosDeValor.RetornoVerificacaoCliente retornoConversaoDestinatario = servicoCliente.ConverterObjetoValorPessoa(notaFiscal.Destinatario, "Destinatario", _unitOfWork, empresa != null ? empresa.Codigo : 0, false);
            if (retornoConversaoDestinatario.Status)
                xmlNotaFiscal.Destinatario = retornoConversaoDestinatario.cliente;
            else
                mensagem += retornoConversaoDestinatario.Mensagem;

            Dominio.ObjetosDeValor.RetornoVerificacaoCliente retornoConversaoEmitente = servicoCliente.ConverterObjetoValorPessoa(notaFiscal.Emitente, "Emitente", _unitOfWork, empresa != null ? empresa.Codigo : 0, false);
            if (retornoConversaoEmitente.Status)
                xmlNotaFiscal.Emitente = retornoConversaoEmitente.cliente;
            else
                mensagem += retornoConversaoEmitente.Mensagem;

            if (notaFiscal.Recebedor != null && !string.IsNullOrEmpty(notaFiscal.Recebedor.CPFCNPJ))
            {
                Dominio.ObjetosDeValor.RetornoVerificacaoCliente retornoConversaoRecebedor = servicoCliente.ConverterObjetoValorPessoa(notaFiscal.Recebedor, "Recebedor", _unitOfWork, empresa != null ? empresa.Codigo : 0, false);
                if (retornoConversaoRecebedor.Status)
                    xmlNotaFiscal.Recebedor = retornoConversaoRecebedor.cliente;
                else
                    mensagem += retornoConversaoRecebedor.Mensagem;
            }

            if (notaFiscal.Expedidor != null)
            {
                Dominio.ObjetosDeValor.RetornoVerificacaoCliente retornoConversaoExpedidor = servicoCliente.ConverterObjetoValorPessoa(notaFiscal.Expedidor, "Expedidor", _unitOfWork, empresa != null ? empresa.Codigo : 0, false);
                if (retornoConversaoExpedidor.Status)
                    xmlNotaFiscal.Expedidor = retornoConversaoExpedidor.cliente;
                else
                    mensagem += retornoConversaoExpedidor.Mensagem;
            }

            if (notaFiscal.Tomador != null)
            {
                Dominio.ObjetosDeValor.RetornoVerificacaoCliente retornoConversao = servicoCliente.ConverterObjetoValorPessoa(notaFiscal.Tomador, "Tomador", _unitOfWork, empresa != null ? empresa.Codigo : 0, false);
                if (retornoConversao.Status)
                    xmlNotaFiscal.Tomador = retornoConversao.cliente;
                else
                    mensagem += retornoConversao.Mensagem;
            }

            xmlNotaFiscal.MasterBL = notaFiscal.MasterBL;
            xmlNotaFiscal.Filial = filial;
            xmlNotaFiscal.Modelo = notaFiscal.Modelo;

            if (xmlNotaFiscal.Modelo == "04")
                xmlNotaFiscal.TipoDocumento = TipoDocumento.NotaFiscal;
            else if (xmlNotaFiscal.Modelo == "99" || string.IsNullOrWhiteSpace(xmlNotaFiscal.Chave) || xmlNotaFiscal.Chave.Length < 44)
                xmlNotaFiscal.TipoDocumento = TipoDocumento.Outros;

            if (!string.IsNullOrWhiteSpace(notaFiscal.CFOPPredominante))
                xmlNotaFiscal.CFOP = notaFiscal.CFOPPredominante.Length > 4 ? notaFiscal.CFOPPredominante.Substring(0, 4) : notaFiscal.CFOPPredominante;
            if (!string.IsNullOrWhiteSpace(notaFiscal.NCMPredominante))
                xmlNotaFiscal.NCM = notaFiscal.NCMPredominante.Length > 4 ? notaFiscal.NCMPredominante.Substring(0, 4) : notaFiscal.NCMPredominante;
            if (!string.IsNullOrWhiteSpace(notaFiscal.NumeroReferenciaEDI))
                xmlNotaFiscal.NumeroReferenciaEDI = notaFiscal.NumeroReferenciaEDI;
            if (!string.IsNullOrWhiteSpace(notaFiscal.NumeroControleCliente))
                xmlNotaFiscal.NumeroControleCliente = notaFiscal.NumeroControleCliente;
            if (!string.IsNullOrWhiteSpace(notaFiscal.PINSuframa))
                xmlNotaFiscal.PINSUFRAMA = notaFiscal.PINSuframa;
            if (!string.IsNullOrWhiteSpace(notaFiscal.NumeroControleCliente))
                xmlNotaFiscal.NumeroControleCliente = notaFiscal.NumeroControleCliente;
            xmlNotaFiscal.NaturezaOP = notaFiscal.NaturezaOP;
            xmlNotaFiscal.nfAtiva = true;
            xmlNotaFiscal.SemCarga = true;
            xmlNotaFiscal.Numero = notaFiscal.Numero;
            xmlNotaFiscal.CodigoIntegracaoCliente = notaFiscal.CodigoIntegracaoCliente;
            xmlNotaFiscal.Peso = notaFiscal.PesoBruto;
            xmlNotaFiscal.PesoBaseParaCalculo = xmlNotaFiscal.Peso;
            xmlNotaFiscal.MetrosCubicos = notaFiscal.MetroCubico;

            if (xmlNotaFiscal.Tomador != null)
                xmlNotaFiscal.ModalidadeFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.Outros;
            else
                xmlNotaFiscal.ModalidadeFrete = notaFiscal.ModalidadeFrete;

            xmlNotaFiscal.PesoLiquido = notaFiscal.PesoLiquido;
            xmlNotaFiscal.QuantidadePallets = notaFiscal.QuantidadePallets;
            xmlNotaFiscal.KMRota = notaFiscal.KMRota;

            xmlNotaFiscal.CNPJTranposrtador = notaFiscal.Transportador != null ? Utilidades.String.OnlyNumbers(notaFiscal.Transportador.CNPJ) : "";
            xmlNotaFiscal.Empresa = notaFiscal.Transportador != null ? repEmpresa.BuscarPorCNPJ(Utilidades.String.OnlyNumbers(notaFiscal.Transportador.CNPJ)) : null;

            if (notaFiscal.Transportador != null && string.IsNullOrEmpty(xmlNotaFiscal.CNPJTranposrtador))
            {
                if (!string.IsNullOrEmpty(notaFiscal.Transportador.CodigoIntegracao))
                {
                    xmlNotaFiscal.Empresa = repEmpresa.BuscarPorCodigoIntegracao(notaFiscal.Transportador.CodigoIntegracao);
                    if (xmlNotaFiscal.Empresa != null)
                        xmlNotaFiscal.CNPJTranposrtador = xmlNotaFiscal.Empresa.CNPJ;
                }
                if (string.IsNullOrEmpty(xmlNotaFiscal.CNPJTranposrtador))
                    mensagem += string.Concat("CNPJ do Transportador não foi informado; ");
            }

            xmlNotaFiscal.PlacaVeiculoNotaFiscal = notaFiscal.Veiculo != null ? notaFiscal.Veiculo.Placa : "";

            xmlNotaFiscal.Serie = notaFiscal.Serie;
            xmlNotaFiscal.TipoOperacaoNotaFiscal = notaFiscal.TipoOperacaoNotaFiscal;
            xmlNotaFiscal.Descricao = !string.IsNullOrWhiteSpace(notaFiscal.MasterBL) ? "BL" : "";
            xmlNotaFiscal.NumeroOutroDocumento = notaFiscal.NumeroOutroDocumento;

            xmlNotaFiscal.Valor = notaFiscal.Valor;
            xmlNotaFiscal.ValorCOFINS = notaFiscal.ValorCOFINS;

            xmlNotaFiscal.ValorDesconto = notaFiscal.ValorDesconto;
            xmlNotaFiscal.ValorICMS = notaFiscal.ValorICMS;
            xmlNotaFiscal.ValorIPI = notaFiscal.ValorIPI;
            xmlNotaFiscal.ValorPIS = notaFiscal.ValorPIS;
            xmlNotaFiscal.ValorST = notaFiscal.ValorST;
            xmlNotaFiscal.Rota = notaFiscal.Rota;
            xmlNotaFiscal.SubRota = notaFiscal.SubRota;
            xmlNotaFiscal.GrauRisco = notaFiscal.GrauRisco;
            xmlNotaFiscal.NumeroSolicitacao = notaFiscal.NumeroSolicitacao;
            xmlNotaFiscal.NumeroTransporte = notaFiscal.NumeroTransporte;
            xmlNotaFiscal.NumeroPedidoEmbarcador = notaFiscal.NumeroPedido;

            xmlNotaFiscal.NumeroDocumentoEmbarcador = notaFiscal.NumeroDocumentoEmbarcador;
            DateTime dataHoraCriacaoEmbarcador = DateTime.MinValue;
            if (!string.IsNullOrWhiteSpace(notaFiscal.DataHoraCriacaoEmbrcador))
            {
                DateTime.TryParseExact(notaFiscal.DataHoraCriacaoEmbrcador, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dataHoraCriacaoEmbarcador);

                if (notaFiscal.DataHoraCriacaoEmbrcador.Length == 10)
                    DateTime.TryParseExact(notaFiscal.DataHoraCriacaoEmbrcador, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataHoraCriacaoEmbarcador);

                if (notaFiscal.DataHoraCriacaoEmbrcador.Length == 8)
                    DateTime.TryParseExact(notaFiscal.DataHoraCriacaoEmbrcador, "ddMMyyyy", null, System.Globalization.DateTimeStyles.None, out dataHoraCriacaoEmbarcador);
            }
            if (dataHoraCriacaoEmbarcador != DateTime.MinValue)
                xmlNotaFiscal.DataHoraCriacaoEmbrcador = dataHoraCriacaoEmbarcador;
            else
                xmlNotaFiscal.DataHoraCriacaoEmbrcador = null;

            DateTime dataPrevisao = DateTime.MinValue;

            if (!string.IsNullOrWhiteSpace(notaFiscal.DataPrevisao))
            {
                DateTime.TryParseExact(notaFiscal.DataPrevisao, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dataPrevisao);

                if (notaFiscal.DataPrevisao.Length == 10)
                    DateTime.TryParseExact(notaFiscal.DataPrevisao, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataPrevisao);

                if (notaFiscal.DataPrevisao.Length == 8)
                    DateTime.TryParseExact(notaFiscal.DataPrevisao, "ddMMyyyy", null, System.Globalization.DateTimeStyles.None, out dataPrevisao);
            }

            if (dataPrevisao != DateTime.MinValue)
                xmlNotaFiscal.DataPrevisao = dataPrevisao;
            else
                xmlNotaFiscal.DataPrevisao = null;

            xmlNotaFiscal.ValorTotalProdutos = notaFiscal.ValorTotalProdutos;
            xmlNotaFiscal.Volumes = notaFiscal.Volumes != null && notaFiscal.Volumes.Count > 0 ? notaFiscal.Volumes.Sum(obj => obj.Quantidade) : (int)notaFiscal.VolumesTotal > 0 ? (int)notaFiscal.VolumesTotal : (notaFiscal.Produtos?.Sum(produto => (int)produto.Quantidade) ?? 0);
            if (notaFiscal.Veiculo != null)
                xmlNotaFiscal.PlacaVeiculoNotaFiscal = notaFiscal.Veiculo.Placa;

            xmlNotaFiscal.XML = "";
            xmlNotaFiscal.NumeroRomaneio = notaFiscal.NumeroRomaneio;
            xmlNotaFiscal.ClassificacaoNFe = notaFiscal.ClassificacaoNFe;

            return xmlNotaFiscal;
        }

        public void InformarDadosNotaCarga(Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, out string msgAlerta, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado = null)
        {
            msgAlerta = "";

            Carga.Carga serCarga = new Carga.Carga(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoStage repositorioStagePedido = new Repositorio.Embarcador.Pedidos.PedidoStage(_unitOfWork);

            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga> situacoesPermitidas = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga>() {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova
            };

            //if (((!cargaPedido.ReentregaSolicitada && !situacoesPermitidas.Contains(cargaPedido.Carga.SituacaoCarga)
            //    && !cargaPedido.Carga.CargaEmitidaParcialmente)
            //    || (cargaPedido.ReentregaSolicitada && serCarga.VerificarSeCargaEstaNaLogistica(cargaPedido.Carga, tipoServicoMultisoftware))) && string.IsNullOrEmpty(cargaPedido.Pedido?.NumeroControle))
            //{
            //    if (!(stage != null && (cargaPedido.Carga.TipoOperacao.TipoConsolidacao == EnumTipoConsolidacao.PreCheckIn || stage.TipoPercurso == Vazio.PercursoPrincipal)))
            //        throw new ServicoException($"A atual situação da carga ({cargaPedido.Carga.DescricaoSituacaoCarga}) não permite o envio de notas fiscais.");
            //}

            if (((!cargaPedido.ReentregaSolicitada && !situacoesPermitidas.Contains(cargaPedido.Carga.SituacaoCarga) && !cargaPedido.Carga.CargaEmitidaParcialmente) || (cargaPedido.ReentregaSolicitada && serCarga.VerificarSeCargaEstaNaLogistica(cargaPedido.Carga, tipoServicoMultisoftware))) && string.IsNullOrEmpty(cargaPedido.Pedido?.NumeroControle) && !serCarga.VerificarCargaSubTrechoTransferencia(cargaPedido.Carga))
                throw new ServicoException($"A atual situação da carga ({cargaPedido.Carga.DescricaoSituacaoCarga}) não permite o envio de notas fiscais.");


            Servicos.WebService.Carga.Pedido serWSPedido = new Servicos.WebService.Carga.Pedido(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoEndereco repPedidoEndereco = new Repositorio.Embarcador.Pedidos.PedidoEndereco(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configPedido = repConfiguracaoPedido.BuscarConfiguracaoPadrao();

            if ((cargaPedido.Carga.TipoOperacao?.ConfiguracaoEmissaoDocumento?.ValidarRelevanciaNotasPrechekin ?? false) && (cargaPedido.StageRelevanteCusto == null))
                throw new ServicoException($"Não é permitido checkin das notas para essa etapa pois a mesma é irrelevante");

            if (cargaPedido.Carga.TipoOperacao != null && cargaPedido.Carga.TipoOperacao.UtilizarDadosPedidoParaNotasExterior && xmlNotaFiscal.Destinatario != null && xmlNotaFiscal.Destinatario.Tipo == "E")
            {
                switch (cargaPedido.Pedido.TipoPagamento)
                {
                    case Dominio.Enumeradores.TipoPagamento.Pago:
                        xmlNotaFiscal.ModalidadeFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.Pago;
                        break;
                    case Dominio.Enumeradores.TipoPagamento.A_Pagar:
                        xmlNotaFiscal.ModalidadeFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.A_Pagar;
                        break;
                    case Dominio.Enumeradores.TipoPagamento.Outros:
                        xmlNotaFiscal.ModalidadeFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.Outros;
                        break;
                }

                if (xmlNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Entrada)
                {
                    xmlNotaFiscal.Destinatario = cargaPedido.Pedido.Remetente;
                    if (configuracaoTMS.UtilizaEmissaoMultimodal)
                    {
                        Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco enderecoOrigem = new Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco();
                        serWSPedido.PreecherEnderecoPedidoPorCliente(enderecoOrigem, cargaPedido.Pedido.Remetente);
                        repPedidoEndereco.Inserir(enderecoOrigem);
                        cargaPedido.Pedido.EnderecoOrigem = enderecoOrigem;
                    }
                }
                else
                    xmlNotaFiscal.Destinatario = cargaPedido.Pedido.Destinatario;
            }

            bool msgAlertaObservacao = false;
            bool notaFiscalEmOutraCarga = false;
            string mensagemErro = ValidarRegrasNota(xmlNotaFiscal, cargaPedido, tipoServicoMultisoftware, out msgAlertaObservacao, out notaFiscalEmOutraCarga);
            if (msgAlertaObservacao && !string.IsNullOrWhiteSpace(mensagemErro))
            {
                msgAlerta = mensagemErro;
                mensagemErro = "";
            }

            if (!string.IsNullOrWhiteSpace(mensagemErro))
                throw new ServicoException(mensagemErro);

            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repositorioXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(_unitOfWork);

            Servicos.Embarcador.Canhotos.Canhoto servicoCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(_unitOfWork);
            Servicos.Embarcador.Pedido.OcorrenciaPedido servOcorrenciaPedido = new Servicos.Embarcador.Pedido.OcorrenciaPedido(_unitOfWork);

            if (cargaPedido.Expedidor != null && cargaPedido.Recebedor == null && cargaPedido.Carga.Filial?.EmpresaEmissora != null)
            {
                cargaPedido.CargaPedidoTrechoAnterior = repPedidoXMLNotaFiscal.BuscarCargaPedidoAnteriorPorXMLNotaFiscal(xmlNotaFiscal.Codigo, cargaPedido.Expedidor.CPF_CNPJ);

                if (cargaPedido.CargaPedidoTrechoAnterior != null)
                {
                    cargaPedido.CargaPedidoTrechoAnterior.CargaPedidoProximoTrecho = cargaPedido;

                    repositorioCargaPedido.Atualizar(cargaPedido.CargaPedidoTrechoAnterior);

                    if (!cargaPedido.Carga.AguardandoEmissaoDocumentoAnterior)
                    {
                        Carga.Carga servicoCarga = new Carga.Carga(_unitOfWork);

                        if (
                            servicoCarga.VerificarSeCargaEstaNaLogistica(cargaPedido.CargaPedidoTrechoAnterior.Carga, tipoServicoMultisoftware) ||
                            (cargaPedido.CargaPedidoTrechoAnterior.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe) ||
                            (cargaPedido.CargaPedidoTrechoAnterior.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.PendeciaDocumentos)
                        )
                            cargaPedido.Carga.AguardandoEmissaoDocumentoAnterior = true;
                    }
                }
                cargaPedido.Carga.AguardandoEmissaoDocumentoAnterior = true;
            }

            if (cargaPedido.Recebedor != null && cargaPedido.Expedidor == null && cargaPedido.Carga.Filial?.EmpresaEmissora != null)
            {
                cargaPedido.CargaPedidoProximoTrecho = repPedidoXMLNotaFiscal.BuscarProximaCargaPedidoPorXMLNotaFiscal(xmlNotaFiscal.Codigo, cargaPedido.Recebedor.CPF_CNPJ);

                if (cargaPedido.CargaPedidoProximoTrecho != null)
                {
                    cargaPedido.CargaPedidoProximoTrecho.CargaPedidoTrechoAnterior = cargaPedido;

                    repositorioCargaPedido.Atualizar(cargaPedido.CargaPedidoProximoTrecho);
                }
            }

            Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto repConfiguracaoCanhoto = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto configuracaoCanhoto = repConfiguracaoCanhoto.BuscarConfiguracaoPadrao();

            repositorioXmlNotaFiscal.Atualizar(xmlNotaFiscal);
            ValidarSeExisteNotaFiscalParcial(xmlNotaFiscal, cargaPedido);
            servicoCanhoto.SalvarCanhotoNota(xmlNotaFiscal, cargaPedido, cargaPedido.Carga.FreteDeTerceiro ? cargaPedido.Carga.Veiculo != null && cargaPedido.Carga.Veiculo != null ? cargaPedido.Carga.Veiculo.Proprietario : cargaPedido.Carga.ProvedorOS : null, cargaPedido.Carga.Motoristas != null ? cargaPedido.Carga.Motoristas.ToList() : null, tipoServicoMultisoftware, configuracaoTMS, _unitOfWork, configuracaoCanhoto);

            InserirNotaCargaPedido(xmlNotaFiscal, cargaPedido, tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.Venda, configuracaoTMS, notaFiscalEmOutraCarga, out bool alteradoTipoDeCarga, Auditado);

            if (((cargaPedido.Carga.TipoOperacao?.GerarRedespachoParaOutrasEtapasCarregamento ?? false) && cargaPedido.Carga.TipoOperacao?.TipoOperacaoRedespacho != null) || (configPedido?.HerdarNotasImportadasPedido ?? false))
            {
                if (cargaPedido.Pedido != null)
                {
                    if (cargaPedido.Pedido.NotasFiscais == null)
                        cargaPedido.Pedido.NotasFiscais = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();

                    cargaPedido.Pedido.NotasFiscais.Add(xmlNotaFiscal);
                }
            }

            if (cargaPedido.Carga.DadosSumarizados?.CargaTrecho == CargaTrechoSumarizada.SubCarga && (cargaPedido.Carga.TipoOperacao?.ConfiguracaoCarga?.ExecutarCalculoRelevanciaDeCustoNFePorCFOP ?? false))
            {
                //buscar os pedidosXmlNotasFiscal do carga pedido
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidosXmlNotaFiscal = repPedidoXMLNotaFiscal.BuscarPorCargaPedido(cargaPedido.Codigo);

                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> xmlNotasFiscaisIrrelevantesParaFrete = pedidosXmlNotaFiscal.Where(notaPorPedido => notaPorPedido.XMLNotaFiscal.IrrelevanteParaFrete).Select(notaPorPedido => notaPorPedido.XMLNotaFiscal).ToList();

                cargaPedido.PesoMercadoriaDescontar = xmlNotasFiscaisIrrelevantesParaFrete.Sum(notaFiscal => notaFiscal.PesoLiquido);
                cargaPedido.ValorMercadoriaDescontar = xmlNotasFiscaisIrrelevantesParaFrete.Sum(notaFiscal => notaFiscal.ValorTotalProdutos);

                repositorioCargaPedido.Atualizar(cargaPedido);
            }

            repositorioPedido.Atualizar(cargaPedido.Pedido);
            repositorioCarga.Atualizar(cargaPedido.Carga);
        }

        public string InformarDadosNotaCarga(Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal notaFiscal, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, out bool alteradoTipoDeCarga, bool sempreAdiciona = false, bool naoValidarNota = false, bool viaWebService = false, string numeroControlePedido = "")
        {
            string mensagem = "";
            alteradoTipoDeCarga = false;

            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
            Servicos.WebService.Carga.Pedido serWSPedido = new Servicos.WebService.Carga.Pedido(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoEndereco repPedidoEndereco = new Repositorio.Embarcador.Pedidos.PedidoEndereco(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = repConfiguracaoPedido.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();

            Servicos.Embarcador.Pedido.OcorrenciaPedido servOcorrenciaPedido = new Servicos.Embarcador.Pedido.OcorrenciaPedido(_unitOfWork);
            Servicos.Embarcador.Carga.Carga serCarga = new Embarcador.Carga.Carga(_unitOfWork);
            Servicos.Embarcador.Canhotos.Canhoto serCanhoto = new Canhotos.Canhoto(_unitOfWork);

            if (cargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe && !sempreAdiciona && string.IsNullOrEmpty(cargaPedido.Pedido?.NumeroControle))
                return "A atual situação da carga (" + cargaPedido.Carga.DescricaoSituacaoCarga + ") não permite o envio de notas fiscais.";

            if (notaFiscal.SituacaoNFeSefaz != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNFeSefaz.Autorizada)
                return "A nota informada está cancelada, por isso não é possível informá-la.";

            Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = null;
            bool notaFiscalEmOutraCarga = false;

            if (!string.IsNullOrWhiteSpace(notaFiscal.Chave))
                xmlNotaFiscal = repXmlNotaFiscal.BuscarPorChave(notaFiscal.Chave);

            if (xmlNotaFiscal == null)
            {
                xmlNotaFiscal = new Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal();
                xmlNotaFiscal.DataRecebimento = DateTime.Now;
            }
            else
                xmlNotaFiscal.NotaJaEstavaNaBase = true;

            xmlNotaFiscal = PreencherParaXMLNotaFiscal(ref xmlNotaFiscal, notaFiscal, cargaPedido.Carga.Empresa, cargaPedido.Carga.Filial, ref mensagem);

            if (!string.IsNullOrWhiteSpace(numeroControlePedido))
                xmlNotaFiscal.NumeroControlePedido = numeroControlePedido;
            else if (!string.IsNullOrWhiteSpace(cargaPedido.Pedido.NumeroControle) && string.IsNullOrWhiteSpace(xmlNotaFiscal.NumeroControlePedido))
            {
                if (cargaPedido.Pedido.NumeroControle == xmlNotaFiscal.Numero.ToString())
                    xmlNotaFiscal.NumeroControlePedido = cargaPedido.Pedido.NumeroControle;
            }

            if (!string.IsNullOrWhiteSpace(mensagem))
                return mensagem;

            if (cargaPedido.Carga.TipoOperacao != null && cargaPedido.Carga.TipoOperacao.UtilizarDadosPedidoParaNotasExterior && xmlNotaFiscal.Destinatario != null && xmlNotaFiscal.Destinatario.Tipo == "E")
            {
                switch (cargaPedido.Pedido.TipoPagamento)
                {
                    case Dominio.Enumeradores.TipoPagamento.Pago:
                        xmlNotaFiscal.ModalidadeFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.Pago;
                        break;
                    case Dominio.Enumeradores.TipoPagamento.A_Pagar:
                        xmlNotaFiscal.ModalidadeFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.A_Pagar;
                        break;
                    case Dominio.Enumeradores.TipoPagamento.Outros:
                        xmlNotaFiscal.ModalidadeFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.Outros;
                        break;
                    default:
                        break;
                }

                if (xmlNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Entrada)
                {
                    xmlNotaFiscal.Destinatario = cargaPedido.Pedido.Remetente;
                    if (configuracaoTMS.UtilizaEmissaoMultimodal)
                    {
                        Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco enderecoOrigem = new Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco();
                        serWSPedido.PreecherEnderecoPedidoPorCliente(enderecoOrigem, cargaPedido.Pedido.Remetente);
                        repPedidoEndereco.Inserir(enderecoOrigem);
                        cargaPedido.Pedido.EnderecoOrigem = enderecoOrigem;
                    }
                }
                else
                    xmlNotaFiscal.Destinatario = cargaPedido.Pedido.Destinatario;
            }

            if (!naoValidarNota)
            {
                bool msgAlertaObservacao = false;
                string retorno = ValidarRegrasNota(xmlNotaFiscal, cargaPedido, tipoServicoMultisoftware, out msgAlertaObservacao, out notaFiscalEmOutraCarga);
                if (msgAlertaObservacao && !string.IsNullOrWhiteSpace(retorno))
                    retorno = "";
                mensagem += retorno;
            }

            if (!string.IsNullOrWhiteSpace(mensagem))
                return mensagem;

            if (notaFiscal.ValorFrete > 0m || notaFiscal.ValorFreteLiquido > 0m)
            {
                if (notaFiscal.ValorFrete > 0m)//se o valor do frete é cheio tem que remover o icms antes de salvar;
                {
                    Servicos.Embarcador.Carga.ICMS serICMS = new Carga.ICMS(_unitOfWork);
                    Dominio.Entidades.Cliente emitente = xmlNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida ? xmlNotaFiscal.Emitente : xmlNotaFiscal.Destinatario;
                    Dominio.Entidades.Cliente destinatario = xmlNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida ? xmlNotaFiscal.Destinatario : xmlNotaFiscal.Emitente;
                    Dominio.Entidades.Cliente tomador = cargaPedido.ObterTomador();

                    if (xmlNotaFiscal.ModalidadeFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.Pago)
                        tomador = xmlNotaFiscal.Emitente;
                    else if (xmlNotaFiscal.ModalidadeFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.A_Pagar)
                        tomador = xmlNotaFiscal.Destinatario;

                    bool incluirBase = false;
                    decimal baseCalculo = notaFiscal.ValorFrete;
                    decimal percentualIncluir = 100m;

                    Dominio.Entidades.Empresa empresa = cargaPedido.Carga.Empresa;
                    if (cargaPedido.Carga.EmpresaFilialEmissora != null)
                        empresa = cargaPedido.Carga.EmpresaFilialEmissora;

                    Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraICMS = serICMS.BuscarRegraICMS(cargaPedido.Carga, cargaPedido, empresa, emitente, destinatario, tomador, emitente.Localidade, destinatario.Localidade, ref incluirBase, ref percentualIncluir, baseCalculo, null, _unitOfWork, tipoServicoMultisoftware, configuracao);

                    xmlNotaFiscal.ValorFrete = notaFiscal.ValorFrete - regraICMS.ValorICMS;
                }
                else
                {
                    xmlNotaFiscal.ValorFrete = notaFiscal.ValorFreteLiquido;
                }
            }
            else
                xmlNotaFiscal.ValorFrete = 0m;


            if (xmlNotaFiscal.Codigo == 0)
            {
                repXmlNotaFiscal.Inserir(xmlNotaFiscal);
                new PedidoXMLNotaFiscal(_unitOfWork, configuracaoTMS, configuracaoGeralCarga).ArmazenarProdutosNotaFiscalPorListaDeProduto(notaFiscal.Produtos, xmlNotaFiscal, cargaPedido.Pedido, Auditado, tipoServicoMultisoftware);

                if (cargaPedido.Expedidor != null && cargaPedido.Recebedor == null && cargaPedido.Carga.Filial?.EmpresaEmissora != null)
                    cargaPedido.Carga.AguardandoEmissaoDocumentoAnterior = true;

                if (configuracaoGeralCarga?.UtilizarPesoProdutoParaCalcularPesoCarga ?? false)
                {
                    notaFiscal.PesoBruto = xmlNotaFiscal.Peso;
                    notaFiscal.PesoLiquido = xmlNotaFiscal.PesoLiquido;
                }
            }
            else
            {
                if (cargaPedido.Expedidor != null && cargaPedido.Recebedor == null && cargaPedido.Carga.Filial?.EmpresaEmissora != null)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoTrechoAnterior = repPedidoXMLNotaFiscal.BuscarCargaPedidoAnteriorPorXMLNotaFiscal(xmlNotaFiscal.Codigo, cargaPedido.Expedidor.CPF_CNPJ);
                    if (cargaPedidoTrechoAnterior != null && (cargaPedidoTrechoAnterior.Pedido.Codigo == cargaPedido.Pedido.Codigo || tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS))
                    {
                        cargaPedido.CargaPedidoTrechoAnterior = cargaPedidoTrechoAnterior;
                        cargaPedido.CargaPedidoTrechoAnterior.CargaPedidoProximoTrecho = cargaPedido;
                        repCargaPedido.Atualizar(cargaPedido.CargaPedidoTrechoAnterior);
                        if (!cargaPedido.Carga.AguardandoEmissaoDocumentoAnterior)
                        {
                            if (serCarga.VerificarSeCargaEstaNaLogistica(cargaPedido.CargaPedidoTrechoAnterior.Carga, tipoServicoMultisoftware) ||
                                cargaPedido.CargaPedidoTrechoAnterior.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe ||
                                cargaPedido.CargaPedidoTrechoAnterior.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.PendeciaDocumentos)
                            {
                                cargaPedido.Carga.AguardandoEmissaoDocumentoAnterior = true;
                            }
                        }
                    }
                    else
                    {
                        cargaPedido.Carga.AguardandoEmissaoDocumentoAnterior = true;
                    }
                }

                if (cargaPedido.Recebedor != null && cargaPedido.Expedidor == null && cargaPedido.Carga.Filial?.EmpresaEmissora != null)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoProximoTrecho = repPedidoXMLNotaFiscal.BuscarProximaCargaPedidoPorXMLNotaFiscal(xmlNotaFiscal.Codigo, cargaPedido.Recebedor.CPF_CNPJ);
                    if (cargaPedidoProximoTrecho != null && (cargaPedidoProximoTrecho.Pedido.Codigo == cargaPedido.Pedido.Codigo || tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS))
                    {
                        cargaPedido.CargaPedidoProximoTrecho = cargaPedidoProximoTrecho;
                        cargaPedido.CargaPedidoProximoTrecho.CargaPedidoTrechoAnterior = cargaPedido;
                        repCargaPedido.Atualizar(cargaPedido.CargaPedidoProximoTrecho);
                    }
                }

                repXmlNotaFiscal.Atualizar(xmlNotaFiscal);
            }

            if (xmlNotaFiscal != null && notaFiscal.Boletos != null && notaFiscal.Boletos.Count > 0)
            {
                foreach (Dominio.ObjetosDeValor.Embarcador.NFe.Boleto boleto in notaFiscal.Boletos)
                {
                    Repositorio.Embarcador.NotaFiscal.NotaFiscalBoleto repositorioNotaFiscalBoleto = new Repositorio.Embarcador.NotaFiscal.NotaFiscalBoleto(_unitOfWork);
                    Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalBoleto notaFiscalBoleto = new Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalBoleto();

                    notaFiscalBoleto.XMLNotaFiscal = xmlNotaFiscal;
                    notaFiscalBoleto.Numero = boleto?.Numero ?? string.Empty;
                    notaFiscalBoleto.Valor = boleto?.Valor ?? 0;
                    notaFiscalBoleto.Parcela = boleto?.Parcela ?? 0;
                    if (boleto.DataVencimento.HasValue)
                        notaFiscalBoleto.DataVencimento = boleto.DataVencimento.Value;

                    repositorioNotaFiscalBoleto.Inserir(notaFiscalBoleto);
                }
            }

            PreencherDadosContabeisXMLNotaFiscal(xmlNotaFiscal, notaFiscal);

            new Pessoa.GrupoPessoasObservacaoNfe().AdicionarDadosNfePorGrupoPessoasEmitente(cargaPedido, xmlNotaFiscal.Emitente, notaFiscal.InformacoesComplementares, tipoServicoMultisoftware, configuracaoTMS, _unitOfWork, xmlNotaFiscal, Auditado);

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && !AjustarComponentesObrigatoriosNotaFiscal(out mensagem, xmlNotaFiscal, notaFiscal))
                return mensagem;

            ValidarSeExisteNotaFiscalParcial(xmlNotaFiscal, cargaPedido);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto repConfiguracaoCanhoto = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto configuracaoCanhoto = repConfiguracaoCanhoto.BuscarConfiguracaoPadrao();
            serCanhoto.SalvarCanhotoNota(xmlNotaFiscal, cargaPedido, cargaPedido.Carga.FreteDeTerceiro && cargaPedido.Carga.Veiculo != null ? cargaPedido.Carga.Veiculo.Proprietario : cargaPedido.Carga.ProvedorOS, cargaPedido.Carga.Motoristas != null ? cargaPedido.Carga.Motoristas.ToList() : null, tipoServicoMultisoftware, configuracao, _unitOfWork, configuracaoCanhoto);

            InserirNotaCargaPedido(xmlNotaFiscal, cargaPedido, tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.Venda, configuracaoTMS, notaFiscalEmOutraCarga, out alteradoTipoDeCarga, Auditado);

            if (((cargaPedido.Carga.TipoOperacao?.GerarRedespachoParaOutrasEtapasCarregamento ?? false) && cargaPedido.Carga.TipoOperacao?.TipoOperacaoRedespacho != null) || (configuracaoPedido?.HerdarNotasImportadasPedido ?? false))
            {
                if (cargaPedido.Pedido != null)
                {
                    if (cargaPedido.Pedido.NotasFiscais == null)
                        cargaPedido.Pedido.NotasFiscais = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();

                    cargaPedido.Pedido.NotasFiscais.Add(xmlNotaFiscal);
                }
            }

            if (viaWebService)
                Servicos.Auditoria.Auditoria.Auditar(Auditado, xmlNotaFiscal, "Adicionado via Web Service", _unitOfWork);
            else
                Servicos.Auditoria.Auditoria.Auditar(Auditado, xmlNotaFiscal, "Adicionado via importação de arquivo", _unitOfWork);

            repPedido.Atualizar(cargaPedido.Pedido);
            repCarga.Atualizar(cargaPedido.Carga);

            return mensagem;
        }

        public void ValidarTransportadorDivergente(Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                return;

            
            if (!new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(_unitOfWork).BuscarPrimeiroRegistro().ValidarTransportadorNaoInformadoNaImportacaoDocumento)
            {
                if (string.IsNullOrWhiteSpace(xmlNotaFiscal.CNPJTranposrtador))
                    return;

                if (cargaPedido.Carga.Empresa == null)
                    return;
            }

            if (cargaPedido.Carga.TipoOperacao?.NaoValidarTransportadorImportacaoDocumento ?? false)
                return;

            if ((xmlNotaFiscal.CNPJTranposrtador?.ObterSomenteNumeros() != cargaPedido.Carga.Empresa?.CNPJ.ObterSomenteNumeros()) && !cargaPedido.Redespacho && !cargaPedido.Carga.NaoExigeVeiculoParaEmissao)
                throw new ServicoException($"(nf: {xmlNotaFiscal.Chave}) O Transportador da nota ({xmlNotaFiscal.CNPJTranposrtador ?? ""}) é diferente do informado na carga ({cargaPedido.Carga.Empresa?.CNPJ_Formatado ?? ""})");
        }

        public string ValidarRegrasNota(Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, out bool msgAlertaObservacao, out bool notaFiscalEmOutraCarga, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = null)
        {
            //todo: criar aqui as regras de notas que serão permitidas ou não por embarcador no TMS
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(_unitOfWork);
            Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete repCargaPedidoComponentesFrete = new Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete(_unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoEndereco repPedidoEndereco = new Repositorio.Embarcador.Pedidos.PedidoEndereco(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoEspelhoIntercement repPedidoEspelhoIntercement = new Repositorio.Embarcador.Pedidos.PedidoEspelhoIntercement(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(_unitOfWork);

            Servicos.WebService.Carga.Pedido serWSPedido = new Servicos.WebService.Carga.Pedido(_unitOfWork);
            Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(_unitOfWork);
            Servicos.Embarcador.Carga.ICMS serCargaICMS = new Embarcador.Carga.ICMS(_unitOfWork);

            notaFiscalEmOutraCarga = false;
            msgAlertaObservacao = false;

            try
            {
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = repIntegracaoIntercab.BuscarIntegracao();
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = configuracaoTMS != null ? configuracaoTMS : repConfiguracaoTMS.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = repConfiguracaoPedido.BuscarConfiguracaoPadrao();
                bool cargaCargoX = (cargaPedido.Carga.Integracoes != null) && cargaPedido.Carga.Integracoes.Any(o => o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.CargoX);

                if (cargaCargoX && !string.IsNullOrWhiteSpace(xmlNotaFiscal.XML))
                {
                    try
                    {
                        //Feito carregar o XML novamente pois CargoX envia na integração da carga IBGE do cliente errado
                        Servicos.NFe svcNFe = new Servicos.NFe(_unitOfWork);
                        byte[] byteArray = Encoding.UTF8.GetBytes(xmlNotaFiscal.XML);
                        System.IO.MemoryStream memoryStream = new System.IO.MemoryStream(byteArray);
                        svcNFe.ObterDocumentoPorXML(memoryStream, _unitOfWork);
                    }
                    catch (Exception)
                    {
                    }
                }

                if (cargaPedido.Carga.TipoOperacao != null && (cargaPedido.Carga.TipoOperacao?.ConfiguracaoCarga?.VincularPedidoDeAcordoComNumeroOrdem ?? false))
                {
                    Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoOrdem = repPedido.BuscarPedidoPorNumeroOrdem(xmlNotaFiscal.NumeroOrdemPedidoIntegracaoUnilever);

                    if (pedidoOrdem == null)
                        throw new ServicoException("Não foi encontrado um pedido com o número da ordem igual ao informado na nota fiscal");

                    if (pedidoOrdem.Codigo != cargaPedido.Pedido.Codigo)
                        throw new ServicoException("Número do pedido na nota fiscal deve ser igual ao da carga");

                    cargaPedido.Pedido = pedidoOrdem;
                }

                if (cargaPedido.Carga.TipoOperacao != null && (cargaPedido.Carga.TipoOperacao?.PermiteImportarDocumentosManualmente ?? false))
                {
                    int dias = cargaPedido.Carga.TipoOperacao?.ConfiguracaoCarga?.QuantidadeDiasValidacaoNFeDataCarregamento ?? 0;

                    if (cargaPedido.Carga?.DataCarregamentoCarga != null && dias > 0 && Math.Abs(cargaPedido.Carga.DataCarregamentoCarga.Value.Subtract(xmlNotaFiscal.DataEmissao).TotalDays) >= dias)
                    {
                        throw new ServicoException("Não é permitido informar nota fiscal com data de emissão que difere em mais de " + dias.ToString() + " da data de carregamento");
                    }

                }

                Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracaoCarga = null;
                if (cargaPedido.Carga.TipoOperacao != null && cargaPedido.Carga.TipoOperacao.UsarConfiguracaoEmissao && cargaPedido.Carga.TipoOperacao.TipoIntegracao != null)
                    tipoIntegracaoCarga = cargaPedido.Carga.TipoOperacao.TipoIntegracao;
                else if (cargaPedido.Carga.GrupoPessoaPrincipal != null && cargaPedido.Carga.GrupoPessoaPrincipal.TipoIntegracao != null)
                    tipoIntegracaoCarga = cargaPedido.Carga.GrupoPessoaPrincipal.TipoIntegracao;
                else if (cargaPedido.ObterTomador()?.NaoUsarConfiguracaoEmissaoGrupo ?? false)
                    tipoIntegracaoCarga = cargaPedido.ObterTomador()?.TipoIntegracao;

                int numeroCTe = repPedidoCTeParaSubContratacao.ContarPorCargaPedido(cargaPedido.Codigo);
                if (numeroCTe > 0 && !cargaPedido.Carga.CargaRecebidaDeIntegracao)
                    throw new ServicoException("Não é possível enviar notas para o pedido pois o mesmo possui CT-es anteriores vinculados e ele, remova os CT-es anteriores para poder enviar as notas.");

                if (!cargaPedido.Carga.CargaRecebidaDeIntegracao && !((cargaPedido.Carga?.Mercosul ?? false) || (cargaPedido.Carga?.Internacional ?? false)) && !cargaPedido.CTeEmitidoNoEmbarcador && string.IsNullOrWhiteSpace(cargaPedido.Pedido.NumeroControle) && repPedidoXMLNotaFiscal.ContemDocumentoLancadoComOutroTipo(cargaPedido.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.NFe))
                    throw new ServicoException("Não é possível enviar notas de outro tipo de documento já existente nesta carga.");

                if (integracaoIntercab?.BuscarTipoServicoModeloDocumentoVinculadoCarga ?? false)
                {
                    if (!serCarga.AtualizarTipoServicoCargaMultimodal(null, cargaPedido, _unitOfWork, out string msgRetornoTipoServico))
                    {
                        serCarga.AtualizarPendenciaDocumentoPortoPorto(cargaPedido.Carga, true, msgRetornoTipoServico, _unitOfWork);
                        throw new ServicoException(msgRetornoTipoServico);
                    }
                }

                if (cargaPedido.Carga?.TipoOperacao?.ExclusivaDeSubcontratacaoOuRedespacho ?? false)
                    throw new ServicoException("O tipo de operação informado na carga (" + cargaPedido.Carga?.TipoOperacao.Descricao + ") não permite o envio de notas fiscais pois a operação é exclusiva para geração de cargas de subcontratação ou redespacho.");

                if (cargaPedido.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada ||
                    cargaPedido.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Redespacho ||
                    cargaPedido.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SVMProprio ||
                    cargaPedido.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.RedespachoIntermediario ||
                    cargaPedido.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SVMTerceiro)
                {
                    cargaPedido.EmitirComplementarFilialEmissora = false;
                    cargaPedido.TipoContratacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Normal;
                    cargaPedido.Pedido.PedidoSubContratado = false;
                    cargaPedido.Pedido.SubContratante = null;
                    cargaPedido.Tomador = cargaPedido.Pedido.UsarTipoTomadorPedido ? cargaPedido.Pedido.Tomador : null;
                    cargaPedido.TipoTomador = cargaPedido.Pedido.UsarTipoTomadorPedido ? cargaPedido.Pedido.TipoTomador : Dominio.Enumeradores.TipoTomador.Remetente;
                    cargaPedido.ModeloDocumentoFiscal = null;
                }

                if (cargaPedido.Carga.EmitirCTeComplementar)
                    throw new ServicoException("Não é permitido enviar notas fiscais a cargas que emitem CT-es complementares.");

                if (tipoIntegracaoCarga != null && tipoIntegracaoCarga.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Intercement)
                {
                    string numeroPedido = xmlNotaFiscal.NumeroDT;
                    if (!string.IsNullOrWhiteSpace(numeroPedido))
                    {
                        if (!repPedidoEspelhoIntercement.ContemNumeroRemessaPorCargaPedido(numeroPedido, cargaPedido.Codigo))
                            throw new ServicoException("Não foi localizado nenhum espelho importado à esta carga com o número de remessa " + numeroPedido + ".");
                    }
                    else
                        throw new ServicoException("Não foi localizado o número da remessa na observação da nota fiscal para operação Intercemet, favor verificar a configuração do Tipo de Operação/Grupo de Pessoa.");
                }

                ValidarTransportadorDivergente(xmlNotaFiscal, cargaPedido, tipoServicoMultisoftware);

                if (!cargaPedido.CTeEmitidoNoEmbarcador)
                    serCarga.SetarTipoContratacaoCarga(cargaPedido.Carga, _unitOfWork);

                repCargaPedido.Atualizar(cargaPedido);

                bool naoValidarNotaFiscalExistente = false;
                bool naoValidarNotasFiscaisComDiferentesPortos = false;
                int codigoPortoOrigem = cargaPedido.Pedido?.Porto?.Codigo ?? 0;
                int codigoViagem = cargaPedido.Pedido?.PedidoViagemNavio?.Codigo ?? 0;
                int codigoContainer = cargaPedido.Pedido?.Container?.Codigo ?? 0;

                if (cargaPedido.Carga.TipoOperacao != null && cargaPedido.Carga.TipoOperacao.UsarConfiguracaoEmissao)
                    naoValidarNotasFiscaisComDiferentesPortos = cargaPedido.Carga.TipoOperacao.NaoValidarNotasFiscaisComDiferentesPortos;
                else if (cargaPedido.Pedido.Remetente != null && (cargaPedido.Pedido.Remetente.NaoUsarConfiguracaoEmissaoGrupo || cargaPedido.Pedido.Remetente.GrupoPessoas == null))
                    naoValidarNotasFiscaisComDiferentesPortos = cargaPedido.Pedido.Remetente.NaoValidarNotasFiscaisComDiferentesPortos;
                else if (cargaPedido.Pedido.Remetente != null)
                    naoValidarNotasFiscaisComDiferentesPortos = cargaPedido.Pedido.Remetente.GrupoPessoas.NaoValidarNotasFiscaisComDiferentesPortos;

                if (cargaPedido.Carga.TipoOperacao != null && cargaPedido.Carga.TipoOperacao.UsarConfiguracaoEmissao)
                    naoValidarNotaFiscalExistente = cargaPedido.Carga.TipoOperacao.NaoValidarNotaFiscalExistente;
                else if (cargaPedido.Pedido.Remetente != null && (cargaPedido.Pedido.Remetente.NaoUsarConfiguracaoEmissaoGrupo || cargaPedido.Pedido.Remetente.GrupoPessoas == null))
                    naoValidarNotaFiscalExistente = cargaPedido.Pedido.Remetente.NaoValidarNotaFiscalExistente;
                else if (cargaPedido.Pedido.Remetente != null)
                    naoValidarNotaFiscalExistente = cargaPedido.Pedido.Remetente.GrupoPessoas.NaoValidarNotaFiscalExistente;

                if (configuracao.NotaUnicaEmCargas && xmlNotaFiscal.Codigo > 0 && !naoValidarNotasFiscaisComDiferentesPortos && !naoValidarNotaFiscalExistente)
                {
                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotaFiscais = repPedidoXMLNotaFiscal.BuscarPorXMLNotaFiscal(xmlNotaFiscal.Codigo);

                    foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal nota in pedidoXMLNotaFiscais)
                    {
                        bool cargaDaNotaEstaAtiva = nota.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada && nota.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada;
                        bool contemCTeAutorizado = false;

                        if (configuracao.UtilizaEmissaoMultimodal)
                            contemCTeAutorizado = repCTe.ContemCTeAutorizadoParaNotaFiscal(xmlNotaFiscal.Codigo);

                        if (nota.CargaPedido.Carga.Codigo != cargaPedido.Carga.Codigo && cargaDaNotaEstaAtiva && !contemCTeAutorizado)
                            throw new ServicoException("Já existe uma NF-e com esta chave (" + xmlNotaFiscal.Chave + ") vinculada a outra carga.");
                    }
                }

                Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = null;

                if (configuracao.PossuiValidacaoParaLiberacaoCargaComNotaJaUtilizada)
                {
                    if (xmlNotaFiscal.Modelo == "55" && !cargaPedido.CTeEmitidoNoEmbarcador)
                        pedidoXMLNotaFiscal = repPedidoXMLNotaFiscal.BuscarPorChaveAtiva(xmlNotaFiscal.Chave);
                    if (pedidoXMLNotaFiscal != null)
                    {
                        if (pedidoXMLNotaFiscal.CargaPedido != null && pedidoXMLNotaFiscal.CargaPedido.Carga != null && pedidoXMLNotaFiscal.CargaPedido.Carga.Codigo != cargaPedido.Carga.Codigo)
                            notaFiscalEmOutraCarga = true;
                    }

                }
                else if (configuracao.UtilizaEmissaoMultimodal && naoValidarNotasFiscaisComDiferentesPortos && codigoPortoOrigem > 0 && !naoValidarNotaFiscalExistente)
                {
                    if (xmlNotaFiscal.Modelo == "55" && !cargaPedido.CTeEmitidoNoEmbarcador)
                        pedidoXMLNotaFiscal = repPedidoXMLNotaFiscal.BuscarPorChaveAtivaNoCTe(xmlNotaFiscal.Chave, codigoPortoOrigem);
                    if (pedidoXMLNotaFiscal != null)
                    {
                        if (repPedidoXMLNotaFiscal.BuscarPorChaveAtivaNoCTe(xmlNotaFiscal.Chave, codigoPortoOrigem, codigoViagem, 0))
                            throw new ServicoException($"Já existe uma NF-e com esta chave ({xmlNotaFiscal.Chave}) e número ({xmlNotaFiscal.Numero}) vinculada a outro pedido na carga {pedidoXMLNotaFiscal.CargaPedido.Carga.CodigoCargaEmbarcador} com o mesmo Porto de Origem {cargaPedido.Pedido?.Porto?.Descricao} e viagem.");

                        if (repPedidoXMLNotaFiscal.BuscarPorChaveAtivaNaCarga(xmlNotaFiscal.Chave, codigoPortoOrigem, codigoViagem, 0))
                            throw new ServicoException($"Já existe uma NF-e com esta chave ({xmlNotaFiscal.Chave}) e número ({xmlNotaFiscal.Numero}) vinculada a outro pedido na carga {pedidoXMLNotaFiscal.CargaPedido.Carga.CodigoCargaEmbarcador} com o mesmo Porto de Origem {cargaPedido.Pedido?.Porto?.Descricao} e viagem.");

                        if (repPedidoXMLNotaFiscal.BuscarPorChaveAtivaNaCarga(xmlNotaFiscal.Chave, codigoPortoOrigem, 0, 0))
                            throw new ServicoException($"Já existe uma NF-e com esta chave ({xmlNotaFiscal.Chave}) e número ({xmlNotaFiscal.Numero}) vinculada a outro pedido na carga {pedidoXMLNotaFiscal.CargaPedido.Carga.CodigoCargaEmbarcador} com o mesmo Porto de Origem {cargaPedido.Pedido?.Porto?.Descricao}.");
                    }
                }
                else if (configuracao.UtilizaEmissaoMultimodal && !naoValidarNotasFiscaisComDiferentesPortos && codigoPortoOrigem > 0 && !naoValidarNotaFiscalExistente)
                {
                    if (xmlNotaFiscal.Modelo == "55" && !cargaPedido.CTeEmitidoNoEmbarcador)
                        pedidoXMLNotaFiscal = repPedidoXMLNotaFiscal.BuscarPorChaveAtivaNoCTeSemMesmoPorto(xmlNotaFiscal.Chave, codigoPortoOrigem);
                    if (pedidoXMLNotaFiscal != null)
                    {
                        if (repPedidoXMLNotaFiscal.BuscarPorChaveAtivaNoCTeSemMesmoPortoViagem(xmlNotaFiscal.Chave, codigoPortoOrigem, codigoViagem, 0))
                            throw new ServicoException($"Já existe uma NF-e com esta chave ({xmlNotaFiscal.Chave}) e número ({xmlNotaFiscal.Numero}) vinculada a outro pedido na carga {pedidoXMLNotaFiscal.CargaPedido.Carga.CodigoCargaEmbarcador} com diferente porto de origem e viagem.");

                        if (repPedidoXMLNotaFiscal.BuscarPorChaveAtivaNaCargaSemMesmoPorto(xmlNotaFiscal.Chave, codigoPortoOrigem, codigoViagem, 0))
                            throw new ServicoException($"Já existe uma NF-e com esta chave ({xmlNotaFiscal.Chave}) e número ({xmlNotaFiscal.Numero}) vinculada a outro pedido na carga {pedidoXMLNotaFiscal.CargaPedido.Carga.CodigoCargaEmbarcador} com diferente porto de origem e viagem.");

                        if (repPedidoXMLNotaFiscal.BuscarPorChaveAtivaNaCargaSemMesmoPorto(xmlNotaFiscal.Chave, codigoPortoOrigem, 0, 0))
                            throw new ServicoException($"Já existe uma NF-e com esta chave ({xmlNotaFiscal.Chave}) e número ({xmlNotaFiscal.Numero}) vinculada a outro pedido na carga {pedidoXMLNotaFiscal.CargaPedido.Carga.CodigoCargaEmbarcador} com diferente porto de origem.");
                    }
                }
                else
                {
                    if (!naoValidarNotaFiscalExistente && xmlNotaFiscal.Modelo == "55" && !cargaPedido.CTeEmitidoNoEmbarcador)
                        pedidoXMLNotaFiscal = repPedidoXMLNotaFiscal.BuscarPorChaveAtiva(xmlNotaFiscal.Chave);

#if DEBUG
                    pedidoXMLNotaFiscal = null;
#endif

                    if (pedidoXMLNotaFiscal != null)
                    {
                        if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                        {
                            if (pedidoXMLNotaFiscal.CargaPedido != null && pedidoXMLNotaFiscal.CargaPedido.Carga != null && pedidoXMLNotaFiscal.CargaPedido.Carga.Codigo != cargaPedido.Carga.Codigo)
                            {
                                if (!configuracao.UtilizaEmissaoMultimodal)
                                    throw new ServicoException($"Já existe uma NF-e com esta chave ({xmlNotaFiscal.Chave}) e número ({xmlNotaFiscal.Numero}) vinculada a outro pedido na carga {pedidoXMLNotaFiscal.CargaPedido.Carga.CodigoCargaEmbarcador}.");
                            }
                        }
                        else
                        {
                            if (!configuracao.PermitirAutalizarNotaFiscalCarga)
                            {
                                if (pedidoXMLNotaFiscal.CargaPedido.Codigo == cargaPedido.Codigo)
                                    throw new ServicoException("Já existe uma NF-e com esta chave (" + xmlNotaFiscal.Chave + ") vinculada a esse pedido.");
                            }
                        }
                    }
                }

                if (cargaPedido.Pedido.TipoOperacao != null && (cargaPedido.Pedido.TipoOperacao.ConfiguracaoCarga?.ExigeNotaFiscalTenhaTagRetirada ?? false) && xmlNotaFiscal.ClienteRetirada == null)
                    throw new ServicoException("A nota fiscal não possui a tag retirada e a configuração desse tipo de operação exige uma tag de retirada.");

                if (!cargaPedido.CTeEmitidoNoEmbarcador)
                {
                    //verficar se foi baixado algum documento destinado com evento de cancelamento para a nota que se deseja emitir o documento.
                    xmlNotaFiscal.CanceladaPeloEmitente = repDocumentoDestinadoEmpresa.VerificarNotaCanceladaEmitente(xmlNotaFiscal.Chave);
                }

                Dominio.Entidades.Cliente emitenteNota = xmlNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida ? xmlNotaFiscal.Emitente : xmlNotaFiscal.Destinatario;
                Dominio.Entidades.Cliente destinatarioNota = xmlNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida ? xmlNotaFiscal.Destinatario : xmlNotaFiscal.Emitente;

                if (cargaCargoX && cargaPedido.Pedido.Destinatario != null && destinatarioNota.CPF_CNPJ != cargaPedido.Pedido.Destinatario.CPF_CNPJ)
                {
                    if (cargaPedido.Pedido.Destinatario.Localidade.Codigo != destinatarioNota.Localidade.Codigo)
                        cargaPedido.Destino = destinatarioNota.Localidade;

                    cargaPedido.Pedido.Destinatario = destinatarioNota;
                    cargaPedido.Pedido.Destino = destinatarioNota.Localidade;

                    repPedido.Atualizar(cargaPedido.Pedido);
                }
                else if (cargaPedido.Carga.TipoOperacao != null && cargaPedido.Carga.TipoOperacao.ValidarNotaFiscalPeloDestinatario)
                {
                    if (cargaPedido.Pedido.Destinatario != null)
                    {

                        if (destinatarioNota.CPF_CNPJ != cargaPedido.Pedido.Destinatario.CPF_CNPJ)
                            throw new ServicoException("O destinatário da nota " + xmlNotaFiscal.Numero + " (" + destinatarioNota.CPF_CNPJ_Formatado + ") não é o mesmo do pedido (" + cargaPedido.Pedido.Destinatario.CPF_CNPJ_Formatado + ").");
                    }
                    else
                        throw new ServicoException("Não foi informado um destinatário no pedido, e a validação da nota para essa operação exige um destinatário informado.");
                }

                Servicos.Embarcador.Carga.CTe serCargaCTe = new Servicos.Embarcador.Carga.CTe(_unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes tipoEmissaoCTeParticipante = serCargaCTe.BuscarTipoEmissaoCTeParticipantes(cargaPedido, tipoServicoMultisoftware, _unitOfWork, configuracao.UtilizarParticipantesDaCargaPeloPedido);

                if (cargaPedido.Pedido.Destinatario == null)
                {
                    if (destinatarioNota == null)
                        throw new ServicoException("O destinatário da nota fiscal não localizado.");

                    cargaPedido.Pedido.Destinatario = destinatarioNota;
                    cargaPedido.Pedido.Destino = destinatarioNota.Localidade;

                    if (cargaPedido.Destino == null)
                    {
                        cargaPedido.Destino = destinatarioNota.Localidade;
                        repCargaPedido.Atualizar(cargaPedido);
                    }

                    if (!cargaPedido.Pedido.UsarOutroEnderecoDestino && cargaPedido.Pedido.EnderecoDestino != null)
                    {
                        serWSPedido.PreecherEnderecoPedidoPorCliente(cargaPedido.Pedido.EnderecoDestino, cargaPedido.Pedido.Destinatario);

                        repPedidoEndereco.Atualizar(cargaPedido.Pedido.EnderecoDestino);
                    }

                    repPedido.Atualizar(cargaPedido.Pedido);
                }
                else if (cargaPedido.Pedido.Recebedor == null && cargaPedido.Pedido.Destinatario.Localidade != null)
                {
                    if (!cargaPedido.Pedido.UsarOutroEnderecoDestino)
                    {
                        if (cargaPedido.Recebedor == null)
                            cargaPedido.Destino = cargaPedido.Pedido.Destinatario.Localidade;
                        else
                            cargaPedido.Destino = cargaPedido.Recebedor.Localidade;
                    }
                    else
                    {
                        if (cargaPedido.Recebedor == null)
                            cargaPedido.Destino = cargaPedido.Pedido.Destino;
                        else
                            cargaPedido.Destino = cargaPedido.Recebedor.Localidade;
                    }

                    repPedido.Atualizar(cargaPedido.Pedido);
                }

                if (cargaPedido.Pedido.Remetente == null)
                {
                    if (emitenteNota.Localidade.Codigo == cargaPedido.Origem.Codigo || cargaPedido.CTeEmitidoNoEmbarcador)
                    {
                        cargaPedido.Pedido.Remetente = emitenteNota;
                        cargaPedido.Pedido.TipoPessoa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa.Pessoa;

                        if (!cargaPedido.Pedido.UsarOutroEnderecoOrigem && cargaPedido.Pedido.EnderecoOrigem != null)
                        {
                            serWSPedido.PreecherEnderecoPedidoPorCliente(cargaPedido.Pedido.EnderecoOrigem, cargaPedido.Pedido.Remetente);

                            repPedidoEndereco.Atualizar(cargaPedido.Pedido.EnderecoOrigem);
                        }

                        repPedido.Atualizar(cargaPedido.Pedido);
                    }
                    else if (configuracao.UtilizaEmissaoMultimodal && emitenteNota.Localidade.Codigo != cargaPedido.Origem.Codigo)
                    {
                        cargaPedido.Pedido.Remetente = emitenteNota;
                        cargaPedido.Pedido.TipoPessoa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa.Pessoa;
                        repPedido.Atualizar(cargaPedido.Pedido);
                    }
                    else
                        throw new ServicoException("A origem da Nota (" + emitenteNota.Localidade.DescricaoCidadeEstado + ") é diferente da origem informada para a carga (" + cargaPedido.Origem.DescricaoCidadeEstado + ")");
                }
                else if (tipoEmissaoCTeParticipante != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidor && tipoEmissaoCTeParticipante != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor) //se tem expedidor pode informar qualquer remetente
                {
                    if (cargaPedido.Pedido.Remetente.CPF_CNPJ_Formatado != emitenteNota.CPF_CNPJ_Formatado)
                    {
                        AtualizarRemetenteDoPedidoPelaNotaFiscal(cargaPedido, configuracao, configuracaoPedido, cargaCargoX, emitenteNota);
                    }
                    else if (cargaCargoX && ((cargaPedido.Origem.Codigo != emitenteNota.Localidade.Codigo) || (cargaPedido.Pedido.Codigo != emitenteNota.Localidade.Codigo)))
                    {
                        if (cargaPedido.Origem == null)
                        {
                            cargaPedido.Origem = emitenteNota.Localidade;
                            repCargaPedido.Atualizar(cargaPedido);
                        }
                        cargaPedido.Pedido.Remetente = emitenteNota;
                        cargaPedido.Pedido.Origem = emitenteNota.Localidade;
                        repPedido.Atualizar(cargaPedido.Pedido);
                    }
                }

                if (emitenteNota.GrupoPessoas != null)
                {
                    if (emitenteNota.GrupoPessoas.ValidaPlacaNFe && !string.IsNullOrWhiteSpace(xmlNotaFiscal.PlacaVeiculoNotaFiscal))
                    {
                        if (cargaPedido.Carga.Veiculo.Placa.Replace("-", "").ToLower().Trim() != xmlNotaFiscal.PlacaVeiculoNotaFiscal.Replace("-", "").ToLower().Trim() && !cargaPedido.Carga.VeiculosVinculados.Any(obj => obj.Placa.Replace("-", "").ToLower().Trim() == xmlNotaFiscal.PlacaVeiculoNotaFiscal.Replace("-", "").ToLower().Trim()))
                            throw new ServicoException("A placa informada na nota não é a mesma informada na Carga.");
                    }

                    if (emitenteNota.GrupoPessoas.ValidaDestinoNFe)
                    {
                        if (cargaPedido.Destino.Codigo != destinatarioNota.Localidade.Codigo)
                            throw new ServicoException("A localidade do destinatario informada na nota é diferente da informada na carga.");
                    }

                    if (emitenteNota.GrupoPessoas.ValidaOrigemNFe)
                    {
                        if (cargaPedido.Origem.Codigo != emitenteNota.Localidade.Codigo)
                            throw new ServicoException("A localidade do emitente informado na nota é diferente da informada na carga.");
                    }

                    if (emitenteNota.GrupoPessoas.NaoAdicionarNotaNCMPalletCarga && xmlNotaFiscal.TipoNotaFiscalIntegrada == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscalIntegrada.RemessaPallet)
                        throw new ServicoException("Não é permitido adicionar nota com NCM de Pallet na carga!");

                    if (emitenteNota.GrupoPessoas.ZerarPesoNotaNCMPalletCarga && xmlNotaFiscal.TipoNotaFiscalIntegrada == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscalIntegrada.RemessaPallet)
                    {
                        xmlNotaFiscal.Peso = 0m;
                        xmlNotaFiscal.PesoBaseParaCalculo = 0m;
                        xmlNotaFiscal.PesoLiquido = 0m;
                        if (xmlNotaFiscal.Codigo > 0)
                            repXmlNotaFiscal.Atualizar(xmlNotaFiscal);
                    }
                }

                msgAlertaObservacao = true;

                string mensagemAlertaObservacaoNota = ValidarAlertaObservacaoNota(xmlNotaFiscal, cargaPedido, tipoServicoMultisoftware, configuracao.UtilizaEmissaoMultimodal);

                ValidacaoTipoNotaVenda(cargaPedido, xmlNotaFiscal);

                if (!string.IsNullOrWhiteSpace(mensagemAlertaObservacaoNota))
                    throw new ServicoException(mensagemAlertaObservacaoNota);

                return "";
            }
            catch (ServicoException excecao)
            {
                serCarga.AtualizarPendenciaDocumentoPortoPorto(cargaPedido.Carga, true, excecao.Message, _unitOfWork);
                return excecao.Message;
            }
        }

        public void VincularNotaFiscalAPedidosPorNumeroControle(Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorNumeroControleSemCodigoXMLNotaFiscal(xmlNotaFiscal.NumeroControlePedido, xmlNotaFiscal.Codigo);
            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                //cargaPedido.Pedido.NotasFiscais.Add(xmlNotaFiscal);
                //repPedido.Atualizar(cargaPedido.Pedido);

                InserirNotaCargaPedido(xmlNotaFiscal, cargaPedido, tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.Venda, configuracaoTMS, false, out bool alteradoTipoDeCarga);
                Servicos.Auditoria.Auditoria.Auditar(auditado, xmlNotaFiscal, auditado.Texto, _unitOfWork);
            }

            if (cargaPedidos.Count == 0)
            {
                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = repPedido.BuscarPorNumeroControleSemCodigoXMLNotaFiscal(xmlNotaFiscal.NumeroControlePedido, xmlNotaFiscal.Codigo);

                foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidos)
                {
                    pedido.NotasFiscais.Add(xmlNotaFiscal);
                    repPedido.Atualizar(pedido);
                }
            }
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal InserirNotaCargaPedido(Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal tipoNotaFiscal, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, bool notaFiscalEmOutraCarga, out bool alteradoTipoDeCarga, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado = null, Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscalChave notaFiscalIntegracao = null, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto configuracaoCanhoto = null, List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotasFiscais = null)
        {
            Servicos.Embarcador.Canhotos.Canhoto serCanhoto = new Canhotos.Canhoto(_unitOfWork);
            Servicos.Embarcador.Pedido.Pedido serPedido = new Servicos.Embarcador.Pedido.Pedido(_unitOfWork);
            Servicos.Embarcador.Pedido.OcorrenciaPedido servOcorrenciaPedido = new Servicos.Embarcador.Pedido.OcorrenciaPedido(_unitOfWork);
            Servicos.Embarcador.Carga.CargaPedido servicoCargaPedido = new Servicos.Embarcador.Carga.CargaPedido(_unitOfWork);

            alteradoTipoDeCarga = false;

            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoEndereco repPedidoEndereco = new Repositorio.Embarcador.Pedidos.PedidoEndereco(_unitOfWork);
            Repositorio.Embarcador.Cargas.TipoDeCarga repTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(_unitOfWork);
            Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = null;
            Repositorio.Embarcador.Cargas.CargaMotorista repCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial repCargaPedidoXMLNotaFiscalParcial = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(_unitOfWork).BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(_unitOfWork).BuscarPrimeiroRegistro();

            if (cargaPedido.CTeEmitidoNoEmbarcador)
                pedidoXMLNotaFiscal = pedidoXMLNotasFiscais?.Find(o => o.XMLNotaFiscal.Codigo == xmlNotaFiscal.Codigo);
            else
            {
                if (configuracaoEmbarcador.PermiteAdicionarNFeRepetidaParaOutroPedidoCarga)
                    pedidoXMLNotaFiscal = repPedidoXMLNotaFiscal.BuscarPorNotaFiscalECargaPedido(xmlNotaFiscal.Codigo, cargaPedido.Codigo);
                else
                    pedidoXMLNotaFiscal = repPedidoXMLNotaFiscal.BuscarPorNotaFiscalECarga(xmlNotaFiscal.Codigo, cargaPedido.CargaOrigem.Codigo);
            }

            if ((cargaPedido.Carga.TipoOperacao?.ConfiguracaoCarga?.NaoPermitirUsoNotasQueEstaoEmOutraCarga ?? false) && repPedidoXMLNotaFiscal.VerificarSeExistePorNotaFiscalEmOutraCarga(xmlNotaFiscal.Codigo, cargaPedido.Carga.Codigo))
                return null;

            int codigoTipoDeCarga = 0;
            if (!string.IsNullOrWhiteSpace(xmlNotaFiscal.TipoDeCarga))
            {
                Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoDeCargaConsulta = repTipoDeCarga.BuscarPorCodigoEmbarcador(xmlNotaFiscal.TipoDeCarga);
                codigoTipoDeCarga = tipoDeCargaConsulta?.Codigo ?? 0;
            }

            if (cargaPedido.Carga != null && cargaPedido.Carga.TipoOperacao != null
                && cargaPedido.Carga.TipoOperacao.CriarNovoPedidoAoVincularNotaFiscalComDiferentesParticipantes
                && ((cargaPedido.Pedido != null && cargaPedido.Pedido.Remetente != null && cargaPedido.Pedido.Destinatario != null
                && xmlNotaFiscal.Emitente != null && xmlNotaFiscal.Destinatario != null
                && (cargaPedido.Pedido.Remetente.CPF_CNPJ != xmlNotaFiscal.Emitente.CPF_CNPJ || cargaPedido.Pedido.Destinatario.CPF_CNPJ != xmlNotaFiscal.Destinatario.CPF_CNPJ))
                || (codigoTipoDeCarga > 0)))
            {
                int qtdNotasVinculadasCarga = repPedidoXMLNotaFiscal.ContarNotasVinculadas(cargaPedido.Carga.Codigo, 0, "");
                int codigoCargaPedido = 0;
                string retorno = "";

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoVincular = null;
                if (qtdNotasVinculadasCarga >= 1)
                {
                    cargaPedidoVincular = repCargaPedido.BuscarPorEntidadeContainer(cargaPedido.Carga.Codigo, cargaPedido.Pedido.Container?.Codigo ?? 0, xmlNotaFiscal.Emitente.CPF_CNPJ, xmlNotaFiscal.Destinatario.CPF_CNPJ, codigoTipoDeCarga);
                    if (cargaPedidoVincular == null)
                        cargaPedidoVincular = repCargaPedido.BuscarPorEntidadeContainer(cargaPedido.Carga.Codigo, 0, xmlNotaFiscal.Emitente.CPF_CNPJ, xmlNotaFiscal.Destinatario.CPF_CNPJ, codigoTipoDeCarga);
                    if (cargaPedidoVincular == null)
                        cargaPedidoVincular = repCargaPedido.BuscarPorEntidadeContainer(cargaPedido.Carga.Codigo, cargaPedido.Pedido.Container?.Codigo ?? 0, 0, xmlNotaFiscal.Destinatario.CPF_CNPJ, codigoTipoDeCarga);
                    if (cargaPedidoVincular == null)
                        cargaPedidoVincular = repCargaPedido.BuscarPorEntidadeContainer(cargaPedido.Carga.Codigo, cargaPedido.Pedido.Container?.Codigo ?? 0, xmlNotaFiscal.Emitente.CPF_CNPJ, 0, codigoTipoDeCarga);
                    if (cargaPedidoVincular == null)
                        cargaPedidoVincular = repCargaPedido.BuscarPorEntidadeContainer(cargaPedido.Carga.Codigo, 0, 0, 0, codigoTipoDeCarga);
                }

                if (cargaPedidoVincular != null)
                    codigoCargaPedido = cargaPedidoVincular.Codigo;
                else if (codigoTipoDeCarga > 0 && (cargaPedido.Pedido?.Destinatario?.CPF_CNPJ ?? 0d) == (xmlNotaFiscal.Destinatario?.CPF_CNPJ ?? 1d))
                    codigoCargaPedido = cargaPedido.Codigo;

                if (codigoCargaPedido == 0)
                {
                    int.TryParse(Utilidades.String.OnlyNumbers(cargaPedido.Pedido.TaraContainer), out int taraContainer);
                    retorno = Servicos.Embarcador.Carga.CargaPedido.CriarPedidoNormalOuSubcontratacao(DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, taraContainer, cargaPedido.Pedido.Container?.Codigo ?? 0, cargaPedido.Pedido.LacreContainerUm, cargaPedido.Pedido.LacreContainerDois, cargaPedido.Pedido.LacreContainerTres, cargaPedido.Carga.Codigo, xmlNotaFiscal.Emitente.CPF_CNPJ, xmlNotaFiscal.Destinatario.CPF_CNPJ, 0D, cargaPedido.Pedido.NumeroPedidoEmbarcador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedidoVinculado.Normal, _unitOfWork, _unitOfWork.StringConexao, tipoServicoMultisoftware, configuracaoEmbarcador, out codigoCargaPedido, codigoTipoDeCarga, configuracaoGeralCarga);
                }
                if (string.IsNullOrWhiteSpace(retorno) && codigoCargaPedido > 0)
                {
                    Servicos.Embarcador.Hubs.Carga serHubCarga = new Servicos.Embarcador.Hubs.Carga();
                    serHubCarga.InformarCargaAtualizada(cargaPedido.Carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, _unitOfWork.StringConexao);

                    cargaPedido = repCargaPedido.BuscarPorCodigo(codigoCargaPedido);
                }
                else
                {
                    return null;
                }
            }

            if (cargaPedido.Carga != null && cargaPedido.Carga.TipoOperacao != null && cargaPedido.Carga.TipoOperacao.AlterarRemetentePedidoConformeNotaFiscal)
            {
                Dominio.Entidades.Cliente remetenteNota = xmlNotaFiscal.TipoOperacaoNotaFiscal == TipoOperacaoNotaFiscal.Entrada ? xmlNotaFiscal.Destinatario : xmlNotaFiscal.Emitente;
                if (remetenteNota != null && (cargaPedido.Pedido.Remetente == null || remetenteNota.CPF_CNPJ != cargaPedido.Pedido.Remetente.CPF_CNPJ))
                {
                    Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repPedido.BuscarPorCodigo(cargaPedido.Pedido.Codigo);
                    pedido.Remetente = remetenteNota;

                    Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco pedidoEnderecoOrigem = new Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco();
                    serPedido.PreecherEnderecoPedido(ref pedidoEnderecoOrigem, pedido.Remetente);
                    if (pedidoEnderecoOrigem != null && pedidoEnderecoOrigem.Localidade != null)
                    {
                        repPedidoEndereco.Inserir(pedidoEnderecoOrigem);
                        pedido.EnderecoOrigem = pedidoEnderecoOrigem;
                    }

                    repPedido.Atualizar(pedido);

                    if (pedido.Expedidor != null)
                        cargaPedido.Origem = pedido.Expedidor.Localidade;
                    else
                        cargaPedido.Origem = pedido.Remetente.Localidade;

                    repCargaPedido.Atualizar(cargaPedido);

                    if (Auditado != null)
                    {
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, pedido, null, "Alterou o remetente do pedido automaticamente pela nota fiscal inserida.", _unitOfWork);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaPedido.Carga, null, "Alterou o remetente do pedido automaticamente pela nota fiscal inserida.", _unitOfWork);
                    }
                }
                Dominio.Entidades.Cliente destinatarioNota = xmlNotaFiscal.TipoOperacaoNotaFiscal == TipoOperacaoNotaFiscal.Entrada ? xmlNotaFiscal.Emitente : xmlNotaFiscal.Destinatario;
                if (destinatarioNota != null && (cargaPedido.Pedido.Destinatario == null || destinatarioNota.CPF_CNPJ != cargaPedido.Pedido.Destinatario.CPF_CNPJ))
                {
                    if (cargaPedido.Pedido.Destinatario == null)
                    {
                        Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repPedido.BuscarPorCodigo(cargaPedido.Pedido.Codigo);

                        pedido.Destinatario = destinatarioNota;
                        if (pedido.Recebedor == null && pedido.Expedidor == null)
                            pedido.Destino = xmlNotaFiscal.Destinatario.Localidade;

                        repPedido.Atualizar(pedido);

                        if (pedido.Recebedor != null)
                            cargaPedido.Destino = pedido.Recebedor.Localidade;
                        if (cargaPedido.Destino == null)
                            cargaPedido.Destino = xmlNotaFiscal.Destinatario.Localidade;

                        repCargaPedido.Atualizar(cargaPedido);


                        if (Auditado != null)
                        {
                            Servicos.Auditoria.Auditoria.Auditar(Auditado, pedido, null, "Alterou o destinatário do pedido automaticamente pela nota fiscal inserida.", _unitOfWork);
                            Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaPedido.Carga, null, "Alterou o destinatário do pedido automaticamente pela nota fiscal inserida.", _unitOfWork);
                        }
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(xmlNotaFiscal.TipoDeCarga))
            {
                Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoDeCarga = repTipoDeCarga.BuscarPorCodigoEmbarcador(xmlNotaFiscal.TipoDeCarga);
                if (tipoDeCarga != null && ((cargaPedido.Carga.TipoDeCarga == null || cargaPedido.Carga.TipoDeCarga.Codigo != tipoDeCarga.Codigo) || (cargaPedido.Pedido.TipoDeCarga == null || cargaPedido.Pedido.TipoDeCarga.Codigo != tipoDeCarga.Codigo)))
                {
                    alteradoTipoDeCarga = true;
                    cargaPedido.Carga.TipoDeCarga = tipoDeCarga;
                    cargaPedido.Pedido.TipoDeCarga = tipoDeCarga;
                }
            }

            int volumes = xmlNotaFiscal.Volumes;
            int volumesAnterior = xmlNotaFiscal.Volumes;

            if (pedidoXMLNotaFiscal == null)
            {
                volumesAnterior = 0;

                pedidoXMLNotaFiscal = new Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal
                {
                    XMLNotaFiscal = xmlNotaFiscal,
                    ObservacaoNotaFiscal = xmlNotaFiscal.ObservacaoNotaFiscalParaCTe,
                    CargaPedido = cargaPedido,
                    TipoNotaFiscal = tipoNotaFiscal,
                    NotaFiscalEmOutraCarga = notaFiscalEmOutraCarga
                };

                repPedidoXMLNotaFiscal.Inserir(pedidoXMLNotaFiscal);

                if (configuracaoFinanceiro.GerarDoumentoProvisaoAoReceberNotaFiscal ?? false)
                {
                    if ((configuracaoFinanceiro.NaoPermitirProvisionarSemCalculoFrete ?? false))
                    {
                        if (cargaPedido.Carga.CalculandoFrete)
                            throw new ServicoException($"Não é permitido integrar a nota enquanto a carga está em cálculo de frete");

                        if (cargaPedido.Carga.SituacaoCarga == SituacaoCarga.Nova || (cargaPedido.Carga.SituacaoCarga == SituacaoCarga.CalculoFrete && cargaPedido.Carga.TipoFreteEscolhido == TipoFreteEscolhido.todos))
                            throw new ServicoException($"Não é permitido integrar a nota enquanto a carga não passou pela etapa de frete");
                    }
                    Servicos.Embarcador.Escrituracao.DocumentoProvisao.AdicionarDocumentoParaProvisaoPorCargaPedido(pedidoXMLNotaFiscal, false, tipoServicoMultisoftware, _unitOfWork);
                }

                if (!cargaPedido.CTeEmitidoNoEmbarcador)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial> cargaPedidoXMLNotaFiscaisParciais = repCargaPedidoXMLNotaFiscalParcial.ConsultarSemNota(cargaPedido.Codigo);

                    if (cargaPedidoXMLNotaFiscaisParciais.Count > 0)
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial cargaPedidoXMLNotaFiscalParcial = (from obj in cargaPedidoXMLNotaFiscaisParciais where obj.Chave == xmlNotaFiscal.Chave select obj).FirstOrDefault();

                        if (cargaPedidoXMLNotaFiscalParcial == null)
                            cargaPedidoXMLNotaFiscalParcial = (from obj in cargaPedidoXMLNotaFiscaisParciais where obj.Numero == xmlNotaFiscal.Numero || (obj.Pedido == xmlNotaFiscal.NumeroDT && !string.IsNullOrWhiteSpace(xmlNotaFiscal.NumeroDT) && ((obj.XMLNotaFiscal.Emitente.CPF_CNPJ == cargaPedido.Pedido.Remetente.CPF_CNPJ && obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida) || (obj.XMLNotaFiscal.Emitente.CPF_CNPJ == cargaPedido.Pedido.Destinatario.CPF_CNPJ && obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Entrada))) select obj).FirstOrDefault();

                        if (cargaPedidoXMLNotaFiscalParcial != null)
                        {
                            cargaPedidoXMLNotaFiscalParcial.XMLNotaFiscal = xmlNotaFiscal;
                            xmlNotaFiscal.TipoNotaFiscalIntegrada = cargaPedidoXMLNotaFiscalParcial.TipoNotaFiscalIntegrada;

                            repCargaPedidoXMLNotaFiscalParcial.Atualizar(cargaPedidoXMLNotaFiscalParcial);
                            repXMLNotaFiscal.Atualizar(xmlNotaFiscal);
                        }
                    }

                    Dominio.Entidades.Cliente emitenteNota = xmlNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida ? xmlNotaFiscal.Emitente : xmlNotaFiscal.Destinatario;

                    if ((emitenteNota.GrupoPessoas != null && emitenteNota.GrupoPessoas.ArmazenaProdutosXMLNFE) || (cargaPedido.Carga.TipoOperacao?.AtualizarProdutosPorXmlNotaFiscal ?? false) || configuracaoEmbarcador.EmitirNFeRemessaNaCarga)
                        new PedidoXMLNotaFiscal(_unitOfWork, configuracaoEmbarcador).ArmazenarProdutosXML(xmlNotaFiscal.XML, xmlNotaFiscal, Auditado, tipoServicoMultisoftware);
                }
            }

            if (!cargaPedido.CTeEmitidoNoEmbarcador)
            {
                new Pessoa.GrupoPessoasObservacaoNfe().AdicionarDadosNfePorGrupoPessoasEmitente(cargaPedido, xmlNotaFiscal.Emitente, xmlNotaFiscal.Observacao, tipoServicoMultisoftware, configuracaoEmbarcador, _unitOfWork, xmlNotaFiscal, Auditado);

                if (notaFiscalIntegracao != null && notaFiscalIntegracao.TipoNotaFiscalIntegrada == TipoNotaFiscalIntegrada.RemessaPallet && cargaPedido.Pedido != null && !configuracaoEmbarcador.RatearNumeroPalletsModeloVeiculoEntrePedidoPorPeso)
                    cargaPedido.Pedido.NumeroPaletesFracionado = xmlNotaFiscal.QuantidadePallets;
            }

            if (configuracaoCanhoto == null)
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto repConfiguracaoCanhoto = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto(_unitOfWork);
                configuracaoCanhoto = repConfiguracaoCanhoto.BuscarConfiguracaoPadrao();
            }

            serCanhoto.SalvarCanhotoNota(xmlNotaFiscal, pedidoXMLNotaFiscal.CargaPedido, cargaPedido.Carga.FreteDeTerceiro && cargaPedido.Carga.Veiculo != null ? cargaPedido.Carga.Veiculo.Proprietario : cargaPedido.Carga.ProvedorOS, cargaPedido.Carga.Motoristas != null ? cargaPedido.Carga.Motoristas.ToList() : repCargaMotorista.BuscarMotoristasPorCarga(cargaPedido.Carga.Codigo), tipoServicoMultisoftware, configuracaoEmbarcador, _unitOfWork, configuracaoCanhoto);

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
            {
                if (!repCargaPedidoXMLNotaFiscalParcial.ExisteCargaParcialSemNotaParaEstaCarga(cargaPedido.Codigo))
                    cargaPedido.Carga.DataRecebimentoUltimaNFe = DateTime.Now.Date;
            }
            else
                cargaPedido.Carga.DataRecebimentoUltimaNFe = DateTime.Now.Date;

            servicoCargaPedido.AlterarDadosSumarizadosCargaPedido(cargaPedido, volumesAnterior, volumes);
            repCargaPedido.Atualizar(cargaPedido);

            repCarga.Atualizar(cargaPedido.Carga);

            if (!cargaPedido.CTeEmitidoNoEmbarcador)
            {
                ValidacaoTipoNotaVenda(cargaPedido, xmlNotaFiscal);

                try
                {
                    servOcorrenciaPedido.ProcessarOcorrenciaPedido(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EventoColetaEntrega.PedidoFaturado, cargaPedido.Pedido, configuracaoEmbarcador, null);//passado null no cliente pois nao é necessario gerar notificacao de ocorrencia pedido para pedido faturado.
                }
                catch (ServicoException excecao)
                {
                    Log.TratarErro($"InserirNotaCargaPedido (Nota: {xmlNotaFiscal.Numero} | Pedido: {cargaPedido.Pedido.NumeroPedidoEmbarcador}): {excecao.Message}");
                }
            }

            return pedidoXMLNotaFiscal;
        }

        public async Task<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> InserirNotaCargaPedidoAsync(Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal tipoNotaFiscal, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, bool notaFiscalEmOutraCarga, bool alteradoTipoDeCarga, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado = null, Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscalChave notaFiscalIntegracao = null, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto configuracaoCanhoto = null, List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotasFiscais = null)
        {
            Servicos.Embarcador.Canhotos.Canhoto serCanhoto = new Canhotos.Canhoto(_unitOfWork);
            Servicos.Embarcador.Pedido.Pedido serPedido = new Servicos.Embarcador.Pedido.Pedido(_unitOfWork);
            Servicos.Embarcador.Pedido.OcorrenciaPedido servOcorrenciaPedido = new Servicos.Embarcador.Pedido.OcorrenciaPedido(_unitOfWork);
            alteradoTipoDeCarga = false;
            Servicos.Embarcador.Carga.CargaPedido servicoCargaPedido = new Servicos.Embarcador.Carga.CargaPedido(_unitOfWork);


            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoEndereco repPedidoEndereco = new Repositorio.Embarcador.Pedidos.PedidoEndereco(_unitOfWork);
            Repositorio.Embarcador.Cargas.TipoDeCarga repTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(_unitOfWork);
            Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = null;
            Repositorio.Embarcador.Cargas.CargaMotorista repCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial repCargaPedidoXMLNotaFiscalParcial = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = await new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(_unitOfWork).BuscarPrimeiroRegistroAsync();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = await new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(_unitOfWork).BuscarPrimeiroRegistroAsync();

            if (cargaPedido.CTeEmitidoNoEmbarcador)
                pedidoXMLNotaFiscal = pedidoXMLNotasFiscais?.Find(o => o.XMLNotaFiscal.Codigo == xmlNotaFiscal.Codigo);
            else
            {
                if (configuracaoEmbarcador.PermiteAdicionarNFeRepetidaParaOutroPedidoCarga)
                    pedidoXMLNotaFiscal = await repPedidoXMLNotaFiscal.BuscarPorNotaFiscalECargaPedidoAsync(xmlNotaFiscal.Codigo, cargaPedido.Codigo);
                else
                    pedidoXMLNotaFiscal = await repPedidoXMLNotaFiscal.BuscarPorNotaFiscalECargaAsync(xmlNotaFiscal.Codigo, cargaPedido.CargaOrigem.Codigo);
            }

            if ((cargaPedido.Carga.TipoOperacao?.ConfiguracaoCarga?.NaoPermitirUsoNotasQueEstaoEmOutraCarga ?? false) && await repPedidoXMLNotaFiscal.VerificarSeExistePorNotaFiscalEmOutraCargaAsync(xmlNotaFiscal.Codigo, cargaPedido.Carga.Codigo))
                return null;

            int codigoTipoDeCarga = 0;
            if (!string.IsNullOrWhiteSpace(xmlNotaFiscal.TipoDeCarga))
            {
                Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoDeCargaConsulta = await repTipoDeCarga.BuscarPorCodigoEmbarcadorAsync(xmlNotaFiscal.TipoDeCarga);
                codigoTipoDeCarga = tipoDeCargaConsulta?.Codigo ?? 0;
            }

            if (cargaPedido.Carga != null && cargaPedido.Carga.TipoOperacao != null
                && cargaPedido.Carga.TipoOperacao.CriarNovoPedidoAoVincularNotaFiscalComDiferentesParticipantes
                && ((cargaPedido.Pedido != null && cargaPedido.Pedido.Remetente != null && cargaPedido.Pedido.Destinatario != null
                && xmlNotaFiscal.Emitente != null && xmlNotaFiscal.Destinatario != null
                && (cargaPedido.Pedido.Remetente.CPF_CNPJ != xmlNotaFiscal.Emitente.CPF_CNPJ || cargaPedido.Pedido.Destinatario.CPF_CNPJ != xmlNotaFiscal.Destinatario.CPF_CNPJ))
                || (codigoTipoDeCarga > 0)))
            {
                int qtdNotasVinculadasCarga = await repPedidoXMLNotaFiscal.ContarNotasVinculadasAsync(cargaPedido.Carga.Codigo, 0, "");
                int codigoCargaPedido = 0;
                string retorno = "";

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoVincular = null;
                if (qtdNotasVinculadasCarga >= 1)
                {
                    cargaPedidoVincular = await repCargaPedido.BuscarPorEntidadeContainerAsync(cargaPedido.Carga.Codigo, cargaPedido.Pedido.Container?.Codigo ?? 0, xmlNotaFiscal.Emitente.CPF_CNPJ, xmlNotaFiscal.Destinatario.CPF_CNPJ, codigoTipoDeCarga);
                    if (cargaPedidoVincular == null)
                        cargaPedidoVincular = await repCargaPedido.BuscarPorEntidadeContainerAsync(cargaPedido.Carga.Codigo, 0, xmlNotaFiscal.Emitente.CPF_CNPJ, xmlNotaFiscal.Destinatario.CPF_CNPJ, codigoTipoDeCarga);
                    if (cargaPedidoVincular == null)
                        cargaPedidoVincular = await repCargaPedido.BuscarPorEntidadeContainerAsync(cargaPedido.Carga.Codigo, cargaPedido.Pedido.Container?.Codigo ?? 0, 0, xmlNotaFiscal.Destinatario.CPF_CNPJ, codigoTipoDeCarga);
                    if (cargaPedidoVincular == null)
                        cargaPedidoVincular = await repCargaPedido.BuscarPorEntidadeContainerAsync(cargaPedido.Carga.Codigo, cargaPedido.Pedido.Container?.Codigo ?? 0, xmlNotaFiscal.Emitente.CPF_CNPJ, 0, codigoTipoDeCarga);
                    if (cargaPedidoVincular == null)
                        cargaPedidoVincular = await repCargaPedido.BuscarPorEntidadeContainerAsync(cargaPedido.Carga.Codigo, 0, 0, 0, codigoTipoDeCarga);
                }

                if (cargaPedidoVincular != null)
                    codigoCargaPedido = cargaPedidoVincular.Codigo;
                else if (codigoTipoDeCarga > 0 && (cargaPedido.Pedido?.Destinatario?.CPF_CNPJ ?? 0d) == (xmlNotaFiscal.Destinatario?.CPF_CNPJ ?? 1d))
                    codigoCargaPedido = cargaPedido.Codigo;

                if (codigoCargaPedido == 0)
                {
                    int.TryParse(Utilidades.String.OnlyNumbers(cargaPedido.Pedido.TaraContainer), out int taraContainer);
                    retorno = Servicos.Embarcador.Carga.CargaPedido.CriarPedidoNormalOuSubcontratacao(DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, taraContainer, cargaPedido.Pedido.Container?.Codigo ?? 0, cargaPedido.Pedido.LacreContainerUm, cargaPedido.Pedido.LacreContainerDois, cargaPedido.Pedido.LacreContainerTres, cargaPedido.Carga.Codigo, xmlNotaFiscal.Emitente.CPF_CNPJ, xmlNotaFiscal.Destinatario.CPF_CNPJ, 0D, cargaPedido.Pedido.NumeroPedidoEmbarcador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedidoVinculado.Normal, _unitOfWork, _unitOfWork.StringConexao, tipoServicoMultisoftware, configuracaoEmbarcador, out codigoCargaPedido, codigoTipoDeCarga, configuracaoGeralCarga);
                }
                if (string.IsNullOrWhiteSpace(retorno) && codigoCargaPedido > 0)
                {
                    Servicos.Embarcador.Hubs.Carga serHubCarga = new Servicos.Embarcador.Hubs.Carga();
                    serHubCarga.InformarCargaAtualizada(cargaPedido.Carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, _unitOfWork.StringConexao);

                    cargaPedido = repCargaPedido.BuscarPorCodigo(codigoCargaPedido);
                }
                else
                {
                    return null;
                }
            }

            if (cargaPedido.Carga != null && cargaPedido.Carga.TipoOperacao != null && cargaPedido.Carga.TipoOperacao.AlterarRemetentePedidoConformeNotaFiscal)
            {
                Dominio.Entidades.Cliente remetenteNota = xmlNotaFiscal.TipoOperacaoNotaFiscal == TipoOperacaoNotaFiscal.Entrada ? xmlNotaFiscal.Destinatario : xmlNotaFiscal.Emitente;
                if (remetenteNota != null && (cargaPedido.Pedido.Remetente == null || remetenteNota.CPF_CNPJ != cargaPedido.Pedido.Remetente.CPF_CNPJ))
                {
                    Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = await repPedido.BuscarPorCodigoAsync(cargaPedido.Pedido.Codigo);
                    pedido.Remetente = remetenteNota;

                    Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco pedidoEnderecoOrigem = new Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco();
                    serPedido.PreecherEnderecoPedido(ref pedidoEnderecoOrigem, pedido.Remetente);
                    if (pedidoEnderecoOrigem != null && pedidoEnderecoOrigem.Localidade != null)
                    {
                        await repPedidoEndereco.InserirAsync(pedidoEnderecoOrigem);
                        pedido.EnderecoOrigem = pedidoEnderecoOrigem;
                    }

                    await repPedido.AtualizarAsync(pedido);

                    if (pedido.Expedidor != null)
                        cargaPedido.Origem = pedido.Expedidor.Localidade;
                    else
                        cargaPedido.Origem = pedido.Remetente.Localidade;

                    await repCargaPedido.AtualizarAsync(cargaPedido);

                    if (Auditado != null)
                    {
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, pedido, null, "Alterou o remetente do pedido automaticamente pela nota fiscal inserida.", _unitOfWork);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaPedido.Carga, null, "Alterou o remetente do pedido automaticamente pela nota fiscal inserida.", _unitOfWork);
                    }
                }
                Dominio.Entidades.Cliente destinatarioNota = xmlNotaFiscal.TipoOperacaoNotaFiscal == TipoOperacaoNotaFiscal.Entrada ? xmlNotaFiscal.Emitente : xmlNotaFiscal.Destinatario;
                if (destinatarioNota != null && (cargaPedido.Pedido.Destinatario == null || destinatarioNota.CPF_CNPJ != cargaPedido.Pedido.Destinatario.CPF_CNPJ))
                {
                    if (cargaPedido.Pedido.Destinatario == null)
                    {
                        Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = await repPedido.BuscarPorCodigoAsync(cargaPedido.Pedido.Codigo);

                        pedido.Destinatario = destinatarioNota;
                        if (pedido.Recebedor == null && pedido.Expedidor == null)
                            pedido.Destino = xmlNotaFiscal.Destinatario.Localidade;

                        await repPedido.AtualizarAsync(pedido);

                        if (pedido.Recebedor != null)
                            cargaPedido.Destino = pedido.Recebedor.Localidade;
                        if (cargaPedido.Destino == null)
                            cargaPedido.Destino = xmlNotaFiscal.Destinatario.Localidade;

                        await repCargaPedido.AtualizarAsync(cargaPedido);


                        if (Auditado != null)
                        {
                            Servicos.Auditoria.Auditoria.Auditar(Auditado, pedido, null, "Alterou o destinatário do pedido automaticamente pela nota fiscal inserida.", _unitOfWork);
                            Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaPedido.Carga, null, "Alterou o destinatário do pedido automaticamente pela nota fiscal inserida.", _unitOfWork);
                        }
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(xmlNotaFiscal.TipoDeCarga))
            {
                Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoDeCarga = await repTipoDeCarga.BuscarPorCodigoEmbarcadorAsync(xmlNotaFiscal.TipoDeCarga);
                if (tipoDeCarga != null && ((cargaPedido.Carga.TipoDeCarga == null || cargaPedido.Carga.TipoDeCarga.Codigo != tipoDeCarga.Codigo) || (cargaPedido.Pedido.TipoDeCarga == null || cargaPedido.Pedido.TipoDeCarga.Codigo != tipoDeCarga.Codigo)))
                {
                    alteradoTipoDeCarga = true;
                    cargaPedido.Carga.TipoDeCarga = tipoDeCarga;
                    cargaPedido.Pedido.TipoDeCarga = tipoDeCarga;
                }
            }

            int volumes = xmlNotaFiscal.Volumes;
            int volumesAnterior = xmlNotaFiscal.Volumes;

            if (pedidoXMLNotaFiscal == null)
            {
                volumesAnterior = 0;

                pedidoXMLNotaFiscal = new Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal
                {
                    XMLNotaFiscal = xmlNotaFiscal,
                    ObservacaoNotaFiscal = xmlNotaFiscal.ObservacaoNotaFiscalParaCTe,
                    CargaPedido = cargaPedido,
                    TipoNotaFiscal = tipoNotaFiscal,
                    NotaFiscalEmOutraCarga = notaFiscalEmOutraCarga
                };

                await repPedidoXMLNotaFiscal.InserirAsync(pedidoXMLNotaFiscal);

                if (configuracaoFinanceiro.GerarDoumentoProvisaoAoReceberNotaFiscal ?? false)
                {
                    if ((configuracaoFinanceiro.NaoPermitirProvisionarSemCalculoFrete ?? false))
                    {
                        if (cargaPedido.Carga.CalculandoFrete)
                            throw new ServicoException($"Não é permitido integrar a nota enquanto a carga está em cálculo de frete");

                        if (cargaPedido.Carga.SituacaoCarga == SituacaoCarga.Nova || (cargaPedido.Carga.SituacaoCarga == SituacaoCarga.CalculoFrete && cargaPedido.Carga.TipoFreteEscolhido == TipoFreteEscolhido.todos))
                            throw new ServicoException($"Não é permitido integrar a nota enquanto a carga não passou pela etapa de frete");
                    }

                    Servicos.Embarcador.Escrituracao.DocumentoProvisao.AdicionarDocumentoParaProvisaoPorCargaPedido(pedidoXMLNotaFiscal, false, tipoServicoMultisoftware, _unitOfWork);
                }

                if (!cargaPedido.CTeEmitidoNoEmbarcador)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial> cargaPedidoXMLNotaFiscaisParciais = await repCargaPedidoXMLNotaFiscalParcial.ConsultarSemNotaAsync(cargaPedido.Codigo);

                    if (cargaPedidoXMLNotaFiscaisParciais.Count > 0)
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial cargaPedidoXMLNotaFiscalParcial = (from obj in cargaPedidoXMLNotaFiscaisParciais where obj.Chave == xmlNotaFiscal.Chave select obj).FirstOrDefault();

                        if (cargaPedidoXMLNotaFiscalParcial == null)
                            cargaPedidoXMLNotaFiscalParcial = (from obj in cargaPedidoXMLNotaFiscaisParciais where obj.Numero == xmlNotaFiscal.Numero || (obj.Pedido == xmlNotaFiscal.NumeroDT && !string.IsNullOrWhiteSpace(xmlNotaFiscal.NumeroDT) && ((obj.XMLNotaFiscal.Emitente.CPF_CNPJ == cargaPedido.Pedido.Remetente.CPF_CNPJ && obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida) || (obj.XMLNotaFiscal.Emitente.CPF_CNPJ == cargaPedido.Pedido.Destinatario.CPF_CNPJ && obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Entrada))) select obj).FirstOrDefault();

                        if (cargaPedidoXMLNotaFiscalParcial != null)
                        {
                            cargaPedidoXMLNotaFiscalParcial.XMLNotaFiscal = xmlNotaFiscal;
                            xmlNotaFiscal.TipoNotaFiscalIntegrada = cargaPedidoXMLNotaFiscalParcial.TipoNotaFiscalIntegrada;

                            await repCargaPedidoXMLNotaFiscalParcial.AtualizarAsync(cargaPedidoXMLNotaFiscalParcial);
                            await repXMLNotaFiscal.AtualizarAsync(xmlNotaFiscal);
                        }
                    }

                    Dominio.Entidades.Cliente emitenteNota = xmlNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida ? xmlNotaFiscal.Emitente : xmlNotaFiscal.Destinatario;

                    if ((emitenteNota.GrupoPessoas != null && emitenteNota.GrupoPessoas.ArmazenaProdutosXMLNFE) || (cargaPedido.Carga.TipoOperacao?.AtualizarProdutosPorXmlNotaFiscal ?? false) || configuracaoEmbarcador.EmitirNFeRemessaNaCarga)
                        await new PedidoXMLNotaFiscal(_unitOfWork, configuracaoEmbarcador).ArmazenarProdutosXMLAsync(xmlNotaFiscal.XML, xmlNotaFiscal, Auditado, tipoServicoMultisoftware);
                }
            }

            if (!cargaPedido.CTeEmitidoNoEmbarcador)
            {
                new Pessoa.GrupoPessoasObservacaoNfe().AdicionarDadosNfePorGrupoPessoasEmitente(cargaPedido, xmlNotaFiscal.Emitente, xmlNotaFiscal.Observacao, tipoServicoMultisoftware, configuracaoEmbarcador, _unitOfWork, xmlNotaFiscal, Auditado);

                if (notaFiscalIntegracao != null && notaFiscalIntegracao.TipoNotaFiscalIntegrada == TipoNotaFiscalIntegrada.RemessaPallet && cargaPedido.Pedido != null && !configuracaoEmbarcador.RatearNumeroPalletsModeloVeiculoEntrePedidoPorPeso)
                    cargaPedido.Pedido.NumeroPaletesFracionado = xmlNotaFiscal.QuantidadePallets;
            }

            if (configuracaoCanhoto == null)
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto repConfiguracaoCanhoto = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto(_unitOfWork);
                configuracaoCanhoto = await repConfiguracaoCanhoto.BuscarConfiguracaoPadraoAsync();
            }

            serCanhoto.SalvarCanhotoNota(xmlNotaFiscal, pedidoXMLNotaFiscal.CargaPedido, cargaPedido.Carga.FreteDeTerceiro && cargaPedido.Carga.Veiculo != null ? cargaPedido.Carga.Veiculo.Proprietario : cargaPedido.Carga.ProvedorOS, cargaPedido.Carga.Motoristas != null ? cargaPedido.Carga.Motoristas.ToList() : repCargaMotorista.BuscarMotoristasPorCarga(cargaPedido.Carga.Codigo), tipoServicoMultisoftware, configuracaoEmbarcador, _unitOfWork, configuracaoCanhoto);

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
            {
                if (!repCargaPedidoXMLNotaFiscalParcial.ExisteCargaParcialSemNotaParaEstaCarga(cargaPedido.Codigo))
                    cargaPedido.Carga.DataRecebimentoUltimaNFe = DateTime.Now.Date;
            }
            else
                cargaPedido.Carga.DataRecebimentoUltimaNFe = DateTime.Now.Date;

            servicoCargaPedido.AlterarDadosSumarizadosCargaPedido(cargaPedido, volumesAnterior, volumes);
            await repCargaPedido.AtualizarAsync(cargaPedido);

            await repCarga.AtualizarAsync(cargaPedido.Carga);

            if (!cargaPedido.CTeEmitidoNoEmbarcador)
            {
                ValidacaoTipoNotaVenda(cargaPedido, xmlNotaFiscal);

                try
                {
                    servOcorrenciaPedido.ProcessarOcorrenciaPedido(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EventoColetaEntrega.PedidoFaturado, cargaPedido.Pedido, configuracaoEmbarcador, null);//passado null no cliente pois nao é necessario gerar notificacao de ocorrencia pedido para pedido faturado.
                }
                catch (ServicoException excecao)
                {
                    Log.TratarErro($"InserirNotaCargaPedido (Nota: {xmlNotaFiscal.Numero} | Pedido: {cargaPedido.Pedido.NumeroPedidoEmbarcador}): {excecao.Message}");
                }
            }

            return pedidoXMLNotaFiscal;
        }

        public string ValidarRegrasNotaVinculoPedido(Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            Dominio.Entidades.Cliente emitenteNota = xmlNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida ? xmlNotaFiscal.Emitente : xmlNotaFiscal.Destinatario;
            Dominio.Entidades.Cliente destinatarioNota = xmlNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida ? xmlNotaFiscal.Destinatario : xmlNotaFiscal.Emitente;

            if (pedido.Remetente != null && emitenteNota != null)
            {
                if (pedido.Remetente.CPF_CNPJ_Formatado != emitenteNota.CPF_CNPJ_Formatado)
                    return "O remetente da nota (" + emitenteNota.NomeCNPJ + ") não é o mesmo do pedido (" + pedido.Remetente.NomeCNPJ + ").";
            }

            if (pedido.Destinatario != null && destinatarioNota != null)
            {
                if (destinatarioNota.CPF_CNPJ != pedido.Destinatario.CPF_CNPJ)
                    return "O destinatário da nota " + xmlNotaFiscal.Numero + " (" + destinatarioNota.CPF_CNPJ_Formatado + ") não é o mesmo do pedido (" + pedido.Destinatario.CPF_CNPJ_Formatado + ").";
            }

            return string.Empty;
        }

        public bool InformarComponentesOperacaoIntercement(out string erro, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacaoConfiguracaoComponentes repComponenteTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacaoConfiguracaoComponentes(_unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoasConfiguracaoComponentesFrete repComponenteGrupoPessoa = new Repositorio.Embarcador.Pessoas.GrupoPessoasConfiguracaoComponentesFrete(_unitOfWork);
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoEspelhoIntercement repPedidoEspelhoIntercement = new Repositorio.Embarcador.Pedidos.PedidoEspelhoIntercement(_unitOfWork);
            erro = "";

            string numeroPedido = xmlNotaFiscal.NumeroDT;
            if (!string.IsNullOrWhiteSpace(numeroPedido))
            {
                Dominio.Entidades.Embarcador.Pedidos.PedidoEspelhoIntercement pedidoEspelho = repPedidoEspelhoIntercement.BuscarPorNumeroRemessaPorCargaPedido(numeroPedido, cargaPedido.Codigo);
                if (pedidoEspelho != null)
                {
                    Dominio.Entidades.Embarcador.Frete.ComponenteFrete componente = null;
                    Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoConfiguracaoComponentes tipoOperacaoConfiguracaoComponentes = null;
                    Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasConfiguracaoComponentesFrete grupoPessoasConfiguracaoComponentes = null;
                    bool incluirICMS = false;
                    bool incluirIntegralmenteContratoFreteTerceiro = false;

                    //TARIFA = Tarifa de frete
                    //Multiplicar pelas TONELADAS(Quilogramas / 1000) da nota fiscal e informar como valor de frete da nota.
                    if (pedidoEspelho.EspelhoIntercement.TARIFA > 0 && xmlNotaFiscal.Peso > 0)
                        xmlNotaFiscal.ValorFrete = (pedidoEspelho.EspelhoIntercement.TARIFA * (xmlNotaFiscal.PesoLiquido / 1000));

                    //DIFPESO = Adicional de frete por carregamento abaixo do peso lotação
                    //Deve adicionar como um componente na nota fiscal / carga.
                    //Este componente não deve ser incluso na base de cálculo do ICMS.
                    if (pedidoEspelho.EspelhoIntercement.DIFPESO > 0)
                    {
                        if (cargaPedido.Carga?.TipoOperacao != null)
                            tipoOperacaoConfiguracaoComponentes = repComponenteTipoOperacao.BuscarPorOutraDescricaoCTe(cargaPedido.Carga.TipoOperacao.Codigo, "DIFPESO");
                        if (componente == null && cargaPedido.Carga?.GrupoPessoaPrincipal != null)
                            grupoPessoasConfiguracaoComponentes = repComponenteGrupoPessoa.BuscarPorOutraDescricaoCTe(cargaPedido.Carga.GrupoPessoaPrincipal.Codigo, "DIFPESO");

                        if (tipoOperacaoConfiguracaoComponentes != null && tipoOperacaoConfiguracaoComponentes.ComponenteFrete != null)
                        {
                            componente = tipoOperacaoConfiguracaoComponentes.ComponenteFrete;
                            incluirICMS = tipoOperacaoConfiguracaoComponentes.IncluirICMS;
                            incluirIntegralmenteContratoFreteTerceiro = tipoOperacaoConfiguracaoComponentes.IncluirIntegralmenteContratoFreteTerceiro;
                        }
                        else if (grupoPessoasConfiguracaoComponentes != null && grupoPessoasConfiguracaoComponentes.ComponenteFrete != null)
                        {
                            componente = grupoPessoasConfiguracaoComponentes.ComponenteFrete;
                            incluirICMS = grupoPessoasConfiguracaoComponentes.IncluirICMS;
                            incluirIntegralmenteContratoFreteTerceiro = grupoPessoasConfiguracaoComponentes.IncluirIntegralmenteContratoFreteTerceiro;
                        }
                        if (componente == null)
                            componente = repComponenteFrete.BuscarPorCodigoIntegracao("DIFPESO");
                        if (componente == null)
                        {
                            erro = "Não foi encontrado um componente de frete do tipo DIFPESO para inclusão do valor na carga.";
                            return false;
                        }
                        AdicionarComponenteObrigatorioNotaFiscal(xmlNotaFiscal, componente, pedidoEspelho.EspelhoIntercement.DIFPESO, incluirICMS, incluirIntegralmenteContratoFreteTerceiro);
                    }

                    //CHAPA = Adicional para contratação de ajudante
                    //Deve adicionar como um componente na nota fiscal/ carga.
                    //Este componente não deve ser incluso na base de cálculo do ICMS.
                    if (pedidoEspelho.EspelhoIntercement.CHAPA > 0)
                    {
                        if (cargaPedido.Carga?.TipoOperacao != null)
                            tipoOperacaoConfiguracaoComponentes = repComponenteTipoOperacao.BuscarPorOutraDescricaoCTe(cargaPedido.Carga.TipoOperacao.Codigo, "CHAPA");
                        if (componente == null && cargaPedido.Carga?.GrupoPessoaPrincipal != null)
                            grupoPessoasConfiguracaoComponentes = repComponenteGrupoPessoa.BuscarPorOutraDescricaoCTe(cargaPedido.Carga.GrupoPessoaPrincipal.Codigo, "CHAPA");

                        if (tipoOperacaoConfiguracaoComponentes != null && tipoOperacaoConfiguracaoComponentes.ComponenteFrete != null)
                        {
                            componente = tipoOperacaoConfiguracaoComponentes.ComponenteFrete;
                            incluirICMS = tipoOperacaoConfiguracaoComponentes.IncluirICMS;
                            incluirIntegralmenteContratoFreteTerceiro = tipoOperacaoConfiguracaoComponentes.IncluirIntegralmenteContratoFreteTerceiro;
                        }
                        else if (grupoPessoasConfiguracaoComponentes != null && grupoPessoasConfiguracaoComponentes.ComponenteFrete != null)
                        {
                            componente = grupoPessoasConfiguracaoComponentes.ComponenteFrete;
                            incluirICMS = grupoPessoasConfiguracaoComponentes.IncluirICMS;
                            incluirIntegralmenteContratoFreteTerceiro = grupoPessoasConfiguracaoComponentes.IncluirIntegralmenteContratoFreteTerceiro;
                        }

                        if (componente == null)
                            componente = repComponenteFrete.BuscarPorCodigoIntegracao("CHAPA");
                        if (componente == null)
                        {
                            erro = "Não foi encontrado um componente de frete do tipo CHAPA para inclusão do valor na carga.";
                            return false;
                        }
                        AdicionarComponenteObrigatorioNotaFiscal(xmlNotaFiscal, componente, pedidoEspelho.EspelhoIntercement.CHAPA, incluirICMS, incluirIntegralmenteContratoFreteTerceiro);
                    }

                    //OUTROS = Adicional de frete
                    //Deve adicionar como um componente na nota fiscal/ carga.
                    //Este componente não deve ser incluso na base de cálculo do ICMS.
                    if (pedidoEspelho.EspelhoIntercement.OUTROS > 0)
                    {
                        if (cargaPedido.Carga?.TipoOperacao != null)
                            tipoOperacaoConfiguracaoComponentes = repComponenteTipoOperacao.BuscarPorOutraDescricaoCTe(cargaPedido.Carga.TipoOperacao.Codigo, "OUTROS");
                        if (componente == null && cargaPedido.Carga?.GrupoPessoaPrincipal != null)
                            grupoPessoasConfiguracaoComponentes = repComponenteGrupoPessoa.BuscarPorOutraDescricaoCTe(cargaPedido.Carga.GrupoPessoaPrincipal.Codigo, "OUTROS");

                        if (tipoOperacaoConfiguracaoComponentes != null && tipoOperacaoConfiguracaoComponentes.ComponenteFrete != null)
                        {
                            componente = tipoOperacaoConfiguracaoComponentes.ComponenteFrete;
                            incluirICMS = tipoOperacaoConfiguracaoComponentes.IncluirICMS;
                            incluirIntegralmenteContratoFreteTerceiro = tipoOperacaoConfiguracaoComponentes.IncluirIntegralmenteContratoFreteTerceiro;
                        }
                        else if (grupoPessoasConfiguracaoComponentes != null && grupoPessoasConfiguracaoComponentes.ComponenteFrete != null)
                        {
                            componente = grupoPessoasConfiguracaoComponentes.ComponenteFrete;
                            incluirICMS = grupoPessoasConfiguracaoComponentes.IncluirICMS;
                            incluirIntegralmenteContratoFreteTerceiro = grupoPessoasConfiguracaoComponentes.IncluirIntegralmenteContratoFreteTerceiro;
                        }

                        if (componente == null)
                            componente = repComponenteFrete.BuscarPorCodigoIntegracao("OUTROS");
                        if (componente == null)
                        {
                            erro = "Não foi encontrado um componente de frete do tipo OUTROS para inclusão do valor na carga.";
                            return false;
                        }
                        AdicionarComponenteObrigatorioNotaFiscal(xmlNotaFiscal, componente, pedidoEspelho.EspelhoIntercement.OUTROS, incluirICMS, incluirIntegralmenteContratoFreteTerceiro);
                    }


                    //PISCOFINS = Adicional de frete
                    //Deve adicionar como um componente na nota fiscal/ carga.
                    //Este componente não deve ser incluso na base de cálculo do ICMS.
                    if (pedidoEspelho.EspelhoIntercement.PISCOFINS > 0)
                    {
                        if (cargaPedido.Carga?.TipoOperacao != null)
                            tipoOperacaoConfiguracaoComponentes = repComponenteTipoOperacao.BuscarPorOutraDescricaoCTe(cargaPedido.Carga.TipoOperacao.Codigo, "PISCOFINS");
                        if (componente == null && cargaPedido.Carga?.GrupoPessoaPrincipal != null)
                            grupoPessoasConfiguracaoComponentes = repComponenteGrupoPessoa.BuscarPorOutraDescricaoCTe(cargaPedido.Carga.GrupoPessoaPrincipal.Codigo, "PISCOFINS");

                        if (tipoOperacaoConfiguracaoComponentes != null && tipoOperacaoConfiguracaoComponentes.ComponenteFrete != null)
                        {
                            componente = tipoOperacaoConfiguracaoComponentes.ComponenteFrete;
                            incluirICMS = tipoOperacaoConfiguracaoComponentes.IncluirICMS;
                            incluirIntegralmenteContratoFreteTerceiro = tipoOperacaoConfiguracaoComponentes.IncluirIntegralmenteContratoFreteTerceiro;
                        }
                        else if (grupoPessoasConfiguracaoComponentes != null && grupoPessoasConfiguracaoComponentes.ComponenteFrete != null)
                        {
                            componente = grupoPessoasConfiguracaoComponentes.ComponenteFrete;
                            incluirICMS = grupoPessoasConfiguracaoComponentes.IncluirICMS;
                            incluirIntegralmenteContratoFreteTerceiro = grupoPessoasConfiguracaoComponentes.IncluirIntegralmenteContratoFreteTerceiro;
                        }

                        if (componente == null)
                            componente = repComponenteFrete.BuscarPorCodigoIntegracao("PISCOFINS");
                        if (componente == null)
                        {
                            erro = "Não foi encontrado um componente de frete do tipo PISCOFINS para inclusão do valor na carga.";
                            return false;
                        }
                        AdicionarComponenteObrigatorioNotaFiscal(xmlNotaFiscal, componente, pedidoEspelho.EspelhoIntercement.PISCOFINS, incluirICMS, incluirIntegralmenteContratoFreteTerceiro);
                    }
                }
            }

            return true;
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedido CriarCargaPorNotaFiscalPorRegraAutomatizacaoEmail(Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xMLNotaFiscal, ref string mensagem, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracoes.RegraAutomatizacaoEmissoesEmail regraAutomatizacaoEmissoesEmail, string stringConexao)
        {
            StringBuilder stMensagem = new StringBuilder();
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaLocaisPrestacao repCargaLocaisPrestacao = new Repositorio.Embarcador.Cargas.CargaLocaisPrestacao(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(_unitOfWork).BuscarConfiguracaoPadrao();

            Servicos.WebService.Carga.Pedido serPedidoWS = new Servicos.WebService.Carga.Pedido(_unitOfWork);
            Servicos.WebService.Carga.Carga serCargaWS = new Servicos.WebService.Carga.Carga(_unitOfWork);
            Servicos.WebService.Carga.ProdutosPedido serProdutoPedidoWS = new Servicos.WebService.Carga.ProdutosPedido(_unitOfWork);
            Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(_unitOfWork);
            Servicos.Embarcador.Carga.CargaLocaisPrestacao serCargaLocaisPrestacao = new Servicos.Embarcador.Carga.CargaLocaisPrestacao(_unitOfWork);
            Servicos.Embarcador.Carga.Rota serCargaRota = new Servicos.Embarcador.Carga.Rota(_unitOfWork);

            int codigoCargaExistente = 0;
            int protocoloPedidoExistente = 0;

            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = null;

            Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao = ConverterXMLNotaEmCargaIntegracao(xMLNotaFiscal, configuracaoTMS.UtilizarValorFreteNota);

            if (string.IsNullOrWhiteSpace(xMLNotaFiscal.CNPJTranposrtador))
            {
                mensagem = "A nota fiscal (" + xMLNotaFiscal.Chave + ") não possui transportador informado ";
                return null;
            }

            Dominio.Entidades.Embarcador.Filiais.Filial filial = repFilial.buscarPorCodigoEmbarcador(cargaIntegracao.Filial?.CodigoIntegracao ?? "");
            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = regraAutomatizacaoEmissoesEmail.TipoOperacao;

            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = serPedidoWS.CriarPedido(cargaIntegracao, filial, tipoOperacao, ref stMensagem, tipoServicoMultisoftware, ref protocoloPedidoExistente, ref codigoCargaExistente, false);
            if (stMensagem.Length == 0 || protocoloPedidoExistente > 0)
            {
                if (protocoloPedidoExistente == 0)
                    serProdutoPedidoWS.AdicionarProdutosPedido(pedido, configuracaoTMS, cargaIntegracao, ref stMensagem, _unitOfWork);

                cargaPedido = serCargaWS.CriarCarga(pedido, cargaIntegracao, ref protocoloPedidoExistente, ref stMensagem, ref codigoCargaExistente, _unitOfWork, tipoServicoMultisoftware, false, false, null, configuracaoTMS, null, "", filial, tipoOperacao);
                int codCarga = cargaPedido != null ? cargaPedido.Carga.Codigo : 0;

                List<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao> cargaLocaisPrestacao = codCarga > 0 ? repCargaLocaisPrestacao.BuscarPorCarga(codCarga) : new List<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao>();

                if (cargaPedido != null)
                {
                    bool criouNovo = true;
                    serCargaLocaisPrestacao.CriarLocalPrestacao(cargaPedido, cargaLocaisPrestacao, _unitOfWork, out criouNovo);
                    if (criouNovo)
                        serCargaRota.CriarRota(cargaPedido.Carga, tipoServicoMultisoftware, _unitOfWork, configuracaoPedido);

                    serCargaWS.AdicionarProdutosCarga(cargaPedido, cargaIntegracao, ref stMensagem, _unitOfWork, configuracaoTMS.UsarPesoProdutoSumarizacaoCarga);
                    if (cargaIntegracao.FecharCargaAutomaticamente)
                    {
                        if (!cargaPedido.Carga.DataCarregamentoCarga.HasValue)
                            cargaPedido.Carga.DataCarregamentoCarga = DateTime.Now;

                        serCarga.FecharCarga(cargaPedido.Carga, _unitOfWork, tipoServicoMultisoftware, null);
                        Servicos.Log.TratarErro("12 - Fechou Carga (" + cargaPedido.Carga.Codigo + ") " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "FechamentoCarga");
                        cargaPedido.Carga.CargaFechada = true;

                        if (cargaPedido.Carga.ExigeNotaFiscalParaCalcularFrete && !(cargaPedido.Carga.TipoOperacao?.PermiteImportarDocumentosManualmente ?? false) && !(cargaPedido.Carga.TipoOperacao?.ExigirConfirmacaoDadosTransportadorAvancarCarga ?? false))
                        {
                            cargaPedido.Carga.MotivoPendenciaFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoPendenciaFrete.NenhumPendencia;
                            cargaPedido.Carga.PossuiPendencia = false;
                            cargaPedido.Carga.MotivoPendencia = "";
                            cargaPedido.Carga.CalculandoFrete = true;
                            cargaPedido.Carga.DataInicioCalculoFrete = DateTime.Now;
                            cargaPedido.Carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete;
                            Servicos.Log.TratarErro("Atualizou a situação para calculo frete 1 Carga: " + cargaPedido.Carga.CodigoCargaEmbarcador, "AtualizouSituacaoCalculoFrete");
                        }

                        if (!cargaPedido.Carga.DataEnvioUltimaNFe.HasValue && cargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova)
                        {
                            if (!cargaPedido.Carga.TipoOperacao.PermiteImportarDocumentosManualmente || cargaPedido.Carga.TipoOperacao.NaoExigeConformacaoDasNotasEmissao)
                            {
                                cargaPedido.Carga.DataEnvioUltimaNFe = DateTime.Now;
                                cargaPedido.Carga.DataRecebimentoUltimaNFe = DateTime.Now;
                                cargaPedido.SituacaoEmissao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNF.NFEnviada;
                                repCargaPedido.Atualizar(cargaPedido);
                            }
                        }

                        repCarga.Atualizar(cargaPedido.Carga);
                    }
                }
            }

            if (stMensagem.Length > 0)
                mensagem = stMensagem.ToString();

            return cargaPedido;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> GerarPedidosPorNotasFiscaisEVincularNaCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> xmlsNotaFiscal, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado)
        {
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(_unitOfWork);

            Servicos.WebService.Carga.Pedido serPedidoWS = new Servicos.WebService.Carga.Pedido(_unitOfWork);

            List<Dominio.Entidades.Localidade> destinos = xmlsNotaFiscal.Select(o => o.Destinatario.Localidade).Distinct().ToList();
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosAdicionados = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(_unitOfWork).BuscarPrimeiroRegistro();

            //Agrupa as notas por Destino, gerando apenas um pedido caso tenho várias notas para o mesmo destino
            foreach (Dominio.Entidades.Localidade destino in destinos)
            {
                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> xmlsNotaFiscalPorDestino = xmlsNotaFiscal.Where(o => o.Destinatario.Localidade.Codigo == destino.Codigo).ToList();
                Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = xmlsNotaFiscalPorDestino.FirstOrDefault();

                Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao = ConverterXMLNotaEmCargaIntegracao(xmlNotaFiscal, configuracaoTMS.UtilizarValorFreteNota);

                StringBuilder stMensagem = new StringBuilder();
                int codigoCargaExistente = 0;
                int protocoloPedidoExistente = 0;

                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = serPedidoWS.CriarPedido(cargaIntegracao, carga.Filial, carga.TipoOperacao, ref stMensagem, tipoServicoMultisoftware, ref protocoloPedidoExistente, ref codigoCargaExistente, false);

                if (stMensagem.Length > 0)
                    throw new ServicoException(stMensagem.ToString());

                Servicos.Auditoria.Auditoria.Auditar(Auditado, pedido, "Adicionou pedido via seleção de notas avulsas na carga", _unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = Servicos.Embarcador.Carga.CargaPedido.CriarCargaPedido(carga, pedido, null, _unitOfWork, _unitOfWork.StringConexao, tipoServicoMultisoftware, configuracaoTMS, false, configuracaoGeralCarga);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, "Adicionou pedido criado via seleção de notas avulsar na carga", _unitOfWork);

                cargaPedidosAdicionados.Add(cargaPedido);

                if (pedido.NotasFiscais == null)
                    pedido.NotasFiscais = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();

                foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal nota in xmlsNotaFiscalPorDestino)
                {
                    pedido.NotasFiscais.Add(nota);

                    InserirNotaCargaPedido(nota, cargaPedido, tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.Venda, configuracaoTMS, false, out bool alteradoTipoDeCarga, Auditado);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, xmlNotaFiscal, null, "Adicionado via seleção de notas avulsas na carga", _unitOfWork);

                    xmlNotaFiscal.SemCarga = false;
                    repXmlNotaFiscal.Atualizar(xmlNotaFiscal);
                }

                repositorioPedido.Atualizar(pedido);
            }

            return cargaPedidosAdicionados;
        }

        public bool ValidarExisteFaturaFakeNaCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(_unitOfWork);
            return repXMLNotaFiscal.ExisteFacturaFakePorCarga(carga.Codigo);
        }

        public void VincularNotasFiscaisAosPedidosDaCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                return;

            if (!(carga.TipoOperacao?.ConfiguracaoCarga?.DisponibilizarNotaFiscalNoPedidoAoFinalizarCarga ?? false))
                return;

            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> notasFiscaisDosPedidos;

            if (carga.TipoOperacao.ConfiguracaoCarga.LiberarPedidoComRecebedorParaMontagemCarga)
                notasFiscaisDosPedidos = repositorioPedidoXMLNotaFiscal.BuscarPorCargaComRecebedor(carga.Codigo);
            else
                notasFiscaisDosPedidos = repositorioPedidoXMLNotaFiscal.BuscarPorCargaComExpedidor(carga.Codigo);

            if (notasFiscaisDosPedidos.Count == 0)
                return;

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> notasFiscaisDoPedido = notasFiscaisDosPedidos.Where(o => o.CargaPedido.Codigo == cargaPedido.Codigo).ToList();
                if (notasFiscaisDoPedido.Count == 0)
                    continue;

                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido;

                if (carga.TipoOperacao.ConfiguracaoCarga.LiberarPedidoComRecebedorParaMontagemCarga)
                    pedido = LiberarPedidoRecebedor(cargaPedido, carga);
                else
                    pedido = LiberarPedidoExpedidor(cargaPedido, carga);

                if (pedido == null)
                    continue;

                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal notaFiscalDoPedido in notasFiscaisDoPedido)
                {
                    if (pedido.NotasFiscais == null)
                        pedido.NotasFiscais = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();

                    if (!pedido.NotasFiscais.Any(o => o.Codigo == notaFiscalDoPedido.XMLNotaFiscal.Codigo))
                        pedido.NotasFiscais.Add(notaFiscalDoPedido.XMLNotaFiscal);
                }

                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = pedido.GetChanges();
                if (alteracoes.Count > 0)
                {
                    SalvarLogAlteracoesAuditoriaPedidoVincularNotasFiscaisAosPedidosDaCarga(pedido, alteracoes);
                    Servicos.Auditoria.Auditoria.Auditar(auditado, pedido, alteracoes, $"Pedido alterado para permitir montagem da carga do próximo trecho.", _unitOfWork);
                }

                repositorioPedido.Atualizar(pedido);
            }
        }

        private void SalvarLogAlteracoesAuditoriaPedidoVincularNotasFiscaisAosPedidosDaCarga(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes)
        {
            string alteracoesAuditorias = string.Empty;
            for (int i = 0; i < alteracoes.Count; i++)
            {
                var alteracao = alteracoes[i];
                alteracoesAuditorias += $"prop: {alteracao.Propriedade}, de: {alteracao.De}, para: {alteracao.Para}\n";
            }
            Servicos.Log.GravarDebug($@"As seguintes informações do pedido de código: {pedido.Codigo} e de número: {pedido.NumeroPedidoEmbarcador} foram auditadas:
                                                {alteracoesAuditorias}", "VincularNotasFiscaisAosPedidosDaCarga");
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private void AtualizarRemetenteDoPedidoPelaNotaFiscal(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido, bool cargaCargoX, Dominio.Entidades.Cliente emitenteNota)
        {
            if (configuracaoPedido.BloquearInsercaoNotaComEmitenteDiferenteRemetentePedido)
                throw new ServicoException("O emitente da nota (" + emitenteNota.NomeCNPJ + ") não é o mesmo do remetente do pedido (" + cargaPedido.Pedido.Remetente.NomeCNPJ + ").");

            if (cargaPedido.Carga.TipoOperacao?.CriarNovoPedidoAoVincularNotaFiscalComDiferentesParticipantes ?? false)
                return;

            Repositorio.Embarcador.Pedidos.PedidoEndereco repPedidoEndereco = new Repositorio.Embarcador.Pedidos.PedidoEndereco(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            WebService.Carga.Pedido serWSPedido = new WebService.Carga.Pedido(_unitOfWork);

            if (cargaCargoX || emitenteNota.GrupoPessoas == null || !emitenteNota.GrupoPessoas.ValidaEmitenteNFe || (cargaPedido.Carga.TipoOperacao?.ValidarNotaFiscalPeloDestinatario ?? false) || configuracao.UtilizaEmissaoMultimodal)
            {
                if (cargaCargoX || cargaPedido.Pedido.Remetente.Localidade.Codigo == emitenteNota.Localidade.Codigo || cargaPedido.CTeEmitidoNoEmbarcador ||
                    (cargaPedido.Pedido.Remetente.GrupoPessoas != null && emitenteNota.GrupoPessoas != null && cargaPedido.Pedido.Remetente.GrupoPessoas.Codigo == emitenteNota.GrupoPessoas.Codigo) ||
                    (cargaPedido.Carga.TipoOperacao?.ValidarNotaFiscalPeloDestinatario ?? false))// se for do mesmo grupo de pessoas libera e troca o remetente, pois pode ser que foi lançado o remetente diferente da nota pelo operador.
                {
                    if (cargaPedido.Pedido.Remetente.Localidade.Codigo != emitenteNota.Localidade.Codigo)
                        cargaPedido.Origem = emitenteNota.Localidade;

                    Servicos.Log.TratarErro($"2 - Pedido (Código: {cargaPedido.Pedido.Codigo} - Número: {cargaPedido.Pedido.NumeroPedidoEmbarcador}) trocou de remetente de {cargaPedido.Pedido.Remetente.Codigo} para {emitenteNota.Codigo}.", "TrocaRemetentePedido");
                    cargaPedido.Pedido.Remetente = emitenteNota;
                    cargaPedido.Pedido.Origem = emitenteNota.Localidade;

                    if (!cargaPedido.Pedido.UsarOutroEnderecoOrigem && cargaPedido.Pedido.EnderecoOrigem != null)
                    {
                        serWSPedido.PreecherEnderecoPedidoPorCliente(cargaPedido.Pedido.EnderecoOrigem, cargaPedido.Pedido.Remetente);

                        repPedidoEndereco.Atualizar(cargaPedido.Pedido.EnderecoOrigem);
                    }

                    repPedido.Atualizar(cargaPedido.Pedido);
                }
                else
                {
                    if (!configuracao.NaoValidarRemetenteNotaComRemetentePedido && (cargaPedido.Pedido.Remetente.GrupoPessoas == null || emitenteNota.GrupoPessoas == null || emitenteNota.GrupoPessoas.Codigo != cargaPedido.Pedido.Remetente.GrupoPessoas.Codigo)) //se o grupo de pessoas é o mesmo permite coletar em mais de um CNPJ do grupo autoriza a nota
                        throw new ServicoException("O remetente da nota (" + emitenteNota.NomeCNPJ + ") não é o mesmo do pedido (" + cargaPedido.Pedido.Remetente.NomeCNPJ + ").");
                }
            }
            else
            {
                if (!configuracao.NaoValidarRemetenteNotaComRemetentePedido)
                    throw new ServicoException("O remetente da nota (" + emitenteNota.NomeCNPJ + ") não é o mesmo do pedido (" + cargaPedido.Pedido.Remetente.NomeCNPJ + ").");
            }
        }

        private DateTime? ObterDataPorFormatoEntrada(string data)
        {
            if (string.IsNullOrWhiteSpace(data))
                return null;

            DateTime? dataConversao = null;

            string pattern;

            if (data.Length == 10)
                pattern = "dd/MM/yyyy";
            else if (data.Length == 8)
                pattern = "ddMMyyyy";
            else
                pattern = "dd/MM/yyyy HH:mm:ss";

            if (DateTime.TryParseExact(data, pattern, null, System.Globalization.DateTimeStyles.None, out DateTime auxDataConversao))
                dataConversao = auxDataConversao;

            return dataConversao;
        }

        private bool AjustarComponentesObrigatoriosNotaFiscal(out string erro, Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal, Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal notaFiscal)
        {
            Repositorio.Embarcador.Pedidos.XMLNotaFiscalComponente repXMLNotaFiscalComponente = new Repositorio.Embarcador.Pedidos.XMLNotaFiscalComponente(_unitOfWork);
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(_unitOfWork);

            RemoverTodosOsComponentesObrigatoriosNotaFiscal(xmlNotaFiscal);

            if (notaFiscal.ValorComponenteAdValorem > 0m)
            {
                Dominio.Entidades.Embarcador.Frete.ComponenteFrete componente = repComponenteFrete.BuscarPorCodigoIntegracao("ADVALOREM");

                if (componente == null)
                {
                    erro = "Não foi encontrado um componente de frete do tipo ADValorem para inclusão do valor na carga.";
                    return false;
                }

                AdicionarComponenteObrigatorioNotaFiscal(xmlNotaFiscal, componente, notaFiscal.ValorComponenteAdValorem);
            }

            if (notaFiscal.ValorComponenteDescarga > 0m)
            {
                Dominio.Entidades.Embarcador.Frete.ComponenteFrete componente = repComponenteFrete.BuscarPorCodigoIntegracao("DESCARGA");

                if (componente == null)
                {
                    erro = "Não foi encontrado um componente de frete do tipo descarga para inclusão do valor na carga.";
                    return false;
                }

                AdicionarComponenteObrigatorioNotaFiscal(xmlNotaFiscal, componente, notaFiscal.ValorComponenteDescarga);
            }

            if (notaFiscal.ValorComponenteFreteCrossDocking > 0m)
            {
                Dominio.Entidades.Embarcador.Frete.ComponenteFrete componente = repComponenteFrete.BuscarPorCodigoIntegracao("CROSSDOCKING");

                if (componente == null)
                {
                    erro = "Não foi encontrado um componente de frete do tipo Frete Cross Docking para inclusão do valor na carga.";
                    return false;
                }

                AdicionarComponenteObrigatorioNotaFiscal(xmlNotaFiscal, componente, notaFiscal.ValorComponenteFreteCrossDocking);
            }

            if (notaFiscal.ValorComponentePedagio > 0m)
            {
                Dominio.Entidades.Embarcador.Frete.ComponenteFrete componente = repComponenteFrete.BuscarPorCodigoIntegracao("PEDAGIO");

                if (componente == null)
                {
                    erro = "Não foi encontrado um componente de frete do tipo Pedágio para inclusão do valor na carga.";
                    return false;
                }

                AdicionarComponenteObrigatorioNotaFiscal(xmlNotaFiscal, componente, notaFiscal.ValorComponentePedagio);
            }

            erro = string.Empty;
            return true;
        }

        private void RemoverTodosOsComponentesObrigatoriosNotaFiscal(Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal)
        {
            Repositorio.Embarcador.Pedidos.XMLNotaFiscalComponente repXMLNotaFiscalComponente = new Repositorio.Embarcador.Pedidos.XMLNotaFiscalComponente(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalComponente> componentesExistentes = repXMLNotaFiscalComponente.BuscarPorXMLNotaFiscal(xmlNotaFiscal.Codigo);

            foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalComponente componente in componentesExistentes)
                repXMLNotaFiscalComponente.Deletar(componente);
        }

        private void AdicionarComponenteObrigatorioNotaFiscal(Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete, decimal valor, bool? incluirICMS = null, bool? incluirIntegralmenteContratoFreteTerceiro = null)
        {
            Repositorio.Embarcador.Pedidos.XMLNotaFiscalComponente repXMLNotaFiscalComponente = new Repositorio.Embarcador.Pedidos.XMLNotaFiscalComponente(_unitOfWork);
            Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalComponente xmlNotaFiscalComponente = repXMLNotaFiscalComponente.BuscarPorXMLNotaFiscalEComponenteFrete(xmlNotaFiscal.Codigo, componenteFrete.Codigo);

            if (xmlNotaFiscalComponente == null)
                xmlNotaFiscalComponente = new Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalComponente();

            xmlNotaFiscalComponente.ComponenteFrete = componenteFrete;
            xmlNotaFiscalComponente.Valor = valor;
            xmlNotaFiscalComponente.XMLNotaFiscal = xmlNotaFiscal;
            xmlNotaFiscalComponente.IncluirICMS = incluirICMS;
            xmlNotaFiscalComponente.IncluirIntegralmenteContratoFreteTerceiro = incluirIntegralmenteContratoFreteTerceiro;

            if (xmlNotaFiscalComponente.Codigo > 0)
                repXMLNotaFiscalComponente.Atualizar(xmlNotaFiscalComponente);
            else
                repXMLNotaFiscalComponente.Inserir(xmlNotaFiscalComponente);
        }

        private Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao ConverterXMLNotaEmCargaIntegracao(Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal, bool utilizarValorFreteNota)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork);
            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repTipoOperacao.BuscarTipoOperacaoPadraoQuandoNaoInformadaNaIntegracao();

            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);


            Servicos.WebService.Pessoas.Pessoa serPessoa = new WebService.Pessoas.Pessoa(_unitOfWork);
            Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao = new Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao();
            cargaIntegracao.DataCriacaoCarga = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            cargaIntegracao.Destinatario = serPessoa.ConverterObjetoPessoa(xmlNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida ? xmlNotaFiscal.Destinatario : xmlNotaFiscal.Emitente);
            cargaIntegracao.Remetente = serPessoa.ConverterObjetoPessoa(xmlNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida ? xmlNotaFiscal.Emitente : xmlNotaFiscal.Destinatario);

            cargaIntegracao.FecharCargaAutomaticamente = true;
            cargaIntegracao.Filial = new Dominio.ObjetosDeValor.Embarcador.Filial.Filial() { CodigoIntegracao = xmlNotaFiscal.Emitente.CPF_CNPJ_SemFormato };

            Dominio.Entidades.Empresa empresa = null;
            if (!string.IsNullOrWhiteSpace(xmlNotaFiscal.CNPJTranposrtador))
            {
                string cnpj = Utilidades.String.OnlyNumbers(xmlNotaFiscal.CNPJTranposrtador);
                empresa = repEmpresa.BuscarPorCNPJ(cnpj);
            }

            if (!string.IsNullOrWhiteSpace(xmlNotaFiscal.PlacaVeiculoNotaFiscal) && empresa != null)
            {
                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorPlaca(empresa.Codigo, xmlNotaFiscal.PlacaVeiculoNotaFiscal);
                if (veiculo != null && veiculo.ModeloVeicularCarga != null)
                    cargaIntegracao.ModeloVeicular = new Dominio.ObjetosDeValor.Embarcador.Carga.ModeloVeicular() { CodigoIntegracao = veiculo.ModeloVeicularCarga.CodigoIntegracao };
                if (veiculo != null)
                    cargaIntegracao.Veiculo = new Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo() { Placa = xmlNotaFiscal.PlacaVeiculoNotaFiscal };
            }

            cargaIntegracao.NumeroCarga = xmlNotaFiscal.NumeroDT;
            cargaIntegracao.NumeroPedidoEmbarcador = xmlNotaFiscal.NumeroDT;
            if (string.IsNullOrWhiteSpace(cargaIntegracao.NumeroPedidoEmbarcador))
            {
                cargaIntegracao.NumeroPedidoEmbarcador = xmlNotaFiscal.Numero.ToString();
                cargaIntegracao.NumeroCarga = xmlNotaFiscal.Numero.ToString();
            }


            //cargaIntegracao.PesoBruto = xmlNotaFiscal.Peso;
            if (tipoOperacao != null)
            {
                cargaIntegracao.TipoOperacao = new Dominio.ObjetosDeValor.Embarcador.Carga.TipoOperacao() { CodigoIntegracao = tipoOperacao.CodigoIntegracao };
                if (tipoOperacao.TipoDeCargaPadraoOperacao != null)
                    cargaIntegracao.TipoCargaEmbarcador = new Dominio.ObjetosDeValor.Embarcador.Carga.TipoCargaEmbarcador() { CodigoIntegracao = tipoOperacao.TipoDeCargaPadraoOperacao.CodigoTipoCargaEmbarcador };
            }

            if (xmlNotaFiscal.ModalidadeFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.A_Pagar)
            {
                cargaIntegracao.TipoPagamento = Dominio.Enumeradores.TipoPagamento.A_Pagar;
                cargaIntegracao.TipoTomador = Dominio.Enumeradores.TipoTomador.Destinatario;
            }
            else
            {
                cargaIntegracao.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Pago;
                cargaIntegracao.TipoTomador = Dominio.Enumeradores.TipoTomador.Remetente;
            }

            cargaIntegracao.ProdutoPredominante = new Dominio.ObjetosDeValor.Embarcador.Pedido.Produto();
            cargaIntegracao.ProdutoPredominante.CodigoGrupoProduto = "1";
            cargaIntegracao.ProdutoPredominante.DescricaoGrupoProduto = "Diversos";
            cargaIntegracao.ProdutoPredominante.CodigoProduto = "1";
            cargaIntegracao.ProdutoPredominante.DescricaoProduto = "Diversos";

            if (utilizarValorFreteNota && (xmlNotaFiscal.ValorFrete > 0m || (tipoOperacao?.PermitirValorFreteInformadoPeloEmbarcadorZerado ?? false)))
            {
                cargaIntegracao.ValorFrete = new Dominio.ObjetosDeValor.Embarcador.Frete.FreteValor();
                cargaIntegracao.ValorFrete.FreteProprio = xmlNotaFiscal.ValorFrete;
            }

            if (empresa != null)
            {
                cargaIntegracao.TransportadoraEmitente = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa() { CNPJ = empresa.CNPJ_SemFormato };

                if (empresa.FiliaisEmbarcadorHabilitado != null && empresa.FiliaisEmbarcadorHabilitado.Count() > 0)
                {
                    Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
                    Dominio.Entidades.Embarcador.Filiais.Filial filial = repFilial.buscarPorCodigoEmbarcador(cargaIntegracao.Filial.CodigoIntegracao);
                    if (filial != null && !empresa.FiliaisEmbarcadorHabilitado.Any(obj => obj.Codigo == filial.Codigo))
                        cargaIntegracao.TransportadoraEmitente = null;
                }
            }

            return cargaIntegracao;
        }

        private void ValidacaoTipoNotaVenda(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal)
        {
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repositorioXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaPedido.Carga;
            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = cargaPedido.Pedido;

            if (!(carga?.TipoOperacao?.ConfiguracaoDocumentoEmissao?.PossuiNotaOrdemVenda ?? false))
                return;

            if (carga?.TipoOperacao?.ConfiguracaoDocumentoEmissao?.EmitirCTENotaRemessa ?? false)
            {
                if (xmlNotaFiscal.CFOP.StartsWith("59") || xmlNotaFiscal.CFOP.StartsWith("69"))
                {
                    xmlNotaFiscal.TipoNotaFiscalIntegrada = TipoNotaFiscalIntegrada.RemessaVenda;
                    if (xmlNotaFiscal.Codigo > 0)
                        repositorioXmlNotaFiscal.Atualizar(xmlNotaFiscal);
                }
            }
            else if (pedido.Remetente == xmlNotaFiscal.Emitente && xmlNotaFiscal.Destinatario.CPF_CNPJ_SemFormato == (carga?.Filial?.CNPJ ?? string.Empty))
            {
                xmlNotaFiscal.TipoNotaFiscalIntegrada = TipoNotaFiscalIntegrada.OrdemVenda;

                //if (!(carga?.TipoOperacao?.ConfiguracaoDocumentoEmissao?.UtilizaNotaVendaObjetoCTE ?? false))//neste caso a obs do CTe vai ser a nota conta.
                //    pedido.ObservacaoCTe = string.Concat(pedido.ObservacaoCTe, $" Nota: {xmlNotaFiscal.Numero}, Série: {xmlNotaFiscal.Serie}, Data: {xmlNotaFiscal.DataEmissao}, Chave: {xmlNotaFiscal.Chave}. ");

                if (xmlNotaFiscal.Codigo > 0)
                    repositorioXmlNotaFiscal.Atualizar(xmlNotaFiscal);
            }
            else if (pedido.Expedidor == xmlNotaFiscal.Expedidor && (carga?.TipoOperacao?.ConfiguracaoDocumentoEmissao?.UtilizaNotaVendaObjetoCTE ?? false))
            {
                xmlNotaFiscal.TipoNotaFiscalIntegrada = TipoNotaFiscalIntegrada.Faturamento;

                //pedido.ObservacaoCTe = string.Concat(pedido.ObservacaoCTe, $" Nota: {xmlNotaFiscal.Numero}, Série: {xmlNotaFiscal.Serie}, Data: {xmlNotaFiscal.DataEmissao}, Chave: {xmlNotaFiscal.Chave}. ");
                if (xmlNotaFiscal.Codigo > 0)
                    repositorioXmlNotaFiscal.Atualizar(xmlNotaFiscal);
            }
            else
                xmlNotaFiscal.TipoNotaFiscalIntegrada = null;
        }

        private string ValidarAlertaObservacaoNota(Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, bool utilizaSistemMultimodal)
        {
            string msgRetorno = "";

            if (utilizaSistemMultimodal)
            {
                Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(_unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = repIntegracaoIntercab.BuscarIntegracao();

                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && cargaPedido != null && cargaPedido.Carga != null && cargaPedido.Carga?.GrupoPessoaPrincipal != null && xmlNotaFiscal != null && !string.IsNullOrWhiteSpace(xmlNotaFiscal.Observacao))
                {
                    if (integracaoIntercab?.PossuiIntegracaoIntercab ?? false)
                        return "";

                    Repositorio.Embarcador.Pessoas.GrupoPessoaMensagemAlerta repMensagemAlerta = new Repositorio.Embarcador.Pessoas.GrupoPessoaMensagemAlerta(_unitOfWork);
                    List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoaMensagemAlerta> alertas = repMensagemAlerta.BuscarAlertasPorGrupoPessoas(cargaPedido.Carga.GrupoPessoaPrincipal.Codigo, xmlNotaFiscal.Observacao);
                    if (alertas != null && alertas.Count > 0)
                        alertas = alertas.Select(o => o).Distinct().ToList();
                    if (alertas != null && alertas.Count > 0)
                        msgRetorno = string.Join(", ", alertas.Select(o => o.MensagemAlerta).ToList());
                    else
                        msgRetorno = "";
                }

                if (integracaoIntercab?.PossuiIntegracaoIntercab ?? false)
                    return "";

                if (xmlNotaFiscal.Emitente != null && string.IsNullOrWhiteSpace(xmlNotaFiscal.Emitente.CodigoIntegracao))
                    msgRetorno += ", Emitente: " + xmlNotaFiscal.Emitente.Descricao + " não possui o código de integração cadastrado.";
                if (xmlNotaFiscal.Destinatario != null && string.IsNullOrWhiteSpace(xmlNotaFiscal.Destinatario.CodigoIntegracao))
                    msgRetorno += ", Destinatário: " + xmlNotaFiscal.Destinatario.Descricao + " não possui o código de integração cadastrado.";
                if (xmlNotaFiscal.Recebedor != null && string.IsNullOrWhiteSpace(xmlNotaFiscal.Recebedor.CodigoIntegracao))
                    msgRetorno += ", Recebedor: " + xmlNotaFiscal.Recebedor.Descricao + " não possui o código de integração cadastrado.";
                if (xmlNotaFiscal.Expedidor != null && string.IsNullOrWhiteSpace(xmlNotaFiscal.Expedidor.CodigoIntegracao))
                    msgRetorno += ", Expedidor: " + xmlNotaFiscal.Expedidor.Descricao + " não possui o código de integração cadastrado.";
            }

            return msgRetorno;
        }

        private void ValidarSeExisteNotaFiscalParcial(Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido)
        {
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial repCargaPedidoXMLNotaFiscalParcial = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial cargaPedidoXMLNotaFiscalParcial = repCargaPedidoXMLNotaFiscalParcial.BuscarPorNumeroOuNumeroPedidoECargaPedido(xmlNotaFiscal.Numero, xmlNotaFiscal.NumeroPedido, xmlNotaFiscal.NumeroDT, xmlNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida ? xmlNotaFiscal.Emitente.CPF_CNPJ : xmlNotaFiscal.Destinatario.CPF_CNPJ, cargaPedido.Codigo);

            if (cargaPedidoXMLNotaFiscalParcial != null)
            {
                cargaPedidoXMLNotaFiscalParcial.XMLNotaFiscal = xmlNotaFiscal;
                xmlNotaFiscal.TipoNotaFiscalIntegrada = cargaPedidoXMLNotaFiscalParcial.TipoNotaFiscalIntegrada;

                repCargaPedidoXMLNotaFiscalParcial.Atualizar(cargaPedidoXMLNotaFiscalParcial);
                repXMLNotaFiscal.Atualizar(xmlNotaFiscal);
            }

        }

        private Dominio.Entidades.Embarcador.Pedidos.Pedido LiberarPedidoExpedidor(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(_unitOfWork);

            Dominio.Entidades.Empresa transportadorProximoTrecho = repositorioEmpresa.BuscarPorCNPJ(cargaPedido.Expedidor.CPF_CNPJ_SemFormato);
            if (transportadorProximoTrecho == null)
                return null;

            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = cargaPedido.Pedido;

            pedido.Initialize();
            pedido.PedidoTotalmenteCarregado = false;
            pedido.GerarAutomaticamenteCargaDoPedido = false;
            pedido.TipoOperacao = carga.TipoOperacao;
            pedido.Empresa = transportadorProximoTrecho;
            pedido.Expedidor = cargaPedido.Expedidor;
            pedido.Recebedor = null;

            SalvarLogDebugLiberarPedido(pedido, "LiberarPedidoExpedidor");

            return pedido;
        }

        private Dominio.Entidades.Embarcador.Pedidos.Pedido LiberarPedidoRecebedor(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (cargaPedido.Recebedor == null)
                return null;

            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = cargaPedido.Pedido;

            pedido.Initialize();
            pedido.PedidoTotalmenteCarregado = false;
            pedido.GerarAutomaticamenteCargaDoPedido = false;
            pedido.TipoOperacao = carga.TipoOperacao;
            pedido.Expedidor = cargaPedido.Recebedor;
            pedido.Recebedor = null;

            SalvarLogDebugLiberarPedido(pedido, "LiberarPedidoRecebedor");

            return pedido;
        }

        private void SalvarLogDebugLiberarPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, string nomeArquivo)
        {
            Servicos.Log.GravarDebug($@"As seguintes informações do pedido de código: {pedido.Codigo} e de número: {pedido.NumeroPedidoEmbarcador} foram alteradas para:
                                       PedidoTotalmenteCarregado: {pedido.PedidoTotalmenteCarregado.ObterDescricao()}
                                       GerarAutomaticamenteCargaDoPedido: {pedido.GerarAutomaticamenteCargaDoPedido.ObterDescricao()}
                                       TipoOperacao: {pedido.TipoOperacao?.Descricao ?? "null"}
                                       Empresa: {pedido.Empresa?.Descricao ?? "null"}
                                       Expedidor: {pedido.Expedidor?.Descricao ?? "null"}
                                       Recebedor: {pedido.Recebedor?.Descricao ?? "null"}
                                    ", nomeArquivo);
        }

        #endregion Métodos Privados
    }
}
