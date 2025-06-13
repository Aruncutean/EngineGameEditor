using Avalonia.Controls;
using Core.models;
using Core.services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EditorAvalonia.service
{
    public class DialogService
    {
        public Window window;

        private static DialogService instance = null;
        private DialogService()
        {
        }
        public async Task OpenFileDialog(string title, string[] filters, Action<string?> callback)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Selectează un folder",
            };
            var result = await dialog.ShowAsync(window);
            if (result.Length > 0)
            {
                callback(result[0]);
            }
            else
            {
                callback(null);
            }
        }

        public static DialogService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DialogService();
                }
                return instance;
            }
        }

    }
}
