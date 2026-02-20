using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Cargas.ProgramacaoCarga
{
    public class SugestaoProgramacaoCargaHistoricoCarga : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCargaHistoricoCarga>
    {
        #region Construtores

        public SugestaoProgramacaoCargaHistoricoCarga(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Públicos

        public IList<Dominio.ObjetosDeValor.Embarcador.Carga.ProgramacaoCarga.SugestaoProgramacaoCargaHistoricoCarga> BuscarPorSugestaoProgramacaoCarga(int codigoSugestaoProgramacaoCarga)
        {
            string sql = $@"
                select Carga.CAR_CODIGO_CARGA_EMBARCADOR as CodigoCargaEmbarcador,
                       Carga.CAR_DATA_CRIACAO as DataCriacaoCarga,
                       Carga.CAR_DATA_FINALIZACAO_EMISSAO as DataFinalizacaoEmissao,
                       DadosSumarizados.CDS_PESO_TOTAL as Peso,
                       DadosSumarizados.CDS_DESTINOS as CidadesDestino,
                       DadosSumarizados.CDS_UF_DESTINOS as EstadosDestino,
                       DadosSumarizados.CDS_REGIAO_DESTINO as RegioesDestino,
                       Filial.FIL_DESCRICAO as Filial,
                       ModeloVeicularCarga.MVC_DESCRICAO as ModeloVeicularCarga,
                       TipoCarga.TCG_DESCRICAO as TipoCarga,
                       TipoOperacao.TOP_DESCRICAO as TipoOperacao,
                       Empresa.EMP_RAZAO as Transportador,
                       Tracao.VEI_PLACA as PlacaTracao,
                       substring((
                           select distinct ', ' + Reboque.VEI_PLACA
                             from T_CARGA_VEICULOS_VINCULADOS VeiculoVinculado
                             join T_VEICULO Reboque on Reboque.VEI_CODIGO = VeiculoVinculado.VEI_CODIGO
                            where VeiculoVinculado.CAR_CODIGO = Carga.CAR_CODIGO
                              for xml path('')
                       ), 3, 1000) as PlacaReboques
                  from T_SUGESTAO_PROGRAMACAO_CARGA_HISTORICO_CARGA Historico
                  join T_CARGA Carga on Carga.CAR_CODIGO = Historico.CAR_CODIGO
                  left join T_CARGA_DADOS_SUMARIZADOS DadosSumarizados on DadosSumarizados.CDS_CODIGO = Carga.CDS_CODIGO
                  left join T_FILIAL Filial on Filial.FIL_CODIGO = Carga.FIL_CODIGO
                  left join T_EMPRESA Empresa on Empresa.EMP_CODIGO = Carga.EMP_CODIGO
                  left join T_MODELO_VEICULAR_CARGA ModeloVeicularCarga on ModeloVeicularCarga.MVC_CODIGO = Carga.MVC_CODIGO
                  left join T_TIPO_DE_CARGA TipoCarga on TipoCarga.TCG_CODIGO = Carga.TCG_CODIGO
                  left join T_TIPO_OPERACAO TipoOperacao on TipoOperacao.TOP_CODIGO = Carga.TOP_CODIGO
                  left join T_VEICULO Tracao on Tracao.VEI_CODIGO = Carga.CAR_VEICULO
                 where Historico.SPC_CODIGO = {codigoSugestaoProgramacaoCarga}
            ";

            var consultaHistoricoCarga = this.SessionNHiBernate.CreateSQLQuery(sql);

            consultaHistoricoCarga.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Carga.ProgramacaoCarga.SugestaoProgramacaoCargaHistoricoCarga)));

            return consultaHistoricoCarga.SetTimeout(600).List<Dominio.ObjetosDeValor.Embarcador.Carga.ProgramacaoCarga.SugestaoProgramacaoCargaHistoricoCarga>();
        }

        #endregion Métodos Públicos
    }
}
