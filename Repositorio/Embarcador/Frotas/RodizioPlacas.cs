using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Repositorio.Embarcador.Frotas
{
    public class RodizioPlacas : RepositorioBase<Dominio.Entidades.Embarcador.Frotas.RodizioPlacas>
    {
        public RodizioPlacas(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Frotas.RodizioPlacas> Pesquisar(int codFilial, string finaisDePlacas, int diaDaSemana)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frotas.RodizioPlacas>();

            if(diaDaSemana > 0)
                query = query.Where(x => x.DiaSemana == (DiaSemana)diaDaSemana);

            if (codFilial > 0)
                query = query.Where(x => x.Filial.Codigo == codFilial);

            if (!string.IsNullOrWhiteSpace(finaisDePlacas))
            {
                var finais = finaisDePlacas.Split(new char[] { ';', ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).Where(x => x != null && x.Trim().Length > 0).Select(x => x).ToList();
                foreach(var p in finais)
                    query = query.Where(x => x.FinaisPlaca.Contains(p));
            }

            return query.ToList();

        }

    }
}
