using AdminMultisoftware.Dominio.Entidades.Mobile;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.NovoApp.Comum;
using Dominio.ObjetosDeValor.NovoApp.Logs;
using System;

namespace SGT.Mobile.MTrack.V1
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Logs" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Logs.svc or Logs.svc.cs at the Solution Explorer and start debugging.
    public class Logs : BaseControllerNovoApp, ILogs
    {
        public ResponseBool ArmazenarLogApp(RequestArmazenarLogApp request)
        {
            Servicos.Log.TratarErro($"Novo App - Iniciando ArmazenarLogApp");

            try
            {
                var repLogMobile = new AdminMultisoftware.Repositorio.Mobile.LogMobile(adminUnitOfWork);
                var repUsuarioMobile = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobile(adminUnitOfWork);

                var motorista = repUsuarioMobile.BuscarPorCodigo(request.codigoMotorista ?? 0);
                var dataRegistroApp = Utilidades.DateTime.FromUnixSeconds(request.dataRegistroApp);
                if (!dataRegistroApp.HasValue) throw new ServicoException("Data de registro no app inválida");

                repLogMobile.Inserir(new LogMobile
                {
                    DataCriacao = DateTime.Now,
                    DataRegistroApp = dataRegistroApp.Value,
                    IdClienteMultisoftware = request.clienteMultisoftware ?? 0,
                    Motorista = motorista,
                    Mensagem = request.mensagem,
                    Extra = request.extra,
                    Erro = request.erro,
                    MarcaAparelho = request.marcaAparelho,
                    ModeloAparelho = request.modeloAparelho,
                    VersaoApp = request.versaoApp,
                    VersaoSistemaOperacional = request.versaoSistemaOperacional,
                });

                Servicos.Log.TratarErro($"Novo App - ArmazenarLogApp - Log armazenado com sucesso");
                return new ResponseBool
                {
                    Sucesso = true,
                };
            }
            catch (BaseException ex)
            {
                Servicos.Log.TratarErro(ex);
                RetornarErro(ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                RetornarErro("Ocorreu uma falha ao armazenar o log", System.Net.HttpStatusCode.InternalServerError);
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
    }
}
