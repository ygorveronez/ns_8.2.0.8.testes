using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Logistica
{
    public static class CentroDescarregamento
    {
        private static Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha ObterFalhaLinha(string mensagem, int indice)
        {
            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retorno = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = indice, mensagemFalha = mensagem, processou = false };
            return retorno;
        }
        private static Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha ObterSucessoLinha(string mensagem, int indice)
        {
            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retorno = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = indice, mensagemFalha = mensagem, processou = true };
            return retorno;
        }
        private static Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento InserirDiaSemana(Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamento, TimeSpan horaInico, TimeSpan horaFim, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana diaSemana, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.PeriodoDescarregamento repPeriodoDescarregamento = new Repositorio.Embarcador.Logistica.PeriodoDescarregamento(unitOfWork);

            Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento PeriodoDescarregamento = new Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento()
            {
                CentroDescarregamento = centroDescarregamento,
                HoraInicio = horaInico,
                HoraTermino = horaFim,
                Dia = diaSemana,
                ToleranciaExcessoTempo = 0,
                CapacidadeDescarregamentoSimultaneo = 1
            };


            repPeriodoDescarregamento.Inserir(PeriodoDescarregamento);

            return PeriodoDescarregamento;
        }
        private static void InserirPeriodoDescarregamento(Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamento, Dominio.Entidades.Cliente destinatario, Dominio.Entidades.Cliente remetente, Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoDeCarga, TimeSpan horaInico, TimeSpan horaFim, bool Domingo, bool Segunda, bool Terca, bool Quarta, bool Quinta, bool Sexta, bool Sabado, Repositorio.UnitOfWork unitOfWork)
        {
            if (Domingo)
            {
                Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento periodo = InserirDiaSemana(centroDescarregamento, horaInico, horaFim, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana.Domingo, unitOfWork);
                SalvarPeriodoDescarregamentoRemetente(remetente, periodo, unitOfWork);
                SalvarPeriodoDescarregamentoTipoDeCarga(periodo, tipoDeCarga, unitOfWork);
            }

            if (Segunda)
            {
                Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento periodo = InserirDiaSemana(centroDescarregamento, horaInico, horaFim, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana.Segunda, unitOfWork);
                SalvarPeriodoDescarregamentoRemetente(remetente, periodo, unitOfWork);
                SalvarPeriodoDescarregamentoTipoDeCarga(periodo, tipoDeCarga, unitOfWork);
            }

            if (Terca)
            {
                Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento periodo = InserirDiaSemana(centroDescarregamento, horaInico, horaFim, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana.Terca, unitOfWork);
                SalvarPeriodoDescarregamentoRemetente(remetente, periodo, unitOfWork);
                SalvarPeriodoDescarregamentoTipoDeCarga(periodo, tipoDeCarga, unitOfWork);
            }

            if (Quarta)
            {
                Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento periodo = InserirDiaSemana(centroDescarregamento, horaInico, horaFim, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana.Quarta, unitOfWork);
                SalvarPeriodoDescarregamentoRemetente(remetente, periodo, unitOfWork);
                SalvarPeriodoDescarregamentoTipoDeCarga(periodo, tipoDeCarga, unitOfWork);
            }

            if (Quinta)
            {
                Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento periodo = InserirDiaSemana(centroDescarregamento, horaInico, horaFim, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana.Quinta, unitOfWork);
                SalvarPeriodoDescarregamentoRemetente(remetente, periodo, unitOfWork);
                SalvarPeriodoDescarregamentoTipoDeCarga(periodo, tipoDeCarga, unitOfWork);
            }

            if (Sexta)
            {
                Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento periodo = InserirDiaSemana(centroDescarregamento, horaInico, horaFim, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana.Sexta, unitOfWork);
                SalvarPeriodoDescarregamentoRemetente(remetente, periodo, unitOfWork);
                SalvarPeriodoDescarregamentoTipoDeCarga(periodo, tipoDeCarga, unitOfWork);
            }

            if (Sabado)
            {
                Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento periodo = InserirDiaSemana(centroDescarregamento, horaInico, horaFim, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana.Sabado, unitOfWork);
                SalvarPeriodoDescarregamentoRemetente(remetente, periodo, unitOfWork);
                SalvarPeriodoDescarregamentoTipoDeCarga(periodo, tipoDeCarga, unitOfWork);
            }
        }
        private static TimeSpan? ObterHora(string hora)
        {
            try
            {
                DateTime.TryParse(hora, out DateTime data);

                return new TimeSpan(data.Hour, data.Minute, 0);
            }
            catch
            {
                return null;
            }
        }
        private static void SalvarPeriodoDescarregamentoRemetente(Dominio.Entidades.Cliente remetente, Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento periodo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.PeriodoDescarregamentoRemetente repPeriodoDescarregamentoRemetente = new Repositorio.Embarcador.Logistica.PeriodoDescarregamentoRemetente(unitOfWork);

            Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoRemetente periodoRemetente = new Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoRemetente()
            {
                Remetente = remetente,
                PeriodoDescarregamento = periodo
            };

            repPeriodoDescarregamentoRemetente.Inserir(periodoRemetente);
        }
        private static void SalvarPeriodoDescarregamentoTipoDeCarga(Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento periodo, Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoDeCarga, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Logistica.PeriodoDescarregamentoTipoDeCarga repPeriodoDescarregamentoTipoCarga = new Repositorio.Embarcador.Logistica.PeriodoDescarregamentoTipoDeCarga(unidadeDeTrabalho);


            Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoTipoDeCarga periodoTipoCarga = new Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoTipoDeCarga()
            {
                TipoDeCarga = tipoDeCarga,
                PeriodoDescarregamento = periodo
            };

            repPeriodoDescarregamentoTipoCarga.Inserir(periodoTipoCarga);
        }
        public static Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao ImportarCentroCarregamento(string dados, Dominio.Entidades.Usuario usuario, List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes, Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, string AdminStringConexao, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha>>(dados);
            Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao();
            retornoImportacao.Retornolinhas = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha>();
            List<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento> centrosDescarregamento = new List<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento>();
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoDeCarga repTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
            Repositorio.Embarcador.Logistica.CentroDescarregamento repCentroDescarregamento = new Repositorio.Embarcador.Logistica.CentroDescarregamento(unitOfWork);
            List<Dominio.Entidades.Cliente> destinatariosExlcuidos = new List<Dominio.Entidades.Cliente>();
            Repositorio.Embarcador.Logistica.PeriodoDescarregamento repPeriodoDescarregamento = new Repositorio.Embarcador.Logistica.PeriodoDescarregamento(unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
            int contador = 0;

            for (int i = 0; i < linhas.Count; i++)
            {
                var linha = linhas[i];
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCodigoRemetente = (from obj in linha.Colunas where obj.NomeCampo == "CodigoRemetente" select obj).FirstOrDefault();
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCodigoDestinatario = (from obj in linha.Colunas where obj.NomeCampo == "CodigoDestinatario" select obj).FirstOrDefault();
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTipoCarga = (from obj in linha.Colunas where obj.NomeCampo == "TipoCarga" select obj).FirstOrDefault();
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDomingo = (from obj in linha.Colunas where obj.NomeCampo == "Dom" select obj).FirstOrDefault();
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colSegunda = (from obj in linha.Colunas where obj.NomeCampo == "Seg" select obj).FirstOrDefault();
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTerca = (from obj in linha.Colunas where obj.NomeCampo == "Ter" select obj).FirstOrDefault();
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colQuarta = (from obj in linha.Colunas where obj.NomeCampo == "Qua" select obj).FirstOrDefault();
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colQuinta = (from obj in linha.Colunas where obj.NomeCampo == "Qui" select obj).FirstOrDefault();
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colSexta = (from obj in linha.Colunas where obj.NomeCampo == "Sex" select obj).FirstOrDefault();
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colSabado = (from obj in linha.Colunas where obj.NomeCampo == "Sab" select obj).FirstOrDefault();
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colHoraInicio1 = (from obj in linha.Colunas where obj.NomeCampo == "HoraInicio1" select obj).FirstOrDefault();
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colHoraFim1 = (from obj in linha.Colunas where obj.NomeCampo == "HoraFim1" select obj).FirstOrDefault();
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colHoraInicio2 = (from obj in linha.Colunas where obj.NomeCampo == "HoraInicio2" select obj).FirstOrDefault();
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colHoraFim2 = (from obj in linha.Colunas where obj.NomeCampo == "HoraFim2" select obj).FirstOrDefault();
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colHoraInicio3 = (from obj in linha.Colunas where obj.NomeCampo == "HoraInicio3" select obj).FirstOrDefault();
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colHoraFim3 = (from obj in linha.Colunas where obj.NomeCampo == "HoraFim3" select obj).FirstOrDefault();
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colModeloVeiculo1 = (from obj in linha.Colunas where obj.NomeCampo == "ModeloVeiculo1" select obj).FirstOrDefault();
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colModeloVeiculo2 = (from obj in linha.Colunas where obj.NomeCampo == "ModeloVeiculo2" select obj).FirstOrDefault();
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colModeloVeiculo3 = (from obj in linha.Colunas where obj.NomeCampo == "ModeloVeiculo3" select obj).FirstOrDefault();
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colModeloVeiculo4 = (from obj in linha.Colunas where obj.NomeCampo == "ModeloVeiculo4" select obj).FirstOrDefault();
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colModeloVeiculo5 = (from obj in linha.Colunas where obj.NomeCampo == "ModeloVeiculo5" select obj).FirstOrDefault();
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colModeloVeiculo6 = (from obj in linha.Colunas where obj.NomeCampo == "ModeloVeiculo6" select obj).FirstOrDefault();
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colModeloVeiculo7 = (from obj in linha.Colunas where obj.NomeCampo == "ModeloVeiculo7" select obj).FirstOrDefault();
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colModeloVeiculo8 = (from obj in linha.Colunas where obj.NomeCampo == "ModeloVeiculo8" select obj).FirstOrDefault();
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colModeloVeiculo9 = (from obj in linha.Colunas where obj.NomeCampo == "ModeloVeiculo9" select obj).FirstOrDefault();
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colModeloVeiculo10 = (from obj in linha.Colunas where obj.NomeCampo == "ModeloVeiculo10" select obj).FirstOrDefault();

                Dominio.Entidades.Cliente remetente = null;
                if (colCodigoRemetente != null)
                {
                    remetente = repCliente.BuscarPorCodigoIntegracao(colCodigoRemetente.Valor);
                    if (remetente == null)
                    {
                        retornoImportacao.Retornolinhas.Add(ObterFalhaLinha("Remetente informado não existe na base multisoftware", i));
                        continue;
                    }
                }

                Dominio.Entidades.Cliente destinatario = null;
                if (colCodigoDestinatario != null)
                {
                    destinatario = repCliente.BuscarPorCodigoIntegracao(colCodigoDestinatario.Valor);
                    if (destinatario == null)
                    {
                        retornoImportacao.Retornolinhas.Add(ObterFalhaLinha("Destinatário informado não existe na base multisoftware", i));
                        continue;
                    }
                }

                Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoDeCarga = null;
                if (colTipoCarga != null && !string.IsNullOrEmpty(colTipoCarga.Valor))
                {
                    tipoDeCarga = repTipoDeCarga.BuscarPorDescricao(colTipoCarga.Valor);
                    if (tipoDeCarga == null)
                    {
                        retornoImportacao.Retornolinhas.Add(ObterFalhaLinha("Tipo de Carga informado não existe na base multisoftware", i));
                        continue;
                    }
                }

                TimeSpan horaInicio1 = TimeSpan.MinValue;
                if (colHoraInicio1 != null)
                {
                    horaInicio1 = ParseColunaTimeSpan(colHoraInicio1, retornoImportacao, i);
                    if (horaInicio1 == null) continue;
                }

                TimeSpan horaFim1 = TimeSpan.MinValue;
                if (colHoraFim1 != null)
                {
                    horaFim1 = ParseColunaTimeSpan(colHoraFim1, retornoImportacao, i);
                    if (horaFim1 == null) continue;
                }

                TimeSpan horaInicio2 = TimeSpan.MinValue;
                if (colHoraInicio2 != null && colHoraInicio2?.Valor != null)
                {
                    horaInicio2 = ParseColunaTimeSpan(colHoraInicio2, retornoImportacao, i);
                    if (horaInicio2 == null) continue;
                }

                TimeSpan horaFim2 = TimeSpan.MinValue;
                if (colHoraFim2 != null && colHoraFim2?.Valor != null)
                {
                    horaFim2 = ParseColunaTimeSpan(colHoraFim2, retornoImportacao, i);
                    if (horaFim2 == null) continue;
                }

                TimeSpan horaInicio3 = TimeSpan.MinValue;
                if (colHoraInicio3 != null && colHoraInicio3?.Valor != null)
                {
                    horaInicio3 = ParseColunaTimeSpan(colHoraInicio3, retornoImportacao, i);
                    if (horaInicio3 == null) continue;
                }

                TimeSpan horaFim3 = TimeSpan.MinValue;
                if (colHoraFim3 != null && colHoraFim3?.Valor != null)
                {
                    horaFim3 = ParseColunaTimeSpan(colHoraFim3, retornoImportacao, i);
                    if (horaFim3 == null) continue;
                }

                bool descargaDomingo = colDomingo != null && colDomingo?.Valor == "S";
                bool descargaSegunda = colSegunda != null && colSegunda?.Valor == "S";
                bool descargaTerca = colTerca != null && colTerca?.Valor == "S";
                bool descargaQuarta = colQuarta != null && colQuarta?.Valor == "S";
                bool descargaQuinta = colQuinta != null && colQuinta?.Valor == "S";
                bool descargaSexta = colSexta != null && colSexta?.Valor == "S";
                bool descargaSabado = colSabado != null && colSabado?.Valor == "S";

                List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> modelosVeiculares = new List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>();
                Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicular = null;

                if (colModeloVeiculo1 != null && !string.IsNullOrEmpty(colModeloVeiculo1.Valor))
                {
                    modeloVeicular = ParseColunaModeloVeiculo(colModeloVeiculo1, repModeloVeicularCarga, retornoImportacao, i);
                    if (modeloVeicular == null) continue;
                    else modelosVeiculares.Add(modeloVeicular);
                }

                if (colModeloVeiculo2 != null && !string.IsNullOrEmpty(colModeloVeiculo2.Valor))
                {
                    modeloVeicular = ParseColunaModeloVeiculo(colModeloVeiculo2, repModeloVeicularCarga, retornoImportacao, i);
                    if (modeloVeicular == null) continue;
                    else modelosVeiculares.Add(modeloVeicular);
                }

                if (colModeloVeiculo3 != null && !string.IsNullOrEmpty(colModeloVeiculo3.Valor))
                {
                    modeloVeicular = ParseColunaModeloVeiculo(colModeloVeiculo3, repModeloVeicularCarga, retornoImportacao, i);
                    if (modeloVeicular == null) continue;
                    else modelosVeiculares.Add(modeloVeicular);
                }

                if (colModeloVeiculo4 != null && !string.IsNullOrEmpty(colModeloVeiculo4.Valor))
                {
                    modeloVeicular = ParseColunaModeloVeiculo(colModeloVeiculo4, repModeloVeicularCarga, retornoImportacao, i);
                    if (modeloVeicular == null) continue;
                    else modelosVeiculares.Add(modeloVeicular);
                }

                if (colModeloVeiculo5 != null && !string.IsNullOrEmpty(colModeloVeiculo5.Valor))
                {
                    modeloVeicular = ParseColunaModeloVeiculo(colModeloVeiculo5, repModeloVeicularCarga, retornoImportacao, i);
                    if (modeloVeicular == null) continue;
                    else modelosVeiculares.Add(modeloVeicular);
                }

                if (colModeloVeiculo6 != null && !string.IsNullOrEmpty(colModeloVeiculo6.Valor))
                {
                    modeloVeicular = ParseColunaModeloVeiculo(colModeloVeiculo6, repModeloVeicularCarga, retornoImportacao, i);
                    if (modeloVeicular == null) continue;
                    else modelosVeiculares.Add(modeloVeicular);
                }

                if (colModeloVeiculo7 != null && !string.IsNullOrEmpty(colModeloVeiculo7.Valor))
                {
                    modeloVeicular = ParseColunaModeloVeiculo(colModeloVeiculo7, repModeloVeicularCarga, retornoImportacao, i);
                    if (modeloVeicular == null) continue;
                    else modelosVeiculares.Add(modeloVeicular);
                }

                if (colModeloVeiculo8 != null && !string.IsNullOrEmpty(colModeloVeiculo8.Valor))
                {
                    modeloVeicular = ParseColunaModeloVeiculo(colModeloVeiculo8, repModeloVeicularCarga, retornoImportacao, i);
                    if (modeloVeicular == null) continue;
                    else modelosVeiculares.Add(modeloVeicular);
                }

                if (colModeloVeiculo9 != null && !string.IsNullOrEmpty(colModeloVeiculo9.Valor))
                {
                    modeloVeicular = ParseColunaModeloVeiculo(colModeloVeiculo9, repModeloVeicularCarga, retornoImportacao, i);
                    if (modeloVeicular == null) continue;
                    else modelosVeiculares.Add(modeloVeicular);
                }

                if (colModeloVeiculo10 != null && !string.IsNullOrEmpty(colModeloVeiculo10.Valor))
                {
                    modeloVeicular = ParseColunaModeloVeiculo(colModeloVeiculo10, repModeloVeicularCarga, retornoImportacao, i);
                    if (modeloVeicular == null) continue;
                    else modelosVeiculares.Add(modeloVeicular);
                }

                unitOfWork.Start();
                try
                {
                    Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamento = repCentroDescarregamento.BuscarPorDestinatario(destinatario.Codigo);
                    if (centroDescarregamento == null)
                    {
                        centroDescarregamento = new Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento()
                        {
                            Descricao = destinatario.Descricao,
                            Destinatario = destinatario,
                            Ativo = true

                        };
                        repCentroDescarregamento.Inserir(centroDescarregamento);
                    };


                    if (destinatariosExlcuidos.Find(x => x.Codigo == destinatario.Codigo) == null)
                    {
                        List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento> periodos = repPeriodoDescarregamento.BuscarPorCentroDescarregamento(centroDescarregamento.Codigo);

                        foreach (var periodo in periodos)
                            repPeriodoDescarregamento.Deletar(periodo);

                        destinatariosExlcuidos.Add(destinatario);
                    }

                    InserirPeriodoDescarregamento(centroDescarregamento, destinatario, remetente, tipoDeCarga, horaInicio1, horaFim1, descargaDomingo, descargaSegunda, descargaTerca, descargaQuarta, descargaQuinta, descargaSexta, descargaSabado, unitOfWork);

                    if (horaInicio2 != TimeSpan.MinValue && horaFim2 != TimeSpan.MinValue)
                        InserirPeriodoDescarregamento(centroDescarregamento, destinatario, remetente, tipoDeCarga, horaInicio2, horaFim2, descargaDomingo, descargaSegunda, descargaTerca, descargaQuarta, descargaQuinta, descargaSexta, descargaSabado, unitOfWork);

                    if (horaInicio3 != TimeSpan.MinValue && horaFim3 != TimeSpan.MinValue)
                        InserirPeriodoDescarregamento(centroDescarregamento, destinatario, remetente, tipoDeCarga, horaInicio3, horaFim3, descargaDomingo, descargaSegunda, descargaTerca, descargaQuarta, descargaQuinta, descargaSexta, descargaSabado, unitOfWork);

                    centroDescarregamento.VeiculosPermitidos = modelosVeiculares;
                    repCentroDescarregamento.Atualizar(centroDescarregamento);

                    unitOfWork.CommitChanges();
                    retornoImportacao.Retornolinhas.Add(ObterSucessoLinha("", i));
                    contador++;
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();

                    retornoImportacao.Retornolinhas.Add(ObterFalhaLinha("Erro ao gravar dados", i));
                    Servicos.Log.TratarErro(ex);
                }

            }

            retornoImportacao.MensagemAviso = "Importação finalizada";
            retornoImportacao.Total = linhas.Count();
            retornoImportacao.Importados = contador;

            return retornoImportacao;

        }

        public static Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao ImportarCapacidadeDescarregamento(string dados, Dominio.Entidades.Usuario usuario, List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes, Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, int codigoCentroCarregamento, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha>>(dados);
            Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao();
            retornoImportacao.Retornolinhas = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha>();

            Repositorio.Embarcador.Logistica.CentroDescarregamento repCentroDescarregamento = new Repositorio.Embarcador.Logistica.CentroDescarregamento(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoDeCarga repTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
            Repositorio.Embarcador.Produtos.GrupoProduto repGrupoProduto = new Repositorio.Embarcador.Produtos.GrupoProduto(unitOfWork);
            Repositorio.Embarcador.Logistica.PeriodoDescarregamento repPeriodoDescarregamento = new Repositorio.Embarcador.Logistica.PeriodoDescarregamento(unitOfWork);
            Repositorio.Embarcador.Logistica.PeriodoDescarregamentoRemetente repPeriodoDescarregamentoRemetente = new Repositorio.Embarcador.Logistica.PeriodoDescarregamentoRemetente(unitOfWork);
            Repositorio.Embarcador.Logistica.PeriodoDescarregamentoGrupoPessoa repPeriodoDescarregamentoGrupoPessoa = new Repositorio.Embarcador.Logistica.PeriodoDescarregamentoGrupoPessoa(unitOfWork);
            Repositorio.Embarcador.Logistica.PeriodoDescarregamentoTipoDeCarga repPeriodoDescarregamentoTipoDeCarga = new Repositorio.Embarcador.Logistica.PeriodoDescarregamentoTipoDeCarga(unitOfWork);
            Repositorio.Embarcador.Logistica.PeriodoDescarregamentoGrupoProduto repPeriodoDescarregamentoGrupoProduto = new Repositorio.Embarcador.Logistica.PeriodoDescarregamentoGrupoProduto(unitOfWork);

            Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamento = repCentroDescarregamento.BuscarPorCodigo(codigoCentroCarregamento);
            if (centroDescarregamento == null)
                return new Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao()
                {
                    Importados = 0,
                    MensagemAviso = "Não foi encontrado um centro de descarregamento",
                    RegistrosAlterados = 0
                };

            int contador = 0;
            List<(int, int)> arr = new List<(int, int)> { };

            for (int i = 0; i < linhas.Count; i++)
            {
                bool falha = false;
                var linha = linhas[i];
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDiaDoMes = (from obj in linha.Colunas where obj.NomeCampo == "DiaDoMes" select obj).FirstOrDefault();
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colMes = (from obj in linha.Colunas where obj.NomeCampo == "Mes" select obj).FirstOrDefault();
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colInicioJanela = (from obj in linha.Colunas where obj.NomeCampo == "InicioJanela" select obj).FirstOrDefault();
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colFimJanela = (from obj in linha.Colunas where obj.NomeCampo == "FimJanela" select obj).FirstOrDefault();
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTipoCarga = (from obj in linha.Colunas where obj.NomeCampo == "TipoCarga" select obj).FirstOrDefault();
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colRemetente = (from obj in linha.Colunas where obj.NomeCampo == "RemetenteCodigoIntegracao" select obj).FirstOrDefault();
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colGrupoPessoa = (from obj in linha.Colunas where obj.NomeCampo == "GrupoPessoasCodigoIntegracao" select obj).FirstOrDefault();
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colGrupoProduto = (from obj in linha.Colunas where obj.NomeCampo == "GrupoProdutoCodigoIntegracao" select obj).FirstOrDefault();
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDescarregamentoSimultaneo = (from obj in linha.Colunas where obj.NomeCampo == "DescarregamentoSimultaneo" select obj).FirstOrDefault();
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colQuantidadeItensDe = (from obj in linha.Colunas where obj.NomeCampo == "QuantidadeItensDe" select obj).FirstOrDefault();
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colQuantidadeItensAte = (from obj in linha.Colunas where obj.NomeCampo == "QuantidadeItensAte" select obj).FirstOrDefault();

                int diaDoMes = 0;
                if (colDiaDoMes != null)
                {
                    int.TryParse((string)colDiaDoMes.Valor, out diaDoMes);
                    if (diaDoMes == 0 || diaDoMes > 31)
                    {
                        retornoImportacao.Retornolinhas.Add(ObterFalhaLinha("Dia não informado ou inexistente", i));
                        continue;
                    }
                }
                else
                {
                    retornoImportacao.Retornolinhas.Add(ObterFalhaLinha("Dia não informado", i));
                    continue;
                }

                int mes = 0;
                if (colMes != null)
                {
                    int.TryParse((string)colMes.Valor, out mes);
                    if (mes == 0 || mes > 12)
                    {
                        retornoImportacao.Retornolinhas.Add(ObterFalhaLinha("Mês não informado ou inexistente", i));
                        continue;
                    }
                }
                else
                {
                    retornoImportacao.Retornolinhas.Add(ObterFalhaLinha("Mês não informado", i));
                    continue;
                }

                List<Dominio.Entidades.Cliente> listaRemetente = new List<Dominio.Entidades.Cliente>();
                if (colRemetente != null)
                {
                    string[] listaCodigos = colRemetente.Valor.Split(',');
                    foreach (string codigo in listaCodigos)
                    {
                        Dominio.Entidades.Cliente remetente = repCliente.BuscarPorCodigoIntegracao(codigo.Trim());
                        if (remetente == null)
                        {
                            retornoImportacao.Retornolinhas.Add(ObterFalhaLinha("Remetente informado não existe na base multisoftware", i));
                            falha = true;
                            continue;
                        }
                        listaRemetente.Add(remetente);
                    }
                    if (falha)
                        continue;
                }

                List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas> listaGrupoPessoa = new List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas>();
                if (colGrupoPessoa != null && colGrupoPessoa.Valor != null)
                {
                    string[] listaCodigos = colGrupoPessoa.Valor.Split(',');
                    foreach (string codigo in listaCodigos)
                    {
                        Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoa = repGrupoPessoas.BuscarPorCodigoIntegracao(codigo.Trim());
                        if (grupoPessoa == null)
                        {
                            retornoImportacao.Retornolinhas.Add(ObterFalhaLinha("Grupo de Pessoas informado não existe na base multisoftware", i));
                            falha = true;
                            continue;
                        }
                        listaGrupoPessoa.Add(grupoPessoa);
                    }
                    if (falha)
                        continue;
                }

                List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> listaTipoDeCarga = new List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>();
                if (colTipoCarga != null && !string.IsNullOrEmpty(colTipoCarga.Valor))
                {
                    string[] listaCodigos = colTipoCarga.Valor.Split(',');
                    foreach (string codigo in listaCodigos)
                    {
                        Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoDeCarga = repTipoDeCarga.BuscarPorCodigoEmbarcador(codigo.Trim());
                        if (tipoDeCarga == null)
                        {
                            retornoImportacao.Retornolinhas.Add(ObterFalhaLinha("Tipo de Carga informado não existe na base multisoftware", i));
                            falha = true;
                            continue;
                        }
                        listaTipoDeCarga.Add(tipoDeCarga);
                    }
                    if (falha)
                        continue;
                }


                List<Dominio.Entidades.Embarcador.Produtos.GrupoProduto> listaGrupoProduto = new List<Dominio.Entidades.Embarcador.Produtos.GrupoProduto>();
                if (colGrupoProduto != null && !string.IsNullOrEmpty(colGrupoProduto.Valor))
                {
                    string[] listaCodigos = colGrupoProduto.Valor.Split(',');
                    foreach (string codigo in listaCodigos)
                    {
                        Dominio.Entidades.Embarcador.Produtos.GrupoProduto grupoProduto = repGrupoProduto.BuscarPorCodigoEmbarcador(codigo.Trim());
                        if (grupoProduto == null)
                        {
                            retornoImportacao.Retornolinhas.Add(ObterFalhaLinha("Grupo de Produto informado não existe na base multisoftware", i));
                            falha = true;
                            continue;
                        }
                        listaGrupoProduto.Add(grupoProduto);
                    }
                    if (falha)
                        continue;
                }


                int descarregamentoSimultaneo = 0;
                if (colDescarregamentoSimultaneo != null)
                {
                    int.TryParse((string)colDescarregamentoSimultaneo.Valor, out descarregamentoSimultaneo);
                    if (descarregamentoSimultaneo == 0)
                    {
                        retornoImportacao.Retornolinhas.Add(ObterFalhaLinha("Descarregamento Simultâneo não informado", i));
                        continue;
                    }
                }
                else
                {
                    retornoImportacao.Retornolinhas.Add(ObterFalhaLinha("Descarregamento Simultâneo não informado", i));
                    continue;
                }

                int quantidadeItensDe = 0;
                if (colQuantidadeItensDe != null)
                    int.TryParse((string)colQuantidadeItensDe.Valor, out quantidadeItensDe);

                int quantidadeItensAte = 0;
                if (colQuantidadeItensAte != null)
                    int.TryParse((string)colQuantidadeItensAte.Valor, out quantidadeItensAte);

                TimeSpan horaInicio = TimeSpan.MinValue;
                if (colInicioJanela != null)
                {
                    horaInicio = ParseColunaTimeSpan(colInicioJanela, retornoImportacao, i);
                    if (horaInicio == null) continue;
                }

                TimeSpan horaFim = TimeSpan.MinValue;
                if (colFimJanela != null)
                {
                    horaFim = ParseColunaTimeSpan(colFimJanela, retornoImportacao, i);
                    if (horaFim == null) continue;
                }

                unitOfWork.Start();
                try
                {

                    if (!arr.Contains((diaDoMes, mes)))
                    {
                        List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento> periodosDeletar = repPeriodoDescarregamento.BuscarPorDiaMes(codigoCentroCarregamento, diaDoMes, mes);
                        foreach (Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento periodo in periodosDeletar)
                            repPeriodoDescarregamento.Deletar(periodo);

                        arr.Add((diaDoMes, mes));
                    }

                    Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento periodoDescarregamento = new Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento()
                    {
                        Dia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana.Domingo,
                        DiaDoMes = diaDoMes,
                        Mes = mes,
                        CapacidadeDescarregamentoSimultaneo = descarregamentoSimultaneo,
                        HoraInicio = horaInicio,
                        HoraTermino = horaFim,
                        SkuDe = quantidadeItensDe,
                        SkuAte = quantidadeItensAte,
                        CentroDescarregamento = centroDescarregamento
                    };
                    repPeriodoDescarregamento.Inserir(periodoDescarregamento);

                    if (listaRemetente != null && listaRemetente.Count > 0)
                    {
                        foreach (Dominio.Entidades.Cliente remetente in listaRemetente)
                        {
                            Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoRemetente periodoDescarregamentoRemetente = new Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoRemetente()
                            {
                                PeriodoDescarregamento = periodoDescarregamento,
                                Remetente = remetente
                            };
                            repPeriodoDescarregamentoRemetente.Inserir(periodoDescarregamentoRemetente);
                        }
                    }

                    if (listaGrupoPessoa != null && listaGrupoPessoa.Count > 0)
                    {
                        foreach (Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoa in listaGrupoPessoa)
                        {
                            Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoGrupoPessoa periodoDescarregamentoGrupoPessoa = new Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoGrupoPessoa()
                            {
                                PeriodoDescarregamento = periodoDescarregamento,
                                GrupoPessoas = grupoPessoa
                            };
                            repPeriodoDescarregamentoGrupoPessoa.Inserir(periodoDescarregamentoGrupoPessoa);
                        }
                    }


                    if (listaTipoDeCarga != null && listaTipoDeCarga.Count > 0)
                    {
                        foreach (Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoDeCarga in listaTipoDeCarga)
                        {
                            Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoTipoDeCarga periodoDescarregamentoTipoDeCarga = new Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoTipoDeCarga()
                            {
                                PeriodoDescarregamento = periodoDescarregamento,
                                TipoDeCarga = tipoDeCarga
                            };
                            repPeriodoDescarregamentoTipoDeCarga.Inserir(periodoDescarregamentoTipoDeCarga);
                        }
                    }


                    if (listaGrupoProduto != null && listaGrupoProduto.Count > 0)
                    {
                        foreach (Dominio.Entidades.Embarcador.Produtos.GrupoProduto grupoProduto in listaGrupoProduto)
                        {
                            Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoGrupoProduto periodoDescarregamentoGrupoProduto = new Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoGrupoProduto()
                            {
                                PeriodoDescarregamento = periodoDescarregamento,
                                GrupoProduto = grupoProduto
                            };
                            repPeriodoDescarregamentoGrupoProduto.Inserir(periodoDescarregamentoGrupoProduto);
                        }
                    }

                    unitOfWork.CommitChanges();
                    retornoImportacao.Retornolinhas.Add(ObterSucessoLinha("", i));
                    contador++;
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();

                    retornoImportacao.Retornolinhas.Add(ObterFalhaLinha("Erro ao gravar dados", i));
                    Servicos.Log.TratarErro(ex);
                }

            }

            retornoImportacao.MensagemAviso = " Importação finalizada";
            retornoImportacao.Total = linhas.Count();
            retornoImportacao.Importados = contador;

            return retornoImportacao;

        }
        private static TimeSpan ParseColunaTimeSpan(Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna coluna, Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao, int indice)
        {
            TimeSpan hora = ObterHora(coluna.Valor);
            if (hora == null)
            {
                retornoImportacao.Retornolinhas.Add(ObterFalhaLinha($"{coluna.NomeCampo} inválido", indice));
            }
            return hora;
        }

        private static Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga ParseColunaModeloVeiculo(Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna coluna, Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga, Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao, int indice)
        {
            Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicular = repModeloVeicularCarga.buscarPorDescricao(coluna.Valor);
            if (modeloVeicular == null)
            {
                retornoImportacao.Retornolinhas.Add(ObterFalhaLinha($"{coluna.NomeCampo} informado não existe na base Multisoftware", indice));
            }
            return modeloVeicular;
        }

    }

}
