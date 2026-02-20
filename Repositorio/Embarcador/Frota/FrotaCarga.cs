using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Frota
{
    public class FrotaCarga : RepositorioBase<Dominio.Entidades.Embarcador.Frota.FrotaCarga>
    {
        public FrotaCarga(UnitOfWork unitOfWork) : base(unitOfWork) { }


        #region Metodos Publicos

        public Dominio.Entidades.Embarcador.Frota.FrotaCarga BuscarPorFrotaOuCargaEData(int codigoFrota, int CodigoCarga, DateTime dataCarregamento)
        {
            IQueryable<Dominio.Entidades.Embarcador.Frota.FrotaCarga> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.FrotaCarga>();
            query = query.Where(o => o.DataCarregamento.Date == dataCarregamento.Date && (o.Frota.Codigo == codigoFrota || o.Carga.Codigo == CodigoCarga));

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Frota.FrotaCarga ExisteProgramacaoFuturaParaFrota(int codigoFrota, DateTime? dataComparativo = null)
        {
            if (dataComparativo == null)
                dataComparativo = DateTime.Now.Date;

            IQueryable<Dominio.Entidades.Embarcador.Frota.FrotaCarga> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.FrotaCarga>();
            query = query.Where(o => o.Frota.Codigo == codigoFrota && o.DataCarregamento.Date >= dataComparativo);

            return query.FirstOrDefault();
        }


        public bool ExisteProgramacaoFuturaParaMotorista(int codigoMotorista, DateTime? dataComparativo = null)
        {
            if (dataComparativo == null)
                dataComparativo = DateTime.Now.Date;

            IQueryable<Dominio.Entidades.Embarcador.Frota.FrotaCarga> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.FrotaCarga>();
            query = query.Where(o => (o.Frota.Motorista.Codigo == codigoMotorista || o.Frota.MotoristaAuxiliar.Codigo == codigoMotorista) && o.DataCarregamento.Date >= dataComparativo);

            return query.Any();
        }

        public List<Dominio.Entidades.Embarcador.Frota.FrotaCarga> BuscarFrotaCargaFuturaPorMotorista(int codigoMotorista, DateTime? dataComparativo = null)
        {
            if (dataComparativo == null)
                dataComparativo = DateTime.Now.Date;

            IQueryable<Dominio.Entidades.Embarcador.Frota.FrotaCarga> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.FrotaCarga>();
            query = query.Where(o => (o.Frota.Motorista.Codigo == codigoMotorista || o.Frota.MotoristaAuxiliar.Codigo == codigoMotorista) && o.DataCarregamento.Date >= dataComparativo);

            return query.ToList();
        }

        #endregion

        #region Metodos Privados


        #endregion

    }
}
