using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.CTes
{
    public class FaturamentoPorCTe : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioFaturamentoPorCTe, Dominio.Relatorios.Embarcador.DataSource.CTe.FaturamentoPorCTe>
    {
        #region Atributos

        private readonly Repositorio.ConhecimentoDeTransporteEletronico _repositorioFaturamentoPorCTe;

        #endregion

        #region Construtores

        public FaturamentoPorCTe(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioFaturamentoPorCTe = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.CTe.FaturamentoPorCTe> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioFaturamentoPorCTe filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioFaturamentoPorCTe.ConsultarRelatorioFaturamentoPorCTe(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioFaturamentoPorCTe filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioFaturamentoPorCTe.ContarConsultaRelatorioFaturamentoPorCTe(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/CTe/FaturamentoPorCTe";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioFaturamentoPorCTe filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoa = new Repositorio.Embarcador.Pessoas.GrupoPessoas(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Porto repPorto = new Repositorio.Embarcador.Pedidos.Porto(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoViagemNavio repViagem = new Repositorio.Embarcador.Pedidos.PedidoViagemNavio(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Container repContainer = new Repositorio.Embarcador.Pedidos.Container(_unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Carga carga = filtrosPesquisa.CodigoCarga > 0 ? repCarga.BuscarPorCodigo(filtrosPesquisa.CodigoCarga) : null;
            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = filtrosPesquisa.CodigoGrupoPessoas > 0 ? repGrupoPessoa.BuscarPorCodigo(filtrosPesquisa.CodigoGrupoPessoas) : null;
            Dominio.Entidades.Cliente tomador = filtrosPesquisa.CpfCnpjTomador > 0D ? repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CpfCnpjTomador) : null;
            Dominio.Entidades.Embarcador.Pedidos.Porto portoOrigem = filtrosPesquisa.CodigoPortoOrigem > 0 ? repPorto.BuscarPorCodigo(filtrosPesquisa.CodigoPortoOrigem) : null;
            Dominio.Entidades.Embarcador.Pedidos.Porto portoDestino = filtrosPesquisa.CodigoPortoDestino > 0 ? repPorto.BuscarPorCodigo(filtrosPesquisa.CodigoPortoDestino) : null;
            Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio viagem = filtrosPesquisa.CodigoViagem > 0 ? repViagem.BuscarPorCodigo(filtrosPesquisa.CodigoViagem) : null;
            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = filtrosPesquisa.CodigoTipoOperacao > 0 ? repTipoOperacao.BuscarPorCodigo(filtrosPesquisa.CodigoTipoOperacao) : null;

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEmissao", filtrosPesquisa.DataInicialEmissao, filtrosPesquisa.DataFinalEmissao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataFatura", filtrosPesquisa.DataInicialFatura, filtrosPesquisa.DataFinalFatura));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataVencimentoFatura", filtrosPesquisa.DataInicialVencimentoFatura, filtrosPesquisa.DataFinalVencimentoFatura));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Numero", filtrosPesquisa.NumeroInicial, filtrosPesquisa.NumeroFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroFatura", filtrosPesquisa.NumeroFatura));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroTitulo", filtrosPesquisa.NumeroTitulo));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroBoleto", filtrosPesquisa.NumeroBoleto));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SituacaoFatura", filtrosPesquisa.SituacaoFatura?.ObterDescricao()));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Carga", carga?.CodigoCargaEmbarcador));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoPessoas", grupoPessoas?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("StatusCTe", Servicos.Embarcador.CTe.CTe.ObterDescricaoSituacao(filtrosPesquisa.StatusCTe)));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NFe", filtrosPesquisa.NFe));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoServico", filtrosPesquisa.TipoServico.Select(o => o.ObterDescricao())));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SituacaoFaturamentoCTe", filtrosPesquisa.SituacaoFaturamentoCTe?.ObterDescricao()));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoProposta", filtrosPesquisa.TipoProposta?.ObterDescricao()));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("VeioPorImportacao", filtrosPesquisa.VeioPorImportacao.ObterDescricao()));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SomenteCTeSubstituido", filtrosPesquisa.SomenteCTeSubstituido ? "Sim" : "Não"));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoTomador", filtrosPesquisa.TipoTomador.Select(o => o.ToString().ToEnum<Dominio.Enumeradores.TipoTomador>().ObterDescricao())));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("StatusTitulo", filtrosPesquisa.StatusTitulo.Select(o => o.ObterDescricao())));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroBooking", filtrosPesquisa.NumeroBooking));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroOS", filtrosPesquisa.NumeroOS));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroControle", filtrosPesquisa.NumeroControle));
            if (filtrosPesquisa.SituacoesCargaMercante != null && filtrosPesquisa.SituacoesCargaMercante.Count > 1)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SituacaoCarga", filtrosPesquisa.SituacoesCargaMercante != null ? string.Join(",", filtrosPesquisa.SituacoesCargaMercante.Select(o => o.ObterDescricao())) : null));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SituacaoCarga", filtrosPesquisa.SituacaoCarga.ObterDescricao()));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("PortoOrigem", portoOrigem?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("PortoDestino", portoDestino?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Viagem", viagem?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Tomador", tomador?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", parametrosConsulta != null ? parametrosConsulta.PropriedadeAgrupar : string.Empty));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoCTe", filtrosPesquisa.TiposCTe.Select(o => o.ObterDescricao())));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoOperacao", tipoOperacao?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataPrevisaoSaidaNavio", filtrosPesquisa.DataInicialPrevisaoSaidaNavio, filtrosPesquisa.DataFinalPrevisaoSaidaNavio));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar == "DataEmissaoFormatada")
                return "DataEmissao";

            if (propriedadeOrdenarOuAgrupar == "DataFaturaFormatada")
                return "DataFatura";

            if (propriedadeOrdenarOuAgrupar == "DataVencimentoBoletoFormatada")
                return "DataVencimentoBoleto";

            if (propriedadeOrdenarOuAgrupar == "DataBoletoFormatada")
                return "DataBoleto";

            if (propriedadeOrdenarOuAgrupar == "DataEnvioFaturaFormatada")
                return "DataEnvioFatura";

            if (propriedadeOrdenarOuAgrupar == "DataVencimentoFaturaFormatada")
                return "DataVencimentoFatura";

            if (propriedadeOrdenarOuAgrupar == "DataEnvioBoletoFormatada")
                return "DataEnvioBoleto";

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion 
    }
}