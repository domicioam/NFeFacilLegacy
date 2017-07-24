﻿using NFeFacil.View.CaixasDialogo;
using NFeFacil.ItensBD;
using NFeFacil.Log;
using NFeFacil.ModeloXML;
using NFeFacil.WebService;
using NFeFacil.WebService.Pacotes;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace NFeFacil
{
    public struct OperacoesNotaEmitida
    {
        public Processo Processo { get; set; }
        ILog Log { get; }

        public OperacoesNotaEmitida(Processo processo, ILog log = null)
        {
            Processo = processo;
            Log = log ?? Popup.Current;
        }

        public async Task<bool> Cancelar()
        {
            try
            {
                var estado = Processo.NFe.Informações.identificação.CódigoUF;
                var tipoAmbiente = Processo.ProtNFe.InfProt.tpAmb;

                var gerenciador = new GerenciadorGeral<EnvEvento, RetEnvEvento>(estado, Operacoes.RecepcaoEvento, tipoAmbiente == 2);

                var cnpj = Processo.NFe.Informações.emitente.CNPJ;
                var chave = Processo.NFe.Informações.ChaveAcesso;
                var nProtocolo = Processo.ProtNFe.InfProt.nProt;
                var versao = gerenciador.Enderecos.VersaoRecepcaoEvento;
                var entrada = new EntradaTexto("Cancelar NFe", "Motivo");

                if (await entrada.ShowAsync() == ContentDialogResult.Primary)
                {
                    if (!string.IsNullOrEmpty(entrada.Conteudo))
                    {
                        var infoEvento = new InformacoesEvento(estado, cnpj, chave, versao, nProtocolo, entrada.Conteudo, tipoAmbiente);
                        var envio = new EnvEvento(gerenciador.Enderecos.VersaoRecepcaoEvento, infoEvento);
                        await envio.PrepararEventos();
                        var resposta = await gerenciador.EnviarAsync(envio);
                        if (resposta.RetEvento[0].InfEvento.CStat == 135)
                        {
                            using (var contexto = new AplicativoContext())
                            {
                                contexto.Cancelamentos.Add(new RegistroCancelamento()
                                {
                                    ChaveNFe = chave,
                                    DataHoraEvento = resposta.RetEvento[0].InfEvento.DhRegEvento,
                                    TipoAmbiente = tipoAmbiente,
                                    XML = new ProcEventoCancelamento()
                                    {
                                        Eventos = envio.Eventos,
                                        RetEvento = resposta.RetEvento,
                                        Versao = resposta.Versao
                                    }.ToXElement<ProcEventoCancelamento>().ToString()
                                });
                                contexto.SaveChanges();
                            }
                            Log.Escrever(TitulosComuns.Sucesso, "NFe cancelada com sucesso.");
                            return true;
                        }
                        else
                        {
                            Log.Escrever(TitulosComuns.Erro, resposta.RetEvento[0].InfEvento.XMotivo);
                        }
                    }
                    else
                    {
                        Log.Escrever(TitulosComuns.Erro, "Nenhum motivo foi informado.");
                    }
                }
                return false;
            }
            catch (Exception e)
            {
                Log.Escrever(TitulosComuns.Erro, e.Message);
                return false;
            }
        }
    }
}
