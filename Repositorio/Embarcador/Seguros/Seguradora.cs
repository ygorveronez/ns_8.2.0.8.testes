using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Seguros
{
    public class Seguradora : RepositorioBase<Dominio.Entidades.Embarcador.Seguros.Seguradora>
    {
        public Seguradora(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Seguros.Seguradora BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Seguros.Seguradora>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Seguros.Seguradora BuscarPorCNPJ(double cnpj)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Seguros.Seguradora>();

            var result = from obj in query where obj.ClienteSeguradora.CPF_CNPJ == cnpj select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Seguros.Seguradora> Consultar(string nome, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa? ativo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Seguros.Seguradora>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(nome))
                result = result.Where(obj => obj.Nome.Contains(nome));

            if (ativo.HasValue)
            {
                if (ativo.Value == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                    result = result.Where(obj => obj.Ativo == true);
                else if (ativo.Value == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                    result = result.Where(obj => obj.Ativo == false);
            }
            
            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();

        }

        public int ContarConsulta(string nome, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa? ativo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Seguros.Seguradora>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(nome))
                result = result.Where(obj => obj.Nome.Contains(nome));

            if (ativo.HasValue)
            {
                if (ativo.Value == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                    result = result.Where(obj => obj.Ativo == true);
                else if (ativo.Value == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                    result = result.Where(obj => obj.Ativo == false);
            }

            return result.Count();
        }
    }
}
