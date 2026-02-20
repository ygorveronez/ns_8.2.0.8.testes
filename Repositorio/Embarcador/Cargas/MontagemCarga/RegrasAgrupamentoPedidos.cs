using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas.MontagemCarga
{
    public class RegrasAgrupamentoPedidos : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.RegrasAgrupamentoPedidos>
    {
        public RegrasAgrupamentoPedidos(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public RegrasAgrupamentoPedidos(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Task<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.RegrasAgrupamentoPedidos> BuscarRegraAsync(int filial)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.RegrasAgrupamentoPedidos>();
            var result = from obj in query where obj.Ativo select obj;

            if (filial > 0)
                result = result.Where(obj => obj.Filial.Codigo == filial);
            else
                result = result.Where(obj => obj.Filial == null);

            return result.FirstOrDefaultAsync(CancellationToken);
        }

        public Dominio.Entidades.Embarcador.Cargas.MontagemCarga.RegrasAgrupamentoPedidos BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.RegrasAgrupamentoPedidos>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.MontagemCarga.RegrasAgrupamentoPedidos BuscarPorFilial(int filial)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.RegrasAgrupamentoPedidos>();
            var result = from obj in query where obj.Ativo select obj;

            if (filial > 0)
                result = result.Where(obj => obj.Filial.Codigo == filial);
            else
                result = result.Where(obj => obj.Filial == null);

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.RegrasAgrupamentoPedidos> Consultar(int filial, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.RegrasAgrupamentoPedidos>();

            var result = from obj in query select obj;

            if (filial > 0)
                result = result.Where(obj => obj.Filial.Codigo == filial);

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Ativo == true);

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => obj.Ativo == false);


            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();

        }

        public int ContarConsulta(int filial, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.RegrasAgrupamentoPedidos>();

            var result = from obj in query select obj;

            if (filial > 0)
                result = result.Where(obj => obj.Filial.Codigo == filial);

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Ativo == true);

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => obj.Ativo == false);
            return result.Count();
        }

    }
}
