using System;

namespace Servicos.Embarcador.CTe;

public sealed class NumeracaoCTe
{
    #region Atributos

    private static readonly Lazy<NumeracaoCTe> _numeracaoCTe = new Lazy<NumeracaoCTe>(() => new NumeracaoCTe());
    private readonly object _lockSequencial = new object();

    #endregion Atributos

    #region Construtores

    private NumeracaoCTe() { }

    public static NumeracaoCTe GetInstance()
    {
        return _numeracaoCTe.Value;
    }

    #endregion Construtores

    #region Métodos Públicos

    public int ObterProximoNumero(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Repositorio.UnitOfWork unitOfWork)
    {
        lock (_lockSequencial)
        {
            Repositorio.ConhecimentoDeTransporteEletronico repositorioCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

            (int codigoCTE, int proximoNumeroCTE) = repositorioCTe.BuscarNumeroReutilizavel(cte.Empresa.Codigo, cte.Serie.Codigo, cte.TipoAmbiente, cte.ModeloDocumentoFiscal.Codigo);

            if (codigoCTE > 0)
                repositorioCTe.AtulizarSituacaoReutilizacao(Dominio.Enumeradores.ReutilizaNumeracao.Reutilizado, codigoCTE);
            else
                proximoNumeroCTE = repositorioCTe.BuscarUltimoNumero(cte.Empresa.Codigo, cte.Serie.Codigo, cte.TipoAmbiente, cte.ModeloDocumentoFiscal.Codigo) + 1;

            return proximoNumeroCTE;
        }
    }

    #endregion Métodos Públicos

    #region Métodos Privados

    #endregion
}