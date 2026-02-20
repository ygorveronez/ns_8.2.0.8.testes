using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Canhotos
{
    public class LocalArmazenamentoCanhoto : RepositorioBase<Dominio.Entidades.Embarcador.Canhotos.LocalArmazenamentoCanhoto>
    {
        public LocalArmazenamentoCanhoto(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Canhotos.LocalArmazenamentoCanhoto BuscarPorCodigo(int codigo)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.LocalArmazenamentoCanhoto>();
            var result = query.Where(obj => obj.Codigo == codigo);
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Canhotos.LocalArmazenamentoCanhoto BuscarLocalArmanemantoAtual()
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.LocalArmazenamentoCanhoto>();
            var result = query.Where(obj => obj.LocalArmazenagemAtual);
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Canhotos.LocalArmazenamentoCanhoto BuscarLocalArmanemantoAtualFilial(int filial)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.LocalArmazenamentoCanhoto>();
            var result = query.Where(obj => obj.LocalArmazenagemAtual && obj.Filial.Codigo == filial);
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Canhotos.LocalArmazenamentoCanhoto BuscarLocalArmanemantoAtualPorTipoFilial(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto tipoCanhoto, int filial)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.LocalArmazenamentoCanhoto>();
            var result = query.Where(obj => obj.LocalArmazenagemAtual && obj.Filial.Codigo == filial);
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Canhotos.LocalArmazenamentoCanhoto BuscarLocalArmanemantoAtual(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto tipoCanhoto)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.LocalArmazenamentoCanhoto>();
            var result = query.Where(obj => obj.LocalArmazenagemAtual && obj.TipoCanhoto == tipoCanhoto);
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Canhotos.LocalArmazenamentoCanhoto> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto tipoCanhoto, int filial, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.LocalArmazenamentoCanhoto>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (tipoCanhoto != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.Todos)
                result = result.Where(obj => obj.TipoCanhoto == tipoCanhoto);

            if (filial > 0)
                result = result.Where(obj => obj.Filial.Codigo == filial);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();

        }

        public int ContarConsulta(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto tipoCanhoto, int filial)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.LocalArmazenamentoCanhoto>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (tipoCanhoto != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.Todos)
                result = result.Where(obj => obj.TipoCanhoto == tipoCanhoto);

            if (filial > 0)
                result = result.Where(obj => obj.Filial.Codigo == filial);

            return result.Count();
        }
    }
}
