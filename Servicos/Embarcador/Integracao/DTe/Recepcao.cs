using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;

namespace Servicos.Embarcador.Integracao.DTe
{
    public static class Recepcao
    {

        private static DTeRecepcao.DTeRecepcaoSoap12Client ObterWSImportClient(string url)
        {
            //url = url.ToLower();

            if (!url.EndsWith("/"))
                url += "/";

            DTeRecepcao.DTeRecepcaoSoap12Client client = null;
            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(url);

            if (url.StartsWith("https"))
            {
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
                binding.SendTimeout = new TimeSpan(0, 5, 0);
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;
                client = new DTeRecepcao.DTeRecepcaoSoap12Client(binding, endpointAddress);
            }
            else
            {
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
                binding.SendTimeout = new TimeSpan(0, 5, 0);
                client = new DTeRecepcao.DTeRecepcaoSoap12Client(binding, endpointAddress);
            }

            return client;
        }

        private static DTeRecepcao.dteCabecMsg ObterCabecalho(Repositorio.UnitOfWork unidadeTrabalho, Dominio.Entidades.Empresa empresa, out string url)
        {
            Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unidadeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao integracao = repIntegracao.Buscar();

            url = "";
            if (integracao != null)
            {
                DTeRecepcao.dteCabecMsg dteCabec = new DTeRecepcao.dteCabecMsg();
                dteCabec.cUF = empresa?.Localidade?.Estado?.CodigoIBGE.ToString() ?? "35";
                dteCabec.versaoDados = "1.00";
                dteCabec.webhook = "https://brb-dte.dsv.zello.services/dte-servico/externo/webhook-dte";
                url = integracao.URLRecepcaoDTe;
                if (string.IsNullOrEmpty(url))
                    url = "https://dte-hml.infraestrutura.gov.br/ws/DTerecepcao/";// "https://dte.infraestrutura.gov.br/ws/DTerecepcao/";

                return dteCabec;
            }
            else
                return null;
        }

        public static void IntegrarRecepcaoLote(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string url = "";
            DTeRecepcao.dteCabecMsg dteCabec = ObterCabecalho(unitOfWork, cargaIntegracao.Carga.Empresa, out url);
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);

            Repositorio.Embarcador.DTe.DocumentoTransporteEletronico repDTe = new Repositorio.Embarcador.DTe.DocumentoTransporteEletronico(unitOfWork);
            Dominio.Entidades.Embarcador.DTe.DocumentoTransporteEletronico dte = repDTe.BuscarPorCarga(cargaIntegracao.Carga.Codigo);

            if (dte == null)
            {
                dte = new Dominio.Entidades.Embarcador.DTe.DocumentoTransporteEletronico();
                Repositorio.EmpresaSerie repSerie = new Repositorio.EmpresaSerie(unitOfWork);
                Dominio.Entidades.EmpresaSerie empresaSerie = repSerie.BuscarPorEmpresaTipo(cargaIntegracao.Carga.Empresa.Codigo, Dominio.Enumeradores.TipoSerie.MDFe);
                dte.TipoAmbiente = cargaIntegracao.Carga.Empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao ? Dominio.Enumeradores.TipoAmbiente.Producao : Dominio.Enumeradores.TipoAmbiente.Homologacao;
                if (empresaSerie != null)
                {
                    dte.Numero = repDTe.BuscarUltimoNumero(cargaIntegracao.Carga.Empresa.Codigo, empresaSerie.Codigo, dte.TipoAmbiente) + 1;
                    dte.Carga = cargaIntegracao.Carga;
                    dte.Empresa = cargaIntegracao.Carga.Empresa;
                    dte.Motivo = "";
                    dte.NumeroRecibo = "";
                    dte.Serie = empresaSerie;
                    dte.Status = "";
                    dte.DataEmissao = DateTime.Now;
                    dte.Chave = GerarChaveDTe(dte);
                    repDTe.Inserir(dte);
                }
                else
                {
                    cargaIntegracao.ProblemaIntegracao = "Não foi configurada uma serie para emissão do DT-e";
                    cargaIntegracao.NumeroTentativas++;
                    cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    return;
                }
            }

            if (dteCabec != null)
            {
                string mensagem = "";
                Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.TEnviDTe objetoDTe = obterDTe(dte, unitOfWork);

                string xml = Servicos.XML.ConvertObjectToXMLString<Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.TEnviDTe>(objetoDTe);
                XmlDocument xmlDTe = new XmlDocument();
                xmlDTe.LoadXml(xml);

         


                if (xmlDTe != null)
                {
                    try
                    {
                        DTeRecepcao.DTeRecepcaoSoap12Client wsRecepcaoDTe = ObterWSImportClient(url);
                        InspectorBehavior inspector = new InspectorBehavior();
                        wsRecepcaoDTe.Endpoint.EndpointBehaviors.Add(inspector);
                        System.Xml.XmlNode retorno = wsRecepcaoDTe.dteRecepcaoLote(ref dteCabec, xmlDTe);

                        Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();

                        XmlDocument doc = new XmlDocument();

                        doc.LoadXml(inspector.LastResponseXML); 
                        
                        //Display all the book titles. XmlNodeList elemList = doc.GetElementsByTagName("title");

                        //Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.dteRecepcaoLoteResult result = Servicos.XML.ConvertXMLStringToObject<Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.dteRecepcaoLoteResult>(doc.GetElementsByTagName("dteRecepcaoLoteResult")[0].InnerXml);

                        cargaIntegracao.NumeroTentativas++;
                        if (doc != null) //validar retorno
                        {
                            
                            //Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.dteRecepcaoLoteResult result = (Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.dteRecepcaoLoteResult)ObjectToXML(retorno.OuterXml, typeof(Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.dteRecepcaoLoteResult));

                            cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                            cargaIntegracao.Protocolo = doc.GetElementsByTagName("protDTe")[0].InnerText; //result.protDTe.ToString();
                            dte.Motivo = "Lote recebido com sucesso";
                            dte.NumeroRecibo = doc.GetElementsByTagName("nRec")[0].InnerText; //result.nRec;
                            long.TryParse(doc.GetElementsByTagName("protDTe")[0].InnerText, out long prot);
                            dte.Protocolo = (int)prot;  //result.protDTe;
                            dte.Status = "132"; //doc.GetElementsByTagName("cStat")[0].InnerText; //result.cStat;

                            cargaIntegracao.ProblemaIntegracao = dte.Status + " - " + dte.Motivo;
                            repDTe.Atualizar(dte);
                        }
                        else
                            cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                        arquivoIntegracao.Mensagem = cargaIntegracao.ProblemaIntegracao;
                        arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", unitOfWork);
                        arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", unitOfWork);
                        arquivoIntegracao.Data = DateTime.Now;

                        arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento;

                        repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                        cargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
                    }
                    catch (Exception ex)
                    {
                        cargaIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao integrar o DT-e.";
                        cargaIntegracao.NumeroTentativas++;
                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                        Servicos.Log.TratarErro(ex);
                    }
                }
                else
                {
                    cargaIntegracao.ProblemaIntegracao = mensagem;
                    cargaIntegracao.NumeroTentativas++;
                    cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                }
            }
            else
            {
                cargaIntegracao.ProblemaIntegracao = "Não foram configurados os dados de integração com o DT-e";
                cargaIntegracao.NumeroTentativas++;
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

            }
            repCargaCargaIntegracao.Atualizar(cargaIntegracao);
        }

        private static string GerarChaveDTe(Dominio.Entidades.Embarcador.DTe.DocumentoTransporteEletronico dte)
        {
            int.TryParse(dte.Numero.ToString() + "9", out int codigoDTe);

            StringBuilder chave = new StringBuilder();
            chave.Append(dte.Empresa.Localidade.Estado.CodigoIBGE);
            chave.Append(dte.DataEmissao.ToString("yyMM"));
            chave.Append(Utilidades.String.OnlyNumbers(dte.Empresa.CNPJ));
            chave.Append("58");
            chave.Append(string.Format("{0:000}", dte.Serie.Numero));
            chave.Append(string.Format("{0:000000000}", dte.Numero));
            chave.Append(dte.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao ? "1" : "2");
            chave.Append(string.Format("{0:00000000}", codigoDTe));
            chave.Append(Utilidades.Calc.Modulo11(chave.ToString()));
            return chave.ToString();
        }

        private static Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.TEnviDTe obterDTe(Dominio.Entidades.Embarcador.DTe.DocumentoTransporteEletronico dte, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaLocaisPrestacao repCargaLocaisPrestacao = new Repositorio.Embarcador.Cargas.CargaLocaisPrestacao(unitOfWork);
            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMotorista repCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaMotorista> cargaMotoristas = repCargaMotorista.BuscarPorCarga(dte.Carga.Codigo);

            Dominio.Entidades.Localidade localidadeOrigem = dte.Carga.LocalidadeColetaLiberada;
            if (localidadeOrigem == null)
                localidadeOrigem = repCargaLocaisPrestacao.BuscarEstadoOrigemPrestacao(dte.Carga.Codigo);

            List<Dominio.Entidades.Localidade> localidadesDestino = repCargaLocaisPrestacao.BuscarEstadosDestinoPrestacao(dte.Carga.Codigo);
            List<Dominio.Entidades.Localidade> terminosPrestacao = (from obj in localidadesDestino select obj).Distinct().ToList();

            Dominio.Entidades.Localidade terminoPrestacao = (from obj in terminosPrestacao select obj).LastOrDefault();


            Dominio.Entidades.Empresa empresa = dte.Carga.Empresa;

            Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.TEnviDTe dTe = new Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.TEnviDTe();
            dTe.idLote = "";
            dTe.DTe = new Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.TDTe();
            dTe.DTe.infDTe = new Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.TDTeInfDTe();
            dTe.DTe.infDTe.ide = new Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.TDTeInfDTeIde();

            dTe.idLote = dte.Codigo.ToString();

            //Enum.TryParse(empresa.Localidade.Estado.CodigoIBGE.ToString(), out Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.TCodUfIBGE codUfIBGE);
            dTe.DTe.infDTe.ide.cUF = Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.TCodUfIBGE.Item42; //codUfIBGE;
            dTe.DTe.infDTe.ide.tpAmb = dte.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao ? Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.TAmb.Item1 : Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.TAmb.Item2;
            dTe.DTe.infDTe.ide.tpEmit = Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.TEmit.Item1;
            dTe.DTe.infDTe.ide.tpTransp = Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.TTransp.Item1;
            dTe.DTe.infDTe.ide.mod = Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.TModMD.Item58;
            dTe.DTe.infDTe.ide.serie = dte.Serie.Numero.ToString();
            dTe.DTe.infDTe.ide.nDT = dte.Numero.ToString();
            dTe.DTe.infDTe.ide.cDV = dte.Chave;
            dTe.DTe.infDTe.ide.cDT = (string.Format("{0:000000000}", dte.Numero));
            dTe.DTe.infDTe.ide.modal = Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.TModalMD.Item1;
            dTe.DTe.infDTe.ide.dhEmi = dte.DataEmissao.ToString("yyyy-MM-ddTHH:mm:sszzz");
            dTe.DTe.infDTe.ide.tpEmis = Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.TDTeInfDTeIdeTpEmis.Item1;
            dTe.DTe.infDTe.ide.procEmi = Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.TDTeInfDTeIdeProcEmi.Item0;
            dTe.DTe.infDTe.ide.verProc = "1.00";
            dTe.DTe.infDTe.ide.UFIni = ObterUF(localidadeOrigem.Estado.Sigla);
            dTe.DTe.infDTe.ide.UFFim = ObterUF(terminoPrestacao.Estado.Sigla);


            List<Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.TDTeInfDTeIdeInfMunCarrega> municipios = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.TDTeInfDTeIdeInfMunCarrega>();
            Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.TDTeInfDTeIdeInfMunCarrega muni = new Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.TDTeInfDTeIdeInfMunCarrega();
            muni.cMunCarrega = localidadeOrigem.CodigoIBGE.ToString();
            muni.xMunCarrega = localidadeOrigem.Descricao;
            municipios.Add(muni);
            dTe.DTe.infDTe.ide.infMunCarrega = municipios.ToArray();

            dTe.DTe.infDTe.emit = new Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.TDTeInfDTeEmit();
            dTe.DTe.infDTe.emit.ItemElementName = Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.ItemChoiceType.CNPJ;
            dTe.DTe.infDTe.emit.Item = empresa.CNPJ_SemFormato;
            dTe.DTe.infDTe.emit.xNome = empresa.RazaoSocial;
            dTe.DTe.infDTe.emit.xFant = empresa.NomeFantasia;
            dTe.DTe.infDTe.emit.enderEmit = new Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.TEndeEmi();
            dTe.DTe.infDTe.emit.enderEmit.xLgr = empresa.Endereco;
            dTe.DTe.infDTe.emit.enderEmit.nro = empresa.Numero;
            dTe.DTe.infDTe.emit.enderEmit.xBairro = empresa.Bairro;
            dTe.DTe.infDTe.emit.enderEmit.cMun = empresa.Localidade.CodigoIBGE.ToString();
            dTe.DTe.infDTe.emit.enderEmit.xMun = empresa.Localidade.Descricao;
            dTe.DTe.infDTe.emit.enderEmit.CEP = empresa.CEP;
            dTe.DTe.infDTe.emit.enderEmit.UF = ObterUF(empresa.Localidade.Estado.Sigla);
            dTe.DTe.infDTe.emit.enderEmit.fone = empresa.Telefone;
            dTe.DTe.infDTe.emit.enderEmit.email = empresa.Email;



            List<Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.TDTeInfDTeInfMunDescarga> tDTeInfDTeInfMunDescargas = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.TDTeInfDTeInfMunDescarga>();
            foreach (Dominio.Entidades.Localidade descarregamento in terminosPrestacao)
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.TDTeInfDTeInfMunDescarga tDTeInfDTeInfMunDescarga = new Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.TDTeInfDTeInfMunDescarga();
                tDTeInfDTeInfMunDescarga.cMunDescarga = descarregamento.CodigoIBGE.ToString();
                tDTeInfDTeInfMunDescarga.xMunDescarga = descarregamento.Descricao;
                tDTeInfDTeInfMunDescargas.Add(tDTeInfDTeInfMunDescarga);
            }

            dTe.DTe.infDTe.infDoc = tDTeInfDTeInfMunDescargas.ToArray();


            dTe.DTe.infDTe.prodPred = new Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.TDTeInfDTeProdPred();
            //01 - Granel sólido; 02 - Granel líquido; 03 - Frigorificada; 04 - Conteinerizada; 05 - Carga Geral; 06 - Neogranel; 07 - Perigosa(granel sólido); 08 - Perigosa(granel líquido); 09 - Perigosa(carga frigorificada); 10 - Perigosa(conteinerizada); 11 - Perigosa(carga geral)
            dTe.DTe.infDTe.prodPred.tpCarga = Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.TDTeInfDTeProdPredTpCarga.Item01;
            dTe.DTe.infDTe.prodPred.xProd = dte.Carga.TipoDeCarga.Descricao;


            List<Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.TDTeInfDTeSeg> tDTeInfDTeSegs = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.TDTeInfDTeSeg>();


            //List<Dominio.ObjetosDeValor.SeguroMDFeIntegracao> segurosMDFe = repMDFe.BuscarSegurosParaMDFe(dte.Carga.Codigo);
            //foreach (Dominio.ObjetosDeValor.SeguroMDFeIntegracao seguro in segurosMDFe)
            //{
            //    Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.TDTeInfDTeSeg tDTeInfDTeSeg = new Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.TDTeInfDTeSeg();
            //    tDTeInfDTeSeg.infResp = new Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.TDTeInfDTeSegInfResp();
            //    tDTeInfDTeSeg.infResp.ItemElementName = Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.ItemChoiceType1.CNPJ;
            //    tDTeInfDTeSeg.infResp.respSeg = Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.TDTeInfDTeSegInfRespRespSeg.Item1;
            //    tDTeInfDTeSeg.infSeg = new Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.TDTeInfDTeSegInfSeg();
            //    tDTeInfDTeSeg.infSeg.CNPJ = seguro.CNPJSeguradora;
            //    tDTeInfDTeSeg.infSeg.xSeg = seguro.NomeSeguradora;
            //    tDTeInfDTeSeg.nApol = seguro.NumeroApolice;
            //    seguro.NumeroAverbacao
            //    List<string> averbacoes = new List<string>();
            //    averbacoes.Add("");
            //    tDTeInfDTeSeg.nAver = averbacoes.ToArray();
            //    tDTeInfDTeSegs.Add(tDTeInfDTeSeg);
            //    dTe.DTe.infDTe.seg = tDTeInfDTeSegs.ToArray();
            //}


            dTe.DTe.infDTe.tot = new Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.TDTeInfDTeTot();
            dTe.DTe.infDTe.tot.cUnid = Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.TDTeInfDTeTotCUnid.Item01;
            dTe.DTe.infDTe.tot.vCarga = dte.Carga.DadosSumarizados.ValorTotalProdutos.ToString("n2");
            dTe.DTe.infDTe.tot.qCarga = dte.Carga.DadosSumarizados.PesoTotal.ToString("n4");

            Dominio.Entidades.Veiculo veiculo = dte.Carga.Veiculo;

            if (veiculo != null)
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.rodo rodo = new Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.rodo();
                rodo.veicTracao = new Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.rodoVeicTracao();
                //rodo.infANTT = new Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.rodoInfANTT();
                rodo.veicTracao.placa = veiculo.Placa;
                rodo.veicTracao.RENAVAM = veiculo.Renavam;
                rodo.veicTracao.tara = veiculo.Tara.ToString();
                rodo.veicTracao.capKG = veiculo.CapacidadeKG.ToString();
                rodo.veicTracao.capM3 = veiculo.CapacidadeM3.ToString();

                List<Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.rodoVeicTracaoCondutor> rodoVeicTracaoCondutors = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.rodoVeicTracaoCondutor>();
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaMotorista cargaMotorista in cargaMotoristas)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.rodoVeicTracaoCondutor condutor = new Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.rodoVeicTracaoCondutor();
                    condutor.CPF = cargaMotorista.Motorista.CPF;
                    condutor.xNome = cargaMotorista.Motorista.Nome;
                    rodoVeicTracaoCondutors.Add(condutor);
                }

                rodo.veicTracao.condutor = rodoVeicTracaoCondutors.ToArray();
                string xml = Servicos.XML.ConvertObjectToXMLString<Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.rodo>(rodo);
                dTe.DTe.infDTe.infModal = new Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.TDTeInfDTeInfModal();
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);
                dTe.DTe.infDTe.infModal.Any = doc.DocumentElement;
            }

            return dTe;
        }

        public static Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.TUf ObterUF(string sigla)
        {
            switch (sigla)
            {
                case "AC":
                    return Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.TUf.AC;
                case "AL":
                    return Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.TUf.AL;
                case "AM":
                    return Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.TUf.AM;
                case "AP":
                    return Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.TUf.AP;
                case "BA":
                    return Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.TUf.BA;
                case "CE":
                    return Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.TUf.CE;
                case "DF":
                    return Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.TUf.DF;
                case "ES":
                    return Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.TUf.ES;
                case "GO":
                    return Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.TUf.GO;
                case "MA":
                    return Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.TUf.MA;
                case "MG":
                    return Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.TUf.MG;
                case "MS":
                    return Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.TUf.MS;
                case "MT":
                    return Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.TUf.MT;
                case "PA":
                    return Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.TUf.PA;
                case "PB":
                    return Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.TUf.PB;
                case "PE":
                    return Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.TUf.PE;
                case "PI":
                    return Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.TUf.PI;
                case "PR":
                    return Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.TUf.PR;
                case "RJ":
                    return Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.TUf.RJ;
                case "RN":
                    return Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.TUf.RN;
                case "RO":
                    return Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.TUf.RO;
                case "RR":
                    return Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.TUf.RR;
                case "RS":
                    return Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.TUf.RS;
                case "SC":
                    return Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.TUf.SC;
                case "SE":
                    return Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.TUf.SE;
                case "SP":
                    return Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.TUf.SP;
                case "TO":
                    return Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.TUf.TO;
                case "EX":
                    return Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.TUf.EX;
                default:
                    return Dominio.ObjetosDeValor.Embarcador.Integracao.DTe.TUf.SC;
            }
        }
    }
}
