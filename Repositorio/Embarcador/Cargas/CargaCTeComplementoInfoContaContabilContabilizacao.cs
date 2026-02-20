using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;



namespace Repositorio.Embarcador.Cargas
{
    public class CargaCTeComplementoInfoContaContabilContabilizacao : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfoContaContabilContabilizacao>
    {
        public CargaCTeComplementoInfoContaContabilContabilizacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfoContaContabilContabilizacao> BuscarPorOcorrencia(int ocorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfoContaContabilContabilizacao>();
            var result = from obj in query where obj.CargaCTeComplementoInfo.CargaOcorrencia.Codigo == ocorrencia select obj;
            return result
                .Fetch(obj => obj.PlanoConta)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfoContaContabilContabilizacao> BuscarPorCargaCTeComplementoInfo(int cargaCTeComplementoInfo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfoContaContabilContabilizacao>();
            var result = from obj in query where obj.CargaCTeComplementoInfo.Codigo == cargaCTeComplementoInfo select obj;
            return result
                .Fetch(obj => obj.PlanoConta)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfoContaContabilContabilizacao> BuscarPorFechamento(int fechamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfoContaContabilContabilizacao>();
            var result = from obj in query where obj.CargaCTeComplementoInfo.FechamentoFrete.Codigo == fechamento select obj;
            return result
                .Fetch(obj => obj.PlanoConta)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfoContaContabilContabilizacao> BuscarPorCodigos(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfoContaContabilContabilizacao>();
            var result = from obj in query where codigos.Contains(obj.CargaCTeComplementoInfo.Codigo) select obj;
            return result
                .Fetch(obj => obj.PlanoConta)
                .ToList();
        }


        public int DeletarPorOcorrencia(int ocorrencia)
        {
            string hql = "DELETE CargaCTeComplementoInfoContaContabilContabilizacao where CargaCTeComplementoInfo.Codigo in (select obj.Codigo from CargaCTeComplementoInfo obj where obj.CargaOcorrencia.Codigo = :Ocorrencia)";
            var query = this.SessionNHiBernate.CreateQuery(hql);
            query.SetInt32("Ocorrencia", ocorrencia);
            return query.ExecuteUpdate();
        }

        public async Task<int> DeletarPorOcorrenciaAsync(int ocorrencia)
        {
            string hql = "DELETE CargaCTeComplementoInfoContaContabilContabilizacao where CargaCTeComplementoInfo.Codigo in (select obj.Codigo from CargaCTeComplementoInfo obj where obj.CargaOcorrencia.Codigo = :Ocorrencia)";

            var query = this.SessionNHiBernate.CreateQuery(hql);
            query.SetInt32("Ocorrencia", ocorrencia);

            return await query.ExecuteUpdateAsync();
        }

        public int DeletarPorFechamento(int fechamento)
        {
            string hql = "DELETE CargaCTeComplementoInfoContaContabilContabilizacao where CargaCTeComplementoInfo.Codigo in (select obj.Codigo from CargaCTeComplementoInfo obj where obj.FechamentoFrete.Codigo = :Fechamento)";
            var query = this.SessionNHiBernate.CreateQuery(hql);
            query.SetInt32("Fechamento", fechamento);
            return query.ExecuteUpdate();
        }

        public int DeletarPorComplementoInfo(int codigoComplemento)
        {
            string hql = "DELETE CargaCTeComplementoInfoContaContabilContabilizacao where CargaCTeComplementoInfo.Codigo = :CodigoComplemento";
            var query = this.SessionNHiBernate.CreateQuery(hql);
            query.SetInt32("CodigoComplemento", codigoComplemento);
            return query.ExecuteUpdate();
        }
    }
}
