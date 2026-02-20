using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Frotas
{
    public class GeracaoFrotaAutomatizada : RepositorioBase<Dominio.Entidades.Embarcador.Frotas.GeracaoFrotaAutomatizada>
    {
        #region Constructores

        public GeracaoFrotaAutomatizada(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Constructores

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Frotas.GeracaoFrotaAutomatizada BuscarPorCodigo(int codigo)
        {
            var consultaGeracaoFrotaAutomatizada = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frotas.GeracaoFrotaAutomatizada>()
                .Where(obj => obj.Codigo == codigo);

            return consultaGeracaoFrotaAutomatizada.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frotas.GeracaoFrotaAutomatizada> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaGeracaoFrotaAutomatizada = Consultar(descricao);

            return ObterLista(consultaGeracaoFrotaAutomatizada, parametrosConsulta);
        }

        public int ContarConsulta(string descricao)
        {
            var consultaGeracaoFrotaAutomatizada = Consultar(descricao);

            return consultaGeracaoFrotaAutomatizada.Count();
        }

        public bool ExisteConfiguracaoCadastrada(int codigoFilial)
        {
            var consultaGeracaoFrotaAutomatizada = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frotas.GeracaoFrotaAutomatizada>()
                .Where(x => x.Filiais.Any(y => y.Codigo == codigoFilial));

            return consultaGeracaoFrotaAutomatizada.Count() > 0;
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Frotas.GeracaoFrotaAutomatizada> Consultar(string descricao)
        {
            var consultaGeracaoFrotaAutomatizada = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frotas.GeracaoFrotaAutomatizada>();

            if (string.IsNullOrWhiteSpace(descricao))
                consultaGeracaoFrotaAutomatizada = consultaGeracaoFrotaAutomatizada.Where(o => o.Descricao.Contains(descricao));

            return consultaGeracaoFrotaAutomatizada;
        }

        #endregion Métodos Privados
    }
}
