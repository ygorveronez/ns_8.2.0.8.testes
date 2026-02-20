using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Integracao.Migrate;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Servicos.Embarcador.Integracao.Migrate
{
    public partial class IntegracaoMigrate
    {

        #region Métodos Públicos

        public bool EmitirNFSe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Repositorio.UnitOfWork unitOfWork)
        {
            Models.Integracao.InspectorBehavior inspector = new Models.Integracao.InspectorBehavior(true);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoAmbiente repConfiguracaoAmbiente = new Repositorio.Embarcador.Configuracoes.ConfiguracaoAmbiente(unitOfWork);

            try
            {
                if (cte.Status == "E" && cte.RPS != null)
                    return this.ConsultarRetornoNFSe(cte, unitOfWork);

                if (cte.Status == "A" || cte.Status == "C")
                    return true;

                if (string.IsNullOrEmpty(cte.Empresa?.Configuracao?.TokenMigrate))
                    throw new ServicoException(@"Processo Abortado: Token não informado na configuração da empresa");

                if (cte.NaturezaNFSe?.IntegracaoMigrateNFSeNatureza?.NumeroNatureza == null)
                    throw new ServicoException(@"Processo Abortado: Natureza da NFS-e não configurada para integração com a Migrate.");

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAmbiente configuracaoAmbiente = repConfiguracaoAmbiente.BuscarPrimeiroRegistro();

                if (string.IsNullOrEmpty(configuracaoAmbiente.ChaveParceiroMigrate))
                    throw new ServicoException(@"Processo Abortado: Chave do Parceiro não configurado.");

                Repositorio.NFSeItem repNFSeItem = new Repositorio.NFSeItem(unitOfWork);
                Repositorio.Embarcador.CTe.CTeParcela repParcelaNFSe = new Repositorio.Embarcador.CTe.CTeParcela(unitOfWork);

                List<Dominio.Entidades.NFSeItem> itens = repNFSeItem.BuscarPorCTe(cte.Codigo);
                List<Dominio.Entidades.Embarcador.CTe.CTeParcela> parcelas = repParcelaNFSe.BuscarPorNFSe(cte.Codigo);

                Servicos.NFSe serNFSe = new Servicos.NFSe();

                // Validar RPS
                ServicoNFSe.RPSNFSe rps = serNFSe.ObterRPS(cte, unitOfWork);

                // Obter Request
                Servicos.ServicosMigrate.InvoiCy envio = this.obterEmitirNFSe(cte, itens, parcelas, rps, configuracaoAmbiente.ChaveParceiroMigrate);

                // Transmitir
                string urlWebService = this.ObterUrlWebService(cte.TipoAmbiente);
                ServicosMigrate.recepcaoSoapPortClient servicoMigrate = ObterClientIntegrarNFSe(urlWebService);
                servicoMigrate.Endpoint.EndpointBehaviors.Add(inspector);
                Servicos.ServicosMigrate.ExecuteResponse retEnvio = servicoMigrate.ExecuteAsync(envio).Result;

                string xmlEnvioMigrate = inspector.LastRequestXML;
                string xmlRetornoEnvioMigrate = inspector.LastResponseXML;

                // Salvar XML Envio Migrate
                this.SalvarArquivoXML(cte, false, null, xmlEnvioMigrate, Dominio.Enumeradores.TipoXMLCTe.EnvioIntegracao, cte.Status, unitOfWork);
                this.SalvarArquivoXML(cte, false, null, xmlRetornoEnvioMigrate, Dominio.Enumeradores.TipoXMLCTe.RetornoIntegracao, cte.Status, unitOfWork);

                // Processar Response
                this.processarRetornoEmitirNFSe(cte, retEnvio, unitOfWork);
            }
            catch (ServicoException excecao)
            {
                cte.Status = "R";

                String message = excecao.Message;
                if (message.Length > 300)
                {
                    message = message.Substring(0, 300);
                }
                cte.MensagemRetornoSefaz = message;
            }
            catch (Exception excecao)
            {
                cte.Status = "R";
                cte.MensagemRetornoSefaz = "Processo Abortado: Ocorreu um erro ao integrar NFSe com a migrate";
                Servicos.Log.TratarErro(excecao);
            }

            repCTe.Atualizar(cte);
            return true;
        }

        #endregion

        #region Métodos Privados

        private Servicos.ServicosMigrate.InvoiCy obterEmitirNFSe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, List<Dominio.Entidades.NFSeItem> itens, List<Dominio.Entidades.Embarcador.CTe.CTeParcela> parcelas, ServicoNFSe.RPSNFSe rps, string chaveParceiro)
        {
            // Obter XML Envio NFS-e
            string xmlEnvioNFSe = this.obterXMLEnvioNFSe(cte, rps, itens, parcelas);

            Servicos.ServicosMigrate.InvoiCy.DadosType listaDocumentos = new ServicosMigrate.InvoiCy.DadosType();

            //Cria um objeto para guardar os dados do cabeçalho da conexão
            //Chama a função que gera a CK, e depois adiciona a mesma no cabeçalho.
            Servicos.ServicosMigrate.InvoiCyRecepcaoCabecalho cabecalho = new Servicos.ServicosMigrate.InvoiCyRecepcaoCabecalho()
            {
                EmpPK = chaveParceiro,
                EmpCK = this.GeraHashMD5(xmlEnvioNFSe, cte.Empresa.Configuracao.TokenMigrate),
                EmpCO = ""
            };

            //Armazena os dados da requisição.
            Servicos.ServicosMigrate.InvoiCyRecepcaoDadosItem documento = new Servicos.ServicosMigrate.InvoiCyRecepcaoDadosItem();
            documento.Documento = xmlEnvioNFSe;
            documento.Parametros = "";
            listaDocumentos.Add(documento);

            //Adiciona as informações da requisição
            Servicos.ServicosMigrate.InvoiCyRecepcaoInformacoes Info = new Servicos.ServicosMigrate.InvoiCyRecepcaoInformacoes()
            {
                Texto = ""
            };

            //Adiciona os dados na recepção
            Servicos.ServicosMigrate.InvoiCy retorno = new Servicos.ServicosMigrate.InvoiCy()
            {
                Cabecalho = cabecalho,
                Informacoes = Info,
                Dados = listaDocumentos
            };

            return retorno;
        }

        private string obterXMLEnvioNFSe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, ServicoNFSe.RPSNFSe rps, List<Dominio.Entidades.NFSeItem> itens, List<Dominio.Entidades.Embarcador.CTe.CTeParcela> parcelas)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Migrate.envioNFSe envioNFSe = new Dominio.ObjetosDeValor.Embarcador.Integracao.Migrate.envioNFSe();
            envioNFSe.ModeloDocumento = "NFSE";
            envioNFSe.Versao = "1.00";
            envioNFSe.RPS = this.obterXMLEnvioNFSeRPS(cte, rps, itens, parcelas);
            return Serialize((object)envioNFSe).Replace("<?xml version=\"1.0\" encoding=\"utf-16\"?>", "");
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Migrate.RPS obterXMLEnvioNFSeRPS(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, ServicoNFSe.RPSNFSe rps, List<Dominio.Entidades.NFSeItem> itens, List<Dominio.Entidades.Embarcador.CTe.CTeParcela> parcelas)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Migrate.RPS retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Migrate.RPS();
            retorno.RPSNumero = rps.Numero;
            retorno.RPSSerie = rps.Serie;
            retorno.RPSTipo = 1;
            retorno.dEmis = cte.DataEmissao.HasValue ? cte.DataEmissao.Value.ToString("yyyy-MM-dd HH:mm:00").Replace(" ", "T") : DateTime.Now.ToString("yyyy-MM-dd HH:mm:00").Replace(" ", "T");
            retorno.dCompetencia = cte.DataEmissao.HasValue ? cte.DataEmissao.Value.ToString("yyyy-MM-dd HH:mm:00").Replace(" ", "T") : DateTime.Now.ToString("yyyy-MM-dd HH:mm:00").Replace(" ", "T");
            retorno.LocalPrestServ = null; //TODO:
            retorno.natOp = cte.NaturezaNFSe.IntegracaoMigrateNFSeNatureza.NumeroNatureza;
            retorno.RegEspTrib = cte.Empresa.Configuracao?.IntegracaoMigrateRegimeTributario?.Numero;
            retorno.OptSN = cte.Empresa.OptanteSimplesNacional ? 1 : 2;
            retorno.IncCult = 2;
            retorno.Status = 1;
            retorno.tpAmb = cte.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao ? 1 : 2;
            retorno.RPSSubs = this.obterXMLEnvioNFSeRPSSubs(cte);
            retorno.NFSOutrasinformacoes = this.ObterNFSOutrasInformacoes(cte, itens, parcelas);
            retorno.Prestador = this.obterXMLEnvioNFSePrestador(cte.Empresa);
            retorno.ListaItens = this.obterXMLEnvioNFSeListaItens(cte, itens);
            retorno.Servico = this.obterXMLEnvioNFSeServico(cte, itens, parcelas);
            retorno.Tomador = this.obterXMLEnvioNFSeTomador(cte.Tomador);
            retorno.IntermServico = this.obterXMLEnvioNFSeIntermServico(cte.Intermediario);
            return retorno;
        }

        private RPSSubs obterXMLEnvioNFSeRPSSubs(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            RPSSubs retorno = null;

            if (cte.NumeroSubstituicao != null)
            {
                retorno = new RPSSubs();
                retorno.SubsNumero = (int)cte.NumeroSubstituicao;
                retorno.SubsSerie = cte.SerieSubstituicao;
            }

            return retorno;
        }

        private string ObterNFSOutrasInformacoes(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, List<Dominio.Entidades.NFSeItem> itens, List<Dominio.Entidades.Embarcador.CTe.CTeParcela> parcelas)
        {
            string retorno = null;

            string duplicatas = "";
            if (parcelas != null && parcelas.Count() > 0)
            {
                for (int i = 0; i < parcelas.Count(); i++)
                {
                    Dominio.Entidades.Embarcador.CTe.CTeParcela parcela = parcelas[i];
                    if (string.IsNullOrWhiteSpace(duplicatas))
                        duplicatas = " - Duplicatas: " + parcela.Sequencia + " - " + parcela.Valor.ToString("n2") + "-" + parcela.DataVencimento.Value.ToString("dd/MM/yyyy") + ";";
                    else
                        duplicatas = duplicatas + " " + parcela.Sequencia + " - " + parcela.Valor.ToString("n2") + "-" + parcela.DataVencimento.Value.ToString("dd/MM/yyyy") + ";";
                }
            }

            string discriminacaoItem = itens?.Count > 0 ? itens.FirstOrDefault().Discriminacao : string.Empty;

            retorno = (cte.ObservacoesGerais + duplicatas + discriminacaoItem).Trim();

            return TratarTexto(retorno);
        }

        private string TratarTexto(string texto)
        {
            // Remover os caracteres de nova linha
            string retorno = string.Empty;
            if (!string.IsNullOrEmpty(texto))
                retorno = texto.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ");
            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Migrate.Prestador obterXMLEnvioNFSePrestador(Dominio.Entidades.Empresa empresa)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Migrate.Prestador retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Migrate.Prestador();
            retorno.CNPJ_prest = empresa.CNPJ;
            retorno.xNome = empresa.RazaoSocial;
            retorno.xFant = empresa.NomeFantasia;
            retorno.IM = empresa.InscricaoMunicipal;
            retorno.IE = empresa.InscricaoEstadual;
            retorno.CMC = null;
            retorno.CAEPF = null;
            retorno.enderPrest = this.obterXMLEnvioNFSePrestadorEndereco(empresa);
            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Migrate.enderPrest obterXMLEnvioNFSePrestadorEndereco(Dominio.Entidades.Empresa empresa)
        {
            enderPrest retorno = new enderPrest();
            retorno.TPEnd = null;
            retorno.XLgr = empresa.Endereco;
            retorno.nro = empresa.Numero;
            retorno.xBairro = empresa.Bairro;
            retorno.cMun = string.Format("{0:0000000}", empresa.Localidade.CodigoIBGE);
            retorno.UF = empresa.Localidade?.Estado.Sigla;
            retorno.CEP = empresa.CEP;
            retorno.fone = Utilidades.String.OnlyNumbers(empresa.Telefone);
            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Migrate.ListaItens obterXMLEnvioNFSeListaItens(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, List<Dominio.Entidades.NFSeItem> itens)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Migrate.ListaItens retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Migrate.ListaItens();
            retorno.Item = this.obterXMLEnvioNFSeListaItensItem(cte, itens);
            return retorno;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Migrate.Item> obterXMLEnvioNFSeListaItensItem(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, List<Dominio.Entidades.NFSeItem> itens)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Migrate.Item> retorno = null;
            int itemSequencia = 0;
            string duasdecimais = "F2";
            string quatrodecimais = "F4";
            var culture = CultureInfo.InvariantCulture;
            foreach (Dominio.Entidades.NFSeItem item in itens.OrderBy(o => o.Codigo))
            {
                if (retorno == null)
                    retorno = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Migrate.Item>();

                itemSequencia++;
                Dominio.ObjetosDeValor.Embarcador.Integracao.Migrate.Item itemMigrate = new Dominio.ObjetosDeValor.Embarcador.Integracao.Migrate.Item();
                itemMigrate.ItemSeq = itemSequencia;
                itemMigrate.ItemCod = item.Servico.Codigo.ToString();
                itemMigrate.ItemDesc = item.Servico.Descricao.Length > 80 ? item.Servico.Descricao.Substring(0, 80) : item.Servico.Descricao;
                itemMigrate.ItemQtde = item.Quantidade.ToString(duasdecimais, culture);
                itemMigrate.ItemvlDed = item.ValorDeducoes.ToString(duasdecimais, culture);
                itemMigrate.ItemvUnit = item.ValorServico.ToString(duasdecimais, culture);
                itemMigrate.ItemuMed = null;
                itemMigrate.ItemTributavel = null;
                itemMigrate.ItemcCnae = !string.IsNullOrEmpty(item.Servico.CNAE) ? item.Servico.CNAE : null;
                itemMigrate.ItemTributMunicipio = item.Servico.CodigoTributacao;
                itemMigrate.ItemIteListServico = obterIteListServico(item.Servico?.Numero);
                itemMigrate.ItemcMunIncidencia = string.Format("{0:0000000}", cte.LocalidadeTerminoPrestacao.CodigoIBGE);
                itemMigrate.ItemExigibilidadeISS = cte.NaturezaNFSe.IntegracaoMigrateNFSeNatureza.NumeroNatureza.ToString();
                itemMigrate.ItemnAlvara = null;
                itemMigrate.ItemvDesconto = null;

                itemMigrate.ItemBaseCalculo = item.BaseCalculoISS.ToString(duasdecimais, culture);

                itemMigrate.ItemAliquota = item.AliquotaISS.ToString(quatrodecimais, culture);
                itemMigrate.ItemvIss = item.ValorISS.ToString(duasdecimais, culture);

                itemMigrate.ItemIssRetido = cte.ISSRetido ? 1 : 2;

                if (itens.Count() == 1)
                {
                    decimal valorServicos = 0;
                    if (cte.ISSRetido)
                        valorServicos = cte.BaseCalculoISS;
                    else
                        valorServicos = cte.ValorAReceber;

                    decimal valorLiquido = valorServicos - cte.ValorPIS - cte.ValorCOFINS - cte.ValorINSS - cte.ValorIR - cte.ValorCSLL - cte.ValorOutrasRetencoes - cte.ValorISSRetido - cte.ValorDescontoIncondicionado - cte.ValorDescontoIncondicionado;

                    itemMigrate.ItemVlrTotal = valorServicos.ToString(duasdecimais, culture);
                    itemMigrate.ItemVlrLiquido = valorLiquido.ToString(duasdecimais, culture);

                    itemMigrate.ItemValAliqISSRetido = cte.PercentualISSRetido.ToString(quatrodecimais, culture);
                    itemMigrate.ItemBCRetido = cte.ValorISS.ToString(duasdecimais, culture);
                    itemMigrate.ItemvlrISSRetido = cte.ValorISSRetido.ToString(duasdecimais, culture);

                    itemMigrate.ItemValAliqINSS = cte.AliquotaINSS.ToString(quatrodecimais, culture);
                    itemMigrate.ItemValINSS = cte.ValorINSS.ToString(duasdecimais, culture);

                    itemMigrate.ItemValAliqIR = cte.AliquotaIR.ToString(quatrodecimais, culture);
                    itemMigrate.ItemValIR = cte.ValorIR.ToString(duasdecimais, culture);

                    itemMigrate.ItemValAliqCOFINS = cte.AliquotaCOFINS.ToString(quatrodecimais, culture);
                    itemMigrate.ItemValCOFINS = cte.ValorCOFINS.ToString(duasdecimais, culture);

                    itemMigrate.ItemValAliqCSLL = cte.AliquotaCSLL.ToString(quatrodecimais, culture);
                    itemMigrate.ItemValCSLL = cte.ValorCSLL.ToString(duasdecimais, culture);

                    itemMigrate.ItemValAliqPIS = cte.AliquotaPIS.ToString(quatrodecimais, culture);
                    itemMigrate.ItemValPIS = cte.ValorPIS.ToString(duasdecimais, culture);

                    itemMigrate.ItemvOutrasRetencoes = cte.ValorOutrasRetencoes.ToString(duasdecimais, culture);
                }
                else
                {
                    itemMigrate.ItemVlrTotal = item.ValorTotal.ToString(duasdecimais, culture);
                    itemMigrate.ItemVlrLiquido = null;
                }

                itemMigrate.ItemDescIncondicionado = item.ValorDescontoIncondicionado.ToString(duasdecimais, culture);
                itemMigrate.ItemDescCondicionado = item.ValorDescontoCondicionado.ToString(duasdecimais, culture);

                itemMigrate.ItemRespRetencao = null;
                itemMigrate.ItemNumProcesso = null;
                itemMigrate.ItemDedTipo = null;
                itemMigrate.ItemDedCPFRef = null;
                itemMigrate.ItemDedCNPJRef = null;
                itemMigrate.ItemDedNFRef = null;
                itemMigrate.ItemDedvlTotRef = null;
                itemMigrate.ItemDedPer = null;
                itemMigrate.ItemRedBC = null;
                itemMigrate.ItemRedBCRetido = null;
                itemMigrate.ItemPaisImpDevido = null;
                itemMigrate.ItemJustDed = null;

                itemMigrate.ItemTotalAproxTribServ = null;

                retorno.Add(itemMigrate);
            }

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Migrate.Servico obterXMLEnvioNFSeServico(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, List<Dominio.Entidades.NFSeItem> itens, List<Dominio.Entidades.Embarcador.CTe.CTeParcela> parcelas)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Migrate.Servico retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Migrate.Servico();

            var item = itens.FirstOrDefault();

            retorno.IteListServico = this.obterIteListServico(item.Servico?.Numero);
            retorno.Cnae = !string.IsNullOrEmpty(item.Servico.CNAE) ? item.Servico.CNAE : null;
            retorno.fPagamento = null;
            retorno.TributMunicipio = item.Servico.CodigoTributacao;

            if (cte.Empresa.Configuracao.EnviarObservacaoNaDiscriminacaoServicoMigrate)
                retorno.Discriminacao = this.ObterNFSOutrasInformacoes(cte, itens, parcelas);
            else
                retorno.Discriminacao = TratarTexto(!string.IsNullOrWhiteSpace(item.Discriminacao) ? item.Discriminacao.Replace("&", " ") : string.Empty);

            retorno.cMun = string.Format("{0:0000000}", cte.LocalidadeTerminoPrestacao.CodigoIBGE);
            retorno.cMunIncidencia = string.Format("{0:0000000}", cte.LocalidadeTerminoPrestacao.CodigoIBGE);

            retorno.Valores = new Valores();
            string duasdecimais = "F2";
            string quatrodecimais = "F4";
            var culture = CultureInfo.InvariantCulture;

            if (cte.ISSRetido)
                retorno.Valores.RespRetencao = 1;

            decimal valorLiquido = cte.ValorPrestacaoServico - cte.ValorPIS - cte.ValorCOFINS - cte.ValorINSS - cte.ValorIR - cte.ValorCSLL - cte.ValorOutrasRetencoes - cte.ValorISSRetido - cte.ValorDescontoIncondicionado - cte.ValorDescontoIncondicionado;

            retorno.Valores.ValBaseCalculo = cte.BaseCalculoISS.ToString(duasdecimais, culture);
            retorno.Valores.ValServicos = cte.ValorPrestacaoServico.ToString(duasdecimais, culture);
            retorno.Valores.ValLiquido = valorLiquido.ToString(duasdecimais, culture);

            retorno.Valores.ValAliqTotTributos = null;
            retorno.Valores.ValTotal = null;
            retorno.Valores.ValTotalRecebido = null;

            retorno.Valores.ValPercDeducoes = null;
            retorno.Valores.ValDeducoes = cte.ValorDeducoes.ToString(duasdecimais, culture);

            retorno.Valores.ValDescIncond = cte.ValorDescontoIncondicionado.ToString(duasdecimais, culture);
            retorno.Valores.ValDescCond = cte.ValorDescontoCondicionado.ToString(duasdecimais, culture);

            retorno.Valores.ValAliqPIS = cte.AliquotaPIS.ToString(quatrodecimais, culture);
            retorno.Valores.ValPIS = cte.ValorPIS.ToString(duasdecimais, culture);
            retorno.Valores.ValBCPIS = cte.BasePIS.ToString(duasdecimais, culture);
            retorno.Valores.PISRetido = null;

            retorno.Valores.ValAliqCOFINS = cte.AliquotaCOFINS.ToString(quatrodecimais, culture);
            retorno.Valores.ValCOFINS = cte.ValorCOFINS.ToString(duasdecimais, culture);
            retorno.Valores.ValBCCOFINS = cte.BaseCOFINS.ToString(duasdecimais, culture);
            retorno.Valores.COFINSRetido = null;

            retorno.Valores.ValAliqINSS = cte.AliquotaINSS.ToString(quatrodecimais, culture);
            retorno.Valores.ValINSS = cte.ValorINSS.ToString(duasdecimais, culture);
            retorno.Valores.ValBCINSS = cte.ValorBaseCalculoINSS.ToString(duasdecimais, culture);
            retorno.Valores.INSSRetido = null;

            retorno.Valores.ValAliqIR = cte.AliquotaIR.ToString(quatrodecimais, culture);
            retorno.Valores.ValIR = cte.ValorIR.ToString(duasdecimais, culture);
            retorno.Valores.ValBCIRRF = cte.ValorBaseCalculoIR.ToString(duasdecimais, culture);
            retorno.Valores.IRRetido = null;

            retorno.Valores.ValAliqCSLL = cte.AliquotaCSLL.ToString(quatrodecimais, culture);
            retorno.Valores.ValCSLL = cte.ValorCSLL.ToString(duasdecimais, culture);
            retorno.Valores.ValBCCSLL = cte.ValorBaseCalculoCSLL.ToString(duasdecimais, culture);
            retorno.Valores.CSLLRetido = null;

            if (!cte.NaoEnviarAliquotaEValorISS)
            {
                retorno.Valores.ValAliqISS = cte.AliquotaISS.ToString(quatrodecimais, culture);
                retorno.Valores.ValISS = cte.ValorISS.ToString(duasdecimais, culture);
            }

            retorno.Valores.ISSRetido = cte.ISSRetido ? 1 : 2;
            retorno.Valores.ValISSRetido = cte.ValorISSRetido.ToString(duasdecimais, culture);

            retorno.Valores.ValAliqOutrasRetencoes = null;
            retorno.Valores.ValOutrasRetencoes = cte.ValorOutrasRetencoes.ToString(duasdecimais, culture);
            retorno.Valores.ValBCOutrasRetencoes = null;

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Migrate.Tomador obterXMLEnvioNFSeTomador(Dominio.Entidades.ParticipanteCTe tomador)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Migrate.Tomador retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Migrate.Tomador();
            if (tomador.Tipo == Dominio.Enumeradores.TipoPessoa.Juridica)
                retorno.TomaCNPJ = tomador.CPF_CNPJ;
            else
                retorno.TomaCPF = tomador.CPF_CNPJ;

            retorno.TomaRazaoSocial = tomador.Nome;
            retorno.TomatpLgr = null;
            retorno.TomaEndereco = tomador.Endereco;
            retorno.TomaNumero = tomador.Numero;
            retorno.TomaBairro = tomador.Bairro;
            retorno.TomaComplemento = tomador.Complemento;
            retorno.TomacMun = tomador.Exterior ? "9999999" : string.Format("{0:0000000}", tomador.Localidade.CodigoIBGE);
            retorno.TomaxMun = Utilidades.String.Left(tomador.Exterior ? tomador.Cidade : tomador.Localidade.Descricao, 60);
            retorno.TomaUF = tomador.Localidade?.Estado.Sigla ?? "EX";
            retorno.TomaPais = Utilidades.String.Left(tomador.Exterior ? (tomador.Pais != null ? tomador.Pais.Nome : tomador.Localidade != null ? tomador.Localidade.Estado.Pais.Nome : tomador.Cliente.Localidade.Estado.Pais.Nome) : tomador.Localidade.Estado.Pais.Nome, 60);
            retorno.TomaCEP = tomador.CEP;
            retorno.TomaTipoTelefone = null;
            retorno.TomaTelefone = Utilidades.String.OnlyNumbers(tomador.Telefone1);
            retorno.TomaEmail = tomador.EmailStatus ? tomador.Email : string.Empty;
            retorno.TomaIE = tomador.IE_RG;
            retorno.TomaIM = tomador.InscricaoMunicipal;
            retorno.TomaIME = null;
            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Migrate.IntermServico obterXMLEnvioNFSeIntermServico(Dominio.Entidades.ParticipanteCTe intermediario)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Migrate.IntermServico retorno = null;

            if (intermediario != null)
            {
                retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Migrate.IntermServico();

                if (intermediario.Tipo == Dominio.Enumeradores.TipoPessoa.Juridica)
                    retorno.IntermCNPJ = intermediario.CPF_CNPJ;
                else
                    retorno.IntermCPF = intermediario.CPF_CNPJ;

                retorno.IntermRazaoSocial = intermediario.Nome;
                retorno.IntermEndereco = intermediario.Endereco;
                retorno.IntermNumero = intermediario.Numero;
                retorno.IntermBairro = intermediario.Bairro;
                retorno.IntermComplemento = intermediario.Complemento;
                retorno.IntermCmun = intermediario.Exterior ? "9999999" : string.Format("{0:0000000}", intermediario.Localidade.CodigoIBGE);
                retorno.IntermXmun = Utilidades.String.Left(intermediario.Exterior ? intermediario.Cidade : intermediario.Localidade.Descricao, 60);
                retorno.IntermCep = intermediario.CEP;
                retorno.IntermFone = Utilidades.String.OnlyNumbers(intermediario.Telefone1);
                retorno.IntermEmail = intermediario.EmailStatus ? intermediario.Email : string.Empty;
                retorno.ItermIE = intermediario.IE_RG;
                retorno.IntermIM = intermediario.InscricaoMunicipal;
            }

            return retorno;
        }

        private string obterIteListServico(string servicoNumero)
        {
            if (!string.IsNullOrEmpty(servicoNumero) && servicoNumero.Length == 4 && !servicoNumero.Contains("."))
                return $"{servicoNumero.Substring(0, 2)}.{servicoNumero.Substring(2, 2)}";
            else
                return servicoNumero;
        }

        private void processarRetornoEmitirNFSe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Servicos.ServicosMigrate.ExecuteResponse retEnvio, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.RPSNFSe repRPSNFSe = new Repositorio.RPSNFSe(unitOfWork);

            foreach (var msgitem in retEnvio.Invoicyretorno.Mensagem)
            {
                if (msgitem.Codigo == 100 || msgitem.Codigo == 313) // 100 - Consulta efetuada com sucesso; 313 - já efetivado no sistema
                {
                    //Percorre documento atualizando situação
                    foreach (var documentos in msgitem.Documentos)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Integracao.Migrate.RetornoNFSe retWS = new Dominio.ObjetosDeValor.Embarcador.Integracao.Migrate.RetornoNFSe();
                        retWS = (Dominio.ObjetosDeValor.Embarcador.Integracao.Migrate.RetornoNFSe)DeSerialize<Dominio.ObjetosDeValor.Embarcador.Integracao.Migrate.RetornoNFSe>("<RetornoMigrate>" + documentos.Documento + "</RetornoMigrate>");

                        foreach (var documento in retWS.Documento)
                        {
                            if (cte.RPS.Numero == Convert.ToInt32(documento.DocNumero))
                            {
                                if (documento.Situacao.SitCodigo == "999")
                                {
                                    throw new ServicoException($"Processo abortado: {documento.Situacao.SitCodigo + "-" + documento.Situacao.SitDescricao}");
                                }
                                else if (documento.Situacao.SitCodigo == "104" || documento.Situacao.SitCodigo == "105" || documento.Situacao.SitCodigo == "111") // Pendente
                                {
                                    cte.Status = "E";
                                    cte.MensagemRetornoSefaz = "NFS-e em processamento.";
                                    cte.DataIntegracao = DateTime.Now;

                                    cte.RPS.MensagemRetorno = "NFS-e em processamento.";
                                    cte.RPS.Status = "E";
                                    repCTe.Atualizar(cte);
                                    repRPSNFSe.Atualizar(cte.RPS);

                                    //Situacao 111: Retorno da migrate quando o RPS já foi enviado, deixar a trhead consultar o documento...
                                }
                                else if (documento.Situacao.SitCodigo == "100") // Autorizado
                                {
                                    if (!string.IsNullOrEmpty(documento.NFSe.NFSeNumero))
                                    {
                                        autorizarNFSe(cte, documento, unitOfWork);
                                    }
                                    else
                                    {
                                        throw new ServicoException($"Processo abortado: Solicitação de Emissão Enviada com Sucesso. Porém ao receber o retorno não foi possível identificar o número da NFSe.");
                                    }

                                }
                                else
                                {
                                    throw new ServicoException($"Processo abortado: {documento.Situacao.SitCodigo + "-" + documento.Situacao.SitDescricao}");
                                }
                            }
                        }
                    }
                }
                else
                {
                    throw new ServicoException($"Processo abortado: {msgitem.Codigo.ToString() + " - " + msgitem.Descricao}");
                }
            }
        }

        private void autorizarNFSe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Documento documento, Repositorio.UnitOfWork unitOfWork)
        {
            autorizarNFSe(cte, documento.NFSe?.NFSeNumero, documento.DocProtocolo, documento.DocSitCodigo, documento.DocSitDescricao, documento.DocXML, documento.DocPDF, unitOfWork);
        }

        private void autorizarNFSe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, DocumentoEnvio documento, Repositorio.UnitOfWork unitOfWork)
        {
            autorizarNFSe(cte, documento.NFSe?.NFSeNumero, documento.DocProtocolo, documento.Situacao.SitCodigo, documento.Situacao.SitDescricao, documento.DocXMLBase64, documento.DocPDFBase64, unitOfWork);
        }

        private void autorizarNFSe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, string nfseNumero, string docProtocolo, string sitCodigo, string sitDescricao, string docXMLBase64, string docPDFBase64, Repositorio.UnitOfWork unitOfWork)
        {
            unitOfWork.Start();

            try
            {
                Servicos.CTe serCTe = new Servicos.CTe(unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.RPSNFSe repRPSNFSe = new Repositorio.RPSNFSe(unitOfWork);

                cte.Status = "A";
                cte.DataAutorizacao = DateTime.Now;// verificar
                cte.Protocolo = docProtocolo;
                cte.MensagemRetornoSefaz = sitCodigo + "-" + sitDescricao;

                if (nfseNumero.Length > 9)
                    cte.Numero = Convert.ToInt32(nfseNumero.Substring(nfseNumero.Length - 9));
                else
                    cte.Numero = Convert.ToInt32(nfseNumero ?? "0");

                //int quantidadeNFSes = repCTe.ContarCTePorChaveUnica(cte.Numero, cte.Serie.Codigo, cte.ModeloDocumentoFiscal.Codigo, cte.Empresa.Codigo, cte.TipoAmbiente);
                //cte.TipoControle = quantidadeNFSes + 1;
                cte.TipoControle = cte.Codigo;

                cte.RPS.Status = "A";
                cte.RPS.CodigoRetorno = //nfseOracle.CodigoRetornoRPS;
                cte.RPS.MensagemRetorno = cte.MensagemRetornoSefaz;
                cte.RPS.Protocolo = docProtocolo;
                cte.RPS.Data = DateTime.Now;// verificar
                repCTe.Atualizar(cte);
                repRPSNFSe.Atualizar(cte.RPS);

                //salvar XML
                if (!string.IsNullOrEmpty(docXMLBase64))
                    this.SalvarArquivoXML(cte, false, docXMLBase64, null, Dominio.Enumeradores.TipoXMLCTe.Autorizacao, cte.Status, unitOfWork);

                if (!string.IsNullOrEmpty(docPDFBase64))
                    this.SalvarArquivoPDF(cte, docPDFBase64, unitOfWork);

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCTe.BuscarPorCTe(cte.Codigo);

                if (cargaCTe != null && cargaCTe.Carga.problemaCTE)
                {
                    cargaCTe.Carga.PossuiPendencia = false;
                    cargaCTe.Carga.problemaCTE = false;
                    cargaCTe.Carga.MotivoPendencia = string.Empty;
                    repCarga.Atualizar(cargaCTe.Carga);
                }

                serCTe.AjustarAverbacoesParaAutorizacao(cte.Codigo, unitOfWork);
                //Servicos.Auditoria.Auditoria.Auditar(Auditado, cte, "Integrou NFS-e autorizada", unitOfWork);

                unitOfWork.CommitChanges();
            }
            catch
            {
                unitOfWork.Rollback();
                throw;
            }
        }

        #endregion Métodos Privados

    }
}