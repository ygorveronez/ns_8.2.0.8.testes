using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Pedidos
{
    public class CanalVenda : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.CanalVenda>
    {
        public CanalVenda(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Pedidos.CanalVenda BuscarPorCodigoIntegracao(string codigoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.CanalVenda>();

            var result = from obj in query where obj.CodigoIntegracao == codigoIntegracao && obj.Ativo select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.CanalVenda> BuscarPorCodigosIntegracao(List<string> codigoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.CanalVenda>();

            var result = from obj in query where codigoIntegracao.Contains(obj.CodigoIntegracao) && obj.Ativo select obj;

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.CanalVenda BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.CanalVenda>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.CanalVenda> BuscarPorCodigos(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.CanalVenda>();
            var result = from obj in query where codigos.Contains(obj.Codigo) select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.CanalVenda> Consultar(int codigoFilial, string descricao, string codigoIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.CanalVenda>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(codigoIntegracao))
                result = result.Where(obj => obj.CodigoIntegracao == codigoIntegracao);

            if (codigoFilial > 0)
                result = result.Where(obj => obj.Filial.Codigo == codigoFilial);

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Ativo == true);
            else if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => obj.Ativo == false);

            if (maximoRegistros > 0)
                return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
            else
                return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).ToList();

        }

        public int ContarConsulta(int codigoFilial, string descricao, string codigoIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.CanalVenda>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(codigoIntegracao))
                result = result.Where(obj => obj.CodigoIntegracao == codigoIntegracao);

            if (codigoFilial > 0)
                result = result.Where(obj => obj.Filial.Codigo == codigoFilial);

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Ativo == true);
            else if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => obj.Ativo == false);

            return result.Count();
        }
    }
}