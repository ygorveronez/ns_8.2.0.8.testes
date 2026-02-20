using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaLicenca : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaLicenca>
    {
        public CargaLicenca(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public CargaLicenca(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Cargas.CargaLicenca BuscarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaLicenca>()
                .Where(obj => obj.Carga.Codigo == codigoCarga);

            return query.FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Cargas.CargaLicenca> BuscarPorCargaAsync(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaLicenca>()
                .Where(obj => obj.Carga.Codigo == codigoCarga);

            return query.FirstOrDefaultAsync();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaLicenca> BuscarPorCargas(List<int> codigosCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaLicenca>()
                .Where(obj => codigosCarga.Contains(obj.Carga.Codigo));

            return query.ToList();
        }

        public List<(int CodigoCarga, string Mensagem)> BuscarMensagemPorCargas(List<int> codigosCarga)
        {
            var consultaCargaLicenca = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaLicenca>()
                .Where(licenca => codigosCarga.Contains(licenca.Carga.Codigo));

            return consultaCargaLicenca
                .Select(licenca => ValueTuple.Create(licenca.Carga.Codigo, licenca.Mensagem))
                .ToList();
        }

        public string BuscarNumeroUltimaCarga(int codigoVeiculoLicenca)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaLicenca>()
                .Where(obj => obj.LicencaVeiculo.Codigo == codigoVeiculoLicenca).OrderByDescending(obj => obj.Codigo);

            return query.Select(o => o.Carga.CodigoCargaEmbarcador).FirstOrDefault();
        }

        #endregion
    }
}
