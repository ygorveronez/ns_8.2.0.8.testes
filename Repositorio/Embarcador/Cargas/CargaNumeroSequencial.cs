using System.Linq;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaNumeroSequencial : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaNumeroSequencial>
    {
        public CargaNumeroSequencial(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public int ObterProximoCodigo(int? codigoFilial)
        {
            string sql = $@"select MAX(cargaSequencial.CNS_NUMERO_SEQUENCIAL) from T_CARGA_NUMERO_SEQUENCIAL cargaSequencial WITH(NOLOCK)";

            if (codigoFilial.HasValue)
            {
                if (codigoFilial.Value > 0)
                    sql += $" where cargaSequencial.FIL_CODIGO = {codigoFilial}";
                else
                    sql += $" where cargaSequencial.FIL_CODIGO is null";
            }

            var consultaCarga = this.SessionNHiBernate.CreateSQLQuery(sql);

            int? retorno = consultaCarga.UniqueResult<int?>();
            return retorno.HasValue ? (retorno.Value + 1) : 0;
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaNumeroSequencial BuscarPorFilial(int? codigoFilial)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaNumeroSequencial>();

            var result = query.Where(obj => obj.Filial.Codigo == codigoFilial);

            return result.FirstOrDefault();
        }
    }
}
