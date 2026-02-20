using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.IO;

namespace Servicos.Embarcador.Mobile.Ocorrencias
{
    public class Ocorrencia
    {

        public List<Dominio.ObjetosDeValor.Embarcador.Mobile.Ocorrencias.MotivoOcorrencia> BuscarMotivosOcorrencia(DateTime dataUltimaVerificacao, int usuarioAPP, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftare, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Mobile.Ocorrencias.MotivoOcorrencia> motivoMob = new List<Dominio.ObjetosDeValor.Embarcador.Mobile.Ocorrencias.MotivoOcorrencia>();
            Repositorio.TipoDeOcorrenciaDeCTe repTipoDeOcorrenciaDeCTe = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

            Dominio.Entidades.Usuario motorista = repUsuario.BuscarPorCodigoMobile(usuarioAPP);
            if (motorista != null)
            {
                List<Dominio.Entidades.TipoDeOcorrenciaDeCTe> tipos = repTipoDeOcorrenciaDeCTe.BuscarTipoOcorrenciaMobile();
                for (int i = 0; i < tipos.Count; i++)
                    motivoMob.Add(ConverterMotivoOcorrencia(tipos[i], clienteMultisoftare, false, unitOfWork));

            }
            return motivoMob;
        }

        public Dominio.ObjetosDeValor.Embarcador.Mobile.Ocorrencias.MotivoOcorrencia ConverterMotivoOcorrencia(Dominio.Entidades.TipoDeOcorrenciaDeCTe tipo, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftare, bool buscarCanhoto, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Mobile.Ocorrencias.MotivoOcorrencia motivoMob = new Dominio.ObjetosDeValor.Embarcador.Mobile.Ocorrencias.MotivoOcorrencia();

            motivoMob.CodigoIntegracao = tipo.Codigo;
            motivoMob.Descricao = tipo.Descricao;
            motivoMob.Tipo = tipo.Tipo;

            return motivoMob;
        }

        public int EnviarOcorrencia(int usuarioAPP, int ocorrencia, int empresaMultisoftware, DateTime dataOcorrencia, int motivo, string observacao, string latitude, string longitude, Repositorio.UnitOfWork unitOfWork)
        {
            unitOfWork.Start();
            try
            {
                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
                Repositorio.TipoDeOcorrenciaDeCTe repTipoDeOcorrenciaDeCTe = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia = repCargaOcorrencia.BuscarPorCodigo(ocorrencia);
                bool inserir = false;
                if (cargaOcorrencia == null)
                {
                    inserir = true;
                    cargaOcorrencia = new Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia();
                }

                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

                Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorCodigoMobile(usuarioAPP);
                if (usuario != null)
                {
                    cargaOcorrencia.DataAlteracao = dataOcorrencia;
                    cargaOcorrencia.DataOcorrencia = dataOcorrencia;
                    cargaOcorrencia.DataFinalizacaoEmissaoOcorrencia = dataOcorrencia;
                    cargaOcorrencia.NumeroOcorrencia = Servicos.Embarcador.CargaOcorrencia.OcorrenciaSequencial.GetInstance().ObterProximoNumeroSequencial(unitOfWork);
                    if (!string.IsNullOrWhiteSpace(observacao))
                    {
                        cargaOcorrencia.Observacao = observacao;
                        cargaOcorrencia.ObservacaoCTe = observacao;
                    }
                    else
                    {
                        cargaOcorrencia.Observacao = "Ocorrência gerada pelo Multi Mobile.";
                        cargaOcorrencia.ObservacaoCTe = "Ocorrência gerada pelo Multi Mobile.";
                    }
                    cargaOcorrencia.Latitude = latitude;
                    cargaOcorrencia.Longitude = longitude;
                    cargaOcorrencia.SituacaoOcorrencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Finalizada;
                    cargaOcorrencia.ValorOcorrencia = 0;
                    cargaOcorrencia.ValorOcorrenciaOriginal = 0;
                    cargaOcorrencia.TipoOcorrencia = repTipoDeOcorrenciaDeCTe.BuscarPorCodigo(motivo);
                    cargaOcorrencia.OrigemOcorrencia = cargaOcorrencia.TipoOcorrencia.OrigemOcorrencia;

                    if (inserir)
                        repCargaOcorrencia.Inserir(cargaOcorrencia);
                    else
                        repCargaOcorrencia.Atualizar(cargaOcorrencia);

                }
                else
                {
                    return 0;
                }

                unitOfWork.CommitChanges();
                return cargaOcorrencia.Codigo;
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                throw;
            }
            finally
            {

            }
        }

        public bool EnviarDocumentoOcorrencia(int ocorrencia, int codigoDocumento, string documentoRecebedor, string nomeRecebedor, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
        {
            unitOfWork.Start();
            try
            {
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento repCargaOcorrenciaDocumento = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento(unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.OcorrenciaDeCTe repOcorrenciaDeCTe = new Repositorio.OcorrenciaDeCTe(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia = repOcorrencia.BuscarPorCodigo(ocorrencia);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repositorioConfiguracaoTMS.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = null;
                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = null;
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();


                if (cargaOcorrencia == null)
                {
                    Servicos.Log.TratarErro("Ocorrencia não encontrada");
                    return false;
                }

                Dominio.Entidades.Embarcador.Chamados.Chamado chamado = null;

                //List<Dominio.Entidades.Embarcador.Cargas.CargaCTe>  listaCargaCTe =  new List<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
                if (configuracaoTMS.AppUtilizaControleColetaEntrega)
                {
                    double lat = 0;
                    double lng = 0;

                    if (cargaOcorrencia.Latitude != string.Empty && cargaOcorrencia.Longitude != string.Empty)
                    {
                        lat = double.Parse(((string)cargaOcorrencia.Latitude).Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);
                        lng = double.Parse(((string)cargaOcorrencia.Longitude).Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);
                    }

                    OrigemSituacaoEntrega origemSituacaoEntrega = (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador) ?
                                OrigemSituacaoEntrega.UsuarioMultiEmbarcador : OrigemSituacaoEntrega.UsuarioPortalTransportador;

                    Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.RejeitarEntregaParametros parametros = new Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.RejeitarEntregaParametros()
                    {
                        codigoCargaEntrega = codigoDocumento,
                        data = DateTime.Now,
                        wayPoint = new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint { Latitude = lat, Longitude = lng },
                        tipoServicoMultisoftware = tipoServicoMultisoftware,
                        observacao = "",
                        configuracao = configuracaoTMS,
                        OrigemSituacaoEntrega = origemSituacaoEntrega,
                        clienteMultisoftware = clienteMultisoftware

                    };

                    Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.RejeitarEntrega(parametros, null, unitOfWork, out chamado, tipoServicoMultisoftware);

                    Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                    cargaEntrega = repCargaEntrega.BuscarPorCodigo(codigoDocumento);
                    cargaOcorrencia.CargaEntrega = cargaEntrega;
                    cargaOcorrencia.Carga = cargaEntrega.Carga;

                    //List<int> listaNotasFiscais = (from notaFiscal in cargaEntrega.NotasFiscais select notaFiscal.PedidoXMLNotaFiscal.Codigo).ToList();

                    //listaCargaCTe = repCargaCte.BuscarPorPedidosNotasFiscais(listaNotasFiscais);
                }
                else
                {
                    cargaCTe = repCargaCte.BuscarPorCTe(codigoDocumento);
                    cargaPedidos = repCargaPedidoXMLNotaFiscalCTe.BuscarCargaPedidoPorCargaCTe(cargaCTe.Codigo);
                    cargaOcorrencia.Carga = cargaCTe?.Carga;
                }


                repOcorrencia.Atualizar(cargaOcorrencia);

                if (cargaCTe != null)
                {
                    Dominio.Entidades.OcorrenciaDeCTe ocorrenciaDeCTe = new Dominio.Entidades.OcorrenciaDeCTe();
                    ocorrenciaDeCTe.CTe = repCTe.BuscarPorCodigo(codigoDocumento);
                    ocorrenciaDeCTe.DataDaOcorrencia = cargaOcorrencia.DataOcorrencia;
                    ocorrenciaDeCTe.DataDeCadastro = DateTime.Now;
                    ocorrenciaDeCTe.Observacao = cargaOcorrencia.Observacao;
                    ocorrenciaDeCTe.Ocorrencia = cargaOcorrencia.TipoOcorrencia;

                    repOcorrenciaDeCTe.Inserir(ocorrenciaDeCTe);

                    Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento cargaOcorrenciaDocumento = new Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento();
                    cargaOcorrenciaDocumento.CargaCTe = cargaCTe;
                    cargaOcorrenciaDocumento.CargaOcorrencia = cargaOcorrencia;
                    cargaOcorrenciaDocumento.CTeImportado = repCTe.BuscarPorCodigo(codigoDocumento);
                    cargaOcorrenciaDocumento.OcorrenciaDeCTe = ocorrenciaDeCTe;

                    repCargaOcorrenciaDocumento.Inserir(cargaOcorrenciaDocumento);
                }

                unitOfWork.CommitChanges();

                if (chamado != null)
                    new Chamado.Chamado(unitOfWork).EnviarEmailChamadoAberto(chamado, unitOfWork);

                if ((cargaCTe == null) && (!configuracaoTMS.AppUtilizaControleColetaEntrega))
                {
                    Servicos.Log.TratarErro("Documento não encontrado");
                    return false;
                }

                return true;

            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return false;
            }
        }

        private string retornarCaminhoOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaImagem cargaOcorrenciaImagem, Repositorio.UnitOfWork unitOfWork)
        {
            string caminho = "";
            caminho = Utilidades.IO.FileStorageService.Storage.Combine(Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo()?.CaminhoOcorrencias, "OcorrenciasMobiles");
            return caminho;
        }

        public string EnviarImagemOcorrencia(int codigoOcorrencia, string tokenImagem, int idUsuario, int codigoClienteMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            unitOfWork.Start();
            try
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repositorioConfiguracaoTMS.BuscarConfiguracaoPadrao();

                string retorno = "";
                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaImagem repCargaOcorrenciaImagem = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaImagem(unitOfWork);
                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia = repCargaOcorrencia.BuscarPorCodigo(codigoOcorrencia);

                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                string extensao = ".jpg";
                string caminhoTemp = Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoTempArquivosImportacao;
                string fileLocationTemp = Utilidades.IO.FileStorageService.Storage.Combine(caminhoTemp, tokenImagem);

                Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorCodigo(idUsuario);
                if (usuario != null)
                {
                    if (cargaOcorrencia != null)
                    {
                        if (Utilidades.IO.FileStorageService.Storage.Exists(fileLocationTemp))
                        {
                            Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaImagem cargaOcorrenciaImagem = new Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaImagem();

                            cargaOcorrenciaImagem.CargaOcorrencia = cargaOcorrencia;
                            cargaOcorrenciaImagem.GuidNomeArquivo = tokenImagem;
                            cargaOcorrenciaImagem.NomeArquivo = "MI_" + tokenImagem + "_" + codigoClienteMultisoftware + extensao;

                            string caminho = retornarCaminhoOcorrencia(cargaOcorrenciaImagem, unitOfWork);

                            string fileLocation = Utilidades.IO.FileStorageService.Storage.Combine(caminho, cargaOcorrenciaImagem.GuidNomeArquivo + extensao);

                            using (System.IO.StreamReader reader = new StreamReader(Utilidades.IO.FileStorageService.Storage.OpenRead(fileLocationTemp)))
                            using (System.Drawing.Image t = System.Drawing.Image.FromStream(reader.BaseStream))
                            {
                                Utilidades.IO.FileStorageService.Storage.SaveImage(fileLocation, t);
                            }

                            repCargaOcorrenciaImagem.Inserir(cargaOcorrenciaImagem);

                            if (configuracaoTMS.AppUtilizaControleColetaEntrega)
                            {
                                try
                                {
                                    using (Stream fsSource = Utilidades.IO.FileStorageService.Storage.OpenRead(fileLocation))
                                    {
                                        Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SalvarImagem(fsSource, unitOfWork, out tokenImagem);
                                        Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SalvarImagemEntrega(codigoClienteMultisoftware, cargaOcorrencia?.CargaEntrega?.Codigo ?? 0, tokenImagem, unitOfWork, DateTime.Now, 0, 0, OrigemSituacaoEntrega.WebService);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Servicos.Log.TratarErro(ex);
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
                        retorno = "O codigo informado não pertence a uma ocorrência válida";
                    }
                }
                else
                {
                    retorno = "O usuário cadastrado não está cadastrado na Empresa";
                }

                if (Utilidades.IO.FileStorageService.Storage.Exists(fileLocationTemp))
                    Utilidades.IO.FileStorageService.Storage.Delete(fileLocationTemp);

                unitOfWork.CommitChanges();
                return retorno;
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                throw;
            }
        }

        public string EnviarImagemAtendimento(int codigoAtendimento, string tokenImagem, int idUsuario, int codigoClienteMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            unitOfWork.Start();
            try
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repositorioConfiguracaoTMS.BuscarConfiguracaoPadrao();

                string retorno = "";
                Repositorio.Embarcador.Chamados.Chamado repAtendimento = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
                Repositorio.Embarcador.Chamados.ChamadoAnexo repAtendimentoAnexo = new Repositorio.Embarcador.Chamados.ChamadoAnexo(unitOfWork);
                Dominio.Entidades.Embarcador.Chamados.Chamado atendimento = repAtendimento.BuscarPorCodigo(codigoAtendimento);

                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                string extensao = ".jpg";
                string caminhoTemp = Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoTempArquivosImportacao;
                string fileLocationTemp = Utilidades.IO.FileStorageService.Storage.Combine(caminhoTemp, tokenImagem);

                Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorCodigo(idUsuario);
                if (usuario != null)
                {
                    if (atendimento != null)
                    {
                        if (Utilidades.IO.FileStorageService.Storage.Exists(fileLocationTemp))
                        {
                            Dominio.Entidades.Embarcador.Chamados.ChamadoAnexo anexo = new Dominio.Entidades.Embarcador.Chamados.ChamadoAnexo();

                            anexo.Chamado = atendimento;
                            anexo.Descricao = "";
                            anexo.GuidArquivo = tokenImagem;
                            anexo.NomeArquivo = "MI_" + tokenImagem + "_" + codigoClienteMultisoftware + extensao;

                            string caminho = retornarCaminhoOcorrencia(null, unitOfWork);
                            string fileLocation = Utilidades.IO.FileStorageService.Storage.Combine(caminho, anexo.GuidArquivo + extensao);

                            using (System.IO.StreamReader reader = new StreamReader(Utilidades.IO.FileStorageService.Storage.OpenRead(fileLocationTemp)))
                            using (System.Drawing.Image t = System.Drawing.Image.FromStream(reader.BaseStream))
                            {
                                Utilidades.IO.FileStorageService.Storage.SaveImage(fileLocation, t);
                            }

                            repAtendimentoAnexo.Inserir(anexo);
                        }
                        else
                        {
                            retorno = "A imagem não foi enviada para o servidor.";
                        }
                    }
                    else
                    {
                        retorno = "O codigo informado não pertence a um atendimento válido";
                    }
                }
                else
                {
                    retorno = "O usuário cadastrado não está cadastrado na Empresa";
                }

                if (Utilidades.IO.FileStorageService.Storage.Exists(fileLocationTemp))
                    Utilidades.IO.FileStorageService.Storage.Delete(fileLocationTemp);

                unitOfWork.CommitChanges();
                return retorno;
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                throw;
            }
            finally
            {

            }
        }

        public string SalvarImagem(Stream imagem, out string tokenImagem, Repositorio.UnitOfWork unitOfWork)
        {
            tokenImagem = "";
            string retorno = "";

            byte[] buffer = new byte[16 * 1024];
            using MemoryStream ms = new MemoryStream();
            int read;
            while ((read = imagem.Read(buffer, 0, buffer.Length)) > 0)
                ms.Write(buffer, 0, read);

            ms.Position = 0;

            string extensao = ".jpg";
            if (extensao.Equals(".jpg") || extensao.Equals(".jpeg"))
            {
                string token = Guid.NewGuid().ToString().Replace("-", "");
                string caminho = Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoTempArquivosImportacao;
                tokenImagem = token;
                string fileLocation = Utilidades.IO.FileStorageService.Storage.Combine(caminho, tokenImagem);

                using (System.Drawing.Image t = System.Drawing.Image.FromStream(ms))
                {
                    Utilidades.IO.FileStorageService.Storage.SaveImage(fileLocation, t);
                }
            }
            else
            {
                retorno = "A extensão do arquivo é inválida.";
            }
            return retorno;
        }

        public bool EnviarOcorrenciaIntegracao(int ocorrencia, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
        {
            unitOfWork.Start();
            try
            {
                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);

                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia = repOcorrencia.BuscarPorCodigo(ocorrencia);
                if (cargaOcorrencia == null)
                {
                    Servicos.Log.TratarErro("Ocorrencia não encontrada");
                    return false;
                }
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTEs = repOcorrencia.BuscarCargaCTe(ocorrencia);
                Servicos.Embarcador.Integracao.IntegracaoOcorrencia.AdicionarIntegracoesOcorrencia(cargaOcorrencia, cargaCTEs, unitOfWork);
                Servicos.Embarcador.Integracao.IntegracaoEDI.AdicionarEDIParaIntegracao(cargaOcorrencia, false, tipoServicoMultisoftware, unitOfWork);
                Servicos.Embarcador.Carga.Ocorrencia.AvancarEtapaOcorrenciaPosEmissao(ref cargaOcorrencia, null, tipoServicoMultisoftware, unitOfWork, clienteMultisoftware);

                repOcorrencia.Atualizar(cargaOcorrencia);

                unitOfWork.CommitChanges();
                return true;
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return false;
            }
            finally
            {

            }
        }
    }
}
