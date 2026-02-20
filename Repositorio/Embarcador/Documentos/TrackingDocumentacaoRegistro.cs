using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Documentos
{
    public class TrackingDocumentacaoRegistro : RepositorioBase<Dominio.Entidades.Embarcador.Documentos.TrackingDocumentacaoRegistro>
    {
        public TrackingDocumentacaoRegistro(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Documentos.TrackingDocumentacaoRegistro BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.TrackingDocumentacaoRegistro>();

            query = query.Where(o => o.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Documentos.TrackingDocumentacaoRegistro> BuscarPorTraking(long codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.TrackingDocumentacaoRegistro>();

            query = query.Where(o => o.TrackingDocumentacao.Codigo == codigo);

            return query.ToList();
        }

        public bool ContemRegistroPorTraking(long codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.TrackingDocumentacaoRegistro>();

            query = query.Where(o => o.TrackingDocumentacao.Codigo == codigo);

            return query.Any();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Documentos.RegistrosTrackingDocumentacao> BuscarRegistrosTrackingDocumentacao(bool retorenarTodosRegistros, int codigoTracking, int codigoPedidoViagemDirecao, int codigoPortoOrigem, int codigoPortoDestino, DateTime dataGeracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoTrackingDocumentacao situacaoTrackingDocumentacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTrackingDocumentacao tipoTrackingDocumentacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIMO tipoIMO, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string orderBy = string.Empty,
                select = string.Empty;

            select = @"SELECT DISTINCT
                    ISNULL(T.TDR_CODIGO, 0) Codigo,
                    CP.TBF_TIPO_PROPOSTA_MULTIMODAL TipoMultimodal,
                    CASE
	                    WHEN CP.TBF_TIPO_PROPOSTA_MULTIMODAL = 3 THEN 'Feeder'
	                    ELSE 'Cabotagem'	
                    END Tipo,
                    N.PVN_CODIGO CodigoVVD,
                    N.PVN_DESCRICAO VVD,
                    PO.POT_CODIGO CodigoPortoOrigem,
                    PO.POT_DESCRICAO PortoOrigem,
                    PD.POT_CODIGO CodigoPortoDestino,
                    PD.POT_DESCRICAO PortoDestino,
                    P.PED_POSSUI_CARGA_PERIGOSA CargaPerigosa,
                    CASE
	                    WHEN P.PED_POSSUI_CARGA_PERIGOSA = 1 THEN 'SIM'
	                    ELSE 'NÃO'
                    END IMO,
                    '" + dataGeracao.ToString("dd/MM/yyyy HH:mm") + @"' DataGeracao,
                    F.FUN_CODIGO CodigoOperadorCarga,
                    F.FUN_NOME OperadorCarga ";

            var sqlQuery = @" FROM T_CARGA C
                    JOIN T_CARGA_PEDIDO CP ON CP.CAR_CODIGO = C.CAR_CODIGO
                    JOIN T_PEDIDO P ON P.PED_CODIGO = CP.PED_CODIGO
                    LEFT OUTER JOIN T_PEDIDO_VIAGEM_NAVIO N ON N.PVN_CODIGO = P.PVN_CODIGO
                    LEFT OUTER JOIN T_PORTO PO ON PO.POT_CODIGO = P.POT_CODIGO_ORIGEM
                    LEFT OUTER JOIN T_PORTO PD ON PD.POT_CODIGO = P.POT_CODIGO_DESTINO
                    LEFT OUTER JOIN T_FUNCIONARIO F ON F.FUN_CODIGO = C.CAR_OPERADOR
                    LEFT OUTER JOIN T_TRACKING_DOCUMENTACAO_REGISTRO T ON T.TDO_CODIGO = " + codigoTracking + @" AND T.POT_CODIGO_ORIGEM = P.POT_CODIGO_ORIGEM AND T.PVN_CODIGO = P.PVN_CODIGO AND T.FUN_CODIGO = C.CAR_OPERADOR
                    WHERE C.CAR_SITUACAO IN (8, 9, 10, 11, 15, 17, 13, 18)";

            sqlQuery += " AND P.PVN_CODIGO = " + codigoPedidoViagemDirecao;
            sqlQuery += " AND P.POT_CODIGO_ORIGEM = " + codigoPortoOrigem;
            if (codigoPortoDestino > 0)
                sqlQuery += " AND P.POT_CODIGO_DESTINO = " + codigoPortoDestino;

            if (situacaoTrackingDocumentacao == SituacaoTrackingDocumentacao.SemRegistros && codigoTracking == 0)
                sqlQuery += " AND C.CAR_CODIGO NOT IN (SELECT CC.CAR_CODIGO FROM T_TRACKING_DOCUMENTACAO_REGISTRO_CARGA CC)";

            if (tipoTrackingDocumentacao == TipoTrackingDocumentacao.Feeder)
                sqlQuery += " AND CP.TBF_TIPO_PROPOSTA_MULTIMODAL = 3";
            else
                sqlQuery += " AND CP.TBF_TIPO_PROPOSTA_MULTIMODAL <> 3";

            if (tipoIMO == TipoIMO.ApenasIMO)
                sqlQuery += " AND P.PED_POSSUI_CARGA_PERIGOSA = 1";

            sqlQuery = select + sqlQuery;

            orderBy += (orderBy.Length > 0 ? ", " : string.Empty) + propOrdena + " " + dirOrdena;
            sqlQuery += ((retorenarTodosRegistros) ? string.Empty : (orderBy.Length > 0 ? " ORDER BY " + orderBy : " ORDER BY 1 ASC ")) +
                   (retorenarTodosRegistros || (inicio <= 0 && limite <= 0) ? "" : " OFFSET " + inicio.ToString() + " ROWS FETCH NEXT " + limite.ToString() + " ROWS ONLY;");

            var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Documentos.RegistrosTrackingDocumentacao)));

            return query.List<Dominio.ObjetosDeValor.Embarcador.Documentos.RegistrosTrackingDocumentacao>();
        }

        public int ContarBuscarRegistrosTrackingDocumentacao(bool count, int codigoTracking, int codigoPedidoViagemDirecao, int codigoPortoOrigem, int codigoPortoDestino, DateTime dataGeracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoTrackingDocumentacao situacaoTrackingDocumentacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTrackingDocumentacao tipoTrackingDocumentacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIMO tipoIMO, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string orderBy = string.Empty,
                select = string.Empty;

            if (count)
                select = "SELECT DISTINCT(COUNT(0) OVER ()) ";

            var sqlQuery = @" FROM T_CARGA C
                    JOIN T_CARGA_PEDIDO CP ON CP.CAR_CODIGO = C.CAR_CODIGO
                    JOIN T_PEDIDO P ON P.PED_CODIGO = CP.PED_CODIGO
                    LEFT OUTER JOIN T_PEDIDO_VIAGEM_NAVIO N ON N.PVN_CODIGO = P.PVN_CODIGO
                    LEFT OUTER JOIN T_PORTO PO ON PO.POT_CODIGO = P.POT_CODIGO_ORIGEM
                    LEFT OUTER JOIN T_PORTO PD ON PD.POT_CODIGO = P.POT_CODIGO_DESTINO
                    LEFT OUTER JOIN T_FUNCIONARIO F ON F.FUN_CODIGO = C.CAR_OPERADOR
                    LEFT OUTER JOIN T_TRACKING_DOCUMENTACAO_REGISTRO T ON T.TDO_CODIGO = " + codigoTracking + @" AND T.POT_CODIGO_ORIGEM = P.POT_CODIGO_ORIGEM AND T.PVN_CODIGO = P.PVN_CODIGO AND T.FUN_CODIGO = C.CAR_OPERADOR
                    WHERE C.CAR_SITUACAO IN (8, 9, 10, 11, 15, 17, 13, 18)";

            sqlQuery += " AND P.PVN_CODIGO = " + codigoPedidoViagemDirecao;
            sqlQuery += " AND P.POT_CODIGO_ORIGEM = " + codigoPortoOrigem;
            if (codigoPortoDestino > 0)
                sqlQuery += " AND P.POT_CODIGO_DESTINO = " + codigoPortoDestino;

            if (situacaoTrackingDocumentacao == SituacaoTrackingDocumentacao.SemRegistros && codigoTracking == 0)
                sqlQuery += " AND C.CAR_CODIGO NOT IN (SELECT CC.CAR_CODIGO FROM T_TRACKING_DOCUMENTACAO_REGISTRO_CARGA CC)";

            if (tipoTrackingDocumentacao == TipoTrackingDocumentacao.Feeder)
                sqlQuery += " AND CP.TBF_TIPO_PROPOSTA_MULTIMODAL = 3";
            else
                sqlQuery += " AND CP.TBF_TIPO_PROPOSTA_MULTIMODAL <> 3";

            if (tipoIMO == TipoIMO.ApenasIMO)
                sqlQuery += " AND P.PED_POSSUI_CARGA_PERIGOSA = 1";

            sqlQuery = select + sqlQuery;

            orderBy += (orderBy.Length > 0 ? ", " : string.Empty) + propOrdena + " " + dirOrdena;
            sqlQuery += ((count) ? string.Empty : (orderBy.Length > 0 ? " ORDER BY " + orderBy : " ORDER BY 1 ASC ")) +
                   (count || (inicio <= 0 && limite <= 0) ? "" : " OFFSET " + inicio.ToString() + " ROWS FETCH NEXT " + limite.ToString() + " ROWS ONLY;");

            var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);

            //query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Documentos.RegistrosTrackingDocumentacao)));

            return query.UniqueResult<int>();
        }
    }
}

