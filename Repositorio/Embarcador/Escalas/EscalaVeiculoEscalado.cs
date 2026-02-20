using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Escalas
{
    public class EscalaVeiculoEscalado : RepositorioBase<Dominio.Entidades.Embarcador.Escalas.EscalaVeiculoEscalado>
    {
        #region Construtores

        public EscalaVeiculoEscalado(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Escalas.EscalaVeiculoEscalado BuscarPorCodigo(int codigo)
        {
            var consultaEscalaVeiculoEscalado = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escalas.EscalaVeiculoEscalado>()
                .Where(o => o.Codigo == codigo);

            return consultaEscalaVeiculoEscalado.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Escalas.EscalaVeiculoEscalado> BuscarPorGeracaoEscala(int codigoGeracaoEscala)
        {
            var consultaEscalaVeiculoEscalado = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escalas.EscalaVeiculoEscalado>()
                .Where(o => o.EscalaOrigemDestino.ExpedicaoEscala.GeracaoEscala.Codigo == codigoGeracaoEscala);

            consultaEscalaVeiculoEscalado = consultaEscalaVeiculoEscalado
                .Fetch(o => o.EscalaOrigemDestino).ThenFetch(o => o.ExpedicaoEscala);

            return consultaEscalaVeiculoEscalado.ToList();
        }

        public bool VerificarRotaEscalaAnteriorPossuiMesmaClassificacao(int codigoVeiculo, DateTime dataEscalaBase, RotaFreteClasse rotaFreteClasse)
        {
            var consultaEscalaVeiculoEscalado = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escalas.EscalaVeiculoEscalado>()
                .Where(o =>
                    o.Veiculos.Any(v => v.Codigo == codigoVeiculo) &&
                    o.EscalaOrigemDestino.ExpedicaoEscala.GeracaoEscala.DataEscala <= dataEscalaBase
                );

            DateTime? dataUltimaEscala = consultaEscalaVeiculoEscalado.Max(o => (DateTime?)o.EscalaOrigemDestino.ExpedicaoEscala.GeracaoEscala.DataEscala);

            if (!dataUltimaEscala.HasValue)
                return false;

            consultaEscalaVeiculoEscalado = consultaEscalaVeiculoEscalado.Where(o =>
                o.EscalaOrigemDestino.ExpedicaoEscala.GeracaoEscala.DataEscala == dataUltimaEscala.Value &&
                o.EscalaOrigemDestino.Rota.Classificacao.Classe == rotaFreteClasse
            );

            return consultaEscalaVeiculoEscalado.Count() > 0;
        }

        #endregion
    }
}
