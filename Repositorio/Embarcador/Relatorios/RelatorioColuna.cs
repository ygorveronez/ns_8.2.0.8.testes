using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;


namespace Repositorio.Embarcador.Relatorios
{
    public class RelatorioColuna : RepositorioBase<Dominio.Entidades.Embarcador.Relatorios.RelatorioColuna>
    {
        public RelatorioColuna(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public RelatorioColuna(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.Embarcador.Relatorios.RelatorioColuna BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Relatorios.RelatorioColuna>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Relatorios.RelatorioColuna> BuscarPorRelatorio(int relatorio)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Relatorios.RelatorioColuna>();
            var result = from obj in query where obj.Relatorio.Codigo == relatorio select obj;
            return result.ToList();
        }

        public async Task<List<Dominio.Entidades.Embarcador.Relatorios.RelatorioColuna>> BuscarPorRelatorioAsync(int relatorio, CancellationToken cancellationToken)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Relatorios.RelatorioColuna>();
            var result = from obj in query where obj.Relatorio.Codigo == relatorio select obj;
            return await result.ToListAsync(cancellationToken);
        }


    }
}
