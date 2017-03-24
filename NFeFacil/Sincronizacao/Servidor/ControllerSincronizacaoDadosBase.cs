﻿using NFeFacil.Sincronizacao.Pacotes;
using Restup.Webserver.Attributes;
using Restup.Webserver.Models.Contracts;
using Restup.Webserver.Models.Schemas;
using System;
using System.Threading.Tasks;

namespace NFeFacil.Sincronizacao.Servidor
{
    [RestController(InstanceCreationType.PerCall)]
    internal sealed class ControllerSincronizacaoDadosBase
    {
        [UriFormat("/Dados/POST/{senha}")]
        public async Task<IPostResponse> ClienteServidorAsync(int senha, [FromContent] DadosBase pacote)
        {
            return await SupervisionarOperacao.Iniciar(async () =>
            {
                if (senha != Configuracoes.ConfiguracoesSincronizacao.SenhaPermanente)
                    throw new SenhaErrada(senha);
                await ProcessamentoDadosBase.SalvarAsync(pacote);
                return new PostResponse(PostResponse.ResponseStatus.Created);
            }, pacote.HoraRequisição, TipoDado.DadoBase);
        }

        [UriFormat("/Dados/GET/{senha}")]
        public IGetResponse ServidorCliente(int senha)
        {
            return SupervisionarOperacao.Iniciar(() =>
            {
                if (senha != Configuracoes.ConfiguracoesSincronizacao.SenhaPermanente)
                    throw new SenhaErrada(senha);
                return new GetResponse(GetResponse.ResponseStatus.OK, ProcessamentoDadosBase.Obter());
            }, DateTime.Now, TipoDado.DadoBase);
        }
    }
}