using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio
{
    public class CargaFrimesaDocumentos : RepositorioBase<Dominio.Entidades.CargaFrimesaDocumentos>, Dominio.Interfaces.Repositorios.CargaFrimesaDocumentos
    {
        public CargaFrimesaDocumentos(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.CargaFrimesaDocumentos BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CargaFrimesaDocumentos>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }
        
        public List<Dominio.Entidades.CargaFrimesaDocumentos> BuscarPorCargaFrimesa(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CargaFrimesaDocumentos>();
            var result = from obj in query where obj.CargaFrimesa.Codigo == codigoCarga select obj;
            return result.Fetch(o => o.CTe).Fetch(o => o.NFSe).ToList();
        }

        public Dominio.Entidades.CargaFrimesaDocumentos BuscarPorCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CargaFrimesaDocumentos>();
            var result = from obj in query where obj.CTe.Codigo == codigoCTe select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.CargaFrimesaDocumentos BuscarPorNFSe(int codigoNFSe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CargaFrimesaDocumentos>();
            var result = from obj in query where obj.NFSe.Codigo == codigoNFSe select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarCTePorCargaFrimesa(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CargaFrimesaDocumentos>();
            var result = from obj in query where obj.CargaFrimesa.Codigo == codigoCarga && obj.CTe != null select obj.CTe;
            return result.ToList();
        }

    }
}