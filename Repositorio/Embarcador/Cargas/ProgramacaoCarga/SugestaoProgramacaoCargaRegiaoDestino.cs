using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Cargas.ProgramacaoCarga
{
    public sealed class SugestaoProgramacaoCargaRegiaoDestino : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCargaRegiaoDestino>
    {
        #region Construtores

        public SugestaoProgramacaoCargaRegiaoDestino(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Localidades.Regiao> BuscarRegioesPorSugestaoProgramacaoCarga(int codigoSugestaoProgramacaoCarga)
        {
            var consultaRegiaoDestino = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCargaRegiaoDestino>()
                .Where(regiaoDestino => regiaoDestino.SugestaoProgramacaoCarga.Codigo == codigoSugestaoProgramacaoCarga);

            return consultaRegiaoDestino
                .Select(regiaoDestino => regiaoDestino.Regiao)
                .ToList();
        }

        public List<(int CodigoSugestaoProgramacaoCarga, string Regiao)> BuscarPorSugestoesProgramacaoCarga(List<int> codigosSugestaoProgramacaoCarga)
        {
            var consultaRegiaoDestino = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCargaRegiaoDestino>()
                .Where(regiaoDestino => codigosSugestaoProgramacaoCarga.Contains(regiaoDestino.SugestaoProgramacaoCarga.Codigo));

            return consultaRegiaoDestino
                .Select(regiaoDestino => ValueTuple.Create(regiaoDestino.SugestaoProgramacaoCarga.Codigo, regiaoDestino.Regiao.Descricao))
                .ToList();
        }

        #endregion Métodos Públicos
    }
}
