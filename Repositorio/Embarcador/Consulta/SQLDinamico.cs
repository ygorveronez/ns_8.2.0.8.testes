using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;

namespace Repositorio.Embarcador.Consulta 
{
    public class SQLDinamico
    {
        public SQLDinamico(string stringQuery, List<ParametroSQL> parametros)
        {
            StringQuery = stringQuery;
            Parametros = parametros ?? new List<ParametroSQL>();
        }

        public string StringQuery { get; set; }
        public List<ParametroSQL> Parametros { get; set; } 

        public IQuery CriarQuery(ISession session)
        {
            IQuery query = session.CreateSQLQuery(StringQuery);

            foreach (var parametro in Parametros)
            {
                if (parametro.IsLista && parametro.Valor != null)
                {
                    var lista = ((IEnumerable)parametro.Valor).Cast<object>().ToList();

                    query.SetParameterList(parametro.NomeParametro, lista);
                }
                else
                {
                    query.SetParameter(parametro.NomeParametro, parametro.Valor);
                }
            }

            return query;
        }

        public ISQLQuery CriarSQLQuery(ISession session)
        {
            ISQLQuery query = session.CreateSQLQuery(StringQuery);

            foreach (var parametro in Parametros)
            {
                if (parametro.IsLista && parametro.Valor != null)
                {
                    var lista = ((IEnumerable)parametro.Valor).Cast<object>().ToList();

                    query.SetParameterList(parametro.NomeParametro, lista);
                }
                else
                {
                    query.SetParameter(parametro.NomeParametro, parametro.Valor);
                }
            }

            return query;
        }
    }
}
