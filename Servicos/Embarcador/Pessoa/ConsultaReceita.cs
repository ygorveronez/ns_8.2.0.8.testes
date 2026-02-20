using HtmlAgilityPack;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Servicos.Embarcador.Pessoa
{
    public class ConsultaReceita : ServicoBase
    {        
        public ConsultaReceita(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        public string VerificarMensagemErro(Stream html)
        {
            string mensagem = "";

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.Load(html, Encoding.GetEncoding("iso-8859-7"));

            HtmlNode erro = doc.DocumentNode.SelectSingleNode("/html/body/table[3]/tr[2]/td/b/font/text()");
            if (erro != null)
                mensagem = erro.InnerText.Trim().Replace("\r\n", "");

            return mensagem;
        }

        public List<Dominio.ObjetosDeValor.WebService.Pessoa.InscricaoSintegra> ProcessarHTMLSintegraRetorno(Stream html)
        {
            Servicos.Embarcador.Documentos.Documento serDocumento = new Servicos.Embarcador.Documentos.Documento();
            List<Dominio.ObjetosDeValor.WebService.Pessoa.InscricaoSintegra> inscricoes = new List<Dominio.ObjetosDeValor.WebService.Pessoa.InscricaoSintegra>();
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.Load(html);

            HtmlNodeCollection tableInscricoes = doc.DocumentNode.SelectNodes("//*[@class='tabelaResultado']/tr");
            if (tableInscricoes != null && tableInscricoes.Count > 1)
            {
                for (int i = 1; i < tableInscricoes.Count; i++)
                {
                    Dominio.ObjetosDeValor.WebService.Pessoa.InscricaoSintegra inscricao = new Dominio.ObjetosDeValor.WebService.Pessoa.InscricaoSintegra();

                    HtmlNode tr = tableInscricoes[i];
                    HtmlNode tdUF = tr.SelectSingleNode("td[1]");
                    if (tdUF != null)
                        inscricao.UF = tdUF.InnerText.Trim().Replace("\r\n", "");

                    HtmlNode tdIE = tr.SelectSingleNode("td[3]");
                    if (tdIE != null)
                    {
                        HtmlNode aIE = tdIE.SelectSingleNode("a");
                        inscricao.IE = aIE.InnerText.Trim().Replace("\r\n", "");
                        string htmlLink = tdIE.InnerHtml;
                        string patch = serDocumento.ExtractJavaScript(htmlLink, "?cnpj=", "'");
                        inscricao.LinkEstabelecimento = "https://www.sefaz.rs.gov.br/NFE/NFE-CCC-ESTAB.aspx?cnpj=" + patch;
                    }
                    else
                        continue;

                    HtmlNode tdSituacao = tr.SelectSingleNode("td[4]");
                    if (tdSituacao != null)
                        inscricao.Situacao = tdSituacao.InnerText.Trim().Replace("\r\n", "");
                    else
                        continue;

                    inscricoes.Add(inscricao);
                }
            }
            return inscricoes;
        }

        private void PreecherIdentificacaoPessoa(ref Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa pessoa, HtmlNode DivInfo)
        {
            HtmlNode RazaoSocial = DivInfo.SelectSingleNode("//*[@id='ctl00_cphConteudo_nomeEmpresa']");
            if (RazaoSocial != null)
                pessoa.RazaoSocial = RazaoSocial.InnerText.Trim().Replace("\r\n", "");

            HtmlNode IE = DivInfo.SelectSingleNode("//*[@id='ctl00_cphConteudo_txIE']");
            if (IE != null)
                pessoa.RGIE = IE.InnerText.Trim().Replace("\r\n", "");
        }

        private void PreecherDadosContribuintePessoa(ref Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa pessoa, HtmlNode DivInfo)
        {

            HtmlNode Fantasia = DivInfo.SelectSingleNode("//*[@id='ctl00_cphConteudo_txNomeFantasia']");
            if (Fantasia != null)
            {
                pessoa.NomeFantasia = Fantasia.InnerText.Trim().Replace("\r\n", "");
                if (pessoa.NomeFantasia.Contains("informado"))
                    pessoa.NomeFantasia = pessoa.RazaoSocial;
            }
            HtmlNode CNAE = DivInfo.SelectSingleNode("//*[@id='ctl00_cphConteudo_txCnae']");
            if (CNAE != null)
                pessoa.CNAE = CNAE.InnerText.Trim().Replace("\r\n", "");
        }

        private void PreecheEnderecoPessoa(ref Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa pessoa, HtmlNode DivInfo)
        {
            pessoa.Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco();

            HtmlNode Logradouro = DivInfo.SelectSingleNode("//*[@id='ctl00_cphConteudo_txLogradouro']");
            if (Logradouro != null)
                pessoa.Endereco.Logradouro = Logradouro.InnerText.Trim().Replace("\r\n", "");

            HtmlNode Numero = DivInfo.SelectSingleNode("//*[@id='ctl00_cphConteudo_txNro']");
            if (Numero != null)
                pessoa.Endereco.Numero = Numero.InnerText.Trim().Replace("\r\n", "");

            HtmlNode Complemento = DivInfo.SelectSingleNode("//*[@id='ctl00_cphConteudo_txComplemento']");
            if (Complemento != null)
                pessoa.Endereco.Complemento = Complemento.InnerText.Trim().Replace("\r\n", "");

            HtmlNode CEP = DivInfo.SelectSingleNode("//*[@id='ctl00_cphConteudo_txCEP']");
            if (CEP != null)
                pessoa.Endereco.CEP = CEP.InnerText.Trim().Replace("\r\n", "");

            HtmlNode Bairro = DivInfo.SelectSingleNode("//*[@id='ctl00_cphConteudo_txBairro']");
            if (Bairro != null)
                pessoa.Endereco.Bairro = Bairro.InnerText.Trim().Replace("\r\n", "");

            pessoa.Endereco.Cidade = new Dominio.ObjetosDeValor.Localidade();

            HtmlNode IBGE = DivInfo.SelectSingleNode("//*[@id='ctl00_cphConteudo_txCodMunIBGE']");
            if (IBGE != null)
            {
                int ibge = 0;
                int.TryParse(IBGE.InnerText.Trim().Replace("\r\n", ""), out ibge);
                pessoa.Endereco.Cidade.IBGE = ibge;
            }

            HtmlNode Cidade = DivInfo.SelectSingleNode("//*[@id='ctl00_cphConteudo_txMunIBGE']");
            if (Cidade != null)
                pessoa.Endereco.Cidade.Descricao = Cidade.InnerText.Replace("-", "").Trim().Replace("\r\n", "");

            HtmlNode SiglaUF = DivInfo.SelectSingleNode("//*[@id='ctl00_cphConteudo_txUfLocal']");
            if (SiglaUF != null)
            {
                string[] siglaSplite = SiglaUF.InnerText.Trim().Replace("\r\n", "").Split('-');
                if(siglaSplite.Length > 1)
                    pessoa.Endereco.Cidade.SiglaUF = siglaSplite[1];
            }
                
        }

        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa ProcessarHTMLRetornoSintegra(Stream html)
        {
            Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa pessoa = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa();

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.Load(html);


            HtmlNodeCollection fildSetes = doc.DocumentNode.SelectNodes("//*[@id='Estab']/fieldset");
            if (fildSetes != null && fildSetes.Count > 1)
            {
                for (int i = 1; i < fildSetes.Count; i++)
                {
                    HtmlNode fildSet = fildSetes[i];
                    HtmlNode divLegenda = fildSet.SelectSingleNode("legend");
                    if (divLegenda != null)
                    {
                        string titulo = Utilidades.String.RemoveDiacritics(divLegenda.InnerText.Trim().Replace("\r\n", "")).ToLower();

                        if (titulo.Contains("estabelecimento"))
                            PreecherIdentificacaoPessoa(ref pessoa, fildSet);
                        else if (titulo.Contains("contribuinte"))
                            PreecherDadosContribuintePessoa(ref pessoa, fildSet);
                        else if (titulo.Contains("endere"))
                            PreecheEnderecoPessoa(ref pessoa, fildSet);
                    }
                }
            }
            return pessoa;

        }

        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa ProcessarHTMLRetorno(Stream html)
        {
            Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa pessoa = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa();

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.Load(html);

            HtmlNode DivInfo = doc.DocumentNode.SelectSingleNode("//*[@id='principal']/table[2]/tr/td");
            if (DivInfo != null)
            {

                HtmlNode RazaoSocial = DivInfo.SelectSingleNode("table[3]/tr/td/font[2]/b");
                if (RazaoSocial != null)
                    pessoa.RazaoSocial = RazaoSocial.InnerText.Trim().Replace("\r\n", "");

                HtmlNode Fantasia = DivInfo.SelectSingleNode("table[4]/tr/td/font[2]/b");
                if (Fantasia != null)
                    pessoa.NomeFantasia = Fantasia.InnerText.Trim().Replace("\r\n", "");

                pessoa.Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco();

                HtmlNode Logradouro = DivInfo.SelectSingleNode("table[8]/tr/td[1]/font[2]/b");
                if (Logradouro != null)
                    pessoa.Endereco.Logradouro = Logradouro.InnerText.Trim().Replace("\r\n", "");

                HtmlNode Numero = DivInfo.SelectSingleNode("table[8]/tr/td[3]/font[2]/b");
                if (Numero != null)
                    pessoa.Endereco.Numero = Numero.InnerText.Trim().Replace("\r\n", "").Split('-')[0];

                HtmlNode Complemento = DivInfo.SelectSingleNode("table[8]/tr/td[5]/font[2]/b");
                if (Complemento != null)
                    pessoa.Endereco.Complemento = Complemento.InnerText.Trim().Replace("\r\n", "");

                HtmlNode CEP = DivInfo.SelectSingleNode("table[9]/tr/td[1]/font[2]/b");
                if (CEP != null)
                    pessoa.Endereco.CEP = CEP.InnerText.Trim().Replace("\r\n", "");

                HtmlNode Bairro = DivInfo.SelectSingleNode("table[9]/tr/td[3]/font[2]/b");
                if (Bairro != null)
                    pessoa.Endereco.Bairro = Bairro.InnerText.Trim().Replace("\r\n", "");

                pessoa.Endereco.Cidade = new Dominio.ObjetosDeValor.Localidade();

                HtmlNode Cidade = DivInfo.SelectSingleNode("table[9]/tr/td[5]/font[2]/b");
                if (Cidade != null)
                    pessoa.Endereco.Cidade.Descricao = Cidade.InnerText.Trim().Replace("\r\n", "");

                HtmlNode SiglaUF = DivInfo.SelectSingleNode("table[9]/tr/td[7]/font[2]/b");
                if (SiglaUF != null)
                    pessoa.Endereco.Cidade.SiglaUF = SiglaUF.InnerText.Trim().Replace("\r\n", "");

                HtmlNode Email = DivInfo.SelectSingleNode("table[10]/tr/td[1]/font[2]/b");
                if (SiglaUF != null)
                    pessoa.Email = Email.InnerText.Trim().Replace("\r\n", "");

                HtmlNode Telefone = DivInfo.SelectSingleNode("table[10]/tr/td[3]/font[2]/b");
                if (Telefone != null)
                {
                    string strTelefone = Telefone.InnerText.Trim().Replace("\r\n", "");
                    string[] splitTelefone = strTelefone.Split('/');
                    if (splitTelefone.Length > 1)
                    {
                        pessoa.Endereco.Telefone = splitTelefone[0];
                        pessoa.Endereco.Telefone2 = splitTelefone[1];
                    }
                    else
                    {
                        pessoa.Endereco.Telefone = strTelefone;
                    }
                }
                return pessoa;
            }
            else
            {
                return null;
            }

        }
    }
}
