using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaDadosSumarizados : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaDadosSumarizados>
    {
        public CargaDadosSumarizados(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<(int CodigoCarga, Dominio.Entidades.Embarcador.Cargas.CargaDadosSumarizados DadosSumarizados)> BuscarPorCargas(List<int> cargas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Carga>();
            var result = from obj in query select obj;

            result = result.Where(p => cargas.Contains(p.Codigo));

            return result.Select(o => ValueTuple.Create(o.Codigo, o.DadosSumarizados)).ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaDadosSumarizados BuscarPorCarga(int codigoCarga)
        {
            var consultaCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Carga>()
                .Where(o => o.Codigo == codigoCarga)
                .Select(o => o.DadosSumarizados);

            return consultaCarga.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaDadosSumarizados BuscarPorPedido(int codigoPedido)
        {
            var consultaCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(o => o.Pedido.Codigo == codigoPedido)
                .Select(o => o.Carga.DadosSumarizados);

            return consultaCargaPedido.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaDadosSumarizados BuscarPorCargaComFetch(int codigoCarga)
        {
            var consultaCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Carga>()
                .Where(o => o.Codigo == codigoCarga)
                .Select(o => o.DadosSumarizados);

            return consultaCarga
                .FetchMany(o => o.ClientesRemetentes)
                .ThenFetch(o => o.Localidade)
                .FirstOrDefault();
        }

        #endregion
    }
}
