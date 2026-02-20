using Confluent.Kafka;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Newtonsoft.Json;
using Repositorio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Servicos.Embarcador.CTe
{
    public class CTe : ServicoBase
    {               
        public CTe(UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken)
        {
        }

        private readonly string _stringConexao;

        #region Métodos Públicos

        public bool EmitirCTeManualmente(Dominio.ObjetosDeValor.Embarcador.CTe.CTe cteIntegracao, int codigoCarga, Dominio.Entidades.Usuario usuario, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ConfiguracaoEmbarcador, out string msgRetorno, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, string stringConexao, bool cteImportado)
        {
            msgRetorno = "";

            Servicos.Embarcador.Carga.ComponetesFrete serComponentesFrete = new Servicos.Embarcador.Carga.ComponetesFrete(unitOfWork);
            Servicos.Embarcador.Seguro.Seguro svcSeguro = new Servicos.Embarcador.Seguro.Seguro(unitOfWork);
            Servicos.Embarcador.Carga.CTe svcCargaCTe = new Servicos.Embarcador.Carga.CTe(unitOfWork);
            Servicos.Embarcador.Fatura.Fatura servFatura = new Servicos.Embarcador.Fatura.Fatura(unitOfWork);
            Servicos.Embarcador.Carga.CargaDadosSumarizados serCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);
            Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);

            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao repApoliceSeguroAverbacao = new Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao(unitOfWork);
            Repositorio.Embarcador.CTe.CTeRelacaoDocumento repCTeRelacaoDocumento = new Repositorio.Embarcador.CTe.CTeRelacaoDocumento(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            Repositorio.DocumentosCTE repDocumentosCTE = new Repositorio.DocumentosCTE(unitOfWork);
            Repositorio.EmpresaSerie repEmpresaSerie = new Repositorio.EmpresaSerie(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);
            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = null;

            if (ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal)
            {
                int codigoContainer = cteIntegracao.Containers != null && cteIntegracao.Containers.Count > 0 ? cteIntegracao.Containers.FirstOrDefault().Container : 0;
                if (codigoContainer > 0)
                    cargaPedido = repCargaPedido.BuscarPorContainerECarga(codigoContainer, codigoCarga);
            }

            if (cargaPedido == null)
                cargaPedido = repCargaPedido.BuscarPrimeiroPedidoPorCarga(carga.Codigo);

            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe> permissoes = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe>() {
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe.total
                    };

            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = null;

            bool exigirInformarCFOP = carga?.TipoOperacao?.ExigirInformarCFOP ?? false;
            bool exigirInformarNCM = carga?.TipoOperacao?.ExigirInformarNCM ?? false;
            bool exigirInformarPeso = carga?.TipoOperacao?.ExigirInformarPeso ?? false;
            if (!cteImportado)
            {
                if (cteIntegracao != null && cteIntegracao.NotasFiscais != null && cteIntegracao.NotasFiscais.Count > 0)
                {
                    if (exigirInformarCFOP && cteIntegracao.NotasFiscais.Any(o => o.CFOP == "" || o.CFOP == null))
                    {
                        msgRetorno = "Não foi informado a CFOP em todos os documentos deste CT-e.";
                        return false;
                    }
                    if (exigirInformarNCM && cteIntegracao.NotasFiscais.Any(o => o.NCMPredominante == "" || o.NCMPredominante == null))
                    {
                        msgRetorno = "Não foi informado o NCM predominante em todos os documentos deste CT-e.";
                        return false;
                    }
                    if (exigirInformarPeso && cteIntegracao.NotasFiscais.Any(o => o.Peso <= 0))
                    {
                        msgRetorno = "Não foi informado o Peso (Kg) em todos os documentos deste CT-e.";
                        return false;
                    }
                }
                if (cteIntegracao != null && cteIntegracao.NFEs != null && cteIntegracao.NFEs.Count > 0)
                {
                    if (exigirInformarCFOP && cteIntegracao.NFEs.Any(o => o.CFOP == "" || o.CFOP == null))
                    {
                        msgRetorno = "Não foi informado a CFOP em todos os documentos deste CT-e.";
                        return false;
                    }
                    if (exigirInformarNCM && cteIntegracao.NFEs.Any(o => o.NCMPredominante == "" || o.NCMPredominante == null))
                    {
                        msgRetorno = ("Não foi informado o NCM predominante em todos os documentos deste CT-e.");
                        return false;
                    }
                    if (exigirInformarPeso && cteIntegracao.NFEs.Any(o => o.Peso <= 0))
                    {
                        msgRetorno = ("Não foi informado o Peso (Kg) em todos os documentos deste CT-e.");
                        return false;
                    }
                }
                if (cteIntegracao != null && cteIntegracao.OutrosDocumentos != null && cteIntegracao.OutrosDocumentos.Count > 0)
                {
                    if (exigirInformarCFOP && cteIntegracao.OutrosDocumentos.Any(o => o.CFOP == "" || o.CFOP == null))
                    {
                        msgRetorno = ("Não foi informado a CFOP em todos os documentos deste CT-e.");
                        return false;
                    }
                    if (exigirInformarNCM && cteIntegracao.OutrosDocumentos.Any(o => o.NCMPredominante == "" || o.NCMPredominante == null))
                    {
                        msgRetorno = ("Não foi informado o NCM predominante em todos os documentos deste CT-e.");
                        return false;
                    }
                    if (exigirInformarPeso && cteIntegracao.OutrosDocumentos.Any(o => o.Peso <= 0))
                    {
                        msgRetorno = ("Não foi informado o Peso (Kg) em todos os documentos deste CT-e.");
                        return false;
                    }
                }
                if (cteIntegracao != null && cteIntegracao.Containers != null && cteIntegracao.Containers.Count > 0)
                {
                    var documentosContainer = cteIntegracao.Containers.Select(c => c.Documentos).ToList();
                    if (documentosContainer == null || documentosContainer.Count == 0)
                    {
                        msgRetorno = ("Não foi vinculado nenhum documento fiscal ao container deste CT-e.");
                        return false;
                    }

                    if (cteIntegracao != null && cteIntegracao.NFEs != null && cteIntegracao.NFEs.Count > 0)
                    {
                        foreach (var notaFiscal in cteIntegracao.NFEs)
                        {
                            if (!documentosContainer.Any(c => c.Any(p => p.Chave == notaFiscal.Chave)))
                            {
                                msgRetorno = ("A nota fiscal " + notaFiscal.Chave + " não foi vinculada ao container.");
                                return false;
                            }
                        }
                    }
                    else if (cteIntegracao != null && cteIntegracao.NotasFiscais != null && cteIntegracao.NotasFiscais.Count > 0)
                    {
                        foreach (var notaFiscal in cteIntegracao.NotasFiscais)
                        {
                            if (!documentosContainer.Any(c => c.Any(p => p.Numero == notaFiscal.Numero)))
                            {
                                msgRetorno = ("A nota fiscal " + notaFiscal.Numero + " (Outros Documentos) não foi vinculada ao container.");
                                return false;
                            }
                        }
                    }
                    else if (cteIntegracao != null && cteIntegracao.OutrosDocumentos != null && cteIntegracao.OutrosDocumentos.Count > 0)
                    {
                        foreach (var notaFiscal in cteIntegracao.OutrosDocumentos)
                        {
                            if (!documentosContainer.Any(c => c.Any(p => p.Numero == notaFiscal.Numero)))
                            {
                                msgRetorno = ("A nota fiscal " + notaFiscal.Numero + " (Outros Documentos) não foi vinculada ao container.");
                                return false;
                            }
                        }
                    }
                }

                if (cteIntegracao != null && cteIntegracao.Serie != null && cteIntegracao.Emitente != null)
                {
                    Dominio.Entidades.Empresa empresa = repEmpresa.BuscarEmpresaPorCNPJ(cteIntegracao.Emitente.CNPJ);
                    List<Dominio.Entidades.EmpresaSerie> empresaSerie = repEmpresaSerie.BuscarTodosPorEmpresa(empresa.Codigo, Dominio.Enumeradores.TipoSerie.CTe, "A");
                    if (!empresaSerie.Select(o => o.Numero).Contains(int.Parse(cteIntegracao.Serie)))
                    {
                        msgRetorno = "A série informada pertence à outra empresa, não sendo possível realizar a emissão do documento";
                        return false;
                    }
                }
            }

            unitOfWork.Start();

            if (cteIntegracao.Codigo == 0)
                cte = new Dominio.Entidades.ConhecimentoDeTransporteEletronico();
            else
            {
                cte = repCTe.BuscarPorCodigo(cteIntegracao.Codigo);
                if (cte == null)
                    cte = new Dominio.Entidades.ConhecimentoDeTransporteEletronico();
            }

            if (carga != null)
            {
                cte.CentroResultadoFaturamento = repCargaPedido.BuscarCentroResultadoPorCarga(carga.Codigo);
                cte.PossuiPedidoSubstituicao = repCargaPedido.PossuiPedidoSubstituicao(carga.Codigo);
            }

            cte.GeradoManualmente = true;

            SalvarDadosCTe(ref cte, cteIntegracao, cte.SituacaoCTeSefaz, permissoes, usuario, unitOfWork, carga?.CargaSVMTerceiro ?? false, Auditado);

            if (cteIntegracao.CodigoDuplicado > 0 && cte != null)
            {
                Dominio.Entidades.Embarcador.CTe.CTeRelacaoDocumento cTeRelacao = new Dominio.Entidades.Embarcador.CTe.CTeRelacaoDocumento()
                {
                    CTeGerado = cte,
                    CTeOriginal = repCTe.BuscarPorCodigo(cteIntegracao.CodigoDuplicado),
                    TipoCTeGerado = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCTeGerado.Duplicado
                };
                if (cTeRelacao.CTeOriginal != null)
                    repCTeRelacaoDocumento.Inserir(cTeRelacao);
            }

            if (ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal)
            {
                int? diasPrazoFatura = 0;
                int? diaMes = 0;
                bool? permiteFinalSemana = false;
                FormaTitulo formaTitulo = FormaTitulo.Outros;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana? diaSemana = null;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPrazoFaturamento? tipoPrazoFatura = null;

                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana> diasSemana = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana>();
                List<int> diasMes = new List<int>();

                servFatura.RetornarParametrosFaturamento(cte, unitOfWork, true, out diaMes, out diaSemana, out permiteFinalSemana, out diasPrazoFatura, out diasSemana, out diasMes, out tipoPrazoFatura, 0, out formaTitulo);
                DateTime? dataBaseParcela = servFatura.RetornarDataBase(tipoPrazoFatura, cte, true, unitOfWork);

                if (dataBaseParcela.HasValue && (diaMes != null || diaSemana != null || diasPrazoFatura != null || tipoPrazoFatura != null || diasMes.Count > 0 || diasSemana.Count > 0))
                {
                    cte.DataPreviaVencimento = servFatura.RetornaDataPadraoFatura(diaMes, diaSemana, permiteFinalSemana, dataBaseParcela, diasPrazoFatura, diasSemana, diasMes, cte.TomadorPagador.Cliente, cte.TomadorPagador.GrupoPessoas, false, unitOfWork);
                    repCTe.Atualizar(cte);
                }
            }

            Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCTe.BuscarPorCTe(cte.Codigo);

            if (cargaCTe == null)
            {
                cargaCTe = new Dominio.Entidades.Embarcador.Cargas.CargaCTe();
                cargaCTe.Carga = carga;
                cargaCTe.CargaOrigem = carga;
                cargaCTe.CTe = cte;
                cargaCTe.SistemaEmissor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor.MultiCTe;
                repCargaCTe.Inserir(cargaCTe);

                List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao> apolices = repApoliceSeguroAverbacao.BuscarPorCargaPedido(cargaPedido.Codigo, (carga.EmpresaFilialEmissora != null && carga.EmpresaFilialEmissora.Codigo == cte.Empresa.Codigo));

                svcCargaCTe.AverbaCargaCTe(cargaCTe, apolices, unitOfWork, ConfiguracaoEmbarcador, Dominio.Enumeradores.FormaAverbacaoCTE.Definitiva);
            }

            serComponentesFrete.MudarComplementoDaCargaCTe(cargaCTe, cteIntegracao.ValorFrete.ComponentesAdicionais, unitOfWork);

            if (cte.TipoCTE != Dominio.Enumeradores.TipoCTE.Anulacao)
                Servicos.Embarcador.Carga.Documentos.AdicionarCTeManualParaGeracaoDeMovimento(cargaCTe, "A", tipoServicoMultisoftware, unitOfWork);

            Servicos.Auditoria.Auditoria.Auditar(Auditado, cte, null, "Solicitado Emissão do CT-e", unitOfWork);

            cargaCTe = repCargaCTe.BuscarPorCTe(cte.Codigo);

            if (cargaCTe != null && cargaPedido != null)
            {
                List<Dominio.Entidades.DocumentosCTE> documentosCTEs = repDocumentosCTE.BuscarPorCTe(cte.Empresa.Codigo, cte.Codigo);

                if (documentosCTEs != null && documentosCTEs.Count > 0)
                {
                    foreach (Dominio.Entidades.DocumentosCTE documentos in documentosCTEs)
                    {
                        Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notaFiscal = null;

                        if (!string.IsNullOrWhiteSpace(documentos.ChaveNFE))
                        {
                            if (!repCargaPedido.ExisteNotaFiscalVinculada(cargaCTe.Carga.Codigo, Utilidades.String.OnlyNumbers(documentos.ChaveNFE)))
                            {
                                notaFiscal = repXMLNotaFiscal.BuscarPorChave(Utilidades.String.OnlyNumbers(documentos.ChaveNFE));

                                if (notaFiscal == null)
                                    notaFiscal = CriarRegistroXMLNotaFiscalCTeManual(cargaPedido, documentos, unitOfWork);
                            }
                        }
                        else
                            notaFiscal = CriarRegistroXMLNotaFiscalCTeManual(cargaPedido, documentos, unitOfWork);

                        if (notaFiscal != null)
                        {
                            Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNota = new Servicos.Embarcador.Pedido.PedidoXMLNotaFiscal(unitOfWork, ConfiguracaoEmbarcador).PreencherDadosNotaFiscal(notaFiscal, cargaPedido);

                            repPedidoXMLNotaFiscal.Inserir(pedidoXMLNota);

                            Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe pedidoXMLNotaFiscalCTe = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe()
                            {
                                CargaCTe = cargaCTe,
                                PedidoXMLNotaFiscal = pedidoXMLNota
                            };

                            repCargaPedidoXMLNotaFiscalCTe.Inserir(pedidoXMLNotaFiscalCTe);

                            if (cte.XMLNotaFiscais == null || cte.XMLNotaFiscais.Count == 0)
                                cte.XMLNotaFiscais = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();

                            if (!cte.XMLNotaFiscais.Any(o => o.Codigo == notaFiscal.Codigo))
                                cte.XMLNotaFiscais.Add(new Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal() { Codigo = notaFiscal.Codigo });

                            GerarCanhotoNotaFiscal(pedidoXMLNota, ConfiguracaoEmbarcador, tipoServicoMultisoftware, unitOfWork);
                        }
                    }

                    repCTe.Atualizar(cte);
                }

                if (cargaPedido != null && cargaPedido.Carga != null && cte != null && cte.CTeImportadoEmbarcador.HasValue && cte.CTeImportadoEmbarcador.Value)
                    svcCTe.SalvarInformacoesMultiModal(cte, cargaPedido, cte.ValorAReceber, unitOfWork);
            }

            GerarCanhotoCTe(cargaCTe, ConfiguracaoEmbarcador, tipoServicoMultisoftware, unitOfWork);

            if (ConfiguracaoEmbarcador.DeixarCargaPendenteDeIntegracaoAposCTeManual)
            {
                carga.CargaIntegradaEmbarcador = false;
                repCarga.Atualizar(carga);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, "Alterou a carga para pendente de integração devido a geração de um CT-e Manual.", unitOfWork);
            }

            serCargaDadosSumarizados.AtualizarDadosMercanteManifesto(cte.Codigo, unitOfWork);

            cte.UsuarioEmissaoCTe = usuario;
            repCTe.Atualizar(cte);

            unitOfWork.CommitChanges();

            string retorno = !cteImportado ? EmitirCTe(cte.Codigo, unitOfWork) : "";

            return true;
        }

        public static bool ProcessarXMLCTe(System.IO.Stream xml, Repositorio.UnitOfWork unitOfWork, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string nomeArquivo, bool documentoRecebidoPorFTP = false, bool documentoRecebidoPorEmail = false, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = null)
        {
            try
            {
                xml.Position = 0;
                var cteLido = MultiSoftware.CTe.Servicos.Leitura.Ler(xml);

                if (cteLido != null)
                {
                    Servicos.Embarcador.NFe.NFe serNFe = new Servicos.Embarcador.NFe.NFe(unitOfWork);
                    Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);
                    Servicos.Email serEmail = new Servicos.Email(unitOfWork);
                    Servicos.Embarcador.CTe.CTe serCte = new Servicos.Embarcador.CTe.CTe(unitOfWork);
                    Servicos.Embarcador.CTe.CTEsImportados setCTEsImportados = new Servicos.Embarcador.CTe.CTEsImportados(unitOfWork);
                    Servicos.Embarcador.Carga.PreCTe serCargaPreCTe = new Servicos.Embarcador.Carga.PreCTe(unitOfWork);
                    Servicos.Embarcador.Carga.CTeSubContratacao servicoCTeSubContratacao = new Servicos.Embarcador.Carga.CTeSubContratacao(unitOfWork);
                    Servicos.Embarcador.Documentos.GestaoDocumento servicoGestaoDocumento = new Servicos.Embarcador.Documentos.GestaoDocumento(unitOfWork);

                    Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                    Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                    Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                    Repositorio.Embarcador.CTe.CTeTerceiro repositorioCTeTerceiro = new Repositorio.Embarcador.CTe.CTeTerceiro(unitOfWork);
                    Repositorio.Embarcador.CTe.CTeTerceiroOutrosDocumentos repCTeTerceiroOutrosDocumentos = new Repositorio.Embarcador.CTe.CTeTerceiroOutrosDocumentos(unitOfWork);
                    Repositorio.Embarcador.CTe.CTeTerceiroNFe repCTeTerceiroNFe = new Repositorio.Embarcador.CTe.CTeTerceiroNFe(unitOfWork);
                    Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                    Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                    Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                    Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                    Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                    Repositorio.Embarcador.Pedidos.PedidoCTeParcial repPedidoCTeParcial = new Repositorio.Embarcador.Pedidos.PedidoCTeParcial(unitOfWork);
                    Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repositorioConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unitOfWork);

                    Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = repositorioConfiguracaoGeral.BuscarConfiguracaoPadrao();

                    Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
                    bool enviarCTeApenasParaTomador = (configuracaoGeral?.EnviarCTeApenasParaTomador ?? false);
                    string expressaoRegularBooking = repTipoOperacao.BuscarExpressaoRegularBooking();
                    string expressaoRegularContainer = repTipoOperacao.BuscarExpressaoRegularContainer();

                    if (cteLido.GetType() == typeof(MultiSoftware.CTe.v200.ConhecimentoDeTransporteProcessado.cteProc))
                    {
                        MultiSoftware.CTe.v200.ConhecimentoDeTransporteProcessado.cteProc cteProc = (MultiSoftware.CTe.v200.ConhecimentoDeTransporteProcessado.cteProc)cteLido;

                        Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte = serCte.ConverterProcCTeParaCTe(cteProc);

                        Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(cte.Emitente.CNPJ);

                        if (empresa != null)
                        {
                            object cteRetorno = svcCTe.GerarCTeAnterior(empresa, cteProc, xml, unitOfWork, expressaoRegularBooking, expressaoRegularContainer, false, configuracao.UtilizaEmissaoMultimodal, false, tipoServicoMultisoftware);

                            if (cteRetorno.GetType() == typeof(Dominio.Entidades.ConhecimentoDeTransporteEletronico))
                            {
                                Dominio.Entidades.ConhecimentoDeTransporteEletronico cteConvertido = repCTe.BuscarPorCodigo(((Dominio.Entidades.ConhecimentoDeTransporteEletronico)cteRetorno).Codigo);

                                setCTEsImportados.DestinarCTeImportadoParaSeuDestino(cteConvertido, unitOfWork, stringConexao, tipoServicoMultisoftware, nomeArquivo, auditado);
                            }
                        }
                    }
                    else if (cteLido.GetType() == typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc))
                    {

                        try
                        {
                            if (configuracao.UtilizaEmissaoMultimodal)
                            {
                                System.IO.StreamReader stReaderXML = new StreamReader(xml);

                                Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documentoDestinado = serNFe.BuscarDadosCTeDestinada(stReaderXML, unitOfWork, null);
                                if (documentoDestinado != null)
                                {
                                    string caminhoDocumentosFiscais = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoDocumentosFiscaisEmbarcador;
                                    caminhoDocumentosFiscais = Utilidades.IO.FileStorageService.Storage.Combine(caminhoDocumentosFiscais, "CTe", documentoDestinado.Empresa.CNPJ_SemFormato);

                                    caminhoDocumentosFiscais = Utilidades.IO.FileStorageService.Storage.Combine(caminhoDocumentosFiscais, documentoDestinado.Chave + ".xml");

                                    if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoDocumentosFiscais))
                                    {
                                        using (System.IO.Stream output = Utilidades.IO.FileStorageService.Storage.OpenWrite(caminhoDocumentosFiscais))
                                        {
                                            xml.CopyTo(output);
                                        }
                                    }
                                }
                                else
                                    Servicos.Log.TratarErro("Não foi possível converter o arquivo do e-mail para NF-e e CT-e", "XMLEmail");
                            }
                        }
                        catch (Exception excecao)
                        {
                            Servicos.Log.TratarErro(excecao, "XMLEmail");
                        }

                        MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc cteProc = (MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc)cteLido;

                        bool cteSemNotaFiscal = false;
                        Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte = serCte.ConverterProcCTeParaCTe(cteProc);
                        if (configuracao.AdicionarOutroDocumentoQuandoCTeAnteriorNaoTem && ((cte.OutrosDocumentos?.Count ?? 0) <= 0) && ((cte.NotasFiscais?.Count ?? 0) <= 0) && ((cte.NFEs?.Count ?? 0) <= 0))
                        {
                            Servicos.Log.TratarErro("CT-e veio sem nota fiscal vinculada", "XMLEmail");
                            cteSemNotaFiscal = true;
                        }

                        Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(cte.Emitente.CNPJ);

                        if (empresa != null)
                        {
                            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                            {
                                Dominio.ObjetosDeValor.Embarcador.Carga.PreCTe preCTe = serCargaPreCTe.BuscarCargaPreCTe(cteProc, tipoServicoMultisoftware, configuracao, null, unitOfWork);

                                if (preCTe.CargaCTe == null && preCTe.CargaCTeComplementoInfo == null)
                                {
                                    xml.Position = 0;
                                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cteConvertido = servicoGestaoDocumento.CriarCTe(cteProc, xml);

                                    if (cteConvertido != null)
                                    {
                                        Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParcial pedidoCTeParcial = repPedidoCTeParcial.BuscarPorIntegradoOutroPedido(cteConvertido.Remetente.Cliente.CPF_CNPJ, cteConvertido.Numero, 0);
                                        if (pedidoCTeParcial == null)
                                            servicoGestaoDocumento.CriarInconsitencia(cteConvertido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.SemCarga, tipoServicoMultisoftware);
                                        else
                                        {

                                            Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteTerceiro = repositorioCTeTerceiro.BuscarPorChave(cteConvertido.ChaveAcesso);
                                            if (cteTerceiro == null)
                                            {
                                                string mensagemErroCriacaoCTeTerceiro = string.Empty;
                                                Embarcador.CTe.CTe servicoCte = new Embarcador.CTe.CTe(unitOfWork);
                                                Dominio.ObjetosDeValor.Embarcador.CTe.CTe cteIntegracao = servicoCte.ConverterEntidadeCTeParaObjeto(cteConvertido, enviarCTeApenasParaTomador, unitOfWork);
                                                cteTerceiro = servicoCTeSubContratacao.CriarCTeTerceiro(unitOfWork, ref mensagemErroCriacaoCTeTerceiro, null, cteIntegracao);
                                            }

                                            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorPedido(pedidoCTeParcial.Pedido.Codigo);
                                            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                                            {
                                                if (cargaPedido.SituacaoEmissao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNF.AgEnvioNF)
                                                {
                                                    string retorno = servicoCTeSubContratacao.ValidarRegrasCTeParaSubContratacao(cteTerceiro, cargaPedido, unitOfWork, tipoServicoMultisoftware);

                                                    if (string.IsNullOrEmpty(retorno))
                                                    {
                                                        if (cargaPedido.Pedido.Destinatario != null && cteTerceiro.Destinatario.Cliente != null && cargaPedido.Pedido.Destinatario.CPF_CNPJ != cteTerceiro.Destinatario.Cliente.CPF_CNPJ)
                                                        {
                                                            cargaPedido.Pedido.Destinatario = cteTerceiro.Destinatario.Cliente;
                                                            if (cargaPedido.Recebedor == null)
                                                                cargaPedido.Destino = cargaPedido.Pedido.Destinatario.Localidade;
                                                        }

                                                        cteTerceiro.Ativo = true;

                                                        servicoCTeSubContratacao.InserirCTeSubContratacaoCargaPedido(cteTerceiro, cargaPedido, tipoServicoMultisoftware, unitOfWork);

                                                        repPedido.Atualizar(cargaPedido.Pedido);
                                                        repCarga.Atualizar(cargaPedido.Carga);
                                                        repositorioCTeTerceiro.Atualizar(cteTerceiro);

                                                        //servicoCanhoto.SalvarCanhotoCTe(cteTerceiro, cargaPedido, cargaPedido.Carga.FreteDeTerceiro ? cargaPedido.Carga.Veiculo.Proprietario : null, cargaPedido.Carga.Motoristas != null ? cargaPedido.Carga.Motoristas.ToList() : new List<Dominio.Entidades.Usuario>(), tipoServicoMultisoftware, unitOfWork);
                                                    }
                                                }
                                            }


                                        }
                                    }
                                }
                                else
                                {
                                    xml.Position = 0;

                                    if (preCTe.CargaCTe != null)
                                    {
                                        string retorno = serCargaPreCTe.EnviarXMLCTeDoPreCTe(xml, preCTe.CargaCTe.PreCTe, preCTe.CargaCTe, unitOfWork, configuracao, tipoServicoMultisoftware);

                                        if (retorno.Length == 0)
                                        {
                                            if (preCTe.CargaCTe.CTe.TipoServico == Dominio.Enumeradores.TipoServico.SubContratacao)
                                                serCargaPreCTe.SetarDocumentoOriginario(preCTe.CargaCTe.CTe, unitOfWork);

                                            serCargaPreCTe.VerificarEnviouTodosDocumentos(unitOfWork, preCTe.CargaCTe.Carga, tipoServicoMultisoftware, configuracao);
                                        }
                                        else
                                            Servicos.Log.TratarErro("Retorno CT-e " + retorno, "XMLEmail");
                                    }
                                    else
                                    {
                                        string retorno = serCargaPreCTe.EnviarXMLCTeDoPreCTe(xml, preCTe.CargaCTeComplementoInfo.PreCTe, preCTe.CargaCTeComplementoInfo, unitOfWork, configuracao, tipoServicoMultisoftware);

                                        if (retorno.Length == 0)
                                        {
                                            Servicos.Embarcador.Carga.PreCTe.VerificarEnviouTodosPreDocumentos(preCTe.CargaCTeComplementoInfo.CargaOcorrencia, unitOfWork);
                                        }
                                        else
                                            Servicos.Log.TratarErro("Retorno CT-e Complementar" + retorno, "XMLEmail");
                                    }
                                }
                            }
                            else
                            {
                                unitOfWork.Start();

                                object cteRetorno = svcCTe.GerarCTeAnterior(empresa, cteProc, xml, unitOfWork, expressaoRegularBooking, expressaoRegularContainer, false, true, false, tipoServicoMultisoftware, false, cteSemNotaFiscal);

                                if (cteRetorno.GetType() == typeof(Dominio.Entidades.ConhecimentoDeTransporteEletronico) || cteRetorno.GetType().BaseType == typeof(Dominio.Entidades.ConhecimentoDeTransporteEletronico))
                                {
                                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cteConvertido = repCTe.BuscarPorCodigo(((Dominio.Entidades.ConhecimentoDeTransporteEletronico)cteRetorno).Codigo);

                                    setCTEsImportados.DestinarCTeImportadoParaSeuDestino(cteConvertido, unitOfWork, stringConexao, tipoServicoMultisoftware, nomeArquivo, auditado);
                                }
                                else
                                {
                                    unitOfWork.Rollback();

                                    string mensagem = cteRetorno.GetType() == typeof(string) ? (string)cteRetorno : string.Empty;

                                    Servicos.Log.TratarErro("Não foi possível salvar o CT-e " + cteProc?.protCTe?.infProt?.chCTe + ". " + mensagem, "XMLEmail");
                                }
                            }
                        }
                        else if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                        {
                            try
                            {
                                Servicos.Log.TratarErro("Gerando XML de terceiro", "XMLEmail");
                                string mensagemErroCriacaoCTeTerceiro = "";
                                string descricaoItemPeso = "";
                                bool utilizarPrimeiraUnidadeMedidaPeso = false;

                                if ((cte.OutrosDocumentos?.Count ?? 0) <= 0 && (cte.NotasFiscais?.Count ?? 0) <= 0 && (cte.NFEs?.Count ?? 0) <= 0)
                                {
                                    List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroNFe> nfes = repCTeTerceiroNFe.BuscarPorChave(cte.Chave);
                                    List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroOutrosDocumentos> outrosDocumentos = repCTeTerceiroOutrosDocumentos.BuscarPorChave(cte.Chave);
                                    if (nfes != null && nfes.Count > 0)
                                    {
                                        cte.NFEs = new List<Dominio.ObjetosDeValor.Embarcador.CTe.NFe>();
                                        foreach (var nfe in nfes)
                                        {
                                            int.TryParse(nfe.Numero, out int numeroNota);
                                            Dominio.ObjetosDeValor.Embarcador.CTe.NFe nota = new Dominio.ObjetosDeValor.Embarcador.CTe.NFe()
                                            {
                                                DataEmissao = cte.DataEmissao,
                                                Chave = nfe.Chave,
                                                Numero = numeroNota
                                            };
                                            cte.NFEs.Add(nota);
                                        }
                                    }
                                    else if (outrosDocumentos != null && outrosDocumentos.Count > 0)
                                    {
                                        cte.OutrosDocumentos = new List<Dominio.ObjetosDeValor.Embarcador.CTe.OutroDocumento>();
                                        foreach (var outroDocumento in outrosDocumentos)
                                        {
                                            Dominio.ObjetosDeValor.Embarcador.CTe.OutroDocumento outroDoc = new Dominio.ObjetosDeValor.Embarcador.CTe.OutroDocumento()
                                            {
                                                DataEmissao = cte.DataEmissao,
                                                Descricao = outroDocumento.Descricao,
                                                Numero = outroDocumento.Numero,
                                                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOutroDocumento.Outros,
                                                Valor = cte.InformacaoCarga?.ValorTotalCarga ?? 0,
                                            };
                                            cte.OutrosDocumentos.Add(outroDoc);
                                        }
                                    }
                                    else if ((cte.DocumentosAnteriores?.Count ?? 0) > 0 || configuracao.UtilizaEmissaoMultimodal)
                                    {
                                        cte.OutrosDocumentos = new List<Dominio.ObjetosDeValor.Embarcador.CTe.OutroDocumento>();
                                        Dominio.ObjetosDeValor.Embarcador.CTe.OutroDocumento outroDoc = new Dominio.ObjetosDeValor.Embarcador.CTe.OutroDocumento()
                                        {
                                            DataEmissao = cte.DataEmissao,
                                            Descricao = cte.Numero.ToString(),
                                            Numero = cte.Numero.ToString(),
                                            Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOutroDocumento.Outros,
                                            Valor = cte.InformacaoCarga?.ValorTotalCarga ?? 0
                                        };
                                        cte.OutrosDocumentos.Add(outroDoc);
                                    }
                                    else
                                    {
                                        if (!configuracao.GerarOcorrenciaComplementoSubcontratacao || cte.TipoCTE != Dominio.Enumeradores.TipoCTE.Complemento)
                                        {
                                            Servicos.Log.TratarErro("CT-e de terceiro sem nenhum documento vinculado", "XMLEmail");
                                            return false;
                                        }
                                    }
                                }

                                Dominio.Entidades.Cliente emitente = null;
                                if (cte != null && cte.Emitente != null && !string.IsNullOrWhiteSpace(cte.Emitente.CNPJ))
                                {
                                    double.TryParse(cte.Emitente.CNPJ, out double cnpjEmitente);
                                    if (cnpjEmitente > 0)
                                    {
                                        emitente = repCliente.BuscarPorCPFCNPJ(cnpjEmitente);
                                        if (emitente != null)
                                        {
                                            if (emitente.NaoUsarConfiguracaoEmissaoGrupo)
                                            {
                                                descricaoItemPeso = emitente.DescricaoItemPesoCTeSubcontratacao;
                                                utilizarPrimeiraUnidadeMedidaPeso = emitente.ConfiguracaoEmissao?.UtilizarPrimeiraUnidadeMedidaPesoCTeSubcontratacao ?? false;
                                            }
                                            else if (emitente.GrupoPessoas != null)
                                            {
                                                descricaoItemPeso = emitente.GrupoPessoas.DescricaoItemPesoCTeSubcontratacao;
                                                utilizarPrimeiraUnidadeMedidaPeso = emitente.GrupoPessoas.ConfiguracaoEmissao?.UtilizarPrimeiraUnidadeMedidaPesoCTeSubcontratacao ?? false;
                                            }
                                        }
                                    }
                                }

                                unitOfWork.Start();

                                Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteTerceiro = servicoCTeSubContratacao.CriarCTeTerceiro(unitOfWork, ref mensagemErroCriacaoCTeTerceiro, null, cte, descricaoItemPeso, utilizarPrimeiraUnidadeMedidaPeso);

                                if (!string.IsNullOrWhiteSpace(mensagemErroCriacaoCTeTerceiro))
                                {
                                    unitOfWork.Rollback();
                                    Servicos.Log.TratarErro($"Não foi possível criar o CT-e terceiro: {mensagemErroCriacaoCTeTerceiro}.");
                                }
                                else if (cteTerceiro != null)
                                {
                                    if (cteTerceiro.Codigo == 0)
                                    {
                                        repositorioCTeTerceiro.Inserir(cteTerceiro);

                                        unitOfWork.CommitChanges();
                                    }

                                    unitOfWork.Start();

                                    if (!CTEsImportados.DestinarCTeTerceiroAOcorrencia(out string mensagem, cteTerceiro, configuracao, unitOfWork, tipoServicoMultisoftware))
                                    {
                                        unitOfWork.Rollback();
                                        Log.TratarErro($"Não foi possível gerar a ocorrência do CT-e terceiro {cteTerceiro.ChaveAcesso}: {mensagem}.");
                                    }
                                    else if ((emitente?.GrupoPessoas?.VincularCTeSubcontratacaoPeloNumeroPedido ?? false) && !CTEsImportados.DestinarCTeTerceiroAPedido(out mensagem, cteTerceiro, cte, configuracao, unitOfWork, tipoServicoMultisoftware))
                                    {
                                        unitOfWork.Rollback();
                                        Log.TratarErro($"Não foi possível vincular o CT-e terceiro ao pedido {cteTerceiro.ChaveAcesso}: {mensagem}.");
                                    }
                                    else
                                        unitOfWork.CommitChanges();
                                }
                            }
                            catch (Exception excecao)
                            {
                                unitOfWork.Rollback();
                                unitOfWork.Clear();

                                Servicos.Log.TratarErro(excecao, "XMLEmail");
                            }
                        }
                    }
                    else if (cteLido.GetType() == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteProc))
                    {

                        try
                        {
                            if (configuracao.UtilizaEmissaoMultimodal)
                            {
                                System.IO.StreamReader stReaderXML = new StreamReader(xml);

                                Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documentoDestinado = serNFe.BuscarDadosCTeDestinada(stReaderXML, unitOfWork, null);
                                if (documentoDestinado != null)
                                {
                                    string caminhoDocumentosFiscais = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoDocumentosFiscaisEmbarcador;
                                    caminhoDocumentosFiscais = Utilidades.IO.FileStorageService.Storage.Combine(caminhoDocumentosFiscais, "CTe", documentoDestinado.Empresa.CNPJ_SemFormato);

                                    caminhoDocumentosFiscais = Utilidades.IO.FileStorageService.Storage.Combine(caminhoDocumentosFiscais, documentoDestinado.Chave + ".xml");

                                    if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoDocumentosFiscais))
                                    {
                                        using (System.IO.Stream output = Utilidades.IO.FileStorageService.Storage.OpenWrite(caminhoDocumentosFiscais))
                                        {
                                            xml.CopyTo(output);
                                        }
                                    }
                                }
                                else
                                    Servicos.Log.TratarErro("Não foi possível converter o arquivo do e-mail para NF-e e CT-e", "XMLEmail");
                            }
                        }
                        catch (Exception excecao)
                        {
                            Servicos.Log.TratarErro(excecao, "XMLEmail");
                        }

                        MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteProc cteProc = (MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteProc)cteLido;

                        bool cteSemNotaFiscal = false;
                        Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte = serCte.ConverterProcCTeParaCTe(cteProc);
                        if (configuracao.AdicionarOutroDocumentoQuandoCTeAnteriorNaoTem && ((cte.OutrosDocumentos?.Count ?? 0) <= 0) && ((cte.NotasFiscais?.Count ?? 0) <= 0) && ((cte.NFEs?.Count ?? 0) <= 0))
                        {
                            Servicos.Log.TratarErro("CT-e veio sem nota fiscal vinculada", "XMLEmail");
                            cteSemNotaFiscal = true;
                        }

                        Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(cte.Emitente.CNPJ);

                        if (empresa != null)
                        {
                            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                            {
                                Dominio.ObjetosDeValor.Embarcador.Carga.PreCTe preCTe = serCargaPreCTe.BuscarCargaPreCTe(cteProc, tipoServicoMultisoftware, configuracao, null, unitOfWork);

                                if (preCTe.CargaCTe == null && preCTe.CargaCTeComplementoInfo == null)
                                {
                                    xml.Position = 0;

                                    if (!unitOfWork.IsActiveTransaction())
                                        unitOfWork.Start();

                                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cteConvertido = servicoGestaoDocumento.CriarCTe(cteProc, xml);

                                    if (cteConvertido == null)
                                        unitOfWork.Rollback();
                                    else
                                    {
                                        Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParcial pedidoCTeParcial = repPedidoCTeParcial.BuscarPorIntegradoOutroPedido(cteConvertido.Remetente.Cliente.CPF_CNPJ, cteConvertido.Numero, 0);
                                        if (pedidoCTeParcial == null)
                                            servicoGestaoDocumento.CriarInconsitencia(cteConvertido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoInconsistenciaGestaoDocumento.SemCarga, tipoServicoMultisoftware);
                                        else
                                        {

                                            Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteTerceiro = repositorioCTeTerceiro.BuscarPorChave(cteConvertido.ChaveAcesso);
                                            if (cteTerceiro == null)
                                            {
                                                string mensagemErroCriacaoCTeTerceiro = string.Empty;
                                                Embarcador.CTe.CTe servicoCte = new Embarcador.CTe.CTe(unitOfWork);
                                                Dominio.ObjetosDeValor.Embarcador.CTe.CTe cteIntegracao = servicoCte.ConverterEntidadeCTeParaObjeto(cteConvertido, enviarCTeApenasParaTomador, unitOfWork);
                                                cteTerceiro = servicoCTeSubContratacao.CriarCTeTerceiro(unitOfWork, ref mensagemErroCriacaoCTeTerceiro, null, cteIntegracao);
                                            }

                                            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorPedido(pedidoCTeParcial.Pedido.Codigo);
                                            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                                            {
                                                if (cargaPedido.SituacaoEmissao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNF.AgEnvioNF)
                                                {
                                                    string retorno = servicoCTeSubContratacao.ValidarRegrasCTeParaSubContratacao(cteTerceiro, cargaPedido, unitOfWork, tipoServicoMultisoftware);

                                                    if (string.IsNullOrEmpty(retorno))
                                                    {
                                                        if (cargaPedido.Pedido.Destinatario != null && cteTerceiro.Destinatario.Cliente != null && cargaPedido.Pedido.Destinatario.CPF_CNPJ != cteTerceiro.Destinatario.Cliente.CPF_CNPJ)
                                                        {
                                                            cargaPedido.Pedido.Destinatario = cteTerceiro.Destinatario.Cliente;
                                                            if (cargaPedido.Recebedor == null)
                                                                cargaPedido.Destino = cargaPedido.Pedido.Destinatario.Localidade;
                                                        }

                                                        cteTerceiro.Ativo = true;

                                                        servicoCTeSubContratacao.InserirCTeSubContratacaoCargaPedido(cteTerceiro, cargaPedido, tipoServicoMultisoftware, unitOfWork);

                                                        repPedido.Atualizar(cargaPedido.Pedido);
                                                        repCarga.Atualizar(cargaPedido.Carga);
                                                        repositorioCTeTerceiro.Atualizar(cteTerceiro);

                                                        //servicoCanhoto.SalvarCanhotoCTe(cteTerceiro, cargaPedido, cargaPedido.Carga.FreteDeTerceiro ? cargaPedido.Carga.Veiculo.Proprietario : null, cargaPedido.Carga.Motoristas != null ? cargaPedido.Carga.Motoristas.ToList() : new List<Dominio.Entidades.Usuario>(), tipoServicoMultisoftware, unitOfWork);
                                                    }
                                                }
                                            }


                                        }
                                    }
                                }
                                else
                                {
                                    xml.Position = 0;

                                    if (preCTe.CargaCTe != null)
                                    {
                                        string retorno = serCargaPreCTe.EnviarXMLCTeDoPreCTe(xml, preCTe.CargaCTe.PreCTe, preCTe.CargaCTe, unitOfWork, configuracao, tipoServicoMultisoftware);

                                        if (retorno.Length == 0)
                                        {
                                            if (preCTe.CargaCTe.CTe.TipoServico == Dominio.Enumeradores.TipoServico.SubContratacao)
                                                serCargaPreCTe.SetarDocumentoOriginario(preCTe.CargaCTe.CTe, unitOfWork);

                                            serCargaPreCTe.VerificarEnviouTodosDocumentos(unitOfWork, preCTe.CargaCTe.Carga, tipoServicoMultisoftware, configuracao);
                                        }
                                        else
                                            Servicos.Log.TratarErro("Retorno CT-e " + retorno, "XMLEmail");
                                    }
                                    else
                                    {
                                        string retorno = serCargaPreCTe.EnviarXMLCTeDoPreCTe(xml, preCTe.CargaCTeComplementoInfo.PreCTe, preCTe.CargaCTeComplementoInfo, unitOfWork, configuracao, tipoServicoMultisoftware);

                                        if (retorno.Length == 0)
                                        {
                                            Servicos.Embarcador.Carga.PreCTe.VerificarEnviouTodosPreDocumentos(preCTe.CargaCTeComplementoInfo.CargaOcorrencia, unitOfWork);
                                        }
                                        else
                                            Servicos.Log.TratarErro("Retorno CT-e Complementar" + retorno, "XMLEmail");
                                    }
                                }
                            }
                            else
                            {
                                unitOfWork.Start();

                                object cteRetorno = svcCTe.GerarCTeAnterior(empresa, cteProc, xml, unitOfWork, expressaoRegularBooking, expressaoRegularContainer, false, true, false, tipoServicoMultisoftware, false, cteSemNotaFiscal);

                                if (cteRetorno.GetType() == typeof(Dominio.Entidades.ConhecimentoDeTransporteEletronico) || cteRetorno.GetType().BaseType == typeof(Dominio.Entidades.ConhecimentoDeTransporteEletronico))
                                {
                                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cteConvertido = repCTe.BuscarPorCodigo(((Dominio.Entidades.ConhecimentoDeTransporteEletronico)cteRetorno).Codigo);

                                    setCTEsImportados.DestinarCTeImportadoParaSeuDestino(cteConvertido, unitOfWork, stringConexao, tipoServicoMultisoftware, nomeArquivo, auditado);
                                }
                                else
                                {
                                    unitOfWork.Rollback();

                                    string mensagem = cteRetorno.GetType() == typeof(string) ? (string)cteRetorno : string.Empty;

                                    Servicos.Log.TratarErro("Não foi possível salvar o CT-e " + cteProc?.protCTe?.infProt?.chCTe + ". " + mensagem, "XMLEmail");
                                }
                            }
                        }
                        else if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                        {
                            try
                            {
                                Servicos.Log.TratarErro("Gerando XML de terceiro", "XMLEmail");
                                string mensagemErroCriacaoCTeTerceiro = "";
                                string descricaoItemPeso = "";
                                bool utilizarPrimeiraUnidadeMedidaPeso = false;

                                if ((cte.OutrosDocumentos?.Count ?? 0) <= 0 && (cte.NotasFiscais?.Count ?? 0) <= 0 && (cte.NFEs?.Count ?? 0) <= 0)
                                {
                                    List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroNFe> nfes = repCTeTerceiroNFe.BuscarPorChave(cte.Chave);
                                    List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroOutrosDocumentos> outrosDocumentos = repCTeTerceiroOutrosDocumentos.BuscarPorChave(cte.Chave);
                                    if (nfes != null && nfes.Count > 0)
                                    {
                                        cte.NFEs = new List<Dominio.ObjetosDeValor.Embarcador.CTe.NFe>();
                                        foreach (var nfe in nfes)
                                        {
                                            int.TryParse(nfe.Numero, out int numeroNota);
                                            Dominio.ObjetosDeValor.Embarcador.CTe.NFe nota = new Dominio.ObjetosDeValor.Embarcador.CTe.NFe()
                                            {
                                                DataEmissao = cte.DataEmissao,
                                                Chave = nfe.Chave,
                                                Numero = numeroNota
                                            };
                                            cte.NFEs.Add(nota);
                                        }
                                    }
                                    else if (outrosDocumentos != null && outrosDocumentos.Count > 0)
                                    {
                                        cte.OutrosDocumentos = new List<Dominio.ObjetosDeValor.Embarcador.CTe.OutroDocumento>();
                                        foreach (var outroDocumento in outrosDocumentos)
                                        {
                                            Dominio.ObjetosDeValor.Embarcador.CTe.OutroDocumento outroDoc = new Dominio.ObjetosDeValor.Embarcador.CTe.OutroDocumento()
                                            {
                                                DataEmissao = cte.DataEmissao,
                                                Descricao = outroDocumento.Descricao,
                                                Numero = outroDocumento.Numero,
                                                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOutroDocumento.Outros,
                                                Valor = cte.InformacaoCarga?.ValorTotalCarga ?? 0,
                                            };
                                            cte.OutrosDocumentos.Add(outroDoc);
                                        }
                                    }
                                    else if ((cte.DocumentosAnteriores?.Count ?? 0) > 0 || configuracao.UtilizaEmissaoMultimodal)
                                    {
                                        cte.OutrosDocumentos = new List<Dominio.ObjetosDeValor.Embarcador.CTe.OutroDocumento>();
                                        Dominio.ObjetosDeValor.Embarcador.CTe.OutroDocumento outroDoc = new Dominio.ObjetosDeValor.Embarcador.CTe.OutroDocumento()
                                        {
                                            DataEmissao = cte.DataEmissao,
                                            Descricao = cte.Numero.ToString(),
                                            Numero = cte.Numero.ToString(),
                                            Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOutroDocumento.Outros,
                                            Valor = cte.InformacaoCarga?.ValorTotalCarga ?? 0
                                        };
                                        cte.OutrosDocumentos.Add(outroDoc);
                                    }
                                    else
                                    {
                                        if (!configuracao.GerarOcorrenciaComplementoSubcontratacao || cte.TipoCTE != Dominio.Enumeradores.TipoCTE.Complemento)
                                        {
                                            Servicos.Log.TratarErro("CT-e de terceiro sem nenhum documento vinculado", "XMLEmail");
                                            return false;
                                        }
                                    }
                                }

                                Dominio.Entidades.Cliente emitente = null;
                                if (cte != null && cte.Emitente != null && !string.IsNullOrWhiteSpace(cte.Emitente.CNPJ))
                                {
                                    double.TryParse(cte.Emitente.CNPJ, out double cnpjEmitente);
                                    if (cnpjEmitente > 0)
                                    {
                                        emitente = repCliente.BuscarPorCPFCNPJ(cnpjEmitente);
                                        if (emitente != null)
                                        {
                                            if (emitente.NaoUsarConfiguracaoEmissaoGrupo)
                                            {
                                                descricaoItemPeso = emitente.DescricaoItemPesoCTeSubcontratacao;
                                                utilizarPrimeiraUnidadeMedidaPeso = emitente.ConfiguracaoEmissao?.UtilizarPrimeiraUnidadeMedidaPesoCTeSubcontratacao ?? false;
                                            }
                                            else if (emitente.GrupoPessoas != null)
                                            {
                                                descricaoItemPeso = emitente.GrupoPessoas.DescricaoItemPesoCTeSubcontratacao;
                                                utilizarPrimeiraUnidadeMedidaPeso = emitente.GrupoPessoas.ConfiguracaoEmissao?.UtilizarPrimeiraUnidadeMedidaPesoCTeSubcontratacao ?? false;
                                            }
                                        }
                                    }
                                }

                                unitOfWork.Start();

                                Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteTerceiro = servicoCTeSubContratacao.CriarCTeTerceiro(unitOfWork, ref mensagemErroCriacaoCTeTerceiro, null, cte, descricaoItemPeso, utilizarPrimeiraUnidadeMedidaPeso);

                                if (!string.IsNullOrWhiteSpace(mensagemErroCriacaoCTeTerceiro))
                                {
                                    unitOfWork.Rollback();
                                    Servicos.Log.TratarErro($"Não foi possível criar o CT-e terceiro: {mensagemErroCriacaoCTeTerceiro}.");
                                }
                                else if (cteTerceiro != null)
                                {
                                    if (cteTerceiro.Codigo == 0)
                                    {
                                        repositorioCTeTerceiro.Inserir(cteTerceiro);

                                        unitOfWork.CommitChanges();
                                    }

                                    unitOfWork.Start();

                                    if (!CTEsImportados.DestinarCTeTerceiroAOcorrencia(out string mensagem, cteTerceiro, configuracao, unitOfWork, tipoServicoMultisoftware))
                                    {
                                        unitOfWork.Rollback();
                                        Log.TratarErro($"Não foi possível gerar a ocorrência do CT-e terceiro {cteTerceiro.ChaveAcesso}: {mensagem}.");
                                    }
                                    else if ((emitente?.GrupoPessoas?.VincularCTeSubcontratacaoPeloNumeroPedido ?? false) && !CTEsImportados.DestinarCTeTerceiroAPedido(out mensagem, cteTerceiro, cte, configuracao, unitOfWork, tipoServicoMultisoftware))
                                    {
                                        unitOfWork.Rollback();
                                        Log.TratarErro($"Não foi possível vincular o CT-e terceiro ao pedido {cteTerceiro.ChaveAcesso}: {mensagem}.");
                                    }
                                    else
                                        unitOfWork.CommitChanges();
                                }
                            }
                            catch (Exception excecao)
                            {
                                unitOfWork.Rollback();
                                unitOfWork.Clear();

                                Servicos.Log.TratarErro(excecao, "XMLEmail");
                            }
                        }
                    }
                    else if (cteLido.GetType() == typeof(MultiSoftware.CTe.v200.Eventos.TProcEvento))
                    {
                        MultiSoftware.CTe.v200.Eventos.TProcEvento procEvento = (MultiSoftware.CTe.v200.Eventos.TProcEvento)cteLido;
                        if (procEvento.retEventoCTe.infEvento.cStat == "135")
                        {
                            Dominio.Entidades.ConhecimentoDeTransporteEletronico conhecimento = repCTe.BuscarPorChave(procEvento.retEventoCTe.infEvento.chCTe);

                            if (conhecimento != null)
                            {
                                string retorno = svcCTe.CancelarConhencimentoImportado(conhecimento, procEvento, xml, unitOfWork, false);

                                if (string.IsNullOrWhiteSpace(retorno))
                                    setCTEsImportados.VerificarCTeImportadoPertenceAlgumaCargaParaCancelamento(conhecimento, unitOfWork, tipoServicoMultisoftware, configuracao);
                            }
                        }
                    }
                    else if (cteLido.GetType() == typeof(MultiSoftware.CTe.v300.Eventos.TProcEvento))
                    {
                        MultiSoftware.CTe.v300.Eventos.TProcEvento procEvento = (MultiSoftware.CTe.v300.Eventos.TProcEvento)cteLido;

                        if (procEvento.retEventoCTe.infEvento.cStat == "135")
                        {
                            Dominio.Entidades.ConhecimentoDeTransporteEletronico conhecimento = repCTe.BuscarPorChave(procEvento.retEventoCTe.infEvento.chCTe);

                            if (conhecimento != null)
                            {
                                string retorno = svcCTe.CancelarConhencimentoImportado(conhecimento, procEvento, xml, unitOfWork, false);

                                if (string.IsNullOrWhiteSpace(retorno))
                                    setCTEsImportados.VerificarCTeImportadoPertenceAlgumaCargaParaCancelamento(conhecimento, unitOfWork, tipoServicoMultisoftware, configuracao);
                            }
                            else
                            {
                                List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> ctesTerceiros = repositorioCTeTerceiro.BuscarTodosPorChave(procEvento.retEventoCTe.infEvento.chCTe);

                                foreach (Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteTerceiro in ctesTerceiros)
                                {
                                    cteTerceiro.SituacaoSEFAZ = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Cancelada;

                                    repositorioCTeTerceiro.Atualizar(cteTerceiro);
                                }
                            }
                        }
                    }
                    else if (cteLido.GetType() == typeof(MultiSoftware.CTe.v400.Eventos.TProcEvento))
                    {
                        MultiSoftware.CTe.v400.Eventos.TProcEvento procEvento = (MultiSoftware.CTe.v400.Eventos.TProcEvento)cteLido;

                        if (procEvento.retEventoCTe.infEvento.cStat == "135")
                        {
                            Dominio.Entidades.ConhecimentoDeTransporteEletronico conhecimento = repCTe.BuscarPorChave(procEvento.retEventoCTe.infEvento.chCTe);

                            if (conhecimento != null)
                            {
                                string retorno = svcCTe.CancelarConhencimentoImportado(conhecimento, procEvento, xml, unitOfWork, false);

                                if (string.IsNullOrWhiteSpace(retorno))
                                    setCTEsImportados.VerificarCTeImportadoPertenceAlgumaCargaParaCancelamento(conhecimento, unitOfWork, tipoServicoMultisoftware, configuracao);
                            }
                            else
                            {
                                List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> ctesTerceiros = repositorioCTeTerceiro.BuscarTodosPorChave(procEvento.retEventoCTe.infEvento.chCTe);

                                foreach (Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteTerceiro in ctesTerceiros)
                                {
                                    cteTerceiro.SituacaoSEFAZ = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Cancelada;

                                    repositorioCTeTerceiro.Atualizar(cteTerceiro);
                                }
                            }
                        }
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                unitOfWork.Rollback();
                throw;
            }
        }

        public Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao ImportarCTesLote(Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, string dados, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.CTe.DownloadLoteCTe repositorioDownloadLoteCTe = new Repositorio.Embarcador.CTe.DownloadLoteCTe(unitOfWork);
            Repositorio.Embarcador.CTe.DownloadLoteCTeChave repositorioDownloadLoteCTeChave = new Repositorio.Embarcador.CTe.DownloadLoteCTeChave(unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao();
            retornoImportacao.Retornolinhas = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha>();

            List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha>>(dados);

            int contador = 0;

            Dominio.Entidades.Embarcador.CTe.DownloadLoteCTe downloadLoteCTe = null;

            for (int i = 0; i < linhas.Count; i++)
            {
                if (downloadLoteCTe == null)
                {
                    downloadLoteCTe = new Dominio.Entidades.Embarcador.CTe.DownloadLoteCTe();
                    downloadLoteCTe.DataSolicitacao = DateTime.Now;
                    downloadLoteCTe.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDownloadLoteCTe.Pendente;
                    repositorioDownloadLoteCTe.Inserir(downloadLoteCTe);
                }

                try
                {
                    unitOfWork.FlushAndClear();
                    unitOfWork.Start();

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha = linhas[i];
                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colChave = (from obj in linha.Colunas where obj.NomeCampo == "Chave" select obj).FirstOrDefault();

                    if (colChave != null)
                    {
                        string mensagemValidacao = "";
                        string somenteNumeros = Utilidades.String.OnlyNumbers(colChave.Valor);

                        if (!string.IsNullOrEmpty(somenteNumeros))
                        {
                            Dominio.Entidades.Embarcador.CTe.DownloadLoteCTeChave downloadLoteCTeChave = new Dominio.Entidades.Embarcador.CTe.DownloadLoteCTeChave();

                            downloadLoteCTeChave.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDownloadLoteCTe.Pendente;
                            downloadLoteCTeChave.Chave = somenteNumeros;

                            downloadLoteCTeChave.DownloadLoteCTe = downloadLoteCTe;

                            repositorioDownloadLoteCTeChave.Inserir(downloadLoteCTeChave);

                            contador++;
                        }
                        else
                        {
                            mensagemValidacao = "Registro ignorado na importação";
                        }

                    }

                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    Servicos.Log.TratarErro(ex);
                    retornoImportacao.Retornolinhas.Add(Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha.CriarRetornoFalha("Ocorreu uma falha ao processar a linha.", i));
                    continue;
                }
            }
            retornoImportacao.Importados = contador;
            retornoImportacao.Total = linhas.Count;

            Servicos.Auditoria.Auditoria.Auditar(auditado, downloadLoteCTe, "Inseriu Download Lote CTe N° " + downloadLoteCTe.Codigo + " Via importação", unitOfWork);

            unitOfWork.CommitChanges();

            return retornoImportacao;
        }

        public void SalvarDadosCTe(ref Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.ObjetosDeValor.Embarcador.CTe.CTe cteIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz situacaoCTeSefaz, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe> permissoesAlteracao, Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork, bool svmTerceiro, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.EmpresaSerie repEmpresaSerie = new Repositorio.EmpresaSerie(unitOfWork);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
            Repositorio.CFOP repCFOP = new Repositorio.CFOP(unitOfWork);
            Repositorio.Aliquota repAliquota = new Repositorio.Aliquota(unitOfWork);
            Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.ModalTransporte repModalTransporte = new Repositorio.ModalTransporte(unitOfWork);
            Repositorio.Embarcador.CTe.CTeRelacaoDocumento repCTeRelacaoDocumento = new Repositorio.Embarcador.CTe.CTeRelacaoDocumento(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoViagemNavioSchedule repPedidoNavioSchedule = new Repositorio.Embarcador.Pedidos.PedidoViagemNavioSchedule(unitOfWork);

            Servicos.CTe serCTE = new Servicos.CTe(unitOfWork);
            Servicos.Embarcador.CTe.CTeComplementar serCTeComplementar = new Servicos.Embarcador.CTe.CTeComplementar();
            Servicos.Embarcador.CTe.DocumentoCTe serDocumentoCTe = new DocumentoCTe(unitOfWork);
            Servicos.Embarcador.CTe.Participante serParticipante = new Participante(unitOfWork);
            Servicos.Embarcador.CTe.ModalRodoviario serModalRodoviario = new ModalRodoviario(unitOfWork);
            Servicos.Embarcador.CTe.Quantidades serQuantidades = new Quantidades(unitOfWork);
            Servicos.Embarcador.CTe.Seguro serSeguros = new Seguro(unitOfWork);
            Servicos.Embarcador.CTe.ProdutoPerigoso serProdutoPerigoso = new ProdutoPerigoso(unitOfWork);
            Servicos.WebService.Empresa.Empresa serEmpresa = new WebService.Empresa.Empresa(unitOfWork);
            Servicos.Embarcador.Localidades.Localidade serLocalidade = new Localidades.Localidade(unitOfWork);
            Servicos.Embarcador.CTe.DocumentoAnterior serDocumentoAnterior = new DocumentoAnterior(unitOfWork);
            Servicos.Embarcador.CTe.DocumentoTransportaAnteriorPapel serDocumentoTransportaAnteriorPapel = new DocumentoTransportaAnteriorPapel(unitOfWork);
            Servicos.Embarcador.CTe.Duplicatas serDuplicatas = new Duplicatas(unitOfWork);
            Servicos.Embarcador.CTe.Observacoes serObservacoes = new Observacoes(unitOfWork);
            Servicos.Embarcador.CTe.ComponenteFrete serComponenteFrete = new ComponenteFrete(unitOfWork);
            Servicos.Embarcador.CTe.TotalServico serTotalServicos = new TotalServico(unitOfWork);
            Servicos.Embarcador.CTe.InformacaoCarga serInformacaoCarga = new InformacaoCarga(unitOfWork);
            ModalAquaviario serModalAquaviario = new ModalAquaviario(unitOfWork);
            ModalMultimodal serModalMultimodal = new ModalMultimodal(unitOfWork);
            ModalDutoviario serModalDutoviario = new ModalDutoviario(unitOfWork);
            ModalAereo serModalAereo = new ModalAereo(unitOfWork);
            ModalFerroviario serModalFerroviario = new ModalFerroviario(unitOfWork);
            InformacaoModal serInformacaoModal = new InformacaoModal(unitOfWork);
            CTeSubstituicao serCTeSubstituicao = new CTeSubstituicao(unitOfWork);
            CTeAnulacao serCTeAnulacao = new CTeAnulacao(unitOfWork);
            Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(cteIntegracao.Emitente.CNPJ);

            bool permissaoTotal = false;
            if (permissoesAlteracao.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe.total))
                permissaoTotal = true;

            if (cte != null && cte.Codigo > 0)
                cte.Initialize();

            if (permissoesAlteracao.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe.CTe) || permissaoTotal)
            {
                cte.Empresa = empresa;
                cte.TipoTomador = cteIntegracao.TipoTomador;
                cte.TipoPagamento = cteIntegracao.TipoPagamento;
                cte.SVMTerceiro = svmTerceiro;
                cte.NumeroOS = cteIntegracao.NumeroOS;
                cte.Retira = cteIntegracao.Retira ? Dominio.Enumeradores.OpcaoSimNao.Sim : Dominio.Enumeradores.OpcaoSimNao.Nao;
                cte.DetalhesRetira = cteIntegracao.DetalhesRetira;
                cte.RNTRC = cte.Empresa.RegistroANTT;
                cte.TipoAmbiente = empresa.TipoAmbiente;
                cte.TipoCTE = cteIntegracao.TipoCTE;
                cte.TipoImpressao = cteIntegracao.TipoImpressao;
                cte.TipoServico = cteIntegracao.TipoServico;
                cte.TipoModal = cteIntegracao.TipoModal;
                cte.DataEmissao = cteIntegracao.DataEmissao == DateTime.MinValue ? DateTime.Now : cteIntegracao.DataEmissao;
                cte.ModeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorModelo("57");
                cte.LocalidadeEmissao = cte.Empresa.Localidade;
                cte.Versao = cte.Empresa.Configuracao != null && !string.IsNullOrWhiteSpace(cte.Empresa.Configuracao.VersaoCTe) ? cte.Empresa.Configuracao.VersaoCTe : cte.Empresa.EmpresaPai.Configuracao != null && !string.IsNullOrWhiteSpace(cte.Empresa.EmpresaPai.Configuracao.VersaoCTe) ? cte.Empresa.EmpresaPai.Configuracao.VersaoCTe : "4.00";
                //cte.ObservacoesGerais = cteIntegracao.ObservacoesGeral != null ? cteIntegracao.ObservacoesGeral.Texto : "";
                cte.CFOP = repCFOP.BuscarPorNumero(cteIntegracao.CFOP);
                cte.NaturezaDaOperacao = cte.CFOP.NaturezaDaOperacao;
                cte.Status = BuscarStatusCTe(situacaoCTeSefaz);

                cte.LocalidadeInicioPrestacao = repLocalidade.BuscarPorCodigo(cteIntegracao.LocalidadeInicioPrestacao.Codigo);
                cte.LocalidadeTerminoPrestacao = repLocalidade.BuscarPorCodigo(cteIntegracao.LocalidadeFimPrestacao.Codigo);

                cte.IndicadorGlobalizado = cteIntegracao.IndicadorGlobalizado;
                cte.IndicadorIETomador = cteIntegracao.IndicadorIETomador;

                if (cte.Codigo <= 0)
                    alterarSerieCTe(ref cte, cteIntegracao, empresa, unitOfWork);
                if (cteIntegracao.TipoCTE == Dominio.Enumeradores.TipoCTE.Complemento)
                    cte.ChaveCTESubComp = cteIntegracao.ChaveCTeComplementado;

                cte.TipoControle = 1;
                cte.Cancelado = "N";

                #region Não Implementado

                //cte.CIOT = cteIntegracao.CIOT; todo: ver CIOT
                //DateTime dataPrevistaEntrega = DateTime.MinValue;
                //if (cteIntegracao.DataPrevistaEntrega != null)
                //    DateTime.TryParseExact(cteIntegracao.DataPrevistaEntrega, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dataPrevistaEntrega);

                //cte.DataPrevistaEntrega = dataPrevistaEntrega == DateTime.MinValue ? DateTime.Now.AddDays(cte.Empresa.Configuracao.DiasParaEntrega) : dataPrevistaEntrega;
                //cte.TipoEmissao = tipoEmissao;
                //cte.TipoEnvio = tipoEnvio;
                //cte.ObservacaoDaCarga = cteIntegracao.ObservacaoDaCarga;

                //if (cteIntegracao.CFOP > 0)
                //{

                //}
                //else
                //{
                //    this.SetarCFOPENaturezaPorTabelaDeAliquotas(ref cte, cteIntegracao, unidadeDeTrabalho);
                //}

                //cte.ClienteRetira = this.ObterCliente(empresa, cteIntegracao.ClienteRetira, unidadeDeTrabalho);
                //cte.ClienteEntrega = this.ObterCliente(empresa, cteIntegracao.ClienteEntrega, unidadeDeTrabalho);
                #endregion

            }
            else
            {
                if (permissoesAlteracao.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe.AlterarSerie))
                    alterarSerieCTe(ref cte, cteIntegracao, empresa, unitOfWork);

                if (permissoesAlteracao.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe.AlterarCFOP))
                {
                    cte.CFOP = repCFOP.BuscarPorNumero(cteIntegracao.CFOP);
                    if (cteIntegracao.IndicadorIETomador != null)
                        cte.IndicadorIETomador = cteIntegracao.IndicadorIETomador;
                }


                if (permissoesAlteracao.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe.AlterarTipoPagamento))
                    cte.TipoPagamento = cteIntegracao.TipoPagamento;
            }

            if (permissoesAlteracao.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe.TotalServico) || permissaoTotal)
                serTotalServicos.SalvarTotaisCTe(ref cte, cteIntegracao.ValorFrete, unitOfWork);

            if (permissoesAlteracao.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe.InformacaoCarga) || permissaoTotal)
                serInformacaoCarga.SalvarInformacaoCargaCTe(ref cte, cteIntegracao.InformacaoCarga, unitOfWork);
            else
            {
                if (permissoesAlteracao.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe.AlterarProdutoPredominante))
                    cte.ProdutoPredominante = cteIntegracao.InformacaoCarga.ProdutoPredominante;

                if (permissoesAlteracao.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe.AlterarCaracteristicaAdicionalTransporte))
                    cte.CaracteristicaTransporte = cteIntegracao.InformacaoCarga.CaracteristicaTransporte;

                if (permissoesAlteracao.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe.AlterarValorTotalCarga) || permissoesAlteracao.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe.Documento))
                    cte.ValorTotalMercadoria = cteIntegracao.InformacaoCarga.ValorTotalCarga;

            }


            cte.Log = "CT-e alterado manualmente por " + usuario.Nome + " em " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            bool buscarNumero = false;
            cte.ModalTransporte = repModalTransporte.BuscarPorNumero(cte.TipoModal.ToString("D"));

            if (cte.Codigo <= 0)
            {
                buscarNumero = true;
                cte.NumeroControle = "";
                repCTe.Inserir(cte, Auditado);
                cte.Initialize();
            }
            else
                repCTe.Atualizar(cte);

            if (buscarNumero)
            {
                Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);
                cte.Numero = svcCTe.ObterProximoNumero(cte, repCTe);
            }

            string mensagem = "";

            if (permissoesAlteracao.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe.Remetente) || permissoesAlteracao.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe.AlterarIEParticipanete) || permissaoTotal)
                serParticipante.SalvarParticipante(ref cte, cteIntegracao.Remetente, null, Dominio.Enumeradores.TipoTomador.Remetente, permissaoTotal, permissoesAlteracao, ref mensagem, unitOfWork);

            if (permissoesAlteracao.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe.Expedidor) || permissoesAlteracao.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe.AlterarIEParticipanete) || permissaoTotal)
                serParticipante.SalvarParticipante(ref cte, cteIntegracao.Expedidor, null, Dominio.Enumeradores.TipoTomador.Expedidor, permissaoTotal, permissoesAlteracao, ref mensagem, unitOfWork);

            if (permissoesAlteracao.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe.Recebedor) || permissoesAlteracao.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe.AlterarIEParticipanete) || permissaoTotal)
                serParticipante.SalvarParticipante(ref cte, cteIntegracao.Recebedor, null, Dominio.Enumeradores.TipoTomador.Recebedor, permissaoTotal, permissoesAlteracao, ref mensagem, unitOfWork);

            if (permissoesAlteracao.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe.Destinatario) || permissoesAlteracao.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe.AlterarIEParticipanete) || permissaoTotal)
                serParticipante.SalvarParticipante(ref cte, cteIntegracao.Destinatario, null, Dominio.Enumeradores.TipoTomador.Destinatario, permissaoTotal, permissoesAlteracao, ref mensagem, unitOfWork);

            if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros)
            {
                if (permissoesAlteracao.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe.Tomador) || permissoesAlteracao.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe.AlterarIEParticipanete) || permissaoTotal)
                    serParticipante.SalvarParticipante(ref cte, cteIntegracao.Tomador, null, Dominio.Enumeradores.TipoTomador.Outros, permissaoTotal, permissoesAlteracao, ref mensagem, unitOfWork);
            }
            else
            {
                if (permissoesAlteracao.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe.Tomador) || permissaoTotal)
                {
                    Dominio.Entidades.ParticipanteCTe part = cte.ObterParticipante(Dominio.Enumeradores.TipoTomador.Outros);

                    if (part != null)
                    {
                        Repositorio.ParticipanteCTe repParticipante = new Repositorio.ParticipanteCTe(unitOfWork);
                        cte.SetarParticipante(null, Dominio.Enumeradores.TipoTomador.Outros);
                        //repParticipante.Deletar(part);
                    }
                }
            }

            if (permissoesAlteracao.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe.Rodoviario) || permissaoTotal)
                serModalRodoviario.SalvarModalRodoviario(ref cte, cteIntegracao.ModalRodoviario, unitOfWork);

            if (permissoesAlteracao.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe.TotalServico) || permissaoTotal)
                serCTE.SetarPartilhaICMS(ref cte, unitOfWork);

            if (permissoesAlteracao.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe.Componente) || permissaoTotal)
                serComponenteFrete.SalvarComponentesPrestacao(ref cte, cteIntegracao.ValorFrete.ComponentesAdicionais);

            if (permissoesAlteracao.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe.QuantidadeCarga) || permissaoTotal)
                serQuantidades.SalvarQuantidadesCTe(ref cte, cteIntegracao.QuantidadesCarga, unitOfWork);

            if (permissoesAlteracao.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe.Seguro) || permissaoTotal)
                serSeguros.SalvarSeguros(ref cte, cteIntegracao.Seguros, unitOfWork);

            if (permissoesAlteracao.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe.Documento) || permissaoTotal)
            {
                if (cte.TipoCTE == Dominio.Enumeradores.TipoCTE.Simplificado)
                    serDocumentoCTe.SalvarInformacoesEntregasSimplificado(ref cte, cteIntegracao, unitOfWork);
                else
                    serDocumentoCTe.SalvarInformacoesDocumentos(ref cte, cteIntegracao, unitOfWork);
            }

            if (permissoesAlteracao.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe.DocumentoTransporteAnteriorEletronico) || permissaoTotal)
                serDocumentoAnterior.SalvarInformacoesDocumentosAnteriores(ref cte, cteIntegracao.DocumentosAnteriores, unitOfWork);

            if (permissoesAlteracao.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe.DocumentoTransporteAnteriorPapel) || permissaoTotal)
                serDocumentoTransportaAnteriorPapel.SalvarInformacoesDocumentosAnterioresPapel(ref cte, cteIntegracao.DocumentosAnterioresDePapel, unitOfWork);

            if (permissoesAlteracao.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe.ObservacaoContribuinte) || permissaoTotal)
                serObservacoes.SalvarObservacoesContribuinte(ref cte, cteIntegracao.ObservacoesContribuinte, unitOfWork);

            if (permissoesAlteracao.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe.ObservacaoFisco) || permissaoTotal)
                serObservacoes.SalvarObservacoesFisco(ref cte, cteIntegracao.ObservacoesFisco, unitOfWork);

            if (permissoesAlteracao.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe.ProdutoPerigoso) || permissaoTotal)
                serProdutoPerigoso.SalvarProdutosPerigosos(ref cte, cteIntegracao.ProdutosPerigosos, unitOfWork);

            if (permissoesAlteracao.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe.ObservacaoGeral) || permissaoTotal)
                cte.ObservacoesGerais = cteIntegracao.ObservacoesGeral != null ? cteIntegracao.ObservacoesGeral.Texto : "";

            serModalAquaviario.SalvarModalAquaviario(ref cte, cteIntegracao.ModalAquaviario, unitOfWork);
            serModalMultimodal.SalvarModalMultimodal(ref cte, cteIntegracao.ModalMultimodal, unitOfWork);
            serModalDutoviario.SalvarModalDutoviario(ref cte, cteIntegracao.ModalDutoviario, unitOfWork);
            serModalAereo.SalvarModalAereo(ref cte, cteIntegracao.ModalAereo, unitOfWork);
            serModalFerroviario.SalvarModalFerroviario(ref cte, cteIntegracao.ModalFerroviario, unitOfWork);
            serInformacaoModal.SalvarInformacaoModal(ref cte, cteIntegracao.InformacaoModal, unitOfWork);
            serModalAquaviario.SalvarContainersCTe(ref cte, cteIntegracao.Containers, unitOfWork);
            serCTeSubstituicao.SalvarCTeSubstituicao(ref cte, cteIntegracao.CTeSubstituicao, unitOfWork);
            serCTeAnulacao.SalvarCTeAnulacao(ref cte, cteIntegracao.CTeAnulacao);
            serCTeComplementar.SalvarCTeComplementar(cte, cteIntegracao.CTeComplementar);

            if (cte.TipoCTE == Dominio.Enumeradores.TipoCTE.Complemento)
            {
                Dominio.Entidades.Embarcador.CTe.CTeRelacaoDocumento cTeRelacao = new Dominio.Entidades.Embarcador.CTe.CTeRelacaoDocumento()
                {
                    CTeGerado = cte,
                    CTeOriginal = repCTe.BuscarPorChave(cte.ChaveCTESubComp),
                    TipoCTeGerado = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCTeGerado.Complemento
                };
                if (cTeRelacao.CTeOriginal != null)
                    repCTeRelacaoDocumento.Inserir(cTeRelacao);
            }

            if (buscarNumero)
            {
                if (repConfiguracaoTMS.UtilizaEmissaoMultimodal())
                {
                    cte.NumeroControle = serCTE.RetornarNumeroControleCTe(out int numeroSequencia, cte.NumeroBooking, cte.DescricaoCarrier, cte.TipoPropostaFeeder, cte.TipoModal, cte.TipoServico, 0, cte.SVMTerceiro, unitOfWork, false, 0, false, cte?.SequenciaBooking ?? 0);
                    cte.PedidoViagemNavioSchedule = repPedidoNavioSchedule.BuscarPorCodigo(repCTe.BuscarCodigoSchedule(cte.ViagemPassagemCinco?.Codigo ?? 0, cte.ViagemPassagemQuatro?.Codigo ?? 0, cte.ViagemPassagemTres?.Codigo ?? 0, cte.ViagemPassagemDois?.Codigo ?? 0, cte.ViagemPassagemUm?.Codigo ?? 0, cte.Viagem?.Codigo ?? 0, cte.PortoDestino?.Codigo ?? 0, cte.TerminalDestino?.Codigo ?? 0));
                    cte.SequenciaBooking = numeroSequencia;
                    if (cte.NumeroControle == "0" || string.IsNullOrWhiteSpace(cte.NumeroControle))
                        cte.NumeroControle = cte.Codigo.ToString("D");
                    else
                    {
                        if (repCTe.ContemNumeroControleDuplicado(cte.NumeroControle, cte.Empresa?.Codigo ?? 0, cte.Codigo))
                        {
                            cte.NumeroControle = serCTE.RetornarNumeroControleCTe(out numeroSequencia, cte.NumeroBooking, cte.DescricaoCarrier, cte.TipoPropostaFeeder, cte.TipoModal, cte.TipoServico, cte.Codigo, cte.SVMTerceiro, unitOfWork, true, cte.SequenciaBooking, false, cte?.SequenciaBooking ?? 0);
                            cte.SequenciaBooking = numeroSequencia;

                            if (repCTe.ContemNumeroControleDuplicado(cte.NumeroControle, cte.Empresa?.Codigo ?? 0, cte.Codigo))
                            {
                                bool contemNumeroDuplicado = true;
                                int count = 0;
                                while (contemNumeroDuplicado)
                                {
                                    cte.NumeroControle = serCTE.RetornarNumeroControleCTe(out numeroSequencia, cte.NumeroBooking, cte.DescricaoCarrier, cte.TipoPropostaFeeder, cte.TipoModal, cte.TipoServico, cte.Codigo, cte.SVMTerceiro, unitOfWork, true, cte.SequenciaBooking, false, cte?.SequenciaBooking ?? 0);
                                    cte.SequenciaBooking = numeroSequencia;
                                    contemNumeroDuplicado = repCTe.ContemNumeroControleDuplicado(cte.NumeroControle, cte.Empresa?.Codigo ?? 0, cte.Codigo);
                                    count++;
                                    if (count > 100)
                                        break;
                                }
                            }
                            repCTe.Atualizar(cte);
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(cte.NumeroControle))
                {
                    cte.ObservacoesGerais += " Num Ctrl: " + cte.NumeroControle;
                    cte.ObservacoesGerais = cte.ObservacoesGerais.Trim();
                }
                if (cte.TipoModal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Aquaviario && cte.PortoOrigem != null && cte.PortoOrigem != null && cte.TomadorPagador != null && cte.TomadorPagador.GrupoPessoas != null)
                {
                    if (cte.PortoOrigem.AtivarDespachanteComoConsignatario && cte.PortoDestino.AtivarDespachanteComoConsignatario && cte.TomadorPagador != null && cte.TomadorPagador.GrupoPessoas != null && cte.TomadorPagador.GrupoPessoas.AdicionarDespachanteComoConsignatario && cte.TomadorPagador.GrupoPessoas.Despachante != null)
                    {
                        cte.ObservacoesGerais += " A " + cte.TomadorPagador.GrupoPessoas.Despachante.Nome + ", CNPJ " + cte.TomadorPagador.GrupoPessoas.Despachante.CPF_CNPJ_Formatado + ", irá atuar como despachante para a liberação dessa carga no porto de descarga.";
                        cte.ObservacoesGerais = cte.ObservacoesGerais.Trim();
                    }
                }
            }

            Servicos.CTe.SetarTomadorPagadorCTe(ref cte);

            repCTe.Atualizar(cte, Auditado);
        }

        public Dominio.ObjetosDeValor.Embarcador.CTe.CTe ConverterEntidadeCTeParaObjeto(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, bool enviarCTeApenasParaTomador, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.WebService.CTe.CTe servicoCte = new Servicos.WebService.CTe.CTe(unitOfWork);
            Servicos.Embarcador.CTe.DocumentoCTe servicoDocumentoCTe = new DocumentoCTe(unitOfWork);
            Servicos.Embarcador.CTe.Participante servicoParticipante = new Participante(unitOfWork);
            Servicos.Embarcador.CTe.ModalRodoviario servicoModalRodoviario = new ModalRodoviario(unitOfWork);
            Servicos.Embarcador.CTe.Quantidades servicoQuantidades = new Quantidades(unitOfWork);
            Servicos.Embarcador.CTe.Seguro servicoSeguros = new Seguro(unitOfWork);
            Servicos.Embarcador.CTe.ProdutoPerigoso servicoProdutoPerigoso = new ProdutoPerigoso(unitOfWork);
            Servicos.WebService.Empresa.Empresa servicoEmpresa = new WebService.Empresa.Empresa(unitOfWork);
            Servicos.Embarcador.Localidades.Localidade servicoLocalidade = new Localidades.Localidade(unitOfWork);
            Servicos.Embarcador.CTe.DocumentoAnterior servicoDocumentoAnterior = new DocumentoAnterior(unitOfWork);
            Servicos.Embarcador.CTe.DocumentoTransportaAnteriorPapel servicoDocumentoTransportaAnteriorPapel = new DocumentoTransportaAnteriorPapel(unitOfWork);
            Servicos.Embarcador.CTe.Observacoes servicoObservacoes = new Observacoes(unitOfWork);
            Servicos.Embarcador.CTe.TotalServico servicoTotalServicos = new TotalServico(unitOfWork);
            Servicos.Embarcador.CTe.InformacaoCarga servicoInformacaoCarga = new InformacaoCarga(unitOfWork);
            InformacaoModal servicoInformacaoModal = new InformacaoModal(unitOfWork);
            ModalAquaviario servicoModalAquaviario = new ModalAquaviario(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.CTe.CTe cteIntegracao = new Dominio.ObjetosDeValor.Embarcador.CTe.CTe();
            cteIntegracao.Codigo = cte.Codigo;
            cteIntegracao.Emitente = servicoEmpresa.ConverterObjetoEmpresa(cte.Empresa);
            cteIntegracao.Numero = cte.Numero;

            cteIntegracao.Serie = cte.Serie.Numero.ToString();
            cteIntegracao.LocalidadeInicioPrestacao = servicoLocalidade.ConverterObjetoLocalidade(cte.LocalidadeInicioPrestacao);
            cteIntegracao.LocalidadeFimPrestacao = servicoLocalidade.ConverterObjetoLocalidade(cte.LocalidadeTerminoPrestacao);
            cteIntegracao.TipoCTE = cte.TipoCTE;
            cteIntegracao.TipoPagamento = cte.TipoPagamento;
            cteIntegracao.TipoServico = cte.TipoServico;
            cteIntegracao.TipoImpressao = cte.TipoImpressao;
            cteIntegracao.TipoTomador = cte.TipoTomador;
            cteIntegracao.CFOP = cte.CFOP.CodigoCFOP;
            cteIntegracao.ProtocoloCancelamentoInutilizacao = cte.ProtocoloCancelamentoInutilizacao;
            cteIntegracao.DataCancelamento = cte.DataCancelamento;
            cteIntegracao.MensagemRetornoSefaz = cte.MensagemRetornoSefaz;

            cteIntegracao.DataEmissao = cte.DataEmissao.HasValue ? cte.DataEmissao.Value : DateTime.Now;

            cteIntegracao.Retira = cte.Retira == Dominio.Enumeradores.OpcaoSimNao.Sim ? true : false;
            cteIntegracao.DetalhesRetira = cte.DetalhesRetira;
            cteIntegracao.SituacaoCTeSefaz = BuscarSituacaoSefaz(cte.Status);
            cteIntegracao.Chave = cte.Chave;
            cteIntegracao.Protocolo = cte.Protocolo;

            servicoDocumentoCTe.SetarEntidadeCTeParaDocumentos(cte, ref cteIntegracao, unitOfWork);
            cteIntegracao.Remetente = servicoParticipante.ConverterParticipanteParaParticipante(cte.Remetente, enviarCTeApenasParaTomador, cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente, unitOfWork);
            cteIntegracao.Destinatario = servicoParticipante.ConverterParticipanteParaParticipante(cte.Destinatario, enviarCTeApenasParaTomador, cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario, unitOfWork);
            cteIntegracao.Expedidor = servicoParticipante.ConverterParticipanteParaParticipante(cte.Expedidor, enviarCTeApenasParaTomador, cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor, unitOfWork);
            cteIntegracao.Recebedor = servicoParticipante.ConverterParticipanteParaParticipante(cte.Recebedor, enviarCTeApenasParaTomador, cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor, unitOfWork);
            cteIntegracao.Tomador = servicoParticipante.ConverterParticipanteParaParticipante(cte.Tomador, enviarCTeApenasParaTomador, cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros, unitOfWork);
            cteIntegracao.ModalRodoviario = servicoModalRodoviario.ConverterModalTransporteCTeParaModalRodoviario(cte, unitOfWork);
            cteIntegracao.InformacaoCarga = servicoInformacaoCarga.ConverterInfomracaoCTeParaInformacaoCarga(cte, unitOfWork);
            cteIntegracao.InformacaoModal = servicoInformacaoModal.ConverterInformacaoCTeParaInformacaoModal(cte);

            cteIntegracao.QuantidadesCarga = servicoQuantidades.ConverterQuantidadesCTeParaQuantidades(cte.QuantidadesCarga.ToList(), unitOfWork);
            cteIntegracao.Seguros = servicoSeguros.ConverterSegurosCTeParaSeguro(cte.Seguros.ToList(), unitOfWork);
            cteIntegracao.ProdutosPerigosos = servicoProdutoPerigoso.ConverterProdutoPerigosoCTeParaProdutoPerigoso(cte, unitOfWork);
            cteIntegracao.DocumentosAnteriores = servicoDocumentoAnterior.ConverterDocumentosAnterioresCTeParaDocumentosAnteriores(cte.DocumentosTransporteAnterior.ToList(), unitOfWork);
            cteIntegracao.DocumentosAnterioresDePapel = servicoDocumentoTransportaAnteriorPapel.ConverterDocumentoTransporteParaTransporteAnteriorPapel(cte.DocumentosTransporteAnterior.ToList(), unitOfWork);

            cteIntegracao.Xml = servicoCte.ObterRetornoXMLPorStatus(cte, "A", unitOfWork);

            cteIntegracao.ObservacoesContribuinte = servicoObservacoes.ConverterObservacaoContribuintesCTeParaObservacoes(cte.ObservacoesContribuinte.ToList());
            cteIntegracao.ObservacoesFisco = servicoObservacoes.ConverterObservacaoFiscoCTeParaObservacoes(cte.ObservacoesFisco.ToList());

            if (!string.IsNullOrWhiteSpace(cte.ObservacoesGerais))
            {
                cteIntegracao.ObservacoesGeral = new Dominio.ObjetosDeValor.Embarcador.CTe.Observacao();
                cteIntegracao.ObservacoesGeral.Campo = "Observação Geral";
                cteIntegracao.ObservacoesGeral.Texto = cte.ObservacoesGerais;
            }

            cteIntegracao.ValorFrete = servicoTotalServicos.ConverterCTeParaFreteValor(cte, unitOfWork);
            cteIntegracao.Containers = servicoModalAquaviario.ConverterContainersCTeParaConteiners(cte.Containers.ToList());

            return cteIntegracao;
        }

        public void SetarDocumentosCTes(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cteIntegracao, Dominio.ObjetosDeValor.EDI.Notfis.NotaFiscal notaFiscal, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.CTe.DocumentoCTe serDocumentoCTe = new DocumentoCTe(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorChave(cteIntegracao.Chave);
            if (cte != null)
            {
                serDocumentoCTe.SetarEntidadeCTeParaDocumentos(cte, ref cteIntegracao, unitOfWork);
            }
            else
            {
                cteIntegracao.OutrosDocumentos = new List<Dominio.ObjetosDeValor.Embarcador.CTe.OutroDocumento>();
                Dominio.ObjetosDeValor.Embarcador.CTe.OutroDocumento outroDocumento = new Dominio.ObjetosDeValor.Embarcador.CTe.OutroDocumento();
                outroDocumento.DataEmissao = DateTime.Now;
                outroDocumento.Descricao = "Outro " + cteIntegracao.Numero;
                outroDocumento.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOutroDocumento.Outros;
                outroDocumento.Numero = cteIntegracao.Numero.ToString();
                outroDocumento.Valor = notaFiscal.NFe.Valor;
                cteIntegracao.OutrosDocumentos.Add(outroDocumento);
            }
        }

        public Dominio.ObjetosDeValor.Embarcador.CTe.CTe ConverterDynamicParaCTe(dynamic dynCTe, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.EmpresaSerie repEmpresaSerie = new Repositorio.EmpresaSerie(unitOfWork);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
            Repositorio.CFOP repCFOP = new Repositorio.CFOP(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            Servicos.Embarcador.CTe.CTeComplementar svcCTeComplementar = new Servicos.Embarcador.CTe.CTeComplementar();
            Servicos.Embarcador.CTe.DocumentoCTe serDocumentoCTe = new DocumentoCTe(unitOfWork);
            Servicos.Embarcador.CTe.Participante serParticipante = new Participante(unitOfWork);
            Servicos.Embarcador.CTe.ModalRodoviario serModalRodoviario = new ModalRodoviario(unitOfWork);
            Servicos.Embarcador.CTe.Quantidades serQuantidades = new Quantidades(unitOfWork);
            Servicos.Embarcador.CTe.Seguro serSeguros = new Seguro(unitOfWork);
            Servicos.Embarcador.CTe.ProdutoPerigoso serProdutoPerigoso = new ProdutoPerigoso(unitOfWork);
            Servicos.WebService.Empresa.Empresa serEmpresa = new WebService.Empresa.Empresa(unitOfWork);
            Servicos.Embarcador.Localidades.Localidade serLocalidade = new Localidades.Localidade(unitOfWork);
            Servicos.Embarcador.CTe.DocumentoAnterior serDocumentoAnterior = new DocumentoAnterior(unitOfWork);
            Servicos.Embarcador.CTe.DocumentoTransportaAnteriorPapel serDocumentoTransportaAnteriorPapel = new DocumentoTransportaAnteriorPapel(unitOfWork);
            Servicos.Embarcador.CTe.Duplicatas serDuplicatas = new Duplicatas(unitOfWork);
            Servicos.Embarcador.CTe.Observacoes serObservacoes = new Observacoes(unitOfWork);
            Servicos.Embarcador.CTe.ComponenteFrete serComponenteFrete = new ComponenteFrete(unitOfWork);
            Servicos.Embarcador.CTe.TotalServico serTotalServicos = new TotalServico(unitOfWork);
            Servicos.Embarcador.CTe.InformacaoCarga serInformacaoCarga = new InformacaoCarga(unitOfWork);
            ModalAquaviario serModalAquaviario = new ModalAquaviario(unitOfWork);
            ModalMultimodal serModalMultimodal = new ModalMultimodal(unitOfWork);
            ModalDutoviario serModalDutoviario = new ModalDutoviario(unitOfWork);
            ModalAereo serModalAereo = new ModalAereo(unitOfWork);
            ModalFerroviario serModalFerroviario = new ModalFerroviario(unitOfWork);
            InformacaoModal serInformacaoModal = new InformacaoModal(unitOfWork);
            CTeSubstituicao serCTeSubstituicao = new CTeSubstituicao(unitOfWork);
            CTeAnulacao serCTeAnulacao = new CTeAnulacao(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte = new Dominio.ObjetosDeValor.Embarcador.CTe.CTe();
            cte.Codigo = dynCTe.CTe.Codigo != null ? (int)dynCTe.CTe.Codigo : 0;
            cte.CodigoDuplicado = dynCTe.CTe.CodigoDuplicado != null ? (int)dynCTe.CTe.CodigoDuplicado : 0;

            if ((int)dynCTe.CTe.Empresa > 0)
            {
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo((int)dynCTe.CTe.Empresa);
                cte.Emitente = serEmpresa.ConverterObjetoEmpresa(empresa);
            }
            else
            {
                Dominio.Entidades.Cliente terceiro = repCliente.BuscarPorCPFCNPJ((double)dynCTe.CTe.Terceiro);
                cte.Emitente = serEmpresa.ConverterObjetoEmpresa(terceiro);
            }

            if (dynCTe.CTe.Numero != null && (int)dynCTe.CTe.Numero > 0)
                cte.Numero = (int)dynCTe.CTe.Numero;
            else
                cte.Numero = dynCTe.CTe.NumeroTerceiro != null ? (int)dynCTe.CTe.NumeroTerceiro : 0;
            if ((int)dynCTe.CTe.Serie > 0)
            {
                Dominio.Entidades.EmpresaSerie empresaSerie = repEmpresaSerie.BuscarPorCodigo((int)dynCTe.CTe.Serie);
                cte.Serie = empresaSerie.Numero.ToString();
            }
            else
                cte.Serie = dynCTe.CTe.SerieTerceiro;

            cte.LocalidadeInicioPrestacao = serLocalidade.ConverterObjetoLocalidade(repLocalidade.BuscarPorCodigo((int)dynCTe.CTe.LocalidadeInicioPrestacao));
            cte.LocalidadeFimPrestacao = serLocalidade.ConverterObjetoLocalidade(repLocalidade.BuscarPorCodigo((int)dynCTe.CTe.LocalidadeTerminoPrestacao));
            cte.TipoCTE = (Dominio.Enumeradores.TipoCTE)dynCTe.CTe.Tipo;
            cte.TipoPagamento = (Dominio.Enumeradores.TipoPagamento)dynCTe.CTe.TipoPagamento;
            cte.TipoServico = (Dominio.Enumeradores.TipoServico)dynCTe.CTe.TipoServico;
            cte.TipoImpressao = (Dominio.Enumeradores.TipoImpressao)dynCTe.CTe.TipoImpressao;
            cte.TipoTomador = (Dominio.Enumeradores.TipoTomador)dynCTe.CTe.TipoTomador;

            if (dynCTe.CTe.TipoModal != null && !string.IsNullOrWhiteSpace((string)dynCTe.CTe.TipoModal) && (int)dynCTe.CTe.TipoModal > 0)
                cte.TipoModal = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal)dynCTe.CTe.TipoModal;
            else
                cte.TipoModal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Rodoviario;

            Dominio.Entidades.CFOP cfop = repCFOP.BuscarPorId((int)dynCTe.CTe.CFOP);
            cte.CFOP = cfop.CodigoCFOP;

            DateTime dataEmissao = new DateTime();
            DateTime.TryParseExact((string)dynCTe.CTe.DataEmissao, "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out dataEmissao);
            cte.DataEmissao = dataEmissao;

            cte.Retira = (bool)dynCTe.CTe.RecebedorRetira;
            cte.DetalhesRetira = (string)dynCTe.CTe.RecebedorRetiraDetalhes;
            cte.SituacaoCTeSefaz = BuscarSituacaoSefaz((string)dynCTe.CTe.Status);

            if (!string.IsNullOrWhiteSpace((string)dynCTe.CTe.Chave))
                cte.Chave = (string)dynCTe.CTe.Chave;
            else
                cte.Chave = Utilidades.String.RemoveAllSpecialCharacters((string)dynCTe.CTe.ChaveTerceiro);

            cte.Protocolo = (string)dynCTe.CTe.ProtocoloAutorizacao;

            cte.IndicadorGlobalizado = ((string)dynCTe.CTe.Globalizado).ToEnum<Dominio.Enumeradores.OpcaoSimNao>();
            cte.IndicadorIETomador = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE)dynCTe.CTe.IndicadorTomador;

            if (cte.TipoCTE == Dominio.Enumeradores.TipoCTE.Simplificado)
                serDocumentoCTe.SetarDynamicParaEntregasSimplificado(dynCTe, ref cte, unitOfWork);
            else
                serDocumentoCTe.SetarDynamicParaDocumentos(dynCTe, ref cte, unitOfWork);

            cte.Remetente = serParticipante.ConverterDynamicParaParticipante(dynCTe.Remetente, unitOfWork);
            cte.Destinatario = serParticipante.ConverterDynamicParaParticipante(dynCTe.Destinatario, unitOfWork);
            cte.Expedidor = serParticipante.ConverterDynamicParaParticipante(dynCTe.Expedidor, unitOfWork);
            cte.Recebedor = serParticipante.ConverterDynamicParaParticipante(dynCTe.Recebedor, unitOfWork);
            cte.Tomador = serParticipante.ConverterDynamicParaParticipante(dynCTe.Tomador, unitOfWork);

            if (cte.TipoModal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Rodoviario)
                cte.ModalRodoviario = serModalRodoviario.ConverterDynamicModalRodoviario(dynCTe, unitOfWork);
            if (cte.TipoModal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Aereo)
                cte.ModalAereo = serModalAereo.ConverterDynamicModalAereo(dynCTe, unitOfWork);
            else if (cte.TipoModal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Aquaviario)
                cte.ModalAquaviario = serModalAquaviario.ConverterDynamicModalAquaviario(dynCTe, unitOfWork);
            else if (cte.TipoModal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Ferroviario)
                cte.ModalFerroviario = serModalFerroviario.ConverterDynamicModalFerroviario(dynCTe, unitOfWork);
            else if (cte.TipoModal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Dutoviario)
                cte.ModalDutoviario = serModalDutoviario.ConverterDynamicModalDutoviario(dynCTe, unitOfWork);
            else if (cte.TipoModal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Multimodal)
                cte.ModalMultimodal = serModalMultimodal.ConverterDynamicModalMultimodal(dynCTe, unitOfWork);
            cte.InformacaoModal = serInformacaoModal.ConverterDynamicInformacaoModal(dynCTe.InformacaoModal, unitOfWork);
            cte.Containers = serModalAquaviario.ConverterDynamicParaConteiners(dynCTe, unitOfWork);

            cte.InformacaoCarga = serInformacaoCarga.ConverterDynamicParaInformacaoCarga(dynCTe.InformacaoCarga, unitOfWork);
            cte.QuantidadesCarga = serQuantidades.ConverterDynamicParaQuantidades(dynCTe.QuantidadesCarga, unitOfWork);
            cte.Seguros = serSeguros.ConverterDynamicParaSeguro(dynCTe.Seguros, unitOfWork);
            cte.ProdutosPerigosos = serProdutoPerigoso.ConverterDynamicParaProdutoPerigoso(dynCTe.ProdutosPerigosos, unitOfWork);
            cte.DocumentosAnteriores = serDocumentoAnterior.ConverterDynamicParaDocumentosAnteriores(dynCTe.DocumentosTransporteAnteriorEletronico, unitOfWork);
            cte.DocumentosAnterioresDePapel = serDocumentoTransportaAnteriorPapel.ConverterDynamicParaDocumentosTransporteAnteriorPapel(dynCTe.DocumentosTransporteAnteriorPapel, unitOfWork);
            cte.Duplicata = serDuplicatas.ConverterDynamicParaDuplicata(dynCTe.Duplicata, unitOfWork);

            cte.ObservacoesContribuinte = serObservacoes.ConverterDynamicParaObservacoes(dynCTe.ObservacoesContribuinte);
            cte.ObservacoesFisco = serObservacoes.ConverterDynamicParaObservacoes(dynCTe.ObservacoesFisco);

            if (cte.Codigo == 0)
                serObservacoes.SetarObservacaoContribuinte(ref cte, unitOfWork);

            if (!string.IsNullOrWhiteSpace((string)dynCTe.ObservacaoGeral.ObservacaoGeral))
            {
                cte.ObservacoesGeral = new Dominio.ObjetosDeValor.Embarcador.CTe.Observacao();
                cte.ObservacoesGeral.Campo = "Observação Geral";
                cte.ObservacoesGeral.Texto = (string)dynCTe.ObservacaoGeral.ObservacaoGeral;
            }

            cte.ValorFrete = serTotalServicos.ConverterDynamicParaFreteValor(dynCTe, unitOfWork);

            if (cte.TipoCTE == Dominio.Enumeradores.TipoCTE.Substituto)
                cte.CTeSubstituicao = serCTeSubstituicao.ConverterDynamicCTeSubstituicao(dynCTe.CTeSubstituicao, unitOfWork);
            else if (cte.TipoCTE == Dominio.Enumeradores.TipoCTE.Anulacao)
                cte.CTeAnulacao = serCTeAnulacao.ConverterDynamicCTeAnulacao(dynCTe.CTeAnulacao);
            else if (cte.TipoCTE == Dominio.Enumeradores.TipoCTE.Complemento)
                cte.CTeComplementar = svcCTeComplementar.ConverterDynamicCTeComplementar(dynCTe.CTeComplementar);

            return cte;

        }

        public string BuscarStatusCTe(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz situacaoSefaz)
        {
            switch (situacaoSefaz)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Autorizada:
                    return "A";
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Cancelada:
                    return "C";
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Denegada:
                    return "D";
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Rejeitada:
                    return "R";
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Pendente:
                    return "P";
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Enviada:
                    return "E";
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Inutilizada:
                    return "I";
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.EmDigitacao:
                    return "S";
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.EmCancelamento:
                    return "K";
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.EmInutilizacao:
                    return "L";
                default:
                    return "A";
            }
        }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz BuscarSituacaoSefaz(string Status)
        {
            switch (Status)
            {
                case "P":
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Pendente;
                case "E":
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Enviada;
                case "R":
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Rejeitada;
                case "A":
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Autorizada;
                case "C":
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Cancelada;
                case "I":
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Inutilizada;
                case "D":
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Denegada;
                case "S":
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.EmDigitacao;
                case "K":
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.EmCancelamento;
                case "L":
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.EmInutilizacao;
                case "G":
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.AnuladoGerencialmente;
                default:
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Pendente;
            }
        }

        public Dominio.ObjetosDeValor.Embarcador.CTe.CTe ConverterProcCTeParaCTePorObjeto(object cteProc)
        {
            Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte = new Dominio.ObjetosDeValor.Embarcador.CTe.CTe();

            if (cteProc.GetType() == typeof(MultiSoftware.CTe.v200.ConhecimentoDeTransporteProcessado.cteProc))
                return ConverterProcCTeParaCTe((MultiSoftware.CTe.v200.ConhecimentoDeTransporteProcessado.cteProc)cteProc);
            else if (cteProc.GetType() == typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc))
                return ConverterProcCTeParaCTe((MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc)cteProc);
            else if (cteProc.GetType() == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteProc))
                return ConverterProcCTeParaCTe((MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteProc)cteProc);
            else if (cteProc.GetType() == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteSimpProc))
                return ConverterProcCTeParaCTe((MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteSimpProc)cteProc);

            return null;
        }

        public Dominio.ObjetosDeValor.Embarcador.CTe.CTe ConverterProcCTeParaCTe(MultiSoftware.CTe.v200.ConhecimentoDeTransporteProcessado.cteProc cteProc)
        {
            Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte = new Dominio.ObjetosDeValor.Embarcador.CTe.CTe();
            PreecherDadosCTe(ref cte, cteProc);
            return cte;
        }

        public Dominio.ObjetosDeValor.Embarcador.CTe.CTe ConverterProcCTeParaCTe(MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc cteProc)
        {
            Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte = new Dominio.ObjetosDeValor.Embarcador.CTe.CTe();
            PreecherDadosCTe(ref cte, cteProc);
            return cte;
        }

        public Dominio.ObjetosDeValor.Embarcador.CTe.CTe ConverterProcCTeParaCTe(MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteProc cteProc)
        {
            Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte = new Dominio.ObjetosDeValor.Embarcador.CTe.CTe();
            PreecherDadosCTe(ref cte, cteProc);
            return cte;
        }

        public Dominio.ObjetosDeValor.Embarcador.CTe.CTe ConverterProcCTeParaCTe(MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteSimpProc cteProc)
        {
            Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte = new Dominio.ObjetosDeValor.Embarcador.CTe.CTe();
            PreecherDadosCTe(ref cte, cteProc);
            return cte;
        }

        public static bool CancelarOuAnularCTe(out string erro, int codigoCTe, string justificativa, Repositorio.UnitOfWork unidadeTrabalho, string stringConexao, string webServiceConsultaCTe, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Financeiro.TituloDocumento repTituloDocumento = new Repositorio.Embarcador.Financeiro.TituloDocumento(unidadeTrabalho);
            Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual repCargaDocumentoParaEmissaoNFSManual = new Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual(unidadeTrabalho);
            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unidadeTrabalho);
            Repositorio.Embarcador.Acerto.AcertoCarga repAcertoCarga = new Repositorio.Embarcador.Acerto.AcertoCarga(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCTe.BuscarPorCTe(codigoCTe);

            if (!VerificarSeCTeEstaAptoParaCancelamento(out erro, cargaCTe, unidadeTrabalho))
                return false;

            if (cargaCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe || cargaCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe)
            {
                if (!CancelarCTe(out erro, cargaCTe.CTe, justificativa, unidadeTrabalho, stringConexao, webServiceConsultaCTe, tipoServicoMultisoftware))
                    return false;
            }
            else
            {
                if (!CancelarOutrosDocumentos(out erro, cargaCTe, justificativa, unidadeTrabalho, stringConexao, tipoServicoMultisoftware))
                    return false;
            }

            erro = string.Empty;
            return true;
        }

        public static bool VerificarSeCTeEstaAptoParaCancelamento(out string erro, Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe, Repositorio.UnitOfWork unitOfWork, bool naoValidarCTeEmitidoOutroSistema = false, bool anulacaoGerencial = false)
        {
            Repositorio.Embarcador.Financeiro.TituloDocumento repTituloDocumento = new Repositorio.Embarcador.Financeiro.TituloDocumento(unitOfWork);
            Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual repCargaDocumentoParaEmissaoNFSManual = new Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual(unitOfWork);
            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);
            Repositorio.Embarcador.Acerto.AcertoCarga repAcertoCarga = new Repositorio.Embarcador.Acerto.AcertoCarga(unitOfWork);

            if (cargaCTe == null)
            {
                erro = "CT-e não encontrado.";
                return false;
            }

            if (cargaCTe.CTe.Status != "A" && cargaCTe.CTe.Status != "Z")
            {
                erro = "A situação do CT-e não permite o cancelamento/anulação do mesmo.";
                return false;
            }

            if (cargaCTe.Carga.CargaTransbordo)
            {
                erro = "Não é possível cancelar/anular um CT-e de uma carga de transbordo.";
                return false;
            }

            if (!naoValidarCTeEmitidoOutroSistema && cargaCTe.SistemaEmissor != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor.MultiCTe)
            {
                erro = "Não é possível cancelar/anular um CT-e emitido em outro sistema.";
                return false;
            }

            if (repCargaDocumentoParaEmissaoNFSManual.ExisteGeradoPorCargaCTe(cargaCTe.Codigo))
            {
                erro = "Existe uma NFS manual gerada para o CT-e, não sendo possível cancelar/anular o mesmo.";
                return false;
            }

            if (repCargaDocumentoParaEmissaoNFSManual.ExisteGeradoPorPedidoXMLNotaFiscal(cargaCTe.Codigo))//cargaCTe.NotasFiscais.Select(o => o.PedidoXMLNotaFiscal.Codigo).ToArray()))
            {
                erro = "Existe uma NFS manual gerada para as notas fiscais do CT-e, não sendo possível cancelar/anular o mesmo.";
                return false;
            }

            if (cargaCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS)
            {
                erro = "Não é possível cancelar/anular uma NFS manual, utilize a tela de cancelamento de NFS manual.";
                return false;
            }

            List<int> numeros = repFaturaDocumento.BuscarNumeroFaturaPorCTe(cargaCTe.CTe.Codigo);

            if (numeros.Count > 0)
            {
                erro = "Não é possível cancelar/anular o CT-e pois ele está na(s) fatura(s) " + string.Join(", ", numeros) + ".";
                return false;
            }

            numeros = repFaturaDocumento.BuscarNumeroFaturaPorCarga(cargaCTe.Carga.Codigo);

            if (numeros.Count > 0)
            {
                erro = "Não é possível cancelar/anular o CT-e pois a carga está na(s) fatura(s) " + string.Join(", ", numeros) + ".";
                return false;
            }

            numeros = repTituloDocumento.BuscarNumeroTituloPorCTe(cargaCTe.CTe.Codigo);

            if (numeros.Count > 0)
            {
                erro = "Não é possível cancelar/anular o CT-e pois ele está no(s) título(s) " + string.Join(", ", numeros) + ".";
                return false;
            }

            numeros = repTituloDocumento.BuscarNumeroTituloPorCarga(cargaCTe.Carga.Codigo);

            if (numeros.Count > 0)
            {
                erro = "Não é possível cancelar/anular o CT-e pois a carga está no(s) título(s) " + string.Join(", ", numeros) + ".";
                return false;
            }

            numeros = repTituloDocumento.BuscarNumeroBoletoTituloPorCTe(cargaCTe.CTe.Codigo);

            if (numeros.Count > 0)
            {
                erro = "Não é possível cancelar/anular o CT-e pois ele está vinculado à boleto(s) no(s) título(s) " + string.Join(", ", numeros) + ".";
                return false;
            }

            numeros = repTituloDocumento.BuscarNumeroBoletoTituloPorCarga(cargaCTe.Carga.Codigo);

            if (numeros.Count > 0)
            {
                erro = "Não é possível cancelar/anular o CT-e pois a carga está vinculada à boleto(s) no(s) título(s) " + string.Join(", ", numeros) + ".";
                return false;
            }

            Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete = repContratoFrete.BuscarPorCarga(cargaCTe.Carga.Codigo);

            if (!cargaCTe.CTe.GeradoManualmente && !anulacaoGerencial && contratoFrete != null && (contratoFrete.SituacaoContratoFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Finalizada || contratoFrete.SituacaoContratoFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Aprovado))
            {
                erro = "A carga está vinculada ao contrato de frete nº " + contratoFrete.NumeroContrato.ToString() + ", que já está aprovado/finalizado, não sendo possível realizar o cancelamento/anulação do CT-e.";
                return false;
            }

            numeros = repAcertoCarga.BuscarNumeroAcertoPorCarga(cargaCTe.Carga.Codigo);

            if (numeros.Count > 0)
            {
                erro = "A carga está vinculada ao(s) acerto(s) de viagem nº " + string.Join(", ", numeros) + ", não sendo possível realizar o cancelamento/anulação do CT-e.";
                return false;
            }

            erro = string.Empty;
            return true;
        }

        public static bool VerificarSeCTeEstaAptoParaCancelamento(out string erro, Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.TituloDocumento repTituloDocumento = new Repositorio.Embarcador.Financeiro.TituloDocumento(unitOfWork);
            Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual repCargaDocumentoParaEmissaoNFSManual = new Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual(unitOfWork);
            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);
            Repositorio.Embarcador.Acerto.AcertoCarga repAcertoCarga = new Repositorio.Embarcador.Acerto.AcertoCarga(unitOfWork);

            if (cte == null)
            {
                erro = "CT-e não encontrado.";
                return false;
            }

            if (cte.Status != "A" && cte.Status != "Z")
            {
                erro = "A situação do CT-e não permite o cancelamento/anulação do mesmo.";
                return false;
            }

            List<int> numeros = repFaturaDocumento.BuscarNumeroFaturaPorCTe(cte.Codigo);

            if (numeros.Count > 0)
            {
                erro = "Não é possível cancelar/anular o CT-e pois ele está na(s) fatura(s) " + string.Join(", ", numeros) + ".";
                return false;
            }

            numeros = repTituloDocumento.BuscarNumeroTituloPorCTe(cte.Codigo);

            if (numeros.Count > 0)
            {
                erro = "Não é possível cancelar/anular o CT-e pois ele está no(s) título(s) " + string.Join(", ", numeros) + ".";
                return false;
            }

            numeros = repTituloDocumento.BuscarNumeroBoletoTituloPorCTe(cte.Codigo);

            if (numeros.Count > 0)
            {
                erro = "Não é possível cancelar/anular o CT-e pois ele está vinculado à boleto(s) no(s) título(s) " + string.Join(", ", numeros) + ".";
                return false;
            }

            erro = null;
            return true;
        }

        public static bool GerarCTeAnulacao(out string mensagemErro, out Dominio.Entidades.Embarcador.CTe.ControleGeracaoCTeAnulacao controleGeracaoCTeAnulacao, Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTeOriginal, int codigoCarga, DateTime dataEventoDesacordo, decimal valorCTeSubstituicao, string observacaoCTeAnulacao, string observacaoCTeSubstituicao, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, double tomadorCTeSubstituto, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, bool naoGerarCTeSubstituicao, dynamic componentesFrete = null)
        {
            mensagemErro = string.Empty;
            controleGeracaoCTeAnulacao = null;

            if (!VerificarSeCTeEstaAptoParaCancelamento(out mensagemErro, cargaCTeOriginal, unitOfWork, false, true))
                return false;

            Servicos.CTe serCTE = new Servicos.CTe(unitOfWork);

            Repositorio.Embarcador.Pedidos.PedidoViagemNavioSchedule repPedidoNavioSchedule = new Repositorio.Embarcador.Pedidos.PedidoViagemNavioSchedule(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.CFOP repCFOP = new Repositorio.CFOP(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.CTe.ControleGeracaoCTeAnulacao repControleGeracaoCTeAnulacao = new Repositorio.Embarcador.CTe.ControleGeracaoCTeAnulacao(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.CTe.CTeRelacaoDocumento repCTeRelacaoDocumento = new Repositorio.Embarcador.CTe.CTeRelacaoDocumento(unitOfWork);

            Dominio.Entidades.ConhecimentoDeTransporteEletronico cteAnulacao = new Dominio.Entidades.ConhecimentoDeTransporteEletronico();

            cargaCTeOriginal.CTe.CopyProperties(cteAnulacao);

            cteAnulacao.Codigo = 0;

            Utilidades.Object.DefinirListasGenericasComoNulas(cteAnulacao);

            if (cargaCTeOriginal.CTe.LocalidadeInicioPrestacao.Estado.Sigla == cargaCTeOriginal.CTe.LocalidadeTerminoPrestacao.Estado.Sigla)
                cteAnulacao.CFOP = repCFOP.BuscarPorNumero(1206);
            else
                cteAnulacao.CFOP = repCFOP.BuscarPorNumero(2206);

            if (cteAnulacao.CFOP == null)
            {
                mensagemErro = "CFOP para anulação não está configurada.";
                return false;
            }

            cteAnulacao.Chave = "";
            cteAnulacao.NumeroRecibo = "";
            cteAnulacao.DataAutorizacao = null;
            cteAnulacao.DataIntegracao = null;
            cteAnulacao.DataRetornoSefaz = null;
            cteAnulacao.DataEmissao = DateTime.Now;
            cteAnulacao.ChaveCTESubComp = cargaCTeOriginal.CTe.Chave;
            cteAnulacao.ObservacoesGerais = $"CT-e de anulação referente ao documento número {cargaCTeOriginal.CTe.Numero}, série {cargaCTeOriginal.CTe.Serie.Numero}, emitido em {cargaCTeOriginal.CTe.DataEmissao:dd/MM/yyyy HH:mm} e com chave de acesso {cargaCTeOriginal.CTe.Chave}. {observacaoCTeAnulacao}";
            cteAnulacao.TipoCTE = Dominio.Enumeradores.TipoCTE.Anulacao;
            cteAnulacao.Log = "";
            cteAnulacao.LogIntegracao = "";
            cteAnulacao.MensagemStatus = null;
            cteAnulacao.Status = "P";
            cteAnulacao.CodigoCTeIntegrador = 0;
            cteAnulacao.DataAnulacao = dataEventoDesacordo;

            if (cargaCTeOriginal.CTe.Remetente != null)
            {
                cteAnulacao.Remetente = new Dominio.Entidades.ParticipanteCTe();

                cargaCTeOriginal.CTe.Remetente.CopyProperties(cteAnulacao.Remetente);

                cteAnulacao.Remetente.Codigo = 0;

                Utilidades.Object.DefinirListasGenericasComoNulas(cteAnulacao.Remetente);
            }

            if (cargaCTeOriginal.CTe.Expedidor != null)
            {
                cteAnulacao.Expedidor = new Dominio.Entidades.ParticipanteCTe();

                cargaCTeOriginal.CTe.Expedidor.CopyProperties(cteAnulacao.Expedidor);

                cteAnulacao.Expedidor.Codigo = 0;

                Utilidades.Object.DefinirListasGenericasComoNulas(cteAnulacao.Expedidor);
            }

            if (cargaCTeOriginal.CTe.Recebedor != null)
            {
                cteAnulacao.Recebedor = new Dominio.Entidades.ParticipanteCTe();

                cargaCTeOriginal.CTe.Recebedor.CopyProperties(cteAnulacao.Recebedor);

                cteAnulacao.Recebedor.Codigo = 0;

                Utilidades.Object.DefinirListasGenericasComoNulas(cteAnulacao.Recebedor);
            }

            if (cargaCTeOriginal.CTe.Destinatario != null)
            {
                cteAnulacao.Destinatario = new Dominio.Entidades.ParticipanteCTe();

                cargaCTeOriginal.CTe.Destinatario.CopyProperties(cteAnulacao.Destinatario);

                cteAnulacao.Destinatario.Codigo = 0;

                Utilidades.Object.DefinirListasGenericasComoNulas(cteAnulacao.Destinatario);
            }

            if (cargaCTeOriginal.CTe.OutrosTomador != null)
            {
                cteAnulacao.OutrosTomador = new Dominio.Entidades.ParticipanteCTe();

                cargaCTeOriginal.CTe.OutrosTomador.CopyProperties(cteAnulacao.OutrosTomador);

                cteAnulacao.OutrosTomador.Codigo = 0;

                Utilidades.Object.DefinirListasGenericasComoNulas(cteAnulacao.OutrosTomador);
            }

            if (cargaCTeOriginal.CTe.EnderecoRemetente != null)
            {
                cteAnulacao.EnderecoRemetente = new Dominio.Entidades.EnderecoParticipanteCTe();

                cargaCTeOriginal.CTe.EnderecoRemetente.CopyProperties(cteAnulacao.EnderecoRemetente);

                cteAnulacao.EnderecoRemetente.Codigo = 0;

                Utilidades.Object.DefinirListasGenericasComoNulas(cteAnulacao.EnderecoRemetente);
            }

            if (cargaCTeOriginal.CTe.EnderecoExpedidor != null)
            {
                cteAnulacao.EnderecoExpedidor = new Dominio.Entidades.EnderecoParticipanteCTe();

                cargaCTeOriginal.CTe.EnderecoExpedidor.CopyProperties(cteAnulacao.EnderecoExpedidor);

                cteAnulacao.EnderecoExpedidor.Codigo = 0;

                Utilidades.Object.DefinirListasGenericasComoNulas(cteAnulacao.EnderecoExpedidor);
            }

            if (cargaCTeOriginal.CTe.EnderecoRecebedor != null)
            {
                cteAnulacao.EnderecoRecebedor = new Dominio.Entidades.EnderecoParticipanteCTe();

                cargaCTeOriginal.CTe.EnderecoRecebedor.CopyProperties(cteAnulacao.EnderecoRecebedor);

                cteAnulacao.EnderecoRecebedor.Codigo = 0;

                Utilidades.Object.DefinirListasGenericasComoNulas(cteAnulacao.EnderecoRecebedor);
            }

            if (cargaCTeOriginal.CTe.EnderecoDestinatario != null)
            {
                cteAnulacao.EnderecoDestinatario = new Dominio.Entidades.EnderecoParticipanteCTe();

                cargaCTeOriginal.CTe.EnderecoDestinatario.CopyProperties(cteAnulacao.EnderecoDestinatario);

                cteAnulacao.EnderecoDestinatario.Codigo = 0;

                Utilidades.Object.DefinirListasGenericasComoNulas(cteAnulacao.EnderecoDestinatario);
            }

            if (cargaCTeOriginal.CTe.EnderecoTomador != null)
            {
                cteAnulacao.EnderecoTomador = new Dominio.Entidades.EnderecoParticipanteCTe();

                cargaCTeOriginal.CTe.EnderecoTomador.CopyProperties(cteAnulacao.EnderecoTomador);

                cteAnulacao.EnderecoTomador.Codigo = 0;

                Utilidades.Object.DefinirListasGenericasComoNulas(cteAnulacao.EnderecoTomador);
            }

            cteAnulacao.TomadorPagador = cteAnulacao.Tomador;

            unitOfWork.Start();

            cteAnulacao.Numero = serCTE.ObterProximoNumero(cteAnulacao, repCTe);

            cteAnulacao.NumeroControle = serCTE.RetornarNumeroControleCTe(out int numeroSequencia, cteAnulacao.NumeroBooking, cteAnulacao.DescricaoCarrier, cteAnulacao.TipoPropostaFeeder, cteAnulacao.TipoModal, cteAnulacao.TipoServico, 0, cteAnulacao.SVMTerceiro, unitOfWork, false, 0, false, cteAnulacao?.SequenciaBooking ?? 0);
            cteAnulacao.PedidoViagemNavioSchedule = repPedidoNavioSchedule.BuscarPorCodigo(repCTe.BuscarCodigoSchedule(cteAnulacao.ViagemPassagemCinco?.Codigo ?? 0, cteAnulacao.ViagemPassagemQuatro?.Codigo ?? 0, cteAnulacao.ViagemPassagemTres?.Codigo ?? 0, cteAnulacao.ViagemPassagemDois?.Codigo ?? 0, cteAnulacao.ViagemPassagemUm?.Codigo ?? 0, cteAnulacao.Viagem?.Codigo ?? 0, cteAnulacao.PortoDestino?.Codigo ?? 0, cteAnulacao.TerminalDestino?.Codigo ?? 0));
            cteAnulacao.SequenciaBooking = numeroSequencia;
            if (cteAnulacao.NumeroControle == "0" || string.IsNullOrWhiteSpace(cteAnulacao.NumeroControle))
                cteAnulacao.NumeroControle = cteAnulacao.Codigo.ToString("D");
            else
            {
                if (repCTe.ContemNumeroControleDuplicado(cteAnulacao.NumeroControle, cteAnulacao.Empresa?.Codigo ?? 0, cteAnulacao.Codigo))
                {
                    cteAnulacao.NumeroControle = serCTE.RetornarNumeroControleCTe(out numeroSequencia, cteAnulacao.NumeroBooking, cteAnulacao.DescricaoCarrier, cteAnulacao.TipoPropostaFeeder, cteAnulacao.TipoModal, cteAnulacao.TipoServico, cteAnulacao.Codigo, cteAnulacao.SVMTerceiro, unitOfWork, true, cteAnulacao.SequenciaBooking, false, cteAnulacao?.SequenciaBooking ?? 0);
                    cteAnulacao.SequenciaBooking = numeroSequencia;

                    if (repCTe.ContemNumeroControleDuplicado(cteAnulacao.NumeroControle, cteAnulacao.Empresa?.Codigo ?? 0, cteAnulacao.Codigo))
                    {
                        bool contemNumeroDuplicado = true;
                        int count = 0;
                        while (contemNumeroDuplicado)
                        {
                            cteAnulacao.NumeroControle = serCTE.RetornarNumeroControleCTe(out numeroSequencia, cteAnulacao.NumeroBooking, cteAnulacao.DescricaoCarrier, cteAnulacao.TipoPropostaFeeder, cteAnulacao.TipoModal, cteAnulacao.TipoServico, cteAnulacao.Codigo, cteAnulacao.SVMTerceiro, unitOfWork, true, cteAnulacao.SequenciaBooking, false, cteAnulacao?.SequenciaBooking ?? 0);
                            cteAnulacao.SequenciaBooking = numeroSequencia;
                            contemNumeroDuplicado = repCTe.ContemNumeroControleDuplicado(cteAnulacao.NumeroControle, cteAnulacao.Empresa?.Codigo ?? 0, cteAnulacao.Codigo);
                            count++;
                            if (count > 100)
                                break;
                        }
                    }
                    //repCTe.Atualizar(cte);
                }
            }

            if (!string.IsNullOrWhiteSpace(cteAnulacao.NumeroControle))
            {
                cteAnulacao.ObservacoesGerais += " Num Ctrl: " + cteAnulacao.NumeroControle;
                cteAnulacao.ObservacoesGerais = cteAnulacao.ObservacoesGerais.Trim();
            }
            if (cteAnulacao.TipoModal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Aquaviario && cteAnulacao.PortoOrigem != null && cteAnulacao.PortoOrigem != null && cteAnulacao.TomadorPagador != null && cteAnulacao.TomadorPagador.GrupoPessoas != null)
            {
                if (cteAnulacao.PortoOrigem.AtivarDespachanteComoConsignatario && cteAnulacao.PortoDestino.AtivarDespachanteComoConsignatario && cteAnulacao.TomadorPagador != null && cteAnulacao.TomadorPagador.GrupoPessoas != null && cteAnulacao.TomadorPagador.GrupoPessoas.AdicionarDespachanteComoConsignatario && cteAnulacao.TomadorPagador.GrupoPessoas.Despachante != null)
                {
                    cteAnulacao.ObservacoesGerais += " A " + cteAnulacao.TomadorPagador.GrupoPessoas.Despachante.Nome + ", CNPJ " + cteAnulacao.TomadorPagador.GrupoPessoas.Despachante.CPF_CNPJ_Formatado + ", irá atuar como despachante para a liberação dessa carga no porto de descarga.";
                    cteAnulacao.ObservacoesGerais = cteAnulacao.ObservacoesGerais.Trim();
                }
            }

            repCTe.Inserir(cteAnulacao, auditado);

            DuplicarInformacoesContainer(cargaCTeOriginal.CTe, cteAnulacao, unitOfWork);
            DuplicarInformacoesQuantidadeCarga(cargaCTeOriginal.CTe, cteAnulacao, unitOfWork);
            DuplicarSeguros(cargaCTeOriginal.CTe, cteAnulacao, unitOfWork);
            DuplicarMotoristas(cargaCTeOriginal.CTe, cteAnulacao, unitOfWork);
            DuplicarVeiculos(cargaCTeOriginal.CTe, cteAnulacao, unitOfWork);
            DuplicarComponentePrestacao(cargaCTeOriginal.CTe, cteAnulacao, unitOfWork);
            DuplicarDocumentoDeTransporteAnterior(cargaCTeOriginal.CTe, cteAnulacao, unitOfWork);
            DuplicarDocumentos(cargaCTeOriginal.CTe, cteAnulacao, unitOfWork);
            DuplicarProdutoPerigoso(cargaCTeOriginal.CTe, cteAnulacao, unitOfWork);
            DuplicarObservacoesContribuinte(cargaCTeOriginal.CTe, cteAnulacao, unitOfWork);
            DuplicarObservacoesFisco(cargaCTeOriginal.CTe, cteAnulacao, unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTeAnulacao = new Dominio.Entidades.Embarcador.Cargas.CargaCTe()
            {
                Carga = cargaCTeOriginal.Carga,
                CargaOrigem = cargaCTeOriginal.Carga,
                CTe = cteAnulacao,
                SistemaEmissor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor.MultiCTe
            };

            repCargaCTe.Inserir(cargaCTeAnulacao, auditado);

            controleGeracaoCTeAnulacao = new Dominio.Entidades.Embarcador.CTe.ControleGeracaoCTeAnulacao()
            {
                CargaCTeAnulacao = cargaCTeAnulacao,
                DataEventoDesacordo = dataEventoDesacordo,
                CargaCTeOriginal = cargaCTeOriginal,
                ObservacaoAnulacao = observacaoCTeAnulacao,
                ObservacaoSubstituicao = observacaoCTeSubstituicao,
                ValorCTeSubstituicao = valorCTeSubstituicao,
                Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoControleGeracaoCTeAnulacao.AguardandoAutorizacaoAnulacao,
                OutroTomador = tomadorCTeSubstituto > 0 ? repCliente.BuscarPorCPFCNPJ(tomadorCTeSubstituto) : null,
                NaoGerarCTeSubstituicao = naoGerarCTeSubstituicao
            };

            repControleGeracaoCTeAnulacao.Inserir(controleGeracaoCTeAnulacao, auditado);

            SalvarComponentesControleGeracaoCTeAnulacao(controleGeracaoCTeAnulacao, componentesFrete, unitOfWork);

            if (configuracaoEmbarcador.DeixarCargaPendenteDeIntegracaoAposCTeManual && codigoCarga > 0)
            {
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);
                if (carga != null)
                {
                    carga.CargaIntegradaEmbarcador = false;
                    repCarga.Atualizar(carga);
                    Servicos.Auditoria.Auditoria.Auditar(auditado, carga, null, "Alterou a carga para pendente de integração devido a geração de um CT-e Manual.", unitOfWork);
                }
            }

            if (cteAnulacao != null && cargaCTeOriginal?.CTe != null)
            {
                Dominio.Entidades.Embarcador.CTe.CTeRelacaoDocumento cTeRelacao = new Dominio.Entidades.Embarcador.CTe.CTeRelacaoDocumento()
                {
                    CTeGerado = cteAnulacao,
                    CTeOriginal = cargaCTeOriginal.CTe,
                    TipoCTeGerado = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCTeGerado.Anulacao
                };
                repCTeRelacaoDocumento.Inserir(cTeRelacao);
            }

            unitOfWork.CommitChanges();

            Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);

            if (!svcCTe.Emitir(ref cteAnulacao, unitOfWork))
            {
                mensagemErro = "O CT-e foi gerado, porém, ocorreram problemas ao enviar para a SEFAZ. Tente reenviar o mesmo manualmente ou contate o suporte técnico.";
                return false;
            }

            return true;
        }

        public static bool GerarCTeCopia(out string mensagemErro, Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTeOriginal, int codigoCarga, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, bool desvincularCTeOriginalCarga, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Repositorio.UnitOfWork unitOfWork)
        {
            mensagemErro = string.Empty;

            //if (!VerificarSeCTeEstaAptoParaCopia(out mensagemErro, cargaCTeOriginal, unitOfWork, false, true))
            //return false;

            Servicos.CTe serCTE = new Servicos.CTe(unitOfWork);

            Repositorio.Embarcador.Pedidos.PedidoViagemNavioSchedule repPedidoNavioSchedule = new Repositorio.Embarcador.Pedidos.PedidoViagemNavioSchedule(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.CFOP repCFOP = new Repositorio.CFOP(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.CTe.ControleGeracaoCTeAnulacao repControleGeracaoCTeAnulacao = new Repositorio.Embarcador.CTe.ControleGeracaoCTeAnulacao(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.CTe.CTeRelacaoDocumento repCTeRelacaoDocumento = new Repositorio.Embarcador.CTe.CTeRelacaoDocumento(unitOfWork);

            Dominio.Entidades.ConhecimentoDeTransporteEletronico cteCopia = new Dominio.Entidades.ConhecimentoDeTransporteEletronico();

            cargaCTeOriginal.CTe.CopyProperties(cteCopia);

            cteCopia.Codigo = 0;

            Utilidades.Object.DefinirListasGenericasComoNulas(cteCopia);

            cteCopia.Chave = "";
            cteCopia.NumeroRecibo = "";
            cteCopia.DataAutorizacao = null;
            cteCopia.DataIntegracao = null;
            cteCopia.DataRetornoSefaz = null;
            cteCopia.DataEmissao = DateTime.Now;
            cteCopia.Log = "";
            cteCopia.LogIntegracao = "";
            cteCopia.MensagemStatus = null;
            cteCopia.Status = "P";
            cteCopia.CodigoCTeIntegrador = 0;
            cteCopia.CodStatusProtocolo = null;

            if (cargaCTeOriginal.CTe.Remetente != null)
            {
                cteCopia.Remetente = new Dominio.Entidades.ParticipanteCTe();
                cargaCTeOriginal.CTe.Remetente.CopyProperties(cteCopia.Remetente);
                cteCopia.Remetente.Codigo = 0;
                Utilidades.Object.DefinirListasGenericasComoNulas(cteCopia.Remetente);
            }

            if (cargaCTeOriginal.CTe.Expedidor != null)
            {
                cteCopia.Expedidor = new Dominio.Entidades.ParticipanteCTe();
                cargaCTeOriginal.CTe.Expedidor.CopyProperties(cteCopia.Expedidor);
                cteCopia.Expedidor.Codigo = 0;
                Utilidades.Object.DefinirListasGenericasComoNulas(cteCopia.Expedidor);
            }

            if (cargaCTeOriginal.CTe.Recebedor != null)
            {
                cteCopia.Recebedor = new Dominio.Entidades.ParticipanteCTe();
                cargaCTeOriginal.CTe.Recebedor.CopyProperties(cteCopia.Recebedor);
                cteCopia.Recebedor.Codigo = 0;
                Utilidades.Object.DefinirListasGenericasComoNulas(cteCopia.Recebedor);
            }

            if (cargaCTeOriginal.CTe.Destinatario != null)
            {
                cteCopia.Destinatario = new Dominio.Entidades.ParticipanteCTe();
                cargaCTeOriginal.CTe.Destinatario.CopyProperties(cteCopia.Destinatario);
                cteCopia.Destinatario.Codigo = 0;
                Utilidades.Object.DefinirListasGenericasComoNulas(cteCopia.Destinatario);
            }

            if (cargaCTeOriginal.CTe.OutrosTomador != null)
            {
                cteCopia.OutrosTomador = new Dominio.Entidades.ParticipanteCTe();
                cargaCTeOriginal.CTe.OutrosTomador.CopyProperties(cteCopia.OutrosTomador);
                cteCopia.OutrosTomador.Codigo = 0;
                Utilidades.Object.DefinirListasGenericasComoNulas(cteCopia.OutrosTomador);
            }

            if (cargaCTeOriginal.CTe.EnderecoRemetente != null)
            {
                cteCopia.EnderecoRemetente = new Dominio.Entidades.EnderecoParticipanteCTe();
                cargaCTeOriginal.CTe.EnderecoRemetente.CopyProperties(cteCopia.EnderecoRemetente);
                cteCopia.EnderecoRemetente.Codigo = 0;
                Utilidades.Object.DefinirListasGenericasComoNulas(cteCopia.EnderecoRemetente);
            }

            if (cargaCTeOriginal.CTe.EnderecoExpedidor != null)
            {
                cteCopia.EnderecoExpedidor = new Dominio.Entidades.EnderecoParticipanteCTe();
                cargaCTeOriginal.CTe.EnderecoExpedidor.CopyProperties(cteCopia.EnderecoExpedidor);
                cteCopia.EnderecoExpedidor.Codigo = 0;
                Utilidades.Object.DefinirListasGenericasComoNulas(cteCopia.EnderecoExpedidor);
            }

            if (cargaCTeOriginal.CTe.EnderecoRecebedor != null)
            {
                cteCopia.EnderecoRecebedor = new Dominio.Entidades.EnderecoParticipanteCTe();
                cargaCTeOriginal.CTe.EnderecoRecebedor.CopyProperties(cteCopia.EnderecoRecebedor);
                cteCopia.EnderecoRecebedor.Codigo = 0;
                Utilidades.Object.DefinirListasGenericasComoNulas(cteCopia.EnderecoRecebedor);
            }

            if (cargaCTeOriginal.CTe.EnderecoDestinatario != null)
            {
                cteCopia.EnderecoDestinatario = new Dominio.Entidades.EnderecoParticipanteCTe();
                cargaCTeOriginal.CTe.EnderecoDestinatario.CopyProperties(cteCopia.EnderecoDestinatario);
                cteCopia.EnderecoDestinatario.Codigo = 0;
                Utilidades.Object.DefinirListasGenericasComoNulas(cteCopia.EnderecoDestinatario);
            }

            if (cargaCTeOriginal.CTe.EnderecoTomador != null)
            {
                cteCopia.EnderecoTomador = new Dominio.Entidades.EnderecoParticipanteCTe();
                cargaCTeOriginal.CTe.EnderecoTomador.CopyProperties(cteCopia.EnderecoTomador);
                cteCopia.EnderecoTomador.Codigo = 0;
                Utilidades.Object.DefinirListasGenericasComoNulas(cteCopia.EnderecoTomador);
            }

            cteCopia.TomadorPagador = cteCopia.Tomador;

            unitOfWork.Start();

            cteCopia.Numero = serCTE.ObterProximoNumero(cteCopia, repCTe);
            cteCopia.NumeroControle = serCTE.RetornarNumeroControleCTe(out int numeroSequencia, cteCopia.NumeroBooking, cteCopia.DescricaoCarrier, cteCopia.TipoPropostaFeeder, cteCopia.TipoModal, cteCopia.TipoServico, 0, cteCopia.SVMTerceiro, unitOfWork);
            cteCopia.PedidoViagemNavioSchedule = repPedidoNavioSchedule.BuscarPorCodigo(repCTe.BuscarCodigoSchedule(cteCopia.ViagemPassagemCinco?.Codigo ?? 0, cteCopia.ViagemPassagemQuatro?.Codigo ?? 0, cteCopia.ViagemPassagemTres?.Codigo ?? 0, cteCopia.ViagemPassagemDois?.Codigo ?? 0, cteCopia.ViagemPassagemUm?.Codigo ?? 0, cteCopia.Viagem?.Codigo ?? 0, cteCopia.PortoDestino?.Codigo ?? 0, cteCopia.TerminalDestino?.Codigo ?? 0));
            cteCopia.SequenciaBooking = numeroSequencia;
            if (cteCopia.NumeroControle == "0" || string.IsNullOrWhiteSpace(cteCopia.NumeroControle))
                cteCopia.NumeroControle = cteCopia.Codigo.ToString("D");
            else
            {
                if (repCTe.ContemNumeroControleDuplicado(cteCopia.NumeroControle, cteCopia.Empresa?.Codigo ?? 0, cteCopia.Codigo))
                {
                    cteCopia.NumeroControle = serCTE.RetornarNumeroControleCTe(out numeroSequencia, cteCopia.NumeroBooking, cteCopia.DescricaoCarrier, cteCopia.TipoPropostaFeeder, cteCopia.TipoModal, cteCopia.TipoServico, cteCopia.Codigo, cteCopia.SVMTerceiro, unitOfWork, true, cteCopia.SequenciaBooking);
                    cteCopia.SequenciaBooking = numeroSequencia;

                    if (repCTe.ContemNumeroControleDuplicado(cteCopia.NumeroControle, cteCopia.Empresa?.Codigo ?? 0, cteCopia.Codigo))
                    {
                        bool contemNumeroDuplicado = true;
                        int count = 0;
                        while (contemNumeroDuplicado)
                        {
                            cteCopia.NumeroControle = serCTE.RetornarNumeroControleCTe(out numeroSequencia, cteCopia.NumeroBooking, cteCopia.DescricaoCarrier, cteCopia.TipoPropostaFeeder, cteCopia.TipoModal, cteCopia.TipoServico, cteCopia.Codigo, cteCopia.SVMTerceiro, unitOfWork, true, cteCopia.SequenciaBooking);
                            cteCopia.SequenciaBooking = numeroSequencia;
                            contemNumeroDuplicado = repCTe.ContemNumeroControleDuplicado(cteCopia.NumeroControle, cteCopia.Empresa?.Codigo ?? 0, cteCopia.Codigo);
                            count++;
                            if (count > 100)
                                break;
                        }
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(cteCopia.NumeroControle))
            {
                cteCopia.ObservacoesGerais += " Num Ctrl: " + cteCopia.NumeroControle;
                cteCopia.ObservacoesGerais = cteCopia.ObservacoesGerais.Trim();
            }
            if (cteCopia.TipoModal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Aquaviario && cteCopia.PortoOrigem != null && cteCopia.PortoOrigem != null && cteCopia.TomadorPagador != null && cteCopia.TomadorPagador.GrupoPessoas != null)
            {
                if (cteCopia.PortoOrigem.AtivarDespachanteComoConsignatario && cteCopia.PortoDestino.AtivarDespachanteComoConsignatario && cteCopia.TomadorPagador != null && cteCopia.TomadorPagador.GrupoPessoas != null && cteCopia.TomadorPagador.GrupoPessoas.AdicionarDespachanteComoConsignatario && cteCopia.TomadorPagador.GrupoPessoas.Despachante != null)
                {
                    cteCopia.ObservacoesGerais += " A " + cteCopia.TomadorPagador.GrupoPessoas.Despachante.Nome + ", CNPJ " + cteCopia.TomadorPagador.GrupoPessoas.Despachante.CPF_CNPJ_Formatado + ", irá atuar como despachante para a liberação dessa carga no porto de descarga.";
                    cteCopia.ObservacoesGerais = cteCopia.ObservacoesGerais.Trim();
                }
            }

            repCTe.Inserir(cteCopia, auditado);

            DuplicarInformacoesContainer(cargaCTeOriginal.CTe, cteCopia, unitOfWork);
            DuplicarInformacoesQuantidadeCarga(cargaCTeOriginal.CTe, cteCopia, unitOfWork);
            DuplicarSeguros(cargaCTeOriginal.CTe, cteCopia, unitOfWork);
            DuplicarMotoristas(cargaCTeOriginal.CTe, cteCopia, unitOfWork);
            DuplicarVeiculos(cargaCTeOriginal.CTe, cteCopia, unitOfWork);
            DuplicarComponentePrestacao(cargaCTeOriginal.CTe, cteCopia, unitOfWork);
            DuplicarDocumentoDeTransporteAnterior(cargaCTeOriginal.CTe, cteCopia, unitOfWork);
            DuplicarDocumentos(cargaCTeOriginal.CTe, cteCopia, unitOfWork);
            DuplicarProdutoPerigoso(cargaCTeOriginal.CTe, cteCopia, unitOfWork);
            DuplicarObservacoesContribuinte(cargaCTeOriginal.CTe, cteCopia, unitOfWork);
            DuplicarObservacoesFisco(cargaCTeOriginal.CTe, cteCopia, unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTeCopia = new Dominio.Entidades.Embarcador.Cargas.CargaCTe()
            {
                Carga = cargaCTeOriginal.Carga,
                CargaOrigem = cargaCTeOriginal.Carga,
                CTe = cteCopia,
                SistemaEmissor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor.MultiCTe
            };

            repCargaCTe.Inserir(cargaCTeCopia, auditado);

            DuplicarComponentesFreteCargaCTe(cargaCTeOriginal, cargaCTeCopia, unitOfWork);
            DuplicarCargaPedidoXMLNotaFiscalCTe(cargaCTeOriginal, cargaCTeCopia, unitOfWork);

            if (cteCopia != null && cargaCTeOriginal?.CTe != null)
            {
                Dominio.Entidades.Embarcador.CTe.CTeRelacaoDocumento cTeRelacao = new Dominio.Entidades.Embarcador.CTe.CTeRelacaoDocumento()
                {
                    CTeGerado = cteCopia,
                    CTeOriginal = cargaCTeOriginal.CTe,
                    TipoCTeGerado = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCTeGerado.CopiaDesvinculadoCarga
                };
                repCTeRelacaoDocumento.Inserir(cTeRelacao);
            }

            if (desvincularCTeOriginalCarga)
            {
                cargaCTeOriginal.NotasFiscais = null;
                cargaCTeOriginal.Componentes = null;
                cargaCTeOriginal.CIOTs = null;
                Servicos.Auditoria.Auditoria.Auditar(auditado, cargaCTeOriginal.CTe, null, "CT-e removido da Carga devido a geração de uma cópia.", unitOfWork);
                repCargaCTe.Deletar(cargaCTeOriginal);
            }

            unitOfWork.CommitChanges();

            Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);

            if (!svcCTe.Emitir(ref cteCopia, unitOfWork))
            {
                mensagemErro = "O CT-e foi gerado, porém, ocorreram problemas ao enviar para a SEFAZ. Tente reenviar o mesmo manualmente ou contate o suporte técnico.";
                return false;
            }

            return true;
        }

        public void GerarCTeSubstituicao(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTeOriginal, Dominio.Entidades.Embarcador.Cargas.Carga carga, DateTime dataEventoDesacordo, decimal valorCTeSubstituicao, string observacaoCTeSubstituicao, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, double tomadorCTeSubstituto, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, dynamic componentesFrete)
        {
            if (!VerificarSeCTeEstaAptoParaCancelamento(out string mensagemErro, cargaCTeOriginal, unitOfWork, false, true))
                throw new ServicoException(mensagemErro);

            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.CTe.ControleGeracaoCTeAnulacao repControleGeracaoCTeAnulacao = new Repositorio.Embarcador.CTe.ControleGeracaoCTeAnulacao(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            unitOfWork.Start();

            Dominio.Entidades.Embarcador.CTe.ControleGeracaoCTeAnulacao controleGeracaoCTeAnulacao = new Dominio.Entidades.Embarcador.CTe.ControleGeracaoCTeAnulacao()
            {
                DataEventoDesacordo = dataEventoDesacordo,
                CargaCTeOriginal = cargaCTeOriginal,
                ObservacaoSubstituicao = observacaoCTeSubstituicao,
                ValorCTeSubstituicao = valorCTeSubstituicao,
                Situacao = SituacaoControleGeracaoCTeAnulacao.AguardandoAutorizacaoAnulacao,
                OutroTomador = tomadorCTeSubstituto > 0 ? repCliente.BuscarPorCPFCNPJ(tomadorCTeSubstituto) : null
            };

            repControleGeracaoCTeAnulacao.Inserir(controleGeracaoCTeAnulacao, auditado);

            SalvarComponentesControleGeracaoCTeAnulacao(controleGeracaoCTeAnulacao, componentesFrete, unitOfWork);

            if (configuracaoEmbarcador.DeixarCargaPendenteDeIntegracaoAposCTeManual)
            {
                carga.CargaIntegradaEmbarcador = false;
                repCarga.Atualizar(carga);
                Servicos.Auditoria.Auditoria.Auditar(auditado, carga, "Alterou a carga para pendente de integração devido a substituição do CT-e.", unitOfWork);
            }

            unitOfWork.CommitChanges();
        }

        public static void GerarCTeSubstituicao(Dominio.Entidades.Embarcador.CTe.ControleGeracaoCTeAnulacao controleGeracaoCTeAnulacao, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);

            Servicos.CTe serCTE = new Servicos.CTe(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.CTe.ControleGeracaoCTeAnulacao repControleGeracaoCTeAnulacao = new Repositorio.Embarcador.CTe.ControleGeracaoCTeAnulacao(unitOfWork);
            Repositorio.DocumentosAnulacaoCTE repDocumentoAnulacaoCTe = new Repositorio.DocumentosAnulacaoCTE(unitOfWork);
            Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            Repositorio.Embarcador.CTe.CTeRelacaoDocumento repCTeRelacaoDocumento = new Repositorio.Embarcador.CTe.CTeRelacaoDocumento(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfig = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia repConfiguracaoOcorrencia = new Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repositorioConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoViagemNavioSchedule repPedidoNavioSchedule = new Repositorio.Embarcador.Pedidos.PedidoViagemNavioSchedule(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe> cargaPedidoXMLNotaFiscalCTes = repCargaPedidoXMLNotaFiscalCTe.BuscarTodosCargaPedidoXMLNotaFiscalCTePorCargaCTe(controleGeracaoCTeAnulacao.CargaCTeOriginal.Codigo);
            Dominio.Entidades.ConhecimentoDeTransporteEletronico cteSubstituicao = new Dominio.Entidades.ConhecimentoDeTransporteEletronico();
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configGeral = repConfig.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoOcorrencia configuracaoOcorrencia = repConfiguracaoOcorrencia.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCte = repCargaCTe.BuscarTodosPorCTe(controleGeracaoCTeAnulacao.CargaCTeOriginal?.CTe?.Codigo ?? 0).FirstOrDefault();
            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = cargaCte?.Carga?.TipoOperacao ?? null;
            Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoPagamentos configuracaoTipoOperacaoPagamentos = tipoOperacao?.ConfiguracaoPagamentos ?? null;
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repositorioConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();

            controleGeracaoCTeAnulacao.CargaCTeOriginal.CTe.CopyProperties(cteSubstituicao);

            cteSubstituicao.Codigo = 0;

            Utilidades.Object.DefinirListasGenericasComoNulas(cteSubstituicao);

            cteSubstituicao.Chave = "";
            cteSubstituicao.NumeroRecibo = "";
            cteSubstituicao.DataAutorizacao = null;
            cteSubstituicao.DataIntegracao = null;
            cteSubstituicao.DataRetornoSefaz = null;
            cteSubstituicao.DataEmissao = DateTime.Now;
            cteSubstituicao.ChaveCTESubComp = controleGeracaoCTeAnulacao.CargaCTeOriginal.CTe.Chave;
            cteSubstituicao.ObservacoesGerais = ObterObservacoesGerais(controleGeracaoCTeAnulacao, configuracaoOcorrencia);
            cteSubstituicao.TipoCTE = controleGeracaoCTeAnulacao.CargaCTeOriginal.CTe.TipoCTE == Dominio.Enumeradores.TipoCTE.Simplificado ? Dominio.Enumeradores.TipoCTE.SimplificadoSubstituto : Dominio.Enumeradores.TipoCTE.Substituto;
            cteSubstituicao.Log = "";
            cteSubstituicao.LogIntegracao = "";
            cteSubstituicao.MensagemStatus = null;
            cteSubstituicao.Status = "P";
            cteSubstituicao.CodigoCTeIntegrador = 0;
            cteSubstituicao.ValorFrete = controleGeracaoCTeAnulacao.ValorCTeSubstituicao;
            cteSubstituicao.ValorAReceber = controleGeracaoCTeAnulacao.ValorCTeSubstituicao;
            cteSubstituicao.ValorPrestacaoServico = controleGeracaoCTeAnulacao.ValorCTeSubstituicao;
            cteSubstituicao.CentroResultado = (configuracaoFinanceiro?.EfetuarVinculoCentroResultadoCTeSubstituto ?? false) ? configuracaoTipoOperacaoPagamentos.CentroResultado : null;

            if (controleGeracaoCTeAnulacao.CargaCTeOriginal.CTe.Remetente != null)
            {
                cteSubstituicao.Remetente = new Dominio.Entidades.ParticipanteCTe();

                controleGeracaoCTeAnulacao.CargaCTeOriginal.CTe.Remetente.CopyProperties(cteSubstituicao.Remetente);

                cteSubstituicao.Remetente.Codigo = 0;

                Utilidades.Object.DefinirListasGenericasComoNulas(cteSubstituicao.Remetente);
            }

            if (controleGeracaoCTeAnulacao.CargaCTeOriginal.CTe.Expedidor != null)
            {
                cteSubstituicao.Expedidor = new Dominio.Entidades.ParticipanteCTe();

                controleGeracaoCTeAnulacao.CargaCTeOriginal.CTe.Expedidor.CopyProperties(cteSubstituicao.Expedidor);

                cteSubstituicao.Expedidor.Codigo = 0;

                Utilidades.Object.DefinirListasGenericasComoNulas(cteSubstituicao.Expedidor);
            }

            if (controleGeracaoCTeAnulacao.CargaCTeOriginal.CTe.Recebedor != null)
            {
                cteSubstituicao.Recebedor = new Dominio.Entidades.ParticipanteCTe();

                controleGeracaoCTeAnulacao.CargaCTeOriginal.CTe.Recebedor.CopyProperties(cteSubstituicao.Recebedor);

                cteSubstituicao.Recebedor.Codigo = 0;

                Utilidades.Object.DefinirListasGenericasComoNulas(cteSubstituicao.Recebedor);
            }

            if (controleGeracaoCTeAnulacao.CargaCTeOriginal.CTe.Destinatario != null)
            {
                cteSubstituicao.Destinatario = new Dominio.Entidades.ParticipanteCTe();

                controleGeracaoCTeAnulacao.CargaCTeOriginal.CTe.Destinatario.CopyProperties(cteSubstituicao.Destinatario);

                cteSubstituicao.Destinatario.Codigo = 0;

                Utilidades.Object.DefinirListasGenericasComoNulas(cteSubstituicao.Destinatario);
            }

            if (controleGeracaoCTeAnulacao.OutroTomador != null && controleGeracaoCTeAnulacao.CargaCTeOriginal.CTe.TomadorPagador.CPF_CNPJ != controleGeracaoCTeAnulacao.OutroTomador.CPF_CNPJ_SemFormato)
            {
                cteSubstituicao.SubstituicaoTomador = true;

                if (controleGeracaoCTeAnulacao.CargaCTeOriginal.CTe.Remetente != null && controleGeracaoCTeAnulacao.OutroTomador.CPF_CNPJ_SemFormato == controleGeracaoCTeAnulacao.CargaCTeOriginal.CTe.Remetente.CPF_CNPJ)
                    cteSubstituicao.TipoTomador = Dominio.Enumeradores.TipoTomador.Remetente;
                else if (controleGeracaoCTeAnulacao.CargaCTeOriginal.CTe.Expedidor != null && controleGeracaoCTeAnulacao.OutroTomador.CPF_CNPJ_SemFormato == controleGeracaoCTeAnulacao.CargaCTeOriginal.CTe.Expedidor.CPF_CNPJ)
                    cteSubstituicao.TipoTomador = Dominio.Enumeradores.TipoTomador.Expedidor;
                else if (controleGeracaoCTeAnulacao.CargaCTeOriginal.CTe.Recebedor != null && controleGeracaoCTeAnulacao.OutroTomador.CPF_CNPJ_SemFormato == controleGeracaoCTeAnulacao.CargaCTeOriginal.CTe.Recebedor.CPF_CNPJ)
                    cteSubstituicao.TipoTomador = Dominio.Enumeradores.TipoTomador.Recebedor;
                else if (controleGeracaoCTeAnulacao.CargaCTeOriginal.CTe.Destinatario != null && controleGeracaoCTeAnulacao.OutroTomador.CPF_CNPJ_SemFormato == controleGeracaoCTeAnulacao.CargaCTeOriginal.CTe.Destinatario.CPF_CNPJ)
                    cteSubstituicao.TipoTomador = Dominio.Enumeradores.TipoTomador.Destinatario;
                else
                {
                    cteSubstituicao.SetarParticipante(controleGeracaoCTeAnulacao.OutroTomador, Dominio.Enumeradores.TipoTomador.Outros);
                    cteSubstituicao.TipoTomador = Dominio.Enumeradores.TipoTomador.Outros;
                }
            }
            else if (controleGeracaoCTeAnulacao.CargaCTeOriginal.CTe.OutrosTomador != null)
            {
                cteSubstituicao.OutrosTomador = new Dominio.Entidades.ParticipanteCTe();

                controleGeracaoCTeAnulacao.CargaCTeOriginal.CTe.OutrosTomador.CopyProperties(cteSubstituicao.OutrosTomador);

                cteSubstituicao.OutrosTomador.Codigo = 0;

                Utilidades.Object.DefinirListasGenericasComoNulas(cteSubstituicao.OutrosTomador);
            }

            if (controleGeracaoCTeAnulacao.CargaCTeOriginal.CTe.EnderecoRemetente != null)
            {
                cteSubstituicao.EnderecoRemetente = new Dominio.Entidades.EnderecoParticipanteCTe();

                controleGeracaoCTeAnulacao.CargaCTeOriginal.CTe.EnderecoRemetente.CopyProperties(cteSubstituicao.EnderecoRemetente);

                cteSubstituicao.EnderecoRemetente.Codigo = 0;

                Utilidades.Object.DefinirListasGenericasComoNulas(cteSubstituicao.EnderecoRemetente);
            }

            if (controleGeracaoCTeAnulacao.CargaCTeOriginal.CTe.EnderecoExpedidor != null)
            {
                cteSubstituicao.EnderecoExpedidor = new Dominio.Entidades.EnderecoParticipanteCTe();

                controleGeracaoCTeAnulacao.CargaCTeOriginal.CTe.EnderecoExpedidor.CopyProperties(cteSubstituicao.EnderecoExpedidor);

                cteSubstituicao.EnderecoExpedidor.Codigo = 0;

                Utilidades.Object.DefinirListasGenericasComoNulas(cteSubstituicao.EnderecoExpedidor);
            }

            if (controleGeracaoCTeAnulacao.CargaCTeOriginal.CTe.EnderecoRecebedor != null)
            {
                cteSubstituicao.EnderecoRecebedor = new Dominio.Entidades.EnderecoParticipanteCTe();

                controleGeracaoCTeAnulacao.CargaCTeOriginal.CTe.EnderecoRecebedor.CopyProperties(cteSubstituicao.EnderecoRecebedor);

                cteSubstituicao.EnderecoRecebedor.Codigo = 0;

                Utilidades.Object.DefinirListasGenericasComoNulas(cteSubstituicao.EnderecoRecebedor);
            }

            if (controleGeracaoCTeAnulacao.CargaCTeOriginal.CTe.EnderecoDestinatario != null)
            {
                cteSubstituicao.EnderecoDestinatario = new Dominio.Entidades.EnderecoParticipanteCTe();

                controleGeracaoCTeAnulacao.CargaCTeOriginal.CTe.EnderecoDestinatario.CopyProperties(cteSubstituicao.EnderecoDestinatario);

                cteSubstituicao.EnderecoDestinatario.Codigo = 0;

                Utilidades.Object.DefinirListasGenericasComoNulas(cteSubstituicao.EnderecoDestinatario);
            }

            if (controleGeracaoCTeAnulacao.CargaCTeOriginal.CTe.EnderecoTomador != null)
            {
                cteSubstituicao.EnderecoTomador = new Dominio.Entidades.EnderecoParticipanteCTe();

                controleGeracaoCTeAnulacao.CargaCTeOriginal.CTe.EnderecoTomador.CopyProperties(cteSubstituicao.EnderecoTomador);

                cteSubstituicao.EnderecoTomador.Codigo = 0;

                Utilidades.Object.DefinirListasGenericasComoNulas(cteSubstituicao.EnderecoTomador);
            }

            cteSubstituicao.TomadorPagador = cteSubstituicao.Tomador;

            if (configGeral.ConsultarRegraICMSGeracaoCTeSubstitutoAutomaticamente || tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                VerificarRegraICMSSubstituicao(cteSubstituicao, controleGeracaoCTeAnulacao.CargaCTeOriginal, unitOfWork, tipoServicoMultisoftware);

            unitOfWork.Start();

            cteSubstituicao.NumeroControle = serCTE.RetornarNumeroControleCTe(out int numeroSequencia, cteSubstituicao.NumeroBooking, cteSubstituicao.DescricaoCarrier, cteSubstituicao.TipoPropostaFeeder, cteSubstituicao.TipoModal, cteSubstituicao.TipoServico, 0, cteSubstituicao.SVMTerceiro, unitOfWork);
            cteSubstituicao.PedidoViagemNavioSchedule = repPedidoNavioSchedule.BuscarPorCodigo(repCTe.BuscarCodigoSchedule(cteSubstituicao.ViagemPassagemCinco?.Codigo ?? 0, cteSubstituicao.ViagemPassagemQuatro?.Codigo ?? 0, cteSubstituicao.ViagemPassagemTres?.Codigo ?? 0, cteSubstituicao.ViagemPassagemDois?.Codigo ?? 0, cteSubstituicao.ViagemPassagemUm?.Codigo ?? 0, cteSubstituicao.Viagem?.Codigo ?? 0, cteSubstituicao.PortoDestino?.Codigo ?? 0, cteSubstituicao.TerminalDestino?.Codigo ?? 0));
            cteSubstituicao.SequenciaBooking = numeroSequencia;
            if (cteSubstituicao.NumeroControle == "0" || string.IsNullOrWhiteSpace(cteSubstituicao.NumeroControle))
                cteSubstituicao.NumeroControle = cteSubstituicao.Codigo.ToString("D");
            else
            {
                if (repCTe.ContemNumeroControleDuplicado(cteSubstituicao.NumeroControle, cteSubstituicao.Empresa?.Codigo ?? 0, cteSubstituicao.Codigo))
                {
                    cteSubstituicao.NumeroControle = serCTE.RetornarNumeroControleCTe(out numeroSequencia, cteSubstituicao.NumeroBooking, cteSubstituicao.DescricaoCarrier, cteSubstituicao.TipoPropostaFeeder, cteSubstituicao.TipoModal, cteSubstituicao.TipoServico, cteSubstituicao.Codigo, cteSubstituicao.SVMTerceiro, unitOfWork, true, cteSubstituicao.SequenciaBooking);
                    cteSubstituicao.SequenciaBooking = numeroSequencia;

                    if (repCTe.ContemNumeroControleDuplicado(cteSubstituicao.NumeroControle, cteSubstituicao.Empresa?.Codigo ?? 0, cteSubstituicao.Codigo))
                    {
                        bool contemNumeroDuplicado = true;
                        int count = 0;
                        while (contemNumeroDuplicado)
                        {
                            cteSubstituicao.NumeroControle = serCTE.RetornarNumeroControleCTe(out numeroSequencia, cteSubstituicao.NumeroBooking, cteSubstituicao.DescricaoCarrier, cteSubstituicao.TipoPropostaFeeder, cteSubstituicao.TipoModal, cteSubstituicao.TipoServico, cteSubstituicao.Codigo, cteSubstituicao.SVMTerceiro, unitOfWork, true, cteSubstituicao.SequenciaBooking);
                            cteSubstituicao.SequenciaBooking = numeroSequencia;
                            contemNumeroDuplicado = repCTe.ContemNumeroControleDuplicado(cteSubstituicao.NumeroControle, cteSubstituicao.Empresa?.Codigo ?? 0, cteSubstituicao.Codigo);
                            count++;
                            if (count > 100)
                                break;
                        }
                    }
                    //repCTe.Atualizar(cte);
                }
            }

            if (!string.IsNullOrWhiteSpace(cteSubstituicao.NumeroControle))
            {
                cteSubstituicao.ObservacoesGerais += " Num Ctrl: " + cteSubstituicao.NumeroControle;
                cteSubstituicao.ObservacoesGerais = cteSubstituicao.ObservacoesGerais.Trim();
            }
            if (cteSubstituicao.TipoModal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Aquaviario && cteSubstituicao.PortoOrigem != null && cteSubstituicao.PortoOrigem != null && cteSubstituicao.TomadorPagador != null && cteSubstituicao.TomadorPagador.GrupoPessoas != null)
            {
                if (cteSubstituicao.PortoOrigem.AtivarDespachanteComoConsignatario && cteSubstituicao.PortoDestino.AtivarDespachanteComoConsignatario && cteSubstituicao.TomadorPagador != null && cteSubstituicao.TomadorPagador.GrupoPessoas != null && cteSubstituicao.TomadorPagador.GrupoPessoas.AdicionarDespachanteComoConsignatario && cteSubstituicao.TomadorPagador.GrupoPessoas.Despachante != null)
                {
                    cteSubstituicao.ObservacoesGerais += " A " + cteSubstituicao.TomadorPagador.GrupoPessoas.Despachante.Nome + ", CNPJ " + cteSubstituicao.TomadorPagador.GrupoPessoas.Despachante.CPF_CNPJ_Formatado + ", irá atuar como despachante para a liberação dessa carga no porto de descarga.";
                    cteSubstituicao.ObservacoesGerais = cteSubstituicao.ObservacoesGerais.Trim();
                }
            }

            cteSubstituicao.Numero = svcCTe.ObterProximoNumero(cteSubstituicao, repCTe);

            repCTe.Inserir(cteSubstituicao);

            if (cteSubstituicao.NumeroControle == "0" || string.IsNullOrWhiteSpace(cteSubstituicao.NumeroControle))
            {
                cteSubstituicao.NumeroControle = cteSubstituicao.Codigo.ToString("D");
                repCTe.Atualizar(cteSubstituicao);
            }

            DuplicarInformacoesContainer(controleGeracaoCTeAnulacao.CargaCTeOriginal.CTe, cteSubstituicao, unitOfWork);
            DuplicarInformacoesQuantidadeCarga(controleGeracaoCTeAnulacao.CargaCTeOriginal.CTe, cteSubstituicao, unitOfWork);
            DuplicarSeguros(controleGeracaoCTeAnulacao.CargaCTeOriginal.CTe, cteSubstituicao, unitOfWork);
            DuplicarMotoristas(controleGeracaoCTeAnulacao.CargaCTeOriginal.CTe, cteSubstituicao, unitOfWork);
            DuplicarVeiculos(controleGeracaoCTeAnulacao.CargaCTeOriginal.CTe, cteSubstituicao, unitOfWork);

            if (cteSubstituicao.TipoCTE == Dominio.Enumeradores.TipoCTE.SimplificadoSubstituto)
            {
                DuplicarEntregaSimplificado(controleGeracaoCTeAnulacao.CargaCTeOriginal.CTe, cteSubstituicao, unitOfWork);
            }
            else
            {
                DuplicarDocumentoDeTransporteAnterior(controleGeracaoCTeAnulacao.CargaCTeOriginal.CTe, cteSubstituicao, unitOfWork);
                DuplicarDocumentos(controleGeracaoCTeAnulacao.CargaCTeOriginal.CTe, cteSubstituicao, unitOfWork);
            }

            DuplicarProdutoPerigoso(controleGeracaoCTeAnulacao.CargaCTeOriginal.CTe, cteSubstituicao, unitOfWork);
            DuplicarObservacoesContribuinte(controleGeracaoCTeAnulacao.CargaCTeOriginal.CTe, cteSubstituicao, unitOfWork);
            DuplicarObservacoesFisco(controleGeracaoCTeAnulacao.CargaCTeOriginal.CTe, cteSubstituicao, unitOfWork);

            GerarComponentesPrestacaoCTeSubstituicao(controleGeracaoCTeAnulacao, cteSubstituicao, unitOfWork);

            svcCTe.CalcularImpostosCTe(cteSubstituicao, unitOfWork);

            //if (controleGeracaoCTeAnulacao.CargaCTeAnulacao != null)
            //{
            Dominio.Entidades.DocumentosAnulacaoCTE documentoAnulacao = new Dominio.Entidades.DocumentosAnulacaoCTE()
            {
                CTE = cteSubstituicao,
                Chave = controleGeracaoCTeAnulacao.CargaCTeAnulacao?.CTe?.Chave ?? "0".PadLeft(44, '0'),
                ContribuinteICMS = Dominio.Enumeradores.OpcaoSimNao.Nao,
                ModeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorModelo("57")
            };

            repDocumentoAnulacaoCTe.Inserir(documentoAnulacao);
            //}

            Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTeSubstituicao = new Dominio.Entidades.Embarcador.Cargas.CargaCTe()
            {
                Carga = controleGeracaoCTeAnulacao.CargaCTeOriginal.Carga,
                CargaOrigem = controleGeracaoCTeAnulacao.CargaCTeOriginal.Carga,
                CTe = cteSubstituicao,
                SistemaEmissor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor.MultiCTe
            };

            repCargaCTe.Inserir(cargaCTeSubstituicao);

            cteSubstituicao.XMLNotaFiscais = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe cargaPedidoXMLNotaFiscalCTeOriginal in cargaPedidoXMLNotaFiscalCTes)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe cargaPedidoXMLNotaFiscalCTeSubstituicao = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe()
                {
                    CargaCTe = cargaCTeSubstituicao,
                    PedidoXMLNotaFiscal = cargaPedidoXMLNotaFiscalCTeOriginal.PedidoXMLNotaFiscal
                };

                repCargaPedidoXMLNotaFiscalCTe.Inserir(cargaPedidoXMLNotaFiscalCTeSubstituicao);

                cteSubstituicao.XMLNotaFiscais.Add(cargaPedidoXMLNotaFiscalCTeOriginal.PedidoXMLNotaFiscal.XMLNotaFiscal);
            }

            CriarCargaCTeComponentesFrete(cargaCTeSubstituicao, unitOfWork);

            controleGeracaoCTeAnulacao.CargaCTeSubstituicao = cargaCTeSubstituicao;
            controleGeracaoCTeAnulacao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoControleGeracaoCTeAnulacao.AguardandoAutorizacaoSubstituicao;

            repCTe.Atualizar(cteSubstituicao);
            repControleGeracaoCTeAnulacao.Atualizar(controleGeracaoCTeAnulacao);

            if (cteSubstituicao != null && controleGeracaoCTeAnulacao?.CargaCTeOriginal?.CTe != null)
            {
                Dominio.Entidades.Embarcador.CTe.CTeRelacaoDocumento cTeRelacao = new Dominio.Entidades.Embarcador.CTe.CTeRelacaoDocumento()
                {
                    CTeGerado = cteSubstituicao,
                    CTeOriginal = controleGeracaoCTeAnulacao.CargaCTeOriginal.CTe,
                    TipoCTeGerado = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCTeGerado.Substituicao
                };
                repCTeRelacaoDocumento.Inserir(cTeRelacao);
            }

            unitOfWork.CommitChanges();

            svcCTe.Emitir(ref cteSubstituicao, unitOfWork);
        }

        private static string ObterObservacoesGerais(Dominio.Entidades.Embarcador.CTe.ControleGeracaoCTeAnulacao controleGeracaoCTeAnulacao, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoOcorrencia configuracaoOcorrencia)
        {
            string observacoesGerais = controleGeracaoCTeAnulacao.CargaCTeOriginal.CTe.ObservacoesGerais.ToString();

            if (configuracaoOcorrencia?.GerarObservacaoSubstitutoSomenteNumeroCTeAnterior ?? false)
                observacoesGerais += $". CTe Original: {controleGeracaoCTeAnulacao.CargaCTeOriginal.CTe.NumeroControle}/{controleGeracaoCTeAnulacao.CargaCTeOriginal.CTe.Numero}";
            else
                observacoesGerais += ". Substituição de valor relativo à prestação de serviço de transporte, número do CT-e original: " + controleGeracaoCTeAnulacao.CargaCTeOriginal.CTe.Numero.ToString("D");

            observacoesGerais += controleGeracaoCTeAnulacao.ObservacaoSubstituicao;

            return observacoesGerais;
        }

        private static void CriarCargaCTeComponentesFrete(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ComponentePrestacaoCTE repComponentePrestacaoCTe = new Repositorio.ComponentePrestacaoCTE(unitOfWork);
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete repCargaCTeComponenteFrete = new Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete(unitOfWork);

            List<Dominio.Entidades.ComponentePrestacaoCTE> componentesPrestacaoCTe = repComponentePrestacaoCTe.BuscarPorCTe(cargaCTe.CTe.Codigo);

            foreach (Dominio.Entidades.ComponentePrestacaoCTE componentePrestacaoCTe in componentesPrestacaoCTe)
            {
                if (componentePrestacaoCTe.ComponenteFrete == null || componentePrestacaoCTe.Nome == "VALOR FRETE" || componentePrestacaoCTe.Nome == "FRETE VALOR")
                    continue;

                Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete cargaCTeComponenteFrete = new Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete
                {
                    ComponenteFrete = componentePrestacaoCTe.ComponenteFrete,
                    TipoComponenteFrete = componentePrestacaoCTe.ComponenteFrete.TipoComponenteFrete,
                    ValorComponente = componentePrestacaoCTe.Valor,
                    CargaCTe = cargaCTe,
                    TipoValor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo,
                    IncluirBaseCalculoICMS = componentePrestacaoCTe.IncluiNaBaseDeCalculoDoICMS,
                    AcrescentaValorTotalAReceber = componentePrestacaoCTe.IncluiNoTotalAReceber
                };

                repCargaCTeComponenteFrete.Inserir(cargaCTeComponenteFrete);
            }

            if (cargaCTe.CTe.IncluirICMSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim && cargaCTe.CTe.CST != null && cargaCTe.CTe.CST != "60" && cargaCTe.CTe.ValorICMS > 0m)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete cargaCTeComponentesFreteICMS = new Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete
                {
                    ComponenteFrete = repComponenteFrete.BuscarPorTipoComponenteFrete(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS),
                    TipoComponenteFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS,
                    ValorComponente = cargaCTe.CTe.ValorICMS,
                    CargaCTe = cargaCTe,
                    TipoValor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo
                };

                repCargaCTeComponenteFrete.Inserir(cargaCTeComponentesFreteICMS);
            }
        }

        private static void GerarComponentesPrestacaoCTeSubstituicao(Dominio.Entidades.Embarcador.CTe.ControleGeracaoCTeAnulacao controleGeracaoCTeAnulacao, Dominio.Entidades.ConhecimentoDeTransporteEletronico cteSubstituicao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.CTe.ControleGeracaoCTeAnulacaoComponenteFrete repControleGeracaoCteAnulacaoComponenteFrete = new Repositorio.Embarcador.CTe.ControleGeracaoCTeAnulacaoComponenteFrete(unitOfWork);
            Repositorio.ComponentePrestacaoCTE repComponentePrestacaoCTe = new Repositorio.ComponentePrestacaoCTE(unitOfWork);

            List<Dominio.Entidades.Embarcador.CTe.ControleGeracaoCTeAnulacaoComponenteFrete> controleGeracaoCTeAnulacaoComponenteFretes = repControleGeracaoCteAnulacaoComponenteFrete.BuscarPorControleGeracaoCTeAnulacao(controleGeracaoCTeAnulacao.Codigo);

            foreach (Dominio.Entidades.Embarcador.CTe.ControleGeracaoCTeAnulacaoComponenteFrete controleGeracaoCTeAnulacaoComponenteFrete in controleGeracaoCTeAnulacaoComponenteFretes)
            {
                Dominio.Entidades.ComponentePrestacaoCTE componentePrestacaoCTe = new Dominio.Entidades.ComponentePrestacaoCTE()
                {
                    ComponenteFrete = controleGeracaoCTeAnulacaoComponenteFrete.ComponenteFrete,
                    CTE = cteSubstituicao,
                    IncluiNaBaseDeCalculoDoICMS = controleGeracaoCTeAnulacaoComponenteFrete.IncluirBaseCalculoICMS,
                    IncluiNoTotalAReceber = controleGeracaoCTeAnulacaoComponenteFrete.IncluirTotalReceber,
                    Nome = Utilidades.String.Left((controleGeracaoCTeAnulacaoComponenteFrete.ComponenteFrete.ImprimirOutraDescricaoCTe && !string.IsNullOrWhiteSpace(controleGeracaoCTeAnulacaoComponenteFrete.ComponenteFrete.DescricaoCTe) ? controleGeracaoCTeAnulacaoComponenteFrete.ComponenteFrete.DescricaoCTe : controleGeracaoCTeAnulacaoComponenteFrete.ComponenteFrete.Descricao), 15),
                    Valor = controleGeracaoCTeAnulacaoComponenteFrete.Valor
                };

                repComponentePrestacaoCTe.Inserir(componentePrestacaoCTe);
            }
        }

        public static void FinalizarGeracaoCTeSubstituicao(Dominio.Entidades.Embarcador.CTe.ControleGeracaoCTeAnulacao controleGeracaoCTeAnulacao, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            Servicos.Embarcador.Carga.Documentos svcDocumentos = new Carga.Documentos(unitOfWork);
            Servicos.Embarcador.Carga.RateioCTe serRateioCTe = new Servicos.Embarcador.Carga.RateioCTe(unitOfWork);

            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.CTe.ControleGeracaoCTeAnulacao repControleGeracaoCTeAnulacao = new Repositorio.Embarcador.CTe.ControleGeracaoCTeAnulacao(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPrimeiroPedidoPorCarga(controleGeracaoCTeAnulacao.CargaCTeOriginal.Carga.Codigo);

            try
            {
                unitOfWork.Start();

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = controleGeracaoCTeAnulacao.CargaCTeOriginal.CTe;
                if (!cte.DataAnulacao.HasValue && controleGeracaoCTeAnulacao.CargaCTeSubstituicao != null)
                {
                    cte.DataAnulacao = controleGeracaoCTeAnulacao.CargaCTeSubstituicao.CTe.DataAutorizacao;
                    repCTe.Atualizar(cte);
                }

                controleGeracaoCTeAnulacao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoControleGeracaoCTeAnulacao.Finalizado;

                repControleGeracaoCTeAnulacao.Atualizar(controleGeracaoCTeAnulacao);

                if (controleGeracaoCTeAnulacao.CargaCTeSubstituicao != null && !controleGeracaoCTeAnulacao.CargaCTeSubstituicao.GerouMovimentacaoAutorizacao)
                {
                    svcDocumentos.GerarMovimentoEmissaoCTe(controleGeracaoCTeAnulacao.CargaCTeSubstituicao, tipoServicoMultisoftware, unitOfWork, false);

                    controleGeracaoCTeAnulacao.CargaCTeSubstituicao.GerouMovimentacaoAutorizacao = true;

                    repCargaCTe.Atualizar(controleGeracaoCTeAnulacao.CargaCTeSubstituicao);
                }

                if (controleGeracaoCTeAnulacao.CargaCTeSubstituicao != null && !controleGeracaoCTeAnulacao.CargaCTeSubstituicao.GerouControleFaturamento)
                {
                    Servicos.Log.GravarInfo($"FinalizarGeracaoCTeSubstituicao inserindo documento faturamento - Carga {controleGeracaoCTeAnulacao?.CargaCTeSubstituicao?.CargaOrigem?.Codigo ?? 0} -  CTe {controleGeracaoCTeAnulacao?.CargaCTeSubstituicao?.CTe?.Codigo ?? 0}", "ControleDocumentoFaturamento");
                    Servicos.Embarcador.Fatura.FaturamentoDocumento.GerarControleFaturamentoPorDocumento(controleGeracaoCTeAnulacao.CargaCTeSubstituicao.CargaOrigem, controleGeracaoCTeAnulacao.CargaCTeSubstituicao.CTe, null, null, null, null, false, false, false, configuracao, unitOfWork, tipoServicoMultisoftware);

                    controleGeracaoCTeAnulacao.CargaCTeSubstituicao.GerouControleFaturamento = true;

                    repCargaCTe.Atualizar(controleGeracaoCTeAnulacao.CargaCTeSubstituicao);
                }

                Servicos.Embarcador.Escrituracao.DocumentoEscrituracao.AdicionarDocumentoParaEscrituracao(controleGeracaoCTeAnulacao, unitOfWork);

                if (controleGeracaoCTeAnulacao.CargaCTeSubstituicao != null)
                    repCargaCTe.Atualizar(controleGeracaoCTeAnulacao.CargaCTeSubstituicao);

                if (!repControleGeracaoCTeAnulacao.ExistePendenteGeracaoPorCarga(controleGeracaoCTeAnulacao.Codigo, controleGeracaoCTeAnulacao.CargaCTeOriginal.Carga.Codigo))
                {
                    List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repCargaCTe.BuscarPorCargaAutorizadosSemAnulacao(controleGeracaoCTeAnulacao.CargaCTeOriginal.Carga.Codigo);

                    serRateioCTe.AjustarFretePorCTes(cargaPedido, cargaCTes, tipoServicoMultisoftware, unitOfWork);

                    if (configuracao.DeixarCargaPendenteDeIntegracaoAposCTeManual && controleGeracaoCTeAnulacao.CargaCTeOriginal.Carga != null && controleGeracaoCTeAnulacao.CargaCTeOriginal.Carga.Codigo > 0)
                    {
                        Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(controleGeracaoCTeAnulacao.CargaCTeOriginal.Carga.Codigo);
                        if (carga != null)
                        {
                            carga.CargaIntegradaEmbarcador = false;
                            repCarga.Atualizar(carga);
                        }
                    }
                }

                unitOfWork.CommitChanges();
            }
            catch
            {
                unitOfWork.Rollback();
                throw;
            }
        }

        public static void SetarParticipantesCTe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cteAntigo, Dominio.Entidades.ConhecimentoDeTransporteEletronico cteNovo)
        {
            if (cteAntigo.Remetente != null)
            {
                cteNovo.Remetente = new Dominio.Entidades.ParticipanteCTe();

                cteAntigo.Remetente.CopyProperties(cteNovo.Remetente);

                cteNovo.Remetente.Codigo = 0;

                Utilidades.Object.DefinirListasGenericasComoNulas(cteNovo.Remetente);
            }

            if (cteAntigo.Expedidor != null)
            {
                cteNovo.Expedidor = new Dominio.Entidades.ParticipanteCTe();

                cteAntigo.Expedidor.CopyProperties(cteNovo.Expedidor);

                cteNovo.Expedidor.Codigo = 0;

                Utilidades.Object.DefinirListasGenericasComoNulas(cteNovo.Expedidor);
            }

            if (cteAntigo.Recebedor != null)
            {
                cteNovo.Recebedor = new Dominio.Entidades.ParticipanteCTe();

                cteAntigo.Recebedor.CopyProperties(cteNovo.Recebedor);

                cteNovo.Recebedor.Codigo = 0;

                Utilidades.Object.DefinirListasGenericasComoNulas(cteNovo.Recebedor);
            }

            if (cteAntigo.Destinatario != null)
            {
                cteNovo.Destinatario = new Dominio.Entidades.ParticipanteCTe();

                cteAntigo.Destinatario.CopyProperties(cteNovo.Destinatario);

                cteNovo.Destinatario.Codigo = 0;

                Utilidades.Object.DefinirListasGenericasComoNulas(cteNovo.Destinatario);
            }

            if (cteAntigo.OutrosTomador != null)
            {
                cteNovo.OutrosTomador = new Dominio.Entidades.ParticipanteCTe();

                cteAntigo.OutrosTomador.CopyProperties(cteNovo.OutrosTomador);

                cteNovo.OutrosTomador.Codigo = 0;

                Utilidades.Object.DefinirListasGenericasComoNulas(cteNovo.OutrosTomador);
            }

            if (cteAntigo.EnderecoRemetente != null)
            {
                cteNovo.EnderecoRemetente = new Dominio.Entidades.EnderecoParticipanteCTe();

                cteAntigo.EnderecoRemetente.CopyProperties(cteNovo.EnderecoRemetente);

                cteNovo.EnderecoRemetente.Codigo = 0;

                Utilidades.Object.DefinirListasGenericasComoNulas(cteNovo.EnderecoRemetente);
            }

            if (cteAntigo.EnderecoExpedidor != null)
            {
                cteNovo.EnderecoExpedidor = new Dominio.Entidades.EnderecoParticipanteCTe();

                cteAntigo.EnderecoExpedidor.CopyProperties(cteNovo.EnderecoExpedidor);

                cteNovo.EnderecoExpedidor.Codigo = 0;

                Utilidades.Object.DefinirListasGenericasComoNulas(cteNovo.EnderecoExpedidor);
            }

            if (cteAntigo.EnderecoRecebedor != null)
            {
                cteNovo.EnderecoRecebedor = new Dominio.Entidades.EnderecoParticipanteCTe();

                cteAntigo.EnderecoRecebedor.CopyProperties(cteNovo.EnderecoRecebedor);

                cteNovo.EnderecoRecebedor.Codigo = 0;

                Utilidades.Object.DefinirListasGenericasComoNulas(cteNovo.EnderecoRecebedor);
            }

            if (cteAntigo.EnderecoDestinatario != null)
            {
                cteNovo.EnderecoDestinatario = new Dominio.Entidades.EnderecoParticipanteCTe();

                cteAntigo.EnderecoDestinatario.CopyProperties(cteNovo.EnderecoDestinatario);

                cteNovo.EnderecoDestinatario.Codigo = 0;

                Utilidades.Object.DefinirListasGenericasComoNulas(cteNovo.EnderecoDestinatario);
            }

            if (cteAntigo.EnderecoTomador != null)
            {
                cteNovo.EnderecoTomador = new Dominio.Entidades.EnderecoParticipanteCTe();

                cteAntigo.EnderecoTomador.CopyProperties(cteNovo.EnderecoTomador);

                cteNovo.EnderecoTomador.Codigo = 0;

                Utilidades.Object.DefinirListasGenericasComoNulas(cteNovo.EnderecoTomador);
            }
        }

        public static void GerarMovimentacaoCTesAnulados(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            try
            {
                Repositorio.Embarcador.CTe.ControleGeracaoCTeAnulacao repControleGeracaoCTeAnulacao = new Repositorio.Embarcador.CTe.ControleGeracaoCTeAnulacao(unitOfWork);

                List<Dominio.Entidades.Embarcador.CTe.ControleGeracaoCTeAnulacao> anulacoesAutorizadas = repControleGeracaoCTeAnulacao.BuscarPorSituacaoEStatusCTeAnulacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoControleGeracaoCTeAnulacao.AguardandoAutorizacaoAnulacao, "A", 3);

                foreach (Dominio.Entidades.Embarcador.CTe.ControleGeracaoCTeAnulacao controleGeracaoCTeAnulacao in anulacoesAutorizadas)
                    AnularCTeEGerarMovimentacao(controleGeracaoCTeAnulacao, unitOfWork, tipoServicoMultisoftware);

                unitOfWork.FlushAndClear();
            }
            catch
            {
                unitOfWork.Rollback();
                throw;
            }
        }

        public static void GerarSubstituicaoCTesAnulados(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            try
            {
                Repositorio.Embarcador.CTe.ControleGeracaoCTeAnulacao repControleGeracaoCTeAnulacao = new Repositorio.Embarcador.CTe.ControleGeracaoCTeAnulacao(unitOfWork);

                List<Dominio.Entidades.Embarcador.CTe.ControleGeracaoCTeAnulacao> anulacoesAutorizadas = repControleGeracaoCTeAnulacao.BuscarPorSituacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoControleGeracaoCTeAnulacao.GerandoCTeSubstituicao, 3);

                foreach (Dominio.Entidades.Embarcador.CTe.ControleGeracaoCTeAnulacao controleGeracaoCTeAnulacao in anulacoesAutorizadas)
                {
                    if (!controleGeracaoCTeAnulacao.NaoGerarCTeSubstituicao)
                        GerarCTeSubstituicao(controleGeracaoCTeAnulacao, unitOfWork, tipoServicoMultisoftware);
                }

                unitOfWork.FlushAndClear();
            }
            catch
            {
                unitOfWork.Rollback();
                throw;
            }
        }

        public static void FinalizarCTesAnulados(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            try
            {
                Repositorio.Embarcador.CTe.ControleGeracaoCTeAnulacao repControleGeracaoCTeAnulacao = new Repositorio.Embarcador.CTe.ControleGeracaoCTeAnulacao(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();

                List<Dominio.Entidades.Embarcador.CTe.ControleGeracaoCTeAnulacao> anulacoesAutorizadas = repControleGeracaoCTeAnulacao.BuscarPorSituacaoEStatusCTeSubstituicao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoControleGeracaoCTeAnulacao.AguardandoAutorizacaoSubstituicao, "A", 3);

                foreach (Dominio.Entidades.Embarcador.CTe.ControleGeracaoCTeAnulacao controleGeracaoCTeAnulacao in anulacoesAutorizadas)
                    FinalizarGeracaoCTeSubstituicao(controleGeracaoCTeAnulacao, unitOfWork, tipoServicoMultisoftware, configuracao);

                unitOfWork.FlushAndClear();
            }
            catch
            {
                unitOfWork.Rollback();
                throw;
            }
        }

        public static object ObterDadosParticipanteCTe(Dominio.Enumeradores.TipoTomador tipo, Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            Dominio.Entidades.ParticipanteCTe cliente = cte.ObterParticipante(tipo);

            if (cliente == null)
                return null;

            var retorno = new
            {
                CPFCNPJ = cliente.CPF_CNPJ_SemFormato,
                IE = cliente.IE_RG,
                RazaoSocial = cliente.Nome,
                NomeFantasia = cliente.NomeFantasia,
                TelefonePrincipal = cliente.Telefone1,
                TelefoneSecundario = cliente.Telefone2,
                Atividade = new
                {
                    Codigo = cliente.Atividade?.Codigo ?? 0,
                    Descricao = cliente.Atividade?.Descricao
                },
                CEP = cliente.CEP,
                Endereco = cliente.Endereco,
                Numero = cliente.Numero,
                Bairro = cliente.Bairro,
                Complemento = cliente.Complemento,
                Localidade = new
                {
                    Codigo = cliente.Localidade?.Codigo ?? 0,
                    Descricao = cliente.Localidade?.DescricaoCidadeEstado ?? string.Empty,
                    Estado = cliente.Localidade?.Estado?.Sigla ?? string.Empty
                },
                EmailGeral = cliente.Email,
                EnviarXMLEmailGeral = cliente.EmailStatus,
                EmailContato = cliente.EmailContato,
                EnviarXMLEmailContato = cliente.EmailContatoStatus,
                EmailContador = cliente.EmailContador,
                EnviarXMLEmailContador = cliente.EmailContadorStatus,
                SalvarEndereco = cliente.Localidade?.Estado?.Sigla == "EX" ? false : (cliente.Cliente?.NaoAtualizarDados ?? true) ? false : cliente.SalvarEndereco,
                ParticipanteExterior = cliente.Exterior,
                LocalidadeExterior = cliente.Cidade,
                PessoaExterior = new
                {
                    Codigo = cliente.Exterior ? cliente.Cliente?.CPF_CNPJ ?? 0 : 0,
                    Descricao = cliente.Exterior ? cliente.Cliente?.CPF_CNPJ_Formatado ?? string.Empty : string.Empty
                },
                Pais = new
                {
                    Codigo = cliente.Pais?.Codigo ?? 0,
                    Descricao = cliente.Pais?.Nome
                }
            };

            return retorno;
        }

        public static string ObterNomeArquivoDownloadCTe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, string extensao)
        {
            string nomeArquivo = cte?.TomadorPagador?.Cliente?.NomeNomenclaturaArquivosDownloadCTe;

            if (string.IsNullOrWhiteSpace(nomeArquivo))
                nomeArquivo = cte?.TomadorPagador?.GrupoPessoas?.NomeNomenclaturaArquivosDownloadCTe;

            if (!string.IsNullOrWhiteSpace(nomeArquivo))
            {
                nomeArquivo = nomeArquivo.Replace("#NumeroCTe", cte.Numero.ToString())
                                         .Replace("#SerieCTe", cte.Serie?.Numero.ToString() ?? "")
                                         .Replace("#CNPJEmissor", cte.Empresa?.CNPJ_SemFormato ?? "")
                                         .Replace("#CNPJTomador", cte.TomadorPagador?.CPF_CNPJ_SemFormato ?? "")
                                         .Replace("#NumeroBooking", cte.NumeroBooking ?? "")
                                         .Replace("#ChaveCTe", cte.Chave ?? "");

                if (string.IsNullOrWhiteSpace(nomeArquivo))
                    nomeArquivo = cte.Numero.ToString();

                nomeArquivo += "." + extensao;
            }

            if (!string.IsNullOrWhiteSpace(nomeArquivo) && cte?.Status != "A")
                nomeArquivo = "Cancelado_" + nomeArquivo;

            return nomeArquivo;
        }

        public void IntegrarCTeAnteriorEMPAsync(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Servicos.WebService.CTe.CTe serCTe = new Servicos.WebService.CTe.CTe(unitOfWork);

            Repositorio.Embarcador.Integracao.IntegracaoEMPLog repIntegracaoEMPLog = new Repositorio.Embarcador.Integracao.IntegracaoEMPLog(unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoEMP repConfiguracaoIntegracaoEMP = new Repositorio.Embarcador.Configuracoes.IntegracaoEMP(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP configuracaoIntegracaoEMP = repConfiguracaoIntegracaoEMP.Buscar();

            if (configuracaoIntegracaoEMP == null || !configuracaoIntegracaoEMP.AtivarEnvioCTesAnterioresEMP)
                return;

            try
            {
                int limite = 5;
                int inicio = 0;

                DateTime dataFinal = DateTime.Now.Date.AddDays(-1);
                DateTime dataInicial = DateTime.Now.Date.AddDays(-1);

                int totalRegistros = repCTe.ContarConsultaCTesPorPeriodo(dataInicial, dataFinal, true, string.Empty);
                if (totalRegistros == 0)
                {
                    Dominio.Entidades.Embarcador.Integracao.IntegracaoEMPLog integracaoEMPLog = new Dominio.Entidades.Embarcador.Integracao.IntegracaoEMPLog()
                    {
                        ArquivoEnvio = "",
                        ArquivoRetorno = "",
                        DataEnvio = DateTime.Now,
                        MensageRetorno = "Não foi localizado nenhum CT-e anterior para realizar a integração",
                        StatusIntegracaoEMP = StatusIntegracaoEMP.NotPersisted,
                        Topic = configuracaoIntegracaoEMP?.TopicCTesAnterioresEMP
                    };
                    repIntegracaoEMPLog.Inserir(integracaoEMPLog);
                    return;
                }
                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTes = repCTe.ConsultarCTesPorPeriodo(dataInicial, dataFinal, true, inicio, limite, string.Empty);

                List<Dominio.ObjetosDeValor.WebService.CTe.CTe> ctesRetornar = new List<Dominio.ObjetosDeValor.WebService.CTe.CTe>();

                foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cte in listaCTes)
                    ctesRetornar.Add(serCTe.ConverterObjetoCTe(cte, new List<Dominio.Entidades.CTeContaContabilContabilizacao>(), Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.Nenhum, unitOfWork, false));

                var config = new ProducerConfig
                {
                    BootstrapServers = configuracaoIntegracaoEMP?.BoostrapServersEMP ?? string.Empty,
                    SaslUsername = configuracaoIntegracaoEMP?.UsuarioEMP ?? string.Empty,
                    SaslPassword = configuracaoIntegracaoEMP?.SenhaEMP ?? string.Empty,
                    SecurityProtocol = SecurityProtocol.SaslSsl,
                    SaslMechanism = SaslMechanism.Plain,
                    Acks = Acks.Leader,
                    SslEndpointIdentificationAlgorithm = SslEndpointIdentificationAlgorithm.Https
                };
                var kafkaMessage = new Message<Null, string>();
                kafkaMessage.Value = JsonConvert.SerializeObject(ctesRetornar);

                if (string.IsNullOrEmpty(config.SaslUsername) || string.IsNullOrEmpty(config.SaslPassword))
                    return;

                using (var producer = new ProducerBuilder<Null, string>(config).Build())
                {
                    //try
                    //{
                    producer.Produce(configuracaoIntegracaoEMP?.TopicCTesAnterioresEMP ?? string.Empty, kafkaMessage, handlerCteAnterior);
                    //var deliveryReport = await producer.ProduceAsync(configuracaoIntegracaoEMP?.TopicCTesAnterioresEMP ?? string.Empty, kafkaMessage);
                    //SalvarLogEMP(deliveryReport);
                    //}
                    //catch (ProduceException<string, string> e)
                    //{
                    //    Servicos.Log.TratarErro($"Failed to deliver message: {e.Error.Reason}");
                    //}
                    producer.Flush(TimeSpan.FromSeconds(60));
                }
            }
            catch (Exception excecao)
            {
                Dominio.Entidades.Embarcador.Integracao.IntegracaoEMPLog integracaoEMPLog = new Dominio.Entidades.Embarcador.Integracao.IntegracaoEMPLog()
                {
                    ArquivoEnvio = "",
                    ArquivoRetorno = "",
                    DataEnvio = DateTime.Now,
                    MensageRetorno = "Falha ao integrar CT-e anterior: " + excecao.Message.Left(1500),
                    StatusIntegracaoEMP = StatusIntegracaoEMP.NotPersisted,
                    Topic = configuracaoIntegracaoEMP?.TopicCTesAnterioresEMP
                };
                repIntegracaoEMPLog.Inserir(integracaoEMPLog);

                Servicos.Log.TratarErro($"Falha ao integrar CT-e anterior: {excecao.Message}");
            }
        }

        public void IntegrarCTesFaturasDiaAnteriorEMPAsync(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Servicos.WebService.CTe.CTe serCTe = new Servicos.WebService.CTe.CTe(unitOfWork);

            Repositorio.Embarcador.Integracao.IntegracaoEMPLog repIntegracaoEMPLog = new Repositorio.Embarcador.Integracao.IntegracaoEMPLog(unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoEMP repConfiguracaoIntegracaoEMP = new Repositorio.Embarcador.Configuracoes.IntegracaoEMP(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEMP configuracaoIntegracaoEMP = repConfiguracaoIntegracaoEMP.Buscar();

            if (configuracaoIntegracaoEMP == null || !configuracaoIntegracaoEMP.AtivarEnvioCTesAnterioresEMP)
                return;

            try
            {
                int limite = 5;
                int inicio = 0;

                DateTime dataFinal = DateTime.Now.Date.AddDays(-1);
                DateTime dataInicial = DateTime.Now.Date.AddDays(-1);

                int totalRegistros = repCTe.ContarConsultaCTesPorPeriodo(dataInicial, dataFinal, true, string.Empty);
                if (totalRegistros == 0)
                {
                    Dominio.Entidades.Embarcador.Integracao.IntegracaoEMPLog integracaoEMPLog = new Dominio.Entidades.Embarcador.Integracao.IntegracaoEMPLog()
                    {
                        ArquivoEnvio = "",
                        ArquivoRetorno = "",
                        DataEnvio = DateTime.Now,
                        MensageRetorno = "Não foi localizado nenhum CT-e Faturado anterior para realizar a integração",
                        StatusIntegracaoEMP = StatusIntegracaoEMP.NotPersisted,
                        Topic = configuracaoIntegracaoEMP?.TopicBuscarFaturaCTeEMP
                    };
                    repIntegracaoEMPLog.Inserir(integracaoEMPLog);
                    return;
                }
                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTes = repCTe.ConsultarCTesPorPeriodo(dataInicial, dataFinal, true, inicio, limite, string.Empty);

                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargasCtes = listaCTes.SelectMany(o => o.CargaCTes).ToList();

                List<Dominio.ObjetosDeValor.WebService.CTe.CTeFatura> ctesEnvio = new List<Dominio.ObjetosDeValor.WebService.CTe.CTeFatura>();

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in cargasCtes)
                {
                    ctesEnvio.Add(serCTe.ConverterObjetoCargaCTeFatura(cargaCTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.Nenhum, unitOfWork, false, null));
                }
                ;

                var config = new ProducerConfig
                {
                    BootstrapServers = configuracaoIntegracaoEMP?.BoostrapServersEMP ?? string.Empty,
                    SaslUsername = configuracaoIntegracaoEMP?.UsuarioEMP ?? string.Empty,
                    SaslPassword = configuracaoIntegracaoEMP?.SenhaEMP ?? string.Empty,
                    SecurityProtocol = SecurityProtocol.SaslSsl,
                    SaslMechanism = SaslMechanism.Plain,
                    Acks = Acks.Leader,
                    SslEndpointIdentificationAlgorithm = SslEndpointIdentificationAlgorithm.Https
                };
                var kafkaMessage = new Message<Null, string>();
                kafkaMessage.Value = JsonConvert.SerializeObject(ctesEnvio);

                if (string.IsNullOrEmpty(config.SaslUsername) || string.IsNullOrEmpty(config.SaslPassword))
                    return;

                using (var producer = new ProducerBuilder<Null, string>(config).Build())
                {
                    //try
                    // {
                    producer.Produce(configuracaoIntegracaoEMP?.TopicBuscarFaturaCTeEMP ?? string.Empty, kafkaMessage, handler);
                    //}
                    //catch (ProduceException<string, string> e)
                    //{
                    //   Servicos.Log.TratarErro($"Failed to deliver message: {e.Error.Reason}");
                    //}
                    //producer.Flush(TimeSpan.FromSeconds(60));
                }
            }
            catch (Exception excecao)
            {
                Dominio.Entidades.Embarcador.Integracao.IntegracaoEMPLog integracaoEMPLog = new Dominio.Entidades.Embarcador.Integracao.IntegracaoEMPLog()
                {
                    ArquivoEnvio = "",
                    ArquivoRetorno = "",
                    DataEnvio = DateTime.Now,
                    MensageRetorno = "Falha ao integrar Faturas dos CT-e anterior: " + excecao.Message.Left(1500),
                    StatusIntegracaoEMP = StatusIntegracaoEMP.NotPersisted,
                    Topic = configuracaoIntegracaoEMP?.TopicBuscarFaturaCTeEMP
                };
                repIntegracaoEMPLog.Inserir(integracaoEMPLog);

                Servicos.Log.TratarErro($"Falha ao integrar Faturas dos CT-e anterior: {excecao.Message}");
            }
        }

        public void GerarGuiasTributacaoEstadual(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.ConhecimentoDeTransporteEletronico conhecimentoDeTransporteEletronico)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao integracaoUnilever = repositorioTipoIntegracao.BuscarPorTipo(TipoIntegracao.Unilever);

            bool gerarRegistrosReceberGNREParaCTesComCST90 = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unitOfWork).BuscarConfiguracaoPadrao().GerarRegistrosReceberGNREParaCTesComCST90;

            if (integracaoUnilever == null && !gerarRegistrosReceberGNREParaCTesComCST90)
                return;

            //Depois sera visto este comportamento porque a tarefa  #59072 não foi desenvolvida

            //SituacaoGuia situacaoGuia = SituacaoGuia.NaoEmitido;

            //if (conhecimentoDeTransporteEletronico != null && !string.IsNullOrWhiteSpace(carga?.Empresa?.NomeCertificado))
            //    situacaoGuia = SituacaoGuia.AguardandoRetorno;
            //else if ((carga?.Empresa?.EmissaoDocumentosForaDoSistema ?? false) || string.IsNullOrWhiteSpace(carga?.Empresa?.NomeCertificado))
            //    situacaoGuia = SituacaoGuia.NaoEmitido;

            Repositorio.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEtadual repositorioGNRE = new Repositorio.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEtadual(unitOfWork);
            Dominio.Entidades.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEstadual guiaNacionalRecolhimentoTributoEstadual = new Dominio.Entidades.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEstadual()
            {
                Carga = carga,
                Cte = conhecimentoDeTransporteEletronico,
                DataEmissao = conhecimentoDeTransporteEletronico?.DataEmissao ?? DateTime.MinValue,
                Valor = conhecimentoDeTransporteEletronico?.ValorICMS ?? 0,
                Situacao = SituacaoGuia.Gerada,
                NroGuia = conhecimentoDeTransporteEletronico?.Numero.ToString() ?? string.Empty,
                ProblemaIntegracao = "",
                DataIntegracao = DateTime.Now,
                SituacaoIntegracao = SituacaoIntegracao.Integrado,
                NumeroTentativas = 0,
                TipoIntegracao = integracaoUnilever ?? repositorioTipoIntegracao.BuscarAtivos().FirstOrDefault(),
            };

            repositorioGNRE.Inserir(guiaNacionalRecolhimentoTributoEstadual);
        }

        #endregion Métodos Públicos

        #region Métodos Privados - Duplicar

        private static void DuplicarInformacoesContainer(Dominio.Entidades.ConhecimentoDeTransporteEletronico cteAntigo, Dominio.Entidades.ConhecimentoDeTransporteEletronico cteNovo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ContainerCTE repContainerCTE = new Repositorio.ContainerCTE(unitOfWork);
            Repositorio.Embarcador.CTe.CTeContainerDocumento repCTeContainerDocumento = new Repositorio.Embarcador.CTe.CTeContainerDocumento(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscalEletronica = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);

            if (cteAntigo.Containers != null && cteAntigo.Containers.Count > 0)
            {
                foreach (var container in cteAntigo.Containers)
                {
                    Dominio.Entidades.ContainerCTE containerCTE = new Dominio.Entidades.ContainerCTE()
                    {
                        Container = container.Container,
                        CTE = cteNovo,
                        DataPrevista = container.DataPrevista,
                        Lacre1 = container.Lacre1,
                        Lacre2 = container.Lacre2,
                        Lacre3 = container.Lacre3,
                        Numero = container.Numero,
                        //PesoBrutoContainer = container.PesoBrutoContainer,
                        //PesoCubadoContainer = container.PesoCubadoContainer
                    };
                    repContainerCTE.Inserir(containerCTE);

                    if (container.Documentos != null && container.Documentos.Count > 0)
                    {
                        foreach (var documento in container.Documentos)
                        {
                            Dominio.Entidades.Embarcador.CTe.CTeContainerDocumento doc = new Dominio.Entidades.Embarcador.CTe.CTeContainerDocumento()
                            {
                                Chave = documento.Chave,
                                ContainerCTE = containerCTE,
                                DocumentosCTE = documento.DocumentosCTE,
                                Numero = documento.Numero,
                                Serie = documento.Serie,
                                TipoDocumento = documento.TipoDocumento,
                                UnidadeMedidaRateada = documento.UnidadeMedidaRateada
                            };

                            if (!string.IsNullOrWhiteSpace(doc.Chave))
                                doc.XMLNotaFiscal = repXMLNotaFiscalEletronica.BuscarPorChave(doc.Chave);

                            repCTeContainerDocumento.Inserir(doc);
                        }
                    }
                }
            }
        }

        private static void DuplicarInformacoesQuantidadeCarga(Dominio.Entidades.ConhecimentoDeTransporteEletronico cteAntigo, Dominio.Entidades.ConhecimentoDeTransporteEletronico cteNovo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.InformacaoCargaCTE repInformacaoCargaCTe = new Repositorio.InformacaoCargaCTE(unitOfWork);

            List<Dominio.Entidades.InformacaoCargaCTE> informacoesCargaCTe = repInformacaoCargaCTe.BuscarPorCTe(cteAntigo.Codigo);

            foreach (Dominio.Entidades.InformacaoCargaCTE informacaoCargaCTEAntiga in informacoesCargaCTe)
            {
                Dominio.Entidades.InformacaoCargaCTE informacaoCargaCTeNova = informacaoCargaCTEAntiga.Clonar();

                Utilidades.Object.DefinirListasGenericasComoNulas(informacaoCargaCTeNova);

                informacaoCargaCTeNova.CTE = cteNovo;

                repInformacaoCargaCTe.Inserir(informacaoCargaCTeNova);
            }
        }

        public static void DuplicarSeguros(Dominio.Entidades.ConhecimentoDeTransporteEletronico cteAntigo, Dominio.Entidades.ConhecimentoDeTransporteEletronico cteNovo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.SeguroCTE repSeguroCTe = new Repositorio.SeguroCTE(unitOfWork);

            List<Dominio.Entidades.SeguroCTE> segurosCTE = repSeguroCTe.BuscarPorCTe(cteAntigo.Codigo);

            foreach (Dominio.Entidades.SeguroCTE seguroCTeAntigo in segurosCTE)
            {
                Dominio.Entidades.SeguroCTE seguroCTeNovo = seguroCTeAntigo.Clonar();

                Utilidades.Object.DefinirListasGenericasComoNulas(seguroCTeNovo);

                seguroCTeNovo.CTE = cteNovo;

                repSeguroCTe.Inserir(seguroCTeNovo);
            }
        }

        private static void DuplicarMotoristas(Dominio.Entidades.ConhecimentoDeTransporteEletronico cteAntigo, Dominio.Entidades.ConhecimentoDeTransporteEletronico cteNovo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.MotoristaCTE repMotoristaCTe = new Repositorio.MotoristaCTE(unitOfWork);

            List<Dominio.Entidades.MotoristaCTE> motoristasCTe = repMotoristaCTe.BuscarPorCTe(cteAntigo.Codigo);

            foreach (Dominio.Entidades.MotoristaCTE motoristaCTeAntigo in motoristasCTe)
            {
                Dominio.Entidades.MotoristaCTE motoristaCTeNovo = motoristaCTeAntigo.Clonar();

                Utilidades.Object.DefinirListasGenericasComoNulas(motoristaCTeNovo);

                motoristaCTeNovo.CTE = cteNovo;

                repMotoristaCTe.Inserir(motoristaCTeNovo);
            }
        }

        private static void DuplicarVeiculos(Dominio.Entidades.ConhecimentoDeTransporteEletronico cteAntigo, Dominio.Entidades.ConhecimentoDeTransporteEletronico cteNovo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.VeiculoCTE repVeiculoCTe = new Repositorio.VeiculoCTE(unitOfWork);

            List<Dominio.Entidades.VeiculoCTE> veiculosCTe = repVeiculoCTe.BuscarPorCTe(cteAntigo.Codigo);

            foreach (Dominio.Entidades.VeiculoCTE veiculoCTeAntigo in veiculosCTe)
            {
                Dominio.Entidades.VeiculoCTE veiculoCTeNovo = new Dominio.Entidades.VeiculoCTE();

                veiculoCTeAntigo.CopyProperties(veiculoCTeNovo);

                veiculoCTeNovo.Codigo = 0;
                veiculoCTeNovo.CTE = cteNovo;

                Utilidades.Object.DefinirListasGenericasComoNulas(veiculoCTeNovo);

                if (veiculoCTeAntigo.Proprietario != null)
                {
                    veiculoCTeNovo.Proprietario = new Dominio.Entidades.ProprietarioVeiculoCTe();

                    veiculoCTeAntigo.Proprietario.CopyProperties(veiculoCTeNovo.Proprietario);

                    veiculoCTeNovo.Proprietario.Codigo = 0;
                }

                repVeiculoCTe.Inserir(veiculoCTeNovo);
            }
        }

        private static void DuplicarComponentePrestacao(Dominio.Entidades.ConhecimentoDeTransporteEletronico cteAntigo, Dominio.Entidades.ConhecimentoDeTransporteEletronico cteNovo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ComponentePrestacaoCTE repComponentePrestacaoCTe = new Repositorio.ComponentePrestacaoCTE(unitOfWork);

            List<Dominio.Entidades.ComponentePrestacaoCTE> componentesPrestacaoCTe = repComponentePrestacaoCTe.BuscarPorCTe(cteAntigo.Codigo);

            foreach (Dominio.Entidades.ComponentePrestacaoCTE componentePrestacaoCTeAntigo in componentesPrestacaoCTe)
            {
                Dominio.Entidades.ComponentePrestacaoCTE componentePrestacaoCTeNovo = componentePrestacaoCTeAntigo.Clonar();

                Utilidades.Object.DefinirListasGenericasComoNulas(componentePrestacaoCTeNovo);

                componentePrestacaoCTeNovo.CTE = cteNovo;

                repComponentePrestacaoCTe.Inserir(componentePrestacaoCTeNovo);
            }
        }

        private static void DuplicarDocumentoDeTransporteAnterior(Dominio.Entidades.ConhecimentoDeTransporteEletronico cteAntigo, Dominio.Entidades.ConhecimentoDeTransporteEletronico cteNovo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.DocumentoDeTransporteAnteriorCTe repDocumentoTransporteAnteriorCTe = new Repositorio.DocumentoDeTransporteAnteriorCTe(unitOfWork);

            List<Dominio.Entidades.DocumentoDeTransporteAnteriorCTe> documentosTransporteAnteriorCTe = repDocumentoTransporteAnteriorCTe.BuscarPorCTe(cteAntigo.Codigo);

            foreach (Dominio.Entidades.DocumentoDeTransporteAnteriorCTe documentoTransporteAnteriorCTeAntigo in documentosTransporteAnteriorCTe)
            {
                Dominio.Entidades.DocumentoDeTransporteAnteriorCTe documentoTransporteAnteriorCTeNovo = documentoTransporteAnteriorCTeAntigo.Clonar();

                Utilidades.Object.DefinirListasGenericasComoNulas(documentoTransporteAnteriorCTeNovo);

                documentoTransporteAnteriorCTeNovo.CTe = cteNovo;

                repDocumentoTransporteAnteriorCTe.Inserir(documentoTransporteAnteriorCTeNovo);
            }
        }

        private static void DuplicarDocumentos(Dominio.Entidades.ConhecimentoDeTransporteEletronico cteAntigo, Dominio.Entidades.ConhecimentoDeTransporteEletronico cteNovo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.DocumentosCTE repDocumentoCTe = new Repositorio.DocumentosCTE(unitOfWork);

            List<Dominio.Entidades.DocumentosCTE> documentosCTe = repDocumentoCTe.BuscarPorCTe(cteAntigo.Codigo);

            foreach (Dominio.Entidades.DocumentosCTE documentoCTeAntigo in documentosCTe)
            {
                Dominio.Entidades.DocumentosCTE documentoCTeNovo = documentoCTeAntigo.Clonar();

                Utilidades.Object.DefinirListasGenericasComoNulas(documentoCTeNovo);

                if (string.IsNullOrWhiteSpace(documentoCTeNovo.CFOP))
                    documentoCTeNovo.CFOP = cteAntigo.XMLNotaFiscais != null ? cteAntigo.XMLNotaFiscais.Where(x => x.Chave == documentoCTeNovo.ChaveNFE)?.FirstOrDefault()?.CFOP : "";
                if (string.IsNullOrWhiteSpace(documentoCTeNovo.PINSuframa))
                    documentoCTeNovo.PINSuframa = cteAntigo.XMLNotaFiscais != null ? cteAntigo.XMLNotaFiscais.Where(x => x.Chave == documentoCTeNovo.ChaveNFE)?.FirstOrDefault()?.PINSUFRAMA : "";
                if (string.IsNullOrWhiteSpace(documentoCTeNovo.NumeroReferenciaEDI))
                    documentoCTeNovo.NumeroReferenciaEDI = cteAntigo.XMLNotaFiscais != null ? cteAntigo.XMLNotaFiscais.Where(x => x.Chave == documentoCTeNovo.ChaveNFE)?.FirstOrDefault()?.NumeroReferenciaEDI : "";
                if (string.IsNullOrWhiteSpace(documentoCTeNovo.NCMPredominante))
                    documentoCTeNovo.NCMPredominante = cteAntigo.XMLNotaFiscais != null ? cteAntigo.XMLNotaFiscais.Where(x => x.Chave == documentoCTeNovo.ChaveNFE)?.FirstOrDefault()?.NCM : "";
                if (string.IsNullOrWhiteSpace(documentoCTeNovo.NumeroControleCliente))
                    documentoCTeNovo.NumeroControleCliente = cteAntigo.XMLNotaFiscais != null ? cteAntigo.XMLNotaFiscais.Where(x => x.Chave == documentoCTeNovo.ChaveNFE)?.FirstOrDefault()?.NumeroControleCliente : "";

                documentoCTeNovo.CTE = cteNovo;

                repDocumentoCTe.Inserir(documentoCTeNovo);
            }
        }

        private static void DuplicarEntregaSimplificado(Dominio.Entidades.ConhecimentoDeTransporteEletronico cteAntigo, Dominio.Entidades.ConhecimentoDeTransporteEletronico cteNovo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.DocumentosCTE repDocumentoCTe = new Repositorio.DocumentosCTE(unitOfWork);
            Repositorio.DocumentoDeTransporteAnteriorCTe repDocumentoTransporteAnteriorCTe = new Repositorio.DocumentoDeTransporteAnteriorCTe(unitOfWork);
            Repositorio.EntregaCTe repEntregaCTe = new Repositorio.EntregaCTe(unitOfWork);
            Repositorio.EntregaCTeComponentePrestacao repEntregaCTeComponentePrestacao = new Repositorio.EntregaCTeComponentePrestacao(unitOfWork);
            Repositorio.EntregaCTeDocumento repEntregaCTeDocumento = new Repositorio.EntregaCTeDocumento(unitOfWork);
            Repositorio.EntregaCTeDocumentoTransporteAnterior repEntregaCTeDocumentoTransporteAnterior = new Repositorio.EntregaCTeDocumentoTransporteAnterior(unitOfWork);

            List<Dominio.Entidades.EntregaCTe> entregasCTe = repEntregaCTe.BuscarPorCTe(cteAntigo.Codigo);

            foreach (Dominio.Entidades.EntregaCTe entregaCTeAntigo in entregasCTe)
            {
                Dominio.Entidades.EntregaCTe entregaCTeNovo = entregaCTeAntigo.Clonar();
                Utilidades.Object.DefinirListasGenericasComoNulas(entregaCTeNovo);
                repEntregaCTe.Inserir(entregaCTeNovo);

                if (entregaCTeAntigo.ComponentesPrestacao != null)
                {
                    foreach (Dominio.Entidades.EntregaCTeComponentePrestacao ComponentePrestacaoCTeAntigo in entregaCTeAntigo.ComponentesPrestacao)
                    {
                        Dominio.Entidades.EntregaCTeComponentePrestacao ComponentePrestacaoCTeNovo = ComponentePrestacaoCTeAntigo.Clonar();
                        Utilidades.Object.DefinirListasGenericasComoNulas(ComponentePrestacaoCTeNovo);
                        ComponentePrestacaoCTeNovo.EntregaCTe = entregaCTeNovo;
                        repEntregaCTeComponentePrestacao.Inserir(ComponentePrestacaoCTeNovo);
                    }
                }

                if (entregaCTeAntigo.Documentos != null)
                {
                    foreach (Dominio.Entidades.EntregaCTeDocumento entregaCTeDocumentoCTeAntigo in entregaCTeAntigo.Documentos)
                    {
                        Dominio.Entidades.DocumentosCTE documentoCTeNovo = entregaCTeDocumentoCTeAntigo.DocumentosCTE.Clonar();
                        Utilidades.Object.DefinirListasGenericasComoNulas(documentoCTeNovo);

                        if (string.IsNullOrWhiteSpace(documentoCTeNovo.CFOP))
                            documentoCTeNovo.CFOP = cteAntigo.XMLNotaFiscais != null ? cteAntigo.XMLNotaFiscais.Where(x => x.Chave == documentoCTeNovo.ChaveNFE)?.FirstOrDefault()?.CFOP : "";
                        if (string.IsNullOrWhiteSpace(documentoCTeNovo.PINSuframa))
                            documentoCTeNovo.PINSuframa = cteAntigo.XMLNotaFiscais != null ? cteAntigo.XMLNotaFiscais.Where(x => x.Chave == documentoCTeNovo.ChaveNFE)?.FirstOrDefault()?.PINSUFRAMA : "";
                        if (string.IsNullOrWhiteSpace(documentoCTeNovo.NumeroReferenciaEDI))
                            documentoCTeNovo.NumeroReferenciaEDI = cteAntigo.XMLNotaFiscais != null ? cteAntigo.XMLNotaFiscais.Where(x => x.Chave == documentoCTeNovo.ChaveNFE)?.FirstOrDefault()?.NumeroReferenciaEDI : "";
                        if (string.IsNullOrWhiteSpace(documentoCTeNovo.NCMPredominante))
                            documentoCTeNovo.NCMPredominante = cteAntigo.XMLNotaFiscais != null ? cteAntigo.XMLNotaFiscais.Where(x => x.Chave == documentoCTeNovo.ChaveNFE)?.FirstOrDefault()?.NCM : "";
                        if (string.IsNullOrWhiteSpace(documentoCTeNovo.NumeroControleCliente))
                            documentoCTeNovo.NumeroControleCliente = cteAntigo.XMLNotaFiscais != null ? cteAntigo.XMLNotaFiscais.Where(x => x.Chave == documentoCTeNovo.ChaveNFE)?.FirstOrDefault()?.NumeroControleCliente : "";

                        documentoCTeNovo.CTE = cteNovo;
                        repDocumentoCTe.Inserir(documentoCTeNovo);

                        Dominio.Entidades.EntregaCTeDocumento entregaCTeDocumentoCTeNovo = new Dominio.Entidades.EntregaCTeDocumento();
                        entregaCTeDocumentoCTeNovo.EntregaCTe = entregaCTeNovo;
                        entregaCTeDocumentoCTeNovo.DocumentosCTE = documentoCTeNovo;
                        repEntregaCTeDocumento.Inserir(entregaCTeDocumentoCTeNovo);
                    }
                }

                if (entregaCTeAntigo.DocumentosTransporteAnterior != null)
                {
                    foreach (Dominio.Entidades.EntregaCTeDocumentoTransporteAnterior entregaCTeDocumentoTransporteAnteriorCTeAntigo in entregaCTeAntigo.DocumentosTransporteAnterior)
                    {
                        Dominio.Entidades.DocumentoDeTransporteAnteriorCTe documentoTransporteAnteriorCTeNovo = entregaCTeDocumentoTransporteAnteriorCTeAntigo.DocumentoTransporteAnterior.Clonar();
                        Utilidades.Object.DefinirListasGenericasComoNulas(documentoTransporteAnteriorCTeNovo);
                        documentoTransporteAnteriorCTeNovo.CTe = cteNovo;
                        repDocumentoTransporteAnteriorCTe.Inserir(documentoTransporteAnteriorCTeNovo);

                        Dominio.Entidades.EntregaCTeDocumentoTransporteAnterior entregaCTeDocumentoTransporteAnteriorCTeNovo = new Dominio.Entidades.EntregaCTeDocumentoTransporteAnterior();
                        entregaCTeDocumentoTransporteAnteriorCTeNovo.EntregaCTe = entregaCTeNovo;
                        entregaCTeDocumentoTransporteAnteriorCTeNovo.DocumentoTransporteAnterior = documentoTransporteAnteriorCTeNovo;
                        repEntregaCTeDocumentoTransporteAnterior.Inserir(entregaCTeDocumentoTransporteAnteriorCTeNovo);
                    }
                }
            }
        }

        private static void DuplicarProdutoPerigoso(Dominio.Entidades.ConhecimentoDeTransporteEletronico cteAntigo, Dominio.Entidades.ConhecimentoDeTransporteEletronico cteNovo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ProdutoPerigosoCTE repProdutoPerigosoCTe = new Repositorio.ProdutoPerigosoCTE(unitOfWork);

            List<Dominio.Entidades.ProdutoPerigosoCTE> produtosPerigososCTe = repProdutoPerigosoCTe.BuscarPorCTe(cteAntigo.Codigo);

            foreach (Dominio.Entidades.ProdutoPerigosoCTE produtoPerigosoCTeAntigo in produtosPerigososCTe)
            {
                Dominio.Entidades.ProdutoPerigosoCTE produtoPerigosoCTeNovo = produtoPerigosoCTeAntigo.Clonar();

                Utilidades.Object.DefinirListasGenericasComoNulas(produtoPerigosoCTeNovo);

                produtoPerigosoCTeNovo.CTE = cteNovo;

                repProdutoPerigosoCTe.Inserir(produtoPerigosoCTeNovo);
            }
        }

        private static void DuplicarObservacoesContribuinte(Dominio.Entidades.ConhecimentoDeTransporteEletronico cteAntigo, Dominio.Entidades.ConhecimentoDeTransporteEletronico cteNovo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ObservacaoContribuinteCTE repositorioObservacaoContribuinteCTE = new Repositorio.ObservacaoContribuinteCTE(unitOfWork);

            List<Dominio.Entidades.ObservacaoContribuinteCTE> observacoesContribuinteCTE = repositorioObservacaoContribuinteCTE.BuscarPorCTe(cteAntigo.Codigo);

            foreach (Dominio.Entidades.ObservacaoContribuinteCTE observacaoContribuinteCTEAntigo in observacoesContribuinteCTE)
            {
                Dominio.Entidades.ObservacaoContribuinteCTE observacaoContribuinteCTENovo = observacaoContribuinteCTEAntigo.Clonar();

                Utilidades.Object.DefinirListasGenericasComoNulas(observacaoContribuinteCTENovo);

                observacaoContribuinteCTENovo.CTE = cteNovo;

                repositorioObservacaoContribuinteCTE.Inserir(observacaoContribuinteCTENovo);
            }
        }

        private static void DuplicarObservacoesFisco(Dominio.Entidades.ConhecimentoDeTransporteEletronico cteAntigo, Dominio.Entidades.ConhecimentoDeTransporteEletronico cteNovo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ObservacaoFiscoCTE repositorioObservacaoFiscoCTE = new Repositorio.ObservacaoFiscoCTE(unitOfWork);

            List<Dominio.Entidades.ObservacaoFiscoCTE> observacoesFiscoCTE = repositorioObservacaoFiscoCTE.BuscarPorCTe(cteAntigo.Codigo);

            foreach (Dominio.Entidades.ObservacaoFiscoCTE observacaoFiscoCTEAntigo in observacoesFiscoCTE)
            {
                Dominio.Entidades.ObservacaoFiscoCTE observacaoFiscoCTENovo = observacaoFiscoCTEAntigo.Clonar();

                Utilidades.Object.DefinirListasGenericasComoNulas(observacaoFiscoCTENovo);

                observacaoFiscoCTENovo.CTE = cteNovo;

                repositorioObservacaoFiscoCTE.Inserir(observacaoFiscoCTENovo);
            }
        }

        private static void DuplicarComponentesFreteCargaCTe(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTeOriginal, Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTeNovo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete repCargaCTeComponentesFrete = new Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete> CargaCTeComponentesFrete = repCargaCTeComponentesFrete.BuscarPorCargaCTe(cargaCTeOriginal.Codigo);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete cargaCTeComponenteFreteAntigo in CargaCTeComponentesFrete)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete cargaCTeComponenteFreteNovo = cargaCTeComponenteFreteAntigo.Clonar();
                Utilidades.Object.DefinirListasGenericasComoNulas(cargaCTeComponenteFreteNovo);
                cargaCTeComponenteFreteNovo.CargaCTe = cargaCTeNovo;
                repCargaCTeComponentesFrete.Inserir(cargaCTeComponenteFreteNovo);
            }
        }

        private static void DuplicarCargaPedidoXMLNotaFiscalCTe(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTeOriginal, Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTeNovo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe> cargaPedidoXMLNotasFiscaisCTe = repCargaPedidoXMLNotaFiscalCTe.BuscarTodosCargaPedidoXMLNotaFiscalCTePorCargaCTe(cargaCTeOriginal.Codigo);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe cargaPedidoXMLNotaFiscalCTeAntigo in cargaPedidoXMLNotasFiscaisCTe)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe cargaPedidoXMLNotaFiscalCTeNovo = cargaPedidoXMLNotaFiscalCTeAntigo.Clonar();
                Utilidades.Object.DefinirListasGenericasComoNulas(cargaPedidoXMLNotaFiscalCTeNovo);
                cargaPedidoXMLNotaFiscalCTeNovo.CargaCTe = cargaCTeNovo;
                repCargaPedidoXMLNotaFiscalCTe.Inserir(cargaPedidoXMLNotaFiscalCTeNovo);
            }
        }

        #endregion Métodos Privados - Duplicar

        #region Métodos Privados

        private Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal CriarRegistroXMLNotaFiscalCTeManual(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.DocumentosCTE documentoCTe, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);

            int.TryParse(documentoCTe.Numero, out int numeroNota);

            Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notaFiscal = new Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal()
            {
                Chave = documentoCTe.ChaveNFE,
                NCM = documentoCTe.NCMPredominante,
                nfAtiva = true,
                Numero = numeroNota,
                Peso = documentoCTe.Peso,
                Valor = documentoCTe.Valor,
                DataEmissao = documentoCTe.DataEmissao,
                Destinatario = cargaPedido.Pedido.Destinatario,
                Emitente = cargaPedido.Pedido.Remetente,
                PesoLiquido = documentoCTe.Peso,
                Volumes = documentoCTe.Volume,
                TipoOperacaoNotaFiscal = TipoOperacaoNotaFiscal.Saida,
                CNPJTranposrtador = cargaPedido.Pedido.Empresa?.CNPJ_SemFormato ?? "",
                TipoDocumento = documentoCTe.ModeloDocumentoFiscal?.Numero == "55" ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.NFe : Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.Outros,
                Descricao = documentoCTe.Descricao ?? documentoCTe.ChaveNFE,
                ModalidadeFrete = ModalidadePagamentoFrete.Pago,
                Empresa = cargaPedido.Pedido.Empresa,
                PesoBaseParaCalculo = documentoCTe.Peso,
                XML = "",
                PlacaVeiculoNotaFiscal = "",
                DataRecebimento = DateTime.Now,
                NumeroReferenciaEDI = documentoCTe.NumeroReferenciaEDI,
                NumeroControleCliente = documentoCTe.NumeroControleCliente,
                PINSUFRAMA = documentoCTe.PINSuframa,
                Serie = documentoCTe.SerieOuSerieDaChave
            };

            repXMLNotaFiscal.Inserir(notaFiscal);

            return notaFiscal;
        }

        private static void SalvarComponentesControleGeracaoCTeAnulacao(Dominio.Entidades.Embarcador.CTe.ControleGeracaoCTeAnulacao controleGeracaoCTeAnulacao, dynamic componentesFrete, Repositorio.UnitOfWork unitOfWork)
        {
            if (componentesFrete == null || componentesFrete.Count <= 0)
                return;

            Repositorio.Embarcador.CTe.ControleGeracaoCTeAnulacaoComponenteFrete repControleGeracaoCTeAnulacaoComponenteFrete = new Repositorio.Embarcador.CTe.ControleGeracaoCTeAnulacaoComponenteFrete(unitOfWork);
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);

            foreach (dynamic componenteFrete in componentesFrete)
            {
                Dominio.Entidades.Embarcador.CTe.ControleGeracaoCTeAnulacaoComponenteFrete controleGeracaoCTeAnulacaoComponenteFrete = new Dominio.Entidades.Embarcador.CTe.ControleGeracaoCTeAnulacaoComponenteFrete()
                {
                    ControleGeracaoCTeAnulacao = controleGeracaoCTeAnulacao,
                    ComponenteFrete = repComponenteFrete.BuscarPorCodigo((int)componenteFrete.CodigoComponente),
                    IncluirBaseCalculoICMS = (bool)componenteFrete.IncluirBaseCalculoICMS,
                    IncluirTotalReceber = (bool)componenteFrete.IncluirTotalReceber,
                    Valor = Utilidades.Decimal.Converter((string)componenteFrete.Valor)
                };

                repControleGeracaoCTeAnulacaoComponenteFrete.Inserir(controleGeracaoCTeAnulacaoComponenteFrete);
            }
        }

        private static void AnularCTeEGerarMovimentacao(Dominio.Entidades.Embarcador.CTe.ControleGeracaoCTeAnulacao controleGeracaoCTeAnulacao, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.CTe.ControleGeracaoCTeAnulacao repControleGeracaoCTeAnulacao = new Repositorio.Embarcador.CTe.ControleGeracaoCTeAnulacao(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

            Servicos.Embarcador.Carga.CargaCTe servicoCargaCTe = new Servicos.Embarcador.Carga.CargaCTe(unitOfWork);

            unitOfWork.Start();

            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = controleGeracaoCTeAnulacao.CargaCTeOriginal.CTe;
            cte.Status = "Z";
            cte.DataAnulacao = controleGeracaoCTeAnulacao.CargaCTeAnulacao?.CTe?.DataAutorizacao;

            repCTe.Atualizar(cte);

            servicoCargaCTe.GerarIntegracoesCargaCTeManual(cte, false, controleGeracaoCTeAnulacao.CargaCTeOriginal.Carga);

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                if (!Servicos.Embarcador.Carga.Cancelamento.GerarMovimentoCancelamentoCTe(out string erro, controleGeracaoCTeAnulacao.CargaCTeOriginal, tipoServicoMultisoftware, unitOfWork, "", false, true))
                {
                    unitOfWork.Rollback();

                    Servicos.Log.TratarErro(erro);

                    return;
                }

                if (!Servicos.Embarcador.Carga.Cancelamento.ReverterItensEmAbertoAposCancelamentoCTe(out erro, controleGeracaoCTeAnulacao.CargaCTeOriginal, tipoServicoMultisoftware, unitOfWork))
                {
                    unitOfWork.Rollback();

                    Servicos.Log.TratarErro(erro);

                    return;
                }
            }

            if (controleGeracaoCTeAnulacao.NaoGerarCTeSubstituicao)
                controleGeracaoCTeAnulacao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoControleGeracaoCTeAnulacao.AguardandoAutorizacaoSubstituicao;
            else
                controleGeracaoCTeAnulacao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoControleGeracaoCTeAnulacao.GerandoCTeSubstituicao;

            repControleGeracaoCTeAnulacao.Atualizar(controleGeracaoCTeAnulacao);

            unitOfWork.CommitChanges();
        }

        private static bool AnularCTe(out string erro, Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe, string justificativa, Repositorio.UnitOfWork unidadeTrabalho, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (cargaCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.CTe)
                throw new Exception("Não é possível anular documentos com modelo diferente de 57 por este método.");

            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeTrabalho);

            cargaCTe.CTe.Status = "Z";
            cargaCTe.CTe.DataAnulacao = DateTime.Now;
            cargaCTe.CTe.ObservacaoCancelamento = justificativa;

            repCTe.Atualizar(cargaCTe.CTe);

            if (!Servicos.Embarcador.Carga.Cancelamento.GerarMovimentoCancelamentoCTe(out erro, cargaCTe, tipoServicoMultisoftware, unidadeTrabalho, stringConexao))
                return false;

            if (!Servicos.Embarcador.Carga.Cancelamento.ReverterItensEmAbertoAposCancelamentoCTe(out erro, cargaCTe, tipoServicoMultisoftware, unidadeTrabalho))
                return false;

            erro = string.Empty;
            return true;
        }

        private static bool CancelarCTe(out string erro, Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, string justificativa, Repositorio.UnitOfWork unidadeTrabalho, string stringConexao, string webServiceConsultaCTe, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.CTe &&
                cte.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.NFSe)
                throw new Exception("Não é possível cancelar documentos com modelo diferente de 57 por este método.");

            Servicos.NFSe svcNFSe = new Servicos.NFSe(unidadeTrabalho);
            Servicos.CTe svcCTe = new Servicos.CTe(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCTe.BuscarPorCTe(cte.Codigo);


            if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)
            {
                if (!Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.GetEmissorDocumentoCTe(cte.SistemaEmissor).CancelarCte(cte.Codigo, cte.Empresa.Codigo, justificativa, unidadeTrabalho))
                {
                    erro = "Ocorreram problemas ao enviar o CT-e para cancelamento.";
                    return false;
                }
            }
            else if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe)
            {
                if (!svcNFSe.CancelarNFSe(cte.Codigo, unidadeTrabalho))
                {
                    erro = "Ocorreram problemas ao enviar a NFS-e para cancelamento.";
                    return false;
                }
            }
            else
            {
                throw new Exception("Cancelamento para o modelo de documento não implementado.");
            }

            Servicos.Embarcador.Carga.Documentos.AdicionarCTeManualParaGeracaoDeMovimento(cargaCTe, "C", tipoServicoMultisoftware, unidadeTrabalho);

            erro = string.Empty;
            return true;
        }

        private static bool CancelarOutrosDocumentos(out string erro, Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe, string justificativa, Repositorio.UnitOfWork unidadeTrabalho, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (cargaCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe ||
                cargaCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe ||
                cargaCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS)
                throw new Exception("Não é possível cancelar documentos com modelo 57 por este método.");

            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeTrabalho);

            cargaCTe.CTe.Status = "C";
            cargaCTe.CTe.DataRetornoSefaz = DateTime.Now;
            cargaCTe.CTe.DataCancelamento = DateTime.Now;
            cargaCTe.CTe.ObservacaoCancelamento = justificativa;

            repCTe.Atualizar(cargaCTe.CTe);

            if (!Servicos.Embarcador.Carga.Cancelamento.GerarMovimentoCancelamentoCTe(out erro, cargaCTe, tipoServicoMultisoftware, unidadeTrabalho, stringConexao))
                return false;

            if (!Servicos.Embarcador.Carga.Cancelamento.ReverterItensEmAbertoAposCancelamentoCTe(out erro, cargaCTe, tipoServicoMultisoftware, unidadeTrabalho))
                return false;

            erro = string.Empty;
            return true;
        }

        private void alterarSerieCTe(ref Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.ObjetosDeValor.Embarcador.CTe.CTe cteIntegracao, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.CTe serCTE = new Servicos.CTe(unitOfWork);
            Repositorio.EmpresaSerie repSerie = new Repositorio.EmpresaSerie(unitOfWork);
            if (cte.Codigo <= 0)
            {
                if (!string.IsNullOrWhiteSpace(cteIntegracao.Serie))
                {
                    cte.Serie = repSerie.BuscarPorSerie(empresa.Codigo, int.Parse(cteIntegracao.Serie), Dominio.Enumeradores.TipoSerie.CTe);
                }
                else
                {
                    cte.Serie = serCTE.ObterSerie(empresa, cte.LocalidadeEmissao.Estado.Sigla, cte.LocalidadeInicioPrestacao.Estado.Sigla, cte.LocalidadeTerminoPrestacao.Estado.Sigla, cte.Remetente?.CPF_CNPJ ?? string.Empty, cte.Destinatario?.CPF_CNPJ ?? string.Empty, cte.Recebedor?.CPF_CNPJ ?? string.Empty, cte.Expedidor?.CPF_CNPJ ?? string.Empty, cte.TomadorPagador?.CPF_CNPJ ?? string.Empty, unitOfWork);
                }
            }
        }

        private void PreecherDadosCTe(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v200.ConhecimentoDeTransporteProcessado.cteProc cteProc)
        {

            if (cteProc.protCTe.infProt.cStat == "100")
                cte.SituacaoCTeSefaz = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Autorizada;
            else if (cteProc.protCTe.infProt.cStat == "110")
                cte.SituacaoCTeSefaz = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Denegada;
            else if (cteProc.protCTe.infProt.cStat == "101")
                cte.SituacaoCTeSefaz = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Cancelada;
            else
                cte.SituacaoCTeSefaz = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Rejeitada;

            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            SetarEmitente(ref cte, cteProc.CTe.infCte.emit);

            cte.CFOP = int.Parse(cteProc.CTe.infCte.ide.CFOP);
            cte.NaturezaOP = cteProc.CTe.infCte.ide.natOp;

            cte.ValorFrete = new Dominio.ObjetosDeValor.Embarcador.Frete.FreteValor();

            cte.ValorFrete.ValorTotalAReceber = decimal.Parse(cteProc.CTe.infCte.vPrest.vRec, cultura);
            cte.ValorFrete.ValorPrestacaoServico = decimal.Parse(cteProc.CTe.infCte.vPrest.vTPrest, cultura);

            Dominio.ObjetosDeValor.Localidade locInicioPrestacao = new Dominio.ObjetosDeValor.Localidade();
            locInicioPrestacao.IBGE = int.Parse(cteProc.CTe.infCte.ide.cMunIni);
            locInicioPrestacao.Descricao = cteProc.CTe.infCte.ide.xMunIni;
            locInicioPrestacao.SiglaUF = cteProc.CTe.infCte.ide.UFIni.ToString("g");

            Dominio.ObjetosDeValor.Localidade locFimPrestacao = new Dominio.ObjetosDeValor.Localidade();
            locFimPrestacao.IBGE = int.Parse(cteProc.CTe.infCte.ide.cMunFim);
            locFimPrestacao.Descricao = cteProc.CTe.infCte.ide.xMunFim;
            locFimPrestacao.SiglaUF = cteProc.CTe.infCte.ide.UFFim.ToString("g");

            locInicioPrestacao.IBGE = int.Parse(cteProc.CTe.infCte.ide.cMunIni);
            cte.LocalidadeInicioPrestacao = locInicioPrestacao;
            cte.LocalidadeFimPrestacao = locFimPrestacao;

            cte.TipoPagamento = (Dominio.Enumeradores.TipoPagamento)cteProc.CTe.infCte.ide.forPag;
            cte.Serie = cteProc.CTe.infCte.ide.serie;
            cte.TipoServico = (Dominio.Enumeradores.TipoServico)cteProc.CTe.infCte.ide.tpServ;
            cte.TipoCTE = (Dominio.Enumeradores.TipoCTE)cteProc.CTe.infCte.ide.tpCTe;
            cte.Retira = cteProc.CTe.infCte.ide.retira == MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteIdeRetira.Item1 ? false : true;
            cte.DetalhesRetira = cteProc.CTe.infCte.ide.xDetRetira;

            SetarDestinatario(ref cte, cteProc.CTe.infCte.dest);
            SetarRemetente(ref cte, cteProc.CTe.infCte.rem);
            SetarRecebedor(ref cte, cteProc.CTe.infCte.receb);
            SetarExpedidor(ref cte, cteProc.CTe.infCte.exped);

            DateTime dataEmissao;
            DateTime.TryParseExact(cteProc.CTe.infCte.ide.dhEmi, "yyyy-MM-ddTHH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dataEmissao);
            cte.DataEmissao = dataEmissao;
            cte.Numero = int.Parse(cteProc.CTe.infCte.ide.nCT);
            cte.Chave = cteProc.protCTe.infProt.chCTe;
            cte.Protocolo = cteProc.protCTe.infProt.nProt;
            cte.Versao = cteProc.protCTe.versao;
            cte.TipoAmbiente = (Dominio.Enumeradores.TipoAmbiente)cteProc.protCTe.infProt.tpAmb;

            this.ObterICMS(ref cte, cteProc.CTe.infCte.imp);
            this.ObterInformacoesTomador(ref cte, cteProc.CTe.infCte.ide);
            cte.InformacaoCarga = new Dominio.ObjetosDeValor.Embarcador.CTe.InformacaoCarga();
            this.ObterInformacoesCTe(ref cte, cteProc.CTe.infCte);
            this.ObterInformacoesComplementares(ref cte, cteProc.CTe.infCte.compl);
            this.ObterInformacoesComponentesDaPrestacao(ref cte, cteProc.CTe.infCte.vPrest.Comp);
        }

        public static string ObterDescricaoSituacao(string statusCTe)
        {
            switch (statusCTe)
            {
                case "P":
                    return "Pendente";
                case "E":
                    return "Enviado";
                case "R":
                    return "Rejeição";
                case "A":
                    return "Autorizado";
                case "C":
                    return "Cancelado";
                case "I":
                    return "Inutilizado";
                case "D":
                    return "Denegado";
                case "S":
                    return "Em Digitação";
                case "K":
                    return "Em Cancelamento";
                case "L":
                    return "Em Inutilização";
                case "Z":
                    return "Anulado";
                case "G":
                    return "Anulado Gerencialmente";
                case "X":
                    return "Aguardando Assinatura";
                case "V":
                    return "Aguardando Assinatura Cancelamento";
                case "B":
                    return "Aguardando Assinatura Inutilização";
                case "M":
                    return "Aguardando Emissão e-mail";
                case "F":
                    return "Contingência FSDA";
                case "Q":
                    return "Contingência EPEC";
                case "Y":
                    return "Aguardando Finalizar Carga Integração";
                default:
                    return string.Empty;
            }
        }

        public static string ObterDescricaoSituacao(List<string> statusCTe)
        {
            string descricaoStatus = string.Empty;

            foreach (string status in statusCTe)
                descricaoStatus += ObterDescricaoSituacao(status) + ", ";

            if (descricaoStatus.Length > 3)
                descricaoStatus = descricaoStatus.Substring(0, descricaoStatus.Length - 2);

            return descricaoStatus;
        }

        private void PreecherDadosCTe(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc cteProc)
        {
            if (cteProc.protCTe?.infProt == null)
            {
                cte.SituacaoCTeSefaz = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Rejeitada;
            }
            else
            {
                if (cteProc.protCTe.infProt.cStat == "100")
                    cte.SituacaoCTeSefaz = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Autorizada;
                else if (cteProc.protCTe.infProt.cStat == "110")
                    cte.SituacaoCTeSefaz = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Denegada;
                else if (cteProc.protCTe.infProt.cStat == "101")
                    cte.SituacaoCTeSefaz = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Cancelada;
                else
                    cte.SituacaoCTeSefaz = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Rejeitada;
            }

            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            SetarEmitente(ref cte, cteProc.CTe.infCte.emit);

            cte.CFOP = int.Parse(cteProc.CTe.infCte.ide.CFOP);
            cte.NaturezaOP = cteProc.CTe.infCte.ide.natOp;

            cte.ValorFrete = new Dominio.ObjetosDeValor.Embarcador.Frete.FreteValor();

            cte.ValorFrete.ValorTotalAReceber = decimal.Parse(cteProc.CTe.infCte.vPrest.vRec, cultura);
            cte.ValorFrete.ValorPrestacaoServico = decimal.Parse(cteProc.CTe.infCte.vPrest.vTPrest, cultura);

            Dominio.ObjetosDeValor.Localidade locInicioPrestacao = new Dominio.ObjetosDeValor.Localidade();
            locInicioPrestacao.IBGE = int.Parse(cteProc.CTe.infCte.ide.cMunIni);
            locInicioPrestacao.Descricao = cteProc.CTe.infCte.ide.xMunIni;
            locInicioPrestacao.SiglaUF = cteProc.CTe.infCte.ide.UFIni.ToString("g");

            Dominio.ObjetosDeValor.Localidade locFimPrestacao = new Dominio.ObjetosDeValor.Localidade();
            locFimPrestacao.IBGE = int.Parse(cteProc.CTe.infCte.ide.cMunFim);
            locFimPrestacao.Descricao = cteProc.CTe.infCte.ide.xMunFim;
            locFimPrestacao.SiglaUF = cteProc.CTe.infCte.ide.UFFim.ToString("g");

            locInicioPrestacao.IBGE = int.Parse(cteProc.CTe.infCte.ide.cMunIni);
            cte.LocalidadeInicioPrestacao = locInicioPrestacao;
            cte.LocalidadeFimPrestacao = locFimPrestacao;

            cte.Serie = cteProc.CTe.infCte.ide.serie;
            cte.TipoServico = (Dominio.Enumeradores.TipoServico)cteProc.CTe.infCte.ide.tpServ;
            cte.TipoCTE = (Dominio.Enumeradores.TipoCTE)cteProc.CTe.infCte.ide.tpCTe;
            cte.Retira = cteProc.CTe.infCte.ide.retira == MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteIdeRetira.Item1 ? false : true;
            cte.DetalhesRetira = cteProc.CTe.infCte.ide.xDetRetira;

            SetarDestinatario(ref cte, cteProc.CTe.infCte.dest);
            SetarRemetente(ref cte, cteProc.CTe.infCte.rem);
            SetarRecebedor(ref cte, cteProc.CTe.infCte.receb);
            SetarExpedidor(ref cte, cteProc.CTe.infCte.exped);

            DateTime dataEmissao;
            DateTime.TryParseExact(cteProc.CTe.infCte.ide.dhEmi, "yyyy-MM-ddTHH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dataEmissao);
            if (dataEmissao > DateTime.MinValue)
                cte.DataEmissao = dataEmissao;
            else
            {
                DateTime.TryParseExact(cteProc.CTe.infCte.ide.dhEmi, "yyyy-MM-ddTHH:mm:sszzz", null, System.Globalization.DateTimeStyles.None, out dataEmissao);
                cte.DataEmissao = dataEmissao;
            }
            cte.Numero = int.Parse(cteProc.CTe.infCte.ide.nCT);
            cte.Chave = cteProc.protCTe?.infProt?.chCTe ?? string.Empty;
            cte.Protocolo = cteProc.protCTe?.infProt?.nProt ?? string.Empty;
            cte.Versao = cteProc.protCTe?.versao ?? string.Empty;
            if (cteProc.protCTe?.infProt?.tpAmb != null)
                cte.TipoAmbiente = (Dominio.Enumeradores.TipoAmbiente)cteProc.protCTe.infProt.tpAmb;

            this.ObterICMS(ref cte, cteProc.CTe.infCte.imp);
            this.ObterInformacoesTomador(ref cte, cteProc.CTe.infCte.ide);
            cte.InformacaoCarga = new Dominio.ObjetosDeValor.Embarcador.CTe.InformacaoCarga();
            this.ObterInformacoesCTe(ref cte, cteProc.CTe.infCte);
            this.ObterInformacoesComplementares(ref cte, cteProc.CTe.infCte.compl);
            this.ObterInformacoesComponentesDaPrestacao(ref cte, cteProc.CTe.infCte.vPrest.Comp);

            if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente || cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor)
                cte.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Pago;
            else if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario || cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor)
                cte.TipoPagamento = Dominio.Enumeradores.TipoPagamento.A_Pagar;
            else
                cte.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Outros;
        }

        private void PreecherDadosCTe(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteProc cteProc)
        {
            if (cteProc.protCTe.infProt.cStat == "100")
                cte.SituacaoCTeSefaz = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Autorizada;
            else if (cteProc.protCTe.infProt.cStat == "110")
                cte.SituacaoCTeSefaz = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Denegada;
            else if (cteProc.protCTe.infProt.cStat == "101")
                cte.SituacaoCTeSefaz = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Cancelada;
            else
                cte.SituacaoCTeSefaz = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Rejeitada;

            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            SetarEmitente(ref cte, cteProc.CTe.infCte.emit);

            cte.CFOP = int.Parse(cteProc.CTe.infCte.ide.CFOP);
            cte.NaturezaOP = cteProc.CTe.infCte.ide.natOp;

            cte.ValorFrete = new Dominio.ObjetosDeValor.Embarcador.Frete.FreteValor();

            cte.ValorFrete.ValorTotalAReceber = decimal.Parse(cteProc.CTe.infCte.vPrest.vRec, cultura);
            cte.ValorFrete.ValorPrestacaoServico = decimal.Parse(cteProc.CTe.infCte.vPrest.vTPrest, cultura);
            cte.ValorFrete.ValorTotalDocumentoFiscal = decimal.Parse(cteProc.CTe.infCte.imp?.vTotDFe ?? "0", cultura);

            Dominio.ObjetosDeValor.Localidade locInicioPrestacao = new Dominio.ObjetosDeValor.Localidade();
            locInicioPrestacao.IBGE = int.Parse(cteProc.CTe.infCte.ide.cMunIni);
            locInicioPrestacao.Descricao = cteProc.CTe.infCte.ide.xMunIni;
            locInicioPrestacao.SiglaUF = cteProc.CTe.infCte.ide.UFIni.ToString("g");

            Dominio.ObjetosDeValor.Localidade locFimPrestacao = new Dominio.ObjetosDeValor.Localidade();
            locFimPrestacao.IBGE = int.Parse(cteProc.CTe.infCte.ide.cMunFim);
            locFimPrestacao.Descricao = cteProc.CTe.infCte.ide.xMunFim;
            locFimPrestacao.SiglaUF = cteProc.CTe.infCte.ide.UFFim.ToString("g");

            locInicioPrestacao.IBGE = int.Parse(cteProc.CTe.infCte.ide.cMunIni);
            cte.LocalidadeInicioPrestacao = locInicioPrestacao;
            cte.LocalidadeFimPrestacao = locFimPrestacao;

            cte.Serie = cteProc.CTe.infCte.ide.serie;
            cte.TipoServico = (Dominio.Enumeradores.TipoServico)cteProc.CTe.infCte.ide.tpServ;
            cte.TipoCTE = (Dominio.Enumeradores.TipoCTE)cteProc.CTe.infCte.ide.tpCTe;
            cte.Retira = cteProc.CTe.infCte.ide.retira == MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeRetira.Item1 ? false : true;
            cte.DetalhesRetira = cteProc.CTe.infCte.ide.xDetRetira;

            SetarDestinatario(ref cte, cteProc.CTe.infCte.dest);
            SetarRemetente(ref cte, cteProc.CTe.infCte.rem);
            SetarRecebedor(ref cte, cteProc.CTe.infCte.receb);
            SetarExpedidor(ref cte, cteProc.CTe.infCte.exped);

            DateTime dataEmissao;
            DateTime.TryParseExact(cteProc.CTe.infCte.ide.dhEmi, "yyyy-MM-ddTHH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dataEmissao);
            if (dataEmissao > DateTime.MinValue)
                cte.DataEmissao = dataEmissao;
            else
            {
                DateTime.TryParseExact(cteProc.CTe.infCte.ide.dhEmi, "yyyy-MM-ddTHH:mm:sszzz", null, System.Globalization.DateTimeStyles.None, out dataEmissao);
                cte.DataEmissao = dataEmissao;
            }
            cte.Numero = int.Parse(cteProc.CTe.infCte.ide.nCT);
            cte.Chave = cteProc.protCTe.infProt.chCTe;
            cte.Protocolo = cteProc.protCTe.infProt.nProt;
            cte.Versao = cteProc.protCTe.versao;
            cte.TipoAmbiente = (Dominio.Enumeradores.TipoAmbiente)cteProc.protCTe.infProt.tpAmb;

            this.ObterICMS(ref cte, cteProc.CTe.infCte.imp);
            this.ObterImpostoIBSCBS(ref cte, cteProc.CTe.infCte.imp);
            this.ObterInformacoesTomador(ref cte, cteProc.CTe.infCte.ide);
            cte.InformacaoCarga = new Dominio.ObjetosDeValor.Embarcador.CTe.InformacaoCarga();
            this.ObterInformacoesCTe(ref cte, cteProc.CTe.infCte);
            this.ObterInformacoesComplementares(ref cte, cteProc.CTe.infCte.compl);
            this.ObterInformacoesComponentesDaPrestacao(ref cte, cteProc.CTe.infCte.vPrest.Comp);

            if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente || cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor)
                cte.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Pago;
            else if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario || cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor)
                cte.TipoPagamento = Dominio.Enumeradores.TipoPagamento.A_Pagar;
            else
                cte.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Outros;
        }

        private void PreecherDadosCTe(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteSimpProc cteProc)
        {
            if (cteProc.protCTe.infProt.cStat == "100")
                cte.SituacaoCTeSefaz = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Autorizada;
            else if (cteProc.protCTe.infProt.cStat == "110")
                cte.SituacaoCTeSefaz = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Denegada;
            else if (cteProc.protCTe.infProt.cStat == "101")
                cte.SituacaoCTeSefaz = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Cancelada;
            else
                cte.SituacaoCTeSefaz = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Rejeitada;

            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            SetarEmitente(ref cte, cteProc.CTeSimp.infCte.emit);

            cte.CFOP = int.Parse(cteProc.CTeSimp.infCte.ide.CFOP);
            cte.NaturezaOP = cteProc.CTeSimp.infCte.ide.natOp;

            Dominio.ObjetosDeValor.Localidade locInicioPrestacao = new Dominio.ObjetosDeValor.Localidade();
            var inicioPrestacao = cteProc.CTeSimp.infCte.det.OrderBy(o => o.nItem).FirstOrDefault();
            locInicioPrestacao.IBGE = int.Parse(inicioPrestacao.cMunIni);
            locInicioPrestacao.Descricao = inicioPrestacao.xMunIni;
            locInicioPrestacao.SiglaUF = cteProc.CTeSimp.infCte.ide.UFIni.ToString("g");

            Dominio.ObjetosDeValor.Localidade locFimPrestacao = new Dominio.ObjetosDeValor.Localidade();
            var fimPrestacao = cteProc.CTeSimp.infCte.det.OrderByDescending(o => o.nItem).FirstOrDefault();
            locFimPrestacao.IBGE = int.Parse(fimPrestacao.cMunFim);
            locFimPrestacao.Descricao = fimPrestacao.xMunFim;
            locFimPrestacao.SiglaUF = cteProc.CTeSimp.infCte.ide.UFFim.ToString("g");

            locInicioPrestacao.IBGE = int.Parse(inicioPrestacao.cMunIni);
            cte.LocalidadeInicioPrestacao = locInicioPrestacao;
            cte.LocalidadeFimPrestacao = locFimPrestacao;

            cte.Serie = cteProc.CTeSimp.infCte.ide.serie;
            cte.TipoServico = (Dominio.Enumeradores.TipoServico)cteProc.CTeSimp.infCte.ide.tpServ;

            cte.TipoCTE = cteProc.CTeSimp.infCte.ide.tpCTe == MultiSoftware.CTe.v400.ConhecimentoDeTransporteSimplificado.TFinCTeSimp.Item5 ? Dominio.Enumeradores.TipoCTE.Simplificado : Dominio.Enumeradores.TipoCTE.SimplificadoSubstituto;
            cte.Retira = cteProc.CTeSimp.infCte.ide.retira == MultiSoftware.CTe.v400.ConhecimentoDeTransporteSimplificado.TCTeSimpInfCteIdeRetira.Item1 ? false : true;
            cte.DetalhesRetira = cteProc.CTeSimp.infCte.ide.xDetRetira;

            this.ObterInformacoesTomador(ref cte, cteProc.CTeSimp.infCte.toma);

            DateTime dataEmissao;
            DateTime.TryParseExact(cteProc.CTeSimp.infCte.ide.dhEmi, "yyyy-MM-ddTHH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dataEmissao);
            if (dataEmissao > DateTime.MinValue)
                cte.DataEmissao = dataEmissao;
            else
            {
                DateTime.TryParseExact(cteProc.CTeSimp.infCte.ide.dhEmi, "yyyy-MM-ddTHH:mm:sszzz", null, System.Globalization.DateTimeStyles.None, out dataEmissao);
                cte.DataEmissao = dataEmissao;
            }
            cte.Numero = int.Parse(cteProc.CTeSimp.infCte.ide.nCT);
            cte.Chave = cteProc.protCTe.infProt.chCTe;
            cte.Protocolo = cteProc.protCTe.infProt.nProt;
            cte.Versao = cteProc.protCTe.versao;
            cte.TipoAmbiente = (Dominio.Enumeradores.TipoAmbiente)cteProc.protCTe.infProt.tpAmb;

            cte.ValorFrete = new Dominio.ObjetosDeValor.Embarcador.Frete.FreteValor();
            this.ObterICMS(ref cte, cteProc.CTeSimp.infCte.imp);

            cte.InformacaoCarga = new Dominio.ObjetosDeValor.Embarcador.CTe.InformacaoCarga();
            cte.InformacaoCarga.ValorTotalCarga = decimal.Parse(cteProc.CTeSimp.infCte.infCarga.vCarga, cultura);
            cte.InformacaoCarga.ProdutoPredominante = cteProc.CTeSimp.infCte.infCarga.proPred;
            cte.InformacaoCarga.OutrasCaracteristicas = cteProc.CTeSimp.infCte.infCarga.xOutCat;

            ObterQuantidadesDaCarga(ref cte, cteProc.CTeSimp.infCte.infCarga.infQ);
            ObterModal(ref cte, cteProc.CTeSimp.infCte.infModal);
            this.SalvarInformacoesEntregas(ref cte, cteProc.CTeSimp.infCte.det.ToList());
            this.ObterInformacoesComplementares(ref cte, cteProc.CTeSimp.infCte.compl);

            if (cte.TipoCTE == Dominio.Enumeradores.TipoCTE.SimplificadoSubstituto)
                cte.ChaveCTeComplementado = cteProc.CTeSimp.infCte.infCteSub?.chCte;

            if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente || cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor)
                cte.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Pago;
            else if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario || cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor)
                cte.TipoPagamento = Dominio.Enumeradores.TipoPagamento.A_Pagar;
            else
                cte.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Outros;
        }

        private void ObterInformacoesComponentesDaPrestacao(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteVPrestComp[] infCTeComp)
        {
            if (infCTeComp != null)
            {
                cte.ValorFrete.ComponentesAdicionais = new List<Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional>();
                System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");
                foreach (var comp in infCTeComp)
                {
                    Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional componente = new Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional();
                    componente.Componente = new Dominio.ObjetosDeValor.Embarcador.Frete.Componente();
                    componente.Componente.Descricao = comp.xNome;
                    componente.ValorComponente = decimal.Parse(comp.vComp, cultura);
                    cte.ValorFrete.ComponentesAdicionais.Add(componente);
                }
            }
        }

        private void ObterInformacoesComponentesDaPrestacao(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteVPrestComp[] infCTeComp)
        {
            if (infCTeComp != null)
            {
                cte.ValorFrete.ComponentesAdicionais = new List<Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional>();
                System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");
                foreach (var comp in infCTeComp)
                {
                    Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional componente = new Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional();
                    componente.Componente = new Dominio.ObjetosDeValor.Embarcador.Frete.Componente();
                    componente.Componente.Descricao = comp.xNome;
                    componente.ValorComponente = decimal.Parse(comp.vComp, cultura);
                    cte.ValorFrete.ComponentesAdicionais.Add(componente);
                }
            }
        }

        private void ObterInformacoesComponentesDaPrestacao(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteVPrestComp[] infCTeComp)
        {
            if (infCTeComp != null)
            {
                cte.ValorFrete.ComponentesAdicionais = new List<Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional>();
                System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");
                foreach (var comp in infCTeComp)
                {
                    Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional componente = new Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional();
                    componente.Componente = new Dominio.ObjetosDeValor.Embarcador.Frete.Componente();
                    componente.Componente.Descricao = comp.xNome;
                    componente.ValorComponente = decimal.Parse(comp.vComp, cultura);
                    cte.ValorFrete.ComponentesAdicionais.Add(componente);
                }
            }
        }

        private void ObterInformacoesComplementares(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteCompl infCTeCompl)
        {
            if (infCTeCompl != null)
            {
                cte.InformacaoCarga.CaracteristicaServico = infCTeCompl.xCaracSer;
                cte.InformacaoCarga.CaracteristicaTransporte = infCTeCompl.xCaracAd;
                cte.InformacaoAdicionalContribuinte = infCTeCompl.xObs;
                ObterObservacoesContribuinte(ref cte, infCTeCompl.ObsCont);
                this.ObterObservacoesFisco(ref cte, infCTeCompl.ObsFisco);
            }
        }

        private void ObterInformacoesComplementares(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteCompl infCTeCompl)
        {
            if (infCTeCompl != null)
            {
                cte.InformacaoCarga.CaracteristicaServico = infCTeCompl.xCaracSer;
                cte.InformacaoCarga.CaracteristicaTransporte = infCTeCompl.xCaracAd;
                cte.InformacaoAdicionalContribuinte = infCTeCompl.xObs;
                ObterObservacoesContribuinte(ref cte, infCTeCompl.ObsCont);
                this.ObterObservacoesFisco(ref cte, infCTeCompl.ObsFisco);
            }
        }

        private void ObterInformacoesComplementares(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteCompl infCTeCompl)
        {
            if (infCTeCompl != null)
            {
                cte.InformacaoCarga.CaracteristicaServico = infCTeCompl.xCaracSer;
                cte.InformacaoCarga.CaracteristicaTransporte = infCTeCompl.xCaracAd;
                cte.InformacaoAdicionalContribuinte = infCTeCompl.xObs;
                ObterObservacoesContribuinte(ref cte, infCTeCompl.ObsCont);
                this.ObterObservacoesFisco(ref cte, infCTeCompl.ObsFisco);
            }
        }

        private void ObterInformacoesComplementares(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v400.ConhecimentoDeTransporteSimplificado.TCTeSimpInfCteCompl infCTeCompl)
        {
            if (infCTeCompl != null)
            {
                cte.InformacaoCarga.CaracteristicaServico = infCTeCompl.xCaracSer;
                cte.InformacaoCarga.CaracteristicaTransporte = infCTeCompl.xCaracAd;
                cte.InformacaoAdicionalContribuinte = infCTeCompl.xObs;
                ObterObservacoesContribuinte(ref cte, infCTeCompl.ObsCont);
                this.ObterObservacoesFisco(ref cte, infCTeCompl.ObsFisco);
            }
        }

        private void ObterObservacoesContribuinte(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteComplObsCont[] infObsCont)
        {
            if (infObsCont != null)
            {
                cte.ObservacoesContribuinte = new List<Dominio.ObjetosDeValor.Embarcador.CTe.Observacao>();
                foreach (var obs in infObsCont)
                {
                    Dominio.ObjetosDeValor.Embarcador.CTe.Observacao obsContribuinte = new Dominio.ObjetosDeValor.Embarcador.CTe.Observacao();
                    obsContribuinte.Texto = obs.xTexto;
                    obsContribuinte.Campo = obs.xCampo;
                    cte.ObservacoesContribuinte.Add(obsContribuinte);
                }
            }
        }

        private void ObterObservacoesContribuinte(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteComplObsCont[] infObsCont)
        {
            if (infObsCont != null)
            {
                cte.ObservacoesContribuinte = new List<Dominio.ObjetosDeValor.Embarcador.CTe.Observacao>();
                foreach (var obs in infObsCont)
                {
                    Dominio.ObjetosDeValor.Embarcador.CTe.Observacao obsContribuinte = new Dominio.ObjetosDeValor.Embarcador.CTe.Observacao();
                    obsContribuinte.Texto = obs.xTexto;
                    obsContribuinte.Campo = obs.xCampo;
                    cte.ObservacoesContribuinte.Add(obsContribuinte);
                }
            }
        }

        private void ObterObservacoesContribuinte(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteComplObsCont[] infObsCont)
        {
            if (infObsCont != null)
            {
                cte.ObservacoesContribuinte = new List<Dominio.ObjetosDeValor.Embarcador.CTe.Observacao>();
                foreach (var obs in infObsCont)
                {
                    Dominio.ObjetosDeValor.Embarcador.CTe.Observacao obsContribuinte = new Dominio.ObjetosDeValor.Embarcador.CTe.Observacao();
                    obsContribuinte.Texto = obs.xTexto;
                    obsContribuinte.Campo = obs.xCampo;
                    cte.ObservacoesContribuinte.Add(obsContribuinte);
                }
            }
        }

        private void ObterObservacoesContribuinte(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v400.ConhecimentoDeTransporteSimplificado.TCTeSimpInfCteComplObsCont[] infObsCont)
        {
            if (infObsCont != null)
            {
                cte.ObservacoesContribuinte = new List<Dominio.ObjetosDeValor.Embarcador.CTe.Observacao>();
                foreach (var obs in infObsCont)
                {
                    Dominio.ObjetosDeValor.Embarcador.CTe.Observacao obsContribuinte = new Dominio.ObjetosDeValor.Embarcador.CTe.Observacao();
                    obsContribuinte.Texto = obs.xTexto;
                    obsContribuinte.Campo = obs.xCampo;
                    cte.ObservacoesContribuinte.Add(obsContribuinte);
                }
            }
        }

        private void ObterObservacoesFisco(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteComplObsFisco[] infObsFisco)
        {
            if (infObsFisco != null)
            {
                cte.ObservacoesFisco = new List<Dominio.ObjetosDeValor.Embarcador.CTe.Observacao>();
                foreach (var obs in infObsFisco)
                {
                    Dominio.Entidades.ObservacaoFiscoCTE observacao = new Dominio.Entidades.ObservacaoFiscoCTE();
                    Dominio.ObjetosDeValor.Embarcador.CTe.Observacao obsFisco = new Dominio.ObjetosDeValor.Embarcador.CTe.Observacao();
                    obsFisco.Texto = obs.xTexto;
                    obsFisco.Campo = obs.xCampo;
                    cte.ObservacoesFisco.Add(obsFisco);
                }

            }
        }

        private void ObterObservacoesFisco(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteComplObsFisco[] infObsFisco)
        {
            if (infObsFisco != null)
            {
                cte.ObservacoesFisco = new List<Dominio.ObjetosDeValor.Embarcador.CTe.Observacao>();
                foreach (var obs in infObsFisco)
                {
                    Dominio.Entidades.ObservacaoFiscoCTE observacao = new Dominio.Entidades.ObservacaoFiscoCTE();
                    Dominio.ObjetosDeValor.Embarcador.CTe.Observacao obsFisco = new Dominio.ObjetosDeValor.Embarcador.CTe.Observacao();
                    obsFisco.Texto = obs.xTexto;
                    obsFisco.Campo = obs.xCampo;
                    cte.ObservacoesFisco.Add(obsFisco);
                }

            }
        }

        private void ObterObservacoesFisco(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteComplObsFisco[] infObsFisco)
        {
            if (infObsFisco != null)
            {
                cte.ObservacoesFisco = new List<Dominio.ObjetosDeValor.Embarcador.CTe.Observacao>();
                foreach (var obs in infObsFisco)
                {
                    Dominio.Entidades.ObservacaoFiscoCTE observacao = new Dominio.Entidades.ObservacaoFiscoCTE();
                    Dominio.ObjetosDeValor.Embarcador.CTe.Observacao obsFisco = new Dominio.ObjetosDeValor.Embarcador.CTe.Observacao();
                    obsFisco.Texto = obs.xTexto;
                    obsFisco.Campo = obs.xCampo;
                    cte.ObservacoesFisco.Add(obsFisco);
                }

            }
        }

        private void ObterObservacoesFisco(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v400.ConhecimentoDeTransporteSimplificado.TCTeSimpInfCteComplObsFisco[] infObsFisco)
        {
            if (infObsFisco != null)
            {
                cte.ObservacoesFisco = new List<Dominio.ObjetosDeValor.Embarcador.CTe.Observacao>();
                foreach (var obs in infObsFisco)
                {
                    Dominio.Entidades.ObservacaoFiscoCTE observacao = new Dominio.Entidades.ObservacaoFiscoCTE();
                    Dominio.ObjetosDeValor.Embarcador.CTe.Observacao obsFisco = new Dominio.ObjetosDeValor.Embarcador.CTe.Observacao();
                    obsFisco.Texto = obs.xTexto;
                    obsFisco.Campo = obs.xCampo;
                    cte.ObservacoesFisco.Add(obsFisco);
                }

            }
        }

        private void ObterInformacoesCTe(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCte infCTe)
        {
            if (infCTe != null)
            {
                Type tipoInfoCTe = infCTe.Item.GetType();
                if (tipoInfoCTe == typeof(MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCTeNorm))
                {
                    this.ObterInformacoesCTeNormal(ref cte, (MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCTeNorm)infCTe.Item);
                }
                else
                {
                    if (tipoInfoCTe == typeof(MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCteComp))
                    {
                        MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCteComp infCTeComple = (MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCteComp)infCTe.Item;
                        if (infCTeComple != null)
                            cte.ChaveCTeComplementado = infCTeComple.chave;
                    }
                }
            }
        }

        private void ObterInformacoesCTe(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCte infCTe)
        {
            if (infCTe != null)
            {
                Type tipoInfoCTe = infCTe.Item.GetType();
                if (tipoInfoCTe == typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNorm))
                {
                    this.ObterInformacoesCTeNormal(ref cte, (MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNorm)infCTe.Item);
                }
                else
                {
                    if (tipoInfoCTe == typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCteComp))
                    {
                        MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCteComp infCTeComple = (MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCteComp)infCTe.Item;
                        if (infCTeComple != null)
                            cte.ChaveCTeComplementado = infCTeComple.chCTe;
                    }
                }
            }
        }

        private void ObterInformacoesCTe(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCte infCTe)
        {
            if (infCTe != null)
            {
                Type tipoInfoCTe = infCTe.Items.FirstOrDefault()?.GetType();
                if (tipoInfoCTe == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNorm))
                {
                    this.ObterInformacoesCTeNormal(ref cte, (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNorm)infCTe.Items.First());
                }
                else
                {
                    if (tipoInfoCTe == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCteComp))
                    {
                        MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCteComp infCTeComple = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCteComp)infCTe.Items.First();
                        if (infCTeComple != null)
                            cte.ChaveCTeComplementado = infCTeComple.chCTe;
                    }
                }
            }
        }

        private void ObterInformacoesCTeNormal(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCTeNorm infCTeNormal)
        {
            if (infCTeNormal != null)
            {
                System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");
                cte.InformacaoCarga.ValorTotalCarga = decimal.Parse(infCTeNormal.infCarga.vCarga, cultura);
                cte.InformacaoCarga.ProdutoPredominante = infCTeNormal.infCarga.proPred;
                cte.InformacaoCarga.OutrasCaracteristicas = infCTeNormal.infCarga.xOutCat;

                ObterQuantidadesDaCarga(ref cte, infCTeNormal.infCarga.infQ);
                ObterSeguroDaCarga(ref cte, infCTeNormal.seg);

                this.ObterModalRodoviario(ref cte, infCTeNormal.infModal);
                //todo:implementar os outros Modais

                this.ObterInformacoesDocumentos(ref cte, infCTeNormal.infDoc);
            }
        }

        private void ObterInformacoesCTeNormal(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNorm infCTeNormal)
        {
            if (infCTeNormal != null)
            {
                System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");
                cte.InformacaoCarga.ValorTotalCarga = decimal.Parse(infCTeNormal.infCarga.vCarga, cultura);
                cte.InformacaoCarga.ProdutoPredominante = infCTeNormal.infCarga.proPred;
                cte.InformacaoCarga.OutrasCaracteristicas = infCTeNormal.infCarga.xOutCat;

                ObterQuantidadesDaCarga(ref cte, infCTeNormal.infCarga.infQ);

                ObterModal(ref cte, infCTeNormal.infModal);

                this.ObterInformacoesDocumentos(ref cte, infCTeNormal.infDoc);
                this.ObterInformacoesDocumentosAnteriores(ref cte, infCTeNormal.docAnt);
            }
        }

        private void ObterInformacoesCTeNormal(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNorm infCTeNormal)
        {
            if (infCTeNormal != null)
            {
                System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");
                cte.InformacaoCarga.ValorTotalCarga = decimal.Parse(infCTeNormal.infCarga.vCarga, cultura);
                cte.InformacaoCarga.ProdutoPredominante = infCTeNormal.infCarga.proPred;
                cte.InformacaoCarga.OutrasCaracteristicas = infCTeNormal.infCarga.xOutCat;

                ObterQuantidadesDaCarga(ref cte, infCTeNormal.infCarga.infQ);

                ObterModal(ref cte, infCTeNormal.infModal);

                this.ObterInformacoesDocumentos(ref cte, infCTeNormal.infDoc);
                this.ObterInformacoesDocumentosAnteriores(ref cte, infCTeNormal.docAnt);
            }
        }

        private void ObterInformacoesDocumentosAnteriores(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAnt[] docAnt)
        {
            if (docAnt != null)
            {
                cte.DocumentosAnteriores = new List<Dominio.ObjetosDeValor.Embarcador.CTe.DocumentoAnterior>();

                foreach (MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAnt documentoAnterior in docAnt)
                {
                    foreach (MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAntIdDocAnt idDocAnt in documentoAnterior.idDocAnt)
                    {
                        foreach (object item in idDocAnt.Items)
                        {
                            if (item.GetType() == typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAntIdDocAntIdDocAntEle))
                            {
                                MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAntIdDocAntIdDocAntEle documentoAnteriorEletronico = (MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAntIdDocAntIdDocAntEle)item;

                                cte.DocumentosAnteriores.Add(new Dominio.ObjetosDeValor.Embarcador.CTe.DocumentoAnterior()
                                {
                                    ChaveAcesso = documentoAnteriorEletronico.chCTe
                                });
                            }
                        }
                    }
                }
            }
        }

        private void ObterInformacoesDocumentosAnteriores(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAnt[] docAnt)
        {
            if (docAnt != null)
            {
                cte.DocumentosAnteriores = new List<Dominio.ObjetosDeValor.Embarcador.CTe.DocumentoAnterior>();

                foreach (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAnt documentoAnterior in docAnt)
                {
                    foreach (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAntIdDocAnt idDocAnt in documentoAnterior.idDocAnt)
                    {
                        foreach (object item in idDocAnt.Items)
                        {
                            if (item.GetType() == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAntIdDocAntIdDocAntEle))
                            {
                                MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAntIdDocAntIdDocAntEle documentoAnteriorEletronico = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAntIdDocAntIdDocAntEle)item;

                                cte.DocumentosAnteriores.Add(new Dominio.ObjetosDeValor.Embarcador.CTe.DocumentoAnterior()
                                {
                                    ChaveAcesso = documentoAnteriorEletronico.chCTe
                                });
                            }
                        }
                    }
                }
            }
        }

        private void ObterInformacoesDocumentos(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDoc infDoc)
        {
            if (infDoc != null)
            {
                System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");
                foreach (var item in infDoc.Items)
                {
                    Type tipoItem = item.GetType();
                    if (tipoItem == typeof(MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNF))
                    {
                        if (cte.NotasFiscais == null)
                            cte.NotasFiscais = new List<Dominio.ObjetosDeValor.Embarcador.CTe.NotaFiscal>();

                        MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNF nf = (MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNF)item;

                        Dominio.ObjetosDeValor.Embarcador.CTe.NotaFiscal notaFiscal = new Dominio.ObjetosDeValor.Embarcador.CTe.NotaFiscal();

                        DateTime dataPrevista = new DateTime();
                        DateTime.TryParseExact(nf.dEmi, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out dataPrevista);
                        notaFiscal.DataEmissao = dataPrevista;

                        notaFiscal.Numero = nf.nDoc;
                        notaFiscal.Serie = nf.serie;
                        notaFiscal.Valor = decimal.Parse(nf.vNF, cultura);
                        notaFiscal.ValorICMS = decimal.Parse(nf.vICMS, cultura);
                        notaFiscal.ValorICMSST = decimal.Parse(nf.vST, cultura);
                        notaFiscal.ValorProdutos = decimal.Parse(nf.vProd, cultura);

                        notaFiscal.BaseCalculoICMS = decimal.Parse(nf.vBC, cultura);
                        notaFiscal.BaseCalculoICMSST = decimal.Parse(nf.vBCST, cultura);
                        notaFiscal.CFOP = nf.nCFOP;
                        notaFiscal.ModeloNotaFiscal = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModeloNotaFiscal)nf.mod;

                        cte.NotasFiscais.Add(notaFiscal);
                    }
                    else if (tipoItem == typeof(MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNFe))
                    {
                        MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNFe nfe = (MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNFe)item;
                        if (cte.NFEs == null)
                            cte.NFEs = new List<Dominio.ObjetosDeValor.Embarcador.CTe.NFe>();

                        Dominio.ObjetosDeValor.Embarcador.CTe.NFe ChaveNfe = new Dominio.ObjetosDeValor.Embarcador.CTe.NFe();
                        ChaveNfe.Chave = nfe.chave;
                        cte.NFEs.Add(ChaveNfe);
                    }
                    else if (tipoItem == typeof(MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfOutros))
                    {
                        MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfOutros outros = (MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfOutros)item;

                        if (cte.OutrosDocumentos == null)
                            cte.OutrosDocumentos = new List<Dominio.ObjetosDeValor.Embarcador.CTe.OutroDocumento>();

                        Dominio.ObjetosDeValor.Embarcador.CTe.OutroDocumento documento = new Dominio.ObjetosDeValor.Embarcador.CTe.OutroDocumento();
                        documento.Descricao = outros.descOutros != null ? outros.descOutros : string.Empty; ;
                        documento.Numero = outros.nDoc != null ? outros.nDoc : string.Empty;
                        documento.Tipo = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOutroDocumento)outros.tpDoc;
                        documento.Valor = outros.vDocFisc != null ? decimal.Parse(outros.vDocFisc, cultura) : 0;
                        DateTime dataPrevista = new DateTime();
                        DateTime.TryParseExact(outros.dEmi, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out dataPrevista);
                        documento.DataEmissao = dataPrevista;

                        cte.OutrosDocumentos.Add(documento);
                    }
                }
            }
        }

        private void ObterInformacoesDocumentos(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDoc infDoc)
        {
            if (infDoc != null && infDoc.Items != null)
            {
                System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");
                foreach (var item in infDoc.Items)
                {
                    Type tipoItem = item.GetType();
                    if (tipoItem == typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNF))
                    {
                        if (cte.NotasFiscais == null)
                            cte.NotasFiscais = new List<Dominio.ObjetosDeValor.Embarcador.CTe.NotaFiscal>();

                        MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNF nf = (MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNF)item;

                        Dominio.ObjetosDeValor.Embarcador.CTe.NotaFiscal notaFiscal = new Dominio.ObjetosDeValor.Embarcador.CTe.NotaFiscal();

                        DateTime dataPrevista = new DateTime();
                        DateTime.TryParseExact(nf.dEmi, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out dataPrevista);
                        notaFiscal.DataEmissao = dataPrevista;

                        notaFiscal.Numero = nf.nDoc;
                        notaFiscal.Serie = nf.serie;
                        notaFiscal.Valor = decimal.Parse(nf.vNF, cultura);
                        notaFiscal.ValorICMS = decimal.Parse(nf.vICMS, cultura);
                        notaFiscal.ValorICMSST = decimal.Parse(nf.vST, cultura);
                        notaFiscal.ValorProdutos = decimal.Parse(nf.vProd, cultura);

                        notaFiscal.BaseCalculoICMS = decimal.Parse(nf.vBC, cultura);
                        notaFiscal.BaseCalculoICMSST = decimal.Parse(nf.vBCST, cultura);
                        notaFiscal.CFOP = nf.nCFOP;
                        notaFiscal.ModeloNotaFiscal = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModeloNotaFiscal)nf.mod;

                        cte.NotasFiscais.Add(notaFiscal);
                    }
                    else if (tipoItem == typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNFe))
                    {
                        MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNFe nfe = (MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNFe)item;
                        if (cte.NFEs == null)
                            cte.NFEs = new List<Dominio.ObjetosDeValor.Embarcador.CTe.NFe>();

                        Dominio.ObjetosDeValor.Embarcador.CTe.NFe chaveNfe = new Dominio.ObjetosDeValor.Embarcador.CTe.NFe();
                        chaveNfe.Chave = nfe.chave;
                        chaveNfe.Numero = Utilidades.Chave.ObterNumero(nfe.chave);
                        cte.NFEs.Add(chaveNfe);
                    }
                    else if (tipoItem == typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfOutros))
                    {
                        MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfOutros outros = (MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfOutros)item;

                        if (cte.OutrosDocumentos == null)
                            cte.OutrosDocumentos = new List<Dominio.ObjetosDeValor.Embarcador.CTe.OutroDocumento>();

                        Dominio.ObjetosDeValor.Embarcador.CTe.OutroDocumento documento = new Dominio.ObjetosDeValor.Embarcador.CTe.OutroDocumento();
                        documento.Descricao = outros.descOutros != null ? outros.descOutros : string.Empty; ;
                        documento.Numero = outros.nDoc != null ? outros.nDoc : string.Empty;
                        documento.Tipo = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOutroDocumento)outros.tpDoc;
                        documento.Valor = outros.vDocFisc != null ? decimal.Parse(outros.vDocFisc, cultura) : 0;
                        DateTime dataPrevista = new DateTime();
                        DateTime.TryParseExact(outros.dEmi, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out dataPrevista);
                        documento.DataEmissao = dataPrevista;

                        cte.OutrosDocumentos.Add(documento);
                    }
                }
            }
        }

        private void ObterInformacoesDocumentos(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDoc infDoc)
        {
            if (infDoc != null && infDoc.Items != null)
            {
                System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");
                foreach (var item in infDoc.Items)
                {
                    Type tipoItem = item.GetType();
                    if (tipoItem == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNF))
                    {
                        if (cte.NotasFiscais == null)
                            cte.NotasFiscais = new List<Dominio.ObjetosDeValor.Embarcador.CTe.NotaFiscal>();

                        MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNF nf = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNF)item;

                        Dominio.ObjetosDeValor.Embarcador.CTe.NotaFiscal notaFiscal = new Dominio.ObjetosDeValor.Embarcador.CTe.NotaFiscal();

                        DateTime dataPrevista = new DateTime();
                        DateTime.TryParseExact(nf.dEmi, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out dataPrevista);
                        notaFiscal.DataEmissao = dataPrevista;

                        notaFiscal.Numero = nf.nDoc;
                        notaFiscal.Serie = nf.serie;
                        notaFiscal.Valor = decimal.Parse(nf.vNF, cultura);
                        notaFiscal.ValorICMS = decimal.Parse(nf.vICMS, cultura);
                        notaFiscal.ValorICMSST = decimal.Parse(nf.vST, cultura);
                        notaFiscal.ValorProdutos = decimal.Parse(nf.vProd, cultura);

                        notaFiscal.BaseCalculoICMS = decimal.Parse(nf.vBC, cultura);
                        notaFiscal.BaseCalculoICMSST = decimal.Parse(nf.vBCST, cultura);
                        notaFiscal.CFOP = nf.nCFOP;
                        notaFiscal.ModeloNotaFiscal = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModeloNotaFiscal)nf.mod;

                        cte.NotasFiscais.Add(notaFiscal);
                    }
                    else if (tipoItem == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNFe))
                    {
                        MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNFe nfe = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNFe)item;
                        if (cte.NFEs == null)
                            cte.NFEs = new List<Dominio.ObjetosDeValor.Embarcador.CTe.NFe>();

                        Dominio.ObjetosDeValor.Embarcador.CTe.NFe chaveNfe = new Dominio.ObjetosDeValor.Embarcador.CTe.NFe();
                        chaveNfe.Chave = nfe.chave;
                        chaveNfe.Numero = Utilidades.Chave.ObterNumero(nfe.chave);
                        cte.NFEs.Add(chaveNfe);
                    }
                    else if (tipoItem == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfOutros))
                    {
                        MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfOutros outros = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfOutros)item;

                        if (cte.OutrosDocumentos == null)
                            cte.OutrosDocumentos = new List<Dominio.ObjetosDeValor.Embarcador.CTe.OutroDocumento>();

                        Dominio.ObjetosDeValor.Embarcador.CTe.OutroDocumento documento = new Dominio.ObjetosDeValor.Embarcador.CTe.OutroDocumento();
                        documento.Descricao = outros.descOutros != null ? outros.descOutros : string.Empty; ;
                        documento.Numero = outros.nDoc != null ? outros.nDoc : string.Empty;
                        documento.Tipo = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOutroDocumento)outros.tpDoc;
                        documento.Valor = outros.vDocFisc != null ? decimal.Parse(outros.vDocFisc, cultura) : 0;
                        DateTime dataPrevista = new DateTime();
                        DateTime.TryParseExact(outros.dEmi, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out dataPrevista);
                        documento.DataEmissao = dataPrevista;

                        cte.OutrosDocumentos.Add(documento);
                    }
                }
            }
        }

        private void SalvarInformacoesEntregas(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, List<MultiSoftware.CTe.v400.ConhecimentoDeTransporteSimplificado.TCTeSimpInfCteDet> infDet)
        {
            decimal valorTotalAReceber = 0;
            decimal valorTotalPrestacaoServico = 0;

            if (infDet != null)
            {
                System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

                foreach (MultiSoftware.CTe.v400.ConhecimentoDeTransporteSimplificado.TCTeSimpInfCteDet det in infDet)
                {
                    if (cte.Entregas == null)
                        cte.Entregas = new List<Dominio.ObjetosDeValor.Embarcador.CTe.EntregaSimplificado>();

                    Dominio.ObjetosDeValor.Embarcador.CTe.EntregaSimplificado entrega = new Dominio.ObjetosDeValor.Embarcador.CTe.EntregaSimplificado();
                    entrega.Origem = new Dominio.ObjetosDeValor.Localidade();
                    entrega.Origem.IBGE = int.Parse(det.cMunIni);

                    entrega.Destino = new Dominio.ObjetosDeValor.Localidade();
                    entrega.Destino.IBGE = int.Parse(det.cMunFim);

                    entrega.ValorFrete = decimal.Parse(det.vPrest, cultura);
                    entrega.ValorPrestacaoServico = decimal.Parse(det.vPrest, cultura);
                    entrega.ValorAReceber = decimal.Parse(det.vRec, cultura);
                    entrega.NFes = this.SalvarInformacoesEntregasNFes(det.Items);

                    if (det.Comp != null)
                        entrega.ComponentesAdicionais = this.SalvarInformacoesEntregasComponentesAdicionais(det.Comp.ToList(), entrega.ValorFrete);

                    cte.Entregas.Add(entrega);

                    valorTotalAReceber += entrega.ValorAReceber;
                    valorTotalPrestacaoServico += entrega.ValorPrestacaoServico;
                }
            }

            cte.ValorFrete.ValorTotalAReceber = valorTotalAReceber;
            cte.ValorFrete.ValorPrestacaoServico = valorTotalPrestacaoServico;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.CTe.NFe> SalvarInformacoesEntregasNFes(object[] infDet)
        {
            List<Dominio.ObjetosDeValor.Embarcador.CTe.NFe> retorno = null;

            foreach (object item in infDet)
            {
                Dominio.Entidades.DocumentosCTE documento = new Dominio.Entidades.DocumentosCTE();
                Type tipoItem = item.GetType();
                if (tipoItem == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporteSimplificado.TCTeSimpInfCteDetInfNFe))
                {
                    MultiSoftware.CTe.v400.ConhecimentoDeTransporteSimplificado.TCTeSimpInfCteDetInfNFe nfe = (MultiSoftware.CTe.v400.ConhecimentoDeTransporteSimplificado.TCTeSimpInfCteDetInfNFe)item;

                    if (retorno == null)
                        retorno = new List<Dominio.ObjetosDeValor.Embarcador.CTe.NFe>();

                    Dominio.ObjetosDeValor.Embarcador.CTe.NFe chaveNfe = new Dominio.ObjetosDeValor.Embarcador.CTe.NFe();
                    chaveNfe.Chave = nfe.chNFe;
                    chaveNfe.Numero = Utilidades.Chave.ObterNumero(nfe.chNFe);
                    retorno.Add(chaveNfe);
                }
            }

            return retorno;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional> SalvarInformacoesEntregasComponentesAdicionais(List<MultiSoftware.CTe.v400.ConhecimentoDeTransporteSimplificado.TCTeSimpInfCteDetComp> listaComp, decimal valorFrete)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional> retorno = new List<Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional>();

            if (listaComp != null)
            {
                System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

                foreach (MultiSoftware.CTe.v400.ConhecimentoDeTransporteSimplificado.TCTeSimpInfCteDetComp comp in listaComp)
                {
                    if (!comp.xNome.ToUpper().Contains("VALOR FRETE") && !comp.xNome.ToUpper().Contains("FRETE VALOR") && !comp.xNome.ToUpper().Contains("IMPOSTOS"))
                    {
                        Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional componente = new Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional();
                        componente.Descricao = comp.xNome;
                        componente.ValorComponente = Math.Round(decimal.Parse(comp.vComp, cultura), 2, MidpointRounding.ToEven);
                        componente.IncluirBaseCalculoICMS = false;
                        componente.IncluirTotalReceber = false;
                        retorno.Add(componente);
                    }
                }
            }

            Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional componenteValorFrete = new Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional();
            componenteValorFrete.Descricao = "FRETE VALOR";
            componenteValorFrete.ValorComponente = valorFrete;
            componenteValorFrete.IncluirBaseCalculoICMS = false;
            componenteValorFrete.IncluirTotalReceber = true;
            retorno.Add(componenteValorFrete);

            return retorno;
        }

        private void ObterModal(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfModal infModal)
        {
            if (infModal != null)
            {
                if (infModal.Any.Name == "aereo")
                    ObterModalAereo(ref cte, infModal);
                else if (infModal.Any.Name == "aquav")
                    cte.TipoModal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Aquaviario;
                else if (infModal.Any.Name == "duto")
                    cte.TipoModal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Dutoviario;
                else if (infModal.Any.Name == "ferrov")
                    cte.TipoModal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Ferroviario;
                else if (infModal.Any.Name == "multimodal")
                    cte.TipoModal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Multimodal;
                else
                    ObterModalRodoviario(ref cte, infModal);
            }
        }

        private void ObterModal(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfModal infModal)
        {
            if (infModal != null)
            {
                if (infModal.Any.Name == "aereo")
                    ObterModalAereo(ref cte, infModal);
                else if (infModal.Any.Name == "aquav")
                    cte.TipoModal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Aquaviario;
                else if (infModal.Any.Name == "duto")
                    cte.TipoModal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Dutoviario;
                else if (infModal.Any.Name == "ferrov")
                    cte.TipoModal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Ferroviario;
                else if (infModal.Any.Name == "multimodal")
                    cte.TipoModal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Multimodal;
                else
                    ObterModalRodoviario(ref cte, infModal);
            }
        }

        private void ObterModal(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v400.ConhecimentoDeTransporteSimplificado.TCTeSimpInfCteInfModal infModal)
        {
            if (infModal != null)
            {
                if (infModal.Any.Name == "aereo")
                    ObterModalAereo(ref cte, infModal);
                else if (infModal.Any.Name == "aquav")
                    cte.TipoModal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Aquaviario;
                else if (infModal.Any.Name == "duto")
                    cte.TipoModal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Dutoviario;
                else if (infModal.Any.Name == "ferrov")
                    cte.TipoModal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Ferroviario;
                else if (infModal.Any.Name == "multimodal")
                    cte.TipoModal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Multimodal;
                else
                    ObterModalRodoviario(ref cte, infModal);
            }
        }

        private void ObterModalAereo(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfModal infModal)
        {
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(MultiSoftware.CTe.v300.ModalAereo.aereo));

            byte[] data = Encoding.UTF8.GetBytes(infModal.Any.OuterXml);

            System.IO.MemoryStream memStream = new System.IO.MemoryStream(data, 0, data.Length);

            MultiSoftware.CTe.v300.ModalAereo.aereo modalAereo = (MultiSoftware.CTe.v300.ModalAereo.aereo)serializer.Deserialize(memStream);

            DateTime.TryParseExact(modalAereo.dPrevAereo, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime dataPrevistaEntrega);
            long? numeroMinuta = null;
            long? numeroOperacionalConhecimentoAereo = null;

            if (long.TryParse(modalAereo.nMinu, out long numeroMinutaAux))
                numeroMinuta = numeroMinutaAux;

            if (long.TryParse(modalAereo.nOCA, out long numeroOperacionalConhecimentoAereoAux))
                numeroOperacionalConhecimentoAereo = numeroOperacionalConhecimentoAereoAux;

            cte.TipoModal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Aereo;
            cte.ModalAereo = new Dominio.ObjetosDeValor.Embarcador.CTe.ModalAereo()
            {
                DataPrevistaEntrega = dataPrevistaEntrega,
                Dimensao = modalAereo.natCarga.xDime,
                NumeroMinuta = numeroMinuta,
                NumeroOperacionalConhecimentoAereo = numeroOperacionalConhecimentoAereo
            };
        }

        private void ObterModalAereo(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfModal infModal)
        {
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(MultiSoftware.CTe.v400.ModalAereo.aereo));

            byte[] data = Encoding.UTF8.GetBytes(infModal.Any.OuterXml);

            System.IO.MemoryStream memStream = new System.IO.MemoryStream(data, 0, data.Length);

            MultiSoftware.CTe.v400.ModalAereo.aereo modalAereo = (MultiSoftware.CTe.v400.ModalAereo.aereo)serializer.Deserialize(memStream);

            DateTime.TryParseExact(modalAereo.dPrevAereo, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime dataPrevistaEntrega);
            long? numeroMinuta = null;
            long? numeroOperacionalConhecimentoAereo = null;

            if (long.TryParse(modalAereo.nMinu, out long numeroMinutaAux))
                numeroMinuta = numeroMinutaAux;

            if (long.TryParse(modalAereo.nOCA, out long numeroOperacionalConhecimentoAereoAux))
                numeroOperacionalConhecimentoAereo = numeroOperacionalConhecimentoAereoAux;

            cte.TipoModal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Aereo;
            cte.ModalAereo = new Dominio.ObjetosDeValor.Embarcador.CTe.ModalAereo()
            {
                DataPrevistaEntrega = dataPrevistaEntrega,
                Dimensao = modalAereo.natCarga.xDime,
                NumeroMinuta = numeroMinuta,
                NumeroOperacionalConhecimentoAereo = numeroOperacionalConhecimentoAereo
            };
        }

        private void ObterModalAereo(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v400.ConhecimentoDeTransporteSimplificado.TCTeSimpInfCteInfModal infModal)
        {
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(MultiSoftware.CTe.v400.ModalAereo.aereo));

            byte[] data = Encoding.UTF8.GetBytes(infModal.Any.OuterXml);

            System.IO.MemoryStream memStream = new System.IO.MemoryStream(data, 0, data.Length);

            MultiSoftware.CTe.v400.ModalAereo.aereo modalAereo = (MultiSoftware.CTe.v400.ModalAereo.aereo)serializer.Deserialize(memStream);

            DateTime.TryParseExact(modalAereo.dPrevAereo, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime dataPrevistaEntrega);
            long? numeroMinuta = null;
            long? numeroOperacionalConhecimentoAereo = null;

            if (long.TryParse(modalAereo.nMinu, out long numeroMinutaAux))
                numeroMinuta = numeroMinutaAux;

            if (long.TryParse(modalAereo.nOCA, out long numeroOperacionalConhecimentoAereoAux))
                numeroOperacionalConhecimentoAereo = numeroOperacionalConhecimentoAereoAux;

            cte.TipoModal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Aereo;
            cte.ModalAereo = new Dominio.ObjetosDeValor.Embarcador.CTe.ModalAereo()
            {
                DataPrevistaEntrega = dataPrevistaEntrega,
                Dimensao = modalAereo.natCarga.xDime,
                NumeroMinuta = numeroMinuta,
                NumeroOperacionalConhecimentoAereo = numeroOperacionalConhecimentoAereo
            };
        }

        private void ObterModalRodoviario(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfModal infModal)
        {
            if (infModal != null)
            {
                cte.TipoModal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Rodoviario;
                cte.ModalRodoviario = new Dominio.ObjetosDeValor.Embarcador.CTe.ModalRodoviario();
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(MultiSoftware.CTe.v200.ModalRodoviario.rodo));
                byte[] data = Encoding.UTF8.GetBytes(infModal.Any.OuterXml);
                System.IO.MemoryStream memStream = new System.IO.MemoryStream(data, 0, data.Length);
                MultiSoftware.CTe.v200.ModalRodoviario.rodo modalRodoviario = (MultiSoftware.CTe.v200.ModalRodoviario.rodo)serializer.Deserialize(memStream);
                cte.ModalRodoviario.CIOT = modalRodoviario.CIOT;
                DateTime dataPrevista = new DateTime();
                DateTime.TryParseExact(modalRodoviario.dPrev, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out dataPrevista);
                if (dataPrevista != DateTime.MinValue)
                    cte.ModalRodoviario.DataEntrega = dataPrevista.ToString("dd/MM/yyyy");

                cte.ModalRodoviario.Lotacao = modalRodoviario.lota == MultiSoftware.CTe.v200.ModalRodoviario.rodoLota.Item0 ? false : true;
                cte.ModalRodoviario.RNTRC = modalRodoviario.RNTRC;
                cte.Emitente.RNTRC = modalRodoviario.RNTRC;

                this.ObterVeiculos(cte.ModalRodoviario, modalRodoviario.veic);
                this.ObterInformacoesMotoristas(cte.ModalRodoviario, modalRodoviario.moto);
            }
        }

        private void ObterModalRodoviario(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfModal infModal)
        {
            cte.TipoModal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Rodoviario;
            cte.ModalRodoviario = new Dominio.ObjetosDeValor.Embarcador.CTe.ModalRodoviario();

            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(MultiSoftware.CTe.v300.ModalRodoviario.rodo));

            byte[] data = Encoding.UTF8.GetBytes(infModal.Any.OuterXml);
            System.IO.MemoryStream memStream = new System.IO.MemoryStream(data, 0, data.Length);
            MultiSoftware.CTe.v300.ModalRodoviario.rodo modalRodoviario = (MultiSoftware.CTe.v300.ModalRodoviario.rodo)serializer.Deserialize(memStream);
            cte.ModalRodoviario.RNTRC = modalRodoviario.RNTRC;
            cte.Emitente.RNTRC = modalRodoviario.RNTRC;
        }

        private void ObterModalRodoviario(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfModal infModal)
        {
            cte.TipoModal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Rodoviario;
            cte.ModalRodoviario = new Dominio.ObjetosDeValor.Embarcador.CTe.ModalRodoviario();

            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(MultiSoftware.CTe.v400.ModalRodoviario.rodo));

            byte[] data = Encoding.UTF8.GetBytes(infModal.Any.OuterXml);
            System.IO.MemoryStream memStream = new System.IO.MemoryStream(data, 0, data.Length);
            MultiSoftware.CTe.v400.ModalRodoviario.rodo modalRodoviario = (MultiSoftware.CTe.v400.ModalRodoviario.rodo)serializer.Deserialize(memStream);
            cte.ModalRodoviario.RNTRC = modalRodoviario.RNTRC;
            cte.Emitente.RNTRC = modalRodoviario.RNTRC;
        }

        private void ObterModalRodoviario(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v400.ConhecimentoDeTransporteSimplificado.TCTeSimpInfCteInfModal infModal)
        {
            cte.TipoModal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Rodoviario;
            cte.ModalRodoviario = new Dominio.ObjetosDeValor.Embarcador.CTe.ModalRodoviario();

            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(MultiSoftware.CTe.v400.ModalRodoviario.rodo));

            byte[] data = Encoding.UTF8.GetBytes(infModal.Any.OuterXml);
            System.IO.MemoryStream memStream = new System.IO.MemoryStream(data, 0, data.Length);
            MultiSoftware.CTe.v400.ModalRodoviario.rodo modalRodoviario = (MultiSoftware.CTe.v400.ModalRodoviario.rodo)serializer.Deserialize(memStream);
            cte.ModalRodoviario.RNTRC = modalRodoviario.RNTRC;
            cte.Emitente.RNTRC = modalRodoviario.RNTRC;
        }

        private void ObterInformacoesMotoristas(Dominio.ObjetosDeValor.Embarcador.CTe.ModalRodoviario modalRodoviario, MultiSoftware.CTe.v200.ModalRodoviario.rodoMoto[] infMoto)
        {
            if (infMoto != null)
            {
                modalRodoviario.Motoristas = new List<Dominio.ObjetosDeValor.Motorista>();
                foreach (var moto in infMoto)
                {
                    Dominio.ObjetosDeValor.Motorista motorista = new Dominio.ObjetosDeValor.Motorista();
                    motorista.CPF = moto.CPF;
                    motorista.Nome = moto.xNome;
                    motorista.Ativo = true;
                    modalRodoviario.Motoristas.Add(motorista);
                }
            }
        }

        private void ObterVeiculos(Dominio.ObjetosDeValor.Embarcador.CTe.ModalRodoviario modalRodoviario, MultiSoftware.CTe.v200.ModalRodoviario.rodoVeic[] infVeic)
        {
            if (infVeic != null)
            {
                modalRodoviario.Veiculos = new List<Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo>();
                foreach (var veic in infVeic)
                {
                    Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo veiculo = new Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo();
                    veiculo.CapacidadeKG = int.Parse(veic.capKG);
                    veiculo.CapacidadeM3 = int.Parse(veic.capM3);
                    veiculo.Placa = veic.placa;
                    veiculo.Renavam = veic.RENAVAM;
                    veiculo.Tara = int.Parse(veic.tara);
                    veiculo.TipoCarroceria = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCarroceria)veic.tpCar;
                    veiculo.TipoRodado = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRodado)veic.tpRod;
                    veiculo.TipoVeiculo = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoVeiculo)veic.tpVeic;
                    veiculo.UF = veic.UF.ToString("G");
                    modalRodoviario.Veiculos.Add(veiculo);
                }
            }
        }

        private void ObterSeguroDaCarga(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormSeg[] infSeg)
        {
            if (infSeg != null)
            {
                cte.Seguros = new List<Dominio.ObjetosDeValor.Embarcador.CTe.Seguro>();
                System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

                foreach (var informacaoSeguro in infSeg)
                {
                    Dominio.ObjetosDeValor.Embarcador.CTe.Seguro seguro = new Dominio.ObjetosDeValor.Embarcador.CTe.Seguro();
                    seguro.Seguradora = informacaoSeguro.xSeg;
                    seguro.Apolice = informacaoSeguro.nApol;
                    seguro.Averbacao = informacaoSeguro.nAver;
                    seguro.ResponsavelSeguro = (Dominio.Enumeradores.TipoSeguro)informacaoSeguro.respSeg;
                    seguro.Valor = informacaoSeguro.vCarga != null ? decimal.Parse(informacaoSeguro.vCarga, cultura) : 0m;
                    cte.Seguros.Add(seguro);
                }
            }
        }

        private void ObterQuantidadesDaCarga(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfCargaInfQ[] infQuant)
        {
            if (infQuant != null)
            {
                cte.QuantidadesCarga = new List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga>();
                System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");
                foreach (var informacaoQuantidade in infQuant)
                {
                    Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga infoQuantCarga = new Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga();
                    infoQuantCarga.Unidade = (Dominio.Enumeradores.UnidadeMedida)informacaoQuantidade.cUnid;
                    infoQuantCarga.Medida = informacaoQuantidade.tpMed;
                    infoQuantCarga.Quantidade = decimal.Parse(informacaoQuantidade.qCarga, cultura);
                    cte.QuantidadesCarga.Add(infoQuantCarga);
                }
            }
        }

        private void ObterQuantidadesDaCarga(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfCargaInfQ[] infQuant)
        {
            if (infQuant != null)
            {
                cte.QuantidadesCarga = new List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga>();
                System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");
                foreach (var informacaoQuantidade in infQuant)
                {
                    Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga infoQuantCarga = new Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga();
                    infoQuantCarga.Unidade = (Dominio.Enumeradores.UnidadeMedida)informacaoQuantidade.cUnid;
                    infoQuantCarga.Medida = informacaoQuantidade.tpMed;
                    infoQuantCarga.Quantidade = decimal.Parse(informacaoQuantidade.qCarga, cultura);
                    cte.QuantidadesCarga.Add(infoQuantCarga);
                }
            }
        }

        private void ObterQuantidadesDaCarga(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfCargaInfQ[] infQuant)
        {
            if (infQuant != null)
            {
                cte.QuantidadesCarga = new List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga>();
                System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");
                foreach (var informacaoQuantidade in infQuant)
                {
                    Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga infoQuantCarga = new Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga();
                    infoQuantCarga.Unidade = (Dominio.Enumeradores.UnidadeMedida)informacaoQuantidade.cUnid;
                    infoQuantCarga.Medida = informacaoQuantidade.tpMed;
                    infoQuantCarga.Quantidade = decimal.Parse(informacaoQuantidade.qCarga, cultura);
                    cte.QuantidadesCarga.Add(infoQuantCarga);
                }
            }
        }

        private void ObterQuantidadesDaCarga(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v400.ConhecimentoDeTransporteSimplificado.TCTeSimpInfCteInfCargaInfQ[] infQuant)
        {
            if (infQuant != null)
            {
                cte.QuantidadesCarga = new List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga>();
                System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");
                foreach (var informacaoQuantidade in infQuant)
                {
                    Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga infoQuantCarga = new Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga();
                    infoQuantCarga.Unidade = (Dominio.Enumeradores.UnidadeMedida)informacaoQuantidade.cUnid;
                    infoQuantCarga.Medida = informacaoQuantidade.tpMed.ToString();
                    infoQuantCarga.Quantidade = decimal.Parse(informacaoQuantidade.qCarga, cultura);
                    cte.QuantidadesCarga.Add(infoQuantCarga);
                }
            }
        }

        private void SetarEmitente(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteEmit infEmit)
        {
            if (infEmit != null)
            {
                cte.Emitente = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa();
                cte.Emitente.CNPJ = infEmit.CNPJ;
                cte.Emitente.Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco();
                cte.Emitente.Endereco.Bairro = infEmit.enderEmit.xBairro;
                cte.Emitente.Endereco.CEP = infEmit.enderEmit.CEP;
                cte.Emitente.Endereco.Cidade = new Dominio.ObjetosDeValor.Localidade();
                cte.Emitente.Endereco.Cidade.IBGE = int.Parse(infEmit.enderEmit.cMun);
                cte.Emitente.Endereco.Cidade.Descricao = infEmit.enderEmit.xMun;
                cte.Emitente.Endereco.Cidade.SiglaUF = infEmit.enderEmit.UF.ToString("g");
                cte.Emitente.Endereco.Complemento = infEmit.enderEmit.xCpl;
                cte.Emitente.Endereco.Logradouro = infEmit.enderEmit.xLgr;
                cte.Emitente.Endereco.Numero = infEmit.enderEmit.nro;
                cte.Emitente.Endereco.Telefone = infEmit.enderEmit.fone;
                cte.Emitente.IE = infEmit.IE;
                cte.Emitente.NomeFantasia = infEmit.xFant;
                cte.Emitente.RazaoSocial = infEmit.xNome;
                cte.Emitente.RNTRC = "";
                cte.Emitente.SimplesNacional = true;
            }
        }

        private void SetarEmitente(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteEmit infEmit)
        {
            if (infEmit != null)
            {
                cte.Emitente = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa();
                cte.Emitente.CNPJ = infEmit.CNPJ;
                cte.Emitente.Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco();
                cte.Emitente.Endereco.Bairro = infEmit.enderEmit.xBairro;
                cte.Emitente.Endereco.CEP = infEmit.enderEmit.CEP;
                cte.Emitente.Endereco.Cidade = new Dominio.ObjetosDeValor.Localidade();
                cte.Emitente.Endereco.Cidade.IBGE = int.Parse(infEmit.enderEmit.cMun);
                cte.Emitente.Endereco.Cidade.Descricao = infEmit.enderEmit.xMun;
                cte.Emitente.Endereco.Cidade.SiglaUF = infEmit.enderEmit.UF.ToString("g");
                cte.Emitente.Endereco.Complemento = infEmit.enderEmit.xCpl;
                cte.Emitente.Endereco.Logradouro = infEmit.enderEmit.xLgr;
                cte.Emitente.Endereco.Numero = infEmit.enderEmit.nro;
                cte.Emitente.Endereco.Telefone = infEmit.enderEmit.fone;
                cte.Emitente.IE = infEmit.IE;
                cte.Emitente.NomeFantasia = infEmit.xFant;
                cte.Emitente.RazaoSocial = infEmit.xNome;
                cte.Emitente.RNTRC = "";
                cte.Emitente.SimplesNacional = true;
            }
        }

        private void SetarEmitente(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteEmit infEmit)
        {
            if (infEmit != null)
            {
                cte.Emitente = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa();
                cte.Emitente.CNPJ = infEmit.Item;
                cte.Emitente.Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco();
                cte.Emitente.Endereco.Bairro = infEmit.enderEmit.xBairro;
                cte.Emitente.Endereco.CEP = infEmit.enderEmit.CEP;
                cte.Emitente.Endereco.Cidade = new Dominio.ObjetosDeValor.Localidade();
                cte.Emitente.Endereco.Cidade.IBGE = int.Parse(infEmit.enderEmit.cMun);
                cte.Emitente.Endereco.Cidade.Descricao = infEmit.enderEmit.xMun;
                cte.Emitente.Endereco.Cidade.SiglaUF = infEmit.enderEmit.UF.ToString("g");
                cte.Emitente.Endereco.Complemento = infEmit.enderEmit.xCpl;
                cte.Emitente.Endereco.Logradouro = infEmit.enderEmit.xLgr;
                cte.Emitente.Endereco.Numero = infEmit.enderEmit.nro;
                cte.Emitente.Endereco.Telefone = infEmit.enderEmit.fone;
                cte.Emitente.IE = infEmit.IE;
                cte.Emitente.NomeFantasia = infEmit.xFant;
                cte.Emitente.RazaoSocial = infEmit.xNome;
                cte.Emitente.RNTRC = "";
                cte.Emitente.SimplesNacional = true;
            }
        }

        private void SetarEmitente(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v400.ConhecimentoDeTransporteSimplificado.TCTeSimpInfCteEmit infEmit)
        {
            if (infEmit != null)
            {
                cte.Emitente = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa();
                cte.Emitente.CNPJ = infEmit.Item;
                cte.Emitente.Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco();
                cte.Emitente.Endereco.Bairro = infEmit.enderEmit.xBairro;
                cte.Emitente.Endereco.CEP = infEmit.enderEmit.CEP;
                cte.Emitente.Endereco.Cidade = new Dominio.ObjetosDeValor.Localidade();
                cte.Emitente.Endereco.Cidade.IBGE = int.Parse(infEmit.enderEmit.cMun);
                cte.Emitente.Endereco.Cidade.Descricao = infEmit.enderEmit.xMun;
                cte.Emitente.Endereco.Cidade.SiglaUF = infEmit.enderEmit.UF.ToString("g");
                cte.Emitente.Endereco.Complemento = infEmit.enderEmit.xCpl;
                cte.Emitente.Endereco.Logradouro = infEmit.enderEmit.xLgr;
                cte.Emitente.Endereco.Numero = infEmit.enderEmit.nro;
                cte.Emitente.Endereco.Telefone = infEmit.enderEmit.fone;
                cte.Emitente.IE = infEmit.IE;
                cte.Emitente.NomeFantasia = infEmit.xFant;
                cte.Emitente.RazaoSocial = infEmit.xNome;
                cte.Emitente.RNTRC = "";
                cte.Emitente.SimplesNacional = true;
            }
        }

        private void SetarRemetente(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteRem infRem)
        {
            if (infRem != null)
            {
                cte.Remetente = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa();
                cte.Remetente.CPFCNPJ = infRem.Item;
                cte.Remetente.Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco();
                cte.Remetente.Endereco.Bairro = infRem.enderReme.xBairro;
                cte.Remetente.Endereco.CEP = infRem.enderReme.CEP;
                cte.Remetente.Endereco.Cidade = new Dominio.ObjetosDeValor.Localidade();
                cte.Remetente.Endereco.Cidade.IBGE = int.Parse(infRem.enderReme.cMun);
                cte.Remetente.Endereco.Cidade.Descricao = infRem.enderReme.xMun;
                cte.Remetente.Endereco.Cidade.SiglaUF = infRem.enderReme.UF.ToString("g");
                cte.Remetente.Endereco.Complemento = infRem.enderReme.xCpl;
                cte.Remetente.Endereco.Logradouro = infRem.enderReme.xLgr;
                cte.Remetente.Endereco.Numero = infRem.enderReme.nro;
                cte.Remetente.Endereco.Telefone = infRem.fone;
                cte.Remetente.RGIE = infRem.IE;
                cte.Remetente.RazaoSocial = infRem.xNome;
                cte.Remetente.NomeFantasia = infRem.xFant;
                cte.Remetente.Email = infRem.email;
            }
        }

        private void SetarRemetente(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteRem infRem)
        {
            if (infRem != null)
            {
                cte.Remetente = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa();
                cte.Remetente.CPFCNPJ = infRem.Item;
                cte.Remetente.Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco();
                cte.Remetente.Endereco.Bairro = infRem.enderReme.xBairro;
                cte.Remetente.Endereco.CEP = infRem.enderReme.CEP;
                cte.Remetente.Endereco.Cidade = new Dominio.ObjetosDeValor.Localidade();
                cte.Remetente.Endereco.Cidade.IBGE = int.Parse(infRem.enderReme.cMun);
                cte.Remetente.Endereco.Cidade.Descricao = infRem.enderReme.xMun;
                cte.Remetente.Endereco.Cidade.SiglaUF = infRem.enderReme.UF.ToString("g");
                cte.Remetente.Endereco.Complemento = infRem.enderReme.xCpl;
                cte.Remetente.Endereco.Logradouro = infRem.enderReme.xLgr;
                cte.Remetente.Endereco.Numero = infRem.enderReme.nro;
                cte.Remetente.Endereco.Telefone = infRem.fone;
                cte.Remetente.RGIE = infRem.IE;
                cte.Remetente.RazaoSocial = infRem.xNome;
                cte.Remetente.NomeFantasia = infRem.xFant;
                cte.Remetente.Email = infRem.email;

                if (cte.Remetente.CPFCNPJ == "00000000000000" && cte.Remetente.Endereco.Cidade.SiglaUF == "EX")
                {
                    cte.Remetente.ClienteExterior = true;
                    cte.Remetente.Endereco.Cidade.Pais = new Dominio.ObjetosDeValor.Embarcador.Localidade.Pais()
                    {
                        NomePais = infRem.enderReme.xPais,
                        CodigoPais = infRem.enderReme.cPais.ToInt()
                    };
                }
            }
        }

        private void SetarRemetente(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteRem infRem)
        {
            if (infRem != null)
            {
                cte.Remetente = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa();
                cte.Remetente.CPFCNPJ = infRem.Item;
                cte.Remetente.Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco();
                cte.Remetente.Endereco.Bairro = infRem.enderReme.xBairro;
                cte.Remetente.Endereco.CEP = infRem.enderReme.CEP;
                cte.Remetente.Endereco.Cidade = new Dominio.ObjetosDeValor.Localidade();
                cte.Remetente.Endereco.Cidade.IBGE = int.Parse(infRem.enderReme.cMun);
                cte.Remetente.Endereco.Cidade.Descricao = infRem.enderReme.xMun;
                cte.Remetente.Endereco.Cidade.SiglaUF = infRem.enderReme.UF.ToString("g");
                cte.Remetente.Endereco.Complemento = infRem.enderReme.xCpl;
                cte.Remetente.Endereco.Logradouro = infRem.enderReme.xLgr;
                cte.Remetente.Endereco.Numero = infRem.enderReme.nro;
                cte.Remetente.Endereco.Telefone = infRem.fone;
                cte.Remetente.RGIE = infRem.IE;
                cte.Remetente.RazaoSocial = infRem.xNome;
                cte.Remetente.NomeFantasia = infRem.xFant;
                cte.Remetente.Email = infRem.email;

                if (cte.Remetente.CPFCNPJ == "00000000000000" && cte.Remetente.Endereco.Cidade.SiglaUF == "EX")
                {
                    cte.Remetente.ClienteExterior = true;
                    cte.Remetente.Endereco.Cidade.Pais = new Dominio.ObjetosDeValor.Embarcador.Localidade.Pais()
                    {
                        NomePais = infRem.enderReme.xPais,
                        CodigoPais = infRem.enderReme.cPais.ToInt()
                    };
                }
            }
        }

        private void SetarRemetente(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v400.ConhecimentoDeTransporteSimplificado.TCTeSimpInfCteToma infRem)
        {
            if (infRem != null)
            {
                cte.Remetente = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa();
                cte.Remetente.CPFCNPJ = infRem.Item;
                cte.Remetente.Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco();
                cte.Remetente.Endereco.Bairro = infRem.enderToma.xBairro;
                cte.Remetente.Endereco.CEP = infRem.enderToma.CEP;
                cte.Remetente.Endereco.Cidade = new Dominio.ObjetosDeValor.Localidade();
                cte.Remetente.Endereco.Cidade.IBGE = int.Parse(infRem.enderToma.cMun);
                cte.Remetente.Endereco.Cidade.Descricao = infRem.enderToma.xMun;
                cte.Remetente.Endereco.Cidade.SiglaUF = infRem.enderToma.UF.ToString("g");
                cte.Remetente.Endereco.Complemento = infRem.enderToma.xCpl;
                cte.Remetente.Endereco.Logradouro = infRem.enderToma.xLgr;
                cte.Remetente.Endereco.Numero = infRem.enderToma.nro;
                cte.Remetente.Endereco.Telefone = infRem.fone;
                cte.Remetente.RGIE = infRem.IE;
                cte.Remetente.RazaoSocial = infRem.xNome;
                cte.Remetente.NomeFantasia = infRem.xNome;
                cte.Remetente.Email = infRem.email;

                if (cte.Remetente.CPFCNPJ == "00000000000000" && cte.Remetente.Endereco.Cidade.SiglaUF == "EX")
                {
                    cte.Remetente.ClienteExterior = true;
                    cte.Remetente.Endereco.Cidade.Pais = new Dominio.ObjetosDeValor.Embarcador.Localidade.Pais()
                    {
                        NomePais = infRem.enderToma.xPais,
                        CodigoPais = infRem.enderToma.cPais.ToInt()
                    };
                }
            }
        }

        private void SetarDestinatario(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteDest infDest)
        {
            if (infDest != null)
            {
                cte.Destinatario = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa();
                cte.Destinatario.CPFCNPJ = infDest.Item;
                cte.Destinatario.Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco();
                cte.Destinatario.Endereco.Bairro = infDest.enderDest.xBairro;
                cte.Destinatario.Endereco.CEP = infDest.enderDest.CEP;
                cte.Destinatario.Endereco.Cidade = new Dominio.ObjetosDeValor.Localidade();
                cte.Destinatario.Endereco.Cidade.IBGE = int.Parse(infDest.enderDest.cMun);
                cte.Destinatario.Endereco.Cidade.Descricao = infDest.enderDest.xMun;
                cte.Destinatario.Endereco.Cidade.SiglaUF = infDest.enderDest.UF.ToString("g");
                cte.Destinatario.Endereco.Complemento = infDest.enderDest.xCpl;
                cte.Destinatario.Endereco.Logradouro = infDest.enderDest.xLgr;
                cte.Destinatario.Endereco.Numero = infDest.enderDest.nro;
                cte.Destinatario.Endereco.Telefone = infDest.fone;
                cte.Destinatario.RGIE = infDest.IE;
                cte.Destinatario.RazaoSocial = infDest.xNome;
                cte.Destinatario.Email = infDest.email;
            }
        }

        private void SetarDestinatario(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteDest infDest)
        {
            if (infDest != null)
            {
                cte.Destinatario = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa();
                cte.Destinatario.CPFCNPJ = infDest.Item;
                cte.Destinatario.Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco();
                cte.Destinatario.Endereco.Bairro = infDest.enderDest.xBairro;
                cte.Destinatario.Endereco.CEP = infDest.enderDest.CEP;
                cte.Destinatario.Endereco.Cidade = new Dominio.ObjetosDeValor.Localidade();
                cte.Destinatario.Endereco.Cidade.IBGE = int.Parse(infDest.enderDest.cMun);
                cte.Destinatario.Endereco.Cidade.Descricao = infDest.enderDest.xMun;
                cte.Destinatario.Endereco.Cidade.SiglaUF = infDest.enderDest.UF.ToString("g");
                cte.Destinatario.Endereco.Complemento = infDest.enderDest.xCpl;
                cte.Destinatario.Endereco.Logradouro = infDest.enderDest.xLgr;
                cte.Destinatario.Endereco.Numero = infDest.enderDest.nro;
                cte.Destinatario.Endereco.Telefone = infDest.fone;
                cte.Destinatario.RGIE = infDest.IE;
                cte.Destinatario.RazaoSocial = infDest.xNome;
                cte.Destinatario.Email = infDest.email;

                if (cte.Destinatario.CPFCNPJ == "00000000000000" && cte.Destinatario.Endereco.Cidade.SiglaUF == "EX")
                {
                    cte.Destinatario.ClienteExterior = true;
                    cte.Destinatario.Endereco.Cidade.Pais = new Dominio.ObjetosDeValor.Embarcador.Localidade.Pais()
                    {
                        NomePais = infDest.enderDest.xPais,
                        CodigoPais = infDest.enderDest.cPais.ToInt()
                    };
                }
            }
        }

        private void SetarDestinatario(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteDest infDest)
        {
            if (infDest != null)
            {
                cte.Destinatario = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa();
                cte.Destinatario.CPFCNPJ = infDest.Item;
                cte.Destinatario.Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco();
                cte.Destinatario.Endereco.Bairro = infDest.enderDest.xBairro;
                cte.Destinatario.Endereco.CEP = infDest.enderDest.CEP;
                cte.Destinatario.Endereco.Cidade = new Dominio.ObjetosDeValor.Localidade();
                cte.Destinatario.Endereco.Cidade.IBGE = int.Parse(infDest.enderDest.cMun);
                cte.Destinatario.Endereco.Cidade.Descricao = infDest.enderDest.xMun;
                cte.Destinatario.Endereco.Cidade.SiglaUF = infDest.enderDest.UF.ToString("g");
                cte.Destinatario.Endereco.Complemento = infDest.enderDest.xCpl;
                cte.Destinatario.Endereco.Logradouro = infDest.enderDest.xLgr;
                cte.Destinatario.Endereco.Numero = infDest.enderDest.nro;
                cte.Destinatario.Endereco.Telefone = infDest.fone;
                cte.Destinatario.RGIE = infDest.IE;
                cte.Destinatario.RazaoSocial = infDest.xNome;
                cte.Destinatario.Email = infDest.email;

                if (cte.Destinatario.CPFCNPJ == "00000000000000" && cte.Destinatario.Endereco.Cidade.SiglaUF == "EX")
                {
                    cte.Destinatario.ClienteExterior = true;
                    cte.Destinatario.Endereco.Cidade.Pais = new Dominio.ObjetosDeValor.Embarcador.Localidade.Pais()
                    {
                        NomePais = infDest.enderDest.xPais,
                        CodigoPais = infDest.enderDest.cPais.ToInt()
                    };
                }
            }
        }

        private void SetarDestinatario(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v400.ConhecimentoDeTransporteSimplificado.TCTeSimpInfCteToma infDest)
        {
            if (infDest != null)
            {
                cte.Destinatario = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa();
                cte.Destinatario.CPFCNPJ = infDest.Item;
                cte.Destinatario.Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco();
                cte.Destinatario.Endereco.Bairro = infDest.enderToma.xBairro;
                cte.Destinatario.Endereco.CEP = infDest.enderToma.CEP;
                cte.Destinatario.Endereco.Cidade = new Dominio.ObjetosDeValor.Localidade();
                cte.Destinatario.Endereco.Cidade.IBGE = int.Parse(infDest.enderToma.cMun);
                cte.Destinatario.Endereco.Cidade.Descricao = infDest.enderToma.xMun;
                cte.Destinatario.Endereco.Cidade.SiglaUF = infDest.enderToma.UF.ToString("g");
                cte.Destinatario.Endereco.Complemento = infDest.enderToma.xCpl;
                cte.Destinatario.Endereco.Logradouro = infDest.enderToma.xLgr;
                cte.Destinatario.Endereco.Numero = infDest.enderToma.nro;
                cte.Destinatario.Endereco.Telefone = infDest.fone;
                cte.Destinatario.RGIE = infDest.IE;
                cte.Destinatario.RazaoSocial = infDest.xNome;
                cte.Destinatario.Email = infDest.email;

                if (cte.Destinatario.CPFCNPJ == "00000000000000" && cte.Destinatario.Endereco.Cidade.SiglaUF == "EX")
                {
                    cte.Destinatario.ClienteExterior = true;
                    cte.Destinatario.Endereco.Cidade.Pais = new Dominio.ObjetosDeValor.Embarcador.Localidade.Pais()
                    {
                        NomePais = infDest.enderToma.xPais,
                        CodigoPais = infDest.enderToma.cPais.ToInt()
                    };
                }
            }
        }

        private void SetarExpedidor(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteExped infExped)
        {
            if (infExped != null)
            {
                cte.Expedidor = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa();
                cte.Expedidor.CPFCNPJ = infExped.Item;
                cte.Expedidor.Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco();
                cte.Expedidor.Endereco.Bairro = infExped.enderExped.xBairro;
                cte.Expedidor.Endereco.CEP = infExped.enderExped.CEP;
                cte.Expedidor.Endereco.Cidade = new Dominio.ObjetosDeValor.Localidade();
                cte.Expedidor.Endereco.Cidade.IBGE = int.Parse(infExped.enderExped.cMun);
                cte.Expedidor.Endereco.Cidade.Descricao = infExped.enderExped.xMun;
                cte.Expedidor.Endereco.Cidade.SiglaUF = infExped.enderExped.UF.ToString("g");
                cte.Expedidor.Endereco.Complemento = infExped.enderExped.xCpl;
                cte.Expedidor.Endereco.Logradouro = infExped.enderExped.xLgr;
                cte.Expedidor.Endereco.Numero = infExped.enderExped.nro;
                cte.Expedidor.Endereco.Telefone = infExped.fone;
                cte.Expedidor.RGIE = infExped.IE;
                cte.Expedidor.RazaoSocial = infExped.xNome;
                cte.Expedidor.Email = infExped.email;

                if (cte.Expedidor.CPFCNPJ == "00000000000000" && cte.Expedidor.Endereco.Cidade.SiglaUF == "EX")
                {
                    cte.Expedidor.ClienteExterior = true;
                    cte.Expedidor.Endereco.Cidade.Pais = new Dominio.ObjetosDeValor.Embarcador.Localidade.Pais()
                    {
                        NomePais = infExped.enderExped.xPais,
                        CodigoPais = infExped.enderExped.cPais.ToInt()
                    };
                }
            }
        }

        private void SetarExpedidor(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteExped infExped)
        {
            if (infExped != null)
            {
                cte.Expedidor = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa();
                cte.Expedidor.CPFCNPJ = infExped.Item;
                cte.Expedidor.Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco();
                cte.Expedidor.Endereco.Bairro = infExped.enderExped.xBairro;
                cte.Expedidor.Endereco.CEP = infExped.enderExped.CEP;
                cte.Expedidor.Endereco.Cidade = new Dominio.ObjetosDeValor.Localidade();
                cte.Expedidor.Endereco.Cidade.IBGE = int.Parse(infExped.enderExped.cMun);
                cte.Expedidor.Endereco.Cidade.Descricao = infExped.enderExped.xMun;
                cte.Expedidor.Endereco.Cidade.SiglaUF = infExped.enderExped.UF.ToString("g");
                cte.Expedidor.Endereco.Complemento = infExped.enderExped.xCpl;
                cte.Expedidor.Endereco.Logradouro = infExped.enderExped.xLgr;
                cte.Expedidor.Endereco.Numero = infExped.enderExped.nro;
                cte.Expedidor.Endereco.Telefone = infExped.fone;
                cte.Expedidor.RGIE = infExped.IE;
                cte.Expedidor.RazaoSocial = infExped.xNome;
                cte.Expedidor.Email = infExped.email;

                if (cte.Expedidor.CPFCNPJ == "00000000000000" && cte.Expedidor.Endereco.Cidade.SiglaUF == "EX")
                {
                    cte.Expedidor.ClienteExterior = true;
                    cte.Expedidor.Endereco.Cidade.Pais = new Dominio.ObjetosDeValor.Embarcador.Localidade.Pais()
                    {
                        NomePais = infExped.enderExped.xPais,
                        CodigoPais = infExped.enderExped.cPais.ToInt()
                    };
                }
            }
        }

        private void SetarExpedidor(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteExped infExped)
        {
            if (infExped != null)
            {
                cte.Expedidor = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa();
                cte.Expedidor.CPFCNPJ = infExped.Item;
                cte.Expedidor.Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco();
                cte.Expedidor.Endereco.Bairro = infExped.enderExped.xBairro;
                cte.Expedidor.Endereco.CEP = infExped.enderExped.CEP;
                cte.Expedidor.Endereco.Cidade = new Dominio.ObjetosDeValor.Localidade();
                cte.Expedidor.Endereco.Cidade.IBGE = int.Parse(infExped.enderExped.cMun);
                cte.Expedidor.Endereco.Cidade.Descricao = infExped.enderExped.xMun;
                cte.Expedidor.Endereco.Cidade.SiglaUF = infExped.enderExped.UF.ToString("g");
                cte.Expedidor.Endereco.Complemento = infExped.enderExped.xCpl;
                cte.Expedidor.Endereco.Logradouro = infExped.enderExped.xLgr;
                cte.Expedidor.Endereco.Numero = infExped.enderExped.nro;
                cte.Expedidor.Endereco.Telefone = infExped.fone;
                cte.Expedidor.RGIE = infExped.IE;
                cte.Expedidor.RazaoSocial = infExped.xNome;
                cte.Expedidor.Email = infExped.email;

                if (cte.Expedidor.CPFCNPJ == "00000000000000" && cte.Expedidor.Endereco.Cidade.SiglaUF == "EX")
                {
                    cte.Expedidor.ClienteExterior = true;
                    cte.Expedidor.Endereco.Cidade.Pais = new Dominio.ObjetosDeValor.Embarcador.Localidade.Pais()
                    {
                        NomePais = infExped.enderExped.xPais,
                        CodigoPais = infExped.enderExped.cPais.ToInt()
                    };
                }
            }
        }

        private void SetarExpedidor(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v400.ConhecimentoDeTransporteSimplificado.TCTeSimpInfCteToma infExped)
        {
            if (infExped != null)
            {
                cte.Expedidor = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa();
                cte.Expedidor.CPFCNPJ = infExped.Item;
                cte.Expedidor.Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco();
                cte.Expedidor.Endereco.Bairro = infExped.enderToma.xBairro;
                cte.Expedidor.Endereco.CEP = infExped.enderToma.CEP;
                cte.Expedidor.Endereco.Cidade = new Dominio.ObjetosDeValor.Localidade();
                cte.Expedidor.Endereco.Cidade.IBGE = int.Parse(infExped.enderToma.cMun);
                cte.Expedidor.Endereco.Cidade.Descricao = infExped.enderToma.xMun;
                cte.Expedidor.Endereco.Cidade.SiglaUF = infExped.enderToma.UF.ToString("g");
                cte.Expedidor.Endereco.Complemento = infExped.enderToma.xCpl;
                cte.Expedidor.Endereco.Logradouro = infExped.enderToma.xLgr;
                cte.Expedidor.Endereco.Numero = infExped.enderToma.nro;
                cte.Expedidor.Endereco.Telefone = infExped.fone;
                cte.Expedidor.RGIE = infExped.IE;
                cte.Expedidor.RazaoSocial = infExped.xNome;
                cte.Expedidor.Email = infExped.email;

                if (cte.Expedidor.CPFCNPJ == "00000000000000" && cte.Expedidor.Endereco.Cidade.SiglaUF == "EX")
                {
                    cte.Expedidor.ClienteExterior = true;
                    cte.Expedidor.Endereco.Cidade.Pais = new Dominio.ObjetosDeValor.Embarcador.Localidade.Pais()
                    {
                        NomePais = infExped.enderToma.xPais,
                        CodigoPais = infExped.enderToma.cPais.ToInt()
                    };
                }
            }
        }

        private void SetarRecebedor(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteReceb infReceb)
        {
            if (infReceb != null)
            {
                cte.Recebedor = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa();
                cte.Recebedor.CPFCNPJ = infReceb.Item;
                cte.Recebedor.Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco();
                cte.Recebedor.Endereco.Bairro = infReceb.enderReceb.xBairro;
                cte.Recebedor.Endereco.CEP = infReceb.enderReceb.CEP;
                cte.Recebedor.Endereco.Cidade = new Dominio.ObjetosDeValor.Localidade();
                cte.Recebedor.Endereco.Cidade.IBGE = int.Parse(infReceb.enderReceb.cMun);
                cte.Recebedor.Endereco.Cidade.Descricao = infReceb.enderReceb.xMun;
                cte.Recebedor.Endereco.Cidade.SiglaUF = infReceb.enderReceb.UF.ToString("g");
                cte.Recebedor.Endereco.Complemento = infReceb.enderReceb.xCpl;
                cte.Recebedor.Endereco.Logradouro = infReceb.enderReceb.xLgr;
                cte.Recebedor.Endereco.Numero = infReceb.enderReceb.nro;
                cte.Recebedor.Endereco.Telefone = infReceb.fone;
                cte.Recebedor.RGIE = infReceb.IE;
                cte.Recebedor.RazaoSocial = infReceb.xNome;
                cte.Recebedor.Email = infReceb.email;

                if (cte.Recebedor.CPFCNPJ == "00000000000000" && cte.Recebedor.Endereco.Cidade.SiglaUF == "EX")
                {
                    cte.Recebedor.ClienteExterior = true;
                    cte.Recebedor.Endereco.Cidade.Pais = new Dominio.ObjetosDeValor.Embarcador.Localidade.Pais()
                    {
                        NomePais = infReceb.enderReceb.xPais,
                        CodigoPais = infReceb.enderReceb.cPais.ToInt()
                    };
                }
            }
        }

        private void SetarRecebedor(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteReceb infReceb)
        {
            if (infReceb != null)
            {
                cte.Recebedor = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa();
                cte.Recebedor.CPFCNPJ = infReceb.Item;
                cte.Recebedor.Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco();
                cte.Recebedor.Endereco.Bairro = infReceb.enderReceb.xBairro;
                cte.Recebedor.Endereco.CEP = infReceb.enderReceb.CEP;
                cte.Recebedor.Endereco.Cidade = new Dominio.ObjetosDeValor.Localidade();
                cte.Recebedor.Endereco.Cidade.IBGE = int.Parse(infReceb.enderReceb.cMun);
                cte.Recebedor.Endereco.Cidade.Descricao = infReceb.enderReceb.xMun;
                cte.Recebedor.Endereco.Cidade.SiglaUF = infReceb.enderReceb.UF.ToString("g");
                cte.Recebedor.Endereco.Complemento = infReceb.enderReceb.xCpl;
                cte.Recebedor.Endereco.Logradouro = infReceb.enderReceb.xLgr;
                cte.Recebedor.Endereco.Numero = infReceb.enderReceb.nro;
                cte.Recebedor.Endereco.Telefone = infReceb.fone;
                cte.Recebedor.RGIE = infReceb.IE;
                cte.Recebedor.RazaoSocial = infReceb.xNome;
                cte.Recebedor.Email = infReceb.email;

                if (cte.Recebedor.CPFCNPJ == "00000000000000" && cte.Recebedor.Endereco.Cidade.SiglaUF == "EX")
                {
                    cte.Recebedor.ClienteExterior = true;
                    cte.Recebedor.Endereco.Cidade.Pais = new Dominio.ObjetosDeValor.Embarcador.Localidade.Pais()
                    {
                        NomePais = infReceb.enderReceb.xPais,
                        CodigoPais = infReceb.enderReceb.cPais.ToInt()
                    };
                }
            }
        }

        private void SetarRecebedor(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteReceb infReceb)
        {
            if (infReceb != null)
            {
                cte.Recebedor = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa();
                cte.Recebedor.CPFCNPJ = infReceb.Item;
                cte.Recebedor.Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco();
                cte.Recebedor.Endereco.Bairro = infReceb.enderReceb.xBairro;
                cte.Recebedor.Endereco.CEP = infReceb.enderReceb.CEP;
                cte.Recebedor.Endereco.Cidade = new Dominio.ObjetosDeValor.Localidade();
                cte.Recebedor.Endereco.Cidade.IBGE = int.Parse(infReceb.enderReceb.cMun);
                cte.Recebedor.Endereco.Cidade.Descricao = infReceb.enderReceb.xMun;
                cte.Recebedor.Endereco.Cidade.SiglaUF = infReceb.enderReceb.UF.ToString("g");
                cte.Recebedor.Endereco.Complemento = infReceb.enderReceb.xCpl;
                cte.Recebedor.Endereco.Logradouro = infReceb.enderReceb.xLgr;
                cte.Recebedor.Endereco.Numero = infReceb.enderReceb.nro;
                cte.Recebedor.Endereco.Telefone = infReceb.fone;
                cte.Recebedor.RGIE = infReceb.IE;
                cte.Recebedor.RazaoSocial = infReceb.xNome;
                cte.Recebedor.Email = infReceb.email;

                if (cte.Recebedor.CPFCNPJ == "00000000000000" && cte.Recebedor.Endereco.Cidade.SiglaUF == "EX")
                {
                    cte.Recebedor.ClienteExterior = true;
                    cte.Recebedor.Endereco.Cidade.Pais = new Dominio.ObjetosDeValor.Embarcador.Localidade.Pais()
                    {
                        NomePais = infReceb.enderReceb.xPais,
                        CodigoPais = infReceb.enderReceb.cPais.ToInt()
                    };
                }
            }
        }

        private void SetarRecebedor(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v400.ConhecimentoDeTransporteSimplificado.TCTeSimpInfCteToma infReceb)
        {
            if (infReceb != null)
            {
                cte.Recebedor = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa();
                cte.Recebedor.CPFCNPJ = infReceb.Item;
                cte.Recebedor.Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco();
                cte.Recebedor.Endereco.Bairro = infReceb.enderToma.xBairro;
                cte.Recebedor.Endereco.CEP = infReceb.enderToma.CEP;
                cte.Recebedor.Endereco.Cidade = new Dominio.ObjetosDeValor.Localidade();
                cte.Recebedor.Endereco.Cidade.IBGE = int.Parse(infReceb.enderToma.cMun);
                cte.Recebedor.Endereco.Cidade.Descricao = infReceb.enderToma.xMun;
                cte.Recebedor.Endereco.Cidade.SiglaUF = infReceb.enderToma.UF.ToString("g");
                cte.Recebedor.Endereco.Complemento = infReceb.enderToma.xCpl;
                cte.Recebedor.Endereco.Logradouro = infReceb.enderToma.xLgr;
                cte.Recebedor.Endereco.Numero = infReceb.enderToma.nro;
                cte.Recebedor.Endereco.Telefone = infReceb.fone;
                cte.Recebedor.RGIE = infReceb.IE;
                cte.Recebedor.RazaoSocial = infReceb.xNome;
                cte.Recebedor.Email = infReceb.email;

                if (cte.Recebedor.CPFCNPJ == "00000000000000" && cte.Recebedor.Endereco.Cidade.SiglaUF == "EX")
                {
                    cte.Recebedor.ClienteExterior = true;
                    cte.Recebedor.Endereco.Cidade.Pais = new Dominio.ObjetosDeValor.Embarcador.Localidade.Pais()
                    {
                        NomePais = infReceb.enderToma.xPais,
                        CodigoPais = infReceb.enderToma.cPais.ToInt()
                    };
                }
            }
        }

        public static Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa ObterParticipanteCTe(Dominio.Entidades.Cliente infReceb)
        {
            Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa participante = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa();

            participante.CPFCNPJ = infReceb.CPF_CNPJ_SemFormato;
            participante.Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco();
            participante.Endereco.Bairro = infReceb.Bairro;
            participante.Endereco.CEP = infReceb.CEP;
            participante.Endereco.Cidade = new Dominio.ObjetosDeValor.Localidade();
            participante.Endereco.Cidade.IBGE = infReceb.Localidade.CodigoIBGE;
            participante.Endereco.Cidade.Descricao = infReceb.Localidade.Descricao;
            participante.Endereco.Cidade.SiglaUF = infReceb.Localidade.Estado.Sigla;
            participante.Endereco.Complemento = infReceb.Complemento;
            participante.Endereco.Logradouro = infReceb.Endereco;
            participante.Endereco.Numero = infReceb.Numero;
            participante.Endereco.Telefone = infReceb.Telefone1;
            participante.RGIE = infReceb.IE_RG;
            participante.RazaoSocial = infReceb.Nome;
            participante.NomeFantasia = infReceb.NomeFantasia;
            participante.Email = infReceb.Email;

            return participante;
        }

        public static Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa ObterEmpresaCTe(Dominio.Entidades.Cliente infReceb)
        {
            Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa empresa = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa();

            empresa.CNPJ = infReceb.CPF_CNPJ_SemFormato;
            empresa.Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco();
            empresa.Endereco.Bairro = infReceb.Bairro;
            empresa.Endereco.CEP = infReceb.CEP;
            empresa.Endereco.Cidade = new Dominio.ObjetosDeValor.Localidade();
            empresa.Endereco.Cidade.IBGE = infReceb.Localidade.CodigoIBGE;
            empresa.Endereco.Cidade.Descricao = infReceb.Localidade.Descricao;
            empresa.Endereco.Cidade.SiglaUF = infReceb.Localidade.Estado.Sigla;
            empresa.Endereco.Complemento = infReceb.Complemento;
            empresa.Endereco.Logradouro = infReceb.Endereco;
            empresa.Endereco.Numero = infReceb.Numero;
            empresa.Endereco.Telefone = infReceb.Telefone1;
            empresa.IE = infReceb.IE_RG;
            empresa.RazaoSocial = infReceb.Nome;
            empresa.NomeFantasia = infReceb.NomeFantasia;
            empresa.Emails = infReceb.Email;

            return empresa;
        }

        private void ObterInformacoesTomador(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteIde infCTeIde)
        {
            if (infCTeIde.Item != null)
            {
                Type tipoTomador = infCTeIde.Item.GetType();
                if (tipoTomador == typeof(MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteIdeToma03))
                {
                    MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteIdeToma03 tomador = (MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteIdeToma03)infCTeIde.Item;
                    cte.TipoTomador = (Dominio.Enumeradores.TipoTomador)tomador.toma;
                }
                else if (tipoTomador == typeof(MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteIdeToma4))
                {
                    MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteIdeToma4 tomador = (MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteIdeToma4)infCTeIde.Item;
                    cte.TipoTomador = (Dominio.Enumeradores.TipoTomador)tomador.toma;
                    this.SetarTomador(ref cte, tomador);
                }
            }
        }

        private void ObterInformacoesTomador(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteIde infCTeIde)
        {
            if (infCTeIde.Item != null)
            {
                Type tipoTomador = infCTeIde.Item.GetType();
                if (tipoTomador == typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteIdeToma3))
                {
                    MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteIdeToma3 tomador = (MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteIdeToma3)infCTeIde.Item;
                    cte.TipoTomador = (Dominio.Enumeradores.TipoTomador)tomador.toma;
                }
                else if (tipoTomador == typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteIdeToma4))
                {
                    MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteIdeToma4 tomador = (MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteIdeToma4)infCTeIde.Item;
                    cte.TipoTomador = Dominio.Enumeradores.TipoTomador.Outros;//(Dominio.Enumeradores.TipoTomador)tomador.toma;
                    this.SetarTomador(ref cte, tomador);
                }
            }
        }

        private void ObterInformacoesTomador(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIde infCTeIde)
        {
            if (infCTeIde.Item != null)
            {
                Type tipoTomador = infCTeIde.Item.GetType();
                if (tipoTomador == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma3))
                {
                    MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma3 tomador = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma3)infCTeIde.Item;
                    cte.TipoTomador = (Dominio.Enumeradores.TipoTomador)tomador.toma;
                    cte.Tomador = ObterTomador(tomador, cte);
                }
                else if (tipoTomador == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma4))
                {
                    MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma4 tomador = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma4)infCTeIde.Item;
                    cte.TipoTomador = Dominio.Enumeradores.TipoTomador.Outros;//(Dominio.Enumeradores.TipoTomador)tomador.toma;
                    this.SetarTomador(ref cte, tomador);
                }
            }
        }

        private void ObterInformacoesTomador(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v400.ConhecimentoDeTransporteSimplificado.TCTeSimpInfCteToma infCteToma)
        {
            MultiSoftware.CTe.v400.ConhecimentoDeTransporteSimplificado.TCTeSimpInfCteTomaToma tomador = infCteToma.toma;
            if (tomador == MultiSoftware.CTe.v400.ConhecimentoDeTransporteSimplificado.TCTeSimpInfCteTomaToma.Item0)
            {
                cte.TipoTomador = Dominio.Enumeradores.TipoTomador.Remetente;
                this.SetarRemetente(ref cte, infCteToma);
            }
            else if (tomador == MultiSoftware.CTe.v400.ConhecimentoDeTransporteSimplificado.TCTeSimpInfCteTomaToma.Item1)
            {
                cte.TipoTomador = Dominio.Enumeradores.TipoTomador.Expedidor;
                this.SetarExpedidor(ref cte, infCteToma);
            }
            else if (tomador == MultiSoftware.CTe.v400.ConhecimentoDeTransporteSimplificado.TCTeSimpInfCteTomaToma.Item2)
            {
                cte.TipoTomador = Dominio.Enumeradores.TipoTomador.Recebedor;
                this.SetarRecebedor(ref cte, infCteToma);
            }
            else if (tomador == MultiSoftware.CTe.v400.ConhecimentoDeTransporteSimplificado.TCTeSimpInfCteTomaToma.Item3)
            {
                cte.TipoTomador = Dominio.Enumeradores.TipoTomador.Destinatario;
                this.SetarDestinatario(ref cte, infCteToma);
            }
            else if (tomador == MultiSoftware.CTe.v400.ConhecimentoDeTransporteSimplificado.TCTeSimpInfCteTomaToma.Item4)
            {
                cte.TipoTomador = Dominio.Enumeradores.TipoTomador.Outros;
                this.SetarTomador(ref cte, infCteToma);
            }
        }

        public virtual Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa ObterTomador(MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma3 tomador, Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte)
        {
            switch ((Dominio.Enumeradores.TipoTomador)tomador.toma)
            {
                case Dominio.Enumeradores.TipoTomador.Remetente:
                    return cte.Remetente;
                case Dominio.Enumeradores.TipoTomador.Destinatario:
                    return cte.Destinatario;
                case Dominio.Enumeradores.TipoTomador.Outros:
                    return cte.Tomador;
                case Dominio.Enumeradores.TipoTomador.Recebedor:
                    return cte.Recebedor;
                case Dominio.Enumeradores.TipoTomador.Expedidor:
                    return cte.Expedidor;
                default:
                    return null;
            }
        }

        private void SetarTomador(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteIdeToma4 infToma)
        {
            if (infToma != null)
            {
                cte.Tomador = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa();
                cte.Tomador.CPFCNPJ = infToma.Item;
                cte.Tomador.Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco();
                cte.Tomador.Endereco.Bairro = infToma.enderToma.xBairro;
                cte.Tomador.Endereco.CEP = infToma.enderToma.CEP;
                cte.Tomador.Endereco.Cidade = new Dominio.ObjetosDeValor.Localidade();
                cte.Tomador.Endereco.Cidade.IBGE = int.Parse(infToma.enderToma.cMun);
                cte.Tomador.Endereco.Cidade.Descricao = infToma.enderToma.xMun;
                cte.Tomador.Endereco.Cidade.SiglaUF = infToma.enderToma.UF.ToString("g");
                cte.Tomador.Endereco.Complemento = infToma.enderToma.xCpl;
                cte.Tomador.Endereco.Logradouro = infToma.enderToma.xLgr;
                cte.Tomador.Endereco.Numero = infToma.enderToma.nro;
                cte.Tomador.Endereco.Telefone = infToma.fone;
                cte.Tomador.RGIE = infToma.IE;
                cte.Tomador.RazaoSocial = infToma.xNome;
                cte.Tomador.NomeFantasia = infToma.xFant;
                cte.Tomador.Email = infToma.email;
                if (cte.Tomador.CPFCNPJ == "00000000000000")
                    cte.Tomador.ClienteExterior = true;
            }
        }

        private void SetarTomador(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteIdeToma4 infToma)
        {
            if (infToma != null)
            {
                cte.Tomador = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa();
                cte.Tomador.CPFCNPJ = infToma.Item;
                cte.Tomador.Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco();
                cte.Tomador.Endereco.Bairro = infToma.enderToma.xBairro;
                cte.Tomador.Endereco.CEP = infToma.enderToma.CEP;
                cte.Tomador.Endereco.Cidade = new Dominio.ObjetosDeValor.Localidade();
                cte.Tomador.Endereco.Cidade.IBGE = int.Parse(infToma.enderToma.cMun);
                cte.Tomador.Endereco.Cidade.Descricao = infToma.enderToma.xMun;
                cte.Tomador.Endereco.Cidade.SiglaUF = infToma.enderToma.UF.ToString("g");
                cte.Tomador.Endereco.Complemento = infToma.enderToma.xCpl;
                cte.Tomador.Endereco.Logradouro = infToma.enderToma.xLgr;
                cte.Tomador.Endereco.Numero = infToma.enderToma.nro;
                cte.Tomador.Endereco.Telefone = infToma.fone;
                cte.Tomador.RGIE = infToma.IE;
                cte.Tomador.RazaoSocial = infToma.xNome;
                cte.Tomador.NomeFantasia = infToma.xFant;
                cte.Tomador.Email = infToma.email;
                if (cte.Tomador.CPFCNPJ == "00000000000000")
                    cte.Tomador.ClienteExterior = true;
            }
        }

        private void SetarTomador(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma4 infToma)
        {
            if (infToma != null)
            {
                cte.Tomador = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa();
                cte.Tomador.CPFCNPJ = infToma.Item;
                cte.Tomador.Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco();
                cte.Tomador.Endereco.Bairro = infToma.enderToma.xBairro;
                cte.Tomador.Endereco.CEP = infToma.enderToma.CEP;
                cte.Tomador.Endereco.Cidade = new Dominio.ObjetosDeValor.Localidade();
                cte.Tomador.Endereco.Cidade.IBGE = int.Parse(infToma.enderToma.cMun);
                cte.Tomador.Endereco.Cidade.Descricao = infToma.enderToma.xMun;
                cte.Tomador.Endereco.Cidade.SiglaUF = infToma.enderToma.UF.ToString("g");
                cte.Tomador.Endereco.Complemento = infToma.enderToma.xCpl;
                cte.Tomador.Endereco.Logradouro = infToma.enderToma.xLgr;
                cte.Tomador.Endereco.Numero = infToma.enderToma.nro;
                cte.Tomador.Endereco.Telefone = infToma.fone;
                cte.Tomador.RGIE = infToma.IE;
                cte.Tomador.RazaoSocial = infToma.xNome;
                cte.Tomador.NomeFantasia = infToma.xFant;
                cte.Tomador.Email = infToma.email;
                if (cte.Tomador.CPFCNPJ == "00000000000000")
                {
                    cte.Tomador.ClienteExterior = true;
                }
            }
        }

        private void SetarTomador(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v400.ConhecimentoDeTransporteSimplificado.TCTeSimpInfCteToma infToma)
        {
            if (infToma != null)
            {
                cte.Tomador = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa();
                cte.Tomador.CPFCNPJ = infToma.Item;
                cte.Tomador.Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco();
                cte.Tomador.Endereco.Bairro = infToma.enderToma.xBairro;
                cte.Tomador.Endereco.CEP = infToma.enderToma.CEP;
                cte.Tomador.Endereco.Cidade = new Dominio.ObjetosDeValor.Localidade();
                cte.Tomador.Endereco.Cidade.IBGE = int.Parse(infToma.enderToma.cMun);
                cte.Tomador.Endereco.Cidade.Descricao = infToma.enderToma.xMun;
                cte.Tomador.Endereco.Cidade.SiglaUF = infToma.enderToma.UF.ToString("g");
                cte.Tomador.Endereco.Complemento = infToma.enderToma.xCpl;
                cte.Tomador.Endereco.Logradouro = infToma.enderToma.xLgr;
                cte.Tomador.Endereco.Numero = infToma.enderToma.nro;
                cte.Tomador.Endereco.Telefone = infToma.fone;
                cte.Tomador.RGIE = infToma.IE;
                cte.Tomador.RazaoSocial = infToma.xNome;
                cte.Tomador.NomeFantasia = infToma.xNome;
                cte.Tomador.Email = infToma.email;
                if (cte.Tomador.CPFCNPJ == "00000000000000")
                {
                    cte.Tomador.ClienteExterior = true;
                }
            }
        }

        private void ObterICMS(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TCTeInfCteImp infImp)
        {
            if (infImp != null)
            {
                cte.InformacaoAdicionalFisco = infImp.infAdFisco;
                System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");
                var tipoICMS = infImp.ICMS.Item.GetType();
                cte.ValorFrete.ICMS = new Dominio.ObjetosDeValor.Embarcador.ICMS.ICMS();
                if (tipoICMS == typeof(MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TImpICMS00))
                {
                    MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TImpICMS00 impICMS00 = (MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TImpICMS00)infImp.ICMS.Item;
                    cte.ValorFrete.ICMS.Aliquota = decimal.Parse(impICMS00.pICMS, cultura);
                    cte.ValorFrete.ICMS.ValorBaseCalculoICMS = decimal.Parse(impICMS00.vBC, cultura);
                    cte.ValorFrete.ICMS.ValorICMS = decimal.Parse(impICMS00.vICMS, cultura);
                    cte.ValorFrete.ICMS.CST = string.Format("{0:00}", (int)impICMS00.CST);
                }
                else if (tipoICMS == typeof(MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TImpICMS20))
                {
                    MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TImpICMS20 impICMS20 = (MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TImpICMS20)infImp.ICMS.Item;
                    cte.ValorFrete.ICMS.Aliquota = decimal.Parse(impICMS20.pICMS, cultura);
                    cte.ValorFrete.ICMS.ValorBaseCalculoICMS = decimal.Parse(impICMS20.vBC, cultura);
                    cte.ValorFrete.ICMS.ValorICMS = decimal.Parse(impICMS20.vICMS, cultura);
                    cte.ValorFrete.ICMS.CST = string.Format("{0:00}", (int)impICMS20.CST);
                }
                else if (tipoICMS == typeof(MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TImpICMS45))
                {
                    MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TImpICMS45 impICMS45 = (MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TImpICMS45)infImp.ICMS.Item;
                    cte.ValorFrete.ICMS.CST = string.Format("{0:00}", (int)impICMS45.CST);
                }
                else if (tipoICMS == typeof(MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TImpICMS60))
                {
                    MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TImpICMS60 impICMS60 = (MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TImpICMS60)infImp.ICMS.Item;
                    cte.ValorFrete.ICMS.Aliquota = decimal.Parse(impICMS60.pICMSSTRet, cultura);
                    cte.ValorFrete.ICMS.ValorBaseCalculoICMS = decimal.Parse(impICMS60.vBCSTRet, cultura);
                    cte.ValorFrete.ICMS.ValorICMS = decimal.Parse(impICMS60.vICMSSTRet, cultura);
                    cte.ValorFrete.ICMS.ValorCreditoPresumido = impICMS60.vCred != null ? decimal.Parse(impICMS60.vCred, cultura) : 0m;
                    cte.ValorFrete.ICMS.CST = string.Format("{0:00}", (int)impICMS60.CST);
                }
                else if (tipoICMS == typeof(MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TImpICMS90))
                {
                    MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TImpICMS90 impICMS90 = (MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TImpICMS90)infImp.ICMS.Item;
                    cte.ValorFrete.ICMS.Aliquota = decimal.Parse(impICMS90.pICMS, cultura);
                    cte.ValorFrete.ICMS.PercentualReducaoBC = impICMS90.pRedBC != null ? decimal.Parse(impICMS90.pRedBC, cultura) : 0m;
                    cte.ValorFrete.ICMS.ValorBaseCalculoICMS = decimal.Parse(impICMS90.vBC, cultura);
                    cte.ValorFrete.ICMS.ValorICMS = decimal.Parse(impICMS90.vICMS, cultura);
                    cte.ValorFrete.ICMS.ValorCreditoPresumido = impICMS90.vCred != null ? decimal.Parse(impICMS90.vCred, cultura) : 0m;
                    cte.ValorFrete.ICMS.CST = string.Format("{0:00}", (int)impICMS90.CST);
                }
                else if (tipoICMS == typeof(MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TImpICMSOutraUF))
                {
                    MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TImpICMSOutraUF impICMSOutraUF = (MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TImpICMSOutraUF)infImp.ICMS.Item;
                    cte.ValorFrete.ICMS.Aliquota = decimal.Parse(impICMSOutraUF.pICMSOutraUF, cultura);
                    cte.ValorFrete.ICMS.PercentualReducaoBC = impICMSOutraUF.pRedBCOutraUF != null ? decimal.Parse(impICMSOutraUF.pRedBCOutraUF, cultura) : 0m;
                    cte.ValorFrete.ICMS.ValorBaseCalculoICMS = decimal.Parse(impICMSOutraUF.vBCOutraUF, cultura);
                    cte.ValorFrete.ICMS.ValorICMS = decimal.Parse(impICMSOutraUF.vICMSOutraUF, cultura);
                    cte.ValorFrete.ICMS.CST = string.Format("{0:00}", (int)impICMSOutraUF.CST);
                }
                else if (tipoICMS == typeof(MultiSoftware.CTe.v200.ConhecimentoDeTransporte.TImpICMSSN))
                {
                    cte.ValorFrete.ICMS.CST = string.Empty;
                    if (cte.Emitente != null)
                        cte.Emitente.SimplesNacional = true;
                }
            }
        }

        private void ObterICMS(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TCTeInfCteImp infImp)
        {
            if (infImp != null)
            {
                cte.InformacaoAdicionalFisco = infImp.infAdFisco;
                System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");
                var tipoICMS = infImp.ICMS.Item.GetType();
                cte.ValorFrete.ICMS = new Dominio.ObjetosDeValor.Embarcador.ICMS.ICMS();
                if (tipoICMS == typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TImpICMS00))
                {
                    MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TImpICMS00 impICMS00 = (MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TImpICMS00)infImp.ICMS.Item;
                    cte.ValorFrete.ICMS.Aliquota = decimal.Parse(impICMS00.pICMS, cultura);
                    cte.ValorFrete.ICMS.ValorBaseCalculoICMS = decimal.Parse(impICMS00.vBC, cultura);
                    cte.ValorFrete.ICMS.ValorICMS = decimal.Parse(impICMS00.vICMS, cultura);
                    cte.ValorFrete.ICMS.CST = string.Format("{0:00}", (int)impICMS00.CST);
                }
                else if (tipoICMS == typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TImpICMS20))
                {
                    MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TImpICMS20 impICMS20 = (MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TImpICMS20)infImp.ICMS.Item;
                    cte.ValorFrete.ICMS.Aliquota = decimal.Parse(impICMS20.pICMS, cultura);
                    cte.ValorFrete.ICMS.ValorBaseCalculoICMS = decimal.Parse(impICMS20.vBC, cultura);
                    cte.ValorFrete.ICMS.ValorICMS = decimal.Parse(impICMS20.vICMS, cultura);
                    cte.ValorFrete.ICMS.CST = string.Format("{0:00}", (int)impICMS20.CST);
                }
                else if (tipoICMS == typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TImpICMS45))
                {
                    MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TImpICMS45 impICMS45 = (MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TImpICMS45)infImp.ICMS.Item;
                    cte.ValorFrete.ICMS.CST = string.Format("{0:00}", (int)impICMS45.CST);
                }
                else if (tipoICMS == typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TImpICMS60))
                {
                    MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TImpICMS60 impICMS60 = (MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TImpICMS60)infImp.ICMS.Item;
                    cte.ValorFrete.ICMS.Aliquota = decimal.Parse(impICMS60.pICMSSTRet, cultura);
                    cte.ValorFrete.ICMS.ValorBaseCalculoICMS = decimal.Parse(impICMS60.vBCSTRet, cultura);
                    cte.ValorFrete.ICMS.ValorICMS = decimal.Parse(impICMS60.vICMSSTRet, cultura);
                    cte.ValorFrete.ICMS.ValorCreditoPresumido = impICMS60.vCred != null ? decimal.Parse(impICMS60.vCred, cultura) : 0m;
                    cte.ValorFrete.ICMS.CST = string.Format("{0:00}", (int)impICMS60.CST);
                }
                else if (tipoICMS == typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TImpICMS90))
                {
                    MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TImpICMS90 impICMS90 = (MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TImpICMS90)infImp.ICMS.Item;
                    cte.ValorFrete.ICMS.Aliquota = decimal.Parse(impICMS90.pICMS, cultura);
                    cte.ValorFrete.ICMS.PercentualReducaoBC = impICMS90.pRedBC != null ? decimal.Parse(impICMS90.pRedBC, cultura) : 0m;
                    cte.ValorFrete.ICMS.ValorBaseCalculoICMS = decimal.Parse(impICMS90.vBC, cultura);
                    cte.ValorFrete.ICMS.ValorICMS = decimal.Parse(impICMS90.vICMS, cultura);
                    cte.ValorFrete.ICMS.ValorCreditoPresumido = impICMS90.vCred != null ? decimal.Parse(impICMS90.vCred, cultura) : 0m;
                    cte.ValorFrete.ICMS.CST = string.Format("{0:00}", (int)impICMS90.CST);
                }
                else if (tipoICMS == typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TImpICMSOutraUF))
                {
                    MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TImpICMSOutraUF impICMSOutraUF = (MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TImpICMSOutraUF)infImp.ICMS.Item;
                    cte.ValorFrete.ICMS.Aliquota = decimal.Parse(impICMSOutraUF.pICMSOutraUF, cultura);
                    cte.ValorFrete.ICMS.PercentualReducaoBC = impICMSOutraUF.pRedBCOutraUF != null ? decimal.Parse(impICMSOutraUF.pRedBCOutraUF, cultura) : 0m;
                    cte.ValorFrete.ICMS.ValorBaseCalculoICMS = decimal.Parse(impICMSOutraUF.vBCOutraUF, cultura);
                    cte.ValorFrete.ICMS.ValorICMS = decimal.Parse(impICMSOutraUF.vICMSOutraUF, cultura);
                    cte.ValorFrete.ICMS.CST = string.Format("{0:00}", (int)impICMSOutraUF.CST);
                }
                else if (tipoICMS == typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporte.TImpICMSSN))
                {
                    cte.ValorFrete.ICMS.CST = string.Empty;
                    if (cte.Emitente != null)
                        cte.Emitente.SimplesNacional = true;
                }
            }
        }

        private void ObterImpostoIBSCBS(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteImp infImp)
        {
            if (infImp != null && infImp.IBSCBS != null)
            {
                System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

                cte.ValorFrete.IBSCBS = new Dominio.ObjetosDeValor.Embarcador.IBSCBS.IBSCBS()
                {
                    CST = infImp.IBSCBS.CST,
                    ClassificacaoTributaria = infImp.IBSCBS.cClassTrib,
                    BaseCalculo = decimal.Parse(infImp.IBSCBS.gIBSCBS?.vBC ?? "0", cultura),
                    AliquotaIBSEstadual = decimal.Parse(infImp.IBSCBS.gIBSCBS?.gIBSUF?.pIBSUF ?? "0", cultura),
                    PercentualReducaoIBSEstadual = infImp.IBSCBS.gIBSCBS?.gIBSUF?.gRed != null ? decimal.Parse(infImp.IBSCBS.gIBSCBS.gIBSUF.gRed.pRedAliq, cultura) : 0m,
                    ValorIBSEstadual = decimal.Parse(infImp.IBSCBS.gIBSCBS?.gIBSUF?.vIBSUF ?? "0", cultura),
                    AliquotaIBSMunicipal = decimal.Parse(infImp.IBSCBS.gIBSCBS?.gIBSMun?.pIBSMun ?? "0", cultura),
                    PercentualReducaoIBSMunicipal = infImp.IBSCBS.gIBSCBS?.gIBSMun?.gRed != null ? decimal.Parse(infImp.IBSCBS.gIBSCBS.gIBSMun.gRed.pRedAliq, cultura) : 0m,
                    ValorIBSMunicipal = decimal.Parse(infImp.IBSCBS.gIBSCBS?.gIBSMun?.vIBSMun ?? "0", cultura),
                    AliquotaCBS = decimal.Parse(infImp.IBSCBS.gIBSCBS?.gCBS?.pCBS ?? "0", cultura),
                    PercentualReducaoCBS = infImp.IBSCBS.gIBSCBS?.gCBS?.gRed != null ? decimal.Parse(infImp.IBSCBS.gIBSCBS.gCBS.gRed.pRedAliq, cultura) : 0m,
                    ValorCBS = decimal.Parse(infImp.IBSCBS.gIBSCBS?.gCBS?.vCBS ?? "0", cultura),
                };

                cte.ValorFrete.ValorTotalDocumentoFiscal = decimal.Parse(infImp.vTotDFe ?? "0", cultura);
            }
        }

        private void ObterICMS(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteImp infImp)
        {
            if (infImp != null)
            {
                cte.InformacaoAdicionalFisco = infImp.infAdFisco;
                System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");
                var tipoICMS = infImp.ICMS.Item.GetType();
                cte.ValorFrete.ICMS = new Dominio.ObjetosDeValor.Embarcador.ICMS.ICMS();
                if (tipoICMS == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TImpICMS00))
                {
                    MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TImpICMS00 impICMS00 = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TImpICMS00)infImp.ICMS.Item;
                    cte.ValorFrete.ICMS.Aliquota = decimal.Parse(impICMS00.pICMS, cultura);
                    cte.ValorFrete.ICMS.ValorBaseCalculoICMS = decimal.Parse(impICMS00.vBC, cultura);
                    cte.ValorFrete.ICMS.ValorICMS = decimal.Parse(impICMS00.vICMS, cultura);
                    cte.ValorFrete.ICMS.CST = string.Format("{0:00}", (int)impICMS00.CST);
                }
                else if (tipoICMS == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TImpICMS20))
                {
                    MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TImpICMS20 impICMS20 = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TImpICMS20)infImp.ICMS.Item;
                    cte.ValorFrete.ICMS.Aliquota = decimal.Parse(impICMS20.pICMS, cultura);
                    cte.ValorFrete.ICMS.ValorBaseCalculoICMS = decimal.Parse(impICMS20.vBC, cultura);
                    cte.ValorFrete.ICMS.ValorICMS = decimal.Parse(impICMS20.vICMS, cultura);
                    cte.ValorFrete.ICMS.CST = string.Format("{0:00}", (int)impICMS20.CST);
                }
                else if (tipoICMS == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TImpICMS45))
                {
                    MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TImpICMS45 impICMS45 = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TImpICMS45)infImp.ICMS.Item;
                    cte.ValorFrete.ICMS.CST = string.Format("{0:00}", (int)impICMS45.CST);
                }
                else if (tipoICMS == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TImpICMS60))
                {
                    MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TImpICMS60 impICMS60 = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TImpICMS60)infImp.ICMS.Item;
                    cte.ValorFrete.ICMS.Aliquota = decimal.Parse(impICMS60.pICMSSTRet, cultura);
                    cte.ValorFrete.ICMS.ValorBaseCalculoICMS = decimal.Parse(impICMS60.vBCSTRet, cultura);
                    cte.ValorFrete.ICMS.ValorICMS = decimal.Parse(impICMS60.vICMSSTRet, cultura);
                    cte.ValorFrete.ICMS.ValorCreditoPresumido = impICMS60.vCred != null ? decimal.Parse(impICMS60.vCred, cultura) : 0m;
                    cte.ValorFrete.ICMS.CST = string.Format("{0:00}", (int)impICMS60.CST);
                }
                else if (tipoICMS == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TImpICMS90))
                {
                    MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TImpICMS90 impICMS90 = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TImpICMS90)infImp.ICMS.Item;
                    cte.ValorFrete.ICMS.Aliquota = decimal.Parse(impICMS90.pICMS, cultura);
                    cte.ValorFrete.ICMS.PercentualReducaoBC = impICMS90.pRedBC != null ? decimal.Parse(impICMS90.pRedBC, cultura) : 0m;
                    cte.ValorFrete.ICMS.ValorBaseCalculoICMS = decimal.Parse(impICMS90.vBC, cultura);
                    cte.ValorFrete.ICMS.ValorICMS = decimal.Parse(impICMS90.vICMS, cultura);
                    cte.ValorFrete.ICMS.ValorCreditoPresumido = impICMS90.vCred != null ? decimal.Parse(impICMS90.vCred, cultura) : 0m;
                    cte.ValorFrete.ICMS.CST = string.Format("{0:00}", (int)impICMS90.CST);
                }
                else if (tipoICMS == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TImpICMSOutraUF))
                {
                    MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TImpICMSOutraUF impICMSOutraUF = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TImpICMSOutraUF)infImp.ICMS.Item;
                    cte.ValorFrete.ICMS.Aliquota = decimal.Parse(impICMSOutraUF.pICMSOutraUF, cultura);
                    cte.ValorFrete.ICMS.PercentualReducaoBC = impICMSOutraUF.pRedBCOutraUF != null ? decimal.Parse(impICMSOutraUF.pRedBCOutraUF, cultura) : 0m;
                    cte.ValorFrete.ICMS.ValorBaseCalculoICMS = decimal.Parse(impICMSOutraUF.vBCOutraUF, cultura);
                    cte.ValorFrete.ICMS.ValorICMS = decimal.Parse(impICMSOutraUF.vICMSOutraUF, cultura);
                    cte.ValorFrete.ICMS.CST = string.Format("{0:00}", (int)impICMSOutraUF.CST);
                }
                else if (tipoICMS == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TImpICMSSN))
                {
                    cte.ValorFrete.ICMS.CST = string.Empty;
                    if (cte.Emitente != null)
                        cte.Emitente.SimplesNacional = true;
                }
            }
        }

        private void ObterICMS(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, MultiSoftware.CTe.v400.ConhecimentoDeTransporteSimplificado.TCTeSimpInfCteImp infImp)
        {
            if (infImp != null)
            {
                cte.InformacaoAdicionalFisco = infImp.infAdFisco;
                System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");
                var tipoICMS = infImp.ICMS.Item.GetType();
                cte.ValorFrete.ICMS = new Dominio.ObjetosDeValor.Embarcador.ICMS.ICMS();
                if (tipoICMS == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TImpICMS00))
                {
                    MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TImpICMS00 impICMS00 = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TImpICMS00)infImp.ICMS.Item;
                    cte.ValorFrete.ICMS.Aliquota = decimal.Parse(impICMS00.pICMS, cultura);
                    cte.ValorFrete.ICMS.ValorBaseCalculoICMS = decimal.Parse(impICMS00.vBC, cultura);
                    cte.ValorFrete.ICMS.ValorICMS = decimal.Parse(impICMS00.vICMS, cultura);
                    cte.ValorFrete.ICMS.CST = string.Format("{0:00}", (int)impICMS00.CST);
                }
                else if (tipoICMS == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TImpICMS20))
                {
                    MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TImpICMS20 impICMS20 = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TImpICMS20)infImp.ICMS.Item;
                    cte.ValorFrete.ICMS.Aliquota = decimal.Parse(impICMS20.pICMS, cultura);
                    cte.ValorFrete.ICMS.ValorBaseCalculoICMS = decimal.Parse(impICMS20.vBC, cultura);
                    cte.ValorFrete.ICMS.ValorICMS = decimal.Parse(impICMS20.vICMS, cultura);
                    cte.ValorFrete.ICMS.CST = string.Format("{0:00}", (int)impICMS20.CST);
                }
                else if (tipoICMS == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TImpICMS45))
                {
                    MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TImpICMS45 impICMS45 = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TImpICMS45)infImp.ICMS.Item;
                    cte.ValorFrete.ICMS.CST = string.Format("{0:00}", (int)impICMS45.CST);
                }
                else if (tipoICMS == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TImpICMS60))
                {
                    MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TImpICMS60 impICMS60 = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TImpICMS60)infImp.ICMS.Item;
                    cte.ValorFrete.ICMS.Aliquota = decimal.Parse(impICMS60.pICMSSTRet, cultura);
                    cte.ValorFrete.ICMS.ValorBaseCalculoICMS = decimal.Parse(impICMS60.vBCSTRet, cultura);
                    cte.ValorFrete.ICMS.ValorICMS = decimal.Parse(impICMS60.vICMSSTRet, cultura);
                    cte.ValorFrete.ICMS.ValorCreditoPresumido = impICMS60.vCred != null ? decimal.Parse(impICMS60.vCred, cultura) : 0m;
                    cte.ValorFrete.ICMS.CST = string.Format("{0:00}", (int)impICMS60.CST);
                }
                else if (tipoICMS == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TImpICMS90))
                {
                    MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TImpICMS90 impICMS90 = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TImpICMS90)infImp.ICMS.Item;
                    cte.ValorFrete.ICMS.Aliquota = decimal.Parse(impICMS90.pICMS, cultura);
                    cte.ValorFrete.ICMS.PercentualReducaoBC = impICMS90.pRedBC != null ? decimal.Parse(impICMS90.pRedBC, cultura) : 0m;
                    cte.ValorFrete.ICMS.ValorBaseCalculoICMS = decimal.Parse(impICMS90.vBC, cultura);
                    cte.ValorFrete.ICMS.ValorICMS = decimal.Parse(impICMS90.vICMS, cultura);
                    cte.ValorFrete.ICMS.ValorCreditoPresumido = impICMS90.vCred != null ? decimal.Parse(impICMS90.vCred, cultura) : 0m;
                    cte.ValorFrete.ICMS.CST = string.Format("{0:00}", (int)impICMS90.CST);
                }
                else if (tipoICMS == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TImpICMSOutraUF))
                {
                    MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TImpICMSOutraUF impICMSOutraUF = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TImpICMSOutraUF)infImp.ICMS.Item;
                    cte.ValorFrete.ICMS.Aliquota = decimal.Parse(impICMSOutraUF.pICMSOutraUF, cultura);
                    cte.ValorFrete.ICMS.PercentualReducaoBC = impICMSOutraUF.pRedBCOutraUF != null ? decimal.Parse(impICMSOutraUF.pRedBCOutraUF, cultura) : 0m;
                    cte.ValorFrete.ICMS.ValorBaseCalculoICMS = decimal.Parse(impICMSOutraUF.vBCOutraUF, cultura);
                    cte.ValorFrete.ICMS.ValorICMS = decimal.Parse(impICMSOutraUF.vICMSOutraUF, cultura);
                    cte.ValorFrete.ICMS.CST = string.Format("{0:00}", (int)impICMSOutraUF.CST);
                }
                else if (tipoICMS == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TImpICMSSN))
                {
                    cte.ValorFrete.ICMS.CST = string.Empty;
                    if (cte.Emitente != null)
                        cte.Emitente.SimplesNacional = true;
                }
            }
        }

        private string EmitirCTe(int codigoCTe, Repositorio.UnitOfWork unitOfWork)
        {
            string mensagem = "";
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoCTe);

            if (cte != null)
            {
                if (cte.SituacaoCTeSefaz != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Autorizada
                    && cte.SituacaoCTeSefaz != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Cancelada
                    && cte.SituacaoCTeSefaz != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Inutilizada
                    && cte.SituacaoCTeSefaz != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.EmCancelamento
                    && cte.SituacaoCTeSefaz != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.EmInutilizacao)
                {

                    Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);
                    cte.Status = "P";
                    repCTe.Atualizar(cte);

                    Servicos.Embarcador.Carga.CTe serCargaCTE = new Servicos.Embarcador.Carga.CTe(unitOfWork);
                    bool sucesso = svcCTe.Emitir(cte.Codigo, cte.Empresa.Codigo);
                    if (!sucesso)
                    {
                        mensagem = "O CT-e nº " + cte.Numero.ToString() + " da empresa " + cte.Empresa.CNPJ + " foi salvo, porém, ocorreu uma falha ao emiti-lo.";
                    }
                }
                else
                {
                    mensagem = "A atual situação do CT-e (" + cte.DescricaoStatus + ") não permite sua emissão.";
                }
            }
            else
            {
                mensagem = "O CT-e informado não foi localizado";
            }

            return mensagem;
        }

        private void SalvarLogEMP(DeliveryResult<Null, string> deliveryReport)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_stringConexao);

            Repositorio.Embarcador.Integracao.IntegracaoEMPLog repIntegracaoEMPLog = new Repositorio.Embarcador.Integracao.IntegracaoEMPLog(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo repositorioConfiguracaoAquivo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoArquivo configuracaoArquivo = repositorioConfiguracaoAquivo.BuscarPrimeiroRegistro();
            string caminho = Utilidades.IO.FileStorageService.Storage.Combine(configuracaoArquivo.CaminhoArquivosIntegracao, "IntegracaoEMPLog");

            string caminhoArquivoEnvio = Utilidades.IO.FileStorageService.Storage.Combine(caminho, Guid.NewGuid().ToString());
            Utilidades.IO.FileStorageService.Storage.WriteAllText(caminhoArquivoEnvio + ".json", deliveryReport.Message.Value);

            string caminhoArquivoRetorno = Utilidades.IO.FileStorageService.Storage.Combine(caminho, Guid.NewGuid().ToString());
            Utilidades.IO.FileStorageService.Storage.WriteAllText(caminhoArquivoRetorno + ".json", deliveryReport.Value);

            Dominio.Entidades.Embarcador.Integracao.IntegracaoEMPLog integracaoEMPLog = new Dominio.Entidades.Embarcador.Integracao.IntegracaoEMPLog()
            {
                ArquivoEnvio = caminhoArquivoEnvio,
                ArquivoRetorno = caminhoArquivoRetorno,
                DataEnvio = DateTime.Now,
                MensageRetorno = deliveryReport.Message.Value,
                StatusIntegracaoEMP = deliveryReport.Status == PersistenceStatus.NotPersisted ? StatusIntegracaoEMP.NotPersisted : deliveryReport.Status == PersistenceStatus.Persisted ? StatusIntegracaoEMP.Persisted : StatusIntegracaoEMP.PossiblyPersisted,
                Topic = deliveryReport.Topic
            };
            repIntegracaoEMPLog.Inserir(integracaoEMPLog);
        }

        private void handlerCteAnterior(DeliveryReport<Null, string> deliveryReport)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_stringConexao);

            Repositorio.Embarcador.Integracao.IntegracaoEMPLog repIntegracaoEMPLog = new Repositorio.Embarcador.Integracao.IntegracaoEMPLog(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo repositorioConfiguracaoAquivo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoArquivo configuracaoArquivo = repositorioConfiguracaoAquivo.BuscarPrimeiroRegistro();
            string caminho = Utilidades.IO.FileStorageService.Storage.Combine(configuracaoArquivo.CaminhoArquivosIntegracao, "IntegracaoEMPLog");

            string caminhoArquivoEnvio = Utilidades.IO.FileStorageService.Storage.Combine(caminho, Guid.NewGuid().ToString());
            Utilidades.IO.FileStorageService.Storage.WriteAllText(caminhoArquivoEnvio + ".json", deliveryReport.Message.Value);

            string caminhoArquivoRetorno = Utilidades.IO.FileStorageService.Storage.Combine(caminho, Guid.NewGuid().ToString());
            Utilidades.IO.FileStorageService.Storage.WriteAllText(caminhoArquivoRetorno + ".json", deliveryReport.Value);

            Dominio.Entidades.Embarcador.Integracao.IntegracaoEMPLog integracaoEMPLog = new Dominio.Entidades.Embarcador.Integracao.IntegracaoEMPLog()
            {
                ArquivoEnvio = caminhoArquivoEnvio,
                ArquivoRetorno = caminhoArquivoRetorno,
                DataEnvio = DateTime.Now,
                MensageRetorno = deliveryReport.Error?.Reason,
                StatusIntegracaoEMP = deliveryReport.Status == PersistenceStatus.NotPersisted ? StatusIntegracaoEMP.NotPersisted : deliveryReport.Status == PersistenceStatus.Persisted ? StatusIntegracaoEMP.Persisted : StatusIntegracaoEMP.PossiblyPersisted,
                Topic = deliveryReport.Topic
            };
            repIntegracaoEMPLog.Inserir(integracaoEMPLog);

        }

        private void handler(DeliveryReport<Null, string> deliveryReport)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_stringConexao);

            Repositorio.Embarcador.Integracao.IntegracaoEMPLog repIntegracaoEMPLog = new Repositorio.Embarcador.Integracao.IntegracaoEMPLog(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo repositorioConfiguracaoAquivo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoArquivo configuracaoArquivo = repositorioConfiguracaoAquivo.BuscarPrimeiroRegistro();
            string caminho = Utilidades.IO.FileStorageService.Storage.Combine(configuracaoArquivo.CaminhoArquivosIntegracao, "IntegracaoEMPLog");

            string caminhoArquivoEnvio = Utilidades.IO.FileStorageService.Storage.Combine(caminho, Guid.NewGuid().ToString());
            Utilidades.IO.FileStorageService.Storage.WriteAllText(caminhoArquivoEnvio + ".json", deliveryReport.Message.Value);

            string caminhoArquivoRetorno = Utilidades.IO.FileStorageService.Storage.Combine(caminho, Guid.NewGuid().ToString());
            Utilidades.IO.FileStorageService.Storage.WriteAllText(caminhoArquivoRetorno + ".json", deliveryReport.Value);

            Dominio.Entidades.Embarcador.Integracao.IntegracaoEMPLog integracaoEMPLog = new Dominio.Entidades.Embarcador.Integracao.IntegracaoEMPLog()
            {
                ArquivoEnvio = caminhoArquivoEnvio,
                ArquivoRetorno = caminhoArquivoRetorno,
                DataEnvio = DateTime.Now,
                MensageRetorno = deliveryReport.Error?.Reason,
                StatusIntegracaoEMP = deliveryReport.Status == PersistenceStatus.NotPersisted ? StatusIntegracaoEMP.NotPersisted : deliveryReport.Status == PersistenceStatus.Persisted ? StatusIntegracaoEMP.Persisted : StatusIntegracaoEMP.PossiblyPersisted,
                Topic = deliveryReport.Topic
            };
            repIntegracaoEMPLog.Inserir(integracaoEMPLog);

        }

        private void GerarCanhotoNotaFiscal(Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNota, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ConfiguracaoEmbarcador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Canhotos.Canhoto servicoCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto repConfiguracaoCanhoto = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto configuracaoCanhoto = repConfiguracaoCanhoto.BuscarConfiguracaoPadrao();

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[] situacoesCarga = new Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[]
            {
                  Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte,
                  Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada
            };

            if (ConfiguracaoEmbarcador.GerarCanhotoSempre ||
                                (pedidoXMLNota.CargaPedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente && ((pedidoXMLNota.CargaPedido.Pedido.Remetente?.ExigeCanhotoFisico ?? false) || (pedidoXMLNota.CargaPedido.Pedido.Remetente?.GrupoPessoas?.ExigeCanhotoFisico ?? false))) ||
                                (pedidoXMLNota.CargaPedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario && ((pedidoXMLNota.CargaPedido.Pedido.Destinatario?.ExigeCanhotoFisico ?? false) || (pedidoXMLNota.CargaPedido.Pedido.Destinatario?.GrupoPessoas?.ExigeCanhotoFisico ?? false))) ||
                                (pedidoXMLNota.CargaPedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor && ((pedidoXMLNota.CargaPedido.Expedidor?.ExigeCanhotoFisico ?? false) || (pedidoXMLNota.CargaPedido.Expedidor?.GrupoPessoas?.ExigeCanhotoFisico ?? false))) ||
                                (pedidoXMLNota.CargaPedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor && ((pedidoXMLNota.CargaPedido.Recebedor?.ExigeCanhotoFisico ?? false) || (pedidoXMLNota.CargaPedido.Recebedor?.GrupoPessoas?.ExigeCanhotoFisico ?? false))) ||
                                (pedidoXMLNota.CargaPedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros && ((pedidoXMLNota.CargaPedido.Tomador?.ExigeCanhotoFisico ?? false) || (pedidoXMLNota.CargaPedido.Tomador?.GrupoPessoas?.ExigeCanhotoFisico ?? false))))
            {
                bool nfeAtiva = pedidoXMLNota.XMLNotaFiscal.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.NFe && (pedidoXMLNota.XMLNotaFiscal?.nfAtiva ?? false);
                bool status = situacoesCarga.Contains(pedidoXMLNota.CargaPedido.Carga.SituacaoCarga);
                bool naoEhTransbordo = (pedidoXMLNota.CargaPedido.Carga?.CargaTransbordo ?? false) == false;
                bool tipoFreteDiferenteCliente = pedidoXMLNota.CargaPedido.Carga.TipoFreteEscolhido != TipoFreteEscolhido.Cliente;

                if (nfeAtiva && status && naoEhTransbordo && tipoFreteDiferenteCliente)
                    servicoCanhoto.SalvarCanhotoNota(pedidoXMLNota.XMLNotaFiscal, pedidoXMLNota.CargaPedido, pedidoXMLNota.CargaPedido.Carga.FreteDeTerceiro ? pedidoXMLNota.CargaPedido.Carga.Terceiro : null, pedidoXMLNota.CargaPedido.Carga.Motoristas.ToList(), tipoServicoMultisoftware, ConfiguracaoEmbarcador, unitOfWork, configuracaoCanhoto);
            }
        }

        private void GerarCanhotoCTe(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ConfiguracaoEmbarcador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Canhotos.Canhoto servicoCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[] situacoesCarga = new Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[]
            {
                  Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte,
                   Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada
            };

            bool armazenaCanhotoFisicoCTeCliente = (bool)(cargaCTe.CTe.TomadorPagador?.Cliente?.ArmazenaCanhotoFisicoCTe ?? false);
            bool armazenaCanhotoFisicoCTeGrupoPessoas = (bool)(cargaCTe.CTe.TomadorPagador?.Cliente?.GrupoPessoas?.ArmazenaCanhotoFisicoCTe ?? false);

            if (ConfiguracaoEmbarcador.GerarCanhotoSempre || armazenaCanhotoFisicoCTeCliente || armazenaCanhotoFisicoCTeGrupoPessoas)
            {
                bool naoEhNfs = cargaCTe.CTe.ModeloDocumentoFiscal?.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.NFS;
                bool status = (cargaCTe.CTe.Status == "A" && situacoesCarga.Contains(cargaCTe.Carga.SituacaoCarga));
                bool naoEhTransbordo = (cargaCTe.Carga?.CargaTransbordo ?? false) == false;

                if (naoEhNfs && status && naoEhTransbordo)
                    servicoCanhoto.SalvarCanhotoCargaCTe(cargaCTe, tipoServicoMultisoftware, unitOfWork);
            }
        }

        private static void VerificarRegraICMSSubstituicao(Dominio.Entidades.ConhecimentoDeTransporteEletronico cteSubstituicao, Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTeOriginal, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Servicos.Embarcador.Carga.ICMS svcICMS = new Servicos.Embarcador.Carga.ICMS();
            Repositorio.CFOP repCFOP = new Repositorio.CFOP(unitOfWork);

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.ICMS.RegraICMS repRegraICMS = new Repositorio.Embarcador.ICMS.RegraICMS(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repConfiguracaoTMS.BuscarPrimeiroRegistro();

            bool incluirICMS = false;
            decimal percentualICMSIncluir = 0;

            Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraICMS = svcICMS.BuscarRegraICMS(cargaCTeOriginal.Carga, null, cteSubstituicao.Empresa, cteSubstituicao.Expedidor?.Cliente ?? cteSubstituicao.Remetente.Cliente, cteSubstituicao.Recebedor?.Cliente ?? cteSubstituicao.Destinatario.Cliente, cteSubstituicao.TomadorPagador.Cliente, cteSubstituicao.LocalidadeInicioPrestacao, cteSubstituicao.LocalidadeTerminoPrestacao, ref incluirICMS, ref percentualICMSIncluir, cteSubstituicao.ValorAReceber, null, unitOfWork, tipoServicoMultisoftware, configuracaoEmbarcador, cargaCTeOriginal.Carga.TipoContratacaoCarga, true, tipoCTE: Dominio.Enumeradores.TipoCTE.Substituto);

            if (regraICMS == null)
                return;

            cteSubstituicao.SimplesNacional = regraICMS.SimplesNacional ? Dominio.Enumeradores.OpcaoSimNao.Sim : Dominio.Enumeradores.OpcaoSimNao.Nao;
            cteSubstituicao.AliquotaICMS = regraICMS.Aliquota;
            cteSubstituicao.AliquotaICMSSimples = regraICMS.AliquotaSimples;
            cteSubstituicao.ValorICMS = regraICMS.ValorICMS;
            cteSubstituicao.BaseCalculoICMS = regraICMS.ValorBaseCalculoICMS;
            cteSubstituicao.CST = regraICMS.CST;
            cteSubstituicao.CFOP = repCFOP.BuscarPorNumero(regraICMS.CFOP);
            cteSubstituicao.PercentualICMSIncluirNoFrete = regraICMS.PercentualInclusaoBC;
            cteSubstituicao.IncluirICMSNoFrete = regraICMS.IncluirICMSBC ? Dominio.Enumeradores.OpcaoSimNao.Sim : Dominio.Enumeradores.OpcaoSimNao.Nao;
            cteSubstituicao.PercentualReducaoBaseCalculoICMS = regraICMS.PercentualReducaoBC;

            if (cteSubstituicao.RegraICMS == null || cteSubstituicao.RegraICMS.Codigo != regraICMS.CodigoRegra)
                cteSubstituicao.RegraICMS = repRegraICMS.BuscarPorCodigo(regraICMS.CodigoRegra);
        }

        #endregion Métodos Privados
    }
}
