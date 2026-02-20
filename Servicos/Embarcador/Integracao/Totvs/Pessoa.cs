using Dominio.ObjetosDeValor.Enumerador;
using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;

namespace Servicos.Embarcador.Integracao.Totvs
{
    public class Pessoa
    {
        public bool IntegrarPessoaJuridica(Dominio.Entidades.Cliente pessoa, string url, string usuario, string senha, string contexto, out string xmlRequest, out string xmlResponse, Repositorio.UnitOfWork unitOfWork)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("pt-BR");
            xmlRequest = "";
            xmlResponse = "";
            Totvs svcTotvs = new Totvs();

            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            //ServicoTotvs.DataServer.IwsDataServerClient svcDataServer = svcTotvs.ObterDataServerClient("http://177.55.191.130:8052", "multisoftware", "TMTLog1!");
            ServicoTotvs.DataServer.IwsDataServerClient svcDataServer = svcTotvs.ObterDataServerClient(url, usuario, senha);

            string codigoIntegracao = pessoa.CodigoIntegracao;
            if (string.IsNullOrWhiteSpace(codigoIntegracao))
                codigoIntegracao = "-1";

            xmlRequest = @"<FinCFOBR>
                 <FCFO>
                    <CODCOLIGADA>10</CODCOLIGADA>
                    <CODCFO>" + codigoIntegracao + @"</CODCFO>
                    <NOMEFANTASIA>" + (!string.IsNullOrEmpty(pessoa.NomeFantasia) ? pessoa.NomeFantasia : pessoa.Nome) + @"</NOMEFANTASIA>
                    <NOME>" + pessoa.Nome + @"</NOME>
                    <CGCCFO>" + pessoa.CPF_CNPJ_Formatado + @"</CGCCFO>
                    <INSCRESTADUAL>" + pessoa.IE_RG + @"</INSCRESTADUAL>
                    <PAGREC>3</PAGREC>
                    <RUA>" + pessoa.Endereco + @"</RUA>
                    <NUMERO>" + pessoa.Numero + @"</NUMERO>
                    <BAIRRO>" + pessoa.Bairro + @"</BAIRRO>
                    <COMPLEMENTO>" + pessoa.Complemento + @"</COMPLEMENTO>
                    <CIDADE>" + (pessoa.Localidade?.Descricao ?? "") + @"</CIDADE>
                    <CODETD>" + (pessoa.Localidade?.Estado?.Sigla ?? "") + @"</CODETD>
                    <CEP>" + pessoa.CEP + @"</CEP>
                    <TELEFONE>" + pessoa.Telefone1 + @"</TELEFONE>
                    <EMAIL>" + pessoa.Email + @"</EMAIL>
                    <CONTATO>" + (pessoa.Contatos?.FirstOrDefault()?.Contato ?? "") + @"</CONTATO>
                    <ATIVO>1</ATIVO>
                    <DTINICATIVIDADES>2000-01-01T00:01:00-02:00</DTINICATIVIDADES>
                    <CODMUNICIPIO>" + pessoa.Localidade.CodigoIBGESemUf.ToString("D").PadLeft(5, '0') + @"</CODMUNICIPIO>
                    <PESSOAFISOUJUR>" + pessoa.Tipo + @"</PESSOAFISOUJUR>
                    <PAIS>" + (pessoa.Localidade?.Pais?.Descricao ?? "BRASIL") + @"</PAIS>
                    <CONTRIBUINTE>" + (pessoa.IndicadorIE == Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE.ContribuinteICMS ? "1" : "0") + @"</CONTRIBUINTE>
                    <CONTRIBUINTEISS>" + (!String.IsNullOrWhiteSpace(pessoa.InscricaoMunicipal) ? "00001" : "00000") + @"</CONTRIBUINTEISS>
                    <CODRECEITA>" + (pessoa.Tipo == "F" ? "0588" : "1708") + @"</CODRECEITA>
                    <OPTANTEPELOSIMPLES>" + (pessoa.RegimeTributario == Dominio.ObjetosDeValor.Embarcador.Enumeradores.RegimeTributario.SimplesNacional ? "00001" : "00000") + @"</OPTANTEPELOSIMPLES>
                  </FCFO>
                 </FinCFOBR>";

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
                    xmlResponse = svcDataServer.SaveRecord("FinCFODataBR", xmlRequest, contexto);

                    if (!xmlResponse.Contains("SaveRecord") && xmlResponse.Split(';').Count() > 1)
                    {
                        pessoa.CodigoIntegracao = xmlResponse.Split(';')[1];
                        pessoa.CodigoCompanhia = xmlResponse.Split(';')[0];
                        repCliente.Atualizar(pessoa);
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

        public bool IntegrarAutonomo(Dominio.Entidades.Cliente pessoa, string url, string usuario, string senha, string contexto, out string xmlRequest, out string xmlResponse, Repositorio.UnitOfWork unitOfWork)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("pt-BR");
            xmlRequest = "";
            xmlResponse = "";
            Totvs svcTotvs = new Totvs();

            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            //ServicoTotvs.DataServer.IwsDataServerClient svcDataServer = svcTotvs.ObterDataServerClient("http://177.55.191.130:8052", "multisoftware", "TMTLog1!");
            ServicoTotvs.DataServer.IwsDataServerClient svcDataServer = svcTotvs.ObterDataServerClient(url, usuario, senha);

            string codigoIntegracao = pessoa.CodigoIntegracao;
            if (string.IsNullOrWhiteSpace(codigoIntegracao))
                codigoIntegracao = "-1";

            xmlRequest = @"<FinCFOBR>
	                        <FCFO>
		                        <CODCOLIGADA>10</CODCOLIGADA>
		                        <CODCFO>" + codigoIntegracao + @"</CODCFO>
		                        <NOMEFANTASIA>" + (!string.IsNullOrEmpty(pessoa.NomeFantasia) ? pessoa.NomeFantasia : pessoa.Nome) + @"</NOMEFANTASIA>
		                        <NOME>" + pessoa.Nome + @"</NOME>
		                        <CGCCFO>" + pessoa.CPF_CNPJ_Formatado + @"</CGCCFO>
		                        <PAGREC>3</PAGREC>
		                        <RUA>" + pessoa.Endereco + @"</RUA>
		                        <NUMERO>" + pessoa.Numero + @"</NUMERO>
		                        <BAIRRO>" + pessoa.Bairro + @"</BAIRRO>
		                        <CIDADE>" + (pessoa.Localidade?.Descricao ?? "") + @"</CIDADE>
		                        <CODETD>" + (pessoa.Localidade?.Estado?.Sigla ?? "") + @"</CODETD>
		                        <CEP>" + pessoa.CEP + @"</CEP>
		                        <TELEFONE>" + pessoa.Telefone1 + @"</TELEFONE>
		                        <EMAIL>" + pessoa.Email + @"</EMAIL>
		                        <ATIVO>1</ATIVO>
		                        <DTINICATIVIDADES>2000-01-01T00:01:00-02:00</DTINICATIVIDADES>
		                        <CODMUNICIPIO>" + (pessoa.Localidade?.CodigoIBGESemUf.ToString("D").PadLeft(5, '0') ?? "") + @"</CODMUNICIPIO>
		                        <PESSOAFISOUJUR>F</PESSOAFISOUJUR>
		                        <CONTATOPGTO>0</CONTATOPGTO> 
		                        <PAIS>" + (pessoa.Localidade?.Pais?.Descricao ?? "BRASIL") + @"</PAIS>
		                        <CONTRIBUINTE>0</CONTRIBUINTE> 
		                        <CONTRIBUINTEISS>0</CONTRIBUINTEISS>
		                        <NUMDEPENDENTES>" + (pessoa.Contatos?.Count().ToString("D") ?? "0") + @"</NUMDEPENDENTES>
		                        <CATEGORIAAUTONOMO>15</CATEGORIAAUTONOMO> 
		                        <VROUTRASDEDUCOESIRRF>0.0000</VROUTRASDEDUCOESIRRF>
		                        <CODRECEITA>0588</CODRECEITA>
		                        <OPTANTEPELOSIMPLES>0</OPTANTEPELOSIMPLES>
		                        <TIPORUA>0</TIPORUA> 
		                        <TIPOBAIRRO>0</TIPOBAIRRO> 
		                        <REGIMEISS>N</REGIMEISS> 
		                        <RETENCAOISS>0</RETENCAOISS>
		                        <RAMOATIV>0</RAMOATIV>
		                        <CBOAUTONOMO>782505</CBOAUTONOMO>
		                        <CIAUTONOMO>" + Utilidades.String.OnlyNumbers(pessoa.PISPASEP) + @"</CIAUTONOMO>
		                        <NIT>" + Utilidades.String.OnlyNumbers(pessoa.PISPASEP) + @"</NIT>
		                        <DTNASCIMENTO>" + (pessoa.DataNascimento.HasValue ? pessoa.DataNascimento.Value.ToString("yyyy-MM-dd") : "") + @"</DTNASCIMENTO> 
		                        <CODPAGTOGPS>2100</CODPAGTOGPS>
		                        <CIDENTIDADE>" + pessoa.RG_Passaporte + @"</CIDENTIDADE>
		                        <CI_ORGAO>" + (pessoa.OrgaoEmissorRG.HasValue ? pessoa.OrgaoEmissorRG.Value.ObterDescricao() : "") + @"</CI_ORGAO>
		                        <CI_UF>" + (pessoa.Localidade?.Estado.Sigla ?? "") + @"</CI_UF>
	                        </FCFO>
                        </FinCFOBR>";
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
                    xmlResponse = svcDataServer.SaveRecord("FinCFODataBR", xmlRequest, contexto);

                    if (!xmlResponse.Contains("SaveRecord") && xmlResponse.Split(';').Count() > 1)
                    {
                        pessoa.CodigoIntegracao = xmlResponse.Split(';')[1];
                        pessoa.CodigoCompanhia = xmlResponse.Split(';')[0];
                        repCliente.Atualizar(pessoa);
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
    }
}
