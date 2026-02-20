using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;

namespace Repositorio.Embarcador.Frete
{
    public class TempoEtapaAjusteTabelaFrete : RepositorioBase<Dominio.Entidades.Embarcador.Frete.TempoEtapaAjusteTabelaFrete>
    {
        #region Construtores

        public TempoEtapaAjusteTabelaFrete(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public string BuscarTempoAjuste(int codigoAjusteTabelaFrete)
        {
            var consultaTempoEtapa = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TempoEtapaAjusteTabelaFrete>()
                .Where(o => o.AjusteTabelaFrete.Codigo == codigoAjusteTabelaFrete)
                .OrderBy(o => o.Codigo);

            List<Dominio.Entidades.Embarcador.Frete.TempoEtapaAjusteTabelaFrete> tempos = consultaTempoEtapa.ToList();

            if (tempos.Count == 0)
                return "--";

            double horas = 0;

            Dominio.Entidades.Embarcador.Frete.TempoEtapaAjusteTabelaFrete primeiroTempo = tempos.FirstOrDefault();
            Dominio.Entidades.Embarcador.Frete.TempoEtapaAjusteTabelaFrete ultimoTempo = tempos.LastOrDefault();

            if (ultimoTempo.Saida.HasValue)
                horas = ultimoTempo.Saida.Value.Subtract(primeiroTempo.Entrada).TotalHours;
            else
                horas = DateTime.Now.Subtract(primeiroTempo.Entrada).TotalHours;

            return horas.ToString("n1").Replace(',', '.') + "h";
        }

        public Dominio.Entidades.Embarcador.Frete.TempoEtapaAjusteTabelaFrete BuscarUltimaEtapa(int codigoAjusteTabelaFrete)
        {
            var consultaTempoEtapa = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TempoEtapaAjusteTabelaFrete>()
                .Where(o => o.AjusteTabelaFrete.Codigo == codigoAjusteTabelaFrete && o.Saida == null)
                .OrderByDescending(o => o.Codigo);

            return consultaTempoEtapa.FirstOrDefault();
        }

        #endregion
    }
}
