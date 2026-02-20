using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Cargas.ValePedagio
{
    public class CargaValePedagioDadosCompraPraca : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioDadosCompraPraca>
    {
        public CargaValePedagioDadosCompraPraca(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioDadosCompraPraca> BuscarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioDadosCompraPraca>()
                .Where(obj => obj.CargaValePedagioDadosCompra.Carga.Codigo == codigoCarga);

            return query
                .Fetch(obj => obj.CargaValePedagioDadosCompra)
                .ThenFetch(obj => obj.Carga)
                .ThenFetch(obj => obj.DadosSumarizados)
                .ToList();
        }
        
        public List<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioDadosCompraPraca> BuscarPorProtocoloCarga(int protocolo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioDadosCompraPraca>()
                .Where(obj => obj.CargaValePedagioDadosCompra.Carga.Protocolo == protocolo);
            
            return query
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioDadosCompraPraca> BuscarPorProtocoloCarga(List<int> protocolos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioDadosCompraPraca>()
                .Where(obj => protocolos.Contains(obj.CargaValePedagioDadosCompra.Carga.Protocolo));

            return query
                .ToList();
        }
    }
}
