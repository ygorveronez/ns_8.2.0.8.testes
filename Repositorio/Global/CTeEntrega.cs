using System.Linq;

namespace Repositorio
{
    public class CTeEntrega : RepositorioBase<Dominio.Entidades.CTeEntrega>, Dominio.Interfaces.Repositorios.CTeEntrega
    {
        public CTeEntrega(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        public dynamic ConsultarPorMotorista(int codigoEmpresa, int codigoMotorista, bool finalizado, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CTeEntrega>();

            var result = from obj in query where obj.Entrega.Empresa.Codigo == codigoEmpresa && obj.Entrega.Motorista.Codigo == codigoMotorista && obj.Finalizado == finalizado select obj;

            return result.OrderBy(o => o.CTe.Destinatario.Nome)
                         .Skip(inicioRegistros)
                         .Take(maximoRegistros)
                         .Select(o => new
                         {
                             o.CTe.Codigo,
                             o.Entrega.Numero,
                             NumeroCTe = o.CTe.Numero + " - " + o.CTe.Serie.Numero,
                             o.CTe.Destinatario.Nome
                         }).ToList();
        }

        public int ContarConsultaPorMotorista(int codigoEmpresa, int codigoMotorista, bool finalizado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CTeEntrega>();

            var result = from obj in query where obj.Entrega.Empresa.Codigo == codigoEmpresa && obj.Entrega.Motorista.Codigo == codigoMotorista && obj.Finalizado == finalizado select obj;

            return result.Count();
        }

        public Dominio.Entidades.CTeEntrega BuscarPorCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CTeEntrega>();

            var result = from obj in query where obj.CTe.Codigo == codigoCTe select obj;

            return result.FirstOrDefault();
        }
    }
}
