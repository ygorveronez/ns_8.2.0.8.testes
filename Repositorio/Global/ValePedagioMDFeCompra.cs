using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class ValePedagioMDFeCompra : RepositorioBase<Dominio.Entidades.ValePedagioMDFeCompra>, Dominio.Interfaces.Repositorios.ValePedagioMDFeCompra
    {
        public ValePedagioMDFeCompra(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.ValePedagioMDFeCompra BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ValePedagioMDFeCompra>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.ValePedagioMDFeCompra> BuscarPorMDFe(int codigoMDFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ValePedagioMDFeCompra>();
            var result = from obj in query where obj.MDFe.Codigo == codigoMDFe select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.ValePedagioMDFeCompra> BuscarListaPorMDFeTipoStatus(int codigoMDFe, Dominio.Enumeradores.TipoIntegracaoValePedagio tipo, Dominio.Enumeradores.StatusIntegracaoValePedagio status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ValePedagioMDFeCompra>();
            var result = from obj in query where obj.MDFe.Codigo == codigoMDFe && obj.Tipo == tipo && obj.Status == status select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.ValePedagioMDFeCompra> BuscarPorMDFeTipoStatus(int codigoMDFe, Dominio.Enumeradores.TipoIntegracaoValePedagio tipo, Dominio.Enumeradores.StatusIntegracaoValePedagio status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ValePedagioMDFeCompra>();
            var result = from obj in query where obj.MDFe.Codigo == codigoMDFe && obj.Tipo == tipo && obj.Status == status select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.ValePedagioMDFeCompra> BuscarPorMDFeTipo(int codigoMDFe, Dominio.Enumeradores.TipoIntegracaoValePedagio tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ValePedagioMDFeCompra>(); 
            var result = from obj in query where obj.MDFe.Codigo == codigoMDFe && obj.Tipo == tipo select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.ValePedagioMDFeCompra> BuscarPorStatusETipo( Dominio.Enumeradores.StatusIntegracaoValePedagio status, Dominio.Enumeradores.TipoIntegracaoValePedagio tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ValePedagioMDFeCompra>();
            var result = from obj in query where obj.Status == status && obj.Tipo == tipo select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.ValePedagioMDFeCompra> BuscarRejeitadosPorTempo(int tentativas, int minutosTentativas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ValePedagioMDFeCompra>();
            var result = from obj in query
                         where obj.MDFe.DataEmissao >= DateTime.Now.AddDays(-2) &&
                               obj.DataIntegracao < DateTime.Now.AddMinutes(-minutosTentativas) &&
                               obj.Status == Dominio.Enumeradores.StatusIntegracaoValePedagio.RejeicaoCompra && 
                               obj.TentativaReenvio <= tentativas select obj;
            return result.ToList();
        }
    }
}
