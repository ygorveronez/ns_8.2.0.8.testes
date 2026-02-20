using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.CTe
{
    public class CTeTerceiroXML : RepositorioBase<Dominio.Entidades.Embarcador.CTe.CTeTerceiroXML>
    {
        public CTeTerceiroXML(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.CTe.CTeTerceiroXML BuscarPorCodigo(int codigo)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeTerceiroXML>();
            var result = query.Where(obj => obj.Codigo == codigo);
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.CTe.CTeTerceiroXML BuscarPorCodigoCTeTerceiro(int codigoCTeTerceiro)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeTerceiroXML>();
            var result = query.Where(obj => obj.CTeTerceiro.Codigo == codigoCTeTerceiro);
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroXML> BuscarPorCodigosCTesTerceiros(List<int> codigosCTesTerceiros)
        {
            List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroXML> result = new List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroXML>();

            int take = 1000;
            int start = 0;

            while (start < codigosCTesTerceiros.Count)
            {
                List<int> tmp = codigosCTesTerceiros.Skip(start).Take(take).ToList();

                var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeTerceiroXML>();

                var filter = from obj in query
                             where tmp.Contains(obj.CTeTerceiro.Codigo)
                             select obj;

                result.AddRange(filter.ToList());

                start += take;
            }

            return result;
        }

        public bool ExisteCteTerceiroXML(int codigo)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeTerceiroXML>();
            var result = query.Where(obj => obj.CTeTerceiro.Codigo == codigo);
            return result.Any();
        }
    }
}
