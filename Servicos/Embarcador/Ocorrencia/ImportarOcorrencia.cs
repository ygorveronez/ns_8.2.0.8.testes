using Dominio.Excecoes.Embarcador;
using System;
using System.Collections.Generic;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Servicos.Embarcador.Ocorrencia
{
    public class ImportarOcorrencia
    {
        #region Atributos Privados Somente Leitura

        private readonly Dictionary<string, dynamic> _dados;
        private readonly AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;
        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracao;

        #endregion

        #region Construtores

        public ImportarOcorrencia(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dictionary<string, dynamic> dados, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            _dados = dados;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
            _unitOfWork = unitOfWork;
            _configuracao = configuracao;
        }

        #endregion

        #region Métodos Privados
        private int ObterNumeroCTe()
        {
            int valor = 0;

            try
            {
                if (_dados.TryGetValue("NumeroCTe", out var numeroCTe))
                    valor = (((string)numeroCTe).Trim()).ToInt();
            }
            catch
            {
            }

            if (valor == 0)
                throw new ImportacaoException("Número do CT-e não informado");

            return valor;
        }
        private int ObterSerieCTe()
        {
            int valor = 0;
            try
            {
                if (_dados.TryGetValue("SerieCTe", out var numeroCTe))
                    valor = (((string)numeroCTe).Trim()).ToInt();
            }
            catch
            {
            }

            if (valor == 0)
                throw new ImportacaoException("Série do CT-e não informado");

            return valor;
        }

        private DateTime? ObterDataOcorrencia()
        {
            DateTime? data = null;
            try
            {
                if (_dados.TryGetValue("DataOcorrencia", out var dataOcorrencia))
                    data = ((string)dataOcorrencia).ToNullableDateTime();
            }
            catch
            {
            }


            if (data == null)
                throw new ImportacaoException("Data da Ocorrência não informada");

            return data;
        }

        private string ObterObservacao()
        {
            var valor = string.Empty;

            if (_dados.TryGetValue("Observacao", out var observacao))
                valor = ((string)observacao);

            return valor;
        }

        private string ObterObservacaoImpressa()
        {
            var valor = string.Empty;

            if (_dados.TryGetValue("ObservacaoImpressa", out var observacao))
                valor = ((string)observacao);

            return valor;
        }

        private string ObterBooking()
        {
            var valor = string.Empty;

            if (_dados.TryGetValue("Booking", out var observacao))
                valor = ((string)observacao);

            return valor;
        }

        private string ObterOrdemServico()
        {
            var valor = string.Empty;

            if (_dados.TryGetValue("OrdemServico", out var observacao))
                valor = ((string)observacao);

            return valor;
        }

        private string ObterNumeroCarga()
        {
            var valor = string.Empty;

            if (_dados.TryGetValue("NumeroCarga", out var observacao))
                valor = ((string)observacao);

            if (string.IsNullOrWhiteSpace(valor))
                throw new ImportacaoException("Número da Carga não informada");

            return valor;
        }

        private string ObterCST()
        {
            var valor = string.Empty;

            if (_dados.TryGetValue("CST", out var observacao))
                valor = ((string)observacao);

            return valor;
        }

        private string ObterCodigoTipoOcorrencia()
        {
            var valor = string.Empty;

            if (_dados.TryGetValue("CodigoTipoOcorrencia", out var observacao))
                valor = ((string)observacao);

            if (string.IsNullOrWhiteSpace(valor))
                throw new ImportacaoException("Código do tipo da ocorrência não informada");

            return valor;
        }

        private string ObterCodigoComponenteFrete()
        {
            var valor = string.Empty;

            if (_dados.TryGetValue("CodigoComponenteFrete", out var observacao))
                valor = ((string)observacao);

            if (string.IsNullOrWhiteSpace(valor))
                throw new ImportacaoException("Código do componente não informado");

            return valor;
        }
        private decimal ObterValorOcorrencia()
        {
            decimal valor = 0;

            try
            {
                if (_dados.TryGetValue("ValorOcorrencia", out var valorAcrescimo))
                    valor = (((string)valorAcrescimo).Trim()).ToDecimal();
            }
            catch
            {
            }

            if (valor == 0)
                throw new ImportacaoException("Valor da ocorrência não informada");

            return valor;
        }

        private decimal ObterAliquotaICMS()
        {
            decimal valor = 0;

            try
            {
                if (_dados.TryGetValue("AliquotaICMS", out var valorAcrescimo))
                    valor = (((string)valorAcrescimo).Trim()).ToDecimal();
            }
            catch
            {
            }

            return valor;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Ocorrencias.ImportarOcorrencia ObterImportarOcorrencia()
        {
            Repositorio.TipoDeOcorrenciaDeCTe repTipoOcorrencia = new Repositorio.TipoDeOcorrenciaDeCTe(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(_unitOfWork);
            Repositorio.Embarcador.Ocorrencias.ImportarOcorrencia repImportarOcorrencia = new Repositorio.Embarcador.Ocorrencias.ImportarOcorrencia(_unitOfWork);

            Dominio.Entidades.Embarcador.Ocorrencias.ImportarOcorrencia importarOcorrencia = new Dominio.Entidades.Embarcador.Ocorrencias.ImportarOcorrencia();
            importarOcorrencia.Initialize();

            importarOcorrencia.NumeroCTe = ObterNumeroCTe();
            importarOcorrencia.SerieCTe = ObterSerieCTe();
            importarOcorrencia.DataOcorrencia = ObterDataOcorrencia();
            importarOcorrencia.Observacao = ObterObservacao();
            importarOcorrencia.ObservacaoImpressa = ObterObservacaoImpressa();
            importarOcorrencia.Booking = ObterBooking();
            importarOcorrencia.OrdemServico = ObterOrdemServico();
            importarOcorrencia.NumeroCarga = ObterNumeroCarga();
            importarOcorrencia.CST = ObterCST();
            importarOcorrencia.CodigoTipoOcorrencia = ObterCodigoTipoOcorrencia();
            importarOcorrencia.CodigoComponenteFrete = ObterCodigoComponenteFrete();
            importarOcorrencia.ValorOcorrencia = ObterValorOcorrencia();
            importarOcorrencia.AliquotaICMS = ObterAliquotaICMS();

            importarOcorrencia.TipoOcorrencia = repTipoOcorrencia.BuscarPorCodigoIntegracao(importarOcorrencia.CodigoTipoOcorrencia);
            importarOcorrencia.Carga = repCarga.BuscarCargaPorCodigoEmbarcador(importarOcorrencia.NumeroCarga);
            importarOcorrencia.CTe = repCargaCTe.BuscarPorNumeroSerieCTe(importarOcorrencia.NumeroCTe, importarOcorrencia.SerieCTe, importarOcorrencia.Carga?.Codigo ?? 0);
            importarOcorrencia.ComponenteFrete = repComponenteFrete.BuscarPorCodigoIntegracao(importarOcorrencia.CodigoComponenteFrete);

            if (importarOcorrencia.TipoOcorrencia == null)
                throw new ImportacaoException("Não foi localizado um tipo de ocorrência com o código importado.");
            if (importarOcorrencia.Carga == null)
                throw new ImportacaoException("Não foi localizado uma carga com o número importado.");
            if (importarOcorrencia.CTe == null)
                throw new ImportacaoException("Não foi localizado o CT-e com o número importado.");
            if (importarOcorrencia.ComponenteFrete == null)
                throw new ImportacaoException("Não foi localizado o componente to frete com o código importado.");
            if (importarOcorrencia.TipoOcorrencia != null && importarOcorrencia.ComponenteFrete != null && importarOcorrencia.ComponenteFrete.TipoComponenteFrete == TipoComponenteFrete.ICMS && importarOcorrencia.AliquotaICMS <= 0)
                throw new ImportacaoException("Não foi informado a Alíquota de ICMS para a geração do complemento.");

            //if (repImportarOcorrencia.RegistroDuplicado(importarOcorrencia.NumeroCarga, importarOcorrencia.CodigoTipoOcorrencia, importarOcorrencia.CodigoComponenteFrete, importarOcorrencia.NumeroCTe, importarOcorrencia.SerieCTe, importarOcorrencia.ValorOcorrencia, importarOcorrencia.DataOcorrencia, importarOcorrencia.AliquotaICMS))
            //    throw new ImportacaoException("Já existe uma importação de ocorrência com os mesmos dados de carga/cte/ocorrencia.");

            return importarOcorrencia;
        }

        #endregion
    }
}
