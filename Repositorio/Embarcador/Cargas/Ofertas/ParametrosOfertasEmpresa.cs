using NHibernate.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas.Ofertas
{
    public class ParametrosOfertasEmpresa : RepositorioRelacionamentoParametrosOfertas<Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertasEmpresa>, Dominio.Interfaces.Embarcador.Cargas.Ofertas.IRepositorioRelacionamentoParametrosOfertas
    {
        #region Construtores 

        public ParametrosOfertasEmpresa(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion

        public override Task<List<Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertasEmpresa>> BuscarAsync(Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.FiltroPesquisaRelacionamentosParametrosOfertas filtro, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consulta = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Ofertas.ParametrosOfertasEmpresa>();

            consulta = AplicarFiltros(consulta, filtro);
            consulta.Fetch(to => to.ParametrosOfertas);
            consulta.Fetch(to => to.Empresa);

            return ObterListaAsync(consulta, parametrosConsulta);
        }

        public Task<IList<ParametrosOfertasEmpresaCodigosDescricao>> BuscarCodigosDescricaoAsync(int codigoParametrosOfertas)
        {
            var consulta = UnitOfWork.Sessao.CreateSQLQuery($"""
                select 
                    tpoe.POE_CODIGO as CodigoRelacionamento, 
                    tpoe.EMP_CODIGO as Codigo,
                    te.EMP_RAZAO as RazaoSocial,
                    tl.LOC_DESCRICAO as Localidade,
                    tl.UF_SIGLA as Estado,
                    te.EMP_CNPJ as CNPJ,
                    te.EMP_CODIGO as CodigoEntidadeFraca
                from T_PARAMETROS_OFERTAS_EMPRESA tpoe 
                inner join T_EMPRESA te on tpoe.EMP_CODIGO = te.EMP_CODIGO
                inner join T_LOCALIDADES tl on te.LOC_CODIGO = tl.LOC_CODIGO
                where tpoe.POF_CODIGO = {codigoParametrosOfertas};
            """);

            consulta.SetResultTransformer(NHibernate.Transform.Transformers.AliasToBean<ParametrosOfertasEmpresaCodigosDescricao>());

            return consulta.ListAsync<ParametrosOfertasEmpresaCodigosDescricao>();
        }

    }


    public class ParametrosOfertasEmpresaCodigosDescricao : Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.RelacionamentoParametrosOfertasCodigosDescricao
    {
        public string RazaoSocial { get; set; }
        public string Localidade { get; set; }
        public string Estado { get; set; }

        public override string Descricao
        {
            get { return $"{RazaoSocial} ({Localidade} - {Estado})"; }
        }

        public Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.RelacionamentoParametrosOfertasCodigosDescricao ComoSuperclasse()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas.RelacionamentoParametrosOfertasCodigosDescricao()
            {
                Codigo = Codigo,
                CodigoRelacionamento = CodigoRelacionamento,
                Descricao = Descricao,
                CNPJ = CNPJ,
                CodigoEntidadeFraca = CodigoEntidadeFraca,
            };
        }
    }
}
