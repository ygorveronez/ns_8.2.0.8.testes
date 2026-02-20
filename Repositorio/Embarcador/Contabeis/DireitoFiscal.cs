using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Contabeis
{
    public class DireitoFiscal : RepositorioBase<Dominio.Entidades.Embarcador.Contabeis.DireitoFiscal>
    {
        #region Construtores

        public DireitoFiscal(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Contabeis.DireitoFiscal> Consultar(Dominio.ObjetosDeValor.Embarcador.Contabil.FiltroPesquisaDireitoFiscal filtrosPesquisa)
        {
            var consultaDireitoFiscal = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Contabeis.DireitoFiscal>();

            if (!string.IsNullOrEmpty(filtrosPesquisa.Descricao))
                consultaDireitoFiscal = consultaDireitoFiscal.Where(obj => obj.Descricao.Contains(filtrosPesquisa.Descricao));

            if (filtrosPesquisa.CodigoIVA > 0)
                consultaDireitoFiscal = consultaDireitoFiscal.Where(obj => obj.ImpostoValorAgregado.Codigo == filtrosPesquisa.CodigoIVA);

            if (filtrosPesquisa.Situacao != SituacaoAtivoPesquisa.Todos)
            {
                if (filtrosPesquisa.Situacao == SituacaoAtivoPesquisa.Ativo)
                    consultaDireitoFiscal = consultaDireitoFiscal.Where(obj => obj.Ativo);
                else
                    consultaDireitoFiscal = consultaDireitoFiscal.Where(obj => !obj.Ativo);
            }

            return consultaDireitoFiscal;
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Contabeis.DireitoFiscal> Consultar(Dominio.ObjetosDeValor.Embarcador.Contabil.FiltroPesquisaDireitoFiscal filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaDireitoFiscal = Consultar(filtrosPesquisa);

            return ObterLista(consultaDireitoFiscal, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Contabil.FiltroPesquisaDireitoFiscal filtrosPesquisa)
        {
            var consultaDireitoFiscal = Consultar(filtrosPesquisa);

            return consultaDireitoFiscal.Count();
        }

        public bool ExisteDuplicado(Dominio.Entidades.Embarcador.Contabeis.DireitoFiscal direitoFiscalFront)
        {
            var consultaDireitoFiscal = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Contabeis.DireitoFiscal>();

            consultaDireitoFiscal = consultaDireitoFiscal.Where(obj => obj.Codigo != direitoFiscalFront.Codigo);
            if (direitoFiscalFront.ImpostoValorAgregado != null)
                consultaDireitoFiscal = consultaDireitoFiscal.Where(obj => obj.ImpostoValorAgregado.Codigo == direitoFiscalFront.ImpostoValorAgregado.Codigo);
            consultaDireitoFiscal = consultaDireitoFiscal.Where(obj => obj.Descricao == direitoFiscalFront.Descricao);
            consultaDireitoFiscal = consultaDireitoFiscal.Where(obj => obj.ICMS_ISS == direitoFiscalFront.ICMS_ISS);
            consultaDireitoFiscal = consultaDireitoFiscal.Where(obj => obj.PIS == direitoFiscalFront.PIS);
            consultaDireitoFiscal = consultaDireitoFiscal.Where(obj => obj.COFINS == direitoFiscalFront.COFINS);
            consultaDireitoFiscal = consultaDireitoFiscal.Where(obj => obj.Ativo == direitoFiscalFront.Ativo);

            return consultaDireitoFiscal.Any();
        }

        public Dominio.Entidades.Embarcador.Contabeis.DireitoFiscal BuscarPorIVA(int codigoIVA)
        {
            var consultaDireitoFiscal = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Contabeis.DireitoFiscal>();

            consultaDireitoFiscal = consultaDireitoFiscal.Where(obj => obj.ImpostoValorAgregado.Codigo == codigoIVA);

            return consultaDireitoFiscal.FirstOrDefault();
        }

        #endregion Métodos Públicos
    }
}
