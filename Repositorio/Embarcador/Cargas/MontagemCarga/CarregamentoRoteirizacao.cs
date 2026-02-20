using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas.MontagemCarga
{
    public class CarregamentoRoteirizacao : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao>
    {
        public CarregamentoRoteirizacao(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public CarregamentoRoteirizacao(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao>();
            var resut = from obj in query where obj.Codigo == codigo select obj;
            return resut.FirstOrDefault();
        }

        public async Task<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao> BuscarPorCarregamentoAsync(int carregamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao>();
            var resut = from obj in query where obj.Carregamento.Codigo == carregamento select obj;
            return await resut.FirstOrDefaultAsync();
        }

        public Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao BuscarPorCarregamento(int carregamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao>();
            var resut = from obj in query where obj.Carregamento.Codigo == carregamento select obj;
            return resut.FirstOrDefault();
        }

        public Task<List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao>> BuscarPorCarregamentosAsync(List<int> carregamentos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao>();
            var resut = from obj in query where carregamentos.Contains(obj.Carregamento.Codigo) select obj;
            return resut.ToListAsync(CancellationToken);
        }
    }
}
