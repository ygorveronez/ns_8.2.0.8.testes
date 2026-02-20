using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace AdminMultisoftware.Repositorio.Configuracoes
{
    public class InstanciaBase : RepositorioBase<AdminMultisoftware.Dominio.Entidades.Configuracoes.InstanciaBase>
    {
        public InstanciaBase(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public AdminMultisoftware.Dominio.Entidades.Configuracoes.InstanciaBase BuscarConfigPadrao()
        {
            var query = this.SessionNHiBernate.Query<AdminMultisoftware.Dominio.Entidades.Configuracoes.InstanciaBase>();
            var result = from obj in query select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Configuracoes.InstanciaBase> Consultar(string servidor, string usuario, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Configuracoes.InstanciaBase>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(servidor))
                result = result.Where(obj => obj.Servidor.Contains(servidor));
            
            if (!string.IsNullOrWhiteSpace(usuario))
                result = result.Where(obj => obj.Usuario.Contains(usuario));

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(string servidor, string usuario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Configuracoes.InstanciaBase>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(servidor))
                result = result.Where(obj => obj.Servidor.Contains(servidor));

            if (!string.IsNullOrWhiteSpace(usuario))
                result = result.Where(obj => obj.Usuario.Contains(usuario));

            return result.Count();
        }
    }
}
