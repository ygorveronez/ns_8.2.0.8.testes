using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;

namespace Repositorio.Embarcador.Avarias
{
    public class TempoEtapaLote : RepositorioBase<Dominio.Entidades.Embarcador.Avarias.TempoEtapaLote>
    {
        public TempoEtapaLote(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public TempoEtapaLote(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.Embarcador.Avarias.TempoEtapaLote BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.TempoEtapaLote>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Avarias.TempoEtapaLote> BuscarUltimasEtapasLotes(List<int> codigosLotes)
        {
            int take = 1000;
            int start = 0;
            List<Dominio.Entidades.Embarcador.Avarias.TempoEtapaLote> resultado = new List<Dominio.Entidades.Embarcador.Avarias.TempoEtapaLote>();

            while (start < codigosLotes.Count)
            {
                List<int> loteCodigos = codigosLotes.Skip(start).Take(take).ToList();

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.TempoEtapaLote>()
                    .Where(obj => loteCodigos.Contains(obj.Lote.Codigo))
                    .ToList();

                resultado.AddRange(query);
                start += take;
            }

            return resultado;
        }

        public Dominio.Entidades.Embarcador.Avarias.TempoEtapaLote BuscarUltimaEtapa(int codigoLote)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.TempoEtapaLote>();
            var result = from obj in query
                         where obj.Lote.Codigo == codigoLote && obj.Saida == null
                         select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Avarias.TempoEtapaLote> BuscarPorLote(int codigoLote)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.TempoEtapaLote>();
            var result = from obj in query where obj.Lote.Codigo == codigoLote select obj;
            return result.ToList();
        }

        public string TempoNaEtapa(int codigoLote, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaLote etapa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.TempoEtapaLote>();

            var result = from obj in query
                         where
                            obj.Lote.Codigo == codigoLote &&
                            obj.Etapa == etapa &&
                            obj.Saida != null
                         select obj;

            var tempos = result.ToList();

            double horas = 0;
            foreach (var tempo in tempos)
                horas = horas + tempo.Saida.Value.Subtract(tempo.Entrada).TotalHours;

            return horas.ToString("n1").Replace(',', '.') + "h";
        }
    }
}