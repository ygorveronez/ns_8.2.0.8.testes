using System;
using System.IO;

namespace EmissaoCTe.Integracao.Transportador
{
    public class NFe : INFe
    {

        #region Metodos Publicos

        public Retorno<string> EnviarXMLNFeParaIntegracao(Stream arquivo)
        {
            try
            {
                string nomeArquivo = Guid.NewGuid().ToString();

                string caminho = System.Configuration.ConfigurationManager.AppSettings["CaminhoArquivosXMLIntegracao"];

                caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, string.Concat(nomeArquivo, ".xml"));

                using (Stream fStream = Utilidades.IO.FileStorageService.Storage.OpenWrite(caminho))
                {
                    arquivo.CopyTo(fStream);
                }

                arquivo.Close();

                return new Retorno<string>() { Objeto = nomeArquivo, Status = true };
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new Retorno<string>() { Mensagem = "Ocorreu uma falha ao salvar o arquivo.", Status = false };
            }
        }

        public Retorno<bool> DisponibilizarXMLParaEmissaoCTe(string token, string cnpjTransportador, string identificadorXML)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                if (string.IsNullOrWhiteSpace(token))
                    return new Retorno<bool>() { Mensagem = "Token inválido.", Status = false };

                if (string.IsNullOrWhiteSpace(identificadorXML))
                    return new Retorno<bool>() { Mensagem = "Identificador XML inválido.", Status = false };

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);

                Dominio.Entidades.Empresa empresaToken = repEmpresa.BuscarPorTokenIntegracao(token);
                if (empresaToken == null)
                    return new Retorno<bool>() { Mensagem = "Token não é valido para integração.", Status = false };

                Dominio.Entidades.Empresa transportador = repEmpresa.BuscarPorCNPJ(Utilidades.String.OnlyNumbers(cnpjTransportador));
                if (transportador == null)
                    return new Retorno<bool>() { Mensagem = "Transportador não localizado com o CNPJ " + cnpjTransportador, Status = false };

                Servicos.CTe serCTe = new Servicos.CTe(unidadeDeTrabalho);
                Servicos.NFe serNFe = new Servicos.NFe(unidadeDeTrabalho);

                var caminho = Utilidades.IO.FileStorageService.Storage.Combine(System.Configuration.ConfigurationManager.AppSettings["CaminhoArquivosXMLIntegracao"], string.Concat(identificadorXML, ".xml"));

                Stream stream = Utilidades.IO.FileStorageService.Storage.OpenRead(caminho);                
                dynamic xmlNotaFiscal = serNFe.ObterDocumentoPorXML(stream, transportador.Codigo, null, unidadeDeTrabalho);

                if (xmlNotaFiscal == null)
                    return new Retorno<bool>() { Mensagem = "XML NFe enviado não é valido", Status = false };

                string chaveNFe = (string)xmlNotaFiscal.GetType().GetProperty("Chave").GetValue(xmlNotaFiscal, null);
                string stringXmlNFe = Newtonsoft.Json.JsonConvert.SerializeObject(xmlNotaFiscal);

                Repositorio.XMLNotaFiscalEletronica repXMLNotaFiscalEletronica = new Repositorio.XMLNotaFiscalEletronica(unidadeDeTrabalho);
                Dominio.Entidades.XMLNotaFiscalEletronica notaImportada = repXMLNotaFiscalEletronica.BuscarPorChaveNFeEmpresa(chaveNFe, transportador.Codigo);

                if (notaImportada == null)
                {
                    Dominio.ObjetosDeValor.XMLNFe xmlNFe = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.XMLNFe>(stringXmlNFe);

                    notaImportada = new Dominio.Entidades.XMLNotaFiscalEletronica();
                    notaImportada.Chave = xmlNFe.Chave;                    
                    DateTime dataEmissao;
                    if (!DateTime.TryParseExact(xmlNFe.DataEmissao, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dataEmissao))
                        if (!DateTime.TryParseExact(xmlNFe.DataEmissao, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissao))
                            dataEmissao = DateTime.Now;
                    notaImportada.DataEmissao = dataEmissao;
                    notaImportada.Emitente = repCliente.BuscarPorCPFCNPJ(double.Parse(Utilidades.String.OnlyNumbers(xmlNFe.Remetente)));
                    notaImportada.Destinatario = repCliente.BuscarPorCPFCNPJ(double.Parse(Utilidades.String.OnlyNumbers(xmlNFe.Destinatario)));
                    notaImportada.Empresa = transportador;
                    notaImportada.FormaDePagamento = xmlNFe.FormaPagamento == "Pago" ? "0" : "1";
                    notaImportada.Numero = xmlNFe.Numero;
                    notaImportada.Peso = xmlNFe.Peso;
                    notaImportada.Valor = xmlNFe.ValorTotal;
                    notaImportada.GeradoDocumento = false;
                    notaImportada.ValorDoFrete = 0;// serNFSe.CalcularFretePorNotaImportada(notaImportada, transportador.Codigo, "", unitOfWork);
                    notaImportada.Volumes = (int)xmlNFe.Volume;
                    notaImportada.Pedido = xmlNFe.Pedido;
                    notaImportada.Modalidade = xmlNFe.Modalidade;
                    repXMLNotaFiscalEletronica.Inserir(notaImportada);
                }

                return new Retorno<bool>() { Mensagem = "NF-e disponibilizada para o transportador "+ transportador.Descricao, Status = true, Objeto = true };

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new Retorno<bool>() { Mensagem = "Não foi possível ler o XML da NFe.", Status = false };
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }



        #endregion

        #region Metodos Privados


        #endregion
    }
}
