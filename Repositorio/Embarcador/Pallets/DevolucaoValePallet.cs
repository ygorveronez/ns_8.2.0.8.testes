using System.Linq;
using System.Linq.Dynamic.Core;
using System;
using System.Collections.Generic;

namespace Repositorio.Embarcador.Pallets
{
    public class DevolucaoValePallet : RepositorioBase<Dominio.Entidades.Embarcador.Pallets.DevolucaoValePallet>
    {
        #region Construtores

        public DevolucaoValePallet(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos
        public List<Dominio.Entidades.Embarcador.Pallets.DevolucaoValePallet> ConsultaPorFechamento(int fechamento, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.DevolucaoValePallet>();
            var result = from obj in query where obj.Fechamento.Codigo == fechamento select obj;

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsultaPorFechamento(int fechamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.DevolucaoValePallet>();
            var result = from obj in query where obj.Fechamento.Codigo == fechamento select obj;
            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Pallets.DevolucaoValePallet> BuscarPorData(DateTime dataInicial, DateTime dataFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.DevolucaoValePallet>();

            var result = from obj in query
                         where
                             obj.Fechamento == null
                             && obj.Data >= dataInicial
                             && obj.Data <= dataFinal
                             && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDevolucaoValePallet.Finalizada
                         select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pallets.DevolucaoValePallet> BuscarPorFechamento(int fechamento, bool? adicionado = null)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.DevolucaoValePallet>();

            var result = from obj in query where obj.Fechamento.Codigo == fechamento select obj;

            if (adicionado.HasValue)
                result = result.Where(o => o.AdicionarAoFechamento == adicionado.Value);

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Pallets.DevolucaoValePallet BuscarPorCodigo(int codigo)
        {
            var devolucaoValePallet = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.DevolucaoValePallet>()
                .Where(devolucao => devolucao.Codigo == codigo)
                .FirstOrDefault();

            return devolucaoValePallet;
        }

        public Dominio.Entidades.Embarcador.Pallets.DevolucaoValePallet BuscarPorValePallet(int codigo)
        {
            var devolucaoValePallet =  this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.DevolucaoValePallet>()
                .Where(devolucao => devolucao.ValePallet.Codigo == codigo)
                .FirstOrDefault();

            return devolucaoValePallet;
        }

        public int BuscarProximoNumero()
        {
            var consultaDevolucaoValePallet = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.DevolucaoValePallet>();
            int? ultimoNumero = consultaDevolucaoValePallet.Max(o => (int?)o.Numero);

            return ultimoNumero.HasValue ? (ultimoNumero.Value + 1) : 1;
        }

        #endregion
    }
}
