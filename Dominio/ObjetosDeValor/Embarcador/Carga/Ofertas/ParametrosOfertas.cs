using System;
using System.Linq;

namespace Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas;

public class ParametrosOfertas
{
    public int Codigo { get; set; }
    public string Descricao { get; set; }
    public string CodigoIntegracao { get; set; }
    public bool Ativo { get; set; }
    public RelacionamentoParametrosOfertasCodigosDescricao GrupoMotoristas { get; set; }
    public int[] CodigosEmpresas { get; set; }
    public int[] CodigosFiliais { get; set; }
    public int[] CodigosTiposCargas { get; set; }
    public int[] CodigosTiposOperacoes { get; set; }

    public ParametrosOfertas(int? codigoEmpresa, int? codigoFilial, int? codigoTipoCarga, int? codigoTipoOperacao)
    {
        Ativo = true;
        CodigosEmpresas = codigoEmpresa.HasValue ? new[] { codigoEmpresa.Value } : Array.Empty<int>();
        CodigosFiliais = codigoFilial.HasValue ? new[] { codigoFilial.Value } : Array.Empty<int>();
        CodigosTiposCargas = codigoTipoCarga.HasValue ? new[] { codigoTipoCarga.Value } : Array.Empty<int>();
        CodigosTiposOperacoes = codigoTipoOperacao.HasValue ? new[] { codigoTipoOperacao.Value } : Array.Empty<int>();
    }

    public ParametrosOfertas(ParametrosOfertasResultadoConsulta resultadoConsulta)
    {
        Codigo = resultadoConsulta.Codigo;
        Descricao = resultadoConsulta.Descricao;
        CodigoIntegracao = resultadoConsulta.CodigoIntegracao;
        Ativo = resultadoConsulta.Ativo;

        CodigosEmpresas = string.IsNullOrEmpty(resultadoConsulta.CodigosEmpresasString) ?
            Array.Empty<int>() :
            resultadoConsulta.CodigosEmpresasString.Split(',').Select(int.Parse).Distinct().ToArray();

        CodigosFiliais = string.IsNullOrEmpty(resultadoConsulta.CodigosFiliaisString) ?
            Array.Empty<int>() :
            resultadoConsulta.CodigosFiliaisString.Split(',').Select(int.Parse).Distinct().ToArray();

        CodigosTiposCargas = string.IsNullOrEmpty(resultadoConsulta.CodigosTiposCargasString) ?
            Array.Empty<int>() :
            resultadoConsulta.CodigosTiposCargasString.Split(',').Select(int.Parse).Distinct().ToArray();

        CodigosTiposOperacoes = string.IsNullOrEmpty(resultadoConsulta.CodigosTiposOperacoesString) ?
            Array.Empty<int>() :
            resultadoConsulta.CodigosTiposOperacoesString.Split(',').Select(int.Parse).Distinct().ToArray();
    }

}
