using System;
using System.Collections.Generic;
using CoreWCF;
using Microsoft.AspNetCore.Http;

namespace SGT.WebService
{
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any, IncludeExceptionDetailInFaults = true)]
    public class BuscarCarga(IServiceProvider serviceProvider) : WebServiceBase(serviceProvider), IBuscarCarga
    {
        public Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao>> BuscarDadosCarga(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<List<Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao>>.CreateFrom(new Servicos.WebService.Carga.Carga(unitOfWork).BuscarCargaService(protocolo));
            });
        }
        #region Métodos Protegidos Sobrescritos

        protected override Dominio.ObjetosDeValor.Enumerador.OrigemAuditado ObterOrigemAuditado()
        {
            return Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.WebServicePedidos;
        }

        #endregion
    }
}
