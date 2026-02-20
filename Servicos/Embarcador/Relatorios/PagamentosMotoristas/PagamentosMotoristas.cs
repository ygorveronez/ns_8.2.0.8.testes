using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Enumeradores;
using System.Collections.Generic;
using System.Linq;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;

namespace Servicos.Embarcador.Relatorios.PagamentosMotoristas
{
    public class PagamentosMotoristas : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.PagamentosMotoristas.FiltroPesquisaPagamentoMotoristaTMS, Dominio.Relatorios.Embarcador.DataSource.PagamentoMotorista.PagamentoMotorista>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS _repositorio;

        #endregion

        #region Construtores

        public PagamentosMotoristas(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorio = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.PagamentoMotorista.PagamentoMotorista> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.PagamentosMotoristas.FiltroPesquisaPagamentoMotoristaTMS filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorio.ConsultarRelatorio(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.PagamentosMotoristas.FiltroPesquisaPagamentoMotoristaTMS filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorio.ContarConsultaRelatorio(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/PagamentosMotoristas/PagamentoMotoristaTMS";
        }

        protected override List<Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.PagamentosMotoristas.FiltroPesquisaPagamentoMotoristaTMS filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Parametro> parametros = new List<Parametro>();

            Repositorio.Usuario repUsuario = new Repositorio.Usuario(_unitOfWork);
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo repPagamentoMotoristaTipo = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo(_unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);


            Dominio.Entidades.Usuario operador = filtrosPesquisa.CodigoOperador > 0 ? repUsuario.BuscarPorCodigo(filtrosPesquisa.CodigoOperador) : null;
            Dominio.Entidades.Usuario motorista = filtrosPesquisa.CodigoMotorista > 0 ? repUsuario.BuscarPorCodigo(filtrosPesquisa.CodigoMotorista) : null;
            List<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo> tiposPagamento = filtrosPesquisa.CodigosTipoPagamento.Count > 0 ? repPagamentoMotoristaTipo.BuscarPorCodigos(filtrosPesquisa.CodigosTipoPagamento) : new List<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo>();
            Dominio.Entidades.Cliente favorecido = filtrosPesquisa.CpfCnpjFavorecido > 0 ? repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CpfCnpjFavorecido) : null;

            parametros.Add(new Parametro("Data", filtrosPesquisa.DataInicial, filtrosPesquisa.DataFinal));
            parametros.Add(new Parametro("NumeroPagamento", filtrosPesquisa.NumeroPagamento));
            parametros.Add(new Parametro("NumeroCarga", filtrosPesquisa.NumeroCarga));
            parametros.Add(new Parametro("Situacao", filtrosPesquisa.Situacao?.ObterDescricao()));
            parametros.Add(new Parametro("Etapa", filtrosPesquisa.Etapa?.ObterDescricao()));
            parametros.Add(new Parametro("Operador", operador?.Descricao));
            parametros.Add(new Parametro("TipoPagamento", tiposPagamento.Select(o => o.Descricao)));
            parametros.Add(new Parametro("Motorista", motorista?.Descricao));
            parametros.Add(new Parametro("PagamentosSemAcertoViagem", filtrosPesquisa.PagamentosSemAcertoViagem ? "Sim" : string.Empty));
            parametros.Add(new Parametro("Favorecido", favorecido?.Nome));
            parametros.Add(new Parametro("DataEfetivacao", filtrosPesquisa.DataEfetivacaoInicial, filtrosPesquisa.DataEfetivacaoFinal));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar == "DataPagamentoFormatada")
                return "DataPagamento";

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion 
    }
}