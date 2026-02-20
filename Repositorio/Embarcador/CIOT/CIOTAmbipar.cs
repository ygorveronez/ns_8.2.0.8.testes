using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.CIOT
{
    public class CIOTAmbipar : RepositorioBase<Dominio.Entidades.Embarcador.CIOT.CIOTAmbipar>
    {
        public CIOTAmbipar(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.CIOT.CIOTAmbipar BuscarPorCodigo(int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.CIOT.CIOTAmbipar> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CIOT.CIOTAmbipar>();

            query = query.Where(obj => obj.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.CIOT.CIOTAmbipar BuscarPorConfiguracaoCIOT(int codigoConfiguracao)
        {
            IQueryable<Dominio.Entidades.Embarcador.CIOT.CIOTAmbipar> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CIOT.CIOTAmbipar>();

            query = query.Where(obj => obj.ConfiguracaoCIOT.Codigo == codigoConfiguracao);

            return query.FirstOrDefault();
        }
        public IList<Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.Invoice> BuscarNFsPorCarga(int codigoCarga)
        {
            var sql = $@"select NF_SERIE series,CAST(NF_NUMERO AS VARCHAR(25)) number, CAST(NF_TIPO_DOCUMENTO AS VARCHAR(10))  type from T_CARGA C (NOLOCK) 
                        INNER JOIN T_CARGA_PEDIDO (NOLOCK) CP ON CP.CAR_CODIGO = C.CAR_CODIGO
                        INNER JOIN T_PEDIDO_XML_NOTA_FISCAL (NOLOCK) NFP ON NFP.CPE_CODIGO = CP.CPE_CODIGO
                        INNER JOIN T_XML_NOTA_FISCAL (NOLOCK) NF ON NF.NFX_CODIGO = NFP.NFX_CODIGO
                        WHERE C.CAR_CODIGO = {codigoCarga}";
            var consulta = this.SessionNHiBernate.CreateSQLQuery(sql);
            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.Invoice)));
            var NFs = consulta.SetTimeout(7000).List<Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.Invoice>();

            foreach (var nf in NFs)
                nf.type = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoHelper.ObterDescricao(
                    (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento)Enum.Parse(typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento), nf.type)
                    );
            return NFs;
        }
    }
}