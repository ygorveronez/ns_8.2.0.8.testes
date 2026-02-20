using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Filiais
{
    public class GestaoPatioAlertaSla : RepositorioBase<Dominio.Entidades.Embarcador.Filiais.GestaoPatioAlertaSla>
    {
        #region Construtores

        public GestaoPatioAlertaSla(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Filiais.GestaoPatioAlertaSla> BuscarPorConfigurados()
        {
            var consultaGestaoPatioAlertaSla = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.GestaoPatioAlertaSla>()
                .Where(o =>
                    o.Emails.Length > 0 &&
                    o.TiposAlertaEmail.Count() > 0 &&
                    o.Etapas.Count() > 0
                );

            return consultaGestaoPatioAlertaSla.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Filiais.GestaoPatioAlertaSla> BuscarPorFilial(int codigoFilial)
        {
            var consultaGestaoPatioAlertaSla = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.GestaoPatioAlertaSla>()
                .Where(o => o.Filial.Codigo == codigoFilial);
            
            return consultaGestaoPatioAlertaSla
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Filiais.GestaoPatioAlertaSla> BuscarPorFiliais(List<int> codigosFilial)
        {
            var consultaGestaoPatioAlertaSla = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.GestaoPatioAlertaSla>()
                .Where(o => codigosFilial.Contains(o.Filial.Codigo));

            return consultaGestaoPatioAlertaSla
                .ToList();
        }

        #endregion
    }
}
