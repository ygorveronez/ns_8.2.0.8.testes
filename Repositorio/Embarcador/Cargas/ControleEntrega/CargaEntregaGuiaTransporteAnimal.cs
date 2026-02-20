using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas.ControleEntrega
{
    public class CargaEntregaGuiaTransporteAnimal : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaGuiaTransporteAnimal>
    {
        public CargaEntregaGuiaTransporteAnimal(UnitOfWork unitOfWork) : base(unitOfWork) { }


        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaGuiaTransporteAnimal> ConsultarCargasEntrega(int codigoCargaEntrega, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaGuiaTransporteAnimal>();

            var result = query.Where(obj => obj.CargaEntrega.Codigo == codigoCargaEntrega);

            if (parametrosConsulta != null)
            {
                result = result.OrderBy(parametrosConsulta.PropriedadeOrdenar + " " + parametrosConsulta.DirecaoOrdenar);

                if (parametrosConsulta.InicioRegistros > 0 || parametrosConsulta.LimiteRegistros > 0)
                    result = result.Skip(parametrosConsulta.InicioRegistros).Take(parametrosConsulta.LimiteRegistros);
            }

            return result.ToList();
        }

        public int ContarConsultarCargaEntrega(int codigoCargaEntrega)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaGuiaTransporteAnimal>();

            var result = query.Where(obj => obj.CargaEntrega.Codigo == codigoCargaEntrega);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaGuiaTransporteAnimal> BuscarPorCargaEntregaCodigoCarga(int codigoCarga)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaGuiaTransporteAnimal>();
            var result = query.Where(obj => obj.CargaEntrega.Carga.Codigo == codigoCarga);
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaGuiaTransporteAnimal BuscarPorCargaEntregaCodigoBarras(int codigoCargaEntrega, string codigoBarrasGta)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaGuiaTransporteAnimal>();
            var result = query.Where(obj => obj.CargaEntrega.Codigo == codigoCargaEntrega && obj.CodigoBarras == codigoBarrasGta);

            return result.FirstOrDefault();
        }

    }
}

