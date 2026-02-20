using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Pedidos
{
    public class CanalEntrega : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.CanalEntrega>
    {
        public CanalEntrega(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public CanalEntrega(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.Embarcador.Pedidos.CanalEntrega BuscarPorCodigoIntegracao(string codigoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.CanalEntrega>();

            var result = from obj in query where obj.CodigoIntegracao == codigoIntegracao && obj.Ativo select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.CanalEntrega> BuscarPorCodigosIntegracao(List<string> codigoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.CanalEntrega>();

            var result = from obj in query where codigoIntegracao.Contains(obj.CodigoIntegracao) && obj.Ativo select obj;

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.CanalEntrega BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.CanalEntrega>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.CanalEntrega> BuscarPorCodigos(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.CanalEntrega>();
            var result = from obj in query where codigos.Contains(obj.Codigo) select obj;
            return result.ToList();
        }

        public async Task<List<Dominio.Entidades.Embarcador.Pedidos.CanalEntrega>> BuscarPorCodigosAsync(List<int> codigos)
        {
            return await this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.CanalEntrega>()
                .Where(x => codigos.Contains(x.Codigo)).ToListAsync();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.CanalEntrega> BuscarPorCanalEntregaPrincipal(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.CanalEntrega>();
            var result = from obj in query where obj.CanalEntregaPrincipal.Codigo == codigo && obj.Ativo select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.CanalEntrega> Consultar(int codigoFilial, string descricao, string codigoIntegracao, bool filtrarCanaisEntregaPrincipal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.CanalEntrega>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(codigoIntegracao))
                result = result.Where(obj => obj.CodigoIntegracao == codigoIntegracao);

            if (codigoFilial > 0)
                result = result.Where(obj => obj.Filial.Codigo == codigoFilial);

            if (filtrarCanaisEntregaPrincipal)
                result = result.Where(obj => obj.Principal);

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Ativo == true);
            else if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => obj.Ativo == false);

            if (maximoRegistros > 0)
                return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
            else
                return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).ToList();

        }

        public int ContarConsulta(int codigoFilial, string descricao, string codigoIntegracao, bool filtrarCanaisEntregaPrincipal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.CanalEntrega>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(codigoIntegracao))
                result = result.Where(obj => obj.CodigoIntegracao == codigoIntegracao);

            if (codigoFilial > 0)
                result = result.Where(obj => obj.Filial.Codigo == codigoFilial);

            if (filtrarCanaisEntregaPrincipal)
                result = result.Where(obj => obj.Principal);

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Ativo == true);
            else if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => obj.Ativo == false);

            return result.Count();
        }
    }
}
