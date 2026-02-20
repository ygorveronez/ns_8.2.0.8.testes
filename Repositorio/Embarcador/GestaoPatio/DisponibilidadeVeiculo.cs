using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.GestaoPatio
{
    public class DisponibilidadeVeiculo : RepositorioBase<Dominio.Entidades.Embarcador.GestaoPatio.DisponibilidadeVeiculo>
    {
        public DisponibilidadeVeiculo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.GestaoPatio.DisponibilidadeVeiculo BuscarPorVeiculo(int veiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.DisponibilidadeVeiculo>();
            var result = from obj in query where obj.Veiculo.Codigo == veiculo select obj;
            return result.FirstOrDefault();
        }

        private IQueryable<Dominio.Entidades.Embarcador.GestaoPatio.DisponibilidadeVeiculo> _ConsultarDisponibilidadeVeiculo(int transportador, int contratoFrete, DateTime dataVigencia, Dominio.ObjetosDeValor.Embarcador.Enumeradores.PesquisaDisponibilidadeVeiculo pesquisaDisponibilidadeVeiculo)
        {
            //var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio>();

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.DisponibilidadeVeiculo>();

            var queryContratoFrete = (from cfv in this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorVeiculo>()
                                      where
                                      cfv.ContratoFrete.Ativo
                                      && cfv.ContratoFrete.DataInicial.Date <= dataVigencia.Date
                                      && cfv.ContratoFrete.DataFinal.Date >= dataVigencia.Date
                                      select cfv);

            var result = from obj in query
                         where
                            queryContratoFrete.Any(v => v.Veiculo.Codigo == obj.Veiculo.Codigo)
                         //&& queryDisponibilidadeVeiculo.Any(v => v.Veiculo.Codigo == obj.Veiculo.Codigo)
                         select obj;

            if (contratoFrete > 0)
                queryContratoFrete = queryContratoFrete.Where(cfv => cfv.ContratoFrete.Codigo == contratoFrete);

            if (transportador > 0)
                result = result.Where(o => o.Veiculo.Empresa.Codigo == transportador);

            if (pesquisaDisponibilidadeVeiculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.PesquisaDisponibilidadeVeiculo.Disponivel)
                result = result.Where(o => o.Disponivel.HasValue);

            if (pesquisaDisponibilidadeVeiculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.PesquisaDisponibilidadeVeiculo.EmViagem)
                result = result.Where(o => !o.Disponivel.HasValue);

            return result;
        }

        public List<Dominio.Entidades.Embarcador.GestaoPatio.DisponibilidadeVeiculo> ConsultarDisponibilidadeVeiculo(int transportador, int contratoFrete, DateTime dataVigencia, Dominio.ObjetosDeValor.Embarcador.Enumeradores.PesquisaDisponibilidadeVeiculo pesquisaDisponibilidadeVeiculo, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _ConsultarDisponibilidadeVeiculo(transportador, contratoFrete, dataVigencia, pesquisaDisponibilidadeVeiculo);

            //if (!string.IsNullOrWhiteSpace(propOrdena))
            //    result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsultaDisponibilidadeVeiculo(int transportador, int contratoFrete, DateTime dataVigencia, Dominio.ObjetosDeValor.Embarcador.Enumeradores.PesquisaDisponibilidadeVeiculo pesquisaDisponibilidadeVeiculo)
        {
            var result = _ConsultarDisponibilidadeVeiculo(transportador, contratoFrete, dataVigencia, pesquisaDisponibilidadeVeiculo);

            return result.Count();
        }
    }
}