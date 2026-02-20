using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.GestaoPatio
{
    public class MultaAtrasoRetirada : RepositorioBase<Dominio.Entidades.Embarcador.GestaoPatio.MultaAtrasoRetirada>
    {
        #region Construtores

        public MultaAtrasoRetirada(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Públicos

        public List<int> BuscarCodigosParaGeracaoOcorrencias()
        {
            List<SituacaoCarga> situacoesCargaNaoFaturada = SituacaoCargaHelper.ObterSituacoesCargaNaoFaturada();

            var consultaMultaAtrasoRetirada = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.MultaAtrasoRetirada>()
                .Where(multa => multa.GerarOcorrencia == true && !situacoesCargaNaoFaturada.Contains(multa.Carga.SituacaoCarga));

            return consultaMultaAtrasoRetirada.Select(multa => multa.Codigo).ToList();
        }

        public int BuscarTotalCargasRetiradasNoPeriodo(int codigoCargaDesconsiderar, int codigoTransportador, int codigoRegrasMultaAtrasoRetirada, DateTime dataLiberacaoCarga)
        {
            string sql = $@"
                select count(*)
                  from T_MULTA_ATRASO_RETIRADA Multa
                  join T_CARGA Carga on Carga.CAR_CODIGO = Multa.CAR_CODIGO
                 where Multa.RMA_CODIGO = {codigoRegrasMultaAtrasoRetirada}
                   and Multa.EMP_CODIGO = {codigoTransportador}
                   and Multa.MAT_RETIRADA_NO_PERIODO = 1
                   and cast(Multa.MAT_DATA_LIBERACAO_CARGA as Date) = '{dataLiberacaoCarga.ToString("yyyyMMdd")}'
                   and '{dataLiberacaoCarga.ToString("HH:mm:ss")}' between Multa.MAT_HORA_INICIO_PERIODO and Multa.MAT_HORA_TERMINO_PERIODO
                   and Carga.CAR_CODIGO <> {codigoCargaDesconsiderar}
                   and Carga.CAR_SITUACAO <> {(int)SituacaoCarga.Cancelada}
                   and Carga.CAR_SITUACAO <> {(int)SituacaoCarga.Anulada}
            ";

            var consultaMultaAtrasoRetirada = this.SessionNHiBernate.CreateSQLQuery(sql);

            return consultaMultaAtrasoRetirada.SetTimeout(600).UniqueResult<int>();
        }

        #endregion Métodos Públicos
    }
}
