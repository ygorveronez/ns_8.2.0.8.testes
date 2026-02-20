using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Logistica
{
    public class PosicaoAlertaSensor : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.PosicaoAlertaSensor>
    {
        public PosicaoAlertaSensor(UnitOfWork unitOfWork) : base(unitOfWork) { }


        public IList<Dominio.ObjetosDeValor.Embarcador.Logistica.PosicaoAlertaSensor> BuscarListaUltimasPosicoesAlertaSensorPorVeiculos()
        {
            string sql = $@"
                 select
                    max(PAS_CODIGO) ID,
                    max(POS_CODIGO) IDPosicao,
	                Max(PAS_DATA_CADASTRO) AS DataCadastro,
	                Max(PAS_DATA_VEICULO) AS DataVeiculo,
	                VEI_CODIGO IDVeiculo,
	                PAS_TIPO_SENSOR TipoSensor,
	                CAST(MAX(CAST(PAS_VALOR_SENSOR as INT)) AS BIT) ValorSensor
                from
	                T_POSICAO_ALERTA_SENSOR
                group by 
                    PAS_TIPO_SENSOR, VEI_CODIGO
                order by Max(PAS_DATA_CADASTRO) DESC";

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);
            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Logistica.PosicaoAlertaSensor)));
            query.SetTimeout(600);
            return query.List<Dominio.ObjetosDeValor.Embarcador.Logistica.PosicaoAlertaSensor>();
        }


        public Dominio.ObjetosDeValor.Embarcador.Logistica.PosicaoAlertaSensor BuscarListaPosicoesPorVeiculo(int codigoVeiculo, DateTime dataInicial, DateTime dataFinal, bool valorSensor)
        {
            string sql = $@"
                 select
                    top 1
                    PAS_CODIGO ID,
                    POS_CODIGO IDPosicao,
	                PAS_DATA_CADASTRO AS DataCadastro,
	                PAS_DATA_VEICULO AS DataVeiculo,
	                VEI_CODIGO IDVeiculo,
	                PAS_TIPO_SENSOR TipoSensor,
	                PAS_VALOR_SENSOR ValorSensor
                from
	                T_POSICAO_ALERTA_SENSOR
                where
	                VEI_CODIGO in ({codigoVeiculo})
	                and PAS_DATA_VEICULO >= :data_inicio
	                and PAS_DATA_VEICULO <= :data_fim
                    and PAS_VALOR_SENSOR = :valor_sensor
                order by PAS_DATA_CADASTRO DESC";

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);
            query.SetParameter("data_inicio", dataInicial);
            query.SetParameter("data_fim", dataFinal);
            query.SetParameter("valor_sensor", valorSensor);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Logistica.PosicaoAlertaSensor)));
            query.SetTimeout(600);
            return query.List<Dominio.ObjetosDeValor.Embarcador.Logistica.PosicaoAlertaSensor>().FirstOrDefault();
        }
    }
}
