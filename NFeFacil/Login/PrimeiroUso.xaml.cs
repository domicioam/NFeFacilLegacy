﻿using Newtonsoft.Json;
using NFeFacil.Log;
using NFeFacil.PacotesBanco;
using NFeFacil.Sincronizacao;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

// O modelo de item de Página em Branco está documentado em https://go.microsoft.com/fwlink/?LinkId=234238

namespace NFeFacil.Login
{
    [View.DetalhePagina(Symbol.Emoji, "Bem-vindo")]
    public sealed partial class PrimeiroUso : Page
    {
        public PrimeiroUso()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
            {
                using (var repo = new Repositorio.Leitura())
                {
                    if (repo.EmitentesCadastrados)
                    {
                        await Task.Delay(500);
                        MainPage.Current.Navegar<EscolhaEmitente>();
                    }
                }
            }
        }

        void Manualmente(object sender, TappedRoutedEventArgs e) => MainPage.Current.Navegar<AdicionarEmitente>();
        void Sincronizar(object sender, TappedRoutedEventArgs e) => MainPage.Current.Navegar<SincronizacaoCliente>();
        async void RestaurarBackup(object sender, TappedRoutedEventArgs e)
        {
            var caixa = new FileOpenPicker();
            caixa.FileTypeFilter.Add(".json");
            var arq = await caixa.PickSingleFileAsync();
            if (arq != null)
            {
                var stream = await arq.OpenStreamForReadAsync();
                using (var leitor = new StreamReader(stream))
                {
                    try
                    {
                        var texto = await leitor.ReadToEndAsync();
                        var conjunto = JsonConvert.DeserializeObject<ConjuntoBanco>(texto);
                        conjunto.AnalisarESalvar();
                        Popup.Current.Escrever(TitulosComuns.Sucesso, "Backup restaurado com sucesso.");

                        await Task.Delay(500);
                        MainPage.Current.Navegar<EscolhaEmitente>();
                    }
                    catch (Exception erro)
                    {
                        erro.ManipularErro();
                    }
                }
            }
        }
    }
}
