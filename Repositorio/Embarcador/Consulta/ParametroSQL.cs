using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Type;

namespace Repositorio.Embarcador.Consulta
{
    public class ParametroSQL
    {
        public ParametroSQL(string nomeParametro, object valor)
        {
            NomeParametro = nomeParametro;
            Valor = valor;
            TipoDb = ObterTipo(valor);
        }

        public ParametroSQL(string nomeParametro, IEnumerable<string> valor)
        {
            NomeParametro = nomeParametro;
            Valor = valor?.ToList();
            IsLista = true;
        }

        public string NomeParametro { get;  }
        public object Valor { get;  }
        public bool IsLista { get; }
        public IType TipoDb { get; }

        private IType ObterTipo(object valor)
        {
            if (valor == null) return NHibernate.NHibernateUtil.String;

            return valor switch
            {
                string => NHibernate.NHibernateUtil.String,
                int => NHibernate.NHibernateUtil.Int32,
                long => NHibernate.NHibernateUtil.Int64,
                double => NHibernate.NHibernateUtil.Double,
                decimal => NHibernate.NHibernateUtil.Decimal,
                bool => NHibernate.NHibernateUtil.Boolean,
                DateTime => NHibernate.NHibernateUtil.DateTime,
                _ when valor.GetType().IsEnum => NHibernate.NHibernateUtil.Int32,
                _ => NHibernate.NHibernateUtil.String
            };
        }

        

    }
}
