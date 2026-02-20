using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using CoreWCF;


namespace SGT.WebService
{
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any, IncludeExceptionDetailInFaults = true)]
    public class EmissaoNFe(IServiceProvider _serviceProvider) : IEmissaoNFe
    {

        #region Métodos de Busca

        public Retorno<List<Dominio.ObjetosDeValor.WebService.EmissaoNFe.EmissaoNFe>> BuscarNotasAguardandoAssinatura(string cnpjEmpresa)
        {
            Retorno<List<Dominio.ObjetosDeValor.WebService.EmissaoNFe.EmissaoNFe>> retorno = new Retorno<List<Dominio.ObjetosDeValor.WebService.EmissaoNFe.EmissaoNFe>>();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                Repositorio.Embarcador.NotaFiscal.NotaFiscalArquivos repNotaFiscalArquivos = new Repositorio.Embarcador.NotaFiscal.NotaFiscalArquivos(unitOfWork);
                List<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalArquivos> listaArquivos = repNotaFiscalArquivos.BuscarArquivosParaAssinar(cnpjEmpresa);

                retorno.Status = true;
                retorno.Objeto = new List<Dominio.ObjetosDeValor.WebService.EmissaoNFe.EmissaoNFe>();
                for (int i = 0; i < listaArquivos.Count(); i++)
                {
                    Dominio.ObjetosDeValor.WebService.EmissaoNFe.EmissaoNFe obj = new Dominio.ObjetosDeValor.WebService.EmissaoNFe.EmissaoNFe();
                    obj.CNPJEmpresa = cnpjEmpresa;
                    obj.CodigoNFe = listaArquivos[i].NotaFiscal.Codigo;
                    obj.CIdToken = listaArquivos[i].NotaFiscal.Empresa.IdTokenNFCe;
                    obj.Csc = listaArquivos[i].NotaFiscal.Empresa.IdCSCNFCe;

                    byte[] xmlNFe = null;
                    xmlNFe = Encoding.ASCII.GetBytes(listaArquivos[i].XMLNaoAssinado);

                    obj.XMLNaoAssinado = Compacta(Convert.ToBase64String(Encoding.Convert(Encoding.GetEncoding("ISO-8859-1"), Encoding.UTF8, xmlNFe)));
                    obj.ReciboAnterior = listaArquivos[i].NotaFiscal.NumeroRecibo;
                    obj.CodigoStatusAnterior = listaArquivos[i].NotaFiscal.UltimoStatus;


                    retorno.Objeto.Add(obj);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao consultar as notas fiscais aguardando assinatura.";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            return retorno;
        }

        public Retorno<List<Dominio.ObjetosDeValor.WebService.EmissaoNFe.EmissaoNFe>> BuscarNotasAguardandoInutilizacao(string cnpjEmpresa)
        {
            Retorno<List<Dominio.ObjetosDeValor.WebService.EmissaoNFe.EmissaoNFe>> retorno = new Retorno<List<Dominio.ObjetosDeValor.WebService.EmissaoNFe.EmissaoNFe>>();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                Repositorio.Embarcador.NotaFiscal.NotaFiscalArquivos repNotaFiscalArquivos = new Repositorio.Embarcador.NotaFiscal.NotaFiscalArquivos(unitOfWork);
                List<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalArquivos> listaArquivos = repNotaFiscalArquivos.BuscarArquivosParaInutilizar(cnpjEmpresa);

                retorno.Status = true;
                retorno.Objeto = new List<Dominio.ObjetosDeValor.WebService.EmissaoNFe.EmissaoNFe>();
                for (int i = 0; i < listaArquivos.Count(); i++)
                {
                    Dominio.ObjetosDeValor.WebService.EmissaoNFe.EmissaoNFe obj = new Dominio.ObjetosDeValor.WebService.EmissaoNFe.EmissaoNFe();
                    obj.CNPJEmpresa = cnpjEmpresa;
                    obj.CodigoNFe = listaArquivos[i].NotaFiscal.Codigo;
                    obj.XMLNaoAssinado = listaArquivos[i].XMLInutilizacaoNaoAssinado;
                    obj.ReciboAnterior = listaArquivos[i].NotaFiscal.NumeroRecibo;
                    obj.CodigoStatusAnterior = listaArquivos[i].NotaFiscal.UltimoStatus;
                    obj.XMLDistribuicao = listaArquivos[i].XMLDistribuicao;
                    obj.UFEmpresa = listaArquivos[i].NotaFiscal.Empresa.Localidade.Estado.CodigoIBGE;
                    obj.TipoAmbiente = (int)listaArquivos[i].NotaFiscal.TipoAmbiente;
                    obj.Modelo = listaArquivos[i].NotaFiscal.ModeloNotaFiscal;

                    retorno.Objeto.Add(obj);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao consultar as notas fiscais aguardando Inutilização.";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            return retorno;
        }

        public Retorno<List<Dominio.ObjetosDeValor.WebService.EmissaoNFe.EmissaoNFe>> BuscarNotasAguardandoCancelamento(string cnpjEmpresa)
        {
            Retorno<List<Dominio.ObjetosDeValor.WebService.EmissaoNFe.EmissaoNFe>> retorno = new Retorno<List<Dominio.ObjetosDeValor.WebService.EmissaoNFe.EmissaoNFe>>();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                Repositorio.Embarcador.NotaFiscal.NotaFiscalArquivos repNotaFiscalArquivos = new Repositorio.Embarcador.NotaFiscal.NotaFiscalArquivos(unitOfWork);
                List<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalArquivos> listaArquivos = repNotaFiscalArquivos.BuscarArquivosParaCancelar(cnpjEmpresa);

                retorno.Status = true;
                retorno.Objeto = new List<Dominio.ObjetosDeValor.WebService.EmissaoNFe.EmissaoNFe>();
                for (int i = 0; i < listaArquivos.Count(); i++)
                {
                    Dominio.ObjetosDeValor.WebService.EmissaoNFe.EmissaoNFe obj = new Dominio.ObjetosDeValor.WebService.EmissaoNFe.EmissaoNFe();
                    obj.CNPJEmpresa = cnpjEmpresa;
                    obj.CodigoNFe = listaArquivos[i].NotaFiscal.Codigo;
                    obj.XMLNaoAssinado = listaArquivos[i].XMLCancelamentoNaoAssinado;
                    obj.ReciboAnterior = listaArquivos[i].NotaFiscal.NumeroRecibo;
                    obj.CodigoStatusAnterior = listaArquivos[i].NotaFiscal.UltimoStatus;

                    byte[] xmlNFe = null;
                    xmlNFe = Encoding.ASCII.GetBytes(listaArquivos[i].XMLDistribuicao);
                    obj.XMLDistribuicao = Compacta(Convert.ToBase64String(Encoding.Convert(Encoding.GetEncoding("ISO-8859-1"), Encoding.UTF8, xmlNFe)));

                    retorno.Objeto.Add(obj);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao consultar as notas fiscais aguardando Cancelamento.";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            return retorno;
        }

        public Retorno<List<Dominio.ObjetosDeValor.WebService.EmissaoNFe.EmissaoNFe>> BuscarNotasAguardandoCartaCorrecao(string cnpjEmpresa)
        {
            Retorno<List<Dominio.ObjetosDeValor.WebService.EmissaoNFe.EmissaoNFe>> retorno = new Retorno<List<Dominio.ObjetosDeValor.WebService.EmissaoNFe.EmissaoNFe>>();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                Repositorio.Embarcador.NotaFiscal.NotaFiscalArquivos repNotaFiscalArquivos = new Repositorio.Embarcador.NotaFiscal.NotaFiscalArquivos(unitOfWork);
                List<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalArquivos> listaArquivos = repNotaFiscalArquivos.BuscarArquivosParaCartaCorrecao(cnpjEmpresa);

                retorno.Status = true;
                retorno.Objeto = new List<Dominio.ObjetosDeValor.WebService.EmissaoNFe.EmissaoNFe>();
                for (int i = 0; i < listaArquivos.Count(); i++)
                {
                    Dominio.ObjetosDeValor.WebService.EmissaoNFe.EmissaoNFe obj = new Dominio.ObjetosDeValor.WebService.EmissaoNFe.EmissaoNFe();
                    obj.CNPJEmpresa = cnpjEmpresa;
                    obj.CodigoNFe = listaArquivos[i].NotaFiscal.Codigo;
                    obj.XMLNaoAssinado = listaArquivos[i].XMLCartaCorrecaoNaoAssinado;
                    obj.ReciboAnterior = listaArquivos[i].NotaFiscal.NumeroRecibo;
                    obj.CodigoStatusAnterior = listaArquivos[i].NotaFiscal.UltimoStatus;
                    obj.XMLDistribuicao = listaArquivos[i].XMLDistribuicao;

                    retorno.Objeto.Add(obj);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao consultar as notas fiscais aguardando Carta de Correção.";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            return retorno;
        }

        #endregion

        #region Métodos de Envio


        public Retorno<string> SalvarRetornoEnvioNFe(Dominio.ObjetosDeValor.Embarcador.NotaFiscal.RetornoEnvioNFe ret, int codigoNFe)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                unitOfWork.Start();
                Retorno<string> retorno = new Retorno<string>();

                var z = new Zeus.Embarcador.ZeusNFe.Zeus();
                string xmlAssinado = "";

                if (ret.TipoArquivoXML == Dominio.Enumeradores.TipoArquivoXML.Cancelamento || ret.TipoArquivoXML == Dominio.Enumeradores.TipoArquivoXML.CartaCorrecao || ret.TipoArquivoXML == Dominio.Enumeradores.TipoArquivoXML.Inutilizacao)
                    xmlAssinado = ret.XML;
                else
                {
                    xmlAssinado = Descompacta(ret.XML);
                    byte[] xmlNFe = System.Text.Encoding.Convert(System.Text.Encoding.UTF8, System.Text.Encoding.GetEncoding("ISO-8859-1"), Convert.FromBase64String(xmlAssinado));
                    xmlAssinado = System.Text.Encoding.Default.GetString(xmlNFe);
                }

                ret.XML = xmlAssinado;

                string mensagem = z.SalvarRetornoEnvioNFe(ret, codigoNFe, unitOfWork);

                retorno.Status = true;
                retorno.DataRetorno = DateTime.Now.Date.ToString("dd/MM/yyyy HH:mm:ss");
                retorno.Objeto = mensagem;

                unitOfWork.CommitChanges();
                return retorno;
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new Retorno<string>() { Mensagem = "Ocorreu uma falha ao salvar retorno da nota fiscal.", Status = false, CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica };
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        #endregion

        public static string Compacta(string text)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(text);
            MemoryStream ms = new MemoryStream();
            using (GZipStream zip = new GZipStream(ms, CompressionMode.Compress, true))
            {
                zip.Write(buffer, 0, buffer.Length);
            }

            ms.Position = 0;
            MemoryStream outStream = new MemoryStream();

            byte[] compressed = new byte[ms.Length];
            ms.Read(compressed, 0, compressed.Length);

            byte[] gzBuffer = new byte[compressed.Length + 4];
            System.Buffer.BlockCopy(compressed, 0, gzBuffer, 4, compressed.Length);
            System.Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gzBuffer, 0, 4);
            return Convert.ToBase64String(gzBuffer);
        }

        public static string Descompacta(string compressedText)
        {
            byte[] gzBuffer = Convert.FromBase64String(compressedText);
            using (MemoryStream ms = new MemoryStream())
            {
                int msgLength = BitConverter.ToInt32(gzBuffer, 0);
                ms.Write(gzBuffer, 4, gzBuffer.Length - 4);

                byte[] buffer = new byte[msgLength];

                ms.Position = 0;
                using (GZipStream zip = new GZipStream(ms, CompressionMode.Decompress))
                {
                    zip.Read(buffer, 0, buffer.Length);
                }
                return Encoding.UTF8.GetString(buffer);
            }
        }
    }
}
