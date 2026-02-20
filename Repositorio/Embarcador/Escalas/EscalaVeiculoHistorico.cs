using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;

namespace Repositorio.Embarcador.Escalas
{
    public sealed class EscalaVeiculoHistorico : RepositorioBase<Dominio.Entidades.Embarcador.Escalas.EscalaVeiculoHistorico>
    {
        #region Construtores

        public EscalaVeiculoHistorico(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Escalas.EscalaVeiculoHistorico> Consultar(int codigoEscalaVeiculo)
        {
            var consultaEscalaVeiculoHistorico = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escalas.EscalaVeiculoHistorico>();

            consultaEscalaVeiculoHistorico = consultaEscalaVeiculoHistorico.Where(o => o.EscalaVeiculo.Codigo == codigoEscalaVeiculo);

            return consultaEscalaVeiculoHistorico;
        }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Escalas.EscalaVeiculoHistorico> Consultar(int codigoEscalaVeiculo, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaEscalaVeiculoHistorico = Consultar(codigoEscalaVeiculo);

            return ObterLista(consultaEscalaVeiculoHistorico, parametrosConsulta);
        }

        public int ContarConsulta(int codigoEscalaVeiculo)
        {
            var consultaEscalaVeiculoHistorico = Consultar(codigoEscalaVeiculo);

            return consultaEscalaVeiculoHistorico.Count();
        }

        #endregion
    }
}
