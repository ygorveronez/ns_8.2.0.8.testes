using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Importacoes
{
    public class ConfiguracaoImportacao : RepositorioBase<Dominio.Entidades.Embarcador.Importacoes.ConfiguracaoImportacao>
    {
        public ConfiguracaoImportacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Importacoes.ConfiguracaoImportacao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Importacoes.ConfiguracaoImportacao>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Importacoes.ConfiguracaoImportacao> Consultar(Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoControleImportacao codigoControle, string descricao, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(codigoControle, descricao);

            result = result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);
            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }


        public Dominio.Entidades.Embarcador.Importacoes.ConfiguracaoImportacao BuscarPorControle(Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoControleImportacao codigoControle)
        {
            var result = _Consultar(codigoControle, "");

            return result.FirstOrDefault();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoControleImportacao codigoControle, string descricao)
        {
            var result = _Consultar(codigoControle, descricao);

            return result.Count();
        }


        private IQueryable<Dominio.Entidades.Embarcador.Importacoes.ConfiguracaoImportacao> _Consultar(Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoControleImportacao codigoControle, string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Importacoes.ConfiguracaoImportacao>();

            var result = from obj in query where obj.CodigoControle == codigoControle select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            return result;
        }
    }
}
