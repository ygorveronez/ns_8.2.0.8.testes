using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;

namespace Servicos.Embarcador.Relatorios.Chamado
{
    public class ChamadoDevolucao : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaRelatorioChamadoDevolucao, Dominio.Relatorios.Embarcador.DataSource.Chamados.ChamadoDevolucao.ChamadoDevolucao>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Chamados.Chamado _repChamadoDevolucao;

        #endregion

        #region Construtores

        public ChamadoDevolucao(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repChamadoDevolucao = new Repositorio.Embarcador.Chamados.Chamado(_unitOfWork);
        }
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Chamados.ChamadoDevolucao.ChamadoDevolucao> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaRelatorioChamadoDevolucao filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repChamadoDevolucao.ConsultarChamadoDevolucao(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaRelatorioChamadoDevolucao filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repChamadoDevolucao.ContarConsultaChamadoDevolucao(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Chamados/ChamadoDevolucao";
        }

        protected override List<Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaRelatorioChamadoDevolucao filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Parametro> parametros = new List<Parametro>();

            Repositorio.Usuario repUsuario = new Repositorio.Usuario(_unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(_unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
            Repositorio.Embarcador.Pessoas.Representante repRepresentante = new Repositorio.Embarcador.Pessoas.Representante(_unitOfWork);
            Repositorio.Embarcador.Chamados.MotivoChamado repMotivoChamado = new Repositorio.Embarcador.Chamados.MotivoChamado(_unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);

            Dominio.Entidades.Empresa empresa = filtrosPesquisa.CodigoTransportador > 0 ? repEmpresa.BuscarPorCodigo(filtrosPesquisa.CodigoTransportador) : null;
            Dominio.Entidades.Usuario usuario = filtrosPesquisa.CodigoResponsavel > 0 ? repUsuario.BuscarPorCodigo(filtrosPesquisa.CodigoResponsavel) : null;
            Dominio.Entidades.Embarcador.Filiais.Filial filial = filtrosPesquisa.CodigoFilial > 0 ? repFilial.BuscarPorCodigo(filtrosPesquisa.CodigoFilial) : null;
            Dominio.Entidades.Cliente cliente = filtrosPesquisa.CpfCnpjCliente > 0 ? repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CpfCnpjCliente) : null;
            Dominio.Entidades.Usuario motorista = filtrosPesquisa.CodigoMotorista > 0 ? repUsuario.BuscarPorCodigo(filtrosPesquisa.CodigoMotorista) : null;
            Dominio.Entidades.Embarcador.Pessoas.Representante representante = filtrosPesquisa.CodigoRepresentante > 0 ? repRepresentante.BuscarPorCodigo(filtrosPesquisa.CodigoRepresentante) : null;
            Dominio.Entidades.Cliente tomador = filtrosPesquisa.CpfCnpjTomador > 0 ? repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CpfCnpjTomador) : null;
            Dominio.Entidades.Cliente destinatario = filtrosPesquisa.CpfCnpjDestinatario > 0 ? repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CpfCnpjDestinatario) : null;
            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoaTomador = filtrosPesquisa.CodigoGrupoPessoasTomador > 0 ? repGrupoPessoas.BuscarPorCodigo(filtrosPesquisa.CodigoGrupoPessoasTomador) : null;
            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoaCliente = filtrosPesquisa.CodigoGrupoPessoasCliente > 0 ? repGrupoPessoas.BuscarPorCodigo(filtrosPesquisa.CodigoGrupoPessoasCliente) : null;
            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoaDestinatario = filtrosPesquisa.CodigoGrupoPessoasDestinatario > 0 ? repGrupoPessoas.BuscarPorCodigo(filtrosPesquisa.CodigoGrupoPessoasDestinatario) : null;
            Dominio.Entidades.Embarcador.Filiais.Filial filialVenda = filtrosPesquisa.CodigoFilialVenda > 0 ? repFilial.BuscarPorCodigo(filtrosPesquisa.CodigoFilialVenda) : null;
            List<Dominio.Entidades.Cliente> clientesResponsavel = filtrosPesquisa.CpfCnpjClienteResponsavel.Count > 0 ? repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CpfCnpjClienteResponsavel) : new List<Dominio.Entidades.Cliente>();
            List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas> grupoPessoasResponsavel = filtrosPesquisa.CodigosGrupoPessoasResponsavel.Count > 0 ? repGrupoPessoas.BuscarPorCodigo(filtrosPesquisa.CodigosGrupoPessoasResponsavel) : new List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas>();
            List<Dominio.Entidades.Embarcador.Chamados.MotivoChamado> motivos = filtrosPesquisa.CodigosMotivo.Count > 0 ? repMotivoChamado.BuscarPorCodigos(filtrosPesquisa.CodigosMotivo) : new List<Dominio.Entidades.Embarcador.Chamados.MotivoChamado>();
            Dominio.Entidades.Veiculo veiculo = filtrosPesquisa.CodigoVeiculo > 0 ? repVeiculo.BuscarPorCodigo(filtrosPesquisa.CodigoVeiculo) : null;

            parametros.Add(new Parametro("NumeroInicial", filtrosPesquisa.NumeroInicial));
            parametros.Add(new Parametro("NumeroFinal", filtrosPesquisa.NumeroFinal));
            parametros.Add(new Parametro("Carga", filtrosPesquisa.Carga));
            parametros.Add(new Parametro("Placa", veiculo?.Placa ?? filtrosPesquisa.Placa));
            parametros.Add(new Parametro("Transportador", empresa?.Descricao));
            parametros.Add(new Parametro("Responsavel", usuario?.Descricao));
            parametros.Add(new Parametro("Filial", filial?.Descricao));
            parametros.Add(new Parametro("SituacaoChamado", filtrosPesquisa.Situacao.ObterDescricao()));
            parametros.Add(new Parametro("Motivo", motivos.Select(o => o.Descricao)));
            parametros.Add(new Parametro("Cliente", cliente?.Nome));
            parametros.Add(new Parametro("Motorista", motorista?.Nome));
            parametros.Add(new Parametro("Representante", representante?.Descricao));
            parametros.Add(new Parametro("Nota", filtrosPesquisa.Nota));
            parametros.Add(new Parametro("NumeroCTe", filtrosPesquisa.NumeroCTe));
            parametros.Add(new Parametro("GerouOcorrencia", filtrosPesquisa.GerouOcorrencia ? "Sim" : ""));
            parametros.Add(new Parametro("DataCriacaoInicio", filtrosPesquisa.DataCriacaoInicio));
            parametros.Add(new Parametro("DataCriacaoFim", filtrosPesquisa.DataCriacaoFim));
            parametros.Add(new Parametro("DataFinalizacaoInicio", filtrosPesquisa.DataFinalizacaoInicio));
            parametros.Add(new Parametro("DataFinalizacaoFim", filtrosPesquisa.DataFinalizacaoFim));
            parametros.Add(new Parametro("Tomador", tomador?.Nome));
            parametros.Add(new Parametro("GrupoPessoasTomador", grupoPessoaTomador?.Descricao));
            parametros.Add(new Parametro("FilialVenda", filialVenda?.Descricao));
            parametros.Add(new Parametro("PeriodoChegadaDiaria", filtrosPesquisa.DataInicialChegadaDiaria, filtrosPesquisa.DataFinalChegadaDiaria));
            parametros.Add(new Parametro("PeriodoSaidaDiaria", filtrosPesquisa.DataInicialSaidaDiaria, filtrosPesquisa.DataFinalSaidaDiaria));
            parametros.Add(new Parametro("Destinatario", destinatario?.Nome));
            parametros.Add(new Parametro("GrupoPessoasCliente", grupoPessoaCliente?.Descricao));
            parametros.Add(new Parametro("GrupoPessoasDestinatario", grupoPessoaDestinatario?.Descricao));
            parametros.Add(new Parametro("ClienteResponsavel", clientesResponsavel.Select(o => o.Nome)));
            parametros.Add(new Parametro("GrupoPessoasResponsavel", grupoPessoasResponsavel.Select(o => o.Descricao)));
            parametros.Add(new Parametro("SomenteAtendimentoEstornado", filtrosPesquisa.SomenteAtendimentoEstornado ? "Sim" : ""));
            parametros.Add(new Parametro("PossuiAnexoNFSe", filtrosPesquisa.PossuiAnexoNFSe));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar == "DescricaoSituacao")
                propriedadeOrdenarOuAgrupar = "Situacao";

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion


    }
}
