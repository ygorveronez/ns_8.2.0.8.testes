using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Frete.Pontuacao
{
    public class PontuacaoPorModeloCarroceria : RepositorioBase<Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorModeloCarroceria>
    {
        #region Construtores

        public PontuacaoPorModeloCarroceria(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorModeloCarroceria> BuscarTodas(int inicio, int limite)
        {
            var consultaPontuacaoPorModeloCarroceria = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorModeloCarroceria>();

            return consultaPontuacaoPorModeloCarroceria.OrderBy(o => o.Codigo).Skip(inicio).Take(limite).ToList();
        }

        public int ContarTodas()
        {
            var consultaPontuacaoPorModeloCarroceria = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorModeloCarroceria>();

            return consultaPontuacaoPorModeloCarroceria.Count();
        }

        public bool VerificarExistePorModeloCarroceria(int codigoModeloCarroceria)
        {
            return VerificarExistePorModeloCarroceria(codigoModeloCarroceria, codigo: 0);
        }

        public bool VerificarExistePorModeloCarroceria(int codigoModeloCarroceria, int codigo)
        {
            var consultaPontuacaoPorModeloCarroceria = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorModeloCarroceria>()
                .Where(o => o.ModeloCarroceria.Codigo == codigoModeloCarroceria && o.Codigo != codigo);

            return consultaPontuacaoPorModeloCarroceria.Count() > 0;
        }

        #endregion
    }
}
