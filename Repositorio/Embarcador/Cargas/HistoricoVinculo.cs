using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Repositorio.Embarcador.Cargas
{
    public class HistoricoVinculo : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.HistoricoVinculo>
    {
        public HistoricoVinculo(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public HistoricoVinculo(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #region Métodos Públicos

        public bool ExisteRegistroIgual(Dominio.Entidades.Veiculo veiculoTracao, ICollection<Dominio.Entidades.Veiculo> veiculoReboques,
                                ICollection<Dominio.Entidades.Usuario> motoristas, DateTime? dataVinculo, DateTime? dataDesvinculo,
                                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.Entidades.Embarcador.Cargas.Carga carga,
                                Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamento)
        {
            var result = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.HistoricoVinculo>();

            if (veiculoTracao != null)
                result = result.Where(h => h.VeiculoTracao == veiculoTracao);

            if (veiculoReboques != null && veiculoReboques.Any())
                result = result.Where(h => h.VeiculoReboques.Any(r => veiculoReboques.Contains(r)));

            if (motoristas != null && motoristas.Any())
                result = result.Where(h => h.Motoristas.Any(m => motoristas.Contains(m)));

            var intervaloMinutos = TimeSpan.FromMinutes(2);
            if (dataVinculo.HasValue)
            {
                var inicio = dataVinculo.Value - intervaloMinutos;
                var fim = dataVinculo.Value + intervaloMinutos;

                result = result.Where(h => h.DataHoraVinculo >= inicio && h.DataHoraVinculo <= fim);
            }

            if (dataDesvinculo.HasValue)
            {
                var inicio = dataDesvinculo.Value - intervaloMinutos;
                var fim = dataDesvinculo.Value + intervaloMinutos;

                result = result.Where(h => h.DataHoraDesvinculo >= inicio && h.DataHoraDesvinculo <= fim);
            }

            // Aplicar os filtros opcionais de Pedido, Carga e FilaCarregamento corretamente
            if (pedido != null || carga != null || filaCarregamento != null)
            {
                result = result.Where(h =>
                    (pedido != null && h.Pedido == pedido) ||
                    (carga != null && h.Carga == carga) ||
                    (filaCarregamento != null && h.FilaCarregamento == filaCarregamento)
                );
            }

            return result.Any();
        }



        /* public Dominio.Entidades.Embarcador.Cargas.HistoricoVinculo BuscarVinculoAtivo(Dominio.Entidades.Veiculo veiculoTracao, ICollection<Dominio.Entidades.Veiculo> veiculoReboques,
                                             ICollection<Usuario> motoristas, Pedido pedido, Carga carga, FilaCarregamentoVeiculo filaCarregamento)
         {
             var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.HistoricoVinculo>();

             if (veiculoTracao != null)
                 query = query.Where(x => x.VeiculoTracao == veiculoTracao);

            if (veiculoReboques != null && veiculoReboques.Any())
                 query = query.Where(x => x.VeiculoReboques.IsIn(veiculoReboques.ToArray()));

             if (motoristas != null && motoristas.Any())
                 query = query.Where(x => x.Motoristas.IsIn(motoristas.ToArray()));

             if (pedido != null)
                 query = query.Where(x => x.Pedido == pedido);

            if (carga != null)
                 query = query.Where(x => x.Carga == carga);

            if (filaCarregamento != null)
                 query = query.Where(x => x.FilaCarregamento == filaCarregamento);


             query = query.Where(x => x.DataHoraDesvinculo == null);

             var vinculoExistente = query.OrderBy("DataHoraVinculo ascending").ToList();

             foreach (var vinculo in vinculoExistente)
             {
                 var vinculoDesvinculado = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.HistoricoVinculo>()
                     .Where(x => x.VeiculoTracao == veiculoTracao)
                     .Where(x => x.Pedido == pedido)
                     .Where(x => x.Carga == carga)
                     .Where(x => x.FilaCarregamento == filaCarregamento)
                     .Where(x => x.DataHoraDesvinculo != null) // Buscar apenas os desvinculados
                     .SingleOrDefault();

                 if (vinculoDesvinculado == null)
                 {
                     return vinculo;
                 }
             }

             return null;
         }
        */

        #endregion

        #region Relatorio Historico Vinculo

        public IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.HistoricoVinculo> ConsultarRelatorio(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioHistoricoVinculo filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consulta = new ConsultaHistoricoVinculo().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.HistoricoVinculo)));

            return consulta.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.HistoricoVinculo>();
        }

        public int ContarConsultaRelatorio(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioHistoricoVinculo filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var consulta = new ConsultaHistoricoVinculo().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consulta.SetTimeout(600).UniqueResult<int>();
        }


        #endregion
    }
}
