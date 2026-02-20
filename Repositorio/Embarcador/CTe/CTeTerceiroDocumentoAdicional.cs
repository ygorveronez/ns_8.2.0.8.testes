using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.CTe
{
    public class CTeTerceiroDocumentoAdicional : RepositorioBase<Dominio.Entidades.Embarcador.CTe.CTeTerceiroDocumentoAdicional>
    {
        public CTeTerceiroDocumentoAdicional(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.CTe.CTeTerceiroDocumentoAdicional BuscarPorCTeTerceiroEChaveAcesso(int codigoCTeTerceiro, string chaveAcessoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeTerceiroDocumentoAdicional>();
            var result = from obj in query where obj.CTeTerceiro.Codigo == codigoCTeTerceiro && obj.Chave == chaveAcessoCTe select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroDocumentoAdicional> BuscarPorCTeTerceiro(List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> ctesTerceiros)
        {
            IQueryable<Dominio.Entidades.Embarcador.CTe.CTeTerceiroDocumentoAdicional> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeTerceiroDocumentoAdicional>();

            int quantidadeRegistrosConsultarPorVez = 1000;
            int quantidadeConsultas = ctesTerceiros.Count / quantidadeRegistrosConsultarPorVez;

            List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroDocumentoAdicional> registrosRetornar = new List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroDocumentoAdicional>();

            for (int i = 0; i <= quantidadeConsultas; i++)
                registrosRetornar.AddRange(query.Where(o => ctesTerceiros.Skip(i * quantidadeRegistrosConsultarPorVez).Take(quantidadeRegistrosConsultarPorVez).Contains(o.CTeTerceiro))
                                                .ToList());

            return registrosRetornar;
        }

        public void RemoverPorCTeTerceiro(int codigoCTeTerceiro)
        {
            NHibernate.ISQLQuery sqlQuery = SessionNHiBernate.CreateSQLQuery("DELETE FROM T_CTE_TERCEIRO_DOCUMENTO_ADICIONAL_NFE WHERE CPS_CODIGO = :codigoCTeTerceiro;DELETE FROM T_CTE_TERCEIRO_DOCUMENTO_ADICIONAL WHERE CPS_CODIGO = :codigoCTeTerceiro");

            sqlQuery.SetInt32("codigoCTeTerceiro", codigoCTeTerceiro);

            sqlQuery.SetTimeout(180).ExecuteUpdate();
        }

        public decimal BuscarTotalValorMercadoria(int codigoCTeTerceiro)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeTerceiroDocumentoAdicional>();

            var result = from obj in query where obj.CTeTerceiro.Codigo == codigoCTeTerceiro select obj.ValorTotalMercadoria;

            if (result.Count() > 0)
                return result.Sum();
            else
                return 0;
        }

        public decimal BuscarTotalQuantidade(int codigoCTeTerceiro)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeTerceiroDocumentoAdicional>();

            var result = from obj in query where obj.CTeTerceiro.Codigo == codigoCTeTerceiro select obj.Quantidade;

            if (result.Count() > 0)
                return result.Sum();
            else
                return 0;
        }

        public int BuscarTotalDocumentoAdicionalPorTerceiro(int codigoCTeTerceiro)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeTerceiroDocumentoAdicional>();

            var result = from obj in query where obj.CTeTerceiro.Codigo == codigoCTeTerceiro select obj.Codigo;

            return result.Count();
        }
    }
}
