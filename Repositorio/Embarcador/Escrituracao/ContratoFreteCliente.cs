using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Escrituracao
{
    public class ContratoFreteCliente : RepositorioBase<Dominio.Entidades.Embarcador.Escrituracao.ContratoFreteCliente>
    {

        public ContratoFreteCliente(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        #region Contratos
        public List<Dominio.Entidades.Embarcador.Escrituracao.ContratoFreteCliente> Consultar(Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaContratoFreteCliente filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            IQueryable<Dominio.Entidades.Embarcador.Escrituracao.ContratoFreteCliente> consulta = Consultar(filtrosPesquisa);

            return ObterLista(consulta, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaContratoFreteCliente filtrosPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Escrituracao.ContratoFreteCliente> consulta = Consultar(filtrosPesquisa);

            return consulta.Count();
        }

        public bool ValidarInformacoesCadastroContrato(Dominio.Entidades.Embarcador.Escrituracao.ContratoFreteCliente contratoFreteCliente)
        {
            IQueryable<Dominio.Entidades.Embarcador.Escrituracao.ContratoFreteCliente> consulta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.ContratoFreteCliente>();
            consulta = from obj in consulta select obj;

            return consulta.Any(x => x.NumeroContrato == contratoFreteCliente.NumeroContrato);
        }

        public Dominio.Entidades.Embarcador.Escrituracao.ContratoFreteCliente BuscarPorNumeroContrato(string numeroContrato)
        {
            IQueryable<Dominio.Entidades.Embarcador.Escrituracao.ContratoFreteCliente> consulta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.ContratoFreteCliente>();
            consulta = consulta.Where(x => x.NumeroContrato == numeroContrato);

            return consulta.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Escrituracao.ContratoFreteCliente BuscarContratoPorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> consulta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>().Where(obj => obj.Carga.Codigo == codigoCarga);

            return consulta.Select(obj => obj.Pedido.ContratoFreteCliente).FirstOrDefault();
        }
        #endregion

        #region Saldo Contratos

        public List<Dominio.Entidades.Embarcador.Escrituracao.ContratoFreteCliente> ConsultarContratos(Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaSaldoContratoFreteCliente filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            IQueryable<Dominio.Entidades.Embarcador.Escrituracao.ContratoFreteCliente> consulta = ConsultarContratos(filtrosPesquisa);

            return ObterLista(consulta, parametrosConsulta);
        }

        public int ContarConsultaContratos(Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaSaldoContratoFreteCliente filtrosPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Escrituracao.ContratoFreteCliente> consulta = ConsultarContratos(filtrosPesquisa);

            return consulta.Count();
        }

        #endregion

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Escrituracao.ContratoFreteCliente> Consultar(Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaContratoFreteCliente filtrosPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Escrituracao.ContratoFreteCliente> consulta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.ContratoFreteCliente>();
            consulta = from obj in consulta select obj;

            if (filtrosPesquisa.ContratoFechado.HasValue)
                consulta = consulta.Where(obj => obj.Fechado == filtrosPesquisa.ContratoFechado);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroContrato))
                consulta = consulta.Where(obj => obj.NumeroContrato.Contains(filtrosPesquisa.NumeroContrato));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                consulta = consulta.Where(obj => obj.Descricao.Contains(filtrosPesquisa.Descricao));

            if (filtrosPesquisa.TipoOperacao > 0)
                consulta = consulta.Where(obj => obj.TipoOperacao.Codigo == filtrosPesquisa.TipoOperacao);

            if (filtrosPesquisa.Cliente > 0)
                consulta = consulta.Where(obj => obj.Cliente.CPF_CNPJ == filtrosPesquisa.Cliente);

            return consulta;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Escrituracao.ContratoFreteCliente> ConsultarContratos(Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaSaldoContratoFreteCliente filtrosPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Escrituracao.ContratoFreteCliente> consulta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.ContratoFreteCliente>();
            consulta = from obj in consulta select obj;

            IQueryable<Dominio.Entidades.Embarcador.Escrituracao.MovimentoContratoFreteCliente> subquery = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.MovimentoContratoFreteCliente>();
            subquery = from obj in subquery select obj;

            if (filtrosPesquisa.CodigoContrato > 0)
                consulta = consulta.Where(o => o.Codigo == filtrosPesquisa.CodigoContrato);

            if (filtrosPesquisa.Cliente > 0)
                consulta = consulta.Where(obj => obj.Cliente.CPF_CNPJ == filtrosPesquisa.Cliente);

            if (filtrosPesquisa.DataInicialContrato != DateTime.MinValue)
                consulta = consulta.Where(obj => obj.DataInicio >= filtrosPesquisa.DataInicialContrato);

            if (filtrosPesquisa.DataFinalContrato != DateTime.MinValue)
                consulta = consulta.Where(obj => obj.DataFim <= filtrosPesquisa.DataFinalContrato);

            if (filtrosPesquisa.Transportador > 0)
            {
                List<int> codigosContrato = subquery.Where(obj => obj.Carga.Empresa.Codigo == filtrosPesquisa.Transportador).Select(obj => obj.ContratoFreteCliente.Codigo).ToList();
                consulta = consulta.Where(o => codigosContrato.Contains(o.Codigo));
            }

            if (filtrosPesquisa.NumeroCarga > 0)
            {
                List<int> codigosContrato = subquery.Where(obj => obj.Carga.Codigo == filtrosPesquisa.NumeroCarga).Select(obj => obj.ContratoFreteCliente.Codigo).ToList();
                consulta = consulta.Where(o => codigosContrato.Contains(o.Codigo));
            }

            return consulta;
        }
        #endregion
    }
}