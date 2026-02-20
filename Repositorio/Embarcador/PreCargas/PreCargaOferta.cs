using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.PreCargas
{
    public class PreCargaOferta : RepositorioBase<Dominio.Entidades.Embarcador.PreCargas.PreCargaOferta>
    {
        #region Construtores

        public PreCargaOferta(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.PreCargas.PreCargaOferta BuscarPorPreCarga(int codigoPreCarga)
        {
            var consultaPreCargaOferta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PreCargas.PreCargaOferta>()
                .Where(oferta => oferta.PreCarga.Codigo == codigoPreCarga);

            return consultaPreCargaOferta.FirstOrDefault();
        }

        public List<int> BuscarCodigosPreCargaOfertaLiberar()
        {
            var consultaPreCargaOferta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PreCargas.PreCargaOferta>()
                .Where(preCargaOferta =>
                    preCargaOferta.DataLiberacao != null &&
                    preCargaOferta.DataLiberacao.Value <= DateTime.Now &&
                    preCargaOferta.Situacao == SituacaoPreCargaOferta.AguardandoLiberacao &&
                    preCargaOferta.PreCarga.SituacaoPreCarga != SituacaoPreCarga.CargaGerada &&
                    preCargaOferta.PreCarga.Empresa == null
                );

            return consultaPreCargaOferta
                .Select(preCargaOferta => preCargaOferta.Codigo)
                .ToList();
        }

        #endregion Métodos Públicos
    }
}
