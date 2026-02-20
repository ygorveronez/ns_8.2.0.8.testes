using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Contabeis
{
    public class ImpostoValorAgregado : RepositorioBase<Dominio.Entidades.Embarcador.Contabeis.ImpostoValorAgregado>
    {
        #region Construtores

        public ImpostoValorAgregado(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Contabeis.ImpostoValorAgregado> Consultar(Dominio.ObjetosDeValor.Embarcador.Contabil.FiltroPesquisaImpostoValorAgregado filtrosPesquisa)
        {
            var consultaImpostoValorAgregado = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Contabeis.ImpostoValorAgregado>();

            if (filtrosPesquisa.CodigoDesconsiderar > 0)
                consultaImpostoValorAgregado = consultaImpostoValorAgregado.Where(obj => obj.Codigo != filtrosPesquisa.CodigoDesconsiderar);

            if (!string.IsNullOrEmpty(filtrosPesquisa.CodigoIVA))
                consultaImpostoValorAgregado = consultaImpostoValorAgregado.Where(obj => obj.CodigoIVA.Contains(filtrosPesquisa.CodigoIVA));

            if (filtrosPesquisa.ImpostoMaiorQueZero.HasValue)
                consultaImpostoValorAgregado = consultaImpostoValorAgregado.Where(obj => obj.ImpostoMaiorQueZero == filtrosPesquisa.ImpostoMaiorQueZero);

            if (filtrosPesquisa.DestinatarioExterior.HasValue)
                consultaImpostoValorAgregado = consultaImpostoValorAgregado.Where(obj => obj.DestinatarioExterior == filtrosPesquisa.DestinatarioExterior);

            if (filtrosPesquisa.PermitirInformarManualmente.HasValue)
                consultaImpostoValorAgregado = consultaImpostoValorAgregado.Where(obj => obj.PermitirInformarManualmente == filtrosPesquisa.PermitirInformarManualmente);

            if (filtrosPesquisa.CodigoModeloDocumentoFiscal > 0)
                consultaImpostoValorAgregado = consultaImpostoValorAgregado.Where(obj => obj.ModeloDocumentoFiscal.Codigo == filtrosPesquisa.CodigoModeloDocumentoFiscal);

            if (filtrosPesquisa.UsoMaterial.HasValue)
                consultaImpostoValorAgregado = consultaImpostoValorAgregado.Where(obj => obj.UsoMaterial == filtrosPesquisa.UsoMaterial);

            return consultaImpostoValorAgregado;
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Contabeis.ImpostoValorAgregado BuscarPorCodigo(int codigo)
        {
            var consultaImpostoValorAgregado = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Contabeis.ImpostoValorAgregado>()
                .Where(obj => obj.Codigo == codigo);

            return consultaImpostoValorAgregado.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Contabeis.ImpostoValorAgregado BuscarPrimeiro(int codigoModeloDocumentoFiscal, bool impostoMaiorQueZero, bool destinatarioExterior, UsoMaterial usoMaterial)
        {
            var consultaImpostoValorAgregado = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Contabeis.ImpostoValorAgregado>()
                .Where(obj =>
                    obj.ImpostoMaiorQueZero == impostoMaiorQueZero &&
                    obj.DestinatarioExterior == destinatarioExterior &&
                    obj.ModeloDocumentoFiscal.Codigo == codigoModeloDocumentoFiscal &&
                    obj.UsoMaterial == usoMaterial
                );

            return consultaImpostoValorAgregado.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Contabeis.ImpostoValorAgregado> Consultar(Dominio.ObjetosDeValor.Embarcador.Contabil.FiltroPesquisaImpostoValorAgregado filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaImpostoValorAgregado = Consultar(filtrosPesquisa);

            return ObterLista(consultaImpostoValorAgregado, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Contabil.FiltroPesquisaImpostoValorAgregado filtrosPesquisa)
        {
            var consultaImpostoValorAgregado = Consultar(filtrosPesquisa);

            return consultaImpostoValorAgregado.Count();
        }

        public bool PossuiRegistroIVA()
        {
            var consultaImpostoValorAgregado = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Contabeis.ImpostoValorAgregado>();

            return consultaImpostoValorAgregado.Any();
        }

        #endregion Métodos Públicos
    }
}
