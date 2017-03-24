﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace NFeFacil
{
    internal abstract class Importacao
    {
        private string[] Extensão;

        public Importacao(params string[] extensão)
        {
            Extensão = extensão;
        }

        public async Task<StorageFile> ImportarArquivo()
        {
            var importar = CriarImportador();
            return await importar.PickSingleFileAsync();
        }

        public async Task<IReadOnlyList<StorageFile>> ImportarArquivos()
        {
            var importar = CriarImportador();
            return await importar.PickMultipleFilesAsync();
        }

        private FileOpenPicker CriarImportador()
        {
            var importar = new FileOpenPicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };
            foreach (var item in Extensão) importar.FileTypeFilter.Add(item);
            return importar;
        }
    }
}
