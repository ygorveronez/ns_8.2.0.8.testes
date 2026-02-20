using Dominio.ObjetosDeValor.Enumerador;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using CoreWCF;

namespace SGT.WebService
{
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any, IncludeExceptionDetailInFaults = true)]
    public class ModeloVeicular(IServiceProvider serviceProvider) : WebServiceBase(serviceProvider), IModeloVeicular
    {
        public Retorno<List<Dominio.ObjetosDeValor.Embarcador.Carga.ModeloVeicular>> BuscarModelosVeicularesDisponiveis()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            ValidarToken();

            try
            {
                Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);

                List<Dominio.ObjetosDeValor.Embarcador.Carga.ModeloVeicular> modelosVeiculares = repModeloVeicularCarga.BuscarAtivos();

                return new Retorno<List<Dominio.ObjetosDeValor.Embarcador.Carga.ModeloVeicular>>() { Objeto = modelosVeiculares, Status = true, DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") };
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new Retorno<List<Dominio.ObjetosDeValor.Embarcador.Carga.ModeloVeicular>>() { Mensagem = "Ocorreu uma falha genérica ao realizar a consulta.", Status = false };
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        protected override OrigemAuditado ObterOrigemAuditado()
        {
            return OrigemAuditado.WebServiceModeloVeicular;
        }
    }
}
