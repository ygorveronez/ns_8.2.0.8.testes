using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Financeiro
{
    public class FormasTitulo : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.FormasTitulo>
    {
        public FormasTitulo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Financeiro.FormasTitulo BuscarPorCodigo(int codigo, List<Dominio.Entidades.Embarcador.Financeiro.FormasTitulo> listaFormasTitulo = null)
        {
            if (listaFormasTitulo == null)
            {
                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.FormasTitulo>();
                var result = from obj in query where obj.Codigo == codigo select obj;
                return result.FirstOrDefault();
            }
            else
                return listaFormasTitulo.Where(obj => obj.Codigo == codigo).FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.FormasTitulo> BuscarPorCodigos(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.FormasTitulo>();

            var result = from obj in query where codigos.Contains(obj.Codigo) select obj;                        
            
            return result.ToList();
        }

        public dynamic Consultar(string codigoIntegracao, string descricao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.FormasTitulo>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            if (!string.IsNullOrEmpty(codigoIntegracao))
                result.Where(o => o.CodigoIntegracao.Equals(codigoIntegracao));

            return result.OrderBy(o => o.CodigoIntegracao).Skip(inicioRegistros).Take(maximoRegistros).Select(o => new { o.Codigo, o.CodigoIntegracao, o.Descricao }).ToList();
        }

        public int ContarConsulta(string codigoIntegracao, string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.FormasTitulo>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(codigoIntegracao))
                result = result.Where(o => o.CodigoIntegracao.Equals(codigoIntegracao));

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.FormasTitulo> Consultar(string codigoIntegracao, string descricao, string propriedadeOrdenacao, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.FormasTitulo>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            if (!string.IsNullOrEmpty(codigoIntegracao))
                result = result.Where(o => o.CodigoIntegracao.Equals(codigoIntegracao));

            return result.OrderBy(propriedadeOrdenacao + (direcaoOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public bool ContemFormaTituloCadastrado(string codigoIntegracao, int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.FormasTitulo>();

            query = query.Where(obj => obj.CodigoIntegracao == codigoIntegracao);
            if (!string.IsNullOrEmpty(codigoIntegracao))
                query = query.Where(obj => obj.Codigo != codigo);

            return query.Any();
        }
        public Dominio.Entidades.Embarcador.Financeiro.FormasTitulo BuscarPorDescricao(string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.FormasTitulo>();
            var result = from obj in query where obj.Descricao == descricao select obj;
            return result.FirstOrDefault();
        }

    }
}
