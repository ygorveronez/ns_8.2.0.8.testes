using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;

namespace Repositorio.Embarcador.CTe
{
    public class FilaEnvioIntegrador : RepositorioBase<Dominio.Entidades.Embarcador.CTe.FilaEnvioIntegrador>
    {
        public FilaEnvioIntegrador(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.CTe.FilaEnvioIntegrador BuscarPorCodigo(int codigo)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.FilaEnvioIntegrador>();
            var result = query.Where(obj => obj.Codigo == codigo);
            return result.FirstOrDefault();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.CTe.FilaEnvioIntegrador> BuscarFilaEnvioIntegrador()
        {
            IQueryable<Dominio.Entidades.Embarcador.CTe.FilaEnvioIntegrador> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.FilaEnvioIntegrador>();

            return query.Where(obj => obj.Ativo)
                .Select(o => new Dominio.ObjetosDeValor.Embarcador.CTe.FilaEnvioIntegrador
                {
                    Codigo = o.Codigo,
                    CodigoTipoEnvio = o.CodigoTipoEnvio,
                    Ativo = o.Ativo
                }).ToList();
        }
    }
}
