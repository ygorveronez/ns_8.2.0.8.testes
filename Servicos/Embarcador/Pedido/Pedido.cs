using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Entidades.Embarcador.Configuracoes;
using Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Newtonsoft.Json;
using Repositorio;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace Servicos.Embarcador.Pedido
{
    public class Pedido : ServicoBase
    {
        #region Atributos Privados

        private List<(string Carga, Dominio.Entidades.Embarcador.Pedidos.MotivoImportacaoPedidoAtrasada Motivo)> motivosAtraso = null;
        private List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoPacoteImportacao> PacotesCache = new List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoPacoteImportacao>();
        private List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoPacoteImportacao> TodosPacotes = new List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoPacoteImportacao>();
        private string codigoCargaEmbarcadorShopee = string.Empty;

        #endregion

        #region Construtores

        public Pedido() : base() { }
        public Pedido(UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken) { }

        #endregion

        #region Métodos Públicos

        private bool ValidarSemLatLng(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            if ((pedido.Remetente != null) && (string.IsNullOrEmpty(pedido.Remetente.Latitude) || string.IsNullOrEmpty(pedido.Remetente.Longitude)))
                return true;

            if ((pedido.Destinatario != null && !pedido.UsarOutroEnderecoDestino) && (string.IsNullOrEmpty(pedido.Destinatario.Latitude) || string.IsNullOrEmpty(pedido.Destinatario.Longitude)))
                return true;

            if ((pedido.Destinatario != null && pedido.UsarOutroEnderecoDestino) && (string.IsNullOrEmpty(pedido.EnderecoDestino?.ClienteOutroEndereco?.Latitude) || string.IsNullOrEmpty(pedido.EnderecoDestino?.ClienteOutroEndereco?.Longitude)))
                return true;

            if ((pedido.Recebedor != null) && (string.IsNullOrEmpty(pedido.Recebedor.Latitude) || string.IsNullOrEmpty(pedido.Recebedor.Longitude)))
                return true;

            if ((pedido.Expedidor != null) && (string.IsNullOrEmpty(pedido.Expedidor.Latitude) || string.IsNullOrEmpty(pedido.Expedidor.Longitude)))
                return true;

            return false;
        }

        public bool ImportarNotasFiscaisNOTFIS(Dominio.Entidades.LayoutEDI layoutEDI, System.IO.Stream msArquivo, Dominio.Entidades.Usuario usuario, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Repositorio.UnitOfWork unitOfWork, out string retorno, AdminMultisoftware.Repositorio.UnitOfWork adminUnitOfWork)
        {
            retorno = "";
            try
            {
                if (layoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.NOTFIS || layoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.NOTFIS_NOVA_IMPORTACAO)
                {
                    Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
                    Servicos.Embarcador.Pedido.NotaFiscal svcNotaFiscal = new Servicos.Embarcador.Pedido.NotaFiscal(unitOfWork);
                    Servicos.LeituraEDI serLeituraEDI = new Servicos.LeituraEDI(null, layoutEDI, msArquivo, unitOfWork);
                    Servicos.Cliente svcCliente = new Servicos.Cliente(StringConexao);

                    bool ediComNota = false;
                    List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal> notasFiscais = null;
                    Dominio.ObjetosDeValor.EDI.Notfis.EDINotFis notfis = null;
                    try
                    {
                        notasFiscais = serLeituraEDI.GerarNotasFiscais();
                        if (notasFiscais != null && notasFiscais.Count > 0)
                            ediComNota = true;
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro(ex);
                        retorno = ex.Message;
                    }

                    if (!ediComNota || notasFiscais == null || notasFiscais.Count <= 0)
                    {
                        try
                        {
                            notfis = serLeituraEDI.GerarNotasFis();
                            ediComNota = true;
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro(ex);
                            retorno = ex.Message;
                            return false;
                        }
                    }

                    if (ediComNota && (notasFiscais != null && notasFiscais.Count > 0) || notfis != null)
                    {
                        unitOfWork.Start();

                        bool importarCtesSubcontratacao = false;
                        try
                        {
                            if (notfis != null && (notfis.CabecalhoDocumento != null && notfis.CabecalhoDocumento.Embarcador != null && notfis.CabecalhoDocumento.Embarcadores.Any(e => e.Destinatarios.Any(d => d.NotasFiscais.Any(n => n.CTe != null && !string.IsNullOrWhiteSpace(n.CTe.Chave))))) || (!string.IsNullOrWhiteSpace(notfis.ChaveCTeAnterior)))
                                importarCtesSubcontratacao = true;
                            else
                                importarCtesSubcontratacao = false;
                        }
                        catch (Exception)
                        {
                            importarCtesSubcontratacao = false;
                            //throw;
                        }

                        if (importarCtesSubcontratacao)
                        {
                            ProcessarCTesSubcontratacaoNOTFIS(ref retorno, notfis, unitOfWork, adminUnitOfWork, layoutEDI, tipoServicoMultisoftware);
                        }
                        else
                        {
                            if (notasFiscais != null && notasFiscais.Count > 0)
                            {
                                foreach (Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal notaFiscal in notasFiscais)
                                {
                                    Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = null;

                                    if (!string.IsNullOrWhiteSpace(notaFiscal.Chave))
                                        xmlNotaFiscal = repXmlNotaFiscal.BuscarPorChave(notaFiscal.Chave);

                                    if (xmlNotaFiscal == null)
                                        xmlNotaFiscal = new Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal();
                                    else
                                    {
                                        xmlNotaFiscal.Initialize();
                                        xmlNotaFiscal.NotaJaEstavaNaBase = true;
                                    }

                                    notaFiscal.DocumentoRecebidoViaNOTFIS = true;
                                    notaFiscal.Modelo = "55";
                                    notaFiscal.SituacaoNFeSefaz = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNFeSefaz.Autorizada;
                                    notaFiscal.TipoOperacaoNotaFiscal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida;

                                    xmlNotaFiscal = svcNotaFiscal.PreencherParaXMLNotaFiscal(ref xmlNotaFiscal, notaFiscal, null, null, ref retorno);
                                    xmlNotaFiscal.DocumentoRecebidoViaFTP = true;

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


                                    if (!string.IsNullOrWhiteSpace(retorno))
                                        return false;

                                    if (xmlNotaFiscal.Codigo == 0)
                                    {
                                        xmlNotaFiscal.DataRecebimento = DateTime.Now;
                                        repXmlNotaFiscal.Inserir(xmlNotaFiscal, auditado);
                                    }
                                    else
                                        repXmlNotaFiscal.Atualizar(xmlNotaFiscal, auditado);
                                }
                            }

                            if (notfis != null && notfis.CabecalhoDocumento != null && notfis.CabecalhoDocumento.Embarcadores != null)
                            {
                                foreach (Dominio.ObjetosDeValor.EDI.Notfis.Embarcador remetente in notfis.CabecalhoDocumento.Embarcadores)
                                {
                                    foreach (Dominio.ObjetosDeValor.EDI.Notfis.Destinatario destinatario in remetente.Destinatarios)
                                    {
                                        if (destinatario.NotasFiscais != null)
                                        {
                                            foreach (Dominio.ObjetosDeValor.EDI.Notfis.NotaFiscal documento in destinatario.NotasFiscais)
                                            {

                                                Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal notaFiscal = documento.NFe;

                                                notaFiscal.Emitente = svcCliente.SetarDadosPessoa(remetente.Pessoa, adminUnitOfWork, unitOfWork);
                                                notaFiscal.Destinatario = svcCliente.SetarDadosPessoa(destinatario.Pessoa, adminUnitOfWork, unitOfWork);
                                                notaFiscal.Modelo = "55";
                                                notaFiscal.SituacaoNFeSefaz = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNFeSefaz.Autorizada;
                                                notaFiscal.TipoOperacaoNotaFiscal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida;
                                                notaFiscal.Chave = Utilidades.String.OnlyNumbers(notaFiscal.Chave);
                                                notaFiscal.DocumentoRecebidoViaNOTFIS = true;

                                                if (string.IsNullOrWhiteSpace(notaFiscal.Chave) && documento.ComplementoNotaFiscal != null && !string.IsNullOrWhiteSpace(documento.ComplementoNotaFiscal.ChaveNFe))
                                                    notaFiscal.Chave = documento.ComplementoNotaFiscal.ChaveNFe;

                                                if (documento.Recebedor != null && documento.Recebedor.Pessoa != null && documento.NFe != null && documento.NFe.Recebedor == null)

                                                    documento.NFe.Recebedor = documento.Recebedor.Pessoa;
                                                Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = null;

                                                if (!string.IsNullOrWhiteSpace(notaFiscal.Chave))
                                                    xmlNotaFiscal = repXmlNotaFiscal.BuscarPorChave(notaFiscal.Chave);

                                                if (xmlNotaFiscal == null)
                                                    xmlNotaFiscal = new Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal();
                                                else
                                                {
                                                    xmlNotaFiscal.Initialize();
                                                    xmlNotaFiscal.NotaJaEstavaNaBase = true;
                                                }

                                                notaFiscal.DocumentoRecebidoViaNOTFIS = true;
                                                xmlNotaFiscal = svcNotaFiscal.PreencherParaXMLNotaFiscal(ref xmlNotaFiscal, notaFiscal, null, null, ref retorno);
                                                xmlNotaFiscal.DocumentoRecebidoViaFTP = true;

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

                                                if (!string.IsNullOrWhiteSpace(retorno))
                                                    return false;

                                                if (xmlNotaFiscal.Codigo == 0)
                                                {
                                                    xmlNotaFiscal.DataRecebimento = DateTime.Now;
                                                    repXmlNotaFiscal.Inserir(xmlNotaFiscal, auditado);
                                                }
                                                else
                                                    repXmlNotaFiscal.Atualizar(xmlNotaFiscal, auditado);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        unitOfWork.CommitChanges();
                    }
                    else
                    {
                        retorno = "Não foi processada nenhuma nota fiscal para o arquivo recebido.";
                        return false;

                    }
                    return true;
                }
                else
                {
                    retorno = "O arquivo recebido não possui o layout de NOTFIS";
                    return false;
                }
            }
            catch (Exception ex)
            {
                if (unitOfWork != null)
                    unitOfWork.Rollback();

                Servicos.Log.TratarErro(ex);
                retorno = ex.Message;
                return false;
            }
            finally
            {
                //unitOfWork.Dispose();
            }
        }

        public List<Dominio.Entidades.Embarcador.Cargas.Carga> ImportarPedidoNOTFIS(Dominio.Entidades.LayoutEDI layoutEDI, System.IO.Stream msArquivo, Dominio.Entidades.Usuario usuario, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultiSoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Repositorio.UnitOfWork unitOfWork, out string retorno, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga)
        {

            Servicos.LeituraEDI leituraEDI = new Servicos.LeituraEDI(null, layoutEDI, msArquivo, unitOfWork, 0, 0, 0, 0, 0, 0, 0, 0, true, true, Encoding.GetEncoding("iso-8859-1"));
            Dominio.ObjetosDeValor.EDI.Notfis.EDINotFis notfis = leituraEDI.GerarNotasFis();
            retorno = "";
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            Repositorio.Embarcador.Cargas.TipoDeCarga repTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Usuario repMotorista = new Repositorio.Usuario(unitOfWork);
            Repositorio.LayoutEDI repLayoutEDI = new LayoutEDI(unitOfWork);

            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
            Servicos.Embarcador.Carga.Carga serCarga = new Embarcador.Carga.Carga(unitOfWork);

            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

            Servicos.WebService.Empresa.Motorista serMotorista = new WebService.Empresa.Motorista(unitOfWork);
            Servicos.Cliente serCliente = new Cliente();

            List<Dominio.Entidades.Embarcador.PreCargas.PreCarga> preCargas = new List<Dominio.Entidades.Embarcador.PreCargas.PreCarga>();


            if (notfis == null || notfis.CabecalhoDocumento == null)
            {
                retorno = "O arquivo enviado não é um EDI Válido";
                return null;
            }

            List<string> cnpjsEmbarcador = (from obj in notfis.CabecalhoDocumento.Embarcadores select obj.Pessoa.CPFCNPJ).Distinct().ToList();

            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasGeradas = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();

            for (int c = 0; c < cnpjsEmbarcador.Count; c++)
            {
                string prefixoPreCarga = Guid.NewGuid().ToString().Replace("-", "").Substring(5, 10);//cria o prefixo randomico para não correr o risco de inserir um pedido em uma pre carga já existente

                List<Dominio.ObjetosDeValor.EDI.Notfis.Embarcador> Embarcadores = (from obj in notfis.CabecalhoDocumento.Embarcadores where obj.Pessoa.CPFCNPJ == cnpjsEmbarcador[c] select obj).ToList();

                for (int e = 0; e < Embarcadores.Count; e++)
                {
                    try
                    {
                        unitOfWork.Start();

                        layoutEDI = repLayoutEDI.BuscarPorCodigo(layoutEDI.Codigo);
                        Dominio.ObjetosDeValor.EDI.Notfis.Embarcador embarcador = Embarcadores[e];

                        if (embarcador.Destinatarios == null)
                        {
                            unitOfWork.Rollback();
                            retorno = "O arquivo EDI está inconsistente, não existe uma linha com os dados do destinatário informado, por favor verifique o arquivo.";
                            return null;
                        }

                        for (int a = 0; a < embarcador.Destinatarios.Count; a++)
                        {
                            Dominio.ObjetosDeValor.EDI.Notfis.Destinatario destinatarioNotFis = embarcador.Destinatarios[a];

                            Dominio.Entidades.Empresa empresa = null;
                            if (!string.IsNullOrWhiteSpace(notfis.Destinatario))
                            {
                                long n;
                                bool isNumeric = long.TryParse(notfis.Destinatario, out n);

                                if (isNumeric)
                                {
                                    string cnpjFormatado = n.ToString("d14"); //string.Format(@"{0:00000000000000}", notfis.Destinatario);
                                    empresa = repEmpresa.BuscarPorCNPJ(cnpjFormatado);
                                }
                            }

                            if (empresa == null && layoutEDI.Empresa != null && layoutEDI.Empresa.EmpresaPai != null)
                                empresa = layoutEDI.Empresa;

                            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = layoutEDI.TipoOperacao;
                            Dominio.Entidades.Embarcador.Filiais.Filial filial = layoutEDI.Filial;

                            Dominio.ObjetosDeValor.RetornoVerificacaoCliente retornoRemetente = serCliente.ConverterObjetoValorPessoa(embarcador.Pessoa, "Remetente", unitOfWork);
                            if (retornoRemetente.Status)
                            {
                                Dominio.Entidades.Cliente remetente = retornoRemetente.cliente;
                                Dominio.ObjetosDeValor.RetornoVerificacaoCliente retornoDestinatario = serCliente.ConverterObjetoValorPessoa(destinatarioNotFis.Pessoa, "Destinatario", unitOfWork);

                                if (retornoDestinatario.Status)
                                {
                                    Dominio.Entidades.Cliente destinatario = retornoDestinatario.cliente;
                                    Dominio.Entidades.Cliente recebedor = null;
                                    Dominio.Entidades.Cliente expedidor = null;

                                    List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal> nfes = new List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal>();
                                    List<int> notasParciais = new List<int>();

                                    decimal quantidadePalletsFacionada = 0m;
                                    decimal valorADVALOREM = 0m;
                                    decimal valorPedagio = 0m;
                                    decimal valorFreteLiquido = 0m;
                                    decimal valorFrete = 0m;
                                    decimal valorDescarga = 0m;

                                    if (destinatarioNotFis.NotasFiscais.Count > 0)
                                    {
                                        if (destinatarioNotFis.NotasFiscais.FirstOrDefault().Consignatario != null)
                                        {
                                            Dominio.ObjetosDeValor.RetornoVerificacaoCliente retornoExpedidor = serCliente.ConverterObjetoValorPessoa(destinatarioNotFis.NotasFiscais.FirstOrDefault().Consignatario.Pessoa, "Expedidor", unitOfWork);
                                            if (!retornoExpedidor.Status)
                                            {
                                                unitOfWork.Rollback();
                                                retorno = retornoExpedidor.Mensagem;
                                                return null;
                                            }
                                            expedidor = retornoExpedidor.cliente;
                                        }

                                        if (destinatarioNotFis.NotasFiscais.FirstOrDefault().Recebedor != null)
                                        {
                                            Dominio.ObjetosDeValor.RetornoVerificacaoCliente retornoRecebedor = serCliente.ConverterObjetoValorPessoa(destinatarioNotFis.NotasFiscais.FirstOrDefault().Recebedor.Pessoa, "Recebedor", unitOfWork);
                                            if (!retornoRecebedor.Status)
                                            {
                                                unitOfWork.Rollback();
                                                retorno = retornoRecebedor.Mensagem;
                                                return null;
                                            }
                                            recebedor = retornoRecebedor.cliente;
                                        }

                                        for (int n = 0; n < destinatarioNotFis.NotasFiscais.Count; n++)
                                        {
                                            Dominio.ObjetosDeValor.EDI.Notfis.NotaFiscal notaFiscal = destinatarioNotFis.NotasFiscais[n];

                                            valorADVALOREM += notaFiscal.NFe.ValorComponenteAdValorem;
                                            valorPedagio += notaFiscal.NFe.ValorComponentePedagio;
                                            valorFreteLiquido += notaFiscal.NFe.ValorFreteLiquido;
                                            valorFrete += notaFiscal.NFe.ValorFrete;
                                            valorDescarga += notaFiscal.NFe.ValorComponenteDescarga;
                                            quantidadePalletsFacionada += notaFiscal.NFe.QuantidadePallets;

                                            notaFiscal.NFe.Emitente = embarcador.Pessoa;
                                            notaFiscal.NFe.Destinatario = destinatarioNotFis.Pessoa;
                                            notaFiscal.NFe.SituacaoNFeSefaz = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNFeSefaz.Autorizada;
                                            notaFiscal.NFe.TipoOperacaoNotaFiscal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida;
                                            notaFiscal.NFe.DocumentoRecebidoViaNOTFIS = true;

                                            if (!string.IsNullOrWhiteSpace(notaFiscal.NFe.Chave))
                                            {
                                                nfes.Add(notaFiscal.NFe);

                                                Dominio.Entidades.Embarcador.Cargas.Carga cargaExiste = repPedidoXMLNotaFiscal.BuscarCargaPorChaveExpedidor(notaFiscal.NFe.Chave, layoutEDI.TipoOperacao?.Codigo ?? 0, expedidor?.CPF_CNPJ ?? 0, recebedor?.CPF_CNPJ ?? 0);

                                                if (cargaExiste != null)
                                                {
                                                    if (serCarga.VerificarSeCargaEstaNaLogistica(cargaExiste, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador))
                                                    {
                                                        Dominio.ObjetosDeValor.Embarcador.Carga.CargaCancelamentoAdicionar cargaCancelamentoAdicionar = new Dominio.ObjetosDeValor.Embarcador.Carga.CargaCancelamentoAdicionar()
                                                        {
                                                            Carga = cargaExiste,
                                                            MotivoCancelamento = "Viagem atualizada ao importar NOTFIS",
                                                            TipoServicoMultisoftware = tipoServicoMultisoftware
                                                        };

                                                        Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = Servicos.Embarcador.Carga.Cancelamento.GerarCargaCancelamento(cargaCancelamentoAdicionar, configuracaoTMS, unitOfWork);
                                                        Servicos.Embarcador.Carga.Cancelamento.SolicitarCancelamentoCarga(ref cargaCancelamento, unitOfWork, StringConexao, tipoServicoMultisoftware);
                                                    }
                                                    else
                                                    {
                                                        if (cargaExiste.SituacaoCarga != SituacaoCarga.Cancelada && cargaExiste.SituacaoCarga != SituacaoCarga.Anulada)
                                                        {
                                                            unitOfWork.Rollback();
                                                            retorno = "Já existe uma nota fiscal com a chave " + notaFiscal.NFe.Chave + " ativa na carga " + cargaExiste.CodigoCargaEmbarcador;
                                                            return null;
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (notaFiscal.NFe.Numero > 0)
                                                {
                                                    Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial repCargaPedidoXMLNotaFiscalParcial = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial(unitOfWork);
                                                    double.TryParse(notaFiscal.NFe.Emitente.CPFCNPJ, out double emitente);
                                                    Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial cargaPedidoXMLNotaFiscaisParciais = repCargaPedidoXMLNotaFiscalParcial.BuscarNotaParcialPorNumero(notaFiscal.NFe.Numero, emitente, expedidor?.CPF_CNPJ ?? 0, recebedor?.CPF_CNPJ ?? 0);

                                                    if (cargaPedidoXMLNotaFiscaisParciais != null)
                                                    {
                                                        Dominio.Entidades.Embarcador.Cargas.Carga cargaExiste = cargaPedidoXMLNotaFiscaisParciais.CargaPedido.Carga;

                                                        if (cargaExiste != null)
                                                        {
                                                            if (serCarga.VerificarSeCargaEstaNaLogistica(cargaExiste, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador))
                                                            {
                                                                Dominio.ObjetosDeValor.Embarcador.Carga.CargaCancelamentoAdicionar cargaCancelamentoAdicionar = new Dominio.ObjetosDeValor.Embarcador.Carga.CargaCancelamentoAdicionar()
                                                                {
                                                                    Carga = cargaExiste,
                                                                    MotivoCancelamento = "Viagem atualizada ao importar NOTFIS",
                                                                    TipoServicoMultisoftware = tipoServicoMultisoftware
                                                                };

                                                                Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = Servicos.Embarcador.Carga.Cancelamento.GerarCargaCancelamento(cargaCancelamentoAdicionar, configuracaoTMS, unitOfWork);
                                                                Servicos.Embarcador.Carga.Cancelamento.SolicitarCancelamentoCarga(ref cargaCancelamento, unitOfWork, StringConexao, tipoServicoMultisoftware);
                                                            }
                                                            else
                                                            {
                                                                unitOfWork.Rollback();
                                                                retorno = "Já existe uma nota fiscal com a chave " + notaFiscal.NFe.Chave + " ativa na carga " + cargaExiste.CodigoCargaEmbarcador;
                                                                return null;
                                                            }
                                                        }
                                                    }
                                                    notasParciais.Add(notaFiscal.NFe.Numero);
                                                }
                                            }
                                        }
                                    }

                                    Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga = Servicos.Embarcador.Carga.TipoCarga.ObterTipoDeCargaPorRegra(remetente, destinatario, unitOfWork);

                                    if (tipoCarga == null)
                                        tipoCarga = tipoOperacao?.TipoDeCargaPadraoOperacao;

                                    Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga = CriarPreCarga(prefixoPreCarga, empresa, null, null, null, tipoCarga, null, tipoOperacao, false, null, filial, false, null, null, unitOfWork, tipoServicoMultisoftware);

                                    Dominio.ObjetosDeValor.Embarcador.Frete.FreteValor freteValor = new Dominio.ObjetosDeValor.Embarcador.Frete.FreteValor()
                                    {
                                        FreteProprio = valorFreteLiquido,
                                        ValorTotalAReceber = valorFrete,
                                        ComponentesAdicionais = new List<Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional>()
                                    };

                                    if (valorPedagio > 0m)
                                    {
                                        freteValor.ComponentesAdicionais.Add(new Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional()
                                        {
                                            Componente = new Dominio.ObjetosDeValor.Embarcador.Frete.Componente()
                                            {
                                                TipoComponenteFrete = TipoComponenteFrete.PEDAGIO,
                                                CodigoIntegracao = "pedagio"
                                            },
                                            IncluirBaseCalculoICMS = true,
                                            IncluirTotalReceber = true,
                                            ValorComponente = valorPedagio
                                        });
                                    }

                                    if (valorDescarga > 0m)
                                    {
                                        freteValor.ComponentesAdicionais.Add(new Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional()
                                        {
                                            Componente = new Dominio.ObjetosDeValor.Embarcador.Frete.Componente()
                                            {
                                                TipoComponenteFrete = TipoComponenteFrete.DESCARGA,
                                                CodigoIntegracao = "descarga"
                                            },
                                            IncluirBaseCalculoICMS = true,
                                            IncluirTotalReceber = true,
                                            ValorComponente = valorDescarga
                                        });
                                    }

                                    if (valorADVALOREM > 0m)
                                    {
                                        freteValor.ComponentesAdicionais.Add(new Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional()
                                        {
                                            Componente = new Dominio.ObjetosDeValor.Embarcador.Frete.Componente()
                                            {
                                                TipoComponenteFrete = TipoComponenteFrete.ADVALOREM,
                                                CodigoIntegracao = "advalorem"
                                            },
                                            IncluirBaseCalculoICMS = true,
                                            IncluirTotalReceber = true,
                                            ValorComponente = valorADVALOREM
                                        });
                                    }

                                    Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoImportacaoSalvar pedidoImportacaoAdicionar = new Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoImportacaoSalvar()
                                    {
                                        Empresa = empresa,
                                        PreCarga = preCarga,
                                        Nfes = nfes,
                                        NotasParciais = notasParciais,
                                        QuantidadePalletsFacionada = quantidadePalletsFacionada,
                                        TipoCarga = tipoCarga,
                                        TipoOperacao = tipoOperacao,
                                        Remetente = remetente,
                                        Destinatario = destinatario,
                                        Expedidor = expedidor,
                                        Recebedor = recebedor,
                                        Filial = filial,
                                        FreteValor = freteValor,
                                        Usuario = usuario,
                                        TipoTomador = TipoTomador.Remetente
                                    };

                                    Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retornoLinhaPedido = AdicionarPedidoImportacao(pedidoImportacaoAdicionar, false, unitOfWork, configuracaoTMS, tipoServicoMultisoftware, clienteMultiSoftware, auditado);
                                    retorno = retornoLinhaPedido.mensagemFalha;

                                    if (string.IsNullOrWhiteSpace(retorno))
                                    {
                                        if (preCarga != null)
                                        {
                                            if (!preCargas.Contains(preCarga))
                                                preCargas.Add(preCarga);
                                        }
                                    }
                                    else
                                    {
                                        unitOfWork.Rollback();
                                        return null;
                                    }
                                }
                                else
                                {
                                    unitOfWork.Rollback();
                                    retorno = retornoDestinatario.Mensagem;
                                    return null;
                                }
                            }
                            else
                            {
                                unitOfWork.Rollback();
                                retorno = retornoRemetente.Mensagem;
                                return null;
                            }
                        }

                        unitOfWork.CommitChanges();
                    }
                    catch (BaseException excecao)
                    {
                        unitOfWork.Rollback();
                        retorno = excecao.Message;
                        return null;
                    }
                    catch (Exception excecao)
                    {
                        unitOfWork.Rollback();
                        Log.TratarErro(excecao);
                        retorno = "Ocorreu uma falha ao processar o arquivo EDI";
                        return null;
                    }
                }

                Dominio.Entidades.Embarcador.Cargas.Carga carga = FinalizarPreCargasAgMontagem(configuracaoTMS, preCargas.Select(a => a.Codigo).ToList(), null, tipoServicoMultisoftware, unitOfWork, out retorno, out int qtdCargasGeradas, configuracaoGeralCarga);
                if (carga != null)
                    cargasGeradas.Add(carga);
            }

            return cargasGeradas;
        }

        public dynamic ObterDetalhesPedido(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, List<string> filiais, Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            dynamic pedido = ObterDetalhesPedido(cargaPedido.Pedido, montagemCarga: false, montagemCargaPorPedidoProduto: false, filiais, notasFiscaisPedidos: null, carga, configuracaoEmbarcador: null, unitOfWork: unitOfWork);

            Dominio.Entidades.Cliente tomadorPedido = cargaPedido.ObterTomador();

            pedido.NumeroReboque = cargaPedido.NumeroReboque;
            pedido.NumeroReboqueDescricao = cargaPedido.NumeroReboque.ObterDescricao();
            pedido.TipoCarregamentoPedido = cargaPedido.TipoCarregamentoPedido;
            pedido.TipoCarregamentoPedidoDescricao = cargaPedido.TipoCarregamentoPedido.ObterDescricao();
            pedido.PesoCargaPedido = cargaPedido.Peso.ToString("n4");
            pedido.Tomador = tomadorPedido?.Descricao ?? string.Empty;

            return pedido;
        }

        public dynamic ObterDetalhesPedido(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido carregamentoPedido, List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> produtosPedidosCarregamento, List<string> filiais, List<Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoNotaFiscal> notasFiscaisPedidos, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Repositorio.UnitOfWork unitOfWork, bool montagemCargaPorPedidoProduto, List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentosPedidos = null, List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFretePedido> simulacoesFretePedidos = null)
        {
            dynamic pedido = ObterDetalhesPedido(carregamentoPedido.Pedido, montagemCarga: true, montagemCargaPorPedidoProduto: montagemCargaPorPedidoProduto, filiais, notasFiscaisPedidos, null, configuracaoEmbarcador: configuracaoEmbarcador, unitOfWork: unitOfWork, possuiFilialArmazem: false, carregamentosPedidos, simulacoesFretePedidos);

            decimal peso = carregamentoPedido.Peso;
            decimal pesoPallet = carregamentoPedido.PesoPallet;
            decimal pallet = carregamentoPedido.Pallet; //carregamentoPedido.Pedido.TotalPallets;
            decimal cubagem = carregamentoPedido.Pedido.CubagemTotal;
            if (!montagemCargaPorPedidoProduto && (carregamentoPedido.Pedido?.CanalEntrega?.NaoUtilizarCapacidadeVeiculoMontagemCarga ?? false))
            {
                pallet = 0;
                cubagem = 0;
            }
            int quantidadeIdDemanda = 0;

            //#23335 - Adicionado parametro montagemCargaPorPedidoProduto pois deu problema na natural one,.. aonde o peso do pedido é superior ao peso dos produtos...
            if ((produtosPedidosCarregamento?.Count ?? 0) > 0 && montagemCargaPorPedidoProduto)
            {
                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> produtosPedidoCarregamento = (
                    from item in produtosPedidosCarregamento
                    where item.PedidoProduto.Pedido.Codigo == carregamentoPedido.Pedido.Codigo
                    select item
                ).ToList();

                // Se o pedido/produto constar no carregamento, vamos considerar o peso de carregamento do pedido/produto.
                if ((produtosPedidoCarregamento?.Count ?? 0) > 0)
                {
                    peso = produtosPedidoCarregamento.Sum(x => x.Peso);
                    pallet = produtosPedidoCarregamento.Sum(x => x.QuantidadePallet);
                    cubagem = produtosPedidoCarregamento.Sum(x => x.MetroCubico);
                    quantidadeIdDemanda = (from prod in produtosPedidoCarregamento select prod.PedidoProduto.IdDemanda).Distinct().Count();
                }
            }

            pedido.NumeroReboque = carregamentoPedido.NumeroReboque;
            pedido.NumeroReboqueDescricao = carregamentoPedido.NumeroReboque.ObterDescricao();
            pedido.TipoCarregamentoPedido = carregamentoPedido.TipoCarregamentoPedido;
            pedido.TipoCarregamentoPedidoDescricao = carregamentoPedido.TipoCarregamentoPedido.ObterDescricao();
            pedido.PesoPedidoCarregamento = peso;
            pedido.PesoPalletPedidoCarregamento = peso;
            pedido.PalletPedidoCarregamento = pallet;
            pedido.CubagemPedidoCarregamento = cubagem;
            pedido.QuantidadeBipada = decimal.Round(carregamentoPedido.VolumeBipado);
            pedido.QuantidadeBipagemTotal = decimal.Round(carregamentoPedido.VolumeTotal);
            pedido.VolumesBipagem = pedido.QuantidadeBipada + "/" + pedido.QuantidadeBipagemTotal;
            pedido.Ordem = carregamentoPedido.Ordem;
            pedido.Categoria = carregamentoPedido.Pedido.Destinatario?.Categoria?.Descricao ?? string.Empty;
            pedido.QuantidadeIdDemanda = quantidadeIdDemanda;

            return pedido;
        }

        public dynamic ObterDetalhesPedidoTransporteMaritimo(Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimo pedidoDadosTransporteMaritimo, List<Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimoRoteamento> pedidoDadosTransporteMaritimoRoteamentos)
        {
            if (pedidoDadosTransporteMaritimo == null)
                return null;

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimoRoteamento> roteamentos = pedidoDadosTransporteMaritimoRoteamentos
                .Where(o => o.DadosTransporteMaritimo.Codigo == pedidoDadosTransporteMaritimo.Codigo)
                .OrderBy(o => o.Codigo)
                .ToList();

            return new
            {
                CodigoArmador = pedidoDadosTransporteMaritimo.CodigoArmador ?? "",
                CodigoIdentificacaoCarga = pedidoDadosTransporteMaritimo.CodigoIdentificacaoCarga ?? "",
                CodigoNCM = pedidoDadosTransporteMaritimo.CodigoNCM ?? "",
                CodigoPortoCarregamentoTransbordo = pedidoDadosTransporteMaritimo.CodigoPortoCarregamentoTransbordo ?? "",
                CodigoPortoDestinoTransbordo = pedidoDadosTransporteMaritimo.CodigoPortoDestinoTransbordo ?? "",
                CodigoRota = pedidoDadosTransporteMaritimo.CodigoRota ?? "",
                DataBooking = pedidoDadosTransporteMaritimo.DataBooking?.ToString("dd/MM/yyyy HH:mm:ss") ?? "",
                DataDeadLineCarga = pedidoDadosTransporteMaritimo.DataDeadLineCarga?.ToString("dd/MM/yyyy HH:mm:ss") ?? "",
                DataDeadLineDraf = pedidoDadosTransporteMaritimo.DataDeadLineDraf?.ToString("dd/MM/yyyy HH:mm:ss") ?? "",
                DataDepositoContainer = pedidoDadosTransporteMaritimo.DataDepositoContainer?.ToString("dd/MM/yyyy HH:mm:ss") ?? "",
                DataETADestino = pedidoDadosTransporteMaritimo.DataETADestino?.ToString("dd/MM/yyyy HH:mm:ss") ?? "",
                DataETADestinoFinal = pedidoDadosTransporteMaritimo.DataETADestinoFinal?.ToString("dd/MM/yyyy HH:mm:ss") ?? "",
                DataETAOrigem = pedidoDadosTransporteMaritimo.DataETAOrigem?.ToString("dd/MM/yyyy HH:mm:ss") ?? "",
                DataETAOrigemFinal = pedidoDadosTransporteMaritimo.DataETAOrigemFinal?.ToString("dd/MM/yyyy HH:mm:ss") ?? "",
                DataETASegundaOrigem = pedidoDadosTransporteMaritimo.DataETASegundaOrigem?.ToString("dd/MM/yyyy HH:mm:ss") ?? "",
                DataETASegundoDestino = pedidoDadosTransporteMaritimo.DataETASegundoDestino?.ToString("dd/MM/yyyy HH:mm:ss") ?? "",
                DataETATransbordo = pedidoDadosTransporteMaritimo.DataETATransbordo?.ToString("dd/MM/yyyy HH:mm:ss") ?? "",
                DataETS = pedidoDadosTransporteMaritimo.DataETS?.ToString("dd/MM/yyyy HH:mm:ss") ?? "",
                DataETSTransbordo = pedidoDadosTransporteMaritimo.DataETSTransbordo?.ToString("dd/MM/yyyy HH:mm:ss") ?? "",
                DataRetiradaContainerDestino = pedidoDadosTransporteMaritimo.DataRetiradaContainerDestino?.ToString("dd/MM/yyyy HH:mm:ss") ?? "",
                DataRetiradaVazio = pedidoDadosTransporteMaritimo.DataRetiradaVazio?.ToString("dd/MM/yyyy HH:mm:ss") ?? "",
                DataRetornoVazio = pedidoDadosTransporteMaritimo.DataRetornoVazio?.ToString("dd/MM/yyyy HH:mm:ss") ?? "",
                DescricaoIdentificacaoCarga = pedidoDadosTransporteMaritimo.DescricaoIdentificacaoCarga ?? "",
                DescricaoPortoCarregamentoTransbordo = pedidoDadosTransporteMaritimo.DescricaoPortoCarregamentoTransbordo ?? "",
                DescricaoPortoDestinoTransbordo = pedidoDadosTransporteMaritimo.DescricaoPortoDestinoTransbordo ?? "",
                Incoterm = pedidoDadosTransporteMaritimo.Incoterm ?? "",
                MensagemTransbordo = pedidoDadosTransporteMaritimo.MensagemTransbordo ?? "",
                MetragemCarga = pedidoDadosTransporteMaritimo.MetragemCarga ?? "",
                ModoTransporte = pedidoDadosTransporteMaritimo.ModoTransporte ?? "",
                NomeNavio = pedidoDadosTransporteMaritimo.Navio?.Descricao ?? "",
                NomeNavioTransbordo = pedidoDadosTransporteMaritimo.NavioTransbordo?.Descricao ?? "",
                NumeroBL = pedidoDadosTransporteMaritimo.NumeroBL ?? "",
                NumeroContainer = pedidoDadosTransporteMaritimo.Container?.Numero ?? "",
                NumeroLacre = pedidoDadosTransporteMaritimo.NumeroLacre ?? "",
                NumeroViagem = pedidoDadosTransporteMaritimo.NumeroViagem ?? "",
                NumeroViagemTransbordo = pedidoDadosTransporteMaritimo.NumeroViagemTransbordo ?? "",
                TerminalOrigem = pedidoDadosTransporteMaritimo.LocalTerminalOrigem?.Descricao ?? "",
                TipoEnvio = pedidoDadosTransporteMaritimo.TipoEnvio?.ObterDescricao() ?? "",
                TipoTransporte = pedidoDadosTransporteMaritimo.TipoDeTransporte.ObterDescricao(),
                Transbordo = pedidoDadosTransporteMaritimo.Transbordo ?? "",
                Roteamentos = (
                    from o in roteamentos
                    select new
                    {
                        o.CodigoRoteamento,
                        o.CodigoSCAC,
                        o.FlagNavio,
                        o.NomeNavio,
                        o.NumeroViagem,
                        o.PortoCargaLocalizacao,
                        o.PortoDescargaLocalizacao,
                        o.TipoRemessa,
                        PortoCargaData = o.PortoCargaData?.ToString("dd/MM/yyyy") ?? "",
                        PortoDescargaData = o.PortoDescargaData?.ToString("dd/MM/yyyy") ?? ""
                    }
                ).ToList()
            };
        }

        public dynamic ObterDetalhesPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, List<string> filiais, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Repositorio.UnitOfWork unitOfWork)
        {
            return ObterDetalhesPedido(pedido, montagemCarga: false, montagemCargaPorPedidoProduto: false, filiais, notasFiscaisPedidos: null, carga, configuracaoEmbarcador: configuracaoEmbarcador, unitOfWork: unitOfWork);
        }

        public dynamic ObterDetalhesPedido(Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Pedido pedido, List<string> filiais, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Repositorio.UnitOfWork unitOfWork)
        {
            return ObterDetalhesPedido(pedido, montagemCarga: false, montagemCargaPorPedidoProduto: false, filiais, notasFiscaisPedidos: null, carga, configuracaoEmbarcador: configuracaoEmbarcador, unitOfWork: unitOfWork);
        }

        public dynamic ObterDetalhesPedidoParaMontagemCarga(Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Pedido pedido, bool montagemCargaPorPedidoProduto, List<string> filiais, List<Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoNotaFiscal> notasFiscaisPedidos, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Repositorio.UnitOfWork unitOfWork, bool possuiFilialArmazem = false, List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentosPedidos = null, List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFretePedido> simulacoesFretePedidos = null, Dominio.Entidades.Embarcador.Cargas.AcompanhamentoEntregaTempoConfiguracao.AcompanhamentoEntregaTempoConfiguracao configuracaoTempoTendendicas = null, List<Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoDetalhesAdicionais> pedidoDetalhesAdicionais = null)
        {
            return ObterDetalhesPedido(pedido, montagemCarga: true, montagemCargaPorPedidoProduto: montagemCargaPorPedidoProduto, filiais, notasFiscaisPedidos, null, configuracaoEmbarcador: configuracaoEmbarcador, unitOfWork: unitOfWork, possuiFilialArmazem, carregamentosPedidos, simulacoesFretePedidos, configuracaoTempoTendendicas, pedidoDetalhesAdicionais);
        }

        private dynamic ObterDetalhesPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, bool montagemCarga, bool montagemCargaPorPedidoProduto, List<string> filiais, List<Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoNotaFiscal> notasFiscaisPedidos, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Repositorio.UnitOfWork unitOfWork, bool possuiFilialArmazem = false, List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentosPedidos = null, List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFretePedido> simulacoesFretePedidos = null, List<Dominio.Entidades.Embarcador.Pedidos.PedidoAdicional> pedidosAdicionais = null, Dominio.Entidades.Embarcador.Cargas.AcompanhamentoEntregaTempoConfiguracao.AcompanhamentoEntregaTempoConfiguracao configuracaoTempoTendendicas = null, List<Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoDetalhesAdicionais> pedidoDetalhesAdicionais = null, List<Dominio.Entidades.Embarcador.Pedidos.PedidoFronteira> pedidosFronteiras = null)
        {
            CultureInfo culture = CultureInfo.CreateSpecificCulture("pt-BR");
            dynamic pedidoRetornar = new ExpandoObject();

            Repositorio.Embarcador.Logistica.AgendamentoColeta repAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoAdicional repositorioPedidoAdicionais = new Repositorio.Embarcador.Pedidos.PedidoAdicional(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFretePedido repositorioMontagemCarregamentoBlocoSimuladorFretePedido = new Repositorio.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFretePedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoFronteira repositorioPedidoFronteira = new Repositorio.Embarcador.Pedidos.PedidoFronteira(unitOfWork);

            string formatacaoDataCarregamento = (configuracaoEmbarcador?.InformaHorarioCarregamentoMontagemCarga ?? false) ? "dd/MM/yyyy HH:mm" : "dd/MM/yyyy";
            bool filtrarAgendamentoColetaNaMontagemDaCarga = configuracaoEmbarcador?.FiltrarAgendamentoColetaNaMontagemDaCarga ?? false;
            bool possuiCargaPerigosa = filtrarAgendamentoColetaNaMontagemDaCarga ? repAgendamentoColeta.BuscarSePossuiCargaPerigosaPorPedido(pedido.Codigo, pedido.NumeroPedidoEmbarcador) : false;
            bool pedidoDestinadoAFilial = filiais != null && pedido.Destinatario != null && filiais.Contains(pedido.Destinatario.CPF_CNPJ_SemFormato);

            Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta = filtrarAgendamentoColetaNaMontagemDaCarga ? repAgendamentoColeta.BuscarFirstOrDefaultPorPedidos(new List<int> { pedido.Codigo }) : null;
            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = agendamentoColeta != null ? repositorioTipoOperacao.BuscarTipoOperacaoPorTipoDeCarga(agendamentoColeta?.TipoCarga?.Codigo ?? 0) : null;
            Dominio.Entidades.Cliente tomadorPedido = pedido.ObterTomador();

            Dominio.Entidades.Embarcador.Pedidos.PedidoAdicional pedidoAdicionais;
            if (pedidosAdicionais == null)
                pedidoAdicionais = repositorioPedidoAdicionais.BuscarPorPedido(pedido.Codigo);
            else
                pedidoAdicionais = pedidosAdicionais.Find(o => o.Pedido.Codigo == pedido.Codigo);

            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentoPedido = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido>();
            if (carregamentosPedidos == null)
                carregamentoPedido = repCarregamentoPedido.BuscarPorPedido(pedido.Codigo);
            else
                carregamentoPedido = (from o in carregamentosPedidos where o.Pedido.Codigo == pedido.Codigo select o).ToList();

            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFretePedido> simulacoes = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFretePedido>();
            if (simulacoesFretePedidos == null && montagemCarga)
                simulacoes = repositorioMontagemCarregamentoBlocoSimuladorFretePedido.BuscarPorPedido(pedido.Codigo);
            else if (simulacoesFretePedidos != null)
                simulacoes = (from o in simulacoesFretePedidos where o.Pedido.Codigo == pedido.Codigo select o).ToList();

            List<Dominio.Entidades.Cliente> pedidoFronteiras = new List<Dominio.Entidades.Cliente>();
            if (pedidosFronteiras == null || pedido.Fronteira != null)
            {
                if (pedido.Fronteira != null)
                    pedidoFronteiras.Add(pedido.Fronteira);
                else
                    pedidoFronteiras = repositorioPedidoFronteira.BuscarFronteirasPorPedido(pedido.Codigo);
            }
            else
                pedidoFronteiras = (from o in pedidosFronteiras where o.Codigo == pedido.Codigo select o.Fronteira).ToList();

            bool pedidoColetaEntrega = false;

            pedidoRetornar.DT_RowColor = "";
            pedidoRetornar.DT_RowId = pedido.Codigo;
            pedidoRetornar.SemLatLng = ValidarSemLatLng(pedido);
            pedidoRetornar.Codigo = pedido.Codigo;
            pedidoRetornar.CodigoPedidoCliente = pedido.CodigoPedidoCliente;
            pedidoRetornar.TipoCarga = pedido.TipoDeCarga?.Descricao ?? "";
            pedidoRetornar.CodigoTipoCarga = pedido.TipoDeCarga?.Codigo ?? 0;
            pedidoRetornar.NumeroPedido = pedido.Numero.ToString("D");
            pedidoRetornar.CodigoFilial = pedido.Filial?.Codigo ?? 0;
            pedidoRetornar.Filial = pedido.Filial?.Descricao ?? "";
            pedidoRetornar.NumeroPedidoEmbarcador = pedido.NumeroPedidoEmbarcador;
            pedidoRetornar.Origem = pedido.Expedidor == null ? pedido.Origem?.DescricaoCidadeEstado ?? "" : pedido.Expedidor.Localidade.DescricaoCidadeEstado;
            pedidoRetornar.Destino = pedido.Destino?.DescricaoCidadeEstado ?? "";
            pedidoRetornar.CodigoDestino = pedido.Destino?.Codigo ?? 0;
            pedidoRetornar.CodigoRecebedor = pedido.Recebedor?.Localidade?.Codigo ?? 0;
            pedidoRetornar.RemetenteCNPJ = pedido.GrupoPessoas != null ? pedido.GrupoPessoas.Descricao : pedido.Remetente?.CPF_CNPJ_SemFormato ?? "";
            pedidoRetornar.Remetente = pedido.GrupoPessoas != null ? pedido.GrupoPessoas.Descricao : pedido.Remetente?.Descricao ?? "";
            pedidoRetornar.TransportadorLocalCarregamentoRestringido = pedido.Empresa?.RestringirLocaisCarregamentoAutorizadosMotoristas ?? false;
            pedidoRetornar.Empresa = pedido.Empresa?.Descricao ?? "";
            pedidoRetornar.Destinatario = pedido.Destinatario?.Descricao ?? "";
            pedidoRetornar.DestinatarioNomeFantasia = pedido.Destinatario?.NomeFantasia ?? "";
            pedidoRetornar.Recebedor = pedido.Recebedor?.Descricao ?? "";
            pedidoRetornar.RecebedorNomeFantasia = pedido.Recebedor?.NomeFantasia ?? "";
            pedidoRetornar.DestinoRecebedor = pedido.Recebedor?.Localidade?.DescricaoCidadeEstado ?? "";
            pedidoRetornar.Expedidor = pedido.Expedidor?.Descricao ?? "";
            pedidoRetornar.CodigoExpedidor = pedido.Expedidor?.CPF_CNPJ ?? 0;
            pedidoRetornar.ExpedidorExterior = pedido.Expedidor?.Tipo == "E";
            pedidoRetornar.DataCarregamentoPedido = pedido.DataCarregamentoPedido?.ToString("dd/MM/yyyy") ?? "";
            pedidoRetornar.DataHoraCarregamentoPedido = pedido.DataCarregamentoPedido?.ToString("dd/MM/yyyy HH:mm") ?? "";
            pedidoRetornar.DataInicialColeta = pedido.DataInicialColeta?.ToString("dd/MM/yyyy HH:mm") ?? "";
            pedidoRetornar.DataCarregamento = pedido.DataCarregamentoCarga?.ToString(formatacaoDataCarregamento) ?? "";
            pedidoRetornar.DataDescarregamento = pedido.DataTerminoCarregamento?.ToString(formatacaoDataCarregamento) ?? "";
            pedidoRetornar.DataPrevisaoEntrega = pedido.PrevisaoEntrega?.ToString("dd/MM/yyyy HH:mm") ?? "";
            pedidoRetornar.PesoSaldoRestante = pedido.PesoSaldoRestante.ToString("n2");
            pedidoRetornar.PalletSaldoRestante = pedido.PalletSaldoRestante.ToString("n2");
            pedidoRetornar.Peso = pedido.PesoTotal.ToString("n4");
            pedidoRetornar.PesoLiquido = pedido.PesoLiquidoTotal.ToString("n4");
            pedidoRetornar.Volumes = pedido.QtVolumes.ToString() ?? "";
            pedidoRetornar.TotalPallets = (pedido.NumeroPaletes + pedido.NumeroPaletesFracionado).ToString("n3");
            pedidoRetornar.Cubagem = pedido.CubagemTotal.ToString("n3");
            pedidoRetornar.TipoOperacao = pedido.TipoOperacao?.Descricao ?? "";
            pedidoRetornar.TipoOperacaoInformarRecebedor = pedido.TipoOperacao?.PermitirInformarRecebedorMontagemCarga ?? false;
            pedidoRetornar.CodigoTipoOperacao = pedido.TipoOperacao?.Codigo ?? 0;
            pedidoRetornar.NecessarioConfirmacaoMotorista = pedido.TipoOperacao?.ConfiguracaoMobile?.NecessarioConfirmacaoMotorista ?? false;
            pedidoRetornar.TempoLimiteConfirmacaoMotorista = pedido.TipoOperacao?.ConfiguracaoMobile?.TempoLimiteConfirmacaoMotorista.ToString(@"hh\:mm\:ss") ?? "";
            pedidoRetornar.CodigoCanalEntrega = pedido?.CanalEntrega?.Codigo ?? 0;
            pedidoRetornar.CanalEntrega = pedido?.CanalEntrega?.Descricao ?? "";
            pedidoRetornar.CanalVenda = pedido?.CanalVenda?.Descricao ?? "";
            pedidoRetornar.Ordem = pedido.Ordem ?? "";
            pedidoRetornar.NumeroReboque = NumeroReboque.SemReboque;
            pedidoRetornar.NumeroReboqueDescricao = NumeroReboque.SemReboque.ObterDescricao();
            pedidoRetornar.TipoCarregamentoPedido = TipoCarregamentoPedido.Normal;
            pedidoRetornar.TipoCarregamentoPedidoDescricao = TipoCarregamentoPedido.Normal.ObterDescricao();
            pedidoRetornar.PedidoDestinadoAFilial = pedidoDestinadoAFilial;
            pedidoRetornar.CodigoEmpresa = pedido.Empresa?.Codigo ?? 0;
            pedidoRetornar.DescricaoEmpresa = pedido.Empresa?.Descricao ?? "";
            pedidoRetornar.PedidoBloqueado = pedido.PedidoBloqueado;
            pedidoRetornar.LiberadoMontagemCarga = pedido.PedidoLiberadoMontagemCarga;
            pedidoRetornar.PedidoRestricaoData = pedido.PedidoRestricaoData;
            pedidoRetornar.PercentualSeparacaoPedido = pedido.PercentualSeparacaoPedido;
            pedidoRetornar.Tomador = tomadorPedido?.Descricao ?? "";
            pedidoRetornar.CEPDestinatario = pedido.Destinatario?.CEP ?? "";
            pedidoRetornar.CodigoAgrupamentoCarregamento = pedido.CodigoAgrupamentoCarregamento ?? string.Empty;
            pedidoRetornar.Valor = (pedido?.ValorTotalNotasFiscais ?? 0).ToString("c");//.ToString("n4") ?? "0,00";
            pedidoRetornar.NumeroPedidoSequencial = pedido.Numero;
            pedidoRetornar.CategoriaCliente = pedido.Destinatario?.Categoria?.Descricao ?? string.Empty;
            pedidoRetornar.Distancia = pedido.Distancia.ToString("n2") ?? string.Empty;
            pedidoRetornar.ObservacaoDestinatario = pedido.Destinatario?.Observacao ?? string.Empty;
            pedidoRetornar.GrupoPessoa = (pedido.Recebedor != null ? pedido.Recebedor?.GrupoPessoas?.Descricao : pedido.Destinatario?.GrupoPessoas?.Descricao) ?? pedido.GrupoPessoas?.Descricao ?? string.Empty;
            pedidoRetornar.PrazoEntrega = pedido.DiasUteisPrazoTransportador;
            pedidoRetornar.ValorFrete = ((from o in simulacoes orderby o.SimuladorFrete.Vencedor descending select o.SimuladorFrete.ValorTotal)?.FirstOrDefault() ?? 0).ToString("c");
            pedidoRetornar.Tomador = ObterTomadorPedido(pedido.TipoTomador, pedido);
            pedidoRetornar.Gerente = pedido.FuncionarioGerente?.Descricao ?? string.Empty;
            pedidoRetornar.Supervisor = pedido.FuncionarioSupervisor?.Descricao ?? string.Empty;
            pedidoRetornar.PesoTotalPaletes = pedido.PesoTotalPaletes.ToString("n2");
            pedidoRetornar.TipoPaleteCliente = pedido.TipoPaleteCliente;
            pedidoRetornar.NumeroPaletesFracionado = pedido.NumeroPaletesFracionado.ToString("n3");
            pedidoRetornar.ValorTotalNotasFiscais = pedido.ValorTotalNotasFiscais > 0 ? pedido.ValorTotalNotasFiscais.ToString("n2") : "";
            pedidoRetornar.NumeroOrdem = pedido.NumeroOrdem ?? "";

            if ((pedido.Destinatario?.ValidarValorMinimoMercadoriaEntregaMontagemCarregamento ?? false) && pedido.ValorTotalNotasFiscais < (pedido.Destinatario?.ValorMinimoCarga ?? 0))
            {
                pedidoRetornar.ValorMercadoInferior = true;
                pedidoRetornar.DT_RowColor = CoresHelper.Descricao(Cores.Vermelho);
            }
            pedidoRetornar.SituacaoComercial = pedido.SituacaoComercialPedido?.Descricao ?? "";
            pedidoRetornar.SituacaoComercialCor = pedido.SituacaoComercialPedido?.Cor ?? "#FFFFFF";
            pedidoRetornar.SituacaoEstoque = pedidoAdicionais?.SituacaoEstoquePedido?.Descricao ?? "";
            pedidoRetornar.SituacaoEstoqueCor = pedidoAdicionais?.SituacaoEstoquePedido?.Cor ?? "#FFFFFF";

            // ATENÇÃO: Ao adicionar um atributo no retorno do serviço de pedidos, atente-se para adicionar apenas se necessário 
            //          fora do parametro montagemCarga, pois em alguns casos, quando montando com muitos pedidos está apresentando erro 500.
            if (!montagemCargaPorPedidoProduto)
            {
                pedidoRetornar.TipoCondicaoPagamento = pedido.TipoPagamento.ObterTipoCondicaoPagamento();
                pedidoRetornar.DescricaoTipoCondicaoPagamento = pedido.TipoPagamento.ObterDescricao();
                pedidoRetornar.TipoTomador = pedido.TipoTomador;
                pedidoRetornar.DescricaoTipoTomador = pedido.TipoTomador.ObterDescricao();
                pedidoRetornar.Fronteiras = pedidoFronteiras ?? new List<Dominio.Entidades.Cliente>();
                pedidoRetornar.Importacao = pedido.Remetente != null && pedido.Remetente.Tipo == "E";
                pedidoRetornar.Exportacao = pedido.Destinatario != null && pedido.Destinatario.Tipo == "E";
                pedidoRetornar.PortoSaida = pedido.PortoSaida ?? "";
                pedidoRetornar.PortoChegada = pedido.PortoChegada ?? "";
                pedidoRetornar.Companhia = pedido.Companhia ?? "";
                pedidoRetornar.NumeroNavio = pedido.NumeroNavio ?? "";
                pedidoRetornar.Reserva = pedido.Reserva ?? "";
                pedidoRetornar.Resumo = pedido.Resumo ?? "";
                pedidoRetornar.Temperatura = pedido.Temperatura ?? "";
                pedidoRetornar.Vendedor = pedido.Vendedor ?? pedido.FuncionarioVendedor?.Descricao ?? string.Empty;
                pedidoRetornar.TipoEmbarque = pedido.TipoEmbarque ?? "";
                pedidoRetornar.DeliveryTerm = pedido.DeliveryTerm ?? "";
                pedidoRetornar.IdAutorizacao = pedido.IdAutorizacao ?? "";
                pedidoRetornar.DataETA = pedido.DataETA.HasValue ? pedido.DataETA.Value.ToString("dd/MM/yyyy HH:mm") : "";
                pedidoRetornar.DataInclusaoBooking = pedido.DataInclusaoBooking.HasValue ? pedido.DataInclusaoBooking.Value.ToString("dd/MM/yyyy HH:mm") : "";
                pedidoRetornar.DataInclusaoPCP = pedido.DataInclusaoPCP.HasValue ? pedido.DataInclusaoPCP.Value.ToString("dd/MM/yyyy HH:mm") : "";
                pedidoRetornar.PossuiAjudante = (pedido.Ajudante && pedido.QtdAjudantes > 0 ? "Sim, Qtd.: " + pedido.QtdAjudantes.ToString("D") : "Não");
                pedidoRetornar.PossuiAjudanteCarga = (pedidoAdicionais?.AjudanteCarga ?? false && pedidoAdicionais?.QtdAjudantesCarga > 0 ? "Sim, Qtd.: " + pedidoAdicionais?.QtdAjudantesCarga.ToString("D") : "Não");
                pedidoRetornar.PossuiAjudanteDescarga = (pedidoAdicionais?.AjudanteDescarga ?? false && pedidoAdicionais?.QtdAjudantesDescarga > 0 ? "Sim, Qtd.: " + pedidoAdicionais?.QtdAjudantesDescarga.ToString("D") : "Não");
                pedidoRetornar.PossuiCarga = (pedido.PossuiCarga && pedido.ValorCarga.HasValue && pedido.ValorCarga.Value > 0 ? "Sim, R$ " + pedido.ValorCarga.Value.ToString("n2") : "Não");
                pedidoRetornar.PossuiDescarga = (pedido.PossuiDescarga && pedido.ValorDescarga.HasValue && pedido.ValorDescarga.Value > 0 ? "Sim, R$ " + pedido.ValorDescarga.Value.ToString("n2") : "Não");
                pedidoRetornar.NumeroContainer = pedido.Container?.Numero ?? "";
                pedidoRetornar.CodigoPedidoViagemNavio = pedido.PedidoViagemNavio?.Codigo ?? 0;
                pedidoRetornar.PedidoViagemNavio = pedido.PedidoViagemNavio?.Descricao ?? "";
                pedidoRetornar.NumeroBooking = pedido.NumeroBooking;
                pedidoRetornar.PortoOrigem = pedido.Porto?.Descricao ?? "";
                pedidoRetornar.PortoDestino = pedido.PortoDestino?.Descricao ?? "";
                pedidoRetornar.Usuario = pedido.Usuario?.Descricao;
                pedidoRetornar.FuncionarioVendedor = (montagemCarga ? string.Empty : pedido.FuncionarioVendedor?.DescricaoTelefoneEmail ?? "");
                pedidoRetornar.CargaPerigosa = possuiCargaPerigosa ? "Sim" : "Não";
                pedidoRetornar.PermiteInformarPesoCubadoNaMontagemDaCarga = tipoOperacao?.PermiteInformarPesoCubadoNaMontagemDaCarga ?? false;
                pedidoRetornar.NaoExigeRoteirizacaoMontagemCarga = pedido.TipoOperacao?.NaoExigeRoteirizacaoMontagemCarga ?? false;
                pedidoRetornar.TipoOperacaoControlarCapacidadePorUnidade = pedido.TipoOperacao?.ConfiguracaoMontagemCarga?.ControlarCapacidadePorUnidade ?? false;
                pedidoRetornar.PedidoColetaEntrega = pedidoColetaEntrega;
                pedidoRetornar.Produto = pedido.ProdutoPredominante ?? "";
                pedidoRetornar.ValorFreteNegociado = pedido.ValorFreteNegociado;
                pedidoRetornar.ValorCobrancaFreteCombinado = pedido.ValorCobrancaFreteCombinado ?? 0;

                pedidoRetornar.PedidoPrioritario = (montagemCarga ? false : pedido.GrupoPessoas != null ? pedido.GrupoPessoas.TornarPedidosPrioritarios : pedido.Remetente != null && pedido.Remetente.GrupoPessoas != null ? pedido.Remetente.GrupoPessoas.TornarPedidosPrioritarios : false);
                pedidoRetornar.Anexos = (montagemCarga ? null : pedido.Anexos != null && pedido.Anexos.Count > 0 ? (
                from anexo in pedido.Anexos
                select new
                {
                    anexo.Codigo,
                    anexo.Descricao,
                    anexo.NomeArquivo,
                }
                ).ToList() : null);

                Dominio.Entidades.Embarcador.Financeiro.TipoPagamentoRecebimento formaPagamento = tomadorPedido?.FormaPagamento;
                int? diasDePrazoFatura = tomadorPedido?.DiasDePrazoFatura;
                if (!montagemCarga && tomadorPedido?.GrupoPessoas != null)
                {
                    if (formaPagamento == null)
                        formaPagamento = tomadorPedido.GrupoPessoas.FormaPagamento;
                    if (diasDePrazoFatura == null || diasDePrazoFatura == 0)
                        diasDePrazoFatura = tomadorPedido.GrupoPessoas.DiasDePrazoFatura;
                }

                pedidoRetornar.FormaPagamento = formaPagamento?.Descricao ?? string.Empty;
                pedidoRetornar.DiasDePrazoFatura = diasDePrazoFatura ?? 0;
            }

            pedidoRetornar.RotaFrete = new { Codigo = pedido.RotaFrete?.Codigo ?? 0, Descricao = pedido.RotaFrete?.Descricao ?? "" };
            pedidoRetornar.RotaFreteDescricao = pedido.RotaFrete?.Descricao ?? string.Empty;

            if (!montagemCarga)
            {
                pedidoRetornar.SenhaAgendamentoCliente = pedido.SenhaAgendamentoCliente?.Trim() ?? "";
                pedidoRetornar.ExigirPreCargaMontagemCarga = pedido.Filial?.ExigirPreCargaMontagemCarga ?? false;
                pedidoRetornar.DescricaoRecebedorColeta = pedido.RecebedorColeta?.Descricao ?? "";
                pedidoRetornar.CodigoRecebedorColeta = pedido.RecebedorColeta?.Codigo ?? 0;
                pedidoRetornar.DescricaoTipoOperacaoColeta = pedido.Remetente?.GrupoPessoas?.TipoOperacaoColeta?.Descricao ?? "";
                pedidoRetornar.CodigoTipoOperacaoColeta = pedido.Remetente?.GrupoPessoas?.TipoOperacaoColeta?.Codigo ?? 0;
                pedidoRetornar.DisponibilizarPedidoParaColeta = pedido.DisponibilizarPedidoParaColeta;
                pedidoRetornar.ObservacaoCliente = pedido.Destinatario?.Observacao ?? "";
                pedidoRetornar.ValorTotalPedido = (from p in pedido.Produtos select p.ValorProduto * p.Quantidade).Sum().ToString("N", culture);
                pedidoRetornar.Restricao = "";
                pedidoRetornar.PrimeiraEntrega = false;
                pedidoRetornar.Email = "";
                pedidoRetornar.ObservacaoRestricao = "";
                pedidoRetornar.ObservacaoPedido = pedido.Observacao;
                pedidoRetornar.Observacao = pedido.ObservacaoCTe;
                pedidoRetornar.ObservacaoInterna = pedido.ObservacaoInterna ?? string.Empty;
                pedidoRetornar.PalletsCarregados = (from obj in carregamentoPedido where obj.Carregamento.SituacaoCarregamento != SituacaoCarregamento.Cancelado select obj)?.Sum(cp => cp.Pallet).ToString("n2") ?? "0,000";
                pedidoRetornar.AtivarRecebedor = pedido.TipoOperacao?.PermitirInformarRecebedorMontagemCarga ?? false;
                pedidoRetornar.NumeroContainer = pedido.NumeroContainer ?? string.Empty;

                if (carga != null && carga.Carregamento != null)
                    pedidoRetornar.PalletsCarregadosNestaCarga = carregamentoPedido?.Where(x => x.Carregamento.Codigo == carga.Carregamento.Codigo)?.Sum(cp => cp.Pallet).ToString("n2") ?? "0,000";
            }

            pedidoRetornar.Carregamentos = string.Join(",", (from obj in carregamentoPedido where obj.Carregamento.SituacaoCarregamento != SituacaoCarregamento.Cancelado select obj.Carregamento?.NumeroCarregamento ?? string.Empty));
            pedidoRetornar.QuantidadeVolumes = pedido.QtVolumes;
            pedidoRetornar.DataAgendamento = pedido.DataAgendamento?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty;
            pedidoRetornar.SenhaAgendamento = pedido.SenhaAgendamento;
            pedidoRetornar.DataPrevisaoSaida = pedido.DataPrevisaoSaida?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty;
            pedidoRetornar.DataCriacao = pedido.DataCriacao?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty;
            pedidoRetornar.Reentrega = pedido.ReentregaSolicitada;
            pedidoRetornar.ObservacaoTipoOperacao = pedido?.TipoOperacao?.Observacao ?? string.Empty;

            pedidoRetornar.PedidoTotalmenteCarregado = pedido.PedidoTotalmenteCarregado;

            pedidoRetornar.ModeloVeicularCarga = new
            {
                Codigo = pedido.ModeloVeicularCarga?.Codigo ?? 0,
                Descricao = pedido.ModeloVeicularCarga?.Descricao ?? "",
                CapacidadePesoTransporte = pedido.ModeloVeicularCarga?.CapacidadePesoTransporte.ToString("n4") ?? "0,00",
                ToleranciaPesoMenor = pedido.ModeloVeicularCarga?.ToleranciaPesoMenor.ToString("n4") ?? "0,00",
                ModeloControlaCubagem = pedido.ModeloVeicularCarga?.ModeloControlaCubagem ?? false,
                Cubagem = pedido.ModeloVeicularCarga?.Cubagem.ToString("n2") ?? "0,00",
                ToleranciaMinimaCubagem = pedido.ModeloVeicularCarga?.ToleranciaMinimaCubagem.ToString("n2") ?? "0,00",
                VeiculoPaletizado = pedido.ModeloVeicularCarga?.VeiculoPaletizado ?? false,
                NumeroPaletes = pedido.ModeloVeicularCarga?.NumeroPaletes?.ToString() ?? "0",
                NumeroReboques = pedido.ModeloVeicularCarga?.NumeroReboques?.ToString() ?? "0",
                ToleranciaMinimaPaletes = pedido.ModeloVeicularCarga?.ToleranciaMinimaPaletes.ToString() ?? "0",
                OcupacaoCubicaPaletes = pedido.ModeloVeicularCarga?.OcupacaoCubicaPaletes.ToString("n2") ?? "0,00",
                ExigirDefinicaoReboquePedido = pedido.ModeloVeicularCarga?.ExigirDefinicaoReboquePedido ?? false
            };

            pedidoRetornar.EnderecoRemetente = pedido.Remetente != null ? new
            {
                Destinatario = pedido.Remetente.CPF_CNPJ,
                Logradrouro = pedido.Remetente.Endereco,
                pedido.Remetente.CEP,
                pedido.Remetente.Bairro,
                Localidade = pedido.Remetente.Localidade.DescricaoCidadeEstado,
                Latitude = pedido.Remetente.Latitude,
                pedido.Remetente.Numero,
                Longitude = pedido.Remetente.Longitude
            } : null;

            if (!pedido.UsarOutroEnderecoDestino)
            {
                pedidoRetornar.EnderecoDestino = pedido.Destinatario != null ? new
                {
                    Destinatario = pedido.Destinatario.CPF_CNPJ,
                    Logradrouro = pedido.Destinatario.Endereco,
                    pedido.Destinatario.CEP,
                    pedido.Destinatario.Bairro,
                    Localidade = pedido.Destinatario.Localidade.DescricaoCidadeEstado,
                    Latitude = pedido.Destinatario.Latitude,
                    pedido.Destinatario.Numero,
                    Longitude = pedido.Destinatario.Longitude
                } : null;
            }
            else
            {
                pedidoRetornar.EnderecoDestino = pedido.EnderecoDestino != null ? new
                {
                    Destinatario = pedido.Destinatario.CPF_CNPJ,
                    Logradrouro = pedido.EnderecoDestino.Endereco,
                    pedido.EnderecoDestino.CEP,
                    pedido.EnderecoDestino.Bairro,
                    Localidade = pedido.EnderecoDestino.Localidade.DescricaoCidadeEstado,
                    Latitude = pedido.EnderecoDestino.ClienteOutroEndereco?.Latitude,
                    pedido.EnderecoDestino.Numero,
                    Longitude = pedido.EnderecoDestino.ClienteOutroEndereco?.Longitude
                } : null;
            }

            pedidoRetornar.EnderecoRecebedor = pedido.Recebedor != null ? new
            {
                Destinatario = pedido.Recebedor.CPF_CNPJ,
                Logradrouro = pedido.Recebedor.Endereco,
                pedido.Recebedor.CEP,
                pedido.Recebedor.Bairro,
                pedido.Recebedor.Numero,
                Localidade = pedido.Recebedor.Localidade.DescricaoCidadeEstado,
                pedido.Recebedor.Latitude,
                pedido.Recebedor.Longitude,
                pedido.Recebedor.LatitudeTransbordo,
                pedido.Recebedor.LongitudeTransbordo
            } : null;

            pedidoRetornar.EnderecoExpedidor = pedido.Expedidor != null ? new
            {
                Destinatario = pedido.Expedidor.CPF_CNPJ,
                Logradrouro = pedido.Expedidor.Endereco,
                pedido.Expedidor.CEP,
                pedido.Expedidor.Bairro,
                Localidade = pedido.Expedidor.Localidade.DescricaoCidadeEstado,
                pedido.Expedidor.Latitude,
                pedido.Expedidor.Numero,
                pedido.Expedidor.Longitude,
                pedido.Expedidor.LatitudeTransbordo,
                pedido.Expedidor.LongitudeTransbordo
            } : null;

            pedidoRetornar.VolumesBipagem = "";
            pedidoRetornar.Categoria = "";
            pedidoRetornar.TipoOperacaoExigirInformarDataPrevisaoInicioViagem = pedido.TipoOperacao?.ConfiguracaoMontagemCarga?.ExigirInformarDataPrevisaoInicioViagem ?? false;
            pedidoRetornar.TipoOperacaoValidarValorMinimoCarga = pedido.TipoOperacao?.ConfiguracaoCarga?.ValidarValorMinimoCarga ?? false;
            pedidoRetornar.OcultarTipoDeOperacaoNaMontagemDaCarga = pedido.TipoOperacao?.ConfiguracaoMontagemCarga?.OcultarTipoDeOperacaoNaMontagemDaCarga ?? false;

            pedidoRetornar.Saldo = null;

            if (possuiFilialArmazem)
            {
                if (carregamentoPedido.Count > 0)
                {
                    Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto repositorioCarregamentosPedidoProduto = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto(unitOfWork);
                    List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> carregamentosPedidoProduto = repositorioCarregamentosPedidoProduto.BuscarPorCarregamentos((from carregamento in carregamentoPedido select carregamento.Carregamento.Codigo).ToList());
                    pedidoRetornar.Saldo = carregamentosPedidoProduto.Exists(cpp => cpp.Quantidade < cpp.PedidoProduto.Quantidade) ? "Sim" : "Não";
                }
                else
                {
                    Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedidoProduto repositorioSessaoRoteirizadorPedidoProduto = new Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedidoProduto(unitOfWork);
                    List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedidoProduto> sessaoRoteirizadorPedidoProduto = repositorioSessaoRoteirizadorPedidoProduto.BuscarSessaoRoteirizadorPorPedido(pedido.Codigo);

                    pedidoRetornar.Saldo = pedido.Produtos.Any(pp => pp.Quantidade > (sessaoRoteirizadorPedidoProduto.Find(sr => pp.Codigo == sr.PedidoProduto.Codigo)?.QuantidadeProdutoSessao ?? 0)) ? "Sim" : "Não";
                }
            }

            pedidoRetornar.NotasFiscais = string.Empty;
            if (notasFiscaisPedidos != null && notasFiscaisPedidos.Count > 0)
                pedidoRetornar.NotasFiscais = string.Join(", ", (from obj in notasFiscaisPedidos where obj.CodigoPedido == pedido.Codigo select obj.NumeroNota));

            pedidoRetornar.Operador = pedido.Autor?.Nome ?? "";
            pedidoRetornar.TendenciaEntrega = ObterTendenciaEntregaPedido(pedido.PrevisaoEntrega, configuracaoTempoTendendicas).ObterDescricao();
            pedidoRetornar.LocalPaletizacao = pedido.LocalPaletizacao?.Descricao ?? "";
            pedidoRetornar.Regiao = pedido.Remetente?.Regiao?.Descricao ?? string.Empty;
            pedidoRetornar.MesoRegiao = pedido.Remetente?.MesoRegiao?.Descricao ?? string.Empty;

            Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoDetalhesAdicionais pedidoDetalheAdicional = pedidoDetalhesAdicionais?.Find(o => o.CodigoPedido == pedido.Codigo) ?? new Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoDetalhesAdicionais();

            pedidoRetornar.DataDigitalizacaoCanhotoAvulso = pedidoDetalheAdicional?.DataDigitalizacaoCanhoto.ToDateTimeString() ?? string.Empty;
            pedidoRetornar.DataEntregaNotaCanhoto = pedidoDetalheAdicional?.DataEntregaNotaCanhoto.ToDateTimeString() ?? string.Empty;
            pedidoRetornar.PrimeiroCodigoCargaEmbarcador = pedidoDetalheAdicional?.CodigoCargaEmbarcador ?? string.Empty;

            return pedidoRetornar;
        }

        private dynamic ObterDetalhesPedido(Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Pedido pedido, bool montagemCarga, bool montagemCargaPorPedidoProduto, List<string> filiais, List<Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoNotaFiscal> notasFiscaisPedidos, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Repositorio.UnitOfWork unitOfWork, bool possuiFilialArmazem = false, List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentosPedidos = null, List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFretePedido> simulacoesFretePedidos = null, Dominio.Entidades.Embarcador.Cargas.AcompanhamentoEntregaTempoConfiguracao.AcompanhamentoEntregaTempoConfiguracao configuracaoTempoTendendicas = null, List<Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoDetalhesAdicionais> pedidoDetalhesAdicionais = null)
        {
            CultureInfo culture = CultureInfo.CreateSpecificCulture("pt-BR");
            dynamic pedidoRetornar = new ExpandoObject();

            Repositorio.Embarcador.Logistica.AgendamentoColeta repAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFretePedido repositorioMontagemCarregamentoBlocoSimuladorFretePedido = new Repositorio.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFretePedido(unitOfWork);

            string formatacaoDataCarregamento = (configuracaoEmbarcador?.InformaHorarioCarregamentoMontagemCarga ?? false) ? "dd/MM/yyyy HH:mm" : "dd/MM/yyyy";
            bool filtrarAgendamentoColetaNaMontagemDaCarga = configuracaoEmbarcador?.FiltrarAgendamentoColetaNaMontagemDaCarga ?? false;
            bool possuiCargaPerigosa = filtrarAgendamentoColetaNaMontagemDaCarga ? repAgendamentoColeta.BuscarSePossuiCargaPerigosaPorPedido(pedido.Codigo, pedido.NumeroPedidoEmbarcador) : false;
            bool pedidoDestinadoAFilial = filiais != null && pedido.Destinatario != null && filiais.Contains(pedido.Destinatario.CpfCnpj);

            Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta = filtrarAgendamentoColetaNaMontagemDaCarga ? repAgendamentoColeta.BuscarFirstOrDefaultPorPedidos(new List<int> { pedido.Codigo }) : null;
            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = agendamentoColeta != null ? repositorioTipoOperacao.BuscarTipoOperacaoPorTipoDeCarga(agendamentoColeta?.TipoCarga?.Codigo ?? 0) : null;
            Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.Cliente tomadorPedido = pedido.ObterTomador();

            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentoPedido = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido>();
            if (carregamentosPedidos == null)
                carregamentoPedido = repCarregamentoPedido.BuscarPorPedido(pedido.Codigo);
            else
                carregamentoPedido = (from o in carregamentosPedidos where o.Pedido.Codigo == pedido.Codigo select o).ToList();

            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFretePedido> simulacoes = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoBlocoSimuladorFretePedido>();
            if (simulacoesFretePedidos == null && montagemCarga)
                simulacoes = repositorioMontagemCarregamentoBlocoSimuladorFretePedido.BuscarPorPedido(pedido.Codigo);
            else if (simulacoesFretePedidos != null)
                simulacoes = (from o in simulacoesFretePedidos where o.Pedido.Codigo == pedido.Codigo select o).ToList();

            bool pedidoColetaEntrega = false;

            pedidoRetornar.DT_RowColor = "";
            pedidoRetornar.DT_RowId = pedido.Codigo;
            pedidoRetornar.SemLatLng = pedido.LatitudelongitudeNaoInformada;
            pedidoRetornar.Codigo = pedido.Codigo;
            pedidoRetornar.CodigoPedidoCliente = pedido.CodigoPedidoCliente;
            pedidoRetornar.TipoCarga = pedido.TipoCarga?.Descricao ?? "";
            pedidoRetornar.CodigoTipoCarga = pedido.TipoCarga?.Codigo ?? 0;
            pedidoRetornar.NumeroPedido = pedido.Numero.ToString("D");
            pedidoRetornar.CodigoFilial = pedido.Filial?.Codigo ?? 0;
            pedidoRetornar.Filial = pedido.Filial?.Descricao ?? "";
            pedidoRetornar.NumeroPedidoEmbarcador = pedido.NumeroPedidoEmbarcador;
            pedidoRetornar.Origem = pedido.Expedidor == null ? pedido.Origem?.DescricaoCidadeEstado ?? "" : pedido.Expedidor.Endereco.Localidade.DescricaoCidadeEstado;
            pedidoRetornar.Destino = pedido.Destino?.DescricaoCidadeEstado ?? "";
            pedidoRetornar.CodigoDestino = pedido.Destino?.Codigo ?? 0;
            pedidoRetornar.CodigoRecebedor = pedido.Recebedor?.Endereco.Localidade.Codigo ?? 0;
            pedidoRetornar.RemetenteCNPJ = pedido.GrupoPessoas != null ? pedido.GrupoPessoas.Descricao : pedido.Remetente?.CpfCnpj ?? "";
            pedidoRetornar.Remetente = pedido.GrupoPessoas != null ? pedido.GrupoPessoas.Descricao : pedido.Remetente?.Descricao ?? "";
            pedidoRetornar.TransportadorLocalCarregamentoRestringido = pedido.Empresa?.RestringirLocaisCarregamentoAutorizadosMotoristas ?? false;
            pedidoRetornar.Empresa = pedido.Empresa?.Descricao ?? "";
            pedidoRetornar.Destinatario = pedido.Destinatario?.Descricao ?? "";
            pedidoRetornar.DestinatarioNomeFantasia = pedido.Destinatario?.NomeFantasia ?? "";
            pedidoRetornar.Recebedor = pedido.Recebedor?.Descricao ?? "";
            pedidoRetornar.RecebedorNomeFantasia = pedido.Recebedor?.NomeFantasia ?? "";
            pedidoRetornar.DestinoRecebedor = pedido.Recebedor?.Endereco.Localidade.DescricaoCidadeEstado ?? "";
            pedidoRetornar.Expedidor = pedido.Expedidor?.Descricao ?? "";
            pedidoRetornar.CodigoExpedidor = pedido.Expedidor?.Codigo ?? 0;
            pedidoRetornar.ExpedidorExterior = pedido.Expedidor?.Tipo == "E";
            pedidoRetornar.DataCarregamentoPedido = pedido.DataCarregamentoPedido?.ToString("dd/MM/yyyy") ?? "";
            pedidoRetornar.DataHoraCarregamentoPedido = pedido.DataCarregamentoPedido?.ToString("dd/MM/yyyy HH:mm") ?? "";
            pedidoRetornar.DataInicialColeta = pedido.DataInicialColeta?.ToString("dd/MM/yyyy HH:mm") ?? "";
            pedidoRetornar.DataCarregamento = pedido.DataCarregamentoCarga?.ToString(formatacaoDataCarregamento) ?? "";
            pedidoRetornar.DataDescarregamento = pedido.DataTerminoCarregamento?.ToString(formatacaoDataCarregamento) ?? "";
            pedidoRetornar.DataPrevisaoEntrega = pedido.PrevisaoEntrega?.ToString("dd/MM/yyyy HH:mm") ?? "";
            pedidoRetornar.PesoSaldoRestante = pedido.PesoSaldoRestante.ToString("n2");
            pedidoRetornar.PalletSaldoRestante = pedido.PalletSaldoRestante.ToString("n2");
            pedidoRetornar.Peso = pedido.PesoTotal.ToString("n4");
            pedidoRetornar.PesoLiquido = pedido.PesoLiquidoTotal.ToString("n4");
            pedidoRetornar.Volumes = pedido.QtVolumes.ToString() ?? "";
            pedidoRetornar.TotalPallets = (pedido.NumeroPaletes + pedido.NumeroPaletesFracionado).ToString("n3");
            pedidoRetornar.Cubagem = pedido.CubagemTotal.ToString("n3");
            pedidoRetornar.TipoOperacao = pedido.TipoOperacao?.Descricao ?? "";
            pedidoRetornar.TipoOperacaoInformarRecebedor = pedido.TipoOperacao?.PermitirInformarRecebedorMontagemCarga ?? false;
            pedidoRetornar.CodigoTipoOperacao = pedido.TipoOperacao?.Codigo ?? 0;
            pedidoRetornar.NecessarioConfirmacaoMotorista = pedido.TipoOperacao?.ConfiguracaoMobile?.NecessarioConfirmacaoMotorista ?? false;
            pedidoRetornar.TempoLimiteConfirmacaoMotorista = pedido.TipoOperacao?.ConfiguracaoMobile?.TempoLimiteConfirmacaoMotorista.ToString(@"hh\:mm\:ss") ?? "";
            pedidoRetornar.CodigoCanalEntrega = pedido?.CanalEntrega?.Codigo ?? 0;
            pedidoRetornar.CanalEntrega = pedido?.CanalEntrega?.Descricao ?? "";
            pedidoRetornar.CanalVenda = pedido?.CanalVenda?.Descricao ?? "";
            pedidoRetornar.Ordem = pedido.Ordem ?? "";
            pedidoRetornar.NumeroReboque = NumeroReboque.SemReboque;
            pedidoRetornar.NumeroReboqueDescricao = NumeroReboque.SemReboque.ObterDescricao();
            pedidoRetornar.TipoCarregamentoPedido = TipoCarregamentoPedido.Normal;
            pedidoRetornar.TipoCarregamentoPedidoDescricao = TipoCarregamentoPedido.Normal.ObterDescricao();
            pedidoRetornar.PedidoDestinadoAFilial = pedidoDestinadoAFilial;
            pedidoRetornar.CodigoEmpresa = pedido.Empresa?.Codigo ?? 0;
            pedidoRetornar.DescricaoEmpresa = pedido.Empresa?.Descricao ?? "";
            pedidoRetornar.PedidoBloqueado = pedido.PedidoBloqueado;
            pedidoRetornar.LiberadoMontagemCarga = pedido.PedidoLiberadoMontagemCarga;
            pedidoRetornar.PedidoRestricaoData = pedido.PedidoRestricaoData;
            pedidoRetornar.PercentualSeparacaoPedido = pedido.PercentualSeparacaoPedido;
            pedidoRetornar.Tomador = tomadorPedido?.Descricao ?? "";
            pedidoRetornar.CEPDestinatario = pedido.Destinatario?.Endereco.Cep ?? "";
            pedidoRetornar.CodigoAgrupamentoCarregamento = pedido.CodigoAgrupamentoCarregamento ?? string.Empty;
            pedidoRetornar.Valor = (pedido?.ValorTotalNotasFiscais ?? 0).ToString("c");//.ToString("n4") ?? "0,00";
            pedidoRetornar.NumeroPedidoSequencial = pedido.Numero;
            pedidoRetornar.CategoriaCliente = pedido.Destinatario?.Categoria?.Descricao ?? string.Empty;
            pedidoRetornar.Distancia = pedido.Distancia.ToString("n2") ?? string.Empty;
            pedidoRetornar.ObservacaoDestinatario = pedido.Destinatario?.Observacao ?? string.Empty;
            pedidoRetornar.GrupoPessoa = (pedido.Recebedor != null ? pedido.Recebedor?.GrupoPessoas?.Descricao : pedido.Destinatario?.GrupoPessoas?.Descricao) ?? pedido.GrupoPessoas?.Descricao ?? string.Empty;
            pedidoRetornar.PrazoEntrega = pedido.DiasUteisPrazoTransportador;
            pedidoRetornar.ValorFrete = ((from o in simulacoes orderby o.SimuladorFrete.Vencedor descending select o.SimuladorFrete.ValorTotal)?.FirstOrDefault() ?? 0).ToString("c");
            pedidoRetornar.Gerente = pedido.FuncionarioGerente?.Descricao ?? string.Empty;
            pedidoRetornar.Supervisor = pedido.FuncionarioSupervisor?.Descricao ?? string.Empty;
            pedidoRetornar.PesoTotalPaletes = pedido.PesoTotalPaletes.ToString("n2");
            pedidoRetornar.TipoPaleteCliente = pedido.TipoPaleteCliente;
            pedidoRetornar.NumeroPaletesFracionado = pedido.NumeroPaletesFracionado.ToString("n3");
            pedidoRetornar.ValorTotalNotasFiscais = pedido.ValorTotalNotasFiscais > 0 ? pedido.ValorTotalNotasFiscais.ToString("n2") : "";
            pedidoRetornar.NumeroOrdem = pedido.NumeroOrdem ?? "";

            if ((pedido.Destinatario?.ValidarValorMinimoMercadoriaEntregaMontagemCarregamento ?? false) && pedido.ValorTotalNotasFiscais < (pedido.Destinatario?.ValorMinimoCarga ?? 0))
            {
                pedidoRetornar.ValorMercadoInferior = true;
                pedidoRetornar.DT_RowColor = CoresHelper.Descricao(Cores.Vermelho);
            }

            pedidoRetornar.SituacaoComercial = pedido.SituacaoComercialPedido?.Descricao ?? "";
            pedidoRetornar.SituacaoComercialCor = pedido.SituacaoComercialPedido?.Cor ?? "#FFFFFF";
            pedidoRetornar.SituacaoEstoque = pedido.PedidoAdicional?.SituacaoEstoquePedido?.Descricao ?? "";
            pedidoRetornar.SituacaoEstoqueCor = pedido.PedidoAdicional?.SituacaoEstoquePedido?.Cor ?? "#FFFFFF";

            // ATENÇÃO: Ao adicionar um atributo no retorno do serviço de pedidos, atente-se para adicionar apenas se necessário 
            //          fora do parametro montagemCarga, pois em alguns casos, quando montando com muitos pedidos está apresentando erro 500.
            if (!montagemCargaPorPedidoProduto)
            {
                pedidoRetornar.TipoCondicaoPagamento = pedido.TipoPagamento.ObterTipoCondicaoPagamento();
                pedidoRetornar.DescricaoTipoCondicaoPagamento = pedido.TipoPagamento.ObterDescricao();
                pedidoRetornar.TipoTomador = pedido.TipoTomador;
                pedidoRetornar.DescricaoTipoTomador = pedido.TipoTomador.ObterDescricao();
                pedidoRetornar.Importacao = pedido.Remetente != null && pedido.Remetente.Tipo == "E";
                pedidoRetornar.Exportacao = pedido.Destinatario != null && pedido.Destinatario.Tipo == "E";
                pedidoRetornar.PortoSaida = pedido.PortoSaida ?? "";
                pedidoRetornar.PortoChegada = pedido.PortoChegada ?? "";
                pedidoRetornar.Companhia = pedido.Companhia ?? "";
                pedidoRetornar.NumeroNavio = pedido.NumeroNavio ?? "";
                pedidoRetornar.Reserva = pedido.Reserva ?? "";
                pedidoRetornar.Resumo = pedido.Resumo ?? "";
                pedidoRetornar.Temperatura = pedido.Temperatura ?? "";
                pedidoRetornar.Vendedor = pedido.Vendedor ?? pedido.FuncionarioVendedor?.Descricao ?? string.Empty;
                pedidoRetornar.TipoEmbarque = pedido.TipoEmbarque ?? "";
                pedidoRetornar.DeliveryTerm = pedido.DeliveryTerm ?? "";
                pedidoRetornar.IdAutorizacao = pedido.IdAutorizacao ?? "";
                pedidoRetornar.DataETA = pedido.DataETA.HasValue ? pedido.DataETA.Value.ToString("dd/MM/yyyy HH:mm") : "";
                pedidoRetornar.DataInclusaoBooking = pedido.DataInclusaoBooking.HasValue ? pedido.DataInclusaoBooking.Value.ToString("dd/MM/yyyy HH:mm") : "";
                pedidoRetornar.DataInclusaoPCP = pedido.DataInclusaoPCP.HasValue ? pedido.DataInclusaoPCP.Value.ToString("dd/MM/yyyy HH:mm") : "";
                pedidoRetornar.PossuiAjudante = (pedido.Ajudante && pedido.QtdAjudantes > 0 ? "Sim, Qtd.: " + pedido.QtdAjudantes.ToString("D") : "Não");
                pedidoRetornar.PossuiAjudanteCarga = (pedido.PedidoAdicional?.AjudanteCarga ?? false && pedido.PedidoAdicional?.QtdAjudantesCarga > 0 ? "Sim, Qtd.: " + pedido.PedidoAdicional?.QtdAjudantesCarga.ToString("D") : "Não");
                pedidoRetornar.PossuiAjudanteDescarga = (pedido.PedidoAdicional?.AjudanteDescarga ?? false && pedido.PedidoAdicional?.QtdAjudantesDescarga > 0 ? "Sim, Qtd.: " + pedido.PedidoAdicional?.QtdAjudantesDescarga.ToString("D") : "Não");
                pedidoRetornar.PossuiCarga = (pedido.PossuiCarga && pedido.ValorCarga.HasValue && pedido.ValorCarga.Value > 0 ? "Sim, R$ " + pedido.ValorCarga.Value.ToString("n2") : "Não");
                pedidoRetornar.PossuiDescarga = (pedido.PossuiDescarga && pedido.ValorDescarga.HasValue && pedido.ValorDescarga.Value > 0 ? "Sim, R$ " + pedido.ValorDescarga.Value.ToString("n2") : "Não");
                pedidoRetornar.NumeroContainer = pedido.Container?.Numero ?? "";
                pedidoRetornar.CodigoPedidoViagemNavio = pedido.PedidoViagemNavio?.Codigo ?? 0;
                pedidoRetornar.PedidoViagemNavio = pedido.PedidoViagemNavio?.Descricao ?? "";
                pedidoRetornar.NumeroBooking = pedido.NumeroBooking;
                pedidoRetornar.PortoOrigem = pedido.Porto?.Descricao ?? "";
                pedidoRetornar.PortoDestino = pedido.PortoDestino?.Descricao ?? "";
                pedidoRetornar.Usuario = pedido.Usuario?.Descricao;
                pedidoRetornar.FuncionarioVendedor = (montagemCarga ? string.Empty : pedido.FuncionarioVendedor?.DescricaoTelefoneEmail ?? "");
                pedidoRetornar.CargaPerigosa = possuiCargaPerigosa ? "Sim" : "Não";
                pedidoRetornar.PermiteInformarPesoCubadoNaMontagemDaCarga = tipoOperacao?.PermiteInformarPesoCubadoNaMontagemDaCarga ?? false;
                pedidoRetornar.NaoExigeRoteirizacaoMontagemCarga = pedido.TipoOperacao?.NaoExigeRoteirizacaoMontagemCarga ?? false;
                pedidoRetornar.TipoOperacaoControlarCapacidadePorUnidade = pedido.TipoOperacao?.ConfiguracaoMontagemCarga?.ControlarCapacidadePorUnidade ?? false;
                pedidoRetornar.PedidoColetaEntrega = pedidoColetaEntrega;
                pedidoRetornar.Produto = pedido.ProdutoPredominante ?? "";
                pedidoRetornar.ValorFreteNegociado = pedido.ValorFreteNegociado;
                pedidoRetornar.ValorCobrancaFreteCombinado = pedido.ValorCobrancaFreteCombinado ?? 0;
                pedidoRetornar.PedidoPrioritario = (montagemCarga ? false : pedido.GrupoPessoas != null ? pedido.GrupoPessoas.TornarPedidosPrioritarios : pedido.Remetente != null && pedido.Remetente.GrupoPessoas != null ? pedido.Remetente.GrupoPessoas.TornarPedidosPrioritarios : false);

                pedidoRetornar.Anexos = (pedido.Anexos.Count > 0) ? (
                    from anexo in pedido.Anexos
                    select new
                    {
                        anexo.Codigo,
                        anexo.Descricao,
                        anexo.NomeArquivo,
                    }
                ).ToList() : null;

                pedidoRetornar.Fronteiras = (
                    from fronteira in pedido.Fronteiras
                    select new
                    {
                        Codigo = fronteira.Codigo,
                        Nome = fronteira.Nome,
                        Descricao = fronteira.Descricao,
                        Latitude = fronteira.Endereco.Latitude,
                        Longitude = fronteira.Endereco.Longitude,
                    }
                ).ToList();

                Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido.TipoPagamentoRecebimento formaPagamento = tomadorPedido?.FormaPagamento;
                int? diasDePrazoFatura = tomadorPedido?.DiasDePrazoFatura;

                if (!montagemCarga && (tomadorPedido?.GrupoPessoas != null))
                {
                    if (formaPagamento == null)
                        formaPagamento = tomadorPedido.GrupoPessoas.FormaPagamento;

                    if ((diasDePrazoFatura) == null || (diasDePrazoFatura == 0))
                        diasDePrazoFatura = tomadorPedido.GrupoPessoas.DiasDePrazoFatura;
                }

                pedidoRetornar.FormaPagamento = formaPagamento?.Descricao ?? string.Empty;
                pedidoRetornar.DiasDePrazoFatura = diasDePrazoFatura ?? 0;
            }

            pedidoRetornar.RotaFrete = new { Codigo = pedido.RotaFrete?.Codigo ?? 0, Descricao = pedido.RotaFrete?.Descricao ?? "" };
            pedidoRetornar.RotaFreteDescricao = pedido.RotaFrete?.Descricao ?? string.Empty;

            if (!montagemCarga)
            {
                pedidoRetornar.SenhaAgendamentoCliente = pedido.SenhaAgendamentoCliente?.Trim() ?? "";
                pedidoRetornar.ExigirPreCargaMontagemCarga = pedido.Filial?.ExigirPreCargaMontagemCarga ?? false;
                pedidoRetornar.DescricaoRecebedorColeta = pedido.RecebedorColeta?.Descricao ?? "";
                pedidoRetornar.CodigoRecebedorColeta = pedido.RecebedorColeta?.Codigo ?? 0;
                pedidoRetornar.DescricaoTipoOperacaoColeta = pedido.Remetente?.GrupoPessoas?.TipoOperacaoColeta?.Descricao ?? "";
                pedidoRetornar.CodigoTipoOperacaoColeta = pedido.Remetente?.GrupoPessoas?.TipoOperacaoColeta?.Codigo ?? 0;
                pedidoRetornar.DisponibilizarPedidoParaColeta = pedido.DisponibilizarPedidoParaColeta;
                pedidoRetornar.ObservacaoCliente = pedido.Destinatario?.Observacao ?? "";
                pedidoRetornar.ValorTotalPedido = (from p in pedido.Produtos select p.ValorProduto * p.Quantidade).Sum().ToString("N", culture);
                pedidoRetornar.Restricao = "";
                pedidoRetornar.PrimeiraEntrega = false;
                pedidoRetornar.Email = "";
                pedidoRetornar.ObservacaoRestricao = "";
                pedidoRetornar.ObservacaoPedido = pedido.Observacao;
                pedidoRetornar.Observacao = pedido.ObservacaoCTe;
                pedidoRetornar.ObservacaoInterna = pedido.ObservacaoInterna ?? string.Empty;
                pedidoRetornar.PalletsCarregados = (from obj in carregamentoPedido where obj.Carregamento.SituacaoCarregamento != SituacaoCarregamento.Cancelado select obj)?.Sum(cp => cp.Pallet).ToString("n2") ?? "0,000";
                pedidoRetornar.AtivarRecebedor = pedido.TipoOperacao?.PermitirInformarRecebedorMontagemCarga ?? false;
                pedidoRetornar.NumeroContainer = pedido.NumeroContainer ?? string.Empty;

                if (carga != null && carga.Carregamento != null)
                    pedidoRetornar.PalletsCarregadosNestaCarga = carregamentoPedido?.Where(x => x.Carregamento.Codigo == carga.Carregamento.Codigo)?.Sum(cp => cp.Pallet).ToString("n2") ?? "0,000";
            }

            pedidoRetornar.Carregamentos = string.Join(",", (from obj in carregamentoPedido where obj.Carregamento.SituacaoCarregamento != SituacaoCarregamento.Cancelado select obj.Carregamento?.NumeroCarregamento ?? string.Empty));
            pedidoRetornar.QuantidadeVolumes = pedido.QtVolumes;
            pedidoRetornar.DataAgendamento = pedido.DataAgendamento?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty;
            pedidoRetornar.SenhaAgendamento = pedido.SenhaAgendamento;
            pedidoRetornar.DataPrevisaoSaida = pedido.DataPrevisaoSaida?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty;
            pedidoRetornar.DataCriacao = pedido.DataCriacao?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty;
            pedidoRetornar.Reentrega = pedido.ReentregaSolicitada;
            pedidoRetornar.ObservacaoTipoOperacao = pedido?.TipoOperacao?.Observacao ?? string.Empty;
            pedidoRetornar.PedidoTotalmenteCarregado = pedido.PedidoTotalmenteCarregado;

            pedidoRetornar.ModeloVeicularCarga = new
            {
                Codigo = pedido.ModeloVeicularCarga?.Codigo ?? 0,
                Descricao = pedido.ModeloVeicularCarga?.Descricao ?? "",
                CapacidadePesoTransporte = pedido.ModeloVeicularCarga?.CapacidadePesoTransporte.ToString("n4") ?? "0,00",
                ToleranciaPesoMenor = pedido.ModeloVeicularCarga?.ToleranciaPesoMenor.ToString("n4") ?? "0,00",
                ModeloControlaCubagem = pedido.ModeloVeicularCarga?.ModeloControlaCubagem ?? false,
                Cubagem = pedido.ModeloVeicularCarga?.Cubagem.ToString("n2") ?? "0,00",
                ToleranciaMinimaCubagem = pedido.ModeloVeicularCarga?.ToleranciaMinimaCubagem.ToString("n2") ?? "0,00",
                VeiculoPaletizado = pedido.ModeloVeicularCarga?.VeiculoPaletizado ?? false,
                NumeroPaletes = pedido.ModeloVeicularCarga?.NumeroPaletes?.ToString() ?? "0",
                NumeroReboques = pedido.ModeloVeicularCarga?.NumeroReboques?.ToString() ?? "0",
                ToleranciaMinimaPaletes = pedido.ModeloVeicularCarga?.ToleranciaMinimaPaletes.ToString() ?? "0",
                OcupacaoCubicaPaletes = pedido.ModeloVeicularCarga?.OcupacaoCubicaPaletes.ToString("n2") ?? "0,00",
                ExigirDefinicaoReboquePedido = pedido.ModeloVeicularCarga?.ExigirDefinicaoReboquePedido ?? false
            };

            pedidoRetornar.EnderecoRemetente = pedido.Remetente != null ? new
            {
                Destinatario = pedido.Remetente.Codigo,
                Logradrouro = pedido.Remetente.Endereco.Logradouro,
                CEP = pedido.Remetente.Endereco.Cep,
                Bairro = pedido.Remetente.Endereco.Bairro,
                Numero = pedido.Remetente.Endereco.Numero,
                Localidade = pedido.Remetente.Endereco.Localidade.DescricaoCidadeEstado,
                Latitude = pedido.Remetente.Endereco.Latitude,
                Longitude = pedido.Remetente.Endereco.Longitude
            } : null;

            if (!pedido.UsarOutroEnderecoDestino)
            {
                pedidoRetornar.EnderecoDestino = pedido.Destinatario != null ? new
                {
                    Destinatario = pedido.Destinatario.Codigo,
                    Logradrouro = pedido.Destinatario.Endereco.Logradouro,
                    CEP = pedido.Destinatario.Endereco.Cep,
                    Bairro = pedido.Destinatario.Endereco.Bairro,
                    Numero = pedido.Destinatario.Endereco.Numero,
                    Localidade = pedido.Destinatario.Endereco.Localidade.DescricaoCidadeEstado,
                    Latitude = pedido.Destinatario.Endereco.Latitude,
                    Longitude = pedido.Destinatario.Endereco.Longitude
                } : null;
            }
            else
            {
                pedidoRetornar.EnderecoDestino = pedido.EnderecoDestino != null ? new
                {
                    Destinatario = pedido.Destinatario.Codigo,
                    Logradrouro = pedido.EnderecoDestino.Logradouro,
                    CEP = pedido.EnderecoDestino.Cep,
                    Bairro = pedido.EnderecoDestino.Bairro,
                    Numero = pedido.EnderecoDestino.Numero,
                    Localidade = pedido.EnderecoDestino.Localidade.DescricaoCidadeEstado,
                    Latitude = pedido.EnderecoDestino.Latitude,
                    Longitude = pedido.EnderecoDestino.Longitude
                } : null;
            }

            pedidoRetornar.EnderecoRecebedor = pedido.Recebedor != null ? new
            {
                Destinatario = pedido.Recebedor.Codigo,
                Logradrouro = pedido.Recebedor.Endereco.Logradouro,
                CEP = pedido.Recebedor.Endereco.Cep,
                Bairro = pedido.Recebedor.Endereco.Bairro,
                Numero = pedido.Recebedor.Endereco.Numero,
                Localidade = pedido.Recebedor.Endereco.Localidade.DescricaoCidadeEstado,
                pedido.Recebedor.Endereco.Latitude,
                pedido.Recebedor.Endereco.Longitude,
                pedido.Recebedor.Endereco.LatitudeTransbordo,
                pedido.Recebedor.Endereco.LongitudeTransbordo
            } : null;

            pedidoRetornar.EnderecoExpedidor = pedido.Expedidor != null ? new
            {
                Destinatario = pedido.Expedidor.Codigo,
                Logradrouro = pedido.Expedidor.Endereco.Logradouro,
                CEP = pedido.Expedidor.Endereco.Cep,
                Bairro = pedido.Expedidor.Endereco.Bairro,
                Numero = pedido.Expedidor.Endereco.Numero,
                Localidade = pedido.Expedidor.Endereco.Localidade.DescricaoCidadeEstado,
                pedido.Expedidor.Endereco.Latitude,
                pedido.Expedidor.Endereco.Longitude,
                pedido.Expedidor.Endereco.LatitudeTransbordo,
                pedido.Expedidor.Endereco.LongitudeTransbordo
            } : null;

            pedidoRetornar.VolumesBipagem = "";
            pedidoRetornar.Categoria = "";
            pedidoRetornar.TipoOperacaoExigirInformarDataPrevisaoInicioViagem = pedido.TipoOperacao?.ConfiguracaoMontagemCarga?.ExigirInformarDataPrevisaoInicioViagem ?? false;
            pedidoRetornar.TipoOperacaoValidarValorMinimoCarga = pedido.TipoOperacao?.ConfiguracaoCarga?.ValidarValorMinimoCarga ?? false;
            pedidoRetornar.OcultarTipoDeOperacaoNaMontagemDaCarga = pedido.TipoOperacao?.ConfiguracaoMontagemCarga?.OcultarTipoDeOperacaoNaMontagemDaCarga ?? false;

            pedidoRetornar.Saldo = null;

            if (possuiFilialArmazem)
            {
                if (carregamentoPedido.Count > 0)
                {
                    Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto repositorioCarregamentosPedidoProduto = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto(unitOfWork);
                    List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> carregamentosPedidoProduto = repositorioCarregamentosPedidoProduto.BuscarPorCarregamentos((from carregamento in carregamentoPedido select carregamento.Carregamento.Codigo).ToList());
                    pedidoRetornar.Saldo = carregamentosPedidoProduto.Exists(cpp => cpp.Quantidade < cpp.PedidoProduto.Quantidade) ? "Sim" : "Não";
                }
                else
                {
                    Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedidoProduto repositorioSessaoRoteirizadorPedidoProduto = new Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedidoProduto(unitOfWork);
                    List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedidoProduto> sessaoRoteirizadorPedidoProduto = repositorioSessaoRoteirizadorPedidoProduto.BuscarSessaoRoteirizadorPorPedido(pedido.Codigo);

                    pedidoRetornar.Saldo = pedido.Produtos.Any(pp => pp.Quantidade > (sessaoRoteirizadorPedidoProduto.Find(sr => pp.Codigo == sr.PedidoProduto.Codigo)?.QuantidadeProdutoSessao ?? 0)) ? "Sim" : "Não";
                }
            }

            pedidoRetornar.NotasFiscais = string.Empty;
            if (notasFiscaisPedidos != null && notasFiscaisPedidos.Count > 0)
                pedidoRetornar.NotasFiscais = string.Join(", ", (from obj in notasFiscaisPedidos where obj.CodigoPedido == pedido.Codigo select obj.NumeroNota));

            pedidoRetornar.Operador = pedido.Autor?.Nome ?? "";
            pedidoRetornar.TendenciaEntrega = ObterTendenciaEntregaPedido(pedido.PrevisaoEntrega, configuracaoTempoTendendicas).ObterDescricao();
            pedidoRetornar.LocalPaletizacao = pedido.LocalPaletizacao?.Descricao ?? "";
            pedidoRetornar.Regiao = pedido.Remetente?.Regiao?.Descricao ?? string.Empty;
            pedidoRetornar.MesoRegiao = pedido.Remetente?.MesoRegiao?.Descricao ?? string.Empty;

            Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoDetalhesAdicionais pedidoDetalheAdicional = pedidoDetalhesAdicionais?.Find(o => o.CodigoPedido == pedido.Codigo) ?? new Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoDetalhesAdicionais();

            pedidoRetornar.DataDigitalizacaoCanhotoAvulso = pedidoDetalheAdicional?.DataDigitalizacaoCanhoto.ToDateTimeString() ?? string.Empty;
            pedidoRetornar.DataEntregaNotaCanhoto = pedidoDetalheAdicional?.DataEntregaNotaCanhoto.ToDateTimeString() ?? string.Empty;
            pedidoRetornar.PrimeiroCodigoCargaEmbarcador = pedidoDetalheAdicional?.CodigoCargaEmbarcador ?? string.Empty;

            return pedidoRetornar;
        }

        private void AtualizarPreCargaAguardandoGeracao(List<Dominio.Entidades.Embarcador.PreCargas.PreCarga> preCargas, Repositorio.UnitOfWork unitOfWork)
        {
            if (preCargas.Count == 0)
                return;

            Repositorio.Embarcador.PreCargas.PreCarga repPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(unitOfWork);

            List<int> codigos = (from pre in preCargas select pre.Codigo).ToList();

            List<Dominio.Entidades.Embarcador.PreCargas.PreCarga> preCargasAtualizar = repPreCarga.BuscarPorCodigos(codigos);

            foreach (Dominio.Entidades.Embarcador.PreCargas.PreCarga pre in preCargasAtualizar)
            {
                pre.AguardandoGeracaoCarga = true;
                repPreCarga.Atualizar(pre);
            }
        }

        public void AtualizarPedidoIntegracao(Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao dadosIntegracao, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

            if (string.IsNullOrWhiteSpace(pedido.ObservacaoCTe))
                pedido.ObservacaoCTe = "";

            pedido.ObservacaoCTe += $"\n{dadosIntegracao.ObservacaoCTe}";

            repPedido.Atualizar(pedido);
        }

        public void AtualizarSituacaoMontagemContainer(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusMontagemContainer statusMontagem, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.WMS.MontagemContainer repMontagemContainer = new Repositorio.Embarcador.WMS.MontagemContainer(unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = repCargaPedido.BuscarPedidosPorCarga(codigoCarga);
            if (pedidos != null && pedidos.Count > 0)
            {
                foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidos)
                {
                    if (pedido.MontagemContainer != null)
                    {
                        pedido.MontagemContainer.Status = statusMontagem;
                        repMontagemContainer.Atualizar(pedido.MontagemContainer);
                    }
                }
            }
        }

        public List<string> VerificarPedidosPreImportacao(string dados, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, string AdminStringConexao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repositorioConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = repositorioConfiguracaoPedido.BuscarConfiguracaoPadrao();
            List<string> cargasAtrasadas = new List<string>();

            List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha>>(dados);
            for (int i = 0; i < linhas.Count; i++)
            {
                try
                {
                    string numeroCarga = ImportarPedidoVerificacaoPreImportacao(linhas[i], configuracaoPedido, configuracaoTMS, unitOfWork);

                    if (!string.IsNullOrWhiteSpace(numeroCarga))
                        cargasAtrasadas.Add(numeroCarga);
                }
                catch (Exception exception)
                {
                    Servicos.Log.TratarErro(exception);
                }
            }

            return cargasAtrasadas;
        }

        public void SetMotivosAtraso(List<(string Carga, Dominio.Entidades.Embarcador.Pedidos.MotivoImportacaoPedidoAtrasada Motivo)> motivos)
        {
            motivosAtraso = motivos;
        }

        public Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao ImportarPedido(string dados, (string Nome, string Guid) arquivoGerador, Dominio.Entidades.Usuario usuario, Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultiSoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, string AdminStringConexao, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao()
            {
                Retornolinhas = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha>()
            };

            int contador = 0;
            bool pedidoImportadoPorPlanilha = true;

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = repConfiguracaoPedido.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork).BuscarPrimeiroRegistro();

            List<Dominio.Entidades.Embarcador.PreCargas.PreCarga> preCargas = new List<Dominio.Entidades.Embarcador.PreCargas.PreCarga>();
            List<string> cargasParaCancelamento = new List<string>();
            bool possuiCargaDePreCarga = false;
            string prefixoPreCarga = Guid.NewGuid().ToString().Replace("-", "").Substring(5, 10); //cria o prefixo randomico para não correr o risco de inserir um pedido em uma pre carga já existente
            string motivoCancelamentoCarga = "Carga cancelada via importação de planilha na tela de pedidos";

            List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha>>(dados);
            bool primeiraProcessou = false;
            for (int i = 0; i < linhas.Count; i++)
            {
                try
                {
                    unitOfWork.FlushAndClear();
                    unitOfWork.Start();

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaNumeroAgrupamentoPrecarga = (from obj in linhas[i].Colunas where obj.NomeCampo == "AgrupamentoPreCarga" select obj).FirstOrDefault();

                    if (colunaNumeroAgrupamentoPrecarga != null)
                        possuiCargaDePreCarga = true;

                    // Processa linha do arquivo como um pedido isoladamente
                    Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retornoLinha = ImportarPedidoLinha(linhas[i], arquivoGerador, prefixoPreCarga, preCargas, cargasParaCancelamento, motivoCancelamentoCarga, usuario, operadorLogistica, false, tipoServicoMultisoftware, configuracaoTMS, clienteMultiSoftware, configuracaoPedido, auditado, AdminStringConexao, unitOfWork, cancellationToken, pedidoImportadoPorPlanilha);
                    retornoLinha.indice = i;
                    retornoImportacao.Retornolinhas.Add(retornoLinha);

                    // Deve contar como linha importada?
                    if (retornoLinha.contar)
                    {
                        contador++;
                        if (i == 0)
                            primeiraProcessou = true;
                    }



                    // Processou com sucesso?
                    if (retornoLinha.processou)
                        unitOfWork.CommitChanges();
                    else
                        unitOfWork.Rollback();
                }
                catch (ServicoException e)
                {
                    unitOfWork.Rollback();
                    Servicos.Log.TratarErro(e);
                    retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha($"Ocorreu uma falha ao processar a linha: {e.Message}", i));
                    continue;
                }
                catch (Exception ex2)
                {
                    unitOfWork.Rollback();
                    Servicos.Log.TratarErro(ex2);
                    retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("Ocorreu uma falha ao processar a linha.", i));
                    continue;
                }
            }

            bool criarCarga = true;
            bool deixarCargaPendente = configuracaoTMS.ImportarPedidoDeixarCargaPendente;
            string retornoFinaliza = "";

            //todo: rever, regra criada temporariamente para Nestle
            if (configuracaoTMS.UsarMesmoNumeroPreCargaGerarCargaViaImportacao && configuracaoTMS.DestinatarioPadraoImportacaoPlanilhaPedido == null)
            {
                int diferente = linhas.Count() - contador;
                if (!primeiraProcessou)
                    diferente--;

                if (diferente > 0)
                    criarCarga = false;
            }

            if (deixarCargaPendente)
                AtualizarPreCargaAguardandoGeracao(preCargas, unitOfWork);

            if (criarCarga)
            {
                if (!deixarCargaPendente)
                    FinalizarPreCargasAgMontagem(configuracaoTMS, preCargas.Select(a => a.Codigo).ToList(), cargasParaCancelamento, tipoServicoMultisoftware, unitOfWork, out retornoFinaliza, out int quantidadeCargasGeradas, configuracaoGeralCarga, operadorLogistica, motivoCancelamentoCarga, usuario, auditado);
            }
            else
            {
                retornoFinaliza = "Não foi possível importar todos os pedidos. ";

                if (!possuiCargaDePreCarga)
                    CancelarPedidosAgMontagem(configuracaoTMS, preCargas, tipoServicoMultisoftware, unitOfWork);
            }

            if (!string.IsNullOrWhiteSpace(retornoFinaliza))
                retornoFinaliza += "Por favor, tente finalizar a geração da carga para esses pedidos na tela de montagem de pré cargas.";

            retornoImportacao.MensagemAviso = retornoFinaliza;
            retornoImportacao.Total = linhas.Count();
            retornoImportacao.Importados = contador;

            return retornoImportacao;
        }

        public static void ProcessarPedidoIntegracao(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoIntegracao repPedidoIntegracao = new Repositorio.Embarcador.Pedidos.PedidoIntegracao(unitOfWork);
            List<int> integracaoesPendente = repPedidoIntegracao.BuscarPendentesIntegracao(100);

            if (integracaoesPendente != null && integracaoesPendente.Count > 0)
            {
                foreach (int codigoIntegracao in integracaoesPendente)
                    ProcessarIntegracao(unitOfWork, codigoIntegracao);
            }
        }

        public static void ProcessarBuscaOcorrenciaCorreios(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoCorreios repIntegracaoCorreios = new Repositorio.Embarcador.Configuracoes.IntegracaoCorreios(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCorreios configuracaoCorreios = repIntegracaoCorreios.BuscarPrimeiroRegistro();

            if (configuracaoCorreios != null && !string.IsNullOrWhiteSpace(configuracaoCorreios.URLEventos))
            {
                List<SituacaoEntrega> situacoesDiferente = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntregaHelper.ObterListaSituacaoEntregaFinalizada();
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);
                List<int> codigosPedidos = repCargaEntregaPedido.BuscarCodigosPedidosCorreiosPendenteEntrega(situacoesDiferente, 200);

                if (codigosPedidos != null && codigosPedidos.Count > 0)
                {
                    for (int i = 0; i < codigosPedidos.Count; i++)
                        ProcessarIntegracaoCorreios(codigosPedidos[i], configuracaoCorreios, unitOfWork, tipoServicoMultisoftware, clienteMultisoftware, auditado);
                }
            }
        }

        public static void ProcessarPedidoAguardandoRetornoIntegracao(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoIntegracao repPedidoIntegracao = new Repositorio.Embarcador.Pedidos.PedidoIntegracao(unitOfWork);
            List<int> integracaoesAGRetorno = repPedidoIntegracao.BuscarIntegracoesPendentesRetorno(100, 5);

            if (integracaoesAGRetorno != null && integracaoesAGRetorno.Count > 0)
            {
                foreach (int codigoIntegracao in integracaoesAGRetorno)
                    ProcessarIntegracaoAguardandoRetorno(unitOfWork, codigoIntegracao);
            }
        }

        public static void ProcessarPedidoEmCancelamentoIntegracao(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoIntegracao repPedidoIntegracao = new Repositorio.Embarcador.Pedidos.PedidoIntegracao(unitOfWork);
            List<int> integracaoesAGRetorno = repPedidoIntegracao.BuscarIntegracoesEmCancelmantoAguardandoIntegracao(100);

            if (integracaoesAGRetorno != null && integracaoesAGRetorno.Count > 0)
            {
                foreach (int codigoIntegracao in integracaoesAGRetorno)
                    ProcessarIntegracaoEmCancelamento(unitOfWork, codigoIntegracao);
            }
        }

        public Dominio.ObjetosDeValor.Embarcador.Pedido.RetornoImportacaoPedidoGerarCarga ImportarPedidoGerarCargas(Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedido importacaoPedido, Dominio.Entidades.Usuario usuario, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultiSoftware, string AdminStringConexao, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Dominio.ObjetosDeValor.Embarcador.Pedido.RetornoImportacaoPedidoGerarCarga retorno = new Dominio.ObjetosDeValor.Embarcador.Pedido.RetornoImportacaoPedidoGerarCarga();

            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado
            {
                TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Usuario,
                Usuario = usuario,
                Empresa = usuario?.Empresa,
                Texto = ""
            };

            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.ImportacaoPedido repImportacaoPedido = new Repositorio.Embarcador.Pedidos.ImportacaoPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.ImportacaoPedidoLinha repImportacaoPedidoLinha = new Repositorio.Embarcador.Pedidos.ImportacaoPedidoLinha(unitOfWork);
            Repositorio.Embarcador.Pedidos.ImportacaoPedidoLinhaColuna repImportacaoPedidoLinhaColuna = new Repositorio.Embarcador.Pedidos.ImportacaoPedidoLinhaColuna(unitOfWork);
            Repositorio.Embarcador.Operacional.OperadorLogistica repOperadorLogistica = new Repositorio.Embarcador.Operacional.OperadorLogistica(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.Pacote repPacote = new Repositorio.Embarcador.Cargas.Pacote(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica = usuario != null ? repOperadorLogistica.BuscarPorUsuario(usuario.Codigo) : null;

            Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao();
            retornoImportacao.Retornolinhas = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha>();

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = repConfiguracaoPedido.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();

            List<Dominio.Entidades.Embarcador.PreCargas.PreCarga> preCargas = new List<Dominio.Entidades.Embarcador.PreCargas.PreCarga>();
            List<string> cargasParaCancelamento = new List<string>();
            string prefixoPreCarga = Guid.NewGuid().ToString().Replace("-", "").Substring(5, 10);//cria o prefixo randomico para não correr o risco de inserir um pedido em uma pre carga já existente
            string motivoCancelamentoCarga = "Carga cancelada via importação de planilha na tela de pedidos";

            List<Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedidoLinha> LinhasGerar = repImportacaoPedidoLinha.BuscarLinhasPendentesGeracaoPedido(importacaoPedido.Codigo);


            List<Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedidoLinhaColuna> colunasGerar = repImportacaoPedidoLinhaColuna.BuscarPorImportacaoPendentesGeracaoPedido(importacaoPedido.Codigo);
            int contador = 0;
            int contadorPacotes = 0;
            int quantidadeCargasGeradas = 0;
            int primeiroPedido = 0;
            bool possuiCargaDePreCarga = false;
            string retornoFinaliza = "";
            bool possuiColunaShopee = false;
            Dominio.Entidades.Cliente remetente = null;
            Dominio.Entidades.Cliente destinatario = null;

            try
            {
                // Importa cada linha como um pedido
                possuiColunaShopee = false;
                Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedidoLinhaColuna colunaShopee = null;
                if (LinhasGerar != null && LinhasGerar.Count > 0)
                    colunaShopee = (from obj in LinhasGerar.ElementAt(0).Colunas where obj.NomeCampo == "PedidosShopee" select obj).FirstOrDefault();

                if (colunaShopee != null)
                    possuiColunaShopee = true;

                #region Monta Cache de buscas 
                List<Dominio.Entidades.Empresa> lstEmpresa = null;
                List<Dominio.Entidades.Cliente> lstClientes = null;
                List<Dominio.Entidades.Cliente> lstClientesPorCodigoIntegracao = null;
                List<Dominio.Entidades.Embarcador.Filiais.Filial> lstFilial = null;
                List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> lstTipoCarga = null;
                List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> lstModeloVeicularCarga = null;
                List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> lstTipoOperacao = null;
                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> lstPedidosOrigem = null;
                List<Dominio.Entidades.Embarcador.Cargas.Carga> lstCargas = null;
                List<Dominio.Entidades.Embarcador.PreCargas.PreCarga> lstPreCarga = null;


                if (configuracaoPedido.ImportarParalelizando)
                    MontaCache(prefixoPreCarga, unitOfWork, configuracaoTMS, LinhasGerar, colunasGerar, ref lstEmpresa, ref lstClientes, ref lstClientesPorCodigoIntegracao, ref lstFilial, ref lstTipoCarga, ref lstModeloVeicularCarga, ref lstTipoOperacao, ref lstPedidosOrigem, ref lstCargas, ref lstPreCarga);

                #endregion

                for (int i = 0; i < LinhasGerar.Count; i++)
                {
                    Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedidoLinha linha = LinhasGerar.ElementAt(i);
                    if (!possuiColunaShopee)
                    {
                        Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedidoLinhaColuna colunaNumeroAgrupamentoPrecarga = (from obj in linha.Colunas where obj.NomeCampo == "AgrupamentoPreCarga" select obj).FirstOrDefault();
                        if (colunaNumeroAgrupamentoPrecarga != null)
                            possuiCargaDePreCarga = true;
                    }

                    unitOfWork.Start();
                    linha.Situacao = SituacaoImportacaoPedido.Processando;
                    repImportacaoPedidoLinha.Atualizar(linha);
                    unitOfWork.CommitChanges();

                    List<Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedidoLinhaColuna> colunas = colunasGerar.Where(o => o.Linha.Codigo == linha.Codigo).ToList();
                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha dadosLinha = Servicos.Embarcador.Pedido.ImportacaoPedido.ConverterParaImportacao(colunas);

                    unitOfWork.Start();

                    try
                    {
                        Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retornoLinha = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha();
                        if (possuiColunaShopee)
                        {
                            if (i > 0)
                            {// livra o cabeçalho 
                                if (i == 1)
                                {// vai criar o pedido 
                                    retornoLinha = ImportarPedidoLinha(dadosLinha, ValueTuple.Create("", ""), prefixoPreCarga, preCargas, cargasParaCancelamento, motivoCancelamentoCarga, usuario, operadorLogistica, true, tipoServicoMultisoftware, configuracaoTMS, clienteMultiSoftware, configuracaoPedido, auditado, AdminStringConexao, unitOfWork, cancellationToken, false, lstEmpresa, lstClientes, lstClientesPorCodigoIntegracao, lstFilial, lstTipoCarga, lstModeloVeicularCarga, lstTipoOperacao, lstPedidosOrigem, lstCargas, lstPreCarga);
                                    primeiroPedido = 0;

                                    if (!retornoLinha.processou)
                                    {
                                        unitOfWork.Rollback();
                                        unitOfWork.Start();
                                        linha.Situacao = SituacaoImportacaoPedido.Erro;
                                        linha.Mensagem = $"Importação abortada {retornoLinha.mensagemFalha}";
                                        repImportacaoPedidoLinha.Atualizar(linha);
                                        retorno.Sucesso = false;
                                        retorno.Mensagem = linha.Mensagem;
                                        unitOfWork.CommitChanges();
                                        return retorno;
                                    }
                                    else
                                        primeiroPedido = retornoLinha.codigo;
                                }

                                retornoLinha = ImportarCargaPedidoPacoteLinha(dadosLinha, primeiroPedido, unitOfWork, ref remetente, ref destinatario);
                                if (!retornoLinha.processou)
                                {
                                    unitOfWork.Rollback();
                                    unitOfWork.Start();
                                    linha.Situacao = SituacaoImportacaoPedido.Erro;
                                    linha.Mensagem = $"Importação abortada {retornoLinha.mensagemFalha}";
                                    repImportacaoPedidoLinha.Atualizar(linha);
                                    retorno.Sucesso = false;
                                    retorno.Mensagem = linha.Mensagem;
                                    unitOfWork.CommitChanges();
                                    return retorno;
                                }

                                contadorPacotes += 1;
                                if (contadorPacotes >= 50 && PacotesCache.Count >= 50)
                                {
                                    repPacote.InserirImportacaoPacotes(PacotesCache, primeiroPedido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Shopee);
                                    PacotesCache = new List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoPacoteImportacao>();
                                    contadorPacotes = 0;
                                }
                            }
                        }
                        else
                            retornoLinha = ImportarPedidoLinha(dadosLinha, ValueTuple.Create("", ""), prefixoPreCarga, preCargas, cargasParaCancelamento, motivoCancelamentoCarga, usuario, operadorLogistica, true, tipoServicoMultisoftware, configuracaoTMS, clienteMultiSoftware, configuracaoPedido, auditado, AdminStringConexao, unitOfWork, cancellationToken, false, lstEmpresa, lstClientes, lstClientesPorCodigoIntegracao, lstFilial, lstTipoCarga, lstModeloVeicularCarga, lstTipoOperacao, lstPedidosOrigem, lstCargas, lstPreCarga);


                        if (retornoLinha.contar)
                        {
                            if (retornoLinha.codigo > 0)
                            {
                                linha.Pedido = new Dominio.Entidades.Embarcador.Pedidos.Pedido() { Codigo = retornoLinha.codigo };
                                linha.Situacao = SituacaoImportacaoPedido.Sucesso;
                                linha.Mensagem = "Pedido importado.";
                                repImportacaoPedidoLinha.Atualizar(linha);
                                contador++;
                            }
                        }
                        else
                        {
                            unitOfWork.Rollback();
                            unitOfWork.Start();
                            linha.Situacao = SituacaoImportacaoPedido.Erro;
                            linha.Mensagem = retornoLinha.mensagemFalha;
                            repImportacaoPedidoLinha.Atualizar(linha);
                        }

                        unitOfWork.CommitChanges();
                        unitOfWork.FlushAndClear();
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        unitOfWork.Start();
                        linha.Situacao = SituacaoImportacaoPedido.Erro;
                        linha.Mensagem = ex.Message;
                        repImportacaoPedidoLinha.Atualizar(linha);
                        unitOfWork.CommitChanges();
                    }
                }

                if (possuiColunaShopee)
                {
                    unitOfWork.Start();
                    try
                    {
                        if (PacotesCache.Count > 0)
                        {
                            repPacote.InserirImportacaoPacotes(PacotesCache, primeiroPedido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Shopee);
                            PacotesCache = new List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoPacoteImportacao>();
                        }

                        Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedidoLinha linha = repImportacaoPedidoLinha.BuscarPorPedido(primeiroPedido);


                        List<Dominio.Entidades.Embarcador.Pedidos.Pedido> listaPedido = new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>() {
                            repPedido.BuscarPorCodigo(primeiroPedido)
                        };

                        listaPedido[0].CodigoCargaEmbarcador = codigoCargaEmbarcadorShopee;

                        string mensagemRetornoCarga = CriarCarga(out Dominio.Entidades.Embarcador.Cargas.Carga carga, listaPedido, unitOfWork, tipoServicoMultisoftware, clienteMultiSoftware, configuracaoTMS, true, false, false, false);
                        if (!string.IsNullOrWhiteSpace(mensagemRetornoCarga))
                        {
                            unitOfWork.Rollback();
                            unitOfWork.Start();

                            linha.Situacao = SituacaoImportacaoPedido.Erro;
                            linha.Mensagem = (linha.Mensagem + " " + mensagemRetornoCarga).Trim();
                            repImportacaoPedidoLinha.Atualizar(linha);
                        }

                        if (carga != null)
                        {
                            carga.RotaRecorrente = true;
                            repCarga.Atualizar(carga);
                        }

                        unitOfWork.CommitChanges();
                        unitOfWork.Start();

                        Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorPedido(primeiroPedido).FirstOrDefault();
                        if (cargaPedido != null)
                        {
                            repImportacaoPedidoLinha.SetarCargaLinha(cargaPedido.Pedido.Codigo, cargaPedido.Carga.Codigo);
                            cargaPedido.TipoContratacaoCarga = TipoContratacaoCarga.Redespacho;
                            repCargaPedido.Atualizar(cargaPedido);
                        }

                        repPacote.InserirImportacaoCargaPedidoPacote(cargaPedido);
                        retorno.TotalCargas = repImportacaoPedidoLinha.ContarCargasPorImportacaoPedido(importacaoPedido.Codigo);

                        unitOfWork.CommitChanges();
                        unitOfWork.FlushAndClear();
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        Servicos.Log.TratarErro(ex);
                        retorno.Sucesso = false;
                        retorno.Mensagem = ex.Message;

                    }
                }

                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    // Agrupa os pedidos por veículo e tipo de operação
                    List<Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedidoLinha> linhasGeradas = repImportacaoPedidoLinha.BuscarSemCargaPorImportacaoPedido(importacaoPedido.Codigo);
                    List<List<Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedidoLinha>> linhasAgrupadas = AgruparLinhasPorVeiculoOperacao(linhasGeradas);

                    // Gera uma carga para cada grupo de pedidos agrupados por veículo e tipo de operação
                    foreach (List<Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedidoLinha> agrupamentoLinhas in linhasAgrupadas)
                    {
                        if (agrupamentoLinhas.Count <= 0)
                            continue;

                        List<int> codigosPedidos = agrupamentoLinhas.Select(o => o.Pedido.Codigo).ToList();
                        List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = repPedido.BuscarPorCodigos(codigosPedidos);
                        List<Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedidoLinha> linhas = new List<Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedidoLinha>();

                        if (codigosPedidos.Count < 2000)
                            linhas = repImportacaoPedidoLinha.BuscarPorPedidos(codigosPedidos);
                        else
                        {
                            try
                            {
                                decimal decimalBlocos = Math.Ceiling(((decimal)codigosPedidos.Count) / 1000);
                                int blocos = (int)Math.Truncate(decimalBlocos);

                                for (int i = 0; i < blocos; i++)
                                {
                                    Log.TratarErro($"blocos {codigosPedidos.Count} indice {i}");
                                    linhas.AddRange(repImportacaoPedidoLinha.BuscarPorPedidos(codigosPedidos.Skip(i * 1000).Take(1000).ToList()));
                                }
                            }
                            catch (Exception excecao)
                            {
                                Log.TratarErro(excecao);
                            }
                        }

                        unitOfWork.Start();
                        try
                        {
                            string mensagemRetornoCarga = CriarCarga(out Dominio.Entidades.Embarcador.Cargas.Carga carga, pedidos, unitOfWork, tipoServicoMultisoftware, clienteMultiSoftware, configuracaoTMS, true, false, false, false);
                            if (!string.IsNullOrWhiteSpace(mensagemRetornoCarga))
                            {
                                unitOfWork.Rollback();

                                unitOfWork.Start();
                                int total = linhas.Count();
                                for (int i = 0; i < total; i++)
                                {
                                    linhas[i].Situacao = SituacaoImportacaoPedido.Erro;
                                    linhas[i].Mensagem = (linhas[i].Mensagem + " " + mensagemRetornoCarga).Trim();
                                    repImportacaoPedidoLinha.Atualizar(linhas[i]);
                                }
                            }

                            if (carga != null)
                            {
                                carga.RotaRecorrente = true;
                                repCarga.Atualizar(carga);
                                repImportacaoPedidoLinha.SetarCargaLinhas(agrupamentoLinhas.Select(o => o.Codigo).ToList(), carga.Codigo);
                            }

                            unitOfWork.CommitChanges();
                            unitOfWork.FlushAndClear();
                        }
                        catch (Exception ex)
                        {
                            unitOfWork.Rollback();
                            int total = linhas.Count();
                            for (int i = 0; i < total; i++)
                            {
                                linhas[i].Situacao = SituacaoImportacaoPedido.Erro;
                                linhas[i].Mensagem = (linhas[i].Mensagem + " " + ex.Message).Trim();
                                repImportacaoPedidoLinha.Atualizar(linhas[i]);
                            }
                            unitOfWork.CommitChanges();
                        }
                    }
                }
                else
                {
                    bool criarCarga = true;
                    bool deixarCargaPendente = configuracaoTMS.ImportarPedidoDeixarCargaPendente;

                    //todo: rever, regra criada temporariamente para Nestle
                    if (configuracaoTMS.UsarMesmoNumeroPreCargaGerarCargaViaImportacao && configuracaoTMS.DestinatarioPadraoImportacaoPlanilhaPedido == null)
                    {
                        int diferente = LinhasGerar.Count - contador;

                        if (diferente > 0)
                            criarCarga = false;
                    }

                    if (deixarCargaPendente)
                        AtualizarPreCargaAguardandoGeracao(preCargas, unitOfWork);

                    if (criarCarga)
                    {
                        if (!deixarCargaPendente)
                            FinalizarPreCargasAgMontagem(configuracaoTMS, preCargas.Select(a => a.Codigo).ToList(), cargasParaCancelamento, tipoServicoMultisoftware, unitOfWork, out retornoFinaliza, out quantidadeCargasGeradas, configuracaoGeralCarga, operadorLogistica, motivoCancelamentoCarga, usuario, auditado);
                    }
                    else
                    {
                        retornoFinaliza = "Não foi possível importar todos os pedidos. ";

                        if (!possuiCargaDePreCarga)
                            CancelarPedidosAgMontagem(configuracaoTMS, preCargas, tipoServicoMultisoftware, unitOfWork);
                    }

                    if (!string.IsNullOrWhiteSpace(retornoFinaliza))
                        retornoFinaliza += "Por favor, tente finalizar a geração da carga para esses pedidos na tela de montagem de pré cargas.";
                }

                int totalCargas = repImportacaoPedidoLinha.ContarCargasPorImportacaoPedido(importacaoPedido.Codigo);

                retorno.TotalPedidos = repImportacaoPedidoLinha.ContarPedidosPorImportacaoPedido(importacaoPedido.Codigo);
                retorno.TotalCargas = totalCargas > 0 ? totalCargas : quantidadeCargasGeradas;
                retorno.Sucesso = (retorno.TotalPedidos > 0 || retorno.TotalCargas > 0);
                retorno.Mensagem = string.IsNullOrWhiteSpace(retornoFinaliza) ? "Importação de pedidos e geração de cargas finalizada com sucesso." : retornoFinaliza;
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(ex);

                retorno.Sucesso = false;
                retorno.Mensagem = ex.Message;
            }

            return retorno;
        }
        public string ImportarPedidoVerificacaoPreImportacao(Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            CultureInfo cultura = new CultureInfo("pt-BR");

            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNumeroPreCarga = (from obj in linha.Colunas where obj.NomeCampo == "NumeroPreCarga" select obj).FirstOrDefault();
            string numeroPreCarga = "";
            if (colNumeroPreCarga != null)
                numeroPreCarga = colNumeroPreCarga.Valor;

            if (string.IsNullOrWhiteSpace(numeroPreCarga))
                return null;

            string dataAgrupamentoPreCarga = "";
            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNumeroAgrupamentoPrecarga = (from obj in linha.Colunas where obj.NomeCampo == "AgrupamentoPreCarga" select obj).FirstOrDefault();
            if (colNumeroAgrupamentoPrecarga != null)
            {
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colFilial = (from obj in linha.Colunas where obj.NomeCampo == "Filial" select obj).FirstOrDefault();
                string codigoIntegracaoFilial = "";
                if (colFilial != null)
                    codigoIntegracaoFilial = (string)colFilial.Valor;

                numeroPreCarga = (string)colNumeroAgrupamentoPrecarga.Valor.PadLeft(6, '0');

                if (!string.IsNullOrEmpty(dataAgrupamentoPreCarga))
                    numeroPreCarga = dataAgrupamentoPreCarga + codigoIntegracaoFilial + numeroPreCarga;
            }

            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNumeroPedido = (from obj in linha.Colunas where obj.NomeCampo == "NumeroPedido" select obj).FirstOrDefault();
            string numeroPedido = "";
            if (colNumeroPedido != null)
            {
                numeroPedido = (string)colNumeroPedido.Valor;
                if ((configuracaoPedido?.ConcatenarNumeroPreCargaNoPedido ?? false) && !string.IsNullOrWhiteSpace(numeroPreCarga))
                    numeroPedido = numeroPreCarga + "_" + numeroPedido;

                if ((configuracaoPedido?.NaoPermitirImportarPedidosExistentes ?? false) && repositorioPedido.VerificarExistenciaPedido(numeroPedido))
                    return null;
            }

            Dominio.Entidades.Embarcador.Cargas.Carga cargaPreCarga = null;
            if (colNumeroAgrupamentoPrecarga == null)
            {
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colFilial = (from obj in linha.Colunas where obj.NomeCampo == "Filial" select obj).FirstOrDefault();
                Dominio.Entidades.Embarcador.Filiais.Filial filial = null;

                if (colFilial != null)
                    filial = repFilial.buscarPorCodigoEmbarcador((string)colFilial.Valor);

                if (configuracaoTMS.UsarMesmoNumeroPreCargaGerarCargaViaImportacao && !string.IsNullOrWhiteSpace(numeroPreCarga))
                    cargaPreCarga = repCarga.BuscarPorCodigoVinculado(numeroPreCarga);
                else
                    cargaPreCarga = repositorioPedido.BuscarCargaDePreCargaPorPedido(numeroPedido, filial?.CodigoFilialEmbarcador ?? "");
            }

            if (cargaPreCarga == null)
                return null;

            DateTime? dataHoraPrevisaoInicioViagem = cargaPreCarga.DataInicioViagemPrevista;
            if (!dataHoraPrevisaoInicioViagem.HasValue || dataHoraPrevisaoInicioViagem.Value > DateTime.Now)
                return null;

            return numeroPreCarga;
        }

        public void MontaCache(string prefixoPreCarga, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, List<Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedidoLinha> LinhasGerar, List<Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedidoLinhaColuna> colunasGerar, ref List<Dominio.Entidades.Empresa> lstEmpresa, ref List<Dominio.Entidades.Cliente> lstClientes, ref List<Dominio.Entidades.Cliente> lstClientesPorCodigoIntegracao, ref List<Dominio.Entidades.Embarcador.Filiais.Filial> lstFilial, ref List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> lstTipoCarga, ref List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> lstModeloVeicularCarga, ref List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> lstTipoOperacao, ref List<Dominio.Entidades.Embarcador.Pedidos.Pedido> lstPedidosOrigem, ref List<Dominio.Entidades.Embarcador.Cargas.Carga> lstCargas, ref List<Dominio.Entidades.Embarcador.PreCargas.PreCarga> lstPreCarga)
        {
            try
            {
                List<string> lstCnpjTransportador = new List<string>();
                List<double> lstCnpjClientes = new List<double>();
                List<string> lstCodigosIntegracaoClientes = new List<string>();
                List<string> lstCodigoIntegracaoFilial = new List<string>();
                List<string> lstCodigoTipoCarga = new List<string>();
                List<string> lstDescricaoTipoCarga = new List<string>();
                List<string> lstdescricaotipocarga = new List<string>();
                List<string> lstCodigosModeloVeicularCarga = new List<string>();
                List<string> lstNumeroPedidoOrigem = new List<string>();
                List<string> lstNumeroPreCarga = new List<string>();




                for (int i = 0; i < LinhasGerar.Count; i++)
                {
                    Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedidoLinha linha = LinhasGerar.ElementAt(i);

                    List<Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedidoLinhaColuna> colunas = colunasGerar.Where(o => o.Linha.Codigo == linha.Codigo).ToList();
                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha dadosLinha = Servicos.Embarcador.Pedido.ImportacaoPedido.ConverterParaImportacao(colunas);


                    foreach (Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna col in dadosLinha.Colunas)
                    {
                        if (col.NomeCampo == "CNPJTransportadora")
                        {
                            string somenteNumeros = Utilidades.String.OnlyNumbers(col.Valor);
                            if (!string.IsNullOrWhiteSpace(somenteNumeros))
                                if (!string.IsNullOrEmpty(somenteNumeros) && (somenteNumeros.Length > 6 || configuracaoTMS.Pais == TipoPais.Exterior))
                                    lstCnpjTransportador.Add(configuracaoTMS.Pais != TipoPais.Exterior ? long.Parse(somenteNumeros).ToString("d14") : somenteNumeros);
                        }

                        if (col.NomeCampo == "CNPJCPFRemetente" || col.NomeCampo == "CNPJCPFDestinatario" || col.NomeCampo == "Fronteira")
                        {
                            string somenteNumeros = Utilidades.String.OnlyNumbers((string)col.Valor);
                            if (!string.IsNullOrEmpty(somenteNumeros) && (somenteNumeros.Length > 5 || configuracaoTMS.Pais == TipoPais.Exterior))
                                lstCnpjClientes.Add(double.Parse(somenteNumeros));
                        }

                        if (col.NomeCampo == "CodigoRemetente" || col.NomeCampo == "CodigoDestinatario" || col.NomeCampo == "CodigoExpedidor" || col.NomeCampo == "CodigoRecebedor")
                            if (col.Valor != null)
                                lstCodigosIntegracaoClientes.Add((string)col.Valor);


                        if (col.NomeCampo == "CNPJCPFExpedidor" || col.NomeCampo == "CNPJCPFRecebedor" || col.NomeCampo == "CNPJCPFTomador" || col.NomeCampo == "CNPJCPFLocalExpedicao")
                        {
                            double cpfCNPJ = Utilidades.String.OnlyNumbers((string)col.Valor).ToDouble();
                            if (cpfCNPJ > 0d)
                                lstCnpjClientes.Add(cpfCNPJ);
                        }

                        if (col.NomeCampo == "Filial")
                            lstCodigoIntegracaoFilial.Add((string)col.Valor);

                        if (col.NomeCampo == "TipoCarga")
                        {
                            lstCodigoTipoCarga.Add((string)col.Valor);
                            lstDescricaoTipoCarga.Add((string)col.Valor);
                        }

                        if (col.NomeCampo == "ModeloVeicularCarga")
                            lstCodigosModeloVeicularCarga.Add((string)col.Valor);

                        if (col.NomeCampo == "NumeroPedidoOrigem" && col?.Valor != null)
                            lstNumeroPedidoOrigem.Add(col.Valor);


                        if (col.NomeCampo == "NumeroPreCarga")
                        {
                            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNumeroAgrupamentoPrecarga = null;
                            bool cargaDePreCarga = false;
                            string numeroPreCarga = PossuiNumeroPreCarga(dadosLinha, prefixoPreCarga, configuracaoTMS, ref colNumeroAgrupamentoPrecarga, ref cargaDePreCarga);
                            if (!string.IsNullOrEmpty(numeroPreCarga))
                                lstNumeroPreCarga.Add(numeroPreCarga);
                        }

                    }
                }

                Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                lstCodigoIntegracaoFilial = lstCodigoIntegracaoFilial.Distinct().ToList();
                lstFilial = repFilial.buscarPorCodigoEmbarcadorEOutrosCodigos(lstCodigoIntegracaoFilial);
                foreach (Dominio.Entidades.Embarcador.Filiais.Filial filial in lstFilial)
                {
                    if (filial.CNPJ != null)
                        lstCnpjClientes.Add(double.Parse(Utilidades.String.OnlyNumbers(filial.CNPJ)));
                }


                lstCnpjTransportador = lstCnpjTransportador.Distinct().ToList();
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                lstEmpresa = repEmpresa.BuscarPorCNPJs(lstCnpjTransportador);

                if (configuracaoTMS.RemetentePadraoImportacaoPlanilhaPedido != null)
                    lstCnpjClientes.Add(configuracaoTMS.RemetentePadraoImportacaoPlanilhaPedido.CPF_CNPJ);

                lstCnpjClientes = lstCnpjClientes.Distinct().ToList();
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                lstClientes = repCliente.BuscarPorCPFCNPJsParaCache(lstCnpjClientes);

                lstCodigosIntegracaoClientes = lstCodigosIntegracaoClientes.Distinct().ToList();
                lstClientesPorCodigoIntegracao = repCliente.BuscarPorCodigosIntegracao(lstCodigosIntegracaoClientes);

                lstCodigoTipoCarga = lstCodigoTipoCarga.Distinct().ToList();
                lstDescricaoTipoCarga = lstDescricaoTipoCarga.Distinct().ToList();

                Repositorio.Embarcador.Cargas.TipoDeCarga repTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
                lstTipoCarga = repTipoDeCarga.BuscarPorDescricaoECodigosEmbarcador(lstDescricaoTipoCarga, lstCodigoTipoCarga, true);

                lstCodigosModeloVeicularCarga = lstCodigosModeloVeicularCarga.Distinct().ToList();
                Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
                lstModeloVeicularCarga = repModeloVeicularCarga.buscarPorCodigosIntegracaoEDescricoesComFetch(lstCodigosModeloVeicularCarga);

                Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                lstTipoOperacao = repTipoOperacao.BuscarTodos();

                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                lstPedidosOrigem = repPedido.BuscarPorNumerosPedidoEmbarcador(lstNumeroPedidoOrigem);

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                lstCargas = repCarga.BuscarCargasPornumeroCargaVincularPreCarga(lstNumeroPreCarga);


                Repositorio.Embarcador.PreCargas.PreCarga repPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(unitOfWork);
                lstPreCarga = repPreCarga.BuscarPorNumerosPreCarga(lstNumeroPreCarga);

            }
            catch (Exception)
            {

            }
        }

        public Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha ImportarPedidoLinha(Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha, (string Nome, string Guid) arquivoGerador, string prefixoPreCarga, List<Dominio.Entidades.Embarcador.PreCargas.PreCarga> preCargas, List<string> cargasParaCancelamento, string motivoCancelamentoCarga, Dominio.Entidades.Usuario usuario, Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica, bool naoTentarGerarCarga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, string AdminStringConexao, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken, bool pedidoImportadoPorPlanilha = false, List<Dominio.Entidades.Empresa> lstEmpresa = null, List<Dominio.Entidades.Cliente> lstCpfCNPJClientes = null, List<Dominio.Entidades.Cliente> lstClientesPorCodigoIntegracao = null, List<Dominio.Entidades.Embarcador.Filiais.Filial> lstFilial = null, List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> lstTipoCarga = null, List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> lstModeloVeicularCarga = null, List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> lstTipoOperacao = null, List<Dominio.Entidades.Embarcador.Pedidos.Pedido> lstPedidosOrigem = null, List<Dominio.Entidades.Embarcador.Cargas.Carga> lstCargas = null, List<Dominio.Entidades.Embarcador.PreCargas.PreCarga> lstPreCarga = null)
        {
            Repositorio.Embarcador.Cargas.TipoDeCarga repTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaDadosSumarizados repCargaSumarizado = new Repositorio.Embarcador.Cargas.CargaDadosSumarizados(unitOfWork);
            Repositorio.Embarcador.PreCargas.PreCarga repPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Pessoas.ClienteBuscaAutomatica repositorioBuscaCliente = new Repositorio.Embarcador.Pessoas.ClienteBuscaAutomatica(unitOfWork);

            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
            Repositorio.Embarcador.Pessoas.ClienteOutroEndereco repositorioClienteOutroEndereco = new Repositorio.Embarcador.Pessoas.ClienteOutroEndereco(unitOfWork);

            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoNotaParcial repositorioPedidoNotaParcial = new Repositorio.Embarcador.Pedidos.PedidoNotaParcial(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParcial repositorioPedidoCTeParcial = new Repositorio.Embarcador.Pedidos.PedidoCTeParcial(unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Usuario repMotorista = new Repositorio.Usuario(unitOfWork);
            Repositorio.RotaFrete repRotaFrete = new Repositorio.RotaFrete(unitOfWork);
            Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
            Repositorio.Embarcador.Produtos.GrupoProduto repGrupoProduto = new Repositorio.Embarcador.Produtos.GrupoProduto(unitOfWork);
            Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork);
            Repositorio.Embarcador.Pedidos.CanalEntrega repCanalEntrega = new Repositorio.Embarcador.Pedidos.CanalEntrega(unitOfWork);
            Repositorio.Embarcador.Pedidos.LinhaSeparacao repLinhaSeparacao = new Repositorio.Embarcador.Pedidos.LinhaSeparacao(unitOfWork);
            Repositorio.Embarcador.WMS.Deposito repDeposito = new Repositorio.Embarcador.WMS.Deposito(unitOfWork);
            Repositorio.Localidade repositorioLocalidade = new Localidade(unitOfWork);
            Repositorio.Embarcador.Pedidos.ContainerTipo repTipoContainer = new Repositorio.Embarcador.Pedidos.ContainerTipo(unitOfWork);
            Repositorio.Embarcador.Pedidos.Container repContainer = new Repositorio.Embarcador.Pedidos.Container(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Logistica.CentroCarregamento repCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unitOfWork);
            Repositorio.Embarcador.Logistica.CentroCustoViagem repCentroDeCustoViagem = new Repositorio.Embarcador.Logistica.CentroCustoViagem(unitOfWork);
            Repositorio.Embarcador.Escrituracao.ContratoFreteCliente repContratoFreteCliente = new Repositorio.Embarcador.Escrituracao.ContratoFreteCliente(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ConfiguracaoDisponibilidadeCarregamento configuracaoDisponibilidadeCarregamento = new Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ConfiguracaoDisponibilidadeCarregamento()
            {
                PermitirHorarioCarregamentoComLimiteAtingido = true
            };
            Servicos.Embarcador.Logistica.CargaJanelaCarregamentoDisponibilidade servicoCargaJanelaCarregamentoDisponibilidade = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoDisponibilidade(unitOfWork, configuracaoTMS, configuracaoDisponibilidadeCarregamento);
            Servicos.WebService.Empresa.Motorista serMotorista = new WebService.Empresa.Motorista(unitOfWork);
            Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta servicoCargaJanelaCarregamentoConsulta = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta(unitOfWork);
            Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, configuracaoTMS);
            Servicos.Embarcador.Pedido.Pedido serPedido = new Servicos.Embarcador.Pedido.Pedido(unitOfWork);

            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("pt-BR");

            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retornoLinhaPedido = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha();
            Repositorio.Embarcador.Veiculos.LicencaVeiculo repLicencaVeiculo = new Repositorio.Embarcador.Veiculos.LicencaVeiculo(unitOfWork);

            try
            {
                bool horarioCarregamentoInformadoNoPedido = false;

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna coltransportador = (from obj in linha.Colunas where obj.NomeCampo == "CNPJTransportadora" select obj).FirstOrDefault();
                Dominio.Entidades.Empresa empresa = null;
                string cnpjTransportador = string.Empty;

                if (coltransportador != null)
                {
                    string somenteNumeros = Utilidades.String.OnlyNumbers(coltransportador.Valor);
                    if (!string.IsNullOrWhiteSpace(somenteNumeros))
                    {
                        if (!string.IsNullOrEmpty(somenteNumeros) && (somenteNumeros.Length > 6 || configuracaoTMS.Pais == TipoPais.Exterior))
                        {
                            cnpjTransportador = configuracaoTMS.Pais != TipoPais.Exterior ? long.Parse(somenteNumeros).ToString("d14") : somenteNumeros;
                            empresa = repEmpresa.BuscarPorCNPJ(cnpjTransportador, lstEmpresa);
                            if (empresa == null || empresa.Status == "I")
                            {
                                if (configuracaoPedido?.BuscarEmpresaPeloProprietarioDoVeiculo ?? false)
                                    empresa = repVeiculo.BuscarEmpresaPorProprietario(cnpjTransportador.ToDouble());

                                if (empresa == null || empresa.Status == "I")
                                    return RetornarFalhaLinha(Localization.Resources.Pedidos.Pedido.AEmpresaInformadaNaoExisteBaseMultisoftware);
                            }
                        }
                        else
                        {
                            return RetornarFalhaLinha(Localization.Resources.Pedidos.Pedido.RegistroIgnoradoImportacao, true);
                        }
                    }
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colRemetente = (from obj in linha.Colunas where obj.NomeCampo == "CNPJCPFRemetente" select obj).FirstOrDefault();
                Dominio.Entidades.Cliente remetente = null;
                if (colRemetente != null)
                {
                    string somenteNumeros = Utilidades.String.OnlyNumbers((string)colRemetente.Valor);
                    if (!string.IsNullOrEmpty(somenteNumeros) && (somenteNumeros.Length > 5 || configuracaoTMS.Pais == TipoPais.Exterior))
                    {
                        double cpfCNPJRemetente = double.Parse(somenteNumeros);
                        remetente = repCliente.BuscarPorCPFCNPJ(cpfCNPJRemetente, null, lstCpfCNPJClientes);
                        if (remetente == null)
                        {
                            string mensagemValidacao = CriarParticipante(ref remetente, (string)colRemetente.Valor, linha, "Remetente", AdminStringConexao, configuracaoTMS, unitOfWork);
                            if (remetente == null)
                            {
                                return RetornarFalhaLinha(!string.IsNullOrWhiteSpace(mensagemValidacao) ? mensagemValidacao : Localization.Resources.Pedidos.Pedido.ORemetenteInformadoNaoCadastradoBaseMultisoftware);
                            }
                        }
                    }
                    else
                    {
                        return RetornarFalhaLinha(Localization.Resources.Pedidos.Pedido.RegistroIgnoradoImportacao, true);
                    }
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDestinatario = (from obj in linha.Colunas where obj.NomeCampo == "CNPJCPFDestinatario" select obj).FirstOrDefault();
                Dominio.Entidades.Cliente destinatario = null;
                if (colDestinatario != null)
                {
                    double cpfCNPJDestinatario = 0;
                    string somenteNumeros = Utilidades.String.OnlyNumbers((string)colDestinatario.Valor);
                    if (!string.IsNullOrEmpty(somenteNumeros) && (somenteNumeros.Length > 5 || configuracaoTMS.Pais == TipoPais.Exterior))
                    {
                        cpfCNPJDestinatario = double.Parse(somenteNumeros);
                        destinatario = repCliente.BuscarPorCPFCNPJ(cpfCNPJDestinatario, null, lstCpfCNPJClientes);

                        if (destinatario == null)
                        {
                            string mensagemValidacao = CriarParticipante(ref destinatario, (string)colDestinatario.Valor, linha, "Destinatario", AdminStringConexao, configuracaoTMS, unitOfWork);
                            if (destinatario == null)
                            {
                                destinatario = Servicos.Embarcador.Pessoa.Pessoa.CriarPessoa(string.Format(@"{0:00000000000000}", cpfCNPJDestinatario), "Destinatário", unitOfWork);
                                if (destinatario == null)
                                    return RetornarFalhaLinha(!string.IsNullOrWhiteSpace(mensagemValidacao) ? mensagemValidacao : Localization.Resources.Pedidos.Pedido.ODestinatarioInformadoNaoCadastradoBaseMultisoftware);
                            }
                        }
                    }
                    else
                        return RetornarFalhaLinha(Localization.Resources.Pedidos.Pedido.RegistroIgnoradoImportacao, true);
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colFronteira = (from obj in linha.Colunas where obj.NomeCampo == "Fronteira" select obj).FirstOrDefault();
                Dominio.Entidades.Cliente fronteira = null;
                List<Dominio.Entidades.Cliente> listaFronteiras = new List<Dominio.Entidades.Cliente>();
                if (colFronteira != null)
                {
                    double cpfCNPJFronteira = 0;
                    string somenteNumeros = Utilidades.String.OnlyNumbers((string)colFronteira.Valor);
                    if (!string.IsNullOrEmpty(somenteNumeros) && (somenteNumeros.Length > 5 || configuracaoTMS.Pais == TipoPais.Exterior))
                    {
                        cpfCNPJFronteira = double.Parse(somenteNumeros);
                        fronteira = repCliente.BuscarPorCPFCNPJ(cpfCNPJFronteira, null, lstCpfCNPJClientes);
                    }
                    if (fronteira == null)
                    {
                        fronteira = repCliente.BuscarPorCodigoIntegracao((string)colFronteira.Valor, lstClientesPorCodigoIntegracao);
                    }

                    if (fronteira != null)
                    {
                        if (!fronteira.FronteiraAlfandega)
                        {
                            return RetornarFalhaLinha(Localization.Resources.Pedidos.Pedido.OCNPJCodigoIntegracaoClienteInformadoFronteiraNaoMarcadoFronteira);
                        }
                    }
                    else
                        return RetornarFalhaLinha(Localization.Resources.Pedidos.Pedido.AFronteiraInformadaNaoCadastradoBaseMultisoftware);


                    listaFronteiras.Add(fronteira);
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCodigoRemetente = (from obj in linha.Colunas where obj.NomeCampo == "CodigoRemetente" select obj).FirstOrDefault();
                if (remetente == null && colCodigoRemetente != null)
                {
                    remetente = repCliente.BuscarPorCodigoIntegracao(colCodigoRemetente.Valor, lstClientesPorCodigoIntegracao);
                    if (remetente == null)
                    {
                        string retornoRemetente = CriarFornecedor(ref remetente, (string)colCodigoRemetente.Valor, unitOfWork);
                        if (remetente == null)
                        {
                            return RetornarFalhaLinha(!string.IsNullOrWhiteSpace(retornoRemetente) ? retornoRemetente : Localization.Resources.Pedidos.Pedido.ORemetenteInformadoNaoCadastradoBaseMultisoftware);
                        }
                    }
                }

                if (remetente == null && configuracaoTMS.RemetentePadraoImportacaoPlanilhaPedido != null)
                    remetente = repCliente.BuscarPorCPFCNPJ(configuracaoTMS.RemetentePadraoImportacaoPlanilhaPedido.CPF_CNPJ, null, lstCpfCNPJClientes);

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCodigoDestinatario = (from obj in linha.Colunas where obj.NomeCampo == "CodigoDestinatario" select obj).FirstOrDefault();
                if (colCodigoDestinatario != null)
                {
                    string codigoIntegracaoDestinatario = (string)colCodigoDestinatario.Valor;
                    if (destinatario == null)
                    {
                        destinatario = repCliente.BuscarPorCodigoIntegracao(codigoIntegracaoDestinatario, lstClientesPorCodigoIntegracao);
                        if (destinatario == null)
                            return RetornarFalhaLinha(Localization.Resources.Pedidos.Pedido.ODestinatarioInformadoNaoCadastradoBaseMultisoftware);
                    }
                    else if (!string.IsNullOrWhiteSpace(codigoIntegracaoDestinatario) && codigoIntegracaoDestinatario != destinatario.CodigoIntegracao)
                    {
                        destinatario.Initialize();
                        destinatario.CodigoIntegracao = codigoIntegracaoDestinatario;
                        repCliente.Atualizar(destinatario, auditado);
                    }
                }

                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoasRemetente = null;
                if (remetente == null)
                {
                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colGrupoPessoasRemetente = (from obj in linha.Colunas where obj.NomeCampo == "GrupoPessoasRemetente" select obj).FirstOrDefault();

                    string descricaoGrupoPessoas = (string)colGrupoPessoasRemetente?.Valor;

                    if (!string.IsNullOrWhiteSpace(descricaoGrupoPessoas))
                        grupoPessoasRemetente = repGrupoPessoas.BuscarPorDescricao(descricaoGrupoPessoas);

                    if (grupoPessoasRemetente == null)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCodigoGrupoPessoasRemetente = (from obj in linha.Colunas where obj.NomeCampo == "CodigoGrupoPessoasRemetente" select obj).FirstOrDefault();

                        string codigoGrupoPessoas = (string)colCodigoGrupoPessoasRemetente?.Valor;

                        if (!string.IsNullOrWhiteSpace(codigoGrupoPessoas))
                            grupoPessoasRemetente = repGrupoPessoas.BuscarPorCodigoIntegracao(codigoGrupoPessoas);
                    }
                }

                if (grupoPessoasRemetente != null && remetente != null && grupoPessoasRemetente.Codigo != remetente.GrupoPessoas?.Codigo)
                    return RetornarFalhaLinha(Localization.Resources.Pedidos.Pedido.OCNPJCodigoIntegracaoClienteInformadoFronteiraNaoMarcadoFronteira);

                Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultado = null;
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCentroResultado = (from obj in linha.Colunas where obj.NomeCampo == "CentroResultado" select obj).FirstOrDefault();
                if (colCentroResultado != null)
                {
                    string descricaoCentroResultado = colCentroResultado?.Valor;

                    if (!string.IsNullOrWhiteSpace(descricaoCentroResultado))
                        centroResultado = repCentroResultado.BuscarPorDescricao(descricaoCentroResultado);
                }

                if (coltransportador == null)
                {
                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCodigoTransportador = (from obj in linha.Colunas where obj.NomeCampo == "CodigoTransportador" select obj).FirstOrDefault();
                    if (colCodigoTransportador != null)
                    {
                        string codigoIntegracao = (string)colCodigoTransportador.Valor;
                        if (!string.IsNullOrWhiteSpace(codigoIntegracao))
                        {
                            empresa = repEmpresa.BuscarPorCodigoIntegracao(codigoIntegracao);
                            if (empresa == null || empresa.Status == "I")
                            {
                                return RetornarFalhaLinha(Localization.Resources.Pedidos.Pedido.AEmpresaInformadaNaoExisteBaseMultisoftware);
                            }
                        }
                    }
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colPlacaVeiculo = (from obj in linha.Colunas where obj.NomeCampo == "PlacaVeiculo" select obj).FirstOrDefault();
                Dominio.Entidades.Veiculo veiculo = null;
                if (colPlacaVeiculo != null)
                {
                    string placa = (string)colPlacaVeiculo.Valor;

                    if (!string.IsNullOrWhiteSpace(placa))
                    {
                        placa = placa.Replace("-", "").Trim();

                        if (placa.Length > 7)
                            placa = placa.Substring(0, 7);

                        if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS || tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                            veiculo = repVeiculo.BuscarPorPlaca(placa);
                        else if (empresa == null && !string.IsNullOrWhiteSpace(cnpjTransportador) && (configuracaoPedido?.BuscarEmpresaPeloProprietarioDoVeiculo ?? false))
                        {
                            double.TryParse(cnpjTransportador, out double cnpjTerceiro);
                            veiculo = repVeiculo.BuscarPorPlacaETerceiro(placa, cnpjTerceiro);
                            if (veiculo != null)
                                empresa = veiculo.Empresa;
                        }
                        else
                            veiculo = repVeiculo.BuscarPorPlacaVarrendoFiliais(empresa?.Codigo ?? 0, placa);
                        if (veiculo != null)
                        {
                            int codigoVeiculo = veiculo.Codigo;
                            List<int> codigosVeiculo = new List<int> { codigoVeiculo };
                            Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo licenca = repLicencaVeiculo.BuscarLicencaParaBloqueioPedido(codigosVeiculo);

                            if (licenca != null)
                            {
                                string mensagem = string.Empty;
                                if (licenca.Status == StatusLicenca.Vencido)
                                {
                                    mensagem = $"O Veículo {licenca.Veiculo.Placa_Formatada} possui a licença {licenca.Descricao} de número {licenca.Numero} com o status {licenca.Status.ObterDescricao() ?? ""}, favor verifique antes de prosseguir com o planejamento do pedido.";
                                }
                                else
                                {
                                    mensagem = $"O Veículo {licenca.Veiculo.Placa_Formatada} possui a licença {licenca.Descricao} de número {licenca.Numero} vencida na data {licenca.DataVencimento?.ToString("dd/MM/yyyy") ?? ""}, favor verifique antes de prosseguir com o planejamento do pedido.";
                                }

                                return RetornarFalhaLinha(mensagem, false);
                            }
                        }
                        if (veiculo == null)
                        {
                            return RetornarFalhaLinha(Localization.Resources.Pedidos.Pedido.OVeiculoNaoExisteBaseMultisoftware);
                        }
                        else
                        {
                            if (empresa == null && (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS || (configuracaoPedido?.BuscarEmpresaPeloProprietarioDoVeiculo ?? false)))
                                empresa = veiculo.Empresa;
                        }
                    }
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colPlacaReboque = (from obj in linha.Colunas where obj.NomeCampo == "PlacaReboque" select obj).FirstOrDefault();
                Dominio.Entidades.Veiculo veiculoReboque = null;
                if (colPlacaReboque != null && empresa != null)
                {
                    string placa = (string)colPlacaReboque.Valor;

                    if (!string.IsNullOrWhiteSpace(placa))
                    {
                        placa = placa.Replace("-", "").Trim();

                        if (placa.Length > 7)
                            placa = placa.Substring(0, 7);

                        if (!string.IsNullOrWhiteSpace(placa))
                        {
                            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                                veiculoReboque = repVeiculo.BuscarPorPlaca(placa);
                            else
                                veiculoReboque = repVeiculo.BuscarPorPlacaVarrendoFiliais(empresa.Codigo, placa);

                            if (veiculoReboque == null)
                            {
                                return RetornarFalhaLinha(Localization.Resources.Pedidos.Pedido.OVeiculoNaoExisteBaseMultisoftware);
                            }
                        }
                    }
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCPFMotorista = (from obj in linha.Colunas where obj.NomeCampo == "CPFMotorista" select obj).FirstOrDefault();
                Dominio.Entidades.Usuario motorista = null;
                if (colCPFMotorista != null && empresa != null)
                {
                    if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS || tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                        motorista = repMotorista.BuscarMotoristaPorCPF((string)colCPFMotorista.Valor);
                    else
                        motorista = repMotorista.BuscarMotoristaPorCPFVarrendoFiliais(empresa.Codigo, (string)colCPFMotorista.Valor);

                    string retornoMotorista = "";
                    if (motorista == null)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNomeMotorista = (from obj in linha.Colunas where obj.NomeCampo == "NomeMotorista" select obj).FirstOrDefault();
                        if (colNomeMotorista != null)
                        {
                            Dominio.ObjetosDeValor.Embarcador.Carga.Motorista motoristaIntegracao = new Dominio.ObjetosDeValor.Embarcador.Carga.Motorista();
                            motoristaIntegracao.CPF = (string)colCPFMotorista.Valor;
                            motoristaIntegracao.Nome = (string)colNomeMotorista.Valor;
                            motoristaIntegracao.Email = "";
                            motoristaIntegracao.NumeroHabilitacao = "";
                            motoristaIntegracao.RG = "";
                            motoristaIntegracao.CodigoIntegracao = "";
                            motorista = serMotorista.SalvarMotorista(motoristaIntegracao, empresa, ref retornoMotorista, unitOfWork, tipoServicoMultisoftware, auditado);
                        }
                    }

                    if (motorista == null)
                    {
                        return RetornarFalhaLinha(!string.IsNullOrWhiteSpace(retornoMotorista) ? retornoMotorista : Localization.Resources.Pedidos.Pedido.OMotoristaInformadoNaoExisteBaseMultisoftware);
                    }
                }

                if (motorista == null && veiculo != null)
                {
                    Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);
                    Dominio.Entidades.Usuario veiculoMotorista = repVeiculoMotorista.BuscarMotoristaPrincipal(veiculo.Codigo);

                    motorista = veiculoMotorista;
                }


                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNotasParciais = (from obj in linha.Colunas where obj.NomeCampo == "NotasParciais" select obj).FirstOrDefault();
                List<int> notasParciais = new List<int>();
                if (colNotasParciais != null)
                {
                    if (!string.IsNullOrWhiteSpace((string)colNotasParciais.Valor))
                    {
                        string strNotasParciais = (string)colNotasParciais.Valor;
                        string[] splitNotaParcial = strNotasParciais.Split(',');
                        foreach (string strNfParcial in splitNotaParcial)
                        {
                            int numeroNF = 0;
                            int.TryParse(strNfParcial, out numeroNF);
                            if (numeroNF > 0) notasParciais.Add(numeroNF);
                        }
                    }
                    else
                        return RetornarFalhaLinha(Localization.Resources.Pedidos.Pedido.SeInformarColunaNumeroNotasObrigatorioInformarNumeroNota);
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaCtesParciais = (from obj in linha.Colunas where obj.NomeCampo == "CtesParciais" select obj).FirstOrDefault();
                List<int> ctesParciais = new List<int>();
                if (colunaCtesParciais != null)
                {
                    string[] numerosCteParcial = ((string)colunaCtesParciais.Valor).Split(',');
                    foreach (string numeroCteParcial in numerosCteParcial)
                    {
                        int numeroCte = numeroCteParcial.ToInt();
                        if (numeroCte > 0) ctesParciais.Add(numeroCte);
                    }
                }

                Dominio.Entidades.RotaFrete rotaFrete = null;
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colRota = (from obj in linha.Colunas where obj.NomeCampo == "Rota" select obj).FirstOrDefault();
                if (colRota != null)
                {
                    rotaFrete = repRotaFrete.BuscarPorCodigoIntegracao((string)colRota.Valor);
                    if (rotaFrete == null)
                    {
                        return RetornarFalhaLinha(Localization.Resources.Pedidos.Pedido.ARotaInformadaNaoExisteBaseMultisoftware);
                    }
                }

                int ordemColeta = 0;
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colOrdemColeta = (from obj in linha.Colunas where obj.NomeCampo == "OrdemColeta" select obj).FirstOrDefault();
                if (colOrdemColeta != null)
                    int.TryParse((string)colOrdemColeta.Valor, out ordemColeta);


                string adicional1 = "";
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colAdicional1 = (from obj in linha.Colunas where obj.NomeCampo == "Adicional1" select obj).FirstOrDefault();
                if (colAdicional1 != null)
                    adicional1 = colAdicional1.Valor;

                string adicional2 = "";
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colAdicional2 = (from obj in linha.Colunas where obj.NomeCampo == "Adicional2" select obj).FirstOrDefault();
                if (colAdicional2 != null)
                    adicional2 = colAdicional2.Valor;

                string adicional3 = "";
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colAdicional3 = (from obj in linha.Colunas where obj.NomeCampo == "Adicional3" select obj).FirstOrDefault();
                if (colAdicional3 != null)
                    adicional3 = colAdicional3.Valor;

                string adicional4 = "";
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colAdicional4 = (from obj in linha.Colunas where obj.NomeCampo == "Adicional4" select obj).FirstOrDefault();
                if (colAdicional4 != null)
                    adicional4 = colAdicional4.Valor;

                string adicional5 = "";
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colAdicional5 = (from obj in linha.Colunas where obj.NomeCampo == "Adicional5" select obj).FirstOrDefault();
                if (colAdicional5 != null)
                    adicional5 = colAdicional5.Valor;

                string adicional6 = "";
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colAdicional6 = (from obj in linha.Colunas where obj.NomeCampo == "Adicional6" select obj).FirstOrDefault();
                if (colAdicional6 != null)
                    adicional6 = colAdicional6.Valor;

                string adicional7 = "";
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colAdicional7 = (from obj in linha.Colunas where obj.NomeCampo == "Adicional7" select obj).FirstOrDefault();
                if (colAdicional7 != null)
                    adicional7 = colAdicional7.Valor;



                bool ajudante = false;
                int quantidadeAjudantes = 0;

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colAjudante = (from obj in linha.Colunas where obj.NomeCampo == "Ajudante" select obj).FirstOrDefault();
                if (colAjudante != null)
                {
                    ajudante = ((string)colAjudante.Valor).ToLower() == "sim";

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colQuantidadeAjudantes = (from obj in linha.Colunas where obj.NomeCampo == "QuantidadeAjudantes" select obj).FirstOrDefault();
                    if (colQuantidadeAjudantes != null && ajudante)
                    {
                        quantidadeAjudantes = Convert.ToInt32(colQuantidadeAjudantes.Valor);
                    }
                }



                string observacaoAdicional = "";
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colObservacaoAdicional = (from obj in linha.Colunas where obj.NomeCampo == "ObservacaoAdicional" select obj).FirstOrDefault();
                if (colObservacaoAdicional != null)
                    observacaoAdicional = colObservacaoAdicional.Valor;

                string observacaoInterna = "";
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colObservacaoInterna = (from obj in linha.Colunas where obj.NomeCampo == "ObservacaoInterna" select obj).FirstOrDefault();
                if (colObservacaoInterna != null)
                    observacaoInterna = colObservacaoInterna.Valor;

                string canal = "";
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCanal = (from obj in linha.Colunas where obj.NomeCampo == "Canal" select obj).FirstOrDefault();
                if (colCanal != null)
                    canal = colCanal.Valor;

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCidadeOrigem = (from obj in linha.Colunas where obj.NomeCampo == "CidadeOrigem" select obj).FirstOrDefault();
                string origemString = "";
                Dominio.Entidades.Localidade origem = null;
                if (colCidadeOrigem != null)
                {
                    origemString = Utilidades.String.SanitizeString(colCidadeOrigem.Valor);

                    string estadoString = "";

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colUFOrigem = (from obj in linha.Colunas where obj.NomeCampo == "UFOrigem" select obj).FirstOrDefault();
                    if (colUFOrigem != null)
                        estadoString = colUFOrigem.Valor;

                    origem = repositorioLocalidade.BuscarPorUFDescricao(estadoString, origemString).FirstOrDefault();

                    if (origem == null)
                        return RetornarFalhaLinha("A cidade de origem não foi encontrada na base da Multisoftware");
                }

                DateTime dataValidade = DateTime.MinValue;
                string dataValidadeCol;
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDataValidade = (from obj in linha.Colunas where obj.NomeCampo == "DataValidade" select obj).FirstOrDefault();
                if (colDataValidade != null)
                {
                    dataValidadeCol = colDataValidade.Valor;
                    double.TryParse(dataValidadeCol, out double dataValidadeFormatoExcel);

                    if (dataValidadeFormatoExcel > 0)
                        dataValidade = DateTime.FromOADate(dataValidadeFormatoExcel);
                    else
                        dataValidade = dataValidadeCol.ToDateTime();
                }

                DateTime dataInicioJanelaDescarga = DateTime.MinValue;
                string dataInicioJanelaDescargaCol;
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDataInicioJanelaDescarga = (from obj in linha.Colunas where obj.NomeCampo == "DataInicioJanelaDescarga" select obj).FirstOrDefault();
                if (colDataInicioJanelaDescarga != null)
                {
                    dataInicioJanelaDescargaCol = colDataInicioJanelaDescarga.Valor;
                    double.TryParse(dataInicioJanelaDescargaCol, out double dataInicioJanelaDescargaFormatoExcel);

                    if (dataInicioJanelaDescargaFormatoExcel > 0)
                        dataInicioJanelaDescarga = DateTime.FromOADate(dataInicioJanelaDescargaFormatoExcel);
                    else
                        dataInicioJanelaDescarga = dataInicioJanelaDescargaCol.ToDateTime();

                }

                DateTime dataHoraDescarga = DateTime.Now;
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDataDescarga = (from obj in linha.Colunas where obj.NomeCampo == "DataDescarga" select obj).FirstOrDefault();
                string dataDescarga = "";
                if (colDataDescarga != null)
                {
                    dataDescarga = colDataDescarga.Valor;
                    double.TryParse(dataDescarga, out double dataFormatoExcel);
                    if (dataFormatoExcel > 0)
                        dataHoraDescarga = Utilidades.DateTime.ConverterDataExcelToDateTime(dataFormatoExcel);
                    else if (!string.IsNullOrWhiteSpace(dataDescarga))
                        DateTime.TryParse(dataDescarga, out dataHoraDescarga);
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNumeroAgrupamentoPrecarga = null;
                bool cargaDePreCarga = false;
                string numeroPreCarga = PossuiNumeroPreCarga(linha, prefixoPreCarga, configuracaoTMS, ref colNumeroAgrupamentoPrecarga, ref cargaDePreCarga);


                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colQuantidadePallets = (from obj in linha.Colunas where obj.NomeCampo == "QuantidadePallets" select obj).FirstOrDefault();
                decimal quantidadePalletsFacionada = 0;
                if (colQuantidadePallets != null)
                {
                    quantidadePalletsFacionada = Utilidades.Decimal.Converter((string)colQuantidadePallets.Valor);
                }

                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = null;
                Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga = null;
                Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga = null;

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCompostoTipoCargaOperacaoVeiculo = (from obj in linha.Colunas where obj.NomeCampo == "CompostoTipoCargaOperacaoVeiculo" select obj).FirstOrDefault();

                if (colCompostoTipoCargaOperacaoVeiculo != null)
                {
                    string strComposto = colCompostoTipoCargaOperacaoVeiculo.Valor;
                    string[] splitComposto = strComposto.Split(' ');

                    if (splitComposto.Length > 2)
                    {
                        string codigotipoOperacao = splitComposto[0].Trim();
                        tipoOperacao = repTipoOperacao.BuscarPorCodigoIntegracao(codigotipoOperacao);
                        if (tipoOperacao == null) return RetornarFalhaLinha(Localization.Resources.Pedidos.Pedido.OTipoOperacaoInformadoNaoExisteBaseMultisoftware);

                        string codigoModeloVeiculo = splitComposto[1].Trim();
                        modeloVeicularCarga = repModeloVeicularCarga.buscarPorCodigoIntegracao(codigoModeloVeiculo);
                        if (modeloVeicularCarga == null) return RetornarFalhaLinha(Localization.Resources.Pedidos.Pedido.OModeloVeicularInformadoNaoExisteBaseMultisoftware);

                        string codigoTipoCarga = splitComposto[2].Trim();
                        tipoCarga = repTipoDeCarga.BuscarPorCodigoEmbarcador(codigoTipoCarga);
                        if (tipoCarga == null) return RetornarFalhaLinha(Localization.Resources.Pedidos.Pedido.OTipoCargaInformadoNaoExisteBaseMultisoftware);
                    }
                    else if (splitComposto.Length == 2)
                    {
                        string codigotipoOperacao = splitComposto[0].Trim();
                        tipoOperacao = repTipoOperacao.BuscarPorCodigoIntegracao(codigotipoOperacao);
                        if (tipoOperacao == null) return RetornarFalhaLinha(Localization.Resources.Pedidos.Pedido.OTipoOperacaoInformadoNaoExisteBaseMultisoftware);

                        string codigoModeloVeiculo = splitComposto[1].Trim();
                        modeloVeicularCarga = repModeloVeicularCarga.buscarPorCodigoIntegracao(codigoModeloVeiculo);
                        if (modeloVeicularCarga == null) return RetornarFalhaLinha(Localization.Resources.Pedidos.Pedido.OModeloVeicularInformadoNaoExisteBaseMultisoftware);

                        string codigoTipoCarga = string.Empty;
                        tipoCarga = repTipoDeCarga.BuscarPorCodigoEmbarcador(codigoTipoCarga);
                        if (tipoCarga == null) return RetornarFalhaLinha(Localization.Resources.Pedidos.Pedido.OTipoCargaInformadoNaoExisteBaseMultisoftware);
                    }
                    else
                    {
                        return RetornarFalhaLinha(Localization.Resources.Pedidos.Pedido.OCompostoTipoOperacaoModeloVeiculoTipoCargaNaoPadraoEsperado);
                    }
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTipoCarga = (from obj in linha.Colunas where obj.NomeCampo == "TipoCarga" select obj).FirstOrDefault();

                if (colTipoCarga != null)
                {
                    tipoCarga = repTipoDeCarga.BuscarPorCodigoEmbarcador((string)colTipoCarga.Valor, lstTipoCarga);

                    if (tipoCarga == null && (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS || tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador))
                        tipoCarga = repTipoDeCarga.BuscarPorDescricao((string)colTipoCarga.Valor, true, lstTipoCarga);

                    if (tipoCarga == null) return RetornarFalhaLinha(Localization.Resources.Pedidos.Pedido.OTipoCargaInformadoNaoExisteBaseMultisoftware);
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colModeloVeicularCarga = (from obj in linha.Colunas where obj.NomeCampo == "ModeloVeicularCarga" select obj).FirstOrDefault();

                if (colModeloVeicularCarga != null)
                {
                    modeloVeicularCarga = repModeloVeicularCarga.buscarPorCodigoIntegracao((string)colModeloVeicularCarga.Valor, lstModeloVeicularCarga);

                    if (modeloVeicularCarga == null && tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                        modeloVeicularCarga = repModeloVeicularCarga.buscarPorDescricao((string)colModeloVeicularCarga.Valor, lstModeloVeicularCarga);

                    if (modeloVeicularCarga == null) return RetornarFalhaLinha(Localization.Resources.Pedidos.Pedido.OModeloVeicularInformadoNaoExisteBaseMultisoftware);
                }

                if (modeloVeicularCarga == null)
                    modeloVeicularCarga = configuracaoTMS.ModeloVeicularCargaPadraoImportacaoPedido;

                if (modeloVeicularCarga == null)
                    modeloVeicularCarga = veiculo?.ModeloVeicularCarga;

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNumeroPedido = (from obj in linha.Colunas where obj.NomeCampo == "NumeroPedido" select obj).FirstOrDefault();
                string numeroPedido = "";
                if (colNumeroPedido != null)
                {
                    numeroPedido = obterNumeroPedido(colNumeroPedido, numeroPedido, configuracaoPedido, numeroPreCarga);
                    if ((configuracaoPedido?.NaoPermitirImportarPedidosExistentes ?? false) && repositorioPedido.VerificarExistenciaPedido(numeroPedido))
                        return RetornarFalhaLinha(Localization.Resources.Pedidos.Pedido.OPedidoExisteSistema);
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNumeroPedidoCliente = (from obj in linha.Colunas where obj.NomeCampo == "NumeroPedidoCliente" select obj).FirstOrDefault();
                string numeroPedidoCliente = colNumeroPedidoCliente?.Valor;

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTipoOperacao = (from obj in linha.Colunas where obj.NomeCampo == "TipoOperacao" select obj).FirstOrDefault();
                if (colTipoOperacao != null)
                {
                    tipoOperacao = repTipoOperacao.BuscarPorCodigoIntegracao((string)colTipoOperacao.Valor, lstTipoOperacao);

                    if (tipoOperacao == null && tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                        tipoOperacao = repTipoOperacao.BuscarPorDescricao((string)colTipoOperacao.Valor, lstTipoOperacao);

                    if (tipoOperacao == null) return RetornarFalhaLinha(Localization.Resources.Pedidos.Pedido.OTipoOperacaoInformadoNaoExisteBaseMultisoftware);
                }
                else
                {
                    if (tipoOperacao == null)
                    {
                        tipoOperacao = repTipoOperacao.BuscarTipoOperacaoPadraoQuandoNaoInformadaNaIntegracao();
                        if (tipoOperacao != null && tipoCarga == null)
                            tipoCarga = tipoOperacao.TipoDeCargaPadraoOperacao;
                    }
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colExpedidor = (from obj in linha.Colunas where obj.NomeCampo == "CNPJCPFExpedidor" select obj).FirstOrDefault();
                Dominio.Entidades.Cliente expedidor = null;
                if (colExpedidor != null && colExpedidor.Valor != null)
                {
                    double cpfCNPJExpedidor = Utilidades.String.OnlyNumbers((string)colExpedidor.Valor).ToDouble();
                    if (cpfCNPJExpedidor > 0d)
                    {
                        expedidor = repCliente.BuscarPorCPFCNPJ(cpfCNPJExpedidor, null, lstCpfCNPJClientes);
                        if (expedidor == null) return RetornarFalhaLinha(Localization.Resources.Pedidos.Pedido.OExpedidorInformadaNaoCadastradoBaseMultisoftware);
                    }
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCodigoExpedidor = (from obj in linha.Colunas where obj.NomeCampo == "CodigoExpedidor" select obj).FirstOrDefault();
                if (expedidor == null && colCodigoExpedidor != null && colCodigoExpedidor.Valor != null)
                {
                    expedidor = repCliente.BuscarPorCodigoIntegracao(colCodigoExpedidor.Valor);
                    if (expedidor == null)
                        return RetornarFalhaLinha(Localization.Resources.Pedidos.Pedido.OExpedidorInformadaNaoCadastradoBaseMultisoftware);
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colRecebedor = (from obj in linha.Colunas where obj.NomeCampo == "CNPJCPFRecebedor" select obj).FirstOrDefault();
                Dominio.Entidades.Cliente recebedor = null;
                if (colRecebedor != null && colRecebedor.Valor != null)
                {
                    double cpfCNPJRecebedor = Utilidades.String.OnlyNumbers((string)colRecebedor.Valor).ToDouble();
                    if (cpfCNPJRecebedor > 0d)
                    {
                        recebedor = repCliente.BuscarPorCPFCNPJ(cpfCNPJRecebedor, null, lstCpfCNPJClientes);
                        if (recebedor == null) return RetornarFalhaLinha(Localization.Resources.Pedidos.Pedido.ORecebedorInformadoNaoCadastradoBaseMultisoftware);
                    }
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCodigoRecebedor = (from obj in linha.Colunas where obj.NomeCampo == "CodigoRecebedor" select obj).FirstOrDefault();
                if (recebedor == null && colCodigoRecebedor != null)
                {
                    recebedor = repCliente.BuscarPorCodigoIntegracao(colCodigoRecebedor.Valor);
                    if (recebedor == null)
                        return RetornarFalhaLinha(Localization.Resources.Pedidos.Pedido.ORecebedorInformadoNaoCadastradoBaseMultisoftware);
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTomador = (from obj in linha.Colunas where obj.NomeCampo == "CNPJCPFTomador" select obj).FirstOrDefault();
                Dominio.Entidades.Cliente tomador = null;
                if (colTomador != null && colTomador.Valor != null)
                {
                    double cpfCNPJTomador = Utilidades.String.OnlyNumbers((string)colTomador.Valor).ToDouble();
                    if (cpfCNPJTomador > 0d)
                    {
                        tomador = repCliente.BuscarPorCPFCNPJ(cpfCNPJTomador, null, lstCpfCNPJClientes);
                        if (tomador == null) return RetornarFalhaLinha(Localization.Resources.Pedidos.Pedido.OTomadorInformadoNaoCadastradoBaseMultisoftware);
                    }
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaLocalExpedicao = (from obj in linha.Colunas where obj.NomeCampo == "CNPJCPFLocalExpedicao" select obj).FirstOrDefault();
                Dominio.Entidades.Cliente localExpedicao = null;
                if (colunaLocalExpedicao != null)
                {
                    double cpfCnpjLocalExpedicao = ((string)colunaLocalExpedicao.Valor).ObterSomenteNumeros().ToDouble();

                    if (cpfCnpjLocalExpedicao > 0d)
                    {
                        localExpedicao = repCliente.BuscarPorCPFCNPJ(cpfCnpjLocalExpedicao, null, lstCpfCNPJClientes);

                        if (localExpedicao == null)
                            return RetornarFalhaLinha(Localization.Resources.Pedidos.Pedido.OLocalExpedicaoInformadoNaoCadastradoBaseMultisoftware);
                    }
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colSituacaoPedido = (from obj in linha.Colunas where obj.NomeCampo == "SituacaoPedido" select obj).FirstOrDefault();
                bool cancelarPedido = false;
                bool ativarPedido = false;
                if (colSituacaoPedido != null)
                {
                    string _situacao = (string)colSituacaoPedido.Valor.Trim();
                    cancelarPedido = _situacao.ToUpper() == "CANCELADA";
                    ativarPedido = _situacao.ToUpper() == "ATIVA";
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colPedidosShopee = (from obj in linha.Colunas where obj.NomeCampo == "PedidosShopee" select obj).FirstOrDefault();

                if (colPedidosShopee != null)
                {
                    string pedidoShopee = (string)colPedidosShopee.Valor;
                    DateTime dataLiberacaoCD = DateTime.MinValue;

                    if (!string.IsNullOrWhiteSpace(pedidoShopee))
                    {
                        List<string> pedidoShopeeSplitado = new List<string>(pedidoShopee.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));

                        if (!DateTime.TryParseExact(pedidoShopeeSplitado[0], "yyyy-MM-dd HH:mm:ss", cultura, DateTimeStyles.None, out dataLiberacaoCD))
                        {
                            double.TryParse(pedidoShopeeSplitado[0], out double dataFormatoExcel);

                            if (dataFormatoExcel > 0)
                                dataLiberacaoCD = Utilidades.DateTime.ConverterDataExcelToDateTime(dataFormatoExcel);
                            else if (!string.IsNullOrWhiteSpace(pedidoShopeeSplitado[0]))
                                DateTime.TryParse(pedidoShopeeSplitado[0], out dataLiberacaoCD);
                        }

                        numeroPedido = pedidoShopeeSplitado[5];
                        remetente = repCliente.BuscarPorCodigoAlternativo(pedidoShopeeSplitado[2]);
                        destinatario = repCliente.BuscarPorCodigoAlternativo(pedidoShopeeSplitado[3]);
                        codigoCargaEmbarcadorShopee = pedidoShopeeSplitado[5];

                    }
                }


                Dominio.Entidades.Embarcador.Cargas.Carga cargaPreCarga = null;
                Dominio.Entidades.Embarcador.Filiais.Filial filial = null;

                if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colFilial = (from obj in linha.Colunas where obj.NomeCampo == "Filial" select obj).FirstOrDefault();

                    if (colFilial != null)
                    {
                        string codigoIntegracaoFilial = (string)colFilial.Valor;
                        filial = repFilial.buscarPorCodigoEmbarcador(codigoIntegracaoFilial, lstFilial);
                        if (filial == null) return RetornarFalhaLinha(Localization.Resources.Pedidos.Pedido.AFilialCadastradaNaoExisteBaseMultisoftware);

                        if (remetente == null && colRemetente == null && colCodigoRemetente == null)
                        {
                            remetente = repCliente.BuscarPorCPFCNPJ(double.Parse(Utilidades.String.OnlyNumbers(filial.CNPJ)), null, lstCpfCNPJClientes);
                            if (remetente == null) return RetornarFalhaLinha(Localization.Resources.Pedidos.Pedido.ORemetenteInformadoNaoCadastradoBaseMultisoftware);
                        }

                        if (destinatario == null && colDestinatario == null && colCodigoDestinatario == null)
                        {
                            destinatario = repCliente.BuscarPorCPFCNPJ(double.Parse(Utilidades.String.OnlyNumbers(filial.CNPJ)), null, lstCpfCNPJClientes);
                            if (destinatario == null) return RetornarFalhaLinha(Localization.Resources.Pedidos.Pedido.ODestinatarioInformadoNaoCadastradoBaseMultisoftware);
                        }
                    }
                    else if (remetente != null)
                        filial = repFilial.buscarPorCodigoEmbarcador(remetente.CPF_CNPJ_SemFormato);

                    if (filial == null) return RetornarFalhaLinha(Localization.Resources.Pedidos.Pedido.EobrigatorioInformarFilialParaImportacaoPedido);

                    if (configuracaoPedido.FilialPadraoImportacaoPedido != null && remetente != null && destinatario != null && remetente.CPF_CNPJ == destinatario.CPF_CNPJ)
                    {
                        filial = repFilial.BuscarPorCodigo(configuracaoPedido.FilialPadraoImportacaoPedido.Codigo);
                        remetente = repCliente.BuscarPorCPFCNPJ(double.Parse(Utilidades.String.OnlyNumbers(filial.CNPJ)), null, lstCpfCNPJClientes);
                        if (remetente == null) return RetornarFalhaLinha(Localization.Resources.Pedidos.Pedido.OClienteRemetenteMesmoCNPJFilialNaoCadastradoBaseMultisoftware);
                    }

                    if (tipoCarga == null)
                        tipoCarga = filial?.TipoDeCarga;

                    //busca carga relacionada
                    if (colNumeroAgrupamentoPrecarga == null)
                    {
                        if (configuracaoTMS.UsarMesmoNumeroPreCargaGerarCargaViaImportacao && !string.IsNullOrWhiteSpace(numeroPreCarga))
                        {
                            cargaPreCarga = repCarga.BuscarPreCargaPorNumeroCargaVincularPreCarga(numeroPreCarga, lstCargas);

                            Dominio.Entidades.Embarcador.Cargas.Carga preCargaVinculadaCarga = repCarga.BuscarPreCargaVinculadaCargaPedidoImportacao(numeroPreCarga, lstCargas);
                            if (preCargaVinculadaCarga != null)
                            {
                                if (preCargaVinculadaCarga?.Empresa?.CNPJ != cnpjTransportador)
                                    return RetornarFalhaLinha("Transportador da Carga difere do informado na pré carga");
                            }

                        }
                        else if (!string.IsNullOrWhiteSpace(numeroPedido))
                            cargaPreCarga = repositorioPedido.BuscarCargaDePreCargaPorPedido(numeroPedido, filial?.CodigoFilialEmbarcador ?? "");
                    }
                }

                DateTime dataPrevEntrega = DateTime.MinValue;
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDataPrevEntrega = (from obj in linha.Colunas where obj.NomeCampo == "DataPrevEntrega" select obj).FirstOrDefault();
                string strDataPrevEntrega = "";
                if (colDataPrevEntrega != null)
                {
                    strDataPrevEntrega = colDataPrevEntrega.Valor;
                    double dataFormatoExcel = (double)Utilidades.Decimal.Converter(strDataPrevEntrega);
                    if (dataFormatoExcel > 0)
                        dataPrevEntrega = Utilidades.DateTime.ConverterDataExcelToDateTime(dataFormatoExcel, strDataPrevEntrega);
                    else if (!string.IsNullOrWhiteSpace(strDataPrevEntrega))
                        DateTime.TryParse(strDataPrevEntrega, out dataPrevEntrega);

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colHoraPrevEntrega = (from obj in linha.Colunas where obj.NomeCampo == "HoraPrevEntrega" select obj).FirstOrDefault();
                    string strPrevEntrega = "";
                    if (colHoraPrevEntrega != null)
                    {
                        DateTime horaHoraPrevEntrega;
                        strPrevEntrega = colHoraPrevEntrega.Valor;
                        double horaFormatoExcel;
                        double.TryParse(strPrevEntrega, System.Globalization.NumberStyles.Any, cultura, out horaFormatoExcel);
                        if (horaFormatoExcel > 0)
                        {
                            horaHoraPrevEntrega = DateTime.FromOADate(horaFormatoExcel);
                            dataPrevEntrega = new DateTime(dataPrevEntrega.Year, dataPrevEntrega.Month, dataPrevEntrega.Day, horaHoraPrevEntrega.Hour, horaHoraPrevEntrega.Minute, horaHoraPrevEntrega.Second);
                        }
                        else if (!string.IsNullOrWhiteSpace(strPrevEntrega) && (DateTime.TryParse(strPrevEntrega, out horaHoraPrevEntrega)))
                        {
                            dataPrevEntrega = new DateTime(dataPrevEntrega.Year, dataPrevEntrega.Month, dataPrevEntrega.Day, horaHoraPrevEntrega.Hour, horaHoraPrevEntrega.Minute, horaHoraPrevEntrega.Second);
                        }
                    }

                }

                DateTime dataHoraCarregamento = DateTime.Now;
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDataCarregamento = (from obj in linha.Colunas where obj.NomeCampo == "DataCarregamento" select obj).FirstOrDefault();
                string dataCarregamento = "";
                if (colDataCarregamento != null)
                {
                    dataCarregamento = colDataCarregamento.Valor;

                    if (configuracaoPedido?.FormatoDataCarregamento.HasValue ?? false)
                        dataHoraCarregamento = DateTime.ParseExact(dataCarregamento, configuracaoPedido.FormatoDataCarregamento.Value.ObterDescricao(), cultura);
                    else
                    {
                        double.TryParse(dataCarregamento, out double dataFormatoExcel);

                        if (dataFormatoExcel > 0)
                            dataHoraCarregamento = Utilidades.DateTime.ConverterDataExcelToDateTime(dataFormatoExcel);
                        else if (!string.IsNullOrWhiteSpace(dataCarregamento))
                            DateTime.TryParse(dataCarregamento, out dataHoraCarregamento);
                    }
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colHoraCarregamento = (from obj in linha.Colunas where obj.NomeCampo == "HoraCarregamento" select obj).FirstOrDefault();
                string strHoraCarregamento = "";
                if (colHoraCarregamento != null)
                {
                    DateTime horaCarregamento;
                    strHoraCarregamento = colHoraCarregamento.Valor;

                    if (configuracaoPedido?.FormatoHoraCarregamento.HasValue ?? false)
                    {
                        if (strHoraCarregamento.Length == 3)
                            strHoraCarregamento = "0" + strHoraCarregamento;

                        while (strHoraCarregamento.Length < configuracaoPedido.FormatoHoraCarregamento.Value.ObterDescricao().Length)
                            strHoraCarregamento = strHoraCarregamento += "0";

                        horaCarregamento = DateTime.ParseExact(strHoraCarregamento, configuracaoPedido.FormatoHoraCarregamento.Value.ObterDescricao(), cultura);
                        dataHoraCarregamento = dataHoraCarregamento.Date.Add(horaCarregamento.TimeOfDay);
                    }
                    else
                    {
                        double.TryParse(strHoraCarregamento, System.Globalization.NumberStyles.Any, cultura, out double horaFormatoExcel);
                        if (horaFormatoExcel > 0)
                        {
                            horaCarregamento = DateTime.FromOADate(horaFormatoExcel);
                            dataHoraCarregamento = new DateTime(dataHoraCarregamento.Year, dataHoraCarregamento.Month, dataHoraCarregamento.Day, horaCarregamento.Hour, horaCarregamento.Minute, horaCarregamento.Second);
                        }
                        else if (!string.IsNullOrWhiteSpace(strHoraCarregamento) && (DateTime.TryParse(strHoraCarregamento, out horaCarregamento)))
                        {
                            dataHoraCarregamento = new DateTime(dataHoraCarregamento.Year, dataHoraCarregamento.Month, dataHoraCarregamento.Day, horaCarregamento.Hour, horaCarregamento.Minute, horaCarregamento.Second);
                        }
                    }
                    horarioCarregamentoInformadoNoPedido = true;
                }

                Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = null;
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCodCentroCarregamento = (from obj in linha.Colunas where obj.NomeCampo == "CodIntegracaoCentroCarregamento" select obj).FirstOrDefault();
                if (colCodCentroCarregamento != null)
                {
                    centroCarregamento = repCentroCarregamento.BuscarPorCodigoIntegracao((string)colCodCentroCarregamento.Valor);
                    if (centroCarregamento == null)
                    {
                        return RetornarFalhaLinha(Localization.Resources.Pedidos.Pedido.OCentroCarregInformadaNaoExisteBaseMultisoftware);
                    }
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDescCentroCarregamento = (from obj in linha.Colunas where obj.NomeCampo == "DescCentroCarregamento" select obj).FirstOrDefault();
                if (colDescCentroCarregamento != null)
                {
                    centroCarregamento = repCentroCarregamento.BuscarPorDescricao((string)colDescCentroCarregamento.Valor);
                    if (centroCarregamento == null)
                    {
                        return RetornarFalhaLinha(Localization.Resources.Pedidos.Pedido.OCentroCarregInformadaNaoExisteBaseMultisoftware);
                    }
                }

                Dominio.Entidades.Embarcador.Logistica.CentroCustoViagem centroCustoDeViagem = null;
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCodCentroCustoDeViagem = (from obj in linha.Colunas where obj.NomeCampo == "CodIntegracaoCentroCustoDeViagem" select obj).FirstOrDefault();
                if (colCodCentroCustoDeViagem != null)
                {
                    centroCustoDeViagem = repCentroDeCustoViagem.BuscarPorCodigoIntegracao((string)colCodCentroCustoDeViagem.Valor);
                    if (centroCustoDeViagem == null)
                    {
                        return RetornarFalhaLinha("O Centro de Custo de Viagem informado não existe na Base Multisoftware!");
                    }
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDescCentroCustoDeViagem = (from obj in linha.Colunas where obj.NomeCampo == "DescCentroCustoDeViagem" select obj).FirstOrDefault();
                if (colDescCentroCustoDeViagem != null)
                {
                    centroCustoDeViagem = repCentroDeCustoViagem.BuscarPorDescricao((string)colDescCentroCustoDeViagem.Valor);
                    if (centroCustoDeViagem == null)
                    {
                        return RetornarFalhaLinha("O Centro de Custo de Viagem informado não existe na Base Multisoftware!");
                    }
                }

                DateTime? dataHoraPrevisaoInicioViagem = null;
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colHoraPrevisaoInicioViagem = (from obj in linha.Colunas where obj.NomeCampo == "HoraPrevisaoInicioViagem" select obj).FirstOrDefault();
                string strHoraInicioViagem = "";
                if (colHoraPrevisaoInicioViagem != null)
                {
                    DateTime horaInicioViagem;
                    strHoraInicioViagem = colHoraPrevisaoInicioViagem.Valor;
                    double.TryParse(strHoraInicioViagem, out double horaFormatoExcel);
                    if (horaFormatoExcel > 0)
                    {
                        horaInicioViagem = DateTime.FromOADate(horaFormatoExcel);
                        dataHoraPrevisaoInicioViagem = new DateTime(dataHoraCarregamento.Year, dataHoraCarregamento.Month, dataHoraCarregamento.Day, horaInicioViagem.Hour, horaInicioViagem.Minute, horaInicioViagem.Second);
                    }
                    else if (!string.IsNullOrWhiteSpace(strHoraInicioViagem) && (DateTime.TryParse(strHoraInicioViagem, out horaInicioViagem)))
                    {
                        dataHoraPrevisaoInicioViagem = new DateTime(dataHoraCarregamento.Year, dataHoraCarregamento.Month, dataHoraCarregamento.Day, horaInicioViagem.Hour, horaInicioViagem.Minute, horaInicioViagem.Second);
                    }
                }

                Dominio.Entidades.Embarcador.Pedidos.MotivoImportacaoPedidoAtrasada motivo = null;
                if (motivosAtraso != null && !string.IsNullOrWhiteSpace(numeroPreCarga))
                {
                    Dominio.Entidades.Embarcador.Cargas.Carga cargaPreCargaMotivoAtraso = repCarga.BuscarPorCodigoVinculado(numeroPreCarga);
                    DateTime? dataHoraPrevisaoMotivoAtraso = cargaPreCargaMotivoAtraso?.DataInicioViagemPrevista;

                    if (dataHoraPrevisaoMotivoAtraso < DateTime.Now)
                    {
                        IEnumerable<(string Carga, Dominio.Entidades.Embarcador.Pedidos.MotivoImportacaoPedidoAtrasada Motivo)> enumerableMotivoAtraso = motivosAtraso.Where(o => o.Carga == numeroPreCarga);

                        if (!enumerableMotivoAtraso.Any())
                            return RetornarFalhaLinha(Localization.Resources.Pedidos.Pedido.NaoInformadoMotivoAtrasoPreCargNumeroPreCarga + numeroPreCarga);

                        motivo = enumerableMotivoAtraso.FirstOrDefault().Motivo;
                    }
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCubagemPedido = (from obj in linha.Colunas where obj.NomeCampo == "CubagemPedido" select obj).FirstOrDefault();
                decimal cubagemPedido = 0;
                if (colCubagemPedido != null)
                {
                    if (colCubagemPedido.Valor != null)
                        cubagemPedido = Utilidades.Decimal.Converter(((string)colCubagemPedido.Valor).Replace("-", ""));

                    decimal valorMaximoPermitido = 999_999_999_999.999999m;

                    if (cubagemPedido > valorMaximoPermitido)
                        return RetornarFalhaLinha("O valor informado para a coluna CubagemPedido é maior que o permitido");
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colPesoPedido = (from obj in linha.Colunas where obj.NomeCampo == "PesoPedido" select obj).FirstOrDefault();
                decimal pesoPedido = 0;
                if (colPesoPedido != null)
                {
                    if (colPesoPedido.Valor != null)
                        pesoPedido = Utilidades.Decimal.Converter((string)colPesoPedido.Valor.Replace("-", ""));
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colObservacaoPedido = (from obj in linha.Colunas where obj.NomeCampo == "ObservacaoPedido" select obj).FirstOrDefault();
                string observacaoPedido = string.Empty;
                if (colObservacaoPedido != null)
                {
                    observacaoPedido = (string)colObservacaoPedido.Valor;
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTipoTomador = (from obj in linha.Colunas where obj.NomeCampo == "TipoTomador" select obj).FirstOrDefault();
                Dominio.Enumeradores.TipoTomador tipoTomador = Dominio.Enumeradores.TipoTomador.Remetente;
                bool tomadorDefinidoPedido = false;
                if (colTipoTomador != null)
                {
                    string stringTipoTomador = (string)colTipoTomador.Valor;
                    if (!string.IsNullOrWhiteSpace(stringTipoTomador))
                    {
                        tomadorDefinidoPedido = true;

                        if (stringTipoTomador.ToUpper().Trim() == "DESTINATARIO" || stringTipoTomador.ToUpper().Trim() == "DESTINATÁRIO")
                            tipoTomador = Dominio.Enumeradores.TipoTomador.Destinatario;
                        else if (stringTipoTomador.ToUpper().Trim() == "EXPEDIDOR" && expedidor != null)
                            tipoTomador = Dominio.Enumeradores.TipoTomador.Expedidor;
                        else if (stringTipoTomador.ToUpper().Trim() == "RECEBEDOR" && recebedor != null)
                            tipoTomador = Dominio.Enumeradores.TipoTomador.Recebedor;
                        else if (stringTipoTomador.ToUpper().Trim() == "OUTROS" && tomador != null)
                            tipoTomador = Dominio.Enumeradores.TipoTomador.Outros;
                    }
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaNumeroCargaEncaixar = (from obj in linha.Colunas where obj.NomeCampo == "NumeroCargaEncaixar" select obj).FirstOrDefault();
                string numeroCargaEncaixar = string.Empty;
                if (colunaNumeroCargaEncaixar != null)
                {
                    numeroCargaEncaixar = (string)colunaNumeroCargaEncaixar.Valor;
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaNumeroControle = (from obj in linha.Colunas where obj.NomeCampo == "NumeroControle" select obj).FirstOrDefault();
                string numeroControle = string.Empty;
                if (colunaNumeroControle != null)
                    numeroControle = (string)colunaNumeroControle.Valor;


                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTipoEmbarque = (from obj in linha.Colunas where obj.NomeCampo == "TipoEmbarque" select obj).FirstOrDefault();
                string tipoEmbarque = string.Empty;
                if (colTipoEmbarque != null)
                    tipoEmbarque = (string)colTipoEmbarque.Valor;

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colChaveNFe = (from obj in linha.Colunas where obj.NomeCampo == "ChaveNFe" select obj).FirstOrDefault();
                string chaveNFe = string.Empty;
                if (colChaveNFe != null)
                    chaveNFe = (string)colChaveNFe.Valor;

                DateTime? dataPrevisaoTerminoCarregamento = null;
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDataPrevisaoTerminoCarregamento = (from obj in linha.Colunas where obj.NomeCampo == "DataPrevisaoTerminoCarregamento" select obj).FirstOrDefault();
                string dataPrevisaoTermino = "";
                if (colDataPrevisaoTerminoCarregamento != null)
                {
                    dataPrevisaoTermino = colDataPrevisaoTerminoCarregamento.Valor;
                    double.TryParse(dataPrevisaoTermino, out double dataFormatoExcel);
                    if (dataFormatoExcel > 0)
                        dataPrevisaoTerminoCarregamento = Utilidades.DateTime.ConverterDataExcelToDateTime(dataFormatoExcel);
                    else if (!string.IsNullOrWhiteSpace(dataPrevisaoTermino))
                    {
                        DateTime.TryParse(dataPrevisaoTermino, out DateTime tempPrevisaoTerminoCarregamento);
                        dataPrevisaoTerminoCarregamento = tempPrevisaoTerminoCarregamento;
                    }
                }

                foreach (int nfNumero in notasParciais)
                {
                    Dominio.Entidades.Embarcador.Pedidos.PedidoNotaParcial pedidoNotaParcial = repositorioPedidoNotaParcial.BuscarPorIntegradaOutroPedido(remetente.CPF_CNPJ, nfNumero, filial?.Codigo ?? 0);
                    Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xMLNotaFiscal = repXMLNotaFiscal.BuscarPorNumero(pedidoNotaParcial?.Numero ?? 0);
                    if (pedidoNotaParcial != null)
                    {
                        bool chaveNFeUtilizada = !string.IsNullOrEmpty(chaveNFe) ? (xMLNotaFiscal?.Chave ?? string.Empty) == chaveNFe : true;
                        if (pedidoNotaParcial.Pedido.NumeroPedidoEmbarcador != numeroPedido && chaveNFeUtilizada)
                        {
                            if (pedidoNotaParcial.Pedido.CargasPedido.Count > 0)
                            {
                                return RetornarFalhaLinha("A nota fiscal (nf: " + nfNumero + ") foi integrado anteriormente em outro pedido.");
                            }
                            else
                            {
                                pedidoNotaParcial.Pedido.SituacaoPedido = SituacaoPedido.Cancelado;
                                repositorioPedido.Atualizar(pedidoNotaParcial.Pedido, auditado);
                            }
                        }
                    }
                }

                foreach (int numeroCte in ctesParciais)
                {
                    Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParcial pedidoCTeParcial = repositorioPedidoCTeParcial.BuscarPorIntegradoOutroPedido(remetente.CPF_CNPJ, numeroCte, filial?.Codigo ?? 0);
                    if (pedidoCTeParcial != null)
                    {
                        if (pedidoCTeParcial.Pedido.NumeroPedidoEmbarcador != numeroPedido)
                        {
                            if (pedidoCTeParcial.Pedido.CargasPedido.Count > 0)
                            {
                                return RetornarFalhaLinha("O CT-e (" + numeroCte + ") foi integrado anteriormente em outro pedido.");
                            }
                            else
                            {
                                pedidoCTeParcial.Pedido.SituacaoPedido = SituacaoPedido.Cancelado;
                                repositorioPedido.Atualizar(pedidoCTeParcial.Pedido, auditado);
                            }
                        }
                    }
                }

                if (configuracaoTMS.UsarMesmoNumeroPreCargaGerarCargaViaImportacao && configuracaoTMS.CancelarCargaExistenteAutomaticamenteNaImportacaoDePedido && tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                {
                    Dominio.Entidades.Embarcador.Cargas.Carga cargaExiste = !string.IsNullOrWhiteSpace(numeroPreCarga) ? repCarga.BuscarPorCodigoEmbarcador(numeroPreCarga, filial?.Codigo ?? 0) : null;

                    if (cargaExiste == null && configuracaoTMS.BuscarCargaPorNumeroPedido)
                        cargaExiste = repCarga.BuscarAtivaPorNumeroPedido(numeroPedido);

                    if (cargaExiste != null)
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = servicoCargaJanelaCarregamentoConsulta.ObterCargaJanelaCarregamentoPorCarga(cargaExiste.Codigo);

                        if (cargaJanelaCarregamento != null)
                        {
                            cargaJanelaCarregamento.HorarioEncaixado = false;
                            servicoCargaJanelaCarregamentoDisponibilidade.AlterarHorarioCarregamento(cargaJanelaCarregamento, dataHoraCarregamento, tipoServicoMultisoftware);
                        }
                        else
                        {
                            if (dataHoraCarregamento > DateTime.MinValue)
                            {
                                cargaExiste.DataCarregamentoCarga = dataHoraCarregamento;
                                repCarga.Atualizar(cargaExiste);
                                Servicos.Auditoria.Auditoria.Auditar(auditado, cargaExiste, null, "Carga atualizada via importação de planilha", unitOfWork);
                            }

                            servicoFluxoGestaoPatio.AtualizarDataPrevisaoInicioEtapas(cargaExiste, dataHoraCarregamento);
                        }

                        if (serCarga.VerificarSeCargaEstaNaLogistica(cargaExiste, tipoServicoMultisoftware) && (!configuracaoTMS.BuscarCargaPorNumeroPedido || cargaExiste.Empresa != empresa))
                        {
                            Dominio.ObjetosDeValor.Embarcador.Carga.CargaCancelamentoAdicionar cargaCancelamentoAdicionar = new Dominio.ObjetosDeValor.Embarcador.Carga.CargaCancelamentoAdicionar()
                            {
                                Carga = cargaExiste,
                                MotivoCancelamento = "Viagem atualizada ao importar pedido",
                                TipoServicoMultisoftware = tipoServicoMultisoftware
                            };

                            Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = Servicos.Embarcador.Carga.Cancelamento.GerarCargaCancelamento(cargaCancelamentoAdicionar, configuracaoTMS, unitOfWork);
                            Servicos.Embarcador.Carga.Cancelamento.SolicitarCancelamentoCarga(ref cargaCancelamento, unitOfWork, StringConexao, tipoServicoMultisoftware);
                        }
                    }
                }

                Dominio.ObjetosDeValor.Embarcador.Frete.FreteValor freteValor = ObterFreteValor(linha, false, unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Frete.FreteValor freteValorFilialEmissora = ObterFreteValor(linha, true, unitOfWork);

                List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal> nfes = new List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal>();
                int numeroNotaCliente = 0;
                Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal nfe = ObterNFe(linha, remetente, destinatario, ref numeroNotaCliente);

                if (nfe != null)
                {
                    nfes.Add(nfe);
                    numeroNotaCliente = 0;//Só salva o número a parte se não gerar a nota
                }

                Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga = repPreCarga.BuscarPorNumeroPreCarga(numeroPreCarga, lstPreCarga);


                if (preCarga == null && !string.IsNullOrWhiteSpace(numeroPreCarga))
                {
                    preCarga = CriarPreCarga(numeroPreCarga, empresa, veiculo, veiculoReboque, motorista, tipoCarga, modeloVeicularCarga, tipoOperacao, cargaDePreCarga, cargaPreCarga, filial, horarioCarregamentoInformadoNoPedido, dataHoraPrevisaoInicioViagem, motivo, unitOfWork, tipoServicoMultisoftware);

                }

                /*Produtos do pedido ASSAÍ*/
                Dominio.Entidades.Embarcador.Produtos.GrupoProduto grupoProdutoEmbarcador = null;
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colGrupoProdutoEmbarcador = (from obj in linha.Colunas where obj.NomeCampo == "GrupoProdutos" select obj).FirstOrDefault();
                if (colGrupoProdutoEmbarcador != null)
                {
                    string stringCodigoGrupoProdutoEmbarcador = colGrupoProdutoEmbarcador.Valor;
                    if (!string.IsNullOrWhiteSpace(stringCodigoGrupoProdutoEmbarcador))
                    {
                        grupoProdutoEmbarcador = repGrupoProduto.BuscarPorCodigoEmbarcador(stringCodigoGrupoProdutoEmbarcador);
                        //Se não achou o grupo vamos criar um...
                        if (grupoProdutoEmbarcador == null)
                        {
                            grupoProdutoEmbarcador = new Dominio.Entidades.Embarcador.Produtos.GrupoProduto();
                            grupoProdutoEmbarcador.Ativo = true;
                            grupoProdutoEmbarcador.CodigoGrupoProdutoEmbarcador = stringCodigoGrupoProdutoEmbarcador;
                            grupoProdutoEmbarcador.Descricao = stringCodigoGrupoProdutoEmbarcador;
                            repGrupoProduto.Inserir(grupoProdutoEmbarcador);
                        }
                    }
                }

                Dominio.Entidades.Embarcador.Pedidos.PedidoProduto pedidoProduto = null;
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCodigoProduto = (from obj in linha.Colunas where obj.NomeCampo == "CodigoProdutoEmbarcador" select obj).FirstOrDefault();
                Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoEmbarcador = null;

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colLinhaSeparacao = (from obj in linha.Colunas where obj.NomeCampo == "LinhaSeparacaoProduto" select obj).FirstOrDefault();

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDeposito = (from obj in linha.Colunas where obj.NomeCampo == "Deposito" select obj).FirstOrDefault();

                bool quebraPedidoRoteirizacao = false;
                decimal quantidadePedidoProduto = 0;
                decimal pesoTotalPedidoProduto = 0;
                decimal metroCubicoPedidoProduto = 0;
                decimal valorProdutoPedidoProduto = 0;
                bool palletFechadoPedidoProduto = false;
                decimal precoUnitarioPedidoProduto = 0;

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDetPedProd = (from obj in linha.Colunas where obj.NomeCampo == "QuantidadePedidoProduto" select obj).FirstOrDefault();
                if (colDetPedProd != null)
                {
                    if (colDetPedProd.Valor != null)
                        quantidadePedidoProduto = Utilidades.Decimal.Converter((string)colDetPedProd.Valor.Replace("-", ""));
                }
                colDetPedProd = (from obj in linha.Colunas where obj.NomeCampo == "PesoTotalPedidoProduto" select obj).FirstOrDefault();
                if (colDetPedProd != null)
                {
                    if (colDetPedProd.Valor != null)
                        pesoTotalPedidoProduto = Utilidades.Decimal.Converter((string)colDetPedProd.Valor.Replace("-", ""));
                }
                colDetPedProd = (from obj in linha.Colunas where obj.NomeCampo == "MetroCubicoPedidoProduto" select obj).FirstOrDefault();
                if (colDetPedProd != null)
                {
                    if (colDetPedProd.Valor != null)
                        metroCubicoPedidoProduto = Utilidades.Decimal.Converter((string)colDetPedProd.Valor.Replace("-", ""));
                }
                colDetPedProd = (from obj in linha.Colunas where obj.NomeCampo == "ValorTotalPedidoProduto" select obj).FirstOrDefault();
                if (colDetPedProd != null)
                {
                    if (colDetPedProd.Valor != null)
                        valorProdutoPedidoProduto = Utilidades.Decimal.Converter((string)colDetPedProd.Valor.Replace("-", ""));
                }
                colDetPedProd = (from obj in linha.Colunas where obj.NomeCampo == "PalletFechadoPedidoProduto" select obj).FirstOrDefault();
                if (colDetPedProd != null)
                {
                    if (colDetPedProd.Valor != null)
                    {
                        string tmp = (string)colDetPedProd.Valor;
                        palletFechadoPedidoProduto = (tmp.ToUpper() == "SIM" || tmp.ToUpper() == "TRUE" ? true : false);
                    }
                }
                colDetPedProd = (from obj in linha.Colunas where obj.NomeCampo == "PrecoUnitario" select obj).FirstOrDefault();
                if (colDetPedProd != null)
                {
                    if (colDetPedProd.Valor != null)
                        precoUnitarioPedidoProduto = Utilidades.Decimal.Converter((string)colDetPedProd.Valor.Replace("-", ""));
                }

                Dominio.Entidades.Embarcador.WMS.Deposito deposito = null;
                if (colDeposito != null)
                {
                    string stringCodigoDeposito = (string)colDeposito.Valor;
                    if (!string.IsNullOrWhiteSpace(stringCodigoDeposito))
                    {
                        stringCodigoDeposito = stringCodigoDeposito.Trim();
                        deposito = repDeposito.BuscarPorCodigoIntegracao(stringCodigoDeposito);
                        if (deposito == null)
                        {
                            deposito = new Dominio.Entidades.Embarcador.WMS.Deposito()
                            {
                                Ativo = true,
                                CodigoIntegracao = stringCodigoDeposito,
                                Descricao = stringCodigoDeposito
                            };
                            repDeposito.Inserir(deposito);
                        }
                    }
                }

                if (colCodigoProduto != null)
                {
                    string stringCodigoProdutoEmbarcador = (string)colCodigoProduto.Valor;
                    if (!string.IsNullOrWhiteSpace(stringCodigoProdutoEmbarcador))
                    {
                        //Vamos localizar alinha de separação do produto.
                        Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacao linhaSeparacao = null;
                        if (colLinhaSeparacao != null)
                        {
                            string codigoLinhaSeparacao = (string)colLinhaSeparacao.Valor;
                            linhaSeparacao = repLinhaSeparacao.BuscarPorCodigoIntegracao(codigoLinhaSeparacao);
                            if (linhaSeparacao == null)
                            {
                                linhaSeparacao = new Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacao()
                                {
                                    Ativo = true,
                                    CodigoIntegracao = codigoLinhaSeparacao,
                                    Descricao = codigoLinhaSeparacao,
                                    Observacao = codigoLinhaSeparacao,
                                    Roteiriza = true,
                                    NivelPrioridade = 99,
                                    Filial = filial
                                };
                                repLinhaSeparacao.Inserir(linhaSeparacao);
                            }
                        }
                        //Localiza o produto cadastrado
                        produtoEmbarcador = repProdutoEmbarcador.buscarPorCodigoEmbarcador(stringCodigoProdutoEmbarcador);
                        if (produtoEmbarcador == null)
                        {
                            string descrProdutoEmbarcador = stringCodigoProdutoEmbarcador;
                            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDescricaoProduto = (from obj in linha.Colunas where obj.NomeCampo == "DescricaoProdutoEmbarcador" select obj).FirstOrDefault();
                            if (colDescricaoProduto != null)
                            {
                                if (colDescricaoProduto.Valor != null)
                                    descrProdutoEmbarcador = colDescricaoProduto.Valor;
                            }
                            produtoEmbarcador = new Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador();
                            produtoEmbarcador.Ativo = true;
                            produtoEmbarcador.CodigoProdutoEmbarcador = stringCodigoProdutoEmbarcador;
                            produtoEmbarcador.Descricao = descrProdutoEmbarcador;
                            produtoEmbarcador.GrupoProduto = grupoProdutoEmbarcador;
                            produtoEmbarcador.PesoUnitario = (pesoTotalPedidoProduto / (quantidadePedidoProduto == 0 ? 1 : quantidadePedidoProduto));
                            produtoEmbarcador.MetroCubito = metroCubicoPedidoProduto;
                            produtoEmbarcador.LinhaSeparacao = linhaSeparacao;
                            produtoEmbarcador.Integrado = false;
                            try
                            {
                                repProdutoEmbarcador.Inserir(produtoEmbarcador);
                            }
                            catch
                            {
                                return RetornarFalhaLinha(Localization.Resources.Pedidos.Pedido.NaoPossivelLocalizarAdicionarProduto);
                            }
                        }
                        else if (linhaSeparacao != null)
                        {
                            //Se a linhad e separação do produto, for diferente da atual.. vamos atualizar...
                            if (produtoEmbarcador.LinhaSeparacao != linhaSeparacao)
                            {
                                produtoEmbarcador.LinhaSeparacao = linhaSeparacao;
                                repProdutoEmbarcador.Atualizar(produtoEmbarcador);
                            }
                        }
                        quebraPedidoRoteirizacao = true;
                    }
                }

                //Se tiver produto.. vamos pegar os detalhes.
                if (produtoEmbarcador != null)
                {
                    pedidoProduto = new Dominio.Entidades.Embarcador.Pedidos.PedidoProduto();
                    pedidoProduto.Produto = produtoEmbarcador;
                    pedidoProduto.Quantidade = quantidadePedidoProduto;
                    pedidoProduto.PesoUnitario = (pesoTotalPedidoProduto / (quantidadePedidoProduto == 0 ? 1 : quantidadePedidoProduto));
                    pedidoProduto.MetroCubico = metroCubicoPedidoProduto;
                    pedidoProduto.ValorProduto = valorProdutoPedidoProduto;
                    pedidoProduto.PalletFechado = palletFechadoPedidoProduto;
                    pedidoProduto.PrecoUnitario = precoUnitarioPedidoProduto;
                    pedidoProduto.QuantidadePalet = decimal.Round(quantidadePalletsFacionada, 2, MidpointRounding.AwayFromZero);
                    pedidoProduto.LinhaSeparacao = produtoEmbarcador.LinhaSeparacao;
                    pedidoProduto.QuantidadeCaixa = produtoEmbarcador.QuantidadeCaixa;
                    pedidoProduto.QuantidadeCaixaPorPallet = produtoEmbarcador.QuantidadeCaixaPorPallet;
                    pedidoProduto.Canal = canal;
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCanalEntrega = (from obj in linha.Colunas where obj.NomeCampo == "CanalEntrega" select obj).FirstOrDefault();
                Dominio.Entidades.Embarcador.Pedidos.CanalEntrega canalEntrega = null;
                if (colCanalEntrega != null)
                    canalEntrega = repCanalEntrega.BuscarPorCodigoIntegracao((string)colCanalEntrega.Valor);

                int qtdVolumes = 0;
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colQtdVolumes = (from obj in linha.Colunas where obj.NomeCampo == "QtdVolumes" select obj).FirstOrDefault();
                if (colQtdVolumes != null)
                    qtdVolumes = (int)(((string)colQtdVolumes.Valor).ToDecimal());

                decimal valorTotalPedido = 0;
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colValorTotalPedido = (from obj in linha.Colunas where obj.NomeCampo == "ValorTotalPedido" select obj).FirstOrDefault();
                if (colValorTotalPedido != null)
                    decimal.TryParse((string)colValorTotalPedido.Valor, out valorTotalPedido);

                List<Dominio.Entidades.Usuario> motoristas = null;
                List<Dominio.Entidades.Veiculo> reboques = null;

                if (veiculoReboque != null)
                {
                    reboques = new List<Dominio.Entidades.Veiculo>();
                    reboques.Add(veiculoReboque);
                }

                if (motorista != null)
                {
                    motoristas = new List<Dominio.Entidades.Usuario>();
                    motoristas.Add(motorista);

                }

                string codigoProdutoPrincipal = "";
                Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoPrincipal = null;
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colProdutoPrincipal = (from obj in linha.Colunas where obj.NomeCampo == "ProdutoPrincipal" select obj).FirstOrDefault();
                if (colProdutoPrincipal != null)
                {
                    codigoProdutoPrincipal = ((string)colProdutoPrincipal.Valor).ToString();
                    if (!string.IsNullOrWhiteSpace(codigoProdutoPrincipal))
                        produtoPrincipal = repProdutoEmbarcador.buscarPorCodigoEmbarcador(codigoProdutoPrincipal);
                }

                DateTime? dataHoraPrevisaoSaida = null;
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDataHoraPrevisaoSaida = (from obj in linha.Colunas where obj.NomeCampo == "DataPrevisaoSaida" select obj).FirstOrDefault();
                string dataPrevisaoSaidaString = "";
                if (colDataHoraPrevisaoSaida != null)
                {
                    dataPrevisaoSaidaString = colDataHoraPrevisaoSaida.Valor;
                    double.TryParse(dataPrevisaoSaidaString, out double dataFormatoExcel);

                    if (dataFormatoExcel > 0)
                        dataHoraPrevisaoSaida = Utilidades.DateTime.ConverterDataExcelToDateTime(dataFormatoExcel);
                    else if (!string.IsNullOrWhiteSpace(dataPrevisaoSaidaString) && DateTime.TryParse(dataPrevisaoSaidaString, out DateTime dataHoraPrevisaoSaidaConvertida))
                        dataHoraPrevisaoSaida = dataHoraPrevisaoSaidaConvertida;
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDataAlocacaoPedido = (from obj in linha.Colunas where obj.NomeCampo == "DataAlocacaoPedido" select obj).FirstOrDefault();
                DateTime? dataAlocacaoPedido = null;
                SituacaoAgendamentoEntregaPedido? situacaoAgendamentoEntregaPedido = null;
                if (colDataAlocacaoPedido != null)
                {
                    DateTime dataFormatada = DateTime.ParseExact(colDataAlocacaoPedido.Valor, "yyyyMMdd", null);
                    if (dataFormatada != DateTime.MinValue)
                        dataAlocacaoPedido = dataFormatada;

                    situacaoAgendamentoEntregaPedido = SituacaoAgendamentoEntregaPedido.AguardandoAgendamento;
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCidade = (from obj in linha.Colunas where obj.NomeCampo == "Cidade" select obj).FirstOrDefault();
                string cidadeString = "";
                Dominio.Entidades.Localidade cidade = null;
                if (colCidade != null)
                {
                    cidadeString = Utilidades.String.SanitizeString(colCidade.Valor);

                    string estadoString = "";

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colEstado = (from obj in linha.Colunas where obj.NomeCampo == "Estado" select obj).FirstOrDefault();
                    if (colEstado != null)
                        estadoString = colEstado.Valor;

                    cidade = repositorioLocalidade.BuscarPorUFDescricao(estadoString, cidadeString).FirstOrDefault();

                    if (cidade == null)
                        return RetornarFalhaLinha("A cidade de destino não foi encontrada na base da Multisoftware");
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNumeroOrdem = (from obj in linha.Colunas where obj.NomeCampo == "NumeroOrdem" select obj).FirstOrDefault();
                string numeroOrdem = "";
                if (colNumeroOrdem != null)
                    numeroOrdem = colNumeroOrdem.Valor;

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colGrossSales = (from obj in linha.Colunas where obj.NomeCampo == "GrossSales" select obj).FirstOrDefault();
                decimal grossSales = 0m;
                if (colGrossSales != null)
                    grossSales = Utilidades.Decimal.Converter((string)colGrossSales.Valor.Replace("-", ""));


                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colEtiquetagem = (from obj in linha.Colunas where obj.NomeCampo == "Etiquetagem" select obj).FirstOrDefault();
                bool possuiEtiquetagem = false;
                if (colEtiquetagem != null)
                {
                    string etiquetagem = colEtiquetagem.Valor;

                    possuiEtiquetagem = etiquetagem.Replace(" ", "").Equals("B");
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colIsca = (from obj in linha.Colunas where obj.NomeCampo == "Isca" select obj).FirstOrDefault();
                bool possuiIsca = false;
                if (colIsca != null)
                {
                    string isca = colIsca.Valor;

                    possuiIsca = isca.Replace(" ", "").Equals("Y");
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colVolumesPrevios = (from obj in linha.Colunas where obj.NomeCampo == "VolumesPrevios" select obj).FirstOrDefault();
                int quantidadeVolumesPrevios = 0;
                if (colVolumesPrevios != null)
                    quantidadeVolumesPrevios = ((string)colVolumesPrevios.Valor).ToInt();

                DateTime dataHoraPrevisaoColeta = DateTime.MinValue;
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna coldataHoraPrevisaoColeta = (from obj in linha.Colunas where obj.NomeCampo == "DataHoraPrevisaoColeta" select obj).FirstOrDefault();

                if (coldataHoraPrevisaoColeta != null)
                {
                    string dataPrevisaoColeta = "";
                    dataPrevisaoColeta = coldataHoraPrevisaoColeta.Valor;
                    double.TryParse(dataPrevisaoColeta, out double dataFormatoExcel);

                    if (dataFormatoExcel > 0)
                        dataHoraPrevisaoColeta = Utilidades.DateTime.ConverterDataExcelToDateTime(dataFormatoExcel);
                    else if (!string.IsNullOrWhiteSpace(dataPrevisaoColeta))
                        dataHoraPrevisaoColeta = DateTime.ParseExact(dataPrevisaoColeta, "dd/MM/yyyy HH:mm:ss", cultura);
                }

                DateTime dataHoraColetaContainer = DateTime.MinValue;
                Dominio.Entidades.Embarcador.Pedidos.Container container = null;
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaNumeroContainer = (from obj in linha.Colunas where obj.NomeCampo == "NumeroContainer" select obj).FirstOrDefault();
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna coldataColetaContainer = (from obj in linha.Colunas where obj.NomeCampo == "DataColetaContainer" select obj).FirstOrDefault();
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaPropriedadeContainer = (from obj in linha.Colunas where obj.NomeCampo == "PropriedadeContainer" select obj).FirstOrDefault();
                string numeroContainer = string.Empty;
                string propriedadeContainer = string.Empty;

                if (colunaNumeroContainer != null && coldataColetaContainer != null)
                {
                    numeroContainer = (string)colunaNumeroContainer.Valor;
                    if (string.IsNullOrEmpty(numeroContainer))
                        return RetornarFalhaLinha(Localization.Resources.Pedidos.Pedido.NumeroContainerNaoPodeVazio);

                    string dataColetaContainer = "";
                    dataColetaContainer = coldataColetaContainer.Valor;
                    double.TryParse(dataColetaContainer, out double dataFormatoExcel);

                    propriedadeContainer = (string)colunaPropriedadeContainer.Valor;

                    if (dataFormatoExcel > 0)
                        dataHoraColetaContainer = Utilidades.DateTime.ConverterDataExcelToDateTime(dataFormatoExcel);
                    else if (!string.IsNullOrWhiteSpace(dataColetaContainer))
                        dataHoraColetaContainer = DateTime.ParseExact(dataColetaContainer, "dd/MM/yyyy HH:mm:ss", cultura);

                    if (dataHoraColetaContainer == DateTime.MinValue)
                        return RetornarFalhaLinha(Localization.Resources.Pedidos.Pedido.DataHoraColetaContainerNaoInformada);

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTipoContainer = (from obj in linha.Colunas where obj.NomeCampo == "TipoContainer" select obj).FirstOrDefault();
                    Dominio.Entidades.Embarcador.Pedidos.ContainerTipo containerTipo = null;
                    if (colTipoContainer != null)
                    {
                        string codigoIntegracaoTipoContainer = (string)colTipoContainer.Valor;
                        containerTipo = repTipoContainer.BuscarPorCodigoIntegracao(codigoIntegracaoTipoContainer);
                        if (containerTipo == null) return RetornarFalhaLinha(Localization.Resources.Pedidos.Pedido.OTipoContainerCadastradoNaoExisteBaseMultisoftware);
                    }

                    // verificar se nao existe, e criar novo container;
                    container = repContainer.BuscarPorNumero(numeroContainer);
                    if (container == null)
                    {
                        container = new Dominio.Entidades.Embarcador.Pedidos.Container();
                        container.ContainerTipo = containerTipo;
                        container.TipoPropriedade = string.IsNullOrWhiteSpace(propriedadeContainer) ? TipoPropriedadeContainer.Soc : TipoPropriedadeContainer.Proprio;
                        container.Numero = Utilidades.String.SanitizeString(numeroContainer);
                        container.ClienteArmador = remetente;
                        container.Descricao = numeroContainer;
                        container.DataUltimaAtualizacao = DateTime.Now;
                        container.Status = true;

                        repContainer.Inserir(container, auditado);

                        if ((configuracaoPedido?.ValidarCadastroContainerPelaFormulaGlobal ?? false) && container.TipoPropriedade != TipoPropriedadeContainer.Soc && !string.IsNullOrWhiteSpace(container.Numero))
                        {
                            if (!serPedido.ValidarDigitoContainerNumero(container.Numero))
                            {
                                return RetornarFalhaLinha(Localization.Resources.Pedidos.Pedido.NumeroContainerInvalidoAcordoComDigitoVerificado);
                            }
                        }
                    }
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaDataCriacaoPedidoERP = (from obj in linha.Colunas where obj.NomeCampo == "DataCriacaoPedidoERP" select obj).FirstOrDefault();
                DateTime dataCriacaoPedidoERP = DateTime.MinValue;
                string dtCriacaoPedidoERPString = "";
                if (colunaDataCriacaoPedidoERP != null)
                {
                    dtCriacaoPedidoERPString = (string)colunaDataCriacaoPedidoERP.Valor;

                    if (!DateTime.TryParseExact(dtCriacaoPedidoERPString, "yyyyMMdd", cultura, DateTimeStyles.None, out dataCriacaoPedidoERP))
                    {
                        double.TryParse(dtCriacaoPedidoERPString, out double dataFormatoExcel);

                        if (dataFormatoExcel > 0)
                            dataCriacaoPedidoERP = Utilidades.DateTime.ConverterDataExcelToDateTime(dataFormatoExcel);
                        else if (!string.IsNullOrWhiteSpace(dtCriacaoPedidoERPString))
                            DateTime.TryParse(dtCriacaoPedidoERPString, out dataCriacaoPedidoERP);
                    }
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaValorTotalNotasFiscais = (from obj in linha.Colunas where obj.NomeCampo == "ValorTotalNotasFiscais" select obj).FirstOrDefault();
                decimal valorTotalNotasFiscais = 0m;
                if (colunaValorTotalNotasFiscais != null)
                {
                    if (colunaValorTotalNotasFiscais.Valor != null)
                        valorTotalNotasFiscais = Utilidades.Decimal.Converter((string)colunaValorTotalNotasFiscais.Valor.Replace("-", ""));
                }



                // atualiza a data do carregamento
                Dominio.Entidades.Embarcador.Cargas.Carga cargaExistente = null;

                if (!string.IsNullOrWhiteSpace(numeroPreCarga))
                    cargaExistente = cargaDePreCarga == true ? repCarga.BuscarPorCodigoEmbarcadorCargaFechada(numeroPreCarga) : repCarga.BuscarPorCodigoEmbarcador(numeroPreCarga);

                if ((colDataCarregamento != null) && (cargaExistente != null) && !cargaExistente.SituacaoCarga.IsSituacaoCargaNaoEmitida())
                    return RetornarFalhaLinha(Localization.Resources.Pedidos.Pedido.ACargaEstaSituacaoNaoPermiteSerAlterada);

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNumeroDoca = (from obj in linha.Colunas where obj.NomeCampo == "NumeroDoca" select obj).FirstOrDefault();
                string numeroDoca = "";
                if (colNumeroDoca != null)
                    numeroDoca = colNumeroDoca.Valor;


                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCodigoIntegracaoFaixaTemperatura = (from obj in linha.Colunas where obj.NomeCampo == "FaixaTemperatura" select obj).FirstOrDefault();
                string codigoIntegracaoFaixaTemperatura = "";
                Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura faixaTemperatura = null;

                if (colCodigoIntegracaoFaixaTemperatura != null)
                {
                    Repositorio.Embarcador.Cargas.FaixaTemperatura repFaixaTemperatura = new Repositorio.Embarcador.Cargas.FaixaTemperatura(unitOfWork);
                    codigoIntegracaoFaixaTemperatura = colCodigoIntegracaoFaixaTemperatura.Valor;
                    faixaTemperatura = repFaixaTemperatura.BuscarPorCodigoIntegracao(codigoIntegracaoFaixaTemperatura);
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTipoPagamento = (from obj in linha.Colunas where obj.NomeCampo == "TipoPagamento" select obj).FirstOrDefault();
                Dominio.Enumeradores.TipoPagamento tipoPagamento = Dominio.Enumeradores.TipoPagamento.Pago;
                if (colTipoPagamento != null)
                {
                    string stringTipoPagamento = (string)colTipoPagamento.Valor;
                    if (!string.IsNullOrWhiteSpace(stringTipoPagamento))
                    {
                        if (stringTipoPagamento.ToUpper().Trim() == "OUTROS")
                            tipoPagamento = Dominio.Enumeradores.TipoPagamento.Outros;
                        else if (stringTipoPagamento.ToUpper().Trim() == "PAGO")
                            tipoPagamento = Dominio.Enumeradores.TipoPagamento.Pago;
                        else if (stringTipoPagamento.ToUpper().Trim() == "A PAGAR")
                            tipoPagamento = Dominio.Enumeradores.TipoPagamento.A_Pagar;
                    }
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNumeroEXP = (from obj in linha.Colunas where obj.NomeCampo == "NumeroEXP" select obj).FirstOrDefault();
                string numeroEXP = "";
                if (colNumeroEXP != null)
                    numeroEXP = colNumeroEXP.Valor;

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNotaMercadoLivre = (from obj in linha.Colunas where obj.NomeCampo == "NotaMercadoLivre" select obj).FirstOrDefault();
                string notaMercadoLivre = "";
                if (colNotaMercadoLivre != null)
                    notaMercadoLivre = colNotaMercadoLivre.Valor;

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colSiglaFaturamentoMercadoLivre = (from obj in linha.Colunas where obj.NomeCampo == "SiglaFaturamentoMercadoLivre" select obj).FirstOrDefault();
                string siglaFaturamentoMercadoLivre = "";
                if (colSiglaFaturamentoMercadoLivre != null)
                    siglaFaturamentoMercadoLivre = colSiglaFaturamentoMercadoLivre.Valor;

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colPFMercadoLivre = (from obj in linha.Colunas where obj.NomeCampo == "PFMercadoLivre" select obj).FirstOrDefault();
                string pfMercadoLivre = "";
                if (colPFMercadoLivre != null)
                    pfMercadoLivre = colPFMercadoLivre.Valor;

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colItemFaturadoMercadoLivre = (from obj in linha.Colunas where obj.NomeCampo == "ItemFaturadoMercadoLivre" select obj).FirstOrDefault();
                string itemFaturadoMercadoLivre = "I";
                if (colItemFaturadoMercadoLivre != null)
                    itemFaturadoMercadoLivre = colItemFaturadoMercadoLivre.Valor;

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTemperatura = (from obj in linha.Colunas where obj.NomeCampo == "Temperatura" select obj).FirstOrDefault();
                string temperatura = "";
                if (colTemperatura != null)
                    temperatura = colTemperatura.Valor;

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colPortoOrigem = (from obj in linha.Colunas where obj.NomeCampo == "PortoOrigem" select obj).FirstOrDefault();
                Dominio.Entidades.Cliente portoOrigem = null;
                if (colPortoOrigem != null)
                {
                    string descricaoPortoOrigem = (string)colPortoOrigem.Valor;
                    if (!string.IsNullOrWhiteSpace(descricaoPortoOrigem))
                    {
                        portoOrigem = repCliente.BuscarPorCodigoIntegracao(descricaoPortoOrigem);
                        if (portoOrigem == null) portoOrigem = repCliente.BuscarPorNome(descricaoPortoOrigem);

                        if (portoOrigem == null) return RetornarFalhaLinha(Localization.Resources.Pedidos.Pedido.OPortoOrigemInformadoNaoCadastradoBaseMultisoftware);
                    }
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colPortoDestino = (from obj in linha.Colunas where obj.NomeCampo == "PortoDestino" select obj).FirstOrDefault();
                Dominio.Entidades.Cliente portoDestino = null;
                if (colPortoDestino != null)
                {
                    string descricaoPortoDestino = (string)colPortoDestino.Valor;
                    if (!string.IsNullOrWhiteSpace(descricaoPortoDestino))
                    {
                        portoDestino = repCliente.BuscarPorCodigoIntegracao(descricaoPortoDestino);
                        if (portoDestino == null) portoDestino = repCliente.BuscarPorNome(descricaoPortoDestino);

                        if (portoDestino == null) return RetornarFalhaLinha(Localization.Resources.Pedidos.Pedido.OPortoOrigemInformadoNaoCadastradoBaseMultisoftware);
                    }
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colViaTransporte = (from obj in linha.Colunas where obj.NomeCampo == "ViaTransporte" select obj).FirstOrDefault();
                Dominio.Entidades.Embarcador.Cargas.ViaTransporte viaTransporte = null;
                if (colViaTransporte != null)
                {
                    string descricaoViaTransporte = (string)colViaTransporte.Valor;
                    if (!string.IsNullOrWhiteSpace(descricaoViaTransporte))
                    {
                        Repositorio.Embarcador.Cargas.ViaTransporte repViaTransporte = new Repositorio.Embarcador.Cargas.ViaTransporte(unitOfWork);
                        viaTransporte = repViaTransporte.BuscarPorDescricao(descricaoViaTransporte);

                        if (viaTransporte == null) return RetornarFalhaLinha(Localization.Resources.Pedidos.Pedido.AViaTransporteInformadaNaoCadastradoBaseMultisoftware);
                    }
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDespachante = (from obj in linha.Colunas where obj.NomeCampo == "Despachante" select obj).FirstOrDefault();
                Dominio.Entidades.Cliente despachante = null;
                if (colDespachante != null)
                {
                    string descricaoDespachante = (string)colDespachante.Valor;
                    if (!string.IsNullOrWhiteSpace(descricaoDespachante))
                    {
                        despachante = repCliente.BuscarPorCodigoIntegracao(descricaoDespachante);
                        if (despachante == null) despachante = repCliente.BuscarPorNome(descricaoDespachante);

                        if (despachante == null) return RetornarFalhaLinha(Localization.Resources.Pedidos.Pedido.ODespachanteInformadoNaoCadastradaBaseMultisoftware);
                    }
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colClienteFinal = (from obj in linha.Colunas where obj.NomeCampo == "ClienteFinal" select obj).FirstOrDefault();
                Dominio.Entidades.Cliente clienteFinal = null;
                if (colClienteFinal != null)
                {
                    string stringClienteFinal = (string)colClienteFinal.Valor;
                    if (!string.IsNullOrWhiteSpace(stringClienteFinal))
                    {
                        clienteFinal = repCliente.BuscarPorNomeIgualOuParecido(stringClienteFinal);

                        if (clienteFinal == null) return RetornarFalhaLinha(Localization.Resources.Pedidos.Pedido.OClienteFinalInformadoNaoCadastradoBaseMultisoftware);
                    }
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colPaisDestino = (from obj in linha.Colunas where obj.NomeCampo == "PaisDestino" select obj).FirstOrDefault();
                Dominio.Entidades.Pais paisDestino = null;
                if (colPaisDestino != null)
                {
                    string descricaoPaisDestino = (string)colPaisDestino.Valor;
                    if (!string.IsNullOrWhiteSpace(descricaoPaisDestino))
                    {
                        Repositorio.Pais repPais = new Repositorio.Pais(unitOfWork);
                        paisDestino = repPais.BuscarPorNome(descricaoPaisDestino);

                        if (paisDestino == null) return RetornarFalhaLinha(Localization.Resources.Pedidos.Pedido.CadastroPaisNaoEncontrado);
                    }
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colFretePrepaid = (from obj in linha.Colunas where obj.NomeCampo == "FretePrepaid" select obj).FirstOrDefault();
                FretePrepaid fretePrepaid = FretePrepaid.Collect;
                if (colFretePrepaid != null)
                {
                    string stringFretePrepaid = (string)colFretePrepaid.Valor;
                    if (!string.IsNullOrWhiteSpace(stringFretePrepaid))
                    {
                        if (stringFretePrepaid.ToUpper().Trim() == "COLLECT")
                            fretePrepaid = FretePrepaid.Collect;
                        else if (stringFretePrepaid.ToUpper().Trim() == "PREPAID")
                            fretePrepaid = FretePrepaid.Prepaid;
                        else if (stringFretePrepaid.ToUpper().Trim() == "PREPAID ABROAD")
                            fretePrepaid = FretePrepaid.PrepaidAbroad;
                    }
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colLocalidade = (from obj in linha.Colunas where obj.NomeCampo == "LocalidadePedidoDestino" select obj).FirstOrDefault();
                int codigoIntegracaoLocalidade = 0;
                Dominio.Entidades.Localidade localidadeEnderecoDestino = null;

                if (colLocalidade?.Valor != null && colLocalidade?.Valor != "")
                {
                    int.TryParse(colLocalidade.Valor, out codigoIntegracaoLocalidade);

                    localidadeEnderecoDestino = repLocalidade.BuscarPorCodigoIBGE(codigoIntegracaoLocalidade);

                    if (localidadeEnderecoDestino == null)
                        return RetornarFalhaLinha(Localization.Resources.Pedidos.Pedido.NaoFoiEncontradoLocalidadeComCodigoIntegracaoBaseMultisoftware);
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colBairro = (from obj in linha.Colunas where obj.NomeCampo == "BairroPedidoDestino" select obj).FirstOrDefault();
                string bairroPedidoEndereco = "";
                if (colBairro?.Valor != null)
                    bairroPedidoEndereco = colBairro.Valor;

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCEP = (from obj in linha.Colunas where obj.NomeCampo == "CEPPedidoDestino" select obj).FirstOrDefault();
                string cepPedidoEndereco = "";
                if (colCEP?.Valor != null)
                    cepPedidoEndereco = colCEP.Valor;

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colLogradouro = (from obj in linha.Colunas where obj.NomeCampo == "EnderecoPedidoDestino" select obj).FirstOrDefault();
                string logradouroEnderecoDestino = "";
                if (colLogradouro?.Valor != null)
                    logradouroEnderecoDestino = colLogradouro.Valor;

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colComplemento = (from obj in linha.Colunas where obj.NomeCampo == "ComplementoPedidoDestino" select obj).FirstOrDefault();
                string complementoPedidoEndereco = "";
                if (colComplemento?.Valor != null)
                    complementoPedidoEndereco = colComplemento.Valor;

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNumero = (from obj in linha.Colunas where obj.NomeCampo == "NumeroPedidoDestino" select obj).FirstOrDefault();
                string numeroPedidoEndereco = "S/N";
                if (colNumero?.Valor != null)
                    numeroPedidoEndereco = colNumero.Valor;

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTelefone = (from obj in linha.Colunas where obj.NomeCampo == "TelefonePedidoDestino" select obj).FirstOrDefault();
                string telefonePedidoEndereco = "";
                if (colTelefone?.Valor != null)
                    telefonePedidoEndereco = colTelefone.Valor;

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colIE = (from obj in linha.Colunas where obj.NomeCampo == "IEPedidoDestino" select obj).FirstOrDefault();
                string rgiePedidoEndereco = "";
                if (colIE?.Valor != null)
                    rgiePedidoEndereco = colIE.Valor;

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDistancia = (from obj in linha.Colunas where obj.NomeCampo == "Distancia" select obj).FirstOrDefault();
                decimal distancia = 0;
                if (colDistancia?.Valor != null)
                    distancia = ((string)colDistancia.Valor).ToDecimal();

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNumeroPedidoOrigem = (from obj in linha.Colunas where obj.NomeCampo == "NumeroPedidoOrigem" select obj).FirstOrDefault();
                string numeroPedidoOrigem = string.Empty;

                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = null;
                if (colNumeroPedidoOrigem?.Valor != null)
                {
                    numeroPedidoOrigem = colNumeroPedidoOrigem.Valor;

                    pedido = repositorioPedido.BuscarPorNumeroPedidoEmbarcador(numeroPedidoOrigem, lstPedidosOrigem);

                    if (pedido == null)
                        return RetornarFalhaLinha(Localization.Resources.Pedidos.Pedido.OPedidoNaoFoiEncontrado);
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colEssePedidopossuiPedidoBonificacao = (from obj in linha.Colunas where obj.NomeCampo == "EssePedidopossuiPedidoBonificacao" select obj).FirstOrDefault();
                bool essePedidopossuiPedidoBonificacao = false;
                if (colEssePedidopossuiPedidoBonificacao?.Valor != null)
                    essePedidopossuiPedidoBonificacao = (bool)colEssePedidopossuiPedidoBonificacao.Valor;

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colEssePedidopossuiPedidoVenda = (from obj in linha.Colunas where obj.NomeCampo == "EssePedidopossuiPedidoVenda" select obj).FirstOrDefault();
                bool essePedidopossuiPedidoVenda = false;
                if (colEssePedidopossuiPedidoVenda?.Valor != null)
                    essePedidopossuiPedidoVenda = (bool)colEssePedidopossuiPedidoVenda.Valor;

                if (essePedidopossuiPedidoVenda && essePedidopossuiPedidoBonificacao)
                    return RetornarFalhaLinha(Localization.Resources.Pedidos.Pedido.NaoPossivelSelecionarEssePedidoPossuiPedidoBonificacaoEVenda);

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNumeroPedidoVinculado = (from obj in linha.Colunas where obj.NomeCampo == "NumeroPedidoVinculado" select obj).FirstOrDefault();
                string numeroPedidoVinculado = string.Empty;
                if (colNumeroPedidoVinculado?.Valor != null)
                    numeroPedidoVinculado = (string)colNumeroPedidoVinculado.Valor;

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colLocalidadeEnderecoSecundario = (from obj in linha.Colunas where obj.NomeCampo == "CodigoEnderecoSecundario" select obj).FirstOrDefault();
                string codigoIntegracaoEnderecoSecundario = "";

                bool devolucaoPacotes = false;
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDevolucaoPacotes = (from obj in linha.Colunas where obj.NomeCampo == "DevolucaoPacotes" select obj).FirstOrDefault();
                if (colDevolucaoPacotes != null && colDevolucaoPacotes.Valor != null)
                    devolucaoPacotes = colDevolucaoPacotes.Valor == "Sim";

                Dominio.Entidades.Embarcador.Escrituracao.ContratoFreteCliente contratoFreteCliente = null;
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colContratoFreteCliente = (from obj in linha.Colunas where obj.NomeCampo == "NumeroContratoFreteCliente" select obj).FirstOrDefault();
                if (colContratoFreteCliente != null && colContratoFreteCliente.Valor != string.Empty)
                {
                    contratoFreteCliente = repContratoFreteCliente.BuscarPorNumeroContrato(colContratoFreteCliente.Valor);
                    if (contratoFreteCliente == null)
                        return RetornarFalhaLinha(Localization.Resources.Pedidos.Pedido.NumeroContratoFreteClienteNaoEncontrado);
                    else if (contratoFreteCliente.Fechado)
                        return RetornarFalhaLinha(Localization.Resources.Pedidos.Pedido.ContratoFreteClienteFechado);
                }

                Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco localidadeEnderecoSecundario = null;
                if (colLocalidadeEnderecoSecundario?.Valor != null && colLocalidadeEnderecoSecundario?.Valor != "")
                {
                    codigoIntegracaoEnderecoSecundario = colLocalidadeEnderecoSecundario.Valor;

                    localidadeEnderecoSecundario = repositorioClienteOutroEndereco.BuscarClientePorCodigoIntegracao(codigoIntegracaoEnderecoSecundario);

                    if (localidadeEnderecoSecundario == null)
                        return RetornarFalhaLinha(Localization.Resources.Pedidos.Pedido.NaoFoiEncontradoLocalidadeComCodigoIntegracaoBaseMultisoftware);
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colRotaMercadoLivre = (from obj in linha.Colunas where obj.NomeCampo == "RotaMercadoLivre" select obj).FirstOrDefault();
                int rotaMercadoLivre = 0;
                if (colRotaMercadoLivre?.Valor != null)
                    rotaMercadoLivre = int.Parse(colRotaMercadoLivre.Valor);

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colFacilityMercadoLivre = (from obj in linha.Colunas where obj.NomeCampo == "FacilityMercadoLivre" select obj).FirstOrDefault();
                string facilityMercadoLivre = string.Empty;
                if (!string.IsNullOrWhiteSpace(colFacilityMercadoLivre?.Valor))
                    facilityMercadoLivre = colFacilityMercadoLivre.Valor;

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCargaPreCarga = (from obj in linha.Colunas where obj.NomeCampo == "CargaDePreCarga" select obj).FirstOrDefault();
                if (colCargaPreCarga != null)
                    cargaDePreCarga = string.Equals((string)colCargaPreCarga?.Valor, "sim", StringComparison.OrdinalIgnoreCase);

                // Só é gerado o pedido quando for informado um numero e for diferente de vazio e quando não for para ativar ou cancelar o pedido
                // Ou quando a coluna não for informada
                if ((colNumeroPedido == null || !string.IsNullOrEmpty(numeroPedido)) && (!configuracaoTMS.ControlarAgendamentoSKU || (!cancelarPedido && !ativarPedido)))
                {
                    if (remetente == null)
                    {
                        return RetornarFalhaLinha(Localization.Resources.Pedidos.Pedido.RemetenteDeveSerInformado);
                    }

                    if (dataHoraCarregamento.Date == DateTime.MinValue.Date)
                        return RetornarFalhaLinha(Localization.Resources.Pedidos.Pedido.OcorreuUmaFalhaAoEnviarIntegracaoPedido);

                    bool utilizarParametrosBuscaAutomaticaClienteImportacao =
                            configuracaoPedido?.UtilizarParametrosBuscaAutomaticaClienteImportacao
                            ?? false;

                    if (utilizarParametrosBuscaAutomaticaClienteImportacao)
                    {
                        Dominio.Entidades.Cliente novoRemetente = BuscarRemetentePelaChaveDaNFe(nfe, unitOfWork);
                        remetente = novoRemetente != null ? novoRemetente : remetente;

                        Dominio.ObjetosDeValor.Embarcador.ClienteBuscaAutomatica.ParametrosClienteBuscaAutomatica parametrosClienteBuscaAutomatica = new()
                        {
                            CPFCNPJDestinatario = destinatario?.CPF_CNPJ,
                            CPFCNPJRemetente = remetente?.CPF_CNPJ,
                            CodigoFilial = filial?.Codigo,
                            CodigoOrigem = origem?.Codigo,
                            TipoParticipante = TipoParticipante.Expedidor
                        };

                        if (expedidor == null)
                            expedidor = BuscarClientePorBuscaAutomatica(parametrosClienteBuscaAutomatica, unitOfWork, cancellationToken);

                        if (tomador == null && !tomadorDefinidoPedido)
                        {
                            parametrosClienteBuscaAutomatica.TipoParticipante = TipoParticipante.Tomador;
                            Dominio.Entidades.Cliente possivelTomador = BuscarClientePorBuscaAutomatica(parametrosClienteBuscaAutomatica, unitOfWork, cancellationToken);

                            tomador = AjustarTomador(
                                possivelTomador,
                                new Dominio.ObjetosDeValor.Embarcador.Pedido.Participantes() { Tomador = tomador, Remetente = remetente, Destinatario = destinatario, Expedidor = expedidor, Recebedor = recebedor },
                                ref tipoTomador
                            );
                        }
                    }

                    Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoImportacaoSalvar pedidoImportacaoAdicionar = new Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoImportacaoSalvar()
                    {
                        Empresa = empresa,
                        PreCarga = preCarga,
                        CargaExistente = cargaExistente,
                        Nfes = nfes,
                        NotasParciais = notasParciais,
                        CtesParciais = ctesParciais,
                        NumeroPedido = numeroPedido,
                        QuantidadePalletsFacionada = quantidadePalletsFacionada,
                        PesoPedido = pesoPedido,
                        CubagemPedido = cubagemPedido,
                        TipoCarga = tipoCarga,
                        ModeloVeicularCarga = modeloVeicularCarga,
                        TipoOperacao = tipoOperacao,
                        Remetente = remetente,
                        Destinatario = destinatario,
                        Expedidor = expedidor,
                        Recebedor = recebedor,
                        LocalExpedicao = localExpedicao,
                        Filial = filial,
                        RotaFrete = rotaFrete,
                        FreteValor = freteValor,
                        FreteValorFilialEmissora = freteValorFilialEmissora,
                        Usuario = usuario,
                        DataHoraCarregamento = dataHoraCarregamento,
                        DataHoraPrevisaoSaida = dataHoraPrevisaoSaida,
                        DataHoraPrevisaoInicioViagem = dataHoraPrevisaoInicioViagem,
                        TipoTomador = tipoTomador,
                        TipoEmbarque = tipoEmbarque,
                        Observacao = observacaoPedido,
                        OrdemColeta = ordemColeta,
                        DataHoraDescarga = dataHoraDescarga,
                        DataValidade = dataValidade,
                        DataInicioJanelaDescarga = dataInicioJanelaDescarga,
                        NomeArquivoGerador = arquivoGerador.Nome,
                        GuidArquivoGerador = arquivoGerador.Guid,
                        PedidoProduto = pedidoProduto,
                        CanalEntrega = canalEntrega,
                        Veiculo = veiculo,
                        Reboques = reboques,
                        Motoristas = motoristas,
                        Adicional1 = adicional1,
                        Adicional2 = adicional2,
                        Adicional3 = adicional3,
                        Adicional4 = adicional4,
                        Adicional5 = adicional5,
                        Adicional6 = adicional6,
                        Adicional7 = adicional7,
                        ObservacaoAdicional = observacaoAdicional,
                        DataPrevEntrega = dataPrevEntrega,
                        NumeroCargaEncaixar = numeroCargaEncaixar,
                        NumeroControle = numeroControle,
                        QuebraMultiplosCarregamentos = quebraPedidoRoteirizacao,
                        Deposito = deposito,
                        QtdVolumes = qtdVolumes,
                        ValorTotalPedido = valorTotalPedido,
                        CentroResultado = centroResultado,
                        GrupoPessoasRemetente = grupoPessoasRemetente,
                        ProdutoPrincipal = produtoPrincipal,
                        NumeroNotaCliente = numeroNotaCliente,
                        Cidade = cidade,
                        GrossSales = grossSales,
                        PossuiEtiquetagem = possuiEtiquetagem,
                        PossuiIsca = possuiIsca,
                        DataAlocacaoPedido = dataAlocacaoPedido,
                        NumeroOrdem = numeroOrdem,
                        Container = container,
                        DataColetaContainer = dataHoraColetaContainer,
                        SituacaoAgendamentoEntregaPedido = (destinatario?.ExigeQueEntregasSejamAgendadas ?? false) ? situacaoAgendamentoEntregaPedido : SituacaoAgendamentoEntregaPedido.NaoExigeAgendamento,
                        QuantidadeVolumesPrevios = quantidadeVolumesPrevios,
                        NumeroPedidoCliente = numeroPedidoCliente,
                        ValorTotalNotasFiscais = valorTotalNotasFiscais,
                        DataCriacaoPedidoERP = dataCriacaoPedidoERP,
                        ObservacaoInterna = observacaoInterna,
                        NumeroDoca = numeroDoca,
                        Tomador = tomador,
                        FaixaTemperatura = faixaTemperatura,
                        TipoPagamento = tipoPagamento,
                        DataHoraPrevisaoColeta = dataHoraPrevisaoColeta,
                        NumeroEXP = numeroEXP,
                        Temperatura = temperatura,
                        PortoViagemOrigem = portoOrigem,
                        PortoViagemDestino = portoDestino,
                        ViaTransporte = viaTransporte,
                        Despachante = despachante,
                        ClienteFinal = clienteFinal,
                        PaisDestino = paisDestino,
                        FretePrepaid = fretePrepaid,
                        Fronteiras = listaFronteiras,
                        NotaMercadoLivre = notaMercadoLivre,
                        SiglaFaturamentoMercadoLivre = siglaFaturamentoMercadoLivre,
                        PFMercadoLivre = pfMercadoLivre,
                        ItemFaturadoMercadoLivre = itemFaturadoMercadoLivre,
                        LocalidadePedidoDestino = localidadeEnderecoDestino,
                        EnderecoPedidoDestino = logradouroEnderecoDestino,
                        NumeroPedidoDestino = numeroPedidoEndereco,
                        ComplementoPedidoDestino = complementoPedidoEndereco,
                        CEPPedidoDestino = cepPedidoEndereco,
                        BairroPedidoDestino = bairroPedidoEndereco,
                        TelefonePedidoDestino = telefonePedidoEndereco,
                        IEPedidoDestino = rgiePedidoEndereco,
                        DistanciaCarga = distancia,
                        CodigoEnderecoSecundario = localidadeEnderecoSecundario,
                        PedidoImportadoPorPlanilha = pedidoImportadoPorPlanilha,
                        NumeroPedidoOrigem = pedido,
                        EssePedidopossuiPedidoBonificacao = essePedidopossuiPedidoBonificacao,
                        EssePedidopossuiPedidoVenda = essePedidopossuiPedidoVenda,
                        NumeroPedidoVinculado = numeroPedidoVinculado,
                        DataPrevisaoTerminoCarregamento = dataPrevisaoTerminoCarregamento,
                        CentroCarregamento = centroCarregamento,
                        DevolucaoPacotes = devolucaoPacotes,
                        CentroDeCustoViagem = centroCustoDeViagem,
                        ContratoFreteCliente = contratoFreteCliente,
                        Canal = canal,
                        Origem = origem,
                        Ajudante = ajudante,
                        QuantidadeAjudantes = quantidadeAjudantes,
                        Rota = rotaMercadoLivre,
                        Facility = facilityMercadoLivre,
                    };

                    retornoLinhaPedido = SalvarPedidoImportacao(pedidoImportacaoAdicionar, naoTentarGerarCarga, tomadorDefinidoPedido, unitOfWork, configuracaoTMS, tipoServicoMultisoftware, clienteMultisoftware, auditado);
                    if (!string.IsNullOrWhiteSpace(retornoLinhaPedido.mensagemFalha))
                        return RetornarFalhaLinha(retornoLinhaPedido.mensagemFalha);
                }

                if (!string.IsNullOrEmpty(numeroPedido) && ativarPedido)
                {
                    Dominio.Entidades.Embarcador.Cargas.Carga cargaExistenteCancelada = repCarga.BuscarPorNumeroPedidoCancelada(numeroPedido);
                    if (cargaExistenteCancelada != null)
                    {
                        Repositorio.Embarcador.Cargas.CargaCancelamento repCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unitOfWork);
                        Repositorio.Embarcador.Cargas.CargaCancelamentoLog repCargaCancelamentoLog = new Repositorio.Embarcador.Cargas.CargaCancelamentoLog(unitOfWork);
                        Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = repCargaCancelamento.BuscarPorCarga(cargaExistenteCancelada.Codigo);
                        if (cargaCancelamento != null)
                        {
                            List<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoLog> cargaCancelamentoLogs = repCargaCancelamentoLog.BuscarPorCargaCancelamento(cargaCancelamento.Codigo);
                            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoLog cargaCancelamentoLog in cargaCancelamentoLogs)
                                repCargaCancelamentoLog.Deletar(cargaCancelamentoLog);

                            repCargaCancelamento.Deletar(cargaCancelamento);

                        }

                        cargaExistenteCancelada.SituacaoCarga = SituacaoCarga.Nova;
                        repCarga.Atualizar(cargaExistenteCancelada);
                    }
                }

                if ((colDataCarregamento != null) && (cargaExistente != null) && cargaExistente.SituacaoCarga.IsSituacaoCargaNaoEmitida())
                {
                    cargaExistente.DataCarregamentoCarga = dataHoraCarregamento > DateTime.MinValue ? dataHoraCarregamento : DateTime.Now;
                    cargaExistente.TipoOperacao = tipoOperacao;

                    Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retornoValidacao = ValidarAtualizacaoCargaLinha(cargaExistente);

                    if (retornoValidacao != null)
                        return retornoValidacao;

                    repCarga.Atualizar(cargaExistente);
                    Servicos.Auditoria.Auditoria.Auditar(auditado, cargaExistente, null, "Carga atualizada via importação de planilha", unitOfWork);

                    Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = servicoCargaJanelaCarregamentoConsulta.ObterCargaJanelaCarregamentoPorCarga(cargaExistente.Codigo);

                    if (cargaJanelaCarregamento != null)
                    {
                        cargaJanelaCarregamento.Initialize();
                        cargaJanelaCarregamento.HorarioEncaixado = false;
                        servicoCargaJanelaCarregamentoDisponibilidade.AlterarHorarioCarregamento(cargaJanelaCarregamento, dataHoraCarregamento, tipoServicoMultisoftware);
                        Servicos.Auditoria.Auditoria.AuditarComAlteracoesRealizadas(auditado, cargaJanelaCarregamento, cargaJanelaCarregamento.GetChanges(), $"Data de carregamento alterada{(cargaJanelaCarregamento.Excedente ? "" : $" para {cargaJanelaCarregamento.InicioCarregamento.ToString("dd/MM/yyyy")}")}", unitOfWork);
                    }
                    else
                        servicoFluxoGestaoPatio.AtualizarDataPrevisaoInicioEtapas(cargaExistente, dataHoraCarregamento);
                }
                else if (preCarga != null)
                {
                    if (!preCargas.Contains(preCarga))
                        preCargas.Add(preCarga);
                }

                if (!string.IsNullOrEmpty(numeroPedido) && cancelarPedido)
                {
                    cargaExistente = repCarga.BuscarPorNumeroPedido(numeroPedido);

                    if (configuracaoTMS.ControlarAgendamentoSKU)
                    {
                        string mensagemCancelamentoPedidoAgendamentoColeta = string.Empty;

                        if (cargaExistente != null && cargaExistente.Pedidos.Where(obj => obj.Pedido.NumeroPedidoEmbarcador == numeroPedido).Select(obj => obj.Pedido).FirstOrDefault().SituacaoPedido != SituacaoPedido.Cancelado)
                            mensagemCancelamentoPedidoAgendamentoColeta = Localization.Resources.Pedidos.Pedido.NaoPossivelCancelarPoisPedidoJaEstaCarga;
                        else
                            CancelarPedidoAgendamentoColeta(numeroPedido, out mensagemCancelamentoPedidoAgendamentoColeta, usuario, auditado, tipoServicoMultisoftware, unitOfWork);

                        if (!string.IsNullOrWhiteSpace(mensagemCancelamentoPedidoAgendamentoColeta))
                            return RetornarFalhaLinha(mensagemCancelamentoPedidoAgendamentoColeta);
                    }
                    else
                    {
                        if (cargaExistente != null)
                        {
                            string erroCancelamento = string.Empty;

                            if (Carga.Cancelamento.ValidarSePossivelCancelar(cargaExistente, operadorLogistica, tipoServicoMultisoftware, out erroCancelamento, unitOfWork))
                                Carga.Cancelamento.GeraRegistrosCancelamento(cargaExistente, motivoCancelamentoCarga, usuario, auditado, tipoServicoMultisoftware, unitOfWork);
                            else
                            {
                                if (erroCancelamento != Localization.Resources.Pedidos.Pedido.ACargaSelecionadaJaEstCancelamentoCancelada)
                                    return RetornarFalhaLinha(erroCancelamento);
                                else if (preCargas.Contains(preCarga))
                                    preCargas.Remove(preCarga);
                            }
                        }
                        else if (preCarga != null)
                            cargasParaCancelamento.Add(preCarga.NumeroPreCarga);
                        else
                            return RetornarFalhaLinha(Localization.Resources.Pedidos.Pedido.NaoPossivelCancelarCargaPedido + numeroPedido);
                    }
                }
            }
            catch (BaseException excecao)
            {
                return RetornarFalhaLinha(excecao.Message);
            }
            catch (Exception ex2)
            {
                Servicos.Log.TratarErro(ex2);
                return RetornarFalhaLinha(Localization.Resources.Pedidos.Pedido.OcorreuUmaFalhaProcessarLinha + "(" + ex2.Message + ")");
            }

            return RetornarSucessoLinha(retornoLinhaPedido?.codigo ?? 0);
        }

        public Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha ImportarCargaPedidoPacoteLinha(Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha dadosLinha, int primeiroPedido, Repositorio.UnitOfWork unitOfWork, ref Dominio.Entidades.Cliente remetente, ref Dominio.Entidades.Cliente destinatario)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("pt-BR");
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Cargas.Pacote repPacote = new Repositorio.Embarcador.Cargas.Pacote(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoPacote repCargaPedidoPacote = new Repositorio.Embarcador.Cargas.CargaPedidoPacote(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retorno = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha()
            {
                codigo = 0,
                contar = false,
                processou = false,
            };

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colPedidosShopee = (from obj in dadosLinha.Colunas where obj.NomeCampo == "PedidosShopee" select obj).FirstOrDefault();

                if (colPedidosShopee != null)
                {
                    string pedidoShopee = (string)colPedidosShopee.Valor;
                    DateTime dataRecebimento = DateTime.Now;
                    string logKey = string.Empty;

                    List<string> pedidoShopeeSplitado = null;
                    if (!string.IsNullOrWhiteSpace(pedidoShopee))
                    {
                        pedidoShopeeSplitado = new List<string>(pedidoShopee.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));
                        logKey = pedidoShopeeSplitado[1];
                        if (remetente == null)
                            remetente = repCliente.BuscarPorCodigoAlternativo(pedidoShopeeSplitado[2]);
                        if (destinatario == null)
                            destinatario = repCliente.BuscarPorCodigoAlternativo(pedidoShopeeSplitado[3]);
                    }
                    if (string.IsNullOrWhiteSpace(logKey))
                    {
                        retorno.mensagemFalha = "LogKey inválido.";
                        return retorno;
                    }
                    if (remetente == null)
                    {
                        if (pedidoShopeeSplitado != null && pedidoShopeeSplitado.Count > 2)
                            retorno.mensagemFalha = $"Remetente {pedidoShopeeSplitado[2]} inválido.";
                        else
                            retorno.mensagemFalha = $"Remetente inválido.";
                        return retorno;
                    }

                    if (destinatario == null)
                    {
                        if (pedidoShopeeSplitado != null && pedidoShopeeSplitado.Count > 3)
                            retorno.mensagemFalha = $"Destinatario {pedidoShopeeSplitado[3]} inválido.";
                        else
                            retorno.mensagemFalha = $"Destinatario inválido.";
                        return retorno;
                    }

                    Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoPacoteImportacao cargaPedidoPacoteImportacao = new Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoPacoteImportacao();
                    cargaPedidoPacoteImportacao.LogKey = logKey;
                    cargaPedidoPacoteImportacao.DataRecebimento = dataRecebimento;
                    cargaPedidoPacoteImportacao.Destino = destinatario.CPF_CNPJ;
                    cargaPedidoPacoteImportacao.Origem = remetente.CPF_CNPJ;

                    if (!TodosPacotes.Any(obj => obj.LogKey == cargaPedidoPacoteImportacao.LogKey))
                    {
                        TodosPacotes.Add(cargaPedidoPacoteImportacao);
                        PacotesCache.Add(cargaPedidoPacoteImportacao);
                    }

                    retorno = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha()
                    {
                        codigo = primeiroPedido,
                        contar = true,
                        processou = true
                    };
                }
                return retorno;
            }
            catch
            {
                unitOfWork.Rollback();
                return retorno;
            }
        }

        public bool IsGerarCargaAutomaticamente(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            return (pedido.PedidoIntegradoEmbarcador && !pedido.ColetaEmProdutorRural && !pedido.Cotacao);
        }

        public void PreencherCodigoCargaEmbarcador(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Repositorio.UnitOfWork unitOfWork)
        {
            if (IsGerarCargaAutomaticamente(pedido) && pedido.GerarAutomaticamenteCargaDoPedido)
            {
                pedido.CodigoCargaEmbarcador = ObterCodigoCargaEmbarcador(unitOfWork, pedido, configuracaoTMS);
                pedido.AdicionadaManualmente = true;
            }
        }

        public bool ValidarDigitoContainerNumero(string numero)
        {
            if (numero.Length != 11)
                return false;

            int.TryParse(numero.Substring(10, 1), out int digitoVerificador);

            int posicao1 = RetornarPesoLetra(numero.Substring(0, 1));
            int posicao2 = RetornarPesoLetra(numero.Substring(1, 1)) * 2;
            int posicao3 = RetornarPesoLetra(numero.Substring(2, 1)) * 4;
            int posicao4 = RetornarPesoLetra(numero.Substring(3, 1)) * 8;

            int.TryParse(numero.Substring(4, 1), out int posicao5);
            posicao5 = posicao5 * 16;

            int.TryParse(numero.Substring(5, 1), out int posicao6);
            posicao6 = posicao6 * 32;

            int.TryParse(numero.Substring(6, 1), out int posicao7);
            posicao7 = posicao7 * 64;

            int.TryParse(numero.Substring(7, 1), out int posicao8);
            posicao8 = posicao8 * 128;

            int.TryParse(numero.Substring(8, 1), out int posicao9);
            posicao9 = posicao9 * 256;

            int.TryParse(numero.Substring(9, 1), out int posicao10);
            posicao10 = posicao10 * 512;

            int somaCampos = posicao1 + posicao2 + posicao3 + posicao4 + posicao5 + posicao6 + posicao7 + posicao8 + posicao9 + posicao10;
            int resto = somaCampos % 11;
            int digito = resto;
            if (digito >= 10)
                digito = 0;

            return digito == digitoVerificador;
        }

        private string obterNumeroPedido(Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNumeroPedido, string numeroPedido, ConfiguracaoPedido configuracaoPedido, string numeroPreCarga)
        {
            numeroPedido = (string)colNumeroPedido.Valor;
            if ((configuracaoPedido?.ConcatenarNumeroPreCargaNoPedido ?? false) && !string.IsNullOrWhiteSpace(numeroPreCarga))
                numeroPedido = numeroPreCarga + "_" + numeroPedido;
            return numeroPedido;
        }

        private string PossuiNumeroPreCarga(Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha, string prefixoPreCarga, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, ref Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNumeroAgrupamentoPrecarga, ref bool cargaDePreCarga)
        {
            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNumeroPreCarga = (from obj in linha.Colunas where obj.NomeCampo == "NumeroPreCarga" select obj).FirstOrDefault();
            string numeroPreCarga = "";
            if (colNumeroPreCarga != null)
            {
                numeroPreCarga = colNumeroPreCarga.Valor;
                if (!configuracaoTMS.UsarMesmoNumeroPreCargaGerarCargaViaImportacao)
                {
                    if (!string.IsNullOrWhiteSpace(numeroPreCarga))
                        numeroPreCarga = prefixoPreCarga + "_" + numeroPreCarga;
                }
            }


            colNumeroAgrupamentoPrecarga = (from obj in linha.Colunas where obj.NomeCampo == "AgrupamentoPreCarga" select obj).FirstOrDefault();
            cargaDePreCarga = false;
            string dataAgrupamentoPreCarga = "";
            if (colNumeroAgrupamentoPrecarga != null)
            {
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDataAgrupamentoPreCarga = (from obj in linha.Colunas where obj.NomeCampo == "DataCarregamento" select obj).FirstOrDefault();
                cargaDePreCarga = true;

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colFilial = (from obj in linha.Colunas where obj.NomeCampo == "Filial" select obj).FirstOrDefault();
                string codigoIntegracaoFilial = "";
                if (colFilial != null)
                    codigoIntegracaoFilial = (string)colFilial.Valor;

                numeroPreCarga = (string)colNumeroAgrupamentoPrecarga.Valor.PadLeft(6, '0');

                if (colDataAgrupamentoPreCarga != null)
                    dataAgrupamentoPreCarga = colDataAgrupamentoPreCarga.Valor;

                if (!string.IsNullOrEmpty(dataAgrupamentoPreCarga))
                    numeroPreCarga = dataAgrupamentoPreCarga + codigoIntegracaoFilial + numeroPreCarga;


                if (!configuracaoTMS.UsarMesmoNumeroPreCargaGerarCargaViaImportacao)
                {
                    if (!string.IsNullOrWhiteSpace(numeroPreCarga))
                        numeroPreCarga = prefixoPreCarga + "_" + numeroPreCarga;
                }
            }
            return numeroPreCarga;
        }


        private int RetornarPesoLetra(string letra)
        {
            switch (letra)
            {
                case "A":
                    return 10;
                case "B":
                    return 12;
                case "C":
                    return 13;
                case "D":
                    return 14;
                case "E":
                    return 15;
                case "F":
                    return 16;
                case "G":
                    return 17;
                case "H":
                    return 18;
                case "I":
                    return 19;
                case "J":
                    return 20;
                case "K":
                    return 21;
                case "L":
                    return 23;
                case "M":
                    return 24;
                case "N":
                    return 25;
                case "O":
                    return 26;
                case "P":
                    return 27;
                case "Q":
                    return 28;
                case "R":
                    return 29;
                case "S":
                    return 30;
                case "T":
                    return 31;
                case "U":
                    return 32;
                case "V":
                    return 34;
                case "W":
                    return 35;
                case "X":
                    return 36;
                case "Y":
                    return 37;
                case "Z":
                    return 38;
                default:
                    return 0;
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Frete.FreteValor ObterFreteValor(Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha, bool filialEmissora, Repositorio.UnitOfWork unitOfWork)
        {

            string strFilialEmissora = "";
            if (filialEmissora)
                strFilialEmissora = "FilialEmissora";

            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colValorFrete = (from obj in linha.Colunas where obj.NomeCampo == "ValorFrete" + strFilialEmissora select obj).FirstOrDefault();
            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colValorFreteLiquido = (from obj in linha.Colunas where obj.NomeCampo == "ValorFreteLiquido" + strFilialEmissora select obj).FirstOrDefault();

            decimal ValorFrete = 0;
            if (colValorFrete != null || colValorFreteLiquido != null)
            {
                Dominio.ObjetosDeValor.Embarcador.Frete.FreteValor freteValor = new Dominio.ObjetosDeValor.Embarcador.Frete.FreteValor();
                if (colValorFrete != null)
                {
                    ValorFrete = Utilidades.Decimal.Converter((string)colValorFrete.Valor);
                    freteValor.ValorTotalAReceber = ValorFrete;
                }

                if (colValorFreteLiquido != null)
                {
                    decimal valorFreteLiquido = Utilidades.Decimal.Converter((string)colValorFreteLiquido.Valor);
                    freteValor.FreteProprio = valorFreteLiquido;
                }

                freteValor.ComponentesAdicionais = new List<Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional>();

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDescarga = (from obj in linha.Colunas where obj.NomeCampo == "Descarga" + strFilialEmissora select obj).FirstOrDefault();
                if (colDescarga != null)
                {
                    decimal descarga = Utilidades.Decimal.Converter((string)colDescarga.Valor);

                    if (descarga > 0)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional componente = new Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional();
                        componente.Componente = new Dominio.ObjetosDeValor.Embarcador.Frete.Componente();
                        componente.Componente.TipoComponenteFrete = TipoComponenteFrete.DESCARGA;
                        componente.Componente.CodigoIntegracao = "descarga";
                        componente.IncluirBaseCalculoICMS = true;
                        componente.IncluirTotalReceber = true;
                        componente.ValorComponente = descarga;
                        freteValor.ComponentesAdicionais.Add(componente);
                    }
                }

                decimal valorPedagio = 0;
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colPedagioIda = (from obj in linha.Colunas where obj.NomeCampo == "PedagioIda" + strFilialEmissora select obj).FirstOrDefault();
                if (colPedagioIda != null)
                {
                    decimal pedagio = Utilidades.Decimal.Converter((string)colPedagioIda.Valor);
                    valorPedagio += pedagio;
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colPedagioVolta = (from obj in linha.Colunas where obj.NomeCampo == "PedagioVolta" + strFilialEmissora select obj).FirstOrDefault();
                if (colPedagioVolta != null)
                {
                    decimal pedagio = Utilidades.Decimal.Converter((string)colPedagioVolta.Valor);
                    valorPedagio += pedagio;
                }

                if (valorPedagio > 0)
                {
                    Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional componente = new Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional();
                    componente.Componente = new Dominio.ObjetosDeValor.Embarcador.Frete.Componente();
                    componente.Componente.TipoComponenteFrete = TipoComponenteFrete.PEDAGIO;
                    componente.Componente.CodigoIntegracao = "pedagio";
                    componente.IncluirBaseCalculoICMS = true;
                    componente.IncluirTotalReceber = true;
                    componente.ValorComponente = valorPedagio;
                    freteValor.ComponentesAdicionais.Add(componente);
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colAdValorem = (from obj in linha.Colunas where obj.NomeCampo == "AdValorem" + strFilialEmissora select obj).FirstOrDefault();
                if (colAdValorem != null)
                {
                    decimal adValorem = Utilidades.Decimal.Converter((string)colAdValorem.Valor);
                    if (adValorem > 0)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional componente = new Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional();
                        componente.Componente = new Dominio.ObjetosDeValor.Embarcador.Frete.Componente();
                        componente.Componente.TipoComponenteFrete = TipoComponenteFrete.ADVALOREM;
                        componente.Componente.CodigoIntegracao = "advalorem";
                        componente.IncluirBaseCalculoICMS = true;
                        componente.IncluirTotalReceber = true;
                        componente.ValorComponente = adValorem;
                        freteValor.ComponentesAdicionais.Add(componente);
                    }
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colOutrosValores = (from obj in linha.Colunas where obj.NomeCampo == "OutrosValores" + strFilialEmissora select obj).FirstOrDefault();
                if (colOutrosValores != null)
                {
                    decimal outros = Utilidades.Decimal.Converter((string)colOutrosValores.Valor);
                    if (outros > 0)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional componente = new Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional();
                        componente.Componente = new Dominio.ObjetosDeValor.Embarcador.Frete.Componente();
                        componente.Componente.TipoComponenteFrete = TipoComponenteFrete.OUTROS;
                        componente.Componente.CodigoIntegracao = "outrosvalores";
                        componente.IncluirBaseCalculoICMS = true;
                        componente.IncluirTotalReceber = true;
                        componente.ValorComponente = outros;
                        freteValor.ComponentesAdicionais.Add(componente);
                    }
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colEntrega = (from obj in linha.Colunas where obj.NomeCampo == "Entrega" + strFilialEmissora select obj).FirstOrDefault();
                if (colEntrega != null)
                {
                    decimal entrega = Utilidades.Decimal.Converter((string)colEntrega.Valor);
                    if (entrega > 0)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional componente = new Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional();
                        componente.Componente = new Dominio.ObjetosDeValor.Embarcador.Frete.Componente();
                        componente.Componente.TipoComponenteFrete = TipoComponenteFrete.OUTROS;
                        componente.Componente.CodigoIntegracao = "entrega";
                        componente.IncluirBaseCalculoICMS = true;
                        componente.IncluirTotalReceber = true;
                        componente.ValorComponente = entrega;
                        freteValor.ComponentesAdicionais.Add(componente);
                    }
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colAliquotaICMS = (from obj in linha.Colunas where obj.NomeCampo == "AliquotaICMS" + strFilialEmissora select obj).FirstOrDefault();

                if (colAliquotaICMS != null)
                {
                    freteValor.ICMS = new Dominio.ObjetosDeValor.Embarcador.ICMS.ICMS();

                    decimal aliquotaICMS = Utilidades.Decimal.Converter((string)colAliquotaICMS.Valor);
                    freteValor.ICMS.Aliquota = aliquotaICMS;

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colBaseCalculoICMS = (from obj in linha.Colunas where obj.NomeCampo == "BaseCalculoICMS" + strFilialEmissora select obj).FirstOrDefault();
                    if (colBaseCalculoICMS != null)
                    {
                        decimal baseCalculoICMS = Utilidades.Decimal.Converter((string)colBaseCalculoICMS.Valor);
                        freteValor.ICMS.ValorBaseCalculoICMS = baseCalculoICMS;
                    }

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colValorICMS = (from obj in linha.Colunas where obj.NomeCampo == "ValorICMS" + strFilialEmissora select obj).FirstOrDefault();
                    if (colValorICMS != null)
                    {
                        decimal valorICMS = Utilidades.Decimal.Converter((string)colValorICMS.Valor);
                        freteValor.ICMS.ValorICMS = valorICMS;
                    }

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colValorICMSPago = (from obj in linha.Colunas where obj.NomeCampo == "ValorICMSPago" + strFilialEmissora select obj).FirstOrDefault();
                    if (colValorICMSPago != null)
                    {
                        decimal valorICMSPago = Utilidades.Decimal.Converter((string)colValorICMSPago.Valor);
                        if (valorICMSPago > 0)
                        {
                            freteValor.ICMS.IncluirICMSBC = true;
                            freteValor.ICMS.PercentualInclusaoBC = 100;
                        }
                    }
                }

                return freteValor;

            }
            else
            {
                return null;
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal ObterNFe(Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha, Dominio.Entidades.Cliente emitente, Dominio.Entidades.Cliente destinatario, ref int numeroNF)
        {
            Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal nfe = new Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal();

            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colChaveNFe = (from obj in linha.Colunas where obj.NomeCampo == "ChaveNFe" select obj).FirstOrDefault();
            if (colChaveNFe != null)
                nfe.Chave = (string)colChaveNFe.Valor;
            else
            {
                //return null;
                // nfe.Modelo = "99";
            }

            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNumeroNFe = (from obj in linha.Colunas where obj.NomeCampo == "NumeroNFe" select obj).FirstOrDefault();
            if (colNumeroNFe != null)
            {
                int.TryParse((string)colNumeroNFe.Valor, out numeroNF);
                nfe.Numero = numeroNF;
            }
            else if (!string.IsNullOrWhiteSpace(nfe.Chave) && Utilidades.Validate.ValidarChaveNFe(nfe.Chave))
            {
                int numeroDaChave = Utilidades.Chave.ObterNumero(nfe.Chave);

                nfe.Numero = numeroDaChave;
            }
            else
                return null;

            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colSerieNFe = (from obj in linha.Colunas where obj.NomeCampo == "SerieNFe" select obj).FirstOrDefault();
            if (colSerieNFe != null)
                nfe.Serie = (string)colSerieNFe.Valor;
            //else
            //    return null;

            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNaturazaOPNFe = (from obj in linha.Colunas where obj.NomeCampo == "NaturazaOPNFe" select obj).FirstOrDefault();
            if (colNaturazaOPNFe != null)
                nfe.NaturezaOP = (string)colNaturazaOPNFe.Valor;

            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDataEmissaoNFe = (from obj in linha.Colunas where obj.NomeCampo == "DataEmissaoNFe" select obj).FirstOrDefault();
            if (colDataEmissaoNFe != null)
                nfe.DataEmissao = (string)colDataEmissaoNFe.Valor;
            else
                nfe.DataEmissao = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colPesoNFe = (from obj in linha.Colunas where obj.NomeCampo == "PesoNFe" select obj).FirstOrDefault();
            decimal pesoNFe = 0;
            if (colPesoNFe != null)
            {
                pesoNFe = Utilidades.Decimal.Converter((string)colPesoNFe.Valor);
                nfe.PesoBruto = pesoNFe;
                nfe.PesoLiquido = pesoNFe;
            }

            if (nfe.PesoBruto <= 0)
            {
                nfe.PesoBruto = (decimal)0.01;
                nfe.PesoLiquido = (decimal)0.01;
            }

            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colValorNFe = (from obj in linha.Colunas where obj.NomeCampo == "ValorNFe" select obj).FirstOrDefault();
            decimal valorNFe = 0;
            if (colValorNFe != null)
            {
                valorNFe = Utilidades.Decimal.Converter((string)colValorNFe.Valor);
                nfe.Valor = valorNFe;
            }
            else
                return null;

            nfe.Destinatario = Pessoa.Pessoa.Converter(destinatario);
            nfe.Emitente = Pessoa.Pessoa.Converter(emitente);
            nfe.ModalidadeFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.Pago;
            if (!string.IsNullOrWhiteSpace(nfe.Chave))
                nfe.Modelo = "55";
            else
                nfe.Modelo = "99";

            nfe.SituacaoNFeSefaz = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNFeSefaz.Autorizada;
            nfe.TipoOperacaoNotaFiscal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida;

            return nfe;
        }

        private string CriarFornecedor(ref Dominio.Entidades.Cliente participante, string codigoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            string retorno = "";
            Servicos.Cliente serCliente = new Cliente(unitOfWork.StringConexao);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            double cpf = repCliente.BuscarPorProximoExterior();

            Dominio.ObjetosDeValor.CTe.Cliente clienteEmbarcador = new Dominio.ObjetosDeValor.CTe.Cliente
            {
                Emails = "",
                Bairro = "Não Definido",
                CEP = "",
                Endereco = "Não Definido",
                Complemento = "",
                Numero = "S/N",
                Telefone1 = "",
                Telefone2 = "",
                Exportacao = true,
                CodigoIBGECidade = 0,
                NomeFantasia = codigoIntegracao.Length > 80 ? codigoIntegracao.Substring(0, 80) : codigoIntegracao,
                RGIE = "",
                CodigoCliente = codigoIntegracao,
                RazaoSocial = codigoIntegracao.Length > 80 ? codigoIntegracao.Substring(0, 80) : codigoIntegracao,
                CPFCNPJ = cpf.ToString(),
                CodigoAtividade = 3
            };

            Dominio.ObjetosDeValor.RetornoVerificacaoCliente retornoCliente = serCliente.converterClienteEmbarcador(clienteEmbarcador, "Remetente", unitOfWork);
            if (retornoCliente.Status)
                participante = retornoCliente.cliente;
            else
                retorno = retornoCliente.Mensagem;

            return retorno;
        }

        private string CriarParticipante(ref Dominio.Entidades.Cliente participante, string cnpjCPF, Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha, string tipoParticipante, string AdminStringConexao, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Repositorio.UnitOfWork unitOfWork)
        {
            string retorno = "";

            Servicos.Cliente serCliente = new Cliente(unitOfWork.StringConexao);
            Dominio.ObjetosDeValor.CTe.Cliente clienteEmbarcador = new Dominio.ObjetosDeValor.CTe.Cliente();
            clienteEmbarcador.Pais = configuracaoTMS.Pais;

            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colEmail = (from obj in linha.Colunas where obj.NomeCampo == "Email" + tipoParticipante select obj).FirstOrDefault();
            clienteEmbarcador.Emails = "";
            if (colEmail != null)
                clienteEmbarcador.Emails = colEmail.Valor;

            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colBairro = (from obj in linha.Colunas where obj.NomeCampo == "Bairro" + tipoParticipante select obj).FirstOrDefault();
            clienteEmbarcador.Bairro = "";
            if (colBairro != null)
                clienteEmbarcador.Bairro = colBairro.Valor;

            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCEP = (from obj in linha.Colunas where obj.NomeCampo == "CEP" + tipoParticipante select obj).FirstOrDefault();
            clienteEmbarcador.CEP = "";
            if (colCEP != null)
                clienteEmbarcador.CEP = colCEP.Valor;

            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna logradouroCEP = (from obj in linha.Colunas where obj.NomeCampo == "Logradouro" + tipoParticipante select obj).FirstOrDefault();
            clienteEmbarcador.Endereco = "";
            if (logradouroCEP != null)
                clienteEmbarcador.Endereco = logradouroCEP.Valor;
            else
                return "";

            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colComplemento = (from obj in linha.Colunas where obj.NomeCampo == "Complemento" + tipoParticipante select obj).FirstOrDefault();
            clienteEmbarcador.Complemento = "";
            if (colComplemento != null)
                clienteEmbarcador.Complemento = colComplemento.Valor;

            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNumero = (from obj in linha.Colunas where obj.NomeCampo == "Numero" + tipoParticipante select obj).FirstOrDefault();
            clienteEmbarcador.Numero = "S/N";
            if (colNumero != null)
                clienteEmbarcador.Numero = colNumero.Valor;

            if (string.IsNullOrWhiteSpace(clienteEmbarcador.Numero))
                clienteEmbarcador.Numero = "S/N";

            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTelefone = (from obj in linha.Colunas where obj.NomeCampo == "Telefone" + tipoParticipante select obj).FirstOrDefault();
            clienteEmbarcador.Telefone1 = "";
            clienteEmbarcador.Telefone2 = "";
            if (colTelefone != null)
                clienteEmbarcador.Telefone1 = colTelefone.Valor;

            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCodigoAtividade = (from obj in linha.Colunas where obj.NomeCampo == "CodigoAtividade" + tipoParticipante select obj).FirstOrDefault();
            clienteEmbarcador.CodigoAtividade = 3;
            if (colCodigoAtividade != null && !string.IsNullOrWhiteSpace(colCodigoAtividade.Valor))
            {
                bool possuiAtividade = false;
                int.TryParse(colCodigoAtividade.Valor, out int numero);
                possuiAtividade = ValidarCodigoAtividadePessoa(numero);
                if (possuiAtividade)
                    clienteEmbarcador.CodigoAtividade = numero;
            }

            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colIBGE = (from obj in linha.Colunas where obj.NomeCampo == "IBGE" + tipoParticipante select obj).FirstOrDefault();
            int codIBGE = 0;
            if (colIBGE != null)
                int.TryParse(Utilidades.String.OnlyNumbers((string)colIBGE.Valor), out codIBGE);
            else
            {
                if (!string.IsNullOrWhiteSpace(clienteEmbarcador.CEP))
                {
                    using (AdminMultisoftware.Repositorio.UnitOfWork adminUnitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(AdminStringConexao, TipoSessaoBancoDados.Nova))
                    {
                        AdminMultisoftware.Repositorio.Localidades.Endereco repEndereco = new AdminMultisoftware.Repositorio.Localidades.Endereco(adminUnitOfWork);
                        AdminMultisoftware.Dominio.Entidades.Localidades.Endereco enderecoCEP = repEndereco.BuscarCEP(Utilidades.String.OnlyNumbers(clienteEmbarcador.CEP));
                        if (enderecoCEP != null)
                        {
                            codIBGE = int.Parse(enderecoCEP.Localidade.CodigoIBGE);
                        }
                        else
                        {
                            return "Não foi encontrada a localidade para o CEP " + clienteEmbarcador.CEP + ".";
                        }
                    }
                }
                else
                {
                    return "";
                }
            }

            clienteEmbarcador.CodigoIBGECidade = codIBGE;
            clienteEmbarcador.CPFCNPJ = cnpjCPF;

            Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colFantasia = (from obj in linha.Colunas where obj.NomeCampo == "Fantasia" + tipoParticipante select obj).FirstOrDefault();
            clienteEmbarcador.NomeFantasia = "";
            if (colFantasia != null)
                clienteEmbarcador.NomeFantasia = colFantasia.Valor;

            if (string.IsNullOrWhiteSpace(clienteEmbarcador.CPFCNPJ) && configuracaoTMS.GerarCPFRandomicamenteDestinatarioImportacaoPlanilha)
            {
                clienteEmbarcador.CPFCNPJ = GerarCpf();
                clienteEmbarcador.NomeFantasia = "Cliente " + clienteEmbarcador.CPFCNPJ;
                clienteEmbarcador.RazaoSocial = "Cliente " + clienteEmbarcador.CPFCNPJ;
                clienteEmbarcador.RGIE = clienteEmbarcador.CPFCNPJ;
                clienteEmbarcador.CodigoAtividade = 7;
            }
            else
            {
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colIE = (from obj in linha.Colunas where obj.NomeCampo == "IE" + tipoParticipante select obj).FirstOrDefault();
                clienteEmbarcador.RGIE = "";
                if (colIE != null)
                    clienteEmbarcador.RGIE = colIE.Valor;
                else
                    return "";

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colRazao = (from obj in linha.Colunas where obj.NomeCampo == "Razao" + tipoParticipante select obj).FirstOrDefault();
                clienteEmbarcador.RazaoSocial = "";
                if (colRazao != null)
                    clienteEmbarcador.RazaoSocial = colRazao.Valor;
                else
                    return "";
            }

            Dominio.ObjetosDeValor.RetornoVerificacaoCliente retornoCliente = serCliente.converterClienteEmbarcador(clienteEmbarcador, tipoParticipante, unitOfWork);
            if (retornoCliente.Status)
                participante = retornoCliente.cliente;
            else
                retorno = retornoCliente.Mensagem;

            return retorno;
        }

        private bool ValidarCodigoAtividadePessoa(int valor)
        {
            if (valor >= 1 && valor <= 7)
                return true;

            return false;
        }

        private string GerarCpf()
        {
            int soma = 0, resto = 0;
            int[] multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            Random rnd = new Random();
            string semente = rnd.Next(100000000, 999999999).ToString();

            for (int i = 0; i < 9; i++)
                soma += int.Parse(semente[i].ToString()) * multiplicador1[i];

            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            semente = semente + resto;
            soma = 0;

            for (int i = 0; i < 10; i++)
                soma += int.Parse(semente[i].ToString()) * multiplicador2[i];

            resto = soma % 11;

            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            semente = semente + resto;
            return semente;
        }

        private Dominio.Entidades.Embarcador.PreCargas.PreCarga CriarPreCarga(string numeroPreCarga, Dominio.Entidades.Empresa empresa, Dominio.Entidades.Veiculo veiculo, Dominio.Entidades.Veiculo veiculoReboque, Dominio.Entidades.Usuario motorista, Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga, Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, bool cargaDePreCarga, Dominio.Entidades.Embarcador.Cargas.Carga cargaPreCarga, Dominio.Entidades.Embarcador.Filiais.Filial filial, bool horarioCarregamentoInformadoNoPedido, DateTime? dataHoraPrevisaoInicioViagem, Dominio.Entidades.Embarcador.Pedidos.MotivoImportacaoPedidoAtrasada motivo, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.PreCargas.PreCarga repPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Carga.CargaDadosSumarizados servicoCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);

            Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repositorioConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(unitOfWork);
            ConfiguracaoWebService configuracaoWebService = repositorioConfiguracaoWebService.BuscarPrimeiroRegistro();

            Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga = repPreCarga.BuscarAguardandoCargaPorNumero(numeroPreCarga, filial?.Codigo ?? 0);

            if (preCarga == null)
            {
                preCarga = new Dominio.Entidades.Embarcador.PreCargas.PreCarga();

                preCarga.ModeloVeicularCarga = modeloVeicularCarga;
                preCarga.CargaDePreCarga = cargaDePreCarga;
                preCarga.CargaPreCarga = cargaPreCarga;
                preCarga.MotivoImportacaoPedidoAtrasada = motivo;

                if (veiculo != null)
                {
                    if (veiculo.TipoVeiculo == "1")
                    {
                        if (veiculo.VeiculosTracao != null && veiculo.VeiculosTracao.Count > 0)
                            preCarga.Veiculo = veiculo.VeiculosTracao.FirstOrDefault();
                        else if (!configuracaoWebService?.AdicionarVeiculoTipoReboqueComoReboqueAoAdicionarCarga ?? false)
                            preCarga.Veiculo = veiculo;
                        else if (veiculoReboque == null && (configuracaoWebService?.AdicionarVeiculoTipoReboqueComoReboqueAoAdicionarCarga ?? false))
                            veiculoReboque = veiculo;

                    }
                    else
                        preCarga.Veiculo = veiculo;

                    if (preCarga.ModeloVeicularCarga == null)
                    {
                        if (preCarga.Veiculo.VeiculosVinculados != null && preCarga.Veiculo.VeiculosVinculados.Count > 0)
                            preCarga.ModeloVeicularCarga = preCarga.Veiculo.VeiculosVinculados.FirstOrDefault().ModeloVeicularCarga;
                        else
                            preCarga.ModeloVeicularCarga = preCarga.Veiculo.ModeloVeicularCarga;

                        if (preCarga.ModeloVeicularCarga == null)
                            preCarga.ModeloVeicularCarga = veiculo.ModeloVeicularCarga;
                    }
                    else
                    {
                        if (preCarga.Veiculo != null && preCarga.Veiculo?.ModeloVeicularCarga == null)
                        {
                            preCarga.Veiculo.ModeloVeicularCarga = modeloVeicularCarga;
                            repVeiculo.Atualizar(preCarga.Veiculo);

                            if (preCarga.Veiculo.VeiculosVinculados != null && preCarga.Veiculo.VeiculosVinculados.Count > 0)
                            {
                                foreach (Dominio.Entidades.Veiculo reboque in preCarga.Veiculo.VeiculosVinculados.ToList())
                                {
                                    if (reboque.ModeloVeicularCarga == null)
                                    {
                                        reboque.ModeloVeicularCarga = preCarga.ModeloVeicularCarga;
                                        repVeiculo.Atualizar(reboque);
                                    }
                                }
                            }
                        }
                    }
                }

                if (veiculoReboque != null)
                {
                    if (preCarga.VeiculosVinculados == null)
                        preCarga.VeiculosVinculados = new List<Dominio.Entidades.Veiculo>();
                    preCarga.VeiculosVinculados.Add(veiculoReboque);
                }

                preCarga.Empresa = empresa;
                preCarga.TipoDeCarga = tipoCarga;

                if (motorista != null)
                {
                    preCarga.Motoristas = new List<Dominio.Entidades.Usuario>();
                    preCarga.Motoristas.Add(motorista);
                }

                preCarga.Filial = filial;
                preCarga.SituacaoPreCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPreCarga.AguardandoGeracaoCarga;
                preCarga.NumeroPreCarga = numeroPreCarga;
                preCarga.HorarioCarregamentoInformadoNoPedido = horarioCarregamentoInformadoNoPedido;
                preCarga.DataPrevisaoInicioViagem = dataHoraPrevisaoInicioViagem;
                preCarga.DataCriacaoPreCarga = DateTime.Now;

                repPreCarga.Inserir(preCarga);

                servicoCargaDadosSumarizados.AlterarDadosSumarizadosPreCarga(preCarga, unitOfWork, tipoServicoMultisoftware);
            }

            return preCarga;
        }

        private string CancelarPedidosAgMontagem(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, List<Dominio.Entidades.Embarcador.PreCargas.PreCarga> preCargas, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.PreCargas.PreCarga repPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(unitOfWork);
            string retorno = "";
            for (int i = 0; i < preCargas.Count; i++)
            {
                try
                {
                    unitOfWork.Start(System.Data.IsolationLevel.ReadUncommitted);
                    int codigoPreCarga = preCargas[i].Codigo;
                    Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga = repPreCarga.BuscarPorCodigo(codigoPreCarga);

                    List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = repPedido.BuscarPorPreCarga(preCarga.Codigo);
                    foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidos)
                    {
                        pedido.SituacaoPedido = SituacaoPedido.Cancelado;
                        repPedido.Atualizar(pedido);
                    }

                    preCarga.SituacaoPreCarga = SituacaoPreCarga.Cancelada;
                    repPreCarga.Atualizar(preCarga);

                    unitOfWork.CommitChanges();
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    retorno += "Ocorreu uma falha ao cancelar os pedidos " + preCargas[i].NumeroPreCarga + ". ";
                    unitOfWork.Rollback();
                }

            }

            return retorno;
        }

        public void GerarCargasAguardandoGeracaoPreCarga(string StringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Pedido.Pedido serPedido = new Servicos.Embarcador.Pedido.Pedido(unitOfWork);
            Servicos.Global.OrquestradorFila servicoOrquestradorFila = new Servicos.Global.OrquestradorFila(unitOfWork, IdentificadorControlePosicaoThread.GerarCargasAguardandoGeracaoPreCarga);

            Repositorio.Embarcador.PreCargas.PreCarga repPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork).BuscarPrimeiroRegistro();

            if (!configuracaoTMS.ImportarPedidoDeixarCargaPendente)
                return;

            List<int> codigospreCargas = servicoOrquestradorFila.Ordenar((limiteRegistros) => repPreCarga.BuscarAguardandoGeracaoCarga(limiteRegistros));

            try
            {
                string retornoFinaliza = "";
                serPedido.FinalizarPreCargasAgMontagem(configuracaoTMS, codigospreCargas, null, tipoServicoMultisoftware, unitOfWork, out retornoFinaliza, out int qtdCargasGeradas, configuracaoGeralCarga, null, "", null, null);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        private Dominio.Entidades.Embarcador.Cargas.Carga FinalizarPreCargasAgMontagem(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, List<int> codigosPreCargas, List<string> cargasParaCancelamento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, out string retorno, out int quantidadeCargasGeradas, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga, Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica = null, string motivoCancelamentoCarga = "", Dominio.Entidades.Usuario usuario = null, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = null)
        {
            Servicos.Embarcador.PreCarga.PreCarga servicoPreCarga = new Servicos.Embarcador.PreCarga.PreCarga(unitOfWork);
            Servicos.Global.OrquestradorFila servicoOrquestradorFila = new Servicos.Global.OrquestradorFila(unitOfWork, IdentificadorControlePosicaoThread.GerarCargasAguardandoGeracaoPreCarga);

            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.PreCargas.PreCarga repPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(unitOfWork);

            retorno = "";
            quantidadeCargasGeradas = 0;
            Dominio.Entidades.Embarcador.Cargas.Carga cargaGerada = null;

            for (int i = 0; i < codigosPreCargas.Count; i++)
            {
                unitOfWork.Start();
                int codigoPreCarga = codigosPreCargas[i];
                Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga = repPreCarga.BuscarPorCodigo(codigoPreCarga);

                try
                {
                    string criaRetorno = "";

                    cargaGerada = servicoPreCarga.GerarCarga(preCarga, configuracaoTMS.UsarMesmoNumeroPreCargaGerarCargaViaImportacao, tipoServicoMultisoftware, null, out criaRetorno);
                    if (string.IsNullOrWhiteSpace(criaRetorno))
                    {
                        if (cargasParaCancelamento != null && cargasParaCancelamento.Contains(preCarga.NumeroPreCarga))
                        {
                            cargasParaCancelamento.Remove(preCarga.NumeroPreCarga);
                            Dominio.Entidades.Embarcador.Cargas.Carga carga = repPreCarga.BuscarPorNumeroPreCarga(preCarga.NumeroPreCarga)?.Carga;

                            if (carga != null)
                            {
                                if (!Servicos.Embarcador.Carga.Cancelamento.ValidarSePossivelCancelar(carga, operadorLogistica, tipoServicoMultisoftware, out string erroCancelamento, unitOfWork))
                                {
                                    unitOfWork.Rollback();
                                    retorno += "Não foi possível cancelar a carga da pré carga " + preCarga.NumeroPreCarga + " pelo seguinte motivo (" + erroCancelamento + "). ";
                                }

                                Servicos.Embarcador.Carga.Cancelamento.GeraRegistrosCancelamento(carga, motivoCancelamentoCarga, usuario, auditado, tipoServicoMultisoftware, unitOfWork);
                            }
                        }

                        if (cargaGerada != null)
                        {
                            Servicos.Embarcador.Carga.PedidoVinculado.CriarPedidoDeEncaixe(cargaGerada, unitOfWork, tipoServicoMultisoftware, configuracaoTMS, configuracaoGeralCarga);

                            if (auditado != null)
                                Auditoria.Auditoria.Auditar(auditado, cargaGerada, "Criou a carga via importação de pedidos", unitOfWork);

                            quantidadeCargasGeradas++;

                            cargaGerada.RotaRecorrente = true;
                            repCarga.Atualizar(cargaGerada);
                        }

                        unitOfWork.CommitChanges();
                    }
                    else
                    {
                        Servicos.Log.TratarErro("Não foi possível criar a carga da importação por: " + criaRetorno);
                        unitOfWork.Rollback();
                        retorno += "Não foi possível gerar a carga da pré carga " + preCarga.NumeroPreCarga + " pelo seguinte motivo (" + criaRetorno + "). ";
                    }

                    preCarga.AguardandoGeracaoCarga = false;
                    repPreCarga.Atualizar(preCarga);

                    servicoOrquestradorFila.RegistroLiberadoComSucesso(codigosPreCargas[i]);

                }
                catch (Exception excecao)
                {
                    unitOfWork.Rollback();
                    Servicos.Log.TratarErro(excecao);
                    retorno += "Ocorreu uma falha ao gerar a carga da pré carga " + preCarga.NumeroPreCarga + ". ";
                    servicoOrquestradorFila.RegistroComFalha(codigosPreCargas[i], excecao.Message);
                }

                unitOfWork.FlushAndClear();
            }

            return cargaGerada;
        }

        private Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha RetornarFalhaLinha(string mensagem, int indice)
        {
            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retorno = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = indice, mensagemFalha = mensagem, processou = false };
            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha RetornarFalhaLinha(string mensagem, bool contar = false)
        {
            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retorno = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { mensagemFalha = mensagem, processou = false, contar = contar };
            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha RetornarSucessoLinha(int codigo)
        {
            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retorno = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { codigo = codigo, mensagemFalha = "", processou = true, contar = true };
            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha ValidarAtualizacaoCargaLinha(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova && carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.NaLogistica)
            {
                return RetornarFalhaLinha($"Carga está na situação {carga.SituacaoCarga.ObterDescricao()} e não pode ser atualizada");
            }

            return null;
        }

        public bool EnviarRelatorioDetalhesPedidoPorEmail(List<string> emails, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.Entidades.Embarcador.Pessoas.RestricaoEntrega restricao, Repositorio.UnitOfWork unidadeTrabalho, out string mensagem)
        {
            Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo();

            if (email == null)
            {
                mensagem = "Não há um e-mail configurado para realizar o envio.";
                return false;
            }

            string assunto = "Restrição de entrega para o pedido " + pedido.NumeroPedidoEmbarcador + "";

            string mensagemEmail = "Olá,<br/><br/>Seguem os dados da restrição de entrega do pedido " + pedido.NumeroPedidoEmbarcador + ".<br/><br/>";

            mensagemEmail += "Cliente: " + pedido.Destinatario.Descricao + "<br/><br/>";

            if (restricao != null)
            {
                mensagemEmail += "Restrição: " + restricao.Descricao + "<br/><br/>";
                mensagemEmail += "Observação da restrição: " + restricao.Observacao + "<br/><br/>";
            }

            mensagemEmail += "E-mail enviado automaticamente. Por favor, não responda.";

            bool sucesso = Servicos.Email.EnviarEmail(email.Email, email.Email, email.Senha, null, emails.ToArray(), null, assunto, mensagemEmail, email.Smtp, out mensagem, email.DisplayEmail, null, "", email.RequerAutenticacaoSmtp, "", email.PortaSmtp, unidadeTrabalho);

            return sucesso;
        }

        private Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha AdicionarPedidoImportacao(Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoImportacaoSalvar pedidoImportacaoAdicionar, bool tomadorDefinidoPedido, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultiSoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            return SalvarPedidoImportacao(pedidoImportacaoAdicionar, false, tomadorDefinidoPedido, unitOfWork, configuracaoTMS, tipoServicoMultisoftware, clienteMultiSoftware, auditado);
        }

        private void ZerarValoresPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoComponenteFrete repPedidoComponenteFrete = new Repositorio.Embarcador.Pedidos.PedidoComponenteFrete(unitOfWork);
            pedido.ValorFreteFilialEmissora = 0;
            pedido.ValorFreteAReceber = 0;
            pedido.ValorFreteNegociado = 0;
            pedido.ValorFreteToneladaNegociado = 0;
            pedido.BaseCalculoICMS = 0;
            pedido.ValorICMS = 0;
            repPedidoComponenteFrete.DeletarPorPedido(pedido.Codigo);
        }

        private Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha SalvarPedidoImportacao(Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoImportacaoSalvar pedidoImportacaoSalvar, bool naoTentarGerarCarga, bool tomadorDefinidoPedido, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultiSoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            string retorno = "";

            Repositorio.Embarcador.Cargas.CargaPedidoPacote repCargaPedidoPacote = new Repositorio.Embarcador.Cargas.CargaPedidoPacote(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoEndereco repPedidoEndereco = new Repositorio.Embarcador.Pedidos.PedidoEndereco(unitOfWork);
            Repositorio.Embarcador.Pessoas.ClienteOutroEndereco repClienteOutroEndereco = new Repositorio.Embarcador.Pessoas.ClienteOutroEndereco(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoDeCarga repTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
            Repositorio.Embarcador.PreCargas.PreCarga repPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaDadosSumarizados repCargaSumarizado = new Repositorio.Embarcador.Cargas.CargaDadosSumarizados(unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoProduto repPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(unitOfWork);
            Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork);
            Repositorio.RotaFrete repRotaFrete = new Repositorio.RotaFrete(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.Embarcador.Pessoas.ClienteDescarga repClienteDescarga = new Repositorio.Embarcador.Pessoas.ClienteDescarga(unitOfWork);
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoComponenteFrete repPedidoComponenteFrete = new Repositorio.Embarcador.Pedidos.PedidoComponenteFrete(unitOfWork);
            Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unitOfWork);
            Repositorio.Embarcador.Logistica.AgendamentoColetaPedido repAgendamentoColetaPedido = new Repositorio.Embarcador.Logistica.AgendamentoColetaPedido(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega repositorioConfiguracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoEndereco repositorioPedidoEndereco = new Repositorio.Embarcador.Pedidos.PedidoEndereco(unitOfWork);
            Repositorio.Embarcador.Pessoas.ClienteOutroEndereco repositorioClienteOutroEndereco = new Repositorio.Embarcador.Pessoas.ClienteOutroEndereco(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoFronteira repositorioPedidoFronteira = new Repositorio.Embarcador.Pedidos.PedidoFronteira(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega = repositorioConfiguracaoControleEntrega.BuscarPrimeiroRegistro();
            Servicos.Embarcador.Pedido.OcorrenciaPedido servOcorrenciaPedido = new Servicos.Embarcador.Pedido.OcorrenciaPedido(unitOfWork);
            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = null;
            bool criandoNovoPedido = false;
            Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco pedidoEnderecoOrigem = new Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco();
            Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco pedidoEnderecoDestino = new Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco();

            Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repositorioConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = repositorioConfiguracaoPedido.BuscarConfiguracaoPadrao();


            if (pedidoImportacaoSalvar.PreCarga != null && configuracaoTMS.UsarMesmoNumeroPreCargaGerarCargaViaImportacao)
            {
                if (!string.IsNullOrWhiteSpace(pedidoImportacaoSalvar.NumeroPedido))
                {
                    pedido = repPedido.BuscarPorNumeroEmbarcador(pedidoImportacaoSalvar.NumeroPedido, pedidoImportacaoSalvar.Filial?.Codigo ?? 0, "", pedidoDePreCarga: false);
                }
                else
                {
                    pedido = repPedido.BuscarPorPreCargaRemetenteDestinatario(pedidoImportacaoSalvar.PreCarga.Codigo, pedidoImportacaoSalvar.Remetente.CPF_CNPJ, pedidoImportacaoSalvar.Destinatario.CPF_CNPJ);
                }
            }

            Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido agendamentoColetaPedido = repAgendamentoColetaPedido.BuscarPorPedido(pedidoImportacaoSalvar.NumeroPedido);
            if (agendamentoColetaPedido != null)
            {
                if (agendamentoColetaPedido.Pedido.SaldoVolumesRestante > 0)
                    agendamentoColetaPedido.Pedido.DataInicioJanelaDescarga = pedidoImportacaoSalvar.DataInicioJanelaDescarga;

                agendamentoColetaPedido.Pedido.DataValidade = pedidoImportacaoSalvar.DataValidade;
                repPedido.Atualizar(agendamentoColetaPedido.Pedido);

                return new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha
                {
                    codigo = agendamentoColetaPedido.Pedido.Codigo,
                    mensagemFalha = retorno
                };
            }

            if (configuracaoTMS.BuscarCargaPorNumeroPedido)
                pedido = repPedido.BuscarPorNumeroEmbarcador(pedidoImportacaoSalvar.NumeroPedido, pedidoImportacaoSalvar.Filial?.Codigo ?? 0, true);

            bool addProduto = true;

            if (pedido == null || configuracaoTMS.BuscarCargaPorNumeroPedido || configuracaoPedido.AtualizarCargaAoImportarPlanilha)
            {
                if (pedido == null)
                {
                    pedido = new Dominio.Entidades.Embarcador.Pedidos.Pedido();
                    criandoNovoPedido = true;
                }
                else if (configuracaoTMS.BuscarCargaPorNumeroPedido)
                    ZerarValoresPedido(pedido, unitOfWork);

                pedido.Numero = repPedido.BuscarProximoNumero();
                pedido.CodigoPedidoCliente = "";
                pedido.DataFinalColeta = DateTime.Now;
                pedido.DataInicialColeta = DateTime.Now;
                pedido.DataPrevisaoChegadaDestinatario = pedidoImportacaoSalvar.DataHoraDescarga;
                pedido.Adicional1 = pedidoImportacaoSalvar.Adicional1;
                pedido.Adicional2 = pedidoImportacaoSalvar.Adicional2;
                pedido.Adicional3 = pedidoImportacaoSalvar.Adicional3;
                pedido.Adicional4 = pedidoImportacaoSalvar.Adicional4;
                pedido.Adicional5 = pedidoImportacaoSalvar.Adicional5;
                pedido.Adicional6 = pedidoImportacaoSalvar.Adicional6;
                pedido.Adicional7 = pedidoImportacaoSalvar.Adicional7;
                pedido.NumeroCargaEncaixar = pedidoImportacaoSalvar.NumeroCargaEncaixar;
                pedido.NumeroControle = pedidoImportacaoSalvar.NumeroControle;
                pedido.TipoEmbarque = pedidoImportacaoSalvar.TipoEmbarque;
                pedido.Observacao = pedidoImportacaoSalvar.Observacao;
                pedido.OrdemColetaProgramada = pedidoImportacaoSalvar.OrdemColeta;
                pedido.QuebraMultiplosCarregamentos = pedidoImportacaoSalvar.QuebraMultiplosCarregamentos;
                pedido.Deposito = pedidoImportacaoSalvar.Deposito;
                pedido.QtVolumes = pedidoImportacaoSalvar.QtdVolumes;
                pedido.SaldoVolumesRestante = pedidoImportacaoSalvar.QtdVolumes;
                pedido.ValorTotalNotasFiscais = pedidoImportacaoSalvar.ValorTotalPedido;
                pedido.NumeroNotaCliente = pedidoImportacaoSalvar.NumeroNotaCliente;
                pedido.ObservacaoAdicional = pedidoImportacaoSalvar.ObservacaoAdicional ?? string.Empty;
                pedido.NumeroOrdem = pedidoImportacaoSalvar.NumeroOrdem;
                pedido.DataAlocacaoPedido = pedidoImportacaoSalvar.DataAlocacaoPedido;
                pedido.PossuiIsca = pedidoImportacaoSalvar.PossuiIsca;
                pedido.PossuiEtiquetagem = pedidoImportacaoSalvar.PossuiEtiquetagem;
                pedido.Destino = pedidoImportacaoSalvar.Cidade;
                pedido.GrossSales = pedidoImportacaoSalvar.GrossSales;
                pedido.SituacaoAgendamentoEntregaPedido = (pedidoImportacaoSalvar.Destinatario?.ExigeQueEntregasSejamAgendadas ?? false) ? pedidoImportacaoSalvar.SituacaoAgendamentoEntregaPedido : SituacaoAgendamentoEntregaPedido.NaoExigeAgendamento;
                pedido.QuantidadeVolumesPrevios = pedidoImportacaoSalvar.QuantidadeVolumesPrevios;
                pedido.CodigoPedidoCliente = pedidoImportacaoSalvar.NumeroPedidoCliente;
                pedido.ValorTotalNotasFiscais = pedidoImportacaoSalvar.ValorTotalNotasFiscais;
                pedido.ObservacaoInterna = pedidoImportacaoSalvar.ObservacaoInterna ?? string.Empty;
                pedido.Veiculos = new List<Dominio.Entidades.Veiculo>();
                pedido.NumeroDoca = pedidoImportacaoSalvar.NumeroDoca;
                pedido.TipoPagamento = pedidoImportacaoSalvar.TipoPagamento;
                pedido.DataCarregamentoPedido = pedidoImportacaoSalvar.DataHoraPrevisaoColeta;
                pedido.PedidoImportadoPorPlanilha = pedidoImportacaoSalvar.PedidoImportadoPorPlanilha;
                pedido.DataPrevisaoTerminoCarregamento = pedidoImportacaoSalvar.DataPrevisaoTerminoCarregamento;
                pedido.DevolucaoPacote = pedidoImportacaoSalvar.DevolucaoPacotes;
                pedido.ContratoFreteCliente = pedidoImportacaoSalvar.ContratoFreteCliente;
                pedido.Origem = pedidoImportacaoSalvar.Origem;
                pedido.Ajudante = pedidoImportacaoSalvar.Ajudante;
                pedido.QtdAjudantes = pedidoImportacaoSalvar.QuantidadeAjudantes;
                pedido.Rota = pedidoImportacaoSalvar.Rota;
                pedido.Facility = pedidoImportacaoSalvar.Facility;

                if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    if (pedidoImportacaoSalvar.Motoristas?.Count > 0)
                        pedido.Motoristas = pedidoImportacaoSalvar.Motoristas.ToList();

                    if (pedidoImportacaoSalvar.Veiculo != null)
                        pedido.Veiculos.Add(pedidoImportacaoSalvar.Veiculo);
                }

                if (pedidoImportacaoSalvar.DataHoraPrevisaoSaida.HasValue && (pedidoImportacaoSalvar.DataHoraPrevisaoSaida.Value != DateTime.MinValue))
                    pedido.DataPrevisaoSaida = pedidoImportacaoSalvar.DataHoraPrevisaoSaida.Value;

                if (pedidoImportacaoSalvar.DataCriacaoPedidoERP != DateTime.MinValue)
                    pedido.DataCriacaoPedidoERP = pedidoImportacaoSalvar.DataCriacaoPedidoERP;

                if (pedidoImportacaoSalvar.Remetente == null && pedidoImportacaoSalvar.GrupoPessoasRemetente != null)
                {
                    pedido.TipoPessoa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa.GrupoPessoa;
                    pedido.GrupoPessoas = pedidoImportacaoSalvar.GrupoPessoasRemetente;
                }

                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    if (pedidoImportacaoSalvar.CentroResultado != null)
                        pedido.CentroResultado = pedidoImportacaoSalvar.CentroResultado;
                    else if (pedidoImportacaoSalvar.TipoOperacao != null)
                        pedido.CentroResultado = repCentroResultado.BuscarPorTipoOperacao(pedidoImportacaoSalvar.TipoOperacao);
                }

                if (pedidoImportacaoSalvar.DataHoraCarregamento != DateTime.MinValue)
                    pedido.DataCarregamentoPedido = pedidoImportacaoSalvar.DataHoraCarregamento;
                else
                    pedido.DataCarregamentoPedido = DateTime.Now;

                if (pedidoImportacaoSalvar.DataValidade != DateTime.MinValue)
                    pedido.DataValidade = pedidoImportacaoSalvar.DataValidade;

                if (pedidoImportacaoSalvar.DataInicioJanelaDescarga != DateTime.MinValue)
                    pedido.DataInicioJanelaDescarga = pedidoImportacaoSalvar.DataInicioJanelaDescarga;

                if (configuracaoControleEntrega.UtilizarPrevisaoEntregaPedidoComoDataPrevista)
                {
                    if (pedidoImportacaoSalvar.DataHoraPrevisaoColeta != DateTime.MinValue)
                        pedido.DataCarregamentoPedido = pedidoImportacaoSalvar.DataHoraPrevisaoColeta;
                }

                pedido.ProdutoPrincipal = pedidoImportacaoSalvar.ProdutoPrincipal;

                pedido.Remetente = pedidoImportacaoSalvar.Remetente;
                pedido.Expedidor = pedidoImportacaoSalvar.Expedidor;
                pedido.Recebedor = pedidoImportacaoSalvar.Recebedor;
                pedido.Destinatario = pedidoImportacaoSalvar.Destinatario;
                pedido.LocalExpedicao = pedidoImportacaoSalvar.LocalExpedicao;

                if ((pedido?.Destinatario?.GerarPedidoBloqueado ?? false) || configuracaoPedido.BloquearPedidoAoIntegrar)
                    pedido.PedidoBloqueado = true;

                if (pedido.Destino == null && pedido.Recebedor != null && pedido.Recebedor.Localidade != null)
                    pedido.Destino = pedido.Recebedor.Localidade;
                else if (pedido.Destino == null && pedido.Destinatario != null && pedido.Destinatario.Localidade != null)
                    pedido.Destino = pedido.Destinatario.Localidade;

                if (pedidoImportacaoSalvar.DataPrevEntrega != DateTime.MinValue)
                    pedido.PrevisaoEntrega = pedidoImportacaoSalvar.DataPrevEntrega;

                if (pedido.Remetente != null)
                {
                    pedido.GrupoPessoas = pedidoImportacaoSalvar.Remetente.GrupoPessoas;
                    pedido.Origem = pedidoImportacaoSalvar.Remetente.Localidade;
                }

                pedido.Empresa = pedidoImportacaoSalvar.Empresa;

                if (pedido.Remetente != null)
                    PreecherEnderecoPedido(ref pedidoEnderecoOrigem, pedido.Remetente);

                if (pedido.Destinatario != null)
                    PreecherEnderecoPedido(ref pedidoEnderecoDestino, pedido.Destinatario);

                if (pedidoEnderecoOrigem.Localidade != null)
                    repPedidoEndereco.Inserir(pedidoEnderecoOrigem);
                if (pedidoEnderecoDestino.Localidade != null)
                    repPedidoEndereco.Inserir(pedidoEnderecoDestino);

                if (pedidoEnderecoOrigem.Localidade != null)
                {
                    pedido.Origem = pedidoEnderecoOrigem.Localidade;
                    pedido.EnderecoOrigem = pedidoEnderecoOrigem;
                }

                if (pedidoEnderecoDestino.Localidade != null)
                {
                    pedido.Destino = pedidoEnderecoDestino.Localidade;
                    pedido.EnderecoDestino = pedidoEnderecoDestino;
                }

                pedido.QtdEntregas = 1;
                pedido.PedidoTransbordo = false;
                pedido.UsarOutroEnderecoOrigem = false;
                pedido.UsarOutroEnderecoDestino = false;

                if (pedido.Destinatario != null)
                {
                    Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga clienteDescarga = repClienteDescarga.BuscarPorPessoa(pedido.Destinatario.CPF_CNPJ);
                    if (clienteDescarga != null && clienteDescarga.RestricoesDescarga != null)
                    {
                        pedido.RestricoesDescarga = clienteDescarga.RestricoesDescarga.ToList();

                        foreach (Dominio.Entidades.Embarcador.Pessoas.RestricaoEntrega restricao in pedido.RestricoesDescarga)
                        {
                            string email = restricao.Email ?? "";
                            if (!string.IsNullOrWhiteSpace(email))
                            {
                                List<string> emails = email.Split(';').ToList();
                                bool sucesso = EnviarRelatorioDetalhesPedidoPorEmail(emails, pedido, restricao, unitOfWork, out string mensagem);
                                if (!sucesso)
                                    Servicos.Log.TratarErro(mensagem, "EmailRestricao");
                            }
                        }
                    }

                }

                pedido.RotaFrete = pedidoImportacaoSalvar.RotaFrete;

                if (pedido.RotaFrete == null && pedido.Empresa != null && pedido.Origem != null && pedido.Destino != null)
                {
                    Repositorio.RotaFrete repositorioRotaFrete = new RotaFrete(unitOfWork);
                    Dominio.Entidades.RotaFrete rotaFrete = repositorioRotaFrete.BuscarPorOrigemDestinoTipoOperacaoTransportador(pedido.Origem.Codigo, pedido.Destino.Codigo, pedidoImportacaoSalvar.TipoOperacao?.Codigo ?? 0, pedido.Empresa?.Codigo ?? 0);
                    pedido.RotaFrete = rotaFrete;
                }

                if (pedido.RotaFrete == null && pedido.Destino != null)
                {
                    pedido.RotaFrete = repRotaFrete.BuscarPorLocalidade(pedido.Destino, true);

                    if (pedido.RotaFrete == null)
                        pedido.RotaFrete = repRotaFrete.BuscarPorEstado(pedido.Destino.Estado.Sigla, true);
                }

                Dominio.Entidades.Cliente tomador = pedido.ObterTomador();

                if (!string.IsNullOrWhiteSpace(pedidoImportacaoSalvar.TipoCarga?.ProdutoPredominante))
                    pedido.ProdutoPredominante = pedidoImportacaoSalvar.TipoCarga.ProdutoPredominante;
                else if (!string.IsNullOrWhiteSpace(pedidoImportacaoSalvar.TipoOperacao?.ProdutoPredominanteOperacao))
                    pedido.ProdutoPredominante = pedidoImportacaoSalvar.TipoOperacao.ProdutoPredominanteOperacao;
                else if (tomador?.GrupoPessoas != null && !string.IsNullOrWhiteSpace(tomador.GrupoPessoas.ProdutoPredominante))
                    pedido.ProdutoPredominante = tomador.GrupoPessoas.ProdutoPredominante;
                else if (!string.IsNullOrWhiteSpace(configuracaoTMS.DescricaoProdutoPredominatePadrao))
                    pedido.ProdutoPredominante = configuracaoTMS.DescricaoProdutoPredominatePadrao;
                else
                    pedido.ProdutoPredominante = "Importação";

                if (pedidoImportacaoSalvar.Filial != null)
                    pedido.Filial = pedidoImportacaoSalvar.Filial;

                pedido.AdicionadaManualmente = true;
                pedido.NumeroPaletesFracionado = pedidoImportacaoSalvar.QuantidadePalletsFacionada;
                pedido.PalletSaldoRestante = pedido.TotalPallets;
                if (string.IsNullOrEmpty(pedidoImportacaoSalvar.NumeroPedido))
                {
                    if (configuracaoTMS.NumeroCargaSequencialUnico)
                        pedido.NumeroSequenciaPedido = repPedido.ObterProximoCodigo();
                    else
                        pedido.NumeroSequenciaPedido = repPedido.ObterProximoCodigo(pedido.Filial);

                    pedido.NumeroPedidoEmbarcador = pedido.NumeroSequenciaPedido.ToString();
                }
                else
                    pedido.NumeroPedidoEmbarcador = pedidoImportacaoSalvar.NumeroPedido;

                if (pedidoImportacaoSalvar.PesoPedido != 0)
                    pedido.PesoTotal = pedidoImportacaoSalvar.PesoPedido;

                pedido.PesoSaldoRestante = pedidoImportacaoSalvar.PesoPedido;
                pedido.CubagemTotal = pedidoImportacaoSalvar.CubagemPedido;

                pedido.PreCarga = pedidoImportacaoSalvar.PreCarga;
                pedido.ObservacaoCTe = configuracaoTMS.ObservacaoCTePadraoEmbarcador ?? "";
                pedido.Temperatura = pedidoImportacaoSalvar.Temperatura;


                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && !naoTentarGerarCarga)
                {
                    if (pedido.TipoOperacao != null && !pedido.TipoOperacao.GeraCargaAutomaticamente)
                    {
                        pedido.GerarAutomaticamenteCargaDoPedido = false;
                        pedido.PedidoTotalmenteCarregado = false;
                    }
                    else
                        pedido.GerarAutomaticamenteCargaDoPedido = true;

                    pedido.PedidoIntegradoEmbarcador = !configuracaoTMS.UtilizarIntegracaoPedido;

                    PreencherCodigoCargaEmbarcador(pedido, configuracaoTMS, unitOfWork);
                }
                else
                {
                    pedido.PedidoIntegradoEmbarcador = false;
                    pedido.GerarAutomaticamenteCargaDoPedido = false;
                }

                pedido.Requisitante = Dominio.ObjetosDeValor.Embarcador.Enumeradores.RequisitanteColeta.Remetente;
                pedido.SituacaoPedido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Aberto;

                pedido.TipoDeCarga = pedidoImportacaoSalvar.TipoCarga;
                pedido.ModeloVeicularCarga = pedidoImportacaoSalvar.ModeloVeicularCarga;
                pedido.CanalEntrega = pedidoImportacaoSalvar.CanalEntrega;

                pedido.TipoOperacao = pedidoImportacaoSalvar.TipoOperacao;

                if ((pedido.TipoOperacao?.UsarConfiguracaoEmissao ?? false) && !string.IsNullOrWhiteSpace(pedido.TipoOperacao.ObservacaoCTe))
                    pedido.ObservacaoCTe += string.Concat(" ", pedido.TipoOperacao.ObservacaoCTe);

                pedido.TipoTomador = pedidoImportacaoSalvar.TipoTomador;
                pedido.UsarTipoTomadorPedido = tomadorDefinidoPedido;
                if (pedidoImportacaoSalvar.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario)
                    pedido.TipoPagamento = Dominio.Enumeradores.TipoPagamento.A_Pagar;
                else
                    pedido.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Pago;
                pedido.UltimaAtualizacao = DateTime.Now;
                pedido.Usuario = pedidoImportacaoSalvar.Usuario;
                pedido.Autor = pedidoImportacaoSalvar.Usuario;

                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    pedido.DataInicialViagemExecutada = pedido.DataPrevisaoSaida;
                    pedido.DataFinalViagemExecutada = pedido.PrevisaoEntrega;
                    pedido.DataInicialViagemFaturada = pedido.DataPrevisaoSaida;
                    pedido.DataFinalViagemFaturada = pedido.PrevisaoEntrega;
                }

                if (pedidoImportacaoSalvar.Container != null && (pedidoImportacaoSalvar.DataColetaContainer.HasValue && pedidoImportacaoSalvar.DataColetaContainer.Value != DateTime.MinValue))
                {
                    pedido.Container = pedidoImportacaoSalvar.Container;
                    pedido.DataColetaContainer = pedidoImportacaoSalvar.DataColetaContainer.Value;
                }

                if (pedidoImportacaoSalvar.PortoViagemOrigem != null)
                {
                    pedido.CodigoPortoOrigem = pedidoImportacaoSalvar.PortoViagemOrigem?.CodigoIntegracao ?? string.Empty;
                    pedido.DescricaoPortoOrigem = pedidoImportacaoSalvar.PortoViagemOrigem?.Descricao ?? string.Empty;
                    pedido.PaisPortoOrigem = pedidoImportacaoSalvar.PortoViagemOrigem?.Localidade?.Pais?.Nome ?? string.Empty;
                    pedido.SiglaPaisPortoOrigem = pedidoImportacaoSalvar.PortoViagemOrigem?.Localidade?.Pais?.Sigla ?? string.Empty;
                }

                if (pedidoImportacaoSalvar.PortoViagemDestino != null)
                {
                    pedido.CodigoPortoDestino = pedidoImportacaoSalvar.PortoViagemDestino?.CodigoIntegracao ?? string.Empty;
                    pedido.DescricaoPortoDestino = pedidoImportacaoSalvar.PortoViagemDestino?.Nome ?? string.Empty;
                    pedido.PaisPortoDestino = pedidoImportacaoSalvar.PortoViagemDestino?.Localidade?.Pais?.Nome ?? string.Empty;
                    pedido.SiglaPaisPortoDestino = pedidoImportacaoSalvar.PortoViagemDestino?.Localidade?.Pais?.Sigla ?? string.Empty;
                }

                if (pedidoImportacaoSalvar.PaisDestino != null)
                {
                    pedido.PaisPortoDestino = pedidoImportacaoSalvar.PaisDestino.Nome;
                    pedido.SiglaPaisPortoDestino = pedidoImportacaoSalvar.PaisDestino.Sigla;
                }

                pedido.NumeroEXP = pedidoImportacaoSalvar.NumeroEXP;

                if (pedidoImportacaoSalvar.NotaMercadoLivre != null)
                    pedido.NotaMercadoLivre = pedidoImportacaoSalvar.NotaMercadoLivre;

                if (pedidoImportacaoSalvar.SiglaFaturamentoMercadoLivre != null)
                    pedido.SiglaFaturamentoMercadoLivre = pedidoImportacaoSalvar.SiglaFaturamentoMercadoLivre;

                if (pedidoImportacaoSalvar.PFMercadoLivre != null)
                    pedido.PFMercadoLivre = pedidoImportacaoSalvar.PFMercadoLivre;

                if (pedidoImportacaoSalvar.ItemFaturadoMercadoLivre != null)
                    pedidoImportacaoSalvar.ItemFaturadoMercadoLivre = "I";

                if (pedidoImportacaoSalvar.Despachante != null)
                {
                    pedido.CodigoDespachante = pedidoImportacaoSalvar.Despachante.CodigoIntegracao;
                    pedido.DescricaoDespachante = pedidoImportacaoSalvar.Despachante.Nome;
                }

                if (pedidoImportacaoSalvar.ClienteFinal != null)
                    pedido.ClienteAdicional = pedidoImportacaoSalvar.ClienteFinal;

                if (pedidoImportacaoSalvar.ViaTransporte != null)
                    pedido.ViaTransporte = pedidoImportacaoSalvar.ViaTransporte;

                if (pedidoImportacaoSalvar.LocalidadePedidoDestino != null)
                {
                    pedidoEnderecoDestino.Localidade = pedidoImportacaoSalvar.LocalidadePedidoDestino;
                    pedido.UsarOutroEnderecoDestino = true;

                    Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco outroEndereco = null;
                    outroEndereco = repositorioClienteOutroEndereco.BuscarPorCEPNumeroLocalidade(pedidoImportacaoSalvar.CEPPedidoDestino, pedidoImportacaoSalvar.NumeroPedidoDestino, pedidoEnderecoDestino.Localidade.Codigo, pedidoImportacaoSalvar.Destinatario.Codigo);

                    if (outroEndereco == null)
                        outroEndereco = new Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco();

                    outroEndereco.Bairro = pedidoImportacaoSalvar.BairroPedidoDestino ?? string.Empty;
                    outroEndereco.CEP = pedidoImportacaoSalvar.CEPPedidoDestino ?? string.Empty;
                    outroEndereco.Cliente = pedidoImportacaoSalvar.Destinatario;
                    outroEndereco.Complemento = pedidoImportacaoSalvar.CEPPedidoDestino ?? string.Empty;
                    outroEndereco.Endereco = pedidoImportacaoSalvar.EnderecoPedidoDestino ?? string.Empty;
                    outroEndereco.IE_RG = pedidoImportacaoSalvar.IEPedidoDestino ?? string.Empty;
                    outroEndereco.Numero = pedidoImportacaoSalvar.NumeroPedidoDestino ?? string.Empty;
                    outroEndereco.Telefone = pedidoImportacaoSalvar.TelefonePedidoDestino ?? string.Empty;
                    outroEndereco.TipoEndereco = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEndereco.Outros;
                    outroEndereco.TipoLogradouro = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogradouro.Outros;
                    outroEndereco.GeoLocalizacaoStatus = Dominio.ObjetosDeValor.Embarcador.Enumeradores.GeoLocalizacaoStatus.NaoGerado;
                    outroEndereco.Localidade = pedidoImportacaoSalvar.LocalidadePedidoDestino;

                    if (outroEndereco.Codigo > 0)
                        repClienteOutroEndereco.Atualizar(outroEndereco);
                    else
                        repClienteOutroEndereco.Inserir(outroEndereco);

                    pedidoEnderecoDestino.ClienteOutroEndereco = outroEndereco;
                }

                if (!string.IsNullOrEmpty(pedidoImportacaoSalvar.EnderecoPedidoDestino) && pedido.EnderecoDestino != null)
                    pedido.EnderecoDestino.Endereco = pedidoImportacaoSalvar.EnderecoPedidoDestino;

                if (!string.IsNullOrEmpty(pedidoImportacaoSalvar.NumeroPedidoDestino) && pedidoImportacaoSalvar.NumeroPedidoDestino != "S/N" && pedido.EnderecoDestino != null)
                    pedido.EnderecoDestino.Numero = pedidoImportacaoSalvar.NumeroPedidoDestino;

                if (!string.IsNullOrEmpty(pedidoImportacaoSalvar.ComplementoPedidoDestino) && pedido.EnderecoDestino != null)
                    pedido.EnderecoDestino.Complemento = pedidoImportacaoSalvar.ComplementoPedidoDestino;

                if (!string.IsNullOrEmpty(pedidoImportacaoSalvar.CEPPedidoDestino) && pedido.EnderecoDestino != null)
                    pedido.EnderecoDestino.CEP = pedidoImportacaoSalvar.CEPPedidoDestino;

                if (!string.IsNullOrEmpty(pedidoImportacaoSalvar.BairroPedidoDestino) && pedido.EnderecoDestino != null)
                    pedido.EnderecoDestino.Bairro = pedidoImportacaoSalvar.BairroPedidoDestino;

                if (!string.IsNullOrEmpty(pedidoImportacaoSalvar.TelefonePedidoDestino) && pedido.EnderecoDestino != null)
                    pedido.EnderecoDestino.Telefone = pedidoImportacaoSalvar.TelefonePedidoDestino;

                if (!string.IsNullOrEmpty(pedidoImportacaoSalvar.IEPedidoDestino) && pedido.EnderecoDestino != null)
                    pedido.EnderecoDestino.IE_RG = pedidoImportacaoSalvar.IEPedidoDestino;

                if (pedidoImportacaoSalvar.DistanciaCarga != 0)
                    pedido.Distancia = pedidoImportacaoSalvar.DistanciaCarga;

                if (pedidoImportacaoSalvar.CentroCarregamento != null)
                    pedido.CentroCarregamento = pedidoImportacaoSalvar.CentroCarregamento;

                if (pedidoImportacaoSalvar.CentroDeCustoViagem != null)
                    pedido.CentroDeCustoViagem = pedidoImportacaoSalvar.CentroDeCustoViagem;

                if (pedidoImportacaoSalvar.CodigoEnderecoSecundario != null)
                {
                    Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco localidadeEnderecoSecundario = pedidoImportacaoSalvar.CodigoEnderecoSecundario;

                    pedidoEnderecoDestino.ClienteOutroEndereco = localidadeEnderecoSecundario;
                    pedidoEnderecoDestino.Localidade = localidadeEnderecoSecundario.Localidade;
                    pedidoEnderecoDestino.Endereco = localidadeEnderecoSecundario.Endereco;
                    pedidoEnderecoDestino.Numero = localidadeEnderecoSecundario.Numero;
                    pedidoEnderecoDestino.Complemento = localidadeEnderecoSecundario.Complemento;
                    pedidoEnderecoDestino.Bairro = localidadeEnderecoSecundario.Bairro;
                    pedidoEnderecoDestino.CEP = localidadeEnderecoSecundario.CEP;
                    pedidoEnderecoDestino.Telefone = localidadeEnderecoSecundario.Telefone;

                    repositorioPedidoEndereco.Atualizar(pedidoEnderecoDestino);

                    pedido.UsarOutroEnderecoDestino = true;
                }

                if (!string.IsNullOrWhiteSpace(pedidoImportacaoSalvar.NumeroEXP) && !string.IsNullOrWhiteSpace(pedido.NumeroPedidoEmbarcador))
                {
                    Repositorio.Embarcador.Pedidos.PedidoDadosTransporteMaritimo repPedidoDadosTransporteMaritimo = new Repositorio.Embarcador.Pedidos.PedidoDadosTransporteMaritimo(unitOfWork);
                    Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimo bookingOriginal = repPedidoDadosTransporteMaritimo.BuscarPorPedidoENumeroEXP(pedido.NumeroPedidoEmbarcador, pedidoImportacaoSalvar.NumeroEXP);

                    if (bookingOriginal != null)
                    {
                        Servicos.Embarcador.Integracao.Marfrig.IntegracaoPedidoDadosTransporteMaritimo servIntegracaoPedidoDadosTransporteMaritimo = new Servicos.Embarcador.Integracao.Marfrig.IntegracaoPedidoDadosTransporteMaritimo(unitOfWork);
                        Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimo bookingEditar = bookingOriginal.Clonar();

                        bookingEditar.Initialize();

                        bookingEditar.NumeroEXP = pedidoImportacaoSalvar.NumeroEXP;
                        bookingEditar.Despachante = pedidoImportacaoSalvar.Despachante;
                        bookingEditar.PortoOrigem = pedidoImportacaoSalvar.PortoViagemOrigem;
                        bookingEditar.PortoDestino = pedidoImportacaoSalvar.PortoViagemDestino;
                        bookingEditar.Temperatura = pedidoImportacaoSalvar.Temperatura;
                        bookingEditar.ViaTransporte = pedidoImportacaoSalvar.ViaTransporte;
                        bookingEditar.FretePrepaid = pedidoImportacaoSalvar.FretePrepaid;

                        if (bookingEditar.IsChanged())
                        {
                            bookingEditar.CodigoOriginal = bookingOriginal.Codigo;
                            bookingEditar.BookingTemporario = true;

                            repPedidoDadosTransporteMaritimo.Inserir(bookingEditar);
                            servIntegracaoPedidoDadosTransporteMaritimo.AdicionarPedidoDadosTransporteMaritimoIntegracao(bookingEditar);
                        }
                    }
                }

                if (pedidoImportacaoSalvar.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros)
                    pedido.Tomador = pedidoImportacaoSalvar.Tomador;

                Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoExiste = null;
                if (!string.IsNullOrWhiteSpace(pedido.NumeroPedidoEmbarcador) && pedido.Filial != null)
                {
                    pedidoExiste = repPedido.BuscarPorNumeroEmbarcador(pedido.NumeroPedidoEmbarcador, pedido.Filial.Codigo, true);
                }

                if (pedidoExiste == null)
                {
                    pedido.NomeArquivoGerador = pedidoImportacaoSalvar.NomeArquivoGerador;
                    pedido.GuidArquivoGerador = pedidoImportacaoSalvar.GuidArquivoGerador;
                    pedido.PedidoIntegradoEmbarcador = true;
                    pedido.SituacaoAcompanhamentoPedido = SituacaoAcompanhamentoPedido.AgColeta;
                    repPedido.Inserir(pedido);

                    pedido.Protocolo = pedido.Codigo;
                    //repPedido.Atualizar(pedido);

                    if (TomadorPossuiPendenciaFinanceira(pedido, tipoServicoMultisoftware, out string mensagemErro))
                    {
                        return new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha
                        {
                            codigo = pedido.Codigo,
                            mensagemFalha = mensagemErro
                        };
                    }

                    Servicos.Auditoria.Auditoria.Auditar(auditado, pedido, null, "Criou Pedido via importação", unitOfWork);
                }
                else if (!configuracaoTMS.BuscarCargaPorNumeroPedido && !configuracaoPedido.AtualizarCargaAoImportarPlanilha)
                {
                    pedido = pedidoExiste;
                    if (pedidoImportacaoSalvar.PedidoProduto != null)
                    {
                        if (pedidoExiste?.Produtos?.Count > 0)
                        {
                            Dominio.Entidades.Embarcador.Pedidos.PedidoProduto item;

                            if (string.IsNullOrEmpty(pedidoImportacaoSalvar.Canal))
                                item = pedidoExiste.Produtos.Where(p => p.Produto.CodigoProdutoEmbarcador == pedidoImportacaoSalvar.PedidoProduto.Produto.CodigoProdutoEmbarcador).FirstOrDefault();
                            else
                                item = pedidoExiste.Produtos.Where(p => p.Produto.CodigoProdutoEmbarcador == pedidoImportacaoSalvar.PedidoProduto.Produto.CodigoProdutoEmbarcador && p.Canal == pedidoImportacaoSalvar.PedidoProduto.Canal).FirstOrDefault();

                            if (item != null)
                            {
                                addProduto = false;
                                retorno = "O produto informado (" + pedidoImportacaoSalvar.PedidoProduto.Produto.CodigoProdutoEmbarcador + ") já existe no pedido " + pedidoExiste.NumeroPedidoEmbarcador + ".";
                            }
                        }
                    }
                    else
                        retorno = "O pedido informado (" + pedidoExiste.NumeroPedidoEmbarcador + ") já existe " + (pedido.Filial != null ? " para a filial " + pedido.Filial.Descricao : ".");
                }

                if (pedidoExiste != null)
                    Servicos.Auditoria.Auditoria.Auditar(auditado, pedido, null, "Atualizou Pedido via importação", unitOfWork);
            }
            else
            {
                pedido.PreCarga = pedidoImportacaoSalvar.PreCarga;
            }

            if (pedido.PreCarga != null)
            {
                pedido.PreCarga.TipoOperacao = pedidoImportacaoSalvar.TipoOperacao;
                pedido.PreCarga.DataPrevisaoInicioViagem = pedidoImportacaoSalvar.DataHoraPrevisaoInicioViagem;
                pedido.PreCarga.FaixaTemperatura = pedidoImportacaoSalvar.FaixaTemperatura;

                repPreCarga.Atualizar(pedido.PreCarga);

                if (pedido.PreCarga.Carga != null)
                {
                    pedido.PreCarga.Carga.DataInicioViagemPrevista = pedidoImportacaoSalvar.DataHoraPrevisaoInicioViagem;

                    if (pedido.PreCarga.Carga.DadosSumarizados != null)
                    {
                        pedido.PreCarga.Carga.DadosSumarizados.TiposDeOperacao = pedidoImportacaoSalvar.TipoOperacao.Descricao;
                        repCargaSumarizado.Atualizar(pedido.PreCarga.Carga.DadosSumarizados);
                    }

                    Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retornoValidacao = ValidarAtualizacaoCargaLinha(pedido.PreCarga.Carga);

                    if (retornoValidacao != null)
                        return retornoValidacao;

                    repCarga.Atualizar(pedido.PreCarga.Carga);
                    Servicos.Auditoria.Auditoria.Auditar(auditado, pedido.PreCarga.Carga, null, "Carga atualizada via importação de planilha", unitOfWork);
                }
            }

            if (string.IsNullOrWhiteSpace(retorno))
            {
                if (addProduto && pedidoImportacaoSalvar.PedidoProduto != null)
                {
                    if (addProduto)
                    {
                        pedidoImportacaoSalvar.PedidoProduto.Pedido = pedido;
                        repPedidoProduto.Inserir(pedidoImportacaoSalvar.PedidoProduto);
                        pedido.PesoTotal += pedidoImportacaoSalvar.PedidoProduto.PesoUnitario * (pedidoImportacaoSalvar.PedidoProduto.Quantidade == 0 ? 1 : pedidoImportacaoSalvar.PedidoProduto.Quantidade);
                        pedido.PesoSaldoRestante += pedidoImportacaoSalvar.PedidoProduto.PesoUnitario * (pedidoImportacaoSalvar.PedidoProduto.Quantidade == 0 ? 1 : pedidoImportacaoSalvar.PedidoProduto.Quantidade);
                    }
                }

                if (pedidoImportacaoSalvar.FreteValor != null)
                {
                    pedido.ValorFreteFilialEmissora += pedidoImportacaoSalvar.FreteValorFilialEmissora?.FreteProprio ?? 0;

                    pedido.ValorFreteAReceber += pedidoImportacaoSalvar.FreteValor.ValorTotalAReceber;
                    pedido.ValorFreteNegociado += pedidoImportacaoSalvar.FreteValor.FreteProprio;
                    if (pedidoImportacaoSalvar.FreteValor.ICMS != null)
                    {
                        pedido.PercentualInclusaoBC = pedidoImportacaoSalvar.FreteValor.ICMS.PercentualInclusaoBC;
                        pedido.PercentualAliquota = pedidoImportacaoSalvar.FreteValor.ICMS.Aliquota;
                        pedido.BaseCalculoICMS += pedidoImportacaoSalvar.FreteValor.ICMS.ValorBaseCalculoICMS;
                        pedido.ValorICMS += pedidoImportacaoSalvar.FreteValor.ICMS.ValorICMS;
                        pedido.IncluirBaseCalculoICMS = pedidoImportacaoSalvar.FreteValor.ICMS.IncluirICMSBC;
                        pedido.ImpostoNegociado = true;
                    }

                    if (pedidoImportacaoSalvar.FreteValor.ComponentesAdicionais != null && pedidoImportacaoSalvar.FreteValor.ComponentesAdicionais.Count > 0)
                    {
                        foreach (Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional componenteAdicional in pedidoImportacaoSalvar.FreteValor.ComponentesAdicionais)
                        {

                            Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete = null;
                            if (componenteAdicional.Componente.TipoComponenteFrete != TipoComponenteFrete.OUTROS)
                                componenteFrete = repComponenteFrete.BuscarPorTipoComponenteFrete(componenteAdicional.Componente.TipoComponenteFrete);
                            else
                                componenteFrete = repComponenteFrete.buscarPorCodigoEmbarcador(componenteAdicional.Componente.CodigoIntegracao);

                            if (componenteFrete != null)
                            {
                                Dominio.Entidades.Embarcador.Pedidos.PedidoComponenteFrete pedidoComponenteFrete = repPedidoComponenteFrete.BuscarPorCompomente(pedido.Codigo, componenteFrete.TipoComponenteFrete, componenteFrete, false);
                                bool inserir = false;
                                if (pedidoComponenteFrete == null)
                                {
                                    pedidoComponenteFrete = new Dominio.Entidades.Embarcador.Pedidos.PedidoComponenteFrete();
                                    inserir = true;
                                }
                                pedidoComponenteFrete.ComponenteFrete = componenteFrete;
                                pedidoComponenteFrete.IncluirBaseCalculoICMS = componenteAdicional.IncluirBaseCalculoICMS;
                                pedidoComponenteFrete.IncluirIntegralmenteContratoFreteTerceiro = false;
                                if (componenteFrete.ImprimirOutraDescricaoCTe)
                                    pedidoComponenteFrete.OutraDescricaoCTe = componenteFrete.DescricaoCTe;

                                pedidoComponenteFrete.ComponenteFilialEmissora = false;
                                pedidoComponenteFrete.Pedido = pedido;
                                pedidoComponenteFrete.TipoComponenteFrete = componenteFrete.TipoComponenteFrete;
                                pedidoComponenteFrete.ValorComponente += componenteAdicional.ValorComponente;

                                if (inserir)
                                    repPedidoComponenteFrete.Inserir(pedidoComponenteFrete);
                                else
                                    repPedidoComponenteFrete.Atualizar(pedidoComponenteFrete);
                            }
                            else
                                retorno = "Não existe um componente de frete cadastrado do tipo ." + componenteAdicional.Componente.CodigoIntegracao;
                        }
                    }

                    if (pedidoImportacaoSalvar.FreteValorFilialEmissora != null && pedidoImportacaoSalvar.FreteValorFilialEmissora.ComponentesAdicionais != null && pedidoImportacaoSalvar.FreteValorFilialEmissora.ComponentesAdicionais.Count > 0)
                    {
                        foreach (Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional componenteAdicional in pedidoImportacaoSalvar.FreteValorFilialEmissora.ComponentesAdicionais)
                        {

                            Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete = null;
                            if (componenteAdicional.Componente.TipoComponenteFrete != TipoComponenteFrete.OUTROS)
                                componenteFrete = repComponenteFrete.BuscarPorTipoComponenteFrete(componenteAdicional.Componente.TipoComponenteFrete);
                            else
                                componenteFrete = repComponenteFrete.buscarPorCodigoEmbarcador(componenteAdicional.Componente.CodigoIntegracao);

                            if (componenteFrete != null)
                            {
                                Dominio.Entidades.Embarcador.Pedidos.PedidoComponenteFrete pedidoComponenteFrete = repPedidoComponenteFrete.BuscarPorCompomente(pedido.Codigo, componenteFrete.TipoComponenteFrete, componenteFrete, true);
                                bool inserir = false;
                                if (pedidoComponenteFrete == null)
                                {
                                    pedidoComponenteFrete = new Dominio.Entidades.Embarcador.Pedidos.PedidoComponenteFrete();
                                    inserir = true;
                                }
                                pedidoComponenteFrete.ComponenteFrete = componenteFrete;
                                pedidoComponenteFrete.ComponenteFilialEmissora = true;
                                pedidoComponenteFrete.IncluirBaseCalculoICMS = componenteAdicional.IncluirBaseCalculoICMS;
                                pedidoComponenteFrete.IncluirIntegralmenteContratoFreteTerceiro = false;
                                if (componenteFrete.ImprimirOutraDescricaoCTe)
                                    pedidoComponenteFrete.OutraDescricaoCTe = componenteFrete.DescricaoCTe;

                                pedidoComponenteFrete.Pedido = pedido;
                                pedidoComponenteFrete.TipoComponenteFrete = componenteFrete.TipoComponenteFrete;
                                pedidoComponenteFrete.ValorComponente += componenteAdicional.ValorComponente;

                                if (inserir)
                                    repPedidoComponenteFrete.Inserir(pedidoComponenteFrete);
                                else
                                    repPedidoComponenteFrete.Atualizar(pedidoComponenteFrete);
                            }
                            else
                                retorno = "Não existe um componente de frete cadastrado do tipo ." + componenteAdicional.Componente.CodigoIntegracao;
                        }
                    }
                }

                pedido.Destino = pedidoEnderecoDestino.Localidade != null ? pedidoEnderecoDestino.Localidade : pedido.Destino;

                if (!criandoNovoPedido)
                    repPedido.Atualizar(pedido);

                if (pedidoEnderecoDestino.Localidade != null)
                    repositorioPedidoEndereco.Atualizar(pedidoEnderecoDestino);

                servOcorrenciaPedido.ProcessarOcorrenciaPedido(EventoColetaEntrega.PedidoGerado, pedido, configuracaoTMS, null);//passa null para nao gerar notificacao de pedido adicionado

                //if (pedido.Produtos == null)
                //    pedido.Produtos = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>();

                if (pedido.TipoOperacao?.ProdutoEmbarcadorPadraoColeta != null && pedidoImportacaoSalvar.PedidoProduto == null)
                {
                    //Dominio.Entidades.Embarcador.Pedidos.PedidoProduto 
                    pedidoImportacaoSalvar.PedidoProduto = new Dominio.Entidades.Embarcador.Pedidos.PedidoProduto();
                    pedidoImportacaoSalvar.PedidoProduto.Pedido = pedido;
                    pedidoImportacaoSalvar.PedidoProduto.Produto = pedido.TipoOperacao.ProdutoEmbarcadorPadraoColeta;
                    repPedidoProduto.Inserir(pedidoImportacaoSalvar.PedidoProduto);
                }

                if (pedidoImportacaoSalvar.NotasParciais?.Count > 0)
                    SalvarNotasParciais(pedido, pedidoImportacaoSalvar.NotasParciais, unitOfWork);

                if (pedidoImportacaoSalvar.CtesParciais?.Count > 0)
                    SalvarCtesParciais(pedido, pedidoImportacaoSalvar.CtesParciais, unitOfWork);

                SalvarDadosAdicionais(pedido, pedidoImportacaoSalvar, unitOfWork);

                if (pedidoImportacaoSalvar.Nfes != null && pedidoImportacaoSalvar.Nfes.Count > 0)
                {
                    pedido.ValorTotalNotasFiscais = 0;
                    for (int i = 0; i < pedidoImportacaoSalvar.Nfes.Count; i++)
                    {
                        AdicionarNotaFiscal(pedido, pedidoImportacaoSalvar.Nfes[i], unitOfWork);
                        if (pedido.NotasFiscais.Count > 0)
                            servOcorrenciaPedido.ProcessarOcorrenciaPedido(EventoColetaEntrega.PedidoFaturado, pedido, configuracaoTMS, null);//passa null para nao gerar notificacao de pedido adicionado
                    }
                }

                if (!string.IsNullOrWhiteSpace(pedido.NumeroControle))
                {
                    AdicionarXMLNotasFiscais(pedido, unitOfWork);
                    if (pedido.NotasFiscais.Count > 0)
                        servOcorrenciaPedido.ProcessarOcorrenciaPedido(EventoColetaEntrega.PedidoFaturado, pedido, configuracaoTMS, null);//passa null para nao gerar notificacao de pedido adicionado
                }
            }

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                if (configuracaoTMS.PermitirSelecionarReboquePedido)
                {
                    if (pedidoImportacaoSalvar.Veiculo != null)
                        pedido.VeiculoTracao = pedidoImportacaoSalvar.Veiculo;
                }
                else
                {
                    if (pedidoImportacaoSalvar.Veiculo != null)
                        pedido.Veiculos.Add(pedidoImportacaoSalvar.Veiculo);
                }

                if (pedidoImportacaoSalvar.Reboques != null)
                {
                    foreach (Dominio.Entidades.Veiculo reboque in pedidoImportacaoSalvar.Reboques)
                        pedido.Veiculos.Add(reboque);
                }
                else if (pedidoImportacaoSalvar.Veiculo?.VeiculosVinculados.Count > 0)
                {
                    foreach (Dominio.Entidades.Veiculo reboque in pedidoImportacaoSalvar.Veiculo.VeiculosVinculados)
                        pedido.Veiculos.Add(reboque);
                }

                if (pedidoImportacaoSalvar.Motoristas != null)
                {
                    if (pedido.Motoristas == null)
                        pedido.Motoristas = new List<Dominio.Entidades.Usuario>();

                    foreach (Dominio.Entidades.Usuario motorista in pedidoImportacaoSalvar.Motoristas)
                        pedido.Motoristas.Add(motorista);
                }

                if (pedidoImportacaoSalvar.Fronteiras?.Count > 0)
                {
                    SalvarListaFronteiras(pedido, pedidoImportacaoSalvar.Fronteiras, unitOfWork);
                }

                if (string.IsNullOrWhiteSpace(retorno) && IsGerarCargaAutomaticamente(pedido) && !naoTentarGerarCarga)
                    CriarCarga(out retorno, unitOfWork, new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>() { pedido }, configuracaoTMS, tipoServicoMultisoftware, clienteMultiSoftware);
            }

            if (configuracaoPedido.AtualizarCargaAoImportarPlanilha && criandoNovoPedido)
            {
                CriarCargaPedido(pedidoImportacaoSalvar.PreCarga?.Carga, pedido, unitOfWork, tipoServicoMultisoftware, pedidoImportacaoSalvar.FaixaTemperatura, configuracaoTMS, auditado);
            }
            else
                AtualizarCargaPedido(pedido, pedidoImportacaoSalvar.CargaExistente, unitOfWork, configuracaoTMS, tipoServicoMultisoftware, pedidoImportacaoSalvar.FaixaTemperatura, configuracaoPedido.AtualizarCargaAoImportarPlanilha);


            Repositorio.Embarcador.Pedidos.TipoOperacaoIntegracao repositorioTipoOperacaoIntegracao = new Repositorio.Embarcador.Pedidos.TipoOperacaoIntegracao(unitOfWork);

            if (pedido.TipoOperacao != null && repositorioTipoOperacaoIntegracao.ExisteTipoOperacaoIntegracao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Routeasy, pedido.TipoOperacao.Codigo))
                new Servicos.Embarcador.Integracao.IntegracaoPedidoRoterizador(unitOfWork).AdicionarParaIntegracaoAutomaticamente(pedido.Codigo, TipoRoteirizadorIntegracao.EnviarPedido);

            return new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha
            {
                codigo = pedido.Codigo,
                mensagemFalha = retorno
            };
        }

        public void CriarCarga(out string mensagemRetorno, Repositorio.UnitOfWork unitOfWork, List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultiSoftware)
        {
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

            mensagemRetorno = Servicos.Embarcador.Pedido.Pedido.CriarCarga(out Dominio.Entidades.Embarcador.Cargas.Carga carga, pedidos, unitOfWork, tipoServicoMultisoftware, clienteMultiSoftware, configuracao, false, true);

            foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidos)
                repositorioPedido.Atualizar(pedido);
        }

        public void PreecherEnderecoPedido(ref Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco pedidoEndereco, Dominio.Entidades.Cliente cliente)
        {
            pedidoEndereco.Bairro = cliente.Bairro;
            pedidoEndereco.CEP = cliente.CEP;
            pedidoEndereco.Localidade = cliente.Localidade;
            pedidoEndereco.Complemento = cliente.Complemento;
            pedidoEndereco.Endereco = cliente.Endereco.Length > 80 ? cliente.Endereco.Substring(0, 80) : cliente.Endereco;
            pedidoEndereco.Numero = cliente.Numero;
            pedidoEndereco.Telefone = cliente.Telefone1;
            pedidoEndereco.IE_RG = cliente.IE_RG;
        }

        private void AdicionarXMLNotasFiscais(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);

            if (pedido.NotasFiscais == null)
                pedido.NotasFiscais = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();

            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais = repXMLNotaFiscal.BuscarPorNumeroControlePedido(pedido.NumeroControle);
            foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal nota in notasFiscais)
                if (!pedido.NotasFiscais.Contains(nota)) pedido.NotasFiscais.Add(nota);

            repositorioPedido.Atualizar(pedido);
        }

        private void SalvarCtesParciais(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, List<int> ctesParciais, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoCTeParcial repositorioPedidoCTeParcial = new Repositorio.Embarcador.Pedidos.PedidoCTeParcial(unitOfWork);
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParcial> listaPedidoCTeParcial = repositorioPedidoCTeParcial.BuscarPorPedido(pedido.Codigo);

            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParcial pedidoCTeParcial in listaPedidoCTeParcial)
            {
                if (ctesParciais.Contains(pedidoCTeParcial.Numero))
                    repositorioPedidoCTeParcial.Deletar(pedidoCTeParcial);
            }


            foreach (int numeroCte in ctesParciais)
            {
                Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParcial pedidoCteParcial = new Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParcial()
                {
                    Pedido = pedido,
                    Numero = numeroCte
                };

                repositorioPedidoCTeParcial.Inserir(pedidoCteParcial);
            }
        }

        public void SalvarNotasParciais(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, List<int> numerosNFParciais, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoNotaParcial repositorioPedidoNotaParcial = new Repositorio.Embarcador.Pedidos.PedidoNotaParcial(unitOfWork);
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoNotaParcial> pedidosXMLNotaFiscaisParcialExistente = repositorioPedidoNotaParcial.BuscarPorPedido(pedido.Codigo);

            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoNotaParcial pedidoXMLNotaFiscaisParcialExistente in pedidosXMLNotaFiscaisParcialExistente)
            {
                if (numerosNFParciais.Contains(pedidoXMLNotaFiscaisParcialExistente.Numero))
                    repositorioPedidoNotaParcial.Deletar(pedidoXMLNotaFiscaisParcialExistente);
            }

            foreach (int numeroNF in numerosNFParciais)
            {
                Dominio.Entidades.Embarcador.Pedidos.PedidoNotaParcial pedidoNotaParcial = new Dominio.Entidades.Embarcador.Pedidos.PedidoNotaParcial()
                {
                    Pedido = pedido,
                    Numero = numeroNF,
                    NumeroPedido = "",
                    DataCriacao = DateTime.Now
                };

                repositorioPedidoNotaParcial.Inserir(pedidoNotaParcial);
            }
        }

        private void SalvarDadosAdicionais(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoImportacaoSalvar pedidoImportacaoAdicionar, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoAdicional repositorioPedidoAdicional = new Repositorio.Embarcador.Pedidos.PedidoAdicional(unitOfWork);
            Dominio.Entidades.Embarcador.Pedidos.PedidoAdicional pedidoAdicional = repositorioPedidoAdicional.BuscarPorPedido(pedido.Codigo);

            if (pedidoAdicional == null)
            {
                pedidoAdicional = new Dominio.Entidades.Embarcador.Pedidos.PedidoAdicional();
                pedidoAdicional.Pedido = pedido;
            }

            pedidoAdicional.PedidoOrigem = pedidoImportacaoAdicionar?.NumeroPedidoOrigem ?? null;
            pedidoAdicional.NumeroPedidoVinculado = pedidoImportacaoAdicionar?.NumeroPedidoVinculado ?? string.Empty;
            pedidoAdicional.EssePedidopossuiPedidoBonificacao = pedidoImportacaoAdicionar?.EssePedidopossuiPedidoBonificacao ?? false;
            pedidoAdicional.EssePedidopossuiPedidoVenda = pedidoImportacaoAdicionar?.EssePedidopossuiPedidoVenda ?? false;

            if (pedidoAdicional.Codigo > 0)
                repositorioPedidoAdicional.Atualizar(pedidoAdicional);
            else
                repositorioPedidoAdicional.Inserir(pedidoAdicional);
        }

        public string AdicionarNotaFiscal(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal notaFiscal, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repositorioXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Servicos.Embarcador.Pedido.NotaFiscal servicoNotaFiscal = new NotaFiscal(unitOfWork);
            Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = null;

            if (pedido.NotasFiscais == null)
                pedido.NotasFiscais = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();

            if (!string.IsNullOrWhiteSpace(notaFiscal.Chave))
                xmlNotaFiscal = repositorioXmlNotaFiscal.BuscarPorChave(notaFiscal.Chave);

            if (xmlNotaFiscal == null)
            {
                string mensagemErro = "";

                xmlNotaFiscal = new Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal();
                xmlNotaFiscal = servicoNotaFiscal.PreencherParaXMLNotaFiscal(ref xmlNotaFiscal, notaFiscal, pedido.Empresa, pedido.Filial, ref mensagemErro);
                xmlNotaFiscal.DataRecebimento = DateTime.Now;

                if (!string.IsNullOrWhiteSpace(mensagemErro))
                    return mensagemErro;

                repositorioXmlNotaFiscal.Inserir(xmlNotaFiscal);

                if (notaFiscal.Produtos?.Count > 0)
                {
                    Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);
                    Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();

                    if (configuracaoGeralCarga?.UtilizarPesoProdutoParaCalcularPesoCarga ?? false)
                    {
                        Repositorio.Embarcador.Produtos.ProdutoEmbarcador repositorioProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork);
                        int grupoPessoa = xmlNotaFiscal.ObterEmitente.GrupoPessoas?.Codigo ?? 0;
                        decimal pesoBruto = 0m;
                        decimal pesoLiquido = 0m;

                        foreach (Dominio.ObjetosDeValor.Embarcador.Pedido.Produto produtoCargaIntegracao in notaFiscal.Produtos)
                        {
                            Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produto = null;

                            if (grupoPessoa > 0)
                                produto = repositorioProdutoEmbarcador.buscarPorCodigoEmbarcador(produtoCargaIntegracao.CodigoProduto, grupoPessoa);
                            else
                                produto = repositorioProdutoEmbarcador.buscarPorCodigoEmbarcador(produtoCargaIntegracao.CodigoProduto);

                            if (produto != null)
                            {
                                pesoBruto += produtoCargaIntegracao.Quantidade * produto.PesoUnitario;
                                pesoLiquido += produtoCargaIntegracao.Quantidade * produto.PesoLiquidoUnitario;
                            }
                        }

                        xmlNotaFiscal.Peso = pesoBruto;
                        xmlNotaFiscal.PesoLiquido = pesoLiquido;
                        xmlNotaFiscal.PesoBaseParaCalculo = xmlNotaFiscal.Peso;

                        repositorioXmlNotaFiscal.Atualizar(xmlNotaFiscal);
                    }
                }
            }

            if (pedido.PedidoTotalmenteCarregado)
                pedido.PesoTotal += xmlNotaFiscal.Peso;

            pedido.ValorTotalNotasFiscais += xmlNotaFiscal.Valor;

            Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga servicoMontagemCarga = new Carga.MontagemCarga.MontagemCarga(unitOfWork);
            servicoMontagemCarga.AtualizarSituacaoExigeIscaPorPedido(pedido);

            pedido.NotasFiscais.Add(xmlNotaFiscal);
            repositorioPedido.Atualizar(pedido);

            return "";
        }

        public static List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedido> VerificarRegrasPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorLancamento repColaboradorSituacao = new Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorLancamento(unitOfWork);
            Repositorio.Embarcador.Pedidos.RegrasPedido repRegrasPedido = new Repositorio.Embarcador.Pedidos.RegrasPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedido> listaRegras = new List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedido>();
            List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedido> listaFiltrada = new List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedido>();

            pedido = repPedido.BuscarPorCodigo(pedido.Codigo);

            List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedido> listaRegraGrupoPessoa = null;
            if (pedido.GrupoPessoas != null)
            {
                listaRegraGrupoPessoa = repRegrasPedido.BuscarRegraPorGrupoPessoa(pedido.GrupoPessoas.Codigo, pedido.DataCarregamentoPedido.HasValue ? pedido.DataCarregamentoPedido.Value : DateTime.Now.Date);
                listaRegras.AddRange(listaRegraGrupoPessoa);
            }

            List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedido> listaRegraTipoCarga = null;
            if (pedido.TipoDeCarga != null)
            {
                listaRegraTipoCarga = repRegrasPedido.BuscarRegraPorTipoCarga(pedido.TipoDeCarga.Codigo, pedido.DataCarregamentoPedido.HasValue ? pedido.DataCarregamentoPedido.Value : DateTime.Now.Date);
                listaRegras.AddRange(listaRegraTipoCarga);
            }

            List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedido> listaRegraTipoOperacao = null;
            if (pedido.TipoOperacao != null)
            {
                listaRegraTipoOperacao = repRegrasPedido.BuscarRegraPorTipoOperacao(pedido.TipoOperacao.Codigo, pedido.DataCarregamentoPedido.HasValue ? pedido.DataCarregamentoPedido.Value : DateTime.Now.Date);
                listaRegras.AddRange(listaRegraTipoOperacao);
            }

            List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedido> listaRegraModeloVeicular = null;
            if (pedido.ModeloVeicularCarga != null)
            {
                listaRegraModeloVeicular = repRegrasPedido.BuscarRegraPorModeloVeicular(pedido.ModeloVeicularCarga.Codigo, pedido.DataCarregamentoPedido.HasValue ? pedido.DataCarregamentoPedido.Value : DateTime.Now.Date);
                listaRegras.AddRange(listaRegraModeloVeicular);
            }

            List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedido> listaRegraSituacaoColaborador = null;
            if (pedido.Motoristas != null && pedido.Motoristas.Count > 0)
            {
                List<int> codigosMotorista = pedido.Motoristas.Select(c => c.Codigo).ToList();
                if (codigosMotorista != null && codigosMotorista.Count > 0)
                {
                    List<Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorSituacao> situcoesEmExecucao = repColaboradorSituacao.BuscarSituacoesEmExecucao(codigosMotorista);
                    if (situcoesEmExecucao != null && situcoesEmExecucao.Count > 0)
                    {
                        foreach (Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorSituacao situacaoExecucao in situcoesEmExecucao)
                        {
                            listaRegraSituacaoColaborador = repRegrasPedido.BuscarRegraPorSituacaoColaborador(situacaoExecucao.Codigo, pedido.DataCarregamentoPedido.HasValue ? pedido.DataCarregamentoPedido.Value : DateTime.Now.Date);
                            listaRegras.AddRange(listaRegraSituacaoColaborador);
                        }
                    }
                }
            }

            List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedido> listaRegraValor = repRegrasPedido.BuscarRegraPorValor(pedido.ValorFreteNegociado, pedido.DataCarregamentoPedido.HasValue ? pedido.DataCarregamentoPedido.Value : DateTime.Now.Date);
            listaRegras.AddRange(listaRegraValor);

            if (pedido.RotaFrete != null)
            {
                List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedido> listaDistancia = repRegrasPedido.BuscarRegraPorDistancia(pedido.RotaFrete.Quilometros, pedido.DataCarregamentoPedido.HasValue ? pedido.DataCarregamentoPedido.Value : DateTime.Now.Date);
                listaRegras.AddRange(listaDistancia);
            }

            decimal diferencaFreteLiquidoParaFreteTerceiro = 0;
            if ((pedido?.ValorFreteNegociado != 0) && (pedido?.ValorFreteTransportadorTerceiro != 0))
                diferencaFreteLiquidoParaFreteTerceiro = ((pedido?.ValorFreteNegociado ?? 0 - pedido?.ValorFreteTransportadorTerceiro ?? 0 * 100) / pedido?.ValorFreteTransportadorTerceiro ?? 0);

            if (diferencaFreteLiquidoParaFreteTerceiro != 0)
            {
                List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedido> listaDiferencaFreteLiquidoParaFreteTerceiro = repRegrasPedido.BuscarRegraPorDiferencaFreteLiquidoParaFreteTerceiro(diferencaFreteLiquidoParaFreteTerceiro, pedido.DataCarregamentoPedido.HasValue ? pedido.DataCarregamentoPedido.Value : DateTime.Now.Date);
                listaRegras.AddRange(listaDiferencaFreteLiquidoParaFreteTerceiro);
            }

            if (listaRegras.Distinct().Count() > 0)
            {
                listaFiltrada.AddRange(listaRegras.Distinct());

                foreach (Dominio.Entidades.Embarcador.Pedidos.RegrasPedido regra in listaRegras.Distinct())
                {
                    if (regra.RegraPorGrupoPessoa && pedido.GrupoPessoas != null)
                    {
                        bool valido = false;
                        if (regra.RegrasPedidoGrupoPessoa.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.E && o.GrupoPessoas.Codigo == pedido.GrupoPessoas.Codigo))
                            valido = true;
                        else if (regra.RegrasPedidoGrupoPessoa.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.Ou && o.GrupoPessoas.Codigo == pedido.GrupoPessoas.Codigo))
                            valido = true;
                        else if (regra.RegrasPedidoGrupoPessoa.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.Ou && o.GrupoPessoas.Codigo != pedido.GrupoPessoas.Codigo))
                            valido = true;
                        else if (regra.RegrasPedidoGrupoPessoa.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.E && o.GrupoPessoas.Codigo != pedido.GrupoPessoas.Codigo))
                            valido = true;

                        if (!valido)
                        {
                            listaFiltrada.Remove(regra);
                            continue;
                        }
                    }
                    if (regra.RegraPorTipoCarga && pedido.TipoDeCarga != null)
                    {
                        bool valido = false;
                        if (regra.RegrasPedidoTipoCarga.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.E && o.TipoDeCarga.Codigo == pedido.TipoDeCarga.Codigo))
                            valido = true;
                        else if (regra.RegrasPedidoTipoCarga.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.Ou && o.TipoDeCarga.Codigo == pedido.TipoDeCarga.Codigo))
                            valido = true;
                        else if (regra.RegrasPedidoTipoCarga.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.Ou && o.TipoDeCarga.Codigo != pedido.TipoDeCarga.Codigo))
                            valido = true;
                        else if (regra.RegrasPedidoTipoCarga.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.E && o.TipoDeCarga.Codigo != pedido.TipoDeCarga.Codigo))
                            valido = true;

                        if (!valido)
                        {
                            listaFiltrada.Remove(regra);
                            continue;
                        }
                    }

                    if (regra.RegraPorTipoOperacao && pedido.TipoOperacao != null)
                    {
                        bool valido = false;
                        if (regra.RegrasPedidoTipoOperacao.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.E && o.TipoOperacao.Codigo == pedido.TipoOperacao.Codigo))
                            valido = true;
                        else if (regra.RegrasPedidoTipoOperacao.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.Ou && o.TipoOperacao.Codigo == pedido.TipoOperacao.Codigo))
                            valido = true;
                        else if (regra.RegrasPedidoTipoOperacao.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.Ou && o.TipoOperacao.Codigo != pedido.TipoOperacao.Codigo))
                            valido = true;
                        else if (regra.RegrasPedidoTipoOperacao.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.E && o.TipoOperacao.Codigo != pedido.TipoOperacao.Codigo))
                            valido = true;

                        if (!valido)
                        {
                            listaFiltrada.Remove(regra);
                            continue;
                        }
                    }

                    if (regra.RegraPorModeloVeicular && pedido.ModeloVeicularCarga != null)
                    {
                        bool valido = false;
                        if (regra.RegrasPedidoModeloVeicular.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.E && o.ModeloVeicularCarga.Codigo == pedido.ModeloVeicularCarga.Codigo))
                            valido = true;
                        else if (regra.RegrasPedidoModeloVeicular.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.Ou && o.ModeloVeicularCarga.Codigo == pedido.ModeloVeicularCarga.Codigo))
                            valido = true;
                        else if (regra.RegrasPedidoModeloVeicular.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.Ou && o.ModeloVeicularCarga.Codigo != pedido.ModeloVeicularCarga.Codigo))
                            valido = true;
                        else if (regra.RegrasPedidoModeloVeicular.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.E && o.ModeloVeicularCarga.Codigo != pedido.ModeloVeicularCarga.Codigo))
                            valido = true;

                        if (!valido)
                        {
                            listaFiltrada.Remove(regra);
                            continue;
                        }
                    }

                    if (regra.RegraPorSituacaoColaborador && pedido.Motoristas != null && pedido.Motoristas.Count > 0)
                    {
                        List<int> codigosMotorista = pedido.Motoristas.Select(c => c.Codigo).ToList();
                        if (codigosMotorista != null && codigosMotorista.Count > 0)
                        {
                            List<Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorSituacao> situcoesEmExecucao = repColaboradorSituacao.BuscarSituacoesEmExecucao(codigosMotorista);
                            if (situcoesEmExecucao != null && situcoesEmExecucao.Count > 0)
                            {
                                foreach (Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorSituacao situacaoExecucao in situcoesEmExecucao)
                                {
                                    bool valido = false;
                                    if (regra.RegrasPedidoSituacaoColaborador.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.E && o.ColaboradorSituacao.Codigo == situacaoExecucao.Codigo))
                                        valido = true;
                                    else if (regra.RegrasPedidoSituacaoColaborador.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.Ou && o.ColaboradorSituacao.Codigo == situacaoExecucao.Codigo))
                                        valido = true;
                                    else if (regra.RegrasPedidoSituacaoColaborador.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.Ou && o.ColaboradorSituacao.Codigo != situacaoExecucao.Codigo))
                                        valido = true;
                                    else if (regra.RegrasPedidoSituacaoColaborador.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.E && o.ColaboradorSituacao.Codigo != situacaoExecucao.Codigo))
                                        valido = true;

                                    if (!valido)
                                    {
                                        listaFiltrada.Remove(regra);
                                        continue;
                                    }
                                }
                            }
                        }
                    }

                    if (regra.RegraPorValorFrete)
                    {
                        bool valido = false;
                        if (regra.RegrasPedidoValorFrete.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.E && o.Valor == pedido.ValorFreteNegociado))
                            valido = true;
                        else if (regra.RegrasPedidoValorFrete.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.Ou && o.Valor == pedido.ValorFreteNegociado))
                            valido = true;
                        else if (regra.RegrasPedidoValorFrete.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.Ou && o.Valor != pedido.ValorFreteNegociado))
                            valido = true;
                        else if (regra.RegrasPedidoValorFrete.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.E && o.Valor != pedido.ValorFreteNegociado))
                            valido = true;
                        if (regra.RegrasPedidoValorFrete.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.MaiorIgualQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.E && pedido.ValorFreteNegociado >= o.Valor))
                            valido = true;
                        else if (regra.RegrasPedidoValorFrete.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.MaiorIgualQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.Ou && pedido.ValorFreteNegociado >= o.Valor))
                            valido = true;
                        if (regra.RegrasPedidoValorFrete.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.MenorIgualQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.E && pedido.ValorFreteNegociado <= o.Valor))
                            valido = true;
                        else if (regra.RegrasPedidoValorFrete.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.MenorIgualQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.Ou && pedido.ValorFreteNegociado <= o.Valor))
                            valido = true;
                        if (regra.RegrasPedidoValorFrete.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.MaiorQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.E && pedido.ValorFreteNegociado > o.Valor))
                            valido = true;
                        else if (regra.RegrasPedidoValorFrete.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.MaiorQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.Ou && pedido.ValorFreteNegociado > o.Valor))
                            valido = true;
                        if (regra.RegrasPedidoValorFrete.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.MenorQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.E && pedido.ValorFreteNegociado < o.Valor))
                            valido = true;
                        else if (regra.RegrasPedidoValorFrete.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.MenorQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.Ou && pedido.ValorFreteNegociado < o.Valor))
                            valido = true;

                        if (!valido)
                        {
                            listaFiltrada.Remove(regra);
                            continue;
                        }
                    }

                    if (regra.RegraPorDistancia)
                    {
                        bool valido = false;
                        if (regra.RegrasPedidoDistancia.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.E && o.Distancia == (pedido.RotaFrete?.Quilometros ?? 0)))
                            valido = true;
                        else if (regra.RegrasPedidoDistancia.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.Ou && o.Distancia == (pedido.RotaFrete?.Quilometros ?? 0)))
                            valido = true;
                        else if (regra.RegrasPedidoDistancia.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.Ou && o.Distancia != (pedido.RotaFrete?.Quilometros ?? 0)))
                            valido = true;
                        else if (regra.RegrasPedidoDistancia.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.E && o.Distancia != (pedido.RotaFrete?.Quilometros ?? 0)))
                            valido = true;
                        if (regra.RegrasPedidoDistancia.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.MaiorIgualQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.E && (pedido.RotaFrete?.Quilometros ?? 0) >= o.Distancia))
                            valido = true;
                        else if (regra.RegrasPedidoDistancia.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.MaiorIgualQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.Ou && (pedido.RotaFrete?.Quilometros ?? 0) >= o.Distancia))
                            valido = true;
                        if (regra.RegrasPedidoDistancia.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.MenorIgualQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.E && (pedido.RotaFrete?.Quilometros ?? 0) <= o.Distancia))
                            valido = true;
                        else if (regra.RegrasPedidoDistancia.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.MenorIgualQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.Ou && (pedido.RotaFrete?.Quilometros ?? 0) <= o.Distancia))
                            valido = true;
                        if (regra.RegrasPedidoDistancia.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.MaiorQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.E && (pedido.RotaFrete?.Quilometros ?? 0) > o.Distancia))
                            valido = true;
                        else if (regra.RegrasPedidoDistancia.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.MaiorQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.Ou && (pedido.RotaFrete?.Quilometros ?? 0) > o.Distancia))
                            valido = true;
                        if (regra.RegrasPedidoDistancia.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.MenorQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.E && (pedido.RotaFrete?.Quilometros ?? 0) < o.Distancia))
                            valido = true;
                        else if (regra.RegrasPedidoDistancia.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.MenorQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.Ou && (pedido.RotaFrete?.Quilometros ?? 0) < o.Distancia))
                            valido = true;

                        if (!valido)
                        {
                            listaFiltrada.Remove(regra);
                            continue;
                        }
                    }

                    if (regra.RegraPorDiferencaFreteLiquidoParaFreteTerceiro)
                    {
                        bool valido = false;
                        if (regra.RegrasPedidoPercentualDiferencaFreteLiquidoParaFreteTerceiro.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.E && o.DiferencaFreteLiquidoParaFreteTerceiro == diferencaFreteLiquidoParaFreteTerceiro))
                            valido = true;
                        else if (regra.RegrasPedidoPercentualDiferencaFreteLiquidoParaFreteTerceiro.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.Ou && o.DiferencaFreteLiquidoParaFreteTerceiro == diferencaFreteLiquidoParaFreteTerceiro))
                            valido = true;
                        else if (regra.RegrasPedidoPercentualDiferencaFreteLiquidoParaFreteTerceiro.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.Ou && o.DiferencaFreteLiquidoParaFreteTerceiro != diferencaFreteLiquidoParaFreteTerceiro))
                            valido = true;
                        else if (regra.RegrasPedidoPercentualDiferencaFreteLiquidoParaFreteTerceiro.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.E && o.DiferencaFreteLiquidoParaFreteTerceiro != diferencaFreteLiquidoParaFreteTerceiro))
                            valido = true;
                        if (regra.RegrasPedidoPercentualDiferencaFreteLiquidoParaFreteTerceiro.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.MaiorIgualQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.E && diferencaFreteLiquidoParaFreteTerceiro >= o.DiferencaFreteLiquidoParaFreteTerceiro))
                            valido = true;
                        else if (regra.RegrasPedidoPercentualDiferencaFreteLiquidoParaFreteTerceiro.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.MaiorIgualQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.Ou && diferencaFreteLiquidoParaFreteTerceiro >= o.DiferencaFreteLiquidoParaFreteTerceiro))
                            valido = true;
                        if (regra.RegrasPedidoPercentualDiferencaFreteLiquidoParaFreteTerceiro.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.MenorIgualQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.E && diferencaFreteLiquidoParaFreteTerceiro <= o.DiferencaFreteLiquidoParaFreteTerceiro))
                            valido = true;
                        else if (regra.RegrasPedidoPercentualDiferencaFreteLiquidoParaFreteTerceiro.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.MenorIgualQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.Ou && diferencaFreteLiquidoParaFreteTerceiro <= o.DiferencaFreteLiquidoParaFreteTerceiro))
                            valido = true;
                        if (regra.RegrasPedidoPercentualDiferencaFreteLiquidoParaFreteTerceiro.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.MaiorQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.E && diferencaFreteLiquidoParaFreteTerceiro > o.DiferencaFreteLiquidoParaFreteTerceiro))
                            valido = true;
                        else if (regra.RegrasPedidoPercentualDiferencaFreteLiquidoParaFreteTerceiro.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.MaiorQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.Ou && diferencaFreteLiquidoParaFreteTerceiro > o.DiferencaFreteLiquidoParaFreteTerceiro))
                            valido = true;
                        if (regra.RegrasPedidoPercentualDiferencaFreteLiquidoParaFreteTerceiro.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.MenorQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.E && diferencaFreteLiquidoParaFreteTerceiro < o.DiferencaFreteLiquidoParaFreteTerceiro))
                            valido = true;
                        else if (regra.RegrasPedidoPercentualDiferencaFreteLiquidoParaFreteTerceiro.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia.MenorQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.Ou && diferencaFreteLiquidoParaFreteTerceiro < o.DiferencaFreteLiquidoParaFreteTerceiro))
                            valido = true;

                        if (!valido)
                        {
                            listaFiltrada.Remove(regra);
                            continue;
                        }
                    }

                }
            }

            return listaFiltrada;
        }

        public static void CriarRegrasAutorizacao(List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedido> listaFiltrada, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.Entidades.Usuario usuario, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string stringConexao, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(stringConexao, null, tipoServicoMultisoftware, string.Empty);
            Repositorio.Embarcador.Pedidos.PedidoAutorizacao repPedidosAutorizacao = new Repositorio.Embarcador.Pedidos.PedidoAutorizacao(unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Pedidos.RegrasPedido regra in listaFiltrada)
            {
                foreach (Dominio.Entidades.Usuario aprovador in regra.Aprovadores)
                {
                    Dominio.Entidades.Embarcador.Pedidos.PedidoAutorizacao autorizacao = new Dominio.Entidades.Embarcador.Pedidos.PedidoAutorizacao();
                    autorizacao.Pedido = pedido;
                    autorizacao.Usuario = aprovador;
                    autorizacao.RegrasPedido = regra;
                    autorizacao.EtapaAutorizacaoOcorrencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoOcorrencia.AprovacaoOcorrencia;
                    autorizacao.Data = DateTime.Now;
                    repPedidosAutorizacao.Inserir(autorizacao);

                    string titulo = Localization.Resources.Pedidos.Pedido.AlcadaPedido;
                    string nota = string.Empty;
                    nota = string.Format(Localization.Resources.Pedidos.Pedido.SolicitouLiberacaoPedidoValorMotorista, usuario.Nome, pedido.Numero.ToString("n0"), pedido.ValorFreteNegociado.ToString("n2"), (pedido.NomeMotoristas));

                    try
                    {
                        if (regra.RegrasPedidoSituacaoColaborador != null && regra.RegrasPedidoSituacaoColaborador.Count > 0)
                        {
                            string situacoesDaRegra = string.Join(",", regra.RegrasPedidoSituacaoColaborador.Select(c => c.ColaboradorSituacao.Descricao).ToList());
                            nota = string.Format(Localization.Resources.Pedidos.Pedido.PedidoPendenteAprovacaoMotoristaSituacao, pedido.Numero.ToString("D"), (pedido.NomeMotoristas), situacoesDaRegra);
                        }
                    }
                    catch
                    {
                    }

                    serNotificacao.GerarNotificacaoEmail(aprovador, usuario, pedido.Codigo, "Pedidos/Pedido", titulo, nota, Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.cifra, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.credito, tipoServicoMultisoftware, unitOfWork);
                    serNotificacao.GerarNotificacao(aprovador, usuario, pedido.Codigo, "Pedidos/Pedido", nota, Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.agConfirmacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.alerta, tipoServicoMultisoftware, unitOfWork);
                }
            }

        }

        public static string CriarCarga(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware TipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente ClienteMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, bool forcarGeracaoCarga = false, bool cadastroPedido = false)
        {
            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>() { pedido };
            return CriarCarga(out _, pedidos, unitOfWork, TipoServicoMultisoftware, ClienteMultisoftware, configuracao, forcarGeracaoCarga, cadastroPedido);
        }

        public static string CriarCarga(out Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware TipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente ClienteMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, bool forcarGeracaoCarga = false, bool cadastroPedido = false, bool adicionarJanelaDescarregamento = true, bool gerarAgendamentoColeta = true, bool origemDoAgendamento = false, Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta = null, bool origemTelaPlanejamentoPedidoTMS = false)
        {
            string retorno = "";

            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = pedidos.FirstOrDefault();

            carga = null;

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && origemTelaPlanejamentoPedidoTMS)
                foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoValidacao in pedidos)
                    if (pedidoValidacao.SituacaoPedido == SituacaoPedido.AutorizacaoPendente)
                        return $"Pedido: {pedidoValidacao.Numero} está Pendente de Autorização.";

            if (forcarGeracaoCarga || (pedido.GerarAutomaticamenteCargaDoPedido && pedido.SituacaoPedido == SituacaoPedido.Aberto && ((pedido.TipoOperacao == null && !configuracao.NaoGerarCargaDePedidoSemTipoOperacao) || pedido.TipoOperacao?.GeraCargaAutomaticamente == true)))
            {
                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro repositorioConfiguracaoContratoFreteTerceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro configuracaoContratoFreteTerceiro = repositorioConfiguracaoContratoFreteTerceiro.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoDeCarga = origemDoAgendamento ? agendamentoColeta?.TipoCarga ?? pedido.TipoDeCarga : pedido.TipoDeCarga;
                Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga = origemDoAgendamento ? agendamentoColeta?.ModeloVeicular ?? pedido.ModeloVeicularCarga : pedido.ModeloVeicularCarga;
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = repConfiguracaoPedido.BuscarConfiguracaoPadrao();

                carga = new Dominio.Entidades.Embarcador.Cargas.Carga();

                carga.Filial = pedido.Filial;
                carga.Empresa = pedido.Empresa;

                if ((forcarGeracaoCarga || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS) && string.IsNullOrWhiteSpace(pedido.CodigoCargaEmbarcador))
                {
                    if (!configuracao.NumeroCargaSequencialUnico)
                        pedido.CodigoCargaEmbarcador = Servicos.Embarcador.Cargas.CargaSequencial.GetInstance().ObterProximoNumeroSequencial(unitOfWork, pedido.Filial?.Codigo ?? 0).ToString();
                    else
                        pedido.CodigoCargaEmbarcador = Servicos.Embarcador.Cargas.CargaSequencial.GetInstance().ObterProximoNumeroSequencial(unitOfWork).ToString();
                }

                string sequencialCargaAlfanumerico = serCarga.ObtemProximoSequencialAlfanumericoCarga(unitOfWork);

                if (pedido?.TipoOperacao?.ConfiguracaoCarga?.IncrementaCodigoPorTipoOperacao ?? false)
                {
                    carga.NumeroSequenciaCarga = Servicos.Embarcador.Cargas.CargaSequencial.GetInstance().ObterProximoNumeroSequencial(unitOfWork, null, pedido?.TipoOperacao ?? null);
                    carga.CodigoCargaEmbarcador = $"{pedido?.TipoOperacao?.ConfiguracaoCarga?.AdicionaPrefixoCodigoCarga ?? string.Empty}{carga.NumeroSequenciaCarga}";
                    pedido.CodigoCargaEmbarcador = carga.CodigoCargaEmbarcador;
                }
                else if (!string.IsNullOrEmpty(sequencialCargaAlfanumerico))
                {
                    pedido.CodigoCargaEmbarcador = sequencialCargaAlfanumerico;
                    carga.CodigoCargaEmbarcador = pedido.CodigoCargaEmbarcador;
                    carga.CodigoAlfanumericoCarga = pedido.CodigoCargaEmbarcador;
                    repPedido.Atualizar(pedido);
                }
                else if (agendamentoColeta?.Sequencia > 0)
                    carga.CodigoCargaEmbarcador = agendamentoColeta.Sequencia.ToString();
                else
                    carga.CodigoCargaEmbarcador = pedido.CodigoCargaEmbarcador;

                if (pedido.AdicionadaManualmente && (carga.NumeroSequenciaCarga == null || carga.NumeroSequenciaCarga == 0))
                {
                    int.TryParse(pedido.CodigoCargaEmbarcador, out int sequencialCarga);

                    carga.NumeroSequenciaCarga = sequencialCarga;
                    carga.Operador = pedido.Usuario;
                }


                carga.TipoOperacao = pedido.TipoOperacao;
                carga.DataCarregamentoCarga = pedido.DataCarregamentoPedido;

                if (pedido.DataCarregamentoCarga.HasValue)
                    carga.DataCarregamentoCarga = pedido.DataCarregamentoCarga;

                if (carga.TipoOperacao != null)
                {
                    carga.ExigeNotaFiscalParaCalcularFrete = carga.TipoOperacao.ExigeNotaFiscalParaCalcularFrete;
                    carga.NaoExigeVeiculoParaEmissao = carga.TipoOperacao.NaoExigeVeiculoParaEmissao;

                    if (carga.TipoOperacao.FretePorContadoCliente)
                        carga.TipoFreteEscolhido = TipoFreteEscolhido.Cliente;
                }
                else
                {
                    carga.ExigeNotaFiscalParaCalcularFrete = configuracao.ExigirNotaFiscalParaCalcularFreteCarga;
                    carga.NaoExigeVeiculoParaEmissao = false;
                }

                carga.PossuiOperacaoContainer = carga.TipoOperacao?.ConfiguracaoContainer?.GestaoViagemContainerFluxoUnico ?? false;
                carga.RealizandoOperacaoContainer = carga.TipoOperacao?.ConfiguracaoContainer?.GestaoViagemContainerFluxoUnico ?? false;

                carga.ModeloVeicularCarga = modeloVeicularCarga;
                carga.TipoDeCarga = tipoDeCarga;
                carga.CargaTransbordo = pedido.PedidoTransbordo;
                carga.PortoDestino = pedido.PortoDestino;
                carga.PortoOrigem = pedido.Porto;
                carga.TerminalDestino = pedido.TerminalDestino;
                carga.ObservacaoParaFaturamento = pedido.ObservacaoParaFaturamento;
                carga.TerminalOrigem = pedido.TerminalOrigem;
                carga.PedidoViagemNavio = pedido.PedidoViagemNavio;
                carga.CargaSVMTerceiro = pedido.PedidoDeSVMTerceiro;
                carga.CargaTakeOrPay = pedido.PedidoTakeOrPay;
                carga.CargaDemurrage = pedido.PedidoDemurrage;
                carga.CargaDetention = pedido.PedidoDetention;
                carga.CargaDestinadaCTeComplementar = pedido?.TipoOperacao?.OperacaoDestinadaCTeComplementar ?? false;
                carga.TipoServicoCarga = pedido.TipoServicoCarga;
                carga.StatusCustoExtra = StatusCustoExtra.EmAberto;
                carga.TipoOS = pedido.TipoOS;
                carga.TipoOSConvertido = pedido.TipoOSConvertido;
                carga.CategoriaOS = pedido.CategoriaOS;
                carga.DirecionamentoCustoExtra = pedido.TipoOperacao?.ConfiguracaoCarga?.DirecionamentoCustoExtra ?? TipoDirecionamentoCustoExtra.Nenhum;

                if (carga.CargaDestinadaCTeComplementar)
                {
                    string numeroOSMae = repPedido.BuscarNumeroOSMae(pedido.Codigo);
                    if (!string.IsNullOrWhiteSpace(numeroOSMae))
                        serCarga.VincularMotoristaVeiculosOSMae(carga, pedido, numeroOSMae, unitOfWork, TipoServicoMultisoftware, configuracao);
                }

                if ((configuracaoContratoFreteTerceiro?.GerarCargaTerceiroApenasProvedorPedido ?? false) && pedido?.ProvedorOS != null)
                    carga.ProvedorOS = pedido.ProvedorOS;

                if (pedido?.TipoOperacao != null)
                    carga.CargaBloqueadaParaEdicaoIntegracao = pedido?.TipoOperacao?.CargaBloqueadaParaEdicaoIntegracao ?? false;

                if (carga.CargaSVMTerceiro)
                    carga.CargaSVM = false;

                if (configuracao.UtilizaEmissaoMultimodal)
                {
                    carga.NaoExigeVeiculoParaEmissao = true;
                    carga.NaoGerarMDFe = true;
                }

                retorno = serCarga.CriarCargaPorPedidos(ref carga, pedidos, TipoServicoMultisoftware, null, unitOfWork, configuracao);

                if (string.IsNullOrWhiteSpace(retorno))
                {
                    if (pedido.Motoristas != null && pedido.Motoristas.Count() > 0)
                        new Servicos.Embarcador.Carga.CargaMotorista(unitOfWork).AdicionarMotoristas(carga, pedido.Motoristas.ToList());
                    else
                    {

                        Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
                        {
                            TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema,
                            OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema,
                        };

                        serCarga.PreencherMotoristaGenericoCarga(carga, auditado, unitOfWork);
                    }
                    //AQUI

                    if (cadastroPedido && configuracao.PermitirSelecionarReboquePedido)
                    {
                        carga.Veiculo = pedido.VeiculoTracao;

                        if (carga.Veiculo != null && carga.Veiculo.Empresa != null && !(configuracaoPedido?.NaoSubstituirEmpresaNaGeracaoCarga ?? false))
                            carga.Empresa = carga.Veiculo.Empresa;

                        if (carga.ModeloVeicularCarga == null && carga.Veiculo?.ModeloVeicularCarga != null && carga.Veiculo.ModeloVeicularCarga.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModeloVeicularCarga.Tracao)
                            carga.ModeloVeicularCarga = carga.Veiculo.ModeloVeicularCarga;

                        if (carga.VeiculosVinculados != null)
                            carga.VeiculosVinculados.Clear();
                        else
                            carga.VeiculosVinculados = new List<Dominio.Entidades.Veiculo>();

                        if (pedido.Veiculos != null)
                        {
                            foreach (Dominio.Entidades.Veiculo reboque in pedido.Veiculos)
                            {
                                if (carga.ModeloVeicularCarga == null && reboque.ModeloVeicularCarga != null && reboque.ModeloVeicularCarga.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModeloVeicularCarga.Tracao)
                                    carga.ModeloVeicularCarga = reboque.ModeloVeicularCarga;

                                carga.VeiculosVinculados.Add(reboque);
                            }
                        }
                    }
                    else
                    {
                        if (pedido.Veiculos != null)
                        {
                            foreach (Dominio.Entidades.Veiculo veiculo in pedido.Veiculos)
                            {
                                if (veiculo.TipoVeiculo == "0")
                                    carga.Veiculo = veiculo;
                                else
                                {
                                    if (veiculo.VeiculosTracao != null && veiculo.VeiculosTracao.Count > 0)
                                        carga.Veiculo = veiculo.VeiculosTracao.FirstOrDefault();
                                    else
                                        carga.Veiculo = veiculo;
                                }

                                if (carga.Veiculo != null && carga.Veiculo.Empresa != null && !(configuracaoPedido?.NaoSubstituirEmpresaNaGeracaoCarga ?? false))
                                    carga.Empresa = carga.Veiculo.Empresa;

                                if (carga.ModeloVeicularCarga == null && carga.Veiculo?.ModeloVeicularCarga != null && carga.Veiculo.ModeloVeicularCarga.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModeloVeicularCarga.Tracao)
                                    carga.ModeloVeicularCarga = carga.Veiculo.ModeloVeicularCarga;

                                if (carga.Veiculo.VeiculosVinculados != null)
                                {
                                    if (carga.VeiculosVinculados != null)
                                        carga.VeiculosVinculados.Clear();
                                    else
                                        carga.VeiculosVinculados = new List<Dominio.Entidades.Veiculo>();

                                    foreach (Dominio.Entidades.Veiculo reboque in carga.Veiculo.VeiculosVinculados)
                                    {
                                        if (carga.ModeloVeicularCarga == null && reboque.ModeloVeicularCarga != null && reboque.ModeloVeicularCarga.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModeloVeicularCarga.Tracao)
                                            carga.ModeloVeicularCarga = reboque.ModeloVeicularCarga;

                                        carga.VeiculosVinculados.Add(reboque);
                                    }

                                }
                            }
                        }
                    }

                    if (carga.TipoOperacao != null)
                    {
                        if (carga.TipoOperacao.UsaJanelaCarregamentoPorEscala && carga.Empresa != null && carga.Veiculo != null)
                        {
                            Repositorio.Embarcador.PreCargas.PreCarga repPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(unitOfWork);
                            DateTime dataEscala = carga.DataCarregamentoCarga.HasValue ? carga.DataCarregamentoCarga.Value : carga.DataCriacaoCarga;
                            Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga = repPreCarga.BuscarPorEscala(carga.Veiculo.Codigo, carga.Empresa.Codigo, carga.Filial?.Codigo ?? 0, dataEscala);

                            if (preCarga != null)
                            {
                                Servicos.Embarcador.Logistica.CargaJanelaCarregamento servicoCargaJanelaCarregamento = new Servicos.Embarcador.Logistica.CargaJanelaCarregamento(unitOfWork, configuracao);
                                servicoCargaJanelaCarregamento.DefinirCargaPorPreCarga(carga, preCarga);
                            }
                        }

                        if (carga.TipoOperacao.FretePorContadoCliente)
                        {
                            carga.TipoFreteEscolhido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Cliente;
                        }
                    }

                    if (carga.Rota == null)
                        carga.Rota = pedido.RotaFrete;

                    new Servicos.Embarcador.Logistica.RestricaoRodagem(unitOfWork).ValidaAtualizaZonaExclusaoRota(carga.Rota);

                    serCarga.FecharCarga(carga, unitOfWork, TipoServicoMultisoftware, ClienteMultisoftware, recriarRotas: false, adicionarJanelaDescarregamento: adicionarJanelaDescarregamento, adicionarJanelaCarregamento: true, validarDados: false, gerarAgendamentoColeta: gerarAgendamentoColeta);
                    Servicos.Log.TratarErro("13 - Fechou Carga (" + carga.Codigo + ") " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "FechamentoCarga");
                    carga.CargaFechada = true;
                    carga.Protocolo = carga.Codigo;
                    carga.CargaSVM = pedido.PedidoSVM;
                    carga.CargaTakeOrPay = pedido.PedidoTakeOrPay;
                    carga.CargaDemurrage = pedido.PedidoDemurrage;
                    carga.CargaDetention = pedido.PedidoDetention;
                    carga.CargaSVMTerceiro = pedido.PedidoDeSVMTerceiro;
                    carga.CargaDestinadaCTeComplementar = pedido?.TipoOperacao?.OperacaoDestinadaCTeComplementar ?? false;
                    if (carga.CargaSVMTerceiro)
                        carga.CargaSVM = false;

                    if (carga.CargaDestinadaCTeComplementar)
                    {
                        string numeroOSMae = repPedido.BuscarNumeroOSMae(pedido.Codigo);
                        if (!string.IsNullOrWhiteSpace(numeroOSMae))
                            serCarga.VincularMotoristaVeiculosOSMae(carga, pedido, numeroOSMae, unitOfWork, TipoServicoMultisoftware, configuracao);
                    }

                    repCarga.Atualizar(carga);

                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorCarga(carga);

                    Servicos.Embarcador.Seguro.Seguro.SetarDadosSeguroCarga(carga, cargaPedidos, configuracao, TipoServicoMultisoftware, unitOfWork);

                    GerarOcorrenciasColetaEntrega(pedidos, carga, carga.TipoOperacao, TipoGatilhoPedidoOcorrenciaColetaEntrega.CriacaoPedido, configuracao, null, unitOfWork);

                    serCarga.AtualizarDataEstufagemDadosTransporteMaritimo(carga, unitOfWork, true);
                }
            }

            return retorno;
        }

        public static void GerarOcorrenciasColetaEntrega(List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, TipoGatilhoPedidoOcorrenciaColetaEntrega gatilho, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            if ((tipoOperacao == null) || (pedidos == null) || (pedidos.Count == 0))
                return;

            Repositorio.Embarcador.Pedidos.GatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntrega repositorioGatilhoGeracaoAutomatica = new Repositorio.Embarcador.Pedidos.GatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntrega(unitOfWork);
            List<Dominio.Entidades.Embarcador.Pedidos.GatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntrega> listaGatilhoGeracaoAutomatica = repositorioGatilhoGeracaoAutomatica.BuscarPorTipoOperacaoEGatilho(tipoOperacao.Codigo, gatilho);

            if (listaGatilhoGeracaoAutomatica.Count == 0)
                return;

            Dominio.Entidades.Embarcador.GestaoEntregas.ConfiguracaoPortalCliente configuracaoPortalCliente = Servicos.Embarcador.GestaoEntregas.ConfiguracaoPortalCliente.ObterConfiguracao(unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidos)
            {
                Dominio.Entidades.Cliente tomador = pedido.ObterTomador() ?? pedido.Remetente;

                foreach (Dominio.Entidades.Embarcador.Pedidos.GatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntrega gatilhoGeracaoAutomatica in listaGatilhoGeracaoAutomatica)
                {
                    try
                    {
                        string observacao = gatilhoGeracaoAutomatica.Observacao
                            .Replace("#Destinatario", pedido.Destinatario?.Descricao ?? "")
                            .Replace("#Destino", pedido.Destinatario?.Localidade?.Descricao ?? "")
                            .Replace("#Origem", tomador?.Descricao ?? "")
                            .Replace("#Remetente", tomador?.Localidade?.Descricao ?? "");

                        Servicos.Embarcador.Carga.ControleEntrega.OcorrenciaEntrega.GerarPedidoOcorrenciaColetaEntrega(tomador, pedido, carga, gatilhoGeracaoAutomatica.TipoOcorrencia, configuracaoPortalCliente, observacao, configuracao, clienteMultisoftware, unitOfWork);
                    }
                    catch (ServicoException excecao)
                    {
                        Log.TratarErro(excecao);
                    }
                }
            }
        }

        public static void CriarCargaSVM(List<int> codigosPedidos, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, int codigoUsuario)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);
            try
            {
                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaMotorista repCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                int qtdProcessamentoLote = configuracaoTMS.QuantidadeCargaPedidoProcessamentoLote;

                unitOfWork.Start();

                int contador = 0;
                foreach (int codigoPedido in codigosPedidos)
                {
                    string msgRetorno = "";
                    Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repPedido.BuscarPorCodigo(codigoPedido);
                    Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorCodigo(codigoUsuario);

                    msgRetorno = Servicos.Embarcador.Pedido.Pedido.CriarCarga(pedido, unitOfWork, tipoServicoMultisoftware, clienteMultisoftware, configuracaoTMS, true);

                    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPrimeiraPorPedido(pedido.Codigo);

                    if (cargaPedido == null)
                    {
                        Servicos.Log.TratarErro("Não foi possível localizar a carga gerada para este pedido. Por favor tente novamente.", "CriarCargaSVM");
                        continue;
                    }

                    List<Dominio.Entidades.Embarcador.Cargas.CargaMotorista> motoristasCarga = repCargaMotorista.BuscarPorCarga(cargaPedido.Carga.Codigo);

                    Servicos.Embarcador.Integracao.IntegracaoCarga.AdicionarIntegracoesCargaDadosTransporte(cargaPedido.Carga, pedido.PedidosCarga.ToList(), motoristasCarga, configuracaoTMS, tipoServicoMultisoftware, unitOfWork);

                    cargaPedido.TipoCobrancaMultimodal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCobrancaMultimodal.CTEAquaviario;
                    cargaPedido.ModalPropostaMultimodal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalPropostaMultimodal.PortoPorto;
                    cargaPedido.TipoServicoMultimodal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoServicoMultimodal.VinculadoMultimodalProprio;
                    cargaPedido.TipoPropostaMultimodal = pedido.TipoPropostaMultimodal;

                    if (pedido.Expedidor != null && pedido.Recebedor != null)
                        cargaPedido.TipoEmissaoCTeParticipantes = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor;
                    else if (pedido.Expedidor != null)
                        cargaPedido.TipoEmissaoCTeParticipantes = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidor;
                    else if (pedido.Recebedor != null)
                        cargaPedido.TipoEmissaoCTeParticipantes = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComRecebedor;

                    cargaPedido.Carga.TipoServicoCarga = TipoServicoCarga.SVMProprio;

                    repCargaPedido.Atualizar(cargaPedido);
                    repCarga.Atualizar(cargaPedido.Carga);

                    msgRetorno = "";
                    int qtdDocumentos = 0;
                    int codigoCarga = cargaPedido?.Carga?.Codigo ?? 0;
                    AdicionarDocumentosAnteriores(pedido.PossuiCargaPerigosa, pedido.NumeroBooking, pedido.TipoPropostaMultimodal, out msgRetorno, pedido, cargaPedido, unitOfWork, ref qtdDocumentos);
                    if (!string.IsNullOrWhiteSpace(msgRetorno))
                    {
                        Servicos.Log.TratarErro(msgRetorno, "CriarCargaSVM");
                        //if (codigoCarga > 0)
                        //{
                        //    Dominio.Entidades.Embarcador.Cargas.Carga cargaCancelar = repCarga.BuscarPorCodigo(codigoCarga);
                        //    if (cargaCancelar != null)
                        //    {
                        //        cargaCancelar.DataAtualizacaoCarga = DateTime.Now;
                        //        cargaCancelar.DataCriacaoCarga = DateTime.Now;
                        //        cargaCancelar.SituacaoCarga = SituacaoCarga.Cancelada;

                        //        repCarga.Atualizar(cargaCancelar);
                        //        contador++;
                        //    }
                        //}
                        //Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, "Confirmou o envio dos documentos.", unitOfWork);

                        unitOfWork.FlushAndClear();

                        continue;
                    }
                    //repPedido.Atualizar(pedido);

                    Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(cargaPedido.Carga.Codigo);
                    usuario = repUsuario.BuscarPorCodigo(codigoUsuario);
                    msgRetorno = "";
                    //ConfirmarEnvioDocumentos(out msgRetorno, carga, unitOfWork, stringConexao, tipoServicoMultisoftware, usuario, ((contador % 2) == 0));
                    bool gerarLote = ((contador % 2) == 0);
                    if (!gerarLote && (qtdDocumentos >= qtdProcessamentoLote))
                        gerarLote = true;

                    ConfirmarEnvioDocumentos(out msgRetorno, carga, unitOfWork, stringConexao, tipoServicoMultisoftware, usuario, gerarLote ? LoteCalculoFrete.Integracao : LoteCalculoFrete.Padrao);
                    if (!string.IsNullOrWhiteSpace(msgRetorno))
                    {
                        Servicos.Log.TratarErro(msgRetorno, "CriarCargaSVM");
                        continue;
                    }
                    carga.DataAtualizacaoCarga = DateTime.Now;
                    carga.DataCriacaoCarga = DateTime.Now;

                    repCarga.Atualizar(carga);
                    contador++;
                    //Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, "Confirmou o envio dos documentos.", unitOfWork);

                    unitOfWork.FlushAndClear();

                }

                unitOfWork.CommitChanges();
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex, "CriarCargaSVM");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private static void AdicionarDocumentosAnteriores(bool somenteCargaPerigosa, string numeroBooking, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal tipoPropostaMultimodal, out string erro, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Repositorio.UnitOfWork unitOfWork, ref int qtdDocumentos)
        {
            erro = "";
            qtdDocumentos = 0;
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe repCargaPedidoDocumentoCTe = new Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[] situacoesPermitidas = new Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[]
                   {
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgIntegracao,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.PendeciaDocumentos
                   };
            bool possuiCTe = false;
            decimal valorFrete = 0;
            int codigoCargaPedido = cargaPedido.Codigo;

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repCargaCTe.ConsultarMultiModal(somenteCargaPerigosa, numeroBooking, tipoPropostaMultimodal, pedido.PedidoViagemNavio?.Codigo ?? 0, pedido.TerminalOrigem?.Codigo ?? 0, pedido.TerminalDestino?.Codigo ?? 0, situacoesPermitidas, codigoCargaSVM: pedido.CargaSVM?.Codigo ?? 0);
            if (cargaCTes != null && cargaCTes.Count > 0)
            {
                possuiCTe = true;
                List<int> codigosCTes = cargaCTes.Select(c => c.CTe.Codigo).Distinct().ToList();
                int ordem = 1;
                for (int i = 0; i < codigosCTes.Count; i++)
                {
                    unitOfWork.Start();

                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigosCTes[i]);
                    valorFrete += cte.ValorAReceber;
                    Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe cargaPedidoDocumentoCTe = repCargaPedidoDocumentoCTe.BuscarPorCTeECargaPedido(cte.Codigo, codigoCargaPedido);
                    if (cargaPedidoDocumentoCTe == null)
                    {
                        cargaPedidoDocumentoCTe = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe
                        {
                            CargaPedido = cargaPedido,
                            CTe = cte,
                            Ordem = ordem
                        };
                        repCargaPedidoDocumentoCTe.Inserir(cargaPedidoDocumentoCTe);
                        ordem++;
                    }
                    qtdDocumentos++;
                    unitOfWork.CommitChanges();

                    unitOfWork.FlushAndClear();
                }
            }

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTesTransbordo = repCargaCTe.ConsultarMultiModalTransbordo(somenteCargaPerigosa, numeroBooking, tipoPropostaMultimodal, pedido.PedidoViagemNavio?.Codigo ?? 0, pedido.TerminalOrigem?.Codigo ?? 0, pedido.TerminalDestino?.Codigo ?? 0, situacoesPermitidas, codigoCargaSVM: pedido.CargaSVM?.Codigo ?? 0);
            if (cargaCTesTransbordo != null && cargaCTesTransbordo.Count > 0)
            {
                possuiCTe = true;
                List<int> codigosCTes = cargaCTesTransbordo.Select(c => c.CTe.Codigo).Distinct().ToList();
                int ordem = 1;
                for (int i = 0; i < codigosCTes.Count; i++)
                {
                    unitOfWork.Start();

                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigosCTes[i]);
                    valorFrete += cte.ValorAReceber;
                    Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe cargaPedidoDocumentoCTe = repCargaPedidoDocumentoCTe.BuscarPorCTeECargaPedido(cte.Codigo, codigoCargaPedido);
                    if (cargaPedidoDocumentoCTe == null)
                    {
                        cargaPedidoDocumentoCTe = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe
                        {
                            CargaPedido = cargaPedido,
                            CTe = cte,
                            Ordem = ordem
                        };
                        repCargaPedidoDocumentoCTe.Inserir(cargaPedidoDocumentoCTe);
                        ordem++;
                    }
                    qtdDocumentos++;
                    unitOfWork.CommitChanges();

                    unitOfWork.FlushAndClear();
                }
            }

            if (!possuiCTe)
                erro = "Não foi encontrado nenhum CT-e MTL para este carregamento.";

            //validar se tem cte
            pedido = repPedido.BuscarPorCodigo(pedido.Codigo);
            pedido.ValorFreteAReceber = valorFrete;
            pedido.ValorFreteCotado = valorFrete;
            pedido.ValorFreteNegociado = valorFrete;
            pedido.ValorFreteFilialEmissora = valorFrete;
            repPedido.Atualizar(pedido);
        }

        private static void ConfirmarEnvioDocumentos(out string erro, Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Usuario usuario, LoteCalculoFrete calcularFretePorLote)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

            Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            Servicos.Embarcador.Carga.RateioFormula serRateioFormula = new Servicos.Embarcador.Carga.RateioFormula(unitOfWork);

            erro = "";
            if (carga.ProcessandoDocumentosFiscais)
            {
                erro = "A carga já está processando os documentos fiscais para avançar para a próxima etapa, aguarde.";
                return;
            }

            new Servicos.Embarcador.Carga.CargaOperador(unitOfWork).Atualizar(carga, usuario, tipoServicoMultisoftware);

            carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe;
            carga.NaoGerarMDFe = true;
            carga.CalcularFreteLote = calcularFretePorLote;
            if (!carga.CargaDestinadaCTeComplementar)
                carga.NaoExigeVeiculoParaEmissao = true;
            carga.ProcessandoDocumentosFiscais = true;
            carga.DataInicioConfirmacaoDocumentosFiscais = DateTime.Now;
        }

        public static bool ValidarCancelamentoPedido(out string erro, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Repositorio.UnitOfWork unitOfWork)
        {
            if (pedido == null)
            {
                erro = "Protocolo do pedido não localizado.";
                return false;
            }

            Repositorio.Embarcador.Pedidos.PedidoCancelamento repPedidoCancelamento = new Repositorio.Embarcador.Pedidos.PedidoCancelamento(unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.PedidoCancelamento pedidoCancelamento = repPedidoCancelamento.BuscarPorPedido(pedido.Codigo);

            if (pedidoCancelamento != null)
            {
                erro = "Já existe um cancelamento registrado para este pedido em " + pedidoCancelamento.DataCancelamento.ToString("dd/MM/yyyy HH:mm:ss") + ".";
                return false;
            }

            SituacaoPedido[] situacoesPedidoValidar = new SituacaoPedido[]
            {
                 SituacaoPedido.AgAprovacao,
                 SituacaoPedido.AutorizacaoPendente,
                 SituacaoPedido.Cancelado,
                 SituacaoPedido.DesistenciaCarga,
                 SituacaoPedido.DesistenciaCarregamento
            };

            if (situacoesPedidoValidar.Contains(pedido.SituacaoPedido))
            {
                erro = "A situação do pedido (" + pedido.DescricaoSituacaoPedido + ") não permite que o mesmo seja cancelado.";
                return false;
            }

            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasDoPedido = pedido.CargasPedido.Where(o => o.SituacaoCarga != SituacaoCarga.Cancelada && o.SituacaoCarga != SituacaoCarga.Anulada).ToList();

            if (cargasDoPedido.Count > 1)
            {
                erro = "O pedido está vinculado a mais de uma carga, não sendo possível cancelar o mesmo.";
                return false;
            }
            else
            {
                Dominio.Entidades.Embarcador.Cargas.Carga carga = cargasDoPedido.FirstOrDefault();

                if (carga != null)
                {
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[] situacoesCargaPermitidas = new Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[]
                    {
                             Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova,
                             Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe,
                             Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete
                    };

                    if (!situacoesCargaPermitidas.Contains(carga.SituacaoCarga))
                    {
                        erro = "A situação da carga (" + carga.DescricaoSituacaoCarga + ") não permite que o pedido seja cancelado.";
                        return false;
                    }
                    else if (carga.Pedidos.Count > 1)
                    {
                        erro = "A carga " + carga.CodigoCargaEmbarcador + " possui mais de um pedido vinculado à ela, não sendo possível cancelar o pedido.";
                        return false;
                    }
                }
            }

            erro = string.Empty;
            return true;
        }

        public static bool CancelarPedido(out string erro, int codigoPedido, TipoPedidoCancelamento tipoCancelamento, Dominio.Entidades.Usuario usuario, string motivo, UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, int protocoloPedido)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = null;

            if (protocoloPedido > 0)
                pedido = repositorioPedido.BuscarPorProtocolo(protocoloPedido);
            else
                pedido = repositorioPedido.BuscarPorCodigo(codigoPedido);

            return CancelarPedido(out erro, pedido, tipoCancelamento, usuario, motivo, unitOfWork, tipoServicoMultisoftware, auditado, configuracao, clienteMultisoftware);
        }

        public static bool CancelarPedido(out string erro, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, TipoPedidoCancelamento tipoCancelamento, Dominio.Entidades.Usuario usuario, string motivo, UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
        {
            if (!ValidarCancelamentoPedido(out erro, pedido, unitOfWork))
                return false;

            switch (tipoCancelamento)
            {
                case TipoPedidoCancelamento.Cancelamento:
                    return RealizarCancelamentoPedido(out erro, ref pedido, usuario, motivo, unitOfWork, tipoServicoMultisoftware, clienteMultisoftware, auditado);

                case TipoPedidoCancelamento.DesistenciaCarga:
                    return RealizarDesistenciaCarga(out erro, ref pedido, usuario, motivo, unitOfWork, tipoServicoMultisoftware, auditado, configuracao);

                case TipoPedidoCancelamento.DesistenciaCarregamento:
                    return RealizarDesistenciaCarregamento(out erro, ref pedido, usuario, motivo, unitOfWork, tipoServicoMultisoftware, auditado, configuracao);

                default:
                    erro = "Tipo de cancelamento não implementado.";
                    return false;
            }
        }

        public static void RemoverPedidoCancelado(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorPedidoComCargaAtiva(pedido.Codigo);

            if (cargaPedidos.Count == 0)
                return;

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                if (!cargaPedido.Carga.CargaDePreCarga)
                    throw new ServicoException("O pedido já está vinculado à uma carga, não sendo possível realizar a exclusão.");

                Carga.CargaPedido.RemoverPedidoCarga(cargaPedido.Carga, cargaPedido, configuracaoEmbarcador, tipoServicoMultisoftware, unitOfWork, configuracaoGeralCarga);
                Servicos.Auditoria.Auditoria.Auditar(auditado, cargaPedido.Carga, null, $"Removido o pedido {cargaPedido.Pedido.NumeroPedidoEmbarcador} após cancelamento via integração", unitOfWork);
            }

            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = cargaPedidos.Select(o => o.Carga).Distinct().ToList();

            foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargas)
            {
                carga.SituacaoRoteirizacaoCarga = SituacaoRoteirizacao.Aguardando;
                repositorioCarga.Atualizar(carga);
            }

            Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga servicoMontagemCarga = new Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga(unitOfWork, configuracaoEmbarcador);
            Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repositorioCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repositorioCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento> carregamentos = repositorioCarregamento.BuscarPorPedido(pedido.Codigo);

            foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento in carregamentos)
            {
                if (!carregamento.SituacaoCarregamento.CarregamentoPendente())
                    continue;

                int totalPedidosPorCarregamento = repositorioCarregamentoPedido.ContarPorCarregamento(carregamento.Codigo);

                if (totalPedidosPorCarregamento > 1)
                    servicoMontagemCarga.RemoverPedido(carregamento, pedido);
                else
                    repositorioCarregamento.ExcluirCarregamento(carregamento.Codigo, true);
            }
        }

        private static bool RealizarCancelamentoPedido(out string erro, ref Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.Entidades.Usuario usuario, string motivo, UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            Servicos.Embarcador.Pedido.OcorrenciaPedido servOcorrenciaPedido = new Servicos.Embarcador.Pedido.OcorrenciaPedido(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();

            CriarCancelamentoPedido(TipoPedidoCancelamento.Cancelamento, pedido, usuario, motivo, unitOfWork, auditado);

            if (pedido.CotacaoPedido != null && tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                pedido.AguardandoIntegracao = true;
                pedido.CotacaoPedido = null;
            }

            pedido.SituacaoPedido = SituacaoPedido.Cancelado;
            pedido.ControleNumeracao = pedido.Codigo;

            repPedido.Atualizar(pedido);

            servOcorrenciaPedido.ProcessarOcorrenciaPedido(EventoColetaEntrega.PedidoCancelado, pedido, configuracao, clienteMultisoftware);
            new Servicos.Embarcador.Integracao.IntegracaoPedidoRoterizador(unitOfWork).AdicionarParaIntegracaoAutomaticamente(pedido.Codigo, TipoRoteirizadorIntegracao.CancelarPedido);

            Servicos.Auditoria.Auditoria.Auditar(auditado, pedido, null, "Solicitou cancelamento do pedido", unitOfWork);

            erro = string.Empty;
            return true;
        }

        private static bool RealizarDesistenciaCarregamento(out string erro, ref Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.Entidades.Usuario usuario, string motivo, UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();



            CriarCancelamentoPedido(TipoPedidoCancelamento.DesistenciaCarregamento, pedido, usuario, motivo, unitOfWork, auditado);

            if (pedido.CargasPedido.Count <= 0)
            {
                if (!configuracaoTMS.NumeroCargaSequencialUnico)
                    pedido.CodigoCargaEmbarcador = Servicos.Embarcador.Cargas.CargaSequencial.GetInstance().ObterProximoNumeroSequencial(unitOfWork, pedido.Filial?.Codigo ?? 0).ToString();
                else
                    pedido.CodigoCargaEmbarcador = Servicos.Embarcador.Cargas.CargaSequencial.GetInstance().ObterProximoNumeroSequencial(unitOfWork).ToString();

                pedido.AdicionadaManualmente = true;

                erro = CriarCarga(pedido, unitOfWork, tipoServicoMultisoftware, null, configuracao, true);

                if (!string.IsNullOrWhiteSpace(erro))
                    return false;
            }

            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorPedido(pedido.Codigo).FirstOrDefault();

            cargaPedido.Carga.Desistencia = true;
            cargaPedido.Carga.PercentualDesistencia = pedido.TipoOperacao.PercentualCobrarDesistenciaCarregamento;

            repCarga.Atualizar(cargaPedido.Carga);

            pedido.SituacaoPedido = SituacaoPedido.DesistenciaCarregamento;

            repPedido.Atualizar(pedido);

            Servicos.Auditoria.Auditoria.Auditar(auditado, pedido, null, "Solicitou desistencia do carregamento", unitOfWork);

            erro = string.Empty;
            return true;
        }

        private static bool RealizarDesistenciaCarga(out string erro, ref Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.Entidades.Usuario usuario, string motivo, UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            CriarCancelamentoPedido(TipoPedidoCancelamento.DesistenciaCarga, pedido, usuario, motivo, unitOfWork, auditado);

            if (pedido.CargasPedido.Count <= 0)
            {
                if (!configuracaoTMS.NumeroCargaSequencialUnico)
                    pedido.CodigoCargaEmbarcador = Servicos.Embarcador.Cargas.CargaSequencial.GetInstance().ObterProximoNumeroSequencial(unitOfWork, pedido.Filial?.Codigo ?? 0).ToString();
                else
                    pedido.CodigoCargaEmbarcador = Servicos.Embarcador.Cargas.CargaSequencial.GetInstance().ObterProximoNumeroSequencial(unitOfWork).ToString();

                pedido.AdicionadaManualmente = true;

                erro = CriarCarga(pedido, unitOfWork, tipoServicoMultisoftware, null, configuracao, true);

                if (!string.IsNullOrWhiteSpace(erro))
                    return false;
            }

            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorPedido(pedido.Codigo).FirstOrDefault();

            cargaPedido.Carga.Desistencia = true;
            cargaPedido.Carga.PercentualDesistencia = pedido.TipoOperacao.PercentualCobrarDesistenciaCarga;

            repCarga.Atualizar(cargaPedido.Carga);

            pedido.SituacaoPedido = SituacaoPedido.DesistenciaCarga;

            repPedido.Atualizar(pedido);

            Servicos.Auditoria.Auditoria.Auditar(auditado, pedido, null, "Solicitou desistencia da carga", unitOfWork);

            erro = string.Empty;
            return true;
        }

        private static void CriarCancelamentoPedido(TipoPedidoCancelamento tipo, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.Entidades.Usuario usuario, string motivo, UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Embarcador.Pedidos.PedidoCancelamento repPedidoCancelamento = new Repositorio.Embarcador.Pedidos.PedidoCancelamento(unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.PedidoCancelamento pedidoCancelamento = new Dominio.Entidades.Embarcador.Pedidos.PedidoCancelamento()
            {
                DataCancelamento = DateTime.Now,
                MotivoCancelamento = motivo,
                Pedido = pedido,
                Tipo = tipo,
                Usuario = usuario,
                Situacao = SituacaoPedidoCancelamento.Cancelado
            };

            repPedidoCancelamento.Inserir(pedidoCancelamento);

            Servicos.Auditoria.Auditoria.Auditar(auditado, pedidoCancelamento, null, null, unitOfWork);
        }

        public static void GerarPedidoHistorico(Dominio.Entidades.Embarcador.Cargas.Carga carga, string historico, UnitOfWork unitOfWork)
        {
            if (carga == null)
                return;

            Repositorio.Embarcador.Pedidos.PedidoHistorico repPedidoHistorico = new Repositorio.Embarcador.Pedidos.PedidoHistorico(unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in carga.Pedidos)
            {
                Dominio.Entidades.Embarcador.Pedidos.PedidoHistorico pedidoHistorico = new Dominio.Entidades.Embarcador.Pedidos.PedidoHistorico()
                {
                    Data = DateTime.Now,
                    Historico = historico,
                    Pedido = cargaPedido.Pedido
                };


                repPedidoHistorico.Inserir(pedidoHistorico);

            }
        }

        public static void GerarPedidoHistorico(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, string historico, UnitOfWork unitOfWork)
        {
            if (cargaEntrega == null)
                return;

            Repositorio.Embarcador.Pedidos.PedidoHistorico repPedidoHistorico = new Repositorio.Embarcador.Pedidos.PedidoHistorico(unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido pedido in cargaEntrega.Pedidos)
            {
                Dominio.Entidades.Embarcador.Pedidos.PedidoHistorico pedidoHistorico = new Dominio.Entidades.Embarcador.Pedidos.PedidoHistorico()
                {
                    Data = DateTime.Now,
                    Historico = historico,
                    Pedido = pedido.CargaPedido.Pedido
                };


                repPedidoHistorico.Inserir(pedidoHistorico);

            }
        }

        //public static void SetarSituacaoPedido(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, DateTime data, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido situacaoPedido, UnitOfWork unitOfWork)
        //{
        //    if (cargaEntrega == null)
        //        return;

        //    Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

        //    bool coleta = cargaEntrega.Coleta;

        //    foreach (var cargaEntregaPedido in cargaEntrega.Pedidos)
        //    {
        //        cargaEntregaPedido.CargaPedido.Pedido.DataEntrega = data;
        //        cargaEntregaPedido.CargaPedido.Pedido.SituacaoPedido = situacaoPedido;

        //        if (situacaoPedido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Finalizado)
        //            cargaEntregaPedido.CargaPedido.Pedido.DataEntrega = data;
        //        else if (situacaoPedido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Rejeitado)
        //            cargaEntregaPedido.CargaPedido.Pedido.DataEntrega = null;

        //        if (coleta)
        //            cargaEntregaPedido.CargaPedido.Pedido.EtapaAcompanhamentoPedido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAcompanhamentoPedido.Coleta;
        //        else
        //            cargaEntregaPedido.CargaPedido.Pedido.EtapaAcompanhamentoPedido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAcompanhamentoPedido.Entrega;
        //    }
        //}

        public static void GerarPrevisaoColetaEntrega(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, UnitOfWork unitOfWork)
        {
            // 1 - Pegar o Centro de Descarregamento.;
            // 2 - Pegar a janela de entrega do destinatário...
            //if ((pedido?.RotaFrete?.Codigo ?? 0) > 0 && (pedido?.Destinatario?.Codigo ?? 0) > 0)
            if ((pedido?.Destinatario?.Codigo ?? 0) > 0)
            {
                Repositorio.Embarcador.Logistica.CentroDescarregamento repCentroDescarregamento = new Repositorio.Embarcador.Logistica.CentroDescarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamento = repCentroDescarregamento.BuscarPorDestinatario(pedido?.Destinatario?.Codigo ?? 0);
                if (centroDescarregamento != null)
                {
                    List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento> janelas = centroDescarregamento?.PeriodosDescarregamento?.ToList();
                    if (janelas?.Count > 0)
                    {
                        //Vamos verificar o remetente.. se o pedido possuir um remetente no qual a janela está configurada.. vamos pegar essa...
                        List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento> janelasRemetente = janelas.FindAll(x => (x?.Remetentes?.Any(e => e?.Remetente?.Codigo == pedido?.Remetente?.Codigo) ?? false) &&
                                                                                                                                    (x?.TiposDeCarga?.Any(t => t?.TipoDeCarga?.Codigo == pedido?.TipoDeCarga?.Codigo) ?? false)).ToList();
                        if (janelasRemetente.Count == 0)
                            janelasRemetente = janelas.FindAll(x => (x?.Remetentes?.Any(e => e?.Remetente?.Codigo == pedido?.Remetente?.Codigo) ?? false)).ToList();

                        if (janelasRemetente.Count == 0)
                            janelasRemetente = janelas.FindAll(x => (x?.TiposDeCarga?.Any(e => e?.TipoDeCarga?.Codigo == pedido?.TipoDeCarga?.Codigo) ?? false)).ToList();

                        if (janelasRemetente.Count == 0)
                            janelasRemetente = janelas;

                        int tempoRota = pedido?.RotaFrete?.TempoDeViagemEmMinutos ?? 0;
                        int tolerancia_pedido = 0;           // Minutos de tolerancia que o pedido precisa chegar antes de roteirizar...será utilizando para buscar calcular a janela de descarga no destinatário.
                        int tempo_carregamento = 0;          // Minutos pedido?.Remetente?.tem
                        int tempo_descarga_destinatario = 0; // Minutos.

                        Dominio.Entidades.Embarcador.Logistica.CentroCarregamento origem = new Repositorio.Embarcador.Logistica.CentroCarregamento(unitOfWork).BuscarPorFilial(pedido?.Filial?.Codigo ?? 0);

                        if (origem != null)
                        {
                            tolerancia_pedido = origem?.TempoToleranciaPedidoRoteirizar ?? 0;

                            if (pedido?.TipoDeCarga?.Codigo > 0)
                            {
                                tempo_carregamento = (from centro in origem?.TemposCarregamento
                                                      where centro?.TipoCarga?.Codigo == pedido?.TipoDeCarga?.Codigo
                                                      select centro?.Tempo)?.Max() ?? 0;

                                tempo_descarga_destinatario = (from obj in centroDescarregamento?.TemposDescarregamento
                                                               where obj?.TipoCarga?.Codigo == pedido?.TipoDeCarga?.Codigo
                                                               select obj?.Tempo)?.Max() ?? 0;
                            }
                            else
                            {
                                tempo_carregamento = (from centro in origem?.TemposCarregamento
                                                      select centro?.Tempo)?.Max() ?? 0;

                                tempo_descarga_destinatario = (from obj in centroDescarregamento?.TemposDescarregamento
                                                               select obj?.Tempo)?.Max() ?? 0;
                            }
                        }

                        DiaSemana diaSemana = DiaSemanaHelper.ObterDiaSemana(DateTime.Now);
                        int cont = 0;
                        for (int i = (int)diaSemana; i <= (int)diaSemana + 7; i++)
                        {
                            DiaSemana aux = (DiaSemana)(i > 7 ? i - 7 : i);
                            Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento janelaDia = janelasRemetente.Find(x => x.Dia == aux);
                            //Achamos a janela do dia.. agora devemos verificar.. os prazos...
                            if (janelaDia != null)
                            {
                                DateTime dt_prev_entrega = DateTime.Now.Date.AddDays(cont).AddHours(janelaDia.HoraInicio.Hours).AddMinutes(janelaDia.HoraInicio.Minutes);
                                //Removendo o tempo de deslocamento..
                                DateTime dt_prev = dt_prev_entrega.AddMinutes(tempoRota * -1);
                                //Remover o tempo de tolerancia pedidos centro carregamento;
                                dt_prev = dt_prev.AddMinutes(tolerancia_pedido * -1);
                                //Remover o tempo estimado de carregamento.
                                dt_prev = dt_prev.AddMinutes(tempo_carregamento * -1);

                                //Achamos a date previsão de carregamento.
                                if (dt_prev >= DateTime.Now)
                                {
                                    pedido.DataPrevisaoChegadaDestinatario = dt_prev_entrega;
                                    pedido.DataPrevisaoSaidaDestinatario = dt_prev_entrega.AddMinutes(tempo_descarga_destinatario);
                                    pedido.DataCarregamentoPedido = dt_prev;
                                    new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork).Atualizar(pedido);
                                    break;
                                }
                            }
                            cont++;
                        }
                    }
                }
            }
        }

        public void InformarSeparacaoPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.ObjetosDeValor.WebService.Pedido.SeparacaoPedido protocoloSeparacaoPedido, UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoProduto repositorioPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(unitOfWork);
            Repositorio.Embarcador.Produtos.ProdutoEmbarcador repositorioProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork);

            pedido.PercentualSeparacaoPedido = protocoloSeparacaoPedido.PercentualSeparacao;
            pedido.PedidoBloqueado = protocoloSeparacaoPedido.PedidoBloqueado;
            pedido.PedidoLiberadoMontagemCarga = protocoloSeparacaoPedido.PermitirLiberarPedido;
            pedido.PedidoRestricaoData = protocoloSeparacaoPedido.PedidoRestricaoData.HasValue ? protocoloSeparacaoPedido.PedidoRestricaoData.Value : false;

            repositorioPedido.Atualizar(pedido);

            if (protocoloSeparacaoPedido.Produtos == null)
                return;

            List<string> protocolosProdutos = protocoloSeparacaoPedido.Produtos.Select(x => x.ProtocoloProduto).ToList();
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> listaPedidosProdutos = repositorioPedidoProduto.BuscarPorPedidosProdutosEmbarcador(pedido.Codigo, protocolosProdutos);

            if (listaPedidosProdutos.Count == 0)
                return;

            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoProduto pedidoProduto in listaPedidosProdutos)
            {
                Dominio.ObjetosDeValor.WebService.Pedido.SeparacaoPedidoProduto separacaoPedidoProduto = protocoloSeparacaoPedido.Produtos.Find(x => x.ProtocoloProduto == pedidoProduto.Produto.CodigoProdutoEmbarcador);

                pedidoProduto.SituacaoSeparacao = separacaoPedidoProduto.Situacao;
                repositorioPedidoProduto.Atualizar(pedidoProduto);
            }
        }

        public static void AtualizarSituacaoPlanejamentoPedidoTMS(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosBase, SituacaoPlanejamentoPedidoTMS situacao, UnitOfWork unitOfWork)
        {
            if (carga == null && cargaPedidosBase == null)
                return;
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            if (cargaPedidosBase != null)
                cargaPedidos.AddRange(cargaPedidosBase);
            else
                cargaPedidos.AddRange(repCargaPedido.BuscarPorCarga(carga.Codigo));

            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = cargaPedido.Pedido;
                pedido.SituacaoPlanejamentoPedidoTMS = situacao;
                if (pedido.SituacaoPlanejamentoPedidoTMS == SituacaoPlanejamentoPedidoTMS.Pendente)
                {
                    pedido.MotoristaCiente = false;
                    pedido.MotoristaAvisado = false;
                }
                repPedido.Atualizar(pedido);
            }
        }

        public static string RetornarObservacaoCTeDoPedidoFormatado(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMotorista repCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(unitOfWork);

            string cpfMotorista = "";
            string cnhMotorista = "";
            string rgMotorista = "";

            List<string> cpfMotoristas = repCargaMotorista.BuscarCPFMotoristasPorCarga(carga.Codigo);
            cpfMotorista = string.Join(", ", cpfMotorista);

            List<string> cnhMotoristas = repCargaMotorista.BuscarCNHMotoristasPorCarga(carga.Codigo);
            List<string> rgMotoristas = repCargaMotorista.BuscarRGMotoristasPorCarga(carga.Codigo);
            cnhMotorista = string.Join(", ", cnhMotorista);
            rgMotorista = string.Join(", ", rgMotoristas);

            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCargaEPedido(carga.Codigo, pedido.Codigo);

            string observacao = "";
            if (!string.IsNullOrWhiteSpace(pedido.ObservacaoCTe))
            {
                observacao = pedido.ObservacaoCTe.Replace("#", " #");

                observacao = observacao.Replace("#CNPJTomador", cargaPedido?.ObterTomador()?.CPF_CNPJ_Formatado ?? string.Empty).
                                                  Replace("#NomeTomador", cargaPedido?.ObterTomador()?.Nome ?? string.Empty).
                                                  Replace("#CNPJRemetente", pedido.Remetente.CPF_CNPJ_Formatado).
                                                  Replace("#NomeRemetente", pedido.Remetente.Nome).
                                                  Replace("#CNPJDestinatario", pedido.Destinatario?.CPF_CNPJ_Formatado ?? "").
                                                  Replace("#NomeDestinatario", pedido.Destinatario?.Nome ?? "").
                                                  Replace("#NumeroPedido", pedido?.NumeroPedidoEmbarcador ?? "").
                                                  Replace("#NumeroBooking", pedido?.NumeroBooking ?? "").
                                                  Replace("#NumeroOS", pedido?.NumeroOS ?? "").
                                                  Replace("#NumeroPedidoCliente", pedido?.CodigoPedidoCliente ?? "").
                                                  Replace("#NumeroContainer", pedido?.Container?.Numero ?? "").
                                                  Replace("#NavioViagemDirecao", pedido?.PedidoViagemNavio?.Descricao ?? "").
                                                  Replace("#QuantidadeETipoContainer", (pedido != null && pedido.Container != null && pedido.Container.ContainerTipo != null ? "Qtde: 1 container de " + pedido.Container.ContainerTipo.Descricao + " pés" : "")).
                                                  Replace("#PortoOrigem", pedido?.Porto?.Descricao ?? "").
                                                  Replace("#PortoDestino", pedido?.PortoDestino?.Descricao ?? "").
                                                  Replace("#NumeroCTe", carga != null ? (string.Join(",", (from obj in carga.CargaCTes select obj.CTe?.Numero.ToString() ?? "").ToList())) : "").
                                                  Replace("#NumeroCarga", carga?.CodigoCargaEmbarcador ?? "").
                                                  Replace("#SerieCTe", carga != null ? (string.Join(",", (from obj in carga.CargaCTes select obj.CTe?.Serie.ToString() ?? "").ToList())) : "").
                                                  Replace("#RotaPedidoComValor", "").
                                                  Replace("#RotaPedido", pedido.RotaFrete?.Descricao ?? "").
                                                  Replace("#Rota", pedido.RotaFrete?.Descricao ?? "").
                                                  Replace("#ValorTotalPrestacao", cargaPedido?.ValorPrestacaoServico.ToString("n2") ?? "").
                                                  Replace("#Placas", carga?.PlacasVeiculos ?? "").
                                                  Replace("#CPFMotorista", cpfMotorista).
                                                  Replace("#NomeMotorista", carga?.RetornarMotoristas ?? "").
                                                  Replace("#CNHMotorista", cnhMotorista).
                                                  Replace("#RGMotorista", rgMotorista).
                                                  Replace("#CodigoTabelaFrete", carga?.TabelaFrete?.CodigoIntegracao ?? "").
                                                  Replace("#CPFOperador", carga?.Operador?.CPF_Formatado ?? "").
                                                  Replace("#NomeOperador", carga?.Operador?.Nome ?? "").
                                                  Replace("#ModeloVeicularCarga", carga?.ModeloVeicularPlaca ?? "").
                                                  Replace("#Seguradora", cargaPedido != null ? (string.Join(",", (from obj in cargaPedido.ApoliceSeguroAverbacao select obj.ApoliceSeguro.Seguradora.Nome ?? "").ToList())) : "").
                                                  Replace("#ApoliceSeguro", cargaPedido != null ? (string.Join(",", (from obj in cargaPedido.ApoliceSeguroAverbacao select obj.ApoliceSeguro.NumeroApolice ?? "").ToList())) : "");
            }

            return observacao;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> ConfiguracaoImportacaoPedido(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repositorioConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repositorioConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repositorioConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = repositorioConfiguracaoPedido.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebService = repositorioConfiguracaoWebService.BuscarConfiguracaoPadrao();
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>();

            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = Localization.Resources.Pedidos.Pedido.Filial, Propriedade = "Filial", Tamanho = 200, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 2, Descricao = Localization.Resources.Pedidos.Pedido.CNPJTransportadora, Propriedade = "CNPJTransportadora", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 3, Descricao = Localization.Resources.Pedidos.Pedido.CPFCNPJRemetente, Propriedade = "CNPJCPFRemetente", Tamanho = 200, Obrigatorio = configuracao.RemetentePadraoImportacaoPlanilhaPedido == null, Regras = configuracao.RemetentePadraoImportacaoPlanilhaPedido == null ? new List<string> { "required" } : new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 75, Descricao = Localization.Resources.Pedidos.Pedido.CodigoIntegracaoRemetente, Propriedade = "CodigoRemetente", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 4, Descricao = Localization.Resources.Pedidos.Pedido.CPFCNPJDestinatario, Propriedade = "CNPJCPFDestinatario", Tamanho = 200, Obrigatorio = (configuracao.DestinatarioPadraoImportacaoPlanilhaPedido == null && tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS), Regras = configuracao.DestinatarioPadraoImportacaoPlanilhaPedido == null && tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? new List<string> { "required" } : new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 76, Descricao = Localization.Resources.Pedidos.Pedido.CodigoIntegracaoDestinatario, Propriedade = "CodigoDestinatario", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 5, Descricao = Localization.Resources.Pedidos.Pedido.TipoOperacao, Propriedade = "TipoOperacao", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 6, Descricao = Localization.Resources.Pedidos.Pedido.TipoCarga, Propriedade = "TipoCarga", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 7, Descricao = Localization.Resources.Pedidos.Pedido.QuantidadePallets, Propriedade = "QuantidadePallets", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 57, Descricao = Localization.Resources.Pedidos.Pedido.NumeroPedido, Propriedade = "NumeroPedido", Tamanho = 200 });
            //configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 58, Descricao = "Data de Coleta", Propriedade = "Data de Coleta", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 59, Descricao = Localization.Resources.Pedidos.Pedido.PesoPedido, Propriedade = "PesoPedido", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 60, Descricao = Localization.Resources.Pedidos.Pedido.CubagemPedido, Propriedade = "CubagemPedido", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 62, Descricao = Localization.Resources.Pedidos.Pedido.Rota, Propriedade = "Rota", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 8, Descricao = Localization.Resources.Pedidos.Pedido.NumeroNotas, Propriedade = "NotasParciais", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 9, Descricao = Localization.Resources.Pedidos.Pedido.NumeroPreCarga, Propriedade = "NumeroPreCarga", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 10, Descricao = Localization.Resources.Pedidos.Pedido.NumeroNFE, Propriedade = "NumeroNFe", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 11, Descricao = Localization.Resources.Pedidos.Pedido.SerieNFe, Propriedade = "SerieNFe", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 13, Descricao = Localization.Resources.Pedidos.Pedido.ChaveNFe, Propriedade = "ChaveNFe", Tamanho = 400 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 14, Descricao = Localization.Resources.Pedidos.Pedido.NaturezaOPNFe, Propriedade = "NaturazaOPNFe", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 15, Descricao = Localization.Resources.Pedidos.Pedido.DataEmissaoNFe, Propriedade = "DataEmissaoNFe", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 16, Descricao = Localization.Resources.Pedidos.Pedido.PesoNFe, Propriedade = "PesoNFe", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 17, Descricao = Localization.Resources.Pedidos.Pedido.ValorNFe, Propriedade = "ValorNFe", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 18, Descricao = Localization.Resources.Pedidos.Pedido.EmailRemetente, Propriedade = "EmailRemetente", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 19, Descricao = Localization.Resources.Pedidos.Pedido.BairroRemetente, Propriedade = "BairroRemetente", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 20, Descricao = Localization.Resources.Pedidos.Pedido.CEPRemetente, Propriedade = "CEPRemetente", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 21, Descricao = Localization.Resources.Pedidos.Pedido.LogradouroRemetente, Propriedade = "LogradouroRemetente", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 22, Descricao = Localization.Resources.Pedidos.Pedido.ComplementoRemetente, Propriedade = "ComplementoRemetente", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 23, Descricao = Localization.Resources.Pedidos.Pedido.NumeroRemetente, Propriedade = "NumeroRemetente", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 24, Descricao = Localization.Resources.Pedidos.Pedido.TelefoneRemetente, Propriedade = "TelefoneRemetente", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 25, Descricao = Localization.Resources.Pedidos.Pedido.IBGERemetente, Propriedade = "IBGERemetente", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 26, Descricao = Localization.Resources.Pedidos.Pedido.RegiaoRemetente, Propriedade = "RegiaoRemetente", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 27, Descricao = Localization.Resources.Pedidos.Pedido.NomeFantasiaRemetente, Propriedade = "FantasiaRemetente", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 28, Descricao = Localization.Resources.Pedidos.Pedido.IERGRemetente, Propriedade = "IERemetente", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 29, Descricao = Localization.Resources.Pedidos.Pedido.RazaoSocialRemetente, Propriedade = "RazaoRemetente", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 30, Descricao = Localization.Resources.Pedidos.Pedido.TipoPessoaRemetente, Propriedade = "TipoPessoaRemetente", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 31, Descricao = Localization.Resources.Pedidos.Pedido.EmailDestinatario, Propriedade = "EmailDestinatario", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 32, Descricao = Localization.Resources.Pedidos.Pedido.BairroDestinatario, Propriedade = "BairroDestinatario", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 33, Descricao = Localization.Resources.Pedidos.Pedido.CEPDestinatario, Propriedade = "CEPDestinatario", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 34, Descricao = Localization.Resources.Pedidos.Pedido.LogradouroDestinatario, Propriedade = "LogradouroDestinatario", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 35, Descricao = Localization.Resources.Pedidos.Pedido.ComplementoDestinatario, Propriedade = "ComplementoDestinatario", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 36, Descricao = Localization.Resources.Pedidos.Pedido.NumeroDestinatario, Propriedade = "NumeroDestinatario", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 37, Descricao = Localization.Resources.Pedidos.Pedido.TelefoneDestinatario, Propriedade = "TelefoneDestinatario", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 38, Descricao = Localization.Resources.Pedidos.Pedido.IBGEDestinatario, Propriedade = "IBGEDestinatario", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 39, Descricao = Localization.Resources.Pedidos.Pedido.RegiaoDestinatario, Propriedade = "RegiaoDestinatario", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 40, Descricao = Localization.Resources.Pedidos.Pedido.NomeFantasiaDestinatario, Propriedade = "FantasiaDestinatario", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 41, Descricao = Localization.Resources.Pedidos.Pedido.IERGDestinatario, Propriedade = "IEDestinatario", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 52, Descricao = Localization.Resources.Pedidos.Pedido.RazaoSocialDestinatario, Propriedade = "RazaoDestinatario", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 53, Descricao = Localization.Resources.Pedidos.Pedido.TipoPessoaDestinatario, Propriedade = "TipoPessoaDestinatario", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 54, Descricao = Localization.Resources.Pedidos.Pedido.PlacaVeiculo, Propriedade = "PlacaVeiculo", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 55, Descricao = Localization.Resources.Pedidos.Pedido.CPFMotorista, Propriedade = "CPFMotorista", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 56, Descricao = Localization.Resources.Pedidos.Pedido.NomeMotorista, Propriedade = "NomeMotorista", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 61, Descricao = Localization.Resources.Pedidos.Pedido.ModeloVeicular, Propriedade = "ModeloVeicularCarga", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 79, Descricao = Localization.Resources.Pedidos.Pedido.ValorFreteLiquido, Propriedade = "ValorFreteLiquido", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 63, Descricao = Localization.Resources.Pedidos.Pedido.ValorFreteReceber, Propriedade = "ValorFrete", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 64, Descricao = Localization.Resources.Pedidos.Pedido.AliquotaImposto, Propriedade = "AliquotaICMS", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 65, Descricao = Localization.Resources.Pedidos.Pedido.BaseCalculoImposto, Propriedade = "BaseCalculoICMS", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 66, Descricao = Localization.Resources.Pedidos.Pedido.ValorImposto, Propriedade = "ValorICMS", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 78, Descricao = Localization.Resources.Pedidos.Pedido.ValorImpostoPago, Propriedade = "ValorICMSPago", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 67, Descricao = Localization.Resources.Pedidos.Pedido.ReducaoBC, Propriedade = "ReducaoBC", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 68, Descricao = Localization.Resources.Pedidos.Pedido.Descarga, Propriedade = "Descarga", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 80, Descricao = Localization.Resources.Pedidos.Pedido.ValorEntrega, Propriedade = "Entrega", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 69, Descricao = Localization.Resources.Pedidos.Pedido.PedagioIda, Propriedade = "PedagioIda", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 70, Descricao = Localization.Resources.Pedidos.Pedido.PedagioVolta, Propriedade = "PedagioVolta", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 71, Descricao = Localization.Resources.Pedidos.Pedido.AdValorem, Propriedade = "AdValorem", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 72, Descricao = Localization.Resources.Pedidos.Pedido.OutrosValores, Propriedade = "OutrosValores", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 73, Descricao = Localization.Resources.Pedidos.Pedido.HoraCarregamento, Propriedade = "HoraCarregamento", Tamanho = 80 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 74, Descricao = Localization.Resources.Pedidos.Pedido.DataCarregamento, Propriedade = "DataCarregamento", Tamanho = 80 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 50, Descricao = Localization.Resources.Pedidos.Pedido.DataPrevEntrega, Propriedade = "DataPrevEntrega", Tamanho = 80 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 130, Descricao = Localization.Resources.Pedidos.Pedido.HoraPrevEntrega, Propriedade = "HoraPrevEntrega", Tamanho = 80 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 77, Descricao = Localization.Resources.Pedidos.Pedido.TipoOperacaoCargaModeloVeicular, Propriedade = "CompostoTipoCargaOperacaoVeiculo", Tamanho = 80 });
            //configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 78, Descricao = DescricaoEtapaFluxo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.ChegadaVeiculo, unitOfWork), Propriedade = "DataChegadaVeiculo", Tamanho = 60 });
            //configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 79, Descricao = DescricaoEtapaFluxo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.DeslocamentoPatio, unitOfWork), Propriedade = "DataDeslocamentoPatio", Tamanho = 60 });
            //configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 80, Descricao = DescricaoEtapaFluxo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.Guarita, unitOfWork), Propriedade = "DataGuarita", Tamanho = 60 });
            //configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 81, Descricao = DescricaoEtapaFluxo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.InformarDoca, unitOfWork), Propriedade = "DataInformarDoca", Tamanho = 60 });
            //configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 82, Descricao = DescricaoEtapaFluxo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.InicioViagem, unitOfWork), Propriedade = "DataInicioViagem", Tamanho = 60 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 83, Descricao = Localization.Resources.Pedidos.Pedido.SituacaoPedido, Propriedade = "SituacaoPedido", Tamanho = 60 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 84, Descricao = Localization.Resources.Pedidos.Pedido.CNPJCPFExpedidor, Propriedade = "CNPJCPFExpedidor", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 180, Descricao = Localization.Resources.Pedidos.Pedido.CodigoIntegracaoExpedidor, Propriedade = "CodigoExpedidor", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 85, Descricao = Localization.Resources.Pedidos.Pedido.CNPJCPFRecebedor, Propriedade = "CNPJCPFRecebedor", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 181, Descricao = Localization.Resources.Pedidos.Pedido.CodigoIntegracaoRecebedor, Propriedade = "CodigoRecebedor", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 86, Descricao = Localization.Resources.Pedidos.Pedido.ValorFreteLiquidoFilial, Propriedade = "ValorFreteLiquidoFilialEmissora", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 95, Descricao = Localization.Resources.Pedidos.Pedido.ValorFreteReceberFilial, Propriedade = "ValorFreteFilialEmissora", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 96, Descricao = Localization.Resources.Pedidos.Pedido.AliquotaImpostoFilialEmissora, Propriedade = "AliquotaICMSFilialEmissora", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 97, Descricao = Localization.Resources.Pedidos.Pedido.BaseCalculoICMSFilial, Propriedade = "BaseCalculoICMSFilialEmissora", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 98, Descricao = Localization.Resources.Pedidos.Pedido.ValorICMSFilialEmissora, Propriedade = "ValorICMSFilialEmissora", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 99, Descricao = Localization.Resources.Pedidos.Pedido.ValorICMSPagoFilial, Propriedade = "ValorICMSPagoFilialEmissora", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 100, Descricao = Localization.Resources.Pedidos.Pedido.ReducaoBCFilial, Propriedade = "ReducaoBCFilialEmissora", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 101, Descricao = Localization.Resources.Pedidos.Pedido.DescargaFilialEmissora, Propriedade = "DescargaFilialEmissora", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 102, Descricao = Localization.Resources.Pedidos.Pedido.ValorEntregaFilial, Propriedade = "EntregaFilialEmissora", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 103, Descricao = Localization.Resources.Pedidos.Pedido.PedagioIdaFilial, Propriedade = "PedagioIdaFilialEmissora", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 104, Descricao = Localization.Resources.Pedidos.Pedido.PedagioVoltaFilial, Propriedade = "PedagioVoltaFilialEmissora", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 105, Descricao = Localization.Resources.Pedidos.Pedido.AdValoremFilialEmissora, Propriedade = "AdValoremFilialEmissora", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 106, Descricao = Localization.Resources.Pedidos.Pedido.OutrosValoresFilialEmissora, Propriedade = "OutrosValoresFilialEmissora", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 87, Descricao = Localization.Resources.Pedidos.Pedido.PlacaReboque, Propriedade = "PlacaReboque", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 88, Descricao = Localization.Resources.Pedidos.Pedido.TipoTomador, Propriedade = "TipoTomador", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 89, Descricao = Localization.Resources.Pedidos.Pedido.NumeroCTe, Propriedade = "CtesParciais", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 90, Descricao = Localization.Resources.Pedidos.Pedido.AgrupamentoPreCarga, Propriedade = "AgrupamentoPreCarga", Tamanho = 100 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 91, Descricao = Localization.Resources.Pedidos.Pedido.TipoEmbarque, Propriedade = "TipoEmbarque", Tamanho = 100 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 92, Descricao = Localization.Resources.Pedidos.Pedido.Observacao, Propriedade = "ObservacaoPedido", Tamanho = 100 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 93, Descricao = Localization.Resources.Pedidos.Pedido.CodigoTransportador, Propriedade = "CodigoTransportador", Tamanho = 100 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 94, Descricao = Localization.Resources.Pedidos.Pedido.HoraPrevisaoInicioViagem, Propriedade = "HoraPrevisaoInicioViagem", Tamanho = 80 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 111, Descricao = Localization.Resources.Pedidos.Pedido.ProdutoCodigo, Propriedade = "CodigoProdutoEmbarcador", Tamanho = 80 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 112, Descricao = Localization.Resources.Pedidos.Pedido.ProdutoDescricao, Propriedade = "DescricaoProdutoEmbarcador", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 113, Descricao = Localization.Resources.Pedidos.Pedido.ProdutoQuantidade, Propriedade = "QuantidadePedidoProduto", Tamanho = 80 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 114, Descricao = Localization.Resources.Pedidos.Pedido.ProdutoPesoTotalItem, Propriedade = "PesoTotalPedidoProduto", Tamanho = 80 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 115, Descricao = Localization.Resources.Pedidos.Pedido.ProdutoCubagemUnitario, Propriedade = "MetroCubicoPedidoProduto", Tamanho = 80 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 116, Descricao = Localization.Resources.Pedidos.Pedido.ProdutoValorTotal, Propriedade = "ValorTotalPedidoProduto", Tamanho = 100 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 117, Descricao = Localization.Resources.Pedidos.Pedido.ProdutoPalletFechado, Propriedade = "PalletFechadoPedidoProduto", Tamanho = 80 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 118, Descricao = Localization.Resources.Pedidos.Pedido.LinhaSeparacao, Propriedade = "LinhaSeparacaoProduto", Tamanho = 150 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 119, Descricao = Localization.Resources.Pedidos.Pedido.Deposito, Propriedade = "Deposito", Tamanho = 100 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 121, Descricao = Localization.Resources.Pedidos.Pedido.GrupoProdutos, Propriedade = "GrupoProdutos", Tamanho = 150 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 122, Descricao = Localization.Resources.Pedidos.Pedido.CanalEntrega, Propriedade = "CanalEntrega", Tamanho = 150 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 123, Descricao = Localization.Resources.Pedidos.Pedido.OrdemColeta, Propriedade = "OrdemColeta", Tamanho = 100 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 124, Descricao = Localization.Resources.Pedidos.Pedido.DataDescarga, Propriedade = "DataDescarga", Tamanho = 100 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 125, Descricao = string.Format(Localization.Resources.Pedidos.Pedido.AdicionalX, "1"), Propriedade = "Adicional1", Tamanho = 100 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 126, Descricao = string.Format(Localization.Resources.Pedidos.Pedido.AdicionalX, "2"), Propriedade = "Adicional2", Tamanho = 100 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 127, Descricao = string.Format(Localization.Resources.Pedidos.Pedido.AdicionalX, "3"), Propriedade = "Adicional3", Tamanho = 100 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 128, Descricao = string.Format(Localization.Resources.Pedidos.Pedido.AdicionalX, "4"), Propriedade = "Adicional4", Tamanho = 100 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 131, Descricao = string.Format(Localization.Resources.Pedidos.Pedido.AdicionalX, "5"), Propriedade = "Adicional5", Tamanho = 100 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 132, Descricao = string.Format(Localization.Resources.Pedidos.Pedido.AdicionalX, "6"), Propriedade = "Adicional6", Tamanho = 100 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 133, Descricao = string.Format(Localization.Resources.Pedidos.Pedido.AdicionalX, "7"), Propriedade = "Adicional7", Tamanho = 100 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 129, Descricao = Localization.Resources.Pedidos.Pedido.NumeroCargaEncaixe, Propriedade = "NumeroCargaEncaixar", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 134, Descricao = Localization.Resources.Pedidos.Pedido.QtdVolumes, Propriedade = "QtdVolumes", Tamanho = 100 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 135, Descricao = Localization.Resources.Pedidos.Pedido.DataValidade, Propriedade = "DataValidade", Tamanho = 80 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 136, Descricao = Localization.Resources.Pedidos.Pedido.DataInicioJanelaDescarga, Propriedade = "DataInicioJanelaDescarga", Tamanho = 80 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 137, Descricao = Localization.Resources.Pedidos.Pedido.ValorTotalPedido, Propriedade = "ValorTotalPedido", Tamanho = 80 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 138, Descricao = Localization.Resources.Pedidos.Pedido.CodigoGrupoPessoasRemetente, Propriedade = "CodigoGrupoPessoasRemetente", Tamanho = 50 });

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 183, Descricao = "Centro de Resultado", Propriedade = "CentroResultado", Tamanho = 100 });
                configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 139, Descricao = "Grupo de Pessoas do Remetente", Propriedade = "GrupoPessoasRemetente", Tamanho = 100 });
            }

            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 141, Descricao = Localization.Resources.Pedidos.Pedido.NumeroControle, Propriedade = "NumeroControle", Tamanho = 100 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 140, Descricao = Localization.Resources.Pedidos.Pedido.CodigoCategoria, Propriedade = "ProdutoPrincipal", Tamanho = 100 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 142, Descricao = Localization.Resources.Pedidos.Pedido.DataPrevisaoSaida, Propriedade = "DataPrevisaoSaida", Tamanho = 80 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 143, Descricao = Localization.Resources.Pedidos.Pedido.Requisitante, Propriedade = "Requisitante", Tamanho = 80 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 144, Descricao = Localization.Resources.Pedidos.Pedido.ValorFreteNegociado, Propriedade = "ValorFreteNegociado", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 145, Descricao = Localization.Resources.Pedidos.Pedido.ValorFreteTerceiro, Propriedade = "ValorFreteTransportadorTerceiro", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 146, Descricao = Localization.Resources.Pedidos.Pedido.Tomador, Propriedade = "Tomador", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 147, Descricao = Localization.Resources.Pedidos.Pedido.ObservacaoAdicional, Propriedade = "ObservacaoAdicional", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 148, Descricao = Localization.Resources.Pedidos.Pedido.DataAlocacaoPedido, Propriedade = "DataAlocacaoPedido", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 149, Descricao = Localization.Resources.Pedidos.Pedido.Cidade, Propriedade = "Cidade", Tamanho = 80 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 150, Descricao = Localization.Resources.Pedidos.Pedido.Estado, Propriedade = "Estado", Tamanho = 80 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 151, Descricao = Localization.Resources.Pedidos.Pedido.NumeroOrdem, Propriedade = "NumeroOrdem", Tamanho = 80 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 152, Descricao = Localization.Resources.Pedidos.Pedido.GrossSales, Propriedade = "GrossSales", Tamanho = 80 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 153, Descricao = Localization.Resources.Pedidos.Pedido.Etiquetagem, Propriedade = "Etiquetagem", Tamanho = 80 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 154, Descricao = Localization.Resources.Pedidos.Pedido.Isca, Propriedade = "Isca", Tamanho = 80 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 155, Descricao = Localization.Resources.Pedidos.Pedido.VolumesPrevios, Propriedade = "VolumesPrevios", Tamanho = 80 });

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 156, Descricao = Localization.Resources.Pedidos.Pedido.CPFCNPJLocalExpedicao, Propriedade = "CNPJCPFLocalExpedicao", Tamanho = 200 });

            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 157, Descricao = Localization.Resources.Pedidos.Pedido.NumeroPedidoCliente, Propriedade = "NumeroPedidoCliente", Tamanho = 80 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 158, Descricao = Localization.Resources.Pedidos.Pedido.NumeroContainer, Propriedade = "NumeroContainer", Tamanho = 80 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 159, Descricao = Localization.Resources.Pedidos.Pedido.TipoContainer, Propriedade = "TipoContainer", Tamanho = 80 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 160, Descricao = Localization.Resources.Pedidos.Pedido.DataColetaContainer, Propriedade = "DataColetaContainer", Tamanho = 80 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 161, Descricao = Localization.Resources.Pedidos.Pedido.DataCriacaoPedidoERP, Propriedade = "DataCriacaoPedidoERP", Tamanho = 80 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 162, Descricao = Localization.Resources.Pedidos.Pedido.ValorPrevioNFe, Propriedade = "ValorTotalNotasFiscais", Tamanho = 80 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 163, Descricao = Localization.Resources.Pedidos.Pedido.PropriedadeContainer, Propriedade = "PropriedadeContainer", Tamanho = 80 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 164, Descricao = Localization.Resources.Pedidos.Pedido.PrecoUnitario, Propriedade = "PrecoUnitario", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 165, Descricao = Localization.Resources.Pedidos.Pedido.ObservacaoInterna, Propriedade = "ObservacaoInterna", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 166, Descricao = Localization.Resources.Pedidos.Pedido.CNPJTomador, Propriedade = "CNPJCPFTomador", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 167, Descricao = Localization.Resources.Pedidos.Pedido.NumeroDoca, Propriedade = "NumeroDoca", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 168, Descricao = Localization.Resources.Pedidos.Pedido.FaixaTemperatura, Propriedade = "FaixaTemperatura", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 169, Descricao = Localization.Resources.Pedidos.Pedido.TipoPagamento, Propriedade = "TipoPagamento", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 170, Descricao = Localization.Resources.Pedidos.Pedido.DataHoraPrevisaoColeta, Propriedade = "DataHoraPrevisaoColeta", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 171, Descricao = "Número EXP", Propriedade = "NumeroEXP", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 172, Descricao = "Porto de Origem", Propriedade = "PortoOrigem", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 173, Descricao = "Porto de Destino", Propriedade = "PortoDestino", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 174, Descricao = "Cliente Final", Propriedade = "ClienteFinal", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 175, Descricao = "Via Transporte", Propriedade = "ViaTransporte", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 176, Descricao = "Despachante", Propriedade = "Despachante", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 177, Descricao = "País Destino", Propriedade = "PaisDestino", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 178, Descricao = "Frete Prepaid", Propriedade = "FretePrepaid", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 179, Descricao = "Temperatura", Propriedade = "Temperatura", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 184, Descricao = "Fronteira", Propriedade = "Fronteira", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 185, Descricao = Localization.Resources.Pedidos.Pedido.NotaFiscalMercadoLivre, Propriedade = "NotaMercadoLivre", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 185, Descricao = Localization.Resources.Pedidos.Pedido.SiglaFaturamentoMercadoLivre, Propriedade = "SiglaFaturamentoMercadoLivre", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 185, Descricao = Localization.Resources.Pedidos.Pedido.PFMercadoLivre, Propriedade = "PFMercadoLivre", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 185, Descricao = Localization.Resources.Pedidos.Pedido.ItemFaturadoMercadoLivre, Propriedade = "ItemFaturadoMercadoLivre", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 186, Descricao = Localization.Resources.Pedidos.Pedido.DestinoEntregaLocalidade, Propriedade = "LocalidadePedidoDestino", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 187, Descricao = Localization.Resources.Pedidos.Pedido.DestinoEntregaEndereco, Propriedade = "EnderecoPedidoDestino", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 188, Descricao = Localization.Resources.Pedidos.Pedido.DestinoEntregaNumero, Propriedade = "NumeroPedidoDestino", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 189, Descricao = Localization.Resources.Pedidos.Pedido.DestinoEntregaCEP, Propriedade = "CEPPedidoDestino", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 190, Descricao = Localization.Resources.Pedidos.Pedido.DestinoEntregaBairro, Propriedade = "BairroPedidoDestino", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 191, Descricao = Localization.Resources.Pedidos.Pedido.DestinoEntregaTelefone, Propriedade = "TelefonePedidoDestino", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 192, Descricao = Localization.Resources.Pedidos.Pedido.DestinoEntregaIERG, Propriedade = "IEPedidoDestino", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 193, Descricao = Localization.Resources.Pedidos.Pedido.DestinoEntregaComplemento, Propriedade = "ComplementoPedidoDestino", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 194, Descricao = Localization.Resources.Pedidos.Pedido.Distancia, Propriedade = "Distancia", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 195, Descricao = Localization.Resources.Pedidos.Pedido.CodigoEnderecoSecundario, Propriedade = "CodigoEnderecoSecundario", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 196, Descricao = Localization.Resources.Pedidos.Pedido.PedidosShopee, Propriedade = "PedidosShopee", Tamanho = 200 });

            if (configuracaoPedido?.HabilitarBIDTransportePedido ?? false)
                configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 197, Descricao = Localization.Resources.Pedidos.Pedido.NumeroPedidoOrigem, Propriedade = "NumeroPedidoOrigem", Tamanho = 200 });

            if (configuracaoWebService?.AtualizarNumeroPedidoVinculado ?? false)
            {
                configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 198, Descricao = Localization.Resources.Pedidos.Pedido.EssePedidopossuiPedidoBonificacao, Propriedade = "EssePedidopossuiPedidoBonificacao", Tamanho = 80 });
                configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 199, Descricao = Localization.Resources.Pedidos.Pedido.EssePedidopossuiPedidoVenda, Propriedade = "EssePedidopossuiPedidoVenda", Tamanho = 80 });
                configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 200, Descricao = Localization.Resources.Pedidos.Pedido.NumeroPedidoVinculado, Propriedade = "NumeroPedidoVinculado", Tamanho = 200 });
            }

            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 201, Descricao = Localization.Resources.Pedidos.Pedido.CodAtividadeDestinatario, Propriedade = "CodigoAtividadeDestinatario", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 202, Descricao = Localization.Resources.Pedidos.Pedido.CodAtividadeRemetente, Propriedade = "CodigoAtividadeRemetente", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 203, Descricao = Localization.Resources.Pedidos.Pedido.DataPrevisaoTerminoCarregamento, Propriedade = "DataPrevisaoTerminoCarregamento", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 204, Descricao = Localization.Resources.Pedidos.Pedido.CodIntegracaoCentroCarregamento, Propriedade = "CodIntegracaoCentroCarregamento", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 205, Descricao = Localization.Resources.Pedidos.Pedido.DescCentroCarregamento, Propriedade = "DescCentroCarregamento", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 206, Descricao = "Devolução Pacotes", Propriedade = "DevolucaoPacotes", Tamanho = 80 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 207, Descricao = "Cód. Integração Centro de Custo Viagem", Propriedade = "CodIntegracaoCentroCustoDeViagem", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 208, Descricao = "Descrição Centro de Custo Viagem", Propriedade = "DescCentroCustoDeViagem", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 209, Descricao = "Número do Contrato", Propriedade = "NumeroContratoFreteCliente", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 210, Descricao = "Canal", Propriedade = "Canal", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 211, Descricao = "UF Origem", Propriedade = "UFOrigem", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 212, Descricao = "Cidade Origem", Propriedade = "CidadeOrigem", Tamanho = 200 });

            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 213, Descricao = "Facility Mercado Livre", Propriedade = "FacilityMercadoLivre", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 214, Descricao = "Rota Mercado Livre", Propriedade = "RotaMercadoLivre", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 215, Descricao = "Ajudante", Propriedade = "Ajudante", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 216, Descricao = "Quantidade de Ajudantes", Propriedade = "QuantidadeAjudantes", Tamanho = 200 });

            return configuracoes;
        }

        public static bool TomadorPossuiPendenciaFinanceira(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, out string mensagem)
        {
            mensagem = null;
            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                return false;

            Dominio.Entidades.Cliente tomador = pedido.ObterTomador();
            if (tomador != null && tomador.SituacaoFinanceira.HasValue && tomador.SituacaoFinanceira == SituacaoFinanceira.Bloqueada)
            {
                mensagem = $"O cliente {tomador.Descricao} está com pendência financeira e por esse motivo não é permitido criar novos pedidos para o mesmo.";
                return true;
            }
            if (tomador != null && tomador.GrupoPessoas != null && tomador.GrupoPessoas.SituacaoFinanceira.HasValue && tomador.GrupoPessoas.SituacaoFinanceira.Value == SituacaoFinanceira.Bloqueada)
            {
                mensagem = $"O grupo de pessoa {tomador.GrupoPessoas.Descricao} está com pendência financeira e por esse motivo não é permitido criar novos pedidos para o mesmo.";
                return true;
            }
            return false;
        }

        public static void VerificarOcorrenciaPedidoPorEtapaFluxoPatio(Dominio.Entidades.Embarcador.Cargas.Carga carga, EtapaFluxoGestaoPatio etapaFluxoPatio, DateTime dataFinalizacao, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Filiais.ConfiguracaoGestaoPatio repConfiguracaoGestaoPatio = new Repositorio.Embarcador.Filiais.ConfiguracaoGestaoPatio(unitOfWork);
            Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio configuracaoGestaoPatio = repConfiguracaoGestaoPatio.BuscarConfiguracao();
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            if (carga != null && configuracaoGestaoPatio != null)
            {
                if (configuracaoGestaoPatio.GerarOcorrenciaPedidoEtapaDocaCarregamento && configuracaoGestaoPatio.TipoDeOcorrencia != null && etapaFluxoPatio == EtapaFluxoGestaoPatio.InformarDoca)
                {
                    //deve gerar uma ocorrencia no pedido.
                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);
                    if ((cargaPedidos == null) || (cargaPedidos.Count == 0))
                        return;

                    List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = (from o in cargaPedidos select o.Pedido).Distinct().ToList();

                    if ((pedidos == null) || (pedidos.Count == 0))
                        return;

                    Dominio.Entidades.Embarcador.GestaoEntregas.ConfiguracaoPortalCliente configuracaoPortalCliente = Servicos.Embarcador.GestaoEntregas.ConfiguracaoPortalCliente.ObterConfiguracao(unitOfWork);

                    foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidos)
                    {
                        Dominio.Entidades.Cliente tomador = pedido.ObterTomador() ?? pedido.Remetente;
                        string observacao = string.Concat(" Destinatario", pedido.Destinatario?.Descricao ?? "", " Destino", pedido.Destinatario?.Localidade?.Descricao ?? "", " Origem", tomador?.Descricao ?? "", " Remetente", tomador?.Localidade?.Descricao ?? "");

                        Servicos.Embarcador.Carga.ControleEntrega.OcorrenciaEntrega.GerarPedidoOcorrenciaColetaEntrega(tomador, pedido, carga, configuracaoGestaoPatio.TipoDeOcorrencia, configuracaoPortalCliente, observacao, configuracaoEmbarcador, clienteMultisoftware, unitOfWork);

                    }
                }
            }
        }

        public static void RegistrarPedidoDevolucao(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, DateTime dataRejeicao, Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoDeOcorrencia, List<int> codigosNotasFiscais, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Repositorio.UnitOfWork unitOfWork)
        {
            //percorrer todos os pedidos da entrega e pedidos com devolucao parcial ou total deve ser clonado e criado
            //pedidos devolvido totalmente tem que vincular no novo pedido (pedido devolvido) as notas do mesmo
            //pedidos devolvidos parcialmente ficam sem notas

            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoProduto repPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto repCargaEntregaProduto = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaEntregaPedido.BuscarPedidosPorNotasAsync(codigosNotasFiscais).Result;
            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscaisDevolucao = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();
            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosDevolvidos = new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {

                if (cargaPedido.NotasFiscais.Any(x => x.XMLNotaFiscal?.SituacaoEntregaNotaFiscal == SituacaoNotaFiscal.Entregue))
                    throw new ServicoException("Nenhuma nota fiscal pode estar com a situação 'Entregue' para a devolução.");

                if (cargaPedido.NotasFiscais.Any(x => x.XMLNotaFiscal?.SituacaoEntregaNotaFiscal == SituacaoNotaFiscal.Devolvida || x.XMLNotaFiscal?.SituacaoEntregaNotaFiscal == SituacaoNotaFiscal.DevolvidaParcial || x.XMLNotaFiscal?.SituacaoEntregaNotaFiscal == SituacaoNotaFiscal.AgReentrega))// desenvolvido para MATTEL onde sempre deve haver nota
                {

                    Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoDevolucao = cargaPedido.Pedido.Clonar();
                    LimparDadosPedidoDevolucao(pedidoDevolucao);
                    if (configuracaoEmbarcador.NumeroCargaSequencialUnico)
                        pedidoDevolucao.NumeroSequenciaPedido = repPedido.ObterProximoCodigo();
                    else
                        pedidoDevolucao.NumeroSequenciaPedido = repPedido.ObterProximoCodigo(cargaEntrega.Carga.Filial);

                    Utilidades.Object.DefinirListasGenericasComoNulas(pedidoDevolucao);

                    if (cargaPedido.Pedido.SituacaoAcompanhamentoPedido == SituacaoAcompanhamentoPedido.EntregaRejeitada)
                    {
                        pedidoDevolucao.NotasFiscais = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();
                        foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal nota in cargaPedido.NotasFiscais)
                        {
                            if (nota.XMLNotaFiscal.SituacaoEntregaNotaFiscal == SituacaoNotaFiscal.Devolvida)
                            {
                                Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notadevolucao = nota.XMLNotaFiscal.Clonar();
                                Utilidades.Object.DefinirListasGenericasComoNulas(notadevolucao);

                                repNotaFiscal.Inserir(notadevolucao);
                                pedidoDevolucao.NotasFiscais.Add(notadevolucao);
                                notasFiscaisDevolucao.Add(notadevolucao);
                            }
                        }
                    }

                    int addLeadTime = cargaPedido.Carga.Rota != null ? (cargaPedido.Carga.Rota.TempoDeViagemEmMinutos * 2) : 0;
                    pedidoDevolucao.DataAgendamento = null;
                    pedidoDevolucao.TipoOperacao = tipoDeOcorrencia.TipoOperacaoDevolucao;
                    Servicos.Log.TratarErro($"3 - Pedido (Código: {cargaPedido.Pedido.Codigo} - Número: {cargaPedido.Pedido.NumeroPedidoEmbarcador}) trocou de remetente de {cargaPedido.Pedido.Remetente.Codigo} para {cargaPedido.Pedido.Destinatario.Codigo}.", "TrocaRemetentePedido");
                    pedidoDevolucao.Remetente = cargaPedido.Pedido.Destinatario;
                    pedidoDevolucao.Destinatario = cargaPedido.NotasFiscais.FirstOrDefault().XMLNotaFiscal.Emitente;
                    pedidoDevolucao.PesoTotal = cargaPedido.Pedido.PesoTotal;
                    pedidoDevolucao.PesoCubado = cargaPedido.Pedido.PesoCubado;
                    pedidoDevolucao.PesoLiquidoTotal = cargaPedido.Pedido.PesoLiquidoTotal;
                    pedidoDevolucao.Origem = cargaPedido.Pedido.Destinatario?.Localidade;
                    pedidoDevolucao.Destino = pedidoDevolucao.Destinatario?.Localidade;
                    pedidoDevolucao.SituacaoAgendamentoEntregaPedido = (pedidoDevolucao.Destinatario?.ExigeQueEntregasSejamAgendadas ?? false) ? SituacaoAgendamentoEntregaPedido.AguardandoAgendamento : SituacaoAgendamentoEntregaPedido.NaoExigeAgendamento;

                    pedidoDevolucao.UsarOutroEnderecoOrigem = false;
                    pedidoDevolucao.EnderecoOrigem = null;
                    pedidoDevolucao.UsarOutroEnderecoDestino = false;
                    pedidoDevolucao.EnderecoDestino = null;

                    pedidoDevolucao.PrevisaoEntrega = dataRejeicao.AddMinutes(addLeadTime);
                    pedidoDevolucao.GerarAutomaticamenteCargaDoPedido = false;
                    pedidoDevolucao.PedidoTotalmenteCarregado = false;
                    pedidoDevolucao.PedidoRedespachoTotalmenteCarregado = false;
                    pedidoDevolucao.PedidoDeDevolucao = true;
                    pedidoDevolucao.PedidoOrigemDevolucao = cargaPedido.Pedido;

                    repPedido.Inserir(pedidoDevolucao);
                    pedidosDevolvidos.Add(pedidoDevolucao);

                    Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto repcargaEntregaProduto = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto(unitOfWork);

                    List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto> cargaEntregaProdutoPedido = repCargaEntregaProduto.BuscarPorCargaEntrega(cargaEntrega.Codigo);

                    if (cargaPedido.Pedido.SituacaoAcompanhamentoPedido == SituacaoAcompanhamentoPedido.EntregaRejeitada)
                    {
                        foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto cargaEntregaProdutoDevolvido in cargaEntregaProdutoPedido)
                        {
                            Dominio.Entidades.Embarcador.Pedidos.PedidoProduto pedidoProdutoDevolucao = new Dominio.Entidades.Embarcador.Pedidos.PedidoProduto()
                            {
                                Quantidade = cargaEntregaProdutoDevolvido.Quantidade,
                                Pedido = pedidoDevolucao,
                                Produto = cargaEntregaProdutoDevolvido.Produto
                            };

                            pedidoProdutoDevolucao.Pedido.NotasFiscais = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();

                            if (cargaEntregaProdutoDevolvido.XMLNotaFiscal != null)
                            {
                                pedidoProdutoDevolucao.Pedido.NotasFiscais.Add(cargaEntregaProdutoDevolvido.XMLNotaFiscal);
                                notasFiscaisDevolucao.Add(cargaEntregaProdutoDevolvido.XMLNotaFiscal);
                            }

                            Utilidades.Object.DefinirListasGenericasComoNulas(pedidoProdutoDevolucao);
                            repPedidoProduto.Inserir(pedidoProdutoDevolucao);

                        }
                    }
                    else if (cargaPedido.Pedido.SituacaoAcompanhamentoPedido == SituacaoAcompanhamentoPedido.EntregaParcial)
                    {

                        if (cargaEntregaProdutoPedido?.Count > 0)
                        {
                            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto pedidoProduto in cargaPedido.Produtos)
                            {
                                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto cargaEntregaProdutoDevolvido = cargaEntregaProdutoPedido.Where(x => x.Produto.Codigo == pedidoProduto.Produto.Codigo && x.QuantidadeDevolucao > 0).FirstOrDefault();

                                if (cargaEntregaProdutoDevolvido != null)
                                {
                                    Dominio.Entidades.Embarcador.Pedidos.PedidoProduto pedidoProdutoDevolucaoClonar = (from obj in cargaPedido.Pedido.Produtos where obj.Produto.Codigo == pedidoProduto.Produto.Codigo select obj).FirstOrDefault();
                                    Dominio.Entidades.Embarcador.Pedidos.PedidoProduto pedidoProdutoDevolucao = null;

                                    if (pedidoProdutoDevolucaoClonar == null)
                                        pedidoProdutoDevolucao = new Dominio.Entidades.Embarcador.Pedidos.PedidoProduto();
                                    else
                                        pedidoProdutoDevolucao = pedidoProdutoDevolucaoClonar.Clonar();

                                    pedidoProdutoDevolucao.Quantidade = cargaEntregaProdutoDevolvido.QuantidadeDevolucao;
                                    pedidoProdutoDevolucao.Pedido = pedidoDevolucao;
                                    pedidoProdutoDevolucao.Produto = pedidoProduto.Produto;

                                    if (pedidoProdutoDevolucao.Pedido.NotasFiscais == null)
                                        pedidoProdutoDevolucao.Pedido.NotasFiscais = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();

                                    if (cargaEntregaProdutoDevolvido.XMLNotaFiscal != null)
                                    {
                                        pedidoProdutoDevolucao.Pedido.NotasFiscais.Add(cargaEntregaProdutoDevolvido.XMLNotaFiscal);
                                        notasFiscaisDevolucao.Add(cargaEntregaProdutoDevolvido.XMLNotaFiscal);
                                    }

                                    Utilidades.Object.DefinirListasGenericasComoNulas(pedidoProdutoDevolucao);
                                    repPedidoProduto.Inserir(pedidoProdutoDevolucao);
                                }
                            }
                        }
                    }

                }
            }

            EnviarPedidoDevolucaoTransportadorEmail(pedidosDevolvidos, notasFiscaisDevolucao, cargaEntrega, unitOfWork, out string msg);
        }

        public void AtualizarLocalParqueamentoPedido(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                return;

            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Logistica.PontoPassagemPreDefinido repPontoPassagemPreDefinido = new Repositorio.Embarcador.Logistica.PontoPassagemPreDefinido(unitOfWork);

            Dominio.Entidades.Cliente localParqueamento = carga.TipoOperacao?.ConfiguracaoControleEntrega?.LocalDeParqueamentoCliente;

            if (localParqueamento == null && carga.Rota != null)
                localParqueamento = repPontoPassagemPreDefinido.BuscarPrimeiroLocalParqueamentoPorRotaFrete(carga.Rota.Codigo);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = cargaPedido.Pedido;

                if (pedido.LocalParqueamento?.Codigo == localParqueamento?.Codigo)
                    continue;

                pedido.LocalParqueamento = localParqueamento;
                repPedido.Atualizar(pedido);
            }
        }
        public async Task AtualizarLocalParqueamentoPedidoAsync(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                return;

            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Logistica.PontoPassagemPreDefinido repPontoPassagemPreDefinido = new Repositorio.Embarcador.Logistica.PontoPassagemPreDefinido(unitOfWork);

            Dominio.Entidades.Cliente localParqueamento = carga.TipoOperacao?.ConfiguracaoControleEntrega?.LocalDeParqueamentoCliente;

            if (localParqueamento == null && carga.Rota != null)
                localParqueamento = await repPontoPassagemPreDefinido.BuscarPrimeiroLocalParqueamentoPorRotaFreteAsync(carga.Rota.Codigo);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = cargaPedido.Pedido;

                if (pedido.LocalParqueamento?.Codigo == localParqueamento?.Codigo)
                    continue;

                pedido.LocalParqueamento = localParqueamento;
                await repPedido.AtualizarAsync(pedido);
            }
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> ConfiguracaoImportacaoOcorrenciaPorPedido(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repositorioConfiguracaoTMS.BuscarConfiguracaoPadrao();
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>();

            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = "Número do Pedido", Propriedade = "NumeroPedidoEmbarcador", Tamanho = 200, Obrigatorio = true });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 2, Descricao = "Código da Ocorrência", Propriedade = "CodigoOcorrencia", Tamanho = 200, Obrigatorio = true });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 3, Descricao = "Observação", Propriedade = "Observacao", Tamanho = 200, Obrigatorio = true });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 4, Descricao = "Data da Ocorrência", Propriedade = "DataOcorrencia", Tamanho = 200 });

            return configuracoes;
        }

        public Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao ImportarPedidoOcorrencia(string dados, (string Nome, string Guid) arquivoGerador, Dominio.Entidades.Usuario usuario, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultiSoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, string AdminStringConexao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao()
            {
                Retornolinhas = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha>()
            };

            int contador = 0;

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = repConfiguracaoPedido.BuscarConfiguracaoPadrao();
            List<Dominio.Entidades.Embarcador.PreCargas.PreCarga> preCargas = new List<Dominio.Entidades.Embarcador.PreCargas.PreCarga>();
            List<string> cargasParaCancelamento = new List<string>();

            List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha>>(dados);
            for (int i = 0; i < linhas.Count; i++)
            {
                try
                {
                    unitOfWork.FlushAndClear();
                    unitOfWork.Start();

                    // Processa linha do arquivo como um pedido isoladamente
                    Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retornoLinha = ImportarPedidoOcorrenciaLinha(linhas[i], arquivoGerador, usuario, tipoServicoMultisoftware, clienteMultiSoftware, auditado, unitOfWork);
                    retornoLinha.indice = i;
                    retornoImportacao.Retornolinhas.Add(retornoLinha);

                    // Processou com sucesso?
                    if (retornoLinha.processou)
                        unitOfWork.CommitChanges();
                    else
                        unitOfWork.Rollback();
                }
                catch (BaseException ex2)
                {
                    unitOfWork.Rollback();
                    retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha(ex2.Message, i));
                    continue;
                }
                catch (Exception ex2)
                {
                    unitOfWork.Rollback();
                    Servicos.Log.TratarErro(ex2);
                    retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("Ocorreu uma falha ao processar a linha.", i));
                    continue;
                }
            }

            string retornoFinaliza = "";

            retornoImportacao.MensagemAviso = retornoFinaliza;
            retornoImportacao.Total = linhas.Count();
            retornoImportacao.Importados = contador;

            return retornoImportacao;
        }

        public Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha ImportarPedidoOcorrenciaLinha(Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha, (string Nome, string Guid) arquivoGerador, Dominio.Entidades.Usuario usuario, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("pt-BR");

            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retornoLinhaPedido = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha();

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCodigoOcorrencia = (from obj in linha.Colunas where obj.NomeCampo == "CodigoOcorrencia" select obj).FirstOrDefault();
                string codigoOcorrencia = "";
                if (colCodigoOcorrencia != null)
                {
                    codigoOcorrencia = (string)colCodigoOcorrencia.Valor;
                }

                DateTime dataHoraOcorrencia = DateTime.Now;
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDataOcorrencia = (from obj in linha.Colunas where obj.NomeCampo == "DataOcorrencia" select obj).FirstOrDefault();
                string dataOcorrencia = "";
                if (colDataOcorrencia != null)
                {
                    dataOcorrencia = colDataOcorrencia.Valor;
                    double.TryParse(dataOcorrencia, out double dataFormatoExcel);
                    if (dataFormatoExcel > 0)
                        dataHoraOcorrencia = Utilidades.DateTime.ConverterDataExcelToDateTime(dataFormatoExcel);
                    else if (!string.IsNullOrWhiteSpace(dataOcorrencia))
                        DateTime.TryParse(dataOcorrencia, out dataHoraOcorrencia);
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNumeroPedido = (from obj in linha.Colunas where obj.NomeCampo == "NumeroPedidoEmbarcador" select obj).FirstOrDefault();
                string numeroPedido = "";
                if (colNumeroPedido != null)
                {
                    numeroPedido = (string)colNumeroPedido.Valor;

                    if (!repositorioPedido.VerificarExistenciaPedido(numeroPedido))
                        return RetornarFalhaLinha("O pedido não existe no sistema");
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colObservacaoPedido = (from obj in linha.Colunas where obj.NomeCampo == "Observacao" select obj).FirstOrDefault();
                string observacaoPedido = string.Empty;
                if (colObservacaoPedido != null)
                {
                    observacaoPedido = (string)colObservacaoPedido.Valor;
                }

                Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoImportacaoOcorrencia pedidoImportacaoOcorrencia = new Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoImportacaoOcorrencia()
                {
                    CodigoOcorrencia = codigoOcorrencia,
                    NumeroPedido = numeroPedido,
                    Observacao = observacaoPedido,
                    DataHoraOcorrencia = dataHoraOcorrencia,
                    NomeArquivoGerador = arquivoGerador.Nome,
                    GuidArquivoGerador = arquivoGerador.Guid
                };

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                new Servicos.Embarcador.CargaOcorrencia.Ocorrencia().GerarOcorrenciaPorImportacaoPedido(pedidoImportacaoOcorrencia, OrigemSituacaoEntrega.UsuarioMultiEmbarcador, tipoServicoMultisoftware, configuracaoEmbarcador, usuario, clienteMultisoftware, unitOfWork, auditado);
            }
            catch (ServicoException excecao)
            {
                return RetornarFalhaLinha($"Ocorreu uma falha ao processar a linha ({excecao.Message}).");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return RetornarFalhaLinha("Ocorreu uma falha ao processar a linha.");
            }

            return RetornarSucessoLinha(retornoLinhaPedido?.codigo ?? 0);
        }

        public void GerarPedidosConsumoSemCarga(Dominio.ObjetosDeValor.Embarcador.Integracao.Moniloc.RecebimentoPedido recebimentoPedido, string arquivoDisponivel, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoProduto repPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(unitOfWork);
            Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork);
            Repositorio.Embarcador.Produtos.TipoEmbalagem repTipoEmbalagem = new Repositorio.Embarcador.Produtos.TipoEmbalagem(unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicular = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            try
            {
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = new Dominio.Entidades.Embarcador.Pedidos.Pedido();
                unitOfWork.Start();

                pedido.Numero = repPedido.BuscarProximoNumero();
                pedido.SituacaoPedido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Aberto;
                pedido.UltimaAtualizacao = DateTime.Now;

                pedido.Tomador = repCliente.BuscarPorCodigoAlternativo(recebimentoPedido.CodigoPlanta);
                pedido.Remetente = repCliente.BuscarPorCodigoAlternativo(recebimentoPedido.CodigoFornecedor);
                pedido.Destinatario = repCliente.BuscarPorCodigoAlternativo(recebimentoPedido.LocalEntrega);

                if (pedido.Remetente != null)
                {
                    Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco pedidoEnderecoOrigem = new Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco();
                    PreecherEnderecoPedido(ref pedidoEnderecoOrigem, pedido.Remetente);
                    pedido.EnderecoOrigem = pedidoEnderecoOrigem;
                    pedido.Origem = pedido?.Remetente?.Localidade ?? null;
                }

                if (pedido.Destinatario != null)
                {
                    Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco pedidoEnderecoDestino = new Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco();
                    PreecherEnderecoPedido(ref pedidoEnderecoDestino, pedido.Destinatario);
                    pedido.EnderecoOrigem = pedidoEnderecoDestino;
                    pedido.Destino = pedido?.Destinatario?.Localidade ?? null;
                }

                pedido.DataEntrega = recebimentoPedido.Produtos.FirstOrDefault().DataEntrega;
                pedido.DataCarregamentoPedido = recebimentoPedido.Produtos.FirstOrDefault().DataColeta;
                pedido.DataInicialColeta = recebimentoPedido.Produtos.FirstOrDefault().DataColeta;
                pedido.NumeroPedidoEmbarcador = recebimentoPedido.Produtos.FirstOrDefault().NumeroRelease;

                string tipoVeiculo = recebimentoPedido.Coletas.FirstOrDefault().TipoVeiculo;
                pedido.ModeloVeicularCarga = repModeloVeicular.buscarPorCodigoIntegracao(tipoVeiculo);

                repPedido.Inserir(pedido);

                foreach (Dominio.ObjetosDeValor.Embarcador.Integracao.Moniloc.DadosRecebimentoPedidoProduto recebimentoProduto in recebimentoPedido.Produtos)
                {
                    Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produto = repProdutoEmbarcador.buscarPorCodigoEmbarcador(recebimentoProduto.CodigoProduto);
                    if (produto == null)
                    {
                        produto = new Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador();

                        produto.CodigoProdutoEmbarcador = recebimentoProduto.CodigoProduto;
                        produto.Descricao = recebimentoProduto.CodigoProduto;
                        produto.Ativo = true;
                        produto.PesoUnitario = recebimentoProduto.PesoProduto;

                        repProdutoEmbarcador.Inserir(produto);
                    }

                    Dominio.Entidades.Embarcador.Produtos.TipoEmbalagem embalagem = repTipoEmbalagem.BuscarPorCodigoIntegracao(recebimentoProduto.Embalagem);
                    if (embalagem == null)
                    {
                        embalagem = new Dominio.Entidades.Embarcador.Produtos.TipoEmbalagem();

                        embalagem.CodigoIntegracao = recebimentoProduto.Embalagem;
                        embalagem.Descricao = recebimentoProduto.Embalagem;
                        embalagem.Ativo = true;

                        repTipoEmbalagem.Inserir(embalagem);
                    }
                    produto.TipoEmbalagem = embalagem;

                    Dominio.Entidades.Embarcador.Pedidos.PedidoProduto pedidoProduto = new Dominio.Entidades.Embarcador.Pedidos.PedidoProduto();

                    pedidoProduto.Pedido = pedido;
                    pedidoProduto.Produto = produto;
                    pedidoProduto.Quantidade = recebimentoProduto.QuantidadeColetada;
                    pedidoProduto.QuantidadePlanejada = recebimentoProduto.QuantidadeSolicitada;
                    pedidoProduto.PesoUnitario = recebimentoProduto.PesoProduto;

                    repPedidoProduto.Inserir(pedidoProduto);
                }

                Servicos.Auditoria.Auditoria.Auditar(auditado, pedido, null, "Pedido recebido pela integração Moniloc", unitOfWork);

                pedido.Protocolo = pedido.Codigo;
                repPedido.Atualizar(pedido);

                unitOfWork.CommitChanges();
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro($"Arquivo: {arquivoDisponivel} | " + ex, "IntegracaoPedidoMoniloc");
                unitOfWork.Rollback();
            }
        }

        public void GerarPedidosConsumoExtraSemCarga(Dominio.ObjetosDeValor.Embarcador.Integracao.Moniloc.RecebimentoPedidoExtra recebimentoPedidoExtra, string arquivoDisponivel, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoProduto repPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(unitOfWork);
            Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicular = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoDeCarga repTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

            try
            {
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = new Dominio.Entidades.Embarcador.Pedidos.Pedido();
                unitOfWork.Start();

                pedido.Numero = repPedido.BuscarProximoNumero();
                pedido.SituacaoPedido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Aberto;
                pedido.UltimaAtualizacao = DateTime.Now;

                pedido.NumeroPedidoEmbarcador = recebimentoPedidoExtra.NumeroSolicitacao;
                pedido.Tomador = repCliente.BuscarPorCodigoIntegracao(recebimentoPedidoExtra.Planta);
                pedido.Remetente = repCliente.BuscarPorCodigoIntegracao(recebimentoPedidoExtra.Origem);
                pedido.Destinatario = repCliente.BuscarPorCodigoIntegracao(recebimentoPedidoExtra.Destino);
                pedido.Empresa = repEmpresa.BuscarPorCodigoIntegracao(recebimentoPedidoExtra.Transportadora);
                pedido.TipoDeCarga = repTipoDeCarga.BuscarPorCodigoEmbarcador(recebimentoPedidoExtra.TipoSolicitacao);

                if (pedido.Remetente != null)
                {
                    Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco pedidoEnderecoOrigem = new Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco();
                    PreecherEnderecoPedido(ref pedidoEnderecoOrigem, pedido.Remetente);
                    pedido.EnderecoOrigem = pedidoEnderecoOrigem;
                    pedido.Origem = pedido.Remetente.Localidade ?? null;
                }

                if (pedido.Destinatario != null)
                {
                    Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco pedidoEnderecoDestino = new Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco();
                    PreecherEnderecoPedido(ref pedidoEnderecoDestino, pedido.Destinatario);
                    pedido.EnderecoDestino = pedidoEnderecoDestino;
                    pedido.Destino = pedido.Destinatario.Localidade ?? null;
                }

                pedido.DataInicialColeta = recebimentoPedidoExtra.DataColeta;
                pedido.DataCarregamentoPedido = recebimentoPedidoExtra.DataColeta;
                pedido.ModeloVeicularCarga = repModeloVeicular.buscarPorCodigoIntegracao(recebimentoPedidoExtra.TipoVeiculo);

                repPedido.Inserir(pedido);

                foreach (Dominio.ObjetosDeValor.Embarcador.Integracao.Moniloc.DadosRecebimentoPedidoExtraProduto recebimentoProduto in recebimentoPedidoExtra.Produtos)
                {
                    Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produto = repProdutoEmbarcador.buscarPorCodigoEmbarcador(recebimentoProduto.CodigoProduto);
                    if (produto == null)
                    {
                        produto = new Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador();

                        produto.CodigoProdutoEmbarcador = recebimentoProduto.CodigoProduto;
                        produto.Descricao = recebimentoProduto.CodigoProduto;
                        produto.Ativo = true;
                        produto.PesoUnitario = recebimentoProduto.PesoProduto;

                        repProdutoEmbarcador.Inserir(produto);
                    }

                    Dominio.Entidades.Embarcador.Pedidos.PedidoProduto pedidoProduto = new Dominio.Entidades.Embarcador.Pedidos.PedidoProduto();

                    pedidoProduto.Pedido = pedido;
                    pedidoProduto.Produto = produto;
                    pedidoProduto.QuantidadePlanejada = recebimentoProduto.QuantidadeSolicitada;
                    pedidoProduto.PesoUnitario = recebimentoProduto.PesoProduto;

                    repPedidoProduto.Inserir(pedidoProduto);
                }

                Servicos.Auditoria.Auditoria.Auditar(auditado, pedido, null, "Pedido recebido pela integração Moniloc", unitOfWork);

                pedido.Protocolo = pedido.Codigo;
                repPedido.Atualizar(pedido);

                unitOfWork.CommitChanges();
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro($"Arquivo: {arquivoDisponivel} | " + ex, "IntegracaoPedidoMoniloc");
                unitOfWork.Rollback();
            }
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> ConfiguracaoImportacaoAtualizarPedido(Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>();

            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = "Número do Pedido", Propriedade = "NumeroPedidoEmbarcador", Tamanho = 200, Obrigatorio = true });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 2, Descricao = "Número rastreio Correios", Propriedade = "NumeroRastreioCorreios", Tamanho = 200, Obrigatorio = true });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 3, Descricao = "Observação", Propriedade = "Observacao", Tamanho = 200, Obrigatorio = true });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 4, Descricao = "Data previsão de entrega", Propriedade = "DataPrevisaoEntrega", Tamanho = 200, Obrigatorio = true });

            return configuracoes;
        }

        public Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao ImportarAtualizacaoPedido(string dados, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Repositorio.UnitOfWork unitOfWork)
        {

            Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao()
            {
                Retornolinhas = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha>()
            };

            int contador = 0;

            List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha>>(dados);
            for (int i = 0; i < linhas.Count; i++)
            {
                try
                {
                    unitOfWork.FlushAndClear();
                    unitOfWork.Start();

                    // Processa linha do arquivo como um pedido isoladamente
                    Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retornoLinha = ImportarAtualizacaoPedidoLinha(linhas[i], auditado, unitOfWork);
                    retornoLinha.indice = i;
                    retornoImportacao.Retornolinhas.Add(retornoLinha);

                    // Processou com sucesso?
                    if (retornoLinha.processou)
                    {
                        contador++;
                        unitOfWork.CommitChanges();
                    }
                    else
                        unitOfWork.Rollback();
                }
                catch (BaseException ex2)
                {
                    unitOfWork.Rollback();
                    retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha(ex2.Message, i));
                    continue;
                }
                catch (Exception ex2)
                {
                    unitOfWork.Rollback();
                    Servicos.Log.TratarErro(ex2);
                    retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("Ocorreu uma falha ao processar a linha.", i));
                    continue;
                }
            }

            string retornoFinaliza = "";

            retornoImportacao.MensagemAviso = retornoFinaliza;
            retornoImportacao.Total = linhas.Count();
            retornoImportacao.Importados = contador;

            return retornoImportacao;
        }

        public Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha ImportarAtualizacaoPedidoLinha(Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.Pedido PedidoParaAtualizar = null;


            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retornoLinhaPedido = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha();

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNumeroPedido = (from obj in linha.Colunas where obj.NomeCampo == "NumeroPedidoEmbarcador" select obj).FirstOrDefault();
                if (colNumeroPedido != null)
                {
                    PedidoParaAtualizar = repositorioPedido.BuscarPorNumeroEmbarcador((string)colNumeroPedido.Valor);

                    if (PedidoParaAtualizar == null)
                        return RetornarFalhaLinha("O pedido não existe no sistema");
                }
                else
                {
                    return RetornarFalhaLinha("É obrigatorio informar a coluna do número do pedido");
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colObservacaoPedido = (from obj in linha.Colunas where obj.NomeCampo == "Observacao" select obj).FirstOrDefault();
                if (colObservacaoPedido != null)
                {
                    if (string.IsNullOrEmpty((string)colObservacaoPedido.Valor))
                        return RetornarFalhaLinha("É obrigatorio informar uma observação");

                    PedidoParaAtualizar.Observacao = (string)colObservacaoPedido.Valor;
                }
                else
                {
                    return RetornarFalhaLinha("É obrigatorio informar a coluna da observação do pedido");
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNumeroRastreioCorreios = (from obj in linha.Colunas where obj.NomeCampo == "NumeroRastreioCorreios" select obj).FirstOrDefault();
                if (colNumeroRastreioCorreios != null)
                {
                    if (string.IsNullOrEmpty((string)colNumeroRastreioCorreios.Valor))
                        return RetornarFalhaLinha("É obrigatorio informar o número de rastreio do pedido");

                    PedidoParaAtualizar.NumeroRastreioCorreios = (string)colNumeroRastreioCorreios.Valor;
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDataPrevisaoEntrega = (from obj in linha.Colunas where obj.NomeCampo == "DataPrevisaoEntrega" select obj).FirstOrDefault();
                if (colDataPrevisaoEntrega != null)
                {
                    DateTime dataPrevEntrega = DateTime.MinValue;
                    string strDataPrevEntrega = "";
                    strDataPrevEntrega = colDataPrevisaoEntrega.Valor;
                    double dataFormatoExcel = (double)Utilidades.Decimal.Converter(strDataPrevEntrega);
                    if (dataFormatoExcel > 0)
                        dataPrevEntrega = Utilidades.DateTime.ConverterDataExcelToDateTime(dataFormatoExcel, strDataPrevEntrega);
                    else if (!string.IsNullOrWhiteSpace(strDataPrevEntrega))
                        DateTime.TryParse(strDataPrevEntrega, out dataPrevEntrega);

                    if (string.IsNullOrEmpty((string)colDataPrevisaoEntrega.Valor))
                        return RetornarFalhaLinha("É obrigatorio informar a data previsão de entrega do pedido");

                    PedidoParaAtualizar.PrevisaoEntrega = dataPrevEntrega;
                }

                repositorioPedido.Atualizar(PedidoParaAtualizar);

                Servicos.Auditoria.Auditoria.Auditar(auditado, PedidoParaAtualizar, "Atualizou campos do Pedido N°" + PedidoParaAtualizar.NumeroPedidoEmbarcador + " via importação de planilha", unitOfWork);
            }
            catch (ServicoException excecao)
            {
                return RetornarFalhaLinha($"Ocorreu uma falha ao processar a linha ({excecao.Message}).");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return RetornarFalhaLinha("Ocorreu uma falha ao processar a linha.");
            }

            return RetornarSucessoLinha(retornoLinhaPedido?.codigo ?? 0);
        }

        public bool VerificarSeExistePedidosDuplicados(string dados)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha>>(dados);
            List<string> numerosPedidosParaAtualizar = new List<string>();
            for (int i = 0; i < linhas.Count; i++)
            {
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNumeroPedido = (from obj in linhas[i].Colunas where obj.NomeCampo == "NumeroPedidoEmbarcador" select obj).FirstOrDefault();
                string numeroPedido = (string)colNumeroPedido.Valor;

                if (numerosPedidosParaAtualizar.Contains(numeroPedido))
                    return true;
                else
                    numerosPedidosParaAtualizar.Add(numeroPedido);
            }
            return false;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> ConfiguracaoImportacaoBIDTransportePedido(Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>();

            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = "Número do Pedido", Propriedade = "NumeroPedidoEmbarcador", Tamanho = 200, Obrigatorio = true });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 2, Descricao = "ID Oferta", Propriedade = "IDOFerta", Tamanho = 200, Obrigatorio = true });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 3, Descricao = "Transportadora", Propriedade = "Transportadora", Tamanho = 200, Obrigatorio = true });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 4, Descricao = "Valor Frete R$/Ton", Propriedade = "ValorFreteTon", Tamanho = 200, Obrigatorio = true });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 5, Descricao = "Aplicar ICMS (1 = Sim / 0 = Não)", Propriedade = "AplicarICMS", Tamanho = 200, Obrigatorio = true });

            return configuracoes;
        }

        public Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao ImportarBIDTransportePedido(string dados, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao()
            {
                Retornolinhas = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha>()
            };

            int contador = 0;

            List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha>>(dados);
            for (int i = 0; i < linhas.Count; i++)
            {
                try
                {
                    unitOfWork.FlushAndClear();
                    unitOfWork.Start();

                    // Processa linha do arquivo como um pedido isoladamente
                    Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retornoLinha = ImportarBIDTransportePedidoLinha(linhas[i], auditado, unitOfWork);
                    retornoLinha.indice = i;
                    retornoImportacao.Retornolinhas.Add(retornoLinha);

                    // Processou com sucesso?
                    if (retornoLinha.processou)
                    {
                        contador++;
                        unitOfWork.CommitChanges();
                    }
                    else
                        unitOfWork.Rollback();
                }
                catch (BaseException ex2)
                {
                    unitOfWork.Rollback();
                    retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha(ex2.Message, i));
                    continue;
                }
                catch (Exception ex2)
                {
                    unitOfWork.Rollback();
                    Servicos.Log.TratarErro(ex2);
                    retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("Ocorreu uma falha ao processar a linha.", i));
                    continue;
                }
            }

            string retornoFinaliza = "";

            retornoImportacao.MensagemAviso = retornoFinaliza;
            retornoImportacao.Total = linhas.Count();
            retornoImportacao.Importados = contador;

            return retornoImportacao;
        }

        public Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha ImportarBIDTransportePedidoLinha(Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.CotacaoPedido.CotacaoPedido repositorioCotacaoPedido = new Repositorio.Embarcador.CotacaoPedido.CotacaoPedido(unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = null;

            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retornoLinhaPedido = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha();

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNumeroPedido = (from obj in linha.Colunas where obj.NomeCampo == "NumeroPedidoEmbarcador" select obj).FirstOrDefault();
                if (colNumeroPedido?.Valor != null)
                {
                    pedido = repositorioPedido.BuscarPorNumeroEmbarcador((string)colNumeroPedido?.Valor);

                    if (pedido == null)
                        return RetornarFalhaLinha("Não foi localizado Pedido com esse Número.");
                }
                else
                    return RetornarFalhaLinha("É obrigatório informar o Número do Pedido.");

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colIDOFerta = (from obj in linha.Colunas where obj.NomeCampo == "IDOFerta" select obj).FirstOrDefault();
                string IDOferta = string.Empty;

                if (colIDOFerta?.Valor != null)
                    IDOferta = colIDOFerta?.Valor;

                Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido cotacaoPedido = repositorioCotacaoPedido.BuscarPorCodigoIntegracao(IDOferta);

                if (cotacaoPedido == null)
                {
                    cotacaoPedido = new Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido();
                    cotacaoPedido.Pedido = pedido;
                    cotacaoPedido.DataCriacao = null;
                }

                if (IDOferta != null)
                    cotacaoPedido.CodigoIntegracao = IDOferta;
                else
                    return RetornarFalhaLinha("É obrigatório informar a ID Oferta.");

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCodigoTransportador = (from obj in linha.Colunas where obj.NomeCampo == "Transportadora" select obj).FirstOrDefault();
                Dominio.Entidades.Empresa empresa = null;
                if (colCodigoTransportador?.Valor != null)
                {
                    string codigoIntegracaoOuCNPJEmpresa = (string)colCodigoTransportador?.Valor;

                    empresa = repositorioEmpresa.BuscarPorCodigoIntegracao(codigoIntegracaoOuCNPJEmpresa);

                    if (empresa == null)
                        empresa = repositorioEmpresa.BuscarPorCNPJ(codigoIntegracaoOuCNPJEmpresa);

                    if (empresa != null)
                        cotacaoPedido.Empresa = empresa;

                    if (empresa == null || empresa.Status == "I")
                    {
                        return RetornarFalhaLinha("Transportador informado não existe na base Multisoftware.");
                    }
                }
                else
                    return RetornarFalhaLinha("É obrigatório informar o Código de Integração ou CNPJ do Transportador.");

                decimal valorFreteTon = 0;
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colValorFreteTon = (from obj in linha.Colunas where obj.NomeCampo == "ValorFreteTon" select obj).FirstOrDefault();
                if (colValorFreteTon?.Valor != null)
                {
                    decimal.TryParse((string)colValorFreteTon.Valor, out valorFreteTon);

                    cotacaoPedido.ValorFreteTonelada = valorFreteTon;
                }
                else
                    return RetornarFalhaLinha("É obrigatório informar o Valor Frete R$/Ton.");

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colAplicarICMS = (from obj in linha.Colunas where obj.NomeCampo == "AplicarICMS" select obj).FirstOrDefault();
                if (colAplicarICMS?.Valor != null)
                {
                    string aplicaICMS = Utilidades.String.OnlyNumbers((string)colAplicarICMS.Valor.Trim());

                    if (aplicaICMS == "1")
                        cotacaoPedido.IncluirValorICMSBaseCalculo = true;
                    else
                        cotacaoPedido.IncluirValorICMSBaseCalculo = false;
                }
                else
                    return RetornarFalhaLinha("É obrigatório informar 1 se aplica alíquota de ICMS no cálculo de frete e 0 se não aplica.");

                if (cotacaoPedido.Codigo > 0)
                {
                    repositorioCotacaoPedido.Atualizar(cotacaoPedido);
                    Servicos.Auditoria.Auditoria.Auditar(auditado, pedido, "Atualizou Cotações do Pedido Número " + pedido.NumeroPedidoEmbarcador + " via importação de planilha.", unitOfWork);
                }
                else
                {
                    repositorioCotacaoPedido.Inserir(cotacaoPedido);
                    Servicos.Auditoria.Auditoria.Auditar(auditado, pedido, "Inseriu Cotações do Pedido Número " + pedido.NumeroPedidoEmbarcador + " via importação de planilha.", unitOfWork);
                }

            }
            catch (ServicoException excecao)
            {
                return RetornarFalhaLinha($"Ocorreu uma falha ao processar a linha ({excecao.Message}).");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return RetornarFalhaLinha("Ocorreu uma falha ao processar a linha.");
            }

            return RetornarSucessoLinha(retornoLinhaPedido?.codigo ?? 0);
        }

        public string ObterTomadorPedido(TipoTomador tipoTomador, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            switch (tipoTomador)
            {
                case TipoTomador.Remetente:
                    return string.Concat(pedido?.Remetente?.Nome, " ( ", pedido?.Remetente?.CPF_CNPJ_Formatado, " )");
                case TipoTomador.Destinatario:
                    return string.Concat(pedido?.Destinatario?.Nome, " (", pedido?.Destinatario?.CPF_CNPJ_Formatado, " )");
                case TipoTomador.Intermediario:
                    return string.Concat(pedido?.Tomador?.Nome, " ( ", pedido?.Tomador?.CPF_CNPJ_Formatado, " )");
                case TipoTomador.Tomador:
                    return string.Concat(pedido?.Tomador?.Nome, " ( ", pedido?.Tomador?.CPF_CNPJ_Formatado, " )");
                case TipoTomador.Outros:
                    return string.Concat(pedido?.Tomador?.Nome, " ( ", pedido?.Tomador?.CPF_CNPJ_Formatado, " )");
                default:
                    return "";
            }

        }

        public void AdicionarPedidoObservacao(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, DateTime dataHoraInclusao, Dominio.Entidades.Usuario usuario, string observacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoObservacao repPedidoObservacao = new Repositorio.Embarcador.Pedidos.PedidoObservacao(unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.PedidoObservacao pedidoObservacao = new Dominio.Entidades.Embarcador.Pedidos.PedidoObservacao();

            pedidoObservacao.Pedido = pedido;
            pedidoObservacao.DataHoraInclusao = dataHoraInclusao;
            pedidoObservacao.Observacao = observacao;
            pedidoObservacao.Usuario = usuario;

            pedido.Observacao = observacao;

            repPedidoObservacao.Inserir(pedidoObservacao);
            repPedido.Atualizar(pedido);

        }

        public async Task<List<Dominio.ObjetosDeValor.Embarcador.TorreControle.DetalhesPedido.Pedido>> ObterDetalhesPedidosAsync(int codigoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega repOcorrenciaColetaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega repositorioPedidoOcorrenciaColetaEntrega = new Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega> ocorrenciaColetaEntregas = await repOcorrenciaColetaEntrega.BuscarPorCargaAsync(codigoCarga);
            List<Dominio.ObjetosDeValor.Embarcador.TorreControle.DetalhesPedido.Pedido> listaDetalhesPedidos = await repositorioPedido.ObterDetalhesPedidosMonitoramentoAsync(codigoCarga);
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega> pedidosOcorrenciaColetaEntrega = repositorioPedidoOcorrenciaColetaEntrega.BuscarPorCarga(codigoCarga);

            foreach (Dominio.ObjetosDeValor.Embarcador.TorreControle.DetalhesPedido.Pedido pedido in listaDetalhesPedidos)
            {
                pedido.Ocorrencias = ocorrenciaColetaEntregas
                    .Where(ocorrencia => ocorrencia.CargaEntrega.Pedidos
                        .Any(cargaEntregaPedido => cargaEntregaPedido.CargaPedido.Codigo == pedido.CodigoCargaPedido))
                    .Select(ocorrencia => new Dominio.ObjetosDeValor.Embarcador.TorreControle.DetalhesPedido.Ocorrencia
                    {
                        Codigo = ocorrencia.Codigo,
                        CodigoOcorrencia = ocorrencia.TipoDeOcorrencia?.Codigo ?? 0,
                        CodigoPedido = pedido?.CodigoCargaPedido ?? 0,
                        Descricao = Servicos.Embarcador.GestaoEntregas.NotificaoEntrega.ObterDescricaoPortalOcorrencia(
                            ocorrencia.TipoDeOcorrencia,
                            pedido.Carga.CodigoCargaEmbarcador,
                            ocorrencia.CargaEntrega.Cliente?.Nome ?? string.Empty,
                            ocorrencia.CargaEntrega.Cliente?.Localidade?.Descricao ?? string.Empty,
                            pedido.Remetente.Nome,
                            pedido.Remetente.Endereco.Localidade.Descricao,
                            true),
                        DataOcorrencia = ocorrencia.DataOcorrencia.ToString("dd/MM/yyyy HH:mm"),
                        DataPosicao = ocorrencia.DataPosicao?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                        DataReprogramada = ocorrencia.DataPrevisaoRecalculada?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                        Latitude = ocorrencia.Latitude ?? 0,
                        Longitude = ocorrencia.Longitude ?? 0,
                        TempoPercurso = ocorrencia.TempoPercurso ?? string.Empty,
                        Distancia = (ocorrencia.DistanciaAteDestino > 0 ? (ocorrencia.DistanciaAteDestino / 1000).ToString("n3") : "0") + " KM",
                        Origem = ocorrencia.OrigemOcorrencia.HasValue ? OrigemCriacaoOcorrenciaHelper.ObterDescricao(ocorrencia.OrigemOcorrencia.Value) : string.Empty,
                    })
                    .ToList();
                pedido.OcorrenciasComerciais = pedidosOcorrenciaColetaEntrega
                    .Where(pedidoOcorrenciaColetaEntrega => pedidoOcorrenciaColetaEntrega.Pedido.Codigo == pedido.Codigo &&
                                                            (!string.IsNullOrEmpty(pedidoOcorrenciaColetaEntrega.Natureza) ||
                                                             !string.IsNullOrEmpty(pedidoOcorrenciaColetaEntrega.GrupoOcorrencia) ||
                                                             !string.IsNullOrEmpty(pedidoOcorrenciaColetaEntrega.Razao) ||
                                                             pedidoOcorrenciaColetaEntrega.NotaFiscalDevolucao > 0 ||
                                                             !string.IsNullOrEmpty(pedidoOcorrenciaColetaEntrega.SolicitacaoCliente)
                                                            ))
                    .Select(p => new Dominio.ObjetosDeValor.Embarcador.TorreControle.DetalhesPedido.Ocorrencia
                    {
                        Codigo = p.Codigo,
                        CodigoOcorrencia = p.TipoDeOcorrencia?.Codigo ?? 0,
                        CodigoPedido = pedido.CodigoCargaPedido,
                        Descricao = Servicos.Embarcador.GestaoEntregas.NotificaoEntrega.ObterDescricaoPortalOcorrencia(
                            p.TipoDeOcorrencia,
                            pedido.Carga.CodigoCargaEmbarcador,
                            p.Pedido.Destinatario?.Nome ?? string.Empty,
                            p.Pedido.Remetente.Nome ?? string.Empty,
                            pedido.Remetente.Nome,
                            pedido.Remetente.Endereco.Localidade.Descricao,
                            true),
                        DataOcorrencia = p.DataOcorrencia.ToString("dd/MM/yyyy HH:mm"),
                        DataPosicao = p.DataPosicao?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                        DataReprogramada = p.DataPrevisaoRecalculada?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                        Latitude = p.Latitude ?? 0,
                        Longitude = p.Longitude ?? 0,
                        TempoPercurso = p.TempoPercurso ?? string.Empty,
                        Distancia = (p.DistanciaAteDestino > 0 ? (p.DistanciaAteDestino / 1000).ToString("n3") : "0") + " KM",
                        Origem = "Integração",
                        Natureza = p.Natureza ?? string.Empty,
                        GrupoOcorrencia = p.GrupoOcorrencia ?? string.Empty,
                        Razao = p.Razao ?? string.Empty,
                        NotaFiscalDevolucao = p.NotaFiscalDevolucao,
                        SolicitacaoCliente = p.SolicitacaoCliente ?? string.Empty
                    }).ToList();
                pedido.CanalEntrega = pedido.CanalEntrega ?? "Não informado";
            }
            return listaDetalhesPedidos;
        }

        public async Task GerarIntegracaoAsync(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao)
        {
            Repositorio.Embarcador.Pedidos.PedidoIntegracao repPedidoIntegracao = new Repositorio.Embarcador.Pedidos.PedidoIntegracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao pedidoIntegracao = new Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao()
            {
                Pedido = pedido,
                TipoIntegracao = tipoIntegracao,
                ProblemaIntegracao = string.Empty,
                DataEnvio = DateTime.Now
            };

            await repPedidoIntegracao.InserirAsync(pedidoIntegracao);
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        public static void LimparDadosPedidoDevolucao(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            pedido.DataTerminoCarregamento = null;
            pedido.DataAgendamento = null;
            pedido.PrevisaoEntrega = null;
            pedido.SituacaoPedido = SituacaoPedido.Aberto;
        }

        public static bool EnviarPedidoDevolucaoTransportadorEmail(List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosDevolvidos, List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscaisDevolucao, Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, Repositorio.UnitOfWork unidadeTrabalho, out string mensagem)
        {
            Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unidadeTrabalho);
            Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo();

            if (email == null)
            {
                mensagem = "Não há um e-mail configurado para realizar o envio.";
                return false;
            }

            string numeroNotasFiscais = string.Join(", ", notasFiscaisDevolucao.Select(obj => obj.Numero).Distinct());

            string emailEmpresa = cargaEntrega.Carga.Empresa?.Email ?? "";
            List<string> emailsenvio = new List<string>();
            if (!string.IsNullOrWhiteSpace(emailEmpresa))
                emailsenvio = emailEmpresa.Split(';').ToList();

            string assunto = string.Empty;
            StringBuilder bodyEmail = new StringBuilder();

            if (pedidosDevolvidos.Count > 1)
            {
                assunto = $"Novos pedidos devolução {string.Join(", ", pedidosDevolvidos.Select(obj => obj.NumeroPedidoEmbarcador))} gerados automaticamente";
                bodyEmail.AppendLine($"Olá,{Environment.NewLine}Seguem os dados das devoluções dos pedidos {string.Join(", ", pedidosDevolvidos.Select(obj => obj.NumeroPedidoEmbarcador))}.{Environment.NewLine}");
            }
            else
            {
                assunto = $"Novo pedido devolução {pedidosDevolvidos.FirstOrDefault().NumeroPedidoEmbarcador} gerado automaticamente";
                bodyEmail.AppendLine($"Olá,{Environment.NewLine}Seguem os dados das devoluções dos pedidos {pedidosDevolvidos.FirstOrDefault().NumeroPedidoEmbarcador}.{Environment.NewLine}");
            }

            List<Dominio.Entidades.Cliente> destinatarios = pedidosDevolvidos.Where(obj => obj.Destinatario != null).Select(obj => obj.Destinatario).Distinct().ToList();

            if (destinatarios.Count > 1)
                bodyEmail.AppendLine($"Clientes: {string.Join(", ", destinatarios.Select(obj => obj.Descricao))}");
            else
                bodyEmail.AppendLine($"Cliente: {destinatarios.FirstOrDefault()?.Descricao}");

            bodyEmail.AppendLine($"Nº Carga: {cargaEntrega.Carga.CodigoCargaEmbarcador}");
            bodyEmail.AppendLine($"Notas Fiscais: {numeroNotasFiscais}");

            DateTime? dataPrevisaoEntrega = pedidosDevolvidos.Min(x => x.PrevisaoEntrega);

            bodyEmail.AppendLine($"Prazo entrega devolução: {dataPrevisaoEntrega?.ToString("dd/MM/yyyy HH:mm:ss")}", dataPrevisaoEntrega.HasValue);
            bodyEmail.AppendLine($"Favor acessar o Portal do Transportador e montar sua carga de devolução referente as notas fiscais acima.");
            bodyEmail.AppendLine($"Observação: {cargaEntrega.Observacao}", !string.IsNullOrWhiteSpace(cargaEntrega.Observacao));
            bodyEmail.Append($"{Environment.NewLine}E-mail enviado automaticamente. Por favor, não responda.");

            bool sucesso = Servicos.Email.EnviarEmail(email.Email, email.Email, email.Senha, null, emailsenvio.ToArray(), null, assunto, bodyEmail.ToString(), email.Smtp, out mensagem, email.DisplayEmail, null, "", email.RequerAutenticacaoSmtp, "", email.PortaSmtp, unidadeTrabalho);

            return sucesso;
        }

        private static void ProcessarIntegracao(Repositorio.UnitOfWork unitOfWork, int codigoPedidoIntegracao)
        {
            try
            {
                // Importa a planilha gerando pedidos e cargas
                Servicos.Embarcador.Pedido.Pedido serPedido = new Servicos.Embarcador.Pedido.Pedido();
                Repositorio.Embarcador.Pedidos.PedidoIntegracao repPedidoIntegracao = new Repositorio.Embarcador.Pedidos.PedidoIntegracao(unitOfWork);
                Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.Integracao integracao = repIntegracao.Buscar();

                Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao pedidoIntegracao = repPedidoIntegracao.BuscarPorCodigo(codigoPedidoIntegracao);

                unitOfWork.Start();

                if (pedidoIntegracao.TipoIntegracao != null)
                {
                    switch (pedidoIntegracao.TipoIntegracao.Tipo)
                    {
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Trizy:
                            if (!integracao.PossuiIntegracaoTrizy || string.IsNullOrWhiteSpace(integracao.TokenTrizy) || string.IsNullOrWhiteSpace(integracao.URLTrizy))
                            {
                                pedidoIntegracao.ProblemaIntegracao = "Não possui configuração para integração da Trizy";
                                pedidoIntegracao.Tentativas += 1;
                                pedidoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                            }
                            else
                            {
                                Servicos.Embarcador.Integracao.Trizy.IntegracaoTrizy.IntegrarApiMultiplasEntregas(ref pedidoIntegracao, unitOfWork);
                            }
                            break;

                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.AX:
                            if (!integracao.PossuiIntegracaoAX || string.IsNullOrWhiteSpace(integracao.URLAXPedido))
                            {
                                pedidoIntegracao.ProblemaIntegracao = "Não possui configuração para integração da AX";
                                pedidoIntegracao.Tentativas += 1;
                                pedidoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                            }
                            else
                            {
                                Servicos.Embarcador.Integracao.AX.IntegracaoAX.EnviarPedido(ref pedidoIntegracao, unitOfWork, integracao.URLAXPedido, integracao.UsuarioAX, integracao.SenhaAX);
                            }
                            break;

                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.AngelLira:

                            Repositorio.Embarcador.Configuracoes.IntegracaoAngelLira repTipoIntegracaoAngelLira = new Repositorio.Embarcador.Configuracoes.IntegracaoAngelLira(unitOfWork);
                            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoAngelLira IntegracaoAngelLira = repTipoIntegracaoAngelLira.Buscar();

                            if (IntegracaoAngelLira == null || (IntegracaoAngelLira != null && string.IsNullOrEmpty(IntegracaoAngelLira.URLAcessoPedido)))
                            {
                                pedidoIntegracao.ProblemaIntegracao = "Não possui configuração para integração de pedidos com a AngelLira";
                                pedidoIntegracao.Tentativas += 1;
                                pedidoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                            }
                            else
                            {
                                Servicos.Embarcador.Integracao.AngelLira.IntegracaoPedidosAngelLira.EnviarPedido(ref pedidoIntegracao, unitOfWork, IntegracaoAngelLira.URLAcessoPedido, IntegracaoAngelLira.UsuarioAcessoPedido, IntegracaoAngelLira.SenhaAcessoPedido);
                            }
                            break;

                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Correios:

                            Repositorio.Embarcador.Configuracoes.IntegracaoCorreios repositorioConfiguracaoIntegracaoCorreios = new Repositorio.Embarcador.Configuracoes.IntegracaoCorreios(unitOfWork);
                            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCorreios configuracaoIntegracaoCorreios = repositorioConfiguracaoIntegracaoCorreios.Buscar();

                            if (configuracaoIntegracaoCorreios != null && !string.IsNullOrWhiteSpace(configuracaoIntegracaoCorreios.URLPLP))
                                new Servicos.Embarcador.Integracao.Correios.IntegracaoCorreios().IntegrarPreListaPostagem(ref pedidoIntegracao, unitOfWork);
                            else
                                Servicos.Embarcador.Integracao.Correios.IntegracaoCorreios.CarregarPrecoPostagem(ref pedidoIntegracao, unitOfWork);
                            break;

                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.A52:
                            new Servicos.Embarcador.Integracao.A52.IntegracaoA52(unitOfWork).IntegrarPedido(pedidoIntegracao);
                            break;

                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Routeasy:
                            new Servicos.Embarcador.Integracao.RoutEasy.IntegracaoRoutEasy(unitOfWork).IntegrarPedidoAtualizacaoSituacao(pedidoIntegracao).GetAwaiter().GetResult();
                            break;

                        default:
                            pedidoIntegracao.ProblemaIntegracao = "Configuração de integração para pedido desconhecida";
                            pedidoIntegracao.Tentativas += 1;
                            pedidoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                            break;
                    }
                }
                else
                {
                    pedidoIntegracao.ProblemaIntegracao = "Não possui nenhuma configuração de integração para pedido";
                    pedidoIntegracao.Tentativas += 1;
                    pedidoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                }

                repPedidoIntegracao.Atualizar(pedidoIntegracao);

                unitOfWork.CommitChanges();
                unitOfWork.FlushAndClear();

            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                unitOfWork.FlushAndClear();
                Servicos.Log.TratarErro(ex, "IntegracaoPedido");
            }
        }

        private static void ProcessarIntegracaoAguardandoRetorno(Repositorio.UnitOfWork unitOfWork, int codigoPedidoIntegracao)
        {
            try
            {
                Servicos.Embarcador.Pedido.Pedido serPedido = new Servicos.Embarcador.Pedido.Pedido();
                Repositorio.Embarcador.Pedidos.PedidoIntegracao repPedidoIntegracao = new Repositorio.Embarcador.Pedidos.PedidoIntegracao(unitOfWork);
                Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.Integracao integracao = repIntegracao.Buscar();

                Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao pedidoIntegracao = repPedidoIntegracao.BuscarPorCodigo(codigoPedidoIntegracao);

                unitOfWork.Start();

                if (pedidoIntegracao.TipoIntegracao != null)
                {
                    switch (pedidoIntegracao.TipoIntegracao.Tipo)
                    {

                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.AngelLira:

                            Repositorio.Embarcador.Configuracoes.IntegracaoAngelLira repTipoIntegracaoAngelLira = new Repositorio.Embarcador.Configuracoes.IntegracaoAngelLira(unitOfWork);
                            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoAngelLira IntegracaoAngelLira = repTipoIntegracaoAngelLira.Buscar();

                            if (IntegracaoAngelLira == null || string.IsNullOrEmpty(IntegracaoAngelLira.URLAcessoPedido))
                            {
                                pedidoIntegracao.ProblemaIntegracao = "Não possui configuração para integração de pedidos com a AngelLira";
                                pedidoIntegracao.Tentativas += 1;
                                pedidoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                            }
                            else
                            {
                                Servicos.Embarcador.Integracao.AngelLira.IntegracaoPedidosAngelLira.BuscarPedidoAguardando(ref pedidoIntegracao, unitOfWork, IntegracaoAngelLira.URLAcessoPedido, IntegracaoAngelLira.UsuarioAcessoPedido, IntegracaoAngelLira.SenhaAcessoPedido);
                            }
                            break;

                        default:
                            pedidoIntegracao.ProblemaIntegracao = "Configuração de integração para pedido desconhecida";
                            pedidoIntegracao.Tentativas += 1;
                            pedidoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                            break;
                    }
                }
                else
                {
                    pedidoIntegracao.ProblemaIntegracao = "Não possui nenhuma configuração de integração para pedido";
                    pedidoIntegracao.Tentativas += 1;
                    pedidoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                }

                repPedidoIntegracao.Atualizar(pedidoIntegracao);

                unitOfWork.CommitChanges();
                unitOfWork.FlushAndClear();

            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                unitOfWork.FlushAndClear();
                Servicos.Log.TratarErro(ex, "IntegracaoPedido");
            }
        }

        private static void ProcessarIntegracaoEmCancelamento(Repositorio.UnitOfWork unitOfWork, int codigoPedidoIntegracao)
        {
            try
            {

                Servicos.Embarcador.Pedido.Pedido serPedido = new Servicos.Embarcador.Pedido.Pedido();
                Repositorio.Embarcador.Pedidos.PedidoIntegracao repPedidoIntegracao = new Repositorio.Embarcador.Pedidos.PedidoIntegracao(unitOfWork);
                Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.Integracao integracao = repIntegracao.Buscar();

                Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao pedidoIntegracao = repPedidoIntegracao.BuscarPorCodigo(codigoPedidoIntegracao);

                unitOfWork.Start();

                if (pedidoIntegracao.TipoIntegracao != null)
                {
                    switch (pedidoIntegracao.TipoIntegracao.Tipo)
                    {

                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.AngelLira:

                            Repositorio.Embarcador.Configuracoes.IntegracaoAngelLira repTipoIntegracaoAngelLira = new Repositorio.Embarcador.Configuracoes.IntegracaoAngelLira(unitOfWork);
                            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoAngelLira IntegracaoAngelLira = repTipoIntegracaoAngelLira.Buscar();

                            if (IntegracaoAngelLira != null || string.IsNullOrEmpty(IntegracaoAngelLira.URLAcessoPedido))
                            {
                                pedidoIntegracao.ProblemaIntegracao = "Não possui configuração para integração de pedidos com a AngelLira";
                                pedidoIntegracao.Tentativas += 1;
                                pedidoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                            }
                            else
                            {
                                Servicos.Embarcador.Integracao.AngelLira.IntegracaoPedidosAngelLira.CancelarPedidoAguardandoIntegracao(ref pedidoIntegracao, unitOfWork, IntegracaoAngelLira.URLAcessoPedido, IntegracaoAngelLira.UsuarioAcessoPedido, IntegracaoAngelLira.SenhaAcessoPedido);
                            }
                            break;

                        default:
                            pedidoIntegracao.ProblemaIntegracao = "Configuração de integração para pedido desconhecida";
                            pedidoIntegracao.Tentativas += 1;
                            pedidoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                            break;
                    }
                }
                else
                {
                    pedidoIntegracao.ProblemaIntegracao = "Não possui nenhuma configuração de integração para pedido";
                    pedidoIntegracao.Tentativas += 1;
                    pedidoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                }

                repPedidoIntegracao.Atualizar(pedidoIntegracao);

                unitOfWork.CommitChanges();
                unitOfWork.FlushAndClear();

            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                unitOfWork.FlushAndClear();
                Servicos.Log.TratarErro(ex, "IntegracaoPedido");
            }
        }

        private static void ProcessarIntegracaoCorreios(int codigoPedido, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCorreios configuracaoCorreios, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repPedido.BuscarPorCodigo(codigoPedido);

            try
            {
                Servicos.Embarcador.CargaOcorrencia.Ocorrencia serOcorrencia = new Servicos.Embarcador.CargaOcorrencia.Ocorrencia();

                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.TipoDeOcorrenciaDeCTe repTipoOcorrencia = new TipoDeOcorrenciaDeCTe(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Empresa transportador = repEmpresa.BuscarPorCodigo(pedido.Empresa.Codigo);

                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXmlNotasFiscaisPorCarga = repositorioPedidoXMLNotaFiscal.BuscarPorPedido(pedido.Codigo);
                if (pedidoXmlNotasFiscaisPorCarga == null || pedidoXmlNotasFiscaisPorCarga.Count == 0)
                    throw new ServicoException("Pedido codigo " + codigoPedido + " sem  pedidoXmlNotasFiscaisPorCargar");

                Dominio.Entidades.Embarcador.Cargas.Carga carga = pedidoXmlNotasFiscaisPorCarga.FirstOrDefault().CargaPedido.Carga;

                Dominio.ObjetosDeValor.Embarcador.Integracao.Correios.CabecalhoOcorrencias ocorrenciasCorreios = Integracao.Correios.IntegracaoCorreios.ConsultarOcorrenciaPostagem(pedido, configuracaoCorreios, unitOfWork);
                if (ocorrenciasCorreios != null && ocorrenciasCorreios.objetos.Count > 0 && ocorrenciasCorreios.objetos.FirstOrDefault().eventos != null && ocorrenciasCorreios.objetos.FirstOrDefault().eventos.Count > 0)
                {
                    List<Dominio.ObjetosDeValor.Embarcador.Integracao.Correios.Evento> eventosNovos = pedido.DataUltimaConsultaCorreios.HasValue ? ocorrenciasCorreios.objetos.FirstOrDefault().eventos.Where(o => o.dtHrCriado >= pedido.DataUltimaConsultaCorreios.Value).OrderBy(o => o.dtHrCriado).ToList() : ocorrenciasCorreios.objetos.FirstOrDefault().eventos.OrderBy(o => o.dtHrCriado).ToList();

                    for (int i = 0; i < eventosNovos.Count; i++)
                    {
                        try
                        {
                            string codigoOcorrenciaCorreios = string.Concat(eventosNovos[i].codigo, eventosNovos[i].tipo);
                            string descricaoOcorrencia = eventosNovos[i].descricao;
                            DateTime dataOcorrencia = eventosNovos[i].dtHrCriado;

                            Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoDeOcorrencia = transportador?.EmpresaIntelipostTipoOcorrencia.Where(p => p.CodigoIntegracao == codigoOcorrenciaCorreios).FirstOrDefault()?.TipoOcorrencia ?? null;

                            if (tipoDeOcorrencia == null)
                                throw new ServicoException("Ocorrência " + codigoOcorrenciaCorreios + " não encontrada para o transportador " + transportador?.CNPJ);

                            unitOfWork.Start();

                            serOcorrencia.GerarPedidoOcorrenciaNotas(pedidoXmlNotasFiscaisPorCarga, tipoDeOcorrencia, carga, dataOcorrencia, "", "", "", OrigemSituacaoEntrega.Correios, "", 0, tipoServicoMultisoftware, configuracao, null, clienteMultisoftware, unitOfWork, auditado);

                            unitOfWork.CommitChanges();
                        }
                        catch (ServicoException excecao)
                        {
                            unitOfWork.Rollback();
                            Log.TratarErro("Pedido código " + pedido.Codigo + " - " + excecao, "ProcessarIntegracaCorreios");
                        }
                        catch (Exception excecao)
                        {
                            unitOfWork.Rollback();
                            Log.TratarErro("Pedido código " + pedido.Codigo + " - " + excecao, "ProcessarIntegracaCorreios");
                        }
                    }
                }
                else
                    Log.TratarErro("Pedido código " + pedido.Codigo + " - Sem eventos", "ProcessarIntegracaCorreios");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex, "ProcessarIntegracaCorreios");
            }

            pedido.DataUltimaConsultaCorreios = DateTime.Now;
            repPedido.Atualizar(pedido);

            unitOfWork.FlushAndClear();
        }

        private void ProcessarCTesSubcontratacaoNOTFIS(ref string mensagem, Dominio.ObjetosDeValor.EDI.Notfis.EDINotFis notfis, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork adminUnitOfWork, Dominio.Entidades.LayoutEDI layoutEDI, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Servicos.Embarcador.Carga.CTeSubContratacao serCargaCteParaSubContratacao = new Servicos.Embarcador.Carga.CTeSubContratacao(unitOfWork);

            if (notfis.CabecalhoDocumento != null && notfis.CabecalhoDocumento.Embarcadores != null && notfis.CabecalhoDocumento.Embarcadores.Count > 0)
            {
                foreach (Dominio.ObjetosDeValor.EDI.Notfis.Embarcador remetente in notfis.CabecalhoDocumento.Embarcadores)
                {
                    foreach (Dominio.ObjetosDeValor.EDI.Notfis.Destinatario destinatario in remetente.Destinatarios)
                    {
                        List<string> chavesCTes = destinatario.NotasFiscais.Select(o => o.CTe.Chave).Distinct().ToList();

                        foreach (string chaveCTe in chavesCTes)
                        {
                            List<Dominio.ObjetosDeValor.EDI.Notfis.NotaFiscal> notasFiscaisCTe = destinatario.NotasFiscais.Where(o => o.CTe.Chave == chaveCTe).ToList();

                            Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte = notasFiscaisCTe[0].CTe;

                            cte.Remetente = SetarDadosPessoa(remetente.Pessoa, adminUnitOfWork, unitOfWork);
                            cte.Destinatario = SetarDadosPessoa(destinatario.Pessoa, adminUnitOfWork, unitOfWork);
                            cte.Tomador = SetarDadosPessoa(notasFiscaisCTe[0].ResponsavelFrete.Pessoa, adminUnitOfWork, unitOfWork);
                            cte.Emitente = Servicos.Embarcador.Pessoa.Pessoa.Converter(cte.Tomador);

                            if (destinatario.Recebedor != null)
                                cte.Recebedor = SetarDadosPessoa(destinatario.Recebedor.Pessoa, adminUnitOfWork, unitOfWork);

                            cte.LocalidadeInicioPrestacao = remetente.Pessoa.Endereco.Cidade;
                            cte.LocalidadeFimPrestacao = destinatario.Pessoa.Endereco.Cidade;

                            if (layoutEDI != null && layoutEDI.UtilizarTomadorComoExpedidor)
                                cte.Expedidor = cte.Tomador;

                            if (cte.Recebedor != null)
                                cte.LocalidadeFimPrestacao = cte.Recebedor.Endereco.Cidade;

                            if (cte.Expedidor != null)
                                cte.LocalidadeInicioPrestacao = cte.Expedidor.Endereco.Cidade;

                            cte.ValorFrete = new Dominio.ObjetosDeValor.Embarcador.Frete.FreteValor()
                            {
                                ValorPrestacaoServico = notasFiscaisCTe.Sum(o => o.NFe.ValorFrete),
                                ValorTotalAReceber = notasFiscaisCTe.Sum(o => o.NFe.ValorFrete)
                            };

                            cte.TipoTomador = Dominio.Enumeradores.TipoTomador.Outros;
                            cte.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Outros;

                            cte.NumeroRomaneio = notasFiscaisCTe[0].NumeroRomaneio;
                            cte.NumeroPedido = notasFiscaisCTe[0].NumeroPedido;

                            if (cte.NFEs == null || cte.NFEs.Count <= 0)
                            {
                                cte.NFEs = new List<Dominio.ObjetosDeValor.Embarcador.CTe.NFe>();

                                foreach (Dominio.ObjetosDeValor.EDI.Notfis.NotaFiscal notaFiscal in notasFiscaisCTe)
                                {
                                    DateTime dataEmissao;
                                    if (!DateTime.TryParseExact(notaFiscal.NFe.DataEmissao, "ddMMyyyy", null, DateTimeStyles.None, out dataEmissao))
                                        dataEmissao = DateTime.Now;

                                    int numero = notaFiscal.NFe.Numero;
                                    if (numero <= 0 && !string.IsNullOrWhiteSpace(notaFiscal.NFe.Chave) && notaFiscal.NFe.Chave.Length >= 44)
                                        int.TryParse(notaFiscal.NFe.Chave.Substring(25, 9), out numero);

                                    cte.NFEs.Add(new Dominio.ObjetosDeValor.Embarcador.CTe.NFe()
                                    {
                                        Chave = notaFiscal.NFe.Chave.Length > 44 ? notaFiscal.NFe.Chave.Substring(0, 44) : notaFiscal.NFe.Chave,
                                        DataEmissao = dataEmissao,
                                        Numero = numero,
                                        Peso = notaFiscal.NFe.PesoBruto,
                                        Valor = notaFiscal.NFe.Valor,
                                        NumeroPedido = notaFiscal.NumeroPedido,
                                        NumeroRomaneio = notaFiscal.NumeroRomaneio,
                                        NumeroReferenciaEDI = (!string.IsNullOrWhiteSpace(notaFiscal.NFe.NumeroReferenciaEDI) ? notaFiscal.NFe.NumeroReferenciaEDI : notaFiscal.NumeroReferenciaEDI),
                                        PINSuframa = (!string.IsNullOrWhiteSpace(notaFiscal.NFe.PINSuframa) ? notaFiscal.NFe.PINSuframa : notaFiscal.PINSuframa),
                                        NCMPredominante = (!string.IsNullOrWhiteSpace(notaFiscal.NFe.NCMPredominante) ? notaFiscal.NFe.NCMPredominante : ""),
                                        NumeroControleCliente = (!string.IsNullOrWhiteSpace(notaFiscal.NFe.NumeroControleCliente) ? notaFiscal.NFe.NumeroControleCliente : notaFiscal.NumeroControleCliente)
                                    });
                                }
                            }

                            cte.InformacaoCarga = new Dominio.ObjetosDeValor.Embarcador.CTe.InformacaoCarga()
                            {
                                ValorTotalCarga = cte.NFEs.Sum(o => o.Valor)
                            };

                            cte.QuantidadesCarga = new List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga>()
                                {
                                    new Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga()
                                    {
                                         Unidade = Dominio.Enumeradores.UnidadeMedida.KG,
                                         Medida = "Quilograma",
                                         Quantidade = cte.NFEs.Sum(o => o.Peso)
                                    }
                                };

                            //string retorno = serCargaCteParaSubContratacao.InformarDadosCTeNaCarga(unitOfWork, cte, cargaPedido, tipoServicoMultisoftware);
                            string retorno = "";
                            Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteParaSubContratacao = serCargaCteParaSubContratacao.CriarCTeTerceiro(unitOfWork, ref retorno, null, cte, "", false, 0, true);

                            if (!string.IsNullOrWhiteSpace(retorno))
                                mensagem = retorno;
                        }
                    }
                }
            }
            else if (!string.IsNullOrWhiteSpace(notfis.ChaveCTeAnterior))
            {
                List<string> chavesCTes = new List<string>();
                chavesCTes.Add(notfis.ChaveCTeAnterior);

                foreach (string chaveCTe in chavesCTes)
                {
                    List<Dominio.ObjetosDeValor.EDI.Notfis.NotaFiscal> notasFiscaisCTe = notfis.NotasFiscais.ToList();

                    Dominio.ObjetosDeValor.EDI.Notfis.NotaFiscal primeiraNotaFiscal = notasFiscaisCTe[0];
                    Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte = new Dominio.ObjetosDeValor.Embarcador.CTe.CTe();

                    cte.Chave = notfis.ChaveCTeAnterior;
                    cte.Serie = notfis.SerieCTEAnterior;
                    int.TryParse(notfis.NumeroCTeAnterior, out int numeroCTe);
                    cte.Numero = numeroCTe;
                    cte.Remetente = SetarDadosPessoa(notfis.Participantes.Remetente.Pessoa, adminUnitOfWork, unitOfWork);
                    cte.Destinatario = SetarDadosPessoa(notfis.Participantes.Destinatario.Pessoa, adminUnitOfWork, unitOfWork);
                    cte.Tomador = SetarDadosPessoa(notasFiscaisCTe[0].ResponsavelFrete.Pessoa, adminUnitOfWork, unitOfWork);
                    cte.Emitente = Servicos.Embarcador.Pessoa.Pessoa.Converter(cte.Tomador);

                    if (primeiraNotaFiscal.Recebedor != null)
                        cte.Recebedor = SetarDadosPessoa(primeiraNotaFiscal.Recebedor.Pessoa, adminUnitOfWork, unitOfWork);

                    cte.LocalidadeInicioPrestacao = notfis.Participantes.Remetente.Pessoa.Endereco.Cidade;
                    cte.LocalidadeFimPrestacao = notfis.Participantes.Destinatario.Pessoa.Endereco.Cidade;

                    if (layoutEDI != null && layoutEDI.UtilizarTomadorComoExpedidor)
                        cte.Expedidor = cte.Tomador;

                    if (cte.Recebedor != null)
                        cte.LocalidadeFimPrestacao = cte.Recebedor.Endereco.Cidade;

                    if (cte.Expedidor != null)
                        cte.LocalidadeInicioPrestacao = cte.Expedidor.Endereco.Cidade;

                    cte.ValorFrete = new Dominio.ObjetosDeValor.Embarcador.Frete.FreteValor()
                    {
                        ValorPrestacaoServico = notasFiscaisCTe.Sum(o => o.NFe.ValorFrete),
                        ValorTotalAReceber = notasFiscaisCTe.Sum(o => o.NFe.ValorFrete)
                    };

                    cte.TipoTomador = Dominio.Enumeradores.TipoTomador.Outros;
                    cte.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Outros;

                    cte.NumeroRomaneio = notasFiscaisCTe[0].NumeroRomaneio;
                    cte.NumeroPedido = notasFiscaisCTe[0].NumeroPedido;

                    if (cte.NFEs == null || cte.NFEs.Count <= 0)
                    {
                        cte.NFEs = new List<Dominio.ObjetosDeValor.Embarcador.CTe.NFe>();

                        foreach (Dominio.ObjetosDeValor.EDI.Notfis.NotaFiscal notaFiscal in notasFiscaisCTe)
                        {
                            DateTime dataEmissao;
                            if (!DateTime.TryParseExact(notaFiscal.NFe.DataEmissao, "ddMMyyyy", null, DateTimeStyles.None, out dataEmissao))
                                dataEmissao = DateTime.Now;

                            int numero = notaFiscal.NFe.Numero;
                            if (numero <= 0 && !string.IsNullOrWhiteSpace(notaFiscal.NFe.Chave) && notaFiscal.NFe.Chave.Length >= 44)
                                int.TryParse(notaFiscal.NFe.Chave.Substring(25, 9), out numero);

                            cte.NFEs.Add(new Dominio.ObjetosDeValor.Embarcador.CTe.NFe()
                            {
                                Chave = notaFiscal.NFe.Chave.Length > 44 ? notaFiscal.NFe.Chave.Substring(0, 44) : notaFiscal.NFe.Chave,
                                DataEmissao = dataEmissao,
                                Numero = numero,
                                Peso = notaFiscal.NFe.PesoBruto,
                                Valor = notaFiscal.NFe.Valor,
                                NumeroPedido = notaFiscal.NumeroPedido,
                                NumeroRomaneio = notaFiscal.NumeroRomaneio,
                                NumeroReferenciaEDI = (!string.IsNullOrWhiteSpace(notaFiscal.NFe.NumeroReferenciaEDI) ? notaFiscal.NFe.NumeroReferenciaEDI : notaFiscal.NumeroReferenciaEDI),
                                PINSuframa = (!string.IsNullOrWhiteSpace(notaFiscal.NFe.PINSuframa) ? notaFiscal.NFe.PINSuframa : notaFiscal.PINSuframa),
                                NCMPredominante = (!string.IsNullOrWhiteSpace(notaFiscal.NFe.NCMPredominante) ? notaFiscal.NFe.NCMPredominante : ""),
                                NumeroControleCliente = (!string.IsNullOrWhiteSpace(notaFiscal.NFe.NumeroControleCliente) ? notaFiscal.NFe.NumeroControleCliente : notaFiscal.NumeroControleCliente)
                            });
                        }
                    }

                    cte.InformacaoCarga = new Dominio.ObjetosDeValor.Embarcador.CTe.InformacaoCarga()
                    {
                        ValorTotalCarga = cte.NFEs.Sum(o => o.Valor)
                    };

                    cte.QuantidadesCarga = new List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga>()
                                {
                                    new Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga()
                                    {
                                         Unidade = Dominio.Enumeradores.UnidadeMedida.KG,
                                         Medida = "Quilograma",
                                         Quantidade = cte.NFEs.Sum(o => o.Peso)
                                    }
                                };

                    //string retorno = serCargaCteParaSubContratacao.InformarDadosCTeNaCarga(unitOfWork, cte, cargaPedido, tipoServicoMultisoftware);
                    string retorno = "";
                    Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteParaSubContratacao = serCargaCteParaSubContratacao.CriarCTeTerceiro(unitOfWork, ref retorno, null, cte, "", false, 0);

                    if (!string.IsNullOrWhiteSpace(retorno))
                        mensagem = retorno;
                }
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa SetarDadosPessoa(Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa pessoa, AdminMultisoftware.Repositorio.UnitOfWork adminUnitOfWork, Repositorio.UnitOfWork unidadeTrabalho)
        {
            bool adminUnitOfWorkStarted = false;
            string[] splitEnderecorEmitente = pessoa.Endereco.Logradouro.Split(',');
            pessoa.Endereco.Logradouro = splitEnderecorEmitente[0].Trim();

            if (string.IsNullOrWhiteSpace(pessoa.Endereco.Logradouro) || pessoa.Endereco.Logradouro.Length <= 2)
                pessoa.Endereco.Logradouro = "NAO INFORMADO";

            if (splitEnderecorEmitente.Length > 1)
            {
                string[] splitNumero = splitEnderecorEmitente[1].Split('-');
                pessoa.Endereco.Numero = splitNumero[0].Trim().Replace("-", "");

                if (pessoa.Endereco.Numero == "0")
                    pessoa.Endereco.Numero = "S/N";
                if (splitNumero.Count() > 1)
                    pessoa.Endereco.Complemento = splitNumero[1].Trim();
            }
            else
            {
                if (string.IsNullOrWhiteSpace(pessoa.Endereco.Numero))
                    pessoa.Endereco.Numero = "S/N";
            }

            AdminMultisoftware.Dominio.Entidades.Localidades.Endereco endereco = null;

            if (pessoa.Endereco.Bairro == null || pessoa.Endereco.Bairro.Length < 3 || string.IsNullOrWhiteSpace(pessoa.Endereco.Logradouro))
            {
                if (!string.IsNullOrWhiteSpace(pessoa.Endereco.CEP))
                {
                    if (!adminUnitOfWork.IsOpenSession())
                    {
                        adminUnitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(adminUnitOfWork.StringConexao);
                        adminUnitOfWorkStarted = true;
                    }

                    AdminMultisoftware.Repositorio.Localidades.Endereco repEndereco = new AdminMultisoftware.Repositorio.Localidades.Endereco(adminUnitOfWork);
                    endereco = repEndereco.BuscarCEP(int.Parse(Utilidades.String.OnlyNumbers(pessoa.Endereco.CEP)).ToString());
                    if (endereco != null)
                        pessoa.Endereco.Bairro = endereco.Bairro?.Descricao;

                    if (string.IsNullOrWhiteSpace(pessoa.Endereco.Logradouro))
                        pessoa.Endereco.Logradouro = endereco.Logradouro;
                }
            }

            if (pessoa.Endereco.Cidade.IBGE <= 0)
            {
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeTrabalho);
                Dominio.Entidades.Localidade localidade = repLocalidade.BuscarPorDescricaoEUF(pessoa.Endereco.Cidade.Descricao, pessoa.Endereco.Cidade.SiglaUF);

                if (localidade != null)
                {
                    pessoa.Endereco.Cidade.IBGE = localidade.CodigoIBGE;
                }
                else
                {
                    if (endereco == null)
                    {
                        if (!adminUnitOfWork.IsOpenSession())
                        {
                            adminUnitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(adminUnitOfWork.StringConexao);
                            adminUnitOfWorkStarted = true;
                        }

                        AdminMultisoftware.Repositorio.Localidades.Endereco repEndereco = new AdminMultisoftware.Repositorio.Localidades.Endereco(adminUnitOfWork);
                        endereco = repEndereco.BuscarCEP(int.Parse(Utilidades.String.OnlyNumbers(pessoa.Endereco.CEP)).ToString());
                    }

                    if (endereco != null)
                    {
                        int.TryParse(endereco.Localidade.CodigoIBGE, out int codigoIBGE);
                        pessoa.Endereco.Cidade.IBGE = codigoIBGE;
                    }
                }
            }

            pessoa.CPFCNPJ = Utilidades.String.OnlyNumbers(pessoa.CPFCNPJ);
            pessoa.AtualizarEnderecoPessoa = false;

            if (pessoa.CPFCNPJ.Length >= 14)
            {
                pessoa.CPFCNPJ = pessoa.CPFCNPJ.Substring(pessoa.CPFCNPJ.Length - 14);
                if (Utilidades.Validate.ValidarCNPJ(pessoa.CPFCNPJ))
                    pessoa.TipoPessoa = Dominio.Enumeradores.TipoPessoa.Juridica;
                else
                {
                    pessoa.CPFCNPJ = pessoa.CPFCNPJ.Substring(pessoa.CPFCNPJ.Length - 11);

                    if (string.IsNullOrWhiteSpace(pessoa.RGIE))
                        pessoa.RGIE = "ISENTO";

                    pessoa.TipoPessoa = Dominio.Enumeradores.TipoPessoa.Fisica;
                }
            }
            else
                pessoa.TipoPessoa = Dominio.Enumeradores.TipoPessoa.Fisica;

            if (adminUnitOfWorkStarted) adminUnitOfWork.Dispose();
            return pessoa;
        }

        private List<List<Dominio.Entidades.Embarcador.Pedidos.Pedido>> AgruparPedidosPorVeiculoOperacao(List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos)
        {
            List<List<Dominio.Entidades.Embarcador.Pedidos.Pedido>> pedidosAgrupados = new List<List<Dominio.Entidades.Embarcador.Pedidos.Pedido>>();
            int totalPedidos = pedidos?.Count ?? 0;
            if (totalPedidos > 0)
            {

                List<int> codigosVeiculosDistintos = new List<int>();
                List<int> codigosOperacoesDistintas = new List<int>();
                for (int i = 0; i < totalPedidos; i++)
                {
                    Dominio.Entidades.Veiculo veiculo = pedidos[i].Veiculos.First();
                    if (veiculo != null && !codigosVeiculosDistintos.Contains(veiculo.Codigo)) codigosVeiculosDistintos.Add(veiculo.Codigo);

                    int codigoTipoOperacao = pedidos[i].TipoOperacao?.Codigo ?? 0;
                    if (!codigosOperacoesDistintas.Contains(codigoTipoOperacao)) codigosOperacoesDistintas.Add(codigoTipoOperacao);
                }

                // Percorre os veículos distintos
                int totalVeiculos = codigosVeiculosDistintos.Count;
                int totalOperacoes = codigosOperacoesDistintas.Count;
                for (int i = 0; i < totalVeiculos; i++)
                {
                    for (int j = 0; j < totalOperacoes; j++)
                    {
                        List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosVeiculoOperacao = (from obj in pedidos where (obj.TipoOperacao?.Codigo ?? 0) == codigosOperacoesDistintas[j] && obj.Veiculos.Any(v => v.Codigo == codigosVeiculosDistintos[i]) select obj).ToList();
                        pedidosAgrupados.Add(pedidosVeiculoOperacao);
                    }
                }
            }
            return pedidosAgrupados;
        }

        private List<List<Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedidoLinha>> AgruparLinhasPorVeiculoOperacao(List<Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedidoLinha> linhas)
        {
            List<List<Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedidoLinha>> linhasAgrupadas = new List<List<Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedidoLinha>>();

            if ((linhas?.Count ?? 0) <= 0)
                return linhasAgrupadas;

            List<(int codigoVeiculo, int codigoTipoOperacao)> retorno = linhas.Select(o => (codigoVeiculo: o.Pedido.Veiculos?.FirstOrDefault()?.Codigo ?? 0, codigoTipoOperacao: o.Pedido.TipoOperacao?.Codigo ?? 0)).Distinct().ToList();

            foreach ((int codigoVeiculo, int codigoTipoOperacao) in retorno)
            {
                linhasAgrupadas.Add(linhas.Where(o => (o.Pedido.TipoOperacao?.Codigo ?? 0) == codigoTipoOperacao && (o.Pedido.Veiculos?.FirstOrDefault()?.Codigo ?? 0) == codigoVeiculo).ToList());
            }

            return linhasAgrupadas;
        }

        private List<Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedidoLinha> AgruparPorPedidoShopee(List<Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedidoLinha> linhas)
        {
            List<Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedidoLinha> linhasAgrupadas = new List<Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedidoLinha>();

            if ((linhas?.Count ?? 0) <= 0)
                return linhasAgrupadas;

            linhasAgrupadas.AddRange(linhas.Where(o => o.Colunas.Any(x => x.NomeCampo == "PedidosShopee" && !string.IsNullOrWhiteSpace(x.Valor))).ToList());

            return linhasAgrupadas;
        }

        private string ObterCodigoCargaEmbarcador(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            return ((!configuracaoTMS.NumeroCargaSequencialUnico) ?
                Servicos.Embarcador.Cargas.CargaSequencial.GetInstance().ObterProximoNumeroSequencial(unitOfWork, pedido.Filial?.Codigo ?? 0)
                : Servicos.Embarcador.Cargas.CargaSequencial.GetInstance().ObterProximoNumeroSequencial(unitOfWork)).ToString();
        }

        ///<summary>
        /// Função destinada ao cancelamento de pedidos que podem possuir um ou mais agendamentos de coleta.
        ///</summary>
        private void CancelarPedidoAgendamentoColeta(string numeroPedido, out string mensagemCancelamentoPedido, Dominio.Entidades.Usuario usuario, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, UnitOfWork unitOfWork)
        {
            mensagemCancelamentoPedido = string.Empty;

            Repositorio.Embarcador.Logistica.AgendamentoColeta repositorioAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repositorioPedido.BuscarPorNumeroEmbarcador(numeroPedido, 0, "", null, false);

            if (pedido == null || pedido.SituacaoPedido == SituacaoPedido.Cancelado)
            {
                mensagemCancelamentoPedido = $"O pedido {numeroPedido} não consta no sistema ou já está cancelado.";
                return;
            }

            bool possuiAgendamento = repositorioAgendamentoColeta.BuscarSePossuiAgendamentoPorPedido(pedido.Codigo);

            if (possuiAgendamento)
            {
                mensagemCancelamentoPedido = "Não é possível cancelar o pedido pois o mesmo se encontra em um agendamento de coleta que não está cancelado.";
                return;
            }

            RealizarCancelamentoPedido(out mensagemCancelamentoPedido, ref pedido, usuario, "Pedido cancelado pela importação de planilha.", unitOfWork, tipoServicoMultisoftware, null, auditado);
        }

        private void CriarCargaPedido(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura faixaTemperatura, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            if (carga == null)
                return;

            //Carga.Carga servicoCarga = new(unitOfWork);
            //servicoCarga.CriarCargaPedidosPorPedidos(ref carga, new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>() { pedido }, tipoServicoMultisoftware, null, unitOfWork, configuracao, auditado);

            Carga.CargaPedido servicoCargaPedido = new(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = servicoCargaPedido.AdicionarPedidoCarga(carga, pedido, configuracao, tipoServicoMultisoftware);

            FinalizarAlteracaoCargaPedido(carga, new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>() { cargaPedido }, faixaTemperatura, unitOfWork, tipoServicoMultisoftware);
        }

        private void AtualizarCargaPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura faixaTemperatura, bool atualizarCarga)
        {
            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador) //De momento a implementação será apenas para embarcador
                return;

            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorPedidoNaCarga(pedido.Codigo);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                if (cargaPedido.Pedido.Codigo != pedido.Codigo)
                    continue;

                if (!servicoCarga.VerificarSeCargaEstaNaLogistica(cargaPedido.Carga, tipoServicoMultisoftware))
                    continue;

                //TODO: PPC - Adicionado log temporário para identificar problema no cargaPedido.Peso.
                Servicos.Log.TratarErro($"Pedido {cargaPedido.Pedido.NumeroPedidoEmbarcador} - CargaPedido.Codigo = {cargaPedido.Codigo} - Peso Total De.: {cargaPedido.Peso} - Para.: {pedido.PesoTotal}. Pedido.AtualizarCargaPedido", "PesoCargaPedido");

                cargaPedido.Peso = pedido.PesoTotal;
                cargaPedido.QtVolumes = pedido.QtVolumes;

                repositorioCargaPedido.Atualizar(cargaPedido);
            }

            if (carga != null)
            {
                if (atualizarCarga)
                    AlterarAtributosDaCargaConformePedido(pedido, carga);

                FinalizarAlteracaoCargaPedido(carga, cargaPedidos, faixaTemperatura, unitOfWork, tipoServicoMultisoftware);
            }
            else
            {
                for (int i = 0; i < cargaPedidos.Count; i++)
                {
                    if (atualizarCarga)
                        AlterarAtributosDaCargaConformePedido(pedido, cargaPedidos[i].Carga);

                    FinalizarAlteracaoCargaPedido(cargaPedidos[i].Carga, cargaPedidos, faixaTemperatura, unitOfWork, tipoServicoMultisoftware);
                }
            }
        }

        private void AlterarAtributosDaCargaConformePedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            carga.ModeloVeicularCarga = pedido.ModeloVeicularCarga ?? carga.ModeloVeicularCarga;
            carga.Empresa = pedido.Empresa ?? carga.Empresa;
            carga.DataCarregamentoCarga = pedido.DataCarregamentoPedido ?? carga.DataCarregamentoCarga;
        }

        private void FinalizarAlteracaoCargaPedido(
            Dominio.Entidades.Embarcador.Cargas.Carga carga,
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos,
            Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura faixaTemperatura,
            Repositorio.UnitOfWork unitOfWork,
            AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Servicos.Embarcador.Carga.CargaDadosSumarizados cargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosPorCarga = cargaPedidos.Where(cargaPedido => cargaPedido.Carga.Codigo == carga.Codigo).ToList();

            AtualizarCarga(carga, faixaTemperatura, unitOfWork);
            cargaDadosSumarizados.AtualizarPesos(carga, cargaPedidosPorCarga, unitOfWork, tipoServicoMultisoftware);
        }

        private Dominio.ObjetosDeValor.Localidade retornarLocalidade(Dominio.Entidades.Localidade localidade)
        {
            if (localidade != null)
            {
                return new Dominio.ObjetosDeValor.Localidade()
                {
                    Codigo = localidade.Codigo,
                    CodigoIntegracao = localidade.CodigoLocalidadeEmbarcador,
                    Descricao = localidade.DescricaoCidadeEstado,
                    IBGE = localidade.CodigoIBGE
                };
            }
            else
            {
                return null;
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoEndereco retornarPedidoEndereco(Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco pedidoEnderecoEntidade)
        {
            if (pedidoEnderecoEntidade != null)
            {
                Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoEndereco enderecoPedido = new Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoEndereco();
                enderecoPedido.Bairro = pedidoEnderecoEntidade.Bairro;
                enderecoPedido.CEP = pedidoEnderecoEntidade.CEP;
                enderecoPedido.CodigoEnderecoEmbarcador = pedidoEnderecoEntidade.ClienteOutroEndereco != null ? pedidoEnderecoEntidade.ClienteOutroEndereco.CodigoEmbarcador : "";
                enderecoPedido.Complemento = pedidoEnderecoEntidade.Complemento;
                enderecoPedido.CPFCNPJ = pedidoEnderecoEntidade.ClienteOutroEndereco != null ? pedidoEnderecoEntidade.ClienteOutroEndereco.Cliente.CPF_CNPJ.ToString() : "";
                enderecoPedido.Localidade = new Dominio.ObjetosDeValor.Localidade()
                {
                    Codigo = pedidoEnderecoEntidade.Localidade.Codigo,
                    CodigoIntegracao = pedidoEnderecoEntidade.Localidade.CodigoLocalidadeEmbarcador,
                    Descricao = pedidoEnderecoEntidade.Localidade.DescricaoCidadeEstado,
                    IBGE = pedidoEnderecoEntidade.Localidade.CodigoIBGE
                };

                enderecoPedido.Logradouro = pedidoEnderecoEntidade.Endereco;
                enderecoPedido.Numero = pedidoEnderecoEntidade.Numero;
                enderecoPedido.Telefone = pedidoEnderecoEntidade.Telefone;
                return enderecoPedido;
            }
            else
            {
                return null;
            }
        }

        private void AtualizarCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura faixaTemperatura, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            if (faixaTemperatura != null)
            {
                carga.FaixaTemperatura = faixaTemperatura;

                repositorioCarga.Atualizar(carga);
            }
        }

        public void EnviarEmailPedidoComEntregaPendente(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repConfigPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configPedido = repConfigPedido.BuscarConfiguracaoPadrao();

            if (!configPedido?.EnviarEmailTransportadorEntregaEmAtraso ?? false)
                return;

            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = repPedido.BuscarPedidosComEntregaPendente(DateTime.Now.Date.AddDays(-1));

            List<Dominio.Entidades.Empresa> transportadores = pedidos.Select(ped => ped.Empresa).Distinct().ToList();

            if (pedidos.Count == 0 || pedidos == null)
                return;

            string enderecoLogoEmpresa = ObterCaminhoLogoCliente(unitOfWork);
            string subject = "Pedido com entrega Pendente";

            foreach (Dominio.Entidades.Empresa transportador in transportadores)
            {
                if (transportador == null)
                    continue;

                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosTransportador = pedidos.Where(o => (o.Empresa?.Codigo ?? 0) == transportador.Codigo).ToList();
                System.Text.StringBuilder mensagemEmail = new System.Text.StringBuilder();

                mensagemEmail.AppendLine($"<img src='{enderecoLogoEmpresa}' alt='Logo' title='Logo' style='outline: none;text-decoration: none;-ms-interpolation-mode: bicubic;clear: both;display: inline-block !important;border: none;height: auto;float: none;width: 100%;max-width: 200px;' width='480'/> {Environment.NewLine} ")
                             .AppendLine($"Atenção! {Environment.NewLine}")
                             .AppendLine($"Os pedidos abaixo constam como pendentes: {Environment.NewLine}");

                foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidosTransportador)
                {
                    if (pedido == null)
                        continue;

                    mensagemEmail.AppendLine($"Pedido: {pedido.NumeroPedidoEmbarcador} - Data de previsão de entrega: {pedido.PrevisaoEntrega}");

                }
                mensagemEmail.AppendLine($"")
                             .AppendLine($"Por favor verifique o status destas entregas, aguardamos a integração das ocorrências de entrega. {Environment.NewLine}")
                             .AppendLine($"E-mail enviado automaticamente. Por favor, não responda.");

                string emailTransportadora = transportador?.Email ?? "";

                if (!Servicos.Email.EnviarEmailAutenticado(emailTransportadora, subject, mensagemEmail.ToString(), unitOfWork, out string msg, "", null, null, null))
                    Servicos.Log.TratarErro("Falha ao enviar email de aviso de entrega em atraso: " + msg);

            }
        }

        private string ObterCaminhoLogoCliente(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo repConfigArquivo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoArquivo configArquivo = repConfigArquivo.BuscarPrimeiroRegistro();

            return configArquivo?.CaminhoLogoEmbarcador ?? "";
        }

        private TendenciaEntrega ObterTendenciaEntregaPedido(DateTime? dataReferencia, Dominio.Entidades.Embarcador.Cargas.AcompanhamentoEntregaTempoConfiguracao.AcompanhamentoEntregaTempoConfiguracao configuracaoTempoTendendicas)
        {
            if (!dataReferencia.HasValue || configuracaoTempoTendendicas == null)
                return TendenciaEntrega.Nenhum;

            double diffMinutos = (DateTime.Now - dataReferencia.Value).TotalMinutes;

            if (diffMinutos <= configuracaoTempoTendendicas.DestinoEmTempo.TotalMinutes)
                return TendenciaEntrega.Adiantado;

            if (diffMinutos <= configuracaoTempoTendendicas.DestinoAtraso1.TotalMinutes && diffMinutos > configuracaoTempoTendendicas.DestinoEmTempo.TotalMinutes)
                return TendenciaEntrega.Nohorario;

            if (diffMinutos <= configuracaoTempoTendendicas.DestinoAtraso2.TotalMinutes && diffMinutos > configuracaoTempoTendendicas.DestinoAtraso1.TotalMinutes)
                return TendenciaEntrega.Poucoatrasado;

            if (diffMinutos > configuracaoTempoTendendicas.DestinoAtraso3.TotalMinutes)
                return TendenciaEntrega.Atrasado;

            return TendenciaEntrega.Nenhum;
        }

        private Dominio.Entidades.Cliente AjustarTomador(Dominio.Entidades.Cliente possivelTomador, Dominio.ObjetosDeValor.Embarcador.Pedido.Participantes participantes, ref TipoTomador tipoTomador)
        {
            if (possivelTomador == null)
                return participantes.Tomador;

            if (participantes.Remetente.Codigo == possivelTomador.Codigo)
                tipoTomador = TipoTomador.Remetente;

            else if (participantes.Expedidor != null && participantes.Expedidor.Codigo == possivelTomador.Codigo)
                tipoTomador = TipoTomador.Expedidor;

            else if (participantes.Recebedor != null && participantes.Recebedor.Codigo == possivelTomador.Codigo)
                tipoTomador = TipoTomador.Recebedor;

            else if (participantes.Destinatario != null && participantes.Destinatario.Codigo == possivelTomador.Codigo)
                tipoTomador = TipoTomador.Destinatario;

            else
            {
                participantes.Tomador = possivelTomador;
                tipoTomador = TipoTomador.Outros;
            }

            return participantes.Tomador;
        }

        private Dominio.Entidades.Cliente BuscarRemetentePelaChaveDaNFe(Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal nfe, Repositorio.UnitOfWork unitOfWork)
        {
            if (!(nfe?.Chave?.Length > 19 && double.TryParse(nfe.Chave.Substring(6, 14), out double cpfCcnpjDoRemetenteNFe)))
                return null;

            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            return repCliente.BuscarPorCPFCNPJ(cpfCcnpjDoRemetenteNFe);
        }

        private Dominio.Entidades.Cliente BuscarClientePorBuscaAutomatica(
            Dominio.ObjetosDeValor.Embarcador.ClienteBuscaAutomatica.ParametrosClienteBuscaAutomatica parametrosClienteBuscaAutomatica,
            Repositorio.UnitOfWork unitOfWork,
            CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Pessoas.ClienteBuscaAutomatica repositorioBuscaCliente = new(unitOfWork);
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);

            Dominio.Entidades.Cliente clienteCompativel = null;

            IList<Dominio.ObjetosDeValor.Embarcador.ClienteBuscaAutomatica.ParametrosClienteBuscaAutomatica> parametrosClienteBuscaAutomaticas = repositorioBuscaCliente.BuscarParaOrdenarPorCompatibilidade(cancellationToken).GetAwaiter().GetResult();

            if (parametrosClienteBuscaAutomaticas.Count > 0)
            {
                IList<Dominio.ObjetosDeValor.Embarcador.ClienteBuscaAutomatica.ParametrosClienteBuscaAutomatica> parametrosOrdenados = Utilidades.Object
                        .OrdenarEnumeravelPorCompatibilidade(parametrosClienteBuscaAutomatica, parametrosClienteBuscaAutomaticas)
                        .ToArray();

                if (parametrosOrdenados.Count > 0 && parametrosOrdenados[0].CPFCNPJCliente > 0)
                    clienteCompativel = repositorioCliente.BuscarPorCPFCNPJ(parametrosOrdenados[0].CPFCNPJCliente);
            }

            return clienteCompativel;
        }

        private void SalvarListaFronteiras(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, List<Dominio.Entidades.Cliente> listaFronteiras, Repositorio.UnitOfWork _unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoFronteira repositorioPedidoFronteira = new Repositorio.Embarcador.Pedidos.PedidoFronteira(_unitOfWork);
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(_unitOfWork);

            if (listaFronteiras?.Count == 0 && pedido.Fronteira == null)
                return;

            List<double> fronteirasAtuais = repositorioPedidoFronteira.BuscarCPFCNPJFronteirasPorPedido(pedido.Codigo);

            if (pedido.Fronteira != null)
            {
                if (!fronteirasAtuais.Contains(pedido.Fronteira.CPF_CNPJ))
                {
                    fronteirasAtuais.Add(pedido.Fronteira.CPF_CNPJ);
                    pedido.Fronteira = null;
                }
            }

            List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade> alteracoes = new List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade>();

            List<double> codigos = listaFronteiras?.Select(x => x.CPF_CNPJ).ToList() ?? new List<double>();

            List<double> fronteirasDeletarCodigos = fronteirasAtuais.Where(o => !codigos.Contains(o)).ToList();

            if (fronteirasDeletarCodigos?.Count() > 0)
            {
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoFronteira> fronteirasDeletar = repositorioPedidoFronteira.BuscarFronteirasPorPedidoCPFCNPJ(fronteirasDeletarCodigos, pedido.Codigo);
                if (fronteirasDeletar?.Count > 0)
                {
                    foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoFronteira fronteira in fronteirasDeletar)
                    {
                        alteracoes.Add(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                        {
                            Propriedade = "Fronteiras",
                            De = fronteira.Descricao ?? "",
                            Para = ""
                        });

                        repositorioPedidoFronteira.Deletar(fronteira);
                    }
                }
            }

            if (listaFronteiras?.Count > 0)
            {
                foreach (Dominio.Entidades.Cliente fronteira in listaFronteiras)
                {
                    double codigoFronteira = fronteira.Codigo;
                    bool jaExiste = fronteirasAtuais.Contains(codigoFronteira);

                    if (!jaExiste)
                    {
                        Dominio.Entidades.Embarcador.Pedidos.PedidoFronteira fronteiraInserir = new Dominio.Entidades.Embarcador.Pedidos.PedidoFronteira()
                        {
                            Pedido = pedido,
                            Fronteira = repositorioCliente.BuscarFronteiraPorCPFCNPJ(codigoFronteira)
                        };

                        repositorioPedidoFronteira.Inserir(fronteiraInserir);

                        alteracoes.Add(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                        {
                            Propriedade = "Fronteiras",
                            De = "",
                            Para = fronteiraInserir.Descricao ?? ""
                        });
                    }
                }
            }

            pedido.SetExternalChanges(alteracoes);
        }

        #endregion Métodos Privados
    }
}