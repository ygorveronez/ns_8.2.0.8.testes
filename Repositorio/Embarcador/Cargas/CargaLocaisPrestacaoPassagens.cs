using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaLocaisPrestacaoPassagens : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacaoPassagens>
    {
        public CargaLocaisPrestacaoPassagens(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public CargaLocaisPrestacaoPassagens(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacaoPassagens BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacaoPassagens>();
            var result = from obj in query select obj;
            result = result.Where(p => p.Codigo == codigo);
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacaoPassagens> BuscarPorLocalPrestacao(int codCargaLocaPrestacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacaoPassagens>();
            var result = from obj in query where obj.CargaLocaisPrestacao.Codigo == codCargaLocaPrestacao select obj;
            return result.OrderBy(obj => obj.Posicao).ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacaoPassagens>> BuscarPorLocalPrestacaoAsync(int codCargaLocaPrestacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacaoPassagens>();
            var result = from obj in query where obj.CargaLocaisPrestacao.Codigo == codCargaLocaPrestacao select obj;
            return result.OrderBy(obj => obj.Posicao).ToListAsync(CancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacaoPassagens> BuscarPorLocaisPrestacao(List<int> codCargaLocaisPrestacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacaoPassagens>();
            var result = from obj in query where codCargaLocaisPrestacao.Contains(obj.CargaLocaisPrestacao.Codigo) select obj;
            return result.OrderBy(obj => obj.Posicao)
                .Fetch(obj => obj.CargaLocaisPrestacao)
                .Fetch(obj => obj.EstadoDePassagem)
                .ToList();
        }

        public int ContarPorLocalPrestacao(int codCargaLocaPrestacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacaoPassagens>();
            var result = from obj in query where obj.CargaLocaisPrestacao.Codigo == codCargaLocaPrestacao select obj;
            return result.Count();
        }

        public int ContarPorLocalPrestacaoEEstado(int codCargaLocaPrestacao, string estado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacaoPassagens>();
            var result = from obj in query where obj.CargaLocaisPrestacao.Codigo == codCargaLocaPrestacao && obj.EstadoDePassagem.Sigla == estado select obj;
            return result.Count();
        }

        public bool ExistePorLocalPrestacao(int codigoCargaLocaisPrestacao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacaoPassagens> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacaoPassagens>();

            query = query.Where(o => o.CargaLocaisPrestacao.Codigo == codigoCargaLocaisPrestacao);

            return query.Select(o => o.Codigo).Any();
        }
    }
}
