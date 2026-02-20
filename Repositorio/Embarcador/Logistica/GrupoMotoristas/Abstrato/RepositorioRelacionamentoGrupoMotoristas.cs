using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Logistica.GrupoMotoristas.Abstrato
{
    public abstract class RepositorioRelacionamentoGrupoMotoristas<T> : RepositorioBase<T> where T : Dominio.Entidades.EntidadeBase, Dominio.Interfaces.Embarcador.Logistica.GrupoMotoristas.IEntidadeRelacionamentoGrupoMotoristas
    {
        private readonly string _nomeTabela;

        protected RepositorioRelacionamentoGrupoMotoristas(UnitOfWork unitOfWork, CancellationToken cancellationToken, string nomeTabela) : base(unitOfWork, cancellationToken) 
        {
            _nomeTabela = nomeTabela;
        }


        public virtual async Task DeletarPorCodigoAsync(int codigo, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            var gm = SessionNHiBernate.Query<T>();
            await gm.Where(o => o.Codigo == codigo).DeleteAsync();
        }

        public virtual async Task<List<T>> BuscarAsync(int codigoGrupoMotorista, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consulta = MontarConsulta(codigoGrupoMotorista);

            consulta = consulta.Fetch(x => x.GrupoMotoristas);

            return await ObterListaAsync(consulta, parametrosConsulta);
        }

        public virtual async Task<List<T>> BuscarAsync(int codigoGrupoMotorista)
        {
            return await ObterListaAsync(MontarConsulta(codigoGrupoMotorista), null);
        }


        public virtual async Task InserirMuitosAsync(List<T> entidades)
        {
            await InserirAsync(entidades, _nomeTabela);
        }

        protected virtual IQueryable<T> MontarConsulta(int codigoGrupoMotorista)
        {
            return SessionNHiBernate.Query<T>().Where(o => o.GrupoMotoristas.Codigo.Equals(codigoGrupoMotorista));
        }
    }
}
