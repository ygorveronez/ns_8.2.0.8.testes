using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NFe.Classes;
using NFe.Classes.Informacoes;
using NFe.Classes.Informacoes.Cobranca;
using NFe.Classes.Informacoes.Destinatario;
using NFe.Classes.Informacoes.Detalhe;
using NFe.Classes.Informacoes.Detalhe.Tributacao;
using NFe.Classes.Informacoes.Detalhe.Tributacao.Estadual;
using NFe.Classes.Informacoes.Detalhe.Tributacao.Estadual.Tipos;
using NFe.Classes.Informacoes.Detalhe.Tributacao.Federal;
using NFe.Classes.Informacoes.Detalhe.Tributacao.Federal.Tipos;
using NFe.Classes.Informacoes.Emitente;
using NFe.Classes.Informacoes.Identificacao;
using NFe.Classes.Informacoes.Identificacao.Tipos;
using NFe.Classes.Informacoes.Observacoes;
using NFe.Classes.Informacoes.Pagamento;
using NFe.Classes.Informacoes.Total;
using NFe.Classes.Informacoes.Transporte;
using NFe.Classes.Servicos.Tipos;
using NFe.Servicos;
using NFe.Servicos.Retorno;
using NFe.Utils;
using NFe.Utils.Assinatura;
using NFe.Utils.Email;
using NFe.Utils.InformacoesSuplementares;
using NFe.Utils.NFe;

namespace Zeus.Embarcador.Zeus
{
    public class Zeus
    {
        public string CriarEnviarNFe(int codigoNFe, ModeloDocumento modelo, VersaoServico versao, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                #region Cria e Envia NFe                
                NFe.Classes.NFe _nfe;
                _nfe = GetNf(codigoNFe, ModeloDocumento.NFe, VersaoServico.ve310, unitOfWork);
                _nfe.Assina();
                var servicoNFe = new ServicosNFe(ConfiguracaoServico.Instancia);
                var retornoEnvio = servicoNFe.NfeRecepcao(codigoNFe, new List<NFe.Classes.NFe> { _nfe });

                //TrataRetorno(retornoEnvio);
                return retornoEnvio.RetornoCompletoStr;

                #endregion
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(ex.Message))
                    return ex.Message;
                else
                    return "Problemas ao criar e enviar NF-e";
            }
        }
        protected virtual NFe.Classes.NFe GetNf(int codigoNFe, ModeloDocumento modelo, VersaoServico versao, Repositorio.UnitOfWork unitOfWork)
        {
            var nf = new NFe.Classes.NFe { infNFe = GetInf(codigoNFe, modelo, versao, unitOfWork) };
            return nf;
        }

        protected virtual infNFe GetInf(int codigoNFe, ModeloDocumento modelo, VersaoServico versao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.NotaFiscal.NotaFiscal repNFe = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unitOfWork);
            Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe = repNFe.BuscarPorCodigo(codigoNFe);

            var infNFe = new infNFe
            {
                versao = Conversao.VersaoServicoParaString(versao),
                ide = GetIdentificacao(nfe, modelo, versao),
                emit = GetEmitente(nfe),
                dest = GetDestinatario(nfe, versao, modelo),
                transp = GetTransporte(nfe)
            };

            for (var i = 0; i < nfe.ItensNFe.Count; i++)
            {
                infNFe.det.Add(GetDetalhe(nfe.ItensNFe[i], i, infNFe.emit.CRT, modelo));
            }

            infNFe.total = GetTotal(nfe, versao, infNFe.det);

            if (infNFe.ide.mod == ModeloDocumento.NFe & versao == VersaoServico.ve310)
                infNFe.cobr = GetCobranca(nfe, infNFe.total.ICMSTot); //V3.00 Somente
            if (infNFe.ide.mod == ModeloDocumento.NFCe)
                infNFe.pag = GetPagamento(nfe, infNFe.total.ICMSTot); //NFCe Somente  

            if (infNFe.ide.mod == ModeloDocumento.NFCe)
                infNFe.infAdic = new infAdic() { infCpl = nfe.ObservacaoTributaria }; //Susgestão para impressão do troco em NFCe

            return infNFe;
        }

        protected virtual ide GetIdentificacao(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe, ModeloDocumento modelo, VersaoServico versao)
        {
            var ide = new ide
            {
                cUF = (Estado)nfe.Empresa.Localidade.Estado.CodigoIBGE, //Estado.SE,
                natOp = nfe.NaturezaDaOperacao.Descricao,
                indPag = nfe.ParcelasNFe == null ? IndicadorPagamento.ipOutras : nfe.ParcelasNFe.Count() == 1 ? IndicadorPagamento.ipVista : IndicadorPagamento.ipPrazo,//IndicadorPagamento.ipVista,
                mod = modelo,
                serie = nfe.EmpresaSerie.Numero,
                nNF = nfe.Numero,
                tpNF = nfe.TipoEmissao == Dominio.Enumeradores.TipoEmissaoNFe.Saida ? TipoNFe.tnSaida : TipoNFe.tnEntrada,
                cMunFG = nfe.Empresa.Localidade.CodigoIBGE,
                tpEmis = TipoEmissao.teNormal,
                tpImp = TipoImpressao.tiRetrato,
                cNF = Convert.ToString(nfe.Numero),
                tpAmb = nfe.Empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao ? TipoAmbiente.taHomologacao : TipoAmbiente.taProducao,
                finNFe = nfe.Finalidade == Dominio.Enumeradores.FinalidadeNFe.Normal ? FinalidadeNFe.fnNormal : nfe.Finalidade == Dominio.Enumeradores.FinalidadeNFe.Ajuste ? FinalidadeNFe.fnAjuste : nfe.Finalidade == Dominio.Enumeradores.FinalidadeNFe.Complementar ? FinalidadeNFe.fnComplementar : FinalidadeNFe.fnDevolucao,
                verProc = "1.000"
            };

            if (ide.tpEmis != TipoEmissao.teNormal)
            {
                ide.dhCont = DateTime.Now.ToString(versao == VersaoServico.ve310 ? "yyyy-MM-ddTHH:mm:sszzz" : "yyyy-MM-ddTHH:mm:ss");
                ide.xJust = "TESTE DE CONTIGÊNCIA PARA NFe/NFCe";
            }

            #region V2.00

            if (versao == VersaoServico.ve200)
            {
                ide.dEmi = DateTime.Today.ToString("yyyy-MM-dd"); //Mude aqui para enviar a nfe vinculada ao EPEC, V2.00
                ide.dSaiEnt = DateTime.Today.ToString("yyyy-MM-dd");
            }

            #endregion

            #region V3.00

            if (versao != VersaoServico.ve310) return ide;
            ide.idDest = nfe.Cliente.Localidade.Estado.Sigla == "EX" ? DestinoOperacao.doExterior : nfe.Empresa.Localidade.Estado.Sigla == nfe.Cliente.Localidade.Estado.Sigla ? DestinoOperacao.doInterna : DestinoOperacao.doInterestadual;
            ide.dhEmi = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz");
            //Mude aqui para enviar a nfe vinculada ao EPEC, V3.10
            if (ide.mod == ModeloDocumento.NFe)
                ide.dhSaiEnt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz");
            else
                ide.tpImp = TipoImpressao.tiNFCe;
            ide.procEmi = ProcessoEmissao.peAplicativoContribuinte;
            ide.indFinal = nfe.Cliente.IndicadorIE == Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE.NaoContribuinte ? ConsumidorFinal.cfConsumidorFinal : ConsumidorFinal.cfNao; //NFCe: Tem que ser consumidor Final
            ide.indPres = nfe.IndicadorPresenca == Dominio.Enumeradores.IndicadorPresencaNFe.Internet ? PresencaComprador.pcInternet : nfe.IndicadorPresenca == Dominio.Enumeradores.IndicadorPresencaNFe.NaoSeAplica ? PresencaComprador.pcNao : nfe.IndicadorPresenca == Dominio.Enumeradores.IndicadorPresencaNFe.NFCe ? PresencaComprador.pcPresencial : nfe.IndicadorPresenca == Dominio.Enumeradores.IndicadorPresencaNFe.Outros ? PresencaComprador.pcOutros : nfe.IndicadorPresenca == Dominio.Enumeradores.IndicadorPresencaNFe.Presencial ? PresencaComprador.pcPresencial : PresencaComprador.pcTeleatendimento; //NFCe: deve ser 1 ou 4

            #endregion

            return ide;
        }

        protected virtual emit GetEmitente(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe)
        {
            var emit = new emit
            {
                IEST = nfe.Empresa.Inscricao_ST,
                IM = nfe.Empresa.InscricaoMunicipal,
                xNome = nfe.Empresa.RazaoSocial,
                xFant = nfe.Empresa.NomeFantasia,
                IE = nfe.Empresa.InscricaoEstadual,
                CNPJ = nfe.Empresa.CNPJ,
                CNAE = nfe.Empresa.CNAE
            };
            emit.enderEmit = GetEnderecoEmitente(nfe);
            return emit;
        }

        protected virtual enderEmit GetEnderecoEmitente(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe)
        {
            var enderEmit = new enderEmit
            {
                xLgr = nfe.Empresa.Endereco,
                nro = nfe.Empresa.Numero,
                xCpl = nfe.Empresa.Complemento,
                xBairro = nfe.Empresa.Bairro,
                cMun = nfe.Empresa.Localidade.CodigoIBGE,
                xMun = nfe.Empresa.Localidade.Descricao,
                UF = nfe.Empresa.Localidade.Estado.Sigla,
                CEP = nfe.Empresa.CEP,
                fone = Convert.ToInt64(Utilidades.String.OnlyNumbers(nfe.Empresa.Telefone)),
                cPais = nfe.Empresa.Localidade.Pais.Codigo,
                xPais = nfe.Empresa.Localidade.Pais.Nome
            };
            return enderEmit;
        }

        protected virtual dest GetDestinatario(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe, VersaoServico versao, ModeloDocumento modelo)
        {

            var dest = new dest(versao)
            {
                CNPJ = nfe.Empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao ? "99999999000191" : nfe.Cliente.Tipo == "J" ? Convert.ToString(nfe.Cliente.CPF_CNPJ) : "",
                email = nfe.Cliente.Email,
                CPF = nfe.Empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao ? "" : nfe.Cliente.Tipo == "F" ? Convert.ToString(nfe.Cliente.CPF_CNPJ) : "",
                idEstrangeiro = nfe.Empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao ? "" : nfe.Cliente.Tipo == "E" ? Convert.ToString(nfe.Cliente.RG_Passaporte) : "",
                IE = nfe.Empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao ? "" : nfe.Cliente.IE_RG,
                IM = nfe.Empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao ? "" : nfe.Cliente.InscricaoMunicipal,
                ISUF = nfe.Empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao ? "" : nfe.Cliente.InscricaoSuframa
            };
            if (modelo == ModeloDocumento.NFe)
            {
                dest.xNome = nfe.Empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao ? "NF-E EMITIDA EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL" : nfe.Cliente.Nome; //Obrigatório para NFe e opcional para NFCe                
                dest.enderDest = GetEnderecoDestinatario(nfe); //Obrigatório para NFe e opcional para NFCe
            }

            //if (versao == VersaoServico.ve200)
            //    dest.IE = "ISENTO";
            if (versao != VersaoServico.ve310) return dest;
            dest.indIEDest = nfe.Cliente.IndicadorIE == Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE.ContribuinteICMS ? indIEDest.ContribuinteICMS : nfe.Cliente.IndicadorIE == Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE.ContribuinteIsento ? indIEDest.Isento : indIEDest.NaoContribuinte; //NFCe: Tem que ser não contribuinte V3.00 Somente

            return dest;
        }

        protected virtual enderDest GetEnderecoDestinatario(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe)
        {
            var enderDest = new enderDest
            {
                xLgr = nfe.Cliente.Endereco,
                nro = nfe.Cliente.Numero,
                xBairro = nfe.Cliente.Bairro,
                cMun = nfe.Cliente.Localidade.CodigoIBGE,
                xMun = nfe.Cliente.Localidade.Descricao,
                UF = nfe.Cliente.Localidade.Estado.Sigla,
                CEP = nfe.Cliente.CEP,
                cPais = nfe.Cliente.Localidade.Pais.Codigo,
                xPais = nfe.Cliente.Localidade.Pais.Nome,
                fone = Convert.ToInt64(nfe.Cliente.Telefone1),
                xCpl = nfe.Cliente.Complemento
            };
            return enderDest;
        }

        protected virtual det GetDetalhe(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalProdutos item, int i, CRT crt, ModeloDocumento modelo)
        {
            var det = new det
            {
                nItem = i + 1,
                prod = GetProduto(item, i + 1),
                imposto = new imposto
                {
                    vTotTrib = 0            
                }
            };

            if (modelo == ModeloDocumento.NFe)
            {
                if (item.Produto != null && item.CSTIPI != null && item.ValorIPI > 0)
                {
                    det.imposto.IPI = new IPI()
                    {
                        cEnq = Convert.ToInt16(item.Produto.CodigoEnquadramentoIPI),
                        TipoIPI = new IPITrib() { CST = RetornaCSTIPI(item.CSTIPI), pIPI = item.AliquotaIPI, vBC = item.BCIPI, vIPI = item.ValorIPI }
                    };
                }
                if (item.Produto != null && item.CSTCOFINS != null && item.ValorCOFINS > 0)
                {
                    det.imposto.COFINS = new COFINS()
                    {
                        TipoCOFINS = new COFINSOutr { CST = RetornaCSTCOFINS(item.CSTCOFINS), pCOFINS = item.AliquotaCOFINS, vBC = item.BCCOFINS, vCOFINS = item.ValorCOFINS }
                    };
                }
                if (item.Produto != null && item.CSTPIS != null && item.ValorPIS > 0)
                {
                    det.imposto.PIS = new PIS()
                    {
                        TipoPIS = new PISOutr { CST = RetornaCSTPIS(item.CSTCOFINS), pPIS = item.AliquotaCOFINS, vBC = item.BCCOFINS, vPIS = item.ValorCOFINS }
                    };
                }
                if (item.Produto != null && item.CSTICMS != null && item.Produto != null)
                {
                    det.imposto.ICMS = new ICMS()
                    {
                        TipoICMS = RetornaImpostoICMS(item.CSTICMS, item)
                    };
                }
                if (item.Produto != null && item.PercentualPartilha > 0 && item.AliquotaICMSDestino > 0 && item.AliquotaICMSInterno > 0)
                {
                    det.imposto.ICMSUFDest = new ICMSUFDest()
                    {
                        pFCPUFDest = item.AliquotaFCP,
                        pICMSInter = item.AliquotaICMSInterno,
                        pICMSInterPart = item.PercentualPartilha,
                        pICMSUFDest = item.AliquotaICMSDestino,
                        vBCUFDest = item.BCICMSDestino,
                        vFCPUFDest = item.ValorFCP,
                        vICMSUFDest = item.ValorICMSDestino,
                        vICMSUFRemet = item.ValorICMSRemetente
                    };
                }
                if (item.Produto != null && item.BaseII > 0 && item.ValorII > 0) {
                    det.imposto.II = new II()
                    {
                        vBC = item.BaseII,
                        vDespAdu = item.ValorDespesaII,
                        vII = item.ValorII,
                        vIOF = item.ValorIOFII
                    };
                }
                if (item.Servico != null)
                {
                    det.imposto.ISSQN = new NFe.Classes.Informacoes.Detalhe.Tributacao.Municipal.ISSQN()
                    {
                        cListServ = item.Servico.DescricaoCodigoServico,
                        cMun = item.NotaFiscal.LocalidadePrestacaoServico.CodigoIBGE,
                        cMunFG = item.NotaFiscal.LocalidadePrestacaoServico.CodigoIBGE,
                        cPais = item.NotaFiscal.LocalidadePrestacaoServico.Pais.Codigo,
                        cServico = "",
                        IndicadorIss = (NFe.Classes.Informacoes.Detalhe.Tributacao.Municipal.IndicadorISS)(int)item.ExigibilidadeISS - 1,
                        indIncentivo = (NFe.Classes.Informacoes.Detalhe.Tributacao.Municipal.indIncentivo)(item.IncentivoFiscal == true ? 0 : 1),
                        nProcesso = item.ProcessoJudicial,
                        vAliq = item.AliquotaISS,
                        vBC = item.BaseISS,
                        vDeducao = item.BCDeducao,
                        vDescCond = item.DescontoCondicional,
                        vDescIncond = item.DescontoIncondicional,
                        vISSQN = item.ValorISS,
                        vISSRet = item.RetencaoISS,
                        vOutro = item.OutrasRetencoes
                    };
                }
                //det.impostoDevol = new impostoDevol() { IPI = new IPIDevolvido() { vIPIDevol = 10 }, pDevol = 100 };
            }

            return det;
        }

        protected virtual ICMSBasico RetornaImpostoICMS(Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS? cst, Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalProdutos item)
        {
            if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN101)
                return new ICMSSN101 { CSOSN = Csosnicms.Csosn101, orig = (OrigemMercadoria)item.Produto.OrigemMercadoria, pCredSN = 0, vCredICMSSN = 0 };
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN102)
                return new ICMSSN102 { CSOSN = Csosnicms.Csosn102, orig = (OrigemMercadoria)item.Produto.OrigemMercadoria };
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN201)
                return new ICMSSN201 { CSOSN = Csosnicms.Csosn201, orig = (OrigemMercadoria)item.Produto.OrigemMercadoria, pCredSN = 0, vCredICMSSN = 0, modBCST = DeterminacaoBaseIcmsSt.DbisMargemValorAgregado, pICMSST = item.AliquotaICMSST, pMVAST = item.MVAICMSST, pRedBCST = item.ReducaoBCICMSST, vBCST = item.BCICMSST, vICMSST = item.ValorICMSST };
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN202)
                return new ICMSSN202 { CSOSN = Csosnicms.Csosn202, orig = (OrigemMercadoria)item.Produto.OrigemMercadoria, modBCST = DeterminacaoBaseIcmsSt.DbisPrecoTabelado, pICMSST = item.AliquotaICMSST, pMVAST = item.MVAICMSST, pRedBCST = item.ReducaoBCICMSST, vBCST = item.BCICMSST, vICMSST = item.ValorICMSST };
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN500)
                return new ICMSSN500 { CSOSN = Csosnicms.Csosn500, orig = (OrigemMercadoria)item.Produto.OrigemMercadoria, vBCSTRet = 0, vICMSSTRet = 0 };
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN900)
                return new ICMSSN900 { CSOSN = Csosnicms.Csosn900, orig = (OrigemMercadoria)item.Produto.OrigemMercadoria, modBC = DeterminacaoBaseIcms.DbiPrecoTabelado, modBCST = DeterminacaoBaseIcmsSt.DbisPrecoTabelado, pCredSN = 0, pICMS = item.AliquotaICMS, pICMSST = item.AliquotaICMSST, pMVAST = item.MVAICMSST, pRedBC = item.ReducaoBCICMS, pRedBCST = item.ReducaoBCICMSST, vBC = item.BCICMS, vBCST = item.BCICMSST, vCredICMSSN = 0, vICMS = item.ValorICMS, vICMSST = item.ValorICMSST };
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST00)
                return new ICMS00 { CST = Csticms.Cst00, orig = (OrigemMercadoria)item.Produto.OrigemMercadoria, modBC = DeterminacaoBaseIcms.DbiPrecoTabelado, pICMS = item.AliquotaICMS, vBC = item.BCICMS, vICMS = item.ValorICMS };
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST10)
                return new ICMS10 { CST = Csticms.Cst10, orig = (OrigemMercadoria)item.Produto.OrigemMercadoria, modBC = DeterminacaoBaseIcms.DbiPrecoTabelado, pICMS = item.AliquotaICMS, vBC = item.BCICMS, vICMS = item.ValorICMS, modBCST = DeterminacaoBaseIcmsSt.DbisPrecoTabelado, pICMSST = item.AliquotaICMSST, pMVAST = item.MVAICMSST, pRedBCST = item.ReducaoBCICMSST, vBCST = item.BCICMSST, vICMSST = item.ValorICMSST };
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST20)
                return new ICMS20 { CST = Csticms.Cst20, orig = (OrigemMercadoria)item.Produto.OrigemMercadoria, modBC = DeterminacaoBaseIcms.DbiPrecoTabelado, pICMS = item.AliquotaICMS, vBC = item.BCICMS, vICMS = item.ValorICMS, vICMSDeson = item.ValorICMSDesonerado, pRedBC = 0, motDesICMS = (MotivoDesoneracaoIcms)item.MotivoDesoneracao };
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST30)
                return new ICMS30 { CST = Csticms.Cst30, orig = (OrigemMercadoria)item.Produto.OrigemMercadoria, modBCST = DeterminacaoBaseIcmsSt.DbisPrecoTabelado, pICMSST = item.AliquotaICMSST, pMVAST = item.MVAICMSST, pRedBCST = item.ReducaoBCICMSST, vBCST = item.BCICMSST, vICMSST = item.ValorICMSST, motDesICMS = (MotivoDesoneracaoIcms)item.MotivoDesoneracao, vICMSDeson = item.ValorICMSDesonerado };
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST40)
                return new ICMS40 { CST = Csticms.Cst40, orig = (OrigemMercadoria)item.Produto.OrigemMercadoria, motDesICMS = (MotivoDesoneracaoIcms)item.MotivoDesoneracao, vICMSDeson = item.ValorICMSDesonerado };
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST51)
                return new ICMS51 { CST = Csticms.Cst51, orig = (OrigemMercadoria)item.Produto.OrigemMercadoria, modBC = DeterminacaoBaseIcms.DbiPrecoTabelado, pICMS = item.AliquotaICMS, vBC = item.BCICMS, vICMS = item.ValorICMS, pRedBC = item.ReducaoBCICMS };
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST60)
                return new ICMS60 { CST = Csticms.Cst60, orig = (OrigemMercadoria)item.Produto.OrigemMercadoria };
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST70)
                return new ICMS70 { CST = Csticms.Cst70, orig = (OrigemMercadoria)item.Produto.OrigemMercadoria, modBC = DeterminacaoBaseIcms.DbiPrecoTabelado, pICMS = item.AliquotaICMS, vBC = item.BCICMS, vICMS = item.ValorICMS, modBCST = DeterminacaoBaseIcmsSt.DbisPrecoTabelado, pICMSST = item.AliquotaICMSST, pMVAST = item.MVAICMSST, pRedBCST = item.ReducaoBCICMSST, vBCST = item.BCICMSST, vICMSST = item.ValorICMSST, vICMSDeson = item.ValorICMSDesonerado, pRedBC = 0, motDesICMS = (MotivoDesoneracaoIcms)item.MotivoDesoneracao };
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST90)
                return new ICMS90 { CST = Csticms.Cst90, orig = (OrigemMercadoria)item.Produto.OrigemMercadoria, modBC = DeterminacaoBaseIcms.DbiPrecoTabelado, pICMS = item.AliquotaICMS, vBC = item.BCICMS, vICMS = item.ValorICMS, modBCST = DeterminacaoBaseIcmsSt.DbisPrecoTabelado, pICMSST = item.AliquotaICMSST, pMVAST = item.MVAICMSST, pRedBCST = item.ReducaoBCICMSST, vBCST = item.BCICMSST, vICMSST = item.ValorICMSST, vICMSDeson = item.ValorICMSDesonerado, pRedBC = 0, motDesICMS = (MotivoDesoneracaoIcms)item.MotivoDesoneracao };
            else
                return new ICMSSN101 { CSOSN = Csosnicms.Csosn101, orig = (OrigemMercadoria)item.Produto.OrigemMercadoria, pCredSN = 0, vCredICMSSN = 0 };
        }

        protected virtual CSTPIS RetornaCSTPIS(Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS? cst)
        {
            if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST01)
                return CSTPIS.pis01;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST02)
                return CSTPIS.pis02;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST03)
                return CSTPIS.pis03;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST04)
                return CSTPIS.pis04;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST05)
                return CSTPIS.pis05;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST06)
                return CSTPIS.pis06;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST07)
                return CSTPIS.pis07;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST08)
                return CSTPIS.pis08;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST09)
                return CSTPIS.pis09;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST49)
                return CSTPIS.pis49;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST50)
                return CSTPIS.pis50;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST51)
                return CSTPIS.pis51;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST52)
                return CSTPIS.pis52;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST53)
                return CSTPIS.pis53;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST54)
                return CSTPIS.pis54;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST55)
                return CSTPIS.pis55;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST56)
                return CSTPIS.pis56;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST60)
                return CSTPIS.pis60;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST61)
                return CSTPIS.pis61;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST62)
                return CSTPIS.pis62;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST63)
                return CSTPIS.pis63;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST64)
                return CSTPIS.pis64;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST65)
                return CSTPIS.pis65;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST66)
                return CSTPIS.pis66;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST67)
                return CSTPIS.pis67;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST70)
                return CSTPIS.pis70;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST71)
                return CSTPIS.pis71;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST72)
                return CSTPIS.pis72;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST73)
                return CSTPIS.pis73;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST74)
                return CSTPIS.pis74;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST75)
                return CSTPIS.pis75;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST98)
                return CSTPIS.pis98;
            else
                return CSTPIS.pis99;
        }

        protected virtual CSTCOFINS RetornaCSTCOFINS(Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS? cst)
        {
            if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST01)
                return CSTCOFINS.cofins01;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST02)
                return CSTCOFINS.cofins02;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST03)
                return CSTCOFINS.cofins03;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST04)
                return CSTCOFINS.cofins04;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST05)
                return CSTCOFINS.cofins05;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST06)
                return CSTCOFINS.cofins06;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST07)
                return CSTCOFINS.cofins07;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST08)
                return CSTCOFINS.cofins08;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST09)
                return CSTCOFINS.cofins09;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST49)
                return CSTCOFINS.cofins49;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST50)
                return CSTCOFINS.cofins50;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST51)
                return CSTCOFINS.cofins51;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST52)
                return CSTCOFINS.cofins52;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST53)
                return CSTCOFINS.cofins53;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST54)
                return CSTCOFINS.cofins54;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST55)
                return CSTCOFINS.cofins55;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST56)
                return CSTCOFINS.cofins56;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST60)
                return CSTCOFINS.cofins60;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST61)
                return CSTCOFINS.cofins61;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST62)
                return CSTCOFINS.cofins62;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST63)
                return CSTCOFINS.cofins63;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST64)
                return CSTCOFINS.cofins64;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST65)
                return CSTCOFINS.cofins65;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST66)
                return CSTCOFINS.cofins66;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST67)
                return CSTCOFINS.cofins67;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST70)
                return CSTCOFINS.cofins70;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST71)
                return CSTCOFINS.cofins71;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST72)
                return CSTCOFINS.cofins72;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST73)
                return CSTCOFINS.cofins73;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST74)
                return CSTCOFINS.cofins74;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST75)
                return CSTCOFINS.cofins75;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST98)
                return CSTCOFINS.cofins98;
            else
                return CSTCOFINS.cofins99;
        }

        protected virtual CSTIPI RetornaCSTIPI(Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTIPI? cst)
        {
            if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST00)
                return CSTIPI.ipi00;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST01)
                return CSTIPI.ipi01;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST02)
                return CSTIPI.ipi02;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST03)
                return CSTIPI.ipi03;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST04)
                return CSTIPI.ipi04;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST05)
                return CSTIPI.ipi05;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST49)
                return CSTIPI.ipi49;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST50)
                return CSTIPI.ipi50;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST51)
                return CSTIPI.ipi51;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST52)
                return CSTIPI.ipi52;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST53)
                return CSTIPI.ipi53;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST54)
                return CSTIPI.ipi54;
            else if (cst == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTIPI.CST55)
                return CSTIPI.ipi55;
            else
                return CSTIPI.ipi99;
        }

        protected virtual prod GetProduto(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalProdutos item, int i)
        {
            var p = new prod
            {
                cProd = i.ToString().PadLeft(item.Produto.Codigo, '0'),
                cEAN = item.Produto.CodigoEAN,
                xProd = item.NotaFiscal.Empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao ? "NOTA FISCAL EMITIDA EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL" : item.Produto.DescricaoNotaFiscal,
                NCM = item.Produto.CodigoNCM,
                CFOP = item.CFOP.CodigoCFOP,
                uCom = item.Produto.DescricaoUnidadeDeMedida,
                qCom = item.Quantidade,
                vUnCom = item.ValorUnitario,
                vProd = item.ValorTotal,
                vDesc = item.ValorDesconto,
                cEANTrib = "",
                uTrib = item.Produto.DescricaoUnidadeDeMedida,
                qTrib = item.Quantidade,
                vUnTrib = item.ValorUnitario,
                indTot = IndicadorTotal.ValorDoItemCompoeTotalNF,
                CEST = item.Produto.CodigoCEST,
                EXTIPI = "",
                nFCI = "",
                nItemPed = Convert.ToInt16(item.NumeroItemOrdemCompra),
                detExport = null,
                DI = null,
                nRECOPI = "",
                NVE = null,
                ProdutoEspecifico = null,
                vFrete = item.ValorFrete,
                vOutro = item.ValorOutrasDespesas,
                vSeg = item.ValorSeguro,
                xPed = item.NumeroOrdemCompra
                //NVE = {"AA0001", "AB0002", "AC0002"},
                //CEST = ?

                //ProdutoEspecifico = new arma
                //{
                //    tpArma = TipoArma.UsoPermitido,
                //    nSerie = "123456",
                //    nCano = "123456",
                //    descr = "TESTE DE ARMA"
                //}
            };
            return p;
        }

        protected virtual ICMSBasico InformarICMS(Csticms CST, VersaoServico versao)
        {
            var icms20 = new ICMS20
            {
                orig = OrigemMercadoria.OmNacional,
                CST = Csticms.Cst20,
                modBC = DeterminacaoBaseIcms.DbiValorOperacao,
                vBC = 1,
                pICMS = 17,
                vICMS = 0.17m,
                motDesICMS = MotivoDesoneracaoIcms.MdiTaxi
            };
            if (versao == VersaoServico.ve310)
                icms20.vICMSDeson = 0.10m; //V3.00 ou maior Somente

            switch (CST)
            {
                case Csticms.Cst00:
                    return new ICMS00
                    {
                        CST = Csticms.Cst00,
                        modBC = DeterminacaoBaseIcms.DbiValorOperacao,
                        orig = OrigemMercadoria.OmNacional,
                        pICMS = 17,
                        vBC = 1,
                        vICMS = 0.17m
                    };
                case Csticms.Cst20:
                    return icms20;
                    //Outros casos aqui
            }

            return new ICMS10();
        }

        protected virtual ICMSBasico InformarCSOSN(Csosnicms CST)
        {
            switch (CST)
            {
                case Csosnicms.Csosn101:
                    return new ICMSSN101
                    {
                        CSOSN = Csosnicms.Csosn101,
                        orig = OrigemMercadoria.OmNacional
                    };
                case Csosnicms.Csosn102:
                    return new ICMSSN102
                    {
                        CSOSN = Csosnicms.Csosn102,
                        orig = OrigemMercadoria.OmNacional
                    };
                //Outros casos aqui
                default:
                    return new ICMSSN201();
            }
        }

        protected virtual total GetTotal(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe, VersaoServico versao, List<det> produtos)
        {
            var icmsTot = new ICMSTot
            {
                vProd = produtos.Sum(p => p.prod.vProd),
                vNF = produtos.Sum(p => p.prod.vProd) - produtos.Sum(p => p.prod.vDesc ?? 0),
                vDesc = produtos.Sum(p => p.prod.vDesc ?? 0),
                vOutro = produtos.Sum(p => p.prod.vOutro ?? 0),
                vTotTrib = produtos.Sum(p => p.imposto.vTotTrib ?? 0),
                vSeg = produtos.Sum(p => p.prod.vSeg ?? 0),
            };
            if (versao == VersaoServico.ve310)
                icmsTot.vICMSDeson = 0;

            foreach (var produto in produtos)
            {
                if (produto.imposto.IPI != null && produto.imposto.IPI.TipoIPI.GetType() == typeof(IPITrib))
                    icmsTot.vIPI = icmsTot.vIPI + ((IPITrib)produto.imposto.IPI.TipoIPI).vIPI ?? 0;

                if (produto.imposto.PIS != null && produto.imposto.PIS.TipoPIS.GetType() == typeof(PISOutr))
                    icmsTot.vPIS = icmsTot.vPIS + ((PISOutr)produto.imposto.PIS.TipoPIS).vPIS ?? 0;

                if (produto.imposto.COFINS != null && produto.imposto.COFINS.TipoCOFINS.GetType() == typeof(COFINSOutr))
                    icmsTot.vCOFINS = icmsTot.vCOFINS + ((COFINSOutr)produto.imposto.COFINS.TipoCOFINS).vCOFINS ?? 0;

                if (produto.imposto.II != null && produto.imposto.II.GetType() == typeof(II))
                    icmsTot.vII = icmsTot.vII + ((II)produto.imposto.II).vII;

                if (produto.imposto.ICMSUFDest != null && produto.imposto.ICMSUFDest.GetType() == typeof(ICMSUFDest))
                    icmsTot.vFCPUFDest = icmsTot.vFCPUFDest + ((ICMSUFDest)produto.imposto.ICMSUFDest).vFCPUFDest ?? 0;

                if (produto.imposto.ICMSUFDest != null && produto.imposto.ICMSUFDest.GetType() == typeof(ICMSUFDest))
                    icmsTot.vICMSUFDest = icmsTot.vICMSUFDest + ((ICMSUFDest)produto.imposto.ICMSUFDest).vICMSUFDest ?? 0;

                if (produto.imposto.ICMSUFDest != null && produto.imposto.ICMSUFDest.GetType() == typeof(ICMSUFDest))
                    icmsTot.vICMSUFRemet = icmsTot.vICMSUFRemet + ((ICMSUFDest)produto.imposto.ICMSUFDest).vICMSUFRemet ?? 0;

                if (produto.imposto.ICMS.TipoICMS.GetType() == typeof(ICMS00))
                {                    
                    icmsTot.vBC = icmsTot.vBC + ((ICMS00)produto.imposto.ICMS.TipoICMS).vBC;
                    icmsTot.vICMS = icmsTot.vICMS + ((ICMS00)produto.imposto.ICMS.TipoICMS).vICMS;
                }
                if (produto.imposto.ICMS.TipoICMS.GetType() == typeof(ICMS10))
                {                    
                    icmsTot.vBC = icmsTot.vBC + ((ICMS10)produto.imposto.ICMS.TipoICMS).vBC;
                    icmsTot.vICMS = icmsTot.vICMS + ((ICMS10)produto.imposto.ICMS.TipoICMS).vICMS;
                    icmsTot.vBCST = icmsTot.vST + ((ICMS10)produto.imposto.ICMS.TipoICMS).vBCST;
                    icmsTot.vST = icmsTot.vST + ((ICMS10)produto.imposto.ICMS.TipoICMS).vICMSST;
                }
                if (produto.imposto.ICMS.TipoICMS.GetType() == typeof(ICMS20))
                {
                    icmsTot.vICMSDeson = icmsTot.vICMSDeson + ((ICMS20)produto.imposto.ICMS.TipoICMS).vICMSDeson;
                    icmsTot.vBC = icmsTot.vBC + ((ICMS20)produto.imposto.ICMS.TipoICMS).vBC;
                    icmsTot.vICMS = icmsTot.vICMS + ((ICMS20)produto.imposto.ICMS.TipoICMS).vICMS;
                }
                if (produto.imposto.ICMS.TipoICMS.GetType() == typeof(ICMS30))
                {
                    icmsTot.vICMSDeson = icmsTot.vICMSDeson + ((ICMS30)produto.imposto.ICMS.TipoICMS).vICMSDeson;
                    icmsTot.vBCST = icmsTot.vST + ((ICMS30)produto.imposto.ICMS.TipoICMS).vBCST;
                    icmsTot.vST = icmsTot.vST + ((ICMS30)produto.imposto.ICMS.TipoICMS).vICMSST;
                }
                if (produto.imposto.ICMS.TipoICMS.GetType() == typeof(ICMS40))
                {
                    icmsTot.vICMSDeson = icmsTot.vICMSDeson + ((ICMS40)produto.imposto.ICMS.TipoICMS).vICMSDeson;                    
                }
                if (produto.imposto.ICMS.TipoICMS.GetType() == typeof(ICMS51))
                {                    
                    icmsTot.vBC = icmsTot.vBC + ((ICMS51)produto.imposto.ICMS.TipoICMS).vBC ?? 0;
                    icmsTot.vICMS = icmsTot.vICMS + ((ICMS51)produto.imposto.ICMS.TipoICMS).vICMS ?? 0;
                }
                if (produto.imposto.ICMS.TipoICMS.GetType() == typeof(ICMS70))
                {
                    icmsTot.vICMSDeson = icmsTot.vICMSDeson + ((ICMS70)produto.imposto.ICMS.TipoICMS).vICMSDeson;
                    icmsTot.vBC = icmsTot.vBC + ((ICMS70)produto.imposto.ICMS.TipoICMS).vBC;
                    icmsTot.vICMS = icmsTot.vICMS + ((ICMS70)produto.imposto.ICMS.TipoICMS).vICMS;
                    icmsTot.vBCST = icmsTot.vST + ((ICMS70)produto.imposto.ICMS.TipoICMS).vBCST;
                    icmsTot.vST = icmsTot.vST + ((ICMS70)produto.imposto.ICMS.TipoICMS).vICMSST;
                }
                if (produto.imposto.ICMS.TipoICMS.GetType() == typeof(ICMS90))
                {
                    icmsTot.vICMSDeson = icmsTot.vICMSDeson + ((ICMS90)produto.imposto.ICMS.TipoICMS).vICMSDeson;
                    icmsTot.vBC = icmsTot.vBC + ((ICMS90)produto.imposto.ICMS.TipoICMS).vBC ?? 0;
                    icmsTot.vICMS = icmsTot.vICMS + ((ICMS90)produto.imposto.ICMS.TipoICMS).vICMS ?? 0;
                    icmsTot.vBCST = icmsTot.vST + ((ICMS90)produto.imposto.ICMS.TipoICMS).vBCST ?? 0;
                    icmsTot.vST = icmsTot.vST + ((ICMS90)produto.imposto.ICMS.TipoICMS).vICMSST ?? 0;
                }
                if (produto.imposto.ICMS.TipoICMS.GetType() == typeof(ICMSSN201))
                {                    
                    icmsTot.vBCST = icmsTot.vST + ((ICMSSN201)produto.imposto.ICMS.TipoICMS).vBCST;
                    icmsTot.vST = icmsTot.vST + ((ICMSSN201)produto.imposto.ICMS.TipoICMS).vICMSST;
                }
                if (produto.imposto.ICMS.TipoICMS.GetType() == typeof(ICMSSN202))
                {                    
                    icmsTot.vBCST = icmsTot.vST + ((ICMSSN202)produto.imposto.ICMS.TipoICMS).vBCST;
                    icmsTot.vST = icmsTot.vST + ((ICMSSN202)produto.imposto.ICMS.TipoICMS).vICMSST;
                }
                if (produto.imposto.ICMS.TipoICMS.GetType() == typeof(ICMSSN900))
                {
                    icmsTot.vBC = icmsTot.vBC + ((ICMSSN900)produto.imposto.ICMS.TipoICMS).vBC ?? 0;
                    icmsTot.vICMS = icmsTot.vICMS + ((ICMSSN900)produto.imposto.ICMS.TipoICMS).vICMS ?? 0;
                    icmsTot.vBCST = icmsTot.vST + ((ICMSSN900)produto.imposto.ICMS.TipoICMS).vBCST ?? 0;
                    icmsTot.vST = icmsTot.vST + ((ICMSSN900)produto.imposto.ICMS.TipoICMS).vICMSST ?? 0;
                }                
            }

            var issqnTot = new ISSQNtot
            {
                cRegTrib = RegTribISSQN.RTISSCooperativa,
                dCompet =  "",
                vBC = 0,
                vCOFINS = 0,
                vDeducao = 0,
                vDescCond = 0,
                vDescIncond = 0,
                vISS = 0,
                vISSRet = 0,
                vOutro = 0,
                vPIS = 0,
                vServ = 0
            };

            var t = new total { ICMSTot = icmsTot, ISSQNtot = issqnTot };
            return t;
        }

        protected virtual transp GetTransporte(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe)
        {
            //var volumes = new List<vol> {GetVolume(), GetVolume()};

            var t = new transp
            {
                modFrete = ModalidadeFrete.mfSemFrete //NFCe: Não pode ter frete
                //vol = volumes 
            };

            return t;
        }

        protected virtual vol GetVolume()
        {
            var v = new vol
            {
                esp = "teste de especia",
                lacres = new List<lacres> { new lacres { nLacre = "123456" } }
            };

            return v;
        }

        protected virtual cobr GetCobranca(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe, ICMSTot icmsTot)
        {
            var valorParcela = Valor.Arredondar(icmsTot.vProd / 2, 2);
            var c = new cobr
            {
                fat = new fat { nFat = "12345678910", vLiq = icmsTot.vProd },
                dup = new List<dup>
                {
                    new dup {nDup = "12345678", vDup = valorParcela},
                    new dup {nDup = "987654321", vDup = icmsTot.vProd - valorParcela}
                }
            };

            return c;
        }

        protected virtual List<pag> GetPagamento(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe, ICMSTot icmsTot)
        {
            var valorPagto = Valor.Arredondar(icmsTot.vProd / 2, 2);
            var p = new List<pag>
            {
                new pag {tPag = FormaPagamento.fpDinheiro, vPag = valorPagto},
                new pag {tPag = FormaPagamento.fpCheque, vPag = icmsTot.vProd - valorPagto}
            };
            return p;
        }
    }
}
