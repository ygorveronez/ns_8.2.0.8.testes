using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class IntegracaoNaturaController : ApiController
    {
        #region Métodos Globais

        [AcceptVerbs("POST")]
        public ActionResult ConsultarIntegracoesDT()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int numeroDocumentoTransporte;
                int.TryParse(Utilidades.String.OnlyNumbers(Request.Params["NumeroDocumentoTransporte"]), out numeroDocumentoTransporte);

                Repositorio.DocumentoTransporteNatura repDocumento = new Repositorio.DocumentoTransporteNatura(unidadeDeTrabalho);
                Dominio.Entidades.DocumentoTransporteNatura DT = repDocumento.BuscarPorCodigo(numeroDocumentoTransporte);

                if (DT != null && DT.NaturaXMLs != null && DT.NaturaXMLs.Count > 0)
                {
                    List<Dominio.Entidades.NaturaXML> listaNaturaXML = new List<Dominio.Entidades.NaturaXML>();
                    foreach (Dominio.Entidades.NaturaXML xml in DT.NaturaXMLs)
                    {
                        if (xml != null)
                        {
                            if (xml.Tipo == Dominio.ObjetosDeValor.Enumerador.TipoXMLNatura.RetornoDocumentoTransporte || xml.Tipo == Dominio.ObjetosDeValor.Enumerador.TipoXMLNatura.RetornoDocumentoTransporteComplementar)
                                listaNaturaXML.Add(xml);
                        }
                    }

                    int countDocumentos = listaNaturaXML.Count();

                    var retorno = (from naturaXML in listaNaturaXML
                                   select new
                                   {
                                       Codigo = naturaXML.Codigo,
                                       Tipo = naturaXML.Tipo,
                                       Data = naturaXML.Data.ToString("dd/MM/yyyy HH:mm"),
                                       naturaXML.Usuario.Nome,
                                       naturaXML.DescricaoTipo,
                                       Mensagem = naturaXML.Mensagem ?? string.Empty
                                   }).OrderByDescending(o => o.Codigo).ToList();

                    return Json(retorno, true, null, new string[] { "Codigo", "Tipo", "Data|15", "Usuario|20", "Tipo|20", "Mensagem|25" }, countDocumentos);
                }
                else
                {
                    return Json(string.Empty.ToList(), true, null, new string[] { "Codigo", "Tipo", "Data|15", "Usuario|20", "Tipo|20", "Mensagem|25" }, 0);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao consultar as integrações para o documento de transporte.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }

        }

        [AcceptVerbs("POST")]
        public ActionResult ConsultarCTesEmitidos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int numeroDocumentoTransporte;
                int.TryParse(Utilidades.String.OnlyNumbers(Request.Params["NumeroDocumentoTransporte"]), out numeroDocumentoTransporte);


                Repositorio.DocumentoTransporteNatura repDocumento = new Repositorio.DocumentoTransporteNatura(unitOfWork);

                Dominio.Entidades.DocumentoTransporteNatura DT = repDocumento.BuscarPorCodigo(numeroDocumentoTransporte);

                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
                foreach (Dominio.Entidades.NotaFiscalDocumentoTransporteNatura nfNatura in DT.NotasFiscais)
                {
                    if (nfNatura.CTe != null)
                    {
                        if (!ctes.Contains(nfNatura.CTe))
                            ctes.Add(nfNatura.CTe);
                    }

                }

                int countDocumentos = ctes.Count();

                var retorno = (from cte in ctes
                               select new
                               {
                                   CodigoCriptografado = Servicos.Criptografia.Criptografar(cte.Codigo.ToString(), "CT3##MULT1@#$S0FTW4R3"),
                                   CodigoEmpresa = cte.Empresa.Codigo,
                                   cte.Codigo,
                                   cte.Status,
                                   cte.Numero,
                                   Serie = cte.Serie.Numero,
                                   DataEmissao = cte.DataEmissao.HasValue ? cte.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                                   cte.DescricaoTipoCTE,
                                   LocalidadeRemetente = cte.Remetente != null ? cte.Remetente.Exterior ? string.Concat(cte.Remetente.Cidade, " / ", cte.Remetente.Pais.Nome) : string.Concat(cte.Remetente.Localidade.Estado.Sigla, " / ", cte.Remetente.Localidade.Descricao) : string.Empty,
                                   LocalidadeDestinatario = cte.Destinatario != null ? cte.Destinatario.Exterior ? string.Concat(cte.Destinatario.Cidade, " / ", cte.Destinatario.Pais.Nome) : string.Concat(cte.Destinatario.Localidade.Estado.Sigla, " / ", cte.Destinatario.Localidade.Descricao) : string.Empty,
                                   Valor = string.Format("{0:n2}", cte.ValorAReceber),
                                   cte.DescricaoStatus,
                                   MensagemRetornoSefaz = cte.MensagemStatus == null ? string.IsNullOrEmpty(cte.MensagemRetornoSefaz) ? string.Empty : System.Web.HttpUtility.HtmlEncode(cte.MensagemRetornoSefaz) : cte.MensagemStatus.MensagemDoErro
                               }).OrderByDescending(o => o.Numero).ToList();

                return Json(retorno, true, null, new string[] { "CodigoCriptografado", "CodigoEmpresa", "Codigo", "Status", "Núm.|6", "Série|5", "Emissão|8", "Tipo CT-e|12", "Loc. Remet.|13", "Loc. Destin.|13", "Valor|9", "Status|12", "Retorno Sefaz|15" }, countDocumentos);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao consultar os CT-es emitidos para o documento de transporte.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ConsultarNFSesEmitidas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int numeroDocumentoTransporte;
                int.TryParse(Utilidades.String.OnlyNumbers(Request.Params["NumeroDocumentoTransporte"]), out numeroDocumentoTransporte);


                Repositorio.DocumentoTransporteNatura repDocumento = new Repositorio.DocumentoTransporteNatura(unitOfWork);

                Dominio.Entidades.DocumentoTransporteNatura DT = repDocumento.BuscarPorCodigo(numeroDocumentoTransporte);

                List<Dominio.Entidades.NFSe> nfses = new List<Dominio.Entidades.NFSe>();

                foreach (Dominio.Entidades.NotaFiscalDocumentoTransporteNatura nfNatura in DT.NotasFiscais)
                {
                    if (nfNatura.NFSe != null)
                    {
                        if (!nfses.Contains(nfNatura.NFSe))
                            nfses.Add(nfNatura.NFSe);
                    }

                }

                int countDocumentos = nfses.Count();

                var retorno = (from nfse in nfses
                               select new
                               {
                                   nfse.Codigo,
                                   nfse.Status,
                                   nfse.Numero,
                                   Serie = nfse.Serie.Numero,
                                   DataEmissao = nfse.DataEmissao.ToString("dd/MM/yyyy HH:mm"),
                                   LocalidadePrestacao = string.Concat(nfse.LocalidadePrestacaoServico.Estado.Sigla, " / ", nfse.LocalidadePrestacaoServico.Descricao),
                                   Valor = string.Format("{0:n2}", nfse.ValorServicos),
                                   nfse.DescricaoStatus,
                                   MensagemRetorno = System.Web.HttpUtility.HtmlEncode(nfse.RPS?.MensagemRetorno ?? string.Empty),
                               }).ToList();

                return Json(retorno, true, null, new string[] { "Codigo", "Status", "Núm.|8", "Série|8", "Emissão|12", "Loc. Prest.|16", "Valor|12", "Status|12", "Retorno|16" }, countDocumentos);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao consultar as NFS-es emitidas para o documento de transporte.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                long numeroDocumentoTransporte;
                long.TryParse(Utilidades.String.OnlyNumbers(Request.Params["NumeroDocumentoTransporte"]), out numeroDocumentoTransporte);

                int numeroNFe;
                int.TryParse(Utilidades.String.OnlyNumbers(Request.Params["NumeroNFe"]), out numeroNFe);

                DateTime dataInicial, dataFinal;
                DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                int inicioRegistros;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);

                Dominio.ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura statusDTAux;
                Dominio.ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura? statusDT = null;
                if (Enum.TryParse<Dominio.ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura>(Request.Params["StatusDT"], out statusDTAux))
                    statusDT = statusDTAux;

                Repositorio.DocumentoTransporteNatura repDocumento = new Repositorio.DocumentoTransporteNatura(unitOfWork);

                List<Dominio.Entidades.DocumentoTransporteNatura> documentos = repDocumento.Consultar(this.EmpresaUsuario.Codigo, numeroDocumentoTransporte, dataInicial, dataFinal, numeroNFe, statusDT, inicioRegistros, 50);

                int countDocumentos = repDocumento.ContarConsulta(this.EmpresaUsuario.Codigo, numeroDocumentoTransporte, dataInicial, dataFinal, numeroNFe, statusDT);

                Repositorio.NotaFiscalDocumentoTransporteNatura repNotas = new Repositorio.NotaFiscalDocumentoTransporteNatura(unitOfWork);

                var retorno = (from obj in documentos
                               select new
                               {
                                   obj.Codigo,
                                   obj.NumeroDT,
                                   NotasFiscais = repNotas.BuscarNotasPorDocumentoTransporte(obj.Codigo),
                                   DataEmissao = obj.DataEmissao.ToString("dd/MM/yyyy HH:mm"),
                                   Motorista = obj.Motorista?.Nome ?? string.Empty,
                                   Veiculo = obj.Veiculo?.Placa ?? string.Empty,
                                   ValorFrete = obj.Status == Dominio.ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura.EmDigitacao || obj.Status == Dominio.ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura.AguardandoEmissaoAutomatica ? obj.NotasFiscais.Sum(nf => nf.ValorFrete).ToString("n2") + " (Valor Natura)" : obj.ValorFrete.ToString("n2"),
                                   //ValorAReceber = obj.NotasFiscais.Where(o => o.CTe != null).Sum(o => o.CTe.ValorAReceber).ToString("n2"),
                                   Status = obj.Status == Dominio.ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura.Erro ? obj.DescricaoStatus + " (" + obj.Observacao + ")" : obj.DescricaoStatus
                               }).ToList();

                return Json(retorno, true, "", new string[] { "Código", "Número|10", "Notas Fiscais|15", "Data Emissão|15", "Motorista|15", "Veículo|10", "Valor Frete|14", "Status|12" }, countDocumentos); //"Valor Receber|12",
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao consultar os documentos de transporte.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ConsultarDocumentosTransporte()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                DateTime dataInicial, dataFinal;
                DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                long numeroDT;
                long.TryParse(Request.Params["NumeroDT"].Replace(".", ""), out numeroDT);

                bool atualizaDT = false;
                bool.TryParse(Request.Params["AtualizaDT"], out atualizaDT);

                Servicos.Natura svcNatura = new Servicos.Natura(unidadeDeTrabalho);

                svcNatura.ConsultarDocumentosTransporte(this.EmpresaUsuario.Codigo, numeroDT, dataInicial, dataFinal, unidadeDeTrabalho, this.UsuarioAdministrativo != null ? this.UsuarioAdministrativo.Codigo : this.Usuario.Codigo, atualizaDT);

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao consultar os documentos de transporte.");
            }
        }


        public Dominio.ObjetosDeValor.CTe.CTe GerarObjetoCTeComplementarDoCTePai(Dominio.Entidades.ConhecimentoDeTransporteEletronico ctePai, decimal valorComplemento, string observacao, bool incluirICMSFrete, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Cliente serCliente = new Servicos.Cliente(Conexao.StringConexao);
            Servicos.CTe serCTe = new Servicos.CTe(unitOfWork);
            Servicos.Veiculo serVeiculo = new Servicos.Veiculo(unitOfWork);
            Servicos.CTe servicoCte = new Servicos.CTe(unitOfWork);
            Servicos.Embarcador.Carga.CTe serCargaCTe = new Servicos.Embarcador.Carga.CTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTeComplementar = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);

            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);

            Dominio.ObjetosDeValor.CTe.CTe cte = new Dominio.ObjetosDeValor.CTe.CTe();

            cte.Documentos = new List<Dominio.ObjetosDeValor.CTe.Documento>();
            Dominio.ObjetosDeValor.CTe.Documento documento = new Dominio.ObjetosDeValor.CTe.Documento();
            documento.Descricao = "Complemento";
            documento.Numero = "0";
            documento.ModeloDocumentoFiscal = "00";
            documento.Tipo = Dominio.Enumeradores.TipoDocumentoCTe.Outros;
            documento.DataEmissao = DateTime.Now.ToString("dd/MM/yyyy");

            cte.Documentos.Add(documento);
            cte.ValorTotalMercadoria = 0;
            cte.ChaveCTESubstituicaoComplementar = ctePai.Chave;

            cte.Serie = ctePai.Empresa.Configuracao != null && ctePai.Empresa.Configuracao.SerieInterestadual != null ? ctePai.Empresa.Configuracao.SerieInterestadual.Numero : ctePai.Serie.Numero;

            cte.ValorAReceber += valorComplemento;
            cte.ValorTotalPrestacaoServico += valorComplemento;
            cte.ValorFrete += valorComplemento;
            cte.IncluirICMSNoFrete = incluirICMSFrete ? Dominio.Enumeradores.OpcaoSimNao.Sim : Dominio.Enumeradores.OpcaoSimNao.Nao;
            cte.ObservacoesGerais = observacao;

            if (!string.IsNullOrWhiteSpace(cte.ObservacoesGerais))
                cte.ObservacoesGerais = string.Concat(cte.ObservacoesGerais, " / ", "CT-e emitido como complemento ao CT-e ", ctePai.Chave, ".");
            else
                cte.ObservacoesGerais = string.Concat("CT-e emitido como complemento ao CT-e ", ctePai.Chave, ".");

            cte.Remetente = serCliente.ObterClienteCTE(ctePai.Remetente);
            cte.Destinatario = serCliente.ObterClienteCTE(ctePai.Destinatario);
            cte.Emitente = Servicos.Empresa.ObterEmpresaCTE(ctePai.Empresa);
            cte.Expedidor = serCliente.ObterClienteCTE(ctePai.Expedidor);
            cte.Recebedor = serCliente.ObterClienteCTE(ctePai.Recebedor);

            cte.CFOP = ctePai.CFOP.CodigoCFOP;

            //cte.DataEmissao = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            TimeZoneInfo fusoHorarioEmpresa = TimeZoneInfo.FindSystemTimeZoneById(ctePai.Empresa.FusoHorario);
            DateTime dataFuso = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);
            if (fusoHorarioEmpresa != TimeZoneInfo.Local)
                dataFuso = TimeZoneInfo.ConvertTime(dataFuso, TimeZoneInfo.Local, fusoHorarioEmpresa);
            cte.DataEmissao = dataFuso.ToString("dd/MM/yyyy HH:mm");

            cte.Lotacao = ctePai.Lotacao;

            if (ctePai.Veiculos.Count > 0)
            {
                cte.Veiculos = new List<Dominio.ObjetosDeValor.CTe.Veiculo>();
                foreach (Dominio.Entidades.VeiculoCTE veiculoCte in ctePai.Veiculos)
                {
                    cte.Veiculos.Add(serVeiculo.ObterVeiculoCTE(veiculoCte));
                }
            }

            if (ctePai.Motoristas.Count > 0)
            {
                cte.Motoristas = new List<Dominio.ObjetosDeValor.CTe.Motorista>();
                foreach (Dominio.Entidades.MotoristaCTE motoristaCTe in ctePai.Motoristas)
                {
                    Dominio.ObjetosDeValor.CTe.Motorista motorista = new Dominio.ObjetosDeValor.CTe.Motorista();
                    motorista.CPF = motoristaCTe.CPFMotorista;
                    motorista.Nome = motoristaCTe.NomeMotorista;
                    cte.Motoristas.Add(motorista);
                }
            }

            cte.ProdutoPredominante = ctePai.ProdutoPredominante;

            cte.QuantidadesCarga = new List<Dominio.ObjetosDeValor.CTe.QuantidadeCarga>();
            Dominio.ObjetosDeValor.CTe.QuantidadeCarga quantidadeCarga = new Dominio.ObjetosDeValor.CTe.QuantidadeCarga();
            quantidadeCarga.UnidadeMedida = "01";
            quantidadeCarga.Quantidade = 0;
            quantidadeCarga.Descricao = "Kilograma";
            cte.QuantidadesCarga.Add(quantidadeCarga);

            cte.Retira = ctePai.Retira;

            if (ctePai.Seguros.Count > 0)
            {
                cte.Seguros = new List<Dominio.ObjetosDeValor.CTe.Seguro>();
                foreach (Dominio.Entidades.SeguroCTE seguroCTe in ctePai.Seguros)
                {
                    Dominio.ObjetosDeValor.CTe.Seguro seguro = new Dominio.ObjetosDeValor.CTe.Seguro()
                    {
                        NomeSeguradora = seguroCTe.NomeSeguradora,
                        NumeroAverbacao = seguroCTe.NumeroAverbacao,
                        Tipo = seguroCTe.Tipo,
                        NumeroApolice = seguroCTe.NumeroApolice,
                        Valor = seguroCTe.Valor
                    };
                    cte.Seguros.Add(seguro);
                }
            }

            cte.TipoCTe = Dominio.Enumeradores.TipoCTE.Complemento;
            cte.TipoImpressao = ctePai.Empresa.Configuracao != null && ctePai.Empresa.Configuracao.TipoImpressao != 0 ? ctePai.Empresa.Configuracao.TipoImpressao : ctePai.TipoImpressao;
            cte.TipoPagamento = ctePai.TipoPagamento;

            cte.CodigoIBGECidadeInicioPrestacao = ctePai.LocalidadeInicioPrestacao.CodigoIBGE;
            cte.CodigoIBGECidadeTerminoPrestacao = ctePai.LocalidadeTerminoPrestacao.CodigoIBGE;

            cte.TipoServico = ctePai.TipoServico;

            cte.TipoTomador = ctePai.TipoTomador;
            cte.Tomador = serCliente.ObterClienteCTE(ctePai.Tomador);
            cte.PercentualICMSIncluirNoFrete = ctePai.PercentualICMSIncluirNoFrete;

            return cte;
        }


        [AcceptVerbs("POST")]
        public ActionResult EmitirCTeComplentar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                unidadeDeTrabalho.Start();

                int codigoDocumentoTransporte = int.Parse(Request.Params["NumeroDocumentoTransporte"]);
                int CodigoCte = int.Parse(Request.Params["CodigoCte"]);
                decimal valorComplemento = decimal.Parse(Request.Params["ValorComplemento"]);
                string observacao = Request.Params["Observacao"];
                bool incluirICMSFrete = bool.Parse(Request.Params["IncluirICMSFrete"]);

                Repositorio.NotaFiscalDocumentoTransporteNatura repNotaFiscalDocumentoTransporteNatura = new Repositorio.NotaFiscalDocumentoTransporteNatura(unidadeDeTrabalho);
                Servicos.CTe servicoCte = new Servicos.CTe(unidadeDeTrabalho);
                Servicos.Embarcador.Carga.CTeComplementar serCTeComplementar = new Servicos.Embarcador.Carga.CTeComplementar(unidadeDeTrabalho);

                Dominio.Entidades.NotaFiscalDocumentoTransporteNatura notaFiscalDocumentoTransporteNatura = repNotaFiscalDocumentoTransporteNatura.BuscarPorCTe(this.EmpresaUsuario.Codigo, CodigoCte);

                if (notaFiscalDocumentoTransporteNatura != null)
                {
                    if (notaFiscalDocumentoTransporteNatura.DocumentoTransporte.Codigo == codigoDocumentoTransporte)
                    {

                        Dominio.ObjetosDeValor.CTe.CTe cte = GerarObjetoCTeComplementarDoCTePai(notaFiscalDocumentoTransporteNatura.CTe, valorComplemento, observacao, incluirICMSFrete, unidadeDeTrabalho);

                        if (!string.IsNullOrEmpty(this.EmpresaUsuario.NomeCertificado))
                        {

                            Dominio.Entidades.ConhecimentoDeTransporteEletronico cteIntegrado = servicoCte.GerarCTePorObjeto(cte, 0, unidadeDeTrabalho);
                            Dominio.Entidades.NotaFiscalDocumentoTransporteNatura CopiaNotaFiscalDocumentoTransporteNatura = notaFiscalDocumentoTransporteNatura.Clonar();
                            CopiaNotaFiscalDocumentoTransporteNatura.CTe = cteIntegrado;
                            CopiaNotaFiscalDocumentoTransporteNatura.Codigo = 0;
                            repNotaFiscalDocumentoTransporteNatura.Inserir(CopiaNotaFiscalDocumentoTransporteNatura);

                            unidadeDeTrabalho.CommitChanges();

                            bool sucesso = servicoCte.Emitir(cteIntegrado.Codigo, cteIntegrado.Empresa.Codigo);
                            if (sucesso)
                            {
                                if (cteIntegrado.SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento.Integrador)
                                    FilaConsultaCTe.GetInstance().QueueItem(1, cteIntegrado.Codigo, Dominio.Enumeradores.TipoObjetoConsulta.CTe, Conexao.StringConexao);
                            }
                            else
                            {
                                return Json<bool>(false, false, "O CT-e nº " + cteIntegrado.Numero.ToString() + " da empresa " + cteIntegrado.Empresa.CNPJ + " foi salvo, porém, ocorreu uma falha ao emiti-lo.");
                            }
                        }
                        else
                        {
                            unidadeDeTrabalho.Rollback();
                            return Json<bool>(false, false, "Não empresa não está configurada para emissão de CT-e(s).");
                        }


                    }
                    else
                    {
                        unidadeDeTrabalho.Rollback();
                        return Json<bool>(false, false, "A DT informado não é mesma do CT-e a ser complementado.");
                    }
                }
                else
                {
                    unidadeDeTrabalho.Rollback();
                    return Json<bool>(false, false, "O CT-e informado para ser complementado não pertence a DT informada.");
                }
                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao emitir os CT-es do documento de transporte.");
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult EmitirCTes()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoDocumentoTransporte, codigoMotorista, codigoVeiculo, codigoTipoOperacao, codigoTipoCarga;
                int.TryParse(Request.Params["CodigoDocumentoTransporte"], out codigoDocumentoTransporte);
                int.TryParse(Request.Params["CodigoMotorista"], out codigoMotorista);
                int.TryParse(Request.Params["CodigoVeiculo"], out codigoVeiculo);
                int.TryParse(Request.Params["CodigoTipoOperacao"], out codigoTipoOperacao);
                int.TryParse(Request.Params["CodigoTipoCarga"], out codigoTipoCarga);

                string observacao = Request.Params["Observacao"];

                decimal valorFrete, valorFreteCalculado;
                decimal.TryParse(Request.Params["ValorFrete"], out valorFrete);
                decimal.TryParse(Request.Params["ValorFreteCalculado"], out valorFreteCalculado);

                Repositorio.DocumentoTransporteNatura repDocumento = new Repositorio.DocumentoTransporteNatura(unidadeDeTrabalho);

                Dominio.Entidades.DocumentoTransporteNatura documento = repDocumento.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigoDocumentoTransporte);

                if (documento == null)
                    return Json<bool>(false, false, "Documento de transporte não encontrado.");

                if (documento.Status != Dominio.ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura.EmDigitacao)
                    return Json<bool>(false, false, "O status do documento não permite a emissão dos CT-es.");

                //Bloqueia a emissão de CTe quando:
                // - Localidade Emitente igual a localidade do destinatario E tag QtdRevista diferente de "011 PLURAL SP 3547304"
                if (documento.Tipo == Dominio.ObjetosDeValor.Enumerador.TipoDocumentoTransporteNatura.WebService)
                {
                    if ((from doc in documento.NotasFiscais where doc.Emitente.Localidade.Codigo == doc.Destinatario.Localidade.Codigo select doc).Any() &&
                        (from doc in documento.NotasFiscais where doc.QtdRevista != "011 PLURAL SP 3547304" select doc).Any())
                        return Json<bool>(false, false, "Existem documentos em que a localidade do emitente é igual à localidade do destinatário, não sendo possível emitir os CT-es.");
                }

                Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unidadeDeTrabalho);
                Repositorio.Embarcador.Cargas.TipoDeCarga repTipoCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unidadeDeTrabalho);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unidadeDeTrabalho);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeDeTrabalho);

                documento.Motorista = repUsuario.BuscarMotoristaPorCodigoEEmpresa(this.EmpresaUsuario.Codigo, codigoMotorista);
                documento.Veiculo = repVeiculo.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigoVeiculo);
                documento.ValorFrete = valorFrete;
                documento.ValorFreteCalculado = valorFreteCalculado;
                documento.TipoCarga = repTipoCarga.BuscarPorCodigo(codigoTipoCarga);
                documento.TipoOperacao = repTipoOperacao.BuscarPorCodigo(codigoTipoOperacao);

                if (documento.Motorista == null)
                    return Json<bool>(false, false, "Motorista não encontrado.");

                if (documento.Veiculo == null)
                    return Json<bool>(false, false, "Veículo não encontrado.");

                if (documento.ValorFrete <= 0)
                    return Json<bool>(false, false, "Valor do frete deve ser maior que zero.");

                repDocumento.Atualizar(documento);

                Servicos.Natura svcNatura = new Servicos.Natura(unidadeDeTrabalho);

                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = svcNatura.EmitirCTes(documento.Codigo, observacao, unidadeDeTrabalho);

                if (ctes != null && ctes.Count() > 0)
                {
                    foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cte in ctes)
                    {
                        if (cte.SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento.Integrador)
                            FilaConsultaCTe.GetInstance().QueueItem(1, cte.Codigo, Dominio.Enumeradores.TipoObjetoConsulta.CTe, Conexao.StringConexao);
                    }

                    var notasPendentes = (from obj in documento.NotasFiscais where obj.Status == Dominio.ObjetosDeValor.Enumerador.StatusNotaFiscalNatura.Pendente select obj).ToList();
                    if (notasPendentes != null && notasPendentes.Count == 0)
                    {
                        documento.Status = Dominio.ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura.Emitido;
                        repDocumento.Atualizar(documento);
                    }

                    return Json<bool>(true, true);
                }
                else
                    return Json<bool>(false, false, "Nenhum CTe foi gerado.");
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao emitir os CT-es do documento de transporte.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult CalcularFrete()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                //int codigoDocumentoTransporte, codigoVeiculo, codigoTipoOperacao, codigoTipoCarga, codigoModeloVeicularCarga;
                //int.TryParse(Request.Params["CodigoDocumentoTransporte"], out codigoDocumentoTransporte);
                //int.TryParse(Request.Params["CodigoVeiculo"], out codigoVeiculo);
                //int.TryParse(Request.Params["CodigoTipoOperacao"], out codigoTipoOperacao);
                //int.TryParse(Request.Params["CodigoTipoCarga"], out codigoTipoCarga);
                //int.TryParse(Request.Params["CodigoModeloVeicularCarga"], out codigoModeloVeicularCarga);

                //Repositorio.DocumentoTransporteNatura repDocumento = new Repositorio.DocumentoTransporteNatura(unidadeDeTrabalho);

                //Dominio.Entidades.DocumentoTransporteNatura documento = repDocumento.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigoDocumentoTransporte);

                //if (documento == null)
                //    return Json<bool>(false, false, "Documento de transporte não encontrado.");

                //Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unidadeDeTrabalho);
                //Repositorio.Embarcador.Cargas.TipoDeCarga repTipoCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unidadeDeTrabalho);
                //Repositorio.Usuario repUsuario = new Repositorio.Usuario(unidadeDeTrabalho);
                //Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeDeTrabalho);
                //Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unidadeDeTrabalho);

                //Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga = repTipoCarga.BuscarPorCodigo(codigoTipoCarga);
                //Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repTipoOperacao.BuscarPorCodigo(codigoTipoOperacao);

                //Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigoVeiculo);

                //if (veiculo == null)
                //    return Json<bool>(false, false, "Veículo não encontrado.");

                //Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga = repModeloVeicularCarga.BuscarPorCodigo(codigoModeloVeicularCarga);

                //if (modeloVeicularCarga == null)
                //    return Json<bool>(false, false, "Modelo veicular de carga não encontrado.");

                //List<Dominio.Entidades.Veiculo> reboques = Servicos.Veiculo.ObterReboques(veiculo);

                //foreach (Dominio.Entidades.Veiculo reboque in reboques)
                //{
                //    reboque.ModeloVeicularCarga = modeloVeicularCarga;

                //    repVeiculo.Atualizar(reboque);
                //}

                //StringBuilder msgTabelaFrete = new StringBuilder();

                //Servicos.Embarcador.Carga.FreteCliente svcFrete = new Servicos.Embarcador.Carga.FreteCliente(Conexao.StringConexao);

                //Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularReboque = null;

                //if (veiculo.ModeloVeicularCarga != null && veiculo.ModeloVeicularCarga.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModeloVeicularCarga.Tracao)
                //    modeloVeicularReboque = veiculo.ModeloVeicularCarga;
                //else
                //{
                //    if (veiculo.VeiculosVinculados != null && veiculo.VeiculosVinculados.Count > 0)
                //    {
                //        modeloVeicularReboque = (from obj in veiculo.VeiculosVinculados where obj.ModeloVeicularCarga.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModeloVeicularCarga.Tracao select obj.ModeloVeicularCarga).FirstOrDefault();
                //    }
                //}


                //List<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente> tabelasFrete = svcFrete.ObterTabelasFrete(unidadeDeTrabalho, ref msgTabelaFrete, DateTime.Now, (from doc in documento.NotasFiscais select doc.Emitente).FirstOrDefault(), (from doc in documento.NotasFiscais select doc.Destinatario).FirstOrDefault(), (from doc in documento.NotasFiscais select doc.Emitente.Localidade).Distinct().ToList(), (from doc in documento.NotasFiscais select doc.Destinatario.Localidade).Distinct().ToList(), veiculo, tipoCarga, tipoOperacao, modeloVeicularReboque);

                //if (tabelasFrete.Count <= 0)
                //    return Json<bool>(false, false, msgTabelaFrete.ToString());
                //else if (tabelasFrete.Count > 1)
                //    return Json<bool>(false, false, "Foi encontrada mais de uma tabela de frete para esta configuração de frete.");

                //decimal valorFreteCalculado = Math.Round(svcFrete.CalcularValorFrete(tabelasFrete[0], veiculo, tipoCarga, (from obj in documento.NotasFiscais select obj.Valor).Sum()), 2, MidpointRounding.ToEven);

                //return Json(new { ValorFrete = valorFreteCalculado.ToString("n2") }, true);

                return Json<bool>(false, false, "Nenhuma tabela de frete encontrada.");
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao emitir os CT-es do documento de transporte.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult EmitirNFSes()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                Servicos.NFSe servicoNFSe = new Servicos.NFSe(unidadeDeTrabalho);
                Dominio.Entidades.ServicoNFSe servicoMultiCTe = servicoNFSe.ObterServicoNFSe(this.EmpresaUsuario, false, this.EmpresaUsuario.Localidade.CodigoIBGE, unidadeDeTrabalho);
                Dominio.Entidades.NaturezaNFSe naturezaMultiCTe = servicoNFSe.ObterNaturezaNFSe(this.EmpresaUsuario, false, this.EmpresaUsuario.Localidade.CodigoIBGE, unidadeDeTrabalho);

                if (this.EmpresaUsuario.Configuracao == null || servicoNFSe == null || naturezaMultiCTe == null || this.EmpresaUsuario.Configuracao.SerieNFSe == null)
                    return Json<bool>(false, false, "Configure o serviço, a natureza e a série para NFS-e nas configurações da empresa.");

                int codigoDocumentoTransporte;
                int.TryParse(Request.Params["CodigoDocumentoTransporte"], out codigoDocumentoTransporte);

                decimal valorFrete;
                decimal.TryParse(Request.Params["ValorFrete"], out valorFrete);

                Repositorio.DocumentoTransporteNatura repDocumento = new Repositorio.DocumentoTransporteNatura(unidadeDeTrabalho);

                Dominio.Entidades.DocumentoTransporteNatura documento = repDocumento.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigoDocumentoTransporte);

                if (documento == null)
                    return Json<bool>(false, false, "Documento de transporte não encontrado.");

                if (documento.Status != Dominio.ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura.EmDigitacao && documento.Tipo == Dominio.ObjetosDeValor.Enumerador.TipoDocumentoTransporteNatura.WebService)
                    return Json<bool>(false, false, "O status do documento não permite a emissão das NFS-es.");

                //Bloqueia a emissão de NFSe quando:
                // - Localidade Emitente diferente da localidade do destinatario
                // - Localidade Emitente igual a localidade do destinatário E tag QtdRevista == "011 PLURAL SP 3547304"
                if (documento.Tipo == Dominio.ObjetosDeValor.Enumerador.TipoDocumentoTransporteNatura.WebService)
                {
                    if ((from doc in documento.NotasFiscais where doc.Emitente.Localidade.Codigo != doc.Destinatario.Localidade.Codigo select doc).Any() ||
                    ((from doc in documento.NotasFiscais where doc.Emitente.Localidade.Codigo == doc.Destinatario.Localidade.Codigo select doc).Any() &&
                      (from doc in documento.NotasFiscais where doc.QtdRevista.Equals("011 PLURAL SP 3547304") select doc).Any()))
                        return Json<bool>(false, false, "Existem documentos em que a localidade do emitente difere da localidade do destinatário, não sendo possível emitir as NFS-es.");
                }

                documento.ValorFrete = valorFrete;

                if (documento.ValorFrete <= 0)
                    return Json<bool>(false, false, "Valor do frete deve ser maior que zero.");

                repDocumento.Atualizar(documento);

                Servicos.Natura svcNatura = new Servicos.Natura(unidadeDeTrabalho);

                List<Dominio.Entidades.NFSe> nfses = svcNatura.EmitirNFSe(documento.Codigo, unidadeDeTrabalho);

                if (nfses.Count() > 0)
                {
                    Servicos.NFSe svcNFSe = new Servicos.NFSe(unidadeDeTrabalho);
                    foreach (Dominio.Entidades.NFSe nfse in nfses)
                    {
                        if (svcNFSe.Emitir(nfse, unidadeDeTrabalho))
                            FilaConsultaCTe.GetInstance().QueueItem(3, nfse.Codigo, Dominio.Enumeradores.TipoObjetoConsulta.NFSe, Conexao.StringConexao);
                    }

                    var notasPendentes = (from obj in documento.NotasFiscais where obj.Status == Dominio.ObjetosDeValor.Enumerador.StatusNotaFiscalNatura.Pendente select obj).ToList();
                    if (notasPendentes != null && notasPendentes.Count == 0)
                    {
                        documento.Status = Dominio.ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura.Emitido;
                        repDocumento.Atualizar(documento);
                    }

                    return Json<bool>(true, true);
                }
                else
                    return Json<bool>(false, false, "Nenhuma NFSe foi gerada.");

            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao emitir as NFS-es do documento de transporte.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult EnviarRetornoDocumentoTransporte()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoDocumentoTransporte;
                int.TryParse(Request.Params["CodigoDocumentoTransporte"], out codigoDocumentoTransporte);

                string tipoRetorno = Request.Params["tipoRetorno"];

                Repositorio.DocumentoTransporteNatura repDocumento = new Repositorio.DocumentoTransporteNatura(unitOfWork);

                Dominio.Entidades.DocumentoTransporteNatura documento = repDocumento.BuscarPorCodigo(codigoDocumentoTransporte);

                if (documento == null)
                    return Json<bool>(false, false, "Documento de transporte não encontrado.");

                Repositorio.NotaFiscalDocumentoTransporteNatura repNotaFiscal = new Repositorio.NotaFiscalDocumentoTransporteNatura(unitOfWork);

                List<Dominio.Entidades.NotaFiscalDocumentoTransporteNatura> notasFiscais = repNotaFiscal.BuscarPorDocumentoTransporte(codigoDocumentoTransporte);

                //Solicitado pela Juliana a remoção desta validação
                if ((from obj in notasFiscais where obj.Status == Dominio.ObjetosDeValor.Enumerador.StatusNotaFiscalNatura.Pendente select obj).Any())
                    return Json<bool>(false, false, "Há notas fiscais pendentes de emissão de CT-e/NFS-e para este documento de transporte.");

                if ((from obj in notasFiscais where (obj.CTe == null && obj.NFSe == null) || (obj.CTe != null && obj.CTe.Status != "A" && obj.CTe.Status != "C") || (obj.NFSe != null && obj.NFSe.Empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao && obj.NFSe.Status != Dominio.Enumeradores.StatusNFSe.Autorizado && obj.NFSe.Status != Dominio.Enumeradores.StatusNFSe.Cancelado) select obj).Any())
                    return Json<bool>(false, false, "Há CT-es/NFS-es com o status diferente de autorizado/cancelado para este documento de transporte.");

                Servicos.Natura svcNatura = new Servicos.Natura(unitOfWork);

                if (tipoRetorno == "Normal")
                    svcNatura.EnviarRetornoDocumentoTransporte(codigoDocumentoTransporte, unitOfWork, this.UsuarioAdministrativo != null ? this.UsuarioAdministrativo.Codigo : this.Usuario.Codigo);
                else
                    svcNatura.EnviarRetornoDocumentoTransporteCTeComplementar(codigoDocumentoTransporte, unitOfWork, this.UsuarioAdministrativo != null ? this.UsuarioAdministrativo.Codigo : this.Usuario.Codigo);

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao enviar o retorno do documento de transporte.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult EnviarOcorrenciasDocumentoTransporte()
        {

            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoDocumentoTransporte;
                int.TryParse(Request.Params["CodigoDocumentoTransporte"], out codigoDocumentoTransporte);

                Repositorio.DocumentoTransporteNatura repDocumento = new Repositorio.DocumentoTransporteNatura(unidadeDeTrabalho);

                Dominio.Entidades.DocumentoTransporteNatura documento = repDocumento.BuscarPorCodigo(codigoDocumentoTransporte);

                if (documento == null)
                    return Json<bool>(false, false, "Documento de transporte não encontrado.");

                Repositorio.NotaFiscalDocumentoTransporteNatura repNotaFiscal = new Repositorio.NotaFiscalDocumentoTransporteNatura(unidadeDeTrabalho);

                List<Dominio.Entidades.NotaFiscalDocumentoTransporteNatura> notasFiscais = repNotaFiscal.BuscarPorDocumentoTransporte(codigoDocumentoTransporte);

                //Solicitado pela Juliana a remoção desta validação
                if ((from obj in notasFiscais where obj.Status != Dominio.ObjetosDeValor.Enumerador.StatusNotaFiscalNatura.Retornado && obj.Status != Dominio.ObjetosDeValor.Enumerador.StatusNotaFiscalNatura.Finalizado select obj).Any())
                    return Json<bool>(false, false, "Há notas documentos pendentes de retorno para a Natura.");

                Repositorio.OcorrenciaDeCTe repOcorrencia = new Repositorio.OcorrenciaDeCTe(unidadeDeTrabalho);

                if (documento.Tipo == Dominio.ObjetosDeValor.Enumerador.TipoDocumentoTransporteNatura.FTP)
                {
                    if (notasFiscais[0].NFSe == null)
                    {
                        List<Dominio.Entidades.OcorrenciaDeCTe> ocorrencias = repOcorrencia.BuscarPorCTe((from obj in notasFiscais where obj.CTe != null select obj.CTe.Codigo).ToArray());

                        if (ocorrencias != null && ocorrencias.Count > 0)
                        {
                            if ((from obj in notasFiscais where obj.CTe != null && !(from oco in ocorrencias select oco.CTe.Codigo).Contains(obj.CTe.Codigo) select obj).Any())
                                return Json<bool>(false, false, "Há CT-es sem ocorrência cadastrada para este documento de transporte.");
                        }
                        else
                            return Json<bool>(false, false, "Há CT-es sem ocorrência cadastrada para este documento de transporte.");
                    }
                    else if (notasFiscais[0].NFSe.Status != Dominio.Enumeradores.StatusNFSe.Autorizado)
                    {
                        return Json<bool>(false, false, "Há NFS-es que não estão autorizadas.");
                    }
                }

                Servicos.Natura svcNatura = new Servicos.Natura(unidadeDeTrabalho);

                if (documento.Tipo == Dominio.ObjetosDeValor.Enumerador.TipoDocumentoTransporteNatura.WebService)
                    svcNatura.EnviarOcorrenciasDocumentoTransporte(codigoDocumentoTransporte, unidadeDeTrabalho);
                else
                {
                    // Valida configuracoes FTP
                    if (this.EmpresaUsuario.Configuracao == null)
                        return Json<bool>(false, false, "Não há configuração cadastrada para a empresa.");

                    if (!this.ValidaFTP())
                        return Json<bool>(false, false, "Nenhuma configuração de FTP cadastrada para a empresa.");

                    if (this.EmpresaUsuario.Configuracao.LayoutEDIOcoren == null)
                        return Json<bool>(false, false, "Nenhum Layout EDI OCOREN configurado.");

                    if (svcNatura.EnviarOcorrenciasDocumentoTransporteFTP(codigoDocumentoTransporte, unidadeDeTrabalho))
                    {
                        try
                        {
                            //Atualizar status do Documento e das notas dos documento da natura
                            documento.Status = Dominio.ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura.Finalizado;
                            repDocumento.Atualizar(documento);
                            foreach (Dominio.Entidades.NotaFiscalDocumentoTransporteNatura notaFiscal in notasFiscais)
                            {
                                notaFiscal.Status = Dominio.ObjetosDeValor.Enumerador.StatusNotaFiscalNatura.Finalizado;
                                repNotaFiscal.Atualizar(notaFiscal);
                            }
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro("Falha ao atualizar status DT Natura: " + ex);
                        }
                    }
                    else
                        return Json<bool>(false, false, "Ocorreu uma falha ao enviar ao FTP ocorrência do documento de transporte.");
                }

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao enviar ocorrência do documento de transporte.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult CancelarDocumentosTransporte()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoDocumentoTransporte;
                int.TryParse(Request.Params["CodigoDocumentoTransporte"], out codigoDocumentoTransporte);

                Repositorio.DocumentoTransporteNatura repDocumento = new Repositorio.DocumentoTransporteNatura(unitOfWork);

                Dominio.Entidades.DocumentoTransporteNatura documento = repDocumento.BuscarPorCodigo(codigoDocumentoTransporte);

                if (documento == null)
                    return Json<bool>(false, false, "Documento de transporte não encontrado.");

                Repositorio.NotaFiscalDocumentoTransporteNatura repNotaFiscal = new Repositorio.NotaFiscalDocumentoTransporteNatura(unitOfWork);

                List<Dominio.Entidades.NotaFiscalDocumentoTransporteNatura> notasFiscais = repNotaFiscal.BuscarPorDocumentoTransporte(codigoDocumentoTransporte);

                foreach (Dominio.Entidades.NotaFiscalDocumentoTransporteNatura notaFiscal in notasFiscais)
                {
                    if (notaFiscal.CTe != null && notaFiscal.CTe.Status != "C" && notaFiscal.CTe.Status != "I")
                        return Json<bool>(false, false, "Há CT-e(s) pendente(s) de cancelamento/inutilização.");
                    if (notaFiscal.NFSe != null && notaFiscal.NFSe.Status != Dominio.Enumeradores.StatusNFSe.Cancelado)
                        return Json<bool>(false, false, "Há NFS-e(s) pendente(s) de cancelamento.");
                }

                documento.Status = Dominio.ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura.Cancelado;
                repDocumento.Atualizar(documento);

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao enviar o retorno do documento de transporte.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterDadosVeiculo()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoVeiculo;
                int.TryParse(Request.Params["CodigoVeiculo"], out codigoVeiculo);

                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeDeTrabalho);

                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(codigoVeiculo);

                if (veiculo == null)
                    return Json<bool>(false, false, "Veículo não encontrado.");

                Dominio.Entidades.Usuario motoristaPrincipal = veiculo.Motoristas.Where(o => o.Principal).Select(o => o.Motorista).FirstOrDefault();

                var retorno = new
                {
                    CodigoMotorista = motoristaPrincipal?.Codigo ?? 0,
                    CPFMotorista = motoristaPrincipal?.CPF_Formatado ?? string.Empty,
                    NomeMotorista = motoristaPrincipal?.Nome ?? string.Empty,
                    CodigoModeloVeicularCarga = veiculo.ModeloVeicularCarga?.Codigo ?? 0,
                    DescricaoModeloVeicularCarga = veiculo.ModeloVeicularCarga?.Descricao ?? string.Empty
                };

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao obter os detalhes do veículo.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterDetalhesEmissaoCTe()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoDT;
                int.TryParse(Request.Params["CodigoDT"], out codigoDT);

                Repositorio.DocumentoTransporteNatura repDT = new Repositorio.DocumentoTransporteNatura(unidadeDeTrabalho);

                Dominio.Entidades.DocumentoTransporteNatura dt = repDT.BuscarPorCodigo(codigoDT);

                if (dt == null)
                    return Json<bool>(false, false, "Documento de transporte não encontrado.");

                var retorno = new
                {
                    ValorFrete = dt.ValorFrete > 0 ? dt.ValorFrete.ToString("n2") : (from obj in dt.NotasFiscais select obj.ValorFrete).Sum().ToString("n2"),
                    ValorFreteCalculado = dt.ValorFreteCalculado > 0 ? dt.ValorFreteCalculado.ToString("n2") : string.Empty,
                    Origem = string.Join(" / ", (from obj in dt.NotasFiscais select obj.Emitente.Localidade.DescricaoCidadeEstado).Distinct()),
                    Destino = dt.Tipo == Dominio.ObjetosDeValor.Enumerador.TipoDocumentoTransporteNatura.FTP ? string.Join(" / ", (from obj in dt.NotasFiscais where obj.Emitente.Localidade.Codigo != obj.Destinatario.Localidade.Codigo select obj.Destinatario.Localidade.DescricaoCidadeEstado).Distinct()) : string.Join(" / ", (from obj in dt.NotasFiscais select obj.Destinatario.Localidade.DescricaoCidadeEstado).Distinct()),
                    CodigoMotorista = dt.Motorista?.Codigo ?? 0,
                    CPFMotorista = dt.Motorista?.CPF_Formatado ?? string.Empty,
                    NomeMotorista = dt.Motorista?.Nome ?? string.Empty,
                    CodigoModeloVeicularCarga = dt.Veiculo?.ModeloVeicularCarga?.Codigo ?? 0,
                    DescricaoModeloVeicularCarga = dt.Veiculo?.ModeloVeicularCarga?.Descricao ?? string.Empty,
                    CodigoVeiculo = dt.Veiculo?.Codigo,
                    PlacaVeiculo = dt.Veiculo?.Placa,
                    CodigoTipoCarga = dt.TipoCarga?.Codigo ?? 0,
                    DescricaoTipoCarga = dt.TipoCarga?.Descricao ?? string.Empty,
                    CodigoTipoOperacao = dt.TipoOperacao?.Codigo ?? 0,
                    DescricaoTipoOperacao = dt.TipoOperacao?.Descricao ?? string.Empty,
                    Tipo = dt.Tipo,
                    QuantidadeNotas = (from obj in dt.NotasFiscais select obj.Destinatario.Localidade.DescricaoCidadeEstado).Count(),
                    QuantidadeNotasNFSe = dt.Tipo == Dominio.ObjetosDeValor.Enumerador.TipoDocumentoTransporteNatura.FTP ? (from obj in dt.NotasFiscais where obj.Emitente.Localidade.Codigo == obj.Destinatario.Localidade.Codigo select obj.Destinatario.Localidade.DescricaoCidadeEstado).Count() : 0
                };

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao obter os detalhes para emissão do(s) CT-e(s).");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterDetalhesEmissaoNFSe()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoDT;
                int.TryParse(Request.Params["CodigoDT"], out codigoDT);

                Repositorio.DocumentoTransporteNatura repDT = new Repositorio.DocumentoTransporteNatura(unidadeDeTrabalho);

                Dominio.Entidades.DocumentoTransporteNatura dt = repDT.BuscarPorCodigo(codigoDT);

                if (dt == null)
                    return Json<bool>(false, false, "Documento de transporte não encontrado.");

                var retorno = new
                {
                    ValorFrete = dt.ValorFrete > 0 ? dt.ValorFrete.ToString("n2") : (from obj in dt.NotasFiscais select obj.ValorFrete).Sum().ToString("n2"),
                    ValorFreteCalculado = dt.ValorFreteCalculado > 0 ? dt.ValorFreteCalculado.ToString("n2") : string.Empty,
                    Origem = dt.Tipo == Dominio.ObjetosDeValor.Enumerador.TipoDocumentoTransporteNatura.FTP ? string.Join(" / ", (from obj in dt.NotasFiscais where obj.Emitente.Localidade.Codigo == obj.Destinatario.Localidade.Codigo select obj.Emitente.Localidade.DescricaoCidadeEstado).Distinct()) : string.Join(" / ", (from obj in dt.NotasFiscais select obj.Emitente.Localidade.DescricaoCidadeEstado).Distinct()),
                    Destino = dt.Tipo == Dominio.ObjetosDeValor.Enumerador.TipoDocumentoTransporteNatura.FTP ? string.Join(" / ", (from obj in dt.NotasFiscais where obj.Emitente.Localidade.Codigo == obj.Destinatario.Localidade.Codigo select obj.Destinatario.Localidade.DescricaoCidadeEstado).Distinct()) : string.Join(" / ", (from obj in dt.NotasFiscais select obj.Destinatario.Localidade.DescricaoCidadeEstado).Distinct()),
                    CodigoMotorista = dt.Motorista?.Codigo ?? 0,
                    CPFMotorista = dt.Motorista?.CPF_Formatado ?? string.Empty,
                    NomeMotorista = dt.Motorista?.Nome ?? string.Empty,
                    CodigoModeloVeicularCarga = dt.Veiculo?.ModeloVeicularCarga?.Codigo ?? 0,
                    DescricaoModeloVeicularCarga = dt.Veiculo?.ModeloVeicularCarga?.Descricao ?? string.Empty,
                    CodigoVeiculo = dt.Veiculo?.Codigo,
                    PlacaVeiculo = dt.Veiculo?.Placa,
                    CodigoTipoCarga = dt.TipoCarga?.Codigo ?? 0,
                    DescricaoTipoCarga = dt.TipoCarga?.Descricao ?? string.Empty,
                    CodigoTipoOperacao = dt.TipoOperacao?.Codigo ?? 0,
                    DescricaoTipoOperacao = dt.TipoOperacao?.Descricao ?? string.Empty,
                    Tipo = dt.Tipo,
                    QuantidadeNotas = (from obj in dt.NotasFiscais select obj.Destinatario.Localidade.DescricaoCidadeEstado).Count(),
                    QuantidadeNotasNFSe = dt.Tipo == Dominio.ObjetosDeValor.Enumerador.TipoDocumentoTransporteNatura.FTP ? (from obj in dt.NotasFiscais where obj.Emitente.Localidade.Codigo == obj.Destinatario.Localidade.Codigo select obj.Destinatario.Localidade.DescricaoCidadeEstado).Count() : (from obj in dt.NotasFiscais select obj.Destinatario.Localidade.DescricaoCidadeEstado).Count()
                };

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao obter os detalhes para emissão do(s) CT-e(s).");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult RemoverCTeDocumentoTransporte()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoCTe, codigoDT;
                int.TryParse(Request.Params["CodigoCTe"], out codigoCTe);

                Repositorio.NotaFiscalDocumentoTransporteNatura repNotaFiscalDT = new Repositorio.NotaFiscalDocumentoTransporteNatura(unidadeDeTrabalho);
                Repositorio.DocumentoTransporteNatura repDTNatura = new Repositorio.DocumentoTransporteNatura(unidadeDeTrabalho);
                Repositorio.ItemFaturaNatura repItemFatura = new Repositorio.ItemFaturaNatura(unidadeDeTrabalho);

                List<Dominio.Entidades.NotaFiscalDocumentoTransporteNatura> listaNotasDT = repNotaFiscalDT.BuscarListaNotasPorCTe(this.EmpresaUsuario.Codigo, codigoCTe);

                if (listaNotasDT == null || listaNotasDT.Count == 0)
                    return Json<bool>(false, false, "CT-e não encontrado.");

                codigoDT = listaNotasDT.FirstOrDefault().DocumentoTransporte.Codigo;

                List<Dominio.Entidades.ItemFaturaNatura> listaItensFatura = repItemFatura.BuscarPorCodigoCTe(codigoCTe);
                if (listaItensFatura.Count > 0)
                    return Json<bool>(false, false, "CT-e já se encontra na Fatura número " + listaItensFatura.FirstOrDefault().Fatura.Numero + ".");

                foreach (Dominio.Entidades.NotaFiscalDocumentoTransporteNatura notaDT in listaNotasDT)
                    repNotaFiscalDT.Deletar(notaDT);

                List<Dominio.Entidades.NotaFiscalDocumentoTransporteNatura> notasDT = repNotaFiscalDT.BuscarPorDocumentoTransporte(codigoDT);
                if (notasDT == null || notasDT.Count == 0)
                {
                    Dominio.Entidades.DocumentoTransporteNatura DTNatura = repDTNatura.BuscarPorCodigo(codigoDT);
                    DTNatura.Status = Dominio.ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura.EmDigitacao;

                    repDTNatura.Atualizar(DTNatura);
                }

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao remover o CT-e do Documento de Transporte Natura.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult RemoverNFSeDocumentoTransporte()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoNFSe, codigoDT, notasGeradas = 0;
                int.TryParse(Request.Params["CodigoNFSe"], out codigoNFSe);

                Repositorio.NotaFiscalDocumentoTransporteNatura repNotaFiscalDT = new Repositorio.NotaFiscalDocumentoTransporteNatura(unidadeDeTrabalho);
                Repositorio.DocumentoTransporteNatura repDTNatura = new Repositorio.DocumentoTransporteNatura(unidadeDeTrabalho);
                Repositorio.ItemFaturaNatura repItemFatura = new Repositorio.ItemFaturaNatura(unidadeDeTrabalho);

                List<Dominio.Entidades.NotaFiscalDocumentoTransporteNatura> listaNotasDT = repNotaFiscalDT.BuscarListaNotasPorNFSe(this.EmpresaUsuario.Codigo, codigoNFSe);

                if (listaNotasDT == null || listaNotasDT.Count == 0)
                    return Json<bool>(false, false, "NFS-e não encontrado.");

                codigoDT = listaNotasDT.FirstOrDefault().DocumentoTransporte.Codigo;

                List<Dominio.Entidades.ItemFaturaNatura> listaItensFatura = repItemFatura.BuscarPorCodigoNFSe(codigoNFSe);
                if (listaItensFatura.Count > 0)
                    return Json<bool>(false, false, "NFS-e já se encontra na Fatura número " + listaItensFatura.FirstOrDefault().Fatura.Numero + ".");

                foreach (Dominio.Entidades.NotaFiscalDocumentoTransporteNatura notaDT in listaNotasDT)
                {
                    notaDT.NFSe = null;
                    notaDT.Status = Dominio.ObjetosDeValor.Enumerador.StatusNotaFiscalNatura.Pendente;
                    repNotaFiscalDT.Atualizar(notaDT);
                }

                foreach (Dominio.Entidades.NotaFiscalDocumentoTransporteNatura notaDT in listaNotasDT)
                    if (notaDT.Status == Dominio.ObjetosDeValor.Enumerador.StatusNotaFiscalNatura.Pendente && notaDT.NFSe == null)
                        notasGeradas += notasGeradas;

                if (notasGeradas == 0)
                {
                    Dominio.Entidades.DocumentoTransporteNatura DTNatura = repDTNatura.BuscarPorCodigo(codigoDT);
                    DTNatura.Status = Dominio.ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura.EmDigitacao;

                    repDTNatura.Atualizar(DTNatura);
                }

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao remover o NFS-e do Documento de Transporte Natura.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public ActionResult DownloadXMLConsultaDT()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigoDT = 0;
                int.TryParse(Request.Params["CodigoDT"], out codigoDT);

                if (codigoDT > 0)
                {
                    Repositorio.DocumentoTransporteNatura repDT = new Repositorio.DocumentoTransporteNatura(unidadeDeTrabalho);
                    Repositorio.NaturaXML repNaturaXML = new Repositorio.NaturaXML(unidadeDeTrabalho);

                    Dominio.Entidades.DocumentoTransporteNatura dt = repDT.BuscarPorCodigo(codigoDT);

                    if (dt != null && dt.NaturaXMLs != null && dt.NaturaXMLs.Count() > 0)
                    {
                        foreach (Dominio.Entidades.NaturaXML naturaXML in dt.NaturaXMLs)
                        {
                            if (naturaXML.Tipo == Dominio.ObjetosDeValor.Enumerador.TipoXMLNatura.ConsultaDocumentoTransporte)
                            {
                                byte[] data = System.Text.Encoding.UTF8.GetBytes(naturaXML.XMLEnvio);

                                if (data != null)
                                {
                                    return Arquivo(data, "text/xml", string.Concat("ConsultaDT_" + dt.NumeroDT, ".xml"));
                                }
                                else
                                    return Json<bool>(false, false, "Ocorreu uma falha ao carregar XML, atualize a página e tente novamente.");
                            }
                        }
                        return Json<bool>(false, false, "Documento de Transporte não possui XML salvo.");
                    }
                    else
                        return Json<bool>(false, false, "Documento de Transporte não possui XML salvo.");
                }
                else
                    return Json<bool>(false, false, "Documento de Transporte não encontrada, atualize a página e tente novamente.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao realizar o download do XML.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public ActionResult DownloadXMLDT()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigoDT = 0;
                int.TryParse(Request.Params["CodigoDT"], out codigoDT);

                if (codigoDT > 0)
                {
                    Repositorio.DocumentoTransporteNatura repDT = new Repositorio.DocumentoTransporteNatura(unidadeDeTrabalho);
                    Repositorio.NaturaXML repNaturaXML = new Repositorio.NaturaXML(unidadeDeTrabalho);

                    Dominio.Entidades.DocumentoTransporteNatura dt = repDT.BuscarPorCodigo(codigoDT);

                    if (dt != null && dt.NaturaXMLs != null && dt.NaturaXMLs.Count() > 0)
                    {
                        foreach (Dominio.Entidades.NaturaXML naturaXML in dt.NaturaXMLs)
                        {
                            if (naturaXML.Tipo == Dominio.ObjetosDeValor.Enumerador.TipoXMLNatura.ConsultaDocumentoTransporte)
                            {
                                byte[] data = System.Text.Encoding.UTF8.GetBytes(naturaXML.XMLRetorno);

                                if (data != null)
                                {
                                    return Arquivo(data, "text/xml", string.Concat("DT_" + dt.NumeroDT, ".xml"));
                                }
                                else
                                    return Json<bool>(false, false, "Ocorreu uma falha ao carregar XML, atualize a página e tente novamente.");
                            }
                        }
                        return Json<bool>(false, false, "Documento de Transporte não possui XML salvo.");
                    }
                    else
                        return Json<bool>(false, false, "Documento de Transporte não possui XML salvo.");
                }
                else
                    return Json<bool>(false, false, "Documento de Transporte não encontrada, atualize a página e tente novamente.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao realizar o download do XML.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public ActionResult DownloadXMLNatura()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigo = 0;
                int.TryParse(Request.Params["Codigo"], out codigo);

                if (codigo > 0)
                {
                    Repositorio.DocumentoTransporteNatura repDT = new Repositorio.DocumentoTransporteNatura(unidadeDeTrabalho);
                    Repositorio.NaturaXML repNaturaXML = new Repositorio.NaturaXML(unidadeDeTrabalho);

                    Dominio.Entidades.NaturaXML naturaXML = repNaturaXML.BuscaPorCodigo(codigo);

                    if (naturaXML != null)
                    {
                        byte[] data = System.Text.Encoding.UTF8.GetBytes(naturaXML.XMLEnvio);

                        if (data != null)
                        {
                            if (naturaXML.Tipo == Dominio.ObjetosDeValor.Enumerador.TipoXMLNatura.ConsultaDocumentoTransporte)
                                return Arquivo(data, "text/xml", string.Concat("ConsultaDT.xml"));
                            else if (naturaXML.Tipo == Dominio.ObjetosDeValor.Enumerador.TipoXMLNatura.ConsultaPreFatura)
                                return Arquivo(data, "text/xml", string.Concat("ConsultaPreFatura.xml"));
                            else if (naturaXML.Tipo == Dominio.ObjetosDeValor.Enumerador.TipoXMLNatura.EnvioFatura)
                                return Arquivo(data, "text/xml", string.Concat("EnvioFatura.xml"));
                            else if (naturaXML.Tipo == Dominio.ObjetosDeValor.Enumerador.TipoXMLNatura.RetornoDocumentoTransporte)
                                return Arquivo(data, "text/xml", string.Concat("RetornoDT.xml"));
                            else if (naturaXML.Tipo == Dominio.ObjetosDeValor.Enumerador.TipoXMLNatura.RetornoDocumentoTransporteComplementar)
                                return Arquivo(data, "text/xml", string.Concat("RetornoDTComplementar.xml"));
                            else
                                return Arquivo(data, "text/xml", string.Concat("XMLNatura.xml"));

                        }
                        else
                            return Json<bool>(false, false, "Ocorreu uma falha ao carregar XML, atualize a página e tente novamente.");
                    }
                    return Json<bool>(false, false, "Documento de Transporte não possui XML salvo.");
                }
                else
                    return Json<bool>(false, false, "Documento de Transporte não possui XML salvo.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao realizar o download do XML.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public ActionResult ImportarNOTFIS()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            string nomeArquivo = null;
            try
            {
                if (this.EmpresaUsuario.Configuracao == null)
                    return Json<bool>(false, false, "A empresa emissora não possui as configurações necessárias para a importação de NOTFIS.");

                if (this.EmpresaUsuario.Configuracao.LayoutEDINatura == null)
                    return Json<bool>(false, false, "Não há um Layout de NOTFIS Natura configurado.");

                Dominio.Entidades.LayoutEDI layout = (from obj in this.EmpresaUsuario.LayoutsEDI where obj.Codigo == this.EmpresaUsuario.Configuracao.LayoutEDINatura.Codigo && obj.Tipo == Dominio.Enumeradores.TipoLayoutEDI.NOTFIS select obj).FirstOrDefault();

                if (layout != null)
                {
                    unidadeDeTrabalho.Start();

                    nomeArquivo = Request.Files[0].FileName;
                    Servicos.LeituraEDI serLeituraEDI = new Servicos.LeituraEDI(this.EmpresaUsuario, layout, Request.Files[0].InputStream, unidadeDeTrabalho);
                    List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal> notasFiscais = serLeituraEDI.GerarNotasFiscais();

                    if (notasFiscais.Count == 0)
                        return Json<bool>(false, false, "Nenhuma nota foi importada.");

                    if (string.IsNullOrWhiteSpace(notasFiscais[0].NumeroDT))
                        return Json<bool>(false, false, "NOTFIS natura não possui o número do documento de transporte.");

                    if (!GerarDTPorNOTFIS(this.EmpresaUsuario.Codigo, notasFiscais, unidadeDeTrabalho))
                    {
                        unidadeDeTrabalho.Rollback();
                        return Json<bool>(false, false, "Não foi possível gerar DT a partir do NOTIFIS importado, contate o suporte para mais informações.");
                    }

                    unidadeDeTrabalho.CommitChanges();

                    return Json<bool>(true, true);
                }
                else
                {
                    return Json<bool>(false, false, "Layout de EDI NOTFIS Natura não configurado.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                unidadeDeTrabalho.Rollback();

                return Json<bool>(false, false, "Ocorreu uma falha genérica ao ler o arquivo " + nomeArquivo != null ? nomeArquivo : "NOTFIS.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private bool GerarDTPorNOTFIS(int codigoEmpresa, List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal> notasFiscais, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            try
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
                Repositorio.DocumentoTransporteNatura repDTNatura = new Repositorio.DocumentoTransporteNatura(unidadeDeTrabalho);
                Repositorio.NotaFiscalDocumentoTransporteNatura repDTNotaFiscal = new Repositorio.NotaFiscalDocumentoTransporteNatura(unidadeDeTrabalho);

                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

                foreach (Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal nota in notasFiscais)
                {
                    if (!string.IsNullOrWhiteSpace(nota.NumeroDT))
                    {
                        long.TryParse(nota.NumeroDT, out long numeroDT);
                        Dominio.Entidades.DocumentoTransporteNatura dtNatura = repDTNatura.BuscarPorNumeroNaoCancelado(codigoEmpresa, numeroDT);

                        if (dtNatura != null && dtNatura.Status != Dominio.ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura.EmDigitacao)
                        {
                            Servicos.Log.TratarErro("A DT " + numeroDT.ToString() + " esta com status " + dtNatura.DescricaoStatus + " e não pode ser modificada.");
                            continue;
                        }

                        if (dtNatura == null)
                        {
                            if (!DateTime.TryParseExact(nota.DataEmissaoDT, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out DateTime dataEmissaoDT))
                                dataEmissaoDT = DateTime.Today;

                            dtNatura = new Dominio.Entidades.DocumentoTransporteNatura
                            {
                                Tipo = Dominio.ObjetosDeValor.Enumerador.TipoDocumentoTransporteNatura.FTP,
                                Empresa = empresa,
                                NumeroDT = numeroDT,
                                Status = Dominio.ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura.EmDigitacao,
                                DataEmissao = dataEmissaoDT
                            };

                            repDTNatura.Inserir(dtNatura);
                        }
                        Dominio.Entidades.NotaFiscalDocumentoTransporteNatura notaDT = repDTNotaFiscal.BuscarPorDTeChaveNFe(codigoEmpresa, numeroDT, nota.Chave);
                        if (notaDT == null)
                        {
                            if (!DateTime.TryParseExact(nota.DataEmissaoDT, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out DateTime dataEmissaoNFe))
                                dataEmissaoNFe = DateTime.Today;

                            int.TryParse(nota.Serie, out int serieNFe);
                            int.TryParse(nota.VolumesTotal.ToString(), out int quantidadeNFe);

                            notaDT = new Dominio.Entidades.NotaFiscalDocumentoTransporteNatura
                            {
                                DocumentoTransporte = dtNatura,
                                DataEmissao = dataEmissaoNFe,
                                Emitente = VerificarCliente(nota.Emitente, this.EmpresaUsuario, unidadeDeTrabalho, false),
                                Destinatario = VerificarCliente(nota.Destinatario, this.EmpresaUsuario, unidadeDeTrabalho, true),
                                Numero = nota.Numero,
                                Serie = serieNFe,
                                Chave = nota.Chave,
                                Valor = nota.Valor,
                                Peso = nota.PesoBruto > 0 ? nota.PesoBruto / 1000000 : 0, //Natura envia o peso em MILIGRAMAS
                                Quantidade = quantidadeNFe,
                                Status = Dominio.ObjetosDeValor.Enumerador.StatusNotaFiscalNatura.Pendente,
                                TipoPagamento = nota.ModalidadeFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.Pago ? Dominio.Enumeradores.TipoPagamento.Pago : Dominio.Enumeradores.TipoPagamento.A_Pagar,
                                ValorFrete = nota.ValorFrete,
                                SolicitacaoNumero = nota.NumeroSolicitacao,
                                PedidoNumero = nota.NumeroPedido,
                                CodigoCF = nota.Destinatario != null && !string.IsNullOrWhiteSpace(nota.Destinatario.CodigoIntegracao) ? nota.Destinatario.CodigoIntegracao : string.Empty
                            };

                            repDTNotaFiscal.Inserir(notaDT);
                        }

                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Não foi possível gerar DT por NOTFIS:" + ex);
                return false;
            }

        }

        private Dominio.Entidades.Cliente VerificarCliente(Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa pessoa, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unidadeDeTrabalho, bool atualizar)
        {
            if (pessoa != null && !string.IsNullOrWhiteSpace(pessoa.CPFCNPJ))
            {
                double cpfCnpjPessoa = 0;
                double.TryParse(pessoa.CPFCNPJ, out cpfCnpjPessoa);
                bool inserirCliente = false;

                Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);
                Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(cpfCnpjPessoa);
                if (cliente == null)
                {
                    cliente = new Dominio.Entidades.Cliente();
                    inserirCliente = true;
                }
                if (inserirCliente || atualizar)
                {

                    Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);
                    Dominio.Entidades.Localidade localidade = null;
                    if (pessoa.Endereco != null && pessoa.Endereco.Cidade != null && pessoa.Endereco.Cidade.IBGE > 0)
                        localidade = repLocalidade.BuscarPorCodigoIBGE(pessoa.Endereco.Cidade.IBGE);
                    if (localidade == null && pessoa.Endereco != null && pessoa.Endereco.Cidade != null && !string.IsNullOrWhiteSpace(pessoa.Endereco.Cidade.Descricao) && !string.IsNullOrWhiteSpace(pessoa.Endereco.Cidade.SiglaUF))
                        localidade = repLocalidade.BuscarPorDescricaoEUF(pessoa.Endereco.Cidade.Descricao, pessoa.Endereco.Cidade.SiglaUF);
                    if (localidade == null && pessoa.Endereco != null && pessoa.Endereco.Cidade != null && !string.IsNullOrWhiteSpace(pessoa.Endereco.CEP) && pessoa.Endereco.CEP.Length > 3)
                        localidade = repLocalidade.BuscarPorCEP(Utilidades.String.OnlyNumbers(pessoa.Endereco.CEP.Substring(0, 3)));
                    if (localidade == null)
                        localidade = empresa.Localidade;

                    cliente.CPF_CNPJ = cpfCnpjPessoa;
                    cliente.Bairro = pessoa.Endereco != null && !string.IsNullOrWhiteSpace(pessoa.Endereco.Bairro) ? pessoa.Endereco.Bairro : "Bairro";
                    cliente.CEP = pessoa.Endereco != null && !string.IsNullOrWhiteSpace(pessoa.Endereco.CEP) ? pessoa.Endereco.CEP : "000000-00";
                    cliente.Complemento = pessoa.Endereco != null && !string.IsNullOrWhiteSpace(pessoa.Endereco.Complemento) ? pessoa.Endereco.Complemento : string.Empty;
                    cliente.Endereco = pessoa.Endereco != null && !string.IsNullOrWhiteSpace(pessoa.Endereco.Logradouro) ? pessoa.Endereco.Logradouro : "SEM LOGRADOURO";
                    cliente.IE_RG = !string.IsNullOrWhiteSpace(pessoa.RGIE) ? pessoa.RGIE : "ISENTO";
                    cliente.Localidade = localidade;
                    cliente.Nome = !string.IsNullOrWhiteSpace(pessoa.RazaoSocial) ? pessoa.RazaoSocial : cliente.Nome;
                    cliente.NomeFantasia = !string.IsNullOrWhiteSpace(pessoa.NomeFantasia) ? pessoa.NomeFantasia : cliente.NomeFantasia;
                    cliente.Telefone1 = pessoa.Endereco != null && !string.IsNullOrWhiteSpace(pessoa.Endereco.Telefone) ? pessoa.Endereco.Telefone : string.Empty;

                    //Alterado para salvar na nota da DT e utilizar quando for emitir o documento pois mesmos clientes estão retornando da Natura com códigos diferentes
                    //if (!string.IsNullOrWhiteSpace(pessoa.CodigoIntegracao))
                    //{
                    //    string nome = string.Concat("(" + pessoa.CodigoIntegracao, ") ", cliente.Nome);
                    //    cliente.Nome = nome.Length > 80 ? nome.Substring(0, 80) : nome;

                    //    if (!string.IsNullOrWhiteSpace(pessoa.NomeFantasia))
                    //    {
                    //        nome = string.Concat("(" + pessoa.CodigoIntegracao, ") ", pessoa.NomeFantasia);
                    //        cliente.NomeFantasia = nome.Length > 80 ? nome.Substring(0, 80) : nome;
                    //    }
                    //}

                    cliente.Numero = pessoa.Endereco != null && !string.IsNullOrWhiteSpace(pessoa.Endereco.Numero) && pessoa.Endereco.Numero.Length > 2 ? pessoa.Endereco.Numero : "S/N";

                    if (Utilidades.Validate.ValidarCPF(pessoa.CPFCNPJ.Length > 11 ? pessoa.CPFCNPJ.Substring(pessoa.CPFCNPJ.Length - 11, 11) : pessoa.CPFCNPJ))
                    {
                        cliente.Atividade = Servicos.Atividade.ObterAtividade(empresa.Codigo, "F", Conexao.StringConexao);
                        cliente.Tipo = "F";
                        cliente.IE_RG = "ISENTO";
                    }
                    else if (Utilidades.Validate.ValidarCNPJ(pessoa.CPFCNPJ))
                    {

                        cliente.Atividade = Servicos.Atividade.ObterAtividade(empresa.Codigo, "J", Conexao.StringConexao);
                        cliente.Tipo = "J";
                    }
                    else
                    {
                        cliente.Atividade = Servicos.Atividade.ObterAtividade(empresa.Codigo, pessoa.CPFCNPJ.Length == 14 ? "J" : "F", Conexao.StringConexao);
                        cliente.Tipo = pessoa.CPFCNPJ.Length == 14 ? "J" : "F";
                    }

                    if (cliente.Tipo == "J" && cliente.GrupoPessoas == null)
                    {
                        Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unidadeDeTrabalho);
                        Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = repGrupoPessoas.BuscarPorRaizCNPJ(Utilidades.String.OnlyNumbers(cliente.CPF_CNPJ_Formatado).Remove(8, 6));
                        if (grupoPessoas != null)
                        {
                            cliente.GrupoPessoas = grupoPessoas;
                        }
                    }

                    if (inserirCliente)
                    {
                        cliente.DataCadastro = DateTime.Now;
                        cliente.Ativo = true;
                        repCliente.Inserir(cliente);
                    }
                    else
                    {
                        cliente.DataUltimaAtualizacao = DateTime.Now;
                        cliente.Integrado = false;
                        repCliente.Atualizar(cliente);
                    }
                }

                return cliente;
            }
            return null;
        }
        private bool ValidaFTP()
        {
            if (string.IsNullOrWhiteSpace(this.EmpresaUsuario.Configuracao.FTPNaturaDiretorio)) return false;
            if (string.IsNullOrWhiteSpace(this.EmpresaUsuario.Configuracao.FTPNaturaHost)) return false;
            if (string.IsNullOrWhiteSpace(this.EmpresaUsuario.Configuracao.FTPNaturaPorta)) return false;
            if (string.IsNullOrWhiteSpace(this.EmpresaUsuario.Configuracao.FTPNaturaSenha)) return false;
            if (string.IsNullOrWhiteSpace(this.EmpresaUsuario.Configuracao.FTPNaturaUsuario)) return false;

            return true;
        }

        #endregion
    }
}