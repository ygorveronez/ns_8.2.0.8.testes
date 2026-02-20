using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Enumeradores;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Relatorios.Canhotos
{
    public class Canhoto : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Canhoto.FiltroPesquisaCanhoto, Dominio.Relatorios.Embarcador.DataSource.Canhotos.Canhoto.Canhoto>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Canhotos.Canhoto _repositorioCanhoto;

        #endregion

        #region Construtores

        public Canhoto(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(_unitOfWork);
        }

        public Canhoto(
            Repositorio.UnitOfWork unitOfWork,
            AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware,
            AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware,
            CancellationToken cancellationToken) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(_unitOfWork, cancellationToken);
        }

        #endregion

        #region
        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Canhotos.Canhoto.Canhoto>> ConsultarRegistrosAsync(Dominio.ObjetosDeValor.Embarcador.Canhoto.FiltroPesquisaCanhoto filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return await _repositorioCanhoto.ConsultarRelatorioCanhotoAsync(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta);
        }
        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Canhotos.Canhoto.Canhoto> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Canhoto.FiltroPesquisaCanhoto filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioCanhoto.ConsultarRelatorioCanhoto(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Canhoto.FiltroPesquisaCanhoto filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioCanhoto.ContarConsultaRelatorioCanhoto(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Canhotos/Canhoto";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Canhoto.FiltroPesquisaCanhoto filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Embarcador.Relatorios.RelatorioControleGeracao repRelatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(_unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(_unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(_unitOfWork);
            Repositorio.Embarcador.Canhotos.LocalArmazenamentoCanhoto repLocalArmazenamentoCanhoto = new Repositorio.Embarcador.Canhotos.LocalArmazenamentoCanhoto(_unitOfWork);
            Repositorio.Localidade repositorioLocalidades = new Repositorio.Localidade(_unitOfWork);
            Repositorio.Embarcador.Canhotos.Malote repositorioMalote = new Repositorio.Embarcador.Canhotos.Malote(_unitOfWork);

            Dominio.Entidades.Cliente terceiro = filtrosPesquisa.Terceiro > 0d ? repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.Terceiro) : null;
            Dominio.Entidades.Usuario motorista = filtrosPesquisa.Motorista > 0 ? repUsuario.BuscarPorCodigo(filtrosPesquisa.Motorista) : null;
            Dominio.Entidades.Cliente emitente = filtrosPesquisa.Pessoa > 0d ? repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.Pessoa) : null;
            Dominio.Entidades.Cliente recebedor = filtrosPesquisa.Recebedor > 0d ? repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.Recebedor) : null;
            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupo = filtrosPesquisa.GrupoPessoa > 0 ? repGrupoPessoas.BuscarPorCodigo(filtrosPesquisa.GrupoPessoa) : null;
            Dominio.Entidades.Embarcador.Canhotos.LocalArmazenamentoCanhoto localArmazenamentoCanhoto = filtrosPesquisa.CodigoLocalArmazenamento > 0 ? repLocalArmazenamentoCanhoto.BuscarPorCodigo(filtrosPesquisa.CodigoLocalArmazenamento) : null;
            Dominio.Entidades.Usuario usuario = filtrosPesquisa.Usuario > 0 ? repUsuario.BuscarPorCodigo(filtrosPesquisa.Usuario) : null;
            Dominio.Entidades.Veiculo veiculo = !string.IsNullOrWhiteSpace(filtrosPesquisa.PlacaVeiculoResponsavelEntrega) ? new Repositorio.Veiculo(_unitOfWork).BuscarPorPlaca(filtrosPesquisa.PlacaVeiculoResponsavelEntrega) : null;
            Dominio.Entidades.Localidade origem = filtrosPesquisa.CodigoLocalidadeOrigem > 0 ? repositorioLocalidades.BuscarPorCodigo(filtrosPesquisa.CodigoLocalidadeOrigem) : null;
            Dominio.Entidades.Localidade destino = filtrosPesquisa.CodigoLocalidadeDestino > 0 ? repositorioLocalidades.BuscarPorCodigo(filtrosPesquisa.CodigoLocalidadeDestino) : null;
            Dominio.Entidades.Cliente expedidor = filtrosPesquisa.CnpjExpedidor > 0d ? repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CnpjExpedidor) : null;
            Dominio.Entidades.Embarcador.Canhotos.Malote malote = filtrosPesquisa.CodigoMalote > 0 ? repositorioMalote.BuscarPorCodigo(filtrosPesquisa.CodigoMalote) : null;

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Periodo", filtrosPesquisa.DataInicio, filtrosPesquisa.DataFim));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Carga", false));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Chave", filtrosPesquisa.Chave));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Terceiro", terceiro != null ? terceiro?.CPF_CNPJ_Formatado + " - " + terceiro?.Nome : string.Empty));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Motorista", motorista?.Nome));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Usuario", usuario?.Nome));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SituacaoCanhoto", filtrosPesquisa.Situacao.HasValue && filtrosPesquisa.Situacao != SituacaoCanhoto.Todas ? filtrosPesquisa.Situacao.Value.ToString("d") : string.Empty, "0"));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SituacaoDigitalizacaoCanhoto", filtrosPesquisa.SituacaoDigitalizacaoCanhoto.HasValue && filtrosPesquisa.SituacaoDigitalizacaoCanhoto != SituacaoDigitalizacaoCanhoto.Todas ? filtrosPesquisa.SituacaoDigitalizacaoCanhoto.Value.ToString("d") : string.Empty, "0"));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoCanhoto", filtrosPesquisa.TipoCanhoto.HasValue && filtrosPesquisa.TipoCanhoto != TipoCanhoto.Todos ? filtrosPesquisa.TipoCanhoto.Value.ToString("d") : string.Empty, "0"));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Emitente", emitente != null ? emitente.CPF_CNPJ_Formatado + " - " + emitente.Nome : string.Empty));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Recebedor", recebedor != null ? recebedor.CPF_CNPJ_Formatado + " - " + recebedor.Nome : string.Empty));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoPessoa", grupo?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("LocalArmazenamento", localArmazenamentoCanhoto?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Pacote", filtrosPesquisa.Pacote));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Posicao", filtrosPesquisa.Posicao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEmissaoCTe", filtrosPesquisa.DataEmissaoCTeInicial, filtrosPesquisa.DataEmissaoCTeFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SituacaoPgtoCanhoto", filtrosPesquisa.SituacaoPgtoCanhoto?.ObterDescricao()));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataDigitalizacao", filtrosPesquisa.DataInicioDigitalizacao, filtrosPesquisa.DataFimDigitalizacao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEnvio", filtrosPesquisa.DataInicioEnvio, filtrosPesquisa.DataFimEnvio));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("OrigemDigitalizacao", filtrosPesquisa.OrigemDigitalizacao.ObterDescricao()));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("PlacaVeiculoResponsavelEntrega", veiculo?.Placa_Formatada));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataHistorico", filtrosPesquisa.DataInicialHistorico, filtrosPesquisa.DataFinalHistorico));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataCriacaoCarga", filtrosPesquisa.DataCriacaoCargaInicial, filtrosPesquisa.DataCriacaoCargaFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SituacaoHistorico", filtrosPesquisa.SituacaoHistorico?.ObterDescricao()));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Origem", origem?.DescricaoCidadeEstado ?? string.Empty));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Destino", destino?.DescricaoCidadeEstado ?? string.Empty));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Expedidor", expedidor?.Descricao ?? string.Empty));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Malote", malote?.Protocolo.ToString() ?? string.Empty));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SituacaoViagem", filtrosPesquisa.SituacaoViagem?.ObterDescricao() ?? string.Empty));

            if (filtrosPesquisa.Empresas?.Count() > 0)
            {
                if (filtrosPesquisa.Empresas.Count() == 1)
                {
                    Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(_unitOfWork);
                    Dominio.Entidades.Empresa empresa = repositorioEmpresa.BuscarPorCodigo(filtrosPesquisa.Empresas.FirstOrDefault());
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Empresa", empresa.CNPJ_Formatado + " - " + empresa.RazaoSocial, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Empresa", "Múltiplos Registros Selecionados", true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Empresa", false));

            if (filtrosPesquisa.Filiais?.Count() > 0)
            {
                if (filtrosPesquisa.Filiais.Count() == 1)
                {
                    Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
                    Dominio.Entidades.Embarcador.Filiais.Filial filial = repositorioFilial.BuscarPorCodigo(filtrosPesquisa.Filiais.FirstOrDefault());
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Filial", filial.Descricao, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Filial", "Múltiplos Registros Selecionados", true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Filial", false));

            if (filtrosPesquisa.Numeros?.Count() > 0)
            {
                if (filtrosPesquisa.Numeros.Count() == 1)
                {
                    Repositorio.Embarcador.Canhotos.Canhoto repositorioCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(_unitOfWork);
                    Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = repositorioCanhoto.BuscarPorCodigo(filtrosPesquisa.Numeros.FirstOrDefault());
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Numero", canhoto.Descricao, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Numero", "Múltiplos Registros Selecionados", true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Numero", false));

            if (filtrosPesquisa.TiposCarga?.Count() > 0)
            {
                if (filtrosPesquisa.TiposCarga.Count() == 1)
                {
                    Repositorio.Embarcador.Cargas.TipoDeCarga repositorioTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(_unitOfWork);
                    Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga = repositorioTipoDeCarga.BuscarPorCodigo(filtrosPesquisa.TiposCarga.FirstOrDefault());
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoCarga", tipoCarga.Descricao, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoCarga", "Múltiplos Registros Selecionados", true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoCarga", false));

            if (filtrosPesquisa.TiposOperacao?.Count() > 0)
            {
                if (filtrosPesquisa.TiposOperacao.Count() == 1)
                {
                    Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork);
                    Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repositorioTipoOperacao.BuscarPorCodigo(filtrosPesquisa.TiposOperacao.FirstOrDefault());
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoOperacao", tipoOperacao.Descricao, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoOperacao", "Múltiplos Registros Selecionados", true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoOperacao", false));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar == "DescricaoSituacaoCanhoto")
                return "SituacaoCanhoto";

            if (propriedadeOrdenarOuAgrupar == "DescricaoDataEmissao")
                return "DataEmissao";

            if (propriedadeOrdenarOuAgrupar == "DescricaoDataEnvioCanhoto")
                return "DataEnvioCanhoto";

            if (propriedadeOrdenarOuAgrupar.Contains("Formatada"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatada", "");

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion 
    }
}