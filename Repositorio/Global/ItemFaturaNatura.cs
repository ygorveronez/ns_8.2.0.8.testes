using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class ItemFaturaNatura : RepositorioBase<Dominio.Entidades.ItemFaturaNatura>, Dominio.Interfaces.Repositorios.ItemFaturaNatura
    {
        public ItemFaturaNatura(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.ItemFaturaNatura BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ItemFaturaNatura>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.ItemFaturaNatura> BuscarPorFatura(int codigoFatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ItemFaturaNatura>();

            var result = from obj in query where obj.Fatura.Codigo == codigoFatura select obj;

            return result.ToList();
        }

        public Dominio.Entidades.ItemFaturaNatura BuscarPorNotaFiscal(int codigoNotaFiscal, int codigoFatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ItemFaturaNatura>();

            var result = from obj in query where obj.NotaFiscal.Codigo == codigoNotaFiscal && obj.Fatura.Codigo == codigoFatura select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.ItemFaturaNatura> BuscarPorCodigoCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ItemFaturaNatura>();

            var result = from obj in query where obj.NotaFiscal.CTe.Codigo == codigoCTe select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.ItemFaturaNatura> BuscarPorCodigoNFSe(int codigoNFSe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ItemFaturaNatura>();

            var result = from obj in query where obj.NotaFiscal.NFSe.Codigo == codigoNFSe select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.ItemFaturaNatura> BuscarDocumentosFatura(int codigoFatura, int tipo, int numero)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ItemFaturaNatura>();

            var result = from obj in query where obj.Fatura.Codigo == codigoFatura select obj;

            if (tipo == 1)
                result = result.Where(o => o.NotaFiscal.CTe != null);

            if (tipo == 2)
                result = result.Where(o => o.NotaFiscal.NFSe != null);

            if (numero > 0)
                result = result.Where(o => (o.NotaFiscal.CTe != null && o.NotaFiscal.CTe.Numero == numero) || (o.NotaFiscal.NFSe != null && o.NotaFiscal.NFSe.Numero == numero));


            return result.ToList();
        }
    }
}
