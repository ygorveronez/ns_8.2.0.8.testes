using Dominio.ObjetosDeValor.Enumerador;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using CoreWCF;

namespace SGT.WebService
{
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any, IncludeExceptionDetailInFaults = true)]
    public class TipoCarga(IServiceProvider serviceProvider) : WebServiceBase(serviceProvider), ITipoCarga
    {
        public Retorno<List<Dominio.ObjetosDeValor.Embarcador.Carga.TipoCargaEmbarcador>> BuscarTiposCargasDisponiveis()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            ValidarToken();

            try
            {
                Repositorio.Embarcador.Cargas.TipoDeCarga repTipoCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);

                List<Dominio.ObjetosDeValor.Embarcador.Carga.TipoCargaEmbarcador> tiposCarga = repTipoCarga.BuscarAtivos();

                return new Retorno<List<Dominio.ObjetosDeValor.Embarcador.Carga.TipoCargaEmbarcador>>() { Objeto = tiposCarga, Status = true, DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") };
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new Retorno<List<Dominio.ObjetosDeValor.Embarcador.Carga.TipoCargaEmbarcador>>() { Mensagem = "Ocorreu uma falha genérica ao realizar a consulta.", Status = false };
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
