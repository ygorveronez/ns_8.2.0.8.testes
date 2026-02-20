using Dominio.ObjetosDeValor.Enumerador;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace EmissaoCTe.Integracao.Base
{
    public abstract class BaseService
    {
        private Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;

        protected Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado
        {
            get
            {
                if (_auditado == null)
                {
                    _auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado
                    {
                        IP = ObterIP(),
                        OrigemAuditado = ObterOrigemAuditado(),
                        TipoAuditado = TipoAuditado.Integradoras
                    };
                }
                return _auditado;
            }
        }

        protected abstract OrigemAuditado ObterOrigemAuditado();

        private string ObterIP()
        {
            var properties = OperationContext.Current?.IncomingMessageProperties;
            if (properties != null && properties.ContainsKey(RemoteEndpointMessageProperty.Name))
            {
                var endpoint = (RemoteEndpointMessageProperty)properties[RemoteEndpointMessageProperty.Name];
                return endpoint.Address;
            }

            return "";
        }
    }
}
