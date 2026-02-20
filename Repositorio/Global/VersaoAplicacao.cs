using NHibernate.Criterion;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio
{
    public class VersaoAplicacao : RepositorioBase<Dominio.Entidades.VersaoAplicacao>
    {
        #region Construtores

        public VersaoAplicacao(UnitOfWork unitOfWork) : base(unitOfWork) { }


        public Dominio.Entidades.VersaoAplicacao ConsultarPorAmbienteECliente(string ambiente, int codCliente) 
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.VersaoAplicacao>();
            var result = from obj in query where obj.Ambiente.Equals(ambiente) && obj.CodigoCliente.Equals(codCliente) select obj;
            return result.FirstOrDefault();
        }
        
        public long AtualizarNumeroVersao(Dominio.Entidades.VersaoAplicacao versao)  
        {
            return base.Inserir(versao);
        }

        #endregion
    }
}