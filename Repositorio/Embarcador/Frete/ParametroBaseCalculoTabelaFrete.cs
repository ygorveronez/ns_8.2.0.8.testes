using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Frete
{
    public class ParametroBaseCalculoTabelaFrete : RepositorioBase<Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete>
    {
        public ParametroBaseCalculoTabelaFrete(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete Buscar(int codigoTabelaFrete, int codigoObjeto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete>();

            var result = from obj in query where obj.CodigoObjeto == codigoObjeto && obj.TabelaFrete.Codigo == codigoTabelaFrete select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete> BuscarPorTabelaFrete(int codigoTabelaFrete)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete>();

            var result = from obj in query where obj.TabelaFrete.Codigo == codigoTabelaFrete select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete> BuscarDiff(int codigoTabelaFrete, int[] codigosParametros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete>();

            var result = from obj in query where obj.TabelaFrete.Codigo == codigoTabelaFrete && !codigosParametros.Contains(obj.Codigo) select obj;

            return result.ToList();
        }

        public void ExecutarInsercaoDeParametros(List<string> listaValoresSql)
        {
            for (int i = 0; i < listaValoresSql.Count; i += 1000)
            {
                string sqlParaExecutar = "INSERT INTO T_TABELA_FRETE_PARAMETRO_BASE_CALCULO (TBC_CODIGO_OBJETO, TFC_CODIGO) VALUES ";
                
                List<string> valoresInserir = listaValoresSql.Skip(i).Take(1000).ToList();
                sqlParaExecutar += string.Join(", ", valoresInserir);
                
                var query = this.SessionNHiBernate.CreateSQLQuery(sqlParaExecutar);

                query.ExecuteUpdate();
            }
        }
    }
}
