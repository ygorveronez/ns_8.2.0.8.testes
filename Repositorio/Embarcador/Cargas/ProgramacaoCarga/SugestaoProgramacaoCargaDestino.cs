using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Cargas.ProgramacaoCarga
{
    public sealed class SugestaoProgramacaoCargaDestino : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCargaDestino>
    {
        #region Construtores

        public SugestaoProgramacaoCargaDestino(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Públicos

        public List<Dominio.Entidades.Localidade> BuscarDestinosPorSugestaoProgramacaoCarga(int codigoSugestaoProgramacaoCarga)
        {
            var consultaDestino = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCargaDestino>()
                .Where(destino => destino.SugestaoProgramacaoCarga.Codigo == codigoSugestaoProgramacaoCarga);

            return consultaDestino
                .Select(destino => destino.Localidade)
                .ToList();
        }

        public List<(int CodigoSugestaoProgramacaoCarga, string Destino)> BuscarPorSugestoesProgramacaoCarga(List<int> codigosSugestaoProgramacaoCarga)
        {
            var consultaDestino = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCargaDestino>()
                .Where(destino => codigosSugestaoProgramacaoCarga.Contains(destino.SugestaoProgramacaoCarga.Codigo));

            return consultaDestino
                .Select(destino => ValueTuple.Create(destino.SugestaoProgramacaoCarga.Codigo, destino.Localidade.Descricao))
                .ToList();
        }

        #endregion Métodos Públicos
    }
}
