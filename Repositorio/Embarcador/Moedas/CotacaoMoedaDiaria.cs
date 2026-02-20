using System;
using System.Linq;

namespace Repositorio.Embarcador.Moedas;

public class CotacaoMoedaDiaria : RepositorioBase<Dominio.Entidades.Embarcador.Moedas.CotacaoMoedaDiaria>
{
    public CotacaoMoedaDiaria(UnitOfWork unitOfWork) : base(unitOfWork) { }

    #region Métodos Públicos

    public decimal BuscarValorCotacaoDiaria(Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral moeda, DateTime dataBase)
    {
        var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Moedas.CotacaoMoedaDiaria>()
            .Where(o => o.MoedaCotacaoBancoCentral == moeda && o.DataConsulta == dataBase.Date);

        return query.Select(obj => obj.ValorMoeda).FirstOrDefault();
    }

    public bool ExisteCotacaoDiaria(Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral moeda, DateTime dataBase)
    {
        var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Moedas.CotacaoMoedaDiaria>()
            .Where(o => o.MoedaCotacaoBancoCentral == moeda && o.DataConsulta == dataBase.Date);

        return query.Any();
    }

    #endregion

    #region Métodos Privados

    #endregion
}
