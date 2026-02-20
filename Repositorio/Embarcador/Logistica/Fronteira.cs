using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{
    public class Fronteira: RepositorioBase<Dominio.Entidades.Embarcador.Logistica.Fronteira>
    {
         public Fronteira(UnitOfWork unitOfWork) : base(unitOfWork) { }

         public Dominio.Entidades.Embarcador.Logistica.Fronteira buscarPorCodigoEmbarcador(string codigoFronteiraEmbarcador)
         {
             var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Fronteira>();
             var result = from obj in query where obj.CodigoFronteiraEmbarcador == codigoFronteiraEmbarcador select obj;
             return result.FirstOrDefault();
         }

         public Dominio.Entidades.Embarcador.Logistica.Fronteira BuscarPorCodigo(int codigo)
         {
             var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Fronteira>();
             var result = from obj in query where obj.Codigo == codigo select obj;
             return result.FirstOrDefault();
         }

         public List<Dominio.Entidades.Embarcador.Logistica.Fronteira> Consultar(string descricao, int localidade, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
         {
             var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Fronteira>();

             var result = from obj in query select obj;

             if (!string.IsNullOrWhiteSpace(descricao))
                 result = result.Where(obj => obj.Descricao.Contains(descricao));

             if (localidade > 0)
                 result = result.Where(obj => obj.Localidade.Codigo == localidade);

             if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                 result = result.Where(obj => obj.Ativo == true);

             if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                 result = result.Where(obj => obj.Ativo == false);

             return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();

         }

         public int ContarConsulta(string descricao, int localidade, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo)
         {
             var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Fronteira>();

             var result = from obj in query select obj;

             if (!string.IsNullOrWhiteSpace(descricao))
                 result = result.Where(obj => obj.Descricao.Contains(descricao));

             if (localidade > 0)
                 result = result.Where(obj => obj.Localidade.Codigo == localidade);

             if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                 result = result.Where(obj => obj.Ativo == true);

             if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                 result = result.Where(obj => obj.Ativo == false);

             return result.Count();
         }

    }
}
