using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Veiculos
{
    public class Macro : RepositorioBase<Dominio.Entidades.Embarcador.Veiculos.Macro>
    {
        public Macro(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Veiculos.Macro BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.Macro>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Veiculos.Macro BuscarPorCodigoIntegracao(string codigoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.Macro>();
            var result = from obj in query where obj.CodigoIntegracao == codigoIntegracao select obj;
            return result.FirstOrDefault();
        }

        public bool ValidarCodigoIntegracao(string codigoIntegracao, int codigoDiferente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.Macro>();
            var result = from obj in query where obj.CodigoIntegracao == codigoIntegracao && obj.Codigo != codigoDiferente select obj;
            return result.Count() == 0;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Veiculos.Macro> _Consultar(string descricao, string codigoIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.Macro>();

            var result = from obj in query select obj;

            // Filtros
            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(codigoIntegracao))
                result = result.Where(o => o.CodigoIntegracao == codigoIntegracao);

            if (status != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
            {
                if(status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                    result = result.Where(o => o.Ativo == true);
                else
                    result = result.Where(o => o.Ativo == false);
            }

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Veiculos.Macro> Consultar(string descricao, string codigoIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(descricao, codigoIntegracao, status);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(string descricao, string codigoIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status)
        {
            var result = _Consultar(descricao, codigoIntegracao, status);

            return result.Count();
        }
    }
}
