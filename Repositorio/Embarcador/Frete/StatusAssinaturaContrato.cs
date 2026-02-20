using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Frete
{
    public class StatusAssinaturaContrato : RepositorioBase<Dominio.Entidades.Embarcador.Frete.StatusAssinaturaContrato>
    {
        #region Construtores
        public StatusAssinaturaContrato(UnitOfWork unitOfWork) : base(unitOfWork) { }
        #endregion

        #region Metodos Publicos
        public Dominio.Entidades.Embarcador.Frete.StatusAssinaturaContrato BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.StatusAssinaturaContrato>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frete.StatusAssinaturaContrato> Consultar(string codigoIntegracao, string descricao, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var result = Consultar(codigoIntegracao, descricao);

            return ObterLista(result, parametrosConsulta);
        }

        public int ContarConsulta(string codigoIntegracao, string descricao)
        {
            var result = Consultar(codigoIntegracao, descricao);

            return result.Count();
        }

        public Dominio.Entidades.Embarcador.Frete.StatusAssinaturaContrato BuscarPorCodigoIntegracao(string codigoIntegracao)
        {
            var result = Consultar(codigoIntegracao, "");
            return result.FirstOrDefault();
        }
        #endregion

        #region Metodos Privados
        private IQueryable<Dominio.Entidades.Embarcador.Frete.StatusAssinaturaContrato> Consultar(string codigoIntegracao, string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.StatusAssinaturaContrato>();
            query = from obj in query select obj;

            if (!string.IsNullOrEmpty(codigoIntegracao))
                query = query.Where(obj => obj.CodigoIntegracao.Equals(codigoIntegracao));

            if (!string.IsNullOrEmpty(descricao))
                query = query.Where(obj => obj.Descricao.Contains(descricao));

            return query;
        }
        #endregion
    }
}
