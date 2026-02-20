using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;


namespace Repositorio.Embarcador.Anexo
{
    public class Anexo<TAnexo, TEntidadeAnexo> : RepositorioBase<TAnexo>
        where TAnexo : Dominio.Entidades.Embarcador.Anexo.Anexo<TEntidadeAnexo>
        where TEntidadeAnexo : Dominio.Entidades.EntidadeBase, Dominio.Interfaces.Embarcador.Entidade.IEntidade
    {
        #region Construtores

        public Anexo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Anexo(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion

        #region Métodos Privados

        private IQueryable<TAnexo> Consultar(int codigoEntidade)
        {
            var anexos = this.SessionNHiBernate.Query<TAnexo>()
                .Where(a => a.EntidadeAnexo.Codigo == codigoEntidade)
                .AsQueryable();

            return anexos;
        }

        #endregion

        #region Métodos Públicos

        public TEntidadeAnexo BuscarEntidadePorCodigo(int codigo)
        {
            var anexos = this.SessionNHiBernate.Query<TAnexo>()
                .Where(a => a.EntidadeAnexo.Codigo == codigo);

            return anexos.Select(o => o.EntidadeAnexo).FirstOrDefault();
        }

        public TAnexo BuscarPorCodigo(int codigo)
        {
            var anexo = this.SessionNHiBernate.Query<TAnexo>()
                .Where(a => a.Codigo == codigo)
                .FirstOrDefault();

            return anexo;
        }

        public List<TAnexo> BuscarPorEntidade(int codigo)
        {
            var anexos = this.SessionNHiBernate.Query<TAnexo>()
                .Where(a => a.EntidadeAnexo.Codigo == codigo);

            return anexos.ToList();
        }
        public async Task<List<TAnexo>> BuscarPorEntidadeAsync(int codigo)
        {
            var anexos = this.SessionNHiBernate.Query<TAnexo>()
                .Where(a => a.EntidadeAnexo.Codigo == codigo);

            return await anexos.ToListAsync();
        }

        public List<TAnexo> BuscarPorEntidades(List<int> codigos)
        {
            var anexos = this.SessionNHiBernate.Query<TAnexo>()
                .Where(a => codigos.Contains(a.EntidadeAnexo.Codigo));

            return anexos.ToList();
        }

        public List<TAnexo> Consultar(int codigoEntidade, string propriedadeOrdenar, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var anexos = Consultar(codigoEntidade);

            return ObterLista(anexos, propriedadeOrdenar, direcaoOrdenacao, inicioRegistros, maximoRegistros);
        }

        public int ContarConsulta(int codigoEntidade)
        {
            var anexos = Consultar(codigoEntidade);

            return anexos.Count();
        }

        #endregion
    }
}
