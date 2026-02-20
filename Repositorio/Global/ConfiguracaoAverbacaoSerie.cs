using System.Collections.Generic;
using System.Linq;


namespace Repositorio
{
    public class ConfiguracaoAverbacaoSerie : RepositorioBase<Dominio.Entidades.ConfiguracaoAverbacaoSerie>, Dominio.Interfaces.Repositorios.ConfiguracaoAverbacaoSerie
    {
        public ConfiguracaoAverbacaoSerie(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.ConfiguracaoAverbacaoSerie BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConfiguracaoAverbacaoSerie>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }
        public List<Dominio.Entidades.ConfiguracaoAverbacaoSerie> BuscarPorConfiguracao(int codigoConfiguracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConfiguracaoAverbacaoSerie>();
            var result = from obj in query where obj.Configuracao.Codigo == codigoConfiguracao select obj;
            return result.ToList();
        }

        public Dominio.Entidades.ConfiguracaoAverbacaoSerie BuscarPorSerie(int codigoSerie, bool naoAverbar)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConfiguracaoAverbacaoSerie>();
            var result = from obj in query where obj.EmpresaSerie.Codigo == codigoSerie && obj.NaoAverbar == naoAverbar select obj;
            return result.FirstOrDefault();
        }

    }
}
