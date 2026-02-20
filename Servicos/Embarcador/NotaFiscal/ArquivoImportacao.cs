using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Text;
using System.Text.RegularExpressions;

namespace Servicos.Embarcador.NotaFiscal
{
    public class ArquivoImportacao
    {
        #region Métodos Globais

        public static List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal> LerArquivoPDF(byte[] arquivo, out string mensagemRetorno, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Servicos.WebService.Pessoas.Pessoa serPessoa = new WebService.Pessoas.Pessoa(unitOfWork);

            mensagemRetorno = "";
            List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal> notasFiscais = new List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal>();

            using (PdfReader leitor = new PdfReader(arquivo))
            {
                StringBuilder texto = new StringBuilder();
                for (int a = 1; a <= leitor.NumberOfPages; a++)
                {
                    texto.Append(PdfTextExtractor.GetTextFromPage(leitor, a));

                    Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal notaFiscal = new Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal();

                    MatchCollection matchList = Regex.Matches(texto.ToString(), @"[0-9]{4}\ ?[0-9]{4}\ ?[0-9]{4}\ ?[0-9]{4}\ ?[0-9]{4}\ ?[0-9]{4}\ ?[0-9]{4}\ ?[0-9]{4}\ ?[0-9]{4}\ ?[0-9]{4}\ ?[0-9]{4}");
                    var listChave = matchList.Cast<Match>().Select(match => match.Value).ToList();
                    for (int i = 0; i < listChave.Count; i++)
                    {
                        if (Utilidades.Validate.ValidarChaveNFe(Utilidades.String.OnlyNumbers(listChave[i])))
                        {
                            if (notasFiscais.Any(c => c.Chave == Utilidades.String.OnlyNumbers(listChave[i])))
                                continue;
                            notaFiscal.Chave = Utilidades.String.OnlyNumbers(listChave[i]);
                            break;
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(notaFiscal.Chave))
                    {
                        matchList = Regex.Matches(texto.ToString(), @"[0-9]{2}\.?[0-9]{3}\.?[0-9]{3}\/?[0-9]{4}\-?[0-9]{2}");
                        var listCNPJ = matchList.Cast<Match>().Select(match => match.Value).ToList();
                        int qtdCNPJValido = 0;
                        bool encontrouDestinatario = false;
                        List<double> cnpjs = new List<double>();
                        for (int i = 0; i < listCNPJ.Count; i++)
                        {
                            if (Utilidades.Validate.ValidarCNPJ(Utilidades.String.OnlyNumbers(listCNPJ[i])))
                            {
                                if (qtdCNPJValido == 0)//remetente
                                {
                                    double.TryParse(Utilidades.String.OnlyNumbers(listCNPJ[i]), out double cnpjRemetente);
                                    if (cnpjRemetente > 0)
                                    {
                                        cnpjs.Add(cnpjRemetente);
                                        Dominio.Entidades.Cliente remetente = repCliente.BuscarPorCPFCNPJ(cnpjRemetente);
                                        if (remetente != null)
                                            notaFiscal.Emitente = serPessoa.ConverterObjetoPessoa(remetente);
                                        else
                                            mensagemRetorno += " Emitente da NF-e com o CNPJ " + listCNPJ[i] + " não se encontra cadastrado";
                                    }
                                    else
                                        mensagemRetorno += " Emitente da NF-e com o CNPJ " + listCNPJ[i] + " não se encontra cadastrado";
                                }
                                else if (qtdCNPJValido >= 1 && !encontrouDestinatario)//destinatário
                                {
                                    double.TryParse(Utilidades.String.OnlyNumbers(listCNPJ[i]), out double cnpjDestinatario);
                                    if (cnpjDestinatario > 0 && !cnpjs.Contains(cnpjDestinatario))
                                    {
                                        encontrouDestinatario = true;
                                        Dominio.Entidades.Cliente destinatario = repCliente.BuscarPorCPFCNPJ(cnpjDestinatario);
                                        if (destinatario != null)
                                            notaFiscal.Destinatario = serPessoa.ConverterObjetoPessoa(destinatario);
                                        else
                                            mensagemRetorno += " Destinatário da NF-e com o CNPJ " + listCNPJ[i] + " não se encontra cadastrado";
                                        break;
                                    } 
                                    else if (cnpjDestinatario <= 0)
                                        mensagemRetorno += " Destinatário da NF-e com o CNPJ " + listCNPJ[i] + " não se encontra cadastrado";
                                }
                                qtdCNPJValido++;
                            }
                        }

                        matchList = Regex.Matches(texto.ToString(), @"[0-9]{2}\/[0-9]{2}\/[0-9]{4}");
                        var listDataEmissao = matchList.Cast<Match>().Select(match => match.Value).ToList();
                        for (int i = 0; i < listDataEmissao.Count; i++)
                        {
                            if (i == 1)
                                notaFiscal.DataEmissao = listDataEmissao[i];
                        }

                        if (!string.IsNullOrWhiteSpace(notaFiscal.Chave))
                        {
                            int numero = 0;
                            if (int.TryParse(notaFiscal.Chave.Substring(25, 9), out numero))
                                notaFiscal.Numero = numero;
                            notaFiscal.Serie = notaFiscal.Chave.Substring(22, 3);
                        }

                        string strConsultaValor = texto.ToString();
                        if (strConsultaValor.ToLower().Contains("calculo do imposto") || strConsultaValor.ToLower().Contains("cálculo do imposto") || strConsultaValor.ToLower().Contains("base de cálculo do icms"))
                        {
                            int posicao = strConsultaValor.ToLower().IndexOf("calculo do imposto");
                            if (posicao <= 0)
                                posicao = strConsultaValor.ToLower().IndexOf("cálculo do imposto");
                            if (posicao <= 0)
                                posicao = strConsultaValor.ToLower().IndexOf("base de cálculo do icms");
                            int posicaoTransportador = strConsultaValor.ToLower().IndexOf("transportador");
                            if (posicaoTransportador <= 0)
                                posicaoTransportador = strConsultaValor.ToLower().IndexOf("peso bruto");
                            if (posicaoTransportador <= 0)
                                posicaoTransportador = strConsultaValor.ToLower().IndexOf("volumes");
                            if (posicao > 0)
                            {
                                if (posicaoTransportador > 0)
                                    strConsultaValor = strConsultaValor.Substring(posicao, posicaoTransportador - posicao);
                                else
                                    strConsultaValor = strConsultaValor.Substring(posicao);

                                strConsultaValor = strConsultaValor.Replace(".", "");
                                matchList = Regex.Matches(strConsultaValor, @"[0-9]{0,10}[,]{1,1}[0-9]{0,4}");
                                var listValores = matchList.Cast<Match>().Select(match => match.Value).ToList();
                                BuscarValoresNotaFiscal(listValores, ref notaFiscal);
                            }
                        }

                        string strConsultaPeso = texto.ToString();
                        if (strConsultaPeso.ToLower().Contains("peso bruto"))
                        {
                            int posicao = strConsultaPeso.ToLower().IndexOf("peso bruto");
                            if (posicao > 0)
                            {
                                strConsultaPeso = strConsultaPeso.Substring(posicao);
                                int posicaoProdutos = strConsultaPeso.ToLower().IndexOf("serviços");
                                if (posicaoProdutos <= 0)
                                    posicaoProdutos = strConsultaPeso.ToLower().IndexOf("serviço");

                                if (posicaoProdutos > 0)
                                    strConsultaPeso = strConsultaPeso.Substring(0, posicaoProdutos);

                                strConsultaPeso = strConsultaPeso.Replace(".", "");
                                matchList = Regex.Matches(strConsultaPeso, @"[0-9]{0,10}[,]{1,1}[0-9]{0,4}");
                                var listValores = matchList.Cast<Match>().Select(match => match.Value).ToList();
                                BuscarValoresPeso(listValores, ref notaFiscal);
                            }
                        }

                        string strConsultaObservacao = texto.ToString();
                        if (strConsultaObservacao.ToLower().Contains("complementares"))
                        {
                            int posicao = strConsultaObservacao.ToLower().IndexOf("complementares");
                            if (posicao > 0)
                            {
                                notaFiscal.Observacao = strConsultaObservacao.Substring(posicao, (strConsultaObservacao.Length - posicao) > 1000 ? 1000 : (strConsultaObservacao.Length - posicao));
                            }
                        }

                        notaFiscal.ModalidadeFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.NaoDefinido;
                        if (notaFiscal.Emitente != null && notaFiscal.Destinatario != null && !string.IsNullOrWhiteSpace(notaFiscal.Chave) && !string.IsNullOrWhiteSpace(notaFiscal.DataEmissao) && !string.IsNullOrWhiteSpace(notaFiscal.Serie) && notaFiscal.Numero > 0)
                        {
                            double.TryParse(notaFiscal.Emitente.CPFCNPJ, out double cnpjEmitente);
                            if (cnpjEmitente == 0)
                                continue;
                            Dominio.Entidades.Cliente emitente = repCliente.BuscarPorCPFCNPJ(cnpjEmitente);
                            if (emitente == null || emitente.GrupoPessoas == null || !emitente.GrupoPessoas.LerPDFNotaFiscalRecebidaPorEmail)
                                continue;
                            notasFiscais.Add(notaFiscal);
                        }
                    }

                    texto = new StringBuilder();
                }

                return notasFiscais;
            }
        }

        public static List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal> LerArquivo(System.IO.MemoryStream arquivo, Dominio.Entidades.Embarcador.Configuracoes.ArquivoImportacaoNotaFiscal modelo)
        {
            List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal> notasFiscais = new List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal>();
            arquivo.Seek(0, System.IO.SeekOrigin.Begin);
            ExcelPackage package = new ExcelPackage(arquivo);

            ExcelWorksheet worksheet = package.Workbook.Worksheets.First();

            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("pt-BR");

            for (var i = 2; i <= worksheet.Dimension.End.Row; i++)
            {
                Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal notaFiscal = new Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal();

                for (var j = 0; j < modelo.Campos.Count; j++)
                {
                    Dominio.Entidades.Embarcador.Configuracoes.ArquivoImportacaoNotaFiscalCampo campo = modelo.Campos[j];

                    object valor = LerCampo(campo, worksheet.Cells[i, campo.Posicao].Text);

                    SetNestedPropertyValue(notaFiscal, campo.Propriedade, valor);
                }

                if (!string.IsNullOrWhiteSpace(notaFiscal.Chave) || notaFiscal.Numero > 0)
                    notasFiscais.Add(notaFiscal);
            }

            return notasFiscais;
        }

        public static DataTable ObterDataTable(string extensao, System.IO.MemoryStream arquivo, Repositorio.UnitOfWork unitOfWork)
        {

            arquivo.Seek(0, SeekOrigin.Begin);

            string caminhoArquivo = Utilidades.IO.FileStorageService.Storage.Combine(Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivos, "Importacao");

            caminhoArquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminhoArquivo, Guid.NewGuid().ToString() + extensao);

            if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoArquivo))
                Utilidades.IO.FileStorageService.Storage.Delete(caminhoArquivo);

            using (Stream file = Utilidades.IO.FileStorageService.Storage.OpenWrite(caminhoArquivo))
                arquivo.CopyTo(file);

            DataTable dt = null;

            if (extensao == ".csv")
            {
                dt = Utilidades.File.ConvertCSVtoDataTable(caminhoArquivo);
            }
            else if (extensao == ".xls")
            {
                string connString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + caminhoArquivo + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                dt = Utilidades.File.ConvertXSLXtoDataTable(caminhoArquivo, connString);
            }
            else if (extensao == ".xlsx")
            {
                string connString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + caminhoArquivo + ";Extended Properties=Excel 12.0;";
                dt = Utilidades.File.ConvertXSLXtoDataTable(caminhoArquivo, connString);
            }

            return dt;
        }

        public static List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal> LerArquivo(string extensao, System.IO.MemoryStream arquivo, Dominio.Entidades.Embarcador.Configuracoes.ArquivoImportacaoNotaFiscal modelo, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal> notasFiscais = new List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal>();

            arquivo.Seek(0, SeekOrigin.Begin);

            string caminhoArquivo = Utilidades.IO.FileStorageService.Storage.Combine(Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivos, "Importacao");

            caminhoArquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminhoArquivo, Guid.NewGuid().ToString() + extensao);

            if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoArquivo))
                Utilidades.IO.FileStorageService.Storage.Delete(caminhoArquivo);

            using (Stream file = Utilidades.IO.FileStorageService.Storage.OpenWrite(caminhoArquivo))
                arquivo.CopyTo(file);

            DataTable dt = null;

            if (extensao == ".csv")
            {
                dt = Utilidades.File.ConvertCSVtoDataTable(caminhoArquivo);
            }
            else if (extensao == ".xls")
            {
                string connString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + caminhoArquivo + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                dt = Utilidades.File.ConvertXSLXtoDataTable(caminhoArquivo, connString);
            }
            else if (extensao == ".xlsx")
            {
                string connString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + caminhoArquivo + ";Extended Properties=Excel 12.0;";
                dt = Utilidades.File.ConvertXSLXtoDataTable(caminhoArquivo, connString);
            }

            if (dt == null)
                return null;

            for (var i = 0; i < dt.Rows.Count; i++)
            {
                Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal notaFiscal = new Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal();

                for (var j = 0; j < modelo.Campos.Count; j++)
                {
                    Dominio.Entidades.Embarcador.Configuracoes.ArquivoImportacaoNotaFiscalCampo campo = modelo.Campos[j];

                    //Servicos.Log.TratarErro("Campo " + campo.Descricao + " " + campo.Posicao.ToString());

                    object valor = LerCampo(campo, dt.Rows[i][(campo.Posicao - 1)].ToString());

                    //Servicos.Log.TratarErro("Campo valor " + valor);

                    SetNestedPropertyValue(notaFiscal, campo.Propriedade, valor);
                }

                if (!string.IsNullOrWhiteSpace(notaFiscal.Chave) || notaFiscal.Numero > 0)
                    notasFiscais.Add(notaFiscal);
            }

            return notasFiscais;
        }

        #endregion

        #region Métodos Privados

        public static void BuscarValoresPeso(List<string> listValores, ref Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal notaFiscal)
        {
            if (listValores.Count == 3)
            {
                for (int i = 0; i < listValores.Count; i++)
                {
                    if (i == 0)//base de icms                            
                    {
                        decimal.TryParse(listValores[i], out decimal valor);
                        notaFiscal.VolumesTotal = valor;
                    }
                    else if (i == 1)//peso bruto
                    {
                        decimal.TryParse(listValores[i], out decimal valor);
                        notaFiscal.PesoBruto = valor;
                    }
                    else if (i == 2)//peso liquido
                    {
                        decimal.TryParse(listValores[i], out decimal valor);
                        notaFiscal.PesoLiquido = valor;
                    }
                }
            }
            else if (listValores.Count == 2)
            {
                for (int i = 0; i < listValores.Count; i++)
                {
                    if (i == 0)//peso bruto
                    {
                        decimal.TryParse(listValores[i], out decimal valor);
                        notaFiscal.PesoBruto = valor;
                    }
                    else if (i == 1)//peso liquido
                    {
                        decimal.TryParse(listValores[i], out decimal valor);
                        notaFiscal.PesoLiquido = valor;
                    }
                }
            }
        }

        public static void BuscarValoresNotaFiscal(List<string> listValores, ref Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal notaFiscal)
        {
            if (listValores.Count == 18)
            {
                for (int i = 0; i < listValores.Count; i++)
                {
                    if (i == 0)//base de icms                            
                    {
                        decimal.TryParse(listValores[i], out decimal valor);
                        notaFiscal.BaseCalculoICMS = valor;
                    }
                    else if (i == 1)//valor do icms
                    {
                        decimal.TryParse(listValores[i], out decimal valor);
                        notaFiscal.ValorICMS = valor;
                    }
                    else if (i == 2)//base icms st
                    {
                        decimal.TryParse(listValores[i], out decimal valor);
                        notaFiscal.BaseCalculoST = valor;
                    }
                    else if (i == 3)//base st
                    {
                        decimal.TryParse(listValores[i], out decimal valor);
                        notaFiscal.ValorST = valor;
                    }
                    else if (i == 4)//valor importacao
                    {
                        decimal.TryParse(listValores[i], out decimal valor);
                        notaFiscal.ValorImpostoImportacao = valor;
                    }
                    else if (i == 5)//valor uf remet
                    {

                    }
                    else if (i == 6)//valor FCP
                    {

                    }
                    else if (i == 7)//valor pis
                    {
                        decimal.TryParse(listValores[i], out decimal valor);
                        notaFiscal.ValorPIS = valor;
                    }
                    else if (i == 8)//valor produtos
                    {
                        decimal.TryParse(listValores[i], out decimal valor);
                        notaFiscal.ValorTotalProdutos = valor;
                    }
                    else if (i == 9)//valor frete
                    {
                        //decimal.TryParse(listValores[i], out decimal valor);
                        //notaFiscal.ValorFrete = valor;
                    }
                    else if (i == 10)//valor seguro
                    {

                    }
                    else if (i == 11)//valor desconto
                    {

                    }
                    else if (i == 12)//valor outras
                    {

                    }
                    else if (i == 13)//valor ipi
                    {
                        decimal.TryParse(listValores[i], out decimal valor);
                        notaFiscal.ValorIPI = valor;
                    }
                    else if (i == 14)//valor uf dest
                    {

                    }
                    else if (i == 15)//valor difal
                    {

                    }
                    else if (i == 16)//valor cofins
                    {
                        decimal.TryParse(listValores[i], out decimal valor);
                        notaFiscal.ValorCOFINS = valor;
                    }
                    else if (i == 17)//valor nota
                    {
                        decimal.TryParse(listValores[i], out decimal valor);
                        notaFiscal.Valor = valor;
                    }
                }
            }
            else if (listValores.Count == 11 || listValores.Count == 12)
            {
                for (int i = 0; i < listValores.Count; i++)
                {
                    if (i == 0)//base de icms                            
                    {
                        decimal.TryParse(listValores[i], out decimal valor);
                        notaFiscal.BaseCalculoICMS = valor;
                    }
                    else if (i == 1)//valor do icms
                    {
                        decimal.TryParse(listValores[i], out decimal valor);
                        notaFiscal.ValorICMS = valor;
                    }
                    else if (i == 2)//base icms st
                    {
                        decimal.TryParse(listValores[i], out decimal valor);
                        notaFiscal.BaseCalculoST = valor;
                    }
                    else if (i == 3)//valor st
                    {
                        decimal.TryParse(listValores[i], out decimal valor);
                        notaFiscal.ValorST = valor;
                    }
                    else if (i == 4)//valor produtos
                    {
                        decimal.TryParse(listValores[i], out decimal valor);
                        notaFiscal.ValorTotalProdutos = valor;
                    }
                    else if (i == 5)//valor frete
                    {
                        //decimal.TryParse(listValores[i], out decimal valor);
                        //notaFiscal.ValorFrete = valor;
                    }
                    else if (i == 6)//valor seguro
                    {

                    }
                    else if (i == 7)//valor desconto
                    {

                    }
                    else if (i == 8)//valor outras
                    {

                    }
                    else if (i == 9)//valor ipi
                    {
                        decimal.TryParse(listValores[i], out decimal valor);
                        notaFiscal.ValorIPI = valor;
                    }
                    else if (i == 10)//valor nota
                    {
                        decimal.TryParse(listValores[i], out decimal valor);
                        notaFiscal.Valor = valor;
                    }
                }
            }
            else if (listValores.Count == 13)
            {
                for (int i = 0; i < listValores.Count; i++)
                {
                    if (i == 0)//base de icms                            
                    {
                        decimal.TryParse(listValores[i], out decimal valor);
                        notaFiscal.BaseCalculoICMS = valor;
                    }
                    else if (i == 1)//valor do icms
                    {
                        decimal.TryParse(listValores[i], out decimal valor);
                        notaFiscal.ValorICMS = valor;
                    }
                    else if (i == 2)//base icms st
                    {
                        decimal.TryParse(listValores[i], out decimal valor);
                        notaFiscal.BaseCalculoST = valor;
                    }
                    else if (i == 3)//valor st
                    {
                        decimal.TryParse(listValores[i], out decimal valor);
                        notaFiscal.ValorST = valor;
                    }
                    else if (i == 4)//valor pis
                    {
                        decimal.TryParse(listValores[i], out decimal valor);
                        notaFiscal.ValorPIS = valor;
                    }
                    else if (i == 5)//valor produtos
                    {
                        decimal.TryParse(listValores[i], out decimal valor);
                        notaFiscal.ValorTotalProdutos = valor;
                    }
                    else if (i == 6)//valor frete
                    {
                        //decimal.TryParse(listValores[i], out decimal valor);
                        //notaFiscal.ValorFrete = valor;
                    }
                    else if (i == 7)//valor seguro
                    {

                    }
                    else if (i == 8)//valor desconto
                    {

                    }
                    else if (i == 9)//valor outras
                    {

                    }
                    else if (i == 10)//valor ipi
                    {
                        decimal.TryParse(listValores[i], out decimal valor);
                        notaFiscal.ValorIPI = valor;
                    }
                    else if (i == 11)//valor COFINS
                    {
                        decimal.TryParse(listValores[i], out decimal valor);
                        notaFiscal.ValorCOFINS = valor;
                    }
                    else if (i == 12)//valor nota
                    {
                        decimal.TryParse(listValores[i], out decimal valor);
                        notaFiscal.Valor = valor;
                    }
                }
            }
        }

        private static void SetNestedPropertyValue(object target, string propertyName, object value, string mask = "")
        {
            object obj = target;

            string[] properties = propertyName.Split('.');
            List<KeyValuePair<PropertyInfo, object>> valuesOfProperties = new List<KeyValuePair<PropertyInfo, object>>();

            for (int i = 0; i < properties.Length - 1; i++)
            {
                PropertyInfo propertyToGet = target.GetType().GetProperty(properties[i]);

                target = propertyToGet.GetValue(target, null);

                if (target == null)
                    target = Activator.CreateInstance(propertyToGet.PropertyType);

                valuesOfProperties.Add(new KeyValuePair<PropertyInfo, object>(propertyToGet, target));
            }

            PropertyInfo propertyToSet = target.GetType().GetProperty(properties.Last());

            object valueCheck = propertyToSet.GetValue(target, null);

            if (valueCheck != null)
            {
                Type typeOfValue = valueCheck.GetType();

                if (typeOfValue == typeof(string) && !string.IsNullOrWhiteSpace((string)valueCheck))
                    return;
                else if (typeOfValue == typeof(int) && (int)valueCheck > 0)
                    return;
                else if (typeOfValue == typeof(decimal) && (decimal)valueCheck > 0m)
                    return;
                else if (typeOfValue == typeof(double) && (double)valueCheck > 0d)
                    return;
                else if (typeOfValue == typeof(DateTime) && (DateTime)valueCheck != DateTime.MinValue)
                    return;
            }

            if (propertyToSet.PropertyType.FullName.StartsWith("System"))
            {
                if (propertyToSet.PropertyType.FullName.Contains("DateTime") && !string.IsNullOrWhiteSpace(mask))
                {
                    DateTime data = DateTime.MinValue;

                    DateTime.TryParseExact((string)value, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out data);

                    if (data == DateTime.MinValue)
                        DateTime.TryParseExact((string)value, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out data);
                    if (data == DateTime.MinValue)
                        DateTime.TryParseExact((string)value, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out data);

                    if (data != DateTime.MinValue)
                        value = data;
                    else if (propertyToSet.PropertyType.FullName.Contains("Nullable"))
                        value = null;
                    else
                        value = DateTime.Now;
                }
                else
                {
                    value = Convert.ChangeType(value, propertyToSet.PropertyType);
                }
            }

            propertyToSet.SetValue(target, value, null);

            valuesOfProperties.Add(new KeyValuePair<PropertyInfo, object>(propertyToSet, value));

            for (var i = valuesOfProperties.Count() - 1; i > 0; i--)
                valuesOfProperties[i].Key.SetValue(valuesOfProperties[i - 1].Value, valuesOfProperties[i].Value);

            valuesOfProperties[0].Key.SetValue(obj, valuesOfProperties[0].Value);
        }

        public static object LerCampo(Dominio.Entidades.Embarcador.Configuracoes.ArquivoImportacaoNotaFiscalCampo campo, string valor)
        {
            switch (campo.TipoPropriedade)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampo.Alfanumerico:

                    return valor.Trim();

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampo.Data:

                    if (!string.IsNullOrWhiteSpace(valor))
                    {
                        DateTime data = DateTime.MinValue;

                        DateTime.TryParseExact(valor, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out data);

                        if (data == DateTime.MinValue)
                            DateTime.TryParseExact(valor, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out data);
                        if (data == DateTime.MinValue)
                            DateTime.TryParseExact(valor, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out data);
                        if (data == DateTime.MinValue)
                            DateTime.TryParseExact(valor, "ddMMyyyy", null, System.Globalization.DateTimeStyles.None, out data);

                        return data;
                    }
                    else
                    {
                        return null;
                    }

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampo.Decimal:

                    if (!string.IsNullOrWhiteSpace(valor))
                    {
                        //decimal dec = 0m;

                        //decimal.TryParse(valor, out dec);

                        return Utilidades.Decimal.Converter(valor);

                        //return dec;
                    }
                    else
                    {
                        return 0m;
                    }

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampo.Numerico:

                    int i = 0;

                    int.TryParse(Utilidades.String.OnlyNumbers(valor), out i);

                    return i;

                default:

                    return null;
            }
        }

        #endregion
    }
}
