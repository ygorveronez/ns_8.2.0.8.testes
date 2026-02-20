using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas.Ofertas
{
    public class ParametrosOfertasDadosOfertaDiaSemana : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertasDadosOfertaDiaSemana>
    {
        #region Construtores

        public ParametrosOfertasDadosOfertaDiaSemana(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion

        #region Métodos Públicos 

        public async Task InserirMuitosAsync(List<Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertasDadosOfertaDiaSemana> registros, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            for (int i = 0; i < registros.Count; i++)
            {
                var registro = registros[i];

                await InserirAsync(registro, auditado);
            }
        }

        public Task<List<Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertasDadosOfertaDiaSemana>> BuscarAsync(
            Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.FiltroPesquisaParametrosOfertasDadosOferta filtro,
            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta
        )
        {
            var consulta = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertasDadosOfertaDiaSemana>();

            consulta = AplicarFiltros(consulta, filtro);

            consulta.Fetch(ds => ds.ParametrosOfertasDadosOferta);

            return ObterListaAsync(consulta, parametrosConsulta);
        }

        public async Task DeletarPorDiaAsync(Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana dia, int codigoParametrosOfertasDadosOferta)
        {
            var op = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertasDadosOfertaDiaSemana>();

            op = op.Where(o => o.ParametrosOfertasDadosOferta.Codigo == codigoParametrosOfertasDadosOferta && o.DiaSemana == dia);

            await op.DeleteAsync();
        }

        public async Task DeletarPorCodigoDadosOfertaAsync(int codigoParametrosOfertasDadosOferta)
        {
            var op = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertasDadosOfertaDiaSemana>();

            op = op.Where(o => o.ParametrosOfertasDadosOferta.Codigo == codigoParametrosOfertasDadosOferta);

            await op.DeleteAsync();
        }

        public Task<int> ContarConsultaAsync(Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.FiltroPesquisaParametrosOfertasDadosOferta filtro)
        {
            var consulta = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertasDadosOfertaDiaSemana>();

            consulta = AplicarFiltros(consulta, filtro);

            return consulta.CountAsync(CancellationToken);
        }

        public Task<Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertasDadosOfertaDiaSemana> BuscarPorCodigoParametrosOfertaEPeriodoAsync(int codigoParametrosOfertas, DateTime data, CancellationToken cancellationToken)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertasDadosOfertaDiaSemana> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertasDadosOfertaDiaSemana>()
                .Where(diaSemana => diaSemana.ParametrosOfertasDadosOferta.ParametrosOfertas.Codigo == codigoParametrosOfertas &&
                    diaSemana.DiaSemana == DiaSemanaHelper.ObterDiaSemana(data) &&
                    diaSemana.ParametrosOfertasDadosOferta.HoraInicio <= data.TimeOfDay &&
                    diaSemana.ParametrosOfertasDadosOferta.HoraTermino >= data.TimeOfDay);

            return query.FirstOrDefaultAsync(cancellationToken);
        }

        public Task<List<Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertasDadosOfertaDiaSemana>> BuscarPorCodigoParametrosOfertaDadosOfertaAsync(int codigoParametrosOfertasDadosOferta, CancellationToken cancellationToken)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertasDadosOfertaDiaSemana> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertasDadosOfertaDiaSemana>()
                .Where(diaSemana => diaSemana.ParametrosOfertasDadosOferta.ParametrosOfertas.Codigo == codigoParametrosOfertasDadosOferta);
            return query.ToListAsync(cancellationToken);
        }


        public Task<List<Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertasDadosOfertaDiaSemana>> BuscarPorCodigosParametrosOfertaDadosOfertaAsync(List<int> codigosParametrosOfertasDadosOferta, CancellationToken cancellationToken)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertasDadosOfertaDiaSemana> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertasDadosOfertaDiaSemana>()
                .Where(diaSemana => codigosParametrosOfertasDadosOferta.Contains(diaSemana.ParametrosOfertasDadosOferta.Codigo));
            return query.ToListAsync(cancellationToken);
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertasDadosOfertaDiaSemana> AplicarFiltros(IQueryable<Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertasDadosOfertaDiaSemana> consulta, Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.FiltroPesquisaParametrosOfertasDadosOferta filtro)
        {


            if (filtro.CodigoParametrosOfertasDadosOferta > 0)
            {
                consulta = consulta.Where(po => po.ParametrosOfertasDadosOferta.Codigo.Equals(filtro.CodigoParametrosOfertasDadosOferta));
            }

            if (filtro.CodigoParametrosOfertas > 0)
            {
                consulta = consulta.Where(po => po.ParametrosOfertasDadosOferta.ParametrosOfertas.Codigo.Equals(filtro.CodigoParametrosOfertas));
            }

            return consulta;
        }

        #endregion

    }
}
