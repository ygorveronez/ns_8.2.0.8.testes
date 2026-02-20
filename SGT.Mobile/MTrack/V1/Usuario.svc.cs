using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Enumerador;
using Dominio.ObjetosDeValor.NovoApp.Comum;
using Dominio.ObjetosDeValor.NovoApp.Usuario;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SGT.Mobile.MTrack.V1
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Usuario" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Usuario.svc or Usuario.svc.cs at the Solution Explorer and start debugging.
    public class Usuario : BaseControllerNovoApp, IUsuario
    {
        #region Métodos Globais

        public ResponseBool EnviarImagemMotorista(RequestEnviarImagemMotorista request)
        {
            Servicos.Log.TratarErro($"Novo App - Iniciando EnviarImagemMotorista");
            AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = ValidarSessao();

            try
            {
                byte[] buffer = System.Convert.FromBase64String(request.imagem);
                MemoryStream ms = new MemoryStream(buffer);

                string extensaoArquivo = ".jpg";
                string caminho = ObterCaminhoArquivoFoto();
                string nomeArquivoFotoExistente = Utilidades.IO.FileStorageService.Storage.GetFiles(caminho, $"{usuarioMobile.CPF}.*").FirstOrDefault();

                if (!string.IsNullOrWhiteSpace(nomeArquivoFotoExistente))
                    Utilidades.IO.FileStorageService.Storage.Delete(nomeArquivoFotoExistente);

                ArmazenarArquivoFisico(ms, caminho, $"{usuarioMobile.CPF}{extensaoArquivo}");

                Servicos.Log.TratarErro($"Novo App - EnviarImagemMotorista - Imagem salva com sucesso");
                return new ResponseBool
                {
                    Sucesso = true,
                };
            }
            catch (BaseException ex)
            {
                RetornarErro(ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                RetornarErro("Ocorreu uma falha ao enviar a imagem do motorista", System.Net.HttpStatusCode.InternalServerError);
            }
            finally
            {
                adminUnitOfWork.Dispose();
            }

            return new ResponseBool
            {
                Sucesso = false,
            };
        }

        public ResponseBool EnviarAvaliacaoMotorista(RequestAvaliacaoMotorista request)
        {
            Servicos.Log.TratarErro($"Novo App - EnviarAvaliacaoMotorista-  Iniciando Envio Avaliação", "avaliação");
            AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = ValidarSessao();
            try
            {
                AdminMultisoftware.Repositorio.Mobile.UsuarioAvaliacao repUsuarioAvaliacao = new AdminMultisoftware.Repositorio.Mobile.UsuarioAvaliacao(adminUnitOfWork);
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioAvaliacao avaliacao = new AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioAvaliacao();
                avaliacao.DataAvaliacao = DateTime.Now;
                avaliacao.NotaExperienciaUsoAplicativo = request.NotaExperienciaUsoAplicativo;
                avaliacao.ObservacaoExperienciaUsoAplicativo = request.ObservacaoExperienciaUsoAplicativo.Left(350);
                avaliacao.UsuarioMobile = usuarioMobile;
                avaliacao.VersaoAPP = request.VersaoAPP;
                repUsuarioAvaliacao.Inserir(avaliacao);

                Servicos.Log.TratarErro($"Novo App - EnviarAvaliacaoMotorista - Finalizou Envio Avaliação");
                return new ResponseBool
                {
                    Sucesso = true,
                };
            }
            catch (BaseException ex)
            {
                RetornarErro(ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                RetornarErro("Ocorreu uma falha ao enviar a avaliação do motorista", System.Net.HttpStatusCode.InternalServerError);
            }
            finally
            {
                adminUnitOfWork.Dispose();
            }

            return new ResponseBool
            {
                Sucesso = false,
            };
        }

        public ResponseImagem ObterImagemMotorista()
        {
            Servicos.Log.TratarErro($"Novo App - Iniciando ObterImagemMotorista");
            AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = ValidarSessao();

            try
            {
                return new ResponseImagem
                {
                    Imagem = ObterFotoBase64(usuarioMobile.CPF),
                };
            }
            catch (BaseException ex)
            {
                RetornarErro(ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                RetornarErro("Ocorreu uma falha ao obter a imagem do motorista", System.Net.HttpStatusCode.InternalServerError);
            }
            finally
            {
                adminUnitOfWork.Dispose();
            }

            Servicos.Log.TratarErro($"Novo App - Finalizado ObterImagemMotorista");
            return new ResponseImagem
            {
                Imagem = null,
            };
        }

        /// <summary>
        /// Obtém a foto de um motorista através de seu CPF. Essa rota não precisa de autenticação.
        /// </summary>
        /// <param name="cpf"></param>
        /// <returns></returns>
        public ResponseImagem ObterImagemMotoristaPorCpf(string cpf)
        {
            Servicos.Log.TratarErro($"Novo App - Iniciando ObterImagemMotoristaPorCpf");
            try
            {
                cpf = cpf.Trim().Replace(".", "").Replace("-", "").Replace("/", "").Replace(" ", "");
                return new ResponseImagem
                {
                    Imagem = ObterFotoBase64(cpf),
                };
            }
            catch (BaseException ex)
            {
                RetornarErro(ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                RetornarErro("Ocorreu uma falha ao obter a imagem do motorista", System.Net.HttpStatusCode.InternalServerError);
            }

            Servicos.Log.TratarErro($"Novo App - Finalizado ObterImagemMotoristaPorCpf");

            return new ResponseImagem
            {
                Imagem = null,
            };
        }

        public static void ArmazenarArquivoFisico(Stream imagem, string caminho, string nomeArquivoComExtensao)
        {
            if (string.IsNullOrWhiteSpace(caminho))
            {
                string mensagemRetorno = "Local para armazenamento do arquivo não está configurado! Favor entrar em contato com o suporte.";
                throw new ServicoException(mensagemRetorno);
            }

            byte[] buffer = new byte[16 * 1024];
            using MemoryStream ms = new MemoryStream();
            int read;
            while ((read = imagem.Read(buffer, 0, buffer.Length)) > 0)
                ms.Write(buffer, 0, read);

            ms.Position = 0;

            string fileLocation = Utilidades.IO.FileStorageService.Storage.Combine(caminho, nomeArquivoComExtensao);

            using (System.Drawing.Image t = System.Drawing.Image.FromStream(ms))
            {
                Utilidades.IO.FileStorageService.Storage.SaveImage(fileLocation, t);
            }

            if (!Utilidades.IO.FileStorageService.Storage.Exists(fileLocation))
            {
                string mensagemRetorno = "Arquivo enviado não foi armazenado! Favor entrar em contato com o suporte.";
                throw new ServicoException(mensagemRetorno);
            }
        }

        public List<Dominio.ObjetosDeValor.WebService.Transportadores.MotoristaLicenca> ObterLicencas(int clienteMultisoftware)
        {
            Servicos.Log.TratarErro($"Novo App - ObterLicenca -  Iniciando obter licença");
            AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ValidarSessaoEObterUsuarioMobileCliente(clienteMultisoftware, out Repositorio.UnitOfWork unitOfWork);

            try
            {
                Servicos.WebService.Usuarios.Usuario servicoUsuario = new Servicos.WebService.Usuarios.Usuario(unitOfWork);
                List<Dominio.ObjetosDeValor.WebService.Transportadores.MotoristaLicenca> motoristaLicencas = servicoUsuario.ObterLicencasMotorista(usuarioMobileCliente.UsuarioMobile.Codigo, unitOfWork);

                Servicos.Log.TratarErro($"Novo App - ObterLicenca -  Finalizando obter licença");
                return motoristaLicencas;
            }
            catch (BaseException ex)
            {
                RetornarErro(ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                RetornarErro("Ocorreu uma falha ao enviar ao obter as licenças", System.Net.HttpStatusCode.InternalServerError);
                throw;
            }
            finally
            {
                adminUnitOfWork.Dispose();
                unitOfWork.Dispose();
            }
        }

        public ResponseBool ConfirmarLeituraPendenciaLicenca(RequestConfirmarLeituraPendenciaLicenca request)
        {
            AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ValidarSessaoEObterUsuarioMobileCliente(request.clienteMultisoftware, out Repositorio.UnitOfWork unitOfWork);

            try
            {
                Repositorio.Embarcador.Transportadores.MotoristaLicenca repositorioMotoristaLicenca = new Repositorio.Embarcador.Transportadores.MotoristaLicenca(unitOfWork);
                Dominio.Entidades.Embarcador.Transportadores.MotoristaLicenca motoristaLicenca = repositorioMotoristaLicenca.BuscarPorCodigo(request.protocolo, false);

                if (motoristaLicenca == null)
                    throw new WebServiceException("Licença do motorista não encontrada");

                if (motoristaLicenca.Motorista.CPF != usuarioMobileCliente.UsuarioMobile.CPF)
                    throw new WebServiceException("Licença enviada é de outro motorista, não permitindo confirmar");

                if (motoristaLicenca.ConfirmadaLeituraPendencia)
                    throw new WebServiceException("Já foi confirmada a leitura dessa licença");

                unitOfWork.Start();

                motoristaLicenca.ConfirmadaLeituraPendencia = true;
                motoristaLicenca.DataConfirmadaLeituraPendencia = DateTime.Now;

                repositorioMotoristaLicenca.Atualizar(motoristaLicenca);

                Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
                {
                    TipoAuditado = TipoAuditado.Usuario,
                    OrigemAuditado = OrigemAuditado.WebServiceMobile
                };
                Servicos.Auditoria.Auditoria.Auditar(auditado, motoristaLicenca.Motorista, $"Confirmada leitura da pendência da Licença {motoristaLicenca.Descricao} via app", unitOfWork);

                unitOfWork.CommitChanges();

                return new ResponseBool { Sucesso = true };
            }
            catch (BaseException ex)
            {
                Servicos.Log.TratarErro(ex);
                RetornarErro(ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                RetornarErro("Ocorreu uma falha ao enviar ao confirmar a leitura", System.Net.HttpStatusCode.InternalServerError);
            }
            finally
            {
                adminUnitOfWork.Dispose();
                unitOfWork.Dispose();
            }

            return new ResponseBool { Sucesso = false };
        }

        #endregion

        #region Métodos Privados

        private string ObterFotoBase64(string cpfMotorista)
        {
            string caminho = ObterCaminhoArquivoFoto();
            string nomeArquivo = Utilidades.IO.FileStorageService.Storage.GetFiles(caminho, $"{cpfMotorista}.*").FirstOrDefault();

            if (string.IsNullOrWhiteSpace(nomeArquivo))
                return "";

            byte[] imageArray = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(nomeArquivo);
            string base64ImageRepresentation = Convert.ToBase64String(imageArray);

            return base64ImageRepresentation;
        }

        private string ObterCaminhoArquivoFoto()
        {
            return CaminhoArquivos(new string[] { "Foto", "Motorista" });
        }

        private string CaminhoArquivos(string[] sufix)
        {
            string caminho = Startup.appSettingsAD["AppSettings:CaminhoArquivos"]?.ToString();
            for (var i = 0; i < sufix.Length; i++)
                caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, sufix[i]);

            return caminho;
        }

        #endregion
    }
}
