using Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.Rastreamento;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Logistica.GrupoMotoristas.Abstrato
{
    public class RepositorioRelacionamentoGrupoMotoristaIntegracao<T> : RepositorioBase<T> where T : Dominio.Entidades.EntidadeBase, Dominio.Interfaces.Embarcador.Logistica.GrupoMotoristas.IEntidadeRelacionamentoGrupoMotoristasIntegracao
    {
        protected RepositorioRelacionamentoGrupoMotoristaIntegracao(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        //public virtual async Task DeletarPorCodigoAsync(int codigo, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        //{
        //    var gm = SessionNHiBernate.Query<T>();
        //    await gm.Where(o => o.Codigo == codigo).DeleteAsync();
        //}

        public virtual async Task<List<T>> BuscarAsync(int codigoGrupoMotoristaIntegracao, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consulta = MontarConsulta(codigoGrupoMotoristaIntegracao);

            consulta = consulta.Fetch(x => x.GrupoMotoristasIntegracao);

            return await ObterListaAsync(consulta, parametrosConsulta);
        }

        //public virtual async Task<int> ContarBuscaAsync(Dominio.Interfaces.Embarcador.Logistica.GrupoMotoristas.IFiltroPesquisaRelacionamentosGrupoMotoristas filtro)
        //{
        //    var consulta = MontarConsulta(filtro);

        //    return await consulta.CountAsync();
        //}

        //public virtual async Task<List<T>> BuscarAsync(Dominio.ObjetosDeValor.FiltroPesquisaRelacionamentos filtro)
        //{
        //    return await ObterListaAsync(MontarConsulta(filtro), null);
        //}

        //public virtual async Task<int> ContarConsultaAsync(Dominio.ObjetosDeValor.FiltroPesquisaRelacionamentos filtro)
        //{
        //    return await MontarConsulta(filtro).CountAsync(CancellationToken);
        //}

        //public virtual async Task InserirMuitosAsync(List<T> entidades)
        //{
        //    await InserirAsync(entidades, _nomeTabela);
        //}

        

        protected virtual IQueryable<T> MontarConsulta(int codigoGrupoMotoristaIntegracao)
        {
            return SessionNHiBernate.Query<T>().Where(o => o.GrupoMotoristasIntegracao.Codigo.Equals(codigoGrupoMotoristaIntegracao));
        }
    }
}
