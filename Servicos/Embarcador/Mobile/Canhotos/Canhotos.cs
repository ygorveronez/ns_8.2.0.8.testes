using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Servicos.Embarcador.Mobile.Canhotos
{
    public class Canhotos
    {
        #region Métodos Públicos

        public List<Dominio.ObjetosDeValor.Embarcador.Mobile.Canhotos.Canhoto> ObterCanhotosModificadosPorUltimaConsulta(DateTime dataUltimaConsulta, int usuarioAPP, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Mobile.Canhotos.Canhoto> canhotosMob = new List<Dominio.ObjetosDeValor.Embarcador.Mobile.Canhotos.Canhoto>();
            Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

            Dominio.Entidades.Usuario motorista = repUsuario.BuscarPorCodigoMobile(usuarioAPP);
            if (motorista != null)
            {
                List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotos = repCanhoto.ObterCanhotosModificadosPorUltimaConsulta(motorista.Codigo, dataUltimaConsulta);
                for (int i = 0; i < canhotos.Count; i++)
                    canhotosMob.Add(ConverterCanhoto(canhotos[i], clienteMultisoftware, unitOfWork));
            }
            return canhotosMob;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Mobile.Documentos.Documento> ObterDocumentosModificadosPorUltimaConsulta(DateTime dataUltimaConsulta, int usuarioAPP, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Mobile.Documentos.Documento> documentosMob = new List<Dominio.ObjetosDeValor.Embarcador.Mobile.Documentos.Documento>();
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

            Dominio.Entidades.Usuario motorista = repUsuario.BuscarPorCodigoMobile(usuarioAPP);
            if (motorista != null)
            {
                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = null;
                if (motorista.Cliente == null)
                    cargas = repCarga.BuscarCargaPorMotorista(motorista.Codigo, dataUltimaConsulta);
                else
                    cargas = repCarga.BuscarCargaPorClienteDestino(motorista.Cliente.CPF_CNPJ, dataUltimaConsulta);

                for (int i = 0; i < cargas.Count; i++)
                {
                    List<Dominio.ObjetosDeValor.Embarcador.Mobile.Documentos.Documento> retornoLista = ObterDocumentosPorCarga(cargas[i], clienteMultisoftware, motorista.Cliente, unitOfWork);
                    for (int k = 0; k < retornoLista.Count; k++)
                    {
                        documentosMob.Add(retornoLista[k]);
                    }
                }
            }
            return documentosMob;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Mobile.Documentos.Documento> ObterDocumentosPorCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.Entidades.Cliente clienteDestino, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Mobile.Documentos.Documento> documentosMob = new List<Dominio.ObjetosDeValor.Embarcador.Mobile.Documentos.Documento>();
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargasCTe = repCargaCTe.BuscarPorCargaEClienteDestino(carga.Codigo, clienteDestino?.CPF_CNPJ ?? 0);
            for (int i = 0; i < cargasCTe.Count; i++)
                documentosMob.Add(ConverterDocumento(cargasCTe[i], clienteMultisoftware, unitOfWork));

            documentosMob = documentosMob.OrderBy(obj => obj.OrdemEntrega).ToList();

            for (int i = 0; i < documentosMob.Count; i++)
                documentosMob[i].OrdemEntrega = i + 1;

            return documentosMob;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Mobile.Canhotos.Canhoto> ObterCanhotosPorCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Mobile.Canhotos.Canhoto> canhotosMob = new List<Dominio.ObjetosDeValor.Embarcador.Mobile.Canhotos.Canhoto>();
            Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);

            List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotos = repCanhoto.BuscarPorCarga(carga.Codigo);
            for (int i = 0; i < canhotos.Count; i++)
            {
                if (canhotos[i].TipoCanhoto != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.NFe || canhotos[i].XMLNotaFiscal.nfAtiva)
                    canhotosMob.Add(ConverterCanhoto(canhotos[i], clienteMultisoftware, unitOfWork));
            }

            return canhotosMob;
        }

        public Dominio.ObjetosDeValor.Embarcador.Mobile.Documentos.Documento ConverterDocumento(Dominio.Entidades.Embarcador.Cargas.CargaCTe documento, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Mobile.Cargas.Carga serCarga = new Cargas.Carga();
            Servicos.WebService.Pessoas.Pessoa serPessoa = new WebService.Pessoas.Pessoa(unitOfWork);
            Servicos.WebService.Empresa.Empresa serEmpresa = new WebService.Empresa.Empresa(unitOfWork);
            Servicos.WebService.Filial.Filial serFilial = new WebService.Filial.Filial(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);


            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedidoXMLNotaFiscalCTe.BuscarCargaPedidoPorCargaCTe(documento.Codigo);

            Dominio.ObjetosDeValor.Embarcador.Mobile.Documentos.Documento documentooMob = new Dominio.ObjetosDeValor.Embarcador.Mobile.Documentos.Documento
            {
                CodigoIntegracao = documento.CTe.Codigo,
                Carga = serCarga.ConverterCarga(documento.Carga, clienteMultisoftware, false, unitOfWork),
                Destinatario = serPessoa.ConverterObjetoPessoa(documento.CTe.Destinatario.Cliente),
                Emitente = serPessoa.ConverterObjetoPessoa(documento.CTe.Remetente.Cliente),
                Numero = documento.CTe.Numero,
                Serie = documento.CTe.Serie.Numero,
                OrdemEntrega = cargaPedidos.FirstOrDefault()?.OrdemEntrega ?? 0,
                NumeroNF = string.Join(",", (from obj in documento.CTe.XMLNotaFiscais select obj.Numero).ToList()),
                NumeroPedido = cargaPedidos.FirstOrDefault()?.Pedido.NumeroPedidoEmbarcador ?? ""
            };

            return documentooMob;
        }

        public Dominio.ObjetosDeValor.Embarcador.Mobile.Canhotos.Canhoto ConverterCanhoto(Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Mobile.Cargas.Carga serCarga = new Cargas.Carga();
            Servicos.WebService.Pessoas.Pessoa serPessoa = new WebService.Pessoas.Pessoa(unitOfWork);
            Servicos.WebService.Empresa.Empresa serEmpresa = new WebService.Empresa.Empresa(unitOfWork);
            Servicos.WebService.Filial.Filial serFilial = new WebService.Filial.Filial(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Mobile.Canhotos.Canhoto canhotoMob = new Dominio.ObjetosDeValor.Embarcador.Mobile.Canhotos.Canhoto();
            canhotoMob.CodigoIntegracao = canhoto.Codigo;
            if (clienteMultisoftware != null)
                canhotoMob.Carga = serCarga.ConverterCarga(canhoto.Carga, clienteMultisoftware, false, unitOfWork);

            canhotoMob.DataEmissao = canhoto.DataEmissao.ToString("dd/MM/yyyy HH:mm:ss");
            canhotoMob.DataEnvioCanhoto = canhoto.DataEnvioCanhoto.ToString("dd/MM/yyyy HH:mm:ss");
            canhotoMob.Destinatario = serPessoa.ConverterObjetoPessoa(canhoto.Destinatario);
            canhotoMob.Emitente = serPessoa.ConverterObjetoPessoa(canhoto.Emitente);
            canhotoMob.Empresa = serEmpresa.ConverterObjetoEmpresa(canhoto.Empresa);
            canhotoMob.Filial = serFilial.ConverterObjetoFilial(canhoto.Filial);
            if (canhoto.TipoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.NFe)
            {
                canhotoMob.Identificacao = canhoto.XMLNotaFiscal.Chave;
                canhotoMob.Descricao = canhoto.XMLNotaFiscal.Chave;
            }
            else if (canhoto.TipoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.Avulso)
            {
                canhotoMob.Identificacao = canhoto.CanhotoAvulso.QRCode;
                canhotoMob.Descricao = "Canhoto avulso " + canhoto.CanhotoAvulso.Numero.ToString();
            }
            else if (canhoto.TipoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.CTeSubcontratacao)
            {
                canhotoMob.Identificacao = canhoto.CTeSubcontratacao.ChaveAcesso;
                canhotoMob.Descricao = canhoto.CTeSubcontratacao.ChaveAcesso;
            }
            canhotoMob.MotivoRejeicaoDigitalizacao = canhoto.MotivoRejeicaoDigitalizacao;
            canhotoMob.Numero = canhoto.Numero;
            canhotoMob.Observacao = canhoto.Observacao;
            canhotoMob.Peso = canhoto.Peso;
            canhotoMob.SituacaoCanhoto = canhoto.SituacaoCanhoto;
            canhotoMob.SituacaoDigitalizacaoCanhoto = canhoto.SituacaoDigitalizacaoCanhoto;
            canhotoMob.TipoCanhoto = canhoto.TipoCanhoto;
            canhotoMob.Valor = canhoto.Valor;
            canhotoMob.DataEntregaNotaCliente = canhoto.DataEntregaNotaCliente?.ToString("dd/MM/yyyy") ?? string.Empty;
            canhotoMob.DigitalizacaoCanhotoInteiro = canhoto.XMLNotaFiscal?.Emitente?.DigitalizacaoCanhotoInteiro ?? false;

            return canhotoMob;
        }

        public string EnviarJustificativaCanhoto(string latitude, string longitude, int codigoCanhoto, string Juntificativa, int usuarioAPP, Repositorio.UnitOfWork unitOfWork)
        {
            string retorno = "";
            Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
            Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = repCanhoto.BuscarPorCodigo(codigoCanhoto);
            Servicos.Embarcador.Canhotos.Canhoto serCanhoto = new Embarcador.Canhotos.Canhoto(unitOfWork);

            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

            Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorCodigoMobile(usuarioAPP);
            if (usuario != null)
            {
                if (canhoto != null)
                {
                    if (canhoto.SituacaoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.Pendente)
                    {
                        //justificativa enviada pelo transportador teria que pensar em alguma aprovação etc antes de liberar o canhoto.
                        canhoto.SituacaoCanhoto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.Justificado;
                        canhoto.Observacao = Juntificativa;
                        canhoto.DataUltimaModificacao = DateTime.Now;
                        canhoto.Latitude = latitude;
                        canhoto.Longitude = longitude;
                        repCanhoto.Atualizar(canhoto);
                        serCanhoto.GerarHistoricoCanhoto(canhoto, usuario, "Justificativa enviada via dispositivel móvel pelo motorista.", unitOfWork);
                    }
                    else
                    {
                        retorno = "A atual situação do canhoto (" + canhoto.DescricaoSituacao + ") não permite que ele seja justificativa.";
                    }
                }
                else
                {
                    retorno = "O codigo informado não pertence a um canhoto válido";
                }
            }
            else
            {
                retorno = "O usuário cadastrado não está cadastrado na Empresa";
            }

            return retorno;
        }

        public string EnviarImagemCanhotoLeituraOCR(string tokenImagem, int idUsuario, int codigoClienteMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            string retorno = "";
            Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
            Servicos.Embarcador.Canhotos.Canhoto serCanhoto = new Embarcador.Canhotos.Canhoto(unitOfWork);

            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
            string extensao = ".jpg";
            string caminhoTemp = Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoTempArquivosImportacao;

            string[] nomeSplit = tokenImagem.Split('_');

            string fileLocationTemp = Utilidades.IO.FileStorageService.Storage.Combine(caminhoTemp, nomeSplit[2]);

            Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorCodigo(idUsuario);
            if (usuario != null)
            {
                if (Utilidades.IO.FileStorageService.Storage.Exists(fileLocationTemp))
                {
                    using (System.IO.StreamReader reader = new StreamReader(Utilidades.IO.FileStorageService.Storage.OpenRead(fileLocationTemp)))
                    {
                        string caminhoRaiz = Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRaiz;
                        string caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminhoRaiz, "Canhotos", "Processados");
                        string fileLocation = Utilidades.IO.FileStorageService.Storage.Combine(caminho, tokenImagem + extensao);

                        using (System.Drawing.Image t = System.Drawing.Image.FromStream(reader.BaseStream))
                        {
                            Utilidades.IO.FileStorageService.Storage.SaveImage(fileLocation, t);
                        }
                    }
                }
                else
                {
                    retorno = "A imagem não foi enviada para o servidor.";
                }
            }
            else
            {
                retorno = "O usuário cadastrado não está cadastrado na Empresa";
            }

            if (Utilidades.IO.FileStorageService.Storage.Exists(fileLocationTemp))
                Utilidades.IO.FileStorageService.Storage.Delete(fileLocationTemp);

            return retorno;
        }

        public string EnviarImagemCanhoto(string latitude, string longitude, int codigoCanhoto, string tokenImagem, int idUsuario, int codigoClienteMultisoftware, DateTime? dataEntregaNotaCliente, Repositorio.UnitOfWork unitOfWork, string chaveNFe, int numeroNota, int serieNota, string cnpjCpfEmissor)
        {
            string retorno = "";
            Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();


            Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = null;
            if (codigoCanhoto > 0)
                canhoto = repCanhoto.BuscarPorCodigo(codigoCanhoto);
            else if (!string.IsNullOrWhiteSpace(chaveNFe))
                canhoto = repCanhoto.BuscarPorChave(chaveNFe);
            else if (numeroNota > 0 && serieNota > 0 && !string.IsNullOrWhiteSpace(cnpjCpfEmissor))
            {
                string serie = Utilidades.String.OnlyNumbers(serieNota.ToString("n0"));
                double.TryParse(cnpjCpfEmissor, out double cnpjEmissor);
                canhoto = repCanhoto.BuscarPorNumeroSerieEmitente(numeroNota, serie, cnpjEmissor);
                if (canhoto == null)
                    canhoto = repCanhoto.BuscarPorNumeroSerieEmitenteNFe(numeroNota, serie, cnpjEmissor);
            }

            Servicos.Embarcador.Canhotos.Canhoto serCanhoto = new Embarcador.Canhotos.Canhoto(unitOfWork);

            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
            string extensao = ".jpg";
            string caminhoTemp = Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoTempArquivosImportacao;
            string fileLocationTemp = Utilidades.IO.FileStorageService.Storage.Combine(caminhoTemp, tokenImagem + extensao);

            Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorCodigo(idUsuario);
            if (usuario != null)
            {
                if (canhoto != null)
                {
                    if (Utilidades.IO.FileStorageService.Storage.Exists(fileLocationTemp))
                    {
                        using System.IO.StreamReader reader = new StreamReader(Utilidades.IO.FileStorageService.Storage.OpenRead(fileLocationTemp));

                        if (canhoto.SituacaoDigitalizacaoCanhoto != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto.Digitalizado)
                        {
                            canhoto.Latitude = latitude;
                            canhoto.Longitude = longitude;
                            canhoto.Observacao = "";
                            canhoto.GuidNomeArquivo = tokenImagem;
                            canhoto.NomeArquivo = "MI_" + codigoCanhoto + "_" + codigoClienteMultisoftware + extensao;
                            canhoto.DataEnvioCanhoto = DateTime.Now;
                            canhoto.DataDigitalizacao = DateTime.Now;
                            canhoto.DataEntregaNotaCliente = dataEntregaNotaCliente;
                            canhoto.SituacaoDigitalizacaoCanhoto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto.AgAprovocao;
                            canhoto.SituacaoPgtoCanhoto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPgtoCanhoto.Pendente;
                            canhoto.MotivoRejeicaoDigitalizacao = string.Empty;

                            string caminho = Embarcador.Canhotos.Canhoto.CaminhoCanhoto(canhoto, unitOfWork);

                            string fileLocation = Utilidades.IO.FileStorageService.Storage.Combine(caminho, canhoto.GuidNomeArquivo + extensao);

                            using (System.Drawing.Image t = System.Drawing.Image.FromStream(reader.BaseStream))
                            {
                                Utilidades.IO.FileStorageService.Storage.SaveImage(fileLocation, t);
                            }
                            if (!Utilidades.IO.FileStorageService.Storage.Exists(fileLocation))
                            {
                                Servicos.Log.TratarErro($"Erro02 - Falha ao salvar imagem do Canhoto de id {canhoto.Codigo} e guid {canhoto.GuidNomeArquivo}.", "EnviarCanhoto");
                                retorno = "Erro02 - Falha ao salvar imagem do Canhoto.";
                            }

                            canhoto.DataUltimaModificacao = DateTime.Now;

                            repCanhoto.Atualizar(canhoto);

                            serCanhoto.GerarHistoricoCanhoto(canhoto, usuario, "Imagem do canhoto enviada via web service.", unitOfWork);

                            Servicos.Embarcador.Canhotos.Canhoto.CanhotoAgAprovacao(canhoto, configuracao, unitOfWork);
                        }
                        else
                        {
                            retorno = "A imagem já foi digitalizada anteriormente.";
                        }
                    }
                    else
                    {
                        retorno = "A imagem não foi enviada para o servidor.";
                    }
                }
                else
                {
                    retorno = "Não foi localizado o canhoto pelos parametros repassados.";
                }
            }
            else
            {
                retorno = "O usuário cadastrado não está cadastrado na Empresa";
            }

            if (Utilidades.IO.FileStorageService.Storage.Exists(fileLocationTemp))
                Utilidades.IO.FileStorageService.Storage.Delete(fileLocationTemp);

            return retorno;
        }

        public string SalvarImagemCanhoto(Stream imagem, out string tokenImagem, Repositorio.UnitOfWork unitOfWork)
        {
            tokenImagem = "";
            string retorno = "";

            const int bufferSize = 16 * 1024;
            byte[] buffer = new byte[bufferSize];
            using MemoryStream ms = new MemoryStream();

            int read;
            while ((read = imagem.Read(buffer, 0, buffer.Length)) > 0)
                ms.Write(buffer, 0, read);

            ms.Position = 0;

            string extensao = ".jpg";

            if (DetectarFormatoPDF(ms))
            {
                extensao = ".pdf";
            }

            if (extensao.Equals(".jpg") || extensao.Equals(".jpeg") || extensao.Equals(".pdf"))
            {
                string token = Guid.NewGuid().ToString("N");
                string caminho = Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo()?.CaminhoTempArquivosImportacao ?? string.Empty;
                string fileLocation = Utilidades.IO.FileStorageService.Storage.Combine(caminho, token + extensao);

                tokenImagem = token;

                if (extensao.Equals(".jpg"))
                {
                    using (System.Drawing.Image t = Image.FromStream(ms))
                    {
                        Utilidades.IO.FileStorageService.Storage.SaveImage(fileLocation, t);
                    }

                }
                else if (extensao.Equals(".pdf"))
                {
                    try
                    {
                        using (Stream arquivoPDF = ms)
                        {
                            using (Stream fileStream = Utilidades.IO.FileStorageService.Storage.OpenWrite(fileLocation))
                            {
                                arquivoPDF.CopyTo(fileStream);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        retorno = $"Erro ao processar o PDF: {ex.Message}";
                    }
                }
            }
            else
            {
                retorno = "A extensão do arquivo é inválida.";
            }

            return retorno;
        }

        #endregion

        #region Métodos Privados

        private bool DetectarFormatoPDF(Stream stream)
        {
            byte[] pdfSignature = { 0x25, 0x50, 0x44, 0x46 };

            byte[] header = new byte[pdfSignature.Length];
            stream.Read(header, 0, pdfSignature.Length);

            for (int i = 0; i < pdfSignature.Length; i++)
            {
                if (header[i] != pdfSignature[i])
                {
                    stream.Position = 0;
                    return false;
                }
            }

            stream.Position = 0;
            return true;
        }

        #endregion
    }
}
