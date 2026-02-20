using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Dominio.ObjetosDeValor.Enumerador;

namespace Repositorio.Embarcador.Configuracoes
{
    public class Licenca : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.Licenca>
    {
        public Licenca(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.Licenca BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.Licenca>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Configuracoes.Licenca BuscarPorDescricao(string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.Licenca>();
            var result = from obj in query where obj.Descricao.Equals(descricao) select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Configuracoes.Licenca> Consultar(int codigoEmpresa, string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, int tipoLicenca , string propOrdena, string dirOrdena, int inicio, int limite)
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.Licenca> query = ObterConsulta(codigoEmpresa, descricao, ativo, tipoLicenca);

            return query.OrderBy(propOrdena + " " + dirOrdena).Skip(inicio).Take(limite).ToList();
        }

        public int ContarConsulta(int codigoEmpresa, string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, int tipoLicenca)
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.Licenca> query = ObterConsulta(codigoEmpresa, descricao, ativo, tipoLicenca);

            return query.Count();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Configuracoes.Licenca> ObterConsulta(int codigoEmpresa, string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, int tipoLicenca)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.Licenca>();

            if (!string.IsNullOrWhiteSpace(descricao))
                query = query.Where(o => o.Descricao.Contains(descricao));

            if (codigoEmpresa > 0)
                query = query.Where(o => o.Empresa.Codigo == codigoEmpresa);

            if (ativo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
            {
                if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                    query = query.Where(o => o.Ativo);
                else
                    query = query.Where(o => !o.Ativo);
            }

            if (tipoLicenca > -1 ) 
            {
                TipoLicenca tipoEnum = (TipoLicenca)tipoLicenca;
                query = query.Where(o => (o.Tipo != null && o.Tipo == tipoEnum) || (o.Tipo == null && tipoEnum == TipoLicenca.Geral) );
            }

            return query;
        }

        #endregion
    }
}
