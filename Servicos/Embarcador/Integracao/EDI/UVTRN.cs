using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Integracao.EDI
{
    public class UVTRN
    {
        public Dominio.ObjetosDeValor.EDI.UVTRN.UVTRN ConverterCargaMDFesParaEDIUVTRN(Dominio.Entidades.Embarcador.Cargas.CargaEDIIntegracao cargaEDIIntegracao, List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> cargaMDFes)
        {
            Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaEDIIntegracao.Carga;
            Dominio.Entidades.Usuario motorista = carga.Motoristas.FirstOrDefault();
            Dominio.Entidades.Veiculo veiculo = carga.Veiculo;

            Dominio.ObjetosDeValor.EDI.UVTRN.UVTRN ediUVTRN = new Dominio.ObjetosDeValor.EDI.UVTRN.UVTRN()
            {
                CPFMotorista = motorista.CPF,
                InscricaoEstadual = carga.Empresa.InscricaoEstadual,
                Estado = carga.Empresa.Localidade.Estado.Sigla,
                DataHoraLancamento = cargaEDIIntegracao.DataIntegracao,
                DataHoraBatch = DateTime.Now,
                Placa = veiculo?.Placa,
                MDFes = new List<Dominio.ObjetosDeValor.EDI.UVTRN.MDFe>()
            };

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaMDFe cargaMDFe in cargaMDFes)
            {
                Dominio.ObjetosDeValor.EDI.UVTRN.MDFe mdfe = new Dominio.ObjetosDeValor.EDI.UVTRN.MDFe()
                {
                    Numero = cargaMDFe.MDFe.Numero,
                    NotasFiscais = new List<Dominio.ObjetosDeValor.EDI.UVTRN.NotaFiscal>()
                };

                foreach (Dominio.Entidades.MunicipioDescarregamentoMDFe municipioDescarregamento in cargaMDFe.MDFe.MunicipiosDescarregamento)
                {
                    foreach (Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe documentoMunicipioDescarregamento in municipioDescarregamento.Documentos)
                    {
                        if (documentoMunicipioDescarregamento.CTe == null)
                            continue;

                        foreach (Dominio.Entidades.DocumentosCTE documentoCTe in documentoMunicipioDescarregamento.CTe.Documentos)
                        {
                            if (string.IsNullOrWhiteSpace(documentoCTe.ChaveNFE))
                                continue;

                            mdfe.NotasFiscais.Add(new Dominio.ObjetosDeValor.EDI.UVTRN.NotaFiscal()
                            {
                                Chave = documentoCTe.ChaveNFE
                            });
                        }
                    }
                }

                ediUVTRN.MDFes.Add(mdfe);
            }

            return ediUVTRN;
        }
    }
}
