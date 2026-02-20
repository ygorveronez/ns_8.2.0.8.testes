using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;

namespace Servicos.Embarcador.Integracao.Totvs
{
    public class Movimento
    {
        public bool IntegrarAcrescimoDescontoContratoTerceiro(Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoIntegracao integracaoContrato, Repositorio.UnitOfWork unitOfWork, out string msgRetorno)
        {
            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();

            msgRetorno = string.Empty;

            if (configuracaoIntegracao == null || string.IsNullOrWhiteSpace(configuracaoIntegracao.URLIntegracaoTotvs))
            {
                msgRetorno = "Não existe configuração de integração disponível para a TOTVS";
                return false;
            }

            if (IntegrarLancamentoAcrescimoDescontoContratoTerceiro(integracaoContrato, unitOfWork, out msgRetorno))
            {
                if (!IntegrarBaixaAcrescimoDescontoContratoTerceiro(integracaoContrato, unitOfWork, out msgRetorno))
                {
                    msgRetorno = "Falha ao realizar a integração da baixa financeira";
                    return false;
                }
            }
            else
            {
                msgRetorno = "Falha ao realizar a integração do lançamento financeiro";
                return false;
            }
            return true;
        }

        public bool IntegrarCancelamentoAcrescimoDescontoContratoTerceiro(Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoIntegracao integracaoContrato, Repositorio.UnitOfWork unitOfWork, out string msgRetorno)
        {
            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();

            msgRetorno = string.Empty;

            if (configuracaoIntegracao == null || string.IsNullOrWhiteSpace(configuracaoIntegracao.URLIntegracaoTotvs))
            {
                msgRetorno = "Não existe configuração de integração disponível para a TOTVS";
                return false;
            }

            if (string.IsNullOrWhiteSpace(integracaoContrato.ContratoFreteAcrescimoDesconto.CodigoIntegracaoBaixa) && string.IsNullOrWhiteSpace(integracaoContrato.ContratoFreteAcrescimoDesconto.CodigoIntegracaoLancamento))
            {
                msgRetorno = "Contrato não possui codigos de integração realizado anteriormente";
                return true;
            }

            if (IntegrarCancelamentoBaixaAcrescimoDescontoContratoTerceiro(integracaoContrato, unitOfWork, out msgRetorno))
            {
                if (!IntegrarCancelamentoLancamentoAcrescimoDescontoContratoTerceiro(integracaoContrato, unitOfWork, out msgRetorno))
                {
                    msgRetorno = "Falha ao realizar o cancelamento do lançamento financeiro";
                    return false;
                }
                else if (!IntegrarExclusaoLancamentoAcrescimoDescontoContratoTerceiro(integracaoContrato, unitOfWork, out msgRetorno))
                {
                    msgRetorno = "Falha ao realizar a exclusão do lançamento financeiro";
                    return false;
                }
            }
            else
            {
                msgRetorno = "Falha ao realizar o cancelamento da baixa";
                return false;
            }

            return true;
        }


        public bool IntegrarCTe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.Cliente pessoa, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Dominio.Entidades.Veiculo veiculo, string numeroContratoCIOT, string url, string usuario, string senha, string contexto, out string xmlRequest, out string xmlResponse, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("pt-BR");
            xmlRequest = "";
            xmlResponse = "";
            Totvs svcTotvs = new Totvs();

            ServicoTotvs.DataServer.IwsDataServerClient svcDataServer = svcTotvs.ObterDataServerClient(url, usuario, senha);

            Dominio.Entidades.Cliente entrega = cte.Recebedor != null && cte.Recebedor.Cliente != null ? cte.Recebedor.Cliente : cte.Destinatario.Cliente;
            Dominio.Entidades.Cliente coleta = cte.Expedidor != null && cte.Expedidor.Cliente != null ? cte.Expedidor.Cliente : cte.Remetente.Cliente;
            bool coletaEmpresa = coleta != null ? repEmpresa.ContemEmpresaCadastrada(coleta.CPF_CNPJ_SemFormato) : false;
            bool entregaEmpresa = entrega != null ? repEmpresa.ContemEmpresaCadastrada(entrega.CPF_CNPJ_SemFormato) : false;

            Repositorio.Embarcador.Contabeis.CodigoIntegracaoCFOPCST repCodigoIntegracaoCFOPCST = new Repositorio.Embarcador.Contabeis.CodigoIntegracaoCFOPCST(unitOfWork);
            Dominio.Entidades.Embarcador.Contabeis.CodigoIntegracaoCFOPCST codigoIntegracaoCFOPCST = repCodigoIntegracaoCFOPCST.BuscarCodigoIntegracaoCFOPCST(cte.CST, cte.CFOP?.CodigoCFOP.ToString("D") ?? "");
            string idnat = codigoIntegracaoCFOPCST?.CodigoIntegracao ?? "";

            xmlRequest = @"<MovMovimento>
						<TMOV>
							<CODCOLIGADA>" + (string.IsNullOrEmpty(pessoa.CodigoCompanhia) ? "10" : pessoa.CodigoCompanhia) + @"</CODCOLIGADA>
							<IDMOV>-1</IDMOV>
							<CODFILIAL>" + cte.Empresa.CodigoIntegracao + @"</CODFILIAL>
							<CODLOC>" + cte.Empresa.CodigoCentroCusto + @"</CODLOC>
							<CODCFO>" + pessoa.CodigoIntegracao + @"</CODCFO>
							<NUMEROMOV>" + cte.Numero.ToString("D") + @"</NUMEROMOV>
							<SERIE>" + cte.Serie.Numero.ToString("D") + @"</SERIE>
							<CODTMV>2.2.14</CODTMV>
							<TIPO>S</TIPO>
							<STATUS>R</STATUS>
							<MOVIMPRESSO>0</MOVIMPRESSO>
							<DOCIMPRESSO>0</DOCIMPRESSO>
							<FATIMPRESSA>0</FATIMPRESSA>
							<DATAEMISSAO>" + cte.DataEmissao.Value.ToString("yyyy-MM-ddTHH:mm:ss") + @"</DATAEMISSAO>
							<DATASAIDA>" + cte.DataEmissao.Value.ToString("yyyy-MM-ddTHH:mm:ss") + @"</DATASAIDA>
							<COMISSAOREPRES>0.0000</COMISSAOREPRES>
							<CODCPG>" + tipoOperacao.CodigoCondicaoPagamento + @"</CODCPG>
							<VALORBRUTO>" + cte.ValorAReceber.ToString("n2").Replace(".", "") + @"</VALORBRUTO>
							<VALORLIQUIDO>" + cte.ValorAReceber.ToString("n2").Replace(".", "") + @"</VALORLIQUIDO>
							<VALOROUTROS>0.0000</VALOROUTROS>
							<PERCENTUALDESC>0.0000</PERCENTUALDESC>
							<VALORDESC>0.0000</VALORDESC>
							<PERCENTUALDESP>0.0000</PERCENTUALDESP>
							<VALORDESP>0.0000</VALORDESP>
							<PERCCOMISSAO>0.0000</PERCCOMISSAO>
							<PESOLIQUIDO>0.0000</PESOLIQUIDO>
							<PESOBRUTO>0.0000</PESOBRUTO>
							<CODMOEVALORLIQUIDO>R$</CODMOEVALORLIQUIDO>
							<DATAMOVIMENTO>" + cte.DataEmissao.Value.ToString("yyyy-MM-ddTHH:mm:ss") + @"</DATAMOVIMENTO>
							<CODCCUSTO>" + (tipoOperacao?.CodigoIntegracao ?? "") + @"</CODCCUSTO>
							<CODCXA>" + (pessoa.CodigoIntegracaoDadosBancarios) + @"</CODCXA>
							<CODCOLCFO>" + pessoa.CodigoCompanhia + @"</CODCOLCFO>
							<CODUSUARIO>" + usuario + @"</CODUSUARIO>
							<CODCOLCXA>10</CODCOLCXA>
							<GERADOPORLOTE>0</GERADOPORLOTE>
							<HORULTIMAALTERACAO>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + @"</HORULTIMAALTERACAO>
							<HORARIOEMISSAO>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + @"</HORARIOEMISSAO>
							<USUARIOCRIACAO>" + usuario + @"</USUARIOCRIACAO>
							<DATACRIACAO>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + @"</DATACRIACAO>
							<VALORBRUTOINTERNO>" + cte.ValorAReceber.ToString("n2").Replace(".", "") + @"</VALORBRUTOINTERNO>
							<CODTDO>CT-e</CODTDO>
							<INTEGRAAPLICACAO>T</INTEGRAAPLICACAO>
							<DATALANCAMENTO>" + cte.DataEmissao.Value.ToString("yyyy-MM-ddTHH:mm:ss") + @"</DATALANCAMENTO>
							<USARATEIOVALORFIN>1</USARATEIOVALORFIN>
							<VALORRATEIOLAN>" + cte.ValorAReceber.ToString("n2").Replace(".", "") + @"</VALORRATEIOLAN>
							<RATEIOCCUSTODEPTO>" + cte.ValorAReceber.ToString("n2").Replace(".", "") + @"</RATEIOCCUSTODEPTO>
							<VALORBRUTOORIG>" + cte.ValorAReceber.ToString("n2").Replace(".", "") + @"</VALORBRUTOORIG>
							<VALORLIQUIDOORIG>" + cte.ValorAReceber.ToString("n2").Replace(".", "") + @"</VALORLIQUIDOORIG>
							<VALOROUTROSORIG>0.0000</VALOROUTROSORIG>
							<VALORRATEIOLANORIG>" + cte.ValorAReceber.ToString("n2").Replace(".", "") + @"</VALORRATEIOLANORIG>
							<FLAGCONCLUSAO>0</FLAGCONCLUSAO>
							<RECCREATEDBY>" + usuario + @"</RECCREATEDBY>
							<RECCREATEDON>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + @"</RECCREATEDON>
							<RECMODIFIEDBY>" + usuario + @"</RECMODIFIEDBY>
							<RECMODIFIEDON>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + @"</RECMODIFIEDON>
							<PLACA>" + veiculo?.Placa + @"</PLACA>
							<CODETDPLACA>" + veiculo.Estado?.Sigla + @"</CODETDPLACA>
							<PESOLIQUIDO>" + cte.PesoLiquido.ToString("n2").Replace(".", "") + @"</PESOLIQUIDO>
							<PESOBRUTO>" + cte.Peso.ToString("n2").Replace(".", "") + @"</PESOBRUTO>
							<FRETECIFOUFOB>9</FRETECIFOUFOB>
							<CHAVEACESSONFE>" + cte.Chave + @"</CHAVEACESSONFE>
							<IDNAT>" + idnat + @"</IDNAT>
						</TMOV>
						<TITMMOV>
							<CODCOLIGADA>10</CODCOLIGADA>
							<IDMOV>-1</IDMOV>
							<NSEQITMMOV>1</NSEQITMMOV>
							<CODFILIAL>" + cte.Empresa.CodigoIntegracao + @"</CODFILIAL>
							<NUMEROSEQUENCIAL>1</NUMEROSEQUENCIAL>
							<IDPRD>99218</IDPRD>
							<QUANTIDADE>1,0000</QUANTIDADE>
							<PRECOUNITARIO>" + cte.ValorAReceber.ToString("n2").Replace(".", "") + @"</PRECOUNITARIO>
							<DATAEMISSAO>" + cte.DataEmissao.Value.ToString("yyyy-MM-ddTHH:mm:ss") + @"</DATAEMISSAO>
							<CODTB1FAT>034</CODTB1FAT>
							<CODTB2FAT>1.01.001</CODTB2FAT>
							<CODUND>UN</CODUND>
							<QUANTIDADEARECEBER>1,0000</QUANTIDADEARECEBER>
							<VALORUNITARIO>0.0000</VALORUNITARIO>
							<VALORFINANCEIRO>0.0000</VALORFINANCEIRO>
							<CODCCUSTO>" + (tipoOperacao?.CodigoIntegracao ?? "") + @"</CODCCUSTO>
							<QUANTIDADEORIGINAL>1,0000</QUANTIDADEORIGINAL>
							<VALORBRUTOITEM>" + cte.ValorAReceber.ToString("n2").Replace(".", "") + @"</VALORBRUTOITEM>
							<CODTBORCAMENTO>5.01.01.0001</CODTBORCAMENTO>
							<CODCOLTBORCAMENTO>0</CODCOLTBORCAMENTO>
							<CODLOC>" + cte.Empresa.CodigoCentroCusto + @"</CODLOC>
							<VALORLIQUIDO>" + cte.ValorAReceber.ToString("n2").Replace(".", "") + @"</VALORLIQUIDO>
							<RATEIOCCUSTODEPTO>" + cte.ValorAReceber.ToString("n2").Replace(".", "") + @"</RATEIOCCUSTODEPTO>
							<QUANTIDADETOTAL>1,0000</QUANTIDADETOTAL>
							<INTEGRAAPLICACAO>T</INTEGRAAPLICACAO>
							<RECCREATEDBY>" + usuario + @"</RECCREATEDBY>
							<RECCREATEDON>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + @"</RECCREATEDON>
							<RECMODIFIEDBY>" + usuario + @"</RECMODIFIEDBY>
							<RECMODIFIEDON>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + @"</RECMODIFIEDON>
						</TITMMOV>

						<TMOVTRANSP>
							<CODCOLIGADA>10</CODCOLIGADA>
							<IDMOV>-1</IDMOV>
							<CODETDENTREGA>" + cte.LocalidadeTerminoPrestacao.Estado.Sigla + @"</CODETDENTREGA>
							<CODMUNICIPIOENTREGA>" + cte.LocalidadeTerminoPrestacao.CodigoIBGESemUf.ToString("D").PadLeft(5, '0') + @"</CODMUNICIPIOENTREGA>
							<CODETDCOLETA>" + cte.LocalidadeInicioPrestacao.Estado.Sigla  + @"</CODETDCOLETA>
							<CODMUNICIPIOCOLETA>" + cte.LocalidadeInicioPrestacao.CodigoIBGESemUf.ToString("D").PadLeft(5, '0') + @"</CODMUNICIPIOCOLETA>

							<TIPOREMETENTE>" + (coletaEmpresa ? "M" : "C") + @"</TIPOREMETENTE>
							<REMETENTECODCOLCFO>10</REMETENTECODCOLCFO>							
							" + (coletaEmpresa ? "<REMETENTECODCOLCFO>10</REMETENTECODCOLCFO>" : "<REMETENTECODCFO>" + coleta.CodigoIntegracao + @"</REMETENTECODCFO>") + @"
							" + (coletaEmpresa ? "<REMETENTEFILIAL>1</REMETENTEFILIAL>" : "") + @"

							<TIPODESTINATARIO>" + (entregaEmpresa ? "M" : "C") + @"</TIPODESTINATARIO>    
							<DESTINATARIOCODCOLCFO>" + (!string.IsNullOrWhiteSpace(entrega.CodigoCompanhia) ? entrega.CodigoCompanhia : "10") + @"</DESTINATARIOCODCOLCFO>
							<DESTINATARIOCODCFO>" + entrega.CodigoIntegracao + @"</DESTINATARIOCODCFO>

							<CNPJCPFCOLETA>" + coleta.CPF_CNPJ_Formatado + @"</CNPJCPFCOLETA>
							<INSCRESTCOLETA>" + coleta.IE_RG + @"</INSCRESTCOLETA>

							<CNPJCPFENTREGA>" + entrega.CPF_CNPJ_Formatado + @"</CNPJCPFENTREGA>
							<INSCRESTENTREGA>" + entrega.IE_RG + @"</INSCRESTENTREGA>
    
							<RUACOLETA>" + coleta.Endereco + @"</RUACOLETA>
							<NUMEROCOLETA>" + coleta.Numero + @"</NUMEROCOLETA>
							<COMPLCOLETA>" + coleta.Complemento + @"</COMPLCOLETA>
							<BAIRROCOLETA>" + coleta.Bairro + @"</BAIRROCOLETA>

							<RUAENTREGA>" + entrega.Endereco + @"</RUAENTREGA>
							<NUMEROENTREGA>" + entrega.Numero + @"</NUMEROENTREGA>
							<COMPLENTREGA>" + entrega.Complemento + @"</COMPLENTREGA>
							<BAIRROENTREGA>" + entrega.Bairro + @"</BAIRROENTREGA>

							<RETIRAMERCADORIA>0</RETIRAMERCADORIA>
							<TIPOCTE>0</TIPOCTE>
							<TOMADORTIPO>0</TOMADORTIPO>
							<TIPOEMITENTEMDFE>0</TIPOEMITENTEMDFE>
							<LOTACAO>1</LOTACAO>
							<TIPOTRANSPORTADORMDFE>0</TIPOTRANSPORTADORMDFE>
							<TIPOBPE>0</TIPOBPE>

							<RECCREATEDBY>" + usuario + @"</RECCREATEDBY>
							<RECCREATEDON>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + @"</RECCREATEDON>
							<RECMODIFIEDBY>" + usuario + @"</RECMODIFIEDBY>
							<RECMODIFIEDON>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + @"</RECMODIFIEDON>
						  </TMOVTRANSP>

						<TTRBITMMOV>
							<CODCOLIGADA>10</CODCOLIGADA>
							<IDMOV>-1</IDMOV>
							<NSEQITMMOV>1</NSEQITMMOV>
							<CODTRB>ICMS</CODTRB>
							<BASEDECALCULO>" + cte.BaseCalculoICMS.ToString("n2").Replace(".", "") + @"</BASEDECALCULO>
							<ALIQUOTA>" + cte.AliquotaICMS.ToString("n2").Replace(".", "") + @"</ALIQUOTA>
							<VALOR>" + cte.ValorICMS.ToString("n2").Replace(".", "") + @"</VALOR>
							<EDITADO>1</EDITADO>
							<RECCREATEDBY>" + usuario + @"</RECCREATEDBY>
							<RECCREATEDON>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + @"</RECCREATEDON>
							<RECMODIFIEDBY>" + usuario + @"</RECMODIFIEDBY>
							<RECMODIFIEDON>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + @"</RECMODIFIEDON>
						  </TTRBITMMOV>

					</MovMovimento>";

            try
            {
                using (OperationContextScope scope = new OperationContextScope(svcDataServer.InnerChannel))
                {
                    var httpRequestProperty = new HttpRequestMessageProperty();
                    httpRequestProperty.Headers[System.Net.HttpRequestHeader.Authorization] = "Basic " +
                                 Convert.ToBase64String(Encoding.ASCII.GetBytes(svcDataServer.ClientCredentials.UserName.UserName + ":" +
                                 svcDataServer.ClientCredentials.UserName.Password));
                    OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = httpRequestProperty;

                    //xmlResponse = svcDataServer.SaveRecord("FinCFODataBR", xmlRequest, "CODCOLIGADA=10;CODUSUARIO=multisoftware;CODSISTEMA=G");
                    xmlResponse = svcDataServer.SaveRecord("MOVMOVIMENTOTBCDATA", xmlRequest, contexto);
                    xmlResponse = xmlResponse.Trim();
                    
                    if (xmlResponse.Contains("Já Existe Nota do Emitente com este Número e Série"))
                    {
                        // Se já tem código de integração, considera sucesso
                        if (!string.IsNullOrEmpty(cte.CodigoIntegracao) && !string.IsNullOrEmpty(cte.CodigoCompanhia))
                        {
                            Servicos.Log.TratarErro($"CTE ({cte.Codigo}) já existente no TOTVS, tratado como sucesso: {cte.CodigoIntegracao} - {cte.CodigoCompanhia}", "IntegracaoTOTVS");
                            return true;
                        }
                        return false; // Se não tiver código, trata como erro

                    }
                    else if (xmlResponse.Contains("SaveRecord"))
                        return false;
                    else if (!xmlResponse.Contains("SaveRecord") && xmlResponse.Split(';').Count() > 1)
                    {
                        cte.CodigoIntegracao = xmlResponse.Split(';')[1];
                        cte.CodigoCompanhia = xmlResponse.Split(';')[0];
                        repCTe.Atualizar(cte);
                    }
                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao, "IntegracaoTOTVS");
                xmlResponse = excecao.Message;
                return false;
            }

            return true;
        }

        public bool IntegrarNFs(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.Cliente pessoa, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Dominio.Entidades.Veiculo veiculo, string url, string usuario, string senha, string contexto, out string xmlRequest, out string xmlResponse, Repositorio.UnitOfWork unitOfWork)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("pt-BR");
            xmlRequest = "";
            xmlResponse = "";
            Totvs svcTotvs = new Totvs();

            //ServicoTotvs.DataServer.IwsDataServerClient svcDataServer = svcTotvs.ObterDataServerClient("http://177.55.191.130:8052", "multisoftware", "TMTLog1!");
            ServicoTotvs.DataServer.IwsDataServerClient svcDataServer = svcTotvs.ObterDataServerClient(url, usuario, senha);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.Embarcador.Contabeis.CodigoIntegracaoCFOPCST repCodigoIntegracaoCFOPCST = new Repositorio.Embarcador.Contabeis.CodigoIntegracaoCFOPCST(unitOfWork);
            Dominio.Entidades.Embarcador.Contabeis.CodigoIntegracaoCFOPCST codigoIntegracaoCFOPCST = repCodigoIntegracaoCFOPCST.BuscarCodigoIntegracaoCFOPCST(cte.CST, cte.CFOP?.CodigoCFOP.ToString("D") ?? "");
            string idnat = codigoIntegracaoCFOPCST?.CodigoIntegracao ?? "";

            xmlRequest = @"<MovMovimento>
							<TMOV>
								<CODCOLIGADA>" + (string.IsNullOrEmpty(pessoa.CodigoCompanhia) ? "10" : pessoa.CodigoCompanhia) + @"</CODCOLIGADA>
								<IDMOV>-1</IDMOV>
								<CODFILIAL>" + cte.Empresa.CodigoIntegracao + @"</CODFILIAL>
								<CODLOC>" + cte.Empresa.CodigoCentroCusto + @"</CODLOC>
								<CODCFO>" + pessoa.CodigoIntegracao + @"</CODCFO>
								<NUMEROMOV>" + cte.Numero.ToString("D") + @"</NUMEROMOV>
								<SERIE>NFES</SERIE>
								<CODTMV>2.2.01</CODTMV>
								<TIPO>A</TIPO>
								<STATUS>R</STATUS>
								<MOVIMPRESSO>0</MOVIMPRESSO>
								<DOCIMPRESSO>0</DOCIMPRESSO>
								<FATIMPRESSA>0</FATIMPRESSA>
								<DATAEMISSAO>" + cte.DataEmissao.Value.ToString("yyyy-MM-ddTHH:mm:ss") + @"</DATAEMISSAO>
								<DATASAIDA>" + cte.DataEmissao.Value.ToString("yyyy-MM-ddTHH:mm:ss") + @"</DATASAIDA>
								<COMISSAOREPRES>0.0000</COMISSAOREPRES>
								<CODCPG>" + tipoOperacao.CodigoCondicaoPagamento + @"</CODCPG>
								<VALORBRUTO>" + cte.ValorAReceber.ToString("n2").Replace(".", "") + @"</VALORBRUTO>
								<VALORLIQUIDO>" + cte.ValorAReceber.ToString("n2").Replace(".", "") + @"</VALORLIQUIDO>
								<VALOROUTROS>0.0000</VALOROUTROS>
								<PERCENTUALDESC>0.0000</PERCENTUALDESC>
								<VALORDESC>0.0000</VALORDESC>
								<PERCENTUALDESP>0.0000</PERCENTUALDESP>
								<VALORDESP>0.0000</VALORDESP>
								<PERCCOMISSAO>0.0000</PERCCOMISSAO>
								<PESOLIQUIDO>0.0000</PESOLIQUIDO>
								<PESOBRUTO>0.0000</PESOBRUTO>
								<CODMOEVALORLIQUIDO>R$</CODMOEVALORLIQUIDO>
								<DATAMOVIMENTO>" + cte.DataEmissao.Value.ToString("yyyy-MM-ddTHH:mm:ss") + @"</DATAMOVIMENTO>
								<CODCCUSTO>" + (tipoOperacao?.CodigoIntegracao ?? "") + @"</CODCCUSTO>
								<CODCXA>" + (pessoa.CodigoIntegracaoDadosBancarios) + @"</CODCXA>
								<CODCOLCFO>" + pessoa.CodigoCompanhia + @"</CODCOLCFO>
								<CODUSUARIO>" + usuario + @"</CODUSUARIO>
								<CODCOLCXA>10</CODCOLCXA>
								<GERADOPORLOTE>0</GERADOPORLOTE>
								<HORULTIMAALTERACAO>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + @"</HORULTIMAALTERACAO>
								<HORARIOEMISSAO>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + @"</HORARIOEMISSAO>
								<USUARIOCRIACAO>" + usuario + @"</USUARIOCRIACAO>
								<DATACRIACAO>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + @"</DATACRIACAO>
								<VALORBRUTOINTERNO>" + cte.ValorAReceber.ToString("n2").Replace(".", "") + @"</VALORBRUTOINTERNO>
								<CODTDO>75</CODTDO>
								<INTEGRAAPLICACAO>T</INTEGRAAPLICACAO>
								<DATALANCAMENTO>" + cte.DataEmissao.Value.ToString("yyyy-MM-ddTHH:mm:ss") + @"</DATALANCAMENTO>
								<USARATEIOVALORFIN>1</USARATEIOVALORFIN>
								<VALORRATEIOLAN>" + cte.ValorAReceber.ToString("n2").Replace(".", "") + @"</VALORRATEIOLAN>
								<RATEIOCCUSTODEPTO>" + cte.ValorAReceber.ToString("n2").Replace(".", "") + @"</RATEIOCCUSTODEPTO>
								<VALORBRUTOORIG>" + cte.ValorAReceber.ToString("n2").Replace(".", "") + @"</VALORBRUTOORIG>
								<VALORLIQUIDOORIG>" + cte.ValorAReceber.ToString("n2").Replace(".", "") + @"</VALORLIQUIDOORIG>
								<VALOROUTROSORIG>0.0000</VALOROUTROSORIG>
								<VALORRATEIOLANORIG>" + cte.ValorAReceber.ToString("n2").Replace(".", "") + @"</VALORRATEIOLANORIG>
								<FLAGCONCLUSAO>0</FLAGCONCLUSAO>
								<RECCREATEDBY>" + usuario + @"</RECCREATEDBY>
								<RECCREATEDON>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + @"</RECCREATEDON>
								<RECMODIFIEDBY>" + usuario + @"</RECMODIFIEDBY>
								<RECMODIFIEDON>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + @"</RECMODIFIEDON>
								<CHAVEACESSONFE>" + cte.Codigo.ToString("D") + @"</CHAVEACESSONFE>
								<IDNAT>" + idnat + @"</IDNAT>
							</TMOV>
							<TITMMOV>
								<CODCOLIGADA>10</CODCOLIGADA>
								<IDMOV>-1</IDMOV>
								<NSEQITMMOV>1</NSEQITMMOV>
								<CODFILIAL>" + cte.Empresa.CodigoIntegracao + @"</CODFILIAL>
								<NUMEROSEQUENCIAL>1</NUMEROSEQUENCIAL>
								<IDPRD>100502</IDPRD>
								<QUANTIDADE>1,0000</QUANTIDADE>
								<PRECOUNITARIO>" + cte.ValorAReceber.ToString("n2").Replace(".", "") + @"000000</PRECOUNITARIO>
								<DATAEMISSAO>" + cte.DataEmissao.Value.ToString("yyyy-MM-ddTHH:mm:ss") + @"</DATAEMISSAO>
								<CODTB1FAT>034</CODTB1FAT>
								<CODTB2FAT>1.01.001</CODTB2FAT>
								<CODUND>UN</CODUND>
								<QUANTIDADEARECEBER>1,0000</QUANTIDADEARECEBER>
								<VALORUNITARIO>0.0000</VALORUNITARIO>
								<VALORFINANCEIRO>0.0000</VALORFINANCEIRO>
								<CODCCUSTO>" + (tipoOperacao?.CodigoIntegracao ?? "") + @"</CODCCUSTO>
								<QUANTIDADEORIGINAL>1,0000</QUANTIDADEORIGINAL>
								<VALORBRUTOITEM>" + cte.ValorAReceber.ToString("n2").Replace(".", "") + @"000000</VALORBRUTOITEM>
								<CODTBORCAMENTO>5.01.01.0001</CODTBORCAMENTO>
								<CODCOLTBORCAMENTO>0</CODCOLTBORCAMENTO>
								<CODLOC>" + cte.Empresa.CodigoCentroCusto + @"</CODLOC>
								<VALORLIQUIDO>" + cte.ValorAReceber.ToString("n2").Replace(".", "") + @"</VALORLIQUIDO>
								<RATEIOCCUSTODEPTO>" + cte.ValorAReceber.ToString("n2").Replace(".", "") + @"</RATEIOCCUSTODEPTO>
								<QUANTIDADETOTAL>1,0000</QUANTIDADETOTAL>
								<INTEGRAAPLICACAO>T</INTEGRAAPLICACAO>
								<RECCREATEDBY>" + usuario + @"</RECCREATEDBY>
								<RECCREATEDON>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + @"</RECCREATEDON>
								<RECMODIFIEDBY>" + usuario + @"</RECMODIFIEDBY>
								<RECMODIFIEDON>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + @"</RECMODIFIEDON>
							</TITMMOV>
							<TMOVTRANSP>
								<CODCOLIGADA>10</CODCOLIGADA>
								<IDMOV>-1</IDMOV>
								<RETIRAMERCADORIA>0</RETIRAMERCADORIA>
								<TIPOCTE>0</TIPOCTE>
								<TOMADORTIPO>0</TOMADORTIPO>
								<TIPOEMITENTEMDFE>0</TIPOEMITENTEMDFE>
								<LOTACAO>1</LOTACAO>
								<TIPOTRANSPORTADORMDFE>0</TIPOTRANSPORTADORMDFE>
								<TIPOBPE>0</TIPOBPE>
								<RECCREATEDBY>" + usuario + @"</RECCREATEDBY>
								<RECCREATEDON>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + @"</RECCREATEDON>
								<RECMODIFIEDBY>" + usuario + @"</RECMODIFIEDBY>
								<RECMODIFIEDON>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + @"</RECMODIFIEDON>
							</TMOVTRANSP>
						</MovMovimento>";

            try
            {
                using (OperationContextScope scope = new OperationContextScope(svcDataServer.InnerChannel))
                {
                    var httpRequestProperty = new HttpRequestMessageProperty();
                    httpRequestProperty.Headers[System.Net.HttpRequestHeader.Authorization] = "Basic " +
                                 Convert.ToBase64String(Encoding.ASCII.GetBytes(svcDataServer.ClientCredentials.UserName.UserName + ":" +
                                 svcDataServer.ClientCredentials.UserName.Password));
                    OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = httpRequestProperty;

                    //xmlResponse = svcDataServer.SaveRecord("FinCFODataBR", xmlRequest, "CODCOLIGADA=10;CODUSUARIO=multisoftware;CODSISTEMA=G");
                    xmlResponse = svcDataServer.SaveRecord("MOVMOVIMENTOTBCDATA", xmlRequest, contexto);
                    if (xmlResponse.Contains("SaveRecord"))
                        return false;
                    else if (!xmlResponse.Contains("SaveRecord") && xmlResponse.Split(';').Count() > 1)
                    {
                        cte.CodigoIntegracao = xmlResponse.Split(';')[1];
                        cte.CodigoCompanhia = xmlResponse.Split(';')[0];
                        repCTe.Atualizar(cte);
                    }
                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao, "IntegracaoTOTVS");
                xmlResponse = excecao.Message;
                return false;
            }

            return true;
        }

        public bool IntegrarContratoTerceiro(Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contrato, Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.Cliente pessoa, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, string url, string usuario, string senha, string contexto, out string xmlRequest, out string xmlResponse, Repositorio.UnitOfWork unitOfWork)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("pt-BR");
            xmlRequest = "";
            xmlResponse = "";
            Totvs svcTotvs = new Totvs();

            ServicoTotvs.DataServer.IwsDataServerClient svcDataServer = svcTotvs.ObterDataServerClient(url, usuario, senha);

            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);
            Repositorio.Embarcador.Contabeis.CodigoIntegracaoCFOPCST repCodigoIntegracaoCFOPCST = new Repositorio.Embarcador.Contabeis.CodigoIntegracaoCFOPCST(unitOfWork);
            Dominio.Entidades.Embarcador.Contabeis.CodigoIntegracaoCFOPCST codigoIntegracaoCFOPCST = repCodigoIntegracaoCFOPCST.BuscarCodigoIntegracaoCFOPCST(cte.CST, cte.CFOP?.CodigoCFOP.ToString("D") ?? "");
            string idnat = codigoIntegracaoCFOPCST?.CodigoIntegracao ?? "";

            xmlRequest = @"<MovMovimento>
						<TMOV>
							<CODCOLIGADA>10</CODCOLIGADA>
							<IDMOV>-1</IDMOV>
							<CODFILIAL>" + cte.Empresa.CodigoIntegracao + @"</CODFILIAL>
							<CODLOC>" + cte.Empresa.CodigoCentroCusto + @"</CODLOC>
							<CODCFO>" + pessoa.CodigoIntegracao + @"</CODCFO>
							<NUMEROMOV>" + contrato.NumeroContrato.ToString("D") + @"</NUMEROMOV>
							<SERIE>RPA</SERIE>
							<CODTMV>1.2.48</CODTMV>
							<TIPO>S</TIPO>
							<STATUS>R</STATUS>
							<MOVIMPRESSO>0</MOVIMPRESSO>
							<DOCIMPRESSO>0</DOCIMPRESSO>
							<FATIMPRESSA>0</FATIMPRESSA>
							<DATAEMISSAO>" + contrato.DataEmissaoContrato.ToString("yyyy-MM-ddTHH:mm:ss") + @"</DATAEMISSAO>
							<DATASAIDA>" + contrato.DataEmissaoContrato.ToString("yyyy-MM-ddTHH:mm:ss") + @"</DATASAIDA>
							<COMISSAOREPRES>0.0000</COMISSAOREPRES>
							<CODCPG>" + tipoOperacao.CodigoCondicaoPagamento + @"</CODCPG>
							<VALORBRUTO>" + contrato.ValorFreteSubcontratacao.ToString("n2").Replace(".", "") + @"</VALORBRUTO>
							<VALORLIQUIDO>" + contrato.ValorFreteSubcontratacao.ToString("n2").Replace(".", "") + @"</VALORLIQUIDO>
							<VALOROUTROS>0.0000</VALOROUTROS>
							<PERCENTUALDESC>0.0000</PERCENTUALDESC>
							<VALORDESC>0.0000</VALORDESC>
							<PERCENTUALDESP>0.0000</PERCENTUALDESP>
							<VALORDESP>0.0000</VALORDESP>
							<PERCCOMISSAO>0.0000</PERCCOMISSAO>
							<PESOLIQUIDO>0.0000</PESOLIQUIDO>
							<PESOBRUTO>0.0000</PESOBRUTO>
							<CODMOEVALORLIQUIDO>R$</CODMOEVALORLIQUIDO>
							<DATAMOVIMENTO>" + contrato.DataEmissaoContrato.ToString("yyyy-MM-ddTHH:mm:ss") + @"</DATAMOVIMENTO>
							<CODCCUSTO>" + (tipoOperacao?.CodigoIntegracao ?? "") + @"</CODCCUSTO>
							<CODCXA>" + (pessoa.CodigoIntegracaoDadosBancarios) + @"</CODCXA>
							<CODCOLCFO>" + pessoa.CodigoCompanhia + @"</CODCOLCFO>
							<CODUSUARIO>" + usuario + @"</CODUSUARIO>
							<CODCOLCXA>10</CODCOLCXA>
							<HORULTIMAALTERACAO>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + @"</HORULTIMAALTERACAO>
							<HORARIOEMISSAO>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + @"</HORARIOEMISSAO>
							<USUARIOCRIACAO>" + usuario + @"</USUARIOCRIACAO>
							<DATACRIACAO>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + @"</DATACRIACAO>
							<VALORBRUTOINTERNO>" + contrato.ValorFreteSubcontratacao.ToString("n2").Replace(".", "") + @"</VALORBRUTOINTERNO>
							<CODTDO>RPA-T</CODTDO>
							<PERCENTBASEINSS>20.0000</PERCENTBASEINSS>
							<PERCBASEINSSEMPREGADO>20.0000</PERCBASEINSSEMPREGADO>
							<VALORSERVICO>" + contrato.ValorFreteSubcontratacao.ToString("n2").Replace(".", "") + @"</VALORSERVICO>
							<INTEGRAAPLICACAO>T</INTEGRAAPLICACAO>
							<DATALANCAMENTO>" + contrato.DataEmissaoContrato.ToString("yyyy-MM-ddTHH:mm:ss") + @"</DATALANCAMENTO>
							<USARATEIOVALORFIN>1</USARATEIOVALORFIN>
							<VALORRATEIOLAN>" + contrato.ValorFreteSubcontratacao.ToString("n2").Replace(".", "") + @"</VALORRATEIOLAN>
							<RATEIOCCUSTODEPTO>" + contrato.ValorFreteSubcontratacao.ToString("n2").Replace(".", "") + @"</RATEIOCCUSTODEPTO>
							<VALORBRUTOORIG>" + contrato.ValorFreteSubcontratacao.ToString("n2").Replace(".", "") + @"</VALORBRUTOORIG>
							<VALORLIQUIDOORIG>" + contrato.ValorFreteSubcontratacao.ToString("n2").Replace(".", "") + @"</VALORLIQUIDOORIG>
							<VALOROUTROSORIG>0.0000</VALOROUTROSORIG>
							<VALORRATEIOLANORIG>" + contrato.ValorFreteSubcontratacao.ToString("n2").Replace(".", "") + @"</VALORRATEIOLANORIG>
							<FLAGCONCLUSAO>0</FLAGCONCLUSAO>
							<RECCREATEDBY>" + usuario + @"</RECCREATEDBY>
							<RECCREATEDON>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + @"</RECCREATEDON>
							<RECMODIFIEDBY>" + usuario + @"</RECMODIFIEDBY>
							<RECMODIFIEDON>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + @"</RECMODIFIEDON>							
						</TMOV>
						<TITMMOV>
							<CODCOLIGADA>10</CODCOLIGADA>
							<IDMOV>-1</IDMOV>
							<NSEQITMMOV>1</NSEQITMMOV>
							<CODFILIAL>" + cte.Empresa.CodigoIntegracao + @"</CODFILIAL>
							<NUMEROSEQUENCIAL>1</NUMEROSEQUENCIAL>
							<IDPRD>99498</IDPRD>
							<QUANTIDADE>1,0000</QUANTIDADE>
							<PRECOUNITARIO>" + contrato.ValorFreteSubcontratacao.ToString("n2").Replace(".", "") + @"</PRECOUNITARIO>
							<DATAEMISSAO>" + contrato.DataEmissaoContrato.ToString("yyyy-MM-ddTHH:mm:ss") + @"</DATAEMISSAO>
							<CODTB1FAT>004</CODTB1FAT>
							<CODTB2FAT>2.03.001</CODTB2FAT>
							<CODUND>UN</CODUND>
							<QUANTIDADEARECEBER>1,0000</QUANTIDADEARECEBER>
							<VALORUNITARIO>0.0000</VALORUNITARIO>
							<VALORFINANCEIRO>0.0000</VALORFINANCEIRO>
							<CODCCUSTO>" + (tipoOperacao?.CodigoIntegracao ?? "") + @"</CODCCUSTO>
							<QUANTIDADEORIGINAL>1,0000</QUANTIDADEORIGINAL>
							<VALORBRUTOITEM>" + contrato.ValorFreteSubcontratacao.ToString("n2").Replace(".", "") + @"</VALORBRUTOITEM>
							<CODTBORCAMENTO>3.55.01.0006</CODTBORCAMENTO>
							<CODCOLTBORCAMENTO>0</CODCOLTBORCAMENTO>
							<CODLOC>" + cte.Empresa.CodigoCentroCusto + @"</CODLOC>
							<VALORLIQUIDO>" + contrato.ValorFreteSubcontratacao.ToString("n2").Replace(".", "") + @"</VALORLIQUIDO>
							<RATEIOCCUSTODEPTO>" + contrato.ValorFreteSubcontratacao.ToString("n2").Replace(".", "") + @"</RATEIOCCUSTODEPTO>
							<QUANTIDADETOTAL>1,0000</QUANTIDADETOTAL>
							<INTEGRAAPLICACAO>T</INTEGRAAPLICACAO>
							<RECCREATEDBY>" + usuario + @"</RECCREATEDBY>
							<RECCREATEDON>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + @"</RECCREATEDON>
							<RECMODIFIEDBY>" + usuario + @"</RECMODIFIEDBY>
							<RECMODIFIEDON>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + @"</RECMODIFIEDON>
						</TITMMOV>
					</MovMovimento>";

            try
            {
                using (OperationContextScope scope = new OperationContextScope(svcDataServer.InnerChannel))
                {
                    var httpRequestProperty = new HttpRequestMessageProperty();
                    httpRequestProperty.Headers[System.Net.HttpRequestHeader.Authorization] = "Basic " +
                                 Convert.ToBase64String(Encoding.ASCII.GetBytes(svcDataServer.ClientCredentials.UserName.UserName + ":" +
                                 svcDataServer.ClientCredentials.UserName.Password));
                    OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = httpRequestProperty;

                    //xmlResponse = svcDataServer.SaveRecord("FinCFODataBR", xmlRequest, "CODCOLIGADA=10;CODUSUARIO=multisoftware;CODSISTEMA=G");
                    xmlResponse = svcDataServer.SaveRecord("MOVMOVIMENTOTBCDATA", xmlRequest, contexto);
                    if (xmlResponse.Contains("SaveRecord"))
                        return false;
                    else if (!xmlResponse.Contains("SaveRecord") && xmlResponse.Split(';').Count() > 1)
                    {
                        contrato.CodigoIntegracao = xmlResponse.Split(';')[1];
                        contrato.CodigoCompanhia = xmlResponse.Split(';')[0];
                        repContratoFrete.Atualizar(contrato);
                    }
                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao, "IntegracaoTOTVS");
                xmlResponse = excecao.Message;
                return false;
            }

            return true;
        }

        public bool IntegrarCancelamentoCTe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.Cliente pessoa, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Dominio.Entidades.Veiculo veiculo, string numeroContratoCIOT, string url, string usuario, string senha, string contexto, out string xmlRequest, out string xmlResponse, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("pt-BR");
            xmlRequest = "";
            xmlResponse = "";
            Totvs svcTotvs = new Totvs();

            ServicoTotvs.DataServer.IwsDataServerClient svcDataServer = svcTotvs.ObterDataServerClient(url, usuario, senha);

            Dominio.Entidades.Cliente entrega = cte.Recebedor != null && cte.Recebedor.Cliente != null ? cte.Recebedor.Cliente : cte.Destinatario.Cliente;
            Dominio.Entidades.Cliente coleta = cte.Expedidor != null && cte.Expedidor.Cliente != null ? cte.Expedidor.Cliente : cte.Remetente.Cliente;
            bool coletaEmpresa = coleta != null ? repEmpresa.ContemEmpresaCadastrada(coleta.CPF_CNPJ_SemFormato) : false;
            bool entregaEmpresa = entrega != null ? repEmpresa.ContemEmpresaCadastrada(entrega.CPF_CNPJ_SemFormato) : false;

            Repositorio.Embarcador.Contabeis.CodigoIntegracaoCFOPCST repCodigoIntegracaoCFOPCST = new Repositorio.Embarcador.Contabeis.CodigoIntegracaoCFOPCST(unitOfWork);
            Dominio.Entidades.Embarcador.Contabeis.CodigoIntegracaoCFOPCST codigoIntegracaoCFOPCST = repCodigoIntegracaoCFOPCST.BuscarCodigoIntegracaoCFOPCST(cte.CST, cte.CFOP?.CodigoCFOP.ToString("D") ?? "");
            string idnat = codigoIntegracaoCFOPCST?.CodigoIntegracao ?? "";

            xmlRequest = @"<MovMovimento>
						<TMOV>
							<CODCOLIGADA>10</CODCOLIGADA>
							<IDMOV>" + cte.CodigoIntegracao + @"</IDMOV>
							<CODFILIAL>" + cte.Empresa.CodigoIntegracao + @"</CODFILIAL>
							<CODLOC>" + cte.Empresa.CodigoCentroCusto + @"</CODLOC>
							<CODCFO>" + pessoa.CodigoIntegracao + @"</CODCFO>
							<NUMEROMOV>" + cte.Numero.ToString("D") + @"</NUMEROMOV>
							<SERIE>" + cte.Serie.Numero.ToString("D") + @"</SERIE>
							<CODTMV>2.2.14</CODTMV>
							<TIPO>S</TIPO>
							<STATUS>R</STATUS>
							<MOVIMPRESSO>0</MOVIMPRESSO>
							<DOCIMPRESSO>0</DOCIMPRESSO>
							<FATIMPRESSA>0</FATIMPRESSA>
							<DATAEMISSAO>" + cte.DataEmissao.Value.ToString("yyyy-MM-ddTHH:mm:ss") + @"</DATAEMISSAO>
							<DATASAIDA>" + cte.DataEmissao.Value.ToString("yyyy-MM-ddTHH:mm:ss") + @"</DATASAIDA>
							<COMISSAOREPRES>0.0000</COMISSAOREPRES>
							<CODCPG>" + tipoOperacao.CodigoCondicaoPagamento + @"</CODCPG>
							<VALORBRUTO>" + cte.ValorAReceber.ToString("n2").Replace(".", "") + @"</VALORBRUTO>
							<VALORLIQUIDO>" + cte.ValorAReceber.ToString("n2").Replace(".", "") + @"</VALORLIQUIDO>
							<VALOROUTROS>0.0000</VALOROUTROS>
							<PERCENTUALDESC>0.0000</PERCENTUALDESC>
							<VALORDESC>0.0000</VALORDESC>
							<PERCENTUALDESP>0.0000</PERCENTUALDESP>
							<VALORDESP>0.0000</VALORDESP>
							<PERCCOMISSAO>0.0000</PERCCOMISSAO>
							<PESOLIQUIDO>0.0000</PESOLIQUIDO>
							<PESOBRUTO>0.0000</PESOBRUTO>
							<CODMOEVALORLIQUIDO>R$</CODMOEVALORLIQUIDO>
							<DATAMOVIMENTO>" + cte.DataEmissao.Value.ToString("yyyy-MM-ddTHH:mm:ss") + @"</DATAMOVIMENTO>
							<CODCCUSTO>" + (tipoOperacao?.CodigoIntegracao ?? "") + @"</CODCCUSTO>
							<CODCXA>" + (pessoa.CodigoIntegracaoDadosBancarios) + @"</CODCXA>
							<CODCOLCFO>" + pessoa.CodigoCompanhia + @"</CODCOLCFO>
							<CODUSUARIO>" + usuario + @"</CODUSUARIO>
							<CODCOLCXA>10</CODCOLCXA>
							<GERADOPORLOTE>0</GERADOPORLOTE>
							<HORULTIMAALTERACAO>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + @"</HORULTIMAALTERACAO>
							<HORARIOEMISSAO>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + @"</HORARIOEMISSAO>
							<USUARIOCRIACAO>" + usuario + @"</USUARIOCRIACAO>
							<DATACRIACAO>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + @"</DATACRIACAO>
							<VALORBRUTOINTERNO>" + cte.ValorAReceber.ToString("n2").Replace(".", "") + @"</VALORBRUTOINTERNO>
							<CODTDO>CT-e</CODTDO>
							<INTEGRAAPLICACAO>T</INTEGRAAPLICACAO>
							<DATALANCAMENTO>" + cte.DataEmissao.Value.ToString("yyyy-MM-ddTHH:mm:ss") + @"</DATALANCAMENTO>
							<USARATEIOVALORFIN>1</USARATEIOVALORFIN>
							<VALORRATEIOLAN>" + cte.ValorAReceber.ToString("n2").Replace(".", "") + @"</VALORRATEIOLAN>
							<RATEIOCCUSTODEPTO>" + cte.ValorAReceber.ToString("n2").Replace(".", "") + @"</RATEIOCCUSTODEPTO>
							<VALORBRUTOORIG>" + cte.ValorAReceber.ToString("n2").Replace(".", "") + @"</VALORBRUTOORIG>
							<VALORLIQUIDOORIG>" + cte.ValorAReceber.ToString("n2").Replace(".", "") + @"</VALORLIQUIDOORIG>
							<VALOROUTROSORIG>0.0000</VALOROUTROSORIG>
							<VALORRATEIOLANORIG>" + cte.ValorAReceber.ToString("n2").Replace(".", "") + @"</VALORRATEIOLANORIG>
							<FLAGCONCLUSAO>0</FLAGCONCLUSAO>
							<RECCREATEDBY>" + usuario + @"</RECCREATEDBY>
							<RECCREATEDON>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + @"</RECCREATEDON>
							<RECMODIFIEDBY>" + usuario + @"</RECMODIFIEDBY>
							<RECMODIFIEDON>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + @"</RECMODIFIEDON>
							<PLACA>" + veiculo?.Placa + @"</PLACA>
							<CODETDPLACA>" + veiculo.Estado?.Sigla + @"</CODETDPLACA>
							<PESOLIQUIDO>" + cte.PesoLiquido.ToString("n2").Replace(".", "") + @"</PESOLIQUIDO>
							<PESOBRUTO>" + cte.Peso.ToString("n2").Replace(".", "") + @"</PESOBRUTO>
							<FRETECIFOUFOB>9</FRETECIFOUFOB>
							<CHAVEACESSONFE>" + cte.Chave + @"</CHAVEACESSONFE>
							<IDNAT>" + idnat + @"</IDNAT>
						</TMOV>
						<TITMMOV>
							<CODCOLIGADA>10</CODCOLIGADA>
							<IDMOV>" + cte.CodigoIntegracao + @"</IDMOV>
							<NSEQITMMOV>1</NSEQITMMOV>
							<CODFILIAL>" + cte.Empresa.CodigoIntegracao + @"</CODFILIAL>
							<NUMEROSEQUENCIAL>1</NUMEROSEQUENCIAL>
							<IDPRD>99218</IDPRD>
							<QUANTIDADE>1,0000</QUANTIDADE>
							<PRECOUNITARIO>" + cte.ValorAReceber.ToString("n2").Replace(".", "") + @"</PRECOUNITARIO>
							<DATAEMISSAO>" + cte.DataEmissao.Value.ToString("yyyy-MM-ddTHH:mm:ss") + @"</DATAEMISSAO>
							<CODTB1FAT>034</CODTB1FAT>
							<CODTB2FAT>1.01.001</CODTB2FAT>
							<CODUND>UN</CODUND>
							<QUANTIDADEARECEBER>1,0000</QUANTIDADEARECEBER>
							<VALORUNITARIO>0.0000</VALORUNITARIO>
							<VALORFINANCEIRO>0.0000</VALORFINANCEIRO>
							<CODCCUSTO>" + (tipoOperacao?.CodigoIntegracao ?? "") + @"</CODCCUSTO>
							<QUANTIDADEORIGINAL>1,0000</QUANTIDADEORIGINAL>
							<VALORBRUTOITEM>" + cte.ValorAReceber.ToString("n2").Replace(".", "") + @"</VALORBRUTOITEM>
							<CODTBORCAMENTO>5.01.01.0001</CODTBORCAMENTO>
							<CODCOLTBORCAMENTO>0</CODCOLTBORCAMENTO>
							<CODLOC>" + cte.Empresa.CodigoCentroCusto + @"</CODLOC>
							<VALORLIQUIDO>" + cte.ValorAReceber.ToString("n2").Replace(".", "") + @"</VALORLIQUIDO>
							<RATEIOCCUSTODEPTO>" + cte.ValorAReceber.ToString("n2").Replace(".", "") + @"</RATEIOCCUSTODEPTO>
							<QUANTIDADETOTAL>1,0000</QUANTIDADETOTAL>
							<INTEGRAAPLICACAO>T</INTEGRAAPLICACAO>
							<RECCREATEDBY>" + usuario + @"</RECCREATEDBY>
							<RECCREATEDON>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + @"</RECCREATEDON>
							<RECMODIFIEDBY>" + usuario + @"</RECMODIFIEDBY>
							<RECMODIFIEDON>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + @"</RECMODIFIEDON>
						</TITMMOV>

						<TMOVTRANSP>
							<CODCOLIGADA>10</CODCOLIGADA>
							<IDMOV>" + cte.CodigoIntegracao + @"</IDMOV>
							<CODETDENTREGA>" + cte.LocalidadeTerminoPrestacao.Estado.Sigla + @"</CODETDENTREGA>
							<CODMUNICIPIOENTREGA>" + cte.LocalidadeTerminoPrestacao.CodigoIBGESemUf.ToString("D").PadLeft(5, '0') + @"</CODMUNICIPIOENTREGA>
							<CODETDCOLETA>" + cte.LocalidadeInicioPrestacao.Estado.Sigla + @"</CODETDCOLETA>
							<CODMUNICIPIOCOLETA>" + cte.LocalidadeInicioPrestacao.CodigoIBGESemUf.ToString("D").PadLeft(5, '0') + @"</CODMUNICIPIOCOLETA>

							<TIPOREMETENTE>" + (coletaEmpresa ? "M" : "C") + @"</TIPOREMETENTE>
							<REMETENTECODCOLCFO>10</REMETENTECODCOLCFO>							
							" + (coletaEmpresa ? "<REMETENTECODCOLCFO>10</REMETENTECODCOLCFO>" : "<REMETENTECODCFO>" + coleta.CodigoIntegracao + @"</REMETENTECODCFO>") + @"
							" + (coletaEmpresa ? "<REMETENTEFILIAL>1</REMETENTEFILIAL>" : "") + @"

							<TIPODESTINATARIO>" + (entregaEmpresa ? "M" : "C") + @"</TIPODESTINATARIO>    
							<DESTINATARIOCODCOLCFO>" + (!string.IsNullOrWhiteSpace(entrega.CodigoCompanhia) ? entrega.CodigoCompanhia : "10") + @"</DESTINATARIOCODCOLCFO>
							<DESTINATARIOCODCFO>" + entrega.CodigoIntegracao + @"</DESTINATARIOCODCFO>

							<CNPJCPFCOLETA>" + coleta.CPF_CNPJ_Formatado + @"</CNPJCPFCOLETA>
							<INSCRESTCOLETA>" + coleta.IE_RG + @"</INSCRESTCOLETA>

							<CNPJCPFENTREGA>" + entrega.CPF_CNPJ_Formatado + @"</CNPJCPFENTREGA>
							<INSCRESTENTREGA>" + entrega.IE_RG + @"</INSCRESTENTREGA>
    
							<RUACOLETA>" + coleta.Endereco + @"</RUACOLETA>
							<NUMEROCOLETA>" + coleta.Numero + @"</NUMEROCOLETA>
							<COMPLCOLETA>" + coleta.Complemento + @"</COMPLCOLETA>
							<BAIRROCOLETA>" + coleta.Bairro + @"</BAIRROCOLETA>

							<RUAENTREGA>" + entrega.Endereco + @"</RUAENTREGA>
							<NUMEROENTREGA>" + entrega.Numero + @"</NUMEROENTREGA>
							<COMPLENTREGA>" + entrega.Complemento + @"</COMPLENTREGA>
							<BAIRROENTREGA>" + entrega.Bairro + @"</BAIRROENTREGA>

							<RETIRAMERCADORIA>0</RETIRAMERCADORIA>
							<TIPOCTE>0</TIPOCTE>
							<TOMADORTIPO>0</TOMADORTIPO>
							<TIPOEMITENTEMDFE>0</TIPOEMITENTEMDFE>
							<LOTACAO>1</LOTACAO>
							<TIPOTRANSPORTADORMDFE>0</TIPOTRANSPORTADORMDFE>
							<TIPOBPE>0</TIPOBPE>

							<RECCREATEDBY>" + usuario + @"</RECCREATEDBY>
							<RECCREATEDON>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + @"</RECCREATEDON>
							<RECMODIFIEDBY>" + usuario + @"</RECMODIFIEDBY>
							<RECMODIFIEDON>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + @"</RECMODIFIEDON>
						  </TMOVTRANSP>

						<TTRBITMMOV>
							<CODCOLIGADA>10</CODCOLIGADA>
							<IDMOV>" + cte.CodigoIntegracao + @"</IDMOV>
							<NSEQITMMOV>1</NSEQITMMOV>
							<CODTRB>ICMS</CODTRB>
							<BASEDECALCULO>" + cte.BaseCalculoICMS.ToString("n2").Replace(".", "") + @"</BASEDECALCULO>
							<ALIQUOTA>" + cte.AliquotaICMS.ToString("n2").Replace(".", "") + @"</ALIQUOTA>
							<VALOR>" + cte.ValorICMS.ToString("n2").Replace(".", "") + @"</VALOR>
							<EDITADO>1</EDITADO>
							<RECCREATEDBY>" + usuario + @"</RECCREATEDBY>
							<RECCREATEDON>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + @"</RECCREATEDON>
							<RECMODIFIEDBY>" + usuario + @"</RECMODIFIEDBY>
							<RECMODIFIEDON>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + @"</RECMODIFIEDON>
						  </TTRBITMMOV>

					</MovMovimento>";

            try
            {
                using (OperationContextScope scope = new OperationContextScope(svcDataServer.InnerChannel))
                {
                    var httpRequestProperty = new HttpRequestMessageProperty();
                    httpRequestProperty.Headers[System.Net.HttpRequestHeader.Authorization] = "Basic " +
                                 Convert.ToBase64String(Encoding.ASCII.GetBytes(svcDataServer.ClientCredentials.UserName.UserName + ":" +
                                 svcDataServer.ClientCredentials.UserName.Password));
                    OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = httpRequestProperty;


                    xmlResponse = svcDataServer.SaveRecord("MOVMOVIMENTOTBCDATA", xmlRequest, contexto);
                    if (xmlResponse.Contains("SaveRecord"))
                        return false;
                    else
                        return true;
                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao, "IntegracaoTOTVS");
                xmlResponse = excecao.Message;
                return false;
            }

            return true;
        }

        public bool IntegrarCancelamentoNFs(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.Cliente pessoa, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Dominio.Entidades.Veiculo veiculo, string url, string usuario, string senha, string contexto, out string xmlRequest, out string xmlResponse, Repositorio.UnitOfWork unitOfWork)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("pt-BR");
            xmlRequest = "";
            xmlResponse = "";
            Totvs svcTotvs = new Totvs();

            //ServicoTotvs.DataServer.IwsDataServerClient svcDataServer = svcTotvs.ObterDataServerClient("http://177.55.191.130:8052", "multisoftware", "TMTLog1!");
            ServicoTotvs.DataServer.IwsDataServerClient svcDataServer = svcTotvs.ObterDataServerClient(url, usuario, senha);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.Embarcador.Contabeis.CodigoIntegracaoCFOPCST repCodigoIntegracaoCFOPCST = new Repositorio.Embarcador.Contabeis.CodigoIntegracaoCFOPCST(unitOfWork);
            Dominio.Entidades.Embarcador.Contabeis.CodigoIntegracaoCFOPCST codigoIntegracaoCFOPCST = repCodigoIntegracaoCFOPCST.BuscarCodigoIntegracaoCFOPCST(cte.CST, cte.CFOP?.CodigoCFOP.ToString("D") ?? "");
            string idnat = codigoIntegracaoCFOPCST?.CodigoIntegracao ?? "";

            xmlRequest = @"<MovMovimento>
							<TMOV>
								<CODCOLIGADA>10</CODCOLIGADA>
								<IDMOV>" + cte.CodigoIntegracao + @"</IDMOV>
								<CODFILIAL>" + cte.Empresa.CodigoIntegracao + @"</CODFILIAL>
								<CODLOC>" + cte.Empresa.CodigoCentroCusto + @"</CODLOC>
								<CODCFO>" + pessoa.CodigoIntegracao + @"</CODCFO>
								<NUMEROMOV>" + cte.Numero.ToString("D") + @"</NUMEROMOV>
								<SERIE>NFES</SERIE>
								<CODTMV>2.2.01</CODTMV>
								<TIPO>A</TIPO>
								<STATUS>R</STATUS>
								<MOVIMPRESSO>0</MOVIMPRESSO>
								<DOCIMPRESSO>0</DOCIMPRESSO>
								<FATIMPRESSA>0</FATIMPRESSA>
								<DATAEMISSAO>" + cte.DataEmissao.Value.ToString("yyyy-MM-ddTHH:mm:ss") + @"</DATAEMISSAO>
								<DATASAIDA>" + cte.DataEmissao.Value.ToString("yyyy-MM-ddTHH:mm:ss") + @"</DATASAIDA>
								<COMISSAOREPRES>0.0000</COMISSAOREPRES>
								<CODCPG>" + tipoOperacao.CodigoCondicaoPagamento + @"</CODCPG>
								<VALORBRUTO>" + cte.ValorAReceber.ToString("n2").Replace(".", "") + @"</VALORBRUTO>
								<VALORLIQUIDO>" + cte.ValorAReceber.ToString("n2").Replace(".", "") + @"</VALORLIQUIDO>
								<VALOROUTROS>0.0000</VALOROUTROS>
								<PERCENTUALDESC>0.0000</PERCENTUALDESC>
								<VALORDESC>0.0000</VALORDESC>
								<PERCENTUALDESP>0.0000</PERCENTUALDESP>
								<VALORDESP>0.0000</VALORDESP>
								<PERCCOMISSAO>0.0000</PERCCOMISSAO>
								<PESOLIQUIDO>0.0000</PESOLIQUIDO>
								<PESOBRUTO>0.0000</PESOBRUTO>
								<CODMOEVALORLIQUIDO>R$</CODMOEVALORLIQUIDO>
								<DATAMOVIMENTO>" + cte.DataEmissao.Value.ToString("yyyy-MM-ddTHH:mm:ss") + @"</DATAMOVIMENTO>
								<CODCCUSTO>" + (tipoOperacao?.CodigoIntegracao ?? "") + @"</CODCCUSTO>
								<CODCXA>" + (pessoa.CodigoIntegracaoDadosBancarios) + @"</CODCXA>
								<CODCOLCFO>" + pessoa.CodigoCompanhia + @"</CODCOLCFO>
								<CODUSUARIO>" + usuario + @"</CODUSUARIO>
								<CODCOLCXA>10</CODCOLCXA>
								<GERADOPORLOTE>0</GERADOPORLOTE>
								<HORULTIMAALTERACAO>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + @"</HORULTIMAALTERACAO>
								<HORARIOEMISSAO>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + @"</HORARIOEMISSAO>
								<USUARIOCRIACAO>" + usuario + @"</USUARIOCRIACAO>
								<DATACRIACAO>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + @"</DATACRIACAO>
								<VALORBRUTOINTERNO>" + cte.ValorAReceber.ToString("n2").Replace(".", "") + @"</VALORBRUTOINTERNO>
								<CODTDO>75</CODTDO>
								<INTEGRAAPLICACAO>T</INTEGRAAPLICACAO>
								<DATALANCAMENTO>" + cte.DataEmissao.Value.ToString("yyyy-MM-ddTHH:mm:ss") + @"</DATALANCAMENTO>
								<USARATEIOVALORFIN>1</USARATEIOVALORFIN>
								<VALORRATEIOLAN>" + cte.ValorAReceber.ToString("n2").Replace(".", "") + @"</VALORRATEIOLAN>
								<RATEIOCCUSTODEPTO>" + cte.ValorAReceber.ToString("n2").Replace(".", "") + @"</RATEIOCCUSTODEPTO>
								<VALORBRUTOORIG>" + cte.ValorAReceber.ToString("n2").Replace(".", "") + @"</VALORBRUTOORIG>
								<VALORLIQUIDOORIG>" + cte.ValorAReceber.ToString("n2").Replace(".", "") + @"</VALORLIQUIDOORIG>
								<VALOROUTROSORIG>0.0000</VALOROUTROSORIG>
								<VALORRATEIOLANORIG>" + cte.ValorAReceber.ToString("n2").Replace(".", "") + @"</VALORRATEIOLANORIG>
								<FLAGCONCLUSAO>0</FLAGCONCLUSAO>
								<RECCREATEDBY>" + usuario + @"</RECCREATEDBY>
								<RECCREATEDON>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + @"</RECCREATEDON>
								<RECMODIFIEDBY>" + usuario + @"</RECMODIFIEDBY>
								<RECMODIFIEDON>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + @"</RECMODIFIEDON>
								<CHAVEACESSONFE>" + cte.Codigo.ToString("D") + @"</CHAVEACESSONFE>
								<IDNAT>" + idnat + @"</IDNAT>
							</TMOV>
							<TITMMOV>
								<CODCOLIGADA>10</CODCOLIGADA>
								<IDMOV>" + cte.CodigoIntegracao + @"</IDMOV>
								<NSEQITMMOV>1</NSEQITMMOV>
								<CODFILIAL>" + cte.Empresa.CodigoIntegracao + @"</CODFILIAL>
								<NUMEROSEQUENCIAL>1</NUMEROSEQUENCIAL>
								<IDPRD>100502</IDPRD>
								<QUANTIDADE>1,0000</QUANTIDADE>
								<PRECOUNITARIO>" + cte.ValorAReceber.ToString("n2").Replace(".", "") + @"000000</PRECOUNITARIO>
								<DATAEMISSAO>" + cte.DataEmissao.Value.ToString("yyyy-MM-ddTHH:mm:ss") + @"</DATAEMISSAO>
								<CODTB1FAT>034</CODTB1FAT>
								<CODTB2FAT>1.01.001</CODTB2FAT>
								<CODUND>UN</CODUND>
								<QUANTIDADEARECEBER>1,0000</QUANTIDADEARECEBER>
								<VALORUNITARIO>0.0000</VALORUNITARIO>
								<VALORFINANCEIRO>0.0000</VALORFINANCEIRO>
								<CODCCUSTO>" + (tipoOperacao?.CodigoIntegracao ?? "") + @"</CODCCUSTO>
								<QUANTIDADEORIGINAL>1,0000</QUANTIDADEORIGINAL>
								<VALORBRUTOITEM>" + cte.ValorAReceber.ToString("n2").Replace(".", "") + @"000000</VALORBRUTOITEM>
								<CODTBORCAMENTO>5.01.01.0001</CODTBORCAMENTO>
								<CODCOLTBORCAMENTO>0</CODCOLTBORCAMENTO>
								<CODLOC>" + cte.Empresa.CodigoCentroCusto + @"</CODLOC>
								<VALORLIQUIDO>" + cte.ValorAReceber.ToString("n2").Replace(".", "") + @"</VALORLIQUIDO>
								<RATEIOCCUSTODEPTO>" + cte.ValorAReceber.ToString("n2").Replace(".", "") + @"</RATEIOCCUSTODEPTO>
								<QUANTIDADETOTAL>1,0000</QUANTIDADETOTAL>
								<INTEGRAAPLICACAO>T</INTEGRAAPLICACAO>
								<RECCREATEDBY>" + usuario + @"</RECCREATEDBY>
								<RECCREATEDON>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + @"</RECCREATEDON>
								<RECMODIFIEDBY>" + usuario + @"</RECMODIFIEDBY>
								<RECMODIFIEDON>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + @"</RECMODIFIEDON>
							</TITMMOV>
							<TMOVTRANSP>
								<CODCOLIGADA>10</CODCOLIGADA>
								<IDMOV>" + cte.CodigoIntegracao + @"</IDMOV>
								<RETIRAMERCADORIA>0</RETIRAMERCADORIA>
								<TIPOCTE>0</TIPOCTE>
								<TOMADORTIPO>0</TOMADORTIPO>
								<TIPOEMITENTEMDFE>0</TIPOEMITENTEMDFE>
								<LOTACAO>1</LOTACAO>
								<TIPOTRANSPORTADORMDFE>0</TIPOTRANSPORTADORMDFE>
								<TIPOBPE>0</TIPOBPE>
								<RECCREATEDBY>" + usuario + @"</RECCREATEDBY>
								<RECCREATEDON>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + @"</RECCREATEDON>
								<RECMODIFIEDBY>" + usuario + @"</RECMODIFIEDBY>
								<RECMODIFIEDON>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + @"</RECMODIFIEDON>
							</TMOVTRANSP>
						</MovMovimento>";

            try
            {
                using (OperationContextScope scope = new OperationContextScope(svcDataServer.InnerChannel))
                {
                    var httpRequestProperty = new HttpRequestMessageProperty();
                    httpRequestProperty.Headers[System.Net.HttpRequestHeader.Authorization] = "Basic " +
                                 Convert.ToBase64String(Encoding.ASCII.GetBytes(svcDataServer.ClientCredentials.UserName.UserName + ":" +
                                 svcDataServer.ClientCredentials.UserName.Password));
                    OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = httpRequestProperty;

                    //xmlResponse = svcDataServer.SaveRecord("FinCFODataBR", xmlRequest, "CODCOLIGADA=10;CODUSUARIO=multisoftware;CODSISTEMA=G");
                    xmlResponse = svcDataServer.SaveRecord("MOVMOVIMENTOTBCDATA", xmlRequest, contexto);
                    if (xmlResponse.Contains("SaveRecord"))
                        return false;
                    else
                        return true;
                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao, "IntegracaoTOTVS");
                xmlResponse = excecao.Message;
                return false;
            }

            return true;
        }

        public void IntegrarCancelamento(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cargaCargaIntegracao, Repositorio.UnitOfWork unitOfWork, string mensagem)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao repCargaCargaCancelamentoCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();

            cargaCargaIntegracao.NumeroTentativas += 1;
            cargaCargaIntegracao.DataIntegracao = DateTime.Now;

            bool situacaoIntegracao = false;

            string mensagemErro = string.Empty;
            string xmlRequest = string.Empty;
            string xmlResponse = string.Empty;

            try
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargasCTe = repCargaCTe.BuscarPorCarga(cargaCargaIntegracao.CargaCancelamento.Carga.Codigo);

                foreach (var cargaCTe in cargasCTe)
                {
                    mensagemErro = string.Empty;
                    xmlRequest = string.Empty;
                    xmlResponse = string.Empty;

                    xmlResponse = ConsultarMovimento(cargaCTe.CTe.CodigoIntegracao, cargaCTe.CTe.CodigoCompanhia, configuracaoIntegracao.URLIntegracaoTotvs, configuracaoIntegracao.UsuarioTotvs, configuracaoIntegracao.SenhaTotvs, configuracaoIntegracao.ContextoTotvs, out mensagemErro);
                    Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                    arquivoIntegracao.Data = cargaCargaIntegracao.DataIntegracao;
                    arquivoIntegracao.Mensagem = "Consulta de movimento CT-e: " + cargaCTe.CTe.Numero.ToString("D") + " - " + mensagemErro;
                    arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;

                    arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlRequest, "xml", unitOfWork);
                    arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlResponse, "xml", unitOfWork);

                    repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                    cargaCargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

                    if (string.IsNullOrWhiteSpace(mensagemErro))
                    {
                        mensagemErro = string.Empty;
                        xmlRequest = string.Empty;

                        xmlResponse = ExcluirMovimento(xmlResponse, configuracaoIntegracao.URLIntegracaoTotvs, configuracaoIntegracao.UsuarioTotvs, configuracaoIntegracao.SenhaTotvs, configuracaoIntegracao.ContextoTotvs, out mensagemErro);

                        arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                        arquivoIntegracao.Data = cargaCargaIntegracao.DataIntegracao;
                        arquivoIntegracao.Mensagem = "Exclusão de movimento CT-e: " + cargaCTe.CTe.Numero.ToString("D") + " - " + mensagemErro;
                        arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;

                        arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlRequest, "xml", unitOfWork);
                        arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlResponse, "xml", unitOfWork);

                        repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                        cargaCargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

                        if (string.IsNullOrWhiteSpace(mensagemErro))
                        {
                            situacaoIntegracao = true;
                            mensagemErro = string.Empty;
                        }
                        else
                        {
                            situacaoIntegracao = false;

                            cargaCargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                            cargaCargaIntegracao.ProblemaIntegracao = mensagemErro;
                            repCargaCargaCancelamentoCargaIntegracao.Atualizar(cargaCargaIntegracao);

                            break;
                        }
                    }
                    else
                    {
                        situacaoIntegracao = false;

                        cargaCargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                        cargaCargaIntegracao.ProblemaIntegracao = mensagemErro;
                        repCargaCargaCancelamentoCargaIntegracao.Atualizar(cargaCargaIntegracao);

                        break;
                    }
                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao, "IntegracaoTOTVS");
                Servicos.Log.TratarErro("Request: " + xmlRequest, "IntegracaoTOTVS");
                Servicos.Log.TratarErro("Response: " + xmlResponse, "IntegracaoTOTVS");

                mensagemErro = "Ocorreu uma falha ao comunicar com o Serviço da Totvs.";
                situacaoIntegracao = false;

                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                arquivoIntegracao.Data = cargaCargaIntegracao.DataIntegracao;
                arquivoIntegracao.Mensagem = excecao.Message.Length > 400 ? excecao.Message.Substring(0, 400) : excecao.Message;
                arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
                arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlRequest, "xml", unitOfWork);
                arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlResponse, "xml", unitOfWork);

                repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                cargaCargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

                cargaCargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaCargaIntegracao.ProblemaIntegracao = mensagemErro;
                repCargaCargaCancelamentoCargaIntegracao.Atualizar(cargaCargaIntegracao);
                return;
            }

            if (!situacaoIntegracao)
            {
                cargaCargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaCargaIntegracao.ProblemaIntegracao = mensagemErro;
                repCargaCargaCancelamentoCargaIntegracao.Atualizar(cargaCargaIntegracao);
            }
            else
            {
                cargaCargaIntegracao.ProblemaIntegracao = string.Empty;
                cargaCargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                repCargaCargaCancelamentoCargaIntegracao.Atualizar(cargaCargaIntegracao);
            }
        }

        public bool IntegrarCancelamentoContratoTerceiro(Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contrato, Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.Cliente pessoa, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, string url, string usuario, string senha, string contexto, Repositorio.UnitOfWork unitOfWork, out string mensagemErro)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("pt-BR");

            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);
            Repositorio.Embarcador.Terceiros.ContratoFreteIntegracaoArquivo repContratoFreteIntegracaoArquivo = new Repositorio.Embarcador.Terceiros.ContratoFreteIntegracaoArquivo(unitOfWork);

            string xmlRequest = "";
            string xmlResponse = "";
            string msgErro = "";
            mensagemErro = "";

            Dominio.Entidades.Embarcador.Terceiros.ContratoFreteIntegracaoArquivo integracaoArquivo = null;

            try
            {
                xmlResponse = ConsultarMovimento(contrato.CodigoIntegracao, contrato.CodigoCompanhia, url, usuario, senha, contexto, out msgErro);
                integracaoArquivo = new Dominio.Entidades.Embarcador.Terceiros.ContratoFreteIntegracaoArquivo
                {
                    ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlRequest, "xml", unitOfWork),
                    ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlResponse, "xml", unitOfWork),
                    Data = DateTime.Now,
                    Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento,
                    Mensagem = "Consulta do movimento de contrato " + msgErro
                };
                repContratoFreteIntegracaoArquivo.Inserir(integracaoArquivo);
                contrato.ArquivosTransacao.Add(integracaoArquivo);

                repContratoFrete.Atualizar(contrato);

                if (string.IsNullOrWhiteSpace(msgErro))
                {
                    msgErro = "";
                    xmlResponse = ExcluirMovimento(xmlResponse, url, usuario, senha, contexto, out msgErro);
                    integracaoArquivo = new Dominio.Entidades.Embarcador.Terceiros.ContratoFreteIntegracaoArquivo
                    {
                        ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlRequest, "xml", unitOfWork),
                        ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlResponse, "xml", unitOfWork),
                        Data = DateTime.Now,
                        Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento,
                        Mensagem = "Excluir o movimento de contrato " + msgErro
                    };
                    repContratoFreteIntegracaoArquivo.Inserir(integracaoArquivo);
                    contrato.ArquivosTransacao.Add(integracaoArquivo);

                    repContratoFrete.Atualizar(contrato);

                    if (string.IsNullOrWhiteSpace(msgErro))
                        return true;
                    else
                    {
                        mensagemErro = msgErro;
                        return false;
                    }
                }
                else
                {
                    mensagemErro = msgErro;
                    return false;
                }

            }
            catch (Exception excecao)
            {
                xmlResponse = excecao.Message;
                integracaoArquivo = new Dominio.Entidades.Embarcador.Terceiros.ContratoFreteIntegracaoArquivo
                {
                    ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlRequest, "xml", unitOfWork),
                    ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlResponse, "xml", unitOfWork),
                    Data = DateTime.Now,
                    Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento,
                    Mensagem = "Falha no envio do cancelamento do contrato para a TOTVS " + excecao.Message
                };
                contrato.ArquivosTransacao.Add(integracaoArquivo);
                repContratoFreteIntegracaoArquivo.Inserir(integracaoArquivo);

                repContratoFrete.Atualizar(contrato);

                Servicos.Log.TratarErro(excecao, "IntegracaoTOTVS");
                mensagemErro = "Falha no envio do cancelamento do contrato para a TOTVS " + excecao.Message;
                return false;
            }
        }

        public string ConsultarMovimento(string codigoIntegracao, string codigoCompanhia, string url, string usuario, string senha, string contexto, out string msgErro)
        {
            msgErro = string.Empty;
            string xmlRetorno = string.Empty;

            if (string.IsNullOrWhiteSpace(codigoIntegracao))
            {
                msgErro = "Este documento não possui código de integração com a TOTVS";
                return xmlRetorno;
            }

            Totvs svcTotvs = new Totvs();
            ServicoTotvs.DataServer.IwsDataServerClient svcDataServer = svcTotvs.ObterDataServerClient(url, usuario, senha);

            try
            {
                using (OperationContextScope scope = new OperationContextScope(svcDataServer.InnerChannel))
                {
                    var httpRequestProperty = new HttpRequestMessageProperty();
                    httpRequestProperty.Headers[System.Net.HttpRequestHeader.Authorization] = "Basic " +
                                 Convert.ToBase64String(Encoding.ASCII.GetBytes(svcDataServer.ClientCredentials.UserName.UserName + ":" +
                                 svcDataServer.ClientCredentials.UserName.Password));
                    OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = httpRequestProperty;

                    xmlRetorno = svcDataServer.ReadRecord("MovMovimentoTBCData", codigoCompanhia + ";" + codigoIntegracao, contexto);// SaveRecord("MOVMOVIMENTOTBCDATA", xmlRequest, contexto);
                    if (xmlRetorno.Contains("TMOV"))
                        return xmlRetorno;
                    else
                    {
                        msgErro = xmlRetorno;
                        return string.Empty;
                    }
                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao, "IntegracaoTOTVS");
                msgErro = excecao.Message;
                return string.Empty;
            }
        }

        public string ExcluirMovimento(string xmlMovimento, string url, string usuario, string senha, string contexto, out string msgErro)
        {
            msgErro = string.Empty;
            string xmlRetorno = string.Empty;

            if (string.IsNullOrWhiteSpace(xmlMovimento))
            {
                msgErro = "A TOTVS não disponibilizou o XML do movimento para realizar a sua exclusão.";
                return xmlRetorno;
            }

            Totvs svcTotvs = new Totvs();
            ServicoTotvs.DataServer.IwsDataServerClient svcDataServer = svcTotvs.ObterDataServerClient(url, usuario, senha);

            try
            {
                using (OperationContextScope scope = new OperationContextScope(svcDataServer.InnerChannel))
                {
                    var httpRequestProperty = new HttpRequestMessageProperty();
                    httpRequestProperty.Headers[System.Net.HttpRequestHeader.Authorization] = "Basic " +
                                 Convert.ToBase64String(Encoding.ASCII.GetBytes(svcDataServer.ClientCredentials.UserName.UserName + ":" +
                                 svcDataServer.ClientCredentials.UserName.Password));
                    OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = httpRequestProperty;

                    xmlRetorno = svcDataServer.DeleteRecord("MovMovimentoTBCData", xmlMovimento, contexto);// SaveRecord("MOVMOVIMENTOTBCDATA", xmlRequest, contexto);
                    if (xmlRetorno.Contains("sucesso"))
                        return xmlRetorno;
                    else
                    {
                        msgErro = xmlRetorno;
                        return string.Empty;
                    }
                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao, "IntegracaoTOTVS");
                msgErro = excecao.Message;
                return string.Empty;
            }
        }

        private bool IntegrarLancamentoAcrescimoDescontoContratoTerceiro(Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoIntegracao integracaoContrato, Repositorio.UnitOfWork unitOfWork, out string msgRetorno)
        {
            Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto repContratoFreteAcrescimoDesconto = new Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto(unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Terceiros.ContratoFreteIntegracaoArquivo repContratoFreteIntegracaoArquivo = new Repositorio.Embarcador.Terceiros.ContratoFreteIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();
            Dominio.Entidades.Cliente pessoa = integracaoContrato.ContratoFreteAcrescimoDesconto.ContratoFrete.TransportadorTerceiro;

            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("pt-BR");
            msgRetorno = string.Empty;
            string xmlRequest = "";
            string xmlResponse = "";
            Totvs svcTotvs = new Totvs();

            string numeroDocumento = integracaoContrato.ContratoFreteAcrescimoDesconto.Codigo.ToString("D").PadLeft(10, '0') + "01";

            ServicoTotvs.DataServer.IwsDataServerClient svcDataServer = svcTotvs.ObterDataServerClient(configuracaoIntegracao.URLIntegracaoTotvs, configuracaoIntegracao.UsuarioTotvs, configuracaoIntegracao.SenhaTotvs);
            xmlRequest = @"<FinLAN>
                              <FLAN>
                                <CODCOLIGADA>10</CODCOLIGADA>
                                <IDLAN>-1</IDLAN>
                                <NUMERODOCUMENTO>" + numeroDocumento + @"</NUMERODOCUMENTO>
                                <PAGREC>2</PAGREC>
                                <CODAPLICACAO>F</CODAPLICACAO>
                                <CODCCUSTO>1.0001.001.001</CODCCUSTO>
                                <HISTORICO>" + integracaoContrato.ContratoFreteAcrescimoDesconto.Observacao + @"</HISTORICO>
                                <DATACRIACAO>" + integracaoContrato.ContratoFreteAcrescimoDesconto.Data.ToString("yyyy-MM-ddTHH:mm:ss") + @"</DATACRIACAO>
                                <DATAVENCIMENTO>" + integracaoContrato.ContratoFreteAcrescimoDesconto.Data.ToString("yyyy-MM-ddTHH:mm:ss") + @"</DATAVENCIMENTO>
                                <DATAEMISSAO>" + integracaoContrato.ContratoFreteAcrescimoDesconto.Data.ToString("yyyy-MM-ddTHH:mm:ss") + @"</DATAEMISSAO>
                                <DATAPREVBAIXA>" + integracaoContrato.ContratoFreteAcrescimoDesconto.Data.ToString("yyyy-MM-ddTHH:mm:ss") + @"</DATAPREVBAIXA>
                                <VALORORIGINAL>" + integracaoContrato.ContratoFreteAcrescimoDesconto.Valor.ToString("n2").Replace(".", "") + @"</VALORORIGINAL>
                                <VALORDESCONTO>0,0000</VALORDESCONTO>
                                <VALOROP8>0,0000</VALOROP8>
                                <CODCOLCFO>" + pessoa.CodigoCompanhia + @"</CODCOLCFO>
                                <CODCFO>" + pessoa.CodigoIntegracao + @"</CODCFO>
                                <CODCOLCXA>10</CODCOLCXA>
                                <CODCXA>" + (pessoa.CodigoIntegracaoDadosBancarios) + @"</CODCXA>
                                <CODTDO>" + (pessoa.Tipo == "F" ? "00152" : "00153") + @"</CODTDO>
                                <CODFILIAL>" + integracaoContrato.ContratoFreteAcrescimoDesconto.ContratoFrete.Carga.Empresa.CodigoIntegracao + @"</CODFILIAL>
                                <SERIEDOCUMENTO>@@@</SERIEDOCUMENTO>
                                <TIPOCONTABILLAN>2</TIPOCONTABILLAN>
                              </FLAN>
                              <FLANRATCCU>
                                <IDRATCCU>-1</IDRATCCU>
                                <CODCOLIGADA>10</CODCOLIGADA>
                                <IDLAN>-1</IDLAN>
                                <CODCCUSTO>1.0001.001.001</CODCCUSTO>
                                <VALOR>" + integracaoContrato.ContratoFreteAcrescimoDesconto.Valor.ToString("n2").Replace(".", "") + @"</VALOR>
                                <CODCOLNATFINANCEIRA>0</CODCOLNATFINANCEIRA>
                                <CODNATFINANCEIRA>3.55.01.0006</CODNATFINANCEIRA>
                              </FLANRATCCU>
                            </FinLAN>";

            try
            {
                using (OperationContextScope scope = new OperationContextScope(svcDataServer.InnerChannel))
                {
                    var httpRequestProperty = new HttpRequestMessageProperty();
                    httpRequestProperty.Headers[System.Net.HttpRequestHeader.Authorization] = "Basic " +
                                 Convert.ToBase64String(Encoding.ASCII.GetBytes(svcDataServer.ClientCredentials.UserName.UserName + ":" +
                                 svcDataServer.ClientCredentials.UserName.Password));
                    OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = httpRequestProperty;

                    xmlResponse = svcDataServer.SaveRecord("FinLanDataBR", xmlRequest, configuracaoIntegracao.ContextoTotvs);
                    if (xmlResponse.Contains("SaveRecord"))
                        return false;
                    else if (!xmlResponse.Contains("SaveRecord") && xmlResponse.Split(';').Count() > 1)
                    {
                        integracaoContrato.ContratoFreteAcrescimoDesconto.CodigoIntegracaoLancamento = xmlResponse.Split(';')[1];
                        integracaoContrato.ContratoFreteAcrescimoDesconto.CodigoIntegracaoBaixa = xmlResponse.Split(';')[0];
                        repContratoFreteAcrescimoDesconto.Atualizar(integracaoContrato.ContratoFreteAcrescimoDesconto);
                    }
                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao, "IntegracaoTOTVS");
                xmlResponse = excecao.Message;
                return false;
            }

            Dominio.Entidades.Embarcador.Terceiros.ContratoFreteIntegracaoArquivo integracaoArquivo = new Dominio.Entidades.Embarcador.Terceiros.ContratoFreteIntegracaoArquivo
            {
                ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlRequest, "xml", unitOfWork),
                ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlResponse, "xml", unitOfWork),
                Data = DateTime.Now,
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento,
                Mensagem = "Integração do Lançamento do acréscimo e desconto"
            };
            repContratoFreteIntegracaoArquivo.Inserir(integracaoArquivo);
            integracaoContrato.ContratoFreteAcrescimoDesconto.ContratoFrete.ArquivosTransacao.Add(integracaoArquivo);

            repContratoFrete.Atualizar(integracaoContrato.ContratoFreteAcrescimoDesconto.ContratoFrete);

            return true;
        }

        private bool IntegrarBaixaAcrescimoDescontoContratoTerceiro(Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoIntegracao integracaoContrato, Repositorio.UnitOfWork unitOfWork, out string msgRetorno)
        {
            Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto repContratoFreteAcrescimoDesconto = new Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto(unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Terceiros.ContratoFreteIntegracaoArquivo repContratoFreteIntegracaoArquivo = new Repositorio.Embarcador.Terceiros.ContratoFreteIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();
            Dominio.Entidades.Cliente pessoa = integracaoContrato.ContratoFreteAcrescimoDesconto.ContratoFrete.TransportadorTerceiro;

            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("pt-BR");
            msgRetorno = string.Empty;
            string xmlRequest = "";
            string xmlResponse = "";
            Totvs svcTotvs = new Totvs();

            ServicoTotvs.wsProcess.IwsProcessClient svcProcess = svcTotvs.ObterProcessClient(configuracaoIntegracao.URLIntegracaoTotvsProcess, configuracaoIntegracao.UsuarioTotvs, configuracaoIntegracao.SenhaTotvs);
            xmlRequest = @"<FinTBCBaixaParamsProc>
	                        <CodColigada>10</CodColigada>
	                        <DataBaixa>" + integracaoContrato.ContratoFreteAcrescimoDesconto.Data.ToString("yyyy-MM-dd") + @"</DataBaixa>
	                        <CodMoeda>R$</CodMoeda>
	                        <HistoricoBaixa>" + integracaoContrato.ContratoFreteAcrescimoDesconto.Observacao + @"</HistoricoBaixa>
	                        <CotacaoBaixa/>
	                        <UsarDataVencimentoBaixa/>
	                        <UsarDataDefaultBaixa>false</UsarDataDefaultBaixa>
	                        <TipoGeracaoExtratoBaixa>ExtratoParaCadaLancamento</TipoGeracaoExtratoBaixa>
	                        <ContabilizarPosBaixa>false</ContabilizarPosBaixa>
	                        <CodUsuario>" + integracaoContrato.ContratoFreteAcrescimoDesconto.Usuario.Nome + @"</CodUsuario>
	                        <ValidaDataBaixaFeriadoFimDeSemana>false</ValidaDataBaixaFeriadoFimDeSemana>
	                        <Lancamentos>
		                        <FinTBCBaixaLancamento>
			                        <CodColigada>10</CodColigada>
			                        <IdLan>" + integracaoContrato.ContratoFreteAcrescimoDesconto.CodigoIntegracaoLancamento + @"</IdLan>
			                        <Pagamentos>
				                        <FinTBCBaixaPagamento>
					                        <CodColigada>10</CodColigada>
					                        <IdFormaPagamento>1</IdFormaPagamento>
					                        <IdPagto>1</IdPagto>
					                        <Valor>" + integracaoContrato.ContratoFreteAcrescimoDesconto.Valor.ToString("n2").Replace(".", "") + @"</Valor>
					                        <CodColCxa>10</CodColCxa>
					                        <CodCxa>" + (pessoa.CodigoIntegracaoDadosBancarios) + @"</CodCxa>
				                        </FinTBCBaixaPagamento>
			                        </Pagamentos>
		                        </FinTBCBaixaLancamento>
	                        </Lancamentos>
                        </FinTBCBaixaParamsProc>";

            try
            {
                using (OperationContextScope scope = new OperationContextScope(svcProcess.InnerChannel))
                {
                    var httpRequestProperty = new HttpRequestMessageProperty();
                    httpRequestProperty.Headers[System.Net.HttpRequestHeader.Authorization] = "Basic " +
                                 Convert.ToBase64String(Encoding.ASCII.GetBytes(svcProcess.ClientCredentials.UserName.UserName + ":" +
                                 svcProcess.ClientCredentials.UserName.Password));
                    OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = httpRequestProperty;

                    xmlResponse = svcProcess.ExecuteWithParams("FinTBCBaixaDataProcess", xmlRequest);
                    if (xmlResponse.Contains("ExecuteWithParams"))
                        return false;
                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao, "IntegracaoTOTVS");
                xmlResponse = excecao.Message;
                return false;
            }

            Dominio.Entidades.Embarcador.Terceiros.ContratoFreteIntegracaoArquivo integracaoArquivo = new Dominio.Entidades.Embarcador.Terceiros.ContratoFreteIntegracaoArquivo
            {
                ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlRequest, "xml", unitOfWork),
                ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlResponse, "xml", unitOfWork),
                Data = DateTime.Now,
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento,
                Mensagem = "Integração da baixa do acréscimo e desconto"
            };
            repContratoFreteIntegracaoArquivo.Inserir(integracaoArquivo);
            integracaoContrato.ContratoFreteAcrescimoDesconto.ContratoFrete.ArquivosTransacao.Add(integracaoArquivo);

            repContratoFrete.Atualizar(integracaoContrato.ContratoFreteAcrescimoDesconto.ContratoFrete);

            return true;
        }

        private bool IntegrarCancelamentoBaixaAcrescimoDescontoContratoTerceiro(Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoIntegracao integracaoContrato, Repositorio.UnitOfWork unitOfWork, out string msgRetorno)
        {
            Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto repContratoFreteAcrescimoDesconto = new Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto(unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Terceiros.ContratoFreteIntegracaoArquivo repContratoFreteIntegracaoArquivo = new Repositorio.Embarcador.Terceiros.ContratoFreteIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();

            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("pt-BR");
            msgRetorno = string.Empty;
            string xmlRequest = "";
            string xmlResponse = "";
            Totvs svcTotvs = new Totvs();

            ServicoTotvs.wsProcess.IwsProcessClient svcDataServer = svcTotvs.ObterProcessClient(configuracaoIntegracao.URLIntegracaoTotvsProcess, configuracaoIntegracao.UsuarioTotvs, configuracaoIntegracao.SenhaTotvs);
            xmlRequest = @"<FinLanCancelamentoBaixaParamsProc>
	                        <Origem>Baixa</Origem>
	                        <DescompensarExtrato>false</DescompensarExtrato>
	                        <DataCancelamento>" + DateTime.Now.Date.ToString("yyyy-MM-dd") + @"</DataCancelamento>
	                        <CodSistema>F</CodSistema>
	                        <TipoCancelamentoBaixaExtrato>CancelaSomenteItensSelecionados</TipoCancelamentoBaixaExtrato>
	                        <ListIdlanIdBaixa>
		                        <FinLanBaixaPKPar>
			                        <CodColigada>10</CodColigada>
			                        <IdLan>" + integracaoContrato.ContratoFreteAcrescimoDesconto.CodigoIntegracaoLancamento + @"</IdLan>
		                        </FinLanBaixaPKPar>
	                        </ListIdlanIdBaixa>
                        </FinLanCancelamentoBaixaParamsProc>";

            try
            {
                using (OperationContextScope scope = new OperationContextScope(svcDataServer.InnerChannel))
                {
                    var httpRequestProperty = new HttpRequestMessageProperty();
                    httpRequestProperty.Headers[System.Net.HttpRequestHeader.Authorization] = "Basic " +
                                 Convert.ToBase64String(Encoding.ASCII.GetBytes(svcDataServer.ClientCredentials.UserName.UserName + ":" +
                                 svcDataServer.ClientCredentials.UserName.Password));
                    OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = httpRequestProperty;

                    xmlResponse = svcDataServer.ExecuteWithParams("FinLanBaixaCancelamentoData", xmlRequest);
                    if (xmlResponse.Contains("ExecuteWithParams"))
                        return false;
                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao, "IntegracaoTOTVS");
                xmlResponse = excecao.Message;
                return false;
            }

            Dominio.Entidades.Embarcador.Terceiros.ContratoFreteIntegracaoArquivo integracaoArquivo = new Dominio.Entidades.Embarcador.Terceiros.ContratoFreteIntegracaoArquivo
            {
                ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlRequest, "xml", unitOfWork),
                ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlResponse, "xml", unitOfWork),
                Data = DateTime.Now,
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento,
                Mensagem = "Integração do cancelamento da baixa do acréscimo e desconto"
            };
            repContratoFreteIntegracaoArquivo.Inserir(integracaoArquivo);
            integracaoContrato.ContratoFreteAcrescimoDesconto.ContratoFrete.ArquivosTransacao.Add(integracaoArquivo);

            repContratoFrete.Atualizar(integracaoContrato.ContratoFreteAcrescimoDesconto.ContratoFrete);

            return true;
        }

        private bool IntegrarCancelamentoLancamentoAcrescimoDescontoContratoTerceiro(Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoIntegracao integracaoContrato, Repositorio.UnitOfWork unitOfWork, out string msgRetorno)
        {
            Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto repContratoFreteAcrescimoDesconto = new Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto(unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Terceiros.ContratoFreteIntegracaoArquivo repContratoFreteIntegracaoArquivo = new Repositorio.Embarcador.Terceiros.ContratoFreteIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();

            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("pt-BR");
            msgRetorno = string.Empty;
            string xmlRequest = "";
            string xmlResponse = "";
            Totvs svcTotvs = new Totvs();

            ServicoTotvs.wsProcess.IwsProcessClient svcDataServer = svcTotvs.ObterProcessClient(configuracaoIntegracao.URLIntegracaoTotvsProcess, configuracaoIntegracao.UsuarioTotvs, configuracaoIntegracao.SenhaTotvs);
            xmlRequest = @"<FinLanCancelamentoParamsProc>
	                        <CodColigada>10</CodColigada>
	                        <DataCancelamento>" + DateTime.Now.Date.ToString("yyyy-MM-dd") + @"</DataCancelamento>
	                        <Historico>[HIS]</Historico>
	                        <ListaDeLancamentos>
		                        <int>" + integracaoContrato.ContratoFreteAcrescimoDesconto.CodigoIntegracaoLancamento + @"</int>
	                        </ListaDeLancamentos>
                        </FinLanCancelamentoParamsProc>";

            try
            {
                using (OperationContextScope scope = new OperationContextScope(svcDataServer.InnerChannel))
                {
                    var httpRequestProperty = new HttpRequestMessageProperty();
                    httpRequestProperty.Headers[System.Net.HttpRequestHeader.Authorization] = "Basic " +
                                 Convert.ToBase64String(Encoding.ASCII.GetBytes(svcDataServer.ClientCredentials.UserName.UserName + ":" +
                                 svcDataServer.ClientCredentials.UserName.Password));
                    OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = httpRequestProperty;

                    xmlResponse = svcDataServer.ExecuteWithParams("FinLanCancelamentoData", xmlRequest);
                    if (xmlResponse.Contains("ExecuteWithParams"))
                        return false;
                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao, "IntegracaoTOTVS");
                xmlResponse = excecao.Message;
                return false;
            }

            Dominio.Entidades.Embarcador.Terceiros.ContratoFreteIntegracaoArquivo integracaoArquivo = new Dominio.Entidades.Embarcador.Terceiros.ContratoFreteIntegracaoArquivo
            {
                ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlRequest, "xml", unitOfWork),
                ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlResponse, "xml", unitOfWork),
                Data = DateTime.Now,
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento,
                Mensagem = "Integração do cancelamento do lançamento do acréscimo e desconto"
            };
            repContratoFreteIntegracaoArquivo.Inserir(integracaoArquivo);
            integracaoContrato.ContratoFreteAcrescimoDesconto.ContratoFrete.ArquivosTransacao.Add(integracaoArquivo);

            repContratoFrete.Atualizar(integracaoContrato.ContratoFreteAcrescimoDesconto.ContratoFrete);

            return true;
        }

        private bool IntegrarExclusaoLancamentoAcrescimoDescontoContratoTerceiro(Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoIntegracao integracaoContrato, Repositorio.UnitOfWork unitOfWork, out string msgRetorno)
        {
            Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto repContratoFreteAcrescimoDesconto = new Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto(unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Terceiros.ContratoFreteIntegracaoArquivo repContratoFreteIntegracaoArquivo = new Repositorio.Embarcador.Terceiros.ContratoFreteIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();

            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("pt-BR");
            msgRetorno = string.Empty;
            string xmlRequest = "";
            string xmlResponse = "";
            Totvs svcTotvs = new Totvs();

            ServicoTotvs.DataServer.IwsDataServerClient svcDataServer = svcTotvs.ObterDataServerClient(configuracaoIntegracao.URLIntegracaoTotvs, configuracaoIntegracao.UsuarioTotvs, configuracaoIntegracao.SenhaTotvs);
            xmlRequest = @"<FinLAN>
	                        <FLAN>
		                        <CODCOLIGADA>10</CODCOLIGADA>
		                        <IDLAN>" + integracaoContrato.ContratoFreteAcrescimoDesconto.CodigoIntegracaoLancamento + @"</IDLAN>
	                        </FLAN>
                        </FinLAN>";

            try
            {
                using (OperationContextScope scope = new OperationContextScope(svcDataServer.InnerChannel))
                {
                    var httpRequestProperty = new HttpRequestMessageProperty();
                    httpRequestProperty.Headers[System.Net.HttpRequestHeader.Authorization] = "Basic " +
                                 Convert.ToBase64String(Encoding.ASCII.GetBytes(svcDataServer.ClientCredentials.UserName.UserName + ":" +
                                 svcDataServer.ClientCredentials.UserName.Password));
                    OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = httpRequestProperty;

                    xmlResponse = svcDataServer.DeleteRecord("FinLanDataBR", xmlRequest, configuracaoIntegracao.ContextoTotvs);
                    if (!xmlResponse.Contains("sucesso"))
                    {
                        msgRetorno = xmlResponse;
                        return false;
                    }
                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao, "IntegracaoTOTVS");
                xmlResponse = excecao.Message;
                return false;
            }

            Dominio.Entidades.Embarcador.Terceiros.ContratoFreteIntegracaoArquivo integracaoArquivo = new Dominio.Entidades.Embarcador.Terceiros.ContratoFreteIntegracaoArquivo
            {
                ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlRequest, "xml", unitOfWork),
                ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlResponse, "xml", unitOfWork),
                Data = DateTime.Now,
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento,
                Mensagem = "Integração da exclusão do lançamento do acréscimo e desconto"
            };
            repContratoFreteIntegracaoArquivo.Inserir(integracaoArquivo);
            integracaoContrato.ContratoFreteAcrescimoDesconto.ContratoFrete.ArquivosTransacao.Add(integracaoArquivo);

            repContratoFrete.Atualizar(integracaoContrato.ContratoFreteAcrescimoDesconto.ContratoFrete);

            return true;
        }

    }
}
