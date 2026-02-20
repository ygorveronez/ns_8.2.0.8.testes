using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using System;

namespace Repositorio.Embarcador.Logistica
{
    public class PracaPedagioTarifa : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.PracaPedagioTarifa>
    {
        #region Construtores

        public PracaPedagioTarifa(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Logistica.PracaPedagioTarifa BuscarPorCodigo(int codigo)
        {
            var pracaPedagioTarifa = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PracaPedagioTarifa>()
                .Where(o => o.Codigo == codigo)
                .FirstOrDefault();

            return pracaPedagioTarifa;
        }

        public List<Dominio.Entidades.Embarcador.Logistica.PracaPedagioTarifa> BuscarPorPracaPedagio(int codigoPracaPedagio)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PracaPedagioTarifa>()
                .Where(o => o.PracaPedagio.Codigo == codigoPracaPedagio);

            return query.OrderBy(o => o.ModeloVeicularCarga.Descricao).ToList();
        }

        public Dominio.Entidades.Embarcador.Logistica.PracaPedagioTarifa BuscarPorPracaPedagioModeloVeicularCarga(int codigoPracaPedagio, int codigoModeloVeicularCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PracaPedagioTarifa>()
                .Where(o => o.PracaPedagio.Codigo == codigoPracaPedagio && o.ModeloVeicularCarga.Codigo == codigoModeloVeicularCarga);

            return query.FirstOrDefault();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Frete.TarifaModeloVeicular> BuscarSumarizadasPorRotaFrete(int codigoRotaFrete)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Frete.TarifaModeloVeicular> pracasPedagioTarifaSumarizadas = new List<Dominio.ObjetosDeValor.Embarcador.Frete.TarifaModeloVeicular>();

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.RotaFrete>();
            var result = from obj in query where obj.Codigo == codigoRotaFrete select obj;
            Dominio.Entidades.RotaFrete rotaFrete = result.FirstOrDefault();
            if (rotaFrete != null)
            {
                List<Dominio.Entidades.Embarcador.Logistica.PracaPedagio> pracasPedagio = rotaFrete.PracasPedagio.ToList();
                int total = pracasPedagio.Count();
                for (int i = 0; i < total; i++)
                {
                    List<Dominio.Entidades.Embarcador.Logistica.PracaPedagioTarifa> pracasPedagioTarifa = pracasPedagio[i].PracaPedagioTarifa.ToList();
                    int totalTarifas = pracasPedagioTarifa.Count();
                    for (int j = 0; j < totalTarifas; j++)
                    {
                        bool encontrou = false;
                        int totalPracasPedagioTarifaSumarizadas = pracasPedagioTarifaSumarizadas.Count;
                        for (int k = 0; k < totalPracasPedagioTarifaSumarizadas; k++)
                        {
                            if (pracasPedagioTarifaSumarizadas[k].ModeloVeicularCarga.Codigo == pracasPedagioTarifa[j].ModeloVeicularCarga.Codigo)
                            {
                                pracasPedagioTarifaSumarizadas[k].Tarifa += pracasPedagioTarifa[j].Tarifa;
                                encontrou = true;
                                break;
                            }
                        }
                        if (!encontrou)
                        {
                            pracasPedagioTarifaSumarizadas.Add(new Dominio.ObjetosDeValor.Embarcador.Frete.TarifaModeloVeicular
                            {
                                ModeloVeicularCarga = pracasPedagioTarifa[j].ModeloVeicularCarga,
                                Tarifa = pracasPedagioTarifa[j].Tarifa
                            });
                        }
                    }
                }
                pracasPedagioTarifaSumarizadas.Sort((x, y) => String.Compare(x.ModeloVeicularCarga.Descricao, y.ModeloVeicularCarga.Descricao));
            }
            return pracasPedagioTarifaSumarizadas;
        }

        #endregion
    }
}
