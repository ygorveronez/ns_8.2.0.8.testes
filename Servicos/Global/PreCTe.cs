using Dominio.Entidades;
using Dominio.ObjetosDeValor.Embarcador.IBSCBS;
using MultiSoftware.CTe.v400.ConhecimentoDeTransporte;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace Servicos
{
    public class PreCTe : ServicoBase
    {

        #region Construtores

        public PreCTe(Repositorio.UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken)
        {
        }

        #endregion Construtores

        #region Métodos Públicos

        public void SalvarDadosPreCTe(ref Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCte, Dominio.ObjetosDeValor.Embarcador.CTe.CTe cteIntegracao)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(_unitOfWork);
            Repositorio.CFOP repCFOP = new Repositorio.CFOP(_unitOfWork);
            Repositorio.PreConhecimentoDeTransporteEletronico repPreCTe = new Repositorio.PreConhecimentoDeTransporteEletronico(_unitOfWork);
            Repositorio.ModalTransporte repModalTransporte = new Repositorio.ModalTransporte(_unitOfWork);

            Embarcador.CTe.DocumentoCTe serDocumentoCTe = new Embarcador.CTe.DocumentoCTe(_unitOfWork);
            Embarcador.CTe.Participante serParticipante = new Embarcador.CTe.Participante(_unitOfWork);
            Embarcador.CTe.ModalRodoviario serModalRodoviario = new Embarcador.CTe.ModalRodoviario(_unitOfWork);
            Embarcador.CTe.Quantidades serQuantidades = new Embarcador.CTe.Quantidades(_unitOfWork);
            Embarcador.CTe.Seguro serSeguros = new Embarcador.CTe.Seguro(_unitOfWork);
            Embarcador.CTe.ProdutoPerigoso serProdutoPerigoso = new Embarcador.CTe.ProdutoPerigoso(_unitOfWork);
            Embarcador.CTe.DocumentoAnterior serDocumentoAnterior = new Embarcador.CTe.DocumentoAnterior(_unitOfWork);
            Embarcador.CTe.DocumentoTransportaAnteriorPapel serDocumentoTransportaAnteriorPapel = new Embarcador.CTe.DocumentoTransportaAnteriorPapel(_unitOfWork);
            Embarcador.CTe.Observacoes serObservacoes = new Embarcador.CTe.Observacoes(_unitOfWork);
            Embarcador.CTe.ComponenteFrete serComponenteFrete = new Embarcador.CTe.ComponenteFrete(_unitOfWork);
            Embarcador.CTe.TotalServico serTotalServicos = new Embarcador.CTe.TotalServico(_unitOfWork);
            Embarcador.CTe.InformacaoCarga serInformacaoCarga = new Embarcador.CTe.InformacaoCarga(_unitOfWork);

            preCte.TransportadorTerceiro = repCliente.BuscarPorCPFCNPJ(double.Parse(Utilidades.String.OnlyNumbers(cteIntegracao.Emitente.CNPJ)));
            preCte.TipoTomador = cteIntegracao.TipoTomador;
            preCte.TipoPagamento = cteIntegracao.TipoPagamento;
            preCte.Retira = cteIntegracao.Retira ? Dominio.Enumeradores.OpcaoSimNao.Sim : Dominio.Enumeradores.OpcaoSimNao.Nao;
            preCte.DetalhesRetira = cteIntegracao.DetalhesRetira;
            preCte.RNTRC = preCte.Empresa != null ? preCte.Empresa.RegistroANTT : "";
            preCte.TipoCTE = cteIntegracao.TipoCTE;
            preCte.TipoImpressao = cteIntegracao.TipoImpressao;
            preCte.TipoServico = cteIntegracao.TipoServico;
            preCte.DataEmissao = cteIntegracao.DataEmissao == DateTime.MinValue ? DateTime.Now : cteIntegracao.DataEmissao;
            preCte.Versao = preCte.Empresa != null ? preCte.Empresa.Versao : "2.00";
            preCte.ObservacoesGerais = cteIntegracao.ObservacoesGeral != null ? cteIntegracao.ObservacoesGeral.Texto : "";
            preCte.CFOP = repCFOP.BuscarPorNumero(cteIntegracao.CFOP);
            preCte.ChaveCTEReferenciado = cteIntegracao.ChaveCTeComplementado;
            preCte.LocalidadeInicioPrestacao = repLocalidade.BuscarPorCodigo(cteIntegracao.LocalidadeInicioPrestacao.Codigo);
            preCte.LocalidadeTerminoPrestacao = repLocalidade.BuscarPorCodigo(cteIntegracao.LocalidadeFimPrestacao.Codigo);
            preCte.LocalidadeEmissao = preCte.Empresa != null ? preCte.Empresa.Localidade : preCte.TransportadorTerceiro != null ? preCte.TransportadorTerceiro.Localidade : preCte.LocalidadeInicioPrestacao;

            serTotalServicos.SalvarTotaisPreCTe(ref preCte, cteIntegracao.ValorFrete, _unitOfWork);
            serInformacaoCarga.SalvarInformacaoCargaPreCTe(ref preCte, cteIntegracao.InformacaoCarga, _unitOfWork);

            if (preCte.Codigo <= 0)
            {
                preCte.ModalTransporte = repModalTransporte.BuscarPorCodigo(1, false);
                repPreCTe.Inserir(preCte);
            }
            else
                repPreCTe.Atualizar(preCte);

            string mensagem = "";

            serParticipante.SalvarParticipantePreCTe(ref preCte, cteIntegracao.Remetente, null, Dominio.Enumeradores.TipoTomador.Remetente, ref mensagem, _unitOfWork);
            serParticipante.SalvarParticipantePreCTe(ref preCte, cteIntegracao.Expedidor, null, Dominio.Enumeradores.TipoTomador.Expedidor, ref mensagem, _unitOfWork);
            serParticipante.SalvarParticipantePreCTe(ref preCte, cteIntegracao.Recebedor, null, Dominio.Enumeradores.TipoTomador.Recebedor, ref mensagem, _unitOfWork);
            serParticipante.SalvarParticipantePreCTe(ref preCte, cteIntegracao.Destinatario, null, Dominio.Enumeradores.TipoTomador.Destinatario, ref mensagem, _unitOfWork);

            if (preCte.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros)
                serParticipante.SalvarParticipantePreCTe(ref preCte, cteIntegracao.Tomador, null, Dominio.Enumeradores.TipoTomador.Outros, ref mensagem, _unitOfWork);
            else
            {
                Dominio.Entidades.ParticipanteCTe part = preCte.ObterParticipante(Dominio.Enumeradores.TipoTomador.Outros);

                if (part != null)
                {
                    Repositorio.ParticipanteCTe repParticipante = new Repositorio.ParticipanteCTe(_unitOfWork);
                    preCte.SetarParticipante(null, Dominio.Enumeradores.TipoTomador.Outros, null, null);
                    repParticipante.Deletar(part);
                }
            }

            serModalRodoviario.SalvarModalRodoviarioPreCTe(ref preCte, cteIntegracao.ModalRodoviario, _unitOfWork);
            serComponenteFrete.SalvarComponentesPrestacaoPreCte(ref preCte, cteIntegracao.ValorFrete.ComponentesAdicionais);
            serQuantidades.SalvarQuantidadesPreCTe(ref preCte, cteIntegracao.QuantidadesCarga, _unitOfWork);
            serSeguros.SalvarSegurosPreCte(ref preCte, cteIntegracao.Seguros, _unitOfWork);
            serDocumentoCTe.SalvarInformacoesDocumentosPreCTe(ref preCte, cteIntegracao, _unitOfWork);
            serDocumentoAnterior.SalvarInformacoesDocumentosAnterioresPreCTe(ref preCte, cteIntegracao.DocumentosAnteriores, _unitOfWork);
            serDocumentoTransportaAnteriorPapel.SalvarInformacoesDocumentosAnterioresPapelPreCTe(ref preCte, cteIntegracao.DocumentosAnterioresDePapel, _unitOfWork);
            serObservacoes.SalvarObservacoesContribuintePreCTe(ref preCte, cteIntegracao.ObservacoesContribuinte, _unitOfWork);
            serObservacoes.SalvarObservacoesFiscoPreCte(ref preCte, cteIntegracao.ObservacoesFisco, _unitOfWork);
            serProdutoPerigoso.SalvarProdutosPerigososPreCTe(ref preCte, cteIntegracao.ProdutosPerigosos, _unitOfWork);

            repPreCTe.Atualizar(preCte);
        }

        public Dominio.Entidades.PreConhecimentoDeTransporteEletronico GerarPreCTePorObjeto(Dominio.ObjetosDeValor.CTe.CTe cteIntegracao, Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal, string codigoPedidoCliente, decimal preCTePesoCarga, string preCTeValoresFormula, int codigoCanalEntrega)
        {
            Repositorio.ModalTransporte repModalTransporte = new Repositorio.ModalTransporte(_unitOfWork);
            Repositorio.CFOP repCFOP = new Repositorio.CFOP(_unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(_unitOfWork);
            Repositorio.PreConhecimentoDeTransporteEletronico repPreCTe = new Repositorio.PreConhecimentoDeTransporteEletronico(_unitOfWork);
            Servicos.CTe serCTE = new CTe(_unitOfWork);
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(_unitOfWork);
            Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(cteIntegracao.Emitente.CNPJ);

            if (cteIntegracao.Emitente.Atualizar)
                empresa = Servicos.Empresa.AtualizarEmpresa(empresa.Codigo, cteIntegracao.Emitente, _unitOfWork);

            Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCte = new Dominio.Entidades.PreConhecimentoDeTransporteEletronico();

            DateTime dataPrevistaEntrega;
            DateTime.TryParseExact(cteIntegracao.DataPrevistaEntrega, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dataPrevistaEntrega);

            DateTime dataEmissao;
            DateTime.TryParseExact(cteIntegracao.DataEmissao, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dataEmissao);

            preCte.Empresa = empresa;
            preCte.TipoTomador = cteIntegracao.TipoTomador;
            preCte.TipoPagamento = cteIntegracao.TipoPagamento;
            preCte.IncluirICMSNoFrete = cteIntegracao.IncluirICMSNoFrete;
            preCte.PercentualICMSIncluirNoFrete = cteIntegracao.PercentualICMSIncluirNoFrete;
            preCte.ValorFrete = cteIntegracao.ValorFrete;
            preCte.ValorPrestacaoServico = cteIntegracao.ValorTotalPrestacaoServico;
            preCte.ValorAReceber = cteIntegracao.ValorAReceber;
            preCte.ValorTotalMercadoria = cteIntegracao.ValorTotalMercadoria;
            preCte.ProdutoPredominante = cteIntegracao.ProdutoPredominante.Left(60);
            preCte.SimplesNacional = preCte.Empresa.OptanteSimplesNacional ? Dominio.Enumeradores.OpcaoSimNao.Sim : Dominio.Enumeradores.OpcaoSimNao.Nao;
            preCte.Retira = cteIntegracao.Retira;
            preCte.DetalhesRetira = cteIntegracao.DetalhesRetira;
            preCte.RNTRC = preCte.Empresa.RegistroANTT;
            preCte.CIOT = cteIntegracao.CIOT;

            if (preCte.Empresa.Configuracao != null)
                preCte.DataPrevistaEntrega = dataPrevistaEntrega == DateTime.MinValue ? DateTime.Now.AddDays(preCte.Empresa.Configuracao.DiasParaEntrega) : dataPrevistaEntrega;

            preCte.TipoCTE = cteIntegracao.TipoCTe;
            preCte.TipoImpressao = cteIntegracao.TipoImpressao;
            preCte.TipoServico = cteIntegracao.TipoServico;
            preCte.DataEmissao = dataEmissao == DateTime.MinValue ? DateTime.Now : dataEmissao;
            preCte.ModalTransporte = repModalTransporte.BuscarPorCodigo(1, false);
            preCte.Lotacao = cteIntegracao.Lotacao;
            preCte.LocalidadeEmissao = preCte.Empresa.Localidade;

            if (modeloDocumentoFiscal.DocumentoTipoCRT)
            {
                preCte.LocalidadeInicioPrestacao = repositorioCliente.BuscarLocalidadePorCPFCNPJ(cteIntegracao.Remetente.Codigo);
                preCte.LocalidadeTerminoPrestacao = repositorioCliente.BuscarLocalidadePorCPFCNPJ(cteIntegracao.Destinatario.Codigo);
            }
            else
            {
                preCte.LocalidadeInicioPrestacao = repLocalidade.BuscarPorCodigoIBGE(cteIntegracao.CodigoIBGECidadeInicioPrestacao);
                preCte.LocalidadeTerminoPrestacao = repLocalidade.BuscarPorCodigoIBGE(cteIntegracao.CodigoIBGECidadeTerminoPrestacao);
            }

            preCte.Versao = preCte.Empresa.Versao;
            preCte.ObservacoesGerais = cteIntegracao.ObservacoesGerais;
            preCte.ObservacaoDaCarga = cteIntegracao.ObservacaoDaCarga;
            preCte.CodigoPedidoCliente = codigoPedidoCliente ?? string.Empty;
            preCte.CodigoCanalEntrega = codigoCanalEntrega;
            preCte.PesoTotalCarga = preCTePesoCarga > 0 ? preCTePesoCarga : 0;
            preCte.ValoresFormulaCalculoFreteCarga = preCTeValoresFormula ?? string.Empty;

            preCte.ModeloDocumentoFiscal = modeloDocumentoFiscal;
            preCte.IncluirISSNoFrete = cteIntegracao.IncluirISSNoFrete;
            if (cteIntegracao.ISS != null)
            {
                preCte.AliquotaISS = cteIntegracao.ISS.Aliquota;
                preCte.ISSRetido = cteIntegracao.ISS.PercentualRetencao > 0m ? true : false;
                preCte.PercentualISSRetido = cteIntegracao.ISS.PercentualRetencao;
                preCte.ValorISSRetido = cteIntegracao.ISS.ValorRetencao;
                preCte.BaseCalculoISS = cteIntegracao.ISS.BaseCalculo;
                preCte.ValorISS = cteIntegracao.ISS.Valor;
            }

            ObterParticipante(ref preCte, cteIntegracao.Remetente, Dominio.Enumeradores.TipoTomador.Remetente);
            ObterParticipante(ref preCte, cteIntegracao.Expedidor, Dominio.Enumeradores.TipoTomador.Expedidor);
            ObterParticipante(ref preCte, cteIntegracao.Recebedor, Dominio.Enumeradores.TipoTomador.Recebedor);
            ObterParticipante(ref preCte, cteIntegracao.Destinatario, Dominio.Enumeradores.TipoTomador.Destinatario);

            if (cteIntegracao.CFOP > 0)
                preCte.CFOP = repCFOP.BuscarPorNumero(cteIntegracao.CFOP);
            else
                SetarCFOPENaturezaPorTabelaDeAliquotas(ref preCte);

            if (cteIntegracao.TipoCTe == Dominio.Enumeradores.TipoCTE.Complemento)
                preCte.ChaveCTESubComp = cteIntegracao.ChaveCTESubstituicaoComplementar;

            preCte.Cancelado = "N";

            repPreCTe.Inserir(preCte);

            if (preCte.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros)
                ObterParticipante(ref preCte, cteIntegracao.Tomador, Dominio.Enumeradores.TipoTomador.Outros);
            else
            {
                Dominio.Entidades.ParticipanteCTe part = preCte.ObterParticipante(Dominio.Enumeradores.TipoTomador.Outros);

                if (part != null)
                {
                    Repositorio.ParticipanteCTe repParticipante = new Repositorio.ParticipanteCTe(_unitOfWork);

                    preCte.SetarParticipante(null, Dominio.Enumeradores.TipoTomador.Outros, null, null);

                    repParticipante.Deletar(part);
                }
            }

            preCte.ClienteRetira = serCTE.ObterCliente(empresa, cteIntegracao.ClienteRetira, _unitOfWork);
            preCte.ClienteEntrega = serCTE.ObterCliente(empresa, cteIntegracao.ClienteEntrega, _unitOfWork);

            SalvarInformacoesComponentesDaPrestacao(preCte, cteIntegracao.ComponentesDaPrestacao);
            SalvarInformacoesVeiculos(preCte, cteIntegracao);
            SalvarInformacoesMotoristas(preCte, cteIntegracao.Motoristas);
            SalvarInformacoesDeQuantidadeDaCarga(preCte, cteIntegracao.QuantidadesCarga);
            SalvarInformacoesDeSeguroDaCarga(preCte, cteIntegracao.Seguros);
            SalvarInformacoesDocumentos(preCte, cteIntegracao.Documentos);
            SalvarInformacoesDocumentosAnteriores(preCte, cteIntegracao.DocumentosTransporteAnteriores);
            SalvarObservacoesContribuinte(preCte, cteIntegracao.ObservacoesContribuinte);
            SalvarObservacoesFisco(preCte, cteIntegracao.ObservacoesFisco);
            SalvarInformacoesProdutosPerigosos(preCte, cteIntegracao.ProdutosPerigosos);
            CalcularImpostos(ref preCte, cteIntegracao);

            repPreCTe.Atualizar(preCte);

            return preCte;
        }

        public string BuscarXMLPreCte(Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCte)
        {
            System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo("en-US");
            MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTe tCte = new MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTe();
            tCte.infCte = new MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCte();
            tCte.infCte.versao = "4.00";
            tCte.infCte.ide = new MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIde();

            if (preCte.Empresa != null)
            {
                tCte.infCte.ide.cUF = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCodUfIBGE)preCte.Empresa.Localidade.Estado.CodigoIBGE;
                tCte.infCte.ide.tpAmb = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TAmb)preCte.Empresa.TipoAmbiente;
            }
            else
            {
                tCte.infCte.ide.cUF = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCodUfIBGE)preCte.TransportadorTerceiro.Localidade.Estado.CodigoIBGE;
                tCte.infCte.ide.tpAmb = MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TAmb.Item1;
            }


            tCte.infCte.ide.cCT = preCte.Codigo.ToString();
            tCte.infCte.ide.CFOP = preCte.CFOP.CodigoCFOP.ToString();
            tCte.infCte.ide.natOp = preCte.CFOP.NaturezaDaOperacao.Descricao;
            tCte.infCte.ide.mod = MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TModCT.Item57;
            tCte.infCte.ide.serie = "";
            tCte.infCte.ide.nCT = "";
            tCte.infCte.ide.dhEmi = "";
            tCte.infCte.ide.tpImp = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeTpImp)preCte.TipoImpressao;
            tCte.infCte.ide.tpEmis = MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeTpEmis.Item1;
            tCte.infCte.ide.cDV = "";

            tCte.infCte.ide.tpCTe = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TFinCTe)preCte.TipoCTE;
            tCte.infCte.ide.procEmi = MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TProcEmi.Item0;
            tCte.infCte.ide.verProc = "4.00";

            tCte.infCte.ide.indIEToma = MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeIndIEToma.Item1;
            tCte.infCte.ide.cMunEnv = preCte.LocalidadeEmissao.CodigoIBGE.ToString();
            tCte.infCte.ide.xMunEnv = preCte.LocalidadeEmissao.Descricao;
            tCte.infCte.ide.UFEnv = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TUf)preCte.LocalidadeEmissao.Estado.CodigoIBGE;
            tCte.infCte.ide.modal = MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TModTransp.Item01;
            tCte.infCte.ide.tpServ = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeTpServ)preCte.TipoServico;
            tCte.infCte.ide.cMunIni = preCte.LocalidadeInicioPrestacao.CodigoIBGE.ToString();
            tCte.infCte.ide.xMunIni = preCte.LocalidadeInicioPrestacao.Descricao;
            tCte.infCte.ide.UFIni = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TUf)preCte.LocalidadeInicioPrestacao.Estado.CodigoIBGE;
            tCte.infCte.ide.cMunFim = preCte.LocalidadeTerminoPrestacao.CodigoIBGE.ToString();
            tCte.infCte.ide.xMunFim = preCte.LocalidadeTerminoPrestacao.Descricao;
            tCte.infCte.ide.UFFim = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TUf)preCte.LocalidadeTerminoPrestacao.Estado.CodigoIBGE;
            tCte.infCte.ide.retira = preCte.Retira == Dominio.Enumeradores.OpcaoSimNao.Sim ? MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeRetira.Item0 : MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeRetira.Item1;
            tCte.infCte.compl = new MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteCompl();
            tCte.infCte.compl.xObs = preCte.ObservacoesGerais;
            tCte.infCte.compl.xCaracAd = preCte.CaracteristicaTransporte;
            tCte.infCte.compl.xCaracSer = preCte.CaracteristicaServico;

            setarTomador(preCte, ref tCte);
            setarEmitente(preCte, ref tCte);
            setarRemetente(preCte, ref tCte);
            setarExpedidor(preCte, ref tCte);
            setarRecebedor(preCte, ref tCte);
            setarDestinatario(preCte, ref tCte);

            setarComponentes(preCte, cultureInfo, ref tCte);
            setarImpostos(preCte, cultureInfo, ref tCte);
            setarInfo(preCte, cultureInfo, ref tCte);

            System.IO.StringWriter stWrite = new System.IO.StringWriter();
            System.Xml.Serialization.XmlSerializer serializerCTE = new System.Xml.Serialization.XmlSerializer(tCte.GetType());
            serializerCTE.Serialize(stWrite, tCte);
            return stWrite.ToString();
        }

        public void GerarNFSPorPreCTe(Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCte, Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe, int numero, int serie, string xml, byte[] pdfDanfse, DateTime dataEmissao, decimal aliquotaISS, decimal baseCalculo, decimal valorISS, decimal percentualRetencao, decimal valorRetencao, decimal valorPIS, decimal valorCOFINS, decimal valorIR, decimal valorCSLL, decimal valorFrete, decimal valorPrestacaoServico, decimal valorReceber, string numeroRPS, string observacao, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
            Repositorio.XMLCTe repXMLCTe = new Repositorio.XMLCTe(_unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);
            Repositorio.DocumentosPreCTE repDocumentosPreCte = new Repositorio.DocumentosPreCTE(_unitOfWork);

            Servicos.Embarcador.Carga.CTe serCte = new Servicos.Embarcador.Carga.CTe(_unitOfWork);

            Dominio.ObjetosDeValor.CTe.CTe cte = new Dominio.ObjetosDeValor.CTe.CTe
            {
                Documentos = new List<Dominio.ObjetosDeValor.CTe.Documento>()
            };

            List<Dominio.Entidades.DocumentosPreCTE> documentosPreCte = repDocumentosPreCte.BuscarPorPreCte(preCte.Codigo);

            foreach (DocumentosPreCTE doc in documentosPreCte)
            {
                cte.Documentos.Add(new Dominio.ObjetosDeValor.CTe.Documento
                {
                    Numero = doc.Numero,
                    Serie = doc.Serie,
                    ModeloDocumentoFiscal = doc.NumeroModelo,
                    DataEmissao = doc.DataEmissao.ToString(),
                    Volume = doc.Volume,
                    Peso = doc.Peso,
                    BaseCalculoICMS = doc.BaseCalculoICMS,
                    ValorICMS = doc.ValorICMS,
                    BaseCalculoICMSST = doc.BaseCalculoICMSST,
                    ValorICMSST = doc.ValorICMSST,
                    ValorProdutos = doc.ValorProdutos,
                    CFOP = doc.CFOP,
                    ItemPrincipal = doc.ItemPrincipal,
                    ChaveNFE = doc.ChaveNFE,
                    PINSuframa = doc.PINSuframa,
                    Descricao = doc.Descricao,
                    NCMPredominante = doc.NCMPredominante,
                });
            }

            cte.Veiculos = new List<Dominio.ObjetosDeValor.CTe.Veiculo>();
            cte.Numero = numero;
            cte.Serie = serie;
            cte.ValorAReceber = valorReceber;
            cte.ValorFrete = valorFrete;
            cte.ValorTotalPrestacaoServico = valorPrestacaoServico;
            cte.IncluirISSNoFrete = preCte.IncluirISSNoFrete;
            cte.SerieRPS = numeroRPS;
            cte.ObservacoesGerais = observacao;

            cte.PIS = new Dominio.ObjetosDeValor.CTe.ImpostoPIS()
            {
                Valor = valorPIS
            };

            cte.COFINS = new Dominio.ObjetosDeValor.CTe.ImpostoCOFINS()
            {
                Valor = valorCOFINS
            };

            cte.IR = new Dominio.ObjetosDeValor.CTe.ImpostoIR()
            {
                Valor = valorIR
            };

            cte.CSLL = new Dominio.ObjetosDeValor.CTe.ImpostoCSLL()
            {
                Valor = valorCSLL
            };

            cte.ISS = new Dominio.ObjetosDeValor.CTe.ImpostoISS()
            {
                Aliquota = aliquotaISS,
                BaseCalculo = baseCalculo,
                PercentualRetencao = percentualRetencao,
                Valor = valorISS,
                ValorRetencao = valorRetencao
            };

            Dominio.Entidades.ConhecimentoDeTransporteEletronico cteIntegrado = serCte.GerarCTe(preCte, cargaCTe, cte, _unitOfWork, tipoServicoMultisoftware);

            cteIntegrado.Status = "A";
            cteIntegrado.DataEmissao = dataEmissao;
            cteIntegrado.DataAutorizacao = cteIntegrado.DataEmissao;
            repCTe.Atualizar(cteIntegrado);

            if (!string.IsNullOrWhiteSpace(xml))
            {
                Dominio.Entidades.XMLCTe xmlCTe = new Dominio.Entidades.XMLCTe();
                xmlCTe.XML = xml;
                xmlCTe.Tipo = Dominio.Enumeradores.TipoXMLCTe.Autorizacao;
                xmlCTe.CTe = cteIntegrado;
                repXMLCTe.Inserir(xmlCTe);
            }

            new Servicos.Embarcador.Carga.PreCTe(_unitOfWork).VincularPedidoXMLNotaAoCTe(cteIntegrado, cargaCTe.Carga, cargaCTe, _unitOfWork);

            cargaCTe.CTe = cteIntegrado;
            repCargaCTe.Atualizar(cargaCTe);

            if (pdfDanfse?.Length > 0)
                SalvarDANFSE(cteIntegrado.Numero, serie, cteIntegrado.Codigo, preCte.Empresa, pdfDanfse, _unitOfWork);
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private void setarInfo(Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCte, System.Globalization.CultureInfo cultureInfo, ref MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTe tCte)
        {

            MultiSoftware.CTe.v400.ModalRodoviario.rodo rodo = buscarRodo(preCte, cultureInfo);

            System.Xml.XmlDocument document = new System.Xml.XmlDocument();
            using (System.Xml.XmlWriter xmlWriter = document.CreateNavigator().AppendChild())
            {
                new System.Xml.Serialization.XmlSerializer(typeof(MultiSoftware.CTe.v400.ModalRodoviario.rodo)).Serialize(xmlWriter, rodo);
            }


            Repositorio.InformacaoCargaPreCTE repInformacaoCargaPreCte = new Repositorio.InformacaoCargaPreCTE(_unitOfWork);
            List<Dominio.Entidades.InformacaoCargaPreCTE> informacoesCargaPreCTe = repInformacaoCargaPreCte.BuscarPorPreCTe(preCte.Codigo);

            Repositorio.DocumentosPreCTE repDocumentosPreCte = new Repositorio.DocumentosPreCTE(_unitOfWork);
            List<Dominio.Entidades.DocumentosPreCTE> documentosPreCte = repDocumentosPreCte.BuscarPorPreCte(preCte.Codigo);

            Repositorio.SeguroPreCTE repSeguroPreCTE = new Repositorio.SeguroPreCTE(_unitOfWork);
            List<Dominio.Entidades.SeguroPreCTE> segurosPreCTE = repSeguroPreCTE.BuscarPorPreCte(preCte.Codigo);


            Repositorio.DocumentoDeTransporteAnteriorPreCTE repDocumentoDeTransporteAnteriorPreCTE = new Repositorio.DocumentoDeTransporteAnteriorPreCTE(_unitOfWork);
            List<Dominio.Entidades.DocumentoDeTransporteAnteriorPreCTE> documentosTransportaAnterior = repDocumentoDeTransporteAnteriorPreCTE.BuscarPorPreCte(preCte.Codigo);

            List<Dominio.Entidades.Cliente> emissoresDocumentosAnteriores = (from obj in documentosTransportaAnterior select obj.Emissor).Distinct().ToList();

            List<MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAnt> docsAnt = new List<MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAnt>();

            foreach (Dominio.Entidades.Cliente emissor in emissoresDocumentosAnteriores)
            {
                List<Dominio.Entidades.DocumentoDeTransporteAnteriorPreCTE> documentosTransportaAnteriorEmissor = (from obj in documentosTransportaAnterior where obj.Emissor.CPF_CNPJ == emissor.CPF_CNPJ select obj).ToList();


                List<MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAntIdDocAntIdDocAntPap> papeis = new List<MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAntIdDocAntIdDocAntPap>();
                List<MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAntIdDocAntIdDocAntEle> eletronicos = new List<MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAntIdDocAntIdDocAntEle>();

                foreach (Dominio.Entidades.DocumentoDeTransporteAnteriorPreCTE documentoTransporteAnteriorEmissor in documentosTransportaAnteriorEmissor)
                {
                    if (!string.IsNullOrWhiteSpace(documentoTransporteAnteriorEmissor.Chave))
                    {
                        eletronicos.Add(new MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAntIdDocAntIdDocAntEle()
                        {
                            chCTe = documentoTransporteAnteriorEmissor.Chave
                        });
                    }
                    else
                    {
                        papeis.Add(new MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAntIdDocAntIdDocAntPap()
                        {
                            dEmi = documentoTransporteAnteriorEmissor.DataEmissao != null ? documentoTransporteAnteriorEmissor.DataEmissao.Value.ToString("yyyy-MM-dd") : "",
                            nDoc = documentoTransporteAnteriorEmissor.Numero,
                            serie = documentoTransporteAnteriorEmissor.Serie,
                            subser = documentoTransporteAnteriorEmissor.Serie,
                            tpDoc = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAntIdDocAntIdDocAntPapTpDoc)int.Parse(documentoTransporteAnteriorEmissor.Tipo)
                        });
                    }

                }

                MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAnt docAnt = new MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAnt()
                {
                    Item = emissor.CPF_CNPJ_SemFormato,
                    ItemElementName = emissor.Tipo == "J" ? MultiSoftware.CTe.v400.ConhecimentoDeTransporte.ItemChoiceType6.CNPJ : MultiSoftware.CTe.v400.ConhecimentoDeTransporte.ItemChoiceType6.CPF,
                    IE = emissor.IE_RG,
                    UF = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TUf)emissor.Localidade.Estado.CodigoIBGE,
                    xNome = emissor.Nome
                };

                List<MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAntIdDocAnt> idsDocAnt = new List<MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAntIdDocAnt>();
                if (eletronicos.Count > 0)
                {
                    idsDocAnt.Add(new MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAntIdDocAnt()
                    {
                        Items = eletronicos.ToArray(),
                    });
                }
                if (papeis.Count > 0)
                {
                    idsDocAnt.Add(new MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormEmiDocAntIdDocAnt()
                    {
                        Items = papeis.ToArray()
                    });
                }
                docAnt.idDocAnt = idsDocAnt.ToArray();
                docsAnt.Add(docAnt);
            }

            if (preCte.TipoCTE == Dominio.Enumeradores.TipoCTE.Normal)
            {
                MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNorm info = new MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNorm()
                {
                    infCarga = new MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfCarga()
                    {
                        vCarga = preCte.ValorTotalMercadoria.ToString("f2", cultureInfo),
                        proPred = preCte.ProdutoPredominante,
                        infQ = (from obj in informacoesCargaPreCTe
                                select new MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfCargaInfQ
                                {
                                    cUnid = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfCargaInfQCUnid)int.Parse(obj.UnidadeMedida),
                                    qCarga = obj.Quantidade.ToString("f4", cultureInfo),
                                    tpMed = obj.Tipo
                                }).ToArray(),
                        xOutCat = preCte.OutrasCaracteristicasDaCarga
                    },
                    infDoc = new MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDoc()
                    {

                    },
                    infModal = new MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfModal()
                    {
                        versaoModal = "4.00",
                        Any = document.DocumentElement
                    },
                    docAnt = docsAnt.ToArray()
                };

                if ((from obj in documentosPreCte where obj.NumeroModelo == "55" select obj).Any())
                {
                    info.infDoc.Items = (from obj in documentosPreCte
                                         select new MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNFe
                                         {
                                             chave = obj.ChaveNFE
                                         }).ToArray();
                }
                else if ((from obj in documentosPreCte where obj.NumeroModelo == "00" || obj.NumeroModelo == "10" || obj.NumeroModelo == "99" select obj).Any())
                {
                    info.infDoc.Items = (from obj in documentosPreCte
                                         select new MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfOutros
                                         {
                                             dEmi = obj.DataEmissao.ToString("yyyy-MM-dd"),
                                             descOutros = obj.Descricao,
                                             nDoc = obj.Numero,
                                             tpDoc = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfOutrosTpDoc)int.Parse(obj.NumeroModelo),
                                             vDocFisc = obj.Valor.ToString("f2", cultureInfo)
                                         }).ToArray();
                }
                else if ((from obj in documentosPreCte where obj.NumeroModelo == "01" || obj.NumeroModelo == "04" select obj).Any())
                {
                    info.infDoc.Items = (from obj in documentosPreCte
                                         select new MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCTeNormInfDocInfNF
                                         {
                                             dEmi = obj.DataEmissao.ToString("yyyy-MM-dd"),
                                             mod = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TModNF)int.Parse(obj.NumeroModelo),
                                             nCFOP = obj.CFOP,
                                             nPeso = obj.Peso.ToString("f3", cultureInfo),
                                             PIN = obj.PINSuframa,
                                             nDoc = obj.Numero,
                                             serie = obj.Serie,
                                             vBC = obj.BaseCalculoICMS.ToString("f2", cultureInfo),
                                             vBCST = obj.BaseCalculoICMSST.ToString("f2", cultureInfo),
                                             vICMS = obj.ValorICMS.ToString("f2", cultureInfo),
                                             vNF = obj.Valor.ToString("f2", cultureInfo),
                                             vProd = obj.ValorProdutos.ToString("f2", cultureInfo),
                                             vST = obj.ValorICMSST.ToString("f2", cultureInfo)
                                         }).ToArray();
                }
                tCte.infCte.Items = new object[] { info };
            }
            else
            {
                tCte.infCte.Items = new object[] { new MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteInfCteComp { chCTe = preCte.ChaveCTESubComp } };
            }

        }

        private void setarImpostos(Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCte, System.Globalization.CultureInfo cultureInfo, ref MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTe tCte)
        {
            tCte.infCte.imp = new MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteImp();
            tCte.infCte.imp.ICMS = new MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TImp();

            switch (preCte.CST)
            {
                case "00":
                    tCte.infCte.imp.ICMS.Item = new MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TImpICMS00()
                    {
                        CST = MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TImpICMS00CST.Item00,
                        pICMS = preCte.AliquotaICMS.ToString("f2", cultureInfo),
                        vBC = preCte.BaseCalculoICMS.ToString("f2", cultureInfo),
                        vICMS = preCte.ValorICMS.ToString("f2", cultureInfo)
                    };
                    break;
                case "20":
                    tCte.infCte.imp.ICMS.Item = new MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TImpICMS20()
                    {
                        CST = MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TImpICMS20CST.Item20,
                        pICMS = preCte.AliquotaICMS.ToString("f2", cultureInfo),
                        vBC = preCte.BaseCalculoICMS.ToString("f2", cultureInfo),
                        vICMS = preCte.ValorICMS.ToString("f2", cultureInfo),
                        pRedBC = preCte.PercentualReducaoBaseCalculoICMS.ToString("f2", cultureInfo)
                    };
                    break;
                case "40":
                case "41":
                case "51":
                    tCte.infCte.imp.ICMS.Item = new MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TImpICMS45()
                    {
                        CST = preCte.CST == "40" ? MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TImpICMS45CST.Item40 : preCte.CST == "41" ? MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TImpICMS45CST.Item41 : MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TImpICMS45CST.Item51
                    };
                    break;
                case "60":
                    tCte.infCte.imp.ICMS.Item = new MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TImpICMS60()
                    {
                        CST = MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TImpICMS60CST.Item60,
                        pICMSSTRet = preCte.AliquotaICMS.ToString("f2", cultureInfo),
                        vBCSTRet = preCte.BaseCalculoICMS.ToString("f2", cultureInfo),
                        vICMSSTRet = preCte.ValorICMS.ToString("f2", cultureInfo),
                        vCred = preCte.ValorPresumido.ToString("f2", cultureInfo)
                    };
                    break;
                case "91":
                    tCte.infCte.imp.ICMS.Item = new MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TImpICMS90()
                    {
                        CST = MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TImpICMS90CST.Item90,
                        pICMS = preCte.AliquotaICMS.ToString("f2", cultureInfo),
                        vBC = preCte.BaseCalculoICMS.ToString("f2", cultureInfo),
                        vICMS = preCte.ValorICMS.ToString("f2", cultureInfo),
                        pRedBC = preCte.PercentualReducaoBaseCalculoICMS.ToString("f2", cultureInfo),
                        vCred = preCte.ValorPresumido.ToString("f2", cultureInfo)
                    };
                    break;
                case "90":
                    tCte.infCte.imp.ICMS.Item = new MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TImpICMSOutraUF()
                    {
                        CST = MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TImpICMSOutraUFCST.Item90,
                        pICMSOutraUF = preCte.AliquotaICMS.ToString("f2", cultureInfo),
                        vBCOutraUF = preCte.BaseCalculoICMS.ToString("f2", cultureInfo),
                        vICMSOutraUF = preCte.ValorICMS.ToString("f2", cultureInfo),
                        pRedBCOutraUF = preCte.PercentualReducaoBaseCalculoICMS.ToString("f2", cultureInfo)
                    };
                    break;
                case "":
                    tCte.infCte.imp.ICMS.Item = new MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TImpICMSSN()
                    {
                        CST = MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TImpICMSSNCST.Item90,
                        indSN = MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TImpICMSSNIndSN.Item1
                    };
                    break;
                default:
                    tCte.infCte.imp.ICMS.Item = null;
                    break;
            }

            setarIBSCBS(preCte, cultureInfo, ref tCte);

            if (tCte.infCte.imp.IBSCBS != null && preCte.OutrasAliquotas != null)
            {
                decimal valorTotalDocumento = preCte.ValorPrestacaoServico;
                Dominio.ObjetosDeValor.Embarcador.Imposto.OutraAliquota impostoIBSCSB = new Servicos.Embarcador.Imposto.ImpostoIBSCBS(_unitOfWork).ObterOutrasAliquotasIBSCBS(preCte.OutrasAliquotas.Codigo);
                if ((impostoIBSCSB?.SomarImpostosDocumento ?? false) || (preCte?.OutrasAliquotas?.CalcularImpostoDocumento ?? false))
                    valorTotalDocumento = valorTotalDocumento + preCte.ValorIBSMunicipal + preCte.ValorIBSEstadual + preCte.ValorCBS;

                tCte.infCte.imp.vTotDFe = Math.Round(valorTotalDocumento, 2, MidpointRounding.AwayFromZero).ToString("f2", cultureInfo);
            }
        }

        private void setarIBSCBS(Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCte, System.Globalization.CultureInfo cultureInfo, ref MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTe tCte)
        {
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoIntegracaoEmissorDocumento configuracaoIntegracaoEmissorDocumento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoIntegracaoEmissorDocumento(_unitOfWork).BuscarConfiguracaoPadrao();

            if ((preCte.OutrasAliquotas == null || (!(preCte.Empresa.Configuracao?.EnviarNovoImposto ?? false))) || DateTime.Now <= configuracaoIntegracaoEmissorDocumento.DataLiberacaoImpostos)
                return;

            tCte.infCte.imp.IBSCBS = new MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TTribCTe()
            {
                CST = preCte.CST,
                cClassTrib = preCte.ClassificacaoTributariaIBSCBS,
            };

            if (preCte.BaseCalculoIBSCBS == 0)
                return;

            tCte.infCte.imp.IBSCBS.gIBSCBS = new MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCIBS()
            {
                vBC = preCte.BaseCalculoIBSCBS.ToString("f2", cultureInfo),
                gIBSUF = new MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCIBSGIBSUF()
                {
                    pIBSUF = preCte.AliquotaIBSEstadual.ToString("f4", cultureInfo),
                    vIBSUF = preCte.ValorIBSEstadual.ToString("f2", cultureInfo)
                },
                gIBSMun = new MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCIBSGIBSMun()
                {
                    pIBSMun = preCte.AliquotaIBSMunicipal.ToString("f4", cultureInfo),
                    vIBSMun = preCte.ValorIBSMunicipal.ToString("f2", cultureInfo)
                },
                vIBS = (preCte.ValorIBSEstadual + preCte.ValorIBSMunicipal).ToString("f2", cultureInfo),
                gCBS = new MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCIBSGCBS()
                {
                    pCBS = preCte.AliquotaCBS.ToString("f4", cultureInfo),
                    vCBS = preCte.ValorCBS.ToString("f2", cultureInfo)
                }
            };

            if (preCte.PercentualReducaoIBSEstadual > 0)
                tCte.infCte.imp.IBSCBS.gIBSCBS.gIBSUF.gRed = new MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TRed()
                {
                    pRedAliq = preCte.PercentualReducaoIBSEstadual.ToString("f2", cultureInfo),
                    pAliqEfet = (preCte.AliquotaIBSEstadual - (preCte.AliquotaIBSEstadual * (preCte.PercentualReducaoIBSEstadual / 100))).ToString("f2", cultureInfo)
                };

            if (preCte.PercentualReducaoIBSMunicipal > 0)
                tCte.infCte.imp.IBSCBS.gIBSCBS.gIBSMun.gRed = new MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TRed()
                {
                    pRedAliq = preCte.PercentualReducaoIBSMunicipal.ToString("f2", cultureInfo),
                    pAliqEfet = (preCte.AliquotaIBSMunicipal - (preCte.AliquotaIBSMunicipal * (preCte.PercentualReducaoIBSMunicipal / 100))).ToString("f2", cultureInfo)
                };

            if (preCte.PercentualReducaoCBS > 0)
                tCte.infCte.imp.IBSCBS.gIBSCBS.gCBS.gRed = new MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TRed()
                {
                    pRedAliq = preCte.PercentualReducaoCBS.ToString("f2", cultureInfo),
                    pAliqEfet = (preCte.AliquotaCBS - (preCte.AliquotaCBS * (preCte.PercentualReducaoCBS / 100))).ToString("f2", cultureInfo)
                };
        }

        private MultiSoftware.CTe.v400.ModalRodoviario.rodo buscarRodo(Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCte, System.Globalization.CultureInfo cultureInfo)
        {
            Repositorio.VeiculoPreCTE repVeiculoPreCTE = new Repositorio.VeiculoPreCTE(_unitOfWork);
            List<Dominio.Entidades.VeiculoPreCTE> veiculosPreCTe = repVeiculoPreCTE.BuscarPorPreCte(preCte.Codigo);

            Repositorio.MotoristaPreCTE repMotoristaPreCte = new Repositorio.MotoristaPreCTE(_unitOfWork);
            List<Dominio.Entidades.MotoristaPreCTE> motoristasPreCTe = repMotoristaPreCte.BuscarPorPreCte(preCte.Codigo);

            MultiSoftware.CTe.v400.ModalRodoviario.rodo rodo = new MultiSoftware.CTe.v400.ModalRodoviario.rodo();
            rodo.RNTRC = preCte.RNTRC;

            return rodo;
        }

        private void setarComponentes(Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCte, System.Globalization.CultureInfo cultureInfo, ref MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTe tCte)
        {
            tCte.infCte.vPrest = new MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteVPrest();
            tCte.infCte.vPrest.vTPrest = preCte.ValorPrestacaoServico.ToString("f2", cultureInfo);
            tCte.infCte.vPrest.vRec = preCte.ValorAReceber.ToString("f2", cultureInfo);
            Repositorio.ComponentePrestacaoPreCTE repComponentesPrestacaoPreCTe = new Repositorio.ComponentePrestacaoPreCTE(_unitOfWork);

            List<Dominio.Entidades.ComponentePrestacaoPreCTE> componentesPreCTe = repComponentesPrestacaoPreCTe.BuscarPorPreCTe(preCte.Codigo);
            List<MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteVPrestComp> cpTcte = new List<MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteVPrestComp>();
            foreach (Dominio.Entidades.ComponentePrestacaoPreCTE componentePrestacaoPreCTe in componentesPreCTe)
            {
                cpTcte.Add(new MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteVPrestComp()
                {
                    vComp = componentePrestacaoPreCTe.Valor.ToString("f2", cultureInfo),
                    xNome = componentePrestacaoPreCTe.Nome
                });
            }
            tCte.infCte.vPrest.Comp = cpTcte.ToArray();
        }

        private void setarDestinatario(Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCte, ref MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTe tCte)
        {
            if (preCte.Destinatario != null)
            {
                tCte.infCte.dest = new MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteDest()
                {
                    email = preCte.Destinatario.Email,
                    enderDest = retornarEnderecoParticipamento(preCte.Destinatario),
                    fone = preCte.Destinatario.Telefone1,
                    Item = preCte.Destinatario.CPF_CNPJ_SemFormato,
                    ItemElementName = preCte.Destinatario.Tipo == Dominio.Enumeradores.TipoPessoa.Fisica ? MultiSoftware.CTe.v400.ConhecimentoDeTransporte.ItemChoiceType5.CPF : MultiSoftware.CTe.v400.ConhecimentoDeTransporte.ItemChoiceType5.CNPJ,
                    IE = preCte.Destinatario.IE_RG,
                    xNome = Utilidades.String.Left(preCte.Destinatario.Nome, 60),
                    ISUF = preCte.Destinatario.InscricaoSuframa
                };
            }
        }

        private void setarRecebedor(Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCte, ref MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTe tCte)
        {
            if (preCte.Recebedor != null)
            {
                tCte.infCte.receb = new MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteReceb()
                {
                    email = preCte.Recebedor.Email,
                    enderReceb = retornarEnderecoParticipamento(preCte.Recebedor),
                    fone = preCte.Recebedor.Telefone1,
                    Item = retornarCPFCNPJParticipamente(preCte.Recebedor),
                    ItemElementName = preCte.Recebedor.Tipo == Dominio.Enumeradores.TipoPessoa.Fisica ? MultiSoftware.CTe.v400.ConhecimentoDeTransporte.ItemChoiceType4.CPF : MultiSoftware.CTe.v400.ConhecimentoDeTransporte.ItemChoiceType4.CNPJ,
                    IE = preCte.Recebedor.IE_RG,
                    xNome = Utilidades.String.Left(preCte.Recebedor.Nome, 60)
                };
            }
        }

        private void setarExpedidor(Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCte, ref MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTe tCte)
        {
            if (preCte.Expedidor != null)
            {
                tCte.infCte.exped = new MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteExped()
                {
                    email = preCte.Expedidor.Email,
                    enderExped = retornarEnderecoParticipamento(preCte.Expedidor),
                    fone = preCte.Expedidor.Telefone1,
                    Item = retornarCPFCNPJParticipamente(preCte.Expedidor),
                    ItemElementName = preCte.Expedidor.Tipo == Dominio.Enumeradores.TipoPessoa.Fisica ? MultiSoftware.CTe.v400.ConhecimentoDeTransporte.ItemChoiceType3.CPF : MultiSoftware.CTe.v400.ConhecimentoDeTransporte.ItemChoiceType3.CNPJ,
                    IE = preCte.Expedidor.IE_RG,
                    xNome = Utilidades.String.Left(preCte.Expedidor.Nome, 60)
                };
            }
        }

        private void setarRemetente(Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCte, ref MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTe tCte)
        {
            if (preCte.Remetente != null)
            {
                tCte.infCte.rem = new MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteRem()
                {
                    email = preCte.Remetente.Email,
                    enderReme = retornarEnderecoParticipamento(preCte.Remetente),
                    fone = preCte.Remetente.Telefone1,
                    Item = retornarCPFCNPJParticipamente(preCte.Remetente),
                    ItemElementName = preCte.Remetente.Tipo == Dominio.Enumeradores.TipoPessoa.Fisica ? MultiSoftware.CTe.v400.ConhecimentoDeTransporte.ItemChoiceType2.CPF : MultiSoftware.CTe.v400.ConhecimentoDeTransporte.ItemChoiceType2.CNPJ,
                    IE = preCte.Remetente.IE_RG,
                    xFant = Utilidades.String.Left(preCte.Remetente.NomeFantasia, 60),
                    xNome = Utilidades.String.Left(preCte.Remetente.Nome, 60)
                };
            }
        }

        private void setarEmitente(Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCte, ref MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTe tCte)
        {
            if (preCte.Empresa != null)
            {
                tCte.infCte.emit = new MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteEmit()
                {
                    enderEmit = new MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TEndeEmi()
                    {
                        CEP = Utilidades.String.OnlyNumbers(preCte.Empresa.CEP),
                        cMun = preCte.Empresa.Localidade.CodigoIBGE.ToString(),
                        nro = preCte.Empresa.Numero,
                        UF = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TUF_sem_EX)preCte.Empresa.Localidade.Estado.CodigoIBGE,
                        xBairro = preCte.Empresa.Bairro,
                        xCpl = preCte.Empresa.Complemento,
                        xLgr = Utilidades.String.Left(preCte.Empresa.Endereco, 255),
                        xMun = preCte.Empresa.Localidade.Descricao,
                        fone = preCte.Empresa.Telefone
                    },
                    Item = preCte.Empresa.CNPJ,
                    IE = preCte.Empresa.InscricaoEstadual,
                    xFant = Utilidades.String.Left(preCte.Empresa.NomeFantasia, 60),
                    xNome = Utilidades.String.Left(preCte.Empresa.RazaoSocial, 60),
                    CRT = preCte.Empresa.RegimeTributarioCTe == 0 ? MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCRT.Item1 : (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCRT)preCte.Empresa.RegimeTributarioCTe
                };
            }
            else
            {
                tCte.infCte.emit = new MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteEmit()
                {
                    enderEmit = new MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TEndeEmi()
                    {
                        CEP = Utilidades.String.OnlyNumbers(preCte.TransportadorTerceiro.CEP),
                        cMun = preCte.TransportadorTerceiro.Localidade.CodigoIBGE.ToString(),
                        nro = preCte.TransportadorTerceiro.Numero,
                        UF = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TUF_sem_EX)preCte.TransportadorTerceiro.Localidade.Estado.CodigoIBGE,
                        xBairro = preCte.TransportadorTerceiro.Bairro,
                        xCpl = preCte.TransportadorTerceiro.Complemento,
                        xLgr = Utilidades.String.Left(preCte.TransportadorTerceiro.Endereco, 255),
                        xMun = preCte.TransportadorTerceiro.Localidade.Descricao,
                        fone = preCte.TransportadorTerceiro.Telefone1
                    },
                    Item = preCte.TransportadorTerceiro.CPF_CNPJ_SemFormato,
                    IE = preCte.TransportadorTerceiro.IE_RG,
                    xFant = Utilidades.String.Left(preCte.TransportadorTerceiro.NomeFantasia, 60),
                    xNome = Utilidades.String.Left(preCte.TransportadorTerceiro.Nome, 60),
                    CRT = preCte.Empresa.RegimeTributarioCTe == 0 ? MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCRT.Item1 : (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCRT)preCte.Empresa.RegimeTributarioCTe
                };
            }
        }

        private void setarTomador(Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCte, ref MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTe tCte)
        {
            if (preCte.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros)
            {
                tCte.infCte.ide.Item = new MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma4()
                {
                    email = preCte.Tomador.Email,
                    enderToma = retornarEnderecoParticipamento(preCte.Tomador),
                    fone = preCte.Tomador.Telefone1,
                    Item = retornarCPFCNPJParticipamente(preCte.Tomador),
                    ItemElementName = preCte.Tomador.Tipo == Dominio.Enumeradores.TipoPessoa.Fisica ? MultiSoftware.CTe.v400.ConhecimentoDeTransporte.ItemChoiceType.CPF : MultiSoftware.CTe.v400.ConhecimentoDeTransporte.ItemChoiceType.CNPJ,
                    IE = preCte.Tomador.IE_RG,
                    toma = MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma4Toma.Item4,
                    xFant = Utilidades.String.Left(preCte.Tomador.NomeFantasia, 60),
                    xNome = Utilidades.String.Left(preCte.Tomador.Nome, 60)
                };
            }
            else
            {
                tCte.infCte.ide.Item = new MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma3()
                {
                    toma = (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TCTeInfCteIdeToma3Toma)preCte.TipoTomador
                };
            }
        }

        private MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TEndereco retornarEnderecoParticipamento(Dominio.Entidades.ParticipanteCTe participamenteCTE)
        {
            MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TEndereco enderecoParticipante = new MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TEndereco()
            {
                CEP = Utilidades.String.OnlyNumbers(participamenteCTE.CEP),
                cMun = participamenteCTE.Exterior == true ? "9999999" : participamenteCTE.Localidade.CodigoIBGE.ToString(),
                cPais = participamenteCTE.Exterior == true ? participamenteCTE.Pais.Sigla : participamenteCTE.Localidade.Estado.Pais.Sigla,
                xPais = participamenteCTE.Exterior == true ? participamenteCTE.Pais.Nome : participamenteCTE.Localidade.Estado.Pais.Nome,
                nro = participamenteCTE.Numero,
                UF = participamenteCTE.Exterior == true ? MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TUf.EX : (MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TUf)participamenteCTE.Localidade.Estado.CodigoIBGE,
                xBairro = participamenteCTE.Bairro,
                xCpl = participamenteCTE.Complemento,
                xLgr = Utilidades.String.Left(participamenteCTE.Endereco, 255),
                xMun = participamenteCTE.Exterior == true ? "EXTERIOR" : participamenteCTE.Localidade.Descricao
            };
            return enderecoParticipante;
        }

        private string retornarCPFCNPJParticipamente(Dominio.Entidades.ParticipanteCTe participamenteCTE)
        {
            if (participamenteCTE.Exterior)
            {
                if (participamenteCTE.Tipo == Dominio.Enumeradores.TipoPessoa.Fisica)
                    return "00000000000";
                else
                    return "00000000000000";
            }
            else
            {
                return participamenteCTE.CPF_CNPJ_SemFormato;
            }
        }

        private void CalcularImpostos(ref Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCte, Dominio.ObjetosDeValor.CTe.CTe cteIntegracao)
        {
            if (cteIntegracao.ICMS != null)
            {
                preCte.CST = cteIntegracao.ICMS.CST;
                preCte.AliquotaICMS = cteIntegracao.ICMS.Aliquota;
                preCte.PercentualReducaoBaseCalculoICMS = cteIntegracao.ICMS.PercentualReducaoBaseCalculo;
                preCte.BaseCalculoICMS = cteIntegracao.ICMS.BaseCalculo;
                preCte.ValorPresumido = cteIntegracao.ICMS.ValorCreditoPresumido;
                preCte.ValorICMS = cteIntegracao.ICMS.Valor;
                preCte.ValorICMSDevido = cteIntegracao.ICMS.ValorDevido;

                if (preCte.CFOP == null || preCte.CFOP.NaturezaDaOperacao == null)
                    this.SetarCFOPENaturezaPorTabelaDeAliquotas(ref preCte);
            }
            else
            {
                this.CalcularICMSPorTabelaDeAliquotas(ref preCte, preCte.SimplesNacional, null);
            }

            if (cteIntegracao.IBSCBS != null)
            {
                Dominio.ObjetosDeValor.CTe.ImpostoIBSCBS impostoIBSCBS = cteIntegracao.IBSCBS;

                preCte.CSTIBSCBS = impostoIBSCBS.CST;
                preCte.ClassificacaoTributariaIBSCBS = impostoIBSCBS.ClassificacaoTributaria;
                preCte.BaseCalculoIBSCBS = impostoIBSCBS.BaseCalculo;
                preCte.AliquotaIBSEstadual = impostoIBSCBS.AliquotaIBSEstadual;
                preCte.PercentualReducaoIBSEstadual = impostoIBSCBS.PercentualReducaoIBSEstadual;
                preCte.ValorIBSEstadual = impostoIBSCBS.ValorIBSEstadual;
                preCte.AliquotaIBSMunicipal = impostoIBSCBS.AliquotaIBSMunicipal;
                preCte.PercentualReducaoIBSMunicipal = impostoIBSCBS.PercentualReducaoIBSMunicipal;
                preCte.ValorIBSMunicipal = impostoIBSCBS.ValorIBSMunicipal;
                preCte.AliquotaCBS = impostoIBSCBS.AliquotaCBS;
                preCte.PercentualReducaoCBS = impostoIBSCBS.PercentualReducaoCBS;
                preCte.ValorCBS = impostoIBSCBS.ValorCBS;
            }
        }

        private void CalcularICMSPorTabelaDeAliquotas(ref Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCte, Dominio.Enumeradores.OpcaoSimNao simplesNacional, Dominio.Entidades.Aliquota aliquota = null)
        {
            aliquota = (aliquota == null ? this.ObterAliquota(preCte) : aliquota);

            if (aliquota != null)
            {
                preCte.CFOP = aliquota.CFOP;

                decimal valorFreteContratado = 0;
                decimal valorAReceber = 0;
                decimal valorBaseCalculoICMS = 0;

                Repositorio.ComponentePrestacaoPreCTE repComponente = new Repositorio.ComponentePrestacaoPreCTE(_unitOfWork);

                List<Dominio.Entidades.ComponentePrestacaoPreCTE> componentes = repComponente.BuscarPorPreCTe(preCte.Codigo);

                for (var i = 0; i < componentes.Count(); i++)
                {
                    if (componentes[i].Nome == "VALOR FRETE" || componentes[i].Nome == "FRETE VALOR")
                    {
                        valorFreteContratado += componentes[i].Valor;
                        valorBaseCalculoICMS += componentes[i].Valor;
                    }

                    if (componentes[i].IncluiNaBaseDeCalculoDoICMS)
                        valorBaseCalculoICMS += componentes[i].Valor;

                    if (componentes[i].IncluiNoTotalAReceber)
                        valorAReceber += componentes[i].Valor;
                }

                if (simplesNacional == Dominio.Enumeradores.OpcaoSimNao.Nao)
                {
                    preCte.SimplesNacional = Dominio.Enumeradores.OpcaoSimNao.Nao;
                    preCte.CST = aliquota.CST;

                    CalcularInclusaoICMSNoFrete(ref preCte, valorAReceber, valorBaseCalculoICMS, valorFreteContratado, aliquota.Percentual);

                    CalcularICMS(ref preCte);
                }
                else
                {
                    preCte.CST = "";

                    CalcularInclusaoICMSNoFrete(ref preCte, valorAReceber, 0, valorFreteContratado, 0);

                    CalcularICMS(ref preCte);
                }
            }
            else
            {
                throw new Exception("Alíquota para cálculo de impostos não encontrada.");
            }
        }

        private void CalcularICMS(ref Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCte)
        {
            if (preCte.CST != "40" && preCte.CST != "41" && preCte.CST != "51" && preCte.CST != "")
            {
                decimal valor = preCte.BaseCalculoICMS;
                decimal aliquota = preCte.AliquotaICMS;
                decimal valorICMS = valor * (aliquota / 100);

                preCte.ValorICMS = decimal.Round(valorICMS, 2, MidpointRounding.AwayFromZero);
            }
            else
            {
                preCte.PercentualReducaoBaseCalculoICMS = 0;
                preCte.BaseCalculoICMS = 0;
                preCte.ValorICMS = 0;
                preCte.ValorPresumido = 0;
                preCte.AliquotaICMS = 0;
            }
        }

        private void CalcularInclusaoICMSNoFrete(ref Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCte, decimal valorAReceber, decimal valorBaseCalculoICMS, decimal valorFreteContratado, decimal aliquota)
        {
            decimal percentualReducaoBaseCalculoICMS = preCte.PercentualReducaoBaseCalculoICMS;
            valorBaseCalculoICMS -= valorBaseCalculoICMS * (percentualReducaoBaseCalculoICMS / 100);
            decimal percentualAliquota = preCte.CST != "40" && preCte.CST != "41" && preCte.CST != "51" && preCte.CST != "" ? aliquota : 0;
            decimal percentualICMSRecolhido = preCte.IncluirICMSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim ? preCte.PercentualICMSIncluirNoFrete : 0;
            valorBaseCalculoICMS += preCte.IncluirICMSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim ? (percentualAliquota > 0 ? ((valorBaseCalculoICMS / ((100 - percentualAliquota) / 100)) - valorBaseCalculoICMS) : 0) : 0;
            decimal valorICMS = valorBaseCalculoICMS * (percentualAliquota / 100);
            decimal valorRecolhido = valorICMS * (percentualICMSRecolhido / 100);

            preCte.AliquotaICMS = decimal.Round(percentualAliquota, 2, MidpointRounding.AwayFromZero);
            preCte.ValorPrestacaoServico = decimal.Round(valorFreteContratado + valorRecolhido, 2, MidpointRounding.AwayFromZero);

            if (preCte.CST == "60")
                preCte.ValorAReceber = decimal.Round(valorAReceber, 2, MidpointRounding.AwayFromZero);
            else
                preCte.ValorAReceber = decimal.Round(valorAReceber + valorRecolhido, 2, MidpointRounding.AwayFromZero);

            preCte.BaseCalculoICMS = decimal.Round(valorBaseCalculoICMS, 2, MidpointRounding.AwayFromZero);
        }

        private void SalvarInformacoesProdutosPerigosos(Dominio.Entidades.PreConhecimentoDeTransporteEletronico preConhecimento, List<Dominio.ObjetosDeValor.CTe.ProdutoPerigoso> produtos)
        {
            if (produtos != null)
            {
                Repositorio.ProdutoPerigosoPreCTE repProduto = new Repositorio.ProdutoPerigosoPreCTE(_unitOfWork);

                foreach (var prod in produtos)
                {
                    Dominio.Entidades.ProdutoPerigosoPreCTE produto = new Dominio.Entidades.ProdutoPerigosoPreCTE();

                    produto.ClasseRisco = prod.ClasseRisco;
                    produto.PreCTE = preConhecimento;
                    produto.Grupo = prod.GrupoEmbalagem;
                    prod.NomeApropriado = prod.NomeApropriado;
                    prod.NumeroONU = prod.NumeroONU;
                    prod.PontoFulgor = prod.PontoFulgor;
                    prod.QuantidadeTipo = prod.QuantidadeTipo;
                    prod.QuantidadeTotal = prod.QuantidadeTotal;

                    repProduto.Inserir(produto);
                }
            }
        }

        private void SalvarObservacoesFisco(Dominio.Entidades.PreConhecimentoDeTransporteEletronico preConhecimento, List<Dominio.ObjetosDeValor.CTe.Observacao> observacoes)
        {
            if (observacoes != null)
            {
                Repositorio.ObservacaoFiscoPreCTE repObsFisco = new Repositorio.ObservacaoFiscoPreCTE(_unitOfWork);

                foreach (var obs in observacoes)
                {
                    Dominio.Entidades.ObservacaoFiscoPreCTE observacao = new Dominio.Entidades.ObservacaoFiscoPreCTE();
                    observacao.preCTE = preConhecimento;
                    observacao.Descricao = obs.Descricao;
                    observacao.Identificador = obs.Identificador;

                    repObsFisco.Inserir(observacao);
                }
            }
        }

        private void SalvarObservacoesContribuinte(Dominio.Entidades.PreConhecimentoDeTransporteEletronico preConhecimento, List<Dominio.ObjetosDeValor.CTe.Observacao> observacoes)
        {
            if (observacoes != null)
            {
                Repositorio.ObservacaoContribuintePreCTE repObsContribuinte = new Repositorio.ObservacaoContribuintePreCTE(_unitOfWork);

                foreach (var obs in observacoes)
                {
                    Dominio.Entidades.ObservacaoContribuintePreCTE observacao = new Dominio.Entidades.ObservacaoContribuintePreCTE();
                    observacao.PreCTE = preConhecimento;
                    observacao.Descricao = obs.Descricao;
                    observacao.Identificador = obs.Identificador;

                    repObsContribuinte.Inserir(observacao);
                }
            }
        }

        private void SalvarInformacoesDocumentosAnteriores(Dominio.Entidades.PreConhecimentoDeTransporteEletronico preConhecimento, List<Dominio.ObjetosDeValor.CTe.DocumentoTransporteAnterior> documentosAnteriores)
        {
            if (documentosAnteriores != null)
            {
                Repositorio.DocumentoDeTransporteAnteriorPreCTE repDocumento = new Repositorio.DocumentoDeTransporteAnteriorPreCTE(_unitOfWork);
                Repositorio.ModeloDocumentoFiscal repModelo = new Repositorio.ModeloDocumentoFiscal(_unitOfWork);

                foreach (var doc in documentosAnteriores)
                {
                    Dominio.Entidades.DocumentoDeTransporteAnteriorPreCTE documento = new Dominio.Entidades.DocumentoDeTransporteAnteriorPreCTE();

                    documento.Chave = doc.Chave;
                    documento.PreCTE = preConhecimento;

                    DateTime dataEmissao;
                    DateTime.TryParseExact(doc.DataEmissao, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dataEmissao);

                    if (dataEmissao != DateTime.MinValue)
                        documento.DataEmissao = dataEmissao;

                    Servicos.CTe serCTE = new CTe(_unitOfWork);
                    documento.Emissor = serCTE.ObterCliente(preConhecimento.Empresa, doc.Emissor, _unitOfWork);
                    documento.Numero = doc.Numero;
                    documento.Serie = doc.Serie;
                    documento.Tipo = doc.Tipo;

                    repDocumento.Inserir(documento);
                }
            }
        }

        private void SalvarInformacoesDocumentos(Dominio.Entidades.PreConhecimentoDeTransporteEletronico preConhecimento, List<Dominio.ObjetosDeValor.CTe.Documento> documentos)
        {
            if (documentos != null)
            {
                Repositorio.DocumentosPreCTE repDocumento = new Repositorio.DocumentosPreCTE(_unitOfWork);
                Repositorio.ModeloDocumentoFiscal repModelo = new Repositorio.ModeloDocumentoFiscal(_unitOfWork);

                foreach (var doc in documentos)
                {
                    Dominio.Entidades.DocumentosPreCTE documento = new Dominio.Entidades.DocumentosPreCTE();

                    documento.BaseCalculoICMS = doc.BaseCalculoICMS;
                    documento.BaseCalculoICMSST = doc.BaseCalculoICMSST;
                    documento.CFOP = doc.CFOP;
                    documento.ChaveNFE = doc.ChaveNFE;
                    documento.PreCTE = preConhecimento;

                    DateTime dataEmissao;
                    if (!DateTime.TryParseExact(doc.DataEmissao, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dataEmissao))
                        dataEmissao = DateTime.Now;

                    documento.DataEmissao = dataEmissao;
                    documento.Descricao = doc.Descricao;
                    documento.ItemPrincipal = doc.ItemPrincipal;
                    if (doc.ModeloDocumentoFiscal != null)
                        documento.ModeloDocumentoFiscal = repModelo.BuscarPorModelo(doc.ModeloDocumentoFiscal);
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(doc.ChaveNFE))
                            documento.ModeloDocumentoFiscal = repModelo.BuscarPorModelo("55");
                        else
                            documento.ModeloDocumentoFiscal = repModelo.BuscarPorModelo("00");
                    }

                    documento.Numero = doc.Numero;
                    documento.Peso = doc.Peso;
                    documento.PINSuframa = doc.PINSuframa;
                    documento.NCMPredominante = doc.NCMPredominante;
                    documento.Serie = doc.Serie;
                    documento.Valor = doc.Valor;
                    documento.ValorICMS = doc.ValorICMS;
                    documento.ValorICMSST = doc.ValorICMSST;
                    documento.ValorProdutos = doc.ValorProdutos;
                    documento.Volume = doc.Volume;

                    repDocumento.Inserir(documento);
                }
            }
        }

        private void SalvarInformacoesDeSeguroDaCarga(Dominio.Entidades.PreConhecimentoDeTransporteEletronico preConhecimento, List<Dominio.ObjetosDeValor.CTe.Seguro> seguros)
        {
            if (seguros != null)
            {
                Repositorio.SeguroPreCTE repSeguro = new Repositorio.SeguroPreCTE(_unitOfWork);

                foreach (var informacaoSeguro in seguros)
                {
                    Dominio.Entidades.SeguroPreCTE seguro = new Dominio.Entidades.SeguroPreCTE();
                    seguro.PreCTE = preConhecimento;
                    seguro.NomeSeguradora = Utilidades.String.Left(informacaoSeguro.NomeSeguradora, 30);
                    seguro.NumeroApolice = informacaoSeguro.NumeroApolice;
                    seguro.NumeroAverbacao = informacaoSeguro.NumeroAverbacao;
                    seguro.Tipo = informacaoSeguro.Tipo;
                    seguro.Valor = informacaoSeguro.Valor;

                    repSeguro.Inserir(seguro);
                }
            }
        }

        private void SalvarInformacoesDeQuantidadeDaCarga(Dominio.Entidades.PreConhecimentoDeTransporteEletronico preConhecimento, List<Dominio.ObjetosDeValor.CTe.QuantidadeCarga> quantidades)
        {
            if (quantidades != null)
            {
                Repositorio.InformacaoCargaPreCTE repInformacaoCarga = new Repositorio.InformacaoCargaPreCTE(_unitOfWork);

                foreach (var quantidade in quantidades)
                {
                    Dominio.Entidades.InformacaoCargaPreCTE informacaoCarga = new Dominio.Entidades.InformacaoCargaPreCTE();

                    informacaoCarga.PreCTE = preConhecimento;
                    informacaoCarga.Quantidade = quantidade.Quantidade;
                    informacaoCarga.Tipo = quantidade.Descricao;
                    informacaoCarga.UnidadeMedida = quantidade.UnidadeMedida;

                    repInformacaoCarga.Inserir(informacaoCarga);
                }
            }
        }

        private void ObterParticipante(ref Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCte, Dominio.ObjetosDeValor.CTe.Cliente participante, Dominio.Enumeradores.TipoTomador tipo)
        {
            if (participante != null)
            {
                double cpfCnpj = 0;
                double.TryParse(Utilidades.String.OnlyNumbers(participante.CPFCNPJ), out cpfCnpj);

                if (cpfCnpj > 0)
                {
                    Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);

                    Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(cpfCnpj);

                    if (!participante.Exportacao)
                    {
                        Repositorio.Atividade repAtividade = new Repositorio.Atividade(_unitOfWork);
                        Repositorio.Localidade repLocalidade = new Repositorio.Localidade(_unitOfWork);

                        bool inserir = false;

                        if (cliente == null)
                        {
                            inserir = true;
                            cliente = new Dominio.Entidades.Cliente();
                        }
                        Dominio.Entidades.Localidade localidade = null;

                        if (inserir || !participante.NaoAtualizarEndereco)
                        {
                            cliente.CPF_CNPJ = cpfCnpj;
                            Servicos.CTe serCTE = new CTe(_unitOfWork);
                            cliente.Atividade = participante.CodigoAtividade > 0 ? repAtividade.BuscarPorCodigo(participante.CodigoAtividade) : serCTE.ObterAtividade(preCte.Empresa.Codigo, _unitOfWork);
                            cliente.Bairro = Utilidades.String.Left(participante.Bairro, 60);
                            cliente.CEP = participante.CEP;
                            cliente.Complemento = !string.IsNullOrWhiteSpace(participante.Complemento) && participante.Complemento.Length > 2 ? Utilidades.String.Left(participante.Complemento, 60) : null;
                            cliente.Email = participante.Emails;
                            cliente.EmailStatus = participante.StatusEmails ? "A" : "I";
                            cliente.EmailContador = participante.EmailsContador;
                            cliente.EmailContadorStatus = participante.StatusEmailsContador ? "A" : "I";
                            cliente.EmailContato = participante.EmailsContato;
                            cliente.EmailContatoStatus = participante.StatusEmailsContato ? "A" : "I";
                            cliente.Endereco = Utilidades.String.Left(participante.Endereco, 255);
                            cliente.IE_RG = participante.RGIE;
                            cliente.Localidade = repLocalidade.BuscarPorCodigoIBGE(participante.CodigoIBGECidade);
                            cliente.Nome = Utilidades.String.Left(participante.RazaoSocial, 60);
                            cliente.NomeFantasia = Utilidades.String.Left(participante.NomeFantasia, 60);
                            cliente.Numero = Utilidades.String.Left(participante.Numero, 60);
                            cliente.Telefone1 = Utilidades.String.OnlyNumbers(participante.Telefone1);
                            cliente.Telefone2 = Utilidades.String.OnlyNumbers(participante.Telefone2);
                            cliente.Tipo = Utilidades.String.OnlyNumbers(participante.CPFCNPJ).Length == 11 ? "F" : "J";

                            if (inserir)
                            {
                                if (cliente.Tipo == "J" && cliente.GrupoPessoas == null)
                                {
                                    Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(_unitOfWork);
                                    Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = repGrupoPessoas.BuscarPorRaizCNPJ(Utilidades.String.OnlyNumbers(cliente.CPF_CNPJ_Formatado).Remove(8, 6));
                                    if (grupoPessoas != null)
                                    {
                                        cliente.GrupoPessoas = grupoPessoas;
                                    }
                                }
                                cliente.Ativo = true;
                                cliente.DataCadastro = DateTime.Now;
                                cliente.DataUltimaAtualizacao = DateTime.Now;
                                cliente.Integrado = false;
                                repCliente.Inserir(cliente);
                            }
                            else
                            {
                                cliente.DataUltimaAtualizacao = DateTime.Now;
                                cliente.Integrado = false;
                                repCliente.Atualizar(cliente);
                            }
                        }
                        else
                            localidade = repLocalidade.BuscarPorCodigoIBGE(participante.CodigoIBGECidade);

                        preCte.SetarParticipante(cliente, tipo, participante, localidade);
                    }
                    else if (participante.Exportacao)
                    {
                        Repositorio.Pais repPais = new Repositorio.Pais(_unitOfWork);

                        preCte.SetarParticipanteExportacao(participante, cliente, tipo, repPais.BuscarPorSigla(participante.CodigoPais));

                    }
                }
            }
            else
            {
                Dominio.Entidades.ParticipanteCTe part = preCte.ObterParticipante(tipo);

                if (part != null)
                {
                    Repositorio.ParticipanteCTe repParticipante = new Repositorio.ParticipanteCTe(_unitOfWork);

                    preCte.SetarParticipante(null, tipo, participante, null);

                    repParticipante.Deletar(part);
                }
            }
        }

        private void SalvarInformacoesComponentesDaPrestacao(Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCTe, List<Dominio.ObjetosDeValor.CTe.ComponentePrestacao> componentes)
        {
            Repositorio.ComponentePrestacaoPreCTE repositorioComponentePrestacaoPreCTE = new Repositorio.ComponentePrestacaoPreCTE(_unitOfWork);

            if (componentes != null)
            {
                foreach (Dominio.ObjetosDeValor.CTe.ComponentePrestacao componente in componentes)
                {
                    if (componente.Descricao.ToUpper().Contains("VALOR FRETE") || componente.Descricao.ToUpper().Contains("FRETE VALOR"))
                        continue;

                    Dominio.Entidades.ComponentePrestacaoPreCTE componenteDaPrestacaoPreCte = new Dominio.Entidades.ComponentePrestacaoPreCTE();

                    componenteDaPrestacaoPreCte.PreCTE = preCTe;
                    componenteDaPrestacaoPreCte.Nome = componente.Descricao;
                    componenteDaPrestacaoPreCte.Valor = componente.Valor;
                    componenteDaPrestacaoPreCte.IncluiNaBaseDeCalculoDoICMS = componente.IncluiBaseCalculoICMS;
                    componenteDaPrestacaoPreCte.IncluiNoTotalAReceber = componente.IncluiValorAReceber;

                    if (componente.CodigoComponenteFrete > 0)
                        componenteDaPrestacaoPreCte.ComponenteFrete = new Dominio.Entidades.Embarcador.Frete.ComponenteFrete() { Codigo = componente.CodigoComponenteFrete };

                    repositorioComponentePrestacaoPreCTE.Inserir(componenteDaPrestacaoPreCte);
                }
            }

            Dominio.Entidades.ComponentePrestacaoPreCTE componenteDaPrestacaoPreCtePadrao = new Dominio.Entidades.ComponentePrestacaoPreCTE();

            componenteDaPrestacaoPreCtePadrao.PreCTE = preCTe;
            componenteDaPrestacaoPreCtePadrao.Nome = "FRETE VALOR";
            componenteDaPrestacaoPreCtePadrao.Valor = preCTe.ValorFrete;
            componenteDaPrestacaoPreCtePadrao.IncluiNaBaseDeCalculoDoICMS = false;
            componenteDaPrestacaoPreCtePadrao.IncluiNoTotalAReceber = true;

            repositorioComponentePrestacaoPreCTE.Inserir(componenteDaPrestacaoPreCtePadrao);
        }

        private void SalvarInformacoesVeiculos(Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCTe, Dominio.ObjetosDeValor.CTe.CTe cte)
        {
            if (cte.Veiculos != null && cte.Veiculos.Count() > 0)
            {
                Repositorio.VeiculoPreCTE repVeiculoPreCTe = new Repositorio.VeiculoPreCTE(_unitOfWork);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);
                Repositorio.Estado repEstado = new Repositorio.Estado(_unitOfWork);
                Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(_unitOfWork);

                Servicos.CTe serCTE = new CTe(_unitOfWork);
                List<Dominio.Entidades.Veiculo> veiculosCadastrados = new List<Dominio.Entidades.Veiculo>();
                bool situacaoAnterior = false;
                foreach (var veic in cte.Veiculos)
                {
                    Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorPlaca(preCTe.Empresa.Codigo, veic.Placa);

                    if (veiculo == null)
                        veiculo = new Dominio.Entidades.Veiculo();
                    else situacaoAnterior = veiculo.Ativo;

                    veiculo.Empresa = preCTe.Empresa;
                    veiculo.CapacidadeKG = veic.CapacidadeKG > 0 ? veic.CapacidadeKG : veiculo.CapacidadeKG;
                    veiculo.CapacidadeM3 = veic.CapacidadeM3 > 0 ? veic.CapacidadeM3 : veiculo.CapacidadeM3;
                    veiculo.Placa = veic.Placa;
                    veiculo.Renavam = string.IsNullOrWhiteSpace(veic.Renavam) ? veiculo.Renavam : veic.Renavam;
                    veiculo.Tipo = string.IsNullOrWhiteSpace(veic.TipoPropriedade) ? veiculo.Tipo : veic.TipoPropriedade;
                    veiculo.Tara = veic.Tara > 0 ? veic.Tara : veiculo.Tara;
                    veiculo.TipoCarroceria = string.IsNullOrWhiteSpace(veic.TipoCarroceria) ? veiculo.TipoCarroceria : veic.TipoCarroceria;
                    veiculo.TipoRodado = string.IsNullOrWhiteSpace(veic.TipoRodado) ? veiculo.TipoRodado : veic.TipoRodado;
                    veiculo.TipoVeiculo = string.IsNullOrWhiteSpace(veic.TipoVeiculo) ? veiculo.TipoVeiculo : veic.TipoVeiculo;
                    veiculo.Estado = string.IsNullOrWhiteSpace(veic.UF) ? veiculo.Estado : repEstado.BuscarPorSigla(veic.UF);
                    veiculo.Ativo = true;
                    veiculo.Chassi = string.IsNullOrWhiteSpace(veic.Chassi) ? veiculo.Chassi : veic.Chassi;
                    veiculo.Proprietario = serCTE.ObterCliente(preCTe.Empresa, veic.Proprietario, _unitOfWork);
                    veiculo.TipoProprietario = veic.TipoProprietario;
                    veiculo.RNTRC = veic.RNTRCProprietario > 0 ? veic.RNTRCProprietario : veiculo.RNTRC;

                    if (veiculo.Codigo <= 0)
                        repVeiculo.Inserir(veiculo);
                    else
                    {
                        Servicos.Embarcador.Veiculo.VeiculoHistorico.InserirHistoricoVeiculo(veiculo, situacaoAnterior, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MetodosAlteracaoVeiculo.SalvarInformacoesVeiculos_PreCTE, null, _unitOfWork);
                        repVeiculo.Atualizar(veiculo);
                    }

                    veiculosCadastrados.Add(veiculo);

                    Dominio.Entidades.VeiculoPreCTE veiculoPreCTe = new Dominio.Entidades.VeiculoPreCTE();

                    veiculoPreCTe.PreCTE = preCTe;
                    veiculoPreCTe.Veiculo = veiculo;
                    veiculoPreCTe.SetarDadosVeiculo(veiculo);

                    repVeiculoPreCTe.Inserir(veiculoPreCTe);
                }

                if (veiculosCadastrados.Count() > 0 && cte.Motoristas != null && cte.Motoristas.Count() >= 0)
                {
                    Dominio.Entidades.Veiculo veiculo = repVeiculoMotorista.BuscarVeiculoPorCPFMotorista(cte.Motoristas.FirstOrDefault().CPF);

                    if (veiculo != null)
                        this.SalvarInformacoesMotoristas(preCTe, veiculo);
                }
            }
        }

        private void SalvarInformacoesMotoristas(Dominio.Entidades.PreConhecimentoDeTransporteEletronico preConhecimento, List<Dominio.ObjetosDeValor.CTe.Motorista> motoristas)
        {
            if (motoristas != null && motoristas.Count() > 0)
            {
                Repositorio.MotoristaPreCTE repMotoristaCTe = new Repositorio.MotoristaPreCTE(_unitOfWork);

                foreach (var moto in motoristas)
                {
                    Dominio.Entidades.MotoristaPreCTE motorista = new Dominio.Entidades.MotoristaPreCTE();

                    motorista.CPFMotorista = moto.CPF;
                    motorista.PreCTE = preConhecimento;
                    motorista.NomeMotorista = moto.Nome;

                    repMotoristaCTe.Inserir(motorista);
                }
            }
        }

        private void SalvarInformacoesMotoristas(Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCTe, Dominio.Entidades.Veiculo veiculo)
        {
            Dominio.Entidades.Usuario motoristaPrincipal = veiculo.Motoristas.Where(o => o.Principal).Select(o => o.Motorista).FirstOrDefault();

            if (motoristaPrincipal != null)
            {
                Repositorio.MotoristaPreCTE repMotorista = new Repositorio.MotoristaPreCTE(_unitOfWork);

                Dominio.Entidades.MotoristaPreCTE motorista = new Dominio.Entidades.MotoristaPreCTE();

                motorista.PreCTE = preCTe;
                motorista.NomeMotorista = motoristaPrincipal.Nome;
                motorista.CPFMotorista = motoristaPrincipal.CPF;

                repMotorista.Inserir(motorista);
            }
        }

        private void SetarCFOPENaturezaPorTabelaDeAliquotas(ref Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCte)
        {
            Dominio.Entidades.Aliquota aliquota = this.ObterAliquota(preCte);

            if (aliquota != null)
            {
                preCte.CFOP = aliquota.CFOP;
            }
        }

        private Dominio.Entidades.Aliquota ObterAliquota(Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCte)
        {
            Repositorio.Aliquota repAliquota = new Repositorio.Aliquota(_unitOfWork);

            Dominio.Entidades.Aliquota aliquota = repAliquota.BuscarParaCalculoDoICMS(preCte.Empresa.Localidade.Estado.Sigla, preCte.LocalidadeInicioPrestacao.Estado.Sigla, preCte.LocalidadeTerminoPrestacao.Estado.Sigla, preCte?.Remetente?.Atividade?.Codigo ?? 0, preCte.Destinatario?.Atividade.Codigo ?? 7);

            if (aliquota == null)
                aliquota = repAliquota.BuscarParaCalculoDoICMS(preCte.Empresa.Localidade.Estado.Sigla, preCte.LocalidadeInicioPrestacao.Estado.Sigla, preCte.LocalidadeTerminoPrestacao.Estado.Sigla, preCte.Tomador != null ? preCte.Tomador.Atividade.Codigo : 3);

            return aliquota;
        }

        private string SalvarDANFSE(int numero, int serie, int codigoCTe, Dominio.Entidades.Empresa empresa, byte[] pdfDanfse, Repositorio.UnitOfWork unitOfWork)
        {
            string caminhoPDF = Utilidades.IO.FileStorageService.Storage.Combine(ObterConfiguracaoArquivo(unitOfWork).CaminhoRelatorios, "NFSe", empresa.CNPJ, codigoCTe.ToString() + "_" + numero.ToString() + "_" + serie.ToString()) + ".pdf";

            if (string.IsNullOrWhiteSpace(caminhoPDF))
                return "";

            Utilidades.IO.FileStorageService.Storage.WriteAllBytes(caminhoPDF, pdfDanfse);

            return caminhoPDF;
        }

        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoArquivo ObterConfiguracaoArquivo(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo repConfiguracaoArquivo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoArquivo configuracaoArquivo = repConfiguracaoArquivo.BuscarPrimeiroRegistro();

            return configuracaoArquivo;
        }

        #endregion Métodos Privados
    }
}
