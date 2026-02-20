using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace SGT.Mobile
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Canhotos" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Canhotos.svc or Canhotos.svc.cs at the Solution Explorer and start debugging.
    public class Canhotos : WebServiceBase, ICanhotos
    {
        #region Métodos Públicos

        public Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Canhotos.Canhoto>> BuscarCanhotosAlteradosPorUsuario(string usuario, string dataUltimaVerificacao, string token)
        {
            ValidarToken(token);

            DateTime dataInicioRequisicao = DateTime.Now;

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);
            AdminMultisoftware.Repositorio.Mobile.UsuarioMobile repUsuarioMobile = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobile(unitOfWorkAdmin);

            Servicos.Embarcador.Mobile.Canhotos.Canhotos serCanhoto = new Servicos.Embarcador.Mobile.Canhotos.Canhotos();

            Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Canhotos.Canhoto>> retorno = new Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Canhotos.Canhoto>>();
            retorno.Status = true;
            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = repUsuarioMobile.BuscarPorCodigo(int.Parse(usuario));

                if (usuarioMobile != null)
                {
                    DateTime data;
                    if (!DateTime.TryParseExact(dataUltimaVerificacao, "ddMMyyyyHHmmss", null, System.Globalization.DateTimeStyles.None, out data))
                    {
                        retorno.Status = false;
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Mensagem = "A data de ultima verificação não esta em um formato correto (ddMMyyyyHHmmss); ";
                    }
                    else
                    {
                        retorno.Objeto = new List<Dominio.ObjetosDeValor.Embarcador.Mobile.Canhotos.Canhoto>();
                        foreach (AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente in usuarioMobile.Clientes.ToList())
                        {
                            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente));
                            try
                            {
                                if (usuarioMobileCliente.Cliente.ClienteConfiguracao.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador ||
                                    usuarioMobileCliente.Cliente.ClienteConfiguracao.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                                {
                                    retorno.Objeto.AddRange(serCanhoto.ObterCanhotosModificadosPorUltimaConsulta(data, usuarioMobile.Codigo, usuarioMobileCliente.Cliente, unitOfWork));
                                }
                                else if (usuarioMobileCliente.Cliente.ClienteConfiguracao.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                                {
                                    //todo: implementar aqui a variação quando não for multiEmbarcador.
                                }
                            }
                            finally
                            {
                                unitOfWork.Dispose();
                                unitOfWork = null;
                            }
                        }
                    }
                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "O usuário informado é inválido";
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Status = false;
                retorno.Mensagem = "Ocorreu uma falha ao consultar os canhotos alterados";
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }

            retorno.DataRetorno = dataInicioRequisicao.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Documentos.Documento>> BuscarDocumentosAlteradosPorUsuario(string usuario, string dataUltimaVerificacao, string token)
        {
            ValidarToken(token);

            DateTime dataInicioRequisicao = DateTime.Now;

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);
            AdminMultisoftware.Repositorio.Mobile.UsuarioMobile repUsuarioMobile = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobile(unitOfWorkAdmin);

            Servicos.Embarcador.Mobile.Canhotos.Canhotos serCanhoto = new Servicos.Embarcador.Mobile.Canhotos.Canhotos();

            Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Documentos.Documento>> retorno = new Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Documentos.Documento>>();
            retorno.Status = true;
            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = repUsuarioMobile.BuscarPorCodigo(int.Parse(usuario));

                if (usuarioMobile != null)
                {
                    DateTime data;
                    if (!DateTime.TryParseExact(dataUltimaVerificacao, "ddMMyyyyHHmmss", null, System.Globalization.DateTimeStyles.None, out data))
                    {
                        retorno.Status = false;
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Mensagem = "A data de ultima verificação não esta em um formato correto (ddMMyyyyHHmmss); ";
                    }
                    else
                    {
                        retorno.Objeto = new List<Dominio.ObjetosDeValor.Embarcador.Mobile.Documentos.Documento>();
                        foreach (AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente in usuarioMobile.Clientes.ToList())
                        {
                            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente));
                            try
                            {

                                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repositorioConfiguracaoTMS.BuscarConfiguracaoPadrao();

                                if (!configuracaoTMS.AppUtilizaControleColetaEntrega)
                                {
                                    if (usuarioMobileCliente.Cliente.ClienteConfiguracao.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador ||
                                        usuarioMobileCliente.Cliente.ClienteConfiguracao.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                                    {
                                        retorno.Objeto.AddRange(serCanhoto.ObterDocumentosModificadosPorUltimaConsulta(data, usuarioMobile.Codigo, usuarioMobileCliente.Cliente, unitOfWork));
                                    }
                                    else if (usuarioMobileCliente.Cliente.ClienteConfiguracao.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                                    {
                                        //todo: implementar aqui a variação quando não for multiEmbarcador.
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                throw;
                            }
                            finally
                            {
                                unitOfWork.Dispose();
                                unitOfWork = null;
                            }
                        }
                    }
                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "O usuário informado é inválido";
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Status = false;
                retorno.Mensagem = "Ocorreu uma falha ao consultar os canhotos alterados";
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }

            retorno.DataRetorno = dataInicioRequisicao.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<bool> EnviarJustificativaCanhoto(string latitude, string longitude, int usuario, int canhoto, int empresaMultisoftware, string justificativa, string token)
        {
            ValidarToken(token);

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);
            AdminMultisoftware.Repositorio.Mobile.UsuarioMobile repUsuarioMobile = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobile(unitOfWorkAdmin);
            Servicos.Embarcador.Mobile.Canhotos.Canhotos serCanhoto = new Servicos.Embarcador.Mobile.Canhotos.Canhotos();
            Retorno<bool> retorno = new Retorno<bool>();
            retorno.Status = true;
            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = repUsuarioMobile.BuscarPorCodigo(usuario);

                if (usuarioMobile != null)
                {

                    Servicos.Log.TratarErro("us " + usuario);
                    Servicos.Log.TratarErro("emp " + empresaMultisoftware);
                    AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = (from obj in usuarioMobile.Clientes where obj.Cliente.Codigo == empresaMultisoftware select obj).FirstOrDefault();

                    if (usuarioMobileCliente != null)
                    {
                        Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente));
                        try
                        {
                            if (usuarioMobileCliente.Cliente.ClienteConfiguracao.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador ||
                                usuarioMobileCliente.Cliente.ClienteConfiguracao.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                            {
                                retorno.Mensagem = serCanhoto.EnviarJustificativaCanhoto(latitude, longitude, canhoto, justificativa, usuarioMobile.Codigo, unitOfWork);
                                if (!string.IsNullOrWhiteSpace(retorno.Mensagem))
                                {
                                    retorno.Status = false;
                                    retorno.Objeto = false;
                                }
                            }
                            else if (usuarioMobileCliente.Cliente.ClienteConfiguracao.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                            {
                                //todo: implementar aqui a variação quando não for multiEmbarcador.
                            }
                        }
                        catch (Exception ex)
                        {
                            throw;
                        }
                        finally
                        {
                            unitOfWork.Dispose();
                            unitOfWork = null;
                        }
                    }
                    else
                    {
                        retorno.Status = false;
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Mensagem = "O usuário não possui acesso para a empresa informada";
                    }
                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "O usuário informado é inválido";
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Status = false;
                retorno.Mensagem = "Ocorreu uma falha ao enviar a justificativa do canhoto";
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Dominio.Entidades.Embarcador.Filiais.Filial ObterFilialPadrao(Dominio.Entidades.Usuario usuario, ref Dominio.Entidades.Empresa empresaUsuario, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Operacional.OperadorLogistica repOperadorLogistica = new Repositorio.Embarcador.Operacional.OperadorLogistica(unidadeDeTrabalho);

            if (usuario.Filial == null)
            {
                if (usuario.CPF == "11111111111")//usuário padrão empresa
                {
                    if (usuario.Empresa != null)
                    {
                        empresaUsuario = usuario.Empresa;
                        if (usuario.Empresa.FiliaisEmbarcadorHabilitado.Count() > 0)
                            return usuario.Empresa.FiliaisEmbarcadorHabilitado.FirstOrDefault();
                        else
                            return null;
                    }
                    else
                        return null;
                }
                else
                {
                    Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica = repOperadorLogistica.BuscarPorUsuario(usuario.Codigo);

                    if (operadorLogistica != null && operadorLogistica.Filiais.Count == 1)
                        return operadorLogistica.Filiais.First().Filial;
                    else
                        return null;
                }

            }
            else
            {
                return usuario.Filial;
            }
        }

        public Retorno<bool> EnviarCanhotoDigitalizado(int clienteMultisoftware, int codigoCanhoto, bool requerAprovacao, bool devolvido, string observacao, string imagem, Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Coordenada coordenada)
        {
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);
            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = ValidarSessao(unitOfWorkAdmin);
                if (usuarioMobile == null)
                    return Retorno<bool>.CriarRetornoSessaoExpirada();

                AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente repUsuarioMobileCliente = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente(unitOfWorkAdmin);

                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = repUsuarioMobileCliente.BuscarPorUsuarioECliente(usuarioMobile.Codigo, clienteMultisoftware);


                if (usuarioMobileCliente != null)
                {
                    Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente));
                    try
                    {
                        Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
                        Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                        Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

                        Servicos.Embarcador.Canhotos.Canhoto serCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(unitOfWork);

                        Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = repCanhoto.BuscarPorCodigo(codigoCanhoto);
                        Dominio.Entidades.Usuario motorista = repUsuario.BuscarMotoristaMobilePorCPF(usuarioMobileCliente?.UsuarioMobile.CPF);

                        if (canhoto != null)
                        {

                            if (canhoto.SituacaoDigitalizacaoCanhoto != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto.Digitalizado &&
                                canhoto.SituacaoDigitalizacaoCanhoto != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto.AgAprovocao)
                            {

                                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                                if (!devolvido)
                                {
                                    byte[] data = System.Convert.FromBase64String(imagem);
                                    using MemoryStream ms = new MemoryStream(data);

                                    unitOfWork.Start();
                                    if (!configuracao.ExigeAprovacaoDigitalizacaoCanhoto && !requerAprovacao)
                                    {
                                        canhoto.SituacaoDigitalizacaoCanhoto = SituacaoDigitalizacaoCanhoto.Digitalizado;
                                        canhoto.DigitalizacaoIntegrada = false;
                                        Servicos.Embarcador.Canhotos.Canhoto.CanhotoLiberado(canhoto, configuracao, unitOfWork, TipoServicoMultisoftware, this.Cliente);
                                        Servicos.Embarcador.Canhotos.CanhotoIntegracao.GerarIntegracaoDigitalizacaoCanhoto(canhoto, configuracao, TipoServicoMultisoftware, this.Cliente, unitOfWork);
                                        Servicos.Embarcador.Canhotos.Canhoto.FinalizarDigitalizacaoCanhoto(canhoto, unitOfWork, TipoServicoMultisoftware);
                                    }
                                    else
                                    {
                                        canhoto.SituacaoDigitalizacaoCanhoto = SituacaoDigitalizacaoCanhoto.AgAprovocao;
                                        Servicos.Embarcador.Canhotos.Canhoto.CanhotoAgAprovacao(canhoto, configuracao, unitOfWork);
                                    }


                                    canhoto.DataDigitalizacao = DateTime.Now;
                                    canhoto.UsuarioDigitalizacao = motorista;

                                    string caminhoCanhoto = Servicos.Embarcador.Canhotos.Canhoto.CaminhoCanhoto(canhoto, unitOfWork);

                                    string extensao = ".jpg";
                                    canhoto.GuidNomeArquivo = Guid.NewGuid().ToString().Replace("-", "");
                                    canhoto.NomeArquivo = canhoto.Numero.ToString() + extensao;

                                    string fileLocation = Utilidades.IO.FileStorageService.Storage.Combine(caminhoCanhoto, canhoto.GuidNomeArquivo + extensao);
                                    using (var canhotofile = Image.FromStream(ms))
                                    using (var canhotoImagem = new Bitmap(canhotofile))
                                    {
                                        Utilidades.IO.FileStorageService.Storage.SaveImage(fileLocation, canhotoImagem, ImageFormat.Jpeg);
                                    }

                                    canhoto.OrigemDigitalizacao = CanhotoOrigemDigitalizacao.Mobile;

                                    serCanhoto.GerarHistoricoCanhoto(canhoto, motorista, $"Imagem do Canhoto digitalizada via Mobile com o nome {canhoto.GuidNomeArquivo}", unitOfWork);

                                    if (!Utilidades.IO.FileStorageService.Storage.Exists(fileLocation))
                                    {
                                        serCanhoto.GerarHistoricoCanhoto(canhoto, motorista, $"Falha ao salvar imagem do Canhoto", unitOfWork);
                                        Servicos.Log.TratarErro($"Falha ao salvar imagem do Canhoto de id {canhoto.Codigo} e guid {canhoto.GuidNomeArquivo}. Imagem: {imagem} ");
                                    }
                                }

                                canhoto.DataEnvioCanhoto = DateTime.Now;
                                canhoto.Observacao = !string.IsNullOrWhiteSpace(observacao) ? observacao : "";
                                //canhoto.Latitude = canhotoIntegracao.Latitude;
                                //canhoto.Longitude = canhotoIntegracao.Longitude;
                                canhoto.DataUltimaModificacao = DateTime.Now;

                                if (devolvido)
                                {
                                    canhoto.SituacaoCanhoto = SituacaoCanhoto.Cancelado;
                                    serCanhoto.GerarHistoricoCanhoto(canhoto, motorista, "Cancelado via devolução registrada no Mobile.", unitOfWork);
                                    Servicos.Embarcador.Canhotos.Canhoto.CanhotoLiberado(canhoto, configuracao, unitOfWork, TipoServicoMultisoftware, this.Cliente);
                                }

                                canhoto.SituacaoPgtoCanhoto = SituacaoPgtoCanhoto.Pendente;

                                repCanhoto.Atualizar(canhoto);


                                unitOfWork.CommitChanges();
                            }
                            return Retorno<bool>.CriarRetornoSucesso(true);
                        }
                        else
                        {
                            return Retorno<bool>.CriarRetornoDadosInvalidos("Não foi localizado um canhoto compatível com a entrega para a imagem informada");
                        }
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        throw;
                    }
                    finally
                    {
                        unitOfWork.Dispose();
                        unitOfWork = null;
                    }
                }
                else
                    return Retorno<bool>.CriarRetornoSessaoNaoVinculadaAoClienteMultisoftware();
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao enviar a imagem do monitoriamento.");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<int> EnviarCanhotoDigitalizacao(int clienteMultisoftware, int codigoCargaEntrega, string imagem)
        {
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);
            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = ValidarSessao(unitOfWorkAdmin);
                if (usuarioMobile == null)
                    return Retorno<int>.CriarRetornoSessaoExpirada();

                AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente repUsuarioMobileCliente = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente(unitOfWorkAdmin);

                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = repUsuarioMobileCliente.BuscarPorUsuarioECliente(usuarioMobile.Codigo, clienteMultisoftware);


                if (usuarioMobileCliente != null)
                {
                    Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente));

                    try
                    {
                        Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                        Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
                        Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                        Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

                        Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repCargaEntrega.BuscarPorCodigo(codigoCargaEntrega);
                        Dominio.Entidades.Usuario motorista = repUsuario.BuscarMotoristaMobilePorCPF(usuarioMobileCliente?.UsuarioMobile.CPF);

                        byte[] data = System.Convert.FromBase64String(imagem);

                        using MemoryStream ms = new MemoryStream(data);

                        Servicos.Embarcador.Mobile.Canhotos.Canhotos serCanhotoMob = new Servicos.Embarcador.Mobile.Canhotos.Canhotos();
                        Servicos.Embarcador.Canhotos.Canhoto serCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(unitOfWork);

                        serCanhotoMob.SalvarImagemCanhoto(ms, out string tokenImagem, unitOfWork);

                        string caminho = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoTempArquivosImportacao;
                        string caminhoCompleto = Utilidades.IO.FileStorageService.Storage.Combine(caminho, tokenImagem);
                        string apiLink = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().APIOCRLink;
                        string apiKey = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().APIOCRKey;
                        Servicos.Embarcador.Canhotos.LeitorOCR serLeitorOCR = new Servicos.Embarcador.Canhotos.LeitorOCR(unitOfWork);

                        serLeitorOCR.DefinirAPI(apiLink, apiKey);
                        Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = serLeitorOCR.ObterCanhoto(cargaEntrega.Carga, caminhoCompleto, unitOfWork);

                        if (canhoto != null)
                        {

                            if (canhoto.SituacaoDigitalizacaoCanhoto != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto.Digitalizado &&
                                canhoto.SituacaoDigitalizacaoCanhoto != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto.AgAprovocao)
                            {
                                unitOfWork.Start();
                                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                                string caminhoCanhoto = Servicos.Embarcador.Canhotos.Canhoto.CaminhoCanhoto(canhoto, unitOfWork);

                                string extensao = ".jpg";
                                canhoto.GuidNomeArquivo = tokenImagem;
                                canhoto.NomeArquivo = canhoto.Numero.ToString() + extensao;
                                canhoto.DataEnvioCanhoto = DateTime.Now;

                                if (!configuracao.ExigeAprovacaoDigitalizacaoCanhoto)
                                {
                                    canhoto.SituacaoDigitalizacaoCanhoto = SituacaoDigitalizacaoCanhoto.Digitalizado;
                                    canhoto.DigitalizacaoIntegrada = false;

                                    Servicos.Embarcador.Canhotos.Canhoto.CanhotoLiberado(canhoto, configuracao, unitOfWork, TipoServicoMultisoftware, this.Cliente);
                                    Servicos.Embarcador.Canhotos.CanhotoIntegracao.GerarIntegracaoDigitalizacaoCanhoto(canhoto, configuracao, TipoServicoMultisoftware, this.Cliente, unitOfWork);
                                    Servicos.Embarcador.Canhotos.Canhoto.FinalizarDigitalizacaoCanhoto(canhoto, unitOfWork, TipoServicoMultisoftware);
                                }
                                else
                                {
                                    canhoto.SituacaoDigitalizacaoCanhoto = SituacaoDigitalizacaoCanhoto.AgAprovocao;
                                    Servicos.Embarcador.Canhotos.Canhoto.CanhotoAgAprovacao(canhoto, configuracao, unitOfWork);
                                }

                                canhoto.DataDigitalizacao = DateTime.Now;
                                canhoto.UsuarioDigitalizacao = motorista;
                                canhoto.MotivoRejeicaoDigitalizacao = string.Empty;
                                canhoto.DataUltimaModificacao = DateTime.Now;
                                canhoto.SituacaoPgtoCanhoto = SituacaoPgtoCanhoto.Pendente;

                                canhoto.OrigemDigitalizacao = CanhotoOrigemDigitalizacao.Mobile;

                                serCanhoto.GerarHistoricoCanhoto(canhoto, motorista, "Imagem do Canhoto digitalizada via Mobile.", unitOfWork);

                                string fileLocation = Utilidades.IO.FileStorageService.Storage.Combine(caminhoCanhoto, canhoto.GuidNomeArquivo + extensao);
                                using (var canhotofile = Image.FromStream(Utilidades.IO.FileStorageService.Storage.OpenRead(caminhoCompleto)))
                                using (var canhotoImagem = new Bitmap(canhotofile))
                                {
                                    Utilidades.IO.FileStorageService.Storage.SaveImage(fileLocation, canhotoImagem, ImageFormat.Jpeg);
                                }

                                if (!Utilidades.IO.FileStorageService.Storage.Exists(fileLocation))
                                {
                                    Servicos.Log.TratarErro($"Erro03 - Falha ao salvar imagem do Canhoto de id {canhoto.Codigo} e guid {canhoto.GuidNomeArquivo}.", "EnviarCanhoto");
                                    unitOfWork.Rollback();
                                    return Retorno<int>.CriarRetornoDadosInvalidos("Erro03 - Falha ao salvar imagem do Canhoto.");
                                }

                                repCanhoto.Atualizar(canhoto);
                                unitOfWork.CommitChanges();
                            }
                            return Retorno<int>.CriarRetornoSucesso(canhoto.Codigo);
                        }
                        else
                        {
                            return Retorno<int>.CriarRetornoDadosInvalidos("Não foi localizado um canhoto compatível com a entrega para a imagem informada");
                        }
                    }
                    catch
                    {
                        unitOfWork.Rollback();
                        throw;
                    }
                    finally
                    {
                        unitOfWork.Dispose();
                    }
                }
                else
                    return Retorno<int>.CriarRetornoSessaoNaoVinculadaAoClienteMultisoftware();
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Retorno<int>.CriarRetornoExcecao("Ocorreu uma falha ao enviar a imagem do monitoriamento.");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<bool> EnviarImagemCanhotoLeituraOCR(int usuario, int empresaMultisoftware, string tokenImagem, string token, string cnpjEmitente, string codigoGrupoPessoaEmitente)
        {
            ValidarToken(token);

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);
            AdminMultisoftware.Repositorio.Mobile.UsuarioMobile repUsuarioMobile = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobile(unitOfWorkAdmin);
            Servicos.Embarcador.Mobile.Canhotos.Canhotos serCanhoto = new Servicos.Embarcador.Mobile.Canhotos.Canhotos();
            Retorno<bool> retorno = new Retorno<bool>();
            retorno.Status = true;
            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = repUsuarioMobile.BuscarPorCodigo(usuario);

                if (usuarioMobile != null)
                {
                    AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = (from obj in usuarioMobile.Clientes where obj.Cliente.Codigo == empresaMultisoftware select obj).FirstOrDefault();
                    if (usuarioMobileCliente != null)
                    {
                        Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente));
                        string caminhoTemp = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoTempArquivosImportacao;
                        string fileLocationTemp = Utilidades.IO.FileStorageService.Storage.Combine(caminhoTemp, tokenImagem);

                        try
                        {
                            if (usuarioMobileCliente.Cliente.ClienteConfiguracao.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador ||
                                usuarioMobileCliente.Cliente.ClienteConfiguracao.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                            {
                                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                                Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
                                Dominio.Entidades.Usuario motorista = repUsuario.BuscarPorCodigoMobile(usuarioMobile.Codigo);

                                if (motorista != null)
                                {
                                    List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();

                                    List<Dominio.Entidades.Usuario> motoristas = repUsuario.BuscarPorTodosCodigoMobile(usuarioMobile.Codigo);
                                    foreach (Dominio.Entidades.Usuario motoristaCarga in motoristas)
                                    {
                                        Dominio.Entidades.Embarcador.Cargas.Carga ultimaCarga = repCarga.BuscarUltimaCargaPorMotorista(motoristaCarga.Codigo);
                                        if (ultimaCarga != null)
                                            cargas.Add(ultimaCarga);
                                    }

                                    Dominio.Entidades.Embarcador.Cargas.Carga carga = null;
                                    if (cargas.Count > 0)
                                        carga = (from obj in cargas select obj).OrderByDescending(obj => obj.Codigo).FirstOrDefault();

                                    Dominio.Entidades.Empresa empresaUsuario = null;
                                    Dominio.Entidades.Embarcador.Filiais.Filial filial = null;

                                    if (usuarioMobileCliente.Cliente.ClienteConfiguracao.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                                    {
                                        empresaUsuario = carga?.Empresa;
                                    }
                                    else
                                    {
                                        if (carga != null && carga.Filial != null)
                                        {
                                            filial = carga.Filial;
                                            motorista = carga.Motoristas.FirstOrDefault();
                                        }
                                        else
                                            filial = ObterFilialPadrao(motorista, ref empresaUsuario, unitOfWork);
                                    }

                                    if (filial == null && empresaUsuario == null)
                                    {
                                        retorno.Status = false;
                                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                                        retorno.Mensagem = "Não existe nenhuma viagem vinculada ao motorista.";
                                    }
                                    else
                                    {
                                        if (Utilidades.IO.FileStorageService.Storage.Exists(fileLocationTemp))
                                        {
                                            string url = ObterURLCliente(usuarioMobileCliente);
                                            if (!string.IsNullOrWhiteSpace(url))
                                            {
                                                if (url.Contains("http://piracanjuba.multiembarcador.com.br"))//Gambiarra de momento sugerida pelo Rodrigo, necessário criar utilizahttps no adminmultisoftware
                                                    url = url.Replace("http:", "https:");

                                                url = $"{url}/SGT.WebService/NFe.svc";
                                                System.IO.StreamReader reader = new StreamReader(Utilidades.IO.FileStorageService.Storage.OpenRead(fileLocationTemp));
                                                try
                                                {
                                                    NFe.NFeClient wsNFe = ObterClient(url);
                                                    Servicos.InspectorBehavior inspector = new Servicos.InspectorBehavior();
                                                    wsNFe.Endpoint.EndpointBehaviors.Add(inspector);
                                                    OperationContextScope scope = new OperationContextScope(wsNFe.InnerChannel);
                                                    MessageHeader header = MessageHeader.CreateHeader("Token", "Token", "9834cdd53573429da6f4b3fbf9b50f4e");
                                                    OperationContext.Current.OutgoingMessageHeaders.Add(header);

                                                    NFe.RetornoOfstring retornoToken = wsNFe.EnviarStreamImagemCanhoto(reader.BaseStream);
                                                    if (retornoToken.Status)
                                                    {
                                                        string nome = "";
                                                        if (!string.IsNullOrWhiteSpace(cnpjEmitente))
                                                            nome = cnpjEmitente + "_C_" + retornoToken.Objeto;
                                                        if (!string.IsNullOrWhiteSpace(codigoGrupoPessoaEmitente))
                                                            nome = codigoGrupoPessoaEmitente + "_G_" + retornoToken.Objeto;
                                                        else if (empresaUsuario == null)
                                                            nome = filial.CNPJ + "_M_" + retornoToken.Objeto;
                                                        else
                                                            nome = empresaUsuario.CNPJ_SemFormato + "_E_" + retornoToken.Objeto;

                                                        NFe.RetornoOfboolean retornoEnvio = wsNFe.EnviarImagemCanhotoLeituraOCR(motorista.Codigo, nome);
                                                        if (!retornoEnvio.Status)
                                                        {
                                                            retorno.Status = false;
                                                            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                                                            retorno.Mensagem = retornoEnvio.Mensagem;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        retorno.Status = false;
                                                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                                                        retorno.Mensagem = retornoToken.Mensagem;
                                                    }
                                                }
                                                catch (Exception)
                                                {
                                                    throw;
                                                }
                                                finally
                                                {
                                                    reader.Dispose();
                                                }
                                            }
                                            else
                                            {
                                                retorno.Status = false;
                                                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                                                retorno.Mensagem = "A integração de imagens de canhotos não está habilitada.";
                                            }
                                        }
                                        else
                                        {
                                            retorno.Status = false;
                                            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                                            retorno.Mensagem = "A imagem não foi enviada para o servidor.";
                                        }
                                    }
                                }
                                else
                                {
                                    retorno.Status = false;
                                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                                    retorno.Mensagem = "O usuário cadastrado não está cadastrado na Empresa";
                                }

                            }
                            else if (usuarioMobileCliente.Cliente.ClienteConfiguracao.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                            {
                                //todo: implementar aqui a variação quando não for multiEmbarcador.
                            }
                        }
                        catch (Exception ex)
                        {
                            throw;
                        }
                        finally
                        {

                            if (Utilidades.IO.FileStorageService.Storage.Exists(fileLocationTemp))
                                Utilidades.IO.FileStorageService.Storage.Delete(fileLocationTemp);

                            unitOfWork.Dispose();
                            unitOfWork = null;
                        }
                    }
                    else
                    {
                        retorno.Status = false;
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Mensagem = "O usuário não possui acesso para a empresa informada";
                    }
                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "O usuário informado é inválido";
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Status = false;
                retorno.Mensagem = "Ocorreu uma falha ao confirmar o envio da imagem do Canhoto";
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<bool> EnviarImagemCanhoto(string latitude, string longitude, int usuario, int canhoto, int empresaMultisoftware, string tokenImagem, string token, string dataEntrega)
        {
            ValidarToken(token);

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);
            AdminMultisoftware.Repositorio.Mobile.UsuarioMobile repUsuarioMobile = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobile(unitOfWorkAdmin);
            Servicos.Embarcador.Mobile.Canhotos.Canhotos serCanhoto = new Servicos.Embarcador.Mobile.Canhotos.Canhotos();
            Retorno<bool> retorno = new Retorno<bool>();
            retorno.Status = true;
            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = repUsuarioMobile.BuscarPorCodigo(usuario);

                if (usuarioMobile != null)
                {
                    AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = (from obj in usuarioMobile.Clientes where obj.Cliente.Codigo == empresaMultisoftware select obj).FirstOrDefault();
                    if (usuarioMobileCliente != null)
                    {
                        Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente));
                        string caminhoTemp = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoTempArquivosImportacao;
                        string fileLocationTemp = Utilidades.IO.FileStorageService.Storage.Combine(caminhoTemp, tokenImagem);

                        try
                        {
                            if (usuarioMobileCliente.Cliente.ClienteConfiguracao.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador ||
                                usuarioMobileCliente.Cliente.ClienteConfiguracao.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                            {
                                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                                Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
                                Dominio.Entidades.Usuario motorista = repUsuario.BuscarPorCodigoMobile(usuarioMobile.Codigo);

                                if (motorista != null)
                                {
                                    Dominio.Entidades.Embarcador.Canhotos.Canhoto canhotoNF = repCanhoto.BuscarPorCodigo(canhoto);

                                    if (canhotoNF != null)
                                    {
                                        if (Utilidades.IO.FileStorageService.Storage.Exists(fileLocationTemp))
                                        {
                                            string url = ObterURLCliente(usuarioMobileCliente);
                                            if (!string.IsNullOrWhiteSpace(url))
                                            {
                                                if (url.Contains("http://piracanjuba.multiembarcador.com.br"))//Gambiarra de momento sugerida pelo Rodrigo, necessário criar utilizahttps no adminmultisoftware
                                                    url = url.Replace("http:", "https:");

                                                url = $"{url}/SGT.WebService/NFe.svc";
                                                System.IO.StreamReader reader = new StreamReader(Utilidades.IO.FileStorageService.Storage.OpenRead(fileLocationTemp));

                                                DateTime? dataEntregaNotaCliente = null;
                                                if (!string.IsNullOrWhiteSpace(dataEntrega))
                                                    dataEntregaNotaCliente = dataEntrega.ToNullableDateTime("ddMMyyyy");
                                                try
                                                {
                                                    NFe.NFeClient wsNFe = ObterClient(url);
                                                    Servicos.InspectorBehavior inspector = new Servicos.InspectorBehavior();
                                                    wsNFe.Endpoint.EndpointBehaviors.Add(inspector);
                                                    OperationContextScope scope = new OperationContextScope(wsNFe.InnerChannel);
                                                    MessageHeader header = MessageHeader.CreateHeader("Token", "Token", "9834cdd53573429da6f4b3fbf9b50f4e");
                                                    OperationContext.Current.OutgoingMessageHeaders.Add(header);

                                                    NFe.RetornoOfstring retornoToken = wsNFe.EnviarStreamImagemCanhoto(reader.BaseStream);
                                                    if (retornoToken.Status)
                                                    {
                                                        NFe.RetornoOfboolean retornoEnvio = wsNFe.EnviarImagemCanhoto(latitude, longitude, motorista.Codigo, canhotoNF.Codigo, retornoToken.Objeto, dataEntregaNotaCliente, string.Empty, 0, 0, string.Empty);
                                                        if (!retornoEnvio.Status)
                                                        {
                                                            retorno.Status = false;
                                                            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                                                            retorno.Mensagem = retornoEnvio.Mensagem;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        retorno.Status = false;
                                                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                                                        retorno.Mensagem = retornoToken.Mensagem;
                                                    }
                                                }
                                                catch (Exception)
                                                {
                                                    throw;
                                                }
                                                finally
                                                {
                                                    reader.Dispose();
                                                }
                                            }
                                            else
                                            {
                                                retorno.Status = false;
                                                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                                                retorno.Mensagem = "A integração de imagens de canhotos com este embarcador não está habilitada.";
                                            }
                                        }
                                        else
                                        {
                                            retorno.Status = false;
                                            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                                            retorno.Mensagem = "A imagem não foi enviada para o servidor.";
                                        }
                                    }
                                    else
                                    {
                                        retorno.Status = false;
                                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                                        retorno.Mensagem = "O codigo informado não pertence a um canhoto válido";
                                    }
                                }
                                else
                                {
                                    retorno.Status = false;
                                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                                    retorno.Mensagem = "O usuário cadastrado não está cadastrado na Empresa";
                                }

                            }
                            else if (usuarioMobileCliente.Cliente.ClienteConfiguracao.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                            {
                                //todo: implementar aqui a variação quando não for multiEmbarcador.
                            }
                        }
                        catch (Exception ex)
                        {
                            throw;
                        }
                        finally
                        {

                            if (Utilidades.IO.FileStorageService.Storage.Exists(fileLocationTemp))
                                Utilidades.IO.FileStorageService.Storage.Delete(fileLocationTemp);

                            unitOfWork.Dispose();
                            unitOfWork = null;
                        }
                    }
                    else
                    {
                        retorno.Status = false;
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Mensagem = "O usuário não possui acesso para a empresa informada";
                    }
                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "O usuário informado é inválido";
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Status = false;
                retorno.Mensagem = "Ocorreu uma falha ao confirmar o envio da imagem do Canhoto";
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<string> EnviarByteImagemCanhoto(Stream imagem)
        {
            Servicos.Embarcador.Mobile.Canhotos.Canhotos serCanhoto = new Servicos.Embarcador.Mobile.Canhotos.Canhotos();
            Retorno<string> retorno = new Retorno<string>();
            retorno.Status = true;

            try
            {
                string tokenImagem = "";
                retorno.Mensagem = serCanhoto.SalvarImagemCanhoto(imagem, out tokenImagem, null);
                if (!string.IsNullOrWhiteSpace(retorno.Mensagem))
                {
                    retorno.Status = false;
                    retorno.Objeto = "";
                }
                else
                    retorno.Objeto = tokenImagem;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Status = false;
                retorno.Mensagem = "Ocorreu uma falha ao enviar a imagem do Canhoto";
            }


            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        #endregion

        #region Métodos Privados

        private NFe.NFeClient ObterClient(string url)
        {
            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(url);
            System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
            binding.SendTimeout = new TimeSpan(0, 5, 0);

            if (url.StartsWith("https"))
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;

            return new NFe.NFeClient(binding, endpointAddress);
        }

        #endregion
    }
}
