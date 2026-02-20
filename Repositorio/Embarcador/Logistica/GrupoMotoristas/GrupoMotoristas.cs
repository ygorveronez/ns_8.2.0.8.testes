using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NHibernate.Linq;

namespace Repositorio.Embarcador.Logistica.GrupoMotoristas
{
    public class GrupoMotoristas(UnitOfWork unitOfWork, CancellationToken cancellationToken)
        : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.GrupoMotoristas>(unitOfWork, cancellationToken)
    {

        #region Métodos Públicos

        public async Task<List<Dominio.Entidades.Embarcador.Logistica.GrupoMotoristas>> BuscarAsync(
            Dominio.ObjetosDeValor.Embarcador.Logistica.GrupoMotoristas.FiltroPesquisaGrupoMotoristas filtro,
            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaGrupoMotoristas = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.GrupoMotoristas>();

            consultaGrupoMotoristas = AplicarFiltros(consultaGrupoMotoristas, filtro);

            return await ObterListaAsync(consultaGrupoMotoristas, parametrosConsulta);
        }

        #endregion

        private static IQueryable<Dominio.Entidades.Embarcador.Logistica.GrupoMotoristas> AplicarFiltros(
            IQueryable<Dominio.Entidades.Embarcador.Logistica.GrupoMotoristas> consulta,
            Dominio.ObjetosDeValor.Embarcador.Logistica.GrupoMotoristas.FiltroPesquisaGrupoMotoristas filtro)
        {
            if (filtro.Descricao != null && filtro.Descricao != string.Empty)
            {
                consulta = consulta.Where(po => po.Descricao.Contains(filtro.Descricao));
            }

            if (filtro.CodigoIntegracao != null && filtro.CodigoIntegracao != string.Empty)
            {
                consulta = consulta.Where(po => po.CodigoIntegracao.Equals(filtro.CodigoIntegracao));
            }

            if (filtro.Situacao != null)
            {
                consulta = consulta.Where(po => po.Situacao.Equals(filtro.Situacao));
            }

            if (filtro.Ativo != null)
                consulta = consulta.Where(po => po.Ativo == filtro.Ativo);


            return consulta;
        }

        public async Task<int> ContarConsultaAsync(Dominio.ObjetosDeValor.Embarcador.Logistica.GrupoMotoristas.FiltroPesquisaGrupoMotoristas filtro)
        {
            var consulta = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.GrupoMotoristas>();

            consulta = AplicarFiltros(consulta, filtro);

            return await consulta.CountAsync(CancellationToken);
        }
    }
}
